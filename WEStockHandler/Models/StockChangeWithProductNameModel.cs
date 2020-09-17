using System;

namespace WEStockHandler.Controllers
{
    public class StockChangeWithProductNameModel
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int ProductPrice { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateTime { get; set; }
        public string StockChangeType { get; set; }
    }
}