Option Strict On
Option Explicit On

Imports System

Public Class ClsPrnSitenYomikae
    Inherits CAstReports.ClsReportBase

    Private ToriSCode As String         '取引先主コード
    Private ToriFCode As String         '取引先副コード
    Private FuriDate As String          '振込日

    Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KFSP004"

        '定義体名セット
        ReportBaseName = "KFSP004_支店読替明細表.rpd"
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

            '------------------------------------------------------
            'データ出力
            '------------------------------------------------------
            CSVObject.Output(InfoMei.FURIKAE_DATE)                          '振込日
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
            CSVObject.Output(InfoMei.KEIYAKU_KNAME, True)                   '受取人名
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
        Return MyBase.ReportExecute()
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
