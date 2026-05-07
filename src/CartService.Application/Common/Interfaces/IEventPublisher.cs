namespace CartService.Application.Common.Interfaces;

public interface IEventPublisher
{
    void TryPublish(string routingKey, object payload);
}
