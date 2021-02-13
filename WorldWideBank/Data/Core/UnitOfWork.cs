using System;
using System.Data;
using System.Threading.Tasks;
using NHibernate;

namespace WorldWideBank.Data.Core
{
    public interface IUnitOfWork : IDisposable
    {
        void Start();
        void Commit();
        Task CommitAsync();
        void Invalidate();
        void Start(IsolationLevel isolationlevel);

    }

    public class UnitOfWork : IUnitOfWork
    {
        readonly ISession session;
        ITransaction currentTransaction;
        private bool hasBeenInvalidated { get; set; }

        public UnitOfWork(ISession session)
        {
            this.session = session;
        }

        public void Start()
        {
            currentTransaction = session.BeginTransaction();
            currentTransaction.Begin();
        }

        public void Start(IsolationLevel isolationlevel)
        {
            currentTransaction = session.BeginTransaction(isolationlevel);
        }

        public void Commit()
        {
            if (hasBeenInvalidated)
                return;

            if (currentTransaction == null)
            {
                throw new UnitOfWorkNotStartedException();
            }
            session.Flush();
            currentTransaction.Commit();
        }

        public async Task CommitAsync()
        {
            if (hasBeenInvalidated)
                return;

            if (currentTransaction == null)
            {
                throw new UnitOfWorkNotStartedException();
            }

            await session.FlushAsync();
            await currentTransaction.CommitAsync();
        }


        public void Invalidate()
        {
            hasBeenInvalidated = true;
        }


        public void Dispose()
        {
            if (currentTransaction != null && !currentTransaction.WasCommitted)
                currentTransaction.Rollback();

            session.Dispose();
        }
    }
}
