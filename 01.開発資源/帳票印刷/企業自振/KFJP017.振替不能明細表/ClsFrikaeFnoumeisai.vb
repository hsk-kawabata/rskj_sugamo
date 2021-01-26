Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic
Imports Microsoft.VisualBasic
' 
Public Class ClsFrikaeFnoumeisai

    ' 引数
    Public TORIS_CODE As String         ' 取引先主コード
    Public TORIF_CODE As String         ' 取引先副コード
    Public FURI_DATE As String          ' 振替日
    Public FUNO_FLG As String           ' 不能フラグ
    Public PRINTERNAME As String = ""   ' 出力プリンタ
    Public INVOKE_KBN As String = ""    ' 呼び出し区分      '2012/06/30 標準版　WEB伝送対応

    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
    Private TAX As CASTCommon.ClsTAX
    '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

    ' 2015/12/18 タスク）綾部 ADD 【PG】UI_B-14-05(RSV2対応) -------------------- START
    Private INI_RSV2_J_FUNOU As String = ""
    Private INI_COMMON_PRINT_NAME As String = ""
    ' 2015/12/18 タスク）綾部 ADD 【PG】UI_B-14-05(RSV2対応) -------------------- END

    ' 機能   ： 振替不能明細表メイン処理
    '
    ' 引数   ： なし
    '
    ' 戻り値 ： 0 - 正常 0以外 - 異常
    '
    ' 備考   ： 
    '
    Function Main() As Integer
        ' オラクル
        MainDB = New CASTCommon.MyOracle

        Dim bRet As Boolean
        Try
            MainLOG.ToriCode = TORIS_CODE & TORIF_CODE
            MainLOG.FuriDate = FURI_DATE

            ' 振替不能明細表印刷処理
            bRet = PrintFunoumeisai()

        Catch ex As Exception
            MainLOG.Write("振替不能明細表", "失敗", ex.Message & ":" & ex.StackTrace)

            Return -1
        Finally
            MainDB.Close()
        End Try

        If bRet Then
            Return 0
        Else
            Return 2
        End If

    End Function

    ' 機能   ： 振替不能明細表出力処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考   ： 
    '
    Private Function PrintFunoumeisai() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim sFMTKbn As String = ""

        '*** 修正 mitsu 2009/06/11 合計件数に対する手数料考慮 ***
        ' 自金庫コード
        Dim Jikinko As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

        '自金庫名をiniファイルから取得
        Dim Jikinko_NAME As String = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")
        'とりまとめ店名
        Dim strTORIMATOME_SIT_T As String = ""
        Dim strTORIMATOME_SIT_NAME As String = ""
        'フォーマット区分
        Dim strFormatKubun As String = ""
        Dim strFormatKubunName As String = ""

        '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
        '' 消費税
        'Dim sTax As String = CASTCommon.GetFSKJIni("COMMON", "ZEIRITU")
        'If sTax = "err" Then
        '    sTax = "1.05"
        'End If
        '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<

        ' 2015/12/18 タスク）綾部 ADD 【PG】UI_B-14-05(RSV2対応) -------------------- START
        INI_RSV2_J_FUNOU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "J_FUNOU")
        INI_COMMON_PRINT_NAME = CASTCommon.GetFSKJIni("COMMON", "PRINT_NAME")
        ' 2015/12/18 タスク）綾部 ADD 【PG】UI_B-14-05(RSV2対応) -------------------- END

        Dim TesuuKin1 As Long = 0                    ' 手数料金額１
        Dim TesuuKin2 As Long = 0                    ' 手数料金額２
        Dim TesuuKin3 As Long = 0                    ' 手数料金額３
        Dim JifuriKin As Long = 0                    ' 自振金額
        '********************************************************

        '2009/12/29      '手数料テーブルファイルの読込
        '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
        'Dim strTESUU_TABLE_FILE As String = ""
        'Dim intFILE_NO As Integer = 0
        '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<
        Dim intKIJYUN_ID As Integer
        '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
        'strTESUU_TABLE_FILE = Path.Combine(TXT_FOLDER, strTESUU_TABLE_FILE_NAME)
        'intFILE_NO = FreeFile()
        ''読取アクセスで開く
        'FileOpen(intFILE_NO, strTESUU_TABLE_FILE, OpenMode.Input)

        'Dim strKIJYUN_ID_CODE As String = ""
        'Dim strKIJYUN_ID_TEXT As String = ""
        'Dim intIndex As Integer = 0
        '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<

        '2010/10/07.Sakon　フォーマット区分印字要否をINIファイルから設定 +++++++++++++++
        Dim FMTKBN_PRINT As String = CASTCommon.GetFSKJIni("PRINT", "FMTKBN_PRINT")
        If FMTKBN_PRINT = "err" Then
            FMTKBN_PRINT = "1"
        End If
        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        '2012/06/30 標準版　WEB伝送対応
        Dim User_ID As String = ""  'ユーザＩＤ
        Dim ITAKU_CODE As String = "" '委託者コード
        Dim WEB_PRINTERNAME As String = "" 'WEB伝送用プリンタ名
        Dim WEB_PRINT As String = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_PRINT")    'WEB伝送プリント区分(0:PDFのみ、1:PDFと紙）

        '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
        'Do Until EOF(intFILE_NO)
        '    Input(intFILE_NO, strKIJYUN_ID_CODE)
        '    If strKIJYUN_ID_CODE = "" Then
        '        Exit Do
        '    End If
        '    Input(intFILE_NO, strKIJYUN_ID_TEXT)
        '    intIndex = CInt(strKIJYUN_ID_CODE)
        '    If intIndex = -1 Then
        '    Else
        '        If intIndex > 0 And intIndex < 10 Then
        '            TESUU_TABLE_DATA(intIndex).strKIJYUN_ID_CODE = strKIJYUN_ID_CODE
        '            TESUU_TABLE_DATA(intIndex).strKIJYUN_ID_TEXT = strKIJYUN_ID_TEXT
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng10000UNDER_JITEN)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000UNDER_JITEN)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000OVER_JITEN)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng10000UNDER_HONSITEN)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000UNDER_HONSITEN)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000OVER_HONSITEN)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng10000UNDER_TAKOU)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000UNDER_TAKOU)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000OVER_TAKOU)
        '        End If
        '    End If
        'Loop
        'FileClose(intFILE_NO)
        '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<
        '=============================================

        '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
        Me.TAX = New CASTCommon.ClsTAX
        Dim ZEIKIJUN As String = CASTCommon.GetFSKJIni("JIFURI", "ZEIKIJUN")
        If ZEIKIJUN.Equals("err") = True OrElse ZEIKIJUN = String.Empty Then
            MainLOG.Write("振替不能明細表印刷", "失敗", "[JIFURI]ZEIKIJUN 設定なし")
            Return False
        End If
        Dim TaxKey As String = String.Empty         '税率取得判断キー
        '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

        '2011/06/16 標準版修正 取引先コードALL9対応 ------------------START
        If TORIS_CODE = "9999999999" AndAlso TORIF_CODE = "99" Then
        Else
            '2011/06/16 標準版修正 取引先コードALL9対応 ------------------END
            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append(" FMT_KBN_T")
            SQL.Append(", TORIMATOME_SIT_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(TORIF_CODE))
            Dim OraTori As New CASTCommon.MyOracleReader(MainDB)
            If OraTori.DataReader(SQL) = True Then
                sFMTKbn = OraTori.GetValue(0)
                strTORIMATOME_SIT_T = OraTori.GetValue(1)
            End If
            OraTori.Close()

            'とりまとめ店名取得
            Dim OraTen As New CASTCommon.MyOracleReader(MainDB)
            SQL.Length = 0
            SQL.Append("SELECT SIT_NNAME_N")
            SQL.Append(" FROM TENMAST")
            SQL.Append(" WHERE KIN_NO_N = " & SQ(Jikinko))
            SQL.Append(" AND SIT_NO_N = " & SQ(strTORIMATOME_SIT_T))
            SQL.Append(" GROUP BY KIN_NO_N")
            SQL.Append(", SIT_NO_N")
            SQL.Append(", SIT_NNAME_N")
            If OraTen.DataReader(SQL) = True Then
                strTORIMATOME_SIT_NAME = OraTen.GetValue(0)
            End If
            OraTen.Close()
            '2011/06/16 標準版修正 取引先コードALL9対応 ------------------START
        End If
        '2011/06/16 標準版修正 取引先コードALL9対応 ------------------END

        Dim PrnFunoumeisai As New ClsPrnFrikaeFunoumeisai    ' 振替不能明細表

        SQL = New StringBuilder(128)
        SQL.Append("SELECT")
        SQL.Append(" TORIMAST.TORIS_CODE_T")
        SQL.Append(",TORIMAST.TORIF_CODE_T")
        SQL.Append(",TORIMAST.ITAKU_NNAME_T")
        SQL.Append(",MEIMAST.FURI_DATE_K")
        SQL.Append(",TORIMAST.TORIMATOME_SIT_T")
        SQL.Append(",TORIMAST.FUNOU_MEISAI_KBN_T")
        SQL.Append(",TENMAST.KIN_NNAME_N")
        SQL.Append(",TENMAST.SIT_NNAME_N")
        SQL.Append(",MEIMAST.ITAKU_KIN_K")
        SQL.Append(",MEIMAST.ITAKU_SIT_K")
        SQL.Append(",MEIMAST.KEIYAKU_KIN_K")
        SQL.Append(",MEIMAST.KEIYAKU_SIT_K")
        SQL.Append(",MEIMAST.KEIYAKU_KAMOKU_K")
        SQL.Append(",MEIMAST.KEIYAKU_KOUZA_K")
        SQL.Append(",MEIMAST.KEIYAKU_KNAME_K")
        SQL.Append(",MEIMAST.FURIKIN_K")
        SQL.Append(",MEIMAST.FURIKETU_CODE_K")
        SQL.Append(",MEIMAST.JYUYOUKA_NO_K")
        SQL.Append(",SCHMAST.SYORI_KEN_S")
        SQL.Append(",SCHMAST.SYORI_KIN_S")
        SQL.Append(",SCHMAST.FUNOU_KEN_S")
        SQL.Append(",SCHMAST.FUNOU_KIN_S")
        SQL.Append(",SCHMAST.FURI_KEN_S")
        SQL.Append(",SCHMAST.FURI_KIN_S")
        SQL.Append(",SCHMAST.TESUU_KIN_S")
        SQL.Append(",SCHMAST.TESUU_KIN1_S")
        SQL.Append(",SCHMAST.TESUU_KIN2_S")
        SQL.Append(",SCHMAST.TESUU_KIN3_S")
        SQL.Append(",TORIMAST.ITAKU_CODE_T")
        '*** 修正 mitsu 2009/06/11 合計件数に対する手数料考慮 ***
        '手数料計算に使用する項目を追加
        SQL.Append(",TORIMAST.TESUUTYO_KBN_T")
        SQL.Append(",TORIMAST.TESUUMAT_PATN_T")
        '2013/11/14 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
        '引落手数料は別途計算する
        SQL.Append(", TORIMAST.KIHTESUU_T")
        SQL.Append(", TORIMAST.SEIKYU_KBN_T")
        SQL.Append(", TORIMAST.KOTEI_TESUU1_T")
        SQL.Append(", TORIMAST.KOTEI_TESUU2_T")
        SQL.Append(", TORIMAST.SYOUHI_KBN_T")
        'SQL.Append(",TRUNC(")
        'SQL.Append("  ((KIHTESUU_T * DECODE(SEIKYU_KBN_T,'0',SYORI_KEN_S,FURI_KEN_S) / 100)")
        'SQL.Append("   + NVL(KOTEI_TESUU1_T,0)")
        'SQL.Append("   + NVL(KOTEI_TESUU2_T,0))")
        'SQL.Append("   * DECODE(SYOUHI_KBN_T,'0'," & sTax & ",1)) TESUU_KIN1")     ' 引落手数料
        '2013/11/14 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<
        SQL.Append(",SOURYO_T")                       ' 送料
        SQL.Append(", TORIMAST.TESUU_TABLE_ID_T")   '2009/12/29 追加
        SQL.Append(",TUKEKIN_NO_T")                   ' 決済金融機関
        SQL.Append(",TUKESIT_NO_T")                   ' 決済支店
        '********************************************************
        SQL.Append(", SCHMAST.FILE_SEQ_S")
        SQL.Append(", TORIMAST.FMT_KBN_T")
        SQL.Append(", TORIMAST.FURI_CODE_T")        '振替コード
        SQL.Append(", TORIMAST.KIGYO_CODE_T")       '企業コード
        ' 2012/06/30 標準版　WEB伝送対応------------------------------------->
        SQL.Append(", TORIMAST.BAITAI_CODE_T")      '媒体コード
        SQL.Append(", TORIMAST.MULTI_KBN_T")        'マルチ区分
        SQL.Append(", TORIMAST.ITAKU_KANRI_CODE_T") '代表委託者コード
        ' 2012/06/30 標準版　WEB伝送対応-------------------------------------<
        '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
        SQL.Append(", SCHMAST.KESSAI_YDATE_S")
        '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<
        SQL.Append(" FROM TORIMAST")
        SQL.Append(", (SELECT KIN_NO_N")
        SQL.Append(", SIT_NO_N")
        SQL.Append(", KIN_NNAME_N")
        SQL.Append(", SIT_NNAME_N")
        SQL.Append(" FROM TENMAST")
        SQL.Append(" GROUP BY KIN_NO_N")
        SQL.Append(", SIT_NO_N")
        SQL.Append(", KIN_NNAME_N")
        SQL.Append(", SIT_NNAME_N) TENMAST")
        SQL.Append(",MEIMAST")
        SQL.Append(",SCHMAST")
        SQL.Append(" WHERE ")
        SQL.Append(" FSYORI_KBN_T = '1'")
        If TORIS_CODE = "9999999999" AndAlso TORIF_CODE = "99" Then
            ' 全印刷
            SQL.Append(" AND MEIMAST.DATA_KBN_K = DECODE(FMT_KBN_T,'02','3','2')")
        Else
            SQL.Append(" AND TORIS_CODE_T = " & SQ(TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(TORIF_CODE))
            If sFMTKbn = "02" Then
                ' 国税の場合，
                SQL.Append(" AND MEIMAST.DATA_KBN_K = '3'")
            Else
                SQL.Append(" AND MEIMAST.DATA_KBN_K = '2'")
            End If
        End If
        SQL.Append(" AND FURI_DATE_S = " & SQ(FURI_DATE))
        SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
        SQL.Append(" AND SCHMAST.FURI_DATE_S = MEIMAST.FURI_DATE_K")
        SQL.Append(" AND SCHMAST.TORIS_CODE_S = MEIMAST.TORIS_CODE_K")
        SQL.Append(" AND SCHMAST.TORIF_CODE_S = MEIMAST.TORIF_CODE_K")
        SQL.Append(" AND SCHMAST.FSYORI_KBN_S = TORIMAST.FSYORI_KBN_T")
        SQL.Append(" AND SCHMAST.TORIS_CODE_S = TORIMAST.TORIS_CODE_T")
        SQL.Append(" AND SCHMAST.TORIF_CODE_S = TORIMAST.TORIF_CODE_T")
        SQL.Append(" AND TENMAST.KIN_NO_N(+) = MEIMAST.KEIYAKU_KIN_K")
        SQL.Append(" AND TENMAST.SIT_NO_N(+) = MEIMAST.KEIYAKU_SIT_K")
        SQL.Append(" ORDER BY SCHMAST.FILE_SEQ_S")
        SQL.Append(", SCHMAST.TORIS_CODE_S")
        SQL.Append(", SCHMAST.TORIF_CODE_S")
        SQL.Append(", MEIMAST.RECORD_NO_K")

        Dim name As String = ""
        Dim bSQL As Boolean
        bSQL = OraReader.DataReader(SQL)

        If bSQL = True Then

            ' 2012/06/30 標準版　WEB伝送対応------------------------------------->
            If OraReader.GetString("BAITAI_CODE_T") = "10" And INVOKE_KBN = "1" Then
                WEB_PRINTERNAME = CASTCommon.GetFSKJIni("WEB_DEN", "PRINTER")  'PDFを作成するプリンタ名を設定
                SQL = New StringBuilder(128)
                Dim OraWebReader As New CASTCommon.MyOracleReader(MainDB)
                SQL.Append(" SELECT USER_ID_W ")
                SQL.Append(" FROM WEB_RIREKIMAST ")
                If OraReader.GetString("MULTI_KBN_T") = "0" Then
                    SQL.Append(" WHERE TORIS_CODE_W = '" & OraReader.GetString("TORIS_CODE_T") & "'")
                    SQL.Append(" AND TORIF_CODE_W  = '" & OraReader.GetString("TORIF_CODE_T") & "'")
                Else
                    SQL.Append(" WHERE ITAKU_KANRI_CODE_W = '" & OraReader.GetString("ITAKU_KANRI_CODE_T") & "'")
                End If
                SQL.Append(" AND FURI_DATE_W = '" & CASTCommon.ConvertDate(OraReader.GetItem("FURI_DATE_K"), "yyyyMMdd") & "'")
                SQL.Append(" AND FSYORI_KBN_W = '1'")

                If OraWebReader.DataReader(SQL) Then
                    User_ID = OraWebReader.GetString("USER_ID_W")
                Else
                    User_ID = ""
                End If

                ITAKU_CODE = OraReader.GetString("ITAKU_CODE_T")

                OraWebReader.Close()

            End If
            ' 2012/06/30 標準版　WEB伝送対応-------------------------------------<

            name = PrnFunoumeisai.CreateCsvFile()


            Dim OldKey As String = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")

            Do
                '2011/06/16 標準版修正 取引先コードALL9対応 ------------------START
                If TORIS_CODE = "9999999999" AndAlso TORIF_CODE = "99" Then
                    Dim OraTen As New CASTCommon.MyOracleReader(MainDB)
                    SQL.Length = 0
                    SQL.Append("SELECT SIT_NNAME_N")
                    SQL.Append(" FROM TENMAST")
                    SQL.Append(" WHERE KIN_NO_N = " & SQ(Jikinko))
                    SQL.Append(" AND SIT_NO_N = " & SQ(OraReader.GetString("TORIMATOME_SIT_T")))
                    SQL.Append(" GROUP BY KIN_NO_N")
                    SQL.Append(", SIT_NO_N")
                    SQL.Append(", SIT_NNAME_N")
                    If OraTen.DataReader(SQL) = True Then
                        strTORIMATOME_SIT_NAME = OraTen.GetValue(0)
                    End If
                    OraTen.Close()
                End If
                '2011/06/16 標準版修正 取引先コードALL9対応 ------------------END
                '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
                If TaxKey.Equals(OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & OraReader.GetString("FURI_DATE_K")) = False Then
                    '税率取得キーが異なる場合に税率を取得する
                    Dim strKijunDate As String = String.Empty
                    If ZEIKIJUN.Equals("0") = True Then
                        strKijunDate = OraReader.GetString("FURI_DATE_K")
                    Else
                        strKijunDate = OraReader.GetString("KESSAI_YDATE_S")
                    End If

                    '税率取得
                    Me.TAX.GetZeiritsu(strKijunDate)
                    If Me.TAX.ZEIRITSU.Equals("err") = True Then
                        MainLOG.Write("税率取得", "失敗", "基準日：" & strKijunDate)
                        Return False
                    End If

                    '振込手数料マスタ読み込み
                    If Me.GetJifuriTesuuTable(Me.TAX.ZEIRITSU_ID) = False Then
                        Return False
                    End If

                    '2013/12/27 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
                    '印紙税取得
                    Me.TAX.GetInshizei(strKijunDate)
                    If Me.TAX.INSHIZEI_ID.Equals("err") = True Then
                        MainLOG.Write("印紙税取得", "失敗", "基準日：" & strKijunDate)
                        Return False
                    End If
                    '2013/12/27 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<
                End If
                '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

                PrnFunoumeisai.OutputCsvData(Jikinko_NAME)      '自金庫名
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T"))
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))      ' 委託者名漢字
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("ITAKU_CODE_T"))       ' 委託者コード
                PrnFunoumeisai.OutputCsvData(CASTCommon.ConvertDate(OraReader.GetItem("FURI_DATE_K"), "yyyyMMdd"))  '振替日
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("TORIMATOME_SIT_T"))   ' とりまとめ店コード
                PrnFunoumeisai.OutputCsvData(strTORIMATOME_SIT_NAME)        ' とりまとめ店名漢字
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("KEIYAKU_KIN_K"))      ' 金融機関コード
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("KEIYAKU_SIT_K"))      ' 支店
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("KEIYAKU_KNAME_K"), True)    ' 口座名義
                If OraReader.GetString("KEIYAKU_KAMOKU_K") = "01" Then
                    PrnFunoumeisai.OutputCsvData("当座")
                ElseIf OraReader.GetString("KEIYAKU_KAMOKU_K") = "02" Then
                    PrnFunoumeisai.OutputCsvData("普通")
                ElseIf OraReader.GetString("KEIYAKU_KAMOKU_K") = "05" Then
                    PrnFunoumeisai.OutputCsvData("納税")
                ElseIf OraReader.GetString("KEIYAKU_KAMOKU_K") = "37" Then
                    PrnFunoumeisai.OutputCsvData("職員")
                ElseIf OraReader.GetString("KEIYAKU_KAMOKU_K") = "09" Then
                    PrnFunoumeisai.OutputCsvData("その他")
                Else
                    PrnFunoumeisai.OutputCsvData("")
                End If

                PrnFunoumeisai.OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_K"))        ' 口座番号（契約者口座名）
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("FURIKIN_K").ToString)      ' 振替依頼金額（振替金額K）
                If OraReader.GetInt64("FURIKETU_CODE_K") = 0 Then
                    PrnFunoumeisai.OutputCsvData("")
                ElseIf OraReader.GetInt64("FURIKETU_CODE_K") = 1 Then
                    PrnFunoumeisai.OutputCsvData("1:資金不足")
                ElseIf OraReader.GetInt64("FURIKETU_CODE_K") = 2 Then
                    PrnFunoumeisai.OutputCsvData("2:取引なし")
                ElseIf OraReader.GetInt64("FURIKETU_CODE_K") = 3 Then
                    PrnFunoumeisai.OutputCsvData("3:預金者都合")
                ElseIf OraReader.GetInt64("FURIKETU_CODE_K") = 4 Then
                    PrnFunoumeisai.OutputCsvData("4:依頼書なし")
                ElseIf OraReader.GetInt64("FURIKETU_CODE_K") = 8 Then
                    PrnFunoumeisai.OutputCsvData("8:委託者都合")
                ElseIf OraReader.GetInt64("FURIKETU_CODE_K") = 9 Then
                    PrnFunoumeisai.OutputCsvData("9:その他")
                Else
                    PrnFunoumeisai.OutputCsvData("9:その他")
                End If
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("JYUYOUKA_NO_K"))      ' 備考　
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("FURI_KEN_S").ToString)     ' 振替済件数
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("FURI_KIN_S").ToString)     ' 振替済金額
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("FUNOU_KEN_S").ToString)    ' 振替不能件数
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("FUNOU_KIN_S").ToString)    ' 振替不能金額
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("SYORI_KEN_S").ToString)    ' 振替依頼件数
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("SYORI_KIN_S").ToString)    ' 振替依頼金額
                '*** 修正 mitsu 2009/06/11 合計件数に対する手数料考慮 ***
                'PrnKekkameisai.OutputCsvData(OraReader.GetInt64("TESUU_KIN_S"))         ' 手数料消費税込みS（手数料S）
                If OraReader.GetString("TESUUTYO_KBN_T") <> "2" AndAlso OraReader.GetString("TESUUMAT_PATN_T") = "1" Then
                    '手数料徴求区分が特別免除以外で手数料徴求方法が合計件数に対する手数料の場合は、
                    'マスタの手数料が同一手数料徴求日の合算値になっているので、
                    '単一スケジュールのみの手数料を算出する。
                    '2013/11/14 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
                    '引落手数料は別途計算する
                    If Me.CalcTesuuKin1(TesuuKin1, OraReader) = False Then
                        Return False
                    End If
                    'TesuuKin1 = OraReader.GetInt64("TESUU_KIN1") ' 引落手数料
                    '2013/11/14 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<
                    TesuuKin2 = OraReader.GetInt64("SOURYO_T") ' 送料
                    JifuriKin = OraReader.GetInt64("FURI_KIN_S") - (TesuuKin1 + TesuuKin2)
                    TesuuKin3 = 0

                    '2009/12/29
                    '振込手数料基準ＩＤを設定/計算式の追加
                    If OraReader.GetString("TESUU_TABLE_ID_T") = "" Then
                        intKIJYUN_ID = 0
                    Else
                        intKIJYUN_ID = CInt(OraReader.GetString("TESUU_TABLE_ID_T"))
                    End If

                    '2013/12/27 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                    If OraReader.GetString("TUKEKIN_NO_T") = Jikinko Then
                        ' 決済金融機関が，自金庫の場合
                        If OraReader.GetString("TUKESIT_NO_T") = OraReader.GetString("TORIMATOME_SIT_T") Then
                            ' 決済支店がとりまとめ店と一致する場合，自店内
                            If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN)
                            ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                                ' ０円より大きい かつ ３万円未満
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN)
                            ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                                ' ３万円以上
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN)
                            End If
                        Else
                            ' 決済支店がとりまとめ店と一致しない場合，本支店
                            If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                                ' ０円より大きい かつ １万円未満
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN)
                            ElseIf Me.TAX.INSHIZEI1 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                                ' １万円より大きい かつ ３万円未満
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN)
                            ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                                ' ３万円以上
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN)
                            End If
                        End If
                    Else
                        If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                            ' ０円より大きい かつ １万円未満
                            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU)
                        ElseIf Me.TAX.INSHIZEI1 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                            ' １万円円より大きい かつ ３万円未満
                            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU)
                        ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                            ' ３万円以上
                            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU)
                        End If
                    End If

                    'If OraReader.GetString("TUKEKIN_NO_T") = Jikinko Then
                    '    ' 決済金融機関が，自金庫の場合
                    '    If OraReader.GetString("TUKESIT_NO_T") = OraReader.GetString("TORIMATOME_SIT_NO_T") Then
                    '        ' 決済支店がとりまとめ店と一致する場合，自店内
                    '        If 0 < JifuriKin And JifuriKin < 10000 Then
                    '            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN)
                    '        ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                    '            ' ０円より大きい かつ ３万円未満
                    '            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN)
                    '        ElseIf 30000 <= JifuriKin Then
                    '            ' ３万円以上
                    '            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN)
                    '        End If
                    '    Else
                    '        ' 決済支店がとりまとめ店と一致しない場合，本支店
                    '        If 0 < JifuriKin And JifuriKin < 10000 Then
                    '            ' ０円より大きい かつ １万円未満
                    '            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN)
                    '        ElseIf 10000 < JifuriKin And JifuriKin < 30000 Then
                    '            ' １万円より大きい かつ ３万円未満
                    '            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN)
                    '        ElseIf 30000 <= JifuriKin Then
                    '            ' ３万円以上
                    '            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN)
                    '        End If
                    '    End If
                    'Else
                    '    If 0 < JifuriKin And JifuriKin < 10000 Then
                    '        ' ０円より大きい かつ １万円未満
                    '        TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU)
                    '    ElseIf 10000 < JifuriKin And JifuriKin < 30000 Then
                    '        ' １万円円より大きい かつ ３万円未満
                    '        TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU)
                    '    ElseIf 30000 <= JifuriKin Then
                    '        ' ３万円以上
                    '        TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU)
                    '    End If
                    'End If
                    '2013/12/27 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                    '=========================
                    'マスタの値ではなく算出した値を出力
                    PrnFunoumeisai.OutputCsvData((TesuuKin1 + TesuuKin2 + TesuuKin3).ToString)  ' 手数料消費税込みS（手数料S）
                Else
                    PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("TESUU_KIN_S").ToString)    ' 手数料消費税込みS（手数料S）
                End If
                '********************************************************

                PrnFunoumeisai.OutputCsvData(mMatchingDate)
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("KIN_NNAME_N"))
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("SIT_NNAME_N"))

                '2010/10/07.Sakon　フォーマット区分印字要否をINIファイルから設定 +++++
                If FMTKBN_PRINT = "1" Then
                    strFormatKubun = OraReader.GetString("FMT_KBN_T")
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    'フォーマット名をテキストから取得する
                    strFormatKubunName = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_フォーマット区分.TXT"), _
                                                                       strFormatKubun)
                    'Select Case strFormatKubun
                    '    Case "00"
                    '        strFormatKubunName = "全銀"
                    '    Case "01"
                    '        strFormatKubunName = "ＮＨＫ"
                    '    Case "02"
                    '        strFormatKubunName = "国税"
                    '    Case "03"
                    '        strFormatKubunName = "年金"
                    '    Case "04"
                    '        strFormatKubunName = "依頼書"
                    '    Case "05"
                    '        strFormatKubunName = "伝票"
                    '    Case Else
                    '        strFormatKubunName = ""
                    'End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END
                Else
                    strFormatKubunName = ""
                End If
                '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                PrnFunoumeisai.OutputCsvData(strFormatKubunName)                                'フォーマット区分
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("FURI_CODE_T"))                '振替コード
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("KIGYO_CODE_T"), False, True)  '企業コード

                '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
                TaxKey = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & OraReader.GetString("FURI_DATE_K")
                '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

                OraReader.NextRead()

                If OraReader.EOF = True OrElse OldKey <> OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") Then
                    If OraReader.EOF = False Then
                        OldKey = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")
                    End If
                End If

            Loop Until OraReader.EOF    ' EOFまで作業を繰り返す。

            OraReader.Close()

            ' 帳票出力

            'If PrnFunoumeisai.ReportExecute(PRINTERNAME) = True Then
            '    MainLOG.Write("振替不能明細表出力", "成功")
            'Else
            '    MainLOG.Write("振替不能明細表出力", "失敗", PrnFunoumeisai.ReportMessage)
            '    Return False
            'End If

            '2012/06/30 標準版　WEB伝送対応
            If User_ID <> "" Then
                If PrnFunoumeisai.ReportExecute(WEB_PRINTERNAME, User_ID, ITAKU_CODE) = True Then
                    MainLOG.Write("振替不能明細表出力", "成功")
                Else
                    MainLOG.Write("振替不能明細表出力", "失敗", PrnFunoumeisai.ReportMessage)
                    Return False
                End If

                If WEB_PRINT = "1" Then '区分が１の場合、通常使うプリンタでも印刷する
                    If PrnFunoumeisai.ReportExecute(PRINTERNAME) = True Then
                        MainLOG.Write("振替不能明細表出力", "成功")
                    Else
                        MainLOG.Write("振替不能明細表出力", "失敗", PrnFunoumeisai.ReportMessage)
                        Return False
                    End If
                End If
            Else
                ' 2015/12/18 タスク）綾部 CHG 【PG】UI_B-14-05(RSV2対応) -------------------- START
                'If PrnFunoumeisai.ReportExecute(PRINTERNAME) = True Then
                '    MainLOG.Write("振替不能明細表出力", "成功")
                'Else
                '    MainLOG.Write("振替不能明細表出力", "失敗", PrnFunoumeisai.ReportMessage)
                '    Return False
                'End If
                '=====================================================================================
                ' <INI_RSV2_J_FUNOU>
                '  CASE "1"  : 通常プリンタ印刷           (PRINTERNAME)
                '  CASE "2"  : ListWorks印刷              (INI_COMMON_PRINT_NAME)
                '  CASE "3"  : 通常プリンタ,ListWorks印刷 (PRINTERNAME,INI_COMMON_PRINT_NAME)
                '  CASE ELSE : 通常プリンタ印刷           (PRINTERNAME)
                '=====================================================================================
                Select Case INI_RSV2_J_FUNOU
                    Case "1"
                        If PrnFunoumeisai.ReportExecute(PRINTERNAME) = True Then
                            MainLOG.Write("振替不能明細表出力", "成功")
                        Else
                            MainLOG.Write("振替不能明細表出力", "失敗", PrnFunoumeisai.ReportMessage)
                            Return False
                        End If
                    Case "2"
                        If PrnFunoumeisai.ReportExecute(INI_COMMON_PRINT_NAME) = True Then
                            MainLOG.Write("振替不能明細表出力(ListWorks)", "成功")
                        Else
                            MainLOG.Write("振替不能明細表出力(ListWorks)", "失敗", PrnFunoumeisai.ReportMessage)
                            Return False
                        End If
                    Case "3"
                        If PrnFunoumeisai.ReportExecute(PRINTERNAME) = True Then
                            MainLOG.Write("振替不能明細表出力", "成功")
                        Else
                            MainLOG.Write("振替不能明細表出力", "失敗", PrnFunoumeisai.ReportMessage)
                            Return False
                        End If

                        If PrnFunoumeisai.ReportExecute(INI_COMMON_PRINT_NAME) = True Then
                            MainLOG.Write("振替不能明細表出力(ListWorks)", "成功")
                        Else
                            MainLOG.Write("振替不能明細表出力(ListWorks)", "失敗", PrnFunoumeisai.ReportMessage)
                            Return False
                        End If
                    Case Else
                        If PrnFunoumeisai.ReportExecute(PRINTERNAME) = True Then
                            MainLOG.Write("振替不能明細表出力", "成功")
                        Else
                            MainLOG.Write("振替不能明細表出力", "失敗", PrnFunoumeisai.ReportMessage)
                            Return False
                        End If
                End Select
                ' 2015/12/18 タスク）綾部 CHG 【PG】UI_B-14-05(RSV2対応) -------------------- END
            End If
 
            If Not PrnFunoumeisai.HostCsvName Is Nothing AndAlso PrnFunoumeisai.HostCsvName <> "" Then
                Try
                    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    DestName &= PrnFunoumeisai.HostCsvName
                    File.Copy(PrnFunoumeisai.FileName, DestName, True)
                Catch ex As Exception
                    MainLOG.Write("結果ＣＳＶ出力処理", "失敗", ex.Message)
                End Try
            End If

            Return True
        Else
            MainLOG.Write("対象データ ０件", "成功")
            '2010/02/18 0円データ対応
            Return True
            '========================
        End If

    End Function

    ''' <summary>
    ''' 引落手数料を計算します。
    ''' </summary>
    ''' <param name="oraReader"></param>
    ''' <returns>引落手数料</returns>
    ''' <remarks>2013/11/14 標準版 消費税対応</remarks>
    Private Function CalcTesuuKin1(ByRef TesuuKin1 As Long, _
                                   ByVal oraReader As CASTCommon.MyOracleReader) As Boolean
        Try
            '-------------------------------------------------------
            '引落手数料の計算
            '-------------------------------------------------------
            Dim intKen As Integer = 0
            Select Case oraReader.GetString("SEIKYU_KBN_T")
                Case "0" : intKen = oraReader.GetInt("SYORI_KEN_S")
                Case "1" : intKen = oraReader.GetInt("FURI_KEN_S")
                Case Else   '請求区分が他にあれば設定
            End Select

            Select Case oraReader.GetString("SYOUHI_KBN_T")
                Case "0" : TesuuKin1 = CLng(Math.Floor((oraReader.GetInt("KIHTESUU_T") * intKen + oraReader.GetInt("KOTEI_TESUU1_T") + oraReader.GetInt("KOTEI_TESUU2_T")) * Double.Parse(Me.TAX.ZEIRITSU)))
                Case "1" : TesuuKin1 = oraReader.GetInt("KIHTESUU_T") * intKen + oraReader.GetInt("KOTEI_TESUU1_T") + oraReader.GetInt("KOTEI_TESUU2_T")
                Case Else
            End Select

            Return True

        Catch ex As Exception
            MainLOG.Write("引落手数料計算", "失敗", ex.Message)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' 振込手数料基準IDテキストを読み込みます。
    ''' </summary>
    ''' <param name="TAX_ID">税率ID</param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/11/27 標準版 消費税対応</remarks>
    Private Function GetJifuriTesuuTable(ByVal TAX_ID As String) As Boolean

        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            With SQL
                .Append("select * from TESUUMAST")
                .Append(" where TAX_ID_C = " & SQ(TAX_ID))
                .Append(" and FSYORI_KBN_C = '1'")
                .Append(" and SYUBETU_C = '91'")
                .Append(" order by TESUU_TABLE_ID_C")
            End With

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
                    'If oraReader.GetInt("TESUU_TABLE_ID_C") > 0 AndAlso oraReader.GetInt("TESUU_TABLE_ID_C") < 10 Then
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).strKIJYUN_ID_CODE = oraReader.GetString("TESUU_TABLE_ID_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).strKIJYUN_ID_TEXT = oraReader.GetString("TESUU_TABLE_NAME_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng10000UNDER_JITEN = oraReader.GetInt("TESUU_A1_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000UNDER_JITEN = oraReader.GetInt("TESUU_A2_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000OVER_JITEN = oraReader.GetInt("TESUU_A3_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng10000UNDER_HONSITEN = oraReader.GetInt("TESUU_B1_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000UNDER_HONSITEN = oraReader.GetInt("TESUU_B2_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000OVER_HONSITEN = oraReader.GetInt("TESUU_B3_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng10000UNDER_TAKOU = oraReader.GetInt("TESUU_C1_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000UNDER_TAKOU = oraReader.GetInt("TESUU_C2_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000OVER_TAKOU = oraReader.GetInt("TESUU_C3_C")
                    'End If
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).strKIJYUN_ID_CODE = oraReader.GetString("TESUU_TABLE_ID_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).strKIJYUN_ID_TEXT = oraReader.GetString("TESUU_TABLE_NAME_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng10000UNDER_JITEN = oraReader.GetInt("TESUU_A1_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000UNDER_JITEN = oraReader.GetInt("TESUU_A2_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000OVER_JITEN = oraReader.GetInt("TESUU_A3_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng10000UNDER_HONSITEN = oraReader.GetInt("TESUU_B1_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000UNDER_HONSITEN = oraReader.GetInt("TESUU_B2_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000OVER_HONSITEN = oraReader.GetInt("TESUU_B3_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng10000UNDER_TAKOU = oraReader.GetInt("TESUU_C1_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000UNDER_TAKOU = oraReader.GetInt("TESUU_C2_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000OVER_TAKOU = oraReader.GetInt("TESUU_C3_C")
                    ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

                    oraReader.NextRead()
                End While
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("振込手数料マスタ読み込み", "失敗", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

End Class
