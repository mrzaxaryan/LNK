using System.Text;

namespace ShortcutLib;

/// <summary>
/// Builds a serialized MS-PROPSTORE byte array for use with
/// <see cref="ShortcutOptions.PropertyStoreData"/>.
/// </summary>
public sealed class PropertyStoreBuilder
{
    // Well-known format IDs
    private static readonly Guid AppUserModelFormatId =
        new("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3");

    private static readonly Guid LinkFormatId =
        new("B9B4B3FC-2B51-4A42-B5D8-324146AFCF25");

    private static readonly Guid SystemFormatId =
        new("B725F130-47EF-101A-A5F1-02608C9EEBAC");

    private static readonly Guid NamedPropertyFormatId =
        new("D5CDD505-2E9C-101B-9397-08002B2CF9AE");

    // Property IDs under AppUserModelFormatId
    private const uint PID_RelaunchCommand = 2;
    private const uint PID_RelaunchIconResource = 3;
    private const uint PID_RelaunchDisplayNameResource = 4;
    private const uint PID_AppUserModelId = 5;
    private const uint PID_IsDestListSeparator = 6;
    private const uint PID_ExcludeFromShowInNewInstall = 8;
    private const uint PID_PreventPinning = 9;
    private const uint PID_ToastActivatorCLSID = 26;

    // Property IDs under LinkFormatId
    private const uint PID_TargetParsingPath = 2;
    private const uint PID_TargetSFGAOFlags = 8;

    // Property IDs under SystemFormatId
    private const uint PID_ItemTypeText = 4;
    private const uint PID_MimeType = 26;

    // Additional format IDs
    private static readonly Guid TargetUrlFormatId =
        new("5CBF2787-48CF-4208-B90E-EE5E5D420294");

    private static readonly Guid TargetExtensionFormatId =
        new("7A7D76F4-B630-4BD7-95FF-37CC51A975C9");

    private static readonly Guid WinXFormatId =
        new("FB8D2D7B-90D1-4E34-BF60-6EAC09922BBF");

    // Additional AppUserModel PIDs
    private const uint PID_IsDestListLink = 7;
    private const uint PID_BestShortcut = 10;
    private const uint PID_IsDualMode = 11;
    private const uint PID_StartPinOption = 12;
    private const uint PID_PackageRelativeApplicationID = 13;
    private const uint PID_HostEnvironment = 14;
    private const uint PID_PackageFamilyName = 15;
    private const uint PID_PackageFullName = 16;
    private const uint PID_PackageInstallPath = 17;
    private const uint PID_InstalledBy = 18;
    private const uint PID_RecordState = 19;
    private const uint PID_ParentID = 20;
    private const uint PID_Relevance = 21;
    private const uint PID_DestListProvidedTitle = 22;
    private const uint PID_DestListProvidedDescription = 23;
    private const uint PID_DestListProvidedGroupName = 24;
    private const uint PID_DestListLogoUri = 25;
    private const uint PID_RunFlags = 27;
    private const uint PID_ActivationContext = 28;
    private const uint PID_VisualElementsManifestHintPath = 29;
    private const uint PID_ExcludedFromLauncher = 30;
    private const uint PID_FeatureOnDemand = 31;
    private const uint PID_TileUniqueId = 32;

    // Additional System.Link PIDs
    private const uint PID_LinkComment = 3;
    private const uint PID_DateVisited = 4;
    private const uint PID_FeedUrl = 5;
    private const uint PID_LinkStatus = 6;

    // WinX PID
    private const uint PID_WinXHash = 2;

    // VT type constants from MS-OLEPS
    private const ushort VT_I2 = 2;
    private const ushort VT_I4 = 3;
    private const ushort VT_BOOL = 11;
    private const ushort VT_UI2 = 18;
    private const ushort VT_UI4 = 19;
    private const ushort VT_I8 = 20;
    private const ushort VT_UI8 = 21;
    private const ushort VT_LPSTR = 30;
    private const ushort VT_LPWSTR = 31;
    private const ushort VT_FILETIME = 64;
    private const ushort VT_BLOB = 65;
    private const ushort VT_CLSID = 72;

