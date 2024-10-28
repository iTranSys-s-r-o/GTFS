$csprojPath = (Get-ChildItem -Path . -Filter *.csproj | Select-Object -First 1).Name

$projName = $csprojPath -replace '.csproj', ''

####################################################################################
# Load version from csproj
####################################################################################
[xml]$csproj = Get-Content $csprojPath

if ($csproj.Project.PropertyGroup.PackageVersion -and $csproj.Project.PropertyGroup.PackageVersion.Length -gt 0) {
    if ($csproj.Project.PropertyGroup.PackageVersion -is [string]) {
        $nugetVersion = $csproj.Project.PropertyGroup.PackageVersion
    } elseif ($csproj.Project.PropertyGroup.PackageVersion -is [array]) {
        $nugetVersion = $csproj.Project.PropertyGroup.PackageVersion[0].ToString()
    }
} else {
    if ($csproj.Project.PropertyGroup.AssemblyVersion -and $csproj.Project.PropertyGroup.AssemblyVersion.Length -gt 0) {
        if ($csproj.Project.PropertyGroup.AssemblyVersion -is [string]) {
            $nugetVersion = $csproj.Project.PropertyGroup.AssemblyVersion
        } elseif ($csproj.Project.PropertyGroup.AssemblyVersion -is [array]) {
            $nugetVersion = $csproj.Project.PropertyGroup.AssemblyVersion[0].ToString()
        }
    } 
}

####################################################################################
# Increment AssemblyVersion in csproj
####################################################################################
$response = Read-Host "Do you want to increment the AssemblyVersion? (Y/n)"
if ($response -eq "" -or $response -eq "Y" -or $response -eq "y") {
    $versionParts = $nugetVersion -split '\.'

    $versionParts[-1] = [int]$versionParts[-1] + 1

    $nugetVersion = $versionParts -join '.'
    Write-Host "New PackageVersion: $nugetVersion"

    #change version in csproj
    $versionNode = $csproj.SelectSingleNode("//Project/PropertyGroup/PackageVersion")

    if ($versionNode) {
        $versionNode.InnerText = $nugetVersion
        Write-Host "Version updated to: $nugetVersion"
    } else {
        $assemblyVersionNode = $csproj.SelectSingleNode("//Project/PropertyGroup/AssemblyVersion")
        
        if ($assemblyVersionNode) {
            $assemblyVersionNode.InnerText = $nugetVersion
            Write-Host "AssemblyVersion updated to: $nugetVersion"
        } else {
            Write-Host "Neither Version nor AssemblyVersion node found."
        }
    }

    $csproj.Save($csprojPath)
}   


# Remove last part of version
if (($nugetVersion -split '\.').Count -eq 4) {
    $nugetVersion = $nugetVersion -replace '\.\d+$'
}     

if ([string]::IsNullOrEmpty($nugetVersion)) {
    throw "Version not found in csproj file"
}

####################################################################################
# Build
####################################################################################
dotnet build $csprojPath -c Release

$outputDir = "bin\Release"
dotnet pack $csprojPath -c Release -o $outputDir

####################################################################################
# Publish
####################################################################################
$nugetTitle = $csproj.Project.PropertyGroup.Title[0]

$packagePath = "bin\Release\$nugetTitle.$nugetVersion.nupkg"
dotnet nuget push $packagePath -s https://nuget.inprop.sk/v3/index.json
