using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ChildImmunizationCare_Child.Models
{
    public class ChildRequest
    {
      
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("vaccinated")]
        public bool Vaccinated { get; set; }

        [BsonElement("vaccinatedByDoctorId")]
        public string vaccinatedByDoctorId { get; set; }

        [BsonElement("vaccinatedByDoctorName")]
        public string vaccinatedByDoctorName { get; set; }
    }

}
