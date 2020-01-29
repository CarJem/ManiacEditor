echo off
set ConfigurationName=%1
set SolutionDir=%~2
set AssistantPath="D:\Users\CarJem\source\personal_repos\GenerationsLib\GenerationsLib.UpdateAssistant\bin\x86\Release\GenerationsLib.UpdateAssistant.exe"

if %ConfigurationName% == "Publish" (
call "%SolutionDir%Installer\MakeInstaller.bat"
call "%AssistantPath%" "ManiacEditor"
)