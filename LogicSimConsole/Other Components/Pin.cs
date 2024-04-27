using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Pin
{
    public static readonly int PinRadius = 12;
    private Point position;
    private bool power;
    private Gate parentGate;
    private Wire inputWire;
    private Wire outputWire;

    public Wire InputWire
    {
        get => inputWire;
        set
        {
            inputWire = value;
        }
    }
    public Wire OutputWire
    {
        get => outputWire;
            set
        {
            outputWire = value;
        }
    }
    public Pin(Point position, bool power, Gate parentGate)
    {
        Position = position;
        Power = power;
        ParentGate = parentGate;
    }
    public Point Position
    {
        get => position;
        set
        {
            position = value;
        }
    }
    public bool Power
    {
        get => power;
        set => power = value;
    }
    public Gate ParentGate
    {
        get => parentGate;
        set
        {
            parentGate = value;
        }
    }
}
