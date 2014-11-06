namespace SharpQuant.UI.ErrorManager
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    public class TraceLevel
    {
        public static readonly System.Diagnostics.TraceLevel Error = System.Diagnostics.TraceLevel.Error;
        public static readonly System.Diagnostics.TraceLevel Info = System.Diagnostics.TraceLevel.Info;
        public static readonly System.Diagnostics.TraceLevel Off = System.Diagnostics.TraceLevel.Off;
        public static readonly System.Diagnostics.TraceLevel Verbose = System.Diagnostics.TraceLevel.Verbose;
        public static readonly System.Diagnostics.TraceLevel Warning = System.Diagnostics.TraceLevel.Warning;

    }

    /// <summary>
    /// Own wrapper around System.Diagnostics.Trace
    /// </summary>
    public class Trace
    {
        private const string _switch = "SharpQuant";
        private static TraceSwitch _traceSwitch = new TraceSwitch("SharpQuant", "");

        static Trace()
        {
            _traceSwitch.Level = System.Diagnostics.TraceLevel.Error;
            //clear default trace listener collection
            System.Diagnostics.Trace.Listeners.Clear();
        }

        public Trace()
        {
        }

        public static bool IsLevelEnabled(System.Diagnostics.TraceLevel level)
        {
            switch (level)
            {
                case System.Diagnostics.TraceLevel.Error:
                    return _traceSwitch.TraceError;

                case System.Diagnostics.TraceLevel.Warning:
                    return _traceSwitch.TraceWarning;

                case System.Diagnostics.TraceLevel.Info:
                    return _traceSwitch.TraceInfo;

                case System.Diagnostics.TraceLevel.Verbose:
                    return _traceSwitch.TraceVerbose;
            }
            return false;
        }

        internal static void SetTraceLevel(System.Diagnostics.TraceLevel level)
        {
            _traceSwitch.Level = level;
        }

        internal static void SetAutoFlush(bool value)
        {
            System.Diagnostics.Trace.AutoFlush = value;
        }

        public static void WriteLine(string message)
        {
            System.Diagnostics.Trace.WriteLine(string.Format("{0} {1}",DateTime.Now, message));
        }

        public static TraceListenerCollection Listeners
        {
            get
            {
                return System.Diagnostics.Trace.Listeners;
            }
        }
    }
}

