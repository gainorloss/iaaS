﻿using Dev.Application.Contracts;
using Galosoft.IaaS.AspNetCore.DynamicApi;
using Galosoft.IaaS.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Dev.Application
{
    /// <summary>
    /// 示例
    /// </summary>
    [RestController("dev")]
    public class DevAppService
        : ApplicationService, IDevAppService
    {
        /// <summary>
        ///  Get .
        /// </summary>
        /// <returns></returns>  
        [HttpGet]
        public async Task<RestResult> GetCountAsync()
        {
            await Task.Delay(100);
            Trace.WriteLine(GetHashCode(), "“TestService”");
            return RestResult.Succeed(1);
        }

        /// <summary>
        /// post.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<RestResult> CreateAsync()
        {
            await Task.Delay(100);
            return RestResult.Succeed(false);
        }

        /// <summary>
        /// 抛出异常
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpPost]
        public async Task<RestResult> ThrowExceptionAsync()
        {
            await Task.Delay(100);
            throw new InvalidOperationException();
        }

        /// <summary>
        /// 分页获取
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<RestResult> PageGetAsync(PagedRequestDto requestDto)
        {
            await Task.Delay(10);
            var rt = new PagedResponse<int>(Enumerable.Range(0, 5), 15);
            return RestResult.Succeed(rt);
        }
    }
}