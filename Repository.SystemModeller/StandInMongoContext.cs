// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Mongo.Context;
using MongoDB.Driver;

namespace Econolite.OdeRepository.SystemModeller;

public class StandInMongoContext : IMongoContext
{
    public void Dispose() {}

    public void AddCommand(Func<CancellationToken, Task> func) {}

#pragma warning disable CS1998
    public async Task<(bool success, string? errors)> SaveChangesAsync(CancellationToken? cancellationToken = null)
#pragma warning restore CS1998
    {
        return (true, null);
    }

    public IMongoCollection<TDocument> GetCollection<TDocument>(string name)
    {
#pragma warning disable CS8603
        return null;
#pragma warning restore CS8603
    }
}
