SET ManiacInput="D:\Users\CarJem\source\rsdk_repos\ManiacEditor-GenerationsEdition\ManiacEditor\bin\Release"
SET ManiacOutput="D:\Users\CarJem\source\rsdk_repos\ManiacEditor-GenerationsEdition\Installer\Build.zip"
echo {"PortableMode":true} > "%ManiacInput%\Settings\internal_switches.json"
powershell.exe Compress-Archive -Path %ManiacInput% -DestinationPath %ManiacOutput% -Force
powershell.exe Set-Clipboard -Value %ManiacOutput%