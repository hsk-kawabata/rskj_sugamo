Option Strict On
Option Explicit On

Imports System
Imports System.Text

' 受付明細表
Class ClsPrnUketukeMeisai
    Inherits CAstReports.ClsReportBase

    Private ToriSCode As String         '取引先主コード
    Private ToriFCode As String         '取引先副コード
    Private FuriDate As String          '振替日

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFSP002"

        ' 定義体名セット
        ReportBaseName = "KFSP002_受付明細表.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        Return file
    End Function

    '
    ' 機能　 ： 受付明細表をデータに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal aReadFmt As CAstFormat.CFormat)
        Try
            Dim OraReader As CASTCommon.MyOracleReader
            Dim InfoPara As CAstBatch.CommData.stPARAMETER = aReadFmt.ToriData.INFOParameter    'パラメータ情報
            Dim InfoTori As CAstBatch.CommData.stTORIMAST = aReadFmt.ToriData.INFOToriMast      '取引先情報
            Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast                  '依頼明細情報

            ToriSCode = InfoTori.TORIS_CODE_T
            ToriFCode = InfoTori.TORIF_CODE_T
            FuriDate = InfoMei.FURIKAE_DATE

            '----------------------------------------------------
            'データ出力
            '----------------------------------------------------
            CSVObject.Output(InfoMei.FURIKAE_DATE)                          '振込日
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '処理日
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    'タイプスタンプ
            CSVObject.Output(InfoTori.TORIS_CODE_T)                         '取引先主コード
            CSVObject.Output(InfoTori.TORIF_CODE_T)                         '取引先副コード
            CSVObject.Output(InfoTori.ITAKU_NNAME_T, True)                  '取引先名
            CSVObject.Output(InfoTori.TKIN_NO_T)                            '取扱金融機関コード
            CSVObject.Output(InfoTori.TKIN_NNAME_N, True)                   '取扱金融機関名
            CSVObject.Output(InfoTori.TSIT_NO_T)                            '取扱支店コード
            CSVObject.Output(InfoTori.TSIT_NNAME_N, True)                   '取扱支店名
            CSVObject.Output(InfoTori.SYUBETU_T)                            '種別

            Select Case InfoPara.CODE_KBN                                   'コード区分
                Case "0"
                    CSVObject.Output("JIS")
                Case "1"
                    CSVObject.Output("JIS改有(120)")
                Case "2"
                    CSVObject.Output("JIS改有(119)")
                Case "3"
                    CSVObject.Output("JIS改有(118)")
                Case "4"
                    CSVObject.Output("EBCDIC")
            End Select

            If aReadFmt.InErrorArray.Count = 0 Then                         'エラーメッセージ
                CSVObject.Output("")
            Else
                CSVObject.Output(CType(aReadFmt.InErrorArray(0), CAstFormat.CFormat.INPUTERROR).ERRINFO, True)
            End If

            OraReader = aReadFmt.GetTENMAST(InfoMei.KEIYAKU_KIN, InfoMei.KEIYAKU_SIT)

            If OraReader Is Nothing OrElse OraReader.EOF = True OrElse _
                (Not OraReader Is Nothing AndAlso OraReader.GetItem("SIT_N") = "NG") Then
                CSVObject.Output(InfoMei.KEIYAKU_KIN)                       '金融機関コード
                CSVObject.Output("")                                        '金融機関名
                CSVObject.Output(InfoMei.KEIYAKU_SIT)                       '支店コード
                CSVObject.Output("")                                        '支店名
            Else
                CSVObject.Output(InfoMei.KEIYAKU_KIN)                       '金融機関コード
                CSVObject.Output(OraReader.GetValue(0), True)               '金融機関名
                CSVObject.Output(InfoMei.KEIYAKU_SIT)                       '支店コード
                CSVObject.Output(OraReader.GetValue(1), True)               '支店名
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If

            CSVObject.Output(InfoMei.KEIYAKU_KAMOKU)                        '科目
            CSVObject.Output(InfoMei.KEIYAKU_KOUZA, True)                   '口座番号
            CSVObject.Output(InfoMei.KEIYAKU_KNAME, True)                   '預金者名
            CSVObject.Output(InfoMei.FURIKIN.ToString)                      '金額
            CSVObject.Output("", True)                                      '備考


            '改ページ文字設定(店番ソートの場合は金融機関／支店コードを設定)
            ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- START
            'If InfoTori.UMEISAI_KBN_T = "1" Then
            '    CSVObject.Output(InfoMei.KEIYAKU_KIN & _
            '                     InfoMei.KEIYAKU_SIT, False, True)          '改ページキー(店番ソート)
            'Else
            '    CSVObject.Output("0000000", False, True)                    '改ページキー(その他)
            'End If
            If InfoTori.UMEISAI_KBN_T = "1" Then
                CSVObject.Output(InfoMei.KEIYAKU_KIN & _
                                 InfoMei.KEIYAKU_SIT)          '改ページキー(店番ソート)
            Else
                CSVObject.Output("0000000")                    '改ページキー(その他)
            End If
            ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- END

            ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- START
            CSVObject.Output(InfoMei.RECORD_NO.ToString.PadLeft(6), False, True)     'レコード番号出力
            ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- END

        Catch ex As Exception
            BatchLog.Write("(受付明細表印刷)", "失敗", ex.Message)

        End Try
    End Sub

    Public Sub SortData(ByVal SortParam As String)
        '---------------------------------------------------------------------------------------
        'データソート処理
        '---------------------------------------------------------------------------------------
        Try
            ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- START
            ''ソート順：取引先主コード、取引先副コード、金融機関コード、支店コード
            'Call SortFile("3.10sjia 4.2sjia 13.4sjia 15.3sjia")
            Call SortFile(SortParam)
            ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- END

        Catch ex As Exception
            BatchLog.Write("(受付明細表印刷)ソート", "失敗", ex.Message)
        Finally
        End Try
    End Sub

    '
    ' 機能　 ： 印刷実行
    '
    ' 備考　 ： 
    '
    Public Overloads Overrides Function ReportExecute() As Boolean
        Try
            MyBase.CloseCsv()
            Return MyBase.ReportExecute()
        Catch ex As System.Exception
            BatchLog.Write("(受付明細表印刷)印刷実行", "失敗", ex.Message)

        End Try
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class

