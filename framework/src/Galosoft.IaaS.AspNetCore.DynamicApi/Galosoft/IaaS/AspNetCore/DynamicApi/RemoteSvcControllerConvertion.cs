using Galosoft.IaaS.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Galosoft.IaaS.AspNetCore.DynamicApi
{
    internal class RestControllerConvertion
          : IApplicationModelConvention
    {
        /// <inheritdoc/>
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                // 获取RestControllerAttribute
                //var remoteSvc = controller.Attributes.FirstOrDefault(attr => attr.GetType() == typeof(RestClientAttribute)) as RestClientAttribute;
                var @is = controller.ControllerType.GetInterfaces();
                var remoteSvc = @is.SelectMany(i => i.GetCustomAttributes<RestServiceAttribute>(true)).Concat(controller.ControllerType.GetCustomAttributes<RestServiceAttribute>(true))
                    .FirstOrDefault();
                if (remoteSvc == null)
                    continue;

                if (string.IsNullOrWhiteSpace(controller.ControllerName))
                    continue;

                controller.ControllerName = remoteSvc.Route;

                if (string.IsNullOrWhiteSpace(controller.ApiExplorer.GroupName))
                    controller.ApiExplorer.GroupName = controller.ControllerName;

                if (controller.ApiExplorer.IsVisible == null)
                    controller.ApiExplorer.IsVisible = true;

                var selectorModel = new SelectorModel();
                var attributeRouteModel = new AttributeRouteModel();
                foreach (var action in controller.Actions)
                {
                    if (string.IsNullOrWhiteSpace(action.ActionName))
                        continue;


                    if (action.ApiExplorer.IsVisible == null)
                        action.ApiExplorer.IsVisible = true;


                    attributeRouteModel.Template = remoteSvc.ToString();

                    var httpMethod = "POST";
                    var args = action.Parameters.Select(p => p.ParameterType).ToArray();

                    var funcs = new List<RestServiceFuncAttribute>();
                    foreach (var i in @is)
                    {
                        var method = i.GetMethod(action.ActionMethod.Name, args);
                        if (method == null)
                            continue;
                        var attrs = method.GetCustomAttributes<RestServiceFuncAttribute>();
                        if (attrs == null || !attrs.Any())
                            continue;
                        funcs.AddRange(attrs);
                    }

                    var remoteFunc = funcs.FirstOrDefault();

                    if (remoteFunc != null && remoteFunc.FuncType == RemoteFuncType.Read)
                    {
                        action.ActionName = remoteFunc.Route;
                        httpMethod = "GET";
                    }

                    if (!action.Selectors.Any())
                    {
                        selectorModel.AttributeRouteModel = attributeRouteModel;
                        selectorModel.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { httpMethod }));
                        action.Selectors.Add(selectorModel);
                    }
                    else
                    {
                        foreach (var selector in action.Selectors)
                        {
                            if (!selector.ActionConstraints.Any())
                                selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { httpMethod }));

                            selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(selector.AttributeRouteModel, attributeRouteModel);
                        }
                    }

                    foreach (var parameter in action.Parameters)
                    {
                        if (parameter.BindingInfo != null)
                            continue;

                        if (!typeof(ValueType).IsAssignableFrom(parameter.ParameterType) && CanUseFromBody(action.Selectors))
                            parameter.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
                        else
                            parameter.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromQueryAttribute() });
                    }

                }
            }
        }

        private bool CanUseFromBody(IList<SelectorModel> selectors)
        {
            var methods = new string[] { "GET", "HEAD", "DELETE" };
            foreach (var selector in selectors)
            {
                foreach (var actionConstraint in selector.ActionConstraints)
                {
                    var httpActionConstraint = actionConstraint as HttpMethodActionConstraint;
                    if (httpActionConstraint.HttpMethods.Any(method => methods.Contains(method)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
