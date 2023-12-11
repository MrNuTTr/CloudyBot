using Microsoft.AspNetCore.Mvc;

namespace CloudyBot.Models
{
    public class JsonContentResult : ContentResult
    {
        public JsonContentResult(string jsonContent)
        {
            Content = jsonContent;
            ContentType = "application/json";
            StatusCode = 200;
        }

        public JsonContentResult(string jsonContent, int statusCode)
        {
            Content = jsonContent;
            ContentType = "application/json";
            StatusCode = statusCode;
        }
    }
}
