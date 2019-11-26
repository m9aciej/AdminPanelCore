using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Required")]
        [System.ComponentModel.DataAnnotations.StringLength(50, ErrorMessage = "Max 50 characters")]
        public string UserName { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Required")]
        [System.ComponentModel.DataAnnotations.StringLength(50, ErrorMessage = "Max 50 characters")]
        [System.ComponentModel.DataAnnotations.RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "write email")]
        public string Email { get; set; }
        public int? RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
