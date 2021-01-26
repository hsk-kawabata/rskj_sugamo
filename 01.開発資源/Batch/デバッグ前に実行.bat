@ECHO OFF

ECHO ビルドしています．．．
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" Batch.sln /Rebuild Release
COPY ..\DLL\*.dll ..\EXE\ /Y
COPY ..\DLL\*.pdb ..\EXE\ /Y
