using EqlibApi.Models;
using EqlibApi.Models.Db;
using EqlibApi.Models.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqlibApi.Services
{
    public class CheckoutValidators : AbstractValidator<Checkout>
    {
        private readonly IApplicationContext appContext;
        public CheckoutValidators(IApplicationContext appContext)
        {
            this.appContext = appContext;

            RuleFor(c => c.ItemIds).Must(i => i.Count() > 0)
                .WithMessage("Need at least 1 ItemId.");

            RuleForEach(c => c.ItemIds).Custom((id, context) =>
            {
                var item = appContext.Items.Find(id);
                if (item == null)
                {
                    context.AddFailure("Item specified by ItemId does not exist.");
                }
                else
                {
                    if (item.Availability != EAvailability.Available)
                    {
                        context.AddFailure("Item specified by ItemId is not Available.");
                    }
                }
            });
        }
    }
}
