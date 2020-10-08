using System;
using System.Collections.Generic;
using System.Linq;
using CrowdControl.Common.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CrowdControl.Common
{
    [UsedImplicitly]
    public static class FormulaBank
    {
        private static readonly Dictionary<Formulas, Func<IEnumerable<long>, long>> _formulas =
            new Dictionary<Formulas, Func<IEnumerable<long>, long>>
            {
                {Formulas.Sum, Enumerable.Sum},
                {Formulas.Product, a => a.Aggregate((v1, v2) => v1 * v2)},
                {
                    Formulas.BaseAddMultipliedPairs, a =>
                    {
                        long[] par = a.Skip(1).ToArray();
                        long total = 0;
                        for (int i = 0; i < par.Length; i += 2) { total += par[i] * par[i + 1]; }
                        return a.First() + total;
                    }
                },
                {
                    Formulas.ArcLevel, a =>
                    {
                        long[] par = a.Skip(1).ToArray();
                        double total = 0;
                        for (int i = 0; i < par.Length; i += 2)
                        {
                            total += (par[i] * Math.Log(par[i + 1], 1.3584562741829884));
                        }
                        return (long) Math.Ceiling(a.First() + total);
                    }
                },
                {Formulas.First, Enumerable.First},
                {Formulas.Last, Enumerable.Last}
            };

        [UsedImplicitly]
        public static long ApplyFormula([NotNull] EffectRequest request, Formulas formula)
            => ApplyFormula(request.AllItems, formula);

        [UsedImplicitly]
        public static long ApplyFormula([NotNull] IEnumerable<IFormulaVariable> items, Formulas formula)
            => _formulas[formula](items.Select(p => p.Reduce()));

        [UsedImplicitly]
        public enum Formulas : byte
        {
            Sum = 0,
            Product = 1,
            BaseAddMultipliedPairs = 2,
            ArcLevel = 3,
            First = 4,
            Last = 5,
            Expression = 255
        }

        [Serializable]
        public class FormulaBox : IFormulaVariable
        {
            [JsonProperty(PropertyName = "value")]
            public readonly long Value;

            [JsonProperty(PropertyName = "formulaVariableType")]
            public FormulaVariableType FormulaVariableType => FormulaVariableType.FormulaBox;

            [JsonConstructor]
            public FormulaBox(long value) => Value = value;

            public long Reduce() => Convert.ToInt64(Value);

            [JsonIgnore]
            public string FinalCode => Value.ToString();
        }

        [UsedImplicitly]
        public static long Reduce(this List<IFormulaVariable> vars, Formulas formula)
            => ApplyFormula(vars, formula);

        [UsedImplicitly]
        public static long Reduce(this List<IFormulaVariable> vars)
            => ApplyFormula(vars, ((IItem)vars[0]).Formula);

        private static readonly Dictionary<string, Func<double[], double>> FUNCTIONS = new Dictionary<string, Func<double[], double>>
        {
            {"+", a => a.Sum()},
            {"-", a => a.Aggregate((x, y) => x - y)},
            {"*", a => a.Aggregate((x, y) => x * y)},
            {"/", a => a.Aggregate((x, y) => x / y)},
            {"%", a => a.Aggregate((x, y) => x % y)},
            {"^", a => a.Aggregate(Math.Pow)},

            {"ln", a => Math.Log(a.First())},
            {"log10", a => Math.Log10(a.First())}
        };

        [UsedImplicitly]
        public static long Evaluate(Dictionary<string, double> env, string formula) => (long) Math.Floor(Evaluate(env, JToken.Parse(formula)));

        [UsedImplicitly]
        private static double Evaluate(Dictionary<string, double> env, JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Array:
                    var values = token.Values();
                    return FUNCTIONS[values.First().Value<string>()](values.Skip(1).Select(t => Evaluate(env, t)).ToArray());
                case JTokenType.Integer:
                    return token.Value<long>();
                case JTokenType.Float:
                    return token.Value<double>();
                case JTokenType.String:
                    return env[token.Value<string>()];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
