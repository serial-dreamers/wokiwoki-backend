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

var swaggerEnabled = app.Configuration.GetValue<bool>("Swagger:Enabled");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || swaggerEnabled)
{
	DotEnv.Load();

	//using var scope = app.Services.CreateScope();
	//var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	//var dbContext = scope.ServiceProvider.GetRequiredService<WokiwokiDbContext>();

	//await ApplicationRoleSeeder.SeedAsync(roleManager);
	//await ApplicationCategorySeeder.SeedAsync(dbContext);
	//await ApplicationTagSeeder.SeedAsync(dbContext);

	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization(); 


app.MapControllers();

app.Run();
