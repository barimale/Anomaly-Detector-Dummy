using Common.RabbitMQ;
using Christmas.Secret.Gifter.Infrastructure;

namespace AlgorithmWorkerService {
    public class Program {
        public static void Main(string[] args) {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddRabbitMQServices();
            builder.Services.AddMSSQLServices();

            var host = builder.Build();
            host.Run();
        }
    }
}
