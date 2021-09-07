using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace WebApiMyGalleryPolygon.ContractsImpl
{
    [CollectionDefinition(PolygonClientIntegrationFixture.POLYGON_CLIENT_COLLECTION_DEFAULT)]
    public class PolygonClientCollection : ICollectionFixture<PolygonClientIntegrationFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

}
