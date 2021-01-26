''' <summary>
''' 処理結果確認表　印刷クラス
''' </summary>
''' <remarks></remarks>
Public Class ClsPrnSyoriKekkaList
    Inherits CAstReports.ClsReportBase

#Region "クラス変数"

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'CSVファイル名設定
        InfoReport.ReportName = "KFSP032"
        '定義体名設定
        ReportBaseName = "KFSP032_処理結果確認表(CSVリエンタ).rpd"
    End Sub

#End Region

End Class
