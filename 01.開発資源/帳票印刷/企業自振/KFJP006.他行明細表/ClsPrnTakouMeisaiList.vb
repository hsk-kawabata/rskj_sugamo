Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 他行明細表印刷
Public Class ClsPrnTakouMeisaiList

    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP006"

        ' 定義体名セット
        ReportBaseName = "KFJP006_他行明細表.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' タイトル行
        CSVObject.Output("振替日")
        CSVObject.Output("処理日")
        CSVObject.Output("他行金融機関コード")
        CSVObject.Output("他行金融機関名")
        CSVObject.Output("取引先主コード")
        CSVObject.Output("取引先副コード")
        CSVObject.Output("委託者名")
        CSVObject.Output("支店コード")
        CSVObject.Output("支店名")
        CSVObject.Output("科目")
        CSVObject.Output("口座番号")
        CSVObject.Output("預金者名")
        CSVObject.Output("振替金額")
        CSVObject.Output("需要家番号", False, True)

        Return file
    End Function

    '
    ' 機能　 ： 他行明細表をデータに書き込む
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
