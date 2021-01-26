Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFJP036

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP036"

        ' 定義体名セット
        ReportBaseName = "KFJP036_振替結果変更チェックリスト.rpd"
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
            Dim FuriketuCode As String
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",FURI_DATE_K")
            SQL.Append(",KIGYO_SEQ_K")
            SQL.Append(",KEIYAKU_KIN_K")
            SQL.Append(",KIN_NNAME_N")
            SQL.Append(",KEIYAKU_SIT_K")
            SQL.Append(",SIT_NNAME_N")
            SQL.Append(",KEIYAKU_KAMOKU_K")
            SQL.Append(",KEIYAKU_KOUZA_K")
            SQL.Append(",KEIYAKU_KNAME_K")
            SQL.Append(",FURIKIN_K")
            SQL.Append(",RECORD_NO_K")
            SQL.Append(",FURIKETU_MAE_HK")
            SQL.Append(",FURIKETU_ATO_HK")
            SQL.Append(" FROM MEIMAST")
            SQL.Append(" ,KEKKA_RIREKIMAST")
            SQL.Append(" ,TORIMAST")
            SQL.Append(" ,TENMAST")
            SQL.Append(" WHERE FSYORI_KBN_K ='1'")
            SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_K = TORIF_CODE_T")
            SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_HK")
            SQL.Append(" AND TORIF_CODE_K = TORIF_CODE_HK")
            SQL.Append(" AND FURI_DATE_K = FURI_DATE_HK")
            SQL.Append(" AND RECORD_NO_K = RECORD_NO_HK")
            SQL.Append(" AND KOUSIN_DATE_HK = " & SQ(SyoriDate))
            SQL.Append(" AND KIN_NO_N = KEIYAKU_KIN_K(+)")
            SQL.Append(" AND SIT_NO_N = KEIYAKU_SIT_K(+)")
            SQL.Append(" ORDER BY TORIS_CODE_HK,TORIF_CODE_HK,FURI_DATE_HK,RECORD_NO_HK,HENKO_NO_HK")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate, True)                                      '処理日
                    OutputCsvData(mMatchingTime, True)                                      'タイムスタンプ
                    OutputCsvData(SyoriDate, True)                                          '変更日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIS_CODE_T")), True)    '取引先主コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIF_CODE_T")), True) '取引先副コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True) '取引先名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_K")), True) '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_SEQ_K")), True) '企業シーケンス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_KIN_K")), True) '金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIN_NNAME_N")), True) '金融機関名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_SIT_K")), True) '支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")), True)  '支店名
                    Select Case GCOM.NzStr(oraReader.GetString("KEIYAKU_KAMOKU_K")) '科目
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
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_KOUZA_K")), True) '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_KNAME_K")), True) '契約者名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURIKIN_K")), True) '振替金額
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("RECORD_NO_K")), True) 'レコード番号
                    FuriketuCode = GCOM.NzStr(oraReader.GetString("FURIKETU_MAE_HK"))
                    OutputCsvData(FuriketuCode, True) '変更前振替結果コード
                    OutputCsvData(SetFuriketu_Code(FuriketuCode), True) '変更前振替結果
                    FuriketuCode = GCOM.NzStr(oraReader.GetString("FURIKETU_ATO_HK"))
                    OutputCsvData(FuriketuCode, True) '変更後振替結果コード
                    OutputCsvData(SetFuriketu_Code(FuriketuCode), True, True)   '変更後振替結果

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
    '振替結果を取得
    Private Function SetFuriketu_Code(ByVal FURIKETU_CODE As Integer) As String
        SetFuriketu_Code = ""
        Select Case FURIKETU_CODE
            Case 0
                SetFuriketu_Code = "振替済み"
            Case 1
                SetFuriketu_Code = "資金不足"
            Case 2
                SetFuriketu_Code = "取引なし"
            Case 3
                SetFuriketu_Code = "預金者都合"
            Case 4
                SetFuriketu_Code = "依頼書なし"
            Case 8
                SetFuriketu_Code = "委託者都合"
            Case 9
                SetFuriketu_Code = "その他"
            Case Else
                SetFuriketu_Code = ""
        End Select
        Return SetFuriketu_Code
    End Function
End Class
