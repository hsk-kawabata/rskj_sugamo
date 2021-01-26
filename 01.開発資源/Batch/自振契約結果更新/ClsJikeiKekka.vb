Option Strict On
Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
'Imports CAstSystem
Imports CASTCommon.ModPublic
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports CASTCommon
' センタ自振契約結果更新処理
Public Class ClsJikeiKekka

    Private JobTuuban As Integer

    Public MainLOG As New CASTCommon.BatchLOG("KFJ100", "自振契約結果更新")

    ' ジョブメッセージ
    Private JobMessage As String = ""

    ' 起動パラメータ 共通情報
    'Private mArgumentData As CommData

    ' 自振契約結果ファイル名
    Private mDataFileName As String

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    Private Const msgTitle As String = "自振契約結果更新(KFJ100)"

    Structure strcIni
        Dim RIENTA_PATH As String        'リエンタファイル作成先
        Dim DAT_PATH As String           ' DATのパス
        Dim JIKEI_RIENTAFILENAME As String    'リエンタファイル 
        Dim JIKONKO_CODE As String    'リエンタファイル 
    End Structure
    Private ini_info As strcIni

    ' New
    Public Sub New()
    End Sub

    ' 機能　 ： 自振契約結果更新処理 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Function Main(ByVal CmdArg As String) As Integer

        Dim TimeStamp As String
        Dim RientaFileName As String = ""
        Dim FullRientaFileName As String = ""
        Dim TankingFileName As String = ""
        Dim bRet As Boolean = True
        Dim Param() As String = CmdArg.Split(","c)

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Try
            '*********************************
            ' 初期処理
            '*********************************
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "メイン処理(開始)", "成功")
            'BatchLog.Write("0000000000-00", "00000000", "ログイン(開始)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            MainLOG.Write("(初期処理)開始", "成功", "")

            If Param.Length = 2 Then
                TimeStamp = Param(0)
                JobTuuban = CASTCommon.CAInt32(Param(1))
            Else
                TimeStamp = Param(0)
                JobTuuban = 0
            End If

            MainLOG.JobTuuban = JobTuuban
            MainLOG.FuriDate = TimeStamp.Substring(0, 8)

            MainLOG.Write("パラメータ取得", "成功", "タイムスタンプ" & TimeStamp)

            ' オラクル
            MainDB = New CASTCommon.MyOracle

            If ini_read() = False Then
                bRet = False
            End If

            MainLOG.Write("(初期処理)終了", "成功", "")


            '*********************************
            ' 主処理
            '*********************************
            MainLOG.Write("(主処理)開始", "成功", "")


            '結果リエンタファイルの名前取得
            If bRet = True Then
                If GetRientaFileName(TimeStamp, RientaFileName) = False Then
                    bRet = False
                    MainLOG.Write("ファイルチェック", "失敗", "リエンタファイル名取得失敗")
                Else
                    MainLOG.Write("ファイルチェック", "成功", "リエンタファイル名取得")

                End If
            End If

            'フロッピーのチェック
            If bRet = True Then
                Do
                    Try

                        '2012/01/13 saitou 標準修正 MODIFY ------------------------------------->>>>
                        'リエンタ作成先をINIファイルで管理する
                        Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                        'Dim DirInfo As New DirectoryInfo("A:\")
                        '2012/01/13 saitou 標準修正 MODIFY -------------------------------------<<<<
                        Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()

                        Exit Do

                    Catch ex As Exception
                        '2012/01/13 saitou 標準修正 MODIFY ------------------------------------->>>>
                        'リエンタ作成先をINIファイルで管理する
                        If MessageBox.Show(String.Format(MSG0065I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                            bRet = False
                            JobMessage = "FD挿入がキャンセルされました。"
                            MainLOG.Write("FD要求", "失敗", "FD挿入がキャンセル")
                            Exit Do
                        End If
                        'If MessageBox.Show(String.Format(MSG0065I, Path.GetPathRoot("A:\")), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                        '    bRet = False
                        '    JobMessage = "FD挿入がキャンセルされました。"
                        '    MainLOG.Write("FD要求", "失敗", "FD挿入がキャンセル")
                        '    Exit Do
                        'End If
                        '2012/01/13 saitou 標準修正 MODIFY -------------------------------------<<<<
                    End Try
                Loop
            End If

            'ファイルの存在チェック
            If bRet = True Then

                '2012/01/13 saitou 標準修正 MODIFY ------------------------------------->>>>
                'リエンタ作成先をINIファイルで管理する
                FullRientaFileName = Path.Combine(ini_info.RIENTA_PATH, RientaFileName)
                'FullRientaFileName = Path.Combine("A:\", RientaFileName)
                '2012/01/13 saitou 標準修正 MODIFY -------------------------------------<<<<

                If File.Exists(FullRientaFileName) = False Then
                    JobMessage = "入力ファイルが見つかりません"
                    MainLOG.Write("ファイルチェック", "失敗", "入力ファイルが見つかりません")
                    bRet = False
                End If

            End If

            'タンキングデータのみのファイル作成
            If bRet = True Then
                If CreateTankingData(TimeStamp, FullRientaFileName, TankingFileName) <> 0 Then
                    MainLOG.Write("ワークファイル作成", "失敗", "")
                    bRet = False
                Else
                    MainLOG.Write("ワークファイル作成", "成功")

                End If
            End If

            ' 結果更新処理
            If bRet = True Then
                bRet = KekkaMain(TankingFileName, TimeStamp)
            End If

            If bRet = False Then
                MainDB.Rollback()
                If JobMessage = "" Then
                    MainLOG.UpdateJOBMASTbyErr("ログ参照")
                Else
                    MainLOG.UpdateJOBMASTbyErr(JobMessage)
                End If
            Else
                MainDB.Commit()
                MainLOG.UpdateJOBMASTbyOK(JobMessage)
            End If

            If bRet = False Then
                Return 2
            End If

        Catch ex As Exception
            If Not MainDB Is Nothing Then MainDB.Rollback()
            MainLOG.Write("(主処理)", "失敗", ex.Message)
            Return -1
        Finally

            If Not MainDB Is Nothing Then MainDB.Close()
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "メイン処理(終了)", "成功")
            'MainLOG.Write("(主処理)終了", "成功", "")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        End Try

        Return 0
    End Function

    Private Function ini_read() As Boolean

        ini_info.RIENTA_PATH = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")        'リエンタファイル作成先
        If ini_info.RIENTA_PATH = "err" OrElse ini_info.RIENTA_PATH = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:リエンタファイル作成フォルダ 分類:COMMON 項目:RIENTADR")
            JobMessage = "設定ファイル取得失敗 項目名:リエンタファイル作成フォルダ 分類:COMMON 項目:RIENTADR"
            Return False
        End If

        ini_info.DAT_PATH = CASTCommon.GetFSKJIni("COMMON", "DAT")           'DATのパス
        If ini_info.DAT_PATH = "err" OrElse ini_info.DAT_PATH = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT")
            JobMessage = "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT"
            Return False
        End If

        ini_info.JIKEI_RIENTAFILENAME = CASTCommon.GetFSKJIni("COMMON", "JIKEI_RIENTAFILENAME")
        If ini_info.JIKEI_RIENTAFILENAME = "err" OrElse ini_info.JIKEI_RIENTAFILENAME = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:リエンタファイル名 分類:COMMON 項目:JIKEI_RIENTAFILENAME")
            JobMessage = "設定ファイル取得失敗 項目名:リエンタファイル名 分類:COMMON 項目:JIKEI_RIENTAFILENAME"
            Return False
        End If

        ini_info.JIKONKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
        If ini_info.JIKONKO_CODE = "err" OrElse ini_info.JIKONKO_CODE = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:JIKONKO_CODE")
            JobMessage = "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:JIKONKO_CODE"
            Return False
        End If

        Return True
    End Function


    ' 機能　 ： 自振契約結果更新処理
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function KekkaMain(ByVal TankingFileName As String, ByVal TimeStamp As String) As Boolean

        Dim fr As FileStream = Nothing
        Dim br As BinaryReader = Nothing

        Dim Enco As Encoding = Encoding.GetEncoding(50220)  'JIS

        Dim RecordLen As Integer = 262

        Dim lngTuuban As Long
        Dim strOPE_CODE As String
        Dim strKOKYAKU_NO As String
        Dim strKOUZA_NO As String
        Dim strKekka As String

        Dim errMsg(4) As String '実際には４だが・・・

        Dim KFJP044 As New clsKFJP044

        Try

            fr = New FileStream(TankingFileName, FileMode.Open, FileAccess.Read)     'フロッピーのファイル
            br = New BinaryReader(fr)

            Dim byteRecordData(RecordLen - 1) As Byte

            Dim Filelen As Long = fr.Length
            Dim Pos As Long = 0
            Dim clsKFJP044 As New clsKFJP044
            Dim DataList As New List(Of clsKFJP044.UpdateInfo)
            Dim Item As clsKFJP044.UpdateInfo = Nothing
            Do
                lngTuuban = 0
                strOPE_CODE = ""
                strKOKYAKU_NO = ""
                strKOUZA_NO = ""
                strKekka = ""
                errMsg = New String() {"", "", "", "", ""}

                byteRecordData = Nothing

                br.BaseStream.Seek(Pos, SeekOrigin.Begin)

                '1レコード読込
                byteRecordData = br.ReadBytes(RecordLen)

                'ファイルの最後(最終データの次)は「FF FF」
                If byteRecordData(0).ToString("X").PadLeft(2, "0"c) = "FF" And byteRecordData(1).ToString("X").PadLeft(2, "0"c) = "FF" Then
                    Exit Do
                End If

                lngTuuban = byteRecordData(16) * 256 + byteRecordData(17)

                'オペコード取得
                strOPE_CODE = Enco.GetString(byteRecordData, 18, 5)

                '処理結果取得
                strKekka = byteRecordData(28).ToString("X").PadLeft(2, "0"c)


                '顧客番号取得
                strKOKYAKU_NO = Enco.GetString(byteRecordData, 32, 7)

                '口座番号取得

                strKOUZA_NO = Enco.GetString(byteRecordData, 39, 7)


                '結果が"23"の場合のみエラーメッセージを読み取る 2007/08/16
                If strKekka = "23" AndAlso byteRecordData(46).ToString("X").PadLeft(2, "0"c) = "0F" Then '0x0fが口座番号の後に来た場合エラーMSGが設定されている
                    'エラーMSG取得

                    Dim errSet As String = ""
                    Dim cnt As Integer = 0

                    errSet = Enco.GetString(byteRecordData, 49, 213)
                    For Each TextLine As String In errSet.Split(CType(Microsoft.VisualBasic.vbCrLf, Char))
                        If TextLine.Trim.Length <> 0 Then
                            errMsg(cnt) = TextLine.Trim
                            cnt += 1
                        End If
                    Next

                End If

                Dim SQL As New StringBuilder(128)
                SQL.Append("UPDATE JIKEIMAST")
                SQL.Append(" SET")
                SQL.Append(" ERR_CODE_JR  =" & SQ(strKekka))   'とりあえず1番目だけ更新
                SQL.Append(",ERR_MSG_JR =" & SQ(errMsg(0)))
                SQL.Append(" WHERE")
                SQL.Append("     TIME_STAMP_JR = " & SQ(TimeStamp))
                SQL.Append(" AND KAMOKU_CODE_JR = " & SQ(strOPE_CODE.Substring(0, 2)))
                SQL.Append(" AND OPE_CODE_JR = " & SQ(strOPE_CODE.Substring(2, 3)))
                SQL.Append(" AND RECORD_NO_JR        = " & lngTuuban)

                Dim nRet As Integer
                nRet = MainDB.ExecuteNonQuery(SQL)
                If nRet = 0 Then
                    MainLOG.Write("自振契約マスタ更新", "失敗", "更新明細なし")

                    JobMessage = "更新明細なし"
                    Return False
                Else
                    MainLOG.Write("自振契約マスタ更新", "成功", "")
                    JobMessage = ""
                End If

                '処理結果確認表の印刷データを設定する
                Item.ErrCode = strKekka
                Item.ErrMsg = errMsg(0)
                Item.Kamoku = strOPE_CODE.Substring(0, 2)
                Item.OpeCode = strOPE_CODE.Substring(2, 3)
                Item.TimeStamp = TimeStamp
                Item.RecordNo = lngTuuban
                Item.Jikinko = ini_info.JIKONKO_CODE
                If KFJP044.SetData(Item, MainDB) = False Then
                    JobMessage = "処理結果確認表(自振契約結果)印刷情報作成失敗"
                    Return False
                Else
                    DataList.Add(Item)
                End If

                If fr.Position <> 0 Then Pos += RecordLen

            Loop Until Filelen <= Pos

            KFJP044.OraDB = MainDB

            If KFJP044.MakeRecord(DataList) = False Then   '印刷対象なし
                JobMessage = "処理結果確認表(自振契約結果)印刷失敗"
                Return False
            End If

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String = ""

            'パラメータ設定：ログイン名、ＣＳＶファイル名
            param = MainLOG.UserID & "," & KFJP044.FileName

            Dim Ret As Integer = ExeRepo.ExecReport("KFJP044.EXE", param)
            If Ret <> 0 Then
                '印刷失敗：戻り値に対応したエラーメッセージを表示する
                Select Case Ret
                    Case -1
                        JobMessage = "処理結果確認表(自振契約結果)印刷対象０件。"
                    Case Else
                        JobMessage = "処理結果確認表(自振契約結果)印刷失敗。エラーコード：" & Ret
                End Select
                MainLOG.Write("処理結果確認表(自振契約結果)印刷", "失敗", JobMessage)

                Return False
            Else
                MainLOG.Write("処理結果確認表(自振契約結果)印刷", "成功")
            End If
        Catch
            Throw
        Finally
            If Not fr Is Nothing Then fr.Close()
            If Not br Is Nothing Then br.Close()
            fr = Nothing
            br = Nothing

            If File.Exists(TankingFileName) Then File.Delete(TankingFileName)

        End Try

        Return True

    End Function

    Private Function GetRientaFileName(ByVal TimeStamp As String, ByRef FileName As String) As Boolean

        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(MainDB)

        Try

            sql.Append("SELECT FILE_NAME_JR FROM JIKEIMAST WHERE TIME_STAMP_JR = '" & TimeStamp & "'")

            If orareader.DataReader(sql) = True Then
                FileName = orareader.GetString("FILE_NAME_JR")
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function

    Private Function CreateTankingData(ByVal TimeStamp As String, ByVal RientaFileName As String, ByRef CreateFileName As String) As Integer

        'リエンタ結果ファイルからデータ部の取り出し

        '元データ
        Dim fr As FileStream = Nothing
        Dim br As BinaryReader = Nothing
        '書込データ(書込を行うファイル)
        Dim fw As FileStream = Nothing
        Dim bw As BinaryWriter = Nothing

        Dim Enco As Encoding = Encoding.GetEncoding(50220)  'JIS
        Dim headFileName As String      'リエンタに書き込んであるファイル名
        Dim headKbn As String           'ヘッダ区分
        Dim headHiduke As String        'タンキング日付

        Dim FileNameOnly As String = Path.GetFileName(RientaFileName) '引数のフルパスリエンタファイル名からリエンタファイル名を抜き出してセット
        'Dim FileNameOnly As String = Path.GetFileNameWithoutExtension(RientaFileName) '結果ファイルは拡張子が消えるかも？

        Dim RecordLen As Integer = 256
        Dim StartPos As Integer = 1024

        Dim WorkFile As String = Path.Combine(ini_info.DAT_PATH, "RIENTER_WORK")

        Try


            fr = New FileStream(RientaFileName, FileMode.Open, FileAccess.Read)     'フロッピーのファイル
            br = New BinaryReader(fr)
            fw = New FileStream(WorkFile, FileMode.Create, FileAccess.Write)        '書き込み用のファイル
            bw = New BinaryWriter(fw)

            Dim byteRecordData(RecordLen - 1) As Byte

            Dim Filelen As Long = fr.Length
            Dim Pos As Long = 0

            Do
                byteRecordData = Nothing

                br.BaseStream.Seek(Pos, SeekOrigin.Begin)

                '1レコード読込
                byteRecordData = br.ReadBytes(RecordLen)

                '最初のレコードだったらチェック
                If Pos = 0 Then

                    headFileName = Enco.GetString(byteRecordData, 0, 12).Trim
                    headKbn = Enco.GetString(byteRecordData, 19, 1).Trim
                    headHiduke = Enco.GetString(byteRecordData, 24, 8).Trim

                    If headFileName.CompareTo(FileNameOnly) <> 0 Then
                        JobMessage = "ファイル名不一致"
                        MainLOG.Write("ワークファイル作成", "失敗", JobMessage)
                        Return -1
                    End If

                    If headKbn.CompareTo("E") <> 0 Then
                        JobMessage = "リエンタ済ファイルではありません"
                        MainLOG.Write("ワークファイル作成", "失敗", JobMessage)
                        Return -1
                    End If

                    If headHiduke.CompareTo(TimeStamp.Substring(0, 8)) <> 0 Then
                        JobMessage = "タンキング日付不一致"
                        MainLOG.Write("ワークファイル作成", "失敗", JobMessage)
                        Return -1
                    End If

                End If

                If Pos >= StartPos Then

                    bw.Write(byteRecordData)

                End If
                If fr.Position <> 0 Then
                    Pos += RecordLen
                End If

            Loop Until Filelen <= Pos

            If Not fr Is Nothing Then fr.Close()
            If Not br Is Nothing Then br.Close()
            If Not fw Is Nothing Then fw.Close()
            If Not bw Is Nothing Then bw.Close()
            fr = Nothing
            br = Nothing
            fw = Nothing
            bw = Nothing

            CreateFileName = Path.Combine(ini_info.DAT_PATH, "TANKING_WORK")

            'FTRAN+で顧客番号・口座番号のパック変換
            Dim strCMD As String
            Dim strDIR As String
            Dim strFTRDIR As String = CASTCommon.GetFSKJIni("COMMON", "FTR")
            Dim strFTRANPDIR As String = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
            Dim lngEXITCODE As Long

            Dim P_File As String = "RIENTA.p"
            strDIR = Microsoft.VisualBasic.CurDir()

            Microsoft.VisualBasic.CurDir(CType(strFTRANPDIR, Char))

            strCMD = "FP /nwd/ cload " & strFTRDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & WorkFile & " " & CreateFileName & " ++" & strFTRDIR & P_File

            Dim ProcFT As New Process
            Dim ProcInfo As New ProcessStartInfo(strFTRANPDIR & "FP", strCMD.Substring(3))
            ProcInfo.CreateNoWindow = True
            ProcInfo.WorkingDirectory = strFTRANPDIR
            ProcFT = Process.Start(ProcInfo)
            ProcFT.WaitForExit()
            lngEXITCODE = ProcFT.ExitCode

            If lngEXITCODE = 0 Then
                Return 0
            Else
                JobMessage = "コード変換"
                MainLOG.Write("ワークファイル作成", "失敗", JobMessage)
                Return 100         'コード変換失敗
            End If
        Catch ex As Exception
            Throw
        Finally
            If Not fr Is Nothing Then fr.Close()
            If Not br Is Nothing Then br.Close()
            If Not fw Is Nothing Then fw.Close()
            If Not bw Is Nothing Then bw.Close()
            fr = Nothing
            br = Nothing
            fw = Nothing
            bw = Nothing

            If File.Exists(WorkFile) Then File.Delete(WorkFile)

        End Try

    End Function

End Class
