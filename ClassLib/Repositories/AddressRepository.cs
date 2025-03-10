using ClassLib.DTO.Address;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class AddressRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;

        public AddressRepository(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Address>> GetAllAddresses()
        {
            return await _context.Addresses.ToListAsync();
        }

        public async Task<Address?> GetAddressById(int id)
        {
            return await _context.Addresses.FindAsync(id);
        }

        public async Task<Address?> AddAddress(AddAddress addAddress)
        {
            if (addAddress == null) throw new ArgumentNullException(nameof(addAddress));
            if (string.IsNullOrWhiteSpace(addAddress.Name)) throw new ArgumentException("Address name cannot be empty", nameof(addAddress));

            var address = new Address
            {
                Name = addAddress.Name
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return address;
        }

        public async Task<bool> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Address?> UpdateAddress(int id, AddAddress updateAddress)
        {
            if (updateAddress == null) throw new ArgumentNullException(nameof(updateAddress));
            if (string.IsNullOrWhiteSpace(updateAddress.Name)) throw new ArgumentException("Address name cannot be empty", nameof(updateAddress));

            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return null;

            address.Name = updateAddress.Name;
            await _context.SaveChangesAsync();
            return address;
        }
    }
}