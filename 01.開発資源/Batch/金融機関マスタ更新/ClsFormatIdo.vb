Imports System
Imports System.Collections

' ���Z�@�֎x�X�ٓ��ʒm�f�[�^ �t�H�[�}�b�g
Public Class ClsFormatIdo

    ' �Œ蒷�\���̗p�C���^�[�t�F�[�X
    Protected Interface IFormat
        ' �f�[�^
        Sub Init()
        WriteOnly Property Data() As Byte()
    End Interface

    ' SHIT-JIS�G���R�[�f�B���O
    Protected Shared EncdSJIS As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")

#Region "���Z�@�֎x�X�ٓ��ʒm�f�[�^ ���Z�@�փf�[�^��"
    '�@���Z�@�֎x�X�ٓ��ʒm�f�[�^ ���Z�@�փf�[�^��
    Public Structure KinkoFormat
        Implements ClsFormatIdo.IFormat

        Dim DataKubun As String         ' �f�[�^�敪
        Dim DataSyubetu As String       ' �f�[�^���
        Dim KinCode As String           ' ���Z�@�փR�[�h
        Dim KinFukaCode As String       ' ���Z�@�֕t���R�[�h
        Dim DaiKinCode As String        ' ��\���Z�@�փR�[�h
        Dim SeiKinKana As String        ' ���ǋ��Z�@�֖��i�J�i�j
        Dim SeiKinKanji As String       ' ���ǋ��Z�@�֖��i�����j
        Dim RyakuKinKana As String      ' ���̋��Z�@�֖��i�J�i�j
        Dim RyakuKinKanji As String     ' ���̋��Z�@�֖��i�����j
        Dim JISIN() As String           ' �n�k�����n������Z�@�֕\��
        Dim IdoDate As String           ' �ٓ��N����
        Dim IdoJiyuCode As String       ' �ٓ����R�R�[�h
        Dim NewKinCode As String        ' �V���Z�@�փR�[�h
        Dim NewKinFukaCode As String    ' �V���Z�@�֕t���R�[�h
        Dim DeleteDate As String        ' �폜��
        Dim Dummy As String             ' �_�~�[

        Public Sub Init() Implements IFormat.Init
            DataKubun = ""
            DataSyubetu = ""
            KinCode = ""
            KinFukaCode = ""
            DaiKinCode = ""
            SeiKinKana = ""
            SeiKinKanji = ""
            RyakuKinKana = ""
            RyakuKinKanji = ""
            IdoDate = ""
            IdoJiyuCode = ""
            NewKinCode = ""
            NewKinFukaCode = ""
            DeleteDate = ""
            Dummy = ""

            JISIN = New String(9 - 1) {}

        End Sub

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public WriteOnly Property Data() As Byte() Implements IFormat.Data
            Set(ByVal value() As Byte)
                DataKubun = EncdSJIS.GetString(value, 0, 1)
                DataSyubetu = EncdSJIS.GetString(value, 1, 1)
                KinCode = EncdSJIS.GetString(value, 2, 4)
                KinFukaCode = EncdSJIS.GetString(value, 6, 1)
                DaiKinCode = EncdSJIS.GetString(value, 7, 4)
                SeiKinKana = EncdSJIS.GetString(value, 11, 15)

                SeiKinKanji = EncdSJIS.GetString(value, 26, 30)

                RyakuKinKana = EncdSJIS.GetString(value, 56, 15)

                RyakuKinKanji = EncdSJIS.GetString(value, 71, 30)

                For i As Integer = 0 To JISIN.Length - 1
                    JISIN(i) = EncdSJIS.GetString(value, 101 + i, 1)
                Next i
                IdoDate = EncdSJIS.GetString(value, 110, 8)
                IdoJiyuCode = EncdSJIS.GetString(value, 118, 2)
                NewKinCode = EncdSJIS.GetString(value, 120, 4)
                NewKinFukaCode = EncdSJIS.GetString(value, 124, 1)
                DeleteDate = EncdSJIS.GetString(value, 125, 8)
                Dummy = EncdSJIS.GetString(value, 133, 247)
            End Set
        End Property
    End Structure
#End Region

