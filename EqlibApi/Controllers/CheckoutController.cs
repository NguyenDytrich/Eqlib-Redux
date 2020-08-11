using EqlibApi.Models.Db;
using EqlibApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EqlibApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService service;
        public CheckoutController(ICheckoutService service)
        {
            this.service = service;
        }

        [HttpGet]
        /// <summary>
        /// Get all Checkout entries
        /// </summary>
        public async Task<ActionResult<IEnumerable<Checkout>>> GetCheckouts()
        {
            return await service.GetAsync();
        }

        [HttpGet("{id}")]
        /// <summary>
        /// Get a Checkout entry by Id
        /// </summary>
        /// <param name="id">Target Checkout id</param>
        public async Task<ActionResult<Checkout>> GetCheckouts(int id)
        {
            var result = await service.GetAsync(c => c.Id == id);
            if (result.Count == 0)
            {
                return new NotFoundResult();
            }
            else
            {
                return result.FirstOrDefault();
            }
        }
        
        [HttpPost]
        /// <summary>
        /// Creates a new Checkout entry in the database
        /// </summary>
        /// <param name="checkout">A Checkout object</param>
        /// <seealso cref="Checkout"/>
        public async Task<ActionResult<Checkout>> PostCheckout(Checkout checkout)
        {
            foreach(var i in checkout.ItemIds)
            {
                var exists = service.ItemExists(i);
                if (!exists)
                {
                    return new BadRequestObjectResult($"Item not found for id {i}");
                }
            }
            return await service.CreateAsync(checkout);
        }

        [HttpDelete("{id}")]
        /// <summary>
        /// Delete a Checkout entry by Id
        /// </summary>
        /// <returns>404 if no Checkout exists by Id. 200 on success.</returns>
        public async Task<StatusCodeResult> DeleteCheckout(int id)
        {
            // 
            var doesExist = service.CheckoutExists(id);
            if (doesExist)
            {
                await service.DeleteAsync(id);
                return NoContent();
            } else
            {
                return NotFound();
            }
        }
    }
}