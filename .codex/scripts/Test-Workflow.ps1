param(
    [string]$Base = "dev",
    [switch]$FailOnWarning
)

$ErrorActionPreference = "Stop"
$warnings = New-Object System.Collections.Generic.List[string]

function Add-Warning {
    param([string]$Message)
    $script:warnings.Add($Message) | Out-Null
}

function Get-GitLines {
    param([Parameter(ValueFromRemainingArguments = $true)][string[]]$Args)
    $result = & git @Args
    if ($LASTEXITCODE -ne 0) {
        throw "git $($Args -join ' ') failed"
    }
    return @($result)
}

function Test-PathWarn {
    param(
        [string]$Path,
        [string]$Message
    )
    if (-not (Test-Path -LiteralPath $Path)) {
        Add-Warning $Message
    }
}

$branch = (Get-GitLines branch --show-current | Select-Object -First 1).Trim()
$status = @(Get-GitLines status --short)
$changedSinceBase = @(Get-GitLines diff --name-only "$Base...HEAD")

Test-PathWarn "AGENTS.md" "Missing AGENTS.md. Future agents will not automatically see the workflow rules."
Test-PathWarn "docs/project-state.md" "Missing docs/project-state.md. New sessions need a compact state file."
Test-PathWarn "docs/branch-handoffs/TEMPLATE.md" "Missing branch handoff template."
Test-PathWarn "docs/test-reports/TEMPLATE.md" "Missing test report template."
Test-PathWarn ".github/pull_request_template.md" "Missing pull request template."
Test-PathWarn ".codex/scripts/Make-Handoff.ps1" "Missing Make-Handoff.ps1."

if (Test-Path -LiteralPath "docs/project-state.md") {
    $stateLength = (Get-Item -LiteralPath "docs/project-state.md").Length
    if ($stateLength -gt 12288) {
        Add-Warning "docs/project-state.md is larger than 12 KiB. Move detail into focused docs and keep the state file compact."
    }
}

if ($branch -in @("dev", "master", "main") -and $status.Count -gt 0) {
    Add-Warning "Working directly on $branch with local changes. Prefer a topic branch unless the user explicitly requested this."
}

if ($changedSinceBase.Count -gt 0 -and $branch -notin @("dev", "master", "main")) {
    $handoffs = @()
    if (Test-Path -LiteralPath "docs/branch-handoffs") {
        $handoffs = @(Get-ChildItem -LiteralPath "docs/branch-handoffs" -Filter "*.md" | Where-Object { $_.Name -ne "TEMPLATE.md" })
    }
    if ($handoffs.Count -eq 0) {
        Add-Warning "This branch has changes relative to $Base but no branch handoff under docs/branch-handoffs."
    }
}

if ($status.Count -gt 0) {
    Write-Host "Working tree entries:"
    foreach ($entry in $status) {
        Write-Host "  $entry"
    }
    Write-Host ""
}

if ($warnings.Count -eq 0) {
    Write-Host "Workflow check passed."
    exit 0
}

Write-Host "Workflow check warnings:"
foreach ($warning in $warnings) {
    Write-Host "  - $warning"
}

if ($FailOnWarning) {
    exit 1
}

exit 0

