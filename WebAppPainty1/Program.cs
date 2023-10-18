using BlogTestApp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using WebAppPainty1;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApiContext>
    (opt => opt.UseInMemoryDatabase("TestDB"));

builder.Services.AddIdentity<User, IdentityRole>()
                 .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApiContext>();






builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApiContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.InitializeAsync(context, userManager, rolesManager);


}


app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
