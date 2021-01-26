Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFGP108
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP108"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        '列名設定
        With CSVObject
            .Output("タイトル")
            .Output("処理日")
            .Output("タイムスタンプ")
            .Output("とりまとめ店")
            .Output("学校コード")
            .Output("学校名")
            .Output("入学年度")
            .Output("学年")
            .Output("通番")
            .Output("生徒番号")
            .Output("生徒名カナ")
            .Output("生徒名漢字")
            .Output("口座名義カナ")
            .Output("口座名義漢字")
            .Output("取扱金融機関コード")
            .Output("取扱金融機関名")
            .Output("取扱支店コード")
            .Output("取扱支店名")
            .Output("科目")
            .Output("口座番号")
            .Output("クラス")
            .Output("性別")
            .Output("振替方法")
            .Output("解約区分")
            .Output("電話番号")
            .Output("進級区分")
            .Output("住所")
            .Output("費目ID")
            .Output("費目名")
            .Output("金額1")
            .Output("金額2")
            .Output("金額3")
            .Output("金額4")
            .Output("金額5")
            .Output("金額6")
            .Output("金額7")
            .Output("金額8")
            .Output("金額9")
            .Output("金額10")
            .Output("金額11")
            .Output("金額12")
            .Output("費目表示", False, True)
        End With

        Return file
    End Function

    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
