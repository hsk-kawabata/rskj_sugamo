Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic

Public Class ClsKouzafurikaeSeikyuDataInvoice

    ' 引数
    Public TORI_CODE As String          ' 取引先コード
    Public TORIS_CODE As String         ' 取引先主コード
    Public TORIF_CODE As String         ' 取引先副コード
    Public FURI_DATE As String          ' 振替日
    Public KEIYAKU_KIN As String           ' 相手金融機関
    Public PRINTERNAME As String = ""   ' 出力プリンタ

    ' ログ処理クラス
    Private LOG As CASTCommon.BatchLOG

    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle
    ' FSKJ.INI セクション名
    Private ReadOnly AppTOUROKU As String = "REPORTS"

    ' 機能   ： 口座振替請求データ送付票メイン処理
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
            LOG = New CASTCommon.BatchLOG("口座振替請求データ送付票", AppTOUROKU)
            LOG.Write("STEP出力", "", "LOG = New CASTCommon...")

            ' 口座振替請求データ送付票印刷処理
            bRet = PrintKouzafurikaeSeikyuDataInvoice()
            LOG.Write("STEP出力", "", "bRet = PrintFurikaed...")

        Catch ex As Exception
            LOG.Write("口座振替請求データ送付票", "失敗", ex.Message & ":" & ex.StackTrace)

            Return -1
        Finally
            MainDB.Close()
            LOG.Write("STEP出力", "", "MainDB.Close()...")
        End Try

        If bRet = True Then
            LOG.Write("STEP出力", "", "If bRet = True Then...")
            Return 0
        Else
            LOG.Write("STEP出力", "", "Else...")
            Return 2
        End If

    End Function

    ' 機能   ： 他行明細表出力処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考   ： 
    '
    Private Function PrintKouzafurikaeSeikyuDataInvoice() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim TenSQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim OraTenReader As New CASTCommon.MyOracleReader(MainDB)

        Dim PrnKouzafurikaeSeikyuDataInvoice As New ClsPrnKouzafurikaeSeikyuDataInvoice    '口座振替請求データ送付票
        Dim JIKINKO_NAME As String
        Dim KINKOBUSYO As String        '担当部署
        Dim KINKOTANTO As String        '担当者
        Dim KINKOTEL As String          '金融機関電話番号
        Dim KINKOFAX As String          '金融機関ＦＡＸ番号

        JIKINKO_NAME = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")
        If JIKINKO_NAME = "err" Then
            JIKINKO_NAME = ""
        End If
        KINKOBUSYO = CASTCommon.GetFSKJIni("PRINT", "KINKOBUSYO")
        If KINKOBUSYO = "err" Then
            KINKOBUSYO = ""
        End If
        KINKOTANTO = CASTCommon.GetFSKJIni("PRINT", "KINKOTANTO")
        If KINKOTANTO = "err" Then
            KINKOTANTO = ""
        End If
        KINKOTEL = CASTCommon.GetFSKJIni("PRINT", "KINKOTEL")
        If KINKOTEL = "err" Then
            KINKOTEL = ""
        End If
        KINKOFAX = CASTCommon.GetFSKJIni("PRINT", "KINKOFAX")
        If KINKOFAX = "err" Then
            KINKOFAX = ""
        End If
        '2010/02/19 農協まとめ取得項目追加
        Dim MATOME_NAME As String
        Dim MATOME_FROM As String
        Dim MATOME_TO As String
        Dim MATOME_TEL As String
        Dim MATOME_FAX As String
        MATOME_NAME = CASTCommon.GetFSKJIni("TAKO", "NOKYOCENTER")
        If MATOME_NAME = "err" Then
            MATOME_NAME = ""
        End If
        MATOME_FROM = CASTCommon.GetFSKJIni("TAKO", "NOUKYOFROM")
        If MATOME_FROM = "err" Then
            MATOME_FROM = ""
        End If
        MATOME_TO = CASTCommon.GetFSKJIni("TAKO", "NOUKYOTO")
        If MATOME_TO = "err" Then
            MATOME_TO = ""
        End If
        MATOME_TEL = CASTCommon.GetFSKJIni("TAKO", "NOKYOTEL")
        If MATOME_TEL = "err" Then
            MATOME_TEL = ""
        End If
        MATOME_FAX = CASTCommon.GetFSKJIni("TAKO", "NOKYOFAX")
        If MATOME_FAX = "err" Then
            MATOME_FAX = ""
        End If
        '=======================

        SQL.Append("SELECT TKIN_NO_U")
        SQL.Append(", TORIS_CODE_U")
        SQL.Append(", TORIF_CODE_U")
        SQL.Append(", FURI_DATE_U")
        SQL.Append(", SYORI_KEN_U")
        SQL.Append(", SYORI_KIN_U")
        SQL.Append(", KIN_NNAME_N")
        SQL.Append(", ITAKU_CODE_T")
        SQL.Append(", ITAKU_NNAME_T")
        SQL.Append(", KIN_NO_N")
        '2010/01/25 支店コードを保持 
        SQL.Append(", MIN(SIT_NO_N) SIT_NO_N")
        '==========================
        SQL.Append(" FROM TAKOSCHMAST")
        SQL.Append(", (SELECT MIN(KIN_NO_N) KIN_NO_N")
        SQL.Append(", KIN_NNAME_N")
        '2010/01/25 支店コードを保持 
        SQL.Append(", MIN(SIT_NO_N) SIT_NO_N")
        '==========================
        SQL.Append(" FROM TENMAST")
        SQL.Append(" WHERE SIT_NO_N = (SELECT TSIT_NO_V")
        SQL.Append("  FROM TAKOUMAST")
        SQL.Append("  WHERE TORIS_CODE_V = " & SQ(TORIS_CODE))
        SQL.Append("  AND TORIF_CODE_V = " & SQ(TORIF_CODE))
        SQL.Append("  AND TKIN_NO_V = " & SQ(KEIYAKU_KIN))
        SQL.Append(")")
        SQL.Append(" GROUP BY KIN_NO_N")
        SQL.Append(", KIN_NNAME_N) TENMAST")
        SQL.Append(", (SELECT FSYORI_KBN_T")
        SQL.Append(", TORIS_CODE_T")
        SQL.Append(", TORIF_CODE_T")
        SQL.Append(", ITAKU_CODE_T")
        SQL.Append(", ITAKU_NNAME_T")
        SQL.Append(" FROM TORIMAST")
        SQL.Append(" WHERE FSYORI_KBN_T = '1')")
        SQL.Append(" WHERE TORIS_CODE_U = '" & TORIS_CODE & "'")
        SQL.Append(" AND TORIF_CODE_U = '" & TORIF_CODE & "'")
        SQL.Append(" AND FURI_DATE_U = '" & FURI_DATE & "'")
        SQL.Append(" AND TKIN_NO_U = '" & KEIYAKU_KIN & "'")
        SQL.Append(" AND TKIN_NO_U = KIN_NO_N (+)")
        SQL.Append(" AND TORIS_CODE_U = TORIS_CODE_T")
        SQL.Append(" AND TORIF_CODE_U = TORIF_CODE_T")
        SQL.Append(" GROUP BY TKIN_NO_U")                           '金融機関コードでグループ化
        SQL.Append(", TORIS_CODE_U")
        SQL.Append(", TORIF_CODE_U")
        SQL.Append(", FURI_DATE_U")
        SQL.Append(", SYORI_KEN_U")
        SQL.Append(", SYORI_KIN_U")
        SQL.Append(", KIN_NNAME_N")
        SQL.Append(", ITAKU_CODE_T")
        SQL.Append(", ITAKU_NNAME_T")
        SQL.Append(", KIN_NO_N")
        SQL.Append(" ORDER BY TORIS_CODE_U")
        SQL.Append(", TORIF_CODE_U")
        SQL.Append(", FURI_DATE_U")
        SQL.Append(", TKIN_NO_U")

        Dim name As String = ""
        Dim blnSQL As Boolean
        Dim strDENWA_N As String = ""
        Dim strFAX_N As String = ""

        Dim strSystemDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
        Dim strTimeStamp As String = CASTCommon.Calendar.Now.ToString("HHmmss")

        blnSQL = OraReader.DataReader(SQL)

        If blnSQL = True Then

            name = PrnKouzafurikaeSeikyuDataInvoice.CreateCsvFile()

            Do
                TenSQL = New StringBuilder(128)
                TenSQL.Append("SELECT MIN(KIN_NO_N) KIN_NO_N")
                TenSQL.Append(", MIN(SIT_NO_N) SIT_NO_N")
                TenSQL.Append(", DENWA_N")
                TenSQL.Append(", FAX_N")
                TenSQL.Append(" FROM SITEN_INFOMAST")
                TenSQL.Append(" WHERE KIN_NO_N = '" & OraReader.GetString("KIN_NO_N") & "'")
                '2010/01/25 支店コードを条件に追加
                TenSQL.Append(" AND SIT_NO_N = '" & OraReader.GetString("SIT_NO_N") & "'")
                '===================================
                TenSQL.Append(" GROUP BY KIN_NO_N")
                TenSQL.Append(", SIT_NO_N")
                TenSQL.Append(", DENWA_N")
                TenSQL.Append(", FAX_N")
                TenSQL.Append(" ORDER BY KIN_NO_N")
                TenSQL.Append(", SIT_NO_N")

                If OraTenReader.DataReader(TenSQL) = True Then
                    strDENWA_N = OraTenReader.GetString("DENWA_N")
                    strFAX_N = OraTenReader.GetString("FAX_N")
                Else
                    strDENWA_N = ""
                    strFAX_N = ""
                End If

                PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(strSystemDate)                           ' 処理日（システム日付）
                '2010/02/19 農協まとめ対応追加
                Select Case OraReader.GetString("KIN_NO_N")
                    Case MATOME_FROM To MATOME_TO
                        PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(MATOME_NAME)                     ' 相手金融機関名
                        PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(MATOME_TEL)                      ' 相手電話番号
                        PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(MATOME_FAX)                      ' 相手ＦＡＸ番号
                    Case Else
                        PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(OraReader.GetString("KIN_NNAME_N"))      ' 相手金融機関名
                        PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(strDENWA_N)                              ' 相手電話番号
                        PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(strFAX_N)                                ' 相手ＦＡＸ番号
                End Select
                '==============================
                PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(JIKINKO_NAME)                            ' 金融機関名（KINKONAME）
                PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(KINKOBUSYO)                              ' 担当部署名
                PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(KINKOTANTO)                              ' 担当者名
                PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(KINKOTEL)                                ' 電話番号
                PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(KINKOFAX)                                ' ＦＡＸ番号
                PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))    ' 委託者名
                PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(OraReader.GetString("FURI_DATE_U"))      ' 振替日
                PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(OraReader.GetString("SYORI_KEN_U"))      ' 処理件数
                PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(OraReader.GetString("SYORI_KIN_U"), False, True) '処理金額

                OraReader.NextRead()
            Loop Until OraReader.EOF
            OraReader.Close()
            OraTenReader.Close()

            If PrnKouzafurikaeSeikyuDataInvoice.ReportExecute() = True Then
                LOG.Write("印刷", "成功")
                Return True
            Else
                LOG.Write("印刷", "失敗", PrnKouzafurikaeSeikyuDataInvoice.ReportMessage)
                Return False
            End If
        Else
            LOG.Write("印刷対象データ０件", "成功")
            OraReader.Close()
            OraTenReader.Close()
            LOG.Write("STEP出力", "", "OraReader.Close()...")

            Return True
        End If
    End Function
End Class
