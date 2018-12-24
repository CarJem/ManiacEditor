using Microsoft.Scripting.Utils;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ManiacEditor.Actions
{
    class ActionMultipleMoveEntities : IAction
    {
        Dictionary<EditorEntity, Point> initalPos = new Dictionary<EditorEntity, Point>();
        Dictionary<EditorEntity, Point> postPos = new Dictionary<EditorEntity, Point>();
        bool key;

        public string Description => GenerateActionDescription();

        private string GenerateActionDescription()
        {
            return $"Flip Multiple Objects";
        }

        public ActionMultipleMoveEntities(Dictionary<EditorEntity, Point> initalPos, Dictionary<EditorEntity, Point> postPos, bool key=false)
        {
            this.initalPos = initalPos;
            this.postPos = postPos;
            this.key = key;
        }

        public bool UpdateFromKey(List<EditorEntity> entities, Point change)
        {
            return false;
        }

        public void Undo()
        {
            foreach (KeyValuePair<EditorEntity, Point> entry in initalPos)
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
