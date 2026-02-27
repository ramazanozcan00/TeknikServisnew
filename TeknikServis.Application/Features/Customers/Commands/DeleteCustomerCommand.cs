using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.Customers.Commands
{
    public record DeleteCustomerCommand(Guid Id) : IRequest<Result<bool>>;

    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<bool>>
    {
        private readonly IRepository<Customer> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustomerCommandHandler(IRepository<Customer> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (customer == null) return Result<bool>.Failure("Müşteri bulunamadı.");

            _repository.Delete(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}