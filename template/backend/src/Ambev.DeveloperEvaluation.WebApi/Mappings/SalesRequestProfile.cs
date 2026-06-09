using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings;

/// <summary>
/// AutoMapper profile for WebApi request and response mapping of Sales.
/// </summary>
public class SalesRequestProfile : Profile
{
    public SalesRequestProfile()
    {
        // CreateSale mappings
        CreateMap<CreateSaleRequest, CreateSaleCommand>();
        CreateMap<CreateSaleItemRequest, CreateSaleItemDto>();
        CreateMap<CreateSaleResult, CreateSaleResponse>();
        CreateMap<Application.Sales.CreateSale.SaleItemResultDto, WebApi.Features.Sales.CreateSale.SaleItemResponseDto>();

        // GetSale mappings
        CreateMap<GetSaleResult, GetSaleResponse>();
        CreateMap<Application.Sales.GetSale.SaleItemResultDto, WebApi.Features.Sales.GetSale.SaleItemResponseDto>();

        // UpdateSale mappings
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
        CreateMap<UpdateSaleItemRequest, UpdateSaleItemDto>();
        CreateMap<UpdateSaleResult, UpdateSaleResponse>();
        CreateMap<Application.Sales.UpdateSale.SaleItemResultDto, WebApi.Features.Sales.UpdateSale.SaleItemResponseDto>();

        // ListSales mappings
        CreateMap<Application.Sales.ListSales.SaleItemDto, WebApi.Features.Sales.ListSales.SaleItemResponseDto>();
    }
}
