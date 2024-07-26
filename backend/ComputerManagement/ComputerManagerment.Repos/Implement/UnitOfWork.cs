using ComputerManagement.BO;
using ComputerManagement.Data;
using ComputerManagerment.Repos.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace ComputerManagerment.Repos.Implement
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private IDbContextTransaction? _dbTransaction = null;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AppDbContext DbContext => _dbContext;

        public IDbContextTransaction Transaction => _dbTransaction;

        public void BeginTransaction()
        {
            _dbTransaction = _dbContext.Database.BeginTransaction();
        }

        public async Task BeginTransactionAsync()
        {
            _dbTransaction = await _dbContext.Database.BeginTransactionAsync();

        }

        public void Commit()
        {
            _dbTransaction?.Commit();
        }

        public async Task CommitAsync()
        {
            if (_dbTransaction != null)
            {
                await _dbTransaction.CommitAsync();
            }
        }

        public void Dispose()
        {
            _dbTransaction?.Dispose();
            _dbTransaction = null;
            _dbContext.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_dbTransaction != null)
            {
                await _dbTransaction.DisposeAsync();
            }
            await _dbContext.DisposeAsync();
        }

        public void Rollback()
        {
            _dbTransaction?.Rollback();
            Dispose();
        }

        public async Task RollbackAsync()
        {
            if (_dbTransaction != null)
            {
                await _dbTransaction.RollbackAsync();
            }
            await DisposeAsync();
        }
    }
}
