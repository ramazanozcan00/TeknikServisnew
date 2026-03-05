using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.Customers.Commands
{
    // 1. Kuryeye City ve District alanlarını ekledik
    public record UpdateCustomerCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string? TaxNumber,
        string? TaxOffice,
        string? City,      // EKLENDİ
        string? District,  // EKLENDİ
        string? Address,
        string? Notes
    ) : IRequest<Result<bool>>;

    // 2. Kuryeyi teslim alan Handler
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<bool>>
    {
        private readonly IRepository<Customer> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCustomerCommandHandler(IRepository<Customer> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (customer == null) return Result<bool>.Failure("Müşteri bulunamadı.");

            // BURASI DÜZELTİLDİ: Update metoduna eksik olan City ve District parametreleri eklendi!
            customer.Update(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.TaxNumber,
                request.TaxOffice,
                request.City,      // EKLENDİ
                request.District,  // EKLENDİ
                request.Address,
                request.Notes);

            _repository.Update(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}