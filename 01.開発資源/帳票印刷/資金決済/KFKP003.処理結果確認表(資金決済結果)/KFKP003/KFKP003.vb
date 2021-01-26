Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFKP003

    Inherits CAstReports.ClsReportBase

    Sub New(ByVal CSVFileName As String, ByVal RpdPtn As String)
        Try
            '定義体名セット
            Dim strLSTPass As String = CASTCommon.GetFSKJIni("COMMON", "LST")
            Select Case RpdPtn
                Case "1"
                    ReportBaseName = strLSTPass & "KFKP003_処理結果確認表(資金決済結果_金バッチ).rpd"
                Case Else
                    ReportBaseName = strLSTPass & "KFKP003_処理結果確認表(資金決済結果).rpd"
            End Select
            'ＣＳＶファイル名セット(フルパス)
            MyBase.CsvName = CSVFileName
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
