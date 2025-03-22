using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string CustomerName { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        [Range(0, 2)]
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
