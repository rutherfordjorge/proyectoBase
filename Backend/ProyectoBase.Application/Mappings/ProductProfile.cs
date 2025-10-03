using System;
using AutoMapper;
using ProyectoBase.Api.Application.DTOs;
using ProyectoBase.Api.Domain.Entities;

namespace ProyectoBase.Api.Application.Mappings;

/// <summary>
/// Define los mapeos entre las entidades de dominio de productos y sus DTO.
/// </summary>
public class ProductProfile : Profile
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ProductProfile"/>.
    /// </summary>
    public ProductProfile()
    {
        CreateMap<ProductCreateDto, Product>()
            .ConstructUsing(dto => new Product(Guid.NewGuid(), dto.Name, dto.Price, dto.Stock, dto.Description));

        CreateMap<ProductUpdateDto, Product>()
            .AfterMap((dto, product) =>
            {
                product.UpdateName(dto.Name);
                product.UpdateDescription(dto.Description);
                product.ChangePrice(dto.Price);

                if (dto.Stock != product.Stock.Value)
                {
                    var difference = dto.Stock - product.Stock.Value;

                    if (difference > 0)
                    {
                        product.IncreaseStock(difference);
                    }
                    else
                    {
                        product.DecreaseStock(Math.Abs(difference));
                    }
                }
            });

        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description != null ? src.Description.Value : null))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock.Value));
    }
}
