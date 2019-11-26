using AdminPanelCore.Data;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.Settings
{
    public static class Settings
    {

        public static void CheckMode(HttpContext Context)
        {
            SqlConnection connection = new SqlConnection(ConnectionString.connectionString);

            var mode = connection.Query<bool>("SELECT Mode FROM [SETTINGS]").FirstOrDefault();
            var rolePermitted = connection.Query<int>("SELECT RolePermitted FROM [SETTINGS]").FirstOrDefault();

            if (mode == false && Context.Session.GetInt32("RoleId") != rolePermitted)
            {
                Context.Session.Clear();
                Context.Response.Redirect("/Account/Login");
            }
        }

    }
}
