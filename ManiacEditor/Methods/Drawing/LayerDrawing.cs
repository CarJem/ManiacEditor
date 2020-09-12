using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RSDKv5;
using ManiacEditor.Actions;
using System.Drawing;
using ManiacEditor.Classes.Rendering;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Scene = RSDKv5.Scene;
using EditorLayer = ManiacEditor.Classes.Scene.EditorLayer;
using ManiacEditor.Enums;
using ManiacEditor.Extensions;
using SFML.System;
using SFML.Graphics;
using ManiacEditor.Controls.Editor;
using EditorScene = ManiacEditor.Classes.Scene.EditorScene;
using ManiacEditor.Methods.Solution;

namespace ManiacEditor.Methods.Drawing
{

    public static class LayerDrawing
    {


        private static MainEditor Instance { get; set; }

        public static void UpdateInstance(MainEditor Editor)
        {
            Instance = Editor;
        }


        #region Layer Visibility Checks

        public static void ApplyLayerRules(EditorLayer layer)
        {
            if (!AreWeAnEditLayer() && Methods.Solution.SolutionState.Main.IsTilesEdit())
                layer.RenderingTransparency = 0x52;
            else if (Instance.EditorToolbar.EditEntities.IsCheckedAll && Methods.Solution.SolutionState.Main.ApplyEditEntitiesTransparency)
                layer.RenderingTransparency = 0x32;
            else
                layer.RenderingTransparency = 0xFF;

            bool AreWeAnEditLayer()
            {
                bool isEditA = layer == Methods.Solution.CurrentSolution.EditLayerA;
                bool isEditB = layer == Methods.Solution.CurrentSolution.EditLayerB;
                bool isEditC = layer == Methods.Solution.CurrentSolution.EditLayerC;
                bool isEditD = layer == Methods.Solution.CurrentSolution.EditLayerD;

                return (isEditA || isEditB || isEditC || isEditD);
            }
        }
        public static void UpdateLayerVisibility()
        {
            UpdateLayer(Instance.EditorToolbar.ShowFGLower.IsChecked.Value, Instance.EditorToolbar.EditFGLower.IsCheckedAll, Methods.Solution.CurrentSolution.FGLower);
            UpdateLayer(Instance.EditorToolbar.ShowFGLow.IsChecked.Value, Instance.EditorToolbar.EditFGLow.IsCheckedAll, Methods.Solution.CurrentSolution.FGLow);
            UpdateLayer(Instance.EditorToolbar.ShowFGHigh.IsChecked.Value, Instance.EditorToolbar.EditFGHigh.IsCheckedAll, Methods.Solution.CurrentSolution.FGHigh);
            UpdateLayer(Instance.EditorToolbar.ShowFGHigher.IsChecked.Value, Instance.EditorToolbar.EditFGHigher.IsCheckedAll, Methods.Solution.CurrentSolution.FGHigher);

            for (int i = 0; i < Instance.EditorToolbar.ExtraLayerEditViewButtons.Count; i++)
            {
                var elb = Instance.EditorToolbar.ExtraLayerEditViewButtons.ElementAt(i);

                int index = Instance.EditorToolbar.ExtraLayerEditViewButtons.IndexOf(elb);
                var _extraViewLayer = Methods.Solution.CurrentSolution.CurrentScene.OtherLayers.ElementAt(index);

                if (elb.Value.IsCheckedAll || elb.Key.IsCheckedAll) _extraViewLayer.Visible = true;
                else _extraViewLayer.Visible = false;
            }

            void UpdateLayer(bool ShowLayer, bool EditLayer, Classes.Scene.EditorLayer layer)
            {
                if (layer != null)
                {
                    if (ShowLayer || EditLayer) layer.Visible = true;
                    else layer.Visible = false;
                }
            }
        }

        #endregion
    }
}
