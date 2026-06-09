using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Handler for handling CancelSaleItemCommand requests.
/// </summary>
public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of <see cref="CancelSaleItemHandler"/>.
    /// </summary>
    public CancelSaleItemHandler(ISaleRepository saleRepository, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the CancelSaleItemCommand request.
    /// </summary>
    public async Task<CancelSaleItemResult> Handle(CancelSaleItemCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken);
        if (sale == null)
        {
            throw new SalesNotFoundException($"Sale with ID {command.SaleId} not found");
        }

        var cancelled = sale.CancelItem(command.ItemId);
        if (!cancelled)
        {
            throw new SalesNotFoundException($"Item with ID {command.ItemId} not found or already cancelled in sale {command.SaleId}");
        }

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        // Publish SaleItemCancelledEvent domain event
        await _mediator.Publish(new Domain.Events.SaleItemCancelledEvent(sale, command.ItemId), cancellationToken);

        return new CancelSaleItemResult
        {
            Success = true,
            ItemId = command.ItemId,
            IsCancelled = true
        };
    }
}
