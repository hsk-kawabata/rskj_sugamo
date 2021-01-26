Imports System
Imports System.IO
Imports Microsoft.VisualBasic.Strings

' スケジュールエラーリスト
Class ClsPrnToriError
    Inherits CAstReports.ClsReportBase

    Sub New()

        ' CSVファイルセット
        InfoReport.ReportName = "KFJP015"

        ' 定義体名セット
        ReportBaseName = "KFJP015_取引先マスタエラーリスト.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        Return file
    End Function

    Public Sub OutputData(ByVal fmt As CAstFormat.CFormat)

        Dim mei As CAstFormat.CFormat.MEISAI = fmt.InfoMeisaiMast

        Try

            CSVObject.Output("センター直接持込落込")                        'サブタイトル
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '処理日
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    'タイムスタンプ
            CSVObject.Output(mei.FURI_CODE, True)                           '振替コード
            CSVObject.Output(mei.KIGYO_CODE, True)                          '企業コード
            CSVObject.Output(mei.FURIKAE_DATE, True, True)                  '振替日

        Catch ex As Exception
            BatchLog.Write("(取引先エラーリスト印刷)CSVデータ出力", "失敗", ex.Message)
        End Try

    End Sub

    Public Overloads Overrides Function ReportExecute() As Boolean

        '帳票出力処理
        '2010/02/03 通常使用するプリンタにする ===
        'Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
        Return MyBase.ReportExecute()
        '=======================================================
    End Function

    Protected Overrides Sub Finalize()

        MyBase.Finalize()

    End Sub

End Class
