/*
The MIT License (MIT)

Copyright (c) 2014 Joachim Loebb

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using System.Threading;



namespace SharpQuant.Batchjobs
{
    public class BatchJobExceptionEventArgs : EventArgs
    {
        private Exception _e;

        public Exception Exception
        {
            get { return _e; }
        }

        public BatchJobExceptionEventArgs(Exception e)
        {
            _e = e;
        }
    }
    

    public enum BatchJobStatus
    {
        Idle,
        Busy,

        //These should be set by the batch before completion
        Warning,
        Error,
        Success,

        CancellationPending,
        Canceled,
        Aborted,
    }


    /// <summary>
    /// Encapsulates batch job functionality with threadpool and progress reporting
    /// Instead of throwing an exception it calls the OnException event.
    /// </summary>
    public abstract class BatchJob : MarshalByRefObject, IProgress
    {
        //remark: implement logging in the derived class

        #region fields

        private string _name;
        private string _group;
        private string _description;

        [Category("Info")]
        public string Name
        {
            get { return _name; }
            protected set { _name = value; }
        }
        [Category("Info")]
        public string Group
        {
            get { return _group; }
            protected set { _group = value; }
        }
        [Category("Info")]
        public string Description
        {
            get { return _description; }
            protected set { _description = value; }
        }


        private object _locker = new object();
        private Thread _workerThread = null;

        // We use AsyncOperation similarly as the background worker does;
        // this means  that events will be called on the same thread as the UI
        // and we do not need to wrap any calls on UI components
        // this is nice-to-have sugar functionality...
        private System.ComponentModel.AsyncOperation _asyncOperation;
        private bool _finishAsyncopertion = false;

        private readonly SendOrPostCallback _operationCompleted;
        private readonly SendOrPostCallback _operationCancelled;
        private readonly SendOrPostCallback _operationAborted;
        private readonly SendOrPostCallback _operationException;
        private readonly SendOrPostCallback _operationProgressUpdate;


        private bool _canCancel = true;
        private bool _isbusy = false;
        private bool _cancellationPending = false;
        private BatchJobStatus _status = BatchJobStatus.Idle;
        
        [Category("Status")]
        public bool CancellationPending
        {
            get { lock (_locker) return _cancellationPending; }
            protected set { lock (_locker) _cancellationPending = value; }
        }
        [Category("Status")]
        public bool CanCancel
        {
            get { lock (_locker) return _canCancel; }
            protected set { _canCancel = value; }
        }
        [Category("Status")]
        public bool IsBusy
        {
            get { lock (_locker) return _isbusy; }
            protected set { _isbusy = value; }
        }
        [Category("Status")]
        public BatchJobStatus Status
        {
            get { lock (_locker) return _status; }
            protected set { lock (_locker) _status = value; }
        }

        #endregion


        protected BatchJob()
        {
            this._operationCompleted = new SendOrPostCallback(this.OperationCompleted);
            this._operationCancelled = new SendOrPostCallback(this.OperationCancelled);
            this._operationProgressUpdate = new SendOrPostCallback(this.OperationProgressUpdate);
            this._operationException = new SendOrPostCallback(this.OperationException);
            this._operationAborted = new SendOrPostCallback(this.OperationAborted);
        }


        #region worker

        protected abstract void RunThis();

        protected virtual void CleanUpWhenAborted()
        {
        }


        public void Cancel()
        {

            if (!CanCancel) throw new InvalidOperationException("Cancelling is not supported for this batch job!");

            if (CanCancel && IsBusy)
            {
                CancellationPending = true;
                Status = BatchJobStatus.CancellationPending;
            }

        }

        private Timer abort_timer = null;

        public void Abort(int max_timeout)
        {
            if (!IsBusy || _workerThread == null)
                return;
            else
            {
                if (CanCancel) Cancel();
                if (max_timeout>0)
                {
                    abort_timer = new Timer(new TimerCallback(
                        (object o) => { abort_timer.Dispose(); if (_workerThread != null) { _workerThread.Abort(); _workerThread.Join(); } }
                        ), null, max_timeout, Timeout.Infinite);

                }
                else
                    if (_workerThread != null)
                    {
                        _workerThread.Abort();
                        _workerThread.Join();
                    }
            }

        }

        public void RunSync()
        {

            if (!this._isbusy)
            {
                try
                {
                    this.IsBusy = true;
                    this.Status = BatchJobStatus.Busy;
                    //Log.Info("Job {0} started syncronously.", Name);
                    RunThis();
                    EmitJobCompleted();
                }
                catch (OperationCanceledException)
                {
                    EmitJobCancelled();
                }
                catch (Exception e)
                {
                    EmitException(e);
                }
            }
            else
                EmitException(new InvalidOperationException("Job is already running!"));     
        }

        public void RunAsync()
        {
            _finishAsyncopertion = true;
            RunAsync(System.ComponentModel.AsyncOperationManager.CreateOperation(null));
        }

        public void RunAsync(System.ComponentModel.AsyncOperation syncContext)
        {
            if (!IsBusy)
            {
                this._asyncOperation = syncContext;
                //we pass the worker thread back to this calling thread so we can call abort on it
                Action action = () => { this._workerThread = Thread.CurrentThread; this.RunThis(); };
                this.IsBusy = true;
                this.Status = BatchJobStatus.Busy;
                action.BeginInvoke(this.RunJobAsyncCallback, action);
            }
            else
                EmitException(new InvalidOperationException("Job is already running!"));     
            
        }

        private void RunJobAsyncCallback(IAsyncResult result)
        {
            Action action = result.AsyncState as Action;
            try
            {
                action.EndInvoke(result);
                EmitJobCompleted();
            }
            catch (OperationCanceledException)
            {
                EmitJobCancelled();
            }
            catch (ThreadAbortException)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((object o) => { CleanUpWhenAborted(); }));
                EmitJobAborted();
            }
            catch (Exception e)
            {
                EmitException(e);
            }
            finally
            {
                _workerThread = null;
            }
        }

        protected void ThrowIfCancelRequested()
        {
            if (CancellationPending) throw new OperationCanceledException();
        }

        #endregion

        #region Events

        private void OperationCompleted(object o)
        {
            IsBusy = false;
            CancellationPending = false;

            //set the status if we forgot to do so in the batch
            BatchJobStatus status = Status;
            if (status != BatchJobStatus.Success && status != BatchJobStatus.Success && status != BatchJobStatus.Error) Status = BatchJobStatus.Success;
            //Log.Debug("Job {0} completed.", Name);
            if (OnJobCompleted != null) OnJobCompleted(this, EventArgs.Empty);
        }
        private void OperationCancelled(object o)
        {
            IsBusy = false;
            CancellationPending = false;
            Status = BatchJobStatus.Canceled;
            //Log.Info("Job {0} cancelled by user.", Name);
            if (OnJobCancelled != null) OnJobCancelled(this, EventArgs.Empty);
        }

        private void OperationAborted(object o)
        {
            IsBusy = false;
            CancellationPending = false;
            Status = BatchJobStatus.Aborted;
            //Log.Info("Job {0} cancelled by user.", Name);
            if (OnJobCancelled != null) OnJobCancelled(this, EventArgs.Empty);
        }

        private void OperationException(object o)
        {
            OnException(this, o as BatchJobExceptionEventArgs);
        }
        private void OperationProgressUpdate(object o)
        {
             OnProgressUpdate(this,o as ProgressUpdateEventArgs);
        }

        protected void EmitJobCompleted()
        {
            if (this._asyncOperation != null)
                if (_finishAsyncopertion)
                    this._asyncOperation.PostOperationCompleted(this._operationCompleted, null);
                else
                    this._asyncOperation.Post(this._operationCompleted, null);
            else
                this._operationCompleted(null);
            _finishAsyncopertion = false;
        }
        protected void EmitJobCancelled()
        {
            if (this._asyncOperation != null)
                if (_finishAsyncopertion)
                    this._asyncOperation.PostOperationCompleted(this._operationCancelled, null);
                else
                    this._asyncOperation.Post(this._operationCancelled, null);
            else
                this._operationCancelled(null);
            _finishAsyncopertion = false;
        }

        protected void EmitJobAborted()
        {
            if (this._asyncOperation != null)
                if (_finishAsyncopertion)
                    this._asyncOperation.PostOperationCompleted(this._operationAborted, null);
                else
                    this._asyncOperation.Post(this._operationAborted, null);
            else
                this._operationAborted(null);
            _finishAsyncopertion = false;
        }

        protected void EmitException(Exception e)
        {
            IsBusy = false;
            CancellationPending = false;
            Status = BatchJobStatus.Error;

            if (OnException != null)
            {
                if (this._asyncOperation != null)
                    if (_finishAsyncopertion)
                        this._asyncOperation.PostOperationCompleted(this._operationException, new BatchJobExceptionEventArgs(e));
                    else
                        this._asyncOperation.Post(this._operationException, new BatchJobExceptionEventArgs(e));
                else
                    this._operationException(new BatchJobExceptionEventArgs(e));
            }
            _finishAsyncopertion = false;
        }
        protected void EmitProgressUpdate(string message, int currentOp, int totalOps)
        {
            if (OnProgressUpdate != null)
            {
                ProgressUpdateEventArgs args = new ProgressUpdateEventArgs(message, currentOp, totalOps);
                if (this._asyncOperation != null)
                    this._asyncOperation.Post(this._operationProgressUpdate, args);
                else
                    this._operationProgressUpdate(args);
            }
        }

       
        public event EventHandler OnJobCompleted;
        public event EventHandler OnJobCancelled;        
        public event EventHandler<BatchJobExceptionEventArgs> OnException;
        public event ProgressUpdateEventHandler OnProgressUpdate;


        #endregion

    }
}
