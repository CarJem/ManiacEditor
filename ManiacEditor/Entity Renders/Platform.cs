using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using SharpDX;
using System.Runtime.InteropServices;
using System.Data;

namespace ManiacEditor.Entity_Renders
{
    public class Platform : EntityRenderer
    {
        //EditorAnimations platformMove = new EditorAnimations();

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int frameID = 0;
            int targetFrameID = -1;
            var attribute = entity.attributesMap["frameID"];
            int angle = (int)entity.attributesMap["angle"].ValueInt32;
            int angleRotate = (int)entity.attributesMap["angle"].ValueInt32;
            int type = (int)entity.attributesMap["type"].ValueVar;
            int amplitudeX = (int)entity.attributesMap["amplitude"].ValuePosition.X.High;
            int amplitudeY = (int)entity.attributesMap["amplitude"].ValuePosition.Y.High;
            int childCount = (int)entity.attributesMap["childCount"].ValueVar;
            bool hasTension = entity.attributesMap["hasTension"].ValueBool;
            UInt32 speed = e.FetchAttribute.AttributesMapVar("speed", entity);
            int angleStateX = 0;
            int angleStateY = 0;

            var platformIcon = e.LoadAnimation2("EditorIcons", d, 0, 8, false, false, false);
            

            switch (attribute.Type)
            {
                case AttributeTypes.UINT8:
                    targetFrameID = attribute.ValueUInt8;
                    break;
                case AttributeTypes.INT8:
                    targetFrameID = attribute.ValueInt8;
                    break;
                case AttributeTypes.VAR:
                    targetFrameID = (int)attribute.ValueVar;
                    break;
            }
            int aminID = 0;
            EditorEntity.EditorAnimation editorAnim = null;
            bool doNotShow = false;
            while (true)
            {

                try
                {
                    if (targetFrameID == -1)
                    {
                        doNotShow = true;
                    }
                        editorAnim = e.LoadAnimation("Platform", d, aminID, -1, false, false, false, 0);

                        if (type == 4)
                        {
                            editorAnim = e.LoadAnimation("Platform", d, -2, 0, false, false, false, 0);
                        }

                        if (editorAnim == null) return; // no animation, bail out

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
                    throw new ApplicationException($"Pop Loading Platforms! {aminID}", i);
                }
            }
            var tensionBall = e.LoadAnimation("Platform", d, aminID, frameID + 1, false, false, false, 0);
            var tensionBallCenter = e.LoadAnimation("Platform", d, aminID, frameID + 2, false, false, false, 0);
            if (type == 4)
            {
                tensionBall = e.LoadAnimation("Platform", d, -2, 1, false, false, false, 0);
                tensionBallCenter = e.LoadAnimation("Platform", d, -2, 2, false, false, false, 0);
            }
            if (editorAnim.Frames.Count != 0 && platformIcon != null)
            {
                EditorEntity.EditorAnimation.EditorFrame frame = null;
                if (editorAnim.Frames[0].Entry.FrameSpeed > 0 && doNotShow == false)
                {
                    frame = editorAnim.Frames[e.index];
                }
                else if (doNotShow == true)
                {
                    frame = platformIcon.Frames[e.index];
                }
                else
                {
                    frame = editorAnim.Frames[frameID > 0 ? frameID : 0];
                }

                e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                
                if ((amplitudeX != 0 || amplitudeY != 0) && type == 2 && e.Selected)
                {
                    d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + amplitudeX, y + frame.Frame.CenterY + amplitudeY,
                        frame.Frame.Width, frame.Frame.Height, false, 125);
                    d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX - amplitudeX, y + frame.Frame.CenterY - amplitudeY,
                        frame.Frame.Width, frame.Frame.Height, false, 125);
                    d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                
                
