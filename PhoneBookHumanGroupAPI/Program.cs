using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.EntityFrameworkCore;
using PhoneBookHumanGroupAPI.Models;
using PhoneBookHumanGroupBL.EmailSenderManager;
using PhoneBookHumanGroupBL.ImplementationsOfManagers;
using PhoneBookHumanGroupBL.InterfacesOfManagers;
using PhoneBookHumanGroupDL.ContextInfo;
using PhoneBookHumanGroupDL.ImplementationofRepos;
using PhoneBookHumanGroupDL.InterfacesofRepos;
using PhoneBookHumanGroupEL.Mappings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// serilog logger ayarlari

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


//contexti ayarliyoruz.
builder.Services.AddDbContext<PhoneBookHumanGroupContext>(options =>
{
    //klasik mvcde connection string web configte yer alir.
    //core mvcde connection string appsetting.json dosyasindan alinir.
    options.UseSqlServer(builder.Configuration.GetConnectionString("Betul"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

});


builder.Services.AddAutoMapper(x =>
{
    x.AddExpressionMapping();
    x.AddProfile<Maps>();
    x.AddProfile<MapForAPI>();


});

//interfaceleri gerceklestirecek classlarin burada yasam donguleri tanimlanir
builder.Services.AddScoped<IMemberRepo, MemberRepo>();
builder.Services.AddScoped<IMemberManager, MemberManager>();



builder.Services.AddScoped<IMemberPhoneRepo, MemberPhoneRepo>();
builder.Services.AddScoped<IMemberPhoneManager, MemberPhoneManager>();


builder.Services.AddScoped<IPhoneGroupRepo, PhoneGroupRepo>();
builder.Services.AddScoped<IPhoneGroupManager, PhoneGroupManager>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
