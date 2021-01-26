Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Imports CAstReports

Public Class KFGMAST530

#Region " ���ʕϐ���` "

    Private Lng_Seito_Ok_Cnt As Long
    Private Lng_Gak_Ok_Cnt As Long
    Private Lng_Kin_Ok_Cnt As Long

    Private Lng_Seito_Ng_Cnt As Long
    Private Lng_Gak_Ng_Cnt As Long

    'Private Lng_Csv_Ok_Cnt As String = 0
    'Private Lng_Csv_Ng_Cnt As String = 0

    Private Lng_Seito_SeqNo As Long
    Private Lng_Gak_SeqNo As Long

    Public STR���[�\�[�g�� As String = "0"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST530", "�w�Z�c�[���A�g���")
    Private ToolLOG As New CASTCommon.BatchLOG("KFGMAST530/ERROR", "�f�[�^�捞�G���[���O")

    Private Const msgTitle As String = "�w�Z�c�[���A�g���(KFGMAST530)"
    Private MainDB As MyOracle

    Private KobetuLogFileName As String '�G���[���O
    Private KobetuLogFileName2 As String '�Ǎ����O

    Private err_code As Integer = 0
    Private err_Filename As String = ""

    '�w�Z�c�[���쐬��A�捞��PATH�擾
    Private GAKTOOL_IN_PATH As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "SAVE_PATH")
    Private GAKTOOL_INBK_PATH As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "CSVBK")

    Private GAKTOOL_MOD_FILE As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "TORIKOMI_CSV_MOD")
    Private GAKTOOL_GAK_FILE As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "TORIKOMI_CSV_GAKKO")

    Private GAKTOOL_LOG_PATH As String = GFUNC_INI_READ("COMMON", "LOG")

    Private EXP_NEND As Integer
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAST530_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            If MessageBox.Show("�b�r�u����̃C���|�[�g���J�n���܂�", _
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

                    Lng_Seito_Ok_Cnt = 0
                    Lng_Seito_Ng_Cnt = 0
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


                    '*** �U�֓������׃��R�[�h ***
                    'CSV���́��}�X�^�o�^
                    If PFUNC_IMP_SEITO(Gakkou_Code, GAKTOOL_IN_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv", Lng_Seito_Ok_Cnt, Lng_Seito_Ng_Cnt) = False Then
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
                        If File.Exists(GAKTOOL_IN_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_IN_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv", GAKTOOL_INBK_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_IN_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv")
                        End If
                    End If

                    aoraReader.NextRead()
                    OK_ken += Lng_Seito_Ok_Cnt
                    NG_ken += Lng_Seito_Ng_Cnt
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
                MessageBox.Show("�C���|�[�g���I�����܂����B" & vbCrLf & "��������:" & Lng_Seito_Ok_Cnt & "���̃f�[�^��o�^���܂����B" & _
                                 vbCrLf & vbCrLf & "KFGMAIN070 ���k���ד���/KFGPRNT110 ���k���ד��̓`�F�b�N���X�g����œ��͓��e���m�F���Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Else

                MainDB.Rollback()

                '�t�@�C���o�b�N�A�b�v�߂��i�w�Z�f�[�^�j
                If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv") Then
                    FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                    File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                End If
                '�t�@�C���o�b�N�A�b�v�߂��i�������׃f�[�^�j
                If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv") Then
                    FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv")
                    File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv")
                End If


                '�G���[�����b�Z�[�W
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
                    sql.Append("SELECT ENTRI_FLG_S FROM G_SCHMAST ")
                    sql.Append(" WHERE")
                    sql.Append(" GAKKOU_CODE_S  = '" & gakkou_code_H & "' ")
                    sql.Append(" AND")
                    sql.Append(" SCH_KBN_S  = '2' ")
                    sql.Append(" AND")
                    sql.Append(" FURI_KBN_S  = '2' ")
                    sql.Append(" AND")
                    sql.Append(" FURI_DATE_S = '" & strGET_furi_date & "' ")

                    If oraReader.DataReader(sql) = False Then
                        MessageBox.Show("�U�֓����X�P�W���[�������݂��Ȃ��̂ŏ����ł��܂���B" & vbCrLf & "�f�[�^�U�֓�:" & strGET_furi_date, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Y���X�P�W���[���Ȃ� �f�[�^�U�֓�:" & strGET_furi_date, "���s", "")
                        oraReader.Close()
                        Return False
                    Else
                        If oraReader.GetItem("ENTRI_FLG_S").ToString = "1" Then
                            oraReader.Close()
                        Else
                            MessageBox.Show("KFGMAIN060 ���k���׍쐬���s���Ă��������B" & vbCrLf & "�f�[�^�U�֓�:" & strGET_furi_date, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                            oraReader.Close()
                            Return False
                        End If
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

    Private Function PFUNC_IMP_SEITO(ByVal gakkou_code_S As String, ByVal pCsvPath As String, ByRef lngOkCnt As Long, ByRef lngErrCnt As Long) As Boolean

        Dim red As System.IO.StreamReader = Nothing

        Dim strReadLine As String
        'Dim Shinsei_KBN As String   '�\���敪
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
                If PFUNC_CHECK_SEITO(gakkou_code_S, Lng_Seito_SeqNo, strReadLine, strErrorLine, NENDO, TUUBAN) = True Then
                    If PFUNC_UPDATE_SEITO(Lng_Seito_SeqNo, strReadLine, strErrorLine) = True Then
                        lngOkCnt += 1
                    Else
                        lngErrCnt += 1
                        blnSQL = False

                        '�װ۸ނ̏����o��
                        ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_UPDATE_SEITO", "���s", Replace(strErrorLine, ",", "/"))

                        'SQL�ŃG���[�����������ꍇ�͏������f
                        red.Close()
                        Exit Do
                    End If
                    
                Else
                    lngErrCnt += 1

                    '�װ۸ނ̏����o��
                    ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_CHECK_SEITO", "���s", Replace(strErrorLine, ",", "/"))

                End If

                Lng_Seito_SeqNo += 1
            Loop

            red.Close()
            Return True

        Catch ex As FileNotFoundException
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                MessageBox.Show("�t�@�C����������܂���B" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�U�֓������׃t�@�C���Ȃ�", "���s", ex.Message)
            Else
                Return True
            End If
        Catch ex As System.IO.IOException
            MessageBox.Show("�t�@�C�����g�p���ł��B" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�U�֓������׃t�@�C���A�N�Z�X", "���s", ex.Message)
            err_code = 1
            err_Filename = pCsvPath
            Return False
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�U�֓������׃`�F�b�N��O�G���[", "���s", ex.Message)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
        End Try

    End Function

    Private Function PFUNC_CHECK_SEITO(ByVal gakkou_code_C As String, ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String, ByRef NENDO As Integer, ByRef TUUBAN As Integer) As Boolean

        Dim intCnt As Integer
        Dim intCnt2 As Integer
        Dim strSplitValue() As String
        Dim strRET As String = ""

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader


        PFUNC_CHECK_SEITO = False
        Try

            strSplitValue = Split(pLineValue, ",")

            '���͓��e�`�F�b�N
            If strSplitValue.Length = 17 Then
                For intCnt = 0 To UBound(strSplitValue)
                    Select Case intCnt
                        Case 0
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

                        Case 1
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

                        Case 2
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

                        Case 3
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
                            sql.Append(" GAKKOU_CODE_G = '" & Trim(strSplitValue(0)).PadLeft(10, "0"c) & "'")
                            sql.Append(" AND")
                            sql.Append(" GAKUNEN_CODE_G = " & Trim(strSplitValue(intCnt)))

                            If oraReader.DataReader(sql) = False Then
                                pError = pSeqNo & ",�w�N�R�[�h,�w�N�R�[�h���w�Z�}�X�^�ɓo�^����Ă��܂���"
                                oraReader.Close()
                                Return False
                            End If
                            oraReader.Close()
                        Case 4
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
                                sql.Append(" GAKKOU_CODE_G = '" & Trim(strSplitValue(0)).PadLeft(10, "0"c) & "'")
                                sql.Append(" AND")
                                sql.Append(" GAKUNEN_CODE_G = " & Trim(strSplitValue(3)))
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
                        Case 5
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

                        Case 6
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
                        Case 7 '���k�������@�����`�F�b�N
                            If Trim(strSplitValue(intCnt)) <> "" Then
                                '�Q�o�C�g�ϊ�
                                strSplitValue(intCnt) = StrConv(Trim(strSplitValue(intCnt)), VbStrConv.Wide)
                                '�������`�F�b�N
                                If Trim(strSplitValue(intCnt)).Length > 25 Then
                                    pError = pSeqNo & ",���k����(����),�S�p25�����ȓ��Őݒ肵�Ă�������"
                                    Exit Function
                                End If
                            End If
                        Case 8
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
                        Case 9
                            '���z
                            '�����̓`�F�b�N
                            If IsNumeric(strSplitValue(intCnt)) = False Then
                                pError = pSeqNo & ",��ڋ��z" & Format((intCnt - 10) \ 2, "00") & ",��ڋ��z�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"
                                Exit Function
                            End If

                            If strSplitValue(intCnt).Trim.Length > 12 Then
                                pError = pSeqNo & ",��ڋ��z" & Format((intCnt - 10) \ 2, "00") & ",��ڋ��z���P�Q���𒴂��Đݒ肳��Ă��܂�"
                                Exit Function
                            End If

                        Case 10
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

                        Case 11
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

                        Case 12
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

                        Case 13
                            '�����ԍ�
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",�戵�����ԍ�,�����͂ł�"

                                Exit Function
                            End If

                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",�戵�����ԍ�,�����ԍ��ɐ��l�ȊO�̒l���ݒ肳��Ă��܂�"

                                Exit Function
                            End If

                            '�����`�F�b�N
                            If strSplitValue(intCnt).Trim.Length > 7 Then

                                pError = pSeqNo & ",�戵�����ԍ�,�����ԍ����V���𒴂��Đݒ肳��Ă��܂�"

                                Exit Function
                            End If


                        Case 14
                            '���`�l(��)

                        Case 15
                            '���`�l����


                        Case 16
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
                    End Select
                Next intCnt
            Else
                pError = pSeqNo & ",���ڐ�,�U�֓������׃f�[�^�̍��ڐ��ɕs��������܂�"
                'pError = pSeqNo & ",���k���f�[�^�̍��ڐ��ɕs��������܂�"
                Exit Function
            End If

            PFUNC_CHECK_SEITO = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�U�֓������׃f�[�^�`�F�b�N��O�G���[", "���s", intCnt & ex.Message)
            Return False
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
            sql = New StringBuilder(128)
            strSql = ""
            Dim count As Integer = 0
            Dim TUKI_FURIKIN As Long = 0
            sql.Append(" UPDATE G_ENTMAST1 SET ")
            sql.Append(" FURIKIN_E = " & Trim(strSplitValue(9)))
            sql.Append(", KOUSIN_DATE_E = '" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
            sql.Append(" WHERE GAKKOU_CODE_E = '" & strSplitValue(0).Trim.PadLeft(10, "0"c) & "'")
            sql.Append(" AND NENDO_E = '" & strSplitValue(1).Trim.PadLeft(4, "0"c) & "'")
            sql.Append(" AND TUUBAN_E = " & Trim(strSplitValue(2)))

            Dim RET As Integer = MainDB.ExecuteNonQuery(sql)

            If RET < 0 Then
                pError = pSeqNo & ",�ړ�,�X�V�������ɃG���[���������܂���"
                Return False
            ElseIf RET = 0 Then
                pError = pSeqNo & ",�ړ�,�X�V�Ώۂ�����܂���"
                Return False
            End If

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�U�֓�������SQL��O�G���[", "���s", ex.Message)
        End Try

    End Function
#End Region

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
