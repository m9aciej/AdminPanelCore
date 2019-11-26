using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace AdminPanelCore.Helpers
{
    public static class MyHTMLHelpers
    {

        public static bool CheckPermission(this IHtmlHelper htmlHelper, string permission)
        {
            var permissions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<String>>(htmlHelper.ViewContext.HttpContext.Session.GetString("permissions"));
            foreach(var e in permissions)
            {
                if(e == permission)
                {
                    return true;
                }
            }
            return false;
        }
    }

}