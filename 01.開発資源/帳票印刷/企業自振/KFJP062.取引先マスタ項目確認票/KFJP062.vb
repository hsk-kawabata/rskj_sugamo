Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports MenteCommon.clsCommon
Public Class KFJP062
    Inherits CAstReports.ClsReportBase

    Private Structure strcInshizeiLabelString
        Dim Inshizei1 As String
        Dim Inshizei2 As String
        Dim Inshizei3 As String

        Public Sub Init()
            Inshizei1 = "1���~����"
            Inshizei2 = "1���~�ȏ�3���~����"
            Inshizei3 = "3���~�ȏ�"
        End Sub
    End Structure
    Private InshizeiLabel As strcInshizeiLabelString

#Region "�U���萔��"
    Private Structure strcTesuu
        Dim KijyunIDName As String
        Dim A1 As Long
        Dim A2 As Long
        Dim A3 As Long
        Dim B1 As Long
        Dim B2 As Long
        Dim B3 As Long
        Dim C1 As Long
        Dim C2 As Long
        Dim C3 As Long

        Public Sub Init()
            KijyunIDName = ""
            A1 = 0
            A2 = 0
            A3 = 0
            B1 = 0
            B2 = 0
            B3 = 0
            C1 = 0
            C2 = 0
            C3 = 0
        End Sub
    End Structure
    Private TESUU As strcTesuu
