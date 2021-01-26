Imports CASTCommon
Public Class KFGMENU010

    Private MainLOG As New CASTCommon.BatchLOG("KFGMENU010", "�w�Z�����j���[")
    Private Const msgTitle As String = "�w�Z�����j���[(KFGMENU010)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite
    Private noClose As Boolean

    ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- START
    ' RSV2�@�\�ݒ�
    '  1:�W��(RSV1�Ɠ��l) 2:��K�͔œI�Ȃ���
    Private INI_RSV2_EDITION As String

    ' �����}�X�^�̃}�X�^�p�^�[���w��
    '  1:�W��(RSV1�Ɠ��l) 2:��K�͔œI�Ȃ���
    Private INI_RSV2_MASTPTN As String
    ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- END

    '****************************
    'Form Load
    '****************************
    Private Sub KFGMENU010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try

            GCom.GetUserID = Microsoft.VisualBasic.Command()
            GCom.GetSysDate = Date.Now

            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            ' 2015/12/21 �^�X�N�j���� ADD �yPG�zUI_B-14-10(RSV2�Ή�) -------------------- START
            '*********************************************
            ' �g�p��ʏ����ݒ�
            '*********************************************
            Dim ERRMSG As String = ""
            If SetDisplayInfo(ERRMSG) = False Then
                If ERRMSG <> "" Then
                    MessageBox.Show(ERRMSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                Exit Try
            End If
            ' 2015/12/21 �^�X�N�j���� ADD �yPG�zUI_B-14-10(RSV2�Ή�) -------------------- END

            '���g�p�̃{�^�����\���ɂ���B
            For Each OBJ As TabPage In TabControl1.TabPages
                For Each CTL As Control In OBJ.Controls
                    If TypeOf CTL Is Button Then
                        CTL.Visible = (GCom.NzInt(CTL.Tag) > 0)
                        If CTL.Visible Then
                            CTL.Enabled = (GCom.NzInt(CTL.Tag) = 1)
                        End If
                    End If
                Next CTL
            Next OBJ

            '*****************************************
            '�N������
            '*****************************************
            'If UBound(Diagnostics.Process.GetProcessesByName( _
            '  Diagnostics.Process.GetCurrentProcess.ProcessName)) > 0 Then
            '    End
            'End If

            'fskj.ini���݃`�F�b�N
            If Not CheckIniFile() Then
                End
            End If

            GCom.GetLogFolder = STR_LOG_PATH
            GCom.GetTXTFolder = STR_TXT_PATH
            GCom.GetLSTFolder = STR_LST_PATH

            ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- START
            Dim clsExDsp As New CAstExternal.ClsExternalMenu()
            clsExDsp.Read_Menu(GCom.GetUserID, Me, TabControl1, CAstExternal.ClsExternalMenu.ExternalMENU_GAKKOU)
            ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- END

            '*** Str Add 2015/12/09 SO)H.Yamagishi for �g����� ***
            Dim clsExPrt As New CAstExtendPrint.ClsExtendPrintMenu()
            clsExPrt.Read_PrtMenu(GCom.GetUserID, Me, TabControl1, CAstExtendPrint.ClsExtendPrintMenu.EXPRTMENU_GAKKOU)
            '*** End Add 2015/12/09 SO)H.Yamagishi for �g����� ***

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub

    '****************************
    'Button Click
    '****************************
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            noClose = True
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            If Not OBJ_CONNECTION Is Nothing AndAlso OBJ_CONNECTION.State = ConnectionState.Open Then
                'Oracle CLOSE
                OBJ_CONNECTION.Close()
                OBJ_CONNECTION = Nothing
            End If

            Dim MenuModule As String = "MENU.EXE"
            Dim BinDirectory As String = GCom.SET_PATH(GCom.GetBinFolder)

            If System.IO.File.Exists(BinDirectory & MenuModule) Then
                Call StartProc(BinDirectory, MenuModule)
            End If
            Me.Close()
            Me.Dispose()
            Application.Exit()
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try

    End Sub


    '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
    '�R�l�N�V�����֐������ʊ֐��Ɉړ�
    ''*****************************************
    ''Oracle Conection
    ''*****************************************
    'Private Sub PSUB_CONNECT()
    '    If Not OBJ_CONNECTION Is Nothing AndAlso OBJ_CONNECTION.State = ConnectionState.Open Then
    '    Else
    '        Try
    '            'Oracle �ڑ�
    '            OBJ_CONNECTION = New Data.OracleClient.OracleConnection(STR_CONNECTION)
    '            'Oracle OPEN
    '            OBJ_CONNECTION.Open()

    '        Catch ex As Exception
    '            MessageBox.Show(ex.Message, "main", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '            End
    '        End Try
    '    End If
    'End Sub
    '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

#Region "����Ɩ�"

    '****************************
    '�����U��(GFJMAIN0100)
    '****************************
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAIN010 As New KFGMAIN010
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAIN010, Form), Me)
    End Sub

    '*** Str Del 2016/01/05 sys)mori for V2�p�C�� ***
    ''****************************
    ''���s���f�[�^�쐬(GFJMAIN0200)
    ''****************************
    'Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

    '    Call PSUB_CONNECT()

    '    Dim KFGMAIN020 As New KFGMAIN020
    '    Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAIN020, Form), Me)
    'End Sub
    '*** End Del 2016/01/05 sys)mori for V2�p�C�� ***

    '****************************
    '�����U�փf�[�^�쐬(GFJMAIN0300)
    '****************************
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAIN030 As New KFGMAIN030
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAIN030, Form), Me)
    End Sub

    '****************************
    '�����U�֕s�\���ʍX�V(GFJMAIN0400)
    '****************************
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAIN040 As New KFGMAIN040
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAIN040, Form), Me)
    End Sub

    '****************************
    '�������σf�[�^�쐬����(GFJMAIN0500)
    '****************************
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAIN050 As New KFGMAIN050
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAIN050, Form), Me)
    End Sub

    '****************************
    '���k���׍쐬(GFJMAIN0600)
    '****************************
    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAIN060 As New KFGMAIN060
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAIN060, Form), Me)
    End Sub

    '****************************
    '���k���ד���(GFJMAIN0700)
    '****************************
    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAIN070 As New KFGMAIN070
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAIN070, Form), Me)
    End Sub

