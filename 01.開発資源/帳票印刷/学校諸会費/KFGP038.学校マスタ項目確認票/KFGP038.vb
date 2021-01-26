Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports MenteCommon.clsCommon
Public Class KFGP038
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
        InfoReport.ReportName = "KFGP038"

        If RSV2_MASTPTN = "2" Then
            ' ��`�̖��Z�b�g
            ReportBaseName = "KFGP038_�w�Z�}�X�^���ڊm�F�[(MASTPTN2).rpd"
        Else
            ' ��`�̖��Z�b�g
            ReportBaseName = "KFGP038_�w�Z�}�X�^���ڊm�F�[.rpd"
        End If
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        '�^�C�g���s
        CSVObject.Output("������")
        CSVObject.Output("�^�C���X�^���v")
        CSVObject.Output("���O�C����")
        CSVObject.Output("�[����")
        CSVObject.Output("�w�Z�R�[�h")
        CSVObject.Output("�w�Z���J�i")
        CSVObject.Output("�w�Z������")
        CSVObject.Output("�g�p�w�N")
        CSVObject.Output("�ō��w�N")
        CSVObject.Output("�ŏI�i�������N")
        CSVObject.Output("�w�N")
        CSVObject.Output("�w�N��")
        CSVObject.Output("�N���X1")
        CSVObject.Output("�N���X2")
        CSVObject.Output("�N���X3")
        CSVObject.Output("�N���X4")
        CSVObject.Output("�N���X5")
        CSVObject.Output("�N���X6")
        CSVObject.Output("�N���X7")
        CSVObject.Output("�N���X8")
        CSVObject.Output("�N���X9")
        CSVObject.Output("�N���X10")
        CSVObject.Output("�N���X11")
        CSVObject.Output("�N���X12")
        CSVObject.Output("�N���X13")
        CSVObject.Output("�N���X14")
        CSVObject.Output("�N���X15")
        CSVObject.Output("�N���X16")
        CSVObject.Output("�N���X17")
        CSVObject.Output("�N���X18")
        CSVObject.Output("�N���X19")
        CSVObject.Output("�N���X20")
        CSVObject.Output("�N���X��1")
        CSVObject.Output("�N���X��2")
        CSVObject.Output("�N���X��3")
        CSVObject.Output("�N���X��4")
        CSVObject.Output("�N���X��5")
        CSVObject.Output("�N���X��6")
        CSVObject.Output("�N���X��7")
        CSVObject.Output("�N���X��8")
        CSVObject.Output("�N���X��9")
        CSVObject.Output("�N���X��10")
        CSVObject.Output("�N���X��11")
        CSVObject.Output("�N���X��12")
        CSVObject.Output("�N���X��13")
        CSVObject.Output("�N���X��14")
        CSVObject.Output("�N���X��15")
        CSVObject.Output("�N���X��16")
        CSVObject.Output("�N���X��17")
        CSVObject.Output("�N���X��18")
        CSVObject.Output("�N���X��19")
        CSVObject.Output("�N���X��20")
        CSVObject.Output("�ϑ��҃R�[�h")
        CSVObject.Output("�戵���Z�@�փR�[�h")
        CSVObject.Output("�戵���Z�@�֖�")
        CSVObject.Output("�戵�x�X�R�[�h")
        CSVObject.Output("�戵�x�X��")
        CSVObject.Output("�Ȗ�")
        CSVObject.Output("�����ԍ�")
        CSVObject.Output("���s�敪")
        CSVObject.Output("���U�_��쐬�敪")
        CSVObject.Output("�E�v�敪")
        CSVObject.Output("�J�i�E�v")
        CSVObject.Output("�����E�v")
        CSVObject.Output("�U�փR�[�h")
        CSVObject.Output("��ƃR�[�h")
        CSVObject.Output("�X�֔ԍ�")
        CSVObject.Output("�d�b�ԍ�")
        CSVObject.Output("FAX�ԍ�")
        CSVObject.Output("�ڋq�ԍ�")
        CSVObject.Output("�֘A��Ə��")
        CSVObject.Output("�ϑ��ҏZ������")
        CSVObject.Output("�U�֓�")
        CSVObject.Output("�ĐU��")
        CSVObject.Output("�}�̃R�[�h")
        CSVObject.Output("�t�@�C����")
        CSVObject.Output("�ĐU���")
        CSVObject.Output("���z���")
        CSVObject.Output("�����x���V�t�g")
        CSVObject.Output("�o���x���V�t�g")
        CSVObject.Output("�ĐU�x���V�t�g")
        CSVObject.Output("��������")
        CSVObject.Output("���U�敪")
        CSVObject.Output("�U�֌��ʕϊ��e�[�u��ID")
        CSVObject.Output("�_���")
        CSVObject.Output("�J�n�N��")
        CSVObject.Output("�I���N��")
        CSVObject.Output("�����U�֌��ʈꗗ�\")
        CSVObject.Output("�����U�֕s�\���׈ꗗ�\")
        CSVObject.Output("���[�񍐏�")
        CSVObject.Output("�����U�֓X�ʏW�v�\")
        CSVObject.Output("�����U�֖��[�̂��m�点")
        CSVObject.Output("�v���������`�[")
        CSVObject.Output("�U�֗\�薾�ו\�쐬�敪")
        CSVObject.Output("���[�\�[�g���w��")
        CSVObject.Output("�쐬��")
        CSVObject.Output("�X�V��")
        CSVObject.Output("���ϋ敪")
        CSVObject.Output("�Ƃ�܂ƂߓX�R�[�h")
        CSVObject.Output("�Ƃ�܂ƂߓX��")
        CSVObject.Output("�{���ʒi�����ԍ�")
        CSVObject.Output("����/���(����)")
        CSVObject.Output("���t�敪(����)")
        CSVObject.Output("���ϋx���V�t�g")
        CSVObject.Output("���ϋ��Z�@�փR�[�h")
        CSVObject.Output("���ϋ��Z�@�֖�")
        CSVObject.Output("���ώx�X�R�[�h")
        CSVObject.Output("���ώx�X��")
        CSVObject.Output("���ωȖ�")
        CSVObject.Output("���ό����ԍ�")
        CSVObject.Output("���ϖ��`�l(�J�i)")
        CSVObject.Output("�`�[���l1")
        CSVObject.Output("�`�[���l2")
        CSVObject.Output("�萔�������x�X�R�[�h")
        CSVObject.Output("�萔�������x�X��")
        CSVObject.Output("�萔�������Ȗ�")
        CSVObject.Output("�萔�����������ԍ�")
        CSVObject.Output("�萔�������敪")
        CSVObject.Output("�萔���������@")
        CSVObject.Output("�萔���W�v����")
        CSVObject.Output("����/���(�萔������)")
        CSVObject.Output("���t�敪(�萔������)")
        CSVObject.Output("�萔�������x���V�t�g")
        CSVObject.Output("�萔�������敪")
        CSVObject.Output("�U�֎萔���P��")
        CSVObject.Output("����ŋ敪")
        CSVObject.Output("����")
        CSVObject.Output("�Œ�萔��1")
        CSVObject.Output("�Œ�萔��2")
        CSVObject.Output("�W�v���")
        CSVObject.Output("�W�v�I����")
        CSVObject.Output("�W�v�")
        CSVObject.Output("�W�v���@")
        CSVObject.Output("�W�v���GRP")
        CSVObject.Output("�U���萔���ID")
        CSVObject.Output("���X1���~����")
        CSVObject.Output("���X1���~�ȏ�3���~����")
        CSVObject.Output("���X3���~�ȏ�")
        CSVObject.Output("�{�x�X1���~����")
        CSVObject.Output("�{�x�X1���~�ȏ�3���~����")
        CSVObject.Output("�{�x�X3���~�ȏ�")
        CSVObject.Output("���s1���~����")
        CSVObject.Output("���s1���~�ȏ�3���~����")
        CSVObject.Output("���s3���~�ȏ�")
        CSVObject.Output("�󎆐�1")
        CSVObject.Output("�󎆐�2")
        CSVObject.Output("�󎆐�3")

        If RSV2_MASTPTN = "2" Then
            CSVObject.Output("�_�񏑔ԍ�")
            CSVObject.Output("���ώ��")
            CSVObject.Output("�������@")
            CSVObject.Output("�������[�g�P")
            CSVObject.Output("�������[�g�Q")
            CSVObject.Output("�������[�g�R")
            CSVObject.Output("�ԋp�x�X�R�[�h")
            CSVObject.Output("�ԋp�x�X��")
            CSVObject.Output("�ƍ��v�ۋ敪")
        End If

        CSVObject.Output("WEB�`�����[�U��")
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
        Dim PATH_TXT As String = CASTCommon.GetFSKJIni("GCOMMON", "TXT")
        Dim KINKO As String = ""
        Dim SITEN As String = ""
        Dim FURI_DATE As String
        Dim TUKI As String
        Dim JIKINKO As String = CASTCommon.GetFSKJIni("GCOMMON", "KINKOCD ")
        Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
        Dim NowTime As String = Format(DateTime.Now, "HHmmss")

        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM ")
            If RSV2_MASTPTN = "2" Then
                SQL.Append("     GAKMAST_VIEW")
            Else
                SQL.Append("     GAKMAST2")
            End If
            SQL.Append("   , GAKMAST1")
            SQL.Append(" WHERE ")
            SQL.Append("     GAKKOU_CODE_T = " & SQ(strGakkouCd))
            SQL.Append(" AND GAKKOU_CODE_T = GAKKOU_CODE_G")
            SQL.Append(" ORDER BY ")
            SQL.Append("     GAKUNEN_CODE_G ")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStr��NULL�l�΍�
                    '�R���{�{�b�N�X�̓��e�́A��ʂ̍X�V�㍀�ڂ��擾����
                    FURI_DATE = ""
                    TUKI = ""

                    OutputCsvData(Today, True)                                                      '�V�X�e�����t
                    OutputCsvData(NowTime, True)                                                    '�^�C���X�^���v
                    OutputCsvData(strUserId, True)                                                  '���O�C����
                    OutputCsvData(Environment.MachineName, True)                                    '�[����
                    OutputCsvData(GCom.NzStr(oraReader.GetString("GAKKOU_CODE_G")), True)           '�w�Z�R�[�h
                    OutputCsvData(GCom.NzStr(oraReader.GetString("GAKKOU_KNAME_G")), True)          '�w�Z���J�i
                    OutputCsvData(GCom.NzStr(oraReader.GetString("GAKKOU_NNAME_G")), True)          '�w�Z������
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SIYOU_GAKUNEN_T")), True)         '�g�p�w�N
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SAIKOU_GAKUNEN_T")), True)        '�ō��w�N
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SINKYU_NENDO_T")), True)          '�ŏI�i�������N
                    OutputCsvData(GCom.NzStr(oraReader.GetString("GAKUNEN_CODE_G")), True)          '�w�N
                    OutputCsvData(GCom.NzStr(oraReader.GetString("GAKUNEN_NAME_G")), True)          '�w�N��
                    For No As Integer = 1 To 20                                                     '�N���X1�`20
                        If GCom.NzStr(oraReader.GetString("CLASS_NAME1" & No.ToString("00") & "_G")).Trim = "" Then
                            OutputCsvData("")
                        Else
                            OutputCsvData(GCom.NzStr(oraReader.GetString("CLASS_CODE1" & No.ToString("00") & "_G")), True)
                        End If
                    Next
                    For No As Integer = 1 To 20                                                     '�N���X��1�`20
                        OutputCsvData(GCom.NzStr(oraReader.GetString("CLASS_NAME1" & No.ToString("00") & "_G")), True)
                    Next
                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_CODE_T")), True)            '�ϑ��҃R�[�h
                    KINKO = GCom.NzStr(oraReader.GetString("TKIN_NO_T"))
                    OutputCsvData(KINKO, True)                                                      '�戵���Z�@�փR�[�h
                    OutputCsvData(GCom.GetBKBRName(KINKO, "", 30), True)                            '�戵���Z�@�֖�
                    SITEN = GCom.NzStr(oraReader.GetString("TSIT_NO_T"))
                    OutputCsvData(SITEN, True)                                                      '�戵�x�X�R�[�h
                    OutputCsvData(GCom.GetBKBRName(KINKO, SITEN, 30), True)                         '�戵�x�X��

                    '�Ȗږ����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "GFJ_�Ȗ�.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KAMOKU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOUZA_T")), True)                 '�����ԍ�

                    '���s�敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "GFJ_���s�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TAKO_KBN_T"))), True)
                    '���U�_��쐬�敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_���U�_��쐬�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("JIFURICHK_KBN_T"))), True)
                    '�E�v�敪�����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_�E�v�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TEKIYOU_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("KTEKIYOU_T")), True)              '�J�i�E�v
                    OutputCsvData(GCom.NzStr(oraReader.GetString("NTEKIYOU_T")), True)              '�����E�v
                    OutputCsvData(GCom.NzStr(oraReader.GetString("FURI_CODE_T")), True)             '�U�փR�[�h
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KIGYO_CODE_T")), True)            '��ƃR�[�h
                    OutputCsvData(GCom.NzStr(oraReader.GetString("YUUBIN_T")), True)                '�X�֔ԍ�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("DENWA_T")), True)                 '�d�b�ԍ�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("FAX_T")), True)                   'FAX�ԍ�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOKYAKU_NO_T")), True)            '�ڋq�ԍ�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KANREN_KIGYO_CODE_T")), True)     '�֘A��Ə��
                    OutputCsvData(GCom.NzStr(oraReader.GetString("ITAKU_NJYU_T")), True)            '�ϑ��ҏZ������
                    OutputCsvData(GCom.NzStr(oraReader.GetString("FURI_DATE_T")), True)             '�U�֓�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SFURI_DATE_T")), True)            '�ĐU��

                    '�}�̖����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "GFJ_�}��.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("BAITAI_CODE_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("FILE_NAME_T")), True)                       '�t�@�C����
                    Select Case GCom.NzInt(oraReader.GetString("SFURI_SYUBETU_T"))
                        Case 0
                            OutputCsvData("�Ȃ�", True)    '�ĐU���
                            OutputCsvData("�Ȃ�", True)    '���z���
                        Case 1
                            OutputCsvData("����", True)    '�ĐU���
                            OutputCsvData("�Ȃ�", True)    '���z���
                        Case 2
                            OutputCsvData("����", True)    '�ĐU���
                            OutputCsvData("����", True)    '���z���
                        Case 3
                            OutputCsvData("�Ȃ�", True)    '�ĐU���
                            OutputCsvData("����", True)    '���z���
                    End Select

                    '�o���x���V�t�g���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_�U�֋x���V�t�g.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("NKYU_CODE_T"))), True)
                    '�����x���V�t�g���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_�U�֋x���V�t�g.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SKYU_CODE_T"))), True)
                    '�ĐU�x���V�t�g���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_�ĐU�x���V�t�g.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SFURI_KYU_CODE_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("MOTIKOMI_KIJITSU_T")), True)      '��������

                    '���U�敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_���U�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("JIFURI_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("FKEKKA_TBL_T")), True)            '�U�֌��ʕϊ��e�[�u��ID
                    If GCom.NzStr(oraReader.GetString("KEIYAKU_DATE_T")) = "00000000" Then          '�_���
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCom.NzStr(oraReader.GetString("KEIYAKU_DATE_T")), True)
                    End If
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KAISI_DATE_T")), True)            '�J�n�N��
                    OutputCsvData(GCom.NzStr(oraReader.GetString("SYURYOU_DATE_T")), True)          '�I���N��
                    If GCom.NzStr(oraReader.GetString("MEISAI_FUNOU_T")) = "1" Then                 '�����U�֌��ʈꗗ�\
                        OutputCsvData("�o�͑Ώ�", True)
                    Else
                        OutputCsvData("�o�͂Ȃ�", True)
                    End If
                    If GCom.NzStr(oraReader.GetString("MEISAI_KEKKA_T")) = "1" Then                 '�����U�֕s�\���׈ꗗ�\
                        OutputCsvData("�o�͑Ώ�", True)
                    Else
                        OutputCsvData("�o�͂Ȃ�", True)
                    End If
                    If GCom.NzStr(oraReader.GetString("MEISAI_HOUKOKU_T")) = "1" Then               '���[�񍐏�
                        OutputCsvData("�o�͑Ώ�", True)
                    Else
                        OutputCsvData("�o�͂Ȃ�", True)
                    End If
                    If GCom.NzStr(oraReader.GetString("MEISAI_TENBETU_T")) = "1" Then               '�����U�֓X�ʏW�v�\
                        OutputCsvData("�o�͑Ώ�", True)
                    Else
                        OutputCsvData("�o�͂Ȃ�", True)
                    End If
                    If GCom.NzStr(oraReader.GetString("MEISAI_MINOU_T")) = "1" Then                 '�����U�֖��[�̂��m�点
                        OutputCsvData("�o�͑Ώ�", True)
                    Else
                        OutputCsvData("�o�͂Ȃ�", True)
                    End If
                    If GCom.NzStr(oraReader.GetString("MEISAI_YOUKYU_T")) = "1" Then                '�v���������`�[
                        OutputCsvData("�o�͑Ώ�", True)
                    Else
                        OutputCsvData("�o�͂Ȃ�", True)
                    End If

                    '�U�֗\�薾�ו\�쐬�敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_�U�֗\�薾�ו\�쐬�敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("MEISAI_KBN_T"))), True)
                    '���[�\�[�g���w����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_���[�\�[�g���w��.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("MEISAI_OUT_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("SAKUSEI_DATE_T")), True)          '�쐬��
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOUSIN_DATE_T")), True)           '�X�V��

                    '���ϋ敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "GFJ_���ϋ敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_KBN_T"))), True)

                    SITEN = GCom.NzStr(oraReader.GetString("TORIMATOME_SIT_T"))
                    OutputCsvData(SITEN, True)                                                      '�Ƃ�܂ƂߓX�R�[�h
                    OutputCsvData(GCom.GetBKBRName(JIKINKO, SITEN, 30), True)                       '�Ƃ�܂ƂߓX��
                    OutputCsvData(GCom.NzStr(oraReader.GetString("HONBU_KOUZA_T")), True)           '�{���ʒi�����ԍ�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KESSAI_DAY_T")), True)            '����/���(����)

                    '���t�敪(����)���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_���t�敪(����).TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_KIJITSU_T"))), True)
                    '���ϋx���V�t�g���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_���ϋx���V�t�g.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("KESSAI_KYU_CODE_T"))), True)

                    KINKO = GCom.NzStr(oraReader.GetString("TUKEKIN_NO_T"))
                    OutputCsvData(KINKO, True)                                                      '���ϋ��Z�@�փR�[�h
                    OutputCsvData(GCom.GetBKBRName(KINKO, "", 30), True)                            '���ϋ��Z�@�֖�
                    SITEN = GCom.NzStr(oraReader.GetString("TUKESIT_NO_T"))
                    OutputCsvData(SITEN, True)                                                      '���ώx�X�R�[�h
                    OutputCsvData(GCom.GetBKBRName(KINKO, SITEN, 30), True)                         '���ώx�X��

                    '���ωȖڂ��e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_���ωȖ�.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TUKEKAMOKU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TUKEKOUZA_T")), True)             '���ό����ԍ�
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TUKEMEIGI_T")), True)             '���ϖ��`�l(�J�i)
                    OutputCsvData(GCom.NzStr(oraReader.GetString("DENPYO_BIKOU1_T")), True)         '�`�[���l1
                    OutputCsvData(GCom.NzStr(oraReader.GetString("DENPYO_BIKOU2_T")), True)         '�`�[���l2
                    SITEN = GCom.NzStr(oraReader.GetString("TESUUTYO_SIT_T"))
                    OutputCsvData(SITEN, True)                                                      '�萔�������x�X�R�[�h
                    OutputCsvData(GCom.GetBKBRName(JIKINKO, SITEN, 30), True)                       '�萔�������x�X��

                    '�萔�������Ȗڂ��e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_�萔�������Ȗ�.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_KAMOKU_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUTYO_KOUZA_T")), True)        '�萔�����������ԍ�

                    '�萔�������敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_�萔�������敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_KBN_T"))), True)
                    '�萔���������@���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_�萔���������@.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_PATN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUMAT_NO_T")).Trim, True)      '�萔���W�v����
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUTYO_DAY_T")), True)          '����/���(�萔������)

                    '���t�敪(�萔������)���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_���t�敪(�萔������).TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUTYO_KIJITSU_T"))), True)
                    '�萔�������x���V�t�g���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_�萔���x���V�t�g.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUU_KYU_CODE_T"))), True)
                    '�萔�������敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_�萔�������敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SEIKYU_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("KIHTESUU_T")), True)              '�U�֎萔���P��

                    '����ŋ敪���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_����ŋ敪.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("SYOUHI_KBN_T"))), True)

                    OutputCsvData(GCom.NzStr(oraReader.GetString("SOURYO_T")), True)                '����
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOTEI_TESUU1_T")), True)          '�Œ�萔��1
                    OutputCsvData(GCom.NzStr(oraReader.GetString("KOTEI_TESUU2_T")), True)          '�Œ�萔��2
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUMAT_MONTH_T")), True)        '�W�v���
                    OutputCsvData(GCom.NzStr(oraReader.GetString("TESUUMAT_ENDDAY_T")), True)       '�W�v�I����

                    '�W�v����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_�W�v�.TXT"), _
                                                                GCom.NzStr(oraReader.GetString("TESUUMAT_KIJYUN_T"))), True)
                    '�W�v���@���e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_�W�v���@.TXT"), _
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

                    OutputCsvData(Me.InshizeiLabel.Inshizei1, True)                                 '�󎆐�1
                    OutputCsvData(Me.InshizeiLabel.Inshizei2, True)                                 '�󎆐�2
                    OutputCsvData(Me.InshizeiLabel.Inshizei3, True)                                 '�󎆐�3

                    If RSV2_MASTPTN = "2" Then
                        OutputCsvData(GCom.NzStr(oraReader.GetString("KEIYAKU_NO_T")), True)            '�_�񏑔ԍ�

                        '���ώ�ʂ��e�L�X�g����擾����
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST010_���ώ��.TXT"), _
                                                                    GCom.NzStr(oraReader.GetString("KESSAI_SYUBETU_T"))), True)
                        '�������@���e�L�X�g����擾����
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST011_�������@.TXT"), _
                                                                    GCom.NzStr(oraReader.GetString("HANSOU_KBN_T"))), True)

                        OutputCsvData(GCom.NzStr(oraReader.GetString("HANSOU_ROOT1_T")), True)          '�������[�g�P
                        OutputCsvData(GCom.NzStr(oraReader.GetString("HANSOU_ROOT2_T")), True)          '�������[�g�Q
                        OutputCsvData(GCom.NzStr(oraReader.GetString("HANSOU_ROOT3_T")), True)          '�������[�g�R
                        SITEN = GCom.NzStr(oraReader.GetString("HENKYAKU_SIT_NO_T"))
                        OutputCsvData(SITEN, True)                                                      '�ԋp�x�X�R�[�h
                        OutputCsvData(GCom.GetBKBRName(JIKINKO, SITEN, 30), True)                       '�ԋp�x�X��

                        '�ƍ��v�ۋ敪���e�L�X�g����擾����
                        OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "KFGMAST011_�ƍ��v�ۋ敪.TXT"), _
                                                                    GCom.NzStr(oraReader.GetString("SYOUGOU_KBN_T"))), True)
                    End If

                    OutputCsvData(GCom.NzStr(oraReader.GetString("YOBI1_T")), True)                 '�\���P(WEB�`�����[�U��)
                    OutputCsvData("", , True)          '�_�~�[

                    oraReader.NextRead()
                End While

            Else
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����f�[�^�쐬", "���s", "�o�^�Ȃ�")
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

                oraReader = New CASTCommon.MyOracleReader(OraDB)
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
