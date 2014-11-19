namespace NeuSim.Eval
{
    using Jint;
    using Jint.Native;
    using NCalc;
    using System;

    public static class Evaluator
    {
        public static Func<double, double> ToDelegate(string mathExpression)
        {
            Expression.CacheEnabled = true;

            var expressionPreparsed = new Expression(mathExpression);
            return x =>
                   {
                       expressionPreparsed.Parameters["x"] = x;
                       return (double) expressionPreparsed.Evaluate();
                   };
        }

        public static string JsEval(double value, string script)
        {
            if (string.IsNullOrWhiteSpace(script))
            {
                return value.ToString("R");
            }

            var jsEngine = new Engine().SetValue("x", value).Execute(script);
            var result = jsEngine.GetValue("transform");

            var transformExecuted = result.Invoke(new[] {new JsValue(value)});

            return transformExecuted.ToString();
        }
    }
}
