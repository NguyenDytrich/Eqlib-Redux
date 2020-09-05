using EqlibApi.Models.Db;
using EqlibApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqlibApi.Controllers
{
    [Route("api/v1/checkouts")]
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
        public async Task<ActionResult> GetCheckouts()
        {
            var checkouts = await service.GetAsync();
            return Ok(new Dictionary<string, List<Checkout>>()
            {
                {"checkouts", checkouts }
            });
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
        /// Creates a checkout entity in the ApplicationContext. Unless specified,
        /// the following fields will have the default values:
        /// <code>
        ///     CheckoutDate = DateTime.Now
        ///     ApprovalStatus = EApprovalStatus.Pending
        /// </code>
        /// In order for checkouts to be valid, DueDate and ReturnDates must be
        /// after the CheckoutDate. All Items referenced by the request must exist
        /// in the database and be marked as available for checkout.
        ///
        /// After the checkout request is validated, it is inserted into the database,
        /// and all items referenced have their availability changed to Hold by
        /// default.
        /// </summary>
        /// <param name="checkout"></param>
        /// <returns>500 on validation error or 200 with JSON serialized Checkout object instance</returns>
        /// <seealso cref="Checkout"/>
        /// <seealso cref="CheckoutService.CreateAsync(Checkout)"/>
        /// <seealso cref="CheckoutValidators"/>
        public async Task<ActionResult<Checkout>> PostCheckout([FromBody] Checkout checkout)
        {
            try
            {
                return await service.CreateAsync(checkout);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        /// <summary>
        /// Delete a Checkout entry by Id
        /// </summary>
        /// <returns>404 if no Checkout exists by Id. 200 on success.</returns>
        public async Task<ActionResult> DeleteCheckout(int id)
        {
            try
            {
                await service.DeleteAsync(id);
                return NoContent();
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}