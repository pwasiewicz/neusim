namespace NeuSim.AI
{
    using System;

    public class NeuronNetworkContext
    {
        private double learnStep;
        private int learnEpoch;

        public Func<double, double> Function { get; set; }

        public Func<double, double> Derivative { get; set; }

        public int LearnEpoch
        {
            get { return this.learnEpoch; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Learn epoch count cannot be less than 1.");
                }

                this.learnEpoch = value;
            }
        }

        public double LearnStep
        {
            get { return this.learnStep; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Learn step cannot be negative or equal 0.", "value");
                }

                this.learnStep = value;
            }
        }

        public double ErrorTolerance { get; set; }
    }
}
