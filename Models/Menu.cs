using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.Models
{
    public class Menu
    {
        [Key]
        public int MenuId {get; set;}
        public string Name {get; set;}
        public string Icon {get; set;}
        public string  Url {get; set;}
        public int ParentId {get; set;}
    }
}
