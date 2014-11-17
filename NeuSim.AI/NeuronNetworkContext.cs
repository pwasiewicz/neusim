namespace NeuSim.AI
{
    using System;

    public class NeuronNetworkContext
    {
        public Func<double, double> Function { get; set; }

        public Func<double, double> Derivative { get; set; }
    }
}
