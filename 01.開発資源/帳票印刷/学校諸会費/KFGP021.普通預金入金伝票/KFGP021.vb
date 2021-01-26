Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Imports Microsoft.VisualBasic
Public Class KFGP021

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP021"

        ' 定義体名セット
        ReportBaseName = "KFGP021_普通預金入金伝票.rpd"
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

        ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- START
        Dim SFuriCode As String = String.Empty
        ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- END

        Try

            ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- START
            SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SFURI_CODE")
            If SFuriCode = "err" OrElse SFuriCode = "" Then
                SFuriCode = "1,2,3,4,5,6,7,8,9"
            End If
            ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- END

            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT ")
            SQL.Append(" G_MEIMAST.GAKKOU_CODE_M")
            SQL.Append(",G_MEIMAST.CLASS_CODE_M")
            SQL.Append(",G_MEIMAST.SEITO_NO_M")
            SQL.Append(",G_MEIMAST.SEIKYU_TAISYOU_M")
            SQL.Append(",G_MEIMAST.SEIKYU_KIN_M")
            SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M")
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",GAKMAST2.KOUZA_T")
            SQL.Append(",GAKMAST2.JIFURI_KBN_T")
            SQL.Append(",SEITOMAST.SEITO_NNAME_O")
            SQL.Append(",SEITOMAST.SEITO_KNAME_O")
            SQL.Append(",TENMAST.KIN_NNAME_N")
            SQL.Append(",TENMAST.SIT_NNAME_N")
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
            SQL.Append(",GAKMAST2.YOBI1_T")             'ユーザID
            SQL.Append(",GAKMAST2.BAITAI_CODE_T")       '媒体コード 
            SQL.Append(",GAKMAST2.ITAKU_CODE_T")        '委託者コード
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
            SQL.Append(" FROM")
            SQL.Append(" KZFMAST.G_MEIMAST")
            SQL.Append(",KZFMAST.GAKMAST1")
            SQL.Append(",KZFMAST.GAKMAST2")
            SQL.Append(",KZFMAST.SEITOMAST")
            SQL.Append(",KZFMAST.TENMAST")

            SQL.Append(" WHERE ")
            SQL.Append(" G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(" AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G")
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M = GAKMAST2.GAKKOU_CODE_T")
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M = SEITOMAST.GAKKOU_CODE_O")
            SQL.Append(" AND G_MEIMAST.NENDO_M = SEITOMAST.NENDO_O")
            SQL.Append(" AND G_MEIMAST.TUUBAN_M = SEITOMAST.TUUBAN_O")
            SQL.Append(" AND '04' = SEITOMAST.TUKI_NO_O")
            SQL.Append(" AND G_MEIMAST.TKIN_NO_M = TENMAST.KIN_NO_N")
            SQL.Append(" AND G_MEIMAST.TSIT_NO_M = TENMAST.SIT_NO_N")
            Select Case GakkouCode
                Case Is <> "9999999999"
                    '指定学校コード
                    SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M =" & SQ(GakkouCode))
            End Select
            '振替日
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M = " & SQ(FuriDate))
            '振替区分=0,1
            SQL.Append(" AND (G_MEIMAST.FURI_KBN_M = '0' OR G_MEIMAST.FURI_KBN_M = '1' ) ")
            '契約金融機関
            SQL.Append(" AND G_MEIMAST.TKIN_NO_M= " & SQ(JIKINKO))

            ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- START
            'SQL.Append(" AND G_MEIMAST.FURIKETU_CODE_M  <>  0")
            SQL.Append(" AND G_MEIMAST.FURIKETU_CODE_M IN (" & SFuriCode & ")")
            ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- END
            '請求金額が0円のデータは出力しない
            SQL.Append(" AND G_MEIMAST.SEIKYU_KIN_M > 0 ")

            SQL.Append(" ORDER BY ")
            Select Case PrintSort
                Case "0"
                    SQL.Append(" G_MEIMAST.GAKKOU_CODE_M")
                    SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M")
                    SQL.Append(",G_MEIMAST.CLASS_CODE_M")
                    SQL.Append(",G_MEIMAST.SEITO_NO_M")
                    SQL.Append(",G_MEIMAST.NENDO_M")
                    SQL.Append(",G_MEIMAST.TUUBAN_M")
                Case "1"
                    SQL.Append(" G_MEIMAST.GAKKOU_CODE_M")
                    SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M")
                    SQL.Append(",G_MEIMAST.NENDO_M")
                    SQL.Append(",G_MEIMAST.TUUBAN_M")
                Case Else
                    SQL.Append(" G_MEIMAST.GAKKOU_CODE_M")
                    SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M")
                    SQL.Append(",SEITOMAST.SEITO_KNAME_O")
                    SQL.Append(",G_MEIMAST.NENDO_M")
                    SQL.Append(",G_MEIMAST.TUUBAN_M")
            End Select

            If oraReader.DataReader(SQL) = True Then
                '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
                User_ID = oraReader.GetString("YOBI1_T")
                BaitaiCode = oraReader.GetString("BAITAI_CODE_T")
                ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
                '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_T")))                   '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))            '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEIKYU_KIN_M")))              '振替金額
                    OutputCsvData(Mid(oraReader.GetString("SEIKYU_TAISYOU_M"), 5, 2))           '請求月
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True)       '生徒名カナ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True)       '生徒名漢字
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIN_NNAME_N")))               '金融機関名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")))               '支店名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_M")))            '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_M")))              'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_M")))                '生徒番号
                    OutputCsvData(GCOM.NzInt(oraReader.GetString("JIFURI_KBN_T")), False, True) '自振区分
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
