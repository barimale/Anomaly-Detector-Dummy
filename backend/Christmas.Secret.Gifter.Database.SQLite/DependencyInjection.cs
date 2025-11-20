using AutoMapper.Configuration;
using Christmas.Secret.Gifter.Infrastructure.Repositories;
using Christmas.Secret.Gifter.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Christmas.Secret.Gifter.Infrastructure;
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
