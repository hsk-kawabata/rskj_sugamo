Imports System
Imports System.IO
Imports System.Text
Imports System.Collections

' �^���L���O�w�b�_�t�H�[�}�b�g
Public Class ClsT_RIENT10

    ' SHIT-JIS�G���R�[�f�B���O
    Protected Shared EncdJ As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")

    ' �Z�p���[�^
    Protected Shared SEPARATE As String = Microsoft.VisualBasic.ChrW(&H1E)

    ' �Œ蒷�\���̗p�C���^�[�t�F�[�X
    Protected Interface IFormat
        ' �f�[�^
        Sub Init_10()
        ReadOnly Property Data_10() As Byte()
        Sub Init_48()
        ReadOnly Property Data_48() As Byte()
    End Interface

#Region "�^���L���O�w�b�_��"
    '�@�^���L���O�w�b�_��
    Public Structure T_RIENT10T
        Implements ClsT_RIENT10.IFormat

        Dim bytDATA_LENGTH() As Byte     '�f�[�^��(1)
        Dim bytDATA_SIKIBETU As Byte     '�f�[�^���� 
        Dim bytDATA_KBN As Byte          '�f�[�^�敪
        Dim bytKINGAKU_SEPA() As Byte     '���z�Z�p���[�^(1)
        Dim strSOFT_NO1 As String        '�\�t�g�@��1
        Dim bytTANMATU As Byte           '�[������   
        Dim bytKINKO_CODE() As Byte      '���ɃR�[�h(3)
        Dim bytTENPO_CODE() As Byte      '�X�܃R�[�h(2)  
        Dim bytYOBI1 As Byte             '�\��1 
        Dim bytTANKING_NO() As Byte       '�^���L���O�A��(1) 
        Dim strOPE_CODE As String        '�I�y�R�[�h   
        Dim bytYOBI2 As Byte             '�\��2    
        Dim bytNEXT_DATA_ADD() As Byte   '���f�[�^�A�h���X(3)
        Dim bytFIN_FLG As Byte           '�I���t���O 
        Dim bytYOBI3() As Byte           '�\��3(2)  
        '�e�L�X�g�w�b�_
        Dim bytDENBUN_JOKEN As Byte      '�d������ 
        Dim bytTUUBAN() As Byte          '�ʔ�(3) 
        Dim bytNYURYOKU() As Byte        '���͏���(2)   
        Dim bytOPERATE_KEY As Byte       '�I�y���[�^�L�[  
        Dim bytBAITAI_SET As Byte        '�}�̃Z�b�g���  
        Dim bytYAKUSEKI As Byte          '��ȃL�[   
        Dim bytTORIHIKI_MODE As Byte     '������[�h�L�[ 
        Dim strKAMOKU_CODE As String     '�Ȗڃ��[�h
        Dim strTORIHIKI_CODE As String   '����R�[�h
        Dim strKAWASESOFTNO As String    '�\�t�g�@�ԁi�ב�TEXT�̂݁j
        Dim bytKAWASETUUBAN() As Byte     '�ב֒ʔԁi�ב�TEXT�̂݁j
        Dim strSEPARATE_1 As String      '��1�Z�p���[�^  
        Dim strKINTEN_CD As String        '���X�R�[�h     
        Dim strSEPARATE_2 As String      '��2�Z�p���[�^    
        '�����ڊǓǑփf�[�^
        Dim strTANKING_DATA As String    '�^���L���O�f�[�^ 

        Public Sub Init_10() Implements IFormat.Init_10
            bytDATA_LENGTH = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytDATA_LENGTH(0) = 0
            bytDATA_LENGTH(1) = 224

            bytDATA_SIKIBETU = 1
            bytDATA_KBN = 0

            bytKINGAKU_SEPA = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytKINGAKU_SEPA(0) = 0
            bytKINGAKU_SEPA(1) = 0

            strSOFT_NO1 = "V"
            bytTANMATU = 128

            bytKINKO_CODE = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytKINKO_CODE(0) = 0
            bytKINKO_CODE(1) = 0
            bytKINKO_CODE(2) = 0
            bytKINKO_CODE(3) = 0

            bytTENPO_CODE = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytTENPO_CODE(0) = 0
            bytTENPO_CODE(1) = 0
            bytTENPO_CODE(2) = 0

            bytYOBI1 = 0

            bytTANKING_NO = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytTANKING_NO(0) = 0
            bytTANKING_NO(1) = 0

            bytYOBI2 = 0

            bytNEXT_DATA_ADD = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytNEXT_DATA_ADD(0) = 0
            bytNEXT_DATA_ADD(1) = 0
            bytNEXT_DATA_ADD(2) = 0
            bytNEXT_DATA_ADD(3) = 0

            bytFIN_FLG = 0

            bytYOBI3 = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytYOBI3(0) = 0
            bytYOBI3(1) = 0
            bytYOBI3(2) = 0

            bytDENBUN_JOKEN = 1

            bytTUUBAN = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytTUUBAN(0) = 0
            bytTUUBAN(1) = 0
            bytTUUBAN(2) = 0
            bytTUUBAN(3) = 0

            bytNYURYOKU = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytNYURYOKU(0) = 0
            bytNYURYOKU(1) = 0
            bytNYURYOKU(2) = 0

            bytOPERATE_KEY = 1
            bytBAITAI_SET = 3
            bytYAKUSEKI = 0
            bytTORIHIKI_MODE = 0
            strSEPARATE_1 = SEPARATE
            strKINTEN_CD = New String(" "c, 7)
            strSEPARATE_2 = SEPARATE

            strTANKING_DATA = New String(" "c, 198)
        End Sub

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public ReadOnly Property Data_10() As Byte() Implements IFormat.Data_10
            Get
                Dim ret(511) As Byte

                Array.Copy(bytDATA_LENGTH, 0, ret, 0, 2)
                ret(2) = bytDATA_SIKIBETU
                ret(3) = bytDATA_KBN
                Array.Copy(bytKINGAKU_SEPA, 0, ret, 4, 2)
                Array.Copy(EncdJ.GetBytes(SubData(strSOFT_NO1, 1)), 0, ret, 6, 1)
                ret(7) = bytTANMATU
                Array.Copy(bytKINKO_CODE, 0, ret, 8, 4)
                Array.Copy(bytTENPO_CODE, 0, ret, 12, 3)
                ret(15) = bytYOBI1
                Array.Copy(bytTANKING_NO, 0, ret, 16, 2)
                Array.Copy(EncdJ.GetBytes(SubData(strOPE_CODE, 5)), 0, ret, 18, 5)
                ret(23) = bytYOBI2
                Array.Copy(bytNEXT_DATA_ADD, 0, ret, 24, 4)
                ret(28) = bytFIN_FLG
                Array.Copy(bytYOBI3, 0, ret, 29, 3)
                ret(32) = bytDENBUN_JOKEN
                Array.Copy(bytTUUBAN, 0, ret, 33, 4)
                Array.Copy(bytNYURYOKU, 0, ret, 37, 3)
                ret(40) = bytOPERATE_KEY
                ret(41) = bytBAITAI_SET
                ret(42) = bytYAKUSEKI
                ret(43) = bytTORIHIKI_MODE
                Array.Copy(EncdJ.GetBytes(SubData(strKAMOKU_CODE, 2)), 0, ret, 44, 2)
                Array.Copy(EncdJ.GetBytes(SubData(strTORIHIKI_CODE, 3)), 0, ret, 46, 3)
                Array.Copy(EncdJ.GetBytes(SubData(strSEPARATE_1, 1)), 0, ret, 49, 1)
                Array.Copy(EncdJ.GetBytes(SubData(strKINTEN_CD, 7)), 0, ret, 50, 7)
                Array.Copy(EncdJ.GetBytes(SubData(strSEPARATE_2, 1)), 0, ret, 57, 1)
                Array.Copy(EncdJ.GetBytes(SubData(strTANKING_DATA, 198)), 0, ret, 58, 198)

                '***�C���@20080926************************
                '113�o�C�g�ڈȍ~��NULL�Ŗ��߂遨���Ԓ�����138
                '58(�^���L���O�f�[�^�w�b�_��) + �^���L���O�f�[�^��
                For i As Integer = 58 + strTANKING_DATA.Length To ret.Length - 1
                    ret(i) = 0
                Next i
                '*****************************************

                Return ret
            End Get
        End Property

        Public Sub Init_48() Implements IFormat.Init_48
            bytDATA_LENGTH = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytDATA_LENGTH(0) = 0
            bytDATA_LENGTH(1) = 224

            bytDATA_SIKIBETU = 2
            bytDATA_KBN = 0

            bytKINGAKU_SEPA = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytKINGAKU_SEPA(0) = 0
            bytKINGAKU_SEPA(1) = 0

            strSOFT_NO1 = "V"
            bytTANMATU = 128

            bytKINKO_CODE = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytKINKO_CODE(0) = 0
            bytKINKO_CODE(1) = 0
            bytKINKO_CODE(2) = 0
            bytKINKO_CODE(3) = 0

            bytTENPO_CODE = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytTENPO_CODE(0) = 0
            bytTENPO_CODE(1) = 0
            bytTENPO_CODE(2) = 0

            bytYOBI1 = 0

            bytTANKING_NO = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytTANKING_NO(0) = 0
            bytTANKING_NO(1) = 0

            bytYOBI2 = 0

            bytNEXT_DATA_ADD = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytNEXT_DATA_ADD(0) = 0
            bytNEXT_DATA_ADD(1) = 0
            bytNEXT_DATA_ADD(2) = 0
            bytNEXT_DATA_ADD(3) = 0

            bytFIN_FLG = 0

            bytYOBI3 = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytYOBI3(0) = 0
            bytYOBI3(1) = 0
            bytYOBI3(2) = 0

            bytDENBUN_JOKEN = 2

            bytTUUBAN = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytTUUBAN(0) = 0
            bytTUUBAN(1) = 0
            bytTUUBAN(2) = 0
            bytTUUBAN(3) = 0

            bytNYURYOKU = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytNYURYOKU(0) = 0
            bytNYURYOKU(1) = 0
            bytNYURYOKU(2) = 0

            bytOPERATE_KEY = 1
            bytBAITAI_SET = 3
            bytYAKUSEKI = 0
            bytTORIHIKI_MODE = 0

            strKAWASESOFTNO = "V"   '�\�t�g�@�ԁi�ב�TEXT�̂݁j
            bytKAWASETUUBAN = CType(Array.CreateInstance(GetType(Byte), 5), Byte()) '�ב֒ʔԁi�ב�TEXT�̂݁j
            bytKAWASETUUBAN(0) = 0
            bytKAWASETUUBAN(1) = 0
            bytKAWASETUUBAN(2) = 0
            bytKAWASETUUBAN(3) = 0
            bytKAWASETUUBAN(4) = 0

            strSEPARATE_1 = SEPARATE
            strKINTEN_CD = New String(" "c, 7)
            strSEPARATE_2 = SEPARATE

            strTANKING_DATA = New String(" "c, 198)
        End Sub

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public ReadOnly Property Data_48() As Byte() Implements IFormat.Data_48
            Get
                Dim ret(511) As Byte

                Array.Copy(bytDATA_LENGTH, 0, ret, 0, 2)
                ret(2) = bytDATA_SIKIBETU
                ret(3) = bytDATA_KBN
                Array.Copy(bytKINGAKU_SEPA, 0, ret, 4, 2)
                Array.Copy(EncdJ.GetBytes(SubData(strSOFT_NO1, 1)), 0, ret, 6, 1)
                ret(7) = bytTANMATU
                Array.Copy(bytKINKO_CODE, 0, ret, 8, 4)
                Array.Copy(bytTENPO_CODE, 0, ret, 12, 3)
                ret(15) = bytYOBI1
                Array.Copy(bytTANKING_NO, 0, ret, 16, 2)
                Array.Copy(EncdJ.GetBytes(SubData(strOPE_CODE, 5)), 0, ret, 18, 5)
                ret(23) = bytYOBI2
                Array.Copy(bytNEXT_DATA_ADD, 0, ret, 24, 4)
                ret(28) = bytFIN_FLG
                Array.Copy(bytYOBI3, 0, ret, 29, 3)
                ret(32) = bytDENBUN_JOKEN
                Array.Copy(bytTUUBAN, 0, ret, 33, 4)
                Array.Copy(bytNYURYOKU, 0, ret, 37, 3)
                ret(40) = bytOPERATE_KEY
                ret(41) = bytBAITAI_SET
                ret(42) = bytYAKUSEKI
                ret(43) = bytTORIHIKI_MODE
                Array.Copy(EncdJ.GetBytes(SubData(strKAMOKU_CODE, 2)), 0, ret, 44, 2)
                Array.Copy(EncdJ.GetBytes(SubData(strTORIHIKI_CODE, 3)), 0, ret, 46, 3)
                Array.Copy(EncdJ.GetBytes(SubData(strSOFT_NO1, 1)), 0, ret, 49, 1)
                Array.Copy(bytKAWASETUUBAN, 0, ret, 50, 5)
                Array.Copy(EncdJ.GetBytes(SubData(strSEPARATE_1, 1)), 0, ret, 55, 1)
                Array.Copy(EncdJ.GetBytes(SubData(strKINTEN_CD, 7)), 0, ret, 56, 7)
                Array.Copy(EncdJ.GetBytes(SubData(strSEPARATE_2, 1)), 0, ret, 63, 1)
                Array.Copy(EncdJ.GetBytes(SubData(strTANKING_DATA, 198)), 0, ret, 64, 198)

                '***�C���@20080926************************
                For i As Integer = 64 + strTANKING_DATA.Length To ret.Length - 1
                    ret(i) = 0
                Next i
                '*****************************************

                Return ret
            End Get
        End Property

    End Structure
