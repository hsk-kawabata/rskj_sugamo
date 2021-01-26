Imports System
Imports System.IO
' 2016/01/22 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
Imports System.Text
' 2016/01/22 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END
' 2016/05/30 タスク）綾部 ADD 【RD】UI_B-14-99(RSV2対応) -------------------- START
Imports System.Reflection
' 2016/05/30 タスク）綾部 ADD 【RD】UI_B-14-99(RSV2対応) -------------------- END
' GENGOU 2019/04/19 ADD ITL)OOKUBO ------------------------------------------------- START
Imports System.Globalization
' GENGOU 2019/04/19 ADD ITL)OOKUBO ------------------------------------------------- END

Public Module ModPublic

    '
    ' 機能　 ： 文字列を括る
    '
    ' 引数　 ： ARG1 - 文字列
    '
    ' 戻り値 ： 括った後の文字列
    '
    ' 備考　 ：
    '
    Public Function SQ(ByVal value As String) As String
        If value Is Nothing Then
            Return "''"
        End If
        Return EncloseValue(value.Replace("'", "''"), "'")
    End Function

    '
    ' 機能　 ： 文字列を括る
    '
    ' 引数　 ： ARG1 - 文字列
    '
    ' 戻り値 ： 括った後の文字列
    '
    ' 備考　 ：
    '
    Public Function SQ(ByVal value As Integer) As String
        Return SQ(value.ToString)
    End Function

    '
    ' 機能　 ： 文字列を括る
    '
    ' 引数　 ： ARG1 - 文字列
    '
    ' 戻り値 ： 括った後の文字列
    '
    ' 備考　 ：
    '
    Public Function EncloseValue(ByVal value As String, Optional ByVal quotationmark As String = """") As String
        Return quotationmark & value & quotationmark
    End Function

    '
    ' 機能　 ： 文字列を括る
    '
    ' 引数　 ： ARG1 - Long値
    '
    ' 戻り値 ： 括った後の文字列
    '
    ' 備考　 ：
    '
    Public Function EncloseValue(ByVal value As Long, Optional ByVal quotationmark As String = """") As String
        Return EncloseValue(value.ToString, quotationmark)
    End Function

    '
    ' 機能　 ： 文字列を括る
    '
    ' 引数　 ： ARG1 - Decimal値
    '
    ' 戻り値 ： 括った後の文字列
    '
    ' 備考　 ：
    '
    Public Function EncloseValue(ByVal value As Decimal, Optional ByVal quotationmark As String = """") As String
        Return EncloseValue(value.ToString, quotationmark)
    End Function

    '
    ' 機能　 ： 文字列を括る
    '
    ' 引数　 ： ARG1 - Integer値
    '
    ' 戻り値 ： 括った後の文字列
    '
    ' 備考　 ：
    '
    Public Function EncloseValue(ByVal value As Integer, Optional ByVal quotationmark As String = """") As String
        Return EncloseValue(value.ToString, quotationmark)
    End Function

    '
    ' 機能　 ： 日付変換
    '
    ' 引数　 ： ARG1 - 日付文字列
    '
    ' 戻り値 ： 日付
    '
    ' 備考　 ：
    '
    Public Function ConvertDate(ByVal Value As String) As Date
        Try
            Dim sWork As String = Value.Replace("/", "").Replace(".", "")
            Dim Today As DateTime = CASTCommon.Calendar.Now
            If Value.Length = 4 Then
                ' ４桁の場合，MMDD として 日付にする
                ' 未来２ヶ月までを現在日付として，それより先の場合は，過去日付にする

                Dim nYear As Integer    ' 年
                If Today.ToString("MMdd") > Value Then
                    ' 現在より過去の月日の場合, 年を１年後にする
                    nYear = Today.Year + 1
                Else
                    nYear = Today.Year
                End If

                ' 2017/02/15 タスク）綾部 ADD 【OT】UI_99-99(RSV2対応) -------------------- START
                'Dim sTwoAfter As String ' ２ヶ月後
                'sTwoAfter = Today.AddMonths(2).ToString("yyyyMMdd") ' ２ヶ月後
                'If sTwoAfter < ((nYear).ToString & Value) Then
                '    ' ２ヶ月後 ＜ 対象日付
                '    nYear -= 1
                'End If
                'sWork = (nYear).ToString & Value
                Dim strShiftMonth As String = GetRSKJIni("RSV2_V1.0.0", "CONV_SHIFTMONTH")
                Dim intShiftMonth As Integer = 0
                Select Case strShiftMonth.ToUpper
                    Case "", "ERR", "0"
                        intShiftMonth = 2
                    Case Else
                        intShiftMonth = CInt(strShiftMonth.Trim)
                End Select

                Dim sShiftAfter As String
                sShiftAfter = Today.AddMonths(intShiftMonth).ToString("yyyyMMdd") ' 指定月後
                If sShiftAfter < ((nYear).ToString & Value) Then
                    ' ２ヶ月後 ＜ 対象日付
                    nYear -= 1
                End If
                sWork = (nYear).ToString & Value
                ' 2017/02/15 タスク）綾部 ADD 【OT】UI_99-99(RSV2対応) -------------------- END

            ElseIf Value.Length = 6 Then
                ' ６桁の場合，和暦と勝手に判断して，西暦日付８桁にする
                sWork = ConvertYear(Value.Substring(0, 2)).ToString & Value.Substring(2)
            ElseIf Value.Length = 14 Then
                ' 日時分秒
                sWork = sWork.Insert(4, "/")
                sWork = sWork.Insert(7, "/")
                sWork = sWork.Insert(10, " ")
                sWork = sWork.Insert(13, ":")
                sWork = sWork.Insert(16, ":")
                Return Date.Parse(sWork)
            End If
            If sWork.Length <> 8 Then
                Return New Date
            End If

            sWork = sWork.Insert(4, "/")
            sWork = sWork.Insert(7, "/")

            Return Date.Parse(sWork)
        Catch ex As Exception
            Return New Date
        End Try
    End Function

    '
    ' 機能　 ： 日付変換
    '
    ' 引数　 ： ARG1 - 日付文字列
    '        ： ARG2 - 書式
    '
    ' 戻り値 ： 日付
    '
    ' 備考　 ：
    '
    Public Function ConvertDate(ByVal sdt As String, ByVal fmt As String) As String
        Try
            Return ConvertDate(sdt).ToString(fmt)
        Catch ex As Exception
            Return ""
        End Try
    End Function

    '
    ' 機能　 ： 日付変換
    '
    ' 引数　 ： ARG1 - 日付文字列
    '        ： ARG2 - 書式
    '
    ' 戻り値 ： 日付
    '
    ' 備考　 ：
    '
    Public Function ConvertDate(ByVal dt As Date, ByVal fmt As String) As String
        Try
            Return String.Format(fmt, dt)
        Catch ex As Exception
            Return ""
        End Try
    End Function

    '
    ' 機能　 ： 数値変換
    '
    ' 引数　 ： ARG1 - 数値文字列
    '        ： ARG2 - 書式
    '
    ' 戻り値 ： 数値
    '
    ' 備考　 ：
    '
    Public Function CAInt32(ByVal value As String) As Int32
        Try
            Return Int32.Parse(value)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    '
    ' 機能　 ： 数値変換
    '
    ' 引数　 ： ARG1 - 数値文字列
    '        ： ARG2 - 書式
    '
    ' 戻り値 ： 数値
    '
    ' 備考　 ：
    '
    Public Function CALng(ByVal value As String) As Int64
        Try
            Return Int64.Parse(value)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    '
    ' 機能　 ： 数値変換
    '
    ' 引数　 ： ARG1 - 数値文字列
    '        ： ARG2 - 書式
    '
    ' 戻り値 ： 数値
    '
    ' 備考　 ：
    '
    Public Function CaDecNormal(ByVal value As String) As Decimal
        Try
            If IsDecimal(value) = True Then
                Return Decimal.Parse(value)
            End If

            Return 0
        Catch ex As Exception
            Return 0
        End Try
    End Function

    '
    ' 機能　 ： 数値変換
    '
    ' 引数　 ： ARG1 - 数値文字列
    '        ： ARG2 - 書式
    '
    ' 戻り値 ： 数値
    '
    ' 備考　 ：
    '
    Public Function CADec(ByVal value As String) As Decimal
        Try
            Return Decimal.Parse(value)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Function IsDecimal(ByVal value As String) As Boolean
        Try
            Return System.Text.RegularExpressions.Regex.IsMatch(value, "^\d+$")
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function ValidPath(ByVal name As String) As String
        For Each c As Char In Path.GetInvalidPathChars
            name.Replace(c, "")
        Next c

        Return name
    End Function

    '機能   :   和暦を西暦に変換(YY(和暦)→YYYY)
    '
    '引数   :   dateText  変換対象(YY(和暦))
    '
    '戻り値 :   西暦(YYYY)
    '
    '備考   :   
    '
    Public Function ConvertYear(ByVal dateText As String) As Integer
        ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- START
        'Dim dateTime As DateTime
        ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- END
        If Convert.ToInt32(dateText) = 0 Then
            Return 2000
        End If

        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- START
        'Dim JapanCal As New System.Globalization.JapaneseCalendar
        'dateTime = JapanCal.ToDateTime(Convert.ToInt32(dateText), 1, 1, 0, 0, 0, 0, 4)  '西暦取得
        'Return Convert.ToInt32(dateTime.ToString("yyyy"))

        Dim targetDate As Integer = Convert.ToInt32(dateText)

        '切り分け基準
        Dim strKijun As String = GetFSKJIni("COMMON", "GENGOHANBETU")
        Dim kijun As Integer = Convert.ToInt32(strKijun.Trim)

        Dim strGannen As String
        If targetDate < kijun Then
            '現在元号の基準年
            strGannen = GetFSKJIni("COMMON", "CURRENTGENGOKIJUN")
        Else
            '旧元号の基準年
            strGannen = GetFSKJIni("COMMON", "OLDGENGOKIJUN")
        End If
        Dim gannen As Integer = Convert.ToInt32(strGannen.Trim)

        Return gannen + targetDate - 1
        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- END
    End Function

    ' GENGOU 2019/04/19 ADD ITL)OOKUBO ------------------------------------------------- START
    ''' <summary>
    ''' 和暦変換
    ''' </summary>
    ''' <param name="strDate">変換する日付</param>
    ''' <param name="strFormat">変換するフォーマット</param>
    ''' <returns>変換後の日付</returns>
    ''' <remarks></remarks>
    Public Function GetWAREKI(ByVal strDate As String, ByVal strFormat As String) As String
        Dim culture As CultureInfo = New CultureInfo("ja-JP", True)
        culture.DateTimeFormat.Calendar = New JapaneseCalendar
        Dim target As DateTime = New DateTime(strDate.Substring(0, 4), strDate.Substring(4, 2), strDate.Substring(6, 2))
        Return target.ToString(strFormat, culture)
    End Function
    ' GENGOU 2019/04/19 ADD ITL)OOKUBO ------------------------------------------------- END

    '機能   :   営業日計算
    '
    '引数   :   
    '
    '戻り値 :  
    '
    '備考   :   
    '
    Public Function GetEigyobi(ByVal base As Date, ByVal days As Long, ByVal holiday As System.Collections.ArrayList) As Date
        If base = New Date Then
            Return base
        End If

        Dim nBaseNum As Long = 1
        If days < 0 Then
            nBaseNum *= -1
        End If

        For l As Long = nBaseNum To days Step nBaseNum
            Do While True
                base = base.AddDays(nBaseNum)

                If base.DayOfWeek() = DayOfWeek.Saturday OrElse _
                    base.DayOfWeek() = DayOfWeek.Sunday Then
                Else
                    ' 休日判定
                    If holiday.BinarySearch(base.ToString("yyyyMMdd")) >= 0 Then
                    Else
                        Exit Do
                    End If
                End If
            Loop
        Next l

        Return base
    End Function

    '機能   :   営業日計算（０日対応）
    '
    '引数   :   
    '
    '戻り値 :  
    '
    '備考   :   
    '
    Public Function GetEigyobiZero(ByVal base As Date, ByVal kubun As String, ByVal holiday As System.Collections.ArrayList) As Date
        Dim nBaseNum As Long = 1
        If kubun = "1" Then
            ' 前営業日
            nBaseNum *= -1
        End If

        Do While True
            If base.DayOfWeek() = DayOfWeek.Saturday OrElse _
                base.DayOfWeek() = DayOfWeek.Sunday Then
            Else
                ' 休日判定
                If holiday.BinarySearch(base.ToString("yyyyMMdd")) >= 0 Then
                Else
                    base = base.AddDays(nBaseNum)
                    Exit Do
                End If
            End If
            base = base.AddDays(nBaseNum)
        Loop

        Return base
    End Function

    Public Function ConvertKamoku1TO2(ByVal strKAMOKU As String) As String
        '=====================================================================================
        'NAME           :ConvertKamoku1TO2
        'Parameter      :strKAMOKU：文字コード（1桁）
        'Description    :規定外文字判定を行う
        'Return         :科目コード（2桁）
        'Create         :2004/07/29
        'Update         :
        '=====================================================================================

        ' 2016/10/19 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(金バッチ連携項目追加)) -------------------- START
        'Select Case CAInt32(strKAMOKU)
        '    Case 1
        '        Return "02"
        '    Case 2
        '        Return "01"
        '    Case 3
        '        Return "05"
        '    Case 4
        '        Return "37"
        '    Case 5
        '        Return "04"
        '    Case 9
        '        Return "02"
        '    Case Else
        '        Return "02"
        'End Select
        Select Case GetIni("科目変換テーブル.INI", "CONVERT_1TO2", "KAMOKU_" & strKAMOKU)
            Case "err", ""
                Select Case CAInt32(strKAMOKU)
                    Case 1
                        Return "02"
                    Case 2
                        Return "01"
                    Case 3
                        Return "05"
                    Case 4
                        Return "37"
                    Case 5
                        Return "04"
                    Case 9
                        Return "02"
                    Case Else
                        Return "02"
                End Select
            Case Else
                Return GetIni("科目変換テーブル.INI", "CONVERT_1TO2", "KAMOKU_" & strKAMOKU)
        End Select
        ' 2016/10/19 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(金バッチ連携項目追加)) -------------------- END

    End Function
    '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------START
    Public Function ConvertKamoku1TO2_K(ByVal strKAMOKU As String) As String
        '=====================================================================================
        'NAME           :ConvertKamoku1TO2_K
        'Parameter      :strKAMOKU：文字コード（1桁）
        'Description    :科目変換を行う
        'Return         :科目コード（2桁）
        'Create         :2011/06/28
        'Update         :
        '=====================================================================================

        ' 2016/10/19 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(金バッチ連携項目追加)) -------------------- START
        'Select Case CAInt32(strKAMOKU)
        '    Case 1
        '        Return "02"
        '    Case 2
        '        Return "01"
        '    Case 3
        '        Return "05"
        '    Case 4
        '        Return "37"
        '    Case 5
        '        Return "04"
        '    Case 9
        '        Return "09"
        '    Case Else
        '        Return "02"
        'End Select
        Select Case GetIni("科目変換テーブル.INI", "CONVERT_1TO2_K", "KAMOKU_" & strKAMOKU)
            Case "err", ""
                Select Case CAInt32(strKAMOKU)
                    Case 1
                        Return "02"
                    Case 2
                        Return "01"
                    Case 3
                        Return "05"
                    Case 4
                        Return "37"
                    Case 5
                        Return "04"
                    Case 9
                        Return "09"
                    Case Else
                        Return "02"
                End Select
            Case Else
                Return GetIni("科目変換テーブル.INI", "CONVERT_1TO2_K", "KAMOKU_" & strKAMOKU)
        End Select
        ' 2016/10/19 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(金バッチ連携項目追加)) -------------------- END

    End Function
    '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------END
    Public Function ConvertKamoku2TO1(ByVal strKAMOKU As String) As String
        '=====================================================================================
        'NAME           :ConvertKamoku2TO1
        'Parameter      :strKAMOKU：文字コード（1桁）
        'Description    :規定外文字判定を行う
        'Return         :科目コード（2桁）
        'Create         :2004/07/29
        'Update         :
        '=====================================================================================

        ' 2016/10/19 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(金バッチ連携項目追加)) -------------------- START
        'Select Case CAInt32(strKAMOKU)
        '    Case 1
        '        Return "2"
        '    Case 2
        '        Return "1"
        '    Case 4
        '        Return "5"
        '    Case 5
        '        Return "3"
        '    Case 37
        '        Return "4"
        '    Case 9
        '        Return "1"
        '    Case Else
        '        Return "9"
        'End Select
        Select Case GetIni("科目変換テーブル.INI", "CONVERT_2TO1", "KAMOKU_" & strKAMOKU)
            Case "err", ""
                Select Case CAInt32(strKAMOKU)
                    Case 1
                        Return "2"
                    Case 2
                        Return "1"
                    Case 4
                        Return "5"
                    Case 5
                        Return "3"
                    Case 37
                        Return "4"
                    Case 9
                        Return "1"
                    Case Else
                        Return "9"
                End Select
            Case Else
                Return GetIni("科目変換テーブル.INI", "CONVERT_2TO1", "KAMOKU_" & strKAMOKU)
        End Select
        ' 2016/10/19 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(金バッチ連携項目追加)) -------------------- END

    End Function
    '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------START
    Public Function ConvertKamoku2TO1_K(ByVal strKAMOKU As String) As String
        '=====================================================================================
        'NAME           :ConvertKamoku2TO1_K
        'Parameter      :strKAMOKU：文字コード（2桁）
        'Description    :規定外文字判定を行う
        'Return         :科目コード（1桁）
        'Create         :2011/06/28
        'Update         :
        '=====================================================================================

        ' 2016/10/19 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(金バッチ連携項目追加)) -------------------- START
        'Select Case CAInt32(strKAMOKU)
        '    Case 1
        '        Return "2"
        '    Case 2
        '        Return "1"
        '    Case 4
        '        Return "5"
        '    Case 5
        '        Return "3"
        '    Case 37
        '        Return "4"
        '    Case 9
        '        Return "9"
        '    Case Else
        '        Return "9"
        'End Select
        Select Case GetIni("科目変換テーブル.INI", "CONVERT_2TO1_K", "KAMOKU_" & strKAMOKU)
            Case "err", ""
                Select Case CAInt32(strKAMOKU)
                    Case 1
                        Return "2"
                    Case 2
                        Return "1"
                    Case 4
                        Return "5"
                    Case 5
                        Return "3"
                    Case 37
                        Return "4"
                    Case 9
                        Return "9"
                    Case Else
                        Return "9"
                End Select
            Case Else
                Return GetIni("科目変換テーブル.INI", "CONVERT_2TO1_K", "KAMOKU_" & strKAMOKU)
        End Select
        ' 2016/10/19 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(金バッチ連携項目追加)) -------------------- END

    End Function
    '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------END
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
    Public Function Cutting(ByRef value As String, ByVal len As Integer) As String
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

    Public Function MakeLogTrace() As String
        Dim strace As New System.Diagnostics.StackTrace(True)
        Dim count As Integer
        Dim Msg As New System.Text.StringBuilder(128)

        ' High up the call stack, there is only one stack frame
        While count < strace.FrameCount
            Dim frame As New System.Diagnostics.StackFrame
            frame = strace.GetFrame(count)
            Msg.Append(" : ")
            Msg.Append(frame.GetMethod())
            Msg.Append(" Line:")
            Msg.Append(frame.GetFileLineNumber().ToString)
            count += 1
        End While

        Return Msg.ToString
    End Function

    Public Function WatchAndWaitProcess(ByVal TimeSpan As Integer, ByVal WaitCount As Integer) As Integer
        Dim cnt As Integer = 0

        Try
            Dim myProcess As System.Diagnostics.Process = System.Diagnostics.Process.GetCurrentProcess '自分自身のプロセス
            Dim AllProcess() As System.Diagnostics.Process = System.Diagnostics.Process.GetProcesses() 'プロセス一覧
            Dim ExcessFlg As Boolean = False '強制実行フラグ

            '停止しているプロセス情報
            Dim errId As Integer = 0
            Dim errStartTime As Date = DateTime.Now

            'プロセス検索
            For Each targetProcess As System.Diagnostics.Process In AllProcess
                '同じプロセス名でプロセスIDが違う場合は二重起動検出
                If targetProcess.ProcessName = myProcess.ProcessName AndAlso targetProcess.Id <> myProcess.Id Then
                    '開始時間が自分より早い場合
                    If targetProcess.StartTime < myProcess.StartTime Then
                        '対象プロセスの最大待機時刻
                        Dim MaxWaitDate As DateTime = targetProcess.StartTime.AddMilliseconds(TimeSpan * WaitCount)

                        'プロセス終了するまでループ
                        While targetProcess.HasExited = False
                            '最大待機時刻が超過した場合は次のプロセス取得へ
                            If MaxWaitDate < DateTime.Now Then
                                ExcessFlg = True
                                errId = targetProcess.Id
                                errStartTime = targetProcess.StartTime
                                Exit While
                            End If

                            System.Threading.Thread.Sleep(TimeSpan)
                            cnt += 1
                        End While
                    End If
                End If
            Next

            If ExcessFlg Then
                '強制実行が行われた場合はイベントログ出力する
                Dim ELog As New ClsEventLOG
                ELog.Write("強制実行：実行中PID＝" & errId & ", " & _
                            "開始時刻：" & errStartTime.ToString("yyyy/MM/dd HH:mm:ss"), Diagnostics.EventLogEntryType.Warning)
            End If

        Catch
            '例外はスルー
        End Try

        Return cnt
    End Function

    ' 2016/01/22 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
    Public Function GetText_CodeToName(ByVal TxtFile As String, ByVal Code As String) As String

        Dim ReturnData As String = ""

        Dim sr As StreamReader = Nothing

        Try

            sr = New StreamReader(TxtFile, Encoding.GetEncoding("SHIFT-JIS"))
            While sr.Peek > -1
                Dim strLineData() As String = sr.ReadLine().Split(","c)
                If strLineData(0) = Code Then
                    ReturnData = strLineData(1).Trim
                    Exit While
                End If
            End While

            sr.Close()
            sr = Nothing

            Return ReturnData

        Catch ex As Exception
            Return ""
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
        End Try

    End Function

    Public Function GetText_CodeToName(ByVal TxtFile As String, ByVal Code As String, ByVal Column As Integer) As String

        Dim ReturnData As String = ""

        Dim sr As StreamReader = Nothing

        Try

            sr = New StreamReader(TxtFile, Encoding.GetEncoding("SHIFT-JIS"))
            While sr.Peek > -1
                Dim strLineData() As String = sr.ReadLine().Split(","c)
                If strLineData(0) = Code Then
                    ReturnData = strLineData(Column).Trim
                    Exit While
                End If
            End While

            sr.Close()
            sr = Nothing

            Return ReturnData

        Catch ex As Exception
            Return ""
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
        End Try

    End Function

    Public Function GetText_CodeToLine(ByVal TxtFile As String, ByVal Code As String) As String

        Dim ReturnData As String = ""

        Dim sr As StreamReader = Nothing

        Try

            sr = New StreamReader(TxtFile, Encoding.GetEncoding("SHIFT-JIS"))
            While sr.Peek > -1
                Dim strLine As String = sr.ReadLine()
                Dim strLineData() As String = strLine.Split(","c)
                If strLineData(0) = Code Then
                    ReturnData = strLine
                    Exit While
                End If
            End While

            sr.Close()
            sr = Nothing

            Return ReturnData

        Catch ex As Exception
            Return ""
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
        End Try

    End Function
    ' 2016/01/22 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END

    ' 2016/05/30 タスク）綾部 ADD 【RD】UI_B-14-99(RSV2対応) -------------------- START
    Public Function ExternalSub(ByVal dllName As String, ByVal className As String, ByVal methodName As String, ByVal CmdArgs() As String, ByRef RetValue As String) As Boolean

        Dim DllAsm As Assembly         ' DLLアセンブリ
        Dim ClassInstance As Object    ' クラスインスタンス
        RetValue = ""

        Try

            Try
                DllAsm = System.Reflection.Assembly.LoadFrom(dllName & ".dll")
            Catch ex As Exception
                Return False
            End Try

            ' クラスをインスタンス化
            ClassInstance = DllAsm.CreateInstance(dllName & "." & className)
            If ClassInstance Is Nothing Then
                Return False
            End If

            ' メソッド呼出し
            ' 業務固有印刷メソッド開始ログ出力
            Dim methodInfo As MethodInfo = ClassInstance.GetType.GetMethod(methodName)
            If methodInfo Is Nothing Then
                Return False
            End If

            RetValue = methodInfo.Invoke(ClassInstance, CmdArgs)

            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function ExternalSub(ByVal dllName As String, ByVal className As String, ByVal methodName As String, ByVal CmdArgs() As String, ByRef RetValue As Integer) As Boolean

        Dim DllAsm As Assembly         ' DLLアセンブリ
        Dim ClassInstance As Object    ' クラスインスタンス
        RetValue = 0

        Try

            Try
                DllAsm = System.Reflection.Assembly.LoadFrom(dllName & ".dll")
            Catch ex As Exception
                Return False
            End Try

            ' クラスをインスタンス化
            ClassInstance = DllAsm.CreateInstance(dllName & "." & className)
            If ClassInstance Is Nothing Then
                Return False
            End If

            ' メソッド呼出し
            ' 業務固有印刷メソッド開始ログ出力
            Dim methodInfo As MethodInfo = ClassInstance.GetType.GetMethod(methodName)
            If methodInfo Is Nothing Then
                Return False
            End If

            Dim methodParams() As Object = {CmdArgs}

            RetValue = methodInfo.Invoke(ClassInstance, methodParams)

            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function ExternalSub(ByVal dllName As String, ByVal className As String, ByVal methodName As String, ByVal CmdArgs() As String, ByRef RetValue As Boolean) As Boolean

        Dim DllAsm As Assembly         ' DLLアセンブリ
        Dim ClassInstance As Object    ' クラスインスタンス
        RetValue = False

        Try

            Try
                DllAsm = System.Reflection.Assembly.LoadFrom(dllName & ".dll")
            Catch ex As Exception
                Return False
            End Try

            ' クラスをインスタンス化
            ClassInstance = DllAsm.CreateInstance(dllName & "." & className)
            If ClassInstance Is Nothing Then
                Return False
            End If

            ' メソッド呼出し
            ' 業務固有印刷メソッド開始ログ出力
            Dim methodInfo As MethodInfo = ClassInstance.GetType.GetMethod(methodName)
            If methodInfo Is Nothing Then
                Return False
            End If

            RetValue = methodInfo.Invoke(ClassInstance, CmdArgs)

            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function
    ' 2016/05/30 タスク）綾部 ADD 【RD】UI_B-14-99(RSV2対応) -------------------- END

End Module
