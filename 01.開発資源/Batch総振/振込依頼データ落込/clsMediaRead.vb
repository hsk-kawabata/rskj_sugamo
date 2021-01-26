Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports CAstFormat
Imports System.Collections
Imports System.String
Imports Microsoft.VisualBasic
Imports System.Diagnostics
Imports System.Windows.Forms

Public Class clsMediaRead

    Function fn_FD_CPYTO_DISK(ByVal astrTORI_CODE As String, ByRef astrIN_FILE_NAME As String, ByVal astrOUT_FILE_NAME As String, _
                              ByVal aintREC_LENGTH As Integer, ByVal astrCODE_KBN As String, ByVal astrP_FILE As String, ByVal msgTitle As String) As Integer
        '=====================================================================================
        'NAME           :fn_FD_CPYTO_DISK
        'Parameter      :astrTORI_CODE：取引先コード／astrIN_FILE_NAME：入力ファイル名／
        '               :astrOUT_FILE_NAME：出力ファイル／aintREC_LENGTH：レコード長／
        '               :astrCODE_KBN：コード区分／astrP_FILE：FTRAN+定義ファイル　／ msgTitle:メッセージタイトル
        'Description    :伝送ファイルをコピーする
        'Return         :0=成功、100=ファイル読み込み失敗、200=コード区分異常（JIS改行あり）、
        '               :300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
        'Create         :2004/07/21
        'Update         :
        '=====================================================================================
        fn_FD_CPYTO_DISK = 100

        Dim strDIR As String
        strDIR = CurDir()

        Dim gstrFTR_OPENDIR As String = GetFSKJIni("COMMON", "FTR")
        Dim gstrFTRAN2000_OPENDIR As String = GetFSKJIni("COMMON", "FTRAN2000")
        Dim lngEXITCODE As Long


        '-----------------------------------------------------------
        'コード変換処理
        'コード変換区分が 2 の場合はIBM形式 EBCDIC コードとみなし
        'JIS8コードへのコード変換を行う
        '-----------------------------------------------------------

        Select Case astrCODE_KBN

            Case "4"            'EBCDIC コード区分変更 2009/09/30 kakiwaki

                If MessageBox.Show(String.Format(MSG0073I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                    fn_FD_CPYTO_DISK = 999  '挿入キャンセル
                    Exit Function
                End If
                '
                If Dir(astrOUT_FILE_NAME) <> "" Then
                    Kill(astrOUT_FILE_NAME)
                End If
                Dim strCMD As String
                Dim strTEIGI_FIEL As String

                ChDir(gstrFTRAN2000_OPENDIR)
                strTEIGI_FIEL = gstrFTR_OPENDIR & astrP_FILE

                'ＩＢＭフロッピーディスクがセットされているか判定
FD_EXIST_IBM:
                ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                'strCMD = "FT /wc/ ILIST A:"
                strCMD = "FT /nwd/ ILIST A:"
                ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                Dim ProcFT As New Process
                Dim ProcInfo As New ProcessStartInfo(gstrFTRAN2000_OPENDIR & "FT", strCMD.Substring(3))
                ProcInfo.CreateNoWindow = True
                ProcInfo.WorkingDirectory = gstrFTRAN2000_OPENDIR
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
                lngEXITCODE = ProcFT.ExitCode

                If lngEXITCODE = 0 Then
                Else
                    '2009/12/03 キャンセル時ループ終了==========
                    'MessageBox.Show(String.Format(MSG0073I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                    'GoTo FD_EXIST_IBM
                    If MessageBox.Show(String.Format(MSG0073I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), _
                                       msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                        fn_FD_CPYTO_DISK = 999         'ユーザキャンセル
                        Exit Function
                    Else
                        GoTo FD_EXIST_IBM
                    End If
                    '=====================
                End If

                ChDir(gstrFTRAN2000_OPENDIR)

                ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                'strCMD = "FT /WC/ GETRAND " & "A:" & astrIN_FILE_NAME & Space(1) & astrOUT_FILE_NAME & " ++" & strTEIGI_FIEL
                strCMD = "FT /NWD/ GETRAND " & """A:" & astrIN_FILE_NAME & """" & Space(1) & """" & astrOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                ProcFT = New Process
                ProcInfo = New ProcessStartInfo(gstrFTRAN2000_OPENDIR & "FT", strCMD.Substring(3))
                ProcInfo.CreateNoWindow = True
                ProcInfo.WorkingDirectory = gstrFTRAN2000_OPENDIR
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
                lngEXITCODE = ProcFT.ExitCode

                If lngEXITCODE = 0 Then
                    fn_FD_CPYTO_DISK = 0
                Else
                    ChDir(strDIR)
                    fn_FD_CPYTO_DISK = 100         'コード変換失敗
                    Exit Function
                End If

            Case Else        'JIS,JIS改

                If MessageBox.Show(String.Format(MSG0064I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), _
                                   msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                    fn_FD_CPYTO_DISK = 999  '挿入キャンセル
                    Exit Function
                End If

                Dim strFOLDER As String
                Dim strFILE As String
                If Dir(astrOUT_FILE_NAME) <> "" Then
                    Kill(astrOUT_FILE_NAME)
                End If
                strFOLDER = "A:\"
                strFILE = strFOLDER & astrIN_FILE_NAME.Trim

                If astrIN_FILE_NAME = "" Then
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'MessageBox.Show(MSG0346W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MessageBox.Show(MSG0346W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                    fn_FD_CPYTO_DISK = 100
                    Exit Function
                End If

FD_EXIST:
                Try
                    Dir(strFOLDER)
                    '-----------------------------
                    'ＤＯＳ形式か確認
                    '-----------------------------
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'FileOpen(1, Dir(strFOLDER), OpenMode.Binary)
                    'FileClose(1)
                    If Directory.GetFiles(strFOLDER).Length = 0 Then
                        Throw New Exception
                    End If
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                Catch EX As Exception      'FDがセットされていなかった場合
                    If MessageBox.Show(String.Format(MSG0064I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) _
                       = DialogResult.OK Then
                        GoTo FD_EXIST
                    Else
                        ChDir(strDIR)
                        fn_FD_CPYTO_DISK = 200         'ファイル読み込み失敗
                        Exit Function
                    End If
                End Try
                If Dir(strFILE) = "" Then 'FD内にファイルが存在しなかったら
                    Dim OPENFILEDIALOG1 As New OpenFileDialog
                    OPENFILEDIALOG1.InitialDirectory = strFOLDER
                    OPENFILEDIALOG1.Multiselect = False
                    OPENFILEDIALOG1.CheckFileExists = True
                    OPENFILEDIALOG1.FileName = strFILE
                    OPENFILEDIALOG1.AddExtension = True
                    OPENFILEDIALOG1.CheckFileExists = True
                    Dim dlgRESULT As DialogResult
                    dlgRESULT = OPENFILEDIALOG1.ShowDialog()
                    If dlgRESULT = DialogResult.Cancel Then    'キャンセルボタンが押されたら
                        ChDir(strDIR)
                        fn_FD_CPYTO_DISK = 200         'ファイル読み込み失敗
                        Exit Function
                    End If
                    strFILE = OPENFILEDIALOG1.FileName
                    astrIN_FILE_NAME = ""
                    Dim j As Integer
                    For j = 1 To strFILE.Length
                        If strFILE.Chars(strFILE.Length - j) = "\" Then
                            Exit For
                        End If
                        astrIN_FILE_NAME = strFILE.Chars(strFILE.Length - j) & astrIN_FILE_NAME
                    Next
                End If

                '2012/01/13 saitou 標準修正 ADD ---------------------------------------->>>>
                '依頼ファイルをコピーする前に、FD内の依頼ファイルの読み取り専用属性を解除する
                If File.Exists(strFILE) = True Then
                    Dim fiCopyFile As New System.IO.FileInfo(strFILE)
                    If (fiCopyFile.Attributes And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                        fiCopyFile.Attributes = FileAttributes.Normal
                    End If
                End If
                '2012/01/13 saitou 標準修正 ADD ----------------------------------------<<<<

                '単純にコピー
                File.Copy(strFILE, astrOUT_FILE_NAME, True)
                Return 0

        End Select
        ChDir(strDIR)
        fn_FD_CPYTO_DISK = 0

    End Function

    '****20120618 mubuchi DVD対応 STASRT******
    Function fn_DVD_CPYTO_DISK(ByVal astrTORI_CODE As String, ByRef astrIN_FILE_NAME As String, ByVal astrOUT_FILE_NAME As String, _
                              ByVal aintREC_LENGTH As Integer, ByVal astrCODE_KBN As String, ByVal astrP_FILE As String, ByVal msgTitle As String, ByVal readfmt As CAstFormat.CFormat, _
                              ByVal astrBAITAI_CODE As String) As Integer
        '=====================================================================================
        'NAME           :fn_DVD_CPYTO_DISK
        'Parameter      :astrTORI_CODE：取引先コード／astrIN_FILE_NAME：入力ファイル名／
        '               :astrOUT_FILE_NAME：出力ファイル／aintREC_LENGTH：レコード長／
        '               :astrCODE_KBN：コード区分／astrP_FILE：FTRAN+定義ファイル　／ msgTitle:メッセージタイトル
        'Description    :
        'Return         :0=成功、100=ファイル読み込み失敗、200=コード区分異常（JIS改行あり）、
        '               :300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
        'Create         :2004/07/21
        'Update         :2013/12/24 saitou 標準版 外部媒体対応
        '=====================================================================================
        fn_DVD_CPYTO_DISK = 100

        Dim strDIR As String
        strDIR = CurDir()

        Dim gstrFTR_OPENDIR As String = GetFSKJIni("COMMON", "FTR")
        Dim gstrFTRAN2000_OPENDIR As String = GetFSKJIni("COMMON", "FTRAN2000")
        '2013/12/24 saitou 標準版 外部媒体対応 DEL -------------------------------------------------->>>>
        'DVDは媒体1として扱うことにする
        'Dim DVD_DRIVE As String = GetFSKJIni("DVD", "DRIVE")
        '2013/12/24 saitou 標準版 外部媒体対応 DEL --------------------------------------------------<<<<
        Dim work_path As String = GetFSKJIni("COMMON", "DATBK")
        Dim sWorkFilename As String 'ワークファイル名
        'ワークファイルのパスを決める
        sWorkFilename = work_path & astrIN_FILE_NAME.Trim

        '2013/12/24 saitou 標準版 外部媒体対応 ADD -------------------------------------------------->>>>
        ' 2016/01/27 タスク）綾部 CHG 【PG】UI_B-14-99(RSV2対応(USB)) -------------------- START
        'Dim BAITAI_1 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_1")
        'Dim BAITAI_2 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_2")
        'Dim BAITAI_3 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_3")
        'Dim BAITAI_4 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_4")
        'Dim BAITAI_5 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_5")
        'Dim strUseBaitai As String = String.Empty       '使う媒体
        'Dim strUseBaitaiName As String = String.Empty   '使う媒体名
        'Select Case astrBAITAI_CODE
        '    Case "11"   'DVD
        '        strUseBaitai = BAITAI_1
        '        strUseBaitaiName = "DVD-RAM"
        '    Case "12"   'USB
        '        strUseBaitai = BAITAI_2
        '        strUseBaitaiName = ""
        '    Case "13"
        '        strUseBaitai = BAITAI_3
        '        strUseBaitaiName = ""
        '    Case "14"
        '        strUseBaitai = BAITAI_4
        '        strUseBaitaiName = ""
        '    Case "15"
        '        strUseBaitai = BAITAI_5
        '        strUseBaitaiName = ""
        'End Select
        Dim strUseBaitai As String = String.Empty       '使う媒体
        Dim strUseBaitaiName As String = String.Empty   '使う媒体名
        Select Case astrBAITAI_CODE
            Case "11"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_1")
            Case "12"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_2")
            Case "13"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_3")
            Case "14"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_4")
            Case "15"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_5")
        End Select
        strUseBaitaiName = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_媒体コード.TXT"), astrBAITAI_CODE)
        ' 2016/01/27 タスク）綾部 CHG 【PG】UI_B-14-99(RSV2対応(USB)) -------------------- END
        '2013/12/24 saitou 標準版 外部媒体対応 ADD --------------------------------------------------<<<<

        '-----------------------------------------------------------
        'コード変換処理
        'コード変換区分が 2 の場合はIBM形式 EBCDIC コードとみなし
        'JIS8コードへのコード変換を行う
        '-----------------------------------------------------------

        Select Case astrCODE_KBN

            Case "4"            'EBCDIC コード区分変更 2009/09/30 kakiwaki
                '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                '媒体名を設定できるように変更
                If MessageBox.Show(String.Format(MSG0075I, strUseBaitaiName, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), _
                                   msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                    fn_DVD_CPYTO_DISK = 999 '挿入キャンセル
                    Exit Function
                End If
                'If MessageBox.Show(String.Format(MSG0075I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                '    fn_DVD_CPYTO_DISK = 999  '挿入キャンセル
                '    Exit Function
                'End If
                '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<

                Dim strFOLDER As String
                Dim strFILE As String


                If Dir(astrOUT_FILE_NAME) <> "" Then
                    Kill(astrOUT_FILE_NAME)
                End If
                'ＤＶＤドライブはiniファイルより取得する
                '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                strFOLDER = strUseBaitai
                'strFOLDER = DVD_DRIVE
                '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<
                strFILE = strFOLDER & astrIN_FILE_NAME.Trim

                If astrIN_FILE_NAME = "" Then
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'MessageBox.Show(MSG0346W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MessageBox.Show(MSG0346W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                    fn_DVD_CPYTO_DISK = 100
                    Exit Function
                End If

                '2017/04/21 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ START
                'EBCDICの場合でもファイル選択できるように処理追加
DVD_EBC_EXIST:
                Try
                    '-----------------------------
                    '文字コードがJISか確認
                    '-----------------------------
                    If Directory.GetFiles(strFOLDER).Length = 0 Then
                        Throw New Exception
                    End If

                Catch EX As Exception      'ＤＶＤがセットされていなかった場合
                    '媒体名を設定できるように変更
                    If MessageBox.Show(String.Format(MSG0076I, strUseBaitaiName, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), _
                                       msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.OK Then
                        GoTo DVD_EBC_EXIST
                    Else
                        ChDir(strDIR)
                        fn_DVD_CPYTO_DISK = 200         'ファイル読み込み失敗
                        Exit Function
                    End If
                End Try
                If Dir(strFILE) = "" Then 'DVD内にファイルが存在しなかったら
                    Dim OPENFILEDIALOG1 As New OpenFileDialog
                    OPENFILEDIALOG1.InitialDirectory = strFOLDER
                    OPENFILEDIALOG1.Multiselect = False
                    OPENFILEDIALOG1.CheckFileExists = True
                    OPENFILEDIALOG1.FileName = strFILE
                    OPENFILEDIALOG1.AddExtension = True
                    OPENFILEDIALOG1.CheckFileExists = True
                    Dim dlgRESULT As DialogResult
                    dlgRESULT = OPENFILEDIALOG1.ShowDialog()
                    If dlgRESULT = DialogResult.Cancel Then    'キャンセルボタンが押されたら
                        ChDir(strDIR)
                        fn_DVD_CPYTO_DISK = 999         'ユーザキャンセル
                        Exit Function
                    End If
                    strFILE = OPENFILEDIALOG1.FileName
                    astrIN_FILE_NAME = Path.GetFileName(strFILE)
                End If
                '2017/04/21 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ END

                '2012/01/13 saitou 標準修正 ADD ---------------------------------------->>>>
                '依頼ファイルをコピーする前に、DVD内の依頼ファイルの読み取り専用属性を解除する
                If File.Exists(strFILE) = True Then
                    Dim fiCopyFile As New System.IO.FileInfo(strFILE)
                    If (fiCopyFile.Attributes And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                        fiCopyFile.Attributes = FileAttributes.Normal
                    End If
                End If
                '2012/01/13 saitou 標準修正 ADD ----------------------------------------<<<<

                'ローカルへコピーする
                File.Copy(strFILE, sWorkFilename, True)

                ' FTRANを使用して変換する
                If ConvertFtranPlus(sWorkFilename, astrOUT_FILE_NAME, readfmt.FTRANP) <> 0 Then
                    Return 100
                Else
                    Return 0
                End If

            Case Else        'JIS,JIS改

                '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                '媒体名を設定できるように変更
                If MessageBox.Show(String.Format(MSG0075I, strUseBaitaiName, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), _
                                   msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                    fn_DVD_CPYTO_DISK = 999 '挿入キャンセル
                    Exit Function
                End If
                'If MessageBox.Show(String.Format(MSG0076I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), _
                '                                msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                '    fn_DVD_CPYTO_DISK = 999  '挿入キャンセル
                '    Exit Function
                'End If
                '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<

                Dim strFOLDER As String
                Dim strFILE As String
                If Dir(astrOUT_FILE_NAME) <> "" Then
                    Kill(astrOUT_FILE_NAME)
                End If
                'ＤＶＤドライブはiniファイルより取得する
                '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                strFOLDER = strUseBaitai
                'strFOLDER = DVD_DRIVE
                '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<
                strFILE = strFOLDER & astrIN_FILE_NAME.Trim

                If astrIN_FILE_NAME = "" Then
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'MessageBox.Show(MSG0346W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MessageBox.Show(MSG0346W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                    fn_DVD_CPYTO_DISK = 100
                    Exit Function
                End If

DVD_EXIST:
                Try
                    Dir(strFOLDER)
                    '-----------------------------
                    '文字コードがJISか確認
                    '-----------------------------
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'FileOpen(1, Dir(strFOLDER), OpenMode.Binary)
                    'FileClose(1)
                    If Directory.GetFiles(strFOLDER).Length = 0 Then
                        Throw New Exception
                    End If
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                Catch EX As Exception      'ＤＶＤがセットされていなかった場合
                    '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                    '媒体名を設定できるように変更
                    If MessageBox.Show(String.Format(MSG0076I, strUseBaitaiName, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), _
                                       msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.OK Then
                        GoTo DVD_EXIST
                    Else
                        ChDir(strDIR)
                        fn_DVD_CPYTO_DISK = 200         'ファイル読み込み失敗
                        Exit Function
                    End If

                    'If MessageBox.Show(String.Format(MSG0077I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) _
                    '                      = DialogResult.OK Then
                    '    GoTo DVD_EXIST
                    'Else
                    '    ChDir(strDIR)
                    '    fn_DVD_CPYTO_DISK = 200         'ファイル読み込み失敗
                    '    Exit Function
                    'End If
                    '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<
                End Try
                If Dir(strFILE) = "" Then 'DVD内にファイルが存在しなかったら
                    Dim OPENFILEDIALOG1 As New OpenFileDialog
                    OPENFILEDIALOG1.InitialDirectory = strFOLDER
                    OPENFILEDIALOG1.Multiselect = False
                    OPENFILEDIALOG1.CheckFileExists = True
                    OPENFILEDIALOG1.FileName = strFILE
                    OPENFILEDIALOG1.AddExtension = True
                    OPENFILEDIALOG1.CheckFileExists = True
                    Dim dlgRESULT As DialogResult
                    dlgRESULT = OPENFILEDIALOG1.ShowDialog()
                    If dlgRESULT = DialogResult.Cancel Then    'キャンセルボタンが押されたら
                        ChDir(strDIR)
                        fn_DVD_CPYTO_DISK = 200         'ファイル読み込み失敗
                        Exit Function
                    End If
                    strFILE = OPENFILEDIALOG1.FileName
                    astrIN_FILE_NAME = ""
                    Dim j As Integer
                    For j = 1 To strFILE.Length
                        If strFILE.Chars(strFILE.Length - j) = "\" Then
                            Exit For
                        End If
                        astrIN_FILE_NAME = strFILE.Chars(strFILE.Length - j) & astrIN_FILE_NAME
                    Next
                End If

                '2012/01/13 saitou 標準修正 ADD ---------------------------------------->>>>
                '依頼ファイルをコピーする前に、DVD内の依頼ファイルの読み取り専用属性を解除する
                If File.Exists(strFILE) = True Then
                    Dim fiCopyFile As New System.IO.FileInfo(strFILE)
                    If (fiCopyFile.Attributes And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                        fiCopyFile.Attributes = FileAttributes.Normal
                    End If
                End If
                '2012/01/13 saitou 標準修正 ADD ----------------------------------------<<<<

                'コード区分チェック用にファイルをコピー
                File.Copy(strFILE, sWorkFilename, True)
                '単純にコピー
                File.Copy(strFILE, astrOUT_FILE_NAME, True)
                Return 0

        End Select
        ChDir(strDIR)
        fn_DVD_CPYTO_DISK = 0

    End Function
    '****20120618 mubuchi DVD対応 END*********
    '
    ' 機能　 ： FTRAN PLUS
    '
    ' 引数   ： ARG1 - 入力ファイル名
    '           ARG2 - 出力ファイル名
    '           ARG3 - 変換定義ファイル
    '
    ' 戻り値 ： 
    '
    ' 備考　 ： 
    '
    Private Function ConvertFtranPlus(ByVal infile As String, ByVal outfile As String, ByVal teigi As String) As Integer
        Dim nRet As Integer

        ' EBCDIC 変換
        Dim strCMD As String
        Dim strTEIGI_FIEL As String

        Dim sFtranPPath As String = CASTCommon.GetFSKJIni("COMMON", "FTR")

        strTEIGI_FIEL = sFtranPPath & teigi

        'FTRAN+のコマンドを使ってコード変換する
        ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
        'strCMD = "/nwd/ cload " & sFtranPPath & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & infile & " " & outfile & " ++" & strTEIGI_FIEL
        strCMD = "/nwd/ cload " & sFtranPPath & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & """" & infile & """" & " " & """" & outfile & """" & " ++" & """" & strTEIGI_FIEL & """"
        ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

        Dim ProcFT As Process
        Dim ProcInfo As New ProcessStartInfo
        ProcInfo.FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "FTRANP"), "FP.EXE")
        ProcInfo.Arguments = strCMD
        ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
        ProcFT = Process.Start(ProcInfo)
        ProcFT.WaitForExit()
        nRet = ProcFT.ExitCode

        If nRet <> 0 Then
            ' コード変換失敗
            Return 100
        End If
        'End If

        Return 0
    End Function

End Class
