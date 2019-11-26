using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminPanelCore.Data;
using AdminPanelCore.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AdminPanelCore.Controllers
{
    public class ClientsController : Controller
    { 
        public List<ClientViewModel> Filterr(List<ClientViewModel> list, int? parentId)
        {
            var c = new List<ClientViewModel>();
            while (parentId != 0)
            {
                c.AddRange(list.Where(e => e.UserParentId == parentId));
                parentId = (list.Where(e => e.UserParentId == parentId).Select(e => e.UserChildId).FirstOrDefault());
            }
            return c;
        }

        SqlConnection connection = new SqlConnection(ConnectionString.connectionString);
        public IActionResult Index()
        {

            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");

            var clientViewModel = connection.Query<ClientViewModel>($"exec ProcedureGetUsers {roleId}").ToList();
            var c = new List<ClientViewModel>();
           
            int? parentId = userId;

            while (parentId != 0) 
            {
                c.AddRange(clientViewModel.Where(e => e.UserParentId == parentId));

                if(clientViewModel.Where(e => e.UserParentId == parentId).Count() > 1) {
                    var parentIds = clientViewModel.Where(e => e.UserParentId == parentId).Select(e => e.UserChildId);
                    foreach (var el in parentIds)
                    {
                        c.AddRange(Filterr(clientViewModel, el));
                    }
                }
                parentId = (clientViewModel.Where(e => e.UserParentId == parentId).Select(e => e.UserChildId).FirstOrDefault());
            }

            return View(c.GroupBy(x => x.UserName).Select(x => x.First()));
        }



    }
}