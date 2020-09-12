using System;
using SFML.Graphics;
using SharpDX.Direct3D9;

namespace ManiacEditor.Events
{
    public delegate void RenderEventHandler(object sender, DeviceEventArgs e);
    public delegate void CreateDeviceEventHandler(object sender, DeviceEventArgs e);

    public delegate void RenderEventHandlerSFML(object sender, DeviceEventArgsSFML e);
    public delegate void CreateDeviceEventHandlerSFML(object sender, DeviceEventArgsSFML e);

    public class DeviceEventArgs : EventArgs
    {
        private Device _device;

        public Device Device
        {
            get
            {
                return _device;
            }
        }

        public DeviceEventArgs(Device device)
        {
            _device = device;
        }



    }



    public class DeviceEventArgsSFML : EventArgs
    {
        private RenderWindow _device;

        public RenderWindow Device
        {
            get
            {
                return _device;
            }
        }

        public DeviceEventArgsSFML(RenderWindow device)
        {
            _device = device;
        }
    }

}
