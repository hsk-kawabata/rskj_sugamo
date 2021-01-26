Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.Data
Imports MenteCommon

Public Class ClsDensouReceive

    Private MainLOG As New CASTCommon.BatchLOG("KFD011", "伝送連携受信個別")

    Private FileNameFullPath As String = ""
    Private SplitFileNameFullPath As String = ""
    Private DensouXmlFile As String = "DensoRenkei.xml"

    Private MainDB As CASTCommon.MyOracle = Nothing

    Private GCom As MenteCommon.clsCommon

    ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- START
    Private JobMessage As String = " "
    ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- END

#Region "ジョブ登録情報構造体"

    Private Structure StructJobInfo
        Public SyoriKbn As String
        Public SyoriName As String
        Public JobName As String
        Public Parameta As String
        Public ItakuCode As String
        Public PositionFuriDate As String
        Public DestFileName As String
        Public Command As String

        Sub Init()
            SyoriKbn = Nothing
            SyoriName = Nothing
            JobName = Nothing
            Parameta = Nothing
            ItakuCode = Nothing
            PositionFuriDate = Nothing
            DestFileName = Nothing
            Command = Nothing
        End Sub
    End Structure
    Private JobInfo As New StructJobInfo
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
        Public FileSplitNo As Integer

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

        '落し込み連携用のファイル名を生成
        Public Function GetTourokuFileName() As String

            Dim RetFileName As String = ""

            '処理区分
            If FSyoriKbn = "1" Then
                RetFileName = "J"
            Else
                RetFileName = "S"
            End If
            '媒体区分
            Dim BaitaiName As String = GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "媒体命名規約.txt"), BaitaiCode)
            If BaitaiName.Trim = "" Then
                Throw New Exception("媒体命名規約取得失敗")
            End If
            RetFileName &= "_" & BaitaiName

            'マルチ区分、取引先情報
            If MultiKbn = "0" Then
                RetFileName &= "_" & "S"
                RetFileName &= "_" & TorisCode & TorifCode
            Else
                RetFileName &= "_" & "M"
                RetFileName &= "_" & DaihyoItakuCode & "00"
            End If
            'フォーマット区分
            RetFileName &= "_" & FmtKbn

            '振替日
            RetFileName &= "_" & FuriDate

            'タイムスタンプ
            RetFileName &= "_" & Now.ToString("yyyyMMddHHmmssfff")

            'プロセスID
            Dim ProcessID As Integer = System.Diagnostics.Process.GetCurrentProcess.Id
            RetFileName &= "_" & ProcessID.ToString("00000000").Substring(8 - 4)

            'ファイル分割番号(000)
            RetFileName &= "_" & FileSplitNo.ToString("000")

            '拡張子
            RetFileName &= ".dat"

            Return RetFileName

        End Function
    End Structure
    Private ToriInfo As New StructToriInfo

#End Region

