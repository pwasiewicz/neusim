﻿namespace NeuSim.AI
{
    using System;
    using System.Linq;

    [Serializable]
    internal class Neuron 
    {
        private static readonly Lazy<Random> RandomHolder = new Lazy<Random>(() => new Random());

        private readonly double[] inputs;
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

        public double[] Inputs
        {
            get { return this.inputs; }
            set { value.CopyTo(this.inputs, 0); }
        }

        public double GetOutput()
        {
            var sum = this.inputs.Select((t, i) => t*this.weights[i]).Sum() + this.bias;
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
                this.weights[i] += this.LastError * this.inputs[i];
            }

            this.bias += this.LastError;
        }

        //public static double Activation(double x)
        //{
        //    return 1.0 / (1.0 + Math.Exp(-x));
        //}

        //public static double ActivationDerivative(double x)
        //{
        //    return x * (1 - x);
        //}
    }
}