using Newtonsoft.Json;
using System.Text.Json;

namespace Galosoft.IaaS.Core
{

    #region Serializer galo@2023-7-28 09:57:17
    /// <summary>
    /// 
    /// </summary>
    public interface IObjectSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        T Deserialize<T>(string message);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        string Serialize<T>(T obj);
    }

    /// <summary>
    /// 
    /// </summary>
    public class MicrosoftJsonSerializer : IObjectSerializer
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,//新增：设置大小写不敏感 galo@2022-2-21 09:57:49，
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,//新增：设置使用驼峰形式属性名 提交 galo@2022-2-23 16:08:28
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T Deserialize<T>(string value)
        {
            var @event = System.Text.Json.JsonSerializer.Deserialize<T>(value, _options);
            return @event;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize<T>(T obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj, _options);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NewtonsoftJsonSerializer : IObjectSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T Deserialize<T>(string value)
        {
            var @event = JsonConvert.DeserializeObject<T>(value);
            return @event;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
    #endregion
}
