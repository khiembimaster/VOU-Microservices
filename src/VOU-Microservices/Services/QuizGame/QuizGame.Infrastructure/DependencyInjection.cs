using BuildingBlocks.Behaviors;
using System.Reflection;

namespace QuizGame.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices
           (this IServiceCollection services, IConfiguration configuration)
        {
            //MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            //Scheduler
            services.AddQuartz();
            services.AddQuartzHostedService(opt =>
            {
                opt.WaitForJobsToComplete = true;
            });

            //MassTransit RabbitMQ
            //services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
