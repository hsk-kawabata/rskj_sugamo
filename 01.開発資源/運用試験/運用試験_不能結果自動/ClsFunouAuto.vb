Imports System.Data.OracleClient
Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Imports CAstExternal

''' <summary>
''' 運用試験_不能結果自動　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class ClsFunouAuto

#Region "クラス変数"

    Public MainLOG As New CASTCommon.BatchLOG("KFO020", "運用試験_不能結果自動")
    Private MainDB As CASTCommon.MyOracle

    Private GCom As MenteCommon.clsCommon

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

#End Region

#Region "パブリックメソッド"

    Public Function Main() As Integer

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "不能結果更新処理(運用試験時)", "開始", "")
            GCom = New MenteCommon.clsCommon

            '-----------------------------------------------------------
            'テキストボックスの入力チェック
            '-----------------------------------------------------------
            Dim SystemDate As String = Now.ToString("yyyyMMdd")
            Dim JyushinDate As String = SystemDate
            Dim JyushinPath As String = "\\192.168.3.55\d$\FSKJ\DAT\KEKKA\II_KOBETU\NORMAL\JIF\MT\"

            Dim FileList As String() = Directory.GetFiles(JyushinPath)
            If FileList.Length = 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "伝送ファイルコピー処理", "成功", "受信日 [" & JyushinDate & "] の結果データなし")
                Return 1
            ElseIf FileList.Length >= 2 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "伝送ファイルコピー処理", "成功", "受信日 [" & JyushinDate & "] の結果データ複数存在")
                Return 1
            End If

            '----------------------------------------------------------
            ' ファイル移動開始
            '----------------------------------------------------------
            Dim FileFullPath As String = String.Empty

            For Each FileFullPath In FileList
                Dim FileName As String = Path.GetFileName(FileFullPath)
                Dim OutputPath As String = "C:\Linkexpress\"
                Dim RetFileName As String = "FUNOU_OT.DAT"

                File.Copy(FileFullPath, Path.Combine(OutputPath, RetFileName))

                System.Threading.Thread.Sleep(3000)
                Dim Proc As New Process
                Dim ProcInfo As New ProcessStartInfo
                ProcInfo.FileName = "C:\RSKJ\EXE\KFD001.EXE"
                ProcInfo.Arguments = Path.Combine(OutputPath, RetFileName)
                If File.Exists(ProcInfo.FileName) = True Then

                    ProcInfo.WorkingDirectory = ""

                    MainLOG.Write("伝送連携受信実行", "開始", "Command :" & ProcInfo.FileName)
                    MainLOG.Write("伝送連携受信実行", "　　", "Parameta:" & ProcInfo.Arguments)

                    Proc = Process.Start(ProcInfo)
                    Proc.WaitForExit()

                    If Proc.ExitCode = 0 Then
                        ' 連携成功
                    Else
                        ' 連携失敗
                        MainLOG.Write("伝送連携受信コマンド実行", "失敗", "Return:" & Proc.ExitCode.ToString & " " & ProcInfo.FileName & " " & ProcInfo.Arguments)
                        Proc.StandardOutput.ReadToEnd()
                        Return 1
                    End If
                Else
                    MainLOG.Write("伝送連携受信コマンド実行", "失敗", "起動アプリケーションなし：" & ProcInfo.FileName)
                    Return 1
                End If

            Next

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "不能結果更新処理(運用試験時)", "成功", "")

            Return 0

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "不能結果更新処理(運用試験時)", "失敗", ex.ToString)
            Return 1
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "不能結果更新処理(運用試験時)", "終了", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try

    End Function

#End Region

#Region "プライベートメソッド"



#End Region

End Class
