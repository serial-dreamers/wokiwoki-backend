using dotenv.net;
using Wokiwoki.Api;

var builder = WebApplication.CreateBuilder(args);

//DI Application Services
builder.AddApplicationServices();
 
//DI Infrastructure Services
builder.AddInfrastructureServices();
 
//DI Web API Services
builder.AddWebAPIServices();

var app = builder.Build();

// Middlewares
app.UseWebAPIMiddlewares();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	DotEnv.Load();

	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization(); 


app.MapControllers();

app.Run();
