using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handler for handling CancelSaleCommand requests.
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of <see cref="CancelSaleHandler"/>.
    /// </summary>
    public CancelSaleHandler(ISaleRepository saleRepository, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the CancelSaleCommand request.
    /// </summary>
    public async Task<CancelSaleResult> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
        {
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");
        }

        sale.Cancel();

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        // Publish SaleCancelledEvent domain event
        await _mediator.Publish(new Domain.Events.SaleCancelledEvent(sale), cancellationToken);

        return new CancelSaleResult
        {
            Success = true,
            SaleNumber = sale.SaleNumber,
            IsCancelled = sale.IsCancelled
        };
    }
}
