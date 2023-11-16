using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using MongoDB.Driver;
using ChildImmunizationCare_Child.Models;
using System.Linq;

namespace ChildImmunizationCare_Child
{
    public class Child
    {
        private readonly IMongoCollection<ChildDetails> _mongoChilds;

        public Child()
        {
            var dbClient = new MongoClient("mongodb+srv://dhivakar:dhivakar@cluster0.rmwyf47.mongodb.net/cluster0");
            IMongoDatabase db = dbClient.GetDatabase("ChildImmunizationCare");
            _mongoChilds = db.GetCollection<ChildDetails>("Child");
        }
        

        [FunctionName("InsertChild")]
        public async Task<IActionResult> InsertChild(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Childs")]
            ChildRequest childRequest
          )
        {

            var childToInsert = new ChildDetails
            {
                Name = childRequest.Name,
                Age = childRequest.Age,
                ParentId = childRequest.ParentId,
                Vaccinated = childRequest.Vaccinated,
            };

            await _mongoChilds.InsertOneAsync(childToInsert);

            return new OkResult();
        }

        [FunctionName("UpdateChild")]
        public async Task<IActionResult> UpdateChild(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Childs/{childId}")]
            ChildRequest childRequest,
            HttpRequest req,
            string childId,
            ILogger log)
        {

            var fields = new Dictionary<string, Object>();

            if (childRequest.Age > 0)
                fields.Add(nameof(ChildDetails.Age), childRequest.Age);

            if (childRequest.Name is not null)
                fields.Add(nameof(ChildDetails.Name), childRequest.Name);

            fields.Add(nameof(ChildDetails.Vaccinated), childRequest.Vaccinated);

            if (childRequest.ParentId is not null)
                fields.Add(nameof(ChildDetails.ParentId), childRequest.ParentId);

            var filter = Builders<ChildDetails>.Filter.Eq(e => e.Id, childId);
            var updates = fields.Select(f => Builders<ChildDetails>.Update.Set(f.Key, f.Value));
            var update = Builders<ChildDetails>.Update.Combine(updates);
            var result = await _mongoChilds.UpdateOneAsync(filter, update);

            return new OkObjectResult(result);
        }

        [FunctionName("GetAllChilds")]
        public async Task<IActionResult> GetAllChilds(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Childs")]
            HttpRequest req,
           ILogger log)
        {

            var cursor = await _mongoChilds.FindAsync(Builders<ChildDetails>.Filter.Empty);
            var list = await cursor.ToListAsync();

            return new OkObjectResult(list);
        }


        [FunctionName("DeleteChild")]
        public async Task<IActionResult> DeleteChild(
         [HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "Childs/{childId}")]
         HttpRequest req,
         string childId,
         ILogger log)
        {
            var filter = Builders<ChildDetails>.Filter
                        .Eq(r => r.Id, childId);

            await _mongoChilds.DeleteOneAsync(filter);

            return new OkResult();
        }


    }
}
