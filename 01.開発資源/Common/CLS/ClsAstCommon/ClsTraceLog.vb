Option Explicit On 
Option Strict On

Imports System
Imports System.IO
Imports System.Diagnostics

'�g���[�X���O�o�͗p�N���X ���V���O���g���N���X����
Public NotInheritable Class TraceLog

#Region "�ϐ�"
    Private Shared Singleton As TraceLog = New TraceLog     '�V���O���g��
    Private Shared FileName As String                       '���O�t�@�C����
    Private Shared HostName As String                       'PC��
    Private Shared PID As Integer                           '�v���Z�X�h�c
    Private Shared Source As String                         '�v���Z�X��
    Private Shared CRLF As String                           '���s�R�[�h
    Private Shared State As FileState                       '�t�@�C�����
    Private Shared Output As FileStream                     '�t�@�C���X�g���[��
    Private Shared Writer As StreamWriter                   '�����X�g���[��

    '�t�@�C����ԗp
    Private Enum FileState
        Close
        Open
        Err
    End Enum
#End Region

#Region "�v���p�e�B"
    Public Shared ReadOnly Property Instance() As TraceLog
        Get
            Return Singleton
        End Get
    End Property
#End Region

#Region "�R���X�g���N�^"
    Private Sub New()
        Try
            '�e����ݒ�
            Dim p As Process = Process.GetCurrentProcess
            HostName = Environment.MachineName
            PID = p.Id
            Source = p.ProcessName
            CRLF = Environment.NewLine
            State = FileState.Close

            '�f�B���N�g������LOG\TRACE\�V�X�e�����t8��
            FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "LOG"), "TRACE\" & DateTime.Today.ToString("yyyyMMdd"))
            If Directory.Exists(FileName) = False Then
                Directory.CreateDirectory(FileName)
            End If
            '�t�@�C�����̓V�X�e������6��.�v���Z�X��.�v���Z�XID4��.LOG
            FileName = Path.Combine(FileName, DateTime.Now.ToString("HHmmss") & "." & Source & "." & p.Id.ToString("0000") & ".LOG")
        Catch
        End Try
    End Sub
#End Region

