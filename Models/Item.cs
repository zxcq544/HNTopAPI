using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace HNTopAPI.Models
{
    public class Item
    {
        public int Id { get; set; }

        [JsonPropertyName("by")]
        public string Author { get; set; }
        public int Descendants { get; set; }

        public int Score { get; set; }
        [JsonPropertyName("time")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DateTime { get; set; }


        public string Title { get; set; }
        [JsonIgnore]
        public string Type { get; set; }
        public string Url { get; set; }

        public override string ToString()

        {
            return $"{Id} {Author} {Score} {Url}";
        }
    }

}
