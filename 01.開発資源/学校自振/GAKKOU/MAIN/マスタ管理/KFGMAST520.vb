Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Imports CAstReports

Public Class KFGMAST520

#Region " ���ʕϐ���` "
    Private Lng_Himoku_Ok_Cnt As Long
    Private Lng_Seito_Ok_Cnt As Long
    Private Lng_Gak_Ok_Cnt As Long
    Private Lng_Kin_Ok_Cnt As Long

    Private Lng_Himoku_Ng_Cnt As Long
    Private Lng_Seito_Ng_Cnt As Long
    Private Lng_Gak_Ng_Cnt As Long

    'Private Lng_Csv_Ok_Cnt As String = 0
    'Private Lng_Csv_Ng_Cnt As String = 0

    Private Lng_Himoku_SeqNo As Long
    Private Lng_Seito_SeqNo As Long
    Private Lng_Gak_SeqNo As Long

    Public STR���[�\�[�g�� As String = "0"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST520", "�w�Z�c�[���A�g���")
    Private ToolLOG As New CASTCommon.BatchLOG("KFGMAST520/ERROR", "�f�[�^�捞�G���[���O")

    Private Const msgTitle As String = "�w�Z�c�[���A�g���(KFGMAST520)"
    Private MainDB As MyOracle

    Private KobetuLogFileName As String '�G���[���O
    Private KobetuLogFileName2 As String '�Ǎ����O

    Private err_code As Integer = 0
    Private err_Filename As String = ""

    '�w�Z�c�[���쐬��A�捞��PATH�擾
    Private GAKTOOL_IN_PATH As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "SAVE_PATH")
    Private GAKTOOL_INBK_PATH As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "CSVBK")

    Private GAKTOOL_SEITO_FILE As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "TORIKOMI_CSV_SEITO")
    Private GAKTOOL_HIMO_FILE As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "TORIKOMI_CSV_HIMOKU")
    Private GAKTOOL_GAK_FILE As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "TORIKOMI_CSV_GAKKO")

    Private GAKTOOL_LOG_PATH As String = GFUNC_INI_READ("COMMON", "LOG")

    '�����N��
    Private strSEIKYUTUKI As String
    '���k�}�X�^�������`�����E�J�i�E�Z���E�s�d�k
    Private strKANJI_MEI As String
    Private strKANA_MEI As String
    Private strADDRESS As String
    Private strTELNO As String

    Private EXP_NEND As Integer
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAST520_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '���O�p
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            With Me
                .WindowState = FormWindowState.Normal
                .FormBorderStyle = FormBorderStyle.FixedDialog
                .ControlBox = True
            End With

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")
                MessageBox.Show("�w�Z���R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub
#End Region

#Region " Button Click "
    Private Sub btnImp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImp.Click
        MainDB = New MyOracle
        Dim sql As New StringBuilder(128)
        Dim aoraReader As New MyOracleReader(MainDB)
        Dim OK_ken As Long = 0
        Dim NG_ken As Long = 0

        Try
            err_code = 0
            Dim Gakkou_Code As String = 0
            LW.ToriCode = txtGAKKOU_CODE.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ړ�)�J�n", "����", "")

            Cursor.Current = Cursors.WaitCursor()
            Dim strDIR As String

            '���͒l�`�F�b�N
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                MessageBox.Show("�ړ������͑S���������s���܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If
            If Trim(txtGAKKOU_CODE.Text) = "" Then
                MessageBox.Show("�w�Z�R�[�h�����͂���Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If

            '�m�F���b�Z�[�W
            If MessageBox.Show("�b�r�u����̃C���|�[�g���J�n���܂�" & vbCrLf & "��ڃ}�X�^�͌��݂̏������ׂč폜���Ă���o�^���s���܂�", _
                                          msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            strDIR = CurDir()

            '�o�b�N�A�b�v�t�H���_�̊m�F
            If Directory.Exists(GAKTOOL_INBK_PATH) = False Then
                Directory.CreateDirectory(GAKTOOL_INBK_PATH)
            End If

            '�w�Z�R�[�h�̎擾
            sql.Append(" SELECT DISTINCT GAKKOU_CODE_T FROM GAKMAST1,GAKMAST2")
            sql.Append(" WHERE GAKKOU_CODE_T = GAKKOU_CODE_G")
            sql.Append(" AND GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text & "'")
            sql.Append(" ORDER BY GAKKOU_CODE_T")

            If aoraReader.DataReader(sql) = True Then

                Do Until aoraReader.EOF
                    Lng_Himoku_Ok_Cnt = 0
                    Lng_Seito_Ok_Cnt = 0

                    Lng_Himoku_Ng_Cnt = 0
                    Lng_Seito_Ng_Cnt = 0

                    Lng_Himoku_SeqNo = 1
                    Lng_Seito_SeqNo = 1

                    Gakkou_Code = aoraReader.GetString("GAKKOU_CODE_T")

                    MainDB.BeginTrans()

                    '***�w�Z�E�X�P�W���[���`�F�b�N***
                    If PFUNC_IMP_GAKKOU(Gakkou_Code, GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv") = False Then
                        MessageBox.Show("���������f����܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainDB.Rollback()
                        ChDir(strDIR)
                        btnEnd.Focus()
                        Return
                    Else
                        '�t�@�C���o�b�N�A�b�v���폜
                        If File.Exists(GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv", GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                        End If
                    End If


                    '*** ��ڃ}�X�^ ***
                    'CSV���́��}�X�^�o�^
                    If PFUNC_IMP_HIMOKU(Gakkou_Code, GAKTOOL_IN_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv", Lng_Himoku_Ok_Cnt, Lng_Himoku_Ng_Cnt) = False Then
                        MessageBox.Show("���������f����܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainDB.Rollback()
                        ChDir(strDIR)
                        btnEnd.Focus()

                        '�t�@�C���o�b�N�A�b�v�߂��i�w�Z�f�[�^�j
                        If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                        End If

                        Return
                    Else
                        '�t�@�C���o�b�N�A�b�v���폜
                        If File.Exists(GAKTOOL_IN_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_IN_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv", GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_IN_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv")
                        End If
                    End If

                    '���O�t�@�C���폜
                    KobetuLogFileName = Path.Combine(GAKTOOL_LOG_PATH, "Err_SEITO" & Gakkou_Code & ".log")
                    If File.Exists(KobetuLogFileName) Then
                        File.Delete(KobetuLogFileName)
                    End If
                    KobetuLogFileName2 = Path.Combine(GAKTOOL_LOG_PATH, "Red_SEITO" & Gakkou_Code & ".log")
                    If File.Exists(KobetuLogFileName2) Then
                        File.Delete(KobetuLogFileName2)
                    End If

                    '*** ���k�}�X�^ ***
                    'CSV���́��}�X�^�o�^
                    If PFUNC_IMP_SEITO(Gakkou_Code, GAKTOOL_IN_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv", Lng_Seito_Ok_Cnt, Lng_Seito_Ng_Cnt) = False Then
                        MessageBox.Show("���������f����܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainDB.Rollback()
                        ChDir(strDIR)
                        btnEnd.Focus()

                        '�t�@�C���o�b�N�A�b�v�߂��i�w�Z�f�[�^�j
                        If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                        End If
                        '�t�@�C���o�b�N�A�b�v�߂��i��ڃf�[�^�j
                        If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv")
                        End If

                        Return
                    Else
                        '�t�@�C���o�b�N�A�b�v���폜
                        If File.Exists(GAKTOOL_IN_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_IN_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv", GAKTOOL_INBK_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_IN_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv")
                        End If
                    End If

                    aoraReader.NextRead()
                    OK_ken += Lng_Himoku_Ok_Cnt + Lng_Seito_Ok_Cnt
                    NG_ken += Lng_Himoku_Ng_Cnt + Lng_Seito_Ng_Cnt
                Loop
            Else
                aoraReader.Close()
                MessageBox.Show("�w�Z�}�X�^�ɓo�^����Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return

            End If


            If NG_ken = 0 Then     '����

                MainDB.Commit()

                '�������b�Z�[�W
                MessageBox.Show("�C���|�[�g���I�����܂����B" & vbCrLf & "���:" & Lng_Himoku_Ok_Cnt & "���̃f�[�^��o�^���܂����B" & vbCrLf & "���k:" & Lng_Seito_Ok_Cnt & "���̃f�[�^��o�^���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else

                MainDB.Rollback()

                '�t�@�C���o�b�N�A�b�v�߂��i�w�Z�f�[�^�j
                If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv") Then
                    FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                    File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                End If
                '�t�@�C���o�b�N�A�b�v�߂��i��ڃf�[�^�j
                If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv") Then
                    FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv")
                    File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv")
                End If
                '�t�@�C���o�b�N�A�b�v�߂��i���k�f�[�^�j
                If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv") Then
                    FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv")
                    File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv")
                End If


                '�G���[�����b�Z�[�W
                '2010/11/22�@�����ύX
                MessageBox.Show("�C���|�[�g�Ɏ��s���܂����B" & vbCrLf & NG_ken & "���̃G���[�����O�ɏo�͂��܂����B" & vbCrLf & _
                             "���O���m�F���Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            ChDir(strDIR)
            btnEnd.Focus()

        Catch ex As System.IO.IOException
            MessageBox.Show("�t�@�C�����g�p���ł��B" & vbCrLf & KobetuLogFileName, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���O�t�@�C���A�N�Z�X", "���s", ex.Message)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ړ�)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ړ�)�I��", "����", "")
            If Not aoraReader Is Nothing Then aoraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try


    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Me.Close()

    End Sub
#End Region

#Region " Private Function "
#Region "�ړ�"
    Private Function PFUNC_IMP_GAKKOU(ByVal gakkou_code_H As String, ByVal pCsvPath As String) As Boolean

        Dim red As System.IO.StreamReader = Nothing


        Dim strReadLine As String
        Dim strSplitValue() As String
        Dim strGET_gakkou_code As String
        Dim strGET_furi_date As String

        Dim strErrorLine As String = ""

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUNC_IMP_GAKKOU = False

        Try

            Dim dirs As String() = System.IO.Directory.GetFiles(GAKTOOL_IN_PATH)
            Dim dir As String
            Dim dirflg As String

            dirflg = "0"
            For Each dir In dirs
                If dir = pCsvPath Then
                    dirflg = "1"
                End If
            Next

            If dirflg = "0" Then
                err_code = 2
                Throw New FileNotFoundException(String.Format("�t�@�C����������܂���B" _
                                        & ControlChars.Cr & "'{0}'", pCsvPath))
            End If

            red = New System.IO.StreamReader(pCsvPath, System.Text.Encoding.Default)

            Do While Not red.Peek() = -1

                strReadLine = red.ReadLine.ToString
                strSplitValue = Split(strReadLine, ",")

                strGET_gakkou_code = Trim(strSplitValue(0).PadLeft(10, "0"c))
                strGET_furi_date = Trim(strSplitValue(11).PadLeft(8, "0"c))

                '���͓��e�̃`�F�b�N�i�w�Z�R�[�h�`�F�b�N���������擾�j
                If gakkou_code_H = strGET_gakkou_code Then

                    '�X�P�W���[���}�X�^�̃`�F�b�N
                    sql = New StringBuilder(128)
                    oraReader = New MyOracleReader(MainDB)
                    sql.Append("SELECT NENGETUDO_S FROM G_SCHMAST ")
                    sql.Append(" WHERE")
                    sql.Append(" GAKKOU_CODE_S  = '" & gakkou_code_H & "' ")
                    sql.Append(" AND")
                    sql.Append(" FURI_KBN_S  = '0' ")
                    sql.Append(" AND")
                    sql.Append(" FURI_DATE_S = '" & strGET_furi_date & "' ")

                    If oraReader.DataReader(sql) = False Then
                        MessageBox.Show("�X�P�W���[�������݂��Ȃ��̂ŏ����ł��܂���B" & vbCrLf & "�f�[�^�U�֓�:" & strGET_furi_date, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Y���X�P�W���[���Ȃ� �f�[�^�U�֓�:" & strGET_furi_date, "���s", "")
                        oraReader.Close()
                        Return False
                    Else
                        strSEIKYUTUKI = oraReader.GetItem("NENGETUDO_S")
                        oraReader.Close()
                    End If

                Else
                    MessageBox.Show("�f�[�^�̊w�Z�R�[�h�ƈ�v���܂���̂ŏ����ł��܂���B" & vbCrLf & "�f�[�^�w�Z�R�[�h:" & strGET_gakkou_code, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�w�Z�R�[�h�s��v �f�[�^�w�Z�R�[�h:" & strGET_gakkou_code, "���s", "")
                    Return False
                End If
            Loop

            red.Close()
            Return True

        Catch ex As FileNotFoundException
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                MessageBox.Show("�t�@�C����������܂���B" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�w�Z�t�@�C���Ȃ�", "���s", ex.Message)
            Else
                Return True
            End If
        Catch ex As System.IO.IOException
            MessageBox.Show("�t�@�C�����g�p���ł��B" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�w�Z�t�@�C���A�N�Z�X", "���s", ex.Message)
            err_code = 1
            err_Filename = pCsvPath
            Return False
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�w�Z�`�F�b�N��O�G���[", "���s", ex.Message)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
        End Try

    End Function


    Private Function PFUNC_IMP_HIMOKU(ByVal gakkou_code_H As String, ByVal pCsvPath As String, ByRef lngOkCnt As Long, ByRef lngErrCnt As Long) As Boolean

        Dim red As System.IO.StreamReader = Nothing


        Dim strReadLine As String

        Dim strErrorLine As String = ""

        Dim sql As StringBuilder

        lngOkCnt = 0
        lngErrCnt = 0
        PFUNC_IMP_HIMOKU = False

        Try

            Dim dirs As String() = System.IO.Directory.GetFiles(GAKTOOL_IN_PATH)
            Dim dir As String
            Dim dirflg As String

            dirflg = "0"
            For Each dir In dirs
                If dir = pCsvPath Then
                    dirflg = "1"
                End If
            Next

            If dirflg = "0" Then
                err_code = 2
                Throw New FileNotFoundException(String.Format("�t�@�C����������܂���B" _
                                        & ControlChars.Cr & "'{0}'", pCsvPath))
            End If

            red = New System.IO.StreamReader(pCsvPath, System.Text.Encoding.Default)

            'SQL���쐬
            '��ʂɐݒ肳��Ă���w�Z�R�[�h�������ɔ�ڃ}�X�^���폜����
            sql = New StringBuilder(128)
            sql.Append(" DELETE  FROM HIMOMAST")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_H ='" & gakkou_code_H & "'")

            '�g�����U�N�V�����f�[�^����J�n
            If MainDB.ExecuteNonQuery(sql) < 0 Then
                MainLOG.Write("��ڃ}�X�^�폜", "���s", sql.ToString)
                Return False
            End If

            Do While Not red.Peek() = -1

                strReadLine = red.ReadLine.ToString

                '���͓��e�̃`�F�b�N
                If PFUNC_CHECK_HIMOKU(gakkou_code_H, Lng_Himoku_SeqNo, strReadLine, strErrorLine) = True Then
                    'SQL���쐬��SQL�������s
                    If PFUNC_INSERT_HIMOKU(Lng_Himoku_SeqNo, strReadLine, strErrorLine) = True Then
                        lngOkCnt += 1
                    Else
                        lngErrCnt += 1

                        '�װ۸ނ̏����o��
                        ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_INSERT_HIMOKU", "���s", Replace(strErrorLine, ",", "/"))
                        'SQL�ŃG���[�����������ꍇ�͏������f
                        red.Close()
                        Return False
                    End If
                Else
                    lngErrCnt += 1

                    '�װ۸ނ̏����o��
                    ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_CHECK_HIMOKU", "���s", Replace(strErrorLine, ",", "/"))
                End If

                Lng_Himoku_SeqNo += 1
            Loop

            red.Close()
            Return True

        Catch ex As FileNotFoundException
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                MessageBox.Show("�t�@�C����������܂���B" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��ڃt�@�C���Ȃ�", "���s", ex.Message)
            Else
                Return True
            End If
        Catch ex As System.IO.IOException
            MessageBox.Show("�t�@�C�����g�p���ł��B" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��ڃt�@�C���A�N�Z�X", "���s", ex.Message)
            err_code = 1
            err_Filename = pCsvPath
            Return False
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��ڃ`�F�b�N��O�G���[", "���s", ex.Message)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
        End Try

    End Function
    Private Function PFUNC_CHECK_HIMOKU(ByVal gakkou_code_C As String, ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean

        Dim intCnt As Integer
        Dim strSplitValue() As String
        Dim strRET As String = ""

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader


        PFUNC_CHECK_HIMOKU = False
        Try
            strSplitValue = Split(pLineValue, ",")

            '���͓��e�`�F�b�N
            If strSplitValue.Length = 76 Then
                For intCnt = 0 To UBound(strSplitValue)
                    Select Case intCnt
                        Case 0
                            '�\���敪
                            Select Case Trim(strSplitValue(intCnt))
                                Case 1
                                Case 2
                                Case 9 '�폜�Ȃ̂ŃX�L�b�v
                                    PFUNC_CHECK_HIMOKU = True
                                    Exit Function
                                Case ""
                                Case Else
                                    pError = pSeqNo & ",�\���敪,�\���敪���͕s���ł�"
                                    Exit Function
                            End Select

                        Case 1
                            '�w�Z�R�[�h

                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",�w�Z�R�[�h,�����͂ł�"
                                Exit Function
                            End If

                            '�t�@�C�����ɋL�ڂ���Ă���w�Z�R�[�h�ȊO�̃R�[�h�̏ꍇ�̓G���[
                            If Trim(gakkou_code_C) <> Trim(strSplitValue(1).PadLeft(10, "0"c)) Then
                                pError = pSeqNo & ",�w�Z�R�[�h,�t�@�C�����ƃf�[�^�̊w�Z�R�[�h���Ⴂ�܂�"
                                Exit Function
                            End If

                            ''�w�Z�}�X�^�`�F�b�N
                            'sql = New StringBuilder(128)
                            'oraReader = New MyOracleReader(MainDB)
                            'sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
                            'sql.Append(" WHERE GAKKOU_CODE_G = '" & strSplitValue(intCnt).PadLeft(10, "0"c) & "'")

                            'If oraReader.DataReader(sql) = False Then

                            '    pError = pSeqNo & ",�w�Z�R�[�h,�w�Z�}�X�^�ɓo�^����Ă���w�Z�R�[�h�ȊO�̂��͓̂o�^�ł��܂���"
                            '    oraReader.Close()
                            '    Return False
                            'End If
                            'oraReader.Close()
                        Case 2
                            '�w�N�R�[�h

                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",�w�N�R�[�h,�����͂ł�"

                                Exit Function
                            End If

                            '���l�`�F�b�N
                            If IsNumeric(strSplitValue(intCnt)) = False Then
                                pError = pSeqNo & ",�w�N�R�[�h,�w�N�R�[�h�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"
                                Exit Function
                            Else
                                '���͒l�͈̓`�F�b�N
                                Select Case CLng(strSplitValue(intCnt))
                                    Case 1 To 9
                                    Case Else
                                        pError = pSeqNo & ",�w�N�R�[�h,�w�N�R�[�h�͂P�`�X�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"
                                        Exit Function
                                End Select
                            End If

                            '�w�Z�}�X�^�`�F�b�N(���͊w�N)
                            sql = New StringBuilder(128)
                            oraReader = New MyOracleReader(MainDB)
                            sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
                            sql.Append(" WHERE")
                            sql.Append(" GAKKOU_CODE_G = '" & Trim(strSplitValue(1).PadLeft(10, "0"c)) & "'")
                            sql.Append(" AND")
                            sql.Append(" GAKUNEN_CODE_G = " & Trim(strSplitValue(intCnt)))

                            If oraReader.DataReader(sql) = False Then
                                pError = pSeqNo & ",�w�N�R�[�h,�w�N�R�[�h�����݂��܂���"
                                oraReader.Close()
                                Return False
                            End If
                            oraReader.Close()
                        Case 3
                            '���ID
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",���ID,�����͂ł�"
                                Exit Function
                            End If

                            '���ID�̒����`�F�b�N 2007/02/12
                            If strSplitValue(intCnt).Trim.Length >= 4 Then
                                pError = pSeqNo & ",���ID,���ID���S���𒴂��Đݒ肳��Ă��܂�"
                                Exit Function
                            End If
                        Case 4
                            '��ږ���
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",��ږ�,�����͂ł�"
                                Exit Function
                            End If
                            '��ږ��̂̒����`�F�b�N 
                            '2011/01/27
                            If strSplitValue(intCnt).Trim.Length > 20 Then
                                pError = pSeqNo & ",��ږ�,��ږ��̂�20���𒴂��Đݒ肳��Ă��܂�"
                                Exit Function
                            End If
                        Case 5
                            '������
                            '���ID��000(���ό���)�̏ꍇ�����������͂���Ă��Ȃ����߃`�F�b�N�͍s��Ȃ�
                            Select Case strSplitValue(3).Trim.PadLeft(3, "0"c)
                                Case Is <> "000"
                                    If IsNumeric(strSplitValue(intCnt)) = False Then

                                        pError = pSeqNo & ",������,�����͂ł�"

                                        Exit Function
                                    Else
                                        If IsNumeric(strSplitValue(intCnt)) = False Then
                                            pError = pSeqNo & ",������,�������͂P�`�P�Q�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"
                                            Exit Function
                                        Else
                                            Select Case CLng(strSplitValue(intCnt))
                                                Case 1 To 12
                                                Case Else
                                                    pError = pSeqNo & ",������,�������͂P�`�P�Q�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"

                                                    Exit Function
                                            End Select

                                        End If
                                    End If
                            End Select
                        Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69
                            '��ږ���
                            '��ږ��̂̒����`�F�b�N 
                            If strSplitValue(intCnt).Trim.Length > 20 Then
                                pError = pSeqNo & ",��ږ���,��ږ��̂�20���𒴂��Đݒ肳��Ă��܂�"

                                Exit Function
                            End If
                        Case 7, 14, 21, 28, 35, 42, 49, 56, 63, 70
                            If Trim(strSplitValue(intCnt - 1)) <> "" Then
                                '���Z�@�փR�[�h
                                If Trim(strSplitValue(intCnt)) = "" Then

                                Else

                                    '�����`�F�b�N 2007/02/12
                                    If strSplitValue(intCnt).Trim.Length > 4 Then

                                        pError = pSeqNo & ",���ϋ��Z�@��,���ϋ��Z�@�փR�[�h���S���𒴂��Ă��܂�"

                                        Exit Function
                                    End If

                                    '���l�`�F�b�N
                                    If IsNumeric(strSplitValue(intCnt)) = False Then
                                        pError = pSeqNo & ",���ϋ��Z�@��,���ϋ��Z�@�փR�[�h�ɂ͐��l����͂��Ă�������"

                                        Exit Function
                                    End If
                                End If
                            End If


                        Case 8, 15, 22, 29, 36, 43, 50, 57, 64, 71

                            If Trim(strSplitValue(intCnt - 1)) <> "" Then
                                '�x�X�R�[�h
                                If Trim(strSplitValue(intCnt)) = "" Then

                                Else

                                    '�����`�F�b�N 2007/02/12
                                    If strSplitValue(intCnt).Trim.Length > 3 Then

                                        pError = pSeqNo & ",���ώx�X,���ώx�X�R�[�h���R���𒴂��Ă��܂�"

                                        Exit Function
                                    End If
                                    '���l�`�F�b�N
                                    If IsNumeric(strSplitValue(intCnt)) = False Then
                                        pError = pSeqNo & ",���ώx�X,���ώx�X�R�[�h�ɂ͐��l����͂��Ă�������"

                                        Exit Function
                                    End If


                                    '���Z�@�փ}�X�^���݃`�F�b�N
                                    sql = New StringBuilder(128)
                                    oraReader = New MyOracleReader(MainDB)
                                    sql.Append("SELECT * FROM TENMAST ")
                                    sql.Append(" WHERE")
                                    sql.Append(" KIN_NO_N = '" & Trim(strSplitValue(intCnt - 1)).PadLeft(4, "0"c) & "'")
                                    sql.Append(" AND")
                                    sql.Append(" SIT_NO_N = '" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                                    If oraReader.DataReader(sql) = False Then
                                        pError = pSeqNo & ",���ώx�X,���Z�@�փ}�X�^�ɓo�^����Ă��Ȃ����Z�@�ւ�ݒ肷�邱�Ƃ͂ł��܂���"
                                        oraReader.Close()
                                        Return False
                                    End If
                                    oraReader.Close()
                                End If
                            End If
                        Case 9, 16, 23, 30, 37, 44, 51, 58, 65, 72
                            '���ωȖ� �����͂̂Ƃ��G���[���b�Z�[�W�o��
                            If Trim(strSplitValue(intCnt - 1)) <> "" Then '�x�X�R�[�h���ݒ肳��Ă���ꍇ
                                If Trim(strSplitValue(intCnt)) = "" Then
                                    pError = pSeqNo & ",���ωȖ�,�����͂ł�"
                                    Exit Function
                                Else '2007/02/15
                                    '���l�`�F�b�N
                                    If IsNumeric(strSplitValue(intCnt)) = False Then
                                        pError = pSeqNo & ",���ωȖ�,���ωȖڂɂ͐��l����͂��Ă�������"

                                        Exit Function
                                    End If
                                    Select Case CInt(Trim(strSplitValue(intCnt)))
                                        Case 1, 2
                                        Case Else
                                            pError = pSeqNo & ",���ωȖ�,���ωȖڂ͕��ʂ܂��͓����̂ݎw��\�ł�"
                                            Exit Function
                                    End Select

                                End If
                            End If

                        Case 10, 17, 24, 31, 38, 45, 52, 59, 66, 73

                            '�����ԍ�
                            If Trim(strSplitValue(intCnt - 4)) <> "" Then
                                If Trim(strSplitValue(intCnt)) = "" Then

                                Else
                                    '���l�`�F�b�N
                                    If IsNumeric(strSplitValue(intCnt)) = False Then
                                        pError = pSeqNo & ",���ό����ԍ�,���ό����ԍ��ɂ͐��l����͂��Ă�������"

                                        Exit Function
                                    End If
                                    '���ό����ԍ������`�F�b�N
                                    If strSplitValue(intCnt).Trim.Length > 7 Then

                                        pError = pSeqNo & ",���ό����ԍ�,���ό����ԍ����V���𒴂��Ă��܂�"

                                        Exit Function
                                    End If
                                End If
                            End If
                        Case 11, 18, 25, 32, 39, 46, 53, 60, 67, 74

                            '2007/09/19�@�ǉ�
                            '���`�l��(��)

                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt - 1)) <> "" Then
                                If Trim(strSplitValue(intCnt)) = "" Then

                                Else
                                    '�K��O�����`�F�b�N 2006/04/16
                                    If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = False Then
                                        pError = pSeqNo & ",���`�l��(��),�K��O�������L��܂�"
                                        Exit Function
                                    End If
                                    '�������`�F�b�N 2006/05/10
                                    If Trim(strSplitValue(intCnt)).Length > 48 Then
                                        pError = pSeqNo & ",���`�l��(��),���p48�����ȓ��Őݒ肵�Ă�������"
                                        Exit Function
                                    End If

                                End If
                            End If
                        Case 12, 19, 26, 33, 40, 47, 54, 61, 68, 75

                            '��ڋ��z 
                            If strSplitValue(intCnt).Trim <> "" Then
                                If IsNumeric(strSplitValue(intCnt)) = False Then
                                    pError = pSeqNo & ",��ڋ��z,��ڋ��z�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"
                                    Exit Function
                                End If
                            End If
                            '���ID�̒����`�F�b�N
                            If strSplitValue(intCnt).Trim.Length > 8 Then
                                pError = pSeqNo & ",��ڋ��z,��ڋ��z���W���𒴂��Đݒ肳��Ă��܂�"
                                Exit Function
                            End If
                            'Case 76
                            '    '�쐬���t
                            '    If Trim(strSplitValue(intCnt)) = "" Then
                            '        'pError = pSeqNo & ",�쐬���t,�����͂ł�"
                            '        'Exit Function
                            '    Else
                            '        '���l�`�F�b�N
                            '        If IsNumeric(Trim(strSplitValue(intCnt))) = False Then
                            '            pError = pSeqNo & ",�쐬���t,�쐬���t�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"
                            '            Exit Function
                            '        End If

                            '        '�������`�F�b�N 
                            '        If Trim(strSplitValue(intCnt)).Length > 8 Then
                            '            pError = pSeqNo & ",�쐬���t,�쐬���t���W���𒴂��Đݒ肳��Ă��܂�"
                            '            Exit Function
                            '        End If
                            '    End If
                            'Case 77
                            '    '�X�V���t
                            '    If Trim(strSplitValue(intCnt)) = "" Then
                            '        'pError = pSeqNo & "�X�V���t,�����͂ł�"
                            '        'Exit Function
                            '    Else
                            '        '���l�`�F�b�N
                            '        If IsNumeric(Trim(strSplitValue(intCnt))) = False Then
                            '            pError = pSeqNo & ",�X�V���t,�X�V���t�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"
                            '            Exit Function
                            '        End If
                            '        '�������`�F�b�N
                            '        If Trim(strSplitValue(intCnt)).Length > 8 Then
                            '            pError = pSeqNo & ",�X�V���t,�X�V���t���W���𒴂��Đݒ肳��Ă��܂�"
                            '            Exit Function
                            '        End If
                            '    End If
                    End Select
                Next intCnt
            Else
                pError = pSeqNo & ",���ڐ�,��ڏ��f�[�^�̍��ڐ��ɕs��������܂�"
                Exit Function
            End If

            PFUNC_CHECK_HIMOKU = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��ڃ`�F�b�N��O�G���[", "���s", intCnt & ex.Message)
            Return False
        End Try
    End Function
    Private Function PFUNC_INSERT_HIMOKU(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean

        Dim intCnt As Integer
        Dim strSplitValue() As String
        Dim strRET As String = ""
        Dim KIN_NO As String = ""
        Dim SIT_NO As String = ""
        Dim KOUZA_NO As String = ""

        Dim sql As New StringBuilder(128)

        PFUNC_INSERT_HIMOKU = False
        Try
            strSplitValue = Split(pLineValue, ",")

            '�w�Z�}�X�^�����Z�@�փR�[�h�A�x�X�R�[�h�A�����ԍ��擾
            Get_HIMOKU_INFO(strSplitValue(1), KIN_NO, SIT_NO, KOUZA_NO)
            For intCnt = 0 To 112
                Select Case intCnt
                    Case 0
                        If strSplitValue(intCnt).Trim = "9" Then
                            Return True
                            Exit Function
                        Else
                            sql.Append(" INSERT INTO HIMOMAST VALUES(")
                        End If
                    Case 1
                        sql.Append("'" & strSplitValue(intCnt).Trim.PadLeft(10, "0"c) & "'")
                        '�w�N�����1�`10�܂ł̋��z
                    Case 2, 12, 19, 26, 33, 40, 47, 54, 61, 68, 75
                        If Trim(strSplitValue(intCnt)) = "" Then
                            sql.Append(",0")
                        Else
                            sql.Append("," & strSplitValue(intCnt))
                        End If
                        '���11�`15(���ݒ�Ȃ̂ŌŒ�)
                    Case 82, 89, 96, 103, 110
                        sql.Append(",0")
                    Case 3
                        '���ID�OZERO�l��
                        sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")
                    Case 4
                        '���ID��
                        strSplitValue(intCnt) = StrConv(strSplitValue(intCnt), VbStrConv.Wide)
                        If strSplitValue(intCnt).Trim.Length > 15 Then
                            sql.Append(",'" & strSplitValue(intCnt).Substring(0, 15) & "'")
                        Else
                            sql.Append(",'" & strSplitValue(intCnt) & "'")
                        End If
                    Case 5
                        '������
                        If Trim(strSplitValue(intCnt - 2)) = "000" Then
                            sql.Append(",'  '")
                        Else
                            sql.Append(",'" & Format(CInt(strSplitValue(intCnt)), "00") & "'")
                        End If
                        '���1�`10�܂�
                    Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69
                        '��ږ�
                        strSplitValue(intCnt) = StrConv(strSplitValue(intCnt), VbStrConv.Wide)
                        If strSplitValue(intCnt).Trim.Length > 10 Then
                            sql.Append(",'" & strSplitValue(intCnt).Substring(0, 10) & "'")
                        Else
                            sql.Append(",'" & strSplitValue(intCnt) & "'")
                        End If
                        '���11�`15(���ݒ�Ȃ̂ŌŒ�)
                    Case 76, 83, 90, 97, 104
                        sql.Append(",''")
                        'Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69, 76, 83, 90, 97, 104
                        '���1�`10�܂�
                    Case 7, 14, 21, 28, 35, 42, 49, 56, 63, 70
                        '���ϋ��Z�@��
                        If Trim(strSplitValue(intCnt - 1)) <> "" Then
                            If strSplitValue(intCnt).Trim = "" Then
                                sql.Append(",'" & KIN_NO & "'")
                            Else
                                sql.Append(",'" & strSplitValue(intCnt).PadLeft(4, "0"c) & "'")
                            End If
                        Else
                            sql.Append(",''")
                        End If
                        '���11�`15(���ݒ�Ȃ̂ŌŒ�)
                    Case 77, 84, 91, 98, 105
                        sql.Append(",''")
                    Case 8, 15, 22, 29, 36, 43, 50, 57, 64, 71
                        '���ϋ��Z�@�֎x�X
                        If Trim(strSplitValue(intCnt - 2)) <> "" Then
                            If strSplitValue(intCnt).Trim = "" Then
                                sql.Append(",'" & SIT_NO & "'")
                            Else
                                sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(3, "0"c) & "'")
                            End If
                        Else
                            sql.Append(",''")
                        End If
                        '���11�`15(���ݒ�Ȃ̂ŌŒ�)
                    Case 78, 85, 92, 99, 106
                        sql.Append(",''")
                        '���1�`10�܂�
                    Case 9, 16, 23, 30, 37, 44, 51, 58, 65, 72
                        '���ωȖ�
                        If Trim(strSplitValue(intCnt - 3)) <> "" Then
                            If Trim(strSplitValue(intCnt)) = "" Then
                                sql.Append(",'01'")
                            Else
                                Select Case Trim(strSplitValue(intCnt)).PadLeft(2, "0"c)
                                    Case "01", "02", "05", "37"
                                        sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(2, "0"c) & "'")
                                    Case Else
                                        sql.Append(",'01'")
                                End Select
                            End If
                        Else
                            sql.Append(",''")
                        End If
                        '���11�`15(���ݒ�Ȃ̂ŌŒ�)
                    Case 79, 86, 93, 100, 107
                        sql.Append(",''")
                        '���1�`10�܂�
                    Case 10, 17, 24, 31, 38, 45, 52, 59, 66, 73
                        '�����ԍ�
                        If Trim(strSplitValue(intCnt - 4)) <> "" Then
                            If strSplitValue(intCnt).Trim = "" Then
                                sql.Append(",'" & KOUZA_NO & "'")
                            Else
                                sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(7, "0"c) & "'")
                            End If
                        Else
                            sql.Append(",''")
                        End If
                        '���11�`15(���ݒ�Ȃ̂ŌŒ�)
                    Case 80, 87, 94, 101, 108
                        sql.Append(",''")

                    Case 11, 18, 25, 32, 39, 46, 53, 60, 67, 74
                        '���`�l�i����������ϊ����ݒ�j
                        If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = True Then
                            sql.Append(",'" & strRET & "'")
                        Else
                            sql.Append(",'" & Space(40) & "'")
                        End If
                        '���11�`15(���ݒ�Ȃ̂ŌŒ�)
                    Case 81, 88, 95, 102, 109
                        sql.Append(",''")

                    Case 111
                        '�쐬���t
                        sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
                    Case 112
                        '�X�V���t
                        sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")

                        sql.Append(",'" & Space(50) & "'")
                        sql.Append(",'" & Space(50) & "'")
                        sql.Append(",'" & Space(50) & "'")
                        sql.Append(",'" & Space(50) & "'")
                        sql.Append(",'" & Space(50) & "'")
                        sql.Append(")")

                    Case Else
                        sql.Append(",'" & strSplitValue(intCnt) & "'")
                End Select
            Next intCnt

            'sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
            'sql.Append(",'00000000'")


            If MainDB.ExecuteNonQuery(sql) < 0 Then
                pError = pSeqNo & ",�ړ�,�ړ��������ɃG���[���������܂���"
                Return False
            End If

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���SQL��O�G���[", "���s", intCnt & ex.Message)
        End Try

    End Function
    Private Function PFUNC_IMP_SEITO(ByVal gakkou_code_S As String, ByVal pCsvPath As String, ByRef lngOkCnt As Long, ByRef lngErrCnt As Long) As Boolean

        Dim red As System.IO.StreamReader = Nothing

        Dim strReadLine As String
        Dim Shinsei_KBN As String   '�\���敪
        Dim NENDO As Integer        '���w�N�x
        Dim TUUBAN As Integer       '�ʔ�

        Dim strErrorLine As String = ""
        Dim strSEITOLine As String = ""

        lngOkCnt = 0
        lngErrCnt = 0
        Dim blnSQL As Boolean = True

        PFUNC_IMP_SEITO = False

        Try

            Dim dirs As String() = System.IO.Directory.GetFiles(GAKTOOL_IN_PATH)
            Dim dir As String
            Dim dirflg As String

            dirflg = "0"
            For Each dir In dirs
                If dir = pCsvPath Then
                    dirflg = "1"
                End If
            Next

            If dirflg = "0" Then
                err_code = 2
                Throw New FileNotFoundException(String.Format("�t�@�C����������܂���B" _
                                        & ControlChars.Cr & "'{0}'", pCsvPath))
            End If

            red = New System.IO.StreamReader(pCsvPath, System.Text.Encoding.Default)

            Do While Not red.Peek() = -1

                strReadLine = red.ReadLine.ToString

                '���͓��e�̃`�F�b�N
                Shinsei_KBN = ""
                If PFUNC_CHECK_SEITO(gakkou_code_S, Lng_Seito_SeqNo, strReadLine, strErrorLine, Shinsei_KBN, NENDO, TUUBAN) = True Then

                    Dim strSeitoInfoOld As String = ""

                    '20130405 �����k���̎擾
                    If Not GetSeitoInfo(gakkou_code_S, NENDO, TUUBAN, strSeitoInfoOld) Then
                        Return False
                    End If

                    '�ǂݍ��݃��O�̏����o��
                    Select Case Shinsei_KBN
                        Case 1
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "�V�K," & strReadLine & "," & strKANA_MEI & "," & strKANJI_MEI & "," & strSeitoInfoOld & "," & "1") = False Then
                                Return False
                            End If
                        Case 2
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "�ύX," & strReadLine & "," & strKANA_MEI & "," & strKANJI_MEI & "," & strSeitoInfoOld & "," & "0") = False Then
                                Return False
                            End If
                            If Get_SEITO_UPDATE_BEFORE(gakkou_code_S, NENDO, TUUBAN, strReadLine, strSEITOLine) = False Then
                                'Return False
                            End If
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "�ύX�O," & strSEITOLine & ",," & "," & strSeitoInfoOld & "," & "1") = False Then
                                Return False
                            End If
                        Case 9
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "�폜," & strReadLine & "," & strKANA_MEI & "," & strKANJI_MEI & "," & strSeitoInfoOld & "," & "1") = False Then
                                Return False
                            End If
                    End Select
                    Select Case Shinsei_KBN
                        Case "1"    '�o�^
                            'SQL���쐬��SQL�������s
                            If PFUNC_INSERT_SEITO(Lng_Seito_SeqNo, strReadLine, strErrorLine) = True Then
                                lngOkCnt += 1
                            Else

                                lngErrCnt += 1
                                blnSQL = False

                                '�װ۸ނ̏����o��
                                If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                                    Return False
                                End If
                                ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_INSERT_SEITO", "���s", Replace(strErrorLine, ",", "/"))

                                'SQL�ŃG���[�����������ꍇ�͏������f
                                red.Close()
                                Exit Do
                            End If
                            '�װ۸ނ̏����o���i�������擾�G���[�̏ꍇ�̂݁j
                            If strErrorLine <> "" Then
                                If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                                    Return False
                                End If
                            End If
                        Case "2"    '�X�V
                            If PFUNC_UPDATE_SEITO(Lng_Seito_SeqNo, strReadLine, strErrorLine) = True Then
                                lngOkCnt += 1
                            Else

                                lngErrCnt += 1
                                blnSQL = False

                                '�װ۸ނ̏����o��
                                If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                                    Return False
                                End If
                                ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_UPDATE_SEITO", "���s", Replace(strErrorLine, ",", "/"))

                                'SQL�ŃG���[�����������ꍇ�͏������f
                                red.Close()
                                Exit Do
                            End If
                            '�װ۸ނ̏����o���i�������擾�G���[�̏ꍇ�̂݁j
                            If strErrorLine <> "" Then
                                If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                                    Return False
                                End If
                            End If
                        Case "9"    '�폜
                            If PFUNC_DELETE_SEITO(Lng_Seito_SeqNo, strReadLine, strErrorLine) = True Then
                                'lngOkCnt += 1
                            Else

                                lngErrCnt += 1
                                blnSQL = False

                                '�װ۸ނ̏����o��
                                If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                                    Return False
                                End If
                                ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_DELETE_SEITO", "���s", Replace(strErrorLine, ",", "/"))

                                'SQL�ŃG���[�����������ꍇ�͏������f
                                red.Close()
                                Exit Do
                            End If
                        Case Else
                            lngOkCnt += 1
                    End Select
                Else
                    lngErrCnt += 1

                    Dim strSeitoInfoOld As String = ""

                    '20130405 �����k���̎擾
                    If Not GetSeitoInfo(gakkou_code_S, NENDO, TUUBAN, strSeitoInfoOld) Then
                        Return False
                    End If

                    '�ǂݍ��݃��O�̏����o��
                    Select Case Shinsei_KBN
                        Case 1
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "�V�K," & strReadLine & "," & strKANA_MEI & "," & strKANJI_MEI & "," & strSeitoInfoOld & "," & "1") = False Then
                                Return False
                            End If
                        Case 2
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "�ύX," & strReadLine & "," & strKANA_MEI & "," & strKANJI_MEI & "," & strSeitoInfoOld & "," & "0") = False Then
                                Return False
                            End If
                            If Get_SEITO_UPDATE_BEFORE(gakkou_code_S, NENDO, TUUBAN, strReadLine, strSEITOLine) = False Then
                                Return False
                            End If
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "�ύX�O," & strSEITOLine & ",," & "," & strSeitoInfoOld & "," & "1") = False Then
                                Return False
                            End If
                        Case 9
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "�폜," & strReadLine & "," & strKANA_MEI & "," & strKANJI_MEI & "," & strSeitoInfoOld & "," & "1") = False Then
                                Return False
                            End If
                    End Select
                    '�װ۸ނ̏����o��
                    If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                        Return False
                    End If
                    ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_CHECK_SEITO", "���s", Replace(strErrorLine, ",", "/"))

                End If

                Lng_Seito_SeqNo += 1
            Loop

            '���k�f�[�^�捞�o�^���X�g�o��
            If File.Exists(KobetuLogFileName2) Then
                'If lngErrCnt > 0 Then
                Dim param As String = ""
                Dim nret As Integer = 0
                '�p�����[�^�ݒ�
                param = GCom.GetUserID & "," & gakkou_code_S ' & "," & pCsvPath

                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me
                nret = ExeRepo.ExecReport("KFGP501.EXE", param)
                '�߂�l�ɑΉ��������b�Z�[�W��\������
                Select Case nret
                    Case 0
                    Case Else
                        '������s���b�Z�[�W
                        MessageBox.Show(String.Format(MSG0004E, "���k�f�[�^�捞�o�^���X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                End Select
            Else
                MessageBox.Show("�X�V�Ώۂ�����܂���ł����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            If blnSQL = False Then
                Return False
            Else
                red.Close()
                Return True
            End If


        Catch ex As FileNotFoundException
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                MessageBox.Show("�t�@�C����������܂���B" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���k�t�@�C���Ȃ�", "���s", ex.Message)
            Else
                Return True
            End If
        Catch ex As System.IO.IOException
            MessageBox.Show("�t�@�C�����g�p���ł��B" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���k�t�@�C���A�N�Z�X", "���s", ex.Message)
            err_code = 1
            err_Filename = pCsvPath
            Return False
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���k�`�F�b�N��O�G���[", "���s", ex.Message)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
        End Try

    End Function

    Private Function PFUNC_CHECK_SEITO(ByVal gakkou_code_C As String, ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String, ByRef S_KBN As String, ByRef NENDO As Integer, ByRef TUUBAN As Integer) As Boolean

        Dim intCnt As Integer
        Dim intCnt2 As Integer
        Dim strSplitValue() As String
        Dim strRET As String = ""

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader


        PFUNC_CHECK_SEITO = False
        Try
            strKANJI_MEI = ""
            strKANA_MEI = ""
            strADDRESS = ""
            strTELNO = ""

            pError = ""

            strSplitValue = Split(pLineValue, ",")

            '���͓��e�`�F�b�N
            If strSplitValue.Length = 44 Then
                For intCnt = 0 To UBound(strSplitValue)
                    Select Case intCnt
                        Case 0
                            '�\���敪
                            Select Case Trim(strSplitValue(intCnt))
                                Case 1
                                    S_KBN = "1"
                                Case 2
                                    S_KBN = "2"
                                Case 9
                                    S_KBN = "9"
                                Case ""
                                    '2011/12/14
                                    PFUNC_CHECK_SEITO = True
                                    Exit Function
                                    '2011/12/14
                                Case Else
                                    pError = pSeqNo & ",�\���敪,�\���敪���͕s���ł�"
                                    Exit Function
                            End Select
                        Case 1
                            '�w�Z�R�[�h

                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",�w�Z,�����͂ł�"

                                Exit Function
                            End If

                            '��ʏ�œ��͂���Ă���R�[�h�ȊO�̃R�[�h�̏ꍇ�̓G���[
                            If gakkou_code_C <> Trim(strSplitValue(intCnt)).PadLeft(10, "0"c) Then

                                pError = pSeqNo & ",�w�Z�R�[�h,�t�@�C�����ƃf�[�^�̊w�Z�R�[�h���Ⴂ�܂�"
                                Exit Function
                            End If

                        Case 2
                            '���w�N�x

                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",���w�N�x,�����͂ł�"

                                Exit Function
                            End If

                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",���w�N�x,���l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"

                                Exit Function

                            End If
                            '���͌����`�F�b�N
                            If strSplitValue(intCnt).Trim.Length > 4 Then
                                pError = pSeqNo & ",���w�N�x,���w�N�x���S���𒴂��Đݒ肳��Ă��܂�"

                                Exit Function

                            End If

                            If strSplitValue(intCnt).Trim.Length < 4 Then
                                pError = pSeqNo & ",���w�N�x,���w�N�x���S�������Őݒ肳��Ă��܂�"

                                Exit Function

                            End If

                            NENDO = CInt(strSplitValue(intCnt))

                        Case 3
                            '�ʔ�

                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",�ʔ�,�����͂ł�"

                                Exit Function
                            End If

                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",�ʔ�,���l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"

                                Exit Function
                            Else
                                '���͒l�͈̓`�F�b�N
                                Select Case CLng(strSplitValue(intCnt))
                                    Case 1 To 9999
                                    Case Else
                                        pError = pSeqNo & ",�ʔ�,1�`9999�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"
                                        Exit Function
                                End Select
                            End If

                            TUUBAN = CInt(strSplitValue(intCnt))

                        Case 4
                            '�w�N�R�[�h

                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",�w�N�R�[�h,�����͂ł�"

                                Exit Function
                            End If

                            '���l�`�F�b�N
                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",�w�N�R�[�h,�w�N�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"

                                Exit Function
                            Else
                                '���͒l�͈̓`�F�b�N
                                Select Case CLng(strSplitValue(intCnt))
                                    Case 1 To 9
                                    Case Else
                                        pError = pSeqNo & ",�w�N�R�[�h,�w�N��1�`9�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"
                                        Exit Function
                                End Select
                            End If

                            '�w�Z�}�X�^�`�F�b�N(���͊w�N)
                            sql = New StringBuilder(128)
                            oraReader = New MyOracleReader(MainDB)
                            sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
                            sql.Append(" WHERE")
                            sql.Append(" GAKKOU_CODE_G = '" & Trim(strSplitValue(1)).PadLeft(10, "0"c) & "'")
                            sql.Append(" AND")
                            sql.Append(" GAKUNEN_CODE_G = " & Trim(strSplitValue(intCnt)))

                            If oraReader.DataReader(sql) = False Then
                                pError = pSeqNo & ",�w�N�R�[�h,�w�N�R�[�h���w�Z�}�X�^�ɓo�^����Ă��܂���"
                                oraReader.Close()
                                Return False
                            End If
                            oraReader.Close()
                        Case 5
                            '�N���X�R�[�h
                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",�N���X,�����͂ł��B"

                                Exit Function
                            Else

                                If IsNumeric(strSplitValue(intCnt)) = False Then

                                    pError = pSeqNo & ",�N���X,���l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"

                                    Exit Function
                                Else
                                    '���͒l�͈̓`�F�b�N
                                    Select Case CLng(strSplitValue(intCnt))
                                        Case 1 To 99
                                        Case Else
                                            pError = pSeqNo & ",�N���X,1�`99�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"

                                            Exit Function
                                    End Select
                                End If

                                '�g�p�N���X�`�F�b�N
                                sql = New StringBuilder(128)
                                oraReader = New MyOracleReader(MainDB)
                                sql.Append("SELECT * FROM GAKMAST1")
                                sql.Append(" WHERE")
                                sql.Append(" GAKKOU_CODE_G = '" & Trim(strSplitValue(1)).PadLeft(10, "0"c) & "'")
                                sql.Append(" AND")
                                sql.Append(" GAKUNEN_CODE_G = " & Trim(strSplitValue(4)))
                                sql.Append(" AND (")
                                For intCnt2 = 1 To 20
                                    sql.Append(IIf(intCnt2 = 1, "", "or") & " CLASS_CODE1" & Format(intCnt2, "00") & "_G = '" & Trim(strSplitValue(intCnt)).PadLeft(2, "0"c) & "'")
                                Next intCnt2
                                sql.Append(" )")

                                If oraReader.DataReader(sql) = False Then
                                    pError = pSeqNo & ",�N���X,�w�Z�}�X�^�ɑ��݂��Ȃ��N���X��ݒ肷�邱�Ƃ͏o���܂���"
                                    oraReader.Close()
                                    Return False
                                End If
                                oraReader.Close()
                            End If
                        Case 6
                            '���k�ԍ�

                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",���k�ԍ�,�����͂ł�"

                                Exit Function
                            End If

                            '���l�`�F�b�N
                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",���k�ԍ�,���k�ԍ��ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���B"

                                Exit Function
                            End If

                            '�����`�F�b�N 2007/02/12
                            If strSplitValue(intCnt).Trim.Length > 7 Then

                                pError = pSeqNo & ",���k�ԍ�,���k�ԍ����V���𒴂��Ă��܂�"

                                Exit Function
                            End If

                        Case 7
                            '���k����(��)

                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",���k����(��),�����͂ł�"

                                Exit Function
                            Else
                                '�K��O�����`�F�b�N 2006/04/16
                                If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = False Then
                                    pError = pSeqNo & ",���k����(��),�K��O�������L��܂�"
                                    Exit Function
                                End If
                                '�������`�F�b�N 2006/05/10
                                If Trim(strSplitValue(intCnt)).Length > 40 Then
                                    pError = pSeqNo & ",���k����(��),���p40�����ȓ��Őݒ肵�Ă�������"
                                    Exit Function
                                End If

                            End If
                        Case 8 '���k�������@�����`�F�b�N
                            If Trim(strSplitValue(intCnt)) <> "" Then
                                '�Q�o�C�g�ϊ�
                                strSplitValue(intCnt) = StrConv(Trim(strSplitValue(intCnt)), VbStrConv.Wide)
                                '�������`�F�b�N
                                If Trim(strSplitValue(intCnt)).Length > 25 Then
                                    pError = pSeqNo & ",���k����(����),�S�p25�����ȓ��Őݒ肵�Ă�������"
                                    Exit Function
                                End If
                            End If
                        Case 9
                            '����
                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",����,�����͂ł�"
                                Exit Function
                            Else
                                '���l�`�F�b�N
                                If IsNumeric((strSplitValue(intCnt))) = False Then
                                    pError = pSeqNo & ",����,���ʂɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���B"
                                    Exit Function
                                Else
                                    '0�A1�A2�ȊO�͋����Ȃ�
                                    Select Case CLng(strSplitValue(intCnt))
                                        Case 0, 1, 2
                                        Case Else
                                            pError = pSeqNo & ",����,���ʂ�0�`2�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"
                                            Exit Function
                                    End Select

                                End If
                            End If
                        Case 10
                            '���ID
                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",���ID,�����͂ł�"
                                Exit Function
                            Else
                                '���l�`�F�b�N
                                If IsNumeric((strSplitValue(intCnt))) = False Then
                                    pError = pSeqNo & ",���ID,���ID�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���B"
                                    Exit Function
                                Else
                                    '�����`�F�b�N
                                    If strSplitValue(intCnt).Trim.Length > 3 Then
                                        pError = pSeqNo & ",���ID,���ID���R���𒴂��Đݒ肳��Ă��܂�"

                                        Exit Function
                                    End If

                                    '��ڃ}�X�^���݃`�F�b�N
                                    sql = New StringBuilder(128)
                                    oraReader = New MyOracleReader(MainDB)
                                    sql.Append(" SELECT * FROM HIMOMAST")
                                    sql.Append(" WHERE")
                                    sql.Append(" GAKKOU_CODE_H  ='" & Trim(strSplitValue(1)).PadLeft(10, "0"c) & "'")
                                    sql.Append(" AND")
                                    sql.Append(" GAKUNEN_CODE_H =" & Trim(strSplitValue(4)))
                                    sql.Append(" AND")
                                    sql.Append(" HIMOKU_ID_H    ='" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                                    If oraReader.DataReader(sql) = False Then

                                        pError = pSeqNo & ",���ID,��ڃ}�X�^�ɓo�^����Ă��Ȃ����ID��ݒ肷�邱�Ƃ͂ł��܂���"
                                        oraReader.Close()
                                        Return False
                                    End If
                                    oraReader.Close()
                                End If
                            End If

                        Case 11, 13, 15, 17, 19, 21, 23, 25, 27, 29
                            '�������@
                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",�������@" & Format((intCnt - 9) \ 2, "00") & ",�������@�������͂ł��B"
                                Exit Function
                            End If

                            '���l�`�F�b�N
                            If IsNumeric((strSplitValue(intCnt))) = False Then
                                pError = pSeqNo & ",�������@" & Format((intCnt - 9) \ 2, "00") & ",�������@�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"
                                Exit Function
                            Else
                                '0�A1�ȊO�͋����Ȃ�
                                Select Case CLng(strSplitValue(intCnt))
                                    Case 0, 1
                                    Case Else
                                        pError = pSeqNo & ",�������@" & Format((intCnt - 9) \ 2, "00") & ",�������@��0,1�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"
                                        Exit Function
                                End Select

                            End If

                        Case 12, 14, 16, 18, 20, 22, 24, 26, 28, 30
                            '��ڋ��z
                            '�����̓`�F�b�N
                            If IsNumeric(strSplitValue(intCnt)) = False Then
                                pError = pSeqNo & ",��ڋ��z" & Format((intCnt - 10) \ 2, "00") & ",��ڋ��z�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"
                                Exit Function
                            End If

                            If strSplitValue(intCnt).Trim.Length > 12 Then
                                pError = pSeqNo & ",��ڋ��z" & Format((intCnt - 10) \ 2, "00") & ",��ڋ��z���P�Q���𒴂��Đݒ肳��Ă��܂�"
                                Exit Function
                            End If

                        Case 31
                            '���v���z�@�`�F�b�N�Ȃ�
                        Case 32
                            '�戵���Z�@�փR�[�h

                            '���l�`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Or IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",�戵���Z�@��,���Z�@�փR�[�h�ɋ󔒂܂��͐��l�ł͂Ȃ��l���ݒ肳��Ă��܂�"

                                Exit Function
                            End If

                            '�����`�F�b�N
                            If strSplitValue(intCnt).Trim.Length > 4 Then

                                pError = pSeqNo & ",�戵���Z�@��,���Z�@�փR�[�h���S���𒴂��Ă��܂�"

                                Exit Function
                            End If
                        Case 33
                            '���Z�@�֖��`�F�b�N�Ȃ�
                        Case 34
                            '�戵�x�X�R�[�h
                            '���l�`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Or IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",�戵�x�X,�x�X�R�[�h�ɋ󔒂܂��͐��l�ȊO�̒l���ݒ肳��Ă��܂�"

                                Exit Function
                            End If

                            '�����`�F�b�N
                            If strSplitValue(intCnt).Trim.Length > 3 Then

                                pError = pSeqNo & ",�戵�x�X,�x�X�R�[�h���R���𒴂��Ă��܂�"

                                Exit Function
                            End If

                            If Trim(strSplitValue(40)) = "0" Then
                                '���Z�@�փ}�X�^���݃`�F�b�N
                                sql = New StringBuilder(128)
                                oraReader = New MyOracleReader(MainDB)
                                sql.Append("SELECT * FROM TENMAST ")
                                sql.Append(" WHERE")
                                sql.Append(" KIN_NO_N = '" & Trim(strSplitValue(intCnt - 2)).PadLeft(4, "0"c) & "'")
                                sql.Append(" AND")
                                sql.Append(" SIT_NO_N = '" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                                If oraReader.DataReader(sql) = False Then

                                    pError = pSeqNo & ",�戵�x�X,���Z�@�փ}�X�^�ɓo�^����Ă��܂���"
                                    oraReader.Close()
                                    Return False
                                End If
                                oraReader.Close()
                            End If
                        Case 35
                            '�x�X���`�F�b�N�Ȃ�
                        Case 36
                            '�Ȗ�
                            If Trim(strSplitValue(intCnt)) = "" Or IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",�Ȗ�,�Ȗڂɋ󔒂܂��͐��l�ȊO�̒l���ݒ肳��Ă��܂�"

                                Exit Function
                            Else
                                If Trim(strSplitValue(intCnt)).Length > 2 Then
                                    pError = pSeqNo & ",�Ȗ�,�Ȗڂ�2���𒴂��Ă��܂�"
                                    Exit Function
                                End If
                                Select Case CDec(Trim(strSplitValue(intCnt)))
                                    Case 2, 1, 37, 5, 9
                                    Case Else
                                        pError = pSeqNo & ",�Ȗ�,���݂��Ȃ��Ȗڂ��ݒ肳��Ă��܂�"
                                        Exit Function
                                End Select
                            End If
                        Case 37
                            '�����ԍ�
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",�戵�����ԍ�,�����͂ł�"

                                Exit Function
                            End If

                            '���l�`�F�b�N  2007/02/12
                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",�戵�����ԍ�,�����ԍ��ɐ��l�ȊO�̒l���ݒ肳��Ă��܂�"

                                Exit Function
                            End If

                            '�����`�F�b�N 2007/02/12
                            If strSplitValue(intCnt).Trim.Length > 7 Then

                                pError = pSeqNo & ",�戵�����ԍ�,�����ԍ����V���𒴂��Đݒ肳��Ă��܂�"

                                Exit Function
                            End If


                        Case 38
                            '���`�l(��)
                            '�J�X�^�}�C�Y�@�����`�F�b�N�E���擾���s���B

                            If strSplitValue(intCnt).Trim = STR_JIKINKO_CODE Then
                                '�����}�X�^���݃`�F�b�N
                                sql = New StringBuilder(128)
                                oraReader = New MyOracleReader(MainDB)
                                sql.Append("SELECT KOKYAKU_KNAME_D, KOKYAKU_NNAME_D FROM KDBMAST ")
                                sql.Append(" WHERE")
                                sql.Append(" TSIT_NO_D = '" & Trim(strSplitValue(intCnt - 4)).PadLeft(3, "0"c) & "'")
                                sql.Append(" AND")
                                sql.Append(" KAMOKU_D = '" & Trim(strSplitValue(intCnt - 2)).PadLeft(2, "0"c) & "'")
                                sql.Append(" AND")
                                sql.Append(" KOUZA_D = '" & Trim(strSplitValue(intCnt - 1)).PadLeft(7, "0"c) & "'")
                                sql.Append(" AND")
                                sql.Append(" KATU_KOUZA_D = '1'") '�������̂ݎg�p����

                                If oraReader.DataReader(sql) = False Then
                                    pError = pSeqNo & ",��,�����}�X�^(KDBMAST)�ɓo�^����Ă��܂���B�i���툵���j"
                                    oraReader.Close()
                                    'Return False
                                Else
                                    strADDRESS = ""
                                    strTELNO = ""
                                    oraReader.Close()
                                End If
                            End If

                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",�戵���`�l(��),�����͂ł�"

                                Exit Function
                            Else

                                '�K��O�����`�F�b�N 2006/04/16
                                If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = False Then
                                    pError = pSeqNo & ",�戵���`�l(��),�K��O�������L��܂�"
                                    Exit Function
                                End If
                                '�������`�F�b�N 2006/05/10
                                If Trim(strSplitValue(intCnt)).Length > 40 Then
                                    pError = pSeqNo & ",�戵���`�l(��),���p40�����ȓ��Őݒ肵�Ă�������"
                                    Exit Function
                                End If
                            End If

                            strKANA_MEI = Trim(strSplitValue(intCnt))

                        Case 39    '���`�l����

                            If Trim(strSplitValue(intCnt)) <> "" Then
                                '�Q�o�C�g�ϊ�
                                strSplitValue(intCnt) = StrConv(Trim(strSplitValue(intCnt)), VbStrConv.Wide)
                                '�������`�F�b�N
                                If Trim(strSplitValue(intCnt)).Length > 25 Then
                                    pError = pSeqNo & ",���`�l(����),�S�p25�����ȓ��Őݒ肵�Ă�������"
                                    Exit Function
                                End If
                            End If

                            strKANJI_MEI = Trim(strSplitValue(intCnt))

                        Case 40
                            '�U�֕��@
                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",�U�֕��@,�����͂ł�"
                                Exit Function
                            Else
                                '���l�`�F�b�N
                                If IsNumeric((strSplitValue(intCnt))) = False Then
                                    pError = pSeqNo & ",�U�֕��@,�U�֕��@�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���B"
                                    Exit Function
                                Else
                                    '0�A1�A2�ȊO�͋����Ȃ�
                                    Select Case CLng(strSplitValue(intCnt))
                                        Case 0, 1, 2
                                        Case Else
                                            pError = pSeqNo & ",�U�֕��@,�U�֕��@��0�`2�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"
                                            Exit Function
                                    End Select

                                End If
                            End If
                        Case 41
                        Case 42
                        Case 43
                            '���敪
                            '�����̓`�F�b�N
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",���敪,�����͂ł�"
                                Exit Function
                            Else
                                '���l�`�F�b�N
                                If IsNumeric((strSplitValue(intCnt))) = False Then
                                    pError = pSeqNo & ",���敪,���敪�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���B"
                                    Exit Function
                                Else
                                    '0�A9�ȊO�͋����Ȃ�
                                    Select Case CLng(strSplitValue(intCnt))
                                        Case 0, 9
                                        Case Else
                                            pError = pSeqNo & ",���敪,���敪��0,9�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"
                                            Exit Function
                                    End Select

                                End If
                            End If
                    End Select
                Next intCnt
            Else
                pError = pSeqNo & ",���ڐ�,���k���f�[�^�̍��ڐ��ɕs��������܂�"
                'pError = pSeqNo & ",���k���f�[�^�̍��ڐ��ɕs��������܂�"
                Exit Function
            End If

            PFUNC_CHECK_SEITO = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���k�`�F�b�N��O�G���[", "���s", intCnt & ex.Message)
            Return False
        End Try
    End Function
    Private Function PFUNC_INSERT_SEITO(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean
        '�V�K�o�^
        Dim intCnt As Integer = 0
        Dim strSplitValue() As String
        Dim strRET As String = ""
        Dim sql As StringBuilder
        Dim strSql As String = ""
        PFUNC_INSERT_SEITO = False
        Try

            strSplitValue = Split(pLineValue, ",")

            'SQL���쐬
            For intTuki As Integer = 1 To 12
                sql = New StringBuilder(128)
                strSql = ""
                Dim TUKI_FURIKIN As Long = 0
                For intCnt = 0 To 65
                    Select Case intCnt
                        Case 0
                            If strSplitValue(intCnt).Trim = "9" Then
                                Return True
                                Exit Function
                            Else
                                sql.Append(" INSERT INTO SEITOMAST VALUES(")
                            End If
                        Case 1
                            '�w�Z�R�[�h
                            sql.Append("'" & strSplitValue(intCnt).Trim.PadLeft(10, "0"c) & "'")

                        Case 2
                            '�N�x
                            sql.Append("," & "'" & strSplitValue(intCnt).Trim.PadLeft(4, "0"c) & "'")

                        Case 3, 4, 5
                            '�ʔ�,�w�N����,�׽����
                            sql.Append("," & Trim(strSplitValue(intCnt)))
                        Case 6
                            '���k�ԍ�
                            sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(7, "0"c) & "'")
                        Case 7 '���k����(��)
                            If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = True Then
                                sql.Append(",'" & strRET & "'")
                            Else
                                sql.Append(",'" & Space(40) & "'")
                            End If
                        Case 8
                            '���k����(����)
                            sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                        Case 9
                            '����
                            '�l�Ȃ��̏ꍇ�͂Q�Œ�
                            Select Case Trim(strSplitValue(intCnt))
                                Case "0", "1", "2"
                                    sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                                Case Else
                                    sql.Append(",'2'")
                            End Select
                        Case 10
                            '�戵���Z�@�փR�[�h
                            If Trim(strSplitValue(32)) <> "" Then
                                sql.Append(",'" & Trim(strSplitValue(32)).PadLeft(4, "0"c) & "'")
                            Else
                                sql.Append(",' '")
                            End If
                        Case 11
                            '�戵�x�X�R�[�h
                            If Trim(strSplitValue(34)) <> "" Then
                                sql.Append(",'" & Trim(strSplitValue(34)).PadLeft(3, "0"c) & "'")
                            Else
                                sql.Append(",' '")
                            End If
                        Case 12
                            '�Ȗ�
                            Select Case Trim(strSplitValue(36)).PadLeft(2, "0"c)
                                Case "01", "02", "05", "09", "37"
                                    sql.Append(",'" & Trim(strSplitValue(36)).PadLeft(2, "0"c) & "'")
                                Case Else
                                    sql.Append(",'02'")
                            End Select
                        Case 13
                            '�����ԍ�
                            sql.Append(",'" & Trim(strSplitValue(37)).PadLeft(7, "0"c) & "'")
                        Case 14
                            '���`�l(��) 2007/02/12
                            If ConvKanaNGToKanaOK(strSplitValue(38), strRET) = True Then
                                sql.Append(",'" & strRET & "'")
                            Else
                                sql.Append(",'" & Space(40) & "'")
                            End If
                        Case 15
                            '���`�l(����)
                            If Trim(strSplitValue(39)) = "" Then
                                sql.Append(",'" & Space(50) & "'")
                            Else
                                sql.Append(",'" & Trim(strSplitValue(39)) & "'")
                            End If
                        Case 16
                            '�U�֕��@
                            '�l�Ȃ��̏ꍇ�͂O�Œ�
                            Select Case Trim(strSplitValue(40))
                                Case "0", "1", "2"
                                    sql.Append(",'" & Trim(strSplitValue(40)) & "'")
                                Case Else
                                    sql.Append(",'0'")
                            End Select
                        Case 17
                            '�_��ҏZ��
                            sql.Append(",'" & Space(50) & "'")
                        Case 18
                            ' �J�X�^�}�C�Y
                            sql.Append(",'" & Space(13) & "'")
                        Case 19
                            '���敪
                            '�l�Ȃ��̏ꍇ�͂O�Œ�
                            Select Case Trim(strSplitValue(43))
                                Case "0", "9"
                                    sql.Append(",'" & Trim(strSplitValue(43)) & "'")
                                Case Else
                                    sql.Append(",'0'")
                            End Select
                        Case 20
                            '�i���敪
                            '��{�O�Œ�i�i������j
                            sql.Append(",'0'")
                        Case 21
                            '���ID
                            sql.Append(",'" & Trim(strSplitValue(10)).PadLeft(3, "0"c) & "'")
                        Case 22, 24 To 26
                            '���q�t���O ,���q�ʔ�,���q�w�N,���q�N���X
                            sql.Append(",'0'")
                        Case 23
                            '���q���w�N�x
                            sql.Append(",'" & Space(4) & "'")
                        Case 27
                            '���q���k�ԍ�
                            sql.Append(",'" & Space(7) & "'")
                        Case 28
                            '������
                            sql.Append(",'" & Format(intTuki, "00") & "'")

                        Case 29, 31, 33, 35, 37, 39, 41, 43, 45, 47
                            '�������@1�`10 
                            sql.Append(",'" & Trim(strSplitValue(intCnt - 18)) & "'")
                        Case 49, 51, 53, 55, 57
                            '�������@11�`15 
                            sql.Append(", '0'")

                        Case 30, 32, 34, 36, 38, 40, 42, 44, 46, 48
                            '�������z1�`10
                            'If Trim(strSplitValue(intCnt - 18)) = "1" Then
                            sql.Append(", " & CLng(Trim(strSplitValue(intCnt - 18))))
                            'Else
                            '    sql.Append(",0")
                            'End If

                        Case 50, 52, 54, 56, 58
                            '�������z11�`15�@0�~�Œ�
                            sql.Append(",0")

                        Case 59
                            '�쐬���t
                            sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")

                        Case 60
                            '�X�V���t
                            sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")

                            '20130308 maeda
                            'Case 61 To 65
                            ''�\��
                            'sql.Append(",''")
                        Case 61
                            '�\��1
                            sql.Append(",''")
                        Case 62
                            '�\��2
                            sql.Append(",''")
                        Case 63
                            '�\��3
                            sql.Append(",''")
                        Case 64
                            '�\��4
                            sql.Append(",''")
                        Case 65
                            '�\��5
                            sql.Append(",''")
                            '20130308 maeda

                    End Select
                Next intCnt
                sql.Append(" )")
                '�d���`�F�b�N
                strSql = " SELECT * FROM SEITOMAST"
                strSql += " WHERE"
                strSql += " GAKKOU_CODE_O ='" & Trim(strSplitValue(1)).PadLeft(10, "0"c) & "'"
                strSql += " AND"
                strSql += " NENDO_O ='" & Trim(strSplitValue(2)) & "'"
                strSql += " AND"
                strSql += " TUUBAN_O =" & Trim(strSplitValue(3))
                strSql += " AND"
                strSql += " TUKI_NO_O ='" & Format(intTuki, "00") & "'"

                Dim oraReader As New MyOracleReader(MainDB)
                If oraReader.DataReader(strSql) = True Then
                    pError = pSeqNo & ",��L�[,�f�[�^���d�����Ă��܂�"
                    oraReader.Close()
                    Return False
                End If

                If MainDB.ExecuteNonQuery(sql) <> 1 Then
                    pError = pSeqNo & ",�ړ�,�o�^�������ɃG���[���������܂���"
                    Return False
                End If
            Next intTuki

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���kSQL��O�G���[", "���s", intCnt & ex.Message)
        End Try

    End Function
    Private Function PFUNC_UPDATE_SEITO(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean
        '�X�V
        Dim strSplitValue() As String
        Dim strRET As String = ""
        Dim sql As StringBuilder
        Dim strSql As String = ""
        PFUNC_UPDATE_SEITO = False
        Try

            strSplitValue = Split(pLineValue, ",")

            'SQL���쐬
            For intTuki As Integer = 1 To 12
                sql = New StringBuilder(128)
                strSql = ""
                Dim count As Integer = 0
                Dim TUKI_FURIKIN As Long = 0
                sql.Append(" UPDATE SEITOMAST SET ")
                sql.Append(" GAKUNEN_CODE_O = " & Trim(strSplitValue(4)))
                sql.Append(", CLASS_CODE_O = " & Trim(strSplitValue(5)))
                sql.Append(", SEITO_NO_O = '" & Trim(strSplitValue(6)).PadLeft(7, "0"c) & "'")

                '���k�����J�i
                If ConvKanaNGToKanaOK(strSplitValue(7), strRET) = True Then
                    sql.Append(", SEITO_KNAME_O = '" & strRET & "'")
                End If
                '���k������
                strSplitValue(8) = StrConv(Trim(strSplitValue(8)), VbStrConv.Wide)

                sql.Append(", SEITO_NNAME_O = '" & strSplitValue(8) & "'")
                sql.Append(", SEIBETU_O = '" & Trim(strSplitValue(9)) & "'")
                sql.Append(", TKIN_NO_O = '" & Trim(strSplitValue(32)).PadLeft(4, "0"c) & "'")
                sql.Append(", TSIT_NO_O = '" & Trim(strSplitValue(34)).PadLeft(3, "0"c) & "'")
                sql.Append(", HIMOKU_ID_O = '" & Trim(strSplitValue(10)).PadLeft(3, "0"c) & "'")
                sql.Append(", KAMOKU_O = '" & Trim(strSplitValue(36)).PadLeft(2, "0"c) & "'")
                sql.Append(", KOUZA_O = '" & Trim(strSplitValue(37)).PadLeft(7, "0"c) & "'")

                '���`�l�J�i
                '�J�X�^�}�C�Y
                If ConvKanaNGToKanaOK(strSplitValue(38), strRET) = True Then
                    sql.Append(", MEIGI_KNAME_O = '" & strRET & "'")
                End If

                'strSplitValue(39) = StrConv(Trim(strSplitValue(39)), VbStrConv.Wide)
                '���`�l�����E�Z���E�d�b�ԍ�
                '�J�X�^�}�C�Y
                sql.Append(", MEIGI_NNAME_O = '" & Trim(strSplitValue(39)) & "'")
                sql.Append(", KEIYAKU_NJYU_O = '" & strADDRESS & "'")
                sql.Append(", KEIYAKU_DENWA_O = '" & strTELNO & "'")

                sql.Append(", FURIKAE_O = '" & Trim(strSplitValue(40)) & "'")
                sql.Append(", KAIYAKU_FLG_O = '" & Trim(strSplitValue(43)) & "'")

                '�������̂ݔ�ڋ��z�ύX���遨2011/09/16�@�i�J�X�^�}�C�Y�j�������ȊO���ύX����
                ''If strSEIKYUTUKI.Substring(4, 2) = Format(intTuki, "00") Then
                '�������@�E�������z1�`10
                Dim intCNT As Integer = 11
                For count = 1 To 10
                    sql.Append(", SEIKYU" & Format(count, "00") & "_O = '" & Trim(strSplitValue(intCNT)) & "'")

                    sql.Append(", KINGAKU" & Format(count, "00") & "_O = " & CLng(strSplitValue(intCNT + 1)))

                    intCNT += 2
                Next count
                ''End If

                '�X�V���t
                sql.Append(", KOUSIN_DATE_O = '" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")

                sql.Append(" WHERE GAKKOU_CODE_O = '" & strSplitValue(1).Trim.PadLeft(10, "0"c) & "'")
                sql.Append(" AND NENDO_O = '" & strSplitValue(2).Trim.PadLeft(4, "0"c) & "'")
                sql.Append(" AND TUUBAN_O = '" & Trim(strSplitValue(3)) & "'")
                sql.Append(" AND TUKI_NO_O = '" & Format(intTuki, "00") & "'")

                '���݃`�F�b�N
                strSql = " SELECT * FROM SEITOMAST"
                strSql += " WHERE"
                strSql += " GAKKOU_CODE_O ='" & Trim(strSplitValue(1)).PadLeft(10, "0"c) & "'"
                strSql += " AND"
                strSql += " NENDO_O ='" & Trim(strSplitValue(2)) & "'"
                strSql += " AND"
                strSql += " TUUBAN_O =" & Trim(strSplitValue(3))
                strSql += " AND"
                strSql += " TUKI_NO_O ='" & Format(intTuki, "00") & "'"

                Dim oraReader As New MyOracleReader(MainDB)
                If oraReader.DataReader(strSql) = False Then
                    pError = pSeqNo & ",�\���敪,�X�V�Ώۂ̐��k�����݂��܂���"
                    oraReader.Close()
                    Return False
                End If

                If MainDB.ExecuteNonQuery(sql) < 0 Then
                    pError = pSeqNo & ",�ړ�,�X�V�������ɃG���[���������܂���"
                    Return False
                End If
            Next intTuki

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���kSQL��O�G���[", "���s", ex.Message)
        End Try

    End Function
    Private Function PFUNC_DELETE_SEITO(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean
        '�폜
        Dim strSplitValue() As String
        Dim sql As StringBuilder

        PFUNC_DELETE_SEITO = False
        Try

            strSplitValue = Split(pLineValue, ",")

            'SQL���쐬
            sql = New StringBuilder(128)
            sql.Append("DELETE FROM SEITOMAST ")
            sql.Append(" WHERE GAKKOU_CODE_O = '" & strSplitValue(1).Trim.PadLeft(10, "0"c) & "'")
            sql.Append(" AND NENDO_O = '" & strSplitValue(2).Trim.PadLeft(4, "0"c) & "'")
            sql.Append(" AND TUUBAN_O = '" & Trim(strSplitValue(3)) & "'")

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                pError = pSeqNo & ",�ړ�,�폜�������ɃG���[���������܂���"
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���kSQL��O�G���[", "���s", ex.Message)
        End Try

    End Function
#End Region

    Private Function Get_HIMOKU_INFO(ByVal gakkou_code As String, ByRef kin_no As String, ByRef sit_no As String, ByRef kouza As String) As Boolean
        '�w�Z�}�X�^�Q�̎擾
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        sql.Append(" SELECT TKIN_NO_T,TSIT_NO_T,KOUZA_T FROM GAKMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_T ='" & gakkou_code & "'")

        If oraReader.DataReader(sql) = True Then
            kin_no = oraReader.GetString("TKIN_NO_T").ToString
            sit_no = oraReader.GetString("TSIT_NO_T").ToString
            kouza = oraReader.GetString("KOUZA_T").ToString
        Else
            oraReader.Close()
            Return False
        End If

        oraReader.Close()
        Return True

    End Function
    Private Function Get_SEITO_UPDATE_BEFORE(ByVal GAKCODE As String, ByVal NENDO As Integer, ByVal TUUBAN As Integer, ByVal strRec As String, ByRef strTBL As String) As Boolean
        '�X�V�O���̎擾�i���k�}�X�^�j
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)
        Dim strSplitValue() As String

        Dim �\���敪 As String = ""
        Dim �˗��l�R�[�h As String = ""
        Dim �o�^�N�x As String = ""
        Dim �ʔ� As String = ""
        Dim �w�N As String = ""
        Dim �N���X As String = ""
        Dim ���k�ԍ� As String = ""
        Dim ���k���J�i As String = ""
        Dim ���k������ As String = ""
        Dim ���� As String = ""
        Dim ��ڂh�c As String = ""
        Dim �������@�t���O�P As String = ""
        Dim ��ڂP As String = ""
        Dim �������@�t���O�Q As String = ""
        Dim ��ڂQ As String = ""
        Dim �������@�t���O�R As String = ""
        Dim ��ڂR As String = ""
        Dim �������@�t���O�S As String = ""
        Dim ��ڂS As String = ""
        Dim �������@�t���O�T As String = ""
        Dim ��ڂT As String = ""
        Dim �������@�t���O�U As String = ""
        Dim ��ڂU As String = ""
        Dim �������@�t���O�V As String = ""
        Dim ��ڂV As String = ""
        Dim �������@�t���O�W As String = ""
        Dim ��ڂW As String = ""
        Dim �������@�t���O�X As String = ""
        Dim ��ڂX As String = ""
        Dim �������@�t���O�P�O As String = ""
        Dim ��ڂP�O As String = ""
        Dim ���v���z As String = ""
        Dim ���Z�@�փR�[�h As String = ""
        Dim ���Z�@�֖� As String = ""
        Dim �x�X�R�[�h As String = ""
        Dim �x�X�� As String = ""
        Dim �Ȗ� As String = ""
        Dim �����ԍ� As String = ""
        Dim �������`�J�i As String = ""
        Dim �������`���� As String = ""
        Dim �U�֕��@ As String = ""
        Dim �Z�� As String = ""
        Dim �d�b�ԍ� As String = ""
        Dim �����@ As String = ""

        sql.Append(" SELECT * FROM SEITOMASTVIEW")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & GAKCODE & "'")
        sql.Append(" AND")
        sql.Append(" NENDO_O = " & NENDO)
        sql.Append(" AND")
        sql.Append(" TUUBAN_O = " & TUUBAN)
        sql.Append(" AND")
        sql.Append(" TUKI_NO_O = '" & strSEIKYUTUKI.Substring(4, 2) & "'")

        If oraReader.DataReader(sql) = True Then
            strSplitValue = Split(strRec, ",")
            Dim intCnt As Integer
            For intCnt = 0 To UBound(strSplitValue)
                Select Case intCnt
                    Case 0
                        �\���敪 = "2"
                    Case 1
                        �˗��l�R�[�h = ""
                    Case 2
                        �o�^�N�x = ""
                    Case 3
                        �ʔ� = ""
                    Case 4
                        If oraReader.GetInt("GAKUNEN_CODE_O") = CInt(strSplitValue(intCnt)) Then
                            �w�N = ""
                        Else
                            �w�N = oraReader.GetInt("GAKUNEN_CODE_O").ToString
                        End If
                    Case 5
                        If oraReader.GetInt("CLASS_CODE_O") = CInt(strSplitValue(intCnt)) Then
                            �N���X = ""
                        Else
                            �N���X = oraReader.GetInt("CLASS_CODE_O").ToString
                        End If
                    Case 6
                        If oraReader.GetString("SEITO_NO_O").Trim = strSplitValue(intCnt) Then
                            ���k�ԍ� = ""
                        Else
                            ���k�ԍ� = oraReader.GetString("SEITO_NO_O").Trim
                        End If
                    Case 7
                        If oraReader.GetString("SEITO_KNAME_O").Trim = strSplitValue(intCnt) Then
                            ���k���J�i = ""
                        Else
                            ���k���J�i = oraReader.GetString("SEITO_KNAME_O").Trim
                        End If
                    Case 8
                        If oraReader.GetString("SEITO_NNAME_O").Trim = strSplitValue(intCnt) Then
                            ���k������ = ""
                        Else
                            ���k������ = oraReader.GetString("SEITO_NNAME_O").Trim
                        End If
                    Case 9
                        If oraReader.GetString("SEIBETU_O").Trim = strSplitValue(intCnt) Then
                            ���� = ""
                        Else
                            ���� = oraReader.GetString("SEIBETU_O").Trim
                        End If
                    Case 10
                        If oraReader.GetString("HIMOKU_ID_O").Trim = strSplitValue(intCnt) Then
                            ��ڂh�c = ""
                        Else
                            ��ڂh�c = oraReader.GetString("HIMOKU_ID_O").Trim
                        End If
                    Case 11
                        '�J�X�^�}�C�Y �ʋ��z�\�������邽�ߐ������@�t���O�P�͏�Ɏ擾������i2011/09/16�j
                        �������@�t���O�P = oraReader.GetString("SEIKYU01_O").Trim
                        'If oraReader.GetString("SEIKYU01_O").Trim = strSplitValue(intCnt) Then
                        '    �������@�t���O�P = ""
                        'Else
                        '    �������@�t���O�P = oraReader.GetString("SEIKYU01_O").Trim
                        'End If
                    Case 12
                        If oraReader.GetInt64("KINGAKU01_O") = CLng(strSplitValue(intCnt)) Then
                            ��ڂP = ""
                        Else
                            ��ڂP = oraReader.GetInt64("KINGAKU01_O").ToString
                        End If
                    Case 13
                        If oraReader.GetString("SEIKYU02_O").Trim = strSplitValue(intCnt) Then
                            �������@�t���O�Q = ""
                        Else
                            �������@�t���O�Q = oraReader.GetString("SEIKYU02_O").Trim
                        End If
                    Case 14
                        If oraReader.GetInt64("KINGAKU02_O") = CLng(strSplitValue(intCnt)) Then
                            ��ڂQ = ""
                        Else
                            ��ڂQ = oraReader.GetInt64("KINGAKU02_O").ToString
                        End If
                    Case 15
                        If oraReader.GetString("SEIKYU03_O").Trim = strSplitValue(intCnt) Then
                            �������@�t���O�R = ""
                        Else
                            �������@�t���O�R = oraReader.GetString("SEIKYU03_O").Trim
                        End If
                    Case 16
                        If oraReader.GetInt64("KINGAKU03_O") = CLng(strSplitValue(intCnt)) Then
                            ��ڂR = ""
                        Else
                            ��ڂR = oraReader.GetInt64("KINGAKU03_O").ToString
                        End If
                    Case 17
                        If oraReader.GetString("SEIKYU04_O").Trim = strSplitValue(intCnt) Then
                            �������@�t���O�S = ""
                        Else
                            �������@�t���O�S = oraReader.GetString("SEIKYU04_O").Trim
                        End If
                    Case 18
                        If oraReader.GetInt64("KINGAKU04_O") = CLng(strSplitValue(intCnt)) Then
                            ��ڂS = ""
                        Else
                            ��ڂS = oraReader.GetInt64("KINGAKU04_O").ToString
                        End If
                    Case 19
                        If oraReader.GetString("SEIKYU05_O").Trim = strSplitValue(intCnt) Then
                            �������@�t���O�T = ""
                        Else
                            �������@�t���O�T = oraReader.GetString("SEIKYU05_O").Trim
                        End If
                    Case 20
                        If oraReader.GetInt64("KINGAKU05_O") = CLng(strSplitValue(intCnt)) Then
                            ��ڂT = ""
                        Else
                            ��ڂT = oraReader.GetInt64("KINGAKU05_O").ToString
                        End If
                    Case 21
                        If oraReader.GetString("SEIKYU06_O").Trim = strSplitValue(intCnt) Then
                            �������@�t���O�U = ""
                        Else
                            �������@�t���O�U = oraReader.GetString("SEIKYU06_O").Trim
                        End If
                    Case 22
                        If oraReader.GetInt64("KINGAKU06_O") = CLng(strSplitValue(intCnt)) Then
                            ��ڂU = ""
                        Else
                            ��ڂU = oraReader.GetInt64("KINGAKU06_O").ToString
                        End If
                    Case 23
                        If oraReader.GetString("SEIKYU07_O").Trim = strSplitValue(intCnt) Then
                            �������@�t���O�V = ""
                        Else
                            �������@�t���O�V = oraReader.GetString("SEIKYU07_O").Trim
                        End If
                    Case 24
                        If oraReader.GetInt64("KINGAKU07_O") = CLng(strSplitValue(intCnt)) Then
                            ��ڂV = ""
                        Else
                            ��ڂV = oraReader.GetInt64("KINGAKU07_O").ToString
                        End If
                    Case 25
                        If oraReader.GetString("SEIKYU08_O").Trim = strSplitValue(intCnt) Then
                            �������@�t���O�W = ""
                        Else
                            �������@�t���O�W = oraReader.GetString("SEIKYU08_O").Trim
                        End If
                    Case 26
                        If oraReader.GetInt64("KINGAKU08_O") = CLng(strSplitValue(intCnt)) Then
                            ��ڂW = ""
                        Else
                            ��ڂW = oraReader.GetInt64("KINGAKU08_O").ToString
                        End If
                    Case 27
                        If oraReader.GetString("SEIKYU09_O").Trim = strSplitValue(intCnt) Then
                            �������@�t���O�X = ""
                        Else
                            �������@�t���O�X = oraReader.GetString("SEIKYU09_O").Trim
                        End If
                    Case 28
                        If oraReader.GetInt64("KINGAKU09_O") = CLng(strSplitValue(intCnt)) Then
                            ��ڂX = ""
                        Else
                            ��ڂX = oraReader.GetInt64("KINGAKU09_O").ToString
                        End If
                    Case 29
                        If oraReader.GetString("SEIKYU10_O").Trim = strSplitValue(intCnt) Then
                            �������@�t���O�P�O = ""
                        Else
                            �������@�t���O�P�O = oraReader.GetString("SEIKYU10_O").Trim
                        End If
                    Case 30
                        If oraReader.GetInt64("KINGAKU10_O") = CLng(strSplitValue(intCnt)) Then
                            ��ڂP�O = ""
                        Else
                            ��ڂP�O = oraReader.GetInt64("KINGAKU10_O").ToString
                        End If
                    Case 31
                        ���v���z = ""
                    Case 32
                        If oraReader.GetString("TKIN_NO_O").Trim = strSplitValue(intCnt) Then
                            ���Z�@�փR�[�h = ""
                        Else
                            ���Z�@�փR�[�h = oraReader.GetString("TKIN_NO_O").Trim
                        End If
                    Case 33
                        ���Z�@�֖� = ""
                    Case 34
                        If oraReader.GetString("TSIT_NO_O").Trim = strSplitValue(intCnt) Then
                            �x�X�R�[�h = ""
                        Else
                            �x�X�R�[�h = oraReader.GetString("TSIT_NO_O").Trim
                        End If
                    Case 35
                        �x�X�� = ""
                    Case 36
                        If oraReader.GetString("KAMOKU_O").Trim = strSplitValue(intCnt) Then
                            �Ȗ� = ""
                        Else
                            �Ȗ� = oraReader.GetString("KAMOKU_O").Trim
                        End If
                    Case 37
                        If oraReader.GetString("KOUZA_O").Trim = strSplitValue(intCnt) Then
                            �����ԍ� = ""
                        Else
                            �����ԍ� = oraReader.GetString("KOUZA_O").Trim
                        End If
                    Case 38
                        �������`�J�i = ""
                    Case 39
                        �������`���� = ""
                    Case 40
                        If oraReader.GetString("FURIKAE_O").Trim = strSplitValue(intCnt) Then
                            �U�֕��@ = ""
                        Else
                            �U�֕��@ = oraReader.GetString("FURIKAE_O").Trim
                        End If
                    Case 41
                        �Z�� = ""
                    Case 42
                        �d�b�ԍ� = ""
                    Case 43
                        If oraReader.GetString("KAIYAKU_FLG_O").Trim = strSplitValue(intCnt) Then
                            �����@ = ""
                        Else
                            �����@ = oraReader.GetString("KAIYAKU_FLG_O").Trim
                        End If
                End Select
            Next

            strTBL = _
            �\���敪 & "," & _
            �˗��l�R�[�h & "," & _
            �o�^�N�x & "," & _
            �ʔ� & "," & _
            �w�N & "," & _
            �N���X & "," & _
            ���k�ԍ� & "," & _
            ���k���J�i & "," & _
            ���k������ & "," & _
            ���� & "," & _
            ��ڂh�c & "," & _
            �������@�t���O�P & "," & _
            ��ڂP & "," & _
            �������@�t���O�Q & "," & _
            ��ڂQ & "," & _
            �������@�t���O�R & "," & _
            ��ڂR & "," & _
            �������@�t���O�S & "," & _
            ��ڂS & "," & _
            �������@�t���O�T & "," & _
            ��ڂT & "," & _
            �������@�t���O�U & "," & _
            ��ڂU & "," & _
            �������@�t���O�V & "," & _
            ��ڂV & "," & _
            �������@�t���O�W & "," & _
            ��ڂW & "," & _
            �������@�t���O�X & "," & _
            ��ڂX & "," & _
            �������@�t���O�P�O & "," & _
            ��ڂP�O & "," & _
            ���v���z & "," & _
            ���Z�@�փR�[�h & "," & _
            ���Z�@�֖� & "," & _
            �x�X�R�[�h & "," & _
            �x�X�� & "," & _
            �Ȗ� & "," & _
            �����ԍ� & "," & _
            �������`�J�i & "," & _
            �������`���� & "," & _
            �U�֕��@ & "," & _
            �Z�� & "," & _
            �d�b�ԍ� & "," & _
            �����@

        Else
            oraReader.Close()
            Return False
        End If

        oraReader.Close()
        Return True

    End Function
    Private Function PFUNC_ERR_LOG_WRITE(ByVal pError As String) As Boolean

        Dim Obj_StreamWriter As StreamWriter

        PFUNC_ERR_LOG_WRITE = False

        Try
            '���O�̏�������
            Obj_StreamWriter = New StreamWriter(KobetuLogFileName, _
                                                True, _
                                                Encoding.GetEncoding("Shift_JIS"))
        Catch ex As Exception
            '
            Exit Function
        End Try

        Obj_StreamWriter.WriteLine(pError)
        Obj_StreamWriter.Flush()
        Obj_StreamWriter.Close()

        PFUNC_ERR_LOG_WRITE = True

    End Function
    Private Function PFUNC_PRINT_LOG_WRITE(ByVal strRec As String) As Boolean

        Dim Obj_StreamWriter As StreamWriter

        PFUNC_PRINT_LOG_WRITE = False

        Try
            '���O�̏�������
            Obj_StreamWriter = New StreamWriter(KobetuLogFileName2, _
                                                True, _
                                                Encoding.GetEncoding("Shift_JIS"))
        Catch ex As Exception
            '
            Exit Function
        End Try

        Obj_StreamWriter.WriteLine(strRec)
        Obj_StreamWriter.Flush()
        Obj_StreamWriter.Close()

        PFUNC_PRINT_LOG_WRITE = True

    End Function
    '20130405 maeda
    '�J���}��؂�ŕ������߂�
    ''' <summary>
    ''' �X�V�O�̋��w�N�N���X���k�ԍ������擾����
    ''' </summary>
    ''' <param name="GAKCODE">�w�Z�R�[�h</param>
    ''' <param name="NENDO">�N�x</param>
    ''' <param name="TUUBAN">�ʔ�</param>
    ''' <param name="strTBL">���w�N�N���X���k�ԍ����</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function GetSeitoInfo(ByVal GAKCODE As String, ByVal NENDO As Integer, ByVal TUUBAN As Integer, ByRef strTBL As String) As Boolean

        Dim ret As Boolean = False

        Try
            Dim sql As New StringBuilder(128)
            Dim oraReader As New MyOracleReader(MainDB)

            '�X�V�O���̎擾�i���k�}�X�^�j
            sql.Append(" SELECT * FROM SEITOMASTVIEW")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_O ='" & GAKCODE & "'")
            sql.Append(" AND")
            sql.Append(" NENDO_O = " & NENDO)
            sql.Append(" AND")
            sql.Append(" TUUBAN_O = " & TUUBAN)
            sql.Append(" AND")
            sql.Append(" TUKI_NO_O = '" & strSEIKYUTUKI.Substring(4, 2) & "'")

            If oraReader.DataReader(sql) = True Then
                strTBL = ""
                strTBL &= oraReader.GetItem("GAKUNEN_CODE_O") '�w�N
                strTBL &= "," & oraReader.GetItem("CLASS_CODE_O").Trim.PadLeft(2, "0"c) '�N���X
                strTBL &= "," & oraReader.GetItem("SEITO_NO_O").Trim.PadLeft(7, "0"c) '���k�ԍ�
            Else
                '�V�K�����̓G���[��
                strTBL = "9,99,9999999"
            End If

            oraReader.Close()

            ret = True

        Catch ex As Exception

        End Try

        Return ret

    End Function
    '20130405 maeda
#End Region

#Region "�C�x���g"
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- END

        '�w�Z���R���{�{�b�N�X�ݒ�
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = True Then
            cmbGAKKOUNAME.Focus()
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGAKKOUNAME.SelectedIndexChanged

        'COMBOBOX�I�����w�Z��,�w�Z�R�[�h�ݒ�
        lblGAKKOU_NAME.Text = cmbGAKKOUNAME.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGAKKOUNAME.SelectedIndex)

        '�w�N�e�L�X�g�{�b�N�X��FOCUS
        txtGAKKOU_CODE.Focus()

    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

End Class
