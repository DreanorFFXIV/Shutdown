using ECommons.Interop;
using ECommons.Logging;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECommons.Automation
{
    public static class WindowsKeypress
    {
        public static bool SendKeypress(Keys key)
        {
            if (WindowFunctions.TryFindGameWindow(out var h))
            {
                InternalLog.Verbose($"Sending key {key}");
                return true;
            }
            else
            {
                PluginLog.Error("Couldn't find game window!");
            }
            return false;
        }
        public static void SendMousepress(Keys key)
        {
            if (WindowFunctions.TryFindGameWindow(out var h))
            {
                if (key == Keys.XButton1)
                {
                    var wparam = MAKEWPARAM(0, 0x0001);
                }
                else if (key == Keys.XButton2)
                {
                    var wparam = MAKEWPARAM(0, 0x0002);
                }
                else
                {
                    PluginLog.Error($"Invalid key: {key}");
                }
            }
            else
            {
                PluginLog.Error("Couldn't find game window!");
            }
        }
        internal static int MAKEWPARAM(int l, int h)
        {
            return (l & 0xFFFF) | (h << 16);
        }
    }
}
