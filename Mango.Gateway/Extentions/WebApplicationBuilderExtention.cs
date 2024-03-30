using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Mango.Gateway.Extentions
{
	public static class WebApplicationBuilderExtention
	{
		public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
		{
            var secret = builder.Configuration.GetSection("ApiSettings:Secret").Value;
            var issuer = builder.Configuration.GetSection("ApiSettings:Issuer").Value;
            var audience = builder.Configuration.GetSection("ApiSettings:Audience").Value;

            var key = System.Text.Encoding.ASCII.GetBytes(secret!);

            builder.Services.AddAuthentication(optons =>
            {
                optons.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                optons.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience
                };
            });

            return builder;
        }
	}
}

