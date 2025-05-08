using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRSWebApi.DTOs;
using PRSWebApi.Models;

namespace PRSWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly PrsDbContext _context;

        public RequestsController(PrsDbContext context)
        {
            _context = context;
        }
        
        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            return await _context.Requests.ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            return request;
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request)
        {
            if (id != request.ID)
            {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
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

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPost]
        public async Task<ActionResult<RequestDTO>> PostRequest(RequestDTO requestDTO)
        {
            var request = new Models.Request
            {
                UserID = (int)requestDTO.UserID,
                RequestNumber = GetNextRequestNumber(),
                Description = requestDTO.Description,
                Justification = requestDTO.Justification,
                DateNeeded = requestDTO.DateNeeded,
                DeliveryMode = requestDTO.DeliveryMode,
                Status = "NEW",
                Total = 0,
                SubmittedDate = DateTime.Now,
                ReasonForRejection = null
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.ID }, request);

        }
        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Generate new request number
        private string GetNextRequestNumber()
        {
            // requestNumber format: R2409230011
            // 11 chars, 'R' + YYMMDD + 4 digit # w/ leading zeros
            string requestNbr = "R";
            // add YYMMDD string
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            string datePart = today.ToString("yyMMdd");
            requestNbr += datePart;
            // get maximum request number from db **for today**
            string maxReqNbr = _context.Requests
                                       .Where(r => r.RequestNumber.StartsWith("R" + datePart))
                                       .Max(r => r.RequestNumber);
            
            String reqNbr = "";
            if (maxReqNbr != null)
            {
                // get last 4 characters, convert to number
                String tempNbr = maxReqNbr.Substring(7);
                int nbr = Int32.Parse(tempNbr);
                nbr++;
                // pad w/ leading zeros
                reqNbr += nbr;
                reqNbr = reqNbr.PadLeft(4, '0');
            }
            else
            {
                reqNbr = "0001";
            }
            requestNbr += reqNbr;
            return requestNbr;
        }

        // PUT: api/Requests/submit-review
        [HttpPut("submit-review/{requestid}")]
        public async Task<IActionResult> SubmitReview(int requestid)
        {
            var request = await _context.Requests.FindAsync(requestid);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = request.Total <= 50 ? "APPROVED" : "REVIEW";
            request.SubmittedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(request);
        }

        // GET: api/Requests/list-review/#
        [HttpGet("list-review/{userId}")]
        public async Task<ActionResult<IEnumerable<RequestDTO>>> GetRequestsForReview(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
  
            if (user == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                                        .Where(r => r.Status == "REVIEW" && r.UserID != userId)
                                        .ToListAsync();
            return Ok(request);
        }

        // PUT: api/Requests/approve/{id}
        [HttpPut("approve/{requestid}")]
        public async Task<ActionResult> RequestApproval(int requestid)
        {
            var request = await _context.Requests.FindAsync(requestid);
            if (Request == null)
            {
                return NotFound();
            }
            request.Status = "APPROVED";
            await _context.SaveChangesAsync();
            return Ok(request);
        }

        // PUT: api/Requests/reject/{id}
        // To protect from over
        // ing attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectRequest(int id, [FromBody] string reason)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            request.Status = "REJECTED";
            request.ReasonForRejection = reason;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.ID == id);
        }

    }
}
