using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Secullum.Internationalization.WebService.Data;
using Secullum.Internationalization.WebService.Services;
using static Secullum.Internationalization.WebService.Services.ExpressionsService;

namespace Secullum.Internationalization.DatabaseTranslator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddDbContext<SecullumInternationalizationWebServiceContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("SecullumInternationalizationWebServiceContext")))
                .Configure<TranslatorSettings>(configuration.GetSection("TranslatorSettings"))
                .AddSingleton<TranslationService>()
                .AddTransient<DatabaseTranslation>()
                .BuildServiceProvider();

            var databaseTranslation = serviceProvider.GetRequiredService<DatabaseTranslation>();
            await databaseTranslation.TranslateExpressionsAsync();
        }
    }

    public class DatabaseTranslation
    {
        private readonly SecullumInternationalizationWebServiceContext m_secullumInternationalizationWebServiceContext;
        private readonly TranslationService m_translationService;

        public DatabaseTranslation(SecullumInternationalizationWebServiceContext seci18nWebServiceContext, TranslationService translationService)
        {
            m_secullumInternationalizationWebServiceContext = seci18nWebServiceContext;
            m_translationService = translationService;
        }

        public async Task TranslateExpressionsAsync()
        {
            Console.WriteLine("Iniciando a tradução das expressões...");

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
                .Select(x => x.Value);

            foreach (var expression in expressionsToTranslate)
            {
                await m_translationService.TranslateExpression(expression);

                Console.WriteLine($"\nTraduzindo: {expression.Portuguese} -> EN: {expression.English} | ES: {expression.Spanish}");
            }

            await m_secullumInternationalizationWebServiceContext.SaveChangesAsync();

            Console.WriteLine("Tradução concluída!");
        }
    }
}
