﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Program;

public class AndGate : Gate
{
    public AndGate(int numOfInputs, int numOfOutputs) : base(numOfInputs, 1)
    {
        inputs = new List<bool>(new bool[numOfInputs]);
        outputs = new List<bool>(new bool[numOfOutputs]);
        GateName = "And";
    }
    public override void CalculateOutputs()
    {
        for(int i = 0; i < NumOfInputs; i++) {
            inputs[i]=(pins[i].Power);
        }
        outputs[0] = inputs.All(input => input == true);
        pins[pins.Count-1].Power = outputs[0];
    }
}