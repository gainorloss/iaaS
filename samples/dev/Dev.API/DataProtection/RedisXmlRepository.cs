using FreeRedis;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.AspNetCore.DataProtection.FreeRedis
{
    /// <summary>
    /// An XML repository backed by a Redis list entry.
    /// </summary>
    public class RedisXmlRepository : IXmlRepository
    {
        private readonly IRedisClient _redis;
        private readonly string _key;

        /// <summary>
        /// Creates a <see cref="RedisXmlRepository"/> with keys stored at the given directory.
        /// </summary>
        public RedisXmlRepository(IRedisClient redis, string key)
        {
            _redis = redis;
            _key = key;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return GetAllElementsCore().ToList().AsReadOnly();
        }

        private IEnumerable<XElement> GetAllElementsCore()
        {
            // Note: Inability to read any value is considered a fatal error (since the file may contain
            // revocation information), and we'll fail the entire operation rather than return a partial
            // set of elements. If a value contains well-formed XML but its contents are meaningless, we
            // won't fail that operation here. The caller is responsible for failing as appropriate given
            // that scenario.
            var len = _redis.LLen(_key);
            foreach (var value in _redis.LRange(_key, 0, len))
            {
                yield return XElement.Parse(value);
            }
        }

        /// <inheritdoc />
        public void StoreElement(XElement element, string friendlyName)
        {
            _redis.RPush(_key, element.ToString(SaveOptions.DisableFormatting));
        }
    }
}
