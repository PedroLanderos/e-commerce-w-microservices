using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services
{
    //La clase sirve para implementar las interfaces, despues usa un http client que tiene el request hecho, el resilienciepipeline sirve para hacer x numero de request y reintentar en caso de no tener exito n veces
    public class OrderService(IOrder orderInterface, HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        //obtiene el producto llamando al endpoint que tiene el microservicio (api/products/productid)
        public async Task<ProductDto> GetProduct(int productId)
        {
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null!;
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDto>();
            return product!;

        }

        //obtiene el usuario/cliente que hizo la peticion
        public async Task<AppUserDto> GetUser(int userId)
        {
            var getuser = await httpClient.GetAsync($"/api/authentication/{userId}");
            if(!getuser.IsSuccessStatusCode)
                return null!;

            var product = await getuser.Content.ReadFromJsonAsync<AppUserDto>();
            return product!;
            

        }

        public async Task<OrderDetailsDto> GetOrderDetails(int orderId)
        {
            var order = await orderInterface.FindByIdAsync(orderId); 
            if(order is null || order!.Id <= 0)
                return null!;

            var retryPipeline = resiliencePipeline.GetPipeline("retry-pipeline");

            var productDto = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            var appUserDto = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            return new OrderDetailsDto(
                order.Id,
                productDto.Id,
                appUserDto.Id,
                appUserDto.Name,
                appUserDto.Email,
                appUserDto.Address,
                appUserDto.TelephoneNumber,
                productDto.Name,
                order.PurchaseQuantity,
                productDto.Price,
                productDto.Quantity * order.PurchaseQuantity,
                order.OrderedDate
                );
        }

        public async Task<IEnumerable<OrderDto>> GetOrderByClientId(int clientId)
        {
            var orders = await orderInterface.GetOrdersAsync(x => x.ClientId == clientId);
            if (!orders.Any())
                return null!;

            var (_, _orders) = OrderConversion.FromEntity(null, orders);

            return _orders!; 
        }

    }
}
