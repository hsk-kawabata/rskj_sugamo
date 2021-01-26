Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFJP003

    ' 受付明細表
    Inherits CAstReports.ClsReportBase

    Public ToriSCode As String  '取引先主コード
    Public ToriFCode As String  '取引先副コード
    Public FuriDate As String   '振替日
    Public SyoriKbn As String   '処理区分（自振 or 総振）
    Public SortNo As String     'ソートコード

    Private DataOK As Boolean = False

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP003"

        ' 定義体名セット
        ReportBaseName = "KFJP003_受付明細表.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile()
        Return file

    End Function

    '
    ' 機能　 ： 受付明細表をデータに書き込む
    '
    ' 備考　 ： 
    '
    Public Function OutputCsvData() As Integer
        Dim oraDB As New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)

        Dim Mei_Kin_Name As String
        Dim Mei_Sit_Name As String

        Try
            '----------------------------------------------------
            'データ取得
            '----------------------------------------------------
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append(" SELECT ")
            SQL.Append(" ITAKU_NNAME_T")
            SQL.Append(",TKIN_NO_T")
            SQL.Append(",KIN_NNAME_N")
            SQL.Append(",TSIT_NO_T")
            SQL.Append(",SIT_NNAME_N")
            SQL.Append(",SYUBETU_T")
            SQL.Append(",CODE_KBN_T")
            SQL.Append(",FURI_CODE_T")
            SQL.Append(",KIGYO_CODE_T")
            SQL.Append(",YOBI5_K")
            SQL.Append(",KEIYAKU_KIN_K")
            SQL.Append(",KEIYAKU_SIT_K")
            SQL.Append(",KEIYAKU_KAMOKU_K")
            SQL.Append(",KEIYAKU_KOUZA_K")
            SQL.Append(",KEIYAKU_KNAME_K")
            SQL.Append(",FURIKIN_K")

            '2010/11/05 レコード番号追加 -----
            SQL.Append(",RECORD_NO_K")
            '---------------------------------

            If SyoriKbn = 1 Then
                SQL.Append(" FROM TORIMAST,MEIMAST,TENMAST")
            Else
                SQL.Append(" FROM K_TORIMAST,K_MEIMAST,TENMAST")
            End If
            SQL.Append(" WHERE TORIS_CODE_T = '" & ToriSCode & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & ToriFCode & "'")
            SQL.Append(" AND FURI_DATE_K = '" & FuriDate & "'")
            SQL.Append(" AND DATA_KBN_K = '2'")
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_K")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_K")
            SQL.Append(" AND TKIN_NO_T = KIN_NO_N")
            SQL.Append(" AND TSIT_NO_T = SIT_NO_N")

            ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- START
            'Select Case SortNo
            '    Case "0"    '店番ソート
            '        'ソートはＣＳＶファイル作成後に行う
            '    Case "1"    '非ソート
            '        '何も設定しない
            '    Case "2"    'エラー分のみ
            '        SQL.Append(" AND YOBI5_K <> ' '")
            'End Select
            Dim UketukeErrOut As String = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "KFSMAST010_受付明細表出力区分.TXT"), _
                                                                        SortNo, _
                                                                        2)
            Select Case UketukeErrOut.Trim
                Case "E"
                    SQL.Append(" AND YOBI5_K <> ' '")
            End Select
            ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- END

            SQL.Append(" ORDER BY RECORD_NO_K")     '2010/11/05 レコード番号順に取得する

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False

                    '----------------------------------------------------
                    'データ出力
                    '----------------------------------------------------
                    CSVObject.Output(FuriDate)                                      '振替日
                    CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '処理日
                    CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    'タイプスタンプ
                    CSVObject.Output(ToriSCode)                                     '取引先主コード
                    CSVObject.Output(ToriFCode)                                     '取引先副コード
                    CSVObject.Output(oraReader.GetItem("ITAKU_NNAME_T"), True)      '取引先名
                    CSVObject.Output(oraReader.GetItem("TKIN_NO_T"))                '取扱金融機関コード
                    CSVObject.Output(oraReader.GetItem("KIN_NNAME_N"), True)        '取扱金融機関名
                    CSVObject.Output(oraReader.GetItem("TSIT_NO_T"))                '取扱支店コード
                    CSVObject.Output(oraReader.GetItem("SIT_NNAME_N"), True)        '取扱支店名
                    CSVObject.Output(oraReader.GetItem("SYUBETU_T"))                '種別

                    Select Case oraReader.GetItem("CODE_KBN_T")                     'コード区分
                        Case "0"
                            CSVObject.Output("JIS")
                        Case "1"
                            CSVObject.Output("JIS改有(120)")
                        Case "2"
                            CSVObject.Output("JIS改有(119)")
                        Case "3"
                            CSVObject.Output("JIS改有(118)")
                        Case "4"
                            CSVObject.Output("EBCDIC")
                    End Select

                    CSVObject.Output(oraReader.GetItem("FURI_CODE_T"))              '振替コード
                    CSVObject.Output(oraReader.GetItem("KIGYO_CODE_T"))             '企業コード
                    CSVObject.Output(oraReader.GetItem("YOBI5_K"), True)            'エラー情報

                    '明細金融機関・店舗名取得
                    Mei_Kin_Name = ""
                    Mei_Sit_Name = ""
                    '2013/12/24 saitou 標準版 MODIFY ----------------------------------------------->>>>
                    '引数にオラクル接続を渡す
                    Call GetTENMAST(oraReader.GetItem("KEIYAKU_KIN_K"), oraReader.GetItem("KEIYAKU_SIT_K"), Mei_Kin_Name, Mei_Sit_Name, oraDB)
                    'Call GetTENMAST(oraReader.GetItem("KEIYAKU_KIN_K"), oraReader.GetItem("KEIYAKU_SIT_K"), Mei_Kin_Name, Mei_Sit_Name)
                    '2013/12/24 saitou 標準版 MODIFY -----------------------------------------------<<<<

                    CSVObject.Output(oraReader.GetItem("KEIYAKU_KIN_K"))            '金融機関コード
                    CSVObject.Output(Mei_Kin_Name, True)                            '金融機関名
                    CSVObject.Output(oraReader.GetItem("KEIYAKU_SIT_K"))            '支店コード
                    CSVObject.Output(Mei_Sit_Name, True)                            '支店名
                    CSVObject.Output(oraReader.GetItem("KEIYAKU_KAMOKU_K"))         '科目
                    CSVObject.Output(oraReader.GetItem("KEIYAKU_KOUZA_K"))          '口座番号
                    CSVObject.Output(oraReader.GetItem("KEIYAKU_KNAME_K"), True)    '預金者名
                    CSVObject.Output(oraReader.GetItem("FURIKIN_K"))                '金額
                    CSVObject.Output("", True)                                      '備考

                    '2010/11/05 レコード番号を追加（店別・レコードNo順ソート用） -------------------------------------------
                    '改ページ文字設定(店番ソートの場合は金融機関／支店コードを設定)
                    'If SortNo = "0" Then                                            '改ページキー(店番ソート)
                    '    CSVObject.Output(oraReader.GetItem("KEIYAKU_KIN_K") & _
                    '                     oraReader.GetItem("KEIYAKU_SIT_K"), False, True)
                    'Else
                    '    CSVObject.Output("0000000", False, True)                    '改ページキー(その他)
                    'End If

                    If SortNo = "1" Then                                            '改ページキー(店番ソート)
                        CSVObject.Output(oraReader.GetItem("KEIYAKU_KIN_K") & _
                                         oraReader.GetItem("KEIYAKU_SIT_K"))
                    Else
                        CSVObject.Output("0000000")                    '改ページキー(その他)
                    End If

                    CSVObject.Output(oraReader.GetItem("RECORD_NO_K").ToString.PadLeft(6), False, True)    'レコード番号出力
                    '-------------------------------------------------------------------------------------------------------

                    oraReader.NextRead()

                End While

                CSVObject.Close()

                ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- START
                'If SortNo = "0" Then
                '    Call SortData()
                'End If
                Dim SortParam As String = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "KFJMAST010_受付明細表出力区分.TXT"), _
                                                                        SortNo, _
                                                                        3)
                If SortParam.Trim <> "" Then
                    Call SortData(SortParam.Trim)
                End If
                ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- END

                '正常終了
                Return 0

            Else
                '印刷対象なし
                Return -1
            End If

        Catch ex As Exception
            BatchLog.Write("(受付明細表印刷)", "失敗", ex.Message)
            Return -300
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
            End If
        End Try
    End Function

    '==========================================================================
    '機　能：店舗マスタを取得する
    '引　数：kinno - 金融機関コード
    '      　sitno - 支店コード
    '戻り値：KIN_NAME - 金融機関名
    '        SIT_NAME - 支店名
    '備　考：
    '        2013/12/24 saitou 標準版 MODIFY 引数にオラクル接続追加
    '        2013/12/24 saitou 標準版 ADD    リーダーのクローズ追加
    '==========================================================================
    Public Sub GetTENMAST(ByVal kinno As String, ByVal sitno As String, ByRef KIN_NAME As String, ByRef SIT_NAME As String, _
                          ByVal oraDB As CASTCommon.MyOracle)
        '2013/12/24 saitou 標準版 MODIFY ----------------------------------------------->>>>
        'Dim oraDB As New CASTCommon.MyOracle
        '2013/12/24 saitou 標準版 MODIFY -----------------------------------------------<<<<
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(256)

        Try
            SQL.Append("SELECT")
            SQL.Append(" KIN_NNAME_N")
            SQL.Append(",SIT_NNAME_N")
            SQL.Append(" FROM TENMAST")
            SQL.Append(" WHERE KIN_NO_N = " & SQ(kinno))
            SQL.Append(" AND SIT_NO_N = " & SQ(sitno))

            oraReader = New CASTCommon.MyOracleReader(oraDB)

            If oraReader.DataReader(SQL) = True Then
                KIN_NAME = oraReader.GetItem("KIN_NNAME_N")
                SIT_NAME = oraReader.GetItem("SIT_NNAME_N")
            Else
                oraReader.Close()

                SQL.Length = 0
                SQL.Append("SELECT")
                SQL.Append(" KIN_NNAME_N")
                SQL.Append(" FROM TENMAST")
                SQL.Append(" WHERE KIN_NO_N = " & SQ(kinno))

                oraReader = New CASTCommon.MyOracleReader(oraDB)
                If oraReader.DataReader(SQL) = True Then
                    KIN_NAME = oraReader.GetItem("KIN_NNAME_N")
                    SIT_NAME = ""
                End If
            End If

        Catch ex As Exception
            KIN_NAME = ""
            SIT_NAME = ""
            BatchLog.Write("金融機関名取得", "失敗", ex.Message)
        Finally
            '2013/12/24 saitou 標準版 ADD -------------------------------------------------->>>>
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
            '2013/12/24 saitou 標準版 ADD --------------------------------------------------<<<<
        End Try
    End Sub

    Public Sub SortData(ByVal SortParam As String)
        '---------------------------------------------------------------------------------------
        'データソート処理
        '---------------------------------------------------------------------------------------
        Try
            ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- START
            ''ソート順：取引先主コード、取引先副コード、金融機関コード、支店コード
            ''2010/11/05 レコード番号順を追加
            'Call SortFile("3.10sjia 4.2sjia 15.4sjia 17.3sjia 25.6sjia")
            ''Call SortFile("3.10sjia 4.2sjia 15.4sjia 17.3sjia")
            Call SortFile(SortParam)
            ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- END
        Catch ex As Exception
            BatchLog.Write("(受付明細表印刷)ソート", "失敗", ex.Message)
        Finally
        End Try
    End Sub

    '
    ' 機能　 ： 印刷実行
    '
    ' 備考　 ： 
    '
    Public Overloads Overrides Function ReportExecute() As Boolean
        Try
            MyBase.CloseCsv()
            Return MyBase.ReportExecute()
        Catch ex As System.Exception
            BatchLog.Write("(受付明細表印刷)印刷実行", "失敗", ex.Message)
        End Try
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
