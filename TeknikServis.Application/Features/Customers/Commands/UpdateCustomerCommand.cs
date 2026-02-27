using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.Customers.Commands
{
    public record UpdateCustomerCommand(Guid Id, string FirstName, string LastName, string Email, string PhoneNumber) : IRequest<Result<Guid>>;

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<Guid>>
    {
        private readonly IRepository<Customer> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCustomerCommandHandler(IRepository<Customer> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (customer == null) return Result<Guid>.Failure("Müşteri bulunamadı.");

            // Domain katmanındaki Update metodumuzu tetikliyoruz
            customer.Update(request.FirstName, request.LastName, request.Email, request.PhoneNumber);

            _repository.Update(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(customer.Id);
        }
    }
}