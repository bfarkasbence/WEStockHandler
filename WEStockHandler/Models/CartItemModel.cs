using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEStockHandler.Models
{
    public class CartItemModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int productPrice { get; set; }
        public int Quantity { get; set; }
    }
}
