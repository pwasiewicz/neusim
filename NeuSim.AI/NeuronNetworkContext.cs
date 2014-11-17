namespace NeuSim.AI
{
    using System;

    public class NeuronNetworkContext
    {
        public Func<double, double> Function { get; set; }

        public Func<double, double> Derivative { get; set; }


        public static NeuronNetworkContext BuildDefault()
        {
            return new NeuronNetworkContext
                   {
                       Derivative = x => x*(x - 1),
                       Function = x => 1/(1 + Math.Exp(-x))
                   };
        }
    }
}
