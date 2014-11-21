namespace NeuSim.Services
{
    using System;

    public interface IEvaluatorService
    {
        Func<double, double> SingleVariableExpression(string mathExpression);


        string CallFunction(string script, string funcName, object input);
    }
}
