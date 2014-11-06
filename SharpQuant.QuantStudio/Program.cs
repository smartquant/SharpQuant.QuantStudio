using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Configuration;

using System.Threading;

using SharpQuant.Common;
using SharpQuant.UI.ErrorManager;

using QuantStudio.Startup;
using Autofac;

namespace QuantStudio
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Platform.IsUnix)
            if (!SingleInstance.Start())
            {
                SingleInstance.ShowFirstInstance();
                return;
            }


            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LoadAssemblies);

             //re-direct exceptions to runtime error manager
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //threadpool optimization
            ThreadPool.SetMinThreads(5, 5);

            ProgramInit.InitContainer();
            var splashScreen = Globals.container.Resolve<ISplashScreen>();
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => { Application.Run(splashScreen as Form); }));

            Thread.Sleep(100);

            splashScreen.SetProgressText("Initializing Settings...");
            ProgramInit.RegisterModule();

            

            //Need to do this again...
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            

            //Setup mainform
            var main = new MainForm();
            var setup = new SetupMainForm(main, Globals.container);
            setup.DoIt();


            splashScreen.CloseThis();


            Application.Run(main);

            ProgramInit.Cleanup();

            if (!Platform.IsUnix)
            SingleInstance.Stop();

        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            RuntimeErrorManager.ReportError(new RuntimeError(RuntimeErrorLevel.Medium, (Exception)e.ExceptionObject, "Application"));
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            RuntimeErrorManager.ReportError(new RuntimeError(RuntimeErrorLevel.Medium, e.Exception, "Application"));
        }


        static Assembly LoadAssemblies(object sender, ResolveEventArgs args)
        {

            string dllpath =  ConfigurationManager.AppSettings["dllpath"];

            string simpleName = new AssemblyName(args.Name).Name;

            string path = dllpath + simpleName + ".dll";

            if (System.IO.File.Exists(path)) return Assembly.LoadFrom(path);
            return null;

        }

    }
}