using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DocumentsServiceBusSender.Core
{
    public class JsonHandler
    {
        public bool TryParse(string input, out JObject? parsedObject)
        {
            try
            {
                parsedObject = JObject.Parse(input);
                return true;
            }
            catch (JsonReaderException e)
            {
                parsedObject = null;
                return false;
            }
        }

        public string? GetPropertyValue(JObject jsonObject, string propertyName)
        {
            return jsonObject.GetValue(propertyName)?.ToString();
        }
    }
}
