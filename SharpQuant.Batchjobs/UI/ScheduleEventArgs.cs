using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.Batchjobs.UI
{
    public class ScheduleEventArgs:EventArgs
    {
        public BatchJob Batchjob { get; private set; }

        public ScheduleEventArgs(BatchJob job)
        {
            Batchjob = job;
        }
    }
}
