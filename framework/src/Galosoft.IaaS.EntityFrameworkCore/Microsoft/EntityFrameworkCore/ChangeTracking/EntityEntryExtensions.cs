using System.Linq;

namespace Microsoft.EntityFrameworkCore.ChangeTracking
{
    public static class EntityEntryExtensions
    {
        public static void PropertyValue(this EntityEntry entityEntry, string propertyName, object propertyValue)
        {
            var props = entityEntry.Properties;

            if (!props.Any(prop => prop.Metadata.Name.Equals(propertyName)))
                return;

            entityEntry.Property(propertyName).CurrentValue = propertyValue;
        }
    }
}
