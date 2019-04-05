using System.Threading;

namespace CsLaboratory5.Tools
{
    internal static class Cancellation
    {
        public static CancellationTokenSource RefreshListCancellation { get; set; } = new CancellationTokenSource();
        public static CancellationToken RefreshListToken { get; set; } = RefreshListCancellation.Token;
        public static CancellationTokenSource RefreshMetaDataCancellation { get; set; } = new CancellationTokenSource();
        public static CancellationToken RefreshMetaDataToken { get; set; } = RefreshMetaDataCancellation.Token;
    }
}
