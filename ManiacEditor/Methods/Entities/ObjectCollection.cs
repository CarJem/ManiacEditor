using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSDKv5;
using System.IO;
using Newtonsoft.Json;

namespace ManiacEditor.Methods.Entities
{
    public static class ObjectCollection
    {
        private static bool RemoveObjectImportLock
        {
            get
            {
                return Properties.Settings.MySettings.RemoveObjectImportLock;
            }
        }
        public static MultiZoneObjectSearchPair GetObjectsFromDataFolder(string DataFolder, GameConfig SourceConfig, IList<SceneObject> TargetSceneObjects)
        {
            var targetNames = TargetSceneObjects.Select(tso => tso.Name.ToString());
            List<SceneObject> _sourceSceneObjects = new List<SceneObject>();

            List<ImportableZoneObject> MegaList = new List<ImportableZoneObject>();
            List<SceneObject> GlobalObjects = new List<SceneObject>();

            foreach (var category in SourceConfig.Categories)
            {
                foreach (var scene in category.Scenes)
                {
                    string sceneLocation = scene.GetFilePath(DataFolder);
                    if (File.Exists(sceneLocation))
                    {
                        Scene sourceScene = new Scene(sceneLocation);
                        foreach (var sceneObj in sourceScene.Objects.Distinct())
                        {
                            if (!MegaList.Exists(x => x.Zone == scene.Zone))
                            {
                                ImportableZoneObject newZone = new ImportableZoneObject(scene.Zone);
                                MegaList.Add(newZone);
                            }
                            var zoneEntry = MegaList.Where(x => x.Zone == scene.Zone).First();
                            if (!zoneEntry.Objects.Exists(x => x.Name.Name == sceneObj.Name.Name))
                            {
                                if (!SourceConfig.ObjectsNames.Contains(sceneObj.Name.Name))
                                {
                                    zoneEntry.Objects.Add(sceneObj);
                                    if (!_sourceSceneObjects.Contains(sceneObj)) _sourceSceneObjects.Add(sceneObj);
                                }
                                else
                                {
                                    if (!GlobalObjects.Exists(x => x.Name.Name == sceneObj.Name.Name))
                                    {
                                        GlobalObjects.Add(sceneObj);
                                    }
                                }

                            }

                        }
                    }
                }
            }

            MegaList.ForEach(x => x.Objects = x.Objects.Where(sso => !GlobalObjects.Contains(sso)).ToList());
            MegaList.Insert(0, new ImportableZoneObject("Global", GlobalObjects));

            foreach (var item in GlobalObjects) _sourceSceneObjects.Insert(0, item);

            if (!RemoveObjectImportLock) MegaList.ForEach(x => x.Objects = x.Objects.Where(sso => !targetNames.Contains(sso.Name.Name)).ToList());

            return new MultiZoneObjectSearchPair(targetNames.ToList(), MegaList.ToList());
        }
        public static ObjectSearchPair GetObjectsFromScene(IList<SceneObject> SourceSceneObjects, IList<SceneObject> TargetSceneObjects)
        {
            var targetNames = TargetSceneObjects.Select(tso => tso.Name.ToString());
            var importableObjects = SourceSceneObjects.Where(sso => !targetNames.Contains(sso.Name.ToString())).OrderBy(sso => sso.Name.ToString());

            if (RemoveObjectImportLock) importableObjects = SourceSceneObjects.OrderBy(sso => sso.Name.ToString());
            else importableObjects = SourceSceneObjects.Where(sso => !targetNames.Contains(sso.Name.ToString())).OrderBy(sso => sso.Name.ToString());

            return new ObjectSearchPair(targetNames.ToList(), importableObjects.ToList());
        }
        public static void ExportObjectsFromDataFolderToFile(string FilePath, string DataFolder, GameConfig SourceConfig, IList<SceneObject> TargetSceneObjects)
        {
            MultiZoneObjectExportPair MegaList = new MultiZoneObjectExportPair(GetObjectsFromDataFolder(DataFolder, SourceConfig, TargetSceneObjects));
            string json = JsonConvert.SerializeObject(MegaList.SourceObjects, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }
        public static void ExportObjectsFromSceneToFile(string FilePath, IList<SceneObject> SourceSceneObjects, IList<SceneObject> TargetSceneObjects)
        {
            ObjectExportPair ImportList = new ObjectExportPair(GetObjectsFromScene(SourceSceneObjects, TargetSceneObjects));
            string json = JsonConvert.SerializeObject(ImportList.SourceObjects, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }
    }
}
