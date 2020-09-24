using System;
using System.Collections.Generic;
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
    
    public class BtbSentProductsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BtbSentProductsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BtbSentProductsModel>>> GetBtnSentProductModel()
        {
            return await _context.BtbSentProductModel.ToListAsync();
        }

        [HttpGet("sent")]
        public async Task<ActionResult<IEnumerable<BtbSentProductsModel>>> GetSentProducts()
        {
            return await _context.BtbSentProductModel.Where(obj => obj.Status == "sent").ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BtbSentProductsModel>> GetBtbSentProductsModel(int id)
        {
            var btbSentProductsModel = await _context.BtbSentProductModel.FindAsync(id);

            if (btbSentProductsModel == null)
            {
                return NotFound();
            }

            return btbSentProductsModel;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBtbSentProductsModel(int id, BtbSentProductsModel btbSentProductsModel)
        {
            if (id != btbSentProductsModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(btbSentProductsModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BtbSentProductsModelExists(id))
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
        public async Task<ActionResult<BtbSentProductsModel>> PostBtbSentProductsModel(List<ProductFillUpModel> productsToFill)
        {
            DateTime dateTime = DateTime.Now;
            foreach (var product in productsToFill)
            {
                if (product.SendQuantity >= 0)
                {
                    var btbSentProduct = ConvertFillUpModelToBtbSentProductsModel(product, dateTime);
                    _context.BtbSentProductModel.Add(btbSentProduct);
                    await _context.SaveChangesAsync();
                }
            }

            return  Ok("Items saved");
        }

        private BtbSentProductsModel ConvertFillUpModelToBtbSentProductsModel(ProductFillUpModel item, DateTime dateTime)
        {
            BtbSentProductsModel btbProduct = new BtbSentProductsModel
            {
                ProductId = item.Id,
                DateTime = dateTime,
                SentQuantity = item.SendQuantity,
                Status = "sent"
            };

            return btbProduct;
        }

        private bool BtbSentProductsModelExists(int id)
        {
            return _context.BtbSentProductModel.Any(e => e.Id == id);
        }
    }
}
