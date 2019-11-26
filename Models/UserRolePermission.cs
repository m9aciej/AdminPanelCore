using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.Models
{
    public class UserRolePermission
    {
        [Key]
        public int UserRolePermissionId { get; set; }
        public int SourceId { get; set; }
        public string TypeName { get; set; }
        public int PermissionId { get; set; }
    }
}