    // Named property accumulator
    private readonly List<(string name, byte[] serializedValue)> _namedProperties = new();

    // --- AppUserModel properties ---

    /// <summary>AppUserModel.ID (string). Controls taskbar grouping, jump lists, and toast notifications.</summary>
    public string? AppUserModelId { get; set; }

    /// <summary>Toast activator CLSID for COM notification activation.</summary>
    public Guid? ToastActivatorCLSID { get; set; }

    /// <summary>Prevents the shortcut from being pinned to taskbar or Start.</summary>
    public bool? PreventPinning { get; set; }

    /// <summary>Relaunch command for taskbar pinning.</summary>
    public string? RelaunchCommand { get; set; }

    /// <summary>Display name resource for relaunch.</summary>
    public string? RelaunchDisplayNameResource { get; set; }

    /// <summary>Icon resource for relaunch.</summary>
    public string? RelaunchIconResource { get; set; }

    /// <summary>Excludes from "New programs" highlight in Start Menu.</summary>
    public bool? ExcludeFromShowInNewInstall { get; set; }

    /// <summary>Marks this as a destination list separator in a jump list.</summary>
    public bool? IsDestListSeparator { get; set; }

    /// <summary>Marks this as a destination list link.</summary>
    public bool? IsDestListLink { get; set; }

    /// <summary>Marks as the best shortcut for the application.</summary>
    public bool? BestShortcut { get; set; }

    /// <summary>Enables dual-mode (desktop + immersive) for the application.</summary>
    public bool? IsDualMode { get; set; }

    /// <summary>Start pin option: 0=Default, 1=NoPinOnInstall, 2=UserPinned.</summary>
    public uint? StartPinOption { get; set; }

    /// <summary>Package-relative application ID for packaged apps.</summary>
    public string? PackageRelativeApplicationID { get; set; }

    /// <summary>Host environment identifier.</summary>
    public uint? HostEnvironment { get; set; }

    /// <summary>Package family name for UWP/MSIX apps.</summary>
    public string? PackageFamilyName { get; set; }

    /// <summary>Full package name for UWP/MSIX apps.</summary>
    public string? PackageFullName { get; set; }

    /// <summary>Package install path.</summary>
    public string? PackageInstallPath { get; set; }

    /// <summary>Application installed by identifier.</summary>
    public string? InstalledBy { get; set; }

    /// <summary>Record state value.</summary>
    public uint? RecordState { get; set; }

    /// <summary>Parent application ID.</summary>
    public string? ParentID { get; set; }

    /// <summary>Relevance score.</summary>
    public uint? Relevance { get; set; }

    /// <summary>Destination list provided title.</summary>
    public string? DestListProvidedTitle { get; set; }

    /// <summary>Destination list provided description.</summary>
    public string? DestListProvidedDescription { get; set; }

    /// <summary>Destination list provided group name.</summary>
    public string? DestListProvidedGroupName { get; set; }

    /// <summary>Destination list logo URI.</summary>
    public string? DestListLogoUri { get; set; }

    /// <summary>Run flags for the application.</summary>
    public uint? RunFlags { get; set; }

    /// <summary>Activation context string.</summary>
    public string? ActivationContext { get; set; }

    /// <summary>Path hint for visual elements manifest.</summary>
    public string? VisualElementsManifestHintPath { get; set; }

    /// <summary>Excludes from launcher/Start screen.</summary>
    public bool? ExcludedFromLauncher { get; set; }

    /// <summary>Marks as feature-on-demand.</summary>
    public bool? FeatureOnDemand { get; set; }

    /// <summary>Unique tile identifier.</summary>
    public string? TileUniqueId { get; set; }

    // --- System.Link properties ---

    /// <summary>System.Link.TargetParsingPath — canonical parsing path to the target.</summary>
    public string? TargetParsingPath { get; set; }

    /// <summary>System.Link.TargetSFGAOFlags — shell attributes of the target.</summary>
    public uint? TargetSFGAOFlags { get; set; }

    /// <summary>System.Link.Comment — link comment.</summary>
    public string? LinkComment { get; set; }

    /// <summary>System.Link.DateVisited — date the link was last visited.</summary>
    public DateTime? DateVisited { get; set; }

