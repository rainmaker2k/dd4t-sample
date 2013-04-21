using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Models;

// -----------------------------------------------------------------------
// <copyright file="IPublicationMetaDataProvider.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------



namespace Sample.Website.Providers.Contracts
{
    public interface IPublicationMetaDataProvider
    {
        string RetrieveMetadataValue(string fieldName, string pubMetaDataMapPath);
    }
}
