using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Los DTOs se usan para transferir datos entre capas y solo es un tipo de interfaz que contiene los datos necesarios sin la logica que se requiere
namespace ProductApi.Application.DTOs
{
    public record ProductDTO(int Id,
        [Required] string Name, [Required, Range(0, int.MaxValue)] int Quantity,
        [Required, DataType(DataType.Currency)] decimal Price);
}
