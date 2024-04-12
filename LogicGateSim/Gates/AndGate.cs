using LogicGateSim.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogicGateSim.Gates
{
    public class AndGate : ElectricalComponent
    {
        private bool[] input;
        private bool output;
        public AndGate(int numOfInputs, int numOfOutputs) : base(numOfInputs, numOfOutputs)
        {
            input = new bool[numOfInputs]; 
            output = false; 
        }
        public bool[] Input
        {
            get=> input;
            set => input = value;
        }
        
    }
}
