Imports System
Imports System.IO
Imports System.Text
'*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
Imports System.Diagnostics
'*** End Add 2015/12/01 SO)�r�� for ���O���� ***

' �b�r�u�N���X
Public Class CSVBase
    ' �t�@�C��
    Private IOFile As FileStream
    Public Message As String
    Public FileName As String

    Private e As Encoding = Encoding.GetEncoding("SHIFT-JIS")
    Private btCrLf() As Byte = {13, 10}
    Private btComma() As Byte = {44}

    Public Property Encoding() As Encoding
        Get
            Return e
        End Get
        Set(ByVal value As Encoding)
            e = value
        End Set
    End Property

    ' New
    Public Sub New()
    End Sub

    ' New
    Public Sub New(ByVal file As String)
        FileName = file
    End Sub

    ' �t�@�C�����J��
    Public Overridable Function Open(Optional ByVal iomode As System.IO.FileMode = FileMode.CreateNew) As Integer
        Try
            IOFile = New FileStream(FileName, iomode, FileAccess.Write, FileShare.ReadWrite)
        Catch ex As Exception
            Message = ex.Message
            Return 0
        End Try

        Return 1
    End Function

    ' �t�@�C�����J��
    Public Overridable Function Open(ByVal filename As String, Optional ByVal overwrite As Boolean = False) As Integer
        Try
            If overwrite = True Then
                IOFile = New FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite)
            Else
                IOFile = New FileStream(filename, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite)
            End If
        Catch ex As Exception
            Message = ex.Message
            Return 0
        End Try

        Return 1
    End Function

    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
    ' �@�\�@ �F �r�����[�h�Ńt�@�C�����J��
    ' �����@ �F iomode     �FI/O���[�h
    '           retryCount �F�r���G���[���̃��g���C��
    '           waitTime   �F�r���G���[���̃��g���C�҂����ԁi�~���b�j
    ' �߂�l �F 1: ����I��
    '           0: �ُ�I��
    ' ���l�@ �F �r���G���[���̓��g���C����
    '           �X���b�h�r���͌ďo�����ł��s��
    '
    Public Overridable Function OpenLock(ByVal iomode As System.IO.FileMode, ByVal retryCount As Integer, ByVal waitTime As Integer) As Integer

        For i As Integer = 1 To retryCount
            Try
                ' �r���I�[�v��
                IOFile = New FileStream(FileName, iomode, FileAccess.Write, FileShare.None)
                Return 1

            Catch ex As Exception
                System.Threading.Thread.Sleep(waitTime)

            End Try
        Next

        ' ���g���C�I�[�o�[�Ȃ̂ŁA�t�@�C�����Ƀv���Z�XID��t�����t�@�C�����I�[�v������
        Try
            FileName = FileName.Replace(".LOG", "." & Process.GetCurrentProcess.Id & ".LOG")
            IOFile = New FileStream(FileName, iomode, FileAccess.Write, FileShare.None)
            Return 1

        Catch ex As Exception
            Message = ex.Message
            Return 0
        End Try

    End Function
    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***


    ' Output
    Public Function Output() As Integer
        Try
            IOFile.Write(btCrLf, 0, btCrLf.Length)
        Catch ex As Exception
            Message = ex.Message
            Return 0
        End Try

        Return 1
    End Function

    ' Output
    Public Function Output(ByVal data As Long, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False) As Integer
        Dim sData As String
        Try
            sData = data.ToString
        Catch ex As Exception
            sData = "0"
        End Try
        Return Output(sData, dq, crlf)
    End Function

    ' Output
    Public Function Output(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False) As Integer
        Try
            If data Is Nothing Then
                data = ""
            End If
            If dq = True Then
                data = EncloseValue(data)
            End If
            Dim bt() As Byte = e.GetBytes(data)
            IOFile.Write(bt, 0, bt.Length)

            If crlf = True Then
                IOFile.Write(btCrLf, 0, btCrLf.Length)
            Else
                IOFile.Write(btComma, 0, btComma.Length)
            End If
        Catch ex As Exception
            Message = ex.Message
            Return 0
        End Try

        Return 1
    End Function

    ' Output
    Public Function OutputPlus(ByVal ParamArray data() As String) As Integer
        For i As Integer = 0 To data.Length - 1
            data(i) = EncloseValue(data(i))
        Next i
        Return Output(data)
    End Function

    ' Output
    Public Function Output(ByVal ParamArray data() As String) As Integer
        Try
            Dim sJoin As String = String.Join(",", data)
            Dim bt() As Byte = e.GetBytes(sJoin)
            IOFile.Write(bt, 0, bt.Length)

            IOFile.Write(btCrLf, 0, 2)
        Catch ex As Exception
            Message = ex.Message
            Return 0
        End Try

        Return 1
    End Function

    ' Close
    Public Function Close() As Integer
        Try
            If Not IOFile Is Nothing Then
                IOFile.Close()
            End If
        Catch ex As Exception

        End Try
    End Function
End Class
