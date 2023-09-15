using Galosoft.IaaS.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dev.ConsoleApp.RestClients
{
    //[RestController("posts")]
    internal interface JsonPlaceholderClient
    {
        //[RestServiceFunc(RemoteFuncType.Read,"/")]
        Task<IEnumerable<Post>> PostsGetAsync();
    }
    internal class Post
    {
        public int Id { get; set; }
    }
}