    /// <summary>System.Link.FeedUrl — feed URL for the link.</summary>
    public string? FeedUrl { get; set; }

    /// <summary>System.Link.Status — link status value.</summary>
    public int? LinkStatus { get; set; }

    /// <summary>System.Link.TargetUrl (separate format ID: 5CBF2787-...).</summary>
    public string? TargetUrl { get; set; }

    /// <summary>System.Link.TargetExtension (separate format ID: 7A7D76F4-...).</summary>
    public string? TargetExtension { get; set; }

    // --- WinX menu hash ---

    /// <summary>WinX power user menu hash. Use <see cref="WinXHasher.ComputeHash"/> to compute.</summary>
    public uint? WinXHash { get; set; }

    // --- System properties ---

    /// <summary>System.ItemTypeText — file type description (e.g. "Text Document").</summary>
    public string? ItemTypeText { get; set; }

    /// <summary>System.MIMEType — MIME type of the target (e.g. "text/plain").</summary>
    public string? MimeType { get; set; }

    // --- Arbitrary named property methods ---

    /// <summary>
    /// Adds a custom string property by name.
    /// Uses the string-named property format ({D5CDD505-...}).
    /// </summary>
    public PropertyStoreBuilder AddNamedStringProperty(string name, string value)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);
        _namedProperties.Add((name, SerializeString(value)));
        return this;
    }

    /// <summary>
    /// Adds a custom unsigned 32-bit integer property by name.
    /// Uses the string-named property format ({D5CDD505-...}).
    /// </summary>
    public PropertyStoreBuilder AddNamedUInt32Property(string name, uint value)
    {
        ArgumentNullException.ThrowIfNull(name);
        _namedProperties.Add((name, SerializeUInt32(value)));
        return this;
    }

    /// <summary>
    /// Adds a custom boolean property by name.
    /// Uses the string-named property format ({D5CDD505-...}).
    /// </summary>
    public PropertyStoreBuilder AddNamedBoolProperty(string name, bool value)
    {
        ArgumentNullException.ThrowIfNull(name);
        _namedProperties.Add((name, SerializeBool(value)));
        return this;
    }

    /// <summary>Adds a custom 16-bit signed integer property by name.</summary>
    public PropertyStoreBuilder AddNamedInt16Property(string name, short value)
    {
        ArgumentNullException.ThrowIfNull(name);
        _namedProperties.Add((name, SerializeInt16(value)));
        return this;
    }

    /// <summary>Adds a custom 32-bit signed integer property by name.</summary>
    public PropertyStoreBuilder AddNamedInt32Property(string name, int value)
    {
        ArgumentNullException.ThrowIfNull(name);
        _namedProperties.Add((name, SerializeInt32(value)));
        return this;
    }

    /// <summary>Adds a custom 16-bit unsigned integer property by name.</summary>
    public PropertyStoreBuilder AddNamedUInt16Property(string name, ushort value)
    {
        ArgumentNullException.ThrowIfNull(name);
        _namedProperties.Add((name, SerializeUInt16(value)));
        return this;
    }

    /// <summary>Adds a custom 64-bit signed integer property by name.</summary>
    public PropertyStoreBuilder AddNamedInt64Property(string name, long value)
    {
        ArgumentNullException.ThrowIfNull(name);
        _namedProperties.Add((name, SerializeInt64(value)));
        return this;
    }

    /// <summary>Adds a custom 64-bit unsigned integer property by name.</summary>
    public PropertyStoreBuilder AddNamedUInt64Property(string name, ulong value)
    {
        ArgumentNullException.ThrowIfNull(name);
        _namedProperties.Add((name, SerializeUInt64(value)));
        return this;
    }

    /// <summary>Adds a custom FILETIME (DateTime) property by name.</summary>
    public PropertyStoreBuilder AddNamedFileTimeProperty(string name, DateTime value)
    {
        ArgumentNullException.ThrowIfNull(name);
        _namedProperties.Add((name, SerializeFileTime(value)));
        return this;
    }

    /// <summary>Adds a custom ANSI string property by name.</summary>
    public PropertyStoreBuilder AddNamedAnsiStringProperty(string name, string value)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);
        _namedProperties.Add((name, SerializeAnsiString(value)));
        return this;
    }

    /// <summary>Adds a custom binary blob property by name.</summary>
    public PropertyStoreBuilder AddNamedBlobProperty(string name, byte[] value)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);
        _namedProperties.Add((name, SerializeBlob(value)));
        return this;
    }

    /// <summary>
    /// Serializes all set properties into the MS-PROPSTORE binary format.
    /// Returns the byte array suitable for <see cref="ShortcutOptions.PropertyStoreData"/>.
    /// </summary>
    public byte[] Build()
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        WriteAppUserModelStorage(writer);
        WriteLinkStorage(writer);
        WriteSystemStorage(writer);
        WriteTargetUrlStorage(writer);
        WriteTargetExtensionStorage(writer);
        WriteWinXStorage(writer);
        WriteNamedPropertyStorage(writer);

        // Terminal: 0-size storage
        writer.Write(0);

        writer.Flush();
        return ms.ToArray();
    }

    private void WriteAppUserModelStorage(BinaryWriter writer)
    {
        var entries = new List<(uint propertyId, byte[] serializedValue)>();

        if (RelaunchCommand != null)
            entries.Add((PID_RelaunchCommand, SerializeString(RelaunchCommand)));
        if (RelaunchIconResource != null)
            entries.Add((PID_RelaunchIconResource, SerializeString(RelaunchIconResource)));
        if (RelaunchDisplayNameResource != null)
            entries.Add((PID_RelaunchDisplayNameResource, SerializeString(RelaunchDisplayNameResource)));
        if (AppUserModelId != null)
            entries.Add((PID_AppUserModelId, SerializeString(AppUserModelId)));
        if (IsDestListSeparator.HasValue)
            entries.Add((PID_IsDestListSeparator, SerializeBool(IsDestListSeparator.Value)));
        if (IsDestListLink.HasValue)
            entries.Add((PID_IsDestListLink, SerializeBool(IsDestListLink.Value)));
        if (ExcludeFromShowInNewInstall.HasValue)
            entries.Add((PID_ExcludeFromShowInNewInstall, SerializeBool(ExcludeFromShowInNewInstall.Value)));
        if (PreventPinning.HasValue)
            entries.Add((PID_PreventPinning, SerializeBool(PreventPinning.Value)));
        if (BestShortcut.HasValue)
            entries.Add((PID_BestShortcut, SerializeBool(BestShortcut.Value)));
        if (IsDualMode.HasValue)
            entries.Add((PID_IsDualMode, SerializeBool(IsDualMode.Value)));
        if (StartPinOption.HasValue)
            entries.Add((PID_StartPinOption, SerializeUInt32(StartPinOption.Value)));
        if (PackageRelativeApplicationID != null)
            entries.Add((PID_PackageRelativeApplicationID, SerializeString(PackageRelativeApplicationID)));
        if (HostEnvironment.HasValue)
            entries.Add((PID_HostEnvironment, SerializeUInt32(HostEnvironment.Value)));
        if (PackageFamilyName != null)
            entries.Add((PID_PackageFamilyName, SerializeString(PackageFamilyName)));
        if (PackageFullName != null)
            entries.Add((PID_PackageFullName, SerializeString(PackageFullName)));
        if (PackageInstallPath != null)
            entries.Add((PID_PackageInstallPath, SerializeString(PackageInstallPath)));
        if (InstalledBy != null)
            entries.Add((PID_InstalledBy, SerializeString(InstalledBy)));
        if (RecordState.HasValue)
            entries.Add((PID_RecordState, SerializeUInt32(RecordState.Value)));
        if (ParentID != null)
            entries.Add((PID_ParentID, SerializeString(ParentID)));
        if (Relevance.HasValue)
            entries.Add((PID_Relevance, SerializeUInt32(Relevance.Value)));
        if (DestListProvidedTitle != null)
            entries.Add((PID_DestListProvidedTitle, SerializeString(DestListProvidedTitle)));
        if (DestListProvidedDescription != null)
            entries.Add((PID_DestListProvidedDescription, SerializeString(DestListProvidedDescription)));
        if (DestListProvidedGroupName != null)
            entries.Add((PID_DestListProvidedGroupName, SerializeString(DestListProvidedGroupName)));
        if (DestListLogoUri != null)
            entries.Add((PID_DestListLogoUri, SerializeString(DestListLogoUri)));
        if (ToastActivatorCLSID.HasValue)
            entries.Add((PID_ToastActivatorCLSID, SerializeClsid(ToastActivatorCLSID.Value)));
        if (RunFlags.HasValue)
            entries.Add((PID_RunFlags, SerializeUInt32(RunFlags.Value)));
        if (ActivationContext != null)
            entries.Add((PID_ActivationContext, SerializeString(ActivationContext)));
        if (VisualElementsManifestHintPath != null)
            entries.Add((PID_VisualElementsManifestHintPath, SerializeString(VisualElementsManifestHintPath)));
        if (ExcludedFromLauncher.HasValue)
            entries.Add((PID_ExcludedFromLauncher, SerializeBool(ExcludedFromLauncher.Value)));
        if (FeatureOnDemand.HasValue)
            entries.Add((PID_FeatureOnDemand, SerializeBool(FeatureOnDemand.Value)));
        if (TileUniqueId != null)
            entries.Add((PID_TileUniqueId, SerializeString(TileUniqueId)));

        if (entries.Count == 0) return;

        WriteStorage(writer, AppUserModelFormatId, entries);
    }

    private void WriteLinkStorage(BinaryWriter writer)
    {
        var entries = new List<(uint propertyId, byte[] serializedValue)>();

        if (TargetParsingPath != null)
            entries.Add((PID_TargetParsingPath, SerializeString(TargetParsingPath)));
        if (LinkComment != null)
            entries.Add((PID_LinkComment, SerializeString(LinkComment)));
        if (DateVisited.HasValue)
            entries.Add((PID_DateVisited, SerializeFileTime(DateVisited.Value)));
        if (FeedUrl != null)
            entries.Add((PID_FeedUrl, SerializeString(FeedUrl)));
        if (LinkStatus.HasValue)
            entries.Add((PID_LinkStatus, SerializeInt32(LinkStatus.Value)));
        if (TargetSFGAOFlags.HasValue)
            entries.Add((PID_TargetSFGAOFlags, SerializeUInt32(TargetSFGAOFlags.Value)));

        if (entries.Count == 0) return;

        WriteStorage(writer, LinkFormatId, entries);
    }

    private void WriteSystemStorage(BinaryWriter writer)
    {
        var entries = new List<(uint propertyId, byte[] serializedValue)>();

        if (ItemTypeText != null)
            entries.Add((PID_ItemTypeText, SerializeString(ItemTypeText)));
        if (MimeType != null)
            entries.Add((PID_MimeType, SerializeString(MimeType)));

        if (entries.Count == 0) return;

        WriteStorage(writer, SystemFormatId, entries);
    }

    private void WriteTargetUrlStorage(BinaryWriter writer)
    {
        if (TargetUrl == null) return;
        var entries = new List<(uint pid, byte[] value)> { (2, SerializeString(TargetUrl)) };
        WriteStorage(writer, TargetUrlFormatId, entries);
    }

    private void WriteTargetExtensionStorage(BinaryWriter writer)
    {
        if (TargetExtension == null) return;
        var entries = new List<(uint pid, byte[] value)> { (2, SerializeString(TargetExtension)) };
        WriteStorage(writer, TargetExtensionFormatId, entries);
    }

    private void WriteWinXStorage(BinaryWriter writer)
    {
        if (!WinXHash.HasValue) return;
        var entries = new List<(uint pid, byte[] value)> { (PID_WinXHash, SerializeUInt32(WinXHash.Value)) };
        WriteStorage(writer, WinXFormatId, entries);
    }

    /// <summary>
    /// Writes the common storage frame: size prefix, "1SPS" magic, format ID,
    /// then delegates entry writing to the caller, followed by a terminal entry.
    /// </summary>
    private static void WriteStorageFrame(BinaryWriter writer, Guid formatId, Action<BinaryWriter> writeEntries)
    {
        using var storageMs = new MemoryStream();
        using var storageWriter = new BinaryWriter(storageMs);

        storageWriter.Write(0x53505331u); // "1SPS"
        storageWriter.Write(formatId.ToByteArray());
        writeEntries(storageWriter);
        storageWriter.Write(0); // Terminal entry

        storageWriter.Flush();
        byte[] storageData = storageMs.ToArray();

        writer.Write(4 + storageData.Length); // StorageSize
        writer.Write(storageData);
    }

    private void WriteNamedPropertyStorage(BinaryWriter writer)
    {
        if (_namedProperties.Count == 0) return;

        WriteStorageFrame(writer, NamedPropertyFormatId, storageWriter =>
        {
            foreach (var (name, value) in _namedProperties)
            {
                byte[] nameBytes = Encoding.Unicode.GetBytes(name + "\0");
                int valueSize = 4 + 4 + 1 + nameBytes.Length + value.Length;
                storageWriter.Write(valueSize);
                storageWriter.Write(nameBytes.Length);
                storageWriter.Write((byte)0); // Reserved
                storageWriter.Write(nameBytes);
                storageWriter.Write(value);
            }
        });
    }

    private static void WriteStorage(BinaryWriter writer, Guid formatId, List<(uint pid, byte[] value)> entries)
    {
        WriteStorageFrame(writer, formatId, storageWriter =>
        {
            foreach (var (pid, value) in entries)
            {
                int valueSize = 4 + 4 + 1 + value.Length;
                storageWriter.Write(valueSize);
                storageWriter.Write(pid);
                storageWriter.Write((byte)0); // Reserved
                storageWriter.Write(value);
            }
        });
    }

    private static byte[] SerializeProperty(ushort vtType, Action<BinaryWriter> writeValue)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write(vtType);
        w.Write((ushort)0); // padding
        writeValue(w);
        w.Flush();
        return ms.ToArray();
    }

    private static byte[] SerializeString(string value) =>
        SerializeProperty(VT_LPWSTR, w =>
        {
            byte[] strBytes = Encoding.Unicode.GetBytes(value + "\0");
            w.Write(strBytes.Length);
            w.Write(strBytes);
        });

    private static byte[] SerializeBool(bool value) =>
        SerializeProperty(VT_BOOL, w =>
        {
            w.Write(value ? (short)-1 : (short)0); // VT_BOOL: -1 = true, 0 = false
            w.Write((short)0); // padding to 4-byte boundary
        });

    private static byte[] SerializeClsid(Guid value) =>
        SerializeProperty(VT_CLSID, w => w.Write(value.ToByteArray()));

    private static byte[] SerializeUInt32(uint value) =>
        SerializeProperty(VT_UI4, w => w.Write(value));

    private static byte[] SerializeInt16(short value) =>
        SerializeProperty(VT_I2, w =>
        {
            w.Write(value);
            w.Write((short)0); // padding to 4-byte boundary
        });

    private static byte[] SerializeInt32(int value) =>
        SerializeProperty(VT_I4, w => w.Write(value));

    private static byte[] SerializeUInt16(ushort value) =>
        SerializeProperty(VT_UI2, w =>
        {
            w.Write(value);
            w.Write((ushort)0); // padding to 4-byte boundary
        });

    private static byte[] SerializeInt64(long value) =>
        SerializeProperty(VT_I8, w => w.Write(value));

    private static byte[] SerializeUInt64(ulong value) =>
        SerializeProperty(VT_UI8, w => w.Write(value));

    private static byte[] SerializeAnsiString(string value) =>
        SerializeProperty(VT_LPSTR, w =>
        {
            byte[] strBytes = Encoding.Default.GetBytes(value + "\0");
            w.Write(strBytes.Length);
            w.Write(strBytes);
        });

    private static byte[] SerializeFileTime(DateTime value) =>
        SerializeProperty(VT_FILETIME, w => w.Write(value.ToFileTimeUtc()));

    private static byte[] SerializeBlob(byte[] value) =>
        SerializeProperty(VT_BLOB, w =>
        {
            w.Write(value.Length);
            w.Write(value);
        });
}
