using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Required")]
        public string OrderInformation { get; set; }
        public int UserId { get; set; }
    }
}
