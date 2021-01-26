Imports System.Data.OracleClient
Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Imports CAstExternal

''' <summary>
''' 運用試験_伝送受信自動　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class ClsDensoAuto

#Region "クラス変数"

    Public MainLOG As New CASTCommon.BatchLOG("KFO010", "運用試験_伝送受信自動")
    Private MainDB As CASTCommon.MyOracle

    Private GCom As MenteCommon.clsCommon

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

    Private TestAiteCenterCode As String = "00000000000009"
    Private TestTohoCenterCode As String = "00000000000010"
    Private TestZenginFileName As String = "502001910100"

#End Region

#Region "取引先情報構造体"
    Private Structure StructToriInfo
        Public FSyoriKbn As String
        Public TorisCode As String
        Public TorifCode As String
        Public ItakuCode As String
        Public DaihyoItakuCode As String
        Public BaitaiCode As String
        Public LabelKbn As String
        Public CodeKbn As String
        Public FmtKbn As String
        Public MultiKbn As String
        Public FuriDate As String
        Public FileCount As Integer

        Sub Init()
            FSyoriKbn = Nothing
            TorisCode = Nothing
            TorifCode = Nothing
            ItakuCode = Nothing
            DaihyoItakuCode = Nothing
            BaitaiCode = Nothing
            LabelKbn = Nothing
            CodeKbn = Nothing
            FmtKbn = Nothing
            MultiKbn = Nothing
            FuriDate = Nothing
        End Sub

        '取引先情報を設定
        Public WriteOnly Property Data() As CASTCommon.MyOracleReader
            Set(ToriMastReader As CASTCommon.MyOracleReader)

                FSyoriKbn = ToriMastReader.GetItem("FSYORI_KBN_T")
                TorisCode = ToriMastReader.GetItem("TORIS_CODE_T")
                TorifCode = ToriMastReader.GetItem("TORIF_CODE_T")
                ItakuCode = ToriMastReader.GetItem("ITAKU_CODE_T")
                DaihyoItakuCode = ToriMastReader.GetItem("ITAKU_KANRI_CODE_T")
                BaitaiCode = ToriMastReader.GetItem("BAITAI_CODE_T")
                LabelKbn = ToriMastReader.GetItem("LABEL_KBN_T")
                CodeKbn = ToriMastReader.GetItem("CODE_KBN_T")
                FmtKbn = ToriMastReader.GetItem("FMT_KBN_T")
                MultiKbn = ToriMastReader.GetItem("MULTI_KBN_T")
            End Set
        End Property
    End Structure
    Private ToriInfo As New StructToriInfo

    Private CodeKbnData As String = "0"
    Private CodeChange As String = ""

#End Region

