using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NotifyBotApi.Models;

namespace NotifyBotApi.Data;

public partial class NotifyBotContext : IdentityDbContext<AppUser>
{
    public NotifyBotContext(DbContextOptions<NotifyBotContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6));
            }
        }

        modelBuilder.Entity<GroupUser>()
            .HasKey(gu => new { gu.UserId, gu.GroupId });

        modelBuilder.Entity<GroupUser>()
            .HasOne<AppUser>(gu => gu.User)
            .WithMany(u => u.GroupUsers)
            .HasForeignKey(gu => gu.UserId);

        modelBuilder.Entity<GroupUser>()
            .HasOne<Group>(gu => gu.Group)
            .WithMany(g => g.GroupUsers)
            .HasForeignKey(gu => gu.GroupId);

        modelBuilder.Entity<MessageChat>()
            .HasOne<Group>(mc => mc.Group)
            .WithMany(g => g.MessageChats)
            .HasForeignKey(mc => mc.GroupId);
    }

    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupUser> GroupUsers { get; set; }
    public DbSet<MessageChat> MessageChats { get; set; }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
