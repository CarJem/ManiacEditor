using System;
using System.Linq;
using RSDKv5;
using System.Data;

namespace ManiacEditor.Entity_Renders
{
    public class Platform : EntityRenderer
    {

        //EditorAnimations platformMove = new EditorAnimations();
        public static int PlatformWidth = 0;
        public static int PlatformHight = 0;
        public static int PlatformOffsetX = 0;
        public static int PlatformOffsetY = 0;



        public override void Draw(Structures.EntityRenderProp properties)
        {
            Methods.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;


            int frameID = 0;
            int targetFrameID = -1;
            var attribute = entity.attributesMap["frameID"];
            int angle = (int)entity.attributesMap["angle"].ValueInt32;
            int angleRotate = (int)entity.attributesMap["angle"].ValueInt32;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            int amplitudeX = (int)entity.attributesMap["amplitude"].ValueVector2.X.High;
            int amplitudeY = (int)entity.attributesMap["amplitude"].ValueVector2.Y.High;
            int childCount = (int)entity.attributesMap["childCount"].ValueEnum;
            bool hasTension = entity.attributesMap["hasTension"].ValueBool;
            int speed = Methods.Entities.AttributeHandler.AttributesMapVar("speed", entity);
            int angleStateX = 0;
            int angleStateY = 0;

            var platformIcon = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorIcons2", d.DevicePanel, 0, 20, false, false, false);

            
            if (childCount != previousChildCount)
            {
                for (int z = 0; z < previousChildCount; z++)
                {
                    try
                    {
                        Classes.Scene.Sets.EditorEntity childEntity = Methods.Editor.Solution.Entities.Entities.ToList().Where(t => t.SlotID == entity.SlotID + (z + 1)).FirstOrDefault();
                        childEntity.childDraw = false;
                        childEntity.childDrawAddMode = false;
                        childEntity.childX = 0;
                        childEntity.childY = 0;
                    }
                    catch
                    {

                    }

                }
            }
            

            switch (attribute.Type)
            {
                case AttributeTypes.UINT8:
                    targetFrameID = attribute.ValueUInt8;
                    break;
                case AttributeTypes.INT8:
                    targetFrameID = attribute.ValueInt8;
                    break;
                case AttributeTypes.ENUM:
                    targetFrameID = (int)attribute.ValueEnum;
                    break;
            }
            int aminID = 0;
            Methods.Entities.EntityDrawing.EditorAnimation editorAnim = null;
            bool doNotShow = false;

            while (true)
            {
                try
                {
                    if (targetFrameID == -1) doNotShow = true;
                    editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("Platform", d.DevicePanel, aminID, -1, false, false, false, 0);
                    if (type == 4) editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("Platform", d.DevicePanel, 1, 0, false, false, false, 0);
                    if (editorAnim == null)
                    {
                        return; // no animation, bail out
                    }
                    
                    frameID += editorAnim.Frames.Count;
                    if (targetFrameID < frameID)
                    {
                        int aminStart = (frameID - editorAnim.Frames.Count);
                        frameID = targetFrameID - aminStart;
                        break;
                    }
                    aminID++;
                }
                catch (Exception i)
                {
                    throw new ApplicationException($"Problem Loading Platforms! AnimID: {aminID}", i);
                }
            }

            var tensionBall = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("Platform", d.DevicePanel, aminID, frameID + 1, false, false, false, 0);
            var tensionBallCenter = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("Platform", d.DevicePanel, aminID, frameID + 2, false, false, false, 0);
            if (type == 4) tensionBall = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("Platform", d.DevicePanel, 1, 1, false, false, false, 0);
            if (type == 4) tensionBallCenter = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("Platform", d.DevicePanel, 1, 2, false, false, false, 0);

            if (editorAnim.Frames.Count != 0 && platformIcon != null && editorAnim != null && platformIcon.Frames.Count != 0)
            {

                Methods.Entities.EntityDrawing.EditorAnimation.EditorFrame frame = null;
                if (editorAnim.Frames[0].Entry.SpeedMultiplyer > 0 && doNotShow == false && type != 4)
                {
                    frame = editorAnim.Frames[Animation.index];
                }
                else if (doNotShow == true && platformIcon != null && platformIcon.Frames.Count != 0)
                {
                    frame = platformIcon.Frames[Animation.index];

                }
                else
                {
                    frame = editorAnim.Frames[frameID > 0 ? frameID : 0];
                }

                if (frame != null)
                {
                    PlatformWidth = frame.Frame.Width;
                    PlatformHight = frame.Frame.Height;
                    PlatformOffsetX = frame.Frame.PivotX;
                    PlatformOffsetY = frame.Frame.PivotY;

                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                    if ((amplitudeX != 0 || amplitudeY != 0) && type == 2 && selected)
                    {
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + amplitudeX, y + frame.Frame.PivotY + amplitudeY,
                            frame.ImageWidth, frame.ImageHeight, false, 125);
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX - amplitudeX, y + frame.Frame.PivotY - amplitudeY,
                            frame.ImageWidth, frame.ImageHeight, false, 125);
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                    }


                    if (type == 2 || type == 7)
                    {
                        if (type == 7)
                        {
                            amplitudeX /= 2;
                            amplitudeY /= 2;
                        }

                        int[] position = new int[2] { 0, 0 };
                        int posX = amplitudeX;
                        int posY = amplitudeY;
                        //Negative Values only work atm
                        if (amplitudeX <= -1) posX = -posX;
                        if (amplitudeY <= -1) posY = -posY;

                        if (amplitudeX != 0 && amplitudeY == 0)
                        {
                            position = Animation.ProcessMovingPlatform2(posX, 0, x, y, frame.Frame.Width, frame.Frame.Height, (int)speed);
                        }
                        if (amplitudeX == 0 && amplitudeY != 0)
                        {
                            position = Animation.ProcessMovingPlatform2(0, posY, x, y, frame.Frame.Width, frame.Frame.Height, (int)speed);
                        }
                        if (amplitudeX != 0 && amplitudeY != 0)
                        {
                            // Since we can don't know how to do it other than x or y yet
                            position = Animation.ProcessMovingPlatform2D(posX, posY, x, y, frame.Frame.Width, frame.Frame.Height, (uint)speed);
                        }

                        if (childCount != 0 && Methods.Editor.Solution.Entities.Entities.ToList().Exists(t => t.SlotID == entity.SlotID + 1))
                        {
                            previousChildCount = childCount;
                            for (int i = 0; i < childCount; i++)
                            {
                                try
                                {
                                    Classes.Scene.Sets.EditorEntity childEntity = Methods.Editor.Solution.Entities.Entities.ToList().Where(t => t.SlotID == entity.SlotID + (i + 1)).FirstOrDefault();
                                    childEntity.childDraw = true;
                                    childEntity.childX = position[0];
                                    childEntity.childY = -position[1];
                                }
                                catch
                                {
                                    break;
                                }

                            }
                        }


                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + position[0], y + frame.Frame.PivotY - position[1],
                        frame.ImageWidth, frame.ImageHeight, false, Transparency);

                    }


