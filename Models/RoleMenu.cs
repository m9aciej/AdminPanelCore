using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.Models
{
    public class RoleMenu
    {
        [Key]
        public int RoleMenuId {get; set;}
        public int MenuId { get; set; }
        public int RoleId { get; set; }

    }
}
