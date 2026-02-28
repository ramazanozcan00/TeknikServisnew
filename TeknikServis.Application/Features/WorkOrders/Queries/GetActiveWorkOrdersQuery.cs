using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.WorkOrders.DTOs;
using TeknikServis.Application.Interfaces;

namespace TeknikServis.Application.Features.WorkOrders.Queries
{
    // DİKKAT: Dönüş tipi WorkOrderBoardDto olarak ayarlandı!
    public record GetActiveWorkOrdersQuery() : IRequest<Result<List<WorkOrderBoardDto>>>;

    public class GetActiveWorkOrdersQueryHandler : IRequestHandler<GetActiveWorkOrdersQuery, Result<List<WorkOrderBoardDto>>>
    {
        private readonly IWorkOrderQueryRepository _repository;

        public GetActiveWorkOrdersQueryHandler(IWorkOrderQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<WorkOrderBoardDto>>> Handle(GetActiveWorkOrdersQuery request, CancellationToken cancellationToken)
        {
            // Repository'deki o harika metodu çağırıyoruz ve direkt BoardDto listesi alıyoruz
            var activeWorkOrders = await _repository.GetActiveWorkOrdersAsync(cancellationToken);

            return Result<List<WorkOrderBoardDto>>.Success(activeWorkOrders);
        }
    }
}