Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic

' 委託者別振替結果一覧表
Class ClsPrnSyorikekkaKakuninhyo
    Inherits CAstReports.ClsReportBase

    Public CsvData(13) As String
    ' 2016/06/17 小嶋 ADD (RSV2対応<小浜信金>) ------------------------------------ START
    Public strATENA_UMU As String
    ' 2016/06/17 小嶋 ADD (RSV2対応<小浜信金>) -------------------------------------- END

    Sub New()
        '   ' CSVファイルセット
        '   InfoReport.ReportName = "委託者別振替結果一覧表"

        '   ' 定義体名セット
        '   ReportBaseName = "委託者別振替結果一覧表.rpd"

        ' CSVファイルセット
        InfoReport.ReportName = "KFJP020"

        ' 定義体名セット
        ReportBaseName = "KFJP020_処理結果確認表(返還データ作成).rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile()

        ' タイトル行
        CSVObject.Output("処理日")
        CSVObject.Output("タイムスタンプ")
        CSVObject.Output("振替日")
        ' 2016/06/17 小嶋 ADD (RSV2対応<小浜信金>) -------------------------------- START
        If strATENA_UMU <> "err" Then         '小浜信金以降のVerのみ項目追加
            CSVObject.Output("郵送先名")      '郵送先（なければ委託者名)設定
            CSVObject.Output("金庫名")        '金庫名（iniファイル)設定
        End If
        ' 2016/06/17 小嶋 ADD (RSV2対応<小浜信金>) ---------------------------------- END
        CSVObject.Output("取引先主コード")
        CSVObject.Output("取引先副コード")
        CSVObject.Output("取引先名")
        CSVObject.Output("委託者コード")
        CSVObject.Output("請求件数")
        CSVObject.Output("請求金額")
        CSVObject.Output("振替件数")
        CSVObject.Output("振替金額")
        CSVObject.Output("不能件数")
        CSVObject.Output("不能金額")
        CSVObject.Output("備考")
        CSVObject.Output("区分", False, True)

        Return file
    End Function

    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
