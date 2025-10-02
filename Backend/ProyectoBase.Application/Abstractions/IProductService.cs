using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProyectoBase.Api.Application.DTOs;

namespace ProyectoBase.Api.Application.Abstractions
{
    /// <summary>
    /// Define las operaciones disponibles para administrar productos dentro de la capa de aplicación.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Crea un nuevo producto a partir de la información proporcionada.
        /// </summary>
        /// <param name="product">Los datos que describen el producto a crear.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns>La representación del producto creado.</returns>
        Task<ProductResponseDto> CreateAsync(ProductCreateDto product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza un producto existente utilizando la información suministrada.
        /// </summary>
        /// <param name="product">Los datos que describen el producto a actualizar.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns>La representación del producto actualizado.</returns>
        Task<ProductResponseDto> UpdateAsync(ProductUpdateDto product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera un producto por su identificador único.
        /// </summary>
        /// <param name="id">El identificador del producto que se desea obtener.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns>La representación del producto solicitado, si existe.</returns>
        Task<ProductResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera todos los productos disponibles en el contexto de la aplicación.
        /// </summary>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns>Una colección que contiene las representaciones de los productos disponibles.</returns>
        Task<IReadOnlyCollection<ProductResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina el producto que coincide con el identificador proporcionado.
        /// </summary>
        /// <param name="id">El identificador del producto que se eliminará.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
