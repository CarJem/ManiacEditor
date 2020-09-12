using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RSDKv5;
using ManiacEditor.Actions;
using SharpDX.Direct3D9;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Scene = RSDKv5.Scene;
using ManiacEditor.Classes.Scene;
using ManiacEditor.Classes.Rendering;
using SFML.System;
using SFML.Graphics;
using ManiacEditor.Methods.Drawing;

namespace ManiacEditor.Classes.Scene
{
    public class EditorScene : RSDKv5.Scene, IDisposable
    {
        #region Definitions

        public ManiacEditor.Controls.Editor.MainEditor Instance;
        public EditorEntities Entities { get; set; }

        #endregion

        #region Layers

        public EditorLayer LowDetails
        {
            get => AllLayers.FirstOrDefault(el => el.Name.Equals(Classes.Prefrences.SceneCurrentSettings.ManiacINIData.ForegroundLower) || el.Name.Equals(ManiacEditor.Properties.Settings.MyDefaults.CustomFGLower) || el.Name.Equals("FG Lower") || el.Name.Equals("FG Supa Low"));
        }
        public EditorLayer ForegroundLow
        {
            get => AllLayers.LastOrDefault(el => el.Name.Equals("FG Low") || el.Name.Equals("Playfield"));
        }
        public EditorLayer Scratch
        {
            get => AllLayers.LastOrDefault(el => el.Name.Equals("Scratch"));
        }
        public EditorLayer Move
        {
            get => AllLayers.LastOrDefault(el => el.Name.Equals("Move"));
        }
        public EditorLayer HighDetails
        {
            get => AllLayers.FirstOrDefault(el => el.Name.Equals(Classes.Prefrences.SceneCurrentSettings.ManiacINIData.ForegroundHigher) || el.Name.Equals(ManiacEditor.Properties.Settings.MyDefaults.CustomFGHigher) || el.Name.Equals("FG Higher") || el.Name.Equals("FG Overlay") || el.Name.Equals("FG Supa High"));
        }
        public EditorLayer ForegroundHigh
        {
            get => AllLayers.LastOrDefault(el => el.Name.Equals("FG High") || el.Name.Equals("Ring Count"));
        }
        #endregion

        #region List of Layers
        public IList<EditorLayer> AllLayers { get; set; }
        public IList<EditorLayer> AllLayersList
        {
            get { return AllLayers; }
        }
        public IEnumerable<EditorLayer> OtherLayers
        {
            get
            {
                return AllLayers.Where(el => el != ForegroundLow && el != ForegroundHigh && el != HighDetails && el != LowDetails);
            }
        }
        public IEnumerable<EditorLayer> LayerByDrawingOrder
        {
            get
            {
                return AllLayers.Where(el => el.Layer.DrawingOrder.Equals(1));
            }
        }
        #endregion

        #region Misc
        public EditorScene(string filename, DevicePanel d, ManiacEditor.Controls.Editor.MainEditor instance) : base(filename)
        {
            Instance = instance;
            AllLayers = new List<EditorLayer>(Layers.Count);
            foreach (SceneLayer layer in Layers)
            {
                AllLayers.Add(new EditorLayer(layer, Instance));
            }
        }

        public EditorScene(DevicePanel d, int width, int height, int BGWidth, int BGHeight, ManiacEditor.Controls.Editor.MainEditor instance)
        {
            Instance = instance;
            Layers = new List<SceneLayer>(3);
            Layers.Add(new SceneLayer("FG Low", (ushort)width, (ushort)height));
            Layers.Add(new SceneLayer("FG High", (ushort)width, (ushort)height));
            Layers.Add(new SceneLayer("Background", (ushort)BGWidth, (ushort)BGHeight));

            AllLayers = new List<EditorLayer>(Layers.Count);
            foreach (SceneLayer layer in Layers)
            {
                AllLayers.Add(new EditorLayer(layer, Instance));
            }
        }

