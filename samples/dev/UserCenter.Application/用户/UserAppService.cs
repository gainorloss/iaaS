using Dapper;
using Dev.ConsoleApp.Entities;
using Galosoft.IaaS.AspNetCore.DynamicApi;
using Galosoft.IaaS.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace UserCenter.Application
{
    /// <summary>
    /// 用户
    /// </summary>
    [RestController("uc")]
    public class UserAppService
    {
        private readonly AdminDbContext _ctx;

        /// <summary>
        /// 
        /// </summary>
        public UserAppService(AdminDbContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<RestResult> AllAsync()
        {
            var cnn = _ctx.Database.GetDbConnection();
            var users = await cnn.QueryAsync(@"SELECT *
from uc.uc_users uu ");

            return RestResult.Succeed(users);
        }
    }
}
