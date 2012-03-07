@echo off

@echo *******************************************
@echo * BUILDING VSIX PACKAGE					*
@echo *******************************************
mkdir src\Callisto.VSIX\Redist\CommonConfiguration\neutral\Themes\
mkdir src\Callisto.VSIX\References\CommonConfiguration\neutral\
copy License.txt src\Callisto.VSIX\License.txt
copy SDKManifest.xml src\Callisto.VSIX\SDKManifest.xml
copy src\Callisto\bin\release\Callisto.dll src\Callisto.VSIX\References\CommonConfiguration\neutral\
copy src\Callisto\bin\release\Callisto.pri src\Callisto.VSIX\Redist\CommonConfiguration\neutral\
copy src\Callisto\bin\release\themes\generic.xaml src\Callisto.VSIX\Redist\CommonConfiguration\neutral\Themes\
msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release src\Callisto.VSIX\Callisto.VSIX.csproj

popd