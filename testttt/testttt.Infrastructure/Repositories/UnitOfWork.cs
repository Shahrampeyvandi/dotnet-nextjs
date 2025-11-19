using Microsoft.EntityFrameworkCore.Storage;
using testttt.Application.Interfaces;
using testttt.Infrastructure.Data;

namespace testttt.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ECommerceDbContext _context;

    public UnitOfWork(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return new UnitOfWorkTransaction(transaction);
    }
}

internal class UnitOfWorkTransaction : IUnitOfWorkTransaction
{
    private readonly IDbContextTransaction _transaction;

    public UnitOfWorkTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();
    }
}
