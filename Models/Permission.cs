using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.Models
{
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; }
        public string Description { get; set; }
        public string OptionName { get; set; }
    }

}
