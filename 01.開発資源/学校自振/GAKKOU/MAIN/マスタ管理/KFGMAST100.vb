Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon

Public Class KFGMAST100

#Region " ���ʕϐ���` "
    Private Lng_Csv_Ok_Cnt As Long
    Private Lng_Csv_Ng_Cnt As Long
    Private Lng_Csv_SeqNo As Long

    Private Str_Csv_Himoku_Name(14) As String
#End Region

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST100", "��ڃ}�X�^�������")
    Private Const msgTitle As String = "��ڃ}�X�^�������(KFGMAST100)"
    Private MainDB As MyOracle

    Private KobetuLogFileName As String

    '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- START
    Private GAKKOU_CODE_LENGTH As Integer = 10
    '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- END
    '2017/02/22 �^�X�N�j���� ADD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
    Private intMaxHimokuCnt As Integer = CInt(IIf(STR_HIMOKU_PTN = "1", 15, 10))
    '2017/02/22 �^�X�N�j���� ADD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAST100_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            If MessageBox.Show(G_MSG0012I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If
            strDIR = CurDir()

            Lng_Csv_Ok_Cnt = 0
            Lng_Csv_Ng_Cnt = 0
            Lng_Csv_SeqNo = 1 '0��1�ɕύX 2007/02/12
            '�G���[���O�t�@�C���폜 2007/02/12
            KobetuLogFileName = Path.Combine(STR_LOG_PATH, "HIMOKU" & txtGAKKOU_CODE.Text & ".log")
            If File.Exists(KobetuLogFileName) Then
                File.Delete(KobetuLogFileName)
            End If

            '���͐�̎w��
            '�t�@�C���̑I���_�C�A���O��\��
            Dim dlg As New Windows.Forms.OpenFileDialog

            dlg.ReadOnlyChecked = False

            dlg.CheckFileExists = True

            dlg.InitialDirectory = STR_CSV_PATH
            dlg.FileName = "HIMOKU" & Trim(txtGAKKOU_CODE.Text)

            dlg.Title = "CSV�t�@�C���̑I��"
            dlg.Filter = "CSV�t�@�C�� (*.csv)|*.csv"
            dlg.FilterIndex = 1

            Select Case dlg.ShowDialog()
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
            If PFUNC_IMP_HIMOKU(dlg.FileName, Lng_Csv_Ok_Cnt, Lng_Csv_Ng_Cnt) = False Then
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
            dlg.FileName = "HIMOKU" & Trim(txtGAKKOU_CODE.Text)

            dlg.Title = "CSV�t�@�C���̍쐬��"
            dlg.Filter = "CSV�t�@�C�� (.csv)|*.csv"
            dlg.FilterIndex = 1

            Select Case dlg.ShowDialog()
                Case DialogResult.OK
                    '�_�C�A���OOK�{�^��������
                    Console.WriteLine(dlg.FileName)
                Case Else
                    '�_�C�A���OOK�{�^���ȊO������
                    ChDir(strDIR)
                    Exit Sub
            End Select

            'CSV�o��
            Lng_Csv_Ok_Cnt = PFUNC_EXP_HIMOKU(dlg.FileName)

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
    Private Function PFUNC_IMP_HIMOKU(ByVal pCsvPath As String, ByRef lngOkCnt As Long, ByRef lngErrCnt As Long) As Boolean

        Dim red As System.IO.StreamReader = Nothing

        Dim strHeadder As String
        Dim strReadLine As String

        Dim strErrorLine As String = ""

        Dim sql As StringBuilder

        lngOkCnt = 0
        lngErrCnt = 0
        PFUNC_IMP_HIMOKU = False

        Try
            red = New System.IO.StreamReader(pCsvPath, System.Text.Encoding.Default)


            'SQL���쐬
            '��ʂɐݒ肳��Ă���w�Z�R�[�h�������ɔ�ڃ}�X�^���폜����
            sql = New StringBuilder(128)
            sql.Append(" DELETE  FROM HIMOMAST")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_H ='" & Trim(txtGAKKOU_CODE.Text) & "'")

            '�g�����U�N�V�����f�[�^����J�n
            If MainDB.ExecuteNonQuery(sql) < 0 Then
                MainLOG.Write("��ڃ}�X�^�폜", "���s", sql.ToString)
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
                        'SQL�ŃG���[�����������ꍇ�͏������f�@2007/02/12
                        red.Close()
                        Return False
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_IMP_SEITO)��O�G���[", "���s", ex.Message)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
        End Try

    End Function
    Private Function PFUNC_CHECK_STR(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean

        Dim intCnt As Integer
        Dim strSplitValue() As String
        Dim strRET As String = ""

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader


        PFUNC_CHECK_STR = False

        strSplitValue = Split(pLineValue, ",")

        '���͓��e�`�F�b�N
        For intCnt = 0 To UBound(strSplitValue)
            Select Case intCnt
                Case 0
                    '�w�Z�R�[�h

                    '�����̓`�F�b�N
                    If Trim(strSplitValue(intCnt)) = "" Then

                        pError = pSeqNo & ",�w�Z�R�[�h,�����͂ł�"

                        Exit Function
                    End If

                    '��ʏ�œ��͂���Ă���R�[�h�ȊO�̃R�[�h�̏ꍇ�̓G���[
                    If Trim(txtGAKKOU_CODE.Text) <> strSplitValue(intCnt).PadLeft(10, "0"c) Then

                        pError = pSeqNo & ",�w�Z�R�[�h,��ʂœ��͂���Ă���w�Z�R�[�h�ȊO�̂��͓̂o�^�ł��܂���"

                        Exit Function
                    End If

                    '�w�Z�}�X�^�`�F�b�N
                    sql = New StringBuilder(128)
                    oraReader = New MyOracleReader(MainDB)
                    sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
                    sql.Append(" WHERE GAKKOU_CODE_G = '" & strSplitValue(intCnt).PadLeft(10, "0"c) & "'")

                    If oraReader.DataReader(sql) = False Then

                        pError = pSeqNo & ",�w�Z�R�[�h,�w�Z�}�X�^�ɓo�^����Ă���w�Z�R�[�h�ȊO�̂��͓̂o�^�ł��܂���"
                        oraReader.Close()
                        Return False
                    End If
                    oraReader.Close()
                Case 1
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
                    sql.Append(" GAKKOU_CODE_G = '" & strSplitValue(0).PadLeft(10, "0"c) & "'")
                    sql.Append(" AND")
                    sql.Append(" GAKUNEN_CODE_G = " & Trim(strSplitValue(intCnt)))

                    If oraReader.DataReader(sql) = False Then
                        'MSG�ύX 2007/02/12
                        pError = pSeqNo & ",�w�N�R�[�h,�w�N�R�[�h�����݂��܂���"
                        oraReader.Close()
                        Return False
                    End If
                    oraReader.Close()
                Case 2
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
                    '������

                    '���ID��000(���ό���)�̏ꍇ�����������͂���Ă��Ȃ����߃`�F�b�N�͍s��Ȃ�
                    Select Case strSplitValue(2).Trim.PadLeft(3, "0"c)
                        Case Is <> "000"
                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",������,�����͂ł�"

                                Exit Function
                            Else
                                Select Case CLng(strSplitValue(intCnt))
                                    Case 1 To 12
                                    Case Else
                                        pError = pSeqNo & ",������,�������͂P�`�P�Q�ȊO�̐��l��ݒ肷�邱�Ƃ͂ł��܂���"

                                        Exit Function
                                End Select
                            End If
                    End Select
                Case 5, 12, 19, 26, 33, 40, 47, 54, 61, 68, 75, 82, 89, 96, 103
                    '��ږ���

                Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69, 76, 83, 90, 97, 104
                    If Trim(strSplitValue(intCnt - 1)) <> "" Then
                        '���Z�@�փR�[�h
                        If Trim(strSplitValue(intCnt)) = "" Then

                            pError = pSeqNo & ",���ϋ��Z�@��,�����͂ł�"

                            Exit Function
                        End If

                        '�����`�F�b�N 2007/02/12
                        If strSplitValue(intCnt).Trim.Length > 4 Then

                            pError = pSeqNo & ",���ϋ��Z�@��,���ϋ��Z�@�փR�[�h���S���𒴂��Ă��܂�"

                            Exit Function
                        End If

                    End If


                Case 7, 14, 21, 28, 35, 42, 49, 56, 63, 70, 77, 84, 91, 98, 105

                    If Trim(strSplitValue(intCnt - 1)) <> "" Then
                        '�x�X�R�[�h
                        If Trim(strSplitValue(intCnt)) = "" Then

                            pError = pSeqNo & ",���ώx�X,�����͂ł�"

                            Exit Function
                        Else

                            '�����`�F�b�N 2007/02/12
                            If strSplitValue(intCnt).Trim.Length > 3 Then

                                pError = pSeqNo & ",���ώx�X,���ώx�X�R�[�h���R���𒴂��Ă��܂�"

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
                Case 8, 15, 22, 29, 36, 43, 50, 57, 64, 71, 78, 85, 92, 99, 106

                    '���ωȖ� �����͂̂Ƃ��G���[���b�Z�[�W�o�� 2007/02/12
                    If Trim(strSplitValue(intCnt - 1)) <> "" Then '�x�X�R�[�h���ݒ肳��Ă���ꍇ
                        If Trim(strSplitValue(intCnt)) = "" Then
                            pError = pSeqNo & ",���ωȖ�,�����͂ł�"
                            Exit Function
                        Else '2007/02/15
                            Select Case CInt(Trim(strSplitValue(intCnt)))
                                Case 1, 2
                                Case Else
                                    pError = pSeqNo & ",���ωȖ�,���ωȖڂ͕��ʂ܂��͓����̂ݎw��\�ł�"
                                    Exit Function
                            End Select

                        End If
                    End If

                Case 9, 16, 23, 30, 37, 44, 51, 58, 65, 72, 79, 86, 93, 100, 107

                    '�����ԍ�
                    If Trim(strSplitValue(intCnt - 4)) <> "" Then
                        If Trim(strSplitValue(intCnt)) = "" Then

                            pError = pSeqNo & ",���ό����ԍ�,�����͂ł�"

                            Exit Function
                        End If
                        '���ό����ԍ������`�F�b�N 2007/02/12
                        If strSplitValue(intCnt).Trim.Length > 7 Then

                            pError = pSeqNo & ",���ό����ԍ�,���ό����ԍ����V���𒴂��Ă��܂�"

                            Exit Function
                        End If

                    End If
                Case 10, 17, 24, 31, 38, 45, 52, 59, 66, 73, 80, 87, 94, 101, 108

                    '2007/09/19�@�ǉ�
                    '���`�l��(��)

                    '�����̓`�F�b�N
                    If Trim(strSplitValue(intCnt - 1)) <> "" Then
                        If Trim(strSplitValue(intCnt)) = "" Then
                            '===2008/04/17 �����͂ł�OK�Ƃ���===============
                            'pError = pSeqNo & ",���`�l��(��),�����͂ł�"
                            'Exit Function
                            '===============================================
                        Else
                            '�K��O�����`�F�b�N 2006/04/16
                            If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = False Then
                                pError = pSeqNo & ",���`�l��(��),�K��O�������L��܂�"
                                Exit Function
                            End If
                            '�������`�F�b�N 2006/05/10
                            If Trim(strSplitValue(intCnt)).Length > 40 Then
                                pError = pSeqNo & ",���`�l��(��),���p40�����ȓ��Őݒ肵�Ă�������"
                                Exit Function
                            End If

                        End If
                    End If
                Case 11, 18, 25, 32, 39, 46, 53, 60, 67, 74, 81, 88, 95, 102, 109

                    '2007/09/19�@�ǉ�
                    '��ڋ��z
                    If IsNumeric(strSplitValue(intCnt)) = False Then
                        pError = pSeqNo & ",��ڋ��z,��ڋ��z�ɐ��l�ȊO��ݒ肷�邱�Ƃ͂ł��܂���"
                        Exit Function
                    End If

                    '���ID�̒����`�F�b�N 2007/02/12
                    If strSplitValue(intCnt).Trim.Length >= 8 Then
                        pError = pSeqNo & ",��ڋ��z,��ڋ��z���W���𒴂��Đݒ肳��Ă��܂�"
                        Exit Function
                    End If
            End Select
        Next intCnt

        PFUNC_CHECK_STR = True

    End Function
    Private Function PFUNC_INSERT_STR(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean

        Dim intCnt As Integer
        '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
        Dim strSplitValue(109) As String
        'Dim strSplitValue() As String
        '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
        '2017/02/23 �^�X�N�j���� ADD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
        '�ǎ�p�̃��[�N�z��
        Dim strWKSplitValue() As String
        '2017/02/23 �^�X�N�j���� ADD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
        Dim strRET As String = ""

        Dim sql As New StringBuilder(128)

        PFUNC_INSERT_STR = False
        '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
        strWKSplitValue = Split(pLineValue, ",")
        Array.Copy(strWKSplitValue, strSplitValue, strWKSplitValue.Length)
        'strSplitValue = Split(pLineValue, ",")
        '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END

        'SQL���쐬
        sql.Append(" INSERT INTO HIMOMAST values(")
        'For intCnt = 0 To UBound(strSplitValue)
        For intCnt = 0 To 109 '���S�M�������@���11�`15�̕��܂ŌŒ�ł܂킷
            '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
            Select Case intCnt
                Case 0
                    '�w�Z�R�[�h
                    sql.Append("'" & strSplitValue(intCnt).Trim.PadLeft(10, "0"c) & "'")
                Case 1
                    '�w�N
                    If Trim(strSplitValue(intCnt)) = "" Then
                        sql.Append(",0")
                    Else
                        sql.Append("," & strSplitValue(intCnt))
                    End If
                Case 2
                    '���ID�OZERO�l��
                    sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")
                Case 4
                    '������
                    If Trim(strSplitValue(intCnt)) = "" Then
                        sql.Append(",'  '")
                    Else
                        sql.Append(",'" & Format(CInt(strSplitValue(intCnt)), "00") & "'")
                    End If
                Case 5, 12, 19, 26, 33, 40, 47, 54, 61, 68, 75, 82, 89, 96, 103
                    '��ږ�
                    strSplitValue(intCnt) = StrConv(strSplitValue(intCnt), VbStrConv.Wide)
                    If strSplitValue(intCnt).Trim.Length > 10 Then
                        sql.Append(",'" & strSplitValue(intCnt).Substring(0, 10) & "'")
                    Else
                        sql.Append(",'" & strSplitValue(intCnt) & "'")
                    End If
                Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69, 76, 83, 90, 97, 104
                    '���ϋ��Z�@��
                    If Trim(strSplitValue(intCnt - 1)) <> "" Then
                        sql.Append(",'" & strSplitValue(intCnt).PadLeft(4, "0"c) & "'")
                    Else
                        sql.Append(",''")
                    End If
                Case 7, 14, 21, 28, 35, 42, 49, 56, 63, 70, 77, 84, 91, 98, 105
                    '���ϋ��Z�@�֎x�X
                    If Trim(strSplitValue(intCnt - 2)) <> "" Then
                        sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(3, "0"c) & "'")
                    Else
                        sql.Append(",''")
                    End If
                Case 8, 15, 22, 29, 36, 43, 50, 57, 64, 71, 78, 85, 92, 99, 106
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
                Case 9, 16, 23, 30, 37, 44, 51, 58, 65, 72, 79, 86, 93, 100, 107
                    '�����ԍ�
                    If Trim(strSplitValue(intCnt - 4)) <> "" Then
                        sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(7, "0"c) & "'")
                    Else
                        sql.Append(",''")
                    End If
                Case 10, 17, 24, 31, 38, 45, 52, 59, 66, 73, 80, 87, 94, 101, 108
                    '���`�l�J�i
                    '2007/09/19�@�������Ⴂ��������ϊ����ݒ�
                    If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = True Then
                        sql.Append(",'" & strRET & "'")
                    Else
                        sql.Append(",'" & Space(40) & "'")
                    End If
                Case 11, 18, 25, 32, 39, 46, 53, 60, 67, 74, 81, 88, 95, 102, 109
                    '���z
                    If IsNumeric(strSplitValue(intCnt)) = True Then
                        sql.Append("," & strSplitValue(intCnt))
                    Else
                        sql.Append(",0")
                    End If

                Case Else
                    sql.Append(",'" & strSplitValue(intCnt) & "'")
            End Select
            'Select Case intCnt
            '    Case 0
            '        sql.Append("'" & strSplitValue(intCnt).Trim.PadLeft(10, "0"c) & "'")
            '        'Case 1, 11, 18, 25, 32, 39, 46, 53, 60, 67, 74, 81, 88, 95, 102, 109
            '        '���S�M������ �w�N�����1�`10�܂ł̋��z
            '    Case 1, 11, 18, 25, 32, 39, 46, 53, 60, 67, 74
            '        If Trim(strSplitValue(intCnt)) = "" Then
            '            sql.Append(",0")
            '        Else
            '            sql.Append("," & strSplitValue(intCnt))
            '        End If
            '        '���S�M�������@���11�`15(���ݒ�Ȃ̂ŌŒ�)
            '    Case 81, 88, 95, 102, 109
            '        sql.Append(",0")
            '    Case 2
            '        '���ID�OZERO�l��
            '        sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")
            '    Case 4
            '        '������
            '        If Trim(strSplitValue(intCnt)) = "" Then
            '            sql.Append(",'  '")
            '        Else
            '            sql.Append(",'" & Format(CInt(strSplitValue(intCnt)), "00") & "'")
            '        End If
            '        'Case 5, 12, 19, 26, 33, 40, 47, 54, 61, 68, 75, 82, 89, 96, 103
            '        '���S�M������ ���1�`10�܂�
            '    Case 5, 12, 19, 26, 33, 40, 47, 54, 61, 68
            '        '2007/02/15 ��ږ�
            '        strSplitValue(intCnt) = StrConv(strSplitValue(intCnt), VbStrConv.Wide)
            '        If strSplitValue(intCnt).Trim.Length > 10 Then
            '            sql.Append(",'" & strSplitValue(intCnt).Substring(0, 10) & "'")
            '        Else
            '            sql.Append(",'" & strSplitValue(intCnt) & "'")
            '        End If
            '        '���S�M�������@���11�`15(���ݒ�Ȃ̂ŌŒ�)
            '    Case 75, 82, 89, 96, 103
            '        sql.Append(",''")
            '        'Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69, 76, 83, 90, 97, 104
            '        '���S�M������ ���1�`10�܂�
            '    Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69

            '        '���ϋ��Z�@��
            '        If Trim(strSplitValue(intCnt - 1)) <> "" Then
            '            sql.Append(",'" & strSplitValue(intCnt).PadLeft(4, "0"c) & "'")
            '        Else
            '            sql.Append(",''")
            '        End If
            '        '���S�M�������@���11�`15(���ݒ�Ȃ̂ŌŒ�)
            '    Case 76, 83, 90, 97, 104
            '        sql.Append(",''")
            '        'Case 7, 14, 21, 28, 35, 42, 49, 56, 63, 70, 77, 84, 91, 98, 105
            '        '���S�M������ ���1�`10�܂�
            '    Case 7, 14, 21, 28, 35, 42, 49, 56, 63, 70
            '        '���ϋ��Z�@�֎x�X
            '        If Trim(strSplitValue(intCnt - 2)) <> "" Then
            '            sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(3, "0"c) & "'")
            '        Else
            '            sql.Append(",''")
            '        End If
            '        '���S�M�������@���11�`15(���ݒ�Ȃ̂ŌŒ�)
            '    Case 77, 84, 91, 98, 105
            '        sql.Append(",''")
            '        'Case 8, 15, 22, 29, 36, 43, 50, 57, 64, 71, 78, 85, 92, 99, 106
            '        '���S�M������ ���1�`10�܂�
            '    Case 8, 15, 22, 29, 36, 43, 50, 57, 64, 71
            '        '���ωȖ�
            '        If Trim(strSplitValue(intCnt - 3)) <> "" Then
            '            If Trim(strSplitValue(intCnt)) = "" Then
            '                sql.Append(",'01'")
            '            Else
            '                Select Case Trim(strSplitValue(intCnt)).PadLeft(2, "0"c)
            '                    Case "01", "02", "05", "37"
            '                        sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(2, "0"c) & "'")
            '                    Case Else
            '                        sql.Append(",'01'")
            '                End Select
            '            End If
            '        Else
            '            sql.Append(",''")
            '        End If
            '        '���S�M�������@���11�`15(���ݒ�Ȃ̂ŌŒ�)
            '    Case 78, 85, 92, 99, 106
            '        sql.Append(",''")
            '        'Case 9, 16, 23, 30, 37, 44, 51, 58, 65, 72, 79, 86, 93, 100, 107
            '        '���S�M������ ���1�`10�܂�
            '    Case 9, 16, 23, 30, 37, 44, 51, 58, 65, 72
            '        '�����ԍ�
            '        If Trim(strSplitValue(intCnt - 4)) <> "" Then
            '            sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(7, "0"c) & "'")
            '        Else
            '            sql.Append(",''")
            '        End If
            '        '���S�M�������@���11�`15(���ݒ�Ȃ̂ŌŒ�)
            '    Case 79, 86, 93, 100, 107
            '        sql.Append(",''")

            '    Case 10, 17, 24, 31, 38, 45, 52, 59, 66, 73
            '        '2007/09/19�@�������Ⴂ��������ϊ����ݒ�
            '        If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = True Then
            '            sql.Append(",'" & strRET & "'")
            '        Else
            '            sql.Append(",'" & Space(40) & "'")
            '        End If
            '    Case 11, 18, 25, 32, 39, 46, 53, 60, 67, 74
            '        If IsNumeric(strSplitValue(intCnt)) = True Then
            '            sql.Append(CInt(strSplitValue(intCnt)))
            '        Else
            '            sql.Append(",''")
            '        End If

            '        '���S�M�������@���11�`15�̖��`�l(���ݒ�Ȃ̂ŌŒ�)
            '    Case 80, 87, 94, 101, 108
            '        sql.Append(",''")

            '    Case Else
            '        sql.Append(",'" & strSplitValue(intCnt) & "'")
            'End Select
            '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
        Next intCnt

        sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
        sql.Append(",'00000000'")
        sql.Append(",'" & Space(50) & "'")
        sql.Append(",'" & Space(50) & "'")
        sql.Append(",'" & Space(50) & "'")
        sql.Append(",'" & Space(50) & "'")
        sql.Append(",'" & Space(50) & "'")
        sql.Append(")")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            pError = pSeqNo & ",�ړ�,�ړ��������ɃG���[���������܂���"
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_EXP_HIMOKU(ByVal pCsvPath As String) As Long

        Dim intCol As Integer
        Dim lngLine As Long = 0
        Dim strLine As String
        Dim wrt As System.IO.StreamWriter = Nothing
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        PFUNC_EXP_HIMOKU = 0

        Try
            sql.Append(" SELECT * FROM HIMOMAST")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_H ='" & Trim(txtGAKKOU_CODE.Text) & "'")
            sql.Append(" ORDER BY GAKKOU_CODE_H ASC, GAKUNEN_CODE_H ASC, HIMOKU_ID_H ASC, TUKI_NO_H ASC")

            If oraReader.DataReader(sql) = True Then

                lngLine = 0

                '���ږ����o���ݒ�
                strLine = "�w�Z�R�[�h,�w�N,���ID,��ږ�,������"
                strLine += ",���1��,���1���Z�@�փR�[�h,���1�x�X�R�[�h,���1�Ȗ�,���1�����ԍ�,���1���`�l�J�i,���1���z"
                strLine += ",���2��,���2���Z�@�փR�[�h,���2�x�X�R�[�h,���2�Ȗ�,���2�����ԍ�,���2���`�l�J�i,���2���z"
                strLine += ",���3��,���3���Z�@�փR�[�h,���3�x�X�R�[�h,���3�Ȗ�,���3�����ԍ�,���3���`�l�J�i,���3���z"
                strLine += ",���4��,���4���Z�@�փR�[�h,���4�x�X�R�[�h,���4�Ȗ�,���4�����ԍ�,���4���`�l�J�i,���4���z"
                strLine += ",���5��,���5���Z�@�փR�[�h,���5�x�X�R�[�h,���5�Ȗ�,���5�����ԍ�,���5���`�l�J�i,���5���z"
                strLine += ",���6��,���6���Z�@�փR�[�h,���6�x�X�R�[�h,���6�Ȗ�,���6�����ԍ�,���6���`�l�J�i,���6���z"
                strLine += ",���7��,���7���Z�@�փR�[�h,���7�x�X�R�[�h,���7�Ȗ�,���7�����ԍ�,���7���`�l�J�i,���7���z"
                strLine += ",���8��,���8���Z�@�փR�[�h,���8�x�X�R�[�h,���8�Ȗ�,���8�����ԍ�,���8���`�l�J�i,���8���z"
                strLine += ",���9��,���9���Z�@�փR�[�h,���9�x�X�R�[�h,���9�Ȗ�,���9�����ԍ�,���9���`�l�J�i,���9���z"
                strLine += ",���10��,���10���Z�@�փR�[�h,���10�x�X�R�[�h,���10�Ȗ�,���10�����ԍ�,���10���`�l�J�i,���10���z"
                '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                If STR_HIMOKU_PTN = "1" Then
                    strLine += ",���11��,���11���Z�@�փR�[�h,���11�x�X�R�[�h,���11�Ȗ�,���11�����ԍ�,���11���`�l�J�i,���11���z"
                    strLine += ",���12��,���12���Z�@�փR�[�h,���12�x�X�R�[�h,���12�Ȗ�,���12�����ԍ�,���12���`�l�J�i,���12���z"
                    strLine += ",���13��,���13���Z�@�փR�[�h,���13�x�X�R�[�h,���13�Ȗ�,���13�����ԍ�,���13���`�l�J�i,���13���z"
                    strLine += ",���14��,���14���Z�@�փR�[�h,���14�x�X�R�[�h,���14�Ȗ�,���14�����ԍ�,���14���`�l�J�i,���14���z"
                    strLine += ",���15��,���15���Z�@�փR�[�h,���15�x�X�R�[�h,���15�Ȗ�,���15�����ԍ�,���15���`�l�J�i,���15���z"
                End If
                '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                '2007/04/06�@��ڂ͂P�O�܂�
                'strLine += ",���11��,���11���Z�@�փR�[�h,���11�x�X�R�[�h,���11�Ȗ�,���11�����ԍ�,���11���`�l�J�i,���11���z"
                'strLine += ",���12��,���12���Z�@�փR�[�h,���12�x�X�R�[�h,���12�Ȗ�,���12�����ԍ�,���12���`�l�J�i,���12���z"
                'strLine += ",���13��,���13���Z�@�փR�[�h,���13�x�X�R�[�h,���13�Ȗ�,���13�����ԍ�,���13���`�l�J�i,���13���z"
                'strLine += ",���14��,���14���Z�@�փR�[�h,���14�x�X�R�[�h,���14�Ȗ�,���14�����ԍ�,���14���`�l�J�i,���14���z"
                'strLine += ",���15��,���15���Z�@�փR�[�h,���15�x�X�R�[�h,���15�Ȗ�,���15�����ԍ�,���15���`�l�J�i,���15���z"
                strLine += vbCrLf

                Do Until oraReader.EOF

                    '�w�Z�R�[�h
                    If IsDBNull(oraReader.GetString("GAKKOU_CODE_H")) = False Then
                        '2016/11/04 �^�X�N�j���� CHG �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- START
                        strLine += Trim(Microsoft.VisualBasic.Right(oraReader.GetString("GAKKOU_CODE_H"), GAKKOU_CODE_LENGTH))
                        'strLine += Trim(CStr(oraReader.GetString("GAKKOU_CODE_H")))
                        '2016/11/04 �^�X�N�j���� CHG �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- END
                    End If

                    '�w�N�R�[�h
                    If IsDBNull(oraReader.GetString("GAKUNEN_CODE_H")) = False Then
                        strLine += "," & Trim(CStr(oraReader.GetString("GAKUNEN_CODE_H")))
                    Else
                        strLine += ","
                    End If
                    '���ID
                    If IsDBNull(oraReader.GetString("HIMOKU_ID_H")) = False Then
                        strLine += "," & Trim(CStr(oraReader.GetString("HIMOKU_ID_H")))
                    Else
                        strLine += ","
                    End If
                    '��ږ�
                    If IsDBNull(oraReader.GetString("HIMOKU_ID_NAME_H")) = False Then
                        strLine += "," & Trim(CStr(oraReader.GetString("HIMOKU_ID_NAME_H")))
                    Else
                        strLine += ","
                    End If
                    '������
                    If IsDBNull(oraReader.GetString("TUKI_NO_H")) = False Then
                        strLine += "," & Trim(CStr(oraReader.GetString("TUKI_NO_H")))
                    Else
                        strLine += ","
                    End If

                    '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                    For intCol = 1 To intMaxHimokuCnt
                        'For intCol = 1 To 10 '2007/04/06�@��ڂ͂P�O�܂�
                        '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                        '��ږ�
                        If IsDBNull(oraReader.GetString("HIMOKU_NAME" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & Trim(CStr(oraReader.GetString("HIMOKU_NAME" & Format(intCol, "00") & "_H")))
                        Else
                            strLine += ","
                        End If

                        '���Z�@�փR�[�h
                        If IsDBNull(oraReader.GetString("KESSAI_KIN_CODE" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & CStr(oraReader.GetString("KESSAI_KIN_CODE" & Format(intCol, "00") & "_H"))
                        Else
                            strLine += ","
                        End If

                        '�x�X�R�[�h
                        If IsDBNull(oraReader.GetString("KESSAI_TENPO" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & CStr(oraReader.GetString("KESSAI_TENPO" & Format(intCol, "00") & "_H"))
                        Else
                            strLine += ","
                        End If

                        '�Ȗ�
                        If IsDBNull(oraReader.GetString("KESSAI_KAMOKU" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & CStr(oraReader.GetString("KESSAI_KAMOKU" & Format(intCol, "00") & "_H"))
                        Else
                            strLine += ","
                        End If

                        '�����ԍ�
                        If IsDBNull(oraReader.GetString("KESSAI_KOUZA" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & CStr(oraReader.GetString("KESSAI_KOUZA" & Format(intCol, "00") & "_H"))
                        Else
                            strLine += ","
                        End If

                        '�������`�l(��)
                        If IsDBNull(oraReader.GetString("KESSAI_MEIGI" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & Trim(CStr(oraReader.GetString("KESSAI_MEIGI" & Format(intCol, "00") & "_H")))
                        Else
                            strLine += ","
                        End If

                        '���z
                        If IsDBNull(oraReader.GetString("HIMOKU_KINGAKU" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & CStr(oraReader.GetString("HIMOKU_KINGAKU" & Format(intCol, "00") & "_H"))
                        Else
                            strLine += ","
                        End If
                    Next intCol

                    strLine += vbCrLf

                    lngLine += 1

                    oraReader.NextRead()

                Loop

            Else
                oraReader.Close()
                Return 0
            End If
            oraReader.Close()

            wrt = New System.IO.StreamWriter(pCsvPath, False, System.Text.Encoding.Default)

            wrt.Write(strLine)

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
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGAKKOUNAME.SelectedIndexChanged

        'COMBOBOX�I�����w�Z��,�w�Z�R�[�h�ݒ�
        lblGAKKOU_NAME.Text = cmbGAKKOUNAME.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGAKKOUNAME.SelectedIndex)

        '�w�N�e�L�X�g�{�b�N�X��FOCUS
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        If Me.txtGAKKOU_CODE.Text.Trim <> "" Then

            '�w�Z������
            sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If oraReader.DataReader(sql) = False Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                lblGAKKOU_NAME.Text = ""
                oraReader.Close()
                Exit Sub
            Else
                lblGAKKOU_NAME.Text = oraReader.GetString("GAKKOU_NNAME_G")
            End If

            oraReader.Close()
        End If
    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
