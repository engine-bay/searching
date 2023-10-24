namespace EngineBay.ModuleTemplate
{
    using EngineBay.Core;

    public class ModuleTemplateModule : BaseModule
    {
        public override IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
        {
            // override the base method to configure dependency injection
            // otherwise, delete this method
            return services;
        }

        public override RouteGroupBuilder MapEndpoints(RouteGroupBuilder endpoints)
        {
            // override the base method to register API endpoints
            // otherwise, delete this method
            return endpoints;
        }

        public override WebApplication AddMiddleware(WebApplication app)
        {
            // override the base method to register any middleware
            // otherwise, delete this method
            return app;
        }

        public override IServiceCollection RegisterPolicies(IServiceCollection services)
        {
            // override the base method to register any role and claim policies
            // otherwise, delete this method
            return services;
        }

        public override void SeedDatabase(string seedDataPath, IServiceProvider serviceProvider)
        {
            // override the base method to load seed data for the module
            // otherwise, delete this method
            return;
        }
    }
}