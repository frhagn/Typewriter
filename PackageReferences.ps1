#
#	Enable script execution by running PowerShell (x86) as Administrator and type the command:
#	Set-ExecutionPolicy RemoteSigned
#

param ([string]$configuration = "Release")

Add-Type -Path "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\System.IO.Compression.FileSystem.dll"

$invocation = (Get-Variable MyInvocation).Value
$directorypath = Split-Path $invocation.MyCommand.Path

$zip = [System.IO.Compression.ZipFile]::Open($directorypath + "\Typewriter\bin\" + $configuration + "\Typewriter.vsix", "Update")

# Roslyn dependencies
"Adding Roslyn dependencies for Visual Studio 2013"
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $configuration + "\Microsoft.CodeAnalysis.CSharp.dll", "Microsoft.CodeAnalysis.CSharp.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $configuration + "\Microsoft.CodeAnalysis.CSharp.Workspaces.dll", "Microsoft.CodeAnalysis.CSharp.Workspaces.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $configuration + "\Microsoft.CodeAnalysis.dll", "Microsoft.CodeAnalysis.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $configuration + "\Microsoft.CodeAnalysis.Workspaces.dll", "Microsoft.CodeAnalysis.Workspaces.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $configuration + "\Microsoft.VisualStudio.ComponentModelHost.dll", "Microsoft.VisualStudio.ComponentModelHost.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $configuration + "\System.Composition.AttributedModel.dll", "System.Composition.AttributedModel.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $configuration + "\System.Composition.Convention.dll", "System.Composition.Convention.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $configuration + "\System.Composition.Hosting.dll", "System.Composition.Hosting.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $configuration + "\System.Composition.Runtime.dll", "System.Composition.Runtime.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $configuration + "\System.Composition.TypedParts.dll", "System.Composition.TypedParts.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $configuration + "\System.Collections.Immutable.dll", "System.Collections.Immutable.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $directorypath + "\Typewriter\bin\" + $configuration + "\System.Reflection.Metadata.dll", "System.Reflection.Metadata.dll")

$zip.Dispose()

"Done"