# LNKLib

A zero-dependency .NET library for creating Windows Shell Link (.lnk) shortcut files in memory.

## Features

- Create shortcuts to local files, folders, network shares, and printers
- Environment variable support (e.g. `%windir%\notepad.exe`)
- Custom icon, arguments, working directory, and description
- Argument padding to hide command-line arguments in shortcut properties
- Returns raw `byte[]` — no COM interop or Windows Shell dependency
- Targets .NET 10

## Installation

Add a reference to the `LNKLib` project or install the NuGet package:

```
dotnet add package LNKLib
```

## Usage

```csharp
using LNKLib;

// Simple file shortcut
byte[] lnk = Shortcut.Create(@"C:\Windows\notepad.exe");
File.WriteAllBytes("Notepad.lnk", lnk);

// Shortcut with all options
byte[] lnk = Shortcut.Create(
    target: @"C:\Windows\notepad.exe",
    arguments: @"C:\notes.txt",
    iconLocation: @"C:\Windows\notepad.exe",
    iconIndex: 0,
    description: "My Notepad Shortcut",
    workingDirectory: @"C:\Windows");
File.WriteAllBytes("Notepad.lnk", lnk);

// Network share shortcut
byte[] lnk = Shortcut.Create(@"\\server\share\document.docx");

// Environment variable shortcut
byte[] lnk = Shortcut.Create(@"%windir%\notepad.exe");

// Shortcut with padded arguments (hidden in properties UI)
byte[] lnk = Shortcut.Create(
    target: @"C:\Windows\notepad.exe",
    arguments: "--secret-flag",
    padArguments: true);
```

## API

```csharp
public static byte[] Shortcut.Create(
    string target,                   // Target path (required)
    string? arguments = null,        // Command-line arguments
    bool padArguments = false,       // Pad arguments to 31 KB buffer
    string? iconLocation = null,     // Icon file path
    int iconIndex = 0,               // Icon index within file
    string? description = null,      // Shortcut description
    string? workingDirectory = null,  // Working directory
    bool isPrinterLink = false)      // Treat target as a printer
```

## Building

```
dotnet build
```

## Testing

```
dotnet test
```

## License

[MIT](LICENSE)
