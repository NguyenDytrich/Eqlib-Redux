using EqlibApi.Models;
using EqlibApi.Models.Db;
using FluentValidation;
using FluentValidation.Results;
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
        private readonly CheckoutValidators validator;

        public CheckoutService(IApplicationContext context, CheckoutValidators validator)
        {
            this.context = context;
            this.validator = validator;
        }

        public async Task<List<Checkout>> GetAsync()
        {
            var checkouts = context.Checkouts;
            return await checkouts.ToListAsync();
        }

        public async Task<List<Checkout>> GetAsync(Expression<Func<Checkout, bool>> filter)
        {
            return await context.Checkouts.Where(filter).Select(c => c).ToListAsync();
        }

        public async Task<Checkout> CreateAsync(Checkout checkout)
        {
            ValidationResult result = validator.Validate(checkout);
            if (result.IsValid)
            {
                foreach (int i in checkout.ItemIds) {
                    var item = await context.Items.FindAsync(i);
                    item.Availability = Models.Enums.EAvailability.Hold;
                    context.SaveChanges();
                 }
                await context.Checkouts.AddAsync(checkout);
                context.SaveChanges();
                return await context.Checkouts.FindAsync(checkout.Id);
            }
            else
            {
                throw new ArgumentException(result.ToString(";"));
            }
        }

        public async Task DeleteAsync(int id)
        {
            var checkout = await context.Checkouts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (checkout != null)
            {
                context.Checkouts.Remove(checkout);
                foreach(Item i in checkout.Items)
                {
                    i.Availability = Models.Enums.EAvailability.Available;
                }
                context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Checkout not found.");
            }
        }
    }
}
