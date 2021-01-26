Imports System
Imports System.Collections
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Imports Microsoft.VisualBasic
Public Class KFGP501
    Inherits CAstReports.ClsReportBase

    Public Gakkou_code As String
    Public Err_Count As Integer

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP501"

        ' 定義体名セット
        ReportBaseName = "KFGP501_生徒データ取込登録リスト.rpd"
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

        Dim red As System.IO.StreamReader = Nothing
        Dim strReadLine As String

        Dim red_err As System.IO.StreamReader = Nothing
        Dim strReadLine_err As String

        Dim Red_File_Name As String '読込ログファイル名
        Dim Err_File_Name As String 'エラーログファイル名

        Dim strERR_NO As New ArrayList
        Dim strERR_MSG As New ArrayList

        Try
            Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
            Dim NowTime As String = Format(DateTime.Now, "HHmmss")
            '--------------------------------
            ' 設定ファイル取得
            '--------------------------------

            '読込ログファイル
            Red_File_Name = CASTCommon.GetFSKJIni("GCOMMON", "LOG")
            If Red_File_Name.ToUpper = "ERR" OrElse Red_File_Name = Nothing Then
                BatchLog.Write("設定ファイル取得", "失敗", "項目名:読込ログフォルダ 分類:GCOMMON 項目:LOG")
                Return False
            End If

            'エラーログファイル
            Err_File_Name = CASTCommon.GetFSKJIni("GCOMMON", "LOG")
            If Err_File_Name.ToUpper = "ERR" OrElse Err_File_Name = Nothing Then
                BatchLog.Write("設定ファイル取得", "失敗", "項目名:エラーログフォルダ 分類:GCOMMON 項目:LOG")
                Return False
            End If

            '読込ログファイルの読み込み
            Dim dirs As String() = System.IO.Directory.GetFiles(Red_File_Name)

            Red_File_Name &= "Red_SEITO" & Gakkou_code & ".log"
            Dim dir As String
            Dim dirflg As String = 0

            For Each dir In dirs
                If dir = Red_File_Name Then
                    dirflg = "1"
                End If
            Next

            If dirflg = "0" Then
                Throw New FileNotFoundException(String.Format("ファイルが見つかりません。" _
                                        & ControlChars.Cr & "'{0}'", Red_File_Name))
            End If

            red = New System.IO.StreamReader(Red_File_Name, System.Text.Encoding.Default)


            'エラーログファイルの読み込み
            dirs = System.IO.Directory.GetFiles(Err_File_Name)

            Err_File_Name &= "Err_SEITO" & Gakkou_code & ".log"
            dirflg = "0"

            For Each dir In dirs
                If dir = Err_File_Name Then
                    dirflg = "1"
                End If
            Next


            If dirflg = "0" Then
                'Throw New FileNotFoundException(String.Format("ファイルが見つかりません。" _
                '                        & ControlChars.Cr & "'{0}'", Err_File_Name))
            Else
                'エラーログの読込
                Dim strSplitValue_err() As String

                red_err = New System.IO.StreamReader(Err_File_Name, System.Text.Encoding.Default)
                Do While Not red_err.Peek() = -1
                    strReadLine_err = red_err.ReadLine.ToString
                    strSplitValue_err = Split(strReadLine_err, ",")
                    strERR_NO.Add(strSplitValue_err(0))
                    strERR_MSG.Add(strSplitValue_err(1) & "：" & strSplitValue_err(2))
                Loop
            End If

            'レコード番号の取得,申請区分の取得
            Dim Rec_No As String
            Dim SNS_Msg As String

            Dim intcnt As Integer = 0

            Dim strSplitValue() As String

            '移入データの読込
            Do While Not red.Peek() = -1

                strReadLine = red.ReadLine.ToString
                strSplitValue = Split(strReadLine, ",")

                Rec_No = ""
                SNS_Msg = ""

                OutputCsvData(Today, True)  '処理日
                OutputCsvData(NowTime, True)  '処理時間
                For intcnt = 0 To UBound(strSplitValue)
                    OutputCsvData(strSplitValue(intcnt), True)  '移入データ
                    Select intcnt
                        Case 0
                            Rec_No = strSplitValue(intcnt)
                        Case 1
                            SNS_Msg = strSplitValue(intcnt)
                    End Select
                Next intcnt

                If strERR_NO.IndexOf(Rec_No) <> -1 And SNS_Msg <> "変更前" Then
                    OutputCsvData(strERR_MSG(strERR_NO.IndexOf(Rec_No)), True) 'エラー内容
                    OutputCsvData("", True, True) '改行レコード
                Else
                    If SNS_Msg <> "変更前" Then
                        OutputCsvData("○", True) 'エラー内容
                        OutputCsvData("", True, True) '改行レコード
                    Else
                        OutputCsvData("", True) 'エラー内容
                        OutputCsvData("", True, True) '改行レコード
                    End If
                End If
                RecordCnt += 1
            Loop

            Return True

        Catch ex As FileNotFoundException
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ファイルなし", "失敗", ex.Message)
            Return False
        Catch ex As System.IO.IOException
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ファイルアクセス", "失敗", ex.Message)
            Return False
        Catch ex As Exception
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
            If Not red_err Is Nothing Then red_err.Close()
        End Try
    End Function

End Class
