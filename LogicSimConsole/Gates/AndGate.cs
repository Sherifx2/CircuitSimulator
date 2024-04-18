using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Program;

public class AndGate : Gate
{
    public AndGate(int numOfInputs, int numOfOutputs) : base(numOfInputs, numOfOutputs)
    {

    }

    public override void Paint(Graphics g, Rectangle bounds)
    {

        int bodyX = 96;
        int bodyY = 96;
        int bodyWidth = 192;
        int bodyHeight;

        if (NumOfInputs > NumOfInputs)
        {
            bodyHeight = 48 * NumOfInputs;
        }
        else
        {
            bodyHeight = 48 * NumOfOutputs;
        }

        g.FillRectangle(Brushes.Black, bodyX, bodyY, bodyWidth, bodyHeight);

        int circleRadius = 12;
        int inputCircleX = bounds.Left - circleRadius;
        int outputCircleX = bounds.Right + circleRadius;
        int circleYSpacing = bounds.Height / (NumOfInputs + 1);

        for (int i = 0; i < NumOfInputs; i++)
        {
            g.FillEllipse(Brushes.Red, inputCircleX, inputCircleX+circleRadius*2, 2 * circleRadius, 2 * circleRadius);
        }
        for (int i = 0; i < NumOfOutputs; i++)
        {
            g.FillEllipse(Brushes.Red, outputCircleX, outputCircleX+circleRadius*2, 2 * circleRadius, 2 * circleRadius);
        }
    }
}



