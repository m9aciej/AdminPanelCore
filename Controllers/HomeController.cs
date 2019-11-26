using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AdminPanelCore.Models;
using Microsoft.AspNetCore.Http;
using AdminPanelCore.ViewModels;
using Microsoft.Data.SqlClient;
using AdminPanelCore.Data;
using Dapper;
using RestSharp;
using Newtonsoft.Json;

namespace AdminPanelCore.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection connection = new SqlConnection(ConnectionString.connectionString);
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return Redirect("/Account/Login");
            }

            // tryb pracy
            Settings.Settings.CheckMode(HttpContext);

            DashboardViewModel dashboard = new DashboardViewModel();

            dashboard.UsersCount = connection.Query<User>(@"SELECT * FROM [USERS]").Count();
            dashboard.RolesCount = connection.Query<Role>(@"SELECT * FROM [ROLES]").Count();

            //Weather API//
            string ApiCity = "http://ip-api.com/json";
            var client = new RestClient(ApiCity);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            Geo geo = JsonConvert.DeserializeObject<Geo>(response.Content);

            string Api = string.Format($"https://api.openweathermap.org/data/2.5/weather?q={geo.City}&appid=c1e9826fe7cbbcb53884ce4e27206e06&units=metric");
            
            client = new RestClient(Api);
            request = new RestRequest(Method.GET);
            response = client.Execute(request);
            string json = response.Content;

            var dataWeather = JsonConvert.DeserializeObject<DataWeather>(json);
            ViewBag.dataWeather = dataWeather;

            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
