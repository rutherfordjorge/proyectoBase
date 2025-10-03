using System;
using ProyectoBase.Api.Domain.Exceptions;
using ProyectoBase.Api.Domain.ValueObjects;

namespace ProyectoBase.Api.Domain.Entities
{
    /// <summary>
    /// Representa un producto disponible para la venta.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Product"/>.
        /// </summary>
        /// <param name="id">El identificador único del producto.</param>
        /// <param name="name">El nombre del producto.</param>
        /// <param name="price">El precio unitario del producto.</param>
        /// <param name="stock">La cantidad disponible en inventario.</param>
        /// <param name="description">La descripción opcional del producto.</param>
        public Product(Guid id, string name, decimal price, int stock, string? description = null)
            : this(id, ProductName.Create(name), Money.From(price), ProductStock.Create(stock), ProductDescription.Create(description))
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Product"/> utilizando objetos de valor.
        /// </summary>
        /// <param name="id">El identificador único del producto.</param>
        /// <param name="name">El nombre del producto.</param>
        /// <param name="price">El precio unitario del producto.</param>
        /// <param name="stock">La cantidad disponible en inventario.</param>
        /// <param name="description">La descripción opcional del producto.</param>
        public Product(Guid id, ProductName name, Money price, ProductStock stock, ProductDescription? description = null)
        {
            if (id == Guid.Empty)
            {
                throw new ValidationException("Se debe proporcionar el identificador del producto.");
            }

            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(price);
            ArgumentNullException.ThrowIfNull(stock);

            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
            Description = description;
        }

        private Product()
        {
            Name = null!;
            Price = null!;
            Stock = null!;
        }

        /// <summary>
        /// Obtiene el identificador único del producto.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Obtiene el nombre del producto.
        /// </summary>
        public ProductName Name { get; private set; }

        /// <summary>
        /// Obtiene la descripción opcional del producto.
        /// </summary>
        public ProductDescription? Description { get; private set; }

        /// <summary>
        /// Obtiene el precio vigente del producto.
        /// </summary>
        public Money Price { get; private set; }

        /// <summary>
        /// Obtiene la cantidad disponible en inventario del producto.
        /// </summary>
        public ProductStock Stock { get; private set; }

        /// <summary>
        /// Actualiza el nombre del producto.
        /// </summary>
        /// <param name="name">El nuevo nombre del producto.</param>
        public void UpdateName(string name)
        {
            Name = ProductName.Create(name);
        }

        /// <summary>
        /// Actualiza la descripción del producto.
        /// </summary>
        /// <param name="description">La nueva descripción del producto.</param>
        public void UpdateDescription(string? description)
        {
            Description = ProductDescription.Create(description);
        }

        /// <summary>
        /// Cambia el precio del producto.
        /// </summary>
        /// <param name="price">El nuevo precio a asignar.</param>
        public void ChangePrice(decimal price)
        {
            Price = Money.From(price);
        }

        /// <summary>
        /// Incrementa la cantidad en inventario en la medida indicada.
        /// </summary>
        /// <param name="quantity">La cantidad que se agregará al inventario.</param>
        public void IncreaseStock(int quantity)
        {
            Stock = Stock.Increase(quantity);
        }

        /// <summary>
        /// Disminuye la cantidad en inventario en la medida indicada.
        /// </summary>
        /// <param name="quantity">La cantidad que se retirará del inventario.</param>
        public void DecreaseStock(int quantity)
        {
            Stock = Stock.Decrease(quantity);
        }
    }
}
