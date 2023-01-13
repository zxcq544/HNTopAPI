using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace HNTopAPI.Models
{
    public class Item
    {
        public int id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int[] kids { get; set; }

        [JsonPropertyName("by")]
        public string author { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int descendants { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int score { get; set; }
        [JsonPropertyName("time")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DateTime { get; set; }
        public string text { get; set; }

        public int parent { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string title { get; set; }
        [JsonIgnore]
        public string type { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string url { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? dead { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? deleted { get; set; }
        public int? num_kids { get; set; }
        public bool? show_kids { get; set; }

        public override string ToString()

        {
            return $"descendants={descendants}  num_kids={num_kids}";
        }
    }

}
