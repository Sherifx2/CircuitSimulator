using System.Drawing;
using System.Runtime.InteropServices;

class Program
{
    static bool GLOBAL_RUNNING = true;
    const string WINDOW_CLASS = "LogicGateSimWindowClass";

    private static Bitmap offScreenBuffer;
    private static Graphics bufferGraphics;

    static List<Gate> gates = new List<Gate>();
    static List<Wire> wires = new List<Wire>();


    public static readonly int gridSize = 24;
    static Dictionary<Point, object> gridMap = new Dictionary<Point, object>();

    static Sidebar sidebar;

    static string currentDraggedGate = null;

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
    [DllImport("user32.dll")]
    static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
    [DllImport("user32.dll")]
    static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
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
    const int WM_CHAR = 0x0102;
    const int WM_SIZE = 0x0005;

    const int WM_LBUTTONUP = 0x202;
    const int WM_LBUTTONDOWN = 0x201;
    const int WM_MOUSEMOVE = 0x200;

    const int VK_ESCAPE = 0x1B;

    static void PaintWindow(IntPtr hWnd)
    {
        PAINTSTRUCT PS;
        IntPtr HDC = BeginPaint(hWnd, out PS);
        RECT rect = PS.rcPaint;

        if (offScreenBuffer == null || offScreenBuffer.Width != rect.right || offScreenBuffer.Height != rect.bottom)
        {
            CreateBuffer(rect.right, rect.bottom);
        }

        bufferGraphics.Clear(SystemColors.Window);


        DrawGrid(bufferGraphics, rect.right, rect.bottom);
        foreach (Wire wire in wires)
        {
            wire.Draw(bufferGraphics);
        }
        foreach (Gate gate in gates)
        {
            Rectangle gateBounds = new Rectangle(gate.Position.X, gate.Position.Y, gate.BodyWidth, gate.BodyHeight);
            gate.Paint(bufferGraphics, gateBounds);
        }
        sidebar.Draw(bufferGraphics);
        using (Graphics g = Graphics.FromHdc(HDC))
        {
            g.DrawImage(offScreenBuffer, 0, 0);
        }
        EndPaint(hWnd, ref PS);
    }
    static void DrawGrid(Graphics graphics, int width, int height)
    {
        Pen gridPen = new Pen(Color.LightGray);
        for (int x = 0; x < width; x += gridSize)
        {
            graphics.DrawLine(gridPen, x, 0, x, height);
        }
        for (int y = 0; y < height; y += gridSize)
        {
            graphics.DrawLine(gridPen, 0, y, width, y);
        }
    }
    static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case WM_SIZE:
                HandleWindowResize(hWnd);
                break;
            case WM_CLOSE:
                GLOBAL_RUNNING = false;
                break;
            case WM_PAINT:
                PaintWindow(hWnd);
                break;
            case WM_LBUTTONUP:
                HandleButtonUp(hWnd, lParam);
                break;
            case WM_LBUTTONDOWN:
                HandleButtonDown(lParam, hWnd);
                break;
            case WM_MOUSEMOVE:
                HandleMouseMove(lParam, hWnd);
                break;
            default:
                return DefWindowProc(hWnd, msg, wParam, lParam);
        }
        return 0;
    }
    private static Point CalculateMousePos(IntPtr lParam)
    {
        int x = lParam.ToInt32() & 0xFFFF;
        int y = (lParam.ToInt32() >> 16) & 0xFFFF;
        return new Point(x, y);
    }
    static (int width, int height) GetWindowSize(IntPtr hWnd)
    {
        RECT rect;
        GetClientRect(hWnd, out rect);
        int width = rect.right - rect.left;
        int height = rect.bottom - rect.top;
        return (width, height);
    }
    private static void CreateBuffer(int width, int height)
    {
        if (bufferGraphics != null)
        {
            bufferGraphics.Dispose();
        }
        if (offScreenBuffer != null)
        {
            offScreenBuffer.Dispose();
        }
        offScreenBuffer = new Bitmap(width, height);
        bufferGraphics = Graphics.FromImage(offScreenBuffer);
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
    static bool dragging = false;
    static bool mouseOnPin = false;
    static bool mouseOnGate = false;
    static bool mouseOnBottomOfGate = false;
    static Gate currentGate;
    static Point lastMousePos;

    private static bool IsColliding(Point point, int width, int height)
    {
        for (int x = point.X; x < point.X + width; x += gridSize)
        {
            for (int y = point.Y; y < point.Y + height; y += gridSize)
            {
                Point gridPoint = new Point(x, y);
                if (gridMap.ContainsKey(gridPoint))
                {
                    return true;
                }
            }
        }
        return false;
    }
    static Pin currentPin;
    static Pin destinationPin;

    static void HandleWindowResize(IntPtr hWnd)
    {
        (int width, int height) = GetWindowSize(hWnd);

        if (sidebar != null)
        {
            sidebar.UpdateSize(width, height);

            InvalidateRect(hWnd, 0, true);
        }
    }
    static void HandleButtonDown(IntPtr lParam, IntPtr hWnd)
    {
        foreach (Gate gate in gates)
        {
            gate.CalculateOutputs();
            InvalidateRect(hWnd, IntPtr.Zero, true);
        }

        Point mousePos = CalculateMousePos(lParam);
        lastMousePos = mousePos;

        mouseOnPin = false;
        mouseOnGate = false;
        mouseOnBottomOfGate = false;
        dragging = false;

        foreach (Gate gate in gates)
        {
            foreach (Pin pin in gate.Pins)
            {
                Rectangle pinBounds = new Rectangle(pin.Position.X, pin.Position.Y, Pin.PinRadius * 2, Pin.PinRadius * 2);
                if (pinBounds.Contains(mousePos))
                {
                    currentPin = pin;
                    mouseOnPin = true;
                    return;
                }
            }
        }

        foreach (Gate gate in gates)
        {
            Rectangle gateBounds = new Rectangle(gate.Position, new Size(gate.BodyWidth, gate.BodyHeight));
            Rectangle bottomDragBounds = new Rectangle(new Point(gate.Position.X, gate.Position.Y + gate.BodyHeight - 12), new Size(gate.BodyWidth, 12));
            if (bottomDragBounds.Contains(mousePos))
            {
                mouseOnBottomOfGate = true;
            }
            if (gateBounds.Contains(mousePos))
            {
                currentGate = gate;
                mouseOnGate = true;
                dragging = true;
                if (gate is Switch switchGate)
                {
                    switchGate.ToggleSwitch();
                    InvalidateRect(hWnd, IntPtr.Zero, true);
                }
                return;
            }
        }


        if (sidebar.IsMouseOverButton(mousePos, out string gateType))
        {
            currentDraggedGate = sidebar.GetCurrentlyDraggedGate();
            dragging = true;
        }
    }

    static void HandleMouseMove(IntPtr lParam, IntPtr hWnd)
    {
        Point mousePos = CalculateMousePos(lParam);
        Point snappedMousePos = SnapToGrid(mousePos);
        if (dragging && currentGate != null)
        {
            Point offset = new Point(snappedMousePos.X - lastMousePos.X, snappedMousePos.Y - lastMousePos.Y);
            lastMousePos = snappedMousePos;
            InvalidateRect(hWnd, 0, true);
            if (mouseOnPin)
            {
            }
            else if (mouseOnBottomOfGate)
            {

            }
            else if (mouseOnGate)
            {
                Point newPosition = new Point(currentGate.Position.X + offset.X, currentGate.Position.Y + offset.Y);
                newPosition = SnapToGrid(newPosition);
                currentGate.Position = newPosition;
            }
        }
    }
    static void HandleButtonUp(IntPtr hWnd, IntPtr lParam)
    {
        Point mousePos = CalculateMousePos(lParam);
        Point snappedMousePos = SnapToGrid(mousePos);
        if (mouseOnPin)
        {
            foreach (Gate gate in gates)
            {
                foreach (Pin pin in gate.Pins)
                {
                    Rectangle pinBounds = new Rectangle(pin.Position.X, pin.Position.Y, Pin.PinRadius * 2, Pin.PinRadius * 2);
                    if (pinBounds.Contains(mousePos))
                    {
                        destinationPin = pin;
                        Wire newWire = new Wire(currentPin.Power, currentPin, destinationPin);
                        newWire.SourcePin = currentPin;
                        currentPin.OutputWire = newWire;
                        destinationPin.InputWire = newWire;
                        newWire.DestinationPin = destinationPin;
                        wires.Add(newWire);
                        newWire.SourcePin.Power = newWire.Powered;
                        destinationPin.Power = newWire.Powered;
                        InvalidateRect(hWnd, IntPtr.Zero, true);
                        return;
                    }
                }
            }
        }

        if (currentDraggedGate != null)
        {
            PlaceDraggedGate(hWnd, currentDraggedGate, snappedMousePos);
            currentDraggedGate = null;
        }

        dragging = false;
    }
    static void PlaceDraggedGate(IntPtr hWnd, string gateType, Point gridPosition)
    {
        if (gridPosition.X < 100)
        {
            return;
        }
        Gate newGate = CreateGateInstance(gateType);
        newGate.Position = gridPosition;
        gridMap[gridPosition] = newGate;
        gates.Add(newGate);
        InvalidateRect(hWnd, IntPtr.Zero, true);
    }
    static Gate CreateGateInstance(string gateType)
    {
        foreach (Gate existingGate in gates)
        {
            if (existingGate.GateName == gateType)
            {
                return existingGate;
            }
        }
        switch (gateType)
        {
            case "AND Gate":
                return new AndGate(2, 1);
            case "OR Gate":
                return new OrGate(2, 1);
            case "NOT Gate":
                return new NotGate(1, 1);
            case "SWITCH":
                return new Switch(1, 1);
            default:
                Console.WriteLine($"Gate type: {gateType}");
                throw new ArgumentException("Gate type Null/ Unknown gate type");
        }
    }
    static Point SnapToGrid(Point point)
    {
        int x = (point.X / gridSize) * gridSize;
        int y = (point.Y / gridSize) * gridSize;
        return new Point(x, y);
    }
    static void Main(string[] args)
    {
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
            1000,
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
        (int windowWidth, int windowHeight) = GetWindowSize(hWnd);
        sidebar = new Sidebar(windowWidth, windowHeight);
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
