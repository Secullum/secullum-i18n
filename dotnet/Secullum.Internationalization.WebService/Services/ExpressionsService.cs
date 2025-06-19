using Microsoft.EntityFrameworkCore;
using Secullum.Internationalization.WebService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Secullum.Internationalization.WebService.Services
{
    public class ExpressionsService
    {
        private readonly SecullumInternationalizationWebServiceContext m_secullumInternationalizationWebServiceContext;
        private readonly TranslationService m_translationService;

        public ExpressionsService(SecullumInternationalizationWebServiceContext seci18nWebServiceContext, TranslationService translationService)
        {
            m_secullumInternationalizationWebServiceContext = seci18nWebServiceContext;
            m_translationService = translationService;
        }

        public async Task<ExpressionsResponse> GenerateAsync(GenerateParameters parameters)
        {
            CheckRepeatedExpressions(parameters);

            var expressionsFromDatabase = await m_secullumInternationalizationWebServiceContext.Expressions
                .Select(x => new ExpressionRecord()
                {
                    Portuguese = x.Portuguese,
                    English = x.English,
                    Spanish = x.Spanish
                })
                .ToDictionaryAsync(x => x.Portuguese.ToUpper(), x => x, StringComparer.OrdinalIgnoreCase);

            var portgueseExpressions = new Dictionary<string, string>();
            var englishExpressions = new Dictionary<string, string>();
            var spanishExpressions = new Dictionary<string, string>();
            var newExpressions = new Dictionary<string, string>();

            foreach (var expression in parameters.Expressions)
            {
                ExpressionRecord expressionRecord = null;

                if (expressionsFromDatabase.ContainsKey(expression))
                {
                    expressionRecord = expressionsFromDatabase[expression];
                }
                else
                {
                    // Foi necessária uma segunda verificação, pois algumas expressões não estão sendo filtradas por causa do espaço no final.
                    // Fazendo a verificação separada, não penalizamos a maioria dos casos que continuam usando um Dictionary, que tem acesso mais rápido.
                    expressionRecord = expressionsFromDatabase
                        .Where(x => x.Value.Portuguese.ToUpper().Trim() == expression.ToUpper().Trim())
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
                    expressionRecord = new ExpressionRecord
                    {
                        Portuguese = expression
                    };

                    expressionRecord = await m_translationService.TranslateExpression(expressionRecord);

                    portgueseExpressions.Add(expression, expression);
                    englishExpressions.Add(expression, expressionRecord.English);
                    spanishExpressions.Add(expression, expressionRecord.Spanish);
                    newExpressions.Add($"pt: {expression}", $"en: {expressionRecord.English} || es: {expressionRecord.Spanish}");
                }
            }

            await m_secullumInternationalizationWebServiceContext.SaveChangesAsync();

            return new ExpressionsResponse
            {
                Languages = new Dictionary<string, Dictionary<string, string>>
                {
                    { "pt", portgueseExpressions },
                    { "en", englishExpressions },
                    { "es", spanishExpressions }
                },
                NewExpressions = newExpressions
            };
        }

        public async Task<List<ExpressionRecord>> TranslateAllExpressionsAsync()
        {
            var expressionsFromDatabase = await m_secullumInternationalizationWebServiceContext.Expressions
                .Select(x => new ExpressionRecord
                {
                    Portuguese = x.Portuguese,
                    English = x.English,
                    Spanish = x.Spanish
                })
                .ToDictionaryAsync(x => x.Portuguese.ToUpper(), x => x, StringComparer.OrdinalIgnoreCase);

            var expressionsToTranslate = expressionsFromDatabase
                .Where(x => string.IsNullOrWhiteSpace(x.Value.English) || string.IsNullOrWhiteSpace(x.Value.Spanish))
                .Select(x => x.Value)
                .ToList();

            var translatedExpressions = new List<ExpressionRecord>();

            foreach (var expression in expressionsToTranslate)
            {
                await m_translationService.TranslateExpression(expression);

                translatedExpressions.Add(expression);
            }

            await m_secullumInternationalizationWebServiceContext.SaveChangesAsync();

            return translatedExpressions;
        }

        private void CheckRepeatedExpressions(GenerateParameters parameters)
        {
            var repeatedExpressions = parameters.Expressions
                .GroupBy(x => x)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (repeatedExpressions.Any())
            {
                throw new GenerateException($"There are repeated expressions in the parameter list: {string.Join("; ", repeatedExpressions)}.");
            }
        }

        public class GenerateParameters
        {
            public List<string> Expressions { get; set; }
        }

        public class ExpressionsResponse
        {
            public Dictionary<string, Dictionary<string, string>> Languages { get; set; }
            public Dictionary<string, string> NewExpressions { get; set; }
        }

        public class GenerateException : Exception
        {
            public GenerateException(string message) : base(message)
            {
            }
        }

        public class ExpressionRecord
        {
            public string Portuguese { get; set; }
            public string English { get; set; }
            public string Spanish { get; set; }
        }
    }
}
