using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEStockHandler.Models
{
    public class BtbSentProductsModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime DateTime { get; set; }
        public int SentQuantity { get; set; }
        public string Status { get; set; }
    }
}
