Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon

' レポート基本クラス
Public Class ClsReportBase
    Protected Structure PARAREPORT
        Dim Tenmei As String        ' 店名  
        Dim ReportName As String    ' 帳票名
        Dim Itakusyamei As String   ' 委託者名
        Dim Hiduke As String        ' 日付
        Dim HostCsvName As String   ' ホスト保存用ＣＳＶファイル
        ReadOnly Property CsvName() As String
            Get
                Dim sBuff As String = ""
                If Not Tenmei Is Nothing AndAlso Tenmei <> "" Then
                    sBuff &= Tenmei & "_"
                End If
                If Not ReportName Is Nothing AndAlso ReportName <> "" Then
                    sBuff &= ReportName & "_"
                End If
                If Not Itakusyamei Is Nothing AndAlso Itakusyamei <> "" Then
                    sBuff &= Itakusyamei & "_"
                End If
                If Not Hiduke Is Nothing AndAlso Hiduke <> "" Then
                    sBuff &= Hiduke & "_"
                Else
                    ' sBuff &= CASTCommon.Calendar.Now.ToString("MMdd") & "_" 
                    sBuff &= CASTCommon.Calendar.Now.ToString("yyyyMMddHHmmss") & "_" & Process.GetCurrentProcess.Id & "_"
                End If

                Return ValidPath(sBuff)
            End Get
        End Property
    End Structure
    Protected InfoReport As PARAREPORT
    ' ＣＳＶ名
    Protected CsvName As String
    Public ReadOnly Property FileName() As String
        Get
            Return CsvName
        End Get
    End Property
    ' ＣＳＶパス
    Protected CsvPath As String = GetFSKJIni("COMMON", "PRT")

    ' 帳票定義名
    Protected ReportBaseName As String

    ' ＣＳＶ用公開クラス
    Public CSVObject As CSV

    ' メモリ
    Public MEMObject As MEM

    ' レポート公開メッセージ
    Public ReportMessage As String

    ' 印刷部数
    Public Copies As Integer = 1
    ' 文字コード
    Public DataCode As Integer = 0
    Public Enum DataCodeType
        SHIFT_JIS = 0
        UTF8 = 1
        UTF16 = 2
    End Enum

    ' 店名プロパティ
    Public Property Tenmei() As String
        Get
            Return InfoReport.Tenmei
        End Get
        Set(ByVal Value As String)
            InfoReport.Tenmei = Value
        End Set
    End Property

    ' 委託者名プロパティ
    Public Property Itakusyamei() As String
        Get
            Return InfoReport.Itakusyamei
        End Get
        Set(ByVal Value As String)
            InfoReport.Itakusyamei = Value
        End Set
    End Property

    ' 日付プロパティ
    Public Property Hiduke() As String
        Get
            Return InfoReport.Hiduke
        End Get
        Set(ByVal Value As String)
            InfoReport.Hiduke = Value
        End Set
    End Property

    ' ホスト保存用ＣＳＶファイル
    Public Property HostCsvName() As String
        Get
            Return InfoReport.HostCsvName
        End Get
        Set(ByVal Value As String)
            InfoReport.HostCsvName = Value
        End Set
    End Property

    ' ＯＲＡＣＬＥクラス
    Protected MainDB As MyOracle
    Public WriteOnly Property OraDB() As MyOracle
        Set(ByVal Value As CASTCommon.MyOracle)
            MainDB = Value
        End Set
    End Property

    ' プリンタ名
    Private Shared mPrinter(4) As String

    ' プリンタ名称取得
    Private Shared Sub GetPrinterName()
        For i As Integer = 0 To 4
            mPrinter(i) = GetFSKJIni("COMMON", "PRINTER_" & (i + 1).ToString)
            If mPrinter(i) = "err" Then
                mPrinter(i) = ""
            End If
        Next i
    End Sub

    Public Enum PrinterType As Integer
        Printer3F = 0
        Printer5F = 1
    End Enum

    ' プリンタ名称取得
    ' PRINTER_1 ～ PRINTER_5
    ' nIndex 1 ～ 5 を指定する
    Public Shared ReadOnly Property PrinterName(ByVal prnType As PrinterType) As String
        Get
            Try
                If mPrinter(0) Is Nothing Then
                    Call ClsReportBase.GetPrinterName()
                End If
                Return ClsReportBase.mPrinter(prnType)
            Catch ex As Exception
                Return ""
            End Try
        End Get
    End Property

    '
    ' 機能　 ： 印刷（プリンタ指定）
    '
    ' 引数　 ： ARG1 - プリンタ名
    '
    ' 戻り値 ： True-成功，False-失敗
    '
    ' 備考　 ： 
    '
    Public Shared Function DetectPrinter(ByVal printerName As String) As Boolean
        'プリンタ情報を取得する
        Try
            Dim pinfo As PRINTER_INFO_2 = GetPrinterInfo(printerName)
            If GetString(pinfo.pDriverName).Trim = "" Then
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    ' New
    Sub New()
        ' プリンタ名称取得
        Call GetPrinterName()
    End Sub

    '
    ' 機能　 ： ＣＳＶファイル作成
    '
    ' 戻り値 ： ＣＳＶファイル名
    '
    ' 備考　 ： 
    '
    Public Overridable Function CreateCsvFile() As String
        ' ファイル名決定
        Dim sWork As String = Path.Combine(CsvPath, InfoReport.CsvName & "{0}.csv")
        For nKaiji As Integer = 1 To Integer.MaxValue - 1
            Dim fl As New FileInfo(String.Format(sWork, nKaiji))
            If fl.Exists() = False Then
                Try
                    Dim fs As FileStream = fl.Create()
                    ' ファイル作成が成功したので抜ける
                    CsvName = fl.FullName
                    fs.Close()
                    Exit For
                Catch ex As Exception
                    Exit For
                End Try
            End If
        Next nKaiji

        Call OpenCsv()

        Return CsvName
    End Function

    '
    ' 機能　 ： メモリストリーム作成
    '
    ' 備考　 ： 
    '
    Public Overridable Sub CreateMemFile()
        MEMObject = New MEM
        MEMObject.Open()
    End Sub

    '
    ' 機能　 ： 印刷実行
    '
    ' 備考　 ： 
    '
    Public Overridable Overloads Function ReportExecute() As Boolean
        Return ReportExecute("")
    End Function

    '
    ' 機能　 ： 印刷実行
    '
    ' 備考　 ： 
    '
    Public Overridable Overloads Function ReportExecute(ByVal prnType As PrinterType) As Boolean
        Return ReportExecute(PrinterName(prnType))
    End Function

    '
    ' 機能　 ： 印刷実行
    '
    ' 備考　 ： 
    '
    Public Overridable Overloads Function ReportExecute(ByVal printerName As String) As Boolean
        Dim repo As RAX
        Try
            Call CloseCsv()

            repo = New RAX(ReportBaseName)
            repo.CsvName = CsvName
            repo.Copies = Copies
            repo.DataCode = DataCode

            ' 2016/06/08 タスク）綾部 CHG 【RD】UI_B-14-99(RSV2対応) -------------------- START
            '' 印刷実行
            'repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
            'If Directory.Exists(repo.LogPath) = False Then
            '    Directory.CreateDirectory(repo.LogPath)
            'End If
            'repo.LogName = repo.ReportName & ".LOG"
            'If printerName Is Nothing OrElse printerName.Trim = "" Then
            '    repo.PrintOut()
            'Else
            '    repo.PrintOut(printerName)
            'End If

            'ReportMessage = repo.Message
            'If Not ReportMessage Is Nothing And ReportMessage <> "" Then
            '    Return False
            'End If
            Dim GetIniReturn As String = String.Empty
            Dim PrinterSetNum As Integer = -1

            If printerName = "" Then
                GetIniReturn = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "SET_NUM")
                Select Case GetIniReturn
                    Case "err", ""
                        PrinterSetNum = -1
                    Case Else
                        PrinterSetNum = CInt(Trim(GetIniReturn))
                End Select
            End If

            Select Case PrinterSetNum
                Case -1
                    '================================================================
                    ' 印刷実行(RSV1方式)
                    '================================================================
                    repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
                    If Directory.Exists(repo.LogPath) = False Then
                        Directory.CreateDirectory(repo.LogPath)
                    End If
                    repo.LogName = repo.ReportName & ".LOG"
                    If printerName Is Nothing OrElse printerName.Trim = "" Then
                        repo.PrintOut()
                    Else
                        repo.PrintOut(printerName)
                    End If

                    ReportMessage = repo.Message
                    If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                        Return False
                    End If
                Case Else
                    '================================================================
                    ' 印刷実行(RSV2方式)
                    '================================================================
                    For i As Integer = 1 To PrinterSetNum Step 1
                        printerName = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "PRINTER" & i.ToString)
                        Dim PrnIni_PrintBusuu As Integer = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "BUSUU" & i.ToString)
                        Dim PrnIni_SpoolName As String = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "SPOOL" & i.ToString)

                        ' 2017/04/17 タスク）綾部 CHG 【ME】UI_99-99(RSV2対応 機能拡張) -------------------- START
                        Dim PrnIni_ReportBaseName As String = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "RPDNAME" & i.ToString)
                        Select Case PrnIni_ReportBaseName
                            Case "err", ""
                            Case Else
                                repo = New RAX(PrnIni_ReportBaseName)
                                repo.CsvName = CsvName
                                repo.Copies = Copies
                                repo.DataCode = DataCode
                        End Select
                        ' 2017/04/17 タスク）綾部 CHG 【ME】UI_99-99(RSV2対応 機能拡張) -------------------- END

                        For j As Integer = 1 To PrnIni_PrintBusuu Step 1
                            repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
                            If Directory.Exists(repo.LogPath) = False Then
                                Directory.CreateDirectory(repo.LogPath)
                            End If
                            repo.LogName = repo.ReportName & ".LOG"
                            Select Case PrnIni_SpoolName
                                Case ""
                                    If printerName Is Nothing OrElse printerName.Trim = "" Then
                                        repo.PrintOut()
                                    Else
                                        repo.PrintOut(printerName)
                                    End If
                                Case Else
                                    If printerName Is Nothing OrElse printerName.Trim = "" Then
                                        repo.PrintOut("", PrnIni_SpoolName)
                                    Else
                                        repo.PrintOut(printerName, PrnIni_SpoolName)
                                    End If
                            End Select

                            ReportMessage = repo.Message
                            If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                                Return False
                            End If
                        Next
                    Next
            End Select
            ' 2016/06/08 タスク）綾部 CHG 【RD】UI_B-14-99(RSV2対応) -------------------- END

        Catch ex As Exception
            ReportMessage = ex.ToString    '2010/02/25 エラー情報を返却
            Return False
        Finally
            repo = Nothing
        End Try

        Return True
    End Function

    '
    ' 機能　 ： 印刷実行(学校用)
    '
    ' 備考　 ： 学校用印刷関数(2010/02/25)
    '
    Public Overridable Overloads Function G_ReportExecute(ByVal printerName As String) As Boolean
        Dim repo As CAstReports.RAX
        Try
            Call CloseCsv()

            repo = New CAstReports.RAX(CASTCommon.GetFSKJIni("GCOMMON", "LST"), ReportBaseName)
            repo.CsvName = CsvName
            repo.Copies = Copies
            repo.DataCode = DataCode

            ' 2016/06/08 タスク）綾部 CHG 【RD】UI_B-14-99(RSV2対応) -------------------- START
            '' 印刷実行
            'repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
            'If Directory.Exists(repo.LogPath) = False Then
            '    Directory.CreateDirectory(repo.LogPath)
            'End If
            'repo.LogName = repo.ReportName & ".LOG"
            'If printerName Is Nothing OrElse printerName.Trim = "" Then
            '    repo.PrintOut()
            'Else
            '    repo.PrintOut(printerName)
            'End If

            'ReportMessage = repo.Message
            'If Not ReportMessage Is Nothing And ReportMessage <> "" Then
            '    Return False
            'End If
            Dim GetIniReturn As String = String.Empty
            Dim PrinterSetNum As Integer = -1

            If printerName = "" Then
                GetIniReturn = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "SET_NUM")
                Select Case GetIniReturn
                    Case "err", ""
                        PrinterSetNum = -1
                    Case Else
                        PrinterSetNum = CInt(Trim(GetIniReturn))
                End Select
            End If

            Select Case PrinterSetNum
                Case -1
                    '================================================================
                    ' 印刷実行(RSV1方式)
                    '================================================================
                    repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
                    If Directory.Exists(repo.LogPath) = False Then
                        Directory.CreateDirectory(repo.LogPath)
                    End If
                    repo.LogName = repo.ReportName & ".LOG"
                    If printerName Is Nothing OrElse printerName.Trim = "" Then
                        repo.PrintOut()
                    Else
                        repo.PrintOut(printerName)
                    End If

                    ReportMessage = repo.Message
                    If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                        Return False
                    End If
                Case Else
                    '================================================================
                    ' 印刷実行(RSV2方式)
                    '================================================================
                    For i As Integer = 1 To PrinterSetNum Step 1
                        printerName = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "PRINTER" & i.ToString)
                        Dim PrnIni_PrintBusuu As Integer = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "BUSUU" & i.ToString)
                        Dim PrnIni_SpoolName As String = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "SPOOL" & i.ToString)

                        ' 2017/04/17 タスク）綾部 CHG 【ME】UI_99-99(RSV2対応 機能拡張) -------------------- START
                        Dim PrnIni_ReportBaseName As String = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "RPDNAME" & i.ToString)
                        Select Case PrnIni_ReportBaseName
                            Case "err", ""
                            Case Else
                                repo = New CAstReports.RAX(CASTCommon.GetFSKJIni("GCOMMON", "LST"), PrnIni_ReportBaseName)
                                repo.CsvName = CsvName
                                repo.Copies = Copies
                                repo.DataCode = DataCode
                        End Select
                        ' 2017/04/17 タスク）綾部 CHG 【ME】UI_99-99(RSV2対応 機能拡張) -------------------- END

                        For j As Integer = 1 To PrnIni_PrintBusuu Step 1
                            repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
                            If Directory.Exists(repo.LogPath) = False Then
                                Directory.CreateDirectory(repo.LogPath)
                            End If
                            repo.LogName = repo.ReportName & ".LOG"
                            Select Case PrnIni_SpoolName
                                Case ""
                                    If printerName Is Nothing OrElse printerName.Trim = "" Then
                                        repo.PrintOut()
                                    Else
                                        repo.PrintOut(printerName)
                                    End If
                                Case Else
                                    If printerName Is Nothing OrElse printerName.Trim = "" Then
                                        repo.PrintOut("", PrnIni_SpoolName)
                                    Else
                                        repo.PrintOut(printerName, PrnIni_SpoolName)
                                    End If
                            End Select

                            ReportMessage = repo.Message
                            If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                                Return False
                            End If
                        Next
                    Next
            End Select
            ' 2016/06/08 タスク）綾部 CHG 【RD】UI_B-14-99(RSV2対応) -------------------- END

        Catch ex As Exception
            ReportMessage = ex.ToString
            Return False
        Finally
            repo = Nothing
        End Try

        Return True
    End Function

    '
    ' 機能　 ： 印刷実行(WEB伝送用)
    '
    ' 備考　 ： WEB伝送対応(2012/06/30) ※標準版
    '
    Public Overridable Overloads Function ReportExecute(ByVal printerName As String, ByVal USER_ID As String, ByVal ITAKU_CODE As String) As Boolean
        Dim repo As RAX
        
        Try
            Call CloseCsv()

            repo = New RAX(ReportBaseName)
            repo.CsvName = CsvName
            repo.Copies = Copies
            repo.DataCode = DataCode

            ' 印刷実行
            repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
            If Directory.Exists(repo.LogPath) = False Then
                Directory.CreateDirectory(repo.LogPath)
            End If
            repo.LogName = repo.ReportName & ".LOG"
            If printerName Is Nothing OrElse printerName.Trim = "" Then
                repo.PrintOut()
            Else
                repo.PrintOut(printerName, USER_ID, ITAKU_CODE)
            End If

            ReportMessage = repo.Message
            If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                Return False
            End If

        Catch ex As Exception
            ReportMessage = ex.ToString    '2010/02/25 エラー情報を返却
            Return False
        Finally
            repo = Nothing
        End Try

        Return True
    End Function

    '2017/04/25 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
    '
    ' 機能　 ： 印刷実行
    '
    ' 備考　 ： 2014/07/02 奈良信金　WEB伝送対応
    '
    Public Overridable Overloads Function G_ReportExecute(ByVal printerName As String, ByVal USER_ID As String, ByVal ITAKU_CODE As String) As Boolean
        Dim repo As RAX

        Try
            Call CloseCsv()

            repo = New CAstReports.RAX(CASTCommon.GetFSKJIni("GCOMMON", "LST"), ReportBaseName)
            repo.CsvName = CsvName
            repo.Copies = Copies
            repo.DataCode = DataCode

            ' 印刷実行
            repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
            If Directory.Exists(repo.LogPath) = False Then
                Directory.CreateDirectory(repo.LogPath)
            End If
            repo.LogName = repo.ReportName & ".LOG"
            If printerName Is Nothing OrElse printerName.Trim = "" Then
                repo.PrintOut()
            Else
                repo.PrintOut(printerName, USER_ID, ITAKU_CODE)
            End If

            ReportMessage = repo.Message
            If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                Return False
            End If

        Catch ex As Exception
            ReportMessage = ex.ToString    '2010/02/25 エラー情報を返却
            Return False
        Finally
            repo = Nothing
        End Try

        Return True
    End Function
    '2017/04/25 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END

    '*** Str Add 2015/12/04 SO)沖野 for 拡張印刷対応 ***

    ' 機能   ： 指定されたプリンタ名配列の順番で拡張印刷を行う
    ' 引数   ： repo  RepoAgent
    '           prtDspName  印刷名 （例： 帳票ID_帳票名）
    '           printerArray  プリンタ名配列（通常使うプリンタのみの場合はNothing、配列中の通常使うプリンタ名は空文字）
    ' 戻り値 :  True-成功, False-失敗
    '
    Public Function Extend_ReportExecute(ByVal repo As RAX, ByVal prtDspName As String, _
                                         ByVal printerArray As String()) As Boolean

        Dim prtRet As Boolean = False

        Dim LOG As CASTCommon.BatchLOG = New CASTCommon.BatchLOG("CAstReports", "ClsReportBase")

        ' 処理開始ログ出力
        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("ClsReportBase.Extend_ReportExecute")

        Try
            ' 印刷実行
            If printerArray Is Nothing Then
                ' 通常使うプリンタのみの場合
                printerArray = New String(0) {}
                printerArray(0) = ""
            End If

            For printerNo As Integer = 0 To printerArray.Length - 1
                prtRet = repo.ExtendPrintOut(printerArray(printerNo), prtDspName)

               If prtRet = False Then
                   ReportMessage = repo.Message
                   Return prtRet
               End If
            Next

            Return prtRet

        Catch ex As Exception
            ReportMessage = ex.ToString
            Return prtRet

        Finally
            ' 処理終了ログ出力
            LOG.Write_Exit3(sw, "ClsReportBase.Extend_ReportExecute", "復帰値=" & prtRet)
        End Try

    End Function

    '*** End Add 2015/12/04 SO)沖野 for 拡張印刷対応 ***

    '===================================================================

    '
    ' 機能　 ： プレビュー実行
    '
    ' 備考　 ： 
    '
    Public Function PreviewExecute() As Boolean
        Dim repo As RAX
        Try
            Call CloseCsv()

            repo = New RAX(ReportBaseName)
            repo.CsvName = CsvName

            ' 印刷実行
            repo.LogPath = repo.ReportPath
            repo.LogName = repo.ReportName & ".LOG"
            repo.Preview()

            ReportMessage = repo.Message
            If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            repo = Nothing
        End Try

        Return True
    End Function

    '2018/03/09 saitou 広島信金(RSV2標準) ADD 学校用プレビュー印刷 -------------------- START
    Public Function G_PreviewExecute() As Boolean
        Dim repo As RAX
        Try
            Call CloseCsv()

            repo = New CAstReports.RAX(CASTCommon.GetFSKJIni("GCOMMON", "LST"), ReportBaseName)
            repo.CsvName = CsvName

            ' 印刷実行
            repo.LogPath = repo.ReportPath
            repo.LogName = repo.ReportName & ".LOG"
            repo.Preview()

            ReportMessage = repo.Message
            If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            repo = Nothing
        End Try

        Return True
    End Function
    '2018/03/09 saitou 広島信金(RSV2標準) ADD ----------------------------------------- END

    '
    ' 機能　 ： ＣＳＶファイルを開く
    '
    ' 備考　 ： 
    '
    Public Sub OpenCsv()
        CSVObject = New CSV
        CSVObject.Open(CsvName, True)
    End Sub

    '
    ' 機能　 ： ＣＳＶファイルを閉じる
    '
    ' 備考　 ： 
    '
    Public Sub CloseCsv()
        If Not CSVObject Is Nothing Then
            CSVObject.Close()
        End If
        CSVObject = Nothing
    End Sub

    '機能   :   ソート実行
    '
    '備考   :   
    '
    Public Sub SortFile(ByVal keyItem As String, Optional ByVal SkipRec As String = "")
        ' 配列に読込
        Dim arraySort As New ArrayList(2048)        ' ソート用配列
        Try
            '2017/12/11 saitou 広島信金(RSV2標準) UPD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START
            Dim PowerSort As New clsPowerSORT
            PowerSort.DisposalNumber = 0        ' ソート機能
            PowerSort.FieldDefinition = 0       ' 浮動フィールド
            PowerSort.FieldDelimiter = ","      ' フィールド分離文字
            PowerSort.KeyCmdStr = keyItem       ' ソートキー
            If SkipRec <> "" Then
                PowerSort.InputFilesSkiprec = SkipRec
            End If
            PowerSort.InputFiles = CsvName      ' ファイル名
            PowerSort.InputFileType = 0         ' テキストファイル
            PowerSort.OutputFile = CsvName & ".sort.csv"
            PowerSort.OutputFileType = 0
            PowerSort.MaxRecordLength = 1000
            PowerSort.Action()

            If PowerSort.ErrorCode <> 0 Then
                Return
            End If

            'Dim FrmSort As New FrmPowerSort
            'Dim PowerSort As AxPowerSORT_Lib.AxPowerSORT = FrmSort.objAxPowerSORT
            'PowerSort.DispMessage = False
            'PowerSort.DisposalNumber = 0        ' ソート機能
            'PowerSort.FieldDefinition = 0       ' 浮動フィールド
            'PowerSort.FieldDelimiter = "','"    ' フィールド分離文字
            'PowerSort.KeyCmdStr = keyItem       ' "0.9sjia"     ' ソートキー
            'If SkipRec <> "" Then
            '    PowerSort.InputFilesSkiprec = SkipRec
            'End If
            'PowerSort.InputFiles = CsvName      ' ファイル名
            'PowerSort.InputFileType = 0         ' テキストファイル
            'PowerSort.OutputFile = CsvName & ".sort.csv"
            'PowerSort.OutputFileType = 0
            'PowerSort.MaxRecordLength = 1000
            'PowerSort.Action()

            'If PowerSort.ErrorCode <> 0 Then
            '    Return
            'End If
            '2017/12/11 saitou 広島信金(RSV2標準) UPD ------------------------------------------------------------ END

            If File.Exists(CsvName) = True Then
                File.Delete(CsvName)
            End If
            File.Move(PowerSort.OutputFile, CsvName)
        Catch ex As Exception
            Return
        End Try

    End Sub

    '電子帳票化のため追加
    Private Function CreateFTPFile(ByVal linedata() As String) As Boolean
        '==========================================================================
        '概要-------FTP送信のためのCSVファイル･制御ファイル・ダミーファイルを作成し
        '           FTP送信を行う
        '引数-------FTP_Data TXTの1行データ
        '戻り値-----実行結果
        '==========================================================================
        Dim FTPFileName As String = linedata(3)
        Dim FTPFileNameFull As String
        Dim WorkDir As String = Path.Combine(CsvPath, "WrkFTP" & Process.GetCurrentProcess.Id)

        Try
            'ファイル名設定
            FTPFileName = FTPFileName.Replace("YYYYMMDDHHMMSS", Now.ToString("yyyyMMddHHmmss"))
            FTPFileName = FTPFileName.Replace("99999", Process.GetCurrentProcess.Id.ToString("00000"))
            FTPFileNameFull = Path.Combine(WorkDir, FTPFileName)

            'ファイルをワークフォルダにコピー
            Directory.CreateDirectory(WorkDir)
            File.Copy(CsvName, FTPFileNameFull, True)

            '制御ファイルの作成
            Dim sw As StreamWriter = New StreamWriter(FTPFileNameFull & ".flw", False, Encoding.GetEncoding(932))
            sw.WriteLine("fm-printerentry = ReportViewer")              '固定
            sw.WriteLine("fm-printer = " & FTPFileName.Split("@"c)(0))  'RV登録のホスト転送ファイル名(FTPファイル名の@から前を設定する)
            sw.WriteLine("fm-formentry = dummy")                        '固定
            sw.WriteLine("fm-formfilename = " & linedata(8))            'FMEditorで作成したフォーム名
            sw.Close()

            'ダミーファイル作成
            Dim fs As FileStream = File.Create(FTPFileNameFull & ".dmy")
            fs.Close()

            'FTP送信
            If FTPExecute(FTPFileNameFull, linedata) = False Then
                Return False
            End If

            '正常終了時はPRT/FTPBKフォルダにファイル移動
            Dim FtpDir As String = Path.Combine(CsvPath, "FTPBK")
            If Directory.Exists(FtpDir) = False Then
                Directory.CreateDirectory(FtpDir)
            End If

            File.Move(FTPFileNameFull, Path.Combine(FtpDir, FTPFileName))
            File.Move(FTPFileNameFull & ".flw", Path.Combine(FtpDir, FTPFileName) & ".flw")
            File.Move(FTPFileNameFull & ".dmy", Path.Combine(FtpDir, FTPFileName) & ".dmy")

        Catch ex As Exception
            ReportMessage = "FTP送信ファイルの作成に失敗しました。"
            Return False
        Finally
            Try
                Directory.Delete(WorkDir, True)
            Catch ex As Exception
            End Try
        End Try

        Return True
    End Function

    Private Function FTPExecute(ByVal FTPFileName As String, ByVal linedata() As String) As Boolean
        '==========================================================================
        '概要-------FTP送信を行う
        '引数-------FTP_FileName ファイル名,FTP_Data() TXTの1行データ
        '戻り値-----実行結果
        '==========================================================================

        Dim FTPHOST As String = linedata(6)
        Dim FTPPATH As String = linedata(7)

        Dim strUSERID As String = linedata(4)
        Dim strPASSWORD As String = linedata(5)

        Try
            Dim WorkDir As String = Path.GetDirectoryName(FTPFileName)
            Dim FTPText As String = Path.Combine(WorkDir, "ftpcmd.txt")

            'FTP.EXEに送るコマンドをテキスト出力する
            Dim sw As StreamWriter = New StreamWriter(FTPText, False, Encoding.GetEncoding(932))
            sw.AutoFlush = False
            sw.WriteLine("OPEN " & FTPHOST)
            sw.WriteLine(strUSERID)
            sw.WriteLine(strPASSWORD)
            sw.WriteLine("BINARY")
            If Not FTPPATH = "" Then
                sw.WriteLine("CD " & FTPPATH)
            End If
            sw.WriteLine("PUT " & Path.GetFileName(FTPFileName))
            sw.WriteLine("PUT " & Path.GetFileName(FTPFileName) & ".flw")
            sw.WriteLine("PUT " & Path.GetFileName(FTPFileName) & ".dmy")
            sw.WriteLine("QUIT")
            sw.Close()

            Dim psInfo As New ProcessStartInfo
            psInfo.FileName = "ftp"
            psInfo.Arguments = "-s:" & FTPText
            psInfo.WorkingDirectory = WorkDir
            psInfo.UseShellExecute = False
            psInfo.CreateNoWindow = True '2010/05/18追加

            'FTP実行
            Dim p As Process = Process.Start(psInfo)
            p.WaitForExit()

            If Not p.ExitCode = 0 Then
                ReportMessage = "ＦＴＰ処理失敗：リターンコード" & p.ExitCode
                Return False
            End If

        Catch ex As Exception
            ReportMessage = "ＦＴＰ処理失敗：" & ex.Message
            Return False
        End Try

        Return True
    End Function

