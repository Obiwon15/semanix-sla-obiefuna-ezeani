using FormsService.Domain.Events;

namespace FormsService.Application.Services;

public interface IEventPublisher
{
    Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}