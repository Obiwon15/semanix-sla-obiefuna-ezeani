using MediatR;
using FormsService.Application.Queries;
using FormsService.Application.DTOs;
using FormsService.Application.Services;

namespace FormsService.Application.Handlers;

public class GetFormsHandler : IRequestHandler<GetFormsQuery, List<FormDto>>
{
    private readonly IFormQueryService _queryService;

    public GetFormsHandler(IFormQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<List<FormDto>> Handle(GetFormsQuery request, CancellationToken cancellationToken)
    {
        return await _queryService.GetFormsAsync(request.TenantId, request.EntityId, cancellationToken);
    }
}