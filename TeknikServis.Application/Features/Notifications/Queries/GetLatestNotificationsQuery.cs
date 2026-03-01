using MediatR;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.Notifications.DTOs;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.Notifications.Queries
{
    public record GetLatestNotificationsQuery(int Count = 5) : IRequest<Result<List<NotificationDto>>>;

    public class GetLatestNotificationsQueryHandler : IRequestHandler<GetLatestNotificationsQuery, Result<List<NotificationDto>>>
    {
        private readonly IRepository<SystemNotification> _repository;
        public GetLatestNotificationsQueryHandler(IRepository<SystemNotification> repository) => _repository = repository;

        public async Task<Result<List<NotificationDto>>> Handle(GetLatestNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _repository.GetAllAsync(cancellationToken);
            var result = notifications.OrderByDescending(n => n.CreatedAt)
                                      .Take(request.Count)
                                      .Select(n => new NotificationDto(n.Id, n.Title, n.Message, n.Color, n.CreatedAt))
                                      .ToList();
            return Result<List<NotificationDto>>.Success(result);
        }
    }
}