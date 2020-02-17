@ECHO OFF
set ConfigurationName=%1
set SolutionDir=%~2
set AssistantPath="D:\Users\CarJem\source\personal_repos\GenerationsLib\GenerationsLib.UpdateAssistant\bin\x86\Release\GenerationsLib.UpdateAssistant.exe"
set SettingsPath="D:\Users\CarJem\source\rsdk_repos\ManiacEditor-GenerationsEdition\ManiacEditor\bin\Release\Settings"

rmdir %SettingsPath% /s /q

if %ConfigurationName% == "Publish" (
call "%SolutionDir%Installer\MakeInstaller.bat"
call "%AssistantPath%" "ManiacEditor"
)
if %ConfigurationName% == "Experiment" (
mkdir %SettingsPath%
call "%SolutionDir%Installer\MakeZIP.bat"
)