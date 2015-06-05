cd ABus
nuget.exe pack
xcopy *.nupkg "D:\dev\NuGet" /F /Y
cd ..

cd ABus.AzureServiceBus
nuget.exe pack
xcopy *.nupkg "D:\dev\NuGet" /F /Y
cd ..

cd ABus.Host
nuget.exe pack
xcopy *.nupkg "D:\dev\NuGet" /F /Y
cd ..

cd ABus.Unity
nuget.exe pack
xcopy *.nupkg "D:\dev\NuGet" /F /Y
cd ..
