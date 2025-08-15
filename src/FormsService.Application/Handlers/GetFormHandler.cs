using MediatR;
using FormsService.Application.Queries;
using FormsService.Application.DTOs;
using FormsService.Application.Services;
using FormsService.Domain.ValueObjects;

namespace FormsService.Application.Handlers;

public class GetFormHandler : IRequestHandler<GetFormQuery, FormDto?>
{
    private readonly IFormQueryService _queryService;

    public GetFormHandler(IFormQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<FormDto?> Handle(GetFormQuery request, CancellationToken cancellationToken)
    {
        return await _queryService.GetFormAsync(request.FormId, request.TenantId, cancellationToken);
    }
}