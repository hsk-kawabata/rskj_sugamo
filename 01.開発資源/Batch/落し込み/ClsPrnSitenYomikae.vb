Imports System
Imports System.IO
Imports System.Text

Public Class ClsPrnSitenYomikae
    Inherits CAstReports.ClsReportBase

    Private ToriSCode As String         '取引先主コード
    Private ToriFCode As String         '取引先副コード
    Private FuriDate As String          '振替日
    Private DataOK As Boolean = False

    Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KFJP005"

        '定義体名セット
        ReportBaseName = "KFJP005_支店読替明細表.rpd"
    End Sub

    '=====================================================================
    ' 機能　 ：ファイル作成
    ' 備考　 ： 
    ' 作成　 ：2009/09/17
    ' 更新　 ：
    '=====================================================================
    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()
        Return file
    End Function

    '=====================================================================
    ' 機能　 ：ファイル書き込み
    ' 備考　 ： 
    ' 作成　 ：2009/09/17
    ' 更新　 ：
    '=====================================================================
    Public Sub OutputCsvData(ByVal aReadFmt As CAstFormat.CFormat)
        Try
            Dim InfoTori As CAstBatch.CommData.stTORIMAST = aReadFmt.ToriData.INFOToriMast      '取引先情報
            Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast                  '依頼明細情報
            
            'データを１件でも出力すればＯＫフラグを立てる
            DataOK = True

            '------------------------------------------------------
            'データ出力
            '------------------------------------------------------
            CSVObject.Output(InfoMei.FURIKAE_DATE)                          '振替日
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '処理日
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    'タイプスタンプ
            CSVObject.Output(InfoTori.TORIS_CODE_T)                         '取引先主コード
            CSVObject.Output(InfoTori.TORIF_CODE_T)                         '取引先副コード
            CSVObject.Output(InfoTori.ITAKU_NNAME_T, True)                  '取引先名
            
            CSVObject.Output(InfoMei.OLD_KIN_NO)                            '読替前金融機関コード
            CSVObject.Output(InfoMei.OLD_SIT_NO)                            '読替前支店コード
            CSVObject.Output(InfoMei.OLD_KOUZA)                             '読替前口座番号
            CSVObject.Output(InfoMei.KEIYAKU_KIN)                           '読替後金融機関コード
            CSVObject.Output(InfoMei.KEIYAKU_SIT)                           '読替後支店コード
            CSVObject.Output(InfoMei.KEIYAKU_KOUZA)                         '読替前口座番号
            CSVObject.Output(InfoMei.KEIYAKU_KAMOKU)                        '科目
            CSVObject.Output(InfoMei.KEIYAKU_KNAME, True)                   '預金者名
            CSVObject.Output(InfoMei.FURIKIN.ToString)                      '金額
            CSVObject.Output(InfoMei.IDOU_DATE)                             '異動日
            CSVObject.Output("", False)                                     'エラー情報
            CSVObject.Output(InfoMei.RECORD_NO, True, True)                 'レコード番号
            
        Catch ex As Exception
            BatchLog.Write("支店読替明細表ＣＳＶ作成", "失敗", ex.Message)
        End Try
    End Sub


    '=====================================================================
    ' 機能　 ：印刷実行
    ' 備考　 ： 
    ' 作成　 ：2009/09/17
    ' 更新　 ：
    '=====================================================================
    Public Overloads Overrides Function ReportExecute() As Boolean
        'Try
        '    MyBase.CloseCsv()
        '    Dim CSVFileName As String = ""

        '    'If DataOK = False Then
        '    '' データレコードを出力していない場合，レポート出力は行わない
        '    'If File.Exists(CsvName) = True Then
        '    '    File.Delete(CsvName)
        '    'End If

        '    'Return True
        '    'End If

        '    CSVFileName = Path.Combine(Path.GetDirectoryName(CsvName), "KFJP005" & ToriSCode & ToriFCode & FuriDate & ".CSV")

        '    If File.Exists(CSVFileName) = True Then
        '        File.Delete(CSVFileName)
        '    End If

        '    File.Copy(CsvName, CSVFileName)
        'Catch ex As System.Exception
        '    BatchLog.Write("KFJP005_支店読替明細表印刷", "失敗", ex.Message)
        '    Return False
        'End Try

        '2010/02/03 通常使用するプリンタにする ===
        'Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
        Return MyBase.ReportExecute()
        '=======================================================
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
