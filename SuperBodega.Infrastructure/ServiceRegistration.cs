using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Messaging;
using SuperBodega.Core.Services;
using SuperBodega.Infrastructure.Data;
using SuperBodega.Infrastructure.Messaging;
using SuperBodega.Infrastructure.Repositories;
using SuperBodega.Infrastructure.Services;

namespace SuperBodega.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();

            // Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<ISaleService, SaleService>();

            // Messaging
            services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();
            services.AddSingleton<IMessageConsumer, RabbitMQConsumer>();

            // Agregar esto a ServiceRegistration.cs
            services.AddScoped<IAsyncSaleService, AsyncSaleService>();

            // Añadir a ServiceRegistration.cs
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<NotificationService>();

            // Añadir a ServiceRegistration.cs
            services.AddScoped<ICartService, CartService>();
            return services;
        }
    }
}