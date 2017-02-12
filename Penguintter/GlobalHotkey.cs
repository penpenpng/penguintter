using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Penguintter
{
    class GlobalHotkey
    {
        private const int WM_HOTKEY = 0x0312;
        private IntPtr _windowHandle;
        private Dictionary<int, EventHandler> _hotkeyEvents;

        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, int MOD_KEY, int VK);

        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);

        public GlobalHotkey(Window window)
        {

            WindowInteropHelper _host = new WindowInteropHelper(window);
            this._windowHandle = _host.Handle;

            ComponentDispatcher.ThreadPreprocessMessage
                += ComponentDispatcher_ThreadPreprocessMessage;

            _hotkeyEvents = new Dictionary<int, System.EventHandler>();
        }

        public void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message != WM_HOTKEY) return;

            var hotkeyID = msg.wParam.ToInt32();
            if (!_hotkeyEvents.Any((x) => x.Key == hotkeyID)) return;

            new ThreadStart(
                () => _hotkeyEvents[hotkeyID](this, EventArgs.Empty)
            ).Invoke();
        }

        private int i = 0x0000;
        public void Regist(ModifierKeys modkey, Key trigger, EventHandler eh)
        {
            var imod = modkey.ToInt32();
            var itrg = KeyInterop.VirtualKeyFromKey(trigger);

            while ((++i < 0xc000) && RegisterHotKey(_windowHandle, i, imod, itrg) == 0) ;

            if (i < 0xc000)
            {
                _hotkeyEvents.Add(i, eh);
            }
        }

        public void Unregist()
        {
            foreach (var hotkeyid in _hotkeyEvents.Keys)
            {
                UnregisterHotKey(_windowHandle, hotkeyid);
            }
        }
    }

    static class Extention
    {
        public static int ToInt32(this ModifierKeys m)
        {
            return (int)m;
        }
    }
}
