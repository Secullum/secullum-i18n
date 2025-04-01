using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Secullum.Internationalization.WebService.Data;
using Secullum.Internationalization.WebService.Model;
using static Secullum.Internationalization.WebService.Services.ExpressionsService;

namespace Secullum.Internationalization.WebService.Services
{
    public class TranslationService
    {
        private readonly SecullumInternationalizationWebServiceContext m_secullumInternationalizationWebServiceContext;
        private readonly TranslationRequestService m_translationRequestService;

        public TranslationService(SecullumInternationalizationWebServiceContext seci18nWebServiceContext, IOptions<TranslatorSettings> settings)
        {
            m_secullumInternationalizationWebServiceContext = seci18nWebServiceContext;
            m_translationRequestService = new TranslationRequestService(settings);
        }

        public async Task<ExpressionRecord> TranslateExpression(ExpressionRecord expressionRecord)
        {
            if (string.IsNullOrWhiteSpace(expressionRecord.English))
            {
                expressionRecord.English = await m_translationRequestService.TranslateAsync(expressionRecord.Portuguese, "en");
            }

            if (string.IsNullOrWhiteSpace(expressionRecord.Spanish))
            {
                expressionRecord.Spanish = await m_translationRequestService.TranslateAsync(expressionRecord.Portuguese, "es");
            }

            var existingExpression = await m_secullumInternationalizationWebServiceContext.Expressions
                .FirstOrDefaultAsync(e => e.Portuguese == expressionRecord.Portuguese);

            if (existingExpression == null)
            {
                existingExpression = new Expression()
                {
                    Portuguese = expressionRecord.Portuguese,
                    English = expressionRecord.English,
                    Spanish = expressionRecord.Spanish,
                    DateCreated = DateTime.Now
                };

                await m_secullumInternationalizationWebServiceContext.Expressions.AddAsync(existingExpression);
            }
            else
            {
                existingExpression.English = expressionRecord.English;
                existingExpression.Spanish = expressionRecord.Spanish;
                existingExpression.DateChanged = DateTime.Now;

                m_secullumInternationalizationWebServiceContext.Expressions.Update(existingExpression);
            }

            return expressionRecord;
        }
    }
}
