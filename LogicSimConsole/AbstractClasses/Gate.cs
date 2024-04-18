using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Gate
{
    private int numOfInputs;
    private int numOfOutputs;

    protected List<bool> inputs;
    protected List<bool> outputs;

    public int NumOfInputs
    {
        get => numOfInputs;
        set
        {
            if (value > 0)
            {
                numOfInputs = value;
                InitializeInputs();
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
    public abstract void Paint(Graphics g, Rectangle bounds);
}
