using AutoMapper;
using ProyectoBase.Application.DTOs;
using ProyectoBase.Domain.Entities;

namespace ProyectoBase.Application.Mapping;

/// <summary>
/// Defines the mappings between product domain entities and DTOs.
/// </summary>
public class ProductProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductProfile"/> class.
    /// </summary>
    public ProductProfile()
    {
        CreateMap<Product, ProductResponseDto>();
    }
}
