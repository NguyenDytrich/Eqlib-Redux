using EqlibApi.Models.Db;
using EqlibApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        public async Task<ActionResult<Checkout>> GetCheckouts(int id)
        {
            var result = await service.GetAsync(c => c.Id == id);
            if (result.Count == 0)
            {
                return new NotFoundResult();
            }
            else
            {
                return result[0];
            }
        }

        [HttpDelete("{id}")]
        /// <summary>
        /// Delete a Checkout entry by Id
        /// </summary>
        public async Task<StatusCodeResult> DeleteCheckout(int id)
        {
            var doesExist = service.IdExists(id);
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