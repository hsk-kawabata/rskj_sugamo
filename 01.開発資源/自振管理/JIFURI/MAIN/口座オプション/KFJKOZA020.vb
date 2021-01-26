Imports System.Text
Imports System.IO
Imports CASTCommon

Public Class KFJKOZA020

    '共通イベントコントロール
    Private CAST As New CASTCommon.Events

    Private MyOwnerForm As Form

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private Structure strcIni
        Dim JIKINKO_CODE As String            '自金庫コード
        Dim CHECK_FILE As String
        Dim DAT As String
    End Structure
    Private ini_info As strcIni

    Private Structure strcTori
        Dim TorisCode As String
        Dim TorifCode As String
        Dim FuriCode As String
        Dim KigyoCode As String
        Dim ItakuCode As String
        Dim itakuNname As String
        Dim Syubetu As String
    End Structure
    Private ToriInfo As strcTori


    Private MainLOG As New CASTCommon.BatchLOG("KFJKOZA020", "口座チェック（随時）")
    Private Const msgTitle As String = "口座チェック（随時）(KFJKOZA020)"

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

    Private Sub KFJKOZA020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            '###################
            ' ini取得
            '###################
            If read_ini() = False Then Return

            '###################
            ' 画面初期化
            '###################
            If set_gamen() = False Then Return

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    Private Function read_ini() As Boolean

        Try

            ini_info.JIKINKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If ini_info.JIKINKO_CODE = "err" OrElse ini_info.JIKINKO_CODE = "" Then
                MessageBox.Show(String.Format(MSG0001E, "自金庫コード", "COMMON", "KINKOCD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            ini_info.CHECK_FILE = CASTCommon.GetFSKJIni("COMMON", "KOUZACHECKFILE")
            If ini_info.CHECK_FILE = "err" OrElse ini_info.CHECK_FILE = "" Then
                MessageBox.Show(String.Format(MSG0001E, "チェック対象ファイル名", "COMMON", "KOUZACHECKFILE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            ini_info.DAT = CASTCommon.GetFSKJIni("COMMON", "DAT")
            If ini_info.DAT = "err" OrElse ini_info.DAT = "" Then
                MessageBox.Show(String.Format(MSG0001E, "DATフォルダ", "COMMON", "DAT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

        Catch ex As Exception
            Throw
        End Try

        Return True
    End Function

    Private Function set_gamen() As Boolean

        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        'Dim msg As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        Try

            Select Case GCom.SetComboBox(cmbSyubetu, "KFJKOZA020_種別.TXT", True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "種別", "KFJKOZA020_種別.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "種別"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
            End Select

            Select Case GCom.SetComboBox(cmbHenkan, "KFJKOZA020_返却区分.TXT", True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "返却区分", "KFJKOZA020_返却区分.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "返却区分"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
            End Select

        Catch ex As Exception
            Throw
        End Try

        Return True

    End Function


    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim MainDB As New CASTCommon.MyOracle
        Dim clsKCheck As New clsKouzaCheck
        Dim clsPrnt As New KFJP042

        Dim iRet As Integer

        Dim iCount As Integer = 0

        Dim KDBinfo(4) As String    '名義人カナ、新支店コード、新口座番号、エラーコード、エラーメッセージ

        Dim bRet As Boolean = True

        Dim encod As Encoding = Nothing
        Dim SyoriLen As Integer = 120

        Dim LineData(SyoriLen - 1) As Byte
        Dim TotalLen As Integer

        Dim pos As Integer = 0

        Dim fs As FileStream = Nothing
        Dim br As BinaryReader = Nothing
        Dim fsW As FileStream = Nothing
        Dim bw As BinaryWriter = Nothing
        Dim CheckFileName As String = Path.Combine(ini_info.DAT, ini_info.CHECK_FILE)
        Dim WriteFileName As String = Path.Combine(ini_info.DAT, Path.GetFileNameWithoutExtension(ini_info.CHECK_FILE) & "_AFTER" & Path.GetExtension(ini_info.CHECK_FILE))

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            btnAction.Enabled = False
            btnClose.Enabled = False

            If MessageBox.Show(MSG0015I.Replace("{0}", "口座チェック"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then Return

            '*******************************
            ' フォーマットチェックと情報収集
            '*******************************

            If readCheckHead(MainDB, CheckFileName, encod) = False Then Return

            fsW = New FileStream(WriteFileName, FileMode.Create, FileAccess.Write)
            bw = New BinaryWriter(fsW, encod)

            '*************************
            ' ファイル読込
            '*************************
            fs = New FileStream(CheckFileName, FileMode.Open, FileAccess.Read)
            br = New BinaryReader(fs)

            clsPrnt.CreateCsvFile()
            Dim strCSVFileName As String = clsPrnt.FileName
            Dim PrintFlg As Boolean = False

            TotalLen = fs.Length

            Do
                Dim KeiyakuKin As String = ""
                Dim KeiyakuSit As String = ""
                Dim KeiyakuKamoku As String = ""
                Dim keiyakuKouza As String = ""
                Dim KeiyakuKname As String = ""
                Dim JyuyoukaNo As String = ""
                iRet = 0

                LineData = Nothing

                br.BaseStream.Seek(pos, SeekOrigin.Begin)

                '1レコード読込
                LineData = br.ReadBytes(SyoriLen)

                '**********************************************
                ' チェック開始
                '**********************************************
                KDBinfo = New String() {"", "", "", "", ""} '名義人、移管支店、移管口座番号、エラーコード、エラーメッセージ

                'ログ情報
                LW.ToriCode = String.Concat(ToriInfo.TorisCode, ToriInfo.TorifCode)

                Select Case encod.GetString(LineData, 0, 1)

                    Case "2"

                        Select Case ToriInfo.Syubetu
                            Case "93"

                                KeiyakuKin = encod.GetString(LineData, 1, 4)
                                KeiyakuSit = encod.GetString(LineData, 5, 3)
                                KeiyakuKamoku = encod.GetString(LineData, 8, 1)
                                keiyakuKouza = encod.GetString(LineData, 9, 7)
                                KeiyakuKname = encod.GetString(LineData, 16, 30)
                                JyuyoukaNo = encod.GetString(LineData, 46, 20)

                            Case Else

                                KeiyakuKin = encod.GetString(LineData, 1, 4)
                                KeiyakuSit = encod.GetString(LineData, 20, 3)
                                KeiyakuKamoku = encod.GetString(LineData, 42, 1)
                                keiyakuKouza = encod.GetString(LineData, 43, 7)
                                KeiyakuKname = encod.GetString(LineData, 50, 30)
                                JyuyoukaNo = encod.GetString(LineData, 91, 20)

                        End Select

                        '科目変換
                        Select Case KeiyakuKamoku
                            Case "1"
                                KeiyakuKamoku = "02"
                            Case "2"
                                KeiyakuKamoku = "01"
                            Case "3"
                                KeiyakuKamoku = "05"
                            Case "9"
                                KeiyakuKamoku = "02"
                            Case Else
                                MessageBox.Show(MSG0262W.Replace("{0}", KeiyakuKamoku), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Return
                        End Select

                        'このifに入らないなら他行
                        iRet = 21
                        KDBinfo(3) = "1"
                        KDBinfo(4) = "他行異常"

                        If ini_info.JIKINKO_CODE = KeiyakuKin Then

                            iRet = clsKCheck.KouzaChk(KeiyakuSit, _
                                                   KeiyakuKamoku, _
                                                   keiyakuKouza, _
                                                   ToriInfo.FuriCode, _
                                                   ToriInfo.KigyoCode, _
                                                   ToriInfo.Syubetu, KeiyakuKname, _
                                                   KDBinfo, MainDB, chkYomikae.Checked)

                        End If

                        '結果を受けて口座チェック確認表出力データ作成
                        '0:異常なし 1:口座なし 2:自振契約なし  3:名義人名(カナ)相違  4:自振契約無-名義人名(カナ)相違  5:解約済 -1:異常
                        '(移管後) 10:移管済 12:自振契約なし 13:名義人名(カナ)相違 14:自振契約無-名義人名(カナ)相違 15:解約済

                        If iRet <> 0 Then

                            clsPrnt.OutputCsvData("", True)                         '振替日
                            clsPrnt.OutputCsvData(KeiyakuSit, True)                 '支店コード
                            clsPrnt.OutputCsvData(GCom.GetBKBRName(ini_info.JIKINKO_CODE, KeiyakuSit), True) '支店名
                            clsPrnt.OutputCsvData(ToriInfo.TorisCode, True)         '取引先主コード
                            clsPrnt.OutputCsvData(ToriInfo.TorifCode, True)         '取引先副コード
                            clsPrnt.OutputCsvData(ToriInfo.itakuNname, True)        '取引先名（漢字）
                            clsPrnt.OutputCsvData(ToriInfo.Syubetu, True)           '種別
                            clsPrnt.OutputCsvData(ToriInfo.FuriCode, True)          '振替コード
                            clsPrnt.OutputCsvData(ToriInfo.KigyoCode, True)         '企業コード
                            clsPrnt.OutputCsvData(KeiyakuKamoku, True)              '科目
                            clsPrnt.OutputCsvData(keiyakuKouza, True)               '口座番号
                            clsPrnt.OutputCsvData(JyuyoukaNo, True)                 '需要家番号
                            clsPrnt.OutputCsvData(KeiyakuKname.Trim, True)          '契約者名カナ
                            clsPrnt.OutputCsvData(KDBinfo(0), True)                 'KDB名義人
                            clsPrnt.OutputCsvData(KDBinfo(1), True)                 '新支店コード
                            clsPrnt.OutputCsvData(KDBinfo(2), True)                 '新口座番号
                            clsPrnt.OutputCsvData(KDBinfo(4), True)                 'エラー理由
                            clsPrnt.OutputCsvData("", True, True)                   '備考

                            PrintFlg = True

                            '口座移管の場合値を設定
                            Select Case iRet
                                Case 10, 12, 13, 14, 15
                                    Select Case GCom.GetComboBox(cmbSyubetu)
                                        Case "93"
                                            '新引落支店番号
                                            iCount = 0
                                            For Each byte1 As Byte In encod.GetBytes(KDBinfo(1))
                                                LineData(66 + iCount) = byte1
                                                iCount += 1
                                            Next
                                            '新引落支店名
                                            iCount = 0
                                            '2014/05/01 saitou 標準修正 MODIFY ----------------------------------------------->>>>
                                            '支店名漢字→支店名カナに修正
                                            For Each byte1 As Byte In encod.GetBytes(GCom.GetBKBRKName(ini_info.JIKINKO_CODE, KDBinfo(1)).PadRight(15, " "c).Substring(0, 15))
                                                LineData(69 + iCount) = byte1
                                                iCount += 1
                                            Next
                                            'For Each byte1 As Byte In encod.GetBytes(GCom.GetBKBRName(ini_info.JIKINKO_CODE, KDBinfo(1)).PadRight(15, " "c).Substring(0, 15))
                                            '    LineData(69 + iCount) = byte1
                                            '    iCount += 1
                                            'Next
                                            '2014/05/01 saitou 標準修正 MODIFY -----------------------------------------------<<<<
                                            '新預金種目
                                            LineData(84) = LineData(8)

                                            '新口座番号
                                            iCount = 0
                                            For Each byte1 As Byte In encod.GetBytes(KDBinfo(2))
                                                LineData(85 + iCount) = byte1
                                                iCount += 1
                                            Next
                                        Case Else
                                            '引落支店
                                            iCount = 0
                                            For Each byte1 As Byte In encod.GetBytes(KDBinfo(1))
                                                LineData(20 + iCount) = byte1
                                                iCount += 1
                                            Next
                                            '引落支店名名
                                            iCount = 0
                                            For Each byte1 As Byte In encod.GetBytes(GCom.GetBKBRKName(ini_info.JIKINKO_CODE, KDBinfo(1)).PadRight(15, " "c).Substring(0, 15))
                                                LineData(23 + iCount) = byte1
                                                iCount += 1
                                            Next
                                            '預金種目
                                            '口座番号
                                            iCount = 0
                                            For Each byte1 As Byte In encod.GetBytes(KDBinfo(2))
                                                LineData(43 + iCount) = byte1
                                                iCount += 1
                                            Next
                                    End Select
                            End Select

                            Select Case GCom.GetComboBox(cmbSyubetu)
                                Case "93"
                                    '何もしない
                                Case Else
                                    LineData(111) = encod.GetBytes(KDBinfo(3))(0)   '結果コード
                            End Select

                        End If
                End Select

                '結果書込
                Select Case GCom.GetComboBox(cmbHenkan)
                    Case 0
                        '結果不要
                    Case 1
                        '全件
                        bw.Write(LineData)
                    Case 2
                        Select Case encod.GetString(LineData, 0, 1)
                            Case "2"
                                '不能分のみ
                                If iRet <> 0 Then bw.Write(LineData)
                            Case Else
                                bw.Write(LineData)
                        End Select
                End Select

                pos += SyoriLen

            Loop Until TotalLen <= pos

            clsPrnt.CloseCsv()

            If Not fs Is Nothing Then fs.Close() : fs = Nothing
            If Not br Is Nothing Then br.Close() : br = Nothing
            If Not bw Is Nothing Then bw.Close() : bw = Nothing
            If Not fsW Is Nothing Then fsW.Close() : fsW = Nothing

            '口座チェック確認表の印刷
            If bRet = True AndAlso PrintFlg = True Then
                ''印刷バッチ呼び出し
                Dim ExeRepo As New CAstReports.ClsExecute
                Dim param As String
                ExeRepo.SetOwner = Me
                Dim errMsg As String = ""

                'パラメータ設定：ログイン名、ＣＳＶファイル名、取引先コード
                param = GCom.GetUserID & "," & strCSVFileName
                iRet = ExeRepo.ExecReport("KFJP042.EXE", param)

                If iRet <> 0 Then

                    '印刷失敗：戻り値に対応したエラーメッセージを表示する
                    Select Case iRet
                        Case -1
                            MessageBox.Show(String.Format(MSG0226W, "口座チェック確認表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座チェック確認表印刷", "失敗", "エラーコード：" & iRet)
                        Case Else
                            MessageBox.Show(String.Format(MSG0004E, "口座チェック確認表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座チェック確認表印刷", "失敗", "エラーコード：" & iRet)
                    End Select

                    bRet = False
                End If
            End If

            If bRet = True Then
                MessageBox.Show(MSG0016I.Replace("{0}", "口座チェック"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainDB.Commit()
            Else
                MessageBox.Show(MSG0236W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
            End If

        Catch ex As Exception

            If Not MainDB Is Nothing Then MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)例外エラー", "失敗", ex.Message)

        Finally

            btnAction.Enabled = True
            btnClose.Enabled = True
            If Not fs Is Nothing Then fs.Close() : fs = Nothing
            If Not br Is Nothing Then br.Close() : br = Nothing
            If Not bw Is Nothing Then bw.Close() : bw = Nothing
            If Not fsW Is Nothing Then fsW.Close() : fsW = Nothing
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try

    End Sub

    Private Function readCheckHead(ByVal MainDB As CASTCommon.MyOracle, ByVal FileName As String, ByRef Enc As Encoding) As Boolean

        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        'Dim MSG As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Dim SyoriLen As Integer = 120
        Dim fs As FileStream = Nothing
        Dim br As BinaryReader = Nothing
        Dim FirstData(SyoriLen - 1) As Byte
        Dim LineData(SyoriLen - 1) As Byte
        Dim ReadLen As Integer
        Dim TotalLen As Integer
        Dim pos As Integer = 0
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Try

            If File.Exists(FileName) = False Then
                MessageBox.Show(MSG0255W.Replace("{0}", "口座チェック対象"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If


            fs = New FileStream(FileName, FileMode.Open, FileAccess.Read)
            br = New BinaryReader(fs)

            TotalLen = fs.Length()

            ReadLen = fs.Read(FirstData, 0, SyoriLen)

            'レコード長が120で割り切れない。
            If TotalLen Mod SyoriLen <> 0 Then
                MessageBox.Show(MSG0257W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            If Encoding.GetEncoding("IBM290").GetString(FirstData, 0, 1) = "1" Then
                ' EBCDIC
                Enc = Encoding.GetEncoding("IBM290")
            ElseIf Encoding.GetEncoding("SHIFT-JIS").GetString(FirstData, 0, 1) = "1" Then
                ' SJIS
                Enc = Encoding.GetEncoding("SHIFT-JIS")
            Else
                MessageBox.Show(MSG0257W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            ' 委託者コード取得
            Dim ItakuCode As String = Enc.GetString(FirstData, 4, 10)
            ' 種別の取得
            Dim Syubetu As String = Enc.GetString(FirstData, 1, 2)

            If GCom.GetComboBox(cmbSyubetu) <> Syubetu Then
                MessageBox.Show(MSG0256W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            Dim SQL As New StringBuilder

            SQL.Append("SELECT TORIS_CODE_T")
            SQL.Append("     , TORIF_CODE_T")
            SQL.Append("     , KIGYO_CODE_T")
            SQL.Append("     , FURI_CODE_T")
            SQL.Append("     , ITAKU_CODE_T")
            SQL.Append("     , ITAKU_NNAME_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE ITAKU_CODE_T = '" & ItakuCode & "'")

            If OraReader.DataReader(SQL) = True Then

                Dim cnt As Integer = 0

                Do Until OraReader.EOF

                    If cnt = 1 Then
                        If MessageBox.Show(String.Format(MSG0068I, ToriInfo.FuriCode, ToriInfo.KigyoCode), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                            Return False
                        Else
                            Exit Do
                        End If

                    End If

                    ToriInfo.TorisCode = OraReader.GetString("TORIS_CODE_T")
                    ToriInfo.TorifCode = OraReader.GetString("TORIF_CODE_T")
                    ToriInfo.KigyoCode = OraReader.GetString("KIGYO_CODE_T")
                    ToriInfo.FuriCode = OraReader.GetString("FURI_CODE_T")
                    ToriInfo.ItakuCode = OraReader.GetString("ITAKU_CODE_T")
                    ToriInfo.itakuNname = OraReader.GetString("ITAKU_NNAME_T")
                    ToriInfo.Syubetu = Syubetu                                  'データの種別をセット
                    cnt += 1

                    OraReader.NextRead()
                Loop
            Else

                MessageBox.Show(MSG0258W.Replace("{0}", "取引先マスタ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '***************************************
            ' 並び順チェック
            '***************************************
            Dim DataKbn As String = "0"

            Do

                LineData = Nothing

                br.BaseStream.Seek(pos, SeekOrigin.Begin)

                '1レコード読込
                LineData = br.ReadBytes(SyoriLen)

                Select Case DataKbn
                    Case "0"     '開始
                        If Enc.GetString(LineData, 0, 1).Equals("1") = False Then
                            MessageBox.Show(MSG0259W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return False
                        End If
                    Case "1"    'ヘッダ
                        If Enc.GetString(LineData, 0, 1).Equals("2") = False And Enc.GetString(LineData, 0, 1).Equals("8") = False Then
                            MessageBox.Show(MSG0259W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return False
                        End If
                    Case "2"    'データ
                        If Enc.GetString(LineData, 0, 1).Equals("2") = False And Enc.GetString(LineData, 0, 1).Equals("8") = False Then
                            MessageBox.Show(MSG0259W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return False
                        End If
                    Case "8"    'トレーラ
                        If Enc.GetString(LineData, 0, 1).Equals("9") = False Then
                            MessageBox.Show(MSG0259W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return False
                        End If
                    Case "9"    'エンド
                        '何もしない
                    Case Else
                        If Enc.GetString(LineData, 0, 1).Equals("9") = False Then
                            MessageBox.Show(MSG0260W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return False
                        End If
                End Select

                DataKbn = Enc.GetString(LineData, 0, 1)

                pos += SyoriLen

            Loop Until TotalLen <= pos

            Return True

        Catch ex As Exception
            Throw
        Finally
            If Not br Is Nothing Then br.Close() : br = Nothing
            If Not fs Is Nothing Then fs.Close() : fs = Nothing
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

    End Function

End Class
