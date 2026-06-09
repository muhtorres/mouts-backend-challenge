using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Sales.EventHandlers;

/// <summary>
/// Handles domain events raised by the Sales aggregate and logs them to the application log.
/// </summary>
public class SaleEventHandlers : 
    INotificationHandler<SaleCreatedEvent>,
    INotificationHandler<SaleModifiedEvent>,
    INotificationHandler<SaleCancelledEvent>,
    INotificationHandler<SaleItemCancelledEvent>
{
    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("Domain Event: [SaleCreatedEvent] published for Sale Number: {SaleNumber}, ID: {SaleId}, Total Amount: {TotalAmount}", 
            notification.Sale.SaleNumber, notification.Sale.Id, notification.Sale.TotalAmount);
        return Task.CompletedTask;
    }

    public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("Domain Event: [SaleModifiedEvent] published for Sale Number: {SaleNumber}, ID: {SaleId}, Total Amount: {TotalAmount}", 
            notification.Sale.SaleNumber, notification.Sale.Id, notification.Sale.TotalAmount);
        return Task.CompletedTask;
    }

    public Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("Domain Event: [SaleCancelledEvent] published for Sale Number: {SaleNumber}, ID: {SaleId}", 
            notification.Sale.SaleNumber, notification.Sale.Id);
        return Task.CompletedTask;
    }

    public Task Handle(SaleItemCancelledEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("Domain Event: [SaleItemCancelledEvent] published for Sale Number: {SaleNumber}, ID: {SaleId}, Item ID: {ItemId}", 
            notification.Sale.SaleNumber, notification.Sale.Id, notification.SaleItemId);
        return Task.CompletedTask;
    }
}
