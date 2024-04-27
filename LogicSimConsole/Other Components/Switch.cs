using System;
using System.Collections.Generic;

public class Switch : Gate
{
    public Switch(int numOfInputs = 1, int numOfOutputs = 1) : base(numOfInputs, numOfOutputs)
    {
        inputs = new List<bool>(new bool[numOfInputs]);
        outputs = new List<bool>(new bool[numOfOutputs]);

        GateName = "Switch";
    }

    public override void CalculateOutputs()
    {
        outputs[0] = inputs[0];

        pins[0].Power = outputs[0];
        pins[1].Power = outputs[0];
    }
    public void ToggleSwitch()
    {

        inputs[0] = !inputs[0];
        CalculateOutputs();
    }
}
