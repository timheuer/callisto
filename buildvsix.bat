@echo off

@echo *******************************************
@echo * BUILDING VSIX PACKAGE					*
@echo *******************************************
xcopy License.txt src\Callisto.VSIX\ /Y
xcopy SDKManifest.xml src\Callisto.VSIX\ /Y
xcopy CallistoMoon.png src\Callisto.VSIX\ /Y
xcopy CallistoLogo.png src\Callisto.VSIX\ /Y
xcopy src\Callisto\bin\release\Callisto.dll src\Callisto.VSIX\References\CommonConfiguration\neutral\ /Y
xcopy src\Callisto\bin\Callisto.xml src\Callisto.VSIX\References\CommonConfiguration\neutral\ /Y
xcopy src\Callisto\bin\release\Callisto.pri src\Callisto.VSIX\Redist\CommonConfiguration\neutral\ /Y
xcopy src\Callisto\bin\release\themes\generic.xaml src\Callisto.VSIX\Redist\CommonConfiguration\neutral\Themes\ /Y
xcopy src\Callisto\bin\release\Design\Callisto.Design.dll src\Callisto.VSIX\DesignTime\CommonConfiguration\neutral\ /Y
msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release src\Callisto.VSIX\Callisto.VSIX.csproj
del src\Callisto.VSIX\CallistoLogo.png
del src\Callisto.VSIX\CallistoMoon.png
popd