using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ManiacEditor.Properties;
using Microsoft.Xna.Framework;

namespace ManiacEditor
{
    /// <summary>
    /// A form for managing Editor Layers of a Sonic Mania Scene at a high level.
    /// </summary>
    public partial class LayerManager : Form
    {
        private EditorScene _editorScene;
        private IEnumerable<EditorLayer> Layers
        {
            get => _editorScene?.AllLayers;
        }

        private bool _layerArrangementChanged = false;
        bool initilzing = true;


        //Previous Values for Specific Varriables
        int nudStartLineTemp = 0;
        int nudLineCountTemp = 0;

        //Clipboards
        private EditorLayer LayerClipboard;

        private BindingSource _bsHorizontal;
        private BindingSource _bsHorizontalMap;

        // I clearly have no understanding of WinForms Data Binding
        public LayerManager(EditorScene editorScene)
        {
            InitializeComponent();
            if (Settings.mySettings.NightMode)
            {
                rtbWarn.Rtf = Resources.LayerManagerWarningDarkTheme;
            }
            else
            {
                rtbWarn.Rtf = Resources.LayerManagerWarning;
            }

            _editorScene = editorScene;
            bsLayers.DataSource = Layers;
            lbLayers.DisplayMember = "Name";

            lblRawWidthValue.DataBindings.Add(CreateTextBinding("Width", FormatBasicNumber));
            lblRawHeightValue.DataBindings.Add(CreateTextBinding("Height", FormatBasicNumber));

            lblEffSizeWidth.DataBindings.Add(CreateTextBinding("Width", FormatEffectiveNumber));
            lblEffSizeHeight.DataBindings.Add(CreateTextBinding("Height", FormatEffectiveNumber));

            nudWidth.DataBindings.Add(new Binding("Value", bsLayers, "Width"));
            nudHeight.DataBindings.Add(new Binding("Value", bsLayers, "Height"));

            tbName.DataBindings.Add(CreateBinding("Text", bsLayers, "Name"));

            nudVerticalScroll.DataBindings.Add(CreateBinding("Value", bsLayers, "ScrollingVertical"));
            nudUnknownByte2.DataBindings.Add(CreateBinding("Value", bsLayers, "UnknownByte2"));
            nudUnknownWord1.DataBindings.Add(CreateBinding("Value", bsLayers, "UnknownWord1"));
            nudUnknownWord2.DataBindings.Add(CreateBinding("Value", bsLayers, "UnknownWord2"));

            _bsHorizontal = new BindingSource(bsLayers, "HorizontalLayerScroll");
            lbHorizontalRules.DataSource = _bsHorizontal;
            lbHorizontalRules.DisplayMember = "Id";

            nudHorizontalEffect.DataBindings.Add(CreateBinding("Value", _bsHorizontal, "UnknownByte1"));
            nudHorizByte2.DataBindings.Add(CreateBinding("Value", _bsHorizontal, "UnknownByte2"));
            nudHorizVal1.DataBindings.Add(CreateBinding("Value", _bsHorizontal, "UnknownWord1"));
            nudHorizVal2.DataBindings.Add(CreateBinding("Value", _bsHorizontal, "UnknownWord2"));

            _bsHorizontalMap = new BindingSource(_bsHorizontal, "LinesMapList");
            lbMappings.DataSource = _bsHorizontalMap;
            nudStartLine.DataBindings.Add(CreateBinding("Value", _bsHorizontalMap, "StartIndex"));
            nudLineCount.DataBindings.Add(CreateBinding("Value", _bsHorizontalMap, "LineCount"));
        }

        private Binding CreateTextBinding(string property, ConvertEventHandler formatHandler)
        {
            var b = new Binding("Text", bsLayers, property, true, DataSourceUpdateMode.OnPropertyChanged, "unknown", property + ": {0:N0}");
            b.Format += formatHandler;
            return b;
        }

