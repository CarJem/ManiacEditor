using RSDKv5;


namespace ManiacEditor.Entity_Renders
{
    public class LaunchSpring : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            int angle = (int)e.attributesMap["angle"].ValueInt32;
            int type = (int)e.attributesMap["type"].ValueUInt8;
            int rotation = 0;

            int offset_x1 = 0;
            int offset_y1 = 0;

            int offset_x2 = 0;
            int offset_y2 = 0;

            int offset_x2_type = 0;
            int offset_y2_type = 0;

            int offset_x3 = 0;
            int offset_y3 = 0;


            /*
                    offset_x1 = (int)ManiacEditor.Properties.Settings.MyDevSettings.DevInt1;
                    offset_y1 = (int)ManiacEditor.Properties.Settings.MyDevSettings.DevInt2;
                    offset_x2 = (int)ManiacEditor.Properties.Settings.MyDevSettings.DevInt3;
                    offset_y2 = (int)ManiacEditor.Properties.Settings.MyDevSettings.DevInt4;
                    offset_x2_type = (int)ManiacEditor.Properties.Settings.MyDevSettings.DevInt5;
                    offset_y2_type = (int)ManiacEditor.Properties.Settings.MyDevSettings.DevInt6;
                    offset_x3 = (int)ManiacEditor.Properties.Settings.MyDevSettings.DevInt7;
                    offset_y3 = (int)ManiacEditor.Properties.Settings.MyDevSettings.DevInt8;
             */

            switch (angle)
            {
                case 0:
                    rotation = 0;
                    //Offsets
                    offset_x1 = -20;
                    offset_y1 = -23;
                    offset_x2 = 20;
                    offset_y2 = -23;
                    offset_x2_type = 0;
                    offset_y2_type = 47;
                    offset_x3 = 0;
                    offset_y3 = -8;
                    break;
                case 1:
                    rotation = 45;
                    //Offsets
                    offset_x1 = 3;
                    offset_y1 = -30;
                    offset_x2 = 29;
                    offset_y2 = -4;
                    offset_x2_type = -33;
                    offset_y2_type = 33;
                    offset_x3 = 6;
                    offset_y3 = -6;
                    break;
                case 2:
                    rotation = 90;
                    //Offsets 
                    offset_x1 = 23;
                    offset_y1 = -20;
                    offset_x2 = 23;
                    offset_y2 = 20;
                    offset_x2_type = -47;
                    offset_y2_type = 0;
                    offset_x3 = 8;
                    offset_y3 = 0;
                    break;
                case 3:
                    rotation = 135;
                    //Offsets 
                    offset_x1 = 30;
                    offset_y1 = 3;
                    offset_x2 = 2;
                    offset_y2 = 29;
                    offset_x2_type = -32;
                    offset_y2_type = -32;
                    offset_x3 = 6;
                    offset_y3 = 4;
                    break;
                case 4:
                   rotation = 180;
                    //Offsets 
                    offset_x1 = 20;
                    offset_y1 = 23;
                    offset_x2 = -20;
                    offset_y2 = 23;
                    offset_x2_type = 0;
                    offset_y2_type = -46;
                    offset_x3 = 0;
                    offset_y3 = 9;
                    break;
                case 5:
                    rotation = 225;
                    //Offsets 
                    offset_x1 = -3;
                    offset_y1 = 31;
                    offset_x2 = -31;
                    offset_y2 = 3;
                    offset_x2_type = 33;
                    offset_y2_type = -32;
                    offset_x3 = -5;
                    offset_y3 = 7;
                    break;
                case 6:
                    rotation = 270;
                    //Offsets 
                    offset_x1 = -24;
                    offset_y1 = 20;
                    offset_x2 = -24;
                    offset_y2 = -20;
                    offset_x2_type = 47;
                    offset_y2_type = 0;
                    offset_x3 = -8;
                    offset_y3 = 0;
                    break;
                case 7:
                    rotation = 315;
                    //Offsets
                    offset_x1 = -29;
                    offset_y1 = -3;
                    offset_x2 = -1;
                    offset_y2 = -29;
                    offset_x2_type = 31;
                    offset_y2_type = 32;
                    offset_x3 = -7;
                    offset_y3 = -5;
                    break;
            }

            var Animation = LoadAnimation("LaunchSpring", d, 2, 1);
            DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset_x3, y + offset_y3, Transparency, false, false, rotation);
            Animation = LoadAnimation("LaunchSpring", d, 0, 0);
            DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset_x1, y + offset_y1, Transparency, false, false, rotation);
            Animation = LoadAnimation("LaunchSpring", d, 0, 0);
            DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (type == 0 ? offset_x2_type : 0) + offset_x2, y + (type == 0 ? offset_y2_type : 0) + offset_y2, Transparency, true, false, rotation);
            Animation = LoadAnimation("LaunchSpring", d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, false, false);

        }




        public override string GetObjectName()
        {
            return "LaunchSpring";
        }
    }
}
