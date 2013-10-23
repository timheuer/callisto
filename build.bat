@echo off

@echo *******************************************
@echo * BUILDING SOLUTION IN RELEASE			*
@echo *******************************************
msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release,OutputPath=bin\Release\81-NuGet\ /property:GenerateLibraryLayout=false /p:NoWarn=0618 src\Callisto\Callisto.csproj
msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release,OutputPath=bin\Release\81-ExtSDK\ /property:DisableXbfGeneration=true /property:GenerateLibraryLayout=true /p:NoWarn=0618 src\Callisto\Callisto.csproj
msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release,OutputPath=bin\Release\81-Design src\Design\Callisto.Design\Callisto.Design.csproj

@echo *******************************************
@echo BUILDING DIRECTORY STRUCTURE FOR SDK		*
@echo *******************************************
mkdir SDK\Callisto\1.0.1\References\CommonConfiguration\neutral
mkdir SDK\Callisto\1.0.1\Redist\CommonConfiguration\neutral\Callisto\Themes

@echo *******************************************
@echo * COPYING BINARIES FOR EXTENSION SDK		*
@echo *******************************************
copy src\Callisto\bin\Release\81-ExtSDK\Callisto.dll SDK\Callisto\1.0.1\References\CommonConfiguration\neutral
copy src\Callisto\bin\Release\81-ExtSDK\Callisto\themes\generic.xaml SDK\Callisto\1.0.1\Redist\CommonConfiguration\neutral\Callisto\Themes
copy src\Callisto\bin\Release\81-ExtSDK\Callisto.pri SDK\Callisto\1.0.1\Redist\CommonConfiguration\neutral\
copy SDKManifest.xml SDK\Callisto\1.0.1\

@echo *******************************************
@echo * DONE BUILDING EXTENSION SDK LAYOUT		*
@echo *******************************************

pushd Tools\nuget

@echo *******************************************
@echo * COPYING BINARIES FOR NUGET              *
@echo *******************************************
mkdir .\Callisto\lib\netcore451
mkdir .\Callisto\lib\netcore451\Callisto
mkdir .\Callisto\lib\netcore451\Callisto\Themes
copy ..\..\src\Callisto\bin\release\81-NuGet\Callisto.dll .\Callisto\lib\netcore451\
copy ..\..\src\Callisto\bin\release\81-NuGet\Callisto.pri .\Callisto\lib\netcore451\
copy ..\..\src\Callisto\bin\release\81-NuGet\themes\generic.xbf .\Callisto\lib\netcore451\Callisto\Themes

@echo *******************************************
@echo * BUILDING NUGET PAKCAGE					*
@echo *******************************************
nuget pack Callisto\Callisto.nuspec -o .\

@echo *******************************************
@echo * DONE BUILDING NUGET - 					*
@echo * DON'T FORGET TO PUBLISH					*
@echo *******************************************

popd