using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.DTOs;

namespace WebApplication1.Interface
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(OrderDto orderDto);
        Task<PagedResultDto<OrderDto>> GetOrdersAsync(int page, int pageSize);
        Task<OrderDto> GetOrderAsync(int id);
        Task<bool> UpdateOrderAsync(int id, OrderDto orderDto);
        Task<bool> DeleteOrderAsync(int id);
        Task<OrderDetailDto> AddOrderDetailAsync(int orderId, OrderDetailDto detailDto);
        Task<List<OrderDetailDto>> GetOrderDetailsAsync(int orderId);
        Task<bool> DeleteOrderDetailAsync(int detailId);
    }
}
