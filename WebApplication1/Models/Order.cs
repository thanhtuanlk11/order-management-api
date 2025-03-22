namespace WebApplication1.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; } // 0: Pending, 1: Completed, 2: Canceled
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public System.Collections.Generic.List<OrderDetail> OrderDetails { get; set; }
    
}
}
