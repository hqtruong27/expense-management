using System.Text.Json.Serialization;

namespace ExpenseManagement.Api.Model
{
    public class FacebookUserInfoResponse
    {
        public FacebookUserInfoResponse(string surname, string givenName, FacebookPicture picture, string email, string id)
        {
            Surname = surname;
            GivenName = givenName;
            Picture = picture;
            Email = email;
            Id = id;
        }

        [JsonPropertyName("first_name")]
        public string Surname { get; set; }
        [JsonPropertyName("last_name")]
        public string GivenName { get; set; }
        [JsonPropertyName("picture")]
        public FacebookPicture Picture { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class FacebookPicture
    {
        [JsonPropertyName("data")]
        public FacebookPictureData Data { get; set; }
        public FacebookPicture()
        {
            Data = new FacebookPictureData();
        }
    }
    public class FacebookPictureData
    {
        [JsonPropertyName("height")]
        public long Height { get; set; }
        [JsonPropertyName("is_silhouette")]
        public bool IsSilhouette { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("width")]
        public long Width { get; set; }
        public FacebookPictureData()
        {
            Url = string.Empty;
        }
    }

    public class FacebookInfoResponse
    {
        public FacebookInfoResponse(FacebookInfoData data)
        {
            Data = data;
        }

        [JsonPropertyName("data")]
        public FacebookInfoData Data { get; set; }
    }
    public class FacebookInfoData
    {
        public FacebookInfoData(string appId, string type, string application, long dataAccessExpiresAt, long expiresAt, bool isValid, string[] scopes, string userId)
        {
            AppId = appId;
            Type = type;
            Application = application;
            DataAccessExpiresAt = dataAccessExpiresAt;
            ExpiresAt = expiresAt;
            IsValid = isValid;
            Scopes = scopes;
            UserId = userId;
        }

        [JsonPropertyName("app_id")]
        public string AppId { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("aplication")]
        public string Application { get; set; }
        [JsonPropertyName("data_access_expires_at")]
        public long DataAccessExpiresAt { get; set; }
        [JsonPropertyName("expires_at")]
        public long ExpiresAt { get; set; }
        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }
        [JsonPropertyName("scopes")]
        public string[] Scopes { get; set; }
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }
}