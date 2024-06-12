var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<NuGet.Server.Core.Configuration.NuGetServerOptions>(options =>
{
    options.PackagePath = builder.Configuration["NuGet:PackagesPath"];
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.UseStaticFiles();//±Ò¥ÎÀRºA
app.UseDirectoryBrowser();//±Ò¥Î¥Ø¿ý

app.Run();