#Region "���\�b�h"
    Private Shared Function FileOpen() As Boolean
        Try
            Select Case State
                Case FileState.Close
                    '�t�@�C����r�����䖳���ŊJ��
                    Output = New FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
                    '�X�g���[���o�͂�S-JIS�Ƃ���
                    Writer = New StreamWriter(Output, Text.Encoding.GetEncoding(932))
                    Writer.AutoFlush = True
                    State = FileState.Open
                    Return True

                Case FileState.Open
                    Return True

                Case FileState.Err
                    Return False
            End Select
        Catch
            State = FileState.Err
            Return False
        End Try
    End Function

    '�C�x���g���O�o�͊֐�(��O)
    Public Shared Sub ELogWrite(ByVal ex As Exception, Optional ByVal type As EventLogEntryType = EventLogEntryType.Information)
        Try
            EventLog.WriteEntry(Source, _
                "�������ɃV�X�e���G���[���������܂����B" & CRLF & _
                "�^�C���X�^���v�F " & DateTime.Now & "." & DateTime.Now.Millisecond & CRLF & _
                "�R���s���[�^���F " & HostName & CRLF & _
                "�v���Z�X�h�c�F " & PID & CRLF & _
                "�v���Z�X���F " & Source & CRLF & _
                "��O�̎�ށF " & ex.GetType.Name & CRLF & _
                "�G���[���b�Z�[�W�F " & ex.Message & CRLF & _
                ex.StackTrace & CRLF, _
                type)
            '���O�t�@�C���ɂ��o�͂���
            LogWrite(ex)
        Catch ex2 As Exception
            EventLog.WriteEntry(Source, ex2.Message)
        End Try
    End Sub

    '�C�x���g���O�o�͊֐�(�O���v���Z�X)
    Public Shared Sub ELogWrite(ByVal ps As Process, Optional ByVal type As EventLogEntryType = EventLogEntryType.Information)
        Try
            EventLog.WriteEntry(Source, _
                "�������ɃV�X�e���G���[���������܂����B" & CRLF & _
                "�^�C���X�^���v�F " & DateTime.Now & "." & DateTime.Now.Millisecond & CRLF & _
                "�R���s���[�^���F " & HostName & CRLF & _
                "�v���Z�X�h�c�F " & PID & CRLF & _
                "�v���Z�X���F " & Source & CRLF & _
                "��ďo�v���Z�X���F " & ps.StartInfo.FileName & CRLF & _
                "�p�����[�^�F " & ps.StartInfo.Arguments & CRLF & _
                "���^�[���R�[�h�F " & ps.ExitCode & CRLF, _
                type)
            '���O�t�@�C���ɂ��o�͂���
            LogWrite(ps)
        Catch ex As Exception
            EventLog.WriteEntry(Source, ex.Message)
        End Try
    End Sub

    '�C�x���g���O�o�͊֐�(���b�Z�[�W)
    Public Shared Sub ELogWrite(ByVal message As String, Optional ByVal type As EventLogEntryType = EventLogEntryType.Information)
        Try
            EventLog.WriteEntry(Source, _
                "�������ɃV�X�e���G���[���������܂����B" & CRLF & _
                "�^�C���X�^���v�F " & DateTime.Now & "." & DateTime.Now.Millisecond & CRLF & _
                "�R���s���[�^���F " & HostName & CRLF & _
                "�v���Z�X�h�c�F " & PID & CRLF & _
                "�v���Z�X���F " & Source & CRLF & _
                "�G���[���b�Z�[�W�F " & message & CRLF, _
                type)
            '���O�t�@�C���ɂ��o�͂���
            LogWrite(message)
        Catch ex As Exception
            EventLog.WriteEntry(Source, ex.Message)
        End Try
    End Sub

    '���O�t�@�C���o�͊֐�(��O)
    Public Shared Sub LogWrite(ByVal ex As Exception)
        Try
            If FileOpen() = True Then
                Writer.WriteLine("�^�C���X�^���v�F " & DateTime.Now & "." & DateTime.Now.Millisecond & CRLF & _
                    "�R���s���[�^���F " & HostName & CRLF & _
                    "�v���Z�X�h�c�F " & PID & CRLF & _
                    "�v���Z�X���F " & Source & CRLF & _
                    "��O�̎�ށF " & ex.GetType.Name & CRLF & _
                    "�G���[���b�Z�[�W�F " & ex.Message & CRLF & _
                    ex.StackTrace & CRLF)
            End If
        Catch
            '�C�x���g���O�ł��o�͂���ꍇ�A�������[�v�ɂȂ�̂ŃR�����g�A�E�g
            ''���O�t�@�C���ɏo�͏o���Ȃ��ꍇ�̓C�x���g���O�ɏo�͂���
            'ELogWrite(ex)
        End Try
    End Sub

    '���O�t�@�C���o�͊֐�(�O���v���Z�X)
    Public Shared Sub LogWrite(ByVal ps As Process)
        Try
            If FileOpen() = True Then
                Writer.WriteLine("�^�C���X�^���v�F " & DateTime.Now & "." & DateTime.Now.Millisecond & CRLF & _
                    "�R���s���[�^���F " & HostName & CRLF & _
                    "�v���Z�X�h�c�F " & PID & CRLF & _
                    "�v���Z�X���F " & Source & CRLF & _
                    "��ďo�v���Z�X���F " & ps.StartInfo.FileName & CRLF & _
                    "�p�����[�^�F " & ps.StartInfo.Arguments & CRLF & _
                    "���^�[���R�[�h�F " & ps.ExitCode & CRLF)
            End If
        Catch
            '�C�x���g���O�ł��o�͂���ꍇ�A�������[�v�ɂȂ�̂ŃR�����g�A�E�g
            ''���O�t�@�C���ɏo�͏o���Ȃ��ꍇ�̓C�x���g���O�ɏo�͂���
            'ELogWrite(ps)
        End Try
    End Sub

    '���O�t�@�C���o�͊֐�(��O)
    Public Shared Sub LogWrite(ByVal message As String)
        Try
            If FileOpen() = True Then
                Writer.WriteLine("�^�C���X�^���v�F " & DateTime.Now & "." & DateTime.Now.Millisecond & CRLF & _
                    "�R���s���[�^���F " & HostName & CRLF & _
                    "�v���Z�X�h�c�F " & PID & CRLF & _
                    "�v���Z�X���F " & Source & CRLF & _
                    "�G���[���b�Z�[�W�F " & message & CRLF)
            End If
        Catch
            '�C�x���g���O�ł��o�͂���ꍇ�A�������[�v�ɂȂ�̂ŃR�����g�A�E�g
            ''���O�t�@�C���ɏo�͏o���Ȃ��ꍇ�̓C�x���g���O�ɏo�͂���
            'ELogWrite(message)
        End Try
    End Sub
#End Region

#Region "�ߑ�����Ȃ�������O�p"
    Public Shared Function AddErrHandler() As Boolean
        Try
            Dim currentDomain As AppDomain = AppDomain.CurrentDomain
            AddHandler currentDomain.UnhandledException, AddressOf Singleton.ErrHandler
        Catch
            Return False
        End Try

        Return True
    End Function

    Private Sub ErrHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Try
            Dim ex As Exception = DirectCast(e.ExceptionObject, Exception)
            LogWrite(ex)
            ELogWrite(ex, EventLogEntryType.Error)
        Catch
        End Try
    End Sub
#End Region

#Region "�f�X�g���N�^"
    Protected Overrides Sub Finalize()
        Try
            If Not Output Is Nothing Then
                Output.Close()
            End If
            If Not Writer Is Nothing Then
                Writer.Close()
            End If
        Catch
        End Try
        MyBase.Finalize()
    End Sub
#End Region

End Class
