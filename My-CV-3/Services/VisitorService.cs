using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using My_CV_3.Models;
using System.Threading.Tasks;

namespace My_CV_3.Services
{
    public class VisitorService
    {
        private readonly IMongoCollection<Visitor> _visitorCollection;

        public VisitorService(IConfiguration configuration)
        {
            var mongoSettings = configuration.GetSection("MongoDbSettings");
            var client = new MongoClient(mongoSettings["ConnectionString"]);
            var database = client.GetDatabase(mongoSettings["DatabaseName"]);
            _visitorCollection = database.GetCollection<Visitor>(mongoSettings["VisitorCollectionName"]);
        }

        public async Task RecordVisitAsync(Visitor visitor)
        {
            await _visitorCollection.InsertOneAsync(visitor);
        }

        public async Task<long> GetVisitCountAsync()
        {
            return await _visitorCollection.CountDocumentsAsync(FilterDefinition<Visitor>.Empty);
        }
    }
}