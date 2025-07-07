﻿namespace Pokok.BuildingBlocks.Cqrs.Abstractions
{
    public interface IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
}
