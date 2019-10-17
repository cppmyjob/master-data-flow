
RMDIR /S /Q %UserProfile%\Documents\MasterDataFlow
MKDIR %UserProfile%\Documents\MasterDataFlow

xcopy AFLT.save %UserProfile%\Documents\MasterDataFlow\AFLT.save* /Y
xcopy AFLT.csv %UserProfile%\Documents\MasterDataFlow\AFLT.csv* /Y

xcopy ..\bin\Debug\*.* "C:\Program Files\’Š›’ˆ…-à®ª¥à\" /Y

 