#End Region

#Region "�}�X�^�o�^"

    '*********************************************
    '�V�����}�X�^����(KFGMAST120)
    '*********************************************
    Private Sub Button29_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button29.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAST120 As New GAKKOU.KFGMAST120
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST120, Form), Me)
    End Sub

    '*********************************************
    '���k�}�X�^����(KFGMAST110)
    '*********************************************
    Private Sub Button28_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button28.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAST110 As New GAKKOU.KFGMAST110
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST110, Form), Me)
    End Sub

    '*********************************************
    '��ڃ}�X�^����(KFGMAST100)
    '*********************************************
    Private Sub Button27_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button27.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAST100 As New GAKKOU.KFGMAST100
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST100, Form), Me)
    End Sub

    '*** Str Del 2016/01/05 sys)mori for V2�p�C�� ***
    ''*********************************************
    ''���ԃX�P�W���[���쐬(KFGMAST061)
    ''*********************************************
    'Private Sub Button23_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button23.Click

    '    Call PSUB_CONNECT()

    '    Dim KFGMAST061 As New MAIN.KFGMAST061
    '    Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST061, Form), Me)
    'End Sub
    '*** End Del 2016/01/05 sys)mori for V2�p�C�� ***

    '*********************************************
    '�X�P�W���[���}�X�^�X�V(KFGMAST070)
    '*********************************************
    Private Sub Button24_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button24.Click
        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAST070 As New GAKKOU.KFGMAST070
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST070, Form), Me)
    End Sub

    '*********************************************
    '�N�ԃX�P�W���[���쐬(KFGMAST060)
    '*********************************************
    Private Sub Button22_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button22.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAST060 As New GAKKOU.KFGMAST060
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST060, Form), Me)
    End Sub

    '*********************************************
    '���s���}�X�^�o�^(KFGMAST020)
    '*********************************************
    Private Sub Button18_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button18.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAST020 As New GAKKOU.KFGMAST020
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST020, Form), Me)
    End Sub

    '*********************************************
    '��ڃ}�X�^�o�^(KFGMAST030)
    '*********************************************
    Private Sub Button19_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button19.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAST030 As New GAKKOU.KFGMAST030
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST030, Form), Me)
    End Sub

    '*********************************************
    '�V�����}�X�^�o�^(KFGMAST050)
    '*********************************************
    Private Sub Button21_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button21.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAST050 As New GAKKOU.KFGMAST050
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST050, Form), Me)
    End Sub

    '*********************************************
    '���k�}�X�^�o�^(KFGMAST040)
    '*********************************************
    Private Sub Button20_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button20.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGMAST040 As New GAKKOU.KFGMAST040
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST040, Form), Me)
    End Sub

    '*********************************************
    '�w�Z�}�X�^�o�^(KFGMAST010)
    '*********************************************
    Private Sub Button17_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button17.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        ' 2016/09/01 �^�X�N�j���� CHG �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- START
        'Dim KFGMAST010 As New MAIN.KFGMAST010
        'Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST010, Form), Me)
        Select Case INI_RSV2_MASTPTN
            Case "1"
                Dim KFGMAST010 As New GAKKOU.KFGMAST010
                Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST010, Form), Me)
            Case "2"
                Dim KFGMAST011 As New GAKKOU.KFGMAST011
                Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST011, Form), Me)
        End Select
        ' 2016/09/01 �^�X�N�j���� CHG �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- END

    End Sub

    '*********************************************
    '�w�Z�c�[���A�g�i�f�[�^�ڏo�j(KFGMAST510)
    '*********************************************
    ' 2015/12/21 �^�X�N�j���� CHG �yPG�zUI_B-14-10(RSV2�Ή�) -------------------- START
    Private Sub Button30_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button30.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�w�Z�c�[���A�g�i�f�[�^�ڏo�j���(�ďo)�J�n", "����", "")
            '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
            'Call PSUB_CONNECT()
            '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END
            Dim KFGMAST510 As New KFGMAST510
            Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST510, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�w�Z�c�[���A�g�i�f�[�^�ڏo�j���(�ďo)�I��", "����", "")
        End Try
    End Sub
    ' 2015/12/21 �^�X�N�j���� CHG �yPG�zUI_B-14-10(RSV2�Ή�) -------------------- END

    '*********************************************
    '�w�Z�c�[���A�g�i�f�[�^�ړ��j(KFGMAST520)
    '*********************************************
    ' 2015/12/21 �^�X�N�j���� CHG �yPG�zUI_B-14-10(RSV2�Ή�) -------------------- START
    Private Sub Button31_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button31.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�w�Z�c�[���A�g�i�f�[�^�ړ��j���(�ďo)�J�n", "����", "")
            '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
            'Call PSUB_CONNECT()
            '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END
            Dim KFGMAST520 As New KFGMAST520
            Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST520, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�w�Z�c�[���A�g�i�f�[�^�ړ��j���(�ďo)�I��", "����", "")
        End Try
    End Sub
    ' 2015/12/21 �^�X�N�j���� CHG �yPG�zUI_B-14-10(RSV2�Ή�) -------------------- END

    '*********************************************
    '�w�Z�c�[���A�g�i�߂����ړ��j(KFGMAST530)
    '*********************************************
    ' 2015/12/21 �^�X�N�j���� CHG �yPG�zUI_B-14-10(RSV2�Ή�) -------------------- START
    Private Sub Button32_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button32.Click

        Try

            '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
            'Call PSUB_CONNECT()
            '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END
            Dim KFGMAST530 As New KFGMAST530
            Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGMAST530, Form), Me)

        Catch ex As Exception
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�w�Z�c�[���A�g�i�߂����ړ��j���(�ďo)�I��", "����", "")
        End Try
    End Sub
    ' 2015/12/21 �^�X�N�j���� CHG �yPG�zUI_B-14-10(RSV2�Ή�) -------------------- END

#End Region

#Region "���[���"

    '****************************
    '�w�Z���U�����`�F�b�N���X�g���(KFGPRNT120)
    '****************************
    Private Sub Button47_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button47.Click
        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT120 As New GAKKOU.KFGPRNT120
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT120, Form), Me)
    End Sub

    Private Sub Button38_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button38.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT130 As New GAKKOU.KFGPRNT130
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT130, Form), Me)

    End Sub

    '****************************
    '2010/09/15.Sakon�@�����U�ֈ˗������(KFGPRNT140)
    '****************************
    Private Sub Button39_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button39.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT140 As New GAKKOU.KFGPRNT140
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT140, Form), Me)
    End Sub

    Private Sub Button40_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button40.Click
        '2017/05/09 �^�X�N�j���� ADD �W���ŏC���i�����}�X�^���ڊm�F�[����@�\�ǉ��j---------------------- START
        Dim KFGPRNT160 As New GAKKOU.KFGPRNT160
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT160, Form), Me)
        '2017/05/09 �^�X�N�j���� ADD �W���ŏC���i�����}�X�^���ڊm�F�[����@�\�ǉ��j---------------------- END

        'Call PSUB_CONNECT()

        'Dim KFGPRNT1600G As New MAIN.KFGPRNT1600G
        'Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT1600G, Form), Me)
    End Sub

    '****************************
    '���q�`�F�b�N���X�g���(KFGPRNT100)
    '****************************
    Private Sub Button45_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button45.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT100 As New GAKKOU.KFGPRNT100
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT100, Form), Me)
    End Sub

    '****************************
    '�w�Z�}�X�^��������(KFGPRNT060)
    '****************************
    Private Sub Button41_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button41.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT060 As New GAKKOU.KFGPRNT060
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT060, Form), Me)
    End Sub

    '****************************
    '���k���ד��̓`�F�b�N���X�g���(KFGPRNT110)
    '****************************
    Private Sub Button46_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button46.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT110 As New GAKKOU.KFGPRNT110
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT110, Form), Me)
    End Sub

    '****************************
    '�������X�g���(KFGPRNT040)
    '****************************
    Private Sub Button36_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button36.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT040 As New GAKKOU.KFGPRNT040
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT040, Form), Me)
    End Sub

    '****************************
    '���[�󋵈ꗗ�\���(KFGPRNT050)
    '****************************
    Private Sub Button37_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button37.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT050 As New GAKKOU.KFGPRNT050
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT050, Form), Me)
    End Sub

    '****************************
    '���ԃX�P�W���[���\���(KFGPRNT0100)
    '****************************
    Private Sub Button33_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button33.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT010 As New GAKKOU.KFGPRNT010
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT010, Form), Me)
    End Sub

    '****************************
    '���k�}�X�^�`�F�b�N���X�g���(KFGPRNT0800)
    '****************************
    Private Sub Button43_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button43.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT080 As New GAKKOU.KFGPRNT080
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT080, Form), Me)
    End Sub

    '****************************
    '���k������(KFGPRNT070)
    '****************************
    Private Sub Button42_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button42.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT070_b As New GAKKOU.KFGPRNT070
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT070, Form), Me)
    End Sub

    '****************************
    '�V�����}�X�^�`�F�b�N���X�g���(KFGPRNT090)
    '****************************
    Private Sub Button44_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button44.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT090 As New GAKKOU.KFGPRNT090
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT090, Form), Me)
    End Sub

    '****************************
    '�����U�֗\��ꗗ�\���(KFGPRNT020)
    '****************************
    Private Sub Button34_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button34.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT020 As New GAKKOU.KFGPRNT020
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT020, Form), Me)
    End Sub

    '****************************
    '�����U�֌��ʒ��[���(KFGPRNT030)
    '****************************
    Private Sub Button35_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button35.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT030 As New GAKKOU.KFGPRNT030
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT030, Form), Me)
    End Sub
    '2011/06/23 �W���ŏC�� ���k�}�X�^�Ǘ��\����ǉ� ------------------START
    Private Sub Button48_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button48.Click
        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGPRNT150 As New GAKKOU.KFGPRNT150
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGPRNT150, Form), Me)
    End Sub
    '2011/06/23 �W���ŏC�� ���k�}�X�^�Ǘ��\����ǉ� ------------------END

