using System;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AdminPanelCore.Models;
using Microsoft.Data.SqlClient;
using AdminPanelCore.Data;
using AdminPanelCore.AdminPanelCore;
using System.Collections.Generic;
using AdminPanelCore.ViewModels;
namespace AdminPanelCore.Controllers
{

    [PermissionAuthorize]
    public class UsersController : Controller
    {
        SqlConnection connection = new SqlConnection(ConnectionString.connectionString);

        //[UserAuthorize]
        // GET: Users
        public IActionResult Index()
        {

            string sql = "select * from [users] AS U INNER JOIN [ROLES] AS R on R.RoleId = U.RoleId";
            var users = connection.Query<User, Role, User>(sql, (User, Role) =>
            {
                User.Role = Role;
                return User;
            }, splitOn:"RoleId");

            return View(users);
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = "select * from [users] AS U INNER JOIN [ROLES] AS R on R.RoleId = U.RoleId";
            var user = connection.Query<User, Role, User>(sql, (User, Role) =>
            {
                User.Role = Role;
                return User;
            }, splitOn: "RoleId").Where(e => e.UserId == id).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        //[UserAuthorize]
        public ActionResult Create()
        {
            var roles = connection.Query<Role>("SELECT * FROM [ROLES]");
            ViewBag.Roles = new SelectList(roles, "RoleId", "RoleName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User model)
        {
            if (ModelState.IsValid)
            {
                var userValid = connection.Query<User>($"SELECT * FROM [USERS] WHERE Email = '{model.Email}' OR Username = '{model.UserName}'").FirstOrDefault();
                
                if(userValid != null) {
                    if (userValid.UserName == model.UserName)
                    {
                        ViewBag.ErrorUserName = "username is already in use";
                    }
                    if (userValid.Email == model.Email)
                    {
                        ViewBag.ErrorEmail = "email is already in use";
                    }
                    ViewBag.Roles = new SelectList(connection.Query<Role>("SELECT * FROM [ROLES]"), "RoleId", "RoleName", model.RoleId);
                    return View(model);
                }
                
                connection.Query<User>($"INSERT INTO [Users] (UserName, Password, RoleId, Email) VALUES('{model.UserName}','{model.Password}','{model.RoleId}','{model.Email}')");
                return RedirectToAction(nameof(Index));
            }

            var roles = connection.Query<Role>("SELECT * FROM [ROLES]");
            ViewBag.Roles = new SelectList(roles, "RoleId", "RoleName",model.RoleId);
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var user = connection.Query<User>("SELECT * FROM [USERS]").Where(e => e.UserId == id).FirstOrDefault(); ;
         
            if (user == null)
            {
                return NotFound();
            }

            var roles = connection.Query<Role>("SELECT * FROM [ROLES]");
            ViewBag.Roles = new SelectList(roles, "RoleId", "RoleName",user.RoleId);
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User model)
        {
            if (id != model.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
               
                try
                {
                    string sql = $"UPDATE [Users] SET UserName = '{model.UserName}', Password = '{model.Password}', RoleId = '{model.RoleId}', Email = '{model.Email}'  WHERE UserId = {model.UserId}; ";
                    connection.Query<User>(sql);
                }
                catch
                {
                    var userValid = connection.Query<User>($"SELECT * FROM [USERS] WHERE Email = '{model.Email}' OR Username = '{model.UserName}'").FirstOrDefault();

                    if (userValid != null)
                    {
                        if (userValid.UserName == model.UserName)
                        {
                            ViewBag.ErrorUserName = "username is already in use";
                        }
                        if (userValid.Email == model.Email)
                        {
                            ViewBag.ErrorEmail = "email is already in use";
                        }
                        ViewBag.Roles = new SelectList(connection.Query<Role>("SELECT * FROM [ROLES]"), "RoleId", "RoleName", model.RoleId);
                        return View(model);
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = new SelectList(connection.Query<Role>("SELECT * FROM [ROLES]"), "RoleId", "RoleName", model.RoleId);
            return View(model);
        }


        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = connection.Query<User>(@"SELECT * FROM [USERS] WHERE UserId = @id", new { id }).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = connection.Query<User>(@"SELECT * FROM [USERS] WHERE UserId = @id", new { id }).FirstOrDefault();
            connection.Delete(user);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Permissions(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = connection.Query<User>(@"SELECT RoleId, UserName FROM [USERS] WHERE UserId = @id", new { id }).FirstOrDefault();


            string sql = $"WITH SUB_ROLE(PermissionId, ActiveRole) AS " +
                $"(SELECT P.PermissionId, CASE WHEN URP.SourceId IS NULL THEN 0 ELSE 1 END AS ActiveRole " +
                $"FROM [Permissions] P LEFT JOIN [UserRolePermissions] URP on P.PermissionId = URP.PermissionId " +
                $"AND URP.TypeName = 'Role' AND URP.SourceId = {user.RoleId} LEFT JOIN [Users] U on U.UserId = URP.SourceId), " +
                $"SUB_USER(PermissionId, Description, OptionName, SourceId, Active) AS " +
                $"(SELECT P.PermissionId, P.Description, P.OptionName, URP.SourceId, CASE WHEN URP.SourceId IS NULL " +
                $"THEN 0 ELSE 1 END AS Active FROM [Permissions] P LEFT JOIN [UserRolePermissions] URP on P.PermissionId = URP.PermissionId " +
                $" AND URP.TypeName = 'User' AND URP.SourceId = {id} LEFT JOIN [Roles] R on R.RoleId = URP.SourceId) " +
                $"SELECT R.PermissionId, U.Description, U.OptionName, U.SourceId, U.Active, R.ActiveRole FROM SUB_USER as U INNER JOIN SUB_ROLE as R ON U.PermissionId = R.PermissionId;";

            var PermissionViewModel = connection.Query<PermissionViewModel>(sql).ToList();

            ViewBag.UserId = id;
            ViewBag.UserName = user.UserName;
            return View(PermissionViewModel);
        }

        [HttpPost, ActionName("Permissions")]
        public IActionResult UpdatePermissions(int id, List<int> checkedPermission)
        {
            var temp = connection.Query<UserRolePermission>($"SELECT * FROM [UserRolePermissions] WHERE SourceId = {id} AND TypeName = 'User'").ToList();
            connection.Delete(temp);

            foreach (var permissionId in checkedPermission)
            {
                connection.Insert(new UserRolePermission { SourceId = id, TypeName = "User", PermissionId = permissionId });
            }

            return Json(new { status = true, message = "Successfully Updated User permission!" });
        }



    }
}