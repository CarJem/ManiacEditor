using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class LottoBall : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int lottoNum = (int)entity.attributesMap["lottoNum"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            bool ignoreNum = false;
            int frameID1 = 0;
            int frameID2 = 0;
            int frameID3 = 0;
            switch (type)
            {
                case 1:
                    frameID1 = 1;
                    break;
                case 2:
                    frameID1 = 2;
                    break;
                case 3:
                    frameID1 = 3;
                    break;
                default:
                    frameID1 = 0;
                    break;
            }
            if (type == 1 || type == 0)
            {
                if (lottoNum <= 9)
                {
                    frameID2 = 0;
                    frameID3 = lottoNum;
                }
                else if (lottoNum <= 99)
                {
                    int lottoNumReduced = lottoNum;
                    int lottoNumTens = 0;
                    for (int i = 0; lottoNumReduced <= 9; i++)
                    {
                        if (lottoNumReduced >= 10)
                        {
                            lottoNumReduced = lottoNumReduced - 10;
                            lottoNumTens++;
                        }
                    }
                    frameID2 = lottoNumTens;
                    frameID3 = lottoNumReduced;
                }
                else
                {
                    frameID2 = 9;
                    frameID3 = 9;
                }
            }
            if (type == 2 && lottoNum <= 9)
            {
                frameID2 = 10;
                frameID3 = lottoNum;
            }
            if (type == 3)
            {
                frameID2 = 11;
                ignoreNum = true;
            }


            var ballAnim = Editor.Instance.EntityDrawing.LoadAnimation2("LottoBall", d.DevicePanel, 0, frameID1, fliph, flipv, false);
            var numAnimL = Editor.Instance.EntityDrawing.LoadAnimation2("LottoBall", d.DevicePanel, 1, frameID2, fliph, flipv, false);
            var numAnimR = Editor.Instance.EntityDrawing.LoadAnimation2("LottoBall", d.DevicePanel, 2, frameID3, fliph, flipv, false);
            if (ballAnim != null && ballAnim.Frames.Count != 0 && numAnimL != null && numAnimL.Frames.Count != 0 && numAnimR != null && numAnimR.Frames.Count != 0)
            {
                var frame = ballAnim.Frames[Animation.index];
                var frame2 = numAnimL.Frames[Animation.index];
                var frame3 = numAnimR.Frames[Animation.index];


                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - ballAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - ballAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - numAnimL.Frames[0].Frame.Width) : 0),
                    y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - numAnimL.Frames[0].Frame.Height) : 0),
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                if (ignoreNum != true)
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                        x + frame3.Frame.PivotX - (fliph ? (frame3.Frame.Width - numAnimR.Frames[0].Frame.Width) : 0),
                        y + frame3.Frame.PivotY + (flipv ? (frame3.Frame.Height - numAnimR.Frames[0].Frame.Height) : 0),
                        frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                }

            }
        }

        public override string GetObjectName()
        {
            return "LottoBall";
        }
    }
}
