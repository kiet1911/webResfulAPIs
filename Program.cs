using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Unicode;
using webResfulAPIs.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//cors
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(p =>
//    {
//        p.WithOrigins("https://google.com");
//    });
//});

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
        ValidateIssuer = true ,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["Secret:audience"],
        ValidIssuer = builder.Configuration["Secret:issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Secret:keys"]!))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = (context) =>
        {

            var token = context.Request.Cookies["accessToken"];
            context.Token = token;
            Console.WriteLine($"Token nhận được từ Cookie: {token}"); // Kiểm tra xem có bị dư dấu "" không

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

            return context.Response.WriteAsJsonAsync(new
            {
                status = 401,
                error = "unAuthentication"
            });
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

app.UseRouting();
app.UseHttpsRedirection();

//app.UseCors();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

app.Run();
