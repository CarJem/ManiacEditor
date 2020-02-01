using System.Collections.Generic;
using System.Drawing;

namespace ManiacEditor.Actions
{
    class ActionMultipleMoveEntities : IAction
    {
        Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point> initalPos = new Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point>();
        Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point> postPos = new Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point>();
        bool key;

        public string Description => GenerateActionDescription();

        private string GenerateActionDescription()
        {
            return $"Flip Multiple Objects";
        }

        public ActionMultipleMoveEntities(Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point> initalPos, Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point> postPos, bool key=false)
        {
            this.initalPos = initalPos;
            this.postPos = postPos;
            this.key = key;
        }

        public bool UpdateFromKey(List<Classes.Editor.Scene.Sets.EditorEntity> entities, Point change)
        {
            return false;
        }

        public void Undo()
        {
            foreach (KeyValuePair<Classes.Editor.Scene.Sets.EditorEntity, Point> entry in initalPos)
            {
                entry.Key.Move(entry.Value, false);
            }
        }

        public IAction Redo()
        {
            // Don't pass key, because we don't want to merge with it after 
            return new ActionMultipleMoveEntities(initalPos, postPos);
        }
    }
}
