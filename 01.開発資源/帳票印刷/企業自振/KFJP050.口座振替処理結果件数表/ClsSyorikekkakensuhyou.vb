Imports System.Text

Public Class ClsSyorikekkakensuhyou


    Private MainDB As CASTCommon.MyOracle = Nothing

    Private errdetail As String = ""

    ''ファイル先頭レコード（Ａ）
    '<VBFixedString(1)> Public KZ1 As String         'データ区分(=1)
    '<VBFixedString(2)> Public KZ2 As String         'ファイル区分
    '<VBFixedString(19)> Public KZ3 As String        'ダミー
    '<VBFixedString(3)> Public KZ4 As String         '科目コード
    '<VBFixedString(2)> Public KZ5 As String         '年度
    '<VBFixedString(2)> Public KZ6 As String         '課税年分
    '<VBFixedString(1)> Public KZ7 As String         '納期区分
    '<VBFixedString(7)> Public KZ8 As String         '納期カナ文字
    '<VBFixedString(2)> Public KZ9 As String         '徴定区分
    '<VBFixedString(6)> Public KZ10 As String        '発送年月日
    '<VBFixedString(6)> Public KZ11 As String        '振替日
    '<VBFixedString(6)> Public KZ12 As String        '課税期間（自）
    '<VBFixedString(6)> Public KZ13 As String        '課税期間（至）
    '<VBFixedString(325)> Public KZ14 As String      'ダミー
    '<VBFixedString(2)> Public KZ15 As String        '依頼ファイルＮＯ

    ''署別金融機関店舗別名称レコード（Ｂ）
    ''署別金融機関店舗別トータルレコード（Ｄ）
    ''署別金融機関別トータルレコード（Ｅ）
    '<VBFixedString(1)> Public KZ1 As String         'データ区分(=2)
    '<VBFixedString(2)> Public KZ2 As String         'ファイル区分
    '<VBFixedString(5)> Public KZ3 As String         '局署番号
    '<VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
    '<VBFixedString(9)> Public KZ5 As String         'ダミー
    '<VBFixedString(5)> Public KZ6 As String         '日銀コード
    '<VBFixedString(10)> Public KZ7 As String        '税務署名
    '<VBFixedString(7)> Public KZ8 As String         'ダミー
    '<VBFixedString(5)> Public KZ9 As String         '税務署郵便番号
    '<VBFixedString(7)> Public KZ10 As String        '取扱金融機関番号
    '<VBFixedString(5)> Public KZ11 As String        '金融機関郵便番号
    '<VBFixedString(7)> Public KZ12 As String        'ダミー
    '<VBFixedString(6)> Public KZ13 As String        '送付分件数
    '<VBFixedString(12)> Public KZ14 As String       '送付分合計金額
    '<VBFixedString(6)> Public KZ15 As String        '振替納付不能件数
    '<VBFixedString(12)> Public KZ16 As String       '振替納付不能合計
    '<VBFixedString(6)> Public KZ17 As String        '振替納付件数
    '<VBFixedString(12)> Public KZ18 As String       '振替納付合計金額
    '<VBFixedString(5)> Public KZ19 As String        'ダミー
    '<VBFixedString(8)> Public KZ20 As String        '税務署電話番号
    '<VBFixedString(8)> Public KZ21 As String        '金融機関電話番号
    '<VBFixedString(27)> Public KZ22 As String       'ダミー
    '<VBFixedString(30)> Public KZ23 As String       '都市区名
    '<VBFixedString(30)> Public KZ24 As String       '所在地Ⅰ
    '<VBFixedString(30)> Public KZ25 As String       '所在地Ⅱ
    '<VBFixedString(30)> Public KZ26 As String       '肩書
    '<VBFixedString(30)> Public KZ27 As String       '金融機関名称Ⅰ
    '<VBFixedString(30)> Public KZ28 As String       '金融機関名称Ⅱ
    '<VBFixedString(30)> Public KZ29 As String       '店舗名称
    '<VBFixedString(1)> Public KZ30 As String        '補充記入
    '<VBFixedString(5)> Public KZ31 As String        'ダミー
    '<VBFixedString(2)> Public KZ32 As String        '依頼ファイルＮＯ

    ''個別明細レコード（Ｃ）
    '<VBFixedString(1)> Public KZ1 As String         'データ区分(=3)
    '<VBFixedString(2)> Public KZ2 As String         'ファイル区分(=91)
    '<VBFixedString(5)> Public KZ3 As String         '局署番号
    '<VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
    '<VBFixedString(7)> Public KZ5 As String         '納税者番号
    '<VBFixedString(1)> Public KZ6 As String         '継承区分
    '<VBFixedString(1)> Public KZ7 As String         '補完表示区分
    '<VBFixedString(1)> Public KZ8 As String         '振替結果コード
    '<VBFixedString(10)> Public KZ9 As String        '納付税額
    '<VBFixedString(9)> Public KZ10 As String        '内利子税
    '<VBFixedString(1)> Public KZ11 As String        '預金種別
    '<VBFixedString(7)> Public KZ12 As String        '口座番号
    '<VBFixedString(8)> Public KZ13 As String        '整理番号
    '<VBFixedString(69)> Public KZ14 As String       'ダミー
    '<VBFixedString(7)> Public KZ15 As String        '郵便番号（7桁）
    '<VBFixedString(5)> Public KZ16 As String        '郵便番号（5桁）
    '<VBFixedString(1)> Public KZ17 As String        '補完表示
    '<VBFixedString(7)> Public KZ18 As String        '取扱金融機関番号
    '<VBFixedString(7)> Public KZ19 As String        'ダミー
    '<VBFixedString(6)> Public KZ20 As String        '市外局番(納税者)
    '<VBFixedString(8)> Public KZ21 As String        '電話番号(納税者)
    '<VBFixedString(2)> Public KZ22 As String        'ダミー
    '<VBFixedString(23)> Public KZ23 As String       '都市区分
    '<VBFixedString(23)> Public KZ24 As String       '住所Ⅰ
    '<VBFixedString(23)> Public KZ25 As String       '住所Ⅱ
    '<VBFixedString(23)> Public KZ26 As String       '住所Ⅲ
    '<VBFixedString(23)> Public KZ27 As String       '肩書Ⅰ
    '<VBFixedString(23)> Public KZ28 As String       '肩書Ⅱ
    '<VBFixedString(23)> Public KZ29 As String       '肩書Ⅲ
    '<VBFixedString(23)> Public KZ30 As String       '納税者名Ⅰ
    '<VBFixedString(23)> Public KZ31 As String       '納税者名Ⅱ
    '<VBFixedString(5)> Public KZ32 As String        '納貯番号
    '<VBFixedString(3)> Public KZ33 As String        '口座番号
    '<VBFixedString(1)> Public KZ34 As String        '継続区分
    '<VBFixedString(2)> Public KZ35 As String        '依頼ファイルＮＯ

    ''ファイル合計レコード（Ｆ）
    '<VBFixedString(1)> Public KZ1 As String         'データ区分(=8)
    '<VBFixedString(2)> Public KZ2 As String         'ファイル区分(=91)
    '<VBFixedString(5)> Public KZ3 As String         '局署番号
    '<VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
    '<VBFixedString(10)> Public KZ5 As String        'ダミー
    '<VBFixedString(45)> Public KZ6 As String        'ダミー
    '<VBFixedString(6)> Public KZ7 As String         '送付分件数
    '<VBFixedString(12)> Public KZ8 As String        '送付分合計金額
    '<VBFixedString(6)> Public KZ9 As String         '振替納付不能件数
    '<VBFixedString(12)> Public KZ10 As String       '振替納付不能合計金額
    '<VBFixedString(6)> Public KZ11 As String        '振替納付件数
    '<VBFixedString(12)> Public KZ12 As String       '振替納付合計金額
    '<VBFixedString(264)> Public KZ13 As String      'ダミー
    '<VBFixedString(2)> Public KZ14 As String        '依頼ファイルＮＯ

    ''ファイルエンドレコード（Ｇ）
    '<VBFixedString(1)> Public KZ1 As String         'データ区分(=9)
    '<VBFixedString(2)> Public KZ2 As String         'ファイル区分(=91)
    '<VBFixedString(5)> Public KZ3 As String         '局署番号
    '<VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
    '<VBFixedString(10)> Public KZ5 As String        'ダミー
    '<VBFixedString(45)> Public KZ6 As String        'ダミー
    '<VBFixedString(6)> Public KZ7 As String         '送付分件数
    '<VBFixedString(12)> Public KZ8 As String        '送付分合計金額
    '<VBFixedString(6)> Public KZ9 As String         '振替納付不能件数
    '<VBFixedString(12)> Public KZ10 As String       '振替納付不能金額
    '<VBFixedString(6)> Public KZ11 As String        '振替納付件数
    '<VBFixedString(12)> Public KZ12 As String       '振替納付金額
    '<VBFixedString(264)> Public KZ13 As String      'ダミー
    '<VBFixedString(2)> Public KZ14 As String        '依頼ファイルＮＯ

    Public Function Main(ByVal CmdArgs As String) As Boolean

        Dim ret As Boolean = False

        Try

            Dim Cmd As String() = CmdArgs.Split(",")

            Select Case Cmd.Length
                Case 0 To 4
                    MainLOG.Write("Main", "失敗", "引数不足")

                    Exit Try
                Case 5 '正常
                    MainLOG.Write("Main", "成功", CmdArgs)

                Case Else
                    MainLOG.Write("Main", "失敗", "引数まちがい")

                    Exit Try
            End Select

            Dim TorisCode As String = Cmd(0)
            Dim TorifCode As String = Cmd(1)
            Dim FuriDate As String = Cmd(2)
            Dim PrinterName As String = Cmd(3)
            Dim PaperName As String = Cmd(4)

            errdetail = "取引先コード：" & TorisCode & "-" & TorifCode & " 振替日：" & FuriDate

            MainDB = New CASTCommon.MyOracle

            '帳票印刷
            If Not PrnRyousyusyosyo(TorisCode, TorifCode, FuriDate, PrinterName) Then
                Exit Try
            End If

            MainDB.Close()
            MainDB = Nothing

            ret = True

        Catch ex As Exception
            ret = False
            MainLOG.Write("想定外のエラーが発生しました", "失敗", ex.Message & "：" & ex.StackTrace)
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
        End Try

        Return ret

    End Function

    Private Function PrnRyousyusyosyo(ByVal TorisCode As String, ByVal TorifCode As String, ByVal FuriDate As String, ByVal printername As String) As Boolean

        Dim ret As Boolean = False

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            'INIファイルより設定 ==========
            '自金庫名
            Dim JikinkoName As String = CASTCommon.GetFSKJIni("KOKUZEI", "KINKONAME")
            If JikinkoName.ToUpper = "ERR" OrElse JikinkoName = "" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:自金庫名 分類:PRINT 項目:KINKONAME")
                Return -300
            End If
            '==============================
            Dim ClsPrnt As New ClsPrnSyorikekkakensuhyou

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim SQL As New StringBuilder(128)

            SQL.Append(" SELECT ")
            SQL.Append(" DATA_KBN_K ")
            SQL.Append(",FURI_DATA_K ")
            SQL.Append(",FURIKIN_K ")
            SQL.Append(",FURIKETU_CODE_K ")
            SQL.Append(" FROM MEIMAST ")
            SQL.Append(" WHERE TORIS_CODE_K = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_K = '" & TorifCode & "'")
            SQL.Append(" AND FURI_DATE_K = '" & FuriDate & "'")
            SQL.Append(" ORDER BY RECORD_NO_K")

            Dim name As String = ""

            If Not OraReader.DataReader(SQL) Then
                MainLOG.Write("明細情報の取得に失敗しました", "失敗", errdetail)

                OraReader.Close()
                OraReader = Nothing

                Exit Try
            End If

            name = ClsPrnt.CreateCsvFile()

            Dim KFmt As New CAstFormat.CFormatKokuzei

            Dim RecCnt As Long = 1
            Dim ZumiCnt As Long = 0
            Dim BK_DATA_KBN As String = ""

            '局所番号
            Dim KyokusyoBango As String = ""
            Dim KyokusyoBango_bk As String = ""

            '金額カウント用変数
            Dim TotalKen As Long = 0
            Dim TotalKin As Long = 0
            Dim FunoKen As Long = 0
            Dim FunoKin As Long = 0
            Dim FuriKen As Long = 0
            Dim FuriKin As Long = 0

            Do While OraReader.EOF = False

                Select Case OraReader.GetItem("DATA_KBN_K").Trim
                    Case "1" 'ファイル先頭レコード（Ａ）
                        '先頭レコードチェック
                        If RecCnt <> 1 Then
                            MainLOG.Write("明細情報の取得に失敗しました", "失敗", errdetail)

                            OraReader.Close()
                            OraReader = Nothing

                            Exit Try
                        End If
                        KFmt.KOKUZEI_REC1.Data = OraReader.GetItem("FURI_DATA_K")
                    Case "2"  '署別金融機関店舗別名称レコード（Ｂ）
                        KFmt.KOKUZEI_REC2.Data = OraReader.GetItem("FURI_DATA_K")
                    Case "3" '個別明細レコード（Ｃ）
                        KFmt.KOKUZEI_REC3.Data = OraReader.GetItem("FURI_DATA_K")

                        If OraReader.GetItem("FURIKETU_CODE_K") <> 0 Then
                            TotalKen += 1
                            TotalKin += OraReader.GetInt64("FURIKIN_K")
                            FunoKen += 1
                            FunoKin += OraReader.GetInt64("FURIKIN_K")
                        Else
                            TotalKen += 1
                            TotalKin += OraReader.GetInt64("FURIKIN_K")
                            FuriKen += 1
                            FuriKin += OraReader.GetInt64("FURIKIN_K")
                        End If

                    Case "4" '署別金融機関店舗別トータルレコード（Ｄ）
                        KFmt.KOKUZEI_REC2.Data = OraReader.GetItem("FURI_DATA_K")
                    Case "5" '署別金融機関別トータルレコード（Ｅ）
                        Select Case BK_DATA_KBN
                            Case "2", "3", "4"
                            Case Else
                                MainLOG.Write("明細情報の取得に失敗しました", "失敗", errdetail)

                                OraReader.Close()
                                OraReader = Nothing

                                Exit Try
                        End Select
                        KFmt.KOKUZEI_REC2.Data = OraReader.GetItem("FURI_DATA_K")

                        'データ出力
                        ClsPrnt.OutputCsvData(Now.ToString("yyyyMMdd")) '処理日
                        ClsPrnt.OutputCsvData("東京国税局総務部 事務管理第二課") '相手先名
                        ClsPrnt.OutputCsvData("電 子 計 算 第 ２ 係") '相手先担当部署
                        ClsPrnt.OutputCsvData(JikinkoName) '自金庫名
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ5.Trim)  '年度
                        '***納期区分***
                        Select Case KFmt.KOKUZEI_REC1.KZ7.Trim
                            Case "1"
                                ClsPrnt.OutputCsvData("1")  '期
                            Case "2"
                                ClsPrnt.OutputCsvData("2")  '期
                            Case "3", "4"
                                ClsPrnt.OutputCsvData("3")  '期
                            Case "8"
                                ClsPrnt.OutputCsvData("3")  '期
                        End Select
                        ClsPrnt.OutputCsvData(FuriDate) '振替日
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ7.Trim) '税務署名
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ3.Trim & "-" & KFmt.KOKUZEI_REC2.KZ6.Trim) '局所番号等
                        ClsPrnt.OutputCsvData(TotalKen.ToString) '送付分件数
                        ClsPrnt.OutputCsvData(TotalKin.ToString) '送付分合計金額
                        ClsPrnt.OutputCsvData(FunoKen) '振替納付不能件数
                        ClsPrnt.OutputCsvData(FunoKin) '振替納付不能合計金額
                        ClsPrnt.OutputCsvData(FuriKen) '振替納付件数
                        ClsPrnt.OutputCsvData(FuriKin, False, True) '振替納付合計金額

                        TotalKen = 0
                        TotalKin = 0
                        FunoKen = 0
                        FunoKin = 0
                        FuriKen = 0
                        FuriKin = 0

                    Case "8" 'ファイル合計レコード（Ｆ）
                        KFmt.KOKUZEI_REC8.Data = OraReader.GetItem("FURI_DATA_K")
                    Case "9" 'ファイルエンドレコード（Ｇ）
                        KFmt.KOKUZEI_REC9.Data = OraReader.GetItem("FURI_DATA_K")
                    Case Else
                        MainLOG.Write("明細情報の取得に失敗しました", "失敗", errdetail)

                        OraReader.Close()
                        OraReader = Nothing

                        Exit Try
                End Select

                BK_DATA_KBN = OraReader.GetItem("DATA_KBN_K").Trim

                RecCnt += 1

                OraReader.NextRead()
            Loop

            ' 帳票出力
            If ClsPrnt.ReportExecute(printername) = True Then
                MainLOG.Write("口座振替処理結果件数表", "成功")
            Else
                MainLOG.Write("口座振替処理結果件数表", "失敗", ClsPrnt.ReportMessage)
                Return False
            End If

            If Not ClsPrnt.HostCsvName Is Nothing AndAlso ClsPrnt.HostCsvName <> "" Then
                Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                DestName &= ClsPrnt.HostCsvName
                IO.File.Copy(ClsPrnt.FileName, DestName, True)
            End If

            ret = True

        Catch ex As Exception
            ret = False
            MainLOG.Write("想定外のエラーが発生しました", "失敗", ex.Message & "：" & ex.StackTrace & Space(1) & errdetail)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function

    Private Function GetSitName(ByVal KinCode As String, ByVal SitCode As String) As String

        Dim ret As String = "err"

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim sql As New StringBuilder(128)

            sql.Append(" SELECT ")
            sql.Append(" SIT_NNAME_N ")
            sql.Append(" FROM TENMAST ")
            sql.Append(" WHERE KIN_NO_N = '" & KinCode & "'")
            sql.Append(" AND SIT_NO_N = '" & SitCode & "'")

            If OraReader.DataReader(sql) Then
                ret = OraReader.GetItem("SIT_NNAME_N")
            End If

            OraReader.Close()
            OraReader = Nothing

        Catch ex As Exception
            ret = False
            MainLOG.Write("想定外のエラーが発生しました", "失敗", ex.Message & "：" & ex.StackTrace & Space(1) & errdetail)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function
End Class
