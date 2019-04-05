using System.Threading;

namespace CsLaboratory5.Tools
{
    internal static class Cancellation
    {
        internal static CancellationTokenSource RefreshListCancellation { get; set; } = new CancellationTokenSource();
        internal static CancellationToken RefreshListToken { get; set; } = RefreshListCancellation.Token;
        internal static CancellationTokenSource RefreshMetaDataCancellation { get; set; } = new CancellationTokenSource();
        internal static CancellationToken RefreshMetaDataToken { get; set; } = RefreshMetaDataCancellation.Token;
    }
}
