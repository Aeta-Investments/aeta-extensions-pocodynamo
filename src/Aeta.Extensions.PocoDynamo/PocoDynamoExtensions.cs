using ServiceStack.Aws.DynamoDb;

namespace Aeta.Extensions.PocoDynamo
{
    public static class PocoDynamoExtensions
    {
        public static IPocoDynamo UseTablePrefix(this IPocoDynamo poco, string prefix)
        {
            foreach (var tableMetadata in DynamoMetadata.GetTables()) tableMetadata.Name = prefix + tableMetadata.Name;

            return poco;
        }
    }
}