using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SharpQuant.Common.DB
{

    public interface IAsyncRepository
    {
        Task Delete<T>(T entity) where T : class;
        Task<IList<T>> GetAll<T>() where T : class;
        Task<T> GetSingle<T>(long ID) where T : class;
        Task<T> GetSingle<T>(string ID) where T : class;
        Task Insert<T>(T entity) where T : class;
        Task<IList<T>> SearchFor<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<IList<T>> SearchFor<T>(string whereClause) where T : class;
        Task Update<T>(T entity) where T : class;
        T Create<T>() where T : class;
    }

    public class AsyncRepositoryProvider : IAsyncRepository, IDisposable
    {


        class WorkItem
        {
            public readonly Action OnRun;
            public readonly Action OnCancelation;
            public readonly Action<Exception> OnException;
            public readonly CancellationToken? CancelToken;
            public WorkItem(Action onRun, Action onCancelation, Action<Exception> onException, CancellationToken? cancelToken)
            {
                OnRun = onRun;
                OnCancelation = onCancelation;
                OnException = onException;
                CancelToken = cancelToken;
            }
        }


        IRepositoryProvider _context;
        BlockingCollection<WorkItem> _taskqueue;
        Task _dbtask;

        public AsyncRepositoryProvider(Func<IRepositoryProvider> factory)
        {
            _taskqueue = new BlockingCollection<WorkItem>();

            _dbtask = Task.Factory.StartNew(() =>
            {
                //need to create dbcontext within this new task
                _context = factory();
                Run();
            }, TaskCreationOptions.LongRunning);
        }

        void Run()
        {
            foreach (WorkItem workItem in _taskqueue.GetConsumingEnumerable())
                if (workItem.CancelToken.HasValue &&
                workItem.CancelToken.Value.IsCancellationRequested)
                {
                    workItem.OnCancelation();
                }
                else
                    try
                    {
                        //Thread.Sleep(2000);
                        workItem.OnRun();
                    }
                    catch (Exception ex)
                    {
                        workItem.OnException(ex);
                    }

        }


        public Task EnqueueTask(Action run, CancellationToken? cancelToken)
        {
            var tcs = new TaskCompletionSource<object>();

            Action onRun = () => { run(); tcs.SetResult(null); };
            Action onCancelled = () => tcs.SetCanceled();
            Action<Exception> onException = (ex) => tcs.SetException(ex);

            _taskqueue.Add(new WorkItem(onRun, onCancelled, onException, cancelToken));
            return tcs.Task;
        }


        #region wrapped repository methods

        public Task<IList<T>> GetAll<T>() where T:class
        {
            var tcs = new TaskCompletionSource<IList<T>>();
            Action onRun = () =>
            {
                var result = _context.GetRepository<T>().GetAll();
                tcs.SetResult(result);
            };
            Action onCancelled = () => tcs.SetCanceled();
            Action<Exception> onException = (ex) => tcs.SetException(ex);

            _taskqueue.Add(new WorkItem(onRun, onCancelled, onException, null));
            return tcs.Task;
        }

        public Task<IList<T>> SearchFor<T>(string whereClause) where T : class
        {
            var tcs = new TaskCompletionSource<IList<T>>();
            Action onRun = () =>
            {
                var result = _context.GetRepository<T>().SearchFor(whereClause).ToList();
                tcs.SetResult(result);
            };
            Action onCancelled = () => tcs.SetCanceled();
            Action<Exception> onException = (ex) => tcs.SetException(ex);

            _taskqueue.Add(new WorkItem(onRun, onCancelled, onException, null));
            return tcs.Task;
        }

        public Task<IList<T>> SearchFor<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var tcs = new TaskCompletionSource<IList<T>>();
            Action onRun = () =>
            {
                var result = _context.GetRepository<T>().SearchFor(predicate).ToList();
                tcs.SetResult(result);
            };
            Action onCancelled = () => tcs.SetCanceled();
            Action<Exception> onException = (ex) => tcs.SetException(ex);

            _taskqueue.Add(new WorkItem(onRun, onCancelled, onException, null));
            return tcs.Task;
        }

        public Task<T> GetSingle<T>(long ID) where T : class
        {
            var tcs = new TaskCompletionSource<T>();
            Action onRun = () =>
            {
                var result = _context.GetRepository<T>().GetSingle(ID);
                tcs.SetResult(result);
            };
            Action onCancelled = () => tcs.SetCanceled();
            Action<Exception> onException = (ex) => tcs.SetException(ex);

            _taskqueue.Add(new WorkItem(onRun, onCancelled, onException,null));
            return tcs.Task;
        }

        public Task<T> GetSingle<T>(string ID) where T : class
        {
            var tcs = new TaskCompletionSource<T>();
            Action onRun = () =>
            {
                var result = _context.GetRepository<T>().GetSingle(ID);
                tcs.SetResult(result);
            };
            Action onCancelled = () => tcs.SetCanceled();
            Action<Exception> onException = (ex) => tcs.SetException(ex);

            _taskqueue.Add(new WorkItem(onRun, onCancelled, onException, null));
            return tcs.Task;
        }

        public Task Insert<T>(T entity) where T : class
        {
            var tcs = new TaskCompletionSource<T>();
            Action onRun = () =>
            {
                _context.GetRepository<T>().Insert(entity);
                tcs.SetResult(null);
            };
            Action onCancelled = () => tcs.SetCanceled();
            Action<Exception> onException = (ex) => tcs.SetException(ex);

            _taskqueue.Add(new WorkItem(onRun, onCancelled, onException, null));
            return tcs.Task;
        }

        public Task Update<T>(T entity) where T : class
        {
            var tcs = new TaskCompletionSource<T>();
            Action onRun = () =>
            {
                _context.GetRepository<T>().Update(entity);
                tcs.SetResult(null);
            };
            Action onCancelled = () => tcs.SetCanceled();
            Action<Exception> onException = (ex) => tcs.SetException(ex);

            _taskqueue.Add(new WorkItem(onRun, onCancelled, onException, null));
            return tcs.Task;
        }

        public Task Delete<T>(T entity) where T : class
        {
            var tcs = new TaskCompletionSource<T>();
            Action onRun = () =>
            {
                _context.GetRepository<T>().Delete(entity);
                tcs.SetResult(null);
            };
            Action onCancelled = () => tcs.SetCanceled();
            Action<Exception> onException = (ex) => tcs.SetException(ex);

            _taskqueue.Add(new WorkItem(onRun, onCancelled, onException, null));
            return tcs.Task;
        }

        public T Create<T>() where T :class
        {
            return _context.GetRepository<T>().Create();
        }

        #endregion

        #region IDisposable

        ~AsyncRepositoryProvider()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Code to dispose the managed resources of the class
            }
            // Code to dispose the un-managed resources of the class

            _taskqueue.CompleteAdding();
            _dbtask.Wait(500);
            if (_context != null) _context.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
