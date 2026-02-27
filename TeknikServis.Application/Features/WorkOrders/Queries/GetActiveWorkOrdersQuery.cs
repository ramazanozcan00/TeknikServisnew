using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.WorkOrders.DTOs;
using TeknikServis.Application.Interfaces;

namespace TeknikServis.Application.Features.WorkOrders.Queries
{
    public record GetActiveWorkOrdersQuery() : IRequest<Result<List<WorkOrderBoardDto>>>;

    public class GetActiveWorkOrdersQueryHandler : IRequestHandler<GetActiveWorkOrdersQuery, Result<List<WorkOrderBoardDto>>>
    {
        private readonly IWorkOrderQueryRepository _queryRepository;
        public GetActiveWorkOrdersQueryHandler(IWorkOrderQueryRepository queryRepository) => _queryRepository = queryRepository;

        public async Task<Result<List<WorkOrderBoardDto>>> Handle(GetActiveWorkOrdersQuery request, CancellationToken cancellationToken)
        {
            var workOrders = await _queryRepository.GetActiveWorkOrdersAsync(cancellationToken);
            return Result<List<WorkOrderBoardDto>>.Success(workOrders);
        }
    }
}