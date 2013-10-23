@echo off

@echo *******************************************
@echo * BUILDING SOLUTION IN RELEASE			*
@echo *******************************************
msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release,OutputPath=bin\Release\81\,DefineConstants="NETFX_CORE_451" src\Callisto\Callisto.csproj

@echo *******************************************
@echo BUILDING DIRECTORY STRUCTURE FOR SDK		*
@echo *******************************************
mkdir SDK\Callisto\1.0.1\References\CommonConfiguration\neutral
mkdir SDK\Callisto\1.0.1\Redist\CommonConfiguration\neutral\Callisto\Themes

@echo *******************************************
@echo * COPYING BINARIES						*
@echo *******************************************
copy src\Callisto\bin\Release\81\Callisto.dll SDK\Callisto\1.0.1\References\CommonConfiguration\neutral
copy src\Callisto\bin\Release\81\themes\generic.xaml SDK\Callisto\1.0.1\Redist\CommonConfiguration\neutral\Callisto\Themes
copy src\Callisto\bin\Release\81\Callisto.pri SDK\Callisto\1.0.1\Redist\CommonConfiguration\neutral\
copy SDKManifest.xml SDK\Callisto\1.0.1\

@echo *******************************************
@echo * DONE BUILDING SDK LAYOUT				*
@echo *******************************************

pushd Tools\nuget

@echo *******************************************
@echo * COPYING BINARIES for NETFX Core 4.5.1			*
@echo *******************************************
mkdir .\Callisto\lib\netcore451
mkdir .\Callisto\lib\netcore451\Callisto
mkdir .\Callisto\lib\netcore451\Callisto\Themes
copy ..\..\src\Callisto\bin\release\81\Callisto.dll .\Callisto\lib\netcore451\
copy ..\..\src\Callisto\bin\release\81\Callisto.pri .\Callisto\lib\netcore451\
copy ..\..\src\Callisto\bin\release\81\themes\generic.xaml .\Callisto\lib\netcore451\Callisto\Themes

@echo *******************************************
@echo * BUILDING NUGET PAKCAGE					*
@echo *******************************************
nuget pack Callisto\Callisto.nuspec -o .\

@echo *******************************************
@echo * DONE BUILDING NUGET - 					*
@echo * DON'T FORGET TO PUBLISH					*
@echo *******************************************

popd