#Region "初期処理"

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

            ToriInfo.FileSplitNo = 0

            If param.Length = 2 Then
                FileNameFullPath = param(0)
                MainLOG.JobTuuban = CInt(param(1))
                MainLOG.Write("初期処理", "成功", "パラメータ① ファイルパス:" & FileNameFullPath)
                MainLOG.Write("初期処理", "成功", "パラメータ② ジョブ通番　:" & MainLOG.JobTuuban)
            ElseIf param.Length = 1 Then
                FileNameFullPath = param(0)
                MainLOG.JobTuuban = 0
                MainLOG.Write("初期処理", "成功", "パラメータ① ファイルパス:" & FileNameFullPath)
                MainLOG.Write("初期処理", "成功", "　　　　　　 ジョブ通番　:指定なし(0設定)")
            Else
                MainLOG.Write("初期処理", "失敗", "コマンドライン引数のパラメータが不正です")
                Exit Try
            End If

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

    Public Function RevMain() As Boolean

        Dim ret As Boolean = False

        Try
            MainLOG.Write("伝送連携受信個別メイン処理", "開始")

            '==============================================
            ' FAX振込データ有無チェック
            ' 存在する場合は、処理開始時点で退避する
            ' ファイル削除は後続にて実施
            '==============================================
            Dim BackupFile As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DENBK"), Path.GetFileName(FileNameFullPath))
            If Not File.Exists(FileNameFullPath) Then
                'ファイル無し
                MainLOG.Write("伝送連携受信個別メイン処理", "失敗", "ファイル無し:" & FileNameFullPath)
                ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- START
                JobMessage = "ファイルなし:" & FileNameFullPath
                ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- END
                Exit Try
            End If
            File.Copy(FileNameFullPath, BackupFile, True)

            '==============================================
            ' 処理対象/分割対象ファイルかどうか判定する
            ' ヘッダ、トレーラ、エンドレコードのの場合は、
            ' 処理対象外とする
            '==============================================
            Dim FileInfo_FAX As New System.IO.FileInfo(FileNameFullPath)
            If FileInfo_FAX.Length <= 360 Then
                File.Delete(FileNameFullPath)
                MainLOG.Write("伝送連携受信個別メイン処理", "成功", "処理対象無しのため処理中断:" & FileNameFullPath & " -> " & BackupFile)
                ret = True
                Exit Try
            End If

            '==============================================
            ' ファイル分割処理
            '==============================================
            Dim SplitFileName As New ArrayList
            SplitFileName.Clear()
            Dim clsFileDivide As New ClsFileDivideMultiToSingle
            clsFileDivide.InFilePath = FileNameFullPath
            clsFileDivide.RecordLength = 120
            clsFileDivide.UserId = MainLOG.UserID
            clsFileDivide.ToriCode = MainLOG.ToriCode
            clsFileDivide.FuriDate = MainLOG.FuriDate
            Dim iRetValue As Integer = clsFileDivide.DivideFileMain(SplitFileName)
            Select Case iRetValue
                Case 0
                    MainLOG.Write(MainLOG.UserID, MainLOG.ToriCode, MainLOG.FuriDate, "伝送連携受信個別メイン処理", "成功", "")
                Case Else
                    MainLOG.Write(MainLOG.UserID, MainLOG.ToriCode, MainLOG.FuriDate, "伝送連携受信個別メイン処理", "失敗", "分割処理結果コード:" & iRetValue)
                    ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- START
                    JobMessage = "ファイル分割処理失敗"
                    ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- END
                    Exit Try
            End Select

            '==============================================
            ' 分割ファイル数分繰り返し
            '  FAX振込は自振連携のみ
            '==============================================
            For i As Integer = 0 To SplitFileName.Count - 1 Step 1
                MainLOG.Write(MainLOG.UserID, MainLOG.ToriCode, MainLOG.FuriDate, "伝送連携受信個別メイン処理", "成功", "ファイル名:" & SplitFileName(i).ToString)
            Next

            MainDB = New CASTCommon.MyOracle
            For i As Integer = 0 To SplitFileName.Count - 1 Step 1
                SplitFileNameFullPath = SplitFileName(i).ToString
                MainDB.BeginTrans()

                'Key 当方センター確認コード+相手センター確認コード+全銀ファイル名先頭8桁
                If Not GetJobInfo(Path.GetFileName(SplitFileNameFullPath).Substring(3, 36)) Then
                    Exit Try
                End If

                File.Copy(SplitFileNameFullPath, JobInfo.DestFileName, True)

                If Not MainLOG.InsertJOBMAST(JobInfo.JobName, "admin", JobInfo.Parameta, MainDB) Then
                    MainLOG.Write("ジョブ監視登録", "失敗", "InsertJOBMAST")
                    ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- START
                    JobMessage = "落込処理登録失敗 JOB名:" & JobInfo.JobName & " パラメータ:" & JobInfo.Parameta
                    ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- END
                    Exit Try
                End If

                MainDB.Commit()
            Next
            MainDB.Close()
            MainDB = Nothing

            '==============================================
            ' FAX振込データ削除
            '==============================================
            If File.Exists(FileNameFullPath) = True Then
                If File.Exists(JobInfo.DestFileName) = True Then
                    File.Delete(FileNameFullPath)
                End If
            End If

            MainLOG.Write("伝送連携受信個別メイン処理", "成功")

            ret = True

        Catch ex As Exception
            MainLOG.Write("伝送連携受信個別メイン処理", "失敗", ex.Message)
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

            MainLOG.Write("伝送連携受信個別メイン処理", "終了")
        End Try

        Return ret

    End Function

    Private Function GetJobInfo(ByVal Key As String) As Boolean

        Dim ret As Boolean = False

        MainLOG.Write("ジョブ情報取得", "開始")

        Try
            'ジョブ情報初期化
            Call JobInfo.Init()

            'xmlから取得できなければ全銀ファイル落し込みのジョブ情報を格納する
            If Not GetJobInfoTouroku() Then
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            MainLOG.Write("ジョブ情報取得", "失敗", ex.Message)
        Finally
            MainLOG.Write("ジョブ情報取得", "終了")
        End Try

        Return ret

    End Function

    Private Function GetJobInfoTouroku() As Boolean

        Dim ret As Boolean = False

        Dim fs As FileStream = Nothing
        Dim br As BinaryReader = Nothing

        'デフォルトS-JIS
        Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding(932)

        Dim Syubetu As String = ""
        Dim FuriDate As String = ""
        Dim FuriDateMoto As String = ""

        Try
            fs = New FileStream(SplitFileNameFullPath, FileMode.Open)
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
                Case 240 To 249 'F0～F9
                    'IBM290
                    enc = System.Text.Encoding.GetEncoding(20290)
            End Select

            '全銀フォーマット
            '種別をファイルから取得
            Syubetu = enc.GetString(rbyte, 1, 2)
            '委託者コードをファイルから取得
            JobInfo.ItakuCode = enc.GetString(rbyte, 4, 10)
            '振替日をファイルから取得
            FuriDateMoto = enc.GetString(rbyte, 54, 4)
            '振替日を西暦に返還
            FuriDate = ConvertDate(FuriDateMoto).ToString("yyyyMMdd")

            '取引先情報取得
            If Not GetToriInfo(Syubetu, JobInfo.ItakuCode, FuriDate) Then
                Exit Try
            End If

            '総振 落し込みJOB用パラメタ組み立て
            JobInfo.JobName = "S010"
            JobInfo.SyoriKbn = "0"
            JobInfo.Parameta = ToriInfo.TorisCode & ToriInfo.TorifCode & ","
            JobInfo.Parameta &= ToriInfo.FuriDate & ","
            JobInfo.Parameta &= ToriInfo.CodeKbn & ","
            JobInfo.Parameta &= ToriInfo.FmtKbn & ","
            JobInfo.Parameta &= ToriInfo.BaitaiCode & ","
            JobInfo.Parameta &= ToriInfo.LabelKbn
            
            'コピー先ファイル名組み立て
            Dim DestFilePath As String = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If DestFilePath.ToUpper = "ERR" Then
                MainLOG.Write("INIファイル取得", "失敗", "[COMMON].DEN")
                Exit Try
            End If

            ToriInfo.FileSplitNo += 1
            JobInfo.DestFileName = Path.Combine(DestFilePath, ToriInfo.GetTourokuFileName)

            ret = True

        Catch ex As Exception
            MainLOG.Write("ジョブ情報取得（詳細）", "失敗", ex.Message)
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

        Return ret

    End Function

    Private Function GetToriInfo(ByVal Syubetu As String, ByVal ItakuCode As String, ByVal FuriDate As String) As Boolean

        Dim ret As Boolean = False

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write("取引先情報取得", "開始")

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim SQL As New StringBuilder(128)

            '総振取引先マスタ検索
            SQL.Length = 0
            SQL.Append(" SELECT * ")
            SQL.Append(" FROM S_TORIMAST ")
            SQL.Append(" WHERE ITAKU_CODE_T = " & SQ(ItakuCode))
            SQL.Append(" AND SYUBETU_T = " & SQ(Syubetu))
            SQL.Append(" ORDER BY TORIS_CODE_T, TORIF_CODE_T ")

            If Not OraReader.DataReader(SQL) Then
                MainLOG.Write("取引先情報取得", "失敗", "種別：" & Syubetu & " 委託者コード：" & ItakuCode)
                ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- START
                JobMessage = "取引先情報取得失敗　種別：" & Syubetu & " 委託者コード：" & ItakuCode
                ' 2017/03/22 タスク）綾部 ADD 【ME】(ジョブメッセージ考慮不足修正) -------------------- END
                Exit Try
            End If

            '取引先情報をセット
            ToriInfo.Data = OraReader
            '振替日は別でセット
            ToriInfo.FuriDate = FuriDate

            OraReader.Close()
            OraReader = Nothing

            ret = True

        Catch ex As Exception
            MainLOG.Write("取引先情報取得", "失敗", ex.Message)
        Finally
            If OraReader IsNot Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            MainLOG.Write("取引先情報取得", "終了")
        End Try

        Return ret

    End Function

End Class
