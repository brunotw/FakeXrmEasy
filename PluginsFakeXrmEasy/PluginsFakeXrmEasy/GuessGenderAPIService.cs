using PluginsFakeXrmEasy.Interfaces;
using System.Net.Http;

namespace PluginsFakeXrmEasy
{
    public class GuessGenderAPIService : IGuessGenderAPI
    {
        private readonly string Endpoint = "https://api.genderize.io?name=";

        public string GuessGenderBasedOnName(string name)
        {
            using (var client = new HttpClient())
            {
                var url = Endpoint + name;
                var result = client.GetStringAsync(url).Result;
                return result;
            }
        }
    }
}
