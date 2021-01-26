Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

' �m�s�s�����U�փf�[�^�t�H�[�}�b�g�N���X
Public Class CFormatNTTCMT
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 180

    '------------------------------------------
    '�m�s�s�t�H�[�}�b�g
    '------------------------------------------
    '�w�b�_���R�[�h
    Public Structure NTRECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NT1 As String     ' �f�[�^�敪
        <VBFixedString(2)> Public NT2 As String     ' ��ʃR�[�h
        <VBFixedString(4)> Public NT3 As String     ' ������s�ԍ�
        <VBFixedString(4)> Public NT4 As String     ' �����N��
        <VBFixedString(1)> Public NT5 As String     ' �Q
        <VBFixedString(6)> Public NT6 As String     ' ���x������
        <VBFixedString(6)> Public NT7 As String     ' �ĐU�֓�
        <VBFixedString(3)> Public NT8 As String     ' �����Z���^�R�[�h
        <VBFixedString(40)> Public NT9 As String    ' �����Z���^��
        <VBFixedString(4)> Public NT10 As String    ' �����s�ԍ�
        <VBFixedString(3)> Public NT11 As String    ' ����X�ܔԍ�
        <VBFixedString(15)> Public NT12 As String   ' �����s��
        <VBFixedString(1)> Public NT13 As String    ' �a����ځi�ϑ��ҁj
        <VBFixedString(7)> Public NT14 As String    ' �����ԍ��i�ϑ��ҁj
        <VBFixedString(6)> Public NT15 As String    ' �U�֓�
        <VBFixedString(77)> Public NT16 As String   ' �m�s�s�g�p��

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {NT1, NT2, NT3, NT4, NT5, NT6, NT7, _
                            NT8, NT9, NT10, NT11, NT12, NT13, NT14, _
                            NT15, NT16})
            End Get
            Set(ByVal value As String)
                NT1 = CuttingData(value, 1)
                NT2 = CuttingData(value, 2)
                NT3 = CuttingData(value, 4)
                NT4 = CuttingData(value, 4)
                NT5 = CuttingData(value, 1)
                NT6 = CuttingData(value, 6)
                NT7 = CuttingData(value, 6)
                NT8 = CuttingData(value, 3)
                NT9 = CuttingData(value, 40)
                NT10 = CuttingData(value, 4)
                NT11 = CuttingData(value, 3)
                NT12 = CuttingData(value, 15)
                NT13 = CuttingData(value, 1)
                NT14 = CuttingData(value, 7)
                NT15 = CuttingData(value, 6)
                NT16 = CuttingData(value, 77)
            End Set
        End Property
    End Structure
    Public NTT_REC1 As NTRECORD1

    '�f�[�^���R�[�h
    Structure NTRECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NT1 As String      ' �f�[�^�敪
        <VBFixedString(4)> Public NT2 As String      ' �����ʒm�R�[�h
        <VBFixedString(4)> Public NT3 As String      ' �U�֒ʒm�R�[�h
        <VBFixedString(5)> Public NT4 As String      ' �s�O�ǔ�
        <VBFixedString(4)> Public NT5 As String      ' �s���ǔ�
        <VBFixedString(4)> Public NT6 As String      ' �d�b�ԍ�
        <VBFixedString(1)> Public NT7 As String      ' �����ԍ�
        <VBFixedString(1)> Public NT8 As String      ' �����Ώۈ�
        <VBFixedString(2)> Public NT9 As String      ' ���t�敪
        <VBFixedString(4)> Public NT10 As String     ' ��s�ԍ�
        <VBFixedString(3)> Public NT11 As String     ' �X�ܔԍ�
        <VBFixedString(1)> Public NT12 As String     ' �a�����
        <VBFixedString(7)> Public NT13 As String     ' �����ԍ�
        <VBFixedString(2)> Public NT14 As String     ' �U�֗v�ۃR�[�h
        <VBFixedString(2)> Public NT15 As String     ' �ٓ����		
        <VBFixedString(1)> Public NT16 As String     ' �����̎�����]��
        <VBFixedString(1)> Public NT17 As String     ' �U�֌��ʈ�		
        <VBFixedString(10)> Public NT18 As String    ' �����z			
        <VBFixedString(13)> Public NT19 As String    ' ���d�b�ԍ�		
        <VBFixedString(40)> Public NT20 As String    ' �������`		
        <VBFixedString(1)> Public NT21 As String     ' NTT�g�p��		
        <VBFixedString(16)> Public NT22 As String    ' �ǖ�			
        <VBFixedString(16)> Public NT23 As String    ' �d�b�戵�ǖ�	
        <VBFixedString(1)> Public NT24 As String     ' �^���ǔԈ�		
        <VBFixedString(32)> Public NT25 As String    ' ��			
        <VBFixedString(4)> Public NT26 As String     ' �����ʒm�R�[�h	

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {NT1, NT2, NT3, NT4, NT5, NT6, NT7, _
                            NT8, NT9, NT10, NT11, NT12, NT13, NT14, _
                            NT15, NT16, NT17, NT18, NT19, NT20, NT21, _
                            NT22, NT23, NT24, NT25, NT26})
            End Get
            Set(ByVal Value As String)
                NT1 = CuttingData(Value, 1)
                NT2 = CuttingData(Value, 4)
                NT3 = CuttingData(Value, 4)
                NT4 = CuttingData(Value, 5)
                NT5 = CuttingData(Value, 4)
                NT6 = CuttingData(Value, 4)
                NT7 = CuttingData(Value, 1)
                NT8 = CuttingData(Value, 1)
                NT9 = CuttingData(Value, 2)
                NT10 = CuttingData(Value, 4)
                NT11 = CuttingData(Value, 3)
                NT12 = CuttingData(Value, 1)
                NT13 = CuttingData(Value, 7)
                NT14 = CuttingData(Value, 2)
                NT15 = CuttingData(Value, 2)
                NT16 = CuttingData(Value, 1)
                NT17 = CuttingData(Value, 1)
                NT18 = CuttingData(Value, 10)
                NT19 = CuttingData(Value, 13)
                NT20 = CuttingData(Value, 40)
                NT21 = CuttingData(Value, 1)
                NT22 = CuttingData(Value, 16)
                NT23 = CuttingData(Value, 16)
                NT24 = CuttingData(Value, 1)
                NT25 = CuttingData(Value, 32)
                NT26 = CuttingData(Value, 4)
            End Set
        End Property
    End Structure
    Public NTT_REC2 As NTRECORD2

    '�g���[�����R�[�h
    Structure NTRECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NT1 As String     ' �f�[�^�敪				
        <VBFixedString(6)> Public NT2 As String     ' �U�֕�����				
        <VBFixedString(12)> Public NT3 As String    ' �U�֕����z				
        <VBFixedString(6)> Public NT4 As String     ' �U�֕s�\������			
        <VBFixedString(12)> Public NT5 As String    ' �U�֕s�\�����z			
        <VBFixedString(6)> Public NT6 As String     ' �U�֕ۗ�������			
        <VBFixedString(12)> Public NT7 As String    ' �U�֕ۗ������z			
        <VBFixedString(6)> Public NT8 As String     ' �ۗ����̓��U�֕�����	
        <VBFixedString(12)> Public NT9 As String    ' �ۗ����̓��U�֕����z	
        <VBFixedString(6)> Public NT10 As String    ' �ۗ����̓��s�\������
        <VBFixedString(12)> Public NT11 As String   ' �ۗ����̓��s�\�����z
        <VBFixedString(6)> Public NT12 As String    ' �U�ֈ˗����̍��v����
        <VBFixedString(12)> Public NT13 As String   ' �U�ֈ˗����̍��v���z
        <VBFixedString(4)> Public NT14 As String    ' �U�֔ی���			
        <VBFixedString(6)> Public NT15 As String    ' �����̎�����]����	
        <VBFixedString(61)> Public NT16 As String   ' �m�s�s�g�p����	

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {NT1, NT2, NT3, NT4, NT5, NT6, NT7, _
                            NT8, NT9, NT10, NT11, NT12, NT13, NT14, _
                            NT15, NT16})
            End Get
            Set(ByVal Value As String)
                NT1 = CuttingData(Value, 1)
                NT2 = CuttingData(Value, 6)
                NT3 = CuttingData(Value, 12)
                NT4 = CuttingData(Value, 6)
                NT5 = CuttingData(Value, 12)
                NT6 = CuttingData(Value, 6)
                NT7 = CuttingData(Value, 12)
                NT8 = CuttingData(Value, 6)
                NT9 = CuttingData(Value, 12)
                NT10 = CuttingData(Value, 6)
                NT11 = CuttingData(Value, 12)
                NT12 = CuttingData(Value, 6)
                NT13 = CuttingData(Value, 12)
                NT14 = CuttingData(Value, 4)
                NT15 = CuttingData(Value, 6)
                NT16 = CuttingData(Value, 61)
            End Set
        End Property
    End Structure
    Public NTT_REC8 As NTRECORD8

    '�G���h���R�[�h
    Structure NTRECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NT1 As String    ' �f�[�^�敪		
        <VBFixedString(90)> Public NT2 As String    ' �m�s�s�g�p��	
        <VBFixedString(3)> Public NT3 As String    ' �����s		
        <VBFixedString(86)> Public NT4 As String    ' �m�s�s�g�p����

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {NT1, NT2, NT3, NT4})
            End Get
            Set(ByVal Value As String)
                NT1 = CuttingData(Value, 1)
                NT2 = CuttingData(Value, 90)
                NT3 = CuttingData(Value, 3)
                NT4 = CuttingData(Value, 86)
            End Set
        End Property
    End Structure
    Public NTT_REC9 As NTRECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"1", "8", "9"}

        FtranPfile = "NTT.P"

        HeaderKubun = New String() {"1"}
        DataKubun = New String() {"2"}
        TrailerKubun = New String() {"8"}
    End Sub

    '
    ' �@�\�@ �F �K�蕶���`�F�b�N ���@�����u������
    '
    ' �߂�l �F �s�������̈ʒu
    '
    ' ���l�@ �F RepaceString()�֐��ɂĕ����u�������{
    '           �u���Ώە����́C�s�������ɂ͂Ȃ�Ȃ��͂�
    '
    Public Overrides Function CheckRegularString() As Long
        Dim buff As New StringBuilder(RecordLen)

        Select Case RecordData.Substring(0, 1)
            Case "1"        ' �w�b�_���R�[�h
            Case "2"        ' �f�[�^���R�[�h
            Case "8"        ' �g���[��
            Case "9"        ' �G���h
        End Select

        Return MyBase.CheckRegularString()
    End Function

End Class
