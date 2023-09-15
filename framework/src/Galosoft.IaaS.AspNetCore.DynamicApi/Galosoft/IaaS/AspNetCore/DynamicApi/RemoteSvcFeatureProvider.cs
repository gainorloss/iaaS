using Galosoft.IaaS.Core;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Galosoft.IaaS.AspNetCore.DynamicApi
{
    /// <summary>
    /// Rest controller feature provider.
    /// </summary>
    internal class RestControllerFeatureProvider
       : ControllerFeatureProvider
    {
        /// <inheritdoc/>
        protected override bool IsController(TypeInfo typeInfo)
        {
            if (!typeInfo.IsClass || typeInfo.IsAbstract || !typeInfo.Name.EndsWith("Service"))
                return false;

            var @is = typeInfo.GetInterfaces().SelectMany(i => i.GetCustomAttributes<RestServiceAttribute>(true)).Concat(typeInfo.GetCustomAttributes<RestServiceAttribute>(true));
            return @is.Any();
        }
    }
}
