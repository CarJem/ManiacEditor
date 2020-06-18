@echo off
SET ConfigurationName=%~1
SET SolutionDir=%~2
SET TargetDir=%~3
SET ProjectDir=%~4

SET CurrentDir=%~dp0

:: if not %ConfigurationName% == "Quick Debug"
CALL "%CurrentDir%\MoveBinaries.bat" "%ConfigurationName%" "%SolutionDir%" "%TargetDir%" "%ProjectDir%"
CALL "%CurrentDir%\PublishStep.bat" "%ConfigurationName%" "%SolutionDir%" "%TargetDir%" "%ProjectDir%"
	