#End Region

#Region "�ŏI���R�[�h"
    '�@�ŏI���R�[�h
    Public Structure T_RIENT10L
        Implements ClsT_RIENT10.IFormat

        Private bytBYTE_DATA() As Byte           '�o�C�g�f�[�^(511)

        Public Sub Init() Implements IFormat.Init_10, IFormat.Init_48
            bytBYTE_DATA = CType(Array.CreateInstance(GetType(Byte), 512), Byte())
            bytBYTE_DATA.Initialize()

            bytBYTE_DATA(0) = 255
            bytBYTE_DATA(1) = 255
            'bytBYTE_DATA(48) = 30
            'bytBYTE_DATA(56) = 30
        End Sub

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public ReadOnly Property Data() As Byte() Implements IFormat.Data_10, IFormat.Data_48
            Get
                Dim ret(511) As Byte

                Array.Copy(bytBYTE_DATA, 0, ret, 0, 512)

                Return ret
            End Get
        End Property
    End Structure
#End Region

    ' �^���L���O�f�[�^
    Public TANKING_DATA As T_RIENT10T

    ' �^���L���O�ŏI
    Public TANKING_LAST As T_RIENT10L

    '
    ' �@�\�@ �F �����񂩂�C�w��̒�����؂���
    '
    ' �����@ �F ARG1 - ������
    ' �@�@�@ �@ ARG2 - ����
    '           ARG3 - �O�F���l�C�P�F�E�l
    '           ARG4 - ���ߕ���
    '
    ' �߂�l �F �؂�������̎c��̕�����
    '
    ' ���l�@ �F
    '
    Protected Shared Function SubData(ByVal value As String, ByVal len As Integer, _
                    Optional ByVal align As Integer = 0, Optional ByVal pad As Char = " "c) As String
        Try
            If len = 0 Then
                Return ""
            End If

            ' �؂��镶����
            If align = 0 Then
                ' ���l
                value = value.PadRight(len, pad)
            Else
                ' �E�l
                value = value.PadLeft(len, pad)
            End If

            ' �؂��镶����
            Dim bt() As Byte = EncdJ.GetBytes(value)
            Return EncdJ.GetString(bt, 0, len)
        Catch ex As Exception
            Return New String(" "c, len)
        End Try
    End Function

End Class
