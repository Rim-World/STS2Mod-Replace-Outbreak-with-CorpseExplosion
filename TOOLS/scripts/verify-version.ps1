param(
    [string]$ArtifactDir = (Join-Path (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent) '10.OUTPUT\01-构建产物\CorpseExplosionReplacementMod v8（0.111.1-0.113.1兼容，打包后mod）')
)

$ErrorActionPreference = 'Stop'
$root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$errors = New-Object System.Collections.Generic.List[string]

if (-not (Test-Path $ArtifactDir)) {
    throw "Artifact directory not found: $ArtifactDir"
}

$manifestPath = Join-Path $ArtifactDir 'CorpseExplosionReplacementMod.json'
$dllPath = Join-Path $ArtifactDir 'CorpseExplosionReplacementMod.dll'
$pckPath = Join-Path $ArtifactDir 'CorpseExplosionReplacementMod.pck'

if (-not (Test-Path $manifestPath)) { $errors.Add("missing manifest: $manifestPath") }
if (-not (Test-Path $dllPath)) { $errors.Add("missing DLL: $dllPath") }
if (-not (Test-Path $pckPath)) { $errors.Add("missing PCK: $pckPath") }

if ($errors.Count -eq 0) {
    $manifest = Get-Content $manifestPath -Raw | ConvertFrom-Json
    $version = [string]$manifest.version
    $minVersion = [string]$manifest.min_game_version
    $assemblyVersion = [Reflection.AssemblyName]::GetAssemblyName($dllPath).Version.ToString()

    Write-Host "manifest version : $version"
    Write-Host "assembly version : $assemblyVersion"
    Write-Host "min_game_version : $minVersion"

    if ([string]::IsNullOrWhiteSpace($version)) {
        $errors.Add('manifest version is empty')
    }
    if (-not $assemblyVersion.StartsWith("$version.0") -and $assemblyVersion -ne $version) {
        $errors.Add("assembly version $assemblyVersion does not match manifest $version")
    }
    if ($minVersion -ne '0.111.1') {
        $errors.Add("min_game_version is $minVersion, expected 0.111.1")
    }

    $changelog = Join-Path $root 'CHANGELOG.md'
    if (-not (Select-String -Path $changelog -Pattern "[$version]" -SimpleMatch)) {
        $errors.Add("CHANGELOG.md has no [$version] section")
    }

    $versionLog = Get-ChildItem (Join-Path $root '7.ChangeLog') -Filter "*-$version-*.md" -ErrorAction SilentlyContinue
    if (-not $versionLog) {
        $errors.Add("7.ChangeLog has no file for $version")
    }
}

if ($errors.Count -gt 0) {
    Write-Host 'FAILED:'
    foreach ($item in $errors) { Write-Host " - $item" }
    exit 1
}

Write-Host 'OK: version numbers are consistent.'
