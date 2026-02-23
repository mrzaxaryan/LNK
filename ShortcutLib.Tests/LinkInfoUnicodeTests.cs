using ShortcutLib;
using Xunit;

namespace ShortcutLib.Tests;

public class LinkInfoUnicodeTests
{
    [Fact]
    public void Unicode_LocalPath_RoundTrips()
    {
        string japanesePath = @"C:\Users\テスト\Documents\ファイル.txt";
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = japanesePath,
            UseUnicodeLinkInfo = true,
            LinkInfo = new LinkInfo
            {
                Local = new LocalPathInfo
                {
                    BasePath = japanesePath,
                    VolumeLabel = "テスト"
                }
            }
        });

        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.LinkInfo?.Local);
        Assert.Equal(japanesePath, options.LinkInfo.Local.BasePath);
        Assert.Equal("テスト", options.LinkInfo.Local.VolumeLabel);
    }

    [Fact]
    public void Unicode_Header_Is0x24()
    {
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\Users\тест\file.txt",
            UseUnicodeLinkInfo = true,
            LinkInfo = new LinkInfo
            {
                Local = new LocalPathInfo { BasePath = @"C:\Users\тест\file.txt" }
            }
        });

        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.LinkInfo?.Local);
        Assert.Equal(@"C:\Users\тест\file.txt", options.LinkInfo.Local.BasePath);
    }

    [Fact]
    public void Unicode_NetworkShare_RoundTrips()
    {
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"\\サーバー\共有\ドキュメント.docx",
            UseUnicodeLinkInfo = true,
            LinkInfo = new LinkInfo
            {
                Network = new NetworkPathInfo
                {
                    ShareName = @"\\サーバー\共有",
                    CommonPathSuffix = "ドキュメント.docx",
                    DeviceName = "Z:"
                }
            }
        });

        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.LinkInfo?.Network);
        Assert.Equal(@"\\サーバー\共有", options.LinkInfo.Network.ShareName);
        Assert.Equal("ドキュメント.docx", options.LinkInfo.Network.CommonPathSuffix);
        Assert.Equal("Z:", options.LinkInfo.Network.DeviceName);
    }

    [Fact]
    public void AutoDetect_NonAscii_UsesUnicode()
    {
        string cyrillicPath = @"C:\Документы\файл.txt";
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = cyrillicPath,
            // UseUnicodeLinkInfo = null (auto-detect)
            LinkInfo = new LinkInfo
            {
                Local = new LocalPathInfo { BasePath = cyrillicPath }
            }
        });

        var options = Shortcut.Open(lnk);
        Assert.NotNull(options.LinkInfo?.Local);
        Assert.Equal(cyrillicPath, options.LinkInfo.Local.BasePath);
    }

    [Fact]
    public void AutoDetect_AsciiOnly_UsesAnsi()
    {
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\Windows\notepad.exe",
            // UseUnicodeLinkInfo = null (auto-detect, ASCII = ANSI)
            LinkInfo = new LinkInfo
            {
                Local = new LocalPathInfo
                {
                    BasePath = @"C:\Windows\notepad.exe",
                    VolumeLabel = "Windows"
                }
            }
        });

        // Verify binary uses 0x1C header (ANSI)
        var options = Shortcut.Open(lnk);
        Assert.Equal(@"C:\Windows\notepad.exe", options.LinkInfo?.Local?.BasePath);
        Assert.Equal("Windows", options.LinkInfo?.Local?.VolumeLabel);
    }

    [Fact]
    public void Unicode_VolumeLabel_RoundTrips()
    {
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\file.txt",
            UseUnicodeLinkInfo = true,
            LinkInfo = new LinkInfo
            {
                Local = new LocalPathInfo
                {
                    BasePath = @"C:\file.txt",
                    VolumeLabel = "Données"
                }
            }
        });

        var options = Shortcut.Open(lnk);
        Assert.Equal("Données", options.LinkInfo?.Local?.VolumeLabel);
    }

    [Fact]
    public void ForceFalse_UsesAnsiEvenForNonAscii()
    {
        // When explicitly set to false, ANSI header is used (may lose data)
        byte[] lnk = Shortcut.Create(new ShortcutOptions
        {
            Target = @"C:\test.exe",
            UseUnicodeLinkInfo = false,
            LinkInfo = new LinkInfo
            {
                Local = new LocalPathInfo
                {
                    BasePath = @"C:\test.exe",
                    VolumeLabel = "Test"
                }
            }
        });

        var options = Shortcut.Open(lnk);
        Assert.Equal(@"C:\test.exe", options.LinkInfo?.Local?.BasePath);
    }
}
