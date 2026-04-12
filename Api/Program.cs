using Api.Services.ChatService;
using Api.Services.ClassifyTextService;
using Api.Services.StatisticsServise;
using Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            string modelPath = "D:\\Мальков 2 курс\\Negative_classifier\\Api\\Data\\dataTest2.json";
            if (File.Exists(modelPath))
            {
                var model = DataModel.Load(modelPath);
                builder.Services.AddSingleton(model);
            }
            builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IClassifyText, ClassifyText>();
            builder.Services.AddScoped<IMessageStats, MessageStats>();
            builder.Services.AddScoped<IChatService, ChatService>();

            var testConn = builder.Configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"Connection String is: {testConn}");

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

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
        }
    }
}