#Region "パブリックメソッド"

    Public Function Main() As Integer

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "伝送ファイルコピー処理", "開始", "")
            GCom = New MenteCommon.clsCommon

            '-----------------------------------------------------------
            'テキストボックスの入力チェック
            '-----------------------------------------------------------
            Dim SystemDate As String = Now.ToString("yyyyMMdd")
            Dim JyushinDate As String = SystemDate
            Dim JyushinPath As String = "\\192.168.3.55\d$\FSKJ\DAT\DEN\"
            Dim OutputPath As String = "C:\Linkexpress\"

            Dim FileList As String() = Directory.GetFiles(JyushinPath)
            If FileList.Length = 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "伝送ファイルコピー処理", "成功", "受信日 [" & JyushinDate & "] の伝送ファイル０件")
                Return 0
            End If

            '----------------------------------------------------------
            ' ファイル移動開始
            '----------------------------------------------------------
            ToriInfo.FileCount = 0
            MainDB = New CASTCommon.MyOracle
            Dim FileFullPath As String = String.Empty
            For Each FileFullPath In FileList
                Dim FileName As String = Path.GetFileName(FileFullPath)
                Dim Extension As String = Path.GetExtension(FileName)
                Dim RetFileName As String = String.Empty

                Select Case Extension.ToUpper
                    Case ".DAT"
                        Dim PutIniValue As String = "1"
                        If Ex_GetIni("C:\RSKJ\IKOU\FSKJ\OT_" & JyushinDate & ".ini", JyushinDate, Path.GetFileNameWithoutExtension(FileName)).ToUpper <> "ERR" Then
                            MainLOG.Write("伝送連携受信実行", "成功", "受信INIチェック　既に実行済み：" & FileName)
                            GoTo NEXTDATA
                        End If

                        CodeKbnData = "0"
                        CodeChange = ""
                        Select Case FileName.Substring(0, 3).ToUpper
                            Case "DSO"
                                System.Threading.Thread.Sleep(3000)
                                RetFileName = "DSO" & FileName.Substring(3, 40) & Now.ToString("yyyyMMddHHmmssfff") & ".dat"
                            Case "DEN"
                                'ＦＡＸ振込分割後データの処理は行わない
                                GoTo NEXTDATA
                            Case Else
                                System.Threading.Thread.Sleep(3000)
                                RetFileName = "DSO" & _
                                              TestAiteCenterCode & _
                                              TestTohoCenterCode & _
                                              TestZenginFileName & _
                                              Now.ToString("yyyyMMddHHmmssfff") & ".dat"
                        End Select

                        If GetCopyName(JyushinDate, FileFullPath) = False Then
                            ToriInfo.ItakuCode = "9999999999"
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "伝送ファイルコピー処理", "失敗", "取引先マスタに該当なし コピー元ファイル:[" & FileFullPath & "]")
                        End If

                        Select Case ToriInfo.ItakuCode
                            Case "8888888888"
                                'すでに処理済みデータのため処理は行わない
                                '（処理日当日分複数回起動の前回以前の対象ファイル）
                            Case "9999999999"
                                'ＦＡＸ振込０件データのため処理は行わない
                            Case Else
                                Select Case CodeChange
                                    Case ""
                                        File.Copy(FileFullPath, Path.Combine(OutputPath, RetFileName))

                                        Dim Proc As New Process
                                        Dim ProcInfo As New ProcessStartInfo
                                        Select Case RetFileName.Substring(3, 36)
                                            Case "035462337700010001394999000050201121"
                                                ProcInfo.FileName = "C:\RSKJ\EXE\KFD011.EXE"
                                            Case Else
                                                ProcInfo.FileName = "C:\RSKJ\EXE\KFD001.EXE"
                                        End Select
                                        ProcInfo.Arguments = Path.Combine(OutputPath, RetFileName)
                                        If File.Exists(ProcInfo.FileName) = True Then

                                            ProcInfo.WorkingDirectory = ""

                                            MainLOG.Write("伝送連携受信実行", "開始", "Command :" & ProcInfo.FileName)
                                            MainLOG.Write("伝送連携受信実行", "　　", "Parameta:" & ProcInfo.Arguments)

                                            System.Threading.Thread.Sleep(1000)
                                            Proc = Process.Start(ProcInfo)
                                            Proc.WaitForExit()

                                            If Proc.ExitCode = 0 Then
                                                ' 連携成功
                                            Else
                                                ' 連携失敗
                                                MainLOG.Write("伝送連携受信コマンド実行", "失敗", "Return:" & Proc.ExitCode.ToString & " " & ProcInfo.FileName & " " & ProcInfo.Arguments)
                                                Proc.StandardOutput.ReadToEnd()
                                            End If
                                        Else
                                            MainLOG.Write("伝送連携受信コマンド実行", "失敗", "起動アプリケーションなし：" & ProcInfo.FileName)
                                        End If
                                        PutIniValue = RetFileName

                                    Case Else
                                        Dim FtranRet As Integer = 0
                                        Dim InFileName As String = Path.Combine(OutputPath, RetFileName & "BEF")
                                        Dim OutFileName As String = Path.Combine(OutputPath, RetFileName)
                                        File.Copy(FileFullPath, InFileName)

                                        System.Threading.Thread.Sleep(2000)
                                        FtranRet = FileCodeChange(InFileName, OutFileName, 120, CodeChange, "120.P")

                                        Dim Proc As New Process
                                        Dim ProcInfo As New ProcessStartInfo
                                        Select Case RetFileName.Substring(3, 36)
                                            Case "035462337700010001394999000050201121"
                                                ProcInfo.FileName = "C:\RSKJ\EXE\KFD011.EXE"
                                            Case Else
                                                ProcInfo.FileName = "C:\RSKJ\EXE\KFD001.EXE"
                                        End Select
                                        ProcInfo.Arguments = Path.Combine(OutputPath, RetFileName)

                                        If File.Exists(ProcInfo.FileName) = True Then

                                            ProcInfo.WorkingDirectory = ""

                                            MainLOG.Write("伝送連携受信実行", "開始", "Command :" & ProcInfo.FileName)
                                            MainLOG.Write("伝送連携受信実行", "　　", "Parameta:" & ProcInfo.Arguments)

                                            System.Threading.Thread.Sleep(1000)
                                            Proc = Process.Start(ProcInfo)
                                            Proc.WaitForExit()

                                            If Proc.ExitCode = 0 Then
                                                ' 連携成功
                                            Else
                                                ' 連携失敗
                                                MainLOG.Write("伝送連携受信コマンド実行", "失敗", "Return:" & Proc.ExitCode.ToString & " " & ProcInfo.FileName & " " & ProcInfo.Arguments)
                                                Proc.StandardOutput.ReadToEnd()
                                            End If
                                        Else
                                            MainLOG.Write("伝送連携受信コマンド実行", "失敗", "起動アプリケーションなし：" & ProcInfo.FileName)
                                        End If

                                        If File.Exists(Path.Combine(OutputPath, RetFileName & "BEF")) = True Then
                                            File.Delete(Path.Combine(OutputPath, RetFileName & "BEF"))
                                        End If
                                        PutIniValue = OutFileName

                                End Select
                        End Select

                        If PutIniValue <> "1" Then
                            Ex_PutIni("C:\RSKJ\IKOU\FSKJ\OT_" & JyushinDate & ".ini", JyushinDate, Path.GetFileNameWithoutExtension(FileName), PutIniValue)
                        End If
