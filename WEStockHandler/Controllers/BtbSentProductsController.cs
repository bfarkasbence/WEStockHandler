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
        public async Task<ActionResult<IEnumerable<SentProductModel>>> GetSentProducts()
        {
            var sentProducts = new List<SentProductModel>();
            var sentBtbtProducts = await _context.BtbSentProductModel
                .Join(_context.ProductModel,
                btbSentProduct => btbSentProduct.ProductId,
                product => product.Id,
                (btbSentProduct, product) => new
                {
                    ProductId = btbSentProduct.ProductId,
                    ProductCode = product.ProductCode,
                    ProductName = product.Name,
                    SentQuantity = btbSentProduct.SentQuantity,
                    Status = btbSentProduct.Status,
                    BtbId = btbSentProduct.Id
                })
                .Where(obj => obj.Status == "sent")
                .ToListAsync();

            foreach (var product in sentBtbtProducts)
            {
                var inTheList = false;

                foreach (var item in sentProducts)
                {
                    if (product.ProductId == item.ProductId)
                    {
                        inTheList = true;
                        item.SentQuantity += product.SentQuantity;
                        item.BtbIds.Add(product.BtbId);
                        break;
                    }
                }

                if (!inTheList)
                {
                    var newSentProduct = new SentProductModel
                    {
                        ProductId = product.ProductId,
                        SentQuantity = product.SentQuantity,
                        RecievedQuantity = product.SentQuantity,
                        ProductName = product.ProductName,
                        ProductCode = product.ProductCode,
                        BtbIds = new List<int> {product.BtbId}
                    };

                    sentProducts.Add(newSentProduct);
                }
            }

            return sentProducts;
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
