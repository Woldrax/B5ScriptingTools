param(
	[Parameter()] $ProjectName,
	[Parameter()] $ConfigurationName,
	[Parameter()] $TargetDir
)

Copy 'B5ScriptingTools.dll' '.\B5ScriptingTools' -Force -Verbose