using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.Notifications.Commands
{
    public record CreateNotificationCommand(string Title, string Message, string Color) : IRequest<Result<Guid>>;

    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Result<Guid>>
    {
        private readonly IRepository<SystemNotification> _repo;
        private readonly IUnitOfWork _unitOfWork;
        public CreateNotificationCommandHandler(IRepository<SystemNotification> repo, IUnitOfWork unitOfWork) { _repo = repo; _unitOfWork = unitOfWork; }

        public async Task<Result<Guid>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = SystemNotification.Create(request.Title, request.Message, request.Color);
            await _repo.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(notification.Id);
        }
    }
}