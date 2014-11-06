using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.Batchjobs
{

    public delegate void ProgressUpdateEventHandler(object sender, ProgressUpdateEventArgs e);


    public class ProgressUpdateEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public int CurrentOp { get; private set; }
        public int TotalOps { get; private set; }

        public ProgressUpdateEventArgs(string message, int currentOp, int totalOps)
        {
            Message = message;
            CurrentOp = (currentOp < 0 || currentOp > totalOps) ? 0 : currentOp;
            TotalOps = (totalOps < 0 || currentOp > totalOps) ? CurrentOp + 1 : totalOps;
        }
    }


    public interface IProgress
    {
        event ProgressUpdateEventHandler OnProgressUpdate;
    }
}
