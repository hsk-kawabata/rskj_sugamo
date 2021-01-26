Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFSP022
    Inherits CAstReports.ClsReportBase

    Private TORIS_CODE As String    '取引先主コード
    Private TORIF_CODE As String    '取引先副コード
    Private TKIN_NO As String       '取扱金融機関コード
    Private KIN_NAME As String      '取扱金融機関名
    Private TSIT_NO As String       '取扱支店名
    Private SIT_NAME As String      '取扱支店名
    Private ITAKU_CODE As String    '委託者コード
    Private ITAKU_NAME As String    '委託者漢字名
    Private NS_KBN As String        '入出金区分
    Private TEIGAKU_KBN As String   '定額区分
    Private IRAISYO_SORT As String  '依頼書ソート順

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFSP022"

        ' 定義体名セット
        ReportBaseName = "KFSP022_総合振込依頼書.rpd"
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
        MainDB = New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            oraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.Append(" SELECT ")
            SQL.Append(" TORIS_CODE_T")         '取引先主コード
            SQL.Append(",TORIF_CODE_T")         '取引先副コード
            SQL.Append(",TKIN_NO_T")            '金融機関コード
            SQL.Append(",KIN_NNAME_N")          '金融機関名
            SQL.Append(",TSIT_NO_T")            '支店コード
            SQL.Append(",SIT_NNAME_N")          '支店名
            SQL.Append(",ITAKU_CODE_T")         '委託者コード
            SQL.Append(",ITAKU_NNAME_T")        '委託者名
            SQL.Append(",NS_KBN_T")             '入出金区分
            SQL.Append(",TEIGAKU_KBN_T")        '定額区分
            SQL.Append(",IRAISYO_SORT_T")       '依頼書出力順

            SQL.Append(" FROM S_TORIMAST,S_SCHMAST,TENMAST")
            SQL.Append(" WHERE FSYORI_KBN_T = FSYORI_KBN_S")
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_S = " & SQ(FuriDate))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            If TorisCode + TorifCode <> "111111111111" Then
                SQL.Append(" AND TORIS_CODE_T = " & SQ(TorisCode))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(TorifCode))
            End If
            SQL.Append(" AND SYURYOU_DATE_T >= " & SQ(FuriDate))
            SQL.Append(" AND BAITAI_CODE_T = '04'")
            SQL.Append(" AND KIN_NO_N = TKIN_NO_T")
            SQL.Append(" AND SIT_NO_N = TSIT_NO_T")
            SQL.Append(" ORDER BY TORIS_CODE_T,TORIF_CODE_T")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    TORIS_CODE = GCOM.NzStr(oraReader.GetString("TORIS_CODE_T"))        '取引先主コード
                    TORIF_CODE = GCOM.NzStr(oraReader.GetString("TORIF_CODE_T"))        '取引先副コード
                    TKIN_NO = GCOM.NzStr(oraReader.GetString("TKIN_NO_T"))              '金融機関コード
                    KIN_NAME = GCOM.NzStr(oraReader.GetString("KIN_NNAME_N"))           '金融機関名
                    TSIT_NO = GCOM.NzStr(oraReader.GetString("TSIT_NO_T"))              '支店コード
                    SIT_NAME = GCOM.NzStr(oraReader.GetString("SIT_NNAME_N"))           '支店名
                    ITAKU_CODE = GCOM.NzStr(oraReader.GetString("ITAKU_CODE_T"))        '委託者コード
                    ITAKU_NAME = GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T"))       '委託者名
                    NS_KBN = GCOM.NzStr(oraReader.GetString("NS_KBN_T"))                '入出金区分
                    TEIGAKU_KBN = GCOM.NzStr(oraReader.GetString("TEIGAKU_KBN_T"))      '定額区分
                    IRAISYO_SORT = GCOM.NzStr(oraReader.GetString("IRAISYO_SORT_T"))    '依頼書出力順
                    If SetENTMAST() = False Then
                        Return False
                    End If
                    oraReader.NextRead()
                End While
                If AllRecordCnt = 0 Then
                    BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                    RecordCnt = -1
                    Return False
                End If
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
            If Not MainDB Is Nothing Then MainDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function
    '
    ' 機能　 ： 依頼書の内容を書き出す
    '
    ' 備考　 ： 
    '
    Private Function SetENTMAST() As Boolean

        RecordCnt = 0
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.Append(" SELECT")
            SQL.Append(" TKIN_NO_E")
            SQL.Append(",KIN_NNAME_N")
            SQL.Append(",TSIT_NO_E")
            SQL.Append(",SIT_NNAME_N")
            SQL.Append(",KEIYAKU_KNAME_E")
            SQL.Append(",KEIYAKU_NNAME_E")
            SQL.Append(",KEIYAKU_NO_E")
            SQL.Append(",KAMOKU_E")
            SQL.Append(",KOUZA_E")
            SQL.Append(",FURIKIN_E")
            SQL.Append(" FROM S_ENTMAST,S_TORIMAST,TENMAST")
            SQL.Append(" WHERE TORIS_CODE_T = TORIS_CODE_E")
            SQL.Append(" AND   TORIF_CODE_T = TORIF_CODE_E")
            SQL.Append(" AND   KAIYAKU_E = '0'")    '解約済みでない
            SQL.Append(" AND   TORIS_CODE_E = " & SQ(TORIS_CODE))
            SQL.Append(" AND   TORIF_CODE_E = " & SQ(TORIF_CODE))
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            '金融機関マスタに存在しない場合も出力する
            SQL.Append(" AND   KIN_NO_N(+) = TKIN_NO_E")
            SQL.Append(" AND   SIT_NO_N(+) = TSIT_NO_E")
            'SQL.Append(" AND   KIN_NO_N = TKIN_NO_E")
            'SQL.Append(" AND   SIT_NO_N = TSIT_NO_E")
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
            Select Case IRAISYO_SORT
                Case "0"
                    SQL.Append(" ORDER BY KEIYAKU_NO_E")
                Case "1"
                    SQL.Append(" ORDER BY KEIYAKU_KNAME_E , KEIYAKU_NO_E")
                Case "2"
                    SQL.Append(" ORDER BY JYUYOUKA_NO_E , KEIYAKU_NO_E")
                    ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-18(RSV2対応<小浜信金>) -------------------- START
                Case "3"
                    SQL.Append(" ORDER BY TKIN_NO_E , TSIT_NO_E , KAMOKU_E , KOUZA_E ASC")
                    ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-18(RSV2対応<小浜信金>) -------------------- END
            End Select

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(TKIN_NO, True)           '取扱金融機関コード
                    OutputCsvData(TSIT_NO, True)           '取扱支店コード
                    OutputCsvData(KIN_NAME, True)          '取扱金融機関名
                    OutputCsvData(SIT_NAME, True)          '取扱支店名
                    OutputCsvData(ITAKU_CODE, True)        '委託者コード
                    OutputCsvData(ITAKU_NAME, True)        '委託者名
                    OutputCsvData(FuriDate, True)          '振込日
                    Select Case NS_KBN                     '入出金区分
                        Case "1"
                            OutputCsvData("入金", True)
                        Case "9"
                            OutputCsvData("出金", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    OutputCsvData(TORIS_CODE, True)        '取引先主コード
                    OutputCsvData(TORIF_CODE, True)        '取引先副コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_E")), True)               '振込金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIN_NNAME_N")), True)             '振込金融機関名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_E")), True)               '振込支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")), True)             '振込支店名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_KNAME_E")), True)         '受取人名カナ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_NNAME_E")), True)         '受取人名漢字
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_NO_E")), True)            '契約者番号
                    Select Case GCOM.NzStr(oraReader.GetString("KAMOKU_E"))                         '科目
                        Case "02"
                            OutputCsvData("普通", True)
                        Case "01"
                            OutputCsvData("当座", True)
                        Case "37"
                            OutputCsvData("職員", True)
                        Case "05"
                            OutputCsvData("納税", True)
                        Case "04"
                            OutputCsvData("別段", True)
                        Case "99"
                            OutputCsvData("諸勘定", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_E")), True)                 '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURIKIN_E")), True)               '振込金額
                    OutputCsvData(TEIGAKU_KBN, True, True)                                          '定額区分
                    AllRecordCnt += 1
                    RecordCnt += 1

                    oraReader.NextRead()
                End While
                Return True
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = 0
                Return True
            End If
        Catch ex As Exception
            '2017/05/22 saitou RSV2 DEL 標準版修正（潜在バグ） ---------------------------------------- START
            '不要
            'Ret = -300
            '2017/05/22 saitou RSV2 DEL --------------------------------------------------------------- END
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function
End Class