        private Binding CreateBinding(string targetControlProperty, BindingSource bindingSource, string sourceDataProperty)
        {
            return new Binding(targetControlProperty, bindingSource, sourceDataProperty, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void FormatBasicNumber(object sender, ConvertEventArgs e)
        {
            e.Value = string.Format(((Binding)sender).FormatString, Convert.ToInt32(e.Value));
        }

        private void FormatEffectiveNumber(object sender, ConvertEventArgs e)
        {
            e.Value = string.Format(((Binding)sender).FormatString, Convert.ToInt32(e.Value) * 16);
        }

        private void DesiredSizeChanged(object sender, EventArgs e)
        {
            lblResizedEffective.Text = $"Effective Width {(nudWidth.Value * 16):N0}, " +
                                       $"Effective Height {(nudHeight.Value * 16):N0}";
        }

        private void btnResize_Click(object sender, EventArgs e)
        {
            var check = MessageBox.Show(@"Resizing a layer can not be undone. 
You really should save what you have and take a backup first.
Proceed with the resize?",
                                        "Caution!",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning,
                                        MessageBoxDefaultButton.Button2);
            if (check == DialogResult.Yes)
            {
                var layer = lbLayers.SelectedItem as EditorLayer;
                layer.Resize(Convert.ToUInt16(nudWidth.Value), Convert.ToUInt16(nudHeight.Value));
            }

            bsLayers.ResetCurrentItem();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            var current = bsLayers.Current as EditorLayer;
            if (current == null) return;

            int index = bsLayers.Position;
            if (index == 0) return;
            bsLayers.Remove(current);
            bsLayers.Insert(--index, current);
            bsLayers.Position = index;

            _layerArrangementChanged = true;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var current = bsLayers.Current as EditorLayer;
            if (current == null) return;

            int index = bsLayers.Position;
            if (index == bsLayers.Count - 1) return;
            bsLayers.Remove(current);
            bsLayers.Insert(++index, current);
            bsLayers.Position = index;

            _layerArrangementChanged = true;

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            EditorLayer newEditorLayer = _editorScene.ProduceLayer();
            int newIndex = bsLayers.Add(newEditorLayer);
            bsLayers.Position = newIndex;

            _layerArrangementChanged = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var current = bsLayers.Current as EditorLayer;
            if (null == current) return;

            var check = MessageBox.Show($@"Deleting a layer can not be undone!
Are you sure you want to delete the [{current.Name}] layer?",
                                        "Confirm Deletion",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning,
                                        MessageBoxDefaultButton.Button2);
            if (check == DialogResult.Yes)
            {
                bsLayers.Remove(current);

                _layerArrangementChanged = true;
            }
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            var current = bsLayers.Current as EditorLayer;
            if (null == current) return;
            CopyLayerToClipboard(current);
            bsLayers.Remove(current);
            _layerArrangementChanged = true;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var current = bsLayers.Current as EditorLayer;
            if (null == current) return;
            CopyLayerToClipboard(current);
        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            var current = bsLayers.Current as EditorLayer;
            if (null == current) return;
            bsLayers.Insert(bsLayers.IndexOf(current), current);
            _layerArrangementChanged = true;
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            PasteLayerFromClipboard();
        }

        private void LayerManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!_layerArrangementChanged) return;

            MessageBox.Show(@"If you have changed the number, or order of the layers, 
you may need to update any layer switching entities/objects in the scene too.

If you don't, strange things may well happen.
They may well happen anway, this is all experimental!",
                            "Don't forget!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }


        private void btnAddHorizontalRule_Click(object sender, EventArgs e)
        {
            // create the horizontal rule set
            var layer = bsLayers.Current as EditorLayer;
            layer.ProduceHorizontalLayerScroll();

            // make sure our view of the underlying set of rules is refreshed
            _bsHorizontal.CurrencyManager.Refresh();

            // and select the one we just made
            _bsHorizontal.Position = _bsHorizontal.Count - 1;
        }


        private void btnRemoveHorizontalRule_Click(object sender, EventArgs e)
        {
            if (_bsHorizontal.Count == 1)
            {
                MessageBox.Show("There must be at least one set of horizontal scrolling rules.",
                                "Delete not allowed.",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            var current = _bsHorizontal.Current as HorizontalLayerScroll;
            if (null == current) return;

            var check = MessageBox.Show($@"Deleting a set of horizontal scrolling rules can not be undone!
Are you sure you want to delete the set of rules with id '{current.Id}'?
All mappings for this rule will be deleted too!",
                                        "Confirm Deletion",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning,
                                        MessageBoxDefaultButton.Button2);

            if (check == DialogResult.Yes)
            {
                _bsHorizontal.Remove(current);
            }
        }


        private void btnAddHorizontalMapping_Click(object sender, EventArgs e)
        {
            var hls = _bsHorizontal.Current as HorizontalLayerScroll;
            if (null == hls) return;

            hls.AddMapping();
            _bsHorizontalMap.CurrencyManager.Refresh();
        }


        private void btnRemoveHorizontalMapping_Click(object sender, EventArgs e)
        {
            var current = _bsHorizontalMap.Current as ScrollInfoLines;
            if (null == current) return;

            var check = MessageBox.Show($@"Deleting a set of horizontal scrolling rule mappings can not be undone!
Are you sure you want to delete this mapping?",
                                        "Confirm Deletion",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning,
                                        MessageBoxDefaultButton.Button2);

            if (check == DialogResult.Yes)
            {
                _bsHorizontalMap.Remove(current);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            // clean up our bindings
            if (_bsHorizontal != null) _bsHorizontal.Dispose();
            if (_bsHorizontalMap != null) _bsHorizontal.Dispose();

            base.Dispose(disposing);
        }

        private void nudStartLine_ValueChanged(object sender, EventArgs e)
        {
            if (!initilzing)
            {
                if (nudStartLine.Value + nudLineCount.Value > (nudHeight.Value * 16))
                {
                    overflowMessage.Text = $@"The Start Line Value plus the Line Count Value must not Exceed the Maximum Layer Height! (" + nudStartLine.Value + "+" + nudLineCount.Value + " (" + (nudStartLine.Value + nudLineCount.Value) + ") " + "> " + (nudHeight.Value * 16) + ") You won't be able to save!";
                    overflowMessage.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    overflowMessage.Text = "Make sure the Start Line Value plus the Line Count Value does not Exceed the Maximum Layer Height! Otherwise, you will be unable to save!";
                    overflowMessage.ForeColor = SystemColors.WindowText;
                }
            }

        }

        private void LayerManager_Load(object sender, EventArgs e)
        {
            if (nudStartLine.Value + nudLineCount.Value > (nudHeight.Value * 16))
            {
                nudStartLineTemp = (int)nudStartLine.Value;
                nudLineCountTemp = (int)nudLineCount.Value;
            }
            initilzing = false;
        }

        private void gbHorizontalMappings_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void lbLayers_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void lbLayers_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (lbLayers.SelectedItem != null)
                {
                    contextMenuStrip1.Show(lbLayers, e.X, e.Y);
                }

            }
        }

        private void CopyLayerToClipboard(EditorLayer layerToCopy)
        {
            EditorLayer copyData = layerToCopy;

            // Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
            if (Settings.mySettings.EnableWindowsClipboard)
                Clipboard.SetDataObject(new DataObject("ManiacLayer", copyData), true);

            // Also copy to Maniac's clipboard in case it gets overwritten elsewhere
            LayerClipboard = copyData;

        }

        public void PasteLayerFromClipboard()
        {
            // check if there is a layer on the Windows clipboard; if so, use those

            // For Some reason this isn't working, please check this out campbell. (And no, I put in false to prevent it from running, that's not the problem)
            if (Settings.mySettings.EnableWindowsClipboard && Clipboard.ContainsData("ManiacLayer") && false)
            {
                var layerToPaste = (EditorLayer)Clipboard.GetDataObject().GetData("ManiacLayer");

                bsLayers.Insert(bsLayers.Count - 1, layerToPaste);

                _layerArrangementChanged = true;
            }
            

            // if there's none, use the internal clipboard
            else if (LayerClipboard != null)
            {

                var layerToPaste = LayerClipboard;
                bsLayers.Insert(bsLayers.Count - 1, layerToPaste);
                _layerArrangementChanged = true;

            }
        }
    }
}
