Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic

' 
Public Class ClsFurikaeFnouSyuukei

    ' 引数
    Public TORIS_CODE As String         ' 取引先主コード
    Public TORIF_CODE As String         ' 取引先副コード
    Public FURI_DATE As String          ' 振替日
    Public PRINTERNAME As String = ""   ' 出力プリンタ
    Public INVOKE_KBN As String = ""    ' 呼び出し区分      '2012/06/30 標準版　WEB伝送対応

    ' ログ処理クラス
    Private LOG As CASTCommon.BatchLOG

    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    ' FSKJ.INI セクション名
    Private ReadOnly AppTOUROKU As String = "REPORTS"

    ' 機能   ： 振替不能事由別集計表メイン処理
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
            LOG = New CASTCommon.BatchLOG("振替不能事由別集計表", AppTOUROKU)

            LOG.ToriCode = TORIS_CODE & TORIF_CODE
            LOG.FuriDate = FURI_DATE

            ' 振替不能事由別集計表印刷処理
            bRet = PrintFunouSyuukei()

        Catch ex As Exception
            LOG.Write("振替不能事由別集計表", "失敗", ex.Message & ":" & ex.StackTrace)

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

    ' 機能   ： 振替不能事由別集計表処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考   ： 
    '
    Private Function PrintFunouSyuukei() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim sFMTKbn As String = ""

        ' 自金庫コード
        Dim Jikinko As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

        '自金庫名をiniファイルから取得
        Dim Jikinko_NAME As String = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")
        'とりまとめ店名
        Dim strTORIMATOME_SIT_T As String = ""
        Dim strTORIMATOME_SIT_NAME As String = ""
        Dim strITAKU_KANRI_CODE As String = ""

        '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
        '' 消費税
        'Dim sTax As String = CASTCommon.GetFSKJIni("COMMON", "ZEIRITU")
        'If sTax = "err" Then
        '    sTax = "1.05"
        'End If
        '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<

        Dim TesuuKin1 As Long = 0                    ' 手数料金額１
        Dim TesuuKin2 As Long = 0                    ' 手数料金額２
        Dim TesuuKin3 As Long = 0                    ' 手数料金額３
        Dim JifuriKin As Long = 0                    ' 自振金額

        '2012/06/30 標準版　WEB伝送対応
        Dim User_ID As String = ""  'ユーザＩＤ
        Dim ITAKU_CODE As String = "" '委託者コード
        Dim WEB_PRINTERNAME As String = "" 'WEB伝送用プリンタ名
        Dim WEB_PRINT As String = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_PRINT")    'WEB伝送プリント区分(0:PDFのみ、1:PDFと紙）

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

        Dim PrnFurikaeFunouSyuukei As New ClsPrnFurikaeFunouSyuukei    ' 振替不能事由別集計表

        SQL = New StringBuilder(128)
        '2013/11/14 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
        '消費税対応と同時に不要な項目をコメントアウトする。
        SQL.Append("SELECT")
        SQL.Append(" TORIMAST.TORIS_CODE_T")
        SQL.Append(",TORIMAST.TORIF_CODE_T")
        SQL.Append(",TORIMAST.ITAKU_NNAME_T")
        SQL.Append(",MEIMAST.FURI_DATE_K")
        SQL.Append(",TORIMAST.TORIMATOME_SIT_T")
        'SQL.Append(",TORIMAST.FUNOU_MEISAI_KBN_T")
        'SQL.Append(",TENMAST.KIN_NNAME_N")
        'SQL.Append(",TENMAST.SIT_NNAME_N")
        'SQL.Append(",MEIMAST.ITAKU_KIN_K")
        'SQL.Append(",MEIMAST.ITAKU_SIT_K")
        'SQL.Append(",MEIMAST.KEIYAKU_KIN_K")
        'SQL.Append(",MEIMAST.KEIYAKU_SIT_K")
        'SQL.Append(",MEIMAST.KEIYAKU_KAMOKU_K")
        'SQL.Append(",MEIMAST.KEIYAKU_KOUZA_K")
        'SQL.Append(",MEIMAST.KEIYAKU_KNAME_K")
        SQL.Append(",MEIMAST.FURIKIN_K")
        SQL.Append(",MEIMAST.FURIKETU_CODE_K")
        'SQL.Append(",MEIMAST.JYUYOUKA_NO_K")
        'SQL.Append(",SCHMAST.SYORI_KEN_S")
        'SQL.Append(",SCHMAST.SYORI_KIN_S")
        'SQL.Append(",SCHMAST.FUNOU_KEN_S")
        'SQL.Append(",SCHMAST.FUNOU_KIN_S")
        'SQL.Append(",SCHMAST.FURI_KEN_S")
        'SQL.Append(",SCHMAST.FURI_KIN_S")
        'SQL.Append(",SCHMAST.TESUU_KIN_S")
        'SQL.Append(",SCHMAST.TESUU_KIN1_S")
        'SQL.Append(",SCHMAST.TESUU_KIN2_S")
        'SQL.Append(",SCHMAST.TESUU_KIN3_S")
        SQL.Append(",TORIMAST.ITAKU_CODE_T")
        'SQL.Append(",TORIMAST.TESUUTYO_KBN_T")
        'SQL.Append(",TORIMAST.TESUUMAT_PATN_T")
        'SQL.Append(",TRUNC(")
        'SQL.Append("  ((KIHTESUU_T * DECODE(SEIKYU_KBN_T,'0',SYORI_KEN_S,FURI_KEN_S) / 100)")
        'SQL.Append("   + NVL(KOTEI_TESUU1_T,0)")
        'SQL.Append("   + NVL(KOTEI_TESUU2_T,0))")
        'SQL.Append("   * DECODE(SYOUHI_KBN_T,'0'," & sTax & ",1)) TESUU_KIN1")     ' 引落手数料
        'SQL.Append(",SOURYO_T")                       ' 送料
        'SQL.Append(",TESUU1_T")                       ' 基準別手数料１
        'SQL.Append(",TESUU2_T")                       ' 基準別手数料２
        'SQL.Append(",TESUU3_T")                       ' 基準別手数料３
        'SQL.Append(",TESUU4_T")                       ' 基準別手数料４
        'SQL.Append(",TESUU5_T")                       ' 基準別手数料５
        'SQL.Append(",TESUU6_T")                       ' 基準別手数料６
        'SQL.Append(",TUKEKIN_NO_T")                   ' 決済金融機関
        'SQL.Append(",TUKESIT_NO_T")                   ' 決済支店
        'SQL.Append(",TORIMATOME_SIT_T")              ' とりまとめ店
        'SQL.Append(", SCHMAST.FILE_SEQ_S")
        ' 2012/06/30 標準版　WEB伝送対応------------------------------------->
        SQL.Append(", TORIMAST.BAITAI_CODE_T")      '媒体コード
        SQL.Append(", TORIMAST.MULTI_KBN_T")        'マルチ区分
        SQL.Append(", TORIMAST.ITAKU_KANRI_CODE_T") '代表委託者コード
        ' 2012/06/30 標準版　WEB伝送対応-------------------------------------<
        SQL.Append(" FROM TORIMAST")
        'SQL.Append(", (SELECT KIN_NO_N")
        'SQL.Append(", SIT_NO_N")
        'SQL.Append(", KIN_NNAME_N")
        'SQL.Append(", SIT_NNAME_N")
        'SQL.Append(" FROM TENMAST")
        'SQL.Append(" GROUP BY KIN_NO_N")
        'SQL.Append(", SIT_NO_N")
        'SQL.Append(", KIN_NNAME_N")
        'SQL.Append(", SIT_NNAME_N) TENMAST")
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
        'SQL.Append(" AND TENMAST.KIN_NO_N = MEIMAST.KEIYAKU_KIN_K")
        'SQL.Append(" AND TENMAST.SIT_NO_N = MEIMAST.KEIYAKU_SIT_K")
        SQL.Append(" ORDER BY SCHMAST.FILE_SEQ_S")
        SQL.Append(", SCHMAST.TORIS_CODE_S")
        SQL.Append(", SCHMAST.TORIF_CODE_S")
        SQL.Append(", MEIMAST.RECORD_NO_K")
        '2013/11/14 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<

        Dim name As String = ""
        'Dim KinkoCd As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

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

            name = PrnFurikaeFunouSyuukei.CreateCsvFile()

            Dim OldKey As String = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")

            Do
                PrnFurikaeFunouSyuukei.OutputCsvData(CASTCommon.ConvertDate(OraReader.GetItem("FURI_DATE_K"), "yyyyMMdd"))  '振替日
                PrnFurikaeFunouSyuukei.OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))          ' 委託者名漢字
                PrnFurikaeFunouSyuukei.OutputCsvData(OraReader.GetString("ITAKU_CODE_T"))           ' 委託者コード

                PrnFurikaeFunouSyuukei.OutputCsvData(Jikinko_NAME)      '自金庫名
                PrnFurikaeFunouSyuukei.OutputCsvData(OraReader.GetString("TORIMATOME_SIT_T"))       ' とりまとめ店コード
                PrnFurikaeFunouSyuukei.OutputCsvData(strTORIMATOME_SIT_NAME)                        ' とりまとめ店名漢字
                PrnFurikaeFunouSyuukei.OutputCsvData(OraReader.GetString("FURIKETU_CODE_K"))        ' 振替結果コード
                PrnFurikaeFunouSyuukei.OutputCsvData(OraReader.GetInt64("FURIKIN_K").ToString)      ' 振替依頼金額（振替金額K）

                PrnFurikaeFunouSyuukei.OutputCsvData(mMatchingDate, False, True)

                OraReader.NextRead()

                If OraReader.EOF = True OrElse OldKey <> OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") Then
                    If OraReader.EOF = False Then
                        OldKey = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")
                    End If
                End If

            Loop Until OraReader.EOF    ' EOFまで作業を繰り返す。

            OraReader.Close()

            ' 帳票出力
            'If PrnFurikaeFunouSyuukei.ReportExecute(PRINTERNAME) = True Then
            '    LOG.Write("振替不能事由別集計表出力", "成功")
            'Else
            '    LOG.Write("振替不能事由別集計表出力", "失敗", PrnFurikaeFunouSyuukei.ReportMessage)
            '    Return False
            'End If

            '2012/06/30 標準版　WEB伝送対応
            If User_ID <> "" Then
                If PrnFurikaeFunouSyuukei.ReportExecute(WEB_PRINTERNAME, User_ID, ITAKU_CODE) = True Then
                    LOG.Write("振替不能事由別集計表出力", "成功")
                Else
                    LOG.Write("振替不能事由別集計表出力", "失敗", PrnFurikaeFunouSyuukei.ReportMessage)
                    Return False
                End If

                If WEB_PRINT = "1" Then '区分が１の場合、通常使うプリンタでも印刷する
                    If PrnFurikaeFunouSyuukei.ReportExecute(PRINTERNAME) = True Then
                       LOG.Write("振替不能事由別集計表出力", "成功")
                    Else
                        LOG.Write("振替不能事由別集計表出力", "失敗", PrnFurikaeFunouSyuukei.ReportMessage)
                        Return False
                    End If
                End If
            Else
                If PrnFurikaeFunouSyuukei.ReportExecute(PRINTERNAME) = True Then
                    LOG.Write("振替不能事由別集計表出力", "成功")
                Else
                    LOG.Write("振替不能事由別集計表出力", "失敗", PrnFurikaeFunouSyuukei.ReportMessage)
                    Return False
                End If
            End If

            If Not PrnFurikaeFunouSyuukei.HostCsvName Is Nothing AndAlso PrnFurikaeFunouSyuukei.HostCsvName <> "" Then
                Try
                    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    DestName &= PrnFurikaeFunouSyuukei.HostCsvName
                    File.Copy(PrnFurikaeFunouSyuukei.FileName, DestName, True)
                Catch ex As Exception
                    LOG.Write("振替不能事由別集計表出力", "失敗", ex.Message)
                End Try
            End If

            Return True
        Else
            LOG.Write("対象データ ０件", "成功")
            '20190910 maeda 対象0件時の対応
            Return True
            '20190910 maeda 対象0件時の対応
        End If
    End Function

End Class
