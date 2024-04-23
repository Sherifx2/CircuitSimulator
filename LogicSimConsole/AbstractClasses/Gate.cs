using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Gate
{
    private int xPosition;
    private int yPosition;
    private int numOfInputs;
    private int numOfOutputs;
    private int bodyWidth;
    private int bodyHeight;

    protected List<bool> inputs;
    protected List<bool> outputs;
    
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

    public int XPosition
    {
        get => xPosition;
        set
        {
            if (value.GetType() == typeof(int))
            {
                xPosition = value;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
    public int YPosition
    {
        get => yPosition;
        set
        {
            if (value.GetType() == typeof(int))
            {
                yPosition = value;
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
                InitializeInputs();
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
                InitializeOutputs();
                SetLength();
            }
        }
    }
    public Gate(int numOfInputs, int numOfOutputs)
    {
        NumOfInputs = numOfInputs;
        NumOfOutputs = numOfOutputs;
    }
    private void InitializeInputs()
    {
        inputs = new List<bool>(numOfInputs);
        for (int i = 0; i < numOfInputs; i++)
        {
            inputs.Add(false);
        }
    }
    private void InitializeOutputs()
    {
        outputs = new List<bool>(numOfOutputs);
        for (int i = 0; i < numOfOutputs; i++)
        {
            outputs.Add(false);
        }
    }
    private void SetLength()
    {
        bodyWidth = 192;
        bodyHeight = Math.Max(numOfInputs, numOfOutputs) * 48;
    }
    public void Paint(Graphics g, Rectangle bounds)
    {
        g.FillRectangle(Brushes.Black, XPosition, YPosition, BodyWidth, BodyHeight);

        int circleRadius = 12;

        for (int i = 0; i < NumOfInputs; i++)
        {
            g.FillEllipse(Brushes.Red, XPosition - circleRadius, YPosition + (BodyHeight / (NumOfInputs + 1)) * (i + 1) - circleRadius, 2 * circleRadius, 2 * circleRadius);
        }
        for (int i = 0; i < NumOfOutputs; i++)
        {
            g.FillEllipse(Brushes.Red, XPosition + BodyWidth - circleRadius, YPosition + (BodyHeight / (NumOfOutputs + 1)) * (i + 1) - circleRadius, 2 * circleRadius, 2 * circleRadius);
        }
    }
}
