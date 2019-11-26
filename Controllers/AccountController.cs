using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminPanelCore.Data;
using AdminPanelCore.MenuBuilder;
using AdminPanelCore.Models;
using AdminPanelCore.Redis;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AdminPanelCore.Controllers
{
    public class AccountController : Controller
    {


        SqlConnection connection = new SqlConnection(ConnectionString.connectionString);
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                return Redirect("/Home");
            }

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User model)
        {
            var user = connection.Query<User>("SELECT * FROM [USERS]").Where(n => n.UserName == model.UserName).FirstOrDefault();

            if (user!=null)
            {
                if (user.Password == model.Password)
                {

                    HttpContext.Session.SetString("UserName", user.UserName);
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetInt32("UserId", (int)user.UserId);
                    HttpContext.Session.SetInt32("RoleId", (int)user.RoleId);
                    

                    int RoleId = (int)user.RoleId;
                    List<Menu> menu = connection.Query<Menu>($"SELECT  P.MenuId, P.ParentId, P.Name, P.Icon, P.Url FROM [MENUS] AS P INNER JOIN [ROLEMENUS] AS RP ON P.MenuId = RP.MenuId WHERE RP.RoleId = {RoleId}").ToList();

                    DataSet ds = new DataSet();
                    ds = TreeBuilder.ToDataSet(menu);
                    DataTable table = ds.Tables[0];
                    DataRow[] parentMenus = table.Select("ParentId = 0");
                    var sb = new StringBuilder();
                    string menuString = TreeBuilder.MenuGenerator(parentMenus, table, sb);
                    HttpContext.Session.SetString("menuString", menuString);

                    //ustawienia praw 
                    var permissions = connection.Query<Permission>($"(SELECT P.OptionName FROM [Permissions] P INNER JOIN [UserRolePermissions] URP ON P.PermissionId = URP.PermissionId AND URP.TypeName = 'Role' WHERE URP.SourceId = {user.RoleId}) UNION (SELECT P.OptionName FROM[Permissions] P INNER JOIN [UserRolePermissions] URP ON P.PermissionId = URP.PermissionId AND URP.TypeName = 'User' WHERE URP.SourceId = {user.UserId})").Select(e => e.OptionName);
                    HttpContext.Session.SetString("permissions", Newtonsoft.Json.JsonConvert.SerializeObject(permissions));



                    return Redirect("/Home");
                }
                else
                {
                    ViewBag.Error = "Wrong user name or password";
                    return View(model);
                }

            }
            else
            {
                ViewBag.Error = "Wrong user name or password";
                return View(model);
            }

        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


    }
}