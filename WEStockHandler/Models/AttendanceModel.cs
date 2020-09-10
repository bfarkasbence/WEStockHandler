using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEStockHandler.Models
{
    public class AttendanceModel
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int ConsultantId { get; set; }
    }
}
