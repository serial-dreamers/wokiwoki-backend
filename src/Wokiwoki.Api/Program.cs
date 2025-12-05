using dotenv.net;
using Wokiwoki.Api;
using Wokiwoki.Api.Hubs; 

var builder = WebApplication.CreateBuilder(args);

//DI Application Services
builder.AddApplicationServices();
 
//DI Infrastructure Services
builder.AddInfrastructureServices();
 
//DI Web API Services
builder.AddWebAPIServices();



var app = builder.Build(); 

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
app.UseRouting();

app.UseCors("AllowFrontend");


app.UseAuthentication();
app.UseAuthorization();


// Middlewares
app.UseWebAPIMiddlewares();

app.MapControllers();
app.MapHub<BookingHub>("/hubs/booking");

app.Run();
