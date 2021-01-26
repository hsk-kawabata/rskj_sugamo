Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 口座振替資金決済一覧表
Class KFJP012
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP012"

        'センターコード
        Dim CENTER As String = ""
        CENTER = CASTCommon.GetFSKJIni("COMMON", "CENTER")
        If CENTER.ToUpper = "ERR" Or CENTER = "" Then
            MainLOG.Write("設定ファイル取得", "失敗", "項目名:センターコード 分類:COMMON 項目:CENTER")
        End If
        ' 定義体名セット
        '2011/07/05 標準版修正 東海とその他の地域でレイアウトを変更する ------------------START
        'ReportBaseName = "KFJP012_預金口座振替変更通知書.rpd"
        If CENTER = "4" Then
            ReportBaseName = "KFJP012_2_預金口座振替変更通知書_東海.rpd"
        Else
            ReportBaseName = "KFJP012_預金口座振替変更通知書.rpd"
        End If
        '2011/07/05 標準版修正 東海とその他の地域でレイアウトを変更する ------------------END
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

End Class