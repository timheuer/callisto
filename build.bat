@echo off

@echo *******************************************
@echo * BUILDING SOLUTION IN RELEASE			*
@echo *******************************************
msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release,OutputPath=bin\Release\81-NuGet\ /property:GenerateLibraryLayout=false /p:NoWarn=0618 src\Callisto\Callisto.csproj
msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release,OutputPath=bin\Release\81-ExtSDK\ /property:GenerateLibraryLayout=true /p:NoWarn=0618 src\Callisto\Callisto.csproj
xcopy src\Callisto\themes\generic.xaml src\Design\Callisto.Design\Themes\ /Y
msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release,OutputPath=bin\Release\81-Design src\Design\Callisto.Design\Callisto.Design.csproj

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

@echo *******************************************
@echo * BUILDING VSIX PACKAGE					*
@echo *******************************************
xcopy License.txt src\Callisto.VSIX\ /Y
xcopy SDKManifest.xml src\Callisto.VSIX\ /Y
xcopy CallistoMoon.png src\Callisto.VSIX\ /Y
xcopy CallistoLogo.png src\Callisto.VSIX\ /Y
xcopy src\Callisto\bin\release\81-ExtSDK\Callisto.dll src\Callisto.VSIX\References\CommonConfiguration\neutral\ /Y
xcopy src\Callisto\bin\Callisto.xml src\Callisto.VSIX\References\CommonConfiguration\neutral\ /Y
xcopy src\Callisto\bin\release\81-ExtSDK\Callisto.pri src\Callisto.VSIX\Redist\CommonConfiguration\neutral\ /Y
xcopy src\Callisto\bin\release\81-ExtSDK\Callisto\themes\generic.xbf src\Callisto.VSIX\Redist\CommonConfiguration\neutral\Themes\ /Y
xcopy src\Design\Callisto.Design\bin\release\81-Design\Callisto.Design.dll src\Callisto.VSIX\DesignTime\CommonConfiguration\neutral\ /Y
xcopy src\Callisto\themes\generic.xaml src\Callisto.VSIX\DesignTime\CommonConfiguration\neutral\Themes\ /Y
msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release src\Callisto.VSIX\Callisto.VSIX.csproj
del src\Callisto.VSIX\CallistoLogo.png
del src\Callisto.VSIX\CallistoMoon.png
popd