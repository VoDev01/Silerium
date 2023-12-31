﻿using System.ComponentModel.DataAnnotations;

namespace Silerium.Models
{
    public enum OrderStatus { ISSUING, OPENED, PENDING, CLOSED, DELIVERY }
    public class Order
    {
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public float TotalPrice { get; set; }
        public int OrderAmount { get; set; }
        public DateTime OrderDate { get; set; }
        [MaxLength(200)]
        [Required]
        public string OrderAddress { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.ISSUING;
    }
}
