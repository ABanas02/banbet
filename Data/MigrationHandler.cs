using Microsoft.EntityFrameworkCore;

namespace banbet.Data 
{
    public static class MigrationHandler 
    {
        public static void ApplyMigrations(this IApplicationBuilder app) 
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using ApplicationDbContext dbContext = 
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }
    }
}

