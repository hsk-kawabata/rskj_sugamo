Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Imports Microsoft.VisualBasic
Public Class KFGPRNT120
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' �����`�F�b�N���X�g���
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " ���ʕϐ���` "
    Private STR�w�Z�� As String
    Dim strKIGYO_CODE As String
    Dim strFURI_CODE As String
    Dim strGAKKOU_CODE As String
    Dim strKIN_NO As String
    Dim strSIT_NO As String
    Dim strKAMOKU As String
    Dim strKOUZA As String
    Dim strKNAME As String
    Dim strReKNAME As String
    Dim intKEKKA As Integer
    Dim STR���[�\�[�g�� As String
    Dim Str_Kubn As String
    Dim STR�w�Z�R�[�h As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT120", "�w�Z���U�����`�F�b�N���X�g������")
    Private Const msgTitle As String = "�w�Z���U�����`�F�b�N���X�g������(KFGPRNT120)"
    Private MainDB As CASTCommon.MyOracle   '�p�u���b�N�f�[�^�[�x�[�X

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGPRNT120_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")
                MessageBox.Show("�w�Z���R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '���̓{�^������
            btnPrnt.Enabled = True
            btnEnd.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Try
            '����{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            MainDB = New MyOracle

            '�w�Z�R�[�h�K�{�`�F�b�N
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If

            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab�w�Z��.Text = "�S�w�Z�Ώ�"
            End If

            LW.ToriCode = Trim(txtGAKKOU_CODE.Text)
            '--------------------------------
            'PR_KOUZAMAST�̃f�[�^���폜����
            '--------------------------------
            MainDB.BeginTrans()
            If fn_DELETE_PR_KOUZAMAST() = False Then
                MainDB.Rollback()
                Exit Sub
            End If

            '--------------------------------------------------------------------------
            '�����`�F�b�N�����s���A�`�F�b�N�Ɉ��������������ׂ�PR_KOUZAMAST�ɃC���T�[�g����
            '--------------------------------------------------------------------------
            If fn_KOUZA_CHK_MAIN() = False Then
                MainDB.Rollback()
                Return
            End If
            MainDB.Commit()
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT DISTINCT GAKKOU_CODE_P")
            SQL.Append(" FROM PR_KOUZAMAST")
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                SQL.Append(" WHERE GAKKOU_CODE_P = " & SQ(txtGAKKOU_CODE.Text))
            End If
            SQL.Append(" ORDER BY GAKKOU_CODE_P")

            '����Ώۑ��݃`�F�b�N
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("�Ώۃf�[�^�����݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0013I, "�w�Z���U�����`�F�b�N���X�g"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            While OraReader.EOF = False
                STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_P")
                '���[�\�[�g���̎擾
                If PFUNC_GAKNAME_GET(False) = False Then
                    STR���[�\�[�g�� = "0"
                End If

                '���O�C��ID,�w�Z�R�[�h,���[�\�[�g��
                Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR���[�\�[�g��
                nRet = ExeRepo.ExecReport("KFGP031.EXE", Param)
                '�߂�l�ɑΉ��������b�Z�[�W��\������
                Select Case nRet
                    Case 0
                    Case Else
                        '������s���b�Z�[�W
                        MessageBox.Show(String.Format(MSG0004E, "�w�Z���U�����`�F�b�N���X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                OraReader.NextRead()
            End While
            txtGAKKOU_CODE.Focus()
            MessageBox.Show(String.Format(MSG0014I, "�w�Z���U�����`�F�b�N���X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.ToString)
            MainDB.Rollback()
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            If OraReader IsNot Nothing Then OraReader.Close()
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET(Optional ByVal NameChg As Boolean = True) As Boolean

        '�w�Z���̐ݒ�
        PFUNC_GAKNAME_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            If Trim(STR�w�Z�R�[�h) = "9999999999" Then
                lab�w�Z��.Text = ""
            Else
                SQL.Append(" SELECT ")
                SQL.Append(" GAKMAST1.*")
                SQL.Append(",MEISAI_OUT_T")
                SQL.Append(" FROM GAKMAST1,GAKMAST2")
                SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T")
                SQL.Append(" AND GAKKOU_CODE_G = " & SQ(STR�w�Z�R�[�h))

                If OraReader.DataReader(SQL) = False Then

                    MessageBox.Show("�w�Z�}�X�^�ɓo�^����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    lab�w�Z��.Text = ""
                    STR���[�\�[�g�� = 0

                    Exit Function
                End If

                If NameChg Then lab�w�Z��.Text = OraReader.GetString("GAKKOU_NNAME_G")
                STR���[�\�[�g�� = OraReader.GetInt("MEISAI_OUT_T")

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�Z����)", "���s", ex.ToString)
            Return False
        Finally
            If Not OraDB Is Nothing Then OraDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        PFUNC_GAKNAME_GET = True

    End Function

    Function fn_KOUZA_CHK_MAIN() As Boolean
        '-------------------------------------------
        '�Ώۂ̊w�Z�̊�ƃR�[�h�A�U�փR�[�h���擾
        '-------------------------------------------
        fn_KOUZA_CHK_MAIN = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraReader2 As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            OraReader2 = New MyOracleReader(MainDB)

            SQL.Append("SELECT KIGYO_CODE_T,FURI_CODE_T,GAKKOU_CODE_T")
            SQL.Append(" FROM GAKMAST2")
            If txtGAKKOU_CODE.Text = "9999999999" Then
                SQL.Append(" ORDER BY GAKKOU_CODE_T")
            Else
                SQL.Append(" WHERE GAKKOU_CODE_T = " & SQ(txtGAKKOU_CODE.Text))
            End If

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If

            While OraReader.EOF = False
                strKIGYO_CODE = Trim(OraReader.GetString("KIGYO_CODE_T"))
                strFURI_CODE = Trim(OraReader.GetString("FURI_CODE_T"))
                strGAKKOU_CODE = OraReader.GetString("GAKKOU_CODE_T")
                '-------------------------------------------
                '�����`�F�b�N�����s
                '-------------------------------------------
                SQL = New StringBuilder
                SQL.Append(" SELECT *")
                SQL.Append(" FROM SEITOMAST")
                SQL.Append(" WHERE GAKKOU_CODE_O = " & SQ(strGAKKOU_CODE))
                SQL.Append(" AND  TUKI_NO_O = '01'")
                SQL.Append(" ORDER BY NENDO_O,TUUBAN_O")

                If OraReader2.DataReader(SQL) = True Then
                    While OraReader2.EOF = False
                        strKIN_NO = OraReader2.GetString("TKIN_NO_O")
                        strSIT_NO = OraReader2.GetString("TSIT_NO_O")
                        strKAMOKU = OraReader2.GetString("KAMOKU_O")
                        strKOUZA = OraReader2.GetString("KOUZA_O")
                        strKNAME = Trim(OraReader2.GetString("MEIGI_KNAME_O"))

                        '�����ɃR�[�h�̏ꍇ�̂݌����`�F�b�N���s
                        If strKIN_NO = STR_JIKINKO_CODE Then
                            If fn_KOUZA_CHK(strKIGYO_CODE, strFURI_CODE, strKIN_NO, strSIT_NO, strKAMOKU, strKOUZA, _
                                            strKNAME, strReKNAME, intKEKKA) = False Then
                                Return False
                            End If
                            '-------------------------------------------------------
                            '�����ُ�̃f�[�^�͌����`�F�b�N���X�g�}�X�^�ɃC���T�[�g����
                            '-------------------------------------------------------
                            If intKEKKA <> 0 Then
                                If fn_INSERT_PR_KOUZAMAST(OraReader2) = False Then
                                    Return False
                                End If
                            End If
                        End If
                        OraReader2.NextRead()
                    End While
                Else '���ׂ��Ȃ��ꍇ�͎��̊w�Z����������
                End If
                OraReader.NextRead()
            End While
            fn_KOUZA_CHK_MAIN = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�Z����)", "���s", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not OraReader2 Is Nothing Then OraReader2.Close()
        End Try

    End Function

    Function fn_KOUZA_CHK(ByVal astrKIGYO_CODE As String, ByVal astrFURI_CODE As String, ByVal astrKIN_NO As String, ByVal astrSIT_NO As String, _
                          ByVal astrKAMOKU As String, ByVal astrKOUZA As String, ByVal astrKNAME As String, ByRef astrReKNAME As String, _
                          ByRef aintKEKKA As Integer) As Boolean
        '============================================================================
        'NAME           :fn_KOUZA_CHK
        'Parameter      :astrKIGYO_CODE�F��ƃR�[�h�^astrFURI_CODE�F�U�փR�[�h�^astrKIN_NO�F���Z�@�փR�[�h
        '               :astrSIT_NO�F�x�X�R�[�h�^astrKAMOKU�F�ȖڃR�[�h�^astrKOUZA�F�����ԍ�
        '�@�@�@�@�@�@�@�@:astrKNAME: �J�i�����^astrReKNAME�F�����J�i�����i��v���Ȃ������ꍇ�l��Ԃ��j
        '�@�@�@�@�@�@�@�@:aintKEKKA: �����`�F�b�N�̌���(0=���U�_�񂠂�J�i��v,1=�_�񂠂�J�i�s��v,
        '�@�@�@�@�@�@�@�@:�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@2=�_��Ȃ��J�i��v,3=�_��Ȃ��J�i�s��v,4=�����Ȃ�)
        'Description    :�����`�F�b�N�̎��s
        'Return         :True=OK(����),False=NG�i���s�j
        'Create         :2005/04/14
        'UPDATE         :
        '============================================================================
        fn_KOUZA_CHK = False
        aintKEKKA = 0
        astrReKNAME = ""

        ' 2017/05/09 �^�X�N�j���� ADD �yOM�z(RSV2�Ή� �@�\�ǉ�) -------------------- START
        Dim RSKJ_G_KNAMECHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_KNAMECHK")
        Dim RSKJ_G_JIFURICHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_JIFURICHK")
        ' 2017/05/09 �^�X�N�j���� ADD �yOM�z(RSV2�Ή� �@�\�ǉ�) -------------------- END

        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            ' 2017/05/09 �^�X�N�j���� CHG �yOM�z(RSV2�Ή� �@�\�ǉ�) -------------------- START
            'OraReader = New MyOracleReader(MainDB)
            ''��ƃR�[�h�A�U�փR�[�h����v������̂����邩�ǂ�������
            'SQL.Append(" SELECT * FROM KDBMAST")
            'SQL.Append(" WHERE TSIT_NO_D =" & SQ((Trim(astrSIT_NO))))
            'SQL.Append(" AND KAMOKU_D =" & SQ(astrKAMOKU))
            'SQL.Append(" AND KOUZA_D =" & SQ(Trim(astrKOUZA)))
            'SQL.Append(" AND KIGYOU_CODE_D =" & SQ(astrKIGYO_CODE))
            'SQL.Append(" AND FURI_CODE_D =" & SQ(astrFURI_CODE))

            'If OraReader.DataReader(SQL) = True Then
            '    If Trim(astrKNAME) = Trim(OraReader.GetString("KOKYAKU_KNAME_D")) Then
            '        aintKEKKA = 0
            '    Else
            '        aintKEKKA = 1
            '        astrReKNAME = Trim(OraReader.GetString("KOKYAKU_KNAME_D"))
            '    End If
            '    Return True
            'End If

            ''��ƃR�[�h�A�U�փR�[�h����v������̂��Ȃ������Ƃ��A��������v������̂����邩����
            'SQL = New StringBuilder
            'SQL.Append(" SELECT * FROM KDBMAST")
            'SQL.Append(" WHERE TSIT_NO_D =" & SQ(Trim(astrSIT_NO)))
            'SQL.Append(" AND KAMOKU_D =" & SQ(astrKAMOKU))
            'SQL.Append(" AND KOUZA_D =" & SQ(Trim(astrKOUZA)))

            'If OraReader.DataReader(SQL) = True Then
            '    If Mid(Trim(astrKNAME), 1, 40) = Mid(Trim(OraReader.GetString("KOKYAKU_KNAME_D")), 1, 40) Then
            '        aintKEKKA = 2
            '    Else
            '        aintKEKKA = 3
            '        astrReKNAME = Mid(Trim(OraReader.GetString("KOKYAKU_KNAME_D")), 1, 40)
            '    End If
            'Else
            '    aintKEKKA = 4
            'End If
            'fn_KOUZA_CHK = True
            '-------------------------------------------------------------------------
            ' ���������݂��邩�ǂ����m�F���A�e�����擾����
            '-------------------------------------------------------------------------
            OraReader = New MyOracleReader(MainDB)
            SQL.Length = 0
            SQL.Append(" SELECT * FROM KDBMAST")
            SQL.Append(" WHERE ")
            SQL.Append("     TSIT_NO_D =" & SQ(Trim(astrSIT_NO)))
            SQL.Append(" AND KAMOKU_D  =" & SQ(astrKAMOKU))
            SQL.Append(" AND KOUZA_D   =" & SQ(Trim(astrKOUZA)))
            '2017/06/23 saitou ADD �W���ŏC�� ---------------------------------------- START
            '�������ς݂��o�͂���Ȃ����ߊ������t���O�Ń\�[�g����
            SQL.Append(" ORDER BY KATU_KOUZA_D DESC")
            '2017/06/23 saitou ADD �W���ŏC�� ---------------------------------------- END

            '-------------------------------------------------------------
            ' �����l�ݒ�[0:���U�_�񂠂�(�����[�o�͂Ȃ�)]
            '-------------------------------------------------------------
            aintKEKKA = 0

            If OraReader.DataReader(SQL) = True Then
                '2017/06/23 saitou UPD �W���ŏC�� ---------------------------------------- START
                '�������ς݂̃`�F�b�N�R��ƌ����������Ă����[�v���Ă��Ȃ��̂ŁA
                '�S�Ď��U�_��Ȃ��ɂȂ��Ă��܂��̂��C���B
                Dim KigyoCode As String = ""
                Dim FuriCode As String = ""
                Dim KDBKName As String = ""

                If OraReader.GetString("KATU_KOUZA_D") = "0" Then
                    ' [8:�������ς�]
                    aintKEKKA = 8
                Else
                    While OraReader.EOF = False
                        KigyoCode = OraReader.GetString("KIGYOU_CODE_D")
                        FuriCode = OraReader.GetString("FURI_CODE_D")
                        KDBKName = OraReader.GetString("KOKYAKU_KNAME_D")

                        If RSKJ_G_JIFURICHK = "YES" Then
                            '-------------------------------------------------------------
                            ' ���U�_��i��ƃR�[�h�E�U�փR�[�h�j�̃`�F�b�N���s��
                            '-------------------------------------------------------------
                            If astrKIGYO_CODE = KigyoCode And _
                                       astrFURI_CODE = FuriCode Then
                                aintKEKKA = 0
                                Exit While
                            End If

                            ' [1:���U�_��Ȃ�]
                            aintKEKKA = 1
                        Else
                            Exit While
                        End If

                        OraReader.NextRead()
                    End While

                    If RSKJ_G_KNAMECHK = "YES" Then
                        '-----------------------------------------------------
                        ' �J�i���`�F�b�N���s��
                        '-----------------------------------------------------
                        If Trim(astrKNAME) <> Trim(KDBKName) Then
                            Select Case aintKEKKA
                                Case 1
                                    ' [2:���U�_��Ȃ��J�i�s��v]
                                    aintKEKKA = 2
                                    astrReKNAME = Trim(KDBKName)
                                Case Else
                                    ' [3:���U�_�񂠂�J�i�s��v]
                                    aintKEKKA = 3
                                    astrReKNAME = Trim(KDBKName)
                            End Select
                        End If
                    End If
                End If

                'Select Case RSKJ_G_JIFURICHK
                '    Case "YES"
                '        '-------------------------------------------------------------
                '        ' ���U�_��i��ƃR�[�h�E�U�փR�[�h�j�̃`�F�b�N���s��
                '        '-------------------------------------------------------------
                '        Dim KigyoCode As String = OraReader.GetString("KIGYOU_CODE_D")
                '        Dim FuriCode As String = OraReader.GetString("FURI_CODE_D")
                '        If astrKIGYO_CODE = KigyoCode And _
                '                   astrFURI_CODE = FuriCode Then
                '        Else
                '            ' [1:���U�_��Ȃ�]
                '            aintKEKKA = 1
                '        End If

                '        Select Case RSKJ_G_KNAMECHK
                '            Case "YES"
                '                '-----------------------------------------------------
                '                ' �J�i���`�F�b�N���s��
                '                '-----------------------------------------------------
                '                Dim KDBKName As String = OraReader.GetString("KOKYAKU_KNAME_D")
                '                If Trim(astrKNAME) <> Trim(KDBKName) Then
                '                    Select Case aintKEKKA
                '                        Case 1
                '                            ' [2:���U�_��Ȃ��J�i�s��v]
                '                            aintKEKKA = 2
                '                            astrReKNAME = Trim(KDBKName)
                '                        Case Else
                '                            ' [3:���U�_�񂠂�J�i�s��v]
                '                            aintKEKKA = 3
                '                            astrReKNAME = Trim(KDBKName)
                '                    End Select
                '                End If
                '        End Select
                '    Case Else
                '        '-------------------------------------------------------------
                '        ' ���U�_��i��ƃR�[�h�E�U�փR�[�h�j�̃`�F�b�N���s��Ȃ�
                '        '-------------------------------------------------------------
                '        Select Case RSKJ_G_KNAMECHK
                '            Case "YES"
                '                '-----------------------------------------------------
                '                ' �J�i���`�F�b�N���s��
                '                '-----------------------------------------------------
                '                Dim KDBKName As String = OraReader.GetString("KOKYAKU_KNAME_D")
                '                If Trim(astrKNAME) <> Trim(KDBKName) Then
                '                    ' [4:�J�i�s��v]
                '                    aintKEKKA = 4
                '                    astrReKNAME = Trim(KDBKName)
                '                End If
                '        End Select
                'End Select
                '2017/06/23 saitou UPD �W���ŏC�� ---------------------------------------- END
            Else
                '-----------------------------------------------------
                ' �����Y���Ȃ��̏ꍇ�́A9(�����Ȃ�)
                '-----------------------------------------------------
                aintKEKKA = 9
            End If

            Return True
            ' 2017/05/09 �^�X�N�j���� CHG �yOM�z(RSV2�Ή� �@�\�ǉ�) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����`�F�b�N)", "���s", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function
    Function fn_INSERT_PR_KOUZAMAST(ByVal WorkReader As MyOracleReader) As Boolean
        '============================================================================
        'NAME           :fn_INSERT_PR_KOUZAMAST
        'Parameter      :
        'Description    :�����`�F�b�N���X�g�}�X�^�ւ̃C���T�[�g
        'Return         :True=OK(����),False=NG�i���s�j
        'Create         :2005/04/14
        'UPDATE         :
        '============================================================================
        fn_INSERT_PR_KOUZAMAST = False

        Dim SQL As New StringBuilder
        Try
            SQL.Append(" INSERT INTO PR_KOUZAMAST VALUES(")
            SQL.Append(SQ(strGAKKOU_CODE))                              '�w�Z�R�[�h
            SQL.Append("," & WorkReader.GetString("NENDO_O"))           '���w�N�x
            SQL.Append("," & WorkReader.GetString("TUUBAN_O"))          '�ʔ�
            SQL.Append("," & WorkReader.GetString("GAKUNEN_CODE_O"))    '�w�N
            SQL.Append("," & WorkReader.GetString("CLASS_CODE_O"))      '�N���X
            SQL.Append("," & SQ(WorkReader.GetString("SEITO_NO_O")))    '���k�ԍ�
            SQL.Append("," & SQ(strKIN_NO))                             '���Z�@�փR�[�h
            SQL.Append("," & SQ(strSIT_NO))                             '�x�X�R�[�h
            SQL.Append("," & SQ(strKAMOKU))                             '�ȖڃR�[�h
            SQL.Append("," & SQ(strKOUZA))                              '�����ԍ�
            SQL.Append("," & SQ(strKNAME))                              '���`�l�J�i����
            SQL.Append("," & SQ(strReKNAME))                            '�����J�i����
            SQL.Append("," & SQ(strKIGYO_CODE))                         '��ƃR�[�h
            SQL.Append("," & SQ(strFURI_CODE))                          '�U�փR�[�h
            SQL.Append("," & SQ(intKEKKA))                              '����
            SQL.Append(")")

            If MainDB.ExecuteNonQuery(SQL) <> 1 Then
                Throw New Exception("���[�N�}�X�^�}�����s")
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�N�}�X�^�쐬)", "���s", ex.ToString)
            Return False
        End Try

        fn_INSERT_PR_KOUZAMAST = True
    End Function
    Function fn_DELETE_PR_KOUZAMAST() As Boolean
        '============================================================================
        'NAME           :fn_DELETE_PR_KOUZAMAST
        'Parameter      :
        'Description    :�����`�F�b�N���X�g�}�X�^�̑S�f�[�^���폜����
        'Return         :True=OK(����),False=NG�i���s�j
        'Create         :2005/04/14
        'UPDATE         :
        '============================================================================
        fn_DELETE_PR_KOUZAMAST = False
        Try
            Dim SQL As New StringBuilder
            SQL.Append("DELETE FROM PR_KOUZAMAST")
            MainDB.ExecuteNonQuery(SQL)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�N�}�X�^�폜)", "���s", ex.ToString)
            Return False
        End Try
        fn_DELETE_PR_KOUZAMAST = True
    End Function
#End Region

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- END

        '�w�Z���R���{�{�b�N�X�ݒ�
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = True Then
            cmbGakkouName.Focus()
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '�w�Z������̊w�Z�R�[�h�ݒ�
        '�w�Z���̎擾
        lab�w�Z��.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '�w�Z�R�[�h�ɃJ�[�\���ݒ�
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '2010/11/09 �w�Z�R�[�h�[������
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            '�w�Z���̎擾
            STR�w�Z�R�[�h = Trim(txtGAKKOU_CODE.Text)
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If
        End If
    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
