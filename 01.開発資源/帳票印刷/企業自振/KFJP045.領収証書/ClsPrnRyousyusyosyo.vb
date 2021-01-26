Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 領収証書
Class ClsPrnRyousyusyosyo
    Inherits CAstReports.ClsReportBase

    Sub New(ByVal PrntKbn As String)
        Select Case PrntKbn
            Case "1"    '領収証書(2つ折)
                ' CSVファイルセット
                InfoReport.ReportName = "KFJP045"
                ' 定義体名セット
                ReportBaseName = "KFJP045_領収証書.rpd"
            Case "2"    '領収証書(3つ折)
                ' CSVファイルセット
                InfoReport.ReportName = "KFJP047"
                ' 定義体名セット
                ReportBaseName = "KFJP047_領収証書.rpd"
        End Select
        

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        CSVObject.Output("料金後納欄")
        CSVObject.Output("郵便番号")
        CSVObject.Output("都市区名")
        CSVObject.Output("住所１")
        CSVObject.Output("住所２")
        CSVObject.Output("住所３")
        CSVObject.Output("肩書１")
        CSVObject.Output("肩書２")
        CSVObject.Output("肩書３")
        CSVObject.Output("納税者名１")
        CSVObject.Output("納税者名２")
        CSVObject.Output("差出人")
        CSVObject.Output("差出人住所")
        CSVObject.Output("差出人電話番号")
        CSVObject.Output("差出人支店")
        CSVObject.Output("通番")
        CSVObject.Output("番号")
        CSVObject.Output("税目コード")
        CSVObject.Output("納期区分")
        CSVObject.Output("表示納期区分")
        CSVObject.Output("納期カナ文字")
        CSVObject.Output("納税期間(自)年")
        CSVObject.Output("納税期間(自)月")
        CSVObject.Output("納税期間(自)日")
        CSVObject.Output("納税期間(至)年")
        CSVObject.Output("納税期間(至)月")
        CSVObject.Output("納税期間(至)日")
        CSVObject.Output("年度")
        CSVObject.Output("税務署名")
        CSVObject.Output("日銀コード")
        CSVObject.Output("納付税額")
        CSVObject.Output("内利子税")
        CSVObject.Output("振替日")
        CSVObject.Output("領収欄１")
        CSVObject.Output("領収欄２")
        CSVObject.Output("領収欄３")
        '----------------京都北都信用金庫カスタマイズ　2009/11/2----------------
        'CSVObject.Output("領収欄４", False, True)
        CSVObject.Output("領収欄４")
        CSVObject.Output("総合通番", False, True)
        '----------------京都北都信用金庫カスタマイズ　2009/11/2----------------

        Return file
    End Function

    '
    ' 機能　 ： 領収証書をデータに書き込む
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
