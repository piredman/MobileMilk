using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Phone.Info;

namespace MobileMilk.Common
{
    public static class Environment
    {
        public static bool InDesignMode()
        {
            var inDesignMode = false;
#if DEBUG
            if (DesignerProperties.IsInDesignTool) inDesignMode = true;
#endif
            return inDesignMode;
        }

        public static bool InEmulator()
        {
            var inEmulator = false;
#if DEBUG
            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
                inEmulator = true;
#endif
            return inEmulator;
        }
    }
}
