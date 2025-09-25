using dotenv.net;

var builder = WebApplication.CreateBuilder(args);
 
//DI Web API Services
builder.AddWebAPIServices();

//DI Application Services
builder.AddApplicationServices();
 
//DI Infrastructure Services
builder.AddInfrastructureServices();

var app = builder.Build();

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
