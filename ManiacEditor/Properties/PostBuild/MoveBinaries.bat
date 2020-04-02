@echo off
SET ConfigurationName=%1
SET SolutionDir=%~2
SET TargetDir=%~3
SET ProjectDir=%~4

:: Remove the Previous Lib Folder
if not %ConfigurationName% == "Debug" (
	rmdir "%TargetDir%Lib\ " /s /q
)

SET COPYCMD=/Y

:: Move the Binaries
move  "%TargetDir%*.dll" "%TargetDir%\Lib"
move  "%TargetDir%*.pdb" "%TargetDir%\Lib"
move  "%TargetDir%*.pdb" "%TargetDir%\Lib"

robocopy  "%TargetDir%\cs-CZ" "%TargetDir%\Lib\cs-CZ" /S /Move
robocopy  "%TargetDir%\de" "%TargetDir%\Lib\de" /S /Move
robocopy  "%TargetDir%\es" "%TargetDir%\Lib\es" /S /Move
robocopy  "%TargetDir%\fr" "%TargetDir%\Lib\fr" /S /Move
robocopy  "%TargetDir%\hu" "%TargetDir%\Lib\hu" /S /Move
robocopy  "%TargetDir%\it" "%TargetDir%\Lib\it" /S /Move
robocopy  "%TargetDir%\pt-BR" "%TargetDir%\Lib\pt-BR" /S /Move
robocopy  "%TargetDir%\ro" "%TargetDir%\Lib\ro" /S /Move
robocopy  "%TargetDir%\ru" "%TargetDir%\Lib\ru" /S /Move
robocopy  "%TargetDir%\sv" "%TargetDir%\Lib\sv" /S /Move
robocopy  "%TargetDir%\x64" "%TargetDir%\Lib\x64" /S /Move
robocopy  "%TargetDir%\x86" "%TargetDir%\Lib\x86" /S /Move
robocopy  "%TargetDir%\zh-Hans" "%TargetDir%\Lib\zh-Hans" /S /Move

move  "%TargetDir%libSkiaSharp.dylib" "%TargetDir%\Lib"
move  "%TargetDir%RSDKv5.dll.config" "%TargetDir%\Lib"
