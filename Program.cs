
using DobirnaGraServer.Hubs;
using DobirnaGraServer.Services;

namespace DobirnaGraServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
			builder.Services.AddSignalR().AddMessagePackProtocol();

			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(policy =>
				{
					policy.AllowAnyOrigin();
					policy.AllowAnyHeader();
					policy.AllowAnyMethod();
				});
			});

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddSingleton<ProfileService>();
			builder.Services.AddSingleton<GameService>();
			builder.Services.AddSingleton<ResourceService>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseCors();

			app.UseAuthorization();

			app.MapControllers().RequireCors();
			app.MapHub<GameHub>("/gamehub");

			app.Run();
		}
	}
}
