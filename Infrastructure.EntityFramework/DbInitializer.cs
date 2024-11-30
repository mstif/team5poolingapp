
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Infrastructure.EntityFramework;
using ApplicationCore.Interfaces;

namespace Otus.Teaching.PromoCodeFactory.DataAccess.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DbInitializer> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public DbInitializer(IServiceScopeFactory scopeFactory,
            ILogger<DbInitializer> logger,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            this._scopeFactory = scopeFactory;
            this._logger = logger;
            this._configuration = configuration;
            this._unitOfWork = unitOfWork;

        }

        public  void Initialize()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {


                    //    context.Database.Migrate();
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    SeedData seedData = new SeedData(_unitOfWork);
                    seedData.Seed();

                    
                }
            }
        }

        
    }
}
