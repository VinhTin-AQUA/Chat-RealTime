using Microsoft.AspNetCore.Identity;

namespace NotifyBotApi.Services
{
    public class ResultErrorsObj
    {
        public object ToErrorObj(IEnumerable<IdentityError> errors) 
        {
            var errorArray = errors.Select(e => e.Description).ToArray();
            return new { errors = errorArray };
        }
    }
}
