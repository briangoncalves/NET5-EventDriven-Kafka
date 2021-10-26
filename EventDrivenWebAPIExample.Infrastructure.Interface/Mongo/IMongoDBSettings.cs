﻿using MongoDB.Driver;

namespace EventDrivenWebAPIExample.Infrastructure.Interface.Mongo
{
    public interface IMongoDBSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string CertificatePath { get; set; }
        string CertificateHash { get; set; }
        IMongoClient CreateClient();
    }
}
