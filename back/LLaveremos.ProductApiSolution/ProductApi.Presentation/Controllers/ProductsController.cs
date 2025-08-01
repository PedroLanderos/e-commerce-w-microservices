using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    //El controlador sirve para mandar las solicitudes http previamente definidas por la interfaz e implementadas usando una extension de la interfaz del producto por lo que ahora solamente se llamana a las implementaciones de cada interfaz para obtener una respuesta de la base de datos a traves de un contexto definido
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController(IProduct productInterface) : ControllerBase 
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();
            if(!products.Any())
                return NotFound("No products in the database");
            var (_, list) = ProductConversion.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No product found");
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAvailableProducts()
        {
            var products = await productInterface.GetAllAsync();

            // Filtrar productos con cantidad mayor a 0
            var availableProducts = products.Where(product => product.Quantity > 0);

            if (!availableProducts.Any())
                return NotFound("No hay productos disponibles.");

            var (_, productDtos) = ProductConversion.FromEntity(null!, availableProducts.ToList());

            return Ok(productDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
                return NotFound("The product has not been find");

            var (_product, _) = ProductConversion.FromEntity(product, null!);
            return _product is not null ? Ok(product) : NotFound("Product is not found");
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.CreateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response); 
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.DeleteAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

    }
}
