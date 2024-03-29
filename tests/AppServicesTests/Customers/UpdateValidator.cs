﻿using FluentValidation.TestHelper;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Customers.Validators;
using MyApp.TestData.Constants;

namespace AppServicesTests.Customers;

public class UpdateValidator
{
    private static CustomerUpdateDto DefaultCustomerUpdate => new();

    [Test]
    public async Task ValidDto_ReturnsAsValid()
    {
        var model = DefaultCustomerUpdate with { Name = TextData.ValidName };
        var validator = new CustomerUpdateValidator();

        var result = await validator.TestValidateAsync(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task NameTooShort_ReturnsAsInvalid()
    {
        var model = DefaultCustomerUpdate with { Name = TextData.ShortName };
        var validator = new CustomerUpdateValidator();

        var result = await validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(e => e.Name);
    }
}
