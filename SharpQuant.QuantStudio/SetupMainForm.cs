using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;

using WeifenLuo.WinFormsUI.Docking;
using SharpQuant.UI;
using SharpQuant.UI.Docking;
using SharpQuant.UI.Utils;
using SharpQuant.UI.Command;
using SharpQuant.Common;
using Autofac;

namespace QuantStudio
{
    public class SetupMainForm
    {
             
        MainForm _form;
        ResourceManager _resourceManager;
        IContainer _container;



        public SetupMainForm(MainForm form, IContainer container)
        {
            _form = form;
            _resourceManager = container.Resolve<ResourceManager>();
            _container = container;

        }


        public void DoIt()
        {

            //create window menus
            var _windowDefs = GetWindowDefs();

            var windowMaps = new List<WindowMapping>();
            var menuDefs = new List<MenuDef>();
            var toolStripDefs = new List<ToolStripDef>();

            var menuDefMapping = new Dictionary<Type, MenuDef>();
            var tstDefMapping = new Dictionary<Type, ToolStripDef>();


            //create window manager
            WindowManager wm = new WindowManager(_form, _form.dockPanel);
            //register window manager
            var autofacBuilder = new ContainerBuilder();
            autofacBuilder.RegisterInstance(wm).As<IWindowManager>();
            autofacBuilder.RegisterInstance<MainForm>(_form);
            autofacBuilder.Update(_container);


            foreach (var def in _windowDefs)
            {
                var window = CreateWindow(def, _resourceManager, _container);

                //menudef
                var m = new MenuDef()
                {
                    ParentID = 0,
                    MenuType = MenuType.Command,
                    Image = def.ButtonImage,
                    Name = "menu" + def.ButtonName,
                    Text = def.MenuText,
                    SizeH = 200,
                    SizeV = 22,
                    //Command = new DelegateCommand(()=>_wm.ShowWindow(def.Type))

                };
                menuDefs.Add(m);
                menuDefMapping.Add(def.ViewType, m);

                //ToolstripDef
                var tsd = new ToolStripDef()
                {
                    Image=def.ButtonImage,
                    Name="tst" + def.ButtonName,
                    SizeH = 23,
                    SizeV = 22,
                    ToolTip=def.ButtonTooltip,
                    Text = def.ButtonText,
                    //Command = new DelegateCommand(() => _wm.ShowWindow(def.Type))
                };
                toolStripDefs.Add(tsd);
                tstDefMapping.Add(def.ViewType, tsd);

                windowMaps.Add(window);
            }


            //documents
            var docTypeList = new List<Type>();
            var _docDefs = GetDocumentDefs();
            foreach (var def in _docDefs)
            {
                if (def.ViewType == null) 
                    def.ViewType = Type.GetType(string.Format("{0}, {1}", def.ClassName, def.AssemblyName));

                docTypeList.Add(def.ViewType);
            }


            //register the windows
            wm.Init(new AutofacResolver(_container), windowMaps, docTypeList);
            _form.SetWindowmanager(wm);



            //Menus & toolstrip
            foreach (var m in menuDefMapping)
            {
                var type = m.Key;
                m.Value.Command = new DelegateCommand(() => wm.ShowWindow(type));
            }
            foreach (var m in tstDefMapping)
            {
                var type = m.Key;
                m.Value.Command = new DelegateCommand(() => wm.ShowWindow(type));
            }

            //Insert the menus into the main form

            MenuBuilder.InsertSubItems(_form.menuItemView,_resourceManager, menuDefs,0);

            ToolStripBuilder.AppendItems(_form.toolBar, toolStripDefs, _resourceManager);


            RenderMenus();

        }


        public void RenderMenus()
        {

            var fileMenus = _container.ResolveNamed<IEnumerable<IMenuDef>>("filemenu");
            MenuBuilder.InsertMenuItems(_form.menuItemFile, _resourceManager, fileMenus, 0);

            var viewmenus = _container.ResolveNamed<IEnumerable<IMenuDef>>("viewmenu");
            MenuBuilder.InsertMenuItems(_form.menuItemFile, _resourceManager, viewmenus, 0);

            var toolmenus = _container.ResolveNamed<IEnumerable<IMenuDef>>("toolmenu");
            MenuBuilder.InsertMenuItems(_form.menuItemFile, _resourceManager, toolmenus, 0);

            var windowmenus = _container.ResolveNamed<IEnumerable<IMenuDef>>("windowmenu");
            MenuBuilder.InsertMenuItems(_form.menuItemFile, _resourceManager, windowmenus, 0);

            var editmenus = _container.ResolveNamed<IEnumerable<IMenuDef>>("editmenu");
            MenuBuilder.InsertMenuItems(_form.mainMenu, _resourceManager, toolmenus, 1);

            var othermenus = _container.ResolveNamed<IEnumerable<IMenuDef>>("othermenu");
            MenuBuilder.AppendMenuItems(_form.mainMenu, _resourceManager, othermenus);

            var helpmenus = _container.ResolveNamed<IEnumerable<IMenuDef>>("helpmenu");
            MenuBuilder.AppendMenuItems(_form.mainMenu, _resourceManager, helpmenus); 


        }


        static WindowMapping CreateWindow(IWindowDef def, ResourceManager resourceManager, IContainer container)
        {
            if (def.ViewType == null)
            {
                def.ViewType = Type.GetType(string.Format("{0}, {1}", def.ClassName, def.AssemblyName));
            }
            QDockingWindow window = null;


            if (def.ViewType.InheritsOrImplements(typeof(QDockingWindow)))
            {
                window = container.Resolve(def.ViewType) as QDockingWindow;
                if (window == null) throw new ArgumentException("Could not resolve the QDockingWindow!");

            }
            else if (def.ViewType.InheritsOrImplements(typeof(UserControl)))
            {
                var control = container.Resolve(def.ViewType) as UserControl;
                if (control == null) throw new ArgumentException("Could not resolve the UserControl!");

                window = new QViewForm(control);
            }
            else
                throw new ArgumentException("Type must be either of QDockingWindow or UserControl!");


            window.Name = def.TabText;
            window.Text = def.TabText;
            window.TabText = def.TabText;

            //IMPORTANT: Windows do not close!
            window.HideOnClose = true;
            window.DockAreas = (DockAreas)def.DockAreas;

            if (!string.IsNullOrEmpty(def.Icon))
                window.Icon = ((System.Drawing.Icon)(resourceManager.GetObject(def.Icon)));

            return new WindowMapping() { Window = window, Type = def.ViewType };

        }




        IEnumerable<IWindowDef> GetWindowDefs()
        {

            //should be from DB
            return _container.ResolveNamed<IEnumerable<IWindowDef>>("windows");
        }

        IEnumerable<IWindowDef> GetDocumentDefs()
        {
            return _container.ResolveNamed<IEnumerable<IWindowDef>>("docs");
        }

    }
}
