Imports System
Imports CASTCommon.ModPublic

Public Class ClsFormatSSCKekka
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 210

#Region "�w�b�_���R�[�h"
    Public Structure T_HEAD
        Implements CFormat.IFormat

        Dim DATA_KBN As String          ' �f�[�^�敪
        Dim DATA_CODE As String         ' �f�[�^�R�[�h
        Dim CODE_KBN As String          ' �R�[�h�敪
        Dim DUMMY As String             ' �_�~�[
        Dim CYCLE_NO As String          ' �T�C�N���ԍ�
        Dim KURIKOSI As String          ' �J�z�\��
        Dim SOUSINBI As String          ' ���M��
        Dim KIN_NO As String            ' �d�����Z�@�փR�[�h
        Dim DUMMY2 As String            ' �_�~�[

        Public Property Data() As String Implements CFormat.IFormat.Data
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(DATA_KBN, 1), _
                    SubData(DATA_CODE, 4), _
                    SubData(CODE_KBN, 1), _
                    SubData(DUMMY, 1), _
                    SubData(CYCLE_NO, 2), _
                    SubData(KURIKOSI, 1), _
                    SubData(SOUSINBI, 4), _
                    SubData(KIN_NO, 4), _
                    SubData(DUMMY2, 192) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                DATA_KBN = CuttingData(Value, 1)
                DATA_CODE = CuttingData(Value, 4)
                CODE_KBN = CuttingData(Value, 1)
                DUMMY = CuttingData(Value, 1)
                CYCLE_NO = CuttingData(Value, 2)
                KURIKOSI = CuttingData(Value, 1)
                SOUSINBI = CuttingData(Value, 4)
                KIN_NO = CuttingData(Value, 4)
                DUMMY2 = CuttingData(Value, 192)
            End Set
        End Property
    End Structure
#End Region

#Region "�f�[�^���R�[�h"
    Public Structure T_DATA
        Implements CFormat.IFormat

        Dim DATA_KBN As String          ' �f�[�^�敪
        Dim FILENAME As String          ' �t�@�C����
        Dim ERROR_FILENO As String      ' �G���[�t�@�C���ԍ�
        Dim DATA_RECORD As String       ' �f�[�^���R�[�h
        Dim CYCLE_NO As String          ' �T�C�N���ԍ�
        Dim ERR_CODE() As String        ' �G���[�R�[�h�P�`�Q�O
        Dim ERR_RECO() As String        ' �G���[���R�[�h�ԍ��P�`�Q�O
        Dim DUMMY As String             ' �_�~�[

        ' ������
        Public Sub Init()
            ERR_CODE = CType(Array.CreateInstance(GetType(String), 20), String())
            ERR_RECO = CType(Array.CreateInstance(GetType(String), 20), String())
        End Sub

        Public Property Data() As String Implements CFormat.IFormat.Data
            ' �f�[�^�擾
            Get
                Dim RetString As String
                RetString = String.Concat(New String() _
                    { _
                    SubData(DATA_KBN, 1), _
                    SubData(FILENAME, 12), _
                    SubData(ERROR_FILENO, 2), _
                    SubData(DATA_RECORD, 4), _
                    SubData(CYCLE_NO, 2) _
                    })

                For i As Integer = 0 To 19
                    RetString &= SubData(ERR_CODE(i), 2)
                    RetString &= SubData(ERR_RECO(i), 2)
                Next i

                RetString &= SubData(DUMMY, 49)

                Return RetString
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                DATA_KBN = CuttingData(Value, 1)
                FILENAME = CuttingData(Value, 12)
                ERROR_FILENO = CuttingData(Value, 2)
                DATA_RECORD = CuttingData(Value, 4)
                CYCLE_NO = CuttingData(Value, 2)
                For i As Integer = 0 To 19
                    ERR_CODE(i) = CuttingData(Value, 2)
                    ERR_RECO(i) = CuttingData(Value, 5)
                Next i
                DUMMY = CuttingData(Value, 49)
            End Set
        End Property
    End Structure
#End Region

#Region "�g���[�����R�[�h"
    Public Structure T_TRAILER
        Implements CFormat.IFormat

        Dim DATA_KBN As String          ' �f�[�^�敪
        Dim TOTAL_KEN As String         ' ���v����
        Dim ERROR_TOTAL_KEN As String   ' �G���[���v����
        Dim DUMMY As String             ' �_�~�[

        Public Property Data() As String Implements CFormat.IFormat.Data
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(DATA_KBN, 1), _
                    SubData(TOTAL_KEN, 6), _
                    SubData(ERROR_TOTAL_KEN, 6), _
                    SubData(DUMMY, 197) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                DATA_KBN = CuttingData(Value, 1)
                TOTAL_KEN = CuttingData(Value, 6)
                ERROR_TOTAL_KEN = CuttingData(Value, 6)
                DUMMY = CuttingData(Value, 197)
            End Set
        End Property
    End Structure
#End Region

#Region "�G���h���R�[�h"
    Public Structure T_END
        Implements CFormat.IFormat

        Dim DATA_KBN As String          ' �f�[�^�敪
        Dim DUMMY As String             ' �_�~�[

        Public Property Data() As String Implements CFormat.IFormat.Data
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(DATA_KBN, 1), _
                    SubData(DUMMY, 209) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                DATA_KBN = CuttingData(Value, 1)
                DUMMY = CuttingData(Value, 209)
            End Set
        End Property
    End Structure
#End Region

End Class
