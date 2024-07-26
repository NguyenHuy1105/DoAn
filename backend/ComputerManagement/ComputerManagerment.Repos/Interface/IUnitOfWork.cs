
using ComputerManagement.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagerment.Repos.Interface
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        #region Property
        AppDbContext DbContext { get; }
        IDbContextTransaction Transaction { get; }
        #endregion

        #region Methods

        /// <summary>
        /// mở transaction
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// mở transaction async
        /// </summary>
        /// <returns></returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// commit transaction
        /// </summary>
        void Commit();

        /// <summary>
        /// commit transaction async
        /// </summary>
        /// <returns></returns>
        Task CommitAsync();

        /// <summary>
        /// rollback transaction
        /// </summary>
        void Rollback();

        /// <summary>
        /// rollback transaction async
        /// </summary>
        /// <returns></returns>
        Task RollbackAsync();

        #endregion

    }
}
