using FreeRedis;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Galosoft.IaaS.Redis
{
    /// <summary>
    /// redis code generator.
    /// </summary>
    public class RedisCodeGenerator
    {
        private readonly RedisClient _cli;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="cli"></param>
        public RedisCodeGenerator(RedisClient cli)
        {
            _cli = cli;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bizCode"></param>
        /// <param name="centerCode"></param>
        /// <returns></returns>
        public async Task<string> CodeGenerateDailyAsync(string bizCode, string centerCode = "sc")
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            return await StrGetByRedisAsync(bizCode, centerCode, date);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bizCode"></param>
        /// <param name="centerCode"></param>
        /// <returns></returns>
        public string CodeGenerateDaily(string bizCode, string centerCode = "sc")
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            return StrGetByRedis(bizCode, centerCode, date);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bizCode"></param>
        /// <param name="centerCode"></param>
        /// <returns></returns>
        public async Task<string> CodeGenerateAsync(string bizCode, string centerCode = "sc")
        {
            return await StrGetByRedisAsync(bizCode, centerCode, width: 6);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bizCode"></param>
        /// <param name="centerCode"></param>
        /// <returns></returns>
        public string CodeGenerate(string bizCode, string centerCode = "sc")
        {
            return StrGetByRedis(bizCode, centerCode, width: 6);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bizCode"></param>
        /// <param name="centerCode"></param>
        /// <returns></returns>
        public async Task<long> IdentityGenerateAsync(string bizCode, string centerCode = "sc")
        {
            return await LongGetByRedisAsync(bizCode, centerCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bizCode"></param>
        /// <param name="centerCode"></param>
        /// <returns></returns>
        public long IdentityGenerate(string bizCode, string centerCode = "sc")
        {
            return LongGetByRedis(bizCode, centerCode);
        }

        internal string StrGetByRedis(string bizCode, string centerCode, string? date = null, int width = 7)
        {
            long val = LongGetByRedis(bizCode,centerCode,date);

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(bizCode);
            stringBuilder.Append(date);

            var seed = val.ToString().PadLeft(width, '0');
            stringBuilder.Append(seed);
            var code = stringBuilder.ToString();
            Trace.WriteLine($"\t{code}", $"“{centerCode}”>");
            return code;
        }

        internal async Task<string> StrGetByRedisAsync(string bizCode, string centerCode, string? date = null, int width = 7)
        {
            long val = await LongGetByRedisAsync(bizCode,centerCode,date);

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(bizCode);
            stringBuilder.Append(date);

            var seed = val.ToString().PadLeft(width, '0');
            stringBuilder.Append(seed);
            var code = stringBuilder.ToString();
            Trace.WriteLine($"\t{code}", $"“{centerCode}”>");
            return code;
        }

        internal long LongGetByRedis(string bizCode, string centerCode, string? date = null)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(centerCode);

            if (!string.IsNullOrEmpty(date))
                keyBuilder.AppendFormat(":{0}", date);
            keyBuilder.AppendFormat(":{0}", bizCode);

            var key = keyBuilder.ToString();

            long val = 1;
            var rt = _cli.SetNx(key, val);
            if (!rt)
                val = _cli.Incr(key);
            Trace.WriteLine($"\t{val}",$"“{centerCode}”>");
            return val;
        }

        internal async Task<long> LongGetByRedisAsync(string bizCode, string centerCode, string? date = null)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(centerCode);

            if (!string.IsNullOrEmpty(date))
                keyBuilder.AppendFormat(":{0}", date);
            keyBuilder.AppendFormat(":{0}", bizCode);

            var key = keyBuilder.ToString();

            long val = 1;
            var rt = await _cli.SetNxAsync(key, val);
            if (!rt)
                val = await _cli.IncrAsync(key);
            Trace.WriteLine($"\t{val}", $"“{centerCode}”>");
            return val;
        }
    }
}
