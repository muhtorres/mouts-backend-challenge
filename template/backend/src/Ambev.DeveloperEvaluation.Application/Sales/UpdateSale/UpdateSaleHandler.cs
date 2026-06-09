using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for handling UpdateSaleCommand requests.
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of <see cref="UpdateSaleHandler"/>.
    /// </summary>
    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request.
    /// </summary>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
        {
            throw new SalesNotFoundException($"Sale with ID {command.Id} not found");
        }

        // Update header details
        sale.SaleDate = command.SaleDate;
        sale.CustomerId = command.CustomerId;
        sale.CustomerName = command.CustomerName;
        sale.BranchId = command.BranchId;
        sale.BranchName = command.BranchName;
        sale.UpdatedAt = DateTime.UtcNow;

        // Sync items:
        // 1. Remove items from the database collection that are not present in the update command.
        var commandItemIds = command.Items
            .Where(i => i.Id.HasValue && i.Id.Value != Guid.Empty)
            .Select(i => i.Id!.Value)
            .ToList();

        sale.Items.RemoveAll(item => !commandItemIds.Contains(item.Id));

        // 2. Add new items or update properties of existing ones.
        foreach (var itemDto in command.Items)
        {
            if (itemDto.Id.HasValue && itemDto.Id.Value != Guid.Empty)
            {
                var existingItem = sale.Items.FirstOrDefault(i => i.Id == itemDto.Id.Value);
                if (existingItem != null)
                {
                    existingItem.ProductId = itemDto.ProductId;
                    existingItem.ProductName = itemDto.ProductName;
                    existingItem.Quantity = itemDto.Quantity; // triggers recalculation
                    existingItem.UnitPrice = itemDto.UnitPrice; // triggers recalculation
                }
            }
            else
            {
                var newItem = new SaleItem
                {
                    SaleId = sale.Id,
                    ProductId = itemDto.ProductId,
                    ProductName = itemDto.ProductName,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice
                };
                sale.Items.Add(newItem);
            }
        }

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        // Publish SaleModifiedEvent
        await _mediator.Publish(new Domain.Events.SaleModifiedEvent(updatedSale), cancellationToken);

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}
