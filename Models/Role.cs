using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.Models
{  
        public class Role
        {
            [Dapper.Contrib.Extensions.Key]
            public int RoleId { get; set; }
            [Required]
            public string RoleName { get; set; }
            [Required]
            public string Description { get; set; }

    }
    
}
