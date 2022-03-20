using System.Text.Json.Serialization;

namespace ExpenseManagement.Api.Models
{
    public class ResponseResult
    {
        private readonly int _statusCode;
        public bool Succeeded => _statusCode >= 200 && _statusCode <= 299;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Code { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Items { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Errors { get; set; }
        public ResponseResult(int statusCode = 200)
        {
            _statusCode = statusCode;
        }
        public ResponseResult(object data, int statusCode = 200)
        {
            _statusCode = statusCode;
            Items = data;
        }
        public ResponseResult(int statusCode, object errors)
        {
            _statusCode = statusCode;
            Errors = errors;
        }
        public ResponseResult(string description)
        {
            _statusCode = 200;
            Description = description;
        }
        public ResponseResult(int statusCode, string description)
        {
            _statusCode = statusCode;
            Description = description;
        }
    }
}