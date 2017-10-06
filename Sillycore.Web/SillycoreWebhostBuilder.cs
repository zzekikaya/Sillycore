﻿using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sillycore.Web
{
    public class SillycoreWebhostBuilder
    {
        private readonly string[] _args;
        private readonly string _applicationName;
        private readonly SillycoreAppBuilder _sillycoreAppBuilder;

        public SillycoreWebhostBuilder(SillycoreAppBuilder sillycoreAppBuilder, string applicationName, string[] args)
        {
            _sillycoreAppBuilder = sillycoreAppBuilder;
            _applicationName = applicationName;
            _args = args;
        }

        public SillycoreWebhostBuilder WithUrl(string rootUrl)
        {
            if (!String.IsNullOrEmpty(rootUrl))
            {
                _sillycoreAppBuilder.DataStore.SetData(Constants.ApiRootUrl, rootUrl.TrimEnd('/'));
            }

            return this;
        }

        public void Build()
        {
            _sillycoreAppBuilder.DataStore.SetData(Constants.ApplicationName, _applicationName);

            _sillycoreAppBuilder.BeforeBuild(() =>
            {
                IWebHost webhost = WebHost.CreateDefaultBuilder(_args)
                    .UseStartup<Startup>()
                    .Build();

                _sillycoreAppBuilder.DataStore.SetData(Constants.WebHost, webhost);
            });

            SillycoreApp app = _sillycoreAppBuilder.Build();

            ServiceProvider serviceProvider = app.DataStore.GetData<ServiceProvider>(Sillycore.Constants.ServiceProvider);
            ILogger<SillycoreWebhostBuilder> logger = serviceProvider.GetService<ILogger<SillycoreWebhostBuilder>>();
            logger.LogInformation($"{_applicationName} started.");

            app.DataStore.GetData<IWebHost>(Constants.WebHost).Run();
        }
    }
}