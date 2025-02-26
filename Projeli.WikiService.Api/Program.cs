using System.Reflection;
using Projeli.WikiService.Api.Extensions;
using Projeli.WikiService.Application.Profiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWikiServiceCors(builder.Configuration, builder.Environment);
builder.Services.AddWikiServiceSwagger();
builder.Services.AddWikiServiceServices();
builder.Services.AddWikiServiceRepositories();
builder.Services.AddControllers().AddWikiServiceJson();
builder.Services.AddWikiServiceDatabase(builder.Configuration, builder.Environment);
builder.Services.AddWikiServiceAuthentication(builder.Configuration, builder.Environment);
builder.Services.UseWikiServiceRabbitMq();
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(WikiProfile)));

var app = builder.Build();

app.UseWikiServiceMiddleware();
app.MapControllers();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseWikiServiceSwagger();
}

app.UseWikiServiceCors();
app.UseWikiServiceAuthentication();
app.UseWikiServiceDatabase();

app.Run();