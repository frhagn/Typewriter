#
#	Enable script execution by running PowerShell (x86) as Administrator and type the command:
#	Set-ExecutionPolicy RemoteSigned
#

param ([string]$configuration = "Release")

#Add-Type -Path "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\System.IO.Compression.FileSystem.dll"

$invocation = (Get-Variable MyInvocation).Value
$directorypath = Split-Path $invocation.MyCommand.Path
$directorypath = Split-Path $directorypath

$bin = $directorypath + "\src\Typewriter\bin\" + $configuration
$vsix = $bin + "\Typewriter.vsix"
$artifacts = $directorypath + "\artifacts"
$destination = $artifacts + "\Typewriter.vsix"

#$zip = [System.IO.Compression.ZipFile]::Open($bin + "\Typewriter.vsix", "Update")

# Roslyn dependencies
#"Adding Roslyn dependencies for Visual Studio 2013"
#[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $bin + "\Microsoft.CodeAnalysis.CSharp.dll", "Microsoft.CodeAnalysis.CSharp.dll")
#[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $bin + "\Microsoft.CodeAnalysis.CSharp.Workspaces.dll", "Microsoft.CodeAnalysis.CSharp.Workspaces.dll")
#[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $bin + "\Microsoft.CodeAnalysis.dll", "Microsoft.CodeAnalysis.dll")
#[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $bin + "\Microsoft.CodeAnalysis.Workspaces.dll", "Microsoft.CodeAnalysis.Workspaces.dll")
#[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $bin + "\Microsoft.VisualStudio.ComponentModelHost.dll", "Microsoft.VisualStudio.ComponentModelHost.dll")
#[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $bin + "\System.Composition.AttributedModel.dll", "System.Composition.AttributedModel.dll")
#[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $bin + "\System.Composition.Convention.dll", "System.Composition.Convention.dll")
#[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $bin + "\System.Composition.Hosting.dll", "System.Composition.Hosting.dll")
#[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $bin + "\System.Composition.Runtime.dll", "System.Composition.Runtime.dll")
#[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $bin + "\System.Composition.TypedParts.dll", "System.Composition.TypedParts.dll")
#[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $bin + "\System.Collections.Immutable.dll", "System.Collections.Immutable.dll")
#[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $bin + "\System.Reflection.Metadata.dll", "System.Reflection.Metadata.dll")

#$zip.Dispose()

# Copy output
if($configuration -eq "Release")
{
	"Copying artifacts"
	if((Test-Path $artifacts) -eq 0) { MD $artifacts }
	Copy-Item -Path $vsix -Destination $artifacts
}

"Done"