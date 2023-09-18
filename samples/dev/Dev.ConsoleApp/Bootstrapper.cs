using AngleSharp;
using AngleSharp.Html.Parser;
using Dev.Application;
using Dev.Application.Contracts;
using Dev.ConsoleApp.DynamicProxy;
using Dev.ConsoleApp.Entities;
using Dev.ConsoleApp.RestClients;
using Dev.ConsoleApp.Services;
using Dev.ConsoleApp.WindowsAPI;
using Dev.Core.Models;
using Dev.Entities;
using FreeRedis;
using Galosoft.IaaS.Dev;
using Galosoft.IaaS.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nacos.V2;
using NetCasbin;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ToolGood.Words;

namespace Dev.ConsoleApp
{
    internal class Bootstrapper
        : BackgroundService
    {
        private readonly IPerformanceTester _performanceTester;
        private readonly IDevAppService _sampleSvc;
        private readonly RedisClient _cli;
        private readonly IHostEnvironment _env;
        private readonly INacosConfigService _nacosConfig;
        private readonly RedisCodeGenerator _codeGenerator;
        private readonly ILogger<Bootstrapper> _logger;
        private readonly JsonPlaceholderClient _jsonPlaceholderClient;
        private readonly IConnectionFactory _factory;
        private readonly IComponentSvc _component;
        private readonly DynamicProxyGenerator _proxyGenerator;
        private readonly IServiceProvider _root;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly HttpClient _client;

        public Bootstrapper(IPerformanceTester performanceTester,
            IDevAppService sampleSvc,
            IHttpClientFactory factory,
            RedisClient cli,
            IHostEnvironment env,
            INacosConfigService nacosConfig,
            RedisCodeGenerator codeGenerator,
            ILogger<Bootstrapper> logger,
            //JsonPlaceholderClient jsonPlaceholderClient,
            IConnectionFactory connectionFactory,
            IComponentSvc component,
            DynamicProxyGenerator proxyGenerator,
            IServiceProvider root,
            IServiceScopeFactory scopeFactory)
        {
            _performanceTester = performanceTester;
            _sampleSvc = sampleSvc;
            _cli = cli;
            _env = env;
            _nacosConfig = nacosConfig;
            _codeGenerator = codeGenerator;
            _logger = logger;
            //_jsonPlaceholderClient = jsonPlaceholderClient;
            _factory = connectionFactory;
            _component = component;
            _proxyGenerator = proxyGenerator;
            _root = root;
            _scopeFactory = scopeFactory;
            _client = factory.CreateClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //await NacosConfigTestAsync();
            //var json = await _jsonPlaceholderClient.PostsGetAsync();

            //await DlxTestAsync();
            //await CsbinTestAsync();
            //await RedisCodeGeneratorTestAsync();
            //_component.ThrowException();
            //_component.DisplayName();
            //DynamicProxyTest();
            //User32Test();
            //ToolGoodWordsTest();
            //await AngleSharpTestAsync();
            //ServiceCollectionTest();
        }
        private void ServiceCollectionTest()
        {
            //using (var scope = _scopeFactory.CreateScope())
            //{
            //    var sp = scope.ServiceProvider;
            //    svc = sp.GetRequiredService<OrderService>();
            //    var list = await svc.ListAsync();
            //}
            {
                var idx = 1;
                while (true)
                {
                    Console.WriteLine($"{idx++}?");
                    Console.ReadLine();
                    Parallel.For(0, 2000, async i =>
                    {
                        {
                            //using (var scope = _scopeFactory.CreateScope())
                            //{
                            //    var sp = scope.ServiceProvider;
                            //    var ctx = sp.GetRequiredService<OrderDbContext>();
                            //    var orders = await ctx.Query<Order>().Take(100).ToListAsync();
                            //}
                        }
                        {
                            //OrderService svc = null;
                            //using (var scope = _scopeFactory.CreateScope())
                            //{
                            //    var sp = scope.ServiceProvider;
                            //    var ctx = sp.GetRequiredService<OrderDbContext>();
                            //    svc = new OrderService(ctx);
                            //    await svc.ListAsync();
                            //}
                        }
                        {
                            OrderService svc = null;
                            using (var scope = _scopeFactory.CreateScope())
                            {
                                var sp = scope.ServiceProvider;
                                svc = sp.GetRequiredService<OrderService>();
                                await svc.ListAsync();
                            }
                        }

                    });
                }
            }

            //{
            //    var ctx = _root.GetRequiredService<OrderDbContext>();
            //    svc = new OrderService(ctx);
            //    var list = await svc.ListAsync();
            //}

        }
        private async Task AngleSharpTestAsync()
        {
            var sw = Stopwatch.StartNew();
            var html = @"<img class=""desc_anchor"" id=""desc-module-1"" src=""https://assets.alicdn.com/kissy/1.0.0/build/imglazyload/spaceball.gif""><p>..........<span style="""">健康成长、快乐生活，是父母对孩子的美好期望，但在日常养育中，大大小小的健康问题总会时不时地来&ldquo;敲门&rdquo;，比如以上刚刚提到的那些问题，家有萌娃的你是不是没少遇到过呢？</span></p><p><br>本书选取育儿界关注度至高的0~6岁儿童常见病，同时以亲切感十足的问题形式开启，把父母应该掌握的疾病知识 护理技巧娓娓道来。哪些疾病春季高发？哪些疾病夏季高发？秋冬季节时，孩子们又容易获得哪些疾病的&ldquo;青睐&rdquo;呢？根据疾病高发季节不同，本书分春、夏、秋、冬四季讲述，育儿，也可按部就班。此外，对待每一种疾病，如何从衣食住行4个方面来精心防护呢？书中同样给出了、细致、操作性极强的解析及应对方法，让父母看到就能做到。<br><br>关于长高、补钙、食欲、入园等成长中的高频问题，想听听儿科专家的意见吗？用医生的眼界给出的答案，不但新鲜感十足，更会让我们受益匪浅。</p><img class=""desc_anchor"" id=""desc-module-2"" src=""https://assets.alicdn.com/kissy/1.0.0/build/imglazyload/spaceball.gif""><p>春季篇<br><br>孩子手上起小红点了，手足口病到底怎么判断？<br><br>了解手足口病<br><br>贴身衣物分开清洗，通风处暴晒<br><br>避免进食偏热、偏酸的食物<br><br>居家隔离至少1周<br><br>体温稳定后可以外出活动<br><br> <br><br>孩子起了水痘，还能让他上幼儿园吗？ <br><br>了解水痘<br><br>贴身衣物要柔软，避免摩擦皮疹、水泡引起破溃 <br><br>可以喝稀释后的果汁，患儿餐具要消毒<br><br>需隔离至疱疹全部结痂<br><br>与水痘患儿接触后要观察至少3周<br><br> <br><br>急性荨麻疹会自愈吗？出疹期间可以给孩子洗澡吗？ <br><br>了解急性荨麻疹<br><br>贴身衣物纯棉宽松，且避免颜色鲜艳和有过多图案<br><br>严格回避过敏原食物<br><br>排查居住环境中的过敏原<br><br>出行时要避免疲劳和大量出汗<br><br> <br><br>带孩子去公园看花，他怎么突然就不停地打喷嚏、流鼻涕、揉眼睛呢？ <br><br>了解过敏性鼻炎<br><br>不穿毛织类衣物<br><br>少吃高蛋白食物<br><br>暂别毛绒类玩具，彻底清洁窗帘等<br><br>外出戴口罩和护目镜<br><br> <br><br>孩子得了幼儿急疹要退烧吗？疹子需不需要涂药膏？ <br><br>了解幼儿急疹<br><br>贴身衣物透气、吸汗<br><br>保证母乳喂养，辅食减量、减淡、降稠<br><br>室内温度25~28℃，波动范围5℃之内<br><br>出疹后可以短时间户外活动<br><br> <br><br>孩子刚满月，每天会出现阵发性哭闹，很难安抚，同时全身有皮疹，是怎么回事？ <br><br>了解牛奶蛋白过敏<br><br>贴身衣被纯棉柔软，避免绑带勒紧<br><br>6月龄内以纯母乳喂养为<br><br>注意交叉过敏，牛奶蛋白过敏可能并存环境因素过敏<br><br>过敏期间暂不外出，除非就医<br><br> <br><br> <br><br>猩红热是春天常见的传染病，如何让孩子不被传染呢？ <br><br>了解猩红热<br><br>衣物慢脱、慢减 <br><br>食物要应季新鲜<br><br>房间开窗通风<br><br>避免去人多的地方<br><br> <br><br> <br><br>春天是长个儿的好季节！这样吃才能让孩子长高！ 吃得要&ldquo;杂&rdquo; <br><br>吃得要&ldquo;准&rdquo; <br><br>吃得要&ldquo;活&rdquo;<br><br>&hellip;&hellip;<br><br> <br><br>......<br>&ldquo;孩子手上起了很多小疹子，是手足口病吗？&rdquo;<br><br>&ldquo;春暖花开，带孩子去公园看花，为什么孩子突然不停地打喷嚏，流眼泪了呢？&rdquo;<br><br>&ldquo;冬季流感高发，如何做好防护呢？&rdquo;<br><br>&ldquo;孩子刚上幼儿园，就病了好几次了，怎么办呢？&rdquo;</p>";

            var parser = new HtmlParser();
            sw.Start();
            var doc = parser.ParseDocument(html);
            var text = doc.Body.TextContent;
            Tracer.Trace(text, "anglesharp html parser");
            sw.Stop();
            Tracer.Trace(sw.ElapsedMilliseconds, "anglesharp html parser");

            var ctx = BrowsingContext.New(Configuration.Default);
            sw.Restart();
            var document = await ctx.OpenAsync(r => r.Content(html));
            text = document.Body.TextContent;
            Tracer.Trace(text, "anglesharp html parser");
            sw.Stop();
            Tracer.Trace(sw.ElapsedMilliseconds, "anglesharp browsing context");
        }

        private void ToolGoodWordsTest()
        {
            string s = "中国|国人|zg人";
            string test = "我是中国人";

            StringSearch iwords = new StringSearch();
            iwords.SetKeywords(s.Split('|'));

            var b = iwords.ContainsAny(test);

            var f = iwords.FindFirst(test);

            var all = iwords.FindAll(test);

            var str = iwords.Replace(test, '*');
        }

        private void User32Test()
        {
            WindowApi.SetCursorPos(10, 10);
        }

        private void DynamicProxyTest()
        {
            //var proxy = DispatchProxy.Create<IComponentSvc, DynamicProxy>();
            //(proxy as DynamicProxy).Instance = new ComponentSvc();
            //proxy.DisplayName();

            var svc = _proxyGenerator.Generate<IComponentSvc, LoggingDynamicInterceptor>();
            //svc.DisplayName();
            _performanceTester.SingleThread(ctx => svc.DisplayName(), fact: "dynamic proxy", times: 10000 * 1000);
            _performanceTester.SingleThread(ctx => _component.DisplayName(), fact: "instance", times: 10000 * 1000);
        }

        private async Task RedisCodeGeneratorTestAsync()
        {
            var idx = 7;
            while (idx-- >= 0)
            {
                _codeGenerator.CodeGenerateDaily("SC");
            }

            idx = 7;
            while (idx-- >= 0)
            {
                await _codeGenerator.CodeGenerateDailyAsync("SC");
            }

            idx = 7;
            while (idx-- >= 0)
            {
                _codeGenerator.CodeGenerate("JS");
            }

            idx = 7;
            while (idx-- >= 0)
            {
                await _codeGenerator.CodeGenerateAsync("JS");
            }

            idx = 7;
            while (idx-- >= 0)
            {
                _codeGenerator.IdentityGenerate("JS");
            }

            idx = 7;
            while (idx-- >= 0)
            {
                await _codeGenerator.IdentityGenerateAsync("JS");
            }
        }

        private async Task CsbinTestAsync()
        {
            var enforcer = new Enforcer("model.conf", "policy.csv");
            var allowed = await enforcer.EnforceAsync("alice", "data1", "read");
            allowed = await enforcer.EnforceAsync("alice", "data1", "write");
            allowed = await enforcer.EnforceAsync("alice", "data2", "read");
            allowed = await enforcer.EnforceAsync("alice", "data2", "write");
            var roles = enforcer.GetRolesForUser("alice");
            var users = enforcer.GetUsersForRole("data2_admin");
        }
        private async Task DlxTestAsync()
        {
            var handlerProperty = new HandlerProperty(1, true, 0);
            var arguments = new QueueArgument(dlxEnabled: true, msgTtl: 15 * 60 * 1000, idempotenceKeyFormat: "spring.boot.amqp:{0}", redis: _cli);

            _factory.Handle<SpringBootAmqpTestRequest>(handlerProperty, async (e, sender, args) =>
            {
                await Task.Delay(100);
                Trace.WriteLine($"\t于{DateTime.Now.ToLongTimeString()}\t接收到{e.Message}", "“dlx”>");
                return true;
            }, "spring.boot.amqp.test", arguments: arguments);

            //_factory.Handle<SpringBootAmqpTestRequest>(handlerProperty, async e =>
            //{
            //    await Task.Delay(100);
            //    Trace.WriteLine($"\t于{DateTime.Now.ToLongTimeString()}\t接收到{e.Message}", "“dlx”>");
            //    return true;
            //}, "spring.boot.amqp.test", arguments: arguments);
            var idx = 0;
            while (true)
            {
                ++idx;
                await Task.Delay(1000);
                var msg = new SpringBootAmqpTestRequest
                {
                    MessageKey = idx.ToString(),
                    Message = DateTime.Now.ToLongTimeString()
                };
                _factory.Distribute("spring.boot.amqp.test", arguments: arguments, msgs: msg);
            }
        }

        private async Task NacosConfigTestAsync()
        {
            while (true)
            {
                var status = await _nacosConfig.GetServerStatus();
                var content = await _nacosConfig.GetConfig("name", "DEFAULT_GROUP", 3000);
                Trace.WriteLine($"nacos {DateTime.Now.ToShortTimeString()}>\t{status}:{content}");
                await Task.Delay(3000);
            }
        }

        private void TestOrderStateMachine()
        {
            var order = new TraderOrder(10001, "202212130001", TraderOrderState.Submitted);
            order.Pay(1000l);
        }
    }

    internal class LoggingDynamicInterceptor : IDynamicInterceptor
    {
        public object Intercept(object target, MethodInfo methodInfo, params object[] paras)
        {
            try
            {
                var rt = methodInfo.Invoke(target, paras);
                return rt;
            }
            catch (Exception e)
            {
                Tracer.Verbose(e, nameof(LoggingDynamicInterceptor));
                return target;
            }
            finally
            {
                //Tracer.Verbose("finally", nameof(LoggingDynamicInterceptor));
            }
        }
    }

    public interface IDynamicInterceptor
    {
        object Intercept(object target, MethodInfo methodInfo, params object[] paras);
    }


    internal class OrderSnapshotRequest
    {
        public string MessageId { get; set; }
        [JsonPropertyName("order_id_out")]
        [JsonProperty("order_id_out")]
        public string OrderIdOut { get; set; }
        [JsonPropertyName("paid_at")]
        [JsonProperty("paid_at")]
        public DateTime PaidAt { get; set; }
        [JsonPropertyName("order_payload")]
        [JsonProperty("order_payload")]
        public string OrderPayload { get; set; }
    }

    internal class SpringBootAmqpTestRequest
    {
        [MessageId]
        //[MessageKey]
        public string MessageKey { get; set; }
        public string Message { get; set; }
        public int Bytes
        {
            get
            {
                return Encoding.UTF8.GetByteCount(Message);
            }
        }
    }

    internal class EAOutput
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
