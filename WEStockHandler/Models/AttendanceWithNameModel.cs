using System;

namespace WEStockHandler.Controllers
{
    public class AttendanceWithNameModel
    {
        public int Id { get; set; }
        public string ConsulantName { get; set; }
        public DateTime DateTime { get; set; }
}
}