#Region "���Z�@�֎x�X�ٓ��ʒm�f�[�^ �X�܃f�[�^��"
    '�@���Z�@�֎x�X�ٓ��ʒm�f�[�^ �X�܃f�[�^��
    Public Structure TenpoFormat
        Implements ClsFormatIdo.IFormat

        Dim DataKubun As String         ' �f�[�^�敪
        Dim DataSyubetu As String       ' �f�[�^���
        Dim KinCode As String           ' ���Z�@�փR�[�h
        Dim KinFukaCode As String       ' ���Z�@�֕t���R�[�h
        Dim TenCode As String           ' �X�܃R�[�h
        Dim TenFukaCode As String       ' �X�ܕt���R�[�h
        Dim TenKana As String           ' �X�ܖ��i�J�i�j
        Dim TenKanji As String          ' �X�ܖ��i�����j
        Dim SeiHyouji As String         ' ���ǓX���\��
        Dim Yubin As String             ' �X�֔ԍ�
        Dim TenSyozaiKana As String     ' �X�܏��ݒn�i�J�i�j
        Dim TenSyozaiKanji As String    ' �X�܏��ݒn�i�����j
        Dim TegataKoukan As String      ' ��`�������ԍ�
        Dim TelNo As String             ' �d�b�ԍ�
        Dim TenZokusei As String        ' �X�ܑ����\��
        Dim JikoCenter As String        ' ���s�Z���^�\��
        Dim FuriCenter As String        ' �U���Z���^�\��
        Dim SyuCenter As String         ' �W��Z���^�\��
        Dim KawaseCenter As String      ' �בփZ���^�\��
        Dim Daitegai As String          ' ���ΏۊO�\��
        Dim JisinHyoji As String        ' �n�k�����n����X�ܕ\��
        Dim JCBango As String           ' �i�b�ԍ�
        Dim IdoDate As String           ' �ٓ��N����
        Dim IdoJiyuCode As String       ' �ٓ����R�R�[�h
        Dim NewKinCode As String        ' �V���Z�@�փR�[�h
        Dim NewKinFukaCode As String    ' �V���Z�@�֕t���R�[�h
        Dim NewTenCode As String        ' �V�X�܃R�[�h
        Dim NewTenFukaCode As String    ' �V�X�ܕt���R�[�h
        Dim DeleteDate As String        ' �폜��
        Dim TegataKoukanKanji As String ' ������`���������i�����j
        Dim Dummy As String             ' �_�~�[

        Public Sub Init() Implements IFormat.Init
            DataKubun = ""
            DataSyubetu = ""
            KinCode = ""
            KinFukaCode = ""
            TenCode = ""
            TenFukaCode = ""
            TenKana = ""
            TenKanji = ""
            SeiHyouji = ""
            Yubin = ""
            TenSyozaiKana = ""
            TenSyozaiKanji = ""
            TegataKoukan = ""
            TelNo = ""
            TenZokusei = ""
            JikoCenter = ""
            FuriCenter = ""
            SyuCenter = ""
            KawaseCenter = ""
            Daitegai = ""
            JisinHyoji = ""
            JCBango = ""
            IdoDate = ""
            IdoJiyuCode = ""
            NewKinCode = ""
            NewKinFukaCode = ""
            NewTenCode = ""
            NewTenFukaCode = ""
            DeleteDate = ""
            TegataKoukanKanji = ""
            Dummy = ""
        End Sub

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public WriteOnly Property Data() As Byte() Implements IFormat.Data
            Set(ByVal value() As Byte)
                DataKubun = EncdSJIS.GetString(value, 0, 1)
                DataSyubetu = EncdSJIS.GetString(value, 1, 1)
                KinCode = EncdSJIS.GetString(value, 2, 4)
                KinFukaCode = EncdSJIS.GetString(value, 6, 1)
                TenCode = EncdSJIS.GetString(value, 7, 3)
                TenFukaCode = EncdSJIS.GetString(value, 10, 2)
                TenKana = EncdSJIS.GetString(value, 12, 15)

                TenKanji = EncdSJIS.GetString(value, 27, 30)

                SeiHyouji = EncdSJIS.GetString(value, 57, 1)
                Yubin = EncdSJIS.GetString(value, 58, 10)
                TenSyozaiKana = EncdSJIS.GetString(value, 68, 80)

                TenSyozaiKanji = EncdSJIS.GetString(value, 148, 110)

                TegataKoukan = EncdSJIS.GetString(value, 258, 4)
                TelNo = EncdSJIS.GetString(value, 262, 17)
                TenZokusei = EncdSJIS.GetString(value, 279, 1)
                JikoCenter = EncdSJIS.GetString(value, 280, 1)
                FuriCenter = EncdSJIS.GetString(value, 281, 1)
                SyuCenter = EncdSJIS.GetString(value, 282, 1)
                KawaseCenter = EncdSJIS.GetString(value, 283, 1)
                Daitegai = EncdSJIS.GetString(value, 284, 1)
                JisinHyoji = EncdSJIS.GetString(value, 285, 1)
                JCBango = EncdSJIS.GetString(value, 286, 1)
                IdoDate = EncdSJIS.GetString(value, 287, 8)
                IdoJiyuCode = EncdSJIS.GetString(value, 295, 2)
                NewKinCode = EncdSJIS.GetString(value, 297, 4)
                NewKinFukaCode = EncdSJIS.GetString(value, 301, 1)
                NewTenCode = EncdSJIS.GetString(value, 302, 3)
                NewTenFukaCode = EncdSJIS.GetString(value, 305, 2)
                DeleteDate = EncdSJIS.GetString(value, 307, 8)

                TegataKoukanKanji = EncdSJIS.GetString(value, 315, 20)

                Dummy = EncdSJIS.GetString(value, 335, 45)
            End Set
        End Property
    End Structure
#End Region
End Class
