Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports MenteCommon.clsCommon
Public Class KFGP038
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
        InfoReport.ReportName = "KFGP038"

        If RSV2_MASTPTN = "2" Then
            ' 定義体名セット
            ReportBaseName = "KFGP038_学校マスタ項目確認票(MASTPTN2).rpd"
        Else
            ' 定義体名セット
            ReportBaseName = "KFGP038_学校マスタ項目確認票.rpd"
        End If
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        'タイトル行
        CSVObject.Output("処理日")
        CSVObject.Output("タイムスタンプ")
        CSVObject.Output("ログイン名")
        CSVObject.Output("端末名")
        CSVObject.Output("学校コード")
        CSVObject.Output("学校名カナ")
        CSVObject.Output("学校名漢字")
        CSVObject.Output("使用学年")
        CSVObject.Output("最高学年")
        CSVObject.Output("最終進級処理年")
        CSVObject.Output("学年")
        CSVObject.Output("学年名")
        CSVObject.Output("クラス1")
        CSVObject.Output("クラス2")
        CSVObject.Output("クラス3")
        CSVObject.Output("クラス4")
        CSVObject.Output("クラス5")
        CSVObject.Output("クラス6")
        CSVObject.Output("クラス7")
        CSVObject.Output("クラス8")
        CSVObject.Output("クラス9")
        CSVObject.Output("クラス10")
        CSVObject.Output("クラス11")
        CSVObject.Output("クラス12")
        CSVObject.Output("クラス13")
        CSVObject.Output("クラス14")
        CSVObject.Output("クラス15")
        CSVObject.Output("クラス16")
        CSVObject.Output("クラス17")
        CSVObject.Output("クラス18")
        CSVObject.Output("クラス19")
        CSVObject.Output("クラス20")
        CSVObject.Output("クラス名1")
        CSVObject.Output("クラス名2")
        CSVObject.Output("クラス名3")
        CSVObject.Output("クラス名4")
        CSVObject.Output("クラス名5")
        CSVObject.Output("クラス名6")
        CSVObject.Output("クラス名7")
        CSVObject.Output("クラス名8")
        CSVObject.Output("クラス名9")
        CSVObject.Output("クラス名10")
        CSVObject.Output("クラス名11")
        CSVObject.Output("クラス名12")
        CSVObject.Output("クラス名13")
        CSVObject.Output("クラス名14")
        CSVObject.Output("クラス名15")
        CSVObject.Output("クラス名16")
        CSVObject.Output("クラス名17")
        CSVObject.Output("クラス名18")
        CSVObject.Output("クラス名19")
        CSVObject.Output("クラス名20")
        CSVObject.Output("委託者コード")
        CSVObject.Output("取扱金融機関コード")
        CSVObject.Output("取扱金融機関名")
        CSVObject.Output("取扱支店コード")
        CSVObject.Output("取扱支店名")
        CSVObject.Output("科目")
        CSVObject.Output("口座番号")
        CSVObject.Output("他行区分")
        CSVObject.Output("自振契約作成区分")
        CSVObject.Output("摘要区分")
        CSVObject.Output("カナ摘要")
        CSVObject.Output("漢字摘要")
        CSVObject.Output("振替コード")
        CSVObject.Output("企業コード")
        CSVObject.Output("郵便番号")
        CSVObject.Output("電話番号")
        CSVObject.Output("FAX番号")
        CSVObject.Output("顧客番号")
        CSVObject.Output("関連企業情報")
        CSVObject.Output("委託者住所漢字")
        CSVObject.Output("振替日")
        CSVObject.Output("再振日")
        CSVObject.Output("媒体コード")
        CSVObject.Output("ファイル名")
        CSVObject.Output("再振種別")
        CSVObject.Output("持越種別")
        CSVObject.Output("入金休日シフト")
        CSVObject.Output("出金休日シフト")
        CSVObject.Output("再振休日シフト")
        CSVObject.Output("持込期日")
        CSVObject.Output("自振区分")
        CSVObject.Output("振替結果変換テーブルID")
        CSVObject.Output("契約日")
        CSVObject.Output("開始年月")
        CSVObject.Output("終了年月")
        CSVObject.Output("口座振替結果一覧表")
        CSVObject.Output("口座振替不能明細一覧表")
        CSVObject.Output("収納報告書")
        CSVObject.Output("口座振替店別集計表")
        CSVObject.Output("口座振替未納のお知らせ")
        CSVObject.Output("要求性入金伝票")
        CSVObject.Output("振替予定明細表作成区分")
        CSVObject.Output("帳票ソート順指定")
        CSVObject.Output("作成日")
        CSVObject.Output("更新日")
        CSVObject.Output("決済区分")
        CSVObject.Output("とりまとめ店コード")
        CSVObject.Output("とりまとめ店名")
        CSVObject.Output("本部別段口座番号")
        CSVObject.Output("日数/基準日(決済)")
        CSVObject.Output("日付区分(決済)")
        CSVObject.Output("決済休日シフト")
        CSVObject.Output("決済金融機関コード")
        CSVObject.Output("決済金融機関名")
        CSVObject.Output("決済支店コード")
        CSVObject.Output("決済支店名")
        CSVObject.Output("決済科目")
        CSVObject.Output("決済口座番号")
        CSVObject.Output("決済名義人(カナ)")
        CSVObject.Output("伝票備考1")
        CSVObject.Output("伝票備考2")
        CSVObject.Output("手数料徴求支店コード")
        CSVObject.Output("手数料徴求支店名")
        CSVObject.Output("手数料徴求科目")
        CSVObject.Output("手数料徴求口座番号")
        CSVObject.Output("手数料徴求区分")
        CSVObject.Output("手数料徴求方法")
        CSVObject.Output("手数料集計周期")
        CSVObject.Output("日数/基準日(手数料徴求)")
        CSVObject.Output("日付区分(手数料徴求)")
        CSVObject.Output("手数料徴求休日シフト")
        CSVObject.Output("手数料請求区分")
        CSVObject.Output("振替手数料単価")
        CSVObject.Output("消費税区分")
        CSVObject.Output("送料")
        CSVObject.Output("固定手数料1")
        CSVObject.Output("固定手数料2")
        CSVObject.Output("集計基準月")
        CSVObject.Output("集計終了日")
        CSVObject.Output("集計基準")
        CSVObject.Output("集計方法")
        CSVObject.Output("集計企業GRP")
        CSVObject.Output("振込手数料基準ID")
        CSVObject.Output("自店1万円未満")
        CSVObject.Output("自店1万円以上3万円未満")
        CSVObject.Output("自店3万円以上")
        CSVObject.Output("本支店1万円未満")
        CSVObject.Output("本支店1万円以上3万円未満")
        CSVObject.Output("本支店3万円以上")
        CSVObject.Output("他行1万円未満")
        CSVObject.Output("他行1万円以上3万円未満")
        CSVObject.Output("他行3万円以上")
        CSVObject.Output("印紙税1")
        CSVObject.Output("印紙税2")
        CSVObject.Output("印紙税3")

        If RSV2_MASTPTN = "2" Then
            CSVObject.Output("契約書番号")
            CSVObject.Output("決済種別")
            CSVObject.Output("搬送方法")
            CSVObject.Output("搬送ルート１")
            CSVObject.Output("搬送ルート２")
            CSVObject.Output("搬送ルート３")
            CSVObject.Output("返却支店コード")
            CSVObject.Output("返却支店名")
            CSVObject.Output("照合要否区分")
        End If

        CSVObject.Output("WEB伝送ユーザ名")
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
        Dim PATH_TXT As String = CASTCommon.GetFSKJIni("GCOMMON", "TXT")
        Dim KINKO As String = ""
        Dim SITEN As String = ""
        Dim FURI_DATE As String
        Dim TUKI As String
        Dim JIKINKO As String = CASTCommon.GetFSKJIni("GCOMMON", "KINKOCD ")
        Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
        Dim NowTime As String = Format(DateTime.Now, "HHmmss")

        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM ")
            If RSV2_MASTPTN = "2" Then
                SQL.Append("     GAKMAST_VIEW")
            Else
                SQL.Append("     GAKMAST2")
            End If
            SQL.Append("   , GAKMAST1")
            SQL.Append(" WHERE ")
            SQL.Append("     GAKKOU_CODE_T = " & SQ(strGakkouCd))
            SQL.Append(" AND GAKKOU_CODE_T = GAKKOU_CODE_G")
            SQL.Append(" ORDER BY ")
            SQL.Append("     GAKUNEN_CODE_G ")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    'コンボボックスの内容は、画面の更新後項目を取得する
                    FURI_DATE = ""
                    TUKI = ""

                    OutputCsvData(Today, True)                                                      'システム日付
                    OutputCsvData(NowTime, True)                                                    'タイムスタンプ
                    OutputCsvData(strUserId, True)                                                  'ログイン名
                    OutputCsvData(Environment.MachineName, True)                                    '端末名
                    OutputCsvData(GCom.NzStr(oraReader.GetString("GAKKOU_CODE_G")), True)           '学校コード
                    OutputCsvData(GCom.NzStr(oraReader.GetString("GAKKOU_KNAME_G")), True)          '学校名カナ
                    OutputCsvData(GCom.NzStr(oraReader.GetString("GAKKOU_NNAME_G")), True)          '学校名漢字
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SIYOU_GAKUNEN_T")), True)         '使用学年
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SAIKOU_GAKUNEN_T")), True)        '最高学年
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SINKYU_NENDO_T")), True)          '最終進級処理年
                    OutputCsvData(GCom.NzStr(oraReader.GetString("GAKUNEN_CODE_G")), True)          '学年
                    OutputCsvData(GCom.NzStr(oraReader.GetString("GAKUNEN_NAME_G")), True)          '学年名
                    For No As Integer = 1 To 20                                                     'クラス1～20
                        If GCom.NzStr(oraReader.GetString("CLASS_NAME1" & No.ToString("00") & "_G")).Trim = "" Then
                            OutputCsvData("")
                        Else
                            OutputCsvData(GCom.NzStr(oraReader.GetString("CLASS_CODE1" & No.ToString("00") & "_G")), True)
                        End If
                    Next
                    For No As Integer = 1 To 20                                                     'クラス名1～20
                        OutputCsvData(GCom.NzStr(oraReader.GetString("CLASS_NAME1" & No.ToString("00") & "_G")), True)
                    Next
                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_CODE_T")), True)            '委託者コード
                    KINKO = GCom.NzStr(oraReader.GetString("TKIN_NO_T"))
                    OutputCsvData(KINKO, True)                                                      '取扱金融機関コード
                    OutputCsvData(GCom.GetBKBRName(KINKO, "", 30), True)                            '取扱金融機関名
                    SITEN = GCom.NzStr(oraReader.GetString("TSIT_NO_T"))
                    OutputCsvData(SITEN, True)                                                      '取扱支店コード
                    OutputCsvData(GCom.GetBKBRName(KINKO, SITEN, 30), True)                         '取扱支店名

                    '科目名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "GFJ_科目.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KAMOKU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOUZA_T")), True)                 '口座番号

                    '他行区分名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "GFJ_他行区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TAKO_KBN_T"))), True)
                    '自振契約作成区分名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_自振契約作成区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("JIFURICHK_KBN_T"))), True)
                    '摘要区分名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_摘要区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TEKIYOU_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("KTEKIYOU_T")), True)              'カナ摘要
                    OutputCsvData(GCom.NzStr(oraReader.GetString("NTEKIYOU_T")), True)              '漢字摘要
                    OutputCsvData(GCom.NzStr(oraReader.GetString("FURI_CODE_T")), True)             '振替コード
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KIGYO_CODE_T")), True)            '企業コード
                    OutputCsvData(GCom.NzStr(oraReader.GetString("YUUBIN_T")), True)                '郵便番号
                    OutputCsvData(GCom.NzStr(oraReader.GetString("DENWA_T")), True)                 '電話番号
                    OutputCsvData(GCom.NzStr(oraReader.GetString("FAX_T")), True)                   'FAX番号
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOKYAKU_NO_T")), True)            '顧客番号
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KANREN_KIGYO_CODE_T")), True)     '関連企業情報
                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_NJYU_T")), True)            '委託者住所漢字
                    OutputCsvData(GCom.NzStr(oraReader.GetString("FURI_DATE_T")), True)             '振替日
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SFURI_DATE_T")), True)            '再振日

                    '媒体名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "GFJ_媒体.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("BAITAI_CODE_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("FILE_NAME_T")), True)                       'ファイル名
                    Select Case GCom.NzInt(oraReader.GetString("SFURI_SYUBETU_T"))
                        Case 0
                            OutputCsvData("なし", True)    '再振種別
                            OutputCsvData("なし", True)    '持越種別
                        Case 1
                            OutputCsvData("あり", True)    '再振種別
                            OutputCsvData("なし", True)    '持越種別
                        Case 2
                            OutputCsvData("あり", True)    '再振種別
                            OutputCsvData("あり", True)    '持越種別
                        Case 3
                            OutputCsvData("なし", True)    '再振種別
                            OutputCsvData("あり", True)    '持越種別
                    End Select

                    '出金休日シフトをテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_振替休日シフト.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("NKYU_CODE_T"))), True)
                    '入金休日シフトをテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_振替休日シフト.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SKYU_CODE_T"))), True)
                    '再振休日シフトをテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_再振休日シフト.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SFURI_KYU_CODE_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("MOTIKOMI_KIJITSU_T")), True)      '持込期日

                    '自振区分をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_自振区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("JIFURI_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("FKEKKA_TBL_T")), True)            '振替結果変換テーブルID
                    If GCom.NzStr(oraReader.GetString("KEIYAKU_DATE_T")) = "00000000" Then          '契約日
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCom.NzStr(oraReader.GetString("KEIYAKU_DATE_T")), True)
                    End If
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KAISI_DATE_T")), True)            '開始年月
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SYURYOU_DATE_T")), True)          '終了年月
                    If GCom.NzStr(oraReader.GetString("MEISAI_FUNOU_T")) = "1" Then                 '口座振替結果一覧表
                        OutputCsvData("出力対象", True)
                    Else
                        OutputCsvData("出力なし", True)
                    End If
                    If GCom.NzStr(oraReader.GetString("MEISAI_KEKKA_T")) = "1" Then                 '口座振替不能明細一覧表
                        OutputCsvData("出力対象", True)
                    Else
                        OutputCsvData("出力なし", True)
                    End If
                    If GCom.NzStr(oraReader.GetString("MEISAI_HOUKOKU_T")) = "1" Then               '収納報告書
                        OutputCsvData("出力対象", True)
                    Else
                        OutputCsvData("出力なし", True)
                    End If
                    If GCom.NzStr(oraReader.GetString("MEISAI_TENBETU_T")) = "1" Then               '口座振替店別集計表
                        OutputCsvData("出力対象", True)
                    Else
                        OutputCsvData("出力なし", True)
                    End If
                    If GCom.NzStr(oraReader.GetString("MEISAI_MINOU_T")) = "1" Then                 '口座振替未納のお知らせ
                        OutputCsvData("出力対象", True)
                    Else
                        OutputCsvData("出力なし", True)
                    End If
                    If GCom.NzStr(oraReader.GetString("MEISAI_YOUKYU_T")) = "1" Then                '要求性入金伝票
                        OutputCsvData("出力対象", True)
                    Else
                        OutputCsvData("出力なし", True)
                    End If

                    '振替予定明細表作成区分をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_振替予定明細表作成区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("MEISAI_KBN_T"))), True)
                    '帳票ソート順指定をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_帳票ソート順指定.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("MEISAI_OUT_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("SAKUSEI_DATE_T")), True)          '作成日
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOUSIN_DATE_T")), True)           '更新日

                    '決済区分をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "GFJ_決済区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_KBN_T"))), True)

                    SITEN = GCom.NzStr(oraReader.GetString("TORIMATOME_SIT_T"))
                    OutputCsvData(SITEN, True)                                                      'とりまとめ店コード
                    OutputCsvData(GCom.GetBKBRName(JIKINKO, SITEN, 30), True)                       'とりまとめ店名
                    OutputCsvData(GCom.NzStr(oraReader.GetString("HONBU_KOUZA_T")), True)           '本部別段口座番号
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KESSAI_DAY_T")), True)            '日数/基準日(決済)

                    '日付区分(決済)をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_日付区分(決済).TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_KIJITSU_T"))), True)
                    '決済休日シフトをテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_決済休日シフト.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_KYU_CODE_T"))), True)

                    KINKO = GCom.NzStr(oraReader.GetString("TUKEKIN_NO_T"))
                    OutputCsvData(KINKO, True)                                                      '決済金融機関コード
                    OutputCsvData(GCom.GetBKBRName(KINKO, "", 30), True)                            '決済金融機関名
                    SITEN = GCom.NzStr(oraReader.GetString("TUKESIT_NO_T"))
                    OutputCsvData(SITEN, True)                                                      '決済支店コード
                    OutputCsvData(GCom.GetBKBRName(KINKO, SITEN, 30), True)                         '決済支店名

                    '決済科目をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_決済科目.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TUKEKAMOKU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TUKEKOUZA_T")), True)             '決済口座番号
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TUKEMEIGI_T")), True)             '決済名義人(カナ)
                    OutputCsvData(GCom.NzStr(oraReader.GetString("DENPYO_BIKOU1_T")), True)         '伝票備考1
                    OutputCsvData(GCom.NzStr(oraReader.GetString("DENPYO_BIKOU2_T")), True)         '伝票備考2
                    SITEN = GCom.NzStr(oraReader.GetString("TESUUTYO_SIT_T"))
                    OutputCsvData(SITEN, True)                                                      '手数料徴求支店コード
                    OutputCsvData(GCom.GetBKBRName(JIKINKO, SITEN, 30), True)                       '手数料徴求支店名

                    '手数料徴求科目をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_手数料徴求科目.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_KAMOKU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUTYO_KOUZA_T")), True)        '手数料徴求口座番号

                    '手数料徴求区分をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_手数料徴求区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_KBN_T"))), True)
                    '手数料徴求方法をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_手数料徴求方法.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_PATN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUMAT_NO_T")).Trim, True)      '手数料集計周期
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUTYO_DAY_T")), True)          '日数/基準日(手数料徴求)

                    '日付区分(手数料徴求)をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_日付区分(手数料徴求).TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_KIJITSU_T"))), True)
                    '手数料徴求休日シフトをテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_手数料休日シフト.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUU_KYU_CODE_T"))), True)
                    '手数料請求区分をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_手数料請求区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SEIKYU_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("KIHTESUU_T")), True)              '振替手数料単価

                    '消費税区分をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_消費税区分.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SYOUHI_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("SOURYO_T")), True)                '送料
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOTEI_TESUU1_T")), True)          '固定手数料1
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOTEI_TESUU2_T")), True)          '固定手数料2
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUMAT_MONTH_T")), True)        '集計基準月
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUMAT_ENDDAY_T")), True)       '集計終了日

                    '集計基準をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_集計基準.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUMAT_KIJYUN_T"))), True)
                    '集計方法をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_集計方法.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUMAT_PATN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUU_GRP_T")), True)             '集計企業GRP

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

                    OutputCsvData(Me.InshizeiLabel.Inshizei1, True)                                 '印紙税1
                    OutputCsvData(Me.InshizeiLabel.Inshizei2, True)                                 '印紙税2
                    OutputCsvData(Me.InshizeiLabel.Inshizei3, True)                                 '印紙税3

                    If RSV2_MASTPTN = "2" Then
                        OutputCsvData(GCom.NzStr(oraReader.GetString("KEIYAKU_NO_T")), True)            '契約書番号

                        '決済種別をテキストから取得する
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_決済種別.TXT"), _
                                                                    GCom.NzStr(oraReader.GetString("KESSAI_SYUBETU_T"))), True)
                        '搬送方法をテキストから取得する
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST011_搬送方法.TXT"), _
                                                                    GCom.NzStr(oraReader.GetString("HANSOU_KBN_T"))), True)

                        OutputCsvData(GCom.NzStr(oraReader.GetString("HANSOU_ROOT1_T")), True)          '搬送ルート１
                        OutputCsvData(GCom.NzStr(oraReader.GetString("HANSOU_ROOT2_T")), True)          '搬送ルート２
                        OutputCsvData(GCom.NzStr(oraReader.GetString("HANSOU_ROOT3_T")), True)          '搬送ルート３
                        SITEN = GCom.NzStr(oraReader.GetString("HENKYAKU_SIT_NO_T"))
                        OutputCsvData(SITEN, True)                                                      '返却支店コード
                        OutputCsvData(GCom.GetBKBRName(JIKINKO, SITEN, 30), True)                       '返却支店名

                        '照合要否区分をテキストから取得する
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST011_照合要否区分.TXT"), _
                                                                    GCom.NzStr(oraReader.GetString("SYOUGOU_KBN_T"))), True)
                    End If

                    OutputCsvData(GCom.NzStr(oraReader.GetString("YOBI1_T")), True)                 '予備１(WEB伝送ユーザ名)
                    OutputCsvData("", , True)          'ダミー

                    oraReader.NextRead()
                End While

            Else
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", "登録なし")
                RecordCnt = -1
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
                    .Append(" where FSYORI_KBN_C = '1'")
                    .Append(" and SYUBETU_C = '91'")
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
