Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.IO
Imports System.Collections
Imports CAstBatch

Class ClsKFJP043
    Inherits CAstReports.ClsReportBase

    Public CsvData(12) As String

    Private strKESSAI_DATE As String                    ' 決済日

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP043"

        ' 定義体名セット
        ReportBaseName = "KFJP043_処理結果確認表(自振契約).rpd"

        CsvData(0) = "00010101"                                     ' 処理日
        CsvData(1) = "00010101"                                     ' タイムスタンプ
        CsvData(2) = ""                                             ' 振替コード
        CsvData(3) = ""                                             ' 企業コード
        CsvData(4) = ""                                             ' 取引先主コード
        CsvData(5) = ""                                             ' 取引先副コード
        CsvData(6) = ""                                             ' 取引先名
        CsvData(7) = ""                                             ' 契約者支店コード
        CsvData(8) = ""                                             ' 契約者支店名
        CsvData(9) = ""                                             ' 契約者科目
        CsvData(10) = ""                                            ' 契約者口座番号
        CsvData(11) = ""                                            ' 契約者カナ氏名
        CsvData(12) = ""                                            ' 備考

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' タイトル行
        CSVObject.Output("処理日")
        CSVObject.Output("タイムスタンプ")
        CSVObject.Output("振替コード")
        CSVObject.Output("企業コード")
        CSVObject.Output("取引先主コード")
        CSVObject.Output("取引先副コード")
        CSVObject.Output("取引先名")
        CSVObject.Output("契約者支店コード")
        CSVObject.Output("契約者支店名")
        CSVObject.Output("契約者科目")
        CSVObject.Output("契約者口座番号")
        CSVObject.Output("契約者カナ氏名")
        CSVObject.Output("備考", False, True)

        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Return file
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

    End Function

    '
    ' 機能　 ： 自振契約処理結果確認表ＣＳＶを出力する
    '
    ' 備考　 ： 
    '
    Public Function OutputCSVKekka(ByVal ary As ArrayList, ByVal jikinko As String, ByVal strDate As String, ByVal strTime As String) As Integer


        Dim JData As New CAstFormKes.ClsFormKes.JifkeiData
        Dim fmt10004 As New CAstFormKes.ClsFormSikinFuri.T_10004

        Dim cnt As Integer = ary.Count - 1 'ループ回数

        Try

            For i As Integer = 0 To cnt

                JData.Init()
                fmt10004.Init()

                JData = CType(ary.Item(i), CAstFormKes.ClsFormKes.JifkeiData)
                fmt10004.Data = JData.record320

                ' ＣＳＶデータ設定
                CsvData(0) = strDate                                                ' 処理日
                CsvData(1) = strTime                                                ' タイムスタンプ
                CsvData(2) = JData.FuriCode                                         ' 振替コード
                CsvData(3) = JData.KigyoCode                                        ' 企業コード
                CsvData(4) = JData.TorisCode                                        ' 取引先主コード
                CsvData(5) = JData.TorifCode                                        ' 取引先副コード
                CsvData(6) = JData.ToriNName.Trim                                   ' 取引先名
                CsvData(7) = fmt10004.GENTEN_NO                                     ' 契約者支店コード
                CsvData(8) = GetTenmast(jikinko, fmt10004.GENTEN_NO, MainDB)        ' 契約者支店名
                CsvData(9) = fmt10004.KAMOKU_KOUZA_NO.Substring(0, 2)               ' 契約者科目
                CsvData(10) = fmt10004.KAMOKU_KOUZA_NO.Substring(2, 7)              ' 契約者口座番号
                CsvData(11) = JData.KeiyakuKname                                    ' 契約者カナ氏名
                CsvData(12) = ""                                                    ' 備考

                'ＣＳＶ出力処理
                If CSVObject.Output(CsvData) = 0 Then
                    Return -1
                End If

            Next

        Catch ex As Exception

            'MainLOG.Write("処理結果確認表(自振契約)ＣＳＶ出力", "失敗", ex.Message)
            Return -1
        Finally

        End Try

        Return 0

    End Function

    Private Function GetTenmast(ByVal kinCode As String, ByVal sitCode As String, ByVal db As CASTCommon.MyOracle) As String


        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(db)
        Dim strSitName As String = ""

        Try

            sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' AND SIT_NO_N = '" & sitCode & "'")

            If orareader.DataReader(sql) = True Then
                strSitName = orareader.GetString("SIT_NNAME_N")
                Return strSitName
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function

End Class
