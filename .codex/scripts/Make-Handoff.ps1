param(
    [string]$Base = "dev",
    [string]$Output = ""
)

$ErrorActionPreference = "Stop"

function Get-GitLines {
    param([Parameter(ValueFromRemainingArguments = $true)][string[]]$Args)
    $result = & git @Args
    if ($LASTEXITCODE -ne 0) {
        throw "git $($Args -join ' ') failed"
    }
    return $result
}

$branch = (Get-GitLines branch --show-current | Select-Object -First 1).Trim()
$head = (Get-GitLines rev-parse --short HEAD | Select-Object -First 1).Trim()
$baseShort = (Get-GitLines rev-parse --short $Base | Select-Object -First 1).Trim()
$mergeBase = (Get-GitLines merge-base HEAD $Base | Select-Object -First 1).Trim()
$status = @(Get-GitLines status --short)
$commits = @(Get-GitLines log --oneline "$Base..HEAD")
$files = @(Get-GitLines diff --name-status "$Base...HEAD")

$lines = @()
$lines += "# Branch Handoff"
$lines += ""
$lines += "Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss zzz')"
$lines += "Branch: $branch"
$lines += "Base branch: $Base ($baseShort)"
$lines += "Head commit: $head"
$lines += "Merge base: $mergeBase"
$lines += ""
$lines += "## Goal"
$lines += ""
$lines += "TODO"
$lines += ""
$lines += "## Commits Since Base"
$lines += ""
if ($commits.Count -eq 0) {
    $lines += "- None"
} else {
    foreach ($commit in $commits) {
        $lines += "- $commit"
    }
}
$lines += ""
$lines += "## Changed Files"
$lines += ""
if ($files.Count -eq 0) {
    $lines += "- None"
} else {
    foreach ($file in $files) {
        $lines += "- $file"
    }
}
$lines += ""
$lines += "## Working Tree"
$lines += ""
if ($status.Count -eq 0) {
    $lines += "- Clean"
} else {
    foreach ($entry in $status) {
        $lines += "- $entry"
    }
}
$lines += ""
$lines += "## Verification"
$lines += ""
$lines += "- TODO"
$lines += ""
$lines += "## Open Risks"
$lines += ""
$lines += "- TODO"
$lines += ""
$lines += "## Next Action"
$lines += ""
$lines += "TODO"

$content = $lines -join [Environment]::NewLine

if ([string]::IsNullOrWhiteSpace($Output)) {
    Write-Output $content
} else {
    Set-Content -LiteralPath $Output -Value $content -Encoding UTF8
}
