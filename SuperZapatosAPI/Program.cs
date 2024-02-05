using DB;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Inyección de dependencia SuperZapatosContext
builder.Services.AddDbContext<SuperZapatosContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SuperZapatosConnection"));
});

var app = builder.Build();

//Migracion en base de datos
//using (var scope = app.Services.CreateScope())
//{
//    var dataContext = scope.ServiceProvider.GetRequiredService<SuperZapatosContext>();
//    dataContext.Database.Migrate();
//}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
