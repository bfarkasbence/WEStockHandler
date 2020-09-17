using System;

namespace WEStockHandler.Controllers
{
    public class AttendanceWithNameModel
    {
        public int Id { get; set; }
        public int ConsultantId { get; set; }
        public string ConsultantName { get; set; }
        public DateTime DateTime { get; set; }
}
}