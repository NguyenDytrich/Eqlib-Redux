using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EqlibApi.Models;
using EqlibApi.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EqlibApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ApplicationContext context;

        public CheckoutController(ApplicationContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Checkout>>> GetCheckouts()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Checkout>> GetCheckouts(int id)
        {
            throw new NotImplementedException();
        }
    }
}