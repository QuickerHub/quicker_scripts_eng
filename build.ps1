# Build the project
dotnet build ConsoleApp1/ChromeController --configuration Release --output ./publish/ChromeController

$dllPath = Get-ChildItem ./publish/ChromeController/ChromeController*.dll | Select-Object -ExpandProperty FullName
Write-Host "ChromeController DLL path: $dllPath"

# Create zip file with same name as DLL
$dllName = [System.IO.Path]::GetFileNameWithoutExtension($dllPath)
$zipPath = "./publish/ChromeController/$dllName.zip"
Compress-Archive -Path $dllPath -DestinationPath $zipPath -Force

# Copy zip file to clipboard
Add-Type -AssemblyName System.Windows.Forms
$fileDropList = New-Object System.Windows.Forms.DataObject
$fileDropList.SetData("FileDrop", [string[]]@($zipPath))
[System.Windows.Forms.Clipboard]::SetDataObject($fileDropList)
Write-Host "ZIP file path copied to clipboard as file drop list"
