using EqlibApi.Models;
using EqlibApi.Models.Db;
using EqlibApi.Models.Enums;
using FluentValidation;
using System;
using System.Linq;

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
                    context.AddFailure($"Item specified by ItemId {id} does not exist.");
                }
                else
                {
                    if (item.Availability != EAvailability.Available)
                    {
                        context.AddFailure("Item specified by ItemId is not Available.");
                    }
                }
            });

            RuleFor(c => c.DueDate).Must((checkout, dueDate) => checkout.CheckoutDate < dueDate)
                .WithMessage("Due date must be after checkout date.")
                .When(c => c.CheckoutDate != null);

            RuleFor(c => c.DueDate).Must(dueDate => DateTime.Now < dueDate)
                .WithMessage("Due date must be after checkout date.")
                .When(c => c.CheckoutDate == null);

            RuleFor(c => c.ReturnDate).Must((checkout, returnDate) => checkout.CheckoutDate < returnDate)
                .WithMessage("Return date must be after checkout date.")
                .When(c => c.ReturnDate != null);
        }
    }
}
