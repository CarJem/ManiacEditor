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
            foreach (var entity in ManiacEditor.Methods.Solution.CurrentSolution.Entities.InternalEntities)
            {
                if (entity.Name == "Spline")
                {
                    int id = entity.attributesMap["SplineID"].ValueInt32;
                    if (id == splineID)
                    {
                        SplineXPos.Add(entity.Position.X.High);
                        SplineYPos.Add(entity.Position.Y.High);
                    }
                }
            }
        }

        private static void RemoveSplineObjects(int splineID)
        {
            List<Classes.Scene.EditorEntity> EntitiesToRemove = new List<Classes.Scene.EditorEntity>();
            foreach (var entity in ManiacEditor.Methods.Solution.CurrentSolution.Entities.InternalEntities)
            {
                if (entity.Name == "Spline")
                {
                    int id = entity.attributesMap["SplineID"].ValueInt32;
                    if (id == splineID)
                    {
                        EntitiesToRemove.Add(entity);
                    }
                }
            }

            ManiacEditor.Methods.Solution.CurrentSolution.Entities.DeleteEntities(EntitiesToRemove, true, true);
            Actions.UndoRedoModel.UndoStack.Push(Methods.Solution.CurrentSolution.Entities.LastActionInternal);
            Actions.UndoRedoModel.RedoStack.Clear();
        }

        public static void RenderSplineByID(int splineID)
        {
            GatherSplineObjects(splineID);
            RemoveSplineObjects(splineID);

            Methods.Solution.SolutionState.SplineOptions selectedOptions = Methods.Solution.SolutionState.SplineOptionsGroup[splineID];
            if (SplineXPos.Count > 1)
            {
                float[] xs, ys;
                Extensions.Spline.CubicSpline.FitParametric(SplineXPos.ToArray(), SplineYPos.ToArray(), (selectedOptions.SplineSize > 0 ? selectedOptions.SplineSize : 1), out xs, out ys);
                var points = Extensions.Extensions.CreateDataPoints(xs, ys);
                List<KeyValuePair<RSDKv5.SceneObject, RSDKv5.Position>> EntitiesAddList = new List<KeyValuePair<RSDKv5.SceneObject, RSDKv5.Position>>();
                foreach (var p in points)
                {
                    EntitiesAddList.Add(new KeyValuePair<RSDKv5.SceneObject, RSDKv5.Position>(selectedOptions.SplineObjectRenderingTemplate.Object, new RSDKv5.Position((short)p.X, (short)p.Y)));
                }
                Methods.Solution.CurrentSolution.Entities.SpawnMultiple(EntitiesAddList);
                Actions.UndoRedoModel.UndoStack.Push(Methods.Solution.CurrentSolution.Entities.LastAction);
                Actions.UndoRedoModel.RedoStack.Clear();
            }

            Methods.Solution.SolutionState.SplineOptionsGroup[splineID].SplineTotalNumberOfObjects = 0;
            Methods.Solution.SolutionState.SplineOptionsGroup[splineID].SplineNumberOfObjectsRendered = 0;
            SplineXPos.Clear();
            SplineYPos.Clear();
        }
    }
}
