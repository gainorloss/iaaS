using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Loader;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DbContextBase<T>
        : DbContext, IDisposable
        where T : DbContext
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly ILogger<DbContextBase<T>> Logger;
        protected DateTime CreatedAt => DateTime.Now;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public DbContextBase([NotNull] DbContextOptions<T> options)
            : base(options)
        {
            if (ServiceProvider == null)
            {
                var coreOptionsExtension = options.FindExtension<CoreOptionsExtension>();
                ServiceProvider = coreOptionsExtension.ApplicationServiceProvider;
            }

            Logger = ServiceProvider.GetRequiredService<ILogger<DbContextBase<T>>>();

            SavingChanges += ApplicationDbContext_SavingChanges;
        }

        /// <summary>
        /// 用于查
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            return Set<TEntity>().AsNoTracking();
        }

        /// <summary>
        /// 用于增删改
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public DbSet<TEntity> Entity<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }

        private void ApplicationDbContext_SavingChanges(object sender, SavingChangesEventArgs e)
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (DisablePhysicalDeletion && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Unchanged;
                    continue;
                }
                OnSavingChanges(entry);
            }
        }

        protected abstract void OnSavingChanges(EntityEntry entry);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.EnableServiceProviderCaching();
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);//默认关闭追踪

            if (ServiceProvider != null)
            {
                var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
                if (loggerFactory != null)
                    optionsBuilder.UseLoggerFactory(loggerFactory);

                var env = ServiceProvider.GetRequiredService<IHostEnvironment>();
                if (env != null && env.IsDevelopment())
                    optionsBuilder.EnableSensitiveDataLogging()
                        .EnableDetailedErrors();

                var memoryCache = ServiceProvider.GetService<IMemoryCache>() as IMemoryCache;
                if (memoryCache != null)
                    optionsBuilder.UseMemoryCache(memoryCache);
            }

            //optionsBuilder.AddInterceptors(,);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var model = modelBuilder.Model;
            var types = DependencyContext.Default.GetTypes(type => type.HasCustomeAttribute<TableAttribute>(false));
            if (types == null || !types.Any())
                return;

            EntityTypeBuilder? entityBuilder = null;
            foreach (var type in types)
            {
                model.AddEntityTypeIfNotExists(type);

                entityBuilder = modelBuilder.Entity(type);

                //entityBuilder.Configure<DescriptionAttribute>((entityTypeBuilder, attr) => entityTypeBuilder.HasComment(attr.Description));

                var props = type.GetProperties(System.Reflection.BindingFlags.Public)
                      .Where(prop => !prop.PropertyType.IsInterface && !prop.PropertyType.IsClass && !prop.PropertyType.IsAbstract);

                foreach (var prop in props)
                {
                    //entityBuilder.Property(prop.Name).Configure<DefaultValueAttribute>((propertyBuilder, attr) => propertyBuilder.HasDefaultValue(attr.Value));
                    //entityBuilder.Property(prop.Name).Configure<DescriptionAttribute>((propertyBuilder, attr) => propertyBuilder.HasComment(attr.Description));
                    //entityBuilder.Property(prop.Name).Configure<EnumDataTypeAttribute>((propertyBuilder, attr) => propertyBuilder.HasConversion(attr.EnumType));
                }
            }
        }

        /// <summary>
        /// 禁用物理删除
        /// </summary>
        protected bool DisablePhysicalDeletion { get; set; } = false;

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
