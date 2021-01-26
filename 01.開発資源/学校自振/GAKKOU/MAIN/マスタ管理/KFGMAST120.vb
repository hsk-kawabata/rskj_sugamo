Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon

Public Class KFGMAST120

#Region " ���ʕϐ���` "
    Private Lng_Csv_Ok_Cnt As Long
    Private Lng_Csv_Ng_Cnt As Long
    Private Lng_Csv_SeqNo As Long
    Private strSplitValue() As String
    Public STR���[�\�[�g�� As String = "0"

#End Region

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST120", "�V�����}�X�^�������")
    Private Const msgTitle As String = "�V�����}�X�^�������(KFGMAST120)"
    Private MainDB As MyOracle

    Private KobetuLogFileName As String

    '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- START
    Private GAKKOU_CODE_LENGTH As Integer = 10
    '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- END

#Region " Form Load "
    Private Sub KFGMAST120_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- START
            '�w�Z�c�[���̊w�Z�R�[�h�������擾����
            Dim strLen As String = GetFSKJIni("GTOOL", "GAKKOU_CODE_LENGTH")
            If Not strLen.ToUpper = "ERR" AndAlso strLen <> "" AndAlso Not strLen = Nothing Then
                GAKKOU_CODE_LENGTH = CInt(strLen)
            End If
            '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- END

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

        Try

            MainDB = New MyOracle

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ړ�)�J�n", "����", "")

            Cursor.Current = Cursors.WaitCursor()
            Dim strDIR As String

            '���͒l�`�F�b�N
            If Trim(txtGAKKOU_CODE.Text) = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If

            '�m�F���b�Z�[�W
            If MessageBox.Show(G_MSG0014I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            strDIR = CurDir()

            Lng_Csv_Ok_Cnt = 0
            Lng_Csv_Ng_Cnt = 0
            Lng_Csv_SeqNo = 1 '0��1�ɕύX 2006/04/16
            '�G���[���O�t�@�C���폜 2006/04/16
            KobetuLogFileName = Path.Combine(STR_LOG_PATH, "SINNYU" & txtGAKKOU_CODE.Text & ".log")
            If File.Exists(KobetuLogFileName) Then
                File.Delete(KobetuLogFileName)
            End If

            '���͐�̎w��
            '�t�@�C���̑I���_�C�A���O��\��
            Dim dlg As New Windows.Forms.OpenFileDialog

            dlg.ReadOnlyChecked = False

            dlg.CheckFileExists = True

            dlg.InitialDirectory = STR_CSV_PATH
            dlg.FileName = "SINNYU" & Trim(txtGAKKOU_CODE.Text)

            dlg.Title = "CSV�t�@�C���̑I��"
            dlg.Filter = "CSV�t�@�C�� (*.csv)|*.csv"
            dlg.FilterIndex = 1

            Select Case (dlg.ShowDialog())
                Case DialogResult.OK
                    '�_�C�A���OOK�{�^��������
                    Console.WriteLine(dlg.FileName)
                Case Else
                    '�_�C�A���OOK�{�^���ȊO������
                    ChDir(strDIR)
                    Exit Sub
            End Select

            MainDB.BeginTrans()

            'CSV���́��}�X�^�o�^
            If PFUNC_IMP_SINNYU(dlg.FileName, Lng_Csv_Ok_Cnt, Lng_Csv_Ng_Cnt) = False Then
                MainDB.Rollback()
                ChDir(strDIR)
                btnEnd.Focus()
                Return
            End If

            If Lng_Csv_Ng_Cnt = 0 Then     '����

                MainDB.Commit()

                '�������b�Z�[�W
                MessageBox.Show(String.Format(G_MSG0015I, Lng_Csv_Ok_Cnt), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else

                MainDB.Rollback()

                '�G���[�����b�Z�[�W
                MessageBox.Show(String.Format(G_MSG0046W, Lng_Csv_Ng_Cnt, KobetuLogFileName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            End If

            ChDir(strDIR)
            btnEnd.Focus()

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ړ�)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ړ�)�I��", "����", "")
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Sub
    Private Sub btnExp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExp.Click

        Try
            MainDB = New MyOracle

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo)�J�n", "����", "")

            Cursor.Current = Cursors.WaitCursor()
            Dim strDIR As String

            '���͒l�`�F�b�N
            If Trim(txtGAKKOU_CODE.Text) = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            Else
                '���[�\�[�g���擾 2007/02/15
                If PFUNC_GAKMAST2_GET() = False Then
                    Call GSUB_LOG(0, "�w�Z�}�X�^�ɓo�^����Ă��܂���")
                    txtGAKKOU_CODE.Focus()
                    Return
                End If
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text

            '�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0015I, "�ڏo"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            strDIR = CurDir()

            Lng_Csv_Ok_Cnt = 0
            Lng_Csv_Ng_Cnt = 0

            '���O�����ĕۑ��_�C�A���O�̕\��
            Dim dlg As New Windows.Forms.SaveFileDialog

            dlg.InitialDirectory = STR_CSV_PATH
            dlg.FileName = "SINNYU" & Trim(txtGAKKOU_CODE.Text)

            dlg.Title = "CSV�t�@�C���̍쐬��"
            dlg.Filter = "CSV�t�@�C�� (.csv)|*.csv"
            dlg.FilterIndex = 1

            Select Case (dlg.ShowDialog())
                Case DialogResult.OK
                    '�_�C�A���OOK�{�^��������
                    Console.WriteLine(dlg.FileName)
                Case Else
                    '�_�C�A���OOK�{�^���ȊO������
                    ChDir(strDIR)
                    Return
            End Select

            'CSV�o��
            Lng_Csv_Ok_Cnt = PFUNC_EXP_SINNYU(dlg.FileName)

            '�������b�Z�[�W
            If Lng_Csv_Ok_Cnt < 0 Then
            ElseIf Lng_Csv_Ok_Cnt = 0 Then
                MessageBox.Show(G_MSG0042W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                MessageBox.Show(String.Format(G_MSG0016I, Lng_Csv_Ok_Cnt), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            ChDir(strDIR)
            btnEnd.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo)�I��", "����", "")
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Me.Close()
    End Sub
#End Region

#Region " Private Function "
    Private Function PFUNC_IMP_SINNYU(ByVal pCsvPath As String, ByRef lngOkCnt As Long, ByRef lngErrCnt As Long) As Boolean

        Dim red As System.IO.StreamReader = Nothing

        Dim strHeadder As String
        Dim strReadLine As String

        Dim strErrorLine As String = ""
        Dim sql As StringBuilder


        lngOkCnt = 0
        lngErrCnt = 0
        PFUNC_IMP_SINNYU = False

        '2007/02/12
        Try
            red = New System.IO.StreamReader(pCsvPath, System.Text.Encoding.Default)

            'SQL���쐬
            '��ʂɐݒ肳��Ă���w�Z�R�[�h�������ɐV�����}�X�^���폜����
            sql = New StringBuilder(128)
            sql.Append(" DELETE  FROM SEITOMAST2")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'")

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                '�f�[�^�������G���[
                MainLOG.Write("�V�����}�X�^�폜", "���s", sql.ToString)
                Return False
            End If

            strHeadder = red.ReadLine.ToString()

            Do While Not red.Peek() = -1

                strReadLine = red.ReadLine.ToString

                '���͓��e�̃`�F�b�N
                If PFUNC_CHECK_STR(Lng_Csv_SeqNo, strReadLine, strErrorLine) = True Then
                    'SQL���쐬��SQL�������s
                    If PFUNC_INSERT_STR(Lng_Csv_SeqNo, strReadLine, strErrorLine) = True Then
                        lngOkCnt += 1
                    Else
                        lngErrCnt += 1

                        '�װ۸ނ̏����o��
                        If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                            Return False
                        End If
                    End If
                Else
                    lngErrCnt += 1

                    '�װ۸ނ̏����o��
                    If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                        Return False
                    End If
                End If

                Lng_Csv_SeqNo += 1
            Loop

            red.Close()
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_IMP_SINNYU)��O�G���[", "���s", ex.Message)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
        End Try

    End Function
    Private Function PFUNC_CHECK_STR(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean

        Dim intCnt As Integer
        Dim intCnt2 As Integer

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        '�ǉ� 2006/04/16
        Dim strRET As String = ""

        PFUNC_CHECK_STR = False

        strSplitValue = Split(pLineValue, ",")

        '���͓��e�`�F�b�N
        For intCnt = 0 To UBound(strSplitValue)
            Select Case (intCnt)
                Case 0
                    '�w�Z�R�[�h

                    '�����̓`�F�b�N
                    If Trim(strSplitValue(intCnt)) = "" Then

                        pError = pSeqNo & ",�w�Z,�����͂ł�"

                        Exit Function
                    End If

                    '��ʏ�œ��͂���Ă���R�[�h�ȊO�̃R�[�h�̏ꍇ�̓G���[
                    If Trim(txtGAKKOU_CODE.Text) <> Trim(strSplitValue(intCnt)).PadLeft(10, "0"c) Then

                        pError = pSeqNo & ",�w�Z,��ʂœ��͂���Ă���w�Z�R�[�h�ȊO�̂��͓̂o�^�ł��܂���"

                        Exit Function
                    End If

                    '�w�Z�}�X�^�`�F�b�N
                    sql = New StringBuilder(128)
                    oraReader = New MyOracleReader(MainDB)
                    sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
                    sql.Append(" WHERE GAKKOU_CODE_G = '" & Trim(strSplitValue(intCnt)).PadLeft(10, "0"c) & "'")

                    If oraReader.DataReader(sql) = False Then

                        pError = pSeqNo & ",�w�Z,�w�Z�}�X�^�ɓo�^����Ă���w�Z�R�[�h�ȊO�̂��͓̂o�^�ł��܂���"
                        oraReader.Close()
                        Return False
                    End If
                    oraReader.Close()
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

                    '���͌����`�F�b�N 2007/02/12
                    If strSplitValue(intCnt).Trim.Length > 4 Then
                        pError = pSeqNo & ",���w�N�x,���w�N�x���S���𒴂��Đݒ肳��Ă��܂�"

                        Exit Function

                    End If

                    If strSplitValue(intCnt).Trim.Length < 4 Then
                        pError = pSeqNo & ",���w�N�x,���w�N�x���S�������Őݒ肳��Ă��܂�"

                        Exit Function

                    End If
                    '���͒l�͈̓`�F�b�N
                    Select Case (CInt(strSplitValue(intCnt)))
                        Case Is >= CInt(txtNENDO.Text)
                        Case Else
                            pError = pSeqNo & ",���w�N�x,�i�������N�x�ȉ��̐��l��ݒ肷�邱�Ƃ͂ł��܂���"
                            Exit Function
                    End Select

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
                        Select Case (CLng(strSplitValue(intCnt)))
                            Case 1 To 9999
                            Case Else
                                pError = pSeqNo & ",�ʔ�,�P�`�X�X�X�X�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"

                                Exit Function
                        End Select
                    End If

                Case 3
                    '�w�N�R�[�h

                    '�����̓`�F�b�N
                    If Trim(strSplitValue(intCnt)) = "" Then

                        pError = pSeqNo & ",�w�N,�����͂ł�"

                        Exit Function
                    End If

                    '���l�`�F�b�N
                    If IsNumeric(strSplitValue(intCnt)) = False Then

                        pError = pSeqNo & ",�w�N,�w�N�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"

                        Exit Function
                    Else
                        '���͒l�͈̓`�F�b�N
                        Select Case (CLng(strSplitValue(intCnt)))
                            Case 0
                            Case Else
                                pError = pSeqNo & ",�w�N,�V�����̊w�N�͂O�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"

                                Exit Function
                        End Select
                    End If
                Case 4
                    '�N���X�R�[�h
                    '�����̓`�F�b�N
                    If Trim(strSplitValue(intCnt)) = "" Then
                        pError = pSeqNo & ",�N���X,�����͂ł�"

                        Exit Function
                    Else

                        If IsNumeric(strSplitValue(intCnt)) = False Then

                            pError = pSeqNo & ",�N���X,���l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"

                            Exit Function
                        Else
                            '���͒l�͈̓`�F�b�N
                            Select Case (CLng(strSplitValue(intCnt)))
                                Case 1 To 20
                                Case Else
                                    pError = pSeqNo & ",�N���X,�P�`�Q�O�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"

                                    Exit Function
                            End Select
                        End If

                        '�g�p�N���X�`�F�b�N
                        '1�w�N�̃N���X���Q��
                        sql = New StringBuilder(128)
                        oraReader = New MyOracleReader(MainDB)
                        sql.Append("SELECT * FROM GAKMAST1")
                        sql.Append(" WHERE")
                        sql.Append(" GAKKOU_CODE_G = '" & Trim(strSplitValue(0)).PadLeft(10, "0"c) & "'")
                        sql.Append(" AND")
                        sql.Append(" GAKUNEN_CODE_G = 1")
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

                        pError = pSeqNo & ",���k�ԍ�,���k�ԍ��ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"

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
                Case 7 '���k�������@�����`�F�b�N2006/05/10
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
                    '�戵���Z�@�փR�[�h

                    '�U�֕��@���W������(=1)�̏ꍇ�͖����͂ł���
                    Select Case (Trim(strSplitValue(15)))
                        Case "1"

                            If Trim(strSplitValue(intCnt)) <> "" Then
                                '���l�`�F�b�N  2007/02/12
                                If IsNumeric(strSplitValue(intCnt)) = False Then

                                    pError = pSeqNo & ",�戵���Z�@��,���Z�@�փR�[�h�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"

                                    Exit Function
                                End If

                                '�����`�F�b�N 2007/02/12
                                If strSplitValue(intCnt).Trim.Length > 4 Then

                                    pError = pSeqNo & ",�戵���Z�@��,���Z�@�փR�[�h���S���𒴂��Ă��܂�"

                                    Exit Function
                                End If
                            End If

                        Case Else
                            '���l�`�F�b�N  2007/02/12
                            If Trim(strSplitValue(intCnt)) = "" Or IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",�戵���Z�@��,���Z�@�փR�[�h�ɋ󔒂܂��͐��l�ł͂Ȃ��l���ݒ肳��Ă��܂�"

                                Exit Function
                            End If

                            '�����`�F�b�N 2007/02/12
                            If strSplitValue(intCnt).Trim.Length > 4 Then

                                pError = pSeqNo & ",�戵���Z�@��,���Z�@�փR�[�h���S���𒴂��Ă��܂�"

                                Exit Function
                            End If
                    End Select
                Case 10
                    '�戵�x�X�R�[�h
                    '�U�֕��@���W������(=1)�̏ꍇ�͖����͂ł���
                    Select Case (Trim(strSplitValue(15)))
                        Case "1"
                            If Trim(strSplitValue(intCnt)) <> "" Then
                                '���l�`�F�b�N  2007/02/12
                                If IsNumeric(strSplitValue(intCnt)) = False Then

                                    pError = pSeqNo & ",�戵�x�X,�x�X�R�[�h�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"

                                    Exit Function
                                End If

                                '�����`�F�b�N 2007/02/12
                                If strSplitValue(intCnt).Trim.Length > 3 Then

                                    pError = pSeqNo & ",�戵�x�X,�x�X�R�[�h���R���𒴂��Ă��܂�"

                                    Exit Function
                                Else
                                    If Trim(strSplitValue(intCnt - 1)) <> "" Then '���Z�@�ւ��󔒂łȂ��Ȃ瑶�݃`�F�b�N
                                        '���Z�@�փ}�X�^���݃`�F�b�N�@2007/02/12
                                        sql = New StringBuilder(128)
                                        oraReader = New MyOracleReader(MainDB)
                                        sql.Append("SELECT * FROM TENMAST ")
                                        sql.Append(" WHERE")
                                        sql.Append(" KIN_NO_N = '" & Trim(strSplitValue(intCnt - 1)).PadLeft(4, "0"c) & "'")
                                        sql.Append(" AND")
                                        sql.Append(" SIT_NO_N = '" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                                        If oraReader.DataReader(sql) = False Then
                                            pError = pSeqNo & ",�戵�x�X,���Z�@�փ}�X�^�ɓo�^����Ă��܂���"
                                            oraReader.Close()
                                            Return False
                                        End If
                                        oraReader.Close()
                                    End If
                                End If

                            End If
                        Case Else
                            '���l�`�F�b�N  2007/02/12
                            If Trim(strSplitValue(intCnt)) = "" Or IsNumeric(strSplitValue(intCnt)) = False Then
                                pError = pSeqNo & ",�戵�x�X,�x�X�R�[�h�ɋ󔒂܂��͐��l�ȊO�̒l���ݒ肳��Ă��܂�"
                                Exit Function
                            End If

                            '�����`�F�b�N 2007/02/12
                            If strSplitValue(intCnt).Trim.Length > 3 Then
                                pError = pSeqNo & ",�戵�x�X,�x�X�R�[�h���R���𒴂��Ă��܂�"
                                Exit Function
                            End If

                            '���Z�@�փ}�X�^���݃`�F�b�N�@2007/02/12
                            sql = New StringBuilder(128)
                            oraReader = New MyOracleReader(MainDB)
                            sql.Append("SELECT * FROM TENMAST ")
                            sql.Append(" WHERE")
                            sql.Append(" KIN_NO_N = '" & Trim(strSplitValue(intCnt - 1)).PadLeft(4, "0"c) & "'")
                            sql.Append(" AND")
                            sql.Append(" SIT_NO_N = '" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                            If oraReader.DataReader(sql) = False Then

                                pError = pSeqNo & ",�戵�x�X,���Z�@�փ}�X�^�ɓo�^����Ă��܂���"
                                oraReader.Close()
                                Return False
                            End If
                            oraReader.Close()
                    End Select
                Case 12
                    '�����ԍ�
                    '�U�֕��@���W������(=1)�̏ꍇ�͖����͂ł���
                    Select Case (Trim(strSplitValue(15)))
                        Case "1"
                            If Trim(strSplitValue(intCnt)) <> "" Then

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
                            End If
                        Case Else
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

                            '''''�`�F�b�N�f�W�b�g�����ǉ� 2007/02/12
                            If STR_CHK_DJT = "1" Then '<---ini�t�@�C���̃`�F�b�N�f�W�b�g����敪 0:���Ȃ� 1:����
                                If Format(CLng(strSplitValue(9)), "0000") = STR_JIKINKO_CODE Then '�����Ƀf�[�^�̂݃`�F�b�N�f�W�b�g���s
                                    If GFUNC_CHK_DEJIT(Format(CLng(strSplitValue(9)), "0000"), Format(CLng(strSplitValue(10)), "000"), CInt(strSplitValue(11)), Format(CLng(strSplitValue(12)), "0000000")) = False Then
                                        pError = pSeqNo & ",�戵�����ԍ�,�`�F�b�N�f�W�b�g�G���[�ł�"
                                        Exit Function
                                    End If
                                End If
                            End If
                    End Select
                Case 13
                    '���`�l(��)
                    '�U�֕��@���W������(=1)�̏ꍇ�͖����͂ł���
                    Select Case (Trim(strSplitValue(15)))
                        Case "1"
                            If strSplitValue(intCnt) <> "" Then
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
                        Case Else
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",�戵���`�l(��),�����͂ł��B"

                                Exit Function
                            Else
                                '�K��O�����`�F�b�N 2006/04/16
                                If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = False Then
                                    pError = pSeqNo & ",�戵���`�l(��),�K��O�������L��܂��B"
                                    Exit Function
                                End If
                                '�������`�F�b�N 2006/05/10
                                If Trim(strSplitValue(intCnt)).Length > 40 Then
                                    pError = pSeqNo & ",�戵���`�l(��),���p40�����ȓ��Őݒ肵�Ă�������"
                                    Exit Function
                                End If
                            End If
                    End Select
                Case 14    '���`�l�����@�����`�F�b�N2006/05/10

                    If Trim(strSplitValue(intCnt)) <> "" Then
                        '�Q�o�C�g�ϊ�
                        strSplitValue(intCnt) = StrConv(Trim(strSplitValue(intCnt)), VbStrConv.Wide)
                        '�������`�F�b�N
                        If Trim(strSplitValue(intCnt)).Length > 25 Then
                            pError = pSeqNo & ",���`�l(����),�S�p25�����ȓ��Őݒ肵�Ă�������"
                            Exit Function
                        End If
                    End If
                Case 16 '�Z���@�����`�F�b�N 2006/05/10
                    If Trim(strSplitValue(intCnt)) <> "" Then
                        '�Q�o�C�g�ϊ�
                        strSplitValue(intCnt) = StrConv(Trim(strSplitValue(intCnt)), VbStrConv.Wide)
                        '�������`�F�b�N
                        If Trim(strSplitValue(intCnt)).Length > 25 Then
                            pError = pSeqNo & ",�Z��,�S�p25�����ȓ��Őݒ肵�Ă�������"
                            Exit Function
                        End If
                    End If
                Case 17 '�d�b�ԍ��@�����`�F�b�N 2006/05/10
                    If strSplitValue(intCnt) <> "" Then
                        '�K��O�����`�F�b�N 2006/04/16
                        If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = False Then
                            pError = pSeqNo & ",�d�b�ԍ�,�K��O�������L��܂��B"
                            Exit Function
                        End If
                        '1�o�C�g�ϊ�
                        strSplitValue(intCnt) = StrConv(Trim(strSplitValue(intCnt)), VbStrConv.Narrow)
                        '�������`�F�b�N
                        '2012/04/13 saitou �W���C�� MODIFY ---------------------------------------->>>>
                        '�d�b�ԍ�12����13���ɕύX
                        If Trim(strSplitValue(intCnt)).Length > 13 Then
                            pError = pSeqNo & ",�d�b�ԍ�,���p13�����ȓ��Őݒ肵�Ă�������"
                            Exit Function
                        End If
                        'If Trim(strSplitValue(intCnt)).Length > 12 Then
                        '    pError = pSeqNo & ",�d�b�ԍ�,���p12�����ȓ��Őݒ肵�Ă�������"
                        '    Exit Function
                        'End If
                        '2012/04/13 saitou �W���C�� MODIFY ----------------------------------------<<<<
                    End If
                Case 20
                    '���ID
                    If Trim(strSplitValue(intCnt)) = "" Then
                        pError = pSeqNo & ",���ID,�����͂ł�"
                        Exit Function
                    Else
                        '���l�`�F�b�N  2007/02/15
                        If IsNumeric(strSplitValue(intCnt)) = False Then

                            pError = pSeqNo & ",���ID,���ID�ɐ��l�ȊO�̒l���ݒ肳��Ă��܂�"

                            Exit Function
                        End If

                        '�����`�F�b�N�ǉ� 2007/02/12
                        If strSplitValue(intCnt).Trim.Length >= 4 Then
                            pError = pSeqNo & ",���ID,���ID���S���𒴂��Đݒ肳��Ă��܂�"
                            Exit Function
                        End If
                        '��ڃ}�X�^���݃`�F�b�N
                        sql = New StringBuilder(128)
                        oraReader = New MyOracleReader(MainDB)
                        sql.Append(" SELECT * FROM HIMOMAST")
                        sql.Append(" WHERE")
                        sql.Append(" GAKKOU_CODE_H ='" & Trim(txtGAKKOU_CODE.Text) & "'")
                        sql.Append(" AND ")
                        sql.Append(" GAKUNEN_CODE_H = 1")
                        sql.Append(" AND")
                        sql.Append(" HIMOKU_ID_H ='" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                        If oraReader.DataReader(sql) = False Then

                            pError = pSeqNo & ",���ID,��ڃ}�X�^�ɓo�^����Ă��Ȃ����ID��ݒ肷�邱�Ƃ͂ł��܂���"
                            oraReader.Close()
                            Return False
                        End If
                        oraReader.Close()
                    End If
                    '�V�����͒��q���̓`�F�b�N���Ȃ� 2007/02/12
                    'Case 21  '���q���ǉ� 2005/03/09
                    '        '���q�t���O
                    '        '�����̓`�F�b�N
                    '        If Trim(strSplitValue(intCnt)) = "" Then

                    '            pError = pSeqNo & ",���q�t���O,�����͂ł��B"

                    '            Exit Function
                    '        End If

                    'Case 22
                    '        '���q���w�N�x
                    '        If Trim(strSplitValue(21)) = "1" Then
                    '            If Trim(strSplitValue(intCnt)) = "" Then
                    '                pError = pSeqNo & ",���q���w�N�x,�����͂ł��B"

                    '                Exit Function
                    '            End If
                    '        End If
                    'Case 23
                    '        '���q�ʔ�
                    '        If Trim(strSplitValue(21)) = "1" Then
                    '            If Trim(strSplitValue(intCnt)) = "0" OR Trim(strSplitValue(intCnt)) = "" Then
                    '                pError = pSeqNo & ",���q�ʔ�,�����͂ł��B"

                    '                Exit Function
                    '            End If
                    '        End If
                    'Case 24
                    '        '���q�w�N
                    '        If Trim(strSplitValue(21)) = "1" Then
                    '            If Trim(strSplitValue(intCnt)) = "0" OR Trim(strSplitValue(intCnt)) = "" Then
                    '                pError = pSeqNo & ",���q�w�N,�����͂ł��B"

                    '                Exit Function
                    '            End If
                    '        End If
                    'Case 25
                    '        '���q�N���X
                    '        If Trim(strSplitValue(21)) = "1" Then
                    '            If Trim(strSplitValue(intCnt)) = "0" OR Trim(strSplitValue(intCnt)) = "" Then
                    '                pError = pSeqNo & ",���q�N���X,�����͂ł��B"

                    '                Exit Function
                    '            End If
                    '        End If
                    'Case 26
                    '        '���q���k�ԍ�
                    '        If Trim(strSplitValue(21)) = "1" Then
                    '            If Trim(strSplitValue(intCnt)) = "0" OR Trim(strSplitValue(intCnt)) = "" Then
                    '                pError = pSeqNo & ",���q���k�ԍ�,�����͂ł��B"

                    '                Exit Function
                    '            End If
                    '        End If

            End Select
        Next intCnt

        Return True

    End Function
    Private Function PFUNC_INSERT_STR(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean

        Dim intTuki As Integer
        Dim intCol As Integer

        Dim intCnt As Integer

        Dim strSql As String

        Dim sql As StringBuilder

        Dim strRET As String = ""

        PFUNC_INSERT_STR = False

        For intTuki = 1 To 12
            'SQL���쐬
            sql = New StringBuilder(128)
            sql.Append(" INSERT INTO SEITOMAST2 values(")
            'For intCnt = 0 To UBound(strSplitValue)
            For intCnt = 0 To 26 '2005/06/15�C��
                Select Case (intCnt)
                    Case 0
                        '�w�Z�R�[�h
                        sql.Append(IIf(intCnt <> 0, ",", "") & "'" & Trim(strSplitValue(intCnt)).PadLeft(10, "0"c) & "'")

                    Case 5, 12
                        '���k�ԍ�,�����ԍ�
                        sql.Append(IIf(intCnt <> 0, ",", "") & "'" & Trim(strSplitValue(intCnt)).PadLeft(7, "0"c) & "'")

                    Case 1, 7, 14, 16, 17
                        '���w�N�x,���k����(����),���`�l(����),
                        '�_��Z��(����),�_��d�b�ԍ�
                        sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                    Case 6, 13 '���k����(��),���`�l(�J�i) 2007/02/12
                        If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = True Then
                            sql.Append(",'" & strRET & "'")
                        Else
                            sql.Append(",'" & Space(40) & "'")
                        End If

                    Case 2, 3, 4
                        '�ʔ�,�w�N����,�׽����
                        sql.Append("," & Trim(strSplitValue(intCnt)))
                    Case 8
                        '����
                        '�l�Ȃ��̏ꍇ�͂Q�Œ�
                        Select Case (Trim(strSplitValue(intCnt)))
                            Case "0", "1", "2"
                                sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                            Case Else
                                sql.Append(",'2'")
                        End Select
                    Case 9
                        '�戵���Z�@�փR�[�h
                        If Trim(strSplitValue(intCnt)) <> "" Then
                            sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(4, "0"c) & "'")
                        Else
                            sql.Append(",' '")
                        End If
                    Case 10
                        '�戵�x�X�R�[�h
                        If Trim(strSplitValue(intCnt)) <> "" Then
                            sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")
                        Else
                            sql.Append(",' '")
                        End If
                    Case 11
                        '�Ȗ�
                        Select Case (Trim(strSplitValue(intCnt)).PadLeft(2, "0"c))
                            Case "01", "02", "05"
                                sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(2, "0"c) & "'")
                            Case Else
                                sql.Append(",'01'")
                        End Select
                    Case 15
                        '�U�֕��@
                        '�l�Ȃ��̏ꍇ�͂O�Œ�
                        Select Case (Trim(strSplitValue(intCnt)))
                            Case "0", "1", "2"
                                sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                            Case Else
                                sql.Append(",'0'")
                        End Select
                    Case 18
                        '���敪
                        '�l�Ȃ��̏ꍇ�͂O�Œ�
                        Select Case (Trim(strSplitValue(intCnt)))
                            Case "0", "9"
                                sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                            Case Else
                                sql.Append(",'0'")
                        End Select
                    Case 19
                        '�i���敪
                        '�l�Ȃ��̏ꍇ�͂O�Œ�
                        Select Case (Trim(strSplitValue(intCnt)))
                            Case "0", "1"
                                sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                            Case Else
                                sql.Append(",'0'")
                        End Select
                    Case 20
                        '���ID
                        sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                End Select
            Next intCnt

            '���q���
            For intCol = 0 To 5
                Select Case (intCol)
                    Case 0, 2, 3, 4
                        '���q�L���׸�,���q�ʔ�,���q�w�N,���q�׽
                        sql.Append(",0")
                    Case 1, 5
                        '���q�N�x,���q���k�ԍ�
                        sql.Append(",' '")
                End Select
            Next intCol

            '������
            sql.Append(",'" & Format(intTuki, "00") & "'")

            '��ڂP�`�P�T
            For intCol = 1 To 15
                '�������@
                sql.Append(",'0'")
                '�������z
                sql.Append(",0")
            Next intCol

            sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
            sql.Append(",'00000000'")
            sql.Append(",'" & Space(50) & "'") 'YOBI1_O
            sql.Append(",'" & Space(50) & "'") 'YOBI2_O
            sql.Append(",'" & Space(50) & "'") 'YOBI3_O
            sql.Append(",'" & Space(50) & "'") 'YOBI4_O
            sql.Append(",'" & Space(50) & "'") 'YOBI5_O
            sql.Append(")")

            '�d���`�F�b�N
            strSql = " SELECT * FROM SEITOMAST2"
            strSql += " WHERE GAKKOU_CODE_O ='" & Trim(strSplitValue(0)).PadLeft(10, "0"c) & "'"
            strSql += " AND NENDO_O ='" & Trim(strSplitValue(1)) & "'"
            strSql += " AND TUUBAN_O =" & Trim(strSplitValue(2).PadLeft(4, "0"c))
            strSql += " AND TUKI_NO_O ='" & Format(intTuki, "00") & "'"

            Dim oraReader As New MyOracleReader(MainDB)

            If oraReader.DataReader(strSql) = True Then
                pError = pSeqNo & ",��L�[,�f�[�^���d�����Ă��܂�"
                oraReader.Close()
                Return False
            End If
            oraReader.Close()

            '�g�����U�N�V�����f�[�^�������s
            If MainDB.ExecuteNonQuery(sql) < 0 Then

                pError = pSeqNo & ",�ړ�,�ړ��������ɃG���[���������܂���"
                Return False

            End If

        Next intTuki

        Return True

    End Function
    Private Function PFUNC_EXP_SINNYU(ByVal pCsvPath As String) As Long

        Dim lngLine As Long = 0
        '2012/10/06 saitou ��_�M�� �J�X�^�}�C�Y UPD -------------------------------------------------->>>>
        'PTA�������ǉ��Ή�
        'String��StringBuilder�ɕύX���A�������x�����}��
        Dim sbLine As New StringBuilder
        'Dim strLine As String
        '2012/10/06 saitou ��_�M�� �J�X�^�}�C�Y UPD --------------------------------------------------<<<<

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)
        Dim wrt As System.IO.StreamWriter = Nothing

        Try
            sql.Append(" SELECT distinct")
            sql.Append(" GAKKOU_CODE_O , NENDO_O , TUUBAN_O , GAKUNEN_CODE_O , CLASS_CODE_O")
            sql.Append(",SEITO_NO_O , SEITO_KNAME_O , SEITO_NNAME_O , SEIBETU_O , TKIN_NO_O")
            sql.Append(",TSIT_NO_O , KAMOKU_O , KOUZA_O , MEIGI_KNAME_O , MEIGI_NNAME_O")
            sql.Append(",FURIKAE_O , KEIYAKU_NJYU_O , KEIYAKU_DENWA_O , KAIYAKU_FLG_O , SINKYU_KBN_O")
            sql.Append(",HIMOKU_ID_O")
            sql.Append(" FROM SEITOMAST2")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'")
            sql.Append(" ORDER BY ")
            Select Case (STR���[�\�[�g��)
                Case "0"
                    sql.Append("      GAKKOU_CODE_O    ASC")
                    sql.Append("     ,GAKUNEN_CODE_O   ASC")
                    sql.Append("     ,CLASS_CODE_O     ASC")
                    sql.Append("     ,SEITO_NO_O       ASC")
                    sql.Append("     ,NENDO_O          ASC")
                    sql.Append("     ,TUUBAN_O         ASC")
                Case "1"
                    sql.Append("      GAKKOU_CODE_O    ASC")
                    sql.Append("     ,GAKUNEN_CODE_O   ASC")
                    sql.Append("     ,NENDO_O          ASC")
                    sql.Append("     ,TUUBAN_O         ASC")
                Case Else
                    sql.Append("      GAKKOU_CODE_O    ASC")
                    sql.Append("     ,GAKUNEN_CODE_O   ASC")
                    sql.Append("     ,SEITO_KNAME_O    ASC")
                    sql.Append("     ,NENDO_O          ASC")
                    sql.Append("     ,TUUBAN_O         ASC")
            End Select

            If oraReader.DataReader(sql) = True Then

                lngLine = 0

                '���ږ����o���ݒ�
                '2012/10/06 saitou ��_�M�� �J�X�^�}�C�Y UPD -------------------------------------------------->>>>
                'PTA�������ǉ��Ή�
                'String��StringBuilder�ɕύX���A�������x�����}��
                sbLine.Length = 0
                With sbLine
                    .Append("�w�Z�R�[�h,���w�N�x,�ʔ�,�w�N�R�[�h,�N���X")
                    .Append(",���k�ԍ�,���k����(�J�i),���k����(����),����")
                    .Append(",���Z�@�փR�[�h,�x�X�R�[�h,�Ȗ�,�����ԍ�,���`�l�i�J�i),���`�l�i����),�U�֕��@")
                    .Append(",�_��Z��(����),�_��d�b�ԍ�")
                    .AppendLine(",���敪,�i���敪,���ID")
                End With
                'strLine = "�w�Z�R�[�h,���w�N�x,�ʔ�,�w�N�R�[�h,�N���X"
                'strLine += ",���k�ԍ�,���k����(�J�i),���k����(����),����"
                'strLine += ",���Z�@�փR�[�h,�x�X�R�[�h,�Ȗ�,�����ԍ�,���`�l�i�J�i),���`�l�i����),�U�֕��@"
                'strLine += ",�_��Z��(����),�_��d�b�ԍ�"
                'strLine += ",���敪,�i���敪,���ID"
                'strLine += vbCrLf
                '2012/10/06 saitou ��_�M�� �J�X�^�}�C�Y UPD --------------------------------------------------<<<<

                Do Until oraReader.EOF

                    '2012/10/06 saitou ��_�M�� �J�X�^�}�C�Y UPD -------------------------------------------------->>>>
                    'PTA�������ǉ��Ή�
                    'String��StringBuilder�ɕύX���A�������x�����}��
                    'GetString�Ŏ擾���Ă��鎞�_��DBNull�͗L�肦�Ȃ����ATrim���K�v�Ȃ�

                    '�w�Z�R�[�h
                    '2016/11/04 �^�X�N�j���� CHG �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- START
                    sbLine.Append(Microsoft.VisualBasic.Right(oraReader.GetString("GAKKOU_CODE_O"), GAKKOU_CODE_LENGTH))
                    'sbLine.Append(oraReader.GetString("GAKKOU_CODE_O"))
                    '2016/11/04 �^�X�N�j���� CHG �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- END
                    '���w�N�x
                    sbLine.Append("," & oraReader.GetString("NENDO_O"))
                    '�ʔ�
                    sbLine.Append("," & oraReader.GetString("TUUBAN_O"))
                    '�w�N�R�[�h
                    sbLine.Append("," & oraReader.GetString("GAKUNEN_CODE_O"))
                    '�N���X�R�[�h
                    sbLine.Append("," & oraReader.GetString("CLASS_CODE_O"))
                    '���k�ԍ�
                    sbLine.Append("," & oraReader.GetString("SEITO_NO_O"))
                    '���k����(�J�i)
                    sbLine.Append("," & oraReader.GetString("SEITO_KNAME_O"))
                    '���k����(����)
                    sbLine.Append("," & oraReader.GetString("SEITO_NNAME_O"))
                    '����
                    If oraReader.GetString("SEIBETU_O") <> String.Empty Then
                        sbLine.Append("," & oraReader.GetString("SEIBETU_O"))
                    Else
                        sbLine.Append(",2")
                    End If
                    '���Z�@�փR�[�h
                    sbLine.Append("," & oraReader.GetString("TKIN_NO_O"))
                    '�x�X�R�[�h
                    sbLine.Append("," & oraReader.GetString("TSIT_NO_O"))
                    '�Ȗ�
                    sbLine.Append("," & oraReader.GetString("KAMOKU_O"))
                    '�����ԍ�
                    sbLine.Append("," & oraReader.GetString("KOUZA_O"))
                    '���`�l(�J�i)
                    sbLine.Append("," & oraReader.GetString("MEIGI_KNAME_O"))
                    '���`�l(����)
                    sbLine.Append("," & oraReader.GetString("MEIGI_NNAME_O"))
                    '�U�֕��@
                    sbLine.Append("," & oraReader.GetString("FURIKAE_O"))
                    '�_��Z��(����)
                    sbLine.Append("," & oraReader.GetString("KEIYAKU_NJYU_O"))
                    '�_��d�b�ԍ�
                    sbLine.Append("," & oraReader.GetString("KEIYAKU_DENWA_O"))
                    '���敪
                    sbLine.Append("," & oraReader.GetString("KAIYAKU_FLG_O"))
                    '�i���敪
                    sbLine.Append("," & oraReader.GetString("SINKYU_KBN_O"))
                    '���ID
                    sbLine.AppendLine("," & oraReader.GetString("HIMOKU_ID_O"))

                    ''�w�Z�R�[�h
                    'If IsDBNull(oraReader.GetString("GAKKOU_CODE_O")) = False Then
                    '    strLine += Trim(CStr(oraReader.GetString("GAKKOU_CODE_O")))
                    'End If

                    ''���w�N�x
                    'If IsDBNull(oraReader.GetString("NENDO_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("NENDO_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''�ʔ�
                    'If IsDBNull(oraReader.GetString("TUUBAN_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("TUUBAN_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''�w�N�R�[�h
                    'If IsDBNull(oraReader.GetString("GAKUNEN_CODE_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("GAKUNEN_CODE_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''�N���X�R�[�h
                    'If IsDBNull(oraReader.GetString("CLASS_CODE_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("CLASS_CODE_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''���k�ԍ�
                    'If IsDBNull(oraReader.GetString("SEITO_NO_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("SEITO_NO_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''���k����(�J�i)
                    'If IsDBNull(oraReader.GetString("SEITO_KNAME_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("SEITO_KNAME_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''���k����(����)
                    'If IsDBNull(oraReader.GetString("SEITO_NNAME_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("SEITO_NNAME_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''����
                    'If IsDBNull(oraReader.GetString("SEIBETU_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("SEIBETU_O")))
                    'Else
                    '    strLine += ",2"
                    'End If

                    ''���Z�@�փR�[�h
                    'If IsDBNull(oraReader.GetString("TKIN_NO_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("TKIN_NO_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''�x�X�R�[�h
                    'If IsDBNull(oraReader.GetString("TSIT_NO_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("TSIT_NO_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''�Ȗ�
                    'If IsDBNull(oraReader.GetString("KAMOKU_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("KAMOKU_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''�����ԍ�
                    'If IsDBNull(oraReader.GetString("KOUZA_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("KOUZA_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''���`�l(�J�i)
                    'If IsDBNull(oraReader.GetString("MEIGI_KNAME_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("MEIGI_KNAME_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''���`�l(����)
                    'If IsDBNull(oraReader.GetString("MEIGI_NNAME_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("MEIGI_NNAME_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''�U�֕��@
                    'If IsDBNull(oraReader.GetString("FURIKAE_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("FURIKAE_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''�_��Z��(����)
                    'If IsDBNull(oraReader.GetString("KEIYAKU_NJYU_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("KEIYAKU_NJYU_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''�_��d�b�ԍ�
                    'If IsDBNull(oraReader.GetString("KEIYAKU_DENWA_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("KEIYAKU_DENWA_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''���敪
                    'If IsDBNull(oraReader.GetString("KAIYAKU_FLG_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("KAIYAKU_FLG_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''�i���敪
                    'If IsDBNull(oraReader.GetString("SINKYU_KBN_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("SINKYU_KBN_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''���ID
                    'If IsDBNull(oraReader.GetString("HIMOKU_ID_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("HIMOKU_ID_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    'strLine += vbCrLf
                    '2012/10/06 saitou ��_�M�� �J�X�^�}�C�Y UPD --------------------------------------------------<<<<

                    lngLine += 1

                    oraReader.NextRead()

                Loop

            Else

                Return 0

            End If
            oraReader.Close()

            wrt = New System.IO.StreamWriter(pCsvPath, False, System.Text.Encoding.Default)

            '2012/10/06 saitou ��_�M�� �J�X�^�}�C�Y UPD -------------------------------------------------->>>>
            'PTA�������ǉ��Ή�
            'String��StringBuilder�ɕύX���A�������x�����}��
            wrt.Write(sbLine.ToString)
            'wrt.Write(strLine)
            '2012/10/06 saitou ��_�M�� �J�X�^�}�C�Y UPD --------------------------------------------------<<<<

            wrt.Close()

            Return lngLine

        Catch ex As Exception
            '2014/01/06 saitou �W���� ���b�Z�[�W�萔�� ADD -------------------------------------------------->>>>
            '��O�������̓��b�Z�[�W�o��
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '2014/01/06 saitou �W���� ���b�Z�[�W�萔�� ADD --------------------------------------------------<<<<
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�b�r�u�쐬", "���s", ex.Message)
            Return -1       '���s
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not wrt Is Nothing Then wrt.Close()
        End Try

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
    Private Function PFUNC_GAKMAST2_GET() As Boolean

        '�w�Z�}�X�^�Q�̎擾
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        sql.Append(" SELECT MEISAI_OUT_T FROM GAKMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_T ='" & txtGAKKOU_CODE.Text.Trim & "'")

        If oraReader.DataReader(sql) = True Then
            STR���[�\�[�g�� = oraReader.GetString("MEISAI_OUT_T").ToString
        Else
            oraReader.Close()
            Return False
        End If

        oraReader.Close()
        Return True

    End Function
#End Region

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '********************************************
        '�w�Z�J�i�i���݃R���{
        '********************************************
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
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        Dim oraDB As New MyOracle
        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader = Nothing

        'COMBOBOX�I�����w�Z��,�w�Z�R�[�h�ݒ�
        lblGAKKOU_NAME.Text = cmbGAKKOUNAME.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGAKKOUNAME.SelectedIndex)

        '���w�N�x�ݒ菈���@2007/02/12
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            Sql = New StringBuilder(128)
            oraReader = New MyOracleReader(oraDB)
            Sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
            Sql.Append(" WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If oraReader.DataReader(Sql) = True Then
                lblGAKKOU_NAME.Text = CStr(oraReader.GetString("GAKKOU_NNAME_G"))
            Else
                lblGAKKOU_NAME.Text = ""
            End If
            oraReader.Close()

            '�ŏI�i�������N����\������
            Sql = New StringBuilder(128)
            oraReader = New MyOracleReader(oraDB)
            Sql.Append("SELECT SINKYU_NENDO_T FROM GAKMAST2 ")
            Sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If oraReader.DataReader(Sql) = True Then
                txtNENDO.Text = CInt(oraReader.GetString("SINKYU_NENDO_T")) + 1
            Else
                txtNENDO.Text = ""
            End If
            oraReader.Close()
        End If


        '�w�N�e�L�X�g�{�b�N�X��FOCUS
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        Dim oraDB As New MyOracle
        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader = Nothing

        Try

            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                '�w�Z������
                sql = New StringBuilder(128)
                oraReader = New MyOracleReader(oraDB)
                sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
                sql.Append(" WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

                If oraReader.DataReader(sql) = False Then
                    MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    lblGAKKOU_NAME.Text = ""
                    oraReader.Close()
                    Return
                Else
                    lblGAKKOU_NAME.Text = CStr(oraReader.GetString("GAKKOU_NNAME_G"))
                End If
                oraReader.Close()


                '�ŏI�i�������N����\������
                sql = New StringBuilder(128)
                oraReader = New MyOracleReader(oraDB)
                sql.Append("SELECT SINKYU_NENDO_T FROM GAKMAST2 ")
                sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

                If oraReader.DataReader(sql) = True Then
                    txtNENDO.Text = CInt(oraReader.GetString("SINKYU_NENDO_T")) + 1
                Else
                    txtNENDO.Text = ""
                End If
                oraReader.Close()
            End If

        Catch ex As Exception

        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
