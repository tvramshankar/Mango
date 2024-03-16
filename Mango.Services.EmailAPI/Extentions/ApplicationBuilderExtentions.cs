﻿using System;
using System.Reflection.Metadata;
using Mango.Services.EmailAPI.Messaging;
using Microsoft.AspNetCore.Builder;

namespace Mango.Services.EmailAPI.Extentions
{
	public static class ApplicationBuilderExtentions
	{
        private static IAzureServiceBusConsumer azureServiceBusConsumer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            azureServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>()!;
            var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLife!.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopped.Register(OnStop);
            return app;
        }

        private static void OnStart()
        {
            azureServiceBusConsumer.Start();
        }

        private static void OnStop()
        {
            azureServiceBusConsumer.Stop();
        }
    }
}

