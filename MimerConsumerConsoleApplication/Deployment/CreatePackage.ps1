Import-Module -Name ".\Invoke-MsBuild.psm1"

Write-Host "`nBuilding AnyCPU Release`n"
$buildAnyRelease = Invoke-MsBuild -Path "..\..\MimerConsumerConsoleApplication.sln" -Params "/target:Clean;Build /property:Configuration=Release;Platform=""Any CPU"" /verbosity:Minimal" -BypassVisualStudioDeveloperCommandPrompt -ShowBuildOutputInCurrentWindow
if ($buildAnyRelease.BuildSucceeded)
{ Write-Host "`nBuild of Any CPU Release completed successfully.`n" }
else
{ Write-Host "`nBuild of Any CPU Release failed.`n" }

if ($buildAnyRelease.BuildSucceeded)
{
	Write-Host "`nCreating package...`n"

	$invocation = (Get-Variable MyInvocation).Value
	$invocationPath = Split-Path $invocation.MyCommand.Path
	Write-Host "Invocation Path: "$invocationPath
	# get parent dir of $directorypath
	$parent = (get-item $invocationPath).Parent.FullName
	Write-Host "Parent dir: "$parent
	$directorypath = $parent

	#source
	$anyCpuPathRelease = $directorypath + "\bin\Release\*"

	#destination
	$destPath = "MimerConsumerConsoleApplication"
	#Write-Host $destPath

	Write-Host "Creating dir: "$destPath
	New-Item $destPath -force -type directory | out-null

	Write-Host "Getting child items for: "$anyCpuPathRelease
	Write-Host "Copying them to: "$destPath
	Get-ChildItem $anyCpuPathRelease -recurse | Copy-Item -destination $destPath -force

	# Path to 7-zip
	$7z = "C:\Program Files\7-Zip\7z.exe"
	$outZipName = "MimerConsumerConsoleApplication.sfx.exe"
	# 7-zip Arguments
	$7zArgs = "a", $outZipName, "-sfx", $destPath
	# Do the packing
	& $7z $7zArgs

	# Remove the "Package" dir
	Write-Host "Removing dir: "$destPath
	Remove-Item -recurse -force $destPath
}