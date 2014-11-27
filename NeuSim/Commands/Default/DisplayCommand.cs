namespace NeuSim.Commands.Default
{
    using System.IO;
    using Arguments;
    using Context;

    internal class DisplayCommand : CommandBase<DisplaySubOptions>
    {
        public DisplayCommand(SessionContext sessionContext) : base(sessionContext) { }

        public override string Name
        {
            get { return "display"; }
        }

        public override bool AllowNotInitialized
        {
            get { return false; }
        }

        public TextWriter Output
        {
            get { return this.SessionContext.Output; }
        }

        public override void Run(DisplaySubOptions options)
        {
            var spec = this.SessionContext.NeuronNetwork.Specification();
            var layers = spec.GetNeuronLayers();

            this.Output.WriteLine();

            for (var i = 1; i <= layers.Length; i++)
            {
                var layerIndex = i - 1;
                this.Output.WriteLine("\t{0} layer:", i);

                if (i == 1)
                {
                    for (var j = 0; j < layers[layerIndex].Length; j++)
                    {
                        this.Output.WriteLine("\t\tInput neruon (no weight or bias)");
                    }
                }
                else
                {
                    for (var j = 0; j < layers[layerIndex].Length; j++)
                    {
                        this.Output.WriteLine("\t\t{0} neuron:", j + 1);
                        this.Output.WriteLine("\t\t\tbias: {0}", layers[layerIndex][j].Bias);

                        var weights = layers[layerIndex][j].Weights();
                        for (var k = 0; k < weights.Length; k++)
                        {
                            this.Output.WriteLine("\t\t\t{0} input weight: {1}", k + 1, weights[k]);
                        }
                    }
                }

            }
        }


    }
}
