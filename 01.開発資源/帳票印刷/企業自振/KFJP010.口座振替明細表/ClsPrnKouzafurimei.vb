Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 口座振替明細表
Class ClsPrnKouzafurimei
    Inherits CAstReports.ClsReportBase

    Sub New(ByVal SortKey As String)
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP010"

        ' 定義体名セット
        Select Case SortKey
            '2018/01/10 タスク）西野 CHG (標準版修正(№181)　帳票定義体ファイル名の変更) -------------------- START
            Case "1"
                ReportBaseName = "KFJP010_口座振替明細表(取引先別).rpd"
            Case "2"
                ReportBaseName = "KFJP010_口座振替明細表(支店別).rpd"
            Case Else
                ReportBaseName = "KFJP010_口座振替明細表(取引先別).rpd"
                'Case "1"
                '    ReportBaseName = "KFJP010.口座振替明細表(取引先別).rpd"
                'Case "2"
                '    ReportBaseName = "KFJP010.口座振替明細表(支店別).rpd"
                'Case Else
                '    ReportBaseName = "KFJP010.口座振替明細表(取引先別).rpd"
                '2018/01/10 タスク）西野 CHG (標準版修正(№181)　帳票定義体ファイル名の変更) -------------------- END
        End Select

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' タイトル行
        CSVObject.Output("振替日")
        CSVObject.Output("処理日")
        CSVObject.Output("タイムスタンプ")
        CSVObject.Output("取引先主コード")
        CSVObject.Output("取引先副コード")
        CSVObject.Output("委託者コード")
        CSVObject.Output("振替企業名")
        CSVObject.Output("金融機関コード")
        CSVObject.Output("金融機関名")
        CSVObject.Output("支店コード")
        CSVObject.Output("支店名")
        CSVObject.Output("科目")
        CSVObject.Output("口座番号")
        CSVObject.Output("契約者氏名")
        CSVObject.Output("金額")
        CSVObject.Output("入出金区分")
        CSVObject.Output("振替コード")
        CSVObject.Output("企業コード")
        CSVObject.Output("企業シーケンス")
        CSVObject.Output("摘要")
        CSVObject.Output("需要家番号", False, True)

        Return file
    End Function

    '
    ' 機能　 ： 口座振替明細表をデータに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
