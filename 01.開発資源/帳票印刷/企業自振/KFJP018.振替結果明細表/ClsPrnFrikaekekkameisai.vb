Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 振替結果明細表
Class ClsPrnFrikaeKekkameisai
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        'InfoReport.ReportName = "Frikaekekkameisai"

        InfoReport.ReportName = "KFJP018"

        ' 定義体名セット
        ReportBaseName = "KFJP018_振替結果明細表.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' タイトル行
        '*** 修正 mitsu 2008/09/18 項目不足修正 ***
        '    CSVObject.Output ("取引先コード")
        '    CSVObject.Output ("委託者名カナ")
        '    CSVObject.Output ("振替日")
        '    CSVObject.Output ("タイトル区分")
        '    CSVObject.Output ("取扱金融機関コード")
        '    CSVObject.Output ("金融機関名カナ")
        '    CSVObject.Output ("取扱支店コード")
        '    CSVObject.Output ("支店名カナ")
        '    CSVObject.Output ("取扱店番")
        '    CSVObject.Output ("預金種別")
        '    CSVObject.Output ("口座番号")
        '    CSVObject.Output ("預金者名")
        '    CSVObject.Output ("振替依頼金額")
        '    CSVObject.Output ("振替不能理由")
        '    CSVObject.Output ("備考")
        '    CSVObject.Output ("振替依頼件数")
        '    CSVObject.Output ("振替依頼金額S")
        '    CSVObject.Output ("振替不能件数")
        '    CSVObject.Output ("振替不能金額")
        '    CSVObject.Output ("振替済件数")
        '    CSVObject.Output ("振替済金額")
        '    CSVObject.Output ("手数料(消費税込)")
        '    CSVObject.Output ("不能区分")
        '    CSVObject.Output ("委託者コード")
        '    CSVObject.Output ("取引先主コード")
        '    CSVObject.Output("取引先副コード", False, True)
        '******************************************

        CSVObject.Output("自金庫名")
        CSVObject.Output("取引先コード")
        CSVObject.Output("委託者名漢字")
        CSVObject.Output("委託者コード")
        CSVObject.Output("振替日")
        CSVObject.Output("とりまとめ店コード")
        CSVObject.Output("とりまとめ店名漢字")
        CSVObject.Output("金融機関コード")
        CSVObject.Output("支店")
        CSVObject.Output("口座名義")
        CSVObject.Output("科目")
        CSVObject.Output("口座番号")
        CSVObject.Output("振替金額")
        CSVObject.Output("振替不能理由")
        CSVObject.Output("需要家番号")
        CSVObject.Output("振替済件数")
        CSVObject.Output("振替済金額")
        CSVObject.Output("振替不能件数")
        CSVObject.Output("振替不能金額")
        CSVObject.Output("振替依頼件数")
        CSVObject.Output("振替依頼金額Ｓ")
        CSVObject.Output("手数料(消費税込)")
        CSVObject.Output("処理日")
        CSVObject.Output("金融機関名")
        'CSVObject.Output("支店名", False, True)
        CSVObject.Output("支店名")
        CSVObject.Output("フォーマット区分", False, True)

        Return file
    End Function

    '
    ' 機能　 ： 振替結果明細表をデータに書き込む
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
