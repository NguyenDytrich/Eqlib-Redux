using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EqlibApi.Models;
using EqlibApi.Models.Db;
using EqlibApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<IEnumerable<Checkout>>> GetCheckouts()
        {
            return await service.GetAsync();
        }

        [HttpGet("{id}")]
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
    }
}