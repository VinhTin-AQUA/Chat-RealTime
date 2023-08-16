using AuthApi.Interfaces;
using AuthApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotifyBotApi.Data;
using NotifyBotApi.Hubs;
using NotifyBotApi.Interfaces;
using NotifyBotApi.Models;
using NotifyBotApi.Models.MailService;
using NotifyBotApi.Repositories;
using NotifyBotApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ContextSeedService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IMessageChatRepository, MessageChatRepository>();
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<ResultErrorsObj>();
builder.Services.AddSingleton<ChatService>();

// enable cors
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", option => option.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


// sql server
builder.Services.AddDbContext<NotifyBotContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("NotifyBotConnectionString"));
});


// identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<NotifyBotContext>() // provide our context
    .AddDefaultTokenProviders() // create email for email confirmation
    .AddRoles<IdentityRole>() // be able to add roles
    .AddRoleManager<RoleManager<IdentityRole>>() // be able to make use of RoleManager
    .AddSignInManager<SignInManager<AppUser>>() // make use of sign in manager
    .AddUserManager<UserManager<AppUser>>(); // make use of user manager to create user

// configure user
builder.Services.Configure<IdentityOptions>(options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 5; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 0; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    //options.User.AllowedUserNameCharacters = null;

    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ " +
        "áãạàả" +
        "âấầậẫẩ" +
        "ăặắằẵẳ" +
        "éèẻẹẽ" +
        "êếểềệễ" +
        "íịỉĩì" +
        "ýỳỷỹỵ" +
        "úùủụũ" +
        "ưứủụữừ" +
        "óòỏọõ" +
        "ôốồổộỗ" +
        "ơớờởợỡ" +
        "đĐ";

    options.User.RequireUniqueEmail = true;  // Email là duy nhất
                                             // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;  // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    
    options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại

    // mặc định false, true => khi đăng ký => chuyển hướng đến trang RegisterConfirmation (xác thực email)
    options.SignIn.RequireConfirmedAccount = true;
});

// authenticate user using jwt
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        // validate the issuer (who ever is issuing the JWT)
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true, // validate token based on the key we have provided in appsetting.json

        // don't validate audience (angular side)
        ValidateAudience = false,

        //ValidAudience = builder.Configuration.GetSection("JWT:ValidAudience").Value,
        // the issuer which in here is the api project url
        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,

        // the issuer signin key based on JWT:Key
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Key").Value!))
    };
});

// định dạng lỗi gửi đến client
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = (actionContext) =>
    {
        var errors = actionContext.ModelState
            .Where(x => x.Value!.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage).ToArray();

        var toReturn = new
        {
            Errors = errors
        };

        return new BadRequestObjectResult(toReturn);
    };
});

// mail service
var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddTransient<IEmailSender, EmailSender>();


builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// enable cors
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["JWT:UrlClient"]));

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapFallbackToController("Index", "Fallback");

using var scope = app.Services.CreateScope();
try
{
    var contextSeedService = scope.ServiceProvider.GetService<ContextSeedService>();
    await contextSeedService!.InitializeContextAsync();
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
    logger.LogError("Eror: " + ex.InnerException, ex.InnerException);
}

app.MapHub<ChatHub>("/hubs/chat");

app.Run();
