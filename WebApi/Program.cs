using WebApi.Services.Dispute;
using WebApi.Services.Excel;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(kestrelOption => {
   kestrelOption.ListenLocalhost(5263);
});

builder.Services.AddScoped<IFileByteImporter, DisputeFileImporter>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

//app.UseCors("development");
    app.UseSwaggerUI();
}
// global cors policy
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials()); // allow credentials

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