#End Region

#Region "�����e�i���X"

    '****************************
    '�U�֌��ʕύX(GFJMNTC0200)
    '****************************
    Private Sub Button50_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button50.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGOTHR020 As New GAKKOU.KFGOTHR020
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGOTHR020, Form), Me)
    End Sub

    '****************************
    '�����U�փf�[�^�쐬���(GFJMNTC0100)
    '****************************
    Private Sub Button49_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button49.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGOTHR010 As New GAKKOU.KFGOTHR010
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGOTHR010, Form), Me)

    End Sub

    '****************************
    '�������ώ��(GFJMNTC0900)
    '****************************
    Private Sub Button52_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button52.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGOTHR090 As New GAKKOU.KFGOTHR090
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGOTHR090, Form), Me)
    End Sub

#End Region

#Region "�N������"

    '*** Str Add 2016/01/05 sys)mori for V2�p�C�� ***
    'Private Sub Button65_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button65.Click

    '    Call PSUB_CONNECT()

    '    Dim KFGNENJ030 As New MAIN.KFGNENJ030
    '    Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGNENJ030, Form), Me)
    'End Sub
    '*** End Add 2016/01/05 sys)mori for V2�p�C�� ***

    '****************************
    '�N���X�ւ�����(GFJNENJ0200)
    '****************************
    Private Sub Button67_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button67.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGNENJ020 As New GAKKOU.KFGNENJ020
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGNENJ020, Form), Me)
    End Sub

    '****************************
    '�i����������(GFJNENJ0100)
    '****************************
    Private Sub Button66_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button66.Click

        '2016/10/07 saitou RSV2 DEL �w�Z�������e�i���X ---------------------------------------- START
        'Call PSUB_CONNECT()
        '2016/10/07 saitou RSV2 DEL --------------------------------------------------------------- END

        Dim KFGNENJ010 As New GAKKOU.KFGNENJ010
        Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGNENJ010, Form), Me)
    End Sub

