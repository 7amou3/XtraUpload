namespace XtraUpload.Authentication.Service.Common
{
    public class JwtToken
    {
        public string Token { get; set; }
        /// <summary>
        /// Token expiration time (in seconds)
        /// </summary>
        public int Expires_in { get; set; }
    }
}
