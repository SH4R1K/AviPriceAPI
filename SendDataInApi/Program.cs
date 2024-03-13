using SendDataInApi.Data;
using ProtoBuf;
using System.Net.Http.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri("http://94.241.169.171:32777/") };
        using (var context = new AviApiContext())
        {
            using (var memoryStream = new MemoryStream())
            {
                foreach (var category in context.CategoryTreePaths.ToList())
                    Serializer.SerializeWithLengthPrefix(memoryStream, category, PrefixStyle.Fixed32);
                var byteArray = memoryStream.ToArray();

                var request = await httpClient.PostAsJsonAsync("/Categories/Update", byteArray);
            }
            using (var memoryStream = new MemoryStream())
            {
                foreach (var category in context.LocationTreePaths.ToList())
                    Serializer.SerializeWithLengthPrefix(memoryStream, category, PrefixStyle.Fixed32);
                var byteArray = memoryStream.ToArray();

                var request = await httpClient.PostAsJsonAsync("/Locations/Update", byteArray);
            }
        }
    }
}