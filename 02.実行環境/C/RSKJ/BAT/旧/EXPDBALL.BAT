@ECHO OFF
REM *******************************
REM  ＤＢ退避
REM *******************************
SET ORACLE_SID=RSKJ
EXP.EXE KZFMAST/KZFMAST@RSKJ_LSNR COMPRESS=N CONSISTENT=N CONSTRAINTS=Y STATISTICS=NONE FEEDBACK=100000 FILE=D:\RSKJ\BKUP\EXPDBALL.DMP LOG=D:\RSKJ\BKUP\EXPDBALL.LOG

REM *******************************
REM D:\RSKJフォルダのコピー　正→副
REM *******************************
rmdir \\JIFURI-SV1\rskj_d\ /s/q
xcopy D:\RSKJ\*.* \\JIFURI-SV2\rskj_d\*.* /s/e

REM 副サーバにコピー
REM COPY D:\RSKJ\BKUP\EXPDBALL.DMP \\OT0019807\RSKJ\BKUP\EXPDBALL.DMP