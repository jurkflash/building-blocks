using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Dispatching;
using Pokok.BuildingBlocks.Cqrs.Events;
using Pokok.BuildingBlocks.Cqrs.Validation;

namespace Pokok.BuildingBlocks.Cqrs.Extensions
{
    public static class CqrsRegistrationExtensions
    {
        // With Validator
        public static IServiceCollection AddCommandHandler<TCommand, TResult, THandler, TValidator>(
            this IServiceCollection services)
            where TCommand : ICommand<TResult>
            where THandler : class, ICommandHandler<TCommand, TResult>
            where TValidator : class, IValidator<TCommand>
        {
            services.AddScoped<THandler>();
            services.AddScoped<IValidator<TCommand>, TValidator>();

            services.AddScoped<ICommandHandler<TCommand, TResult>>(provider =>
            {
                var handler = provider.GetRequiredService<THandler>();
                var validators = provider.GetServices<IValidator<TCommand>>();
                var logger = provider.GetRequiredService<ILogger<ValidatingCommandHandler<TCommand, TResult>>>();

                return new ValidatingCommandHandler<TCommand, TResult>(handler, validators, logger);
            });

            return services;
        }

        // Without Validator
        public static IServiceCollection AddCommandHandler<TCommand, TResult, THandler>(
            this IServiceCollection services)
            where TCommand : ICommand<TResult>
            where THandler : class, ICommandHandler<TCommand, TResult>
        {
            services.AddScoped<ICommandHandler<TCommand, TResult>, THandler>();
            return services;
        }

        public static IServiceCollection AddQueryHandler<TQuery, TResult, THandler>(
        this IServiceCollection services)
        where TQuery : IQuery<TResult>
        where THandler : class, IQueryHandler<TQuery, TResult>
        {
            services.AddScoped<IQueryHandler<TQuery, TResult>, THandler>();
            return services;
        }

        public static IServiceCollection AddQueryHandler<TQuery, TResult, THandler, TValidator>(
            this IServiceCollection services)
            where TQuery : IQuery<TResult>
            where THandler : class, IQueryHandler<TQuery, TResult>
            where TValidator : class, IValidator<TQuery>
        {
            services.AddScoped<THandler>();
            services.AddScoped<IValidator<TQuery>, TValidator>();

            services.AddScoped<IQueryHandler<TQuery, TResult>>(provider =>
            {
                var handler = provider.GetRequiredService<THandler>();
                var validators = provider.GetServices<IValidator<TQuery>>();
                var logger = provider.GetRequiredService<ILogger<ValidatingQueryHandler<TQuery, TResult>>>();

                return new ValidatingQueryHandler<TQuery, TResult>(handler, validators, logger);
            });

            return services;
        }

        public static IServiceCollection AddDomainEventDispatcher(this IServiceCollection services)
        {
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            return services;
        }
    }
}