#End Region

#Region " �N���[�Y"
    Private Sub KFJMENU010_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If noClose = False Then
            e.Cancel = True
        End If
    End Sub

    Private Function StartProc(ByVal dir As String, ByVal exename As String, Optional ByVal wait As Boolean = False) As String
        Dim Arguments As String = LW.UserID    '����
        Try
            Dim ExecProc As Process = Process.Start(dir & exename, Arguments)
            If ExecProc.Id <> 0 Then
                Me.Visible = False
                If wait = True Then
                    '�I���ҋ@
                    ExecProc.WaitForExit()
                    Me.Visible = True
                    Me.Activate()
                Else
                    Me.Close()
                End If
            Else
                Throw New Exception(String.Format("�A�v���P�[�V�����̋N���Ɏ��s���܂����B'{0}'", exename))
            End If

        Catch ex As Exception
            Dim MessageText As String
            MessageText = ex.Message
            MessageText &= Environment.NewLine
            MessageText &= dir & exename
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�A�v���P�[�V�����N��", "���s", MessageText)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return ""
    End Function
#End Region

    ' 2015/12/21 �^�X�N�j���� ADD �yPG�zUI_B-14-10(RSV2�Ή�) -------------------- START
#Region "�֐�"
    Private Function SetDisplayInfo(ByRef ERRMSG As String) As Boolean

        Try

            Dim INI_RSV2_KFGMAST510 As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KFGMAST510")
            If INI_RSV2_KFGMAST510 = "err" OrElse INI_RSV2_KFGMAST510 = "" Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI�t�@�C���擾", "�ݒ�t�@�C���擾���s ���ږ�:KFGMAST510�g�p�L�� ����:RSV2_V1.0.0 ����:KFGMAST510")
                ERRMSG = "�ݒ�t�@�C���擾���s ���ږ�:KFGMAST510�g�p�L�� ����:RSV2_V1.0.0 ����:KFGMAST510"
                Return False
            End If
            If INI_RSV2_KFGMAST510 = "0" Then
                Button30.Tag = 0
            Else
                Button30.Tag = 1
            End If

            Dim INI_RSV2_KFGMAST520 As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KFGMAST520")
            If INI_RSV2_KFGMAST520 = "err" OrElse INI_RSV2_KFGMAST520 = "" Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI�t�@�C���擾", "�ݒ�t�@�C���擾���s ���ږ�:KFGMAST520�g�p�L�� ����:RSV2_V1.0.0 ����:KFGMAST520")
                ERRMSG = "�ݒ�t�@�C���擾���s ���ږ�:KFGMAST520�g�p�L�� ����:RSV2_V1.0.0 ����:KFGMAST520"
                Return False
            End If
            If INI_RSV2_KFGMAST520 = "0" Then
                Button31.Tag = 0
            Else
                Button31.Tag = 1
            End If

            Dim INI_RSV2_KFGMAST530 As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KFGMAST530")
            If INI_RSV2_KFGMAST530 = "err" OrElse INI_RSV2_KFGMAST530 = "" Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI�t�@�C���擾", "�ݒ�t�@�C���擾���s ���ږ�:KFGMAST530�g�p�L�� ����:RSV2_V1.0.0 ����:KFGMAST530")
                ERRMSG = "�ݒ�t�@�C���擾���s ���ږ�:KFGMAST530�g�p�L�� ����:RSV2_V1.0.0 ����:KFGMAST530"
                Return False
            End If
            If INI_RSV2_KFGMAST530 = "0" Then
                Button32.Tag = 0
            Else
                Button32.Tag = 1
            End If

            ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- START
            '--------------------------------
            ' RSV2�@�\�ݒ�
            '--------------------------------
            INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            If INI_RSV2_EDITION.ToUpper = "ERR" OrElse INI_RSV2_EDITION = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "RSV2�@�\�ݒ�", "RSV2_V1.0.0", "EDITION"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ݒ�t�@�C���擾", "���s", "���ږ�:RSV2�@�\�ݒ� ����:RSV2_V1.0.0 ����:EDITION")
                Return False
            End If

            '--------------------------------
            ' �����}�X�^�p�^�[��
            '--------------------------------
            INI_RSV2_MASTPTN = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN")
            If INI_RSV2_MASTPTN.ToUpper = "ERR" OrElse INI_RSV2_MASTPTN = Nothing Then
                INI_RSV2_MASTPTN = "1"
            Else
                Select Case INI_RSV2_MASTPTN
                    Case "1", "2"
                        ' �擾�n�j
                    Case Else
                        MessageBox.Show(String.Format(MSG0001E, "�����}�X�^�p�^�[��", "RSV2_V1.0.0", "MASTPTN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ݒ�t�@�C���擾", "���s", "���ږ�:�����}�X�^�p�^�[�� ����:RSV2_V1.0.0 ����:MASTPTN")
                        Return False
                End Select
            End If
            ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- END

            Return True

        Catch ex As Exception
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�g�p��ʏ����ݒ�", "", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            'NOP
        End Try

    End Function
#End Region
    ' 2015/12/21 �^�X�N�j���� ADD �yPG�zUI_B-14-10(RSV2�Ή�) -------------------- END

End Class
