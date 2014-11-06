using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Resources;
using System.Windows.Forms;

using Autofac;

using SharpQuant.Common;
using SharpQuant.Common.Validation;

using SharpQuant.UI;
using SharpQuant.UI.Command;
using SharpQuant.UI.Console;
using SharpQuant.UI.Docking;
using SharpQuant.UI.PropertyGrid;
using SharpQuant.UI.UserMessage;
using SharpQuant.UI.MenuTreeView;
using SharpQuant.UI.Utils;


namespace QuantStudio.ExampleConf
{
    class ExampleSplashConfig : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Startup.SplashScreen>().OnActivated(e => e.Instance.lblCopyright.Text = "Copyright 2014, Joachim Loebb").As<Startup.ISplashScreen>();
        }
    }

    class ExampleConfig : Module
    {


        protected override void Load(ContainerBuilder builder)
        {
            //we register an abstraction of the container
            builder.RegisterType(typeof(AutofacResolver)).As<IResolver>();
            //we need this for the property editor
            builder.RegisterType(typeof(AutofacValidationProvider)).As<IValidationProvider>();
            //Icons for the menus
            builder.RegisterInstance(SharpQuant.UI.Resources.ResourceManager).As<ResourceManager>();

            //define some windows
            builder.RegisterInstance<IEnumerable<IWindowDef>>(GetWindowDefs())
                .Named<IEnumerable<IWindowDef>>("windows");

            //define the help menu with the about window
            builder.RegisterType<AboutDialog>().OnActivated(e=>e.Instance.lblCopyright.Text="Copyright 2014, Joachim Loebb");
            builder.Register<IEnumerable<IMenuDef>>(c => GetHelpMenus(c.Resolve<IResolver>()))
                .Named<IEnumerable<IMenuDef>>("helpmenu");

            LoadUI(builder);

        }

        private void LoadUI(ContainerBuilder builder)
        {
            //IUserMessageService
            builder.Register(c => new UserMessageService()).As<IUserMessageService>().SingleInstance();

            //register console
            builder.Register(c => new ConsoleControl()).SingleInstance();

            //PropertyEditor
            builder.Register(c =>
            {
                var control = new PropertyWindow();
                control.Init(c.Resolve<IValidationProvider>(), c.Resolve<IUserMessageService>());
                return control;
            }).AsSelf().As<IPropertyEditableObjectEditor>().SingleInstance();

            //Tree view launcher control
            builder.Register<MenuTreeViewControl>(c =>
            {
                var control = new MenuTreeViewControl();
                control.Init(GetTree(), GetContextMenus(c), c.Resolve<ResourceManager>());
                return control;
            });
        }

        List<IWindowDef> GetWindowDefs()
        {

            //should be from DB

            var list = new List<IWindowDef>();
            list.Add(new WindowDef()
            {
                ClassName = "SharpQuant.UI.Console.ConsoleControl",
                AssemblyName = "SharpQuant.UI",
                TabText = "Console",
                ButtonTooltip = "Open Console",
                ButtonImage = "application_xp_terminal",
                ButtonName = "btnConsole",
                ButtonText = "",
                MenuText = "Console Window",
                DockAreas = 63,
                Icon = "DefaultIcon"
            });
            list.Add(new WindowDef()
            {
                ClassName = "SharpQuant.UI.PropertyGrid.PropertyWindow",
                AssemblyName = "SharpQuant.UI",
                TabText = "PropertyEditor",
                ButtonTooltip = "Property Editor",
                ButtonImage = "overlays",
                ButtonName = "btnProperty",
                ButtonText = "",
                MenuText = "Property Editor",
                DockAreas = 63,
                Icon = "DefaultIcon"
            });

            list.Add(new WindowDef()
            {
                ClassName = "SharpQuant.UI.MenuTreeView.MenuTreeViewControl",
                AssemblyName = "SharpQuant.UI",
                TabText = "Launcher",
                ButtonTooltip = "Launcher",
                ButtonImage = "page_copy",
                ButtonName = "btnLauncher",
                ButtonText = "",
                MenuText = "Launcher",
                DockAreas = 63,
                Icon = "DefaultIcon"
            });

            return list;
        }
       
        IEnumerable<TreeMenuDef> GetTree()
        {
            var list = new List<TreeMenuDef>()
            {
                new TreeMenuDef()
                {
                    //This is a root group node
                    ID=1,
                    ParentID=0,
                    IsLeaf=false,
                    Name = "group1",
                    Description = "Test group node",
                
                },
                new TreeMenuDef()
                {
                    //This is an example menu entry
                    ID = 2,
                    ParentID = 1,
                    IsLeaf = true,
                    Name = "Write to Console",
                    Description = "This will write the settings to the console",
                    Settings = "Hallo console!",
                    Menus = "console" 
                },
                new TreeMenuDef()
                {
                    //This is an example menu entry
                    ID = 3,
                    ParentID = 1,
                    IsLeaf = true,
                    Name = "Edit object",
                    Description = "This will let you edit a test object",
                    Menus = "propertyWindow" 
                }
            };
            return list;
        }

        IEnumerable<MenuDef> GetContextMenus(IComponentContext c)
        {
            var list = new List<MenuDef>()
            {
                new MenuDef()
                {
                    Command = new SharpQuant.UI.Command.DelegateCommand(()=>
                        {
                            Console.WriteLine("Hallo Console!");
                        }),
                    Image = "application_xp_terminal",
                    MenuType = MenuType.Command,
                    Name = "console", //this is the menuID
                    Text = "Write to Console"
                },
                new MenuDef()
                {
                    Command = new PropertyEditorCommand(c.Resolve<IPropertyEditableObjectEditor>()),
                    Image = "cog",
                    MenuType = MenuType.Command,
                    Name = "propertyWindow", //this is the menuID
                    Text = "EditObject"
                },
            };

            return list;
        }

        List<IMenuDef> GetHelpMenus(IResolver container)
        {
            var helpMenus = new List<IMenuDef>();

            helpMenus.Add(new MenuDef()
            {
                ID = 1,
                ParentID = 0,
                MenuType = MenuType.Menu,
                Name = "mnuHelp",
                Text = "&Help",
                SizeH = 200,
                SizeV = 22,
            });

            helpMenus.Add(new MenuDef()
            {
                ID = 2,
                ParentID = 1,
                MenuType = MenuType.Command,
                Name = "mnuAbout",
                Text = "&About",
                SizeH = 200,
                SizeV = 22,
                Command = new ShowAboutDialog(container),
            });

            return helpMenus;
        }

    }

    class TestObject
    {
        [Category("Category 1"), Description("Description 1")]
        public string Field1 { get; set; }
        [Category("Category 2"), Description("Description 2")]
        public string Field2 { get; set; }
    }

    class PropertyEditorCommand : ICommand
    {
        IPropertyEditableObjectEditor _editor;
        public PropertyEditorCommand(IPropertyEditableObjectEditor editor)
        {
            _editor = editor;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _editor.EditObject<TestObject>(new PropertyEditableObject<TestObject>(
                new TestObject(){ Field1="Value1",Field2="Value2"}));
        }
    }

    public class ShowAboutDialog : ICommand
    {

        IResolver _containter;

        public ShowAboutDialog(IResolver container)
        {
            _containter = container;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            AboutDialog aboutDialog = _containter.Resolve<AboutDialog>();
            aboutDialog.ShowDialog(_containter.Resolve<MainForm>());
        }
    }

}
