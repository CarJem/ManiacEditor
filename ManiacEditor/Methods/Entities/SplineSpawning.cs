using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ManiacEditor.Methods.Entities
{
    public static class SplineSpawning
    {
        private static ManiacEditor.Controls.Editor.MainEditor Instance { get; set; }
        public static void UpdateInstance(ManiacEditor.Controls.Editor.MainEditor _Instance)
        {
            Instance = _Instance;
        }

        public static List<float> SplineXPos = new List<float>();
        public static List<float> SplineYPos = new List<float>();


        private static void GatherSplineObjects(int splineID)
        {
            foreach (var entity in ManiacEditor.Classes.Editor.Solution.Entities.InternalEntities.Values)
            {
                if (entity.Name == "Spline")
                {
                    int id = entity.Entity.attributesMap["SplineID"].ValueInt32;
                    if (id == splineID)
                    {
                        SplineXPos.Add(entity.Entity.Position.X.High);
                        SplineYPos.Add(entity.Entity.Position.Y.High);
                    }
                }
            }
        }

        private static void RemoveSplineObjects(int splineID)
        {
            List<Classes.Editor.Scene.Sets.EditorEntity> EntitiesToRemove = new List<Classes.Editor.Scene.Sets.EditorEntity>();
            foreach (var entity in ManiacEditor.Classes.Editor.Solution.Entities.InternalEntities.Values)
            {
                if (entity.Name == "Spline")
                {
                    int id = entity.Entity.attributesMap["SplineID"].ValueInt32;
                    if (id == splineID)
                    {
                        EntitiesToRemove.Add(entity);
                    }
                }
            }

            ManiacEditor.Classes.Editor.Solution.Entities.DeleteInternalEntities(EntitiesToRemove, true);
            Instance.UndoStack.Push(Classes.Editor.Solution.Entities.LastActionInternal);
            Instance.RedoStack.Clear();
        }

        public static void RenderSplineByID(int splineID)
        {
            GatherSplineObjects(splineID);
            RemoveSplineObjects(splineID);

            Classes.Editor.SolutionState.SplineOptions selectedOptions = Classes.Editor.SolutionState.SplineOptionsGroup[splineID];
            if (SplineXPos.Count > 1)
            {
                float[] xs, ys;
                Extensions.Spline.CubicSpline.FitParametric(SplineXPos.ToArray(), SplineYPos.ToArray(), (selectedOptions.SplineSize > 0 ? selectedOptions.SplineSize : 1), out xs, out ys);
                var points = Extensions.Extensions.CreateDataPoints(xs, ys);
                List<KeyValuePair<RSDKv5.SceneObject, RSDKv5.Position>> EntitiesAddList = new List<KeyValuePair<RSDKv5.SceneObject, RSDKv5.Position>>();
                foreach (var p in points)
                {
                    EntitiesAddList.Add(new KeyValuePair<RSDKv5.SceneObject, RSDKv5.Position>(selectedOptions.SplineObjectRenderingTemplate.Entity.Object, new RSDKv5.Position((short)p.X, (short)p.Y)));
                }
                Classes.Editor.Solution.Entities.Add(EntitiesAddList);
                Instance.UndoStack.Push(Classes.Editor.Solution.Entities.LastAction);
                Instance.RedoStack.Clear();
            }

            Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineTotalNumberOfObjects = 0;
            Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineNumberOfObjectsRendered = 0;
            SplineXPos.Clear();
            SplineYPos.Clear();
        }
    }
}
