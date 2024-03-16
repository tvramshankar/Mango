using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Extentions;
using Mango.Services.ShoppingCartAPI.Service;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Mango.Services.ShoppingCartAPI.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as follows: 'Bearer Genereted-JWT_Token'",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme,
                }
            }, new string[]{}
        }
    });
});


builder.AddAppAuthentication();
builder.Services.AddAuthorization();

var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options => options.UseMySQL(ConnectionString!));
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddHttpContextAccessor(); // to inject httpcontect accessor
builder.Services.AddScoped<BackEndApiAuthenticationHttpClientHandler>();
builder.Services.AddHttpClient("Product", u => u.BaseAddress =
    new Uri(builder.Configuration.GetSection("ServiceUrls:ProductAPI").Value!)).AddHttpMessageHandler<BackEndApiAuthenticationHttpClientHandler>(); // to add/set the token captured from http request to new http request to product api
builder.Services.AddHttpClient("Coupon", u => u.BaseAddress =
    new Uri(builder.Configuration.GetSection("ServiceUrls:CouponAPI").Value!)).AddHttpMessageHandler<BackEndApiAuthenticationHttpClientHandler>(); /// to add/set the token captured from http request to new http request to product api
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IMessageBus, MessageBus>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();

void ApplyMigration()
{

    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<DataContext>();
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}


