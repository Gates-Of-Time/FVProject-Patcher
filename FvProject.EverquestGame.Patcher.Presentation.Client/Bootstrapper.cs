using System;
using System.IO;
using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Presentation.Client.Converters;
using FvProject.EverquestGame.Patcher.Presentation.Client.Pages;
using Microsoft.Extensions.Configuration;
using Stylet;
using StyletIoC;

namespace FvProject.EverquestGame.Patcher.Presentation.Client {
    public partial class Bootstrapper : Bootstrapper<ShellViewModel> {
        protected override void ConfigureIoC(IStyletIoCBuilder builder) {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            var configBuilder = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddEnvironmentVariables()
                                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                        .AddJsonFile($"appsettings.{env}.json", optional: true);

            var config = configBuilder.Build();
            var appSettings = config.Get<AppSettings>();

            // Configure the IoC container in here
            builder.Bind<IApplicationConfig>().ToFactory(container => {
                var converter = new AppSettingsConverter();
                return converter.Convert(appSettings);
            });
        }

        protected override void Configure() {
            // Perform any other configuration before the application starts
        }
    }
}
