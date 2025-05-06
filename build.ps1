dotnet build ConsoleApp1/ChromeController --configuration Release --output ./publish/ChromeController

$dllPath = Get-ChildItem ./publish/ChromeController/ChromeController*.dll | Select-Object -ExpandProperty FullName
Write-Host "ChromeController DLL path: $dllPath"

# Copy DLL file path to clipboard as file drop list
Add-Type -AssemblyName System.Windows.Forms
$fileDropList = New-Object System.Windows.Forms.DataObject
$fileDropList.SetData("FileDrop", [string[]]@($dllPath))
[System.Windows.Forms.Clipboard]::SetDataObject($fileDropList)
Write-Host "DLL file path copied to clipboard as file drop list"
