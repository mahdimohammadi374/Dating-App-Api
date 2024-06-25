using API.Data;
using API.Data.SeedData;
using API.Entities;
using API.Errors;
using API.Helpers;
using API.Interfaces;
using API.Middlewares;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Definations
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
;
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
     .SetIsOriginAllowed((host) => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithOrigins("http://localhost:4200/"));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlServer("Data Source=DESKTOP-C45ID5O\\MAHDI;Initial Catalog=Dating-App;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"));

#endregion

#region Identity
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;

})
    .AddRoles<Role>()
    .AddUserManager<UserManager<User>>()
    .AddRoleManager<RoleManager<Role>>()
    .AddSignInManager<SignInManager<User>>()
    .AddRoleValidator<RoleValidator<Role>>()
    .AddEntityFrameworkStores<DataContext>();
#endregion

#region Cloudinary setting

builder.Services.Configure<CloudinarySettings>(opt =>
{
    opt.ApiKey = "372667769897752";
    opt.ApiSecret = "XXiiusPGq_s-nAgWU4vRXNIWT6E";
    opt.CloudName = "dhb0nknt4";
});

#endregion

#region Validation Error Handling
builder.Services.Configure<ApiBehaviorOptions>(opt =>
opt.InvalidModelStateResponseFactory = actionContext =>
{
    var errors = actionContext.ModelState.Where(x => x.Value.Errors.Count() > 0).SelectMany(x => x.Value.Errors)
    .Select(x => x.ErrorMessage);
    var errorResponse = new ApiValidationErrorResponse
    {
        Errors = errors
    };
    return new BadRequestObjectResult(errorResponse);
}
);
#endregion

#region AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#endregion

#region Swagger Auth Config
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    var xmlFile=$"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath=Path.Combine(AppContext.BaseDirectory, xmlFile);
    option.IncludeXmlComments(xmlPath);
    option.AddSecurityDefinition(
        JwtBearerDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });
    option.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            { new OpenApiSecurityScheme
            { Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            }
            },
                new string[] { } } });
});
#endregion

#region JWT

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("{8CCC4F4C-47CC-4C90-B4CF-C69795A501A2-A8E4E217-0AC5-4E75-B5E9-01317A10D12F}"))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
#endregion

#region IOC
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IUserRepository ,  UserRepository>();
builder.Services.AddScoped<IPhotoService , PhotoService>();
builder.Services.AddScoped<IAccountRepository , AccountRepository>();
builder.Services.AddScoped<IUserLikeRepository , UserLikeRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUnitOfWork ,  UnitOfWork>();
builder.Services.AddScoped<logUserActivity>();
builder.Services.AddSingleton<PresenceTracker>();

#endregion

#region SignalR
builder.Services.AddSignalR();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExeptionHandlerMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(endpoint =>
{
    endpoint.MapHub<PresenceHub>( "/hubs/presence");
    endpoint.MapHub<MessageHub>( "/hubs/messages");
});                                                 
                                                    
#region SeedData
using var scope=app.Services.CreateScope();
var services = scope.ServiceProvider;
var loggerFactory= services.GetRequiredService<ILoggerFactory>();
try
{
    var context=services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();

    await SeedUserData.SeedUsers(context , loggerFactory);
}
catch (Exception ex)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(ex.Message);
}

#endregion


app.Run();
