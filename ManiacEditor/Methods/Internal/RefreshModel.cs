using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor.Classes.Scene;

namespace ManiacEditor.Methods.Internal
{
    public static class RefreshModel
    {
        private static int LastViewPositionX { get; set; } = -1;
        private static int LastViewPositionY { get; set; } = -1;

        public static void RequestObjectPropertyRefresh(bool force = false)
        {
            if (Methods.Editor.Solution.Entities != null)
            {
                if (Methods.Editor.SolutionState.ViewPositionX != LastViewPositionX || Methods.Editor.SolutionState.ViewPositionX != LastViewPositionY)
                {
                    LastViewPositionX = Methods.Editor.SolutionState.ViewPositionX;
                    LastViewPositionY = Methods.Editor.SolutionState.ViewPositionY;
                    Classes.Scene.EditorEntities.ObjectRefreshNeeded = true;
                }
                else if (force)
                {
                    Classes.Scene.EditorEntities.ObjectRefreshNeeded = true;
                }
            }
        }
    }
}
