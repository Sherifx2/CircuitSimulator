using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static Program;

public class OrGate : Gate
{
    public OrGate(int numOfInputs, int numOfOutputs) : base(numOfInputs, numOfOutputs)
    {
        GateName = "Or";
        inputs = new List<bool>(new bool[numOfInputs]);
        outputs = new List<bool>(new bool[numOfOutputs]);
    }
    public override void CalculateOutputs()
    {
        for (int i = 0; i < NumOfInputs; i++)
        {
            inputs[i] = (pins[i].Power);
        }

        outputs[0] = inputs.Any(input => input == true);
        pins[pins.Count - 1].Power = outputs[0];
    }
}



