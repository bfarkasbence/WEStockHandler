﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEStockHandler.Data;
using WEStockHandler.Models;

namespace WEStockHandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllHeaders")]
    public class StockChangeController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public StockChangeController(ApplicationContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockChangeModel>>> GetStockChangeModel()
        {
            return await _context.StockChangeModel.ToListAsync();
        }

        [HttpGet("date")]
        public async Task<ActionResult<IEnumerable<StockChangeModel>>> GetStockChangeModelByTimeAndType(int from = 19000101, int to = 21001231, string type = "cart")
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }

            var fromDate = ConvertIntToDate(from);
            var ToDate = ConvertIntToDate(to).AddDays(1);

            var filteredStockChanges = _context.StockChangeModel.Where(obj => obj.DateTime >= fromDate && obj.DateTime < ToDate && obj.StockChangeType == type);

            return filteredStockChanges.ToList();
        }
        

                
        [HttpPost]
        public async Task<ActionResult<StockChangeModel>> PostStockChangeModel(StockChangeModel stockChangeModel)
        {
            _context.StockChangeModel.Add(stockChangeModel);
            await _context.SaveChangesAsync();
            await SavesStockChangeToProductTable(stockChangeModel);

            return CreatedAtAction("GetStockChangeModel", new { id = stockChangeModel.Id }, stockChangeModel);
        }

        private async Task SavesStockChangeToProductTable(StockChangeModel stockChangeModel)
        {
            var productModel = await _context.ProductModel.FindAsync(stockChangeModel.ProductId);
            productModel.Quantity += stockChangeModel.Quantity;
            _context.Entry(productModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        [HttpPost("cart")]
        public async Task<IActionResult> PostCartItems(IEnumerable<CartItemModel> cartItems)
        {
            DateTime dateTime = DateTime.Now;
            foreach (var item in cartItems)
            {
                var stockChange = ConvertCartItemToStockChange(item, dateTime);
                _context.StockChangeModel.Add(stockChange);
                await _context.SaveChangesAsync();
                await SavesStockChangeToProductTable(stockChange);
            }

            return Ok("Cart items are saved");
        }

        private StockChangeModel ConvertCartItemToStockChange(CartItemModel item, DateTime dateTime)
        {
            StockChangeModel stockChange = new StockChangeModel
            {
                DateTime = dateTime,
                ProductId = item.ProductId,
                Quantity = item.Quantity*-1,
                StockChangeType = "cart"
            };

            return stockChange;
        }

        private bool StockChangeModelExists(int id)
        {
            return _context.StockChangeModel.Any(e => e.Id == id);
        }

        private DateTime ConvertIntToDate(int dateNumber)
        {
            var date = new DateTime();

            date = DateTime.ParseExact(dateNumber.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

            return date;
        }

    }
}
