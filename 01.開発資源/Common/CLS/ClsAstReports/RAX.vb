Imports System.IO

' RepoAgent制御クラス（ActiveXバージョン）
Public Class RAX
    'Inherits CR
    Protected mReportPath As String                 ' レポートパス
    Protected mReportName As String                 ' レポート名
    Protected mRecordSelectionFormula As String     ' レポート選択式
    Protected mErrorMessage As String               ' エラー情報
    Protected mFormulaFieldDefinition As String     ' フィールド値

    Protected mCsvPath As String                    ' ＣＳＶ出力パス
    Protected mCsvName As String                    ' ＣＳＶ名


    Protected mLogPath As String                    ' ログ出力パス
    Protected mLogName As String                    ' ログファイル名

    '2018/02/15 saitou 広島信金(RSV2標準) ADD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START
    'ここでインスタンスの生成は行わない。
    Private raReport As raocxlib.RepoAgent
    'Private raReport As New raocxlib.RepoAgent
    '2018/02/15 saitou 広島信金(RSV2標準) ADD ------------------------------------------------------------ END

    Public Copies As Integer = 1                    '印刷部数
    Public DataCode As Integer = 0                  '文字コード

    '2017/12/11 saitou 広島信金(RSV2標準) ADD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START
    Private raReport64 As RA64
    Private FLG64 As String = "0"
    '2017/12/11 saitou 広島信金(RSV2標準) ADD ------------------------------------------------------------ END

    Public Sub New()
        'レポートパスデフォルト指定
        mReportPath = Path.GetDirectoryName(ReportName)
        If mReportPath = "" Then
            CASTCommon.GetFSKJIni("COMMON", "LST", mReportPath)
            mReportPath = Path.GetDirectoryName(mReportPath)
        End If

        mReportName = ""
        mRecordSelectionFormula = ""
        mErrorMessage = ""
        mCsvPath = ""
        mCsvName = ""
    End Sub

    ' reportname : レポート名 フルパス
    Public Sub New(ByVal reportname As String)

        MyClass.New()

        mReportName = Path.GetFileName(reportname)

        Call SetupReport()

    End Sub

    ' reportpath : パス名
    ' reportname : レポート名
    Public Sub New(ByVal reportpath As String, ByVal reportname As String)

        MyClass.New()

        mReportPath = reportpath
        mReportName = reportname

        Call SetupReport()

    End Sub

    ' レポートパス
    Public Property ReportPath() As String
        Get
            Return mReportPath
        End Get
        Set(ByVal Value As String)
            mReportPath = Value
        End Set
    End Property

    ' レポート名
    Public Overridable Property ReportName() As String
        Get
            Return mReportName
        End Get
        Set(ByVal Value As String)
            If mReportPath = "" Then
                mReportPath = Path.GetDirectoryName(Value)
            End If
            mReportName = Path.GetFileName(Value)

            If File.Exists(Path.Combine(mReportPath, mReportName)) = True Then
                Call SetupReport()
            End If
        End Set
    End Property

    ' CSVパス
    Public Property CsvPath() As String
        Get
            Return mCsvPath
        End Get
        Set(ByVal Value As String)
            mCsvPath = Value
        End Set
    End Property

    ' CSV名
    Public Property CsvName() As String
        Get
            Return mCsvName
        End Get
        Set(ByVal Value As String)
            If mCsvPath = "" Then
                mCsvPath = Path.GetDirectoryName(Value)
            End If
            mCsvName = Path.GetFileName(Value)
        End Set
    End Property

    '
    ' プロパティ： メッセージ
    '
    Public Property Message() As String
        Get
            Return mErrorMessage
        End Get
        Set(ByVal Value As String)
            mErrorMessage = Value
        End Set
    End Property


    ' LOGパス
    Public Property LogPath() As String
        Get
            Return mLogPath
        End Get
        Set(ByVal Value As String)
            mLogPath = Value
        End Set
    End Property

    ' LOG名
    Public Property LogName() As String
        Get
            Return mLogName
        End Get
        Set(ByVal Value As String)
            If mLogPath = "" Then
                mLogPath = Path.GetDirectoryName(Value)
            End If
            mLogName = Path.GetFileName(Value)
        End Set
    End Property

    '
    ' 機能　 ： 印刷
    '
    ' 引数　 ： ARG1 - 
    '
    ' 戻り値 ： True-成功，False-失敗
    '
    ' 備考　 ： 
    '
    Public Overloads Function PrintOut() As Boolean
        Dim ret As Boolean
        Try
            If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
                mErrorMessage = "ＣＳＶファイルが見つかりません。" & Path.Combine(mCsvPath, mCsvName)
                Return False
            End If

            ' レポート設定
            Call SetupReport()

            ' 印刷
            '2017/12/11 saitou 広島信金(RSV2標準) UPD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START
            If FLG64 = "1" Then
                raReport64.ReportName = Path.GetFileNameWithoutExtension(raReport64.RpdFileName) & CASTCommon.Calendar.Now.ToString("HHmmss") & "_" & CASTCommon.Calendar.Now.ToString("%fffffff")
                ret = raReport64.ReportPrint()
                If ret = False Then
                    mErrorMessage = raReport64.ErrorDetail
                End If
            Else
                raReport.ReportName = Path.GetFileNameWithoutExtension(raReport.RpdFileName) & CASTCommon.Calendar.Now.ToString("HHmmss") & "_" & CASTCommon.Calendar.Now.ToString("%fffffff")
                ret = raReport.ReportPrint()
                If ret = False Then
                    mErrorMessage = raReport.ErrorDetail
                End If
            End If
            'raReport.ReportName = Path.GetFileNameWithoutExtension(raReport.RpdFileName) & CASTCommon.Calendar.Now.ToString("HHmmss") & "_" & CASTCommon.Calendar.Now.ToString("%fffffff")
            'ret = raReport.ReportPrint()
            'If ret = False Then
            '    mErrorMessage = raReport.ErrorDetail
            'End If
            '2017/12/11 saitou 広島信金(RSV2標準) UPD ------------------------------------------------------------ END
        Catch ex As Exception
            mErrorMessage = ex.Message
            Return False
        End Try
        Return ret
    End Function

    '
    ' 機能　 ： 印刷
    '
    ' 引数　 ： ARG1 - プリンタ名
    '
    ' 戻り値 ： True-成功，False-失敗
    '
    ' 備考　 ： 
    '
    Public Overloads Function PrintOut(ByVal printername As String) As Boolean
        Dim ret As Boolean
        Try
            If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
                mErrorMessage = "ＣＳＶファイルが見つかりません。" & Path.Combine(mCsvPath, mCsvName)
                Return False
            End If

            ' レポート設定
            Call SetupReport()

            ' 印刷
            '2017/12/11 saitou 広島信金(RSV2標準) UPD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START
            If FLG64 = "1" Then
                raReport64.PrtName = printername

                raReport64.ReportName = Path.GetFileNameWithoutExtension(raReport64.RpdFileName) & CASTCommon.Calendar.Now.ToString("HHmmss") & "_" & CASTCommon.Calendar.Now.ToString("%fffffff")

                ret = raReport64.ReportPrint()
                If ret = False Then
                    mErrorMessage = raReport64.ErrorDetail
                End If
            Else
                raReport.PrtName = printername

                raReport.ReportName = Path.GetFileNameWithoutExtension(raReport.RpdFileName) & CASTCommon.Calendar.Now.ToString("HHmmss") & "_" & CASTCommon.Calendar.Now.ToString("%fffffff")

                ret = raReport.ReportPrint()
                If ret = False Then
                    mErrorMessage = raReport.ErrorDetail
                End If
            End If
            'raReport.PrtName = printername

            'raReport.ReportName = Path.GetFileNameWithoutExtension(raReport.RpdFileName) & CASTCommon.Calendar.Now.ToString("HHmmss") & "_" & CASTCommon.Calendar.Now.ToString("%fffffff")

            'ret = raReport.ReportPrint()
            'If ret = False Then
            '    mErrorMessage = raReport.ErrorDetail
            'End If
            '2017/12/11 saitou 広島信金(RSV2標準) UPD ------------------------------------------------------------ END
        Catch ex As Exception
            mErrorMessage = ex.Message
            ret = False
        End Try
        Return ret
    End Function

    ' 2016/06/08 タスク）綾部 ADD 【RD】UI_B-14-99(RSV2対応) -------------------- START
    '
    ' 機能　 ： 印刷
    '
    ' 引数　 ： ARG1 - プリンタ名
    '
    ' 戻り値 ： True-成功，False-失敗
    '
    ' 備考　 ： 
    '
    Public Overloads Function PrintOut(ByVal printername As String, ByVal ReportName As String) As Boolean
        Dim ret As Boolean
        Try
            If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
                mErrorMessage = "ＣＳＶファイルが見つかりません。" & Path.Combine(mCsvPath, mCsvName)
                Return False
            End If

            ' レポート設定
            Call SetupReport()

            ' 印刷
            '2017/12/11 saitou 広島信金(RSV2標準) UPD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START
            If FLG64 = "1" Then
                If printername <> "" Then
                    raReport64.PrtName = printername
                End If

                raReport64.ReportName = ReportName

                ret = raReport64.ReportPrint()
                If ret = False Then
                    mErrorMessage = raReport64.ErrorDetail
                End If
            Else
                If printername <> "" Then
                    raReport.PrtName = printername
                End If

                raReport.ReportName = ReportName

                ret = raReport.ReportPrint()
                If ret = False Then
                    mErrorMessage = raReport.ErrorDetail
                End If
            End If
            'If printername <> "" Then
            '    raReport.PrtName = printername
            'End If

            'raReport.ReportName = ReportName

            'ret = raReport.ReportPrint()
            'If ret = False Then
            '    mErrorMessage = raReport.ErrorDetail
            'End If
            '2017/12/11 saitou 広島信金(RSV2標準) UPD ------------------------------------------------------------ END
        Catch ex As Exception
            mErrorMessage = ex.Message
            ret = False
        End Try
        Return ret
    End Function
    ' 2016/06/08 タスク）綾部 ADD 【RD】UI_B-14-99(RSV2対応) -------------------- END

    '
    ' 機能　 ： 印刷
    '
    ' 引数　 ： ARG1 - プリンタ名
    '
    ' 戻り値 ： True-成功，False-失敗
    '
    ' 備考　 ： 2012/06/30 標準版　WEB伝送対応
    '
    Public Overloads Function PrintOut(ByVal printername As String, ByVal USER_ID As String, ByVal ITAKU_CODE As String) As Boolean
        Dim ret As Boolean
        Dim SrcPath As String = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_TMP")
        Dim DstPath As String = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_SED")
        '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
        Dim FNAME_MODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "WEBDEN_FNAME_MODE")
        '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
        Try
            If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
                mErrorMessage = "ＣＳＶファイルが見つかりません。" & Path.Combine(mCsvPath, mCsvName)
                Return False
            End If

            ' レポート設定
            Call SetupReport()

            ' 印刷
            '2017/12/11 saitou 広島信金(RSV2標準) UPD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START
            If FLG64 = "1" Then
                raReport64.PrtName = printername

                '2017/04/26 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ START
                Dim RPD_NAME As String = Path.GetFileNameWithoutExtension(raReport64.RpdFileName)

                'WEB伝送のファイル名を置換する（帳票定義体部分）
                If FNAME_MODE = "1" Then
                    Dim RET_RPD_NAME As String = CASTCommon.GetIni("WEBDEN_RPDNAME.INI", RPD_NAME, "RPDNAME")
                    Select Case RET_RPD_NAME
                        Case "err", ""
                            '何もしない（標準仕様）
                        Case Else
                            RPD_NAME = RET_RPD_NAME
                    End Select
                End If
                raReport64.ReportName = USER_ID & "_" & RPD_NAME & "_" & ITAKU_CODE
                'raReport64.ReportName = USER_ID & "_" & Path.GetFileNameWithoutExtension(raReport64.RpdFileName) & "_" & ITAKU_CODE
                '2017/04/26 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ END

                ret = raReport64.ReportPrint()

                '大量印刷対応　待機時間を指定
                Dim start As Object
                start = CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss"))
                Do While CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss")) > start + 60
                    Application.DoEvents()
                Loop

                If ret = False Then
                    mErrorMessage = raReport64.ErrorDetail
                End If

                'PDFファイルが作成完了するまで待機する
                For cnt As Integer = 1 To 300 Step 1
                    Try
                        If File.Exists(SrcPath & raReport64.ReportName & ".pdf") = True Then
                            File.Copy(SrcPath & raReport64.ReportName & ".pdf", DstPath & raReport64.ReportName & ".pdf", True)
                            File.Delete(SrcPath & raReport64.ReportName & ".pdf")
                            Exit For
                        End If
                    Catch ex As Exception
                        'System.Threading.Thread.Sleep(1000) '2010/05/18 コメントアウト
                    End Try
                    '2010/05/18 例外にならない場合があるのでループに待機を入れる
                    System.Threading.Thread.Sleep(1000)
                Next

            Else
                raReport.PrtName = printername

                '2017/04/26 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ START
                Dim RPD_NAME As String = Path.GetFileNameWithoutExtension(raReport.RpdFileName)

                'WEB伝送のファイル名を置換する（帳票定義体部分）
                If FNAME_MODE = "1" Then
                    Dim RET_RPD_NAME As String = CASTCommon.GetIni("WEBDEN_RPDNAME.INI", RPD_NAME, "RPDNAME")
                    Select Case RET_RPD_NAME
                        Case "err", ""
                            '何もしない（標準仕様）
                        Case Else
                            RPD_NAME = RET_RPD_NAME
                    End Select
                End If
                raReport.ReportName = USER_ID & "_" & RPD_NAME & "_" & ITAKU_CODE
                'raReport.ReportName = USER_ID & "_" & Path.GetFileNameWithoutExtension(raReport.RpdFileName) & "_" & ITAKU_CODE
                '2017/04/26 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ END

                ret = raReport.ReportPrint()

                '大量印刷対応　待機時間を指定
                Dim start As Object
                start = CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss"))
                Do While CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss")) > start + 60
                    Application.DoEvents()
                Loop

                If ret = False Then
                    mErrorMessage = raReport.ErrorDetail
                End If

                'PDFファイルが作成完了するまで待機する
                For cnt As Integer = 1 To 300 Step 1
                    Try
                        If File.Exists(SrcPath & raReport.ReportName & ".pdf") = True Then
                            File.Copy(SrcPath & raReport.ReportName & ".pdf", DstPath & raReport.ReportName & ".pdf", True)
                            File.Delete(SrcPath & raReport.ReportName & ".pdf")
                            Exit For
                        End If
                    Catch ex As Exception
                        'System.Threading.Thread.Sleep(1000) '2010/05/18 コメントアウト
                    End Try
                    '2010/05/18 例外にならない場合があるのでループに待機を入れる
                    System.Threading.Thread.Sleep(1000)
                Next
            End If
            'raReport.PrtName = printername

            ''2017/04/26 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ START
            'Dim RPD_NAME As String = Path.GetFileNameWithoutExtension(raReport.RpdFileName)

            ''WEB伝送のファイル名を置換する（帳票定義体部分）
            'If FNAME_MODE = "1" Then
            '    Dim RET_RPD_NAME As String = CASTCommon.GetIni("WEBDEN_RPDNAME.INI", RPD_NAME, "RPDNAME")
            '    Select Case RET_RPD_NAME
            '        Case "err", ""
            '            '何もしない（標準仕様）
            '        Case Else
            '            RPD_NAME = RET_RPD_NAME
            '    End Select
            'End If
            'raReport.ReportName = USER_ID & "_" & RPD_NAME & "_" & ITAKU_CODE
            ''raReport.ReportName = USER_ID & "_" & Path.GetFileNameWithoutExtension(raReport.RpdFileName) & "_" & ITAKU_CODE
            ''2017/04/26 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ END

            'ret = raReport.ReportPrint()

            ''大量印刷対応　待機時間を指定
            'Dim start As Object
            'start = CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss"))
            'Do While CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss")) > start + 60
            '    Application.DoEvents()
            'Loop

            'If ret = False Then
            '    mErrorMessage = raReport.ErrorDetail
            'End If

            ''PDFファイルが作成完了するまで待機する
            'For cnt As Integer = 1 To 300 Step 1
            '    Try
            '        If File.Exists(SrcPath & raReport.ReportName & ".pdf") = True Then
            '            File.Copy(SrcPath & raReport.ReportName & ".pdf", DstPath & raReport.ReportName & ".pdf", True)
            '            File.Delete(SrcPath & raReport.ReportName & ".pdf")
            '            Exit For
            '        End If
            '    Catch ex As Exception
            '        'System.Threading.Thread.Sleep(1000) '2010/05/18 コメントアウト
            '    End Try
            '    '2010/05/18 例外にならない場合があるのでループに待機を入れる
            '    System.Threading.Thread.Sleep(1000)
            'Next
            '2017/12/11 saitou 広島信金(RSV2標準) UPD ------------------------------------------------------------ END

        Catch ex As Exception
            mErrorMessage = ex.Message
            ret = False
        End Try
        Return ret
    End Function

    '*** Str Add 2015/12/04 SO)沖野 for 拡張印刷対応 ***

    ' 機能   ： 印刷（拡張印刷対応）
    ' 引数   ： printername  プリンタ名
    '           prtDspName  印刷名
    ' 戻り値 ： True-成功，False-失敗
    '
    Public Function ExtendPrintOut(ByVal printername As String, ByVal prtDspName As String) As Boolean

        Dim ret As Boolean = False

        Dim LOG As CASTCommon.BatchLOG = New CASTCommon.BatchLOG("CAstReports", "RAX")

        ' 処理開始ログ出力
        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("RAX.ExtendPrintOut")

        Try
            If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
                mErrorMessage = "ＣＳＶファイルが見つかりません。" & Path.Combine(mCsvPath, mCsvName)
                Return ret
            End If

            ' レポート設定
            Call SetupReport()

            '2017/12/11 saitou 広島信金(RSV2標準) UPD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START
            If FLG64 = "1" Then
                If Not printername = "" Then
                    ' 通常使うプリンタ以外の場合、プリンタ名を設定
                    raReport64.PrtName = printername
                End If

                ' 印刷名を設定
                raReport64.ReportName = prtDspName

                ' 印刷
                ret = raReport64.ReportPrint()

                If ret = False Then
                    mErrorMessage = raReport64.ErrorDetail
                End If
            Else
                If Not printername = "" Then
                    ' 通常使うプリンタ以外の場合、プリンタ名を設定
                    raReport.PrtName = printername
                End If

                ' 印刷名を設定
                raReport.ReportName = prtDspName

                ' 印刷
                ret = raReport.ReportPrint()

                If ret = False Then
                    mErrorMessage = raReport.ErrorDetail
                End If
            End If
            'If Not printername = "" Then
            '    ' 通常使うプリンタ以外の場合、プリンタ名を設定
            '    raReport.PrtName = printername
            'End If

            '' 印刷名を設定
            'raReport.ReportName = prtDspName

            '' 印刷
            'ret = raReport.ReportPrint()

            'If ret = False Then
            '    mErrorMessage = raReport.ErrorDetail
            'End If
            '2017/12/11 saitou 広島信金(RSV2標準) UPD ------------------------------------------------------------ END

            Return ret

        Catch ex As Exception
            mErrorMessage = ex.ToString
            Return ret

        Finally
            ' 処理終了ログ出力
            LOG.Write_Exit3(sw, "RAX.ExtendPrintOut", "復帰値=" & ret)
        End Try

    End Function

    '*** End Add 2015/12/04 SO)沖野 for 拡張印刷対応 ***

    '
    ' 機能　 ： プレビュー
    '
    ' 引数　 ： ARG1 - 
    '
    ' 戻り値 ： True-成功，False-失敗
    '
    ' 備考　 ： 
    '
    Public Function Preview() As Boolean
        Dim ret As Boolean
        Try
            If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
                mErrorMessage = "ＣＳＶファイルが見つかりません。" & Path.Combine(mCsvPath, mCsvName)
                Return False
            End If

            ' レポート設定
            Call SetupReport()

            ' プレビュー
            '2017/12/11 saitou 広島信金(RSV2標準) UPD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START
            If FLG64 = "1" Then
                ret = raReport64.ReportView()
                If ret = False Then
                    mErrorMessage = raReport64.ErrorDetail
                End If
            Else
                ret = raReport.ReportView()
                If ret = False Then
                    mErrorMessage = raReport.ErrorDetail
                End If
            End If
            'ret = raReport.ReportView()
            'If ret = False Then
            '    mErrorMessage = raReport.ErrorDetail
            'End If
            '2017/12/11 saitou 広島信金(RSV2標準) UPD ------------------------------------------------------------ END
        Catch ex As Exception
            mErrorMessage = ex.Message
            ret = False
        End Try

        Return ret
    End Function

    '
    ' 機能　 ： レポート設定
    '
    ' 引数　 ： ARG1 - 
    '
    ' 戻り値 ： 
    '
    ' 備考　 ： 
    '
    Protected Overloads Sub SetupReport()
        '2017/12/11 saitou 広島信金(RSV2標準) UPD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START
        '印刷を実行している端末が64ビットかどうか判断
        If System.Environment.Is64BitProcess Then
            FLG64 = "1"
        Else
            FLG64 = "0"
        End If

        If FLG64 = "1" Then
            ' レポート作成
            raReport64 = New RA64

            ' レポートパス指定
            raReport64.DataFileInputPath = mReportPath & ";"
            raReport64.CustomDataFileInputPath = mReportPath & ";"
            ' レポート名指定
            raReport64.RpdFileName = Path.Combine(mReportPath, mReportName)
            ' インプットＣＳＶファイル指定
            raReport64.InputDataTextName = Path.Combine(mCsvPath, mCsvName)

            ' 印刷部数
            raReport64.Copies = Copies
            ' 文字コード
            raReport64.DataCode = DataCode

            If Not mLogPath Is Nothing AndAlso Not mLogName Is Nothing Then
                If mLogName = mReportName Then
                    raReport64.LogFileName = Path.Combine(mLogPath, mLogName & ".LOG")
                Else
                    raReport64.LogFileName = Path.Combine(mLogPath, mLogName)
                End If
                raReport64.LogSize = 1024 * 1024
            End If
        Else
            ' レポート作成
            raReport = New raocxlib.RepoAgent

            ' レポートパス指定
            raReport.DataFileInputPath = mReportPath & ";"
            raReport.CustomDataFileInputPath = mReportPath & ";"
            ' レポート名指定
            raReport.RpdFileName = Path.Combine(mReportPath, mReportName)
            ' インプットＣＳＶファイル指定
            raReport.InputDataTextName = Path.Combine(mCsvPath, mCsvName)

            ' 印刷部数
            raReport.Copies = Copies
            ' 文字コード
            raReport.DataCode = DataCode

            If Not mLogPath Is Nothing AndAlso Not mLogName Is Nothing Then
                If mLogName = mReportName Then
                    raReport.LogFileName = Path.Combine(mLogPath, mLogName & ".LOG")
                Else
                    raReport.LogFileName = Path.Combine(mLogPath, mLogName)
                End If
                raReport.LogSize = 1024 * 1024
            End If
        End If
        '' レポート作成
        'raReport = New raocxlib.RepoAgent

        '' レポートパス指定
        'raReport.DataFileInputPath = mReportPath & ";"
        'raReport.CustomDataFileInputPath = mReportPath & ";"
        '' レポート名指定
        'raReport.RpdFileName = Path.Combine(mReportPath, mReportName)
        '' インプットＣＳＶファイル指定
        'raReport.InputDataTextName = Path.Combine(mCsvPath, mCsvName)

        '' 印刷部数
        'raReport.Copies = Copies
        '' 文字コード
        'raReport.DataCode = DataCode

        'If Not mLogPath Is Nothing AndAlso Not mLogName Is Nothing Then
        '    If mLogName = mReportName Then
        '        raReport.LogFileName = Path.Combine(mLogPath, mLogName & ".LOG")
        '    Else
        '        raReport.LogFileName = Path.Combine(mLogPath, mLogName)
        '    End If
        '    raReport.LogSize = 1024 * 1024
        'End If
        '2017/12/11 saitou 広島信金(RSV2標準) UPD ------------------------------------------------------------ END
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class

