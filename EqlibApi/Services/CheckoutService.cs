using EqlibApi.Models;
using EqlibApi.Models.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EqlibApi.Services
{
    public interface ICheckoutService
    {
        /// <summary>
        /// Asynchronously gets all Checkout entries.
        /// <seealso cref="Checkout"/>
        /// </summary>
        /// <returns>A list of Checkout objects</returns>
        Task<List<Checkout>> GetAsync();

        /// <summary>
        /// Asynchronously gets all Checkout entries matching an expression.
        /// <seealso cref="Checkout"/>
        /// </summary>
        /// <param name="filterby">An expression that returns a boolean.</param>
        /// <returns>A list of Checkout objects</returns>
        Task<List<Checkout>> GetAsync(Expression<Func<Checkout, bool>> filter);

        Task DeleteAsync(int id);

        /// <summary>
        /// Asynchronously creates a Checkout entry in the database from a Checkout object.
        /// </summary>
        /// <param name="checkout"></param>
        /// <returns>The created checkout object</returns>
        /// <exception cref="ArgumentException">If ItemIds are invalid.</exception>
        Task<Checkout> CreateAsync(Checkout checkout);

    }
    public class CheckoutService : ICheckoutService
    {
        private readonly IApplicationContext context;

        public CheckoutService(IApplicationContext context)
        {
            this.context = context;
        }

        public Task<List<Checkout>> GetAsync()
        {
            var checkouts = context.Checkouts;
            return checkouts.ToListAsync();
        }

        public Task<List<Checkout>> GetAsync(Expression<Func<Checkout, bool>> filter)
        {
            return context.Checkouts.Where(filter).Select(c => c).ToListAsync();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Checkout> CreateAsync(Checkout checkout)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether or not a Checkout exists.
        /// <param name="id">An Id integer to search for.</param>
        /// <returns>Boolean result specifying whether or not a Checkout by Id exists.</returns>
        /// </summary>
        private bool CheckoutExists(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether or not a Checkout exists.
        /// <param name="id">An Id integer to search for.</param>
        /// <returns>Boolean result specifying whether or not a Checkout by Id exists.</returns>
        /// </summary>
        private bool ItemExists(int id)
        {
            throw new NotImplementedException();
        }
    }
}
