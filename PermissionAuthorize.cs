using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminPanelCore.Models;

using AdminPanelCore.Settings;

namespace AdminPanelCore
{
    namespace AdminPanelCore
    {

        public class PermissionAuthorize : ActionFilterAttribute
        {
            public override void OnResultExecuting(ResultExecutingContext Context)
            {

            }
            public override void OnActionExecuting(ActionExecutingContext Context)
            {
                base.OnActionExecuting(Context);

                if (Context.HttpContext.Session.GetString("UserName") == null)
                {
                    Context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
                    return;
                }
                //tryb pracy
                Settings.Settings.CheckMode(Context.HttpContext);


                var menus = JsonConvert.DeserializeObject<List<string>>(Context.HttpContext.Session.GetString("permissions"));
                var permissions = JsonConvert.DeserializeObject<List<string>>(Context.HttpContext.Session.GetString("permissions"));

                var controllerName = Context.RouteData.Values["controller"].ToString();
                var actionName = Context.RouteData.Values["action"].ToString();
                //string controllerAction = controllerName + "." + actionName;

                if (!permissions.Where(s => s.Contains(controllerName) && s.Contains(actionName)).Any())
                
                {
                    Context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
                    return;
                }
            }
        }
    }

}