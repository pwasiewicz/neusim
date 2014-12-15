namespace NeuSim.AI.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SampleXOR
    {
        [TestMethod]
        public void TestMethod1()
        {
            var neuronNetwork = new NeuronNetwork(2, 2, new NeuronNetworkContext
                                                        {
                                                            Derivative = x => x*(1-x),
                                                            Function = x => 1.0/(1.0 + Math.Exp(-x)),
                                                            LearnEpoch = 5000
                                                        });

            neuronNetwork.Train(new[] {new[] {1d, 0d}, new[] {0d, 1d}, new[] {1d, 1d}, new[] {0d, 0d}},
                                new[] {1d, 1d, 0d, 0d}, (i, i1) => { });

            var result = neuronNetwork.Process(new[] {1d, 1d});
        }
    }
}
