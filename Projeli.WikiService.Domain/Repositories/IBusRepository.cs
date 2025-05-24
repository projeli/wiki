namespace Projeli.WikiService.Domain.Repositories;

public interface IBusRepository
{
    Task Publish(object @event);
}