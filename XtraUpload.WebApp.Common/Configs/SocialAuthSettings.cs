namespace XtraUpload.WebApp.Common
{
    /// <summary>
    /// Social media auth providers
    /// </summary>
    public class SocialAuthSettings
    {
        public FacebookAuth FacebookAuth { get; set; }
        public GoogleAuth GoogleAuth { get; set; }
    }

    public class FacebookAuth
    {
        public string AppId { get; set; }
    }

    public class GoogleAuth
    {
        public string ClientId { get; set; }
    }
}
