Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports System.Collections.Generic
Imports System.Collections.Specialized

Public Class ClsPrnKawaseFurikomiMeisai
    Inherits CAstReports.ClsReportBase

    Public CsvData(24) As String

    Private ReportID As String

    Private BaitaiList As New StringDictionary
    Private SyubetuList As New StringDictionary
    Private TenList As New StringDictionary

    Sub New(ByVal ReportID As String, ByVal ReportName As String)
        Me.ReportID = ReportID

        ' CSVファイルセット
        InfoReport.ReportName = ReportID

        ' 定義体名セット
        ReportBaseName = ReportID & "_" & ReportName & ".rpd"
    End Sub

    '
    ' 機能　 ： 為替振込明細表ＣＳＶを出力する
    '
    ' 備考　 ： 
    '
    Public Function OutputCSVKekka(ByVal HassinList As List(Of CAstFormKes.ClsFormKes.KessaiData), ByVal jikinko As String, ByVal HassinDate As String, ByVal strDate As String, ByVal strTime As String) As Integer
        Dim KData As New CAstFormKes.ClsFormKes.KessaiData
        Dim fmt48100 As New CAstFormKes.ClsFormSikinFuri.T_48100
        'Dim fmt10015 As New CAstFormKes.ClsT_RIENT10.T_10015

        Dim cnt As Integer = HassinList.Count - 1 'ループ回数

        Try

            For i As Integer = 0 To cnt

                KData.Init()
                fmt48100.Init()
                'fmt10015.Init()

                KData = HassinList.Item(i)

                Select Case KData.OpeCode
                    Case "48100"
                        '帳票の振分条件
                        If (ReportID = "KFSP010" AndAlso KData.KesKinCode = jikinko) OrElse _
                           (ReportID = "KFSP011" AndAlso KData.KesKinCode <> jikinko) Then

                            fmt48100.Data = KData.record320

                            ' ＣＳＶデータ設定
                            CsvData(0) = (i + 1).ToString("000000")                             ' レコード番号
                            CsvData(1) = strDate                                                ' 処理日
                            CsvData(2) = strTime                                                ' タイムスタンプ
                            CsvData(3) = fmt48100.TORIATUKAI                                    ' 振込日
                            CsvData(4) = KData.TorisCode                                        ' 取引先主コード
                            CsvData(5) = KData.TorifCode                                        ' 取引先副コード
                            CsvData(6) = KData.ToriNName                                        ' 委託者名漢字
                            CsvData(7) = HassinDate                                             ' 発信日
                            CsvData(8) = GetBAITAI(KData.TorisCode, KData.TorifCode)            ' 媒体
                            CsvData(9) = GetSYUBETU(KData.TorisCode, KData.TorifCode)           ' 種別

                            Select Case fmt48100.SYUMOKU                                        ' 適用種別
                                Case "1074" : CsvData(10) = "振込公金"
                                Case "1054" : CsvData(10) = "振込国庫金"
                                Case "1022" : CsvData(10) = "振込一般"

                                Case "1174" : CsvData(10) = "先振公金"
                                Case "1154" : CsvData(10) = "先振国庫金"
                                Case "1122" : CsvData(10) = "先振一般"

                                Case "1271" : CsvData(10) = "給与公金"
                                Case "1251" : CsvData(10) = "給与国庫金"
                                Case "1211" : CsvData(10) = "給与一般"

                                Case "1272" : CsvData(10) = "賞与公金"
                                Case "1252" : CsvData(10) = "賞与国庫金"
                                Case "1212" : CsvData(10) = "賞与一般"

                                Case Else : CsvData(10) = ""
                            End Select

                            CsvData(11) = KData.TorimatomeSit                                   ' 取りまとめ店コード
                            CsvData(12) = GetTENMAST(jikinko, KData.TorimatomeSit)              ' 取りまとめ店名
                            CsvData(13) = fmt48100.UKETORI_NAME.Trim                            ' 受取人
                            CsvData(14) = KData.KesKinCode                                      ' 振込金融機関コード
                            CsvData(15) = KData.KesSitCode                                      ' 振込支店コード

                            If fmt48100.JUSIN_TEN.Trim = "" Then
                                CsvData(16) = ""                                                ' 振込金融機関名
                                CsvData(17) = ""                                                ' 振込支店名
                            Else
                                Dim tenmei() As String = fmt48100.JUSIN_TEN.Trim.Split(" "c)
                                CsvData(16) = tenmei(0).Trim                                    ' 振込金融機関名
                                CsvData(17) = tenmei(1).Trim                                    ' 振込支店名
                            End If

                            Select Case fmt48100.UKETORI_KAMOKU                                 ' 科目名
                                Case "1" : CsvData(18) = "普通"
                                Case "2" : CsvData(18) = "当座"
                                Case "4" : CsvData(18) = "貯蓄"
                                Case "9" : CsvData(18) = "他"
                                Case Else : CsvData(18) = "他"
                            End Select

                            CsvData(19) = fmt48100.UKETORI_KOUZA.Trim                           ' 口座番号
                            CsvData(20) = CLng(fmt48100.KINGAKU).ToString                       ' 振込金額
                            CsvData(21) = fmt48100.KINGAKU_FUGOU.Trim                           ' 複記符号
                            CsvData(22) = KData.FurikomiTesuukin                                ' 手数料
                            CsvData(23) = fmt48100.BIKOU1.Trim                                  ' 備考１
                            CsvData(24) = fmt48100.BIKOU2.Trim                                  ' 備考２

                            'ＣＳＶ出力処理
                            If CSVObject.Output(CsvData) = 0 Then
                                Return -1
                            End If
                        End If

                    Case "10015"
                        'fmt10015.Data = KData.record320

                    Case Else
                End Select
            Next

        Catch ex As Exception
            Return -1
        Finally

        End Try

        Return 0

    End Function

    Private Function GetTENMAST(ByVal kinno As String, ByVal sitno As String) As String
        If TenList.ContainsKey(kinno & sitno) = True Then
            Return TenList(kinno & sitno)
        End If

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            Dim SQL As String = "SELECT SIT_NNAME_N FROM TENMAST WHERE KIN_NO_N = '" & kinno & "' AND SIT_NO_N = '" & sitno & "'"

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) = True Then
                TenList.Add(kinno & sitno, OraReader.GetString("SIT_NNAME_N"))
                Return OraReader.GetString("SIT_NNAME_N")
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

    Private Function GetBAITAI(ByVal toriscode As String, ByVal torifcode As String) As String
        If BaitaiList.ContainsKey(toriscode & torifcode) = True Then
            Return BaitaiList(toriscode & torifcode)
        End If

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            Dim SQL As String = "SELECT BAITAI_CODE_T FROM S_TORIMAST WHERE TORIS_CODE_T = '" & toriscode & "' AND TORIF_CODE_T = '" & torifcode & "'"

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) = True Then
                Dim baitai As String = ""

                ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                '媒体名をテキストから取得する
                baitai = CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_総振_媒体コード.TXT"), _
                                                       OraReader.GetString("BAITAI_CODE_T"))
                'Select Case OraReader.GetString("BAITAI_CODE_T")
                '    Case "00" : baitai = "伝送"
                '    Case "01" : baitai = "FD3.5"
                '    Case "04" : baitai = "依頼書"
                '    Case "05" : baitai = "MT"
                '    Case "06" : baitai = "CMT"
                '    Case "09" : baitai = "伝票"
                '    Case "10" : baitai = "WEB伝送"      '2012/06/30 標準版　WEB伝送対応
                '    Case Else : baitai = ""
                'End Select
                ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END

                BaitaiList.Add(toriscode & torifcode, baitai)
                Return baitai
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

    Private Function GetSYUBETU(ByVal toriscode As String, ByVal torifcode As String) As String
        If SyubetuList.ContainsKey(toriscode & torifcode) = True Then
            Return SyubetuList(toriscode & torifcode)
        End If

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            Dim SQL As String = "SELECT SYUBETU_T FROM S_TORIMAST WHERE TORIS_CODE_T = '" & toriscode & "' AND TORIF_CODE_T = '" & torifcode & "'"

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) = True Then
                Dim syubetu As String = ""

                Select Case OraReader.GetString("SYUBETU_T")
                    Case "21" : syubetu = "総振"
                    Case "11" : syubetu = "給与"
                    Case "12" : syubetu = "賞与"
                    Case Else : syubetu = ""
                End Select

                SyubetuList.Add(toriscode & torifcode, syubetu)
                Return syubetu
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

End Class
