using HolidaysPB.Api.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Service validation
builder.Host.UseDefaultServiceProvider((context, options) => {
    options.ValidateOnBuild = true;
    options.ValidateScopes = true;
});

// Add services to the container.
builder.Services.AddHolidaysPB(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
