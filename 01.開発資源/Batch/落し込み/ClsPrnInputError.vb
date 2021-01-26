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

    '2017/02/27 タスク）西野 ADD 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- START
    Private STR_EDITION As String = ""
    '2017/02/27 タスク）西野 ADD 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- END

    '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- START
    Sub New(ByVal EDITION As String)

        STR_EDITION = EDITION

        If EDITION = "2" Then
            '大規模版の帳票

            ' CSVファイルセット
            InfoReport.ReportName = "KFJP061"

            ' 定義体名セット
            ReportBaseName = "KFJP061_インプットエラーリスト.rpd"
        Else
            ' CSVファイルセット
            InfoReport.ReportName = "KFJP004"

            ' 定義体名セット
            ReportBaseName = "KFJP004_インプットエラーリスト.rpd"
        End If
    End Sub
    'Sub New()
    '    ' CSVファイルセット
    '    InfoReport.ReportName = "KFJP004"

    '    ' 定義体名セット
    '    ReportBaseName = "KFJP004_インプットエラーリスト.rpd"
    'End Sub
    '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- END

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
    '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- START
    '処理復活
    Public Sub PutMem(ByVal value As String, Optional ByVal crlf As Boolean = False)
        Call MEMObject.Output(value, True, crlf)
    End Sub

    Public Sub PutCsv(ByVal value As String, Optional ByVal crlf As Boolean = False)
        Call CSVObject.Output(value, False, crlf)
    End Sub
    '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- END

    '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- START
    'メモリストリーム書き出しに変更（大規模版の処理に合わせる）
    Public Sub OutputMemData(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
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
                PutMem(InfoPara.FURI_DATE)                            '振替日
                PutMem(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '処理日
                PutMem(CASTCommon.Calendar.Now.ToString("HHmmss"))    'タイムスタンプ
                PutMem(InfoTori.TORIS_CODE_T)                         '取引先主コード 
                PutMem(InfoTori.TORIF_CODE_T)                         '取引先副コード
                PutMem(InfoTori.ITAKU_NNAME_T)                  '取引先名
                PutMem(InfoTori.TKIN_NO_T)                            '取引金融機関コード
                PutMem(InfoTori.TKIN_NNAME_N)                   '取引金融機関名
                PutMem(InfoTori.TSIT_NO_T)                            '取引支店コード
                PutMem(InfoTori.TSIT_NNAME_N)                   '取引支店名
                PutMem(InfoTori.FURI_CODE_T)                          '振替コード
                PutMem(InfoTori.KIGYO_CODE_T)                         '企業コード

                Select Case InfoTori.CODE_KBN_T                                 'コード区分
                    Case "0"
                        PutMem("JIS")
                    Case "1"
                        PutMem("JIS改有(120)")
                    Case "2"
                        PutMem("JIS改有(119)")
                    Case "3"
                        PutMem("JIS改有(118)")
                    Case "4"
                        PutMem("EBCDIC")
                End Select

                '2017/02/27 タスク）西野 ADD 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- START
                If STR_EDITION = "2" Then
                    '---------------------------------------
                    '依頼ファイル　ヘッダー情報
                    '---------------------------------------
                    PutMem(TextData.GetBaitaiCode(InfoTori.BAITAI_CODE_T))  ' 媒体コード
                    PutMem(InfoMei.SYUBETU_CODE)                            ' 持込種別コード
                    Select Case InfoMei.SYUBETU_CODE                        ' 持込種別名
                        Case "11"
                            PutMem("給与振込(民間)")
                        Case "12"
                            PutMem("賞与振込(民間)")
                        Case "71"
                            PutMem("給与振込(公務員)")
                        Case "72"
                            PutMem("賞与振込(公務員)")
                        Case "21"
                            PutMem("総合振込")
                        Case "41"
                            PutMem("株式配当金振込")
                        Case "43"
                            PutMem("年金信託契約一時金給付振込")
                        Case "44"
                            PutMem("公的年金一時金給付金振込")
                        Case "45"
                            PutMem("医療保険の給付振込")
                        Case "91"
                            PutMem("預金口座振替")
                        Case Else
                            PutMem("")
                    End Select
                    PutMem(InfoMei.ITAKU_CODE)                              ' 会社コード
                    PutMem(InfoMei.ITAKU_KNAME)                             ' 依頼人名
                    PutMem(InfoMei.ITAKU_SIT)                               ' 取引店舗コード
                    PutMem(InfoMei.I_SIT_NNAME)                             ' 取引店舗名
                    PutMem(CASTCommon.ConvertKamoku2TO1(InfoMei.ITAKU_KAMOKU))                            ' 依頼人預金種目
                    PutMem(InfoMei.ITAKU_KOUZA)                             ' 依頼人口座番号
                End If
                '2017/02/27 タスク）西野 ADD 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- END

                '---------------------------------------
                'インプットエラー情報
                '---------------------------------------
                PutMem(CStr(i + 1))                                   '通番
                PutMem(inf.KIN)                                       '引落金融機関コード
                PutMem(InfoMei2.KEIYAKU_KIN_KNAME.Trim)         '引落金融機関名
                PutMem(inf.SIT)                                       '引落支店コード
                PutMem(InfoMei2.KEIYAKU_SIT_KNAME.Trim)         '引落支店名
                PutMem(inf.KAMOKU)                                    '科目
                PutMem(inf.KOUZA)                                     '口座番号

                PutMem(inf.JYUYOKA_NO)                          '需要家番号
                'If inf.JYUYOKA_NO.Length > 10 Then                              '契約者番号
                '    PutMem(Left(inf.JYUYOKA_NO, 10))
                'Else
                '    PutMem(inf.JYUYOKA_NO)
                'End If

                PutMem(inf.KNAME)                               '契約者名
                PutMem(inf.FURIKIN)                                   '振替金額
                PutMem(inf.ERRINFO, True)                             'エラーメッセージ
            Next i

        Catch ex As Exception
            BatchLog.Write("(インプットエラー印刷)", "失敗", ex.Message)
        End Try
    End Sub
    ''==================================================================
    '' 機能　 ： インプットエラー情報出力
    '' 引数　 ： aReadFmt - フォーマットクラス / aToriComm - バッチ処理クラス
    '' 備考　 ： 
    '' 作成日 ：2009/09/16
    '' 更新日 ：
    ''==================================================================
    ''Public Sub OutputMemData(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
    'Public Sub OutputData(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
    '    Dim InfoPara As CAstBatch.CommData.stPARAMETER = aToriComm.INFOParameter    'パラメータ情報
    '    Dim InfoTori As CAstBatch.CommData.stTORIMAST = aToriComm.INFOToriMast      '取引先情報
    '    Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast          '依頼明細情報
    '    Dim InfoMei2 As CAstFormat.CFormat.MEISAI2 = aReadFmt.InfoMeisaiMast2       '依頼明細情報2(金融機関名、店舗名)

    '    Try
    '        For i As Integer = 0 To aReadFmt.InErrorArray.Count - 1
    '            Dim inf As CAstFormat.CFormat.INPUTERROR
    '            inf = CType(aReadFmt.InErrorArray(i), CAstFormat.CFormat.INPUTERROR)

    '            '---------------------------------------
    '            'ヘッダ情報
    '            '---------------------------------------
    '            CSVObject.Output(InfoPara.FURI_DATE)                            '振替日
    '            CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '処理日
    '            CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    'タイムスタンプ
    '            CSVObject.Output(InfoTori.TORIS_CODE_T)                         '取引先主コード 
    '            CSVObject.Output(InfoTori.TORIF_CODE_T)                         '取引先副コード
    '            CSVObject.Output(InfoTori.ITAKU_NNAME_T, True)                  '取引先名
    '            CSVObject.Output(InfoTori.TKIN_NO_T)                            '取引金融機関コード
    '            CSVObject.Output(InfoTori.TKIN_NNAME_N, True)                   '取引金融機関名
    '            CSVObject.Output(InfoTori.TSIT_NO_T)                            '取引支店コード
    '            CSVObject.Output(InfoTori.TSIT_NNAME_N, True)                   '取引支店名
    '            CSVObject.Output(InfoTori.FURI_CODE_T)                          '振替コード
    '            CSVObject.Output(InfoTori.KIGYO_CODE_T)                         '企業コード

    '            Select Case InfoTori.CODE_KBN_T                                 'コード区分
    '                Case "0"
    '                    CSVObject.Output("JIS")
    '                Case "1"
    '                    CSVObject.Output("JIS改有(120)")
    '                Case "2"
    '                    CSVObject.Output("JIS改有(119)")
    '                Case "3"
    '                    CSVObject.Output("JIS改有(118)")
    '                Case "4"
    '                    CSVObject.Output("EBCDIC")
    '            End Select

    '            '---------------------------------------
    '            'インプットエラー情報
    '            '---------------------------------------
    '            CSVObject.Output(CStr(i + 1))                                   '通番
    '            CSVObject.Output(inf.KIN)                                       '引落金融機関コード
    '            CSVObject.Output(InfoMei2.KEIYAKU_KIN_KNAME.Trim, True)         '引落金融機関名
    '            CSVObject.Output(inf.SIT)                                       '引落支店コード
    '            CSVObject.Output(InfoMei2.KEIYAKU_SIT_KNAME.Trim, True)         '引落支店名
    '            CSVObject.Output(inf.KAMOKU)                                    '科目
    '            CSVObject.Output(inf.KOUZA)                                     '口座番号

    '            CSVObject.Output(inf.JYUYOKA_NO, True)                          '需要家番号
    '            'If inf.JYUYOKA_NO.Length > 10 Then                              '契約者番号
    '            '    CSVObject.Output(Left(inf.JYUYOKA_NO, 10))
    '            'Else
    '            '    CSVObject.Output(inf.JYUYOKA_NO)
    '            'End If

    '            CSVObject.Output(inf.KNAME, True)                               '契約者名
    '            CSVObject.Output(inf.FURIKIN)                                   '振替金額
    '            CSVObject.Output(inf.ERRINFO, True, True)                       'エラーメッセージ
    '        Next i

    '    Catch ex As Exception
    '        BatchLog.Write("(インプットエラー印刷)", "失敗", ex.Message)
    '    End Try
    'End Sub
    '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- END

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


    '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- START
    ''
    ' 機能　 ： インプットエラー情報にトレーラ情報を付加する
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal aReadFmt As CAstFormat.CFormat)
        ' 依頼明細情報
        Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast
        Dim Memory As CAstReports.MEM = MEMObject

        ' 先頭に位置づけ
        Memory.Seek(0)
        Dim LineData As String = Memory.ReadLine

        If Not LineData Is Nothing Then
            Tenmei = aReadFmt.ToriData.INFOToriMast.TSIT_NNAME_N
            Itakusyamei = aReadFmt.ToriData.INFOToriMast.ITAKU_NNAME_T
            Hiduke = aReadFmt.ToriData.INFOParameter.FURI_DATE
            CreateCsvFile()
        End If

        Do Until LineData Is Nothing
            '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- START
            If STR_EDITION = "2" Then
                '大規模版
                PutCsv(LineData)
                PutCsv(InfoMei.TOTAL_IRAI_KEN.ToString)    '合計件数
                PutCsv(InfoMei.TOTAL_IRAI_KIN.ToString)    '合計金額
                PutCsv(InfoMei.TOTAL_ZUMI_KEN.ToString)    '振替済件数
                PutCsv(InfoMei.TOTAL_ZUMI_KIN.ToString)    '振替済金額
                PutCsv(InfoMei.TOTAL_FUNO_KEN.ToString)    '振替不能件数
                PutCsv(InfoMei.TOTAL_FUNO_KIN.ToString)    '振替不能金額
                PutCsv("", True)
            Else
                '標準版
                PutCsv(LineData, True)

            End If
            'PutCsv(LineData)
            'PutCsv(InfoMei.TOTAL_IRAI_KEN.ToString)    '合計件数
            'PutCsv(InfoMei.TOTAL_IRAI_KIN.ToString)    '合計金額
            'PutCsv(InfoMei.TOTAL_ZUMI_KEN.ToString)    '振替済件数
            'PutCsv(InfoMei.TOTAL_ZUMI_KIN.ToString)    '振替済金額
            'PutCsv(InfoMei.TOTAL_FUNO_KEN.ToString)    '振替不能件数
            'PutCsv(InfoMei.TOTAL_FUNO_KIN.ToString)    '振替不能金額
            'PutCsv("", True)
            '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- END
            LineData = Memory.ReadLine()
        Loop
    End Sub
    '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- END

End Class
