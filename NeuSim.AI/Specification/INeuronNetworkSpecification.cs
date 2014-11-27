namespace NeuSim.AI.Specification
{
    public interface INeuronNetworkSpecification
    {
        int LayerNumber { get; }

        int NeuronInLayers(int layerNo);

        int InputsInNeuron(int layerNo, int neuronNo);

        void SetWeight(int layer, int neuron, int input, double value);

        void SetBias(int layer, int neuron, int input, double value);

        INeuron[][] GetNeuronLayers();
    }

    public interface INeuron
    {
        double[] Weights();

        double Bias { get; }
    }
}
