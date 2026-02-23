namespace ShortcutLib;

/// <summary>
/// Well-known network provider type constants (WNNC_NET_*) per [MS-SHLLINK] 2.3.2.
/// </summary>
public static class NetworkProviderTypes
{
    /// <summary>SMB / CIFS (Microsoft Windows Network).</summary>
    public const uint Lanman = 0x00020000;

    /// <summary>Novell NetWare.</summary>
    public const uint Netware = 0x00030000;

    /// <summary>Sun PC NFS.</summary>
    public const uint SunPcNfs = 0x00070000;

    /// <summary>Banyan VINES.</summary>
    public const uint Vines = 0x000D0000;

    /// <summary>Avid Technology network.</summary>
    public const uint Avid = 0x001A0000;

    /// <summary>DocuSpace network.</summary>
    public const uint Docuspace = 0x001B0000;

    /// <summary>MangoSoft network.</summary>
    public const uint Mangosoft = 0x001C0000;

    /// <summary>Sernet network.</summary>
    public const uint Sernet = 0x001D0000;

    /// <summary>Riverfront1 network.</summary>
    public const uint Riverfront1 = 0x001E0000;

    /// <summary>Riverfront2 network.</summary>
    public const uint Riverfront2 = 0x001F0000;

    /// <summary>Decorb network.</summary>
    public const uint Decorb = 0x00200000;

    /// <summary>ProtstoR network.</summary>
    public const uint Protstor = 0x00210000;

    /// <summary>Fujitsu redirector.</summary>
    public const uint FjRedir = 0x00220000;

    /// <summary>Distinct network.</summary>
    public const uint Distinct = 0x00230000;

    /// <summary>Twins network.</summary>
    public const uint Twins = 0x00240000;

    /// <summary>RDR2 sample network.</summary>
    public const uint Rdr2Sample = 0x00250000;

    /// <summary>Client Side Caching.</summary>
    public const uint Csc = 0x00260000;

    /// <summary>3-In-1 network.</summary>
    public const uint ThreeIn1 = 0x00270000;

    /// <summary>ExtendNet network.</summary>
    public const uint ExtendNet = 0x00290000;

    /// <summary>Stac network.</summary>
    public const uint Stac = 0x002A0000;

    /// <summary>Foxbat network.</summary>
    public const uint Foxbat = 0x002B0000;

    /// <summary>Yahoo! network.</summary>
    public const uint Yahoo = 0x002C0000;

    /// <summary>Exifs network.</summary>
    public const uint Exifs = 0x002D0000;

    /// <summary>WebDAV (DAV).</summary>
    public const uint Dav = 0x002E0000;

    /// <summary>Knoware network.</summary>
    public const uint Knoware = 0x002F0000;

    /// <summary>Object Directories network.</summary>
    public const uint ObjectDire = 0x00300000;

    /// <summary>MAS FAX network.</summary>
    public const uint Masfax = 0x00310000;

    /// <summary>HOB NFS.</summary>
    public const uint HobNfs = 0x00320000;

    /// <summary>Shiva network.</summary>
    public const uint Shiva = 0x00330000;

    /// <summary>IBM AS/400 network.</summary>
    public const uint Ibmal = 0x00340000;

    /// <summary>Lock network.</summary>
    public const uint Lock = 0x00350000;

    /// <summary>Terminal Services.</summary>
    public const uint TerminalServices = 0x00360000;

    /// <summary>SRT network.</summary>
    public const uint Srt = 0x00370000;

    /// <summary>Quincy network.</summary>
    public const uint Quincy = 0x00380000;

    /// <summary>OpenAFS.</summary>
    public const uint OpenAfs = 0x00390000;

    /// <summary>Avid1 network.</summary>
    public const uint Avid1 = 0x003A0000;

    /// <summary>Distributed File System.</summary>
    public const uint Dfs = 0x003B0000;

    /// <summary>KWNP network.</summary>
    public const uint Kwnp = 0x003C0000;

    /// <summary>Novell ZENworks.</summary>
    public const uint Zenworks = 0x003D0000;

    /// <summary>DriveOnWeb network.</summary>
    public const uint DriveOnWeb = 0x003E0000;

    /// <summary>VMware shared folders.</summary>
    public const uint VMware = 0x003F0000;

    /// <summary>RSFX network.</summary>
    public const uint Rsfx = 0x00400000;

    /// <summary>M-Files network.</summary>
    public const uint Mfiles = 0x00410000;

    /// <summary>Microsoft NFS.</summary>
    public const uint MsNfs = 0x00420000;

    /// <summary>Google file system.</summary>
    public const uint Google = 0x00430000;
}
