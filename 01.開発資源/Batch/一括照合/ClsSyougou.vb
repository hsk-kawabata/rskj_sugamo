Imports System
Imports System.Text

''' <summary>
''' 一括照合　メインクラス
''' </summary>
''' <remarks>2017/11/30 saitou 広島信金(RSV2標準版) added for 大規模構築対応</remarks>
Public Class ClsSyougou

#Region "クラス変数"

    Private mErrorCount As Integer = 0

    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    Private Structure strcSyougouParam
        Dim RENKEI_KBN As String
        Dim FSYORI_KBN As String

        Public WriteOnly Property Data() As String
            Set(ByVal value As String)
                Dim para() As String = value.Split(","c)
                RENKEI_KBN = para(0)
                FSYORI_KBN = para(1)
            End Set
        End Property
    End Structure
    Private SYOUGOU As strcSyougouParam

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 一括照合処理の初期設定を行います。
    ''' </summary>
    ''' <param name="CmdArgs">コマンドライン引数</param>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Public Function Init(CmdArgs() As String) As Boolean

        Try
            BatchLog.Write("(初期処理)開始", "成功")

            '----------------------------------------
            'パラメータチェック
            '----------------------------------------
            Dim para() As String = CmdArgs(0).Split(","c)
            If para.Length = 3 Then
                BatchLog.ToriCode = "000000000000"
                BatchLog.FuriDate = "00000000"
                BatchLog.JobTuuban = CInt(para(2))

                BatchLog.Write("パラメータチェック", "成功", "パラメータ引数：" & CmdArgs(0))

                Me.SYOUGOU.Data = CmdArgs(0)

            Else
                BatchLog.Write("パラメータチェック", "失敗", "パラメータ引数異常：" & CmdArgs(0))
                Return False
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write("初期処理", "失敗", ex.ToString)
            Return False
        Finally
            BatchLog.Write("(初期処理)終了", "成功")
        End Try
    End Function

    ''' <summary>
    ''' 一括照合の主処理を行います。
    ''' </summary>
    ''' <returns>0 - 正常 , 0以外 - 異常</returns>
    ''' <remarks></remarks>
    Public Function Main() As Integer

        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 60
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME1")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 60
            End If
        End If

        MainDB = New CASTCommon.MyOracle

        Try
            BatchLog.Write("(主処理)開始", "成功")

            MainDB.BeginTrans()

            ' ジョブ実行ロック
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                BatchLog.Write_Err("主処理", "失敗", "一括照合処理で実行待ちタイムアウト")
                BatchLog.UpdateJOBMASTbyErr("一括照合処理で実行待ちタイムアウト")
                Return -1
            End If

            ' 照合メイン
            If Me.SyougouMain() = False Then
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
                Return -1
            End If

            ' ジョブ実行アンロック
            dblock.Job_UnLock(MainDB)

            MainDB.Commit()
            MainDB.Close()
            MainDB = Nothing

            Return 0

        Catch ex As Exception
            BatchLog.Write("主処理", "失敗", ex.ToString)
            BatchLog.UpdateJOBMASTbyErr("ログ参照")
            Return 1

        Finally
            If Not MainDB Is Nothing Then
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)

                ' ロールバック
                MainDB.Rollback()

                MainDB.Close()
                MainDB = Nothing
            End If

            BatchLog.Write("(主処理)終了", "成功")
        End Try
    End Function

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 照合メイン処理
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function SyougouMain() As Boolean

        Try
            BatchLog.Write("(照合処理)開始", "成功")

            ' 照合ＯＫデータ処理
            If Me.SYOUGOU.FSYORI_KBN = "1" Then
                If Me.UpdateOKSchmast("1") = False Then
                    BatchLog.UpdateJOBMASTbyErr("照合(自振)OKデータ処理エラー")
                    Return False
                End If
            ElseIf Me.SYOUGOU.FSYORI_KBN = "3" Then
                If Me.UpdateOKS_Schmast() = False Then
                    BatchLog.UpdateJOBMASTbyErr("照合(振込)OKデータ処理エラー")
                    Return False
                End If
            End If

            ' データなし処理
            If Me.UpdateDataNothing() = False Then
                BatchLog.UpdateJOBMASTbyErr("データなし処理エラー")
                Return False
            End If

            ' 合計票なし処理
            If Me.UpdateSofujyoNothing() = False Then
                BatchLog.UpdateJOBMASTbyErr("合計票なし処理エラー")
                Return False
            End If

            ' インプットエラーあり確認処理
            If Me.UpdateInputError() = False Then
                BatchLog.UpdateJOBMASTbyErr("インプットエラーあり処理エラー")
                Return False
            End If

            ' 重複処理
            If Me.UpdateDuplicate() = False Then
                BatchLog.UpdateJOBMASTbyErr("重複処理エラー")
                Return False
            End If

            ' 受付照合エラーリスト印刷
            Dim Message As String = ""
            If Me.PrintErrorList(Message) = False Then
                BatchLog.UpdateJOBMASTbyErr("受付照合エラーリスト出力処理エラー")
                Return False
            End If

            ' 照合漏れがないことを確認
            Call Me.LastCheck()

            ' ジョブマスタ更新
            Call BatchLog.UpdateJOBMASTbyOK(Message)

            Return True

        Catch ex As Exception
            BatchLog.Write("照合処理", "失敗", ex.ToString)
            Return False

        Finally
            BatchLog.Write("(照合処理)終了", "成功")
        End Try

    End Function

    ''' <summary>
    ''' 照合OKデータ処理
    ''' </summary>
    ''' <param name="fsyoriKbn">振替処理区分</param>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function UpdateOKSchmast(fsyoriKbn As String) As Boolean
        Dim SQL As StringBuilder
        Dim nCount As Integer

        Try
            '----------------------------------------
            'スケジュールマスタ更新
            '一致データ
            '----------------------------------------
            SQL = New StringBuilder
            With SQL
                If fsyoriKbn = "1" Then
                    .Append("update SCHMAST")
                ElseIf fsyoriKbn = "3" Then
                    .Append("update S_SCHMAST")
                End If
                .Append(" set TOUROKU_FLG_S = '1'")
                .Append("    ,TOUROKU_DATE_S = '" & mMatchingDate & "'")
                .Append("    ,ERROR_INF_S = ''")
                .Append(" where")
                .Append("     UKETUKE_FLG_S = '1'")
                .Append(" and exists (")
                .Append("     select")
                .Append("         *")
                .Append("     from")
                If fsyoriKbn = "1" Then
                    .Append("         SCHMAST_SUB")
                ElseIf fsyoriKbn = "3" Then
                    .Append("         S_SCHMAST_SUB")
                End If
                .Append("     where")
                .Append("         TORIS_CODE_S = TORIS_CODE_SSUB")
                .Append("     and TORIF_CODE_S = TORIF_CODE_SSUB")
                .Append("     and FURI_DATE_S = FURI_DATE_SSUB")
                If fsyoriKbn = "3" Then
                    .Append("     and MOTIKOMI_SEQ_S = MOTIKOMI_SEQ_SSUB")
                End If
                .Append("     and SYOUGOU_FLG_S = '8'")
                .Append(" )")
                .Append(" and exists (")
                .Append("     select")
                .Append("         *")
                .Append("     from")
                .Append("         MEDIA_ENTRY_TBL")
                .Append("     inner join")
                If fsyoriKbn = "1" Then
                    .Append("         TORIMAST_VIEW")
                ElseIf fsyoriKbn = "3" Then
                    .Append("         S_TORIMAST_VIEW")
                End If
                .Append("     on  TORIS_CODE_ME = TORIS_CODE_T")
                .Append("     and TORIF_CODE_ME = TORIF_CODE_T")
                .Append("     where")
                .Append("         TORIS_CODE_ME = TORIS_CODE_S")
                .Append("     and TORIF_CODE_ME = TORIF_CODE_S")
                .Append("     and FURI_DATE_ME = FURI_DATE_S")
                If fsyoriKbn = "3" Then
                    .Append("     and CYCLE_NO_ME = MOTIKOMI_SEQ_S")
                End If
                .Append("     and SYORI_KEN_ME = SYORI_KEN_S")
                .Append("     and SYORI_KIN_ME = SYORI_KIN_S")
                .Append("     and CHECK_FLG_ME = '8'")
                .Append("     and SYOUGOU_KBN_T = '1'")
                .Append(" )")
            End With

            nCount = MainDB.ExecuteNonQuery(SQL)
            If nCount < 0 Then
                BatchLog.Write("スケジュールマスタ更新　照合OK処理", "失敗", MainDB.Message)
                Return False
            End If

            BatchLog.Write("スケジュールマスタ更新　照合OK処理", "成功", "件数=" & nCount.ToString)

            '----------------------------------------
            'スケジュールマスタサブ更新
            '一致データ
            '----------------------------------------
            With SQL
                .Length = 0
                If fsyoriKbn = "1" Then
                    .Append("update SCHMAST_SUB")
                ElseIf fsyoriKbn = "3" Then
                    .Append("update S_SCHMAST_SUB")
                End If
                .Append(" set SYOUGOU_FLG_S = '1'")
                .Append("    ,SYOUGOU_DATE_S = '" & mMatchingDate & "'")
                .Append(" where")
                .Append("     SYOUGOU_FLG_S = '8'")
                .Append(" and exists (")
                .Append("     select")
                .Append("         *")
                .Append("     from")
                If fsyoriKbn = "1" Then
                    .Append("         SCHMAST")
                ElseIf fsyoriKbn = "3" Then
                    .Append("         S_SCHMAST")
                End If
                .Append("     where")
                .Append("         TORIS_CODE_SSUB = TORIS_CODE_S")
                .Append("     and TORIF_CODE_SSUB = TORIF_CODE_S")
                .Append("     and FURI_DATE_SSUB = FURI_DATE_S")
                If fsyoriKbn = "3" Then
                    .Append("     and MOTIKOMI_SEQ_SSUB = MOTIKOMI_SEQ_S")
                End If
                .Append("     and UKETUKE_FLG_S = '1'")
                .Append(" )")
                .Append(" and exists (")
                .Append("     select")
                .Append("         *")
                .Append("     from")
                .Append("         MEDIA_ENTRY_TBL")
                .Append("     inner join")
                If fsyoriKbn = "1" Then
                    .Append("         TORIMAST_VIEW")
                ElseIf fsyoriKbn = "3" Then
                    .Append("         S_TORIMAST_VIEW")
                End If
                .Append("     on  TORIS_CODE_ME = TORIS_CODE_T")
                .Append("     and TORIF_CODE_ME = TORIF_CODE_T")
                .Append("     inner join")
                If fsyoriKbn = "1" Then
                    .Append("         SCHMAST")
                ElseIf fsyoriKbn = "3" Then
                    .Append("         S_SCHMAST")
                End If
                .Append("     on  TORIS_CODE_ME = TORIS_CODE_S")
                .Append("     and TORIF_CODE_ME = TORIF_CODE_S")
                .Append("     and FURI_DATE_ME = FURI_DATE_S")
                If fsyoriKbn = "3" Then
                    .Append("     and CYCLE_NO_ME = MOTIKOMI_SEQ_S")
                End If
                .Append("     and SYORI_KEN_ME = SYORI_KEN_S")
                .Append("     and SYORI_KIN_ME = SYORI_KIN_S")
                .Append("     where")
                .Append("         TORIS_CODE_S = TORIS_CODE_SSUB")
                .Append("     and TORIF_CODE_S = TORIF_CODE_SSUB")
                .Append("     and FURI_DATE_S = FURI_DATE_SSUB")
                If fsyoriKbn = "3" Then
                    .Append("     and MOTIKOMI_SEQ_S = MOTIKOMI_SEQ_SSUB")
                End If
                .Append("     and CHECK_FLG_ME = '8'")
                .Append("     and SYOUGOU_KBN_T = '1'")
                .Append(" )")
            End With

            nCount = MainDB.ExecuteNonQuery(SQL)
            If nCount < 0 Then
                BatchLog.Write("スケジュールマスタサブ更新　照合OK処理", "失敗", MainDB.Message)
                Return False
            End If

            BatchLog.Write("スケジュールマスタサブ更新　照合OK処理", "成功", "件数=" & nCount.ToString)

            '----------------------------------------
            '送付状実績テーブル更新
            '一致データ
            '----------------------------------------
            With SQL
                .Length = 0
                .Append("update MEDIA_ENTRY_TBL")
                .Append(" set CHECK_FLG_ME = '1'")
                .Append("    ,UPDATE_DATE_ME = '" & mMatchingDate & mMatchingTime & "'")
                .Append(" where")
                .Append("     CHECK_FLG_ME = '8'")
                .Append(" and exists (")
                .Append("     select")
                .Append("         *")
                .Append("     from")
                If fsyoriKbn = "1" Then
                    .Append("         SCHMAST")
                ElseIf fsyoriKbn = "3" Then
                    .Append("         S_SCHMAST")
                End If
                .Append("     inner join")
                If fsyoriKbn = "1" Then
                    .Append("         TORIMAST_VIEW")
                ElseIf fsyoriKbn = "3" Then
                    .Append("         S_TORIMAST_VIEW")
                End If
                .Append("     on  TORIS_CODE_S = TORIS_CODE_T")
                .Append("     and TORIF_CODE_S = TORIF_CODE_T")
                .Append("     inner join")
                If fsyoriKbn = "1" Then
                    .Append("         SCHMAST_SUB")
                ElseIf fsyoriKbn = "3" Then
                    .Append("         S_SCHMAST_SUB")
                End If
                .Append("     on  TORIS_CODE_S = TORIS_CODE_SSUB")
                .Append("     and TORIF_CODE_S = TORIF_CODE_SSUB")
                .Append("     and FURI_DATE_S = FURI_DATE_SSUB")
                If fsyoriKbn = "3" Then
                    .Append("     and MOTIKOMI_SEQ_S = MOTIKOMI_SEQ_SSUB")
                End If
                .Append("     where")
                .Append("         TORIS_CODE_ME = TORIS_CODE_S")
                .Append("     and TORIF_CODE_ME = TORIF_CODE_S")
                .Append("     and FURI_DATE_ME = FURI_DATE_S")
                If fsyoriKbn = "3" Then
                    .Append("     and CYCLE_NO_ME = MOTIKOMI_SEQ_S")
                End If
                .Append("     and SYORI_KEN_ME = SYORI_KEN_S")
                .Append("     and SYORI_KIN_ME = SYORI_KIN_S")
                .Append("     and UKETUKE_FLG_S = '1'")
                .Append("     and SYOUGOU_FLG_S = '1'")
                .Append("     and TOUROKU_FLG_S = '1'")
                .Append("     and SYOUGOU_KBN_T = '1'")
                .Append(" )")
            End With

            nCount = MainDB.ExecuteNonQuery(SQL)
            If nCount < 0 Then
                BatchLog.Write("送付状実績テーブル更新　照合OK処理", "失敗", MainDB.Message)
                Return False
            End If

            BatchLog.Write("送付状実績テーブル更新　照合OK処理", "成功", "件数=" & nCount.ToString)

            Return True

        Catch ex As Exception
            BatchLog.Write("照合OK処理", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 照合(振込)OKデータ処理
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function UpdateOKS_Schmast() As Boolean
        Dim SQL As New StringBuilder
        Dim nCount As Integer
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim OraEntryRea As CASTCommon.MyOracleReader = Nothing

        Try
            'スケジュールマスタから対象データを抽出して、送付状実績テーブルのサイクルを更新する
            With SQL
                .Append("select")
                .Append("     TORIS_CODE_S")
                .Append("    ,TORIF_CODE_S")
                .Append("    ,FURI_DATE_S")
                .Append("    ,MOTIKOMI_SEQ_S")
                .Append("    ,SYORI_KEN_S")
                .Append("    ,SYORI_KIN_S")
                .Append("    ,ERR_KEN_S")
                .Append("    ,ERR_KIN_S")
                .Append(" from")
                .Append("     S_SCHMAST_VIEW")
                .Append(" inner join")
                .Append("     S_TORIMAST_VIEW")
                .Append(" on  TORIS_CODE_S = TORIS_CODE_T")
                .Append(" and TORIF_CODE_S = TORIF_CODE_T")
                .Append(" where")
                .Append("     UKETUKE_FLG_S = '1'")
                .Append(" and SYOUGOU_FLG_S = '8'")
                .Append(" and SYOUGOU_KBN_T = '1'")
            End With

            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                nCount = 0
                OraEntryRea = New CASTCommon.MyOracleReader(MainDB)
                Do
                    With SQL
                        .Length = 0
                        .Append("select")
                        .Append("     FSYORI_KBN_ME")
                        .Append("    ,TORIS_CODE_ME")
                        .Append("    ,TORIF_CODE_ME")
                        .Append("    ,ENTRY_DATE_ME")
                        .Append("    ,STATION_ENTRY_NO_ME")
                        .Append("    ,BAITAI_CODE_ME")
                        .Append("    ,ENTRY_NO_ME")
                        .Append(" from")
                        .Append("     MEDIA_ENTRY_TBL")
                        .Append(" where")
                        .Append("     FSYORI_KBN_ME = '3'")
                        .Append(" and TORIS_CODE_ME = '" & OraReader.GetString("TORIS_CODE_S") & "'")
                        .Append(" and TORIF_CODE_ME = '" & OraReader.GetString("TORIF_CODE_S") & "'")
                        .Append(" and FURI_DATE_ME = '" & OraReader.GetString("FURI_DATE_S") & "'")
                        .Append(" and SYORI_KEN_ME = " & OraReader.GetInt("SYORI_KEN_S").ToString)
                        .Append(" and SYORI_KIN_ME = " & OraReader.GetInt64("SYORI_KIN_S").ToString)
                        .Append(" and CYCLE_NO_ME = 0")
                        .Append(" and CHECK_FLG_ME = '8'")
                    End With

                    If OraEntryRea.DataReader(SQL) = True Then
                        '送付状実績テーブルとスケジュールマスタを関連付け
                        With SQL
                            .Length = 0
                            .Append("update MEDIA_ENTRY_TBL")
                            .Append(" set CYCLE_NO_ME = " & OraReader.GetInt("MOTIKOMI_SEQ_S").ToString)
                            .Append("    ,UPDATE_DATE_ME = '" & mMatchingDate & mMatchingTime & "'")
                            .Append(" where")
                            .Append("     FSYORI_KBN_ME = '" & OraEntryRea.GetString("FSYORI_KBN_ME") & "'")
                            .Append(" and TORIS_CODE_ME = '" & OraEntryRea.GetString("TORIS_CODE_ME") & "'")
                            .Append(" and TORIF_CODE_ME = '" & OraEntryRea.GetString("TORIF_CODE_ME") & "'")
                            .Append(" and ENTRY_DATE_ME = '" & OraEntryRea.GetString("ENTRY_DATE_ME") & "'")
                            .Append(" and STATION_ENTRY_NO_ME = '" & OraEntryRea.GetString("STATION_ENTRY_NO_ME") & "'")
                            .Append(" and BAITAI_CODE_ME = '" & OraEntryRea.GetString("BAITAI_CODE_ME") & "'")
                            .Append(" and ENTRY_NO_ME = " & OraEntryRea.GetInt("ENTRY_NO_ME").ToString)
                        End With

                        nCount += MainDB.ExecuteNonQuery(SQL)
                        If nCount <= 0 Then
                            If MainDB.Code <> 0 Then
                                BatchLog.Write("送付状実績テーブル更新　照合(振込)OKデータ処理", "失敗", OraEntryRea.Code.ToString)
                                Return False
                            End If
                        End If
                    Else
                        If OraEntryRea.Code <> 0 Then
                            BatchLog.Write("送付状実績テーブル更新　照合(振込)OKデータ処理", "失敗", OraEntryRea.Code.ToString)
                            Return False
                        End If
                    End If

                    OraEntryRea.Close()

                    OraReader.NextRead()
                Loop Until OraReader.EOF
                OraReader.Close()
                BatchLog.Write("送付状実績テーブル更新　照合(振込)OKデータ処理", "成功", "件数=" & nCount.ToString)
            End If
        Catch ex As Exception
            BatchLog.Write("送付状実績テーブル更新　照合(振込)OKデータ処理", "失敗", ex.ToString)
            Return False

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not OraEntryRea Is Nothing Then
                OraEntryRea.Close()
                OraEntryRea = Nothing
            End If
        End Try

        Return UpdateOKSchmast("3")

    End Function

    ''' <summary>
    ''' データなし処理
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function UpdateDataNothing() As Boolean
        Dim SQL As StringBuilder
        Dim nCount As Integer

        Try
            '----------------------------------------
            '照合エラー履歴テーブル登録
            'データなし
            '----------------------------------------
            SQL = New StringBuilder
            With SQL
                .Append("insert into MATCHING_ERR_TBL (")
                .Append("     MATCHING_DATE_SE")
                .Append("    ,MATCHING_TIME_SE")
                .Append("    ,SEQ_SE")
                .Append("    ,FSYORI_KBN_SE")
                .Append("    ,TORIS_CODE_SE")
                .Append("    ,TORIF_CODE_SE")
                .Append("    ,FURI_DATE_SE")
                .Append("    ,CYCLE_NO_SE")
                .Append("    ,UKE_DATE_SE")
                .Append("    ,UKE_BAITAI_CODE_SE")
                .Append("    ,UKE_STATION_ENTRY_NO_SE")
                .Append("    ,UKE_ENTRY_NO_SE")
                .Append("    ,UKE_ITAKU_CODE_SE")
                .Append("    ,UKE_FURI_DATE_SE")
                .Append("    ,UKE_KEN_SE")
                .Append("    ,UKE_KIN_SE")
                .Append("    ,IN_DATE_SE")
                .Append("    ,IN_BAITAI_CODE_SE")
                .Append("    ,IN_ITAKU_CODE_SE")
                .Append("    ,IN_FURI_DATE_SE")
                .Append("    ,IN_KEN_SE")
                .Append("    ,IN_KIN_SE")
                .Append("    ,ERR_CODE_SE")
                .Append("    ,ERR_TEXT_SE")
                .Append(")")
                .Append(" select")
                .Append("     '" & mMatchingDate & "'")
                .Append("    ,'" & mMatchingTime & "'")
                .Append("    ,ROWNUM+" & mErrorCount.ToString)
                .Append("    ,FSYORI_KBN_ME")
                .Append("    ,TORIS_CODE_ME")
                .Append("    ,TORIF_CODE_ME")
                .Append("    ,FURI_DATE_ME")
                .Append("    ,CYCLE_NO_ME")
                .Append("    ,ENTRY_DATE_ME")
                .Append("    ,BAITAI_CODE_ME")
                .Append("    ,STATION_ENTRY_NO_ME")
                .Append("    ,ENTRY_NO_ME")
                .Append("    ,ITAKU_CODE_ME")
                .Append("    ,FURI_DATE_ME")
                .Append("    ,SYORI_KEN_ME")
                .Append("    ,SYORI_KIN_ME")
                .Append("    ,''")
                .Append("    ,''")
                .Append("    ,''")
                .Append("    ,''")
                .Append("    ,0")
                .Append("    ,0")
                .Append("    ,4")
                .Append("    ,'データなし'")
                .Append(" from")
                .Append("     MEDIA_ENTRY_TBL")
                .Append(" where")
                .Append("     CHECK_FLG_ME = '8'")
                .Append(" and FSYORI_KBN_ME = '" & Me.SYOUGOU.FSYORI_KBN & "'")
            End With

            nCount = MainDB.ExecuteNonQuery(SQL)
            If nCount < 0 Then
                BatchLog.Write("照合エラー履歴テーブル登録　データなし処理", "失敗", MainDB.Message)
                Return False
            End If

            BatchLog.Write("照合エラー履歴テーブル登録　データなし処理", "成功", "件数=" & nCount.ToString)
            mErrorCount += nCount

            '----------------------------------------
            '送付状実績テーブル更新　一致データ
            '----------------------------------------
            With SQL
                .Length = 0
                .Append("update MEDIA_ENTRY_TBL")
                .Append(" set CHECK_FLG_ME = '9'")
                .Append("    ,UPDATE_DATE_ME = '" & mMatchingDate & mMatchingTime & "'")
                .Append(" where")
                .Append("     CHECK_FLG_ME = '8'")
            End With

            nCount = MainDB.ExecuteNonQuery(SQL)
            If nCount < 0 Then
                BatchLog.Write("送付状実績テーブル更新　データなし処理", "失敗", MainDB.Message)
                Return False
            End If

            BatchLog.Write("送付状実績テーブル更新　データなし処理", "成功", "件数=" & nCount.ToString)

            Return True

        Catch ex As Exception
            BatchLog.Write("データなし処理", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 合計票なし処理
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function UpdateSofujyoNothing() As Boolean
        Dim SQL As StringBuilder
        Dim nCount As Integer

        Try
            '----------------------------------------
            '照合エラー履歴テーブル登録
            '合計票なし
            '----------------------------------------
            SQL = New StringBuilder
            With SQL
                .Append("insert into MATCHING_ERR_TBL (")
                .Append("     MATCHING_DATE_SE")
                .Append("    ,MATCHING_TIME_SE")
                .Append("    ,SEQ_SE")
                .Append("    ,FSYORI_KBN_SE")
                .Append("    ,TORIS_CODE_SE")
                .Append("    ,TORIF_CODE_SE")
                .Append("    ,FURI_DATE_SE")
                .Append("    ,CYCLE_NO_SE")
                .Append("    ,UKE_DATE_SE")
                .Append("    ,UKE_BAITAI_CODE_SE")
                .Append("    ,UKE_STATION_ENTRY_NO_SE")
                .Append("    ,UKE_ENTRY_NO_SE")
                .Append("    ,UKE_ITAKU_CODE_SE")
                .Append("    ,UKE_FURI_DATE_SE")
                .Append("    ,UKE_KEN_SE")
                .Append("    ,UKE_KIN_SE")
                .Append("    ,IN_DATE_SE")
                .Append("    ,IN_BAITAI_CODE_SE")
                .Append("    ,IN_ITAKU_CODE_SE")
                .Append("    ,IN_FURI_DATE_SE")
                .Append("    ,IN_KEN_SE")
                .Append("    ,IN_KIN_SE")
                .Append("    ,ERR_CODE_SE")
                .Append("    ,ERR_TEXT_SE")
                .Append(")")
                .Append(" select")
                .Append("     '" & mMatchingDate & "'")
                .Append("    ,'" & mMatchingTime & "'")
                .Append("    ,ROWNUM+" & mErrorCount.ToString)
                .Append("    ,FSYORI_KBN_S")
                .Append("    ,TORIS_CODE_S")
                .Append("    ,TORIF_CODE_S")
                .Append("    ,FURI_DATE_S")
                .Append("    ,MOTIKOMI_SEQ_S")
                .Append("    ,''")
                .Append("    ,''")
                .Append("    ,''")
                .Append("    ,0")
                .Append("    ,''")
                .Append("    ,''")
                .Append("    ,0")
                .Append("    ,0")
                .Append("    ,UKETUKE_DATE_S")
                .Append("    ,BAITAI_CODE_S")
                .Append("    ,ITAKU_CODE_S")
                .Append("    ,FURI_DATE_S")
                .Append("    ,SYORI_KEN_S")
                .Append("    ,SYORI_KIN_S")
                .Append("    ,decode(ERR_KEN_S, 0, 3, 23)")
                .Append("    ,decode(ERR_KEN_S, 0, '合計票なし', '合計票なし/インプットエラー確認')")
                .Append(" from")
                If Me.SYOUGOU.FSYORI_KBN = "1" Then
                    .Append("     SCHMAST_VIEW")
                ElseIf Me.SYOUGOU.FSYORI_KBN = "3" Then
                    .Append("     S_SCHMAST_VIEW")
                End If
                .Append(" where")
                .Append("     SYOUGOU_FLG_S = '8'")
            End With

            nCount = MainDB.ExecuteNonQuery(SQL)
            If nCount < 0 Then
                BatchLog.Write("照合エラー履歴テーブル登録　合計票なし処理", "失敗", MainDB.Message)
                Return False
            End If

            BatchLog.Write("照合エラー履歴テーブル登録　合計票なし処理", "成功", "件数=" & nCount.ToString)
            mErrorCount += nCount

            '----------------------------------------
            'スケジュールマスタ更新　一致データ
            '----------------------------------------
            With SQL
                .Length = 0
                If Me.SYOUGOU.FSYORI_KBN = "1" Then
                    .Append("update SCHMAST_SUB")
                ElseIf Me.SYOUGOU.FSYORI_KBN = "3" Then
                    .Append("update S_SCHMAST_SUB")
                End If
                .Append(" set SYOUGOU_FLG_S = '9'")
                .Append(" where")
                .Append("     SYOUGOU_FLG_S = '8'")
            End With

            nCount = MainDB.ExecuteNonQuery(SQL)
            If nCount < 0 Then
                BatchLog.Write("スケジュールマスタ更新　合計票なし処理", "失敗", MainDB.Message)
                Return False
            End If

            BatchLog.Write("スケジュールマスタ更新　合計票なし処理", "成功", "件数=" & nCount.ToString)

            Return True

        Catch ex As Exception
            BatchLog.Write("合計票なし処理", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' インプットエラーあり確認処理
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function UpdateInputError() As Boolean
        Dim SQL As StringBuilder
        Dim nCount As Integer

        Try
            '----------------------------------------
            '照合エラー履歴テーブル登録
            'インプットエラーあり
            '----------------------------------------
            SQL = New StringBuilder
            With SQL
                .Append("insert into MATCHING_ERR_TBL (")
                .Append("     MATCHING_DATE_SE")
                .Append("    ,MATCHING_TIME_SE")
                .Append("    ,SEQ_SE")
                .Append("    ,FSYORI_KBN_SE")
                .Append("    ,TORIS_CODE_SE")
                .Append("    ,TORIF_CODE_SE")
                .Append("    ,FURI_DATE_SE")
                .Append("    ,CYCLE_NO_SE")
                .Append("    ,UKE_DATE_SE")
                .Append("    ,UKE_BAITAI_CODE_SE")
                .Append("    ,UKE_STATION_ENTRY_NO_SE")
                .Append("    ,UKE_ENTRY_NO_SE")
                .Append("    ,UKE_ITAKU_CODE_SE")
                .Append("    ,UKE_FURI_DATE_SE")
                .Append("    ,UKE_KEN_SE")
                .Append("    ,UKE_KIN_SE")
                .Append("    ,IN_DATE_SE")
                .Append("    ,IN_BAITAI_CODE_SE")
                .Append("    ,IN_ITAKU_CODE_SE")
                .Append("    ,IN_FURI_DATE_SE")
                .Append("    ,IN_KEN_SE")
                .Append("    ,IN_KIN_SE")
                .Append("    ,ERR_CODE_SE")
                .Append("    ,ERR_TEXT_SE")
                .Append(")")
                .Append(" select")
                .Append("     '" & mMatchingDate & "'")
                .Append("    ,'" & mMatchingTime & "'")
                .Append("    ,ROWNUM+" & mErrorCount.ToString)
                .Append("    ,FSYORI_KBN_S")
                .Append("    ,TORIS_CODE_ME")
                .Append("    ,TORIF_CODE_ME")
                .Append("    ,FURI_DATE_ME")
                .Append("    ,CYCLE_NO_ME")
                .Append("    ,ENTRY_DATE_ME")
                .Append("    ,BAITAI_CODE_ME")
                .Append("    ,STATION_ENTRY_NO_ME")
                .Append("    ,ENTRY_NO_ME")
                .Append("    ,ITAKU_CODE_ME")
                .Append("    ,FURI_DATE_ME")
                .Append("    ,SYORI_KEN_ME")
                .Append("    ,SYORI_KIN_ME")
                .Append("    ,UKETUKE_DATE_S")
                .Append("    ,BAITAI_CODE_S")
                .Append("    ,ITAKU_CODE_S")
                .Append("    ,FURI_DATE_S")
                .Append("    ,SYORI_KEN_S")
                .Append("    ,SYORI_KIN_S")
                .Append("    ,21")
                .Append("    ,'インプットエラーあり確認'")
                .Append(" from")
                .Append("     MEDIA_ENTRY_TBL")
                .Append(" inner join")
                If Me.SYOUGOU.FSYORI_KBN = "1" Then
                    .Append("     SCHMAST")
                ElseIf Me.SYOUGOU.FSYORI_KBN = "3" Then
                    .Append("     S_SCHMAST")
                End If
                .Append(" on  TORIS_CODE_ME = TORIS_CODE_S")
                .Append(" and TORIF_CODE_ME = TORIF_CODE_S")
                .Append(" and FURI_DATE_ME = FURI_DATE_S")
                If Me.SYOUGOU.FSYORI_KBN = "3" Then
                    .Append(" and CYCLE_NO_ME = MOTIKOMI_SEQ_S")
                End If
                .Append(" where")
                .Append("     FSYORI_KBN_ME = '" & Me.SYOUGOU.FSYORI_KBN & "'")
                .Append(" and UKETUKE_FLG_S = '1'")
                .Append(" and TOUROKU_FLG_S = '1'")
                .Append(" and TOUROKU_DATE_S = '" & mMatchingDate & "'")
                .Append(" and ERR_KEN_S > 0")
            End With

            nCount = MainDB.ExecuteNonQuery(SQL)
            If nCount < 0 Then
                BatchLog.Write("照合エラー履歴テーブル登録　インプットエラーあり処理", "失敗", MainDB.Message)
                Return False
            End If

            BatchLog.Write("照合エラー履歴テーブル登録　インプットエラーあり処理", "成功", "件数=" & nCount.ToString)
            mErrorCount += nCount

            Return True

        Catch ex As Exception
            BatchLog.Write("インプットエラーあり処理", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 重複処理
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function UpdateDuplicate() As Boolean
        Dim SQL As StringBuilder
        Dim nCount As Integer

        Try
            If SYOUGOU.FSYORI_KBN = "1" Then
                Return True
            End If

            '----------------------------------------
            '照合エラー履歴テーブル登録
            '重複処理
            '----------------------------------------
            SQL = New StringBuilder
            With SQL
                .Append("insert into MATCHING_ERR_TBL (")
                .Append("     MATCHING_DATE_SE")
                .Append("    ,MATCHING_TIME_SE")
                .Append("    ,SEQ_SE")
                .Append("    ,FSYORI_KBN_SE")
                .Append("    ,TORIS_CODE_SE")
                .Append("    ,TORIF_CODE_SE")
                .Append("    ,FURI_DATE_SE")
                .Append("    ,CYCLE_NO_SE")
                .Append("    ,UKE_DATE_SE")
                .Append("    ,UKE_BAITAI_CODE_SE")
                .Append("    ,UKE_STATION_ENTRY_NO_SE")
                .Append("    ,UKE_ENTRY_NO_SE")
                .Append("    ,UKE_ITAKU_CODE_SE")
                .Append("    ,UKE_FURI_DATE_SE")
                .Append("    ,UKE_KEN_SE")
                .Append("    ,UKE_KIN_SE")
                .Append("    ,IN_DATE_SE")
                .Append("    ,IN_BAITAI_CODE_SE")
                .Append("    ,IN_ITAKU_CODE_SE")
                .Append("    ,IN_FURI_DATE_SE")
                .Append("    ,IN_KEN_SE")
                .Append("    ,IN_KIN_SE")
                .Append("    ,ERR_CODE_SE")
                .Append("    ,ERR_TEXT_SE")
                .Append(")")
                .Append(" select")
                .Append("     '" & mMatchingDate & "'")
                .Append("    ,'" & mMatchingTime & "'")
                .Append("    ,ROWNUM+" & mErrorCount.ToString)
                .Append("    ,FSYORI_KBN_S")
                .Append("    ,TORIS_CODE_S")
                .Append("    ,TORIF_CODE_S")
                .Append("    ,FURI_DATE_S")
                .Append("    ,MOTIKOMI_SEQ_S")
                If SYOUGOU.RENKEI_KBN <> "1" Then
                    .Append("    ,ENTRY_DATE_ME")
                    .Append("    ,BAITAI_CODE_ME")
                    .Append("    ,STATION_ENTRY_NO_ME")
                    .Append("    ,ENTRY_NO_ME")
                    .Append("    ,ITAKU_CODE_ME")
                    .Append("    ,FURI_DATE_ME")
                    .Append("    ,SYORI_KEN_ME")
                    .Append("    ,SYORI_KIN_ME")
                Else
                    .Append("    ,''")
                    .Append("    ,''")
                    .Append("    ,''")
                    .Append("    ,0")
                    .Append("    ,''")
                    .Append("    ,''")
                    .Append("    ,0")
                    .Append("    ,0")
                End If
                .Append("    ,UKETUKE_DATE_S")
                .Append("    ,BAITAI_CODE_S")
                .Append("    ,ITAKU_CODE_S")
                .Append("    ,FURI_DATE_S")
                .Append("    ,SYORI_KEN_S")
                .Append("    ,SYORI_KIN_S")
                .Append("    ,20")
                .Append("    ,'重複'")
                .Append(" from")
                .Append("     S_SCHMAST")
                .Append(" inner join")
                .Append("     S_SCHMAST_SUB")
                .Append(" on  TORIS_CODE_S = TORIS_CODE_SSUB")
                .Append(" and TORIF_CODE_S = TORIF_CODE_SSUB")
                .Append(" and FURI_DATE_S = FURI_DATE_SSUB")
                .Append(" and MOTIKOMI_SEQ_S = MOTIKOMI_SEQ_SSUB")
                .Append(" left outer join")
                .Append("     MEDIA_ENTRY_TBL")
                .Append(" on  TORIS_CODE_S = TORIS_CODE_ME")
                .Append(" and TORIF_CODE_S = TORIF_CODE_ME")
                .Append(" and FURI_DATE_S = FURI_DATE_ME")
                .Append(" and MOTIKOMI_SEQ_S = CYCLE_NO_ME")
                .Append(" where")
                .Append("     UKETUKE_FLG_S = '0'")
                .Append(" and SYOUGOU_FLG_S in ('0', '9')")
                .Append(" and TOUROKU_FLG_S = '0'")
                .Append(" and ERROR_INF_S = '020'")
            End With

            nCount = MainDB.ExecuteNonQuery(SQL)
            If nCount < 0 Then
                BatchLog.Write("照合エラー履歴テーブル登録　重複処理", "失敗", MainDB.Message)
                Return False
            End If

            BatchLog.Write("照合エラー履歴テーブル登録　重複", "成功処理", "件数=" & nCount.ToString)
            mErrorCount += nCount

            Return True

        Catch ex As Exception
            BatchLog.Write("重複処理", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 受付照合エラーリスト　印刷
    ''' </summary>
    ''' <param name="Message">メッセージ（参照渡し）</param>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function PrintErrorList(ByRef Message As String) As Boolean
        Dim SQL As New StringBuilder
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim PrnMeisai As ClsPrnSyougouError = Nothing

        Dim ErrCount As Integer = 0

        Dim JifuriBaitai As New CAstFormat.ClsText
        Dim SoufuriBaitai As New CAstFormat.ClsText("Common_総振_媒体コード.TXT")
        Dim JifuriSyubetu As New CAstFormat.ClsText("KFJMAST010_種別.TXT")
        Dim SoufuriSyubetu As New CAstFormat.ClsText("KFSMAST010_種別.TXT")

        Try
            If Me.SYOUGOU.FSYORI_KBN = "1" Then
                PrnMeisai = New ClsPrnSyougouError(1)
            ElseIf Me.SYOUGOU.FSYORI_KBN = "3" Then
                PrnMeisai = New ClsPrnSyougouError(3)
            End If
            PrnMeisai.OraDB = MainDB

            With SQL
                .Append("select")
                .Append("     MATCHING_DATE_SE")
                .Append("    ,MATCHING_TIME_SE")
                .Append("    ,SEQ_SE")
                .Append("    ,FSYORI_KBN_SE")
                .Append("    ,TORIS_CODE_SE")
                .Append("    ,TORIF_CODE_SE")
                .Append("    ,FURI_DATE_SE")
                .Append("    ,CYCLE_NO_SE")
                .Append("    ,UKE_DATE_SE")
                .Append("    ,UKE_BAITAI_CODE_SE")
                .Append("    ,UKE_ITAKU_CODE_SE")
                .Append("    ,UKE_FURI_DATE_SE")
                .Append("    ,UKE_KEN_SE")
                .Append("    ,UKE_KIN_SE")
                .Append("    ,IN_BAITAI_CODE_SE")
                .Append("    ,IN_ITAKU_CODE_SE")
                .Append("    ,IN_FURI_DATE_SE")
                .Append("    ,IN_KEN_SE")
                .Append("    ,IN_KIN_SE")
                .Append("    ,ERR_CODE_SE")
                .Append("    ,ERR_TEXT_SE")
                .Append("    ,TSIT_NO_T")
                .Append("    ,TORIMATOME_SIT_T")
                .Append("    ,ITAKU_CODE_T")
                .Append("    ,ITAKU_NNAME_T")
                .Append("    ,MULTI_KBN_T")
                .Append("    ,KANREN_KIGYO_CODE_T")
                .Append("    ,BAITAI_CODE_T")
                .Append("    ,SYUBETU_T")
                .Append("    ,ENTRY_NO_ME")
                .Append("    ,SYUBETU_CODE_ME")
                .Append("    ,STATION_ENTRY_NO_ME")
                .Append("    ,SYUBETU_CODE_ME")
                .Append(" from")
                .Append("     MATCHING_ERR_TBL")
                .Append(" left outer join")
                .Append("     TORIMAST")
                .Append(" on  FSYORI_KBN_SE = FSYORI_KBN_T")
                .Append(" and TORIS_CODE_SE = TORIS_CODE_T")
                .Append(" and TORIF_CODE_SE = TORIF_CODE_T")
                .Append(" left outer join")
                .Append("     MEDIA_ENTRY_TBL")
                .Append(" on  FSYORI_KBN_SE = FSYORI_KBN_ME")
                .Append(" and TORIS_CODE_SE = TORIS_CODE_ME")
                .Append(" and TORIF_CODE_SE = TORIF_CODE_ME")
                .Append(" and UKE_DATE_SE = ENTRY_DATE_ME")
                .Append(" and UKE_STATION_ENTRY_NO_SE = STATION_ENTRY_NO_ME")
                .Append(" and UKE_BAITAI_CODE_SE = BAITAI_CODE_ME")
                .Append(" and UKE_ENTRY_NO_SE = ENTRY_NO_ME")
                .Append(" left outer join")
                .Append("     SCHMAST")
                .Append(" on  FSYORI_KBN_SE = FSYORI_KBN_S")
                .Append(" and TORIS_CODE_SE = TORIS_CODE_S")
                .Append(" and TORIF_CODE_SE = TORIF_CODE_S")
                .Append(" and FURI_DATE_SE = FURI_DATE_S")
                .Append(" and CYCLE_NO_SE = MOTIKOMI_SEQ_S")
                .Append(" where")
                .Append("     MATCHING_DATE_SE = '" & mMatchingDate & "'")
                .Append(" and MATCHING_TIME_SE = '" & mMatchingTime & "'")
                .Append(" and FSYORI_KBN_SE = '" & Me.SYOUGOU.FSYORI_KBN & "'")
                'エラーコード21(インプットエラー)は当日のみ出力
                .Append(" and (ERR_CODE_SE <> 21")
                .Append(" or  (ERR_CODE_SE = 21 and IN_DATE_SE = '" & mMatchingDate & "')")
                .Append(" )")
                .Append(" order by")
                .Append("     BAITAI_CODE_T")
                .Append("    ,TORIMATOME_SIT_T")
                .Append("    ,ITAKU_CODE_T")
                .Append("    ,FURI_DATE_SE")
                .Append("    ,STATION_ENTRY_NO_ME")
                .Append("    ,ENTRY_NO_ME")
            End With

            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If Me.SYOUGOU.FSYORI_KBN = "3" Then
                SQL.Replace("TORIMAST", "S_TORIMAST").Replace("SCHMAST", "S_SCHMAST")
            End If

            Dim name As String = ""
            If OraReader.DataReader(SQL) = True Then

                If name = "" Then
                    ' ＣＳＶを作成する
                    name = PrnMeisai.CreateCsvFile()
                End If

                While OraReader.EOF = False
                    With PrnMeisai
                        .OutputCsvData(mMatchingDate)
                        .OutputCsvData(mMatchingTime)
                        .OutputCsvData(OraReader.GetString("TORIMATOME_SIT_T"))
                        .OutputCsvData(OraReader.GetString("TORIS_CODE_SE"))
                        .OutputCsvData(OraReader.GetString("TORIF_CODE_SE"))
                        .OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))
                        If OraReader.GetString("UKE_DATE_SE") = "" Then
                            ' 合計票なし
                            If Me.SYOUGOU.FSYORI_KBN = "1" Then
                                .OutputCsvData(JifuriBaitai.GetBaitaiCode(OraReader.GetString("IN_BAITAI_CODE_SE")))
                                .OutputCsvData(OraReader.GetString("SYUBETU_T"))
                                .OutputCsvData(JifuriSyubetu.GetBaitaiCode(OraReader.GetString("SYUBETU_T")))
                            Else
                                .OutputCsvData(SoufuriBaitai.GetBaitaiCode(OraReader.GetString("IN_BAITAI_CODE_SE")))
                                .OutputCsvData(OraReader.GetString("SYUBETU_T"))
                                .OutputCsvData(SoufuriSyubetu.GetBaitaiCode(OraReader.GetString("SYUBETU_T")))
                            End If
                        Else
                            ' データなし
                            If Me.SYOUGOU.FSYORI_KBN = "1" Then
                                .OutputCsvData(JifuriBaitai.GetBaitaiCode(OraReader.GetString("UKE_BAITAI_CODE_SE")))
                                .OutputCsvData(OraReader.GetString("SYUBETU_CODE_ME"))
                                .OutputCsvData(JifuriSyubetu.GetBaitaiCode(OraReader.GetString("SYUBETU_CODE_ME")))
                            Else
                                .OutputCsvData(SoufuriBaitai.GetBaitaiCode(OraReader.GetString("UKE_BAITAI_CODE_SE")))
                                .OutputCsvData(OraReader.GetString("SYUBETU_CODE_ME"))
                                .OutputCsvData(SoufuriSyubetu.GetBaitaiCode(OraReader.GetString("SYUBETU_CODE_ME")))
                            End If
                        End If
                        .OutputCsvData(OraReader.GetString("KANREN_KIGYO_CODE_T"))
                        .OutputCsvData(OraReader.GetString("ITAKU_CODE_T"))
                        .OutputCsvData(OraReader.GetString("FURI_DATE_SE"))
                        If OraReader.GetString("UKE_DATE_SE") = "" Then
                            ' 合計票なし
                            .OutputCsvData(OraReader.GetInt("IN_KEN_SE").ToString)
                            .OutputCsvData(OraReader.GetInt64("IN_KIN_SE").ToString)
                        Else
                            ' データなし
                            .OutputCsvData(OraReader.GetInt("UKE_KEN_SE").ToString)
                            .OutputCsvData(OraReader.GetInt64("UKE_KIN_SE").ToString)
                        End If
                        .OutputCsvData(OraReader.GetString("ERR_TEXT_SE"))
                        If OraReader.GetString("UKE_DATE_SE") = "" Then
                            ' 合計票なし
                            .OutputCsvData("")
                        Else
                            .OutputCsvData(OraReader.GetString("UKE_BAITAI_CODE_SE") & OraReader.GetString("STATION_ENTRY_NO_ME") & OraReader.GetInt("ENTRY_NO_ME").ToString.PadLeft(4, "0"c))
                        End If
                        .OutputCsvData("1,", False, True)

                    End With

                    ErrCount += 1

                    OraReader.NextRead()
                End While
            End If

            OraReader.Close()
            OraReader = Nothing

            If name <> "" Then
                If PrnMeisai.ReportExecute = False Then
                    BatchLog.Write("受付照合エラーリスト印刷", "失敗", PrnMeisai.ReportMessage)
                End If

                Message = "エラー件数：" & ErrCount.ToString & "件"
            Else

                With PrnMeisai
                    .CreateCsvFile()
                    .OutputCsvData(mMatchingDate)
                    .OutputCsvData(mMatchingTime)
                    .OutputCsvData(New String(","c, 5))
                    .OutputCsvData("")

                    If PrnMeisai.CheckSyougouTaisyou(Me.SYOUGOU.FSYORI_KBN) = True Then
                        ' 照合対象なしの場合
                        .OutputCsvData(New String(","c, 7) & "2", False, True)
                        Message = "照合対象なし"
                    Else
                        ' 照合対象ありで０件の場合
                        .OutputCsvData(New String(","c, 7) & "0", False, True)
                        Message = "照合エラーなし"
                    End If

                    .ReportExecute()

                End With
            End If

            Return True

        Catch ex As Exception
            Return False

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

    End Function

    ''' <summary>
    ''' 照合最終チェック
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function LastCheck() As Boolean
        Dim SQL As New StringBuilder
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            ' 送付状実績テーブル
            With SQL
                .Append("select")
                .Append("     count(*)")
                .Append(" from")
                .Append("     MEDIA_ENTRY_TBL")
                .Append(" where")
                .Append("     CHECK_FLG_ME = '8'")
                .Append(" and FURI_DATE_ME >= '" & mMatchingDate & "'")
                .Append(" and FSYORI_KBN_ME = '" & Me.SYOUGOU.FSYORI_KBN & "'")
            End With
            If OraReader.DataReader(SQL) = False Then
                BatchLog.Write("照合最終チェック　送付状実績テーブル", "失敗", MainDB.Message)
                Return False
            End If
            BatchLog.Write("照合最終チェック　送付状実績テーブル", "成功", "件数=" & OraReader.GetInt(0).ToString)
            OraReader.Close()

            ' スケジュールマスタ
            With SQL
                .Length = 0
                .Append("select")
                .Append("     count(*)")
                .Append(" from")
                .Append("     SCHMAST_SUB")
                .Append(" where")
                .Append("     SYOUGOU_FLG_S = '8'")
                .Append(" and SYOUGOU_DATE_S = '" & mMatchingDate & "'")
                .Append(" and FURI_DATE_SSUB >= '" & mMatchingDate & "'")
            End With

            If Me.SYOUGOU.FSYORI_KBN = "3" Then
                SQL.Replace("SCHMAST_SUB", "S_SCHMAST_SUB")
            End If
            If OraReader.DataReader(SQL) = False Then
                If Me.SYOUGOU.FSYORI_KBN = "3" Then
                    BatchLog.Write("照合最終チェック　総振スケジュールマスタ", "失敗", MainDB.Message)
                Else
                    BatchLog.Write("照合最終チェック　スケジュールマスタ", "失敗", MainDB.Message)
                End If
                Return False
            End If
            If Me.SYOUGOU.FSYORI_KBN = "3" Then
                BatchLog.Write("照合最終チェック　総振スケジュールマスタ", "成功", "件数=" & OraReader.GetInt(0).ToString)
            Else
                BatchLog.Write("照合最終チェック　スケジュールマスタ", "成功", "件数=" & OraReader.GetInt(0).ToString)
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write("照合最終チェック", "失敗", ex.ToString)
            Return False

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try
    End Function

#End Region

#Region "プライベートクラス"

    ''' <summary>
    ''' 受付照合エラーリスト印刷クラス
    ''' </summary>
    ''' <remarks></remarks>
    Private Class ClsPrnSyougouError
        Inherits CAstReports.ClsReportBase

#Region "パブリックメソッド"
        ''' <summary>
        ''' コンストラクタ
        ''' </summary>
        ''' <param name="PrintKbn"></param>
        ''' <remarks></remarks>
        Public Sub New(PrintKbn As Integer)
            If PrintKbn = 1 Then
                ' CSVファイルセット
                InfoReport.ReportName = "KFJP064"
                ' 定義体名セット
                ReportBaseName = "KFJP064_受付照合エラーリスト.rpd"
            Else
                ' CSVファイルセット
                InfoReport.ReportName = "KFSP037"
                ' 定義体名セット
                ReportBaseName = "KFSP037_受付照合エラーリスト.rpd"
            End If
        End Sub

        Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
            CSVObject.Output(data, dq, crlf)
        End Sub

        ''' <summary>
        ''' 照合対象の存在チェックを行います。
        ''' </summary>
        ''' <param name="fsyoriKbn">処理区分</param>
        ''' <returns>True - 正常 , False - 異常</returns>
        ''' <remarks></remarks>
        Public Function CheckSyougouTaisyou(fsyoriKbn As String) As Boolean
            Dim SQL As New StringBuilder
            Dim OraReader As CASTCommon.MyOracleReader = Nothing

            Try
                ' 送付状実績テーブル
                With SQL
                    .Append("select")
                    .Append("     CHECK_KBN_ME")
                    .Append(" where")
                    .Append("     UPLOAD_FLG_ME = '1'")
                    .Append(" and CHECK_KBN_ME = '1'")
                    .Append(" and FURI_DATE_ME >= '" & System.DateTime.Today.ToString("yyyyMMdd") & "'")
                    .Append(" and UPLOAD_DATE_ME = '" & System.DateTime.Today.ToString("yyyyMMdd") & "'")
                    .Append(" and FSYORI_KBN_ME = '" & fsyoriKbn & "'")
                End With
                OraReader = New CASTCommon.MyOracleReader(MainDB)
                If OraReader.DataReader(SQL) = True Then
                    Return False
                End If

                OraReader.Close()


                ' スケジュールマスタ
                With SQL
                    .Length = 0
                    .Append("select")
                    .Append("     SYOUGOU_KBN_T")
                    .Append(" from")
                    .Append("     SCHMAST")
                    .Append(" inner join")
                    .Append("     TORIMAST_VIEW")
                    .Append(" on  TORIS_CODE_S = TORIS_CODE_T")
                    .Append(" and TORIF_CODE_S = TORIF_CODE_T")
                    .Append(" where")
                    .Append("     UKETUKE_FLG_S = '1'")
                    .Append(" and TOUROKU_FLG_S = '0'")
                    .Append(" and TYUUDAN_FLG_S = '0'")
                    .Append(" and FURI_DATE_S >= '" & System.DateTime.Today.ToString("yyyyMMdd") & "'")
                    .Append(" and FSYORI_KBN_T = '" & fsyoriKbn & "'")
                    .Append(" and TOUROKU_DATE_S = " & System.DateTime.Today.ToString("yyyyMMdd") & "'")
                    .Append(" and SYOUGOU_KBN_T = '1'")
                End With

                If fsyoriKbn = "3" Then
                    SQL.Replace("SCHMAST", "S_SCHMAST").Replace("TORIMAST_VIEW", "S_TORIMAST_VIEW")
                End If
                If OraReader.DataReader(SQL) = True Then
                    Return False
                End If

                Return True

            Catch ex As Exception
                Return False

            Finally
                If Not OraReader Is Nothing Then
                    OraReader.Close()
                    OraReader = Nothing
                End If
            End Try
        End Function

#End Region

    End Class

#End Region

End Class
