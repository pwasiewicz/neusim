namespace NeuSim.Services.Implementations
{
    using Eval;
    using System;

    internal class EvaluatorService : IEvaluatorService
    {
        public Func<double, double> SingleVariableExpression(string mathExpression)
        {
            return Evaluator.ToDelegate(mathExpression);
        }

        public string CallFunction(string script, string funcName, object input)
        {
            return Evaluator.CallFunction(input, script, "aggregate");
        }
    }
}
