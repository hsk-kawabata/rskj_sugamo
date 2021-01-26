' 2016/01/11 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text
Imports CAstBatch
Imports System.Configuration
Imports System.Xml
Imports System.Windows.Forms
Imports System.Drawing

Public Class KFCMAIN020

#Region " �萔/�ϐ� "

    '--------------------------------
    ' ���ʊ֘A����
    '--------------------------------
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private clsFUSION As New clsFUSION.clsMain()

    '--------------------------------
    ' LOG�֘A����
    '--------------------------------
    Private MainLOG As New CASTCommon.BatchLOG("KFCMAIN020", "�}�̏����i�f�B�X�N���}�́j���")
    Private Const msgTitle As String = "�}�̏����i�f�B�X�N���}�́j���(KFCMAIN020)"
    Private Structure LogWrite
        Dim UserID As String                         ' ���[�UID
        Dim ToriCode As String                       ' �����啛�R�[�h
        Dim FuriDate As String                       ' �U�֓�
    End Structure
    Private LW As LogWrite

    '--------------------------------
    ' Oracle�֘A����
    '--------------------------------
    Private MainDB As CASTCommon.MyOracle            ' �p�u���b�N�f�[�^�[�x�[�X

    '--------------------------------
    ' ���X�g�֘A����
    '--------------------------------
    Private ListViewArray As ArrayList               ' ���X�g�ҏW�pArrayList
    Private WriteKbn As String
    Private UketukeNoDisp As String
    Private UketukeNo As Integer

    '--------------------------------
    ' INI�֘A����
    '--------------------------------
    Friend Structure INI_INFO
        Dim COMMON_TXT As String                     ' TXT�t�H���_
        Dim COMMON_FDDRIVE As String                 ' FD�h���C�u
        Dim COMMON_BAITAI_1 As String                ' COMMON-BAITAI_1
        Dim COMMON_BAITAI_2 As String                ' COMMON-BAITAI_2
        Dim COMMON_BAITAI_3 As String                ' COMMON-BAITAI_3
        Dim COMMON_BAITAI_4 As String                ' COMMON-BAITAI_4
        Dim COMMON_BAITAI_5 As String                ' COMMON-BAITAI_5
        Dim COMMON_BAITAIWRITE As String             ' �}�̏����f�[�^�i�[�t�H���_
        Dim COMMON_DATBK As String                   ' �}�̏����f�[�^�ޔ��t�H���_
    End Structure
    Private IniInfo As INI_INFO

    '--------------------------------
    ' �����A�X�P�W���[������
    '--------------------------------
    Friend Structure TORI_INFO
        Dim TORIS_CODE As String                     ' ������R�[�h
        Dim TORIF_CODE As String                     ' ����敛�R�[�h
        Dim ITAKU_CODE As String                     ' �ϑ��҃R�[�h
        Dim ITAKU_KANRI_CODE As String               ' ��\�ϑ��҃R�[�h
        Dim BAITAI_CODE As String                    ' �}�̃R�[�h
        Dim CODE_KBN As String                       ' �R�[�h�敪
        Dim FMT_KBN As String                        ' �t�H�[�}�b�g�敪
        Dim MULTI_KBN As String                      ' �}���`�敪
        Dim FILE_NAME As String                      ' �t�@�C����
        Dim FURIDATE As String                       ' �U�֓�
        Dim RECORD_LENGTH As String                  ' ���R�[�h��
        Dim FTRANP As String                         ' �R�[�h�ϊ���`
        Dim FTRANP_IBM As String                     ' �R�[�h�ϊ���`�iIBM�t�H�[�}�b�g�j
        Dim SFURI_FLG As String                      ' �ĐU�t���O
        Dim HENKAN_FILENAME As String                ' �Ԋ҃f�[�^�t�@�C����
    End Structure
    Private ToriInfo As TORI_INFO

    Structure gstrFURI_DATA_118
        <VBFixedString(118)> Public strDATA As String
    End Structure
    Public gstrDATA_118 As gstrFURI_DATA_118

    Structure gstrFURI_DATA_119
        <VBFixedString(119)> Public strDATA As String
    End Structure
    Public gstrDATA_119 As gstrFURI_DATA_119

    Structure gstrFURI_DATA_120
        <VBFixedString(120)> Public strDATA As String
    End Structure
    Public gstrDATA_120 As gstrFURI_DATA_120

    Structure gstrFURI_DATA_130
        <VBFixedString(130)> Public strDATA As String
    End Structure
    Public gstrDATA_130 As gstrFURI_DATA_130

    Structure gstrFURI_DATA_220
        <VBFixedString(220)> Public strDATA As String
    End Structure
    Public gstrDATA_220 As gstrFURI_DATA_220

    Structure gstrFURI_DATA_390
        <VBFixedString(390)> Public strDATA As String
    End Structure
    Public gstrDATA_390 As gstrFURI_DATA_390

    '�S��
    Public gZENGIN_REC1 As CAstFormat.CFormatZengin.ZGRECORD1
    Public gZENGIN_REC2 As CAstFormat.CFormatZengin.ZGRECORD2
    Public gZENGIN_REC8 As CAstFormat.CFormatZengin.ZGRECORD8
    '����
    Public gKOKUZEI_REC1 As CAstFormat.CFormatKokuzei.KOKUZEI_RECORD1
    Public gKOKUZEI_REC2 As CAstFormat.CFormatKokuzei.KOKUZEI_RECORD2
    Public gKOKUZEI_REC3 As CAstFormat.CFormatKokuzei.KOKUZEI_RECORD3
    Public gKOKUZEI_REC8 As CAstFormat.CFormatKokuzei.KOKUZEI_RECORD8
    '�m�g�j
    Public gNHK_REC1 As CAstFormat.CFormatNHK.NHKRECORD1
    Public gNHK_REC2 As CAstFormat.CFormatNHK.NHKRECORD2
    Public gNHK_REC8 As CAstFormat.CFormatNHK.NHKRECORD8
    Public gNHK_REC9 As CAstFormat.CFormatNHK.NHKRECORD9

#End Region

