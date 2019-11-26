using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AdminPanelCore.Data;
using AdminPanelCore.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using AdminPanelCore.MenuBuilder;
using System.Text;
using AdminPanelCore.AdminPanelCore;
using AdminPanelCore.ViewModels;

namespace AdminPanelCore.Controllers
{
   // [PermissionAuthorize]
    public class RolesController : Controller
    {
        SqlConnection connection = new SqlConnection(ConnectionString.connectionString);

        public IActionResult Index()
        {
            var roles = connection.Query<Role>(@"SELECT * FROM [ROLES]").ToList();
            return View(roles);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Role model)
        {
            if (ModelState.IsValid)
            {
                connection.Insert(new Role
                {
                    RoleName = model.RoleName,
                    Description = model.Description
                });
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Roles/Edit/5
        public IActionResult Edit(int? id)
        {

            var role = connection.Query<Role>(@"SELECT * FROM ROLES WHERE RoleId = @id", new { id }).FirstOrDefault();

            if (id == null)
            {
                return NotFound();
            }

            if (role == null)
            {
                return NotFound();
            }

            DataSet ds = new DataSet();

            List<string> permission_id = connection.Query<RoleMenu>(@"SELECT MenuId FROM [RoleMenus] WHERE RoleId = @id", new { id }).Select(s => s.MenuId.ToString()).ToList();
            ds = TreeBuilder.ToDataSet(connection.Query<Menu>("SELECT * FROM [Menus]").ToList());
            DataTable table = ds.Tables[0];
            DataRow[] parentMenus = table.Select("ParentId = 0");

            var sb = new StringBuilder();
            string unorderedList = TreeBuilder.TreeGenerator(parentMenus, table, sb, permission_id);
            ViewBag.menu = unorderedList;

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("RoleId,RoleName,Description")] Role roles)
        {
            if (id != roles.RoleId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    connection.Update(roles);
                }
                catch
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(roles);
        }

        [HttpPost]
        public IActionResult Update(int id, List<int> roles)
        {
            List<RoleMenu> temp = connection.Query<RoleMenu>(@"SELECT * FROM [RoleMenus] WHERE RoleId = @id", new { id }).ToList();
            connection.Delete(temp);

            foreach (var role in roles)
            {
                connection.Insert(new RoleMenu { MenuId = role, RoleId = id });
            }
            return Json(new { status = true, message = "Successfully Updated Role!" });
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var role = connection.Query<Role>(@"SELECT * FROM ROLES WHERE RoleId = @id", new { id }).FirstOrDefault();
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var role = connection.Query<Role>(@"SELECT * FROM ROLES WHERE RoleId = @id", new { id }).FirstOrDefault();
            try
            {
                List<RoleMenu> temp = connection.Query<RoleMenu>(@"SELECT * FROM [RoleMenus] WHERE RoleId = @id", new { id }).ToList();
                connection.Delete(temp);
                connection.Delete(role);
            }
            catch
            {
                // nie mozna usunąc roli, która ma przypisanego uzytkownika
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Permissions(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string sql = $"SELECT P.PermissionId,P.Description,P.OptionName, URP.SourceId, CASE WHEN URP.SourceId IS NULL THEN 0 ELSE 1 END AS Active FROM [Permissions] P LEFT JOIN [UserRolePermissions] URP on P.PermissionId = URP.PermissionId AND URP.TypeName = 'Role' AND URP.SourceId = {id} LEFT JOIN [Roles] R on R.RoleId = URP.SourceId";
            var PermissionViewModel = connection.Query<PermissionViewModel>(sql).ToList();
            ViewBag.RoleId = id;
            return View(PermissionViewModel);
        }

        [HttpPost, ActionName("Permissions")]
        public IActionResult UpdatePermissions(int id, List<int> checkedPermission)
        {
            var temp = connection.Query<UserRolePermission>($"SELECT * FROM [UserRolePermissions] WHERE SourceId = {id} AND TypeName = 'Roles'").ToList();
            connection.Delete(temp);

            foreach (var permissionId in checkedPermission)
            {
                connection.Insert(new UserRolePermission { SourceId = id, TypeName = "Role", PermissionId = permissionId });
            }
            return Json(new { status = true, message = "Successfully Updated Role permissions!" });
        }

    }
}