namespace XtraUpload.StorageManager.Common
{
    /// <summary>
    /// Client certificate config
    /// </summary>
    public class ClientCertificateConfig
    {
        /// <summary>
        /// Path to the pfx client certificate
        /// </summary>
        public string PfxPath { get; set; }
        /// <summary>
        /// Passwrd of the pfx cert
        /// </summary>
        public string Password { get; set; }
    }
}
