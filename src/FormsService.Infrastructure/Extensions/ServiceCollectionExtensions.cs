using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using FormsService.Application.Services;
using FormsService.Domain.Repositories;
using FormsService.Infrastructure.Persistence;
using FormsService.Infrastructure.Repositories;
using FormsService.Infrastructure.Queries;
using FormsService.Infrastructure.Messaging;
using FluentValidation;
using MediatR;
using System.Reflection;

namespace FormsService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<FormsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IFormRepository, FormRepository>();

        // Query Services
        services.AddScoped<IFormQueryService>(provider =>
            new FormQueryService(configuration.GetConnectionString("DefaultConnection")!));

        // RabbitMQ
        services.AddSingleton<IConnection>(provider =>
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(configuration.GetConnectionString("RabbitMQ")!)
            };
            return factory.CreateConnection();
        });

        services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}