'2017/12/11 saitou 広島信金(RSV2標準) ADD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START

''' <summary>
''' RepoAgent 64ビット環境印刷クラス
''' </summary>
''' <remarks>2017/12/11 saitou 広島信金(RSV2標準) added for サーバー印刷対応</remarks>
Public Class RA64
    Protected mPrtName As String                    'プリンタ名
    Protected mReportName As String                 '文書名
    Protected mRpdFileName As String                'レポート定義ファイル名
    Protected mDataFileInputPath As String          'データソースファイル検索パス
    Protected mInputDataTextName As String          'データソースファイル名
    Protected mCustomDataFileInputPath As String    'カスタムデータソースファイル検索パス
    Protected mCopies As Integer                    'コピー部数
    Protected mErrorDetail As String                'エラー内容文字列
    Protected mDataCode As String                   '文字コード体系
    Protected mLogFileName As String                'ログ採取ファイル名
    Protected mLogSize As String                    '最大ログ採取量
    '2018/03/07 saitou 広島信金(RSV2標準) ADD サーバー印刷対応 -------------------- START
    Private htErrorMessage As Hashtable             'エラーメッセージリスト
    '2018/03/07 saitou 広島信金(RSV2標準) ADD ------------------------------------- END

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        mPrtName = ""
        mReportName = ""
        mRpdFileName = ""
        mErrorDetail = ""
        mDataFileInputPath = ""
        mInputDataTextName = ""
        mCustomDataFileInputPath = ""
        mCopies = 1
        mDataCode = ""

        '2018/03/07 saitou 広島信金(RSV2標準) ADD サーバー印刷対応 -------------------- START
        'RepoAgentのエラーメッセージリストの作成
        htErrorMessage = New Hashtable
        Dim sr As StreamReader = Nothing

        Try
            Dim ErrTxt As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "印刷エラーコード.TXT")
            If File.Exists(ErrTxt) Then
                sr = New StreamReader(ErrTxt, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
                While sr.Peek > -1
                    Dim s() As String = sr.ReadLine.Split("="c)
                    If s.Length >= 2 Then
                        If htErrorMessage.ContainsKey(s(0)) = False Then
                            Dim ErrStr As String = String.Empty
                            'エラーメッセージ内のイコールが区切り文字として判断され、
                            '文字列として表示されないので、イコールを別途追加する
                            For i As Integer = 1 To s.Length - 1
                                ErrStr &= s(i)
                                If i <> s.Length - 1 Then
                                    ErrStr &= "="
                                End If
                            Next
                            htErrorMessage.Add(s(0), s(1))
                        End If
                    End If
                End While
                sr.Close()
                sr = Nothing
            End If
        Catch ex As Exception
            Throw
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
        End Try
        '2018/03/07 saitou 広島信金(RSV2標準) ADD ------------------------------------- END
    End Sub

#Region "プロパティ"
    ''' <summary>
    ''' プリンタ名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PrtName() As String
        Get
            Return mPrtName
        End Get
        Set(value As String)
            mPrtName = value
        End Set
    End Property

    ''' <summary>
    ''' 文書名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReportName() As String
        Get
            Return mReportName
        End Get
        Set(value As String)
            mReportName = value
        End Set
    End Property

    ''' <summary>
    ''' レポート定義ファイル名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RpdFileName As String
        Get
            Return mRpdFileName
        End Get
        Set(value As String)
            mRpdFileName = value
        End Set
    End Property

    ''' <summary>
    ''' データソースファイル検索パス
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataFileInputPath() As String
        Get
            Return mDataFileInputPath
        End Get
        Set(ByVal Value As String)
            mDataFileInputPath = Value
        End Set
    End Property

    ''' <summary>
    ''' データソースファイル名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property InputDataTextName() As String
        Get
            Return mInputDataTextName
        End Get
        Set(ByVal Value As String)
            mInputDataTextName = Value
        End Set
    End Property

    ''' <summary>
    ''' カスタムデータソースファイル検索パス
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CustomDataFileInputPath() As String
        Get
            Return mCustomDataFileInputPath
        End Get
        Set(ByVal Value As String)
            mCustomDataFileInputPath = Value
        End Set
    End Property

    ''' <summary>
    ''' コピー部数
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Copies() As String
        Get
            Return mCopies.ToString
        End Get
        Set(ByVal Value As String)
            mCopies = CInt(Value)
        End Set
    End Property

    ''' <summary>
    ''' エラー内容文字列
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ErrorDetail() As String
        Get
            Return mErrorDetail
        End Get
        Set(ByVal Value As String)
            mErrorDetail = Value
        End Set
    End Property

    ''' <summary>
    ''' 文字コード系
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataCode() As String
        Get
            Return mDataCode
        End Get
        Set(ByVal Value As String)
            mDataCode = Value
        End Set
    End Property

    ''' <summary>
    ''' ログ採取ファイル名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LogFileName() As String
        Get
            Return mLogFileName
        End Get
        Set(ByVal Value As String)
            mLogFileName = Value
        End Set
    End Property

    ''' <summary>
    ''' 最大ログ採取量
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LogSize() As String
        Get
            Return mLogSize
        End Get
        Set(ByVal Value As String)
            mLogSize = Value
        End Set
    End Property
#End Region

#Region "メソッド"
    ''' <summary>
    ''' 帳票出力
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Public Function ReportPrint() As Boolean
        Dim CmdArg As String = ""

        CmdArg = RpdFileName & " -id " & InputDataTextName

        'ログファイル名
        If Not LogFileName Is Nothing Then
            CmdArg &= " -lf " & LogFileName
        End If

        '部数
        CmdArg &= " -cp " & Copies
        'プリンタ名
        CmdArg &= " -p """ & PrtName & """"
        'ドキュメント名
        CmdArg &= " -n " & ReportName

        Try
            Dim ProcReport As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(CASTCommon.GetRSKJIni("RSV2_V1.0.0", "REPOAGENT_PATH"), "raprint.exe")
            'ProcInfo.FileName = "raprint.exe"
            If CmdArg.Length > 0 Then
                ProcInfo.Arguments = CmdArg
            End If
            ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("COMMON", "EXE")
            ProcInfo.UseShellExecute = False
            ProcInfo.CreateNoWindow = True
            ProcInfo.RedirectStandardOutput = True
            ProcReport = Process.Start(ProcInfo)

            Do While True
                If ProcReport.HasExited = True Then
                    Exit Do
                Else
                    Dim NowProc As Process
                    Try
                        NowProc = Diagnostics.Process.GetProcessById(ProcReport.Id)
                        If NowProc Is Nothing Then
                            Exit Do
                        End If
                    Catch ex As Exception
                        Exit Do
                    End Try
                End If

                Application.DoEvents()

                Threading.Thread.Sleep(100)

            Loop

            If ProcReport.ExitCode <> 0 Then
                '2018/03/07 saitou 広島信金(RSV2標準) ADD サーバー印刷対応 -------------------- START
                'エラーメッセージを設定し、異常で返す。
                ErrorDetail = GetErrMessageReport(ProcReport.ExitCode.ToString)
                Return False
                '2018/03/07 saitou 広島信金(RSV2標準) ADD ------------------------------------- END
            End If

            Return True
        Catch ex As Exception
            '2018/03/07 saitou 広島信金(RSV2標準) ADD サーバー印刷対応 -------------------- START
            '例外発生時もエラーメッセージを設定する。
            ErrorDetail = ex.ToString
            '2018/03/07 saitou 広島信金(RSV2標準) ADD ------------------------------------- END
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 帳票プレビュー
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Public Function ReportView() As Boolean
        Dim CmdArg As String = ""

        CmdArg = RpdFileName & " -id " & InputDataTextName

        'ログファイル名
        If Not LogFileName Is Nothing Then
            CmdArg &= " -lf " & LogFileName
        End If

        Try
            Dim ProcReport As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(CASTCommon.GetRSKJIni("RSV2_V1.0.0", "REPOAGENT_PATH"), "raview.exe")
            'ProcInfo.FileName = "raview.exe"

            If CmdArg.Length > 0 Then
                ProcInfo.Arguments = CmdArg
            End If
            ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("COMMON", "EXE")
            ProcInfo.UseShellExecute = False
            ProcInfo.CreateNoWindow = True
            ProcInfo.RedirectStandardOutput = True
            ProcReport = Process.Start(ProcInfo)

            Do While True
                If ProcReport.HasExited = True Then
                    Exit Do
                Else
                    Dim NowProc As Process
                    Try
                        NowProc = Diagnostics.Process.GetProcessById(ProcReport.Id)
                        If NowProc Is Nothing Then
                            Exit Do
                        End If
                    Catch ex As Exception
                        Exit Do
                    End Try
                End If

                Application.DoEvents()

                Threading.Thread.Sleep(100)

            Loop

            If ProcReport.ExitCode <> 0 Then
                '2018/03/07 saitou 広島信金(RSV2標準) ADD サーバー印刷対応 -------------------- START
                'エラーメッセージを設定し、異常で返す。
                ErrorDetail = GetErrMessageReport(ProcReport.ExitCode.ToString)
                Return False
                '2018/03/07 saitou 広島信金(RSV2標準) ADD ------------------------------------- END
            End If

            Return True
        Catch ex As Exception
            '2018/03/07 saitou 広島信金(RSV2標準) ADD サーバー印刷対応 -------------------- START
            '例外発生時もエラーメッセージを設定する。
            ErrorDetail = ex.ToString
            '2018/03/07 saitou 広島信金(RSV2標準) ADD ------------------------------------- END
            Return False
        End Try
    End Function

    '2018/03/07 saitou 広島信金(RSV2標準) ADD サーバー印刷対応 -------------------- START
    Private Function GetErrMessageReport(ByVal errCode As String) As String
        If htErrorMessage.ContainsKey(errCode) = True Then
            Return htErrorMessage.Item(errCode).ToString
        Else
            Return "印刷エラーコードに設定されていないエラーが発生しました。エラーコード：" & errCode
        End If
    End Function
    '2018/03/07 saitou 広島信金(RSV2標準) ADD ------------------------------------- END
#End Region

End Class
'2017/12/11 saitou 広島信金(RSV2標準) ADD ------------------------------------------------------------ END