#End Region

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP062"

        If RSV2_MASTPTN = "2" Then
            ' ��`�̖��Z�b�g
            ReportBaseName = "KFJP062_�����}�X�^���ڊm�F�[(MASTPTN2).rpd"
        Else
            ' ��`�̖��Z�b�g
            ReportBaseName = "KFJP062_�����}�X�^���ڊm�F�[.rpd"
        End If
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        '�^�C�g���s
        If RSV2_MASTPTN = "2" Then
            CSVObject.Output("�V�X�e�����t")
            CSVObject.Output("�^�C���X�^���v")
            CSVObject.Output("���O�C����")
            CSVObject.Output("�[����")
            CSVObject.Output("������R�[�h")
            CSVObject.Output("����敛�R�[�h")
            CSVObject.Output("�}�̃R�[�h")
            CSVObject.Output("���x���敪")
            CSVObject.Output("�R�[�h�敪")
            CSVObject.Output("��\�ϑ��҃R�[�h")
            CSVObject.Output("�t�@�C����")
            CSVObject.Output("�t�H�[�}�b�g�敪")
            CSVObject.Output("�}���`�敪")
            CSVObject.Output("���o���敪")
            CSVObject.Output("���")
            CSVObject.Output("�ϑ��҃R�[�h")
            CSVObject.Output("�ϑ��Җ��J�i")
            CSVObject.Output("�戵���Z�@�փR�[�h")
            CSVObject.Output("�戵���Z�@�֖�")
            CSVObject.Output("�戵�x�X�R�[�h")
            CSVObject.Output("�戵�x�X��")
            CSVObject.Output("�Ȗ�")
            CSVObject.Output("�����ԍ�")
            CSVObject.Output("�����敪")
            CSVObject.Output("���M�敪")
            CSVObject.Output("���s�敪")
            CSVObject.Output("���U�_��쐬�敪")
            CSVObject.Output("�E�v�敪")
            CSVObject.Output("�J�i�E�v")
            CSVObject.Output("�����E�v")
            CSVObject.Output("�U�փR�[�h")
            CSVObject.Output("��ƃR�[�h")
            CSVObject.Output("�ϑ��Җ�����")
            CSVObject.Output("�X�֔ԍ�")
            CSVObject.Output("�d�b�ԍ�")
            CSVObject.Output("FAX�ԍ�")
            CSVObject.Output("�ڋq�ԍ�")
            CSVObject.Output("�֘A��Ə��")
            CSVObject.Output("�ϑ��ҏZ������")
            CSVObject.Output("�X���於�J�i")
            CSVObject.Output("�X���於����")
            CSVObject.Output("�P��")
            CSVObject.Output("�Q��")
            CSVObject.Output("�R��")
            CSVObject.Output("�S��")
            CSVObject.Output("�T��")
            CSVObject.Output("�U��")
            CSVObject.Output("�V��")
            CSVObject.Output("�W��")
            CSVObject.Output("�X��")
            CSVObject.Output("�P�O��")
            CSVObject.Output("�P�P��")
            CSVObject.Output("�P�Q��")
            CSVObject.Output("�P�R��")
            CSVObject.Output("�P�S��")
            CSVObject.Output("�P�T��")
            CSVObject.Output("�P�U��")
            CSVObject.Output("�P�V��")
            CSVObject.Output("�P�W��")
            CSVObject.Output("�P�X��")
            CSVObject.Output("�Q�O��")
            CSVObject.Output("�Q�P��")
            CSVObject.Output("�Q�Q��")
            CSVObject.Output("�Q�R��")
            CSVObject.Output("�Q�S��")
            CSVObject.Output("�Q�T��")
            CSVObject.Output("�Q�U��")
            CSVObject.Output("�Q�V��")
            CSVObject.Output("�Q�W��")
            CSVObject.Output("�Q�X��")
            CSVObject.Output("�R�O��")
            CSVObject.Output("�R�P��")
            CSVObject.Output("�U�֋x���V�t�g")
            CSVObject.Output("�P��")
            CSVObject.Output("�Q��")
            CSVObject.Output("�R��")
            CSVObject.Output("�S��")
            CSVObject.Output("�T��")
            CSVObject.Output("�U��")
            CSVObject.Output("�V��")
            CSVObject.Output("�W��")
            CSVObject.Output("�X��")
            CSVObject.Output("�P�O��")
            CSVObject.Output("�P�P��")
            CSVObject.Output("�P�Q��")
            CSVObject.Output("�ĐU�_��")
            CSVObject.Output("�ĐU���R�[�h")
            CSVObject.Output("�����^���(�ĐU)")
            CSVObject.Output("���t�敪(�ĐU)")
            CSVObject.Output("�ĐU�x���V�t�g")
            CSVObject.Output("�_���")
            CSVObject.Output("�J�n�N��")
            CSVObject.Output("�I���N��")
            CSVObject.Output("��������")
            CSVObject.Output("�����^���(�˗���)")
            CSVObject.Output("���t�敪(�˗���)")
            CSVObject.Output("�˗����x���V�t�g")
            CSVObject.Output("�˗������")
            CSVObject.Output("�˗����o�͏�")
            CSVObject.Output("��z�敪")
            CSVObject.Output("��t���ו\�o�͋敪")
            CSVObject.Output("�s�\���ʖ��ו\�o�͋敪")
            CSVObject.Output("�������")
            CSVObject.Output("���ʕԋp�v��")
            CSVObject.Output("���ʖ��׃f�[�^�쐬�敪")
            CSVObject.Output("�U�֌��ʕϊ��e�[�u��ID")
            CSVObject.Output("���ϋ敪")
            CSVObject.Output("�Ƃ�܂ƂߓX�R�[�h")
            CSVObject.Output("�Ƃ�܂ƂߓX��")
            CSVObject.Output("�{���ʒi�����ԍ�")
            CSVObject.Output("�����^���(����)")
            CSVObject.Output("���t�敪(����)")
            CSVObject.Output("���ϋx���V�t�g")
            CSVObject.Output("���ϋ��Z�@�փR�[�h")
            CSVObject.Output("���ϋ��Z�@�֖�")
            CSVObject.Output("���ώx�X�R�[�h")
            CSVObject.Output("���ώx�X��")
            CSVObject.Output("���ωȖ�")
            CSVObject.Output("���ό����ԍ�")
            CSVObject.Output("���ϖ��`�l(�J�i)")
            CSVObject.Output("���l�P")
            CSVObject.Output("���l�Q")
            CSVObject.Output("�萔�������x�X�R�[�h")
            CSVObject.Output("�萔�������x�X��")
            CSVObject.Output("�萔�������Ȗ�")
            CSVObject.Output("�萔�����������ԍ�")
            CSVObject.Output("�萔�������敪")
            CSVObject.Output("�萔���������@")
            CSVObject.Output("�萔���W�v����")
            CSVObject.Output("�����^���(�萔������)")
            CSVObject.Output("���t�敪(�萔������)")
            CSVObject.Output("�萔�������x���V�t�g")
            CSVObject.Output("�萔�������敪")
            CSVObject.Output("�U�֎萔���P��")
            CSVObject.Output("����ŋ敪")
            CSVObject.Output("����")
            CSVObject.Output("�Œ�萔���P")
            CSVObject.Output("�Œ�萔���Q")
            CSVObject.Output("�W�v���")
            CSVObject.Output("�W�v�I����")
            CSVObject.Output("�W�v�")
            CSVObject.Output("�W�v���@")
            CSVObject.Output("�W�v��Ƃf�q�o")
            CSVObject.Output("�U���萔����h�c")
            CSVObject.Output("���X�P���~����")
            CSVObject.Output("���X�P���~�ȏ�R���~����")
            CSVObject.Output("���X�R���~�ȏ�")
            CSVObject.Output("�{�x�X�P���~����")
            CSVObject.Output("�{�x�X�P���~�ȏ�R���~����")
            CSVObject.Output("�{�x�X�R���~�ȏ�")
            CSVObject.Output("���s�P���~����")
            CSVObject.Output("���s�P���~�ȏ�R���~����")
            CSVObject.Output("���s�R���~�ȏ�")
            CSVObject.Output("�Í��������敪")
            CSVObject.Output("�`�d�r�I�v�V����")
            CSVObject.Output("�Í����L�[�P")
            CSVObject.Output("�Í����L�[�Q")
            CSVObject.Output("�󎆐łP")
            CSVObject.Output("�󎆐łQ")
            CSVObject.Output("�󎆐łR")
            CSVObject.Output("����Z���^�[�m�F�b�c")
            CSVObject.Output("�����Z���^�[�m�F�b�c")
            CSVObject.Output("�`���t�@�C���h�c")
            CSVObject.Output("�������@")
            CSVObject.Output("�������[�g�P")
            CSVObject.Output("�������[�g�Q")
            CSVObject.Output("�������[�g�R")
            CSVObject.Output("�ԋp�x�X�R�[�h")
            CSVObject.Output("�ԋp�x�X")
            CSVObject.Output("�ƍ��v�ۋ敪")
            CSVObject.Output("�_�񏑔ԍ�")
            CSVObject.Output("�ʑO����")
            CSVObject.Output("�ʌ㏈��")
            CSVObject.Output("���L����")
            CSVObject.Output("��t���L����")
            CSVObject.Output("�ԋp���L����")
            CSVObject.Output("�������L����")
        Else
            CSVObject.Output("�V�X�e�����t")
            CSVObject.Output("�^�C���X�^���v")
            CSVObject.Output("���O�C����")
            CSVObject.Output("�[����")
            CSVObject.Output("������R�[�h")
            CSVObject.Output("����敛�R�[�h")
            CSVObject.Output("�}�̃R�[�h")
            CSVObject.Output("���x���敪")
            CSVObject.Output("�R�[�h�敪")
            CSVObject.Output("��\�ϑ��҃R�[�h")
            CSVObject.Output("�t�@�C����")
            CSVObject.Output("�t�H�[�}�b�g�敪")
            CSVObject.Output("�}���`�敪")
            CSVObject.Output("���o���敪")
            CSVObject.Output("���")
            CSVObject.Output("�ϑ��҃R�[�h")
            CSVObject.Output("�ϑ��Җ��J�i")
            CSVObject.Output("�戵���Z�@�փR�[�h")
            CSVObject.Output("�戵���Z�@�֖�")
            CSVObject.Output("�戵�x�X�R�[�h")
            CSVObject.Output("�戵�x�X��")
            CSVObject.Output("�Ȗ�")
            CSVObject.Output("�����ԍ�")
            CSVObject.Output("�����敪")
            CSVObject.Output("���M�敪")
            CSVObject.Output("���s�敪")
            CSVObject.Output("���U�_��쐬�敪")
            CSVObject.Output("�E�v�敪")
            CSVObject.Output("�J�i�E�v")
            CSVObject.Output("�����E�v")
            CSVObject.Output("�U�փR�[�h")
            CSVObject.Output("��ƃR�[�h")
            CSVObject.Output("�ϑ��Җ�����")
            CSVObject.Output("�X�֔ԍ�")
            CSVObject.Output("�d�b�ԍ�")
            CSVObject.Output("FAX�ԍ�")
            CSVObject.Output("�ڋq�ԍ�")
            CSVObject.Output("�֘A��Ə��")
            CSVObject.Output("�ϑ��ҏZ������")
            CSVObject.Output("�X���於�J�i")
            CSVObject.Output("�X���於����")
            CSVObject.Output("�U�֓�")
            CSVObject.Output("�U�֋x���V�t�g")
            CSVObject.Output("���ʏ����t���O(���U�E��������)")
            CSVObject.Output("�ĐU�_��")
            CSVObject.Output("�ĐU���R�[�h")
            CSVObject.Output("�����^���(�ĐU)")
            CSVObject.Output("���t�敪(�ĐU)")
            CSVObject.Output("�ĐU�x���V�t�g")
            CSVObject.Output("�_���")
            CSVObject.Output("�J�n�N��")
            CSVObject.Output("�I���N��")
            CSVObject.Output("��������")
            CSVObject.Output("�����^���(�˗���)")
            CSVObject.Output("���t�敪(�˗���)")
            CSVObject.Output("�˗����x���V�t�g")
            CSVObject.Output("�˗������")
            CSVObject.Output("�˗����o�͏�")
            CSVObject.Output("��z�敪")
            CSVObject.Output("��t���ו\�o�͋敪")
            CSVObject.Output("�s�\���ʖ��ו\�o�͋敪")
            CSVObject.Output("�������")
            CSVObject.Output("���ʕԋp�v��")
            CSVObject.Output("���ʖ��׃f�[�^�쐬�敪")
            CSVObject.Output("�U�֌��ʕϊ��e�[�u��ID")
            CSVObject.Output("���ϋ敪")
            CSVObject.Output("�Ƃ�܂ƂߓX�R�[�h")
            CSVObject.Output("�Ƃ�܂ƂߓX��")
            CSVObject.Output("�{���ʒi�����ԍ�")
            CSVObject.Output("�����^���(����)")
            CSVObject.Output("���t�敪(����)")
            CSVObject.Output("���ϋx���V�t�g")
            CSVObject.Output("���ϋ��Z�@�փR�[�h")
            CSVObject.Output("���ϋ��Z�@�֖�")
            CSVObject.Output("���ώx�X�R�[�h")
            CSVObject.Output("���ώx�X��")
            CSVObject.Output("���ωȖ�")
            CSVObject.Output("���ό����ԍ�")
            CSVObject.Output("���ϖ��`�l(�J�i)")
            CSVObject.Output("���l�P")
            CSVObject.Output("���l�Q")
            CSVObject.Output("�萔�������x�X�R�[�h")
            CSVObject.Output("�萔�������x�X��")
            CSVObject.Output("�萔�������Ȗ�")
            CSVObject.Output("�萔�����������ԍ�")
            CSVObject.Output("�萔�������敪")
            CSVObject.Output("�萔���������@")
            CSVObject.Output("�萔���W�v����")
            CSVObject.Output("�����^���(�萔������)")
            CSVObject.Output("���t�敪(�萔������)")
            CSVObject.Output("�萔�������x���V�t�g")
            CSVObject.Output("�萔�������敪")
            CSVObject.Output("�U�֎萔���P��")
            CSVObject.Output("����ŋ敪")
            CSVObject.Output("����")
            CSVObject.Output("�Œ�萔���P")
            CSVObject.Output("�Œ�萔���Q")
            CSVObject.Output("�萔�������Ώۋ敪")
            CSVObject.Output("�W�v���")
            CSVObject.Output("�W�v�I����")
            CSVObject.Output("�W�v�")
            CSVObject.Output("�W�v���@")
            CSVObject.Output("�W�v��Ƃf�q�o")
            CSVObject.Output("�U���萔����h�c")
            CSVObject.Output("���X�P���~����")
            CSVObject.Output("���X�P���~�ȏ�R���~����")
            CSVObject.Output("���X�R���~�ȏ�")
            CSVObject.Output("�{�x�X�P���~����")
            CSVObject.Output("�{�x�X�P���~�ȏ�R���~����")
            CSVObject.Output("�{�x�X�R���~�ȏ�")
            CSVObject.Output("���s�P���~����")
            CSVObject.Output("���s�P���~�ȏ�R���~����")
            CSVObject.Output("���s�R���~�ȏ�")
            CSVObject.Output("�Í��������敪")
            CSVObject.Output("�`�d�r�I�v�V����")
            CSVObject.Output("�Í����L�[�P")
            CSVObject.Output("�Í����L�[�Q")
            CSVObject.Output("�󎆐łP")
            CSVObject.Output("�󎆐łQ")
            CSVObject.Output("�󎆐łR")
        End If
        CSVObject.Output("", , True)

        Return file
    End Function

    '
    ' �@�\�@ �F CSV�t�@�C���ɏ�������
    '
    ' ���l�@ �F 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    '
    ' �@�\�@ �F ����f�[�^�̍쐬
    '
    ' ���l�@ �F 
    '
    Public Function MakeRecord() As Boolean
        Dim oraDB As New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim PATH_TXT As String = CASTCommon.GetFSKJIni("COMMON", "TXT")
        Dim KINKO As String = ""
        Dim SITEN As String = ""
        Dim JIKINKO As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD ")

        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM ")
            If RSV2_MASTPTN = "2" Then
                SQL.Append("     TORIMAST_VIEW")
            Else
                SQL.Append("     TORIMAST")
            End If
            SQL.Append(" WHERE ")
            SQL.Append("     FSYORI_KBN_T = '1'")
            SQL.Append(" AND TORIS_CODE_T = " & SQ(strToriSCd))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(strToriFCd))

            If oraReader.DataReader(SQL) = True Then

                While oraReader.EOF = False

                    OutputCsvData(Today.ToString("yyyyMMdd"), True)                                 '�V�X�e�����t
                    OutputCsvData(Now.ToString("HHmmss"), True)                                     '�^�C���X�^���v
                    OutputCsvData(strUserId, True)                                                  '���O�C����
                    OutputCsvData(Environment.MachineName, True)                                    '�[����
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TORIS_CODE_T")), True)            '������R�[�h
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TORIF_CODE_T")), True)            '����敛�R�[�h
                    '�}�̖����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_�}�̃R�[�h.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("BAITAI_CODE_T"))), True)
                    '���x���敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���x���敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("LABEL_KBN_T"))), True)
                    '�R�[�h�敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_�R�[�h�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("CODE_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_KANRI_CODE_T")), True)      '��\�ϑ��҃R�[�h
                    OutputCsvData(GCom.NzStr(oraReader.GetString("FILE_NAME_T")), True)             '�t�@�C����

                    '�t�H�[�}�b�g�敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_�t�H�[�}�b�g�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("FMT_KBN_T"))), True)
                    '�}���`�敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�}���`�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("MULTI_KBN_T"))), True)
                    '���o���敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���o���敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("NS_KBN_T"))), True)
                    '��ʖ����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SYUBETU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_CODE_T")), True)            '�ϑ��҃R�[�h
                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_KNAME_T")), True)           '�ϑ��Җ��J�i
                    KINKO = GCom.NzStr(oraReader.GetString("TKIN_NO_T"))
                    OutputCsvData(KINKO, True)                                                      '�戵���Z�@�փR�[�h
                    OutputCsvData(GCom.GetBKBRName(KINKO, "", 30), True)                            '�戵���Z�@�֖�
                    SITEN = GCom.NzStr(oraReader.GetString("TSIT_NO_T"))
                    OutputCsvData(SITEN, True)                                                      '�戵�x�X�R�[�h
                    OutputCsvData(GCom.GetBKBRName(KINKO, SITEN, 30), True)                         '�戵�x�X��

                    '�Ȗږ����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_�Ȗ�.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KAMOKU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOUZA_T")), True)                 '�����ԍ�

                    '�����敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_�����敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("MOTIKOMI_KBN_T"))), True)
                    '���M�敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���M�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SOUSIN_KBN_T"))), True)
                    '���s�敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���s�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TAKO_KBN_T"))), True)
                    '���U�_��쐬�敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���U�_��쐬�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("JIFURICHK_KBN_T"))), True)
                    '�E�v�敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�E�v�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TEKIYOU_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("KTEKIYOU_T")), True)              '�J�i�E�v
                    OutputCsvData(GCom.NzStr(oraReader.GetString("NTEKIYOU_T")), True)              '�����E�v
                    OutputCsvData(GCom.NzStr(oraReader.GetString("FURI_CODE_T")), True)             '�U�փR�[�h
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KIGYO_CODE_T")), True)            '��ƃR�[�h
                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True)           '�ϑ��Ж�����
                    OutputCsvData(GCom.NzStr(oraReader.GetString("YUUBIN_T")), True)                '�X�֔ԍ�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("DENWA_T")), True)                 '�d�b�ԍ�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("FAX_T")), True)                   'FAX�ԍ�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOKYAKU_NO_T")), True)            '�ڋq�ԍ�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KANREN_KIGYO_CODE_T")), True)     '�֘A��Ə��
                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_NJYU_T")), True)            '�ϑ��ҏZ������
                    OutputCsvData(GCom.NzStr(oraReader.GetString("YUUBIN_KNAME_T")), True)          '�X���於�J�i
                    OutputCsvData(GCom.NzStr(oraReader.GetString("YUUBIN_NNAME_T")), True)          '�X���於����

                    '�U�֓�
                    If RSV2_MASTPTN = "2" Then
                        For No As Integer = 1 To 31
                            If GCom.NzStr(GCom.NzStr(oraReader.GetString("DATE" & No & "_T"))) = "1" Then
                                OutputCsvData("��", True)
                            Else
                                OutputCsvData("", True)
                            End If
                        Next
                    Else
                        Dim FURI_DATE As String = ""
                        For No As Integer = 1 To 31
                            If GCom.NzStr(GCom.NzStr(oraReader.GetString("DATE" & No & "_T"))) = "1" Then
                                FURI_DATE &= No & " "
                            End If
                        Next
                        OutputCsvData(FURI_DATE, True)
                    End If

                    '�U�֋x���V�t�g���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�U�֋x���V�t�g.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("FURI_KYU_CODE_T"))), True)

                    '���ʏ����t���O
                    If RSV2_MASTPTN = "2" Then
                        For No As Integer = 1 To 12
                            If GCom.NzStr(GCom.NzStr(oraReader.GetString("TUKI" & No & "_T"))) = "1" Then
                                OutputCsvData("��", True)
                            Else
                                OutputCsvData("", True)
                            End If
                        Next
                    Else
                        Dim TUKI As String = ""
                        For No As Integer = 1 To 12
                            If GCom.NzStr(GCom.NzStr(oraReader.GetString("TUKI" & No & "_T"))) = "1" Then
                                TUKI &= No & " "
                            End If
                        Next
                        OutputCsvData(TUKI, True)
                    End If

                    '�ĐU�_�񖼂��e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�ĐU�_��.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SFURI_FLG_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("SFURI_FCODE_T")), True)           '�ĐU���R�[�h
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SFURI_DAY_T")), True)             '����/���(�ĐU)

                    '���t�敪(�ĐU)�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���t�敪(�ĐU).TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SFURI_KIJITSU_T"))), True)
                    '�ĐU�x���V�t�g�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�ĐU�x���V�t�g.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SFURI_KYU_CODE_T"))), True)

                    If GCom.NzStr(oraReader.GetString("KEIYAKU_DATE_T")) = "00000000" Then          '�_���
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCom.NzStr(oraReader.GetString("KEIYAKU_DATE_T")), True)
                    End If
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KAISI_DATE_T")), True)            '�J�n�N��
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SYURYOU_DATE_T")), True)          '�I���N��
                    OutputCsvData(GCom.NzStr(oraReader.GetString("MOTIKOMI_KIJITSU_T")), True)      '��������
                    OutputCsvData(GCom.NzStr(oraReader.GetString("IRAISYO_YDATE_T")), True)         '����/���(�˗���)

                    '���t�敪(�˗���)�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���t�敪(�˗���).TXT"), _
                                                                GCom.NzStr(oraReader.GetString("IRAISYO_KIJITSU_T"))), True)
                    '�˗����x���V�t�g�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�˗����x���V�t�g.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("IRAISYO_KYU_CODE_T"))), True)
                    '�˗�����ʖ����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_�˗������.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("IRAISYO_KBN_T"))), True)
                    '�˗����o�͏����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�˗����o�͏�.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("IRAISYO_SORT_T"))), True)
                    '��z�敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_��z�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TEIGAKU_KBN_T"))), True)
                    '��t���ו\�o�͋敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_��t���ו\�o�͋敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("UMEISAI_KBN_T"))), True)
                    '�s�\���ʖ��ו\�o�͋敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�s�\���ʖ��ו\�o�͋敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("FUNOU_MEISAI_KBN_T"))), True)
                    '����������e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�������.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("PRTNUM_T"))), True)
                    '���ʕԋp�v�ۂ��e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���ʕԋp�v��.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KEKKA_HENKYAKU_KBN_T"))), True)
                    '���ʖ��׃f�[�^�쐬�敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���ʖ��׃f�[�^�쐬�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KEKKA_MEISAI_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("FKEKKA_TBL_T")), True)            '�U�֌��ʕϊ��e�[�u��ID

                    '���ϋ敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���ϋ敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_KBN_T"))), True)

                    SITEN = GCom.NzStr(oraReader.GetString("TORIMATOME_SIT_T"))
                    OutputCsvData(SITEN, True)                                                      '�Ƃ�܂ƂߓX�R�[�h
                    OutputCsvData(GCom.GetBKBRName(JIKINKO, SITEN, 30), True)                       '�Ƃ�܂ƂߓX��
                    OutputCsvData(GCom.NzStr(oraReader.GetString("HONBU_KOUZA_T")), True)           '�{���ʒi�����ԍ�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KESSAI_DAY_T")), True)            '����/���(����)

                    '���t�敪(����)���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���t�敪(����).TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_KIJITSU_T"))), True)
                    '���ϋx���V�t�g���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���ϋx���V�t�g.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_KYU_CODE_T"))), True)

                    KINKO = GCom.NzStr(oraReader.GetString("TUKEKIN_NO_T"))
                    OutputCsvData(KINKO, True)                                                      '���ϋ��Z�@�փR�[�h
                    OutputCsvData(GCom.GetBKBRName(KINKO, "", 30), True)                            '���ϋ��Z�@�֖�
                    SITEN = GCom.NzStr(oraReader.GetString("TUKESIT_NO_T"))
                    OutputCsvData(SITEN, True)                                                      '���ώx�X�R�[�h
                    OutputCsvData(GCom.GetBKBRName(KINKO, SITEN, 30), True)                         '���ώx�X��

                    '���ωȖږ����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_���ωȖ�.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TUKEKAMOKU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TUKEKOUZA_T")), True)             '���ό����ԍ�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TUKEMEIGI_KNAME_T")), True)       '���ϖ��`�l(�J�i)
                    OutputCsvData(GCom.NzStr(oraReader.GetString("BIKOU1_T")), True)                '���l1
                    OutputCsvData(GCom.NzStr(oraReader.GetString("BIKOU2_T")), True)                '���l2
                    SITEN = GCom.NzStr(oraReader.GetString("TESUUTYO_SIT_T"))
                    OutputCsvData(SITEN, True)                                                      '�萔�������x�X�R�[�h
                    OutputCsvData(GCom.GetBKBRName(JIKINKO, SITEN, 30), True)                       '�萔�������x�X��

                    '�萔�������Ȗڂ��e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_�萔�������Ȗ�.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_KAMOKU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUTYO_KOUZA_T")), True)        '�萔�����������ԍ�

                    '�萔�������敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�萔�������敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_KBN_T"))), True)
                    '�萔���������@���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�萔���������@.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_PATN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUMAT_NO_T")).Trim, True)       '�萔���W�v����
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUTYO_DAY_T")), True)           '����/���(�萔������)

                    '���t�敪(�萔������)���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_���t�敪(�萔������).TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_KIJITSU_T"))), True)
                    '�萔�������x���V�t�g���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�萔���x���V�t�g.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUU_KYU_CODE_T"))), True)
                    '�萔�������敪�V�t�g���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�萔�������敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SEIKYU_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("KIHTESUU_T")), True)              '�U�֎萔���P��

                    '����ŋ敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_����ŋ敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SYOUHI_KBN_T"))), True)


                    OutputCsvData(GCom.NzStr(oraReader.GetString("SOURYO_T")), True)                '����
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOTEI_TESUU1_T")), True)          '�Œ�萔��1
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOTEI_TESUU2_T")), True)          '�Œ�萔��2
                    If RSV2_MASTPTN <> "2" Then
                        OutputCsvData("", True)                                                     '�萔�������Ώۋ敪
                    End If
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUMAT_MONTH_T")), True)        '�W�v���
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUMAT_ENDDAY_T")), True)       '�W�v�I����

                    '�W�v����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�W�v�.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUMAT_KIJYUN_T"))), True)
                    '�W�v���@���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�W�v���@.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUMAT_PATN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUU_GRP_T")), True)             '�W�v���GRP

                    '�󎆐ŁA�U���萔�����擾
                    TESUU.Init()
                    Call GetInshizei(oraReader.GetString("TESUU_TABLE_ID_T"), TESUU, oraDB)
                    OutputCsvData(TESUU.KijyunIDName, True)

                    OutputCsvData(TESUU.A1.ToString, True)                                          '���X1���~����
                    OutputCsvData(TESUU.A2.ToString, True)                                          '���X1���~�ȏ�3���~����
                    OutputCsvData(TESUU.A3.ToString, True)                                          '���X3���~�ȏ�
                    OutputCsvData(TESUU.B1.ToString, True)                                          '�{�x�X1���~����
                    OutputCsvData(TESUU.B2.ToString, True)                                          '�{�x�X1���~�ȏ�3���~����
                    OutputCsvData(TESUU.B3.ToString, True)                                          '�{�x�X3���~�ȏ�
                    OutputCsvData(TESUU.C1.ToString, True)                                          '���s1���~����
                    OutputCsvData(TESUU.C2.ToString, True)                                          '���s1���~�ȏ�3���~����
                    OutputCsvData(TESUU.C3.ToString, True)                                          '���s3���~�ȏ�

                    '�Í��������敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�Í��������敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("ENC_KBN_T"))), True)
                    'AES�I�v�V�������e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST010_�`�d�r�I�v�V����.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("ENC_OPT1_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("ENC_KEY1_T")), True)              '�Í����L�[1
                    OutputCsvData(GCom.NzStr(oraReader.GetString("ENC_KEY2_T")), True)              '�Í����L�[2
                    OutputCsvData(Me.InshizeiLabel.Inshizei1, True)                                 '�󎆐�1
                    OutputCsvData(Me.InshizeiLabel.Inshizei2, True)                                 '�󎆐�2
                    OutputCsvData(Me.InshizeiLabel.Inshizei3, True)                                 '�󎆐�3

                    If RSV2_MASTPTN = "2" Then
                        OutputCsvData(GCom.NzStr(oraReader.GetString("AITE_CNT_CODE_T")), True)         '����Z���^�[�m�F�b�c
                        OutputCsvData(GCom.NzStr(oraReader.GetString("TOHO_CNT_CODE_T")), True)         '�����Z���^�[�m�F�b�c
                        OutputCsvData(GCom.NzStr(oraReader.GetString("DENSO_FILE_ID_T")), True)         '�`���t�@�C���h�c

                        '�������@���e�L�X�g����擾����
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST011_�������@.TXT"), _
                                                                    GCom.NzStr(oraReader.GetString("HANSOU_KBN_T"))), True)

                        OutputCsvData(GCom.NzStr(oraReader.GetString("HANSOU_ROOT1_T")), True)          '�������[�g�P
                        OutputCsvData(GCom.NzStr(oraReader.GetString("HANSOU_ROOT2_T")), True)          '�������[�g�Q
                        OutputCsvData(GCom.NzStr(oraReader.GetString("HANSOU_ROOT3_T")), True)          '�������[�g�R
                        SITEN = GCom.NzStr(oraReader.GetString("HENKYAKU_SIT_NO_T"))
                        OutputCsvData(SITEN, True)                                                      '�ԋp�x�X�R�[�h
                        OutputCsvData(GCom.GetBKBRName(JIKINKO, SITEN, 30), True)                       '�ԋp�x�X

                        '�ƍ��v�ۋ敪���e�L�X�g����擾����
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST011_�ƍ��v�ۋ敪.TXT"), _
                                                                    GCom.NzStr(oraReader.GetString("SYOUGOU_KBN_T"))), True)

                        OutputCsvData(GCom.NzStr(oraReader.GetString("KEIYAKU_NO_T")), True)            '�_�񏑔ԍ�

                        '�ʑO�������e�L�X�g����擾����
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST011_�ʑO����.TXT"), _
                                                                    GCom.NzStr(oraReader.GetString("MAE_SYORI_T"))), True)
                        '�ʌ㏈�����e�L�X�g����擾����
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFJMAST011_�ʌ㏈��.TXT"), _
                                                                    GCom.NzStr(oraReader.GetString("ATO_SYORI_T"))), True)

                        OutputCsvData(GCom.NzStr(oraReader.GetString("TOKKIJIKOU1_T")), True)           '���L����
                        OutputCsvData(GCom.NzStr(oraReader.GetString("TOKKIJIKOU2_T")), True)           '��t���L����
                        OutputCsvData(GCom.NzStr(oraReader.GetString("TOKKIJIKOU3_T")), True)           '�ԋp���L����
                        OutputCsvData(GCom.NzStr(oraReader.GetString("TOKKIJIKOU4_T")), True)           '�������L����
                    End If

                    OutputCsvData("", , True)    '�_�~�[

                    oraReader.NextRead()
                End While
            Else
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���R�[�h�쐬", "���s", "����ΏۂȂ�")
                RecordCnt = -1
                Return False
            End If
            Return True
        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���R�[�h�쐬", "���s", ex.ToString)
            Return False
        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function

    ''' <summary>
    ''' �󎆐ŁA�U���萔���ID�����擾����
    ''' </summary>
    ''' <param name="ID">�U���萔���ID</param>
    ''' <param name="TESUU">�U���萔�����</param>
    ''' <remarks></remarks>
    Private Sub GetInshizei(ByVal ID As String, ByRef TESUU As strcTesuu, ByVal oraDB As MyOracle)
        Dim SQL As New StringBuilder
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim TAX As CASTCommon.ClsTAX
        Try
            '�ŗ��擾
            TAX = New CASTCommon.ClsTAX
            TAX.GetZeiritsu(strKijyunDate)
            TAX.GetInshizei(strKijyunDate)

            '���[����p�Ɉ󎆐ł̋敪��ϐ��Ɏ��悤�ɂ���
            Me.InshizeiLabel.Init()
            If TAX.INSHIZEI_ID.Equals("err") = False Then
                '���z�ɂ���ĕ\���`����ς���
                Dim strInshizei1 As String
                Dim strInshizei2 As String
                If TAX.INSHIZEI1 >= 10000 Then
                    strInshizei1 = String.Format("{0:#,##0}", TAX.INSHIZEI1 / 10000) & "��"
                ElseIf TAX.INSHIZEI1 >= 1000 Then
                    strInshizei1 = String.Format("{0:#,##0}", TAX.INSHIZEI1 / 1000) & "��"
                Else
                    strInshizei1 = String.Format("{0:#,##0}", TAX.INSHIZEI1)
                End If

                If TAX.INSHIZEI2 >= 10000 Then
                    strInshizei2 = String.Format("{0:#,##0}", TAX.INSHIZEI2 / 10000) & "��"
                ElseIf TAX.INSHIZEI1 >= 1000 Then
                    strInshizei2 = String.Format("{0:#,##0}", TAX.INSHIZEI2 / 1000) & "��"
                Else
                    strInshizei2 = String.Format("{0:#,##0}", TAX.INSHIZEI2)
                End If

                Me.InshizeiLabel.Inshizei1 = strInshizei1 & "�~����"
                Me.InshizeiLabel.Inshizei2 = strInshizei1 & "�~�ȏ�" & strInshizei2 & "�~����"
                Me.InshizeiLabel.Inshizei3 = strInshizei2 & "�~�ȏ�"
            End If

            '=================================
            '= �U���萔���ID���擾
            '=================================
            Try
                With SQL
                    .Append("select * from TESUUMAST")
                    .Append(" where FSYORI_KBN_C = '1'")
                    .Append(" and SYUBETU_C = '91'")
                    .Append(" and TESUU_TABLE_ID_C = " & SQ(ID))
                    .Append(" and TAX_ID_C = " & SQ(TAX.ZEIRITSU_ID))
                End With

                oraReader = New CASTCommon.MyOracleReader(oraDB)
                If oraReader.DataReader(SQL) Then
                    With TESUU
                        .KijyunIDName = oraReader.GetString("TESUU_TABLE_NAME_C")
                        .A1 = oraReader.GetInt64("TESUU_A1_C")
                        .A2 = oraReader.GetInt64("TESUU_A2_C")
                        .A3 = oraReader.GetInt64("TESUU_A3_C")
                        .B1 = oraReader.GetInt64("TESUU_B1_C")
                        .B2 = oraReader.GetInt64("TESUU_B2_C")
                        .B3 = oraReader.GetInt64("TESUU_B3_C")
                        .C1 = oraReader.GetInt64("TESUU_C1_C")
                        .C2 = oraReader.GetInt64("TESUU_C2_C")
                        .C3 = oraReader.GetInt64("TESUU_C3_C")
                    End With
                End If

            Catch ex As Exception
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�U���萔���}�X�^�Q��", "���s", ex.ToString)
            Finally
                If Not oraReader Is Nothing Then
                    oraReader.Close()
                    oraReader = Nothing
                End If
            End Try

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�󎆐ŁA�U���萔���ID���擾", "���s", ex.ToString)
        End Try
    End Sub
End Class
