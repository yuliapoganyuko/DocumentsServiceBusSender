using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DocumentsServiceBusSender.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DocumentsServiceBusSender
{
    public class DocumentSender
    {
        private readonly string _defaultPropertyName = "type";
        private readonly string _defaultPropertyValue = "not specified";

        private readonly ILogger _logger;
        private ServiceBusService _serviceBusService;
        private JsonHandler _jsonHandler;

        public DocumentSender(ILoggerFactory loggerFactory, ServiceBusService serviceBusService, JsonHandler jsonHandler)
        {
            _serviceBusService = serviceBusService;
            _jsonHandler = jsonHandler;
            _logger = loggerFactory.CreateLogger<DocumentSender>();
        }

        [Function("sendDocument")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "send")] HttpRequestData req)
        {
            _logger.LogInformation("Send Document function processed a request.");

            var requestBody = await req.ReadAsStringAsync();

            bool requestBodyValid = _jsonHandler.TryParse(requestBody, out JObject messageAsJson);
            if (!requestBodyValid)
                return new BadRequestObjectResult("Provide valid non-empty json in the request body");

            var propertyValue = _jsonHandler.GetPropertyValue(messageAsJson, _defaultPropertyName) ?? _defaultPropertyValue;
            
            await _serviceBusService.SendMessageWithProperty(requestBody, _defaultPropertyName, propertyValue);

            return new OkObjectResult(requestBody);
        }
    }
}
