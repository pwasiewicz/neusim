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
                       return (double)expressionPreparsed.Evaluate();
                   };
        }

        public static string CallFunction(object input, string script, string functionName)
        {
            if (script == null)
            {
                throw new ArgumentNullException("script");
            }

            if (functionName == null)
            {
                throw new ArgumentNullException("functionName");
            }

            var jsEngine = new Engine().Execute(script);
            var function = jsEngine.GetValue(functionName);

            var result = function.Invoke(new[] { JsValue.FromObject(jsEngine, input) });

            return result.ToString();
        }
    }
}
