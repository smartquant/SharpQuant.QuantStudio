using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using SharpQuant.Batchjobs;

namespace SharpQuant.Batchjobs.UI
{


    class TasklistViewItem :ListViewItem
    {
        private BatchJob _batchjob;

        private const int PROGRESSBAR_COLUMN = 4;


        public BatchJob Batchjob
        {
            get { return _batchjob; }
        }

        public TasklistViewItem(BatchJob batchjob) :base(new string[6])
        {
            _batchjob = batchjob;
            
            UpdateListItem();
            UpdateStatus();
            AttachEvents();
            ResetProgress();

        }

        ~TasklistViewItem()
        {
            DetachEvents();

        }

        #region progress tracking

        //track progress
        private int _currentOp;

        public int CurrentOp
        {
            get { return _currentOp; }
            private set { _currentOp = value; }
        }
        private int _totalOps;

        public int TotalOps
        {
            get { return _totalOps; }
            private set { _totalOps = value; }
        }

        private void ResetProgress()
        {
            _currentOp = 0;
            _totalOps = 100;
        }
        #endregion

        private void AttachEvents()
        {
            _batchjob.OnJobCompleted += new EventHandler(batchjob_OnJobCompleted);
            _batchjob.OnJobCancelled += new EventHandler(batchjob_OnJobCancelled);
            _batchjob.OnProgressUpdate += new ProgressUpdateEventHandler(batchjob_OnProgressTotalUpdate);
            _batchjob.OnException += new EventHandler<BatchJobExceptionEventArgs>(_batchjob_OnException);
        }

        void _batchjob_OnException(object sender, BatchJobExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.ToString());
            UpdateStatus();
            ResetProgress();
        }

        void batchjob_OnProgressTotalUpdate(object sender, ProgressUpdateEventArgs e)
        {
            this.CurrentOp = e.CurrentOp;
            this.TotalOps = e.TotalOps;
            this.SubItems[5].Text = e.Message;
            if (this.ListView!=null)  this.ListView.Invalidate(this.SubItems[PROGRESSBAR_COLUMN].Bounds);
            InvalidateTreeview();
            
        }

        private void InvalidateTreeview()
        {
            var tag = this.Tag as TreeNode;
            if (tag != null)
            {
                System.Drawing.Rectangle rect = tag.Bounds;
                rect.Width = tag.TreeView.Bounds.Width;
                tag.TreeView.Invalidate(rect);
            }
        }

        private void DetachEvents()
        {
            _batchjob.OnJobCompleted -= new EventHandler(batchjob_OnJobCompleted);
            _batchjob.OnJobCancelled -= new EventHandler(batchjob_OnJobCancelled);
            _batchjob.OnProgressUpdate -= new ProgressUpdateEventHandler(batchjob_OnProgressTotalUpdate);
            _batchjob.OnException -= new EventHandler<BatchJobExceptionEventArgs>(_batchjob_OnException);
        }

        void batchjob_OnJobCancelled(object sender, EventArgs e)
        {
            UpdateStatus();
            ResetProgress();
        }

        void batchjob_OnJobCompleted(object sender, EventArgs e)
        {
            UpdateStatus();
            ResetProgress();
        }

        public void UpdateListItem()
        {
            //base.SubItems[0].Text = "!";
            base.SubItems[1].Text = _batchjob.Name;
            base.SubItems[2].Text = _batchjob.Description;
            base.SubItems[4].Text = "!";
            
        }

        public void UpdateStatus()
        {

            InvalidateTreeview();

            base.SubItems[3].Text = _batchjob.Status.ToString();

            switch (_batchjob.Status)
            {
                case BatchJobStatus.Idle:
                    base.ImageIndex = 2;
                    return;
                case BatchJobStatus.Busy:
                    base.ImageIndex = 4;
                    return;
                case BatchJobStatus.Canceled:
                    base.ImageIndex = 5;
                    return;
                case BatchJobStatus.CancellationPending:
                    base.ImageIndex = 5;
                    return;
                case BatchJobStatus.Error:
                    base.ImageIndex = 1;
                    return;
                case BatchJobStatus.Success:
                    base.ImageIndex = 0;
                    return;
                case BatchJobStatus.Warning:
                    base.ImageIndex = 3;
                    return;
            }
            
        }

    }
}
