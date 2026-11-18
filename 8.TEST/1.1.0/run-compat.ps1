param(
    [string]$GameRoot = 'C:\Game\Steam\steamapps\common\Slay the Spire 2',
    [string]$Scenario = (Join-Path $PSScriptRoot 'corpse-explosion-compat.scenario'),
    [string]$LogDir = (Join-Path $PSScriptRoot 'logs')
)

$ErrorActionPreference = 'Stop'

$running = Get-Process -Name 'SlayTheSpire2' -ErrorAction SilentlyContinue
if ($running) {
    throw 'Slay the Spire 2 is running. Close it before the compatibility test so the mod DLL is not locked.'
}

$modDir = Join-Path $GameRoot 'mods\CorpseExplosionReplacementMod'
$modManifest = Join-Path $modDir 'CorpseExplosionReplacementMod.json'
if (-not (Test-Path $modManifest)) {
    throw "Main mod is not installed: $modManifest"
}

$installedVersion = (Get-Content $modManifest -Raw | ConvertFrom-Json).version
if ($installedVersion -ne '1.1.0') {
    throw "Installed mod version is $installedVersion, expected 1.1.0. Run 9.SRC\CorpseExplosionReplacementMod\build.ps1 first."
}

$probeProject = Join-Path $PSScriptRoot 'CorpseExplosionProbe'
$probeDll = Join-Path $probeProject 'bin\Debug\net9.0\CorpseExplosionProbe.dll'
$probeJson = Join-Path $probeProject 'CorpseExplosionProbe.json'
if (-not (Test-Path $probeDll)) {
    throw "Probe DLL not found: $probeDll. Run dotnet build in $probeProject first."
}

$probeOut = Join-Path $GameRoot 'mods\CorpseExplosionProbe'
New-Item -ItemType Directory -Force -Path $probeOut | Out-Null
Copy-Item $probeDll (Join-Path $probeOut 'CorpseExplosionProbe.dll') -Force
Copy-Item $probeJson (Join-Path $probeOut 'CorpseExplosionProbe.json') -Force

New-Item -ItemType Directory -Force -Path $LogDir | Out-Null
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$base = Join-Path $LogDir "0.113.1-$stamp"
$stdout = "$base.stdout.log"
$stderr = "$base.stderr.log"
$engineLog = "$base.engine.log"
$summary = "$base.summary.txt"

$env:SteamAppId = '2868840'
$args = @(
    '--headless',
    "--scenario=$Scenario",
    '--seed=CE-COMPAT-1131',
    '--fast-mode=Fast'
)

$process = Start-Process -FilePath (Join-Path $GameRoot 'SlayTheSpire2.exe') `
    -ArgumentList $args `
    -WorkingDirectory $GameRoot `
    -PassThru `
    -NoNewWindow `
    -RedirectStandardOutput $stdout `
    -RedirectStandardError $stderr

$timedOut = -not $process.WaitForExit(240000)
if ($timedOut) {
    $process.Kill()
    $process.WaitForExit()
}

Start-Sleep -Seconds 2
$defaultLog = Join-Path $env:APPDATA 'SlayTheSpire2\logs\godot.log'
if (Test-Path $defaultLog) {
    Copy-Item $defaultLog $engineLog -Force
}

$exitCode = if ($timedOut) { 'TIMEOUT' } else { $process.ExitCode }
$sentinel = 'MISSING'
if (Test-Path $engineLog) {
    $completed = Select-String -Path $engineLog -Pattern 'Scenario completed successfully' -SimpleMatch
    $failed = Select-String -Path $engineLog -Pattern 'Scenario failed' -SimpleMatch
    if ($completed) { $sentinel = 'COMPLETED' }
    elseif ($failed) { $sentinel = 'FAILED' }
}

@"
time=$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
game=0.113.1
mod=$installedVersion
scenario=$Scenario
exit_code=$exitCode
timeout=$timedOut
sentinel=$sentinel
stdout=$stdout
stderr=$stderr
engine_log=$engineLog
"@ | Set-Content $summary -Encoding utf8

Get-Content $summary
if ($timedOut -or $sentinel -ne 'COMPLETED') {
    exit 1
}
