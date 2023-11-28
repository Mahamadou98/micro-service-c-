using System.Collections;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.InitAsync("SearchDb", MongoClientSettings
            .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();
        

        //=============== Taking data from AuctionService Db =====
        using var scope = app.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

        var items = await httpClient.GetItemsForSearchDb();

        System.Console.WriteLine(items.Count + " retourned from the auction service");

        if(items.Count > 0) await DB.SaveAsync(items);


        //==================== Before taking data from AuctionService =====
        
        // var count = await DB.CountAsync<Item>();

        // if(count == 0)
        // {
        //     Console.WriteLine("No data - will attemp to seed");
            
        //     var itemData = await File.ReadAllTextAsync("Data/auctions.json");

        //     var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

        //     var item = JsonSerializer.Deserialize<List<Item>>(itemData, options);

        //     await DB.SaveAsync(item);
        // }
    }
}
