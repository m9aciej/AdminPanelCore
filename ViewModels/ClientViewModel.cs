using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.ViewModels
{
    public class ClientViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public int UserParentId { get; set; }
	    public int UserChildId { get; set; }
    }
}
