using OrderApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDto order) => new()
        {
            Id = order.Id,
            ClientId = order.ClientId,
            ProductId = order.ProductId,
            OrderedDate = order.OrderedDate,
            PurchaseQuantity = order.PurchaseQuantity
        };

        public static (OrderDto?, IEnumerable<OrderDto>?) FromEntity(Order? order, IEnumerable<Order>? orders)
        {
            // Si es solo una orden
            if (order is not null && orders is null)
            {
                var singleOrder = new OrderDto(
                    order.Id,
                    order.ClientId,
                    order.ProductId,
                    order.PurchaseQuantity,
                    order.OrderedDate
                );
                return (singleOrder, null);
            }

            // Si es más de una orden
            if (orders is not null && order is null)
            {
                if (!orders.Any())
                {
                    // Si la colección está vacía, devolvemos una lista vacía
                    return (null, Enumerable.Empty<OrderDto>());
                }

                var _orders = orders.Select(o => new OrderDto(
                    o.Id,
                    o.ClientId,
                    o.ProductId,
                    o.PurchaseQuantity,
                    o.OrderedDate
                ));
                return (null, _orders);
            }

            // Si ambas son nulas o no se aplica ninguna condición
            return (null, null);
        }

    }
}
