Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 口座振替店別集計表
Class ClsPrnTenbetushuukei
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        '   InfoReport.ReportName = "Tenbetushuukei"
        InfoReport.ReportName = "KFJP019"

        ' 定義体名セット
        '2010/09/09.Sakon　定義体名はフルパスでセットする ++++++++++++++++++++
        Dim strLSTPass As String = CASTCommon.GetFSKJIni("COMMON", "LST")
        ReportBaseName = strLSTPass & "KFJP019_口座振替店別集計表.rpd"
        'ReportBaseName = "KFJP019_口座振替店別集計表.rpd"
        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        ' タイトル行
        CSVObject.Output("タイトル区分")
        CSVObject.Output("取引先主")
        CSVObject.Output("取引先副")
        CSVObject.Output("委託者コード")
        CSVObject.Output("取引先名漢字")
        CSVObject.Output("取扱金融機関コード")
        CSVObject.Output("取扱金融機関漢字")
        CSVObject.Output("とりまとめ支店コード")
        CSVObject.Output("とりまとめ支店名")
        CSVObject.Output("振替日")
        CSVObject.Output("支店コード")
        CSVObject.Output("支店名漢字")
        CSVObject.Output("振替依頼件数")
        CSVObject.Output("振替依頼金額")
        CSVObject.Output("振替件数")
        CSVObject.Output("振替金額")
        CSVObject.Output("不能件数")
        CSVObject.Output("不能金額", False, True)

        Return file
    End Function

    '
    ' 機能　 ： 口座振替店別集計表をデータに書き込む
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
