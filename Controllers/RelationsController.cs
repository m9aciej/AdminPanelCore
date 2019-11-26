using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminPanelCore.AdminPanelCore;
using AdminPanelCore.Data;
using AdminPanelCore.Models;
using AdminPanelCore.ViewModels;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace AdminPanelCore.Controllers
{
    [PermissionAuthorize]
    public class RelationsController : Controller
    {
        SqlConnection connection = new SqlConnection(ConnectionString.connectionString);
        public IActionResult Index()
        {
            var relationViewModel = connection.Query<RelationViewModel>(@"SELECT REL.RelationId, REL.Name, Rel.MasterRoleId, Rel.SlaveRoleId, ROL1.RoleName AS MasterRoleName, ROL2.RoleName AS SlaveRoleName FROM [Relations] REL INNER JOIN [ROLES] ROL1 ON REL.MasterRoleId = ROL1.RoleId INNER JOIN [ROLES] ROL2 ON REL.SlaveRoleId = ROL2.RoleId").ToList();
            return View(relationViewModel);
        }

        public IActionResult Edit(int id)
        {
            var relation = connection.Query<Relation>($"SELECT * FROM [RELATIONS] WHERE RelationId = {id}").First();
            var masterUsers = connection.Query<User>($"SELECT UserName, UserId FROM [USERS] WHERE RoleId = {relation.MasterRoleId}").ToList();
            var slaveUsers = connection.Query<User>($"SELECT UserName, UserId FROM [USERS] WHERE RoleId = {relation.SlaveRoleId}").ToList();

            var relations = connection.Query<UserRelationViewModel>($"SELECT R.RelationsTreeId, U2.UserName AS MasterUserName, U.UserName AS ChildUserName FROM [RELATIONSTREES] R INNER JOIN [USERS] U ON R.UserChildId = U.UserId INNER JOIN [USERS] U2 ON R.UserParentId = U2.UserId where R.RelationId = {id}").ToList();

            ViewBag.relationId = relation.RelationId;
            ViewBag.masterUsers = new SelectList(masterUsers, "UserId", "UserName");
            ViewBag.slaveUsers = new SelectList(slaveUsers, "UserId", "UserName");

            return View(relations);
        }

        [HttpPost]
        public IActionResult Create (int id, List<int> checkedValues)
        {
            string query = "INSERT INTO RelationsTrees (UserChildId, UserParentId, RelationId) " +
                $"SELECT DISTINCT {checkedValues[1]},{checkedValues[0]},{id} WHERE   NOT EXISTS (" +
                $"SELECT RelationsTreeId FROM RelationsTrees WHERE UserChildId = {checkedValues[1]} AND UserParentId = {checkedValues[0]} );";
            
            connection.Query<int>(query);
            return Json(new { status = true, message = "Successfully Updated Relation!" });
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            connection.Query<int>($"DELETE FROM [RelationsTrees] WHERE RelationsTreeId = {id}");
            return RedirectToAction(nameof(Index));
        }

    }
}