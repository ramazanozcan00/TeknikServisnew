using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.Customers.Commands
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
    {
        private readonly IRepository<Customer> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerCommandHandler(IRepository<Customer> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            // Yeni parametreleri Domain varlığına gönderiyoruz
            var customer = Customer.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.TaxNumber, // Yeni eklendi
                request.TaxOffice, // Yeni eklendi
                request.Address,   // Yeni eklendi
                request.Notes      // Yeni eklendi
            );

            await _repository.AddAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Cari kod zaten Domain (Customer.cs) içinde otomatik oluşturuldu
            return Result<Guid>.Success(customer.Id);
        }
    }
}