using AlgorithmAWorkerService.Service;
using Christmas.Secret.Gifter.Infrastructure;
using Common.RabbitMQ;

namespace AlgorithmWorkerService {
    public class Program {
        public static void Main(string[] args) {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddRabbitMQServices();
            builder.Services.AddMSSQLServices();
            builder.Services.AddScoped<IML, MLNetExecutorA>();

            var host = builder.Build();
            host.Run();
        }
    }
}
