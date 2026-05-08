var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Application services
var assembly = typeof(Program).Assembly;
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(assembly));

// Infrastracture services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularFrontend",
        policy => policy
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader());
});
builder.Services.AddTransient<ISqlConnectionFactory, SqlConnectionFactory>();

// Web API services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapCarter();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var secret = app.Configuration["Jwt:Secret"];
    if (string.IsNullOrEmpty(secret) || secret == "SuperSecretKeyForEcommerceSliceAppPracticalExercise!@#!")
    {
        Console.WriteLine("SECURITY NOTE: Using a hardcoded JWT Secret for convenince of testing.");
        Console.WriteLine("In production, this would be managed via Environment Variables.");
    }
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularFrontend");

app.Run();

