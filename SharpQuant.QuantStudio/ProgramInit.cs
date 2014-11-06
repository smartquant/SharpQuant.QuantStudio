using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantStudio
{
    class ProgramInit
    {
        public static void InitContainer()
        {
            Globals.InitSplashConfig();
        }

        public static void RegisterModule()
        {
            Globals.RegisterModule();
        }

        public static void Cleanup()
        {
            Globals.Cleanup();
        }
    }
}
