Option Explicit On 
Option Strict Off

Imports CASTCommon

Public Class KFGMAIN072

    'KFJMAIN0100G�}�X�^�������ݗp
    Private gastrTORI_CODE_MAIN0100 As String
    Private gastrFURI_DATE_MAIN0100 As String
    Private gstrBAITAI_CODE_MAIN0100 As String
    Private gstrFMT_KBN_MAIN0100 As String
    Private gstrCODE_KBN_MAIN0100 As String
    Private gstrLABEL_KBN_MAIN0100 As String

    '�R�����_�C�A���O�p
    Public Const STR_DLG_FILTER As String = "*.dat"
    Public Const STR_DLG_FILTER_NAME As String = "�S��t�@�C��"
    Public Const STR_DEF_FILE_KBN As String = "DAT"

#Region " ���ʕϐ���` "
    Private INTCNT01 As Integer
    Private Bln_Gakunen_Flg As Boolean
    Private Bln_Ginko_Flg(1) As Boolean

    Private Str_ZenginFile As String
    Private Str_Syori_Date(1) As String
    Private Str_Ginko(3) As String
    Private Str_Gakunen_Flg() As String
    Private Str_Baitai_Code As String
    Private Str_WHERE As String

    Private Lng_Ijyo_Count As Long
    Private Lng_Err_Count As Long

    Private Lng_RecordNo As Long

    Private Lng_Trw_Count As Long

    Private Str_Syori_Ginko(,) As String

    '�G���[���X�g�쐬����n�p�p�����[�^
    Private Int_Err_Gakunen_Code As Integer
    Private Int_Err_Class_Code As Integer
    Private Str_Err_Seito_No As String
    Private Int_Err_Tuuban As Integer
    Private Str_Err_Itaku_Name As String
    Private Str_Err_Tkin_No As String
    Private Str_Err_Tsit_No As String
    Private Str_Err_Kamoku As String
    Private Str_Err_Kouza As String
    Private Str_Err_Keiyaku_No As String
    Private Str_Err_Keiyaku_Name As String
    Private Lng_Err_Furikae_Kingaku As Long
    Private Str_Err_Msg As String

    '�ǉ� 2006/10/05
    Public lngGAK_SYORI_KEN(10) As Long
    Public dblGAK_SYORI_KIN(10) As Double

    Public lngTAKOU_SYORISAKI As Long = 0 '���s�f�[�^(�U���ް��t�@�C��)���쐬������ 2006/10/06
    Public Str_Seikyu_Nentuki As String = ""
    Public strFUNOU_YDATE As String = ""
    Public strGAKKOU_KNAME As String = ""
    Public flgNEXT_DATA_MAKE As Boolean = True '�����r���ŃL�����Z�����邩�ǂ��� True:���s False:���f
    Public intPRNT_SORT As Integer = 0 '���[�\�[�g�� 2006/10/18
    Public strTKIN_NO_GAK As String = "" '�w�Z�̎戵���Z�@�փR�[�h 2006/10/23
    Public strTSIT_NO_GAK As String = "" '�w�Z�̎戵�x�X�R�[�h 2006/10/23
    Public strKAMOKU_GAK As String = "" '�w�Z�̉ȖڃR�[�h 2006/10/23
    Public strKOUZA_GAK As String = "" '�w�Z�̌����ԍ� 2006/10/23
    Private Const msgTitle As String = "�����f�[�^�쐬���(KFGMAIN072)"
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAIN072", "�����f�[�^�쐬���")
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite
#End Region

#Region " Form Load "
    Private Sub KFGMAIN072_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '#####################################
        '���O�̏����ɕK�v�ȏ��̎擾
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            LW.ToriCode = STR_���k���׊w�Z�R�[�h
            LW.FuriDate = STR_���k���אU�֓�

            '�w�Z�R�[�h
            lab�w�Z�R�[�h.Text = STR_���k���׊w�Z�R�[�h

            lab�w�Z��.Text = STR_���k���׊w�Z��

            lab�U�֓�.Text = Mid(STR_���k���אU�֓�, 1, 4) & "/" & Mid(STR_���k���אU�֓�, 5, 2) & "/" & Mid(STR_���k���אU�֓�, 7, 2)

            Select Case (STR_���k���ד��o�敪)
                Case "2"
                    lab���o���敪.Text = "����"
                Case "3"
                    lab���o���敪.Text = "�o��"
            End Select

            STR_SQL = "SELECT * FROM GAKMAST2"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_T ='" & STR_���k���׊w�Z�R�[�h & "'"

            Str_Baitai_Code = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "BAITAI_CODE_T")
            intPRNT_SORT = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "MEISAI_OUT_T") '2006/10/18
            strTKIN_NO_GAK = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "TKIN_NO_T")  '�w�Z�̎戵���Z�@�փR�[�h 2006/10/23
            strTSIT_NO_GAK = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "TSIT_NO_T")  '�w�Z�̎戵�x�X�R�[�h 2006/10/23
            strKAMOKU_GAK = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "KAMOKU_T") '�w�Z�̉ȖڃR�[�h 2006/10/23
            strKOUZA_GAK = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "KOUZA_T")  '�w�Z�̌����ԍ� 2006/10/23

            '�w�Z���J�i�擾
            STR_SQL = "SELECT GAKKOU_KNAME_G FROM GAKMAST1"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_G ='" & STR_���k���׊w�Z�R�[�h & "'"
            STR_SQL += " group by GAKKOU_KNAME_G"

            strGAKKOU_KNAME = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "GAKKOU_KNAME_G")


            '�X�v���b�h�ꗗ�̐ݒ�
            If PFUNC_Spread_Set() = False Then
                MainLOG.Write("���[�h", "���s", "���k���׎擾�G���[")
                Exit Sub
            End If

            '�X�P�W���[������������
            If PFUNC_Get_Gakunen() = False Then
                MainLOG.Write("���[�h", "���s", "�X�P�W���[���擾�G���[")
                Exit Sub
            End If

            'Oracle �ڑ�(Read��p)
            OBJ_CONNECTION_DREAD = New Data.OracleClient.OracleConnection(STR_CONNECTION)
            'Oracle OPEN(Read��p)
            OBJ_CONNECTION_DREAD.Open()

            'Oracle �ڑ�(Read��p)
            OBJ_CONNECTION_DREAD2 = New Data.OracleClient.OracleConnection(STR_CONNECTION)
            'Oracle OPEN(Read��p)
            OBJ_CONNECTION_DREAD2.Open()

            'Oracle �ڑ�(Read��p)
            OBJ_CONNECTION_DREAD3 = New Data.OracleClient.OracleConnection(STR_CONNECTION)
            'Oracle OPEN(Read��p)
            OBJ_CONNECTION_DREAD3.Open()

            btnCreate.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    Private Sub btnCreate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreate.Click

        Cursor.Current = Cursors.WaitCursor()
        Dim strDIR As String
        strDIR = CurDir()
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^�쐬)�J�n", "����", "")
            STR_COMMAND = "�����f�[�^�쐬"

            If MessageBox.Show(String.Format(MSG0015I, "�����f�[�^�쐬"), _
                               msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If
            '�������̎擾
            Str_Syori_Date(0) = Format(Now, "yyyyMMdd")
            Str_Syori_Date(1) = Format(Now, "yyyyMMddHHmmss")

            Lng_Ijyo_Count = 0

            '�G���[���X�g�̏�����
            Call PSUB_Delete_IjyoList()

            flgNEXT_DATA_MAKE = True '�������f�t���O������

            'If Bln_Ginko_Flg(0) = True Then'�����ɂ��̔���͂Ȃ�
            '���S�M������ ���s�E���s��̃t�@�C���ɂ܂Ƃ߂� 2007/09/06
            '���s�S��쐬
            Call PSUB_Insert_Meisai()

            Lng_Ijyo_Count += Lng_Err_Count
            'End If

            '���s�����[��� 2006/10/17
            If Lng_Ijyo_Count = 0 Or Lng_Ijyo_Count = Lng_Trw_Count Then
                Select Case (CInt(STR_���k���׈���敪))
                    Case 1, 2

                        Dim nRet As Integer
                        Dim Param As String
                        '����o�b�`�Ăяo��
                        Dim ExeRepo As New CAstReports.ClsExecute
                        ExeRepo.SetOwner = Me

                        '�p�����[�^�ݒ�F���O�C�����A�w�Z�R�[�h�A������
                        Param = GCom.GetUserID & "," & STR_���k���׊w�Z�R�[�h & "," & Str_Syori_Date(1)

                        nRet = ExeRepo.ExecReport("KFGP005.EXE", Param)

                        '�߂�l�ɑΉ��������b�Z�[�W��\������
                        Select Case nRet
                            Case 0
                            Case Else
                                '������s���b�Z�[�W
                                MessageBox.Show(String.Format(MSG0004E, "�����U�֗\��W�v�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Select


                        '�����U�֗\��ꗗ�\
                        ExeRepo = New CAstReports.ClsExecute
                        ExeRepo.SetOwner = Me
                        '�p�����[�^�ݒ�F���O�C�����A�w�Z�R�[�h�A�������A�U�֓��A���[����敪(1:�X�ԃ\�[�g�Ȃ� 2:�X�ԃ\�[�g����)�A���[�\�[�g��
                        Param = GCom.GetUserID & "," & STR_���k���׊w�Z�R�[�h & "," _
                                 & Str_Syori_Date(1) & "," & STR_���k���אU�֓� & "," & STR_���k���׈���敪 & "," _
                                 & intPRNT_SORT

                        nRet = ExeRepo.ExecReport("KFGP003.EXE", Param)

                        '�߂�l�ɑΉ��������b�Z�[�W��\������
                        Select Case nRet
                            Case 0
                            Case Else
                                '������s���b�Z�[�W
                                MessageBox.Show(String.Format(MSG0004E, "�����U�֗\��ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Select
                End Select
            End If

            flgNEXT_DATA_MAKE = True '�������f�t���O������

            '�G���[�������P���ȏ�̏ꍇ�̓G���[���X�g���
            Select Case (Lng_Ijyo_Count)
                Case 0, Lng_Trw_Count
                    '�������b�Z�[�W
                    MessageBox.Show(String.Format(MSG0016I, "�����f�[�^�쐬"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case Else
                    '�G���[���X�g���
                    Dim nRet As Integer
                    Dim Param2 As String
                    '����o�b�`�Ăяo��
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me

                    '�p�����[�^�ݒ�F���O�C����
                    Param2 = GCom.GetUserID

                    nRet = ExeRepo.ExecReport("KFGP001.EXE", Param2)

                    '�߂�l�ɑΉ��������b�Z�[�W��\������
                    Select Case nRet
                        Case 0
                        Case Else
                            '������s���b�Z�[�W
                            MessageBox.Show(String.Format(MSG0004E, "�C���v�b�g�G���[���X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Select

            End Select

            btnEnd.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^�쐬)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^�쐬)�I��", "����", "")
            ChDir(strDIR)
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            If OBJ_CONNECTION_DREAD Is Nothing Then
            Else
                'Oracle CLOSE
                OBJ_CONNECTION_DREAD.Close()
                OBJ_CONNECTION_DREAD = Nothing
            End If

            If OBJ_CONNECTION_DREAD2 Is Nothing Then
            Else
                'Oracle CLOSE
                OBJ_CONNECTION_DREAD2.Close()
                OBJ_CONNECTION_DREAD2 = Nothing
            End If

            If OBJ_CONNECTION_DREAD3 Is Nothing Then
            Else
                'Oracle CLOSE
                OBJ_CONNECTION_DREAD3.Close()
                OBJ_CONNECTION_DREAD3 = Nothing
            End If

            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Private Sub "
    Private Sub PSUB_Delete_IjyoList()

        Lng_Ijyo_Count = 0

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Sub
        End If

        STR_SQL = " DELETE  FROM G_IJYOLIST"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Sub
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Sub
        End If

    End Sub
    Private Sub PSUB_Insert_IjyoList()

        On Error Resume Next

        STR_SQL = " INSERT INTO G_IJYOLIST"
        STR_SQL += " values("
        STR_SQL += "'" & Str_Syori_Date(0) & "'"
        STR_SQL += "," & Int_Err_Gakunen_Code
        STR_SQL += "," & Int_Err_Class_Code
        STR_SQL += ",'" & Str_Err_Seito_No & "'"
        STR_SQL += "," & Int_Err_Tuuban
        STR_SQL += "," & Lng_Ijyo_Count
        STR_SQL += ",'" & STR_���k���׊w�Z�R�[�h & "'"
        STR_SQL += ",'" & STR_���k���אU�֓� & "'"
        STR_SQL += ",'" & Str_Err_Itaku_Name & "'"
        STR_SQL += "," & Lng_RecordNo
        STR_SQL += ",'" & Str_Err_Tkin_No & "'"
        STR_SQL += ",'" & Str_Err_Tsit_No & "'"
        STR_SQL += ",'" & Str_Err_Kamoku & "'"
        STR_SQL += ",'" & Str_Err_Kouza & "'"
        STR_SQL += ",'" & Str_Err_Keiyaku_No & "'"
        STR_SQL += ",'" & Str_Err_Keiyaku_Name.PadRight(40).Substring(0, 30) & "'"
        STR_SQL += "," & Lng_Err_Furikae_Kingaku
        STR_SQL += ",'" & Str_Err_Msg & "'"
        STR_SQL += ",'" & Format(Now, "yyyyMMddHHmmss") & "'" '�^�C���X�^���v 2006/12/25
        STR_SQL += ",'" & Space(14) & "'"
        STR_SQL += ",'" & Space(14) & "'"
        STR_SQL += " )"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Sub
        End If

    End Sub
    Private Sub PSUB_Insert_Meisai()

        Dim bLoopFlg As Boolean

        Dim iLcount As Integer
        Dim iNo As Integer
        Dim iFileNo As Integer

        Dim sJyuyouka_No As String
        Dim sBuff As String
        Dim sZenginFile As String
        Dim sBaitai_Code As String
        Dim sFile_Name As String

        Dim lThrrowCount As Long

        Dim lRecordCount As Long
        Dim lFurikae_Kingaku As Long
        Dim lTotal_Kingaku As Long
        Dim lTotal_Kensuu As Long

        Dim MainDB As New MyOracle

        On Error Resume Next

        '�����U�֖��׃}�X�^�폜(���s�̂�)
        If PFUNC_Delete_Meisai(0) = False Then
            MainLOG.Write("���׍쐬", "���s", "�����U�֖��׃}�X�^�폜�G���[")
            Exit Sub
        End If

        '�S��t�@�C���쐬
        Select Case (CInt(STR_���k���ד��o�敪))
            Case 2
                Str_ZenginFile = STR_DAT_PATH & "D" & STR_���k���׊w�Z�R�[�h & "03.dat"
            Case 3
                Str_ZenginFile = STR_DAT_PATH & "D" & STR_���k���׊w�Z�R�[�h & "04.dat"
        End Select

        If Dir$(Str_ZenginFile) <> "" Then Kill(Str_ZenginFile)

        iFileNo = FreeFile()
        Err.Number = 0

        FileOpen(iFileNo, Str_ZenginFile, OpenMode.Random, , , 120)    '���[�N�t�@�C��

        If Err.Number <> 0 Then
            MainLOG.Write("���׍쐬", "���s", "�S��̧��OPEN�G���[")
            Exit Sub
        End If

        STR_SQL = " SELECT "
        STR_SQL += " ITAKU_CODE_T , KAMOKU_T , KOUZA_T , FILE_NAME_T , BAITAI_CODE_T"
        STR_SQL += ", GAKKOU_KNAME_G"
        STR_SQL += ", KIN_NO_N , KIN_KNAME_N , SIT_NO_N , SIT_KNAME_N"
        STR_SQL += " FROM GAKMAST1 , GAKMAST2 , TENMAST , G_SCHMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_G = GAKKOU_CODE_T"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_T = KIN_NO_N"
        STR_SQL += " AND"
        STR_SQL += " TSIT_NO_T = SIT_NO_N"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_G = GAKKOU_CODE_S"
        STR_SQL += " AND"
        STR_SQL += " NENGETUDO_S = '" & Mid(STR_���k���אU�֓�, 1, 6) & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_S = '" & STR_���k���ד��o�敪 & "'"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_G = '" & STR_���k���׊w�Z�R�[�h & "'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S = '2'"

        '�f�[�^�x�[�XOPEN
        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            MainLOG.Write("���׍쐬", "���s", "�ް��ް�OPEN�G���[")
            Exit Sub
        End If

        '�f�[�^���R�[�h�`�F�b�N
        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Sub
            End If

            MainLOG.Write("���׍쐬", "���s", "���׃f�[�^�O��")
            Exit Sub
        End If

        OBJ_DATAREADER.Read()

        '�S��f�[�^�쐬(�w�b�_)
        '�f�[�^�敪(=1) 
        '��ʃR�[�h(21 OR 91)
        '�R�[�h�敪
        '�U���˗��l�R�[�h
        '�U���˗��l��
        '�戵��
        '�d����s����
        '�d����s��
        '�d���x�X����
        '�d���x�X��
        '�a�����
        '�����ԍ�
        '�_�~�[
        With gZENGIN_REC1
            .ZG1 = "1"
            Select Case (CInt(STR_���k���ד��o�敪))
                Case 2
                    '����
                    .ZG2 = "21"
                    iNo = 1
                Case 3
                    '�����o��
                    .ZG2 = "91"
                    iNo = 2
            End Select
            .ZG3 = "0" 'JIS�`���݂̂Ȃ̂�"1"��"0"�ɏC�� 2006/10/18
            .ZG4 = OBJ_DATAREADER.Item("ITAKU_CODE_T")
            .ZG5 = OBJ_DATAREADER.Item("GAKKOU_KNAME_G")
            .ZG6 = Mid(STR_���k���אU�֓�, 5, 4)
            .ZG7 = OBJ_DATAREADER.Item("KIN_NO_N")
            .ZG8 = OBJ_DATAREADER.Item("KIN_KNAME_N")
            .ZG9 = OBJ_DATAREADER.Item("SIT_NO_N")
            .ZG10 = OBJ_DATAREADER.Item("SIT_KNAME_N")
            .ZG11 = Format(CInt(OBJ_DATAREADER.Item("KAMOKU_T")), "0")
            .ZG12 = Format(CInt(OBJ_DATAREADER.Item("KOUZA_T")), "0000000")
            .ZG13 = Space(17)
        End With
        If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 1) = False Then
            MainLOG.Write("���׍쐬", "���s", "�S��̧��ͯ�ޕ������݃G���[")
            Exit Sub
        End If

        With OBJ_DATAREADER
            sBaitai_Code = Trim(.Item("BAITAI_CODE_T"))
            sFile_Name = Trim(.Item("FILE_NAME_T"))
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Sub
        End If

        bLoopFlg = False

        lRecordCount = 1
        lTotal_Kingaku = 0
        lTotal_Kensuu = 0
        lFurikae_Kingaku = 0
        Lng_Err_Count = 0

        lThrrowCount = 0

        '�G���g���}�X�^�擾(���s�̂�)
        STR_SQL = " SELECT "
        STR_SQL += " G_ENTMAST" & iNo & ".*"
        STR_SQL += ", TKIN_NO_T , TSIT_NO_T , KAMOKU_T , KOUZA_T"
        STR_SQL += " FROM G_ENTMAST" & iNo & " , GAKMAST2"
        STR_SQL += " WHERE GAKKOU_CODE_E = GAKKOU_CODE_T"
        '���S�M������ ���s�E���s��̃t�@�C���ɂ܂Ƃ߂� 2007/09/06
        'STR_SQL += " AND"
        'STR_SQL += " TKIN_NO_E ='" & Str_Jikou_Ginko_Code & "'"
        STR_SQL += " AND FURI_DATE_E ='" & STR_���k���אU�֓� & "'"
        STR_SQL += " AND FURIKIN_E > 0"
        STR_SQL += " AND GAKKOU_CODE_E = '" & STR_���k���׊w�Z�R�[�h & "'"
        If Bln_Gakunen_Flg = False Then
            STR_SQL += " AND ("
            For iLcount = 1 To UBound(Str_Gakunen_Flg)
                If bLoopFlg = True Then
                    STR_SQL += " OR "
                End If
                STR_SQL += " GAKUNEN_CODE_E=" & Str_Gakunen_Flg(iLcount)
                bLoopFlg = True
            Next iLcount
            STR_SQL += " )"
        End If
        '�U���ް��͋��ɁE�X�ԁE�ȖځE�����ԍ��E�w�N(�~��)�E������ 2006/10/17
        STR_SQL += " ORDER BY TKIN_NO_E ASC, TSIT_NO_E ASC, KAMOKU_E ASC, KOUZA_E ASC, GAKUNEN_CODE_E DESC"

        '�f�[�^�x�[�XOPEN
        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            MainLOG.Write("���׍쐬", "���s", "�f�[�^�x�[�XOPEN�G���[")
            Exit Sub
        End If

        '�f�[�^���R�[�h�`�F�b�N
        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Sub
            End If

            MainLOG.Write("���׍쐬", "���s", "���׃f�[�^�O��")
            Exit Sub
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Sub
            End If

            Exit Sub
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD
                Int_Err_Gakunen_Code = .Item("GAKUNEN_CODE_E")
                Int_Err_Class_Code = .Item("CLASS_CODE_E")
                Str_Err_Seito_No = .Item("SEITO_NO_E")
                Int_Err_Tuuban = .Item("TUUBAN_E")
                Str_Err_Itaku_Name = ""
                Str_Err_Tkin_No = .Item("TKIN_NO_E")
                Str_Err_Tsit_No = .Item("TSIT_NO_E")
                Str_Err_Kamoku = .Item("KAMOKU_E")
                Str_Err_Kouza = .Item("KOUZA_E")
                Str_Err_Keiyaku_No = .Item("KEIYAKU_NO_E")
                Str_Err_Keiyaku_Name = .Item("KEIYAKU_KNAME_E")
                Lng_Err_Furikae_Kingaku = .Item("FURIKIN_E")

                '�U�֋��z
                lFurikae_Kingaku = .Item("FURIKIN_E")

                '���v�Ɣԍ�
                sJyuyouka_No = .Item("NENDO_E") & Format(.Item("TUUBAN_E"), "0000") & Str_Seikyu_Nentuki.Substring(4, 2)
            End With

            Call PSUB_GET_GINKONAME(OBJ_DATAREADER_DREAD.Item("TKIN_NO_E"), OBJ_DATAREADER_DREAD.Item("TSIT_NO_E"), MainDB)

            '�S��f�[�^�쐬(����)
            '�f�[�^�敪(=2)
            '��d����s�ԍ�
            '��d����s���@
            '��d���x�X�ԍ�
            '��d���x�X��
            '��`�������ԍ�
            '�a�����
            '�����ԍ�
            '���l
            '�U�����z
            '�V�K�R�[�h
            '�ڋq�R�[�h�P
            '�ڋq�R�[�h�Q
            '�U���w��敪
            '�_�~�[
            With gZENGIN_REC2
                .ZG1 = "2"
                .ZG2 = Format(CLng(OBJ_DATAREADER_DREAD.Item("TKIN_NO_E")), "0000")
                .ZG3 = Str_Ginko(0)
                .ZG4 = Format(CLng(OBJ_DATAREADER_DREAD.Item("TSIT_NO_E")), "000")
                .ZG5 = Str_Ginko(2)
                .ZG6 = Space(4)
                .ZG7 = CASTCommon.ConvertKamoku2TO1(OBJ_DATAREADER_DREAD.Item("KAMOKU_E"))
                .ZG8 = Format(CLng(OBJ_DATAREADER_DREAD.Item("KOUZA_E")), "0000000")
                .ZG9 = Mid(OBJ_DATAREADER_DREAD.Item("KEIYAKU_KNAME_E"), 1, 30)
                .ZG10 = Format(lFurikae_Kingaku, "0000000000")
                .ZG11 = "0"
                .ZG12 = sJyuyouka_No
                .ZG13 = CInt(STR_���k���ד��o�敪) & Space(9)      '�X�P�W���[���敪��ݒ聨�U�֋敪�ɕύX 2006/12/22
                .ZG14 = "0"
                .ZG15 = Space(8)
            End With
            If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 2) = False Then
                If GFUNC_SELECT_SQL3("", 1) = False Then
                    Exit Sub
                End If

                Exit Sub
            End If

            '���דo�^�p�U�փf�[�^(�S��t�H�[�}�b�g�̃f�[�^���R�[�h�P�Q�OBYTE)
            '2017/05/15 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
            '���Z�@�֖��J�i�^�x�X���J�i�̌��󔒂��g��������Ă���
            sBuff = gZENGIN_REC2.Data
            'sBuff = PFUNC_GET_ZENGIN_LINE(2)
            '2017/05/15 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END

            With OBJ_DATAREADER_DREAD
                '�����U�֖��׍쐬
                STR_SQL = " INSERT INTO G_MEIMAST"
                STR_SQL += " values("
                STR_SQL += "'" & STR_���k���׊w�Z�R�[�h & "'"
                STR_SQL += ",'" & .Item("NENDO_E") & "'"
                STR_SQL += ",'" & STR_���k���אU�֓� & "'"
                STR_SQL += "," & .Item("GAKUNEN_CODE_E")
                STR_SQL += "," & .Item("CLASS_CODE_E")
                STR_SQL += ",'" & .Item("SEITO_NO_E") & "'"
                STR_SQL += "," & .Item("TUUBAN_E")
                STR_SQL += ",'" & .Item("TKIN_NO_T") & "'"
                STR_SQL += ",'" & .Item("TSIT_NO_T") & "'"
                STR_SQL += ",'" & .Item("KAMOKU_T") & "'"
                STR_SQL += ",'" & .Item("KOUZA_T") & "'"
                STR_SQL += ",'" & .Item("TKIN_NO_E") & "'"
                STR_SQL += ",'" & .Item("TSIT_NO_E") & "'"
                STR_SQL += ",'" & .Item("KAMOKU_E") & "'"
                STR_SQL += ",'" & .Item("KOUZA_E") & "'"
                STR_SQL += ",'" & .Item("KEIYAKU_KNAME_E") & "'"
                STR_SQL += ",'" & Str_Seikyu_Nentuki.Substring(4, 2) & "�����'"
                STR_SQL += ",'" & sJyuyouka_No & CInt(STR_���k���ד��o�敪) & Space(9) & "'" '�U�֋敪�ɕύX 2006/12/22
                STR_SQL += ",'" & sBuff & "'"
                STR_SQL += "," & lRecordCount
                STR_SQL += ",'" & Mid(STR_���k���אU�֓�, 1, 6) & "'"
                STR_SQL += ",'" & Mid(STR_���k���אU�֓�, 1, 6) & "'"
                STR_SQL += ",'000'"
                STR_SQL += "," & lFurikae_Kingaku
                For iLcount = 1 To 15
                    STR_SQL += ",0"
                Next iLcount
                STR_SQL += ",0"
                STR_SQL += ",'0'"
                '2006/10/20 �����Əo���ŐU�֋敪��ύX����
                'STR_SQL += ",'2'"
                Select Case (CInt(STR_���k���ד��o�敪))
                    Case 2
                        '����
                        STR_SQL += ",'2'"
                    Case 3
                        '�����o��
                        STR_SQL += ",'3'"
                End Select

                STR_SQL += ",'" & Str_Syori_Date(1) & "'"   '�\���P
                STR_SQL += ",' '"                           '�\���Q
                STR_SQL += ",' '"                           '�\���R
                STR_SQL += ",' '"                           '�\���S
                STR_SQL += ",' '"                           '�\���T
                STR_SQL += ",' '"                           '�\���U
                STR_SQL += ",' '"                           '�\���V
                STR_SQL += ",' '"                           '�\���W
                STR_SQL += ",' '"                           '�\���X
                STR_SQL += ",' '"                           '�\���P�O
                STR_SQL += ")"
            End With

            '���׃}�X�^�o�^
            Select Case (PFUNC_Chk_Meisai())
                Case -1
                    '�G���[�L(�ُ탊�X�g�ɒǉ� , ���ׂɓo�^�Ȃ� , �X�P�W���[���X�V�Ȃ�)
                    Lng_Err_Count += 1

                    Call PSUB_Insert_IjyoList()
                Case 0
                    '����I��
                    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                        '�ُ탊�X�g�ǉ�
                        Lng_Err_Count += 1
                        Str_Err_Msg = "�f�[�^�x�[�X�G���[�ł�"

                        Call PSUB_Insert_IjyoList()
                    Else
                        lTotal_Kingaku += lFurikae_Kingaku
                        lTotal_Kensuu += 1

                        lRecordCount += 1
                    End If
                Case Else
                    '�G���[�L(�ُ탊�X�g�ɒǉ� , ���ׂɓo�^���� , �X�P�W���[���X�V����)
                    lThrrowCount += 1
                    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                        '�ُ탊�X�g�ǉ�
                        Lng_Err_Count += 1
                        Str_Err_Msg = "�f�[�^�x�[�X�G���[�ł�"

                        Call PSUB_Insert_IjyoList()
                    Else
                        lTotal_Kingaku += lFurikae_Kingaku
                        lTotal_Kensuu += 1

                        lRecordCount += 1
                    End If

                    Lng_Err_Count += 1

                    Call PSUB_Insert_IjyoList()
            End Select
        End While

        MainDB.Close()

        '�f�[�^�x�[�XCLOSE
        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Sub
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Str_Err_Msg = ""

            Exit Sub
        End If

        '�S��f�[�^�쐬(�g���[���[�s��W�v���ʣ)
        '�쐬���������Ƌ��z��ݒ�
        '�O���̕s�\���͍ĐU��ʂ�0�ȊO�̏ꍇ�̂ݏW�v���ʂɉ��Z�����
        '(�S��f�[�^�쐬(����)�������l)
        With gZENGIN_REC8
            .ZG1 = "8"
            .ZG2 = Format(lTotal_Kensuu, "000000")
            .ZG3 = Format(lTotal_Kingaku, "000000000000")
            .ZG4 = "000000"
            .ZG5 = "000000000000"
            .ZG6 = "000000"
            .ZG7 = "000000000000"
            .ZG8 = Space(65)
        End With

        If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 8) = False Then
            MainLOG.Write("���׍쐬", "���s", "���׃f�[�^�O��")

            Exit Sub
        End If

        '�S��f�[�^�쐬(�I���s)
        '�f�[�^�敪
        '�_�~�[
        With gZENGIN_REC9
            .ZG1 = "9"
            .ZG2 = Space(119)
        End With

        If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 9) = False Then
            Exit Sub
        End If

        Err.Number = 0

        FileClose(iFileNo)

        If Err.Number <> 0 Then
            MainLOG.Write("���׍쐬", "���s", "�S��̧��CLOSE�G���[")

            Exit Sub
        End If

        '��O�Ƃ��āA���Z�@�ւɑ��݂��Ȃ����Z�@�ւ̓o�^���̓G���[���X�P�W���[���̍X�V�������s��
        If Lng_Err_Count = 0 Or Lng_Err_Count = lThrrowCount Then
            '�X�P�W���[���X�V
            If PFUNC_Update_Schedule(iNo) = False Then

                MainLOG.Write("���׍쐬", "���s", "�X�P�W���[���X�V�G���[")

                Exit Sub
            End If

            'FD�R�s�[
            'Select case GFUNC_FD_Copy2(Me, STR_���k���׊w�Z��, Str_ZenginFile, sFile_Name, Str_Baitai_Code)
            '2005/06/27�m�F���b�Z�[�W��\�����Ȃ�
            Select Case (GFUNC_FD_Copy3(Me, STR_���k���׊w�Z��, Str_ZenginFile, sFile_Name, Str_Baitai_Code))
                Case 0
                    '����I��
                    MainLOG.Write("�����f�[�^�쐬", "����", "")

                Case 1
                    '�L�����Z��
                    flgNEXT_DATA_MAKE = False '���f
                    Exit Sub
                Case Else
                    '�G���[
                    MainLOG.Write("���׍쐬", "���s", "�S��̧��FD�ۑ��G���[")

                    Exit Sub
            End Select

            Dim JobID As String = ""
            Dim Param As String = ""
            '2005/06/27
            '�W���u�Ď��Ƀp�����[�^�ǉ�
            '------------------------------------------------
            '�W���u�}�X�^�ɓo�^
            '------------------------------------------------
            JobID = "J010"
            Select Case (STR_���k���ד��o�敪)
                Case "2"
                    gastrTORI_CODE_MAIN0100 = STR_���k���׊w�Z�R�[�h & "03"
                Case "3"
                    gastrTORI_CODE_MAIN0100 = STR_���k���׊w�Z�R�[�h & "04"
            End Select

            gastrFURI_DATE_MAIN0100 = STR_���k���אU�֓�
            gstrCODE_KBN_MAIN0100 = "0"
            gstrFMT_KBN_MAIN0100 = "00"
            gstrBAITAI_CODE_MAIN0100 = "7"
            gstrLABEL_KBN_MAIN0100 = "0"
            Param = gastrTORI_CODE_MAIN0100 & "," & gastrFURI_DATE_MAIN0100 & "," & gstrCODE_KBN_MAIN0100 & "," & gstrFMT_KBN_MAIN0100 _
                            & "," & gstrBAITAI_CODE_MAIN0100 & "," & gstrLABEL_KBN_MAIN0100

            If fn_JOBMAST_TOUROKU_CHECK(JobID, GCom.GetUserID, Param) = False Then
                gdbcCONNECT.Close()
                Me.Close()
                Exit Sub
            End If
            If fn_INSERT_JOBMAST(JobID, GCom.GetUserID, Param) = False Then
                MainLOG.Write("�p�����[�^�o�^", "���s", JobID & ":" & Param)
            Else
                'MessageBox.Show("�N���p�����^��o�^���܂���", gstrSYORI_R)
                MainLOG.Write("�p�����[�^�o�^", "����", JobID & ":" & Param)
            End If
        End If

        Lng_Trw_Count += lThrrowCount

    End Sub
    '    Private Sub PSUB_Insert_Meisai_Takou(ByVal pGinko_Code As String, _
    '                                         ByVal pSiten_Code As String, _
    '                                         ByVal pItaku_Code As String, _
    '                                         ByVal pKamoku As String, _
    '                                         ByVal pKouza As String, _
    '                                         ByVal pFileName As String, _
    '                                         ByVal pCodeKbn As String)

    '        Dim bLoopFlg As Boolean

    '        Dim iLcount As Integer
    '        Dim iNo As Integer
    '        Dim iFileNo As Integer

    '        Dim lThrrowCount As Long

    '        Dim sJyuyouka_No As String
    '        Dim sBuff As String

    '        Dim lRecordCount As Long
    '        Dim lFurikae_Kingaku As Long
    '        Dim lTotal_Kingaku As Long
    '        Dim lTotal_Kensuu As Long
    '        Dim lSyukei(1) As Long

    '        ReDim lngGAK_SYORI_KEN(10)
    '        ReDim dblGAK_SYORI_KIN(10)

    '        Select Case (CInt(STR_���k���ד��o�敪))
    '            Case 2
    '                Str_ZenginFile = STR_DAT_PATH & "D" & STR_���k���׊w�Z�R�[�h & pGinko_Code & "03.dat"
    '            Case 3
    '                Str_ZenginFile = STR_DAT_PATH & "D" & STR_���k���׊w�Z�R�[�h & pGinko_Code & "01.dat"
    '        End Select

    '        If Dir$(Str_ZenginFile) <> "" Then Kill(Str_ZenginFile)

    '        iFileNo = FreeFile()
    '        Err.Number = 0

    '        FileOpen(iFileNo, Str_ZenginFile, OpenMode.Random, , , 120)    '���[�N�t�@�C��

    '        If Err.Number <> 0 Then
    '            If GFUNC_SELECT_SQL3("", 1) = False Then
    '                Exit Sub
    '            End If

    '            Call GSUB_LOG(0, "�S��̧��OPEN�G���[")

    '            Exit Sub
    '        End If

    '        '��s���擾
    '        Call PSUB_GET_GINKONAME(pGinko_Code, pSiten_Code)

    '        '�S��f�[�^�쐬(�w�b�_)
    '        '�f�[�^�敪(=1) 
    '        '��ʃR�[�h(21 OR 91)
    '        '�R�[�h�敪
    '        '�U���˗��l�R�[�h
    '        '�U���˗��l��
    '        '�戵��
    '        '�d����s����
    '        '�d����s��
    '        '�d���x�X����
    '        '�d���x�X��
    '        '�a�����
    '        '�����ԍ�
    '        '�_�~�[
    '        With gZENGIN_REC1
    '            .ZG1 = "1"
    '            Select Case (CInt(STR_���k���ד��o�敪))
    '                Case 2
    '                    '����
    '                    .ZG2 = "21"
    '                    iNo = 1
    '                Case 3
    '                    '�����o��
    '                    .ZG2 = "91"
    '                    iNo = 2
    '            End Select
    '            .ZG3 = "0" 'JIS�`���݂̂Ȃ̂�"1"��"0"�ɏC�� 2006/10/18
    '            .ZG4 = pItaku_Code
    '            .ZG5 = strGAKKOU_KNAME.Trim
    '            .ZG6 = Mid(STR_���k���אU�֓�, 5, 4)
    '            .ZG7 = pGinko_Code
    '            .ZG8 = Str_Ginko(0)
    '            .ZG9 = pSiten_Code
    '            .ZG10 = Str_Ginko(2)
    '            .ZG11 = pKamoku
    '            .ZG12 = pKouza
    '            .ZG13 = Space(17)
    '        End With

    '        If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 1) = False Then
    '            Call GSUB_LOG(0, "�S��̧��ͯ�ޕ������݃G���[")

    '            Exit Sub
    '        End If



    '        bLoopFlg = False

    '        lRecordCount = 1
    '        lTotal_Kingaku = 0
    '        lTotal_Kensuu = 0
    '        lFurikae_Kingaku = 0
    '        Lng_Err_Count = 0

    '        '�G���g���}�X�^�擾(���s�̂�)
    '        STR_SQL = " SELECT "
    '        STR_SQL += " G_ENTMAST" & iNo & ".*"
    '        STR_SQL += ", ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V,BAITAI_CODE_V"
    '        STR_SQL += " FROM "
    '        STR_SQL += " G_ENTMAST" & iNo & " , G_TAKOUMAST "
    '        STR_SQL += " WHERE"
    '        STR_SQL += " GAKKOU_CODE_E = GAKKOU_CODE_V"
    '        STR_SQL += " AND"
    '        STR_SQL += " TKIN_NO_E = TKIN_NO_V"
    '        STR_SQL += " AND"
    '        STR_SQL += " TKIN_NO_E = '" & pGinko_Code & "'"
    '        STR_SQL += " AND"
    '        STR_SQL += " FURIKIN_E > 0"
    '        STR_SQL += " AND"
    '        STR_SQL += " GAKKOU_CODE_E = '" & STR_���k���׊w�Z�R�[�h & "'"
    '        '2006/10/20 �U�֓��������ɒǉ�
    '        STR_SQL += " AND"
    '        STR_SQL += " FURI_DATE_E ='" & STR_���k���אU�֓� & "'"
    '        If Bln_Gakunen_Flg = False Then
    '            STR_SQL += " AND ("
    '            For iLcount = 1 To UBound(Str_Gakunen_Flg)
    '                If bLoopFlg = True Then
    '                    STR_SQL += " OR "
    '                End If
    '                STR_SQL += " GAKUNEN_CODE_E=" & Str_Gakunen_Flg(iLcount)
    '                bLoopFlg = True
    '            Next iLcount
    '            STR_SQL += " )"
    '        End If
    '        '�U���ް��͋��ɁE�X�ԁE�ȖځE�����ԍ��E�w�N(�~��)�E������ 2006/10/17
    '        STR_SQL += " ORDER BY TKIN_NO_E ASC, TSIT_NO_E ASC, KAMOKU_E ASC, KOUZA_E ASC, GAKUNEN_CODE_E DESC"

    '        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
    '            Exit Sub
    '        End If

    '        '�f�[�^���R�[�h�`�F�b�N
    '        If OBJ_DATAREADER_DREAD.HasRows = False Then
    '            If GFUNC_SELECT_SQL3("", 1) = False Then
    '                Exit Sub
    '            End If

    '            Call GSUB_LOG(0, "���׃f�[�^�O��")

    '            Exit Sub
    '        End If

    '        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
    '            If GFUNC_SELECT_SQL3("", 1) = False Then
    '                Exit Sub
    '            End If

    '            Exit Sub
    '        End If

    '        While (OBJ_DATAREADER_DREAD.Read = True)

    '            With OBJ_DATAREADER_DREAD
    '                '�}�̃R�[�h�擾 2006/10/11
    '                Str_Baitai_Code = .Item("BAITAI_CODE_V")

    '                Int_Err_Gakunen_Code = .Item("GAKUNEN_CODE_E")
    '                Int_Err_Class_Code = .Item("CLASS_CODE_E")
    '                Str_Err_Seito_No = .Item("SEITO_NO_E")
    '                Int_Err_Tuuban = .Item("TUUBAN_E")
    '                Str_Err_Itaku_Name = ""
    '                Str_Err_Tkin_No = .Item("TKIN_NO_E")
    '                Str_Err_Tsit_No = .Item("TSIT_NO_E")
    '                Str_Err_Kamoku = .Item("KAMOKU_E")
    '                Str_Err_Kouza = .Item("KOUZA_E")
    '                Str_Err_Keiyaku_No = .Item("KEIYAKU_NO_E")
    '                Str_Err_Keiyaku_Name = .Item("KEIYAKU_KNAME_E")
    '                Lng_Err_Furikae_Kingaku = .Item("FURIKIN_E")

    '                '�U�֋��z
    '                lFurikae_Kingaku = .Item("FURIKIN_E")

    '                '���v�Ɣԍ�
    '                sJyuyouka_No = .Item("NENDO_E") & Format(.Item("TUUBAN_E"), "0000") & Str_Seikyu_Nentuki.Substring(4, 2)
    '            End With

    '            Call PSUB_GET_GINKONAME(OBJ_DATAREADER_DREAD.Item("TKIN_NO_E"), OBJ_DATAREADER_DREAD.Item("TSIT_NO_E"))

    '            If lFurikae_Kingaku = 0 Then '�U�֋��z0�~�̐��k�̓f�[�^�쐬���Ȃ� 2006/10/05
    '                GoTo NEXT_SEITO
    '            End If

    '            '�S��f�[�^�쐬(����)
    '            '�f�[�^�敪(=2)
    '            '��d����s�ԍ�
    '            '��d����s���@
    '            '��d���x�X�ԍ�
    '            '��d���x�X��
    '            '��`�������ԍ�
    '            '�a�����
    '            '�����ԍ�
    '            '���l
    '            '�U�����z
    '            '�V�K�R�[�h
    '            '�ڋq�R�[�h�P
    '            '�ڋq�R�[�h�Q
    '            '�U���w��敪
    '            '�_�~�[
    '            With gZENGIN_REC2
    '                .ZG1 = "2"
    '                .ZG2 = Format(CLng(OBJ_DATAREADER_DREAD.Item("TKIN_NO_E")), "0000")
    '                .ZG3 = Str_Ginko(0)
    '                .ZG4 = Format(CLng(OBJ_DATAREADER_DREAD.Item("TSIT_NO_E")), "000")
    '                .ZG5 = Str_Ginko(2)
    '                .ZG6 = Space(4)
    '                .ZG7 = Format(CInt(OBJ_DATAREADER_DREAD.Item("KAMOKU_E")), "0")
    '                .ZG8 = Format(CLng(OBJ_DATAREADER_DREAD.Item("KOUZA_E")), "0000000")
    '                .ZG9 = CStr(OBJ_DATAREADER_DREAD.Item("KEIYAKU_KNAME_E")).Trim
    '                .ZG10 = Format(lFurikae_Kingaku, "0000000000")
    '                .ZG11 = "0"
    '                .ZG12 = sJyuyouka_No
    '                .ZG13 = CInt(STR_���k���ד��o�敪) & Space(9)      '�X�P�W���[���敪��ݒ聨�U�֋敪�ɕύX 2006/12/22
    '                .ZG14 = "0"
    '                .ZG15 = Space(8)
    '            End With

    '            If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 2) = False Then
    '                If GFUNC_SELECT_SQL3("", 1) = False Then
    '                    Exit Sub
    '                End If

    '                Exit Sub
    '            End If

    '            '���דo�^�p�U�փf�[�^(�S��t�H�[�}�b�g�̃f�[�^���R�[�h�P�Q�OBYTE)
    '            sBuff = PFUNC_GET_ZENGIN_LINE(2)

    '            With OBJ_DATAREADER_DREAD
    '                '�����U�֖��׍쐬
    '                STR_SQL = " INSERT INTO G_MEIMAST"
    '                STR_SQL += " values("
    '                STR_SQL += "'" & STR_���k���׊w�Z�R�[�h & "'"
    '                STR_SQL += ",'" & .Item("NENDO_E") & "'"
    '                STR_SQL += ",'" & STR_���k���אU�֓� & "'"
    '                STR_SQL += "," & .Item("GAKUNEN_CODE_E")
    '                STR_SQL += "," & .Item("CLASS_CODE_E")
    '                STR_SQL += ",'" & .Item("SEITO_NO_E") & "'"
    '                STR_SQL += "," & .Item("TUUBAN_E")
    '                STR_SQL += ",'" & strTKIN_NO_GAK & "'"
    '                STR_SQL += ",'" & strTSIT_NO_GAK & "'"
    '                STR_SQL += ",'" & strKAMOKU_GAK & "'"
    '                STR_SQL += ",'" & strKOUZA_GAK & "'"
    '                STR_SQL += ",'" & .Item("TKIN_NO_E") & "'"
    '                STR_SQL += ",'" & .Item("TSIT_NO_E") & "'"
    '                STR_SQL += ",'" & .Item("KAMOKU_E") & "'"
    '                STR_SQL += ",'" & .Item("KOUZA_E") & "'"
    '                STR_SQL += ",'" & .Item("KEIYAKU_KNAME_E") & "'"
    '                STR_SQL += ",'" & Str_Seikyu_Nentuki.Substring(4, 2) & "�����'"
    '                STR_SQL += ",'" & sJyuyouka_No & CInt(STR_���k���ד��o�敪) & Space(9) & "'" '�U�֋敪�ɕύX 2006/12/22
    '                STR_SQL += ",'" & sBuff & "'"
    '                STR_SQL += "," & lRecordCount
    '                STR_SQL += ",'" & Mid(STR_���k���אU�֓�, 1, 6) & "'"
    '                STR_SQL += ",'" & Mid(STR_���k���אU�֓�, 1, 6) & "'"
    '                STR_SQL += ",'000'"
    '                STR_SQL += "," & lFurikae_Kingaku
    '                For iLcount = 1 To 15
    '                    STR_SQL += ",0"
    '                Next iLcount
    '                STR_SQL += ",0"
    '                STR_SQL += ",'0'"
    '                '2006/10/20 �����Əo���ŐU�֋敪��ύX����
    '                'STR_SQL += ",'2'"
    '                Select Case (CInt(STR_���k���ד��o�敪))
    '                    Case 2
    '                        '����
    '                        STR_SQL += ",'2'"
    '                    Case 3
    '                        '�����o��
    '                        STR_SQL += ",'3'"
    '                End Select
    '                STR_SQL += ",'" & Str_Syori_Date(1) & "'"
    '                STR_SQL += ",' '"
    '                STR_SQL += ",' '"
    '                STR_SQL += ")"

    '                '���׃}�X�^�o�^
    '                Select Case (PFUNC_Chk_Meisai())
    '                    Case -1
    '                        '�G���[�L(�ُ탊�X�g�ɒǉ� , ���ׂɓo�^�Ȃ� , �X�P�W���[���X�V�Ȃ�)
    '                        Lng_Err_Count += 1

    '                        Call PSUB_Insert_IjyoList()
    '                    Case 0
    '                        '����I��
    '                        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
    '                            '�ُ탊�X�g�ǉ�
    '                            Lng_Err_Count += 1
    '                            Str_Err_Msg = "�f�[�^�x�[�X�G���[�ł�"

    '                            Call PSUB_Insert_IjyoList()
    '                        Else
    '                            lTotal_Kingaku += lFurikae_Kingaku
    '                            lTotal_Kensuu += 1

    '                            lRecordCount += 1

    '                            '�w�N���̌����擾 2006/10/05
    '                            lngGAK_SYORI_KEN(CInt(.Item("GAKUNEN_CODE_E"))) += 1
    '                            dblGAK_SYORI_KIN(CInt(.Item("GAKUNEN_CODE_E"))) += lFurikae_Kingaku
    '                        End If
    '                    Case Else
    '                        '�G���[�L(�ُ탊�X�g�ɒǉ� , ���ׂɓo�^���� , �X�P�W���[���X�V����)
    '                        lThrrowCount += 1
    '                        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
    '                            '�ُ탊�X�g�ǉ�
    '                            Lng_Err_Count += 1
    '                            Str_Err_Msg = "�f�[�^�x�[�X�G���[�ł�"

    '                            Call PSUB_Insert_IjyoList()
    '                        Else
    '                            lTotal_Kingaku += lFurikae_Kingaku
    '                            lTotal_Kensuu += 1

    '                            lRecordCount += 1
    '                        End If

    '                        Lng_Err_Count += 1

    '                        Call PSUB_Insert_IjyoList()
    '                End Select
    '            End With

    'NEXT_SEITO:
    '        End While

    '        If GFUNC_SELECT_SQL3("", 1) = False Then
    '            If OBJ_TRANSACTION.Connection Is Nothing Then
    '            Else
    '                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then 'Commit����
    '                    Exit Sub
    '                End If
    '            End If

    '            Exit Sub
    '        End If

    '        '�Y���f�[�^���O���̏ꍇ�I�� 2006/10/06
    '        If lTotal_Kensuu = 0 Then
    '            Err.Number = 0
    '            FileClose(iFileNo)
    '            If Dir$(Str_ZenginFile) <> "" Then Kill(Str_ZenginFile)

    '            If OBJ_TRANSACTION.Connection Is Nothing Then
    '            Else
    '                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then 'Commit����
    '                    Exit Sub
    '                End If
    '            End If

    '            Exit Sub
    '        Else
    '            lngTAKOU_SYORISAKI += 1
    '        End If

    '        '�S��f�[�^�쐬(�g���[���[�s��W�v���ʣ)
    '        '�쐬���������Ƌ��z��ݒ�
    '        '�O���̕s�\���͍ĐU��ʂ�0�ȊO�̏ꍇ�̂ݏW�v���ʂɉ��Z�����
    '        '(�S��f�[�^�쐬(����)�������l)
    '        With gZENGIN_REC8
    '            .ZG1 = "8"
    '            .ZG2 = Format(lTotal_Kensuu, "000000")
    '            .ZG3 = Format(lTotal_Kingaku, "000000000000")
    '            .ZG4 = "000000"
    '            .ZG5 = "000000000000"
    '            .ZG6 = "000000"
    '            .ZG7 = "000000000000"
    '            .ZG8 = Space(65)
    '        End With

    '        If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 8) = False Then
    '            Call GSUB_LOG(0, "�S��̧�كg���[�����R�[�h�������݃G���[")
    '            If OBJ_TRANSACTION.Connection Is Nothing Then
    '            Else
    '                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
    '                    Exit Sub
    '                End If
    '            End If
    '            Exit Sub
    '        End If

    '        '�S��f�[�^�쐬(�I���s)
    '        '�f�[�^�敪
    '        '�_�~�[
    '        With gZENGIN_REC9
    '            .ZG1 = "9"
    '            .ZG2 = Space(119)
    '        End With

    '        If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 9) = False Then
    '            Call GSUB_LOG(0, "�S��̧�كG���h���R�[�h�������݃G���[")
    '            If OBJ_TRANSACTION.Connection Is Nothing Then
    '            Else
    '                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
    '                    Exit Sub
    '                End If
    '            End If
    '            Exit Sub
    '        End If

    '        Err.Number = 0

    '        FileClose(iFileNo)

    '        If Err.Number <> 0 Then
    '            Call GSUB_LOG(0, "�S��̧��CLOSE�G���[")
    '            If OBJ_TRANSACTION.Connection Is Nothing Then
    '            Else
    '                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
    '                    Exit Sub
    '                End If
    '            End If
    '            Exit Sub
    '        End If

    '        ''���s�̏������ƍŌ�̋��Z�@�ւ̏W�v�s����̑S��t�@�C���Ƒ��s���ޭ�ق̍쐬��
    '        ''����Ȃ��̂ł����ōŌ�̋��Z�@�ւ̑S��t�@�C���Ƒ��s���ޭ�ق̍쐬���s��
    '        ''���ŏI���Z�@�֏W�v�s��FD�ۑ�����
    '        ''���s���ޭ�ٍ쐬
    '        ''�����Z�@�ւ̊w�N���Ƃő��s���ޭ�ق��쐬
    '        'If PFUNC_DelIns_TakouSchedule(sEsc_Gakunen, pGinko_Code, lSyukei(0), lSyukei(1)) = False Then
    '        '    Call GSUB_LOG(0, "���s���ޭ�ٍ쐬�G���[")

    '        '    Exit Sub
    '        'End If


    '        '������Z�@�ցE�w�N���Ƃő��s�X�P�W���[���쐬 2006/10/05
    '        For i As Integer = 1 To 9
    '            If lngGAK_SYORI_KEN(i) = 0 Then
    '                GoTo NEXT_FOR
    '            End If
    '            '���s���ޭ�ٍ쐬
    '            '�����Z�@�ւ̊w�N���Ƃő��s���ޭ�ق��쐬
    '            If PFUNC_DelIns_TakouSchedule(STR_���k���׊w�Z�R�[�h, i, 0, pGinko_Code) = False Then
    '                If OBJ_TRANSACTION.Connection Is Nothing Then
    '                Else
    '                    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
    '                        Exit Sub
    '                    End If
    '                End If
    '                Exit Sub
    '            End If
    'NEXT_FOR:
    '        Next


    '        If OBJ_TRANSACTION.Connection Is Nothing Then
    '        Else
    '            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
    '                Exit Sub
    '            End If
    '        End If


    '        If Lng_Err_Count = 0 Or Lng_Err_Count = lThrrowCount Then

    '            'FD�ۑ�
    '            Select Case (GFUNC_FD_Copy(Me, STR_���k���׊w�Z��, Str_ZenginFile, Trim(pFileName), Str_Baitai_Code, pGinko_Code))
    '                Case 0
    '                    '����I��
    '                    Call GSUB_LOG(1, "�����f�[�^�쐬")
    '                Case 1
    '                    '�L�����Z��
    '                    flgNEXT_DATA_MAKE = False '���f
    '                    Exit Sub
    '                Case Else
    '                    '�G���[
    '                    Call GSUB_LOG(0, "�S��̧��FD�ۑ��G���[")

    '                    Exit Sub
    '            End Select
    '            '���ŏI���Z�@�֏W�v�s��FD�ۑ�����
    '        End If

    '        Lng_Trw_Count += lThrrowCount

    '    End Sub
    Private Sub PSUB_GET_GINKONAME(ByVal pGinko_Code As String, ByVal pSiten_Code As String, ByVal db As MyOracle)

        '���Z�@�փR�[�h�Ǝx�X�R�[�h������Z�@�֖��A�x�X���𒊏o

        Str_Ginko(0) = ""
        Str_Ginko(1) = ""
        Str_Ginko(2) = ""
        Str_Ginko(3) = ""

        Dim Orareader As CASTCommon.MyOracleReader = Nothing

        Try
            If pGinko_Code.Trim = "" OrElse pSiten_Code.Trim = "" Then
                Exit Sub
            End If

            Dim SQL As New System.Text.StringBuilder(128)

            SQL.Append(" SELECT ")
            SQL.Append(" KIN_KNAME_N ")
            SQL.Append(",KIN_NNAME_N ")
            SQL.Append(",SIT_KNAME_N ")
            SQL.Append(",SIT_NNAME_N ")
            SQL.Append(" FROM TENMAST ")
            SQL.Append(" WHERE KIN_NO_N = '" & pGinko_Code & "'")
            SQL.Append(" AND SIT_NO_N = '" & pSiten_Code & "'")

            Orareader = New CASTCommon.MyOracleReader(db)

            If Orareader.DataReader(SQL) Then
                Str_Ginko(0) = Orareader.GetItem("KIN_KNAME_N")
                Str_Ginko(1) = Orareader.GetItem("KIN_NNAME_N")
                Str_Ginko(2) = Orareader.GetItem("SIT_KNAME_N")
                Str_Ginko(3) = Orareader.GetItem("SIT_NNAME_N")
            Else
                Exit Sub
            End If

            Orareader.Close()
            Orareader = Nothing

        Catch ex As Exception
            Throw New Exception("TENMAST�擾���s", ex)
        Finally
            If Not Orareader Is Nothing Then
                Orareader.Close()
                Orareader = Nothing
            End If
        End Try

    End Sub
#End Region

#Region " Private Function "

    Private Function PFUNC_Get_Gakunen(ByVal pGakkou_Code As String, ByRef pSiyou_gakunen() As Integer) As Integer

        Dim iLoopCount As Integer
        Dim iMaxGakunen As Integer

        ReDim pSiyou_gakunen(9)

        PFUNC_Get_Gakunen = -1

        '�I�����ꂽ�w�Z�̎w��U�֓��Œ��o
        '(�S�X�P�W���[���敪���Ώ�)
        STR_SQL = " SELECT "
        STR_SQL += " SCH_KBN_S"
        For iLoopCount = 1 To 9
            STR_SQL += ", GAKUNEN" & iLoopCount & "_FLG_S"
            pSiyou_gakunen(iLoopCount) = 0
        Next iLoopCount
        STR_SQL += ", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " left join GAKMAST2 on "
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_S ='" & pGakkou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND"
        STR_SQL += " ENTRI_FLG_S ='1'"
        STR_SQL += " AND"
        STR_SQL += " TYUUDAN_FLG_S ='0'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S ='2'" '�����̂� 2006/10/16
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_S ='2'" '�����̂� 2006/10/16


        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER.Read)
            With OBJ_DATAREADER
                iMaxGakunen = CInt(.Item("SIYOU_GAKUNEN_T"))
                For iLoopCount = 1 To iMaxGakunen
                    Select Case (CInt(.Item("GAKUNEN" & iLoopCount & "_FLG_S")))
                        Case 1
                            pSiyou_gakunen(iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                    End Select
                Next iLoopCount
            End With
        End While

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        '�g�p�w�N�S�ĂɊw�N�t���O������ꍇ�͑S�w�N�ΏۂƂ��Ĉ���
        '�w�N
        For iLoopCount = 1 To iMaxGakunen
            Select Case (pSiyou_gakunen(iLoopCount))
                Case Is <> 1
                    PFUNC_Get_Gakunen = iMaxGakunen

                    Exit Function
            End Select
        Next iLoopCount

        PFUNC_Get_Gakunen = 0

    End Function

    Private Function PFUNC_Spread_Set() As Boolean

        Dim iNo As Integer

        Dim MainDB As New MyOracle

        PFUNC_Spread_Set = False

        Try

            Select Case (STR_���k���ד��o�敪)
                Case "2"
                    iNo = 1
                Case "3"
                    iNo = 2
            End Select

            '�G���g���}�X�^����������

            '�X�v���b�h�w�b�_�̕ҏW
            Select Case (iNo)
                Case 1
                    DataGridView.Columns(17).HeaderText = "�������z"
                Case 2
                    DataGridView.Columns(17).HeaderText = "�o�����z"
            End Select

            '�G���g���}�X�^������SQL���쐬
            STR_SQL = " SELECT "
            STR_SQL += " G_ENTMAST" & iNo & ".*"
            STR_SQL += " FROM "
            STR_SQL += " G_ENTMAST" & iNo
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_E ='" & STR_���k���׊w�Z�R�[�h & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_E ='" & STR_���k���אU�֓� & "'"
            STR_SQL += " ORDER BY "
            Select Case (STR_���k���׃\�[�g��)
                Case "1"
                    '�w�N�A�N���X�A���k�ԍ�
                    STR_SQL += " GAKUNEN_CODE_E ASC, CLASS_CODE_E ASC, SEITO_NO_E ASC"
                Case "2"
                    '���w�N�x�A�ʔ�
                    STR_SQL += " GAKUNEN_CODE_E ASC, NENDO_E ASC, TUUBAN_E ASC"
                Case "3"
                    '���k���̃A�C�E�G�I��
                    STR_SQL += " GAKUNEN_CODE_E ASC, SEITO_KNAME_E ASC"
            End Select

            '�G���g���}�X�^���݃`�F�b�N
            If GFUNC_ISEXIST(STR_SQL) = False Then
                MessageBox.Show(G_MSG0007W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Function
            End If

            INTCNT01 = 0
            Bln_Ginko_Flg(0) = False
            Bln_Ginko_Flg(1) = False

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Function
            End If

            With DataGridView
                Select Case (STR_���k���׃\�[�g��)
                    Case "1"
                        '�w�N�A�N���X�A���k�ԍ�
                        .Columns(0).Visible = False
                        .Columns(1).Visible = False
                        .Columns(2).Visible = True
                        .Columns(3).Visible = True
                        .Columns(4).Visible = True
                        .Columns(5).Visible = False
                        .Columns(6).Visible = False
                        .Columns(7).Visible = False
                        .Columns(18).Visible = False
                    Case "2"
                        '���w�N�x�A�ʔ�
                        .Columns(0).Visible = True
                        .Columns(1).Visible = True
                        .Columns(2).Visible = False
                        .Columns(3).Visible = False
                        .Columns(4).Visible = False
                        .Columns(5).Visible = False
                        .Columns(6).Visible = False
                        .Columns(7).Visible = False
                        .Columns(18).Visible = False
                    Case "3"
                        '���k���̃A�C�E�G�I��
                        .Columns(0).Visible = False
                        .Columns(1).Visible = False
                        .Columns(2).Visible = False
                        .Columns(3).Visible = False
                        .Columns(4).Visible = False
                        .Columns(5).Visible = False
                        .Columns(6).Visible = False
                        .Columns(7).Visible = True
                        .Columns(18).Visible = False
                End Select
            End With

            While (OBJ_DATAREADER.Read = True)
                With DataGridView
                    '�s���̐ݒ�
                    Dim RowItem As New DataGridViewRow
                    RowItem.CreateCells(DataGridView)

                    '���w�N�x
                    RowItem.Cells(0).Value = OBJ_DATAREADER.Item("NENDO_E")
                    '�ʔ�
                    RowItem.Cells(1).Value = OBJ_DATAREADER.Item("TUUBAN_E")
                    '�w�N
                    RowItem.Cells(2).Value = OBJ_DATAREADER.Item("GAKUNEN_CODE_E")
                    '�N���X
                    RowItem.Cells(3).Value = OBJ_DATAREADER.Item("CLASS_CODE_E")
                    '���k�ԍ�
                    RowItem.Cells(4).Value = OBJ_DATAREADER.Item("SEITO_NO_E")
                    '���k���J�i
                    RowItem.Cells(5).Value = OBJ_DATAREADER.Item("SEITO_KNAME_E")
                    '���k������ 2007/02/10
                    If IsDBNull(OBJ_DATAREADER.Item("SEITO_NNAME_E")) = True Then
                        RowItem.Cells(6).Value = OBJ_DATAREADER.Item("SEITO_KNAME_E")
                    Else
                        If Trim(OBJ_DATAREADER.Item("SEITO_NNAME_E")) = "" Then '�X�y�[�X�̏ꍇ�J�i�\��
                            RowItem.Cells(6).Value = OBJ_DATAREADER.Item("SEITO_KNAME_E")
                        Else
                            RowItem.Cells(6).Value = OBJ_DATAREADER.Item("SEITO_NNAME_E")
                        End If
                    End If

                    '�\���p���k��
                    If RowItem.Cells(6).Value = "" Then
                        RowItem.Cells(7).Value = RowItem.Cells(5).Value
                    Else
                        RowItem.Cells(7).Value = RowItem.Cells(6).Value
                    End If

                    Select Case (OBJ_DATAREADER.Item("TKIN_NO_E"))
                        Case STR_JIKINKO_CODE
                            Bln_Ginko_Flg(0) = True
                        Case Else
                            Bln_Ginko_Flg(1) = True
                    End Select

                    Call PSUB_GET_GINKONAME(OBJ_DATAREADER.Item("TKIN_NO_E"), OBJ_DATAREADER.Item("TSIT_NO_E"), MainDB)

                    '���Z�@�փR�[�h�̊i�[
                    RowItem.Cells(8).Value = OBJ_DATAREADER.Item("TKIN_NO_E")
                    RowItem.Cells(9).Value = Str_Ginko(1)

                    '�x�X�R�[�h�̊i�[
                    RowItem.Cells(10).Value = OBJ_DATAREADER.Item("TSIT_NO_E")
                    RowItem.Cells(11).Value = Str_Ginko(3)

                    '�ȖڃR�[�h�̊i�[�i�Q������P���ɕϊ��j
                    Select Case (OBJ_DATAREADER.Item("KAMOKU_E"))
                        Case "01"
                            RowItem.Cells(12).Value = "2"
                        Case "02"
                            RowItem.Cells(12).Value = "1"
                        Case "05"
                            RowItem.Cells(12).Value = "3"
                        Case "37"
                            RowItem.Cells(12).Value = "4"
                        Case "04"
                            RowItem.Cells(12).Value = "9"
                        Case Else
                            RowItem.Cells(12).Value = "2"
                    End Select
                    '�Ȗږ��̕ϊ��A�i�[
                    Select Case (OBJ_DATAREADER.Item("KAMOKU_E"))
                        '2011/06/16 �W���ŏC�� �Ȗڂ�01�̏ꍇ���� ------------------START
                        'Case "02"
                        Case "01"
                            '2011/06/16 �W���ŏC�� �Ȗڂ�01�̏ꍇ���� ------------------END
                            '����
                            RowItem.Cells(13).Value = "��"
                        Case "03"
                            '�[��
                            RowItem.Cells(13).Value = "�["
                        Case "04"
                            '�E��
                            RowItem.Cells(13).Value = "�E"
                        Case Else
                            '����
                            '���̑�
                            RowItem.Cells(13).Value = "��"
                    End Select
                    '�����ԍ��̊i�[
                    RowItem.Cells(14).Value = OBJ_DATAREADER.Item("KOUZA_E")

                    '�_��Җ��̊i�[
                    '2006/12/08�@�f�[�^�x�[�X�ɂ̓X�y�[�X�������Ă��邽�߁AIsDBNull�ł͋󔒔���ł��Ȃ�
                    If IsDBNull(OBJ_DATAREADER.Item("KEIYAKU_NNAME_E")) = True Then
                        RowItem.Cells(15).Value = OBJ_DATAREADER.Item("KEIYAKU_KNAME_E")
                    Else
                        If Trim(OBJ_DATAREADER.Item("KEIYAKU_NNAME_E")) = "" Then '�X�y�[�X�̏ꍇ�J�i�\��
                            RowItem.Cells(15).Value = OBJ_DATAREADER.Item("KEIYAKU_KNAME_E")
                        Else
                            RowItem.Cells(15).Value = OBJ_DATAREADER.Item("KEIYAKU_NNAME_E")
                        End If
                    End If

                    '�_��Ҕԍ��̊i�[
                    RowItem.Cells(16).Value = OBJ_DATAREADER.Item("KEIYAKU_NO_E")
                    '���z�̊i�[
                    RowItem.Cells(17).ReadOnly = True
                    RowItem.Cells(17).Value = Format(CDbl(OBJ_DATAREADER.Item("FURIKIN_E")), "#,##0")

                    '�萔���̊i�[
                    RowItem.Cells(18).Value = Format(CDbl(OBJ_DATAREADER.Item("TESUU_E")), "#,##0")

                    For Cnt As Integer = 0 To RowItem.Cells.Count - 1
                        RowItem.Cells(Cnt).Style.BackColor = Color.Yellow
                    Next

                    .Rows.Add(RowItem)

                    INTCNT01 += 1
                End With
            End While

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            lab����.Text = Format(CDbl(INTCNT01), "#,##0")

            With DataGridView
                Dim SumKin As Decimal = 0
                For cnt As Integer = 0 To INTCNT01 - 1
                    SumKin += CDec(.Rows(cnt).Cells(17).Value)
                Next
                '�s���̐ݒ�
                .RowCount = INTCNT01 + 1
                '���v���z�\���s
                .Rows(INTCNT01).Cells(17).ReadOnly = True
                .Rows(INTCNT01).Cells(17).Value = Format(SumKin, "#,##0")

                txt���͍��v���z.Text = Format(CDbl(.Rows(INTCNT01).Cells(17).Value), "#,##0")
            End With


            PFUNC_Spread_Set = True

        Catch ex As Exception
            MainLog.Write("", "���s", ex.Message)
            Return False
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function
    Private Function PFUNC_Delete_Meisai(ByVal pIndex As Integer) As Boolean

        PFUNC_Delete_Meisai = False

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        '---------------------
        '�����U�֖��׃}�X�^�폜(���s�^���s���܂�)
        '----------------------
        STR_SQL = " DELETE  FROM G_MEIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M = '" & STR_���k���׊w�Z�R�[�h & "'"
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_TAISYOU_M = '" & Str_Seikyu_Nentuki & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M = '" & STR_���k���אU�֓� & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_M = '" & STR_���k���ד��o�敪 & "'"
        '���S�M�������@���s�E���s�ꏏ�̃t�@�C�� 2007/09/06
        'Select case pIndex
        '    Case 0
        '        '���s
        '        STR_SQL += " AND"
        '        STR_SQL += " TKIN_NO_M = '" & Str_Jikou_Ginko_Code & "'"
        '    Case 1
        '        '���s
        '        STR_SQL += " AND"
        '        STR_SQL += " TKIN_NO_M <> '" & Str_Jikou_Ginko_Code & "'"
        'End Select

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            MainLOG.Write("���׍폜", "���s", "���s�X�P�W���[�����׍폜")
            If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                Exit Function
            End If
            Exit Function
        End If

        If pIndex = 1 Then '���s�̏ꍇ 2006/10/17
            '���s�X�P�W���[���}�X�^�폜
            '���O�d�l�ł͂P��̏����łP���R�[�h�����쐬����Ȃ�������
            '�@���s�d�l�ł�1��̏����ŕ������R�[�h���݂���ׂ����ō폜���Ă���
            STR_SQL = " DELETE  G_TAKOUSCHMAST"
            STR_SQL += " WHERE GAKKOU_CODE_U = '" & STR_���k���׊w�Z�R�[�h & "'"
            STR_SQL += " AND FURI_KBN_U = '" & STR_���k���ד��o�敪 & "'"
            STR_SQL += " AND FURI_DATE_U = '" & STR_���k���אU�֓� & "'"
            STR_SQL += " AND TKIN_NO_U <> '" & STR_JIKINKO_CODE & "'"

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                MainLOG.Write("�w�Z���s�}�X�^�폜", "���s", "���s�X�P�W���[���폜")

                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Exit Function
                End If
                Exit Function
            End If

        End If

        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        PFUNC_Delete_Meisai = True

    End Function
    Private Function PFUNC_Get_Gakunen() As Boolean

        '�N�ԃX�P�W���[�����琏�������̂��̂���������
        Dim iLoopCount As Integer
        Dim iGakunenCount As Integer

        PFUNC_Get_Gakunen = False

        STR_SQL = " SELECT "
        STR_SQL += " G_SCHMAST.* "
        STR_SQL += ", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T"
        STR_SQL += " FROM G_SCHMAST , GAKMAST2"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_S ='" & STR_���k���׊w�Z�R�[�h & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & STR_���k���אU�֓� & "'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S = '2'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_S ='" & STR_���k���ד��o�敪 & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        '�X�P�W���[���}�X�^���݃`�F�b�N
        If OBJ_DATAREADER.HasRows = False Then
            MessageBox.Show(G_MSG0008W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        OBJ_DATAREADER.Read()

        iGakunenCount = 1

        With OBJ_DATAREADER

            For iLoopCount = 1 To CInt(.Item("SIYOU_GAKUNEN_T"))
                If .Item("GAKUNEN" & iLoopCount & "_FLG_S") = "1" Then
                    ReDim Preserve Str_Gakunen_Flg(iGakunenCount)
                    Str_Gakunen_Flg(iGakunenCount) = iLoopCount
                    iGakunenCount += 1
                End If
            Next

            '�g�p�w�N�����w�Z�}�X�^�Őݒ肳��Ă���g�p�w�N���ƈ�v����ꍇ��
            '�S�w�N�����o�Ώ�
            If CInt(.Item("SIYOU_GAKUNEN_T")) = (UBound(Str_Gakunen_Flg) - 1) Then
                Bln_Gakunen_Flg = False
            Else
                Bln_Gakunen_Flg = True
            End If
            '�ǉ� 2006/10/16
            Str_Seikyu_Nentuki = .Item("NENGETUDO_S")
            strFUNOU_YDATE = .Item("FUNOU_YDATE_S")
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Gakunen = True

    End Function
    Private Function PFUNC_Update_Schedule(ByVal pNo As Integer) As Boolean

        On Error Resume Next

        Dim iG_Flg(9) As Integer

        Dim bG_Flg As Boolean

        PFUNC_Update_Schedule = False

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Function
        End If

        STR_SQL = " SELECT "
        STR_SQL += " SFURI_DATE_S"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " WHERE"
        STR_SQL += " FURI_KBN_S ='" & STR_���k���ד��o�敪 & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & STR_���k���אU�֓� & "'"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_S = '" & STR_���k���׊w�Z�R�[�h & "'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S ='2'"
        STR_SQL += " group by SFURI_DATE_S"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            '�擾�����v���C�}�����ŒP��̃X�P�W���[�����擾
            '���W�v����X�P�W���[���̑Ώۊw�N���擾�����
            If PFUNC_Get_Gakunen2(OBJ_DATAREADER_DREAD.Item("SFURI_DATE_S"), iG_Flg) = False Then
                GoTo NextWhile
            End If

            bG_Flg = False

            '�擾�����v���C�}�����ƑΏۊw�N���ŏW�v��������
            '������ɂ���ăX�P�W���[���ɍX�V���ׂ��W�v�l���擾�ł������ƂɂȂ�
            STR_SQL = " SELECT "
            STR_SQL += " sum(1) as KENSUU"
            STR_SQL += ",sum(FURIKIN_E) as KINGAKU"
            STR_SQL += " FROM "
            STR_SQL += " G_ENTMAST" & pNo & ""
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_E = '" & STR_���k���׊w�Z�R�[�h & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_E ='" & STR_���k���אU�֓� & "'"
            '2007/10/05�@�ǉ��F�U�֋��z��0�~�̖��ׂ̓J�E���g���Ȃ�---------
            STR_SQL += " AND"
            STR_SQL += " FURIKIN_E <> 0"
            '--------------------------------------------------------------
            STR_SQL += " AND ("
            For i As Integer = 1 To 9
                If iG_Flg(i) = 1 Then
                    If bG_Flg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_E=" & i
                    bG_Flg = True
                End If
            Next i
            STR_SQL += " )"

            If GFUNC_SELECT_SQL4(STR_SQL, 0) = False Then
                If GFUNC_SELECT_SQL3("", 1) = False Then
                    Exit Function
                End If

                Exit Function
            End If

            If OBJ_DATAREADER_DREAD2.HasRows = True Then
                OBJ_DATAREADER_DREAD2.Read()

                '�X�P�W���[���}�X�^�U�փf�[�^�쐬�����X�V
                STR_SQL = " UPDATE  G_SCHMAST SET "
                STR_SQL += " DATA_DATE_S='" & Str_Syori_Date(0) & "'"
                STR_SQL += ",DATA_FLG_S='1'"
                STR_SQL += ",TIME_STAMP_S='" & Str_Syori_Date(1) & "'"
                STR_SQL += ",SYORI_KEN_S=" & CLng(OBJ_DATAREADER_DREAD2.Item("KENSUU"))
                STR_SQL += ",SYORI_KIN_S=" & CDbl(OBJ_DATAREADER_DREAD2.Item("KINGAKU"))
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_S = '" & STR_���k���׊w�Z�R�[�h & "'"
                STR_SQL += " AND"
                STR_SQL += " NENGETUDO_S = '" & Mid(STR_���k���אU�֓�, 1, 6) & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_DATE_S = '" & STR_���k���אU�֓� & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_KBN_S = '" & STR_���k���ד��o�敪 & "'"
                STR_SQL += " AND"
                STR_SQL += " SFURI_DATE_S ='" & OBJ_DATAREADER_DREAD.Item("SFURI_DATE_S") & "'"
                STR_SQL += " AND"
                STR_SQL += " SCH_KBN_S = '2'"

                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                    If GFUNC_SELECT_SQL3("", 1) = False Then
                        Exit Function
                    End If

                    Exit Function
                End If
            End If

            If GFUNC_SELECT_SQL4("", 1) = False Then
                If GFUNC_SELECT_SQL3("", 1) = False Then
                    Exit Function
                End If

                Exit Function
            End If
NextWhile:
        End While

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        PFUNC_Update_Schedule = True

    End Function
    Private Function PFUNC_DelIns_TakouSchedule(ByVal pTGakkou_Code As String, _
                                                  ByVal pTGakunen_Code As String, _
                                                  ByVal pTFurikae_kbn As String, _
                                                  ByVal pTGinko_Code As String) As Boolean

        Dim iG_Flg(1, 9) As Integer

        PFUNC_DelIns_TakouSchedule = False

        If OBJ_TRANSACTION.Connection Is Nothing Then
        Else
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
                Exit Function
            End If
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Function
        End If

        '���s�X�P�W���[���}�X�^�쐬
        STR_SQL = " INSERT INTO G_TAKOUSCHMAST "
        STR_SQL += " values ("
        STR_SQL += "'" & pTGakkou_Code & "'"
        STR_SQL += "," & pTGakunen_Code
        STR_SQL += ",'" & Str_Seikyu_Nentuki & "'"
        STR_SQL += ",'2'"
        STR_SQL += ",'" & STR_���k���ד��o�敪 & "'"
        STR_SQL += ",'" & STR_���k���אU�֓� & "'"
        STR_SQL += ",'" & strFUNOU_YDATE & "'"
        STR_SQL += ",'" & Str_Baitai_Code & "'"
        STR_SQL += ",'" & pTGinko_Code & "'"
        STR_SQL += ",'0'"
        STR_SQL += "," & lngGAK_SYORI_KEN(pTGakunen_Code)
        STR_SQL += "," & dblGAK_SYORI_KIN(pTGakunen_Code)
        STR_SQL += ",0"
        STR_SQL += ",0"
        STR_SQL += ",0"
        STR_SQL += ",0"
        STR_SQL += ",'" & Format(Now, "yyyyMMdd") & "'"
        STR_SQL += ")"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        PFUNC_DelIns_TakouSchedule = True

    End Function

    Private Function PFUNC_DAT_ZENGIN_WRITE(ByVal pFileNo As Integer, ByVal pZengin_kbn As Integer) As Boolean

        On Error Resume Next

        PFUNC_DAT_ZENGIN_WRITE = False

        Err.Number = 0

        Select Case (pZengin_kbn)
            Case 1
                Lng_RecordNo = 1
                FilePut(pFileNo, gZENGIN_REC1, Lng_RecordNo)
            Case 2
                Lng_RecordNo += 1
                FilePut(pFileNo, gZENGIN_REC2, Lng_RecordNo)
            Case 8
                Lng_RecordNo += 1
                FilePut(pFileNo, gZENGIN_REC8, Lng_RecordNo)
            Case 9
                Lng_RecordNo += 1
                FilePut(pFileNo, gZENGIN_REC9, Lng_RecordNo)
        End Select

        If Err.Number <> 0 Then

            Str_Err_Msg = Err.Description

            Exit Function
        End If

        PFUNC_DAT_ZENGIN_WRITE = True

    End Function
    Private Function PFUNC_GET_ZENGIN_LINE(ByVal pIndex As Integer) As String

        Dim sLine As String

        PFUNC_GET_ZENGIN_LINE = ""
        sLine = ""

        Select Case (pIndex)
            Case 1
                With gZENGIN_REC1
                    sLine = .ZG1 & .ZG2 & .ZG3 & .ZG4 & .ZG5 & .ZG6 & .ZG7 & .ZG8 & .ZG9 & .ZG10 & .ZG11 & .ZG12 & .ZG13
                End With
            Case 2
                With gZENGIN_REC2
                    sLine = .ZG1 & .ZG2 & .ZG3 & .ZG4 & .ZG5 & .ZG6 & .ZG7 & .ZG8 & .ZG9 & .ZG10 & .ZG11 & .ZG12 & .ZG13 & .ZG14 & .ZG15
                End With
            Case 8
                With gZENGIN_REC8
                    sLine = .ZG1 & .ZG2 & .ZG3 & .ZG4 & .ZG5 & .ZG6 & .ZG7 & .ZG8
                End With
            Case 9
                With gZENGIN_REC9
                    sLine = .ZG1 & .ZG2
                End With
        End Select

        PFUNC_GET_ZENGIN_LINE = sLine

    End Function
    Private Function PFUNC_Get_Takou_Ginko(ByRef pTakou_Ginko(,) As String) As Boolean

        Dim lCount As Long
        Dim sEntriName As String = ""

        PFUNC_Get_Takou_Ginko = False

        Select Case (STR_���k���ד��o�敪)
            Case "2"
                sEntriName = "G_ENTMAST1"
            Case "3"
                sEntriName = "G_ENTMAST2"
        End Select

        '�����ΏۂƂȂ鑼�s�ꗗ���擾
        '�G���g���}�X�^�擾(���s�̂�)
        STR_SQL = " SELECT "
        STR_SQL += " GAKKOU_CODE_V,TKIN_NO_E "
        STR_SQL += ", ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V,CODE_KBN_V"
        STR_SQL += " FROM "
        STR_SQL += " " & sEntriName & " , G_TAKOUMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_E = GAKKOU_CODE_V"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_E = TKIN_NO_V"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_E <> '" & STR_JIKINKO_CODE & "'"
        STR_SQL += " AND"
        STR_SQL += " FURIKIN_E > 0"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_E = '" & STR_���k���׊w�Z�R�[�h & "'"
        '2007/02/14 �����ɐU�֓��ǉ�
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_E = '" & STR_���k���אU�֓� & "'"
        STR_SQL += " group by GAKKOU_CODE_V,TKIN_NO_E ,  ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V,CODE_KBN_V"
        STR_SQL += " ORDER BY GAKKOU_CODE_V,TKIN_NO_E,  ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V,CODE_KBN_V"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If
            MessageBox.Show("���s���̌����Ɏ��s���܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If
            MessageBox.Show("�����f�[�^�쐬�Ώۂ̐��k�����݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        lCount = 1

        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                ReDim Preserve pTakou_Ginko(6, lCount)

                pTakou_Ginko(0, lCount) = .Item("TKIN_NO_E")
                pTakou_Ginko(1, lCount) = "" '.Item("TSIT_NO_E")
                pTakou_Ginko(2, lCount) = .Item("ITAKU_CODE_V")
                pTakou_Ginko(3, lCount) = .Item("KAMOKU_V")
                pTakou_Ginko(4, lCount) = .Item("KOUZA_V")
                If IsDBNull(.Item("SFILE_NAME_V")) = True Then
                    'S+�w�Z�R�[�h�{���Z�@�փR�[�h+.dat
                    pTakou_Ginko(5, lCount) = "S" & CStr(.Item("GAKKOU_CODE_V")).Trim & CStr(.Item("TKIN_NO_E")).Trim & ".dat"
                Else
                    If CStr(.Item("SFILE_NAME_V")).Trim = "" Then
                        pTakou_Ginko(5, lCount) = "S" & CStr(.Item("GAKKOU_CODE_V")).Trim & CStr(.Item("TKIN_NO_E")).Trim & ".dat"
                    Else
                        pTakou_Ginko(5, lCount) = CStr(.Item("SFILE_NAME_V")).Trim
                    End If
                End If
                '�R�[�h�敪�ǉ� 2006/10/16
                pTakou_Ginko(6, lCount) = .Item("CODE_KBN_V")

                lCount += 1
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Takou_Ginko = True

    End Function
    Private Function PFUNC_Chk_Meisai() As Integer

        Dim Chk_Sql As String

        PFUNC_Chk_Meisai = -1

        '���Z�@�֖����̓`�F�b�N
        If Trim(Str_Err_Tkin_No) = "" Or Trim(Str_Err_Tsit_No) = "" Then
            Str_Err_Msg = "�ϑ����Z�@�ւ������͂ł��B"

            Exit Function
        Else
            '���݃`�F�b�N
            Chk_Sql = " SELECT * FROM TENMAST"
            Chk_Sql += " WHERE"
            Chk_Sql += " KIN_NO_N = '" & Str_Err_Tkin_No & "'"
            Chk_Sql += " AND"
            Chk_Sql += " SIT_NO_N = '" & Str_Err_Tsit_No & "'"

            If GFUNC_SELECT_SQL5(Chk_Sql, 0) = False Then
                Exit Function
            End If

            If OBJ_DATAREADER_DREAD3.HasRows = False Then
                If GFUNC_SELECT_SQL5("", 1) = False Then
                    Exit Function
                End If

                PFUNC_Chk_Meisai = 1

                Str_Err_Msg = "���Z�@�փ}�X�^�ɓo�^����Ă��܂���B"

                Exit Function
            End If

            If GFUNC_SELECT_SQL5("", 1) = False Then
                Exit Function
            End If
        End If

        '�����ԍ��K�茅�`�F�b�N
        Select Case (Len(Trim(Str_Err_Kouza)))
            Case Is <> 7
                Str_Err_Msg = "�����ԍ��̌����V���ȊO�ł�"

                Exit Function
        End Select

        '�����ԍ�����ALLZERO , ALL9 �`�F�b�N
        '
        Select Case (Trim(Str_Err_Kouza))
            Case "0000000"
                Str_Err_Msg = "�����ԍ���ALLZERO�l�͐ݒ�ł��܂���"

                Exit Function
            Case "9999999999"
                Str_Err_Msg = "�����ԍ���ALL9�l�͐ݒ�ł��܂���"

                Exit Function
        End Select

        PFUNC_Chk_Meisai = 0

    End Function

    Private Function PFUNC_Get_Gakunen2(ByVal pSFuri_Date As String, _
                                        ByRef pSiyou_gakunen() As Integer) As Boolean

        Dim iMaxGakunen As Integer

        ReDim pSiyou_gakunen(9)

        PFUNC_Get_Gakunen2 = False

        '�I�����ꂽ�w�Z�ɑ��݂���X�P�W���[�����
        '�ĐU�֓��܂ł𒊏o�����Ƃ��A
        '�X�P�W���[�����ɏW�v��������w�N���擾����
        STR_SQL = " SELECT "
        STR_SQL += " SCH_KBN_S"
        For iLoopCount As Integer = 1 To 9
            STR_SQL += ", GAKUNEN" & iLoopCount & "_FLG_S"
            pSiyou_gakunen(iLoopCount) = 0
        Next iLoopCount
        STR_SQL += ", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " left join GAKMAST2 on "
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_S ='" & STR_���k���׊w�Z�R�[�h & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & STR_���k���אU�֓� & "'"
        STR_SQL += " AND"
        STR_SQL += " SFURI_DATE_S ='" & pSFuri_Date & "'"
        STR_SQL += " AND"
        STR_SQL += " CHECK_FLG_S ='1'"
        STR_SQL += " AND"
        STR_SQL += " TYUUDAN_FLG_S ='0'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S = '2'"

        If GFUNC_SELECT_SQL4(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD2.HasRows = False Then
            If GFUNC_SELECT_SQL4("", 1) = False Then
                Exit Function
            End If

            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD2.Read)
            With OBJ_DATAREADER_DREAD2
                iMaxGakunen = CInt(.Item("SIYOU_GAKUNEN_T"))
                For iLoopCount As Integer = 1 To iMaxGakunen
                    Select Case (CInt(.Item("GAKUNEN" & iLoopCount & "_FLG_S")))
                        Case 1
                            pSiyou_gakunen(iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                    End Select
                Next iLoopCount
            End With
        End While

        If GFUNC_SELECT_SQL4("", 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Gakunen2 = True

    End Function

    Private Function PFUNC_Query_String(ByVal astrKINKO_CD As String) As String

        Dim sQuery As String

        PFUNC_Query_String = ""

        sQuery = "SELECT TENMAST.SIT_NO_N, GAKMAST1.GAKKOU_CODE_G, GAKMAST1.GAKKOU_NNAME_G, HIMOMAST.HIMOKU_NAME01_H, HIMOMAST.HIMOKU_NAME02_H, HIMOMAST.HIMOKU_NAME04_H, HIMOMAST.HIMOKU_NAME06_H, HIMOMAST.HIMOKU_NAME07_H, HIMOMAST.HIMOKU_NAME08_H, HIMOMAST.HIMOKU_NAME09_H, HIMOMAST.HIMOKU_NAME10_H, HIMOMAST.HIMOKU_NAME11_H, HIMOMAST.HIMOKU_NAME12_H, HIMOMAST.HIMOKU_NAME13_H, HIMOMAST.HIMOKU_NAME14_H, HIMOMAST.HIMOKU_NAME15_H, HIMOMAST.HIMOKU_NAME03_H, HIMOMAST.HIMOKU_NAME05_H, TENMAST.KIN_NO_N, G_SCHMAST.FURI_DATE_S, SEITOMASTVIEW.GAKUNEN_CODE_O, SEITOMASTVIEW.SEITO_NO_O, SEITOMASTVIEW.SEITO_KNAME_O, SEITOMASTVIEW.SEITO_NNAME_O, SEITOMASTVIEW.MEIGI_KNAME_O, TENMAST.SIT_NNAME_N, SEITOMASTVIEW.TYOUSI_FLG_O, G_SCHMAST.FUNOU_FLG_S, G_MEIMAST.TKAMOKU_M, G_MEIMAST.GAKKOU_CODE_M, G_MEIMAST.GAKUNEN_CODE_M, G_MEIMAST.HIMOKU1_KIN_M, G_MEIMAST.HIMOKU2_KIN_M, G_MEIMAST.HIMOKU3_KIN_M, G_MEIMAST.HIMOKU4_KIN_M, G_MEIMAST.HIMOKU5_KIN_M, G_MEIMAST.HIMOKU6_KIN_M, G_MEIMAST.HIMOKU7_KIN_M, G_MEIMAST.HIMOKU8_KIN_M, G_MEIMAST.HIMOKU9_KIN_M, G_MEIMAST.HIMOKU10_KIN_M, G_MEIMAST.HIMOKU11_KIN_M, G_MEIMAST.HIMOKU12_KIN_M, G_MEIMAST.HIMOKU13_KIN_M, G_MEIMAST.HIMOKU14_KIN_M, G_MEIMAST.HIMOKU15_KIN_M, G_MEIMAST.TUUBAN_M, G_MEIMAST.NENDO_M, G_MEIMAST.CLASS_CODE_M, G_MEIMAST.SEITO_NO_M, G_MEIMAST.TKOUZA_M FROM   KZFMAST.G_MEIMAST G_MEIMAST, KZFMAST.SEITOMASTVIEW SEITOMASTVIEW, KZFMAST.G_SCHMAST G_SCHMAST, KZFMAST.HIMOMAST HIMOMAST, KZFMAST.GAKMAST1 GAKMAST1, KZFMAST.TENMAST TENMAST "

        sQuery += " WHERE"
        sQuery += " ((((((G_MEIMAST.GAKKOU_CODE_M=SEITOMASTVIEW.GAKKOU_CODE_O) AND (G_MEIMAST.NENDO_M=SEITOMASTVIEW.NENDO_O)) AND (G_MEIMAST.GAKUNEN_CODE_M=SEITOMASTVIEW.GAKUNEN_CODE_O)) AND (G_MEIMAST.CLASS_CODE_M=SEITOMASTVIEW.CLASS_CODE_O)) AND (G_MEIMAST.SEITO_NO_M=SEITOMASTVIEW.SEITO_NO_O)) AND (G_MEIMAST.TUUBAN_M=SEITOMASTVIEW.TUUBAN_O)) AND (((G_MEIMAST.GAKKOU_CODE_M=G_SCHMAST.GAKKOU_CODE_S) AND (G_MEIMAST.FURI_DATE_M=G_SCHMAST.FURI_DATE_S)) AND (G_MEIMAST.FURI_KBN_M=G_SCHMAST.FURI_KBN_S)) AND ((((SEITOMASTVIEW.GAKKOU_CODE_O=HIMOMAST.GAKKOU_CODE_H (+)) AND (SEITOMASTVIEW.GAKUNEN_CODE_O=HIMOMAST.GAKUNEN_CODE_H (+))) AND (SEITOMASTVIEW.HIMOKU_ID_O=HIMOMAST.HIMOKU_ID_H (+))) AND (SEITOMASTVIEW.TUKI_NO_O=HIMOMAST.TUKI_NO_H (+))) AND ((SEITOMASTVIEW.GAKKOU_CODE_O=GAKMAST1.GAKKOU_CODE_G (+)) AND (SEITOMASTVIEW.GAKUNEN_CODE_O=GAKMAST1.GAKUNEN_CODE_G (+))) AND ((SEITOMASTVIEW.TKIN_NO_O=TENMAST.KIN_NO_N (+)) AND (SEITOMASTVIEW.TSIT_NO_O=TENMAST.SIT_NO_N (+))) "

        sQuery += " AND"
        sQuery += " G_MEIMAST.GAKKOU_CODE_M = '" & STR_���k���׊w�Z�R�[�h & "'"
        sQuery += " AND"
        sQuery += " G_MEIMAST.YOBI1_M = '" & Str_Syori_Date(1) & "'"
        sQuery += " AND"
        sQuery += " SEITOMASTVIEW.TUKI_NO_O = '" & Mid(Trim(STR_���k���אU�֓�), 5, 2) & "'"
        sQuery += " AND"
        sQuery += " ((G_MEIMAST.GAKUNEN_CODE_M=1 AND G_SCHMAST.GAKUNEN1_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=2 AND G_SCHMAST.GAKUNEN2_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=3 AND G_SCHMAST.GAKUNEN3_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=4 AND G_SCHMAST.GAKUNEN4_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=5 AND G_SCHMAST.GAKUNEN5_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=6 AND G_SCHMAST.GAKUNEN6_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=7 AND G_SCHMAST.GAKUNEN7_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=8 AND G_SCHMAST.GAKUNEN8_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=9 AND G_SCHMAST.GAKUNEN9_FLG_S='1'))"
        ''���ɂ��Ƃɏo��2006/10/17�����S�͎��s�E���s�ꏏ�ɏo��
        'sQuery += " AND"
        'sQuery += " G_MEIMAST.TKIN_NO_M = '" & astrKINKO_CD & "'"

        sQuery += " ORDER BY"
        If STR_���k���׈���敪 = "2" Then
            sQuery += " G_MEIMAST.TKIN_NO_M , G_MEIMAST.TSIT_NO_M , "
        End If

        Select Case (intPRNT_SORT) '�w�Z�}�X�^�ɓo�^���ꂽ�u���[�\�[�g���v�Ŏw�� 2006/10/18
            Case 0
                '�w�N,�N���X,���k�ԍ�
                sQuery += " G_MEIMAST.GAKUNEN_CODE_M ASC, G_MEIMAST.CLASS_CODE_M ASC, G_MEIMAST.SEITO_NO_M ASC, G_MEIMAST.NENDO_M ASC, G_MEIMAST.TUUBAN_M ASC"
            Case 1
                '���w�N�x,�ʔ�
                sQuery += " G_MEIMAST.GAKUNEN_CODE_M ASC, G_MEIMAST.NENDO_M ASC, G_MEIMAST.TUUBAN_M ASC"
            Case 2
                '����������(���k��(��))
                sQuery += " G_MEIMAST.GAKUNEN_CODE_M ASC, SEITOMASTVIEW.SEITO_KNAME_O ASC, G_MEIMAST.NENDO_M ASC, G_MEIMAST.TUUBAN_M ASC"
        End Select

        PFUNC_Query_String = sQuery

    End Function


    Public Function GFUNC_FD_Copy3(ByVal pForm As Form, _
                              ByVal pTitleName As String, _
                              ByVal pSouceFilePath As String, _
                              ByVal pInitialFileName As String, _
                              ByVal pBaitai As String) As Integer
        '2005/03/14
        Dim sPath As String = ""
        Dim sBuff As String = ""

        Dim oDlg As New SaveFileDialog

        On Error Resume Next

        GFUNC_FD_Copy3 = -1

        '--------------------
        'FD�ۑ�
        '--------------------
        '--------------------
        '�U�փf�[�^�ۑ���p�X
        '--------------------
        ' 2017/06/07 �^�X�N�j���� CHG (RSV2�W���Ή� �}�̃R�[�h����) -------------------- START
        'Select Case (pBaitai)
        '    Case "0"
        '        sPath = STR_IFL_PATH
        '    Case "1"
        '        sPath = "A:\"
        'End Select
        Select Case pBaitai
            Case "1"
                sPath = "A:\"
            Case Else
                sPath = STR_IFL_PATH
        End Select
        ' 2017/06/07 �^�X�N�j���� CHG (RSV2�W���Ή� �}�̃R�[�h����) -------------------- END

        If sPath = "" Then
            'ini�t�@�C����TAKIRAIFL�i�[���񂪐ݒ肳��Ă��܂���
            MessageBox.Show(String.Format(MSG0001E, "�˗��t�@�C���i�[��", "GCOMMON", "IRAIFL"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        Select Case (StrConv(Mid(sPath, 1, 1), vbProperCase))
            Case "A", "B"
                '�e�c�ǂݎ�菈��
                '�m�F���b�Z�[�W�\��
                If MessageBox.Show(G_MSG0002I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> Windows.Forms.DialogResult.OK Then
                    GFUNC_FD_Copy3 = 1
                    Exit Function
                End If
        End Select
        Select Case (StrConv(Mid(sPath, 1, 1), vbProperCase))
            Case "A", "B"

                With oDlg
                    .Filter = STR_DLG_FILTER_NAME & " (" & STR_DLG_FILTER & ")|" & STR_DLG_FILTER


                    .FilterIndex = 1
                    .InitialDirectory = sPath

                    .DefaultExt = STR_DEF_FILE_KBN
                    If Trim(pInitialFileName) <> "" Then
                        .FileName = pInitialFileName
                    End If
                    .Title = "[" & pTitleName & "]�U�փf�[�^�ۑ�"
                    .ShowDialog()
                    sBuff = .FileName
                End With


                If sBuff = pInitialFileName Or Err.Number <> 0 Then
                    '�L�����Z���̂Ƃ��́A�Z�b�g�����t�@�C�����̂ݕԂ�����
                    GFUNC_FD_Copy3 = 1
                    Exit Function
                End If

                If Dir(sBuff, vbNormal) <> "" Then Kill(sBuff)

                FileCopy(pSouceFilePath, sBuff)

                If Err.Number <> 0 Then
                    '�t�@�C���ۑ����s
                    Exit Function
                End If
            Case Else

                Select Case (CInt(STR_���k���ד��o�敪))
                    Case 2 '����
                        sBuff = sPath & "G" & STR_���k���׊w�Z�R�[�h & "03.dat"
                    Case 3 '�Վ��o��
                        sBuff = sPath & "G" & STR_���k���׊w�Z�R�[�h & "04.dat"
                End Select

                FileCopy(pSouceFilePath, sBuff)
                If Err.Number <> 0 Then
                    '�t�@�C���ۑ����s
                    Exit Function
                End If
        End Select

        GFUNC_FD_Copy3 = 0

    End Function

#End Region

    Private colNo, rowNo As Integer
    Private TextEditCtrl As DataGridViewTextBoxEditingControl

    Private Sub CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        colNo = e.ColumnIndex
        rowNo = e.RowIndex

        CType(sender, DataGridView).ImeMode = ImeMode.Disable
    End Sub
    Private Sub CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        '�X�v���b�g�����ڑOZERO�l��
        With CType(sender, DataGridView)
            Select Case colNo
                Case 17
                    Dim str_Value As String
                    str_Value = .Rows(e.RowIndex).Cells(e.ColumnIndex).Value
                    If Not str_Value Is Nothing Then
                        If IsNumeric(str_Value) Then
                            .Rows(e.RowIndex).Cells(e.ColumnIndex).Value = Format(CDec(str_Value), "#,##0")
                        End If
                    End If
            End Select
        End With

    End Sub
    Private Sub EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs)
        TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)

        Select Case colNo
            Case 17
                AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
        End Select
    End Sub
    Private Sub CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Select Case colNo
            Case 17
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
        End Select

        Call CellLeave(sender, e)
    End Sub

    Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DataGridView.RowPostPaint
        Dim dgv As DataGridView = CType(sender, DataGridView)

        ' �s�w�b�_�̃Z���̈���A�s�ԍ���`�悷�钷���`�Ƃ���
        ' �i�������E�[��4�h�b�g�̂����Ԃ��󂯂�j
        Dim rect As New Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, dgv.Rows(e.RowIndex).Height)

        ' ��L�̒����`���ɍs�ԍ����c�����������E�l�ŕ`�悷��
        ' �t�H���g��F�͍s�w�b�_�̊���l���g�p����
        TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, _
                              rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter _
                              Or TextFormatFlags.Right)

    End Sub

End Class
