Imports System
Imports System.IO

''' <summary>
''' 総合振込明細表(CSVリエンタ)メインクラス
''' </summary>
''' <remarks>2016/10/20 saitou RSV2 added for 信組対応</remarks>
Public Class KFSP033
    Inherits CAstReports.ClsReportBase

#Region "クラス定数"

#End Region

#Region "クラス変数"

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' コンストラクタ生成
    ''' </summary>
    ''' <param name="CSVFileName"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal CSVFileName As String)
        Try
            '定義体名セット
            ReportBaseName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "LST"), "KFSP033_総合振込明細表(CSVリエンタ).rpd")
            'ＣＳＶファイル名セット(フルパス)
            MyBase.CsvName = CSVFileName
        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region "プライベートメソッド"

#End Region

End Class
