using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.Models
{
    public class RelationsTree
    {
        [Key]
        public int RelationsTreeId { get; set; }
        public int UserChildId { get; set; }
        public int UserParentId { get; set; }
        public int RelationId { get; set; }
    }
}