End Class
'ファイルソートクラス
Public Class ClsFileSort
    Implements IComparer

    Private mKey(,) As Integer

    Sub New(ByVal keyitem(,) As Integer)
        mKey = keyitem
    End Sub

    '-------------------------------------------------------------------
    '機能   :   比較用関数
    '引数   :   x 比較対象の第 1 オブジェクト
    '           y 比較対象の第 2 オブジェクト
    '戻り値 :   Integer 0 より小さい値 x が y より小さい。 
    '                   0 x と y は等しい。 
    '                   0 より大きい値 x が y より大きい。 
    '備考   :   
    '-------------------------------------------------------------------
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
        Implements IComparer.Compare

        Dim nRet As Integer = 0
        'ワーク配列
        Dim DataX As String
        Dim DataY As String

        '文字に変換
        For i As Integer = 0 To CType((mKey.Length / 2), Integer) - 1
            DataX = CType(x, String).Substring(mKey(i, 0), mKey(i, 1))
            DataY = CType(y, String).Substring(mKey(i, 0), mKey(i, 1))

            nRet = New CaseInsensitiveComparer().Compare(DataX, DataY)
            If nRet <> 0 Then
                Exit For
            End If
        Next i

        Return nRet
    End Function
End Class

'2017/12/11 saitou 広島信金(RSV2標準) ADD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START

