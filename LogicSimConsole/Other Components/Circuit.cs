using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Circuit : Gate
{
    private List<Gate> gates;
    private List<Circuit> circuits;
    private List<Pin> inputs;
    private List<Pin> outputs;

    public Circuit(int numOfInputs, int numOfOutputs) : base(numOfInputs, numOfOutputs)
    {
        Gates = gates;
        Circuits = circuits;
        Inputs = inputs;
        Outputs = outputs;
    }

    public List<Gate> Gates
    {
        get => gates;
        set
        {
            gates = value;
        }
    }
    public List<Circuit> Circuits
    {
        get => circuits;
        set
        {
            circuits = value;
        }
    }
    public List<Pin> Inputs
    {
        get => inputs;
        set
        {
            inputs = value;
        }
    }
    public List<Pin> Outputs
    {
        get => outputs;
        set
        {
            outputs = value;
        }
    }

    public void AddGate(Gate gate)
    {
        Gates.Add(gate);
    }

    public void AddCircuit(Circuit circuit)
    {
        Circuits.Add(circuit);
    }

    public override void CalculateOutputs()
    {
        foreach (Gate gate in Gates)
        {
            gate.CalculateOutputs();
        }

        foreach (Circuit circuit in Circuits)
        {
            circuit.CalculateOutputs();
        }

    }
}
