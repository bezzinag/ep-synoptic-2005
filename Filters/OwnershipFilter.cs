using ep_synoptic_2005.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace ep_synoptic_2005.Filters
{
    public class OwnershipFilter : IAsyncActionFilter
    {
        private readonly IUploadFileRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public OwnershipFilter(IUploadFileRepository repository, UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("id", out var idObj) && idObj is int id)
            {
                var user = context.HttpContext.User;
                var userId = _userManager.GetUserId(user);

                var file = await _repository.GetByIdAsync(id);
                if (file == null || file.UploadedByUserId != userId)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            await next();
        }
    }
}
