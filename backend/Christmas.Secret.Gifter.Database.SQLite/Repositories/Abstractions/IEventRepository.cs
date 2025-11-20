using MSSql.Infrastructure.Entities;

namespace MSSql.Infrastructure.Repositories.Abstractions {
    public interface IEventRepository : IBaseRepository<EventEntry, string>
    {
        //intentionally left blank
    }
}