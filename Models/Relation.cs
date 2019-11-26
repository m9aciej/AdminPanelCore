using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.Models
{
    public class Relation
    {
        [Key]
        public int RelationId { get; set; }
        public string Name { get; set; }
        public int MasterRoleId { get; set; }
        public int SlaveRoleId { get; set; }
    }
}
