using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProyectoBase.Api.Domain.Entities;

namespace ProyectoBase.Api.Application.Abstractions
{
    /// <summary>
    /// Proporciona operaciones de acceso a datos para entidades <see cref="Product"/>.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Recupera un producto por su identificador único.
        /// </summary>
        /// <param name="id">El identificador del producto que se desea obtener.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns>La instancia de <see cref="Product"/> correspondiente, si existe.</returns>
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera todos los productos disponibles en el almacén de datos.
        /// </summary>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns>Una colección de solo lectura con los productos disponibles.</returns>
        Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Persiste un nuevo producto en el almacén de datos.
        /// </summary>
        /// <param name="product">El producto que se desea guardar.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task AddAsync(Product product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza un producto existente en el almacén de datos.
        /// </summary>
        /// <param name="product">La instancia del producto que contiene los cambios.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task UpdateAsync(Product product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina un producto del almacén de datos.
        /// </summary>
        /// <param name="id">El identificador del producto que se desea eliminar.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
