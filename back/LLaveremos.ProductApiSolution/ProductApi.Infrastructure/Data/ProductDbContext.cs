using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Infrastructure.Data
{
    //Se crea un dbset que sirve para poder crear varias instancias del producto y agregarlo a la base de datos del sql server. Se usa un set get para poder recibir y mandar los datos
    public class ProductDbContext (DbContextOptions<ProductDbContext> options): DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
    }
}
