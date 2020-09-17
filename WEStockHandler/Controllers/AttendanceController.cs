using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEStockHandler.Data;
using WEStockHandler.Models;


namespace WEStockHandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllHeaders")]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public AttendanceController(ApplicationContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceModel>>> GetAttendanceModel()
        {
            return await _context.AttendanceModel.ToListAsync();
        }

        [HttpGet("date")]
        public async Task<ActionResult<IEnumerable<AttendanceWithNameModel>>> GetAttendanceModelByTimeWithConsultantName(int from = 19000101, int to = 21001231)
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }

            var fromDate = ConvertIntToDate(from);
            var toDate = ConvertIntToDate(to).AddDays(1);

            var filteredAttendance = _context.AttendanceModel
                .Join(_context.ConsultantModel,
                attendance => attendance.ConsultantId,
                consultant => consultant.ConsultantId,
                (attendance, consultant) => new
                {
                    Id = attendance.Id,
                    ConsultantId = consultant.ConsultantId,
                    ConsultantName = consultant.Name,
                    DateTime = attendance.DateTime
                })
                .Where(obj => obj.DateTime >= fromDate && obj.DateTime < toDate);

            var filteredAttendanceList = new List<AttendanceWithNameModel>();

            foreach (var attendace in filteredAttendance)
            {
                var withName = new AttendanceWithNameModel();
                withName.Id = attendace.Id;
                withName.ConsultantId = attendace.ConsultantId;
                withName.ConsultantName = attendace.ConsultantName;
                withName.DateTime = attendace.DateTime;

                filteredAttendanceList.Add(withName);
            }
                
            return filteredAttendanceList;
        }
        
        
        [HttpPost]
        public async Task<IActionResult> PostAttendanceModel(IEnumerable<ConsultantModel> consultants)
        {
            var lastAttendanceDate = new DateTime();

            try
            {
                lastAttendanceDate = _context.AttendanceModel.Max(e => e.DateTime);
            }
            catch (Exception)
            {
                lastAttendanceDate = DateTime.Now.AddDays(-1000);
            }

            DateTime time = DateTime.Now;

            if (lastAttendanceDate.Date < time.Date)
            {
                foreach (var consultant in consultants)
                {
                    AttendanceModel attendanceModel = new AttendanceModel();
                    attendanceModel.DateTime = time;
                    attendanceModel.ConsultantId = consultant.ConsultantId;
                    _context.AttendanceModel.Add(attendanceModel);
                    await _context.SaveChangesAsync();
                }

                return Ok("Done");
            }

            else
                return BadRequest("Today attendance is already filled");
            
        }

        
        private bool AttendanceModelExists(int id)
        {
            return _context.AttendanceModel.Any(e => e.Id == id);
        }

        private DateTime ConvertIntToDate(int dateNumber)
        {
            var date = new DateTime();

            date = DateTime.ParseExact(dateNumber.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
           
            return date;
        }
    }
}
