namespace NeuSim.AI.Specification
{
    using System;

    internal class NeuronNetworkSpecification  : INeuronNetworkSpecification
    {
        private readonly NeuronNetwork neuronNetwork;

        public NeuronNetworkSpecification(NeuronNetwork neuronNetwork)
        {
            this.neuronNetwork = neuronNetwork;
        }

        public int LayerNumber
        {
            get { return 0; }
        }
        public int NeuronInLayers(int layerNo)
        {
            return 0;
        }

        public int InputsInNeuron(int layerNo, int neuronNo)
        {
            return 0;
        }
    }
}
