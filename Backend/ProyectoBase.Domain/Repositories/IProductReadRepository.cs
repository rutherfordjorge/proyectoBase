using System;
using System.Threading;
using System.Threading.Tasks;
using ProyectoBase.Domain.Entities;

namespace ProyectoBase.Domain.Repositories
{
    /// <summary>
    /// Proporciona acceso de solo lectura a entidades <see cref="Product"/> desde un almacén de persistencia.
    /// </summary>
    public interface IProductReadRepository
    {
        /// <summary>
        /// Recupera un producto por su identificador único.
        /// </summary>
        /// <param name="id">El identificador del producto que se desea obtener.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns>El producto que coincide con el identificador proporcionado o <c>null</c> si no existe.</returns>
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
