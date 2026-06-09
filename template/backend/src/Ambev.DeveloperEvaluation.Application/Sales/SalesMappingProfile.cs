using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;

namespace Ambev.DeveloperEvaluation.Application.Sales;

/// <summary>
/// AutoMapper profile for Sales-related mappings.
/// </summary>
public class SalesMappingProfile : Profile
{
    public SalesMappingProfile()
    {
        // CreateSale mappings
        CreateMap<CreateSaleCommand, Sale>();
        CreateMap<CreateSaleItemDto, SaleItem>();
        CreateMap<Sale, CreateSaleResult>();
        CreateMap<SaleItem, CreateSale.SaleItemResultDto>();

        // GetSale mappings
        CreateMap<Sale, GetSaleResult>();
        CreateMap<SaleItem, GetSale.SaleItemResultDto>();

        // UpdateSale mappings
        CreateMap<UpdateSaleCommand, Sale>();
        CreateMap<UpdateSaleItemDto, SaleItem>();
        CreateMap<Sale, UpdateSaleResult>();
        CreateMap<SaleItem, UpdateSale.SaleItemResultDto>();

        // ListSales mappings
        CreateMap<Sale, ListSales.SaleItemDto>();
    }
}
