Imports System
Imports System.IO
' 2016/01/22 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
Imports System.Text
' 2016/01/22 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END
' 2016/05/30 �^�X�N�j���� ADD �yRD�zUI_B-14-99(RSV2�Ή�) -------------------- START
Imports System.Reflection
' 2016/05/30 �^�X�N�j���� ADD �yRD�zUI_B-14-99(RSV2�Ή�) -------------------- END
' GENGOU 2019/04/19 ADD ITL)OOKUBO ------------------------------------------------- START
Imports System.Globalization
' GENGOU 2019/04/19 ADD ITL)OOKUBO ------------------------------------------------- END

Public Module ModPublic

    '
    ' �@�\�@ �F �����������
    '
    ' �����@ �F ARG1 - ������
    '
    ' �߂�l �F ��������̕�����
    '
    ' ���l�@ �F
    '
    Public Function SQ(ByVal value As String) As String
        If value Is Nothing Then
            Return "''"
        End If
        Return EncloseValue(value.Replace("'", "''"), "'")
    End Function

    '
    ' �@�\�@ �F �����������
    '
    ' �����@ �F ARG1 - ������
    '
    ' �߂�l �F ��������̕�����
    '
    ' ���l�@ �F
    '
    Public Function SQ(ByVal value As Integer) As String
        Return SQ(value.ToString)
    End Function

    '
    ' �@�\�@ �F �����������
    '
    ' �����@ �F ARG1 - ������
    '
    ' �߂�l �F ��������̕�����
    '
    ' ���l�@ �F
    '
    Public Function EncloseValue(ByVal value As String, Optional ByVal quotationmark As String = """") As String
        Return quotationmark & value & quotationmark
    End Function

    '
    ' �@�\�@ �F �����������
    '
    ' �����@ �F ARG1 - Long�l
    '
    ' �߂�l �F ��������̕�����
    '
    ' ���l�@ �F
    '
    Public Function EncloseValue(ByVal value As Long, Optional ByVal quotationmark As String = """") As String
        Return EncloseValue(value.ToString, quotationmark)
    End Function

    '
    ' �@�\�@ �F �����������
    '
    ' �����@ �F ARG1 - Decimal�l
    '
    ' �߂�l �F ��������̕�����
    '
    ' ���l�@ �F
    '
    Public Function EncloseValue(ByVal value As Decimal, Optional ByVal quotationmark As String = """") As String
        Return EncloseValue(value.ToString, quotationmark)
    End Function

    '
    ' �@�\�@ �F �����������
    '
    ' �����@ �F ARG1 - Integer�l
    '
    ' �߂�l �F ��������̕�����
    '
    ' ���l�@ �F
    '
    Public Function EncloseValue(ByVal value As Integer, Optional ByVal quotationmark As String = """") As String
        Return EncloseValue(value.ToString, quotationmark)
    End Function

    '
    ' �@�\�@ �F ���t�ϊ�
    '
    ' �����@ �F ARG1 - ���t������
    '
    ' �߂�l �F ���t
    '
    ' ���l�@ �F
    '
    Public Function ConvertDate(ByVal Value As String) As Date
        Try
            Dim sWork As String = Value.Replace("/", "").Replace(".", "")
            Dim Today As DateTime = CASTCommon.Calendar.Now
            If Value.Length = 4 Then
                ' �S���̏ꍇ�CMMDD �Ƃ��� ���t�ɂ���
                ' �����Q�����܂ł����ݓ��t�Ƃ��āC�������̏ꍇ�́C�ߋ����t�ɂ���

                Dim nYear As Integer    ' �N
                If Today.ToString("MMdd") > Value Then
                    ' ���݂��ߋ��̌����̏ꍇ, �N���P�N��ɂ���
                    nYear = Today.Year + 1
                Else
                    nYear = Today.Year
                End If

                ' 2017/02/15 �^�X�N�j���� ADD �yOT�zUI_99-99(RSV2�Ή�) -------------------- START
                'Dim sTwoAfter As String ' �Q������
                'sTwoAfter = Today.AddMonths(2).ToString("yyyyMMdd") ' �Q������
                'If sTwoAfter < ((nYear).ToString & Value) Then
                '    ' �Q������ �� �Ώۓ��t
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
                sShiftAfter = Today.AddMonths(intShiftMonth).ToString("yyyyMMdd") ' �w�茎��
                If sShiftAfter < ((nYear).ToString & Value) Then
                    ' �Q������ �� �Ώۓ��t
                    nYear -= 1
                End If
                sWork = (nYear).ToString & Value
                ' 2017/02/15 �^�X�N�j���� ADD �yOT�zUI_99-99(RSV2�Ή�) -------------------- END

            ElseIf Value.Length = 6 Then
                ' �U���̏ꍇ�C�a��Ə���ɔ��f���āC������t�W���ɂ���
                sWork = ConvertYear(Value.Substring(0, 2)).ToString & Value.Substring(2)
            ElseIf Value.Length = 14 Then
                ' �������b
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
    ' �@�\�@ �F ���t�ϊ�
    '
    ' �����@ �F ARG1 - ���t������
    '        �F ARG2 - ����
    '
    ' �߂�l �F ���t
    '
    ' ���l�@ �F
    '
    Public Function ConvertDate(ByVal sdt As String, ByVal fmt As String) As String
        Try
            Return ConvertDate(sdt).ToString(fmt)
        Catch ex As Exception
            Return ""
        End Try
    End Function

    '
    ' �@�\�@ �F ���t�ϊ�
    '
    ' �����@ �F ARG1 - ���t������
    '        �F ARG2 - ����
    '
    ' �߂�l �F ���t
    '
    ' ���l�@ �F
    '
    Public Function ConvertDate(ByVal dt As Date, ByVal fmt As String) As String
        Try
            Return String.Format(fmt, dt)
        Catch ex As Exception
            Return ""
        End Try
    End Function

    '
    ' �@�\�@ �F ���l�ϊ�
    '
    ' �����@ �F ARG1 - ���l������
    '        �F ARG2 - ����
    '
    ' �߂�l �F ���l
    '
    ' ���l�@ �F
    '
    Public Function CAInt32(ByVal value As String) As Int32
        Try
            Return Int32.Parse(value)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    '
    ' �@�\�@ �F ���l�ϊ�
    '
    ' �����@ �F ARG1 - ���l������
    '        �F ARG2 - ����
    '
    ' �߂�l �F ���l
    '
    ' ���l�@ �F
    '
    Public Function CALng(ByVal value As String) As Int64
        Try
            Return Int64.Parse(value)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    '
    ' �@�\�@ �F ���l�ϊ�
    '
    ' �����@ �F ARG1 - ���l������
    '        �F ARG2 - ����
    '
    ' �߂�l �F ���l
    '
    ' ���l�@ �F
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
    ' �@�\�@ �F ���l�ϊ�
    '
    ' �����@ �F ARG1 - ���l������
    '        �F ARG2 - ����
    '
    ' �߂�l �F ���l
    '
    ' ���l�@ �F
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

    '�@�\   :   �a��𐼗�ɕϊ�(YY(�a��)��YYYY)
    '
    '����   :   dateText  �ϊ��Ώ�(YY(�a��))
    '
    '�߂�l :   ����(YYYY)
    '
    '���l   :   
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
        'dateTime = JapanCal.ToDateTime(Convert.ToInt32(dateText), 1, 1, 0, 0, 0, 0, 4)  '����擾
        'Return Convert.ToInt32(dateTime.ToString("yyyy"))

        Dim targetDate As Integer = Convert.ToInt32(dateText)

        '�؂蕪���
        Dim strKijun As String = GetFSKJIni("COMMON", "GENGOHANBETU")
        Dim kijun As Integer = Convert.ToInt32(strKijun.Trim)

        Dim strGannen As String
        If targetDate < kijun Then
            '���݌����̊�N
            strGannen = GetFSKJIni("COMMON", "CURRENTGENGOKIJUN")
        Else
            '�������̊�N
            strGannen = GetFSKJIni("COMMON", "OLDGENGOKIJUN")
        End If
        Dim gannen As Integer = Convert.ToInt32(strGannen.Trim)

        Return gannen + targetDate - 1
        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- END
    End Function

    ' GENGOU 2019/04/19 ADD ITL)OOKUBO ------------------------------------------------- START
    ''' <summary>
    ''' �a��ϊ�
    ''' </summary>
    ''' <param name="strDate">�ϊ�������t</param>
    ''' <param name="strFormat">�ϊ�����t�H�[�}�b�g</param>
    ''' <returns>�ϊ���̓��t</returns>
    ''' <remarks></remarks>
    Public Function GetWAREKI(ByVal strDate As String, ByVal strFormat As String) As String
        Dim culture As CultureInfo = New CultureInfo("ja-JP", True)
        culture.DateTimeFormat.Calendar = New JapaneseCalendar
        Dim target As DateTime = New DateTime(strDate.Substring(0, 4), strDate.Substring(4, 2), strDate.Substring(6, 2))
        Return target.ToString(strFormat, culture)
    End Function
    ' GENGOU 2019/04/19 ADD ITL)OOKUBO ------------------------------------------------- END

    '�@�\   :   �c�Ɠ��v�Z
    '
    '����   :   
    '
    '�߂�l :  
    '
    '���l   :   
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
                    ' �x������
                    If holiday.BinarySearch(base.ToString("yyyyMMdd")) >= 0 Then
                    Else
                        Exit Do
                    End If
                End If
            Loop
        Next l

        Return base
    End Function

    '�@�\   :   �c�Ɠ��v�Z�i�O���Ή��j
    '
    '����   :   
    '
    '�߂�l :  
    '
    '���l   :   
    '
    Public Function GetEigyobiZero(ByVal base As Date, ByVal kubun As String, ByVal holiday As System.Collections.ArrayList) As Date
        Dim nBaseNum As Long = 1
        If kubun = "1" Then
            ' �O�c�Ɠ�
            nBaseNum *= -1
        End If

        Do While True
            If base.DayOfWeek() = DayOfWeek.Saturday OrElse _
                base.DayOfWeek() = DayOfWeek.Sunday Then
            Else
                ' �x������
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
        'Parameter      :strKAMOKU�F�����R�[�h�i1���j
        'Description    :�K��O����������s��
        'Return         :�ȖڃR�[�h�i2���j
        'Create         :2004/07/29
        'Update         :
        '=====================================================================================

        ' 2016/10/19 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(���o�b�`�A�g���ڒǉ�)) -------------------- START
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
        Select Case GetIni("�Ȗڕϊ��e�[�u��.INI", "CONVERT_1TO2", "KAMOKU_" & strKAMOKU)
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
                Return GetIni("�Ȗڕϊ��e�[�u��.INI", "CONVERT_1TO2", "KAMOKU_" & strKAMOKU)
        End Select
        ' 2016/10/19 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(���o�b�`�A�g���ڒǉ�)) -------------------- END

    End Function
    '2011/06/28 �W���ŏC�� ���U�̏ꍇ�A�Ȗڂ��̑��́A09�ɕύX ------------------START
    Public Function ConvertKamoku1TO2_K(ByVal strKAMOKU As String) As String
        '=====================================================================================
        'NAME           :ConvertKamoku1TO2_K
        'Parameter      :strKAMOKU�F�����R�[�h�i1���j
        'Description    :�Ȗڕϊ����s��
        'Return         :�ȖڃR�[�h�i2���j
        'Create         :2011/06/28
        'Update         :
        '=====================================================================================

        ' 2016/10/19 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(���o�b�`�A�g���ڒǉ�)) -------------------- START
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
        Select Case GetIni("�Ȗڕϊ��e�[�u��.INI", "CONVERT_1TO2_K", "KAMOKU_" & strKAMOKU)
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
                Return GetIni("�Ȗڕϊ��e�[�u��.INI", "CONVERT_1TO2_K", "KAMOKU_" & strKAMOKU)
        End Select
        ' 2016/10/19 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(���o�b�`�A�g���ڒǉ�)) -------------------- END

    End Function
    '2011/06/28 �W���ŏC�� ���U�̏ꍇ�A�Ȗڂ��̑��́A09�ɕύX ------------------END
    Public Function ConvertKamoku2TO1(ByVal strKAMOKU As String) As String
        '=====================================================================================
        'NAME           :ConvertKamoku2TO1
        'Parameter      :strKAMOKU�F�����R�[�h�i1���j
        'Description    :�K��O����������s��
        'Return         :�ȖڃR�[�h�i2���j
        'Create         :2004/07/29
        'Update         :
        '=====================================================================================

        ' 2016/10/19 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(���o�b�`�A�g���ڒǉ�)) -------------------- START
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
        Select Case GetIni("�Ȗڕϊ��e�[�u��.INI", "CONVERT_2TO1", "KAMOKU_" & strKAMOKU)
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
                Return GetIni("�Ȗڕϊ��e�[�u��.INI", "CONVERT_2TO1", "KAMOKU_" & strKAMOKU)
        End Select
        ' 2016/10/19 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(���o�b�`�A�g���ڒǉ�)) -------------------- END

    End Function
    '2011/06/28 �W���ŏC�� ���U�̏ꍇ�A�Ȗڂ��̑��́A09�ɕύX ------------------START
    Public Function ConvertKamoku2TO1_K(ByVal strKAMOKU As String) As String
        '=====================================================================================
        'NAME           :ConvertKamoku2TO1_K
        'Parameter      :strKAMOKU�F�����R�[�h�i2���j
        'Description    :�K��O����������s��
        'Return         :�ȖڃR�[�h�i1���j
        'Create         :2011/06/28
        'Update         :
        '=====================================================================================

        ' 2016/10/19 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(���o�b�`�A�g���ڒǉ�)) -------------------- START
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
        Select Case GetIni("�Ȗڕϊ��e�[�u��.INI", "CONVERT_2TO1_K", "KAMOKU_" & strKAMOKU)
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
                Return GetIni("�Ȗڕϊ��e�[�u��.INI", "CONVERT_2TO1_K", "KAMOKU_" & strKAMOKU)
        End Select
        ' 2016/10/19 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(���o�b�`�A�g���ڒǉ�)) -------------------- END

    End Function
    '2011/06/28 �W���ŏC�� ���U�̏ꍇ�A�Ȗڂ��̑��́A09�ɕύX ------------------END
    '
    ' �@�\�@ �F �����񂩂�C�w��̒�����؂���
    '
    ' �����@ �F ARG1 - ������
    ' �@�@�@ �@ ARG2 - ����
    '
    ' �߂�l �F �؂�������̎c��̕�����
    '
    ' ���l�@ �F
    '
    Public Function Cutting(ByRef value As String, ByVal len As Integer) As String
        Try
            ' �؂��镶����
            Dim ret As String
            Dim bt() As Byte = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes(value)
            ret = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(bt, 0, len)
            ' �؂�������̎c��̕�����
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
            Dim myProcess As System.Diagnostics.Process = System.Diagnostics.Process.GetCurrentProcess '�������g�̃v���Z�X
            Dim AllProcess() As System.Diagnostics.Process = System.Diagnostics.Process.GetProcesses() '�v���Z�X�ꗗ
            Dim ExcessFlg As Boolean = False '�������s�t���O

            '��~���Ă���v���Z�X���
            Dim errId As Integer = 0
            Dim errStartTime As Date = DateTime.Now

            '�v���Z�X����
            For Each targetProcess As System.Diagnostics.Process In AllProcess
                '�����v���Z�X���Ńv���Z�XID���Ⴄ�ꍇ�͓�d�N�����o
                If targetProcess.ProcessName = myProcess.ProcessName AndAlso targetProcess.Id <> myProcess.Id Then
                    '�J�n���Ԃ�������葁���ꍇ
                    If targetProcess.StartTime < myProcess.StartTime Then
                        '�Ώۃv���Z�X�̍ő�ҋ@����
                        Dim MaxWaitDate As DateTime = targetProcess.StartTime.AddMilliseconds(TimeSpan * WaitCount)

                        '�v���Z�X�I������܂Ń��[�v
                        While targetProcess.HasExited = False
                            '�ő�ҋ@���������߂����ꍇ�͎��̃v���Z�X�擾��
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
                '�������s���s��ꂽ�ꍇ�̓C�x���g���O�o�͂���
                Dim ELog As New ClsEventLOG
                ELog.Write("�������s�F���s��PID��" & errId & ", " & _
                            "�J�n�����F" & errStartTime.ToString("yyyy/MM/dd HH:mm:ss"), Diagnostics.EventLogEntryType.Warning)
            End If

        Catch
            '��O�̓X���[
        End Try

        Return cnt
    End Function

    ' 2016/01/22 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
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
    ' 2016/01/22 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

    ' 2016/05/30 �^�X�N�j���� ADD �yRD�zUI_B-14-99(RSV2�Ή�) -------------------- START
    Public Function ExternalSub(ByVal dllName As String, ByVal className As String, ByVal methodName As String, ByVal CmdArgs() As String, ByRef RetValue As String) As Boolean

        Dim DllAsm As Assembly         ' DLL�A�Z���u��
        Dim ClassInstance As Object    ' �N���X�C���X�^���X
        RetValue = ""

        Try

            Try
                DllAsm = System.Reflection.Assembly.LoadFrom(dllName & ".dll")
            Catch ex As Exception
                Return False
            End Try

            ' �N���X���C���X�^���X��
            ClassInstance = DllAsm.CreateInstance(dllName & "." & className)
            If ClassInstance Is Nothing Then
                Return False
            End If

            ' ���\�b�h�ďo��
            ' �Ɩ��ŗL������\�b�h�J�n���O�o��
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

        Dim DllAsm As Assembly         ' DLL�A�Z���u��
        Dim ClassInstance As Object    ' �N���X�C���X�^���X
        RetValue = 0

        Try

            Try
                DllAsm = System.Reflection.Assembly.LoadFrom(dllName & ".dll")
            Catch ex As Exception
                Return False
            End Try

            ' �N���X���C���X�^���X��
            ClassInstance = DllAsm.CreateInstance(dllName & "." & className)
            If ClassInstance Is Nothing Then
                Return False
            End If

            ' ���\�b�h�ďo��
            ' �Ɩ��ŗL������\�b�h�J�n���O�o��
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

        Dim DllAsm As Assembly         ' DLL�A�Z���u��
        Dim ClassInstance As Object    ' �N���X�C���X�^���X
        RetValue = False

        Try

            Try
                DllAsm = System.Reflection.Assembly.LoadFrom(dllName & ".dll")
            Catch ex As Exception
                Return False
            End Try

            ' �N���X���C���X�^���X��
            ClassInstance = DllAsm.CreateInstance(dllName & "." & className)
            If ClassInstance Is Nothing Then
                Return False
            End If

            ' ���\�b�h�ďo��
            ' �Ɩ��ŗL������\�b�h�J�n���O�o��
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
    ' 2016/05/30 �^�X�N�j���� ADD �yRD�zUI_B-14-99(RSV2�Ή�) -------------------- END

End Module
