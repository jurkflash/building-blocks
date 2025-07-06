using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Persistence.Abstractions;
using Pokok.BuildingBlocks.Persistence.Converters;
using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyGlobalConfigurations(this ModelBuilder modelBuilder)
        {
            ApplySoftDeleteFilter(modelBuilder);
            ApplyUtcDateTimeConverter(modelBuilder);
        }

        private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var propertyMethod = typeof(EF).GetMethod("Property")!
                        .MakeGenericMethod(typeof(bool));
                    var isDeletedProperty = Expression.Call(propertyMethod, parameter, Expression.Constant("IsDeleted"));
                    var compareExpression = Expression.Equal(isDeletedProperty, Expression.Constant(false));

                    var lambda = Expression.Lambda(compareExpression, parameter);
                    entityType.SetQueryFilter(lambda);
                }
            }
        }

        private static void ApplyUtcDateTimeConverter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

                foreach (var property in properties)
                {
                    property.SetValueConverter(new UtcDateTimeConverter());
                }
            }
        }
    }
}
