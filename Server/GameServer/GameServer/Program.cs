// See https://aka.ms/new-console-template for more information
using GameServer.Servers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

 

//// 这种方式配置只能 这样用  Log.Debug("11");
Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Debug().CreateLogger();

Log.Logger.Debug("111");

var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder.ConfigureServices((context, services) =>
{
     
    services.AddHostedService<MyServer02>();
});


hostBuilder.ConfigureAppConfiguration((context, config) =>
{
    //配置其他参数
}); 
await hostBuilder.
    UseConsoleLifetime().
    UseSerilog((ctx, ls) =>
    {
        //这个方法我看了一个小时没找到哪里有问题,最后翻源代码找到的!! Serilog.Settings.Configuration里面的扩展方法
        ls.ReadFrom.Configuration(ctx.Configuration);
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(ctx.Configuration).CreateLogger();
    }).
    RunConsoleAsync();


Console.WriteLine("Done");