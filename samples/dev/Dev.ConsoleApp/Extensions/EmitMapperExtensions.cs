using System.Collections.Generic;

namespace EmitMapper
{
    public static class EmitMapperExtensions
    {
        public static TTarget To<TSource, TTarget>(this TSource o)
        {
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TSource, TTarget>();
            return mapper.Map(o);
        }

        public static IEnumerable<TTarget> To<TSource, TTarget>(this IEnumerable<TSource> o)
        {
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TSource, TTarget>();
            return mapper.MapEnum(o);
        }
    }
}
