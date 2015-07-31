param (
    [string]$c = "Release"
 )

Add-Type -Path "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\System.IO.Compression.FileSystem.dll"

$invocation = (Get-Variable MyInvocation).Value
$directorypath = Split-Path $invocation.MyCommand.Path

$zip = [System.IO.Compression.ZipFile]::Open($directorypath + "\Typewriter\bin\" + $c + "\Typewriter.vsix", "Update")

# Roslyn dependencies
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $c + "\Microsoft.CodeAnalysis.CSharp.dll", "Microsoft.CodeAnalysis.CSharp.dll", "NoCompression")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $c + "\Microsoft.CodeAnalysis.CSharp.Workspaces.dll", "Microsoft.CodeAnalysis.CSharp.Workspaces.dll", "NoCompression")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $c + "\Microsoft.CodeAnalysis.dll", "Microsoft.CodeAnalysis.dll", "NoCompression")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $c + "\Microsoft.CodeAnalysis.Workspaces.dll", "Microsoft.CodeAnalysis.Workspaces.dll", "NoCompression")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $c + "\Microsoft.VisualStudio.ComponentModelHost.dll", "Microsoft.VisualStudio.ComponentModelHost.dll", "NoCompression")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $c + "\System.Composition.AttributedModel.dll", "System.Composition.AttributedModel.dll", "NoCompression")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $c + "\System.Composition.Convention.dll", "System.Composition.Convention.dll", "NoCompression")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $c + "\System.Composition.Hosting.dll", "System.Composition.Hosting.dll", "NoCompression")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $c + "\System.Composition.Runtime.dll", "System.Composition.Runtime.dll", "NoCompression")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $c + "\System.Composition.TypedParts.dll", "System.Composition.TypedParts.dll", "NoCompression")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $c + "\System.Collections.Immutable.dll", "System.Collections.Immutable.dll", "NoCompression")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $c + "\System.Reflection.Metadata.dll", "System.Reflection.Metadata.dll", "NoCompression")

# Roslyn Metadata
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Roslyn\bin\" + $c + "\Typewriter.Metadata.Roslyn.dll", "Typewriter.Metadata.Roslyn.dll", "NoCompression")

$zip.Dispose()