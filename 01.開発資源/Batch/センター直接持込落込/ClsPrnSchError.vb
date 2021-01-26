Imports System
Imports System.IO
Imports Microsoft.VisualBasic.Strings

' スケジュールエラーリスト
Class ClsPrnSchError
    Inherits CAstReports.ClsReportBase

    Sub New()

        ' CSVファイルセット
        InfoReport.ReportName = "KFJP014"

        ' 定義体名セット
        ReportBaseName = "KFJP014_スケジュールエラーリスト.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile()

        Return file
    End Function

    Public Sub OutputData(ByVal aToriComm As CAstBatch.CommData, ByVal fmt As CAstFormat.CFormat, ByVal errPtn As Integer)

        Dim InfoTori As CAstBatch.CommData.stTORIMAST = aToriComm.INFOToriMast      '取引先情報
        Dim mei As CAstFormat.CFormat.MEISAI = fmt.InfoMeisaiMast

        Try

            CSVObject.Output("センター直接持込落込")  '
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '処理日
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    'タイムスタンプ
            CSVObject.Output(InfoTori.TORIS_CODE_T)                         '取引先主コード 
            CSVObject.Output(InfoTori.TORIF_CODE_T)                         '取引先副コード
            CSVObject.Output(InfoTori.ITAKU_NNAME_T, True)                  '取引先名
            CSVObject.Output(InfoTori.FURI_CODE_T, True)                          '振替コード
            CSVObject.Output(InfoTori.KIGYO_CODE_T, True)                         '企業コード
            CSVObject.Output(mei.FURIKAE_DATE, True)                            '振替日

            Select Case errPtn
                Case 1
                    CSVObject.Output("スケジュールなし", True, True)                'エラー内容
                Case 2
                    CSVObject.Output("中断済", True, True)
                Case 3
                    CSVObject.Output("落込済", True, True)
            End Select


        Catch ex As Exception
            BatchLog.Write("(スケジュールエラーリスト印刷)CSVデータ出力", "失敗", ex.Message)
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
