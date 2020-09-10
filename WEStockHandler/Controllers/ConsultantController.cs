using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEStockHandler.Data;
using WEStockHandler.Models;

namespace WEStockHandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultantController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ConsultantController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsultantModel>>> GetConsultantModel()
        {
            return await _context.ConsultantModel.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConsultantModel>> GetConsultantModel(int id)
        {
            var consultantModel = await _context.ConsultantModel.FindAsync(id);

            if (consultantModel == null)
            {
                return NotFound();
            }

            return consultantModel;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutConsultantModel(int id, ConsultantModel consultantModel)
        {
            if (id != consultantModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(consultantModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConsultantModelExists(id))
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
        public async Task<ActionResult<ConsultantModel>> PostConsultantModel(ConsultantModel consultantModel)
        {
            _context.ConsultantModel.Add(consultantModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConsultantModel", new { id = consultantModel.Id }, consultantModel);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ConsultantModel>> DeleteConsultantModel(int id)
        {
            var consultantModel = await _context.ConsultantModel.FindAsync(id);
            if (consultantModel == null)
            {
                return NotFound();
            }

            _context.ConsultantModel.Remove(consultantModel);
            await _context.SaveChangesAsync();

            return consultantModel;
        }
        private bool ConsultantModelExists(int id)
        {
            return _context.ConsultantModel.Any(e => e.Id == id);
        }
    }
}
