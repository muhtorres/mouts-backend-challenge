using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handler for handling ListSalesQuery requests.
/// </summary>
public class ListSalesHandler : IRequestHandler<ListSalesQuery, ListSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of <see cref="ListSalesHandler"/>.
    /// </summary>
    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the ListSalesQuery request.
    /// </summary>
    public async Task<ListSalesResult> Handle(ListSalesQuery query, CancellationToken cancellationToken)
    {
        var (data, totalCount) = await _saleRepository.ListAsync(
            query.Page,
            query.Size,
            query.Order,
            query.Filters,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / query.Size);

        return new ListSalesResult
        {
            Data = _mapper.Map<List<SaleItemDto>>(data),
            TotalItems = totalCount,
            CurrentPage = query.Page,
            TotalPages = totalPages
        };
    }
}
