using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

public abstract class Gate
{
    private string gateName;
    private Point position;
    private int numOfInputs;
    private int numOfOutputs;
    private int bodyWidth;
    private int bodyHeight;

    protected List<bool> inputs;
    protected List<bool> outputs;

    protected List<Pin> pins = new List<Pin>();

    public string GateName
    {
        get => gateName;
        set
        {
            gateName = value;
        }
    }
    public List<Pin> Pins
    {
        get => pins;
    }
    public int BodyWidth
    {
        get => bodyWidth;
        set
        {
            bodyWidth = value;
        }
    }
    public int BodyHeight
    {
        get => bodyHeight;
        set
        {
            bodyHeight = value;
        }
    }

    public Point Position
    {
        get => position;
        set
        {
            if (value.GetType() == typeof(Point))
            {
                position = value;
                UpdatePinPositions();
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
    public int NumOfInputs
    {
        get => numOfInputs;
        set
        {
            if (value > 0)
            {
                numOfInputs = value;
                SetLength();
            }
        }
    }
    public int NumOfOutputs
    {
        get => numOfOutputs;
        set
        {
            if (value > 0)
            {
                numOfOutputs = value;
                SetLength();
            }
        }
    }
    public Gate(int numOfInputs, int numOfOutputs)
    {
        NumOfInputs = numOfInputs;
        NumOfOutputs = numOfOutputs;
        InitializePins();
    }

    private void SetLength()
    {
        bodyWidth = 192;
        bodyHeight = Math.Max(numOfInputs, numOfOutputs) * 48;
    }
    private void UpdatePinPositions()
    {
        if (pins.Count != NumOfInputs + NumOfOutputs)
        {
            throw new InvalidOperationException($"Pins list size does not match expected number of pins. List size = {pins.Count} Num of Inputs {NumOfInputs + NumOfInputs}");
        }
        for (int i = 0; i < NumOfInputs; i++)
        {
            Point inputPosition = new Point(Position.X - Pin.PinRadius, Position.Y + (BodyHeight / (NumOfInputs + 1)) * (i + 1) - Pin.PinRadius);
            pins[i].Position = inputPosition;
        }
        for (int i = 0; i < NumOfOutputs; i++)
        {
            Point outputPosition = new Point(Position.X + BodyWidth - Pin.PinRadius, Position.Y + (BodyHeight / (NumOfOutputs + 1)) * (i + 1) - Pin.PinRadius);
            pins[NumOfInputs + i].Position = outputPosition;
        }
    }
    public virtual void Paint(Graphics g, Rectangle bounds)
    {
        g.FillRectangle(Brushes.Black, Position.X, Position.Y, BodyWidth, BodyHeight);
        Font font = new Font("Arial", 10, FontStyle.Bold);
        Brush brush = Brushes.White;
        float labelX = Position.X + (BodyWidth / 2) - (g.MeasureString(GateName, font).Width / 2);
        float labelY = Position.Y + (BodyHeight / 2) - (g.MeasureString(GateName, font).Height / 2);
        g.DrawString(GateName.ToUpper(), font, brush, labelX, labelY);
        Color borderColor = Color.DarkGray;
        int borderWidth = 3;
        foreach (Pin pin in pins)
        {
            Brush pinBrush = pin.Power ? Brushes.Red : Brushes.Black;
            using (Pen borderPen = new Pen(borderColor, borderWidth))
            {
                g.DrawEllipse(borderPen, pin.Position.X, pin.Position.Y, 2 * Pin.PinRadius, 2 * Pin.PinRadius);
            }
            g.FillEllipse(pinBrush, pin.Position.X, pin.Position.Y, 2 * Pin.PinRadius, 2 * Pin.PinRadius);
            CalculateOutputs();
        }
    }
    private void InitializePins()
    {
        pins.Clear();
        for (int i = 0; i < NumOfInputs; i++)
        {
            Point inputPosition = new Point(Position.X - Pin.PinRadius, Position.Y + (BodyHeight / (NumOfInputs + 1)) * (i + 1) - Pin.PinRadius);
            pins.Add(new Pin(inputPosition, false, this));
        }
        for (int i = 0; i < NumOfOutputs; i++)
        {
            Point outputPosition = new Point(Position.X + BodyWidth - Pin.PinRadius, Position.Y + (BodyHeight / (NumOfOutputs + 1)) * (i + 1) - Pin.PinRadius);
            pins.Add(new Pin(outputPosition, false, this));
        }
    }
    public abstract void CalculateOutputs();
}
