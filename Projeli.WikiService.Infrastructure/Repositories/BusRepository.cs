using MassTransit;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Infrastructure.Repositories;

public class BusRepository(IBus bus) : IBusRepository
{
    public Task Publish(object message)
    {
        return bus.Publish(message);
    }
}