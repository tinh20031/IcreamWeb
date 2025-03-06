using IcreamShopApi.Data;
using IcreamShopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IcreamShopApi.Repository
{
    public class OrderDetailRepository
    {
        private readonly CreamDbContext _context;

        public OrderDetailRepository(CreamDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDetail>> GetAllOrderDetails()
        {
            return await _context.OrderDetails.ToListAsync();
        }

        public async Task<OrderDetail> GetOrderDetailById(int Id)
        {
            return await _context.OrderDetails.FindAsync(Id);
        }

        public async Task AddOrderDetail(OrderDetail orderDetail)
        {
            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteOrderDetail(int Id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(Id);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task EditOrderDetail(OrderDetail orderDetail)
        {
            var existingOrderDetail = await _context.OrderDetails.FindAsync(orderDetail.OrderDetailId);
            if (existingOrderDetail == null)
            {
                throw new Exception("Khong tim thay OrderDetail");
            }

            _context.Entry(existingOrderDetail).CurrentValues.SetValues(orderDetail);
            await _context.SaveChangesAsync();
        }
    }
}
