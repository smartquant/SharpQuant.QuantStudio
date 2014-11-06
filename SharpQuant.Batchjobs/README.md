SharpQuant.Batchjobs
==========================

Batchjob base class which supports simple notification 

- Encapsulates batch job functionality with thread pool and progress reporting
- Instead of throwing an exception it calls the OnException event.
- Design and threading classes were originally created for .net 2.0 framework
- Embedded treeview batch job UI for QuantStudio with progress reporting
- The property editor in QuantStudio can be used to change properties
- Treeview supports running the job with right-click

To create a batch job:


``` csharp

    public class TestJob : BatchJob
    {

        public TestJob()
        {
            base.CanCancel = true;
            base.Name = "Test job";
            base.Description = "Test description";
            base.Group = "Test";

        }


        protected override void DoTheWork()
        {

            //Simulate some heavy work with cancelation awareness
            for (int i=0;i<10;i++)
            {
                base.ThrowIfCancelRequested();
                //...
                //throw new ArgumentException("Error simulated!");
                try
                {
                    
                    Console.WriteLine(string.Format("Loop {0}", i+1));
                    
                    Thread.Sleep(1000);

                    base.EmitProgressUpdate(string.Format("Loop {0}", i+1), i + 1, 10);
                }
                finally {/* any required cleanup */ }
            }
        }
		
		protected override void CleanUpWhenAborted()
        {
            //do some cleanup here
        }
    }

```

It is up to the user to inject dependencies into batchjob classes from an IoC framwork.