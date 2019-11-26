using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.ViewModels
{
    public class PermissionViewModel
    {
        public int PermissionId{ get; set; }
        public string Description{ get; set; }
        public string OptionName { get; set; }
        public int SourceId { get; set; }
        public bool Active { get; set; }
        public bool ActiveRole { get; set; }
    }
}
