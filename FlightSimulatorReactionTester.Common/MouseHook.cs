﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FlightSimulatorReactionTester.Common
{
    public enum ButtonEvent
    {
        LeftButtonDown,
        RightButtonDown,
        BackButtonDown,
        ForwardButtonDown
    }

    public static class MouseHook
    {
        public static event EventHandler Action = delegate { };
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private const int WH_MOUSE_LL = 14;
        private static MouseMessage _mouseMessageToListen;
        private static XButton _xButtonToListen;

        #region P/Invoke statements
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion
        /// <summary>
        /// Starts listening for system mouse events
        /// </summary>
        public static void Start(ButtonEvent buttonEvent)
        {
            ChangeButtonEvent(buttonEvent);
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                // Sets hook to listen for mouse events
                _hookID = SetWindowsHookEx(WH_MOUSE_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public static void ChangeButtonEvent(ButtonEvent buttonEvent)
        {
            switch (buttonEvent)
            {
                case ButtonEvent.LeftButtonDown:
                    _mouseMessageToListen = MouseMessage.WM_LBUTTONDOWN;
                    break;
                case ButtonEvent.RightButtonDown:
                    _mouseMessageToListen = MouseMessage.WM_RBUTTONDOWN;
                    break;
                case ButtonEvent.BackButtonDown:
                    _mouseMessageToListen = MouseMessage.WM_XBUTTONDOWN;
                    _xButtonToListen = XButton.Back;
                    break;
                case ButtonEvent.ForwardButtonDown:
                    _mouseMessageToListen = MouseMessage.WM_XBUTTONDOWN;
                    _xButtonToListen = XButton.Forward;
                    break;
                default:
                    throw new Exception($"Unexpected buttonEvent {buttonEvent}");
            }
        }

        /// <summary>
        /// Stops listening for system mouse events
        /// </summary>
        public static void Stop()
        {
            UnhookWindowsHookEx(_hookID);
        }

        /// <summary>
        /// Method called when system reported a mouse event
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam">Identifier of mouse message (i.e. left mouse click)</param>
        /// <param name="lParam">Pointer to structure containing additional information about event</param>
        /// <returns></returns>
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct;
                switch ((MouseMessage)wParam)
                {
                    case MouseMessage.WM_LBUTTONDOWN:
                        // Inform subscribers about mouse event
                        if (_mouseMessageToListen == MouseMessage.WM_LBUTTONDOWN)
                        {
                            Action?.Invoke(null, EventArgs.Empty);
                        }
                        break;
                    case MouseMessage.WM_RBUTTONDOWN:
                        // Inform subscribers about mouse event
                        if (_mouseMessageToListen == MouseMessage.WM_RBUTTONDOWN)
                        {
                            Action?.Invoke(null, EventArgs.Empty);
                        }
                        break;
                    case MouseMessage.WM_XBUTTONDOWN:
                        if (_mouseMessageToListen == MouseMessage.WM_XBUTTONDOWN)
                        {
                            KeyState fwKeys = GetKeyStateWParam(lParam);
                            hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            var mouseDataHI = HIWORD((IntPtr)hookStruct.mouseData);
                            if (mouseDataHI == (ushort)XButton.Back && _xButtonToListen == XButton.Back)
                            {
                                Action?.Invoke(null, EventArgs.Empty);
                            }
                            else if (mouseDataHI == (ushort)XButton.Forward && _xButtonToListen == XButton.Forward)
                            {
                                Action?.Invoke(null, EventArgs.Empty);
                            }
                        }
                        break;
                }
            }
            // Pass the hook information to the next hook procedure in the current hook chain
            // Otherwise other applications will not receive this mouse event
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        internal static UInt16 LOWORD(IntPtr dwValue)
        {
            return (UInt16)(((UInt32)dwValue) & 0x0000FFFF);
        }

        internal static UInt16 HIWORD(IntPtr dwValue)
        {
            return (UInt16)((((UInt32)dwValue) & 0xFFFF0000) >> 0x10);
        }

        public static void CopyLeastSignificantBytes(byte[] source, byte[] destination, int startByte = 0)
        {
            int sourceIndex;
            if (BitConverter.IsLittleEndian)
            {
                sourceIndex = startByte;
            }
            else
            {
                sourceIndex = source.Length - startByte - destination.Length;
            }
            for (int destinationIndex = 0; destinationIndex < destination.Length; destinationIndex++, sourceIndex++)
            {
                byte nextByte;
                if (0 <= sourceIndex && sourceIndex < source.Length)
                {
                    nextByte = source[sourceIndex];
                }
                else
                {
                    nextByte = 0;
                }
                destination[destinationIndex] = nextByte;
            }
        }
        public static KeyState GetKeyStateWParam(IntPtr wParam)
        {
            byte[] source = BitConverter.GetBytes(wParam.ToInt64());
            byte[] intermediate = new byte[2];
            CopyLeastSignificantBytes(source, intermediate);
            byte[] final = new byte[4];
            CopyLeastSignificantBytes(intermediate, final);
            return (KeyState)BitConverter.ToInt32(final, 0);
        }

        private enum XButton : ushort
        {
            Back = 0x0001,
            Forward = 0x0002
        }

        [Flags]
        public enum KeyState : int
        {

            /// <summary>
            /// No flags are set.
            /// </summary>
            MK_NONE = 0x0000,

            /// <summary>
            /// The SHIFT key is down.
            /// </summary>
            MK_SHIFT = 0x0004,

            /// <summary>
            /// The CTRL key is down.
            /// </summary>
            MK_CONTROL = 0x0008,

            /// <summary>
            /// The left mouse button is down.
            /// </summary>
            MK_LBUTTON = 0x0001,

            /// <summary>
            /// The right mouse button is down.
            /// </summary>
            MK_RBUTTON = 0x0002,

            /// <summary>
            /// The middle mouse button is down.
            /// </summary>
            MK_MBUTTON = 0x0010,

            /// <summary>
            /// The first X button is down.
            /// </summary>
            MK_XBUTTON1 = 0x0020,

            /// <summary>
            /// The second X button is down.
            /// </summary>
            MK_XBUTTON2 = 0x0040,
        }

        private enum MouseMessage
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_XBUTTONDOWN = 0x020B
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
    }
}
