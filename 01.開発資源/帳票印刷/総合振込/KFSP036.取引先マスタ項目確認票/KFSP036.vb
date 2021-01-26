Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports MenteCommon.clsCommon
Public Class KFSP036
    Inherits CAstReports.ClsReportBase

    Private Structure strcInshizeiLabelString
        Dim Inshizei1 As String
        Dim Inshizei2 As String
        Dim Inshizei3 As String

        Public Sub Init()
            Inshizei1 = "1万円未満"
            Inshizei2 = "1万円以上3万円未満"
            Inshizei3 = "3万円以上"
        End Sub
    End Structure
    Private InshizeiLabel As strcInshizeiLabelString

#Region "振込手数料"
    Private Structure strcTesuu
        Dim KijyunIDName As String
        Dim A1 As Long
        Dim A2 As Long
        Dim A3 As Long
        Dim B1 As Long
        Dim B2 As Long
        Dim B3 As Long
        Dim C1 As Long
        Dim C2 As Long
        Dim C3 As Long

        Public Sub Init()
            KijyunIDName = ""
            A1 = 0
            A2 = 0
            A3 = 0
            B1 = 0
            B2 = 0
            B3 = 0
            C1 = 0
            C2 = 0
            C3 = 0
        End Sub
    End Structure
    Private TESUU As strcTesuu
