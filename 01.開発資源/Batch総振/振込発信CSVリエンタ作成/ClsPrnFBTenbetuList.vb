''' <summary>
''' ＦＢタンキングデータ振込店別集計表　印刷クラス
''' </summary>
''' <remarks></remarks>
Public Class ClsPrnFBTenbetuList
    Inherits CAstReports.ClsReportBase

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'CSVファイル名設定
        InfoReport.ReportName = "KFSP034"
        '定義体名設定
        ReportBaseName = "KFSP034_FBタンキングデータ振込店別集計表.rpd"
    End Sub

End Class
