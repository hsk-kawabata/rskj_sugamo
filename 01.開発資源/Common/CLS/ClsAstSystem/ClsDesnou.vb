Imports System
Imports System.IO
Imports System.Diagnostics

Public Class ClsDensou
    ' 業務ヘッダパス
    Private GYOMUHEADPATH As String
    Private GYOMUHEADNAME As String

    Public Message As String = ""
    Public MessageDetail As String = ""

    ' 業務ヘッダ
    Structure DENSOUPARAM
        Dim RecordKubun As String       ' レコード区分
        Dim RecordName As String        ' レコード識別名
        Dim CenterCode As String        ' 相手センター確認コード
        Dim TouhouCode As String        ' 当方センター確認コード
        Dim ZenginName As String        ' 全銀ファイル名
        Dim Syuhaisin As String         ' 集配信区分
        Dim DensouNitiji As String      ' 伝送日時
        Dim HostTuuban As String        ' ホスト通番
        Dim CodeKubun As String         ' コード区分
        Dim EncodeKubun As String       ' 暗号化区分
        Dim RecordLen As String         ' レコード長
        Dim RecordCount As String       ' レコード件数
        Dim FileName As String          ' ファイル名    
        Dim AES As String               ' ＡＥＳ（0:AESなし，1:32，2:48，3:64）
        Dim EncodeKey As String         ' 暗号化キー
        Dim EncodeVIKey As String       ' 暗号化IVキー
        Dim Yobi As String              ' 予備
        '固定長データ処理用プロパティ
        Public Property Data() As String
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(RecordKubun, 1), _
                    SubData(RecordName, 4), _
                    SubData(CenterCode, 14), _
                    SubData(TouhouCode, 14), _
                    SubData(ZenginName, 12), _
                    SubData(Syuhaisin, 1), _
                    SubData(DensouNitiji, 14), _
                    SubData(HostTuuban, 2), _
                    SubData(CodeKubun, 1), _
                    SubData(EncodeKubun, 1), _
                    SubData(RecordLen, 4, 1, "0"c), _
                    SubData(RecordCount, 8, 1, "0"c), _
                    SubData(FileName, 50), _
                    SubData(AES, 1), _
                    SubData(EncodeKey, 64), _
                    SubData(EncodeVIKey, 32), _
                    SubData(Yobi, 27) _
                    })
            End Get
            Set(ByVal value As String)
                value = value.PadRight(200, " "c)

                RecordKubun = CuttingData(value, 1)
                RecordName = CuttingData(value, 4).Trim
                CenterCode = CuttingData(value, 14).Trim
                TouhouCode = CuttingData(value, 14).Trim
                ZenginName = CuttingData(value, 12).Trim
                Syuhaisin = CuttingData(value, 1)
                DensouNitiji = CuttingData(value, 14)
                HostTuuban = CuttingData(value, 2)
                CodeKubun = CuttingData(value, 1)
                EncodeKubun = CuttingData(value, 1)
                RecordLen = CuttingData(value, 4).Trim
                RecordCount = CuttingData(value, 8)
                FileName = CuttingData(value, 50).Trim
                AES = CuttingData(value, 1)
                EncodeKey = CuttingData(value, 64)
                EncodeVIKey = CuttingData(value, 32)
                Yobi = CuttingData(value, 27)
            End Set
        End Property
    End Structure

    ' 業務ヘッダ
    Public Property GyoumuHeadName() As String
        Get
            ' 業務ヘッダ名称取得（パス付き）
            Return Path.Combine(GYOMUHEADPATH, GYOMUHEADNAME)
        End Get
        Set(ByVal Value As String)
            ' 業務ヘッダ名称 設定
            GYOMUHEADPATH = Path.GetDirectoryName(Value)
            If GYOMUHEADPATH = "" Then
                GYOMUHEADPATH = CASTCommon.GetFSKJIni("OTHERSYS", "GYOMUPATH")
                GYOMUHEADNAME = Value
            Else
                GYOMUHEADNAME = Path.GetFileName(Value)
            End If
        End Set
    End Property

    Public Function LinkProcess() As Boolean
        ' LinkExpress 登録
        '*** 修正 mitsu 2008/07/24 エラー時はパラメータも返す ***
        Dim errArguments As String = ""
        '********************************************************
        Try
            Dim Proc As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = CASTCommon.GetFSKJIni("OTHERSYS", "LINK")
            Dim Argum As String = CASTCommon.GetFSKJIni("OTHERSYS", "LINK-PARAM")
            If File.Exists(ProcInfo.FileName) = True Then
                ProcInfo.WorkingDirectory = GYOMUHEADPATH
                ProcInfo.Arguments = Argum.Replace("%1", Path.Combine(GYOMUHEADPATH, GYOMUHEADNAME)).Replace("%2", GYOMUHEADNAME)
                '*** 修正 mitsu 2008/07/24 エラー時はパラメータも返す ***
                errArguments = ProcInfo.Arguments
                '********************************************************
                Proc = Process.Start(ProcInfo)
                Proc.WaitForExit()
                If Proc.ExitCode = 0 Then
                    ' 連携成功
                    Return True
                Else
                    ' 連携失敗
                    Message = "Return:" & Proc.ExitCode.ToString & " " & ProcInfo.FileName & " " & ProcInfo.Arguments
                    Proc.StandardOutput.ReadToEnd()
                    Return False
                End If
            Else
                Message = "起動アプリケーションなし：" & ProcInfo.FileName
                Return False
            End If
            Proc.Close()
        Catch ex As Exception
            '*** 修正 mitsu 2008/07/24 エラー時はパラメータも返す ***
            'Message = ex.Message
            Message = ex.Message & " " & errArguments
            '********************************************************
            Return False
        End Try
    End Function

    Public Function ReadHeader() As String
        Dim ReadData As String

        Dim FStream As New StreamReader(GyoumuHeadName, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
        ReadData = FStream.ReadLine()
        FStream.Close()
        FStream = Nothing

        Return ReadData
    End Function

    Public Function SaveHeader(ByVal data As String) As Boolean
        Dim sw As New StreamWriter(GyoumuHeadName, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
        sw.Write(data)
        sw.Close()

        sw = Nothing
    End Function

    '
    ' 機能　 ： 文字列から，指定の長さを切り取る
    '
    ' 引数　 ： ARG1 - 文字列
    ' 　　　 　 ARG2 - 長さ
    '           ARG3 - ０：左詰，１：右詰
    '           ARG4 - 埋め文字
    '
    ' 戻り値 ： 切り取った後の残りの文字列
    '
    ' 備考　 ：
    '
    Protected Friend Shared Function SubData(ByVal value As String, ByVal len As Integer, _
                Optional ByVal align As Integer = 0, Optional ByVal pad As Char = " "c) As String
        Try
            ' 切り取る文字列
            If align = 0 Then
                ' 左詰
                value = value.PadRight(len, pad)
            Else
                ' 右詰
                value = value.PadLeft(len, pad)
            End If
            Dim bt() As Byte = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes(value)
            Return System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(bt, 0, len)
        Catch ex As Exception
            Return New String(" "c, len)
        End Try
    End Function

    '
    ' 機能　 ： 文字列から，指定の長さを切り取る
    '
    ' 引数　 ： ARG1 - 文字列
    ' 　　　 　 ARG2 - 長さ
    '
    ' 戻り値 ： 切り取った後の残りの文字列
    '
    ' 備考　 ：
    '
    Protected Friend Shared Function CuttingData(ByRef value As String, ByVal len As Integer) As String
        Try
            ' 切り取る文字列
            Dim ret As String
            Dim bt() As Byte = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes(value)
            ret = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(bt, 0, len)
            ' 切り取った後の残りの文字列
            value = value.Substring(ret.Length())
            Return ret
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class
