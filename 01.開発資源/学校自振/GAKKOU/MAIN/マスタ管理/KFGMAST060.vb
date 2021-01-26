Option Explicit On 
Option Strict Off

Imports System.Text
Imports CASTCommon

Public Class KFGMAST060

    Private Enum gintPG_KBN As Integer
        KOBETU = 1
        IKKATU = 2
    End Enum
    Private Enum gintKEKKA As Integer
        OK = 0
        NG = 1
        OTHER = 2
    End Enum

    Private gstrTORIS_CODE As String
    Private gstrFURI_DATE As String

    Private gastrTORIS_CODE_T As String
    Private gastrTORIF_CODE_T As String
    Private gastrITAKU_KNAME_T As String
    Private gastrITAKU_NNAME_T As String
    Private gastrFILE_NAME_T As String
    Private gastrKIGYO_CODE_T As String
    Private gastrFURI_CODE_T As String
    Private gastrBAITAI_CODE_T As String
    Private gastrFMT_KBN_T As String
    Private gastrTAKOU_KBN_T As String
    Private gastrITAKU_CODE_T As String
    Private gastrNS_KBN_T As String
    Private gastrLABEL_KBN As String
    Private gastrITAKU_KIN As String
    Private gastrITAKU_SIT As String
    Private gastrITAKU_KAMOKU As String
    Private gastrITAKU_KOUZA As String
    Private gastrTEKIYO_KBN As String
    Private gastrKTEKIYO As String
    Private gastrNTEKIYO As String
    Private gastrMULTI_KBN As String
    Private gastrNS_KBN As String
    Private gastrCODE_KBN_T As String

    'SCHMAST�p�f�[�^�Z�b�g
    Private gstrKYUJITU As String
    Private gstrWORK_DATE As String
    Private gSCH_DATA(71) As String


#Region " ���ʕϐ��錾 "

    Private STR�����N�� As String
    Private STR�U�֓� As String
    Private STR�ĐU�֓� As String
    '2010/10/21 �_��U�֓��ǉ�
    Private STR�_��U�֓� As String

    Private STR�X�P�敪 As String
    Private STR�U�֋敪 As String
    Private STR�w�N�P As String
    Private STR�w�N�Q As String
    Private STR�w�N�R As String
    Private STR�w�N�S As String
    Private STR�w�N�T As String
    Private STR�w�N�U As String
    Private STR�w�N�V As String
    Private STR�w�N�W As String
    Private STR�w�N�X As String
    Private STR�P�w�N As String
    Private STR�Q�w�N As String
    Private STR�R�w�N As String
    Private STR�S�w�N As String
    Private STR�T�w�N As String
    Private STR�U�w�N As String
    Private STR�V�w�N As String
    Private STR�W�w�N As String
    Private STR�X�w�N As String

    Private STR�N�ԓ��͐U�֓� As String

    'Private STR���׍쐬�\��� As String
    'Private STR�`�F�b�N�\��� As String
    'Private STR�U�փf�[�^�쐬�\��� As String
    'Private STR�s�\���ʍX�V�\��� As String
    'Private STR���ϗ\��� As String
    Private STRW�ĐU�֔N As String
    Private STRW�ĐU�֌� As String
    Private STRW�ĐU�֓� As String
    Private STR������ As String
    Private STRYasumi_List(0) As String

    Private str���U�֓�(6) As String '2006/11/22
    Private str���ĐU��(6) As String '2006/11/22
    Private int���U�ւh�c As Integer '2006/11/22
    Private str�ʏ�U�֓�(12) As String '2006/11/22
    Private str�ʏ�ĐU��(12) As String '2006/11/22

    Private str�ʏ�āX�U��(12) As String '2006/11/30
    Private str���ʍāX�U��(6) As String '2006/11/30
    Private bln�N�ԍX�V(12) As Boolean '2006/11/30
    Private bln���ʍX�V(6) As Boolean '2006/11/30
    Private bln�����X�V(6) As Boolean '2006/11/30

    Private Int_Zengo_Kbn(1) As String

    '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------START
    Private Sai_Zengo_Kbn As String       '�ĐU�x���V�t�g
    '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------END

    Private Structure NenkanData
        <VBFixedStringAttribute(2)> Public Furikae_Date As String
        <VBFixedStringAttribute(2)> Public SaiFurikae_Date As String
        <VBFixedStringAttribute(10)> Public Furikae_Day As String
        <VBFixedStringAttribute(10)> Public SaiFurikae_Day As String
        Public Furikae_Check As Boolean
        Public SaiFurikae_Check As Boolean
        Public Furikae_Enabled As Boolean
        Public SaiFurikae_Enabled As Boolean
        Public CheckFurikae_Flag As Boolean '2006/11/30
        Public FunouFurikae_Flag As Boolean '2006/11/30
        Public CheckSaiFurikae_Flag As Boolean '2006/11/30
    End Structure
    Private NENKAN_SCHINFO(12) As NenkanData
    Private SYOKI_NENKAN_SCHINFO(12) As NenkanData '2006/11/30

    Private Structure TokubetuData
        <VBFixedStringAttribute(2)> Public Seikyu_Tuki As String
        Public SyoriFurikae_Flag As Boolean
        Public CheckFurikae_Flag As Boolean '2006/11/30
        Public FunouFurikae_Flag As Boolean '2006/11/30
        <VBFixedStringAttribute(2)> Public Furikae_Tuki As String
        <VBFixedStringAttribute(2)> Public Furikae_Date As String
        Public SyoriSaiFurikae_Flag As Boolean
        Public CheckSaiFurikae_Flag As Boolean '2006/11/30
        <VBFixedStringAttribute(2)> Public SaiFurikae_Tuki As String
        <VBFixedStringAttribute(2)> Public SaiFurikae_Date As String
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
    End Structure
    Private TOKUBETU_SCHINFO(6) As TokubetuData
    Private SYOKI_TOKUBETU_SCHINFO(6) As TokubetuData

    Private Structure ZuijiData
        <VBFixedStringAttribute(2)> Public Nyusyutu_Kbn As String
        <VBFixedStringAttribute(2)> Public Furikae_Tuki As String
        <VBFixedStringAttribute(2)> Public Furikae_Date As String
        Public Syori_Flag As Boolean
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
    End Structure
    Private ZUIJI_SCHINFO(6) As ZuijiData
    Private SYOKI_ZUIJI_SCHINFO(6) As ZuijiData

    Private Structure GakData
        <VBFixedStringAttribute(7)> Public GAKKOU_CODE As String
        <VBFixedStringAttribute(50)> Public GAKKOU_NNAME As String
        Public SIYOU_GAKUNEN As Integer
        <VBFixedStringAttribute(2)> Public FURI_DATE As String
        <VBFixedStringAttribute(2)> Public SFURI_DATE As String
        <VBFixedStringAttribute(1)> Public BAITAI_CODE As String
        <VBFixedStringAttribute(10)> Public ITAKU_CODE As String
        <VBFixedStringAttribute(4)> Public TKIN_CODE As String
        <VBFixedStringAttribute(3)> Public TSIT_CODE As String
        <VBFixedStringAttribute(1)> Public SFURI_SYUBETU As String
        <VBFixedStringAttribute(6)> Public KAISI_DATE As String
        <VBFixedStringAttribute(6)> Public SYURYOU_DATE As String
        <VBFixedStringAttribute(1)> Public TESUUTYO_KBN As String
        <VBFixedStringAttribute(1)> Public TESUUTYO_KIJITSU As String
        Public TESUUTYO_NO As Integer
        <VBFixedStringAttribute(1)> Public TESUU_KYU_CODE As String
        <VBFixedStringAttribute(6)> Public TAISYOU_START_NENDO As String
        <VBFixedStringAttribute(6)> Public TAISYOU_END_NENDO As String
    End Structure
    Private GAKKOU_INFO As GakData

    Private Str_SyoriDate(1) As String

    '������(0:�N��1:����2:����)
    '0:�X�P�W���[�����쐬
    '1:�X�P�W���[���쐬����
    '2:�X�P�W���[���쐬���s
    Private Int_Syori_Flag(2) As Integer

    Private Int_Zuiji_Flag As Integer
    Private Int_Tokubetu_Flag As Integer


    Private Str_FURI_DATE As String
    Private Str_SFURI_DATE As String

    Private strFURI_DT As String '�w�Z�}�X�^�Q�̐U�֓�
    Private strSFURI_DT As String '�w�Z�}�X�^�Q�̍ĐU�֓�

    '2006/10/24
    Private strENTRI_FLG As String = "0"
    Private strCHECK_FLG As String = "0"
    Private strDATA_FLG As String = "0"
    Private strFUNOU_FLG As String = "0"
    Private strHENKAN_FLG As String = "0"
    Private strSAIFURI_FLG As String = "0"
    Private strKESSAI_FLG As String = "0"
    Private strTYUUDAN_FLG As String = "0"
    Private strENTRI_FLG_SAI As String = "0"
    Private strCHECK_FLG_SAI As String = "0"
    Private strDATA_FLG_SAI As String = "0"
    Private strFUNOU_FLG_SAI As String = "0"
    Private strSAIFURI_FLG_SAI As String = "0"
    Private strKESSAI_FLG_SAI As String = "0"
    Private strTYUUDAN_FLG_SAI As String = "0"

    Private strSAIFURI_DEF As String = "00000000" '�ʏ�X�P�W���[���̍ĐU��

    Private lngSYORI_KEN As Long = 0
    Private dblSYORI_KIN As Double = 0
    Private lngFURI_KEN As Long = 0
    Private dblFURI_KIN As Double = 0
    Private lngFUNOU_KEN As Long = 0
    Private dblFUNOU_KIN As Double = 0

    '��Ǝ��U�X�P�W���[���A�g�p�@2006/12/01
    Private strSYOFURI_NENKAN(12) As String
    Private strSAIFURI_NENKAN(12) As String
    Private strSYOFURI_TOKUBETU(6) As String
    Private strSAIFURI_TOKUBETU(6) As String
    Private strFURI_ZUIJI(6) As String
    Private strFURIKBN_ZUIJI(6) As String
    Private strSYOFURI_NENKAN_AFTER(12) As String '�X�V��̃X�P�W���[��
    Private strSAIFURI_NENKAN_AFTER(12) As String '�X�V��̃X�P�W���[��
    Private strSYOFURI_TOKUBETU_AFTER(6) As String '�X�V��̃X�P�W���[��
    Private strSAIFURI_TOKUBETU_AFTER(6) As String '�X�V��̃X�P�W���[��
    Private strFURI_ZUIJI_AFTER(6) As String '�X�V��̃X�P�W���[��
    Private strFURIKBN_ZUIJI_AFTER(6) As String '�X�V��̃X�P�W���[��

    Private intPUSH_BTN As Integer '0:�쐬�@1:�Q�� 2:�X�V 3:���
#End Region

    '2010.02.27 �ϐ������̂��ߐV�K�쐬 ��************
    Private strGakkouCode As String

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST060", "�N�ԃX�P�W���[���쐬���")
    Private Const msgTitle As String = "�N�ԃX�P�W���[���쐬���(KFGMAST060)"
    Private MainDB As MyOracle

#Region " Form_Load "
    Private Sub KFGMAST060_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '���O�p
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")
                MessageBox.Show("�w�Z���R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '�e�L�X�g�t�@�C������R���{�{�b�N�X�ݒ�
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�P) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmb���o�敪�P)")
                MessageBox.Show("���o�敪�R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�Q) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmb���o�敪�Q)")
                MessageBox.Show("���o�敪�R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�R) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmb���o�敪�R)")
                MessageBox.Show("���o�敪�R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�S) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmb���o�敪�S)")
                MessageBox.Show("���o�敪�R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�T) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmb���o�敪�T)")
                MessageBox.Show("���o�敪�R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�U) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmb���o�敪�U)")
                MessageBox.Show("���o�敪�R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '������ʕ\��
            Call PSUB_FORMAT_ALL()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub
#End Region

#Region " Button Click "
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click


        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)�J�n", "����", "")

            Cursor.Current = Cursors.WaitCursor()
            intPUSH_BTN = 0

            strGakkouCode = Trim(txtGAKKOU_CODE.Text)

            '2010/10/21 ����0�܂���12�����傫���ݒ肳�ꂽ�ꍇ�̓G���[ ��������
            For Each txt���ʌ� As Control In TabPage2.Controls
                If Mid(txt���ʌ�.Name, 1, 8) = "txt���ʐ�����" OrElse Mid(txt���ʌ�.Name, 1, 8) = "txt���ʐU�֌�" _
                    OrElse Mid(txt���ʌ�.Name, 1, 9) = "txt���ʍĐU�֌�" Then
                    If txt���ʌ�.Text <> "" Then
                        If CInt(txt���ʌ�.Text) > 12 OrElse CInt(txt���ʌ�.Text) = 0 Then
                            MessageBox.Show("���͂P�`�P�Q��ݒ肵�Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txt���ʌ�.Focus()
                            Exit Sub
                        End If
                    End If
                End If
            Next

            For Each txt�����U�֌� As Control In TabPage3.Controls
                If Mid(txt�����U�֌�.Name, 1, 8) = "txt�����U�֌�" Then
                    If txt�����U�֌�.Text <> "" Then
                        If CInt(txt�����U�֌�.Text) > 12 OrElse CInt(txt�����U�֌�.Text) = 0 Then
                            MessageBox.Show("���͂P�`�P�Q��ݒ肵�Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txt�����U�֌�.Focus()
                            Exit Sub
                        End If
                    End If
                End If
            Next
            '2010/10/21 ����0�܂���12�����傫���ݒ肳�ꂽ�ꍇ�̓G���[ �����܂�

            Call sb_HENSU_CLEAR()

            '2006/12/08�@�u�쐬����v�Ƃ����t���O�𗧂Ă�
            Call PSUB_Kousin_Check()

            If PFUNC_SCH_INSERT_ALL() = False Then
                Return
            End If

            '���͍��ڐ���
            txt�Ώ۔N�x.Enabled = False
            txtGAKKOU_CODE.Enabled = False

            If Int_Syori_Flag(0) = 2 Then '�ǉ� 2005/06/15
                '���̓{�^������
                Call PSUB_BUTTON_Enable(0)
            Else
                '���̓{�^������
                Call PSUB_BUTTON_Enable(1)
            End If

            Call sb_SANSYOU_SET()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)�I��", "����", "")
        End Try
        

    End Sub
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�J�n", "����", "")
            MainDB = New MyOracle

            Cursor.Current = Cursors.WaitCursor()
            intPUSH_BTN = 1

            '�Q�ƃ{�^��
            strGakkouCode = Trim(txtGAKKOU_CODE.Text)

            If PFUNC_SCH_GET_ALL() = False Then
                Exit Sub
            End If

            '2006/10/11�@�ō��w�N�ȏ�̊w�N�̎g�p�s��
            PSUB_TGAKUNEN_CHK()
            PSUB_ZGAKUNEN_CHK()

            '���̓{�^������
            Call PSUB_BUTTON_Enable(1)

            '��ƘA�g���� 2006/12/04
            Call sb_SANSYOU_SET()

        Catch ex As Exception

            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)��O�G���[", "���s", ex.Message)

        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�I��", "����", "")
            MainDB.Close()
        End Try

    End Sub
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUPDATE.Click


        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�J�n", "����", "")
            MainDB = New MyOracle

            Cursor.Current = Cursors.WaitCursor()
            intPUSH_BTN = 2

            strGakkouCode = Trim(txtGAKKOU_CODE.Text)

            '2010/10/21 ����0�܂���12�����傫���ݒ肳�ꂽ�ꍇ�̓G���[ ��������
            For Each txt���ʌ� As Control In TabPage2.Controls
                If Mid(txt���ʌ�.Name, 1, 8) = "txt���ʐ�����" OrElse Mid(txt���ʌ�.Name, 1, 8) = "txt���ʐU�֌�" _
                    OrElse Mid(txt���ʌ�.Name, 1, 9) = "txt���ʍĐU�֌�" Then
                    If txt���ʌ�.Text <> "" Then
                        If CInt(txt���ʌ�.Text) > 12 OrElse CInt(txt���ʌ�.Text) = 0 Then
                            MessageBox.Show("���͂P�`�P�Q��ݒ肵�Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txt���ʌ�.Focus()
                            Exit Sub
                        End If
                    End If
                End If
            Next

            '2011/06/16 �W���ŏC�� ���ʐU�֓����փ`�F�b�N�ǉ� ------------------START
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If
            '2011/06/16 �W���ŏC�� ���ʐU�֓����փ`�F�b�N�ǉ� ------------------END

            For Each txt�����U�֌� As Control In TabPage3.Controls
                If Mid(txt�����U�֌�.Name, 1, 8) = "txt�����U�֌�" Then
                    If txt�����U�֌�.Text <> "" Then
                        If CInt(txt�����U�֌�.Text) > 12 OrElse CInt(txt�����U�֌�.Text) = 0 Then
                            MessageBox.Show("���͂P�`�P�Q��ݒ肵�Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txt�����U�֌�.Focus()
                            Exit Sub
                        End If
                    End If
                End If
            Next
            '2010/10/21 ����0�܂���12�����傫���ݒ肳�ꂽ�ꍇ�̓G���[ �����܂�

            Call sb_HENSU_CLEAR()

            If PFUNC_SCH_DELETE_INSERT_ALL() = False Then

                MainDB.Rollback()
                Return

            End If

            MainDB.Commit()

            '���͍��ڐ���
            txt�Ώ۔N�x.Enabled = True
            txtGAKKOU_CODE.Enabled = True
            '2006/10/11�@�ō��w�N�ȏ�̊w�N�̎g�p�s��
            PSUB_TGAKUNEN_CHK()
            PSUB_ZGAKUNEN_CHK()

            '���̓{�^������
            Call PSUB_BUTTON_Enable(2)

        Catch ex As Exception

            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)��O�G���[", "���s", ex.Message)
            MainDB.Rollback()

        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�I��", "����", "")
            MainDB.Close()

        End Try

    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click
        intPUSH_BTN = 3

        '����{�^��

        '��ʏ������
        Call PSUB_FORMAT_ALL()

        '�ǉ� 2006/12/27
        ReDim SYOKI_NENKAN_SCHINFO(12)
        ReDim SYOKI_TOKUBETU_SCHINFO(6)
        ReDim SYOKI_ZUIJI_SCHINFO(6)
        ReDim NENKAN_SCHINFO(12)
        ReDim TOKUBETU_SCHINFO(6)
        ReDim ZUIJI_SCHINFO(6)

        txt�Ώ۔N�x.Focus()

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)��O�G���[", "���s", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub
#End Region

#Region " GotFocus "
    '�w�Z���
    Private Sub txt�Ώ۔N�x_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�Ώ۔N�x.GotFocus
        Me.txt�Ώ۔N�x.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�Ώ۔N�x)

    End Sub
    Private Sub txtGAKKOU_CODE_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.GotFocus
        Me.txtGAKKOU_CODE.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txtGAKKOU_CODE)

    End Sub
    '�N�ԃX�P�W���[��
    Private Sub txt�S���U�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�S���U�֓�.GotFocus
        Me.txt�S���U�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�S���U�֓�)

    End Sub
    Private Sub txt�T���U�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�T���U�֓�.GotFocus
        Me.txt�T���U�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�T���U�֓�)

    End Sub
    Private Sub txt�U���U�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�U���U�֓�.GotFocus
        Me.txt�U���U�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�U���U�֓�)

    End Sub
    Private Sub txt�V���U�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�V���U�֓�.GotFocus
        Me.txt�V���U�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�V���U�֓�)

    End Sub
    Private Sub txt�W���U�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�W���U�֓�.GotFocus
        Me.txt�W���U�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�W���U�֓�)

    End Sub
    Private Sub txt�X���U�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�X���U�֓�.GotFocus
        Me.txt�X���U�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�X���U�֓�)

    End Sub
    Private Sub txt�P�O���U�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P�O���U�֓�.GotFocus
        Me.txt�P�O���U�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�P�O���U�֓�)

    End Sub
    Private Sub txt�P�P���U�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P�P���U�֓�.GotFocus
        Me.txt�P�P���U�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�P�P���U�֓�)

    End Sub
    Private Sub txt�P�Q���U�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P�Q���U�֓�.GotFocus
        Me.txt�P�Q���U�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�P�Q���U�֓�)

    End Sub
    Private Sub txt�P���U�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P���U�֓�.GotFocus
        Me.txt�P���U�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�P���U�֓�)

    End Sub
    Private Sub txt�Q���U�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�Q���U�֓�.GotFocus
        Me.txt�Q���U�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�Q���U�֓�)

    End Sub
    Private Sub txt�R���U�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�R���U�֓�.GotFocus
        Me.txt�R���U�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�R���U�֓�)

    End Sub
    Private Sub txt�S���ĐU�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�S���ĐU�֓�.GotFocus
        Me.txt�S���ĐU�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�S���ĐU�֓�)

    End Sub
    Private Sub txt�T���ĐU�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�T���ĐU�֓�.GotFocus
        Me.txt�T���ĐU�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�T���ĐU�֓�)

    End Sub
    Private Sub txt�U���ĐU�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�U���ĐU�֓�.GotFocus
        Me.txt�U���ĐU�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�U���ĐU�֓�)

    End Sub
    Private Sub txt�V���ĐU�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�V���ĐU�֓�.GotFocus
        Me.txt�V���ĐU�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�V���ĐU�֓�)

    End Sub
    Private Sub txt�W���ĐU�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�W���ĐU�֓�.GotFocus
        Me.txt�W���ĐU�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�W���ĐU�֓�)

    End Sub
    Private Sub txt�X���ĐU�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�X���ĐU�֓�.GotFocus
        Me.txt�X���ĐU�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�X���ĐU�֓�)

    End Sub
    Private Sub txt�P�O���ĐU�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P�O���ĐU�֓�.GotFocus
        Me.txt�P�O���ĐU�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�P�O���ĐU�֓�)

    End Sub
    Private Sub txt�P�P���ĐU�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P�P���ĐU�֓�.GotFocus
        Me.txt�P�P���ĐU�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�P�P���ĐU�֓�)

    End Sub
    Private Sub txt�P�Q���ĐU�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P�Q���ĐU�֓�.GotFocus
        Me.txt�P�Q���ĐU�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�P�Q���ĐU�֓�)

    End Sub
    Private Sub txt�P���ĐU�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P���ĐU�֓�.GotFocus
        Me.txt�P���ĐU�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�P���ĐU�֓�)

    End Sub
    Private Sub txt�Q���ĐU�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�Q���ĐU�֓�.GotFocus
        Me.txt�Q���ĐU�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�Q���ĐU�֓�)

    End Sub
    Private Sub txt�R���ĐU�֓�_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�R���ĐU�֓�.GotFocus
        Me.txt�R���ĐU�֓�.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�R���ĐU�֓�)

    End Sub
    '���ʃX�P�W���[��
    Private Sub txt���ʐ������P_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐ������P.GotFocus
        Me.txt���ʐ������P.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐ������P)

    End Sub
    Private Sub txt���ʐ������Q_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐ������Q.GotFocus
        Me.txt���ʐ������Q.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐ������Q)

    End Sub
    Private Sub txt���ʐ������R_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐ������R.GotFocus
        Me.txt���ʐ������R.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐ������R)

    End Sub
    Private Sub txt���ʐ������S_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐ������S.GotFocus
        Me.txt���ʐ������S.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐ������S)

    End Sub
    Private Sub txt���ʐ������T_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐ������T.GotFocus
        Me.txt���ʐ������T.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐ������T)

    End Sub
    Private Sub txt���ʐ������U_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐ������U.GotFocus
        Me.txt���ʐ������U.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐ������U)

    End Sub
    Private Sub txt���ʐU�֌��P_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֌��P.GotFocus
        Me.txt���ʐU�֌��P.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐU�֌��P)

    End Sub
    Private Sub txt���ʐU�֌��Q_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֌��Q.GotFocus
        Me.txt���ʐU�֌��Q.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐU�֌��Q)

    End Sub
    Private Sub txt���ʐU�֌��R_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֌��R.GotFocus
        Me.txt���ʐU�֌��R.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐU�֌��R)

    End Sub
    Private Sub txt���ʐU�֌��S_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֌��S.GotFocus
        Me.txt���ʐU�֌��S.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐU�֌��S)

    End Sub
    Private Sub txt���ʐU�֌��T_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֌��T.GotFocus
        Me.txt���ʐU�֌��T.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐU�֌��T)

    End Sub
    Private Sub txt���ʐU�֌��U_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֌��U.GotFocus
        Me.txt���ʐU�֌��U.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐU�֌��U)

    End Sub
    Private Sub txt���ʐU�֓��P_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֓��P.GotFocus
        Me.txt���ʐU�֓��P.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐU�֓��P)

    End Sub
    Private Sub txt���ʐU�֓��Q_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֓��Q.GotFocus
        Me.txt���ʐU�֓��Q.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐU�֓��Q)

    End Sub
    Private Sub txt���ʐU�֓��R_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֓��R.GotFocus
        Me.txt���ʐU�֓��R.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐU�֓��R)

    End Sub
    Private Sub txt���ʐU�֓��S_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֓��S.GotFocus
        Me.txt���ʐU�֓��S.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐU�֓��S)

    End Sub
    Private Sub txt���ʐU�֓��T_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֓��T.GotFocus
        Me.txt���ʐU�֓��T.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐU�֓��T)

    End Sub
    Private Sub txt���ʐU�֓��U_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֓��U.GotFocus
        Me.txt���ʐU�֓��U.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʐU�֓��U)

    End Sub
    Private Sub txt���ʍĐU�֌��P_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֌��P.GotFocus
        Me.txt���ʍĐU�֌��P.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʍĐU�֌��P)

    End Sub
    Private Sub txt���ʍĐU�֌��Q_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֌��Q.GotFocus
        Me.txt���ʍĐU�֌��Q.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʍĐU�֌��Q)

    End Sub
    Private Sub txt���ʍĐU�֌��R_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֌��R.GotFocus
        Me.txt���ʍĐU�֌��R.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʍĐU�֌��R)

    End Sub
    Private Sub txt���ʍĐU�֌��S_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֌��S.GotFocus
        Me.txt���ʍĐU�֌��S.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʍĐU�֌��S)

    End Sub
    Private Sub txt���ʍĐU�֌��T_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֌��T.GotFocus
        Me.txt���ʍĐU�֌��T.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʍĐU�֌��T)

    End Sub
    Private Sub txt���ʍĐU�֌��U_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֌��U.GotFocus
        Me.txt���ʍĐU�֌��U.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʍĐU�֌��U)

    End Sub
    Private Sub txt���ʍĐU�֓��P_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֓��P.GotFocus
        Me.txt���ʍĐU�֓��P.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʍĐU�֓��P)

    End Sub
    Private Sub txt���ʍĐU�֓��Q_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֓��Q.GotFocus
        Me.txt���ʍĐU�֓��Q.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʍĐU�֓��Q)

    End Sub
    Private Sub txt���ʍĐU�֓��R_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֓��R.GotFocus
        Me.txt���ʍĐU�֓��R.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʍĐU�֓��R)

    End Sub
    Private Sub txt���ʍĐU�֓��S_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֓��S.GotFocus
        Me.txt���ʍĐU�֓��S.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʍĐU�֓��S)

    End Sub
    Private Sub txt���ʍĐU�֓��T_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֓��T.GotFocus
        Me.txt���ʍĐU�֓��T.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʍĐU�֓��T)

    End Sub
    Private Sub txt���ʍĐU�֓��U_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֓��U.GotFocus
        Me.txt���ʍĐU�֓��U.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt���ʍĐU�֓��U)

    End Sub
    '�����X�P�W���[��
    Private Sub txt�����U�֌��P_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֌��P.GotFocus
        Me.txt�����U�֌��P.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�����U�֌��P)

    End Sub
    Private Sub txt�����U�֌��Q_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֌��Q.GotFocus
        Me.txt�����U�֌��Q.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�����U�֌��Q)

    End Sub
    Private Sub txt�����U�֌��R_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֌��R.GotFocus
        Me.txt�����U�֌��R.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�����U�֌��R)

    End Sub
    Private Sub txt�����U�֌��S_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֌��S.GotFocus
        Me.txt�����U�֌��S.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�����U�֌��S)

    End Sub
    Private Sub txt�����U�֌��T_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֌��T.GotFocus
        Me.txt�����U�֌��T.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�����U�֌��T)

    End Sub
    Private Sub txt�����U�֌��U_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֌��U.GotFocus
        Me.txt�����U�֌��U.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�����U�֌��U)

    End Sub
    Private Sub txt�����U�֓��P_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֓��P.GotFocus
        Me.txt�����U�֓��P.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�����U�֓��P)

    End Sub
    Private Sub txt�����U�֓��Q_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֓��Q.GotFocus
        Me.txt�����U�֓��Q.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�����U�֓��Q)

    End Sub
    Private Sub txt�����U�֓��R_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֓��R.GotFocus
        Me.txt�����U�֓��R.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�����U�֓��R)

    End Sub
    Private Sub txt�����U�֓��S_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֓��S.GotFocus
        Me.txt�����U�֓��S.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�����U�֓��S)

    End Sub
    Private Sub txt�����U�֓��T_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֓��T.GotFocus
        Me.txt�����U�֓��T.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�����U�֓��T)

    End Sub
    Private Sub txt�����U�֓��U_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֓��U.GotFocus
        Me.txt�����U�֓��U.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt�����U�֓��U)

    End Sub
#End Region

#Region " LostFocus "
    '��{
    Private Sub txt�Ώ۔N�x_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�Ώ۔N�x.LostFocus
        Me.txt�Ώ۔N�x.BackColor = System.Drawing.Color.White
        '�x�����̕\��
        If PFUNC_KYUJITULIST_SET() = False Then
            Exit Sub
        End If

        If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txt�Ώ۔N�x.Text) <> "" Then
            '�Ώ۔N�x�����͂���Ă���ꍇ�A�X�P�W���[�����݃`�F�b�N������
            '�X�P�W���[�������݂���ꍇ�͎Q�ƃ{�^���Ƀt�H�[�J�X�ړ�
            Call PSUB_SANSYOU_FOCUS()
        End If

    End Sub
    Private Sub txtGAKKOU_CODE_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.LostFocus
        Me.txtGAKKOU_CODE.BackColor = System.Drawing.Color.White
        '�w�Z���̎擾
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txtGAKKOU_CODE, 10)

            '�w�Z���̎擾(�w�Z�����ϐ��Ɋi�[�����)
            If PFUNC_GAKINFO_GET() = False Then
                Exit Sub
            End If

            '�N�ԃX�P�W���[����ʏ�����
            Call PSUB_NENKAN_FORMAT()

            '���ʃX�P�W���[����ʏ�����
            Call PSUB_TOKUBETU_FORMAT()

            '�����X�P�W���[����ʏ�����
            Call PSUB_ZUIJI_FORMAT()

            '�ĐU�֓��̃v���e�N�gTrue
            Call PSUB_SAIFURI_PROTECT(True)

            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "0", "3"
                    Call PSUB_SAIFURI_PROTECT(False)
            End Select

            '2006/10/12�@�ō��w�N�ȏ�̊w�N�̃`�F�b�N�{�b�N�X�g�p�s��
            PSUB_TGAKUNEN_CHK()
            PSUB_ZGAKUNEN_CHK()

            If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txt�Ώ۔N�x.Text) <> "" Then
                '�Ώ۔N�x�����͂���Ă���ꍇ�A�X�P�W���[�����݃`�F�b�N������
                '�X�P�W���[�������݂���ꍇ�͎Q�ƃ{�^���Ƀt�H�[�J�X�ړ�
                Call PSUB_SANSYOU_FOCUS()
            End If
        Else
            '2006/10/12�@�w�Z�R�[�h���󔒂̂Ƃ��A�w�Z�����x�����󔒂ɂ���
            lab�w�Z��.Text = ""
        End If

    End Sub
    '�N��
    Private Sub txt���ʐ������P_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐ������P.LostFocus
        Me.txt���ʐ������P.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐ������P.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐ������P, 2)
        End If

    End Sub
    Private Sub txt���ʐ������Q_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐ������Q.LostFocus
        Me.txt���ʐ������Q.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐ������Q.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐ������Q, 2)
        End If

    End Sub
    Private Sub txt���ʐ������R_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐ������R.LostFocus
        Me.txt���ʐ������R.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐ������R.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐ������R, 2)
        End If

    End Sub
    Private Sub txt���ʐ������S_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐ������S.LostFocus
        Me.txt���ʐ������S.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐ������S.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐ������S, 2)
        End If

    End Sub
    Private Sub txt���ʐ������T_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐ������T.LostFocus
        Me.txt���ʐ������T.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐ������T.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐ������T, 2)
        End If

    End Sub
    Private Sub txt���ʐ������U_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐ������U.LostFocus
        Me.txt���ʐ������U.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐ������U.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐ������U, 2)
        End If

    End Sub
    Private Sub txt�S���U�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�S���U�֓�.LostFocus
        Me.txt�S���U�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�S���U�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�S���U�֓�, 2)
        End If

    End Sub
    Private Sub txt�T���U�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�T���U�֓�.LostFocus
        Me.txt�T���U�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�T���U�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�T���U�֓�, 2)
        End If

    End Sub
    Private Sub txt�U���U�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�U���U�֓�.LostFocus
        Me.txt�U���U�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�U���U�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�U���U�֓�, 2)
        End If

    End Sub
    Private Sub txt�V���U�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�V���U�֓�.LostFocus
        Me.txt�V���U�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�V���U�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�V���U�֓�, 2)
        End If

    End Sub
    Private Sub txt�W���U�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�W���U�֓�.LostFocus
        Me.txt�W���U�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�W���U�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�W���U�֓�, 2)
        End If

    End Sub
    Private Sub txt�X���U�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�X���U�֓�.LostFocus
        Me.txt�X���U�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�X���U�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�X���U�֓�, 2)
        End If

    End Sub
    Private Sub txt�P�O���U�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P�O���U�֓�.LostFocus
        Me.txt�P�O���U�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�P�O���U�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�P�O���U�֓�, 2)
        End If

    End Sub
    Private Sub txt�P�P���U�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P�P���U�֓�.LostFocus
        Me.txt�P�P���U�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�P�P���U�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�P�P���U�֓�, 2)
        End If

    End Sub
    Private Sub txt�P�Q���U�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P�Q���U�֓�.LostFocus
        Me.txt�P�Q���U�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�P�Q���U�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�P�Q���U�֓�, 2)
        End If

    End Sub
    Private Sub txt�P���U�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P���U�֓�.LostFocus
        Me.txt�P���U�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�P���U�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�P���U�֓�, 2)
        End If

    End Sub
    Private Sub txt�Q���U�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�Q���U�֓�.LostFocus
        Me.txt�Q���U�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�Q���U�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�Q���U�֓�, 2)
        End If

    End Sub
    Private Sub txt�R���U�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�R���U�֓�.LostFocus
        Me.txt�R���U�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�R���U�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�R���U�֓�, 2)
        End If

    End Sub
    Private Sub txt�S���ĐU�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�S���ĐU�֓�.LostFocus
        Me.txt�S���ĐU�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�S���ĐU�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�S���ĐU�֓�, 2)
        End If

    End Sub
    Private Sub txt�T���ĐU�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�T���ĐU�֓�.LostFocus
        Me.txt�T���ĐU�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�T���ĐU�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�T���ĐU�֓�, 2)
        End If

    End Sub
    Private Sub txt�U���ĐU�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�U���ĐU�֓�.LostFocus
        Me.txt�U���ĐU�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�U���ĐU�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�U���ĐU�֓�, 2)
        End If

    End Sub
    Private Sub txt�V���ĐU�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�V���ĐU�֓�.LostFocus
        Me.txt�V���ĐU�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�V���ĐU�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�V���ĐU�֓�, 2)
        End If

    End Sub
    Private Sub txt�W���ĐU�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�W���ĐU�֓�.LostFocus
        Me.txt�W���ĐU�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�W���ĐU�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�W���ĐU�֓�, 2)
        End If

    End Sub
    Private Sub txt�X���ĐU�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�X���ĐU�֓�.LostFocus
        Me.txt�X���ĐU�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�X���ĐU�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�X���ĐU�֓�, 2)
        End If

    End Sub
    Private Sub txt�P�O���ĐU�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P�O���ĐU�֓�.LostFocus
        Me.txt�P�O���ĐU�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�P�O���ĐU�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�P�O���ĐU�֓�, 2)
        End If

    End Sub
    Private Sub txt�P�P���ĐU�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P�P���ĐU�֓�.LostFocus
        Me.txt�P�P���ĐU�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�P�P���ĐU�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�P�P���ĐU�֓�, 2)
        End If

    End Sub
    Private Sub txt�P�Q���ĐU�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P�Q���ĐU�֓�.LostFocus
        Me.txt�P�Q���ĐU�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�P�Q���ĐU�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�P�Q���ĐU�֓�, 2)
        End If

    End Sub
    Private Sub txt�P���ĐU�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�P���ĐU�֓�.LostFocus
        Me.txt�P���ĐU�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�P���ĐU�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�P���ĐU�֓�, 2)
        End If

    End Sub
    Private Sub txt�Q���ĐU�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�Q���ĐU�֓�.LostFocus
        Me.txt�Q���ĐU�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�Q���ĐU�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�Q���ĐU�֓�, 2)
        End If

    End Sub
    Private Sub txt�R���ĐU�֓�_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�R���ĐU�֓�.LostFocus
        Me.txt�R���ĐU�֓�.BackColor = System.Drawing.Color.White
        If Trim(txt�R���ĐU�֓�.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�R���ĐU�֓�, 2)
        End If

    End Sub
    '����
    Private Sub txt���ʐU�֌��P_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֌��P.LostFocus
        Me.txt���ʐU�֌��P.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐU�֌��P.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐU�֌��P, 2)
        End If

    End Sub
    Private Sub txt���ʐU�֌��Q_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֌��Q.LostFocus
        Me.txt���ʐU�֌��Q.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐU�֌��Q.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐU�֌��Q, 2)
        End If

    End Sub
    Private Sub txt���ʐU�֌��R_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֌��R.LostFocus
        Me.txt���ʐU�֌��R.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐU�֌��R.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐU�֌��R, 2)
        End If

    End Sub
    Private Sub txt���ʐU�֌��S_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֌��S.LostFocus
        Me.txt���ʐU�֌��S.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐU�֌��S.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐU�֌��S, 2)
        End If

    End Sub
    Private Sub txt���ʐU�֌��T_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֌��T.LostFocus
        Me.txt���ʐU�֌��T.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐU�֌��T.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐU�֌��T, 2)
        End If

    End Sub
    Private Sub txt���ʐU�֌��U_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֌��U.LostFocus
        Me.txt���ʐU�֌��U.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐU�֌��U.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐU�֌��U, 2)
        End If

    End Sub
    Private Sub txt���ʐU�֓��P_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֓��P.LostFocus
        Me.txt���ʐU�֓��P.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐU�֓��P.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐU�֓��P, 2)
        End If

    End Sub
    Private Sub txt���ʐU�֓��Q_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֓��Q.LostFocus
        Me.txt���ʐU�֓��Q.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐU�֓��Q.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐU�֓��Q, 2)
        End If

    End Sub
    Private Sub txt���ʐU�֓��R_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֓��R.LostFocus
        Me.txt���ʐU�֓��R.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐU�֓��R.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐU�֓��R, 2)
        End If

    End Sub
    Private Sub txt���ʐU�֓��S_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֓��S.LostFocus
        Me.txt���ʐU�֓��S.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐU�֓��S.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐU�֓��S, 2)
        End If

    End Sub
    Private Sub txt���ʐU�֓��T_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֓��T.LostFocus
        Me.txt���ʐU�֓��T.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐU�֓��T.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐU�֓��T, 2)
        End If

    End Sub
    Private Sub txt���ʐU�֓��U_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʐU�֓��U.LostFocus
        Me.txt���ʐU�֓��U.BackColor = System.Drawing.Color.White
        If Trim(txt���ʐU�֓��U.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʐU�֓��U, 2)
        End If

    End Sub
    Private Sub txt���ʍĐU�֌��P_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֌��P.LostFocus
        Me.txt���ʍĐU�֌��P.BackColor = System.Drawing.Color.White
        If Trim(txt���ʍĐU�֌��P.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʍĐU�֌��P, 2)
        End If

    End Sub
    Private Sub txt���ʍĐU�֌��Q_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֌��Q.LostFocus
        Me.txt���ʍĐU�֌��Q.BackColor = System.Drawing.Color.White
        If Trim(txt���ʍĐU�֌��Q.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʍĐU�֌��Q, 2)
        End If

    End Sub
    Private Sub txt���ʍĐU�֌��R_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֌��R.LostFocus
        Me.txt���ʍĐU�֌��R.BackColor = System.Drawing.Color.White
        If Trim(txt���ʍĐU�֌��R.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʍĐU�֌��R, 2)
        End If

    End Sub
    Private Sub txt���ʍĐU�֌��S_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֌��S.LostFocus
        Me.txt���ʍĐU�֌��S.BackColor = System.Drawing.Color.White
        If Trim(txt���ʍĐU�֌��S.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʍĐU�֌��S, 2)
        End If

    End Sub
    Private Sub txt���ʍĐU�֌��T_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֌��T.LostFocus
        Me.txt���ʍĐU�֌��T.BackColor = System.Drawing.Color.White
        If Trim(txt���ʍĐU�֌��T.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʍĐU�֌��T, 2)
        End If

    End Sub
    Private Sub txt���ʍĐU�֌��U_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֌��U.LostFocus
        Me.txt���ʍĐU�֌��U.BackColor = System.Drawing.Color.White
        If Trim(txt���ʍĐU�֌��U.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʍĐU�֌��U, 2)
        End If

    End Sub
    Private Sub txt���ʍĐU�֓��P_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֓��P.LostFocus
        Me.txt���ʍĐU�֓��P.BackColor = System.Drawing.Color.White
        If Trim(txt���ʍĐU�֓��P.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʍĐU�֓��P, 2)
        End If

    End Sub
    Private Sub txt���ʍĐU�֓��Q_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֓��Q.LostFocus
        Me.txt���ʍĐU�֓��Q.BackColor = System.Drawing.Color.White
        If Trim(txt���ʍĐU�֓��Q.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʍĐU�֓��Q, 2)
        End If

    End Sub
    Private Sub txt���ʍĐU�֓��R_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֓��R.LostFocus
        Me.txt���ʍĐU�֓��R.BackColor = System.Drawing.Color.White
        If Trim(txt���ʍĐU�֓��R.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʍĐU�֓��R, 2)
        End If

    End Sub
    Private Sub txt���ʍĐU�֓��S_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֓��S.LostFocus
        Me.txt���ʍĐU�֓��S.BackColor = System.Drawing.Color.White
        If Trim(txt���ʍĐU�֓��S.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʍĐU�֓��S, 2)
        End If

    End Sub
    Private Sub txt���ʍĐU�֓��T_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֓��T.LostFocus
        Me.txt���ʍĐU�֓��T.BackColor = System.Drawing.Color.White
        If Trim(txt���ʍĐU�֓��T.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʍĐU�֓��T, 2)
        End If

    End Sub
    Private Sub txt���ʍĐU�֓��U_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt���ʍĐU�֓��U.LostFocus
        Me.txt���ʍĐU�֓��U.BackColor = System.Drawing.Color.White
        If Trim(txt���ʍĐU�֓��U.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt���ʍĐU�֓��U, 2)
        End If

    End Sub
    '����
    Private Sub txt�����U�֌��P_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֌��P.LostFocus
        Me.txt�����U�֌��P.BackColor = System.Drawing.Color.White
        If Trim(txt�����U�֌��P.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�����U�֌��P, 2)
        End If

    End Sub
    Private Sub txt�����U�֌��Q_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֌��Q.LostFocus
        Me.txt�����U�֌��Q.BackColor = System.Drawing.Color.White
        If Trim(txt�����U�֌��Q.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�����U�֌��Q, 2)
        End If

    End Sub
    Private Sub txt�����U�֌��R_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֌��R.LostFocus
        Me.txt�����U�֌��R.BackColor = System.Drawing.Color.White
        If Trim(txt�����U�֌��R.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�����U�֌��R, 2)
        End If

    End Sub
    Private Sub txt�����U�֌��S_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֌��S.LostFocus
        Me.txt�����U�֌��S.BackColor = System.Drawing.Color.White
        If Trim(txt�����U�֌��S.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�����U�֌��S, 2)
        End If

    End Sub
    Private Sub txt�����U�֌��T_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֌��T.LostFocus
        Me.txt�����U�֌��T.BackColor = System.Drawing.Color.White
        If Trim(txt�����U�֌��T.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�����U�֌��T, 2)
        End If

    End Sub
    Private Sub txt�����U�֌��U_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֌��U.LostFocus
        Me.txt�����U�֌��U.BackColor = System.Drawing.Color.White
        If Trim(txt�����U�֌��U.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�����U�֌��U, 2)
        End If

    End Sub
    Private Sub txt�����U�֓��P_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֓��P.LostFocus
        Me.txt�����U�֓��P.BackColor = System.Drawing.Color.White
        If Trim(txt�����U�֓��P.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�����U�֓��P, 2)
        End If

    End Sub
    Private Sub txt�����U�֓��Q_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֓��Q.LostFocus
        Me.txt�����U�֓��Q.BackColor = System.Drawing.Color.White
        If Trim(txt�����U�֓��Q.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�����U�֓��Q, 2)
        End If

    End Sub
    Private Sub txt�����U�֓��R_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֓��R.LostFocus
        Me.txt�����U�֓��R.BackColor = System.Drawing.Color.White
        If Trim(txt�����U�֓��R.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�����U�֓��R, 2)
        End If

    End Sub
    Private Sub txt�����U�֓��S_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֓��S.LostFocus
        Me.txt�����U�֓��S.BackColor = System.Drawing.Color.White
        If Trim(txt�����U�֓��S.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�����U�֓��S, 2)
        End If

    End Sub
    Private Sub txt�����U�֓��T_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֓��T.LostFocus
        Me.txt�����U�֓��T.BackColor = System.Drawing.Color.White
        If Trim(txt�����U�֓��T.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�����U�֓��T, 2)
        End If

    End Sub
    Private Sub txt�����U�֓��U_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt�����U�֓��U.LostFocus
        Me.txt�����U�֓��U.BackColor = System.Drawing.Color.White
        If Trim(txt�����U�֓��U.Text) <> "" Then
            '�O�t��
            Call GFUNC_ZERO_ADD(txt�����U�֓��U, 2)
        End If

    End Sub
#End Region

#Region " KeyPress "
    '��{
    Private Sub txt�Ώ۔N�x_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�Ώ۔N�x.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txtGAKKOU_CODE_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtGAKKOU_CODE.KeyPress
        '�w�Z�R�[�h��KEY���͐���
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    '�N��
    Private Sub txt�S���U�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�S���U�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�T���U�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�T���U�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�U���U�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�U���U�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�V���U�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�V���U�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�W���U�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�W���U�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�X���U�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�X���U�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�P�O���U�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�P�O���U�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�P�P���U�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�P�P���U�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�P�Q���U�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�P�Q���U�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�P���U�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�P���U�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�Q���U�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�Q���U�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�R���U�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�R���U�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�S���ĐU�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�S���ĐU�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�T���ĐU�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�T���ĐU�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�U���ĐU�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�U���ĐU�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�V���ĐU�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�V���ĐU�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�W���ĐU�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�W���ĐU�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�X���ĐU�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�X���ĐU�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�P�O���ĐU�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�P�O���ĐU�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�P�P���ĐU�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�P�P���ĐU�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�P�Q���ĐU�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�P�Q���ĐU�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�P���ĐU�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�P���ĐU�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�Q���ĐU�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�Q���ĐU�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�R���ĐU�֓�_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�R���ĐU�֓�.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    '����
    Private Sub txt���ʐ������P_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐ������P.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt���ʐ������Q_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐ������Q.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt���ʐ������R_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐ������R.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt���ʐ������S_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐ������S.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt���ʐ������T_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐ������T.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt���ʐ������U_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐ������U.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt���ʐU�֌��P_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐU�֌��P.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʐU�֌��Q_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐU�֌��Q.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʐU�֌��R_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐU�֌��R.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʐU�֌��S_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐU�֌��S.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʐU�֌��T_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐU�֌��T.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʐU�֌��U_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐU�֌��U.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʐU�֓��P_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐU�֓��P.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʐU�֓��Q_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐU�֓��Q.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʐU�֓��R_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐU�֓��R.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʐU�֓��S_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐU�֓��S.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʐU�֓��T_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐU�֓��T.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʐU�֓��U_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʐU�֓��U.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʍĐU�֌��P_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʍĐU�֌��P.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʍĐU�֌��Q_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʍĐU�֌��Q.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʍĐU�֌��R_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʍĐU�֌��R.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʍĐU�֌��S_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʍĐU�֌��S.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʍĐU�֌��T_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʍĐU�֌��T.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʍĐU�֌��U_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʍĐU�֌��U.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʍĐU�֓��P_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʍĐU�֓��P.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʍĐU�֓��Q_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʍĐU�֓��Q.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʍĐU�֓��R_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʍĐU�֓��R.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʍĐU�֓��S_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʍĐU�֓��S.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʍĐU�֓��T_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʍĐU�֓��T.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt���ʍĐU�֓��U_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt���ʍĐU�֓��U.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    '����
    Private Sub txt�����U�֌��P_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�����U�֌��P.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�����U�֌��Q_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�����U�֌��Q.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�����U�֌��R_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�����U�֌��R.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�����U�֌��S_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�����U�֌��S.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�����U�֌��T_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�����U�֌��T.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�����U�֌��U_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�����U�֌��U.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�����U�֓��P_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�����U�֓��P.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�����U�֓��Q_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�����U�֓��Q.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�����U�֓��R_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�����U�֓��R.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�����U�֓��S_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�����U�֓��S.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�����U�֓��T_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�����U�֓��T.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt�����U�֓��U_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt�����U�֓��U.KeyPress
        '���͐��l�`�F�b�N
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
#End Region

#Region " KeyUp "
    '�w�Z���
    Private Sub txt�Ώ۔N�x_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�Ώ۔N�x.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�Ώ۔N�x)

    End Sub
    Private Sub txtGAKKOU_CODE_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtGAKKOU_CODE.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txtGAKKOU_CODE)

    End Sub
    '�N�ԃX�P�W���[��
    Private Sub txt�S���U�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�S���U�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�S���U�֓�)

    End Sub
    Private Sub txt�T���U�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�T���U�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�T���U�֓�)

    End Sub
    Private Sub txt�U���U�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�U���U�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�U���U�֓�)

    End Sub
    Private Sub txt�V���U�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�V���U�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�V���U�֓�)

    End Sub
    Private Sub txt�W���U�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�W���U�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�W���U�֓�)

    End Sub
    Private Sub txt�X���U�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�X���U�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�X���U�֓�)

    End Sub
    Private Sub txt�P�O���U�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�P�O���U�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�P�O���U�֓�)

    End Sub
    Private Sub txt�P�P���U�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�P�P���U�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�P�P���U�֓�)

    End Sub
    Private Sub txt�P�Q���U�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�P�Q���U�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�P�Q���U�֓�)

    End Sub
    Private Sub txt�P���U�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�P���U�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�P���U�֓�)

    End Sub
    Private Sub txt�Q���U�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�Q���U�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�Q���U�֓�)

    End Sub
    Private Sub txt�R���U�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�R���U�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�R���U�֓�)

    End Sub
    Private Sub txt�S���ĐU�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�S���ĐU�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�S���ĐU�֓�)

    End Sub
    Private Sub txt�T���ĐU�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�T���ĐU�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�T���ĐU�֓�)

    End Sub
    Private Sub txt�U���ĐU�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�U���ĐU�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�U���ĐU�֓�)

    End Sub
    Private Sub txt�V���ĐU�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�V���ĐU�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�V���ĐU�֓�)

    End Sub
    Private Sub txt�W���ĐU�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�W���ĐU�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�W���ĐU�֓�)

    End Sub
    Private Sub txt�X���ĐU�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�X���ĐU�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�X���ĐU�֓�)

    End Sub
    Private Sub txt�P�O���ĐU�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�P�O���ĐU�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�P�O���ĐU�֓�)

    End Sub
    Private Sub txt�P�P���ĐU�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�P�P���ĐU�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�P�P���ĐU�֓�)

    End Sub
    Private Sub txt�P�Q���ĐU�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�P�Q���ĐU�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�P�Q���ĐU�֓�)

    End Sub
    Private Sub txt�P���ĐU�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�P���ĐU�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�P���ĐU�֓�)

    End Sub
    Private Sub txt�Q���ĐU�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�Q���ĐU�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�Q���ĐU�֓�)

    End Sub
    Private Sub txt�R���ĐU�֓�_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�R���ĐU�֓�.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�R���ĐU�֓�)

    End Sub
    '���ʃX�P�W���[��
    Private Sub txt���ʐ������P_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐ������P.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐ������P)

    End Sub
    Private Sub txt���ʐ������Q_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐ������Q.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐ������Q)

    End Sub
    Private Sub txt���ʐ������R_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐ������R.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐ������R)

    End Sub
    Private Sub txt���ʐ������S_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐ������S.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐ������S)

    End Sub
    Private Sub txt���ʐ������T_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐ������T.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐ������T)

    End Sub
    Private Sub txt���ʐ������U_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐ������U.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐ������U)

    End Sub
    Private Sub txt���ʐU�֌��P_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐU�֌��P.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐU�֌��P)

    End Sub
    Private Sub txt���ʐU�֌��Q_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐU�֌��Q.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐU�֌��Q)

    End Sub
    Private Sub txt���ʐU�֌��R_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐU�֌��R.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐU�֌��R)

    End Sub
    Private Sub txt���ʐU�֌��S_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐU�֌��S.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐU�֌��S)

    End Sub
    Private Sub txt���ʐU�֌��T_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐU�֌��T.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐU�֌��T)

    End Sub
    Private Sub txt���ʐU�֌��U_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐU�֌��U.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐU�֌��U)

    End Sub
    Private Sub txt���ʐU�֓��P_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐU�֓��P.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐU�֓��P)

    End Sub
    Private Sub txt���ʐU�֓��Q_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐU�֓��Q.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐU�֓��Q)

    End Sub
    Private Sub txt���ʐU�֓��R_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐU�֓��R.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐU�֓��R)

    End Sub
    Private Sub txt���ʐU�֓��S_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐU�֓��S.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐU�֓��S)

    End Sub
    Private Sub txt���ʐU�֓��T_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐU�֓��T.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐU�֓��T)

    End Sub
    Private Sub txt���ʐU�֓��U_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʐU�֓��U.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʐU�֓��U)

    End Sub
    Private Sub txt���ʍĐU�֌��P_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʍĐU�֌��P.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʍĐU�֌��P)

    End Sub
    Private Sub txt���ʍĐU�֌��Q_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʍĐU�֌��Q.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʍĐU�֌��Q)

    End Sub
    Private Sub txt���ʍĐU�֌��R_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʍĐU�֌��R.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʍĐU�֌��R)

    End Sub
    Private Sub txt���ʍĐU�֌��S_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʍĐU�֌��S.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʍĐU�֌��S)

    End Sub
    Private Sub txt���ʍĐU�֌��T_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʍĐU�֌��T.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʍĐU�֌��T)

    End Sub
    Private Sub txt���ʍĐU�֌��U_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʍĐU�֌��U.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʍĐU�֌��U)

    End Sub
    Private Sub txt���ʍĐU�֓��P_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʍĐU�֓��P.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʍĐU�֓��P)

    End Sub
    Private Sub txt���ʍĐU�֓��Q_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʍĐU�֓��Q.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʍĐU�֓��Q)

    End Sub
    Private Sub txt���ʍĐU�֓��R_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʍĐU�֓��R.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʍĐU�֓��R)

    End Sub
    Private Sub txt���ʍĐU�֓��S_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʍĐU�֓��S.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʍĐU�֓��S)

    End Sub
    Private Sub txt���ʍĐU�֓��T_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʍĐU�֓��T.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʍĐU�֓��T)

    End Sub
    Private Sub txt���ʍĐU�֓��U_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt���ʍĐU�֓��U.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt���ʍĐU�֓��U)

    End Sub
    '�����X�P�W���[��
    Private Sub txt�����U�֌��P_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�����U�֌��P.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�����U�֌��P)

    End Sub
    Private Sub txt�����U�֌��Q_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�����U�֌��Q.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�����U�֌��Q)

    End Sub
    Private Sub txt�����U�֌��R_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�����U�֌��R.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�����U�֌��R)

    End Sub
    Private Sub txt�����U�֌��S_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�����U�֌��S.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�����U�֌��S)

    End Sub
    Private Sub txt�����U�֌��T_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�����U�֌��T.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�����U�֌��T)

    End Sub
    Private Sub txt�����U�֌��U_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�����U�֌��U.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�����U�֌��U)

    End Sub
    Private Sub txt�����U�֓��P_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�����U�֓��P.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�����U�֓��P)

    End Sub
    Private Sub txt�����U�֓��Q_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�����U�֓��Q.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�����U�֓��Q)

    End Sub
    Private Sub txt�����U�֓��R_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�����U�֓��R.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�����U�֓��R)

    End Sub
    Private Sub txt�����U�֓��S_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�����U�֓��S.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�����U�֓��S)

    End Sub
    Private Sub txt�����U�֓��T_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�����U�֓��T.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�����U�֓��T)

    End Sub
    Private Sub txt�����U�֓��U_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt�����U�֓��U.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt�����U�֓��U)

    End Sub
#End Region

#Region " CheckedChanged(CheckBox) "
    '���ʃX�P�W���[��
    Private Sub chk�P_�S�w�N_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk�P_�S�w�N.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk�P_�S�w�N, _
                                           chk�P_�P�w�N, _
                                           chk�P_�Q�w�N, _
                                           chk�P_�R�w�N, _
                                           chk�P_�S�w�N, _
                                           chk�P_�T�w�N, _
                                           chk�P_�U�w�N, _
                                           chk�P_�V�w�N, _
                                           chk�P_�W�w�N, _
                                           chk�P_�X�w�N)

    End Sub
    Private Sub chk�Q_�S�w�N_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�Q_�S�w�N.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk�Q_�S�w�N, _
                                           chk�Q_�P�w�N, _
                                           chk�Q_�Q�w�N, _
                                           chk�Q_�R�w�N, _
                                           chk�Q_�S�w�N, _
                                           chk�Q_�T�w�N, _
                                           chk�Q_�U�w�N, _
                                           chk�Q_�V�w�N, _
                                           chk�Q_�W�w�N, _
                                           chk�Q_�X�w�N)

    End Sub
    Private Sub chk�R_�S�w�N_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�R_�S�w�N.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk�R_�S�w�N, _
                                           chk�R_�P�w�N, _
                                           chk�R_�Q�w�N, _
                                           chk�R_�R�w�N, _
                                           chk�R_�S�w�N, _
                                           chk�R_�T�w�N, _
                                           chk�R_�U�w�N, _
                                           chk�R_�V�w�N, _
                                           chk�R_�W�w�N, _
                                           chk�R_�X�w�N)

    End Sub
    Private Sub chk�S_�S�w�N_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�S_�S�w�N.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk�S_�S�w�N, _
                                           chk�S_�P�w�N, _
                                           chk�S_�Q�w�N, _
                                           chk�S_�R�w�N, _
                                           chk�S_�S�w�N, _
                                           chk�S_�T�w�N, _
                                           chk�S_�U�w�N, _
                                           chk�S_�V�w�N, _
                                           chk�S_�W�w�N, _
                                           chk�S_�X�w�N)

    End Sub
    Private Sub chk�T_�S�w�N_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�T_�S�w�N.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk�T_�S�w�N, _
                                           chk�T_�P�w�N, _
                                           chk�T_�Q�w�N, _
                                           chk�T_�R�w�N, _
                                           chk�T_�S�w�N, _
                                           chk�T_�T�w�N, _
                                           chk�T_�U�w�N, _
                                           chk�T_�V�w�N, _
                                           chk�T_�W�w�N, _
                                           chk�T_�X�w�N)

    End Sub
    Private Sub chk�U_�S�w�N_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�U_�S�w�N.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk�U_�S�w�N, _
                                           chk�U_�P�w�N, _
                                           chk�U_�Q�w�N, _
                                           chk�U_�R�w�N, _
                                           chk�U_�S�w�N, _
                                           chk�U_�T�w�N, _
                                           chk�U_�U�w�N, _
                                           chk�U_�V�w�N, _
                                           chk�U_�W�w�N, _
                                           chk�U_�X�w�N)

    End Sub
    '�����X�P�W���[��
    Private Sub chk�����P_�S�w�N_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk�����P_�S�w�N.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk�����P_�S�w�N, _
                                           chk�����P_�P�w�N, _
                                           chk�����P_�Q�w�N, _
                                           chk�����P_�R�w�N, _
                                           chk�����P_�S�w�N, _
                                           chk�����P_�T�w�N, _
                                           chk�����P_�U�w�N, _
                                           chk�����P_�V�w�N, _
                                           chk�����P_�W�w�N, _
                                           chk�����P_�X�w�N)

    End Sub
    Private Sub chk�����Q_�S�w�N_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk�����Q_�S�w�N.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk�����Q_�S�w�N, _
                                           chk�����Q_�P�w�N, _
                                           chk�����Q_�Q�w�N, _
                                           chk�����Q_�R�w�N, _
                                           chk�����Q_�S�w�N, _
                                           chk�����Q_�T�w�N, _
                                           chk�����Q_�U�w�N, _
                                           chk�����Q_�V�w�N, _
                                           chk�����Q_�W�w�N, _
                                           chk�����Q_�X�w�N)

    End Sub
    Private Sub chk�����R_�S�w�N_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk�����R_�S�w�N.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk�����R_�S�w�N, _
                                           chk�����R_�P�w�N, _
                                           chk�����R_�Q�w�N, _
                                           chk�����R_�R�w�N, _
                                           chk�����R_�S�w�N, _
                                           chk�����R_�T�w�N, _
                                           chk�����R_�U�w�N, _
                                           chk�����R_�V�w�N, _
                                           chk�����R_�W�w�N, _
                                           chk�����R_�X�w�N)

    End Sub
    Private Sub chk�����S_�S�w�N_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk�����S_�S�w�N.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk�����S_�S�w�N, _
                                           chk�����S_�P�w�N, _
                                           chk�����S_�Q�w�N, _
                                           chk�����S_�R�w�N, _
                                           chk�����S_�S�w�N, _
                                           chk�����S_�T�w�N, _
                                           chk�����S_�U�w�N, _
                                           chk�����S_�V�w�N, _
                                           chk�����S_�W�w�N, _
                                           chk�����S_�X�w�N)

    End Sub
    Private Sub chk�����T_�S�w�N_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk�����T_�S�w�N.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk�����T_�S�w�N, _
                                           chk�����T_�P�w�N, _
                                           chk�����T_�Q�w�N, _
                                           chk�����T_�R�w�N, _
                                           chk�����T_�S�w�N, _
                                           chk�����T_�T�w�N, _
                                           chk�����T_�U�w�N, _
                                           chk�����T_�V�w�N, _
                                           chk�����T_�W�w�N, _
                                           chk�����T_�X�w�N)

    End Sub
    Private Sub chk�����U_�S�w�N_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk�����U_�S�w�N.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk�����U_�S�w�N, _
                                           chk�����U_�P�w�N, _
                                           chk�����U_�Q�w�N, _
                                           chk�����U_�R�w�N, _
                                           chk�����U_�S�w�N, _
                                           chk�����U_�T�w�N, _
                                           chk�����U_�U�w�N, _
                                           chk�����U_�V�w�N, _
                                           chk�����U_�W�w�N, _
                                           chk�����U_�X�w�N)

    End Sub
#End Region

#Region " CheckedChanged "

    Private Sub chk�S���U�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�S���U�֓�.CheckedChanged
        '2006/11/22�@���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If chk�S���U�֓�.Checked = False Then
            chk�S���ĐU�֓�.Checked = False
        End If
    End Sub

    Private Sub chk�S���ĐU�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�S���ĐU�֓�.CheckedChanged
        '2006/11/22�@�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If chk�S���ĐU�֓�.Checked = True Then
            chk�S���U�֓�.Checked = True
        End If
    End Sub

    Private Sub chk�T���U�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�T���U�֓�.CheckedChanged
        '2006/11/22�@���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If chk�T���U�֓�.Checked = False Then
            chk�T���ĐU�֓�.Checked = False
        End If
    End Sub

    Private Sub chk�T���ĐU�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�T���ĐU�֓�.CheckedChanged
        '2006/11/22�@�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If chk�T���ĐU�֓�.Checked = True Then
            chk�T���U�֓�.Checked = True
        End If
    End Sub

    Private Sub chk�U���U�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�U���U�֓�.CheckedChanged
        '2006/11/22�@���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If chk�U���U�֓�.Checked = False Then
            chk�U���ĐU�֓�.Checked = False
        End If
    End Sub

    Private Sub chk�U���ĐU�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�U���ĐU�֓�.CheckedChanged
        '2006/11/22�@�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If chk�U���ĐU�֓�.Checked = True Then
            chk�U���U�֓�.Checked = True
        End If
    End Sub

    Private Sub chk�V���U�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�V���U�֓�.CheckedChanged
        '2006/11/22�@���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If chk�V���U�֓�.Checked = False Then
            chk�V���ĐU�֓�.Checked = False
        End If
    End Sub

    Private Sub chk�V���ĐU�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�V���ĐU�֓�.CheckedChanged
        '2006/11/22�@�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If chk�V���ĐU�֓�.Checked = True Then
            chk�V���U�֓�.Checked = True
        End If
    End Sub

    Private Sub chk�W���U�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�W���U�֓�.CheckedChanged
        '2006/11/22�@���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If chk�W���U�֓�.Checked = False Then
            chk�W���ĐU�֓�.Checked = False
        End If
    End Sub

    Private Sub chk�W���ĐU�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�W���ĐU�֓�.CheckedChanged
        '2006/11/22�@�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If chk�W���ĐU�֓�.Checked = True Then
            chk�W���U�֓�.Checked = True
        End If
    End Sub

    Private Sub chk�X���U�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�X���U�֓�.CheckedChanged
        '2006/11/22�@���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If chk�X���U�֓�.Checked = False Then
            chk�X���ĐU�֓�.Checked = False
        End If
    End Sub

    Private Sub chk�X���ĐU�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�X���ĐU�֓�.CheckedChanged
        '2006/11/22�@�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If chk�X���ĐU�֓�.Checked = True Then
            chk�X���U�֓�.Checked = True
        End If
    End Sub

    Private Sub chk�P�O���U�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�P�O���U�֓�.CheckedChanged
        '2006/11/22�@���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If chk�P�O���U�֓�.Checked = False Then
            chk�P�O���ĐU�֓�.Checked = False
        End If
    End Sub

    Private Sub chk�P�O���ĐU�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�P�O���ĐU�֓�.CheckedChanged
        '2006/11/22�@�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If chk�P�O���ĐU�֓�.Checked = True Then
            chk�P�O���U�֓�.Checked = True
        End If
    End Sub

    Private Sub chk�P�P���U�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�P�P���U�֓�.CheckedChanged
        '2006/11/22�@���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If chk�P�P���U�֓�.Checked = False Then
            chk�P�P���ĐU�֓�.Checked = False
        End If
    End Sub

    Private Sub chk�P�P���ĐU�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�P�P���ĐU�֓�.CheckedChanged
        '2006/11/22�@�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If chk�P�P���ĐU�֓�.Checked = True Then
            chk�P�P���U�֓�.Checked = True
        End If
    End Sub

    Private Sub chk�P�Q���U�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�P�Q���U�֓�.CheckedChanged
        '2006/11/22�@���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If chk�P�Q���U�֓�.Checked = False Then
            chk�P�Q���ĐU�֓�.Checked = False
        End If
    End Sub

    Private Sub chk�P�Q���ĐU�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�P�Q���ĐU�֓�.CheckedChanged
        '2006/11/22�@�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If chk�P�Q���ĐU�֓�.Checked = True Then
            chk�P�Q���U�֓�.Checked = True
        End If
    End Sub

    Private Sub chk�P���U�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�P���U�֓�.CheckedChanged
        '2006/11/22�@���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If chk�P���U�֓�.Checked = False Then
            chk�P���ĐU�֓�.Checked = False
        End If
    End Sub

    Private Sub chk�P���ĐU�֓�_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�P���ĐU�֓�.CheckStateChanged
        '2006/11/22�@�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If chk�P���ĐU�֓�.Checked = True Then
            chk�P���U�֓�.Checked = True
        End If
    End Sub

    Private Sub chk�Q���U�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�Q���U�֓�.CheckedChanged
        '2006/11/22�@���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If chk�Q���U�֓�.Checked = False Then
            chk�Q���ĐU�֓�.Checked = False
        End If
    End Sub

    Private Sub chk�Q���ĐU�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�Q���ĐU�֓�.CheckedChanged
        '2006/11/22�@�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If chk�Q���ĐU�֓�.Checked = True Then
            chk�Q���U�֓�.Checked = True
        End If
    End Sub

    Private Sub chk�R���U�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�R���U�֓�.CheckedChanged
        '2006/11/22�@���U�`�F�b�N���O�����Ƃ��A�ĐU�`�F�b�N���O���i�ĐU�݂̂̓o�^��h�����߁j
        If chk�R���U�֓�.Checked = False Then
            chk�R���ĐU�֓�.Checked = False
        End If
    End Sub

    Private Sub chk�R���ĐU�֓�_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk�R���ĐU�֓�.CheckedChanged
        '2006/11/22�@�ĐU�`�F�b�N����ꂽ�Ƃ��A���U�`�F�b�N�������i�ĐU�݂̂̓o�^��h�����߁j
        If chk�R���ĐU�֓�.Checked = True Then
            chk�R���U�֓�.Checked = True
        End If
    End Sub

#End Region

#Region " Private Sub(����)"
    Private Sub PSUB_FORMAT_ALL()

        '��{��񕔏�����
        Call PSUB_KIHON_FORMAT()

        '�N�ԃX�P�W���[����ʏ�����
        Call PSUB_NENKAN_FORMAT()

        '���ʃX�P�W���[����ʏ�����
        Call PSUB_TOKUBETU_FORMAT()

        '�����X�P�W���[����ʏ�����
        Call PSUB_ZUIJI_FORMAT()

        '�{�^��Enabled�������
        Call PSUB_BUTTON_Enable()

    End Sub

    Private Sub PSUB_BUTTON_Enable(Optional ByVal pIndex As Integer = 0)

        Select Case pIndex
            Case 0
                btnAction.Enabled = True
                btnFind.Enabled = True
                btnUpdate.Enabled = False
                btnEraser.Enabled = True
                txtGAKKOU_CODE.Enabled = True
                cmbGakkouName.Enabled = True
                cmbKana.Enabled = True
                txt�Ώ۔N�x.Enabled = True
            Case 1
                btnAction.Enabled = False
                btnFind.Enabled = True
                btnUpdate.Enabled = True
                btnEraser.Enabled = True
                txtGAKKOU_CODE.Enabled = False
                cmbGakkouName.Enabled = False
                cmbKana.Enabled = False
                txt�Ώ۔N�x.Enabled = False
            Case 2
                btnAction.Enabled = False '2007/02/15
                btnFind.Enabled = True
                btnUpdate.Enabled = False
                btnEraser.Enabled = True
                txtGAKKOU_CODE.Enabled = True
                cmbGakkouName.Enabled = True
                cmbKana.Enabled = True
                txt�Ώ۔N�x.Enabled = True
        End Select

    End Sub

    Private Sub PSUB_KIHON_FORMAT()

        txt�Ώ۔N�x.Enabled = True
        'txt�Ώ۔N�x.Text = ""

        txtGAKKOU_CODE.Enabled = True
        txtGAKKOU_CODE.Text = ""

        lab�w�Z��.Text = ""

        '�x�����X�g�{�b�N�X������
        lst�x��.Items.Clear()

        '�w�Z�����i�J�i�j
        cmbKana.SelectedIndex = -1

        '�ǉ� 2007/02/15
        '�w�Z�R���{�ݒ�i�S�w�Z�j
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�w�Z�R���{�ݒ�", "���s", "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")
            MessageBox.Show("�w�Z���R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        '�w�Z�����i�w�Z���j
        cmbGakkouName.SelectedIndex = -1

    End Sub

    '========================================
    '�X�P�W���[���}�X�^�o�^
    '2006/11/30�@�w�N�t���O��ύX
    '========================================
    Private Function PSUB_INSERT_G_SCHMAST_SQL() As String

        Dim sql As String = ""

        '�e��\����̎Z�o
        Dim CLS As New GAKKOU.ClsSchduleMaintenanceClass
        Call CLS.SetKyuzituInformation()

        CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

        '�X�P�W���[���쐬�Ώۂ̎����R�[�h�𒊏o
        CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(STR�U�֓�), strGakkouCode, "01")

        CLS.SCH.FURI_DATE = GCom.SET_DATE(STR�U�֓�)
        If CLS.SCH.FURI_DATE = "00000000" Then
        Else
            CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
        End If

        Dim strFURI_DATE As String = CLS.SCH.FURI_DATE '�f�o�b�O�p�H

        '2010/10/21 �_��U�֓��Ή� ��������
        If STR�_��U�֓� = "" OrElse STR�_��U�֓�.Length <> 8 Then
            '�������Ȃ��ꍇ�͎��U�֓���ݒ�
            CLS.SCH.KFURI_DATE = CLS.SCH.FURI_DATE
        Else
            CLS.SCH.KFURI_DATE = STR�_��U�֓�
        End If
        '2010/10/21 �_��U�֓��Ή� �����܂�

        Dim BRet As Boolean = CLS.INSERT_NEW_SCHMAST(0, False, True)

        Dim ENTRY_Y_DATE As String = "00000000"                                                   '���׍쐬�\����Z�o
        Dim CHECK_Y_DATE As String = fn_GetEigyoubi(STR�U�֓�, STR_JIFURI_CHECK, "-")           '�`�F�b�N�\����Z�o
        Dim DATA_Y_DATE As String = fn_GetEigyoubi(STR�U�֓�, STR_JIFURI_HAISIN, "-")       '�U�փf�[�^�쐬�\����Z�o
        Dim FUNOU_Y_DATE As String = CLS.SCH.FUNOU_YDATE                                          '�s�\���ʍX�V�\����Z�o
        Dim KESSAI_Y_DATE As String = CLS.SCH.KESSAI_YDATE                                        '�������ϗ\����Z�o
        Dim HENKAN_Y_DATE As String = CLS.SCH.HENKAN_YDATE                                          '�Ԋҗ\���

        'INSERT���쐬
        sql += "INSERT INTO G_SCHMAST "
        sql += " VALUES ( "
        '�w�Z�R�[�h
        sql += "'" & GAKKOU_INFO.GAKKOU_CODE & "'"
        '�����N��
        sql += ",'" & STR�����N�� & "'"
        '�X�P�W���[���敪
        sql += ",'" & STR�X�P�敪 & "'"
        '�U�֋敪
        sql += ",'" & STR�U�֋敪 & "'"
        '�U�֓�
        sql += ",'" & STR�U�֓� & "'"
        '�ĐU�֓�
        sql += ",'" & STR�ĐU�֓� & "'"
        '�w�N�P
        sql += ",'" & STR�P�w�N & "'"
        '�w�N�Q
        sql += ",'" & STR�Q�w�N & "'"
        '�w�N�R
        sql += ",'" & STR�R�w�N & "'"
        '�w�N�S
        sql += ",'" & STR�S�w�N & "'"
        '�w�N�T
        sql += ",'" & STR�T�w�N & "'"
        '�w�N�U
        sql += ",'" & STR�U�w�N & "'"
        '�w�N�V
        sql += ",'" & STR�V�w�N & "'"
        '�w�N�W
        sql += ",'" & STR�W�w�N & "'"
        '�w�N�X
        sql += ",'" & STR�X�w�N & "'"
        '�ϑ��҃R�[�h
       
        '2011/06/16 �W���ŏC�� �ϑ��҃R�[�h�̉��P���ύX���s��Ȃ�------------------START
        sql += ",'" & GAKKOU_INFO.ITAKU_CODE & "'"
        'Select Case STR�U�֋敪
        '    Case "0"
        '        sql += ",'" & "0" + GAKKOU_INFO.ITAKU_CODE.Substring(1, 9) & "'"
        '    Case "1"
        '        sql += ",'" & "1" + GAKKOU_INFO.ITAKU_CODE.Substring(1, 9) & "'"
        '    Case "2"
        '        sql += ",'" & "2" + GAKKOU_INFO.ITAKU_CODE.Substring(1, 9) & "'"
        '    Case "3"
        '        sql += ",'" & "3" + GAKKOU_INFO.ITAKU_CODE.Substring(1, 9) & "'"
        'End Select
        '2011/06/16 �W���ŏC�� �ϑ��҃R�[�h�̉��P���ύX���s��Ȃ�------------------END
        '�戵���Z�@��
        sql += ",'" & GAKKOU_INFO.TKIN_CODE & "'"
        '�戵�x�X
        sql += ",'" & GAKKOU_INFO.TSIT_CODE & "'"
        '�}�̃R�[�h 
        sql += ",'" & GAKKOU_INFO.BAITAI_CODE & "'"
        '�萔���敪 
        sql += ",'" & GAKKOU_INFO.TESUUTYO_KBN & "'"
        '���׍쐬�\���
        sql += "," & "'" & ENTRY_Y_DATE & "'"
        '���׍쐬��
        sql += "," & "'00000000'"
        '�`�F�b�N�\���
        sql += "," & "'" & CHECK_Y_DATE & "'"
        '�`�F�b�N��
        sql += "," & "'00000000'"
        '�U�փf�[�^�쐬�\���
        sql += "," & "'" & DATA_Y_DATE & "'"
        '�U�փf�[�^�쐬��
        sql += "," & "'00000000'"
        '�s�\���ʍX�V�\���
        sql += "," & "'" & FUNOU_Y_DATE & "'"
        '�s�\���ʍX�V��
        sql += "," & "'00000000'"
        '�Ԋҗ\���
        sql += "," & "'" & HENKAN_Y_DATE & "'"
        '�Ԋғ�
        sql += "," & "'00000000'"
        '���ϗ\���
        sql += "," & "'" & KESSAI_Y_DATE & "'"
        '���ϓ�
        sql += "," & "'00000000'"
        '���׍쐬�σt���O
        sql += "," & "'" & strENTRI_FLG & "'"
        '���z�m�F�σt���O
        sql += "," & "'" & strCHECK_FLG & "'"
        '�U�փf�[�^�쐬�σt���O
        sql += "," & "'" & strDATA_FLG & "'"
        '�s�\���ʍX�V�σt���O
        sql += "," & "'" & strFUNOU_FLG & "'"
        '�Ԋҍσt���O
        sql += "," & "'" & strHENKAN_FLG & "'"
        '�ĐU�f�[�^�쐬�σt���O
        sql += "," & "'" & strSAIFURI_FLG & "'"
        '���ύσt���O
        sql += "," & "'" & strKESSAI_FLG & "'"
        '���f�t���O
        sql += "," & "'" & strTYUUDAN_FLG & "'"
        '��������
        sql += "," & lngSYORI_KEN
        '�������z
        sql += "," & dblSYORI_KIN
        '�萔��
        sql += "," & 0
        '�萔���P
        sql += "," & 0
        '�萔���Q
        sql += "," & 0
        '�萔���R
        sql += "," & 0
        '�U�֍ό���
        sql += "," & lngFURI_KEN
        '�U�֍ϋ��z
        sql += "," & dblFURI_KIN
        '�s�\����
        sql += "," & lngFUNOU_KEN
        '�s�\���z
        sql += "," & dblFUNOU_KIN
        '�쐬���t
        sql += "," & "'" & Str_SyoriDate(0) & "'"
        '�^�C���X�^���v
        sql += "," & "'" & Str_SyoriDate(1) & "'"
        '�\���P
        sql += "," & "'" & STR�N�ԓ��͐U�֓� & "'"
        '�\���Q
        sql += "," & "'" & Space(30) & "'"
        '�\���R
        sql += "," & "'" & Space(30) & "'"
        '�\���S
        sql += "," & "'" & Space(30) & "'"
        '�\���T
        sql += "," & "'" & Space(30) & "'"
        '�\���U
        sql += "," & "'" & Space(30) & "'"
        '�\���V
        sql += "," & "'" & Space(30) & "'"
        '�\���W
        sql += "," & "'" & Space(30) & "'"
        '�\���X
        sql += "," & "'" & Space(30) & "'"
        '�\���P�O
        sql += "," & "'" & Space(30) & "')"

        Return sql

    End Function

    '===================================================
    'PSUB_UPDATE_G_SCHMAST_SQL
    'UPDATE 2006/11/30�@�N�ԁE���ʁE�������ꂼ��ɑΉ�
    '===================================================
    Private Function PSUB_UPDATE_G_SCHMAST_SQL(ByVal strJoken_Furi_Date As String, ByVal strJoken_SFuri_Date As String) As String
        'strJoken_Furi_Date �F�X�V�O�U�֓�
        'strJoken_SFuri_Date�F�X�V�O�ĐU��

        Dim sql As String = ""

        '�X�V�O�ĐU�����󔒂̏ꍇ��0���߂���
        If Trim(strJoken_SFuri_Date) = "" Then
            strJoken_SFuri_Date = "00000000"
        End If

        '�e��\����̎Z�o
        Dim CLS As New GAKKOU.ClsSchduleMaintenanceClass
        Call CLS.SetKyuzituInformation()

        CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

        '�X�P�W���[���쐬�Ώۂ̎����R�[�h�𒊏o
        CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(STR�U�֓�), strGakkouCode, "01")

        CLS.SCH.FURI_DATE = GCom.SET_DATE(STR�U�֓�)
        If CLS.SCH.FURI_DATE = "00000000" Then
        Else
            CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
        End If

        Dim strFURI_DATE As String = CLS.SCH.FURI_DATE '�f�o�b�O�p�H

        '2010/10/21 �_��U�֓��Ή� ��������
        If STR�_��U�֓� = "" OrElse STR�_��U�֓�.Length <> 8 Then
            '�������Ȃ��ꍇ�͎��U�֓���ݒ�
            CLS.SCH.KFURI_DATE = CLS.SCH.FURI_DATE
        Else
            CLS.SCH.KFURI_DATE = STR�_��U�֓�
        End If
        '2010/10/21 �_��U�֓��Ή� �����܂�

        Dim BRet As Boolean = CLS.INSERT_NEW_SCHMAST(0, False, True)

        Dim ENTRY_Y_DATE As String = "00000000"                                                   '���׍쐬�\����Z�o
        Dim CHECK_Y_DATE As String = fn_GetEigyoubi(STR�U�֓�, STR_JIFURI_CHECK, "-")           '�`�F�b�N�\����Z�o
        Dim DATA_Y_DATE As String = fn_GetEigyoubi(STR�U�֓�, STR_JIFURI_HAISIN, "-")       '�U�փf�[�^�쐬�\����Z�o
        Dim FUNOU_Y_DATE As String = CLS.SCH.FUNOU_YDATE                                          '�s�\���ʍX�V�\����Z�o
        Dim KESSAI_Y_DATE As String = CLS.SCH.KESSAI_YDATE                                        '�������ϗ\����Z�o
        Dim HENKAN_Y_DATE As String = CLS.SCH.HENKAN_YDATE                                          '�Ԋҗ\���

        'UPDATE���쐬
        sql = " UPDATE  G_SCHMAST"
        sql += " SET "
        sql += " FURI_DATE_S = '" & STR�U�֓� & "'," '   2006/11/22�@�U�֓�
        sql += " SFURI_DATE_S = '" & STR�ĐU�֓� & "'," '2006/11/22�@�ĐU��
        sql += " GAKUNEN1_FLG_S  ='" & STR�P�w�N & "',"
        sql += " GAKUNEN2_FLG_S  ='" & STR�Q�w�N & "',"
        sql += " GAKUNEN3_FLG_S  ='" & STR�R�w�N & "',"
        sql += " GAKUNEN4_FLG_S  ='" & STR�S�w�N & "',"
        sql += " GAKUNEN5_FLG_S  ='" & STR�T�w�N & "',"
        sql += " GAKUNEN6_FLG_S  ='" & STR�U�w�N & "',"
        sql += " GAKUNEN7_FLG_S  ='" & STR�V�w�N & "',"
        sql += " GAKUNEN8_FLG_S  ='" & STR�W�w�N & "',"
        sql += " GAKUNEN9_FLG_S  ='" & STR�X�w�N & "',"
        sql += " SYORI_KEN_S =" & lngSYORI_KEN & ","
        sql += " SYORI_KIN_S =" & dblSYORI_KIN & ","
        sql += " FURI_KEN_S =" & lngFURI_KEN & ","
        sql += " FURI_KIN_S =" & dblFURI_KIN & ","
        sql += " FUNOU_KEN_S =" & lngFUNOU_KEN & ","
        sql += " FUNOU_KIN_S =" & dblFUNOU_KIN & ","
        sql += " YOBI1_S = '" & STR�N�ԓ��͐U�֓� & "',"
        '�e�\����X�V 2007/12/13
        sql += " ENTRI_YDATE_S ='" & ENTRY_Y_DATE & "',"
        sql += " CHECK_YDATE_S ='" & CHECK_Y_DATE & "',"
        sql += " DATA_YDATE_S ='" & DATA_Y_DATE & "',"
        sql += " FUNOU_YDATE_S ='" & FUNOU_Y_DATE & "',"
        sql += " HENKAN_YDATE_S ='" & HENKAN_Y_DATE & "',"
        sql += " KESSAI_YDATE_S ='" & KESSAI_Y_DATE & "'"
        sql += " WHERE"
        sql += " GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'"
        sql += " AND"
        sql += " NENGETUDO_S ='" & STR�����N�� & "'"
        sql += " AND"
        sql += " SCH_KBN_S ='" & STR�X�P�敪 & "'"
        sql += " AND"
        sql += " FURI_KBN_S ='" & STR�U�֋敪 & "'"
        sql += " AND"

        '2006/11/22�@���������f�[�^�ɏC��
        'sql += " FURI_DATE_S ='" & STR�U�֓� & "'"
        'sql += " FURI_DATE_S ='" & str���U�֓�(int���U�ւh�c) & "'"
        sql += " FURI_DATE_S = '" & strJoken_Furi_Date & "'"
        sql += " AND"
        'sql += " SFURI_DATE_S ='" & STR�ĐU�֓� & "'"
        'sql += " SFURI_DATE_S ='" & str���ĐU��(int���U�ւh�c) & "'"
        sql += " SFURI_DATE_S = '" & strJoken_SFuri_Date & "'"

        Return sql

    End Function

    Private Sub PSUB_ZENGAKUNEN_CHKBOX_CNTROL(ByVal chkBOXALL As CheckBox, ByVal chkBOX1 As CheckBox, ByVal chkBOX2 As CheckBox, ByVal chkBOX3 As CheckBox, ByVal chkBOX4 As CheckBox, ByVal chkBOX5 As CheckBox, ByVal chkBOX6 As CheckBox, ByVal chkBOX7 As CheckBox, ByVal chkBOX8 As CheckBox, ByVal chkBOX9 As CheckBox)

        If chkBOXALL.Checked = True Then
            '�e�w�N�̃`�F�b�N������
            chkBOX1.Checked = False
            chkBOX2.Checked = False
            chkBOX3.Checked = False
            chkBOX4.Checked = False
            chkBOX5.Checked = False
            chkBOX6.Checked = False
            chkBOX7.Checked = False
            chkBOX8.Checked = False
            chkBOX9.Checked = False
            '�e�w�N�̃`�F�b�N�{�b�N�X�g�p�s�� 
            chkBOX1.Enabled = False
            chkBOX2.Enabled = False
            chkBOX3.Enabled = False
            chkBOX4.Enabled = False
            chkBOX5.Enabled = False
            chkBOX6.Enabled = False
            chkBOX7.Enabled = False
            chkBOX8.Enabled = False
            chkBOX9.Enabled = False
        Else
            '�e�w�N�̃`�F�b�N�{�b�N�X�g�p�� 
            chkBOX1.Enabled = True
            chkBOX2.Enabled = True
            chkBOX3.Enabled = True
            chkBOX4.Enabled = True
            chkBOX5.Enabled = True
            chkBOX6.Enabled = True
            chkBOX7.Enabled = True
            chkBOX8.Enabled = True
            chkBOX9.Enabled = True
            '2006/10/12�@�ō��w�N�`�F�b�N
            PSUB_TGAKUNEN_CHK()
            PSUB_ZGAKUNEN_CHK()
        End If
    End Sub

    Private Sub PSUB_SANSYOU_FOCUS()

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & Trim(txtGAKKOU_CODE.Text) & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & Trim(txt�Ώ۔N�x.Text) & "04'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & CStr(CInt(Trim(txt�Ώ۔N�x.Text)) + 1) & "03'")

        If oraReader.DataReader(sql) = True Then
            btnAction.Enabled = False
            btnFind.Enabled = True
            btnFind.Focus()
        Else '�ǉ� 2007/02/15
            btnFind.Enabled = False
            btnAction.Enabled = True
            btnAction.Focus()
        End If

        oraReader.Close()

    End Sub
    '2006/10/25 �ϐ��N���A
    Public Sub sb_HENSU_CLEAR()
        strENTRI_FLG = "0"
        strCHECK_FLG = "0"
        strDATA_FLG = "0"
        strFUNOU_FLG = "0"
        strSAIFURI_FLG = "0"
        strKESSAI_FLG = "0"
        strTYUUDAN_FLG = "0"
        strENTRI_FLG_SAI = "0"
        strCHECK_FLG_SAI = "0"
        strDATA_FLG_SAI = "0"
        strFUNOU_FLG_SAI = "0"
        strSAIFURI_FLG_SAI = "0"
        strKESSAI_FLG_SAI = "0"
        strTYUUDAN_FLG_SAI = "0"

        strSAIFURI_DEF = "00000000" '�ʏ�X�P�W���[���̍ĐU��

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0
    End Sub

    '==========================================
    '�ύX���ꂽ���ڂ��`�F�b�N  2006/11/30
    '==========================================
    Private Sub PSUB_Kousin_Check()

        '--------------------------------------
        '�e���̒l���\���̂ɓ��́i�X�V���̂��́j
        '--------------------------------------
        Call PSUB_NENKAN_GET(NENKAN_SCHINFO)
        Call PSUB_TOKUBETU_GET(TOKUBETU_SCHINFO)
        Call PSUB_ZUIJI_GET(ZUIJI_SCHINFO)

        '�Q�Ǝ��ƍX�V���̍��ڂ��ׁA�ύX�����������̂̍X�V�t���O�𗧂Ă�

        For i As Integer = 1 To 12
            '--------------------------------------
            '�N�ԃX�P�W���[���`�F�b�N
            '--------------------------------------
            If NENKAN_SCHINFO(i).Furikae_Check = SYOKI_NENKAN_SCHINFO(i).Furikae_Check And _
               NENKAN_SCHINFO(i).Furikae_Date = SYOKI_NENKAN_SCHINFO(i).Furikae_Date And _
               NENKAN_SCHINFO(i).Furikae_Day = SYOKI_NENKAN_SCHINFO(i).Furikae_Day And _
               NENKAN_SCHINFO(i).Furikae_Enabled = SYOKI_NENKAN_SCHINFO(i).Furikae_Enabled And _
               NENKAN_SCHINFO(i).SaiFurikae_Check = SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Check And _
               NENKAN_SCHINFO(i).SaiFurikae_Date = SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Date And _
               NENKAN_SCHINFO(i).SaiFurikae_Day = SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day And _
               NENKAN_SCHINFO(i).SaiFurikae_Enabled = SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Enabled Then

                bln�N�ԍX�V(i) = False '�ύX�Ȃ�
            Else
                bln�N�ԍX�V(i) = True ' �ύX����
            End If
        Next

        For i As Integer = 1 To 6
            '--------------------------------------
            '���ʃX�P�W���[���`�F�b�N
            '--------------------------------------
            '2006/12/12�@�ꕔ�ǉ��F���͂��s�����Ă����ꍇ�A�X�V���Ȃ�
            If (TOKUBETU_SCHINFO(i).Seikyu_Tuki = SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki And _
               TOKUBETU_SCHINFO(i).Furikae_Tuki = SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki And _
               TOKUBETU_SCHINFO(i).Furikae_Date = SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date And _
               TOKUBETU_SCHINFO(i).SaiFurikae_Tuki = SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki And _
               TOKUBETU_SCHINFO(i).SaiFurikae_Date = SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date And _
               TOKUBETU_SCHINFO(i).SiyouGakunen1_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen1_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen2_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen2_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen3_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen3_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen4_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen4_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen5_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen5_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen6_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen6_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen7_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen7_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen8_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen8_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen9_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen9_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check) Or _
               ((TOKUBETU_SCHINFO(i).Furikae_Tuki = "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "") Or _
               (TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date = "")) Or _
               ((TOKUBETU_SCHINFO(i).SaiFurikae_Tuki = "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "") Or _
               (TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date = "")) Then

                bln���ʍX�V(i) = False '�ύX�Ȃ�
            Else
                bln���ʍX�V(i) = True ' �ύX����
            End If

            '--------------------------------------
            '�����X�P�W���[���`�F�b�N
            '--------------------------------------
            '2006/12/12�@�ꕔ�ǉ��F���͂��s�����Ă����ꍇ�A�X�V���Ȃ�
            If (ZUIJI_SCHINFO(i).Furikae_Tuki = SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki And _
               ZUIJI_SCHINFO(i).Furikae_Date = SYOKI_ZUIJI_SCHINFO(i).Furikae_Date And _
               ZUIJI_SCHINFO(i).Nyusyutu_Kbn = SYOKI_ZUIJI_SCHINFO(i).Nyusyutu_Kbn And _
               ZUIJI_SCHINFO(i).SiyouGakunen1_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen1_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen2_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen2_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen3_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen3_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen4_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen4_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen5_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen5_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen6_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen6_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen7_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen7_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen8_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen8_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen9_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen9_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunenALL_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunenALL_Check) Or _
               ((ZUIJI_SCHINFO(i).Furikae_Tuki = "" And ZUIJI_SCHINFO(i).Furikae_Date <> "") Or _
               (ZUIJI_SCHINFO(i).Furikae_Tuki <> "" And ZUIJI_SCHINFO(i).Furikae_Date = "")) Then

                bln�����X�V(i) = False '�ύX�Ȃ�
            Else
                bln�����X�V(i) = True ' �ύX����
            End If
        Next

    End Sub

    '��ʕ\�����ޔ��@2006/12/04
    Public Sub sb_SANSYOU_SET()
        '�N�ԏ��U
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P���U�֓�.Text.Trim = "" Then
        If lab�P���U�֓�.Text.Trim = "" Or chk�P���U�֓�.Checked = False Then
            strSYOFURI_NENKAN(1) = ""
        Else
            strSYOFURI_NENKAN(1) = Replace(lab�P���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�Q���U�֓�.Text.Trim = "" Then
        If lab�Q���U�֓�.Text.Trim = "" Or chk�Q���U�֓�.Checked = False Then
            strSYOFURI_NENKAN(2) = ""
        Else
            strSYOFURI_NENKAN(2) = Replace(lab�Q���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�R���U�֓�.Text.Trim = "" Then
        If lab�R���U�֓�.Text.Trim = "" Or chk�R���U�֓�.Checked = False Then
            strSYOFURI_NENKAN(3) = ""
        Else
            strSYOFURI_NENKAN(3) = Replace(lab�R���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�S���U�֓�.Text.Trim = "" Then
        If lab�S���U�֓�.Text.Trim = "" Or chk�S���U�֓�.Checked = False Then
            strSYOFURI_NENKAN(4) = ""
        Else
            strSYOFURI_NENKAN(4) = Replace(lab�S���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�T���U�֓�.Text.Trim = "" Then
        If lab�T���U�֓�.Text.Trim = "" Or chk�T���U�֓�.Checked = False Then
            strSYOFURI_NENKAN(5) = ""
        Else
            strSYOFURI_NENKAN(5) = Replace(lab�T���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�U���U�֓�.Text.Trim = "" Then
        If lab�U���U�֓�.Text.Trim = "" Or chk�U���U�֓�.Checked = False Then
            strSYOFURI_NENKAN(6) = ""
        Else
            strSYOFURI_NENKAN(6) = Replace(lab�U���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�V���U�֓�.Text.Trim = "" Then
        If lab�V���U�֓�.Text.Trim = "" Or chk�V���U�֓�.Checked = False Then
            strSYOFURI_NENKAN(7) = ""
        Else
            strSYOFURI_NENKAN(7) = Replace(lab�V���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�W���U�֓�.Text.Trim = "" Then
        If lab�W���U�֓�.Text.Trim = "" Or chk�W���U�֓�.Checked = False Then
            strSYOFURI_NENKAN(8) = ""
        Else
            strSYOFURI_NENKAN(8) = Replace(lab�W���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�X���U�֓�.Text.Trim = "" Then
        If lab�X���U�֓�.Text.Trim = "" Or chk�X���U�֓�.Checked = False Then
            strSYOFURI_NENKAN(9) = ""
        Else
            strSYOFURI_NENKAN(9) = Replace(lab�X���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P�O���U�֓�.Text.Trim = "" Then
        If lab�P�O���U�֓�.Text.Trim = "" Or chk�P�O���U�֓�.Checked = False Then
            strSYOFURI_NENKAN(10) = ""
        Else
            strSYOFURI_NENKAN(10) = Replace(lab�P�O���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P�P���U�֓�.Text.Trim = "" Then
        If lab�P�P���U�֓�.Text.Trim = "" Or chk�P�P���U�֓�.Checked = False Then
            strSYOFURI_NENKAN(11) = ""
        Else
            strSYOFURI_NENKAN(11) = Replace(lab�P�P���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P�Q���U�֓�.Text.Trim = "" Then
        If lab�P�Q���U�֓�.Text.Trim = "" Or chk�P�Q���U�֓�.Checked = False Then
            strSYOFURI_NENKAN(12) = ""
        Else
            strSYOFURI_NENKAN(12) = Replace(lab�P�Q���U�֓�.Text, "/", "")
        End If
        '�N�ԍĐU
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P���ĐU�֓�.Text.Trim = "" Then
        If lab�P���ĐU�֓�.Text.Trim = "" Or chk�P���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN(1) = ""
        Else
            strSAIFURI_NENKAN(1) = Replace(lab�P���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�Q���ĐU�֓�.Text.Trim = "" Then
        If lab�Q���ĐU�֓�.Text.Trim = "" Or chk�Q���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN(2) = ""
        Else
            strSAIFURI_NENKAN(2) = Replace(lab�Q���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�R���ĐU�֓�.Text.Trim = "" Then
        If lab�R���ĐU�֓�.Text.Trim = "" Or chk�R���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN(3) = ""
        Else
            strSAIFURI_NENKAN(3) = Replace(lab�R���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�S���ĐU�֓�.Text.Trim = "" Then
        If lab�S���ĐU�֓�.Text.Trim = "" Or chk�S���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN(4) = ""
        Else
            strSAIFURI_NENKAN(4) = Replace(lab�S���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�T���ĐU�֓�.Text.Trim = "" Then
        If lab�T���ĐU�֓�.Text.Trim = "" Or chk�T���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN(5) = ""
        Else
            strSAIFURI_NENKAN(5) = Replace(lab�T���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�U���ĐU�֓�.Text.Trim = "" Then
        If lab�U���ĐU�֓�.Text.Trim = "" Or chk�U���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN(6) = ""
        Else
            strSAIFURI_NENKAN(6) = Replace(lab�U���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�V���ĐU�֓�.Text.Trim = "" Then
        If lab�V���ĐU�֓�.Text.Trim = "" Or chk�V���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN(7) = ""
        Else
            strSAIFURI_NENKAN(7) = Replace(lab�V���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�W���ĐU�֓�.Text.Trim = "" Then
        If lab�W���ĐU�֓�.Text.Trim = "" Or chk�W���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN(8) = ""
        Else
            strSAIFURI_NENKAN(8) = Replace(lab�W���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�X���ĐU�֓�.Text.Trim = "" Then
        If lab�X���ĐU�֓�.Text.Trim = "" Or chk�X���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN(9) = ""
        Else
            strSAIFURI_NENKAN(9) = Replace(lab�X���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P�O���ĐU�֓�.Text.Trim = "" Then
        If lab�P�O���ĐU�֓�.Text.Trim = "" Or chk�P�O���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN(10) = ""
        Else
            strSAIFURI_NENKAN(10) = Replace(lab�P�O���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P�P���ĐU�֓�.Text.Trim = "" Then
        If lab�P�P���ĐU�֓�.Text.Trim = "" Or chk�P�P���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN(11) = ""
        Else
            strSAIFURI_NENKAN(11) = Replace(lab�P�P���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P�Q���ĐU�֓�.Text.Trim = "" Then
        If lab�P�Q���ĐU�֓�.Text.Trim = "" Or chk�P�Q���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN(12) = ""
        Else
            strSAIFURI_NENKAN(12) = Replace(lab�P�Q���ĐU�֓�.Text, "/", "")
        End If
        '���ʏ��U
        strSYOFURI_TOKUBETU(1) = PFUNC_SEIKYUTUKIHI(txt���ʐU�֌��P.Text) & txt���ʐU�֓��P.Text
        strSYOFURI_TOKUBETU(2) = PFUNC_SEIKYUTUKIHI(txt���ʐU�֌��Q.Text) & txt���ʐU�֓��Q.Text
        strSYOFURI_TOKUBETU(3) = PFUNC_SEIKYUTUKIHI(txt���ʐU�֌��R.Text) & txt���ʐU�֓��R.Text
        strSYOFURI_TOKUBETU(4) = PFUNC_SEIKYUTUKIHI(txt���ʐU�֌��S.Text) & txt���ʐU�֓��S.Text
        strSYOFURI_TOKUBETU(5) = PFUNC_SEIKYUTUKIHI(txt���ʐU�֌��T.Text) & txt���ʐU�֓��T.Text
        strSYOFURI_TOKUBETU(6) = PFUNC_SEIKYUTUKIHI(txt���ʐU�֌��U.Text) & txt���ʐU�֓��U.Text
        '���ʍĐU
        strSAIFURI_TOKUBETU(1) = PFUNC_SEIKYUTUKIHI(txt���ʍĐU�֌��P.Text) & txt���ʍĐU�֓��P.Text
        strSAIFURI_TOKUBETU(2) = PFUNC_SEIKYUTUKIHI(txt���ʍĐU�֌��Q.Text) & txt���ʍĐU�֓��Q.Text
        strSAIFURI_TOKUBETU(3) = PFUNC_SEIKYUTUKIHI(txt���ʍĐU�֌��R.Text) & txt���ʍĐU�֓��R.Text
        strSAIFURI_TOKUBETU(4) = PFUNC_SEIKYUTUKIHI(txt���ʍĐU�֌��S.Text) & txt���ʍĐU�֓��S.Text
        strSAIFURI_TOKUBETU(5) = PFUNC_SEIKYUTUKIHI(txt���ʍĐU�֌��T.Text) & txt���ʍĐU�֓��T.Text
        strSAIFURI_TOKUBETU(6) = PFUNC_SEIKYUTUKIHI(txt���ʍĐU�֌��U.Text) & txt���ʍĐU�֓��U.Text
        '�����U�֓�
        strFURI_ZUIJI(1) = PFUNC_SEIKYUTUKIHI(txt�����U�֌��P.Text) & txt�����U�֓��P.Text
        strFURI_ZUIJI(2) = PFUNC_SEIKYUTUKIHI(txt�����U�֌��Q.Text) & txt�����U�֓��Q.Text
        strFURI_ZUIJI(3) = PFUNC_SEIKYUTUKIHI(txt�����U�֌��R.Text) & txt�����U�֓��R.Text
        strFURI_ZUIJI(4) = PFUNC_SEIKYUTUKIHI(txt�����U�֌��S.Text) & txt�����U�֓��S.Text
        strFURI_ZUIJI(5) = PFUNC_SEIKYUTUKIHI(txt�����U�֌��T.Text) & txt�����U�֓��T.Text
        strFURI_ZUIJI(6) = PFUNC_SEIKYUTUKIHI(txt�����U�֌��U.Text) & txt�����U�֓��U.Text
        '�����U�֋敪
        strFURIKBN_ZUIJI(1) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�P)
        strFURIKBN_ZUIJI(2) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�Q)
        strFURIKBN_ZUIJI(3) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�R)
        strFURIKBN_ZUIJI(4) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�S)
        strFURIKBN_ZUIJI(5) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�T)
        strFURIKBN_ZUIJI(6) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�U)
    End Sub

    '�X�V��̏�ԑޔ��@2006/12/04
    Public Sub sb_KOUSIN_SET()
        '�N�ԏ��U
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P���U�֓�.Text.Trim = "" Then
        If lab�P���U�֓�.Text.Trim = "" Or chk�P���U�֓�.Checked = False Then
            strSYOFURI_NENKAN_AFTER(1) = ""
        Else
            strSYOFURI_NENKAN_AFTER(1) = Replace(lab�P���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�Q���U�֓�.Text.Trim = "" Then
        If lab�Q���U�֓�.Text.Trim = "" Or chk�Q���U�֓�.Checked = False Then
            strSYOFURI_NENKAN_AFTER(2) = ""
        Else
            strSYOFURI_NENKAN_AFTER(2) = Replace(lab�Q���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�R���U�֓�.Text.Trim = "" Then
        If lab�R���U�֓�.Text.Trim = "" Or chk�R���U�֓�.Checked = False Then
            strSYOFURI_NENKAN_AFTER(3) = ""
        Else
            strSYOFURI_NENKAN_AFTER(3) = Replace(lab�R���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�S���U�֓�.Text.Trim = "" Then
        If lab�S���U�֓�.Text.Trim = "" Or chk�S���U�֓�.Checked = False Then
            strSYOFURI_NENKAN_AFTER(4) = ""
        Else
            strSYOFURI_NENKAN_AFTER(4) = Replace(lab�S���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�T���U�֓�.Text.Trim = "" Then
        If lab�T���U�֓�.Text.Trim = "" Or chk�T���U�֓�.Checked = False Then
            strSYOFURI_NENKAN_AFTER(5) = ""
        Else
            strSYOFURI_NENKAN_AFTER(5) = Replace(lab�T���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�U���U�֓�.Text.Trim = "" Then
        If lab�U���U�֓�.Text.Trim = "" Or chk�U���U�֓�.Checked = False Then
            strSYOFURI_NENKAN_AFTER(6) = ""
        Else
            strSYOFURI_NENKAN_AFTER(6) = Replace(lab�U���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�V���U�֓�.Text.Trim = "" Then
        If lab�V���U�֓�.Text.Trim = "" Or chk�V���U�֓�.Checked = False Then
            strSYOFURI_NENKAN_AFTER(7) = ""
        Else
            strSYOFURI_NENKAN_AFTER(7) = Replace(lab�V���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�W���U�֓�.Text.Trim = "" Then
        If lab�W���U�֓�.Text.Trim = "" Or chk�W���U�֓�.Checked = False Then
            strSYOFURI_NENKAN_AFTER(8) = ""
        Else
            strSYOFURI_NENKAN_AFTER(8) = Replace(lab�W���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�X���U�֓�.Text.Trim = "" Then
        If lab�X���U�֓�.Text.Trim = "" Or chk�X���U�֓�.Checked = False Then
            strSYOFURI_NENKAN_AFTER(9) = ""
        Else
            strSYOFURI_NENKAN_AFTER(9) = Replace(lab�X���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P�O���U�֓�.Text.Trim = "" Then
        If lab�P�O���U�֓�.Text.Trim = "" Or chk�P�O���U�֓�.Checked = False Then
            strSYOFURI_NENKAN_AFTER(10) = ""
        Else
            strSYOFURI_NENKAN_AFTER(10) = Replace(lab�P�O���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P�P���U�֓�.Text.Trim = "" Then
        If lab�P�P���U�֓�.Text.Trim = "" Or chk�P�P���U�֓�.Checked = False Then
            strSYOFURI_NENKAN_AFTER(11) = ""
        Else
            strSYOFURI_NENKAN_AFTER(11) = Replace(lab�P�P���U�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P�Q���U�֓�.Text.Trim = "" Then
        If lab�P�Q���U�֓�.Text.Trim = "" Or chk�P�Q���U�֓�.Checked = False Then
            strSYOFURI_NENKAN_AFTER(12) = ""
        Else
            strSYOFURI_NENKAN_AFTER(12) = Replace(lab�P�Q���U�֓�.Text, "/", "")
        End If
        '�N�ԍĐU
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P���ĐU�֓�.Text.Trim = "" Then
        If lab�P���ĐU�֓�.Text.Trim = "" Or chk�P���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN_AFTER(1) = ""
        Else
            strSAIFURI_NENKAN_AFTER(1) = Replace(lab�P���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�Q���ĐU�֓�.Text.Trim = "" Then
        If lab�Q���ĐU�֓�.Text.Trim = "" Or chk�Q���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN_AFTER(2) = ""
        Else
            strSAIFURI_NENKAN_AFTER(2) = Replace(lab�Q���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�R���ĐU�֓�.Text.Trim = "" Then
        If lab�R���ĐU�֓�.Text.Trim = "" Or chk�R���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN_AFTER(3) = ""
        Else
            strSAIFURI_NENKAN_AFTER(3) = Replace(lab�R���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�S���ĐU�֓�.Text.Trim = "" Then
        If lab�S���ĐU�֓�.Text.Trim = "" Or chk�S���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN_AFTER(4) = ""
        Else
            strSAIFURI_NENKAN_AFTER(4) = Replace(lab�S���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�T���ĐU�֓�.Text.Trim = "" Then
        If lab�T���ĐU�֓�.Text.Trim = "" Or chk�T���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN_AFTER(5) = ""
        Else
            strSAIFURI_NENKAN_AFTER(5) = Replace(lab�T���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�U���ĐU�֓�.Text.Trim = "" Then
        If lab�U���ĐU�֓�.Text.Trim = "" Or chk�U���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN_AFTER(6) = ""
        Else
            strSAIFURI_NENKAN_AFTER(6) = Replace(lab�U���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�V���ĐU�֓�.Text.Trim = "" Then
        If lab�V���ĐU�֓�.Text.Trim = "" Or chk�V���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN_AFTER(7) = ""
        Else
            strSAIFURI_NENKAN_AFTER(7) = Replace(lab�V���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�W���ĐU�֓�.Text.Trim = "" Then
        If lab�W���ĐU�֓�.Text.Trim = "" Or chk�W���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN_AFTER(8) = ""
        Else
            strSAIFURI_NENKAN_AFTER(8) = Replace(lab�W���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�X���ĐU�֓�.Text.Trim = "" Then
        If lab�X���ĐU�֓�.Text.Trim = "" Or chk�X���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN_AFTER(9) = ""
        Else
            strSAIFURI_NENKAN_AFTER(9) = Replace(lab�X���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P�O���ĐU�֓�.Text.Trim = "" Then
        If lab�P�O���ĐU�֓�.Text.Trim = "" Or chk�P�O���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN_AFTER(10) = ""
        Else
            strSAIFURI_NENKAN_AFTER(10) = Replace(lab�P�O���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P�P���ĐU�֓�.Text.Trim = "" Then
        If lab�P�P���ĐU�֓�.Text.Trim = "" Or chk�P�P���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN_AFTER(11) = ""
        Else
            strSAIFURI_NENKAN_AFTER(11) = Replace(lab�P�P���ĐU�֓�.Text, "/", "")
        End If
        '2010/10/21 �`�F�b�N�{�b�N�X�̏�Ԃ�����
        'If lab�P�Q���ĐU�֓�.Text.Trim = "" Then
        If lab�P�Q���ĐU�֓�.Text.Trim = "" Or chk�P�Q���ĐU�֓�.Checked = False Then
            strSAIFURI_NENKAN_AFTER(12) = ""
        Else
            strSAIFURI_NENKAN_AFTER(12) = Replace(lab�P�Q���ĐU�֓�.Text, "/", "")
        End If
        '���ʏ��U
        strSYOFURI_TOKUBETU_AFTER(1) = PFUNC_SEIKYUTUKIHI(txt���ʐU�֌��P.Text) & txt���ʐU�֓��P.Text
        strSYOFURI_TOKUBETU_AFTER(2) = PFUNC_SEIKYUTUKIHI(txt���ʐU�֌��Q.Text) & txt���ʐU�֓��Q.Text
        strSYOFURI_TOKUBETU_AFTER(3) = PFUNC_SEIKYUTUKIHI(txt���ʐU�֌��R.Text) & txt���ʐU�֓��R.Text
        strSYOFURI_TOKUBETU_AFTER(4) = PFUNC_SEIKYUTUKIHI(txt���ʐU�֌��S.Text) & txt���ʐU�֓��S.Text
        strSYOFURI_TOKUBETU_AFTER(5) = PFUNC_SEIKYUTUKIHI(txt���ʐU�֌��T.Text) & txt���ʐU�֓��T.Text
        strSYOFURI_TOKUBETU_AFTER(6) = PFUNC_SEIKYUTUKIHI(txt���ʐU�֌��U.Text) & txt���ʐU�֓��U.Text
        '���ʍĐU
        strSAIFURI_TOKUBETU_AFTER(1) = PFUNC_SEIKYUTUKIHI(txt���ʍĐU�֌��P.Text) & txt���ʍĐU�֓��P.Text
        strSAIFURI_TOKUBETU_AFTER(2) = PFUNC_SEIKYUTUKIHI(txt���ʍĐU�֌��Q.Text) & txt���ʍĐU�֓��Q.Text
        strSAIFURI_TOKUBETU_AFTER(3) = PFUNC_SEIKYUTUKIHI(txt���ʍĐU�֌��R.Text) & txt���ʍĐU�֓��R.Text
        strSAIFURI_TOKUBETU_AFTER(4) = PFUNC_SEIKYUTUKIHI(txt���ʍĐU�֌��S.Text) & txt���ʍĐU�֓��S.Text
        strSAIFURI_TOKUBETU_AFTER(5) = PFUNC_SEIKYUTUKIHI(txt���ʍĐU�֌��T.Text) & txt���ʍĐU�֓��T.Text
        strSAIFURI_TOKUBETU_AFTER(6) = PFUNC_SEIKYUTUKIHI(txt���ʍĐU�֌��U.Text) & txt���ʍĐU�֓��U.Text
        '�����U�֓�
        strFURI_ZUIJI_AFTER(1) = PFUNC_SEIKYUTUKIHI(txt�����U�֌��P.Text) & txt�����U�֓��P.Text
        strFURI_ZUIJI_AFTER(2) = PFUNC_SEIKYUTUKIHI(txt�����U�֌��Q.Text) & txt�����U�֓��Q.Text
        strFURI_ZUIJI_AFTER(3) = PFUNC_SEIKYUTUKIHI(txt�����U�֌��R.Text) & txt�����U�֓��R.Text
        strFURI_ZUIJI_AFTER(4) = PFUNC_SEIKYUTUKIHI(txt�����U�֌��S.Text) & txt�����U�֓��S.Text
        strFURI_ZUIJI_AFTER(5) = PFUNC_SEIKYUTUKIHI(txt�����U�֌��T.Text) & txt�����U�֓��T.Text
        strFURI_ZUIJI_AFTER(6) = PFUNC_SEIKYUTUKIHI(txt�����U�֌��U.Text) & txt�����U�֓��U.Text
        '�����U�֋敪
        strFURIKBN_ZUIJI_AFTER(1) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�P)
        strFURIKBN_ZUIJI_AFTER(2) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�Q)
        strFURIKBN_ZUIJI_AFTER(3) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�R)
        strFURIKBN_ZUIJI_AFTER(4) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�S)
        strFURIKBN_ZUIJI_AFTER(5) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�T)
        strFURIKBN_ZUIJI_AFTER(6) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�U)
    End Sub

#End Region

#Region " Private Function(����)"
    Private Function PFUNC_COMMON_CHECK() As Boolean

        Dim sStart As String
        Dim sEnd As String

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        Try

            If Trim(txtGAKKOU_CODE.Text) = "" Then
                MessageBox.Show("�w�Z�R�[�h�����͂���Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return False
            Else
                '�w�Z�}�X�^���݃`�F�b�N
                sql.Append("SELECT *")
                sql.Append(" FROM GAKMAST2")
                sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

                If oraReader.DataReader(sql) = True Then

                    Int_Zengo_Kbn(0) = oraReader.GetString("NKYU_CODE_T")
                    Int_Zengo_Kbn(1) = oraReader.GetString("SKYU_CODE_T")
                    '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------START
                    Sai_Zengo_Kbn = oraReader.GetString("SFURI_KYU_CODE_T")
                    '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------END

                    sStart = Mid(oraReader.GetString("KAISI_DATE_T"), 1, 4)
                    sEnd = Mid(oraReader.GetString("SYURYOU_DATE_T"), 1, 4)

                    strFURI_DT = oraReader.GetString("FURI_DATE_T") '2005/12/09
                    strSFURI_DT = ConvNullToString(oraReader.GetString("SFURI_DATE_T"), "") '2005/12/09

                Else
                    MessageBox.Show("���͂��ꂽ�w�Z�R�[�h�����݂��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Return False
                End If

                oraReader.Close()

            End If

            If (Trim(txt�Ώ۔N�x.Text)) = "" Then
                MessageBox.Show("�Ώ۔N�x����͂��Ă�������", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txt�Ώ۔N�x.Focus()
                Return False
            Else
                Select Case (sStart <= txt�Ώ۔N�x.Text >= sEnd)
                    Case False
                        MessageBox.Show("�Ώ۔N�x�����͔͈͊O�ł�(" & sStart & "�`" & sEnd & ")", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt�Ώ۔N�x.Focus()
                        Return False
                End Select
            End If

            GAKKOU_INFO.TAISYOU_START_NENDO = txt�Ώ۔N�x.Text & "04"
            GAKKOU_INFO.TAISYOU_END_NENDO = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & "03"

            Return True

        Catch ex As Exception
            Throw
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

    End Function

    '==============================================================
    '�`�F�b�N�{�b�N�X��ԃ`�F�b�N�E�w�N�t���O�ϐ��擾�@2006/11/30
    '==============================================================
    Private Function PFUNC_GAKUNENFLG_CHECK(ByVal blnCheck_FLG1 As Boolean, ByVal blnCheck_FLG2 As Boolean, ByVal blnCheck_FLG3 As Boolean, ByVal blnCheck_FLG4 As Boolean, ByVal blnCheck_FLG5 As Boolean, ByVal blnCheck_FLG6 As Boolean, ByVal blnCheck_FLG7 As Boolean, ByVal blnCheck_FLG8 As Boolean, ByVal blnCheck_FLG9 As Boolean, ByVal blnCheck_FLGALL As Boolean) As Boolean

        '�`�F�b�N�{�b�N�X��ԃ`�F�b�N
        PFUNC_GAKUNENFLG_CHECK = False

        If blnCheck_FLG1 = False And _
           blnCheck_FLG2 = False And _
           blnCheck_FLG3 = False And _
           blnCheck_FLG4 = False And _
           blnCheck_FLG5 = False And _
           blnCheck_FLG6 = False And _
           blnCheck_FLG7 = False And _
           blnCheck_FLG8 = False And _
           blnCheck_FLG9 = False And _
           blnCheck_FLGALL = False Then

            Call MessageBox.Show("�����Ώۊw�N�w�肪����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        '�`�F�b�N�{�b�N�X��Ԃ����ʕϐ��ɐݒ�()
        If blnCheck_FLGALL = True Then
            STR�P�w�N = "1"
            STR�Q�w�N = "1"
            STR�R�w�N = "1"
            STR�S�w�N = "1"
            STR�T�w�N = "1"
            STR�U�w�N = "1"
            STR�V�w�N = "1"
            STR�W�w�N = "1"
            STR�X�w�N = "1"
        Else
            If blnCheck_FLG1 = True Then
                STR�P�w�N = "1"
            Else
                STR�P�w�N = "0"
            End If
            If blnCheck_FLG2 = True Then
                STR�Q�w�N = "1"
            Else
                STR�Q�w�N = "0"
            End If
            If blnCheck_FLG3 = True Then
                STR�R�w�N = "1"
            Else
                STR�R�w�N = "0"
            End If
            If blnCheck_FLG4 = True Then
                STR�S�w�N = "1"
            Else
                STR�S�w�N = "0"
            End If
            If blnCheck_FLG5 = True Then
                STR�T�w�N = "1"
            Else
                STR�T�w�N = "0"
            End If
            If blnCheck_FLG6 = True Then
                STR�U�w�N = "1"
            Else
                STR�U�w�N = "0"
            End If
            If blnCheck_FLG7 = True Then
                STR�V�w�N = "1"
            Else
                STR�V�w�N = "0"
            End If
            If blnCheck_FLG8 = True Then
                STR�W�w�N = "1"
            Else
                STR�W�w�N = "0"
            End If
            If blnCheck_FLG9 = True Then
                STR�X�w�N = "1"
            Else
                STR�X�w�N = "0"
            End If
        End If

        PFUNC_GAKUNENFLG_CHECK = True

    End Function

    Private Function PFUNC_KYUJITULIST_SET() As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        '�x�����̕\��
        Dim sTuki As String
        Dim sDay As String
        Dim sYName As String

        lst�x��.Items.Clear()

        If Trim(txt�Ώ۔N�x.Text) <> "" Then
            Select Case CInt(txt�Ώ۔N�x.Text)
                Case Is > 1900
                    sql.Append(" SELECT ")
                    sql.Append(" YASUMI_DATE_Y")
                    sql.Append(",YASUMI_NAME_Y")
                    sql.Append(" FROM YASUMIMAST")
                    sql.Append(" WHERE")
                    sql.Append(" YASUMI_DATE_Y > '" & txt�Ώ۔N�x.Text & "0400'")
                    sql.Append(" AND")
                    sql.Append(" YASUMI_DATE_Y < '" & CStr(CInt(txt�Ώ۔N�x.Text) + 1) & "0399'")
                    sql.Append(" ORDER BY YASUMI_DATE_Y ASC")

                    If oraReader.DataReader(sql) = True Then

                        Do Until oraReader.EOF

                            sTuki = Mid(oraReader.GetString("YASUMI_DATE_Y"), 5, 2)
                            sDay = Mid(oraReader.GetString("YASUMI_DATE_Y"), 7, 2)
                            sYName = Trim(oraReader.GetString("YASUMI_NAME_Y"))

                            lst�x��.Items.Add(sTuki & "��" & sDay & "��" & Space(1) & sYName)

                            '2006/10/23�@�x���ꗗ���擾
                            STRYasumi_List(STRYasumi_List.Length - 1) = txt�Ώ۔N�x.Text & sTuki & sDay
                            ReDim Preserve STRYasumi_List(STRYasumi_List.Length)

                            oraReader.NextRead()

                        Loop

                    End If
                    oraReader.Close()

                Case Else
                    MessageBox.Show("�Ώ۔N�x�͂P�X�O�O�N�ȍ~����͂��Ă�������", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt�Ώ۔N�x.Focus()
                    Return False
            End Select
        End If

        Return True

    End Function

    Private Function PFUNC_GAKINFO_GET() As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        sql.Append(" SELECT ")
        sql.Append(" GAKKOU_NNAME_G")
        sql.Append(",SIYOU_GAKUNEN_T")
        sql.Append(",FURI_DATE_T")
        sql.Append(",SFURI_DATE_T")
        sql.Append(",BAITAI_CODE_T")
        sql.Append(",ITAKU_CODE_T")
        sql.Append(",TKIN_NO_T")
        sql.Append(",TSIT_NO_T")
        sql.Append(",SFURI_SYUBETU_T")
        sql.Append(",KAISI_DATE_T")
        sql.Append(",SYURYOU_DATE_T")
        sql.Append(",TESUUTYO_KBN_T")
        sql.Append(",TESUUTYO_KIJITSU_T")
        sql.Append(",TESUUTYO_DAY_T")
        sql.Append(",TESUU_KYU_CODE_T")
        sql.Append(" FROM ")
        sql.Append(" GAKMAST1")
        sql.Append(",GAKMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_G = GAKKOU_CODE_T")
        sql.Append(" AND")
        sql.Append(" GAKUNEN_CODE_G = 1")
        sql.Append(" AND")
        sql.Append(" GAKKOU_CODE_G ='" & Trim(txtGAKKOU_CODE.Text) & "'")


        If oraReader.DataReader(sql) = True Then

            GAKKOU_INFO.GAKKOU_CODE = Trim(txtGAKKOU_CODE.Text)
            GAKKOU_INFO.GAKKOU_NNAME = Trim(oraReader.GetString("GAKKOU_NNAME_G"))

            '�g�p�w�N��
            If IsDBNull(oraReader.GetString("SIYOU_GAKUNEN_T")) = False Then
                GAKKOU_INFO.SIYOU_GAKUNEN = CInt(oraReader.GetString("SIYOU_GAKUNEN_T"))
            Else
                GAKKOU_INFO.SIYOU_GAKUNEN = 0
            End If

            '�U�֓�
            If IsDBNull(oraReader.GetString("FURI_DATE_T")) = False Then
                GAKKOU_INFO.FURI_DATE = oraReader.GetString("FURI_DATE_T")
            Else
                GAKKOU_INFO.FURI_DATE = ""
            End If

            '�ĐU��
            If IsDBNull(oraReader.GetString("SFURI_DATE_T")) = False Then
                GAKKOU_INFO.SFURI_DATE = oraReader.GetString("SFURI_DATE_T")
            Else
                GAKKOU_INFO.SFURI_DATE = ""
            End If

            '�}�̃R�[�h
            If IsDBNull(oraReader.GetString("BAITAI_CODE_T")) = False Then
                GAKKOU_INFO.BAITAI_CODE = oraReader.GetString("BAITAI_CODE_T")
            Else
                GAKKOU_INFO.BAITAI_CODE = ""
            End If

            '�ϑ��҃R�[�h
            If IsDBNull(oraReader.GetString("ITAKU_CODE_T")) = False Then
                GAKKOU_INFO.ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
            Else
                GAKKOU_INFO.ITAKU_CODE = ""
            End If

            '�戵���Z�@�փR�[�h
            GAKKOU_INFO.TKIN_CODE = oraReader.GetString("TKIN_NO_T")

            '�戵�x�X�R�[�h
            GAKKOU_INFO.TSIT_CODE = oraReader.GetString("TSIT_NO_T")

            '�ĐU���
            If IsDBNull(oraReader.GetString("SFURI_SYUBETU_T")) = False Then
                GAKKOU_INFO.SFURI_SYUBETU = oraReader.GetString("SFURI_SYUBETU_T")
            Else
                GAKKOU_INFO.SFURI_SYUBETU = ""
            End If

            '���U�J�n�N��
            GAKKOU_INFO.KAISI_DATE = oraReader.GetString("KAISI_DATE_T")

            '���U�I���N��
            GAKKOU_INFO.SYURYOU_DATE = oraReader.GetString("SYURYOU_DATE_T")

            '�萔�����������敪
            If IsDBNull(oraReader.GetString("TESUUTYO_KIJITSU_T")) = False Then
                GAKKOU_INFO.TESUUTYO_KIJITSU = oraReader.GetString("TESUUTYO_KIJITSU_T")
            Else
                GAKKOU_INFO.TESUUTYO_KIJITSU = ""
            End If

            '�萔����������
            If IsDBNull(oraReader.GetString("TESUUTYO_DAY_T")) = False Then
                GAKKOU_INFO.TESUUTYO_NO = CInt(oraReader.GetString("TESUUTYO_DAY_T"))
            Else
                GAKKOU_INFO.TESUUTYO_NO = 0
            End If

            '�萔�������敪
            If IsDBNull(oraReader.GetString("TESUUTYO_KBN_T")) = False Then
                GAKKOU_INFO.TESUUTYO_KBN = oraReader.GetString("TESUUTYO_KBN_T")
            Else
                GAKKOU_INFO.TESUUTYO_KBN = ""
            End If

            '���ϋx���R�[�h
            If IsDBNull(oraReader.GetString("TESUU_KYU_CODE_T")) = False Then
                GAKKOU_INFO.TESUU_KYU_CODE = oraReader.GetString("TESUU_KYU_CODE_T")
            Else
                GAKKOU_INFO.TESUU_KYU_CODE = ""
            End If

        Else

            MessageBox.Show("�w�Z�}�X�^�ɓo�^����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            lab�w�Z��.Text = ""

            oraReader.Close()
            Return False

        End If

        oraReader.Close()

        lab�w�Z��.Text = GAKKOU_INFO.GAKKOU_NNAME

        Return True

    End Function

    Private Function PFUNC_SCH_GET_ALL() As Boolean

        PFUNC_SCH_GET_ALL = False

        '���ʓ��̓`�F�b�N
        If PFUNC_COMMON_CHECK() = False Then
            Exit Function
        End If

        '�X�P�W���[���}�X�^���݃`�F�b�N
        If PFUNC_SCHMAST_SERCH() = False Then
            Call MessageBox.Show("�w�肵���N�x�̊w�Z�X�P�W���[���͑��݂��܂���ł����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        '�N�ԃX�P�W���[���Q��
        If PFUNC_SCH_GET_NENKAN() = False Then
            ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- START
            MainLOG.Write("�N�ԃX�P�W���[���Q��", "����", "�i�Ώی����O���j")
            'MainLOG.Write("�N�ԃX�P�W���[���Q��", "���s", "�i�Ώی����O���j")
            ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- END
        Else
            MainLOG.Write("�N�ԃX�P�W���[���Q��", "����", "")
        End If

        '���ʃX�P�W���[���Q��
        If PFUNC_SCH_GET_TOKUBETU() = False Then
            ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- START
            MainLOG.Write("���ʃX�P�W���[���Q��", "����", "�i�Ώی����O���j")
            'MainLOG.Write("���ʃX�P�W���[���Q��", "���s", "�i�Ώی����O���j")
            ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- END
        Else
            MainLOG.Write("���ʃX�P�W���[���Q��", "����", "")
        End If

        '�����X�P�W���[���Q��
        If PFUNC_SCH_GET_ZUIJI() = False Then
            ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- START
            MainLOG.Write("�����X�P�W���[���Q��", "����", "�i�Ώی����O���j")
            'MainLOG.Write("�����X�P�W���[���Q��", "���s", "�i�Ώی����O���j")
            ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- END
        Else
            MainLOG.Write("�����X�P�W���[���Q��", "����", "")
        End If

        '2006/11/30�@�e���̒l���\���̂ɓ���
        Call PSUB_NENKAN_GET(SYOKI_NENKAN_SCHINFO) '    �N�ԃX�P�W���[����
        Call PSUB_TOKUBETU_GET(SYOKI_TOKUBETU_SCHINFO) '���ʃX�P�W���[����
        Call PSUB_ZUIJI_GET(SYOKI_ZUIJI_SCHINFO) '      �����X�P�W���[����

        PFUNC_SCH_GET_ALL = True

    End Function

    Private Function PFUNC_SCH_INSERT_ALL() As Boolean

        PFUNC_SCH_INSERT_ALL = False

        Try
            MainDB = New MyOracle

            '���ʓ��̓`�F�b�N
            If PFUNC_COMMON_CHECK() = False Then
                Exit Function
            End If

            '2006/10/12�@���U�ƍĐU���������ł͂Ȃ����`�F�b�N
            If PFUNC_CHECK_SFURI() = False Then
                Exit Function
            End If

            '2006/11/22�@�X�P�W���[��������`�F�b�N
            If PFUNC_CHECK_TOKUBETSU() = False Then
                Exit Function
            End If

            '2006/11/30�@�������������w�N�t���O���Ȃ����`�F�b�N
            If PFUNC_GAKNENFLG_CHECK() = False Then
                Exit Function
            End If

            '2010/10/21 �����̓���X�P�W���[���`�F�b�N
            If PFUNC_CHECK_ZUIJI() = False Then
                Exit Function
            End If

            Int_Syori_Flag(0) = 0
            Int_Syori_Flag(1) = 0
            Int_Syori_Flag(2) = 0

            Str_SyoriDate(0) = Format(Now, "yyyyMMdd")
            Str_SyoriDate(1) = Format(Now, "yyyyMMddHHmmss")

            MainDB.BeginTrans()

            '�N�ԃX�P�W���[���쐬
            If PFUNC_NENKAN_SAKUSEI() = False Then
                MainDB.Rollback()
                '������ʂ�Ƃ������Ƃ͂P���ł������������R�[�h�����݂����Ƃ������ƂȂ̂�
                Int_Syori_Flag(0) = 2
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", "�N�ԃX�P�W���[���쐬�ŃG���[")
                Return False
            End If

            '���ʃX�P�W���[���쐬
            If PFUNC_TOKUBETU_SAKUSEI("�쐬") = False Then
                MainDB.Rollback()
                '������ʂ�Ƃ������Ƃ͂P���ł������������R�[�h�����݂����Ƃ������ƂȂ̂�
                Int_Syori_Flag(1) = 2
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", "���ʃX�P�W���[���쐬�ŃG���[")
                Return False
            End If

            '�����X�P�W���[���쐬
            If PFUNC_ZUIJI_SAKUSEI("�쐬") = False Then
                MainDB.Rollback()
                '������ʂ�Ƃ������Ƃ͂P���ł������������R�[�h�����݂����Ƃ������ƂȂ̂�
                Int_Syori_Flag(2) = 2
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", "�����X�P�W���[���쐬�ŃG���[")
                Return False
            End If

            '�s�v�N�ԃX�P�W���[���폜����
            If PFUNC_DELETE_GSCHMAST() = False Then
                MainDB.Rollback()
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", "�s�v�N�ԃX�P�W���[���폜�ŃG���[")
                Return False
            End If

            'Select Case True
            '    Case (Int_Syori_Flag(0) = 0 And Int_Syori_Flag(1) = 0 And Int_Syori_Flag(2) = 0)
            '        '���������Ȃ�
            '        Exit Function
            'End Select

            If Int_Syori_Flag(0) = 1 Then
                '�N�ԃX�P�W���[���Q��
                If PFUNC_SCH_GET_NENKAN() = False Then
                    ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- START
                    MainLOG.Write("�N�ԃX�P�W���[���Q��", "����", "�i�Ώی����O���j")
                    'MainLOG.Write("�N�ԃX�P�W���[���Q��", "���s", "�i�Ώی����O���j")
                    ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- END
                Else
                    MainLOG.Write("�N�ԃX�P�W���[���Q��", "����", "")
                End If
            End If

            If Int_Syori_Flag(1) = 1 Then
                '���ʃX�P�W���[���Q��
                If PFUNC_SCH_GET_TOKUBETU() = False Then
                    ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- START
                    MainLOG.Write("���ʃX�P�W���[���Q��", "����", "�i�Ώی����O���j")
                    'MainLOG.Write("���ʃX�P�W���[���Q��", "���s", "�i�Ώی����O���j")
                    ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- END
                Else
                    MainLOG.Write("���ʃX�P�W���[���Q��", "����", "")
                End If
            End If

            If Int_Syori_Flag(2) = 1 Then
                '�����X�P�W���[���Q��
                If PFUNC_SCH_GET_ZUIJI() = False Then
                    ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- START
                    MainLOG.Write("�����X�P�W���[���Q��", "����", "�i�Ώی����O���j")
                    'MainLOG.Write("�����X�P�W���[���Q��", "���s", "�i�Ώی����O���j")
                    ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- END
                Else
                    MainLOG.Write("�����X�P�W���[���Q��", "����", "")
                End If
            End If

            '2006/11/30�@�e���̒l���\���̂ɓ���
            Call PSUB_NENKAN_GET(SYOKI_NENKAN_SCHINFO) '    �N�ԃX�P�W���[����
            Call PSUB_TOKUBETU_GET(SYOKI_TOKUBETU_SCHINFO) '���ʃX�P�W���[����
            Call PSUB_ZUIJI_GET(SYOKI_ZUIJI_SCHINFO) '      �����X�P�W���[����

            MainDB.Commit()

            If Int_Syori_Flag(0) <> 2 Then '�ǉ� 2005/06/15
                MessageBox.Show("�X�P�W���[�����쐬����܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            PFUNC_SCH_INSERT_ALL = True

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error) '2010/10/21 ��O���̃G���[���b�Z�[�W�ǉ�
            Return False
        Finally
            MainDB.Close()
        End Try

    End Function

    Private Function PFUNC_SCH_DELETE_INSERT_ALL() As Boolean

        PFUNC_SCH_DELETE_INSERT_ALL = False

        '���ʓ��̓`�F�b�N
        If PFUNC_COMMON_CHECK() = False Then
            Exit Function
        End If

        '2006/10/12�@���U�ƍĐU���������ł͂Ȃ����`�F�b�N
        If PFUNC_CHECK_SFURI() = False Then
            Exit Function
        End If

        '2006/11/22�@�X�P�W���[��������`�F�b�N
        If PFUNC_CHECK_TOKUBETSU() = False Then
            Exit Function
        End If

        '2006/11/30�@�������������w�N�t���O���Ȃ����`�F�b�N
        If PFUNC_GAKNENFLG_CHECK() = False Then
            Exit Function
        End If

        '2010/10/21 �����̓���X�P�W���[���`�F�b�N
        If PFUNC_CHECK_ZUIJI() = False Then
            Exit Function
        End If

        If MessageBox.Show("���݂̃X�P�W���[���̓��e�͈�V����܂�", msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> vbOK Then
            Return False
        End If

        Int_Syori_Flag(0) = 0
        Int_Syori_Flag(1) = 0
        Int_Syori_Flag(2) = 0

        Str_SyoriDate(0) = Format(Now, "yyyyMMdd")
        Str_SyoriDate(1) = Format(Now, "yyyyMMddHHmmss")

        '2006/11/30�@�Q�Ǝ��̃f�[�^�Ɣ�ׁA�X�V���K�v�ȃf�[�^���`�F�b�N����
        Call PSUB_Kousin_Check()

        '�N�ԃX�P�W���[���쐬
        If PFUNC_SCH_DELETE_INSERT_NENKAN() = False Then
            MainLOG.Write("�N�ԃX�P�W���[���X�V����", "���s", "")
            Exit Function
        Else
            MainLOG.Write("�N�ԃX�P�W���[���X�V����", "����", "")
        End If

        '���ʃX�P�W���[���쐬
        If PFUNC_SCH_DELETE_INSERT_TOKUBETU() = False Then
            MainLOG.Write("���ʃX�P�W���[���X�V����", "���s", "")
            Exit Function
        Else
            MainLOG.Write("���ʃX�P�W���[���X�V����", "����", "")
        End If

        '�����X�P�W���[���쐬
        If PFUNC_SCH_DELETE_INSERT_ZUIJI() = False Then
            MainLOG.Write("�����X�P�W���[���X�V����", "���s", "")
            Exit Function
        Else
            MainLOG.Write("�����X�P�W���[���X�V����", "����", "")
        End If

        'Select case True
        '    Case (Int_Syori_Flag(0) = 0 AND Int_Syori_Flag(1) = 0 AND Int_Syori_Flag(2) = 0)
        '        '���������Ȃ�
        '        Exit Function
        'End Select

        If Int_Syori_Flag(0) = 1 Then
            '�N�ԃX�P�W���[���Q��
            If PFUNC_SCH_GET_NENKAN() = False Then
                ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- START
                MainLOG.Write("�N�ԃX�P�W���[���Q��", "����", "�i�Ώی����O���j")
                'MainLOG.Write("�N�ԃX�P�W���[���Q��", "���s", "�i�Ώی����O���j")
                ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- END
            Else
                MainLOG.Write("�N�ԃX�P�W���[���Q��", "����", "")
            End If
        End If

        If Int_Syori_Flag(1) = 1 Then
            '���ʃX�P�W���[���Q��
            If PFUNC_SCH_GET_TOKUBETU() = False Then
                ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- START
                MainLOG.Write("���ʃX�P�W���[���Q��", "����", "�i�Ώی����O���j")
                'MainLOG.Write("���ʃX�P�W���[���Q��", "���s", "�i�Ώی����O���j")
                ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- END
            Else
                MainLOG.Write("���ʃX�P�W���[���Q��", "����", "")
            End If
        End If

        If Int_Syori_Flag(2) = 1 Then
            '�����X�P�W���[���Q��
            If PFUNC_SCH_GET_ZUIJI() = False Then
                ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- START
                MainLOG.Write("�����X�P�W���[���Q��", "����", "�i�Ώی����O���j")
                'MainLOG.Write("�����X�P�W���[���Q��", "���s", "�i�Ώی����O���j")
                ' 2016/02/08 �^�X�N�j��� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�����o�O�C��)) -------------------- END
            Else
                MainLOG.Write("�����X�P�W���[���Q��", "����", "")
            End If
        End If

        '��Ǝ��U�A�g 2006/12/04
        Call sb_KOUSIN_SET()
        If fn_CHECK_CHANGE() = False Then
            Exit Function
        End If

        '2006/11/30�@�e���̒l���\���̂ɓ���
        Call PSUB_NENKAN_GET(SYOKI_NENKAN_SCHINFO) '    �N�ԃX�P�W���[����
        Call PSUB_TOKUBETU_GET(SYOKI_TOKUBETU_SCHINFO) '���ʃX�P�W���[����
        Call PSUB_ZUIJI_GET(SYOKI_ZUIJI_SCHINFO) '      �����X�P�W���[����

        MessageBox.Show("�X�P�W���[�����X�V����܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        PFUNC_SCH_DELETE_INSERT_ALL = True

    End Function

    Private Function PFUNC_SEIKYUTUKIHI(ByVal strTUKI As String) As String

        '���͑Ώ۔N�x�ƗL���������Ƃɐ����N�����v�Z
        If strTUKI = "01" Or strTUKI = "02" Or strTUKI = "03" Then
            PFUNC_SEIKYUTUKIHI = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & strTUKI
        Else
            PFUNC_SEIKYUTUKIHI = txt�Ώ۔N�x.Text & strTUKI
        End If

    End Function

    Private Function PFUNC_FURIHI_MAKE(ByVal strFURITUKI As String, ByVal strFURIHI As String, ByVal strSCHKUBUN As String, ByVal strFURIKUBUN As String) As String

        '�U�֓��̍쐬
        PFUNC_FURIHI_MAKE = ""

        Select Case strSCHKUBUN
            Case "0"     '�ʏ�
                If strFURIHI = "" Then
                    Select Case strFURIKUBUN
                        Case "0"   '���U
                            PFUNC_FURIHI_MAKE = STR�����N�� & GAKKOU_INFO.FURI_DATE
                        Case "1"   '�ĐU
                            PFUNC_FURIHI_MAKE = STR�����N�� & GAKKOU_INFO.SFURI_DATE
                    End Select
                Else
                    PFUNC_FURIHI_MAKE = STR�����N�� & strFURIHI
                End If
            Case "1"     '����
                '���͑Ώ۔N�x�Ɠ��͐U�֌��A�������ƂɐU�֔N�������v�Z
                If strFURITUKI = "01" Or strFURITUKI = "02" Or strFURITUKI = "03" Then
                    PFUNC_FURIHI_MAKE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & strFURITUKI & strFURIHI
                Else
                    PFUNC_FURIHI_MAKE = txt�Ώ۔N�x.Text & strFURITUKI & strFURIHI
                End If
            Case "2"     '����
                PFUNC_FURIHI_MAKE = STR�����N�� & strFURIHI
        End Select

        '�c�Ɠ��Z�o
        Select Case Int_Zengo_Kbn(1)
            Case 0
                '���c�Ɠ�
                PFUNC_FURIHI_MAKE = PFUNC_EIGYOUBI_GET(PFUNC_FURIHI_MAKE, "0", "+")
            Case 1
                '�O�c�Ɠ�
                PFUNC_FURIHI_MAKE = PFUNC_EIGYOUBI_GET(PFUNC_FURIHI_MAKE, "0", "-")
        End Select

    End Function

    Private Function PFUNC_SAIFURIHI_MAKE(ByVal strFURITUKI As String, ByVal strFURIHI As String) As String

        '�ĐU�֓��̍쐬
        PFUNC_SAIFURIHI_MAKE = ""

        '�ĐU�֓��̏����l�ݒ�
        PFUNC_SAIFURIHI_MAKE = STRW�ĐU�֔N & strFURITUKI & strFURIHI

        '�ĐU��ʂ��u�O�v�A�u�R�v�̏ꍇ�͍ĐU�֓��̐ݒ�͕s�v
        Select Case GAKKOU_INFO.SFURI_SYUBETU
            Case "0"
                PFUNC_SAIFURIHI_MAKE = "00000000"
            Case "3"
                PFUNC_SAIFURIHI_MAKE = "00000000"
            Case Else
                '�c�Ɠ��Z�o
                '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------START
                Select Case Sai_Zengo_Kbn
                    'Select Case Int_Zengo_Kbn(1)
                    '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------END
                    Case 0
                        '���c�Ɠ�
                        PFUNC_SAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(PFUNC_SAIFURIHI_MAKE, "0", "+")
                    Case 1
                        '�O�c�Ɠ�
                        PFUNC_SAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(PFUNC_SAIFURIHI_MAKE, "0", "-")
                End Select
        End Select

    End Function

    Private Function PFUNC_SAISAIFURIHI_MAKE(ByVal strFURITUKI As String, ByVal strFURIHI As String) As String
        '�ĐU���R�[�h�̍ĐU�֓��̍쐬�i����̏��U���j
        Dim str�N As String
        Dim str�� As String

        PFUNC_SAISAIFURIHI_MAKE = ""

        str�N = Mid(STR�����N��, 1, 4)

        If strFURIHI <= GAKKOU_INFO.FURI_DATE Then
            str�� = strFURITUKI
        Else
            If strFURITUKI = "12" Then
                str�� = "01"
                str�N = CStr(CInt(Mid(STR�����N��, 1, 4)) + 1)

            Else
                str�� = Format((CInt(strFURITUKI) + 1), "00")
            End If
        End If

        '�c�Ɠ��Z�o
        PFUNC_SAISAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(str�N & str�� & GAKKOU_INFO.FURI_DATE, "0", "+")

    End Function

    '2011/06/15 �W���ŏC�� �āX�U�֓��Z�o�p�֐��ǉ� -------------START
    Private Function PFUNC_SAISAIFURIHI_MAKE(ByVal strFURIDATE As String) As String
        '�ĐU���R�[�h�̍ĐU�֓��̍쐬�i����̏��U���j
        Dim str�N As String
        Dim str�� As String

        PFUNC_SAISAIFURIHI_MAKE = ""

        str�N = strFURIDATE.Substring(0, 4)
        str�� = strFURIDATE.Substring(4, 2)

        '�ĐU�� >= ���ꌎ�̏��U���ƂȂ�ꍇ�́A�����̏��U��������̏��U���Ƃ���
        If strFURIDATE >= str�N & str�� & GAKKOU_INFO.FURI_DATE Then
            If str�� = "12" Then
                str�N = (CInt(str�N) + 1).ToString("0000")
                str�� = "01"
            Else
                str�� = (CInt(str��) + 1).ToString("00")
            End If
        End If

        '�c�Ɠ��Z�o
        Select Case Int_Zengo_Kbn(1)
            Case 0
                '���c�Ɠ�
                PFUNC_SAISAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(str�N & str�� & GAKKOU_INFO.FURI_DATE, "0", "+")
            Case 1
                '�O�c�Ɠ�
                PFUNC_SAISAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(str�N & str�� & GAKKOU_INFO.FURI_DATE, "0", "-")
        End Select

    End Function
    '2011/06/15 �W���ŏC�� �āX�U�֓��Z�o�p�֐��ǉ� -------------END
    '2011/06/15 �W���ŏC�� �āX�U�֓��Z�o�p�֐��ǉ� -------------START
    Private Function PFUNC_KFURIHI_MAKE(ByVal strFURITUKI As String, ByVal strFURIHI As String, ByVal strSCHKUBUN As String, ByVal strFURIKUBUN As String) As String

        '�U�֓��̍쐬
        PFUNC_KFURIHI_MAKE = ""

        Select Case strSCHKUBUN
            Case "0"     '�ʏ�
                If strFURIHI = "" Then
                    Select Case strFURIKUBUN
                        Case "0"   '���U
                            PFUNC_KFURIHI_MAKE = STR�����N�� & GAKKOU_INFO.FURI_DATE
                        Case "1"   '�ĐU
                            '2011/06/15 �W���ŏC�� �_��U�֓��ƌ_��ĐU�����t�]����ꍇ�͗����̍ĐU���ɂ��� -------------START
                            'PFUNC_KFURIHI_MAKE = STR�����N�� & GAKKOU_INFO.SFURI_DATE
                            If GAKKOU_INFO.FURI_DATE < GAKKOU_INFO.SFURI_DATE Then
                                PFUNC_KFURIHI_MAKE = STR�����N�� & GAKKOU_INFO.SFURI_DATE
                            Else
                                If STR�����N��.Substring(4, 2) = "12" Then
                                    PFUNC_KFURIHI_MAKE = (CInt(STR�����N��.Substring(0, 4)) + 1).ToString("0000") & "01" & GAKKOU_INFO.SFURI_DATE
                                Else
                                    PFUNC_KFURIHI_MAKE = (CInt(STR�����N��) + 1).ToString("000000") & GAKKOU_INFO.SFURI_DATE
                                End If
                            End If
                            '2011/06/15 �W���ŏC�� �_��U�֓��ƌ_��ĐU�����t�]����ꍇ�͗����̍ĐU���ɂ��� -------------E
                    End Select
                Else
                    ''���͓��t���_��U�֓��ɂ���ꍇ
                    'PFUNC_KFURIHI_MAKE = STR�����N�� & strFURIHI

                    '���U�֓����_��U�֓��ɂ���ꍇ
                    PFUNC_KFURIHI_MAKE = PFUNC_FURIHI_MAKE(strFURITUKI, strFURIHI, strSCHKUBUN, strFURIKUBUN)
                End If
            Case "1"     '����
                ''���͓��t���_��U�֓��ɂ���ꍇ
                ''���͑Ώ۔N�x�Ɠ��͐U�֌��A�������ƂɐU�֔N�������v�Z
                'If strFURITUKI = "01" Or strFURITUKI = "02" Or strFURITUKI = "03" Then
                '    PFUNC_KFURIHI_MAKE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & strFURITUKI & strFURIHI
                'Else
                '    PFUNC_KFURIHI_MAKE = txt�Ώ۔N�x.Text & strFURITUKI & strFURIHI
                'End If

                '���U�֓����_��U�֓��ɂ���ꍇ
                PFUNC_KFURIHI_MAKE = PFUNC_FURIHI_MAKE(strFURITUKI, strFURIHI, strSCHKUBUN, strFURIKUBUN)
            Case "2"     '����
                ''���͓��t���_��U�֓��ɂ���ꍇ
                'PFUNC_KFURIHI_MAKE = STR�����N�� & strFURIHI

                '���U�֓����_��U�֓��ɂ���ꍇ
                PFUNC_KFURIHI_MAKE = PFUNC_FURIHI_MAKE(strFURITUKI, strFURIHI, strSCHKUBUN, strFURIKUBUN)
        End Select

        '�����␳(�����w��̏ꍇ�����ɕϊ�����)
        Dim strFURINEN As String = PFUNC_KFURIHI_MAKE.Substring(0, 4)
        strFURITUKI = PFUNC_KFURIHI_MAKE.Substring(4, 2)
        strFURIHI = PFUNC_KFURIHI_MAKE.Substring(6, 2)

        Dim intGETUMATU As Integer = Date.DaysInMonth(CInt(strFURINEN), CInt(strFURITUKI))
        If CInt(strFURIHI) > intGETUMATU Then
            PFUNC_KFURIHI_MAKE = strFURINEN & strFURITUKI & intGETUMATU.ToString("00")
        End If

    End Function
    '2011/06/15 �W���ŏC�� �āX�U�֓��Z�o�p�֐��ǉ� -------------END

    Private Function PFUNC_EIGYOUBI_GET(ByVal str�N���� As String, ByVal str���� As String, ByVal str�O��c�Ɠ��敪 As String) As String

        '�c�Ɠ��Z�o
        Dim WORK_DATE As Date
        Dim YOUBI As Long
        Dim HOSEI As Integer

        Dim int���� As Integer

        PFUNC_EIGYOUBI_GET = ""

        int���� = CInt(str����)

        '-------------------------------------
        '�����␳�i�����w��̏ꍇ�����ɕϊ�����j
        '-------------------------------------
        Select Case Mid(str�N����, 5, 2)
            Case "01", "03", "05", "07", "08", "10", "12"
                If Mid(str�N����, 7, 2) < "01" Then
                    Mid(str�N����, 7, 2) = "01"
                End If
                If Mid(str�N����, 7, 2) > "31" Then
                    Mid(str�N����, 7, 2) = "31"
                End If
                WORK_DATE = DateSerial(CInt(Mid(str�N����, 1, 4)), CInt(Mid(str�N����, 5, 2)), CInt(Mid(str�N����, 7, 2)))
            Case "04", "06", "09", "11"
                If Mid(str�N����, 7, 2) < "01" Then
                    Mid(str�N����, 7, 2) = "01"
                End If
                If Mid(str�N����, 7, 2) > "30" Then
                    Mid(str�N����, 7, 2) = "30"
                End If
                WORK_DATE = DateSerial(CInt(Mid(str�N����, 1, 4)), CInt(Mid(str�N����, 5, 2)), CInt(Mid(str�N����, 7, 2)))
            Case "02"
                If Mid(str�N����, 7, 2) < "01" Then
                    Mid(str�N����, 7, 2) = "01"
                End If
                '�Q���Q�X��,�Q���R�O��,�Q���R�P���͂Q�������w�舵���łQ�������i�����ɕϊ��j
                If Mid(str�N����, 7, 2) > "28" Then
                    '�P�����̎����œ��t�^�f�[�^�ϊ�
                    WORK_DATE = Mid(str�N����, 1, 4) & "/" & "01" & "/" & "31"
                    '�Q�����̎������Z�o
                    WORK_DATE = DateAdd(DateInterval.Month, 1, WORK_DATE)
                Else
                    '�Q�������ȊO�̓��t�ϊ�
                    WORK_DATE = DateSerial(CInt(Mid(str�N����, 1, 4)), CInt(Mid(str�N����, 5, 2)), CInt(Mid(str�N����, 7, 2)))
                End If
        End Select

        '------------
        '�O�c�Ɠ��Ή�
        '------------
        If int���� = 0 Then
            YOUBI = Weekday(WORK_DATE)

            '�j������(Sun = 1:Sat = 7)
            If YOUBI = 1 Or _
               YOUBI = 7 Or _
               PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = False Then
                HOSEI = 1
            Else
                HOSEI = 0
            End If

            Do Until HOSEI = 0
                If str�O��c�Ɠ��敪 = "+" Then
                    WORK_DATE = DateAdd(DateInterval.Day, 1, WORK_DATE)
                End If
                If str�O��c�Ɠ��敪 = "-" Then
                    WORK_DATE = DateAdd(DateInterval.Day, -1, WORK_DATE)
                End If
                YOUBI = Weekday(WORK_DATE)

                '�j������(Sun = 1:Sat = 7)
                If (YOUBI <> 1) And (YOUBI <> 7) Then
                    If PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = True Then
                        HOSEI = HOSEI - 1
                    End If
                End If
            Loop
        Else
            '-----------------
            '�O�c�Ɠ��ȊO�̏���
            '-----------------
            Do Until int���� = 0
                If str�O��c�Ɠ��敪 = "+" Then
                    WORK_DATE = DateAdd(DateInterval.Day, 1, WORK_DATE)
                End If
                If str�O��c�Ɠ��敪 = "-" Then
                    WORK_DATE = DateAdd(DateInterval.Day, -1, WORK_DATE)
                End If

                YOUBI = Weekday(WORK_DATE)

                '�j������(Sun = 1:Sat = 7)
                If (YOUBI <> 1) And (YOUBI <> 7) Then
                    If PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = True Then
                        int���� = int���� - 1
                    End If
                End If
            Loop
        End If

        PFUNC_EIGYOUBI_GET = Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")

    End Function

    Private Function PFUNC_COMMON_YASUMIGET(ByVal str�N���� As String) As Boolean

        '�x���}�X�^���݃`�F�b�N
        PFUNC_COMMON_YASUMIGET = False

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        Try

            sql.Append(" SELECT * FROM YASUMIMAST")
            sql.Append(" WHERE")
            sql.Append(" YASUMI_DATE_Y ='" & str�N���� & "'")

            If oraReader.DataReader(sql) = True Then
                Return False
            End If

            PFUNC_COMMON_YASUMIGET = True

        Catch ex As Exception

            Throw

        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

    End Function

    Private Function PFUNC_SCHMAST_GET(ByVal strSCHKBN As String, ByVal strFURIKBN As String, ByVal strFURIHI As String, ByVal strSAIFURIHI As String) As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)
        Dim bret As Boolean = False

        '�X�P�W���[���}�X�^���݃`�F�b�N 
        '�L�[�́A�w�Z�R�[�h�A�X�P�W���[���敪�A�U�֋敪�A�U�֓�,�ĐU�֓�
        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = '" & strSCHKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & strFURIHI & "'")
        '2006/11/30�@�N�ԃX�P�W���[���͍ĐU�����`�F�b�N���Ȃ�
        If strSCHKBN <> "0" Then
            sql.Append(" AND")
            sql.Append(" SFURI_DATE_S ='" & strSAIFURIHI & "'")
        End If

        If oraReader.DataReader(sql) = True Then
            bret = True
        End If
        oraReader.Close()

        Return bret

    End Function

    Private Function PFUNC_SCHMAST_GET_FLG(ByVal strSCHKBN As String, ByVal strFURIKBN As String, ByVal strFURIHI As String) As Boolean
        '�ʏ�̃X�P�W���[���̏����t���O�擾 2006/10/24

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '�X�P�W���[���}�X�^���݃`�F�b�N 
        '�L�[�́A�w�Z�R�[�h�A�X�P�W���[���敪�A�U�֋敪�A�U�֓�
        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = '" & strSCHKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & strFURIHI & "'")

        '������
        strENTRI_FLG = "0"
        strCHECK_FLG = "0"
        strDATA_FLG = "0"
        strFUNOU_FLG = "0"
        strSAIFURI_FLG = "0"
        strKESSAI_FLG = "0"
        strTYUUDAN_FLG = "0"
        strSAIFURI_DEF = "00000000"

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                strENTRI_FLG = oraReader.GetString("ENTRI_FLG_S")
                strCHECK_FLG = oraReader.GetString("CHECK_FLG_S")
                strDATA_FLG = oraReader.GetString("DATA_FLG_S")
                strFUNOU_FLG = oraReader.GetString("FUNOU_FLG_S")
                strSAIFURI_FLG = oraReader.GetString("SAIFURI_FLG_S")
                strKESSAI_FLG = oraReader.GetString("KESSAI_FLG_S")
                strTYUUDAN_FLG = oraReader.GetString("TYUUDAN_FLG_S")
                strSAIFURI_DEF = oraReader.GetString("SFURI_DATE_S")

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()

        Return True

    End Function
    Private Function PFUNC_SCHMAST_GET_FLG_SAI(ByVal strSCHKBN As String, ByVal strFURIKBN As String, ByVal strFURIHI As String) As Boolean
        '�ʏ�̃X�P�W���[���̏����t���O(�ĐU��)�擾 2006/10/24

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '�X�P�W���[���}�X�^���݃`�F�b�N 
        '�L�[�́A�w�Z�R�[�h�A�X�P�W���[���敪�A�U�֋敪�A�U�֓�
        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = '" & strSCHKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & strFURIHI & "'")

        '������
        strENTRI_FLG_SAI = "0"
        strCHECK_FLG_SAI = "0"
        strDATA_FLG_SAI = "0"
        strFUNOU_FLG_SAI = "0"
        strSAIFURI_FLG_SAI = "0"
        strKESSAI_FLG_SAI = "0"
        strTYUUDAN_FLG_SAI = "0"

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                strENTRI_FLG_SAI = oraReader.GetString("ENTRI_FLG_S")
                strCHECK_FLG_SAI = oraReader.GetString("CHECK_FLG_S")
                strDATA_FLG_SAI = oraReader.GetString("DATA_FLG_S")
                strFUNOU_FLG_SAI = oraReader.GetString("FUNOU_FLG_S")
                strSAIFURI_FLG_SAI = oraReader.GetString("SAIFURI_FLG_S")
                strKESSAI_FLG_SAI = oraReader.GetString("KESSAI_FLG_S")
                strTYUUDAN_FLG_SAI = oraReader.GetString("TYUUDAN_FLG_S")

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()

        Return True

    End Function

    Private Function PFUNC_G_MEIMAST_COUNT_MOTO(ByVal strNENGETUDO As String, ByVal strFURIKBN As String, ByVal strFURIHI As String) As Boolean
        '�����̃X�P�W���[�����̏������ʐ����ăJ�E���g���X�V
        Dim iGakunen(8) As Integer
        Dim iCount As Integer
        Dim bFlg As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '�ʏ탌�R�[�h�̑��݃`�F�b�N
        PFUNC_G_MEIMAST_COUNT_MOTO = True

        '�L�[�́A�w�Z�R�[�h�A�U�֋敪�A�U�֓�
        sql.Append(" SELECT * FROM G_MEIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_M ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_M = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_M ='" & strFURIHI & "'")

        If PFUNC_GAKUNEN_GET(iGakunen) = True Then
            sql.Append(" AND (")

            For iCount = 1 To 9
                If iGakunen(iCount - 1) = 1 Then
                    If bFlg = True Then
                        sql.Append(" OR ")
                    End If

                    sql.Append(" GAKUNEN_CODE_M = " & iCount)
                    bFlg = True
                End If
            Next iCount

            sql.Append(" )")
        End If

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                lngSYORI_KEN = lngSYORI_KEN + 1
                dblSYORI_KIN = dblSYORI_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                If oraReader.GetString("FURIKETU_CODE_M") = "0" Then
                    lngFURI_KEN = lngFURI_KEN + 1
                    dblFURI_KIN = dblFURI_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                Else
                    lngFUNOU_KEN = lngFUNOU_KEN + 1
                    dblFUNOU_KIN = dblFUNOU_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                End If

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()


        PFUNC_G_MEIMAST_COUNT_MOTO = False
        bFlg = False

        sql = New StringBuilder(128)

        '�w�N�w�肪�Ȃ��ꍇ�͏��������Ȃ�
        If PFUNC_GAKUNEN_GET(iGakunen) = True Then
            '���ʃ��R�[�h�̑Ώۊw�N�t���O�̏�Ԃ����ɒʏ탌�R�[�h�̑Ώۊw�N�t���O���n�e�e�ɂ���
            '�n�m�ɂ���@�\�����������ꍇ�A���ʃ��R�[�h�����������݂����ꍇ�ɑO���R�[�h�ł̏��������ʂɂȂ�
            '���ʃ��R�[�h�̑Ώۊw�N�P�t���O���u�P�v�̏ꍇ
            sql.Append(" UPDATE  G_SCHMAST")
            sql.Append(" SET ")
            sql.Append(" SYORI_KEN_S =" & lngSYORI_KEN & ",")
            sql.Append(" SYORI_KIN_S =" & dblSYORI_KIN & ",")
            sql.Append(" FURI_KEN_S =" & lngFURI_KEN & ",")
            sql.Append(" FURI_KIN_S =" & dblFURI_KIN & ",")
            sql.Append(" FUNOU_KEN_S =" & lngFUNOU_KEN & ",")
            sql.Append(" FUNOU_KIN_S =" & dblFUNOU_KIN)
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
            sql.Append(" AND")
            sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
            sql.Append(" AND")
            sql.Append(" SCH_KBN_S ='0'")
            sql.Append(" AND")
            sql.Append(" FURI_KBN_S ='" & strFURIKBN & "'")
            sql.Append(" AND")
            sql.Append(" FURI_DATE_S ='" & strFURIHI & "'")

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                '�X�V�����G���[
                MessageBox.Show("�X�P�W���[���}�X�^�̍X�V�����ŃG���[���������܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        End If

        Return True

    End Function

    Private Function PFUNC_G_MEIMAST_COUNT(ByVal strFURIKBN As String, ByVal strFURIHI As String) As Boolean
        '�f�[�^�t���O=1�̏ꍇ�͖��׃}�X�^���珈�������E���z���J�E���g
        '�s�\�t���O=1�̏ꍇ�͖��׃}�X�^����U�֍ς݌����E���z�A�s�\�����E���z���J�E���g
        Dim iGakunen(8) As Integer
        Dim iCount As Integer
        Dim bFlg As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        PFUNC_G_MEIMAST_COUNT = False

        '�L�[�́A�w�Z�R�[�h�A�U�֋敪�A�U�֓�
        sql.Append(" SELECT * FROM G_MEIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_M ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_M = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_M ='" & strFURIHI & "'")

        If PFUNC_GAKUNEN_GET(iGakunen) = True Then
            sql.Append(" AND (")

            For iCount = 1 To 9
                If iGakunen(iCount - 1) = 1 Then
                    If bFlg = True Then
                        sql.Append(" OR ")
                    End If

                    sql.Append(" GAKUNEN_CODE_M = " & iCount)
                    bFlg = True
                End If
            Next iCount

            sql.Append(" )")
        End If

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                lngSYORI_KEN = lngSYORI_KEN + 1
                dblSYORI_KIN = dblSYORI_KIN + CDbl(oraReader.GetString("SEIKYU_KIN_M"))
                If oraReader.GetString("FURIKETU_CODE_M") = "0" Then
                    lngFURI_KEN = lngFURI_KEN + 1
                    dblFURI_KIN = dblFURI_KIN + CDbl(oraReader.GetString("SEIKYU_KIN_M"))
                Else
                    lngFUNOU_KEN = lngFUNOU_KEN + 1
                    dblFUNOU_KIN = dblFUNOU_KIN + CDbl(oraReader.GetString("SEIKYU_KIN_M"))
                End If

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()

        Return True

    End Function


    Private Function PFUNC_FURIHI_HANI_CHECK() As Boolean

        '�U�֓��A�ĐU�֓��̌_����ԃ`�F�b�N
        PFUNC_FURIHI_HANI_CHECK = False

        ' 2016/05/06 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
        'If Mid(STR�U�֓�, 1, 6) >= GAKKOU_INFO.KAISI_DATE And Mid(STR�U�֓�, 1, 6) <= GAKKOU_INFO.SYURYOU_DATE Then
        'Else
        '    Exit Function
        'End If
        If STR�U�֓� >= GAKKOU_INFO.KAISI_DATE And STR�U�֓� <= GAKKOU_INFO.SYURYOU_DATE Then
        Else
            Exit Function
        End If
        ' 2016/05/06 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

        PFUNC_FURIHI_HANI_CHECK = True

    End Function
    Private Function PFUNC_SCHMAST_SERCH() As Boolean

        Dim sql As New StringBuilder(128)
        Dim orareader As New MyOracleReader(MainDB)
        Dim bret As Boolean = False

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & Trim(txtGAKKOU_CODE.Text) & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & Trim(txt�Ώ۔N�x.Text) & "04'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & CStr(CInt(Trim(txt�Ώ۔N�x.Text)) + 1) & "03'")

        If orareader.DataReader(sql) = True Then
            bret = True
        End If

        orareader.Close()

        Return bret

    End Function

    Private Function PFUNC_SCHMAST_UPDATE_SFURIDATE(ByVal pSCH_KBN_S As String) As Boolean

        Dim sql As New StringBuilder(128)

        '�������̏��U���X�P�W���[���ɂ��ĐU��������X�V�ł��Ȃ��̂�
        '�ĐU���쐬���Ă��鎞�Ɉꏏ�ɍX�V���s��
        sql.Append(" UPDATE  G_SCHMAST SET ")
        sql.Append(" SFURI_DATE_S ='" & Str_SFURI_DATE & "'")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='" & pSCH_KBN_S & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & Str_FURI_DATE & "'")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            Return False
        End If

        Return True

    End Function

    Private Function PFUNC_DELETE_GSCHMAST() As Boolean

        ' 2017/05/26 �^�X�N�j���� CHG �yME�z(RSV2�Ή� �N�ԃX�P�W���[���̍폜�����s���C��) -------------------- START
        '=========================================================================
        ' ���ʃX�P�W���[�����쐬�����ꍇ�A����̔N���x�̔N�ԃX�P�W���[����
        ' ���݂��Ă͂Ȃ�Ȃ����߁A�N�ԃX�P�W���[���͔N���x�P�ʂɍ폜����悤
        ' �ύX����B
        '=========================================================================
        'Dim sql As New StringBuilder(128)

        ''���ʃX�P�W���[�����쐬�������Ƃɂ��
        ''�N�ԂőΏۊw�N�����݂��Ȃ����R�[�h���쐬����Ă��܂���
        ''���ʂ̏����m���A�N�Ԃ̃X�P�W���[���Ŋw�N�t���O��ALLZERO��
        ''���R�[�h���폜����
        'sql.Append(" DELETE  FROM G_SCHMAST")
        'sql.Append(" WHERE")
        'sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        'sql.Append(" AND")
        'sql.Append(" SCH_KBN_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN1_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN2_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN3_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN4_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN5_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN6_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN7_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN8_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN9_FLG_S ='0'")

        'If MainDB.ExecuteNonQuery(sql) < 0 Then
        '    Return False
        'End If

        'Return True
        Dim SQL As New StringBuilder(128)
        Dim SQL_DEL_TOKUSCH As New StringBuilder(128)
        Dim OraReader_Tokubetu As CASTCommon.MyOracleReader = Nothing

        Try

            SQL.Length = 0
            SQL.Append(" SELECT ")
            SQL.Append("     NENGETUDO_S")
            SQL.Append(" FROM ")
            SQL.Append("     G_SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append("     GAKKOU_CODE_S = '" & GAKKOU_INFO.GAKKOU_CODE & "'")
            SQL.Append(" AND SCH_KBN_S     = '1'")
            SQL.Append(" GROUP BY ")
            SQL.Append("     NENGETUDO_S")
            SQL.Append(" ORDER BY ")
            SQL.Append("     NENGETUDO_S")

            OraReader_Tokubetu = New CASTCommon.MyOracleReader(MainDB)
            If OraReader_Tokubetu.DataReader(SQL) = False Then
                '=================================================================
                ' ���ʃX�P�W���[�������݂��Ȃ����߁A�폜�����s�v
                '=================================================================
                Return True
            Else
                '=================================================================
                ' ���ʃX�P�W���[�������݂��邽�߁A�폜�����J�n
                '=================================================================
                Do Until OraReader_Tokubetu.EOF
                    SQL_DEL_TOKUSCH.Length = 0
                    SQL_DEL_TOKUSCH.Append(" DELETE FROM G_SCHMAST ")
                    SQL_DEL_TOKUSCH.Append(" WHERE")
                    SQL_DEL_TOKUSCH.Append("     GAKKOU_CODE_S = '" & GAKKOU_INFO.GAKKOU_CODE & "'")
                    SQL_DEL_TOKUSCH.Append(" AND NENGETUDO_S   = '" & OraReader_Tokubetu.GetString("NENGETUDO_S") & "'")
                    SQL_DEL_TOKUSCH.Append(" AND SCH_KBN_S     = '0'")

                    If MainDB.ExecuteNonQuery(SQL_DEL_TOKUSCH) < 0 Then
                        Return False
                    Else
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�s�v�N�ԃX�P�W���[���폜", "����", "�N���x:" & OraReader_Tokubetu.GetString("NENGETUDO_S"))
                    End If

                    OraReader_Tokubetu.NextRead()
                Loop
            End If

            OraReader_Tokubetu.Close()

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�s�v�N�ԃX�P�W���[���폜", "���s", ex.Message)
            Return False
        Finally
            If Not OraReader_Tokubetu Is Nothing Then
                OraReader_Tokubetu.Close()
                OraReader_Tokubetu = Nothing
            End If
        End Try
        ' 2017/05/26 �^�X�N�j���� CHG �yME�z(RSV2�Ή� �N�ԃX�P�W���[���̍폜�����s���C��) -------------------- END

    End Function

    Private Function PFUNC_CHECK_SFURI() As Boolean
        '2006/10/12�@���U�ƍĐU���������łȂ����`�F�b�N����

        PFUNC_CHECK_SFURI = False

        '�N�ԃX�P�W���[�������`�F�b�N
        If (chk�S���U�֓�.Checked = True And chk�S���ĐU�֓�.Checked = True And txt�S���U�֓�.Text <> "" And txt�S���U�֓�.Text = txt�S���ĐU�֓�.Text) Or _
           (chk�T���U�֓�.Checked = True And chk�T���ĐU�֓�.Checked = True And txt�T���U�֓�.Text <> "" And txt�T���U�֓�.Text = txt�T���ĐU�֓�.Text) Or _
           (chk�U���U�֓�.Checked = True And chk�U���ĐU�֓�.Checked = True And txt�U���U�֓�.Text <> "" And txt�U���U�֓�.Text = txt�U���ĐU�֓�.Text) Or _
           (chk�V���U�֓�.Checked = True And chk�V���ĐU�֓�.Checked = True And txt�V���U�֓�.Text <> "" And txt�V���U�֓�.Text = txt�V���ĐU�֓�.Text) Or _
           (chk�W���U�֓�.Checked = True And chk�W���ĐU�֓�.Checked = True And txt�W���U�֓�.Text <> "" And txt�W���U�֓�.Text = txt�W���ĐU�֓�.Text) Or _
           (chk�X���U�֓�.Checked = True And chk�X���ĐU�֓�.Checked = True And txt�X���U�֓�.Text <> "" And txt�X���U�֓�.Text = txt�X���ĐU�֓�.Text) Or _
           (chk�P�O���U�֓�.Checked = True And chk�P�O���ĐU�֓�.Checked = True And txt�P�O���U�֓�.Text <> "" And txt�P�O���U�֓�.Text = txt�P�O���ĐU�֓�.Text) Or _
           (chk�P�P���U�֓�.Checked = True And chk�P�P���ĐU�֓�.Checked = True And txt�P�P���U�֓�.Text <> "" And txt�P�P���U�֓�.Text = txt�P�P���ĐU�֓�.Text) Or _
           (chk�P�Q���U�֓�.Checked = True And chk�P�Q���ĐU�֓�.Checked = True And txt�P�Q���U�֓�.Text <> "" And txt�P�Q���U�֓�.Text = txt�P�Q���ĐU�֓�.Text) Or _
           (chk�P���U�֓�.Checked = True And chk�P���ĐU�֓�.Checked = True And txt�P���U�֓�.Text <> "" And txt�P���U�֓�.Text = txt�P���ĐU�֓�.Text) Or _
           (chk�Q���U�֓�.Checked = True And chk�Q���ĐU�֓�.Checked = True And txt�Q���U�֓�.Text <> "" And txt�Q���U�֓�.Text = txt�Q���ĐU�֓�.Text) Or _
           (chk�R���U�֓�.Checked = True And chk�R���ĐU�֓�.Checked = True And txt�R���U�֓�.Text <> "" And txt�R���U�֓�.Text = txt�R���ĐU�֓�.Text) Then

            MessageBox.Show("�U�֓��ƍĐU�֓����������̂�����܂�", "�N�ԃX�P�W���[��", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function

        End If

        '���ʐU�֓������`�F�b�N
        If (txt���ʐ������P.Text <> "" And txt���ʐU�֌��P.Text & txt���ʐU�֓��P.Text <> "" And txt���ʐU�֌��P.Text & txt���ʐU�֓��P.Text = txt���ʍĐU�֌��P.Text & txt���ʍĐU�֓��P.Text) Or _
           (txt���ʐ������Q.Text <> "" And txt���ʐU�֌��Q.Text & txt���ʐU�֓��Q.Text <> "" And txt���ʐU�֌��Q.Text & txt���ʐU�֓��Q.Text = txt���ʍĐU�֌��Q.Text & txt���ʍĐU�֓��Q.Text) Or _
           (txt���ʐ������R.Text <> "" And txt���ʐU�֌��R.Text & txt���ʐU�֓��R.Text <> "" And txt���ʐU�֌��R.Text & txt���ʐU�֓��R.Text = txt���ʍĐU�֌��R.Text & txt���ʍĐU�֓��R.Text) Or _
           (txt���ʐ������S.Text <> "" And txt���ʐU�֌��S.Text & txt���ʐU�֓��S.Text <> "" And txt���ʐU�֌��S.Text & txt���ʐU�֓��S.Text = txt���ʍĐU�֌��S.Text & txt���ʍĐU�֓��S.Text) Or _
           (txt���ʐ������T.Text <> "" And txt���ʐU�֌��T.Text & txt���ʐU�֓��T.Text <> "" And txt���ʐU�֌��T.Text & txt���ʐU�֓��T.Text = txt���ʍĐU�֌��T.Text & txt���ʍĐU�֓��T.Text) Or _
           (txt���ʐ������U.Text <> "" And txt���ʐU�֌��U.Text & txt���ʐU�֓��U.Text <> "" And txt���ʐU�֌��U.Text & txt���ʐU�֓��U.Text = txt���ʍĐU�֌��U.Text & txt���ʍĐU�֓��U.Text) Then

            MessageBox.Show("�U�֓��ƍĐU�֓����������̂�����܂�", "���ʐU�֓�", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function

        End If

        '2007/02/12KG �G���ʐU�֓��ŁA���ꌎ�̏��U���ƍĐU��������̏ꍇERR�Ƃ݂Ȃ��B
        '****************************************************************************
        If (txt���ʐ������P.Text <> "" And txt���ʐU�֌��Q.Text <> "") And ((txt���ʐ������P.Text = txt���ʐU�֌��Q.Text) And (txt���ʐU�֌��P.Text & txt���ʐU�֓��P.Text = txt���ʍĐU�֌��Q.Text & txt���ʍĐU�֓��Q.Text) Or (txt���ʐU�֌��Q.Text & txt���ʐU�֓��Q.Text = txt���ʍĐU�֌��P.Text & txt���ʍĐU�֓��P.Text)) Or _
            (txt���ʐ������P.Text <> "" And txt���ʐU�֌��R.Text <> "") And ((txt���ʐ������P.Text = txt���ʐU�֌��R.Text) And (txt���ʐU�֌��P.Text & txt���ʐU�֓��P.Text = txt���ʍĐU�֌��R.Text & txt���ʍĐU�֓��R.Text) Or (txt���ʐU�֌��R.Text & txt���ʐU�֓��R.Text = txt���ʍĐU�֌��P.Text & txt���ʍĐU�֓��P.Text)) Or _
            (txt���ʐ������P.Text <> "" And txt���ʐU�֌��S.Text <> "") And ((txt���ʐ������P.Text = txt���ʐU�֌��S.Text) And (txt���ʐU�֌��P.Text & txt���ʐU�֓��P.Text = txt���ʍĐU�֌��S.Text & txt���ʍĐU�֓��S.Text) Or (txt���ʐU�֌��S.Text & txt���ʐU�֓��S.Text = txt���ʍĐU�֌��P.Text & txt���ʍĐU�֓��P.Text)) Or _
            (txt���ʐ������P.Text <> "" And txt���ʐU�֌��T.Text <> "") And ((txt���ʐ������P.Text = txt���ʐU�֌��T.Text) And (txt���ʐU�֌��P.Text & txt���ʐU�֓��P.Text = txt���ʍĐU�֌��T.Text & txt���ʍĐU�֓��T.Text) Or (txt���ʐU�֌��T.Text & txt���ʐU�֓��T.Text = txt���ʍĐU�֌��P.Text & txt���ʍĐU�֓��P.Text)) Or _
           (txt���ʐ������P.Text <> "" And txt���ʐU�֌��U.Text <> "") And ((txt���ʐ������P.Text = txt���ʐU�֌��U.Text) And (txt���ʐU�֌��P.Text & txt���ʐU�֓��P.Text = txt���ʍĐU�֌��U.Text & txt���ʍĐU�֓��U.Text) Or (txt���ʐU�֌��U.Text & txt���ʐU�֓��U.Text = txt���ʍĐU�֌��P.Text & txt���ʍĐU�֓��P.Text)) Or _
            (txt���ʐ������Q.Text <> "" And txt���ʐU�֌��R.Text <> "") And ((txt���ʐ������Q.Text = txt���ʐU�֌��R.Text) And (txt���ʐU�֌��Q.Text & txt���ʐU�֓��Q.Text = txt���ʍĐU�֌��R.Text & txt���ʍĐU�֓��R.Text) Or (txt���ʐU�֌��R.Text & txt���ʐU�֓��R.Text = txt���ʍĐU�֌��Q.Text & txt���ʍĐU�֓��Q.Text)) Or _
            (txt���ʐ������Q.Text <> "" And txt���ʐU�֌��S.Text <> "") And ((txt���ʐ������Q.Text = txt���ʐU�֌��S.Text) And (txt���ʐU�֌��Q.Text & txt���ʐU�֓��Q.Text = txt���ʍĐU�֌��S.Text & txt���ʍĐU�֓��S.Text) Or (txt���ʐU�֌��S.Text & txt���ʐU�֓��S.Text = txt���ʍĐU�֌��Q.Text & txt���ʍĐU�֓��Q.Text)) Or _
            (txt���ʐ������Q.Text <> "" And txt���ʐU�֌��T.Text <> "") And ((txt���ʐ������Q.Text = txt���ʐU�֌��T.Text) And (txt���ʐU�֌��Q.Text & txt���ʐU�֓��Q.Text = txt���ʍĐU�֌��T.Text & txt���ʍĐU�֓��T.Text) Or (txt���ʐU�֌��T.Text & txt���ʐU�֓��T.Text = txt���ʍĐU�֌��Q.Text & txt���ʍĐU�֓��Q.Text)) Or _
            (txt���ʐ������Q.Text <> "" And txt���ʐU�֌��U.Text <> "") And ((txt���ʐ������Q.Text = txt���ʐU�֌��U.Text) And (txt���ʐU�֌��Q.Text & txt���ʐU�֓��Q.Text = txt���ʍĐU�֌��U.Text & txt���ʍĐU�֓��U.Text) Or (txt���ʐU�֌��U.Text & txt���ʐU�֓��U.Text = txt���ʍĐU�֌��Q.Text & txt���ʍĐU�֓��Q.Text)) Or _
            (txt���ʐ������R.Text <> "" And txt���ʐU�֌��S.Text <> "") And ((txt���ʐ������R.Text = txt���ʐU�֌��S.Text) And (txt���ʐU�֌��R.Text & txt���ʐU�֓��R.Text = txt���ʍĐU�֌��S.Text & txt���ʍĐU�֓��S.Text) Or (txt���ʐU�֌��S.Text & txt���ʐU�֓��S.Text = txt���ʍĐU�֌��R.Text & txt���ʍĐU�֓��R.Text)) Or _
            (txt���ʐ������R.Text <> "" And txt���ʐU�֌��T.Text <> "") And ((txt���ʐ������R.Text = txt���ʐU�֌��T.Text) And (txt���ʐU�֌��R.Text & txt���ʐU�֓��R.Text = txt���ʍĐU�֌��T.Text & txt���ʍĐU�֓��T.Text) Or (txt���ʐU�֌��T.Text & txt���ʐU�֓��T.Text = txt���ʍĐU�֌��R.Text & txt���ʍĐU�֓��R.Text)) Or _
            (txt���ʐ������R.Text <> "" And txt���ʐU�֌��U.Text <> "") And ((txt���ʐ������R.Text = txt���ʐU�֌��U.Text) And (txt���ʐU�֌��R.Text & txt���ʐU�֓��R.Text = txt���ʍĐU�֌��U.Text & txt���ʍĐU�֓��U.Text) Or (txt���ʐU�֌��U.Text & txt���ʐU�֓��U.Text = txt���ʍĐU�֌��R.Text & txt���ʍĐU�֓��R.Text)) Or _
            (txt���ʐ������S.Text <> "" And txt���ʐU�֌��T.Text <> "") And ((txt���ʐ������S.Text = txt���ʐU�֌��S.Text) And (txt���ʐU�֌��S.Text & txt���ʐU�֓��S.Text = txt���ʍĐU�֌��T.Text & txt���ʍĐU�֓��T.Text) Or (txt���ʐU�֌��T.Text & txt���ʐU�֓��T.Text = txt���ʍĐU�֌��S.Text & txt���ʍĐU�֓��S.Text)) Or _
            (txt���ʐ������S.Text <> "" And txt���ʐU�֌��U.Text <> "") And ((txt���ʐ������S.Text = txt���ʐU�֌��S.Text) And (txt���ʐU�֌��S.Text & txt���ʐU�֓��S.Text = txt���ʍĐU�֌��U.Text & txt���ʍĐU�֓��U.Text) Or (txt���ʐU�֌��U.Text & txt���ʐU�֓��U.Text = txt���ʍĐU�֌��S.Text & txt���ʍĐU�֓��S.Text)) Or _
            (txt���ʐ������T.Text <> "" And txt���ʐU�֌��U.Text <> "") And ((txt���ʐ������T.Text = txt���ʐU�֌��T.Text) And (txt���ʐU�֌��T.Text & txt���ʐU�֓��T.Text = txt���ʍĐU�֌��U.Text & txt���ʍĐU�֓��U.Text) Or (txt���ʐU�֌��U.Text & txt���ʐU�֓��U.Text = txt���ʍĐU�֌��T.Text & txt���ʍĐU�֓��T.Text)) Then


            MessageBox.Show("���ꌎ�Ţ���U����Ƣ�ĐU������d�����Ă��܂��B", "���ʐU�֓�", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function

        End If
        '****************************************************************************

        PFUNC_CHECK_SFURI = True

    End Function

    '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    '���ʃX�P�W���[���`�F�b�N 2006/11/22
    '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    Private Function PFUNC_CHECK_TOKUBETSU() As Boolean
        PFUNC_CHECK_TOKUBETSU = False

        '------------------------------------------
        '����U�֓��̓o�^�͂ł��Ȃ�
        '------------------------------------------
        Dim blnCHECK(13) As Boolean ' �U�֎��s�`�F�b�N
        Dim blnSCHECK(13) As Boolean '�ĐU���s�`�F�b�N
        Dim strNyuuryoku(13) As String ' �U�֓����ɓ��͂��ꂽ�l
        Dim strSNyuuryoku(13) As String '�ĐU�����ɓ��͂��ꂽ�l
        Dim strTsuujyou(13) As String '�ʏ�X�P�W���[��
        Dim strTokubetsu(6) As String '���ʃX�P�W���[��

        '�c�Ɠ����擾���A�������E���U�E�ĐU���P�̕�����Ɍ���
        '���ʏ�X�P�W���[�����istrTsuujyou�j
        PFUNC_SET_EIGYOBI(chk�S���U�֓�.Checked, "04", Trim(txt�Ώ۔N�x.Text), "04", Trim(txt�S���U�֓�.Text), chk�S���ĐU�֓�.Checked, Trim(txt�Ώ۔N�x.Text), "04", Trim(txt�S���ĐU�֓�.Text), True, strTsuujyou(4))
        PFUNC_SET_EIGYOBI(chk�T���U�֓�.Checked, "05", Trim(txt�Ώ۔N�x.Text), "05", Trim(txt�T���U�֓�.Text), chk�T���ĐU�֓�.Checked, Trim(txt�Ώ۔N�x.Text), "05", Trim(txt�T���ĐU�֓�.Text), True, strTsuujyou(5))
        PFUNC_SET_EIGYOBI(chk�U���U�֓�.Checked, "06", Trim(txt�Ώ۔N�x.Text), "06", Trim(txt�U���U�֓�.Text), chk�U���ĐU�֓�.Checked, Trim(txt�Ώ۔N�x.Text), "06", Trim(txt�U���ĐU�֓�.Text), True, strTsuujyou(6))
        PFUNC_SET_EIGYOBI(chk�V���U�֓�.Checked, "07", Trim(txt�Ώ۔N�x.Text), "07", Trim(txt�V���U�֓�.Text), chk�V���ĐU�֓�.Checked, Trim(txt�Ώ۔N�x.Text), "07", Trim(txt�V���ĐU�֓�.Text), True, strTsuujyou(7))
        PFUNC_SET_EIGYOBI(chk�W���U�֓�.Checked, "08", Trim(txt�Ώ۔N�x.Text), "08", Trim(txt�W���U�֓�.Text), chk�W���ĐU�֓�.Checked, Trim(txt�Ώ۔N�x.Text), "08", Trim(txt�W���ĐU�֓�.Text), True, strTsuujyou(8))
        PFUNC_SET_EIGYOBI(chk�X���U�֓�.Checked, "09", Trim(txt�Ώ۔N�x.Text), "09", Trim(txt�X���U�֓�.Text), chk�X���ĐU�֓�.Checked, Trim(txt�Ώ۔N�x.Text), "09", Trim(txt�X���ĐU�֓�.Text), True, strTsuujyou(9))
        PFUNC_SET_EIGYOBI(chk�P�O���U�֓�.Checked, "10", Trim(txt�Ώ۔N�x.Text), "10", Trim(txt�P�O���U�֓�.Text), chk�P�O���ĐU�֓�.Checked, Trim(txt�Ώ۔N�x.Text), "10", Trim(txt�P�O���ĐU�֓�.Text), True, strTsuujyou(10))
        PFUNC_SET_EIGYOBI(chk�P�P���U�֓�.Checked, "11", Trim(txt�Ώ۔N�x.Text), "11", Trim(txt�P�P���U�֓�.Text), chk�P�P���ĐU�֓�.Checked, Trim(txt�Ώ۔N�x.Text), "11", Trim(txt�P�P���ĐU�֓�.Text), True, strTsuujyou(11))
        PFUNC_SET_EIGYOBI(chk�P�Q���U�֓�.Checked, "12", Trim(txt�Ώ۔N�x.Text), "12", Trim(txt�P�Q���U�֓�.Text), chk�P�Q���ĐU�֓�.Checked, Trim(txt�Ώ۔N�x.Text), "12", Trim(txt�P�Q���ĐU�֓�.Text), True, strTsuujyou(12))
        PFUNC_SET_EIGYOBI(chk�P���U�֓�.Checked, "01", Trim(txt�Ώ۔N�x.Text), "01", Trim(txt�P���U�֓�.Text), chk�P���ĐU�֓�.Checked, Trim(txt�Ώ۔N�x.Text), "01", Trim(txt�P���ĐU�֓�.Text), True, strTsuujyou(1))
        PFUNC_SET_EIGYOBI(chk�Q���U�֓�.Checked, "02", Trim(txt�Ώ۔N�x.Text), "02", Trim(txt�Q���U�֓�.Text), chk�Q���ĐU�֓�.Checked, Trim(txt�Ώ۔N�x.Text), "02", Trim(txt�Q���ĐU�֓�.Text), True, strTsuujyou(2))
        PFUNC_SET_EIGYOBI(chk�R���U�֓�.Checked, "03", Trim(txt�Ώ۔N�x.Text), "03", Trim(txt�R���U�֓�.Text), chk�R���ĐU�֓�.Checked, Trim(txt�Ώ۔N�x.Text), "03", Trim(txt�R���ĐU�֓�.Text), True, strTsuujyou(3))

        '�����ʃX�P�W���[�����istrTokubetsu�j
        PFUNC_SET_EIGYOBI(True, Trim(txt���ʐ������P.Text), Trim(txt�Ώ۔N�x.Text), Trim(txt���ʐU�֌��P.Text), Trim(txt���ʐU�֓��P.Text), True, Trim(txt�Ώ۔N�x.Text), Trim(txt���ʍĐU�֌��P.Text), Trim(txt���ʍĐU�֓��P.Text), False, strTokubetsu(0))
        PFUNC_SET_EIGYOBI(True, Trim(txt���ʐ������Q.Text), Trim(txt�Ώ۔N�x.Text), Trim(txt���ʐU�֌��Q.Text), Trim(txt���ʐU�֓��Q.Text), True, Trim(txt�Ώ۔N�x.Text), Trim(txt���ʍĐU�֌��Q.Text), Trim(txt���ʍĐU�֓��Q.Text), False, strTokubetsu(1))
        PFUNC_SET_EIGYOBI(True, Trim(txt���ʐ������R.Text), Trim(txt�Ώ۔N�x.Text), Trim(txt���ʐU�֌��R.Text), Trim(txt���ʐU�֓��R.Text), True, Trim(txt�Ώ۔N�x.Text), Trim(txt���ʍĐU�֌��R.Text), Trim(txt���ʍĐU�֓��R.Text), False, strTokubetsu(2))
        PFUNC_SET_EIGYOBI(True, Trim(txt���ʐ������S.Text), Trim(txt�Ώ۔N�x.Text), Trim(txt���ʐU�֌��S.Text), Trim(txt���ʐU�֓��S.Text), True, Trim(txt�Ώ۔N�x.Text), Trim(txt���ʍĐU�֌��S.Text), Trim(txt���ʍĐU�֓��S.Text), False, strTokubetsu(3))
        PFUNC_SET_EIGYOBI(True, Trim(txt���ʐ������T.Text), Trim(txt�Ώ۔N�x.Text), Trim(txt���ʐU�֌��T.Text), Trim(txt���ʐU�֓��T.Text), True, Trim(txt�Ώ۔N�x.Text), Trim(txt���ʍĐU�֌��T.Text), Trim(txt���ʍĐU�֓��T.Text), False, strTokubetsu(4))
        PFUNC_SET_EIGYOBI(True, Trim(txt���ʐ������U.Text), Trim(txt�Ώ۔N�x.Text), Trim(txt���ʐU�֌��U.Text), Trim(txt���ʐU�֓��U.Text), True, Trim(txt�Ώ۔N�x.Text), Trim(txt���ʍĐU�֌��U.Text), Trim(txt���ʍĐU�֓��U.Text), False, strTokubetsu(5))

        '�ʏ�X�P�W���[���Ɠ��ʃX�P�W���[���̃`�F�b�N
        For i As Integer = 0 To 5
            If Trim(strTokubetsu(i)) <> "" Then '�����͂̏ꍇ�A�`�F�b�N�̕K�v�Ȃ�
                '��strTokubetsu(i).Substring(0, 2)�͐�����
                '2010/10/21 strTsuujyou�ɂ͐U�֓��{�ĐU���������Ă���ꍇ������̂ōl������
                'If strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))) = strTokubetsu(i) Then
                If strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))) IsNot Nothing AndAlso _
                   strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))).PadRight(16).Substring(0, 8) = strTokubetsu(i).Substring(0, 8) Then
                    MessageBox.Show("�ʏ�X�P�W���[���Ɠ��ʃX�P�W���[���ɓ���U�֓��̃f�[�^�����݂��܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If

                '2010/10/21 �ĐU���`�F�b�N���� ��������
                If strTokubetsu(i).Length = 16 Then
                    If strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))) IsNot Nothing AndAlso _
                       strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))).PadRight(16).Substring(8, 8) = strTokubetsu(i).Substring(8, 8) Then
                        MessageBox.Show("�ʏ�X�P�W���[���Ɠ��ʃX�P�W���[���ɓ���ĐU�֓��̃f�[�^�����݂��܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                    End If
                End If
                '2010/10/21 �ĐU���`�F�b�N���� �����܂�
            End If
        Next

        '���ʃX�P�W���[�����m�̃`�F�b�N
        For i As Integer = 0 To 4
            If strTokubetsu(i) <> "" Then '�����͂̏ꍇ�A�`�F�b�N�̕K�v�Ȃ�
                For j As Integer = i + 1 To 5
                    If strTokubetsu(i) = strTokubetsu(j) Then
                        MessageBox.Show("���ʃX�P�W���[���ɓ���U�֓��̃f�[�^�����݂��܂��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                    End If
                Next
            End If
        Next

        PFUNC_CHECK_TOKUBETSU = True

    End Function

    '2010/10/21
    '�����X�P�W���[���`�F�b�N
    Private Function PFUNC_CHECK_ZUIJI() As Boolean
        PFUNC_CHECK_ZUIJI = False

        '------------------------------------------
        '������o�敪�A����U�֓��̓o�^�͂ł��Ȃ�
        '------------------------------------------
        Dim strZuiji(6) As String '�����X�P�W���[��
        Dim intNsKbn(6) As Integer
        intNsKbn(0) = cmb���o�敪�P.SelectedIndex
        intNsKbn(1) = cmb���o�敪�Q.SelectedIndex
        intNsKbn(2) = cmb���o�敪�R.SelectedIndex
        intNsKbn(3) = cmb���o�敪�S.SelectedIndex
        intNsKbn(4) = cmb���o�敪�T.SelectedIndex
        intNsKbn(5) = cmb���o�敪�U.SelectedIndex

        '�c�Ɠ����擾
        PFUNC_SET_EIGYOBI(True, txt�����U�֌��P.Text.Trim, txt�Ώ۔N�x.Text.Trim, txt�����U�֌��P.Text.Trim, txt�����U�֓��P.Text.Trim, True, txt�Ώ۔N�x.Text.Trim, txt�����U�֌��P.Text.Trim, txt�����U�֓��P.Text.Trim, False, strZuiji(0))
        PFUNC_SET_EIGYOBI(True, txt�����U�֌��Q.Text.Trim, txt�Ώ۔N�x.Text.Trim, txt�����U�֌��Q.Text.Trim, txt�����U�֓��Q.Text.Trim, True, txt�Ώ۔N�x.Text.Trim, txt�����U�֌��Q.Text.Trim, txt�����U�֓��Q.Text.Trim, False, strZuiji(1))
        PFUNC_SET_EIGYOBI(True, txt�����U�֌��R.Text.Trim, txt�Ώ۔N�x.Text.Trim, txt�����U�֌��R.Text.Trim, txt�����U�֓��R.Text.Trim, True, txt�Ώ۔N�x.Text.Trim, txt�����U�֌��R.Text.Trim, txt�����U�֓��R.Text.Trim, False, strZuiji(2))
        PFUNC_SET_EIGYOBI(True, txt�����U�֌��S.Text.Trim, txt�Ώ۔N�x.Text.Trim, txt�����U�֌��S.Text.Trim, txt�����U�֓��S.Text.Trim, True, txt�Ώ۔N�x.Text.Trim, txt�����U�֌��S.Text.Trim, txt�����U�֓��S.Text.Trim, False, strZuiji(3))
        PFUNC_SET_EIGYOBI(True, txt�����U�֌��T.Text.Trim, txt�Ώ۔N�x.Text.Trim, txt�����U�֌��T.Text.Trim, txt�����U�֓��T.Text.Trim, True, txt�Ώ۔N�x.Text.Trim, txt�����U�֌��T.Text.Trim, txt�����U�֓��T.Text.Trim, False, strZuiji(4))
        PFUNC_SET_EIGYOBI(True, txt�����U�֌��U.Text.Trim, txt�Ώ۔N�x.Text.Trim, txt�����U�֌��U.Text.Trim, txt�����U�֓��U.Text.Trim, True, txt�Ώ۔N�x.Text.Trim, txt�����U�֌��U.Text.Trim, txt�����U�֓��U.Text.Trim, False, strZuiji(5))

        '�����X�P�W���[�����m�̃`�F�b�N
        For i As Integer = 0 To 4
            If strZuiji(i) <> "" Then '�����͂̏ꍇ�A�`�F�b�N�̕K�v�Ȃ�
                For j As Integer = i + 1 To 5
                    If intNsKbn(i) = intNsKbn(j) AndAlso strZuiji(i) = strZuiji(j) Then
                        MessageBox.Show("�����X�P�W���[���ɓ�����o�敪�A����U�֓��̃f�[�^�����݂��܂��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                    End If
                Next
            End If
        Next

        PFUNC_CHECK_ZUIJI = True

    End Function

    '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    '�c�Ɠ��擾 2006/11/22
    '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    Function PFUNC_SET_EIGYOBI(ByVal blnBOX As Boolean, ByVal strSeikyuTuki As String, ByVal strFuridateY As String, ByVal strFuridateM As String, ByVal strFuridateD As String, ByVal blnSBOX As Boolean, ByVal strSFuridateY As String, ByVal strSFuridateM As String, ByVal strSFuridateD As String, ByVal blnCheckFLG As Boolean, ByRef strReturnDate As String) As Boolean
        Dim strEigyobiY As String = "" '�U�։c�ƔN
        Dim strEigyobiM As String = "" '�U�։c�ƌ�
        Dim strEigyobiD As String = "" '�U�։c�Ɠ�
        Dim strSEigyobiY As String = "" '�ĐU�c�ƔN
        Dim strSEigyobiM As String = "" '�ĐU�c�ƌ�
        Dim strSEigyobiD As String = "" '�ĐU�c�Ɠ�

        '���������󔒂̏ꍇ�E�U�ւ��Ȃ��ꍇ�A�擾����K�v�Ȃ�
        If strSeikyuTuki = "" Or blnBOX = False Then
            Exit Function
        End If

        '���������P�`�R���̏ꍇ�͔N�x��ς���
        If CInt(strSeikyuTuki) <= 3 Then
            strFuridateY = CStr(CInt(strFuridateY + 1))
            strSFuridateY = CStr(CInt(strSFuridateY + 1))
        End If

        '���t���󔒂������ꍇ�A������g�p����
        If blnCheckFLG = True Then
            If strFuridateD = "" Then
                strFuridateD = GAKKOU_INFO.FURI_DATE
            End If

            If blnSBOX = True And strSFuridateD = "" Then
                strSFuridateD = GAKKOU_INFO.SFURI_DATE
            End If
        End If

        '�c�Ɠ����擾
        Dim FuriDate As String = fn_GetEigyoubi(strFuridateY & strFuridateM & strFuridateD, "0", "+")
        Dim SFuriDate As String = fn_GetEigyoubi(strSFuridateY & strSFuridateM & strSFuridateD, "0", "+")

        'START 20121114 maeda �C�� �ĐU�֓��������͎��̍l����ǉ�
        '2011/06/15 �W���ŏC�� �_��U�֓��ƌ_��ĐU�����t�]����ꍇ�͗����̍ĐU���ɂ��� -------------START
        If SFuriDate <> "" Then
            If FuriDate >= SFuriDate Then
                If strSFuridateM = "12" Then
                    strSFuridateY = (CInt(strSFuridateY) + 1).ToString("0000")
                    strSFuridateM = "01"
                Else
                    strSFuridateM = (CInt(strSFuridateM) + 1).ToString("00")
                End If
                SFuriDate = fn_GetEigyoubi(strSFuridateY & strSFuridateM & strSFuridateD, "0", "+")
            End If
        End If
        '2011/06/15 �W���ŏC�� �_��U�֓��ƌ_��ĐU�����t�]����ꍇ�͗����̍ĐU���ɂ��� -------------END
        'END   20121114 maeda �C�� �ĐU�֓��������͎��̍l����ǉ�

        '�ĐU�X�P�W���[���i�ʏ�X�P�W���[���ƌ������A�P�̕ϐ��Ƃ��ĕԂ��j
        strReturnDate = FuriDate & SFuriDate

    End Function

    '��Ǝ��U�A�g���� 2006/12/06
    Public Function fn_CHECK_CHANGE() As Boolean
        '================================================================
        '�ޔ������Q�Ǝ��̃X�P�W���[�����X�V��̕ϐ��Ɏc���Ă��邩�`�F�b�N
        '�X�V��Ɏc���Ă��Ȃ��ꍇ=�폜���ꂽ�̂Ŋ�Ǝ��U���̃X�P�W���[�����폜
        '================================================================

        fn_CHECK_CHANGE = False

        '�N�ԃX�P�W���[���X�V
        For i As Integer = 1 To 12
            '���U�N�ԃ`�F�b�N
            If strSYOFURI_NENKAN(i).Length = 8 And strSYOFURI_NENKAN(i) <> strSYOFURI_NENKAN_AFTER(i) Then

                For j As Integer = 1 To 6
                    '���ʐU�֓��ƈ�v����U�֓�������ꍇ�A�폜���Ȃ��̂Ń��[�v�𔲂���
                    If strSYOFURI_NENKAN(i) = strSYOFURI_TOKUBETU_AFTER(j) And strSYOFURI_TOKUBETU_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 6 Then '���ʐU�֓������I��
                        If fn_DELETESCHMAST("01", strSYOFURI_NENKAN(i)) = False Then
                            Exit Function
                        End If
                    End If
                Next

            End If
            '�ĐU�N�ԃ`�F�b�N
            If strSAIFURI_NENKAN(i).Length = 8 And strSAIFURI_NENKAN(i) <> strSAIFURI_NENKAN_AFTER(i) Then
                For j As Integer = 1 To 6
                    '���ʐU�֓��ƈ�v����U�֓�������ꍇ�A�폜���Ȃ��̂Ń��[�v�𔲂���
                    If strSAIFURI_NENKAN(i) = strSAIFURI_TOKUBETU_AFTER(j) And strSAIFURI_TOKUBETU_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 6 Then '���ʐU�֓������I��
                        If fn_DELETESCHMAST("02", strSAIFURI_NENKAN(i)) = False Then
                            Exit Function
                        End If
                    End If
                Next

            End If
        Next

        '���ʍX�V
        For i As Integer = 1 To 6
            '���U���ʃ`�F�b�N
            If strSYOFURI_TOKUBETU(i).Length = 8 And strSYOFURI_TOKUBETU(i) <> strSYOFURI_TOKUBETU_AFTER(i) Then
                For j As Integer = 1 To 12
                    '�N�ԐU�֓��ƈ�v����U�֓�������ꍇ�A�폜���Ȃ��̂Ń��[�v�𔲂���
                    If strSYOFURI_TOKUBETU(i) = strSYOFURI_NENKAN_AFTER(j) And strSYOFURI_NENKAN_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 12 Then '�N�ԐU�֓������I��
                        If fn_DELETESCHMAST("01", strSYOFURI_TOKUBETU(i)) = False Then
                            Exit Function
                        End If
                    End If
                Next
            End If
            '�ĐU���ʃ`�F�b�N
            If strSAIFURI_TOKUBETU(i).Length = 8 And strSAIFURI_TOKUBETU(i) <> strSAIFURI_TOKUBETU_AFTER(i) Then
                For j As Integer = 1 To 12
                    '�N�ԐU�֓��ƈ�v����U�֓�������ꍇ�A�폜���Ȃ��̂Ń��[�v�𔲂���
                    If strSAIFURI_TOKUBETU(i) = strSAIFURI_NENKAN_AFTER(j) And strSAIFURI_NENKAN_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 12 Then '�N�ԐU�֓������I��
                        If fn_DELETESCHMAST("02", strSAIFURI_TOKUBETU(i)) = False Then
                            Exit Function
                        End If
                    End If
                Next
            End If
        Next

        '�����X�V
        For i As Integer = 1 To 6
            If strFURI_ZUIJI(i).Length = 8 And strFURIKBN_ZUIJI(i) & strFURI_ZUIJI(i) <> strFURIKBN_ZUIJI_AFTER(i) & strFURI_ZUIJI_AFTER(i) Then
                For j As Integer = 1 To 6
                    If strFURIKBN_ZUIJI(i) & strFURI_ZUIJI(i) = strFURIKBN_ZUIJI_AFTER(j) & strFURI_ZUIJI_AFTER(j) And strFURI_ZUIJI_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 6 Then
                        If strFURIKBN_ZUIJI(i) = "2" Then '����
                            If fn_DELETESCHMAST("03", strFURI_ZUIJI(i)) = False Then
                                Exit Function
                            End If
                        Else '�o��
                            If fn_DELETESCHMAST("04", strFURI_ZUIJI(i)) = False Then
                                Exit Function
                            End If
                        End If
                    End If

                Next
            End If
        Next

        If Err.Number = 0 Then
            fn_CHECK_CHANGE = True
        End If
    End Function

#End Region

#Region " Private Sub(�N�ԃX�P�W���[��)"
    Private Sub PSUB_NENKAN_GET(ByRef Get_Data() As NenkanData)

        Get_Data(4).Furikae_Check = chk�S���U�֓�.Checked
        Get_Data(4).Furikae_Enabled = chk�S���U�֓�.Enabled
        Get_Data(4).Furikae_Date = txt�S���U�֓�.Text
        Get_Data(4).Furikae_Day = lab�S���U�֓�.Text

        Get_Data(4).SaiFurikae_Check = chk�S���ĐU�֓�.Checked
        Get_Data(4).SaiFurikae_Enabled = chk�S���ĐU�֓�.Enabled
        Get_Data(4).SaiFurikae_Date = txt�S���ĐU�֓�.Text
        Get_Data(4).SaiFurikae_Day = lab�S���ĐU�֓�.Text

        Get_Data(5).Furikae_Check = chk�T���U�֓�.Checked
        Get_Data(5).Furikae_Enabled = chk�T���U�֓�.Enabled
        Get_Data(5).Furikae_Date = txt�T���U�֓�.Text
        Get_Data(5).Furikae_Day = lab�T���U�֓�.Text

        Get_Data(5).SaiFurikae_Check = chk�T���ĐU�֓�.Checked
        Get_Data(5).SaiFurikae_Enabled = chk�T���ĐU�֓�.Enabled
        Get_Data(5).SaiFurikae_Date = txt�T���ĐU�֓�.Text
        Get_Data(5).SaiFurikae_Day = lab�T���ĐU�֓�.Text

        Get_Data(6).Furikae_Check = chk�U���U�֓�.Checked
        Get_Data(6).Furikae_Enabled = chk�U���U�֓�.Enabled
        Get_Data(6).Furikae_Date = txt�U���U�֓�.Text
        Get_Data(6).Furikae_Day = lab�U���U�֓�.Text

        Get_Data(6).SaiFurikae_Check = chk�U���ĐU�֓�.Checked
        Get_Data(6).SaiFurikae_Enabled = chk�U���ĐU�֓�.Enabled
        Get_Data(6).SaiFurikae_Date = txt�U���ĐU�֓�.Text
        Get_Data(6).SaiFurikae_Day = lab�U���ĐU�֓�.Text

        Get_Data(7).Furikae_Check = chk�V���U�֓�.Checked
        Get_Data(7).Furikae_Enabled = chk�V���U�֓�.Enabled
        Get_Data(7).Furikae_Date = txt�V���U�֓�.Text
        Get_Data(7).Furikae_Day = lab�V���U�֓�.Text

        Get_Data(7).SaiFurikae_Check = chk�V���ĐU�֓�.Checked
        Get_Data(7).SaiFurikae_Enabled = chk�V���ĐU�֓�.Enabled
        Get_Data(7).SaiFurikae_Date = txt�V���ĐU�֓�.Text
        Get_Data(7).SaiFurikae_Day = lab�V���ĐU�֓�.Text

        Get_Data(8).Furikae_Check = chk�W���U�֓�.Checked
        Get_Data(8).Furikae_Enabled = chk�W���U�֓�.Enabled
        Get_Data(8).Furikae_Date = txt�W���U�֓�.Text
        Get_Data(8).Furikae_Day = lab�W���U�֓�.Text

        Get_Data(8).SaiFurikae_Check = chk�W���ĐU�֓�.Checked
        Get_Data(8).SaiFurikae_Enabled = chk�W���ĐU�֓�.Enabled
        Get_Data(8).SaiFurikae_Date = txt�W���ĐU�֓�.Text
        Get_Data(8).SaiFurikae_Day = lab�W���ĐU�֓�.Text

        Get_Data(9).Furikae_Check = chk�X���U�֓�.Checked
        Get_Data(9).Furikae_Enabled = chk�X���U�֓�.Enabled
        Get_Data(9).Furikae_Date = txt�X���U�֓�.Text
        Get_Data(9).Furikae_Day = lab�X���U�֓�.Text

        Get_Data(9).SaiFurikae_Check = chk�X���ĐU�֓�.Checked
        Get_Data(9).SaiFurikae_Enabled = chk�X���ĐU�֓�.Enabled
        Get_Data(9).SaiFurikae_Date = txt�X���ĐU�֓�.Text
        Get_Data(9).SaiFurikae_Day = lab�X���ĐU�֓�.Text

        Get_Data(10).Furikae_Check = chk�P�O���U�֓�.Checked
        Get_Data(10).Furikae_Enabled = chk�P�O���U�֓�.Enabled
        Get_Data(10).Furikae_Date = txt�P�O���U�֓�.Text
        Get_Data(10).Furikae_Day = lab�P�O���U�֓�.Text

        Get_Data(10).SaiFurikae_Check = chk�P�O���ĐU�֓�.Checked
        Get_Data(10).SaiFurikae_Enabled = chk�P�O���ĐU�֓�.Enabled
        Get_Data(10).SaiFurikae_Date = txt�P�O���ĐU�֓�.Text
        Get_Data(10).SaiFurikae_Day = lab�P�O���ĐU�֓�.Text

        Get_Data(11).Furikae_Check = chk�P�P���U�֓�.Checked
        Get_Data(11).Furikae_Enabled = chk�P�P���U�֓�.Enabled
        Get_Data(11).Furikae_Date = txt�P�P���U�֓�.Text
        Get_Data(11).Furikae_Day = lab�P�P���U�֓�.Text

        Get_Data(11).SaiFurikae_Check = chk�P�P���ĐU�֓�.Checked
        Get_Data(11).SaiFurikae_Enabled = chk�P�P���ĐU�֓�.Enabled
        Get_Data(11).SaiFurikae_Date = txt�P�P���ĐU�֓�.Text
        Get_Data(11).SaiFurikae_Day = lab�P�P���ĐU�֓�.Text

        Get_Data(12).Furikae_Check = chk�P�Q���U�֓�.Checked
        Get_Data(12).Furikae_Enabled = chk�P�Q���U�֓�.Enabled
        Get_Data(12).Furikae_Date = txt�P�Q���U�֓�.Text
        Get_Data(12).Furikae_Day = lab�P�Q���U�֓�.Text

        Get_Data(12).SaiFurikae_Check = chk�P�Q���ĐU�֓�.Checked
        Get_Data(12).SaiFurikae_Enabled = chk�P�Q���ĐU�֓�.Enabled
        Get_Data(12).SaiFurikae_Date = txt�P�Q���ĐU�֓�.Text
        Get_Data(12).SaiFurikae_Day = lab�P�Q���ĐU�֓�.Text

        Get_Data(1).Furikae_Check = chk�P���U�֓�.Checked
        Get_Data(1).Furikae_Enabled = chk�P���U�֓�.Enabled
        Get_Data(1).Furikae_Date = txt�P���U�֓�.Text
        Get_Data(1).Furikae_Day = lab�P���U�֓�.Text

        Get_Data(1).SaiFurikae_Check = chk�P���ĐU�֓�.Checked
        Get_Data(1).SaiFurikae_Enabled = chk�P���ĐU�֓�.Enabled
        Get_Data(1).SaiFurikae_Date = txt�P���ĐU�֓�.Text
        Get_Data(1).SaiFurikae_Day = lab�P���ĐU�֓�.Text

        Get_Data(2).Furikae_Check = chk�Q���U�֓�.Checked
        Get_Data(2).Furikae_Enabled = chk�Q���U�֓�.Enabled
        Get_Data(2).Furikae_Date = txt�Q���U�֓�.Text
        Get_Data(2).Furikae_Day = lab�Q���U�֓�.Text

        Get_Data(2).SaiFurikae_Check = chk�Q���ĐU�֓�.Checked
        Get_Data(2).SaiFurikae_Enabled = chk�Q���ĐU�֓�.Enabled
        Get_Data(2).SaiFurikae_Date = txt�Q���ĐU�֓�.Text
        Get_Data(2).SaiFurikae_Day = lab�Q���ĐU�֓�.Text

        Get_Data(3).Furikae_Check = chk�R���U�֓�.Checked
        Get_Data(3).Furikae_Enabled = chk�R���U�֓�.Enabled
        Get_Data(3).Furikae_Date = txt�R���U�֓�.Text
        Get_Data(3).Furikae_Day = lab�R���U�֓�.Text

        Get_Data(3).SaiFurikae_Check = chk�R���ĐU�֓�.Checked
        Get_Data(3).SaiFurikae_Enabled = chk�R���ĐU�֓�.Enabled
        Get_Data(3).SaiFurikae_Date = txt�R���ĐU�֓�.Text
        Get_Data(3).SaiFurikae_Day = lab�R���ĐU�֓�.Text

    End Sub

#End Region

#Region " Private Sub(�N�ԃX�P�W���[����ʐ���)"
    Private Sub PSUB_NENKAN_FORMAT()

        '�N�ԃX�P�W���[�����������\��

        '�`�F�b�N�{�b�N�X�l
        Call PSUB_NENKAN_CHK(True)

        '�`�F�b�N�{�b�N�XEnable�l
        Call PSUB_NENKAN_CHKBOXEnabled(True)

        '�e�L�X�g�{�b�N�X
        Call PSUB_NENKAN_DAYCLER()

        '�e�L�X�g�{�b�N�XEnable�l
        Call PSUB_NENKAN_TEXTEnabled(True)

        '�\���p���x��
        Call PSUB_NENKAN_LABCLER()

    End Sub
    Private Sub PSUB_NENKAN_CHK(ByVal pValue As Boolean)

        '�U�֓��̗L���`�F�b�N
        chk�S���U�֓�.Checked = pValue
        chk�T���U�֓�.Checked = pValue
        chk�U���U�֓�.Checked = pValue
        chk�V���U�֓�.Checked = pValue
        chk�W���U�֓�.Checked = pValue
        chk�X���U�֓�.Checked = pValue
        chk�P�O���U�֓�.Checked = pValue
        chk�P�P���U�֓�.Checked = pValue
        chk�P�Q���U�֓�.Checked = pValue
        chk�P���U�֓�.Checked = pValue
        chk�Q���U�֓�.Checked = pValue
        chk�R���U�֓�.Checked = pValue

        '�ĐU�֓��̗L���`�F�b�N
        chk�S���ĐU�֓�.Checked = pValue
        chk�T���ĐU�֓�.Checked = pValue
        chk�U���ĐU�֓�.Checked = pValue
        chk�V���ĐU�֓�.Checked = pValue
        chk�W���ĐU�֓�.Checked = pValue
        chk�X���ĐU�֓�.Checked = pValue
        chk�P�O���ĐU�֓�.Checked = pValue
        chk�P�P���ĐU�֓�.Checked = pValue
        chk�P�Q���ĐU�֓�.Checked = pValue
        chk�P���ĐU�֓�.Checked = pValue
        chk�Q���ĐU�֓�.Checked = pValue
        chk�R���ĐU�֓�.Checked = pValue

    End Sub
    Private Sub PSUB_NENKAN_CHKBOXEnabled(ByVal pValue As Boolean)

        '�U�֓��`�F�b�NBOX�̗L����
        chk�S���U�֓�.Enabled = pValue
        chk�T���U�֓�.Enabled = pValue
        chk�U���U�֓�.Enabled = pValue
        chk�V���U�֓�.Enabled = pValue
        chk�W���U�֓�.Enabled = pValue
        chk�X���U�֓�.Enabled = pValue
        chk�P�O���U�֓�.Enabled = pValue
        chk�P�P���U�֓�.Enabled = pValue
        chk�P�Q���U�֓�.Enabled = pValue
        chk�P���U�֓�.Enabled = pValue
        chk�Q���U�֓�.Enabled = pValue
        chk�R���U�֓�.Enabled = pValue

        '�ĐU�֓��`�F�b�NBOX�̗L����
        chk�S���ĐU�֓�.Enabled = pValue
        chk�T���ĐU�֓�.Enabled = pValue
        chk�U���ĐU�֓�.Enabled = pValue
        chk�V���ĐU�֓�.Enabled = pValue
        chk�W���ĐU�֓�.Enabled = pValue
        chk�X���ĐU�֓�.Enabled = pValue
        chk�P�O���ĐU�֓�.Enabled = pValue
        chk�P�P���ĐU�֓�.Enabled = pValue
        chk�P�Q���ĐU�֓�.Enabled = pValue
        chk�P���ĐU�֓�.Enabled = pValue
        chk�Q���ĐU�֓�.Enabled = pValue
        chk�R���ĐU�֓�.Enabled = pValue

    End Sub
    Private Sub PSUB_NENKAN_DAYCLER()

        '�U�֓��̃N���A����
        txt�S���U�֓�.Text = ""
        txt�T���U�֓�.Text = ""
        txt�U���U�֓�.Text = ""
        txt�V���U�֓�.Text = ""
        txt�W���U�֓�.Text = ""
        txt�X���U�֓�.Text = ""
        txt�P�O���U�֓�.Text = ""
        txt�P�P���U�֓�.Text = ""
        txt�P�Q���U�֓�.Text = ""
        txt�P���U�֓�.Text = ""
        txt�Q���U�֓�.Text = ""
        txt�R���U�֓�.Text = ""

        '�ĐU�֓��̃N���A����
        txt�S���ĐU�֓�.Text = ""
        txt�T���ĐU�֓�.Text = ""
        txt�U���ĐU�֓�.Text = ""
        txt�V���ĐU�֓�.Text = ""
        txt�W���ĐU�֓�.Text = ""
        txt�X���ĐU�֓�.Text = ""
        txt�P�O���ĐU�֓�.Text = ""
        txt�P�P���ĐU�֓�.Text = ""
        txt�P�Q���ĐU�֓�.Text = ""
        txt�P���ĐU�֓�.Text = ""
        txt�Q���ĐU�֓�.Text = ""
        txt�R���ĐU�֓�.Text = ""

    End Sub
    Private Sub PSUB_NENKAN_TEXTEnabled(ByVal pValue As Boolean)

        '�U�֓��e�L�X�gBOX�̗L����
        txt�S���U�֓�.Enabled = pValue
        txt�T���U�֓�.Enabled = pValue
        txt�U���U�֓�.Enabled = pValue
        txt�V���U�֓�.Enabled = pValue
        txt�W���U�֓�.Enabled = pValue
        txt�X���U�֓�.Enabled = pValue
        txt�P�O���U�֓�.Enabled = pValue
        txt�P�P���U�֓�.Enabled = pValue
        txt�P�Q���U�֓�.Enabled = pValue
        txt�P���U�֓�.Enabled = pValue
        txt�Q���U�֓�.Enabled = pValue
        txt�R���U�֓�.Enabled = pValue

        '�U�֓��e�L�X�gBOX�̗L����
        txt�S���ĐU�֓�.Enabled = pValue
        txt�T���ĐU�֓�.Enabled = pValue
        txt�U���ĐU�֓�.Enabled = pValue
        txt�V���ĐU�֓�.Enabled = pValue
        txt�W���ĐU�֓�.Enabled = pValue
        txt�X���ĐU�֓�.Enabled = pValue
        txt�P�O���ĐU�֓�.Enabled = pValue
        txt�P�P���ĐU�֓�.Enabled = pValue
        txt�P�Q���ĐU�֓�.Enabled = pValue
        txt�P���ĐU�֓�.Enabled = pValue
        txt�Q���ĐU�֓�.Enabled = pValue
        txt�R���ĐU�֓�.Enabled = pValue

    End Sub
    Private Sub PSUB_NENKAN_LABCLER()

        '�N�ԃX�P�W���[���̐U�֓����x���A�ĐU�֓����x���̃N���A
        lab�S���U�֓�.Text = ""
        lab�T���U�֓�.Text = ""
        lab�U���U�֓�.Text = ""
        lab�V���U�֓�.Text = ""
        lab�W���U�֓�.Text = ""
        lab�X���U�֓�.Text = ""
        lab�P�O���U�֓�.Text = ""
        lab�P�P���U�֓�.Text = ""
        lab�P�Q���U�֓�.Text = ""
        lab�P���U�֓�.Text = ""
        lab�Q���U�֓�.Text = ""
        lab�R���U�֓�.Text = ""

        lab�S���ĐU�֓�.Text = ""
        lab�T���ĐU�֓�.Text = ""
        lab�U���ĐU�֓�.Text = ""
        lab�V���ĐU�֓�.Text = ""
        lab�W���ĐU�֓�.Text = ""
        lab�X���ĐU�֓�.Text = ""
        lab�P�O���ĐU�֓�.Text = ""
        lab�P�P���ĐU�֓�.Text = ""
        lab�P�Q���ĐU�֓�.Text = ""
        lab�P���ĐU�֓�.Text = ""
        lab�Q���ĐU�֓�.Text = ""
        lab�R���ĐU�֓�.Text = ""

    End Sub
    Private Sub PSUB_SAIFURI_PROTECT(ByVal pValue As Boolean, Optional ByVal pTuki As Integer = 0)

        '�U�֓��L���`�F�b�N�ƐU�֓����͗��̃v���e�N�g(ON/OFF)����
        Select Case pTuki
            Case 0
                '�S���Ώ�
                chk�S���ĐU�֓�.Checked = pValue
                chk�S���ĐU�֓�.Enabled = pValue
                txt�S���ĐU�֓�.Enabled = pValue

                chk�T���ĐU�֓�.Checked = pValue
                chk�T���ĐU�֓�.Enabled = pValue
                txt�T���ĐU�֓�.Enabled = pValue

                chk�U���ĐU�֓�.Checked = pValue
                chk�U���ĐU�֓�.Enabled = pValue
                txt�U���ĐU�֓�.Enabled = pValue

                chk�V���ĐU�֓�.Checked = pValue
                chk�V���ĐU�֓�.Enabled = pValue
                txt�V���ĐU�֓�.Enabled = pValue

                chk�W���ĐU�֓�.Checked = pValue
                chk�W���ĐU�֓�.Enabled = pValue
                txt�W���ĐU�֓�.Enabled = pValue

                chk�X���ĐU�֓�.Checked = pValue
                chk�X���ĐU�֓�.Enabled = pValue
                txt�X���ĐU�֓�.Enabled = pValue

                chk�P�O���ĐU�֓�.Checked = pValue
                chk�P�O���ĐU�֓�.Enabled = pValue
                txt�P�O���ĐU�֓�.Enabled = pValue

                chk�P�P���ĐU�֓�.Checked = pValue
                chk�P�P���ĐU�֓�.Enabled = pValue
                txt�P�P���ĐU�֓�.Enabled = pValue

                chk�P�Q���ĐU�֓�.Checked = pValue
                chk�P�Q���ĐU�֓�.Enabled = pValue
                txt�P�Q���ĐU�֓�.Enabled = pValue

                chk�P���ĐU�֓�.Checked = pValue
                chk�P���ĐU�֓�.Enabled = pValue
                txt�P���ĐU�֓�.Enabled = pValue

                chk�Q���ĐU�֓�.Checked = pValue
                chk�Q���ĐU�֓�.Enabled = pValue
                txt�Q���ĐU�֓�.Enabled = pValue

                chk�R���ĐU�֓�.Checked = pValue
                chk�R���ĐU�֓�.Enabled = pValue
                txt�R���ĐU�֓�.Enabled = pValue
            Case 1
                '�P��
                chk�P���ĐU�֓�.Checked = pValue
                chk�P���ĐU�֓�.Enabled = pValue
                txt�P���ĐU�֓�.Enabled = pValue
            Case 2
                '�Q��
                chk�Q���ĐU�֓�.Checked = pValue
                chk�Q���ĐU�֓�.Enabled = pValue
                txt�Q���ĐU�֓�.Enabled = pValue
            Case 3
                '�R��
                chk�R���ĐU�֓�.Checked = pValue
                chk�R���ĐU�֓�.Enabled = pValue
                txt�R���ĐU�֓�.Enabled = pValue
            Case 4
                '�S��
                chk�S���ĐU�֓�.Checked = pValue
                chk�S���ĐU�֓�.Enabled = pValue
                txt�S���ĐU�֓�.Enabled = pValue
            Case 5
                '�T��
                chk�T���ĐU�֓�.Checked = pValue
                chk�T���ĐU�֓�.Enabled = pValue
                txt�T���ĐU�֓�.Enabled = pValue
            Case 6
                '�U��
                chk�U���ĐU�֓�.Checked = pValue
                chk�U���ĐU�֓�.Enabled = pValue
                txt�U���ĐU�֓�.Enabled = pValue
            Case 7
                '�V��
                chk�V���ĐU�֓�.Checked = pValue
                chk�V���ĐU�֓�.Enabled = pValue
                txt�V���ĐU�֓�.Enabled = pValue
            Case 8
                '�W��
                chk�W���ĐU�֓�.Checked = pValue
                chk�W���ĐU�֓�.Enabled = pValue
                txt�W���ĐU�֓�.Enabled = pValue
            Case 9
                '�X��
                chk�X���ĐU�֓�.Checked = pValue
                chk�X���ĐU�֓�.Enabled = pValue
                txt�X���ĐU�֓�.Enabled = pValue
            Case 10
                '�P�O��
                chk�P�O���ĐU�֓�.Checked = pValue
                chk�P�O���ĐU�֓�.Enabled = pValue
                txt�P�O���ĐU�֓�.Enabled = pValue
            Case 11
                '�P�P��
                chk�P�P���ĐU�֓�.Checked = pValue
                chk�P�P���ĐU�֓�.Enabled = pValue
                txt�P�P���ĐU�֓�.Enabled = pValue
            Case 12
                '�P�Q��
                chk�P�Q���ĐU�֓�.Checked = pValue
                chk�P�Q���ĐU�֓�.Enabled = pValue
                txt�P�Q���ĐU�֓�.Enabled = pValue
        End Select

    End Sub

    Private Sub PSUB_NENKAN_SET(ByVal A As CheckBox, ByVal B As TextBox, ByVal C As Label, ByVal aReader As MyOracleReader)

        '�N�ԃX�P�W���[���̐U�֓��L���`�F�b�N�A�U�֓��A���t�\���A�ĐU�֓��L���`�F�b�N�A�U�֓��A���t�\���̕ҏW
        A.Checked = True

        '�\���̈�P������͂��ꂽ�U�֓��𓾂�
        B.Text = Trim(aReader.GetString("YOBI1_S"))
        C.Text = Mid(aReader.GetString("FURI_DATE_S"), 1, 4) & "/" & Mid(aReader.GetString("FURI_DATE_S"), 5, 2) & "/" & Mid(aReader.GetString("FURI_DATE_S"), 7, 2)

        '�����t���O����
        '����Ɩ��������͕ҏW�ł��Ȃ�
        A.Enabled = False
        B.Enabled = False
        Select Case True
            Case aReader.GetString("ENTRI_FLG_S") = "1"
            Case aReader.GetString("CHECK_FLG_S") = "1"
            Case aReader.GetString("DATA_FLG_S") = "1"
            Case aReader.GetString("FUNOU_FLG_S") = "1"
            Case aReader.GetString("SAIFURI_FLG_S") = "1"
            Case aReader.GetString("KESSAI_FLG_S") = "1"
            Case aReader.GetString("TYUUDAN_FLG_S") = "1"
            Case Else
                A.Enabled = True
                B.Enabled = True
        End Select

    End Sub
#End Region

#Region " Private Function(�N�ԃX�P�W���[��)"
    Private Function PFUNC_SCH_GET_NENKAN() As Boolean

        PFUNC_SCH_GET_NENKAN = False

        '�U�֓��̗L���`�F�b�NOFF�A�ĐU�֓��̗L���`�F�b�NOFF
        Call PSUB_NENKAN_CHK(False)

        '�U�֓����͗��A�ĐU�֓����͗��̃N���A
        Call PSUB_NENKAN_DAYCLER()

        '�U�֓��A�ĐU�֓����x���N���A
        Call PSUB_NENKAN_LABCLER()

        If PFUNC_NENKAN_SANSYOU() = False Then
            Exit Function
        End If

        PFUNC_SCH_GET_NENKAN = True

    End Function

    Private Function PFUNC_SCH_DELETE_INSERT_NENKAN() As Boolean

        '�N�ԃX�P�W���[���X�V����
        If PFUNC_NENKAN_KOUSIN() = False Then
            '������ʂ�Ƃ������Ƃ͂P���ł������������R�[�h�����݂����Ƃ������ƂȂ̂�
            Int_Syori_Flag(0) = 2
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_SCH_NENKAN_GET(ByVal strNENGETUDO As String, ByVal strFURIKUBUN As String, ByVal astrFURI_DATE As String) As Boolean

        Dim iGakunen(8) As Integer
        Dim iCount As Integer
        Dim bFlg As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '�ʏ탌�R�[�h�̑��݃`�F�b�N
        PFUNC_SCH_NENKAN_GET = True

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='" & strFURIKUBUN & "'")

        If oraReader.DataReader(sql) = False Then
            '�ʏ탌�R�|�h����
            oraReader.Close()
            Exit Function
        End If
        oraReader.Close()

        PFUNC_SCH_NENKAN_GET = False
        bFlg = False

        sql = New StringBuilder(128)

        '�w�N�w�肪�Ȃ��ꍇ�͏��������Ȃ�
        If PFUNC_GAKUNEN_GET(iGakunen) = True Then
            '���ʃ��R�[�h�̑Ώۊw�N�t���O�̏�Ԃ����ɒʏ탌�R�[�h�̑Ώۊw�N�t���O���n�e�e�ɂ���
            '�n�m�ɂ���@�\�����������ꍇ�A���ʃ��R�[�h�����������݂����ꍇ�ɑO���R�[�h�ł̏��������ʂɂȂ�
            '���ʃ��R�[�h�̑Ώۊw�N�P�t���O���u�P�v�̏ꍇ
            sql.Append(" UPDATE  G_SCHMAST")
            sql.Append(" SET ")
            For iCount = 1 To 9
                If iGakunen(iCount - 1) = 1 Then
                    If bFlg = False Then
                        sql.Append(" ")

                        bFlg = True
                    Else
                        sql.Append(",")
                    End If

                    sql.Append(" GAKUNEN" & iCount & "_FLG_S ='0'")
                End If
            Next iCount
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
            sql.Append(" AND")
            sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
            sql.Append(" AND")
            sql.Append(" SCH_KBN_S ='0'")
            sql.Append(" AND")
            sql.Append(" FURI_KBN_S ='" & strFURIKUBUN & "'")

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                MessageBox.Show("�X�P�W���[���}�X�^�̍X�V�����ŃG���[���������܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        End If

        PFUNC_SCH_NENKAN_GET = True

    End Function
    Private Function PFUNC_GAKUNEN_GET(ByRef pValue() As Integer) As Boolean

        PFUNC_GAKUNEN_GET = False

        ReDim pValue(8)

        If STR�P�w�N = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(0) = 1
        Else
            pValue(0) = 0
        End If
        If STR�Q�w�N = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(1) = 1
        Else
            pValue(1) = 0
        End If
        If STR�R�w�N = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(2) = 1
        Else
            pValue(2) = 0
        End If
        If STR�S�w�N = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(3) = 1
        Else
            pValue(3) = 0
        End If
        If STR�T�w�N = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(4) = 1
        Else
            pValue(4) = 0
        End If
        If STR�U�w�N = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(5) = 1
        Else
            pValue(5) = 0
        End If
        If STR�V�w�N = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(6) = 1
        Else
            pValue(6) = 0
        End If
        If STR�W�w�N = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(7) = 1
        Else
            pValue(7) = 0
        End If
        If STR�X�w�N = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(8) = 1
        Else
            pValue(8) = 0
        End If

    End Function

    Private Function PFUNC_NENKAN_SANSYOU() As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '�N�ԃX�P�W���[���@�Q�Ə���
        PFUNC_NENKAN_SANSYOU = False

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = 0")

        If oraReader.DataReader(sql) = False Then
            oraReader.Close()
            Exit Function
        End If

        Do Until oraReader.EOF
            Select Case oraReader.GetString("FURI_KBN_S")
                Case "0"
                    '���U�X�P�W���[��
                    Select Case Mid(oraReader.GetString("NENGETUDO_S"), 5, 2)
                        Case "04"   '�U�֓��̌�
                            Call PSUB_NENKAN_SET(chk�S���U�֓�, txt�S���U�֓�, lab�S���U�֓�, oraReader)
                            '2006/11/22�@�\�����̐U�֓����擾
                            str�ʏ�U�֓�(4) = Replace(lab�S���U�֓�.Text, "/", "")
                            '2006/11/30�@�`�F�b�N�t���O�E�s�\�t���O���\���̂Ɋi�[
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(4).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(4).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(4).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(4).FunouFurikae_Flag = False
                            End If
                        Case "05"
                            Call PSUB_NENKAN_SET(chk�T���U�֓�, txt�T���U�֓�, lab�T���U�֓�, oraReader)
                            str�ʏ�U�֓�(5) = Replace(lab�T���U�֓�.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(5).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(5).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(5).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(5).FunouFurikae_Flag = False
                            End If
                        Case "06"
                            Call PSUB_NENKAN_SET(chk�U���U�֓�, txt�U���U�֓�, lab�U���U�֓�, oraReader)
                            str�ʏ�U�֓�(6) = Replace(lab�U���U�֓�.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(6).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(6).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(6).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(6).FunouFurikae_Flag = False
                            End If
                        Case "07"
                            Call PSUB_NENKAN_SET(chk�V���U�֓�, txt�V���U�֓�, lab�V���U�֓�, oraReader)
                            str�ʏ�U�֓�(7) = Replace(lab�V���U�֓�.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(7).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(7).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(7).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(7).FunouFurikae_Flag = False
                            End If
                        Case "08"
                            Call PSUB_NENKAN_SET(chk�W���U�֓�, txt�W���U�֓�, lab�W���U�֓�, oraReader)
                            str�ʏ�U�֓�(8) = Replace(lab�W���U�֓�.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(8).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(8).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(8).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(8).FunouFurikae_Flag = False
                            End If
                        Case "09"
                            Call PSUB_NENKAN_SET(chk�X���U�֓�, txt�X���U�֓�, lab�X���U�֓�, oraReader)
                            str�ʏ�U�֓�(9) = Replace(lab�X���U�֓�.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(9).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(9).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(9).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(9).FunouFurikae_Flag = False
                            End If
                        Case "10"
                            Call PSUB_NENKAN_SET(chk�P�O���U�֓�, txt�P�O���U�֓�, lab�P�O���U�֓�, oraReader)
                            str�ʏ�U�֓�(10) = Replace(lab�P�O���U�֓�.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(10).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(10).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(10).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(10).FunouFurikae_Flag = False
                            End If
                        Case "11"
                            Call PSUB_NENKAN_SET(chk�P�P���U�֓�, txt�P�P���U�֓�, lab�P�P���U�֓�, oraReader)
                            str�ʏ�U�֓�(11) = Replace(lab�P�P���U�֓�.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(11).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(11).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(11).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(11).FunouFurikae_Flag = False
                            End If
                        Case "12"
                            Call PSUB_NENKAN_SET(chk�P�Q���U�֓�, txt�P�Q���U�֓�, lab�P�Q���U�֓�, oraReader)
                            str�ʏ�U�֓�(12) = Replace(lab�P�Q���U�֓�.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(12).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(12).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(12).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(12).FunouFurikae_Flag = False
                            End If
                        Case "01"
                            Call PSUB_NENKAN_SET(chk�P���U�֓�, txt�P���U�֓�, lab�P���U�֓�, oraReader)
                            str�ʏ�U�֓�(1) = Replace(lab�P���U�֓�.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(1).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(1).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(1).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(1).FunouFurikae_Flag = False
                            End If
                        Case "02"
                            Call PSUB_NENKAN_SET(chk�Q���U�֓�, txt�Q���U�֓�, lab�Q���U�֓�, oraReader)
                            str�ʏ�U�֓�(2) = Replace(lab�Q���U�֓�.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(2).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(2).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(2).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(2).FunouFurikae_Flag = False
                            End If
                        Case "03"
                            Call PSUB_NENKAN_SET(chk�R���U�֓�, txt�R���U�֓�, lab�R���U�֓�, oraReader)
                            str�ʏ�U�֓�(3) = Replace(lab�R���U�֓�.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(3).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(3).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(3).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(3).FunouFurikae_Flag = False
                            End If
                    End Select
                Case "1"
                    '�ĐU�X�P�W���[��
                    Select Case Mid(oraReader.GetString("NENGETUDO_S"), 5, 2)
                        Case "04"    '�ĐU�֓��̌�
                            Call PSUB_NENKAN_SET(chk�S���ĐU�֓�, txt�S���ĐU�֓�, lab�S���ĐU�֓�, oraReader)
                            '2006/11/22�@�\�����̐U�֓����擾
                            str�ʏ�ĐU��(4) = Replace(lab�S���ĐU�֓�.Text, "/", "")
                            '2006/11/30�@�ĐU���̍ĐU�������߂�
                            str�ʏ�āX�U��(4) = oraReader.GetString("SFURI_DATE_S")
                            '2006/11/30�@�`�F�b�N�t���O���擾
                            SYOKI_NENKAN_SCHINFO(4).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "05"
                            Call PSUB_NENKAN_SET(chk�T���ĐU�֓�, txt�T���ĐU�֓�, lab�T���ĐU�֓�, oraReader)
                            str�ʏ�ĐU��(5) = Replace(lab�T���ĐU�֓�.Text, "/", "")
                            str�ʏ�āX�U��(5) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(5).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "06"
                            Call PSUB_NENKAN_SET(chk�U���ĐU�֓�, txt�U���ĐU�֓�, lab�U���ĐU�֓�, oraReader)
                            str�ʏ�ĐU��(6) = Replace(lab�U���ĐU�֓�.Text, "/", "")
                            str�ʏ�āX�U��(6) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(6).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "07"
                            Call PSUB_NENKAN_SET(chk�V���ĐU�֓�, txt�V���ĐU�֓�, lab�V���ĐU�֓�, oraReader)
                            str�ʏ�ĐU��(7) = Replace(lab�V���ĐU�֓�.Text, "/", "")
                            str�ʏ�āX�U��(7) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(7).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "08"
                            Call PSUB_NENKAN_SET(chk�W���ĐU�֓�, txt�W���ĐU�֓�, lab�W���ĐU�֓�, oraReader)
                            str�ʏ�ĐU��(8) = Replace(lab�W���ĐU�֓�.Text, "/", "")
                            str�ʏ�āX�U��(8) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(8).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "09"
                            Call PSUB_NENKAN_SET(chk�X���ĐU�֓�, txt�X���ĐU�֓�, lab�X���ĐU�֓�, oraReader)
                            str�ʏ�ĐU��(9) = Replace(lab�X���ĐU�֓�.Text, "/", "")
                            str�ʏ�āX�U��(9) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(9).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "10"
                            Call PSUB_NENKAN_SET(chk�P�O���ĐU�֓�, txt�P�O���ĐU�֓�, lab�P�O���ĐU�֓�, oraReader)
                            str�ʏ�ĐU��(10) = Replace(lab�P�O���ĐU�֓�.Text, "/", "")
                            str�ʏ�āX�U��(10) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(10).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "11"
                            Call PSUB_NENKAN_SET(chk�P�P���ĐU�֓�, txt�P�P���ĐU�֓�, lab�P�P���ĐU�֓�, oraReader)
                            str�ʏ�ĐU��(11) = Replace(lab�P�P���ĐU�֓�.Text, "/", "")
                            str�ʏ�āX�U��(11) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(11).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "12"
                            Call PSUB_NENKAN_SET(chk�P�Q���ĐU�֓�, txt�P�Q���ĐU�֓�, lab�P�Q���ĐU�֓�, oraReader)
                            str�ʏ�ĐU��(12) = Replace(lab�P�Q���ĐU�֓�.Text, "/", "")
                            str�ʏ�āX�U��(12) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(12).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "01"
                            Call PSUB_NENKAN_SET(chk�P���ĐU�֓�, txt�P���ĐU�֓�, lab�P���ĐU�֓�, oraReader)
                            str�ʏ�ĐU��(1) = Replace(lab�P���ĐU�֓�.Text, "/", "")
                            str�ʏ�āX�U��(1) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(1).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "02"
                            Call PSUB_NENKAN_SET(chk�Q���ĐU�֓�, txt�Q���ĐU�֓�, lab�Q���ĐU�֓�, oraReader)
                            str�ʏ�ĐU��(2) = Replace(lab�Q���ĐU�֓�.Text, "/", "")
                            str�ʏ�āX�U��(2) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(2).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "03"
                            Call PSUB_NENKAN_SET(chk�R���ĐU�֓�, txt�R���ĐU�֓�, lab�R���ĐU�֓�, oraReader)
                            str�ʏ�ĐU��(3) = Replace(lab�R���ĐU�֓�.Text, "/", "")
                            str�ʏ�āX�U��(3) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(3).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                    End Select
            End Select

            oraReader.NextRead()

        Loop

        oraReader.Close()

        PFUNC_NENKAN_SANSYOU = True

    End Function
    Private Function PFUNC_NENKAN_DATE_CHECK(ByVal pFurikae As String, ByVal pSaifuri As String) As Boolean

        PFUNC_NENKAN_DATE_CHECK = False

        '�U�֓��ƍĐU�֓�������H
        If Trim(pFurikae) <> "" And Trim(pSaifuri) <> "" Then
            If Trim(pFurikae) = Trim(pSaifuri) Then
                Exit Function
            End If
        End If

        PFUNC_NENKAN_DATE_CHECK = True

    End Function
    Private Function PFUNC_NENKAN_SAKUSEI() As Boolean

        Dim sTuki As String

        PFUNC_NENKAN_SAKUSEI = False

        ''���͓��e��ϐ��ɑޔ�
        ''�@��̏������ȗ�������ׂɕK�v
        'Call PSUB_NENKAN_GET() '2006/11/30�@�R�����g��

        '�U�֓��ƍĐU�֓�������̏ꍇ�̓G���[
        For i As Integer = 1 To 12
            If NENKAN_SCHINFO(i).Furikae_Check = True And NENKAN_SCHINFO(i).SaiFurikae_Check = True Then
                If PFUNC_NENKAN_DATE_CHECK(NENKAN_SCHINFO(i).Furikae_Date, NENKAN_SCHINFO(i).SaiFurikae_Date) = False Then
                    MessageBox.Show("(�N�ԃX�P�W���[��)" & vbCrLf & "�U�֓��ƍĐU�֓�������ł�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If
        Next i

        '�U�֓��`�F�b�N
        For i As Integer = 1 To 12
            If bln�N�ԍX�V(i) = True Then '2006/11/30�@�X�V�̂Ȃ����͍̂X�V�̕K�v�Ȃ�

                If NENKAN_SCHINFO(i).Furikae_Check = True And NENKAN_SCHINFO(i).Furikae_Enabled = True Then
                    sTuki = Format(i, "00")

                    STR�P�w�N = "1"
                    STR�Q�w�N = "1"
                    STR�R�w�N = "1"
                    STR�S�w�N = "1"
                    STR�T�w�N = "1"
                    STR�U�w�N = "1"
                    STR�V�w�N = "1"
                    STR�W�w�N = "1"
                    STR�X�w�N = "1"

                    '�p�����^�͇@�� �A���͐U�֓� �B�ĐU�֌� �C�ĐU�֓�
                    Select Case NENKAN_SCHINFO(i).SaiFurikae_Check
                        Case True
                            If PFUNC_NENKAN_SAKUSEI_SUB(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, sTuki, NENKAN_SCHINFO(i).SaiFurikae_Date, i) = False Then
                                Exit Function
                            End If
                        Case False
                            If PFUNC_NENKAN_SAKUSEI_SUB(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, "", NENKAN_SCHINFO(i).SaiFurikae_Date, i) = False Then
                                Exit Function
                            End If
                    End Select

                    '������ʂ�Ƃ������Ƃ͏����ɐ��������Ƃ������ƂȂ̂�
                    Int_Syori_Flag(0) = 1
                Else
                    '���U�̃X�P�W���[�����������ł��ĐU�̂ق���
                    If NENKAN_SCHINFO(i).SaiFurikae_Check = True And NENKAN_SCHINFO(i).SaiFurikae_Enabled = True Then

                        sTuki = Format(i, "00")
                        STR�P�w�N = "1"
                        STR�Q�w�N = "1"
                        STR�R�w�N = "1"
                        STR�S�w�N = "1"
                        STR�T�w�N = "1"
                        STR�U�w�N = "1"
                        STR�V�w�N = "1"
                        STR�W�w�N = "1"
                        STR�X�w�N = "1"
                        If PFUNC_NENKAN_SAKUSEI_SUB2(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, sTuki, NENKAN_SCHINFO(i).SaiFurikae_Date, i) = False Then
                            Exit Function
                        End If

                        '�쐬�����ĐU�̃X�P�W���[���̐U�֓������U�̃X�P�W���[���̍ĐU���֍X�V����
                        If PFUNC_SCHMAST_UPDATE_SFURIDATE("0") = False Then

                            Exit Function
                        End If
                        '�ǋL 2006/12/04
                        '������ʂ�Ƃ������Ƃ͏����ɐ��������Ƃ������ƂȂ̂�
                        Int_Syori_Flag(0) = 1

                    End If
                End If
            Else '�X�V���Ȃ��Ă���Ƒ��̃X�P�W���[��������

                '��Ǝ��U�A�g���̂�
                If NENKAN_SCHINFO(i).Furikae_Check = True And NENKAN_SCHINFO(i).Furikae_Enabled = True Then
                    sTuki = Format(i, "00")

                    STR�P�w�N = "1"
                    STR�Q�w�N = "1"
                    STR�R�w�N = "1"
                    STR�S�w�N = "1"
                    STR�T�w�N = "1"
                    STR�U�w�N = "1"
                    STR�V�w�N = "1"
                    STR�W�w�N = "1"
                    STR�X�w�N = "1"

                    '�p�����^�͇@�� �A���͐U�֓� �B�ĐU�֌� �C�ĐU�֓�
                    Select Case NENKAN_SCHINFO(i).SaiFurikae_Check
                        Case True
                            If PFUNC_NENKAN_SAKUSEI_SUB_KIGYO(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, sTuki, NENKAN_SCHINFO(i).SaiFurikae_Date) = False Then
                                Exit Function
                            End If
                        Case False
                            If PFUNC_NENKAN_SAKUSEI_SUB_KIGYO(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, "", NENKAN_SCHINFO(i).SaiFurikae_Date) = False Then
                                Exit Function
                            End If
                    End Select

                    '������ʂ�Ƃ������Ƃ͏����ɐ��������Ƃ������ƂȂ̂�
                    Int_Syori_Flag(0) = 1
                Else
                    '���U�̃X�P�W���[�����������ł��ĐU�̂ق���
                    If NENKAN_SCHINFO(i).SaiFurikae_Check = True And NENKAN_SCHINFO(i).SaiFurikae_Enabled = True Then

                        sTuki = Format(i, "00")
                        STR�P�w�N = "1"
                        STR�Q�w�N = "1"
                        STR�R�w�N = "1"
                        STR�S�w�N = "1"
                        STR�T�w�N = "1"
                        STR�U�w�N = "1"
                        STR�V�w�N = "1"
                        STR�W�w�N = "1"
                        STR�X�w�N = "1"
                        If PFUNC_NENKAN_SAKUSEI_SUB2_KIGYO(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, sTuki, NENKAN_SCHINFO(i).SaiFurikae_Date) = False Then
                            Exit Function
                        End If
                        '�ǋL 2006/12/04
                        '������ʂ�Ƃ������Ƃ͏����ɐ��������Ƃ������ƂȂ̂�
                        Int_Syori_Flag(0) = 1

                    End If
                End If
            End If
        Next i

        PFUNC_NENKAN_SAKUSEI = True

    End Function
    Private Function PFUNC_NENKAN_SAKUSEI_SUB(ByVal s������ As String, ByVal s�� As String, ByVal s�U�֓� As String, ByVal s�ĐU�֌� As String, ByVal s�ĐU�֓� As String, ByVal i As Integer) As Boolean
        '�X�P�W���[���@�ʏ탌�R�[�h(���U)�쐬

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUNC_NENKAN_SAKUSEI_SUB = False
        Dim updade As Boolean

        '�����N���̍쐬
        STR�����N�� = PFUNC_SEIKYUTUKIHI(s������)

        '�U�֓��Z�o
        STR�U�֓� = PFUNC_FURIHI_MAKE(s��, s�U�֓�, "0", "0")

        '2010/10/21 �_��U�֓����Z�o����
        STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s��, s�U�֓�, "0", "0")

        '�ĐU���̗�������ƍĐU�֔N�A�ĐU�֌��ݒ�
        '�ĐU�֓������͂���A���N�ԃX�P�W���[���̃`�F�b�N�{�b�N�X���n�m�is�ĐU�֌��Ɍ����ݒ�j�̏ꍇ
        If s�ĐU�֌� <> "" And s�ĐU�֓� <> "" Then
            STRW�ĐU�֔N = Mid(STR�U�֓�, 1, 4)

            'If Mid(STR�U�֓�, 7, 2) <= s�ĐU�֓� Then
            If STR�U�֓� <= STR�����N�� & s�ĐU�֓� Then
                STRW�ĐU�֌� = s�ĐU�֌�
                STRW�ĐU�֓� = s�ĐU�֓�
            Else
                If s�� = "12" Then
                    STRW�ĐU�֌� = "01"
                    STRW�ĐU�֔N = CStr(CInt(Mid(STR�����N��, 1, 4)) + 1)

                Else
                    STRW�ĐU�֌� = Format((CInt(s�ĐU�֌�) + 1), "00")
                End If
                STRW�ĐU�֓� = s�ĐU�֓�
            End If
        End If

        '�ĐU�֓������͂Ȃ��A���N�ԃX�P�W���[���̃`�F�b�N�{�b�N�X���n�m
        If s�ĐU�֌� <> "" And s�ĐU�֓� = "" Then

            STRW�ĐU�֔N = Mid(STR�U�֓�, 1, 4)

            'If Mid(STR�U�֓�, 7, 2) <= GAKKOU_INFO.SFURI_DATE Then
            If STR�U�֓� <= STR�����N�� & GAKKOU_INFO.SFURI_DATE Then
                'STRW�ĐU�֌� = s�ĐU�֌�
                'STRW�ĐU�֓� = GAKKOU_INFO.SFURI_DATE
                If Mid(STR�U�֓�, 5, 2) > Mid(STR�����N��, 5, 2) Then
                    If s�� = "12" Then
                        STRW�ĐU�֌� = "01"
                        STRW�ĐU�֔N = CStr(CInt(Mid(STR�����N��, 1, 4)) + 1)
                    Else
                        STRW�ĐU�֌� = Format((CInt(s�ĐU�֌�) + 1), "00")
                    End If
                    STRW�ĐU�֓� = GAKKOU_INFO.SFURI_DATE

                Else
                    STRW�ĐU�֌� = s�ĐU�֌�
                    STRW�ĐU�֓� = GAKKOU_INFO.SFURI_DATE
                End If
            Else
                If s�� = "12" Then
                    STRW�ĐU�֌� = "01"
                    STRW�ĐU�֔N = CStr(CInt(Mid(STR�����N��, 1, 4)) + 1)
                Else
                    STRW�ĐU�֌� = Format((CInt(s�ĐU�֌�) + 1), "00")
                End If
                STRW�ĐU�֓� = GAKKOU_INFO.SFURI_DATE
            End If
        End If


        If s�ĐU�֌� = "" Then
            STR�ĐU�֓� = "00000000"
        Else
            '�ĐU�֓��Z�o
            STR�ĐU�֓� = PFUNC_SAIFURIHI_MAKE(Trim(STRW�ĐU�֌�), Trim(STRW�ĐU�֓�))
        End If

        '�c�Ɠ����Z�o�������ʂŐU�֓��ƍĐU�֓�������ɂȂ�ꍇ�������
        If STR�U�֓� = STR�ĐU�֓� Then
            MessageBox.Show("(�N�ԃX�P�W���[��)" & vbCrLf & Mid(STR�U�֓�, 5, 2) & "����" & "�U�֓��ƍĐU�֓�������ł�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If


        '�U�֓��̗L���͈̓`�F�b�N
        If PFUNC_FURIHI_HANI_CHECK() = False Then
            MessageBox.Show("(�N�ԃX�P�W���[��)" & vbCrLf & "�U�֌_����ԁi" & GAKKOU_INFO.KAISI_DATE & "�`" & GAKKOU_INFO.SYURYOU_DATE & "�j�O�̌��ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '���ʃ��R�[�h�̑Ώۊw�N�̐ݒ肵����
        '�w�Z�R�[�h�A�����N���A�U�֋敪�i0:���U�j
        If PFUNC_SCH_TOKUBETU_GET(STR�����N��, "0") = False Then
            MessageBox.Show("(�N�ԃX�P�W���[��)" & vbCrLf & "���ʃX�P�W���[���Ώۊw�N�ݒ�ŃG���[���������܂���(���U)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        '�������R�[�h�L���`�F�b�N
        If PFUNC_SCHMAST_GET("0", "0", Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", ""), Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", "")) = True Then
            updade = True
        End If

        '�X�P�W���[���敪�̋��ʕϐ��ݒ�
        STR�X�P�敪 = "0"

        '�U�֋敪�̋��ʕϐ��ݒ�
        STR�U�֋敪 = "0"

        '���͐U�֓��̋��ʕϐ��ݒ�
        If s�U�֓� = "" Then
            STR�N�ԓ��͐U�֓� = Space(15)
        Else
            STR�N�ԓ��͐U�֓� = s�U�֓�
        End If

        Dim strSQL As String = ""
        If updade = False Then
            '�X�P�W���[���}�X�^�o�^SQL��(���U)�쐬
            strSQL = PSUB_INSERT_G_SCHMAST_SQL()
        Else
            '�X�P�W���[���}�X�^�X�VSQL��(���U)�쐬
            strSQL = PSUB_UPDATE_G_SCHMAST_SQL(Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", ""), Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""))
        End If

        If MainDB.ExecuteNonQuery(strSQL) < 0 Then
            MessageBox.Show("�o�^�Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '-----------------------------------------------
        '2006/07/26�@��Ǝ��U�̏��U�̃X�P�W���[�����쐬
        '-----------------------------------------------
        '��Ǝ��U�A�g���̂�

        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)

        '���ɓo�^����Ă��邩�`�F�b�N
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '01' AND ")
        sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")

        If oraReader.DataReader(sql) = True Then    '�X�P�W���[�������ɑ��݂���
        Else     '�X�P�W���[�������݂��Ȃ�
            '�R�����g 2006/12/11
            'If intPUSH_BTN = 2 Then '�X�V��
            '    MessageBox.Show("��Ǝ��U���̃X�P�W���[��(" & STR�����N��.Substring(0, 4) & "�N" & STR�����N��.Substring(4, 2) & "����)�����݂��܂���" & vbCrLf & "��Ǝ��U���Ō��ԃX�P�W���[���쐬��A" & vbCrLf & "�w�Z�X�P�W���[���̍X�V�������ēx�s���Ă�������", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            'Else
            '�X�P�W���[���쐬
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, "01", gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                     gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '�����Ƀq�b�g���Ȃ�������

                '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                'If fn_INSERTSCHMAST(strGakkouCode, "01", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                If fn_INSERTSCHMAST(strGakkouCode, "01", STR�U�֓�, gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", "��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���")
                    MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If
            'End If

        End If
        oraReader.Close()


        '�ĐU���R�[�h�̍쐬
        If STR�ĐU�֓� <> "00000000" Then

            '2006/11/30�@update�t���O�̏�����
            updade = False

            '���U�ŋ��߂��ĐU����U�֓��ɐݒ�
            STR�U�֓� = STR�ĐU�֓�

            '2010/10/21 �ĐU�̌_��U�֓����Z�o����
            '2011/06/15 �W���ŏC�� �_��U�֓��ƌ_��ĐU�����t�]����ꍇ�͗����̍ĐU���ɂ��� -------------START
            'STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s�ĐU�֌�, s�ĐU�֓�, "0", "1")
            If s�ĐU�֓� = "" Then
                STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s�ĐU�֌�, s�ĐU�֓�, "0", "1")
            Else
                STR�_��U�֓� = STR�U�֓�
            End If
            '2011/06/15 �W���ŏC�� �_��U�֓��ƌ_��ĐU�����t�]����ꍇ�͗����̍ĐU���ɂ��� -------------END

            '�ĐU���̎Z�o
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"    '1(�ĐU�L/�J�z��)
                    STR�ĐU�֓� = "00000000"
                Case "2"    '2(�ĐU�L/�J�z�L)   ���񏉐U����ݒ�
                    '2011/06/15 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� -------------START

                    'If s�ĐU�֓� = "" Then
                    '    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(Trim(s�ĐU�֌�), Trim(strSFURI_DT))
                    'Else
                    '    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(Trim(s�ĐU�֌�), Trim(s�ĐU�֓�))
                    'End If
                    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(STR�U�֓�)
                    '2011/06/15 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� -------------END
            End Select


            '���ʃ��R�[�h�̑Ώۊw�N�̐ݒ肵����
            '�w�Z�R�[�h�A�����N���A�U�֋敪�i1:�ĐU�j
            If PFUNC_SCH_TOKUBETU_GET(STR�����N��, "1") = False Then
                MessageBox.Show("(�N�ԃX�P�W���[��)" & vbCrLf & "���ʃX�P�W���[���Ώۊw�N�ݒ�ŃG���[���������܂���(�ĐU)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            If PFUNC_SCHMAST_GET("0", "1", Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""), str�ʏ�āX�U��(i)) = True Then
                updade = True
            End If

            '�X�P�W���[���敪�̋��ʕϐ��ݒ�
            STR�X�P�敪 = "0"
            '�U�֋敪�̋��ʕϐ��ݒ�
            STR�U�֋敪 = "1"
            '���͐U�֓��̋��ʕϐ��ݒ�
            If s�ĐU�֓� = "" Then
                STR�N�ԓ��͐U�֓� = Space(15)
            Else
                STR�N�ԓ��͐U�֓� = s�ĐU�֓�
            End If

            '2006/11/30�@�V�K�o�^���X�V������
            strSQL = ""
            If updade = False Then
                '�X�P�W���[���}�X�^�o�^SQL��(�ĐU)�쐬
                strSQL = PSUB_INSERT_G_SCHMAST_SQL()
            Else
                '�X�P�W���[���}�X�^�X�VSQL��(�ĐU)�쐬
                strSQL = PSUB_UPDATE_G_SCHMAST_SQL(Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""), str�ʏ�āX�U��(i))
            End If

            If MainDB.ExecuteNonQuery(strSQL) < 0 Then
                MessageBox.Show("�o�^�Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            STR�N�ԓ��͐U�֓� = Space(15)
            updade = False

            '-----------------------------------------------
            '2006/07/26�@��Ǝ��U�̍ĐU�̃X�P�W���[�����쐬
            '-----------------------------------------------
            '��Ǝ��U�A�g���̂�
            oraReader = New MyOracleReader(MainDB)
            sql = New StringBuilder(128)

            '���ɓo�^����Ă��邩�`�F�b�N
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")

            '�Ǎ��̂�
            If oraReader.DataReader(sql) = True Then    '�X�P�W���[�������ɑ��݂���
            Else     '�X�P�W���[�������݂��Ȃ�
                '�X�P�W���[���쐬
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '�����Ƀq�b�g���Ȃ�������

                    '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", "��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���")
                        MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()

        End If

        PFUNC_NENKAN_SAKUSEI_SUB = True

    End Function
    Private Function PFUNC_NENKAN_SAKUSEI_SUB2(ByVal s������ As String, ByVal s�� As String, ByVal s�U�֓� As String, ByVal s�ĐU�֌� As String, ByVal s�ĐU�֓� As String, ByVal i As Integer) As Boolean
        '�X�P�W���[���@�ʏ탌�R�[�h�쐬

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUNC_NENKAN_SAKUSEI_SUB2 = False

        Dim updade As Boolean

        '���U���R�[�h�̍쐬

        '�����N���̍쐬
        STR�����N�� = PFUNC_SEIKYUTUKIHI(s������)

        '�U�֓��Z�o
        STR�U�֓� = PFUNC_FURIHI_MAKE(s��, s�U�֓�, "0", "0")

        '2010/10/21 �_��U�֓����Z�o����
        STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s��, s�U�֓�, "0", "0")

        '�ĐU���̗�������ƍĐU�֔N�A�ĐU�֌��ݒ�
        '�ĐU�֓������͂���A���N�ԃX�P�W���[���̃`�F�b�N�{�b�N�X���n�m�is�ĐU�֌��Ɍ����ݒ�j�̏ꍇ
        If s�ĐU�֌� <> "" And s�ĐU�֓� <> "" Then
            STRW�ĐU�֔N = Mid(STR�U�֓�, 1, 4)

            '2011/06/15 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� -------------START
            If STR�U�֓� <= STR�����N�� & s�ĐU�֓� Then
                'If Mid(STR�U�֓�, 7, 2) <= s�ĐU�֓� Then
                '2011/06/15 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� -------------END

                STRW�ĐU�֌� = s�ĐU�֌�
                STRW�ĐU�֓� = s�ĐU�֓�
            Else
                If s�� = "12" Then
                    STRW�ĐU�֌� = "01"
                    STRW�ĐU�֔N = CStr(CInt(Mid(STR�����N��, 1, 4)) + 1)

                Else
                    STRW�ĐU�֌� = Format((CInt(s�ĐU�֌�) + 1), "00")
                End If
                STRW�ĐU�֓� = s�ĐU�֓�
            End If
        End If

        '�ĐU�֓������͂Ȃ��A���N�ԃX�P�W���[���̃`�F�b�N�{�b�N�X���n�m
        If s�ĐU�֌� <> "" And s�ĐU�֓� = "" Then

            STRW�ĐU�֔N = Mid(STR�U�֓�, 1, 4)
            '2011/06/15 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� -------------START
            If STR�U�֓� <= STR�����N�� & GAKKOU_INFO.SFURI_DATE Then
            'If Mid(STR�U�֓�, 7, 2) <= GAKKOU_INFO.SFURI_DATE Then
                '2011/06/15 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� -------------END
                STRW�ĐU�֌� = s�ĐU�֌�
                STRW�ĐU�֓� = GAKKOU_INFO.SFURI_DATE
            Else
                If s�� = "12" Then
                    STRW�ĐU�֌� = "01"
                    STRW�ĐU�֔N = CStr(CInt(Mid(STR�����N��, 1, 4)) + 1)
                Else
                    STRW�ĐU�֌� = Format((CInt(s�ĐU�֌�) + 1), "00")
                End If
                STRW�ĐU�֓� = GAKKOU_INFO.SFURI_DATE
            End If
        End If

        If s�ĐU�֌� = "" Then
            STR�ĐU�֓� = "00000000"
        Else
            '�ĐU�֓��Z�o
            STR�ĐU�֓� = PFUNC_SAIFURIHI_MAKE(Trim(STRW�ĐU�֌�), Trim(STRW�ĐU�֓�))
        End If

        '�c�Ɠ����Z�o�������ʂŐU�֓��ƍĐU�֓�������ɂȂ�ꍇ�������
        If STR�U�֓� = STR�ĐU�֓� Then
            MessageBox.Show("(�N�ԃX�P�W���[��)" & vbCrLf & Mid(STR�U�֓�, 5, 2) & "����" & "�U�֓��ƍĐU�֓�������ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Str_FURI_DATE = STR�U�֓�
        Str_SFURI_DATE = STR�ĐU�֓�

        '�ĐU���R�[�h�̍쐬
        If STR�ĐU�֓� <> "00000000" Then

            '���U�ŋ��߂��ĐU����U�֓��ɐݒ�
            STR�U�֓� = STR�ĐU�֓�

            '2010/10/21 �ĐU�̌_��U�֓����Z�o����
            '2011/06/16 �W���ŏC�� �ĐU�������͂���Ă���ꍇ�͎��U�֓����_��U�֓��Ƃ��� ------------------START
            'STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s�ĐU�֌�, s�ĐU�֓�, "0", "1")
            If s�ĐU�֓� = "" Then
                STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s�ĐU�֌�, s�ĐU�֓�, "0", "1")
            Else
                STR�_��U�֓� = STR�U�֓�
            End If
            '2011/06/16 �W���ŏC�� �ĐU�������͂���Ă���ꍇ�͎��U�֓����_��U�֓��Ƃ��� ------------------END

            '�ĐU���̎Z�o
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"    '1(�ĐU�L/�J�z��)
                    STR�ĐU�֓� = "00000000"
                Case "2"    '2(�ĐU�L/�J�z�L)   ���񏉐U����ݒ�
                    '2011/06/16 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� ------------------START
                    ''s�ĐU�֓��Ɋw�Z�}�X�^�Q�̍ĐU�֓����Z�b�g 2005/12/09
                    'If s�ĐU�֓� = "" Then
                    '    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(Trim(s�ĐU�֌�), Trim(strSFURI_DT))
                    'Else
                    '    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(Trim(s�ĐU�֌�), Trim(s�ĐU�֓�))
                    'End If
                    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(STR�U�֓�)
                    '2011/06/16 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� ------------------END
            End Select


            '���ʃ��R�[�h�̑Ώۊw�N�̐ݒ肵����
            '�w�Z�R�[�h�A�����N���A�U�֋敪�i1:�ĐU�j
            If PFUNC_SCH_TOKUBETU_GET(STR�����N��, "1") = False Then
                MessageBox.Show("(�N�ԃX�P�W���[��)" & vbCrLf & "���ʃX�P�W���[���Ώۊw�N�ݒ�ŃG���[���������܂���(�ĐU)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '�������R�[�h�L���`�F�b�N
            '2011/06/16 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� ------------------START
            If PFUNC_SCHMAST_GET("0", "1", Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""), str�ʏ�āX�U��(i)) = True Then
                'If PFUNC_SCHMAST_GET("0", "0", Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", ""), Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", "")) = True Then
                '2011/06/16 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� ------------------END

                updade = True
            End If

            '�X�P�W���[���敪�̋��ʕϐ��ݒ�
            STR�X�P�敪 = "0"
            '�U�֋敪�̋��ʕϐ��ݒ�
            STR�U�֋敪 = "1"
            '���͐U�֓��̋��ʕϐ��ݒ�
            If s�ĐU�֓� = "" Then
                STR�N�ԓ��͐U�֓� = Space(15)
            Else
                STR�N�ԓ��͐U�֓� = s�ĐU�֓�
            End If

            '2006/11/30�@�V�K�o�^���X�V������
            Dim strSQL As String = ""
            If updade = False Then
                '�X�P�W���[���}�X�^�o�^SQL��(���U)�쐬
                strSQL = PSUB_INSERT_G_SCHMAST_SQL()
            Else
                '�X�P�W���[���}�X�^�X�VSQL��(���U)�쐬
                strSQL = PSUB_UPDATE_G_SCHMAST_SQL(Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""), str�ʏ�āX�U��(i))
            End If

            If MainDB.ExecuteNonQuery(strSQL) < 0 Then
                MessageBox.Show("�o�^�Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '-----------------------------------------------
            '2006/07/26�@��Ǝ��U�̍ĐU�̃X�P�W���[�����쐬
            '-----------------------------------------------
            '��Ǝ��U�A�g���̂�
            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(MainDB)

            '���ɓo�^����Ă��邩�`�F�b�N
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")

            If oraReader.DataReader(sql) = True Then '�X�P�W���[�������ɑ��݂���

            Else     '�X�P�W���[�������݂��Ȃ�
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then

                    '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", "��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���")
                        MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If
                End If

            End If
            oraReader.Close()
        End If

        '-----------------------------------------------
        PFUNC_NENKAN_SAKUSEI_SUB2 = True

    End Function
    '��Ƃ̃X�P�W���[���X�V�p 2006/12/08
    Private Function PFUNC_NENKAN_SAKUSEI_SUB_KIGYO(ByVal s������ As String, ByVal s�� As String, ByVal s�U�֓� As String, ByVal s�ĐU�֌� As String, ByVal s�ĐU�֓� As String) As Boolean
        '�X�P�W���[���@�ʏ탌�R�[�h(���U)�쐬

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUNC_NENKAN_SAKUSEI_SUB_KIGYO = False
        Dim updade As Boolean

        '�����N���̍쐬
        STR�����N�� = PFUNC_SEIKYUTUKIHI(s������)

        '�U�֓��Z�o
        STR�U�֓� = PFUNC_FURIHI_MAKE(s��, s�U�֓�, "0", "0")

        '*** �C�� mitsu 2009/07/29 �_��U�֓����Z�o���� ***
        STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s��, s�U�֓�, "0", "0")

        '�ĐU���̗�������ƍĐU�֔N�A�ĐU�֌��ݒ�
        '�ĐU�֓������͂���A���N�ԃX�P�W���[���̃`�F�b�N�{�b�N�X���n�m�is�ĐU�֌��Ɍ����ݒ�j�̏ꍇ
        If s�ĐU�֌� <> "" And s�ĐU�֓� <> "" Then
            STRW�ĐU�֔N = Mid(STR�U�֓�, 1, 4)

            '2011/06/16 �W���ŏC�� �N���l������ ------------------START
            'If Mid(STR�U�֓�, 7, 2) <= s�ĐU�֓� Then
            If STR�U�֓� <= STR�����N�� & s�ĐU�֓� Then
                '2011/06/16 �W���ŏC�� �N���l������ ------------------END
                STRW�ĐU�֌� = s�ĐU�֌�
                STRW�ĐU�֓� = s�ĐU�֓�
            Else
                If s�� = "12" Then
                    STRW�ĐU�֌� = "01"
                    STRW�ĐU�֔N = CStr(CInt(Mid(STR�����N��, 1, 4)) + 1)

                Else
                    STRW�ĐU�֌� = Format((CInt(s�ĐU�֌�) + 1), "00")
                End If
                STRW�ĐU�֓� = s�ĐU�֓�
            End If
        End If

        '�ĐU�֓������͂Ȃ��A���N�ԃX�P�W���[���̃`�F�b�N�{�b�N�X���n�m
        If s�ĐU�֌� <> "" And s�ĐU�֓� = "" Then

            STRW�ĐU�֔N = Mid(STR�U�֓�, 1, 4)

            '2011/06/16 �W���ŏC�� �N���l������ ------------------START
            'If Mid(STR�U�֓�, 7, 2) <= GAKKOU_INFO.SFURI_DATE Then
            If STR�U�֓� <= STR�����N�� & GAKKOU_INFO.SFURI_DATE Then
                '2011/06/16 �W���ŏC�� �N���l������ ------------------END
                'STRW�ĐU�֌� = s�ĐU�֌�
                'STRW�ĐU�֓� = GAKKOU_INFO.SFURI_DATE
                If Mid(STR�U�֓�, 5, 2) > Mid(STR�����N��, 5, 2) Then
                    If s�� = "12" Then
                        STRW�ĐU�֌� = "01"
                        STRW�ĐU�֔N = CStr(CInt(Mid(STR�����N��, 1, 4)) + 1)
                    Else
                        STRW�ĐU�֌� = Format((CInt(s�ĐU�֌�) + 1), "00")
                    End If
                    STRW�ĐU�֓� = GAKKOU_INFO.SFURI_DATE

                Else
                    STRW�ĐU�֌� = s�ĐU�֌�
                    STRW�ĐU�֓� = GAKKOU_INFO.SFURI_DATE
                End If
            Else
                If s�� = "12" Then
                    STRW�ĐU�֌� = "01"
                    STRW�ĐU�֔N = CStr(CInt(Mid(STR�����N��, 1, 4)) + 1)
                Else
                    STRW�ĐU�֌� = Format((CInt(s�ĐU�֌�) + 1), "00")
                End If
                STRW�ĐU�֓� = GAKKOU_INFO.SFURI_DATE
            End If
        End If


        If s�ĐU�֌� = "" Then
            STR�ĐU�֓� = "00000000"
        Else
            '�ĐU�֓��Z�o
            STR�ĐU�֓� = PFUNC_SAIFURIHI_MAKE(Trim(STRW�ĐU�֌�), Trim(STRW�ĐU�֓�))
        End If

        '�c�Ɠ����Z�o�������ʂŐU�֓��ƍĐU�֓�������ɂȂ�ꍇ�������
        If STR�U�֓� = STR�ĐU�֓� Then
            MessageBox.Show("(�N�ԃX�P�W���[��)" & vbCrLf & Mid(STR�U�֓�, 5, 2) & "����" & "�U�֓��ƍĐU�֓�������ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If


        '�U�֓��̗L���͈̓`�F�b�N
        If PFUNC_FURIHI_HANI_CHECK() = False Then
            MessageBox.Show("(�N�ԃX�P�W���[��)" & vbCrLf & "�U�֌_����ԁi" & GAKKOU_INFO.KAISI_DATE & "�`" & GAKKOU_INFO.SYURYOU_DATE & "�j�O�̌��ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '�X�P�W���[���敪�̋��ʕϐ��ݒ�
        STR�X�P�敪 = "0"

        '�U�֋敪�̋��ʕϐ��ݒ�
        STR�U�֋敪 = "0"

        '���͐U�֓��̋��ʕϐ��ݒ�
        If s�U�֓� = "" Then
            STR�N�ԓ��͐U�֓� = Space(15)
        Else
            STR�N�ԓ��͐U�֓� = s�U�֓�
        End If

        '-----------------------------------------------
        '2006/07/26�@��Ǝ��U�̏��U�̃X�P�W���[�����쐬
        '-----------------------------------------------
        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)

        '���ɓo�^����Ă��邩�`�F�b�N
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '01' AND ")
        '2011/06/16 �W���ŏC�� ���U�X�P�W���[���͉�ʂɕ\�����ꂽ�l����Ɍ��� ------------------START
        sql.Append("FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(Int(s��)).Furikae_Day, "/", "") & "'")
        'sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")
        '2011/06/16 �W���ŏC�� ���U�X�P�W���[���͉�ʂɕ\�����ꂽ�l����Ɍ��� ------------------END

        '�Ǎ��̂�
        If oraReader.DataReader(sql) = True Then    '�X�P�W���[�������ɑ��݂���
        Else     '�X�P�W���[�������݂��Ȃ�
            '�R�����g 2006/12/11
            'If intPUSH_BTN = 2 Then '�X�V��
            '    MessageBox.Show("��Ǝ��U���̃X�P�W���[��(" & STR�����N��.Substring(0, 4) & "�N" & STR�����N��.Substring(4, 2) & "����)�����݂��܂���" & vbCrLf & "��Ǝ��U���Ō��ԃX�P�W���[���쐬��A" & vbCrLf & "�w�Z�X�P�W���[���̍X�V�������ēx�s���Ă�������", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            'Else
            '�X�P�W���[���쐬
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, "01", gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                        gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '�����Ƀq�b�g���Ȃ�������

                '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                'If fn_INSERTSCHMAST(strGakkouCode, "01", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                '2011/06/16 �W���ŏC�� ���U�X�P�W���[���͉�ʂɕ\�����ꂽ�l����Ɍ��� ------------------START
                If fn_INSERTSCHMAST(strGakkouCode, "01", Replace(SYOKI_NENKAN_SCHINFO(Int(s��)).Furikae_Day, "/", ""), gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                    'If fn_INSERTSCHMAST(strGakkouCode, "01", STR�U�֓�, gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                    '2011/06/16 �W���ŏC�� ���U�X�P�W���[���͉�ʂɕ\�����ꂽ�l����Ɍ��� ------------------END
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", Err.Description)
                    MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oraReader.Close()
                    Return False
                End If
            End If
            'End If

        End If
        oraReader.Close()
        '-----------------------------------------------

        '�ĐU���R�[�h�̍쐬
        If STR�ĐU�֓� <> "00000000" Then

            '2006/11/30�@update�t���O�̏�����
            updade = False

            '���U�ŋ��߂��ĐU����U�֓��ɐݒ�
            STR�U�֓� = STR�ĐU�֓�

            '2010/10/21 �ĐU�̌_��U�֓����Z�o����
            '2011/06/16 �W���ŏC�� �ĐU�������͂���Ă���ꍇ�͎��U�֓����_��U�֓��Ƃ��� ------------------START
            'STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s�ĐU�֌�, s�ĐU�֓�, "0", "1")
            If s�ĐU�֓� = "" Then
                STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s�ĐU�֌�, s�ĐU�֓�, "0", "1")
            Else
                STR�_��U�֓� = STR�U�֓�
            End If
            '2011/06/16 �W���ŏC�� �ĐU�������͂���Ă���ꍇ�͎��U�֓����_��U�֓��Ƃ��� ------------------END

            '�ĐU���̎Z�o
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"    '1(�ĐU�L/�J�z��)
                    STR�ĐU�֓� = "00000000"
                Case "2"    '2(�ĐU�L/�J�z�L)   ���񏉐U����ݒ�
                    '2011/06/16 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� ------------------START
                    'If s�ĐU�֓� = "" Then
                    '    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(Trim(s�ĐU�֌�), Trim(strSFURI_DT))
                    'Else
                    '    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(Trim(s�ĐU�֌�), Trim(s�ĐU�֓�))
                    'End If
                                        STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(STR�U�֓�)
                    '2011/06/16 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� ------------------END
            End Select

            '�X�P�W���[���敪�̋��ʕϐ��ݒ�
            STR�X�P�敪 = "0"
            '�U�֋敪�̋��ʕϐ��ݒ�
            STR�U�֋敪 = "1"
            '���͐U�֓��̋��ʕϐ��ݒ�
            If s�ĐU�֓� = "" Then
                STR�N�ԓ��͐U�֓� = Space(15)
            Else
                STR�N�ԓ��͐U�֓� = s�ĐU�֓�
            End If

            STR�N�ԓ��͐U�֓� = Space(15)

            '-----------------------------------------------
            '2006/07/26�@��Ǝ��U�̍ĐU�̃X�P�W���[�����쐬
            '-----------------------------------------------
            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(MainDB)

            '���ɓo�^����Ă��邩�`�F�b�N
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            '2011/06/16 �W���ŏC�� ���U�X�P�W���[���͉�ʂɕ\�����ꂽ�l����Ɍ��� ------------------START
            sql.Append("FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(Int(s��)).SaiFurikae_Day, "/", "") & "'")
            'sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")
            '2011/06/16 �W���ŏC�� ���U�X�P�W���[���͉�ʂɕ\�����ꂽ�l����Ɍ��� ------------------END

            '�Ǎ��̂�
            If oraReader.DataReader(sql) = True Then    '�X�P�W���[�������ɑ��݂���
            Else     '�X�P�W���[�������݂��Ȃ�
                '�R�����g 2006/12/11
                'If intPUSH_BTN = 2 Then '�X�V��
                '    MessageBox.Show("��Ǝ��U���̃X�P�W���[��(" & STR�����N��.Substring(0, 4) & "�N" & STR�����N��.Substring(4, 2) & "����)�����݂��܂���" & vbCrLf & "��Ǝ��U���Ō��ԃX�P�W���[���쐬��A" & vbCrLf & "�w�Z�X�P�W���[���̍X�V�������ēx�s���Ă�������", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'Else
                '�X�P�W���[���쐬
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                            gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '�����Ƀq�b�g���Ȃ�������

                    '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    '2011/06/16 �W���ŏC�� ���U�X�P�W���[���͉�ʂɕ\�����ꂽ�l����Ɍ��� ------------------START
                    If fn_INSERTSCHMAST(strGakkouCode, "02", Replace(SYOKI_NENKAN_SCHINFO(Int(s��)).SaiFurikae_Day, "/", ""), gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                        'If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                        '2011/06/16 �W���ŏC�� ���U�X�P�W���[���͉�ʂɕ\�����ꂽ�l����Ɍ��� ------------------END
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", Err.Description)
                        MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()
        End If

        PFUNC_NENKAN_SAKUSEI_SUB_KIGYO = True

    End Function
    Private Function PFUNC_NENKAN_SAKUSEI_SUB2_KIGYO(ByVal s������ As String, ByVal s�� As String, ByVal s�U�֓� As String, ByVal s�ĐU�֌� As String, ByVal s�ĐU�֓� As String) As Boolean
        '�X�P�W���[���@�ʏ탌�R�[�h�쐬

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        PFUNC_NENKAN_SAKUSEI_SUB2_KIGYO = False

        '���U���R�[�h�̍쐬

        '�����N���̍쐬
        STR�����N�� = PFUNC_SEIKYUTUKIHI(s������)

        '�U�֓��Z�o
        STR�U�֓� = PFUNC_FURIHI_MAKE(s��, s�U�֓�, "0", "0")

        '2010/10/21 �_��U�֓����Z�o����
        STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s��, s�U�֓�, "0", "0")

        '�ĐU���̗�������ƍĐU�֔N�A�ĐU�֌��ݒ�
        '�ĐU�֓������͂���A���N�ԃX�P�W���[���̃`�F�b�N�{�b�N�X���n�m�is�ĐU�֌��Ɍ����ݒ�j�̏ꍇ
        If s�ĐU�֌� <> "" And s�ĐU�֓� <> "" Then
            STRW�ĐU�֔N = Mid(STR�U�֓�, 1, 4)

            '2011/06/16 �W���ŏC�� �N���l������ ------------------START
            'If Mid(STR�U�֓�, 7, 2) <= s�ĐU�֓� Then
            If STR�U�֓� <= STR�����N�� & s�ĐU�֓� Then            
                '2011/06/16 �W���ŏC�� �N���l������ ------------------END
                STRW�ĐU�֌� = s�ĐU�֌�
                STRW�ĐU�֓� = s�ĐU�֓�
            Else
                If s�� = "12" Then
                    STRW�ĐU�֌� = "01"
                    STRW�ĐU�֔N = CStr(CInt(Mid(STR�����N��, 1, 4)) + 1)

                Else
                    STRW�ĐU�֌� = Format((CInt(s�ĐU�֌�) + 1), "00")
                End If
                STRW�ĐU�֓� = s�ĐU�֓�
            End If
        End If

        '�ĐU�֓������͂Ȃ��A���N�ԃX�P�W���[���̃`�F�b�N�{�b�N�X���n�m
        If s�ĐU�֌� <> "" And s�ĐU�֓� = "" Then

            STRW�ĐU�֔N = Mid(STR�U�֓�, 1, 4)

            '2011/06/16 �W���ŏC�� �N���l������ ------------------START
            'If Mid(STR�U�֓�, 7, 2) <= GAKKOU_INFO.SFURI_DATE Then
            If STR�U�֓� <= STR�����N�� & GAKKOU_INFO.SFURI_DATE Then
                '2011/06/16 �W���ŏC�� �N���l������ ------------------END
                STRW�ĐU�֌� = s�ĐU�֌�
                STRW�ĐU�֓� = GAKKOU_INFO.SFURI_DATE
            Else
                If s�� = "12" Then
                    STRW�ĐU�֌� = "01"
                    STRW�ĐU�֔N = CStr(CInt(Mid(STR�����N��, 1, 4)) + 1)
                Else
                    STRW�ĐU�֌� = Format((CInt(s�ĐU�֌�) + 1), "00")
                End If
                STRW�ĐU�֓� = GAKKOU_INFO.SFURI_DATE
            End If
        End If

        If s�ĐU�֌� = "" Then
            STR�ĐU�֓� = "00000000"
        Else
            '�ĐU�֓��Z�o
            STR�ĐU�֓� = PFUNC_SAIFURIHI_MAKE(Trim(STRW�ĐU�֌�), Trim(STRW�ĐU�֓�))
        End If

        '�c�Ɠ����Z�o�������ʂŐU�֓��ƍĐU�֓�������ɂȂ�ꍇ�������
        If STR�U�֓� = STR�ĐU�֓� Then
            MessageBox.Show("(�N�ԃX�P�W���[��)" & vbCrLf & Mid(STR�U�֓�, 5, 2) & "����" & "�U�֓��ƍĐU�֓�������ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Str_FURI_DATE = STR�U�֓�
        Str_SFURI_DATE = STR�ĐU�֓�

        '�ĐU���R�[�h�̍쐬
        If STR�ĐU�֓� <> "00000000" Then

            '���U�ŋ��߂��ĐU����U�֓��ɐݒ�
            STR�U�֓� = STR�ĐU�֓�

            '2010/10/21 �ĐU�̌_��U�֓����Z�o����
            '2011/06/16 �W���ŏC�� �ĐU�������͂���Ă���ꍇ�͎��U�֓����_��U�֓��Ƃ��� ------------------START
            'STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s�ĐU�֌�, s�ĐU�֓�, "0", "1")
            If s�ĐU�֓� = "" Then
                STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s�ĐU�֌�, s�ĐU�֓�, "0", "1")
            Else
                STR�_��U�֓� = STR�U�֓�
            End If
            '2011/06/16 �W���ŏC�� �ĐU�������͂���Ă���ꍇ�͎��U�֓����_��U�֓��Ƃ��� ------------------END

            '�ĐU���̎Z�o
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"    '1(�ĐU�L/�J�z��)
                    STR�ĐU�֓� = "00000000"
                Case "2"    '2(�ĐU�L/�J�z�L)   ���񏉐U����ݒ�
                    '2011/06/16 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� ------------------START
                    ''s�ĐU�֓��Ɋw�Z�}�X�^�Q�̍ĐU�֓����Z�b�g 2005/12/09
                    'If s�ĐU�֓� = "" Then
                    '    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(Trim(s�ĐU�֌�), Trim(strSFURI_DT))
                    'Else
                    '    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(Trim(s�ĐU�֌�), Trim(s�ĐU�֓�))
                    'End If
                                        STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(STR�U�֓�)
                    '2011/06/16 �W���ŏC�� ���ۂ̐U�֓�����ĐU�����Z�o���� ------------------END
            End Select

            '�X�P�W���[���敪�̋��ʕϐ��ݒ�
            STR�X�P�敪 = "0"
            '�U�֋敪�̋��ʕϐ��ݒ�
            STR�U�֋敪 = "1"
            '���͐U�֓��̋��ʕϐ��ݒ�
            If s�ĐU�֓� = "" Then
                STR�N�ԓ��͐U�֓� = Space(15)
            Else
                STR�N�ԓ��͐U�֓� = s�ĐU�֓�
            End If

            '���ɓo�^����Ă��邩�`�F�b�N
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            '2011/06/16 �W���ŏC�� ���U�X�P�W���[���͉�ʂɕ\�����ꂽ�l����Ɍ��� ------------------START
            sql.Append("FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(Int(s��)).SaiFurikae_Day, "/", "") & "'")
            'sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")
            '2011/06/16 �W���ŏC�� ���U�X�P�W���[���͉�ʂɕ\�����ꂽ�l����Ɍ��� ------------------END

            '�Ǎ��̂�
            If oraReader.DataReader(sql) = True Then    '�X�P�W���[�������ɑ��݂���
            Else     '�X�P�W���[�������݂��Ȃ�
                '�X�P�W���[���쐬
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                            gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '�����Ƀq�b�g���Ȃ�������

                    '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    '2011/06/16 �W���ŏC�� ���U�X�P�W���[���͉�ʂɕ\�����ꂽ�l����Ɍ��� ------------------START
                    If fn_INSERTSCHMAST(strGakkouCode, "02", Replace(SYOKI_NENKAN_SCHINFO(Int(s��)).SaiFurikae_Day, "/", ""), gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                        'If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                        '2011/06/16 �W���ŏC�� ���U�X�P�W���[���͉�ʂɕ\�����ꂽ�l����Ɍ��� ------------------END
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", Err.Description)
                        MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
            End If
            oraReader.Close()
        End If

        PFUNC_NENKAN_SAKUSEI_SUB2_KIGYO = True

    End Function


    Private Function PFUNC_NENKAN_KOUSIN() As Boolean

        '�N�ԃX�P�W���[���@�X�V����

        '�폜�����iDELETE�j 2006/11/30
        If PFUNC_NENKAN_DELETE() = False Then
            Return False
        End If

        '�쐬�����iINSERT)
        If PFUNC_NENKAN_SAKUSEI() = False Then
            Return False
        End If

        Return True

    End Function

    '================================================
    '�N�ԃX�P�W���[���폜�@2006/11/30
    '================================================
    Private Function PFUNC_NENKAN_DELETE() As Boolean
        PFUNC_NENKAN_DELETE = False

        Dim sql As New StringBuilder(128)
        Dim orareader As New MyOracleReader(MainDB)

        Dim blnSakujo_Check As Boolean = False '2006/11/30

        '�S�폜�����A�L�[�͊w�Z�R�[�h�A�Ώ۔N�x�A�X�P�W���[���敪�i�O�j�A�����t���O�i�O�j
        sql.Append(" DELETE  FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S =0")
        sql.Append(" AND")
        sql.Append(" ENTRI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" CHECK_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" DATA_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" FUNOU_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" SAIFURI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" KESSAI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" TYUUDAN_FLG_S =0")

        '2006/11/30�@�����ǉ��i�ύX�̂������f�[�^�̂ݍ폜�j=========================
        For i As Integer = 1 To 12
            '�ύX������A�`�F�b�N���O��Ă�����̂��폜����
            If bln�N�ԍX�V(i) = True And NENKAN_SCHINFO(i).Furikae_Check = False And Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", "") <> "" Then
                If blnSakujo_Check = True Then
                    sql.Append(" or")
                Else
                    sql.Append(" and(")
                End If

                '�����ǉ�
                sql.Append(" FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", "") & "'")

                '�ĐU�̃X�P�W���[�����폜����
                If SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day <> "" Then
                    sql.Append(" or")
                    sql.Append(" FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", "") & "'")
                End If

                bln�N�ԍX�V(i) = False '�ύX�t���O���~�낷
                blnSakujo_Check = True '�폜�t���O�𗧂Ă�

            ElseIf bln�N�ԍX�V(i) = True And NENKAN_SCHINFO(i).SaiFurikae_Check = False And SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day <> "" Then
                If blnSakujo_Check = True Then
                    sql.Append(" or")
                Else
                    sql.Append(" and(")
                End If

                '�����ǉ�
                sql.Append(" FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", "") & "'")

                '�ĐU�̂ݍ폜�����ꍇ�A���U�̃X�P�W���[�����ύX���K�v�Ȃ̂ŕύX�t���O�͍~�낳�Ȃ�
                blnSakujo_Check = True '�폜�t���O�𗧂Ă�

            End If
        Next

        If blnSakujo_Check = True Then
            sql.Append(")")
            '�폜�f�[�^������ꍇ�̂ݎ��s����
            If MainDB.ExecuteNonQuery(sql) < 0 Then
                '�폜�����G���[
                MessageBox.Show("(�N�ԃX�P�W���[��)" & vbCrLf & "�X�P�W���[���̍폜�����ŃG���[���������܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        End If

        PFUNC_NENKAN_DELETE = True

    End Function

#End Region

#Region " Private Sub(���ʃX�P�W���[��)"
    Private Sub PSUB_TOKUBETU_GET(ByRef Get_Data() As TokubetuData)

        Get_Data(1).Seikyu_Tuki = txt���ʐ������P.Text
        Get_Data(1).Furikae_Tuki = txt���ʐU�֌��P.Text
        Get_Data(1).Furikae_Date = txt���ʐU�֓��P.Text
        Get_Data(1).SaiFurikae_Tuki = txt���ʍĐU�֌��P.Text
        Get_Data(1).SaiFurikae_Date = txt���ʍĐU�֓��P.Text

        Select Case chk�P_�S�w�N.Checked
            Case True
                Get_Data(1).SiyouGakunenALL_Check = True
                Get_Data(1).SiyouGakunen1_Check = True
                Get_Data(1).SiyouGakunen2_Check = True
                Get_Data(1).SiyouGakunen3_Check = True
                Get_Data(1).SiyouGakunen4_Check = True
                Get_Data(1).SiyouGakunen5_Check = True
                Get_Data(1).SiyouGakunen6_Check = True
                Get_Data(1).SiyouGakunen7_Check = True
                Get_Data(1).SiyouGakunen8_Check = True
                Get_Data(1).SiyouGakunen9_Check = True
            Case False
                Get_Data(1).SiyouGakunenALL_Check = False
                Get_Data(1).SiyouGakunen1_Check = chk�P_�P�w�N.Checked
                Get_Data(1).SiyouGakunen2_Check = chk�P_�Q�w�N.Checked
                Get_Data(1).SiyouGakunen3_Check = chk�P_�R�w�N.Checked
                Get_Data(1).SiyouGakunen4_Check = chk�P_�S�w�N.Checked
                Get_Data(1).SiyouGakunen5_Check = chk�P_�T�w�N.Checked
                Get_Data(1).SiyouGakunen6_Check = chk�P_�U�w�N.Checked
                Get_Data(1).SiyouGakunen7_Check = chk�P_�V�w�N.Checked
                Get_Data(1).SiyouGakunen8_Check = chk�P_�W�w�N.Checked
                Get_Data(1).SiyouGakunen9_Check = chk�P_�X�w�N.Checked
        End Select


        Get_Data(2).Seikyu_Tuki = txt���ʐ������Q.Text
        Get_Data(2).Furikae_Tuki = txt���ʐU�֌��Q.Text
        Get_Data(2).Furikae_Date = txt���ʐU�֓��Q.Text
        Get_Data(2).SaiFurikae_Tuki = txt���ʍĐU�֌��Q.Text
        Get_Data(2).SaiFurikae_Date = txt���ʍĐU�֓��Q.Text

        Select Case chk�Q_�S�w�N.Checked
            Case True
                Get_Data(2).SiyouGakunenALL_Check = True
                Get_Data(2).SiyouGakunen1_Check = True
                Get_Data(2).SiyouGakunen2_Check = True
                Get_Data(2).SiyouGakunen3_Check = True
                Get_Data(2).SiyouGakunen4_Check = True
                Get_Data(2).SiyouGakunen5_Check = True
                Get_Data(2).SiyouGakunen6_Check = True
                Get_Data(2).SiyouGakunen7_Check = True
                Get_Data(2).SiyouGakunen8_Check = True
                Get_Data(2).SiyouGakunen9_Check = True
            Case False
                Get_Data(2).SiyouGakunenALL_Check = False
                Get_Data(2).SiyouGakunen1_Check = chk�Q_�P�w�N.Checked
                Get_Data(2).SiyouGakunen2_Check = chk�Q_�Q�w�N.Checked
                Get_Data(2).SiyouGakunen3_Check = chk�Q_�R�w�N.Checked
                Get_Data(2).SiyouGakunen4_Check = chk�Q_�S�w�N.Checked
                Get_Data(2).SiyouGakunen5_Check = chk�Q_�T�w�N.Checked
                Get_Data(2).SiyouGakunen6_Check = chk�Q_�U�w�N.Checked
                Get_Data(2).SiyouGakunen7_Check = chk�Q_�V�w�N.Checked
                Get_Data(2).SiyouGakunen8_Check = chk�Q_�W�w�N.Checked
                Get_Data(2).SiyouGakunen9_Check = chk�Q_�X�w�N.Checked
        End Select


        Get_Data(3).Seikyu_Tuki = txt���ʐ������R.Text
        Get_Data(3).Furikae_Tuki = txt���ʐU�֌��R.Text
        Get_Data(3).Furikae_Date = txt���ʐU�֓��R.Text
        Get_Data(3).SaiFurikae_Tuki = txt���ʍĐU�֌��R.Text
        Get_Data(3).SaiFurikae_Date = txt���ʍĐU�֓��R.Text

        Select Case chk�R_�S�w�N.Checked
            Case True
                Get_Data(3).SiyouGakunenALL_Check = True
                Get_Data(3).SiyouGakunen1_Check = True
                Get_Data(3).SiyouGakunen2_Check = True
                Get_Data(3).SiyouGakunen3_Check = True
                Get_Data(3).SiyouGakunen4_Check = True
                Get_Data(3).SiyouGakunen5_Check = True
                Get_Data(3).SiyouGakunen6_Check = True
                Get_Data(3).SiyouGakunen7_Check = True
                Get_Data(3).SiyouGakunen8_Check = True
                Get_Data(3).SiyouGakunen9_Check = True
            Case False
                Get_Data(3).SiyouGakunenALL_Check = False
                Get_Data(3).SiyouGakunen1_Check = chk�R_�P�w�N.Checked
                Get_Data(3).SiyouGakunen2_Check = chk�R_�Q�w�N.Checked
                Get_Data(3).SiyouGakunen3_Check = chk�R_�R�w�N.Checked
                Get_Data(3).SiyouGakunen4_Check = chk�R_�S�w�N.Checked
                Get_Data(3).SiyouGakunen5_Check = chk�R_�T�w�N.Checked
                Get_Data(3).SiyouGakunen6_Check = chk�R_�U�w�N.Checked
                Get_Data(3).SiyouGakunen7_Check = chk�R_�V�w�N.Checked
                Get_Data(3).SiyouGakunen8_Check = chk�R_�W�w�N.Checked
                Get_Data(3).SiyouGakunen9_Check = chk�R_�X�w�N.Checked
        End Select


        Get_Data(4).Seikyu_Tuki = txt���ʐ������S.Text
        Get_Data(4).Furikae_Tuki = txt���ʐU�֌��S.Text
        Get_Data(4).Furikae_Date = txt���ʐU�֓��S.Text
        Get_Data(4).SaiFurikae_Tuki = txt���ʍĐU�֌��S.Text
        Get_Data(4).SaiFurikae_Date = txt���ʍĐU�֓��S.Text

        Select Case chk�S_�S�w�N.Checked
            Case True
                Get_Data(4).SiyouGakunenALL_Check = True
                Get_Data(4).SiyouGakunen1_Check = True
                Get_Data(4).SiyouGakunen2_Check = True
                Get_Data(4).SiyouGakunen3_Check = True
                Get_Data(4).SiyouGakunen4_Check = True
                Get_Data(4).SiyouGakunen5_Check = True
                Get_Data(4).SiyouGakunen6_Check = True
                Get_Data(4).SiyouGakunen7_Check = True
                Get_Data(4).SiyouGakunen8_Check = True
                Get_Data(4).SiyouGakunen9_Check = True
            Case False
                Get_Data(4).SiyouGakunenALL_Check = False
                Get_Data(4).SiyouGakunen1_Check = chk�S_�P�w�N.Checked
                Get_Data(4).SiyouGakunen2_Check = chk�S_�Q�w�N.Checked
                Get_Data(4).SiyouGakunen3_Check = chk�S_�R�w�N.Checked
                Get_Data(4).SiyouGakunen4_Check = chk�S_�S�w�N.Checked
                Get_Data(4).SiyouGakunen5_Check = chk�S_�T�w�N.Checked
                Get_Data(4).SiyouGakunen6_Check = chk�S_�U�w�N.Checked
                Get_Data(4).SiyouGakunen7_Check = chk�S_�V�w�N.Checked
                Get_Data(4).SiyouGakunen8_Check = chk�S_�W�w�N.Checked
                Get_Data(4).SiyouGakunen9_Check = chk�S_�X�w�N.Checked
        End Select


        Get_Data(5).Seikyu_Tuki = txt���ʐ������T.Text
        Get_Data(5).Furikae_Tuki = txt���ʐU�֌��T.Text
        Get_Data(5).Furikae_Date = txt���ʐU�֓��T.Text
        Get_Data(5).SaiFurikae_Tuki = txt���ʍĐU�֌��T.Text
        Get_Data(5).SaiFurikae_Date = txt���ʍĐU�֓��T.Text

        Select Case chk�T_�S�w�N.Checked
            Case True
                Get_Data(5).SiyouGakunenALL_Check = True
                Get_Data(5).SiyouGakunen1_Check = True
                Get_Data(5).SiyouGakunen2_Check = True
                Get_Data(5).SiyouGakunen3_Check = True
                Get_Data(5).SiyouGakunen4_Check = True
                Get_Data(5).SiyouGakunen5_Check = True
                Get_Data(5).SiyouGakunen6_Check = True
                Get_Data(5).SiyouGakunen7_Check = True
                Get_Data(5).SiyouGakunen8_Check = True
                Get_Data(5).SiyouGakunen9_Check = True
            Case False
                Get_Data(5).SiyouGakunenALL_Check = False
                Get_Data(5).SiyouGakunen1_Check = chk�T_�P�w�N.Checked
                Get_Data(5).SiyouGakunen2_Check = chk�T_�Q�w�N.Checked
                Get_Data(5).SiyouGakunen3_Check = chk�T_�R�w�N.Checked
                Get_Data(5).SiyouGakunen4_Check = chk�T_�S�w�N.Checked
                Get_Data(5).SiyouGakunen5_Check = chk�T_�T�w�N.Checked
                Get_Data(5).SiyouGakunen6_Check = chk�T_�U�w�N.Checked
                Get_Data(5).SiyouGakunen7_Check = chk�T_�V�w�N.Checked
                Get_Data(5).SiyouGakunen8_Check = chk�T_�W�w�N.Checked
                Get_Data(5).SiyouGakunen9_Check = chk�T_�X�w�N.Checked
        End Select

        Get_Data(6).Seikyu_Tuki = txt���ʐ������U.Text
        Get_Data(6).Furikae_Tuki = txt���ʐU�֌��U.Text
        Get_Data(6).Furikae_Date = txt���ʐU�֓��U.Text
        Get_Data(6).SaiFurikae_Tuki = txt���ʍĐU�֌��U.Text
        Get_Data(6).SaiFurikae_Date = txt���ʍĐU�֓��U.Text

        Select Case chk�U_�S�w�N.Checked
            Case True
                Get_Data(6).SiyouGakunenALL_Check = True
                Get_Data(6).SiyouGakunen1_Check = True
                Get_Data(6).SiyouGakunen2_Check = True
                Get_Data(6).SiyouGakunen3_Check = True
                Get_Data(6).SiyouGakunen4_Check = True
                Get_Data(6).SiyouGakunen5_Check = True
                Get_Data(6).SiyouGakunen6_Check = True
                Get_Data(6).SiyouGakunen7_Check = True
                Get_Data(6).SiyouGakunen8_Check = True
                Get_Data(6).SiyouGakunen9_Check = True
            Case False
                Get_Data(6).SiyouGakunenALL_Check = False
                Get_Data(6).SiyouGakunen1_Check = chk�U_�P�w�N.Checked
                Get_Data(6).SiyouGakunen2_Check = chk�U_�Q�w�N.Checked
                Get_Data(6).SiyouGakunen3_Check = chk�U_�R�w�N.Checked
                Get_Data(6).SiyouGakunen4_Check = chk�U_�S�w�N.Checked
                Get_Data(6).SiyouGakunen5_Check = chk�U_�T�w�N.Checked
                Get_Data(6).SiyouGakunen6_Check = chk�U_�U�w�N.Checked
                Get_Data(6).SiyouGakunen7_Check = chk�U_�V�w�N.Checked
                Get_Data(6).SiyouGakunen8_Check = chk�U_�W�w�N.Checked
                Get_Data(6).SiyouGakunen9_Check = chk�U_�X�w�N.Checked
        End Select

    End Sub

#End Region

#Region " Private Sub(���ʃX�P�W���[����ʐ���)"
    Private Sub PSUB_TOKUBETU_FORMAT(Optional ByVal pIndex As Integer = 1)

        'Select case pIndex
        '    Case 0
        '�Ώۊw�N�`�F�b�N�a�n�w�̗L����
        Call PSUB_TOKUBETU_CHKBOXEnabled(True)
        'End Select

        '�����Ώۊw�N�w��`�F�b�NOFF
        Call PSUB_TOKUBETU_CHK(False)

        '�U�֓����͗��A�ĐU�֓����͗��̃N���A
        Call PSUB_TOKUBETU_DAYCLER()

    End Sub
    Private Sub PSUB_TOKUBETU_CHKBOXEnabled(ByVal pValue As Boolean)

        '�Ώۊw�N�`�F�b�NBOX�̗L����
        chk�P_�P�w�N.Enabled = pValue
        chk�P_�Q�w�N.Enabled = pValue
        chk�P_�R�w�N.Enabled = pValue
        chk�P_�S�w�N.Enabled = pValue
        chk�P_�T�w�N.Enabled = pValue
        chk�P_�U�w�N.Enabled = pValue
        chk�P_�V�w�N.Enabled = pValue
        chk�P_�W�w�N.Enabled = pValue
        chk�P_�X�w�N.Enabled = pValue
        chk�P_�S�w�N.Enabled = pValue

        chk�Q_�P�w�N.Enabled = pValue
        chk�Q_�Q�w�N.Enabled = pValue
        chk�Q_�R�w�N.Enabled = pValue
        chk�Q_�S�w�N.Enabled = pValue
        chk�Q_�T�w�N.Enabled = pValue
        chk�Q_�U�w�N.Enabled = pValue
        chk�Q_�V�w�N.Enabled = pValue
        chk�Q_�W�w�N.Enabled = pValue
        chk�Q_�X�w�N.Enabled = pValue
        chk�Q_�S�w�N.Enabled = pValue

        chk�R_�P�w�N.Enabled = pValue
        chk�R_�Q�w�N.Enabled = pValue
        chk�R_�R�w�N.Enabled = pValue
        chk�R_�S�w�N.Enabled = pValue
        chk�R_�T�w�N.Enabled = pValue
        chk�R_�U�w�N.Enabled = pValue
        chk�R_�V�w�N.Enabled = pValue
        chk�R_�W�w�N.Enabled = pValue
        chk�R_�X�w�N.Enabled = pValue
        chk�R_�S�w�N.Enabled = pValue

        chk�S_�P�w�N.Enabled = pValue
        chk�S_�Q�w�N.Enabled = pValue
        chk�S_�R�w�N.Enabled = pValue
        chk�S_�S�w�N.Enabled = pValue
        chk�S_�T�w�N.Enabled = pValue
        chk�S_�U�w�N.Enabled = pValue
        chk�S_�V�w�N.Enabled = pValue
        chk�S_�W�w�N.Enabled = pValue
        chk�S_�X�w�N.Enabled = pValue
        chk�S_�S�w�N.Enabled = pValue

        chk�T_�P�w�N.Enabled = pValue
        chk�T_�Q�w�N.Enabled = pValue
        chk�T_�R�w�N.Enabled = pValue
        chk�T_�S�w�N.Enabled = pValue
        chk�T_�T�w�N.Enabled = pValue
        chk�T_�U�w�N.Enabled = pValue
        chk�T_�V�w�N.Enabled = pValue
        chk�T_�W�w�N.Enabled = pValue
        chk�T_�X�w�N.Enabled = pValue
        chk�T_�S�w�N.Enabled = pValue

        chk�U_�P�w�N.Enabled = pValue
        chk�U_�Q�w�N.Enabled = pValue
        chk�U_�R�w�N.Enabled = pValue
        chk�U_�S�w�N.Enabled = pValue
        chk�U_�T�w�N.Enabled = pValue
        chk�U_�U�w�N.Enabled = pValue
        chk�U_�V�w�N.Enabled = pValue
        chk�U_�W�w�N.Enabled = pValue
        chk�U_�X�w�N.Enabled = pValue
        chk�U_�S�w�N.Enabled = pValue

    End Sub
    Private Sub PSUB_TOKUBETU_DAYCLER()

        '�������̃N���A����
        txt���ʐ������P.Text = ""
        txt���ʐ������Q.Text = ""
        txt���ʐ������R.Text = ""
        txt���ʐ������S.Text = ""
        txt���ʐ������T.Text = ""
        txt���ʐ������U.Text = ""

        '�U�֓��̃N���A����
        txt���ʐU�֌��P.Text = ""
        txt���ʐU�֓��P.Text = ""
        txt���ʐU�֌��Q.Text = ""
        txt���ʐU�֓��Q.Text = ""
        txt���ʐU�֌��R.Text = ""
        txt���ʐU�֓��R.Text = ""
        txt���ʐU�֌��S.Text = ""
        txt���ʐU�֓��S.Text = ""
        txt���ʐU�֌��T.Text = ""
        txt���ʐU�֓��T.Text = ""
        txt���ʐU�֌��U.Text = ""
        txt���ʐU�֓��U.Text = ""

        '�ĐU�֓��̃N���A����
        txt���ʍĐU�֌��P.Text = ""
        txt���ʍĐU�֓��P.Text = ""
        txt���ʍĐU�֌��Q.Text = ""
        txt���ʍĐU�֓��Q.Text = ""
        txt���ʍĐU�֌��R.Text = ""
        txt���ʍĐU�֓��R.Text = ""
        txt���ʍĐU�֌��S.Text = ""
        txt���ʍĐU�֓��S.Text = ""
        txt���ʍĐU�֌��T.Text = ""
        txt���ʍĐU�֓��T.Text = ""
        txt���ʍĐU�֌��U.Text = ""
        txt���ʍĐU�֓��U.Text = ""

    End Sub
    Private Sub PSUB_TOKUBETU_CHK(ByVal pValue As Boolean)

        '�Ώۊw�N�L���`�F�b�NOFF
        chk�P_�P�w�N.Checked = pValue
        chk�P_�Q�w�N.Checked = pValue
        chk�P_�R�w�N.Checked = pValue
        chk�P_�S�w�N.Checked = pValue
        chk�P_�T�w�N.Checked = pValue
        chk�P_�U�w�N.Checked = pValue
        chk�P_�V�w�N.Checked = pValue
        chk�P_�W�w�N.Checked = pValue
        chk�P_�X�w�N.Checked = pValue
        chk�P_�S�w�N.Checked = pValue

        chk�Q_�P�w�N.Checked = pValue
        chk�Q_�Q�w�N.Checked = pValue
        chk�Q_�R�w�N.Checked = pValue
        chk�Q_�S�w�N.Checked = pValue
        chk�Q_�T�w�N.Checked = pValue
        chk�Q_�U�w�N.Checked = pValue
        chk�Q_�V�w�N.Checked = pValue
        chk�Q_�W�w�N.Checked = pValue
        chk�Q_�X�w�N.Checked = pValue
        chk�Q_�S�w�N.Checked = pValue

        chk�R_�P�w�N.Checked = pValue
        chk�R_�Q�w�N.Checked = pValue
        chk�R_�R�w�N.Checked = pValue
        chk�R_�S�w�N.Checked = pValue
        chk�R_�T�w�N.Checked = pValue
        chk�R_�U�w�N.Checked = pValue
        chk�R_�V�w�N.Checked = pValue
        chk�R_�W�w�N.Checked = pValue
        chk�R_�X�w�N.Checked = pValue
        chk�R_�S�w�N.Checked = pValue

        chk�S_�P�w�N.Checked = pValue
        chk�S_�Q�w�N.Checked = pValue
        chk�S_�R�w�N.Checked = pValue
        chk�S_�S�w�N.Checked = pValue
        chk�S_�T�w�N.Checked = pValue
        chk�S_�U�w�N.Checked = pValue
        chk�S_�V�w�N.Checked = pValue
        chk�S_�W�w�N.Checked = pValue
        chk�S_�X�w�N.Checked = pValue
        chk�S_�S�w�N.Checked = pValue

        chk�T_�P�w�N.Checked = pValue
        chk�T_�Q�w�N.Checked = pValue
        chk�T_�R�w�N.Checked = pValue
        chk�T_�S�w�N.Checked = pValue
        chk�T_�T�w�N.Checked = pValue
        chk�T_�U�w�N.Checked = pValue
        chk�T_�V�w�N.Checked = pValue
        chk�T_�W�w�N.Checked = pValue
        chk�T_�X�w�N.Checked = pValue
        chk�T_�S�w�N.Checked = pValue

        chk�U_�P�w�N.Checked = pValue
        chk�U_�Q�w�N.Checked = pValue
        chk�U_�R�w�N.Checked = pValue
        chk�U_�S�w�N.Checked = pValue
        chk�U_�T�w�N.Checked = pValue
        chk�U_�U�w�N.Checked = pValue
        chk�U_�V�w�N.Checked = pValue
        chk�U_�W�w�N.Checked = pValue
        chk�U_�X�w�N.Checked = pValue
        chk�U_�S�w�N.Checked = pValue

    End Sub

    Private Sub PSUB_TOKUBETU_SET(ByVal txtbox������ As TextBox, ByVal txtbox�� As TextBox, ByVal txtbox�� As TextBox, ByVal chkbox1 As CheckBox, ByVal chkbox2 As CheckBox, ByVal chkbox3 As CheckBox, ByVal chkbox4 As CheckBox, ByVal chkbox5 As CheckBox, ByVal chkbox6 As CheckBox, ByVal chkbox7 As CheckBox, ByVal chkbox8 As CheckBox, ByVal chkbox9 As CheckBox, ByVal chkboxALL As CheckBox, ByVal aReader As MyOracleReader)

        '���ʐU�֓��@�Q�ƃ{�^�����ʕҏW

        '�������̐ݒ�
        txtbox������.Text = Mid(aReader.GetString("NENGETUDO_S"), 5, 2)

        '�U�֌��̐ݒ�
        txtbox��.Text = Mid(aReader.GetString("FURI_DATE_S"), 5, 2)

        '�U�֓��̐ݒ�
        txtbox��.Text = Mid(aReader.GetString("FURI_DATE_S"), 7, 2)

        Select Case CInt(aReader.GetString("FURI_KBN_S"))
            Case 0
                Select Case True
                    Case aReader.GetString("ENTRI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("CHECK_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("DATA_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("FUNOU_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("SAIFURI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("KESSAI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("TYUUDAN_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                End Select
            Case 1
                Select Case True
                    Case aReader.GetString("ENTRI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("CHECK_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                        '2006/11/30�@�`�F�b�N�t���O���擾
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                    Case aReader.GetString("DATA_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("FUNOU_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("SAIFURI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("KESSAI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("TYUUDAN_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                End Select
        End Select

        If aReader.GetString("GAKUNEN1_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN2_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN3_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN4_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN5_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN6_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN7_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN8_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN9_FLG_S") = "1" Then

            '�S�w�N�`�F�b�N�{�b�N�X�n�m
            chkboxALL.Checked = True

            '�P����X�w�N�`�F�b�N�{�N�X�̎g�p�s��
            chkbox1.Enabled = False
            chkbox2.Enabled = False
            chkbox3.Enabled = False
            chkbox4.Enabled = False
            chkbox5.Enabled = False
            chkbox6.Enabled = False
            chkbox7.Enabled = False
            chkbox8.Enabled = False
            chkbox9.Enabled = False
        Else
            If aReader.GetString("GAKUNEN1_FLG_S") = "1" Then
                '�P�w�N�`�F�b�N�{�b�N�X�n�m
                chkbox1.Checked = True
            Else
                chkbox1.Checked = False
            End If

            If aReader.GetString("GAKUNEN2_FLG_S") = "1" Then
                '�Q�w�N�`�F�b�N�{�b�N�X�n�m
                chkbox2.Checked = True
            Else
                chkbox2.Checked = False
            End If

            If aReader.GetString("GAKUNEN3_FLG_S") = "1" Then
                '�R�w�N�`�F�b�N�{�b�N�X�n�m
                chkbox3.Checked = True
            Else
                chkbox3.Checked = False
            End If

            If aReader.GetString("GAKUNEN4_FLG_S") = "1" Then
                '�S�w�N�`�F�b�N�{�b�N�X�n�m
                chkbox4.Checked = True
            Else
                chkbox4.Checked = False
            End If

            If aReader.GetString("GAKUNEN5_FLG_S") = "1" Then
                '�T�w�N�`�F�b�N�{�b�N�X�n�m
                chkbox5.Checked = True
            Else
                chkbox5.Checked = False
            End If

            If aReader.GetString("GAKUNEN6_FLG_S") = "1" Then
                '�U�w�N�`�F�b�N�{�b�N�X�n�m
                chkbox6.Checked = True
            Else
                chkbox6.Checked = False
            End If

            If aReader.GetString("GAKUNEN7_FLG_S") = "1" Then
                '�V�w�N�`�F�b�N�{�b�N�X�n�m
                chkbox7.Checked = True
            Else
                chkbox7.Checked = False
            End If

            If aReader.GetString("GAKUNEN8_FLG_S") = "1" Then
                '�W�w�N�`�F�b�N�{�b�N�X�n�m
                chkbox8.Checked = True
            Else
                chkbox8.Checked = False
            End If

            If aReader.GetString("GAKUNEN9_FLG_S") = "1" Then
                '�X�w�N�`�F�b�N�{�b�N�X�n�m
                chkbox9.Checked = True
            Else
                chkbox9.Checked = False
            End If
        End If

    End Sub
    Private Sub PSUB_TGAKUNEN_CHK()
        '2006/10/12�@�g�p���Ă��Ȃ��w�N�̃`�F�b�N�{�b�N�X���g�p�s�ɂ���

        If GAKKOU_INFO.SIYOU_GAKUNEN <> 9 Then
            chk�P_�X�w�N.Enabled = False
            chk�Q_�X�w�N.Enabled = False
            chk�R_�X�w�N.Enabled = False
            chk�S_�X�w�N.Enabled = False
            chk�T_�X�w�N.Enabled = False
            chk�U_�X�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 8 Then
            chk�P_�W�w�N.Enabled = False
            chk�Q_�W�w�N.Enabled = False
            chk�R_�W�w�N.Enabled = False
            chk�S_�W�w�N.Enabled = False
            chk�T_�W�w�N.Enabled = False
            chk�U_�W�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 7 Then
            chk�P_�V�w�N.Enabled = False
            chk�Q_�V�w�N.Enabled = False
            chk�R_�V�w�N.Enabled = False
            chk�S_�V�w�N.Enabled = False
            chk�T_�V�w�N.Enabled = False
            chk�U_�V�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 6 Then
            chk�P_�U�w�N.Enabled = False
            chk�Q_�U�w�N.Enabled = False
            chk�R_�U�w�N.Enabled = False
            chk�S_�U�w�N.Enabled = False
            chk�T_�U�w�N.Enabled = False
            chk�U_�U�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 5 Then
            chk�P_�T�w�N.Enabled = False
            chk�Q_�T�w�N.Enabled = False
            chk�R_�T�w�N.Enabled = False
            chk�S_�T�w�N.Enabled = False
            chk�T_�T�w�N.Enabled = False
            chk�U_�T�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 4 Then
            chk�P_�S�w�N.Enabled = False
            chk�Q_�S�w�N.Enabled = False
            chk�R_�S�w�N.Enabled = False
            chk�S_�S�w�N.Enabled = False
            chk�T_�S�w�N.Enabled = False
            chk�U_�S�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 3 Then
            chk�P_�R�w�N.Enabled = False
            chk�Q_�R�w�N.Enabled = False
            chk�R_�R�w�N.Enabled = False
            chk�S_�R�w�N.Enabled = False
            chk�T_�R�w�N.Enabled = False
            chk�U_�R�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 2 Then
            chk�P_�Q�w�N.Enabled = False
            chk�Q_�Q�w�N.Enabled = False
            chk�R_�Q�w�N.Enabled = False
            chk�S_�Q�w�N.Enabled = False
            chk�T_�Q�w�N.Enabled = False
            chk�U_�Q�w�N.Enabled = False
        End If
    End Sub

    '=============================================
    '�w�N�t���O��񎟌��z��Ɋi�[����@2006/11/30
    '=============================================
    Private Sub PSUB_GAKUNENFLG_GET(ByRef strGakunen_FLG(,) As Boolean)

        strGakunen_FLG(1, 1) = chk�P_�P�w�N.Checked
        strGakunen_FLG(1, 2) = chk�P_�Q�w�N.Checked
        strGakunen_FLG(1, 3) = chk�P_�R�w�N.Checked
        strGakunen_FLG(1, 4) = chk�P_�S�w�N.Checked
        strGakunen_FLG(1, 5) = chk�P_�T�w�N.Checked
        strGakunen_FLG(1, 6) = chk�P_�U�w�N.Checked
        strGakunen_FLG(1, 7) = chk�P_�V�w�N.Checked
        strGakunen_FLG(1, 8) = chk�P_�W�w�N.Checked
        strGakunen_FLG(1, 9) = chk�P_�X�w�N.Checked
        strGakunen_FLG(1, 10) = chk�P_�S�w�N.Checked

        strGakunen_FLG(2, 1) = chk�Q_�P�w�N.Checked
        strGakunen_FLG(2, 2) = chk�Q_�Q�w�N.Checked
        strGakunen_FLG(2, 3) = chk�Q_�R�w�N.Checked
        strGakunen_FLG(2, 4) = chk�Q_�S�w�N.Checked
        strGakunen_FLG(2, 5) = chk�Q_�T�w�N.Checked
        strGakunen_FLG(2, 6) = chk�Q_�U�w�N.Checked
        strGakunen_FLG(2, 7) = chk�Q_�V�w�N.Checked
        strGakunen_FLG(2, 8) = chk�Q_�W�w�N.Checked
        strGakunen_FLG(2, 9) = chk�Q_�X�w�N.Checked
        strGakunen_FLG(2, 10) = chk�Q_�S�w�N.Checked

        strGakunen_FLG(3, 1) = chk�R_�P�w�N.Checked
        strGakunen_FLG(3, 2) = chk�R_�Q�w�N.Checked
        strGakunen_FLG(3, 3) = chk�R_�R�w�N.Checked
        strGakunen_FLG(3, 4) = chk�R_�S�w�N.Checked
        strGakunen_FLG(3, 5) = chk�R_�T�w�N.Checked
        strGakunen_FLG(3, 6) = chk�R_�U�w�N.Checked
        strGakunen_FLG(3, 7) = chk�R_�V�w�N.Checked
        strGakunen_FLG(3, 8) = chk�R_�W�w�N.Checked
        strGakunen_FLG(3, 9) = chk�R_�X�w�N.Checked
        strGakunen_FLG(3, 10) = chk�R_�S�w�N.Checked

        strGakunen_FLG(4, 1) = chk�S_�P�w�N.Checked
        strGakunen_FLG(4, 2) = chk�S_�Q�w�N.Checked
        strGakunen_FLG(4, 3) = chk�S_�R�w�N.Checked
        strGakunen_FLG(4, 4) = chk�S_�S�w�N.Checked
        strGakunen_FLG(4, 5) = chk�S_�T�w�N.Checked
        strGakunen_FLG(4, 6) = chk�S_�U�w�N.Checked
        strGakunen_FLG(4, 7) = chk�S_�V�w�N.Checked
        strGakunen_FLG(4, 8) = chk�S_�W�w�N.Checked
        strGakunen_FLG(4, 9) = chk�S_�X�w�N.Checked
        strGakunen_FLG(4, 10) = chk�S_�S�w�N.Checked

        strGakunen_FLG(5, 1) = chk�T_�P�w�N.Checked
        strGakunen_FLG(5, 2) = chk�T_�Q�w�N.Checked
        strGakunen_FLG(5, 3) = chk�T_�R�w�N.Checked
        strGakunen_FLG(5, 4) = chk�T_�S�w�N.Checked
        strGakunen_FLG(5, 5) = chk�T_�T�w�N.Checked
        strGakunen_FLG(5, 6) = chk�T_�U�w�N.Checked
        strGakunen_FLG(5, 7) = chk�T_�V�w�N.Checked
        strGakunen_FLG(5, 8) = chk�T_�W�w�N.Checked
        strGakunen_FLG(5, 9) = chk�T_�X�w�N.Checked
        strGakunen_FLG(5, 10) = chk�T_�S�w�N.Checked

        strGakunen_FLG(6, 1) = chk�U_�P�w�N.Checked
        strGakunen_FLG(6, 2) = chk�U_�Q�w�N.Checked
        strGakunen_FLG(6, 3) = chk�U_�R�w�N.Checked
        strGakunen_FLG(6, 4) = chk�U_�S�w�N.Checked
        strGakunen_FLG(6, 5) = chk�U_�T�w�N.Checked
        strGakunen_FLG(6, 6) = chk�U_�U�w�N.Checked
        strGakunen_FLG(6, 7) = chk�U_�V�w�N.Checked
        strGakunen_FLG(6, 8) = chk�U_�W�w�N.Checked
        strGakunen_FLG(6, 9) = chk�U_�X�w�N.Checked
        strGakunen_FLG(6, 10) = chk�U_�S�w�N.Checked

    End Sub

#End Region

#Region " Private Function(���ʃX�P�W���[��)"
    Private Function PFUNC_SCH_GET_TOKUBETU() As Boolean

        PFUNC_SCH_GET_TOKUBETU = False

        '���ʐU�֓�
        '�Ώۊw�N�`�F�b�N�a�n�w�̗L����
        Call PSUB_TOKUBETU_CHKBOXEnabled(True)

        '�����Ώۊw�N�w��`�F�b�NOFF
        Call PSUB_TOKUBETU_CHK(False)

        '�U�֓����͗��A�ĐU�֓����͗��̃N���A
        Call PSUB_TOKUBETU_DAYCLER()

        '���ʐU�֓��Q�Ə���
        If PFUNC_TOKUBETU_SANSYOU() = False Then
            Exit Function
        End If

        PFUNC_SCH_GET_TOKUBETU = True

    End Function

    Private Function PFUNC_SCH_DELETE_INSERT_TOKUBETU() As Boolean

        '���ʃX�P�W���[���X�V����
        If PFUNC_TOKUBETU_KOUSIN() = False Then

            '������ʂ�Ƃ������Ƃ͂P���ł������������R�[�h�����݂����Ƃ������ƂȂ̂�
            Int_Syori_Flag(1) = 2

            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_SCH_TOKUBETU_GET(ByVal strNENGETUDO As String, ByVal strFURIKUBUN As String) As Boolean


        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        Try

            PFUNC_SCH_TOKUBETU_GET = False

            '���ʃX�P�W���[���̃��R�[�h���݃`�F�b�N

            sql.Append(" SELECT * FROM G_SCHMAST")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
            sql.Append(" AND")
            sql.Append(" NENGETUDO_S = '" & strNENGETUDO & "'")
            sql.Append(" AND")
            sql.Append(" SCH_KBN_S = '1'")
            sql.Append(" AND")
            sql.Append(" FURI_KBN_S = " & "'" & strFURIKUBUN & "'")

            If oraReader.DataReader(sql) = True Then '���݂����

                '���ʃ��R�[�h�̑Ώۊw�N�����ɁA�ʏ탌�R�[�h�̑Ώۊw�N��ݒ肵����
                '�����ʃX�P�W���[���Ŏw�肳��Ă���w�N�͔N�ԃX�P�W���[���ł͎w�肵�Ȃ�
                Do Until oraReader.EOF
                    If oraReader.GetString("GAKUNEN1_FLG_S") = "1" Then
                        STR�P�w�N = "0"
                    End If
                    If oraReader.GetString("GAKUNEN2_FLG_S") = "1" Then
                        STR�Q�w�N = "0"
                    End If
                    If oraReader.GetString("GAKUNEN3_FLG_S") = "1" Then
                        STR�R�w�N = "0"
                    End If
                    If oraReader.GetString("GAKUNEN4_FLG_S") = "1" Then
                        STR�S�w�N = "0"
                    End If
                    If oraReader.GetString("GAKUNEN5_FLG_S") = "1" Then
                        STR�T�w�N = "0"
                    End If
                    If oraReader.GetString("GAKUNEN6_FLG_S") = "1" Then
                        STR�U�w�N = "0"
                    End If
                    If oraReader.GetString("GAKUNEN7_FLG_S") = "1" Then
                        STR�V�w�N = "0"
                    End If
                    If oraReader.GetString("GAKUNEN8_FLG_S") = "1" Then
                        STR�W�w�N = "0"
                    End If
                    If oraReader.GetString("GAKUNEN9_FLG_S") = "1" Then
                        STR�X�w�N = "0"
                    End If
                    oraReader.NextRead()
                Loop

            Else    '���݂��Ȃ����True
                PFUNC_SCH_TOKUBETU_GET = True
                Return True
            End If

            PFUNC_SCH_TOKUBETU_GET = True

        Catch ex As Exception
            Throw
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

    End Function

    Private Function PFUNC_TOKUBETU_SANSYOU() As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '���ʐU�֓��@�Q�Ə���
        PFUNC_TOKUBETU_SANSYOU = False

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = 1")
        sql.Append(" ORDER BY FURI_KBN_S asc , FURI_DATE_S ASC")

        If oraReader.DataReader(sql) = False Then
            oraReader.Close()
            Exit Function
        End If

        Do Until oraReader.EOF

            Select Case oraReader.GetString("FURI_KBN_S")
                Case "0"
                    '�܂��l���ݒ肳��Ă��Ȃ��s�ɓ��ʃX�P�W���[����ݒ肷��
                    Select Case True
                        Case (txt���ʐU�֌��P.Text = "")
                            Int_Tokubetu_Flag = 1
                            Call PSUB_TOKUBETU_SET(txt���ʐ������P, txt���ʐU�֌��P, txt���ʐU�֓��P, chk�P_�P�w�N, chk�P_�Q�w�N, chk�P_�R�w�N, chk�P_�S�w�N, chk�P_�T�w�N, chk�P_�U�w�N, chk�P_�V�w�N, chk�P_�W�w�N, chk�P_�X�w�N, chk�P_�S�w�N, oraReader)

                            '�U�֓��ƍĐU�֓��̕\����̑Ή��֌W�i�Z�b�g�j���Ƃ邽�߁A�^�O�ɐU�֓����R�[�h���̍ĐU�֓����ꎞ�ۑ�����
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt���ʐU�֌��P.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            '2006/11/30�@�`�F�b�N�t���O���擾
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            '2006/11/30�@�s�\�t���O���擾
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt���ʐU�֌��Q.Text = "")
                            Int_Tokubetu_Flag = 2
                            Call PSUB_TOKUBETU_SET(txt���ʐ������Q, txt���ʐU�֌��Q, txt���ʐU�֓��Q, chk�Q_�P�w�N, chk�Q_�Q�w�N, chk�Q_�R�w�N, chk�Q_�S�w�N, chk�Q_�T�w�N, chk�Q_�U�w�N, chk�Q_�V�w�N, chk�Q_�W�w�N, chk�Q_�X�w�N, chk�Q_�S�w�N, oraReader)

                            '�U�֓��ƍĐU�֓��̕\����̑Ή��֌W�i�Z�b�g�j���Ƃ邽�߁A�^�O�ɐU�֓����R�[�h���̍ĐU�֓����ꎞ�ۑ�����
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt���ʐU�֌��Q.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt���ʐU�֌��R.Text = "")
                            Int_Tokubetu_Flag = 3
                            Call PSUB_TOKUBETU_SET(txt���ʐ������R, txt���ʐU�֌��R, txt���ʐU�֓��R, chk�R_�P�w�N, chk�R_�Q�w�N, chk�R_�R�w�N, chk�R_�S�w�N, chk�R_�T�w�N, chk�R_�U�w�N, chk�R_�V�w�N, chk�R_�W�w�N, chk�R_�X�w�N, chk�R_�S�w�N, oraReader)

                            '�U�֓��ƍĐU�֓��̕\����̑Ή��֌W�i�Z�b�g�j���Ƃ邽�߁A�^�O�ɐU�֓����R�[�h���̍ĐU�֓����ꎞ�ۑ�����
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt���ʐU�֌��R.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt���ʐU�֌��S.Text = "")
                            Int_Tokubetu_Flag = 4
                            Call PSUB_TOKUBETU_SET(txt���ʐ������S, txt���ʐU�֌��S, txt���ʐU�֓��S, chk�S_�P�w�N, chk�S_�Q�w�N, chk�S_�R�w�N, chk�S_�S�w�N, chk�S_�T�w�N, chk�S_�U�w�N, chk�S_�V�w�N, chk�S_�W�w�N, chk�S_�X�w�N, chk�S_�S�w�N, oraReader)

                            '�U�֓��ƍĐU�֓��̕\����̑Ή��֌W�i�Z�b�g�j���Ƃ邽�߁A�^�O�ɐU�֓����R�[�h���̍ĐU�֓����ꎞ�ۑ�����
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt���ʐU�֌��S.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt���ʐU�֌��T.Text = "")
                            Int_Tokubetu_Flag = 5
                            Call PSUB_TOKUBETU_SET(txt���ʐ������T, txt���ʐU�֌��T, txt���ʐU�֓��T, chk�T_�P�w�N, chk�T_�Q�w�N, chk�T_�R�w�N, chk�T_�S�w�N, chk�T_�T�w�N, chk�T_�U�w�N, chk�T_�V�w�N, chk�T_�W�w�N, chk�T_�X�w�N, chk�T_�S�w�N, oraReader)

                            '�U�֓��ƍĐU�֓��̕\����̑Ή��֌W�i�Z�b�g�j���Ƃ邽�߁A�^�O�ɐU�֓����R�[�h���̍ĐU�֓����ꎞ�ۑ�����
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt���ʐU�֌��T.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt���ʐU�֌��U.Text = "")
                            Int_Tokubetu_Flag = 6
                            Call PSUB_TOKUBETU_SET(txt���ʐ������U, txt���ʐU�֌��U, txt���ʐU�֓��U, chk�U_�P�w�N, chk�U_�Q�w�N, chk�U_�R�w�N, chk�U_�S�w�N, chk�U_�T�w�N, chk�U_�U�w�N, chk�U_�V�w�N, chk�U_�W�w�N, chk�U_�X�w�N, chk�U_�S�w�N, oraReader)

                            '�U�֓��ƍĐU�֓��̕\����̑Ή��֌W�i�Z�b�g�j���Ƃ邽�߁A�^�O�ɐU�֓����R�[�h���̍ĐU�֓����ꎞ�ۑ�����
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt���ʐU�֌��U.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                    End Select

                Case "1"
                    Select Case oraReader.GetString("FURI_DATE_S")
                        Case txt���ʐU�֌��P.Tag
                            Int_Tokubetu_Flag = 1
                            Call PSUB_TOKUBETU_SET(txt���ʐ������P, txt���ʍĐU�֌��P, txt���ʍĐU�֓��P, chk�P_�P�w�N, chk�P_�Q�w�N, chk�P_�R�w�N, chk�P_�S�w�N, chk�P_�T�w�N, chk�P_�U�w�N, chk�P_�V�w�N, chk�P_�W�w�N, chk�P_�X�w�N, chk�P_�S�w�N, oraReader)

                            '2006/11/30�@�āX�U�֓����擾
                            str���ʍāX�U��(1) = oraReader.GetString("SFURI_DATE_S")

                            '2006/11/30�@�`�F�b�N�t���O���擾
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt���ʐU�֌��Q.Tag
                            Int_Tokubetu_Flag = 2
                            Call PSUB_TOKUBETU_SET(txt���ʐ������Q, txt���ʍĐU�֌��Q, txt���ʍĐU�֓��Q, chk�Q_�P�w�N, chk�Q_�Q�w�N, chk�Q_�R�w�N, chk�Q_�S�w�N, chk�Q_�T�w�N, chk�Q_�U�w�N, chk�Q_�V�w�N, chk�Q_�W�w�N, chk�Q_�X�w�N, chk�Q_�S�w�N, oraReader)

                            '2006/11/30�@�āX�U�֓����擾
                            str���ʍāX�U��(2) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt���ʐU�֌��R.Tag
                            Int_Tokubetu_Flag = 3
                            Call PSUB_TOKUBETU_SET(txt���ʐ������R, txt���ʍĐU�֌��R, txt���ʍĐU�֓��R, chk�R_�P�w�N, chk�R_�Q�w�N, chk�R_�R�w�N, chk�R_�S�w�N, chk�R_�T�w�N, chk�R_�U�w�N, chk�R_�V�w�N, chk�R_�W�w�N, chk�R_�X�w�N, chk�R_�S�w�N, oraReader)

                            '2006/11/30�@�āX�U�֓����擾
                            str���ʍāX�U��(3) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt���ʐU�֌��S.Tag
                            Int_Tokubetu_Flag = 4
                            Call PSUB_TOKUBETU_SET(txt���ʐ������S, txt���ʍĐU�֌��S, txt���ʍĐU�֓��S, chk�S_�P�w�N, chk�S_�Q�w�N, chk�S_�R�w�N, chk�S_�S�w�N, chk�S_�T�w�N, chk�S_�U�w�N, chk�S_�V�w�N, chk�S_�W�w�N, chk�S_�X�w�N, chk�S_�S�w�N, oraReader)

                            '2006/11/30�@�āX�U�֓����擾
                            str���ʍāX�U��(4) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt���ʐU�֌��T.Tag
                            Int_Tokubetu_Flag = 5
                            Call PSUB_TOKUBETU_SET(txt���ʐ������T, txt���ʍĐU�֌��T, txt���ʍĐU�֓��T, chk�T_�P�w�N, chk�T_�Q�w�N, chk�T_�R�w�N, chk�T_�S�w�N, chk�T_�T�w�N, chk�T_�U�w�N, chk�T_�V�w�N, chk�T_�W�w�N, chk�T_�X�w�N, chk�T_�S�w�N, oraReader)

                            '2006/11/30�@�āX�U�֓����擾
                            str���ʍāX�U��(5) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt���ʐU�֌��U.Tag
                            Int_Tokubetu_Flag = 6
                            Call PSUB_TOKUBETU_SET(txt���ʐ������U, txt���ʍĐU�֌��U, txt���ʍĐU�֓��U, chk�U_�P�w�N, chk�U_�Q�w�N, chk�U_�R�w�N, chk�U_�S�w�N, chk�U_�T�w�N, chk�U_�U�w�N, chk�U_�V�w�N, chk�U_�W�w�N, chk�U_�X�w�N, chk�U_�S�w�N, oraReader)

                            '2006/11/30�@�āX�U�֓����擾
                            str���ʍāX�U��(6) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                    End Select
            End Select

            oraReader.NextRead()

        Loop

        oraReader.Close()

        'Tag�̏���
        txt���ʐU�֌��P.Tag = ""
        txt���ʐU�֌��Q.Tag = ""
        txt���ʐU�֌��R.Tag = ""
        txt���ʐU�֌��S.Tag = ""
        txt���ʐU�֌��T.Tag = ""
        txt���ʐU�֌��U.Tag = ""

        PFUNC_TOKUBETU_SANSYOU = True

    End Function
    Private Function PFUNC_TOKUBETU_SAKUSEI(ByVal str���� As String) As Boolean
        '���ʐU�֓��@�쐬�����@�@�@
        PFUNC_TOKUBETU_SAKUSEI = False

        '���̓`�F�b�N
        Select Case True
            Case (Trim(txt���ʍĐU�֌��P.Text) <> "" And Trim(txt���ʍĐU�֓��P.Text) <> "" And Trim(txt���ʐ������P.Text) = "" And Trim(txt���ʐU�֌��P.Text) = "" And Trim(txt���ʐU�֓��P.Text) = "")
                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�U�֓��܂��͍ĐU�֓��̓��͂Ɍ�肪����܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt���ʍĐU�֌��Q.Text) <> "" And Trim(txt���ʍĐU�֓��Q.Text) <> "" And Trim(txt���ʐ������Q.Text) = "" And Trim(txt���ʐU�֌��Q.Text) = "" And Trim(txt���ʐU�֓��Q.Text) = "")
                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�U�֓��܂��͍ĐU�֓��̓��͂Ɍ�肪����܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt���ʍĐU�֌��R.Text) <> "" And Trim(txt���ʍĐU�֓��R.Text) <> "" And Trim(txt���ʐ������R.Text) = "" And Trim(txt���ʐU�֌��R.Text) = "" And Trim(txt���ʐU�֓��R.Text) = "")
                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�U�֓��܂��͍ĐU�֓��̓��͂Ɍ�肪����܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt���ʍĐU�֌��S.Text) <> "" And Trim(txt���ʍĐU�֓��S.Text) <> "" And Trim(txt���ʐ������S.Text) = "" And Trim(txt���ʐU�֌��S.Text) = "" And Trim(txt���ʐU�֓��S.Text) = "")
                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�U�֓��܂��͍ĐU�֓��̓��͂Ɍ�肪����܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt���ʍĐU�֌��T.Text) <> "" And Trim(txt���ʍĐU�֓��T.Text) <> "" And Trim(txt���ʐ������T.Text) = "" And Trim(txt���ʐU�֌��T.Text) = "" And Trim(txt���ʐU�֓��T.Text) = "")
                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�U�֓��܂��͍ĐU�֓��̓��͂Ɍ�肪����܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt���ʍĐU�֌��U.Text) <> "" And Trim(txt���ʍĐU�֓��U.Text) <> "" And Trim(txt���ʐ������U.Text) = "" And Trim(txt���ʐU�֌��U.Text) = "" And Trim(txt���ʐU�֓��U.Text) = "")
                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�U�֓��܂��͍ĐU�֓��̓��͂Ɍ�肪����܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
        End Select

        '2006/11/30�@���[�v��
        For i As Integer = 1 To 6

            '2006/11/30�@�ύX���������ꍇ�̂ݎ��s����
            If bln���ʍX�V(i) = True Then

                '2006/12/12�@���U�֓��擾
                If SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki = "" Then
                    '�󔒂̏ꍇ�͓��͂̕K�v�Ȃ�
                ElseIf CInt(SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki) < 4 Then
                    '�P�`�R��
                    str���U�֓�(i) = CInt(txt�Ώ۔N�x.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date
                Else
                    '�S�`�P�Q��
                    str���U�֓�(i) = txt�Ώ۔N�x.Text & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date
                End If

                '2006/12/12�@���ĐU���擾
                If Trim(SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date) = "" Then
                    '�ĐU���Ȃ�
                    str���ĐU��(i) = "00000000"
                ElseIf CInt(SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki) < 4 Then
                    '�P�`�R��
                    str���ĐU��(i) = CInt(txt�Ώ۔N�x.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                Else
                    '�S�`�P�Q��
                    str���ĐU��(i) = txt�Ώ۔N�x.Text & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                End If

                '�U�֓��`�F�b�N 
                If SYOKI_TOKUBETU_SCHINFO(i).SyoriFurikae_Flag = True Then

                    If TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then
                        If PFUNC_TOKUBETU_CHECK(i, TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check, TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, TOKUBETU_SCHINFO(i).SiyouGakunen9_Check) = False Then
                            MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���̃X�P�W���[���͏������̃X�P�W���[���ł��B" & vbCrLf & "�ύX�ł��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Function
                        End If

                        If SYOKI_TOKUBETU_SCHINFO(i).SyoriSaiFurikae_Flag = True Then
                            If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki Then
                                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���̃X�P�W���[���͏������̃X�P�W���[���ł��B" & vbCrLf & "�ύX�ł��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If

                            If TOKUBETU_SCHINFO(i).SaiFurikae_Date <> SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date Then
                                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���̃X�P�W���[���͏������̃X�P�W���[���ł��B" & vbCrLf & "�ύX�ł��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If
                        Else
                            If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then
                                If GAKKOU_INFO.SFURI_SYUBETU = "0" Or GAKKOU_INFO.SFURI_SYUBETU = "3" Then
                                    MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�ĐU�͍s��Ȃ��ݒ�ɂȂ��Ă��܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    Exit Function
                                End If

                                'CHKBOX�`�F�b�N&���ʕϐ��ɐݒ�
                                If PFUNC_GAKUNENFLG_CHECK(TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, TOKUBETU_SCHINFO(i).SiyouGakunen9_Check, TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check) = False Then
                                    Exit Function
                                End If

                                '�ĐU�̃X�P�W���[���̂ݍ쐬
                                If PFUNC_TOKUBETU_SAKUSEI_SUB2(TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, i) = False Then
                                    Exit Function
                                End If

                                If PFUNC_SCHMAST_UPDATE_SFURIDATE(CStr(i)) = False Then
                                    Exit Function
                                End If

                                '������ʂ�Ƃ������Ƃ͏����ɐ��������Ƃ������ƂȂ̂�
                                Int_Syori_Flag(1) = 1
                            End If
                        End If
                    Else
                        MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���̃X�P�W���[���͏������̃X�P�W���[���ł��B" & vbCrLf & "�폜�ł��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                Else
                    If TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then

                        '�\���̂��g�p���邽�߁A���ʕϐ��͕s�v
                        If PFUNC_GAKUNENFLG_CHECK(TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, TOKUBETU_SCHINFO(i).SiyouGakunen9_Check, TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check) = False Then
                            Exit Function
                        End If

                        If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then
                            If GAKKOU_INFO.SFURI_SYUBETU = "0" Or GAKKOU_INFO.SFURI_SYUBETU = "3" Then
                                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�ĐU�͍s��Ȃ��ݒ�ɂȂ��Ă��܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                            End If
                        End If

                        '�p�����^�͇@���A�A���͐U�֓��A�B�ĐU�֌��@�C�ĐU�֓��@�D�U�֋敪�i0:���U)�A�E�X�P�W���[���敪�i1:����)
                        If PFUNC_TOKUBETU_SAKUSEI_SUB(TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, i) = False Then
                            Exit Function
                        End If

                        '������ʂ�Ƃ������Ƃ͏����ɐ��������Ƃ������ƂȂ̂�
                        Int_Syori_Flag(1) = 1
                    End If
                End If

            Else '�X�V���Ȃ��ꍇ�ł���Ǝ��U���̃X�P�W���[��������
                '2011/06/16 �W���ŏC�� ���ʐU�֓����փ`�F�b�N�ǉ� ------------------START
                '2006/12/12�@���ĐU���擾
                If Trim(SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date) = "" Then
                    '�ĐU���Ȃ�
                    str���ĐU��(i) = "00000000"
                ElseIf CInt(SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki) < 4 Then
                    '�P�`�R��
                    str���ĐU��(i) = CInt(txt�Ώ۔N�x.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                Else
                    '�S�`�P�Q��
                    str���ĐU��(i) = txt�Ώ۔N�x.Text & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                End If
                '2011/06/16 �W���ŏC�� ���ʐU�֓����փ`�F�b�N�ǉ� ------------------END

                '��Ǝ��U�A�g���̂�
                '�U�֓��`�F�b�N 
                If SYOKI_TOKUBETU_SCHINFO(i).SyoriFurikae_Flag = True Then

                    If TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then
                        If PFUNC_TOKUBETU_CHECK(i, TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check, TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, TOKUBETU_SCHINFO(i).SiyouGakunen9_Check) = False Then
                            MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���̃X�P�W���[���͏������̃X�P�W���[���ł��B" & vbCrLf & "�ύX�ł��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function
                        End If

                        If SYOKI_TOKUBETU_SCHINFO(i).SyoriSaiFurikae_Flag = True Then
                            If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki Then
                                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���̃X�P�W���[���͏������̃X�P�W���[���ł��B" & vbCrLf & "�ύX�ł��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If

                            If TOKUBETU_SCHINFO(i).SaiFurikae_Date <> SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date Then
                                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���̃X�P�W���[���͏������̃X�P�W���[���ł��B" & vbCrLf & "�ύX�ł��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If
                        Else
                            If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then
                                If GAKKOU_INFO.SFURI_SYUBETU = "0" Or GAKKOU_INFO.SFURI_SYUBETU = "3" Then
                                    MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�ĐU�͍s��Ȃ��ݒ�ɂȂ��Ă��܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    Exit Function
                                End If

                                '�ĐU�̃X�P�W���[���̂ݍ쐬
                                If PFUNC_TOKUBETU_SAKUSEI_SUB2_KIGYO(TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date) = False Then
                                    Exit Function
                                End If

                                '������ʂ�Ƃ������Ƃ͏����ɐ��������Ƃ������ƂȂ̂�
                                Int_Syori_Flag(1) = 1
                            End If
                        End If
                    Else
                        MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���̃X�P�W���[���͏������̃X�P�W���[���ł��B" & vbCrLf & "�폜�ł��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                Else
                    If TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then

                        If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then
                            If GAKKOU_INFO.SFURI_SYUBETU = "0" Or GAKKOU_INFO.SFURI_SYUBETU = "3" Then
                                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�ĐU�͍s��Ȃ��ݒ�ɂȂ��Ă��܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                            End If
                        End If

                        '�p�����^�͇@���A�A���͐U�֓��A�B�ĐU�֌��@�C�ĐU�֓��@�D�U�֋敪�i0:���U)�A�E�X�P�W���[���敪�i1:����)
                        If PFUNC_TOKUBETU_SAKUSEI_SUB_KIGYO(TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, i) = False Then
                            Exit Function
                        End If

                        '������ʂ�Ƃ������Ƃ͏����ɐ��������Ƃ������ƂȂ̂�
                        Int_Syori_Flag(1) = 1
                    End If
                End If

            End If
        Next

        If PFUNC_TOKUBETU_GAKNENFLG_CHECK() = False Then
            Exit Function
        End If

        PFUNC_TOKUBETU_SAKUSEI = True

    End Function
    Private Function PFUNC_TOKUBETU_SAKUSEI_SUB(ByVal s������ As String, ByVal s�� As String, ByVal s�U�֓� As String, ByVal s�ĐU�֌� As String, ByVal s�ĐU�֓� As String, ByVal i As Integer) As Boolean

        Dim oraReader As MyOracleReader
        Dim sql As StringBuilder

        '�X�P�W���[���@���ʃ��R�[�h�쐬
        PFUNC_TOKUBETU_SAKUSEI_SUB = False

        '���U���R�[�h�̍쐬

        '�����N���̍쐬
        STR�����N�� = PFUNC_SEIKYUTUKIHI(s������)

        '�U�֓��Z�o
        STR�U�֓� = PFUNC_FURIHI_MAKE(s��, s�U�֓�, "1", "0")

        '2010/10/21 �_��U�֓����Z�o��
        STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s��, s�U�֓�, "1", "0")

        If s�ĐU�֌� <> "" And s�ĐU�֓� <> "" Then
            '�ĐU���̔N�̊m�菈��
            If s�ĐU�֌� = "01" Or s�ĐU�֌� = "02" Or s�ĐU�֌� = "03" Then
                STRW�ĐU�֔N = CStr(CInt(txt�Ώ۔N�x.Text) + 1)
            Else
                STRW�ĐU�֔N = txt�Ώ۔N�x.Text
            End If

            '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------START
            '�c�Ɠ��Z�o
            Select Sai_Zengo_Kbn
                Case 0
                    '���c�Ɠ�
                    STR�ĐU�֓� = PFUNC_EIGYOUBI_GET(STRW�ĐU�֔N & s�ĐU�֌� & s�ĐU�֓�, "0", "+")
                Case 1
                    '�O�c�Ɠ�
                    STR�ĐU�֓� = PFUNC_EIGYOUBI_GET(STRW�ĐU�֔N & s�ĐU�֌� & s�ĐU�֓�, "0", "-")
            End Select
            'STR�ĐU�֓� = PFUNC_EIGYOUBI_GET(STRW�ĐU�֔N & s�ĐU�֌� & s�ĐU�֓�, "0", "+")

            '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------END
        Else
            STR�ĐU�֓� = "00000000"
        End If

        '�X�P�W���[���敪�̋��ʕϐ��ݒ�
        STR�X�P�敪 = "1"

        '�U�֋敪�̋��ʕϐ��ݒ�
        STR�U�֋敪 = "0"

        '���͐U�֓��̋��ʕϐ��ݒ�
        STR�N�ԓ��͐U�֓� = Space(15)

        '�ʏ탌�R�[�h�̑Ώۊw�N�̃t���O�X�V�i���U���R�[�h�j
        '�w�Z�R�[�h�A�����N���A�U�֋敪�B�U�֓��i0:���U�j
        If PFUNC_SCH_NENKAN_GET(STR�����N��, "0", STR�U�֓�) = False Then
            MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�N�ԃX�P�W���[���̑Ώۊw�N�ݒ�ŃG���[���������܂���(���U)")
            Exit Function
        End If

        '�����X�P�W���[���Ɂu�ēx�X�V�v�p�̏��������E���z�A�U�֍ό����E���z�A�s�\�����E���z�̎擾
        If PFUNC_G_MEIMAST_COUNT_MOTO(STR�����N��, "0", STR�U�֓�) = False Then
            MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���׃}�X�^���擾���s")
            Exit Function
        End If

        Dim blnUP As Boolean = False

        '�������R�[�h�i���ʃX�P�W���[�������łɍ쐬����Ă��邩)�L���`�F�b�N
        '2006/11/22�@
        'If PFUNC_SCHMAST_GET("1", "0", STR�U�֓�, STR�ĐU�֓�) = True Then
        If PFUNC_SCHMAST_GET("1", "0", str���U�֓�(i), str���ĐU��(i)) = True Then
            '���݂��Ă���ꍇUPDATE�Ƃ��� 2006/10/25
            blnUP = True
        End If

        '�������R�[�h�i�N�ԁj�̏����t���O�L�� 2006/10/24
        If PFUNC_SCHMAST_GET_FLG("0", "0", STR�U�֓�) = False Then
            MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�ʏ�X�P�W���[�������󋵎擾���s", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        Else
            If strTYUUDAN_FLG = "1" Then
                MessageBox.Show("�ʏ�X�P�W���[��(���U��)�����f���ł�" & vbCrLf & "�U�֓��F" & STR�U�֓�.Substring(0, 4) & "�N" & STR�U�֓�.Substring(4, 2) & "��" & STR�U�֓�.Substring(6, 2) & "���̒��f��������Ă�������", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        End If

        If strSAIFURI_DEF <> "00000000" Then '�ʏ�X�P�W���[���̍ĐU�����ݒ肳��Ă���ꍇ
            '�������R�[�h�i�N�ԁj�̏����t���O�L�� 2006/10/24
            If PFUNC_SCHMAST_GET_FLG_SAI("0", "1", strSAIFURI_DEF) = False Then
                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�ʏ�X�P�W���[��(�ĐU)�����󋵎擾���s", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            Else
                If strTYUUDAN_FLG_SAI = "1" Then
                    MessageBox.Show("�ʏ�X�P�W���[��(�ĐU��)���������ł�" & vbCrLf & "�ĐU���F" & strSAIFURI_DEF.Substring(0, 4) & "�N" & strSAIFURI_DEF.Substring(4, 2) & "��" & strSAIFURI_DEF.Substring(6, 2) & "���̏�����������Ă�������", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
                '�ʏ�X�P�W���[���i�ĐU�j�̏����Ώۊw�N�t���O�X�V
                If PFUNC_SCH_NENKAN_GET(STR�����N��, "1", strSAIFURI_DEF) = False Then
                    MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "�N�ԃX�P�W���[���̑Ώۊw�N�ݒ�ŃG���[���������܂���(�ĐU)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If

            End If
        End If

        If PFUNC_G_MEIMAST_COUNT("0", STR�U�֓�) = False Then
            MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���׃}�X�^���擾���s", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        '----------------------------------------------
        '�X�V�E�o�^����
        '----------------------------------------------
        Dim strSQL As String = ""
        If blnUP = True Then
            '2006/11/30�@�X�P�W���[���̏����󋵃`�F�b�N
            If PFUNC_TOKUBETUFLG_CHECK("�X�V", "", i) = False Then
                Exit Function
            End If
            '���ɃX�P�W���[��(���U)�����݂��Ă���ꍇUPDATE
            strSQL = PSUB_UPDATE_G_SCHMAST_SQL(str���U�֓�(i), str���ĐU��(i))
        Else
            '2006/11/30�@�X�P�W���[���̏����󋵃`�F�b�N
            If PFUNC_TOKUBETUFLG_CHECK("�쐬", "", i) = False Then
                Exit Function
            End If
            '2006/11/30�@�N�ԃX�P�W���[���X�V
            If PFUNC_TokINSERT_NenUPDATE(STR�����N��, Replace(SYOKI_NENKAN_SCHINFO(CInt(s������)).Furikae_Day, "/", "")) = False Then
                Exit Function
            End If
            '�X�P�W���[���}�X�^�o�^(���U)SQL���쐬
            strSQL = PSUB_INSERT_G_SCHMAST_SQL()
        End If
        blnUP = False

        If MainDB.ExecuteNonQuery(strSQL) < 0 Then
            '�쐬�����G���[
            Exit Function
        End If

        '2006/11/30�@�N�ԃX�P�W���[���̊w�N�t���O�̍X�V
        If PFUNC_NENKAN_GAKUNENFLG_UPDATE(STR�����N��, STR�U�֋敪) = False Then
            Exit Function
        End If

        '-----------------------------------------------
        '2006/07/26�@��Ǝ��U�̏��U�̃X�P�W���[�����쐬
        '-----------------------------------------------
        oraReader = New MyOracleReader(MainDB)
        sql = New StringBuilder(128)
        '���ɓo�^����Ă��邩�`�F�b�N
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '01' AND ")
        sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")

        '�Ǎ��̂�
        If oraReader.DataReader(sql) = True Then    '�X�P�W���[�������ɑ��݂���
        Else     '�X�P�W���[�������݂��Ȃ�
            '�X�P�W���[���쐬
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, "01", gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                     gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '�����Ƀq�b�g���Ȃ�������

                '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                'If fn_INSERTSCHMAST(strGakkouCode, "01", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                If fn_INSERTSCHMAST(strGakkouCode, "01", STR�U�֓�, gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", Err.Description)
                    MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oraReader.Close()
                    Return False
                End If
            End If
        End If
        oraReader.Close()
        '-----------------------------------------------

        '�ĐU���R�[�h�̍쐬
        If STR�ĐU�֓� <> "00000000" Then

            '���U�ŋ��߂��ĐU����U�֓��ɐݒ�
            STR�U�֓� = STR�ĐU�֓�
            str���U�֓�(i) = str���ĐU��(i)

            '2010/10/21 �ĐU�̌_��U�֓����Z�o����
            STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s�ĐU�֌�, s�ĐU�֓�, "1", "1")

            '�U�֋敪�͍ĐU�Ƃ���

            '�ĐU���̎Z�o
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"
                    '1(�ĐU�L/�J�z��)
                    STR�ĐU�֓� = "00000000"
                    '2006/11/22�@�������R�[�h�`�F�b�N�p
                    str���ĐU��(i) = "00000000"
                Case "2"
                    '2(�ĐU�L/�J�z�L)   ���񏉐U����ݒ�
                    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(Trim(s�ĐU�֌�), Trim(s�ĐU�֓�))
                    '2006/11/22�@�������R�[�h�`�F�b�N�p
                    str���ĐU��(i) = PFUNC_SAISAIFURIHI_MAKE(str���ĐU��(i).Substring(4, 2), str���ĐU��(i).Substring(6, 2))
            End Select

            '�ʏ탌�R�[�h�̑Ώۊw�N�̐ݒ肵�����i�ĐU���R�[�h�j
            '�w�Z�R�[�h�A�����N���A�U�֋敪�i1:�ĐU�j
            If PFUNC_SCH_NENKAN_GET(STR�����N��, "1", STR�ĐU�֓�) = False Then
                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���ʃX�P�W���[���Ώۊw�N�ݒ�ŃG���[���������܂���(�ĐU)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            blnUP = False

            '�������R�[�h�L���`�F�b�N
            '2006/11/22
            'If PFUNC_SCHMAST_GET("1", "1", STR�U�֓�, STR�ĐU�֓�) = True Then
            If PFUNC_SCHMAST_GET("1", "1", str���U�֓�(i), str���ĐU��(i)) = True Then
                '���݂��Ă���ꍇUPDATE�Ƃ��� 2006/10/25
                blnUP = True
            End If

            '�������R�[�h�i�N�ԁj�̏����t���O�L�� 2006/10/24
            If PFUNC_SCHMAST_GET_FLG("0", "1", STR�U�֓�) = False Then
                '�ʏ�U�֓��������ꍇ(�����ʐU�֓��őS�w�N����U���Ă��鎞�Ȃǂ͖���
            End If

            If PFUNC_G_MEIMAST_COUNT("1", STR�U�֓�) = False Then
                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���׃}�X�^���擾���s", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            '�X�P�W���[���敪�̋��ʕϐ��ݒ�
            STR�X�P�敪 = "1"
            '�U�֋敪�̋��ʕϐ��ݒ�
            STR�U�֋敪 = "1"
            '���͐U�֓��̋��ʕϐ��ݒ�
            STR�N�ԓ��͐U�֓� = Space(15)

            strSQL = ""
            If blnUP = True Then
                '���ɃX�P�W���[��(���U)�����݂��Ă���ꍇUPDATE
                strSQL = PSUB_UPDATE_G_SCHMAST_SQL(str���U�֓�(i), str���ĐU��(i))
            Else
                '�X�P�W���[���}�X�^�o�^(�ĐU)SQL���쐬
                strSQL = PSUB_INSERT_G_SCHMAST_SQL()
            End If

            '2006/11/30�@�N�ԃX�P�W���[���̊w�N�t���O�̍X�V
            If PFUNC_NENKAN_GAKUNENFLG_UPDATE(STR�����N��, STR�U�֋敪) = False Then
                Exit Function
            End If

            If MainDB.ExecuteNonQuery(strSQL) < 0 Then
                '�쐬�����G���[
                Exit Function
            End If
            '-----------------------------------------------
            '2006/07/26�@��Ǝ��U�̍ĐU�̃X�P�W���[�����쐬
            '-----------------------------------------------
            oraReader = New MyOracleReader(MainDB)
            sql = New StringBuilder(128)
            '���ɓo�^����Ă��邩�`�F�b�N
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")

            '�Ǎ��̂�
            If oraReader.DataReader(sql) = True Then    '�X�P�W���[�������ɑ��݂���
            Else     '�X�P�W���[�������݂��Ȃ�
                '�X�P�W���[���쐬
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '�����Ƀq�b�g���Ȃ�������

                    '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", Err.Description)
                        MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If


            End If
            oraReader.Close()
        End If
        '-----------------------------------------------

        PFUNC_TOKUBETU_SAKUSEI_SUB = True

    End Function
    Private Function PFUNC_TOKUBETU_SAKUSEI_SUB2(ByVal s������ As String, ByVal s�� As String, ByVal s�U�֓� As String, ByVal s�ĐU�֌� As String, ByVal s�ĐU�֓� As String, ByVal i As Integer) As Boolean

        Dim kousin As Boolean = False   '�C���T�[�g���[�h

        PFUNC_TOKUBETU_SAKUSEI_SUB2 = False

        '�X�P�W���[���@���ʃ��R�[�h�쐬
        '���U���������ɍĐU�̃X�P�W���[����ǉ�����ۂɎg�p

        '�����N���̍쐬
        STR�����N�� = PFUNC_SEIKYUTUKIHI(s������)

        '�U�֓��Z�o
        STR�U�֓� = PFUNC_FURIHI_MAKE(s��, s�U�֓�, "1", "0")

        '2010/10/21 �_��U�֓����Z�o����
        STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s��, s�U�֓�, "1", "0")

        '�ĐU���̔N�̊m�菈��
        If s�ĐU�֌� = "01" Or s�ĐU�֌� = "02" Or s�ĐU�֌� = "03" Then
            STRW�ĐU�֔N = CStr(CInt(txt�Ώ۔N�x.Text) + 1)
        Else
            STRW�ĐU�֔N = txt�Ώ۔N�x.Text
        End If

        '�c�Ɠ��Z�o
        '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------START
        '�c�Ɠ��Z�o
        Select Case Sai_Zengo_Kbn
            Case 0
                '���c�Ɠ�
                STR�ĐU�֓� = PFUNC_EIGYOUBI_GET(STRW�ĐU�֔N & s�ĐU�֌� & s�ĐU�֓�, "0", "+")
            Case 1
                '�O�c�Ɠ�
                STR�ĐU�֓� = PFUNC_EIGYOUBI_GET(STRW�ĐU�֔N & s�ĐU�֌� & s�ĐU�֓�, "0", "-")
        End Select
        'STR�ĐU�֓� = PFUNC_EIGYOUBI_GET(STRW�ĐU�֔N & s�ĐU�֌� & s�ĐU�֓�, "0", "+")
        '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------END


        '�c�Ɠ����Z�o�������ʂŐU�֓��ƍĐU�֓�������ɂȂ�ꍇ�������
        If STR�U�֓� = STR�ĐU�֓� Then
            MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & Mid(STR�U�֓�, 5, 2) & "����" & "�U�֓��ƍĐU�֓�������ł�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Str_FURI_DATE = STR�U�֓�
        Str_SFURI_DATE = STR�ĐU�֓�

        '�ĐU���R�[�h�̍쐬
        If STR�ĐU�֓� <> "00000000" Then

            '���U�ŋ��߂��ĐU����U�֓��ɐݒ�
            STR�U�֓� = STR�ĐU�֓�

            '2010/10/21 �ĐU�̌_��U�֓����Z�o����
            STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s�ĐU�֌�, s�ĐU�֓�, "1", "1")

            '�U�֋敪�͍ĐU�Ƃ���

            '�ĐU���̎Z�o
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"
                    '1(�ĐU�L/�J�z��)
                    STR�ĐU�֓� = "00000000"
                Case "2"
                    '2(�ĐU�L/�J�z�L)   ���񏉐U����ݒ�
                    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(Trim(s�ĐU�֌�), Trim(s�ĐU�֓�))
            End Select

            '�ʏ탌�R�[�h�̑Ώۊw�N�̐ݒ肵�����i�ĐU���R�[�h�j
            '�w�Z�R�[�h�A�����N���A�U�֋敪�i1:�ĐU�j
            If PFUNC_SCH_NENKAN_GET(STR�����N��, "1", STR�ĐU�֓�) = False Then
                MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���ʃX�P�W���[���Ώۊw�N�ݒ�ŃG���[���������܂���(�ĐU)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            '�������R�[�h�L���`�F�b�N
            If PFUNC_SCHMAST_GET("1", "1", STR�����N�� & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date, "00000000") = True Then
                'If PFUNC_SCHMAST_GET("1", "1", STR�U�֓�, STR�ĐU�֓�) = True Then
                kousin = True   '�A�b�v�f�[�g���[�h
                'MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���ʃX�P�W���[���쐬�ςł�(�ĐU)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'Exit Function
            End If

            '�X�P�W���[���敪�̋��ʕϐ��ݒ�
            STR�X�P�敪 = "1"
            '���͐U�֓��̋��ʕϐ��ݒ�
            STR�N�ԓ��͐U�֓� = Space(15)

            '�X�P�W���[���}�X�^�X�V(���U)SQL���쐬�@2006/11/30
            Dim strSQL As String = ""
            '�����킩��Ȃ��̂ŃR�����g 2010.03.29 start
            'STR�U�֋敪 = "0" '���U�̔��f�̂��߁A�ꎞ�I��0�ɐݒ�
            'strSQL = PSUB_UPDATE_G_SCHMAST_SQL(STR�����N�� & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date, STR�����N�� & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date)

            'If MainDB.ExecuteNonQuery(strSQL) < 0 Then
            '    Return False
            'End If
            '�����킩��Ȃ��̂ŃR�����g 2010.03.29 end

            '�U�֋敪�̋��ʕϐ��ݒ�
            STR�U�֋敪 = "1"

            '�X�P�W���[���}�X�^�o�^(�ĐU)SQL���쐬
            strSQL = ""
            If kousin = True Then
                '�A�b�v�f�[�g
                strSQL = PSUB_UPDATE_G_SCHMAST_SQL(STR�����N�� & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date, "00000000")
            Else
                '�C���T�[�g
                strSQL = PSUB_INSERT_G_SCHMAST_SQL()
            End If

            If MainDB.ExecuteNonQuery(strSQL) < 0 Then
                Return False
            End If

            '2006/11/30�@�N�ԃX�P�W���[���̊w�N�t���O�̍X�V
            If PFUNC_NENKAN_GAKUNENFLG_UPDATE(STR�����N��, STR�U�֋敪) = False Then
                Return False
            End If

            '-----------------------------------------------
            '2006/07/26�@��Ǝ��U�̍ĐU�̃X�P�W���[�����쐬
            '-----------------------------------------------
            Dim oraReader As New MyOracleReader(MainDB)
            Dim sql As New StringBuilder(128)

            '���ɓo�^����Ă��邩�`�F�b�N
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")

            '�Ǎ��̂�
            If oraReader.DataReader(sql) = True Then    '�X�P�W���[�������ɑ��݂���
            Else     '�X�P�W���[�������݂��Ȃ�
                '�R�����g 2006/12/11
                'If intPUSH_BTN = 2 Then '�X�V��
                '    MessageBox.Show("��Ǝ��U���̃X�P�W���[��(" & STR�����N��.Substring(0, 4) & "�N" & STR�����N��.Substring(4, 2) & "����)�����݂��܂���" & vbCrLf & "��Ǝ��U���Ō��ԃX�P�W���[���쐬��A" & vbCrLf & "�w�Z�X�P�W���[���̍X�V�������ēx�s���Ă�������", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'Else
                '�X�P�W���[���쐬
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '�����Ƀq�b�g���Ȃ�������

                    '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", Err.Description)
                        MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()
        End If

        Return True

    End Function
    '��Ƃ̃X�P�W���[���X�V�p 2006/12/08
    Private Function PFUNC_TOKUBETU_SAKUSEI_SUB_KIGYO(ByVal s������ As String, ByVal s�� As String, ByVal s�U�֓� As String, ByVal s�ĐU�֌� As String, ByVal s�ĐU�֓� As String, ByVal i As Integer) As Boolean

        Dim oraReader As MyOracleReader
        Dim sql As StringBuilder

        '�X�P�W���[���@���ʃ��R�[�h�쐬
        PFUNC_TOKUBETU_SAKUSEI_SUB_KIGYO = False

        '���U���R�[�h�̍쐬

        '�����N���̍쐬
        STR�����N�� = PFUNC_SEIKYUTUKIHI(s������)

        '�U�֓��Z�o
        STR�U�֓� = PFUNC_FURIHI_MAKE(s��, s�U�֓�, "1", "0")

        '2010/10/21 �_��U�֓����Z�o����
        STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s��, s�U�֓�, "1", "0")

        If s�ĐU�֌� <> "" And s�ĐU�֓� <> "" Then
            '�ĐU���̔N�̊m�菈��
            If s�ĐU�֌� = "01" Or s�ĐU�֌� = "02" Or s�ĐU�֌� = "03" Then
                STRW�ĐU�֔N = CStr(CInt(txt�Ώ۔N�x.Text) + 1)
            Else
                STRW�ĐU�֔N = txt�Ώ۔N�x.Text
            End If
            '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------START
            '�c�Ɠ��Z�o
            Select Case Sai_Zengo_Kbn
                Case 0
                    '���c�Ɠ�
                    STR�ĐU�֓� = PFUNC_EIGYOUBI_GET(STRW�ĐU�֔N & s�ĐU�֌� & s�ĐU�֓�, "0", "+")
                Case 1
                    '�O�c�Ɠ�
                    STR�ĐU�֓� = PFUNC_EIGYOUBI_GET(STRW�ĐU�֔N & s�ĐU�֌� & s�ĐU�֓�, "0", "-")
            End Select
            'STR�ĐU�֓� = PFUNC_EIGYOUBI_GET(STRW�ĐU�֔N & s�ĐU�֌� & s�ĐU�֓�, "0", "+")
            '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------END
        Else
            STR�ĐU�֓� = "00000000"
        End If

        '�X�P�W���[���敪�̋��ʕϐ��ݒ�
        STR�X�P�敪 = "1"

        '�U�֋敪�̋��ʕϐ��ݒ�
        STR�U�֋敪 = "0"

        '���͐U�֓��̋��ʕϐ��ݒ�
        STR�N�ԓ��͐U�֓� = Space(15)

        oraReader = New MyOracleReader(MainDB)
        sql = New StringBuilder(128)
        '���ɓo�^����Ă��邩�`�F�b�N
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '01' AND ")
        sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")

        If oraReader.DataReader(sql) = True Then    '�X�P�W���[�������ɑ��݂���
        Else     '�X�P�W���[�������݂��Ȃ�
            '�X�P�W���[���쐬
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, "01", gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                        gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '�����Ƀq�b�g���Ȃ�������

                '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                'If fn_INSERTSCHMAST(strGakkouCode, "01", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                If fn_INSERTSCHMAST(strGakkouCode, "01", STR�U�֓�, gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", Err.Description)
                    MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oraReader.Close()
                    Return False
                End If
            End If
            'End If

        End If
        oraReader.Close()

        '-----------------------------------------------

        '�ĐU���R�[�h�̍쐬
        If STR�ĐU�֓� <> "00000000" Then

            '���U�ŋ��߂��ĐU����U�֓��ɐݒ�
            STR�U�֓� = STR�ĐU�֓�
            str���U�֓�(i) = str���ĐU��(i)

            '2010/10/21 �ĐU�̌_��U�֓����Z�o����
            STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s�ĐU�֌�, s�ĐU�֓�, "1", "1")

            '�U�֋敪�͍ĐU�Ƃ���

            '�ĐU���̎Z�o
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"
                    '1(�ĐU�L/�J�z��)
                    STR�ĐU�֓� = "00000000"
                    '2006/11/22�@�������R�[�h�`�F�b�N�p
                    str���ĐU��(i) = "00000000"
                Case "2"
                    '2(�ĐU�L/�J�z�L)   ���񏉐U����ݒ�
                    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(Trim(s�ĐU�֌�), Trim(s�ĐU�֓�))
                    '2006/11/22�@�������R�[�h�`�F�b�N�p
                    str���ĐU��(i) = PFUNC_SAISAIFURIHI_MAKE(str���ĐU��(i).Substring(4, 2), str���ĐU��(i).Substring(6, 2))
            End Select

            '�X�P�W���[���敪�̋��ʕϐ��ݒ�
            STR�X�P�敪 = "1"
            '�U�֋敪�̋��ʕϐ��ݒ�
            STR�U�֋敪 = "1"
            '���͐U�֓��̋��ʕϐ��ݒ�
            STR�N�ԓ��͐U�֓� = Space(15)

            oraReader = New MyOracleReader(MainDB)
            sql = New StringBuilder(128)
            '���ɓo�^����Ă��邩�`�F�b�N
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")

            If oraReader.DataReader(sql) = True Then    '�X�P�W���[�������ɑ��݂���
            Else     '�X�P�W���[�������݂��Ȃ�
                '�X�P�W���[���쐬
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '�����Ƀq�b�g���Ȃ�������

                    '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", Err.Description)
                        MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()
        End If
        '-----------------------------------------------

        Return True

    End Function
    Private Function PFUNC_TOKUBETU_SAKUSEI_SUB2_KIGYO(ByVal s������ As String, ByVal s�� As String, ByVal s�U�֓� As String, ByVal s�ĐU�֌� As String, ByVal s�ĐU�֓� As String) As Boolean

        Dim oraReader As MyOracleReader
        Dim sql As StringBuilder

        PFUNC_TOKUBETU_SAKUSEI_SUB2_KIGYO = False

        '�X�P�W���[���@���ʃ��R�[�h�쐬
        '���U���������ɍĐU�̃X�P�W���[����ǉ�����ۂɎg�p

        '�����N���̍쐬
        STR�����N�� = PFUNC_SEIKYUTUKIHI(s������)

        '�U�֓��Z�o
        STR�U�֓� = PFUNC_FURIHI_MAKE(s��, s�U�֓�, "1", "0")

        '2010/10/21 �_��U�֓����Z�o����
        STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s��, s�U�֓�, "1", "0")

        '�ĐU���̔N�̊m�菈��
        If s�ĐU�֌� = "01" Or s�ĐU�֌� = "02" Or s�ĐU�֌� = "03" Then
            STRW�ĐU�֔N = CStr(CInt(txt�Ώ۔N�x.Text) + 1)
        Else
            STRW�ĐU�֔N = txt�Ώ۔N�x.Text
        End If

        '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------START
        '�c�Ɠ��Z�o
        Select Case Sai_Zengo_Kbn
            Case 0
                '���c�Ɠ�
                STR�ĐU�֓� = PFUNC_EIGYOUBI_GET(STRW�ĐU�֔N & s�ĐU�֌� & s�ĐU�֓�, "0", "+")
            Case 1
                '�O�c�Ɠ�
                STR�ĐU�֓� = PFUNC_EIGYOUBI_GET(STRW�ĐU�֔N & s�ĐU�֌� & s�ĐU�֓�, "0", "-")
        End Select
        'STR�ĐU�֓� = PFUNC_EIGYOUBI_GET(STRW�ĐU�֔N & s�ĐU�֌� & s�ĐU�֓�, "0", "+")
        '2011/06/16 �W���ŏC�� �ĐU�x���V�t�g�̗��c�Ɠ��l�� ------------------END

        '�c�Ɠ����Z�o�������ʂŐU�֓��ƍĐU�֓�������ɂȂ�ꍇ�������
        If STR�U�֓� = STR�ĐU�֓� Then
            MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & Mid(STR�U�֓�, 5, 2) & "����" & "�U�֓��ƍĐU�֓�������ł�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Str_FURI_DATE = STR�U�֓�
        Str_SFURI_DATE = STR�ĐU�֓�

        '�ĐU���R�[�h�̍쐬
        If STR�ĐU�֓� <> "00000000" Then

            '���U�ŋ��߂��ĐU����U�֓��ɐݒ�
            STR�U�֓� = STR�ĐU�֓�

            '2010/10/21 �ĐU�̌_��U�֓����Z�o����
            STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s�ĐU�֌�, s�ĐU�֓�, "1", "1")

            '�U�֋敪�͍ĐU�Ƃ���

            '�ĐU���̎Z�o
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"
                    '1(�ĐU�L/�J�z��)
                    STR�ĐU�֓� = "00000000"
                Case "2"
                    '2(�ĐU�L/�J�z�L)   ���񏉐U����ݒ�
                    STR�ĐU�֓� = PFUNC_SAISAIFURIHI_MAKE(Trim(s�ĐU�֌�), Trim(s�ĐU�֓�))
            End Select

            '�X�P�W���[���敪�̋��ʕϐ��ݒ�
            STR�X�P�敪 = "1"
            '���͐U�֓��̋��ʕϐ��ݒ�
            STR�N�ԓ��͐U�֓� = Space(15)

            '�U�֋敪�̋��ʕϐ��ݒ�
            STR�U�֋敪 = "1"

            oraReader = New MyOracleReader(MainDB)
            sql = New StringBuilder(128)
            '���ɓo�^����Ă��邩�`�F�b�N
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")

            If oraReader.DataReader(sql) = True Then    '�X�P�W���[�������ɑ��݂���
            Else     '�X�P�W���[�������݂��Ȃ�
                '�X�P�W���[���쐬
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                            gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '�����Ƀq�b�g���Ȃ�������

                    '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR�U�֓�, gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", Err.Description)
                        MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()
        End If

        Return True

    End Function

    '=========================================================
    '���ʃX�P�W���[���o�^���̔N�ԃX�P�W���[���X�V�@2006/11/30
    '=========================================================
    Private Function PFUNC_TokINSERT_NenUPDATE(ByVal strNENGETUDO As String, ByVal strFURI_DATE As String) As Boolean

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        Dim j As Integer '               ���[�v�p�ϐ�
        Dim strGakunen_FLG(9) As String '�w�N�t���O�i�[�z��
        Dim bFlg As Boolean = False '    ���[�v�������ʉߔ���

        '���ʃX�P�W���[���̊w�N�t���O��z��Ɋi�[
        strGakunen_FLG(1) = STR�P�w�N
        strGakunen_FLG(2) = STR�Q�w�N
        strGakunen_FLG(3) = STR�R�w�N
        strGakunen_FLG(4) = STR�S�w�N
        strGakunen_FLG(5) = STR�T�w�N
        strGakunen_FLG(6) = STR�U�w�N
        strGakunen_FLG(7) = STR�V�w�N
        strGakunen_FLG(8) = STR�W�w�N
        strGakunen_FLG(9) = STR�X�w�N

        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)
        '------------------------------------------------
        '���׃}�X�^�����i�����E���z�̎擾�j
        '------------------------------------------------
        sql.Append(" SELECT * FROM G_MEIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_M ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_M = '0'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_M ='" & strFURI_DATE & "'")

        sql.Append(" AND (")

        '�t���O�̗����Ă���w�N�������ɒǉ�
        For j = 1 To 9
            If strGakunen_FLG(j) = 1 Then
                If bFlg = True Then
                    sql.Append(" or")
                End If

                sql.Append(" GAKUNEN_CODE_M = " & j)
                bFlg = True
            End If
        Next j

        sql.Append(" )")

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0

        If oraReader.DataReader(sql) = True Then

            '------------------------------------------------
            '�����E���z�擾
            '------------------------------------------------

            Do Until oraReader.EOF

                lngSYORI_KEN = lngSYORI_KEN + 1
                dblSYORI_KIN = dblSYORI_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                If oraReader.GetString("FURIKETU_CODE_M") = "0" Then
                    lngFURI_KEN = lngFURI_KEN + 1
                    dblFURI_KIN = dblFURI_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                Else
                    lngFUNOU_KEN = lngFUNOU_KEN + 1
                    dblFUNOU_KIN = dblFUNOU_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                End If
                oraReader.NextRead()
            Loop

        End If
        oraReader.Close()

        '------------------------------------------------
        '�N�ԃX�P�W���[���X�V
        '------------------------------------------------
        bFlg = False

        sql = New StringBuilder(128)

        sql.Append("UPDATE  G_SCHMAST SET ")

        '���̃f�[�^�ɍ��Z���̌����E���z�𑫂�
        sql.Append(" SYORI_KEN_S = SYORI_KEN_S - " & CDbl(lngSYORI_KEN) & ",")
        sql.Append(" SYORI_KIN_S = SYORI_KIN_S - " & dblSYORI_KIN & ",")
        sql.Append(" FURI_KEN_S = FURI_KEN_S - " & CDbl(lngFURI_KEN) & ",")
        sql.Append(" FURI_KIN_S =  FURI_KIN_S - " & dblFURI_KIN & ",")
        sql.Append(" FUNOU_KEN_S = FUNOU_KEN_S - " & CDbl(lngFUNOU_KEN) & ",")
        sql.Append(" FUNOU_KIN_S = FUNOU_KIN_S - " & dblFUNOU_KIN)
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='0'")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            '�X�V�����G���[
            MessageBox.Show("�X�P�W���[���}�X�^�̍X�V�����ŃG���[���������܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '-----------------------------------------------------
        '�����t���O�擾�i���ʃX�P�W���[����INSERT�����Ɏg�p�j
        '-----------------------------------------------------
        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='0'")

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                strENTRI_FLG = oraReader.GetString("ENTRI_FLG_S")
                strCHECK_FLG = oraReader.GetString("CHECK_FLG_S")
                strDATA_FLG = oraReader.GetString("DATA_FLG_S")
                strFUNOU_FLG = oraReader.GetString("FUNOU_FLG_S")
                strSAIFURI_FLG = oraReader.GetString("SAIFURI_FLG_S")
                strKESSAI_FLG = oraReader.GetString("KESSAI_FLG_S")

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()

        Return True

    End Function

    Private Function PFUNC_TOKUBETU_KOUSIN() As Boolean

        '�폜�����iDELETE�j
        If PFUNC_TOKUBETU_DELETE() = False Then
            Return False
        End If

        '�쐬�����iINSERT/UPDATE)
        If PFUNC_TOKUBETU_SAKUSEI("�X�V") = False Then
            Return False
        End If

        '�s�v�N�ԃX�P�W���[���폜����
        If PFUNC_DELETE_GSCHMAST() = False Then
            Return False
        End If

        Return True

    End Function

    '====================================================
    '�N�ԃX�P�W���[���̊w�N�t���O�X�V�@2006/11/30
    '====================================================
    Private Function PFUNC_NENKAN_GAKUNENFLG_UPDATE(ByVal strNENGETUDO As String, ByVal strFURIKUBUN As String) As Boolean

        PFUNC_NENKAN_GAKUNENFLG_UPDATE = False

        Dim strGakunen_FLG(9) As String '�w�N�t���O�i�[�p�z��
        Dim sql As New StringBuilder(128) '             SQL���i�[�ϐ�

        '���ʃX�P�W���[���̊w�N�t���O��z��Ɋi�[
        strGakunen_FLG(1) = STR�P�w�N
        strGakunen_FLG(2) = STR�Q�w�N
        strGakunen_FLG(3) = STR�R�w�N
        strGakunen_FLG(4) = STR�S�w�N
        strGakunen_FLG(5) = STR�T�w�N
        strGakunen_FLG(6) = STR�U�w�N
        strGakunen_FLG(7) = STR�V�w�N
        strGakunen_FLG(8) = STR�W�w�N
        strGakunen_FLG(9) = STR�X�w�N

        '�N�ԃX�P�W���[���̊w�N�t���O�̍X�V
        sql.Append("UPDATE  G_SCHMAST SET ")

        For j As Integer = 1 To 9
            If strGakunen_FLG(j) = "1" Then
                sql.Append(" GAKUNEN" & j & "_FLG_S ='0'") '���ʂŃt���O�������Ă���w�N�͔N�Ԃł͍~�낷
            Else
                sql.Append(" GAKUNEN" & j & "_FLG_S ='1'") '���ʂŃt���O���~��Ă���w�N�͔N�Ԃł͗��Ă�
            End If
            If j <> 9 Then
                sql.Append(",")
            End If
        Next

        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")

        If strFURIKUBUN <> "*" Then '*�F���U�E�ĐU�����X�V
            sql.Append(" AND")
            sql.Append(" FURI_KBN_S ='" & strFURIKUBUN & "'")
        Else
            sql.Append(" AND")
            sql.Append(" (FURI_KBN_S ='0'")
            sql.Append(" or")
            sql.Append(" FURI_KBN_S ='1')")
        End If

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show("�X�P�W���[���}�X�^�̍X�V�����ŃG���[���������܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True

    End Function


    '===============================================
    '���ʃX�P�W���[�������t���O�`�F�b�N�@2006/11/30
    '===============================================
    Private Function PFUNC_TOKUBETUFLG_CHECK(ByVal strSyori As String, ByVal strSeikyuNenGetu As String, ByVal i As Integer) As Boolean

        PFUNC_TOKUBETUFLG_CHECK = False

        '�����ɂ���ă`�F�b�N���e��ύX
        Select Case strSyori

            Case "�X�V" '���ʃX�P�W���[����������
                If SYOKI_TOKUBETU_SCHINFO(i).SyoriFurikae_Flag = True Then

                    MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & _
                                                  "�������̂��߁A�ύX�o���܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function

                End If

            Case "�쐬" '�N�ԃX�P�W���[���� �@�`�F�b�N�t���O�������Ă��āA�s�\�t���O���~��Ă���
                '                           �A�ĐU�X�P�W���[����������
                If SYOKI_NENKAN_SCHINFO(CInt(TOKUBETU_SCHINFO(i).Seikyu_Tuki)).CheckFurikae_Flag <> SYOKI_NENKAN_SCHINFO(CInt(TOKUBETU_SCHINFO(i).Seikyu_Tuki)).FunouFurikae_Flag Or _
                   SYOKI_NENKAN_SCHINFO(CInt(TOKUBETU_SCHINFO(i).Seikyu_Tuki)).CheckSaiFurikae_Flag = True Then

                    MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & _
                                                  "�N�ԃX�P�W���[�����������̂��߁A�쐬�o���܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function

                End If

            Case "�폜" '�N�ԁE���ʃX�P�W���[�����������ŁA�Ⴄ�U�֓�
                If (SYOKI_TOKUBETU_SCHINFO(i).CheckFurikae_Flag = True Or SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).CheckFurikae_Flag = True) And _
                    Replace(SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).Furikae_Day, "/", "") <> strSeikyuNenGetu & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date Then

                    MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & _
                                                  "�N�ԃX�P�W���[�����������̂��߁A�폜�ł��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                ElseIf (SYOKI_TOKUBETU_SCHINFO(i).CheckFurikae_Flag = True Or SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).CheckFurikae_Flag = True) And _
                (SYOKI_TOKUBETU_SCHINFO(i).FunouFurikae_Flag = False Or SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).FunouFurikae_Flag = False) And _
                    Replace(SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).Furikae_Day, "/", "") <> strSeikyuNenGetu & TOKUBETU_SCHINFO(i).Furikae_Date Then
                    '�폜�����ǉ�(�C��) 2007/01/09
                    MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & _
                                                      "�N�ԃX�P�W���[�����������̂��߁A�폜�ł��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
        End Select

        PFUNC_TOKUBETUFLG_CHECK = True

    End Function

    '====================================================
    '���ʃX�P�W���[���폜�����@2006/11/30
    '====================================================
    Private Function PFUNC_TOKUBETU_DELETE() As Boolean
        PFUNC_TOKUBETU_DELETE = False

        Dim sql As New StringBuilder(128)

        Dim blnSakujo_Check As Boolean = False
        Dim strNengetu As String '   �����N��
        Dim strSFuri_Date As String '�ĐU��

        '�S�폜�����A�L�[�͊w�Z�R�[�h�A�Ώ۔N�x�A�X�P�W���[���敪�i�P�F���ʁj�A�����t���O�i�O�j
        sql.Append(" DELETE  FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S =1")
        sql.Append(" AND")
        sql.Append(" ((CHECK_FLG_S =0 AND DATA_FLG_S =0 AND FUNOU_FLG_S =0 ) OR (CHECK_FLG_S =1 AND DATA_FLG_S =1 AND FUNOU_FLG_S =1 ))")
        sql.Append(" AND")
        sql.Append(" TYUUDAN_FLG_S =0")

        '2006/11/30�@�����ǉ��i�ύX�̂������f�[�^�̂ݍ폜�j=========================
        For i As Integer = 1 To 6

            '------------------------------------------------------------
            '�ύX������A�������E���U���E���U�������󔒂̂��̂��폜����
            '------------------------------------------------------------
            If bln���ʍX�V(i) = True And TOKUBETU_SCHINFO(i).Seikyu_Tuki = "" And TOKUBETU_SCHINFO(i).Furikae_Date = "" And _
               TOKUBETU_SCHINFO(i).Furikae_Date = "" And SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And _
               SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then

                '�N���x���擾
                If CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki) < 4 Then
                    '�P�`�R��
                    strNengetu = CInt(txt�Ώ۔N�x.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki
                Else
                    '�S�`�P�Q��
                    strNengetu = txt�Ώ۔N�x.Text & SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki
                End If

                '�X�P�W���[���̏����󋵃`�F�b�N
                If PFUNC_TOKUBETUFLG_CHECK("�폜", strNengetu, i) = False Then
                    Exit Function
                End If

                '�ĐU���擾
                If SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date = "" Then
                    '�ĐU�����󔒂̏ꍇ�A0���߂���
                    strSFuri_Date = "00000000"
                Else
                    strSFuri_Date = strNengetu & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                End If

                '�ڑ����ǉ�
                If blnSakujo_Check = True Then
                    sql.Append(" or") '  �񕶖ڈȍ~
                Else
                    sql.Append(" and(") '�ꕶ��
                End If

                '�U�֓��E�ĐU���E�U�֋敪�̐ݒ�
                sql.Append(" (FURI_DATE_S = '" & strNengetu & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date & "'")
                sql.Append(" AND")
                sql.Append(" SFURI_DATE_S = '" & strSFuri_Date & "'")
                sql.Append(" AND")
                sql.Append(" FURI_KBN_S = '0')") 'FURI_KBN_S = 0�F���U��

                '�ĐU�̃X�P�W���[�����폜����
                sql.Append(" or")
                sql.Append(" (FURI_DATE_S = '" & strNengetu & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date & "'")
                sql.Append(" AND")
                sql.Append(" SFURI_DATE_S = '" & str���ʍāX�U��(i) & "'")
                sql.Append(" AND")
                sql.Append(" FURI_KBN_S = '1')") 'FURI_KBN_S = 1�F�ĐU��

                '----------------------------------------------
                '�N�ԃX�P�W���[���w�N�t���O�X�V
                '----------------------------------------------
                '�g�p�w�N�t���O�擾
                If PFUNC_GAKUNENFLG_CHECK(SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen9_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check) = False Then
                    MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���׃}�X�^���擾���s", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
                '�N�ԃX�P�W���[���X�V����
                If PFUNC_TokDELETE_NenUPDATE(strNengetu, strNengetu & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date, strSFuri_Date) = False Then
                    Exit Function
                End If

                bln���ʍX�V(i) = False '�ύX�t���O���~�낷
                blnSakujo_Check = True '�폜�t���O�𗧂Ă�

                '------------------------------------------------------------
                '�ĐU�X�P�W���[���݂̂̍폜
                '------------------------------------------------------------
            ElseIf bln���ʍX�V(i) = True And TOKUBETU_SCHINFO(i).SaiFurikae_Tuki = "" And _
                TOKUBETU_SCHINFO(i).SaiFurikae_Date = "" And SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And _
                SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then

                '�N���x���擾
                If CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki) < 4 Then
                    '�P�`�R��
                    strNengetu = CInt(txt�Ώ۔N�x.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki
                Else
                    '�S�`�P�Q��
                    strNengetu = txt�Ώ۔N�x.Text & SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki
                End If

                If blnSakujo_Check = True Then
                    sql.Append(" or") '  �񕶖ڈȍ~
                Else
                    sql.Append(" and(") '�ꕶ��
                End If

                '�U�֓��E�ĐU���E�U�֋敪�̐ݒ�
                sql.Append(" (FURI_DATE_S = '" & strNengetu & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date & "'")
                sql.Append(" AND")
                sql.Append(" SFURI_DATE_S = '" & str���ʍāX�U��(i) & "'")
                sql.Append(" AND")
                sql.Append(" FURI_KBN_S = '1')") 'FURI_KBN_S = 1�F�ĐU��

                '�ĐU�̂ݍ폜�����ꍇ�A���U���ύX���K�v�Ȃ̂ŕύX�t���O�͍~�낳�Ȃ�
                blnSakujo_Check = True '�폜�t���O�𗧂Ă�

            End If
        Next

        If blnSakujo_Check = True Then
            sql.Append(")")
            '�폜�f�[�^������ꍇ�̂ݎ��s����
            If MainDB.ExecuteNonQuery(sql) < 0 Then
                MessageBox.Show("�X�P�W���[���̍폜�����ŃG���[���������܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        End If

        Return True

    End Function

    '==========================================================
    '���ʃX�P�W���[���폜���̔N�ԃX�P�W���[���X�V�@2006/11/30
    '==========================================================
    Private Function PFUNC_TokDELETE_NenUPDATE(ByVal strNENGETUDO As String, ByVal strFURI_DATE As String, ByVal strSFURI_DATE As String) As Boolean

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        Dim strGakunen_FLG(9) As String '�w�N�t���O�i�[�z��
        Dim bFlg As Boolean = False '    ���[�v�������ʉߔ���

        '���ʃX�P�W���[���̊w�N�t���O��z��Ɋi�[
        strGakunen_FLG(1) = STR�P�w�N
        strGakunen_FLG(2) = STR�Q�w�N
        strGakunen_FLG(3) = STR�R�w�N
        strGakunen_FLG(4) = STR�S�w�N
        strGakunen_FLG(5) = STR�T�w�N
        strGakunen_FLG(6) = STR�U�w�N
        strGakunen_FLG(7) = STR�V�w�N
        strGakunen_FLG(8) = STR�W�w�N
        strGakunen_FLG(9) = STR�X�w�N

        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)

        '---------------------------------------------------
        '�폜����X�P�W���[���}�X�^�����i�����E���z�̎擾�j
        '---------------------------------------------------
        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S = '0'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & strFURI_DATE & "'")
        sql.Append(" AND")
        sql.Append(" SFURI_DATE_S ='" & strSFURI_DATE & "'")

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0

        If oraReader.DataReader(sql) = True Then
            '------------------------------------------------
            '�����E���z�擾
            '------------------------------------------------
            Do Until oraReader.EOF

                '���������E���z�擾
                lngSYORI_KEN = CDbl(oraReader.GetInt64("SYORI_KEN_S"))
                dblSYORI_KIN = CDbl(oraReader.GetInt64("SYORI_KIN_S"))
                '�U�֌����E���z�擾
                lngFURI_KEN = CDbl(oraReader.GetInt64("FURI_KEN_S"))
                dblFURI_KIN = CDbl(oraReader.GetInt64("FURI_KIN_S"))
                '�s�\�����E���z�擾
                lngFUNOU_KEN = CDbl(oraReader.GetInt64("FUNOU_KEN_S"))
                dblFUNOU_KIN = CDbl(oraReader.GetInt64("FUNOU_KIN_S"))

                oraReader.NextRead()
            Loop

        End If
        oraReader.Close()

        '------------------------------------------------
        '�N�ԃX�P�W���[�������E���z�X�V�i���U���̂݁j
        '------------------------------------------------
        sql = New StringBuilder(128)

        sql.Append("UPDATE  G_SCHMAST SET ")

        '���̃f�[�^�ɍ��Z���̌����E���z�𑫂�
        sql.Append(" SYORI_KEN_S = SYORI_KEN_S + " & lngSYORI_KEN & ",")
        sql.Append(" SYORI_KIN_S = SYORI_KIN_S + " & dblSYORI_KIN & ",")
        sql.Append(" FURI_KEN_S = FURI_KEN_S + " & lngFURI_KEN & ",")
        sql.Append(" FURI_KIN_S =  FURI_KIN_S + " & dblFURI_KIN & ",")
        sql.Append(" FUNOU_KEN_S = FUNOU_KEN_S + " & lngFUNOU_KEN & ",")
        sql.Append(" FUNOU_KIN_S = FUNOU_KIN_S + " & dblFUNOU_KIN)

        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='0'")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show("�X�P�W���[���}�X�^�̍X�V�����ŃG���[���������܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '-------------------------------------------------
        '�N�ԃX�P�W���[���w�N�t���O�ύX�i���U�E�ĐU�����j
        '-------------------------------------------------
        bFlg = False

        sql = New StringBuilder(128)

        sql.Append("UPDATE  G_SCHMAST SET ")

        '���Z�f�[�^���̊w�N�t���O�𗧂Ă�
        For j As Integer = 1 To 9
            If strGakunen_FLG(j) = "1" Then
                If bFlg = True Then
                    sql.Append(",")
                End If
                sql.Append(" GAKUNEN" & j & "_FLG_S = '1'")
                bFlg = True
            End If
        Next

        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" (FURI_KBN_S ='0'")
        sql.Append(" or")
        sql.Append(" FURI_KBN_S ='1')")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show("�X�P�W���[���}�X�^�̍X�V�����ŃG���[���������܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True

    End Function

    Private Function PFUNC_TOKUBETU_CHECK(ByVal pIndex As Integer, _
                                          ByVal pSeikyu_Tuki As String, _
                                          ByVal pFuri_Tuki As String, _
                                          ByVal pFuri_Hi As String, _
                                          ByVal pSaiFuri_Tuki As String, _
                                          ByVal pSaiFuri_Hi As String, _
                                          ByVal pSiyouFlag0 As Boolean, ByVal pSiyouFlag1 As Boolean, ByVal pSiyouFlag2 As Boolean, ByVal pSiyouFlag3 As Boolean, ByVal pSiyouFlag4 As Boolean, ByVal pSiyouFlag5 As Boolean, ByVal pSiyouFlag6 As Boolean, ByVal pSiyouFlag7 As Boolean, ByVal pSiyouFlag8 As Boolean, ByVal pSiyouFlag9 As Boolean) As Boolean

        PFUNC_TOKUBETU_CHECK = False

        '�Q�Ǝ��Ɏ擾�������e�ƍX�V���Ɏ擾�������e�ɕύX�����邩�ǂ����̔�����s��

        If pSeikyu_Tuki <> SYOKI_TOKUBETU_SCHINFO(pIndex).Seikyu_Tuki Then
            Exit Function
        End If

        If pFuri_Tuki <> SYOKI_TOKUBETU_SCHINFO(pIndex).Furikae_Tuki Then
            Exit Function
        End If

        If pFuri_Hi <> SYOKI_TOKUBETU_SCHINFO(pIndex).Furikae_Date Then
            Exit Function
        End If

        Select Case pSiyouFlag0
            Case True
                If SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen1_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen2_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen3_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen4_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen5_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen6_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen7_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen8_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen9_Check = True Then
                Else
                    Exit Function
                End If
            Case False
                If pSiyouFlag1 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen1_Check Then
                    Exit Function
                End If
                If pSiyouFlag2 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen2_Check Then
                    Exit Function
                End If
                If pSiyouFlag3 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen3_Check Then
                    Exit Function
                End If
                If pSiyouFlag4 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen4_Check Then
                    Exit Function
                End If
                If pSiyouFlag5 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen5_Check Then
                    Exit Function
                End If
                If pSiyouFlag6 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen6_Check Then
                    Exit Function
                End If
                If pSiyouFlag7 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen7_Check Then
                    Exit Function
                End If
                If pSiyouFlag8 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen8_Check Then
                    Exit Function
                End If
                If pSiyouFlag9 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen9_Check Then
                    Exit Function
                End If
        End Select

        PFUNC_TOKUBETU_CHECK = True

    End Function

    '==================================================================
    '�����������ɓ����w�N�t���O�����������Ă��Ȃ����`�F�b�N 2006/11/30
    '==================================================================
    Private Function PFUNC_GAKNENFLG_CHECK() As Boolean

        PFUNC_GAKNENFLG_CHECK = False

        Dim strSeikyu_Tuki(6) As String '������
        Dim strGakunen_FLG(6, 10) As Boolean '�w�N�t���O�i���ʃX�P�W���[���ԍ�,�w�N�j

        strSeikyu_Tuki(1) = txt���ʐ������P.Text
        strSeikyu_Tuki(2) = txt���ʐ������Q.Text
        strSeikyu_Tuki(3) = txt���ʐ������R.Text
        strSeikyu_Tuki(4) = txt���ʐ������S.Text
        strSeikyu_Tuki(5) = txt���ʐ������T.Text
        strSeikyu_Tuki(6) = txt���ʐ������U.Text

        '�S�w�N�t���O���擾
        PSUB_GAKUNENFLG_GET(strGakunen_FLG)

        '�������������w�N�̃t���O�������Ă��Ȃ����`�F�b�N
        For i As Integer = 1 To 5
            For j As Integer = i + 1 To 6
                '���������`�F�b�N�i�󗓂łȂ��A�������������j
                If strSeikyu_Tuki(i) <> "" And strSeikyu_Tuki(i) = strSeikyu_Tuki(j) Then
                    For k As Integer = 1 To 9
                        If strGakunen_FLG(i, k) = True And strGakunen_FLG(j, k) = True Then
                            '���w�N�t���O�`�F�b�N�i����True�j
                            MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���������ɓ��w�N�̏���������܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Function
                        ElseIf strGakunen_FLG(i, 10) = True Or strGakunen_FLG(j, 10) = True Then
                            '�S�w�N�t���O�`�F�b�N�i�ǂ��炩��True�j
                            MessageBox.Show("(���ʃX�P�W���[��)" & vbCrLf & "���������ɑS�w�N�̏���������܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Function
                        End If
                    Next
                End If
            Next
        Next

        PFUNC_GAKNENFLG_CHECK = True

    End Function

    '2007/01/04
    Private Function PFUNC_TOKUBETU_GAKNENFLG_CHECK() As Boolean
        '==================================================================
        '���ʃX�P�W���[���Ɏw�肳��Ă���w�N�ɂ��ĔN�ԃX�P�W���[����
        '�w�N�t���O��0�ɍX�V���� 2007/01/04
        '==================================================================

        PFUNC_TOKUBETU_GAKNENFLG_CHECK = False
        Dim strSeikyu_Tuki(6) As String '������
        Dim strGakunen_FLG(6, 10) As Boolean '�w�N�t���O�i���ʃX�P�W���[���ԍ�,�w�N�j

        Dim strSEIKYU_NENGETU As String = ""

        Dim sql As New StringBuilder(128)

        strSeikyu_Tuki(1) = txt���ʐ������P.Text
        strSeikyu_Tuki(2) = txt���ʐ������Q.Text
        strSeikyu_Tuki(3) = txt���ʐ������R.Text
        strSeikyu_Tuki(4) = txt���ʐ������S.Text
        strSeikyu_Tuki(5) = txt���ʐ������T.Text
        strSeikyu_Tuki(6) = txt���ʐ������U.Text

        '�S�w�N�t���O���擾
        PSUB_GAKUNENFLG_GET(strGakunen_FLG)

        For i As Integer = 1 To 6
            If strSeikyu_Tuki(i).Trim = "" Then
                GoTo Next_SEIKYUTUKI
            End If

            '�����N���̍쐬
            strSEIKYU_NENGETU = PFUNC_SEIKYUTUKIHI(strSeikyu_Tuki(i))

            For j As Integer = 1 To 10
                If strGakunen_FLG(i, j) = True Then

                    sql.Length = 0
                    sql.Append("UPDATE  G_SCHMAST SET ")
                    If j = 10 Then
                        sql.Append(" GAKUNEN1_FLG_S ='0', ")
                        sql.Append(" GAKUNEN2_FLG_S ='0', ")
                        sql.Append(" GAKUNEN3_FLG_S ='0', ")
                        sql.Append(" GAKUNEN4_FLG_S ='0', ")
                        sql.Append(" GAKUNEN5_FLG_S ='0', ")
                        sql.Append(" GAKUNEN6_FLG_S ='0', ")
                        sql.Append(" GAKUNEN7_FLG_S ='0', ")
                        sql.Append(" GAKUNEN8_FLG_S ='0', ")
                        sql.Append(" GAKUNEN9_FLG_S ='0' ")
                    Else
                        sql.Append(" GAKUNEN" & j & "_FLG_S ='0' ")
                    End If
                    sql.Append(" WHERE GAKKOU_CODE_S = '" & txtGAKKOU_CODE.Text.Trim & "' ")
                    sql.Append(" AND SCH_KBN_S = '0'")
                    sql.Append(" AND NENGETUDO_S = '" & strSEIKYU_NENGETU & "' ")

                    If MainDB.ExecuteNonQuery(sql) < 0 Then
                        MessageBox.Show("�X�P�W���[���}�X�^�̍X�V�����ŃG���[���������܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If

                End If

            Next
Next_SEIKYUTUKI:
        Next

        Return True

    End Function


#End Region

#Region " Private Sub(�����X�P�W���[��)"
    Private Sub PSUB_ZUIJI_GET(ByRef Get_Data() As ZuijiData)

        '�����X�P�W���[���^�u��ʂŌ��ݕ\������Ă��鍀�ڂ̓��e���\���̂Ɏ擾
        Get_Data(1).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�P)
        Get_Data(1).Furikae_Tuki = txt�����U�֌��P.Text
        Get_Data(1).Furikae_Date = txt�����U�֓��P.Text

        Select Case chk�����P_�S�w�N.Checked
            Case True
                Get_Data(1).SiyouGakunen1_Check = True
                Get_Data(1).SiyouGakunen2_Check = True
                Get_Data(1).SiyouGakunen3_Check = True
                Get_Data(1).SiyouGakunen4_Check = True
                Get_Data(1).SiyouGakunen5_Check = True
                Get_Data(1).SiyouGakunen6_Check = True
                Get_Data(1).SiyouGakunen7_Check = True
                Get_Data(1).SiyouGakunen8_Check = True
                Get_Data(1).SiyouGakunen9_Check = True
            Case False
                Get_Data(1).SiyouGakunen1_Check = chk�����P_�P�w�N.Checked
                Get_Data(1).SiyouGakunen2_Check = chk�����P_�Q�w�N.Checked
                Get_Data(1).SiyouGakunen3_Check = chk�����P_�R�w�N.Checked
                Get_Data(1).SiyouGakunen4_Check = chk�����P_�S�w�N.Checked
                Get_Data(1).SiyouGakunen5_Check = chk�����P_�T�w�N.Checked
                Get_Data(1).SiyouGakunen6_Check = chk�����P_�U�w�N.Checked
                Get_Data(1).SiyouGakunen7_Check = chk�����P_�V�w�N.Checked
                Get_Data(1).SiyouGakunen8_Check = chk�����P_�W�w�N.Checked
                Get_Data(1).SiyouGakunen9_Check = chk�����P_�X�w�N.Checked
        End Select

        Get_Data(2).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�Q)
        Get_Data(2).Furikae_Tuki = txt�����U�֌��Q.Text
        Get_Data(2).Furikae_Date = txt�����U�֓��Q.Text

        Select Case chk�����Q_�S�w�N.Checked
            Case True
                Get_Data(2).SiyouGakunen1_Check = True
                Get_Data(2).SiyouGakunen2_Check = True
                Get_Data(2).SiyouGakunen3_Check = True
                Get_Data(2).SiyouGakunen4_Check = True
                Get_Data(2).SiyouGakunen5_Check = True
                Get_Data(2).SiyouGakunen6_Check = True
                Get_Data(2).SiyouGakunen7_Check = True
                Get_Data(2).SiyouGakunen8_Check = True
                Get_Data(2).SiyouGakunen9_Check = True
            Case False
                Get_Data(2).SiyouGakunen1_Check = chk�����Q_�P�w�N.Checked
                Get_Data(2).SiyouGakunen2_Check = chk�����Q_�Q�w�N.Checked
                Get_Data(2).SiyouGakunen3_Check = chk�����Q_�R�w�N.Checked
                Get_Data(2).SiyouGakunen4_Check = chk�����Q_�S�w�N.Checked
                Get_Data(2).SiyouGakunen5_Check = chk�����Q_�T�w�N.Checked
                Get_Data(2).SiyouGakunen6_Check = chk�����Q_�U�w�N.Checked
                Get_Data(2).SiyouGakunen7_Check = chk�����Q_�V�w�N.Checked
                Get_Data(2).SiyouGakunen8_Check = chk�����Q_�W�w�N.Checked
                Get_Data(2).SiyouGakunen9_Check = chk�����Q_�X�w�N.Checked
        End Select

        Get_Data(3).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�R)
        Get_Data(3).Furikae_Tuki = txt�����U�֌��R.Text
        Get_Data(3).Furikae_Date = txt�����U�֓��R.Text

        Select Case chk�����R_�S�w�N.Checked
            Case True
                Get_Data(3).SiyouGakunen1_Check = True
                Get_Data(3).SiyouGakunen2_Check = True
                Get_Data(3).SiyouGakunen3_Check = True
                Get_Data(3).SiyouGakunen4_Check = True
                Get_Data(3).SiyouGakunen5_Check = True
                Get_Data(3).SiyouGakunen6_Check = True
                Get_Data(3).SiyouGakunen7_Check = True
                Get_Data(3).SiyouGakunen8_Check = True
                Get_Data(3).SiyouGakunen9_Check = True
            Case False
                Get_Data(3).SiyouGakunen1_Check = chk�����R_�P�w�N.Checked
                Get_Data(3).SiyouGakunen2_Check = chk�����R_�Q�w�N.Checked
                Get_Data(3).SiyouGakunen3_Check = chk�����R_�R�w�N.Checked
                Get_Data(3).SiyouGakunen4_Check = chk�����R_�S�w�N.Checked
                Get_Data(3).SiyouGakunen5_Check = chk�����R_�T�w�N.Checked
                Get_Data(3).SiyouGakunen6_Check = chk�����R_�U�w�N.Checked
                Get_Data(3).SiyouGakunen7_Check = chk�����R_�V�w�N.Checked
                Get_Data(3).SiyouGakunen8_Check = chk�����R_�W�w�N.Checked
                Get_Data(3).SiyouGakunen9_Check = chk�����R_�X�w�N.Checked
        End Select

        Get_Data(4).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�S)
        Get_Data(4).Furikae_Tuki = txt�����U�֌��S.Text
        Get_Data(4).Furikae_Date = txt�����U�֓��S.Text

        Select Case chk�����S_�S�w�N.Checked
            Case True
                Get_Data(4).SiyouGakunen1_Check = True
                Get_Data(4).SiyouGakunen2_Check = True
                Get_Data(4).SiyouGakunen3_Check = True
                Get_Data(4).SiyouGakunen4_Check = True
                Get_Data(4).SiyouGakunen5_Check = True
                Get_Data(4).SiyouGakunen6_Check = True
                Get_Data(4).SiyouGakunen7_Check = True
                Get_Data(4).SiyouGakunen8_Check = True
                Get_Data(4).SiyouGakunen9_Check = True
            Case False
                Get_Data(4).SiyouGakunen1_Check = chk�����S_�P�w�N.Checked
                Get_Data(4).SiyouGakunen2_Check = chk�����S_�Q�w�N.Checked
                Get_Data(4).SiyouGakunen3_Check = chk�����S_�R�w�N.Checked
                Get_Data(4).SiyouGakunen4_Check = chk�����S_�S�w�N.Checked
                Get_Data(4).SiyouGakunen5_Check = chk�����S_�T�w�N.Checked
                Get_Data(4).SiyouGakunen6_Check = chk�����S_�U�w�N.Checked
                Get_Data(4).SiyouGakunen7_Check = chk�����S_�V�w�N.Checked
                Get_Data(4).SiyouGakunen8_Check = chk�����S_�W�w�N.Checked
                Get_Data(4).SiyouGakunen9_Check = chk�����S_�X�w�N.Checked
        End Select

        Get_Data(5).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�T)
        Get_Data(5).Furikae_Tuki = txt�����U�֌��T.Text
        Get_Data(5).Furikae_Date = txt�����U�֓��T.Text

        Select Case chk�����T_�S�w�N.Checked
            Case True
                Get_Data(5).SiyouGakunen1_Check = True
                Get_Data(5).SiyouGakunen2_Check = True
                Get_Data(5).SiyouGakunen3_Check = True
                Get_Data(5).SiyouGakunen4_Check = True
                Get_Data(5).SiyouGakunen5_Check = True
                Get_Data(5).SiyouGakunen6_Check = True
                Get_Data(5).SiyouGakunen7_Check = True
                Get_Data(5).SiyouGakunen8_Check = True
                Get_Data(5).SiyouGakunen9_Check = True
            Case False
                Get_Data(5).SiyouGakunen1_Check = chk�����T_�P�w�N.Checked
                Get_Data(5).SiyouGakunen2_Check = chk�����T_�Q�w�N.Checked
                Get_Data(5).SiyouGakunen3_Check = chk�����T_�R�w�N.Checked
                Get_Data(5).SiyouGakunen4_Check = chk�����T_�S�w�N.Checked
                Get_Data(5).SiyouGakunen5_Check = chk�����T_�T�w�N.Checked
                Get_Data(5).SiyouGakunen6_Check = chk�����T_�U�w�N.Checked
                Get_Data(5).SiyouGakunen7_Check = chk�����T_�V�w�N.Checked
                Get_Data(5).SiyouGakunen8_Check = chk�����T_�W�w�N.Checked
                Get_Data(5).SiyouGakunen9_Check = chk�����T_�X�w�N.Checked
        End Select

        Get_Data(6).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb���o�敪�U)
        Get_Data(6).Furikae_Tuki = txt�����U�֌��U.Text
        Get_Data(6).Furikae_Date = txt�����U�֓��U.Text

        Select Case chk�����U_�S�w�N.Checked
            Case True
                Get_Data(6).SiyouGakunen1_Check = True
                Get_Data(6).SiyouGakunen2_Check = True
                Get_Data(6).SiyouGakunen3_Check = True
                Get_Data(6).SiyouGakunen4_Check = True
                Get_Data(6).SiyouGakunen5_Check = True
                Get_Data(6).SiyouGakunen6_Check = True
                Get_Data(6).SiyouGakunen7_Check = True
                Get_Data(6).SiyouGakunen8_Check = True
                Get_Data(6).SiyouGakunen9_Check = True
            Case False
                Get_Data(6).SiyouGakunen1_Check = chk�����U_�P�w�N.Checked
                Get_Data(6).SiyouGakunen2_Check = chk�����U_�Q�w�N.Checked
                Get_Data(6).SiyouGakunen3_Check = chk�����U_�R�w�N.Checked
                Get_Data(6).SiyouGakunen4_Check = chk�����U_�S�w�N.Checked
                Get_Data(6).SiyouGakunen5_Check = chk�����U_�T�w�N.Checked
                Get_Data(6).SiyouGakunen6_Check = chk�����U_�U�w�N.Checked
                Get_Data(6).SiyouGakunen7_Check = chk�����U_�V�w�N.Checked
                Get_Data(6).SiyouGakunen8_Check = chk�����U_�W�w�N.Checked
                Get_Data(6).SiyouGakunen9_Check = chk�����U_�X�w�N.Checked
        End Select

    End Sub
    Private Sub PSUB_ZUIJI_CLEAR()

        '�擾�����\���̂̏�����

        For i As Integer = 1 To 6
            SYOKI_ZUIJI_SCHINFO(i).Furikae_Date = ""
            SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki = ""
            SYOKI_ZUIJI_SCHINFO(i).Nyusyutu_Kbn = 0
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen1_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen2_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen3_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen4_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen5_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen6_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen7_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen8_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen9_Check = False
            SYOKI_ZUIJI_SCHINFO(i).Syori_Flag = False
        Next i

    End Sub
#End Region

#Region " Private Sub(�����X�P�W���[����ʐ���)"
    Private Sub PSUB_ZUIJI_FORMAT(Optional ByVal pIndex As Integer = 1)

        'Select case pIndex
        '    Case 0
        '�Ώۊw�N�`�F�b�N�a�n�w�̗L����
        Call PSUB_ZUIJI_CHKBOXEnabled(True)
        'End Select

        '�����Ώۊw�N�w��`�F�b�NOFF
        Call PSUB_ZUIJI_CHK(False)

        '�U�֓����͗��A�ĐU�֓����͗��̃N���A
        Call PSUB_ZUIJI_DAYCLER()

        '���o�敪�R���{�{�b�N�X����
        Call PSUB_ZUIJI_CMB()

        '�Q�Ǝ��擾�l�ێ��\���̏�����
        Call PSUB_ZUIJI_CLEAR()

    End Sub
    Private Sub PSUB_ZUIJI_CHKBOXEnabled(ByVal pValue As Boolean)

        '�Ώۊw�N�`�F�b�NBOX�̗L����
        chk�����P_�P�w�N.Enabled = pValue
        chk�����P_�Q�w�N.Enabled = pValue
        chk�����P_�R�w�N.Enabled = pValue
        chk�����P_�S�w�N.Enabled = pValue
        chk�����P_�T�w�N.Enabled = pValue
        chk�����P_�U�w�N.Enabled = pValue
        chk�����P_�V�w�N.Enabled = pValue
        chk�����P_�W�w�N.Enabled = pValue
        chk�����P_�X�w�N.Enabled = pValue
        chk�����P_�S�w�N.Enabled = pValue

        chk�����Q_�P�w�N.Enabled = pValue
        chk�����Q_�Q�w�N.Enabled = pValue
        chk�����Q_�R�w�N.Enabled = pValue
        chk�����Q_�S�w�N.Enabled = pValue
        chk�����Q_�T�w�N.Enabled = pValue
        chk�����Q_�U�w�N.Enabled = pValue
        chk�����Q_�V�w�N.Enabled = pValue
        chk�����Q_�W�w�N.Enabled = pValue
        chk�����Q_�X�w�N.Enabled = pValue
        chk�����Q_�S�w�N.Enabled = pValue

        chk�����R_�P�w�N.Enabled = pValue
        chk�����R_�Q�w�N.Enabled = pValue
        chk�����R_�R�w�N.Enabled = pValue
        chk�����R_�S�w�N.Enabled = pValue
        chk�����R_�T�w�N.Enabled = pValue
        chk�����R_�U�w�N.Enabled = pValue
        chk�����R_�V�w�N.Enabled = pValue
        chk�����R_�W�w�N.Enabled = pValue
        chk�����R_�X�w�N.Enabled = pValue
        chk�����R_�S�w�N.Enabled = pValue

        chk�����S_�P�w�N.Enabled = pValue
        chk�����S_�Q�w�N.Enabled = pValue
        chk�����S_�R�w�N.Enabled = pValue
        chk�����S_�S�w�N.Enabled = pValue
        chk�����S_�T�w�N.Enabled = pValue
        chk�����S_�U�w�N.Enabled = pValue
        chk�����S_�V�w�N.Enabled = pValue
        chk�����S_�W�w�N.Enabled = pValue
        chk�����S_�X�w�N.Enabled = pValue
        chk�����S_�S�w�N.Enabled = pValue

        chk�����T_�P�w�N.Enabled = pValue
        chk�����T_�Q�w�N.Enabled = pValue
        chk�����T_�R�w�N.Enabled = pValue
        chk�����T_�S�w�N.Enabled = pValue
        chk�����T_�T�w�N.Enabled = pValue
        chk�����T_�U�w�N.Enabled = pValue
        chk�����T_�V�w�N.Enabled = pValue
        chk�����T_�W�w�N.Enabled = pValue
        chk�����T_�X�w�N.Enabled = pValue
        chk�����T_�S�w�N.Enabled = pValue

        chk�����U_�P�w�N.Enabled = pValue
        chk�����U_�Q�w�N.Enabled = pValue
        chk�����U_�R�w�N.Enabled = pValue
        chk�����U_�S�w�N.Enabled = pValue
        chk�����U_�T�w�N.Enabled = pValue
        chk�����U_�U�w�N.Enabled = pValue
        chk�����U_�V�w�N.Enabled = pValue
        chk�����U_�W�w�N.Enabled = pValue
        chk�����U_�X�w�N.Enabled = pValue
        chk�����U_�S�w�N.Enabled = pValue

    End Sub
    Private Sub PSUB_ZUIJI_DAYCLER()

        '�U�֓��̃N���A����
        txt�����U�֌��P.Text = ""
        txt�����U�֓��P.Text = ""
        txt�����U�֌��Q.Text = ""
        txt�����U�֓��Q.Text = ""
        txt�����U�֌��R.Text = ""
        txt�����U�֓��R.Text = ""
        txt�����U�֌��S.Text = ""
        txt�����U�֓��S.Text = ""
        txt�����U�֌��T.Text = ""
        txt�����U�֓��T.Text = ""
        txt�����U�֌��U.Text = ""
        txt�����U�֓��U.Text = ""

    End Sub
    Private Sub PSUB_ZUIJI_CHK(ByVal pValue As Boolean)

        '�Ώۊw�N�L���`�F�b�NOFF
        chk�����P_�P�w�N.Checked = pValue
        chk�����P_�Q�w�N.Checked = pValue
        chk�����P_�R�w�N.Checked = pValue
        chk�����P_�S�w�N.Checked = pValue
        chk�����P_�T�w�N.Checked = pValue
        chk�����P_�U�w�N.Checked = pValue
        chk�����P_�V�w�N.Checked = pValue
        chk�����P_�W�w�N.Checked = pValue
        chk�����P_�X�w�N.Checked = pValue
        chk�����P_�S�w�N.Checked = pValue

        chk�����Q_�P�w�N.Checked = pValue
        chk�����Q_�Q�w�N.Checked = pValue
        chk�����Q_�R�w�N.Checked = pValue
        chk�����Q_�S�w�N.Checked = pValue
        chk�����Q_�T�w�N.Checked = pValue
        chk�����Q_�U�w�N.Checked = pValue
        chk�����Q_�V�w�N.Checked = pValue
        chk�����Q_�W�w�N.Checked = pValue
        chk�����Q_�X�w�N.Checked = pValue
        chk�����Q_�S�w�N.Checked = pValue

        chk�����R_�P�w�N.Checked = pValue
        chk�����R_�Q�w�N.Checked = pValue
        chk�����R_�R�w�N.Checked = pValue
        chk�����R_�S�w�N.Checked = pValue
        chk�����R_�T�w�N.Checked = pValue
        chk�����R_�U�w�N.Checked = pValue
        chk�����R_�V�w�N.Checked = pValue
        chk�����R_�W�w�N.Checked = pValue
        chk�����R_�X�w�N.Checked = pValue
        chk�����R_�S�w�N.Checked = pValue

        chk�����S_�P�w�N.Checked = pValue
        chk�����S_�Q�w�N.Checked = pValue
        chk�����S_�R�w�N.Checked = pValue
        chk�����S_�S�w�N.Checked = pValue
        chk�����S_�T�w�N.Checked = pValue
        chk�����S_�U�w�N.Checked = pValue
        chk�����S_�V�w�N.Checked = pValue
        chk�����S_�W�w�N.Checked = pValue
        chk�����S_�X�w�N.Checked = pValue
        chk�����S_�S�w�N.Checked = pValue

        chk�����T_�P�w�N.Checked = pValue
        chk�����T_�Q�w�N.Checked = pValue
        chk�����T_�R�w�N.Checked = pValue
        chk�����T_�S�w�N.Checked = pValue
        chk�����T_�T�w�N.Checked = pValue
        chk�����T_�U�w�N.Checked = pValue
        chk�����T_�V�w�N.Checked = pValue
        chk�����T_�W�w�N.Checked = pValue
        chk�����T_�X�w�N.Checked = pValue
        chk�����T_�S�w�N.Checked = pValue

        chk�����U_�P�w�N.Checked = pValue
        chk�����U_�Q�w�N.Checked = pValue
        chk�����U_�R�w�N.Checked = pValue
        chk�����U_�S�w�N.Checked = pValue
        chk�����U_�T�w�N.Checked = pValue
        chk�����U_�U�w�N.Checked = pValue
        chk�����U_�V�w�N.Checked = pValue
        chk�����U_�W�w�N.Checked = pValue
        chk�����U_�X�w�N.Checked = pValue
        chk�����U_�S�w�N.Checked = pValue

    End Sub
    Private Sub PSUB_ZUIJI_CMB(Optional ByVal pIndex As Integer = 0)

        cmb���o�敪�P.SelectedIndex = pIndex
        cmb���o�敪�Q.SelectedIndex = pIndex
        cmb���o�敪�R.SelectedIndex = pIndex
        cmb���o�敪�S.SelectedIndex = pIndex
        cmb���o�敪�T.SelectedIndex = pIndex
        cmb���o�敪�U.SelectedIndex = pIndex

    End Sub
    Private Sub PSUB_ZUIJI_SET(ByVal cmbBOX As ComboBox, ByVal txtBOX�� As TextBox, ByVal txtBOX�� As TextBox, ByVal chkBOX1 As CheckBox, ByVal chkBOX2 As CheckBox, ByVal chkBOX3 As CheckBox, ByVal chkBOX4 As CheckBox, ByVal chkBOX5 As CheckBox, ByVal chkBOX6 As CheckBox, ByVal chkBOX7 As CheckBox, ByVal chkBOX8 As CheckBox, ByVal chkBOX9 As CheckBox, ByVal chkBOXALL As CheckBox, ByVal aReader As MyOracleReader)

        '����OPEN���Ă���f�[�^�x�[�X�̓��e����ʂɕ\������i�P���ڍs�P�ʁj

        '���o���R���{�̐ݒ�
        cmbBOX.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_NYUSYUTU2_TXT, aReader.GetString("FURI_KBN_S"))

        txtBOX��.Text = Mid(aReader.GetString("FURI_DATE_S"), 5, 2)
        txtBOX��.Text = Mid(aReader.GetString("FURI_DATE_S"), 7, 2)

        Select Case True
            Case aReader.GetString("ENTRI_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("CHECK_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("DATA_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("FUNOU_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("SAIFURI_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("KESSAI_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("TYUUDAN_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
        End Select

        If aReader.GetString("GAKUNEN1_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN2_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN3_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN4_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN5_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN6_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN7_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN8_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN9_FLG_S") = "1" Then
            '�S�w�N�`�F�b�N�{�b�N�X�n�m
            chkBOXALL.Checked = True

            '�P����X�w�N�`�F�b�N�{�N�X�̎g�p�s��
            chkBOX1.Enabled = False
            chkBOX2.Enabled = False
            chkBOX3.Enabled = False
            chkBOX4.Enabled = False
            chkBOX5.Enabled = False
            chkBOX6.Enabled = False
            chkBOX7.Enabled = False
            chkBOX8.Enabled = False
            chkBOX9.Enabled = False
        Else
            If aReader.GetString("GAKUNEN1_FLG_S") = "1" Then
                '�P�w�N�`�F�b�N�{�b�N�X�n�m
                chkBOX1.Checked = True
            Else
                chkBOX1.Checked = False
            End If

            If aReader.GetString("GAKUNEN2_FLG_S") = "1" Then
                '�Q�w�N�`�F�b�N�{�b�N�X�n�m
                chkBOX2.Checked = True
            Else
                chkBOX2.Checked = False
            End If

            If aReader.GetString("GAKUNEN3_FLG_S") = "1" Then
                '�R�w�N�`�F�b�N�{�b�N�X�n�m
                chkBOX3.Checked = True
            Else
                chkBOX3.Checked = False
            End If

            If aReader.GetString("GAKUNEN4_FLG_S") = "1" Then
                '�S�w�N�`�F�b�N�{�b�N�X�n�m
                chkBOX4.Checked = True
            Else
                chkBOX4.Checked = False
            End If

            If aReader.GetString("GAKUNEN5_FLG_S") = "1" Then
                '�T�w�N�`�F�b�N�{�b�N�X�n�m
                chkBOX5.Checked = True
            Else
                chkBOX5.Checked = False
            End If

            If aReader.GetString("GAKUNEN6_FLG_S") = "1" Then
                '�U�w�N�`�F�b�N�{�b�N�X�n�m
                chkBOX6.Checked = True
            Else
                chkBOX6.Checked = False
            End If

            If aReader.GetString("GAKUNEN7_FLG_S") = "1" Then
                '�V�w�N�`�F�b�N�{�b�N�X�n�m
                chkBOX7.Checked = True
            Else
                chkBOX7.Checked = False
            End If

            If aReader.GetString("GAKUNEN8_FLG_S") = "1" Then
                '�W�w�N�`�F�b�N�{�b�N�X�n�m
                chkBOX8.Checked = True
            Else
                chkBOX8.Checked = False
            End If

            If aReader.GetString("GAKUNEN9_FLG_S") = "1" Then
                '�X�w�N�`�F�b�N�{�b�N�X�n�m
                chkBOX9.Checked = True
            Else
                chkBOX9.Checked = False
            End If
        End If

    End Sub
#End Region

#Region " Private Function(�����X�P�W���[��)"
    Private Function PFUNC_SCH_GET_ZUIJI() As Boolean

        PFUNC_SCH_GET_ZUIJI = False

        '��������
        '�Ώۊw�N�`�F�b�N�a�n�w�̗L����
        Call PSUB_ZUIJI_CHKBOXEnabled(True)

        '�����Ώۊw�N�w��`�F�b�NOFF
        Call PSUB_ZUIJI_CHK(False)

        '�U�֓����͗��̃N���A
        Call PSUB_ZUIJI_DAYCLER()

        '�������� �Q�Ƌ@�\
        If PFUNC_ZUIJI_SANSYOU() = False Then
            Exit Function
        End If

        PFUNC_SCH_GET_ZUIJI = True

    End Function

    Private Function PFUNC_SCH_DELETE_INSERT_ZUIJI() As Boolean

        '�����X�P�W���[���X�V����
        If PFUNC_ZUIJI_KOUSIN() = False Then

            '������ʂ�Ƃ������Ƃ͂P���ł������������R�[�h�����݂����Ƃ������ƂȂ̂�
            Int_Syori_Flag(2) = 2

            Return False
        End If

        Return True

    End Function

    Private Function PFUNC_ZUIJI_SANSYOU() As Boolean

        '�����U�֓��@�Q�Ə���
        PFUNC_ZUIJI_SANSYOU = False

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        For i As Integer = 1 To 6
            SYOKI_ZUIJI_SCHINFO(i).Syori_Flag = False
        Next

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = 2")
        sql.Append(" ORDER BY FURI_DATE_S ASC")

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                '�󂢂Ă��鍀�ڍs�Ƀf�[�^�x�[�X�̓��e���Z�b�g����
                Select Case True
                    Case (txt�����U�֌��P.Text = "")
                        Int_Zuiji_Flag = 1
                        Call PSUB_ZUIJI_SET(cmb���o�敪�P, txt�����U�֌��P, txt�����U�֓��P, chk�����P_�P�w�N, chk�����P_�Q�w�N, chk�����P_�R�w�N, chk�����P_�S�w�N, chk�����P_�T�w�N, chk�����P_�U�w�N, chk�����P_�V�w�N, chk�����P_�W�w�N, chk�����P_�X�w�N, chk�����P_�S�w�N, oraReader)
                    Case (txt�����U�֌��Q.Text = "")
                        Int_Zuiji_Flag = 2
                        Call PSUB_ZUIJI_SET(cmb���o�敪�Q, txt�����U�֌��Q, txt�����U�֓��Q, chk�����Q_�P�w�N, chk�����Q_�Q�w�N, chk�����Q_�R�w�N, chk�����Q_�S�w�N, chk�����Q_�T�w�N, chk�����Q_�U�w�N, chk�����Q_�V�w�N, chk�����Q_�W�w�N, chk�����Q_�X�w�N, chk�����Q_�S�w�N, oraReader)
                    Case (txt�����U�֌��R.Text = "")
                        Int_Zuiji_Flag = 3
                        Call PSUB_ZUIJI_SET(cmb���o�敪�R, txt�����U�֌��R, txt�����U�֓��R, chk�����R_�P�w�N, chk�����R_�Q�w�N, chk�����R_�R�w�N, chk�����R_�S�w�N, chk�����R_�T�w�N, chk�����R_�U�w�N, chk�����R_�V�w�N, chk�����R_�W�w�N, chk�����R_�X�w�N, chk�����R_�S�w�N, oraReader)
                    Case (txt�����U�֌��S.Text = "")
                        Int_Zuiji_Flag = 4
                        Call PSUB_ZUIJI_SET(cmb���o�敪�S, txt�����U�֌��S, txt�����U�֓��S, chk�����S_�P�w�N, chk�����S_�Q�w�N, chk�����S_�R�w�N, chk�����S_�S�w�N, chk�����S_�T�w�N, chk�����S_�U�w�N, chk�����S_�V�w�N, chk�����S_�W�w�N, chk�����S_�X�w�N, chk�����S_�S�w�N, oraReader)
                    Case (txt�����U�֌��T.Text = "")
                        Int_Zuiji_Flag = 5
                        Call PSUB_ZUIJI_SET(cmb���o�敪�T, txt�����U�֌��T, txt�����U�֓��T, chk�����T_�P�w�N, chk�����T_�Q�w�N, chk�����T_�R�w�N, chk�����T_�S�w�N, chk�����T_�T�w�N, chk�����T_�U�w�N, chk�����T_�V�w�N, chk�����T_�W�w�N, chk�����T_�X�w�N, chk�����T_�S�w�N, oraReader)
                    Case (txt�����U�֌��U.Text = "")
                        Int_Zuiji_Flag = 6
                        Call PSUB_ZUIJI_SET(cmb���o�敪�U, txt�����U�֌��U, txt�����U�֓��U, chk�����U_�P�w�N, chk�����U_�Q�w�N, chk�����U_�R�w�N, chk�����U_�S�w�N, chk�����U_�T�w�N, chk�����U_�U�w�N, chk�����U_�V�w�N, chk�����U_�W�w�N, chk�����U_�X�w�N, chk�����U_�S�w�N, oraReader)
                End Select

                oraReader.NextRead()

            Loop
        Else

            oraReader.Close()
            Return False

        End If

        oraReader.Close()

        Return True

    End Function
    Private Function PFUNC_ZUIJI_SAKUSEI(ByVal str���� As String) As Boolean

        '�����U�ց@�쐬����
        Dim str���o�敪 As String
        Dim cmbComboName(6) As ComboBox '2006/11/30�@�R���{�{�b�N�X��

        PFUNC_ZUIJI_SAKUSEI = False

        '2006/11/30�@�R���{�{�b�N�X�����擾
        cmbComboName(1) = cmb���o�敪�P
        cmbComboName(2) = cmb���o�敪�Q
        cmbComboName(3) = cmb���o�敪�R
        cmbComboName(4) = cmb���o�敪�S
        cmbComboName(5) = cmb���o�敪�T
        cmbComboName(6) = cmb���o�敪�U

        For i As Integer = 1 To 6

            '�V�K�쐬
            '2006/12/06�@�ύX�����������݂̂��X�V�E�쐬
            If bln�����X�V(i) = True And ZUIJI_SCHINFO(i).Furikae_Tuki <> "" And ZUIJI_SCHINFO(i).Furikae_Date <> "" Then

                If PFUNC_GAKUNENFLG_CHECK(ZUIJI_SCHINFO(i).SiyouGakunen1_Check, ZUIJI_SCHINFO(i).SiyouGakunen2_Check, ZUIJI_SCHINFO(i).SiyouGakunen3_Check, ZUIJI_SCHINFO(i).SiyouGakunen4_Check, ZUIJI_SCHINFO(i).SiyouGakunen5_Check, ZUIJI_SCHINFO(i).SiyouGakunen6_Check, ZUIJI_SCHINFO(i).SiyouGakunen7_Check, ZUIJI_SCHINFO(i).SiyouGakunen8_Check, ZUIJI_SCHINFO(i).SiyouGakunen9_Check, ZUIJI_SCHINFO(i).SiyouGakunenALL_Check) = False Then
                    Exit Function
                End If

                str���o�敪 = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmbComboName(i))

                '�p�����^�͇@���A�A���͐U�֓��A�B�ĐU�֌��@�C�ĐU�֓��@�D�U�֋敪�i���o�敪)�A�E�X�P�W���[���敪�i2:����)
                If PFUNC_ZUIJI_SAKUSEI_SUB(ZUIJI_SCHINFO(i).Furikae_Tuki, ZUIJI_SCHINFO(i).Furikae_Tuki, ZUIJI_SCHINFO(i).Furikae_Date, "", "", str���o�敪) = False Then
                    Exit Function
                End If

                '������ʂ�Ƃ������Ƃ͏����ɐ��������Ƃ������ƂȂ̂�
                Int_Syori_Flag(2) = 1
            End If
            'End If

        Next

        PFUNC_ZUIJI_SAKUSEI = True

    End Function

    Private Function PFUNC_ZUIJI_SAKUSEI_SUB(ByVal s������ As String, ByVal s�� As String, ByVal s�U�֓� As String, ByVal s�ĐU�֌� As String, ByVal s�ĐU�֓� As String, ByVal s�U�֋敪 As String) As Boolean

        '�X�P�W���[���쐬�@�������R�[�h�쐬
        PFUNC_ZUIJI_SAKUSEI_SUB = False
        '�����N���̍쐬
        STR�����N�� = PFUNC_SEIKYUTUKIHI(s������)
        '�U�֓��Z�o
        STR�U�֓� = PFUNC_FURIHI_MAKE(s��, s�U�֓�, "2", s�U�֋敪)

        '2010/10/21 �_��U�֓����Z�o����
        STR�_��U�֓� = PFUNC_KFURIHI_MAKE(s��, s�U�֓�, "2", s�U�֋敪)
        '�ĐU��
        STR�ĐU�֓� = "00000000"

        '�X�P�W���[���敪�̋��ʕϐ��ݒ�
        STR�X�P�敪 = "2"
        '�U�֋敪�̋��ʕϐ��ݒ�
        STR�U�֋敪 = s�U�֋敪
        '���͐U�֓��̋��ʕϐ��ݒ�
        STR�N�ԓ��͐U�֓� = Space(15)

        Dim strSQL As String = ""
        '�X�P�W���[���}�X�^�o�^(���U)SQL���쐬
        strSQL = PSUB_INSERT_G_SCHMAST_SQL()

        If MainDB.ExecuteNonQuery(strSQL) < 0 Then
            Return False
        End If

        '-----------------------------------------------
        '2006/07/26�@��Ǝ��U�̐����̃X�P�W���[�����쐬
        '-----------------------------------------------
        '��Ǝ��U�A�g���̂�
        Dim strTORIF_CODE_N As String
        If STR�U�֋敪 = "2" Then  '����
            strTORIF_CODE_N = "03"
        Else  '�o��
            strTORIF_CODE_N = "04"
        End If

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '���ɓo�^����Ă��邩�`�F�b�N
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '" & strTORIF_CODE_N & "' AND ")
        sql.Append("FURI_DATE_S = '" & STR�U�֓� & "'")

        '�Ǎ��̂�
        If oraReader.DataReader(sql) = True Then    '�X�P�W���[�������ɑ��݂���
        Else     '�X�P�W���[�������݂��Ȃ�
            '�X�P�W���[���쐬
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, strTORIF_CODE_N, gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                     gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '�����Ƀq�b�g���Ȃ�������

                '2010/10/21 �_��U�֓��Ή� �����ɒǉ�
                'If fn_INSERTSCHMAST(strGakkouCode, strTORIF_CODE_N, STR�U�֓�, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                If fn_INSERTSCHMAST(strGakkouCode, strTORIF_CODE_N, STR�U�֓�, gintPG_KBN.KOBETU, STR�_��U�֓�) = gintKEKKA.NG Then
                    oraReader.Close()
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", "��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���")
                    MessageBox.Show("��Ǝ��U�̃X�P�W���[�����o�^�ł��܂���ł���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If
        End If
        oraReader.Close()

        '�ĐU���R�[�h�쐬�͂Ȃ�
        PFUNC_ZUIJI_SAKUSEI_SUB = True

    End Function

    Private Function PFUNC_ZUIJI_KOUSIN() As Boolean

        '�폜�����iDELETE�j
        If PFUNC_ZUIJI_DELETE() = False Then
            Return False
        End If

        '2010/10/21 �����X�P�W���[���̕ύX�ɑΉ�����
        '�폜���ꂽ���R�[�h�̍X�V�t���O��False�ƂȂ��Ă��邽�߁A������x�A�쐬���ėǂ����`�F�b�N����
        For i As Integer = 1 To 6
            '--------------------------------------
            '�����X�P�W���[���`�F�b�N
            '--------------------------------------
            '2006/12/12�@�ꕔ�ǉ��F���͂��s�����Ă����ꍇ�A�X�V���Ȃ�
            If (ZUIJI_SCHINFO(i).Furikae_Tuki = SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki And _
               ZUIJI_SCHINFO(i).Furikae_Date = SYOKI_ZUIJI_SCHINFO(i).Furikae_Date And _
               ZUIJI_SCHINFO(i).Nyusyutu_Kbn = SYOKI_ZUIJI_SCHINFO(i).Nyusyutu_Kbn And _
               ZUIJI_SCHINFO(i).SiyouGakunen1_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen1_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen2_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen2_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen3_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen3_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen4_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen4_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen5_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen5_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen6_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen6_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen7_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen7_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen8_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen8_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen9_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen9_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunenALL_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunenALL_Check) Or _
               ((ZUIJI_SCHINFO(i).Furikae_Tuki = "" And ZUIJI_SCHINFO(i).Furikae_Date <> "") Or _
               (ZUIJI_SCHINFO(i).Furikae_Tuki <> "" And ZUIJI_SCHINFO(i).Furikae_Date = "")) Then

                bln�����X�V(i) = False '�ύX�Ȃ�
            Else
                bln�����X�V(i) = True ' �ύX����
            End If
        Next

        '�쐬�����iINSERT & UPDATE)
        If PFUNC_ZUIJI_SAKUSEI("�X�V") = False Then
            Return False
        End If

        Return True

    End Function

    '===============================
    '�����f�[�^�폜�����@2006/11/30
    '===============================
    Private Function PFUNC_ZUIJI_DELETE() As Boolean

        Dim sql As New StringBuilder(128)
        Dim bret As Boolean = False
        Dim blnSakujo_Check As Boolean = False '2006/11/30
        Dim strNengetu As String '   �����N��
        Dim strSFuri_Date As String '�ĐU��

        '�S�폜�����A�L�[�͊w�Z�R�[�h�A�Ώ۔N�x�A�X�P�W���[���敪�i�Q�F�����j�A�����t���O�i�O�j

        sql.Append(" DELETE  FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S =2")

        '2006/11/30�@�����ύX�i�t���O�̗����Ă��Ȃ��f�[�^�E�ύX�̂������f�[�^���폜�j
        sql.Append(" AND")
        sql.Append(" (ENTRI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" CHECK_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" DATA_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" FUNOU_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" SAIFURI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" KESSAI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" TYUUDAN_FLG_S =0)")

        For i As Integer = 1 To 6

            '�ύX�����������̂��폜����i�����X�P�W���[���͏�ɍč쐬�\�Ƃ���j
            If bln�����X�V(i) = True And SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki <> "" And SYOKI_ZUIJI_SCHINFO(i).Furikae_Date <> "" Then

                '�N���x���擾
                If CInt(SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki) < 4 Then
                    strNengetu = CInt(txt�Ώ۔N�x.Text) + 1 & SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki
                Else
                    strNengetu = txt�Ώ۔N�x.Text & SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki
                End If

                '�ĐU���� "0" 8��
                strSFuri_Date = "00000000"

                If blnSakujo_Check = True Then
                    sql.Append(" or")
                Else
                    '2010/10/21 or���ƑS�Ă̐����X�P�W���[�����폜����Ă��܂�
                    'sql.Append(" or(")
                    sql.Append(" AND (")
                End If

                '�����ǉ�
                sql.Append(" (FURI_DATE_S = '" & strNengetu & SYOKI_ZUIJI_SCHINFO(i).Furikae_Date & "'")
                sql.Append(" AND")
                sql.Append(" SFURI_DATE_S = '" & strSFuri_Date & "'")
                sql.Append(" AND")
                sql.Append(" FURI_KBN_S = '" & SYOKI_ZUIJI_SCHINFO(i).Nyusyutu_Kbn & "')")

                bln�����X�V(i) = False '�ύX�t���O���~�낷
                blnSakujo_Check = True '�폜�t���O�𗧂Ă�

            End If
        Next

        If blnSakujo_Check = True Then
            sql.Append(")")
        End If

        '2006/12/11�@�폜����Ώۂ��ꌏ��������������s���Ȃ�
        If blnSakujo_Check = True Then

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                '�폜�����G���[
                MessageBox.Show("(�����X�P�W���[��)" & vbCrLf & "�X�P�W���[���̍폜�����ŃG���[���������܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                bret = False
            End If

        End If

        Return True

    End Function

    Private Sub PSUB_ZGAKUNEN_CHK()
        '2006/10/12�@�g�p���Ă��Ȃ��w�N�̃`�F�b�N�{�b�N�X���g�p�s�ɂ���

        If GAKKOU_INFO.SIYOU_GAKUNEN <> 9 Then
            chk�����P_�X�w�N.Enabled = False
            chk�����Q_�X�w�N.Enabled = False
            chk�����R_�X�w�N.Enabled = False
            chk�����S_�X�w�N.Enabled = False
            chk�����T_�X�w�N.Enabled = False
            chk�����U_�X�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 8 Then
            chk�����P_�W�w�N.Enabled = False
            chk�����Q_�W�w�N.Enabled = False
            chk�����R_�W�w�N.Enabled = False
            chk�����S_�W�w�N.Enabled = False
            chk�����T_�W�w�N.Enabled = False
            chk�����U_�W�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 7 Then
            chk�����P_�V�w�N.Enabled = False
            chk�����Q_�V�w�N.Enabled = False
            chk�����R_�V�w�N.Enabled = False
            chk�����S_�V�w�N.Enabled = False
            chk�����T_�V�w�N.Enabled = False
            chk�����U_�V�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 6 Then
            chk�����P_�U�w�N.Enabled = False
            chk�����Q_�U�w�N.Enabled = False
            chk�����R_�U�w�N.Enabled = False
            chk�����S_�U�w�N.Enabled = False
            chk�����T_�U�w�N.Enabled = False
            chk�����U_�U�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 5 Then
            chk�����P_�T�w�N.Enabled = False
            chk�����Q_�T�w�N.Enabled = False
            chk�����R_�T�w�N.Enabled = False
            chk�����S_�T�w�N.Enabled = False
            chk�����T_�T�w�N.Enabled = False
            chk�����U_�T�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 4 Then
            chk�����P_�S�w�N.Enabled = False
            chk�����Q_�S�w�N.Enabled = False
            chk�����R_�S�w�N.Enabled = False
            chk�����S_�S�w�N.Enabled = False
            chk�����T_�S�w�N.Enabled = False
            chk�����U_�S�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 3 Then
            chk�����P_�R�w�N.Enabled = False
            chk�����Q_�R�w�N.Enabled = False
            chk�����R_�R�w�N.Enabled = False
            chk�����S_�R�w�N.Enabled = False
            chk�����T_�R�w�N.Enabled = False
            chk�����U_�R�w�N.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 2 Then
            chk�����P_�Q�w�N.Enabled = False
            chk�����Q_�Q�w�N.Enabled = False
            chk�����R_�Q�w�N.Enabled = False
            chk�����S_�Q�w�N.Enabled = False
            chk�����T_�Q�w�N.Enabled = False
            chk�����U_�Q�w�N.Enabled = False
        End If
    End Sub

#End Region

#Region "�֐�"

    Public Function fn_DELETESCHMAST(ByVal astrTORIF_CODE As String, ByVal astrFURI_DATE As String) As Boolean
        '----------------------------------------------------------------------------
        'Name       :fn_UPDATE_SCHMAST
        'Description:SCHMAST�X�V(�L���t���O)
        'Create     :
        'UPDATE     :
        '----------------------------------------------------------------------------

        '��Ǝ��U�̃X�P�W���[�����폜
        Dim ret As Boolean = False

        Dim SQL As New System.Text.StringBuilder(128)

        Try
            SQL.Append(" DELETE  FROM SCHMAST ")
            SQL.Append(" WHERE TORIS_CODE_S = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
            SQL.Append(" AND TORIF_CODE_S = '" & astrTORIF_CODE & "'")
            SQL.Append(" AND FURI_DATE_S = '" & astrFURI_DATE & "'")
            SQL.Append(" AND UKETUKE_FLG_S = '0'")
            SQL.Append(" AND TOUROKU_FLG_S = '0'")
            SQL.Append(" AND HAISIN_FLG_S = '0'")

            If MainDB.ExecuteNonQuery(SQL) < 0 Then
                MainLOG.Write("���U�X�P�W���[��DELETE", "���s", "SQL:" & SQL.ToString)
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            MainLOG.Write("���U�X�P�W���[��DELETE", "���s", "SQL:" & SQL.ToString & "DETAIL:" & ex.ToString)
        End Try

        Return ret

    End Function

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
    '
    '2010/10/21 �_��U�֓��Ή� �_��U�֓��������ɒǉ�(�ȗ���)
    'Private Function fn_INSERTSCHMAST(ByVal aTORIS_CODE As String, ByVal aTORIF_CODE As String, ByVal aFURI_DATE As String, ByVal aPG_KUBUN As Integer) As Integer
    Private Function fn_INSERTSCHMAST(ByVal aTORIS_CODE As String, ByVal aTORIF_CODE As String, ByVal aFURI_DATE As String, ByVal aPG_KUBUN As Integer, Optional ByVal aKFURI_DATE As String = "") As Integer
        '----------------------------------------------------------------------------
        'Name       :fn_insert_SCHMAST
        'Description:�X�P�W���[���쐬
        'Parameta   :TORIS_CODE , TORIF_CODE,FURI_DATE,TIME_STAMP,PG_KUBUN 1:�� �@2:�ꊇ
        'Create     :2004/08/02
        'UPDATE     :2007/12/26
        '           :***�C���@�ɶ��ϲ�� (��Ǝ��U���ޭ��Ͻ��������Ɋ�Ƒ����ޭ��Ͻ��̍��ڒǉ��ׁ̈j
        '----------------------------------------------------------------------------

        Dim RetCode As Integer = gintKEKKA.NG

        Dim oraReader As New MyOracleReader(MainDB)

        Try
            Dim SQL As StringBuilder
            Dim SCH_DATA(77) As String
            Dim strFURI_DATE As String
            Dim Ret As String

            Dim CLS As New GAKKOU.ClsSchduleMaintenanceClass
            CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

            strFURI_DATE = aFURI_DATE.Substring(0, 4) & "/" & aFURI_DATE.Substring(4, 2) & "/" & aFURI_DATE.Substring(6, 2)

            '----------------
            '�����}�X�^����
            '----------------
            SQL = New StringBuilder(128)

            SQL.Append(" SELECT * FROM TORIMAST ")
            SQL.Append(" WHERE TORIS_CODE_T = '" & aTORIS_CODE.Trim & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & aTORIF_CODE.Trim & "'")

            If oraReader.DataReader(SQL) = False Then
                MessageBox.Show("�����}�X�^�ɍĐU����悪�o�^����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                RetCode = gintKEKKA.NG
                Return RetCode
            End If

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

            '2010/10/21 �_��U�֓��Ή� ��������
            If aKFURI_DATE = "" OrElse aKFURI_DATE.Length <> 8 Then
                '�������Ȃ��ꍇ�͎��U�֓���ݒ�
                CLS.SCH.KFURI_DATE = strFURI_DATE
            Else
                CLS.SCH.KFURI_DATE = aKFURI_DATE
            End If
            '2010/10/21 �_��U�֓��Ή� �����܂�

            Ret = CLS.INSERT_NEW_SCHMAST(0, False, True)

            '------------------
            '�}�X�^�o�^���ڐݒ�
            '------------------
            SCH_DATA(0) = oraReader.GetString("FSYORI_KBN_T")                                       '�U�֏����敪
            SCH_DATA(1) = aTORIS_CODE                                                           '������R�[�h
            SCH_DATA(2) = aTORIF_CODE                                                           '����敛�R�[�h
            SCH_DATA(3) = CLS.SCH.FURI_DATE 'strIN_NEN & strIN_TUKI & strIN_HI 'FURI_DATE_S�@ �@'�U�֓�
            '2010/10/21 �_��U�֓��Ή�
            'SCH_DATA(4) = CLS.SCH.FURI_DATE '"00000000" 'SAIFURI_DATE_S                         '�_��U�֓�=�U�֓�
            SCH_DATA(4) = CLS.SCH.KFURI_DATE                                                    '�_��U�֓�
            SCH_DATA(5) = "00000000"                                                            '�ĐU��
            SCH_DATA(6) = CLS.SCH.KSAIFURI_DATE                                                 '�ĐU�\���
            SCH_DATA(7) = CStr(ConvNullToString(oraReader.GetString("FURI_CODE_T"))).PadLeft(3, "0")  '�U�փR�[�h�r
            SCH_DATA(8) = CStr(ConvNullToString(oraReader.GetString("KIGYO_CODE_T"))).PadLeft(4, "0") '��ƃR�[�h�r
            SCH_DATA(9) = CLS.TR(0).ITAKU_CODE '�ϑ��҃R�[�h
            SCH_DATA(10) = CStr(oraReader.GetString("TKIN_NO_T")).PadLeft(4, "0")
            SCH_DATA(11) = CStr(oraReader.GetString("TSIT_NO_T")).PadLeft(3, "0")
            SCH_DATA(12) = oraReader.GetString("SOUSIN_KBN_T")
            SCH_DATA(13) = oraReader.GetString("MOTIKOMI_KBN_T")
            SCH_DATA(14) = oraReader.GetString("BAITAI_CODE_T") 'BAITAI_CODE_S
            SCH_DATA(15) = 0 'MOTIKOMI_SEQ_S
            SCH_DATA(16) = 0 'FILE_SEQ_S
            '�萔���v�Z�敪�̎Z�o
            Dim strTUKI_KBN As String = ""
            Select Case aFURI_DATE.Substring(4, 2)
                Case "01"
                    strTUKI_KBN = oraReader.GetString("TUKI1_T")
                Case "02"
                    strTUKI_KBN = oraReader.GetString("TUKI2_T")
                Case "03"
                    strTUKI_KBN = oraReader.GetString("TUKI3_T")
                Case "04"
                    strTUKI_KBN = oraReader.GetString("TUKI4_T")
                Case "05"
                    strTUKI_KBN = oraReader.GetString("TUKI5_T")
                Case "06"
                    strTUKI_KBN = oraReader.GetString("TUKI6_T")
                Case "07"
                    strTUKI_KBN = oraReader.GetString("TUKI7_T")
                Case "08"
                    strTUKI_KBN = oraReader.GetString("TUKI8_T")
                Case "09"
                    strTUKI_KBN = oraReader.GetString("TUKI9_T")
                Case "10"
                    strTUKI_KBN = oraReader.GetString("TUKI10_T")
                Case "11"
                    strTUKI_KBN = oraReader.GetString("TUKI11_T")
                Case "12"
                    strTUKI_KBN = oraReader.GetString("TUKI12_T")
            End Select

            Select Case oraReader.GetString("TESUUTYO_KBN_T")
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

            SQL.Append("INSERT INTO SCHMAST ( ")
            SQL.Append("FSYORI_KBN_S")      '0
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

            If MainDB.ExecuteNonQuery(InsertSchmastSQL) < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", SQL.ToString)
                Return False
                ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
            Else
                If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                    Dim ReturnMessage As String = String.Empty
                    Dim SubMastInsert_Ret As Integer = 0
                    Call CAstExternal.ModExternal.Ex_InsertSchmastSub(SCH_DATA(0), _
                                                                      SCH_DATA(1), _
                                                                      SCH_DATA(2), _
                                                                      SCH_DATA(3), _
                                                                      0, _
                                                                      ReturnMessage, _
                                                                      MainDB)
                End If
                ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END
            End If

            RetCode = gintKEKKA.OK

        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�\�����ʃG���[", "���s", ex.ToString)
            RetCode = gintKEKKA.NG

            Return RetCode

        Finally

            If Not oraReader Is Nothing Then oraReader.Close()

        End Try

        Return RetCode

    End Function
#End Region

#Region " ����Key����֐�"

    Public Function GFUNC_KEYCHECK(ByRef P_FORM As Form, _
                                   ByRef P_e As System.Windows.Forms.KeyPressEventArgs, _
                                   ByVal P_Mode As Integer) As Boolean
        GFUNC_KEYCHECK = False

        '*****************************************
        '����KEY����
        '*****************************************
        'ENTER�L�[�Ŏ�Control��Focus�ړ�
        If P_e.KeyChar = ChrW(13) Then
            P_FORM.SelectNextControl(P_FORM.ActiveControl, True, True, True, True)
        End If

        'BS�ETAB�EENTER�L�[���͎��̓X�L�b�v
        Select Case P_e.KeyChar
            Case ControlChars.Back, ControlChars.Tab, ChrW(13)

                Exit Function
        End Select

        Select Case P_Mode
            Case 1
                If (P_e.KeyChar < "0"c Or P_e.KeyChar > "9"c) Then
                    P_e.Handled = True
                End If
            Case 2
                If (P_e.KeyChar >= "0"c Or P_e.KeyChar <= "9"c) Or _
                   (P_e.KeyChar >= "A"c Or P_e.KeyChar <= "Z"c) Or _
                   (P_e.KeyChar >= "a"c Or P_e.KeyChar <= "z"c) Then
                Else
                    P_e.Handled = True
                End If
            Case 3
                If (P_e.KeyChar >= "A"c Or P_e.KeyChar <= "Z"c) Or _
                   (P_e.KeyChar >= "a"c Or P_e.KeyChar <= "z"c) Then
                Else
                    P_e.Handled = True
                End If
            Case 5
                If (P_e.KeyChar < "�"c Or P_e.KeyChar > "�"c) Then
                    P_e.Handled = True
                End If
            Case 6 '2007/02/12�@�t���O�p
                If (P_e.KeyChar < "0"c Or P_e.KeyChar > "1"c) Then
                    P_e.Handled = True
                End If
            Case 10
                If (P_e.KeyChar < "1"c Or P_e.KeyChar > "9"c) Then
                    P_e.Handled = True
                End If
        End Select

        GFUNC_KEYCHECK = True
    End Function
    Public Sub GSUB_PRESEL(ByRef pTxtFile As TextBox)
        'TEXT��޼ު�Ă̓��e��S�I��
        pTxtFile.SelectionStart = 0
        pTxtFile.SelectionLength = Len(pTxtFile.Text)
    End Sub
    Public Sub GSUB_NEXTFOCUS(ByRef P_FORM As Form, _
                              ByRef P_e As System.Windows.Forms.KeyEventArgs, _
                              ByRef pTxtFile As TextBox)

        Select Case P_e.KeyData
            Case Keys.Right, Keys.Left
                '���E���{�^��
                Exit Sub
            Case Keys.Back, Keys.Tab, Keys.Enter
                'BS�ETAB�EENTER�L�[
                Exit Sub
            Case Keys.ShiftKey, 65545
                'Shift + Tab�L�[(KeyUp�Ȃ̂�Shift�L�[�P�̂��K�v)
                Exit Sub
        End Select

        '���͌��ƍő���͌�������v�����Focus�ړ�
        If pTxtFile.MaxLength = Len(Trim(pTxtFile.Text)) Then
            P_FORM.SelectNextControl(P_FORM.ActiveControl, True, True, True, True)
        End If

    End Sub
#End Region

#Region " �w�茅�OZERO�l�ߋ��ʊ֐�"
    Public Function GFUNC_ZERO_ADD(ByRef pTxtFile As TextBox, _
                                   ByVal pKeta As Byte) As Boolean
        GFUNC_ZERO_ADD = False
        pTxtFile.Text = pTxtFile.Text.Trim.PadLeft(pKeta, "0"c)
        GFUNC_ZERO_ADD = True
    End Function

#End Region

    '
    '�@�֐����@-�@fn_ToriMastIsExist
    '
    '�@�@�\    -  �����}�X�^���݃`�F�b�N
    '
    '�@����    -  
    '
    '�@���l    -  �ʏ�A�������ɏ�����
    '
    '
    Private Function fn_IsExistToriMast(ByVal TorisCode As String, _
                                        ByVal TorifCode As String, _
                                        ByRef ItakuKName As String, _
                                        ByRef ItakuNName As String, _
                                        ByRef KigyoCode As String, _
                                        ByRef FuriCode As String, _
                                        ByRef BaitaiCode As String, _
                                        ByRef FmtKbn As String, _
                                        ByRef FileName As String) As Boolean

        Dim ret As Boolean = False
        Dim OraReader As New MyOracleReader(MainDB)

        Try
            Dim SQL As String = ""
            SQL = " SELECT * "
            SQL &= " FROM TORIMAST "
            SQL &= " WHERE TORIS_CODE_T = '" & TorisCode & "'"
            SQL &= " AND TORIF_CODE_T = '" & TorifCode & "'"

            If OraReader.DataReader(SQL) = False Then
                ret = False
            Else
                ItakuKName = OraReader.GetString("ITAKU_KNAME_T")
                ItakuNName = OraReader.GetString("ITAKU_NNAME_T")
                KigyoCode = OraReader.GetString("KIGYO_CODE_T")
                FuriCode = OraReader.GetString("FURI_CODE_T")
                BaitaiCode = OraReader.GetString("BAITAI_CODE_T")
                FmtKbn = OraReader.GetString("FMT_KBN_T")
                FileName = OraReader.GetString("FILE_NAME_T")

                ret = True
            End If

            OraReader.Close()
            OraReader = Nothing

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�\�����ʃG���[", "���s", ex.ToString)
            ret = False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- END

        '�w�Z����
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
            Exit Sub
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then

            Exit Sub
        End If

        '�N�ԃX�P�W���[����ʏ�����
        Call PSUB_NENKAN_FORMAT()

        '���ʃX�P�W���[����ʏ�����
        Call PSUB_TOKUBETU_FORMAT()

        '�����X�P�W���[����ʏ�����
        Call PSUB_ZUIJI_FORMAT()

        '�w�Z������̊w�Z�R�[�h�ݒ�
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())
        '2007/02/15
        txtGAKKOU_CODE.Focus()

        '�w�Z���̎擾(�w�Z�����ϐ��Ɋi�[�����)
        If PFUNC_GAKINFO_GET() = False Then
            Exit Sub
        End If

        '2006/10/12�@�ō��w�N�ȏ�̊w�N�̎g�p�s��
        PSUB_TGAKUNEN_CHK()
        PSUB_ZGAKUNEN_CHK()

        '�ĐU�֓��̃v���e�N�gTrue
        Call PSUB_SAIFURI_PROTECT(True)

        Select Case GAKKOU_INFO.SFURI_SYUBETU
            Case "0", "3"
                Call PSUB_SAIFURI_PROTECT(False)
        End Select
        '2007/02/15
        If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txt�Ώ۔N�x.Text) <> "" Then
            '�Ώ۔N�x�����͂���Ă���ꍇ�A�X�P�W���[�����݃`�F�b�N������
            '�X�P�W���[�������݂���ꍇ�͎Q�ƃ{�^���Ƀt�H�[�J�X�ړ�
            Call PSUB_SANSYOU_FOCUS()
        End If

    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

#Region "���ʐU�֓����̓`�F�b�N"
    '2011/06/16 �W���ŏC�� ���ʐU�֓����փ`�F�b�N�ǉ� ------------------START
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False
        Try
            '���ʐ������P
            If txt���ʐ������P.Text.Trim <> "" Then
                If txt���ʐU�֌��P.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʐU�֌�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʐU�֌��P.Focus()
                    Return False
                Else
                    If txt���ʐU�֓��P.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "���ʐU�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt���ʐU�֓��P.Focus()
                        Return False
                    End If
                End If
            Else
                If txt���ʐU�֓��P.Text.Trim <> "" OrElse txt���ʐU�֌��P.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʐ�����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʐ������P.Focus()
                    Return False
                End If
            End If
            If txt���ʍĐU�֌��P.Text.Trim = "" Then
                If txt���ʍĐU�֓��P.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʍĐU�֌�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʍĐU�֌��P.Focus()
                    Return False
                End If
            Else
                If txt���ʍĐU�֓��P.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʍĐU�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʍĐU�֓��P.Focus()
                    Return False
                Else
                    '�U�֓��A�ĐU�����փ`�F�b�N
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '���U���ݒ�
                    If CInt(txt���ʐU�֌��P.Text) >= 1 AndAlso CInt(txt���ʐU�֌��P.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʐU�֌��P.Text & txt���ʐU�֓��P.Text
                    Else
                        FURI_DATE = txt�Ώ۔N�x.Text & txt���ʐU�֌��P.Text & txt���ʐU�֓��P.Text
                    End If
                    '�ĐU���ݒ�
                    If CInt(txt���ʍĐU�֌��P.Text) >= 1 AndAlso CInt(txt���ʍĐU�֌��P.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʍĐU�֌��P.Text & txt���ʍĐU�֓��P.Text
                    Else
                        If txt���ʐU�֌��P.Text = "03" AndAlso txt���ʍĐU�֌��P.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʍĐU�֌��P.Text & txt���ʍĐU�֓��P.Text
                        Else
                            SAIFURI_DATE = txt�Ώ۔N�x.Text & txt���ʍĐU�֌��P.Text & txt���ʍĐU�֓��P.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("�ĐU���ɂ͏��U���ȍ~�̐U�֓���ݒ肵�Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt���ʍĐU�֌��P.Focus()
                        Return False
                    End If
                End If
            End If

            '���ʐ������Q
            If txt���ʐ������Q.Text.Trim <> "" Then
                If txt���ʐU�֌��Q.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʐU�֌�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʐU�֌��Q.Focus()
                    Return False
                Else
                    If txt���ʐU�֓��Q.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "���ʐU�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt���ʐU�֓��Q.Focus()
                        Return False
                    End If
                End If
            Else
                If txt���ʐU�֓��Q.Text.Trim <> "" OrElse txt���ʐU�֌��Q.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʐ�����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʐ������Q.Focus()
                    Return False
                End If
            End If
            If txt���ʍĐU�֌��Q.Text.Trim = "" Then
                If txt���ʍĐU�֓��Q.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʍĐU�֌�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʍĐU�֌��Q.Focus()
                    Return False
                End If
            Else
                If txt���ʍĐU�֓��Q.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʍĐU�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʍĐU�֓��Q.Focus()
                    Return False
                Else
                    '�U�֓��A�ĐU�����փ`�F�b�N
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '���U���ݒ�
                    If CInt(txt���ʐU�֌��Q.Text) >= 1 AndAlso CInt(txt���ʐU�֌��Q.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʐU�֌��Q.Text & txt���ʐU�֓��Q.Text
                    Else
                        FURI_DATE = txt�Ώ۔N�x.Text & txt���ʐU�֌��Q.Text & txt���ʐU�֓��Q.Text
                    End If
                    '�ĐU���ݒ�
                    If CInt(txt���ʍĐU�֌��Q.Text) >= 1 AndAlso CInt(txt���ʍĐU�֌��Q.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʍĐU�֌��Q.Text & txt���ʍĐU�֓��Q.Text
                    Else
                        If txt���ʐU�֌��Q.Text = "03" AndAlso txt���ʍĐU�֌��Q.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʍĐU�֌��Q.Text & txt���ʍĐU�֓��Q.Text
                        Else
                            SAIFURI_DATE = txt�Ώ۔N�x.Text & txt���ʍĐU�֌��Q.Text & txt���ʍĐU�֓��Q.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("�ĐU���ɂ͏��U���ȍ~�̐U�֓���ݒ肵�Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt���ʍĐU�֌��Q.Focus()
                        Return False
                    End If
                End If
            End If

            '���ʐ������R
            If txt���ʐ������R.Text.Trim <> "" Then
                If txt���ʐU�֌��R.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʐU�֌�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʐU�֌��R.Focus()
                    Return False
                Else
                    If txt���ʐU�֓��R.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "���ʐU�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt���ʐU�֓��R.Focus()
                        Return False
                    End If
                End If
            Else
                If txt���ʐU�֓��R.Text.Trim <> "" OrElse txt���ʐU�֌��R.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʐ�����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʐ������R.Focus()
                    Return False
                End If
            End If
            If txt���ʍĐU�֌��R.Text.Trim = "" Then
                If txt���ʍĐU�֓��R.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʍĐU�֌�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʍĐU�֌��R.Focus()
                    Return False
                End If
            Else
                If txt���ʍĐU�֓��R.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʍĐU�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʍĐU�֓��R.Focus()
                    Return False
                Else
                    '�U�֓��A�ĐU�����փ`�F�b�N
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '���U���ݒ�
                    If CInt(txt���ʐU�֌��R.Text) >= 1 AndAlso CInt(txt���ʐU�֌��R.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʐU�֌��R.Text & txt���ʐU�֓��R.Text
                    Else
                        FURI_DATE = txt�Ώ۔N�x.Text & txt���ʐU�֌��R.Text & txt���ʐU�֓��R.Text
                    End If
                    '�ĐU���ݒ�
                    If CInt(txt���ʍĐU�֌��R.Text) >= 1 AndAlso CInt(txt���ʍĐU�֌��R.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʍĐU�֌��R.Text & txt���ʍĐU�֓��R.Text
                    Else
                        If txt���ʐU�֌��R.Text = "03" AndAlso txt���ʍĐU�֌��R.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʍĐU�֌��R.Text & txt���ʍĐU�֓��R.Text
                        Else
                            SAIFURI_DATE = txt�Ώ۔N�x.Text & txt���ʍĐU�֌��R.Text & txt���ʍĐU�֓��R.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("�ĐU���ɂ͏��U���ȍ~�̐U�֓���ݒ肵�Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt���ʍĐU�֌��R.Focus()
                        Return False
                    End If
                End If
            End If

            '���ʐ������S
            If txt���ʐ������S.Text.Trim <> "" Then
                If txt���ʐU�֌��S.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʐU�֌�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʐU�֌��S.Focus()
                    Return False
                Else
                    If txt���ʐU�֓��S.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "���ʐU�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt���ʐU�֓��S.Focus()
                        Return False
                    End If
                End If
            Else
                If txt���ʐU�֓��S.Text.Trim <> "" OrElse txt���ʐU�֌��S.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʐ�����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʐ������S.Focus()
                    Return False
                End If
            End If
            If txt���ʍĐU�֌��S.Text.Trim = "" Then
                If txt���ʍĐU�֓��S.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʍĐU�֌�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʍĐU�֌��S.Focus()
                    Return False
                End If
            Else
                If txt���ʍĐU�֓��S.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʍĐU�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʍĐU�֓��S.Focus()
                    Return False
                Else
                    '�U�֓��A�ĐU�����փ`�F�b�N
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '���U���ݒ�
                    If CInt(txt���ʐU�֌��S.Text) >= 1 AndAlso CInt(txt���ʐU�֌��S.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʐU�֌��S.Text & txt���ʐU�֓��S.Text
                    Else
                        FURI_DATE = txt�Ώ۔N�x.Text & txt���ʐU�֌��S.Text & txt���ʐU�֓��S.Text
                    End If
                    '�ĐU���ݒ�
                    If CInt(txt���ʍĐU�֌��S.Text) >= 1 AndAlso CInt(txt���ʍĐU�֌��S.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʍĐU�֌��S.Text & txt���ʍĐU�֓��S.Text
                    Else
                        If txt���ʐU�֌��S.Text = "03" AndAlso txt���ʍĐU�֌��S.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʍĐU�֌��S.Text & txt���ʍĐU�֓��S.Text
                        Else
                            SAIFURI_DATE = txt�Ώ۔N�x.Text & txt���ʍĐU�֌��S.Text & txt���ʍĐU�֓��S.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("�ĐU���ɂ͏��U���ȍ~�̐U�֓���ݒ肵�Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt���ʍĐU�֌��S.Focus()
                        Return False
                    End If
                End If
            End If

            '���ʐ������T
            If txt���ʐ������T.Text.Trim <> "" Then
                If txt���ʐU�֌��T.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʐU�֌�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʐU�֌��T.Focus()
                    Return False
                Else
                    If txt���ʐU�֓��T.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "���ʐU�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt���ʐU�֓��T.Focus()
                        Return False
                    End If
                End If
            Else
                If txt���ʐU�֓��T.Text.Trim <> "" OrElse txt���ʐU�֌��T.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʐ�����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʐ������T.Focus()
                    Return False
                End If
            End If
            If txt���ʍĐU�֌��T.Text.Trim = "" Then
                If txt���ʍĐU�֓��T.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʍĐU�֌�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʍĐU�֌��T.Focus()
                    Return False
                End If
            Else
                If txt���ʍĐU�֓��T.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʍĐU�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʍĐU�֓��T.Focus()
                    Return False
                Else
                    '�U�֓��A�ĐU�����փ`�F�b�N
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '���U���ݒ�
                    If CInt(txt���ʐU�֌��T.Text) >= 1 AndAlso CInt(txt���ʐU�֌��T.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʐU�֌��T.Text & txt���ʐU�֓��T.Text
                    Else
                        FURI_DATE = txt�Ώ۔N�x.Text & txt���ʐU�֌��T.Text & txt���ʐU�֓��T.Text
                    End If
                    '�ĐU���ݒ�
                    If CInt(txt���ʍĐU�֌��T.Text) >= 1 AndAlso CInt(txt���ʍĐU�֌��T.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʍĐU�֌��T.Text & txt���ʍĐU�֓��T.Text
                    Else
                        If txt���ʐU�֌��T.Text = "03" AndAlso txt���ʍĐU�֌��T.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʍĐU�֌��T.Text & txt���ʍĐU�֓��T.Text
                        Else
                            SAIFURI_DATE = txt�Ώ۔N�x.Text & txt���ʍĐU�֌��T.Text & txt���ʍĐU�֓��T.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("�ĐU���ɂ͏��U���ȍ~�̐U�֓���ݒ肵�Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt���ʍĐU�֌��T.Focus()
                        Return False
                    End If
                End If
            End If

            '���ʐ������U
            If txt���ʐ������U.Text.Trim <> "" Then
                If txt���ʐU�֌��U.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʐU�֌�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʐU�֌��U.Focus()
                    Return False
                Else
                    If txt���ʐU�֓��U.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "���ʐU�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt���ʐU�֓��U.Focus()
                        Return False
                    End If
                End If
            Else
                If txt���ʐU�֓��U.Text.Trim <> "" OrElse txt���ʐU�֌��U.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʐ�����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʐ������U.Focus()
                    Return False
                End If
            End If
            If txt���ʍĐU�֌��U.Text.Trim = "" Then
                If txt���ʍĐU�֓��U.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʍĐU�֌�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʍĐU�֌��U.Focus()
                    Return False
                End If
            Else
                If txt���ʍĐU�֓��U.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���ʍĐU�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt���ʍĐU�֓��U.Focus()
                    Return False
                Else
                    '�U�֓��A�ĐU�����փ`�F�b�N
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '���U���ݒ�
                    If CInt(txt���ʐU�֌��U.Text) >= 1 AndAlso CInt(txt���ʐU�֌��U.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʐU�֌��U.Text & txt���ʐU�֓��U.Text
                    Else
                        FURI_DATE = txt�Ώ۔N�x.Text & txt���ʐU�֌��U.Text & txt���ʐU�֓��U.Text
                    End If
                    '�ĐU���ݒ�
                    If CInt(txt���ʍĐU�֌��U.Text) >= 1 AndAlso CInt(txt���ʍĐU�֌��U.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʍĐU�֌��U.Text & txt���ʍĐU�֓��U.Text
                    Else
                        If txt���ʐU�֌��U.Text = "03" AndAlso txt���ʍĐU�֌��U.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt�Ώ۔N�x.Text) + 1) & txt���ʍĐU�֌��U.Text & txt���ʍĐU�֓��U.Text
                        Else
                            SAIFURI_DATE = txt�Ώ۔N�x.Text & txt���ʍĐU�֌��U.Text & txt���ʍĐU�֓��U.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("�ĐU���ɂ͏��U���ȍ~�̐U�֓���ݒ肵�Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt���ʍĐU�֌��U.Focus()
                        Return False
                    End If
                End If
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.ToString)
            Return False
        End Try

        PFUNC_Nyuryoku_Check = True

    End Function
    '2011/06/16 �W���ŏC�� ���ʐU�֓����փ`�F�b�N�ǉ� ------------------END
#End Region

End Class
