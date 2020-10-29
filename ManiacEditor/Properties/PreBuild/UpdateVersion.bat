@ECHO OFF
set ConfigurationName=%1
set SolutionDir=%~2
set TargetDir=%~3
set ProjectDir=%~4
set CurrentDir=%~dp0
call "%ProjectDir%Properties\Versioning\GenerationsLib.Versioning.exe" "%ProjectDir%Properties\AssemblyInfo.cs" "Maniac Editor" false false