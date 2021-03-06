using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sklavenwalker.CommonUtilities.DownloadManager.Providers;
using Xunit;

namespace Sklavenwalker.CommonUtilities.DownloadManager.Test.Providers;

public class HttpClientDownloadTest
{
    private readonly MockFileSystem _fileSystem = new();
    private readonly HttpClientDownloader _provider;

    public HttpClientDownloadTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        _provider = new HttpClientDownloader(sc.BuildServiceProvider());
    }

    [Fact]
    public void TestDownloadNotFound()
    {
        var outStream = new MemoryStream();
        var result = _provider.Download(new Uri("https://example.com/test.txt"), outStream, null,
            CancellationToken.None);
        Assert.Equal(0, result.DownloadedSize);
    }

    [Fact]
    public void TestDownload()
    {
        var outStream = new MemoryStream();
        var result = _provider.Download(
            new Uri("https://www.learningcontainer.com/wp-content/uploads/2020/05/sample-zip-file.zip"),
            outStream, null, CancellationToken.None);
        Assert.True(result.DownloadedSize > 0);
        Assert.Equal(result.DownloadedSize, outStream.Length);
    }

    [Fact]
    public void TestDownloadCancelled()
    {
        var outStream = new MemoryStream();
        var cts = new CancellationTokenSource();
        cts.Cancel();
        Assert.Throws<TaskCanceledException>(() =>
            _provider.Download(new Uri("https://example.com/test.txt"), outStream, null, cts.Token));
    }
}