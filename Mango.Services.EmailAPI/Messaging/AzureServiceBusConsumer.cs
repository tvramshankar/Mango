using System;
using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.DTO;
using Mango.Services.EmailAPI.Service;
using Newtonsoft.Json;

namespace Mango.Services.EmailAPI.Messaging
{
	public class AzureServiceBusConsumer : IAzureServiceBusConsumer
	{
		private readonly string serviceBusConnectionString;
		private readonly string emailCartQueue;
		private readonly IConfiguration _configuration;
		private readonly ServiceBusProcessor _emailCartProcessor;
        private readonly EmailService emailService;

		public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
		{
			_configuration = configuration;
			serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString")!;
			emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue")!;

			var client = new ServiceBusClient(serviceBusConnectionString);
			_emailCartProcessor = client.CreateProcessor(emailCartQueue);
            this.emailService = emailService;
        }

        public async Task Start()
        {
			_emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
			_emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = System.Text.Encoding.UTF8.GetString(message.Body);
            CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(body)!;
            try
            {
                await this.emailService.EmailCartAndLog(cartDTO)!;
                await args.CompleteMessageAsync(args.Message);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}

