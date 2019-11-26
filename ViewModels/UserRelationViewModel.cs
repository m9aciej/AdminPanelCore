using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.ViewModels
{
    public class UserRelationViewModel
    {
      
            public int RelationsTreeId { get; set; }
            public string MasterUserName { get; set; }
            public string ChildUserName { get; set; }
    }
}
