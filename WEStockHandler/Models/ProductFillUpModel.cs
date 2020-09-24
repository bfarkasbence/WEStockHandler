using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEStockHandler.Models
{
    public class ProductFillUpModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public int CartonCode { get; set; }
        public int RequiredQuantity { get; set; }
        public int SendQuantity { get; set; }

    }
}
