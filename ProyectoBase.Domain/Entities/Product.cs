using System;
using ProyectoBase.Domain.Exceptions;

namespace ProyectoBase.Domain.Entities
{
    /// <summary>
    /// Represents a product available for sale.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <param name="name">The name of the product.</param>
        /// <param name="price">The unit price of the product.</param>
        /// <param name="stock">The quantity available in stock.</param>
        /// <param name="description">The optional description of the product.</param>
        public Product(Guid id, string name, decimal price, int stock, string? description = null)
        {
            if (id == Guid.Empty)
            {
                throw new ValidationException("The product identifier must be provided.");
            }

            Id = id;
            UpdateName(name);
            UpdateDescription(description);
            ChangePrice(price);
            SetStock(stock);
        }

        /// <summary>
        /// Gets the unique identifier of the product.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the optional description of the product.
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Gets the current price of the product.
        /// </summary>
        public decimal Price { get; private set; }

        /// <summary>
        /// Gets the available stock quantity of the product.
        /// </summary>
        public int Stock { get; private set; }

        /// <summary>
        /// Updates the name of the product.
        /// </summary>
        /// <param name="name">The new name of the product.</param>
        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException("The product name cannot be empty.");
            }

            Name = name.Trim();
        }

        /// <summary>
        /// Updates the description of the product.
        /// </summary>
        /// <param name="description">The new description of the product.</param>
        public void UpdateDescription(string? description)
        {
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        /// <summary>
        /// Changes the price of the product.
        /// </summary>
        /// <param name="price">The new price to assign.</param>
        public void ChangePrice(decimal price)
        {
            if (price < 0)
            {
                throw new ValidationException("The product price cannot be negative.");
            }

            Price = decimal.Round(price, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Increases the stock quantity by the specified amount.
        /// </summary>
        /// <param name="quantity">The quantity to add to the stock.</param>
        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ValidationException("The quantity to increase must be greater than zero.");
            }

            Stock += quantity;
        }

        /// <summary>
        /// Decreases the stock quantity by the specified amount.
        /// </summary>
        /// <param name="quantity">The quantity to remove from the stock.</param>
        public void DecreaseStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ValidationException("The quantity to decrease must be greater than zero.");
            }

            if (quantity > Stock)
            {
                throw new ValidationException("The quantity to decrease exceeds the available stock.");
            }

            Stock -= quantity;
        }

        private void SetStock(int stock)
        {
            if (stock < 0)
            {
                throw new ValidationException("The product stock cannot be negative.");
            }

            Stock = stock;
        }
    }
}
