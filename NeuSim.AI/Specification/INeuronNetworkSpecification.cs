namespace NeuSim.AI.Specification
{
    public interface INeuronNetworkSpecification
    {
        int LayerNumber { get; }

        int NeuronInLayers(int layerNo);

        int InputsInNeuron(int layerNo, int neuronNo);
    }
}
