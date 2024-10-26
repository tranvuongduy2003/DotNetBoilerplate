using Domain.Abstractions.SeedWork.UnitOfWork;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.Abstractions.UnitOfWork;

/// <summary>
/// Represents a unit of work pattern implementation, managing database operations and caching.
/// </summary>
/// <remarks>
/// The Unit of Work pattern is used to handle transactions across multiple repositories and coordinate changes
/// in a single transaction. This class also integrates with a caching service to optimize performance and reduce
/// database load.
/// </remarks>
public class UnitOfWork : IUnitOfWork
{
    private bool _disposed;
    private readonly ApplicationDbContext _context;

    // private CategoriesRepository _categories;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context used for database operations.</param>
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // public ICategoriesRepository Categories
    // {
    //     get
    //     {
    //         if (_categories == null)
    //             _categories = new CategoriesRepository(_context);
    //         return _categories;
    //     }
    // }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Task<int> CommitAsync()
    {
        return _context.SaveChangesAsync();
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return _context.Database.BeginTransactionAsync();
    }

    public async Task EndTransactionAsync()
    {
        await CommitAsync();
        await _context.Database.CommitTransactionAsync();
    }

    public Task RollbackTransactionAsync()
    {
        return _context.Database.RollbackTransactionAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing)
                _context.Dispose();
        _disposed = true;
    }
}