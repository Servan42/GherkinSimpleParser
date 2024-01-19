try {
    # Get versions from csproj
    [xml] $parserCsprojXml = Get-Content .\GherkinSimpleParser\GherkinSimpleParser.csproj
    [string] $parserVersion = $parserCsprojXml.SelectNodes("//Version").InnerText
    [xml] $converterCsprojXml = Get-Content .\GherkinSimpleParser.Converter\GherkinSimpleParser.Converter.csproj
    [string] $converterVersion = $converterCsprojXml.SelectNodes("//Version").InnerText
    
    # Cleanup and prepare directory
    $publishFolder = ".\Publish"
    if (Test-Path $publishFolder -PathType Container) {
        Remove-Item -Recurse $publishFolder
    }
    mkdir $publishFolder
    dotnet clean -c Release
    
    # Publish parser
    $parserNugetFilename = "GherkinSimpleParser.$parserVersion.nupkg"
    $parserZipFilename = "GherkinSimpleParser_dll_$parserVersion.zip"
    dotnet publish .\GherkinSimpleParser -c Release -o $publishFolder\temp
    Copy-Item .\LICENSE.txt $publishFolder\temp
    Copy-Item .\README.md $publishFolder\temp
    Copy-Item ".\GherkinSimpleParser\bin\Release\$parserNugetFilename" $publishFolder\
    Compress-Archive -Path $publishFolder\temp\* -DestinationPath $publishFolder\$parserZipFilename
    Remove-Item -Recurse $publishFolder\temp
    
    # Publish converter
    $converterNugetFilename = "GherkinSimpleParser.Converter.$converterVersion.nupkg"
    $converterZipFilename = "GherkinSimpleParser.Converter_dll_$converterVersion.zip"
    dotnet publish .\GherkinSimpleParser.Converter -c Release -o $publishFolder\temp
    Copy-Item .\LICENSE.txt $publishFolder\temp
    Copy-Item .\README.md $publishFolder\temp
    Copy-Item ".\GherkinSimpleParser.Converter\bin\Release\$converterNugetFilename" $publishFolder\
    Compress-Archive -Path $publishFolder\temp\* -DestinationPath $publishFolder\$converterZipFilename
    Remove-Item -Recurse $publishFolder\temp
    
    # Publish console
    $consoleZipFilename = "GherkinSimpleParser.Console_dll_$parserVersion.zip"
    dotnet publish .\GherkinSimpleParser.Console -c Release -o $publishFolder\temp
    Copy-Item .\LICENSE.txt $publishFolder\temp
    Copy-Item .\README.md $publishFolder\temp
    Compress-Archive -Path $publishFolder\temp\* -DestinationPath $publishFolder\$consoleZipFilename
    Remove-Item -Recurse $publishFolder\temp
    
    # Prepare release notes markdown file
    Add-Content -Path $publishFolder\release_notes.txt -Value "## Release Notes`n`n* TODO`n`n## SHA256`n### Parser"
    $checksum = Get-FileHash -Path $publishFolder\$parserNugetFilename -Algorithm SHA256 | Select-Object -ExpandProperty Hash
    Add-Content -Path $publishFolder\release_notes.txt -Value "* ``$checksum`` $parserNugetFilename"
    $checksum = Get-FileHash -Path $publishFolder\$parserZipFilename -Algorithm SHA256 | Select-Object -ExpandProperty Hash
    Add-Content -Path $publishFolder\release_notes.txt -Value "* ``$checksum`` $parserZipFilename"
    Add-Content -Path $publishFolder\release_notes.txt -Value "### Converter"
    $checksum = Get-FileHash -Path $publishFolder\$converterNugetFilename -Algorithm SHA256 | Select-Object -ExpandProperty Hash
    Add-Content -Path $publishFolder\release_notes.txt -Value "* ``$checksum`` $converterNugetFilename"
    $checksum = Get-FileHash -Path $publishFolder\$converterZipFilename -Algorithm SHA256 | Select-Object -ExpandProperty Hash
    Add-Content -Path $publishFolder\release_notes.txt -Value "* ``$checksum`` $converterZipFilename"
    Add-Content -Path $publishFolder\release_notes.txt -Value "### Console"
    $checksum = Get-FileHash -Path $publishFolder\$consoleZipFilename -Algorithm SHA256 | Select-Object -ExpandProperty Hash
    Add-Content -Path $publishFolder\release_notes.txt -Value "* ``$checksum`` $consoleZipFilename"
}
catch {
    Pause
}