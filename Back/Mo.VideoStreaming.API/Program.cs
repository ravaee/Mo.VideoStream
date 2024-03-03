using Azure.Storage.Blobs;

namespace Mo.VideoStreaming.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddSingleton(x 
            => new BlobServiceClient(builder.Configuration.GetValue<string>("AzureBlobStorage:ConnectionString")));
        
        builder.Services.AddSingleton<BlobService>();
        
        builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

        var app = builder.Build();
        
        app.UseAuthorization();
        app.MapControllers(); 
        app.UseCors("AllowAll"); 
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.Run();
    }
}