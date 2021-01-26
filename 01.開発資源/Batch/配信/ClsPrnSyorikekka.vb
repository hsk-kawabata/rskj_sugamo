Imports System.Globalization
Imports System.Text

' 処理結果確認表
Class ClsPrnSyorikekka
    Inherits CAstReports.ClsReportBase

    Public CsvData(15) As String
    Public CENTER As String

    Sub New(ByVal strCenter As String)
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP008"

        ' 定義体名セット
        '2010/12/24 信組対応 信組の場合は帳票定義変更
        If strCenter = "0" Then
            ReportBaseName = "KFJP008_処理結果確認表(配信データ作成・組合持込).rpd"
        Else
            ReportBaseName = "KFJP008_処理結果確認表(配信データ作成).rpd"
        End If

        'If gstrCENTER = "0" Then
        '    ReportBaseName = "KFJP008_処理結果確認表(配信データ作成・組合持込).rpd"
        'Else
        '    ReportBaseName = "KFJP008_処理結果確認表(配信データ作成).rpd"
        'End If

        ' 処理日
        CsvData(0) = "00010101"                         '処理日
        CsvData(1) = "00010101"                         'タイムスタンプ
        CsvData(2) = "00010101"                         ' 振替日
        CsvData(3) = "0"                                ' 取引先主コード
        CsvData(4) = "0"                                ' 取引先副コード
        CsvData(5) = ""                                 ' 取引先名
        CsvData(6) = "0"                                ' 委託者コード
        CsvData(7) = ""                                 ' 入出金区分
        CsvData(8) = "0"                                ' 振替コード
        CsvData(9) = "0"                                ' 企業コード
        CsvData(10) = "0"                                ' 依頼件数
        CsvData(11) = "0"                                ' 依頼金額
        CsvData(12) = "0"                                ' 処理件数
        CsvData(13) = "0"                                ' 処理金額
        CsvData(14) = ""                                ' 備考
        CsvData(15) = "0"                                ' 伝送合計

        CENTER = strCenter

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' タイトル行
        CSVObject.Output("処理日")
        CSVObject.Output("タイムスタンプ")
        CSVObject.Output("振替日")
        CSVObject.Output("取引先主コード")
        CSVObject.Output("取引先副コード")
        CSVObject.Output("取引先名")
        CSVObject.Output("委託者コード")
        CSVObject.Output("入出金区分")
        CSVObject.Output("振替コード")
        CSVObject.Output("企業コード")
        CSVObject.Output("依頼件数")
        CSVObject.Output("依頼金額")
        CSVObject.Output("処理件数")
        CSVObject.Output("処理金額")
        CSVObject.Output("備考")
        CSVObject.Output("伝送合計", False, True)

        Return file

    End Function

    '
    ' 機能　 ： 処理結果確認表ＣＳＶを出力する
    '
    ' 備考　 ： 
    '
    Public Function OutputCSVKekka(ByVal ary As ArrayList, ByVal densoKen As Integer) As Boolean

        Dim oraDB As New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim sql As StringBuilder

        Try

            Dim i As Integer = 0

            For Each item As String() In ary

                Dim strTakoKbn As String
                '2013/09/20 saitou 蒲郡信金 MODIFY ----------------------------------------------->>>>
                Dim strFmtKbn As String = String.Empty
                '2013/09/20 saitou 蒲郡信金 MODIFY -----------------------------------------------<<<<

                oraReader = New CASTCommon.MyOracleReader(oraDB)
                sql = New StringBuilder(128)
                ''-----------------------------------------
                ''対象スケジュール検索
                ''-----------------------------------------
                '2013/09/20 saitou 蒲郡信金 MODIFY ----------------------------------------------->>>>
                'スリーエス用にフォーマット区分も取得する
                sql.Append("SELECT TAKO_KBN_T, FMT_KBN_T FROM SCHMAST,TORIMAST WHERE FURI_DATE_S = '" & item(2) & "'")
                'sql.Append("SELECT TAKO_KBN_T FROM SCHMAST,TORIMAST WHERE FURI_DATE_S = '" & item(2) & "'")
                '2013/09/20 saitou 蒲郡信金 MODIFY -----------------------------------------------<<<<
                sql.Append(" AND TORIS_CODE_S = '" & item(3) & "' AND TORIF_CODE_S = '" & item(4) & "'")
                sql.Append(" AND TORIS_CODE_S = TORIS_CODE_T AND TORIF_CODE_S = TORIF_CODE_T")

                If oraReader.DataReader(sql) = True Then
                    strTakoKbn = oraReader.GetString("TAKO_KBN_T")
                    '2013/09/20 saitou 蒲郡信金 MODIFY ----------------------------------------------->>>>
                    strFmtKbn = oraReader.GetString("FMT_KBN_T")
                    '2013/09/20 saitou 蒲郡信金 MODIFY -----------------------------------------------<<<<
                Else
                    MainLOG.Write("処理結果確認表CSV作成", "失敗", "他行区分取得")
                    Return False
                End If

                oraReader.Close()

                oraReader = New CASTCommon.MyOracleReader(oraDB)
                sql = New StringBuilder(128)

                ''-----------------------------------------
                ''他行データあり
                ''-----------------------------------------
                Dim takoken As Long
                Dim takokin As Long

                '2017/01/20 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                '標準版(スリーエス決済無し)でも他行スケジュールマスタを作成するが、未作成の条件は蒲郡版で問題なし。
                '今後のために、フォーマット区分21(内外)を条件に追加しておく。
                If strFmtKbn.Equals("20") = True OrElse strFmtKbn.Equals("21") = True Then
                    ''2013/09/20 saitou 蒲郡信金 MODIFY ----------------------------------------------->>>>
                    ''スリーエス委託者が含まれている際に、「未作成あり」と表示されてしまうのを回避
                    ''蒲郡のスリーエスは他行スケジュールが存在しないため、処理件数と依頼件数が絶対に合わない
                    'If strFmtKbn.Equals("20") = True Then
                    '2017/01/20 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END

                    With sql
                        .Append("select TAKOU_FLG_S from SCHMAST")
                        .Append(" where TORIS_CODE_S = '" & item(3) & "'")
                        .Append(" and TORIF_CODE_S = '" & item(4) & "'")
                        .Append(" and FURI_DATE_S = '" & item(2) & "'")
                    End With

                    Dim strTakouFlg As String = String.Empty

                    If oraReader.DataReader(sql) = True Then
                        strTakouFlg = oraReader.GetString("TAKOU_FLG_S")
                    Else
                        'スケジュール取得失敗（この時点でスケジュール無しはおかしい）
                        MainLOG.Write("処理結果確認表CSV作成", "失敗", "ＳＳＳスケジュール無し")
                        Return False
                    End If

                    oraReader.Close()

                    If strTakouFlg.Equals("0") = True Then
                        'スリーエス委託者はＳＳＳ他行データを作成したら他行フラグが1になるため
                        'スリーエスで本当に未作成なのは、スリーエス委託者なのに他行データを作成していないとき
                        item(14) = "ＳＳＳ未作成あり"
                    ElseIf strTakouFlg.Equals("1") = True Then
                        item(14) = "ＳＳＳあり"
                    Else
                        item(14) = ""
                    End If

                Else
                    'スリーエス以外は既存の処理
                    If strTakoKbn = 1 Then '他行作成対象
                        sql.Append("SELECT SUM(SYORI_KEN_U) AS GET_SYORI_KEN,SUM(SYORI_KIN_U) AS GET_SYORI_KIN FROM TAKOSCHMAST")
                        sql.Append(" WHERE TORIS_CODE_U = '" & item(3) & "' AND TORIF_CODE_U = '" & item(4) & "' AND FURI_DATE_U ='" & item(2) & "'")

                        If oraReader.DataReader(sql) = True Then
                            takoken = oraReader.GetInt64("GET_SYORI_KEN")
                            takokin = oraReader.GetInt64("GET_SYORI_KIN")
                        Else
                            MainLOG.Write("処理結果確認表CSV作成", "失敗", "他行処理件数取得")
                            Return False
                        End If

                        oraReader.Close()

                        '備考の設定
                        If takoken > 0 Then '他行作成対象で他行作成済み
                            If item(10) <> (item(12) + takoken) Then '金額0円か振替結果に不能コードが設定されているものが存在する
                                item(14) = "他行未作成あり"
                            Else
                                item(14) = "他行あり"
                            End If
                        Else                '他行作成対象で他行未作成
                            If item(10) <> (item(12) + takoken) Then '金額0円か振替結果に不能コードが設定されているものが存在する
                                item(14) = "未作成あり"
                            End If
                        End If
                    Else
                        If item(10) <> item(12) Then                '金額0円か振替結果に不能コードが設定されているものが存在する
                            item(14) = "未作成あり"
                        End If
                    End If

                End If

                'If strTakoKbn = 1 Then '他行作成対象
                '    sql.Append("SELECT SUM(SYORI_KEN_U) AS GET_SYORI_KEN,SUM(SYORI_KIN_U) AS GET_SYORI_KIN FROM TAKOSCHMAST")
                '    sql.Append(" WHERE TORIS_CODE_U = '" & item(3) & "' AND TORIF_CODE_U = '" & item(4) & "' AND FURI_DATE_U ='" & item(2) & "'")

                '    If oraReader.DataReader(sql) = True Then
                '        takoken = oraReader.GetInt64("GET_SYORI_KEN")
                '        takokin = oraReader.GetInt64("GET_SYORI_KIN")
                '    Else
                '        MainLOG.Write("処理結果確認表CSV作成", "失敗", "他行処理件数取得")
                '        Return False
                '    End If

                '    oraReader.Close()

                '    '備考の設定
                '    If takoken > 0 Then '他行作成対象で他行作成済み
                '        If item(10) <> (item(12) + takoken) Then '金額0円か振替結果に不能コードが設定されているものが存在する
                '            item(14) = "他行未作成あり"
                '        Else
                '            item(14) = "他行あり"
                '        End If
                '    Else                '他行作成対象で他行未作成
                '        If item(10) <> (item(12) + takoken) Then '金額0円か振替結果に不能コードが設定されているものが存在する
                '            item(14) = "未作成あり"
                '        End If
                '    End If
                'Else
                '    If item(10) <> item(12) Then                '金額0円か振替結果に不能コードが設定されているものが存在する
                '        item(14) = "未作成あり"
                '    End If
                'End If
                '2013/09/20 saitou 蒲郡信金 MODIFY -----------------------------------------------<<<<

                CsvData(0) = item(0)
                CsvData(1) = item(1)
                CsvData(2) = item(2)
                CsvData(3) = item(3)
                CsvData(4) = item(4)
                CsvData(5) = item(5)
                CsvData(6) = item(6)
                Select Case item(7)
                    Case "1"
                        CsvData(7) = "入金"
                    Case "9"
                        CsvData(7) = "出金"
                End Select
                CsvData(8) = item(8)
                CsvData(9) = item(9)
                CsvData(10) = item(10)
                CsvData(11) = item(11)
                CsvData(12) = item(12)
                CsvData(13) = item(13)
                CsvData(14) = item(14)
                CsvData(15) = densoKen      '伝送件数
                '2010/12/24 信組対応 信組の場合は送信区分を項目に追加
                If CENTER = "0" Then
                    ' 2015/12/14 タスク）綾部 CHG 【PG】UI_B-14-04(RSV2対応) -------------------- START
                    'ReDim Preserve CsvData(16)
                    'CsvData(16) = item(15)
                    ReDim Preserve CsvData(19)
                    CsvData(16) = item(15)
                    CsvData(17) = item(16)
                    CsvData(18) = item(17)
                    CsvData(19) = item(18)
                    ' 2015/12/14 タスク）綾部 CHG 【PG】UI_B-14-04(RSV2対応) -------------------- END
                End If
                'If gstrCENTER = "0" Then
                '    ReDim Preserve CsvData(16)
                '    CsvData(16) = item(15)
                'End If

                CSVObject.Output(CsvData)
            Next

            Return True

        Catch ex As Exception
            MainLOG.Write("処理結果確認表CSV作成", "失敗", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not oraDB Is Nothing Then oraDB.Close()
        End Try
    End Function

    'Public Overloads Overrides Function ReportExecute() As Boolean
    '    Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
    'End Function
End Class
