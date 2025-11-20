using Microsoft.Extensions.DependencyInjection;
using MSSql.Infrastructure.Repositories;
using MSSql.Infrastructure.Repositories.Abstractions;

namespace MSSql.Infrastructure.Extensions {
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddSQLLiteDatabase(this IServiceCollection services)
        {
            services.AddTransient<IEventRepository, EventRepository>();
            services.AddTransient<IParticipantRepository, ParticipantRepository>();

            //WIP check it
            services.AddSQLLiteDatabaseAutoMapper();

            return services;
        }

        public static IServiceCollection AddSQLLiteDatabaseAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Mappings));

            return services;
        }
    }
}
