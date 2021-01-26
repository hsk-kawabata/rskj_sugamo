Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP039
    Inherits CAstReports.ClsReportBase

    Sub New()
        '---------------------------------------------------------
        ' CSVファイルセット
        '---------------------------------------------------------
        InfoReport.ReportName = "KFGP039"

        '---------------------------------------------------------
        ' 定義体名セット
        '---------------------------------------------------------
        If HimokuPtn = "1" Then
            ReportBaseName = "KFGP039_生徒登録情報一覧(費目15).rpd"
        Else
            ReportBaseName = "KFGP039_生徒登録情報一覧.rpd"
        End If
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile
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

        Try

            oraReader = New CASTCommon.MyOracleReader(oraDB)
            SQL.Append("SELECT")
            SQL.Append("     SEITOMAST.SEIBETU_O")
            SQL.Append("   , SEITOMAST.GAKUNEN_CODE_O")
            SQL.Append("   , SEITOMAST.FURIKAE_O")
            SQL.Append("   , SEITOMAST.KAIYAKU_FLG_O")
            SQL.Append("   , SEITOMAST.KAMOKU_O")
            SQL.Append("   , SEITOMAST.GAKKOU_CODE_O")
            SQL.Append("   , SEITOMAST.NENDO_O")
            SQL.Append("   , SEITOMAST.TUUBAN_O")
            SQL.Append("   , SEITOMAST.SEITO_NO_O")
            SQL.Append("   , SEITOMAST.TUKI_NO_O")
            SQL.Append("   , GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append("   , SEITOMAST.MEIGI_KNAME_O")
            SQL.Append("   , SEITOMAST.TKIN_NO_O")
            SQL.Append("   , SEITOMAST.TSIT_NO_O")
            SQL.Append("   , TENMAST.KIN_NNAME_N")
            SQL.Append("   , TENMAST.SIT_NNAME_N")
            SQL.Append("   , SEITOMAST.CLASS_CODE_O")
            SQL.Append("   , SEITOMAST.KEIYAKU_DENWA_O")
            SQL.Append("   , SEITOMAST.KOUZA_O")
            SQL.Append("   , SEITOMAST.KEIYAKU_NJYU_O")
            SQL.Append("   , SEITOMAST.SEITO_KNAME_O")
            SQL.Append("   , SEITOMAST.SEITO_NNAME_O")
            SQL.Append("   , SEITOMAST.HIMOKU_ID_O")
            SQL.Append("   , GAKMAST1.GAKUNEN_NAME_G")
            For No As Integer = 1 To 15
                SQL.Append("   , SEITOMAST.SEIKYU" & No.ToString("00") & "_O")
                SQL.Append("   , SEITOMAST.KINGAKU" & No.ToString("00") & "_O")
                SQL.Append("   , HIMOMAST.HIMOKU_NAME" & No.ToString("00") & "_H")
                SQL.Append("   , HIMOMAST.HIMOKU_KINGAKU" & No.ToString("00") & "_H")
            Next
            SQL.Append(",GAKMAST2.FURI_CODE_T")
            SQL.Append(",GAKMAST2.KIGYO_CODE_T")
            SQL.Append(" FROM ")
            SQL.Append("     KZFMAST.SEITOMAST")
            SQL.Append("   , KZFMAST.GAKMAST1")
            SQL.Append("   , KZFMAST.TENMAST")
            SQL.Append("   , KZFMAST.HIMOMAST")
            SQL.Append("   , KZFMAST.GAKMAST2")
            SQL.Append(" WHERE ")
            SQL.Append("     SEITOMAST.GAKKOU_CODE_O  = GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(" AND SEITOMAST.GAKUNEN_CODE_O = GAKMAST1.GAKUNEN_CODE_G")
            SQL.Append(" AND SEITOMAST.TKIN_NO_O      = TENMAST.KIN_NO_N(+)")
            SQL.Append(" AND SEITOMAST.TSIT_NO_O      = TENMAST.SIT_NO_N(+)")
            SQL.Append(" AND SEITOMAST.GAKKOU_CODE_O  = HIMOMAST.GAKKOU_CODE_H")
            SQL.Append(" AND SEITOMAST.GAKUNEN_CODE_O = HIMOMAST.GAKUNEN_CODE_H")
            SQL.Append(" AND SEITOMAST.TUKI_NO_O      = HIMOMAST.TUKI_NO_H")
            SQL.Append(" AND SEITOMAST.HIMOKU_ID_O    = HIMOMAST.HIMOKU_ID_H")
            SQL.Append(" AND GAKMAST1.GAKKOU_CODE_G   = GAKMAST2.GAKKOU_CODE_T")

            '---------------------------------------------------------
            ' 学校コードALL9指定：全学校印刷
            '---------------------------------------------------------
            If GakkouCode <> "9999999999" Then
                SQL.Append(" AND SEITOMAST.GAKKOU_CODE_O  =" & SQ(GakkouCode))
            End If

            '---------------------------------------------------------
            ' 学年未入力時　　　：全学年印刷
            '---------------------------------------------------------
            If GakunenCode <> "" Then
                SQL.Append(" AND SEITOMAST.GAKUNEN_CODE_O =" & SQ(GakunenCode))
            End If

            '---------------------------------------------------------
            ' クラス未入力時　　：全クラス印刷
            '---------------------------------------------------------
            If ClassCode <> "" Then
                '2018/02/21 saitou RSV2標準 MODIFY クラス指定時に出力されない修正 -------------------- START
                SQL.Append(" AND SEITOMAST.CLASS_CODE_O   =" & SQ(ClassCode))
                'SQL.Append(" AND SEITOMAST.CLASS_CODE_O   =" & SQ(GakunenCode))
                '2018/02/21 saitou RSV2標準 MODIFY --------------------------------------------------- END
            End If

            '---------------------------------------------------------
            ' < 印刷順 >
            '   ORDER BY句は、INIファイルに記述可能
            '     RSKJ.ini [ "PRINT" , "KFGP039_SORT" ]
            '---------------------------------------------------------
            SQL.Append(" ORDER BY ")
            Select Case PrintSort.ToUpper
                Case "ERR", ""
                    SQL.Append("   GAKKOU_CODE_O")
                    SQL.Append(" , GAKUNEN_CODE_O")
                    SQL.Append(" , CLASS_CODE_O")
                    SQL.Append(" , SEITO_NO_O")
                    SQL.Append(" , NENDO_O")
                    SQL.Append(" , TUUBAN_O")
                    SQL.Append(" , DECODE(TUKI_NO_O,'04','01'")
                    SQL.Append("                   ,'05','02'")
                    SQL.Append("                   ,'06','03'")
                    SQL.Append("                   ,'07','04'")
                    SQL.Append("                   ,'08','05'")
                    SQL.Append("                   ,'09','06'")
                    SQL.Append("                   ,'10','07'")
                    SQL.Append("                   ,'11','08'")
                    SQL.Append("                   ,'12','09'")
                    SQL.Append("                   ,'01','10'")
                    SQL.Append("                   ,'02','11'")
                    SQL.Append("                   ,'03','12')")
                Case Else
                    SQL.Append(PrintSort)
            End Select

            If oraReader.DataReader(SQL) = True Then

                While oraReader.EOF = False

                    '---------------------------------------------------------
                    ' ヘッダ出力
                    '---------------------------------------------------------
                    OutputCsvData(mMatchingDate)                                                   '処理日
                    OutputCsvData(mMatchingTime)                                                   'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_O")))                '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))               '学校名

                    '---------------------------------------------------------
                    ' 明細出力（生徒情報）
                    '---------------------------------------------------------
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENDO_O")))                      '入学年度
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_O")))                     '通番
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_O")))               '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_O")))                 'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_O")))                   '生徒番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True)          '生徒名(カナ)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")), True)          '生徒名(漢字)
                    Select Case oraReader.GetString("SEIBETU_O")                                   '性別
                        Case "0"
                            OutputCsvData("男")
                        Case "1"
                            OutputCsvData("女")
                        Case "2"
                            OutputCsvData("－")
                        Case Else
                            OutputCsvData("")
                    End Select

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_O")))                    '取扱金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIN_NNAME_N")))                  '取扱金融機関名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_O")))                    '支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")))                  '取扱支店名
                    Select Case oraReader.GetString("KAMOKU_O")                                    '科目
                        Case "02"
                            OutputCsvData("普通")
                        Case "01"
                            OutputCsvData("当座")
                        Case "09"
                            OutputCsvData("その他")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_O")))                      '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("MEIGI_KNAME_O")), True)          '名義人名(カナ)
                    Select Case oraReader.GetString("FURIKAE_O")                                   '振替方法
                        Case "0"
                            OutputCsvData("口座振替")
                        Case "1"
                            OutputCsvData("集金扱い")
                        Case "2"
                            OutputCsvData("振替停止")
                        Case Else
                            OutputCsvData("")
                    End Select
                    Select Case oraReader.GetString("KAIYAKU_FLG_O")                               '解約区分
                        Case "0"
                            OutputCsvData("通常")
                        Case "9"
                            OutputCsvData("解約")
                        Case Else
                            OutputCsvData("")
                    End Select

                    '---------------------------------------------------------
                    ' 明細出力（費目情報）
                    '  <費目印字指定> 0または未指定:費目情報印字なし
                    '                 0以外        :費目情報印字あり
                    '---------------------------------------------------------
                    Select Case PrintHimoku.ToUpper                                                '費目印字指定
                        Case "ERR", "", "0"
                            OutputCsvData("0")
                        Case Else
                            OutputCsvData(PrintHimoku.Trim)
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_ID_O")))                  '費目ID 
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUKI_NO_O")))                    '対象月
                    For No As Integer = 1 To 15                                                    '費目名1～15
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H")))
                    Next
                    For No As Integer = 1 To 15                                                    '請求方法1～15
                        Select Case oraReader.GetString("SEIKYU" & No.ToString("00") & "_O")
                            Case "1"
                                OutputCsvData("個")
                            Case Else
                                OutputCsvData("")
                        End Select
                    Next
                    For No As Integer = 1 To 15                                                    '請求金額1～15
                        Select Case oraReader.GetInt64("SEIKYU" & No.ToString("00") & "_O")
                            Case 0
                                OutputCsvData(GCOM.NzStr(oraReader.GetInt64("HIMOKU_KINGAKU" & No.ToString("00") & "_H")))
                            Case Else
                                OutputCsvData(GCOM.NzStr(oraReader.GetInt64("KINGAKU" & No.ToString("00") & "_O")))
                        End Select
                    Next
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")))                  '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")))                 '企業コード
                    OutputCsvData("", False, True)                                                 '改行ダミー

                    oraReader.NextRead()
                    RecordCnt += 1
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
