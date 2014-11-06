using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Resources;

using Autofac;

namespace QuantStudio
{
    internal class Globals
    {

        internal static IContainer container;


        #region Singleton pattern
        Globals()
        {
        }

        static Globals Instance
        {
            get
            {
                return Nested.instance;
            }
        }
        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Globals instance = new Globals();
        }

        ~Globals()
        {
            Cleanup();
        }

        #endregion

        public static void InitSplashConfig()
        {
            //IoC container
            var builder = new ContainerBuilder();

            //register modules here
            Autofac.Core.IModule config;

            try
            {
                string configmodule = ConfigurationManager.AppSettings["splashconfig"];
                var configType = Type.GetType(configmodule);
                config = Activator.CreateInstance(configType) as Autofac.Core.IModule;
            }
            catch
            {
                config = new QuantStudio.ExampleConf.ExampleSplashConfig();
            }

            builder.RegisterModule(config);

            container = builder.Build();
        }

        public static void RegisterModule()
        {
            //IoC container
            var builder = new ContainerBuilder();

            //register modules here
            Autofac.Core.IModule config;

            try
            {
                string configmodule = ConfigurationManager.AppSettings["config"];
                var configType = Type.GetType(configmodule);
                config = Activator.CreateInstance(configType) as Autofac.Core.IModule;
            }
            catch
            {
                config = new QuantStudio.ExampleConf.ExampleConfig();
            }

            builder.RegisterModule(config);

            builder.Update(container);

        }

        public static void Cleanup()
        {
            container.Dispose();
            GC.SuppressFinalize(Globals.Instance);
        }
    }
}
