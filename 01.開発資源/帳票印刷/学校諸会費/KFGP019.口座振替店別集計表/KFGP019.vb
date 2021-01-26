Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP019

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP019"

        ' 定義体名セット
        ReportBaseName = "KFGP019_口座振替店別集計表.rpd"
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

            SQL.Append("SELECT")
            SQL.Append(" GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",GAKMAST1.GAKUNEN_CODE_G")
            SQL.Append(",TENMAST.KIN_NNAME_N")
            SQL.Append(",TENMAST.SIT_NNAME_N")
            SQL.Append(",TENMAST_1.SIT_NNAME_N FURI_SIT_NAME")
            SQL.Append(",G_MEIMAST.GAKKOU_CODE_M")
            SQL.Append(",G_MEIMAST.TKIN_NO_M")
            SQL.Append(",G_MEIMAST.TSIT_NO_M")
            SQL.Append(",G_MEIMAST.FURIKETU_CODE_M")
            SQL.Append(",G_MEIMAST.FURI_DATE_M")
            SQL.Append(",G_MEIMAST.SEIKYU_KIN_M")
            SQL.Append(",GAKMAST2.TSIT_NO_T")
            SQL.Append(",GAKMAST2.TKIN_NO_T")
            SQL.Append(",G_MEIMAST.TSIT_NO_M")
            SQL.Append(",(CASE TKIN_NO_M WHEN " & SQ(JIKINKO) & " THEN 0 ELSE 1 END) SORT_NO")
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
            SQL.Append(",GAKMAST2.YOBI1_T")             'ユーザID
            SQL.Append(",GAKMAST2.BAITAI_CODE_T")       '媒体コード 
            SQL.Append(",GAKMAST2.ITAKU_CODE_T")        '委託者コード
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
            SQL.Append(" FROM ")
            SQL.Append(" KZFMAST.G_MEIMAST G_MEIMAST")
            SQL.Append(",KZFMAST.GAKMAST1 GAKMAST1 ")
            SQL.Append(",KZFMAST.GAKMAST2 GAKMAST2 ")
            SQL.Append(",KZFMAST.TENMAST TENMAST_1 ")
            SQL.Append(",KZFMAST.TENMAST TENMAST ")
            '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
            '特別振替日対応
            SQL.Append(",(SELECT GAKKOU_CODE_S,FURI_DATE_S,FURI_KBN_S,FUNOU_FLG_S FROM G_SCHMAST")
            SQL.Append("  GROUP BY GAKKOU_CODE_S,FURI_DATE_S,FURI_KBN_S,FUNOU_FLG_S) G_SCHMAST")
            'SQL.Append(",KZFMAST.G_SCHMAST G_SCHMAST ")
            '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END
            SQL.Append(" WHERE G_MEIMAST.TKIN_NO_M=TENMAST.KIN_NO_N")
            SQL.Append(" AND G_MEIMAST.TSIT_NO_M=TENMAST.SIT_NO_N")
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M=GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(" AND GAKMAST1.GAKKOU_CODE_G=GAKMAST2.GAKKOU_CODE_T")
            SQL.Append(" AND GAKMAST2.TKIN_NO_T=TENMAST_1.KIN_NO_N (+)")
            SQL.Append(" AND GAKMAST2.TSIT_NO_T=TENMAST_1.SIT_NO_N (+)")
            SQL.Append(" AND GAKMAST1.GAKUNEN_CODE_G=1")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M=G_SCHMAST.FURI_DATE_S")
            SQL.Append(" AND G_MEIMAST.FURI_KBN_M=G_SCHMAST.FURI_KBN_S")
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M = G_SCHMAST.FURI_DATE_S")
            SQL.Append(" AND G_SCHMAST.FUNOU_FLG_S = '1'")
            If GakkouCode.Trim <> "9999999999" Then
                '指定学校コード
                SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M  =" & SQ(GakkouCode))
            End If
            '振替日
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M = " & SQ(FuriDate))

            '自金庫データ以外をその他欄に集計して印字する
            Select Case FuriKbn
                Case "0", "1"
                    SQL.Append(" AND (G_MEIMAST.FURI_KBN_M = '0' OR G_MEIMAST.FURI_KBN_M =  '1') ")
                Case Else
                    SQL.Append(" AND G_MEIMAST.FURI_KBN_M = " & SQ(FuriKbn))
            End Select
            SQL.Append(" AND G_MEIMAST.SEIKYU_KIN_M  > 0 ")
            SQL.Append(" ORDER BY ")
            SQL.Append(" G_MEIMAST.GAKKOU_CODE_M,G_MEIMAST.FURI_DATE_M,SORT_NO,G_MEIMAST.TKIN_NO_M,G_MEIMAST.TSIT_NO_M ")

            If oraReader.DataReader(SQL) = True Then
                Dim SitGRP As String

                '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
                User_ID = oraReader.GetString("YOBI1_T")
                BaitaiCode = oraReader.GetString("BAITAI_CODE_T")
                ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
                '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
                While oraReader.EOF = False
                    SitGRP = ""
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_M")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名
                    '2011/06/16 標準版修正 取り纏め店取得 ------------------START
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_T")))             'とりまとめ店
                    'OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_M")))             '取扱店
                    '2011/06/16 標準版修正 取り纏め店取得 ------------------END
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_SIT_NAME")))         '取扱店名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_M")))           '振替日
                    If oraReader.GetString("TKIN_NO_M") = JIKINKO Then
                        SitGRP = oraReader.GetString("TSIT_NO_M")
                        SitGRP &= "    "
                        SitGRP &= oraReader.GetString("SIT_NNAME_N")
                    Else
                        SitGRP = oraReader.GetString("TKIN_NO_M")
                        SitGRP &= "   "
                        SitGRP &= oraReader.GetString("KIN_NNAME_N")
                    End If
                    OutputCsvData(SitGRP)                                                   '支店グループ名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEIKYU_KIN_M")))          '請求金額
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURIKETU_CODE_M")), False, True) '振替結果コード
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
