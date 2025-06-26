﻿using System;
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
                                .Include(li => li.Product)
                                .Include(li => li.Request)
                                .ToListAsync();
        }

        // GET: api/LineItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LineItem>> GetLineItem(int id)
        {
            var lineItem = await _context.LineItems
                .Include(li => li.Product)
                .Include(li => li.Request)
                .FirstOrDefaultAsync(li => li.ID == id);

            if (lineItem == null)
            {
                return NotFound();
            }

            return lineItem;
        }

        // PUT: api/LineItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLineItem(int ID, LineItem lineItem)
        {
            if (!LineItemExists(lineItem.ID))
            {
                return NotFound();
            }
          
            _context.Entry(lineItem).State = EntityState.Modified;
            

            try
            {
                await _context.SaveChangesAsync();
                await UpdateTotal(lineItem.RequestID);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LineItemExists(lineItem.ID))
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
            var product = await _context.Products.FindAsync(lineItem.ProductID);
            if (product == null)
            {
                return BadRequest();
            }

            _context.LineItems.Add(lineItem);
            await _context.SaveChangesAsync();

            await UpdateTotal(lineItem.RequestID);

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

            await UpdateTotal(lineItem.RequestID);
            return NoContent();
        }



        [HttpGet("lines-for-req/{reqId}")]
        public async Task<ActionResult<LineItem>> GetLineItemsForRequest(int reqId)
        {
            //var request = await _context.Requests
            //    .Include(r => r.LineItems)
            //    .FirstOrDefaultAsync(r => r.ID == reqId);

            //if (request == null)
            //{
            //    return NotFound();
            //}

            //request.LineItems;
            var lineItems = await _context.LineItems
                .Where(li => li.RequestID == reqId)
                .Include(li => li.Product)
                .ThenInclude(p => p.Vendor)
                .Include(li => li.Request)
                .ThenInclude(r => r.User)
                .ToListAsync();

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
                .Where(li => li.RequestID == reqId && li.ProductID != null) 
                .SumAsync(li => li.Quantity * _context.Products
                                                      .Where(p => p.ID == (int)li.ProductID) 
                                                      .Select(p => p.Price)
                                                      .FirstOrDefault());

                await _context.SaveChangesAsync();
            }
        }
        private bool LineItemExists(int id)
        {
            return _context.LineItems.Any(e => e.ID == id);
        }
    }
}
