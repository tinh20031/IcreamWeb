using IcreamShopApi.Data;
using IcreamShopApi.Models;
using IcreamShopApi.Repository;

namespace IcreamShopApi.Services
{
    public class OrderDetailService
    {
        private readonly OrderDetailRepository _orderDetailRepository;
        private readonly CreamDbContext _context;

        public OrderDetailService(OrderDetailRepository orderDetailRepository, CreamDbContext context)
        {
            _orderDetailRepository = orderDetailRepository;
            _context = context;
        }

        public async Task<List<OrderDetail>> GetAllOrderDetails()
        {
            return await _orderDetailRepository.GetAllOrderDetails();
        }

        public async Task<OrderDetail> GetOrderDetailById(int Id)
        {
            return await _orderDetailRepository.GetOrderDetailById(Id);
        }

        public async Task<OrderDetail> AddOrderDetail(OrderDetail orderDetail)
        {
            await _orderDetailRepository.AddOrderDetail(orderDetail);
            return orderDetail;
        }

        public async Task<bool> DeleteOrderDetail(int Id)
        {
            await _orderDetailRepository.DeleteOrderDetail(Id);
            return true;
        }

        public async Task EditOrderDetail(OrderDetail orderDetail)
        {
            var existingOrderDetail = await _orderDetailRepository.GetOrderDetailById(orderDetail.OrderDetailId);
            if (existingOrderDetail == null)
            {
                throw new Exception("Khong tim thay OrderDetail");
            }

            await _orderDetailRepository.EditOrderDetail(orderDetail);
        }
    }
}
