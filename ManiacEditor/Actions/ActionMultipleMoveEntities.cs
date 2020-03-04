using System.Collections.Generic;
using System.Drawing;

namespace ManiacEditor.Actions
{
    class ActionMultipleMoveEntities : IAction
    {
        Dictionary<Classes.Scene.EditorEntity, Point> initalPos = new Dictionary<Classes.Scene.EditorEntity, Point>();
        Dictionary<Classes.Scene.EditorEntity, Point> postPos = new Dictionary<Classes.Scene.EditorEntity, Point>();
        bool key;

        public string Description => GenerateActionDescription();

        private string GenerateActionDescription()
        {
            return $"Flip Multiple Objects";
        }

        public ActionMultipleMoveEntities(Dictionary<Classes.Scene.EditorEntity, Point> initalPos, Dictionary<Classes.Scene.EditorEntity, Point> postPos, bool key=false)
        {
            this.initalPos = initalPos;
            this.postPos = postPos;
            this.key = key;
        }

        public bool UpdateFromKey(List<Classes.Scene.EditorEntity> entities, Point change)
        {
            return false;
        }

        public void Undo()
        {
            foreach (KeyValuePair<Classes.Scene.EditorEntity, Point> entry in initalPos)
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
