echo off
sqlplus kzfmast/kzfmast@rskj_lsnr @C:\RSKJ\BAT\ユーザ一覧.sql

rem msg 0 印刷を実行してください
notepad /pt "C:\RSKJ\BAT\企業自振システム／ユーザ一覧表.LOG" "FUJITSU XL-9500_Pnavi2"
msg 0 印刷を実行しました
rem print "C:\RSKJ\BAT\企業自振システム／ユーザ一覧表.LOG"
rem pause
exit