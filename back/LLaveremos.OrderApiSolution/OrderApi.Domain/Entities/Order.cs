﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get;set; }
        public int ClientId { get; set; }
        public int PurchaseQuantity { get; set; }
        public DateTime OrderedDate { get; set; } = DateTime.Now;    
    }
}
