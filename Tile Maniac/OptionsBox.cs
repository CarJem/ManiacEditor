using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TileManiac
{
    public partial class OptionsBox : Form
    {
        public OptionsBox()
        {
            InitializeComponent();
            switch (Properties.Settings.Default.ListSetting)
            {
                case 0:
                    collisionListRadioButton.Checked = true;
                    break;
                case 1:
                    tileListRadioButton.Checked = true;
                    break;
            }
            switch (Properties.Settings.Default.ViewAppearanceMode)
            {
                case 0:
                    overlayEditorViewRadioButton.Checked = true;
                    break;
                case 1:
                    collisionEditorViewRadioButton.Checked = true;
                    break;
            }
            switch (Properties.Settings.Default.RenderViewerSetting)
            {
                case 1:
                    tileRenderViewRadioButton.Checked = true;
                    break;
                case 0:
                    collisionRenderViewRadioButton.Checked = true;
                    break;
                case 2:
                    overlayRenderViewRadioButton.Checked = true;
                    break;
            }
        }

        private void ListViewModeChanged(object sender, EventArgs e)
        {
            tileListRadioButton.Checked = false;
            collisionListRadioButton.Checked = false;

            if (sender == tileListRadioButton)
            {
                Properties.Settings.Default.ListSetting = 1;
                tileListRadioButton.Checked = true;
            }
            else if (sender == collisionListRadioButton)
            {
                Properties.Settings.Default.ListSetting = 0;
                collisionListRadioButton.Checked = true;
            }
        }

        private void EditorViewModeChanged(object sender, EventArgs e)
        {
            collisionEditorViewRadioButton.Checked = false;
            overlayEditorViewRadioButton.Checked = false;

            if (sender == collisionEditorViewRadioButton)
            {
                Properties.Settings.Default.ViewAppearanceMode = 1;
                collisionEditorViewRadioButton.Checked = true;
            }
            else if(sender == overlayEditorViewRadioButton)
            {
                Properties.Settings.Default.ViewAppearanceMode = 0;
                tileRenderViewRadioButton.Checked = true;
            }
        }

        private void RenderViewModeChanged(object sender, EventArgs e)
        {
            tileRenderViewRadioButton.Checked = false;
            collisionRenderViewRadioButton.Checked = false;
            overlayRenderViewRadioButton.Checked = false;

            if (sender == tileRenderViewRadioButton)
            {
                Properties.Settings.Default.RenderViewerSetting = 0;
                tileRenderViewRadioButton.Checked = true;
            }
            else if (sender == collisionRenderViewRadioButton)
            {
                Properties.Settings.Default.RenderViewerSetting = 1;
                collisionRenderViewRadioButton.Checked = true;
            }
            else if (sender == overlayRenderViewRadioButton)
            {
                Properties.Settings.Default.RenderViewerSetting = 2;
                overlayRenderViewRadioButton.Checked = true;
            }
        }
    }
}
