var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Application services
var assembly = typeof(Program).Assembly;
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

// Web API services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularFrontend");

app.Run();

