# PowerShell script to build and pack NuGet packages for HttpTestGen

Write-Host "Building and packing HttpTestGen NuGet packages..." -ForegroundColor Green

# Set the solution directory
$solutionDir = Split-Path $MyInvocation.MyCommand.Path -Parent

# Define the projects to pack
$projects = @(
    "src\HttpTestGen.Core\HttpTestGen.Core.csproj",
    "src\HttpTestGen.TUnitGenerator\HttpTestGen.TUnitGenerator.csproj",
    "src\HttpTestGen.XunitGenerator\HttpTestGen.XunitGenerator.csproj"
)

# Create output directory for packages
$outputDir = Join-Path $solutionDir "nupkgs"
if (!(Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force
    Write-Host "Created output directory: $outputDir" -ForegroundColor Yellow
}

# Clean and build solution first
Write-Host "Cleaning solution..." -ForegroundColor Yellow
dotnet clean HttpTestGen.sln --configuration Release

Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build HttpTestGen.sln --configuration Release --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. Exiting." -ForegroundColor Red
    exit 1
}

# Pack each project
foreach ($project in $projects) {
    $projectPath = Join-Path $solutionDir $project
    $projectName = [System.IO.Path]::GetFileNameWithoutExtension($project)
    
    Write-Host "Packing $projectName..." -ForegroundColor Yellow
    
    dotnet pack $projectPath `
        --configuration Release `
        --no-build `
        --output $outputDir `
        --include-symbols `
        --include-source
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Successfully packed $projectName" -ForegroundColor Green
    } else {
        Write-Host "✗ Failed to pack $projectName" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Package creation completed!" -ForegroundColor Green
Write-Host "Packages are located in: $outputDir" -ForegroundColor Cyan

# List created packages
Write-Host ""
Write-Host "Created packages:" -ForegroundColor Yellow
Get-ChildItem $outputDir -Filter "*.nupkg" | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor White
}

Write-Host ""
Write-Host "To publish to NuGet.org, use:" -ForegroundColor Cyan
Write-Host "dotnet nuget push `"$outputDir\*.nupkg`" --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor White
