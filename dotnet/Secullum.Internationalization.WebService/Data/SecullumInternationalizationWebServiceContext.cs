using Microsoft.EntityFrameworkCore;
using Secullum.Internationalization.WebService.Model;

namespace Secullum.Internationalization.WebService.Data
{
    public class SecullumInternationalizationWebServiceContext : DbContext
    {
        public SecullumInternationalizationWebServiceContext (DbContextOptions<SecullumInternationalizationWebServiceContext> options)
            : base(options)
        {
        }

        public DbSet<Expression> Expressions { get; set; }
    }
}
