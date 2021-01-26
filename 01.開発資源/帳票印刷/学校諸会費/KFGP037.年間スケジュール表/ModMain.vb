Imports System
Imports System.Text
Imports CASTCommon

''' <summary>
''' 年間スケジュール表印刷　メインモジュール
''' </summary>
''' <remarks>2017/04/28 saitou RSV2 added for 標準機能追加(年間スケジュール表)</remarks>
Public Module ModMain

#Region "モジュール変数"
    'ログ
    Public BatchLog As New CASTCommon.BatchLOG("KFGP037", "年間スケジュール表")
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Public LW As LogWrite

    Public RecordCnt As Integer = 0

#End Region

#Region "パブリックメソッド"

    Public Function Main(ByVal CmdArgs() As String) As Integer

        Dim dbConn As CASTCommon.MyOracle = Nothing
        Dim MainReader As CASTCommon.MyOracleReader = Nothing
        Dim SubReader As CASTCommon.MyOracleReader = Nothing

        Try
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)開始", "成功", "")

            '----------------------------------------
            'パラメータチェック
            '----------------------------------------
            If CmdArgs.Length = 0 Then
                'パラメータ取得失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数なし")
                Return -100
            End If

            Dim Cmd() As String = String.Join(",", CmdArgs).Split(","c)
            If Cmd.Length <> 3 Then
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            LW.UserID = Cmd(0)
            LW.ToriCode = Cmd(1)
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))

            '----------------------------------------
            '印刷処理
            '----------------------------------------
            dbConn = New CASTCommon.MyOracle
            MainReader = New CASTCommon.MyOracleReader(dbConn)

            Dim List As New KFGP037
            List.CreateCsvFile()
            List.OraDB = dbConn

            If MainReader.DataReader(CreateListGakkouSQL(Cmd(1), Cmd(2))) = True Then
                While MainReader.EOF = False
                    List.GAKKOU_CODE = MainReader.GetString("GAKKOU_CODE_S")
                    List.NENDO = Cmd(2)
                    LW.ToriCode = MainReader.GetString("GAKKOU_CODE_S")

                    '共通部設定
                    If List.MakeRecordCommon() = False Then
                        If RecordCnt = -1 Then
                            Return -1
                        Else
                            Return -300
                        End If
                    End If

                    '年間スケジュール設定
                    If List.MakeRecordNenkan() = False Then
                        If RecordCnt = -1 Then
                            Return -1
                        Else
                            Return -300
                        End If
                    End If

                    '特別スケジュール
                    If List.MakeRecordSpecial() = False Then
                        If RecordCnt = -1 Then
                            Return -1
                        Else
                            Return -300
                        End If
                    End If

                    '随時スケジュール
                    If List.MakeRecordAnyTime() = False Then
                        If RecordCnt = -1 Then
                            Return -1
                        Else
                            Return -300
                        End If
                    End If

                    '印刷用ＣＳＶの設定
                    If List.MakeRecord() = False Then
                        Return False
                    End If

                    MainReader.NextRead()
                End While
            End If

            If List.G_ReportExecute("") = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", List.ReportMessage)
                Return -999
            End If

            Return 0

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            Return -9999
        Finally
            If Not MainReader Is Nothing Then
                MainReader.Close()
                MainReader = Nothing
            End If

            If Not SubReader Is Nothing Then
                SubReader.Close()
                SubReader = Nothing
            End If

            If Not dbConn Is Nothing Then
                dbConn.Close()
                dbConn = Nothing
            End If

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", "")
        End Try
    End Function

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 印刷対象の学校コードを取得するSQLを作成します。
    ''' </summary>
    ''' <param name="GakkouCode">学校コード</param>
    ''' <param name="Nendo">年度</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateListGakkouSQL(ByVal GakkouCode As String, _
                                         ByVal Nendo As String) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select")
            .Append("     GAKKOU_CODE_S")
            .Append(" from")
            .Append("     G_SCHMAST")
            .Append(" inner join")
            .Append("     GAKMAST2")
            .Append(" on  GAKKOU_CODE_S = GAKKOU_CODE_T")
            .Append(" where")
            .Append("     NENGETUDO_S")
            .Append(" between")
            .Append("     " & SQ(Nendo & "04"))
            .Append(" and " & SQ(CStr(CInt(Nendo) + 1) & "03"))
            If GakkouCode <> "9999999999" Then
                .Append(" and GAKKOU_CODE_S = " & SQ(GakkouCode))
            End If
            .Append(" group by")
            .Append("     GAKKOU_CODE_S")
            .Append(" order by ")
            .Append("     GAKKOU_CODE_S")
        End With

        Return SQL

    End Function

    ''' <summary>
    ''' 印刷対象のスケジュールが存在するかチェックするSQLを作成します。
    ''' </summary>
    ''' <param name="GakkouCode">学校コード</param>
    ''' <param name="Nendo">年度</param>
    ''' <param name="SchKbn">スケジュール区分(0 - 年間 , 1 - 特別 , 2 - 随時)</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateScheduleCheckSQL(ByVal GakkouCode As String, _
                                            ByVal Nendo As String, _
                                            ByVal SchKbn As String) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select")
            .Append("     GAKKOU_CODE_S")
            .Append(" from")
            .Append("     G_SCHMAST")
            .Append(" inner join")
            .Append("     GAKMAST2")
            .Append(" on  GAKKOU_CODE_S = GAKKOU_CODE_T")
            .Append(" where")
            .Append("     NENGETUDO_S")
            .Append(" between")
            .Append("     " & SQ(Nendo & "04"))
            .Append(" and " & SQ(CStr(CInt(Nendo) + 1) & "03"))
            .Append(" and GAKKOU_CODE_S = " & SQ(GakkouCode))
            .Append(" and SCH_KBN_S = " & SQ(SchKbn))
        End With

        Return SQL

    End Function

#End Region

End Module
