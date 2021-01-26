Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.Data
Imports MenteCommon

Public Class ClsBiwareReceive

    Private MainLOG As New CASTCommon.BatchLOG("KFB001", "Biware受信")

    Private FileNameFullPath As String = ""
    Private DensouXmlFile As String = "DensoRenkei.xml"

    Private MainDB As CASTCommon.MyOracle = Nothing
    Private GCom As MenteCommon.clsCommon

    Private JobMessage As String = " "

    Private dblock As CASTCommon.CDBLock = Nothing
    Private LockWaitTime As Integer = 300

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
        Public ItakuNName As String

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
                ItakuNName = ToriMastReader.GetItem("ITAKU_NNAME_T")
            End Set
        End Property

        '落し込み連携用のファイル名を生成
        Public Function GetTourokuFileName(ByVal FuriDate As String) As String

            Dim RetFileName As String = "D"

            ' マルチ区分、取引先情報
            If MultiKbn = "0" Then
                RetFileName &= TorisCode & TorifCode
            Else
                RetFileName &= DaihyoItakuCode
            End If

            ' 振替日
            RetFileName &= "_" & FuriDate

            ' タイムスタンプ
            RetFileName &= "_" & Now.ToString("yyyyMMddHHmmssfff")

            ' 拡張子
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

            JobMessage = " "

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
            MainLOG.Write("Biware受信メイン処理", "開始")

            MainDB = New CASTCommon.MyOracle
            MainDB.BeginTrans()

            '----------------------------------------------------------------------
            ' << Biware受信処理 JOBロック実行(排他は300秒待ち) >>
            '  Biware受信後処理はマルチスレッド実行は行わない
            '----------------------------------------------------------------------
            If dblock Is Nothing Then
                dblock = New CASTCommon.CDBLock
                LockWaitTime = 300

                If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                    MainLOG.Write("Biware受信メイン処理", "失敗", "Biware受信メイン処理で実行待ちタイムアウト")
                    JobMessage = "Biware受信メイン処理で実行待ちタイムアウト"
                    Exit Try
                End If
            End If

            '----------------------------------------------------------------------
            ' << Biware受信処理 ファイル存在チェック >>
            '----------------------------------------------------------------------
            Dim FileName As String = String.Empty
            If Not File.Exists(FileNameFullPath) Then
                'ファイル無し
                MainLOG.Write("Biware受信メイン処理", "失敗", "ファイル無し:" & FileNameFullPath)
                JobMessage = "ファイルなし:" & FileNameFullPath
                Exit Try
            Else
                FileName = System.IO.Path.GetFileName(FileNameFullPath)
            End If

            '----------------------------------------------------------------------
            ' << Biware自振連携ファイル >>
            '  当機能ではファイル先頭ヘッダの情報から取得するため、
            '  なにを指定しても問題ないものとする。
            '  【ファイル名】シングル："D" + 取引先コード(12桁)     + ".DAT"
            '  　　　　　　　マルチ　："D" + 代表委託者コード(10桁) + ".DAT"
            '----------------------------------------------------------------------
            If GetJobInfo(FileName) = False Then
                Exit Try
            End If

            MainLOG.Write("Biware受信メイン処理", "開始", "処理区分 :" & JobInfo.SyoriKbn)
            MainLOG.Write("Biware受信メイン処理", "　　", "処理名　 :" & JobInfo.SyoriName)
            Select Case JobInfo.SyoriKbn
                Case "3"
                    '処理区分が3以外なら、元ファイルをコピーする
                    '          3の場合は、バッチコマンド実行を行う

                    Dim Proc As New Process
                    Dim ProcInfo As New ProcessStartInfo
                    ProcInfo.FileName = JobInfo.Command
                    ProcInfo.Arguments = JobInfo.Parameta

                    If File.Exists(ProcInfo.FileName) = True Then

                        ProcInfo.WorkingDirectory = ""

                        MainLOG.Write("Biware受信コマンド実行", "開始", "Command :" & ProcInfo.FileName)
                        MainLOG.Write("Biware受信コマンド実行", "　　", "Parameta:" & ProcInfo.Arguments)

                        Proc = Process.Start(ProcInfo)
                        Proc.WaitForExit()

                        If Proc.ExitCode = 0 Then
                            ' 連携成功
                            ret = True
                        Else
                            ' 連携失敗
                            MainLOG.Write("Biware受信コマンド実行", "失敗", "Return:" & Proc.ExitCode.ToString & " " & ProcInfo.FileName & " " & ProcInfo.Arguments)
                            Proc.StandardOutput.ReadToEnd()
                            Exit Try
                        End If
                    Else
                        MainLOG.Write("Biware受信コマンド実行", "失敗", "起動アプリケーションなし：" & ProcInfo.FileName)
                        Exit Try
                    End If
                Case "2"
                    If Trim(FileNameFullPath).ToUpper <> Trim(JobInfo.DestFileName).ToUpper Then
                        File.Copy(FileNameFullPath, JobInfo.DestFileName, True)
                    End If
                Case Else
                    If Trim(FileNameFullPath).ToUpper <> Trim(JobInfo.DestFileName).ToUpper Then
                        File.Copy(FileNameFullPath, JobInfo.DestFileName, True)

                        If File.Exists(FileNameFullPath) = True Then
                            If File.Exists(JobInfo.DestFileName) = True Then
                                File.Delete(FileNameFullPath)
                            End If
                        End If
                    End If
            End Select

            '処理区分が2または3以外なら、JOB監視登録
            Select Case JobInfo.SyoriKbn
                Case "2", "3"
                Case Else
                    If Not MainLOG.InsertJOBMAST(JobInfo.JobName, "admin", JobInfo.Parameta, MainDB) Then
                        MainLOG.Write("ジョブ監視登録", "失敗", "InsertJOBMAST")
                        JobMessage = "落込処理登録失敗 JOB名:" & JobInfo.JobName & " パラメータ:" & JobInfo.Parameta
                        Exit Try
                    End If
            End Select

            MainDB.Commit()
            MainDB.Close()
            MainDB = Nothing

            MainLOG.Write("Biware受信メイン処理", "成功")

            ret = True

        Catch ex As Exception
            MainLOG.Write("Biware受信メイン処理", "失敗", ex.Message)
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

            MainLOG.Write("Biware受信メイン処理", "終了")
        End Try

        Return ret

    End Function

    Private Function GetJobInfo(ByVal Key As String) As Boolean

        Dim ret As Boolean = False
        MainLOG.Write("ジョブ情報取得", "開始")

        Try
            'ジョブ情報初期化
            Call JobInfo.Init()

            '外部ファイルからファイル名にマッチするジョブ情報を取得
            Dim ds As New DataSet
            ds.ReadXml(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, DensouXmlFile))

            Dim dr As DataRow() = ds.Tables("receive_file").Select("key = '" & Key & "'")
            If dr Is Nothing Then
                MainLOG.Write("ジョブ情報取得", "失敗", DensouXmlFile & "の設定に誤りがあります")
                JobMessage = "伝送定義XMLファイル設定誤り(receive_file該当なし) key=" & Key
                Return False
            End If

            If dr.Count = 1 Then

                'xmlから取得できればジョブ情報を格納する
                JobInfo.SyoriKbn = dr(0).Item("syori_kbn")
                JobInfo.SyoriName = dr(0).Item("syori_name")

                Select Case JobInfo.SyoriKbn
                    Case "0"
                        '0:JOB登録有(落込以外)
                        JobInfo.JobName = dr(0).Item("job_name")
                        JobInfo.Parameta = dr(0).Item("parameta")
                        JobInfo.DestFileName = dr(0).Item("dest_file")

                        'ジョブのパラメタは以下の規約で差し替えを行う
                        '%DATE% システム日付(yyyyMMdd)
                        JobInfo.Parameta = JobInfo.Parameta.Replace("%DATE%", Now.ToString("yyyyMMdd"))

                        Dim ReplaceDate As String = String.Empty
                        Dim ReplaceStr As String = JobInfo.Parameta.Replace("DATE_BEF_", "@")
                        Dim SplitInfo() As String = ReplaceStr.Split("@"c)
                        If SplitInfo.Length > 1 Then
                            GCom = New MenteCommon.clsCommon
                            Dim ShiftDate As String = SplitInfo(1).Substring(0, 2)
                            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

                            bRet = GCom.CheckDateModule(Now.ToString("yyyyMMdd"), ReplaceDate, CInt(ShiftDate), 1)
                            JobInfo.Parameta = JobInfo.Parameta.Replace("%DATE_BEF_" & CInt(ShiftDate).ToString("00") & "%", ReplaceDate)
                        End If
                        SplitInfo = Nothing

                        ReplaceDate = String.Empty
                        ReplaceStr = JobInfo.Parameta.Replace("DATE_AFT_", "@")
                        SplitInfo = ReplaceStr.Split("@"c)
                        If SplitInfo.Length > 1 Then
                            GCom = New MenteCommon.clsCommon
                            Dim ShiftDate As String = SplitInfo(1).Substring(0, 2)
                            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

                            bRet = GCom.CheckDateModule(Now.ToString("yyyyMMdd"), ReplaceDate, CInt(ShiftDate), 0)
                            JobInfo.Parameta = JobInfo.Parameta.Replace("%DATE_AFT_" & CInt(ShiftDate).ToString("00") & "%", ReplaceDate)
                        End If

                    Case "1"
                        '1:JOB登録有(全銀以外の落込)
                        JobInfo.ItakuCode = dr(0).Item("itaku_code")
                        JobInfo.PositionFuriDate = dr(0).Item("position_furidate")

                        If Not GetJobInfoTouroku() Then
                            Return False
                        End If
                    Case "2"
                        '2:JOB登録無(ファイルコピーのみ)
                        JobInfo.DestFileName = dr(0).Item("dest_file")
                    Case "3"
                        '3:コマンド実行
                        JobInfo.Command = dr(0).Item("command")

                        JobInfo.Parameta = dr(0).Item("parameta")
                        JobInfo.Parameta = JobInfo.Parameta.Replace("%RCV_NAME_F%", FileNameFullPath)
                        JobInfo.Parameta = JobInfo.Parameta.Replace("%RCV_NAME%", Path.GetFileName(FileNameFullPath))

                    Case Else
                        MainLOG.Write("ジョブ情報取得", "失敗", DensouXmlFile & "の設定に誤りがあります")
                        Return False
                End Select
            Else
                'xmlから取得できなければ全銀ファイル落し込みのジョブ情報を格納する
                If Not GetJobInfoTouroku() Then
                    Return False
                End If
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("ジョブ情報取得", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write("ジョブ情報取得", "終了")
        End Try

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
            fs = New FileStream(FileNameFullPath, FileMode.Open)
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

            If JobInfo.ItakuCode Is Nothing Then
                '全銀フォーマット
                '種別をファイルから取得
                Syubetu = enc.GetString(rbyte, 1, 2)
                '委託者コードをファイルから取得
                JobInfo.ItakuCode = enc.GetString(rbyte, 4, 10)
                '振替日をファイルから取得
                FuriDateMoto = enc.GetString(rbyte, 54, 4)
            Else
                '全銀フォーマット以外
                '種別は口座振替のみ想定
                Syubetu = "91"
                Dim PosFuriDate As String() = JobInfo.PositionFuriDate.Split(","c)
                Dim StartPos As Integer = PosFuriDate(0)
                Dim Count As Integer = PosFuriDate(1)
                '振替日をファイルから取得
                FuriDateMoto = enc.GetString(rbyte, StartPos, Count)
            End If

            '振替日を西暦に返還
            FuriDate = ConvertDate(FuriDateMoto).ToString("yyyyMMdd")

            '取引先情報取得
            If Not GetToriInfo(Syubetu, JobInfo.ItakuCode, FuriDate) Then
                Exit Try
            End If

            '落し込みJOB用パラメタ組み立て
            'CP.TORI_CODE = para(0)                      '取引先コード
            'CP.FURI_DATE = para(1)                      '振替日
            'CP.CODE_KBN = para(2)                       'コード区分
            'CP.FMT_KBN = para(3).PadLeft(2, "0"c)       'フォーマット区分
            'CP.BAITAI_CODE = para(4).PadLeft(2, "0"c)   '媒体コード
            'CP.LABEL_KBN = para(5)                      'ラベル区分
            'CP.RENKEI_FILENAME = ""                     '連携ファイル名
            'CP.ENC_KBN = ""                             '暗号化処理区分
            'CP.ENC_KEY1 = ""                            '暗号化キー１
            'CP.ENC_KEY2 = ""                            '暗号化キー２
            'CP.ENC_OPT1 = ""                            'ＡＥＳオプション
            'CP.CYCLENO = ""                             'サイクル№
            'CP.JOBTUUBAN = Integer.Parse(para(6))       'ジョブ通番
            JobInfo.Parameta = ToriInfo.TorisCode & ToriInfo.TorifCode & ","
            JobInfo.Parameta &= ToriInfo.FuriDate & ","
            JobInfo.Parameta &= ToriInfo.CodeKbn & ","
            JobInfo.Parameta &= ToriInfo.FmtKbn & ","
            JobInfo.Parameta &= ToriInfo.BaitaiCode & ","
            JobInfo.Parameta &= ToriInfo.LabelKbn & ","

            If ToriInfo.FSyoriKbn = "1" Then
                '自振落込
                JobInfo.JobName = "J010"
                JobInfo.SyoriKbn = "0"
            Else
                '総振落込
                JobInfo.JobName = "S010"
                JobInfo.SyoriKbn = "0"
            End If

            'コピー先ファイル名組み立て
            Dim DestFilePath As String = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If DestFilePath.ToUpper = "ERR" Then
                MainLOG.Write("INIファイル取得", "失敗", "[COMMON].DEN")
                Exit Try
            End If

            JobInfo.DestFileName = Path.Combine(DestFilePath, ToriInfo.GetTourokuFileName(ToriInfo.FuriDate))

            JobInfo.Parameta &= JobInfo.DestFileName

            ret = True

        Catch ex As Exception
            MainLOG.Write("ジョブ情報取得", "失敗", ex.Message)
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

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write("取引先情報取得", "開始")

            '--------------------------------------------
            ' 自振取引先マスタ検索
            '--------------------------------------------
            Dim SQL As New StringBuilder(128)
            SQL.Append(" SELECT * FROM TORIMAST ")
            SQL.Append(" WHERE ")
            SQL.Append("     ITAKU_CODE_T = " & SQ(ItakuCode))
            SQL.Append(" AND SYUBETU_T    = " & SQ(Syubetu))
            SQL.Append(" ORDER BY ")
            SQL.Append("     TORIS_CODE_T ")
            SQL.Append("   , TORIF_CODE_T ")

            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If Not OraReader.DataReader(SQL) Then
                '--------------------------------------------
                ' 総振取引先マスタ検索
                '--------------------------------------------
                SQL.Length = 0
                SQL.Append(" SELECT * FROM S_TORIMAST ")
                SQL.Append(" WHERE ")
                SQL.Append("     ITAKU_CODE_T = " & SQ(ItakuCode))
                SQL.Append(" AND SYUBETU_T    = " & SQ(Syubetu))
                SQL.Append(" ORDER BY ")
                SQL.Append("     TORIS_CODE_T ")
                SQL.Append("   , TORIF_CODE_T ")

                If Not OraReader.DataReader(SQL) Then
                    MainLOG.Write("取引先情報取得", "失敗", "種別：" & Syubetu & " 委託者コード：" & ItakuCode)
                    JobMessage = "取引先情報取得失敗　種別：" & Syubetu & " 委託者コード：" & ItakuCode
                    Return False
                End If
            End If

            ToriInfo.Data = OraReader      '取引先情報をセット
            ToriInfo.FuriDate = FuriDate   '振替日は別でセット

            OraReader.Close()
            OraReader = Nothing

            MainLOG.Write("取引先情報取得", "成功", "種別：" & Syubetu & " 委託者コード：" & ItakuCode)

            Return True

        Catch ex As Exception
            MainLOG.Write("取引先情報取得", "失敗", ex.Message)
            JobMessage = "取引先情報取得異常　種別：" & Syubetu & " 委託者コード：" & ItakuCode
            Return False
        Finally
            If OraReader IsNot Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            MainLOG.Write("取引先情報取得", "終了")
        End Try

    End Function

End Class
