using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Handler for processing GetSaleQuery requests.
/// </summary>
public class GetSaleHandler : IRequestHandler<GetSaleQuery, GetSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of <see cref="GetSaleHandler"/>.
    /// </summary>
    public GetSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetSaleQuery request.
    /// </summary>
    public async Task<GetSaleResult> Handle(GetSaleQuery query, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(query.Id, cancellationToken);
        if (sale == null)
        {
            throw new SalesNotFoundException($"Sale with ID {query.Id} not found");
        }

        return _mapper.Map<GetSaleResult>(sale);
    }
}
