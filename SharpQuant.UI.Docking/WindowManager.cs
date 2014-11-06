using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;

using WeifenLuo.WinFormsUI.Docking;

using SharpQuant.Common;
using SharpQuant.UI;

namespace SharpQuant.UI.Docking
{

    public interface IWindowManager
    {

        void ShowWindow(Type key);
        void CloseWindow(Type key);

        /// <summary>
        /// Selects a document if already present, creates it if it does not exist
        /// </summary>
        /// <param name="doctype"></param>
        /// <param name="key"></param>
        /// <param name="dock"></param>
        void OpenDocument(Type doctype, string key, bool dock = true);
        void CloseDocument(Type doctype, string key);


        /// <summary>
        /// Show a user control without docking
        /// </summary>
        /// <param name="control"></param>
        void ShowUserControl(UserControl control);

    }

    public class WindowManager : IWindowManager
    {

        private DeserializeDockContent _deserializeDockContent;

        private Form _mainform;
        private DockPanel _panel;

        private Dictionary<Type, QDockingWindow> _windows;
        private Dictionary<Type, DocumentsDictionary> _documents;
        //we need this mapping for generic types, e.g UserControl<T>
        private Dictionary<string, Type> _typenames;

        private IResolver _container;

        public Dictionary<Type, QDockingWindow> Windows
        {
            get { return _windows; }
            set { _windows = value; }
        }

        public Dictionary<Type, DocumentsDictionary> Documents
        {
            get { return _documents; }
            set { _documents = value; }
        }

        public T GetWindow<T>() where T : QDockingWindow
        {
            return (T)GetWindow(typeof(T));
        }

        public QDockingWindow GetWindow(Type t)
        {
            QDockingWindow window = null;
            Windows.TryGetValue(t, out window);
            return window;
        }

        public T GetOrCreateDocument<T>(string key) where T : Form
        {
            return (T)GetOrCreateDocument(typeof(T),key);
        }


        /// <summary>
        /// Gets or creates a new document window
        /// </summary>
        /// <param name="type">Can be either a QDockWindow or a UserControl </param>
        /// <param name="key">Key should be the serialized settingslist </param>
        /// <returns></returns>
        public Form GetOrCreateDocument(Type type, string key)
        {
            DocumentsDictionary docs = _documents[type];
            if (docs.ContainsKey(key))
                return docs[key];
            else //create new
            {
                SettingsList settings = SettingsList.Deserialize(key);
                var obj = _container.Resolve(type);
                var qWindow = obj as QDockingWindow;
                if (qWindow != null)
                {
                    qWindow.FormClosed += (s, o) => docs.Remove(key);
                    docs.Add(key, qWindow);
                    qWindow.Settings = settings;
                    return qWindow;
                }

                var control = obj as UserControl;
                if (control != null)
                {
                    qWindow = new QViewForm(control);
                    qWindow.FormClosed += (s, o) => docs.Remove(key);
                    docs.Add(key, qWindow);
                    qWindow.Settings = settings;
                    return qWindow;

                }
                return null;
            }
        }


       
        public Form Mainform
        {
            get { return _mainform; }
        }

        public DockPanel DockPanel
        {
            get { return _panel; }
        }

        public WindowManager(Form mainform, QDockPanel dockpanel)
        {
            _mainform = mainform;
            _panel = dockpanel;


            _windows = new Dictionary<Type, QDockingWindow>();
            _documents = new Dictionary<Type, DocumentsDictionary>();
            _typenames = new Dictionary<string, Type>();


            _deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

        }

        public void Init(IResolver container, IEnumerable<WindowMapping> windows, IEnumerable<Type> documents)
        {
            _container = container;

            //create all windows
            foreach (var win in windows)
            {
                _windows.Add(win.Type, win.Window);
                _typenames.Add(win.Type.ToString(), win.Type);
            }

            //create all documents
            foreach (var doctype in documents)
            {
                _documents.Add(doctype, new DocumentsDictionary());
                _typenames.Add(doctype.ToString(), doctype);
            }
        }


        public Form Run()
        {
            return _mainform;
        }


        #region utils for dockpanel

        
        public void CloseAllButThisOne()
        {
            if (_panel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                Form activeMdi = _mainform.ActiveMdiChild;
                foreach (Form form in _mainform.MdiChildren)
                {
                    if (form != activeMdi)
                        form.Close();
                }
            }
            else
            {
                foreach (IDockContent document in _panel.DocumentsToArray())
                {
                    if (!document.DockHandler.IsActivated)
                        document.DockHandler.Close();
                }
            }
        }

        public void CloseAllDocuments()
        {
            if (_panel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in _mainform.MdiChildren)
                    form.Close();
            }
            else
            {
                for (int index = _panel.Contents.Count - 1; index >= 0; index--)
                {
                    if (_panel.Contents[index] is IDockContent)
                    {
                        IDockContent content = (IDockContent)_panel.Contents[index];
                        content.DockHandler.Close();
                    }
                }
            }
        }

        public void CloseActiveDocument()
        {
            if (_panel.DocumentStyle == DocumentStyle.SystemMdi)
                _mainform.ActiveMdiChild.Close();
            else if (_panel.ActiveDocument != null)
                _panel.ActiveDocument.DockHandler.Close();
        }

        public void CloseAllContents()
        {
            // we don't want to create another instance of tool window, set DockPanel to null
            foreach (QDockingWindow window in _windows.Values)
            {
                window.DockPanel = null;
            }

            // Close all other document windows
            CloseAllDocuments();
        }

        #endregion




        #region persistense

        public IDockContent GetContentFromPersistString(string persistString)
        {
            string[] items = persistString.Split('$');
            if (items.Length != 2) return null;

            string typeName = items[0];

            Type type = null;
            if (!_typenames.TryGetValue(typeName, out type))
            {
                try
                {
                    type = Type.GetType(typeName);
                }
                catch { }
            }
            if (type == null) return null;
            string doc_key = items[1];
            SettingsList settings = SettingsList.Deserialize(doc_key);

            if (_documents.ContainsKey(type))
            {
                return GetOrCreateDocument(type,doc_key) as QDockingWindow;
            }
            else
            {
                var window = _windows[type] as QDockingWindow;
                window.Settings = settings;

                return window;
            }

            
        }

        public void LoadLayoutFromXML(string configFile)
        {
            if (File.Exists(configFile))
                _panel.LoadFromXml(configFile, _deserializeDockContent);
        }

        public void SaveLayoutToXML(string configFile)
        {
            _panel.SaveAsXml(configFile);
        }

        #endregion


        #region IWindowManager

        public void ShowWindow(Type key)
        {
            var window = GetWindow(key) as QDockingWindow;
            window.Show(this.DockPanel);
        }

        public void CloseWindow(Type key)
        {
            GetWindow(key).Close();
        }

        public void OpenDocument(Type doctype, string key, bool dock = true)
        {
            var window = GetOrCreateDocument(doctype, key) as QDockingWindow;
            if (dock)
                window.Show(this.DockPanel);
            else
                window.Show();
        }

        public void CloseDocument(Type doctype, string key)
        {
            DocumentsDictionary docs = _documents[doctype];
            QDockingWindow doc = null;
            if (docs.TryGetValue(key,out doc))
            {
                doc.Close();
            }
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public void ShowUserControl(UserControl control)
        {
            var window = new QViewForm(control);
            window.Show();
        }

        #endregion
    }
}
