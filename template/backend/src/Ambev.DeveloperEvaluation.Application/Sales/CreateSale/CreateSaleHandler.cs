using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for handling CreateSaleCommand requests.
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of <see cref="CreateSaleHandler"/>.
    /// </summary>
    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request.
    /// </summary>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var sale = _mapper.Map<Sale>(command);

        // Generate a human-readable sale number
        sale.SaleNumber = $"SL-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        // Triggers automatic calculations for discounts and totals inside entities
        foreach (var item in sale.Items)
        {
            item.Quantity = item.Quantity; // forces CalculateDiscountsAndTotal execution
        }

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        // Publish SaleCreatedEvent domain event
        await _mediator.Publish(new Domain.Events.SaleCreatedEvent(createdSale), cancellationToken);

        return _mapper.Map<CreateSaleResult>(createdSale);
    }
}
