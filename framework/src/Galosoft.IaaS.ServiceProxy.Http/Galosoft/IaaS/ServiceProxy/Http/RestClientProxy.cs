using Galosoft.IaaS.Core;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Galosoft.IaaS.ServiceProxy.Http
{
    public class RestClientProxy : DispatchProxy
    {
        private readonly RestClient _restClient;

        public static IServiceProvider ServiceProvider;

        private static ConcurrentDictionary<RemoteFuncType, MethodInfo> _methodInfos = new ConcurrentDictionary<RemoteFuncType, MethodInfo>();

        public RestClientProxy()
        {
            _restClient = ServiceProvider?.GetService(typeof(RestClient)) as RestClient;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var declaringType = targetMethod.DeclaringType;
            if (!declaringType.HasCustomeAttribute<RestServiceAttribute>(false))
                throw new InvalidOperationException($"请检查一下约束是否缺少特性{nameof(RestServiceAttribute)}");

            var returnType = targetMethod.ReturnType.BaseType == typeof(Task)
                ? targetMethod.ReturnType.GetGenericArguments()[0]
                : targetMethod.ReturnType;

            var remoteSvc = declaringType.GetCustomAttribute<RestServiceAttribute>();
            var method = targetMethod.Name;

            var requestUri = remoteSvc.ToString(targetMethod.Name);

            var funcType = RemoteFuncType.Write;
            if (targetMethod.HasCustomeAttribute<RestServiceFuncAttribute>(false))
            {
                var func = targetMethod.GetCustomAttribute<RestServiceFuncAttribute>(false);

                funcType = func.FuncType;
                if (!string.IsNullOrWhiteSpace(func.Route))
                    requestUri = remoteSvc.ToString(func.Route);
            }

            var genericMi = _methodInfos.GetOrAdd(funcType, funcType =>
              {
                  var method = funcType == RemoteFuncType.Read
                      ? nameof(RestClient.GetAsync)
                     : nameof(RestClient.PostAsync);

                  var genericMi = _restClient.GetType()
                  .GetMethod(method)
                  .MakeGenericMethod(returnType);

                  _methodInfos[funcType] = genericMi;
                  return genericMi;
              });

            var parameters = funcType == RemoteFuncType.Read
                ? new object[] { requestUri }
                : new object[] { requestUri, args?.FirstOrDefault() };
            var rt = genericMi.Invoke(_restClient, parameters);
            return rt;
        }
    }
}
