@ECHO OFF
REM *******************************
REM  ＤＢ退避
REM *******************************
DEL D:\RSKJ\BKUP\EXPDBALL.DMP
SET ORACLE_SID=RSKJ
EXPDP KZFMAST/KZFMAST@RSKJ_LSNR DIRECTORY=RSBKUP SCHEMAS=KZFMAST DUMPFILE=EXPDBALL.DMP LOGFILE=EXP.LOG

REM *******************************
REM D:\RSKJフォルダのコピー　正→副
REM *******************************
REM xcopy D:\RSKJ\*.* \\ \rskj_d\*.* /s/e
