namespace NeuSim.AI
{
    using Specification;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Soap;

    [Serializable]
    public class NeuronNetwork
    {
        private const int Version = 1;

        [NonSerialized]
        private NeuronNetworkContext networkCtx;

        private readonly int inputsNo;

        private readonly int hiddenNeuronsNo;

        private readonly Neuron[] hiddenNeurons;

        private readonly Neuron outputNeuron;

        public NeuronNetwork(int inputsNo, int hiddenNeuronsNo, NeuronNetworkContext context)
        {
            if (inputsNo <= 0)
            {
                throw new ArgumentOutOfRangeException("inputsNo", inputsNo, "The value should be greater than 0");
            }

            if (hiddenNeuronsNo <= 0)
            {
                throw new ArgumentOutOfRangeException("hiddenNeuronsNo", inputsNo, "The value should be greater than 0");
            }

            this.inputsNo = inputsNo;
            this.hiddenNeuronsNo = hiddenNeuronsNo;
            this.hiddenNeurons = new Neuron[hiddenNeuronsNo];
            this.outputNeuron = new Neuron(hiddenNeuronsNo);

            for (var i = 0; i < this.hiddenNeuronsNo; i++)
            {
                this.hiddenNeurons[i] = new Neuron(inputsNo);
            }

            this.SetContext(context);
        }

        public int LearnEpoch
        {
            get { return this.networkCtx.LearnEpoch; }
        }

        public int InputNo
        {
            get { return this.inputsNo; }
        }

        internal int HiddenNeurons
        {
            get { return this.hiddenNeuronsNo; }
        }

        public static void Save(NeuronNetwork network, Stream writer)
        {
            var formatter = new SoapFormatter();

            formatter.Serialize(writer, Version);
            formatter.Serialize(writer, network);
        }

        public static NeuronNetwork Load(Stream stream, NeuronNetworkContext context)
        {
            var formatter = new SoapFormatter();
            var version = (int)formatter.Deserialize(stream);
            if (version != Version)
            {
                throw new InvalidOperationException("Invalid version.");
            }

            var network = (NeuronNetwork)formatter.Deserialize(stream);
            network.SetContext(context);

            return network;
        }

        public double Process(double[] inputs)
        {
            if (inputs == null)
            {
                throw new ArgumentNullException("inputs");
            }

            return this.PropagateInput(inputs).GetOutput();
        }

        public void Train(double[][] inputs, double[] results)
        {
            if (inputs == null)
            {
                throw new ArgumentNullException("inputs");
            }

            if (results == null)
            {
                throw new ArgumentNullException("results");
            }

            if (inputs.Length != results.Length)
            {
                throw new ArgumentException("Inputs length is not equal to result length.");
            }

            var epoch = 0;

            while (epoch < this.LearnEpoch)
            {
                epoch += 1;
                for (var i = 0; i < inputs.Length; i++)
                {
                    var currentVector = inputs[i];
                    this.PropagateInput(currentVector);

                    var outputNeuronValue = this.outputNeuron.GetOutput();

                    this.outputNeuron.LastError = this.networkCtx.Derivative(outputNeuronValue) *
                                                  (results[i] - outputNeuronValue);
                    if (Math.Abs(outputNeuron.LastError) < this.networkCtx.ErrorTolerance)
                    {
                        continue;
                    }

                    this.outputNeuron.NormalizeWeights();

                    for (var j = 0; j < this.hiddenNeuronsNo; j++)
                    {
                        this.hiddenNeurons[j].LastError =
                            this.networkCtx.Derivative(this.hiddenNeurons[j].GetOutput()) * this.outputNeuron.LastError *
                            this.outputNeuron.Weights[j];

                        this.hiddenNeurons[j].NormalizeWeights();
                    }
                }
            }
        }

        public INeuronNetworkSpecification Specification()
        {
            return new NeuronNetworkSpecification(this);
        }

        internal void SetContext(NeuronNetworkContext context)
        {
            this.networkCtx = context;

            this.outputNeuron.NetworkContext = context;
            for (var i = 0; i < this.hiddenNeuronsNo; i++)
            {
                this.hiddenNeurons[i].NetworkContext = context;
            }
        }

        internal IList<IList<Neuron>> GetNeronInLayers()
        {
            var output = new List<IList<Neuron>>();

            var firstLayer = new List<Neuron>();
            for (var i = 0; i < this.inputsNo; i++)
            {
                firstLayer.Add(new Neuron(1) { NetworkContext = networkCtx });
            }

            output.Add(firstLayer);

            output.Add(this.hiddenNeurons);
            output.Add(new[] { this.outputNeuron });

            return output;
        }

        private Neuron PropagateInput(double[] inputs)
        {
            if (inputs == null)
            {
                throw new ArgumentNullException("inputs");
            }

            for (var i = 0; i < this.hiddenNeuronsNo; i++)
            {
                this.hiddenNeurons[i].Inputs = inputs;
            }

            this.outputNeuron.Inputs = this.hiddenNeurons.Select(hiddenNeuron => hiddenNeuron.GetOutput()).ToArray();
            return this.outputNeuron;
        }        
    }
}
