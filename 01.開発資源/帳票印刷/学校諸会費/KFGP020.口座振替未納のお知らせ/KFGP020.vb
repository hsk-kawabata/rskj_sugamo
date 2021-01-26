Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP020

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP020"

        ' 定義体名セット
        ReportBaseName = "KFGP020_口座振替未納のお知らせ.rpd"
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
            SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M")
            SQL.Append(",G_MEIMAST.CLASS_CODE_M")
            SQL.Append(",G_MEIMAST.SEITO_NO_M")
            SQL.Append(",G_MEIMAST.TKAMOKU_M")
            SQL.Append(",G_MEIMAST.TKOUZA_M")
            SQL.Append(",G_MEIMAST.TMEIGI_KNM_M")
            SQL.Append(",G_MEIMAST.SEIKYU_TUKI_M")
            SQL.Append(",G_MEIMAST.SEIKYU_KIN_M")
            SQL.Append(",G_MEIMAST.TSIT_NO_M")
            SQL.Append(",G_MEIMAST.FURI_DATE_M")
            '2017/03/14 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ START
            '特別振替日対応
            SQL.Append(",G_MEIMAST.FURI_KBN_M")
            '2017/03/14 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ END
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",SEITOMAST.SEITO_KNAME_O")
            SQL.Append(",SEITOMAST.SEITO_NNAME_O")
            SQL.Append(",TENMAST.KIN_NNAME_N")
            SQL.Append(",TENMAST.SIT_NNAME_N")
            '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
            '特別振替日対応
            '再振日は別途取得する
            'SQL.Append(",SFURI_DATE_S")
            '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
            SQL.Append(",GAKMAST2.YOBI1_T")             'ユーザID
            SQL.Append(",GAKMAST2.BAITAI_CODE_T")       '媒体コード 
            SQL.Append(",GAKMAST2.ITAKU_CODE_T")        '委託者コード
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
            SQL.Append(" FROM ")
            SQL.Append(" KZFMAST.G_MEIMAST")
            SQL.Append(",KZFMAST.GAKMAST1")
            SQL.Append(",KZFMAST.SEITOMAST")
            SQL.Append(",KZFMAST.TENMAST")
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
            SQL.Append(",KZFMAST.GAKMAST2")
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
            '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
            '特別振替日対応
            'SQL.Append(",KZFMAST.G_SCHMAST")
            '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END
            SQL.Append(" WHERE")
            SQL.Append("       G_MEIMAST.GAKKOU_CODE_M    = GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(" AND   G_MEIMAST.GAKUNEN_CODE_M   = GAKMAST1.GAKUNEN_CODE_G")
            SQL.Append(" AND   G_MEIMAST.GAKKOU_CODE_M    = SEITOMAST.GAKKOU_CODE_O")
            SQL.Append(" AND   G_MEIMAST.NENDO_M          = SEITOMAST.NENDO_O")
            SQL.Append(" AND   G_MEIMAST.TUUBAN_M         = SEITOMAST.TUUBAN_O")
            '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
            '特別振替日対応
            'SQL.Append(" AND   G_MEIMAST.GAKKOU_CODE_M    = G_SCHMAST.GAKKOU_CODE_S")
            'SQL.Append(" AND   G_MEIMAST.FURI_DATE_M      = G_SCHMAST.FURI_DATE_S")
            'SQL.Append(" AND   G_MEIMAST.FURI_KBN_M       = G_SCHMAST.FURI_KBN_S")
            '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END
            SQL.Append(" AND   '04'                       = SEITOMAST.TUKI_NO_O")
            SQL.Append(" AND   G_MEIMAST.TKIN_NO_M        = TENMAST.KIN_NO_N")
            SQL.Append(" AND   G_MEIMAST.TSIT_NO_M        = TENMAST.SIT_NO_N")
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
            SQL.Append(" AND   GAKMAST1.GAKKOU_CODE_G = GAKMAST2.GAKKOU_CODE_T")
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END

            Select Case GakkouCode
                Case Is <> "9999999999"
                    '指定学校コード
                    SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M = " & SQ(GakkouCode))
            End Select
            '振替日
            SQL.Append(" AND   G_MEIMAST.FURI_DATE_M      = " & SQ(FuriDate))

            SQL.Append(" AND   ( G_MEIMAST.FURI_KBN_M     = '0'")
            SQL.Append(" OR      G_MEIMAST.FURI_KBN_M     = '1')")
            ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- START
            'SQL.Append(" AND   G_MEIMAST.FURIKETU_CODE_M <> 0 ")
            SQL.Append(" AND   G_MEIMAST.FURIKETU_CODE_M IN (" & SFuriCode & ")")
            ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- END
            SQL.Append(" AND   G_MEIMAST.SEIKYU_KIN_M     > 0 ")

            '2017/04/05 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
            '特別振替日対応
            'SQL.Append(" AND   ( G_SCHMAST.FURI_KBN_S     = '0'")
            'SQL.Append(" OR      G_SCHMAST.FURI_KBN_S     = '1')")
            '2017/04/05 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END

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
                Dim Saifuri As String

                '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
                User_ID = oraReader.GetString("YOBI1_T")
                BaitaiCode = oraReader.GetString("BAITAI_CODE_T")
                ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
                '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
                While oraReader.EOF = False
                    Saifuri = "0"
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TMEIGI_KNM_M")), True)          '名義人
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '依頼者
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEIKYU_TUKI_M")))         '請求月
                    '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
                    '特別振替日対応
                    Dim SFuriDate As String = Me.GetSFuriDate(oraDB, oraReader)
                    If SFuriDate <> "" Then
                        OutputCsvData(SFuriDate)
                        Saifuri = "1"
                    Else    '再振なし
                        OutputCsvData("")                                                   '再振月

                    End If

                    'If GCOM.NzLong(oraReader.GetString("SFURI_DATE_S")) <> 0 Then
                    '    OutputCsvData(GCOM.NzStr(oraReader.GetString("SFURI_DATE_S")))      '再振月
                    '    Saifuri = "1"
                    'Else    '再振なし
                    '    OutputCsvData("")                                                   '再振月
                    'End If
                    '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_M")))        '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_M")))          'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_M")))            '生徒番号
                    If GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")).Trim = "" Then      '生徒氏名
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")), True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEIKYU_KIN_M")))          '金額
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEIKYU_KIN_M")))          '収納金額

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_M")))             '取扱店
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")))           '取扱店名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_M")))           '振替日

                    OutputCsvData(Saifuri, False, True)         '再振表示
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

    '2017/03/14 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ START
    '特別振替日対応
    Private Function GetSFuriDate(ByVal OraDB As CASTCommon.MyOracle, _
                                  ByVal OraReader As CASTCommon.MyOracleReader) As String
        Dim OraSchReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("select SFURI_DATE_S from G_SCHMAST")
                .Append(" where GAKKOU_CODE_S = " & SQ(OraReader.GetString("GAKKOU_CODE_M")))
                .Append(" and FURI_DATE_S = " & SQ(OraReader.GetString("FURI_DATE_M")))
                .Append(" and FURI_KBN_S = " & SQ(OraReader.GetString("FURI_KBN_M")))
                .Append(" and GAKUNEN" & OraReader.GetString("GAKUNEN_CODE_M") & "_FLG_S = '1'")
            End With

            OraSchReader = New CASTCommon.MyOracleReader(OraDB)
            If OraSchReader.DataReader(SQL) = True Then
                If GCOM.NzLong(OraSchReader.GetString("SFURI_DATE_S")) <> 0 Then
                    Return OraSchReader.GetString("SFURI_DATE_S")
                Else
                    Return ""
                End If
            Else
                Return ""
            End If

        Catch ex As Exception
            BatchLog.Write("再振日取得", "失敗", ex.Message)
            Return ""
        Finally
            If Not OraSchReader Is Nothing Then
                OraSchReader.Close()
                OraSchReader = Nothing
            End If
        End Try
    End Function
    '2017/03/14 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ END
End Class
