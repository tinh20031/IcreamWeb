using IcreamShopApi.Data;
using IcreamShopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IcreamShopApi.Repository
{
    public class IceCreamRepository
    {
        private readonly CreamDbContext _context;

        public IceCreamRepository(CreamDbContext context)
        {
            _context = context;
        }

        public async Task<List<IceCream>> GetAllIceCreams()
        {
            return await _context.IceCreams.ToListAsync();
        }

        public async Task<IceCream> GetIcecreamById(int id)
        {
            return await _context.IceCreams.FindAsync(id);
        }

        public async Task<List<IceCream>> Search(string name)
        {
            return await _context.IceCreams
                .Where(s => s.Name.Contains(name.Trim()))
                .ToListAsync();
        }

        public async Task AddIceCream(IceCream iceCream)
        {
            await _context.IceCreams.AddAsync(iceCream);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteIceCream(int id)
        {
            var iceCream = await _context.IceCreams.FindAsync(id);
            if (iceCream == null)
            {
                return false; // Trả về false nếu không tìm thấy
            }
            _context.IceCreams.Remove(iceCream);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task EditIceCream(IceCream iceCream)
        {
            var existingIceCream = await _context.IceCreams.FindAsync(iceCream.IceCreamId);
            if (existingIceCream == null)
            {
                throw new Exception("Không tìm thấy kem!");
            }
            _context.Entry(existingIceCream).CurrentValues.SetValues(iceCream);
            await _context.SaveChangesAsync();
        }
    }
}




