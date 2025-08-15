using MediatR;
using FormsService.Application.Commands;
using FormsService.Application.Services;
using FormsService.Domain.Repositories;
using FormsService.Domain.ValueObjects;
using FormsService.Domain.Exceptions;

namespace FormsService.Application.Handlers;

public class PublishFormHandler : IRequestHandler<PublishFormCommand>
{
    private readonly IFormRepository _formRepository;
    private readonly IEventPublisher _eventPublisher;

    public PublishFormHandler(IFormRepository formRepository, IEventPublisher eventPublisher)
    {
        _formRepository = formRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(PublishFormCommand request, CancellationToken cancellationToken)
    {
        var formId = new FormId(request.FormId);
        var tenantId = new TenantId(request.TenantId);

        var form = await _formRepository.GetByIdAsync(formId, tenantId, cancellationToken);
        if (form == null)
            throw new DomainException("Form not found");

        form.Publish();

        await _formRepository.UpdateAsync(form, cancellationToken);
        await _eventPublisher.PublishDomainEventsAsync(form.DomainEvents, cancellationToken);
    }
}