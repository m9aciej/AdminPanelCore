using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AdminPanelCore.AdminPanelCore;
using AdminPanelCore.Data;
using AdminPanelCore.DataTableAjax;
using AdminPanelCore.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AdminPanelCore.Controllers
{
    //[PermissionAuthorize]
    public class OrdersController : Controller
    {
        SqlConnection connection = new SqlConnection(ConnectionString.connectionString);
        public IActionResult Index()
        {
           // int? userId = HttpContext.Session.GetInt32("UserId");

           // if (userId == null)
           // {
           //     return NotFound();
           // }
            var orders = connection.Query<Order>(@"SELECT * FROM [Orders]").ToList();

            return View(orders);
        }

        [HttpPost]
        public IActionResult Table(DataTableParameters data)
        {
            var x = new DataTableBootstrap(data);
            // Read value

            //var draw = Request.Form[("draw")][0];
            var draw = data.Draw;
            //int row = Convert.ToInt32(Request.Form["start"][0]);
            int row = data.Start;
            //int rowperpage = Convert.ToInt32(Request.Form["length"][0]);
            int rowperpage = data.Length;
            //int columnIndex = Convert.ToInt32(Request.Form[("order[0][column]")][0]);
            int columnIndex = data.Order[0].Column;
            //string columnName = Request.Form[($"columns[{columnIndex}][data]")][0];
            string columnName = data.Columns[columnIndex].Data;
            //string columnSortOrder = Request.Form[("order[0][dir]")][0];
            string columnSortOrder = data.Order[0].Dir;
            //string searchValue = Request.Form["search[value]"];
            string searchValue = data.Search.Value;

            // Search 
            var searchQuery = "";
            if (searchValue != "")
            { 
                 searchQuery = $"WHERE (OrderId LIKE '%{searchValue}%' OR OrderInformation LIKE '%{searchValue}%' OR UserId LIKE '%{searchValue}%')";
            }

            //Total number of records without filtering
            var totalRecords = connection.Query<int>("SELECT COUNT(*) AS allcount FROM [Orders]").First();

            //Total number of records with filtering
            var totalRecordwithFilter = connection.Query<int>($"SELECT COUNT(*) AS allcount FROM [Orders] {searchQuery}").First();

            //Get records
            var orders = connection.Query<Order>($"SELECT * FROM [Orders] {searchQuery} ORDER BY '{columnName}' {columnSortOrder} OFFSET {row} ROWS FETCH NEXT {rowperpage} ROWS ONLY").ToList();

            var response = new {
                draw = draw,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecordwithFilter,
                aaData = orders
            };
            return Json(response);
        }

        public IActionResult Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return NotFound();
            }
            ViewBag.UserId = userId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Order model)
        {
            if (ModelState.IsValid)
            {
                connection.Insert(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}