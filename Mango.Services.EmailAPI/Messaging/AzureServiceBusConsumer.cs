using System;
using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.DTO;
using Mango.Services.EmailAPI.Service;
using Newtonsoft.Json;

namespace Mango.Services.EmailAPI.Messaging
{
	public class AzureServiceBusConsumer : IAzureServiceBusConsumer
	{
		private readonly string serviceBusConnectionString;
		private readonly string emailCartQueue;
		private readonly string registerUserQueue;
        private readonly IConfiguration _configuration;
		private readonly ServiceBusProcessor _emailCartProcessor;
		private readonly ServiceBusProcessor _registerUserProcessor;
		private readonly ServiceBusProcessor _emailOrderPlacedProcessor;
        private readonly EmailService emailService;
        private readonly string orderCreated_Topic;
        private readonly string orderCreated_Email_Subscription;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
		{
			_configuration = configuration;
			serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString")!;
			emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue")!;
            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue")!;
            orderCreated_Topic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic")!;
            orderCreated_Email_Subscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription")!;

            var client = new ServiceBusClient(serviceBusConnectionString);
			_emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _registerUserProcessor = client.CreateProcessor(registerUserQueue);
            _emailOrderPlacedProcessor = client.CreateProcessor(orderCreated_Topic, orderCreated_Email_Subscription);
            this.emailService = emailService;
        }

        public async Task Start()
        {
			_emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
			_emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();

            _registerUserProcessor.ProcessMessageAsync += OnregisterUserRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();

            _emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
            _emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailOrderPlacedProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();

            await _emailOrderPlacedProcessor.StopProcessingAsync();
            await _emailOrderPlacedProcessor.DisposeAsync();
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
            catch
            {
                throw;
            }
        }

        private async Task OnregisterUserRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = System.Text.Encoding.UTF8.GetString(message.Body);
            string email = JsonConvert.DeserializeObject<string>(body)!;
            try
            {
                await this.emailService.RegisterEmailAndLog(email)!;
                await args.CompleteMessageAsync(args.Message);
            }
            catch
            {
                throw;
            }
        }

        private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = System.Text.Encoding.UTF8.GetString(message.Body);
            RewardMessage rewardMessage = JsonConvert.DeserializeObject<RewardMessage>(body)!;
            try
            {
                await this.emailService.LogOrderPlaced(rewardMessage)!;
                await args.CompleteMessageAsync(args.Message);
            }
            catch
            {
                throw;
            }
        }
    }
}

