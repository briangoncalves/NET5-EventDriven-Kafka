using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using EventDrivenWebAPIExample.Infrastructure.Interface.Mongo;
using MongoDB.Driver;

namespace EventDrivenWebAPIExample.Infrastructure.Mongo
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CertificatePath { get; set; }
        public string CertificateHash { get; set; }
        public IMongoClient Client
        {
            private set { _client = value; }
            get
            {
                if (_client == null)
                    InitializeIMongoClient();
                return _client;
            }
        }
        private IMongoClient _client;

        private void InitializeIMongoClient()
        {
            if (!String.IsNullOrEmpty(CertificatePath))
            {
                var settings = SetupSettingsMongo();
                Client = new MongoClient(settings);
                return;
            }
            if (ConnectionString != null)
            {
                Client = new MongoClient(ConnectionString);
            }
            else
            {
                Client = new MongoClient();
            }

        }

        public MongoDBSettings()
        {
            Client = null;
        }

        MongoClientSettings SetupSettingsMongo()
        {
            var settings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));
            settings.SslSettings = new SslSettings
            {
                EnabledSslProtocols = SslProtocols.Tls12,
                ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return certificate.GetCertHashString() == CertificateHash;
                }
            };
            return settings;
        }

        public IMongoClient CreateClient()
        {
            InitializeIMongoClient();
            return Client;
        }
    }
}
