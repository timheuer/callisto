@echo off

@echo *******************************************
@echo * BUILDING SOLUTION IN RELEASE			*
@echo *******************************************
msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release src\Callisto\Callisto.csproj

@echo *******************************************
@echo BUILDING DIRECTORY STRUCTURE FOR SDK		*
@echo *******************************************
mkdir SDK\Callisto\1.0.1\References\CommonConfiguration\neutral
mkdir SDK\Callisto\1.0.1\Redist\CommonConfiguration\neutral\Callisto\Themes

@echo *******************************************
@echo * COPYING BINARIES						*
@echo *******************************************
copy src\Callisto\bin\Release\Callisto.dll SDK\Callisto\1.0.1\References\CommonConfiguration\neutral
copy src\Callisto\bin\Release\themes\generic.xaml SDK\Callisto\1.0.1\Redist\CommonConfiguration\neutral\Callisto\Themes
copy src\Callisto\bin\Release\Callisto.pri SDK\Callisto\1.0.1\Redist\CommonConfiguration\neutral\
copy SDKManifest.xml SDK\Callisto\1.0\

@echo *******************************************
@echo * DONE BUILDING SDK LAYOUT				*
@echo *******************************************

pushd Tools\nuget

@echo *******************************************
@echo * COPYING BINARIES						*
@echo *******************************************
mkdir .\Callisto\lib\winrt45
mkdir .\Callisto\lib\winrt45\Callisto
mkdir .\Callisto\lib\winrt45\Callisto\Themes
copy ..\..\src\Callisto\bin\release\Callisto.dll .\Callisto\lib\winrt45\
copy ..\..\src\Callisto\bin\release\Callisto.pri .\Callisto\lib\winrt45\
copy ..\..\src\Callisto\bin\release\themes\generic.xaml .\Callisto\lib\winrt45\Callisto\Themes

@echo *******************************************
@echo * BUILDING NUGET PAKCAGE					*
@echo *******************************************
nuget pack Callisto\Callisto.nuspec -o .\

@echo *******************************************
@echo * DONE BUILDING NUGET - 					*
@echo * DON'T FORGET TO PUBLISH					*
@echo *******************************************

popd