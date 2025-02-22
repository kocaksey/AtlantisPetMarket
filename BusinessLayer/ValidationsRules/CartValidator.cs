﻿using BusinessLayer.Models.CartViewModel;
using FluentValidation;
public class CartValidator : AbstractValidator<CartVM>
{
    public CartValidator()
    {
        RuleFor(cart => cart.UserId)
            .GreaterThan(0)
            .WithMessage("UserId must be greater than 0");

        //RuleFor(cart => cart.CreateDateTime)
        //    .LessThanOrEqualTo(DateTime.Now)
        //    .WithMessage("CreateDateTime cannot be in the future");
    }
}
