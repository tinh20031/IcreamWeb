using IcreamShopApi.Models;
using IcreamShopApi.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IcreamShopApi.Services
{
    public class AddressService
    {
        private readonly AddressRepository _addressRepository;

        public AddressService(AddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<List<Address>> GetAddressesByUserId(int userId)
        {
            return await _addressRepository.GetAddressesByUserId(userId);
        }

        public async Task<Address> GetAddressById(int addressId)
        {
            return await _addressRepository.GetAddressById(addressId);
        }

        public async Task AddAddress(Address address)
        {
            await _addressRepository.AddAddress(address);
        }

        public async Task UpdateAddress(Address address)
        {
            await _addressRepository.UpdateAddress(address);
        }

        public async Task DeleteAddress(int addressId)
        {
            await _addressRepository.DeleteAddress(addressId);
        }
    }
}