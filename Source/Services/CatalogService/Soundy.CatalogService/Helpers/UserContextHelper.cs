using Grpc.Core;

namespace Soundy.CatalogService.Helpers
{
    public static class UserContextHelper
    {
        private const string UserIdMetadataKey = "user-id";

        public static Guid? GetUserId(ServerCallContext context)
        {
            var userIdEntry = context.RequestHeaders.FirstOrDefault(h => h.Key == UserIdMetadataKey);
            if (userIdEntry == null || string.IsNullOrEmpty(userIdEntry.Value))
                return null;

            return Guid.TryParse(userIdEntry.Value, out var userId) ? userId : null;
        }
    }
} 