                    else if ((amplitudeX != 0 || amplitudeY != 0) && type == 3)
                    {
                        Animation.ProcessMovingPlatform(angle);
                        angle = Animation.platformAngle;
                        double xd = x;
                        double yd = y;
                        double x2 = x + amplitudeX - amplitudeX / 3.7;
                        double y2 = y + amplitudeY - amplitudeY / 3.7;
                        double radius = Math.Pow(x2 - xd, 2) + Math.Pow(y2 - yd, 2);
                        int radiusInt = (int)Math.Sqrt(radius);
                        int newX = (int)(radiusInt * Math.Cos(Math.PI * angle / 128));
                        int newY = (int)(radiusInt * Math.Sin(Math.PI * angle / 128));
                        int tensionCount = radiusInt / 16;
                        if (hasTension == true && tensionBall.Frames.Count != 0 && tensionBallCenter.Frames.Count != 0)
                        {
                            Methods.Entities.EntityDrawing.EditorAnimation.EditorFrame frame3 = tensionBall.Frames[0];
                            Methods.Entities.EntityDrawing.EditorAnimation.EditorFrame frame4 = tensionBallCenter.Frames[0];
                            for (int i = 0; i < tensionCount; i++)
                            {
                                int[] linePoints = RotatePoints(x + (16) * i, y, x, y, angle);
                                if (i == 0)
                                {
                                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame4),
                                        linePoints[0] + frame4.Frame.PivotX,
                                        linePoints[1] + frame4.Frame.PivotY,
                                        frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                                }
                                else
                                {
                                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame3),
                                        linePoints[0] + frame3.Frame.PivotX,
                                        linePoints[1] + frame3.Frame.PivotY,
                                        frame3.ImageWidth, frame3.ImageHeight, false, Transparency);
                                }

                            }
                        }

                        if (childCount != 0 && Methods.Editor.Solution.Entities.Entities.ToList().Exists(t => t.SlotID == entity.SlotID + 1))
                        {
                            previousChildCount = childCount;
                            for (int i = 0; i < childCount; i++)
                            {
                                Classes.Scene.Sets.EditorEntity childEntity = Methods.Editor.Solution.Entities.Entities.ToList().Where(t => t.SlotID == entity.SlotID + (i + 1)).FirstOrDefault();
                                childEntity.childDraw = true;
                                childEntity.childX = newX;
                                childEntity.childY = -newY;
                            }
                        }

                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), (x + newX) + frame.Frame.PivotX, (y - newY) + frame.Frame.PivotY,
                        frame.ImageWidth, frame.ImageHeight, false, Transparency);

                    }

                    else if ((amplitudeX != 0 || amplitudeY != 0) && type == 4)
                    {
                        Animation.ProcessMovingPlatform4(amplitudeX, angle);
                        angle = Animation.platformAngle4;
                        int tensionCount = amplitudeY;


                        //int newY = amplitudeY * 16;

                        //Child Count
                        //double xd = x;
                        //double yd = y;
                        //double x2 = x + amplitudeX - amplitudeX / 3.7;
                        //double y2 = y + amplitudeY - amplitudeY / 3.7 + amplitudeY*16;
                        //double radius = Math.Pow(x2 - xd, 2) + Math.Pow(y2 - yd, 2);
                        //int radiusInt = (int)Math.Sqrt(radius);
                        //int newX = (int)(radiusInt * Math.Cos(Math.PI * angle / 128));
                        //int newY = (int)(radiusInt * Math.Sin(Math.PI * angle / 128));
                        //int tensionCount = radiusInt / 16;

                        if (tensionBall.Frames.Count != 0 && tensionBallCenter.Frames.Count != 0)
                        {
                            Methods.Entities.EntityDrawing.EditorAnimation.EditorFrame frame3 = tensionBall.Frames[0];
                            Methods.Entities.EntityDrawing.EditorAnimation.EditorFrame frame4 = tensionBallCenter.Frames[0];
                            int i = 0;
                            int[] linePoints = RotatePoints(x, y + (16) * i, x, y, angle);
                            for (i = 0; i <= tensionCount; i++)
                            {
                                linePoints = RotatePoints(x, y + (16) * i, x, y, angle);
                                if (i == 0)
                                {
                                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame4),
                                        linePoints[0] + frame4.Frame.PivotX,
                                        linePoints[1] + frame4.Frame.PivotY,
                                        frame4.ImageWidth, frame4.ImageHeight, false, Transparency);
                                }
                                else if (i == tensionCount)
                                {
                                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), linePoints[0] + frame.Frame.PivotX, linePoints[1] + frame.Frame.PivotY,
                                        frame.ImageWidth, frame.ImageHeight, false, Transparency);

                                    if (childCount != 0 && Methods.Editor.Solution.Entities.Entities.ToList().Exists(t => t.SlotID == entity.SlotID + 1)) 
                                    {
                                        previousChildCount = childCount;
                                        for (int z = 0; z < childCount; z++)
                                        {
                                            Classes.Scene.Sets.EditorEntity childEntity = Methods.Editor.Solution.Entities.Entities.ToList().Where(t => t.SlotID == entity.SlotID + (z + 1)).FirstOrDefault();
                                            if (childEntity != null)
                                            {
                                                childEntity.childDraw = true;
                                                if (childEntity.Entity.Object.Name.Name == "ItemBox")
                                                {
                                                    int cX = childEntity.Entity.Position.X.High;
                                                    int cY = childEntity.Entity.Position.Y.High;
                                                    int rX = linePoints[0] - x;
                                                    int rY = linePoints[1] - y;
                                                    linePoints = RotatePoints(cX, cY + (16) * tensionCount, cX, cY, angle);
                                                    childEntity.childDrawAddMode = false;
                                                    childEntity.childX = linePoints[0];
                                                    childEntity.childY = linePoints[1];
                                                }
                                                else
                                                {
                                                    int cX = childEntity.Entity.Position.X.High;
                                                    int cY = childEntity.Entity.Position.Y.High;
                                                    int rX = linePoints[0] - x;
                                                    int rY = linePoints[1] - y;
                                                    linePoints = RotatePoints(cX, cY, cX, cY - (16) * i, angle);
                                                    childEntity.childDrawAddMode = false;
                                                    childEntity.childX = linePoints[0];
                                                    childEntity.childY = linePoints[1];
                                                }
                                            }


                                        }
                                    }

                                }
                                else
                                {
                                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame3),
                                        linePoints[0] + frame3.Frame.PivotX,
                                        linePoints[1] + frame3.Frame.PivotY,
                                        frame3.ImageWidth, frame3.ImageHeight, false, Transparency);
                                }
                            }
                        }
                    }
                    else
                    {

                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + angleStateX, y + frame.Frame.PivotY - angleStateY,
                            frame.ImageWidth, frame.ImageHeight, false, Transparency);
                    }
                }             
            }

        }
        private static int[] RotatePoints(double initX, double initY, double centerX, double centerY, double angle)
        {
            initX -= centerX;
            initY -= centerY;

            if (initX == 0 && initY == 0)
            {
                int[] results2 = { (int)centerX, (int)centerY };
                return results2;
            }

            const double FACTOR = 40.743665431525205956834243423364;

            double hypo = Math.Sqrt(Math.Pow(initX, 2) + Math.Pow(initY, 2));
            double initAngle = Math.Acos(initX / hypo);
            if (initY < 0) initAngle = 2 * Math.PI - initAngle;
            double newAngle = initAngle - angle / FACTOR;
            double finalX = hypo * Math.Cos(newAngle) + centerX;
            double finalY = hypo * Math.Sin(newAngle) + centerY;

            int[] results = { (int)Math.Round(finalX), (int)Math.Round(finalY) };
            return results;
        }

        public override bool isObjectOnScreen(Methods.Draw.GraphicsHandler d, SceneEntity entity, Classes.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            var attribute = entity.attributesMap["frameID"];
            int angle = (int)entity.attributesMap["angle"].ValueInt32;
            int angleRotate = (int)entity.attributesMap["angle"].ValueInt32;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            int amplitudeX = (int)entity.attributesMap["amplitude"].ValueVector2.X.High;
            int amplitudeY = (int)entity.attributesMap["amplitude"].ValueVector2.Y.High;
            int childCount = (int)entity.attributesMap["childCount"].ValueEnum;
            bool hasTension = entity.attributesMap["hasTension"].ValueBool;

            if (type == 0 || type == 1)
            {
                return d.IsObjectOnScreen(x - PlatformOffsetX - PlatformWidth / 2, y - PlatformOffsetY - PlatformHight / 2, PlatformWidth, PlatformHight);
            }
            else
            {
                //Default Type
                return d.IsObjectOnScreen(x - PlatformOffsetX - PlatformWidth / 2, y - PlatformOffsetY - PlatformHight / 2, PlatformWidth, PlatformHight);
            }

        }


        public override string GetObjectName()
        {
            return "Platform";
        }
    }
}
