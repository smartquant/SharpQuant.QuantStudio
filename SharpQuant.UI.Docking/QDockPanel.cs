using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace SharpQuant.UI.Docking
{
    public class QDockPanel : DockPanel
    {

        public DockWindow DocumentDockWindow
        {
            get
            {
                foreach (DockWindow Window in DockWindows)
                    if (Window.DockState == DockState.Document)
                        return Window;
                return null;
            }
        }

        public bool TwoOrMoreOpenDocumentWindows
        {
            get
            {
                if (this.DocumentDockWindow == null)
                    return false;
                return this.DocumentDockWindow.VisibleNestedPanes[0].Contents.Count > 1;
            }
        }

        public bool IsDocumentVisible
        {
            get
            {
                return this.ActiveDocument != null;
            }
        }

        public bool IsAtLeastOnePaneDocked
        {
            get
            {
                foreach (DockWindow Window in DockWindows)
                    if (Window.DockState != DockState.Document
                        && Window.VisibleNestedPanes.Count > 0)
                        return true;
                return false;
            }
        }

        internal void NewHorizontalTabGroup()
        {
            if (this.DocumentDockWindow == null
                || this.ActiveDocument == null)
                return; // just no-op on error

            IDockContent CurrentDocument = this.ActiveDocument;
            DockPane CurrentPane = this.ActiveContent.DockHandler.Pane;

            // create a new dock pane to the right of the current pane
            // and then transfer the document to the new pane
            DockPane NewPane = DockPaneFactory.CreateDockPane(CurrentDocument,
                CurrentPane, DockAlignment.Bottom, 0.5, true);
            NewPane.Show();
        }

        internal void NewVerticalTabGroup()
        {
            if (this.DocumentDockWindow == null
                || this.ActiveDocument == null)
                return; // just no-op on error

            IDockContent CurrentDocument = this.ActiveDocument;
            DockPane CurrentPane = this.ActiveContent.DockHandler.Pane;

            // create a new dock pane to the right of the current pane
            // and then transfer the document to the new pane
            DockPane NewPane = DockPaneFactory.CreateDockPane(CurrentDocument,
                CurrentPane, DockAlignment.Right, 0.5, true);
            NewPane.Show();
        }

    }
}