#Region " ���[�h "

    Private Sub KFCMAIN020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '--------------------------------
            ' ���O���ݒ�
            '--------------------------------
            LW.UserID = GCom.GetUserID
            If SetLogInfo(True) = False Then
                Exit Try
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���[�h", "�J�n", "")

            '--------------------------------
            ' �x���}�X�^�捞
            '--------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���[�h", "���s", "�x�����擾")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            End If

            '--------------------------------
            ' �V�X�e�����t�ƃ��[�U����\��
            '--------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------
            ' �U�֓��ɑO�c�Ɠ���\��
            '------------------------------------
            Dim strSysDate As String
            Dim strGetdate As String = ""

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            bRet = GCom.CheckDateModule(strSysDate, strGetdate, 1, 1)
            txtFuriDateY.Text = strGetdate.Substring(0, 4)
            txtFuriDateM.Text = strGetdate.Substring(4, 2)
            txtFuriDateD.Text = strGetdate.Substring(6, 2)

            '--------------------------------
            ' �ϑ��Җ����X�g�{�b�N�X�ݒ�
            '--------------------------------
            Dim Jyoken As String = " AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')"
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            '--------------------------------
            ' INI���擾
            '--------------------------------
            If SetIniFIle() = False Then
                Exit Try
            End If

            '--------------------------------
            ' ���X�g�̏�����
            '--------------------------------
            Me.ListView1.Items.Clear()
            UketukeNo = 1
            UketukeNoDisp = ""
            lblItakuCode.Text = ""
            lblItakuName.Text = ""
            lblCodeKbn.Text = ""
            lblFileName.Text = ""
            lblBaitai.Text = ""

            '--------------------------------
            ' �R���{�{�b�N�X�ݒ�
            '--------------------------------
            Select Case GCom.SetComboBox(cmbKakikomiHouhou, "KFCMAIN020_�������@.TXT", True)
                Case 1  '�t�@�C���Ȃ�
                    MessageBox.Show(String.Format(MSG0025E, "�������@", "KFCMAIN020_�������@.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�R���{�{�b�N�X�ݒ�)", "���s", "�������@�ݒ�t�@�C���Ȃ��B�t�@�C��:KFCMAIN020_�������@.TXT")
                Case 2  '�ُ�
                    MessageBox.Show(String.Format(MSG0026E, "�������@"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�R���{�{�b�N�X�ݒ�)", "���s", "�R���{�{�b�N�X�ݒ莸�s �R���{�{�b�N�X��:�������@")
            End Select

            '--------------------------------
            ' �����(�\����)���N���A
            '--------------------------------
            If ClearToriInfo() = False Then
                Exit Try
            End If
            
            '--------------------------------
            ' �{�^���̏�����
            '--------------------------------
            Me.btnWrite.Enabled = True
            Me.btnReset.Enabled = True
            Me.btnEnd.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���[�h", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���[�h", "�I��", "")
        End Try

    End Sub

#End Region

#Region " �{�^�� "

    '================================
    ' �����J�n�{�^��
    '================================
    Private Sub btnWrite_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWrite.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����J�n", "�J�n", "")

            '--------------------------------
            ' ���O���ݒ�
            '--------------------------------
            If SetLogInfo(False) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����J�n", "���s", "���O���ݒ�")
                Exit Try
            End If

            '--------------------------------
            ' �����(�\����)���N���A
            '--------------------------------
            If ClearToriInfo() = False Then
                Exit Try
            End If

            '--------------------------------
            ' �����}�X�^�`�F�b�N
            '--------------------------------
            If CheckTorimast(txtTorisCode.Text, txtTorifCode.Text) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����J�n", "���s", "�����}�X�^�`�F�b�N")
                Exit Try
            End If

            '--------------------------------
            ' �X�P�W���[���}�X�^�`�F�b�N
            '--------------------------------
            Dim FuriDate As String = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            If CheckSchmast(txtTorisCode.Text, txtTorifCode.Text, FuriDate) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����J�n", "���s", "�X�P�W���[���}�X�^�`�F�b�N")
                Exit Try
            End If

            '--------------------------------
            ' �}�̏�������
            '--------------------------------
            If MessageBox.Show(String.Format(MSG0077I, "���͂��������", "�}�̏�������(" & cmbKakikomiHouhou.Text & ")"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����J�n", "���f", "���[�U�L�����Z��")
                Exit Try
            End If

            WriteKbn = ""
            Select Case cmbKakikomiHouhou.Text
                Case "�ʏ폑��"
                    WriteKbn = "1"
                Case "��������"
                    WriteKbn = "2"
            End Select
            ListViewArray = New ArrayList
            ListViewArray.Clear()

            ToriInfo.TORIS_CODE = txtTorisCode.Text
            ToriInfo.TORIF_CODE = txtTorifCode.Text
            ToriInfo.FURIDATE = FuriDate

            If BaitaiWrite() = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����J�n", "���s", "�}�̏���")
                Exit Try
            End If

            '--------------------------------
            ' ���X�g���ҏW
            '--------------------------------
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����J�n", "", "���X�g���ҏW �J�n")
            For i As Integer = 0 To ListViewArray.Count - 1 Step 1
                Dim vLstItem As New ListViewItem(ListViewArray(i).ToString.Split("/"), -1, Color.Black, Color.White, Nothing)
                ListView1.Items.AddRange({vLstItem})
            Next

            '--------------------------------
            ' �������b�Z�[�W�\��
            '--------------------------------
            MessageBox.Show(String.Format(MSG0078I, "�}�̏�������"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����J�n", "����", "")
            UketukeNo += 1

            '2016/02/08 �^�X�N�j��� RSV2�Ή� ADD ---------------------------------------- START
            UketukeNoDisp = ""
            '2016/02/08 �^�X�N�j��� RSV2�Ή� ADD ---------------------------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����J�n", "���s", ex.Message)
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����J�n", "�I��", "")
        End Try

    End Sub

    '================================
    ' ����{�^��
    '================================
    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "�J�n", "")

            '--------------------------------
            ' ���O���ݒ�
            '--------------------------------
            If SetLogInfo(False) = False Then
                Exit Try
            End If

            '--------------------------------
            ' ��ʏ�����
            '--------------------------------
            If ClearInfo() = False Then
                Exit Try
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "�I��", "")
        End Try

    End Sub

    '================================
    ' �I���{�^��
    '================================
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�N���[�Y", "�J�n", "")

            '--------------------------------
            ' ���O���ݒ�
            '--------------------------------
            If SetLogInfo(True) = False Then
                Exit Try
            End If

            Me.Close()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�N���[�Y", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�N���[�Y", "�I��", "")
        End Try

    End Sub

#End Region

#Region " �֐�(Function) "

    '================================
    ' ���O���ݒ�
    '================================
    Private Function SetLogInfo(ByVal Initialize As Boolean) As Boolean

        Try
            Select Case Initialize
                Case True
                    LW.ToriCode = "000000000000"
                    LW.FuriDate = "00000000"
                Case False
                    If txtTorisCode.Text.Trim = "" Or txtTorifCode.Text.Trim = "" Then
                        LW.ToriCode = "000000000000"
                    Else
                        LW.ToriCode = txtTorisCode.Text & txtTorifCode.Text
                    End If
                    LW.FuriDate = "00000000"
            End Select

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���O���ݒ�", "���s", ex.Message)
            Return False
        Finally
            ' NOP
        End Try

    End Function

    '================================
    ' INI���擾
    '================================
    Private Function SetIniFIle() As Boolean

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "�J�n", "")

            IniInfo.COMMON_TXT = CASTCommon.GetFSKJIni("COMMON", "TXT")
            If IniInfo.COMMON_TXT.ToUpper = "ERR" OrElse IniInfo.COMMON_TXT = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "TXT�t�H���_", "COMMON", "TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:TXT�t�H���_ ����:COMMON ����:TXT")
                Return False
            End If

            IniInfo.COMMON_FDDRIVE = CASTCommon.GetFSKJIni("COMMON", "FDDRIVE")
            If IniInfo.COMMON_FDDRIVE.ToUpper = "err" OrElse IniInfo.COMMON_FDDRIVE = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�e�c�h���C�u", "COMMON", "FDDRIVE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:�e�c�h���C�u ����:COMMON ����:FDDRIVE")
                Return False
            End If

            IniInfo.COMMON_BAITAI_1 = CASTCommon.GetFSKJIni("COMMON", "BAITAI_1")
            If IniInfo.COMMON_BAITAI_1.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "�t�H���_�i�}�̂P�j", "COMMON", "BAITAI_1"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:�t�H���_�i�}�̂P�j ����:COMMON ����:BAITAI_1")
                Return False
            End If

            IniInfo.COMMON_BAITAI_2 = CASTCommon.GetFSKJIni("COMMON", "BAITAI_2")
            If IniInfo.COMMON_BAITAI_2.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "�t�H���_�i�}�̂Q�j", "COMMON", "BAITAI_2"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:�t�H���_�i�}�̂Q�j ����:COMMON ����:BAITAI_2")
                Return False
            End If

            IniInfo.COMMON_BAITAI_3 = CASTCommon.GetFSKJIni("COMMON", "BAITAI_3")
            If IniInfo.COMMON_BAITAI_3.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "�t�H���_�i�}�̂R�j", "COMMON", "BAITAI_3"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:�t�H���_�i�}�̂R�j ����:COMMON ����:BAITAI_3")
                Return False
            End If

            IniInfo.COMMON_BAITAI_4 = CASTCommon.GetFSKJIni("COMMON", "BAITAI_4")
            If IniInfo.COMMON_BAITAI_4.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "�t�H���_�i�}�̂S�j", "COMMON", "BAITAI_4"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:�t�H���_�i�}�̂S�j ����:COMMON ����:BAITAI_4")
                Return False
            End If

            IniInfo.COMMON_BAITAI_5 = CASTCommon.GetFSKJIni("COMMON", "BAITAI_5")
            If IniInfo.COMMON_BAITAI_5.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "�t�H���_�i�}�̂T�j", "COMMON", "BAITAI_5"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:�t�H���_�i�}�̂T�j ����:COMMON ����:BAITAI_5")
                Return False
            End If

            IniInfo.COMMON_BAITAIWRITE = CASTCommon.GetFSKJIni("COMMON", "BAITAIWRITE")
            If IniInfo.COMMON_BAITAIWRITE.ToUpper = "ERR" OrElse IniInfo.COMMON_BAITAIWRITE = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�����f�[�^�i�[�t�H���_", "COMMON", "BAITAIWRITE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:�����f�[�^�i�[�t�H���_ ����:COMMON ����:BAITAIWRITE")
                Return False
            End If

            IniInfo.COMMON_DATBK = CASTCommon.GetFSKJIni("COMMON", "DATBK")
            If IniInfo.COMMON_DATBK.ToUpper = "ERR" OrElse IniInfo.COMMON_DATBK = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�����f�[�^�ޔ��t�H���_", "COMMON", "DATBK"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:�����f�[�^�ޔ��t�H���_ ����:COMMON ����:DATBK")
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "�I��", "")
        End Try
    End Function

    '================================
    ' �����}�X�^�`�F�b�N
    '================================
    Private Function CheckTorimast(ByVal TorisCode As String, ByVal TorifCode As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����}�X�^�`�F�b�N", "�J�n", "")

            SQL = New StringBuilder(128)
            SQL.Length = 0
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_T   = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_T   = '" & TorifCode & "'")
            SQL.Append(" AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����}�X�^�`�F�b�N", "���s", "�Y���Ȃ�")
                MessageBox.Show(String.Format(MSG0372W, "�����", "�}�̂̎戵"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Else
                '--------------------------------
                ' ���ʕԋp�v�ۃ`�F�b�N
                '--------------------------------
                Select Case GCom.NzStr(OraReader.Reader.Item("KEKKA_HENKYAKU_KBN_T"))
                    Case "1", "2"
                        ' NOP
                    Case Else
                        MessageBox.Show(String.Format(MSG0372W, "�����R�[�h", "�}�̕ԋp��"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                End Select

                '--------------------------------
                ' ����捀�ڎ擾
                '--------------------------------
                ToriInfo.BAITAI_CODE = GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T"))
                ToriInfo.CODE_KBN = GCom.NzStr(OraReader.Reader.Item("CODE_KBN_T"))
                ToriInfo.FILE_NAME = GCom.NzStr(OraReader.Reader.Item("FILE_NAME_T")).Trim
                ToriInfo.ITAKU_CODE = GCom.NzStr(OraReader.Reader.Item("ITAKU_CODE_T"))
                ToriInfo.ITAKU_KANRI_CODE = GCom.NzStr(OraReader.Reader.Item("ITAKU_KANRI_CODE_T"))
                ToriInfo.FMT_KBN = GCom.NzStr(OraReader.Reader.Item("FMT_KBN_T"))
                ToriInfo.SFURI_FLG = GCom.NzStr(OraReader.Reader.Item("SFURI_FLG_T"))
                ToriInfo.MULTI_KBN = GCom.NzStr(OraReader.Reader.Item("MULTI_KBN_T"))
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����}�X�^�`�F�b�N", "���s", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����}�X�^�`�F�b�N", "�I��", "")
        End Try

    End Function

    '================================
    ' �X�P�W���[���}�X�^�`�F�b�N
    '================================
    Private Function CheckSchmast(ByVal TorisCode As String, ByVal TorifCode As String, ByVal FuriDate As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�X�P�W���[���}�X�^�`�F�b�N", "�J�n", "")

            SQL = New StringBuilder(128)
            SQL.Length = 0
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST , SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_T   = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_T   = '" & TorifCode & "'")
            SQL.Append(" AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')")
            SQL.Append(" AND TORIS_CODE_T   = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T   = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_S    = '" & FuriDate & "'")
            SQL.Append(" AND TOUROKU_FLG_S <> '0'")
            SQL.Append(" AND HAISIN_FLG_S  <> '0'")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�X�P�W���[���}�X�^�`�F�b�N", "���s", "�Y���Ȃ�")
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Else
                '--------------------------------
                ' �i����ԃ`�F�b�N
                '--------------------------------
                If GCom.NzStr(OraReader.Reader.Item("FUNOU_FLG_S")) <> "1" Then
                    MessageBox.Show("���͂��ꂽ�X�P�W���[���́A" & MSG0322W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�X�P�W���[���}�X�^�`�F�b�N", "���s", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�X�P�W���[���}�X�^�`�F�b�N", "�I��", "")
        End Try

    End Function

    '================================
    ' �}�̏���
    '================================
    Private Function BaitaiWrite() As Boolean

        Dim ErrMessage As String

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏������C��", "�J�n", "")

            '--------------------------------
            ' �}�̏������C������(�}��1�{��)
            '--------------------------------
            ErrMessage = ""
            If BaitaiWrite_Main(ErrMessage) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏������C��", "���s", "���}�̏���")
                If ErrMessage <> "" Then
                    MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                Return False
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏������C��", "����", "���}�̏���")
            End If

            '--------------------------------
            ' �}�̏������C������(�}��2�{��)
            '--------------------------------
            If MessageBox.Show(MSG0061I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                ErrMessage = ""
                If BaitaiWrite_Main(ErrMessage) = False Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏������C��", "���s", "���}�̏���")
                    If ErrMessage <> "" Then
                        MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                    Return False
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏������C��", "����", "���}�̏���")
                End If
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏������C��", "����", "���}�̏����L�����Z��")
            End If

            '--------------------------------
            ' �}�̏������C������(�}�̌���)
            '--------------------------------
            MessageBox.Show(MSG0062I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            ErrMessage = ""
            If BaitaiWrite_Check(ErrMessage) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏������C��", "����", "�}�̌���")
                If ErrMessage <> "" Then
                    MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                Return False
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏������C��", "����", "�}�̌���")
            End If

            '--------------------------------
            ' �Ԋ҃f�[�^�ޔ�
            '--------------------------------
            If File.Exists(IniInfo.COMMON_DATBK & Path.GetFileName(ToriInfo.HENKAN_FILENAME)) = True Then
                File.Delete(IniInfo.COMMON_DATBK & Path.GetFileName(ToriInfo.HENKAN_FILENAME))
            End If
            File.Move(ToriInfo.HENKAN_FILENAME, IniInfo.COMMON_DATBK & Path.GetFileName(ToriInfo.HENKAN_FILENAME))

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏������C��", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏������C��", "�I��", "")
        End Try

    End Function

    Private Function BaitaiWrite_Main(ByRef ErrMessage As String) As Boolean

        Dim WORK_FILENAME As String = ""  ' �}�̓��t�@�C���i�`�F�b�N�p�j
        Dim OUT_FILENAME As String        ' �}�̓��t�@�C����
        Dim IN_FILENAME As String         ' �Ԋҏ����쐬�t�@�C��
        Dim intKEKKA As Integer

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "�J�n", "")

            Dim HenkanFMT As New CAstFormat.CFormat
            Dim para As New CAstBatch.CommData.stPARAMETER
            para.FSYORI_KBN = "1"
            para.FMT_KBN = ToriInfo.FMT_KBN
            para.CODE_KBN = ToriInfo.CODE_KBN
            HenkanFMT = CAstFormat.CFormat.GetFormat(para)

            If GetPFileInfo(ToriInfo.RECORD_LENGTH, ToriInfo.FTRANP, ToriInfo.FTRANP_IBM) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�R�[�h�ϊ����擾")
                ErrMessage = String.Format(MSG0371W, "�R�[�h�ϊ����̎擾����", "���O���m�F���Ă��������B")
                Return False
            End If

            WORK_FILENAME = IniInfo.COMMON_BAITAIWRITE & "W_BEF_" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & ".dat"
            If File.Exists(WORK_FILENAME) = True Then
                Kill(WORK_FILENAME)
            End If
            OUT_FILENAME = ToriInfo.FILE_NAME.Trim

            Select Case ToriInfo.MULTI_KBN
                Case "0"
                    IN_FILENAME = IniInfo.COMMON_BAITAIWRITE & "O" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & "_" & ToriInfo.FURIDATE & ".dat"
                Case "1"
                    IN_FILENAME = IniInfo.COMMON_BAITAIWRITE & "O" & ToriInfo.ITAKU_KANRI_CODE & "_" & ToriInfo.FURIDATE & ".dat"
                Case Else
                    IN_FILENAME = IniInfo.COMMON_BAITAIWRITE & "O" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & "_" & ToriInfo.FURIDATE & ".dat"
            End Select

            If File.Exists(IN_FILENAME) = False Then
                ErrMessage = String.Format(MSG0255W, "�I�������X�P�W���[���̕ԊґΏ�")
                Return False
            End If

            If ToriInfo.HENKAN_FILENAME = "" Then
                ToriInfo.HENKAN_FILENAME = IN_FILENAME
            End If

            Select Case ToriInfo.BAITAI_CODE
                Case "01"       'FD������
                    If WriteKbn = "1" Then
                        intKEKKA = clsFUSION.fn_FD_CPYTO_DISK(ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE, _
                                                              OUT_FILENAME, _
                                                              WORK_FILENAME, _
                                                              ToriInfo.RECORD_LENGTH, _
                                                              ToriInfo.CODE_KBN, _
                                                              ToriInfo.FTRANP, msgTitle)
                        Select Case intKEKKA
                            Case 0
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���� ", "�e�c�捞(�R�[�h�ϊ�)")
                            Case 100
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�e�c�捞(�R�[�h�ϊ�)")
                                ErrMessage = String.Format(MSG0371W, "�e�c�捞", "�R�[�h�ϊ�")
                                Return False
                            Case 200
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�e�c�捞(�R�[�h�敪�ُ�(JIS���s����))")
                                ErrMessage = String.Format(MSG0371W, "�e�c�捞", "�R�[�h�敪�ُ�(JIS���s����)")
                                Return False
                            Case 300
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�e�c�捞(�R�[�h�敪�ُ�(JIS���s�Ȃ�))")
                                ErrMessage = String.Format(MSG0371W, "�e�c�捞", "�R�[�h�敪�ُ�(JIS���s�Ȃ�)")
                                Return False
                            Case 400
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�e�c�捞(�o�̓t�@�C���쐬)���s")
                                ErrMessage = String.Format(MSG0371W, "�e�c�捞", "�o�̓t�@�C���쐬")
                                Return False
                        End Select

                        '------------------------------------
                        '�}�̓��̃t�@�C���̃`�F�b�N
                        '------------------------------------
                        If CheckBaitai(WORK_FILENAME, IN_FILENAME) = False Then
                            ErrMessage = String.Format(MSG0370W, "�}�̓��t�@�C���`�F�b�N")
                            Return False
                        End If
                    End If

                    '------------------------------------
                    '�}�̂ɏ�������
                    '------------------------------------
                    intKEKKA = clsFUSION.fn_DISK_CPYTO_FD(ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE, _
                                                          IN_FILENAME, _
                                                          OUT_FILENAME, _
                                                          ToriInfo.RECORD_LENGTH, _
                                                          ToriInfo.CODE_KBN, ToriInfo.FTRANP_IBM, False, msgTitle)

                    Select Case intKEKKA
                        Case 0
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���� ", "�e�c������")
                        Case 100
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�e�c�����ݎ��s(IBM�`��)")
                            ErrMessage = String.Format(MSG0371W, "�e�c����", "IBM�`��")
                            Return False
                        Case 200
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�e�c�����ݎ��s(DOS�`��)")
                            ErrMessage = String.Format(MSG0371W, "�e�c����", "DOS�`��")
                            Return False
                    End Select

                Case "11", "12", "13", "14", "15"       'DVD������
                    If WriteKbn = "1" Then
                        intKEKKA = clsFUSION.fn_DVD_CPYTO_DISK(ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE, _
                                               OUT_FILENAME, _
                                               WORK_FILENAME, _
                                               ToriInfo.RECORD_LENGTH, _
                                               ToriInfo.CODE_KBN, _
                                               ToriInfo.FTRANP, _
                                               msgTitle, _
                                               ToriInfo.BAITAI_CODE)

                        '���O�o�͓��e��ύX�i�c�u�c���O���}�́j
                        Select Case intKEKKA
                            Case 0
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���� ", "�O���}�̎捞(�R�[�h�ϊ�)")
                            Case 100
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�O���}�̎捞(�R�[�h�ϊ�)")
                                ErrMessage = String.Format(MSG0371W, "�O���}�̎捞", "�R�[�h�ϊ�")
                                Return False
                            Case 200
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�O���}�̎捞(�R�[�h�敪�ُ�(JIS���s����))")
                                ErrMessage = String.Format(MSG0371W, "�O���}�̎捞", "�R�[�h�敪�ُ�(JIS���s����)")
                                Return False
                            Case 300
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�O���}�̎捞(�R�[�h�敪�ُ�(JIS���s�Ȃ�))")
                                ErrMessage = String.Format(MSG0371W, "�O���}�̎捞", "�R�[�h�敪�ُ�(JIS���s�Ȃ�)")
                                Return False
                            Case 400
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�O���}�̎捞(�o�̓t�@�C���쐬)���s")
                                ErrMessage = String.Format(MSG0371W, "�O���}�̎捞", "�o�̓t�@�C���쐬")
                                Return False
                        End Select

                        '------------------------------------
                        '�}�̓��̃t�@�C���̃`�F�b�N
                        '------------------------------------
                        If CheckBaitai(WORK_FILENAME, IN_FILENAME) = False Then
                            ErrMessage = String.Format(MSG0370W, "�}�̓��t�@�C���`�F�b�N")
                            Return False
                        End If
                    End If

                    '------------------------------------
                    '�}�̂ɏ�������
                    '------------------------------------
                    intKEKKA = clsFUSION.fn_DISK_CPYTO_DVD(ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE, _
                                                           IN_FILENAME, _
                                                           OUT_FILENAME, _
                                                           ToriInfo.RECORD_LENGTH, _
                                                           ToriInfo.CODE_KBN, ToriInfo.FTRANP, False, msgTitle, _
                                                           ToriInfo.BAITAI_CODE)

                    Select Case intKEKKA
                        Case 0
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���� ", "�O���}�̏�����")
                        Case 100
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�O���}�̏����ݎ��s(EBCDIC)")
                            ErrMessage = String.Format(MSG0371W, "�}�̏���", "�����R�[�h(EBCDIC)")
                            Return False
                        Case 200
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�O���}�̏����ݎ��s(JIS)")
                            ErrMessage = String.Format(MSG0371W, "�}�̏���", "�����R�[�h(JIS)")
                            Return False
                    End Select
            End Select

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s", ex.Message)
            Return False
        Finally
            If Dir(WORK_FILENAME) <> "" Then
                Kill(WORK_FILENAME)
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "�I��", "")
        End Try

    End Function

    Private Function CheckBaitai(ByVal astrCHK_SEND_FILE As String, _
                                 ByVal astrIN_FILE_NAME As String) As Boolean

        Dim FileNo1_Open As String = Nothing
        Dim FileNo2_Open As String = Nothing
        Dim intFILE_NO_1 As Integer
        Dim intFILE_NO_2 As Integer
        Dim strFURI_DATA_1 As String = ""
        Dim strFURI_DATA_2 As String = ""

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃`�F�b�N", "�J�n", "")

            '------------------------------------
            ' �ĐU�̓`�F�b�N�ΏۊO
            '------------------------------------
            If ToriInfo.SFURI_FLG = "1" Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃`�F�b�N", "����", "�ĐU�X�P�W���[��")
                Return True
            End If

            '------------------------------------
            ' �t�@�C���P�I�[�v��
            '------------------------------------
            intFILE_NO_1 = FreeFile()
            FileOpen(intFILE_NO_1, astrCHK_SEND_FILE, OpenMode.Random, , , ToriInfo.RECORD_LENGTH)
            FileNo1_Open = "1"
            If Err.Number = 0 Then
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃`�F�b�N", "���s ", "�}�̃t�@�C���I�[�v�� �t�@�C�����F" & astrCHK_SEND_FILE & Err.Description)
                Return False
            End If

            '------------------------------------
            ' �t�@�C���Q�I�[�v��
            '------------------------------------
            intFILE_NO_2 = FreeFile()
            FileOpen(intFILE_NO_2, astrIN_FILE_NAME, OpenMode.Random, , , ToriInfo.RECORD_LENGTH)
            FileNo2_Open = "1"
            If Err.Number = 0 Then
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃`�F�b�N", "���s ", "�}�̃t�@�C���I�[�v�� �t�@�C�����F" & astrIN_FILE_NAME & Err.Description)
                Return False
            End If

            '------------------------------------
            ' �t�@�C���`�F�b�N
            '------------------------------------
            Dim sysValueType As System.ValueType
            Select Case ToriInfo.RECORD_LENGTH
                Case 120
                    sysValueType = DirectCast(gstrDATA_120, ValueType)
                    FileGet(intFILE_NO_1, sysValueType, 1)
                    gstrDATA_120 = DirectCast(sysValueType, gstrFURI_DATA_120)
                    strFURI_DATA_1 = gstrDATA_120.strDATA

                    sysValueType = DirectCast(gstrDATA_120, ValueType)
                    FileGet(intFILE_NO_2, sysValueType, 1)
                    gstrDATA_120 = DirectCast(sysValueType, gstrFURI_DATA_120)
                    strFURI_DATA_2 = gstrDATA_120.strDATA
            End Select

            If strFURI_DATA_1 <> strFURI_DATA_2 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃`�F�b�N", "���s ", "�}�̕s��v" & Err.Description)
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃`�F�b�N", ex.Message)
            Return False
        Finally
            If FileNo1_Open = "1" Then FileClose(intFILE_NO_1)
            If FileNo2_Open = "1" Then FileClose(intFILE_NO_2)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃`�F�b�N", "�I��", "")
        End Try

    End Function

    Private Function BaitaiWrite_Check(ByRef ErrMessage As String) As Boolean

        Dim WORK_FILENAME As String = ""
        Dim OUT_FILENAME As String
        Dim intKEKKA As Integer

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏����㌟��", "�J�n", "")

            WORK_FILENAME = IniInfo.COMMON_BAITAIWRITE & "W_AFT_" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & ".dat"
            If Dir(WORK_FILENAME) <> "" Then
                Kill(WORK_FILENAME)
            End If
            OUT_FILENAME = ToriInfo.FILE_NAME.Trim

            Select Case ToriInfo.BAITAI_CODE
                Case "01"
                    '------------------------------------
                    ' �}�́F�e�c
                    '------------------------------------
                    intKEKKA = clsFUSION.fn_FD_CPYTO_DISK(ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE, _
                                                          OUT_FILENAME, _
                                                          WORK_FILENAME, _
                                                          ToriInfo.RECORD_LENGTH, _
                                                          ToriInfo.CODE_KBN, _
                                                          ToriInfo.FTRANP, _
                                                          msgTitle)
                    Select Case intKEKKA
                        Case 0
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏����㌟��", "���� ", "�e�c�捞")
                        Case 100
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏����㌟��", "���s ", "�e�c�捞(�R�[�h�ϊ�)���s")
                            ErrMessage = String.Format(MSG0371W, "�}�̌���", "�e�c�捞(�R�[�h�ϊ�)")
                            Return False
                        Case 200
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏����㌟��", "���s ", "�e�c�捞(�R�[�h�敪�ُ�(JIS���s����))")
                            ErrMessage = String.Format(MSG0371W, "�}�̌���", "�e�c�捞(�R�[�h�敪�ُ�(JIS���s����))")
                            Return False
                        Case 300
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�}�̏����㌟��", "���s ", "�e�c�捞(�R�[�h�敪�ُ�(JIS���s�Ȃ�))")
                            ErrMessage = String.Format(MSG0371W, "�}�̌���", "�e�c�捞(�R�[�h�敪�ُ�(JIS���s�Ȃ�))")
                            Return False
                        Case 400
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�}�̏����㌟��", "���s ", "�e�c�捞(�o�̓t�@�C���쐬)���s")
                            ErrMessage = String.Format(MSG0371W, "�}�̌���", "�e�c�捞(�o�̓t�@�C���쐬)")
                            Return False
                    End Select

                    '------------------------------------
                    '�}�̓��̃t�@�C���̃`�F�b�N
                    '------------------------------------
                    '2016/02/05 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- START
                    'XML�t�H�[�}�b�g�l���R��
                    If IsNumeric(ToriInfo.FMT_KBN) = True Then
                        Dim nFmtKbn As Integer = CInt(ToriInfo.FMT_KBN)
                        If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                            If BaitaiWrite_DataCheck_XML(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                                ErrMessage = String.Format(MSG0371W, "�}�̌���", "�}�̓��t�@�C���`�F�b�N")
                                Return False
                            End If
                        Else
                            If BaitaiWrite_DataCheck(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                                ErrMessage = String.Format(MSG0371W, "�}�̌���", "�}�̓��t�@�C���`�F�b�N")
                                Return False
                            End If
                        End If
                    Else
                        If BaitaiWrite_DataCheck(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                            ErrMessage = String.Format(MSG0371W, "�}�̌���", "�}�̓��t�@�C���`�F�b�N")
                            Return False
                        End If
                    End If
                    'If BaitaiWrite_DataCheck(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                    '    ErrMessage = String.Format(MSG0371W, "�}�̌���", "�}�̓��t�@�C���`�F�b�N")
                    '    Return False
                    'End If
                    '2016/02/05 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- END

                Case "11", "12", "13", "14", "15"
                    '------------------------------------
                    ' �}�́F�O���}��
                    '------------------------------------
                    intKEKKA = clsFUSION.fn_DVD_CPYTO_DISK(ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE, _
                                                           OUT_FILENAME, _
                                                           WORK_FILENAME, _
                                                           ToriInfo.RECORD_LENGTH, _
                                                           ToriInfo.CODE_KBN, _
                                                           ToriInfo.FTRANP, _
                                                           msgTitle, _
                                                           ToriInfo.BAITAI_CODE)
                    Select Case intKEKKA
                        Case 0
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�}�̏����㌟��", "���� ", "�O���}�̎捞")
                        Case 100
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�}�̏����㌟��", "���s ", "�O���}�̎捞(�R�[�h�ϊ�)���s")
                            ErrMessage = String.Format(MSG0371W, "�}�̌���", "�O���}�̎捞(�R�[�h�ϊ�")
                            Return False
                        Case 200
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�}�̏����㌟��", "���s ", "�O���}�̎捞(�R�[�h�敪�ُ�(JIS���s����))")
                            ErrMessage = String.Format(MSG0371W, "�}�̌���", "�O���}�̎捞(�R�[�h�敪�ُ�(JIS���s����))")
                            Return False
                        Case 300
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�}�̏����㌟��", "���s ", "�O���}�̎捞(�R�[�h�敪�ُ�(JIS���s�Ȃ�))")
                            ErrMessage = String.Format(MSG0371W, "�}�̌���", "�O���}�̎捞(�R�[�h�敪�ُ�(JIS���s�Ȃ�))")
                            Return False
                        Case 400
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�}�̏����㌟��", "���s ", "�O���}�̎捞(�o�̓t�@�C���쐬)���s")
                            ErrMessage = String.Format(MSG0371W, "�}�̌���", "�O���}�̎捞(�o�̓t�@�C���쐬)")
                            Return False
                    End Select

                    '------------------------------------
                    '�}�̓��̃t�@�C���̃`�F�b�N
                    '------------------------------------
                    '2016/02/05 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- START
                    'XML�t�H�[�}�b�g�l���R��
                    If IsNumeric(ToriInfo.FMT_KBN) = True Then
                        Dim nFmtKbn As Integer = CInt(ToriInfo.FMT_KBN)
                        If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                            If BaitaiWrite_DataCheck_XML(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                                ErrMessage = String.Format(MSG0371W, "�}�̌���", "�}�̓��t�@�C���`�F�b�N")
                                Return False
                            End If
                        Else
                            If BaitaiWrite_DataCheck(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                                ErrMessage = String.Format(MSG0371W, "�}�̌���", "�}�̓��t�@�C���`�F�b�N")
                                Return False
                            End If
                        End If
                    Else
                        If BaitaiWrite_DataCheck(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                            ErrMessage = String.Format(MSG0371W, "�}�̌���", "�}�̓��t�@�C���`�F�b�N")
                            Return False
                        End If
                    End If
                    'If BaitaiWrite_DataCheck(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                    '    ErrMessage = String.Format(MSG0371W, "�}�̌���", "�}�̓��t�@�C���`�F�b�N")
                    '    Return False
                    'End If
                    '2016/02/05 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- END
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�}�̏����㌟��", "����", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�}�̏����㌟��", "���s", ex.Message)
            Return False
        Finally
            If Dir(WORK_FILENAME) <> "" Then
                Kill(WORK_FILENAME)
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�}�̏����㌟��", "�I��", "")
        End Try

    End Function

    Function BaitaiWrite_DataCheck(ByVal astrCHK_SEND_FILE As String, ByVal aintREC_LENGTH As Integer) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Dim intFILE_NO_1 As Integer
        Dim FileNo1_Open As String = Nothing

        Dim strFURI_DATA As String
        Dim dblRECORD_COUNT As Double
        Dim FileItakuCode As String = ""
        Dim FileItakuName As String = ""
        Dim FileSyubetu As String = ""
        Dim FileTorisCode As String = ""
        Dim FileTorifCode As String = ""

        Dim dblSCH_SYORI_KEN As Double
        Dim dblSCH_SYORI_KIN As Double
        Dim dblSCH_FURI_KEN As Double
        Dim dblSCH_FURI_KIN As Double
        Dim dblSCH_FUNOU_KEN As Double
        Dim dblSCH_FUNOU_KIN As Double

        Dim dblFILE_SYORI_KEN As Double
        Dim dblFILE_SYORI_KIN As Double
        Dim dblFILE_FURI_KEN As Double
        Dim dblFILE_FURI_KIN As Double
        Dim dblFILE_FUNOU_KEN As Double
        Dim dblFILE_FUNOU_KIN As Double

        '2016/02/08 �^�X�N�j��� RSV2�Ή� ADD ---------------------------------------- START
        Dim UketukeNoDispFlg As Boolean = True
        '2016/02/08 �^�X�N�j��� RSV2�Ή� ADD ---------------------------------------- END

        Try

            MainDB = New CASTCommon.MyOracle
            UketukeNoDisp = ""
            dblRECORD_COUNT = 1
            intFILE_NO_1 = FreeFile()
            FileOpen(intFILE_NO_1, astrCHK_SEND_FILE, OpenMode.Random, , , aintREC_LENGTH)
            FileNo1_Open = "1"
            If Err.Number = 0 Then
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "�������݃t�@�C���I�[�v�� �t�@�C�����F" & astrCHK_SEND_FILE & Err.Description)
                Return False
            End If

            Dim sysValueType As ValueType
            Do Until EOF(intFILE_NO_1)
                strFURI_DATA = ""
                Select Case aintREC_LENGTH
                    Case 118
                        sysValueType = DirectCast(gstrDATA_118, ValueType)
                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        gstrDATA_118 = DirectCast(sysValueType, gstrFURI_DATA_118)
                        strFURI_DATA = gstrDATA_118.strDATA
                    Case 119
                        sysValueType = DirectCast(gstrDATA_119, ValueType)
                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        gstrDATA_119 = DirectCast(sysValueType, gstrFURI_DATA_119)
                        strFURI_DATA = gstrDATA_119.strDATA
                    Case 120
                        sysValueType = DirectCast(gstrDATA_120, ValueType)
                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        gstrDATA_120 = DirectCast(sysValueType, gstrFURI_DATA_120)
                        strFURI_DATA = gstrDATA_120.strDATA
                    Case 390
                        sysValueType = DirectCast(gstrDATA_390, ValueType)
                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        gstrDATA_390 = DirectCast(sysValueType, gstrFURI_DATA_390)
                        strFURI_DATA = gstrDATA_390.strDATA
                End Select

                If strFURI_DATA = "" Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "�t�@�C���ǂݍ��� ���R�[�hNO�F" & dblRECORD_COUNT)
                    Return False
                End If

                Select Case strFURI_DATA.Substring(0, 1)
                    Case "1"
                        '2016/02/08 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- START
                        If UketukeNoDisp = "" And UketukeNoDispFlg = True Then
                            UketukeNoDisp = UketukeNo
                            UketukeNoDispFlg = False
                        Else
                            UketukeNoDisp = ""
                        End If
                        'If UketukeNoDisp = "" Then
                        '    UketukeNoDisp = UketukeNo
                        'Else
                        '    UketukeNoDisp = ""
                        'End If
                        '2016/02/08 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- END

                        '------------------------------------
                        ' �t�@�C�����ϑ��҃R�[�h���擾
                        '------------------------------------
                        Select Case aintREC_LENGTH
                            Case 118, 119, 120
                                Select Case ToriInfo.FMT_KBN
                                    Case "00"       '�S��
                                        sysValueType = DirectCast(gZENGIN_REC1, ValueType)
                                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                        gZENGIN_REC1 = DirectCast(sysValueType, CAstFormat.CFormatZengin.ZGRECORD1)
                                        FileItakuCode = gZENGIN_REC1.ZG4
                                        FileSyubetu = gZENGIN_REC1.ZG2
                                    Case "01"       '�m�g�j
                                        sysValueType = DirectCast(gNHK_REC1, ValueType)
                                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                        gNHK_REC1 = DirectCast(sysValueType, CAstFormat.CFormatNHK.NHKRECORD1)
                                        FileItakuCode = gNHK_REC1.NH04
                                        FileSyubetu = gNHK_REC1.NH02
                                End Select
                            Case 390
                                sysValueType = DirectCast(gKOKUZEI_REC1, ValueType)
                                FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                gKOKUZEI_REC1 = DirectCast(sysValueType, CAstFormat.CFormatKokuzei.KOKUZEI_RECORD1)
                                FileItakuCode = ""
                                FileSyubetu = ""
                            Case Else
                                FileItakuCode = ""
                                FileSyubetu = ""
                        End Select

                        '--------------------------------------------------------------
                        '�����R�[�h�����i�}���`�̏ꍇ���l������j�A�ϑ��҃R�[�h�̃`�F�b�N
                        '--------------------------------------------------------------
                        SQL = New StringBuilder(128)
                        SQL.Length = 0
                        If FileItakuCode = "" Then
                            Return False
                        Else
                            SQL.Append("SELECT")
                            SQL.Append("     *")
                            SQL.Append(" FROM")
                            SQL.Append("     TORIMAST")
                            SQL.Append(" WHERE")
                            SQL.Append("     ITAKU_CODE_T = '" & Trim(FileItakuCode) & "'")
                        End If
                        OraReader = New CASTCommon.MyOracleReader(MainDB)
                        If OraReader.DataReader(SQL) = False Then            '�����R�[�h�����݂��Ȃ�
                            Return False
                        Else
                            FileItakuName = OraReader.GetString("ITAKU_KNAME_T").Trim
                            FileTorisCode = OraReader.GetString("TORIS_CODE_T")
                            FileTorifCode = OraReader.GetString("TORIF_CODE_T")
                            If FileSyubetu = "" Then
                                FileSyubetu = OraReader.GetString("SYUBETU_T")
                            End If
                            If Not OraReader Is Nothing Then
                                OraReader.Close()
                                OraReader = Nothing
                            End If
                        End If

                        '------------------------------------
                        '�X�P�W���[���̌���
                        '------------------------------------
                        If ToriInfo.FMT_KBN <> "02" Then
                            '------------------------------------
                            ' ���ňȊO�̏ꍇ
                            '------------------------------------
                            SQL.Length = 0
                            SQL.Append("SELECT")
                            SQL.Append("     *")
                            SQL.Append(" FROM")
                            SQL.Append("     SCHMAST")
                            SQL.Append(" WHERE")
                            SQL.Append("     ITAKU_CODE_S = " & SQ(FileItakuCode))
                            SQL.Append(" AND FURI_DATE_S  = " & SQ(ToriInfo.FURIDATE))
                            SQL.Append(" AND FUNOU_FLG_S  = '1'")
                            If ToriInfo.MULTI_KBN = "0" Then  '�V���O��
                                SQL.Append(" AND TORIS_CODE_S  = " & SQ(ToriInfo.TORIS_CODE))
                                SQL.Append(" AND TORIF_CODE_S  = " & SQ(ToriInfo.TORIF_CODE))
                            End If
                        Else
                            '------------------------------------
                            ' ���ł̏ꍇ(�ϑ��҃R�[�h���܂߂Ȃ�)
                            '------------------------------------
                            SQL.Length = 0
                            SQL.Append("SELECT")
                            SQL.Append("     *")
                            SQL.Append(" FROM")
                            SQL.Append("     SCHMAST")
                            SQL.Append(" WHERE")
                            SQL.Append("     FURI_DATE_S   = " & SQ(ToriInfo.FURIDATE))
                            SQL.Append(" AND TORIS_CODE_S  = " & SQ(ToriInfo.TORIS_CODE))
                            SQL.Append(" AND TORIF_CODE_S  = " & SQ(ToriInfo.TORIF_CODE))
                        End If
                        OraReader = New CASTCommon.MyOracleReader(MainDB)
                        If OraReader.DataReader(SQL) = False Then            '�X�P�W���[�������݂��Ȃ�
                            Return False
                        Else
                            dblSCH_SYORI_KEN = OraReader.GetInt64("SYORI_KEN_S")
                            dblSCH_SYORI_KIN = OraReader.GetInt64("SYORI_KIN_S")
                            dblSCH_FURI_KEN = OraReader.GetInt64("FURI_KEN_S")
                            dblSCH_FURI_KIN = OraReader.GetInt64("FURI_KIN_S")
                            dblSCH_FUNOU_KEN = OraReader.GetInt64("FUNOU_KEN_S")
                            dblSCH_FUNOU_KIN = OraReader.GetInt64("FUNOU_KIN_S")

                            If Not OraReader Is Nothing Then
                                OraReader.Close()
                                OraReader = Nothing
                            End If
                        End If
                    Case "2", "3", "4", "5"
                        ' NOP
                    Case "8"
                        Select Case aintREC_LENGTH
                            Case 118, 119, 120
                                Select Case ToriInfo.FMT_KBN
                                    Case "00"
                                        sysValueType = DirectCast(gZENGIN_REC8, ValueType)
                                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                        gZENGIN_REC8 = DirectCast(sysValueType, CAstFormat.CFormatZengin.ZGRECORD8)
                                        dblFILE_SYORI_KEN = Val(gZENGIN_REC8.ZG2)
                                        dblFILE_SYORI_KIN = Val(gZENGIN_REC8.ZG3)
                                        dblFILE_FURI_KEN = Val(gZENGIN_REC8.ZG4.Substring(0, 6))
                                        dblFILE_FURI_KIN = Val(gZENGIN_REC8.ZG5.Substring(0, 12))
                                        dblFILE_FUNOU_KEN = Val(gZENGIN_REC8.ZG6.Substring(0, 6))
                                        dblFILE_FUNOU_KIN = Val(gZENGIN_REC8.ZG7.Substring(0, 12))
                                    Case "01"
                                        sysValueType = DirectCast(gNHK_REC8, ValueType)
                                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                        gNHK_REC8 = DirectCast(sysValueType, CAstFormat.CFormatNHK.NHKRECORD8)
                                        dblFILE_SYORI_KEN = Val(gNHK_REC8.NH02)
                                        dblFILE_SYORI_KIN = Val(gNHK_REC8.NH03)
                                        dblFILE_FURI_KEN = Val(gNHK_REC8.NH04.Substring(0, 6))
                                        dblFILE_FURI_KIN = Val(gNHK_REC8.NH05.Substring(0, 12))
                                        dblFILE_FUNOU_KEN = Val(gNHK_REC8.NH06.Substring(0, 6))
                                        dblFILE_FUNOU_KIN = Val(gNHK_REC8.NH07.Substring(0, 12))
                                End Select
                            Case 390
                                sysValueType = DirectCast(gKOKUZEI_REC8, ValueType)
                                FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                gKOKUZEI_REC8 = DirectCast(sysValueType, CAstFormat.CFormatKokuzei.KOKUZEI_RECORD8)
                                dblFILE_SYORI_KEN = Val(gKOKUZEI_REC8.KZ7.Trim)
                                dblFILE_SYORI_KIN = Val(gKOKUZEI_REC8.KZ8.Trim)
                                dblFILE_FURI_KEN = Val(gKOKUZEI_REC8.KZ11.Trim)
                                dblFILE_FURI_KIN = Val(gKOKUZEI_REC8.KZ12.Trim)
                                dblFILE_FUNOU_KEN = Val(gKOKUZEI_REC8.KZ9.Trim)
                                dblFILE_FUNOU_KIN = Val(gKOKUZEI_REC8.KZ10.Trim)
                        End Select

                        If dblSCH_SYORI_KEN <> dblFILE_SYORI_KEN Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "�g���[���`�F�b�N ���������s��v�A���R�[�hNO�F" & dblRECORD_COUNT & Err.Description)
                            Return False
                        End If

                        If dblSCH_SYORI_KIN <> dblFILE_SYORI_KIN Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "�g���[���`�F�b�N �������z�s��v�A���R�[�hNO�F" & dblRECORD_COUNT & Err.Description)
                            Return False
                        End If

                        If dblSCH_FUNOU_KEN <> dblFILE_FUNOU_KEN Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "�g���[���`�F�b�N �s�\�����s��v�A���R�[�hNO�F" & dblRECORD_COUNT & Err.Description)
                            Return False
                        End If

                        If dblSCH_FUNOU_KIN <> dblFILE_FUNOU_KIN Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "�g���[���`�F�b�N �s�\���z�s��v�A���R�[�hNO�F" & dblRECORD_COUNT & Err.Description)
                            Return False
                        End If

                        Dim ListViewData As String = UketukeNoDisp & "/" & _
                                                     FileItakuCode & "/" & _
                                                     FileItakuName & "/" & _
                                                     ToriInfo.FURIDATE & "/" & _
                                                     FileSyubetu & "/" & _
                                                     Format(CInt(dblFILE_SYORI_KEN), "###,##0") & "/" & _
                                                     Format(CLng(dblFILE_SYORI_KIN), "###,###,###,##0") & "/" & _
                                                     Format(CInt(dblFILE_FURI_KEN), "###,##0") & "/" & _
                                                     Format(CLng(dblFILE_FURI_KIN), "###,###,###,##0") & "/" & _
                                                     Format(CInt(dblFILE_FUNOU_KEN), "###,##0") & "/" & _
                                                     Format(CLng(dblFILE_FUNOU_KIN), "###,###,###,##0") & "/" & _
                                                     FileTorisCode & "-" & FileTorifCode
                        ListViewArray.Add(ListViewData)
                    Case "9"
                        ' NOP
                End Select

                dblRECORD_COUNT += 1
            Loop

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", ex.Message)
            Return False
        Finally
            If FileNo1_Open = "1" Then
                FileClose(intFILE_NO_1)
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "�I��", "")
        End Try

    End Function

    '================================
    ' �ϑ��҃R�[�h�擾
    '================================
    Private Function GetItakuInfo(ByVal TorisCode As String, ByVal TorifCode As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��ҏ��擾", "�J�n", "")

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_T = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & TorifCode & "'")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                lblItakuCode.Text = GCom.NzStr(OraReader.Reader.Item("ITAKU_CODE_T"))
                lblItakuName.Text = GCom.NzStr(OraReader.Reader.Item("ITAKU_NNAME_T"))
                lblCodeKbn.Text = GetTextFileInfo(IniInfo.COMMON_TXT & "Common_�R�[�h�敪.TXT", GCom.NzStr(OraReader.Reader.Item("CODE_KBN_T")))
                lblFileName.Text = GCom.NzStr(OraReader.Reader.Item("FILE_NAME_T"))
                lblBaitai.Text = GetTextFileInfo(IniInfo.COMMON_TXT & "Common_�}�̃R�[�h.TXT", GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T")))
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��ҏ��擾", "����", "")
            Else
                lblItakuCode.Text = ""
                lblItakuName.Text = ""
                lblCodeKbn.Text = ""
                lblFileName.Text = ""
                lblBaitai.Text = ""
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��ҏ��擾", "����", "�Y���Ȃ�")
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��ҏ��擾", "���s", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��ҏ��擾", "�I��", "")
        End Try
    End Function

    '================================
    ' ��ʏ�����
    '================================
    Private Function ClearInfo() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��ʏ�����", "�J�n", "")

            '--------------------------------
            ' �R���{�{�b�N�X������
            '--------------------------------
            cmbKakikomiHouhou.SelectedIndex = 0
            cmbKana.SelectedItem = ""
            Dim Jyoken As String = " AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')"
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            '--------------------------------
            ' �e�L�X�g�{�b�N�X������
            '--------------------------------
            txtTorisCode.Text = ""
            txtTorifCode.Text = ""
            ' 2017/03/23 �^�X�N�j���� CHG �yME�z(RSV2�Ή�) -------------------- START
            'txtFuriDateY.Text = ""
            'txtFuriDateM.Text = ""
            'txtFuriDateD.Text = ""
            '------------------------------------
            ' �U�֓��ɑO�c�Ɠ���\��
            '------------------------------------
            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            Dim strGetdate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            Dim bRet As Boolean = GCom.CheckDateModule(strSysDate, strGetdate, 1, 1)
            txtFuriDateY.Text = strGetdate.Substring(0, 4)
            txtFuriDateM.Text = strGetdate.Substring(4, 2)
            txtFuriDateD.Text = strGetdate.Substring(6, 2)
            ' 2017/03/23 �^�X�N�j���� CHG �yME�z(RSV2�Ή�) -------------------- END

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��ʏ�����", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��ʏ�����", "�I��", "")
        End Try

    End Function

    '================================
    ' �����(�\����)���N���A
    '================================
    Private Function ClearToriInfo() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����(�\����)���N���A", "�J�n", "")

            '--------------------------------
            ' �\���̏�����
            '--------------------------------
            ToriInfo.TORIS_CODE = ""
            ToriInfo.TORIF_CODE = ""
            ToriInfo.ITAKU_CODE = ""
            ToriInfo.ITAKU_KANRI_CODE = ""
            ToriInfo.BAITAI_CODE = ""
            ToriInfo.CODE_KBN = ""
            ToriInfo.FMT_KBN = ""
            ToriInfo.MULTI_KBN = ""
            ToriInfo.FILE_NAME = ""
            ToriInfo.FURIDATE = ""
            ToriInfo.RECORD_LENGTH = ""
            ToriInfo.FTRANP = ""
            ToriInfo.FTRANP_IBM = ""
            ToriInfo.SFURI_FLG = ""
            ToriInfo.HENKAN_FILENAME = ""

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����(�\����)���N���A", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����(�\����)���N���A", "�I��", "")
        End Try

    End Function

    '================================
    ' �e�L�X�g�t�@�C�����擾
    '================================
    Private Function GetTextFileInfo(ByVal TextFileName As String, ByVal KeyInfo As String) As String

        Dim sr As StreamReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�e�L�X�g�t�@�C���Ǎ�", "�J�n", TextFileName)

            sr = New StreamReader(TextFileName, Encoding.GetEncoding("SHIFT-JIS"))
            While sr.Peek > -1
                Dim strLineData() As String = sr.ReadLine().Split(","c)
                If strLineData(0) = KeyInfo Then
                    Return strLineData(1).Trim
                End If
            End While

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�e�L�X�g�t�@�C���Ǎ�", "", "�Y���Ȃ�")
            Return "NON"

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�e�L�X�g�t�@�C���Ǎ�", "���s", ex.Message)
            Return "ERR"
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�e�L�X�g�t�@�C���Ǎ�", "�I��", "")
        End Try

    End Function

    '================================
    ' �R�[�h�ϊ����擾
    '================================
    Private Function GetPFileInfo(ByRef Length As String, ByRef FtranP As String, ByRef FtranPIBM As String) As String

        Dim sr As StreamReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�R�[�h�ϊ����擾", "�J�n", "")

            Length = ""
            FtranP = ""
            FtranPIBM = ""

            Select Case ToriInfo.FMT_KBN
                Case "00"
                    Select Case ToriInfo.CODE_KBN
                        Case "0"
                            Length = "120"
                            FtranP = "120JIS��JIS.P"        'JIS��JIS��
                            FtranPIBM = "120JIS��JIS.P"
                        Case "1"
                            Length = "120"
                            FtranP = "120JIS��JIS��.P"      'JIS��JIS��
                            FtranPIBM = "120JIS��JIS��.P"
                        Case "2"
                            Length = "119"
                            FtranP = "119JIS��JIS��.P"
                            FtranPIBM = "119JIS��JIS��.P"
                        Case "3"
                            Length = "118"
                            FtranP = "118JIS��JIS��.P"
                            FtranPIBM = "118JIS��JIS��.P"
                        Case "4"
                            Length = "120"
                            FtranP = "120.P"                'JIS��EBCDIC
                            FtranPIBM = "120IBM.P"
                    End Select
                Case "01"   'NHK
                    Select Case ToriInfo.CODE_KBN
                        Case "0"
                            Length = "120"
                            FtranP = "120JIS��JIS.P"        'JIS��JIS��
                            FtranPIBM = "120JIS��JIS.P"
                        Case "1"
                            Length = "120"
                            FtranP = "120JIS��JIS��.P"      'JIS��JIS��
                            FtranPIBM = "120JIS��JIS��.P"
                        Case "4"
                            Length = "120"
                            FtranP = "120.P"                'JIS��EBCDIC
                            FtranPIBM = "120IBM.P"
                    End Select
                Case "02"  '�n����
                    Select Case ToriInfo.CODE_KBN
                        Case "0"
                            Length = "390"
                            FtranP = "390JIS��JIS.P"        'JIS��JIS��
                            FtranPIBM = "390JIS��JIS.P"
                        Case "1"
                            Length = "390"
                            FtranP = "390JIS��JIS��.P"      'JIS��JIS��
                            FtranPIBM = "390JIS��JIS��.P"
                        Case "4"
                            Length = "390"
                            FtranP = "390.P"                'JIS��EBCDIC
                            FtranPIBM = "390IBM.P"
                    End Select
                Case Else
                    If IsNumeric(ToriInfo.FMT_KBN) Then
                        Dim nFmtKbn As Integer = CInt(ToriInfo.FMT_KBN)
                        '�t�H�[�}�b�g�敪��50�`99�̏ꍇ
                        If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                            ' XML�t�H�[�}�b�g��root�I�u�W�F�N�g����
                            Dim xmlDoc As New ConfigXmlDocument
                            Dim mXmlRoot As XmlElement
                            Dim node As XmlNode
                            Dim attribute As XmlAttribute
                            Dim sWork As String

                            'XML�p�X�쐬
                            Dim xmlFolderPath As String = CASTCommon.GetFSKJIni("COMMON", "XML_FORMAT_FLD")
                            If xmlFolderPath = "err" Or xmlFolderPath = "" Then
                                Throw New Exception("fskj.ini��XML_FORMAT_FLD����`����Ă��܂���B")
                            End If
                            If xmlFolderPath.EndsWith("\") = False Then
                                xmlFolderPath &= "\"
                            End If
                            Dim mXmlFile As String = "XML_FORMAT_" & ToriInfo.FMT_KBN & ".xml"

                            xmlDoc.Load(xmlFolderPath & mXmlFile)
                            mXmlRoot = xmlDoc.DocumentElement

                            ' �Ԋ҃t�@�C����`���t�@�C���ɃR�s�[����ۂ̃��R�[�h��
                            node = mXmlRoot.SelectSingleNode("�Ԋ�/�R�s�[�ݒ�ꗗ/�R�s�[�ݒ�[@�R�[�h�敪='" & ToriInfo.CODE_KBN & "']")
                            If node Is Nothing Then
                                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�R�s�[�ݒ�ꗗ/�R�s�[�ݒ�[@�R�[�h�敪='" & ToriInfo.CODE_KBN & "']�v�^�O����`����Ă��܂���B")
                            End If

                            attribute = node.Attributes.ItemOf("�f�[�^��")
                            If attribute Is Nothing Then
                                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�R�s�[�ݒ�ꗗ/�R�s�[�ݒ�v�^�O�́u�f�[�^���v��������`����Ă��܂���B�i" & _
                                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                            End If
                            sWork = attribute.Value.Trim
                            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
                                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�R�s�[�ݒ�ꗗ/�R�s�[�ݒ�v�^�O�́u�f�[�^���v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                            End If
                            Length = sWork

                            ' �Ԋ҃t�@�C����`���t�@�C���ɃR�s�[����ۂ̃p�����[�^�t�@�C����
                            attribute = node.Attributes.ItemOf("�p�����[�^�t�@�C��")
                            If attribute Is Nothing OrElse attribute.Value.Trim = "" Then
                                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�R�s�[�ݒ�ꗗ/�R�s�[�ݒ�v�^�O�́u�p�����[�^�t�@�C���v��������`����Ă��܂���B�i" & _
                                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                            End If
                            FtranP = attribute.Value.Trim

                            ' �Ԋ҃t�@�C�����e�c�R�D�T�ɃR�s�[����ۂ̃p�����[�^�t�@�C����
                            attribute = node.Attributes.ItemOf("IBM�p�����[�^�t�@�C��")
                            If attribute Is Nothing OrElse attribute.Value.Trim = "" Then
                                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�R�s�[�ݒ�ꗗ/�R�s�[�ݒ�v�^�O�́uIBM�p�����[�^�t�@�C���v��������`����Ă��܂���B�i" & _
                                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                            End If
                            FtranPIBM = attribute.Value.Trim
                        End If
                    End If
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�R�[�h�ϊ����擾", "����", _
                                 "Length" & Length & " / " & _
                                 "FtranP" & FtranP & " / " & _
                                 "FtranPIBM" & FtranPIBM)
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�R�[�h�ϊ����擾", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�R�[�h�ϊ����擾", "�I��", "")
        End Try

    End Function

    '2016/02/05 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- START
    Function BaitaiWrite_DataCheck_XML(ByVal astrCHK_SEND_FILE As String, ByVal aintREC_LENGTH As Integer) As Boolean
        Dim HenkanFMT As New CAstFormat.CFormat
        Dim sRet As String = ""

        Dim SQL As StringBuilder = Nothing
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim FileItakuCode As String = ""
        Dim FileItakuName As String = ""
        Dim FileSyubetu As String = ""
        Dim FileTorisCode As String = ""
        Dim FileTorifCode As String = ""

        Dim dblSCH_SYORI_KEN As Double
        Dim dblSCH_SYORI_KIN As Double
        Dim dblSCH_FURI_KEN As Double
        Dim dblSCH_FURI_KIN As Double
        Dim dblSCH_FUNOU_KEN As Double
        Dim dblSCH_FUNOU_KIN As Double
        Dim dblFILE_SYORI_KEN As Double
        Dim dblFILE_SYORI_KIN As Double
        Dim dblFILE_FURI_KEN As Double
        Dim dblFILE_FURI_KIN As Double
        Dim dblFILE_FUNOU_KEN As Double
        Dim dblFILE_FUNOU_KIN As Double
        Dim dblRECORD_COUNT As Double

        '2016/02/08 �^�X�N�j��� RSV2�Ή� ADD ---------------------------------------- START
        Dim UketukeNoDispFlg As Boolean = True
        '2016/02/08 �^�X�N�j��� RSV2�Ή� ADD ---------------------------------------- END

        Try
            MainDB = New CASTCommon.MyOracle
            Dim sw As System.Diagnostics.Stopwatch = Nothing

            dblRECORD_COUNT = 0

            Dim para As New CAstBatch.CommData.stPARAMETER
            para.FSYORI_KBN = "1"
            para.FMT_KBN = ToriInfo.FMT_KBN
            HenkanFMT = CAstFormat.CFormat.GetFormat(para)
            HenkanFMT.LOG = MainLOG

            If HenkanFMT.FirstRead(astrCHK_SEND_FILE) = 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "�t�@�C���Ǎ� �Ώۃt�@�C��:" & astrCHK_SEND_FILE)
                Return False
            End If

            Do Until HenkanFMT.EOF()
                dblRECORD_COUNT += 1
                sRet = HenkanFMT.CheckDataFormat()

                If HenkanFMT.IsHeaderRecord Then
                    '=========================================
                    ' �w�b�_���R�[�h
                    '=========================================
                    '2016/02/08 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- START
                    If UketukeNoDisp = "" And UketukeNoDispFlg = True Then
                        UketukeNoDisp = UketukeNo
                        UketukeNoDispFlg = False
                    Else
                        UketukeNoDisp = ""
                    End If
                    'If UketukeNoDisp = "" Then
                    '    UketukeNoDisp = UketukeNo
                    'Else
                    '    UketukeNoDisp = ""
                    'End If
                    '2016/02/08 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- END
                    '-----------------------------------------
                    ' �����R�[�h����
                    '-----------------------------------------
                    SQL = New StringBuilder
                    SQL.Length = 0
                    If HenkanFMT.InfoMeisaiMast.ITAKU_CODE = "" Then
                        Return False
                    Else
                        FileItakuCode = HenkanFMT.InfoMeisaiMast.ITAKU_CODE

                        SQL.Append("SELECT")
                        SQL.Append("     *")
                        SQL.Append(" FROM")
                        SQL.Append("     TORIMAST")
                        SQL.Append(" WHERE")
                        SQL.Append("     FSYORI_KBN_T = '1'")
                        SQL.Append(" AND ITAKU_CODE_T = '" & Trim(HenkanFMT.InfoMeisaiMast.ITAKU_CODE) & "'")
                    End If

                    OraReader = New CASTCommon.MyOracleReader(MainDB)
                    If OraReader.DataReader(SQL) = False Then            '�����R�[�h�����݂��Ȃ�
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "�����Y���Ȃ��@�ϑ��҃R�[�h:" & Trim(HenkanFMT.InfoMeisaiMast.ITAKU_CODE))
                        OraReader.Close()
                        OraReader = Nothing
                        Return False
                    Else
                        FileItakuName = OraReader.GetString("ITAKU_KNAME_T").Trim
                        FileTorisCode = OraReader.GetString("TORIS_CODE_T")
                        FileTorifCode = OraReader.GetString("TORIF_CODE_T")
                        If FileSyubetu = "" Then
                            FileSyubetu = OraReader.GetString("SYUBETU_T")
                        End If
                        OraReader.Close()
                        OraReader = Nothing
                    End If

                    '------------------------------------
                    '�X�P�W���[������
                    '------------------------------------
                    SQL.Length = 0
                    SQL.Append("SELECT")
                    SQL.Append("     *")
                    SQL.Append(" FROM")
                    SQL.Append("     SCHMAST")
                    SQL.Append(" WHERE")
                    SQL.Append("     FSYORI_KBN_S     = '1'")
                    SQL.Append(" AND ITAKU_CODE_S     = '" & Trim(HenkanFMT.InfoMeisaiMast.ITAKU_CODE) & "'")
                    SQL.Append(" AND FURI_DATE_S      = '" & Trim(ToriInfo.FURIDATE) & "'")
                    SQL.Append(" AND FUNOU_FLG_S      = '1'")
                    If ToriInfo.MULTI_KBN = "0" Then  '�V���O��
                        SQL.Append(" AND TORIS_CODE_S = '" & Trim(ToriInfo.TORIS_CODE) & "'")
                        SQL.Append(" AND TORIF_CODE_S = '" & Trim(ToriInfo.TORIF_CODE) & "'")
                    End If

                    OraReader = New CASTCommon.MyOracleReader(MainDB)
                    If OraReader.DataReader(SQL) = False Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "�X�P�W���[���Y���Ȃ�")
                        OraReader.Close()
                        OraReader = Nothing
                        Return False
                    Else
                        dblSCH_SYORI_KEN = OraReader.GetInt64("SYORI_KEN_S")
                        dblSCH_SYORI_KIN = OraReader.GetInt64("SYORI_KIN_S")
                        dblSCH_FURI_KEN = OraReader.GetInt64("FURI_KEN_S")
                        dblSCH_FURI_KIN = OraReader.GetInt64("FURI_KIN_S")
                        dblSCH_FUNOU_KEN = OraReader.GetInt64("FUNOU_KEN_S")
                        dblSCH_FUNOU_KIN = OraReader.GetInt64("FUNOU_KIN_S")

                        OraReader.Close()
                        OraReader = Nothing
                    End If

                ElseIf HenkanFMT.IsTrailerRecord Then
                    '=========================================
                    ' �g���[�����R�[�h
                    '=========================================
                    dblFILE_SYORI_KEN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO.Trim)
                    dblFILE_SYORI_KIN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO.Trim)
                    dblFILE_FURI_KEN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO.Trim)
                    dblFILE_FURI_KIN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO.Trim)
                    dblFILE_FUNOU_KEN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO.Trim)
                    dblFILE_FUNOU_KIN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO.Trim)

                    '-----------------------------------------
                    ' �����E���z�`�F�b�N
                    '-----------------------------------------
                    If dblSCH_SYORI_KEN <> dblFILE_SYORI_KEN Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "���������s��v�A���R�[�hNO�F" & dblRECORD_COUNT & Err.Description)
                        Return False
                    End If

                    If dblSCH_SYORI_KIN <> dblFILE_SYORI_KIN Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "�������z�s��v�A���R�[�hNO�F" & dblRECORD_COUNT & Err.Description)
                        Return False
                    End If

                    If dblSCH_FUNOU_KEN <> dblFILE_FUNOU_KEN Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "�s�\�����s��v�A���R�[�hNO�F" & dblRECORD_COUNT & Err.Description)
                        Return False
                    End If

                    If dblSCH_FUNOU_KIN <> dblFILE_FUNOU_KIN Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", "�s�\���z�s��v�A���R�[�hNO�F" & dblRECORD_COUNT & Err.Description)
                        Return False
                    End If

                    Dim ListViewData As String = UketukeNoDisp & "/" & _
                                                     FileItakuCode & "/" & _
                                                     FileItakuName & "/" & _
                                                     ToriInfo.FURIDATE & "/" & _
                                                     FileSyubetu & "/" & _
                                                     Format(CInt(dblFILE_SYORI_KEN), "###,##0") & "/" & _
                                                     Format(CLng(dblFILE_SYORI_KIN), "###,###,###,##0") & "/" & _
                                                     Format(CInt(dblFILE_FURI_KEN), "###,##0") & "/" & _
                                                     Format(CLng(dblFILE_FURI_KIN), "###,###,###,##0") & "/" & _
                                                     Format(CInt(dblFILE_FUNOU_KEN), "###,##0") & "/" & _
                                                     Format(CLng(dblFILE_FUNOU_KIN), "###,###,###,##0") & "/" & _
                                                     FileTorisCode & "-" & FileTorifCode
                    ListViewArray.Add(ListViewData)
                End If
            Loop

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "����", "")

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "���s", ex.Message)
            Return False
        Finally
            If Not HenkanFMT Is Nothing Then
                HenkanFMT.Close()
                HenkanFMT = Nothing
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃f�[�^�`�F�b�N", "�I��", "")
        End Try
    End Function
    '2016/02/05 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- END
#End Region

#Region " �֐�(Sub) "

    '================================
    ' �e�L�X�g�{�b�N�X0����
    '================================
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtTorisCode.Validating, txtTorifCode.Validating, txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�e�L�X�g�{�b�N�X0����", "���s", ex.Message)
        Finally
            ' NOP
        End Try
    End Sub

    '================================
    ' �����R�[�h���͏���
    '================================
    Private Sub TextBox_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtTorisCode.Validated, txtTorifCode.Validated

        Try
            '--------------------------------
            ' ���O���ݒ�
            '--------------------------------
            If SetLogInfo(False) = False Then
                Exit Try
            End If

            '--------------------------------
            ' �ϑ��҃R�[�h�ݒ�
            '--------------------------------
            If txtTorisCode.Text.Trim <> "" And txtTorifCode.Text.Trim <> "" Then
                If GetItakuInfo(txtTorisCode.Text, txtTorifCode.Text) = False Then
                    Exit Try
                End If
            Else
                lblItakuCode.Text = ""
                lblItakuName.Text = ""
                lblCodeKbn.Text = ""
                lblFileName.Text = ""
                lblBaitai.Text = ""
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����R�[�h���͏���", "���s", ex.Message)
        Finally
            ' NOP
        End Try
    End Sub

    '================================
    ' �����(�R���{�{�b�N�X)���擾
    '================================
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '--------------------------------
                ' �ǉ������ݒ�
                '--------------------------------
                Dim Jyoken As String = " AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')"   '�}�̃R�[�h���}�̂̂���

                '--------------------------------
                ' �R���{�{�b�N�X�ݒ�
                '--------------------------------
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If

            cmbToriName.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����R���{�{�b�N�X�ݒ�", "���s", ex.Message)
        Finally
            ' NOP
        End Try
    End Sub

    '================================
    ' �����(�R���{�{�b�N�X)�I������
    '================================
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged

        Try
            '--------------------------------
            ' �����R�[�h�ݒ�
            '--------------------------------
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
            End If

            '--------------------------------
            ' ���O���ݒ�
            '--------------------------------
            If SetLogInfo(False) = False Then
                Exit Try
            End If

            '--------------------------------
            ' �ϑ��҃R�[�h�ݒ�
            '--------------------------------
            If txtTorisCode.Text.Trim <> "" And txtTorifCode.Text.Trim <> "" Then
                If GetItakuInfo(txtTorisCode.Text, txtTorifCode.Text) = False Then
                    Exit Try
                End If
            Else
                lblItakuCode.Text = ""
                lblItakuName.Text = ""
                lblCodeKbn.Text = ""
                lblFileName.Text = ""
                lblBaitai.Text = ""
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����R�[�h�ݒ�", "���s", ex.Message)
        Finally
            ' NOP
        End Try

    End Sub

    '================================
    ' ���W�I�{�^���ύX
    '================================
    Private Sub rdb_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Try
            '--------------------------------
            ' �ǉ������ݒ�
            '--------------------------------
            Dim Jyoken As String = " AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')"   '�}�̃R�[�h���}�̂̂���

            '--------------------------------
            ' �R���{�{�b�N�X�ݒ�
            '--------------------------------
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            cmbToriName.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���W�I�{�^���ύX", "���s", ex.Message)
        Finally
            ' NOP
        End Try
    End Sub

#End Region

End Class
' 2016/01/11 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
