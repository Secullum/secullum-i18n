using Microsoft.EntityFrameworkCore;
using Secullum.Internationalization.WebService.Data;
using Secullum.Internationalization.WebService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Secullum.Internationalization.WebService.Services
{
    public class TranslatorService
    {
        private readonly SecullumInternationalizationWebServiceContext m_secullumInternationalizationWebServiceContext;

        public TranslatorService(SecullumInternationalizationWebServiceContext secullumInternationalizationWebServiceContext)
        {
            m_secullumInternationalizationWebServiceContext = secullumInternationalizationWebServiceContext;
        }

        public async Task<Dictionary<string, Dictionary<string, string>>> TranslatedExpressionsReturnAsync(ParametersExpressionsList parameter)
        {
            var completeListExpressionsDataBase = await m_secullumInternationalizationWebServiceContext.Expressions
                .Select(x => new ExpressionSelectList()
                {
                    Portuguese = x.Portuguese,
                    English = x.English,
                    Spanish = x.Spanish
                })
                .ToDictionaryAsync(x => x.Portuguese.ToUpper(), x => x);

            var portgueseExpressions = new Dictionary<string, string>();
            var englishExpressions = new Dictionary<string, string>();
            var spanishExpressions = new Dictionary<string, string>();

            var completeExpressionsDictionary = new Dictionary<string, Dictionary<string, string>>();

            foreach (var expression in parameter.Expressions)
            {
                var searchExpression = expression.ToUpper();
                ExpressionSelectList expressionRecord = null;

                if (completeListExpressionsDataBase.ContainsKey(searchExpression))
                {
                    expressionRecord = completeListExpressionsDataBase[searchExpression];
                }
                else
                {
                    // Foi necessária uma segunda verificação, pois algumas expressões não estão sendo filtradas por causa do espaço no final.
                    // Fazendo a verificação separada, gerou ganho de desempenho do WebService.
                    expressionRecord = completeListExpressionsDataBase
                        .Where(x => x.Value.Portuguese.ToUpper().Trim() == searchExpression.Trim())
                        .Select(x => x.Value)
                        .FirstOrDefault();
                }

                if (expressionRecord != null)
                {
                    portgueseExpressions.Add(expression, expressionRecord.Portuguese);
                    englishExpressions.Add(expression, expressionRecord.English);
                    spanishExpressions.Add(expression, expressionRecord.Spanish);
                }
                else
                {
                    m_secullumInternationalizationWebServiceContext.Expressions.Add(new Expression()
                    {
                        Portuguese = expression,
                        DateCreate = DateTime.Now
                    });

                    await m_secullumInternationalizationWebServiceContext.SaveChangesAsync();

                    portgueseExpressions.Add(expression, expression);
                    englishExpressions.Add(expression, null);
                    spanishExpressions.Add(expression, null);
                }
            }

            completeExpressionsDictionary.Add("pt", portgueseExpressions);
            completeExpressionsDictionary.Add("en", englishExpressions);
            completeExpressionsDictionary.Add("es", spanishExpressions);

            return completeExpressionsDictionary;
        }

        public string ErrorStringReturn(ParametersExpressionsList parametro)
        {
            var expressoesRepetidas = CheckRepeatedExpressionsInParametersList(parametro);

            if (expressoesRepetidas.Count == 0)
            {
                return null;
            }

            return $"There are repeated expressions in the parameter list: {string.Join("; ", expressoesRepetidas)}.";
        }

        private List<string> CheckRepeatedExpressionsInParametersList(ParametersExpressionsList parameters)
        {
            return parameters.Expressions
                .GroupBy(x => x)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();
        }

        public class ParametersExpressionsList
        {
            public List<string> Expressions { get; set; }
        }

        private class ExpressionSelectList
        {
            public string Portuguese { get; set; }
            public string English { get; set; }
            public string Spanish { get; set; }
        }
    }
}
