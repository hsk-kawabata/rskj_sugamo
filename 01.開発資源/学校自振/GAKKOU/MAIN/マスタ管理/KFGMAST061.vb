Option Explicit On 
Option Strict Off

Imports System.Text

Public Class KFGMAST061

    Private ReadOnly ThisModuleName As String = "KFGMAST061"  '���W���[����

    Private ERRMSG As String = "" '���ʃG���[���b�Z�[�W
    Private StrYasumi_List(0) As String '�x�����i�[�z��
    Private MainDB As CASTCommon.MyOracle = Nothing

    Private Enum gintKEKKA As Integer
        OK = 0
        NG = 1
        OTHER = 2
    End Enum
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST061", "���ԃX�P�W���[���쐬���")
    Private Const msgTitle As String = "���ԃX�P�W���[���쐬���(KFGMAST061)"
#Region "�\����"
    '*****�w�Z���i�[�p�\����*****
    Private Structure GAKDATA
        <VBFixedStringAttribute(10)> Public GAKKOU_CODE As String '�w�Z�R�[�h
        <VBFixedStringAttribute(50)> Public GAKKOU_NNAME As String '�w�Z������
        Public SIYOU_GAKUNEN As Integer '�g�p�w�N
        <VBFixedStringAttribute(2)> Public FURI_DATE As String '�U�֓�
        <VBFixedStringAttribute(2)> Public SFURI_DATE As String '�ĐU��
        <VBFixedStringAttribute(1)> Public BAITAI_CODE As String '�}�̃R�[�h
        <VBFixedStringAttribute(10)> Public ITAKU_CODE As String '�ϑ��҃R�[�h
        <VBFixedStringAttribute(4)> Public TKIN_CODE As String '���Z�@�փR�[�h
        <VBFixedStringAttribute(3)> Public TSIT_CODE As String '�x�X�R�[�h
        <VBFixedStringAttribute(1)> Public SFURI_SYUBETU As String '�ĐU��ʃR�[�h

        <VBFixedStringAttribute(1)> Public NKYU_CODE_T As String '�����x���R�[�h
        <VBFixedStringAttribute(1)> Public SKYU_CODE_T As String '�o���x���R�[�h
        <VBFixedStringAttribute(6)> Public KAISI_DATE As String '�J�n��
        <VBFixedStringAttribute(6)> Public SYURYOU_DATE As String '�I����
        <VBFixedStringAttribute(1)> Public TESUUTYO_KBN As String '�萔�������敪
        <VBFixedStringAttribute(1)> Public TESUUTYO_KIJITSU As String '�萔����������
        Public TESUUTYO_NO As Integer
        <VBFixedStringAttribute(1)> Public TESUU_KYU_CODE As String '�萔���x���R�[�h
        <VBFixedStringAttribute(6)> Public TAISYOU_START_NENDO As String '�ΏۊJ�n�N�x
        <VBFixedStringAttribute(6)> Public TAISYOU_END_NENDO As String '�ΏۏI���N�x

        Sub New(ByVal strGakkouCode As String, ByRef Flg As Boolean, ByVal db As CASTCommon.MyOracle)

            Dim Orareader As CASTCommon.MyOracleReader = Nothing
            '�Q�ƃt���O
            Flg = False

            Try
                Orareader = New CASTCommon.MyOracleReader(db)

                Dim SQL As New StringBuilder(1024)

                SQL.Append(" SELECT ")
                SQL.Append(" GAKKOU_NNAME_G ")         '�w�Z������
                SQL.Append(",SIYOU_GAKUNEN_T ")        '�g�p�w�N��
                SQL.Append(",FURI_DATE_T ")            '�U�֓�
                SQL.Append(",SFURI_DATE_T ")           '�ĐU��
                SQL.Append(",BAITAI_CODE_T ")          '�}�̃R�[�h
                SQL.Append(",ITAKU_CODE_T ")           '�ϑ��҃R�[�h
                SQL.Append(",TKIN_NO_T ")              '���Z�@�փR�[�h
                SQL.Append(",TSIT_NO_T ")              '�x�X�R�[�h
                SQL.Append(",SFURI_SYUBETU_T ")        '�ĐU���
                SQL.Append(",NKYU_CODE_T ")            '�����x���R�[�h
                SQL.Append(",SKYU_CODE_T ")            '�o���x���R�[�h
                SQL.Append(",KAISI_DATE_T ")           '�J�n��
                SQL.Append(",SYURYOU_DATE_T ")         '�I����
                SQL.Append(",TESUUTYO_KBN_T ")         '�萔�������敪
                SQL.Append(",TESUUTYO_KIJITSU_T ")     '�萔�����������敪
                SQL.Append(",TESUUTYO_DAY_T ")          '�萔����������
                SQL.Append(",TESUU_KYU_CODE_T ")       '�萔������
                SQL.Append(" FROM GAKMAST1,GAKMAST2 ")
                SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T ")
                SQL.Append(" AND GAKUNEN_CODE_G = 1 ")
                SQL.Append(" AND GAKKOU_CODE_G = '" & strGakkouCode & "'")

                If Orareader.DataReader(SQL) = True Then
                    With Orareader
                        GAKKOU_CODE = strGakkouCode                       '�w�Z�R�[�h
                        GAKKOU_NNAME = .GetItem("GAKKOU_NNAME_G")         '�w�Z������
                        SIYOU_GAKUNEN = CInt(.GetItem("SIYOU_GAKUNEN_T")) '�g�p�w�N��
                        FURI_DATE = .GetItem("FURI_DATE_T")               '�U�֓�
                        SFURI_DATE = .GetItem("SFURI_DATE_T")             '�ĐU��
                        BAITAI_CODE = .GetItem("BAITAI_CODE_T")           '�}�̃R�[�h
                        ITAKU_CODE = .GetItem("ITAKU_CODE_T")             '�ϑ��҃R�[�h
                        TKIN_CODE = .GetItem("TKIN_NO_T")                 '�戵���Z�@�փR�[�h
                        TSIT_CODE = .GetItem("TSIT_NO_T")                 '�戵�x�X�R�[�h
                        SFURI_SYUBETU = .GetItem("SFURI_SYUBETU_T")       '�ĐU���
                        NKYU_CODE_T = .GetItem("NKYU_CODE_T")             '�����x���R�[�h
                        SKYU_CODE_T = .GetItem("SKYU_CODE_T")             '�o���x���R�[�h
                        KAISI_DATE = .GetItem("KAISI_DATE_T")             '���U�J�n�N��
                        SYURYOU_DATE = .GetItem("SYURYOU_DATE_T")         '���U�I���N��
                        TESUUTYO_KIJITSU = .GetItem("TESUUTYO_KIJITSU_T") '�萔�����������敪
                        TESUUTYO_NO = CInt(.GetItem("TESUUTYO_DAY_T"))     '�萔����������
                        TESUUTYO_KBN = .GetItem("TESUUTYO_KBN_T")         '�萔�������敪
                        TESUU_KYU_CODE = .GetItem("TESUU_KYU_CODE_T")     '���ϋx���R�[�h
                    End With

                    '�擾����
                    Flg = True
                End If

                Orareader.Close()
                Orareader = Nothing

            Catch ex As Exception
                With GCom.GLog
                    .Job2 = "�X�P�W���[���擾"
                    .Result = "���s"
                    .Discription = "�\�����ʃG���[:" & ex.ToString
                End With
                GCom.FN_LOG_WRITE("GFJMAST0602G", New StackTrace(True))
            Finally
                If Not Orareader Is Nothing Then
                    Orareader.Close()
                End If
            End Try

        End Sub
    End Structure
    '*****�w�Z���i�[�p�\����****

    '***�w�Z�X�P�W���[���}�X�^�f�[�^�ێ��\����***
    Private Structure G_SCHMASTDATA
        <VBFixedString(10)> Public GAKKOU_CODE_S As String    '1.�w�Z�R�[�h
        <VBFixedString(6)> Public NENGETUDO_S As String      '2.�N���x
        <VBFixedString(1)> Public SCH_KBN_S As String        '3.�X�P�W���[���敪
        <VBFixedString(1)> Public FURI_KBN_S As String       '4.�U�֋敪
        <VBFixedString(8)> Public FURI_DATE_S As String      '5.�U�֓�
        <VBFixedString(8)> Public SFURI_DATE_S As String     '6.�ĐU��
        '�Ώۊw�N�t���O
        <VBFixedString(1)> Public GAKUNEN1_FLG_S As String   '7.1�N
        <VBFixedString(1)> Public GAKUNEN2_FLG_S As String   '8.2�N
        <VBFixedString(1)> Public GAKUNEN3_FLG_S As String   '9.3�N
        <VBFixedString(1)> Public GAKUNEN4_FLG_S As String   '10.4�N
        <VBFixedString(1)> Public GAKUNEN5_FLG_S As String   '11.5�N
        <VBFixedString(1)> Public GAKUNEN6_FLG_S As String   '12.6�N
        <VBFixedString(1)> Public GAKUNEN7_FLG_S As String   '13.7�N
        <VBFixedString(1)> Public GAKUNEN8_FLG_S As String   '14.8�N
        <VBFixedString(1)> Public GAKUNEN9_FLG_S As String   '15.9�N

        <VBFixedString(10)> Public ITAKU_CODE_S As String    '16.�ϑ��҃R�[�h
        <VBFixedString(4)> Public TKIN_NO_S As String        '17.���Z�@�փR�[�h
        <VBFixedString(3)> Public TSIT_NO_S As String        '18.�x�X�R�[�h
        <VBFixedString(10)> Public BAITAI_CODE_S As String   '19.�}�̃R�[�h
        <VBFixedString(1)> Public TESUU_KBN_S As String      '20.�萔�������敪
        '�e�\����A������
        <VBFixedString(8)> Public ENTRI_YDATE_S As String    '21.�G���g���\���
        <VBFixedString(8)> Public ENTRI_DATE_S As String     '22.�G���g����
        <VBFixedString(8)> Public CHECK_YDATE_S As String    '23.�`�F�b�N�\���
        <VBFixedString(8)> Public CHECK_DATE_S As String     '24.�`�F�b�N��
        <VBFixedString(8)> Public DATA_YDATE_S As String     '25.�f�[�^�\���
        <VBFixedString(8)> Public DATA_DATE_S As String      '26.�f�[�^��
        <VBFixedString(8)> Public FUNOU_YDATE_S As String    '27.�s�\�X�V�\���
        <VBFixedString(8)> Public FUNOU_DATE_S As String     '28.�s�\�X�V��
        '<VBFixedString(8)> Public HENKAN_YDATE_S As String   '29.�Ԋҗ\���
        '<VBFixedString(8)> Public HENKAN_DATE_S As String    '30.�Ԋғ�
        <VBFixedString(8)> Public KESSAI_YDATE_S As String   '31.���ϗ\���
        <VBFixedString(8)> Public KESSAI_DATE_S As String    '32.���ϓ�
        '�����t���O
        <VBFixedString(1)> Public ENTRI_FLG_S As String      '33.�G���g���t���O
        <VBFixedString(1)> Public CHECK_FLG_S As String      '34.�`�F�b�N�t���O
        <VBFixedString(1)> Public DATA_FLG_S As String       '35.�f�[�^�t���O
        <VBFixedString(1)> Public FUNOU_FLG_S As String      '36.�s�\�t���O
        '<VBFixedString(1)> Public HENKAN_FLG_S As String     '37.�Ԋ҃t���O
        <VBFixedString(1)> Public SAIFURI_FLG_S As String    '38.�ĐU�t���O
        <VBFixedString(1)> Public KESSAI_FLG_S As String     '39.���σt���O
        <VBFixedString(1)> Public TYUUDAN_FLG_S As String    '40.���f�t���O
        '�����A���z
        Public SYORI_KEN_S As Long                           '41.��������
        Public SYORI_KIN_S As Long                           '42.�������z
        Public TESUU_KIN_S As Long                           '43.�萔�����z
        Public TESUU_KIN1_S As Long                          '44.�萔�����z1
        Public TESUU_KIN2_S As Long                          '45.�萔�����z2
        Public TESUU_KIN3_S As Long                          '46.�萔�����z3
        Public FURI_KEN_S As Long                            '47.�U�֌���
        Public FURI_KIN_S As Long                            '48.�U�֋��z

        <VBFixedString(8)> Public SAKUSEI_DATE_S As String   '49.�쐬��
        <VBFixedString(16)> Public TIME_STAMP_S As String    '50.�^�C���X�^���v
        <VBFixedString(15)> Public YOBI1_S As String         '51.�\��1
        <VBFixedString(15)> Public YOBI2_S As String         '52.�\��2
        <VBFixedString(15)> Public YOBI3_S As String         '53.�\��3
        <VBFixedString(15)> Public YOBI4_S As String         '54.�\��4
        <VBFixedString(15)> Public YOBI5_S As String         '55.�\��5

        '
        '�@�֐����@-�@SetG_SCHMASTDATA
        '
        '�@�@�\    -  �w�Z�X�P�W���[���擾
        '
        '�@����    -  aGakkouCode�AaSchKbn�AaFuriKbn�AaFuriDate�AaNengetudo
        '
        '�@���l    -  
        '
        '�@
        Public Function SetG_SCHMASTDATA(ByVal aGakkouCode As String, _
                                        ByVal aSchKbn As String, _
                                        ByVal aFuriKbn As String, _
                                        ByVal aFuriDate As String, _
                                        ByVal db As CASTCommon.MyOracle, _
                                        Optional ByVal aNengetudo As String = Nothing) As Boolean

            Dim ret As Boolean = False

            Dim Orareader As CASTCommon.MyOracleReader = Nothing

            Try
                '***���O�ݒ�***
                STR_SYORI_NAME = "���ԃX�P�W���[���쐬"
                STR_COMMAND = "�X�P�W���[�����擾"
                STR_LOG_GAKKOU_CODE = aGakkouCode
                STR_LOG_FURI_DATE = aFuriDate
                '***���O�ݒ�***

                Orareader = New CASTCommon.MyOracleReader(db)

                Dim SQL As New StringBuilder(128)

                SQL.Append(" SELECT * FROM G_SCHMAST ")
                SQL.Append(" WHERE GAKKOU_CODE_S = '" & aGakkouCode & "'")
                SQL.Append(" AND   SCH_KBN_S = '" & aSchKbn & "'")
                SQL.Append(" AND   FURI_KBN_S = '" & aFuriKbn & "'")
                SQL.Append(" AND   FURI_DATE_S = '" & aFuriDate & "'")
                If aNengetudo <> Nothing Then
                    SQL.Append(" AND   NENGETUDO_S = '" & aNengetudo & "'")
                End If

                If Orareader.DataReader(SQL) = True Then
                    '��{���
                    GAKKOU_CODE_S = Orareader.GetItem("GAKKOU_CODE_S").Trim    '1.�w�Z�R�[�h
                    NENGETUDO_S = Orareader.GetItem("NENGETUDO_S").Trim        '2.�N���x
                    SCH_KBN_S = Orareader.GetItem("SCH_KBN_S").Trim            '3.�X�P�W���[���敪
                    FURI_KBN_S = Orareader.GetItem("FURI_KBN_S").Trim          '4.�U�֋敪
                    FURI_DATE_S = Orareader.GetItem("FURI_DATE_S").Trim        '5.�U�֓�
                    SFURI_DATE_S = Orareader.GetItem("SFURI_DATE_S").Trim      '6.�ĐU��
                    '�Ώۊw�N�t���O
                    GAKUNEN1_FLG_S = Orareader.GetItem("GAKUNEN1_FLG_S").Trim  '7.1�N
                    GAKUNEN2_FLG_S = Orareader.GetItem("GAKUNEN2_FLG_S").Trim  '8.2�N
                    GAKUNEN3_FLG_S = Orareader.GetItem("GAKUNEN3_FLG_S").Trim  '9.3�N
                    GAKUNEN4_FLG_S = Orareader.GetItem("GAKUNEN4_FLG_S").Trim  '10.4�N
                    GAKUNEN5_FLG_S = Orareader.GetItem("GAKUNEN5_FLG_S").Trim  '11.5�N
                    GAKUNEN6_FLG_S = Orareader.GetItem("GAKUNEN6_FLG_S").Trim  '12.6�N
                    GAKUNEN7_FLG_S = Orareader.GetItem("GAKUNEN7_FLG_S").Trim  '13.7�N
                    GAKUNEN8_FLG_S = Orareader.GetItem("GAKUNEN8_FLG_S").Trim  '14.8�N
                    GAKUNEN9_FLG_S = Orareader.GetItem("GAKUNEN9_FLG_S").Trim  '15.9�N
                    '�ʏ��
                    ITAKU_CODE_S = Orareader.GetItem("ITAKU_CODE_S").Trim      '16.�ϑ��҃R�[�h
                    TKIN_NO_S = Orareader.GetItem("TKIN_NO_S").Trim            '17.���Z�@�փR�[�h
                    TSIT_NO_S = Orareader.GetItem("TSIT_NO_S").Trim            '18.�x�X�R�[�h
                    BAITAI_CODE_S = Orareader.GetItem("BAITAI_CODE_S").Trim    '19.�}�̃R�[�h
                    TESUU_KBN_S = Orareader.GetItem("TESUU_KBN_S").Trim        '20.�萔�������敪
                    '�e���t
                    ENTRI_YDATE_S = Orareader.GetItem("ENTRI_YDATE_S").Trim    '21.�G���g���\���
                    ENTRI_DATE_S = Orareader.GetItem("ENTRI_DATE_S").Trim      '22.�G���g����
                    CHECK_YDATE_S = Orareader.GetItem("CHECK_YDATE_S").Trim    '23.�`�F�b�N�\���
                    CHECK_DATE_S = Orareader.GetItem("CHECK_DATE_S").Trim      '24.�`�F�b�N��
                    DATA_YDATE_S = Orareader.GetItem("DATA_YDATE_S").Trim      '25.�f�[�^�\���
                    DATA_DATE_S = Orareader.GetItem("DATA_DATE_S").Trim        '26.�f�[�^��
                    FUNOU_YDATE_S = Orareader.GetItem("FUNOU_YDATE_S").Trim    '27.�s�\�X�V�\���
                    FUNOU_DATE_S = Orareader.GetItem("FUNOU_DATE_S").Trim      '28.�s�\�X�V��
                    'HENKAN_YDATE_S = Orareader.GetItem("HENKAN_YDATE_S").Trim  '29.�Ԋҗ\���
                    'HENKAN_DATE_S = Orareader.GetItem("HENKAN_DATE_S").Trim    '30.�Ԋғ�
                    KESSAI_YDATE_S = Orareader.GetItem("KESSAI_YDATE_S").Trim  '31.���ϗ\���
                    KESSAI_DATE_S = Orareader.GetItem("KESSAI_DATE_S").Trim    '32.���ϓ�
                    '�����t���O
                    ENTRI_FLG_S = Orareader.GetItem("ENTRI_FLG_S").Trim        '33.�G���g���t���O
                    CHECK_FLG_S = Orareader.GetItem("CHECK_FLG_S").Trim        '34.�`�F�b�N�t���O
                    DATA_FLG_S = Orareader.GetItem("DATA_FLG_S").Trim          '35.�f�[�^�t���O
                    FUNOU_FLG_S = Orareader.GetItem("FUNOU_FLG_S").Trim        '36.�s�\�t���O
                    'HENKAN_FLG_S = Orareader.GetItem("HENKAN_FLG_S").Trim      '37.�Ԋ҃t���O
                    SAIFURI_FLG_S = Orareader.GetItem("SAIFURI_FLG_S").Trim    '38.�ĐU�t���O
                    KESSAI_FLG_S = Orareader.GetItem("KESSAI_FLG_S").Trim      '39.���σt���O
                    TYUUDAN_FLG_S = Orareader.GetItem("TYUUDAN_FLG_S").Trim    '40.���f�t���O
                    '�����A���z
                    SYORI_KEN_S = Orareader.GetItem("SYORI_KEN_S").Trim        '41.��������
                    SYORI_KIN_S = Orareader.GetItem("SYORI_KIN_S").Trim        '42.�������z
                    TESUU_KIN_S = Orareader.GetItem("TESUU_KIN_S").Trim        '43.�萔�����z
                    TESUU_KIN1_S = Orareader.GetItem("TESUU_KIN1_S").Trim      '44.�萔�����z1
                    TESUU_KIN2_S = Orareader.GetItem("TESUU_KIN2_S").Trim      '45.�萔�����z2
                    TESUU_KIN3_S = Orareader.GetItem("TESUU_KIN3_S").Trim      '46.�萔�����z3
                    FURI_KEN_S = Orareader.GetItem("FURI_KEN_S").Trim          '47.�U�֌���
                    FURI_KIN_S = Orareader.GetItem("FURI_KIN_S").Trim          '48.�U�֋��z
                    '�쐬���t
                    SAKUSEI_DATE_S = Orareader.GetItem("SAKUSEI_DATE_S").Trim  '49.�쐬��
                    TIME_STAMP_S = Orareader.GetItem("TIME_STAMP_S").Trim      '50.�^�C���X�^���v
                    '�\��
                    YOBI1_S = Orareader.GetItem("YOBI1_S").Trim                '51.�\��1(���͓��t)
                    YOBI2_S = Orareader.GetItem("YOBI2_S").Trim                '52.�\��2
                    YOBI3_S = Orareader.GetItem("YOBI3_S").Trim                '53.�\��3
                    YOBI4_S = Orareader.GetItem("YOBI4_S").Trim                '54.�\��4
                    YOBI5_S = Orareader.GetItem("YOBI5_S").Trim                '55.�\��5

                    ret = True

                End If

            Catch ex As Exception
                Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            Finally
                If Not Orareader Is Nothing Then
                    Orareader.Close()
                End If
            End Try

            Return ret

        End Function

        '
        '�@�֐����@-�@GetGakunenFlg
        '
        '�@�@�\    -  �}�X�^�̊w�N����z��ŕԂ�
        '
        '�@����    -  
        '
        '�@���l    -  
        '
        Public Function GetSchMastGakunenFlg() As Boolean()

            Dim ret As Boolean() = {False, False, False, False, False, False, False, False, False, False}

            Try
                '***���O�ݒ�***
                STR_SYORI_NAME = "���ԃX�P�W���[���쐬"
                STR_COMMAND = "�w�N�t���O�擾"
                STR_LOG_GAKKOU_CODE = GAKKOU_CODE_S
                STR_LOG_FURI_DATE = ""
                '***���O�ݒ�***

                If GAKUNEN1_FLG_S = "1" Then
                    ret(1) = True
                End If

                If GAKUNEN2_FLG_S = "1" Then
                    ret(2) = True
                End If

                If GAKUNEN3_FLG_S = "1" Then
                    ret(3) = True
                End If

                If GAKUNEN4_FLG_S = "1" Then
                    ret(4) = True
                End If

                If GAKUNEN5_FLG_S = "1" Then
                    ret(5) = True
                End If

                If GAKUNEN6_FLG_S = "1" Then
                    ret(6) = True
                End If

                If GAKUNEN7_FLG_S = "1" Then
                    ret(7) = True
                End If

                If GAKUNEN8_FLG_S = "1" Then
                    ret(8) = True
                End If

                If GAKUNEN9_FLG_S = "1" Then
                    ret(9) = True
                End If

                ret(0) = True

            Catch ex As Exception
                Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            End Try

            Return ret

        End Function

    End Structure
    '***�w�Z�X�P�W���[���}�X�^�f�[�^�ێ��\����***

    '*****�ʏ�U�֓���ʏ��i�[�p�\����*****
    Private Structure TuujyouData
        Public TaisyouFlg As Boolean '�쐬�A�X�V�A�Q�ƁA�폜�̑Ώۃt���O

        Public SyoriFurikae_Flag As Boolean '�����U�փt���O(�X�P�W���[����������)
        Public CheckFurikae_Flag As Boolean '�`�F�b�N�U�փt���O
        Public FunouFurikae_Flag As Boolean '�s�\�U�փt���O

        Public SyoriSaiFurikae_Flag As Boolean '�����ĐU�փt���O(�ĐU�X�P�W���[����������)
        Public CheckSaiFurikae_Flag As Boolean '�`�F�b�N�ĐU�փt���O

        '***��ʓ��͏��***
        <VBFixedStringAttribute(4)> Public Seikyu_Nen As String '�����N
        <VBFixedStringAttribute(2)> Public Seikyu_Tuki As String '������

        <VBFixedStringAttribute(2)> Public Furikae_Tuki As String '�U�֌�
        <VBFixedStringAttribute(2)> Public Furikae_Date As String '�U�֓�
        Public Furikae_Check As Boolean
        Public Furikae_Enabled As Boolean

        <VBFixedStringAttribute(2)> Public SaiFurikae_Tuki As String '�ĐU��
        <VBFixedStringAttribute(2)> Public SaiFurikae_Date As String '�ĐU��
        Public SaiFurikae_Check As Boolean
        Public SaiFurikae_Enabled As Boolean

        '�����w�N�t���O
        Public SiyouGakunenALL_Check As Boolean
        Public SiyouGakunen1_Check As Boolean
        Public SiyouGakunen2_Check As Boolean
        Public SiyouGakunen3_Check As Boolean
        Public SiyouGakunen4_Check As Boolean
        Public SiyouGakunen5_Check As Boolean
        Public SiyouGakunen6_Check As Boolean
        Public SiyouGakunen7_Check As Boolean
        Public SiyouGakunen8_Check As Boolean
        Public SiyouGakunen9_Check As Boolean
        '***��ʓ��͏��***

        <VBFixedStringAttribute(10)> Public Furikae_Day As String 'yyyy/mm/dd
        <VBFixedStringAttribute(10)> Public SaiFurikae_Day As String 'yyyy/mm/dd
        '
        '�@�֐����@-�@fn_GetEigyoubi
        '
        '�@�@�\    -  �ʏ�U�֍\���̗p�c�Ɠ��̎擾
        '
        '�@�߂�l  -  �z��(0) ���U�̉c�Ɠ��␳����擾�A�z��(1) �ĐU�̉c�Ɠ��␳����擾
        '
        '�@���l    -  
        '
        Public Function fn_EigyoubiHosei(ByVal aInfoGakkou As GAKDATA, ByRef HoseiEigyoubi() As String) As Boolean

            Try
                '***���O�ݒ�***
                STR_SYORI_NAME = "���ԃX�P�W���[���쐬"
                STR_COMMAND = "�c�Ɠ��擾"
                STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
                STR_LOG_FURI_DATE = ""
                '***���O�ݒ�***

                Dim WorkDate As String = ""

                '���������󔒂̏ꍇ�E�U�ւ��Ȃ��ꍇ�A�擾����K�v�Ȃ�
                If Seikyu_Tuki = "" OrElse Furikae_Check = False Then
                    HoseiEigyoubi(0) = ""
                    HoseiEigyoubi(1) = ""
                    Return True
                End If

                If Furikae_Date.Trim = "" AndAlso SaiFurikae_Date.Trim = "" Then
                    HoseiEigyoubi(0) = ""
                    HoseiEigyoubi(1) = ""
                    Return True
                End If

                '���U�c�Ɠ��␳
                HoseiEigyoubi(0) = fn_GetEigyoubi(Seikyu_Nen & Furikae_Tuki & Furikae_Date, "0", "+")

                '�ĐU�c�Ɠ��␳
                If SaiFurikae_Date.Trim = "" Then
                    HoseiEigyoubi(1) = ""
                    Return True
                End If

                Select Case True
                    Case Furikae_Date < SaiFurikae_Date
                        '�ĐU
                        HoseiEigyoubi(1) = fn_GetEigyoubi(Seikyu_Nen & Furikae_Tuki & SaiFurikae_Date, "0", "+")
                        Return True
                    Case Furikae_Date = SaiFurikae_Date
                        HoseiEigyoubi(0) = "err"
                        HoseiEigyoubi(1) = "err"
                        Return False
                    Case Furikae_Date > SaiFurikae_Date
                        '�ĐU
                        If Furikae_Tuki = "12" Then
                            HoseiEigyoubi(1) = fn_GetEigyoubi(Format(CInt(Seikyu_Nen) + 1, "0000") & "01" & SaiFurikae_Date, "0", "+")
                            Return True
                        Else
                            HoseiEigyoubi(1) = fn_GetEigyoubi(Seikyu_Nen & Format(CInt(Furikae_Tuki) + 1, "00") & SaiFurikae_Date, "0", "+")
                            Return True
                        End If
                End Select

            Catch ex As Exception
                Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
                HoseiEigyoubi(0) = "err"
                HoseiEigyoubi(1) = "err"
                Return False
            End Try

        End Function

        '
        '�@�֐����@-�@GetGakunenFlg
        '
        '�@�@�\    -  ��ʂ̊w�N����z��ŕԂ�
        '
        '�@����    -  
        '
        '�@���l    -  
        '
        Public Function GetGakunenFlg() As String()

            Dim ret As String() = {"err", "0", "0", "0", "0", "0", "0", "0", "0", "0"}

            Try
                '***���O�ݒ�***
                STR_SYORI_NAME = "���ԃX�P�W���[���쐬"
                STR_COMMAND = "��ʏ��擾"
                STR_LOG_GAKKOU_CODE = ""
                STR_LOG_FURI_DATE = ""
                '***���O�ݒ�***

                If SiyouGakunenALL_Check = True Then
                    ret(1) = "1"
                    ret(2) = "1"
                    ret(3) = "1"
                    ret(4) = "1"
                    ret(5) = "1"
                    ret(6) = "1"
                    ret(7) = "1"
                    ret(8) = "1"
                    ret(9) = "1"
                Else
                    If SiyouGakunen1_Check = True Then
                        ret(1) = "1"
                    End If

                    If SiyouGakunen2_Check = True Then
                        ret(2) = "1"
                    End If

                    If SiyouGakunen3_Check = True Then
                        ret(3) = "1"
                    End If

                    If SiyouGakunen4_Check = True Then
                        ret(4) = "1"
                    End If

                    If SiyouGakunen5_Check = True Then
                        ret(5) = "1"
                    End If

                    If SiyouGakunen6_Check = True Then
                        ret(6) = "1"
                    End If

                    If SiyouGakunen7_Check = True Then
                        ret(7) = "1"
                    End If

                    If SiyouGakunen8_Check = True Then
                        ret(8) = "1"
                    End If

                    If SiyouGakunen9_Check = True Then
                        ret(9) = "1"
                    End If
                End If

                ret(0) = "ok"

            Catch ex As Exception
                Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            End Try

            Return ret

        End Function

    End Structure
    Private Tuujyou_SchInfo(5) As TuujyouData
    Private Syoki_Tuujyou_SchInfo(5) As TuujyouData
    '*****�ʏ�U�֓���ʏ��i�[�p�\����*****

    '*****�����U�֓���ʏ��i�[�p�\����*****
    Private Structure ZuijiData
        Public TaisyouFlg As Boolean '�쐬�A�X�V�A�Q�ƁA�폜�̑Ώۃt���O

        <VBFixedStringAttribute(2)> Public Nyusyutu_Kbn As String '���o���敪

        <VBFixedStringAttribute(4)> Public Furikae_Nen As String '�����N
        <VBFixedStringAttribute(2)> Public Furikae_Tuki As String '�U�֌�
        <VBFixedStringAttribute(2)> Public Furikae_Date As String '�U�֓�
        Public Syori_Flag As Boolean

        '�����w�N�t���O
        Public SiyouGakunenALL_Check As Boolean
        Public SiyouGakunen1_Check As Boolean
        Public SiyouGakunen2_Check As Boolean
        Public SiyouGakunen3_Check As Boolean
        Public SiyouGakunen4_Check As Boolean
        Public SiyouGakunen5_Check As Boolean
        Public SiyouGakunen6_Check As Boolean
        Public SiyouGakunen7_Check As Boolean
        Public SiyouGakunen8_Check As Boolean
        Public SiyouGakunen9_Check As Boolean

        <VBFixedStringAttribute(10)> Public Furikae_Day As String 'yyyy/mm/dd

        '
        '�@�֐����@-�@fn_GetEigyoubi
        '
        '�@�@�\    -  �����U�֍\���̗p�c�Ɠ��̎擾
        '
        '�@����    -  
        '
        '�@���l    -  
        '
        Public Function fn_GetEigyoubiZuiji(ByVal aInfoGakkou As GAKDATA) As String

            Dim StrReturnDate As String = "err"

            Try
                '***���O�ݒ�***
                STR_SYORI_NAME = "���ԃX�P�W���[���쐬"
                STR_COMMAND = "�c�Ɠ��擾"
                STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
                STR_LOG_FURI_DATE = ""
                '***���O�ݒ�***

                '���������󔒂̏ꍇ�E�U�ւ��Ȃ��ꍇ�A�擾����K�v�Ȃ�
                If Furikae_Tuki = "" Then
                    Return ""
                End If

                '���t���󔒂������ꍇ�A������g�p����

                If Furikae_Date = "" Then
                    Furikae_Date = aInfoGakkou.FURI_DATE
                End If

                '�c�Ɠ����擾
                '�ʏ�X�P�W���[��
                StrReturnDate = fn_GetEigyoubi(Furikae_Nen & Furikae_Tuki & Furikae_Date, "0", "+")

            Catch ex As Exception
                Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
                StrReturnDate = "err"
            End Try

            Return StrReturnDate

        End Function

        '
        '�@�֐����@-�@GetGakunenFlg
        '
        '�@�@�\    -  ��ʂ̊w�N����z��ŕԂ�
        '
        '�@����    -  
        '
        '�@���l    -  
        '
        Public Function GetGakunenFlg() As String()

            Dim ret As String() = {"err", "0", "0", "0", "0", "0", "0", "0", "0", "0"}

            Try
                '***���O�ݒ�***
                STR_SYORI_NAME = "���ԃX�P�W���[���쐬"
                STR_COMMAND = "��ʏ��擾"
                STR_LOG_GAKKOU_CODE = ""
                STR_LOG_FURI_DATE = ""
                '***���O�ݒ�***

                If SiyouGakunenALL_Check = True Then
                    ret(1) = "1"
                    ret(2) = "1"
                    ret(3) = "1"
                    ret(4) = "1"
                    ret(5) = "1"
                    ret(6) = "1"
                    ret(7) = "1"
                    ret(8) = "1"
                    ret(9) = "1"
                Else
                    If SiyouGakunen1_Check = True Then
                        ret(1) = "1"
                    End If

                    If SiyouGakunen2_Check = True Then
                        ret(2) = "1"
                    End If

                    If SiyouGakunen3_Check = True Then
                        ret(3) = "1"
                    End If

                    If SiyouGakunen4_Check = True Then
                        ret(4) = "1"
                    End If

                    If SiyouGakunen5_Check = True Then
                        ret(5) = "1"
                    End If

                    If SiyouGakunen6_Check = True Then
                        ret(6) = "1"
                    End If

                    If SiyouGakunen7_Check = True Then
                        ret(7) = "1"
                    End If

                    If SiyouGakunen8_Check = True Then
                        ret(8) = "1"
                    End If

                    If SiyouGakunen9_Check = True Then
                        ret(9) = "1"
                    End If
                End If

                ret(0) = "ok"

            Catch ex As Exception
                Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            End Try

            Return ret

        End Function
    End Structure
    Private Zuiji_SchInfo(6) As ZuijiData
    Private Syoki_Zuiji_SchInfo(6) As ZuijiData
    '*****�����U�֓���ʏ��i�[�p�\����*****
#End Region

#Region "Form_Load"
    Private Sub KFGMAST061_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '***���O�ݒ�***
            STR_SYORI_NAME = "���ԃX�P�W���[���쐬"
            STR_COMMAND = "��ʕ\��"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***���O�ݒ�***

            With Me
                .WindowState = FormWindowState.Normal
                .FormBorderStyle = FormBorderStyle.FixedDialog
                .ControlBox = True
            End With

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            MainLog = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '��ʂ̏�����
            Call FormInitializa()

            MainDB = New CASTCommon.MyOracle

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        End Try
    End Sub
#End Region
#Region "TextBox_Validating"

    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txt�Ώ۔N�x.Validating, txt�Ώی�.Validating, txtGakkou_Code.Validating, txt�ʏ�U�֓�1.Validating, txt�ʏ�U�֓�2.Validating, txt�ʏ�U�֓�3.Validating, txt�ʏ�U�֓�4.Validating, txt�ʏ�U�֓�5.Validating, txt�ʏ�ĐU��1.Validating, txt�ʏ�ĐU��2.Validating, txt�ʏ�ĐU��3.Validating, txt�ʏ�ĐU��4.Validating, txt�ʏ�ĐU��5.Validating, txt�����U�֓�1.Validating, txt�����U�֓�2.Validating, txt�����U�֓�3.Validating, txt�����U�֓�4.Validating, txt�����U�֓�5.Validating, txt�����U�֓�6.Validating

        Try
            '***���O�ݒ�***
            STR_SYORI_NAME = "���ԃX�P�W���[���쐬"
            STR_COMMAND = "TextBox_Validating"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***���O�ݒ�***

            Select Case CType(sender, TextBox).Name
                Case "txt�Ώ۔N�x" '�Ώ۔N�x
                    Me.txt�Ώ۔N�x.BackColor = System.Drawing.Color.White

                    If Me.txt�Ώ۔N�x.Text.Trim <> "" Then
                        Me.txt�Ώ۔N�x.Text = Me.txt�Ώ۔N�x.Text.Trim.PadLeft(4, "0"c)
                    End If

                    '�x�����̕\��
                    If fn_HolidayListSet() = False Then
                        Exit Sub
                    End If

                Case "txt�Ώی�" '�Ώی�

                    If Me.txt�Ώی�.Text.Trim <> "" Then
                        Me.txt�Ώی�.Text = Me.txt�Ώی�.Text.Trim.PadLeft(2, "0"c)
                    End If

                    If Trim(txtGakkou_Code.Text) <> "" And Trim(txt�Ώ۔N�x.Text) <> "" And Trim(Me.txt�Ώی�.Text) <> "" Then
                        '�Ώ۔N�x�����͂���Ă���ꍇ�A�X�P�W���[�����݃`�F�b�N������
                        '�X�P�W���[�������݂���ꍇ�͎Q�ƃ{�^���Ƀt�H�[�J�X�ړ�
                        Call Sb_Sansyou_Focus()
                    End If

                Case "txtGakkou_Code" '�w�Z�R�[�h

                    Me.txtGakkou_Code.BackColor = System.Drawing.Color.White
                    '�w�Z���̎擾
                    If txtGakkou_Code.Text.Trim <> "" Then

                        lbl�g�p�w�N.Text = "0"

                        '0����
                        Me.txtGakkou_Code.Text = Me.txtGakkou_Code.Text.PadLeft(10, "0"c)

                        Dim key As Boolean = False
                        Dim G_Info As New GAKDATA(txtGakkou_Code.Text.Trim, key, MainDB)

                        If key = True Then

                            Me.lab�w�Z��.Text = G_Info.GAKKOU_NNAME.Trim
                            Me.lbl�g�p�w�N.Text = CStr(G_Info.SIYOU_GAKUNEN)

                            '�ō��w�N�ȏ�̊w�N�̎g�p�s��
                            Call Sb_SiyouGakunenChkEnabled(G_Info.SIYOU_GAKUNEN)

                            '�ĐU�֓��̃v���e�N�g
                            Select Case G_Info.SFURI_SYUBETU
                                Case "0", "3"
                                    Call Sb_SaifuriProtect(False)
                                Case Else
                                    Call Sb_SaifuriProtect(True)
                            End Select

                            '�X�P�W���[�������݂���ꍇ�͎Q�ƃ{�^���Ƀt�H�[�J�X�ړ�
                            If Me.txt�Ώ۔N�x.Text.Trim <> "" And Trim(Me.txt�Ώی�.Text) Then
                                Call Sb_Sansyou_Focus()
                            End If
                        Else
                            Me.lab�w�Z��.Text = ""
                            Me.lbl�g�p�w�N.Text = "0"
                        End If
                    Else
                        Me.lab�w�Z��.Text = ""
                        Me.lbl�g�p�w�N.Text = "0"
                    End If

                Case Else '����ȊO�̃e�L�X�g�{�b�N�X
                    '0�t��
                    If CType(sender, TextBox).Text.Trim <> "" Then
                        CType(sender, TextBox).Text = CType(sender, TextBox).Text.Trim.PadLeft(2, "0"c)
                    End If
            End Select

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        End Try
    End Sub
#End Region
#Region "CheckedChanged(CheckBox)"
    'CheckBoxt�`�F�b�N�ύX����
    Private Sub Chk�w�N�ʏ�_�SChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk�w�N�ʏ�1_�S.CheckedChanged, Chk�w�N�ʏ�2_�S.CheckedChanged, Chk�w�N�ʏ�3_�S.CheckedChanged, Chk�w�N�ʏ�4_�S.CheckedChanged, Chk�w�N�ʏ�5_�S.CheckedChanged, Chk����1_�S�w�N.CheckedChanged, Chk����2_�S�w�N.CheckedChanged, Chk����3_�S�w�N.CheckedChanged, Chk����4_�S�w�N.CheckedChanged, Chk����5_�S�w�N.CheckedChanged, Chk����6_�S�w�N.CheckedChanged


        Select Case CType(sender, CheckBox).Name
            Case "Chk�w�N�ʏ�1_�S"  '***�ʏ�U�֓��^�u�S�w�N�`�F�b�N�{�b�N�X***
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl�g�p�w�N.Text), Chk�w�N�ʏ�1_�S, _
                                                Chk�w�N�ʏ�1_1, Chk�w�N�ʏ�1_2, Chk�w�N�ʏ�1_3, _
                                                Chk�w�N�ʏ�1_4, _Chk�w�N�ʏ�1_5, Chk�w�N�ʏ�1_6, _
                                                Chk�w�N�ʏ�1_7, Chk�w�N�ʏ�1_8, Chk�w�N�ʏ�1_9)
            Case "Chk�w�N�ʏ�2_�S"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl�g�p�w�N.Text), Chk�w�N�ʏ�2_�S, _
                                                Chk�w�N�ʏ�2_1, Chk�w�N�ʏ�2_2, Chk�w�N�ʏ�2_3, _
                                                Chk�w�N�ʏ�2_4, Chk�w�N�ʏ�2_5, Chk�w�N�ʏ�2_6, _
                                                Chk�w�N�ʏ�2_7, Chk�w�N�ʏ�2_8, Chk�w�N�ʏ�2_9)
            Case "Chk�w�N�ʏ�3_�S"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl�g�p�w�N.Text), Chk�w�N�ʏ�3_�S, _
                                                Chk�w�N�ʏ�3_1, Chk�w�N�ʏ�3_2, Chk�w�N�ʏ�3_3, _
                                                Chk�w�N�ʏ�3_4, Chk�w�N�ʏ�3_5, Chk�w�N�ʏ�3_6, _
                                                Chk�w�N�ʏ�3_7, Chk�w�N�ʏ�3_8, Chk�w�N�ʏ�3_9)
            Case "Chk�w�N�ʏ�4_�S"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl�g�p�w�N.Text), Chk�w�N�ʏ�4_�S, _
                                                Chk�w�N�ʏ�4_1, Chk�w�N�ʏ�4_2, Chk�w�N�ʏ�4_3, _
                                                Chk�w�N�ʏ�4_4, Chk�w�N�ʏ�4_5, Chk�w�N�ʏ�4_6, _
                                                Chk�w�N�ʏ�4_7, Chk�w�N�ʏ�4_8, Chk�w�N�ʏ�4_9)
            Case "Chk�w�N�ʏ�5_�S"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl�g�p�w�N.Text), Chk�w�N�ʏ�5_�S, _
                                                Chk�w�N�ʏ�5_1, Chk�w�N�ʏ�5_2, Chk�w�N�ʏ�5_3, _
                                                Chk�w�N�ʏ�5_4, Chk�w�N�ʏ�5_5, Chk�w�N�ʏ�5_6, _
                                                Chk�w�N�ʏ�5_7, Chk�w�N�ʏ�5_8, Chk�w�N�ʏ�5_9)

            Case "Chk����1_�S�w�N" '***�����U�֓��^�u�S�w�N�`�F�b�N�{�b�N�X***
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl�g�p�w�N.Text), Chk����1_�S�w�N, _
                                                Chk����1_1�w�N, Chk����1_2�w�N, Chk����1_3�w�N, _
                                                Chk����1_4�w�N, Chk����1_5�w�N, Chk����1_6�w�N, _
                                                Chk����1_7�w�N, Chk����1_8�w�N, Chk����1_9�w�N)
            Case "Chk����2_�S�w�N"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl�g�p�w�N.Text), Chk����2_�S�w�N, _
                                                Chk����2_1�w�N, Chk����2_2�w�N, Chk����2_3�w�N, _
                                                Chk����2_4�w�N, Chk����2_5�w�N, Chk����2_6�w�N, _
                                                Chk����2_7�w�N, Chk����2_8�w�N, Chk����2_9�w�N)
            Case "Chk����3_�S�w�N"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl�g�p�w�N.Text), Chk����3_�S�w�N, _
                                                Chk����3_1�w�N, Chk����3_2�w�N, Chk����3_3�w�N, _
                                                Chk����3_4�w�N, Chk����3_5�w�N, Chk����3_6�w�N, _
                                                Chk����3_7�w�N, Chk����3_8�w�N, Chk����3_9�w�N)
            Case "Chk����4_�S�w�N"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl�g�p�w�N.Text), Chk����4_�S�w�N, _
                                                Chk����4_1�w�N, Chk����4_2�w�N, Chk����4_3�w�N, _
                                                Chk����4_4�w�N, Chk����4_5�w�N, Chk����4_6�w�N, _
                                                Chk����4_7�w�N, Chk����4_8�w�N, Chk����4_9�w�N)
            Case "Chk����5_�S�w�N"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl�g�p�w�N.Text), Chk����5_�S�w�N, _
                                                Chk����5_1�w�N, Chk����5_2�w�N, Chk����5_3�w�N, _
                                                Chk����5_4�w�N, Chk����5_5�w�N, Chk����5_6�w�N, _
                                                Chk����5_7�w�N, Chk����5_8�w�N, Chk����5_9�w�N)
            Case "Chk����6_�S�w�N"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl�g�p�w�N.Text), Chk����6_�S�w�N, _
                                                Chk����6_1�w�N, Chk����6_2�w�N, Chk����6_3�w�N, _
                                                Chk����6_4�w�N, Chk����6_5�w�N, Chk����6_6�w�N, _
                                                Chk����6_7�w�N, Chk����6_8�w�N, Chk����6_9�w�N)

            Case Else
        End Select
    End Sub


    Private Sub Chk�L���U�֓�_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk�L���U�֓��ʏ�1.CheckedChanged, Chk�L���U�֓��ʏ�2.CheckedChanged, Chk�L���U�֓��ʏ�3.CheckedChanged, Chk�L���U�֓��ʏ�4.CheckedChanged, Chk�L���U�֓��ʏ�5.CheckedChanged

        '���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If CType(sender, CheckBox).Checked = False Then
            Select Case CType(sender, CheckBox).Name
                Case "Chk�L���U�֓��ʏ�1"
                    Me.Chk�L���ĐU���ʏ�1.Checked = False
                Case "Chk�L���U�֓��ʏ�2"
                    Me.Chk�L���ĐU���ʏ�2.Checked = False
                Case "Chk�L���U�֓��ʏ�3"
                    Me.Chk�L���ĐU���ʏ�3.Checked = False
                Case "Chk�L���U�֓��ʏ�4"
                    Me.Chk�L���ĐU���ʏ�4.Checked = False
                Case "Chk�L���U�֓��ʏ�5"
                    Me.Chk�L���ĐU���ʏ�5.Checked = False

                Case Else
            End Select
        End If
    End Sub

    Private Sub Chk�L���ĐU��_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk�L���ĐU���ʏ�1.CheckedChanged, Chk�L���ĐU���ʏ�2.CheckedChanged, Chk�L���ĐU���ʏ�3.CheckedChanged, Chk�L���ĐU���ʏ�4.CheckedChanged, Chk�L���ĐU���ʏ�5.CheckedChanged

        '�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If CType(sender, CheckBox).Checked = True Then
            Select Case CType(sender, CheckBox).Name
                Case "Chk�L���ĐU���ʏ�1"
                    Me.Chk�L���U�֓��ʏ�1.Checked = True
                Case "Chk�L���ĐU���ʏ�2"
                    Me.Chk�L���U�֓��ʏ�2.Checked = True
                Case "Chk�L���ĐU���ʏ�3"
                    Me.Chk�L���U�֓��ʏ�3.Checked = True
                Case "Chk�L���ĐU���ʏ�4"
                    Me.Chk�L���U�֓��ʏ�4.Checked = True
                Case "Chk�L���ĐU���ʏ�5"
                    Me.Chk�L���U�֓��ʏ�5.Checked = True
                Case Else
            End Select
        End If
    End Sub
