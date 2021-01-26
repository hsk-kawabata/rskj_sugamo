Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.Data

Public Class ClsDensouSend

    Private MainLOG As New CASTCommon.BatchLOG("KFD002", "伝送連携送信")

    Private MainDB As CASTCommon.MyOracle = Nothing

    Private FileNameFullPath As String = ""
    Private DensouXmlFile As String = "DensoRenkei.xml"

    ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- START
    Private JobMessage As String = " "
    ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- END

#Region "送信ファイル情報構造体"
    Private Structure StructSendFileInfo
        Public SyoriName As String
        Public DestFileName As String

        Sub Init()
            SyoriName = Nothing
            DestFileName = Nothing
        End Sub
    End Structure
    Private SendFileInfo As New StructSendFileInfo
#End Region

#Region "取引先情報構造体"
    Private Structure StructToriInfo
        Public ToriCode As String
        Public ItakuCode As String
        Public DaihyoItakuCode As String
        Public FuriDate As String

        Sub Init()
            ToriCode = Nothing
            ItakuCode = Nothing
            DaihyoItakuCode = Nothing
        End Sub
    End Structure

    Private ToriInfo As New StructToriInfo

#End Region

#Region " 初期処理"

    ''' <summary>
    ''' ① 初期処理メイン
    ''' </summary>
    ''' <returns>正常:True 異常:False</returns>
    ''' <remarks></remarks>
    Public Function Init(ByVal CmdArgs() As String) As Boolean

        Dim ret As Boolean = False

        Dim param() As String

        Try
            MainLOG.Write("初期処理", "開始", "")

            'パラメータの読込
            param = CmdArgs(0).Split(","c)

            MainLOG.UserID = "admin"
            MainLOG.FuriDate = "00000000"
            MainLOG.ToriCode = "000000000000"

            '取引先情報初期化
            Call ToriInfo.Init()

            Select Case param.Length
                Case 1
                    'ファイル名のみで連携
                    FileNameFullPath = param(0)
                    MainLOG.JobTuuban = 0
                Case 2
                    'ファイル名のみで連携（JOB監視再実行）
                    FileNameFullPath = param(0)
                    MainLOG.JobTuuban = CInt(param(1))
                Case Else
                    MainLOG.Write("初期処理", "失敗", "コマンドライン引数のパラメータが不正です")
                    Exit Try
            End Select

            ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- START
            JobMessage = " "
            ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- END

            ret = True

        Catch ex As Exception
            MainLOG.Write("初期処理", "失敗", ex.Message)
        Finally
            MainLOG.Write("初期処理", "終了", "")
        End Try

        Return ret

    End Function

#End Region

    Public Function SndMain() As Boolean

        Dim ret As Boolean = False

        Try
            MainLOG.Write("伝送連携送信メイン処理", "開始")

            If Not File.Exists(FileNameFullPath) Then
                'ファイル無し
                MainLOG.Write("伝送連携受信メイン処理", "失敗", "ファイル無し:" & FileNameFullPath)
                ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- START
                JobMessage = "ファイルなし:" & FileNameFullPath
                ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- END
                Exit Try
            End If

            'ファイル名のみ抽出
            Dim FileName As String = System.IO.Path.GetFileName(FileNameFullPath)

            If Not GetSendFileName(FileName) Then
                Exit Try
            End If

            If File.Exists(FileNameFullPath) = True Then
                Dim BackupFileName As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DENBK"), Path.GetFileName(FileNameFullPath))

                If File.Exists(BackupFileName) = True Then
                    File.Delete(BackupFileName)
                End If

                System.Threading.Thread.Sleep(2000)
                File.Move(FileNameFullPath, BackupFileName)
            End If

            ret = True

        Catch ex As Exception
            MainLOG.Write("伝送連携送信メイン処理", "失敗", ex.Message)
        Finally
            If MainLOG.JobTuuban <> 0 Then
                'ジョブ監視を更新
                If ret Then
                    If Not MainLOG.UpdateJOBMASTbyOK("") Then
                        Throw New Exception("JOB監視更新失敗")
                    End If
                Else
                    If Not MainLOG.UpdateJOBMASTbyErr("ログ参照") Then
                        Throw New Exception("JOB監視更新失敗")
                    End If
                End If
            Else
                'ジョブ監視を異常終了で登録
                If Not ret Then
                    MainDB = New CASTCommon.MyOracle
                    MainDB.BeginTrans()

                    '異常終了
                    ' 2017/03/22 タスク）綾部 CHG 【ME】(ジョブメッセージ考慮不足修正) -------------------- START
                    'If Not MainLOG.InsertJOBMAST(MainLOG.ModuleName.Substring(2, 4), "admin", FileNameFullPath, MainDB, "7") Then
                    '    Throw New Exception("JOB監視登録失敗")
                    'End If
                    If JobMessage.Trim = "" Then
                        JobMessage = "ログ参照"
                    End If

                    If Not MainLOG.InsertJOBMASTbyError(MainLOG.ModuleName.Substring(2, 4), _
                                                        "admin", _
                                                        FileNameFullPath, _
                                                        MainDB, _
                                                        JobMessage) Then
                        Throw New Exception("JOB監視登録失敗")
                    End If
                    ' 2017/03/22 タスク）綾部 CHG 【ME】(ジョブメッセージ考慮不足修正) -------------------- END

                    MainDB.Commit()
                    MainDB.Close()
                    MainDB = Nothing
                End If
            End If

            '終了処理
            If MainDB IsNot Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLOG.Write("伝送連携送信メイン処理", "終了")
        End Try

        Return ret

    End Function

    Private Function ExecLinkProcess(ByVal SorceFileName As String, ByVal DestFileName As String) As Boolean

        Dim ret As Boolean = False
        Dim errArguments As String = ""

        Try
            'LinkExpressで伝送サーバに送信するファイル名を取得
            Dim Proc As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = CASTCommon.GetFSKJIni("OTHERSYS", "LINK")
            Dim Argum As String = CASTCommon.GetFSKJIni("OTHERSYS", "LINK-PARAM")
            If File.Exists(ProcInfo.FileName) = True Then
                ProcInfo.WorkingDirectory = ""
                ProcInfo.Arguments = Argum.Replace("%1", SorceFileName).Replace("%2", DestFileName)

                errArguments = ProcInfo.Arguments

                Proc = Process.Start(ProcInfo)
                Proc.WaitForExit()
                If Proc.ExitCode = 0 Then
                    ' 連携成功
                    ret = True
                Else
                    ' 連携失敗
                    MainLOG.Write("伝送ファイル送信処理", "失敗", "Return:" & Proc.ExitCode.ToString & " " & ProcInfo.FileName & " " & ProcInfo.Arguments)

                    Proc.StandardOutput.ReadToEnd()
                    ret = False
                End If
            Else
                MainLOG.Write("伝送ファイル送信処理", "失敗", "起動アプリケーションなし：" & ProcInfo.FileName)

                ret = False
            End If

            Proc.Close()

        Catch ex As Exception
            MainLOG.Write("伝送ファイル送信処理", "失敗", ex.Message & " " & errArguments)
            ret = False
        End Try

        Return ret

    End Function

    Private Function GetSendFileName(ByVal Key As String) As Boolean

        Dim ret As Boolean = False

        MainLOG.Write("送信ファイル情報取得", "開始")

        Try
            '外部ファイルからファイル名にマッチするジョブ情報を取得
            Dim ds As New DataSet

            ds.ReadXml(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, DensouXmlFile))

            Dim dr As DataRow() = ds.Tables("send_file").Select("key = '" & Key & "'")

            If dr Is Nothing Then
                MainLOG.Write("送信ファイル情報取得", "失敗", DensouXmlFile & "の設定に誤りがあります")
                ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- START
                JobMessage = "伝送定義XMLファイル設定誤り(send_file該当なし) key=" & Key
                ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- END
                Exit Try
            End If

            If dr.Count = 1 Then
                'xmlから取得できれば送信ファイル情報を格納する
                SendFileInfo.SyoriName = dr(0).Item("syori_name")
                SendFileInfo.DestFileName = dr(0).Item("dest_file")

                SendFileInfo.DestFileName = SendFileInfo.DestFileName.Replace("%TIME%", Now.ToString("yyyyMMddHHmmssfff"))
            Else
                'xmlから取得できなければスケジュールマスタから送信ファイル名を取得する
                If Not GetSendFileNameFromSchmast(Key) Then
                    Exit Try
                End If
            End If

            If ExecLinkProcess(FileNameFullPath, SendFileInfo.DestFileName) Then
                ret = True
            End If

        Catch ex As Exception
            MainLOG.Write("送信ファイル情報取得", "失敗", ex.Message)
        Finally
            MainLOG.Write("ジョブ情報取得", "終了")
        End Try

        Return ret

    End Function

    Private Function GetSendFileNameFromSchmast(ByVal Key As String) As Boolean

        Dim ret As Boolean = False

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            
            Dim SelectPatn As String = ""
            Dim SQL As New StringBuilder(128)
            Select Case Key.Length
                Case 26
                    'シングルデータのため「取引先コード」で検索
                    ToriInfo.ToriCode = Key.Substring(1, 12)
                    ToriInfo.FuriDate = Key.Substring(14, 8)

                    SQL.Length = 0
                    SQL.Append(" SELECT ")
                    SQL.Append("       DENSO_FILENAME_S ")
                    SQL.Append("     , TOHO_CNT_CODE_T ")
                    SQL.Append("     , AITE_CNT_CODE_T ")
                    SQL.Append("     , DENSO_FILE_ID_T ")
                    SQL.Append(" FROM ")
                    SQL.Append("      TORIMAST_VIEW ")
                    SQL.Append("    , SCHMAST_VIEW ")
                    SQL.Append(" WHERE TORIS_CODE_S =" & SQ(ToriInfo.ToriCode.Substring(0, 10)))
                    SQL.Append(" AND   TORIF_CODE_S =" & SQ(ToriInfo.ToriCode.Substring(10)))
                    SQL.Append(" AND   TORIS_CODE_S = TORIS_CODE_T ")
                    SQL.Append(" AND   TORIF_CODE_S = TORIF_CODE_T ")
                    SQL.Append(" AND   FURI_DATE_S  =" & SQ(ToriInfo.FuriDate))
                    SQL.Append(" AND   UKETUKE_FLG_S      = '1' ")
                    SQL.Append(" AND   FUNOU_FLG_S        = '1' ")
                    SQL.Append(" AND   TRIM(DENSO_FILENAME_S) IS NOT NULL")
                Case Else
                    'マルチデータのため「代表委託者コード」で検索
                    ToriInfo.DaihyoItakuCode = Key.Substring(1, 10)
                    ToriInfo.FuriDate = Key.Substring(12, 8)

                    SQL.Append(" SELECT ")
                    SQL.Append("       DENSO_FILENAME_S ")
                    SQL.Append("     , TOHO_CNT_CODE_T ")
                    SQL.Append("     , AITE_CNT_CODE_T ")
                    SQL.Append("     , DENSO_FILE_ID_T ")
                    SQL.Append(" FROM ")
                    SQL.Append("      TORIMAST_VIEW ")
                    SQL.Append("    , SCHMAST_VIEW ")
                    SQL.Append(" WHERE ITAKU_KANRI_CODE_T =" & SQ(ToriInfo.DaihyoItakuCode.Substring(0, 10)))
                    SQL.Append(" AND   TORIS_CODE_S       = TORIS_CODE_T ")
                    SQL.Append(" AND   TORIF_CODE_S       = TORIF_CODE_T ")
                    SQL.Append(" AND   FURI_DATE_S        =" & SQ(ToriInfo.FuriDate))
                    SQL.Append(" AND   UKETUKE_FLG_S      = '1' ")
                    SQL.Append(" AND   FUNOU_FLG_S        = '1' ")
                    SQL.Append(" AND   TRIM(DENSO_FILENAME_S) IS NOT NULL")
            End Select

            OraReader = New CASTCommon.MyOracleReader
            If OraReader.DataReader(SQL) Then
                Dim AiteCntCode As String = OraReader.GetItem("DENSO_FILENAME_S").Substring(3, 14)
                Dim TohoCntCode As String = OraReader.GetItem("DENSO_FILENAME_S").Substring(17, 14)
                Dim ZenFileName As String = OraReader.GetItem("DENSO_FILENAME_S").Substring(31, 12)

                '取引先マスタ優先
                If OraReader.GetItem("AITE_CNT_CODE_T").Trim <> "" Then
                    AiteCntCode = OraReader.GetItem("AITE_CNT_CODE_T").Trim.PadRight(14, "0"c)
                End If

                If OraReader.GetItem("TOHO_CNT_CODE_T").Trim <> "" Then
                    TohoCntCode = OraReader.GetItem("TOHO_CNT_CODE_T").Trim.PadRight(14, "0"c)
                End If

                If OraReader.GetItem("DENSO_FILE_ID_T").Trim <> "" Then
                    ZenFileName = OraReader.GetItem("DENSO_FILE_ID_T").Trim.PadRight(12, "0"c)
                End If

                SendFileInfo.DestFileName = "JFR"
                SendFileInfo.DestFileName &= AiteCntCode & TohoCntCode & ZenFileName
                SendFileInfo.DestFileName &= Now.ToString("yyyyMMddHHmmssfff")
                SendFileInfo.DestFileName &= ".dat"
                ret = True
                ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- START
            Else
                Select Key.Length
                    Case 26
                        JobMessage = "スケジュール取得失敗 取引先コード:" & ToriInfo.ToriCode.Substring(0, 10) & "-" & ToriInfo.ToriCode.Substring(10) & " 振替日:" & ToriInfo.FuriDate
                    Case Else
                        JobMessage = "スケジュール取得失敗 代表委託者コード:" & ToriInfo.DaihyoItakuCode.Substring(0, 10) & " 振替日:" & ToriInfo.FuriDate
                End Select
                ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- END
            End If

            OraReader.Close()
            OraReader = Nothing

        Catch ex As Exception
            MainLOG.Write("送信ファイル情報取得", "失敗", ex.Message)
        Finally
            If OraReader IsNot Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function
End Class
