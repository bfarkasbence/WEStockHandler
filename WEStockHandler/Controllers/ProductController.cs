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

    public class ProductController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ProductController(ApplicationContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProductModel()
        {
            return await _context.ProductModel.ToListAsync();
        }

        [HttpGet("date")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProductModelByTime(int from = 19000101, int to = 21001231)
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }

            var fromDate = ConvertIntToDate(from);
            var toDate = ConvertIntToDate(to).AddDays(1);

            var filteredProducts = _context.ProductModel.Where(obj => obj.FromDate >= fromDate && obj.ToDate < toDate);

            return filteredProducts.ToList();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProductModel(int id)
        {
            var productModel = await _context.ProductModel.FindAsync(id);

            if (productModel == null)
            {
                return NotFound();
            }

            return productModel;
        }



        
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyQuantityOfProductModel(int id, int quantity)
        { 

            var productModel = await _context.ProductModel.FindAsync(id);

            if (productModel == null)
            {
                return NotFound();
            }

            productModel.Quantity += quantity;


            _context.Entry(productModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPost]
        public async Task<ActionResult<ProductModel>> PostProductModel(ProductModel productModel)
        {
            _context.ProductModel.Add(productModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductModel", new { id = productModel.Id }, productModel);
        }

        private bool ProductModelExists(int id)
        {
            return _context.ProductModel.Any(e => e.Id == id);
        }

        private DateTime ConvertIntToDate(int dateNumber)
        {
            var date = new DateTime();

            date = DateTime.ParseExact(dateNumber.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

            return date;
        }
    }
}
