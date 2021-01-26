SET YEAR=%date:~-10,4%
SET MON=%date:~-5,2%
SET DAY=%date:~-2,2%
SET DFDATE=%DATE:/=%
SET PATH1=%DFDATE%
SET PARA=%MON%-%DAY%-%YEAR%
SET CMD1=XCOPY D:\RSäÈã∆é©êU_V2.0\01.äJî≠éëåπ D:\%PATH1%\ /D:%PARA% /S /EXCLUDE:list.txt

pause 

%CMD1%

pause
