namespace ShortcutLib;

/// <summary>
/// Represents an unrecognized extra data block preserved during parsing.
/// Unknown blocks are round-tripped through Open/Create to maintain fidelity.
/// </summary>
public sealed class RawExtraDataBlock
{
    /// <summary>Block signature (e.g. 0xA000000A).</summary>
    public uint Signature { get; set; }

    /// <summary>Raw block data (everything after the 8-byte size+signature header).</summary>
    public byte[] Data { get; set; } = [];
}
