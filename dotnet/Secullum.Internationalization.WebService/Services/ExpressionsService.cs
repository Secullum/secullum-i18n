using Microsoft.EntityFrameworkCore;
using Secullum.Internationalization.WebService.Data;
using Secullum.Internationalization.WebService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Secullum.Internationalization.WebService.Services
{
    public class ExpressionsService
    {
        private readonly SecullumInternationalizationWebServiceContext m_secullumInternationalizationWebServiceContext;
        private readonly TranslationService _translationService = new TranslationService();

        public ExpressionsService(SecullumInternationalizationWebServiceContext secullumInternationalizationWebServiceContext)
        {
            m_secullumInternationalizationWebServiceContext = secullumInternationalizationWebServiceContext;
        }

        public async Task<Dictionary<string, Dictionary<string, string>>> GenerateAsync(GenerateParameters parameters)
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

            var completeExpressionsDictionary = new Dictionary<string, Dictionary<string, string>>();

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
                    if (parameters.TranslateDatabase)
                    {
                        var update = false;

                        if (string.IsNullOrWhiteSpace(expressionRecord.English))
                        {
                            expressionRecord.English = await _translationService.TranslateAsync(expression, "en");
                            update = true;
                        }

                        if (string.IsNullOrWhiteSpace(expressionRecord.Spanish))
                        {
                            expressionRecord.Spanish = await _translationService.TranslateAsync(expression, "es");
                            update = true;
                        }

                        if (update)
                        {
                            var existingExpression = await m_secullumInternationalizationWebServiceContext.Expressions
                                .FirstOrDefaultAsync(e => e.Portuguese == expressionRecord.Portuguese);

                            existingExpression.English = expressionRecord.English;
                            existingExpression.Spanish = expressionRecord.Spanish;
                            existingExpression.DateChanged = DateTime.Now;

                            m_secullumInternationalizationWebServiceContext.Expressions.Update(existingExpression);

                            await m_secullumInternationalizationWebServiceContext.SaveChangesAsync();
                        }
                    }

                    portgueseExpressions.Add(expression, expressionRecord.Portuguese);
                    englishExpressions.Add(expression, expressionRecord.English);
                    spanishExpressions.Add(expression, expressionRecord.Spanish);
                }
                else
                {
                    var translatedEnglish = await _translationService.TranslateAsync(expression, "en");
                    var translatedSpanish = await _translationService.TranslateAsync(expression, "es");

                    m_secullumInternationalizationWebServiceContext.Expressions.Add(new Expression()
                    {
                        Portuguese = expression,
                        English = translatedEnglish,
                        Spanish = translatedSpanish,
                        DateCreated = DateTime.Now,
                    });

                    await m_secullumInternationalizationWebServiceContext.SaveChangesAsync();

                    portgueseExpressions.Add(expression, expression);
                    englishExpressions.Add(expression, translatedEnglish);
                    spanishExpressions.Add(expression, translatedSpanish);
                    newExpressions.Add($"pt: {expression}", $"en: {translatedEnglish} || es: {translatedSpanish}");
                }
            }

            completeExpressionsDictionary.Add("pt", portgueseExpressions);
            completeExpressionsDictionary.Add("en", englishExpressions);
            completeExpressionsDictionary.Add("es", spanishExpressions);
            completeExpressionsDictionary.Add("newExpressions", newExpressions);

            return completeExpressionsDictionary;
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
            public bool TranslateDatabase { get; set; }
        }

        public class GenerateException : Exception
        {
            public GenerateException(string message) : base(message)
            {
            }
        }

        private class ExpressionRecord
        {
            public string Portuguese { get; set; }
            public string English { get; set; }
            public string Spanish { get; set; }
        }
    }
}
