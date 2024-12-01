using Microsoft.EntityFrameworkCore.Infrastructure; 
using ProductApi.Domain.Entities; //se importa el espacio de donde se define la entidad producto
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Application.DTOs.Conversions
{
    public static class ProductConversion
    {
        public static Product ToEntity(ProductDTO product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Quantity = product.Quantity,
            Price = product.Price,
        };

        public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(Product product, IEnumerable<Product>? products)
        {
            //si es solo un producto
            if (product is not null || product is null)
            {
                var singleProduct = new ProductDTO(
                    product!.Id, product.Name!, product.Quantity, product.Price
                    );
                return (singleProduct, null);
            }

            //si es una lista de productos 
            if(products is not null || products is null)
            {
                var _products = products!.Select(p =>
                new ProductDTO(p.Id, p.Name!, p.Quantity, p.Price)).ToList();

                return (null, _products);
            }

            //si ambos valores son null
            return (null, null);
        }

    }
}
