using System;
using System.Text;
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailAPI.Service
{
	public class EmailService : IEmailService
	{
        private DbContextOptions<DataContext> _dbOptions;

        public EmailService(DbContextOptions<DataContext> dbOptions) 
        {
            _dbOptions = dbOptions;
        }

        //here DbContextOptions is used because
        //we are using EmailService as services.addsigleton in program.cs
        //so we cant use scopped dbcontext(dbcontext is scopped by default) in 
        //singleton EmailService(we registed EmailService as singleton) so we
        //are implementing like this using DbContextOptions

        public async Task EmailCartAndLog(CartDTO cartDto)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<br/>Cart Email Requested");
            message.AppendLine("<br/>Total" + cartDto.CartHeader.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");
            foreach(var item in cartDto.CartDetails!)
            {
                message.Append("<li>");
                message.Append(item.Product!.Name + " x " + item.Count);
                message.Append("</li>");
            }
            message.Append("</ul>");
            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email!);
        }

        public async Task LogOrderPlaced(RewardMessage rewardMessage)
        {
            string message = "New Order Placed. </br> Order ID : " + rewardMessage.OrderId;
            await LogAndEmail(message.ToString(), "ramshankar@gmail.com");
        }

        public async Task RegisterEmailAndLog(string email)
        {
            string message = "User registration successfull. </br> Email : " + email;
            await LogAndEmail(message.ToString(), "ramshankar@gmail.com");
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLogger = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };
                await using var _db = new DataContext(_dbOptions);
                _db.EmailLoggers.Add(emailLogger);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

