using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LogicGateSim.AbstractClasses
{
    public abstract class ElectricalComponent
    {
        protected Rectangle electricalComponentGUI = new Rectangle();
        protected int numOfInputs;
        protected int numOfOutputs;
        public ElectricalComponent(int numOfInputs, int numOfOutputs)
        {
            NumOfInputs = numOfInputs;
            NumOfOutputs = numOfOutputs;
            SetSize();
        }

        public int NumOfInputs
        {
            get => numOfInputs;
            set
            {
                numOfInputs = value;
                SetSize();
            }
        }
        public int NumOfOutputs
        {
            get => numOfOutputs;
            set
            {
                numOfOutputs = value;
                SetSize();
            }
        }
        protected virtual void SetSize()
        {
            electricalComponentGUI.Height = 12* numOfInputs;
            electricalComponentGUI.Width = 24;
        }
    }
}
