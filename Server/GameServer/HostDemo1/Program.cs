// See https://aka.ms/new-console-template for more information

// start from https://www.bilibili.com/video/BV1sX4y1L7c9/?spm_id_from=333.337.search-card.all.click&vd_source=226acdfc51a90ccbfeeeea7b5f0ccd1a
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");

#region 方式1
//
//var hostBuilder = Host.CreateApplicationBuilder(args);
//
//
//hostBuilder.Logging.SetMinimumLevel(LogLevel.Debug);
//
//hostBuilder.Logging.AddFilter((logMsg, level) =>
//{
//
//    return !logMsg.StartsWith("test");
//});
//
//hostBuilder.Services.AddHostedService<MyFistHostedService>();
//
//
//hostBuilder.Build().Run();
//
//
//
//Console.WriteLine();
#endregion


#region 方式2

var hostBuilder1 = Host.CreateDefaultBuilder(args);

hostBuilder1.ConfigureServices((context, services) =>
{
    services.AddHostedService<MyFistHostedService>();
});

hostBuilder1.ConfigureLogging(log => log.SetMinimumLevel(LogLevel.Debug));


hostBuilder1.ConfigureAppConfiguration((context, config) =>
{
    //配置其他参数
});

hostBuilder1.Build().Run();

#endregion