''' <summary>
''' パワーソートクラス
''' </summary>
''' <remarks>2017/12/11 saitou 広島信金(RSV2標準) added for サーバー印刷対応</remarks>
Public Class clsPowerSORT
    Protected iDisposalNumber As Integer
    Protected iFieldDefinition As Integer
    Protected mFieldDelimiter As String
    Protected mKeyCmdStr As String
    Protected mInputFilesSkiprec As String
    Protected mSelCmdStr As String
    Protected mInputFiles As String
    Protected iInputFileType As Integer
    Protected mOutputFile As String
    Protected iOutputFileType As Integer
    Protected iMaxRecordLength As Integer
    Protected iErrorCode As Integer
    Protected mErrorDetail As String

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        iDisposalNumber = 0
        iFieldDefinition = 0
        mFieldDelimiter = ""
        mKeyCmdStr = ""
        mInputFilesSkiprec = ""
        mSelCmdStr = ""
        mInputFiles = ""
        iInputFileType = 0
        mOutputFile = ""
        iOutputFileType = 0
        iMaxRecordLength = 0
        iErrorCode = 0
        mErrorDetail = ""
    End Sub

#Region "プロパティ"

    Public Property DisposalNumber As Integer
        Get
            Return iDisposalNumber
        End Get
        Set(value As Integer)
            iDisposalNumber = value
        End Set
    End Property

    Public Property FieldDefinition As Integer
        Get
            Return iFieldDefinition
        End Get
        Set(value As Integer)
            iFieldDefinition = value
        End Set
    End Property

    Public Property FieldDelimiter As String
        Get
            Return mFieldDelimiter
        End Get
        Set(value As String)
            mFieldDelimiter = value
        End Set
    End Property

    Public Property KeyCmdStr As String
        Get
            Return mKeyCmdStr
        End Get
        Set(value As String)
            mKeyCmdStr = value
        End Set
    End Property

    Public Property InputFilesSkiprec As String
        Get
            Return mInputFilesSkiprec
        End Get
        Set(value As String)
            mInputFilesSkiprec = value
        End Set
    End Property

    Public Property SelCmdStr As String
        Get
            Return mSelCmdStr
        End Get
        Set(value As String)
            mSelCmdStr = value
        End Set
    End Property

    Public Property InputFiles As String
        Get
            Return mInputFiles
        End Get
        Set(value As String)
            mInputFiles = value
        End Set
    End Property

    Public Property InputFileType As Integer
        Get
            Return iInputFileType
        End Get
        Set(value As Integer)
            iInputFileType = value
        End Set
    End Property

    Public Property OutputFile As String
        Get
            Return mOutputFile
        End Get
        Set(value As String)
            mOutputFile = value
        End Set
    End Property

    Public Property OutputFileType As Integer
        Get
            Return iOutputFileType
        End Get
        Set(value As Integer)
            iOutputFileType = value
        End Set
    End Property

    Public Property MaxRecordLength As Integer
        Get
            Return iMaxRecordLength
        End Get
        Set(value As Integer)
            iMaxRecordLength = value
        End Set
    End Property

    Public Property ErrorCode As Integer
        Get
            Return iErrorCode
        End Get
        Set(value As Integer)
            iErrorCode = value
        End Set
    End Property

    Public Property ErrorDetail As String
        Get
            Return mErrorDetail
        End Get
        Set(value As String)
            mErrorDetail = value
        End Set
    End Property

