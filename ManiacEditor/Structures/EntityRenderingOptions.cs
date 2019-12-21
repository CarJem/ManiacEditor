using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Windows.Forms;
using RSDKv5;
using Newtonsoft.Json;

namespace ManiacEditor.Structures
{
    public class EntityRenderingOptions
    {
        public List<string> ObjectToRender { get; set; }
        public List<string> ObjectCullingExclusions { get; set; }

        public List<string> SpecialObjectRenders { get; set; } 
        public List<string> LinkedObjectsToRender { get; set; } 

        public EntityRenderingOptions()
        {
            ObjectToRender = new List<string>();
            ObjectCullingExclusions = new List<string>();
        }

        #region Reading And Writing
        public static void GetInternalData(ref EntityRenderingOptions self)
        {
            string data;

            var resourceName = "ManiacEditor.Resources.rendering_options.json";
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    data = reader.ReadToEnd();
                }
            }
            EntityRenderingOptions Options = JsonConvert.DeserializeObject<EntityRenderingOptions>(data);
            self = Options;
        }

        public static void Save(EntityRenderingOptions self)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Resources\\rendering_options.json");
            string data = JsonConvert.SerializeObject(self, Formatting.Indented);
            File.WriteAllText(path, data);

        }

        public static void GetExternalData(ref EntityRenderingOptions self)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Resources\\rendering_options.json");
            string data = File.ReadAllText(path);
            EntityRenderingOptions Options = JsonConvert.DeserializeObject<EntityRenderingOptions>(data);
            self = Options;

        }
        #endregion
    }
}
