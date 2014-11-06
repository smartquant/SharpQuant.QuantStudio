using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Collections;


namespace SharpQuant.UI.ErrorManager
{

    public enum RuntimeErrorOutputTarget
    {
        Console,
        PopupWindow,
        LogFileOnly
    }

    public enum RuntimeErrorLevel
    {       
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    /// <summary>
    /// Wrapper class for exceptions
    /// </summary>
    public class RuntimeError
    {
        private System.DateTime _dateTime;
        private object _source;
        private RuntimeErrorLevel _level;
        private string _description;
        private string _details;

        public RuntimeError(RuntimeErrorLevel level, Exception e)
            : this(level, e, null)
        {
        }

        public RuntimeError(RuntimeErrorLevel level, Exception e, object source)
            : this(level, e.Message, e.ToString(), source)
        {
        }

        public RuntimeError(RuntimeErrorLevel level, string description, string details, object source)
            : this(System.DateTime.Now, level, description, details, source)
        {
        }

        public RuntimeError(System.DateTime datetime, RuntimeErrorLevel level, string description, string details, object source)
        {
            this._dateTime = datetime;
            this._level = level;
            this._description = description;
            this._details = details;
            this._source = source;
        }

        public override string ToString()
        {
            string str = (this._source == null) ? "Not defined." : this._source.ToString();
            return ("DateTime: " + this._dateTime.ToShortDateString() + " " + this._dateTime.ToLongTimeString() + Environment.NewLine + "Level: " + this._level.ToString() + Environment.NewLine + "Source: " + str + Environment.NewLine + "Description: " + this._description + Environment.NewLine + "Details: " + this._details);
        }

        public System.DateTime DateTime
        {
            get
            {
                return this._dateTime;
            }
        }

        public string Description
        {
            get
            {
                return this._description;
            }
        }

        public string Details
        {
            get
            {
                return this._details;
            }
        }

        public RuntimeErrorLevel Level
        {
            get
            {
                return this._level;
            }
        }

        public object Source
        {
            get
            {
                return this._source;
            }
        }
    }

    public delegate void RuntimeErrorEventHandler(RuntimeErrorEventArgs args);

    public class RuntimeErrorCollection : ReadOnlyCollectionBase
    {
        internal RuntimeErrorCollection()
        {
        }

        internal void AddError(RuntimeError _error)
        {
            base.InnerList.Add(_error);
        }
    }

    public class RuntimeErrorEventArgs : EventArgs
    {
        private RuntimeError _error;

        public RuntimeErrorEventArgs(RuntimeError error)
        {
            this._error = error;
        }

        public RuntimeError Error
        {
            get
            {
                return this._error;
            }
        }
    }



    /// <summary>
    /// Error Manager to redirect runtime errors into logging, dialogbox, etc.
    /// </summary>
    public class RuntimeErrorManager
    {
        private static bool _isenabled = false;
        private static RuntimeErrorCollection _errorCollection = new RuntimeErrorCollection();
        private static RuntimeErrorOutputTarget _outputTarget;

        private static RuntimeErrorEventHandler _runtimeErrorEventHandler;

        public static event RuntimeErrorEventHandler Error
        {
            add { _runtimeErrorEventHandler += value; }
            remove { value -= _runtimeErrorEventHandler; }
        }

        static RuntimeErrorManager()
        {
            Enabled = true;
            _outputTarget = RuntimeErrorOutputTarget.PopupWindow;
        }

        public RuntimeErrorManager()
        {
        }

        public static void ReportError(RuntimeError error)
        {
            lock (typeof(RuntimeErrorManager))
            {
                MethodInvoker invoker2;
                _errorCollection.AddError(error);
                if (Trace.IsLevelEnabled(TraceLevel.Error))
                {
                    Trace.WriteLine(error.ToString());
                }
                if (Environment.UserInteractive)
                {
                    switch (_outputTarget)
                    {
                        case RuntimeErrorOutputTarget.Console:
                            System.Console.WriteLine(error.ToString());
                            break;

                        case RuntimeErrorOutputTarget.PopupWindow:
                            {
                                invoker2 = null;
                                RuntimeErrorForm form = new RuntimeErrorForm();
                                form.ShowError(error);
                                Control app = Control.FromHandle(Process.GetCurrentProcess().MainWindowHandle);
                                if ((app != null) && app.InvokeRequired)
                                {
                                    if (invoker2 == null)
                                    {
                                        invoker2 = () => form.ShowDialog(app);
                                    }
                                    MethodInvoker method = invoker2;
                                    app.Invoke(method);
                                }
                                else
                                {
                                    form.ShowDialog(app);
                                }
                                break;
                            }
                    }
                }
                //Emit error to other listeners
                if (_runtimeErrorEventHandler != null)
                {
                    _runtimeErrorEventHandler(new RuntimeErrorEventArgs(error));
                }
            }
        }



        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exceptionObject = e.ExceptionObject as Exception;
            ReportError(new RuntimeError(RuntimeErrorLevel.Critical, exceptionObject));
        }

        public static bool Enabled
        {
            get
            {
                return _isenabled;
            }
            set
            {
                if (value)
                {
                    if (!_isenabled)
                    {
                        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(RuntimeErrorManager.CurrentDomain_UnhandledException);
                        _isenabled = true;
                    }
                }
                else if (_isenabled)
                {
                    AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(RuntimeErrorManager.CurrentDomain_UnhandledException);
                    _isenabled = false;
                }
            }
        }

        public static RuntimeErrorCollection Errors
        {
            get
            {
                return _errorCollection;
            }
        }

        public static RuntimeErrorOutputTarget Target
        {
            get
            {
                return _outputTarget;
            }
            set
            {
                _outputTarget = value;
            }
        }
    }
}

