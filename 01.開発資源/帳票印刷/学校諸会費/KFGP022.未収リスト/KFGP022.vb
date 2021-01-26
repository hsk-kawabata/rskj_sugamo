Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP022

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP022"

        ' 定義体名セット
        ReportBaseName = "KFGP022_未収リスト.rpd"
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
        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT")
            SQL.Append(" G_MEIMAST.GAKKOU_CODE_M")
            SQL.Append(",G_MEIMAST.NENDO_M ")
            SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M ")
            SQL.Append(",G_MEIMAST.CLASS_CODE_M ")
            SQL.Append(",G_MEIMAST.SEITO_NO_M ")
            SQL.Append(",G_MEIMAST.TUUBAN_M ")
            SQL.Append(",G_MEIMAST.TKIN_NO_M ")
            SQL.Append(",G_MEIMAST.TSIT_NO_M ")
            SQL.Append(",G_MEIMAST.TKAMOKU_M ")
            SQL.Append(",G_MEIMAST.TKOUZA_M ")
            SQL.Append(",G_MEIMAST.TMEIGI_KNM_M ")
            SQL.Append(",G_MEIMAST.SEIKYU_TUKI_M ")
            SQL.Append(",G_MEIMAST.SEIKYU_KIN_M ")

            SQL.Append(",SEITOMAST.GAKKOU_CODE_O")
            SQL.Append(",SEITOMAST.NENDO_O ")
            SQL.Append(",SEITOMAST.TUUBAN_O ")
            SQL.Append(",SEITOMAST.SEITO_NO_O ")
            SQL.Append(",SEITOMAST.SEITO_KNAME_O ")
            SQL.Append(",SEITOMAST.SEITO_NNAME_O ")
            SQL.Append(",SEITOMAST.SEIBETU_O ")
            SQL.Append(",SEITOMAST.MEIGI_KNAME_O ")
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",GAKMAST1.GAKUNEN_NAME_G")
            SQL.Append(",TENMAST.KIN_NO_N")
            SQL.Append(",TENMAST.SIT_NO_N")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",GAKMAST2.FURI_CODE_T")
            SQL.Append(",GAKMAST2.KIGYO_CODE_T")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

            SQL.Append(" FROM ")
            SQL.Append(" KZFMAST.G_MEIMAST")
            SQL.Append(",KZFMAST.SEITOMAST")
            SQL.Append(",KZFMAST.GAKMAST1")
            SQL.Append(",KZFMAST.TENMAST")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",KZFMAST.GAKMAST2")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

            SQL.Append(" WHERE G_MEIMAST.GAKKOU_CODE_M  = SEITOMAST.GAKKOU_CODE_O")
            SQL.Append(" AND G_MEIMAST.NENDO_M = SEITOMAST.NENDO_O")
            SQL.Append(" AND G_MEIMAST.TUUBAN_M = SEITOMAST.TUUBAN_O")
            SQL.Append(" AND G_MEIMAST.HIMOKU_ID_M = SEITOMAST.HIMOKU_ID_O")

            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M = GAKMAST1.GAKKOU_CODE_G")

            SQL.Append(" AND G_MEIMAST.TKIN_NO_M = TENMAST.KIN_NO_N")
            SQL.Append(" AND G_MEIMAST.TSIT_NO_M = TENMAST.SIT_NO_N")

            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            ' 結合条件：GAKMAST1.GAKKOU_CODE_G = GAKMAST2.GAKKOU_CODE_T
            SQL.Append(" AND GAKMAST2.GAKKOU_CODE_T = GAKMAST1.GAKKOU_CODE_G")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

            SQL.Append(" AND GAKMAST1.GAKUNEN_CODE_G = 1")

            SQL.Append(" AND SEITOMAST.TUKI_NO_O ='04'")

            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M = " & SQ(GakkouCode))

            SQL.Append(" AND G_MEIMAST.FURIKETU_CODE_M <> 0")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M = " & SQ(FURI_DATE))
            SQL.Append(" AND (G_MEIMAST.FURI_KBN_M = '0' OR G_MEIMAST.FURI_KBN_M = '1')")

            'ソート条件
            SQL.Append(" ORDER BY ")
            SQL.Append(" G_MEIMAST.GAKKOU_CODE_M")
            SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M")
            SQL.Append(",G_MEIMAST.CLASS_CODE_M")

            '生徒番号順
            If PrintSort_Seito = "1" Then
                SQL.Append(",G_MEIMAST.SEITO_NO_M")
            End If
            '年度・通番順
            If PrintSort_Tuuban = "1" Then
                SQL.Append(",G_MEIMAST.NENDO_M")
                SQL.Append(",G_MEIMAST.TUUBAN_M")
            End If
            'あいうえお順
            If PrintSort_Waon = "1" Then
                SQL.Append(",SEITOMAST.SEITO_KNAME_O")
            End If
            '性別順
            If PrintSort_Seibetu = "1" Then
                SQL.Append(",SEITOMAST.SEIBETU_O")
            End If

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ

                   
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_M")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_M")))        '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_M")))          'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_O")))            '生徒番号
                    If GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")).Trim = "" Then      '生徒名
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")), True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_M")))             '金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_M")))             '支店コード
                    Select Case oraReader.GetString("TKAMOKU_M")                             '科目
                        Case "02"
                            OutputCsvData("普通")
                        Case "01"
                            OutputCsvData("当座")
                        Case "09"
                            OutputCsvData("その他")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKOUZA_M")))               '振替口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("MEIGI_KNAME_O")), True)         '預金者カナ名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEIKYU_KIN_M")))             '振替金額
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEIKYU_TUKI_M")))             '請求年月
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENDO_O")))               '年度
                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
                    ' 振替コード、企業コードを追加（【通番】は最終項目でなくなったため改行を削除）
                    'OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_O")), False, True) '通番
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_O")))                      '通番
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")))                   '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), False, True)     '企業コード
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
