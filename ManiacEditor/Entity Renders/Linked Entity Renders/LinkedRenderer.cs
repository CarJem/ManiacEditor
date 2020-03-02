using System;
using RSDKv5;
using Color = System.Drawing.Color;



namespace ManiacEditor.Entity_Renders
{
    public abstract class LinkedRenderer
    {
        public Controls.Editor.MainEditor EditorInstance;

        public virtual void Draw(Structures.LinkedEntityRenderProp properties)
        {
            
        }

        public abstract string GetObjectName();
        public void DrawLinkArrow(Methods.Draw.GraphicsHandler d, RSDKv5.SceneEntity start, RSDKv5.SceneEntity end)
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
                offsetY = Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HALF_HEIGHT;
                offsetDestinationY = Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HALF_HEIGHT;

                if (dx > 0)
                {
                    offsetX = Methods.Editor.EditorConstants.ENTITY_NAME_BOX_WIDTH;
                }
                else
                {
                    offsetDestinationX = Methods.Editor.EditorConstants.ENTITY_NAME_BOX_WIDTH;
                }
            }
            else
            {
                // vertical difference greater than horizontal difference
                offsetX = Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HALF_WIDTH;
                offsetDestinationX = Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HALF_WIDTH;

                if (dy > 0)
                {
                    offsetY = Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HEIGHT;
                }
                else
                {
                    offsetDestinationY = Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HEIGHT;
                }
            }

            d.DrawArrow(startX + offsetX,
                        startY + offsetY,
                        end.Position.X.High + offsetDestinationX,
                        end.Position.Y.High + offsetDestinationY,
                        Color.GreenYellow);
        }
        public void DrawCenteredLinkArrow(Methods.Draw.GraphicsHandler d, RSDKv5.SceneEntity start, RSDKv5.SceneEntity end, Color? colur = null)
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
        public void DrawCenteredSpline(Methods.Draw.GraphicsHandler d, RSDKv5.SceneEntity start, RSDKv5.SceneEntity end, Int32 length1, Int32 angle1, Int32 length2, Int32 angle2)
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
                    ((filter == 1 || filter == 5) && !Properties.Settings.MyDefaults.ShowBothEntities) ||
                    (filter == 2 && !Properties.Settings.MyDefaults.ShowManiaEntities) ||
                    (filter == 4 && !Properties.Settings.MyDefaults.ShowEncoreEntities) ||
                    (filter == 255 && !Properties.Settings.MyDefaults.ShowPinballEntities) ||
                    ((filter < 1 || filter == 3 || filter > 5 && filter != 255) && !Properties.Settings.MyDefaults.ShowOtherEntities);
            }
            else
            {
                filteredOut = !Properties.Settings.MyDefaults.ShowFilterlessEntities;
            }

            if (EditorInstance != null)
            {
                if (Methods.Editor.SolutionState.entitiesTextFilter != "" && !entity.Object.Name.Name.Contains(Methods.Editor.SolutionState.entitiesTextFilter))
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
