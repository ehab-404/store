using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testRestApi.data;

namespace testRestApi.Controllers  
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {

        private readonly appdbcontext _context;
        public AuditLogsController( appdbcontext context)
        {

            _context=context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            var log= await _context.infrastructureTransactions.OrderByDescending(l => l.Timestamp).ToListAsync();
            if(log != null) 
            { 
                var logs = await _context.infrastructureTransactions.OrderByDescending(l => l.Timestamp).ToListAsync();

                if (logs == null) { return NotFound($"no logs "); }
                else { return Ok(logs); }



            }
            else { return NotFound($"no logs "); }


        }

        [HttpGet("by-table/{tablename}")]
        public async Task<IActionResult> GetLogsByTable(string tablename) {


            var logs = await _context.infrastructureTransactions.Where(l=>l.TableName == tablename).OrderByDescending(l => l.Timestamp).ToListAsync();  


            return Ok(logs);


        }






        [HttpGet("by-user/{username}")]
        public async Task<IActionResult> GetLogsByUser(string userName)
        {


            var logs = await _context.infrastructureTransactions.Where(l => l.UserName == userName).OrderByDescending(l => l.Timestamp).ToListAsync();


            return Ok(logs);


        }

    }


 }

