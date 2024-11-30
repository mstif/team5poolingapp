using MassTransit;
using MongoDB.Bson;
using MongoDB.Driver;
using Services.Contracts;

namespace json.service.Consumers
{
    public class DeliveryContractConsumer : IConsumer<DeliveryContractDto>
    {
        private readonly MongoClient mongoClient;

        public DeliveryContractConsumer(
            MongoClient mongoClient)
        {
            this.mongoClient = mongoClient;
        }

        public async Task Consume(ConsumeContext<DeliveryContractDto> context)
        {
            IMongoDatabase db = mongoClient.GetDatabase("mongoDocumentsDb");
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>("deliverycontracts");

            DeliveryContractDto dto = context.Message;
            BsonDocument document = new()
            {
                {"Id",  dto.Id}
            };
            await collection.InsertOneAsync(document);

            Console.WriteLine(document);

            IMongoCollection<DeliveryContractDto> collectionDto = db.GetCollection<DeliveryContractDto>("deliverycontractsDto");
          
            await collectionDto.InsertOneAsync(dto);
            //var r = await collection.Find(new BsonDocument()).ToListAsync();
            //var rr = await collectionDto.Find(new BsonDocument()).ToListAsync();
        }
    }
}
