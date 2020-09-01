using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WEStockHandler.Models
{
    public class StockChangeModel
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string StockChangeType { get; set; }
    }
}
