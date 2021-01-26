Imports System
Imports System.IO
Imports Microsoft.VisualBasic.Strings

' インプットエラー
Class ClsPrnInputError
    Inherits CAstReports.ClsReportBase

    Private TextData As New CAstFormat.ClsText

    Private ToriSCode As String         ' 取引先主コード
    Private ToriFCode As String         ' 取引先副コード
    Private FuriDate As String          ' 振替日

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP004"

        ' 定義体名セット
        ReportBaseName = "KFJP004_インプットエラーリスト.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()
        ''2008/11/07 T-Sakai ************************************
        ''FTP送信に変更
        '' タイトル行
        'CSVObject.Output("処理日")
        'CSVObject.Output("タイムスタンプ")
        'CSVObject.Output("取引先主コード")
        'CSVObject.Output("取引先副コード")
        'CSVObject.Output("取引先名")
        'CSVObject.Output("委託者コード")
        'CSVObject.Output("振替日")
        'CSVObject.Output("取りまとめ店")
        'CSVObject.Output("媒体コード")
        'CSVObject.Output("持込種別コード")
        'CSVObject.Output("種別名")
        'CSVObject.Output("コード区分")
        'CSVObject.Output("会社コード")
        'CSVObject.Output("依頼人名")
        'CSVObject.Output("取引金融機関コード")
        'CSVObject.Output("取引金融機関名")
        'CSVObject.Output("取引店舗コード")
        'CSVObject.Output("取引店舗名")
        'CSVObject.Output("依頼人預金種目")
        'CSVObject.Output("依頼人口座番号")
        'CSVObject.Output("レコード番号")
        ''***修正 maeda 2008/05/12*********************************************************
        ''インプットエラー出力項目に金融機関名、店舗名を追加
        'CSVObject.Output("引落金融機関コード")
        'CSVObject.Output("引落金融機関名")
        'CSVObject.Output("引落店番")
        'CSVObject.Output("引落店舗名")
        ''CSVObject.Output("引落金融機関コード")
        ''CSVObject.Output("引落店番")
        ''*********************************************************************************
        'CSVObject.Output("科目")
        'CSVObject.Output("引落口座番号")
        'CSVObject.Output("契約者番号")
        'CSVObject.Output("契約者名")
        'CSVObject.Output("振替金額")
        'CSVObject.Output("エラー情報")
        'CSVObject.Output("合計件数")
        'CSVObject.Output("合計金額")
        'CSVObject.Output("振替済件数")
        'CSVObject.Output("振替済金額")
        'CSVObject.Output("振替不能件数")
        'CSVObject.Output("振替不能金額", False, True)
        '**********************************************************************************
        Return file
    End Function

    'Public Sub PutMem(ByVal value As String, Optional ByVal crlf As Boolean = False)
    '    Call MEMObject.Output(value, True, crlf)
    'End Sub

    'Public Sub PutCsv(ByVal value As String, Optional ByVal crlf As Boolean = False)
    '    Call CSVObject.Output(value, False, crlf)
    'End Sub

    '==================================================================
    ' 機能　 ： インプットエラー情報出力
    ' 引数　 ： aReadFmt - フォーマットクラス / aToriComm - バッチ処理クラス
    ' 備考　 ： 
    ' 作成日 ：2009/09/16
    ' 更新日 ：
    '==================================================================
    'Public Sub OutputMemData(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
    Public Sub OutputData(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
        Dim InfoPara As CAstBatch.CommData.stPARAMETER = aToriComm.INFOParameter    'パラメータ情報
        Dim InfoTori As CAstBatch.CommData.stTORIMAST = aToriComm.INFOToriMast      '取引先情報
        Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast          '依頼明細情報
        Dim InfoMei2 As CAstFormat.CFormat.MEISAI2 = aReadFmt.InfoMeisaiMast2       '依頼明細情報2(金融機関名、店舗名)

        Try
            For i As Integer = 0 To aReadFmt.InErrorArray.Count - 1
                Dim inf As CAstFormat.CFormat.INPUTERROR
                inf = CType(aReadFmt.InErrorArray(i), CAstFormat.CFormat.INPUTERROR)

                '---------------------------------------
                'ヘッダ情報
                '---------------------------------------
                CSVObject.Output(InfoPara.FURI_DATE)                            '振替日
                CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '処理日
                CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    'タイムスタンプ
                CSVObject.Output(InfoTori.TORIS_CODE_T)                         '取引先主コード 
                CSVObject.Output(InfoTori.TORIF_CODE_T)                         '取引先副コード
                CSVObject.Output(InfoTori.ITAKU_NNAME_T, True)                  '取引先名
                CSVObject.Output(InfoTori.TKIN_NO_T)                            '取引金融機関コード
                CSVObject.Output(InfoTori.TKIN_NNAME_N, True)                   '取引金融機関名
                CSVObject.Output(InfoTori.TSIT_NO_T)                            '取引支店コード
                CSVObject.Output(InfoTori.TSIT_NNAME_N, True)                   '取引支店名
                CSVObject.Output(InfoTori.FURI_CODE_T)                          '振替コード
                CSVObject.Output(InfoTori.KIGYO_CODE_T)                         '企業コード

                Select Case InfoTori.CODE_KBN_T                                 'コード区分
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

                '---------------------------------------
                'インプットエラー情報
                '---------------------------------------
                CSVObject.Output(CStr(i + 1))                                   '通番
                CSVObject.Output(inf.KIN)                                       '引落金融機関コード
                CSVObject.Output(InfoMei2.KEIYAKU_KIN_KNAME.Trim, True)         '引落金融機関名
                CSVObject.Output(inf.SIT)                                       '引落支店コード
                CSVObject.Output(InfoMei2.KEIYAKU_SIT_KNAME.Trim, True)         '引落支店名
                CSVObject.Output(inf.KAMOKU)                                    '科目
                CSVObject.Output(inf.KOUZA)                                     '口座番号

                CSVObject.Output(inf.JYUYOKA_NO, True)                          '需要家番号
                'If inf.JYUYOKA_NO.Length > 10 Then                              '契約者番号
                '    CSVObject.Output(Left(inf.JYUYOKA_NO, 10))
                'Else
                '    CSVObject.Output(inf.JYUYOKA_NO)
                'End If

                CSVObject.Output(inf.KNAME, True)                               '契約者名
                CSVObject.Output(inf.FURIKIN)                                   '振替金額
                CSVObject.Output(inf.ERRINFO, True, True)                       'エラーメッセージ
            Next i
            '2018/04/13 FJH) 向井 青梅信金　FAX基本手数料契解約チェック処理追加 --------------------------------------------- START
            If aReadFmt.InfoMeisaiMast.FURIKETU_CODE = 6 Then

                '---------------------------------------
                'ヘッダ情報
                '---------------------------------------
                CSVObject.Output(InfoPara.FURI_DATE)                            '振替日
                CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '処理日
                CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    'タイムスタンプ
                CSVObject.Output(InfoTori.TORIS_CODE_T)                         '取引先主コード 
                CSVObject.Output(InfoTori.TORIF_CODE_T)                         '取引先副コード
                CSVObject.Output(InfoTori.ITAKU_NNAME_T, True)                  '取引先名
                CSVObject.Output(InfoTori.TKIN_NO_T)                            '取引金融機関コード
                CSVObject.Output(InfoTori.TKIN_NNAME_N, True)                   '取引金融機関名
                CSVObject.Output(InfoTori.TSIT_NO_T)                            '取引支店コード
                CSVObject.Output(InfoTori.TSIT_NNAME_N, True)                   '取引支店名
                CSVObject.Output(InfoTori.FURI_CODE_T)                          '振替コード
                CSVObject.Output(InfoTori.KIGYO_CODE_T)                         '企業コード

                Select Case InfoTori.CODE_KBN_T                                 'コード区分
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

                '---------------------------------------
                'インプットエラー情報
                '---------------------------------------
                CSVObject.Output(CStr(1))                                       '通番
                CSVObject.Output(InfoMei.KEIYAKU_KIN)                           '引落金融機関コード
                CSVObject.Output(InfoMei2.KEIYAKU_KIN_KNAME.Trim, True)         '引落金融機関名
                CSVObject.Output(InfoMei.KEIYAKU_SIT)                           '引落支店コード
                CSVObject.Output(InfoMei2.KEIYAKU_SIT_KNAME.Trim, True)         '引落支店名
                CSVObject.Output(InfoMei.KEIYAKU_KAMOKU)                        '科目
                CSVObject.Output(InfoMei.KEIYAKU_KOUZA)                         '口座番号

                CSVObject.Output(InfoMei.JYUYOKA_NO, True)                      '需要家番号
                'If inf.JYUYOKA_NO.Length > 10 Then                             '契約者番号
                '    CSVObject.Output(Left(inf.JYUYOKA_NO, 10))
                'Else
                '    CSVObject.Output(inf.JYUYOKA_NO)
                'End If

                CSVObject.Output(InfoMei.KEIYAKU_KNAME, True)                   '契約者名
                CSVObject.Output(InfoMei.FURIKIN.ToString("0000000000"))        '振替金額

                If aReadFmt.RecordDataNoChange.Substring(38, 1) = "1" Then
                    CSVObject.Output("徴求対象外 契約日：" & aReadFmt.RecordDataNoChange.Substring(112, 8), True, True) 'エラーメッセージ
                Else
                    CSVObject.Output("徴求対象外 解約日：" & aReadFmt.RecordDataNoChange.Substring(112, 8), True, True) 'エラーメッセージ
                End If
            End If
            '2018/04/13 FJH) 向井 青梅信金　FAX基本手数料契解約チェック処理追加 --------------------------------------------- END
        Catch ex As Exception
            BatchLog.Write("(インプットエラー印刷)", "失敗", ex.Message)
        End Try
    End Sub

    '==================================================================
    ' 機能　 ： 印刷実行
    ' 備考　 ： 
    ' 作成日 ：2009/09/16
    ' 更新日 ：
    '==================================================================
    Public Overloads Overrides Function ReportExecute() As Boolean
        'Dim InErrFile As String

        'Try
        '    InErrFile = Path.Combine(Path.GetDirectoryName(CsvName), "INERR" & ToriSCode & ToriFCode & FuriDate & ".CSV")

        '    '前回の出力ファイル削除
        '    If File.Exists(InErrFile) = True Then
        '        File.Delete(InErrFile)
        '    End If

        '    File.Copy(CsvName, InErrFile)

        'Catch ex As System.Exception
        '    BatchLog.Write("(インプットエラー印刷)", "CSVファイルコピー失敗：" & ex.Message)
        'End Try

        '帳票出力処理
        '2010/02/03 通常使用するプリンタにする ===
        'Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
        Return MyBase.ReportExecute()
        '=======================================================
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        TextData = Nothing
    End Sub






    ' ''
    '' 機能　 ： インプットエラー情報にトレーラ情報を付加する
    ''
    '' 備考　 ： 
    ''
    'Public Sub OutputCsvData(ByVal aReadFmt As CAstFormat.CFormat)
    '' 依頼明細情報
    'Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast
    'Dim Memory As CAstReports.MEM = MEMObject

    '' 先頭に位置づけ
    'Memory.Seek(0)
    'Dim LineData As String = Memory.ReadLine

    'If Not LineData Is Nothing Then
    '    Tenmei = aReadFmt.ToriData.INFOToriMast.TSIT_NNAME_N
    '    Itakusyamei = aReadFmt.ToriData.INFOToriMast.ITAKU_NNAME_T
    '    Hiduke = aReadFmt.ToriData.INFOParameter.FURI_DATE
    '    CreateCsvFile()
    'End If

    ''Do Until LineData Is Nothing
    ''    PutCsv(LineData)
    ''    PutCsv(InfoMei.TOTAL_IRAI_KEN.ToString)    '合計件数
    ''    PutCsv(InfoMei.TOTAL_IRAI_KIN.ToString)    '合計金額
    ''    PutCsv(InfoMei.TOTAL_ZUMI_KEN.ToString)    '振替済件数
    ''    PutCsv(InfoMei.TOTAL_ZUMI_KIN.ToString)    '振替済金額
    ''    PutCsv(InfoMei.TOTAL_FUNO_KEN.ToString)    '振替不能件数
    ''    PutCsv(InfoMei.TOTAL_FUNO_KIN.ToString)    '振替不能金額
    ''    PutCsv("", True)

    ''Loop
    'End Sub

End Class
