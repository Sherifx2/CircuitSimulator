using System;
using System.Drawing;
using System.Runtime.InteropServices;

class ProgramEntry
{
    public static bool GLOBAL_RUNNING = true;
    const string WINDOW_CLASS = "LogicGateSimWindowClass";

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
    static void PaintWindow(IntPtr hWnd)
    {
        PAINTSTRUCT PS;
        IntPtr HDC = BeginPaint(hWnd, out PS);

        using (Graphics g = Graphics.FromHdc(HDC))
        {
            g.Clear(SystemColors.Window);

            Color rectColor = Color.Black;
            Color circleColor = Color.Red;
            int rectX = 100;
            int rectY = 100;
            int rectWidth = 200;
            int rectHeight = 100;
            int circleRadius = 20;
            using (Brush rectBrush = new SolidBrush(rectColor))
            using (Brush circleBrush = new SolidBrush(circleColor))
            {
                g.FillRectangle(rectBrush, rectX, rectY, rectWidth, rectHeight);
                int inputCircleX = rectX - circleRadius;
                int inputCircleY = rectY + (rectHeight - circleRadius) / 2;
                int outputCircleX = rectX + rectWidth;
                int outputCircleY = rectY + (rectHeight - circleRadius) / 2;
                g.FillEllipse(circleBrush, inputCircleX, inputCircleY, circleRadius * 2, circleRadius * 2);
                g.FillEllipse(circleBrush, outputCircleX, outputCircleY, circleRadius * 2, circleRadius * 2);
            }
        }
        EndPaint(hWnd, ref PS);
    }

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

    const int WM_CLOSE = 0x0010;
    const int WM_PAINT = 0x000F;

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

    static void Main(string[] args)
    {
        WNDCLASSEX wcex = new WNDCLASSEX
        {
            cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
            style = 0,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate<WndProcDelegate>(WndProc),
            hInstance = Marshal.GetHINSTANCE(typeof(ProgramEntry).Module),
            lpszClassName = WINDOW_CLASS
        };

        ushort classAtom = RegisterClassEx(ref wcex);

        if(classAtom == 0)
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
        }
    }
    delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
