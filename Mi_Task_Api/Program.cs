using Mi_Task_Api.Authentication;
using Mi_Task_Api.Managers;
using Mi_Task_Api.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
     .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug)
     .WriteTo.MongoDBBson(cfg =>
     {
         cfg.SetConnectionString("mongodb://localhost:27017/Logs");
         cfg.SetCreateCappedCollection(100);
         cfg.SetRollingInternal(Serilog.Sinks.MongoDB.RollingInterval.Day);
     },restrictedToMinimumLevel: LogEventLevel.Error)
    .CreateLogger();

Log.Information("Starting web application");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();
    //builder.Services.AddSerilog();

    builder.Services.AddControllers();
    builder.Services.AddDbContext<UserDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;

    }).AddEntityFrameworkStores<UserDbContext>();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

    builder.Services.AddAuthorization();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IControllerAuthentication, ControllerAuthentication>();
    builder.Services.AddScoped<IFriends, ManagerFriends>();
    builder.Services.AddScoped<ITasks, ManagerTask>();
    builder.Services.AddScoped<IVerifyTask, VerifyNoteBook>();
    builder.Services.AddScoped<IStatus, VerifyNoteBook>();
    builder.Services.AddScoped<INoteBook, ManagerBook>();

    builder.Services.AddSingleton<IAddBlackList, JwtBlackListService>();
    builder.Services.AddSingleton<ICheckBlackList>(sp => (ICheckBlackList)sp.GetRequiredService<IAddBlackList>());
    builder.Services.AddSingleton<IClearBlackList>(sp => (IClearBlackList)sp.GetRequiredService<IAddBlackList>());

    builder.Services.AddHostedService<TokenCleanupService>();



    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();

    app.UseMiddleware<MiddlewareHeader>();

    app.UseAuthentication();
    app.UseAuthorization();



    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}