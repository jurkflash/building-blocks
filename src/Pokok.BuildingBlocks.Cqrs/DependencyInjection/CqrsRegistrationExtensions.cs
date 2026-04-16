using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Dispatching;
using Pokok.BuildingBlocks.Cqrs.Events;
using Pokok.BuildingBlocks.Cqrs.Validation;

namespace Pokok.BuildingBlocks.Cqrs.Extensions
{
    /// <summary>
    /// Extension methods for registering CQRS command handlers, query handlers, validators,
    /// and domain event dispatchers in the dependency injection container.
    /// Handles automatic wiring of validation decorators when validators are provided.
    /// </summary>
    public static class CqrsRegistrationExtensions
    {
        /// <summary>
        /// Registers a command handler with an associated validator.
        /// The handler is wrapped in a <see cref="ValidatingCommandHandler{TCommand, TResult}"/> decorator.
        /// </summary>
        /// <typeparam name="TCommand">The command type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <typeparam name="THandler">The command handler implementation type.</typeparam>
        /// <typeparam name="TValidator">The validator implementation type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
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

        /// <summary>
        /// Registers a command handler without validation.
        /// </summary>
        /// <typeparam name="TCommand">The command type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <typeparam name="THandler">The command handler implementation type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        // Without Validator
        public static IServiceCollection AddCommandHandler<TCommand, TResult, THandler>(
            this IServiceCollection services)
            where TCommand : ICommand<TResult>
            where THandler : class, ICommandHandler<TCommand, TResult>
        {
            services.AddScoped<ICommandHandler<TCommand, TResult>, THandler>();
            return services;
        }

        /// <summary>
        /// Registers a query handler without validation.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <typeparam name="THandler">The query handler implementation type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddQueryHandler<TQuery, TResult, THandler>(
        this IServiceCollection services)
        where TQuery : IQuery<TResult>
        where THandler : class, IQueryHandler<TQuery, TResult>
        {
            services.AddScoped<IQueryHandler<TQuery, TResult>, THandler>();
            return services;
        }

        /// <summary>
        /// Registers a query handler with an associated validator.
        /// The handler is wrapped in a <see cref="ValidatingQueryHandler{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <typeparam name="THandler">The query handler implementation type.</typeparam>
        /// <typeparam name="TValidator">The validator implementation type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
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

        /// <summary>
        /// Registers the <see cref="DomainEventDispatcher"/> as the <see cref="IDomainEventDispatcher"/> implementation.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddDomainEventDispatcher(this IServiceCollection services)
        {
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            return services;
        }
    }
}
