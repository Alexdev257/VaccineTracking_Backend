using AutoMapper;
using ClassLib.DTO.Address;
using ClassLib.Models;
using ClassLib.Repositories;

namespace ClassLib.Service.Addresses
{
    public class AddressService
    {
        private readonly AddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public AddressService(AddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<List<Address>> GetAllAddresses()
        {
            return (List<Address>)await _addressRepository.GetAllAddresses();
        }

        public async Task<Address?> GetAddressById(int id)
        {
            return await _addressRepository.GetAddressById(id);
        }

        public async Task<Address?> AddAddress(AddAddress addAddress)
        {
            return await _addressRepository.AddAddress(addAddress);
        }

        public async Task<bool> DeleteAddress(int id)
        {
            return await _addressRepository.DeleteAddress(id);
        }

        public async Task<Address?> UpdateAddress(int id, AddAddress updateAddress)
        {
            return await _addressRepository.UpdateAddress(id, updateAddress);
        }

        public async Task<List<GetSourceMap>> GetAllMap()
        {
            var addresses = await _addressRepository.GetAll();
            return _mapper.Map<List<GetSourceMap>>(addresses);
        }
    }
}