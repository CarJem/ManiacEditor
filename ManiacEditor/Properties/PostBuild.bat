@ECHO OFF
set ConfigurationName=%1
set SolutionDir=%~2
set TargetDir=%~3
set ProjectDir=%~4

rmdir "%TargetDir%Lib\ " /s /q

:: Move all assemblies and related files to lib folder
ROBOCOPY "%TargetDir% " "%TargetDir%Lib\ " /XF *.exe *.config *.manifest /XD Lib Settings Resources "Entity Renders" /E /IS /MOVE
if %errorlevel% leq 4 exit 0 else exit %errorlevel%

:: Common Paths for Extra Post Builds
set AssistantPath="D:\Users\CarJem\source\personal_repos\GenerationsLib\GenerationsLib.UpdateAssistant\bin\x86\Release\GenerationsLib.UpdateAssistant.exe"
set SettingsPath="%ProjectDir%\bin\Release\Settings\"

rmdir %SettingsPath% /s /q

:: Generate Installer and Prepare to Publish
if %ConfigurationName% == "Publish" (
	call "%SolutionDir%Installer\MakeInstaller.bat"
	call "%AssistantPath%" "ManiacEditor"
)


:: Generate a ZIP for a Experimental Testing Build
if %ConfigurationName% == "Experiment" (
	mkdir %SettingsPath%
	call "%SolutionDir%Installer\MakeZIP.bat"
)