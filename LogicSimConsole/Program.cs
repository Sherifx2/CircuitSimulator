using System;
using System.Drawing;
using System.Runtime.InteropServices;
using static Program;

class Program
{
    public static bool GLOBAL_RUNNING = true;
    const string WINDOW_CLASS = "LogicGateSimWindowClass";

    static List<Gate> gates = new List<Gate>();

    // DLL Hell
    [DllImport("user32.dll")]
    static extern IntPtr CreateWindowEx(
        uint dwExStyle,
        string lpClassName,
        string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [DllImport("user32.dll")]
    static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    static extern bool TranslateMessage([In] ref MSG lpMsg);

    [DllImport("user32.dll")]
    static extern IntPtr DispatchMessage([In] ref MSG lpMsg);
    [DllImport("user32.dll", SetLastError = true)]
    static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpwcx);
    [DllImport("user32.dll")]
    static extern IntPtr BeginPaint(IntPtr hWnd, out PAINTSTRUCT lpPaint);

    [DllImport("user32.dll")]
    static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);
    [DllImport("user32.dll")]
    static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

    // Struct Hell
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public POINT pt;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WNDCLASSEX
    {
        public uint cbSize;
        public uint style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public int fErase;
        public RECT rcPaint;
        public int fRestore;
        public int fIncUpdate;
        public byte[] rgbReserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    // WinUser.h defs
    const uint WS_OVERLAPPED = 0x00CF0000;
    const uint WS_VISIBLE = 0x10000000;

    const uint PM_REMOVE = 0x0001;
    const int WM_CLOSE = 0x0010;
    const int WM_PAINT = 0x000F;
    const int WM_KEYFIRST = 0x0100;
    const int WM_MOUSEFIRST = 0x0200;
    const int WM_CHAR = 0x0102;

    const int VK_ESCAPE = 0x1B;

    static void PaintWindow(IntPtr hWnd)
    {
        PAINTSTRUCT PS;
        IntPtr HDC = BeginPaint(hWnd, out PS);

        using (Graphics g = Graphics.FromHdc(HDC))
        {
            g.Clear(SystemColors.Window);
            foreach (Gate gate in gates)
            {
                Rectangle gateBounds = new Rectangle(gate.XPosition, gate.YPosition, gate.BodyWidth, gate.BodyWidth);
                gate.Paint(g, gateBounds);
            }
        }
        EndPaint(hWnd, ref PS);
    }

    static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case WM_CLOSE:
                GLOBAL_RUNNING = false;
                break;
            case WM_PAINT:
                PaintWindow(hWnd);
                break;
            default:
                return DefWindowProc(hWnd, msg, wParam, lParam);
        }
        return 0;
    }
    static void ProcessPendingMessages(MSG msg)
    {
        while (PeekMessage(out msg, 0, 0, 0, PM_REMOVE))
        {
            if (msg.message == WM_CHAR)
            {
                if (msg.wParam == VK_ESCAPE)
                {
                    GLOBAL_RUNNING = false;
                }
            }
            TranslateMessage(ref msg);
            DispatchMessage(ref msg);
        }
    }
    static void Main(string[] args)
    {
        //**-----------------------------------------------------
        // ADD GATES HERE
        //**-----------------------------------------------------
        AndGate andGate = new AndGate(2, 1);
        gates.Add(andGate);
        //**-----------------------------------------------------

        WNDCLASSEX wcex = new WNDCLASSEX
        {
            cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
            style = 0,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate<WndProcDelegate>(WndProc),
            hInstance = Marshal.GetHINSTANCE(typeof(Program).Module),
            lpszClassName = WINDOW_CLASS
        };

        ushort classAtom = RegisterClassEx(ref wcex);

        if (classAtom == 0)
        {
            throw new SystemException("Failed to register the window class");
        }

        IntPtr hWnd = CreateWindowEx(
            0,
            WINDOW_CLASS,
            "Logic Gate Sim",
            WS_OVERLAPPED | WS_VISIBLE,
            100,
            100,
            800,
            600,
            0,
            0,
            0,
            0
        );

        if (hWnd == 0)
        {
            throw new SystemException("Failed to create window");

        }
        MSG msg;
        while (GLOBAL_RUNNING)
        {
            GetMessage(out msg, 0, 0, 0);
            TranslateMessage(ref msg);
            DispatchMessage(ref msg);
            ProcessPendingMessages(msg);

        }
    }
    delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
