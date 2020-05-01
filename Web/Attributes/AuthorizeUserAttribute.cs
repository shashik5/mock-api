using System;
using System.Threading.Tasks;
using CommonTypes;
using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserManagement.Models;
using Web.Data;

public class AuthorizeAttribute : TypeFilterAttribute
{
    public AuthorizeAttribute(UserAccountType accountType)
    : base(typeof(AuthorizeActionFilter))
    {
        Arguments = new object[] { accountType };
    }
}

public class AuthorizeActionFilter : IAsyncActionFilter
{
    private readonly UserAccountType _accountType;
    
    public AuthorizeActionFilter(UserAccountType accountType)
    {
        _accountType = accountType;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {

        IApp core = GlobalApplicationData.GetGlobalData<IApp>(GlobalDataKey.Core);
        var userManager = core.GetUserManager();
        bool isAuthorized = userManager.IsUserAuthorised(context.HttpContext.Request.Headers["authorization"], _accountType);

        if (!isAuthorized)
        {
            context.Result = new UnauthorizedResult();

        }
        else
        {
            await next();
        }
    }
}