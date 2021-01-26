Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP010

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP010"

        ' 定義体名セット
        '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
        If STR_HIMOKU_PTN = "1" Then
            ReportBaseName = "KFGP010_費目マスタ一覧(費目15).rpd"
        Else
            ReportBaseName = "KFGP010_費目マスタ一覧.rpd"
        End If
        'ReportBaseName = "KFGP010_費目マスタ一覧.rpd"
        '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
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
        Dim TukiKin(12) As Decimal
        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append(" SELECT DISTINCT ")
            SQL.Append(" MAST0300G_WORK.GAKKOU_CODE_H")
            SQL.Append(",MAST0300G_WORK.GAKUNEN_CODE_H")
            SQL.Append(",MAST0300G_WORK.HIMOKU_ID_H")
            SQL.Append(",MAST0300G_WORK.GAKKOU_NNAME_H")
            SQL.Append(",MAST0300G_WORK.GAKUNEN_NAME_H")
            SQL.Append(",MAST0300G_WORK.HIMOKU_ID_NAME_H")
            For No As Integer = 1 To 15
                SQL.Append(",MAST0300G_WORK.KESSAI_KAMOKU" & No.ToString("00") & "_H")
                SQL.Append(",MAST0300G_WORK.KESSAI_TENPO" & No.ToString("00") & "_H")
                SQL.Append(",MAST0300G_WORK.KESSAI_KOUZA" & No.ToString("00") & "_H")
                SQL.Append(",MAST0300G_WORK.HIMOKU_NAME" & No.ToString("00") & "_H")
                '2015/02/13 saitou 標準版修正 DEL -------------------------------------------------->>>>
                '金融機関マスタの結合は不要
                'SQL.Append(",TENMAST_" & No.ToString & ".SIT_NNAME_N SIT_NNAME" & No.ToString("00") & "_N")
                '2015/02/13 saitou 標準版修正 DEL --------------------------------------------------<<<<
                SQL.Append(",MAST0300G_WORK.KESSAI_MEIGI" & No.ToString("00") & "_H")
                SQL.Append(",MAST0300G_WORK.KESSAI_KIN_CODE" & No.ToString("00") & "_H")
                '2015/02/13 saitou 標準版修正 DEL -------------------------------------------------->>>>
                '金融機関マスタの結合は不要
                'SQL.Append(",TENMAST_" & No.ToString & ".KIN_NNAME_N KIN_NNAME" & No.ToString("00") & "_N")
                '2015/02/13 saitou 標準版修正 DEL --------------------------------------------------<<<<
                For Tuki As Integer = 1 To 12
                    SQL.Append(", MAST0300G_WORK.HIMOKU_KINGAKU" & No.ToString("00") & Tuki.ToString("00") & "_H")
                Next
            Next
            SQL.Append(",GAKMAST2.FURI_CODE_T")
            SQL.Append(",GAKMAST2.KIGYO_CODE_T")

            SQL.Append(" FROM")
            SQL.Append(" KZFMAST.MAST0300G_WORK")
            '2015/02/13 saitou 標準版修正 DEL -------------------------------------------------->>>>
            '金融機関マスタの結合は不要
            'For No As Integer = 1 To 15
            '    SQL.Append(",KZFMAST.TENMAST TENMAST_" & No.ToString)
            'Next
            '2015/02/13 saitou 標準版修正 DEL --------------------------------------------------<<<<
            SQL.Append(",KZFMAST.GAKMAST1")
            SQL.Append(",KZFMAST.GAKMAST2")

            SQL.Append(" WHERE GAKKOU_CODE_H = GAKKOU_CODE_H")
            If GakkouCode.Trim <> "" Then
                SQL.Append(" AND GAKKOU_CODE_H = " & SQ(GakkouCode))
            End If
            '2015/02/13 saitou 標準版修正 DEL -------------------------------------------------->>>>
            '金融機関マスタの結合は不要
            'For No As Integer = 1 To 15
            '    SQL.Append(" AND MAST0300G_WORK.KESSAI_KIN_CODE" & No.ToString("00") & "_H = TENMAST_" & No.ToString & ".KIN_NO_N (+)")
            '    SQL.Append(" AND MAST0300G_WORK.KESSAI_TENPO" & No.ToString("00") & "_H = TENMAST_" & No.ToString & ".SIT_NO_N (+)")
            'Next
            '2015/02/13 saitou 標準版修正 DEL --------------------------------------------------<<<<
            SQL.Append(" AND MAST0300G_WORK.GAKKOU_CODE_H = GAKMAST2.GAKKOU_CODE_T (+)")

            SQL.Append(" ORDER BY GAKKOU_CODE_H ,GAKUNEN_CODE_H ,HIMOKU_ID_H")


            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    ReDim TukiKin(12)
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_H")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_H")))        '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_H")))        '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_NAME_H")))        '学年名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_ID_H")))           '費目ID
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_ID_NAME_H")))      '費目ID名
                    For No As Integer = 1 To 15
                        Dim HimoName As String
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H")).Trim)   '費目名1～15
                        HimoName = GCOM.NzStr(oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H"))
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("KESSAI_TENPO" & No.ToString("00") & "_H")))       '決済支店コード1～15
                        Select Case GCOM.NzStr(oraReader.GetString("KESSAI_KAMOKU" & No.ToString("00") & "_H"))         '決済科目1～15
                            Case "01"
                                OutputCsvData("当座")
                            Case "02"
                                OutputCsvData("普通")
                            Case "09"
                                OutputCsvData("その他")
                            Case Else
                                OutputCsvData("")
                        End Select
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("KESSAI_KOUZA" & No.ToString("00") & "_H")))       '決済口座1～15
                        For Tuki As Integer = 1 To 12                                       '1～12月費目金額1～15 
                            If HimoName <> "" Then
                                OutputCsvData(GCOM.NzLong(oraReader.GetString("HIMOKU_KINGAKU" & No.ToString("00") & Tuki.ToString("00") & "_H")))
                            Else
                                OutputCsvData("")
                            End If
                            TukiKin(Tuki) += GCOM.NzDec(oraReader.GetString("HIMOKU_KINGAKU" & No.ToString("00") & Tuki.ToString("00") & "_H"))
                        Next
                    Next
                    For Tuki As Integer = 1 To 12
                        OutputCsvData(TukiKin(Tuki))
                    Next
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")))
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")))
                    OutputCsvData("", False, True)                                          '改行用ダミー
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