NEXTDATA:
                End Select
            Next

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "伝送ファイルコピー処理", "成功", "")
            Return 0

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "伝送ファイルコピー処理", "失敗", ex.ToString)
            Return 1
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "伝送ファイルコピー処理", "終了", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try

    End Function

#End Region

#Region "プライベートメソッド"

    Private Function GetCopyName(ByVal JyushinDate As String, ByVal FileName As String) As Boolean

        Dim fs As FileStream = Nothing
        Dim br As BinaryReader = Nothing

        'デフォルトS-JIS
        Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding(932)

        Dim Syubetu As String = String.Empty
        Dim ItakuCode As String = String.Empty

        Try
            fs = New FileStream(FileName, FileMode.Open)
            br = New BinaryReader(fs, System.Text.Encoding.GetEncoding(932))

            Dim rbyte As Byte() = br.ReadBytes(120)

            br.Close()
            br = Nothing
            fs.Close()
            fs = Nothing

            '先頭1バイト目が0～9を許可
            Select Case rbyte(0)
                Case 48 To 57 '30～39
                    'S-JIS
                    enc = System.Text.Encoding.GetEncoding(932)
                    CodeKbnData = "0"
                Case 240 To 249 'F0～F9
                    'IBM290
                    enc = System.Text.Encoding.GetEncoding(20290)
                    CodeKbnData = "4"
            End Select

            Syubetu = enc.GetString(rbyte, 1, 2)
            ItakuCode = enc.GetString(rbyte, 4, 10)

            Select Case ItakuCode
                Case "9999999999"
                    'ＦＡＸ振込０件データのため処理は行わない
                    ToriInfo.ItakuCode = ItakuCode
                Case Else
                    '取引先情報取得
                    ToriInfo.Init()
                    ToriInfo.FuriDate = ConvertDate(enc.GetString(rbyte, 54, 4)).ToString("yyyyMMdd")
                    If GetToriInfo(Syubetu, ItakuCode) = False Then
                        MainLOG.Write("取引先情報取得", "失敗", "")
                        Return False
                    End If
            End Select

            If GetSchInfo() = False Then
                ToriInfo.ItakuCode = "8888888888"
                Return True
            End If

            CodeChange = ""
            Select Case CodeKbnData
                Case "0"
                    Select Case ToriInfo.CodeKbn
                        Case "4"
                            CodeChange = "SJIStoEBC"
                    End Select
                Case "4"
                    Select Case ToriInfo.CodeKbn
                        Case "0"
                            CodeChange = "EBCtoSJIS"
                    End Select
            End Select

            Return True

        Catch ex As Exception
            MainLOG.Write("ジョブ情報取得", "失敗", ex.Message)
            Return False
        Finally
            If br IsNot Nothing Then
                br.Close()
                br = Nothing
            End If
            If fs IsNot Nothing Then
                fs.Close()
                fs = Nothing
            End If
        End Try

    End Function

    Private Function GetSchInfo() As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write("スケジュール情報取得", "開始")

            Dim SQL As New StringBuilder(128)
            SQL.Length = 0
            Select Case ToriInfo.FSyoriKbn
                Case "1"
                    '自振取引先マスタ検索
                    SQL.Append(" SELECT * ")
                    SQL.Append(" FROM SCHMAST")
                    SQL.Append(" WHERE ")
                    SQL.Append("     TORIS_CODE_S   = " & SQ(ToriInfo.TorisCode))
                    SQL.Append(" AND TORIF_CODE_S   = " & SQ(ToriInfo.TorifCode))
                    SQL.Append(" AND FURI_DATE_S    = " & SQ(ToriInfo.FuriDate))
                    SQL.Append(" AND TOUROKU_FLG_S  = '0'")
                Case Else
                    MainLOG.Write("スケジュール情報取得", "成功", "総振のため何度でもＯＫとする")
                    Return True
            End Select

            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
            Else
                MainLOG.Write("スケジュール情報取得", "失敗", "取引先コード:" & ToriInfo.TorisCode & "-" & ToriInfo.TorifCode & " 振替日:" & ToriInfo.FuriDate)
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("スケジュール情報取得", "失敗", ex.Message)
            Return False
        Finally
            If OraReader IsNot Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            MainLOG.Write("スケジュール情報取得", "終了")
        End Try

    End Function

    Private Function GetToriInfo(ByVal Syubetu As String, ByVal ItakuCode As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write("取引先情報取得", "開始")

            Dim SQL As New StringBuilder(128)
            SQL.Length = 0
            Select Case Syubetu
                Case "91"
                    '自振取引先マスタ検索
                    SQL.Append(" SELECT * ")
                    SQL.Append(" FROM TORIMAST ")
                    SQL.Append(" WHERE ")
                    SQL.Append("     ITAKU_CODE_T = " & SQ(ItakuCode))
                    SQL.Append(" AND SYUBETU_T    = " & SQ(Syubetu))
                    SQL.Append(" ORDER BY ")
                    SQL.Append("     TORIS_CODE_T ")
                    SQL.Append("   , TORIF_CODE_T ")
                Case Else
                    '総振取引先マスタ検索
                    SQL.Length = 0
                    SQL.Append(" SELECT * ")
                    SQL.Append(" FROM S_TORIMAST ")
                    SQL.Append(" WHERE ")
                    SQL.Append("     ITAKU_CODE_T = " & SQ(ItakuCode))
                    SQL.Append(" AND SYUBETU_T    = " & SQ(Syubetu))
                    SQL.Append(" ORDER BY ")
                    SQL.Append("     TORIS_CODE_T ")
                    SQL.Append("   , TORIF_CODE_T ")
            End Select

            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                ToriInfo.Data = OraReader
            Else
                MainLOG.Write("取引先情報取得", "失敗", "種別：" & Syubetu & " 委託者コード：" & ItakuCode)
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("取引先情報取得", "失敗", ex.Message)
            Return False
        Finally
            If OraReader IsNot Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            MainLOG.Write("取引先情報取得", "終了")
        End Try

    End Function

    Private Function FileCodeChange(ByVal strIN_FILE_NAME As String, ByVal strOUT_FILE_NAME As String, _
                                    ByVal intREC_LENGTH As Integer, _
                                    ByVal strCODE_KBN As String, _
                                    ByVal strP_FILE As String) As Integer
        '=====================================================================================
        'NAME           :fn_DISK_CPYTO_DEN
        'Parameter      :strTORI_CODE：取引先コード／strIN_FILE_NAME：入力ファイル名／
        '               :strOUT_FILE_NAME：出力ファイル／intREC_LENGTH：レコード長／
        '               :strCODE_KBN：コード区分／strP_FILE：FTRAN+の定義ファイル
        'Description    :ファイルを伝送ファイルにコピーする
        'Return         :0=成功、100=コード変換失敗、
        'Create         :2004/08/20
        'Update         :
        '=====================================================================================
        Try
            Dim strDIR As String
            strDIR = CurDir()

            Dim FTR_OPENDIR As String = CASTCommon.GetFSKJIni("COMMON", "FTR")
            If FTR_OPENDIR.Equals("err") = True OrElse FTR_OPENDIR = String.Empty Then
                Return 200
            End If

            Dim FTRANP_OPENDIR As String = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
            If FTRANP_OPENDIR.Equals("err") = True OrElse FTRANP_OPENDIR = String.Empty Then
                Return 201
            End If

            '-----------------------------------------------------------
            'コード変換処理
            'コード変換区分が 2 の場合はIBM形式 EBCDIC コードとみなし
            'JIS8コードへのコード変換を行う
            '-----------------------------------------------------------
            Dim strCMD As String = ""
            Dim strTEIGI_FIEL As String
            If Dir(strOUT_FILE_NAME) <> "" Then
                Kill(strOUT_FILE_NAME)
            End If
            ChDir(FTRANP_OPENDIR)

            strTEIGI_FIEL = FTR_OPENDIR & strP_FILE
            Select Case strCODE_KBN
                Case "EBCtoSJIS"  'JIS  ホスト→WINファイル変換
                    strCMD = "FP /nwd/ cload " & FTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & """" & strIN_FILE_NAME & """" & " " & """" & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                Case "SJIStoEBC"  'EBCDIC コード区分変更 2009/09/30 kakiwaki
                    strCMD = "FP /nwd/ cload " & FTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis putrand " & """" & strIN_FILE_NAME & """" & " " & """" & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                Case Else
                    strCMD = "FP /nwd/ cload " & FTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getdata " & """" & strIN_FILE_NAME & """" & " " & """" & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
            End Select

            Dim ProcFT As New Process
            Dim ProcInfo As New ProcessStartInfo(FTRANP_OPENDIR & "FP", strCMD.Substring(3))
            ProcInfo.CreateNoWindow = True
            ProcInfo.WorkingDirectory = FTRANP_OPENDIR
            ProcFT = Process.Start(ProcInfo)
            ProcFT.WaitForExit()
            Dim lngEXITCODE As Integer = ProcFT.ExitCode

            If lngEXITCODE = 0 Then
                Return 0
            Else
                Return 100         'コード変換失敗
            End If

            ChDir(strDIR)
        Catch ex As Exception
            Return 999
        End Try

    End Function

#End Region

End Class
