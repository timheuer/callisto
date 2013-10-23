@echo off

@echo *******************************************
@echo * BUILDING VSIX PACKAGE					*
@echo *******************************************
mkdir src\Callisto.VSIX\Redist\CommonConfiguration\neutral\Themes\
mkdir src\Callisto.VSIX\References\CommonConfiguration\neutral\
copy License.txt src\Callisto.VSIX\License.txt
copy SDKManifest.xml src\Callisto.VSIX\SDKManifest.xml
copy CallistoMoon.png src\Callisto.VSIX\CallistoMoon.png
copy CallistoLogo.png src\Callisto.VSIX\CallistoLogo.png
copy src\Callisto\bin\release\81-ExtSDK\Callisto.dll src\Callisto.VSIX\References\CommonConfiguration\neutral\
copy src\Callisto\bin\release\81-ExtSDK\Callisto.pri src\Callisto.VSIX\Redist\CommonConfiguration\neutral\
copy src\Callisto\bin\release\81-ExtSDK\Callisto\themes\generic.xaml src\Callisto.VSIX\Redist\CommonConfiguration\neutral\Themes\
msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release src\Callisto.VSIX\Callisto.VSIX.csproj
del src\Callisto.VSIX\CallistoLogo.png
del src\Callisto.VSIX\CallistoMoon.png
popd