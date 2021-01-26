''' <summary>
''' 総合振込明細表　印刷クラス
''' </summary>
''' <remarks></remarks>
Public Class ClsPrnSoufuriMeisai
    Inherits CAstReports.ClsReportBase

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'CSVファイル名設定
        InfoReport.ReportName = "KFSP033"
        '定義体名設定
        ReportBaseName = "KFSP033_総合振込明細表(CSVリエンタ).rpd"
    End Sub

End Class
