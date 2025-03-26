using IcreamShopApi.Data;
using IcreamShopApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IcreamShopApi.Repository
{
    public class AddressRepository
    {
        private readonly CreamDbContext _context;

        public AddressRepository(CreamDbContext context)
        {
            _context = context;
        }

        public async Task<List<Address>> GetAddressesByUserId(int userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Address> GetAddressById(int addressId)
        {
            return await _context.Addresses.FindAsync(addressId);
        }

        public async Task AddAddress(Address address)
        {
            // Nếu địa chỉ mới là mặc định, bỏ mặc định của các địa chỉ khác
            if (address.IsDefault)
            {
                var existingAddresses = await _context.Addresses
                    .Where(a => a.UserId == address.UserId && a.IsDefault)
                    .ToListAsync();
                foreach (var addr in existingAddresses)
                {
                    addr.IsDefault = false;
                }
            }

            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAddress(Address address)
        {
            var existingAddress = await _context.Addresses.FindAsync(address.AddressId);
            if (existingAddress == null)
            {
                throw new Exception("Không tìm thấy địa chỉ.");
            }

            // Nếu địa chỉ được cập nhật thành mặc định, bỏ mặc định của các địa chỉ khác
            if (address.IsDefault)
            {
                var existingAddresses = await _context.Addresses
                    .Where(a => a.UserId == address.UserId && a.IsDefault && a.AddressId != address.AddressId)
                    .ToListAsync();
                foreach (var addr in existingAddresses)
                {
                    addr.IsDefault = false;
                }
            }

            _context.Entry(existingAddress).CurrentValues.SetValues(address);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAddress(int addressId)
        {
            var address = await _context.Addresses.FindAsync(addressId);
            if (address != null)
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
            }
        }
    }
}