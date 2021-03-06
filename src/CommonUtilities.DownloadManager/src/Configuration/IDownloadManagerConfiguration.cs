using System.Net.Http;

namespace Sklavenwalker.CommonUtilities.DownloadManager.Configuration;

/// <summary>
/// Configures the behavior of an <see cref="IDownloadManager"/>
/// </summary>
public interface IDownloadManagerConfiguration
{
    /// <summary>
    /// The delay in ms between a download retry.
    /// </summary>
    int DownloadRetryDelay { get; set; }

    /// <summary>
    /// Specifies whether it's legal to download empty files.
    /// An <see cref="IDownloadManager"/> may react with an error when downloading
    /// an empty file with this options set to <see langword="false"/>.
    /// </summary>
    bool AllowEmptyFileDownload { get; set; }

    /// <summary>
    /// Specifies how verification after the download shall be handled.
    /// </summary>
    VerificationPolicy VerificationPolicy { get; set; }

    /// <summary>
    /// The <see cref="Configuration.InternetClient"/> implementation which shall get used.
    /// Default is <see cref="HttpClient"/>
    /// </summary>
    InternetClient InternetClient { get; set; }
}