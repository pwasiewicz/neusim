namespace NeuSim.AI.Specification
{
    using System.Collections.Generic;
    using System.Linq;

    internal class NeuronNetworkSpecification  : INeuronNetworkSpecification
    {
        private readonly NeuronNetwork neuronNetwork;

        private IList<IList<Neuron>> neuronLayers;

        public NeuronNetworkSpecification(NeuronNetwork neuronNetwork)
        {
            this.neuronNetwork = neuronNetwork;
        }

        public int LayerNumber
        {
            get { return this.NeuronLayers.Count; }
        }
        public int NeuronInLayers(int layerNo)
        {
            return this.NeuronLayers[layerNo].Count;
        }

        public int InputsInNeuron(int layerNo, int neuronNo)
        {
            return this.NeuronLayers[layerNo][neuronNo].Inputs.Length;
        }

        public void SetWeight(int layer, int neuron, int input, double value)
        {
            this.NeuronLayers[layer][neuron][input] = value;
        }

        public void SetBias(int layer, int neuron, int input, double value)
        {
            this.NeuronLayers[layer][neuron].Bias = value;
        }

        public INeuron[][] GetNeuronLayers()
        {
            return this.NeuronLayers.Select(neuronLayer => neuronLayer.Cast<INeuron>().ToArray()).ToArray();
        }

        private IList<IList<Neuron>> NeuronLayers
        {
            get { return this.neuronLayers ?? (this.neuronLayers = this.neuronNetwork.GetNeronInLayers()); }
        }
    }


}
