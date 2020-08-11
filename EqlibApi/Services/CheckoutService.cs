using EqlibApi.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EqlibApi.Services
{
    public interface ICheckoutService
    {
        Task<List<Checkout>> GetAsync();
        Task<List<Checkout>> GetAsync(Expression<Func<Checkout, bool>> filterby);

        Task DeleteAsync(int id);
        bool IdExists(int id);
    }
    // public class CheckoutService : ICheckoutService
    // {
    // }
}
