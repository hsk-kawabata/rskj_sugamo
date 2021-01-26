Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports MenteCommon.clsCommon
Public Class KFJP058
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP058"

        ' 定義体名セット
        ReportBaseName = "KFJP058_データ伝送通知書.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        'タイトル行
        CSVObject.Output("処理日")
        CSVObject.Output("郵送先名漢字")
        CSVObject.Output("電話番号")
        CSVObject.Output("FAX番号")
        CSVObject.Output("金融機関名")
        CSVObject.Output("担当部署")
        CSVObject.Output("担当者名")
        CSVObject.Output("金庫電話番号")
        CSVObject.Output("金庫FAX番号")
        CSVObject.Output("委託者名")
        CSVObject.Output("振替日")
        CSVObject.Output("請求件数")
        CSVObject.Output("請求金額")
        CSVObject.Output("振替件数")
        CSVObject.Output("振替金額")
        CSVObject.Output("不能件数")
        CSVObject.Output("不能金額", False, True)

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
        Dim strKinkoName As String
        Dim strKinkoBusyo As String
        Dim strKinkoTanto As String
        Dim strKinkoTel As String
        Dim strKinkoFax As String
        Dim strErr As String = "err"
        Dim lngSeikyuKen As Long
        Dim lngSeikyuKin As Long
        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT")
            SQL.Append(" TORI.YUUBIN_NNAME_T YUUBIN_NNAME_T")
            SQL.Append(", TORI.DENWA_T DENWA_T")
            SQL.Append(", TORI.FAX_T FAX_T")
            SQL.Append(", TORI.ITAKU_NNAME_T ITAKU_NNAME_T")
            SQL.Append(", SCH.FURI_DATE_S FURI_DATE_S")
            SQL.Append(", SCH.SYORI_KEN_S SYORI_KEN_S")
            SQL.Append(", SCH.SYORI_KIN_S SYORI_KIN_S")
            SQL.Append(", SCH.FURI_KEN_S FURI_KEN_S")
            SQL.Append(", SCH.FURI_KIN_S FURI_KIN_S")
            SQL.Append(", SCH.ERR_KEN_S ERR_KEN_S")
            SQL.Append(", SCH.ERR_KIN_S ERR_KIN_S")
            SQL.Append(", SCH.FUNOU_KEN_S FUNOU_KEN_S")
            SQL.Append(", SCH.FUNOU_KIN_S FUNOU_KIN_S")
            SQL.Append(", SCH.TORIS_CODE_S TORIS_CODE_S")
            SQL.Append(", SCH.TORIF_CODE_S TORIF_CODE_S")
            SQL.Append(" FROM SCHMAST SCH, TORIMAST TORI")
            SQL.Append(" WHERE SCH.FURI_DATE_S = " & FURI_DATE)
            SQL.Append(" AND SCH.TORIS_CODE_S = " & TORIS_CODE)
            SQL.Append(" AND SCH.TORIF_CODE_S = " & TORIF_CODE)
            SQL.Append(" AND SCH.FUNOU_FLG_S = '1'")
            SQL.Append(" AND SCH.HENKAN_FLG_S = '1'")
            SQL.Append(" AND SCH.TORIS_CODE_S = TORI.TORIS_CODE_T")
            SQL.Append(" AND SCH.TORIF_CODE_S = TORI.TORIF_CODE_T")

            If oraReader.DataReader(SQL) = True Then

                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                        '処理日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("YUUBIN_NNAME_T")))    '郵送先名漢字
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("DENWA_T")))           '電話番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FAX_T")))             'FAX番号
                    strKinkoName = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")          '金庫名(INIファイルから取得)
                    If strKinkoName <> strErr Then
                        OutputCsvData(strKinkoName)
                    Else
                        BatchLog.Write("レコード作成", "失敗", "INIファイル情報取得失敗[PRINT]KINKONAME")
                        Return False
                    End If
                    strKinkoBusyo = CASTCommon.GetFSKJIni("PRINT", "KINKOBUSYO")        '部署名(INIファイルから取得)
                    If strKinkoBusyo <> strErr Then
                        OutputCsvData(strKinkoBusyo)
                    Else
                        BatchLog.Write("レコード作成", "失敗", "INIファイル情報取得失敗[PRINT]KINKOBUSYO")
                        Return False
                    End If
                    strKinkoTanto = CASTCommon.GetFSKJIni("PRINT", "KINKOTANTO")        '担当者名(INIファイルから取得)
                    If strKinkoTanto <> strErr Then
                        OutputCsvData(strKinkoTanto)
                    Else
                        BatchLog.Write("レコード作成", "失敗", "INIファイル情報取得失敗[PRINT]KINKOTANTO")
                        Return False
                    End If
                    strKinkoTel = CASTCommon.GetFSKJIni("PRINT", "KINKOTEL")            '金庫電話番号(INIファイルから取得)
                    If strKinkoTel <> strErr Then
                        OutputCsvData(strKinkoTel)
                    Else
                        BatchLog.Write("レコード作成", "失敗", "INIファイル情報取得失敗[PRINT]KINKOTEL")
                        Return False
                    End If
                    strKinkoFax = CASTCommon.GetFSKJIni("PRINT", "KINKOFAX")            '金庫FAX番号(INIファイルから取得)
                    If strKinkoFax <> strErr Then
                        OutputCsvData(strKinkoFax)
                    Else
                        BatchLog.Write("レコード作成", "失敗", "INIファイル情報取得失敗[PRINT]KINKOFAX")
                        Return False
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T")))     '委託者名漢字
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_S")))       '振替日
                    lngSeikyuKen = oraReader.GetInt64("SYORI_KEN_S") + oraReader.GetInt64("ERR_KEN_S")
                    OutputCsvData(lngSeikyuKen.ToString)                                '請求件数
                    lngSeikyuKin = oraReader.GetInt64("SYORI_KIN_S") + oraReader.GetInt64("ERR_KIN_S")
                    OutputCsvData(lngSeikyuKin.ToString)                                '請求金額
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_KEN_S")))        '振替件数
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_KIN_S")))        '振替金額
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FUNOU_KEN_S")))       '不能件数
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FUNOU_KIN_S")), False, True)  '不能金額

                    oraReader.NextRead()
                End While
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = -1
                Return False
            End If
            Return True
        Catch ex As Exception
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function

End Class
