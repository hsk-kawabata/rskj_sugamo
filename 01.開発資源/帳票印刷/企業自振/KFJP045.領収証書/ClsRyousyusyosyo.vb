Imports System.Text



Public Class ClsRyousyusyosyo

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

    Private Function PrnRyousyusyosyo(ByVal TorisCode As String, ByVal TorifCode As String, ByVal FuriDate As String, ByVal PrinterName As String) As Boolean

        Dim ret As Boolean = False

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

     
        Try
            'INIファイルより設定 ==========
            '印刷区分追加
            Dim PrnKbn As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYUSYO")
            If PrnKbn.ToUpper = "ERR" OrElse PrnKbn = "" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:領収証書印刷区分 分類:KOKUZEI 項目:RYOUSYUSYO")
                Return False
            End If

            '料金後納欄
            Dim Gonouran As String = CASTCommon.GetFSKJIni("KOKUZEI", "GONOURAN")
            If Gonouran.ToUpper = "ERR" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:料金後納欄 分類:KOKUZEI 項目:GONOURAN")
                Return False
            End If

            '領収欄１
            Dim Ryousyu1 As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYU1")
            If Ryousyu1.ToUpper = "ERR" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:領収欄１ 分類:KOKUZEI 項目:RYOUSYU1")
                Return False
            End If

            '領収欄２
            Dim Ryousyu2 As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYU2")
            If Ryousyu2.ToUpper = "ERR" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:領収欄２ 分類:PRINT 項目:RYOUSYU2")
                Return False
            End If

            '領収欄３
            Dim Ryousyu3 As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYU3")
            If Ryousyu3.ToUpper = "ERR" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:領収欄３ 分類:KOKUZEI 項目:RYOUSYU3")
                Return False
            End If

            '領収欄４
            Dim Ryousyu4 As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYU4")
            If Ryousyu4.ToUpper = "ERR" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:領収欄４ 分類:KOKUZEI 項目:RYOUSYU4")
                Return False
            End If
            '==============================

            Dim ClsPrnt As New ClsPrnRyousyusyosyo(PrnKbn)

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim SQL As New StringBuilder(128)

            SQL.Append(" SELECT ")
            SQL.Append(" DATA_KBN_K ")
            SQL.Append(",FURI_DATA_K ")
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

            Dim tuuban As Integer = 0
            Dim zei_code As String = ""

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
                        '振替済みのみ印字
                        If OraReader.GetItem("FURIKETU_CODE_K").Trim <> "0" Then
                            Exit Select
                        Else
                            ZumiCnt += 1
                            If tuuban <> 0 Then
                                '通番は、税務署ごとにカウント。日銀コードが変わった場合、通番を1にする。
                                If zei_code <> KFmt.KOKUZEI_REC2.KZ6.Trim Then
                                    ZumiCnt = "1"
                                End If
                            End If
                            zei_code = KFmt.KOKUZEI_REC2.KZ6.Trim               '日銀コードを退避
                            tuuban += 1

                        End If

                        Select Case BK_DATA_KBN
                            Case "2", "3", "4"
                            Case Else
                                MainLOG.Write("明細情報の取得に失敗しました", "失敗", errdetail)

                                OraReader.Close()
                                OraReader = Nothing

                                Exit Try
                        End Select

                        KFmt.KOKUZEI_REC3.Data = OraReader.GetItem("FURI_DATA_K")

                        ClsPrnt.OutputCsvData(Gonouran)  '料金後納欄

                        '***郵便番号***
                        Dim 郵便番号 As String = ""
                        If KFmt.KOKUZEI_REC3.KZ15.Trim.Length < 4 Then
                            郵便番号 = KFmt.KOKUZEI_REC3.KZ15.Trim
                        Else
                            郵便番号 = KFmt.KOKUZEI_REC3.KZ15.PadRight(7).Substring(0, 3) & "-"
                            郵便番号 &= KFmt.KOKUZEI_REC3.KZ15.PadRight(7).Substring(3)
                        End If
                        ClsPrnt.OutputCsvData(郵便番号)

                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ23.Trim)  '都市区名
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ24.Trim)  '住所１
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ25.Trim)  '住所２
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ26.Trim)  '住所３
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ27.Trim)  '肩書１
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ28.Trim)  '肩書２
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ29.Trim)  '肩書３


                        '*** 2010/3/5 FJH 納税者名２が空だったら納税者１を２に。
                        If KFmt.KOKUZEI_REC3.KZ31.Trim.Equals("") Then
                            ClsPrnt.OutputCsvData("")                           '納税者名１
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ30.Trim)  '納税者名２
                        Else
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ30.Trim)  '納税者名１
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ31.Trim)  '納税者名２
                        End If

                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ30.Trim)  '納税者名１
                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ31.Trim)  '納税者名２

                        '***差出人(金融機関名称１、２)***
                        Dim 差出人 As String = KFmt.KOKUZEI_REC2.KZ27.Trim & Space(1) & KFmt.KOKUZEI_REC2.KZ28.Trim
                        ClsPrnt.OutputCsvData(差出人)  '差出人


                        Dim 差出人住所 As String = Getsitjyusyo(KFmt.KOKUZEI_REC2.KZ10.Substring(0, 4).Trim, _
                                                             KFmt.KOKUZEI_REC2.KZ10.Substring(4).Trim)
                        If 差出人住所 = "err" Then
                            MainLOG.Write("支店住所の取得に失敗しました", "失敗", "金融機関コード：" & _
                                     KFmt.KOKUZEI_REC2.KZ10.Substring(0, 4).Trim & "-" & KFmt.KOKUZEI_REC2.KZ10.Substring(4))
                            '取り合えず進める
                            差出人住所 = KFmt.KOKUZEI_REC2.KZ24.Trim & KFmt.KOKUZEI_REC2.KZ25.Trim
                        End If

                        ClsPrnt.OutputCsvData(差出人住所)  '差出人住所

                        '***差出人電話番号(TEL (００００)００－００００)***
                        Dim 差出人電話番号 As String = ""
                        Dim 差出人TELNo As String = GetSitTELNo(KFmt.KOKUZEI_REC2.KZ10.Substring(0, 4).Trim, _
                                                             KFmt.KOKUZEI_REC2.KZ10.Substring(4).Trim)
                        If 差出人TELNo = "err" Then
                            MainLOG.Write("支店電話番号の取得に失敗しました", "失敗", "金融機関コード：" & _
                                      KFmt.KOKUZEI_REC2.KZ10.Substring(0, 4).Trim & "-" & KFmt.KOKUZEI_REC2.KZ10.Substring(4))
                            '取り合えず進める
                            差出人電話番号 = "TEL (" & StrConv(KFmt.KOKUZEI_REC2.KZ21.Substring(0, 4), VbStrConv.Wide) & ")"
                            差出人電話番号 &= StrConv(KFmt.KOKUZEI_REC2.KZ21.Substring(4, 2), VbStrConv.Wide) & "－"
                            差出人電話番号 &= StrConv(KFmt.KOKUZEI_REC2.KZ21.Substring(6).Trim, VbStrConv.Wide)
                        Else
                            差出人電話番号 = "TEL (" & StrConv(差出人TELNo.Substring(0, 4), VbStrConv.Wide) & ")"
                            差出人電話番号 = 差出人電話番号 & StrConv(差出人TELNo.Substring(5).Trim, VbStrConv.Wide)
                        End If

                        ClsPrnt.OutputCsvData(差出人電話番号)  '差出人電話番号


                        '***差出人支店***
                        Dim 差出人支店 As String = GetSitName(KFmt.KOKUZEI_REC2.KZ10.Substring(0, 4).Trim, _
                                                         KFmt.KOKUZEI_REC2.KZ10.Substring(4).Trim)
                        If 差出人支店 = "err" Then
                            MainLOG.Write("支店名の取得に失敗しました", "失敗", "金融機関コード：" & _
                                      KFmt.KOKUZEI_REC2.KZ10.Substring(0, 4).Trim & "-" & KFmt.KOKUZEI_REC2.KZ10.Substring(4))
                            '取り合えず進める
                            差出人支店 = ""
                        End If

                        '*** 2010/3/5 FJH  FSKJのレイアウトに合わせる …取扱店は不要
                        'ClsPrnt.OutputCsvData("取扱い店  " & 差出人支店)  '差出人支店
                        ClsPrnt.OutputCsvData(差出人支店)  '差出人支店
                        '*** 2010/3/5 FJH  FSKJのレイアウトに合わせる …取扱店は不要

                        ClsPrnt.OutputCsvData(ZumiCnt.ToString)  '通番
                        ClsPrnt.OutputCsvData(ZumiCnt.ToString)  '番号
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ4)  '税目コード
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ7)  '納期区分

                        '***納期区分***
                        Select Case KFmt.KOKUZEI_REC1.KZ7.Trim
                            Case "1"
                                ClsPrnt.OutputCsvData("1")  '表示納期区分
                                ClsPrnt.OutputCsvData("")  '納期カナ文字
                            Case "2"
                                ClsPrnt.OutputCsvData("2")  '表示納期区分
                                ClsPrnt.OutputCsvData("")  '納期カナ文字
                            Case "3", "4"
                                ClsPrnt.OutputCsvData("3")  '表示納期区分

                                '"4"＆"8"の時だけ記述する
                                If KFmt.KOKUZEI_REC1.KZ7.Trim = "4" Then
                                    ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ8.Trim)  '納期カナ文字
                                Else
                                    ClsPrnt.OutputCsvData("")  '納期カナ文字
                                End If
                            Case "8"
                                ClsPrnt.OutputCsvData("  ")  '表示納期区分
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ8.Trim)  '納期カナ文字
                            Case Else
                                ClsPrnt.OutputCsvData("")  '表示納期区分
                                ClsPrnt.OutputCsvData("")  '納期カナ文字
                        End Select

                        '***納税期間***
                        Select Case KFmt.KOKUZEI_REC1.KZ4 '税目コード
                            Case "020"
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ6.Trim.Substring(0, 2))  '納税期間(自)年
                                ClsPrnt.OutputCsvData("")  '納税期間(自)月
                                ClsPrnt.OutputCsvData("")  '納税期間(自)日
                                ClsPrnt.OutputCsvData("")  '納税期間(至)年
                                ClsPrnt.OutputCsvData("")  '納税期間(至)月
                                ClsPrnt.OutputCsvData("")  '納税期間(至)日
                            Case "300"
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ12.Trim.Substring(0, 2))  '納税期間(自)年
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ12.Trim.Substring(2, 2))  '納税期間(自)月
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ12.Trim.Substring(4, 2))  '納税期間(自)日
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ13.Trim.Substring(0, 2))  '納税期間(至)年
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ13.Trim.Substring(2, 2))  '納税期間(至)月
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ13.Trim.Substring(4, 2))  '納税期間(至)日
                            Case Else
                                MainLOG.Write("明細情報の取得に失敗しました", "失敗", errdetail)

                                OraReader.Close()
                                OraReader = Nothing

                                Exit Try
                        End Select

                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ5.Trim)  '年度
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ7.Trim)  '税務署名
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ6.Trim)  '日銀コード
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ9.Trim)  '納付税額
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ10.Trim)  '内利子税
                        ClsPrnt.OutputCsvData(FuriDate)  '振替日
                        ClsPrnt.OutputCsvData(Ryousyu1)  '領収欄１
                        ClsPrnt.OutputCsvData(Ryousyu2)  '領収欄２
                        ClsPrnt.OutputCsvData(Ryousyu3)  '領収欄３
                        ClsPrnt.OutputCsvData(Ryousyu4)  '領収欄４


                        '***2010/3/5 FJH 右側の住所

                        If KFmt.KOKUZEI_REC3.KZ24.Trim.Equals("") Then
                            ClsPrnt.OutputCsvData("")  '都市区名_B
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ23.Trim)  '都市区名_B → 住所1_Bに
                        Else
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ23.Trim)  '都市区名_B
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ24.Trim)  '住所１_B
                        End If

                        If KFmt.KOKUZEI_REC3.KZ29.Trim.Equals("") Then
                            ClsPrnt.OutputCsvData("")  '住所２_B
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ25.Trim)  '住所２_B → 肩書３_B
                        Else
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ25.Trim)  '住所２_B
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ29.Trim)  '肩書３_B
                        End If
                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ23.Trim)  '都市区名_B
                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ24.Trim)  '住所１_B
                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ25.Trim)  '住所２_B
                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ29.Trim)  '肩書３_B

                        ClsPrnt.OutputCsvData(tuuban, False, True)   '総合通番

                    Case "4" '署別金融機関店舗別トータルレコード（Ｄ）
                        KFmt.KOKUZEI_REC2.Data = OraReader.GetItem("FURI_DATA_K")
                    Case "5" '署別金融機関別トータルレコード（Ｅ）
                        KFmt.KOKUZEI_REC2.Data = OraReader.GetItem("FURI_DATA_K")
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

            'CSVを加工するため明示的にCSVを閉じる
            Call ClsPrnt.CloseCsv()
              Select PrnKbn
                Case "1"    '2つ折
                    Dim ErrMessage As String = ""
                    Dim CSVName As String = ClsPrnt.FileName
                    '引数で受け取ったCSVファイル名から拡張子(TMP)でA4両面用のCSVファイルを作成する。
                    If Not MakeCsvFile(CSVName, ErrMessage) Then
                        MainLOG.Write("領収証書", "失敗", ErrMessage)
                        Return False
                    Else
                        '拡張子TMPで作成したCSVファイルで元ファイルを上書き
                        Dim TMPFile As String = IO.Path.Combine(IO.Path.GetPathRoot(CSVName), _
                                                                   IO.Path.GetFileNameWithoutExtension(CSVName) & ".TMP")
                        IO.File.Copy(TMPFile, CSVName, True)
                        'TMPファイルの削除
                        IO.File.Delete(TMPFile)
                    End If
                Case "2"    '3つ折　
            End Select

            ' 帳票出力
            If ClsPrnt.ReportExecute(PrinterName) = True Then
                MainLOG.Write("領収証書", "成功")
            Else
                MainLOG.Write("領収証書", "失敗", ClsPrnt.ReportMessage)
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

    Private Function Getsitjyusyo(ByVal KinCode As String, ByVal SitCode As String) As String

        Dim ret As String = "err"

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim sql As New StringBuilder(128)

            sql.Append(" SELECT ")
            sql.Append(" KJYU_N ")
            sql.Append(" FROM SITEN_INFOMAST ")
            sql.Append(" WHERE KIN_NO_N = '" & KinCode & "'")
            sql.Append(" AND SIT_NO_N = '" & SitCode & "'")

            If OraReader.DataReader(sql) Then
                ret = OraReader.GetItem("KJYU_N")
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
    Private Function GetSitTELNo(ByVal KinCode As String, ByVal SitCode As String) As String

        Dim ret As String = "err"

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim sql As New StringBuilder(128)

            sql.Append(" SELECT ")
            sql.Append(" DENWA_N ")
            sql.Append(" FROM SITEN_INFOMAST ")
            sql.Append(" WHERE KIN_NO_N = '" & KinCode & "'")
            sql.Append(" AND SIT_NO_N = '" & SitCode & "'")

            If OraReader.DataReader(sql) Then
                ret = OraReader.GetItem("DENWA_N")
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
    '***************************************************************
    '機能:ＣＳＶ作成（納付書・領収証書（国税））
    '***************************************************************
    '引数:      
    '   CsvFilePath        ＣＳＶファイルパス
    '戻り値:
    '    TRUE       正常終了
    '    FALSE      異常終了
    '備考:   
    '***************************************************************
    Private Function MakeCsvFile(ByVal CsvFileName As String, ByRef ErrMessage As String) As Boolean

        Dim ret As Boolean = False

        Dim sr As IO.StreamReader = Nothing
        Dim sw As IO.StreamWriter = Nothing

        Try
            If Not IO.File.Exists(CsvFileName) Then
                ErrMessage = "CSVファイルが見つかりませんでした。:" & CsvFileName
                Exit Try
            End If

            'S-JIS
            sr = New IO.StreamReader(CsvFileName, Text.Encoding.GetEncoding(932))

            Dim CsvFileNameTmp As String = IO.Path.Combine(IO.Path.GetPathRoot(CsvFileName), _
                                                           IO.Path.GetFileNameWithoutExtension(CsvFileName) & ".TMP")
            'S-JIS
            sw = New IO.StreamWriter(CsvFileNameTmp, False, Text.Encoding.GetEncoding(932))

            Dim ret_read As Boolean = False
            Dim WriteLine As New Text.StringBuilder(1024)

            '1行目は読みとばす
            Call sr.ReadLine()

            '2行ずつ読込
            Do
                Dim ReadLine1 As String = Nothing
                Dim ReadLine2 As String = Nothing

                'A
                ReadLine1 = sr.ReadLine
                'B
                ReadLine2 = sr.ReadLine

                '1行目読込
                If ReadLine1 Is Nothing Then

                    '無ければ抜ける
                    Exit Do
                Else
                    '表面の上
                    WriteLine.Append(ReadLine1 & ",0,0" & vbCrLf)

                    '2行目読込
                    If ReadLine2 Is Nothing Then
                        '2行目が無ければ裏面の上
                        WriteLine.Append(ReadLine1 & ",1,1" & vbCrLf)

                        ReadLine1 = Nothing
                        ReadLine2 = Nothing

                        '読込フラグを立てて抜ける
                        ret_read = True

                        Exit Do
                    Else
                        '2行目が有る
                        '表面の下
                        WriteLine.Append(ReadLine2 & ",0,0" & vbCrLf)
                        '裏面の上
                        WriteLine.Append(ReadLine1 & ",1,1" & vbCrLf)
                        '裏面の下
                        WriteLine.Append(ReadLine2 & ",1,1" & vbCrLf)
                    End If
                End If

                ReadLine1 = Nothing
                ReadLine2 = Nothing

                '読込フラグを立てる
                ret_read = True
            Loop

            '正常に読めていれば書込
            If ret_read = True Then
                sw.Write(WriteLine)
            End If

            sr.Close()
            sr.Dispose()
            sr = Nothing
            sw.Close()
            sw.Dispose()
            sw = Nothing

            CsvFileName = CsvFileNameTmp

            ret = ret_read

        Catch ex As Exception
            ErrMessage = "CSVファイル読込に失敗しました。:" & ex.Message

            ret = False
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr.Dispose()
                sr = Nothing
            End If
            If Not sw Is Nothing Then
                sw.Close()
                sw.Dispose()
                sw = Nothing
            End If
        End Try

        Return ret

    End Function

End Class
