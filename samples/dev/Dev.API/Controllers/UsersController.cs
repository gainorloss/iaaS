using Dapper;
using Dev.ConsoleApp.Entities;
using Galosoft.IaaS.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Sample.API;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/api/[controller]/[action]")]
public class UsersController : ControllerBase
{
    private readonly AdminDbContext _ctx;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ctx"></param>
    public UsersController(AdminDbContext ctx)
    {
        _ctx = ctx;
    }

    /// <summary>
    /// 
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
