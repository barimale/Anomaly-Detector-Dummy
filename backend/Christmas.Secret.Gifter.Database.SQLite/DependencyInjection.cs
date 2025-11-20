using AutoMapper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSSql.Infrastructure.Repositories;
using MSSql.Infrastructure.Repositories.Abstractions;

namespace MSSql.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddMSSQLServices
        (this IServiceCollection services)
    {
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddDbContext<GifterDbContext>(options =>
            options
                .UseSqlite("Data Source=Application.db;Cache=Shared",
            b => b.MigrationsAssembly(typeof(GifterDbContext).Assembly.FullName)));
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }
}