#End Region

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFSP036"

        If RSV2_MASTPTN = "2" Then
            ' 定義体名セット
            ReportBaseName = "KFSP036_取引先マスタ項目確認票(MASTPTN2).rpd"
        Else
            ' 定義体名セット
            ReportBaseName = "KFSP036_取引先マスタ項目確認票.rpd"
        End If
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        'タイトル行
        If RSV2_MASTPTN = "2" Then
            CSVObject.Output("システム日付")
            CSVObject.Output("タイムスタンプ")
            CSVObject.Output("ログイン名")
            CSVObject.Output("端末名")
            CSVObject.Output("取引先主コード")
            CSVObject.Output("取引先副コード")
            CSVObject.Output("媒体コード")
            CSVObject.Output("ラベル区分")
            CSVObject.Output("コード区分")
            CSVObject.Output("代表委託者コード")
            CSVObject.Output("期日管理")
            CSVObject.Output("サイクル管理")
            CSVObject.Output("ファイル名")
            CSVObject.Output("フォーマット区分")
            CSVObject.Output("マルチ区分")
            CSVObject.Output("入出金区分")
            CSVObject.Output("種別")
            CSVObject.Output("委託者コード")
            CSVObject.Output("委託者名カナ")
            CSVObject.Output("取扱金融機関コード")
            CSVObject.Output("取扱金融機関名")
            CSVObject.Output("取扱支店コード")
            CSVObject.Output("取扱支店名")
            CSVObject.Output("科目")
            CSVObject.Output("口座番号")
            CSVObject.Output("送信区分")
            CSVObject.Output("センター区分")
            CSVObject.Output("種目コード")
            CSVObject.Output("付加コード")
            CSVObject.Output("委託者名漢字")
            CSVObject.Output("郵便番号")
            CSVObject.Output("電話番号")
            CSVObject.Output("ＦＡＸ番号")
            CSVObject.Output("顧客番号")
            CSVObject.Output("関連企業情報")
            CSVObject.Output("委託者住所漢字")
            CSVObject.Output("郵送先名カナ")
            CSVObject.Output("郵送先名漢字")
            CSVObject.Output("１日")
            CSVObject.Output("２日")
            CSVObject.Output("３日")
            CSVObject.Output("４日")
            CSVObject.Output("５日")
            CSVObject.Output("６日")
            CSVObject.Output("７日")
            CSVObject.Output("８日")
            CSVObject.Output("９日")
            CSVObject.Output("１０日")
            CSVObject.Output("１１日")
            CSVObject.Output("１２日")
            CSVObject.Output("１３日")
            CSVObject.Output("１４日")
            CSVObject.Output("１５日")
            CSVObject.Output("１６日")
            CSVObject.Output("１７日")
            CSVObject.Output("１８日")
            CSVObject.Output("１９日")
            CSVObject.Output("２０日")
            CSVObject.Output("２１日")
            CSVObject.Output("２２日")
            CSVObject.Output("２３日")
            CSVObject.Output("２４日")
            CSVObject.Output("２５日")
            CSVObject.Output("２６日")
            CSVObject.Output("２７日")
            CSVObject.Output("２８日")
            CSVObject.Output("２９日")
            CSVObject.Output("３０日")
            CSVObject.Output("３１日")
            CSVObject.Output("振込休日シフト")
            CSVObject.Output("１月")
            CSVObject.Output("２月")
            CSVObject.Output("３月")
            CSVObject.Output("４月")
            CSVObject.Output("５月")
            CSVObject.Output("６月")
            CSVObject.Output("７月")
            CSVObject.Output("８月")
            CSVObject.Output("９月")
            CSVObject.Output("１０月")
            CSVObject.Output("１１月")
            CSVObject.Output("１２月")
            CSVObject.Output("契約日")
            CSVObject.Output("開始年月")
            CSVObject.Output("終了年月")
            CSVObject.Output("持込期日")
            CSVObject.Output("日数／基準日(依頼書)")
            CSVObject.Output("日付区分(依頼書)")
            CSVObject.Output("依頼書休日シフト")
            CSVObject.Output("依頼書種別")
            CSVObject.Output("依頼書出力順")
            CSVObject.Output("定額区分")
            CSVObject.Output("受付明細表出力区分")
            CSVObject.Output("印刷部数")
            CSVObject.Output("決済区分")
            CSVObject.Output("資金確保方法")
            CSVObject.Output("とりまとめ店コード")
            CSVObject.Output("とりまとめ店名")
            CSVObject.Output("本部別段口座番号")
            CSVObject.Output("日数／基準日(決済)")
            CSVObject.Output("日付区分(決済)")
            CSVObject.Output("決済休日シフト")
            CSVObject.Output("決済金融機関コード")
            CSVObject.Output("決済金融機関名")
            CSVObject.Output("決済支店コード")
            CSVObject.Output("決済支店名")
            CSVObject.Output("決済科目")
            CSVObject.Output("決済口座番号")
            CSVObject.Output("決済名義人(カナ)")
            CSVObject.Output("備考１")
            CSVObject.Output("備考２")
            CSVObject.Output("手数料徴求支店コード")
            CSVObject.Output("手数料徴求支店名")
            CSVObject.Output("手数料徴求科目")
            CSVObject.Output("手数料徴求口座番号")
            CSVObject.Output("手数料徴求区分")
            CSVObject.Output("手数料徴求方法")
            CSVObject.Output("手数料集計周期")
            CSVObject.Output("日数／基準日(手数料徴求)")
            CSVObject.Output("日付区分(手数料徴求)")
            CSVObject.Output("手数料徴求休日シフト")
            CSVObject.Output("送料")
            CSVObject.Output("固定手数料１")
            CSVObject.Output("固定手数料２")
            CSVObject.Output("手数料徴求対象区分")
            CSVObject.Output("集計基準月")
            CSVObject.Output("集計終了日")
            CSVObject.Output("集計基準")
            CSVObject.Output("集計方法")
            CSVObject.Output("集計企業ＧＲＰ")
            CSVObject.Output("振込手数料基準ＩＤ")
            CSVObject.Output("自店１万円未満")
            CSVObject.Output("自店１万円以上３万円未満")
            CSVObject.Output("自店３万円以上")
            CSVObject.Output("本支店１万円未満")
            CSVObject.Output("本支店１万円以上３万円未満")
            CSVObject.Output("本支店３万円以上")
            CSVObject.Output("他行１万円未満")
            CSVObject.Output("他行１万円以上３万円未満")
            CSVObject.Output("他行３万円以上")
            CSVObject.Output("暗号化処理区分")
            CSVObject.Output("ＡＥＳオプション")
            CSVObject.Output("暗号化キー１")
            CSVObject.Output("暗号化キー２")
            CSVObject.Output("摘要区分")
            CSVObject.Output("カナ摘要")
            CSVObject.Output("漢字摘要")
            CSVObject.Output("振替コード")
            CSVObject.Output("企業コード")
            CSVObject.Output("印紙税１")
            CSVObject.Output("印紙税２")
            CSVObject.Output("印紙税３")
            CSVObject.Output("相手センター確認ＣＤ")
            CSVObject.Output("当方センター確認ＣＤ")
            CSVObject.Output("伝送ファイルＩＤ")
            CSVObject.Output("照合要否区分")
            CSVObject.Output("個別前処理")
            CSVObject.Output("特記事項")
            CSVObject.Output("受付特記事項")
            CSVObject.Output("返却特記事項")
            CSVObject.Output("搬送特記事項")
            CSVObject.Output("給与摘要区分")
        Else
            CSVObject.Output("システム日付")
            CSVObject.Output("タイムスタンプ")
            CSVObject.Output("ログイン名")
            CSVObject.Output("端末名")
            CSVObject.Output("取引先主コード")
            CSVObject.Output("取引先副コード")
            CSVObject.Output("媒体コード")
            CSVObject.Output("ラベル区分")
            CSVObject.Output("コード区分")
            CSVObject.Output("代表委託者コード")
            CSVObject.Output("期日管理")
            CSVObject.Output("サイクル管理")
            CSVObject.Output("ファイル名")
            CSVObject.Output("フォーマット区分")
            CSVObject.Output("マルチ区分")
            CSVObject.Output("入出金区分")
            CSVObject.Output("種別")
            CSVObject.Output("委託者コード")
            CSVObject.Output("委託者名カナ")
            CSVObject.Output("取扱金融機関コード")
            CSVObject.Output("取扱金融機関名")
            CSVObject.Output("取扱支店コード")
            CSVObject.Output("取扱支店名")
            CSVObject.Output("科目")
            CSVObject.Output("口座番号")
            CSVObject.Output("送信区分")
            CSVObject.Output("センター区分")
            CSVObject.Output("種目コード")
            CSVObject.Output("付加コード")
            CSVObject.Output("委託者名漢字")
            CSVObject.Output("郵便番号")
            CSVObject.Output("電話番号")
            CSVObject.Output("ＦＡＸ番号")
            CSVObject.Output("顧客番号")
            CSVObject.Output("関連企業情報")
            CSVObject.Output("委託者住所漢字")
            CSVObject.Output("郵送先名カナ")
            CSVObject.Output("郵送先名漢字")
            CSVObject.Output("振込日")
            CSVObject.Output("振込休日シフト")
            CSVObject.Output("月別処理フラグ（振込・資金決済）")
            CSVObject.Output("契約日")
            CSVObject.Output("開始年月")
            CSVObject.Output("終了年月")
            CSVObject.Output("持込期日")
            CSVObject.Output("日数／基準日(依頼書)")
            CSVObject.Output("日付区分(依頼書)")
            CSVObject.Output("依頼書休日シフト")
            CSVObject.Output("依頼書種別")
            CSVObject.Output("依頼書出力順")
            CSVObject.Output("定額区分")
            CSVObject.Output("受付明細表出力区分")
            CSVObject.Output("印刷部数")
            CSVObject.Output("決済区分")
            CSVObject.Output("資金確保方法")
            CSVObject.Output("とりまとめ店コード")
            CSVObject.Output("とりまとめ店名")
            CSVObject.Output("本部別段口座番号")
            CSVObject.Output("日数／基準日(決済)")
            CSVObject.Output("日付区分(決済)")
            CSVObject.Output("決済休日シフト")
            CSVObject.Output("決済金融機関コード")
            CSVObject.Output("決済金融機関名")
            CSVObject.Output("決済支店コード")
            CSVObject.Output("決済支店名")
            CSVObject.Output("決済科目")
            CSVObject.Output("決済口座番号")
            CSVObject.Output("決済名義人(カナ)")
            CSVObject.Output("備考１")
            CSVObject.Output("備考２")
            CSVObject.Output("手数料徴求支店コード")
            CSVObject.Output("手数料徴求支店名")
            CSVObject.Output("手数料徴求科目")
            CSVObject.Output("手数料徴求口座番号")
            CSVObject.Output("手数料徴求区分")
            CSVObject.Output("手数料徴求方法")
            CSVObject.Output("手数料集計周期")
            CSVObject.Output("日数／基準日(手数料徴求)")
            CSVObject.Output("日付区分(手数料徴求)")
            CSVObject.Output("手数料徴求休日シフト")
            CSVObject.Output("送料")
            CSVObject.Output("固定手数料１")
            CSVObject.Output("固定手数料２")
            CSVObject.Output("手数料徴求対象区分")
            CSVObject.Output("集計基準月")
            CSVObject.Output("集計終了日")
            CSVObject.Output("集計基準")
            CSVObject.Output("集計方法")
            CSVObject.Output("集計企業ＧＲＰ")
            CSVObject.Output("振込手数料基準ＩＤ")
            CSVObject.Output("自店１万円未満")
            CSVObject.Output("自店１万円以上３万円未満")
            CSVObject.Output("自店３万円以上")
            CSVObject.Output("本支店１万円未満")
            CSVObject.Output("本支店１万円以上３万円未満")
            CSVObject.Output("本支店３万円以上")
            CSVObject.Output("他行１万円未満")
            CSVObject.Output("他行１万円以上３万円未満")
            CSVObject.Output("他行３万円以上")
            CSVObject.Output("暗号化処理区分")
            CSVObject.Output("ＡＥＳオプション")
            CSVObject.Output("暗号化キー１")
            CSVObject.Output("暗号化キー２")
            CSVObject.Output("摘要区分")
            CSVObject.Output("カナ摘要")
            CSVObject.Output("漢字摘要")
            CSVObject.Output("振替コード")
            CSVObject.Output("企業コード")
            CSVObject.Output("印紙税１")
            CSVObject.Output("印紙税２")
            CSVObject.Output("印紙税３")
        End If
        CSVObject.Output("", , True)

        Return file
    End Function

    '
    ' 機能　 ： CSVファイルに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    '
    ' 機能　 ： 印刷データの作成
    '
    ' 備考　 ： 
    '
    Public Function MakeRecord() As Boolean
        Dim oraDB As New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim PATH_TXT As String = CASTCommon.GetFSKJIni("COMMON", "TXT")
        Dim KINKO As String = ""
        Dim SITEN As String = ""
        Dim JIKINKO As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
        Dim Center As String = CASTCommon.GetFSKJIni("COMMON", "CENTER")

        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM ")
            If RSV2_MASTPTN = "2" Then
                SQL.Append("     S_TORIMAST_VIEW")
            Else
                SQL.Append("     S_TORIMAST")
            End If
            SQL.Append(" WHERE ")
            SQL.Append("     FSYORI_KBN_T = '3'")
            SQL.Append(" AND TORIS_CODE_T = " & SQ(strToriSCd))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(strToriFCd))

            If oraReader.DataReader(SQL) = True Then

                While oraReader.EOF = False

                    OutputCsvData(Today.ToString("yyyyMMdd"), True)                                 'システム日付
                    OutputCsvData(Now.ToString("HHmmss"), True)                                     'タイムスタンプ
                    OutputCsvData(strUserId, True)                                                  'ログイン名
                    OutputCsvData(Environment.MachineName, True)                                    '端末名
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TORIS_CODE_T")), True)            '取引先主コード
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TORIF_CODE_T")), True)            '取引先副コード
                    '媒体名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_総振_媒体コード.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("BAITAI_CODE_T"))), True)
                    'ラベル区分名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_ラベル区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("LABEL_KBN_T"))), True)
                    'コード区分名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_コード区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("CODE_KBN_T"))), True)
                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_KANRI_CODE_T")), True)      '代表委託者コード

                    '期日管理名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_期日管理.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KIJITU_KANRI_T"))), True)
                    'サイクル管理名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_サイクル管理.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("CYCLE_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("FILE_NAME_T")), True)             'ファイル名

                    'フォーマット区分名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_総振_フォーマット区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("FMT_KBN_T"))), True)
                    'マルチ区分名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_マルチ区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("MULTI_KBN_T"))), True)
                    '入出金区分名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_入出金区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("NS_KBN_T"))), True)
                    '種別名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_種別.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SYUBETU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_CODE_T")), True)            '委託者コード
                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_KNAME_T")), True)           '委託者名カナ

                    KINKO = GCom.NzStr(oraReader.GetString("TKIN_NO_T"))
                    OutputCsvData(KINKO, True)                                                      '取扱金融機関コード
                    OutputCsvData(GCom.GetBKBRName(KINKO, "", 30), True)                            '取扱金融機関名
                    SITEN = GCom.NzStr(oraReader.GetString("TSIT_NO_T"))
                    OutputCsvData(SITEN, True)                                                      '取扱支店コード
                    OutputCsvData(GCom.GetBKBRName(KINKO, SITEN, 30), True)                         '取扱支店名

                    '科目名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_総振_科目.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KAMOKU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOUZA_T")), True)                 '口座番号

                    '送信区分名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_送信区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SOUSIN_KBN_T"))), True)

                    '信組判定用
                    OutputCsvData(Center, True)                                                    'センター区分

                    '種目コード名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_種目コード.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SYUMOKU_CODE_T"))), True)
                    '付加コード名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_付加コード.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("FUKA_CODE_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True)           '委託者名漢字
                    OutputCsvData(GCom.NzStr(oraReader.GetString("YUUBIN_T")), True)                '郵便番号
                    OutputCsvData(GCom.NzStr(oraReader.GetString("DENWA_T")), True)                 '電話番号
                    OutputCsvData(GCom.NzStr(oraReader.GetString("FAX_T")), True)                   'FAX番号
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOKYAKU_NO_T")), True)            '顧客番号
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KANREN_KIGYO_CODE_T")), True)     '関連企業情報
                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_NJYU_T")), True)            '委託者住所漢字
                    OutputCsvData(GCom.NzStr(oraReader.GetString("YUUBIN_KNAME_T")), True)          '郵送先名カナ
                    OutputCsvData(GCom.NzStr(oraReader.GetString("YUUBIN_NNAME_T")), True)          '郵送先名漢字

                    If RSV2_MASTPTN = "2" Then
                        For No As Integer = 1 To 31                                                     '振込日
                            If GCom.NzStr(GCom.NzStr(oraReader.GetString("DATE" & No & "_T"))) = "1" Then
                                OutputCsvData("○", True)
                            Else
                                OutputCsvData("", True)
                            End If
                        Next
                    Else
                        Dim FURI_DATE As String = ""
                        For No As Integer = 1 To 31
                            If GCom.NzStr(GCom.NzStr(oraReader.GetString("DATE" & No & "_T"))) = "1" Then
                                FURI_DATE &= No & " "
                            End If
                        Next
                        OutputCsvData(FURI_DATE, True)                                                 '振込日
                    End If

                    '振込休日シフトをテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_振込休日シフト.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("FURI_KYU_CODE_T"))), True)

                    If RSV2_MASTPTN = "2" Then
                        For No As Integer = 1 To 12                                                     '月別処理フラグ（振込・資金決済）
                            If GCom.NzStr(GCom.NzStr(oraReader.GetString("TUKI" & No & "_T"))) = "1" Then
                                OutputCsvData("○", True)
                            Else
                                OutputCsvData("", True)
                            End If
                        Next
                    Else
                        Dim TUKI As String = ""
                        For No As Integer = 1 To 12
                            If GCom.NzStr(GCom.NzStr(oraReader.GetString("TUKI" & No & "_T"))) = "1" Then
                                TUKI &= No & " "
                            End If
                        Next
                        OutputCsvData(TUKI, True)                                                      '月別処理フラグ（振込・資金決済
                    End If

                    If GCom.NzStr(oraReader.GetString("KEIYAKU_DATE_T")) = "00000000" Then                     '契約日
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCom.NzStr(oraReader.GetString("KEIYAKU_DATE_T")), True)
                    End If

                    OutputCsvData(GCom.NzStr(oraReader.GetString("KAISI_DATE_T")), True)                      '開始年月
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SYURYOU_DATE_T")), True)                    '終了年月
                    OutputCsvData(GCom.NzStr(oraReader.GetString("MOTIKOMI_KIJITSU_T")), True)                '持込期日
                    OutputCsvData(GCom.NzStr(oraReader.GetString("IRAISYO_YDATE_T")), True)                   '日数／基準日(依頼書)

                    '日付区分(依頼書)名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_日付区分(依頼書).TXT"), _
                                                                GCom.NzStr(oraReader.GetString("IRAISYO_KIJITSU_T"))), True)
                    '依頼書休日シフト名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_依頼書休日シフト.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("IRAISYO_KYU_CODE_T"))), True)
                    '依頼書種別名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_総振_依頼書種別.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("IRAISYO_KBN_T"))), True)
                    '依頼書出力順をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_依頼書出力順.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("IRAISYO_SORT_T"))), True)
                    '定額区分をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_定額区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TEIGAKU_KBN_T"))), True)
                    '受付明細表出力区分をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_受付明細表出力区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("UMEISAI_KBN_T"))), True)
                    '印刷部数をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_印刷部数.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("PRTNUM_T"))), True)
                    '決済区分をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_決済区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_KBN_T"))), True)
                    '資金確保方法をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_資金確保方法.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_PATN_T"))), True)

                    SITEN = GCom.NzStr(oraReader.GetString("TORIMATOME_SIT_T"))
                    OutputCsvData(SITEN, True)                                                      'とりまとめ店コード
                    OutputCsvData(GCom.GetBKBRName(JIKINKO, SITEN, 30), True)                       'とりまとめ店名
                    OutputCsvData(GCom.NzStr(oraReader.GetString("HONBU_KOUZA_T")), True)           '本部別段口座番号
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KESSAI_DAY_T")), True)            '日数／基準日(決済)

                    '日付区分(決済)をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_日付区分(決済).TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_KIJITSU_T"))), True)
                    '決済休日シフトをテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_決済休日シフト.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_KYU_CODE_T"))), True)

                    KINKO = GCom.NzStr(oraReader.GetString("TUKEKIN_NO_T"))
                    OutputCsvData(KINKO, True)                                                     '決済金融機関コード
                    OutputCsvData(GCom.GetBKBRName(KINKO, "", 30), True)                           '決済金融機関名
                    SITEN = GCom.NzStr(oraReader.GetString("TUKESIT_NO_T"))
                    OutputCsvData(SITEN, True)                                                     '決済支店コード
                    OutputCsvData(GCom.GetBKBRName(KINKO, SITEN, 30), True)                        '決済支店名

                    '決済科目名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_決済科目.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TUKEKAMOKU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TUKEKOUZA_T")), True)             '決済口座番号
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TUKEMEIGI_KNAME_T")), True)       '決済名義人(カナ)
                    OutputCsvData(GCom.NzStr(oraReader.GetString("BIKOU1_T")), True)                '備考１
                    OutputCsvData(GCom.NzStr(oraReader.GetString("BIKOU2_T")), True)                '備考２
                    SITEN = GCom.NzStr(oraReader.GetString("TESUUTYO_SIT_T"))
                    OutputCsvData(SITEN, True)                                                      '手数料徴求支店コード
                    OutputCsvData(GCom.GetBKBRName(JIKINKO, SITEN, 30), True)                       '手数料徴求支店名

                    '手数料徴求科目をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_手数料徴求科目.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_KAMOKU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUTYO_KOUZA_T")), True)        '手数料徴求口座番号

                    '手数料徴求区分をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_手数料徴求区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_KBN_T"))), True)
                    '手数料徴求方法をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_手数料徴求方法.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_PATN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUMAT_NO_T")).Trim, True)      '手数料集計周期
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUTYO_DAY_T")), True)          '日数／基準日(手数料徴求)

                    '日付区分(手数料徴求)をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_日付区分(手数料徴求).TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_KIJITSU_T"))), True)
                    '手数料徴求休日シフトをテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_手数料休日シフト.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUU_KYU_CODE_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("SOURYO_T")), True)                '送料
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOTEI_TESUU1_T")), True)          '固定手数料１
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOTEI_TESUU2_T")), True)          '固定手数料２
                    OutputCsvData("", True)                                                         '手数料徴求対象区分
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUMAT_MONTH_T")), True)        '集計基準月
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUMAT_ENDDAY_T")), True)       '集計終了日

                    '集計基準をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_集計基準.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUMAT_KIJYUN_T"))), True)
                    '集計方法をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_集計方法.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUMAT_PATN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUU_GRP_T")), True)             '集計企業ＧＲＰ

                    '印紙税、振込手数料を取得
                    TESUU.Init()
                    Call GetInshizei(oraReader.GetString("TESUU_TABLE_ID_T"), TESUU, oraDB)
                    OutputCsvData(TESUU.KijyunIDName, True)

                    OutputCsvData(TESUU.A1.ToString, True)                                          '自店1万円未満
                    OutputCsvData(TESUU.A2.ToString, True)                                          '自店1万円以上3万円未満
                    OutputCsvData(TESUU.A3.ToString, True)                                          '自店3万円以上
                    OutputCsvData(TESUU.B1.ToString, True)                                          '本支店1万円未満
                    OutputCsvData(TESUU.B2.ToString, True)                                          '本支店1万円以上3万円未満
                    OutputCsvData(TESUU.B3.ToString, True)                                          '本支店3万円以上
                    OutputCsvData(TESUU.C1.ToString, True)                                          '他行1万円未満
                    OutputCsvData(TESUU.C2.ToString, True)                                          '他行1万円以上3万円未満
                    OutputCsvData(TESUU.C3.ToString, True)                                          '他行3万円以上

                    '暗号化処理区分をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_暗号化処理区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("ENC_KBN_T"))), True)
                    'AESオプションをテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_ＡＥＳオプション.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("ENC_OPT1_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("ENC_KEY1_T")), True)              '暗号化キー１
                    OutputCsvData(GCom.NzStr(oraReader.GetString("ENC_KEY2_T")), True)              '暗号化キー２

                    '摘要区分をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST010_摘要区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TEKIYOU_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("KTEKIYOU_T")), True)              'カナ摘要
                    OutputCsvData(GCom.NzStr(oraReader.GetString("NTEKIYOU_T")), True)              '漢字摘要
                    OutputCsvData(GCom.NzStr(oraReader.GetString("FURI_CODE_T")), True)             '振替コード
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KIGYO_CODE_T")), True)            '企業コード
                    OutputCsvData(Me.InshizeiLabel.Inshizei1, True)                                 '印紙税１
                    OutputCsvData(Me.InshizeiLabel.Inshizei2, True)                                 '印紙税２
                    OutputCsvData(Me.InshizeiLabel.Inshizei3, True)                                 '印紙税３

                    If RSV2_MASTPTN = "2" Then
                        OutputCsvData(GCom.NzStr(oraReader.GetString("AITE_CNT_CODE_T")), True)         '相手センター確認ＣＤ
                        OutputCsvData(GCom.NzStr(oraReader.GetString("TOHO_CNT_CODE_T")), True)         '当方センター確認ＣＤ
                        OutputCsvData(GCom.NzStr(oraReader.GetString("DENSO_FILE_ID_T")), True)         '伝送ファイルＩＤ

                        '照合要否区分をテキストから取得する
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST011_照合要否区分.TXT"), _
                                                                    GCom.NzStr(oraReader.GetString("SYOUGOU_KBN_T"))), True)
                        '個別前処理をテキストから取得する
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFSMAST011_個別前処理.TXT"), _
                                                                    GCom.NzStr(oraReader.GetString("MAE_SYORI_T"))), True)

                        OutputCsvData(GCom.NzStr(oraReader.GetString("TOKKIJIKOU1_T")), True)           '特記事項
                        OutputCsvData(GCom.NzStr(oraReader.GetString("TOKKIJIKOU2_T")), True)           '受付特記事項
                        OutputCsvData(GCom.NzStr(oraReader.GetString("TOKKIJIKOU3_T")), True)           '返却特記事項
                        OutputCsvData(GCom.NzStr(oraReader.GetString("TOKKIJIKOU4_T")), True)           '搬送特記事項

                        '給与摘要区分をテキストから取得する
                        Dim KYUUYO_KBN_TXT As String = ""
                        If oraReader.GetString("SYUBETU_T") = "21" Then
                            KYUUYO_KBN_TXT = "KFSMAST011_給与摘要区分(総振).TXT"
                        Else
                            KYUUYO_KBN_TXT = "KFSMAST011_給与摘要区分(給与).TXT"
                        End If
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, KYUUYO_KBN_TXT), _
                                                                    GCom.NzStr(oraReader.GetString("KYUUYO_KBN_T"))), True)
                    End If

                    OutputCsvData("", , True)          'ダミー

                    oraReader.NextRead()
                End While
            Else
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "レコード作成", "失敗", "印刷対象なし")
                '2019/09/19 mukai 標準版修正 -------- START
                RecordCnt = -1
                '2019/09/19 mukai 標準版修正 -------- END
                Return False
            End If
            Return True
        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "レコード作成", "失敗", ex.ToString)
            Return False
        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function

    ''' <summary>
    ''' 印紙税、振込手数料基準ID名を取得する
    ''' </summary>
    ''' <param name="ID">振込手数料基準ID</param>
    ''' <param name="TESUU">振込手数料情報</param>
    ''' <remarks></remarks>
    Private Sub GetInshizei(ByVal ID As String, ByRef TESUU As strcTesuu, ByVal oraDB As MyOracle)
        Dim SQL As New StringBuilder
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim TAX As CASTCommon.ClsTAX
        Try
            '税率取得
            TAX = New CASTCommon.ClsTAX
            TAX.GetZeiritsu(strKijyunDate)
            TAX.GetInshizei(strKijyunDate)

            '帳票印刷用に印紙税の区分を変数に持つようにする
            Me.InshizeiLabel.Init()
            If TAX.INSHIZEI_ID.Equals("err") = False Then
                '金額によって表示形式を変える
                Dim strInshizei1 As String
                Dim strInshizei2 As String
                If TAX.INSHIZEI1 >= 10000 Then
                    strInshizei1 = String.Format("{0:#,##0}", TAX.INSHIZEI1 / 10000) & "万"
                ElseIf TAX.INSHIZEI1 >= 1000 Then
                    strInshizei1 = String.Format("{0:#,##0}", TAX.INSHIZEI1 / 1000) & "千"
                Else
                    strInshizei1 = String.Format("{0:#,##0}", TAX.INSHIZEI1)
                End If

                If TAX.INSHIZEI2 >= 10000 Then
                    strInshizei2 = String.Format("{0:#,##0}", TAX.INSHIZEI2 / 10000) & "万"
                ElseIf TAX.INSHIZEI1 >= 1000 Then
                    strInshizei2 = String.Format("{0:#,##0}", TAX.INSHIZEI2 / 1000) & "千"
                Else
                    strInshizei2 = String.Format("{0:#,##0}", TAX.INSHIZEI2)
                End If

                Me.InshizeiLabel.Inshizei1 = strInshizei1 & "円未満"
                Me.InshizeiLabel.Inshizei2 = strInshizei1 & "円以上" & strInshizei2 & "円未満"
                Me.InshizeiLabel.Inshizei3 = strInshizei2 & "円以上"
            End If

            '=================================
            '= 振込手数料基準ID名取得
            '=================================
            Try
                With SQL
                    .Append("select * from TESUUMAST")
                    .Append(" where FSYORI_KBN_C = '3'")
                    .Append(" and SYUBETU_C = '10'")
                    .Append(" and TESUU_TABLE_ID_C = " & SQ(ID))
                    .Append(" and TAX_ID_C = " & SQ(TAX.ZEIRITSU_ID))
                End With

                oraReader = New CASTCommon.MyOracleReader(OraDB)
                If oraReader.DataReader(SQL) Then
                    With TESUU
                        .KijyunIDName = oraReader.GetString("TESUU_TABLE_NAME_C")
                        .A1 = oraReader.GetInt64("TESUU_A1_C")
                        .A2 = oraReader.GetInt64("TESUU_A2_C")
                        .A3 = oraReader.GetInt64("TESUU_A3_C")
                        .B1 = oraReader.GetInt64("TESUU_B1_C")
                        .B2 = oraReader.GetInt64("TESUU_B2_C")
                        .B3 = oraReader.GetInt64("TESUU_B3_C")
                        .C1 = oraReader.GetInt64("TESUU_C1_C")
                        .C2 = oraReader.GetInt64("TESUU_C2_C")
                        .C3 = oraReader.GetInt64("TESUU_C3_C")
                    End With
                End If

            Catch ex As Exception
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振込手数料マスタ参照", "失敗", ex.ToString)
            Finally
                If Not oraReader Is Nothing Then
                    oraReader.Close()
                    oraReader = Nothing
                End If
            End Try

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印紙税、振込手数料基準ID名取得", "失敗", ex.ToString)
        End Try
    End Sub
End Class