#End Region
    'ComboBox�ύX����
    Private Sub ComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged, cmbGakkouName.SelectedIndexChanged

        Try
            '***���O�ݒ�***
            STR_SYORI_NAME = "���ԃX�P�W���[���쐬"
            STR_COMMAND = "ConboBox_SelectedIndexChanged"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***���O�ݒ�***

            Select Case CType(sender, ComboBox).Name
                Case "cmbKana"
                    If cmbKana.Text = "" Then
                        Exit Sub
                    End If

                    '���w�Z����
                    If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                        Exit Sub
                    End If

                Case "cmbGakkouName"

                    If cmbGakkouName.SelectedIndex = -1 Then
                        Exit Sub
                    End If

                    '���S���ڏ�����
                    Call FormInitializa(2)

                    '���w�Z������̊w�Z�R�[�h�ݒ�
                    Me.txtGakkou_Code.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())
                    Me.txtGakkou_Code.Focus()

                    '�w�Z���̎擾
                    If txtGakkou_Code.Text.Trim <> "" Then

                        lbl�g�p�w�N.Text = "0"

                        Dim key As Boolean = False
                        Dim G_Info As New GAKDATA(txtGakkou_Code.Text.Trim, key, MainDB)

                        If key = True Then

                            lbl�g�p�w�N.Text = CStr(G_Info.SIYOU_GAKUNEN)

                            '�ō��w�N�ȏ�̊w�N�̎g�p�s��
                            Call Sb_SiyouGakunenChkEnabled(G_Info.SIYOU_GAKUNEN)

                            '�ĐU�֓��̃v���e�N�g
                            Select Case G_Info.SFURI_SYUBETU
                                Case "0", "3"
                                    Call Sb_SaifuriProtect(False)
                                Case Else
                                    Call Sb_SaifuriProtect(True)
                            End Select

                            '�X�P�W���[�������݂���ꍇ�͎Q�ƃ{�^���Ƀt�H�[�J�X�ړ�
                            If Me.txt�Ώ۔N�x.Text.Trim <> "" Then
                                Call Sb_Sansyou_Focus()
                            End If
                        End If
                    End If

                Case Else
            End Select
        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        End Try
    End Sub

