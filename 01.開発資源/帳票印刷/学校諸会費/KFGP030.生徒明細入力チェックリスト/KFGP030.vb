Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP030

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP030"

        ' 定義体名セット
        ReportBaseName = "KFGP030_生徒明細入力チェックリスト.rpd"
    End Sub
    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' 機能　 ： CSVファイルに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    '
    ' 機能　 ： 印刷データの作成
    '
    ' 備考　 ： 
    '
    Public Function MakeRecord() As Boolean
        Dim oraDB As New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim JIKINKO As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT ")
            SQL.Append(" TENMAST.KIN_NNAME_N")
            SQL.Append(",TENMAST.SIT_NNAME_N")
            SQL.Append(",GAKMAST2.TKIN_NO_T")
            SQL.Append(",GAKMAST2.TSIT_NO_T")
            SQL.Append(",G_SCHMAST.DATA_FLG_S")
            SQL.Append(",G_ENTMAST.GAKKOU_CODE_E")
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",G_ENTMAST.KEIYAKU_NO_E")
            SQL.Append(",G_ENTMAST.TKIN_NO_E")
            SQL.Append(",G_ENTMAST.TSIT_NO_E")
            SQL.Append(",G_ENTMAST.KAMOKU_E")
            SQL.Append(",G_ENTMAST.KOUZA_E")
            SQL.Append(",G_ENTMAST.KEIYAKU_KNAME_E")
            SQL.Append(",G_ENTMAST.FURIKIN_E")
            SQL.Append(",G_ENTMAST.SEITO_KNAME_E")
            SQL.Append(",G_ENTMAST.SEITO_NNAME_E")
            SQL.Append(",G_ENTMAST.FURI_DATE_E")
            SQL.Append(",G_ENTMAST.GAKUNEN_CODE_E")
            SQL.Append(",G_ENTMAST.CLASS_CODE_E")
            SQL.Append(",G_ENTMAST.SEITO_NO_E")
            SQL.Append(",G_ENTMAST.NENDO_E")
            SQL.Append(",G_ENTMAST.TUUBAN_E")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",GAKMAST2.FURI_CODE_T")     '学校マスタ2.振替コード
            SQL.Append(",GAKMAST2.KIGYO_CODE_T")    '学校マスタ2.企業コード
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<
            SQL.Append(" FROM ")
            SQL.Append(" KZFMAST.G_ENTMAST")
            SQL.Append(",KZFMAST.GAKMAST1")
            SQL.Append(",KZFMAST.GAKMAST2")
            SQL.Append(",KZFMAST.G_SCHMAST")
            SQL.Append(",KZFMAST.TENMAST")
            SQL.Append(",KZFMAST.SEITOMAST")

            SQL.Append(" WHERE G_ENTMAST.GAKKOU_CODE_E = GAKMAST1.GAKKOU_CODE_G(+)")
            SQL.Append(" AND G_ENTMAST.GAKUNEN_CODE_E = GAKMAST1.GAKUNEN_CODE_G(+)")
            SQL.Append(" AND G_ENTMAST.GAKKOU_CODE_E = GAKMAST2.GAKKOU_CODE_T(+)")
            SQL.Append(" AND G_ENTMAST.GAKKOU_CODE_E = G_SCHMAST.GAKKOU_CODE_S(+)")
            SQL.Append(" AND G_ENTMAST.SYORI_NENGETU_E = G_SCHMAST.NENGETUDO_S(+)")
            SQL.Append(" AND G_ENTMAST.FURI_DATE_E = G_SCHMAST.FURI_DATE_S(+)")
            SQL.Append(" AND G_ENTMAST.GAKKOU_CODE_E = SEITOMAST.GAKKOU_CODE_O(+)")
            SQL.Append(" AND G_ENTMAST.NENDO_E = SEITOMAST.NENDO_O(+)")
            SQL.Append(" AND G_ENTMAST.TUUBAN_E = SEITOMAST.TUUBAN_O(+)")
            SQL.Append(" AND SUBSTR(G_ENTMAST.FURI_DATE_E,5,2) = SEITOMAST.TUKI_NO_O(+)")
            SQL.Append(" AND G_ENTMAST.TKIN_NO_E = TENMAST.KIN_NO_N(+)")
            SQL.Append(" AND G_ENTMAST.TSIT_NO_E = TENMAST.SIT_NO_N(+)")

            If GakkouCode <> "9999999999" Then
                SQL.Append(" AND G_ENTMAST.GAKKOU_CODE_E = " & SQ(GakkouCode))
            End If
            SQL.Append(" AND G_ENTMAST.FURI_DATE_E = " & SQ(FuriDate))
            SQL.Append(" AND G_ENTMAST.FURIKIN_E <> 0")
            Select Case FuriKbn
                Case "1"
                    SQL.Append(" AND G_SCHMAST.FURI_KBN_S = '2'")
                Case "2"
                    SQL.Append(" AND G_SCHMAST.FURI_KBN_S = '3'")
            End Select
            '振替方法
            SQL.Append(" AND SEITOMAST.FURIKAE_O = '0'")
            '解約区分
            SQL.Append(" AND SEITOMAST.KAIYAKU_FLG_O <> '9'")

            'ソート条件
            SQL.Append(" ORDER BY G_ENTMAST.GAKKOU_CODE_E")
            Select Case PrintSort
                Case "0"
                    SQL.Append(",G_ENTMAST.GAKUNEN_CODE_E")
                    SQL.Append(",G_ENTMAST.CLASS_CODE_E")
                    SQL.Append(",G_ENTMAST.SEITO_NO_E")
                    SQL.Append(",G_ENTMAST.NENDO_E")
                    SQL.Append(",G_ENTMAST.TUUBAN_E")
                Case "1"
                    SQL.Append(",G_ENTMAST.GAKUNEN_CODE_E")
                    SQL.Append(",G_ENTMAST.NENDO_E")
                    SQL.Append(",G_ENTMAST.TUUBAN_E")
                Case Else
                    SQL.Append(",G_ENTMAST.GAKUNEN_CODE_E")
                    SQL.Append(",G_ENTMAST.SEITO_KNAME_E")
                    SQL.Append(",G_ENTMAST.NENDO_E")
                    SQL.Append(",G_ENTMAST.TUUBAN_E")
            End Select

            Select Case FuriKbn
                Case "1"
                    SQL.Replace("G_ENTMAST", "G_ENTMAST1")
                Case "2"
                    SQL.Replace("G_ENTMAST", "G_ENTMAST2")
            End Select

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    Select Case GCOM.NzStr(oraReader.GetString("DATA_FLG_S"))               '作成済表示
                        Case "1"
                            OutputCsvData("振替データ作成済み")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_T")))             '取扱金融機関
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_T")))             '取扱支店
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_E")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名漢字
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_E")))           '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_E")))        '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_E")))          'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_E")))            '生徒番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_E")))             '振替金融機関
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_E")))             '振替支店
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")))           '振替支店名
                    Select Case oraReader.GetString("KAMOKU_E")                             '科目
                        Case "02"
                            OutputCsvData("02 普通")
                        Case "01"
                            OutputCsvData("01 当座")
                        Case "09"
                            OutputCsvData("09 その他")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_E")))               '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_KNAME_E")), True) '契約者名
                    OutputCsvData(GCOM.NzStr(oraReader.GetInt64("FURIKIN_E")))              '振替金額
                    If GCOM.NzStr(oraReader.GetString("SEITO_NNAME_E")).Trim = "" Then      '生徒名
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_E")), True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NNAME_E")), True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENDO_E")))               '年度
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_E")))              '通番
                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
                    ' 振替コード、企業コードを追加（【自金庫コード】は最終項目でなくなったため改行を削除）
                    'OutputCsvData(JIKINKO, False, True)                                     '自金庫コード
                    OutputCsvData(JIKINKO)                                                      '自金庫コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")))               '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), False, True) '企業コード
                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

                    oraReader.NextRead()
                    RecordCnt += 1
                End While
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = -1
                Return False
            End If
            Return True
        Catch ex As Exception
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function
End Class
