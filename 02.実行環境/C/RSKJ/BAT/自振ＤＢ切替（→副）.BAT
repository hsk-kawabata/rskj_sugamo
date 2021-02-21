@ECHO OFF
	ECHO □□□□□□□□□□□□□□□□□□□□□□□□□□□□□
	ECHO     自振ＤＢ接続先変更（→副）
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
	ECHO     ＤＢ接続先変更（→副）
	ECHO ＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
	copy C:\RSKJ\BAT\tnsnames（副).ora   D:\app\sanjo\product\11.2.0\client_1\network\admin\tnsnames.ora
	copy C:\RSKJ\BAT\fskj（副).ini  C:\RSKJ\EXE\fskj.ini
	IF %ERRORLEVEL% NEQ 0 GOTO ERR1
        
:STP2
	ECHO.
	ECHO ＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
	ECHO     ＤＢ接続先を確認してください
	ECHO ＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
	TNSPING RSKJ_LSNR
	EVENTCREATE /L APPLICATION /T Warning /SO "自振ＤＢ切替（→副）.BAT" /ID 999 /D "自振ＤＢの接続先が変更されました（→副）"
	PAUSE
	EXIT /B 00

:EXT1
	ECHO.
	ECHO ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
	ECHO     自振ＤＢの接続先が変更されました（→副）
	ECHO     　　　　               日付:%DATE% 時刻:%TIME:~0,8%
	ECHO ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
	PAUSE
	EXIT /B 80
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
	ECHO     ＤＢ接続先変更失敗！！！
	ECHO     処理エラー（%ERRORLEVEL%）
	ECHO     異常終了               日付:%DATE% 時刻:%TIME:~0,8%
	ECHO ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
	PAUSE
	EXIT /B 99

