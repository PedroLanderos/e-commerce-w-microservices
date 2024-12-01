using Llaveremos.SharedLibrary.Logs;
using Llaveremos.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Infrastructure.Repositories
{
    //Se implementan las interfaces y sus debidas acciones de crud utilizando el context definido para la base de datos de los productos
    //Se intentan realizar todas las operaciones para el CRUD y en caso de no ser positivas se manda una excepcion que comunica mediante un log lo que salio mal
    internal class ProductRepository(ProductDbContext context) : IProduct
    {
        //Crear un producto en la base de datos
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                var getProdcut = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if (getProdcut is not null && !string.IsNullOrEmpty(getProdcut.Name))
                    return new Response(false, $"{entity.Name} already added");

                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();

                if (currentEntity is not null && currentEntity.Id > 0)
                    return new Response(true, "Product added to the database");
                else
                    return new Response(false, "Error adding a new Product");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error adding a new product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, "Error product not found");
                
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return new Response(true, "Product deleted"); 
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error while deleting the product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error, the product was not found");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error getting the products");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error, the product was not found");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, "the product was not found");
                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, "the product was updated");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error, the product was could not be updated.");
            }
        }
    }
}
