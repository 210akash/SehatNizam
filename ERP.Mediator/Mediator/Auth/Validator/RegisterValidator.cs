using FluentValidation;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Auth.Command;
using System;
using System.Linq;

namespace ERP.Mediator.Mediator.Auth.Validator
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator(ERPDbContext context)
        {

            RuleForEach(x => x.RoleId)
                .Must(x => context.AspNetRoles.Any(o => o.Id == new Guid(x))).When(r => r.RoleId != null).WithMessage("Invalid user role");

            RuleFor(x => x.PhoneNumber)
                .Must(p => !context.AspNetUsers.Any(n => n.PhoneNumber == p))
                .When(p => p.PhoneNumber != null && p.PhoneNumber != "")
                .WithMessage(x => $"Phone Number {x.PhoneNumber} has already associated with another account.");
        }
    }
}
