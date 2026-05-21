using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Text;
using webResfulAPIs.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// cache response 
builder.Services.AddResponseCaching();

//cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy( policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Địa chỉ của Frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

//sql server
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//authentication 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["Secret:audience"],
        ValidIssuer = builder.Configuration["Secret:issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Secret:keys"]!))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = (context) =>
        {
            var token = context.Request.Cookies["AccessToken"];
            var refreshcheck = context.Request.Cookies["RefreshToken"];
            if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(refreshcheck))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsJsonAsync(new
                {
                    status = 401,
                    message = "Access Token not exist"
                });
            }
            context.Token = token;
            Console.WriteLine($"Token nhận được từ Cookie: {token}");

            return Task.CompletedTask;
        },

        OnAuthenticationFailed = (context) =>
        {
            Console.WriteLine(context.Exception.Message);
            return Task.CompletedTask;
        },

        OnChallenge = (context) =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            Console.WriteLine($"Token het han");
            return context.Response.WriteAsJsonAsync(new
            {
                status = 401,
                error = "unAuthentication"
            });
        },
       OnTokenValidated = (context) =>
       {
           Console.WriteLine($"Token accept");
           
           return Task.CompletedTask;
       }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();


app.UseCors();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

app.Run();
