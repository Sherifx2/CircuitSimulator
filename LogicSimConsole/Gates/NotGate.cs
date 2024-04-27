using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Program;

public class NotGate : Gate
{
    public NotGate(int numOfInputs, int numOfOutputs) : base(1, 1)
    {
        inputs = new List<bool>(new bool[numOfInputs]);
        outputs = new List<bool>(new bool[numOfOutputs]);
        GateName = "Not";
    }
    public override void CalculateOutputs()
    {
        for (int i = 0; i < NumOfInputs; i++)
        {
            inputs[i] = (pins[i].Power);
        }

        outputs[0] = !pins[0].Power;
        pins[pins.Count - 1].Power = outputs[0];
    }
}



