namespace testttt.Application.Interfaces;

/// <summary>
/// Unit of Work pattern برای مدیریت Transaction و SaveChanges
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// ذخیره تمام تغییرات در database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// شروع یک Transaction جدید
    /// </summary>
    Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface برای مدیریت Transaction
/// </summary>
public interface IUnitOfWorkTransaction : IAsyncDisposable
{
    /// <summary>
    /// Commit کردن Transaction
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rollback کردن Transaction
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

