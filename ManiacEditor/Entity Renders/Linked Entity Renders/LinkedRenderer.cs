using System;
using RSDKv5;
using Color = System.Drawing.Color;



namespace ManiacEditor.Entity_Renders
{
    public abstract class LinkedRenderer
    {
        public Interfaces.Base.MapEditor EditorInstance;

        public virtual void Draw(GraphicsHandler d, RSDKv5.SceneEntity currentEntity, Classes.Editor.Scene.Sets.EditorEntity ObjectInstance)
        {
            
        }

        public abstract string GetObjectName();
        public void DrawLinkArrow(GraphicsHandler d, RSDKv5.SceneEntity start, RSDKv5.SceneEntity end)
        {
            if (SetFilter(end) == true) return;
            int startX = start.Position.X.High;
            int startY = start.Position.Y.High;
            int endX = end.Position.X.High;
            int endY = end.Position.Y.High;

            int dx = endX - startX;
            int dy = endY - startY;

            int offsetX = 0;
            int offsetY = 0;
            int offsetDestinationX = 0;
            int offsetDestinationY = 0;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                // horizontal difference greater than vertical difference
                offsetY = Classes.Editor.Constants.ENTITY_NAME_BOX_HALF_HEIGHT;
                offsetDestinationY = Classes.Editor.Constants.ENTITY_NAME_BOX_HALF_HEIGHT;

                if (dx > 0)
                {
                    offsetX = Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH;
                }
                else
                {
                    offsetDestinationX = Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH;
                }
            }
            else
            {
                // vertical difference greater than horizontal difference
                offsetX = Classes.Editor.Constants.ENTITY_NAME_BOX_HALF_WIDTH;
                offsetDestinationX = Classes.Editor.Constants.ENTITY_NAME_BOX_HALF_WIDTH;

                if (dy > 0)
                {
                    offsetY = Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT;
                }
                else
                {
                    offsetDestinationY = Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT;
                }
            }

            d.DrawArrow(startX + offsetX,
                        startY + offsetY,
                        end.Position.X.High + offsetDestinationX,
                        end.Position.Y.High + offsetDestinationY,
                        Color.GreenYellow);
        }
        public void DrawCenteredLinkArrow(GraphicsHandler d, RSDKv5.SceneEntity start, RSDKv5.SceneEntity end, Color? colur = null)
        {
            if (SetFilter(end) == true) return;
            Color color = (colur != null ? colur.Value : Color.GreenYellow);
            int startX = start.Position.X.High;
            int startY = start.Position.Y.High;
            int endX = end.Position.X.High;
            int endY = end.Position.Y.High;

            int dx = endX - startX;
            int dy = endY - startY;

            int offsetX = 0;
            int offsetY = 0;
            int offsetDestinationX = 0;
            int offsetDestinationY = 0;

            d.DrawArrow(startX + offsetX,
                        startY + offsetY,
                        end.Position.X.High + offsetDestinationX,
                        end.Position.Y.High + offsetDestinationY,
                        color);
        }
        public void DrawCenteredSpline(GraphicsHandler d, RSDKv5.SceneEntity start, RSDKv5.SceneEntity end, Int32 length1, Int32 angle1, Int32 length2, Int32 angle2)
        {
            if (SetFilter(end) == true) return;
            int startX = start.Position.X.High;
            int startY = start.Position.Y.High;
            int endX = end.Position.X.High;
            int endY = end.Position.Y.High;

            int dx = endX - startX;
            int dy = endY - startY;

            int offsetX = 0;
            int offsetY = 0;
            int offsetDestinationX = 0;
            int offsetDestinationY = 0;

            int x1 = startX + offsetX;
            int y1 = startY + offsetY;
            int x2 = end.Position.X.High + offsetDestinationX;
            int y2 = end.Position.Y.High + offsetDestinationY;

            int x3 = x1;
            int y3 = y2;
            int x4 = x2;
            int y4 = y1;

            //d.DrawBézierSplineCubic(x1, y1, x3, y3, x4, y4, x2, y2, Color.YellowGreen);

            // d.DrawLine(x1, y1, x1, y2, Color.YellowGreen);
            // d.DrawLine(x1, y1, x2, y1, Color.YellowGreen);
            // d.DrawLine(x2, y2, x2, y1, Color.YellowGreen);
            // d.DrawLine(x2, y2, x1, y2, Color.YellowGreen);

            d.DrawLine(x1, y1, x2, y2, Color.YellowGreen);



        }
        public bool SetFilter(RSDKv5.SceneEntity entity)
        {
            bool filteredOut = false;
            if (HasFilter(entity))
            {
                int filter = entity.GetAttribute("filter").ValueUInt8;

                /**
                 * 1 or 5 = Both
                 * 2 = Mania
                 * 4 = Encore
				 * 255 = Pinball
                 * 
                 */
                filteredOut =
                    ((filter == 1 || filter == 5) && !Settings.MyDefaults.ShowBothEntities) ||
                    (filter == 2 && !Settings.MyDefaults.ShowManiaEntities) ||
                    (filter == 4 && !Settings.MyDefaults.ShowEncoreEntities) ||
                    (filter == 255 && !Settings.MyDefaults.ShowPinballEntities) ||
                    ((filter < 1 || filter == 3 || filter > 5 && filter != 255) && !Settings.MyDefaults.ShowOtherEntities);
            }
            else
            {
                filteredOut = !Settings.MyDefaults.ShowFilterlessEntities;
            }

            if (EditorInstance != null)
            {
                if (Classes.Editor.SolutionState.entitiesTextFilter != "" && !entity.Object.Name.Name.Contains(Classes.Editor.SolutionState.entitiesTextFilter))
                {
                    filteredOut = true;
                }
            }
            return filteredOut;
        }
        public bool HasFilter(RSDKv5.SceneEntity entity)
        {
            return entity.attributesMap.ContainsKey("filter") && entity.attributesMap["filter"].Type == AttributeTypes.UINT8;
        }

    }
}