#Region "ButtonClick"
    '�쐬�{�^������
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try
            OBJ_CONNECTION.Close()
            OBJ_CONNECTION.Open()

            '�J�[�\�����E�F�C�g��Ԃɂ���
            Cursor.Current = Cursors.WaitCursor()

            '***���O���ݒ�***
            STR_COMMAND = "�X�P�W���[���쐬"
            STR_LOG_GAKKOU_CODE = Trim(txtGakkou_Code.Text)
            STR_LOG_FURI_DATE = ""
            '***���O���ݒ�***

            '���K�{���ړ��̓`�F�b�N
            If fn_Common_Check(1) = False Then
                Exit Try
            End If

            Dim ALLGakkouCode As String()

            If txtGakkou_Code.Text.Trim = "9999999999" Then
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                '�ꊇ�쐬
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                If MessageBox.Show("�S�w�Z�ɑ΂��āA����Ō��ԃX�P�W���[���̍쐬���s�Ȃ��܂����H", _
                "�m�F", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                    Exit Sub
                End If

                ALLGakkouCode = GetALLGakkouCode()

                If ALLGakkouCode Is Nothing Then
                    MessageBox.Show("�w�Z�R�[�h�̎擾�Ɏ��s���܂���", _
                                        "�m�F", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Try
                End If

                '���擾�w�Z�����[�v
                For Cnt As Integer = 0 To ALLGakkouCode.Length - 1

                    '�^�u�ƍ\���̂̏�����
                    Call FormInitializa(2)
                    Call FormInitializa(9)

                    '���w�Z�X�P�W���[�����Ώ۔N���x�ő��݂��邩�`�F�b�N(��ł����݂���΁A�쐬�ςƂ݂Ȃ�)
                    Dim SQL As String
                    SQL = "  SELECT * FROM G_SCHMAST "
                    SQL &= " WHERE GAKKOU_CODE_S = '" & ALLGakkouCode(Cnt).Trim & "'"
                    SQL &= " AND NENGETUDO_S = '" & txt�Ώ۔N�x.Text.Trim & txt�Ώی�.Text.Trim & "'"

                    '���݂��Ȃ���΍쐬
                    If GFUNC_ISEXIST(SQL) = False Then
                        '���X�P�W���[���쐬
                        ERRMSG = ""
                        If fn_InsertG_SCHMAST(ALLGakkouCode(Cnt).Trim) = False Then
                            MessageBox.Show(ERRMSG & vbCrLf & "�w�Z�R�[�h:" & ALLGakkouCode(Cnt).Trim, _
                            "�m�F", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Try
                        End If
                    End If
                Next

                MessageBox.Show("�X�P�W���[�����쐬���܂����B", "�m�F", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                '�ʍ쐬
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                If MessageBox.Show("���ԃX�P�W���[���̍쐬���s�Ȃ��܂����H" & vbCrLf & "�w�Z�R�[�h:" & txtGakkou_Code.Text.Trim, _
                                    "�m�F", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                    Exit Sub
                End If

                '���w�Z�X�P�W���[�����Ώ۔N���x�ő��݂��邩�`�F�b�N(��ł����݂���΁A�쐬�ςƂ݂Ȃ�)
                If txtGakkou_Code.Text.Trim <> "" Then
                    Dim SQL As String
                    SQL = "  SELECT * FROM G_SCHMAST "
                    SQL &= " WHERE GAKKOU_CODE_S = '" & txtGakkou_Code.Text.Trim & "'"
                    SQL &= " AND NENGETUDO_S = '" & txt�Ώ۔N�x.Text.Trim & txt�Ώی�.Text.Trim & "'"

                    If GFUNC_ISEXIST(SQL) = True Then
                        MessageBox.Show("���ԃX�P�W���[���쐬�ςł��B�Q�ƌ�X�V���s�Ȃ��Ă��������B", _
                        "�m�F", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End If

                '�\���̂̏�����
                Call FormInitializa(9)

                '���X�P�W���[���쐬
                ERRMSG = ""
                If fn_InsertG_SCHMAST(txtGakkou_Code.Text.Trim) = False Then
                    MessageBox.Show(ERRMSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    '�w�Z��񐧌�
                    txt�Ώ۔N�x.Enabled = True
                    txtGakkou_Code.Enabled = True
                    '���̓{�^������
                    Call Sb_Btn_Enable(0)
                Else
                    '�Q�Ƃ���
                    If fn_SelectG_SCHMAST(txtGakkou_Code.Text.Trim) = False Then
                        Exit Try
                    End If

                    MessageBox.Show("�X�P�W���[�����쐬���܂����B", "�m�F", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    '���̓{�^������
                    Call Sb_Btn_Enable(1)
                End If
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            MessageBox.Show("�X�P�W���[���쐬�����Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            OBJ_CONNECTION.Close()
            OBJ_CONNECTION.Open()
        End Try

    End Sub
    '�Q�ƃ{�^������
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click
        Try
            Cursor.Current = Cursors.WaitCursor()

            '***�Q�ƃ{�^�����O���***
            STR_COMMAND = "�X�P�W���[���Q��"
            STR_LOG_GAKKOU_CODE = Trim(txtGakkou_Code.Text)
            STR_LOG_FURI_DATE = ""
            '***�Q�ƃ{�^�����O���***

            '���K�{���ړ��̓`�F�b�N
            If fn_Common_Check(2) = False Then
                Exit Try
            End If

            '���Q�Ə���
            ERRMSG = ""
            If fn_SelectG_SCHMAST(txtGakkou_Code.Text.Trim) = False Then
                MessageBox.Show(ERRMSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            Else
                '���̓{�^������
                Call Sb_Btn_Enable(1)
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            MessageBox.Show("�X�P�W���[���Q�Ə����Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            OBJ_CONNECTION.Close()
            OBJ_CONNECTION.Open()
        End Try

    End Sub
    '�X�V�{�^������
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUPDATE.Click
        Try
            Cursor.Current = Cursors.WaitCursor()

            '***�X�V�{�^�����O���***
            STR_COMMAND = "�X�P�W���[���X�V"
            STR_LOG_GAKKOU_CODE = Trim(txtGakkou_Code.Text)
            STR_LOG_FURI_DATE = ""
            '***�X�V�{�^�����O���***

            '���K�{���ړ��̓`�F�b�N
            If fn_Common_Check(3) = False Then
                Exit Try
            End If

            If MessageBox.Show("�X�V���܂����H", "�m�F", MessageBoxButtons.OKCancel, _
            MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            '���X�V����
            ERRMSG = ""
            If fn_UpdateG_SCHMAST(txtGakkou_Code.Text.Trim) = False Then
                MessageBox.Show(ERRMSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            Else
                '�Q�Ƃ���
                If fn_SelectG_SCHMAST(txtGakkou_Code.Text.Trim) = False Then
                    Exit Try
                End If

                MessageBox.Show("�X�P�W���[�����X�V���܂����B", "�m�F", MessageBoxButtons.OK, MessageBoxIcon.Information)
                '���̓{�^������
                Call Sb_Btn_Enable(2)
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            MessageBox.Show("�X�P�W���[���X�V�����Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Finally
            OBJ_CONNECTION.Close()
            OBJ_CONNECTION.Open()
        End Try

    End Sub
    '�폜�{�^������
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        Try
            OBJ_CONNECTION.Close()
            OBJ_CONNECTION.Open()

            '�J�[�\�����E�F�C�g��Ԃɂ���
            Cursor.Current = Cursors.WaitCursor()

            '***���O���ݒ�***
            STR_COMMAND = "�X�P�W���[���폜"
            STR_LOG_GAKKOU_CODE = Trim(txtGakkou_Code.Text)
            STR_LOG_FURI_DATE = ""
            '***���O���ݒ�***

            '���K�{���ړ��̓`�F�b�N
            If fn_Common_Check(1) = False Then
                Exit Try
            End If

            Dim NENGETSUDO As String = txt�Ώ۔N�x.Text.Trim & txt�Ώی�.Text.Trim
            Dim ALLGakkouCode As String()

            If txtGakkou_Code.Text.Trim = "9999999999" Then
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                '�ꊇ�폜
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                If MessageBox.Show("�S�w�Z�ɑ΂��āA�������̃X�P�W���[���̍폜���s�Ȃ��܂����H", _
                "�m�F", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                    Exit Sub
                End If

                ALLGakkouCode = GetALLGakkouCode()

                If ALLGakkouCode Is Nothing Then
                    MessageBox.Show("�w�Z�R�[�h�̎擾�Ɏ��s���܂���", _
                                        "�m�F", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Try
                End If

                '���擾�w�Z�����[�v
                For Cnt As Integer = 0 To ALLGakkouCode.Length - 1

                    '�^�u�ƍ\���̂̏�����
                    Call FormInitializa(2)
                    Call FormInitializa(9)

                    '���w�Z�X�P�W���[�����Ώ۔N���x�ő��݂��邩�`�F�b�N(��ł����݂���΁A�쐬�ςƂ݂Ȃ�)
                    Dim SQL As String
                    SQL = "  SELECT * FROM G_SCHMAST "
                    SQL &= " WHERE GAKKOU_CODE_S = '" & ALLGakkouCode(Cnt).Trim & "'"
                    SQL &= " AND NENGETUDO_S = '" & NENGETSUDO & "'"

                    '���݂���΍폜
                    If GFUNC_ISEXIST(SQL) = True Then
                        '���X�P�W���[���폜
                        ERRMSG = ""
                        If fn_DeleteG_SCHMAST_ALL(NENGETSUDO, ALLGakkouCode(Cnt).Trim) = False Then
                            MessageBox.Show(ERRMSG & vbCrLf & "�w�Z�R�[�h:" & ALLGakkouCode(Cnt).Trim, _
                            "�m�F", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Try
                        End If
                    End If
                Next

                MessageBox.Show("�X�P�W���[�����폜���܂����B", "�m�F", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                '�ʍ폜
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                If MessageBox.Show("�������X�P�W���[���̍폜���s�Ȃ��܂����H" & vbCrLf & "�w�Z�R�[�h:" & txtGakkou_Code.Text.Trim, _
                                    "�m�F", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                    Exit Sub
                End If

                '���w�Z�X�P�W���[�����Ώ۔N���x�ő��݂��邩�`�F�b�N(��ł����݂���΁A�쐬�ςƂ݂Ȃ�)
                If txtGakkou_Code.Text.Trim <> "" Then
                    Dim SQL As String
                    SQL = "  SELECT * FROM G_SCHMAST "
                    SQL &= " WHERE GAKKOU_CODE_S = '" & txtGakkou_Code.Text.Trim & "'"
                    SQL &= " AND NENGETUDO_S = '" & txt�Ώ۔N�x.Text.Trim & txt�Ώی�.Text.Trim & "'"

                    If GFUNC_ISEXIST(SQL) = False Then
                        MessageBox.Show("���ԃX�P�W���[�����쐬�ł��B", _
                        "�m�F", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End If

                '�\���̂̏�����
                Call FormInitializa(9)

                '���X�P�W���[���폜
                ERRMSG = ""
                If fn_DeleteG_SCHMAST_ALL(NENGETSUDO, txtGakkou_Code.Text.Trim) = False Then
                    MessageBox.Show(ERRMSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    '�w�Z��񐧌�
                    txt�Ώ۔N�x.Enabled = True
                    txtGakkou_Code.Enabled = True
                    '���̓{�^������
                    Call Sb_Btn_Enable(0)
                Else
                    '�Q�Ƃ���
                    If fn_SelectG_SCHMAST(txtGakkou_Code.Text.Trim) = False Then
                        Exit Try
                    End If

                    MessageBox.Show("�X�P�W���[�����폜���܂����B", "�m�F", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    '���̓{�^������
                    Call Sb_Btn_Enable(1)
                End If
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            MessageBox.Show("�X�P�W���[���쐬�����Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            OBJ_CONNECTION.Close()
            OBJ_CONNECTION.Open()
        End Try

    End Sub
    '����{�^������
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click

        '��ʏ������
        Call FormInitializa(0)

        Me.btnAction.Enabled = True
        Me.btnFind.Enabled = True
        Me.btnEnd.Enabled = True
        Me.btnEraser.Enabled = True
        Me.btnUPDATE.Enabled = True

        txt�Ώ۔N�x.Focus()
    End Sub
    '�I���{�^������
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '�I���{�^��
        Me.Close()
    End Sub
    '��ʏI�����̏���
    Private Sub GFJMAST0602G_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed

    End Sub
#End Region
#Region "SELECT G_SCHMAST"
    '
    '�@�֐����@-�@fn_SelectG_SCHMAST
    '
    '�@�@�\    -  �X�P�W���[���Q��
    '
    '�@����    -  aInfoGakkou
    '
    '�@���l    -  
    '
    '�@
    Private Function fn_SelectG_SCHMAST(ByVal strGakkouCode As String) As Boolean

        Dim ret As Boolean = False

        Try
            '***���O���***
            STR_COMMAND = "fn_SelectG_SCHMAST"
            STR_LOG_GAKKOU_CODE = strGakkouCode
            STR_LOG_FURI_DATE = ""
            '***���O���***

            '���w�Z���̎擾
            Dim Flg As Boolean = False
            Dim InfoGakkou As New GAKDATA(strGakkouCode, Flg, MainDB)

            If Flg = False Then
                ERRMSG = "�w�Z���̎擾�Ɏ��s���܂����B" & vbCrLf & "�w�Z�R�[�h:" & strGakkouCode
                Exit Try
            End If

            '�^�u�ƍ\���̂̏�����
            Call FormInitializa(2)
            Call FormInitializa(9)

            '�ĐU�֓��̃v���e�N�g
            Select Case InfoGakkou.SFURI_SYUBETU
                Case "0", "3"
                    Call Sb_SaifuriProtect(False)
                Case Else
                    Call Sb_SaifuriProtect(True)
            End Select

            '���ʏ�U�֓������擾����ʂɃZ�b�g
            If fn_SelectTuujyouG_SCHMAST(InfoGakkou) = False Then
                Exit Try
            End If

            '�������U�֓������擾����ʂɃZ�b�g
            If fn_SelectZuijiG_SCHMAST(InfoGakkou) = False Then
                Exit Try
            End If

            '�ō��w�N�ȏ�̊w�N�̎g�p�s��
            Call Sb_SiyouGakunenChkEnabled(InfoGakkou.SIYOU_GAKUNEN)

            '����ʏ��������\���̂ɃZ�b�g����
            Call Sb_GetData(Syoki_Tuujyou_SchInfo, Syoki_Zuiji_SchInfo)

            ret = True

        Catch ex As Exception
            ERRMSG = "�Q�ƂɎ��s���܂����B"
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ret = False
        End Try

        Return ret

    End Function
    '
    '�@�֐����@-�@fn_SelectTuujyouG_SCHMAST
    '
    '�@�@�\    -  �ʏ�X�P�W���[���Q��
    '
    '�@����    -  aInfoGakkou
    '
    '�@���l    -  
    '
    '�@
    Private Function fn_SelectTuujyouG_SCHMAST(ByVal aInfoGakkou As GAKDATA) As Boolean

        Dim ret As Boolean = False

        Dim Orareader As CASTCommon.MyOracleReader

        Try
            '***���O���***
            STR_COMMAND = "fn_SelectTuujyouG_SCHMAST"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***���O���***

            Dim SyoriNen As String = Me.txt�Ώ۔N�x.Text.Trim
            Dim SyoriTuki As String = Me.txt�Ώی�.Text.Trim

            Orareader = New CASTCommon.MyOracleReader()

            Dim SQL As String

            SQL = " SELECT * FROM G_SCHMAST "
            SQL &= " WHERE GAKKOU_CODE_S = '" & aInfoGakkou.GAKKOU_CODE & "'"
            SQL &= " AND   NENGETUDO_S = '" & SyoriNen & SyoriTuki & "'"
            SQL &= " AND   SCH_KBN_S ='0'"   '�ʏ�X�P�W���[���̎擾
            SQL &= " AND   FURI_KBN_S = '0'"   '���U�X�P�W���[���̎擾
            SQL &= " ORDER BY FURI_DATE_S"     '�U�֓����ɒ��o

            Dim InfoG_Schmast_Syofuri As New G_SCHMASTDATA '���U�f�[�^�i�[�p
            Dim InfoG_Schmast_Saifuri As New G_SCHMASTDATA '�ĐU�f�[�^�i�[�p

            If Orareader.DataReader(SQL) = True Then
                '���U�X�P�W���[���̃J�E���^
                Dim Cnt As Integer = 1

                While Orareader.EOF = False

                    '���U�����\���̂Ɋi�[
                    If InfoG_Schmast_Syofuri.SetG_SCHMASTDATA(aInfoGakkou.GAKKOU_CODE, "0", "0", Orareader.GetItem("FURI_DATE_S"), MainDB) = True Then
                        '��ʂɃZ�b�g����
                        If Not SetMonitor(Cnt, InfoG_Schmast_Syofuri) Then
                            '���O�͊֐����ŏo��
                            Exit Try
                        End If
                    Else
                        Call GSUB_LOG(0, "�X�P�W���[���擾���s")
                        Exit Try
                    End If

                    '�ĐU���R�[�h������
                    If InfoG_Schmast_Saifuri.SetG_SCHMASTDATA(aInfoGakkou.GAKKOU_CODE, "0", "1", Orareader.GetItem("SFURI_DATE_S"), MainDB) = True Then
                        '��ʂɃZ�b�g����
                        If Not SetMonitor(Cnt, InfoG_Schmast_Saifuri) Then
                            '���O�͊֐����ŏo��
                            Exit Try
                        End If
                    End If

                    Cnt += 1
                    Orareader.NextRead()
                End While
            End If

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        End Try

        Return ret

    End Function

    '
    '�@�֐����@-�@fn_SelectTuujyouG_SCHMAST
    '
    '�@�@�\    -  �ʏ�X�P�W���[���Q��
    '
    '�@����    -  aInfoGakkou
    '
    '�@���l    -  
    '
    '�@
    Private Function fn_SelectZuijiG_SCHMAST(ByVal aInfoGakkou As GAKDATA) As Boolean

        Dim ret As Boolean = False

        Dim Orareader As CASTCommon.MyOracleReader

        Try
            '***���O���***
            STR_COMMAND = "fn_SelectZuijiG_SCHMAST"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***���O���***

            Dim SyoriNen As String = Me.txt�Ώ۔N�x.Text.Trim
            Dim SyoriTuki As String = Me.txt�Ώی�.Text.Trim

            Orareader = New CASTCommon.MyOracleReader()

            Dim SQL As New StringBuilder(1024)

            SQL.Append(" SELECT * FROM G_SCHMAST ")
            SQL.Append(" WHERE GAKKOU_CODE_S = '" & aInfoGakkou.GAKKOU_CODE & "'")
            SQL.Append(" AND   NENGETUDO_S = '" & SyoriNen & SyoriTuki & "'")
            SQL.Append(" AND   SCH_KBN_S ='2'")     '�����X�P�W���[���̎擾
            SQL.Append(" ORDER BY FURI_DATE_S")     '�U�֓����ɒ��o

            Dim InfoG_Schmast_Zuiji As New G_SCHMASTDATA '���U�f�[�^�i�[�p

            If Orareader.DataReader(SQL) = True Then
                '�����X�P�W���[���̃J�E���^
                Dim Cnt As Integer = 1

                While Orareader.EOF = False
                    '�������\���̂Ɋi�[
                    If InfoG_Schmast_Zuiji.SetG_SCHMASTDATA(aInfoGakkou.GAKKOU_CODE, "2", Orareader.GetItem("FURI_KBN_S"), _
                                                            Orareader.GetItem("FURI_DATE_S"), MainDB) = True Then
                        '��ʂɃZ�b�g����
                        If Not SetMonitor(Cnt, InfoG_Schmast_Zuiji) Then
                            '���O�͊֐����ŏo��
                            Exit Try
                        End If
                    Else
                        '���O�͊֐����ŏo��
                        Exit Try
                    End If

                    Cnt += 1
                    Orareader.NextRead()
                End While
            End If

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        End Try

        Return ret

    End Function
    '
    '�@�֐����@-�@SetMonitor
    '
    '�@�@�\    -  �擾����������ʂɃZ�b�g
    '
    '�@����    -  Cnt
    '
    '�@���l    -  fn_SelectTuujyouG_SCHMAST,fn_SelectZuijiG_SCHMAST�̃T�u�֐�
    '             
    '
    Private Function SetMonitor(ByVal Cnt As Integer, ByVal InfoG_SCHMAST As G_SCHMASTDATA) As Boolean

        Dim ret As Boolean = False

        Try
            '***���O���***
            STR_COMMAND = "SetMonitor"
            STR_LOG_GAKKOU_CODE = InfoG_SCHMAST.GAKKOU_CODE_S
            STR_LOG_FURI_DATE = ""
            '***���O���**

            With InfoG_SCHMAST
                '���w�N���̎擾(�ʂ��S�w�N��)
                Dim GakunenFlg(9) As Boolean
                Dim FlgGakunenAll As Boolean = False
                If .GAKUNEN1_FLG_S = "1" AndAlso _
                .GAKUNEN2_FLG_S = "1" AndAlso _
                .GAKUNEN3_FLG_S = "1" AndAlso _
                .GAKUNEN4_FLG_S = "1" AndAlso _
                .GAKUNEN5_FLG_S = "1" AndAlso _
                .GAKUNEN6_FLG_S = "1" AndAlso _
                .GAKUNEN7_FLG_S = "1" AndAlso _
                .GAKUNEN8_FLG_S = "1" AndAlso _
                .GAKUNEN9_FLG_S = "1" Then
                    '�S�w�N�t���O����
                    FlgGakunenAll = True
                Else
                    GakunenFlg = .GetSchMastGakunenFlg
                    If GakunenFlg(0) = False Then
                        Exit Try
                    End If
                End If

                '��Enable����(���������ǂ���)
                Dim FlgEnable As Boolean = False
                If .ENTRI_FLG_S <> "0" OrElse _
                .CHECK_FLG_S <> "0" OrElse _
                .DATA_FLG_S <> "0" OrElse _
                .FUNOU_FLG_S <> "0" OrElse _
                .SAIFURI_FLG_S <> "0" OrElse _
                .KESSAI_FLG_S <> "0" OrElse _
                .ENTRI_FLG_S <> "0" OrElse _
                .TYUUDAN_FLG_S <> "0" Then
                    '������
                    FlgEnable = True
                End If

                '���͂Ȃ��Ή��@'20081009
                If .YOBI1_S.Trim = "" Then
                    .YOBI1_S = .FURI_DATE_S
                End If

                '���̃Z�b�g
                Select Case .SCH_KBN_S
                    Case "0" '�ʏ�
                        Select Case Cnt '�Z�b�g����ꏊ
                            Case 1
                                '���U���ĐU
                                If .FURI_KBN_S = "0" Then
                                    '�U�֓��ݒ�
                                    Chk�L���U�֓��ʏ�1.Checked = True
                                    txt�ʏ�U�֓�1.Text = .YOBI1_S.Substring(6, 2)
                                    lbl�ʏ�U�֓�1.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    '�Ώۊw�N�ݒ�
                                    If FlgGakunenAll = True Then
                                        Chk�w�N�ʏ�1_�S.Checked = True
                                    Else
                                        Chk�w�N�ʏ�1_1.Checked = GakunenFlg(1)
                                        Chk�w�N�ʏ�1_2.Checked = GakunenFlg(2)
                                        Chk�w�N�ʏ�1_3.Checked = GakunenFlg(3)
                                        Chk�w�N�ʏ�1_4.Checked = GakunenFlg(4)
                                        Chk�w�N�ʏ�1_5.Checked = GakunenFlg(5)
                                        Chk�w�N�ʏ�1_6.Checked = GakunenFlg(6)
                                        Chk�w�N�ʏ�1_7.Checked = GakunenFlg(7)
                                        Chk�w�N�ʏ�1_8.Checked = GakunenFlg(8)
                                        Chk�w�N�ʏ�1_9.Checked = GakunenFlg(9)
                                    End If
                                    '�X�V�\���ݒ�
                                    If FlgEnable = True Then
                                        Chk�L���U�֓��ʏ�1.Enabled = False
                                        txt�ʏ�U�֓�1.Enabled = False
                                        Chk�w�N�ʏ�1_1.Enabled = False
                                        Chk�w�N�ʏ�1_2.Enabled = False
                                        Chk�w�N�ʏ�1_3.Enabled = False
                                        Chk�w�N�ʏ�1_4.Enabled = False
                                        Chk�w�N�ʏ�1_5.Enabled = False
                                        Chk�w�N�ʏ�1_6.Enabled = False
                                        Chk�w�N�ʏ�1_7.Enabled = False
                                        Chk�w�N�ʏ�1_8.Enabled = False
                                        Chk�w�N�ʏ�1_9.Enabled = False
                                        Chk�w�N�ʏ�1_�S.Enabled = False
                                    End If
                                ElseIf .FURI_KBN_S = "1" Then
                                    '�U�֓��ݒ�
                                    Chk�L���ĐU���ʏ�1.Checked = True
                                    txt�ʏ�ĐU��1.Text = .YOBI1_S.Substring(6, 2)
                                    lbl�ʏ�ĐU��1.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    '�X�V�\���ݒ�
                                    If FlgEnable = True Then
                                        Chk�L���ĐU���ʏ�1.Enabled = False
                                        txt�ʏ�ĐU��1.Enabled = False
                                    End If
                                Else
                                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & .FURI_KBN_S)
                                    '�G���[
                                    Exit Try
                                End If
                            Case 2
                                '���U���ĐU
                                If .FURI_KBN_S = "0" Then
                                    '�U�֓��ݒ�
                                    Chk�L���U�֓��ʏ�2.Checked = True
                                    txt�ʏ�U�֓�2.Text = .YOBI1_S.Substring(6, 2)
                                    lbl�ʏ�U�֓�2.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    If FlgGakunenAll = True Then
                                        Chk�w�N�ʏ�2_�S.Checked = True
                                    Else
                                        Chk�w�N�ʏ�2_1.Checked = GakunenFlg(1)
                                        Chk�w�N�ʏ�2_2.Checked = GakunenFlg(2)
                                        Chk�w�N�ʏ�2_3.Checked = GakunenFlg(3)
                                        Chk�w�N�ʏ�2_4.Checked = GakunenFlg(4)
                                        Chk�w�N�ʏ�2_5.Checked = GakunenFlg(5)
                                        Chk�w�N�ʏ�2_6.Checked = GakunenFlg(6)
                                        Chk�w�N�ʏ�2_7.Checked = GakunenFlg(7)
                                        Chk�w�N�ʏ�2_8.Checked = GakunenFlg(8)
                                        Chk�w�N�ʏ�2_9.Checked = GakunenFlg(9)
                                    End If
                                    '�X�V�\���ݒ�
                                    If FlgEnable = True Then
                                        Chk�L���U�֓��ʏ�2.Enabled = False
                                        txt�ʏ�U�֓�2.Enabled = False
                                        Chk�w�N�ʏ�2_1.Enabled = False
                                        Chk�w�N�ʏ�2_2.Enabled = False
                                        Chk�w�N�ʏ�2_3.Enabled = False
                                        Chk�w�N�ʏ�2_4.Enabled = False
                                        Chk�w�N�ʏ�2_5.Enabled = False
                                        Chk�w�N�ʏ�2_6.Enabled = False
                                        Chk�w�N�ʏ�2_7.Enabled = False
                                        Chk�w�N�ʏ�2_8.Enabled = False
                                        Chk�w�N�ʏ�2_9.Enabled = False
                                        Chk�w�N�ʏ�2_�S.Enabled = False
                                    End If
                                ElseIf .FURI_KBN_S = "1" Then
                                    '�U�֓��ݒ�
                                    Chk�L���ĐU���ʏ�2.Checked = True
                                    txt�ʏ�ĐU��2.Text = .YOBI1_S.Substring(6, 2)
                                    lbl�ʏ�ĐU��2.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    '�X�V�\���ݒ�
                                    If FlgEnable = True Then
                                        Chk�L���ĐU���ʏ�2.Enabled = False
                                        txt�ʏ�ĐU��2.Enabled = False
                                    End If
                                Else
                                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & .FURI_KBN_S)
                                    '�G���[
                                    Exit Try
                                End If
                            Case 3
                                '���U���ĐU
                                If .FURI_KBN_S = "0" Then
                                    '�U�֓��ݒ�
                                    Chk�L���U�֓��ʏ�3.Checked = True
                                    txt�ʏ�U�֓�3.Text = .YOBI1_S.Substring(6, 2)
                                    lbl�ʏ�U�֓�3.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    If FlgGakunenAll = True Then
                                        Chk�w�N�ʏ�3_�S.Checked = True
                                    Else
                                        Chk�w�N�ʏ�3_1.Checked = GakunenFlg(1)
                                        Chk�w�N�ʏ�3_2.Checked = GakunenFlg(2)
                                        Chk�w�N�ʏ�3_3.Checked = GakunenFlg(3)
                                        Chk�w�N�ʏ�3_4.Checked = GakunenFlg(4)
                                        Chk�w�N�ʏ�3_5.Checked = GakunenFlg(5)
                                        Chk�w�N�ʏ�3_6.Checked = GakunenFlg(6)
                                        Chk�w�N�ʏ�3_7.Checked = GakunenFlg(7)
                                        Chk�w�N�ʏ�3_8.Checked = GakunenFlg(8)
                                        Chk�w�N�ʏ�3_9.Checked = GakunenFlg(9)
                                    End If
                                    '�X�V�\���ݒ�
                                    If FlgEnable = True Then
                                        Chk�L���U�֓��ʏ�3.Enabled = False
                                        txt�ʏ�U�֓�3.Enabled = False
                                        Chk�w�N�ʏ�3_1.Enabled = False
                                        Chk�w�N�ʏ�3_2.Enabled = False
                                        Chk�w�N�ʏ�3_3.Enabled = False
                                        Chk�w�N�ʏ�3_4.Enabled = False
                                        Chk�w�N�ʏ�3_5.Enabled = False
                                        Chk�w�N�ʏ�3_6.Enabled = False
                                        Chk�w�N�ʏ�3_7.Enabled = False
                                        Chk�w�N�ʏ�3_8.Enabled = False
                                        Chk�w�N�ʏ�3_9.Enabled = False
                                        Chk�w�N�ʏ�3_�S.Enabled = False
                                    End If
                                ElseIf .FURI_KBN_S = "1" Then
                                    '�U�֓��ݒ�
                                    Chk�L���ĐU���ʏ�3.Checked = True
                                    txt�ʏ�ĐU��3.Text = .YOBI1_S.Substring(6, 2)
                                    lbl�ʏ�ĐU��3.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    '�X�V�\���ݒ�
                                    If FlgEnable = True Then
                                        Chk�L���ĐU���ʏ�3.Enabled = False
                                        txt�ʏ�ĐU��3.Enabled = False
                                    End If
                                Else
                                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & .FURI_KBN_S)
                                    '�G���[
                                    Exit Try
                                End If
                            Case 4
                                '���U���ĐU
                                If .FURI_KBN_S = "0" Then
                                    '�U�֓��ݒ�
                                    Chk�L���U�֓��ʏ�4.Checked = True
                                    txt�ʏ�U�֓�4.Text = .YOBI1_S.Substring(6, 2)
                                    lbl�ʏ�U�֓�4.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    If FlgGakunenAll = True Then
                                        Chk�w�N�ʏ�4_�S.Checked = True
                                    Else
                                        Chk�w�N�ʏ�4_1.Checked = GakunenFlg(1)
                                        Chk�w�N�ʏ�4_2.Checked = GakunenFlg(2)
                                        Chk�w�N�ʏ�4_3.Checked = GakunenFlg(3)
                                        Chk�w�N�ʏ�4_4.Checked = GakunenFlg(4)
                                        Chk�w�N�ʏ�4_5.Checked = GakunenFlg(5)
                                        Chk�w�N�ʏ�4_6.Checked = GakunenFlg(6)
                                        Chk�w�N�ʏ�4_7.Checked = GakunenFlg(7)
                                        Chk�w�N�ʏ�4_8.Checked = GakunenFlg(8)
                                        Chk�w�N�ʏ�4_9.Checked = GakunenFlg(9)
                                    End If
                                    '�X�V�\���ݒ�
                                    If FlgEnable = True Then
                                        Chk�L���U�֓��ʏ�4.Enabled = False
                                        txt�ʏ�U�֓�4.Enabled = False
                                        Chk�w�N�ʏ�4_1.Enabled = False
                                        Chk�w�N�ʏ�4_2.Enabled = False
                                        Chk�w�N�ʏ�4_3.Enabled = False
                                        Chk�w�N�ʏ�4_4.Enabled = False
                                        Chk�w�N�ʏ�4_5.Enabled = False
                                        Chk�w�N�ʏ�4_6.Enabled = False
                                        Chk�w�N�ʏ�4_7.Enabled = False
                                        Chk�w�N�ʏ�4_8.Enabled = False
                                        Chk�w�N�ʏ�4_9.Enabled = False
                                        Chk�w�N�ʏ�4_�S.Enabled = False
                                    End If
                                ElseIf .FURI_KBN_S = "1" Then
                                    '�U�֓��ݒ�
                                    Chk�L���ĐU���ʏ�4.Checked = True
                                    txt�ʏ�ĐU��4.Text = .YOBI1_S.Substring(6, 2)
                                    lbl�ʏ�ĐU��4.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    '�X�V�\���ݒ�
                                    If FlgEnable = True Then
                                        Chk�L���ĐU���ʏ�4.Enabled = False
                                        txt�ʏ�ĐU��4.Enabled = False
                                    End If
                                Else
                                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & .FURI_KBN_S)
                                    '�G���[
                                    Exit Try
                                End If
                            Case 5
                                '���U���ĐU
                                If .FURI_KBN_S = "0" Then
                                    '�U�֓��ݒ�
                                    Chk�L���U�֓��ʏ�5.Checked = True
                                    txt�ʏ�U�֓�5.Text = .YOBI1_S.Substring(6, 2)
                                    lbl�ʏ�U�֓�5.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    If FlgGakunenAll = True Then
                                        Chk�w�N�ʏ�5_�S.Checked = True
                                    Else
                                        Chk�w�N�ʏ�5_1.Checked = GakunenFlg(1)
                                        Chk�w�N�ʏ�5_2.Checked = GakunenFlg(2)
                                        Chk�w�N�ʏ�5_3.Checked = GakunenFlg(3)
                                        Chk�w�N�ʏ�5_4.Checked = GakunenFlg(4)
                                        Chk�w�N�ʏ�5_5.Checked = GakunenFlg(5)
                                        Chk�w�N�ʏ�5_6.Checked = GakunenFlg(6)
                                        Chk�w�N�ʏ�5_7.Checked = GakunenFlg(7)
                                        Chk�w�N�ʏ�5_8.Checked = GakunenFlg(8)
                                        Chk�w�N�ʏ�5_9.Checked = GakunenFlg(9)
                                    End If
                                    '�X�V�\���ݒ�
                                    If FlgEnable = True Then
                                        Chk�L���U�֓��ʏ�5.Enabled = False
                                        txt�ʏ�U�֓�5.Enabled = False
                                        Chk�w�N�ʏ�5_1.Enabled = False
                                        Chk�w�N�ʏ�5_2.Enabled = False
                                        Chk�w�N�ʏ�5_3.Enabled = False
                                        Chk�w�N�ʏ�5_4.Enabled = False
                                        Chk�w�N�ʏ�5_5.Enabled = False
                                        Chk�w�N�ʏ�5_6.Enabled = False
                                        Chk�w�N�ʏ�5_7.Enabled = False
                                        Chk�w�N�ʏ�5_8.Enabled = False
                                        Chk�w�N�ʏ�5_9.Enabled = False
                                        Chk�w�N�ʏ�5_�S.Enabled = False
                                    End If
                                ElseIf .FURI_KBN_S = "1" Then
                                    '�U�֓��ݒ�
                                    Chk�L���ĐU���ʏ�5.Checked = True
                                    txt�ʏ�ĐU��5.Text = .YOBI1_S.Substring(6, 2)
                                    lbl�ʏ�ĐU��5.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    '�X�V�\���ݒ�
                                    If FlgEnable = True Then
                                        Chk�L���ĐU���ʏ�5.Enabled = False
                                        txt�ʏ�ĐU��5.Enabled = False
                                    End If
                                Else
                                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & .FURI_KBN_S)
                                    '�G���[
                                    Exit Try
                                End If

                            Case Else
                                Call GSUB_LOG(0, "�ʏ�X�P�W���[���J�E���^�ُ�")
                                Exit Try
                        End Select

                    Case "2" '����
                        Select Case Cnt '�Z�b�g����ꏊ
                            Case 1
                                '�������o��
                                If .FURI_KBN_S = "2" Then
                                    cmb���o�敪1.SelectedIndex = 0 '����
                                ElseIf .FURI_KBN_S = "3" Then
                                    cmb���o�敪1.SelectedIndex = 1  '�o��
                                Else
                                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & .FURI_KBN_S)
                                    '�G���[
                                    Exit Try
                                End If

                                '�U�֓��ݒ�
                                txt�����U�֓�1.Text = .YOBI1_S.Substring(6, 2)
                                lbl�����U�֓�1.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                    .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                    .FURI_DATE_S.Substring(6)
                                If FlgGakunenAll = True Then
                                    Chk����1_�S�w�N.Checked = True
                                Else
                                    Chk����1_1�w�N.Checked = GakunenFlg(1)
                                    Chk����1_2�w�N.Checked = GakunenFlg(2)
                                    Chk����1_3�w�N.Checked = GakunenFlg(3)
                                    Chk����1_4�w�N.Checked = GakunenFlg(4)
                                    Chk����1_5�w�N.Checked = GakunenFlg(5)
                                    Chk����1_6�w�N.Checked = GakunenFlg(6)
                                    Chk����1_7�w�N.Checked = GakunenFlg(7)
                                    Chk����1_8�w�N.Checked = GakunenFlg(8)
                                    Chk����1_9�w�N.Checked = GakunenFlg(9)
                                End If
                                '�X�V�\���ݒ�
                                If FlgEnable = True Then
                                    cmb���o�敪1.Enabled = False
                                    txt�����U�֓�1.Enabled = False
                                    Chk����1_1�w�N.Enabled = False
                                    Chk����1_2�w�N.Enabled = False
                                    Chk����1_3�w�N.Enabled = False
                                    Chk����1_4�w�N.Enabled = False
                                    Chk����1_5�w�N.Enabled = False
                                    Chk����1_6�w�N.Enabled = False
                                    Chk����1_7�w�N.Enabled = False
                                    Chk����1_8�w�N.Enabled = False
                                    Chk����1_9�w�N.Enabled = False
                                    Chk����1_�S�w�N.Enabled = False
                                End If
                            Case 2
                                '�������o��
                                If .FURI_KBN_S = "2" Then
                                    cmb���o�敪2.SelectedIndex = 0 '����
                                ElseIf .FURI_KBN_S = "3" Then
                                    cmb���o�敪2.SelectedIndex = 1  '�o��
                                Else
                                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & .FURI_KBN_S)
                                    '�G���[
                                    Exit Try
                                End If

                                '�U�֓��ݒ�
                                txt�����U�֓�2.Text = .YOBI1_S.Substring(6, 2)
                                lbl�����U�֓�2.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                    .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                    .FURI_DATE_S.Substring(6)
                                If FlgGakunenAll = True Then
                                    Chk����2_�S�w�N.Checked = True
                                Else
                                    Chk����2_1�w�N.Checked = GakunenFlg(1)
                                    Chk����2_2�w�N.Checked = GakunenFlg(2)
                                    Chk����2_3�w�N.Checked = GakunenFlg(3)
                                    Chk����2_4�w�N.Checked = GakunenFlg(4)
                                    Chk����2_5�w�N.Checked = GakunenFlg(5)
                                    Chk����2_6�w�N.Checked = GakunenFlg(6)
                                    Chk����2_7�w�N.Checked = GakunenFlg(7)
                                    Chk����2_8�w�N.Checked = GakunenFlg(8)
                                    Chk����2_9�w�N.Checked = GakunenFlg(9)
                                End If
                                '�X�V�\���ݒ�
                                If FlgEnable = True Then
                                    cmb���o�敪2.Enabled = False
                                    txt�����U�֓�2.Enabled = False
                                    Chk����2_1�w�N.Enabled = False
                                    Chk����2_2�w�N.Enabled = False
                                    Chk����2_3�w�N.Enabled = False
                                    Chk����2_4�w�N.Enabled = False
                                    Chk����2_5�w�N.Enabled = False
                                    Chk����2_6�w�N.Enabled = False
                                    Chk����2_7�w�N.Enabled = False
                                    Chk����2_8�w�N.Enabled = False
                                    Chk����2_9�w�N.Enabled = False
                                    Chk����2_�S�w�N.Enabled = False
                                End If
                            Case 3
                                '�������o��
                                If .FURI_KBN_S = "2" Then
                                    cmb���o�敪1.SelectedIndex = 0 '����
                                ElseIf .FURI_KBN_S = "3" Then
                                    cmb���o�敪1.SelectedIndex = 1  '�o��
                                Else
                                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & .FURI_KBN_S)
                                    '�G���[
                                    Exit Try
                                End If

                                '�U�֓��ݒ�
                                txt�����U�֓�3.Text = .YOBI1_S.Substring(6, 2)
                                lbl�����U�֓�3.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                    .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                    .FURI_DATE_S.Substring(6)
                                If FlgGakunenAll = True Then
                                    Chk����3_�S�w�N.Checked = True
                                Else
                                    Chk����3_1�w�N.Checked = GakunenFlg(1)
                                    Chk����3_2�w�N.Checked = GakunenFlg(2)
                                    Chk����3_3�w�N.Checked = GakunenFlg(3)
                                    Chk����3_4�w�N.Checked = GakunenFlg(4)
                                    Chk����3_5�w�N.Checked = GakunenFlg(5)
                                    Chk����3_6�w�N.Checked = GakunenFlg(6)
                                    Chk����3_7�w�N.Checked = GakunenFlg(7)
                                    Chk����3_8�w�N.Checked = GakunenFlg(8)
                                    Chk����3_9�w�N.Checked = GakunenFlg(9)
                                End If
                                '�X�V�\���ݒ�
                                If FlgEnable = True Then
                                    cmb���o�敪3.Enabled = False
                                    txt�����U�֓�3.Enabled = False
                                    Chk����3_1�w�N.Enabled = False
                                    Chk����3_2�w�N.Enabled = False
                                    Chk����3_3�w�N.Enabled = False
                                    Chk����3_4�w�N.Enabled = False
                                    Chk����3_5�w�N.Enabled = False
                                    Chk����3_6�w�N.Enabled = False
                                    Chk����3_7�w�N.Enabled = False
                                    Chk����3_8�w�N.Enabled = False
                                    Chk����3_9�w�N.Enabled = False
                                    Chk����3_�S�w�N.Enabled = False
                                End If
                            Case 4
                                '�������o��
                                If .FURI_KBN_S = "2" Then
                                    cmb���o�敪4.SelectedIndex = 0 '����
                                ElseIf .FURI_KBN_S = "3" Then
                                    cmb���o�敪4.SelectedIndex = 1  '�o��
                                Else
                                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & .FURI_KBN_S)
                                    '�G���[
                                    Exit Try
                                End If

                                '�U�֓��ݒ�
                                txt�����U�֓�4.Text = .YOBI1_S.Substring(6, 2)
                                lbl�����U�֓�4.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                    .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                    .FURI_DATE_S.Substring(6)
                                If FlgGakunenAll = True Then
                                    Chk����4_�S�w�N.Checked = True
                                Else
                                    Chk����4_1�w�N.Checked = GakunenFlg(1)
                                    Chk����4_2�w�N.Checked = GakunenFlg(2)
                                    Chk����4_3�w�N.Checked = GakunenFlg(3)
                                    Chk����4_4�w�N.Checked = GakunenFlg(4)
                                    Chk����4_5�w�N.Checked = GakunenFlg(5)
                                    Chk����4_6�w�N.Checked = GakunenFlg(6)
                                    Chk����4_7�w�N.Checked = GakunenFlg(7)
                                    Chk����4_8�w�N.Checked = GakunenFlg(8)
                                    Chk����4_9�w�N.Checked = GakunenFlg(9)
                                End If
                                '�X�V�\���ݒ�
                                If FlgEnable = True Then
                                    cmb���o�敪4.Enabled = False
                                    txt�����U�֓�4.Enabled = False
                                    Chk����4_1�w�N.Enabled = False
                                    Chk����4_2�w�N.Enabled = False
                                    Chk����4_3�w�N.Enabled = False
                                    Chk����4_4�w�N.Enabled = False
                                    Chk����4_5�w�N.Enabled = False
                                    Chk����4_6�w�N.Enabled = False
                                    Chk����4_7�w�N.Enabled = False
                                    Chk����4_8�w�N.Enabled = False
                                    Chk����4_9�w�N.Enabled = False
                                    Chk����4_�S�w�N.Enabled = False
                                End If
                            Case 5
                                '�������o��
                                If .FURI_KBN_S = "2" Then
                                    cmb���o�敪5.SelectedIndex = 0 '����
                                ElseIf .FURI_KBN_S = "3" Then
                                    cmb���o�敪5.SelectedIndex = 1  '�o��
                                Else
                                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & .FURI_KBN_S)
                                    '�G���[
                                    Exit Try
                                End If

                                '�U�֓��ݒ�
                                txt�����U�֓�5.Text = .YOBI1_S.Substring(6, 2)
                                lbl�����U�֓�5.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                    .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                    .FURI_DATE_S.Substring(6)
                                If FlgGakunenAll = True Then
                                    Chk����5_�S�w�N.Checked = True
                                Else
                                    Chk����5_1�w�N.Checked = GakunenFlg(1)
                                    Chk����5_2�w�N.Checked = GakunenFlg(2)
                                    Chk����5_3�w�N.Checked = GakunenFlg(3)
                                    Chk����5_4�w�N.Checked = GakunenFlg(4)
                                    Chk����5_5�w�N.Checked = GakunenFlg(5)
                                    Chk����5_6�w�N.Checked = GakunenFlg(6)
                                    Chk����5_7�w�N.Checked = GakunenFlg(7)
                                    Chk����5_8�w�N.Checked = GakunenFlg(8)
                                    Chk����5_9�w�N.Checked = GakunenFlg(9)
                                End If
                                '�X�V�\���ݒ�
                                If FlgEnable = True Then
                                    cmb���o�敪5.Enabled = False
                                    txt�����U�֓�5.Enabled = False
                                    Chk����5_1�w�N.Enabled = False
                                    Chk����5_2�w�N.Enabled = False
                                    Chk����5_3�w�N.Enabled = False
                                    Chk����5_4�w�N.Enabled = False
                                    Chk����5_5�w�N.Enabled = False
                                    Chk����5_6�w�N.Enabled = False
                                    Chk����5_7�w�N.Enabled = False
                                    Chk����5_8�w�N.Enabled = False
                                    Chk����5_9�w�N.Enabled = False
                                    Chk����5_�S�w�N.Enabled = False
                                End If
                            Case 6
                                '�������o��
                                If .FURI_KBN_S = "2" Then
                                    cmb���o�敪6.SelectedIndex = 0 '����
                                ElseIf .FURI_KBN_S = "3" Then
                                    cmb���o�敪6.SelectedIndex = 1  '�o��
                                Else
                                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & .FURI_KBN_S)
                                    '�G���[
                                    Exit Try
                                End If

                                '�U�֓��ݒ�
                                txt�����U�֓�6.Text = .YOBI1_S.Substring(6, 2)
                                lbl�����U�֓�6.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                    .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                    .FURI_DATE_S.Substring(6)
                                If FlgGakunenAll = True Then
                                    Chk����6_�S�w�N.Checked = True
                                Else
                                    Chk����6_1�w�N.Checked = GakunenFlg(1)
                                    Chk����6_2�w�N.Checked = GakunenFlg(2)
                                    Chk����6_3�w�N.Checked = GakunenFlg(3)
                                    Chk����6_4�w�N.Checked = GakunenFlg(4)
                                    Chk����6_5�w�N.Checked = GakunenFlg(5)
                                    Chk����6_6�w�N.Checked = GakunenFlg(6)
                                    Chk����6_7�w�N.Checked = GakunenFlg(7)
                                    Chk����6_8�w�N.Checked = GakunenFlg(8)
                                    Chk����6_9�w�N.Checked = GakunenFlg(9)
                                End If
                                '�X�V�\���ݒ�
                                If FlgEnable = True Then
                                    cmb���o�敪6.Enabled = False
                                    txt�����U�֓�6.Enabled = False
                                    Chk����6_1�w�N.Enabled = False
                                    Chk����6_2�w�N.Enabled = False
                                    Chk����6_3�w�N.Enabled = False
                                    Chk����6_4�w�N.Enabled = False
                                    Chk����6_5�w�N.Enabled = False
                                    Chk����6_6�w�N.Enabled = False
                                    Chk����6_7�w�N.Enabled = False
                                    Chk����6_8�w�N.Enabled = False
                                    Chk����6_9�w�N.Enabled = False
                                    Chk����6_�S�w�N.Enabled = False
                                End If
                            Case Else
                                Call GSUB_LOG(0, "���������X�P�W���[�����ُ�")
                                Exit Try
                        End Select
                    Case Else
                        Call GSUB_LOG(0, "�X�P�W���[���敪�ُ�:" & .SCH_KBN_S)
                        Exit Try
                End Select
            End With

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ret = False
        End Try

        Return ret

    End Function
#End Region
#Region "UPDATE G_SCHMAST"
    '
    '�@�֐����@-�@fn_UpdateG_SCHMAST
    '
    '�@�@�\    -  �X�P�W���[���X�V
    '
    '�@����    -  aInfoGakkou
    '
    '�@���l    -  
    '
    '�@
    Private Function fn_UpdateG_SCHMAST(ByVal strGakkouCode As String) As Boolean

        Dim ret As Boolean = False

        Try
            '***���O���***
            STR_COMMAND = "fn_UpdateG_SCHMAST"
            STR_LOG_GAKKOU_CODE = strGakkouCode
            STR_LOG_FURI_DATE = ""
            '***���O���**

            '���g�����U�N�V�����J�n
            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Try
            End If

            '���w�Z���̎擾
            Dim Flg As Boolean = False
            Dim InfoGakkou As New GAKDATA(strGakkouCode, Flg, MainDB)

            If Flg = False Then
                ERRMSG = "�w�Z���̎擾�Ɏ��s���܂����B" & vbCrLf & "�w�Z�R�[�h:" & strGakkouCode
                Exit Try
            End If

            '����ʏ����\���̂ɃZ�b�g����(�X�V��)
            '���X�V�O���͎Q�Ǝ��ɃZ�b�g����Ă�����̂Ƃ���
            Call Sb_GetData(Tuujyou_SchInfo, Zuiji_SchInfo)

            '���ʏ�U�֓��^�u���͉�ʏ��̃`�F�b�N
            If fn_Check_TuujyouFuriDate(InfoGakkou, Tuujyou_SchInfo, False) = False Then
                Exit Try
            End If

            '�������U�֓��^�u���͉�ʏ��̃`�F�b�N
            If fn_Check_ZuijiFuriDate(InfoGakkou, Zuiji_SchInfo) = False Then
                Exit Try
            End If

            Dim SyoriFlg As Boolean = False

            '���������Ƃ̃}�b�`���O(�`�F�b�N���ʂɂ��֐������ŁAInsert�AUpdate�ADelete���s�Ȃ�)
            For i As Integer = 1 To 5 Step 1
                If Not fn_CompareTuujyouData(InfoGakkou, Syoki_Tuujyou_SchInfo(i), Tuujyou_SchInfo(i)) Then
                    Exit Try
                End If
            Next

            '���������Ƃ̃}�b�`���O(�`�F�b�N���ʂɂ��֐������ŁAInsert�AUpdate�ADelete���s�Ȃ�)
            For i As Integer = 1 To 6 Step 1
                If Not fn_CompareZuijiData(InfoGakkou, Syoki_Zuiji_SchInfo(i), Zuiji_SchInfo(i)) Then
                    Exit Try
                End If
            Next

            ret = True

        Catch ex As Exception
            ret = False
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ERRMSG = "�X�V�Ɏ��s���܂����B"
        Finally
            If ret = True Then
                '�g�����U�N�V�����I���iCOMMIT�j
                If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                    Call GSUB_LOG(0, "COMMIT���s")
                    ret = False
                End If
            Else
                '�g�����U�N�V�����I���iROLLBACK�j
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Call GSUB_LOG(0, "ROLLBACK���s")
                    ret = False
                End If
            End If
        End Try

        Return ret

    End Function

    '
    '�@�֐����@-�@fn_CompareTuujyouData
    '
    '�@�@�\    -  �X�P�W���[���X�V
    '
    '�@����    -  aSyoki_Tuujyou_SchInfo,aTuujyou_SchInfo
    '
    '  �߂�l�@-�@0:�������A1:�����L�A9:�������s
    '
    '�@���l    -  
    '
    '�@
    Private Function fn_CompareTuujyouData(ByVal aInfoGakkou As GAKDATA, _
                                        ByVal aSyoki_Tuujyou_SchInfo As TuujyouData, _
                                        ByVal aTuujyou_SchInfo As TuujyouData) As Boolean
        Dim ret As Boolean = False

        Try
            '�����Ώۃt���O(�������s�Ȃ��K�v�����邩�ǂ���)
            Dim SyoriFlg As Boolean = False

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '�Q�Ǝ��̃X�P�W���[�����Ƃ̔�r
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            With aSyoki_Tuujyou_SchInfo
                '���L���U�֓��`�F�b�N
                Select Case True
                    Case (.Furikae_Check = True AndAlso aTuujyou_SchInfo.Furikae_Check = True)
                        '�������肩�A�X�V�゠�聨Update

                        '���U���̃`�F�b�N
                        If .Furikae_Date <> aTuujyou_SchInfo.Furikae_Date Then
                            SyoriFlg = True
                        End If

                        '�Ώۊw�N�t���O�̃`�F�b�N
                        Dim SyokiGakunenFlg As String() = .GetGakunenFlg
                        Dim GakunenFlg As String() = aTuujyou_SchInfo.GetGakunenFlg

                        If SyokiGakunenFlg(0) = "err" OrElse GakunenFlg(0) = "err" Then
                            '�G���[���O�͓����ŏo��
                            ret = False
                            Exit Try
                        Else
                            For i As Integer = 1 To 9 Step 1
                                If SyokiGakunenFlg(i) <> GakunenFlg(i) Then
                                    SyoriFlg = True
                                    Exit For
                                End If
                            Next
                        End If
                        '�����U�X�P�W���[����UPDATE
                        If SyoriFlg = True Then
                            If Not UpDateTuujoyuG_SCHMAST(0, 0, aInfoGakkou, aSyoki_Tuujyou_SchInfo, aTuujyou_SchInfo) Then
                                Exit Try
                            End If
                        End If

                        '�L���ĐU���`�F�b�N
                        Select Case True
                            Case (.SaiFurikae_Check = True AndAlso aTuujyou_SchInfo.SaiFurikae_Check = True)
                                '�������肩�A�X�V�゠�聨Update

                                '�Ώۊw�N�t���O�̃`�F�b�N
                                SyokiGakunenFlg = .GetGakunenFlg
                                GakunenFlg = aTuujyou_SchInfo.GetGakunenFlg


                                '���ĐU�X�P�W���[����UPDATE
                                If Not UpDateTuujoyuG_SCHMAST(0, 1, aInfoGakkou, aSyoki_Tuujyou_SchInfo, aTuujyou_SchInfo) Then
                                    Exit Try
                                End If

                            Case (.SaiFurikae_Check = False AndAlso aTuujyou_SchInfo.SaiFurikae_Check = True)
                                '�����Ȃ����A�X�V�゠�聨Insert

                                '���ĐU�݂̂�Insert����
                                If Not fn_InsertTuujyouG_SCHMAST(aInfoGakkou, aTuujyou_SchInfo, True) Then
                                    Exit Try
                                End If

                                Dim Nengetsudo As String = .Seikyu_Nen & .Seikyu_Tuki
                                Dim SaiFuriDate As String = ""

                                Select Case True
                                    Case aTuujyou_SchInfo.Furikae_Date < aTuujyou_SchInfo.SaiFurikae_Date
                                        '�U�֓��Z�o
                                        SaiFuriDate = fn_GetFuriDate(aTuujyou_SchInfo.Seikyu_Nen, aTuujyou_SchInfo.Seikyu_Tuki, aTuujyou_SchInfo.SaiFurikae_Date, "0", "1", aInfoGakkou)
                                    Case aTuujyou_SchInfo.Furikae_Date > aTuujyou_SchInfo.SaiFurikae_Date
                                        If .Seikyu_Tuki = "12" Then
                                            '�U�֓��Z�o
                                            SaiFuriDate = fn_GetFuriDate(CStr(CInt(aTuujyou_SchInfo.Seikyu_Nen) + 1), "01", aTuujyou_SchInfo.SaiFurikae_Date, "0", "1", aInfoGakkou)
                                        Else
                                            '�U�֓��Z�o
                                            SaiFuriDate = fn_GetFuriDate(aTuujyou_SchInfo.Seikyu_Nen, Format(CInt(aTuujyou_SchInfo.Seikyu_Tuki) + 1, "00"), .SaiFurikae_Date, "0", "1", aInfoGakkou)
                                        End If

                                    Case aTuujyou_SchInfo.Furikae_Date = aTuujyou_SchInfo.SaiFurikae_Date
                                        Call GSUB_LOG(0, "�U�֓��ُ�:" & "1")
                                        ERRMSG = "�U�֓��ƍĐU��������ł�"
                                        Exit Try
                                End Select

                                '���ĐU�̂ݍ쐬�����ꍇ�͏��U�̍ĐU�����X�V
                                Dim SQL As String
                                SQL = " UPDATE  G_SCHMAST SET "
                                SQL &= " SFURI_DATE_S = '" & SaiFuriDate & "'"
                                SQL &= " WHERE GAKKOU_CODE_S = '" & aInfoGakkou.GAKKOU_CODE & "'"
                                SQL &= " AND   NENGETUDO_S = '" & Nengetsudo & "'"
                                SQL &= " AND   SCH_KBN_S = '0'"
                                SQL &= " AND   FURI_KBN_S = '0'"
                                SQL &= " AND   FURI_DATE_S = '" & .Furikae_Day & "'"

                                If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                                    Call GSUB_LOG(0, "�w�Z�X�P�W���[���X�V���s�ASQL:" & SQL)
                                    ERRMSG = "�w�Z�X�P�W���[���X�V�Ɏ��s���܂���"
                                    Exit Try
                                End If


                            Case (.SaiFurikae_Check = True AndAlso aTuujyou_SchInfo.SaiFurikae_Check = False)
                                '�������肩�A�X�V��Ȃ���Delete

                                Dim Nengetsudo As String = .Seikyu_Nen & .Seikyu_Tuki

                                '�ĐU�̂ݍ폜
                                If .SaiFurikae_Check = True Then
                                    If Not fn_DeleteG_SCHMAST(aInfoGakkou.GAKKOU_CODE, 0, 1, .SaiFurikae_Day, Nengetsudo) Then
                                        Exit Try
                                    End If

                                    '���ĐU�̂ݍ폜�����ꍇ�͏��U�̍ĐU�����X�V
                                    Dim SQL As String

                                    SQL = " UPDATE  G_SCHMAST SET "
                                    SQL &= " SFURI_DATE_S = '00000000' "
                                    SQL &= " WHERE GAKKOU_CODE_S = '" & aInfoGakkou.GAKKOU_CODE & "'"
                                    SQL &= " AND   NENGETUDO_S = '" & Nengetsudo & "'"
                                    SQL &= " AND   SCH_KBN_S = '0'"
                                    SQL &= " AND   FURI_KBN_S = '0'"
                                    SQL &= " AND   FURI_DATE_S = '" & .Furikae_Day & "'"

                                    If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                                        Call GSUB_LOG(0, "�w�Z�X�P�W���[���X�V���s�ASQL:" & SQL)
                                        ERRMSG = "�w�Z�X�P�W���[���X�V�Ɏ��s���܂���"
                                        Exit Try
                                    End If
                                End If
                            Case Else
                                '�����Ȃ����A�X�V��Ȃ����Ȃɂ����Ȃ�
                        End Select

                        ret = True

                    Case (.Furikae_Check = False AndAlso aTuujyou_SchInfo.Furikae_Check = True)
                        '�����Ȃ����A�X�V�゠�聨Insert

                        '���ʏ��Insert�Ɠ��ꏈ��
                        If Not fn_InsertTuujyouG_SCHMAST(aInfoGakkou, aTuujyou_SchInfo) Then
                            Exit Try
                        End If

                    Case (.Furikae_Check = True AndAlso aTuujyou_SchInfo.Furikae_Check = False)
                        '�������肩�A�X�V��Ȃ���Delete

                        Dim Nengetsudo As String = .Seikyu_Nen & .Seikyu_Tuki

                        '���U�̍폜
                        If Not fn_DeleteG_SCHMAST(aInfoGakkou.GAKKOU_CODE, 0, 0, .Furikae_Day, Nengetsudo) Then
                            Exit Try
                        End If

                        '�����X�P�W���[���ɍĐU�`�F�b�N������΁A�ĐU���폜
                        If .SaiFurikae_Check = True Then
                            If Not fn_DeleteG_SCHMAST(aInfoGakkou.GAKKOU_CODE, 0, 1, .SaiFurikae_Day, Nengetsudo) Then
                                Exit Try
                            End If
                        End If

                    Case Else
                        '�����Ȃ����A�X�V��Ȃ����Ȃɂ����Ȃ�
                End Select
            End With

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ERRMSG = "�w�Z�X�P�W���[���̍X�V�Ɏ��s���܂���"
        End Try

        Return ret

    End Function
    '
    '�@�֐����@-�@UpDateTuujoyuG_SCHMAST
    '
    '�@�@�\    -  �X�P�W���[���X�V
    '
    '�@����    -  aInfoGakkou�AaSyoki_Tuujyou_SchInfo�AaTuujyou_SchInfo
    '
    '  �߂�l�@-�@0:�������A1:�����L�A9:�������s
    '
    '�@���l    -  
    '
    '�@
    Private Function UpDateTuujoyuG_SCHMAST(ByVal aFurikbn As Integer, _
                                            ByVal aSchkbn As Integer, _
                                            ByVal aInfoGakkou As GAKDATA, _
                                            ByVal aSyoki_Tuujyou_SchInfo As TuujyouData, _
                                            ByVal aTuujyou_SchInfo As TuujyouData) As Boolean

        Dim ret As Boolean = False

        Try
            '�������t���擾
            Dim SyoriDate(1) As String
            SyoriDate(0) = Format(Now, "yyyyMMdd")
            SyoriDate(1) = Format(Now, "yyyyMMddHHmmss")

            '�w�N�t���O�̎擾
            Dim GakunenFlg As String() = aTuujyou_SchInfo.GetGakunenFlg()
            If GakunenFlg(0) = "err" Then

                ERRMSG = "�w�N�Ώۃt���O�̎擾�Ɏ��s���܂���"
                Exit Try
            End If

            '���͂��ꂽ�U�֓�(�c�Ɠ����l�����Ȃ�)
            Dim EntryFuriDate As String = "00000000"
            Dim FuriDate As String = "00000000"
            Dim SaiFuriDate As String = "00000000"

            '�����̐U�֋敪�A�X�P�W���[���敪���Z�b�g
            Dim Schkbn As Integer = aFurikbn
            Dim Furikbn As Integer = aSchkbn

            Dim NENGETDO As String = aSyoki_Tuujyou_SchInfo.Seikyu_Nen & aSyoki_Tuujyou_SchInfo.Seikyu_Tuki

            '�U�֓��̒��o
            With aTuujyou_SchInfo
                Select Case Furikbn
                    Case 0
                        '���U�p�̐ݒ�
                        EntryFuriDate = .Seikyu_Nen & .Seikyu_Tuki & .Furikae_Date
                        '�U�֓��Z�o
                        FuriDate = fn_GetFuriDate(.Seikyu_Nen, .Seikyu_Tuki, .Furikae_Date, Schkbn, Furikbn, aInfoGakkou)

                        '�ĐU���̔N�̊m�菈��
                        If .SaiFurikae_Date <> "" Then
                            Select Case True
                                Case .Furikae_Date < .SaiFurikae_Date
                                    '�U�֓��Z�o
                                    SaiFuriDate = fn_GetFuriDate(.Seikyu_Nen, .Seikyu_Tuki, .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                Case .Furikae_Date > .SaiFurikae_Date
                                    If .Seikyu_Tuki = "12" Then
                                        '�U�֓��Z�o
                                        SaiFuriDate = fn_GetFuriDate(CStr(CInt(.Seikyu_Nen) + 1), "01", .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                    Else
                                        '�U�֓��Z�o
                                        SaiFuriDate = fn_GetFuriDate(.Seikyu_Nen, Format(CInt(.Seikyu_Tuki) + 1, "00"), .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                    End If

                                Case .Furikae_Date = .SaiFurikae_Date
                                    Call GSUB_LOG(0, "�U�֓��ُ�:" & Furikbn)
                                    ERRMSG = "�U�֓��ƍĐU��������ł�"
                                    Exit Try
                            End Select

                        Else
                            SaiFuriDate = "00000000"
                        End If
                    Case 1
                        '��������
                        Select Case True
                            Case .Furikae_Date < .SaiFurikae_Date
                                '�ĐU�p�̐ݒ�
                                EntryFuriDate = .Seikyu_Nen & .Seikyu_Tuki & .SaiFurikae_Date
                                '�U�֓��Z�o
                                FuriDate = fn_GetFuriDate(.Seikyu_Nen, .Seikyu_Tuki, .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)

                            Case .Furikae_Date > .SaiFurikae_Date
                                If .Seikyu_Tuki = "12" Then
                                    '�ĐU�p�̐ݒ�
                                    EntryFuriDate = CStr(CInt(.Seikyu_Nen) + 1) & "01" & .SaiFurikae_Date
                                    '�U�֓��Z�o
                                    FuriDate = fn_GetFuriDate(CStr(CInt(.Seikyu_Nen) + 1), "01", .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                Else
                                    '�ĐU�p�̐ݒ�
                                    EntryFuriDate = .Seikyu_Nen & Format(CInt(.Seikyu_Tuki) + 1, "00") & .SaiFurikae_Date
                                    '�U�֓��Z�o
                                    FuriDate = fn_GetFuriDate(.Seikyu_Nen, Format(CInt(.Seikyu_Tuki) + 1, "00"), .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                End If

                            Case .Furikae_Date = .SaiFurikae_Date
                                Call GSUB_LOG(0, "�U�֓��ُ�:" & Furikbn)
                                ERRMSG = "�U�֓��ƍĐU��������ł�"
                                Exit Try
                        End Select

                        '���z����̏ꍇ�̂ݍĐU����ݒ�
                        If aInfoGakkou.SFURI_SYUBETU = "3" Then '�ĐU����A���z����
                            '��������12���Ȃ痂�N��1��
                            If .Seikyu_Tuki = "12" Then
                                .Seikyu_Nen = (CInt(.Seikyu_Nen) + 1).ToString
                                .Seikyu_Tuki = "01"
                            Else
                                .Seikyu_Tuki = (CInt(.Seikyu_Tuki) + 1).ToString
                            End If
                            '�c�Ɠ��Z�o
                            SaiFuriDate = fn_GetEigyoubi(.Seikyu_Nen & .Seikyu_Tuki & .SaiFurikae_Date, "0", "+")
                        Else
                            SaiFuriDate = "00000000"
                        End If

                    Case Else
                        Call GSUB_LOG(0, "�U�֋敪�ُ�:" & Furikbn)
                        ERRMSG = "�w�Z���U�X�P�W���[���쐬�Ɏ��s���܂���"
                        Exit Try
                End Select
            End With

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '�e��\����̎Z�o
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            Dim CLS As New MAIN.ClsSchduleMaintenanceClass
            '2012/09/04 saitou �x������ MODIFY -------------------------------------------------->>>>
            CLS.SetSchTable = MAIN.ClsSchduleMaintenanceClass.APL.JifuriApplication
            'CLS.SetSchTable = CLS.APL.JifuriApplication
            '2012/09/04 saitou �x������ MODIFY --------------------------------------------------<<<<

            '�X�P�W���[���쐬�Ώۂ̎����R�[�h�𒊏o
            CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(FuriDate), aInfoGakkou.GAKKOU_CODE, "01")

            CLS.SCH.FURI_DATE = GCom.SET_DATE(FuriDate)
            If CLS.SCH.FURI_DATE = "00000000" Then
            Else
                CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
            End If

            Dim strFURI_DATE As String = CLS.SCH.FURI_DATE

            Dim BRet As Boolean = CLS.INSERT_NEW_SCHMAST(0, False, True)

            Dim ENTRY_Y_DATE As String = "00000000"                                                   '���׍쐬�\����Z�o
            Dim CHECK_Y_DATE As String = fn_GetEigyoubi(FuriDate, STR_JIFURI_CHECK, "-")         '�`�F�b�N�\����Z�o
            Dim DATA_Y_DATE As String = fn_GetEigyoubi(FuriDate, STR_JIFURI_HAISIN, "-")    '�U�փf�[�^�쐬�\����Z�o
            Dim FUNOU_Y_DATE As String = CLS.SCH.FUNOU_YDATE                                          '�s�\���ʍX�V�\����Z�o
            Dim KESSAI_Y_DATE As String = CLS.SCH.KESSAI_YDATE                                        '�������ϗ\����Z�o

            Dim TESSU_YDATE As String = CLS.SCH.TESUU_YDATE


            Dim SQL As String = ""

            SQL = "  UPDATE G_SCHMAST SET "

            SQL &= " FURI_DATE_S ='" & FuriDate & "'"         '5.�U�֓�
            SQL &= ",SFURI_DATE_S ='" & SaiFuriDate & "'"     '6.�ĐU��
            '�Ώۊw�N�t���O
            SQL &= ",GAKUNEN1_FLG_S ='" & GakunenFlg(1) & "'" '7.1�N
            SQL &= ",GAKUNEN2_FLG_S ='" & GakunenFlg(2) & "'" '8.2�N
            SQL &= ",GAKUNEN3_FLG_S ='" & GakunenFlg(3) & "'" '9.3�N
            SQL &= ",GAKUNEN4_FLG_S ='" & GakunenFlg(4) & "'" '10.4�N
            SQL &= ",GAKUNEN5_FLG_S ='" & GakunenFlg(5) & "'" '11.5�N
            SQL &= ",GAKUNEN6_FLG_S ='" & GakunenFlg(6) & "'" '12.6�N
            SQL &= ",GAKUNEN7_FLG_S ='" & GakunenFlg(7) & "'" '13.7�N
            SQL &= ",GAKUNEN8_FLG_S ='" & GakunenFlg(8) & "'" '14.8�N
            SQL &= ",GAKUNEN9_FLG_S ='" & GakunenFlg(9) & "'" '15.9�N

            '�e���t
            SQL &= ",ENTRI_YDATE_S = '" & ENTRY_Y_DATE & "'"   '21.�G���g���\���
            SQL &= ",ENTRI_DATE_S = '00000000'"                '22.�G���g����
            SQL &= ",CHECK_YDATE_S = '" & CHECK_Y_DATE & "'"   '23.�`�F�b�N�\���
            SQL &= ",CHECK_DATE_S = '00000000'"                '24.�`�F�b�N��
            SQL &= ",DATA_YDATE_S = '" & DATA_Y_DATE & "'"     '25.�f�[�^�\���
            SQL &= ",DATA_DATE_S = '00000000'"                 '26.�f�[�^��
            SQL &= ",FUNOU_YDATE_S = '" & FUNOU_Y_DATE & "'"   '27.�s�\�X�V�\���
            SQL &= ",FUNOU_DATE_S = '00000000'"                '28.�s�\�X�V��
            SQL &= ",KESSAI_YDATE_S = '" & KESSAI_Y_DATE & "'" '31.���ϗ\���
            SQL &= ",KESSAI_DATE_S = '00000000'"               '32.���ϓ�

            SQL &= ",YOBI1_S ='" & fn_Hosei(EntryFuriDate) & "'"         '51.�\��1(���͓��t)

            '����������
            SQL &= " WHERE GAKKOU_CODE_S = '" & aInfoGakkou.GAKKOU_CODE & "'"          '�w�Z�R�[�h
            SQL &= " AND   NENGETUDO_S = '" & NENGETDO & "'"                           '�N���x
            SQL &= " AND   SCH_KBN_S = '" & Schkbn & "'"                               '�X�P�W���[���敪
            SQL &= " AND   FURI_KBN_S = '" & Furikbn & "'"                             '�U�֋敪

            If Furikbn = 0 Then
                SQL &= " AND   FURI_DATE_S = '" & aSyoki_Tuujyou_SchInfo.Furikae_Day & "'"    '�U�֓�
            Else
                SQL &= " AND   FURI_DATE_S = '" & aSyoki_Tuujyou_SchInfo.SaiFurikae_Day & "'" '�U�֓�
            End If

            If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                Call GSUB_LOG(0, "�w�Z�X�P�W���[��Update���s�ASQL:" & SQL)
                ERRMSG = "�w�Z�X�P�W���[���X�V�Ɏ��s���܂���"
                Exit Try
            End If

            '���ĐU�̏ꍇ�͊w�Z���U�X�P�W���[���̍ĐU�����X�V����
            If Furikbn = 1 Then
                Dim UpdateSch_FuriDate As String

                With aTuujyou_SchInfo
                    UpdateSch_FuriDate = fn_GetFuriDate(.Seikyu_Nen, .Seikyu_Tuki, .Furikae_Date, Schkbn, Furikbn, aInfoGakkou)

                    SQL = "  UPDATE G_SCHMAST SET "
                    SQL &= " SFURI_DATE_S = '" & FuriDate & "'"
                    SQL &= " WHERE GAKKOU_CODE_S = '" & aInfoGakkou.GAKKOU_CODE & "'"
                    SQL &= " AND   NENGETUDO_S = '" & NENGETDO & "'"
                    SQL &= " AND   SCH_KBN_S = '0'"
                    SQL &= " AND   FURI_KBN_S = '0'"
                    SQL &= " AND   FURI_DATE_S = '" & UpdateSch_FuriDate & "'"

                    If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                        Call GSUB_LOG(0, "�w�Z�X�P�W���[��Update���s�ASQL:" & SQL)
                        ERRMSG = "�w�Z�X�P�W���[���X�V�Ɏ��s���܂���"
                        Exit Try
                    End If

                End With
            End If

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '��Ǝ��U�X�P�W���[����UPDATE
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            Dim ToriS_Code As String = aInfoGakkou.GAKKOU_CODE
            Dim ToriF_Code As String

            '�U�֓��ɑ΂��Ĉ˗�������敪���Z�o
            Dim KASIYU_DATE_S As String = ""



            KASIYU_DATE_S = fn_GetEigyoubi(FuriDate, STR_JIFURI_KAISYU, "-")

            Dim KigyouFuriDate As String

            Select Case Furikbn
                Case 0 '���U
                    ToriF_Code = "01"
                    KigyouFuriDate = aSyoki_Tuujyou_SchInfo.Furikae_Day
                Case 1 '�ĐU
                    ToriF_Code = "02"
                    KigyouFuriDate = aSyoki_Tuujyou_SchInfo.SaiFurikae_Day
                Case 2 '��������
                    ToriF_Code = "03"
                    KigyouFuriDate = aSyoki_Tuujyou_SchInfo.Furikae_Day
                Case 3 '�����o��
                    ToriF_Code = "04"
                    KigyouFuriDate = aSyoki_Tuujyou_SchInfo.Furikae_Day
                Case Else
                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & Furikbn)
                    ERRMSG = "��Ǝ��U�̃X�P�W���[���X�V�Ɏ��s���܂���"
                    Exit Try
            End Select

            '�X�P�W���[���̑��݃`�F�b�N
            If fn_SchMastIsExist(ToriS_Code, ToriF_Code, KigyouFuriDate, OBJ_CONNECTION, OBJ_TRANSACTION) Then

                SQL = "  UPDATE SCHMAST SET "
                SQL &= " FURI_DATE_S = '" & "" & FuriDate & "'"
                SQL &= ",KFURI_DATE_S = '" & "" & fn_Hosei(EntryFuriDate) & "'"
                '��Ǝ��U���ł̃X�P�W���[���ɍĐU���͐ݒ肵�Ȃ�
                SQL &= ",SAIFURI_DATE_S = '00000000'"
                SQL &= ",KSAIFURI_DATE_S = '" & "" & SaiFuriDate & "'"
                '�U�֓�����Z�o�����˗�����������Z�b�g
                SQL &= ",IRAISYOK_YDATE_S  = '" & KASIYU_DATE_S & "'"
                '���׍쐬�\������Z�b�g
                SQL &= ",HAISIN_YDATE_S = '" & DATA_Y_DATE & "'"
                '���M�\������Z�b�g
                SQL &= ",SOUSIN_YDATE_S = '" & DATA_Y_DATE & "'"
                '�s�\�X�V�\������Z�b�g
                SQL &= ",FUNOU_YDATE_S = '" & FUNOU_Y_DATE & "'"
                '���ϗ\������Z�b�g
                SQL &= ",KESSAI_YDATE_S = '" & KESSAI_Y_DATE & "'"
                '�萔���\������Z�b�g
                SQL &= ",TESUU_YDATE_S = '" & TESSU_YDATE & "'"
                '�s�\�X�V�\������Z�b�g
                SQL &= ",HENKAN_YDATE_S = '" & FUNOU_Y_DATE & "'"

                SQL &= " WHERE TORIS_CODE_S = '" & ToriS_Code & "'"
                SQL &= " AND TORIF_CODE_S = '" & ToriF_Code & "'"
                SQL &= " AND FURI_DATE_S = '" & KigyouFuriDate & "'"

                If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                    Call GSUB_LOG(0, "���U�X�P�W���[��Update���s�ASQL:" & SQL)
                    ERRMSG = "��Ǝ��U�X�P�W���[���X�V�Ɏ��s���܂���"
                    Exit Try
                End If
            Else
                Call GSUB_LOG(0, "��Ǝ��U�X�P�W���[���擾���s�A�U�֓�:" & FuriDate)
                ERRMSG = "��Ǝ��U�X�P�W���[���擾�Ɏ��s���܂���"
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ERRMSG = "�w�Z�X�P�W���[���X�V�Ɏ��s���܂���"
            Exit Try
        End Try

        Return ret

    End Function


    '
    '�@�֐����@-�@fn_CompareZuijiData
    '
    '�@�@�\    -  �X�P�W���[���X�V
    '
    '�@����    -  aSyoki_Zuiji_SchInfo,aZuiji_SchInfo
    '
    '  �߂�l�@-�@0:�������A1:�����L�A9:�������s
    '
    '�@���l    -  
    '
    '�@
    Private Function fn_CompareZuijiData(ByVal aInfoGakkou As GAKDATA, _
                                        ByVal aSyoki_Zuiji_SchInfo As ZuijiData, _
                                        ByVal aZuiji_SchInfo As ZuijiData) As Boolean

        Dim ret As Boolean = False

        Try
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '�Q�Ǝ��̃X�P�W���[�����Ƃ̔�r
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            With aSyoki_Zuiji_SchInfo
                '���L���U�֓��`�F�b�N
                If .Furikae_Date = "" Then
                    If aZuiji_SchInfo.Furikae_Date = "" Then
                        ret = True
                        Exit Try
                    Else
                        '***�Ȃ�������:Insert***
                        If Not fn_InsertZuijiG_SCHMAST(aInfoGakkou, aZuiji_SchInfo) Then
                            Exit Try
                        End If
                    End If
                Else
                    If aZuiji_SchInfo.Furikae_Date = "" Then
                        '***���聨�Ȃ�:Delete***
                        Dim Furikbn As Integer
                        Dim Nengetsudo As String = .Furikae_Nen & .Furikae_Tuki

                        '�X�P�W���[���敪�̔���
                        If aSyoki_Zuiji_SchInfo.Nyusyutu_Kbn = "2" Then
                            Furikbn = 2 '����
                        Else
                            Furikbn = 3 '�o��
                        End If

                        If Not fn_DeleteG_SCHMAST(aInfoGakkou.GAKKOU_CODE, 2, Furikbn, .Furikae_Day, Nengetsudo) Then
                            Exit Try
                        End If
                    Else
                        '***���聨����:Update***

                        '�����Ώۃt���O(�������s�Ȃ��K�v�����邩�ǂ���)
                        Dim SyoriFlg As Boolean = False

                        '�����U���`�F�b�N
                        If .Furikae_Date <> aZuiji_SchInfo.Furikae_Date Then
                            SyoriFlg = True
                        End If

                        '�����o���敪�`�F�b�N
                        If .Nyusyutu_Kbn <> aZuiji_SchInfo.Nyusyutu_Kbn Then
                            SyoriFlg = True
                        End If

                        '���w�N�t���O�`�F�b�N
                        Dim SyokiGakunenFlg As String() = .GetGakunenFlg
                        Dim GakunenFlg As String() = aZuiji_SchInfo.GetGakunenFlg

                        If SyokiGakunenFlg(0) = "err" OrElse GakunenFlg(0) = "err" Then
                            '�G���[���O�͓����ŏo��
                            Exit Try
                        Else
                            For i As Integer = 1 To 9 Step 1
                                If SyokiGakunenFlg(i) <> GakunenFlg(i) Then
                                    SyoriFlg = True
                                    Exit For
                                End If
                            Next
                        End If

                        '�������X�P�W���[��Update
                        If SyoriFlg = True Then
                            If Not UpDateZuijiG_SCHMAST(aInfoGakkou, aSyoki_Zuiji_SchInfo, aZuiji_SchInfo) Then
                                '�G���[���O�͓����ŏo��
                                Exit Try
                            End If
                        End If
                    End If
                End If
            End With

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ERRMSG = "�w�Z�X�P�W���[���̍X�V�Ɏ��s���܂���"
        End Try

        Return ret


    End Function
    '
    '�@�֐����@-�@UpDateZuijiG_SCHMAST
    '
    '�@�@�\    -  �X�P�W���[���X�V
    '
    '�@����    -  aInfoGakkou�AaSyoki_Zuiji_SchInfo�AaTuujyou_SchInfo
    '
    '  �߂�l�@-�@True,False
    '
    '�@���l    -  ���o���敪�̕ύX���������ꍇ�X�V���߂�ǂ��̂��߁ADelete�AInsert�ɂ���
    '
    '�@
    Private Function UpDateZuijiG_SCHMAST(ByVal aInfoGakkou As GAKDATA, _
                                        ByVal aSyoki_Zuiji_SchInfo As ZuijiData, _
                                        ByVal aZuiji_SchInfo As ZuijiData) As Boolean

        Dim ret As Boolean = False

        Try
            '***���X�P�W���[���̍폜***
            Dim Furikbn As Integer
            Dim Nengetsudo As String = aSyoki_Zuiji_SchInfo.Furikae_Nen & aSyoki_Zuiji_SchInfo.Furikae_Tuki

            '�X�P�W���[���敪�̔���
            If aSyoki_Zuiji_SchInfo.Nyusyutu_Kbn = "2" Then
                Furikbn = 2 '����
            Else
                Furikbn = 3 '�o��
            End If

            If Not fn_DeleteG_SCHMAST(aInfoGakkou.GAKKOU_CODE, 2, Furikbn, aSyoki_Zuiji_SchInfo.Furikae_Day, Nengetsudo) Then
                ERRMSG = "�w�Z�X�P�W���[���̍X�V�Ɏ��s���܂���"
                Exit Try
            End If

            '***�V�K�ɍ쐬***
            If Not fn_InsertZuijiG_SCHMAST(aInfoGakkou, aZuiji_SchInfo) Then
                ERRMSG = "�w�Z�X�P�W���[���̍X�V�Ɏ��s���܂���"
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ERRMSG = "�w�Z�X�P�W���[���X�V�Ɏ��s���܂���"
            Exit Try
        End Try

        Return ret

    End Function
#End Region
#Region "InsertG_SCHMAST"
    '
    '�@�֐����@-�@fn_InsertG_SCHMAST
    '
    '�@�@�\    -  �X�P�W���[���쐬
    '
    '�@����    -  strGakkouCode
    '
    '�@���l    -  
    '
    '�@
    Private Function fn_InsertG_SCHMAST(ByVal strGakkouCode As String) As Boolean

        Dim ret As Boolean = False

        Try
            '***���O���***
            STR_COMMAND = "fn_InsertG_SCHMAST"
            STR_LOG_GAKKOU_CODE = strGakkouCode
            STR_LOG_FURI_DATE = ""
            '***���O���**

            '���g�����U�N�V�����J�n
            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Try
            End If

            '���w�Z���̎擾
            Dim Flg As Boolean = False
            Dim InfoGakkou As New GAKDATA(strGakkouCode, Flg, MainDB)

            If Flg = False Then
                ERRMSG = "�w�Z���̎擾�Ɏ��s���܂����B" & vbCrLf & "�w�Z�R�[�h:" & strGakkouCode
                Exit Try
            End If

            '����ʏ����\���̂ɃZ�b�g����
            Call Sb_GetData(Tuujyou_SchInfo, Zuiji_SchInfo)

            '���ʏ�U�֓��^�u���͉�ʏ��̃`�F�b�N(�S�Z�b�g�Ȃ��Ȃ����Z�b�g) 
            If fn_Check_TuujyouFuriDate(InfoGakkou, Tuujyou_SchInfo, True) = False Then
                Exit Try
            End If

            '�������U�֓��^�u���͉�ʏ��̃`�F�b�N
            If fn_Check_ZuijiFuriDate(InfoGakkou, Zuiji_SchInfo) = False Then
                Exit Try
            End If

            Dim SyoriFlg As Boolean = False

            '���ʏ�X�P�W���[���̍쐬
            For i As Integer = 1 To 5 Step 1
                '�Ώۃt���O�������Ă����珈��
                If Tuujyou_SchInfo(i).TaisyouFlg = True Then
                    '�w�Z�Ǝ��U�̃X�P�W���[���쐬
                    If fn_InsertTuujyouG_SCHMAST(InfoGakkou, Tuujyou_SchInfo(i)) = False Then
                        Exit Try
                    End If
                End If
            Next

            '�������X�P�W���[���̍쐬
            For i As Integer = 1 To 6 Step 1
                '�Ώۃt���O�������Ă����珈��
                If Zuiji_SchInfo(i).TaisyouFlg = True Then
                    '�w�Z�Ǝ��U�̃X�P�W���[���̍쐬
                    If fn_InsertZuijiG_SCHMAST(InfoGakkou, Zuiji_SchInfo(i)) = False Then
                        Exit Try
                    End If
                End If
            Next

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ret = False
        Finally
            If ret = True Then
                '�g�����U�N�V�����I���iCOMMIT�j
                If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                    Call GSUB_LOG(0, "COMMIT���s")
                    ret = False
                End If
            Else
                '�g�����U�N�V�����I���iROLLBACK�j
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Call GSUB_LOG(0, "ROLLBACK���s")
                    ret = False
                End If
            End If
        End Try

        Return ret

    End Function

    '
    '�@�֐����@-  fn_InsertTuujyouG_SCHMAST
    '
    '�@�@�\    -  �ʏ�X�P�W���[���쐬
    '
    '�@����    -  aInfoGakkou�AaInfoTuujyou�ASaiFuriOnlyFlg
    '
    '�@���l    -  fn_InsertG_SCHMAST�̃T�u�֐�
    '
    '�@
    Private Function fn_InsertTuujyouG_SCHMAST(ByVal aInfoGakkou As GAKDATA, _
                                               ByVal aInfoTuujyou As TuujyouData, _
                                               Optional ByVal SaiFuriOnlyFlg As Boolean = False) As Boolean

        Dim ret As Boolean = False

        Try
            '***���O���***
            STR_COMMAND = "fn_InsertTuujyouG_SCHMAST"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***���O���**

            '�������t���擾
            Dim SyoriDate(1) As String
            SyoriDate(0) = Format(Now, "yyyyMMdd")
            SyoriDate(1) = Format(Now, "yyyyMMddHHmmss")

            '�w�N�t���O�̎擾
            Dim GakunenFlg As String() = aInfoTuujyou.GetGakunenFlg()
            If GakunenFlg(0) = "err" Then

                ERRMSG = "�w�N�Ώۃt���O�̎擾�Ɏ��s���܂����B"
                Exit Try
            End If

            '�e����ݒ�
            Dim Nengetu As String = aInfoTuujyou.Seikyu_Nen & aInfoTuujyou.Seikyu_Tuki
            '���͂��ꂽ�U�֓�(�c�Ɠ����l�����Ȃ�)
            Dim EntryFuriDate As String = "00000000"
            Dim FuriDate As String = "00000000"
            Dim SaiFuriDate As String = "00000000"

            Dim Schkbn As Integer = 0 '�ʏ�
            Dim Furikbn As Integer = 0 '���U

            Dim ITAKU_CODE_S As String

            '�ĐU�f�[�^�쐬�p�Ƀ��[�v
            For SyoriCnt As Integer = 0 To 1 Step 1

                '�ĐU�̂ݍ쐬�̏ꍇ
                If SaiFuriOnlyFlg = True Then
                    SyoriCnt += 1
                End If

                '�ĐU�f�[�^�쐬
                If SyoriCnt = 1 Then
                    Schkbn = 0 '�ʏ�
                    Furikbn = 1 '�ĐU
                End If

                '�U�֓��̒��o
                With aInfoTuujyou
                    Select Case Furikbn
                        Case 0
                            '���U�p�̐ݒ�
                            EntryFuriDate = .Seikyu_Nen & .Seikyu_Tuki & .Furikae_Date
                            '�U�֓��Z�o
                            FuriDate = fn_GetFuriDate(.Seikyu_Nen, .Seikyu_Tuki, .Furikae_Date, Schkbn, Furikbn, aInfoGakkou)

                            '�ĐU���̔N�̊m�菈��
                            If .SaiFurikae_Date <> "" Then

                                '��������
                                Select Case True
                                    Case .Furikae_Date < .SaiFurikae_Date
                                        '�c�Ɠ��Z�o
                                        SaiFuriDate = fn_GetEigyoubi(.Seikyu_Nen & .Seikyu_Tuki & .SaiFurikae_Date, "0", "+")
                                    Case .Furikae_Date > .SaiFurikae_Date
                                        If .Seikyu_Tuki = "12" Then
                                            '�c�Ɠ��Z�o
                                            SaiFuriDate = fn_GetEigyoubi(CStr(CInt(.Seikyu_Nen) + 1) & "01" & .SaiFurikae_Date, "0", "+")
                                        Else
                                            '�c�Ɠ��Z�o
                                            SaiFuriDate = fn_GetEigyoubi(.Seikyu_Nen & Format(CInt(.Seikyu_Tuki) + 1, "00") & .SaiFurikae_Date, "0", "+")
                                        End If
                                    Case .Furikae_Date = .SaiFurikae_Date
                                        Call GSUB_LOG(0, "�U�֓��ُ�:" & Furikbn)
                                        ERRMSG = "�U�֓��ƍĐU��������ł�"
                                        Exit Try

                                End Select
                            Else
                                SaiFuriDate = "00000000"
                            End If

                            ITAKU_CODE_S = aInfoGakkou.ITAKU_CODE
                        Case 1
                            '��������
                            Select Case True
                                Case .Furikae_Date < .SaiFurikae_Date
                                    '�ĐU�p�̐ݒ�
                                    EntryFuriDate = .Seikyu_Nen & .Seikyu_Tuki & .SaiFurikae_Date
                                    '�U�֓��Z�o
                                    FuriDate = fn_GetFuriDate(.Seikyu_Nen, .Seikyu_Tuki, .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)

                                Case .Furikae_Date > .SaiFurikae_Date
                                    If .Seikyu_Tuki = "12" Then
                                        '�ĐU�p�̐ݒ�
                                        EntryFuriDate = CStr(CInt(.Seikyu_Nen) + 1) & "01" & .SaiFurikae_Date
                                        '�U�֓��Z�o
                                        FuriDate = fn_GetFuriDate(CStr(CInt(.Seikyu_Nen) + 1), "01", .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                    Else
                                        '�ĐU�p�̐ݒ�
                                        EntryFuriDate = .Seikyu_Nen & Format(CInt(.Seikyu_Tuki) + 1, "00") & .SaiFurikae_Date
                                        '�U�֓��Z�o
                                        FuriDate = fn_GetFuriDate(.Seikyu_Nen, Format(CInt(.Seikyu_Tuki) + 1, "00"), .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                    End If

                                Case .Furikae_Date = .SaiFurikae_Date
                                    Call GSUB_LOG(0, "�U�֓��ُ�:" & Furikbn)
                                    ERRMSG = "�U�֓��ƍĐU��������ł�"
                                    Exit Try
                            End Select


                            '���z����̏ꍇ�̂ݍĐU����ݒ�
                            If aInfoGakkou.SFURI_SYUBETU = "3" Then '�ĐU����A���z����
                                '��������12���Ȃ痂�N��1��
                                If .Seikyu_Tuki = "12" Then
                                    .Seikyu_Nen = (CInt(.Seikyu_Nen) + 1).ToString
                                    .Seikyu_Tuki = "01"
                                Else
                                    .Seikyu_Tuki = (CInt(.Seikyu_Tuki) + 1).ToString
                                End If
                                '�c�Ɠ��Z�o
                                SaiFuriDate = fn_GetEigyoubi(.Seikyu_Nen & .Seikyu_Tuki & .SaiFurikae_Date, "0", "+")
                            Else
                                SaiFuriDate = "00000000"
                            End If

                            '�ϑ��҃R�[�h��1�����J�E���g�A�b�v
                            ITAKU_CODE_S = aInfoGakkou.ITAKU_CODE
                        Case 2
                            '�����݂̂̂��߂Ȃ�
                            ERRMSG = "�w�Z���U�X�P�W���[���쐬�Ɏ��s���܂���"
                            Call GSUB_LOG(0, "�U�֋敪�ُ�:" & Furikbn)
                            Exit Try
                        Case 3
                            '�����݂̂̂��߂Ȃ�
                            Call GSUB_LOG(0, "�U�֋敪�ُ�:" & Furikbn)
                            ERRMSG = "�w�Z���U�X�P�W���[���쐬�Ɏ��s���܂���"
                            Exit Try
                        Case Else
                            Call GSUB_LOG(0, "�U�֋敪�ُ�:" & Furikbn)
                            ERRMSG = "�w�Z���U�X�P�W���[���쐬�Ɏ��s���܂���"
                            Exit Try
                    End Select

                End With

                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                '�e��\����̎Z�o
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                Dim CLS As New MAIN.ClsSchduleMaintenanceClass
                Call CLS.SetKyuzituInformation()

                '2012/09/04 saitou �x������ MODIFY -------------------------------------------------->>>>
                CLS.SetSchTable = MAIN.ClsSchduleMaintenanceClass.APL.JifuriApplication
                'CLS.SetSchTable = CLS.APL.JifuriApplication
                '2012/09/04 saitou �x������ MODIFY --------------------------------------------------<<<<

                '�X�P�W���[���쐬�Ώۂ̎����R�[�h�𒊏o
                CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(FuriDate), aInfoGakkou.GAKKOU_CODE, "01")

                CLS.SCH.FURI_DATE = GCom.SET_DATE(FuriDate)
                If CLS.SCH.FURI_DATE = "00000000" Then
                Else
                    CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
                End If

                Dim strFURI_DATE As String = CLS.SCH.FURI_DATE

                Dim BRet As Boolean = CLS.INSERT_NEW_SCHMAST(0, False, True)

                Dim ENTRY_Y_DATE As String = "00000000"                                                   '���׍쐬�\����Z�o
                Dim CHECK_Y_DATE As String = fn_GetEigyoubi(FuriDate, STR_JIFURI_CHECK, "-")         '�`�F�b�N�\����Z�o
                Dim DATA_Y_DATE As String = fn_GetEigyoubi(FuriDate, STR_JIFURI_HAISIN, "-")    '�U�փf�[�^�쐬�\����Z�o
                Dim FUNOU_Y_DATE As String = CLS.SCH.FUNOU_YDATE                                          '�s�\���ʍX�V�\����Z�o
                Dim KESSAI_Y_DATE As String = CLS.SCH.KESSAI_YDATE                                        '�������ϗ\����Z�o
                Dim HENKAN_Y_DATE As String = CLS.SCH.HENKAN_YDATE                                          '�Ԋҗ\���

                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                'INSERT���쐬
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                Dim SQL As String

                SQL = ""
                SQL += " INSERT INTO G_SCHMAST "
                SQL += " VALUES ( "
                SQL += "'" & aInfoGakkou.GAKKOU_CODE & "'"        '1.�w�Z�R�[�h
                SQL += ",'" & Nengetu & "'"                       '2.�����N��
                SQL += ",'" & Schkbn & "'"                        '3.�X�P�W���[���敪
                SQL += ",'" & Furikbn & "'"                       '4.�U�֋敪
                SQL += ",'" & FuriDate & "'"                      '5.�U�֓�
                SQL += ",'" & SaiFuriDate & "'"                   '6.�ĐU�֓�
                '���ʏ�X�P�W���[���̊w�N�t���O��z��ɕϊ�����
                SQL += ",'" & GakunenFlg(1) & "'"                 '7.�w�N1
                SQL += ",'" & GakunenFlg(2) & "'"                 '8.�w�N2
                SQL += ",'" & GakunenFlg(3) & "'"                 '9.�w�N3
                SQL += ",'" & GakunenFlg(4) & "'"                 '10.�w�N4
                SQL += ",'" & GakunenFlg(5) & "'"                 '11.�w�N5
                SQL += ",'" & GakunenFlg(6) & "'"                 '12.�w�N6
                SQL += ",'" & GakunenFlg(7) & "'"                 '13.�w�N7
                SQL += ",'" & GakunenFlg(8) & "'"                 '14.�w�N8
                SQL += ",'" & GakunenFlg(9) & "'"                 '15.�w�N9
                '���w�Z���
                SQL += ",'" & ITAKU_CODE_S & "'"                  '16.�ϑ��҃R�[�h
                SQL += ",'" & aInfoGakkou.TKIN_CODE & "'"         '17.�戵���Z�@��
                SQL += ",'" & aInfoGakkou.TSIT_CODE & "'"         '18.�戵�x�X
                SQL += ",'" & aInfoGakkou.BAITAI_CODE & "'"       '19.�}�̃R�[�h 
                SQL += ",'" & aInfoGakkou.TESUUTYO_KBN & "'"      '20.�萔���敪 
                '���e�\���
                SQL += "," & "'" & ENTRY_Y_DATE & "'"             '21.���׍쐬�\���
                SQL += "," & "'00000000'"                         '22.���׍쐬��
                SQL += "," & "'" & CHECK_Y_DATE & "'"             '23.�`�F�b�N�\���
                SQL += "," & "'00000000'"                         '24.�`�F�b�N��
                SQL += "," & "'" & DATA_Y_DATE & "'"              '25.�U�փf�[�^�쐬�\���
                SQL += "," & "'00000000'"                         '26.�U�փf�[�^�쐬��
                SQL += "," & "'" & FUNOU_Y_DATE & "'"             '27.�s�\���ʍX�V�\���
                SQL += "," & "'00000000'"                         '28.�s�\���ʍX�V��
                '***�ǉ��Ԋғ�2010.02.21
                SQL += "," & "'" & HENKAN_Y_DATE & "'"            '29.�Ԋҗ\���
                SQL += "," & "'00000000'"                         '30.�Ԋғ�
                '***�ǉ��Ԋғ�2010.02.21
                SQL += "," & "'" & KESSAI_Y_DATE & "'"            '31.���ϗ\���
                SQL += "," & "'00000000'"                         '32.���ϓ�
                '���e��t���O
                SQL += "," & "'0'"                                '33.���׍쐬�σt���O
                SQL += "," & "'0'"                                '34.���z�m�F�σt���O
                SQL += "," & "'0'"                                '35.�U�փf�[�^�쐬�σt���O
                SQL += "," & "'0'"                                '36.�s�\���ʍX�V�σt���O
                SQL += "," & "'0'"                                '37.�Ԋ҃t���O
                SQL += "," & "'0'"                                '38.�ĐU�f�[�^�쐬�σt���O
                SQL += "," & "'0'"                                '39.���ύσt���O
                SQL += "," & "'0'"                                '40.���f�t���O
                '���������z
                SQL += "," & 0                                    '41.��������
                SQL += "," & 0                                    '42.�������z
                SQL += "," & 0                                    '43.�萔��
                SQL += "," & 0                                    '44.�萔��1
                SQL += "," & 0                                    '45.�萔��2
                SQL += "," & 0                                    '46.�萔��3
                SQL += "," & 0                                    '47.�U�֍ό���
                SQL += "," & 0                                    '48.�U�֍ϋ��z
                SQL += "," & 0                                    '49.�s�\����
                SQL += "," & 0                                    '50.�s�\���z
                '�����t
                SQL += "," & "'" & SyoriDate(0) & "'"             '51.�쐬���t
                SQL += "," & "'" & SyoriDate(1) & "'"             '52.�^�C���X�^���v
                SQL += "," & "'" & fn_Hosei(EntryFuriDate) & "'"  '53.�\��1(���͂��ꂽ�U�֓�)
                SQL += "," & "'" & Space(30) & "'"                '54.�\��2
                SQL += "," & "'" & Space(30) & "'"                '55.�\��3
                SQL += "," & "'" & Space(30) & "'"                '56.�\��4
                SQL += "," & "'" & Space(30) & "'"                '57.�\��5
                SQL += "," & "'" & Space(30) & "'"                '58.�\��6
                SQL += "," & "'" & Space(30) & "'"                '59.�\��7
                SQL += "," & "'" & Space(30) & "'"                '60.�\��8
                SQL += "," & "'" & Space(30) & "'"                '61.�\��9
                SQL += "," & "'" & Space(30) & "')"               '62.�\��10

                If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                    Call GSUB_LOG(0, "�w�Z�X�P�W���[��Insert���s�ASQL:" & SQL)
                    ERRMSG = "�w�Z�X�P�W���[���쐬�Ɏ��s���܂���"
                    Exit Try
                End If

                '��Ǝ��U�A�g�Ȃ��Ȃ�A���U�X�P�W���[���͍쐬���Ȃ�
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                'INSERT���쐬(��Ǝ��U��)
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                Dim ToriS_Code As String = aInfoGakkou.GAKKOU_CODE
                Dim ToriF_Code As String

                Select Case Furikbn
                    Case 0 '���U
                        ToriF_Code = "01"
                    Case 1 '�ĐU
                        ToriF_Code = "02"
                    Case 2 '��������
                        ToriF_Code = "03"
                    Case 3 '�����o��
                        ToriF_Code = "04"
                    Case Else
                        Call GSUB_LOG(0, "�U�֋敪�ُ�:" & Furikbn)
                        ERRMSG = "��Ǝ��U�̃X�P�W���[���쐬�Ɏ��s���܂���"
                        Exit Try
                End Select


                '@@@���ɓo�^����Ă��邩�`�F�b�N@@@
                '�X�P�W���[�����݃`�F�b�N
                If fn_SchMastIsExist(ToriS_Code, ToriF_Code, FuriDate, OBJ_CONNECTION, OBJ_TRANSACTION) <> True Then
                    '�����}�X�^���݃`�F�b�N
                    If fn_ToriMastIsExist(ToriS_Code, ToriF_Code, OBJ_CONNECTION, OBJ_TRANSACTION) = True Then
                        '�����Ƀq�b�g���Ȃ�������
                        If fn_INSERTSCHMAST(ToriS_Code, ToriF_Code, FuriDate) = gintKEKKA.NG Then

                            ret = False
                            Call GSUB_LOG(0, "�X�P�W���[���o�^:���s" & Err.Description)
                            MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Try
                        End If
                    End If
                End If

                '�ĐU�X�P�W���[���쐬����
                If aInfoTuujyou.SaiFurikae_Check = False Then
                    Exit For
                End If
            Next

            ret = True

        Catch ex As Exception
            ERRMSG = "�w�Z�X�P�W���[���쐬�Ɏ��s���܂���"
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        End Try

        Return ret

    End Function
    '
    '�@�֐����@-  fn_InsertZuijiG_SCHMAST
    '
    '�@�@�\    -  �����X�P�W���[���쐬
    '
    '�@����    -  aInfoGakkou�AaInfoZuiji
    '
    '�@���l    -  fn_InsertG_SCHMAST�̃T�u�֐�
    '�@
    Private Function fn_InsertZuijiG_SCHMAST(ByVal aInfoGakkou As GAKDATA, ByVal aInfoZuiji As ZuijiData) As Boolean

        Dim ret As Boolean = False

        Try
            '***���O���***
            STR_COMMAND = "fn_InsertZuijiG_SCHMAST"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***���O���**

            Dim SyoriDate(1) As String
            SyoriDate(0) = Format(Now, "yyyyMMdd")
            SyoriDate(1) = Format(Now, "yyyyMMddHHmmss")

            '�w�N�t���O�̎擾
            Dim GakunenFlg As String() = aInfoZuiji.GetGakunenFlg
            If GakunenFlg(0) = "err" Then
                Exit Try
            End If
            '�e����ݒ�
            Dim Nengetu As String = aInfoZuiji.Furikae_Nen & aInfoZuiji.Furikae_Tuki
            Dim Schkbn As String = "2"
            Dim Furikbn As String

            Dim ITAKU_CODE_S As String

            '���o���敪
            If aInfoZuiji.Nyusyutu_Kbn = "2" Then
                Furikbn = "2"

                '�ϑ��҃R�[�h�̓�1�����J�E���g�A�b�v���Ȃ�
                ITAKU_CODE_S = aInfoGakkou.ITAKU_CODE
            Else
                Furikbn = "3"

                '�ϑ��҃R�[�h�̓�1�����J�E���g�A�b�v���Ȃ�
                ITAKU_CODE_S = aInfoGakkou.ITAKU_CODE
            End If

            '���͂��ꂽ�U�֓�(�c�Ɠ��̍l�������Ȃ�)
            Dim EntryFuriDate As String = aInfoZuiji.Furikae_Nen & aInfoZuiji.Furikae_Tuki & aInfoZuiji.Furikae_Date

            Dim FuriDate As String = "00000000"
            Dim SaiFuriDate As String = "00000000"

            '�U�֓��̎Z�o
            With aInfoZuiji
                FuriDate = fn_GetFuriDate(.Furikae_Nen, .Furikae_Tuki, .Furikae_Date, "2", Furikbn, aInfoGakkou) '�U�֓��Z�o
                '�ĐU�͍s�Ȃ�Ȃ�
                SaiFuriDate = "00000000"
            End With

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '�e��\����̎Z�o
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            Dim CLS As New MAIN.ClsSchduleMaintenanceClass
            '2012/09/04 saitou �x������ MODIFY -------------------------------------------------->>>>
            CLS.SetSchTable = MAIN.ClsSchduleMaintenanceClass.APL.JifuriApplication
            'CLS.SetSchTable = CLS.APL.JifuriApplication
            '2012/09/04 saitou �x������ MODIFY --------------------------------------------------<<<<

            '�X�P�W���[���쐬�Ώۂ̎����R�[�h�𒊏o
            CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(FuriDate), aInfoGakkou.GAKKOU_CODE, "01")

            CLS.SCH.FURI_DATE = GCom.SET_DATE(FuriDate)
            If CLS.SCH.FURI_DATE = "00000000" Then
            Else
                CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
            End If

            Dim strFURI_DATE As String = CLS.SCH.FURI_DATE

            Dim BRet As Boolean = CLS.INSERT_NEW_SCHMAST(0, False, True)

            Dim ENTRY_Y_DATE As String = "00000000"                                                   '���׍쐬�\����Z�o
            Dim CHECK_Y_DATE As String = fn_GetEigyoubi(FuriDate, STR_JIFURI_CHECK, "-")         '�`�F�b�N�\����Z�o
            Dim DATA_Y_DATE As String = fn_GetEigyoubi(FuriDate, STR_JIFURI_HAISIN, "-")    '�U�փf�[�^�쐬�\����Z�o
            Dim FUNOU_Y_DATE As String = CLS.SCH.FUNOU_YDATE                                          '�s�\���ʍX�V�\����Z�o
            Dim KESSAI_Y_DATE As String = CLS.SCH.KESSAI_YDATE                                        '�������ϗ\����Z�o

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            'INSERT���쐬
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            Dim SQL As String

            SQL = ""
            SQL += " INSERT INTO G_SCHMAST "
            SQL += " VALUES ( "
            SQL += "'" & aInfoGakkou.GAKKOU_CODE & "'"        '1.�w�Z�R�[�h
            SQL += ",'" & Nengetu & "'"                       '2.�����N��
            SQL += ",'" & Schkbn & "'"                        '3.�X�P�W���[���敪
            SQL += ",'" & Furikbn & "'"                       '4.�U�֋敪
            SQL += ",'" & FuriDate & "'"                      '5.�U�֓�
            SQL += ",'" & SaiFuriDate & "'"                   '6.�ĐU�֓�
            '���ʏ�X�P�W���[���̊w�N�t���O��z��ɕϊ�����
            SQL += ",'" & GakunenFlg(1) & "'"                 '7.�w�N1
            SQL += ",'" & GakunenFlg(2) & "'"                 '8.�w�N2
            SQL += ",'" & GakunenFlg(3) & "'"                 '9.�w�N3
            SQL += ",'" & GakunenFlg(4) & "'"                 '10.�w�N4
            SQL += ",'" & GakunenFlg(5) & "'"                 '11.�w�N5
            SQL += ",'" & GakunenFlg(6) & "'"                 '12.�w�N6
            SQL += ",'" & GakunenFlg(7) & "'"                 '13.�w�N7
            SQL += ",'" & GakunenFlg(8) & "'"                 '14.�w�N8
            SQL += ",'" & GakunenFlg(9) & "'"                 '15.�w�N9
            '���w�Z���
            SQL += ",'" & ITAKU_CODE_S & "'"                  '16.�ϑ��҃R�[�h
            SQL += ",'" & aInfoGakkou.TKIN_CODE & "'"         '17.�戵���Z�@��
            SQL += ",'" & aInfoGakkou.TSIT_CODE & "'"         '18.�戵�x�X
            SQL += ",'" & aInfoGakkou.BAITAI_CODE & "'"       '19.�}�̃R�[�h 
            SQL += ",'" & aInfoGakkou.TESUUTYO_KBN & "'"      '20.�萔���敪 
            '���e�\���
            SQL += "," & "'" & ENTRY_Y_DATE & "'"             '21.���׍쐬�\���
            SQL += "," & "'00000000'"                         '22.���׍쐬��
            SQL += "," & "'" & CHECK_Y_DATE & "'"             '23.�`�F�b�N�\���
            SQL += "," & "'00000000'"                         '24.�`�F�b�N��
            SQL += "," & "'" & DATA_Y_DATE & "'"              '25.�U�փf�[�^�쐬�\���
            SQL += "," & "'00000000'"                         '26.�U�փf�[�^�쐬��
            SQL += "," & "'" & FUNOU_Y_DATE & "'"             '27.�s�\���ʍX�V�\���
            SQL += "," & "'00000000'"                         '28.�s�\���ʍX�V��
            SQL += "," & "'" & KESSAI_Y_DATE & "'"            '31.���ϗ\���
            SQL += "," & "'00000000'"                         '32.���ϓ�
            '���e��t���O
            SQL += "," & "'0'"                                '33.���׍쐬�σt���O
            SQL += "," & "'0'"                                '34.���z�m�F�σt���O
            SQL += "," & "'0'"                                '35.�U�փf�[�^�쐬�σt���O
            SQL += "," & "'0'"                                '36.�s�\���ʍX�V�σt���O
            SQL += "," & "'0'"                                '38.�ĐU�f�[�^�쐬�σt���O
            SQL += "," & "'0'"                                '39.���ύσt���O
            SQL += "," & "'0'"                                '40.���f�t���O
            '���������z
            SQL += "," & 0                                    '41.��������
            SQL += "," & 0                                    '42.�������z
            SQL += "," & 0                                    '43.�萔��
            SQL += "," & 0                                    '44.�萔��1
            SQL += "," & 0                                    '45.�萔��2
            SQL += "," & 0                                    '46.�萔��3
            SQL += "," & 0                                    '47.�U�֍ό���
            SQL += "," & 0                                    '48.�U�֍ϋ��z
            SQL += "," & 0                                    '49.�s�\����
            SQL += "," & 0                                    '50.�s�\���z
            '�����t
            SQL += "," & "'" & SyoriDate(0) & "'"             '51.�쐬���t
            SQL += "," & "'" & SyoriDate(1) & "'"             '52.�^�C���X�^���v
            SQL += "," & "'" & fn_Hosei(EntryFuriDate) & "'"  '53.�\��1(���͐U�֓�)
            SQL += "," & "'" & Space(15) & "'"                '54.�\��2
            SQL += "," & "'" & Space(15) & "'"                '55.�\��3
            SQL += "," & "'" & Space(15) & "'"                '56.�\��4
            SQL += "," & "'" & Space(15) & "')"               '57.�\��5

            If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                Call GSUB_LOG(0, "�w�Z�X�P�W���[��Insert���s�ASQL:" & SQL)
                ERRMSG = "�w�Z�X�P�W���[���쐬�Ɏ��s���܂���"
                Exit Try
            End If

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            'INSERT���쐬(��Ǝ��U��)
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            '��Ǝ��U�A�g���̂�
            Dim ToriS_Code As String = aInfoGakkou.GAKKOU_CODE
            Dim ToriF_Code As String

            Select Case Furikbn
                Case 0 '���U
                    ToriF_Code = "01"
                Case 1 '�ĐU
                    ToriF_Code = "02"
                Case 2 '��������
                    ToriF_Code = "03"
                Case 3 '�����o��
                    ToriF_Code = "04"
                Case Else
                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & Furikbn)
                    ERRMSG = "��Ǝ��U�̃X�P�W���[���쐬�Ɏ��s���܂���"
                    Exit Try
            End Select


            '@@@���ɓo�^����Ă��邩�`�F�b�N@@@
            '�X�P�W���[�����݃`�F�b�N
            If fn_SchMastIsExist(ToriS_Code, ToriF_Code, FuriDate, OBJ_CONNECTION, OBJ_TRANSACTION) <> True Then
                '�����}�X�^���݃`�F�b�N
                If fn_ToriMastIsExist(ToriS_Code, ToriF_Code, OBJ_CONNECTION, OBJ_TRANSACTION) = True Then
                    '�����Ƀq�b�g���Ȃ�������
                    If fn_INSERTSCHMAST(ToriS_Code, ToriF_Code, FuriDate) = gintKEKKA.NG Then

                        ret = False
                        Call GSUB_LOG(0, "�U�֓�:" & FuriDate & "��Ǝ��U�X�P�W���[���쐬���s")
                        ERRMSG = "��Ǝ��U�̃X�P�W���[���쐬�Ɏ��s���܂���"
                        Exit Try
                    End If
                End If
            End If

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        End Try

        Return ret

    End Function
#End Region
#Region "DELETE G_SCHMAST"
    '
    '�@�֐����@-�@fn_DeleteG_SCHMAST
    '
    '�@�@�\    -  �ʂ̃X�P�W���[���̍폜
    '
    '�@����    -  aGakkouCode,aSchkbn,aFurikbn,aFuriDate,aNengetudo
    '
    '�@���l    -  
    '
    '�@
    Private Function fn_DeleteG_SCHMAST(ByVal aGakkouCode As String, _
                                        ByVal aSchkbn As Integer, _
                                        ByVal aFurikbn As Integer, _
                                        ByVal aFuriDate As String, _
                                        ByVal aNengetudo As String) As Boolean

        Dim ret As Boolean = False

        Try
            Dim InfoG_SCHMAST As New G_SCHMASTDATA

            If Not InfoG_SCHMAST.SetG_SCHMASTDATA(aGakkouCode, aSchkbn, aFurikbn, aFuriDate, MainDB, aNengetudo) Then
                ERRMSG = "�X�P�W���[���擾�Ɏ��s���܂���"
                Exit Try
            End If

            With InfoG_SCHMAST
                If .ENTRI_FLG_S = "1" OrElse _
                .CHECK_FLG_S = "1" OrElse _
                .DATA_FLG_S = "1" OrElse _
                .FUNOU_FLG_S = "1" OrElse _
                .SAIFURI_FLG_S = "1" OrElse _
                .KESSAI_FLG_S = "1" OrElse _
                .TYUUDAN_FLG_S = "1" Then

                    ERRMSG = "�������̃X�P�W���[�������݂��邽�ߍ폜�ł��܂���B"
                    Exit Try
                End If
            End With

            Dim SQL As String

            SQL = "  DELETE FROM G_SCHMAST "
            SQL &= " WHERE GAKKOU_CODE_S = '" & aGakkouCode & "'"          '�w�Z�R�[�h
            SQL &= " AND   NENGETUDO_S = '" & aNengetudo & "'"             '�N���x
            SQL &= " AND   SCH_KBN_S = '" & aSchkbn.ToString & "'"         '�X�P�W���[���敪
            SQL &= " AND   FURI_KBN_S = '" & aFurikbn.ToString & "'"       '�U�֋敪
            SQL &= " AND   FURI_DATE_S = '" & aFuriDate & "'"              '�U�֓�

            If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                Call GSUB_LOG(0, "�w�Z�X�P�W���[���폜���s�ASQL:" & SQL)
                ERRMSG = "�w�Z�X�P�W���[���폜�Ɏ��s���܂���"
                Exit Try
            End If

            Dim ToriS_Code As String = aGakkouCode
            Dim ToriF_Code As String

            Select Case aFurikbn
                Case 0 '���U
                    ToriF_Code = "01"
                Case 1 '�ĐU
                    ToriF_Code = "02"
                Case 2 '��������
                    ToriF_Code = "03"
                Case 3 '�����o��
                    ToriF_Code = "04"
                Case Else
                    Call GSUB_LOG(0, "�U�֋敪�ُ�:" & aFurikbn)
                    ERRMSG = "�U�֋敪�̎擾�Ɏ��s���܂���"
                    Exit Try
            End Select

            SQL = "  DELETE FROM SCHMAST "
            SQL &= " WHERE TORIS_CODE_S = '" & ToriS_Code & "'" '������R�[�h
            SQL &= " AND   TORIF_CODE_S = '" & ToriF_Code & "'" '����敛�R�[�h
            SQL &= " AND   FURI_DATE_S = '" & aFuriDate & "'"   '�U�֓�

            If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                Call GSUB_LOG(0, "�w�Z�X�P�W���[���폜���s�ASQL:" & SQL)
                ERRMSG = "��Ǝ��U�X�P�W���[���폜�Ɏ��s���܂���"
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            ret = False
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ERRMSG = "�w�Z�X�P�W���[���폜�Ɏ��s���܂���"
        End Try

        Return ret

    End Function

    Private Function fn_DeleteG_SCHMAST_ALL(ByVal aNENGETSUDO As String, ByVal aGakkouCode As String) As Boolean

        Dim ret As Boolean = False
        Dim SSQL As StringBuilder 'SELECT�p

        Dim ArrayFuriDate As New ArrayList

        Dim Orareader As CASTCommon.MyOracleReader = Nothing

        Try
            '���g�����U�N�V�����J�n
            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Try
            End If

            '���U���폜�������ǂ���
            Dim DelFlg As Boolean = False
            Dim InfoSaifuriSch As New G_SCHMASTDATA

            Orareader = New CASTCommon.MyOracleReader(MainDB)
            SSQL = New StringBuilder(128)

            '�܂��Ώۊw�Z�̃X�P�W���[�����擾
            '�����͒ʏ�(���U)�A����(�����A�o��)�@
            SSQL.Append(" SELECT * FROM G_SCHMAST ")
            SSQL.Append(" WHERE GAKKOU_CODE_S = '" & aGakkouCode & "'")
            SSQL.Append(" AND SCH_KBN_S IN ('0','2') ")
            SSQL.Append(" AND FURI_KBN_S IN ('0','2','3') ")
            SSQL.Append(" AND NENGETUDO_S = '" & aNENGETSUDO & "'")

            If Orareader.DataReader(SSQL) = True Then
                While Orareader.EOF = False
                    '�폜�t���O�̏�����
                    DelFlg = False

                    '���������ǂ���
                    If Orareader.GetItem("ENTRI_FLG_S").Trim = "1" OrElse _
                    Orareader.GetItem("CHECK_FLG_S").Trim = "1" OrElse _
                    Orareader.GetItem("DATA_FLG_S").Trim = "1" OrElse _
                    Orareader.GetItem("FUNOU_FLG_S").Trim = "1" OrElse _
                    Orareader.GetItem("SAIFURI_FLG_S").Trim = "1" OrElse _
                    Orareader.GetItem("KESSAI_FLG_S").Trim = "1" OrElse _
                    Orareader.GetItem("TYUUDAN_FLG_S").Trim = "1" Then
                        '�������Ȃ�폜���Ȃ�
                        DelFlg = False
                    Else
                        '�������Ȃ�폜
                        If Not fn_DeleteG_SCHMAST(aGakkouCode, _
                        Orareader.GetItem("SCH_KBN_S"), _
                        Orareader.GetItem("FURI_KBN_S"), _
                        Orareader.GetItem("FURI_DATE_S"), _
                        aNENGETSUDO) Then
                            Exit Try
                        End If

                        '�폜�����ꍇ�̓t���O�𗧂Ă�
                        DelFlg = True
                    End If

                    '�X�P�W���[���敪�����U���ĐU����"000000000"�łȂ��ꍇ
                    If Orareader.GetItem("SCH_KBN_S") = "0" AndAlso _
                    Orareader.GetItem("SFURI_DATE_S").Trim <> "00000000" Then

                        '�ĐU�X�P�W���[���̎擾
                        If Not InfoSaifuriSch.SetG_SCHMASTDATA(aGakkouCode, 0, 1, Orareader.GetItem("SFURI_DATE_S"), MainDB) Then
                            ERRMSG = "�ĐU�X�P�W���[���擾�Ɏ��s���܂���"
                            Exit Try
                        End If

                        '�ĐU�����������ǂ�������
                        With InfoSaifuriSch
                            If .ENTRI_FLG_S = "1" OrElse _
                            .CHECK_FLG_S = "1" OrElse _
                            .DATA_FLG_S = "1" OrElse _
                            .FUNOU_FLG_S = "1" OrElse _
                            .SAIFURI_FLG_S = "1" OrElse _
                            .KESSAI_FLG_S = "1" OrElse _
                            .TYUUDAN_FLG_S = "1" Then

                            Else
                                '�������Ȃ�폜
                                If Not fn_DeleteG_SCHMAST(aGakkouCode, 0, 1, .FURI_DATE_S, aNENGETSUDO) Then
                                    Exit Try
                                End If

                                '���U���폜���Ă��Ȃ���΁A���U�̍ĐU����UPDATE
                                If DelFlg = False Then
                                    Dim SQL As String = ""
                                    SQL = " UPDATE  G_SCHMAST SET "
                                    SQL &= " SFURI_DATE_S = '00000000'"
                                    SQL &= " WHERE GAKKOU_CODE_S = '" & aGakkouCode & "'"
                                    SQL &= " AND   NENGETUDO_S = '" & aNENGETSUDO & "'"
                                    SQL &= " AND   SCH_KBN_S = '0'"
                                    SQL &= " AND   FURI_KBN_S = '0'"
                                    SQL &= " AND   FURI_DATE_S = '" & Orareader.GetItem("FURI_DATE_S") & "'"

                                    If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                                        Call GSUB_LOG(0, "�w�Z�X�P�W���[���폜���s�ASQL:" & SQL)
                                        ERRMSG = "�w�Z�X�P�W���[�����U�X�V�Ɏ��s���܂���"
                                        Exit Try
                                    End If
                                End If
                            End If
                        End With
                    End If

                    Orareader.NextRead()
                End While
            End If

            Orareader.Close()
            Orareader = Nothing

            ret = True

        Catch ex As Exception
            ERRMSG = "�폜�Ɏ��s���܂���"
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        Finally
            If Not Orareader Is Nothing Then
                Orareader.Close()
            End If

            If ret = True Then
                '�g�����U�N�V�����I���iCOMMIT�j
                If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                    Call GSUB_LOG(0, "COMMIT���s")
                    ret = False
                End If
            Else
                '�g�����U�N�V�����I���iROLLBACK�j
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Call GSUB_LOG(0, "ROLLBACK���s")
                    ret = False
                End If
            End If
        End Try

        Return ret

    End Function

#End Region
#Region "DB���擾"                     '
    '�@�֐����@-�@GetALLGakkouCode
    '
    '�@�@�\    -  �S�w�Z�R�[�h�̎擾
    '
    '�@����    -  
    '
    '�@���l    -  
    '
    Private Function GetALLGakkouCode() As String()

        Dim Orareader As CASTCommon.MyOracleReader = Nothing

        Try
            '***���O���***
            STR_COMMAND = "GetALLGakkouCode"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***���O���**

            Orareader = New CASTCommon.MyOracleReader(MainDB)

            Dim SQL As String
            Dim cnt As Integer

            SQL = "SELECT COUNT(*) FROM GAKMAST2"

            If Orareader.DataReader(SQL) = False Then
                Return Nothing
            Else
                cnt = CInt(Orareader.GetItem("COUNT(*)"))
                If cnt = 0 Then
                    Exit Try
                End If
            End If

            Orareader.Close()
            Orareader = Nothing
            Orareader = New CASTCommon.MyOracleReader(MainDB)

            Dim GakkouCode(cnt - 1) As String '���ʔz��

            SQL = "SELECT GAKKOU_CODE_T FROM GAKMAST2"

            If Orareader.DataReader(SQL) = False Then
            Else
                cnt = 0
                While Orareader.EOF = False
                    GakkouCode(cnt) = Orareader.GetItem("GAKKOU_CODE_T")

                    cnt += 1
                    Orareader.NextRead()
                End While

                Return GakkouCode '���펞
            End If

            Orareader.Close()
            Orareader = Nothing

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        Finally
            If Not Orareader Is Nothing Then
                Orareader.Close()
            End If
        End Try

        Return Nothing '�ُ�

    End Function
    '
    '�@�֐����@-�@fn_Common_Check
    '
    '�@�@�\    -  �K�{���ڂ̓��̓`�F�b�N
    '
    '�@����    -  mode 1:�쐬���A2:�Q�Ǝ��A3:�X�V���A4:�폜��
    '
    '�@���l    -  
    '
    Private Function fn_Common_Check(ByVal mode As Integer) As Boolean

        Dim ret As Boolean = False

        Try
            '***���O���***
            STR_COMMAND = "fn_Common_Check"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***���O���**

            '���N�x�A���x���͒l�`�F�b�N
            Select Case True
                Case txt�Ώ۔N�x.Text.Trim = "" '***�Ώ۔N�x***
                    MessageBox.Show("�N�x�����͂���Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt�Ώ۔N�x.Focus()
                    Exit Try

                Case txt�Ώ۔N�x.Text.Trim.Length <> 4
                    MessageBox.Show("�N�x�̓��͂��s���ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt�Ώ۔N�x.Focus()
                    Exit Try

                Case Not IsNumeric(txt�Ώ۔N�x.Text)
                    MessageBox.Show("�N�x�̓��͂��s���ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt�Ώ۔N�x.Focus()
                    Exit Try

                Case txt�Ώی�.Text.Trim = "" '***�Ώی�***
                    MessageBox.Show("���x�����͂���Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt�Ώی�.Focus()
                    Exit Try

                Case txt�Ώی�.Text.Trim.Length <> 2
                    MessageBox.Show("���x�̓��͂��s���ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt�Ώی�.Focus()
                    Exit Try


                Case Not IsNumeric(txt�Ώی�.Text)
                    MessageBox.Show("���x�̓��͂��s���ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt�Ώی�.Focus()
                    Exit Try

                Case Not IsDate(Me.txt�Ώ۔N�x.Text & "/" & Me.txt�Ώی�.Text & "/" & "01")
                    MessageBox.Show("�N���x�̓��͂��s���ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt�Ώ۔N�x.Focus()
                    Exit Try
            End Select

            Dim sStart As String
            Dim sEnd As String

            '���w�Z�R�[�h���݃`�F�b�N
            If txtGakkou_Code.Text.Trim = "" Then
                MessageBox.Show("�w�Z�R�[�h�����͂���Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGakkou_Code.Focus()
                Exit Try
            Else
                '���l�`�F�b�N
                If IsNumeric(txtGakkou_Code.Text) = False Then
                    MessageBox.Show("�w�Z�R�[�h�̓��͒l���s���ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGakkou_Code.Focus()
                    Exit Try

                End If

                '�����`�F�b�N
                If txtGakkou_Code.Text.Trim.Length <> 10 Then
                    MessageBox.Show("�w�Z�R�[�h�̓��͒l���s���ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGakkou_Code.Focus()
                    Exit Try

                End If

                '�쐬���ƍ폜���͊w�Z�R�[�h��ALL9������
                If txtGakkou_Code.Text.Trim = "9999999999" Then
                    Select Case mode
                        Case 1, 4
                            '�w�Z�R�[�h��ALL9���쐬���ƍ폜���͊w�Z���݃`�F�b�N�����Ȃ�
                            '����I��
                            ret = True
                            Exit Try
                        Case Else
                            MessageBox.Show("�w�Z�R�[�h�̓��͒l���s���ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtGakkou_Code.Focus()
                            Exit Try
                    End Select
                End If

                Dim SQL As String

                '�w�Z�}�X�^���݃`�F�b�N
                SQL = "  SELECT *"
                SQL += " FROM GAKMAST2"
                SQL += " WHERE GAKKOU_CODE_T = '" & txtGakkou_Code.Text.Trim.PadLeft(txtGakkou_Code.MaxLength, "0"c) & "'"

                If GFUNC_SELECT_SQL2(SQL, 0) = False Then
                    MessageBox.Show("�w�Z�}�X�^�̌����Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Try
                End If

                If OBJ_DATAREADER.HasRows = False Then
                    MessageBox.Show("���͂��ꂽ�w�Z�R�[�h�����݂��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGakkou_Code.Focus()
                    Exit Try
                End If

                OBJ_DATAREADER.Read()

                sStart = Mid(OBJ_DATAREADER.Item("KAISI_DATE_T"), 1, 4)
                sEnd = Mid(OBJ_DATAREADER.Item("SYURYOU_DATE_T"), 1, 4)

                If (sStart <= txt�Ώ۔N�x.Text >= sEnd) = False Then
                    MessageBox.Show("�Ώ۔N�x�����͔͈͊O�ł�(" & sStart & "�`" & sEnd & ")", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt�Ώ۔N�x.Focus()
                    Exit Try
                End If

            End If

            ret = True

        Catch ex As Exception
            MessageBox.Show("�w�Z�}�X�^�̌����Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Call GSUB_LOG(0, "mode:" & mode & "�\�����ʃG���[:" & ex.ToString)
        Finally
            If OBJ_DATAREADER.IsClosed = False Then
                Call GFUNC_SELECT_SQL2("", 1)
            End If
        End Try

        Return ret

    End Function
    '
    '�@�֐����@-�@fn_IsExistG_SCHMASTNENGATUDO
    '
    '�@�@�\    -  �X�P�W���[���̑��݃`�F�b�N(�N���x�Ń`�F�b�N)
    '
    '�@����    -  Gakkou_Code,year,month
    '
    '�@���l    -  yyyyMM
    '
    Private Function fn_IsExistG_SCHMAST_NENGATUDO(ByVal Gakkou_Code As String, ByVal year As String, ByVal month As String) As Boolean

        Try
            '***���O���***
            STR_COMMAND = "fn_IsExistG_SCHMAST_NENGATUDO"
            STR_LOG_GAKKOU_CODE = Gakkou_Code
            STR_LOG_FURI_DATE = ""
            '***���O���**

            Dim nengetudo As String = year & month
            Dim SQL As String

            SQL = "  SELECT * FROM G_SCHMAST "
            SQL &= " WHERE GAKKOU_CODE_S = '" & Gakkou_Code & "'"
            SQL &= " AND NENGETUDO_S = '" & nengetudo & "'"


            If GFUNC_ISEXIST(SQL) = True Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        End Try

        Return False

    End Function
    '
    '�@�֐����@-�@fn_HolidayListSet
    '
    '�@�@�\    -  �x���}�X�^�擾
    '
    '�@����    -  
    '
    '�@���l    -  
    '
    Private Function fn_HolidayListSet() As Boolean

        Dim ret As Boolean = False

        Try
            '***���O���***
            STR_COMMAND = "fn_HolidayListSet"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***���O���**

            Dim SQL As String

            '�x�����̕\��
            Dim sTuki As String
            Dim sDay As String
            Dim sYName As String

            lst�x��.Items.Clear()

            If Trim(txt�Ώ۔N�x.Text) <> "" Then
                Select Case CInt(txt�Ώ۔N�x.Text)
                    Case Is > 1900
                        SQL = "  SELECT"
                        SQL += " YASUMI_DATE_Y"
                        SQL += ",YASUMI_NAME_Y"
                        SQL += " FROM YASUMIMAST"
                        SQL += " WHERE"
                        SQL += " YASUMI_DATE_Y > '" & txt�Ώ۔N�x.Text & "0400'"
                        SQL += " AND"
                        SQL += " YASUMI_DATE_Y < '" & CStr(CInt(txt�Ώ۔N�x.Text) + 1) & "0399'"
                        SQL += " ORDER BY YASUMI_DATE_Y ASC"

                        If GFUNC_SELECT_SQL2(SQL, 0) = False Then
                            MessageBox.Show("�Ώ۔N�x�̋x����񂪓o�^����Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Try
                        End If

                        While (OBJ_DATAREADER.Read = True)
                            With OBJ_DATAREADER
                                sTuki = Mid(.Item("YASUMI_DATE_Y"), 5, 2)
                                sDay = Mid(.Item("YASUMI_DATE_Y"), 7, 2)
                                sYName = Trim(.Item("YASUMI_NAME_Y"))

                                lst�x��.Items.Add(sTuki & "��" & sDay & "��" & Space(1) & sYName)

                                StrYasumi_List(StrYasumi_List.Length - 1) = txt�Ώ۔N�x.Text & sTuki & sDay
                                ReDim Preserve StrYasumi_List(StrYasumi_List.Length)
                            End With
                        End While

                        ReDim Preserve StrYasumi_List(StrYasumi_List.Length - 1)

                    Case Else
                        MessageBox.Show("�Ώ۔N�x��1900�N�ȍ~����͂��Ă�������", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt�Ώ۔N�x.Focus()
                        Exit Try
                End Select
            End If

            ret = True

        Catch ex As Exception
            MessageBox.Show("�x�����̎擾�Ɏ��s���܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        Finally
            If OBJ_DATAREADER.IsClosed = False Then
                Call GFUNC_SELECT_SQL2("", 1)
            End If
        End Try

        Return ret

    End Function
    '
    '�@�֐����@-�@fn_IsExistYASUMIMAST
    '
    '�@�@�\    -  �x���}�X�^���݃`�F�b�N
    '
    '�@����    -  Str�N����
    '
    '�@���l    -  
    '�@
    Private Function fn_IsExistYASUMIMAST(ByVal Str�N���� As String) As Boolean

        '�x���}�X�^���݃`�F�b�N
        fn_IsExistYASUMIMAST = False

        Try
            '***���O���***
            STR_COMMAND = "fn_IsExistYASUMIMAST"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***���O���**

            For cnt As Integer = 0 To StrYasumi_List.Length - 1
                If Not StrYasumi_List(cnt) Is Nothing Then
                    If StrYasumi_List(cnt).Trim = Str�N���� Then
                        fn_IsExistYASUMIMAST = True
                        Exit For
                    End If
                End If
            Next

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        End Try

    End Function

#End Region
#Region "���t�Z�o"
    '
    '�@�֐����@-�@fn_GetFuriDate
    '
    '�@�@�\    -  �U�֓��̋x���␳���s�Ȃ�
    '
    '�@����    -  StrFuriYear�AStrFuriMonth�AStrFuriDay�AStrSchKbn�AStrFuriKbn�AaInfoGakkou
    '
    '�@���l    -  
    '
    '�@
    Private Function fn_GetFuriDate(ByVal StrFuriYear As String, _
                                    ByVal StrFuriMonth As String, _
                                    ByVal StrFuriDay As String, _
                                    ByVal StrSchKbn As String, _
                                    ByVal StrFuriKbn As String, _
                                    ByVal aInfoGakkou As GAKDATA) As String

        Dim FuriDate As String = "err"

        Try
            '***���O���***
            STR_COMMAND = "fn_GetFuriDate"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***���O���**

            '�U�֓���ݒ�
            Select Case StrSchKbn
                Case "0" '�ʏ�
                    '�U�֓��������͂̏ꍇ
                    If StrFuriDay = "" Then
                        Select Case StrFuriKbn
                            Case "0" '���U
                                FuriDate = StrFuriYear & StrFuriMonth & aInfoGakkou.FURI_DATE
                            Case "1" '�ĐU
                                FuriDate = StrFuriYear & StrFuriMonth & aInfoGakkou.SFURI_DATE
                        End Select
                    Else
                        FuriDate = StrFuriYear & StrFuriMonth & StrFuriDay
                    End If
                Case "2"     '����
                    FuriDate = StrFuriYear & StrFuriMonth & StrFuriDay
            End Select

            '�U�֓��x���␳
            Select Case StrFuriKbn
                Case "0", "1", "2"
                    '�U�֓��x���␳(�o���x���R�[�h�ŕ␳)
                    Select Case aInfoGakkou.SKYU_CODE_T
                        Case "0"
                            '���c�Ɠ�
                            FuriDate = fn_GetEigyoubi(FuriDate, "0", "+")
                        Case "1"
                            '�O�c�Ɠ�
                            FuriDate = fn_GetEigyoubi(FuriDate, "0", "-")
                    End Select
                Case "3"
                    '�U�֓��x���␳(�����x���R�[�h�ŕ␳)
                    Select Case aInfoGakkou.NKYU_CODE_T
                        Case "0"
                            '���c�Ɠ�
                            FuriDate = fn_GetEigyoubi(FuriDate, "0", "+")
                        Case "1"
                            '�O�c�Ɠ�
                            FuriDate = fn_GetEigyoubi(FuriDate, "0", "-")
                    End Select
            End Select

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            FuriDate = "err"
        End Try

        Return FuriDate

    End Function

#End Region
#Region "��ʐ���"
    '
    '�@�֐����@-�@Sb_Btn_Enable
    '
    '�@�@�\    -  �{�^������ 
    '
    '�@����    -  pIndex
    '
    '�@���l    -  
    '
    Private Sub Sb_Btn_Enable(Optional ByVal pIndex As Integer = 0)

        Select Case pIndex
            Case 0
                btnAction.Enabled = True
                btnFind.Enabled = True
                btnUPDATE.Enabled = False
                btnEraser.Enabled = True
                txtGakkou_Code.Enabled = True
                cmbGakkouName.Enabled = True
                cmbKana.Enabled = True
                txt�Ώ۔N�x.Enabled = True
                txt�Ώی�.Enabled = True
            Case 1
                btnAction.Enabled = False
                btnFind.Enabled = True
                btnUPDATE.Enabled = True
                btnEraser.Enabled = True
                txtGakkou_Code.Enabled = False
                cmbGakkouName.Enabled = False
                cmbKana.Enabled = False
                txt�Ώ۔N�x.Enabled = False
                txt�Ώی�.Enabled = False
            Case 2
                btnAction.Enabled = False
                btnFind.Enabled = True
                btnUPDATE.Enabled = False
                btnEraser.Enabled = True
                txtGakkou_Code.Enabled = True
                cmbGakkouName.Enabled = True
                cmbKana.Enabled = True
                txt�Ώ۔N�x.Enabled = True
                txt�Ώی�.Enabled = True
        End Select

    End Sub
    '
    '�@�֐����@-�@Sb_Sansyou_Focus
    '
    '�@�@�\    -  �X�P�W���[���̑��݃`�F�b�N���s���A���݂���΁A�Q�ƂɃt�H�[�J�X���Z�b�g
    '
    '�@����    -  
    '
    '�@���l    -  
    '
    '�@
    Private Sub Sb_Sansyou_Focus()

        Dim SQL As String

        SQL = "  SELECT * FROM G_SCHMAST"
        SQL += " WHERE"
        SQL += " GAKKOU_CODE_S ='" & Trim(txtGakkou_Code.Text) & "'"
        SQL += " AND"
        SQL += " NENGETUDO_S ='" & Trim(txt�Ώ۔N�x.Text) & txt�Ώی�.Text.Trim & "'"

        If GFUNC_ISEXIST(SQL) = True Then
            btnAction.Enabled = False
            btnFind.Enabled = True
            btnFind.Focus()
            Me.btnUPDATE.Enabled = False
        Else '�ǉ� 2007/02/15
            btnFind.Enabled = False
            btnAction.Enabled = True
            btnAction.Focus()
            Me.btnUPDATE.Enabled = False
        End If

    End Sub
    '
    '�@�֐����@-�@Sb_AllGakunenChkBox_Control
    '
    '�@�@�\    -  �`�F�b�N�{�b�N�X�̃R���g���[��
    '
    '�@����    - 
    '
    '�@���l    -  
    '
    '�@
    Private Sub Sb_AllGakunenChkBox_Control(ByVal SIYOU_GAKUNEN As Integer, _
                                            ByVal ChkBoxAll As CheckBox, _
                                            ByVal ChkBox1 As CheckBox, _
                                            ByVal ChkBox2 As CheckBox, _
                                            ByVal ChkBox3 As CheckBox, _
                                            ByVal ChkBox4 As CheckBox, _
                                            ByVal ChkBox5 As CheckBox, _
                                            ByVal ChkBox6 As CheckBox, _
                                            ByVal ChkBox7 As CheckBox, _
                                            ByVal ChkBox8 As CheckBox, _
                                            ByVal ChkBox9 As CheckBox)

        If ChkBoxAll.Checked = True Then
            '�e�w�N�̃`�F�b�N������
            ChkBox1.Checked = False
            ChkBox2.Checked = False
            ChkBox3.Checked = False
            ChkBox4.Checked = False
            ChkBox5.Checked = False
            ChkBox6.Checked = False
            ChkBox7.Checked = False
            ChkBox8.Checked = False
            ChkBox9.Checked = False
            '�e�w�N�̃`�F�b�N�{�b�N�X�g�p�s�� 
            ChkBox1.Enabled = False
            ChkBox2.Enabled = False
            ChkBox3.Enabled = False
            ChkBox4.Enabled = False
            ChkBox5.Enabled = False
            ChkBox6.Enabled = False
            ChkBox7.Enabled = False
            ChkBox8.Enabled = False
            ChkBox9.Enabled = False
        Else
            ChkBox1.Enabled = True
            ChkBox2.Enabled = True
            ChkBox3.Enabled = True
            ChkBox4.Enabled = True
            ChkBox5.Enabled = True
            ChkBox6.Enabled = True
            ChkBox7.Enabled = True
            ChkBox8.Enabled = True
            ChkBox9.Enabled = True

            '�e�w�N�̃`�F�b�N�{�b�N�X�g�p�� 
            If SIYOU_GAKUNEN = 0 Then
                Exit Sub
            End If

            If SIYOU_GAKUNEN < 9 Then
                ChkBox9.Enabled = False
            End If

            If SIYOU_GAKUNEN < 8 Then
                ChkBox8.Enabled = False
            End If

            If SIYOU_GAKUNEN < 7 Then
                ChkBox7.Enabled = False
            End If

            If SIYOU_GAKUNEN < 6 Then
                ChkBox6.Enabled = False
            End If

            If SIYOU_GAKUNEN < 5 Then
                ChkBox5.Enabled = False
            End If

            If SIYOU_GAKUNEN < 4 Then
                ChkBox4.Enabled = False
            End If

            If SIYOU_GAKUNEN < 3 Then
                ChkBox3.Enabled = False
            End If

            If SIYOU_GAKUNEN < 2 Then
                ChkBox2.Enabled = False
            End If

            ChkBox1.Enabled = True

        End If
    End Sub
    '
    '�@�֐����@-�@Sb_SaifuriProtect
    '
    '�@�@�\    -  �L���ĐU���̐���
    '
    '�@����    -  pValue ������(True,False) 
    '�@�@�@�@�@�@ 
    '�@���l    -  
    '
    Private Sub Sb_SaifuriProtect(ByVal pValue As Boolean)

        '�U�֓��L���`�F�b�N�ƐU�֓����͗��̃v���e�N�g(ON/OFF)����
        '�ʏ�U�֓��^�u
        Chk�L���ĐU���ʏ�1.Checked = False
        Chk�L���ĐU���ʏ�1.Enabled = pValue
        txt�ʏ�ĐU��1.Enabled = pValue
        Chk�L���ĐU���ʏ�2.Checked = False
        Chk�L���ĐU���ʏ�2.Enabled = pValue
        txt�ʏ�ĐU��2.Enabled = pValue
        Chk�L���ĐU���ʏ�3.Checked = False
        Chk�L���ĐU���ʏ�3.Enabled = pValue
        txt�ʏ�ĐU��3.Enabled = pValue
        Chk�L���ĐU���ʏ�4.Checked = False
        Chk�L���ĐU���ʏ�4.Enabled = pValue
        txt�ʏ�ĐU��4.Enabled = pValue
        Chk�L���ĐU���ʏ�5.Checked = False
        Chk�L���ĐU���ʏ�5.Enabled = pValue
        txt�ʏ�ĐU��5.Enabled = pValue
    End Sub

    '�@�֐����@-�@Sb_SiyouGakunenChkEnabled
    '
    '�@�@�\    -  �g�p���Ă��Ȃ��w�N�̃`�F�b�N�{�b�N�X���g�p�s�ɂ���
    '
    '�@����    -  SiyouGakunen
    '
    '�@���l    -  ��ʏ�ɍ��ڒǉ�
    '
    Private Sub Sb_SiyouGakunenChkEnabled(ByVal SiyouGakunen As Integer)

        If SiyouGakunen = 0 Then
            Exit Sub
        End If

        If SiyouGakunen < 9 Then
            Chk�w�N�ʏ�1_9.Enabled = False
            Chk�w�N�ʏ�2_9.Enabled = False
            Chk�w�N�ʏ�3_9.Enabled = False
            Chk�w�N�ʏ�4_9.Enabled = False
            Chk�w�N�ʏ�5_9.Enabled = False

            Chk����1_9�w�N.Enabled = False
            Chk����2_9�w�N.Enabled = False
            Chk����3_9�w�N.Enabled = False
            Chk����4_9�w�N.Enabled = False
            Chk����5_9�w�N.Enabled = False
            Chk����6_9�w�N.Enabled = False
        End If

        If SiyouGakunen < 8 Then
            Chk�w�N�ʏ�1_8.Enabled = False
            Chk�w�N�ʏ�2_8.Enabled = False
            Chk�w�N�ʏ�3_8.Enabled = False
            Chk�w�N�ʏ�4_8.Enabled = False
            Chk�w�N�ʏ�5_8.Enabled = False

            Chk����1_8�w�N.Enabled = False
            Chk����2_8�w�N.Enabled = False
            Chk����3_8�w�N.Enabled = False
            Chk����4_8�w�N.Enabled = False
            Chk����5_8�w�N.Enabled = False
            Chk����6_8�w�N.Enabled = False
        End If

        If SiyouGakunen < 7 Then
            Chk�w�N�ʏ�1_7.Enabled = False
            Chk�w�N�ʏ�2_7.Enabled = False
            Chk�w�N�ʏ�3_7.Enabled = False
            Chk�w�N�ʏ�4_7.Enabled = False
            Chk�w�N�ʏ�5_7.Enabled = False

            Chk����1_7�w�N.Enabled = False
            Chk����2_7�w�N.Enabled = False
            Chk����3_7�w�N.Enabled = False
            Chk����4_7�w�N.Enabled = False
            Chk����5_7�w�N.Enabled = False
            Chk����6_7�w�N.Enabled = False
        End If

        If SiyouGakunen < 6 Then
            Chk�w�N�ʏ�1_6.Enabled = False
            Chk�w�N�ʏ�2_6.Enabled = False
            Chk�w�N�ʏ�3_6.Enabled = False
            Chk�w�N�ʏ�4_6.Enabled = False
            Chk�w�N�ʏ�5_6.Enabled = False

            Chk����1_6�w�N.Enabled = False
            Chk����2_6�w�N.Enabled = False
            Chk����3_6�w�N.Enabled = False
            Chk����4_6�w�N.Enabled = False
            Chk����5_6�w�N.Enabled = False
            Chk����6_6�w�N.Enabled = False
        End If

        If SiyouGakunen < 5 Then
            Chk�w�N�ʏ�1_5.Enabled = False
            Chk�w�N�ʏ�2_5.Enabled = False
            Chk�w�N�ʏ�3_5.Enabled = False
            Chk�w�N�ʏ�4_5.Enabled = False
            Chk�w�N�ʏ�5_5.Enabled = False

            Chk����1_5�w�N.Enabled = False
            Chk����2_5�w�N.Enabled = False
            Chk����3_5�w�N.Enabled = False
            Chk����4_5�w�N.Enabled = False
            Chk����5_5�w�N.Enabled = False
            Chk����6_5�w�N.Enabled = False
        End If

        If SiyouGakunen < 4 Then
            Chk�w�N�ʏ�1_4.Enabled = False
            Chk�w�N�ʏ�2_4.Enabled = False
            Chk�w�N�ʏ�3_4.Enabled = False
            Chk�w�N�ʏ�4_4.Enabled = False
            Chk�w�N�ʏ�5_4.Enabled = False

            Chk����1_4�w�N.Enabled = False
            Chk����2_4�w�N.Enabled = False
            Chk����3_4�w�N.Enabled = False
            Chk����4_4�w�N.Enabled = False
            Chk����5_4�w�N.Enabled = False
            Chk����6_4�w�N.Enabled = False
        End If

        If SiyouGakunen < 3 Then
            Chk�w�N�ʏ�1_3.Enabled = False
            Chk�w�N�ʏ�2_3.Enabled = False
            Chk�w�N�ʏ�3_3.Enabled = False
            Chk�w�N�ʏ�4_3.Enabled = False
            Chk�w�N�ʏ�5_3.Enabled = False

            Chk����1_3�w�N.Enabled = False
            Chk����2_3�w�N.Enabled = False
            Chk����3_3�w�N.Enabled = False
            Chk����4_3�w�N.Enabled = False
            Chk����5_3�w�N.Enabled = False
            Chk����6_3�w�N.Enabled = False
        End If

        If SiyouGakunen < 2 Then
            Chk�w�N�ʏ�1_2.Enabled = False
            Chk�w�N�ʏ�2_2.Enabled = False
            Chk�w�N�ʏ�3_2.Enabled = False
            Chk�w�N�ʏ�4_2.Enabled = False
            Chk�w�N�ʏ�5_2.Enabled = False

            Chk����1_2�w�N.Enabled = False
            Chk����2_2�w�N.Enabled = False
            Chk����3_2�w�N.Enabled = False
            Chk����4_2�w�N.Enabled = False
            Chk����5_2�w�N.Enabled = False
            Chk����6_2�w�N.Enabled = False
        End If

    End Sub
#End Region
#Region "�\���̐���"
    '
    '�@�֐����@-�@Sb_GetData
    '
    '�@�@�\    -   �X�P�W���[���}�X�^�����\���̂ɃZ�b�g
    '
    '�@����    -  Get_TuujyouData,Get_ZuijiData
    '
    '�@���l    -  
    '
    Private Sub Sb_GetData(ByRef Get_TuujyouData() As TuujyouData, ByRef Get_ZuijiData() As ZuijiData)

        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '�ʏ�U�֓������\���̂ɃZ�b�g
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '�X�P�W���[���^�u��ʂŌ��ݕ\������Ă��鍀�ڂ̓��e���\���̂Ɏ擾

        Get_TuujyouData(1).Seikyu_Nen = txt�Ώ۔N�x.Text.Trim
        Get_TuujyouData(1).Seikyu_Tuki = txt�Ώی�.Text.Trim
        '�U�֓�
        Get_TuujyouData(1).Furikae_Check = Chk�L���U�֓��ʏ�1.Checked
        Get_TuujyouData(1).Furikae_Tuki = txt�Ώی�.Text.Trim
        Get_TuujyouData(1).Furikae_Date = txt�ʏ�U�֓�1.Text.Trim
        '�x���␳��̐U�֓�
        Get_TuujyouData(1).Furikae_Day = Replace(lbl�ʏ�U�֓�1.Text.Trim, "/", "")

        '�ĐU��
        Get_TuujyouData(1).SaiFurikae_Check = Chk�L���ĐU���ʏ�1.Checked
        Get_TuujyouData(1).SaiFurikae_Tuki = txt�Ώی�.Text.Trim
        Get_TuujyouData(1).SaiFurikae_Date = txt�ʏ�ĐU��1.Text
        '�x���␳��̍ĐU��
        Get_TuujyouData(1).SaiFurikae_Day = Replace(lbl�ʏ�ĐU��1.Text.Trim, "/", "")

        Select Case Chk�w�N�ʏ�1_�S.Checked
            Case True
                Get_TuujyouData(1).SiyouGakunenALL_Check = True
                Get_TuujyouData(1).SiyouGakunen1_Check = True
                Get_TuujyouData(1).SiyouGakunen2_Check = True
                Get_TuujyouData(1).SiyouGakunen3_Check = True
                Get_TuujyouData(1).SiyouGakunen4_Check = True
                Get_TuujyouData(1).SiyouGakunen5_Check = True
                Get_TuujyouData(1).SiyouGakunen6_Check = True
                Get_TuujyouData(1).SiyouGakunen7_Check = True
                Get_TuujyouData(1).SiyouGakunen8_Check = True
                Get_TuujyouData(1).SiyouGakunen9_Check = True
            Case False
                Get_TuujyouData(1).SiyouGakunenALL_Check = False
                Get_TuujyouData(1).SiyouGakunen1_Check = Chk�w�N�ʏ�1_1.Checked
                Get_TuujyouData(1).SiyouGakunen2_Check = Chk�w�N�ʏ�1_2.Checked
                Get_TuujyouData(1).SiyouGakunen3_Check = Chk�w�N�ʏ�1_3.Checked
                Get_TuujyouData(1).SiyouGakunen4_Check = Chk�w�N�ʏ�1_4.Checked
                Get_TuujyouData(1).SiyouGakunen5_Check = Chk�w�N�ʏ�1_5.Checked
                Get_TuujyouData(1).SiyouGakunen6_Check = Chk�w�N�ʏ�1_6.Checked
                Get_TuujyouData(1).SiyouGakunen7_Check = Chk�w�N�ʏ�1_7.Checked
                Get_TuujyouData(1).SiyouGakunen8_Check = Chk�w�N�ʏ�1_8.Checked
                Get_TuujyouData(1).SiyouGakunen9_Check = Chk�w�N�ʏ�1_9.Checked
        End Select

        Get_TuujyouData(2).Seikyu_Nen = txt�Ώ۔N�x.Text.Trim
        Get_TuujyouData(2).Seikyu_Tuki = txt�Ώی�.Text.Trim
        '�U�֓�
        Get_TuujyouData(2).Furikae_Check = Chk�L���U�֓��ʏ�2.Checked
        Get_TuujyouData(2).Furikae_Tuki = txt�Ώی�.Text.Trim
        Get_TuujyouData(2).Furikae_Date = txt�ʏ�U�֓�2.Text.Trim
        '�x���␳��̐U�֓�
        Get_TuujyouData(2).Furikae_Day = Replace(lbl�ʏ�U�֓�2.Text.Trim, "/", "")

        '�ĐU��
        Get_TuujyouData(2).SaiFurikae_Check = Chk�L���ĐU���ʏ�2.Checked
        Get_TuujyouData(2).SaiFurikae_Tuki = txt�Ώی�.Text.Trim
        Get_TuujyouData(2).SaiFurikae_Date = txt�ʏ�ĐU��2.Text
        '�x���␳��̍ĐU��
        Get_TuujyouData(2).SaiFurikae_Day = Replace(lbl�ʏ�ĐU��2.Text.Trim, "/", "")


        Select Case Chk�w�N�ʏ�2_�S.Checked
            Case True
                Get_TuujyouData(2).SiyouGakunenALL_Check = True
                Get_TuujyouData(2).SiyouGakunen1_Check = True
                Get_TuujyouData(2).SiyouGakunen2_Check = True
                Get_TuujyouData(2).SiyouGakunen3_Check = True
                Get_TuujyouData(2).SiyouGakunen4_Check = True
                Get_TuujyouData(2).SiyouGakunen5_Check = True
                Get_TuujyouData(2).SiyouGakunen6_Check = True
                Get_TuujyouData(2).SiyouGakunen7_Check = True
                Get_TuujyouData(2).SiyouGakunen8_Check = True
                Get_TuujyouData(2).SiyouGakunen9_Check = True
            Case False
                Get_TuujyouData(2).SiyouGakunenALL_Check = False
                Get_TuujyouData(2).SiyouGakunen1_Check = Chk�w�N�ʏ�2_1.Checked
                Get_TuujyouData(2).SiyouGakunen2_Check = Chk�w�N�ʏ�2_2.Checked
                Get_TuujyouData(2).SiyouGakunen3_Check = Chk�w�N�ʏ�2_3.Checked
                Get_TuujyouData(2).SiyouGakunen4_Check = Chk�w�N�ʏ�2_4.Checked
                Get_TuujyouData(2).SiyouGakunen5_Check = Chk�w�N�ʏ�2_5.Checked
                Get_TuujyouData(2).SiyouGakunen6_Check = Chk�w�N�ʏ�2_6.Checked
                Get_TuujyouData(2).SiyouGakunen7_Check = Chk�w�N�ʏ�2_7.Checked
                Get_TuujyouData(2).SiyouGakunen8_Check = Chk�w�N�ʏ�2_8.Checked
                Get_TuujyouData(2).SiyouGakunen9_Check = Chk�w�N�ʏ�2_9.Checked
        End Select

        Get_TuujyouData(3).Seikyu_Nen = txt�Ώ۔N�x.Text.Trim
        Get_TuujyouData(3).Seikyu_Tuki = txt�Ώی�.Text.Trim
        '�U�֓�
        Get_TuujyouData(3).Furikae_Check = Chk�L���U�֓��ʏ�3.Checked
        Get_TuujyouData(3).Furikae_Tuki = txt�Ώی�.Text.Trim
        Get_TuujyouData(3).Furikae_Date = txt�ʏ�U�֓�3.Text.Trim
        '�x���␳��̐U�֓�
        Get_TuujyouData(3).Furikae_Day = Replace(lbl�ʏ�U�֓�3.Text.Trim, "/", "")

        '�ĐU��
        Get_TuujyouData(3).SaiFurikae_Check = Chk�L���ĐU���ʏ�3.Checked
        Get_TuujyouData(3).SaiFurikae_Tuki = txt�Ώی�.Text.Trim
        Get_TuujyouData(3).SaiFurikae_Date = txt�ʏ�ĐU��3.Text
        '�x���␳��̍ĐU��
        Get_TuujyouData(3).SaiFurikae_Day = Replace(lbl�ʏ�ĐU��3.Text.Trim, "/", "")

        Select Case Chk�w�N�ʏ�3_�S.Checked
            Case True
                Get_TuujyouData(3).SiyouGakunenALL_Check = True
                Get_TuujyouData(3).SiyouGakunen1_Check = True
                Get_TuujyouData(3).SiyouGakunen2_Check = True
                Get_TuujyouData(3).SiyouGakunen3_Check = True
                Get_TuujyouData(3).SiyouGakunen4_Check = True
                Get_TuujyouData(3).SiyouGakunen5_Check = True
                Get_TuujyouData(3).SiyouGakunen6_Check = True
                Get_TuujyouData(3).SiyouGakunen7_Check = True
                Get_TuujyouData(3).SiyouGakunen8_Check = True
                Get_TuujyouData(3).SiyouGakunen9_Check = True
            Case False
                Get_TuujyouData(3).SiyouGakunenALL_Check = False
                Get_TuujyouData(3).SiyouGakunen1_Check = Chk�w�N�ʏ�3_1.Checked
                Get_TuujyouData(3).SiyouGakunen2_Check = Chk�w�N�ʏ�3_2.Checked
                Get_TuujyouData(3).SiyouGakunen3_Check = Chk�w�N�ʏ�3_3.Checked
                Get_TuujyouData(3).SiyouGakunen4_Check = Chk�w�N�ʏ�3_4.Checked
                Get_TuujyouData(3).SiyouGakunen5_Check = Chk�w�N�ʏ�3_5.Checked
                Get_TuujyouData(3).SiyouGakunen6_Check = Chk�w�N�ʏ�3_6.Checked
                Get_TuujyouData(3).SiyouGakunen7_Check = Chk�w�N�ʏ�3_7.Checked
                Get_TuujyouData(3).SiyouGakunen8_Check = Chk�w�N�ʏ�3_8.Checked
                Get_TuujyouData(3).SiyouGakunen9_Check = Chk�w�N�ʏ�3_9.Checked
        End Select

        Get_TuujyouData(4).Seikyu_Nen = txt�Ώ۔N�x.Text.Trim
        Get_TuujyouData(4).Seikyu_Tuki = txt�Ώی�.Text.Trim
        '�U�֓�
        Get_TuujyouData(4).Furikae_Check = Chk�L���U�֓��ʏ�4.Checked
        Get_TuujyouData(4).Furikae_Tuki = txt�Ώی�.Text.Trim
        Get_TuujyouData(4).Furikae_Date = txt�ʏ�U�֓�4.Text.Trim
        '�x���␳��̐U�֓�
        Get_TuujyouData(4).Furikae_Day = Replace(lbl�ʏ�U�֓�4.Text.Trim, "/", "")

        '�ĐU��
        Get_TuujyouData(4).SaiFurikae_Check = Chk�L���ĐU���ʏ�4.Checked
        Get_TuujyouData(4).SaiFurikae_Tuki = txt�Ώی�.Text.Trim
        Get_TuujyouData(4).SaiFurikae_Date = txt�ʏ�ĐU��4.Text
        '�x���␳��̍ĐU��
        Get_TuujyouData(4).SaiFurikae_Day = Replace(lbl�ʏ�ĐU��4.Text.Trim, "/", "")


        Select Case Chk�w�N�ʏ�4_�S.Checked
            Case True
                Get_TuujyouData(4).SiyouGakunenALL_Check = True
                Get_TuujyouData(4).SiyouGakunen1_Check = True
                Get_TuujyouData(4).SiyouGakunen2_Check = True
                Get_TuujyouData(4).SiyouGakunen3_Check = True
                Get_TuujyouData(4).SiyouGakunen4_Check = True
                Get_TuujyouData(4).SiyouGakunen5_Check = True
                Get_TuujyouData(4).SiyouGakunen6_Check = True
                Get_TuujyouData(4).SiyouGakunen7_Check = True
                Get_TuujyouData(4).SiyouGakunen8_Check = True
                Get_TuujyouData(4).SiyouGakunen9_Check = True
            Case False
                Get_TuujyouData(4).SiyouGakunenALL_Check = False
                Get_TuujyouData(4).SiyouGakunen1_Check = Chk�w�N�ʏ�4_1.Checked
                Get_TuujyouData(4).SiyouGakunen2_Check = Chk�w�N�ʏ�4_2.Checked
                Get_TuujyouData(4).SiyouGakunen3_Check = Chk�w�N�ʏ�4_3.Checked
                Get_TuujyouData(4).SiyouGakunen4_Check = Chk�w�N�ʏ�4_4.Checked
                Get_TuujyouData(4).SiyouGakunen5_Check = Chk�w�N�ʏ�4_5.Checked
                Get_TuujyouData(4).SiyouGakunen6_Check = Chk�w�N�ʏ�4_6.Checked
                Get_TuujyouData(4).SiyouGakunen7_Check = Chk�w�N�ʏ�4_7.Checked
                Get_TuujyouData(4).SiyouGakunen8_Check = Chk�w�N�ʏ�4_8.Checked
                Get_TuujyouData(4).SiyouGakunen9_Check = Chk�w�N�ʏ�4_9.Checked
        End Select

        Get_TuujyouData(5).Seikyu_Nen = txt�Ώ۔N�x.Text.Trim
        Get_TuujyouData(5).Seikyu_Tuki = txt�Ώی�.Text.Trim
        '�U�֓�
        Get_TuujyouData(5).Furikae_Check = Chk�L���U�֓��ʏ�5.Checked
        Get_TuujyouData(5).Furikae_Tuki = txt�Ώی�.Text.Trim
        Get_TuujyouData(5).Furikae_Date = txt�ʏ�U�֓�5.Text.Trim
        '�x���␳��̐U�֓�
        Get_TuujyouData(5).Furikae_Day = Replace(lbl�ʏ�U�֓�5.Text.Trim, "/", "")

        '�ĐU��
        Get_TuujyouData(5).SaiFurikae_Check = Chk�L���ĐU���ʏ�5.Checked
        Get_TuujyouData(5).SaiFurikae_Tuki = txt�Ώی�.Text.Trim
        Get_TuujyouData(5).SaiFurikae_Date = txt�ʏ�ĐU��5.Text
        '�x���␳��̍ĐU��
        Get_TuujyouData(5).SaiFurikae_Day = Replace(lbl�ʏ�ĐU��5.Text.Trim, "/", "")

        Select Case Chk�w�N�ʏ�5_�S.Checked
            Case True
                Get_TuujyouData(5).SiyouGakunenALL_Check = True
                Get_TuujyouData(5).SiyouGakunen1_Check = True
                Get_TuujyouData(5).SiyouGakunen2_Check = True
                Get_TuujyouData(5).SiyouGakunen3_Check = True
                Get_TuujyouData(5).SiyouGakunen4_Check = True
                Get_TuujyouData(5).SiyouGakunen5_Check = True
                Get_TuujyouData(5).SiyouGakunen6_Check = True
                Get_TuujyouData(5).SiyouGakunen7_Check = True
                Get_TuujyouData(5).SiyouGakunen8_Check = True
                Get_TuujyouData(5).SiyouGakunen9_Check = True
            Case False
                Get_TuujyouData(5).SiyouGakunenALL_Check = False
                Get_TuujyouData(5).SiyouGakunen1_Check = Chk�w�N�ʏ�5_1.Checked
                Get_TuujyouData(5).SiyouGakunen2_Check = Chk�w�N�ʏ�5_2.Checked
                Get_TuujyouData(5).SiyouGakunen3_Check = Chk�w�N�ʏ�5_3.Checked
                Get_TuujyouData(5).SiyouGakunen4_Check = Chk�w�N�ʏ�5_4.Checked
                Get_TuujyouData(5).SiyouGakunen5_Check = Chk�w�N�ʏ�5_5.Checked
                Get_TuujyouData(5).SiyouGakunen6_Check = Chk�w�N�ʏ�5_6.Checked
                Get_TuujyouData(5).SiyouGakunen7_Check = Chk�w�N�ʏ�5_7.Checked
                Get_TuujyouData(5).SiyouGakunen8_Check = Chk�w�N�ʏ�5_8.Checked
                Get_TuujyouData(5).SiyouGakunen9_Check = Chk�w�N�ʏ�5_9.Checked
        End Select

        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '�����U�֓������\���̂ɃZ�b�g
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '�����X�P�W���[���^�u��ʂŌ��ݕ\������Ă��鍀�ڂ̓��e���\���̂Ɏ擾
        Get_ZuijiData(1).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪1)
        Get_ZuijiData(1).Furikae_Nen = txt�Ώ۔N�x.Text.Trim
        Get_ZuijiData(1).Furikae_Tuki = txt�Ώی�.Text
        Get_ZuijiData(1).Furikae_Date = txt�����U�֓�1.Text

        Get_ZuijiData(1).Furikae_Day = Replace(lbl�����U�֓�1.Text.Trim, "/", "")

        Select Case Chk����1_�S�w�N.Checked
            Case True
                Get_ZuijiData(1).SiyouGakunen1_Check = True
                Get_ZuijiData(1).SiyouGakunen2_Check = True
                Get_ZuijiData(1).SiyouGakunen3_Check = True
                Get_ZuijiData(1).SiyouGakunen4_Check = True
                Get_ZuijiData(1).SiyouGakunen5_Check = True
                Get_ZuijiData(1).SiyouGakunen6_Check = True
                Get_ZuijiData(1).SiyouGakunen7_Check = True
                Get_ZuijiData(1).SiyouGakunen8_Check = True
                Get_ZuijiData(1).SiyouGakunen9_Check = True
            Case False
                Get_ZuijiData(1).SiyouGakunen1_Check = Chk����1_1�w�N.Checked
                Get_ZuijiData(1).SiyouGakunen2_Check = Chk����1_2�w�N.Checked
                Get_ZuijiData(1).SiyouGakunen3_Check = Chk����1_3�w�N.Checked
                Get_ZuijiData(1).SiyouGakunen4_Check = Chk����1_4�w�N.Checked
                Get_ZuijiData(1).SiyouGakunen5_Check = Chk����1_5�w�N.Checked
                Get_ZuijiData(1).SiyouGakunen6_Check = Chk����1_6�w�N.Checked
                Get_ZuijiData(1).SiyouGakunen7_Check = Chk����1_7�w�N.Checked
                Get_ZuijiData(1).SiyouGakunen8_Check = Chk����1_8�w�N.Checked
                Get_ZuijiData(1).SiyouGakunen9_Check = Chk����1_9�w�N.Checked
        End Select

        Get_ZuijiData(2).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪2)
        Get_ZuijiData(2).Furikae_Nen = txt�Ώ۔N�x.Text.Trim
        Get_ZuijiData(2).Furikae_Tuki = txt�Ώی�.Text
        Get_ZuijiData(2).Furikae_Date = txt�����U�֓�2.Text

        Get_ZuijiData(2).Furikae_Day = Replace(lbl�����U�֓�2.Text.Trim, "/", "")

        Select Case Chk����2_�S�w�N.Checked
            Case True
                Get_ZuijiData(2).SiyouGakunen1_Check = True
                Get_ZuijiData(2).SiyouGakunen2_Check = True
                Get_ZuijiData(2).SiyouGakunen3_Check = True
                Get_ZuijiData(2).SiyouGakunen4_Check = True
                Get_ZuijiData(2).SiyouGakunen5_Check = True
                Get_ZuijiData(2).SiyouGakunen6_Check = True
                Get_ZuijiData(2).SiyouGakunen7_Check = True
                Get_ZuijiData(2).SiyouGakunen8_Check = True
                Get_ZuijiData(2).SiyouGakunen9_Check = True
            Case False
                Get_ZuijiData(2).SiyouGakunen1_Check = Chk����2_1�w�N.Checked
                Get_ZuijiData(2).SiyouGakunen2_Check = Chk����2_2�w�N.Checked
                Get_ZuijiData(2).SiyouGakunen3_Check = Chk����2_3�w�N.Checked
                Get_ZuijiData(2).SiyouGakunen4_Check = Chk����2_4�w�N.Checked
                Get_ZuijiData(2).SiyouGakunen5_Check = Chk����2_5�w�N.Checked
                Get_ZuijiData(2).SiyouGakunen6_Check = Chk����2_6�w�N.Checked
                Get_ZuijiData(2).SiyouGakunen7_Check = Chk����2_7�w�N.Checked
                Get_ZuijiData(2).SiyouGakunen8_Check = Chk����2_8�w�N.Checked
                Get_ZuijiData(2).SiyouGakunen9_Check = Chk����2_9�w�N.Checked
        End Select

        Get_ZuijiData(3).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪3)
        Get_ZuijiData(3).Furikae_Nen = txt�Ώ۔N�x.Text.Trim
        Get_ZuijiData(3).Furikae_Tuki = txt�Ώی�.Text
        Get_ZuijiData(3).Furikae_Date = txt�����U�֓�3.Text

        Get_ZuijiData(3).Furikae_Day = Replace(lbl�����U�֓�3.Text.Trim, "/", "")

        Select Case Chk����3_�S�w�N.Checked
            Case True
                Get_ZuijiData(3).SiyouGakunen1_Check = True
                Get_ZuijiData(3).SiyouGakunen2_Check = True
                Get_ZuijiData(3).SiyouGakunen3_Check = True
                Get_ZuijiData(3).SiyouGakunen4_Check = True
                Get_ZuijiData(3).SiyouGakunen5_Check = True
                Get_ZuijiData(3).SiyouGakunen6_Check = True
                Get_ZuijiData(3).SiyouGakunen7_Check = True
                Get_ZuijiData(3).SiyouGakunen8_Check = True
                Get_ZuijiData(3).SiyouGakunen9_Check = True
            Case False
                Get_ZuijiData(3).SiyouGakunen1_Check = Chk����3_1�w�N.Checked
                Get_ZuijiData(3).SiyouGakunen2_Check = Chk����3_2�w�N.Checked
                Get_ZuijiData(3).SiyouGakunen3_Check = Chk����3_3�w�N.Checked
                Get_ZuijiData(3).SiyouGakunen4_Check = Chk����3_4�w�N.Checked
                Get_ZuijiData(3).SiyouGakunen5_Check = Chk����3_5�w�N.Checked
                Get_ZuijiData(3).SiyouGakunen6_Check = Chk����3_6�w�N.Checked
                Get_ZuijiData(3).SiyouGakunen7_Check = Chk����3_7�w�N.Checked
                Get_ZuijiData(3).SiyouGakunen8_Check = Chk����3_8�w�N.Checked
                Get_ZuijiData(3).SiyouGakunen9_Check = Chk����3_9�w�N.Checked
        End Select

        Get_ZuijiData(4).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪4)
        Get_ZuijiData(4).Furikae_Nen = txt�Ώ۔N�x.Text.Trim
        Get_ZuijiData(4).Furikae_Tuki = txt�Ώی�.Text
        Get_ZuijiData(4).Furikae_Date = txt�����U�֓�4.Text

        Get_ZuijiData(4).Furikae_Day = Replace(lbl�����U�֓�4.Text.Trim, "/", "")

        Select Case Chk����4_�S�w�N.Checked
            Case True
                Get_ZuijiData(4).SiyouGakunen1_Check = True
                Get_ZuijiData(4).SiyouGakunen2_Check = True
                Get_ZuijiData(4).SiyouGakunen3_Check = True
                Get_ZuijiData(4).SiyouGakunen4_Check = True
                Get_ZuijiData(4).SiyouGakunen5_Check = True
                Get_ZuijiData(4).SiyouGakunen6_Check = True
                Get_ZuijiData(4).SiyouGakunen7_Check = True
                Get_ZuijiData(4).SiyouGakunen8_Check = True
                Get_ZuijiData(4).SiyouGakunen9_Check = True
            Case False
                Get_ZuijiData(4).SiyouGakunen1_Check = Chk����4_1�w�N.Checked
                Get_ZuijiData(4).SiyouGakunen2_Check = Chk����4_2�w�N.Checked
                Get_ZuijiData(4).SiyouGakunen3_Check = Chk����4_3�w�N.Checked
                Get_ZuijiData(4).SiyouGakunen4_Check = Chk����4_4�w�N.Checked
                Get_ZuijiData(4).SiyouGakunen5_Check = Chk����4_5�w�N.Checked
                Get_ZuijiData(4).SiyouGakunen6_Check = Chk����4_6�w�N.Checked
                Get_ZuijiData(4).SiyouGakunen7_Check = Chk����4_7�w�N.Checked
                Get_ZuijiData(4).SiyouGakunen8_Check = Chk����4_8�w�N.Checked
                Get_ZuijiData(4).SiyouGakunen9_Check = Chk����4_9�w�N.Checked
        End Select

        Get_ZuijiData(5).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪5)
        Get_ZuijiData(5).Furikae_Nen = txt�Ώ۔N�x.Text.Trim
        Get_ZuijiData(5).Furikae_Tuki = txt�Ώی�.Text
        Get_ZuijiData(5).Furikae_Date = txt�����U�֓�5.Text

        Get_ZuijiData(5).Furikae_Day = Replace(lbl�����U�֓�5.Text.Trim, "/", "")

        Select Case Chk����5_�S�w�N.Checked
            Case True
                Get_ZuijiData(5).SiyouGakunen1_Check = True
                Get_ZuijiData(5).SiyouGakunen2_Check = True
                Get_ZuijiData(5).SiyouGakunen3_Check = True
                Get_ZuijiData(5).SiyouGakunen4_Check = True
                Get_ZuijiData(5).SiyouGakunen5_Check = True
                Get_ZuijiData(5).SiyouGakunen6_Check = True
                Get_ZuijiData(5).SiyouGakunen7_Check = True
                Get_ZuijiData(5).SiyouGakunen8_Check = True
                Get_ZuijiData(5).SiyouGakunen9_Check = True
            Case False
                Get_ZuijiData(5).SiyouGakunen1_Check = Chk����5_1�w�N.Checked
                Get_ZuijiData(5).SiyouGakunen2_Check = Chk����5_2�w�N.Checked
                Get_ZuijiData(5).SiyouGakunen3_Check = Chk����5_3�w�N.Checked
                Get_ZuijiData(5).SiyouGakunen4_Check = Chk����5_4�w�N.Checked
                Get_ZuijiData(5).SiyouGakunen5_Check = Chk����5_5�w�N.Checked
                Get_ZuijiData(5).SiyouGakunen6_Check = Chk����5_6�w�N.Checked
                Get_ZuijiData(5).SiyouGakunen7_Check = Chk����5_7�w�N.Checked
                Get_ZuijiData(5).SiyouGakunen8_Check = Chk����5_8�w�N.Checked
                Get_ZuijiData(5).SiyouGakunen9_Check = Chk����5_9�w�N.Checked
        End Select

        Get_ZuijiData(6).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪6)
        Get_ZuijiData(6).Furikae_Nen = txt�Ώ۔N�x.Text.Trim
        Get_ZuijiData(6).Furikae_Tuki = txt�Ώی�.Text
        Get_ZuijiData(6).Furikae_Date = txt�����U�֓�6.Text

        Get_ZuijiData(6).Furikae_Day = Replace(lbl�����U�֓�6.Text.Trim, "/", "")

        Select Case Chk����6_�S�w�N.Checked
            Case True
                Get_ZuijiData(6).SiyouGakunen1_Check = True
                Get_ZuijiData(6).SiyouGakunen2_Check = True
                Get_ZuijiData(6).SiyouGakunen3_Check = True
                Get_ZuijiData(6).SiyouGakunen4_Check = True
                Get_ZuijiData(6).SiyouGakunen5_Check = True
                Get_ZuijiData(6).SiyouGakunen6_Check = True
                Get_ZuijiData(6).SiyouGakunen7_Check = True
                Get_ZuijiData(6).SiyouGakunen8_Check = True
                Get_ZuijiData(6).SiyouGakunen9_Check = True
            Case False
                Get_ZuijiData(6).SiyouGakunen1_Check = Chk����6_1�w�N.Checked
                Get_ZuijiData(6).SiyouGakunen2_Check = Chk����6_2�w�N.Checked
                Get_ZuijiData(6).SiyouGakunen3_Check = Chk����6_3�w�N.Checked
                Get_ZuijiData(6).SiyouGakunen4_Check = Chk����6_4�w�N.Checked
                Get_ZuijiData(6).SiyouGakunen5_Check = Chk����6_5�w�N.Checked
                Get_ZuijiData(6).SiyouGakunen6_Check = Chk����6_6�w�N.Checked
                Get_ZuijiData(6).SiyouGakunen7_Check = Chk����6_7�w�N.Checked
                Get_ZuijiData(6).SiyouGakunen8_Check = Chk����6_8�w�N.Checked
                Get_ZuijiData(6).SiyouGakunen9_Check = Chk����6_9�w�N.Checked
        End Select

    End Sub
    '
    '�@�֐����@-�@fn_Check_TuujyouFuriDate
    '
    '�@�@�\    -  �ʏ�U�֓��^�u���̃`�F�b�N
    '
    '�@����    -  aTuujyouData
    '
    '�@���l    -  
    '�@
    Private Function fn_Check_TuujyouFuriDate(ByVal aInfoGakkou As GAKDATA, _
                                              ByRef aTuujyouData() As TuujyouData, _
                                              Optional ByVal SetDataflg As Boolean = True) As Boolean

        Dim ret As Boolean = False

        Try
            '***���O���***
            STR_COMMAND = "fn_Check_TuujyouFuriDate"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***���O���**

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '�ʏ�U�֓��^�u�`�F�b�N
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            Dim FuriExistFlg As Boolean = False
            Dim SFuriExistFlg As Boolean = False

            For i As Integer = 1 To 5 Step 1
                With aTuujyouData(i)
                    '���ʏ�U�֓����̓`�F�b�N
                    If .Furikae_Check = True Then
                        '�����̓`�F�b�N
                        If .Furikae_Date.Trim = "" Then

                            ERRMSG = "�����͂̐U�֓������݂��܂��B"
                            Me.txt�ʏ�U�֓�1.Focus()
                            Exit Try
                        Else
                            '���t����
                            If IsDate(txt�Ώ۔N�x.Text & "/" & txt�Ώی�.Text & "/" & .Furikae_Date) = False Then

                                ERRMSG = "�s���ȓ��͐U�֓������݂��܂��B"
                                Me.txt�ʏ�U�֓�1.Focus()
                                Exit Try
                            Else
                                '�w�N�t���O�����̓`�F�b�N(���͂��莞�͑Ώۊw�N�̂����ꂩ�Ƀt���O�������OK)
                                If .SiyouGakunen1_Check = False _
                                AndAlso .SiyouGakunen2_Check = False _
                                AndAlso .SiyouGakunen3_Check = False _
                                AndAlso .SiyouGakunen4_Check = False _
                                AndAlso .SiyouGakunen5_Check = False _
                                AndAlso .SiyouGakunen6_Check = False _
                                AndAlso .SiyouGakunen7_Check = False _
                                AndAlso .SiyouGakunen8_Check = False _
                                AndAlso .SiyouGakunen9_Check = False _
                                AndAlso .SiyouGakunenALL_Check = False Then

                                    ERRMSG = "�Ώۊw�N���ݒ肳��Ă��܂���B"
                                    Me.txt�ʏ�U�֓�1.Focus()
                                    Exit Try
                                End If
                            End If
                        End If

                        FuriExistFlg = True
                    Else
                        If .Furikae_Date.Trim <> "" Then

                            ERRMSG = "�U�֓���L���ɂ���ꍇ�́A�U�֓�����͂��Ă��������B"
                            Me.txt�ʏ�U�֓�1.Focus()
                            Exit Try
                        End If
                    End If
                    '���ʏ�ĐU�����̓`�F�b�N
                    If .SaiFurikae_Check = True Then
                        '�����̓`�F�b�N
                        If .SaiFurikae_Date.Trim = "" Then

                            ERRMSG = "�����͂̍ĐU�������݂��܂��B"
                            Me.txt�ʏ�U�֓�1.Focus()
                            Exit Try
                        Else
                            '���t����
                            If IsDate(txt�Ώ۔N�x.Text & "/" & txt�Ώی�.Text & "/" & .SaiFurikae_Date) = False Then

                                ERRMSG = "�s���ȓ��͍ĐU�������݂��܂��B"
                                Me.txt�ʏ�U�֓�1.Focus()
                                Exit Try
                            End If
                        End If

                        SFuriExistFlg = True
                    Else
                        If .SaiFurikae_Date.Trim <> "" Then

                            ERRMSG = "�ĐU����L���ɂ���ꍇ�́A�U�֓�����͂��Ă��������B"
                            Me.txt�ʏ�U�֓�1.Focus()
                            Exit Try
                        End If
                    End If
                End With
            Next

            '�����͒l�̑��݃`�F�b�N
            Select Case True
                Case (FuriExistFlg = False And SFuriExistFlg = False)
                    '�����l�Z�b�g�t���O���莞�̓}�X�^����ݒ�(Insert�p)
                    If SetDataflg = True Then
                        '����ŃX�P�W���[���쐬
                        With aTuujyouData(1)
                            .TaisyouFlg = True
                            .Furikae_Check = True
                            .Furikae_Date = aInfoGakkou.FURI_DATE
                            If aInfoGakkou.SFURI_SYUBETU = 1 Then
                                .SaiFurikae_Check = True
                                .SaiFurikae_Date = aInfoGakkou.SFURI_DATE
                            End If
                            .SiyouGakunen1_Check = False
                            .SiyouGakunen2_Check = False
                            .SiyouGakunen3_Check = False
                            .SiyouGakunen4_Check = False
                            .SiyouGakunen5_Check = False
                            .SiyouGakunen6_Check = False
                            .SiyouGakunen7_Check = False
                            .SiyouGakunen8_Check = False
                            .SiyouGakunen9_Check = False
                            .SiyouGakunenALL_Check = True
                        End With
                    End If

                Case (FuriExistFlg = False And SFuriExistFlg = True)
                    '�ĐU���݂̂̓��͂̓G���[
                    ERRMSG = "�ĐU���݂̂̓��͂͂ł��܂���B"
                    Me.txt�ʏ�U�֓�1.Focus()
                    Exit Try

                Case (FuriExistFlg = True And SFuriExistFlg = True)
                    '���͂���̏ꍇ�̓`�F�b�N

                Case (FuriExistFlg = True And SFuriExistFlg = False)
                    '���͂���̏ꍇ�̓`�F�b�N

            End Select

            '���U�֓��ƍĐU���̓�����`�F�b�N
            For i As Integer = 1 To 5 Step 1
                With aTuujyouData(i)
                    If .Furikae_Check = True AndAlso .SaiFurikae_Check = True AndAlso .Furikae_Date <> "" AndAlso .SaiFurikae_Date <> "" Then
                        If .Furikae_Date = .SaiFurikae_Date Then

                            ERRMSG = "�U�֓����ŐU�֓��ƍĐU�֓����������̂�����܂�"
                            Me.txt�ʏ�U�֓�1.Focus()
                            Exit Try
                        End If
                    End If
                End With
            Next

            '���U�֓����d���`�F�b�N
            For num1 As Integer = 1 To 5 Step 1
                For num2 As Integer = 1 To 5 Step 1
                    If aTuujyouData(num1).Furikae_Date <> "" OrElse aTuujyouData(num2).Furikae_Date <> "" Then
                        If num1 <> num2 AndAlso _
                        aTuujyouData(num1).Furikae_Date = aTuujyouData(num2).Furikae_Date AndAlso _
                        aTuujyouData(num1).Furikae_Check = True AndAlso aTuujyouData(num2).Furikae_Check = True Then

                            ERRMSG = "����U�֓��͐ݒ�ł��܂���B"
                            Me.txt�ʏ�U�֓�1.Focus()
                            Exit Try
                        End If
                    End If
                Next
            Next

            '�c�Ɠ��̕␳
            Dim HoseiEigyoubi1(1) As String
            Dim HoseiEigyoubi2(1) As String
            Dim HoseiEigyoubi3(1) As String
            Dim HoseiEigyoubi4(1) As String
            Dim HoseiEigyoubi5(1) As String
            Dim HoseiEigyoubi6(1) As String
            Dim HoseiEigyoubi7(1) As String
            Dim HoseiEigyoubi8(1) As String
            Dim HoseiEigyoubi9(1) As String
            '1
            If aTuujyouData(1).fn_EigyoubiHosei(aInfoGakkou, HoseiEigyoubi1) Then
                If HoseiEigyoubi1(0) = "err" OrElse HoseiEigyoubi1(1) = "err" Then
                    ERRMSG = "�U�֓��̋x���␳�Ɏ��s���܂����B"
                    Exit Try
                End If
            End If
            '2
            If aTuujyouData(2).fn_EigyoubiHosei(aInfoGakkou, HoseiEigyoubi2) Then
                If HoseiEigyoubi2(0) = "err" OrElse HoseiEigyoubi2(1) = "err" Then
                    ERRMSG = "�U�֓��̋x���␳�Ɏ��s���܂����B"
                    Exit Try
                End If
            End If
            '3
            If aTuujyouData(3).fn_EigyoubiHosei(aInfoGakkou, HoseiEigyoubi3) Then
                If HoseiEigyoubi3(0) = "err" OrElse HoseiEigyoubi3(1) = "err" Then
                    ERRMSG = "�U�֓��̋x���␳�Ɏ��s���܂����B"
                    Exit Try
                End If
            End If
            '4
            If aTuujyouData(4).fn_EigyoubiHosei(aInfoGakkou, HoseiEigyoubi4) Then
                If HoseiEigyoubi4(0) = "err" OrElse HoseiEigyoubi4(1) = "err" Then
                    ERRMSG = "�U�֓��̋x���␳�Ɏ��s���܂����B"
                    Exit Try
                End If
            End If
            '5
            If aTuujyouData(5).fn_EigyoubiHosei(aInfoGakkou, HoseiEigyoubi5) Then
                If HoseiEigyoubi5(0) = "err" OrElse HoseiEigyoubi5(1) = "err" Then
                    ERRMSG = "�U�֓��̋x���␳�Ɏ��s���܂����B"
                    Exit Try
                End If
            End If

            Dim SyofuriDate As String() = {HoseiEigyoubi1(0), _
                                           HoseiEigyoubi2(0), _
                                           HoseiEigyoubi3(0), _
                                           HoseiEigyoubi4(0), _
                                           HoseiEigyoubi5(0), _
                                           HoseiEigyoubi6(0), _
                                           HoseiEigyoubi7(0), _
                                           HoseiEigyoubi8(0), _
                                           HoseiEigyoubi9(0)}

            Dim SaifuriDate As String() = {HoseiEigyoubi1(1), _
                                           HoseiEigyoubi2(1), _
                                           HoseiEigyoubi3(1), _
                                           HoseiEigyoubi4(1), _
                                           HoseiEigyoubi5(1), _
                                           HoseiEigyoubi6(1), _
                                           HoseiEigyoubi7(1), _
                                           HoseiEigyoubi8(1), _
                                           HoseiEigyoubi9(1)}

            For cnt As Integer = 0 To 8 Step 1
                If SyofuriDate(cnt) <> "" AndAlso SyofuriDate(cnt) = SaifuriDate(cnt) Then
                    ERRMSG = "�x���␳��̐U�֓��ƍĐU������v���Ă��܂�"
                    Exit Try
                End If
            Next

            For cnt1 As Integer = 0 To 8 Step 1
                For cnt2 As Integer = 0 To 8 Step 1
                    If cnt1 <> cnt2 Then
                        If SyofuriDate(cnt1) <> "" AndAlso SyofuriDate(cnt1) = SyofuriDate(cnt2) Then
                            ERRMSG = "�x���␳��̐U�֓����d�����Ă��܂�"
                            Exit Try
                        End If
                    End If
                Next
            Next

            For cnt1 As Integer = 0 To 8 Step 1
                For cnt2 As Integer = 0 To 8 Step 1
                    If cnt1 <> cnt2 Then
                        If SaifuriDate(cnt1) <> "" AndAlso SaifuriDate(cnt1) = SaifuriDate(cnt2) Then
                            ERRMSG = "�x���␳��̍ĐU�����d�����Ă��܂�"
                            Exit Try
                        End If
                    End If
                Next
            Next



            '���S�`�F�b�N������ɁA���t���͂�������̂ɁA�����Ώۃt���O�𗧂Ă�
            Dim EntryExistflg As Boolean
            For i As Integer = 1 To 5 Step 1
                If aTuujyouData(i).Furikae_Date.Trim <> "" Then
                    aTuujyouData(i).TaisyouFlg = True
                    EntryExistflg = True
                Else
                    aTuujyouData(i).TaisyouFlg = False
                End If
            Next

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ERRMSG = "���͐U�֓��̃`�F�b�N�Ɏ��s���܂����B"
            Me.txt�ʏ�U�֓�1.Focus()
        End Try

        Return ret

    End Function
    '
    '�@�֐����@-�@fn_Check_ZuijiFuriDate
    '
    '�@�@�\    -  �����U�֓��^�u���̃`�F�b�N
    '
    '�@����    -  aTuujyouData
    '
    '�@���l    -  
    '
    Private Function fn_Check_ZuijiFuriDate(ByVal aInfoGakkou As GAKDATA, ByRef aZuijiData() As ZuijiData) As Boolean

        Dim ret As Boolean = False

        Try
            '***���O���***
            STR_COMMAND = "fn_Check_ZuijiFuriDate"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***���O���**

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '�����U�֓��^�u�`�F�b�N
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            For i As Integer = 1 To 6 Step 1
                With aZuijiData(i)
                    '�������U�֓����̓`�F�b�N

                    '�����̓`�F�b�N
                    If .Furikae_Date.Trim <> "" Then
                        '���t����
                        If IsDate(txt�Ώ۔N�x.Text & "/" & txt�Ώی�.Text & "/" & .Furikae_Date) = False Then

                            ERRMSG = "���������ɕs���ȓ��͐U�֓������݂��܂��B"
                            Me.txt�����U�֓�1.Focus()
                            Exit Try
                        Else
                            '�w�N�t���O�����̓`�F�b�N(���͂��莞�͑Ώۊw�N�̂����ꂩ�Ƀt���O�������OK)
                            If .SiyouGakunen1_Check = False _
                            AndAlso .SiyouGakunen2_Check = False _
                            AndAlso .SiyouGakunen3_Check = False _
                            AndAlso .SiyouGakunen4_Check = False _
                            AndAlso .SiyouGakunen5_Check = False _
                            AndAlso .SiyouGakunen6_Check = False _
                            AndAlso .SiyouGakunen7_Check = False _
                            AndAlso .SiyouGakunen8_Check = False _
                            AndAlso .SiyouGakunen9_Check = False _
                            AndAlso .SiyouGakunenALL_Check = False Then

                                ERRMSG = "���������̑Ώۊw�N���ݒ肳��Ă��܂���B"
                                Me.txt�����U�֓�1.Focus()
                                Exit Try
                            End If
                        End If
                    End If
                End With
            Next

            '���U�֓����d���`�F�b�N
            For num1 As Integer = 1 To 6 Step 1
                For num2 As Integer = 1 To 6 Step 1
                    If aZuijiData(num1).Furikae_Date <> "" OrElse aZuijiData(num2).Furikae_Date <> "" Then
                        If num1 <> num2 AndAlso _
                aZuijiData(num1).Furikae_Date = aZuijiData(num2).Furikae_Date Then

                            ERRMSG = "����U�֓��͐ݒ�ł��܂���B"
                            Me.txt�����U�֓�1.Focus()
                            Exit Try
                        End If
                    End If
                Next
            Next

            Dim StrZuiji(6) As String

            '�����U�̉c�Ɠ����擾
            For i As Integer = 1 To 6 Step 1
                If aZuijiData(i).Furikae_Date.Trim <> "" Then
                    StrZuiji(i) = aZuijiData(i).fn_GetEigyoubiZuiji(aInfoGakkou)
                    If StrZuiji(i) = "err" Then

                        ERRMSG = "�c�Ɠ��̎擾�Ɏ��s���܂����B"
                        Me.txt�����U�֓�1.Focus()
                        Exit Try
                    End If
                End If
            Next

            '���ݒ�U�֓��x���␳��̏d���`�F�b�N
            For i As Integer = 1 To 6 Step 1
                If StrZuiji(i) <> "" Then '�����͂̏ꍇ�A�`�F�b�N�̕K�v�Ȃ�
                    For j As Integer = i + 1 To 6
                        If StrZuiji(i) = StrZuiji(j) Then

                            ERRMSG = "���͐U�֓��ɓ���U�֓��̃f�[�^�����݂��܂�"
                            Me.txt�ʏ�U�֓�1.Focus()
                            Exit Try
                        End If
                    Next
                End If
            Next

            '���S�`�F�b�N������ɁA���t���͂̂��������̂ɁA�����Ώۃt���O�𗧂Ă�
            For i As Integer = 1 To 6 Step 1
                If aZuijiData(i).Furikae_Date.Trim <> "" Then
                    aZuijiData(i).TaisyouFlg = True
                Else
                    aZuijiData(i).TaisyouFlg = False
                End If
            Next

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ERRMSG = "���͐U�֓��̃`�F�b�N�Ɏ��s���܂����B"
            Me.txt�ʏ�U�֓�1.Focus()
        End Try

        Return ret

    End Function
#End Region
#Region "FormInitializa"
    '
    '�@�֐����@-�@FormInitializa
    '
    '�@�@�\    -  �X�P�W���[���}�X�^���݃`�F�b�N 
    '
    '�@����    -  strSCHKBN�AstrFURIKBN�AstrFURIHI�AstrSAIFURIHI
    '
    '�@���l    -  
    '
    '�@
    Private Function FormInitializa(Optional ByVal pIndex As Integer = 0) As Boolean
        Try
            Select Case pIndex
                Case 0 '�S���ڏ�����
                    '�\���̂̏�����
                    Call Sb_StructDataClear()

                    '�w�Z��{��񏉊���
                    Call Sb_Kihon_Clear()

                    '�^�u���̏�����
                    Call Sb_DayClear()
                    Call Sb_ChkGakunenClear()
                    Call Sb_ZuijiCmbClear()

                Case 1 '�w�Z���{���̂ݏ�����
                    '�\���̂̏�����
                    Call Sb_StructDataClear()

                    '�w�Z��{��񏉊���
                    Call Sb_Kihon_Clear()

                Case 2 '�^�u���̂ݏ�����
                    '�^�u���̏�����
                    Call Sb_DayClear()
                    Call Sb_ChkGakunenClear()
                    Call Sb_ZuijiCmbClear()

                Case 9 '�\���̂̂ݏ�����
                    '�\���̂̏�����
                    Call Sb_StructDataClear()
                Case Else
                    Return False
            End Select

            Return True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            Return False
        End Try
    End Function

    '
    '�@�֐����@-�@Sb_Kihon_Crear
    '
    '�@�@�\    -  �X�P�W���[���}�X�^���݃`�F�b�N 
    '
    '�@����    -  strSCHKBN�AstrFURIKBN�AstrFURIHI�AstrSAIFURIHI
    '
    '�@���l    -  
    '
    '�@
    Private Sub Sb_Kihon_Clear()

        txt�Ώ۔N�x.Enabled = True
        txt�Ώی�.Enabled = True

        txtGakkou_Code.Enabled = True
        txtGakkou_Code.Text = ""
        lab�w�Z��.Text = ""
        '�x�����X�g�{�b�N�X������
        lst�x��.Items.Clear()
        '�w�Z�����i�J�i�j
        cmbKana.SelectedIndex = -1

        lbl�g�p�w�N.Text = "0"

        '�ǉ� 2007/02/15
        '�w�Z�R���{�ݒ�i�S�w�Z�j
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
            Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�(cmbGakkouNAME)")
            MessageBox.Show("�w�Z���R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        cmbKana.Enabled = True
        cmbGakkouName.Enabled = True

        '�w�Z�����i�w�Z���j
        cmbGakkouName.SelectedIndex = -1

    End Sub

    '
    '�@�֐����@-�@Sb_DayClear
    '
    '�@�@�\    -  ���͓��t�̏�����
    '
    '�@����    - 
    '
    '�@���l    -  
    '
    '�@
    Private Sub Sb_DayClear()

        '�ʏ�U�֓�
        '1
        Chk�L���U�֓��ʏ�1.Checked = False
        Chk�L���U�֓��ʏ�1.Enabled = True
        txt�ʏ�U�֓�1.Text = ""
        txt�ʏ�U�֓�1.Enabled = True
        lbl�ʏ�U�֓�1.Text = ""
        Chk�L���ĐU���ʏ�1.Checked = False
        Chk�L���ĐU���ʏ�1.Enabled = True
        txt�ʏ�ĐU��1.Text = ""
        txt�ʏ�ĐU��1.Enabled = True
        lbl�ʏ�ĐU��1.Text = ""
        '2
        Chk�L���U�֓��ʏ�2.Checked = False
        Chk�L���U�֓��ʏ�2.Enabled = True
        txt�ʏ�U�֓�2.Text = ""
        txt�ʏ�U�֓�2.Enabled = True
        lbl�ʏ�U�֓�2.Text = ""
        Chk�L���ĐU���ʏ�2.Checked = False
        Chk�L���ĐU���ʏ�2.Enabled = True
        txt�ʏ�ĐU��2.Text = ""
        txt�ʏ�ĐU��2.Enabled = True
        lbl�ʏ�ĐU��2.Text = ""
        '3
        Chk�L���U�֓��ʏ�3.Checked = False
        Chk�L���U�֓��ʏ�3.Enabled = True
        txt�ʏ�U�֓�3.Text = ""
        txt�ʏ�U�֓�3.Enabled = True
        lbl�ʏ�U�֓�3.Text = ""
        Chk�L���ĐU���ʏ�3.Checked = False
        Chk�L���ĐU���ʏ�3.Enabled = True
        txt�ʏ�ĐU��3.Text = ""
        txt�ʏ�ĐU��3.Enabled = True
        lbl�ʏ�ĐU��3.Text = ""
        '4
        Chk�L���U�֓��ʏ�4.Checked = False
        Chk�L���U�֓��ʏ�4.Enabled = True
        txt�ʏ�U�֓�4.Text = ""
        txt�ʏ�U�֓�4.Enabled = True
        lbl�ʏ�U�֓�4.Text = ""
        Chk�L���ĐU���ʏ�4.Checked = False
        Chk�L���ĐU���ʏ�4.Enabled = True
        txt�ʏ�ĐU��4.Text = ""
        txt�ʏ�ĐU��4.Enabled = True
        lbl�ʏ�ĐU��4.Text = ""
        '5
        Chk�L���U�֓��ʏ�5.Checked = False
        Chk�L���U�֓��ʏ�5.Enabled = True
        txt�ʏ�U�֓�5.Text = ""
        txt�ʏ�U�֓�5.Enabled = True
        lbl�ʏ�U�֓�5.Text = ""
        Chk�L���ĐU���ʏ�5.Checked = False
        Chk�L���ĐU���ʏ�5.Enabled = True
        txt�ʏ�ĐU��5.Text = ""
        txt�ʏ�ĐU��5.Enabled = True
        lbl�ʏ�ĐU��5.Text = ""

        '�����U�֓�
        txt�����U�֓�1.Text = ""
        lbl�����U�֓�1.Text = ""
        txt�����U�֓�2.Text = ""
        lbl�����U�֓�2.Text = ""
        txt�����U�֓�3.Text = ""
        lbl�����U�֓�3.Text = ""
        txt�����U�֓�4.Text = ""
        lbl�����U�֓�4.Text = ""
        txt�����U�֓�5.Text = ""
        lbl�����U�֓�5.Text = ""
        txt�����U�֓�6.Text = ""
        lbl�����U�֓�6.Text = ""

        txt�����U�֓�1.Enabled = True
        txt�����U�֓�2.Enabled = True
        txt�����U�֓�3.Enabled = True
        txt�����U�֓�4.Enabled = True
        txt�����U�֓�5.Enabled = True
        txt�����U�֓�6.Enabled = True



    End Sub

    '
    '�@�֐����@-�@Sb_ChkGakunenClear
    '
    '�@�@�\    -  �L���w�N�`�F�b�N�{�b�N�X�̏�����
    '
    '�@����    -  �ʏ�A�������ɏ�����
    '
    '�@���l    -  
    '
    '�@
    Private Sub Sb_ChkGakunenClear()

        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '�ʏ�U�֓�
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '�Ώۊw�N�`�F�b�NBox�̗L����
        Chk�w�N�ʏ�1_1.Enabled = True
        Chk�w�N�ʏ�1_2.Enabled = True
        Chk�w�N�ʏ�1_3.Enabled = True
        Chk�w�N�ʏ�1_4.Enabled = True
        Chk�w�N�ʏ�1_5.Enabled = True
        Chk�w�N�ʏ�1_6.Enabled = True
        Chk�w�N�ʏ�1_7.Enabled = True
        Chk�w�N�ʏ�1_8.Enabled = True
        Chk�w�N�ʏ�1_9.Enabled = True
        Chk�w�N�ʏ�1_�S.Enabled = True

        Chk�w�N�ʏ�2_1.Enabled = True
        Chk�w�N�ʏ�2_2.Enabled = True
        Chk�w�N�ʏ�2_3.Enabled = True
        Chk�w�N�ʏ�2_4.Enabled = True
        Chk�w�N�ʏ�2_5.Enabled = True
        Chk�w�N�ʏ�2_6.Enabled = True
        Chk�w�N�ʏ�2_7.Enabled = True
        Chk�w�N�ʏ�2_8.Enabled = True
        Chk�w�N�ʏ�2_9.Enabled = True
        Chk�w�N�ʏ�2_�S.Enabled = True

        Chk�w�N�ʏ�3_1.Enabled = True
        Chk�w�N�ʏ�3_2.Enabled = True
        Chk�w�N�ʏ�3_3.Enabled = True
        Chk�w�N�ʏ�3_4.Enabled = True
        Chk�w�N�ʏ�3_5.Enabled = True
        Chk�w�N�ʏ�3_6.Enabled = True
        Chk�w�N�ʏ�3_7.Enabled = True
        Chk�w�N�ʏ�3_8.Enabled = True
        Chk�w�N�ʏ�3_9.Enabled = True
        Chk�w�N�ʏ�3_�S.Enabled = True

        Chk�w�N�ʏ�4_1.Enabled = True
        Chk�w�N�ʏ�4_2.Enabled = True
        Chk�w�N�ʏ�4_3.Enabled = True
        Chk�w�N�ʏ�4_4.Enabled = True
        Chk�w�N�ʏ�4_5.Enabled = True
        Chk�w�N�ʏ�4_6.Enabled = True
        Chk�w�N�ʏ�4_7.Enabled = True
        Chk�w�N�ʏ�4_8.Enabled = True
        Chk�w�N�ʏ�4_9.Enabled = True
        Chk�w�N�ʏ�4_�S.Enabled = True

        Chk�w�N�ʏ�5_1.Enabled = True
        Chk�w�N�ʏ�5_2.Enabled = True
        Chk�w�N�ʏ�5_3.Enabled = True
        Chk�w�N�ʏ�5_4.Enabled = True
        Chk�w�N�ʏ�5_5.Enabled = True
        Chk�w�N�ʏ�5_6.Enabled = True
        Chk�w�N�ʏ�5_7.Enabled = True
        Chk�w�N�ʏ�5_8.Enabled = True
        Chk�w�N�ʏ�5_9.Enabled = True
        Chk�w�N�ʏ�5_�S.Enabled = True

        '�Ώۊw�N�L���`�F�b�N
        Chk�w�N�ʏ�1_1.Checked = False
        Chk�w�N�ʏ�1_2.Checked = False
        Chk�w�N�ʏ�1_3.Checked = False
        Chk�w�N�ʏ�1_4.Checked = False
        Chk�w�N�ʏ�1_5.Checked = False
        Chk�w�N�ʏ�1_6.Checked = False
        Chk�w�N�ʏ�1_7.Checked = False
        Chk�w�N�ʏ�1_8.Checked = False
        Chk�w�N�ʏ�1_9.Checked = False
        Chk�w�N�ʏ�1_�S.Checked = False

        Chk�w�N�ʏ�2_1.Checked = False
        Chk�w�N�ʏ�2_2.Checked = False
        Chk�w�N�ʏ�2_3.Checked = False
        Chk�w�N�ʏ�2_4.Checked = False
        Chk�w�N�ʏ�2_5.Checked = False
        Chk�w�N�ʏ�2_6.Checked = False
        Chk�w�N�ʏ�2_7.Checked = False
        Chk�w�N�ʏ�2_8.Checked = False
        Chk�w�N�ʏ�2_9.Checked = False
        Chk�w�N�ʏ�2_�S.Checked = False

        Chk�w�N�ʏ�3_1.Checked = False
        Chk�w�N�ʏ�3_2.Checked = False
        Chk�w�N�ʏ�3_3.Checked = False
        Chk�w�N�ʏ�3_4.Checked = False
        Chk�w�N�ʏ�3_5.Checked = False
        Chk�w�N�ʏ�3_6.Checked = False
        Chk�w�N�ʏ�3_7.Checked = False
        Chk�w�N�ʏ�3_8.Checked = False
        Chk�w�N�ʏ�3_9.Checked = False
        Chk�w�N�ʏ�3_�S.Checked = False

        Chk�w�N�ʏ�4_1.Checked = False
        Chk�w�N�ʏ�4_2.Checked = False
        Chk�w�N�ʏ�4_3.Checked = False
        Chk�w�N�ʏ�4_4.Checked = False
        Chk�w�N�ʏ�4_5.Checked = False
        Chk�w�N�ʏ�4_6.Checked = False
        Chk�w�N�ʏ�4_7.Checked = False
        Chk�w�N�ʏ�4_8.Checked = False
        Chk�w�N�ʏ�4_9.Checked = False
        Chk�w�N�ʏ�4_�S.Checked = False

        Chk�w�N�ʏ�5_1.Checked = False
        Chk�w�N�ʏ�5_2.Checked = False
        Chk�w�N�ʏ�5_3.Checked = False
        Chk�w�N�ʏ�5_4.Checked = False
        Chk�w�N�ʏ�5_5.Checked = False
        Chk�w�N�ʏ�5_6.Checked = False
        Chk�w�N�ʏ�5_7.Checked = False
        Chk�w�N�ʏ�5_8.Checked = False
        Chk�w�N�ʏ�5_9.Checked = False
        Chk�w�N�ʏ�5_�S.Checked = False

        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '�����U�֓�
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '�Ώۊw�N�`�F�b�NBox�̗L����
        Chk����1_1�w�N.Enabled = True
        Chk����1_2�w�N.Enabled = True
        Chk����1_3�w�N.Enabled = True
        Chk����1_4�w�N.Enabled = True
        Chk����1_5�w�N.Enabled = True
        Chk����1_6�w�N.Enabled = True
        Chk����1_7�w�N.Enabled = True
        Chk����1_8�w�N.Enabled = True
        Chk����1_9�w�N.Enabled = True
        Chk����1_�S�w�N.Enabled = True

        Chk����2_1�w�N.Enabled = True
        Chk����2_2�w�N.Enabled = True
        Chk����2_3�w�N.Enabled = True
        Chk����2_4�w�N.Enabled = True
        Chk����2_5�w�N.Enabled = True
        Chk����2_6�w�N.Enabled = True
        Chk����2_7�w�N.Enabled = True
        Chk����2_8�w�N.Enabled = True
        Chk����2_9�w�N.Enabled = True
        Chk����2_�S�w�N.Enabled = True

        Chk����3_1�w�N.Enabled = True
        Chk����3_2�w�N.Enabled = True
        Chk����3_3�w�N.Enabled = True
        Chk����3_4�w�N.Enabled = True
        Chk����3_5�w�N.Enabled = True
        Chk����3_6�w�N.Enabled = True
        Chk����3_7�w�N.Enabled = True
        Chk����3_8�w�N.Enabled = True
        Chk����3_9�w�N.Enabled = True
        Chk����3_�S�w�N.Enabled = True

        Chk����4_1�w�N.Enabled = True
        Chk����4_2�w�N.Enabled = True
        Chk����4_3�w�N.Enabled = True
        Chk����4_4�w�N.Enabled = True
        Chk����4_5�w�N.Enabled = True
        Chk����4_6�w�N.Enabled = True
        Chk����4_7�w�N.Enabled = True
        Chk����4_8�w�N.Enabled = True
        Chk����4_9�w�N.Enabled = True
        Chk����4_�S�w�N.Enabled = True

        Chk����5_1�w�N.Enabled = True
        Chk����5_2�w�N.Enabled = True
        Chk����5_3�w�N.Enabled = True
        Chk����5_4�w�N.Enabled = True
        Chk����5_5�w�N.Enabled = True
        Chk����5_6�w�N.Enabled = True
        Chk����5_7�w�N.Enabled = True
        Chk����5_8�w�N.Enabled = True
        Chk����5_9�w�N.Enabled = True
        Chk����5_�S�w�N.Enabled = True

        Chk����6_1�w�N.Enabled = True
        Chk����6_2�w�N.Enabled = True
        Chk����6_3�w�N.Enabled = True
        Chk����6_4�w�N.Enabled = True
        Chk����6_5�w�N.Enabled = True
        Chk����6_6�w�N.Enabled = True
        Chk����6_7�w�N.Enabled = True
        Chk����6_8�w�N.Enabled = True
        Chk����6_9�w�N.Enabled = True
        Chk����6_�S�w�N.Enabled = True

        '�Ώۊw�N�L���`�F�b�NOFF
        Chk����1_1�w�N.Checked = False
        Chk����1_2�w�N.Checked = False
        Chk����1_3�w�N.Checked = False
        Chk����1_4�w�N.Checked = False
        Chk����1_5�w�N.Checked = False
        Chk����1_6�w�N.Checked = False
        Chk����1_7�w�N.Checked = False
        Chk����1_8�w�N.Checked = False
        Chk����1_9�w�N.Checked = False
        Chk����1_�S�w�N.Checked = False

        Chk����2_1�w�N.Checked = False
        Chk����2_2�w�N.Checked = False
        Chk����2_3�w�N.Checked = False
        Chk����2_4�w�N.Checked = False
        Chk����2_5�w�N.Checked = False
        Chk����2_6�w�N.Checked = False
        Chk����2_7�w�N.Checked = False
        Chk����2_8�w�N.Checked = False
        Chk����2_9�w�N.Checked = False
        Chk����2_�S�w�N.Checked = False

        Chk����3_1�w�N.Checked = False
        Chk����3_2�w�N.Checked = False
        Chk����3_3�w�N.Checked = False
        Chk����3_4�w�N.Checked = False
        Chk����3_5�w�N.Checked = False
        Chk����3_6�w�N.Checked = False
        Chk����3_7�w�N.Checked = False
        Chk����3_8�w�N.Checked = False
        Chk����3_9�w�N.Checked = False
        Chk����3_�S�w�N.Checked = False

        Chk����4_1�w�N.Checked = False
        Chk����4_2�w�N.Checked = False
        Chk����4_3�w�N.Checked = False
        Chk����4_4�w�N.Checked = False
        Chk����4_5�w�N.Checked = False
        Chk����4_6�w�N.Checked = False
        Chk����4_7�w�N.Checked = False
        Chk����4_8�w�N.Checked = False
        Chk����4_9�w�N.Checked = False
        Chk����4_�S�w�N.Checked = False

        Chk����5_1�w�N.Checked = False
        Chk����5_2�w�N.Checked = False
        Chk����5_3�w�N.Checked = False
        Chk����5_4�w�N.Checked = False
        Chk����5_5�w�N.Checked = False
        Chk����5_6�w�N.Checked = False
        Chk����5_7�w�N.Checked = False
        Chk����5_8�w�N.Checked = False
        Chk����5_9�w�N.Checked = False
        Chk����5_�S�w�N.Checked = False

        Chk����6_1�w�N.Checked = False
        Chk����6_2�w�N.Checked = False
        Chk����6_3�w�N.Checked = False
        Chk����6_4�w�N.Checked = False
        Chk����6_5�w�N.Checked = False
        Chk����6_6�w�N.Checked = False
        Chk����6_7�w�N.Checked = False
        Chk����6_8�w�N.Checked = False
        Chk����6_9�w�N.Checked = False
        Chk����6_�S�w�N.Checked = False

    End Sub

    '
    '�@�֐����@-�@Sb_Zuiji_Cmb
    '
    '�@�@�\    -  �����U�֓����o���敪������ 
    '
    '�@����    -  �������ɏ�����
    '
    '�@���l    -  
    '
    '�@
    Private Sub Sb_ZuijiCmbClear()

        Try
            '***���O���***
            STR_COMMAND = "Sb_ZuijiCmbClear"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***���O���**

            '�e�L�X�g�t�@�C������R���{�{�b�N�X�ݒ�
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪1) = False Then
                Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�(cmb���o�敪1)")
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪2) = False Then
                Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�(cmb���o�敪2)")
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪3) = False Then
                Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�(cmb���o�敪3)")
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪4) = False Then
                Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�(cmb���o�敪4)")
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪5) = False Then
                Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�(cmb���o�敪5)")
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪6) = False Then
                Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�(cmb���o�敪6)")
                Exit Sub
            End If

            cmb���o�敪1.SelectedIndex = 0
            cmb���o�敪2.SelectedIndex = 0
            cmb���o�敪3.SelectedIndex = 0
            cmb���o�敪4.SelectedIndex = 0
            cmb���o�敪5.SelectedIndex = 0
            cmb���o�敪6.SelectedIndex = 0

            cmb���o�敪1.Enabled = True
            cmb���o�敪2.Enabled = True
            cmb���o�敪3.Enabled = True
            cmb���o�敪4.Enabled = True
            cmb���o�敪5.Enabled = True
            cmb���o�敪6.Enabled = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        End Try

    End Sub
    '
    '�@�֐����@-�@Sb_ClearStructData
    '
    '�@�@�\    -  ��ʃf�[�^�ێ��\���̂̏�����
    '
    '�@����    -  
    '
    '�@���l    -  �ʏ�A�������ɏ�����
    '
    '�@
    Private Sub Sb_StructDataClear()

        '�ʏ�U�֓�
        For No As Integer = 1 To 5 Step 1
            '�Ώۃt���O
            Tuujyou_SchInfo(No).TaisyouFlg = False
            '�e�t���O
            Tuujyou_SchInfo(No).SyoriFurikae_Flag = False
            Tuujyou_SchInfo(No).CheckFurikae_Flag = False
            Tuujyou_SchInfo(No).FunouFurikae_Flag = False
            Tuujyou_SchInfo(No).SyoriSaiFurikae_Flag = False
            Tuujyou_SchInfo(No).CheckSaiFurikae_Flag = False
            '�����N���x
            Tuujyou_SchInfo(No).Seikyu_Nen = ""
            Tuujyou_SchInfo(No).Seikyu_Tuki = ""
            '�U�֓�
            Tuujyou_SchInfo(No).Furikae_Tuki = ""
            Tuujyou_SchInfo(No).Furikae_Date = ""
            Tuujyou_SchInfo(No).Furikae_Check = False
            Tuujyou_SchInfo(No).Furikae_Enabled = False
            '�ĐU��
            Tuujyou_SchInfo(No).SaiFurikae_Tuki = ""
            Tuujyou_SchInfo(No).SaiFurikae_Date = ""
            Tuujyou_SchInfo(No).SaiFurikae_Check = False
            Tuujyou_SchInfo(No).SaiFurikae_Enabled = False
            '�w�N�t���O
            Tuujyou_SchInfo(No).SiyouGakunen1_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen2_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen3_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen4_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen5_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen6_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen7_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen8_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen9_Check = False
            '��ʕ\���U�ցA�ĐU��
            Tuujyou_SchInfo(No).Furikae_Day = ""
            Tuujyou_SchInfo(No).SaiFurikae_Day = ""

            '�Ώۃt���O
            Syoki_Tuujyou_SchInfo(No).TaisyouFlg = False
            '�e�t���O
            Syoki_Tuujyou_SchInfo(No).SyoriFurikae_Flag = False
            Syoki_Tuujyou_SchInfo(No).CheckFurikae_Flag = False
            Syoki_Tuujyou_SchInfo(No).FunouFurikae_Flag = False
            Syoki_Tuujyou_SchInfo(No).SyoriSaiFurikae_Flag = False
            Syoki_Tuujyou_SchInfo(No).CheckSaiFurikae_Flag = False
            '�����N���x
            Syoki_Tuujyou_SchInfo(No).Seikyu_Nen = ""
            Syoki_Tuujyou_SchInfo(No).Seikyu_Tuki = ""
            '�U�֓�
            Syoki_Tuujyou_SchInfo(No).Furikae_Tuki = ""
            Syoki_Tuujyou_SchInfo(No).Furikae_Date = ""
            Syoki_Tuujyou_SchInfo(No).Furikae_Check = False
            Syoki_Tuujyou_SchInfo(No).Furikae_Enabled = False
            '�ĐU��
            Syoki_Tuujyou_SchInfo(No).SaiFurikae_Tuki = ""
            Syoki_Tuujyou_SchInfo(No).SaiFurikae_Date = ""
            Syoki_Tuujyou_SchInfo(No).SaiFurikae_Check = False
            Syoki_Tuujyou_SchInfo(No).SaiFurikae_Enabled = False
            '�w�N�t���O
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen1_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen2_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen3_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen4_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen5_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen6_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen7_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen8_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen9_Check = False
            '��ʕ\���U�ցA�ĐU��
            Syoki_Tuujyou_SchInfo(No).Furikae_Day = ""
            Syoki_Tuujyou_SchInfo(No).SaiFurikae_Day = ""
        Next

        '�����U�֓�
        For No As Integer = 1 To 6 Step 1
            Zuiji_SchInfo(No).TaisyouFlg = False
            Zuiji_SchInfo(No).Furikae_Nen = ""
            Zuiji_SchInfo(No).Furikae_Tuki = ""
            Zuiji_SchInfo(No).Furikae_Date = ""
            Zuiji_SchInfo(No).Nyusyutu_Kbn = "2"
            Zuiji_SchInfo(No).SiyouGakunen1_Check = False
            Zuiji_SchInfo(No).SiyouGakunen2_Check = False
            Zuiji_SchInfo(No).SiyouGakunen3_Check = False
            Zuiji_SchInfo(No).SiyouGakunen4_Check = False
            Zuiji_SchInfo(No).SiyouGakunen5_Check = False
            Zuiji_SchInfo(No).SiyouGakunen6_Check = False
            Zuiji_SchInfo(No).SiyouGakunen7_Check = False
            Zuiji_SchInfo(No).SiyouGakunen8_Check = False
            Zuiji_SchInfo(No).SiyouGakunen9_Check = False
            Zuiji_SchInfo(No).Syori_Flag = False

            Syoki_Zuiji_SchInfo(No).TaisyouFlg = False
            Syoki_Zuiji_SchInfo(No).Furikae_Nen = ""
            Syoki_Zuiji_SchInfo(No).Furikae_Tuki = ""
            Syoki_Zuiji_SchInfo(No).Furikae_Date = ""
            Syoki_Zuiji_SchInfo(No).Nyusyutu_Kbn = "2"
            Syoki_Zuiji_SchInfo(No).SiyouGakunen1_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen2_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen3_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen4_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen5_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen6_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen7_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen8_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen9_Check = False
            Syoki_Zuiji_SchInfo(No).Syori_Flag = False
        Next

    End Sub

#End Region

#Region "INSERTSCHMAST"
    '
    '�@�֐����@-�@fn_INSERTSCHMAST
    '
    '�@�@�\    -  �X�P�W���[���쐬
    '
    '�@����    -  TORIS_CODE , TORIF_CODE,FURI_DATE,TIME_STAMP,PG_KUBUN 1:�� �@2:�ꊇ
    '
    '�@���l    -  �ʏ�A�������ɏ�����
    '
    '�@
    Private Function fn_INSERTSCHMAST(ByVal aTORIS_CODE As String, ByVal aTORIF_CODE As String, ByVal aFURI_DATE As String) As Integer
        '----------------------------------------------------------------------------
        'Name       :fn_insert_SCHMAST
        'Description:�X�P�W���[���쐬
        'Parameta   :TORIS_CODE , TORIF_CODE,FURI_DATE,TIME_STAMP,PG_KUBUN 1:�� �@2:�ꊇ
        'Create     :2004/08/02
        'UPDATE     :2007/12/26
        '           :***�C���@�ɶ��ϲ�� (��Ǝ��U���ޭ��Ͻ��������Ɋ�Ƒ����ޭ��Ͻ��̍��ڒǉ��ׁ̈j
        '----------------------------------------------------------------------------

        Dim RetCode As Integer = gintKEKKA.NG

        Try
            Dim SQL As StringBuilder
            Dim SCH_DATA(77) As String

            Dim strFURI_DATE As String
            Dim strSOU_KBN As String
            Dim strKYU_KBN As String
            Dim intSFURI_KBN As Integer
            Dim strTESUU_KIJITSU As String
            Dim strKESSAI_KYUCD As Integer '���ϋx���R�[�h
            Dim strTESUU_CNTDATE As Integer '����/���

            Dim Ret As String

            Dim CLS As New MAIN.ClsSchduleMaintenanceClass
            '2012/09/04 saitou �x������ MODIFY -------------------------------------------------->>>>
            CLS.SetSchTable = MAIN.ClsSchduleMaintenanceClass.APL.JifuriApplication
            'CLS.SetSchTable = CLS.APL.JifuriApplication
            '2012/09/04 saitou �x������ MODIFY --------------------------------------------------<<<<

            strFURI_DATE = aFURI_DATE.Substring(0, 4) & "/" & aFURI_DATE.Substring(4, 2) & "/" & aFURI_DATE.Substring(6, 2)

            '----------------
            '�����}�X�^����
            '----------------
            SQL = New StringBuilder(128)

            SQL.Append(" SELECT * FROM TORIMAST ")
            SQL.Append(" WHERE TORIS_CODE_T = '" & aTORIS_CODE.Trim & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & aTORIF_CODE.Trim & "'")

            Dim OraCommand As New OracleClient.OracleCommand
            Dim OraReader As OracleClient.OracleDataReader
            OraCommand.CommandText = SQL.ToString
            OraCommand.Connection = OBJ_CONNECTION
            OraCommand.Transaction = OBJ_TRANSACTION

            OraReader = OraCommand.ExecuteReader   '�Ǎ��̂�

            '�Ǎ��̂�
            If OraReader.Read = False Then
                MessageBox.Show("�����}�X�^�ɍĐU����悪�o�^����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                RetCode = gintKEKKA.NG
                Exit Try
            End If

            strSOU_KBN = ConvNullToString(OraReader.Item("SOUSIN_KBN_T"))
            strKYU_KBN = GCom.NzDec(OraReader.Item("FURI_KYU_CODE_T"), 0).ToString
            strTESUU_KIJITSU = GCom.NzDec(OraReader.Item("TESUUTYO_KIJITSU_T"), 0).ToString

            strTESUU_CNTDATE = OraReader.Item("TESUUTYO_DAY_T")
            strKESSAI_KYUCD = GCom.NzDec(OraReader.Item("TESUU_KYU_CODE_T"), 0).ToString
            intSFURI_KBN = OraReader.Item("SFURI_FLG_T")


            '-------------------------------------
            '�U�֓��͉c�Ɠ��̉c�Ɠ�����i�y�E���E�j�Փ�����j
            '-------------------------------------
            '�X�P�W���[���쐬�Ώۂ̎����R�[�h�𒊏o
            CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(aFURI_DATE), aTORIS_CODE, aTORIF_CODE)

            CLS.SCH.FURI_DATE = GCom.SET_DATE(aFURI_DATE)
            If CLS.SCH.FURI_DATE = "00000000" Then
            Else
                CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
            End If

            strFURI_DATE = CLS.SCH.FURI_DATE

            Ret = CLS.INSERT_NEW_SCHMAST(0, False, True)

            '------------------
            '�}�X�^�o�^���ڐݒ�
            '------------------
            SCH_DATA(0) = OraReader.Item("FSYORI_KBN_T")                                       '�U�֏����敪
            SCH_DATA(1) = aTORIS_CODE                                                           '������R�[�h
            SCH_DATA(2) = aTORIF_CODE                                                           '����敛�R�[�h
            SCH_DATA(3) = CLS.SCH.FURI_DATE 'strIN_NEN & strIN_TUKI & strIN_HI 'FURI_DATE_S�@ �@'�U�֓�
            SCH_DATA(4) = CLS.SCH.FURI_DATE '"00000000" 'SAIFURI_DATE_S                         '�_��U�֓�=�U�֓�
            SCH_DATA(5) = "00000000"                                                            '�ĐU��
            SCH_DATA(6) = CLS.SCH.KSAIFURI_DATE                                                 '�ĐU�\���
            SCH_DATA(7) = CStr(ConvNullToString(OraReader.Item("FURI_CODE_T"))).PadLeft(3, "0")  '�U�փR�[�h�r
            SCH_DATA(8) = CStr(ConvNullToString(OraReader.Item("KIGYO_CODE_T"))).PadLeft(5, "0") '��ƃR�[�h�r
            SCH_DATA(9) = CLS.TR(0).ITAKU_CODE '�ϑ��҃R�[�h
            SCH_DATA(10) = CStr(OraReader.Item("TKIN_NO_T")).PadLeft(4, "0")
            SCH_DATA(11) = CStr(OraReader.Item("TSIT_NO_T")).PadLeft(3, "0")
            SCH_DATA(12) = OraReader.Item("SOUSIN_KBN_T")
            SCH_DATA(13) = OraReader.Item("MOTIKOMI_KBN_T")
            SCH_DATA(14) = OraReader.Item("BAITAI_CODE_T") 'BAITAI_CODE_S
            SCH_DATA(15) = 0 'MOTIKOMI_SEQ_S
            SCH_DATA(16) = 0 'FILE_SEQ_S
            '�萔���v�Z�敪�̎Z�o
            Dim strTUKI_KBN As String = ""
            Select Case aFURI_DATE.Substring(4, 2)
                Case "01"
                    strTUKI_KBN = OraReader.Item("TUKI1_T")
                Case "02"
                    strTUKI_KBN = OraReader.Item("TUKI2_T")
                Case "03"
                    strTUKI_KBN = OraReader.Item("TUKI3_T")
                Case "04"
                    strTUKI_KBN = OraReader.Item("TUKI4_T")
                Case "05"
                    strTUKI_KBN = OraReader.Item("TUKI5_T")
                Case "06"
                    strTUKI_KBN = OraReader.Item("TUKI6_T")
                Case "07"
                    strTUKI_KBN = OraReader.Item("TUKI7_T")
                Case "08"
                    strTUKI_KBN = OraReader.Item("TUKI8_T")
                Case "09"
                    strTUKI_KBN = OraReader.Item("TUKI9_T")
                Case "10"
                    strTUKI_KBN = OraReader.Item("TUKI10_T")
                Case "11"
                    strTUKI_KBN = OraReader.Item("TUKI11_T")
                Case "12"
                    strTUKI_KBN = OraReader.Item("TUKI12_T")
            End Select

            Select Case OraReader.Item("TESUUTYO_KBN_T")
                Case 0
                    SCH_DATA(17) = "1"          'TESUU_KBN_S
                Case 1
                    Select Case strTUKI_KBN
                        Case "1", "3"
                            SCH_DATA(17) = "2"
                        Case Else
                            SCH_DATA(17) = "3"
                    End Select
                Case 2
                    SCH_DATA(17) = "0"
                Case Else
                    SCH_DATA(17) = "0"
            End Select

            SCH_DATA(18) = "00000000"              '�˗����쐬��
            SCH_DATA(19) = CLS.SCH.IRAISYOK_YDATE  '�˗�������\���
            SCH_DATA(20) = CLS.SCH.MOTIKOMI_DATE   'MOTIKOMI_DATE_S
            SCH_DATA(21) = "00000000"              'UKETUKE_DATE_S   
            SCH_DATA(22) = "00000000"              'TOUROKU_DATE_S
            SCH_DATA(23) = CLS.SCH.HAISIN_YDATE    'HAISIN_YDATE_S
            SCH_DATA(24) = "00000000"              'HAISIN_DATE_S
            SCH_DATA(25) = CLS.SCH.HAISIN_YDATE    'SOUSIN_YDATE_S
            SCH_DATA(26) = "00000000"              'SOUSIN_DATE_S
            SCH_DATA(27) = CLS.SCH.FUNOU_YDATE     'FUNOU_YDATE_S
            SCH_DATA(28) = "00000000"              'FUNOU_DATE_S
            SCH_DATA(29) = CLS.SCH.KESSAI_YDATE    'KESSAI_YDATE_S
            SCH_DATA(30) = "00000000"              'KESSAI_DATE_S
            SCH_DATA(31) = CLS.SCH.TESUU_YDATE     'TESUU_YDATE_S
            SCH_DATA(32) = "00000000"              'TESUU_DATE_S
            SCH_DATA(33) = CLS.SCH.HENKAN_YDATE    'HENKAN_YDATE_S
            SCH_DATA(34) = "00000000"              'HENKAN_DATE_S
            SCH_DATA(35) = "00000000"              'UKETORI_DATE_S
            SCH_DATA(36) = "0"                     'UKETUKE_FLG_S
            SCH_DATA(37) = "0"                     'TOUROKU_FLG_S
            SCH_DATA(38) = "0"                     'HAISIN_FLG_S
            SCH_DATA(39) = "0"                     'SAIFURI_FLG_S
            SCH_DATA(40) = "0"                     'SOUSIN_FLG_S
            SCH_DATA(41) = "0"                     'FUNOU_FLG_S
            SCH_DATA(42) = "0"                     'TESUUKEI_FLG_S
            SCH_DATA(43) = "0"                     'TESUUTYO_FLG_S
            SCH_DATA(44) = "0"                     'KESSAI_FLG_S
            SCH_DATA(45) = "0"                     'HENKAN_FLG_S
            SCH_DATA(46) = "0"                     'TYUUDAN_FLG_S
            SCH_DATA(47) = "0"                     'TAKOU_FLG_S
            SCH_DATA(48) = "0"                     'NIPPO_FLG_S
            SCH_DATA(49) = Space(3)                'ERROR_INF_S
            SCH_DATA(50) = 0                       'SYORI_KEN_S
            SCH_DATA(51) = 0                       'SYORI_KIN_S
            SCH_DATA(52) = 0                       'ERR_KEN_S
            SCH_DATA(53) = 0                       'ERR_KIN_S
            SCH_DATA(54) = 0                       'TESUU_KIN_S
            SCH_DATA(55) = 0                       'TESUU_KIN1_S
            SCH_DATA(56) = 0                       'TESUU_KIN2_S
            SCH_DATA(57) = 0                       'TESUU_KIN3_S
            SCH_DATA(58) = 0                       'FURI_KEN_S
            SCH_DATA(59) = 0                       'FURI_KIN_S
            SCH_DATA(60) = 0                       'FUNOU_KEN_S
            SCH_DATA(61) = 0                       'FUNOU_KIN_S
            SCH_DATA(62) = Space(50)               'UFILE_NAME_S
            SCH_DATA(63) = Space(50)               'SFILE_NAME_S
            SCH_DATA(64) = Format(Now, "yyyyMMdd") 'SAKUSEI_DATE_S
            SCH_DATA(65) = Space(14)               'JIFURI_TIME_STAMP_S
            SCH_DATA(66) = Space(14)               'KESSAI_TIME_STAMP_S
            SCH_DATA(67) = Space(14)               'TESUU_TIME_STAMP_S
            SCH_DATA(68) = Space(15)               'YOBI1_S
            SCH_DATA(69) = Space(15)               'YOBI2_S
            SCH_DATA(70) = Space(15)               'YOBI3_S
            SCH_DATA(71) = Space(15)               'YOBI4_S
            SCH_DATA(72) = Space(15)               'YOBI5_S
            SCH_DATA(73) = Space(15)               'YOBI6_S
            SCH_DATA(74) = Space(15)               'YOBI7_S
            SCH_DATA(75) = Space(15)               'YOBI8_S
            SCH_DATA(76) = Space(15)               'YOBI9_S
            SCH_DATA(77) = Space(15)               'YOBI10_S

            '----------------------
            '�X�P�W���[���}�X�^�o�^
            '----------------------
            SQL = New StringBuilder(1024)

            SQL.Append(" INSERT INTO SCHMAST ( ")
            SQL.Append(" FSYORI_KBN_S")     '0
            SQL.Append(",TORIS_CODE_S")     '1
            SQL.Append(",TORIF_CODE_S")     '2
            SQL.Append(",FURI_DATE_S")      '3
            SQL.Append(",KFURI_DATE_S")     '4
            SQL.Append(",SAIFURI_DATE_S")   '5
            SQL.Append(",KSAIFURI_DATE_S")  '6
            SQL.Append(",FURI_CODE_S")      '7
            SQL.Append(",KIGYO_CODE_S")     '8
            SQL.Append(",ITAKU_CODE_S")     '9
            SQL.Append(",TKIN_NO_S")        '10
            SQL.Append(",TSIT_NO_S")        '11
            SQL.Append(",SOUSIN_KBN_S")     '12
            SQL.Append(",MOTIKOMI_KBN_S")   '13
            SQL.Append(",BAITAI_CODE_S")    '14
            SQL.Append(",MOTIKOMI_SEQ_S")   '15
            SQL.Append(",FILE_SEQ_S")       '16
            SQL.Append(",TESUU_KBN_S")      '17
            SQL.Append(",IRAISYO_DATE_S")   '18
            SQL.Append(",IRAISYOK_YDATE_S") '19
            SQL.Append(",MOTIKOMI_DATE_S")  '20
            SQL.Append(",UKETUKE_DATE_S")   '21
            SQL.Append(",TOUROKU_DATE_S")   '22
            SQL.Append(",HAISIN_YDATE_S")   '23
            SQL.Append(",HAISIN_DATE_S")    '24
            SQL.Append(",SOUSIN_YDATE_S")   '25
            SQL.Append(",SOUSIN_DATE_S")    '26
            SQL.Append(",FUNOU_YDATE_S")    '27
            SQL.Append(",FUNOU_DATE_S")     '28
            SQL.Append(",KESSAI_YDATE_S")   '29
            SQL.Append(",KESSAI_DATE_S")    '30
            SQL.Append(",TESUU_YDATE_S")    '31
            SQL.Append(",TESUU_DATE_S")     '32
            SQL.Append(",HENKAN_YDATE_S")   '33
            SQL.Append(",HENKAN_DATE_S")    '34
            SQL.Append(",UKETORI_DATE_S")   '35
            SQL.Append(",UKETUKE_FLG_S")    '36
            SQL.Append(",TOUROKU_FLG_S")    '37
            SQL.Append(",HAISIN_FLG_S")     '38
            SQL.Append(",SAIFURI_FLG_S")    '39
            SQL.Append(",SOUSIN_FLG_S")     '40
            SQL.Append(",FUNOU_FLG_S")      '41
            SQL.Append(",TESUUKEI_FLG_S")   '42
            SQL.Append(",TESUUTYO_FLG_S")   '43
            SQL.Append(",KESSAI_FLG_S")     '44
            SQL.Append(",HENKAN_FLG_S")     '45
            SQL.Append(",TYUUDAN_FLG_S")    '46
            SQL.Append(",TAKOU_FLG_S")      '47
            SQL.Append(",NIPPO_FLG_S")      '48
            SQL.Append(",ERROR_INF_S")      '49
            SQL.Append(",SYORI_KEN_S")      '50
            SQL.Append(",SYORI_KIN_S")      '51
            SQL.Append(",ERR_KEN_S")        '52
            SQL.Append(",ERR_KIN_S")        '53
            SQL.Append(",TESUU_KIN_S")      '54
            SQL.Append(",TESUU_KIN1_S")     '55
            SQL.Append(",TESUU_KIN2_S")     '56
            SQL.Append(",TESUU_KIN3_S")     '57
            SQL.Append(",FURI_KEN_S")       '58
            SQL.Append(",FURI_KIN_S")       '59
            SQL.Append(",FUNOU_KEN_S")      '60
            SQL.Append(",FUNOU_KIN_S")      '61
            SQL.Append(",UFILE_NAME_S")     '62
            SQL.Append(",SFILE_NAME_S")     '63
            SQL.Append(",SAKUSEI_DATE_S")   '64
            SQL.Append(",JIFURI_TIME_STAMP_S")      '65
            SQL.Append(",KESSAI_TIME_STAMP_S")      '66
            SQL.Append(",TESUU_TIME_STAMP_S")       '67
            SQL.Append(",YOBI1_S")          '68
            SQL.Append(",YOBI2_S")          '69
            SQL.Append(",YOBI3_S")          '70
            SQL.Append(",YOBI4_S")          '71
            SQL.Append(",YOBI5_S")          '72
            SQL.Append(",YOBI6_S")          '73
            SQL.Append(",YOBI7_S")          '74
            SQL.Append(",YOBI8_S")          '75
            SQL.Append(",YOBI9_S")          '76
            SQL.Append(",YOBI10_S")         '77
            SQL.Append(" ) VALUES ( ")
            For cnt As Integer = LBound(SCH_DATA) To UBound(SCH_DATA)
                SQL.Append("'" & SCH_DATA(cnt) & "',")
            Next

            Dim InsertSchmastSQL As String = SQL.ToString

            InsertSchmastSQL = InsertSchmastSQL.Substring(0, SQL.Length - 1) & ")"

            If GFUNC_EXECUTESQL_TRANS(InsertSchmastSQL, 1) = False Then
                Call GSUB_LOG(0, "���U�X�P�W���[��Insert���s�ASQL:" & InsertSchmastSQL)
                ERRMSG = "���U�X�P�W���[���쐬�Ɏ��s���܂���"
                Exit Try
            End If

            RetCode = gintKEKKA.OK

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ERRMSG = "���U�X�P�W���[���쐬�Ɏ��s���܂���"
            RetCode = gintKEKKA.NG
        End Try

        Return RetCode

    End Function
#End Region
    '
    '�@�֐����@-�@fn_Hosei
    '
    '�@�@�\    -  �␳
    '
    '�@����    -  �␳
    '
    '�@���l    -  �␳
    '
    '�@
    Private Function fn_Hosei(ByVal aDate As String) As String

        Try
            If aDate.Length <> 8 Then
                Return aDate
            End If

            If IsDate(aDate.Substring(0, 4) & "/" & aDate.Substring(4, 2) & "/" & aDate.Substring(6, 2)) = True Then
                Return aDate
            Else
                Dim MaxDay As Integer = Date.DaysInMonth(CInt(aDate.Substring(0, 4)), CInt(aDate.Substring(4, 2)))
                Return aDate.Substring(0, 4) & aDate.Substring(4, 2) & Format(MaxDay, "00")
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            Return aDate
        End Try

    End Function
    '
    '�@�֐����@-�@fn_ToriMastIsExist
    '
    '�@�@�\    -  �����}�X�^���݃`�F�b�N
    '
    '�@����    -  TORIS_CODE,TORIF_CODE,CONNECTION
    '
    '�@���l    -  �ʏ�A�������ɏ�����
    '
    '�@
    Private Function fn_ToriMastIsExist(ByVal TorisCode As String, _
                                        ByVal TorifCode As String, _
                                        ByVal OraConn As OracleClient.OracleConnection, _
                                        ByVal OraTran As OracleClient.OracleTransaction) As Boolean

        Dim SQL As String = ""

        SQL = " SELECT * "
        SQL &= " FROM TORIMAST "
        SQL &= " WHERE TORIS_CODE_T = '" & TorisCode & "'"
        SQL &= " AND TORIF_CODE_T = '" & TorifCode & "'"

        Return fn_IsExist(SQL, OraConn, OraTran)

    End Function
    '
    '�@�֐����@-�@fn_SchMastIsExist
    '
    '�@�@�\    -  �X�P�W���[���}�X�^���݃`�F�b�N
    '
    '�@����    -  TORIS_CODE,TORIF_CODE,FURI_DATE,CONNECTION
    '
    '�@���l    -  �ʏ�A�������ɏ�����
    '
    '�@
    Private Function fn_SchMastIsExist(ByVal TorisCode As String, _
                                       ByVal TorifCode As String, _
                                       ByVal FuriDate As String, _
                                       ByVal OraConn As OracleClient.OracleConnection, _
                                       ByVal OraTran As OracleClient.OracleTransaction) As Boolean

        Dim SQL As String = ""

        SQL = " SELECT * "
        SQL &= " FROM SCHMAST "
        SQL &= " WHERE TORIS_CODE_S = '" & TorisCode & "'"
        SQL &= " AND TORIF_CODE_S = '" & TorifCode & "'"
        SQL &= " AND FURI_DATE_S = '" & FuriDate & "'"

        Return fn_IsExist(SQL, OraConn, OraTran)

    End Function
    '
    '�@�֐����@-�@fn_IsExist
    '
    '�@�@�\    -  �X�P�W���[���쐬
    '
    '�@����    -  SQL,CONNECTION
    '
    '�@���l    -  �ʏ�A�������ɏ�����
    '
    '�@
    Private Function fn_IsExist(ByVal SQL As String, _
                                ByVal OraConn As OracleClient.OracleConnection, _
                                ByVal OraTran As OracleClient.OracleTransaction) As Boolean

        Dim ret As Boolean = False
        Dim ConnFlg As Boolean = True
        Dim OraReader As OracleClient.OracleDataReader = Nothing

        Try
            Dim OraCommand As New OracleClient.OracleCommand
            OraCommand.CommandText = SQL
            OraCommand.Connection = OraConn
            OraCommand.Transaction = OraTran
            OraReader = OraCommand.ExecuteReader

            If OraReader.Read = False Then
                ret = False
            Else
                ret = True
            End If

            OraReader.Close()
            OraReader = Nothing

        Catch ex As Exception
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
            ret = False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub Label3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label3.Click

    End Sub
End Class
