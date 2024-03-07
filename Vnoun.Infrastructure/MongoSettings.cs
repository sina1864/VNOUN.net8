namespace Vnoun.Infrastructure;

public class MongoSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string EventsCollectionName { get; set; }
    public string PostsCollectionName { get; set; }
}