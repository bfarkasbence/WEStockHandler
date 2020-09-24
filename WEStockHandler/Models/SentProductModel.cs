using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEStockHandler.Models
{
    public class SentProductModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int SentQuantity { get; set; }
        public int RecievedQuantity { get; set; }
    }
}
