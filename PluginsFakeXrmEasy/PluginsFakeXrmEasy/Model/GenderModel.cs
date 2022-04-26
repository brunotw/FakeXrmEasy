using System.Runtime.Serialization;

namespace PluginsFakeXrmEasy.Model
{
    [DataContract]
    public class GenderModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Name = "probability")]
        public double Probability { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }
    }
}