#End Region

#Region "メソッド"

    Public Sub Action()
        Try
            Dim ProcSort As Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(GetRSKJIni("RSV2_V1.0.0", "POWERSORT_PATH"), "bsort.exe")
            If File.Exists(ProcInfo.FileName) = False Then
                ' パワーソートがないので、ソートせずに処理を続行する
                Return
            End If

            '入出力ファイル名チェック
            If InputFiles.Trim = "" OrElse OutputFile.Trim = "" Then
                Return
            End If

            '処理区分
            Dim SyoriKbn As String = String.Empty
            Select Case DisposalNumber
                Case 0 : SyoriKbn = "-s"    'ソート
                Case 1 : SyoriKbn = "-m"    'マージ
                Case 2 : SyoriKbn = "-c"    'コピー
                Case Else : Return
            End Select

            'テキストファイルオプション
            Dim TxtOpt As String = String.Empty
            Select Case FieldDefinition
                Case 0 : TxtOpt = "-Tflt"   '浮動
                Case 1 : TxtOpt = "-Tfix"   '固定
                Case Else : Return
            End Select

            'フィールド分離文字列
            Dim FieldSep As String = String.Empty
            If FieldDefinition = 0 AndAlso FieldDelimiter.Trim <> "" Then
                FieldSep = "-t " & FieldDelimiter
            End If

            '最大レコードサイズ
            Dim RecSize As String = String.Empty
            If MaxRecordLength <> 0 Then
                RecSize = "-z" & MaxRecordLength.ToString
            End If

            'スキップレコード番号
            Dim SkipRec As String = String.Empty
            If InputFilesSkiprec.Trim <> "" Then
                SkipRec = "-R " & InputFilesSkiprec
            End If

            '選択フィールドオプション
            Dim SelCmd As String = String.Empty
            If SelCmdStr.Trim <> "" Then
                SelCmd = "-p " & SelCmdStr
            End If

            ProcInfo.Arguments = String.Format("{0} {1} -""{2}"" {3} {4} {5} {6} -o {7} {8}", SyoriKbn, RecSize, KeyCmdStr, TxtOpt, FieldSep, SkipRec, SelCmd, OutputFile, InputFiles)

            ProcInfo.WorkingDirectory = Path.GetDirectoryName(ProcInfo.FileName)
            ProcInfo.UseShellExecute = False
            ProcInfo.RedirectStandardOutput = True
            ProcSort = Process.Start(ProcInfo)
            ProcSort.WaitForExit()

            'エラーコード設定
            ErrorCode = ProcSort.ExitCode

            If ProcSort.ExitCode <> 0 Then
                ErrorDetail = ProcSort.StandardOutput.ReadToEnd()
                Return
            End If

        Catch ex As Exception
            Return
        End Try
    End Sub

#End Region

End Class
'2017/12/11 saitou 広島信金(RSV2標準) ADD ------------------------------------------------------------ END
