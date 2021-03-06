﻿using System;
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
        private readonly UserManager<IdentityUser> _userManager;

        public AttendanceController(ApplicationContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceModel>>> GetAttendanceModel()
        {
            return await _context.AttendanceModel.ToListAsync();
        }

        [HttpGet("date")]
        public async Task<ActionResult<IEnumerable<AttendanceModel>>> GetAttendanceModelByTime(int from = 19000101, int to = 21001231)
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }

            var fromDate = ConvertIntToDate(from);
            var toDate = ConvertIntToDate(to).AddDays(1);

            var filteredAttendance = _context.AttendanceModel.Where(obj => obj.DateTime >= fromDate && obj.DateTime < toDate);

            return filteredAttendance.ToList();
        }
        
        
        [HttpPost]
        public async Task<IActionResult> PostAttendanceModel(IEnumerable<string> consultants)
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
                    var user = await _userManager.FindByNameAsync(consultant);

                    if (user == null)
                    {
                        return NotFound("User not found!");
                    }
                    else
                    {
                        attendanceModel.User = user;
                        _context.AttendanceModel.Add(attendanceModel);
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok("Done");
            }

            else
                return Ok("Today attendance is already filled");
            
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
