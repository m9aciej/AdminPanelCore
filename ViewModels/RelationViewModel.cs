using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.ViewModels
{
    public class RelationViewModel
    {
        //REL.RelationId, REL.Name, Rel.MasterRoleId, Rel.SlaveRoleId, ROL1.RoleName AS MasterRoleName, ROL2.RoleName AS SlaveRoleName
        public int RelationId { get; set; }
        public string Name { get; set; }
        public int MasterRoleId { get; set; }
        public int SlaveRoleId { get; set; }
        public string MasterRoleName { get; set; }
        public string SlaveRoleName { get; set; }
    }
}
