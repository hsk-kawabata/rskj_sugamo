Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic

Public Class ClsTakouMeisaiList

    ' 引数
    Public TORI_CODE As String          ' 取引先コード
    Public TORIS_CODE As String         ' 取引先主コード
    Public TORIF_CODE As String         ' 取引先副コード
    Public FURI_DATE As String          ' 振替日
    Public KEIYAKU_KIN As String        ' 委託者金融機関コード
    Public PRINTERNAME As String = ""   ' 出力プリンタ

    ' ログ処理クラス
    Private LOG As CASTCommon.BatchLOG

    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle
    ' FSKJ.INI セクション名
    Private ReadOnly AppTOUROKU As String = "REPORTS"

    ' 機能   ： 他行明細表印刷メイン処理
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
            LOG = New CASTCommon.BatchLOG("他行明細表印刷", AppTOUROKU)
            LOG.Write("STEP出力", "", "LOG = New CASTCommon...")

            ' 他行明細表印刷処理
            bRet = PrintTakouMeisaiList()
            LOG.Write("STEP出力", "", "bRet = PrintFurikaed...")

        Catch ex As Exception
            LOG.Write("他行明細表印刷", "失敗", ex.Message & ":" & ex.StackTrace)

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
    Private Function PrintTakouMeisaiList() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim PrnTakouMeisaiList As New ClsPrnTakouMeisaiList    '他行明細表印刷

        SQL = New StringBuilder(128)
        SQL.Append("SELECT TORIS_CODE_K")
        SQL.Append(", TORIF_CODE_K")
        SQL.Append(", FURI_DATE_K")
        SQL.Append(", KEIYAKU_KIN_K")
        SQL.Append(", KEIYAKU_SIT_K")
        SQL.Append(", KEIYAKU_KAMOKU_K")
        SQL.Append(", KEIYAKU_KOUZA_K")
        SQL.Append(", KEIYAKU_KNAME_K")
        SQL.Append(", FURIKIN_K")
        SQL.Append(", JYUYOUKA_NO_K")
        SQL.Append(", RECORD_NO_K")
        SQL.Append(", DATA_KBN_K")
        SQL.Append(", KIN_NNAME_N")
        SQL.Append(", SIT_NNAME_N")
        SQL.Append(", ITAKU_NNAME_T")
        SQL.Append(" FROM MEIMAST")
        SQL.Append(", (SELECT KIN_NO_N")
        SQL.Append(", KIN_NNAME_N")
        SQL.Append(", SIT_NO_N")
        SQL.Append(", SIT_NNAME_N")
        SQL.Append(" FROM TENMAST")
        SQL.Append(" GROUP BY KIN_NO_N")
        SQL.Append(", SIT_NO_N")
        SQL.Append(", KIN_NNAME_N")
        SQL.Append(", SIT_NNAME_N)")
        SQL.Append(", (SELECT FSYORI_KBN_T")
        SQL.Append(", TORIS_CODE_T")
        SQL.Append(", TORIF_CODE_T")
        SQL.Append(", ITAKU_NNAME_T")
        SQL.Append(" FROM TORIMAST")
        SQL.Append(" WHERE FSYORI_KBN_T = '1')")
        SQL.Append(" WHERE TORIS_CODE_K = '" & TORIS_CODE & "'")
        SQL.Append(" AND TORIF_CODE_K = '" & TORIF_CODE & "'")
        SQL.Append(" AND FURI_DATE_K = '" & FURI_DATE & "'")
        SQL.Append(" AND KEIYAKU_KIN_K = '" & KEIYAKU_KIN & "'")
        SQL.Append(" AND KEIYAKU_KIN_K = KIN_NO_N (+)")
        SQL.Append(" AND KEIYAKU_SIT_K = SIT_NO_N (+)")
        SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_T")
        SQL.Append(" AND TORIF_CODE_K = TORIF_CODE_T")
        SQL.Append(" AND DATA_KBN_K = '2'")
        SQL.Append(" ORDER BY TORIS_CODE_K")
        SQL.Append(", TORIF_CODE_K")
        SQL.Append(", FURI_DATE_K")
        SQL.Append(", KEIYAKU_KIN_K")
        SQL.Append(", RECORD_NO_K")

        Dim name As String = ""
        Dim bSQL As Boolean
        bSQL = OraReader.DataReader(SQL)

        Dim strSystemDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
        Dim strTimeStamp As String = CASTCommon.Calendar.Now.ToString("HHmmss")
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        'Dim strTORI_CODE As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        If bSQL = True Then
            LOG.Write("STEP出力", "", "If bSQL = True Then...")

            ' ＣＳＶを作成する
            name = PrnTakouMeisaiList.CreateCsvFile()
            LOG.Write("STEP出力", "", "name = PrnTakouMeisaiList...")

            Do
                PrnTakouMeisaiList.OutputCsvData(OraReader.GetString("FURI_DATE_K"))        ' 振替日
                PrnTakouMeisaiList.OutputCsvData(strSystemDate)                             ' 処理日（システム日付）
                PrnTakouMeisaiList.OutputCsvData(strTimeStamp)                              ' タイムスタンプ
                PrnTakouMeisaiList.OutputCsvData(OraReader.GetString("KEIYAKU_KIN_K"))      ' 他行金融機関コード
                PrnTakouMeisaiList.OutputCsvData(OraReader.GetString("KIN_NNAME_N"))        ' 他行金融機関名
                PrnTakouMeisaiList.OutputCsvData(OraReader.GetString("TORIS_CODE_K"))       ' 取引先主コード
                PrnTakouMeisaiList.OutputCsvData(OraReader.GetString("TORIF_CODE_K"))       ' 取引先副コード
                PrnTakouMeisaiList.OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))      ' 委託者名
                PrnTakouMeisaiList.OutputCsvData(OraReader.GetString("KEIYAKU_SIT_K"))      ' 支店コード
                PrnTakouMeisaiList.OutputCsvData(OraReader.GetString("SIT_NNAME_N"))        ' 支店名
                Select Case OraReader.GetString("KEIYAKU_KAMOKU_K")
                    Case "01"
                        PrnTakouMeisaiList.OutputCsvData("当座")
                    Case "02"
                        PrnTakouMeisaiList.OutputCsvData("普通")
                    Case "05"
                        PrnTakouMeisaiList.OutputCsvData("納税")
                    Case "37"
                        PrnTakouMeisaiList.OutputCsvData("職員")
                    Case "04"
                        PrnTakouMeisaiList.OutputCsvData("別段")
                    Case "99"
                        PrnTakouMeisaiList.OutputCsvData("諸勘定")
                    Case Else
                        PrnTakouMeisaiList.OutputCsvData("")
                End Select
                PrnTakouMeisaiList.OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_K"))    ' 口座番号
                PrnTakouMeisaiList.OutputCsvData(OraReader.GetString("KEIYAKU_KNAME_K"), True)    ' 預金者名
                PrnTakouMeisaiList.OutputCsvData(OraReader.GetString("FURIKIN_K"))          ' 振替金額
                PrnTakouMeisaiList.OutputCsvData(OraReader.GetString("JYUYOUKA_NO_K"), False, True)      ' 需要家番号

                'strTORI_CODE = OraReader.GetString("TORIS_CODE_K") & OraReader.GetString("TORIF_CODE_K")
                'PrnTakouMeisaiList.OutputCsvData(strTORI_CODE, False, True)                 ' 取引先コード

                OraReader.NextRead()
            Loop Until OraReader.EOF    ' EOFまで作業を繰り返す。
            OraReader.Close()
            LOG.Write("STEP出力", "", "OraReader.Close()...")

            If PrnTakouMeisaiList.ReportExecute() = True Then
                LOG.Write("印刷", "成功")
                Return True
            Else
                LOG.Write("印刷", "失敗", PrnTakouMeisaiList.ReportMessage)
                Return False
            End If
        Else
            LOG.Write("印刷対象データ０件", "成功")
            OraReader.Close()
            LOG.Write("STEP出力", "", "OraReader.Close()...")

            Return True
        End If

    End Function
End Class
