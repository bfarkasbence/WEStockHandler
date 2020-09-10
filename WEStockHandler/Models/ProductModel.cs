﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WEStockHandler.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public int CartonCode { get; set; }
        public int Price { get; set; }
        public int RequiredQuantity { get; set; }
        public int Quantity { get; set; }
        
    }    
}
