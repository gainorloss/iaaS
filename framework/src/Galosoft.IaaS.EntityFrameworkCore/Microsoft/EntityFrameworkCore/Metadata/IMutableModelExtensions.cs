using System;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public static class IMutableModelExtensions
    {
        public static IMutableEntityType AddEntityTypeIfNotExists(this IMutableModel model, Type type)
        {
            var mutablEntityType = model.FindEntityType(type);
            if (mutablEntityType != null)
                return mutablEntityType;
            mutablEntityType = model.AddEntityType(type);
            return mutablEntityType;
        }
    }
}