        public void Reload()
        {
            if (!disposedValue)
            {
                // dispose managed state (managed objects).
                foreach (var el in AllLayers)
                {
                    el.DisposeTextures();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below if we need to.
                // then set large fields to null.
                disposedValue = true;
            }
        }

        public EditorLayer ProduceLayer()
        {
            // lets just pick some reasonably safe defaults
            var sceneLayer = new SceneLayer("New Layer", 128, 128);
            var editorLayer = new EditorLayer(sceneLayer, Instance);
            return editorLayer;
        }

        public void DeleteLayer(int byIndex)
        {
            AllLayers.RemoveAt(byIndex);
        }

        public void DeleteLayer(EditorLayer thisLayer)
        {
            AllLayers.Remove(thisLayer);
        }

        public String[] GetEncorePalette(string SelectedZone, string DataDirectory, string SelectedScene, string Result, int searchType, string userLoad = "")
        {
            if (!System.IO.Directory.Exists(DataDirectory)) return new string[6];
            string EncorePallete1 = ""; //Base Pallete
            string EncorePallete2 = "";
            string EncorePallete3 = "";
            string EncorePallete4 = "";
            string EncorePallete5 = "";
            string EncorePallete6 = "";
            //Encore Palette File Loading
            string ZoneName = SelectedZone.Replace("\\", "");
            if (ZoneName == "GHZ")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreGHZ.act");
                EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreGHZ2.act");
            }
            else if (ZoneName == "CPZ")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreCPZ.act");
                EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreCPZw.act");
            }
            else if (ZoneName == "SPZ1")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreSPZ1.act");
                EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreSPZ1b1.act");
                EncorePallete3 = Path.Combine(DataDirectory, "Palettes", "EncoreSPZ1b2.act");
                EncorePallete4 = Path.Combine(DataDirectory, "Palettes", "EncoreSPZ1b3.act");
            }
            else if (ZoneName == "SPZ2")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreSPZ2.act");
            }
            else if (ZoneName == "FBZ")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreFBZ.act");
                EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreFBZf.act");
                EncorePallete3 = Path.Combine(DataDirectory, "Palettes", "EncoreFBZi.act");
                EncorePallete4 = Path.Combine(DataDirectory, "Palettes", "EncoreFBZs.act");
            }
            else if (ZoneName == "PSZ1")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncorePSZ1.act");
            }
            else if (ZoneName == "PSZ2")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncorePSZ2.act");
            }
            else if (ZoneName == "SSZ1")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreSSZ1.act");
            }
            else if (ZoneName == "SSZ2")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreSSZ2.act");
            }
            else if (ZoneName == "HCZ")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreHCZ.act");
                EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreHCZw.act");
            }
            else if (ZoneName == "MSZ")
            {
                if (SelectedScene.Contains("1"))
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreMSZ1.act");
                }
                if (SelectedScene.Contains("2"))
                {
                    EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreMSZ2.act");
                }
            }
            else if (ZoneName == "OOZ1")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreOOZ1.act");
            }
            else if (ZoneName == "OOZ2")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreOOZ2.act");
                EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreOOZSmog.act");
            }
            else if (ZoneName == "LRZ1")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreLRZ1.act");
            }
            else if (ZoneName == "LRZ2")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreLRZ2.act");
            }
            else if (ZoneName == "LRZ3")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreLRZ3.act");
            }
            else if (ZoneName == "MMZ")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreMMZ.act");
                EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreMMZf.act");
                EncorePallete3 = Path.Combine(DataDirectory, "Palettes", "EncoreMMZfp.act");
            }
            else if (ZoneName == "TMZ1")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreTMZ1.act");
                EncorePallete2 = Path.Combine(DataDirectory, "Palettes", "EncoreTMZ1d.act");
                EncorePallete3 = Path.Combine(DataDirectory, "Palettes", "EncoreTMZ1l.act");
            }
            else if (ZoneName == "TMZ2")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreTMZ2.act");
            }
            else if (ZoneName == "TMZ3")
            {
                EncorePallete1 = Path.Combine(DataDirectory, "Palettes", "EncoreTMZ3.act");
            }
            else
            {
                if (searchType == -1)
                {
                    if (File.Exists(userLoad))
                    {
                        EncorePallete1 = userLoad;
                    }
                }
                else
                {
                    // For Custom Encore Palette Finding
                    String CustomEncore;
                    CustomEncore = locateEncorePalettes(ZoneName, SelectedScene, Result, DataDirectory, SelectedScene, SelectedScene);

                    if (File.Exists(CustomEncore))
                    {
                        EncorePallete1 = CustomEncore;
                    }
                }

            }
            string[] encorePalletes = new string[6] { EncorePallete1, EncorePallete2, EncorePallete3, EncorePallete4, EncorePallete5, EncorePallete6 };
            return encorePalletes;
        }

        public int GetEncoreSetupType(string SelectedZone, string DataDirectory, string SelectedScene, string Result)
        {
            //Encore Palette File Loading
            string ZoneName = SelectedZone.Replace("\\", "");
            int encoreType = 0;
            /* Encore Types
             * 0: Singular Palette
             * 1: Singular with Objects (GHZ)
             * 2: Singular with Water (HCZ/CPZ)
             * <3: Zone Specific;
             */

            switch (ZoneName)
            {
                case "GHZ":
                    encoreType = 1;
                    break;
                case "CPZ":
                    encoreType = 2;
                    break;
                case "SPZ1":
                    encoreType = 3;
                    break;
                case "SPZ2":
                    encoreType = 0;
                    break;
                case "FBZ":
                    encoreType = 4;
                    break;
                case "PSZ1":
                    encoreType = 0;
                    break;
                case "PSZ2":
                    encoreType = 0;
                    break;
                case "SSZ1":
                    encoreType = 0;
                    break;
                case "SSZ2":
                    encoreType = 0;
                    break;
                case "HCZ":
                    encoreType = 2;
                    break;
                case "MSZ1":
                    encoreType = 0;
                    break;
                case "OOZ1":
                    encoreType = 0;
                    break;
                case "OOZ2":
                    encoreType = 5;
                    break;
                case "LRZ1":
                    encoreType = 0;
                    break;
                case "LRZ2":
                    encoreType = 0;
                    break;
                case "LRZ3":
                    encoreType = 0;
                    break;
                case "MMZ":
                    encoreType = 6;
                    break;
                case "TMZ1":
                    encoreType = 7;
                    break;
                case "TMZ2":
                    encoreType = 0;
                    break;
                case "TMZ3":
                    encoreType = 0;
                    break;
                default:
                    encoreType = 0;
                    break;
            }
            return encoreType;
        }

        private string locateEncorePalettes(string Zone, string Scene, string FullPath, string DataDirectory, string SelectedZone, string SelectedScene)
        {
            string palettesFolder = Path.Combine(DataDirectory, "Palettes");

            string modifiedZone;
            string modifiedScene;
            string actFile;
            string newPal;

            //First Check (intended for MSZ1e && MSZ1k)
            modifiedZone = Zone.Replace("e", "");
            modifiedZone = modifiedZone.Replace("k", "");
            modifiedScene = Scene.Replace("Scene", "");
            modifiedScene = modifiedScene.Replace(".bin", "");
            modifiedScene = modifiedScene.Replace("1e", "1");
            modifiedScene = modifiedScene.Replace("1k", "1");
            actFile = "Encore" + modifiedZone + modifiedScene + ".act";
            newPal = Path.Combine(DataDirectory, "Palettes", actFile);

            //Debug.Print(newPal);
            if (File.Exists(newPal))
            {
                //Debug.Print("First Check Passed");
                return newPal;
            }
            else
            {
                //Second Check (intended for external data folders)
                actFile = "Encore" + Zone + ".act";
                newPal = Path.Combine(DataDirectory, "Palettes", actFile);
                //Debug.Print(newPal);
                if (File.Exists(newPal))
                {
                    //Debug.Print("Second Check Passed");
                    return newPal;
                }
                else
                {
                    //Third Check (intended for MSZ1e && MSZ1k (in mods))
                    modifiedZone = Zone.Replace("e", "");
                    modifiedZone = modifiedZone.Replace("k", "");
                    modifiedScene = Scene.Replace("Scene", "");
                    modifiedScene = modifiedScene.Replace(".bin", "");
                    modifiedScene = modifiedScene.Replace("1e", "1");
                    modifiedScene = modifiedScene.Replace("1k", "1");
                    actFile = "Encore" + modifiedZone + modifiedScene + ".act";
                    newPal = Path.Combine(DataDirectory, "Palettes", actFile);

                    //Debug.Print(newPal);
                    if (File.Exists(newPal))
                    {
                        //Debug.Print("Third Check Passed");
                        return newPal;
                    }

                    //Debug.Print("Did not find a Work Around");
                    return "";
                }


            }


        }

        public void Save(string filename)
        {
            // save any changes made to the scrolling horizontal rules
            foreach (var el in AllLayers)
            {
                el.WriteHorizontalLineRules();
            }
            Layers = AllLayers.Select(el => el.Layer).ToList();
            Objects.Clear();
            Objects = Entities.Save();
            Write(filename);
        }
        #endregion

        #region Rendering
        public void Draw(DevicePanel d)
        {
            LayerDrawing.UpdateLayerVisibility();
            foreach (var layer in AllLayers) LayerDrawing.ApplyLayerRules(layer);

            foreach (var layer in AllLayers)
            {
                if (layer.Visible) layer.Draw(d);
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    foreach (var el in AllLayers)
                    {
                        el.Dispose();
                        el.DisposeTextures();
                    }
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below if we need to.
                // then set large fields to null.

                disposedValue = true;
            }
        }

        // Override this finalizer only if Dispose(bool disposing) ever gains code to free unmanaged resources.
        // ~EditorScene() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            //GC.SuppressFinalize(this);
        }
        #endregion

    }
}
