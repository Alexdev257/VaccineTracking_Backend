using ClassLib.DTO.PaymentMethod;
using ClassLib.Models;
using ClassLib.Repositories;

namespace ClassLib.Service
{
    public class PaymentMethodService
    {
        private readonly PaymentMethodRepository _paymentMethodRepository;
        public PaymentMethodService(PaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }
        public async Task<List<PaymentMethod>> getAll() => await _paymentMethodRepository.getAll();
        public async Task<PaymentMethod?> getPaymentMethodById(int id) => await _paymentMethodRepository.getPaymentMethodById(id);
        public async Task<PaymentMethod?> getPaymentMethodByName(string name) => await _paymentMethodRepository.getPaymentMethodByName(name);

        public async Task<PaymentMethod?> addPaymentMethod(AddPaymentMethod paymentMethodAdding)
        {
            PaymentMethod paymentMethod = new()
            {
                Name = paymentMethodAdding.Name,
                Description = paymentMethodAdding.Decription
            };
            return await _paymentMethodRepository.addPaymentMethod(paymentMethod);
        }

        public async Task<PaymentMethod?> updatePaymentMethod(int id, UpdatePaymentMethod paymentMethodUpdating)
        {
            return await _paymentMethodRepository.updatePaymentMethod(id, paymentMethodUpdating.Name, paymentMethodUpdating.Decription);
        }
    }
}