using Domain.Entities;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Persistence;

public sealed class MongoDbContext
{
    public MongoDbContext(IOptions<DatabaseSettings> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        Forms = database.GetCollection<FormDocument>("forms");
        Users = database.GetCollection<UserAccount>("users");
        Uploads = database.GetCollection<UploadRecord>("uploads");

        EnsureIndexes();
    }

    public IMongoCollection<FormDocument> Forms { get; }
    public IMongoCollection<UserAccount> Users { get; }
    public IMongoCollection<UploadRecord> Uploads { get; }

    private void EnsureIndexes()
    {
        var formIndexes = new[]
        {
            new CreateIndexModel<FormDocument>(Builders<FormDocument>.IndexKeys.Text("Title").Text("Keywords")),
            new CreateIndexModel<FormDocument>(Builders<FormDocument>.IndexKeys.Ascending(x => x.State).Ascending(x => x.Department).Ascending(x => x.Category)),
            new CreateIndexModel<FormDocument>(Builders<FormDocument>.IndexKeys.Ascending(x => x.IsLatest))
        };
        Forms.Indexes.CreateMany(formIndexes);
        Users.Indexes.CreateOne(new CreateIndexModel<UserAccount>(Builders<UserAccount>.IndexKeys.Ascending(x => x.Email), new CreateIndexOptions { Unique = true }));
        Uploads.Indexes.CreateOne(new CreateIndexModel<UploadRecord>(Builders<UploadRecord>.IndexKeys.Ascending(x => x.Status).Ascending(x => x.CreatedAt)));
    }
}
