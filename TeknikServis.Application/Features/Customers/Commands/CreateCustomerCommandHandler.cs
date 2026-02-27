using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.Customers.Commands
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        // Dependency Injection (Bağımlılık Enjeksiyonu) - Interfaceleri istiyoruz
        public CreateCustomerCommandHandler(IRepository<Customer> customerRepository, IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Domain nesnesini oluştur (Kendi yazdığımız Create metodu ile)
                var customer = Customer.Create(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber,
                    request.TaxNumber);

                // 2. Hafızaya ekle
                await _customerRepository.AddAsync(customer, cancellationToken);

                // 3. Veritabanına kaydet (Commit)
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 4. Başarılı sonucu ve yeni oluşan ID'yi dön
                return Result<Guid>.Success(customer.Id);
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj dön
                return Result<Guid>.Failure($"Müşteri eklenirken bir hata oluştu: {ex.Message}");
            }
        }
    }
}