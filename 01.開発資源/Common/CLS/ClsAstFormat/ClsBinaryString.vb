Option Explicit On 
Option Strict On

Imports System.Text

'�o�C�g�z��𕶎���̂悤�Ɉ����N���X����^
Public NotInheritable Class BinaryString

#Region "�N���X�ϐ�"
    Private MyBuffer() As Byte   '����T�C�Y��0
    Private MyEncode As Encoding '����G���R�[�h��SHIFT-JIS�Ƃ���
#End Region

#Region "�v���p�e�B"
    Default Public Property Item(ByVal index As Integer) As Byte
        Get
            Return MyBuffer(index)
        End Get
        Set(ByVal Value As Byte)
            MyBuffer(index) = Value
        End Set
    End Property
    Public Property Buffer() As Byte()
        Get
            Return MyBuffer
        End Get
        Set(ByVal Value() As Byte)
            If Value Is Nothing Then
                MyBuffer = New Byte() {}
            Else
                MyBuffer = Value
            End If
        End Set
    End Property
    Public Property Encode() As Encoding
        Get
            Return MyEncode
        End Get
        Set(ByVal Value As Encoding)
            If Value Is Nothing Then
                MyEncode = Encoding.GetEncoding(932)
            Else
                MyEncode = Value
            End If
        End Set
    End Property
    Public ReadOnly Property Length() As Integer
        Get
            Return MyBuffer.Length()
        End Get
    End Property
#End Region

#Region "�R���X�g���N�^"
    Public Sub New()
        Buffer = New Byte() {}
        MyEncode = Encoding.GetEncoding(932)
    End Sub
    Public Sub New(ByVal count As Integer)
        MyBuffer = New Byte(count - 1) {}
        MyEncode = Encoding.GetEncoding(932)
    End Sub
    Public Sub New(ByVal count As Integer, ByVal encode As Encoding)
        MyBuffer = New Byte(count - 1) {}
        MyEncode = encode
    End Sub
    Public Sub New(ByVal buffer() As Byte)
        MyClass.Buffer = buffer
        MyEncode = Encoding.GetEncoding(932)
    End Sub
    Public Sub New(ByVal buffer() As Byte, ByVal encode As Encoding)
        MyClass.Buffer = buffer
        MyClass.Encode = encode
    End Sub
    Public Sub New(ByVal value As String)
        MyEncode = Encoding.GetEncoding(932)
        If value Is Nothing Then
            MyBuffer = New Byte() {}
        Else
            MyBuffer = MyEncode.GetBytes(value)
        End If
    End Sub
    Public Sub New(ByVal value As String, ByVal encode As Encoding)
        MyClass.Encode = encode
        If value Is Nothing Then
            MyBuffer = New Byte() {}
        Else
            MyBuffer = MyEncode.GetBytes(value)
        End If
    End Sub
#End Region

#Region "���\�b�h"
    Public Sub Clear()
        MyBuffer = New Byte() {}
    End Sub
    '�o�C�g�z���؂�o��
    Public Function GetBytes(ByVal index As Integer, ByVal count As Integer) As Byte()
        Dim buf(count - 1) As Byte
        System.Buffer.BlockCopy(MyBuffer, index, buf, 0, count)
        Return buf
    End Function
    '�������؂�o��
    Public Function Substring(ByVal startIndex As Integer) As String
        Return Encode.GetString(MyBuffer, startIndex, MyBuffer.Length - startIndex)
    End Function
    Public Function Substring(ByVal startIndex As Integer, ByVal length As Integer) As String
        Return Encode.GetString(MyBuffer, startIndex, MyBuffer.Length)
    End Function
    '�w�肵���o�C�g�z���}������
    Public Function Insert(ByVal index As Integer, ByRef value() As Byte) As Byte()
        System.Buffer.BlockCopy(value, 0, MyBuffer, index, value.Length)
        Return MyBuffer
    End Function
    '�w�肵���������}������
    Public Function Insert(ByVal index As Integer, ByRef value As String) As Byte()
        Dim buf() As Byte = MyEncode.GetBytes(value)
        System.Buffer.BlockCopy(buf, 0, MyBuffer, index, buf.Length)
        Return MyBuffer
    End Function
    '�������Ԃ�
    Public Overloads Overrides Function ToString() As String
        Return Encode.GetString(MyBuffer)
    End Function
#End Region

End Class
