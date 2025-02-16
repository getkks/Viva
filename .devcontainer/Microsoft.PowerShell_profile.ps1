$env:DOTNET_ReadyToRun = 0
$env:DOTNET_TieredPGO = 1
$env:DOTNET_TC_QuickJit = 1
$env:DOTNET_TC_QuickJitForLoops = 0
$env:DOTNET_SCAFFOLD_TELEMETRY_OPTOUT=1
Register-ArgumentCompleter -Native -CommandName dotnet -ScriptBlock {
    param($commandName, $wordToComplete, $cursorPosition)
    dotnet complete --position $cursorPosition "$wordToComplete" | ForEach-Object {
        [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
    }
}

if ($host.Name -eq 'ConsoleHost') {
    Import-Module PSReadLine
}
Set-PSReadLineKeyHandler -Key Ctrl+Shift+b `
    -BriefDescription BuildCurrentDirectory `
    -LongDescription 'dotnet Build the current directory' `
    -ScriptBlock {
    [Microsoft.PowerShell.PSConsoleReadLine]::RevertLine()
    [Microsoft.PowerShell.PSConsoleReadLine]::Insert('dotnet build')
    [Microsoft.PowerShell.PSConsoleReadLine]::AcceptLine()
}
# Shows navigable menu of all options when hitting Tab
Set-PSReadlineKeyHandler -Key Tab -Function MenuComplete

# Autocompletion for arrow keys
Set-PSReadlineKeyHandler -Key UpArrow -Function HistorySearchBackward
Set-PSReadlineKeyHandler -Key DownArrow -Function HistorySearchForward
Set-PSReadLineOption -PredictionSource History
Set-PSReadLineOption -PredictionViewStyle ListView
Set-PSReadLineOption -EditMode Windows

# custom function to add all projects to the solution
function addProjectsToSolution {
    [cmdletbinding()]
    param([Parameter(Mandatory = $False, ValueFromPipeline = $True, ValueFromPipelinebyPropertyName = $True)][System.IO.DirectoryInfo][SupportsWildcards()][PSDefaultValue(Help = 'Current directory is used.')]$directory = ".")
    Get-ChildItem -Path $directory.FullName -Filter *.?sproj  -Recurse -File | ForEach-Object {
        dotnet sln add $_.FullName
    }
}
