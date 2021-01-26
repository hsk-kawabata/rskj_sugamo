Imports System

''' <summary>
''' 処理結果確認表(資金決済リエンタ結果更新)印刷クラス
''' </summary>
''' <remarks>2013/09/17 saitou 大垣信金 金バッチ対応</remarks>
Class KFKP003
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFKP003"
        ' 定義体名セット
        ReportBaseName = "KFKP003_処理結果確認表(資金決済結果_金バッチ).rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()
        Return file
    End Function

End Class
