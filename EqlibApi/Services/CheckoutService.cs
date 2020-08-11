using EqlibApi.Models.Db;
using System;
using System.Collections.Generic;
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
        Task<List<Checkout>> GetAsync(Expression<Func<Checkout, bool>> filterby);

        Task DeleteAsync(int id);

        /// <summary>
        /// Asynchronously creates a Checkout entry in the database from a Checkout object.
        /// </summary>
        /// <param name="checkout"></param>
        /// <returns>The created checkout object</returns>
        /// <exception cref="ArgumentException">If ItemIds are invalid.</exception>
        Task<Checkout> CreateAsync(Checkout checkout);

        /// <summary>
        /// Attempts to find an Item entry by the Id specified.
        /// </summary>
        /// <param name="id">An Id to search for.</param>
        /// <returns>Boolean result specifying whether or not an Item by Id exists.</returns>
        // bool ItemExists(int id);

        /// <summary>
        /// Checks whether or not a Checkout exists.
        /// <param name="id">An Id integer to search for.</param>
        /// <returns>Boolean result specifying whether or not a Checkout by Id exists.</returns>
        /// </summary>
        bool CheckoutExists(int id);
    }
    // public class CheckoutService : ICheckoutService
    // {
    // }
}
