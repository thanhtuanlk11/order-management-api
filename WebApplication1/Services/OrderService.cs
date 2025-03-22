using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Interface;

namespace WebApplication1.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(OrderDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            order.CreatedAt = DateTime.Now;
            order.UpdatedAt = DateTime.Now;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<List<OrderDto>> GetOrdersAsync()
        {
            var orders = await _context.Orders.ToListAsync();
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<PagedResultDto<OrderDto>> GetOrdersAsync(int page, int pageSize)
        {
            {

                page = page < 1 ? 1 : page;
                pageSize = pageSize < 1 ? 10 : pageSize;


                var totalItems = await _context.Orders.CountAsync();
                var orders = await _context.Orders
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var orderDtos = _mapper.Map<List<OrderDto>>(orders);
                return new PagedResultDto<OrderDto>(orderDtos, page, pageSize, totalItems);
            }
        }
        public async Task<OrderDto> GetOrderAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return null;
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<bool> UpdateOrderAsync(int id, OrderDto orderDto)
        {
            if (id != orderDto.Id)
            {
                return false;
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }

            _mapper.Map(orderDto, order);
            order.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<OrderDetailDto> AddOrderDetailAsync(int orderId, OrderDetailDto detailDto)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return null;
            }

            var detail = _mapper.Map<OrderDetail>(detailDto);
            detail.OrderId = orderId;

            _context.OrderDetails.Add(detail);
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderDetailDto>(detail);
        }

        public async Task<List<OrderDetailDto>> GetOrderDetailsAsync(int orderId)
        {
            var details = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            return _mapper.Map<List<OrderDetailDto>>(details);
        }

        public async Task<bool> DeleteOrderDetailAsync(int detailId)
        {
            var detail = await _context.OrderDetails.FindAsync(detailId);
            if (detail == null)
            {
                return false;
            }

            _context.OrderDetails.Remove(detail);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