                if (type == 2 || type == 7)
                {


                    int[] position = new int[2] { 0, 0 };
                    int posX = amplitudeX;
                    int posY = amplitudeY;
                    //Negative Values only work atm
                    if (amplitudeX <= -1) posX = -posX;
                    if (amplitudeY <= -1) posY = -posY;

                    if (amplitudeX != 0 && amplitudeY == 0)
                    {
                        position = e.EditorAnimations.ProcessMovingPlatform2(posX, 0, x, y, frame.Frame.Width, frame.Frame.Height, speed);
                    }
                    if (amplitudeX == 0 && amplitudeY != 0)
                    {
                        position = e.EditorAnimations.ProcessMovingPlatform2(0, posY, x, y, frame.Frame.Width, frame.Frame.Height, speed);
                    }
                    if (amplitudeX != 0 && amplitudeY != 0)
                    {
                        // Since we can don't know how to do it other than x or y yet
                        position = e.EditorAnimations.ProcessMovingPlatform2(posX, posY, x, y, frame.Frame.CenterX, frame.Frame.Height, speed);
                    }

                    if (childCount != 0)
                    {
                        for (int i = 0; i < childCount; i++)
                        {
                            try
                            {
                                EditorEntity childEntity = e.drawEntityList.Where(t => t.Entity.SlotID == e.Entity.SlotID + (i + 1)).FirstOrDefault();
                                childEntity.childDraw = true;
                                childEntity.childX = position[0];
                                childEntity.childY = -position[1];
                            }
                            catch
                            {

                            }

                        }
                    }

                    d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + position[0], y + frame.Frame.CenterY - position[1],
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                
                
                else if ((amplitudeX != 0 || amplitudeY != 0) && type == 3)
                {
                    e.ProcessMovingPlatform(angle);
                    angle = e.platformAngle;
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
                        EditorEntity.EditorAnimation.EditorFrame frame3 = tensionBall.Frames[0];
                        EditorEntity.EditorAnimation.EditorFrame frame4 = tensionBallCenter.Frames[0];
                        for (int i = 0; i < tensionCount; i++)
                        {
                            int[] linePoints = RotatePoints(x + (16) * i, y, x, y, angle);
                            if (i == 0)
                            {
                                d.DrawBitmap(frame4.Texture,
                                    linePoints[0] + frame4.Frame.CenterX,
                                    linePoints[1] + frame4.Frame.CenterY,
                                    frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                            }
                            else
                            {
                                d.DrawBitmap(frame3.Texture,
                                    linePoints[0] + frame3.Frame.CenterX,
                                    linePoints[1] + frame3.Frame.CenterY,
                                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                            }

                        }
                    }

                    if (childCount != 0)
                    {
                        for (int i = 0; i < childCount; i++)
                        {
                            EditorEntity childEntity = e.drawEntityList.Where(t => t.Entity.SlotID == e.Entity.SlotID + (i + 1)).FirstOrDefault();
                            childEntity.childDraw = true;
                            childEntity.childX = newX;
                            childEntity.childY = -newY;
                        }
                    }

                    d.DrawBitmap(frame.Texture, (x + newX) + frame.Frame.CenterX, (y - newY) + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                }

                else if ((amplitudeX != 0 || amplitudeY != 0) && type == 4)
                {
                    //Must improve upon, this isn't the right way of implementing this platform type.
                    /*amplitudeX *= 4;
                    amplitudeY *= 4;
                    double xd = x;
                    double yd = y;
                    double x2 = x + amplitudeX - amplitudeX / 3.7;
                    double y2 = y + amplitudeY - amplitudeY / 3.7;
                    double radius = Math.Pow(x2 - xd, 2) + Math.Pow(y2 - yd, 2);
                    int radiusInt = (int)Math.Sqrt(radius);
                    int newX = (int)(radiusInt * Math.Cos(Math.PI * angle / 128));
                    int newY = (int)(radiusInt * Math.Sin(Math.PI * angle / 128));
                    int tensionCount = radiusInt / 16;
                    */
                    int tensionCount = amplitudeY;
                    int newY = amplitudeY * 16;
                    if (tensionBall.Frames.Count != 0 && tensionBallCenter.Frames.Count != 0)
                    {
                        EditorEntity.EditorAnimation.EditorFrame frame3 = tensionBall.Frames[0];
                        EditorEntity.EditorAnimation.EditorFrame frame4 = tensionBallCenter.Frames[0];
                        for (int i = 0; i < tensionCount; i++)
                        {

                            int[] linePoints = RotatePoints(x, y + (16) * i, x, y, angle);
                            if (i == 0)
                            {
                                d.DrawBitmap(frame4.Texture,
                                    linePoints[0] + frame4.Frame.CenterX,
                                    linePoints[1] + frame4.Frame.CenterY,
                                    frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                            }
                            else
                            {
                                d.DrawBitmap(frame3.Texture,
                                    linePoints[0] + frame3.Frame.CenterX,
                                    linePoints[1] + frame3.Frame.CenterY,
                                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                            }

                        }
                    }

                    d.DrawBitmap(frame.Texture, (x) + frame.Frame.CenterX, (y) + frame.Frame.CenterY + newY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                }
                else
                {

                    d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + angleStateX, y + frame.Frame.CenterY - angleStateY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
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


        public override string GetObjectName()
        {
            return "Platform";
        }
    }
}
