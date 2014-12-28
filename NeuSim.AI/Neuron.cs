namespace NeuSim.AI
{
    using System;
    using System.Linq;
    using NeuSim.AI.Specification;

    [Serializable]
    internal class Neuron : INeuron
    {
        private static readonly Lazy<Random> RandomHolder = new Lazy<Random>(() => new Random());

        private double[] inputs;
        private readonly double[] weights;
        private double bias;

        [NonSerialized]
        internal NeuronNetworkContext NetworkContext;

        public Neuron(int inputsNo)
        {
            this.inputs = new double[inputsNo];
            this.weights = new double[inputsNo];

            this.Randomize();
        }

        public double LastError { get; set; }

        public double[] Weights
        {
            get { return this.weights; }
        }

        public double Bias
        {
            get
            {
                return this.bias;
            }

            set { this.bias = value; }
        }

        public double this[int index]
        {
            get { return this.Inputs[index]; }
            set { this.Inputs[index] = value; }
        }

        public double[] Inputs
        {
            get { return this.inputs; }
            set { this.inputs = value; }
        }

        public double GetOutput()
        {
            var sum = this.inputs.Select((t, i) => t * this.weights[i]).Sum() + this.bias;
            return this.NetworkContext.Function(sum);
        }

        public void Randomize()
        {
            for (var i = 0; i < this.weights.Length; i++)
            {
                this.weights[i] = RandomHolder.Value.NextDouble();
            }

            this.bias = RandomHolder.Value.NextDouble();
        }

        public void NormalizeWeights()
        {
            for (var i = 0; i < this.weights.Length; i++)
            {
                this.weights[i] += this.LastError*this.inputs[i] * this.NetworkContext.LearnStep;
            }

            this.bias += this.LastError;
        }

        #region INeuron

        double[] INeuron.Weights()
        {
            return (double[])this.Weights.Clone();
        }

        double INeuron.Bias
        {
            get { return this.Bias; }
        }

        #endregion
    }
}
