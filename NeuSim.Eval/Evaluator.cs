namespace NeuSim.Eval
{
    using System;
    using NCalc;

    public static class Evaluator
    {
        public static Func<double, double> ToDelegate(string expression)
        {
            Expression.CacheEnabled = false;

            var expressionPreparsed = new Expression(expression);
            return x =>
                   {
                       expressionPreparsed.Parameters["x"] = x;
                       return (double) expressionPreparsed.Evaluate();
                   };
        }
    }
}
