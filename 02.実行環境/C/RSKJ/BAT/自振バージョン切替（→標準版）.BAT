@ECHO OFF
        ECHO □□□□□□□□□□□□□□□□□□□□□□□□□□□□□
        ECHO     自振バージョン変更（→標準版）
        ECHO.    自振システムとジョブ監視画面は終了させて実行してください
        ECHO.
        ECHO     処理開始               日付:%DATE% 時刻:%TIME:~0,8%
        ECHO □□□□□□□□□□□□□□□□□□□□□□□□□□□□□

:CHK1
        IF   EXIST %1 GOTO STR
        SET  CHECKANS=
        SET  /P CHECKANS=処理を実行しますか？[Y/N]:
        IF   /I "%CHECKANS%" == ""  GOTO CHK1
        IF   /I "%CHECKANS%" == "Y" GOTO STR
        IF   /I "%CHECKANS%" == "N" GOTO EXT2
        GOTO CHK1

:STR
        ECHO.
        ECHO ＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
        ECHO     自振バージョン変更（→標準版）
        ECHO ＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
        COPY D:\RS企業自振_V2.0\06.iniファイル管理\fskj（標準版）.ini C:\RSKJ\EXE\fskj.ini
        IF %ERRORLEVEL% NEQ 0 GOTO ERR1

:EXT1
        ECHO.
        ECHO ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        ECHO     自振バージョンが変更されました（→標準版）
        ECHO     　　　　               日付:%DATE% 時刻:%TIME:~0,8%
        ECHO ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        PAUSE
        EXIT /B 00

:EXT2
        ECHO.
        ECHO ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        ECHO     キャンセルされました
        ECHO     　　　　               日付:%DATE% 時刻:%TIME:~0,8%
        ECHO ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        PAUSE
        EXIT /B 80

:ERR1
        ECHO.
        ECHO ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        ECHO     自振バージョン変更失敗！！！
        ECHO     処理エラー（%ERRORLEVEL%）
        ECHO     異常終了               日付:%DATE% 時刻:%TIME:~0,8%
        ECHO ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        PAUSE
        EXIT /B 99

