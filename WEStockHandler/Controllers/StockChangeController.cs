using System;
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
        public async Task<ActionResult<IEnumerable<StockChangeWithProductNameModel>>> GetStockChangeModelByTimeAndTypeWithProductName(int from = 19000101, int to = 21001231, string type = "cart")
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }

            var fromDate = ConvertIntToDate(from);
            var ToDate = ConvertIntToDate(to).AddDays(1);

            var filteredStockChanges = _context.StockChangeModel
                .Join(_context.ProductModel,
                      stockChange => stockChange.ProductId,
                      product => product.Id,
                      (stockChange, product) => new
                      {
                          ProductId = stockChange.ProductId,
                          ProdductCode = product.ProductCode,
                          ProductName = product.Name,
                          ProductPrice = product.Price,
                          Quantity = stockChange.Quantity * -1,
                          DateTime = stockChange.DateTime,
                          StockChangeType = stockChange.StockChangeType
                      })
                .Where(obj => obj.DateTime >= fromDate && obj.DateTime < ToDate && obj.StockChangeType == type);

            var filteredStockChangeWithProductName = new List<StockChangeWithProductNameModel>();

            foreach (var change in filteredStockChanges)
            {
                var inTheList = false;
                            
                foreach (var product in filteredStockChangeWithProductName)
                    if (product.ProductId == change.ProductId)
                    {
                        inTheList = true;
                        product.Quantity += change.Quantity;
                        break;
                    }

                if (!inTheList)
                {
                    var withProductName = new StockChangeWithProductNameModel();

                    withProductName.ProductId = change.ProductId;
                    withProductName.ProductCode = change.ProdductCode;
                    withProductName.ProductName = change.ProductName;
                    withProductName.ProductPrice = change.ProductPrice;
                    withProductName.Quantity = change.Quantity;
                    withProductName.DateTime = change.DateTime;
                    withProductName.StockChangeType = change.StockChangeType;

                    filteredStockChangeWithProductName.Add(withProductName);
                }
            }

            return filteredStockChangeWithProductName;
        }

        [HttpGet("sum")]
        public int GetTodaySumSold()
        {
            var sum = 0;

            var today = DateTime.Today;

            var filteredStockChanges = _context.StockChangeModel
                .Join(_context.ProductModel,
                      stockChange => stockChange.ProductId,
                      product => product.Id,
                      (stockChange, product) => new
                        {
                            Id = stockChange.Id,
                            ProductName = product.Name,
                            ProductPrice =product.Price,
                            Quantity = stockChange.Quantity*-1,
                            DateTime = stockChange.DateTime,
                            StockChangeType = stockChange.StockChangeType
                        })
                .Where(obj => obj.DateTime >= today && obj.DateTime < today.AddDays(1) && obj.StockChangeType == "cart");

            foreach (var change in filteredStockChanges)
            {
                sum += change.Quantity*change.ProductPrice;
            }

            return sum;
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
                var stockChange = ConvertCartItemToStockChange(item, dateTime, "cart", -1);
                _context.StockChangeModel.Add(stockChange);
                await _context.SaveChangesAsync();
                await SavesStockChangeToProductTable(stockChange);
            }

            return Ok("Cart items are saved");
        }

        [HttpPost("storno")]
        public async Task<IActionResult> PostStornoCartItems(IEnumerable<CartItemModel> cartItems)
        {
            DateTime dateTime = DateTime.Now;
            foreach (var item in cartItems)
            {
                var stockChange = ConvertCartItemToStockChange(item, dateTime, "storno", 1);
                _context.StockChangeModel.Add(stockChange);
                await _context.SaveChangesAsync();
                await SavesStornoToProductTable(stockChange);
            }

            return Ok("Storno items are saved");
        }

        private async Task SavesStornoToProductTable(StockChangeModel stockChangeModel)
        {
            var productModel = await _context.ProductModel.FindAsync(stockChangeModel.ProductId);
            var lastProduct = await _context.ProductModel
                .Where(obj => obj.CartonCode == productModel.CartonCode)
                .OrderByDescending(obj => obj.Id)
                .ToListAsync();



            lastProduct[0].Quantity += stockChangeModel.Quantity;
            _context.Entry(lastProduct[0]).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        private StockChangeModel ConvertCartItemToStockChange(CartItemModel item, DateTime dateTime, string type, int multiplier)
        {
            StockChangeModel stockChange = new StockChangeModel
            {
                DateTime = dateTime,
                ProductId = item.ProductId,
                Quantity = item.Quantity*multiplier,
                StockChangeType = type
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
