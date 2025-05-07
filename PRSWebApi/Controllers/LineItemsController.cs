using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRSWebApi.Models;

namespace PRSWebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LineItemsController : ControllerBase
    {
        private readonly PrsDbContext _context;

        public LineItemsController(PrsDbContext context)
        {
            _context = context;
        }

        // GET: api/LineItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LineItem>>> GetLineItems()
        {
            return await _context.LineItems
                                //.Include(li => li.Product)
                                //.Include(li => li.Request)
                                 .ToListAsync();
        }

        // GET: api/LineItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LineItem>> GetLineItem(int id)
        {
            var lineItem = await _context.LineItems
                //.Include(li => li.Product)
                //.Include(li => li.Request)
                .FirstOrDefaultAsync(li => li.Id == id);

            if (lineItem == null)
            {
                return NotFound();
            }

            return lineItem;
        }

        // PUT: api/LineItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutLineItem(LineItem lineItem)
        {
            if (!LineItemExists(lineItem.Id))
            {
                return NotFound();
            }
          
            _context.Entry(lineItem).State = EntityState.Modified;
            

            try
            {
                await _context.SaveChangesAsync();
                await UpdateTotal(lineItem.RequestId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LineItemExists(lineItem.Id))
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

        // POST: api/LineItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LineItem>> PostLineItem(LineItem lineItem)
        {
            var product = await _context.Products.FindAsync(lineItem.ProductId);
            if (product == null)
            {
                return BadRequest();
            }

            _context.LineItems.Add(lineItem);
            await _context.SaveChangesAsync();

            await UpdateTotal(lineItem.RequestId);

            return Ok(lineItem);
        }

        // DELETE: api/LineItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLineItem(int id)
        {
            var lineItem = await _context.LineItems.FindAsync(id);
            if (lineItem == null)
            {
                return NotFound();
            }

            _context.LineItems.Remove(lineItem);
            await _context.SaveChangesAsync();

            // Recalc total for request
            await UpdateTotal(lineItem.RequestId);
            return NoContent();
        }



        [HttpGet("lines-for-req/{reqId}")]
        public async Task<IActionResult> GetLineItemsForRequest(int reqId)
        {
            var request = await _context.Requests
                .Include(r => r.LineItems)
                .FirstOrDefaultAsync(r => r.RequestId == reqId);

            if (request == null)
            {
                return NotFound();
            }

            var lineItems = request.LineItems;

            if (lineItems == null || lineItems.Count == 0)
            {
                return NotFound();
            }

            return Ok(lineItems);
        }

        private async Task UpdateTotal(int reqId)
        {
            var request = await _context.Requests.FindAsync(reqId);
            if (request != null)
            {
                request.Total = await _context.LineItems
                .Where(li => li.RequestId == reqId && li.ProductId != null) 
                .SumAsync(li => li.Quantity * _context.Products
                                                      .Where(p => p.ProductId == (int)li.ProductId) 
                                                      .Select(p => p.Price)
                                                      .FirstOrDefault());

                await _context.SaveChangesAsync();
            }
        }
        private bool LineItemExists(int id)
        {
            return _context.LineItems.Any(e => e.Id == id);
        }
    }
}
