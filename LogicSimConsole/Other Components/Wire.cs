using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

public class Wire
{
    private bool powered;
    private Pin sourcePin;
    private Pin destinationPin;

    public Wire(bool powered, Pin sourcePin, Pin destinationPin)
    {
        Powered = powered;
        SourcePin = sourcePin;
        DestinationPin = destinationPin;
    }
    public bool Powered
    {
        get => powered;
        set
        {
            powered = value;
        }
    }

    public Pin SourcePin
    {
        get => sourcePin;
        set
        {
            sourcePin = value;
        }
    }

    public Pin DestinationPin
    {
        get => destinationPin;
        set
        {
            destinationPin = value;
        }
    }
    
    public void Draw(Graphics g)
    {
        Powered = SourcePin.Power;
        DestinationPin.Power = Powered;

        Pen wirePen = new Pen(Powered ? Color.Red : Color.Black, 2);
        g.DrawLine(wirePen,
                new Point(SourcePin.Position.X + Pin.PinRadius, SourcePin.Position.Y + Pin.PinRadius),
                new Point(DestinationPin.Position.X + Pin.PinRadius, DestinationPin.Position.Y + Pin.PinRadius));
    }

}
