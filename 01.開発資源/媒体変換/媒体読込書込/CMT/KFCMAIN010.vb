' 2015/12/28 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text
Imports CAstBatch
Imports System.Configuration
Imports System.Xml
Imports System.Windows.Forms
Imports System.Drawing

Public Class KFCMAIN010

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
    Private MainLOG As New CASTCommon.BatchLOG("KFCMAIN010", "�}�̓Ǎ��i�}�́��f�B�X�N�j���")
    Private Const msgTitle As String = "�}�̓Ǎ��i�}�́��f�B�X�N�j���(KFCMAIN010)"
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
    Private ClickedColumn As Integer                     ' �N���b�N������ԍ�
    Private SortOrderFlag As Boolean = True              ' �\�[�g�I�[�_�[�t���O
    Private ListViewArray As ArrayList                   ' ���X�g�ҏW�pArrayList

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
        Dim COMMON_BAITAIREAD As String              ' �}�̓Ǎ��f�[�^�i�[�t�H���_
        '2016/02/05 �^�X�N�j�֓� RSV2�Ή� ADD ---------------------------------------- START
        Dim COMMON_FTR As String                     ' FTR�t�H���_
        Dim COMMON_FTRANP As String                  ' FTRANP�t�H���_
        '2016/02/05 �^�X�N�j�֓� RSV2�Ή� ADD ---------------------------------------- END
    End Structure
    Private IniInfo As INI_INFO

    Private mArgumentData As CommData

    '2016/02/08 �^�X�N�j��� RSV2�Ή� ADD ---------------------------------------- START
    Private nRecordNumber As Integer
    '2016/02/08 �^�X�N�j��� RSV2�Ή� ADD ---------------------------------------- END

    '2017/02/24 �^�X�N�j���� RSV2�Ή� ADD ---------------------------------------- START
    Private SearchFlg As Boolean
    '2017/02/24 �^�X�N�j���� RSV2�Ή� ADD ---------------------------------------- END

#End Region

#Region " ���[�h "

    Private Sub KFCMAIN010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            ' ��ʂ̏�����
            '--------------------------------
            Me.ListView1.Items.Clear()
            '2016/02/08 �^�X�N�j��� RSV2�Ή� ADD ---------------------------------------- START
            nRecordNumber = 1
            '2016/02/08 �^�X�N�j��� RSV2�Ή� ADD ---------------------------------------- END
            lblItakuCode.Text = ""
            lblItakuName.Text = ""
            lblCodeKbn.Text = ""
            lblFileName.Text = ""
            lblBaitai.Text = ""
            lblBaitaiCode.Text = ""
            SearchFlg = False

            '--------------------------------
            ' �{�^���̏�����
            '--------------------------------
            Me.btnRead.Enabled = True
            Me.btnReset.Enabled = True
            Me.btnEnd.Enabled = True

            Me.txtTorisCode.Focus()

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
    ' �Ǎ��J�n�{�^��
    '================================
    Private Sub btnRead_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRead.Click

        Dim WorkFileName As String = IniInfo.COMMON_BAITAIREAD & "WORKDATA.DAT"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "�J�n", "")

            '--------------------------------
            ' ���O���ݒ�
            '--------------------------------
            If SetLogInfo(False) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "���s", "���O���ݒ�")
                Exit Try
            End If

            '--------------------------------
            ' �����}�X�^�`�F�b�N
            '--------------------------------
            If CheckTorimast(txtTorisCode.Text, txtTorifCode.Text) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "���s", "�����}�X�^�`�F�b�N")
                Exit Try
            End If

            '--------------------------------
            ' �����J�n�m�F���b�Z�[�W
            '--------------------------------
            If MessageBox.Show(String.Format(MSG0077I, "���͂��������", "�}�̓Ǎ�����"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "���f", "���[�U�L�����Z��")
                Exit Try
            End If

            Dim CopyReturn As Integer = 0

            MainDB = New CASTCommon.MyOracle
            mArgumentData = New CommData(MainDB)
            Dim InfoParam As New CommData.stPARAMETER
            Call mArgumentData.GetTORIMAST(txtTorisCode.Text, txtTorifCode.Text)

            InfoParam.FSYORI_KBN = mArgumentData.INFOToriMast.FSYORI_KBN_T
            InfoParam.TORI_CODE = mArgumentData.INFOToriMast.TORIS_CODE_T & mArgumentData.INFOToriMast.TORIF_CODE_T
            InfoParam.BAITAI_CODE = mArgumentData.INFOToriMast.BAITAI_CODE_T
            InfoParam.FMT_KBN = mArgumentData.INFOToriMast.FMT_KBN_T
            InfoParam.FURI_DATE = ""
            InfoParam.CODE_KBN = mArgumentData.INFOToriMast.CODE_KBN_T
            InfoParam.LABEL_KBN = mArgumentData.INFOToriMast.LABEL_KBN_T
            InfoParam.RENKEI_FILENAME = ""
            InfoParam.ENC_KBN = mArgumentData.INFOToriMast.ENC_KBN_T
            InfoParam.ENC_KEY1 = mArgumentData.INFOToriMast.ENC_KEY1_T
            InfoParam.ENC_KEY2 = mArgumentData.INFOToriMast.ENC_KEY2_T
            InfoParam.ENC_OPT1 = mArgumentData.INFOToriMast.ENC_OPT1_T
            InfoParam.CYCLENO = ""
            InfoParam.JOBTUUBAN = 1
            InfoParam.TIME_STAMP = DateTime.Now.ToString("HHmmss")
            mArgumentData.INFOParameter = InfoParam

            Dim ReadFMT As New CAstFormat.CFormat
            ReadFMT.LOG = MainLOG
            ReadFMT = CAstFormat.CFormat.GetFormat(mArgumentData.INFOParameter)
            ReadFMT.ToriData = mArgumentData

            '--------------------------------
            ' �}�̓��t�@�C���`�F�b�N
            '--------------------------------
            Dim BaitaiDrive As String = String.Empty
            Dim BaitaiFileCount As Integer = 0
            Select Case lblBaitaiCode.Text
                Case "01" : BaitaiDrive = IniInfo.COMMON_FDDRIVE
                Case "11" : BaitaiDrive = IniInfo.COMMON_BAITAI_1
                Case "12" : BaitaiDrive = IniInfo.COMMON_BAITAI_2
                Case "13" : BaitaiDrive = IniInfo.COMMON_BAITAI_3
                Case "14" : BaitaiDrive = IniInfo.COMMON_BAITAI_4
                Case "15" : BaitaiDrive = IniInfo.COMMON_BAITAI_5
            End Select
            If lblBaitaiCode.Text = "01" And mArgumentData.INFOToriMast.CODE_KBN_T = "4" Then
                ' �h�a�l�t�H�[�}�b�g�e�c�̏ꍇ�̓`�F�b�N���s��Ȃ�
            Else
                BaitaiFileCount = Directory.GetFiles(BaitaiDrive).Length
                Select Case BaitaiFileCount
                    Case 0
                        MessageBox.Show(String.Format(MSG0377W, "�Z�b�g�����}�̂Ƀt�@�C�������݂��܂���"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "���s", "�}�̓��t�@�C���O��")
                        Exit Try
                    Case 1
                    Case Else
                        If SearchFlg = False Then
                            MessageBox.Show(String.Format(MSG0377W, "�Z�b�g�����}�̂Ƀt�@�C�����������݂��܂�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "���s", "�}�̓��t�@�C������")
                            Exit Try
                        End If
                End Select
            End If

            '--------------------------------
            ' �}�̓Ǎ��@�����J�n
            '--------------------------------
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            Dim ReadFileName As String = lblFileName.Text.Trim
            Select Case mArgumentData.INFOToriMast.BAITAI_CODE_T
                Case "01"
                    CopyReturn = clsFUSION.fn_FD_CPYTO_DISK(mArgumentData.INFOToriMast.TORIS_CODE_T & mArgumentData.INFOToriMast.TORIF_CODE_T, _
                                                            ReadFileName, _
                                                            WorkFileName, _
                                                            ReadFMT.RecordLen, _
                                                            mArgumentData.INFOToriMast.CODE_KBN_T, _
                                                            ReadFMT.FTRANP, _
                                                            msgTitle)
                Case Else
                    CopyReturn = clsFUSION.fn_DVD_CPYTO_DISK(mArgumentData.INFOToriMast.TORIS_CODE_T & mArgumentData.INFOToriMast.TORIF_CODE_T, _
                                                             ReadFileName, _
                                                             WorkFileName, _
                                                             ReadFMT.RecordLen, _
                                                             mArgumentData.INFOToriMast.CODE_KBN_T, _
                                                             ReadFMT.FTRANP, _
                                                             msgTitle, _
                                                             mArgumentData.INFOToriMast.BAITAI_CODE_T)
            End Select

            Select Case CopyReturn
                Case 0
                    'ReadFMT. = WorkFileName
                Case 100
                    MessageBox.Show(String.Format(MSG0371W, "�}�̓Ǎ�����", "�����̃t�@�C�������m�F���Ă��������B"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "���s", "�����ُ�܂��̓t�@�C�����Ȃ�")
                    Exit Try
                Case 200
                    MessageBox.Show(String.Format(MSG0371W, "�}�̓Ǎ�����", "�t�@�C���Ǎ����L�����Z�����܂��B"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "���f", "�t�@�C���Ǎ��L�����Z��")
                    Exit Try
                Case 300
                    MessageBox.Show(String.Format(MSG0371W, "�}�̓Ǎ�����", "�R�[�h�敪�ُ�iJIS���s�Ȃ��j"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "���s", "�R�[�h�敪�ُ�iJIS���s�Ȃ��j")
                    Exit Try
                Case 400
                    MessageBox.Show(String.Format(MSG0371W, "�}�̓Ǎ�����", "���ԃt�@�C���쐬�������s�B"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "���s", "�o�̓t�@�C���쐬:" & WorkFileName)
                    Exit Try
            End Select

            '--------------------------------
            ' �f�[�^���擾
            '--------------------------------
            Dim FuriDate As String = ""
            Dim Message As String = ""
            ListViewArray = New ArrayList
            ListViewArray.Clear()

            If GetDataInfo(ReadFMT, InfoParam, WorkFileName, FuriDate, Message) = False Then
                If Message <> "" Then
                    MessageBox.Show(String.Format(MSG0371W, "�}�̓Ǎ�����", Message), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "���s", "�f�[�^���擾�������s(" & Message & ")")
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "���s", "�f�[�^���擾�������s")
                End If
                If Not ReadFMT Is Nothing Then
                    ReadFMT.Close()
                    ReadFMT = Nothing
                End If
                Exit Try
            Else
                ReadFMT.Close()
                ReadFMT = Nothing
            End If

            '--------------------------------
            ' �t�@�C�����\�z
            '--------------------------------
            Dim FileName As String = ""
            If MakeFileName(FileName, FuriDate) = False Then
                Exit Try
            End If

            Dim FileList() As String = Directory.GetFiles(IniInfo.COMMON_BAITAIREAD)
            For i As Integer = 0 To FileList.Length - 1 Step 1
                Dim DelFileName As String = Path.GetFileName(FileList(i))
                If DelFileName <> Path.GetFileName(WorkFileName) Then
                    If DelFileName.Substring(0, 32) = FileName.Substring(0, 32) Then
                        Dim FSyoriName As String = String.Empty
                        Select Case rdbKigyo.Checked
                            Case True
                                FSyoriName = "�U�֓�"
                            Case False
                                FSyoriName = "�U����"
                        End Select

                        Dim DispName As String = String.Empty
                        Select Case FileName.Substring(6, 1)
                            Case "S"
                                DispName = "�����R�[�h�F" & FileName.Substring(8, 10) & "-" & FileName.Substring(18, 2)
                            Case "M"
                                DispName = "��\�ϑ��҃R�[�h�F" & FileName.Substring(8, 10)
                        End Select

                        If MessageBox.Show("����" & FSyoriName & "�̃f�[�^�����݂��܂��B" & vbCrLf & _
                                        "���ɓǂݍ��܂�Ă��t�@�C�����폜���A�����𑱍s���܂����H" & vbCrLf & vbCrLf & _
                                        "�@" & DispName & vbCrLf & _
                                        "�@" & FSyoriName & "�F" & FileName.Substring(24, 8), _
                                        msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then

                            MessageBox.Show("�}�̓Ǎ��������I�����܂��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "�L�����Z��", _
                                          "����" & FSyoriName & "�f�[�^����B" & _
                                          "�@" & DispName & _
                                          "�@" & FSyoriName & "�F" & FileName.Substring(24, 8))
                            Exit Try
                        End If
                    End If
                End If
            Next

            '--------------------------------
            ' ���X�g���ҏW
            '--------------------------------
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "", "���X�g���ҏW �J�n")
            For i As Integer = 0 To ListViewArray.Count - 1 Step 1
                Dim vLstItem As New ListViewItem(ListViewArray(i).ToString.Split("/"), -1, Color.Black, Color.White, Nothing)
                ListView1.Items.AddRange({vLstItem})
            Next

            '--------------------------------
            ' �t�@�C������
            '--------------------------------
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "", "�t�@�C������ �J�n(�����t�@�C���폜)")
            FileList = Directory.GetFiles(IniInfo.COMMON_BAITAIREAD)
            For i As Integer = 0 To FileList.Length - 1 Step 1
                Dim DelFileName As String = Path.GetFileName(FileList(i))
                If DelFileName <> Path.GetFileName(WorkFileName) Then
                    If DelFileName.Substring(0, 32) = FileName.Substring(0, 32) Then
                        File.Delete(FileList(i))
                    End If
                End If
            Next

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "", "�t�@�C������ �J�n(�t�@�C���R�s�[)")
            '2016/02/05 �^�X�N�j�֓� RSV2�Ή� UPD ---------------------------------------- START
            '�������ݎ��͎����}�X�^�ɐݒ肳��Ă���R�[�h�敪�ŏ�������邽�߁A
            '�R�s�[�����t�@�C�������̃R�[�h�ɖ߂��K�v������B
            If InfoParam.CODE_KBN = "0" Then
                'JIS�̏ꍇ�͂��̂܂܃R�s�[
                File.Copy(WorkFileName, Path.Combine(Me.IniInfo.COMMON_BAITAIREAD, FileName))
            Else
                'JIS�ȊO��FTRANP���g�p���ăR�[�h�ϊ����s��
                Dim MapFile As String = String.Empty
                If Me.GetPFileInfo(InfoParam.FMT_KBN, InfoParam.CODE_KBN, MapFile) = False Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�R�[�h�ϊ����擾")
                    Exit Try
                End If

                Dim GetOrPut As String = If(InfoParam.CODE_KBN = "4", "PUTRAND", "GETDATA")
                If Me.ConvertFileFtranP(GetOrPut, WorkFileName, Path.Combine(Me.IniInfo.COMMON_BAITAIREAD, FileName), Path.Combine(Me.IniInfo.COMMON_FTR, MapFile)) <> 0 Then
                    MessageBox.Show(String.Format(MSG0371W, "�}�̓Ǎ�����", "FTRANP�̃R�[�h�ϊ������Ɏ��s���܂����B"), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Try
                End If
            End If
            'File.Copy(WorkFileName, IniInfo.COMMON_BAITAIREAD & FileName)
            '2016/02/05 �^�X�N�j�֓� RSV2�Ή� UPD ---------------------------------------- END


            '--------------------------------
            ' �������b�Z�[�W�\��
            '--------------------------------
            MessageBox.Show(String.Format(MSG0078I, "�}�̓Ǎ�����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "����", "�o�̓t�@�C��:" & IniInfo.COMMON_BAITAIREAD & FileName)
            '2016/02/08 �^�X�N�j��� RSV2�Ή� ADD ---------------------------------------- START
            nRecordNumber += 1
            '2016/02/08 �^�X�N�j��� RSV2�Ή� ADD ---------------------------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "���s", ex.Message)
        Finally
            SearchFlg = False

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "", "�t�@�C������ �J�n(���[�N�t�@�C���폜)")
            If File.Exists(WorkFileName) = True Then
                File.Delete(WorkFileName)
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Ǎ��J�n", "�I��", "")
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

    Private Sub btnFileSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFileSelect.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Q��", "�J�n", "")

            '--------------------------------
            ' ���O���ݒ�
            '--------------------------------
            If SetLogInfo(False) = False Then
                Exit Try
            End If

            '--------------------------------
            ' �}�̏��擾
            '--------------------------------
            Dim BaitaiDrive As String = ""
            Select Case lblBaitaiCode.Text
                Case "01" : BaitaiDrive = IniInfo.COMMON_FDDRIVE
                Case "11" : BaitaiDrive = IniInfo.COMMON_BAITAI_1
                Case "12" : BaitaiDrive = IniInfo.COMMON_BAITAI_2
                Case "13" : BaitaiDrive = IniInfo.COMMON_BAITAI_3
                Case "14" : BaitaiDrive = IniInfo.COMMON_BAITAI_4
                Case "15" : BaitaiDrive = IniInfo.COMMON_BAITAI_5
            End Select

            Dim OPENFILEDIALOG1 As New OpenFileDialog
            OPENFILEDIALOG1.InitialDirectory = BaitaiDrive
            OPENFILEDIALOG1.Multiselect = False
            OPENFILEDIALOG1.CheckFileExists = True
            OPENFILEDIALOG1.FileName = ""
            OPENFILEDIALOG1.AddExtension = True
            OPENFILEDIALOG1.CheckFileExists = True
            Dim dlgRESULT As DialogResult
            dlgRESULT = OPENFILEDIALOG1.ShowDialog()

            If dlgRESULT = DialogResult.Cancel Then    '�L�����Z���{�^���������ꂽ��
                Exit Try
            Else
                lblFileName.Text = Path.GetFileName(OPENFILEDIALOG1.FileName)
                SearchFlg = True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Q��", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Q��", "�I��", "")
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
            If IniInfo.COMMON_FDDRIVE.ToUpper = "ERR" OrElse IniInfo.COMMON_FDDRIVE = Nothing Then
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

            IniInfo.COMMON_BAITAIREAD = CASTCommon.GetFSKJIni("COMMON", "BAITAIREAD")
            If IniInfo.COMMON_BAITAIREAD.ToUpper = "ERR" OrElse IniInfo.COMMON_BAITAIREAD = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�Ǎ��f�[�^�i�[�t�H���_", "COMMON", "BAITAIREAD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:�Ǎ��f�[�^�i�[�t�H���_ ����:COMMON ����:BAITAIREAD")
                Return False
            End If

            '2016/02/05 �^�X�N�j�֓� RSV2�Ή� ADD ---------------------------------------- START
            Me.IniInfo.COMMON_FTR = CASTCommon.GetFSKJIni("COMMON", "FTR")
            If IniInfo.COMMON_FTR.ToUpper = "ERR" OrElse IniInfo.COMMON_FTR = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "FTR�t�H���_", "COMMON", "FTR"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:FTR�t�H���_ ����:COMMON ����:FTR")
                Return False
            End If

            Me.IniInfo.COMMON_FTRANP = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
            If IniInfo.COMMON_FTRANP.ToUpper = "ERR" OrElse IniInfo.COMMON_FTRANP = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "FTRANP�t�H���_", "COMMON", "FTRANP"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:FTRANP�t�H���_ ����:COMMON ����:FTRANP")
                Return False
            End If
            '2016/02/05 �^�X�N�j�֓� RSV2�Ή� ADD ---------------------------------------- END

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
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            If rdbKigyo.Checked = True Then
                SQL.Append("     TORIMAST")
            Else
                SQL.Append("     S_TORIMAST")
            End If
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_T   = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_T   = '" & TorifCode & "'")
            SQL.Append(" AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����}�X�^�`�F�b�N", "���s", "�Y���Ȃ�")
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            Return True

        Catch ex As Exception
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
            If rdbKigyo.Checked = True Then
                SQL.Append("     TORIMAST")
            Else
                SQL.Append("     S_TORIMAST")
            End If
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
                If rdbKigyo.Checked = True Then
                    lblBaitai.Text = GetTextFileInfo(IniInfo.COMMON_TXT & "Common_�}�̃R�[�h.TXT", GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T")))
                Else
                    lblBaitai.Text = GetTextFileInfo(IniInfo.COMMON_TXT & "Common_���U_�}�̃R�[�h.TXT", GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T")))
                End If
                lblBaitaiCode.Text = GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T"))
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��ҏ��擾", "����", "")
            Else
                lblItakuCode.Text = ""
                lblItakuName.Text = ""
                lblCodeKbn.Text = ""
                lblFileName.Text = ""
                lblBaitai.Text = ""
                lblBaitaiCode.Text = ""
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
            ' ���W�I�{�^��������
            '--------------------------------
            rdbKigyo.Checked = True

            '--------------------------------
            ' �R���{�{�b�N�X������
            '--------------------------------
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
            lblItakuCode.Text = ""
            lblItakuName.Text = ""
            lblCodeKbn.Text = ""
            lblFileName.Text = ""
            lblBaitai.Text = ""
            lblBaitaiCode.Text = ""

            '--------------------------------
            ' �ϐ�������
            '--------------------------------
            SearchFlg = False

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
    ' �t�@�C�����\�z
    '================================
    Private Function MakeFileName(ByRef FileName As String, ByVal FuriDate As String) As Boolean

        Dim TimeStamp As String

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�t�@�C�����\�z", "�J�n", "")

            TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff")

            '--------------------------------
            ' �Ɩ�
            '--------------------------------
            If mArgumentData.INFOToriMast.FSYORI_KBN_T = "1" Then
                FileName = "J_"
            Else
                FileName = "S_"
            End If

            '--------------------------------
            ' �}��
            '--------------------------------
            FileName &= GetTextFileInfo(IniInfo.COMMON_TXT & "�}�̖����K��.txt", mArgumentData.INFOToriMast.BAITAI_CODE_T) & "_"

            '--------------------------------
            ' �}���`�敪
            '--------------------------------
            If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                FileName &= "S_" & mArgumentData.INFOToriMast.TORIS_CODE_T & mArgumentData.INFOToriMast.TORIF_CODE_T & "_"
            Else
                FileName &= "M_" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & "00" & "_"
            End If

            '--------------------------------
            ' �t�H�[�}�b�g�敪
            '--------------------------------
            FileName &= mArgumentData.INFOToriMast.FMT_KBN_T & "_"

            '--------------------------------
            ' �w���(�U�֓��܂��͐U����)
            '--------------------------------
            FileName &= FuriDate & "_"

            '--------------------------------
            ' ��������
            '--------------------------------
            FileName &= TimeStamp & "_"

            '--------------------------------
            ' �v���Z�X�h�c
            '--------------------------------
            FileName &= Format(Process.GetCurrentProcess.Id, "0000") & "_"

            '--------------------------------
            ' �ʔ� + �g���q
            '--------------------------------
            FileName &= "000" & ".DAT"

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�t�@�C�����\�z", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�t�@�C�����\�z", "�I��", "")
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
    ' �f�[�^���擾
    '================================
    Private Function GetDataInfo(ByVal aReadFMT As CAstFormat.CFormat, ByRef InfoParam As CommData.stPARAMETER, ByVal FileName As String, ByRef FuriDate As String, ByRef Message As String) As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�f�[�^���擾", "�J�n", "")

            '--------------------------------
            ' �t�@�C���I�[�v��
            '--------------------------------
            If aReadFMT.FirstRead(FileName) = 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�f�[�^���擾", "�t�@�C���I�[�v�����s", aReadFMT.Message)
                Message = "�G���[���e�F���ԃt�@�C���I�[�v�����s"
                Return False
            End If

            '--------------------------------
            ' �t�@�C���Ǎ�
            '--------------------------------
            Dim nRecordCount As Integer = 0                         '�t�@�C���S�̂̃��R�[�h�J�E���g
            '2016/02/08 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- START
            Dim UketukeNoDisp As String = ""
            Dim UketukeNoDispFlg As Boolean = True
            'Dim nRecordNumber As Integer = 0                        '�w�b�_�P�ʂ̃��R�[�h�J�E���g
            '2016/02/08 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- END
            Dim EndFlag As Boolean = False                          '�G���h���R�[�h���݃t���O
            Dim SplitData As String = ""
            Dim TorisCode_Header As String = ""
            Dim TorifCode_Header As String = ""
            Do Until aReadFMT.EOF
                Dim sCheckRet As String = ""
                nRecordCount += 1
                '2016/02/08 �^�X�N�j��� RSV2�Ή� DEL ---------------------------------------- START
                'nRecordNumber += 1
                '2016/02/08 �^�X�N�j��� RSV2�Ή� DEL ---------------------------------------- END

                '--------------------------------
                ' �t�H�[�}�b�g�`�F�b�N
                '--------------------------------
                Try
                    sCheckRet = aReadFMT.CheckDataFormat()
                Catch ex As Exception
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�f�[�^���擾", "�t�H�[�}�b�g�`�F�b�N", ex.Message)
                    Message = "�G���[���e�F" & nRecordCount.ToString & "�s�ځ@�t�H�[�}�b�g�`�F�b�N���s"
                    Return False
                End Try

                Select Case sCheckRet
                    Case "ERR"
                        Dim nPos As Long
                        If aReadFMT.RecordData.Length > 0 Then nPos = aReadFMT.CheckRegularString
                        If nPos > 0 Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�f�[�^���擾", "�t�H�[�}�b�g�G���[", nRecordCount.ToString & "�s�ځC" & (nPos + 1).ToString & "�o�C�g�� " & aReadFMT.Message)
                            Message = "�G���[���e�F" & nRecordCount.ToString & "�s�ځC" & (nPos + 1).ToString & "�o�C�g�� �t�H�[�}�b�g�G���[" & vbCrLf & _
                                      aReadFMT.Message
                            Return False
                        Else
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�f�[�^���擾", "�t�H�[�}�b�g�G���[", nRecordCount.ToString & "�s�� " & aReadFMT.Message)
                            Message = "�G���[���e�F" & nRecordCount.ToString & "�s�ځ@�t�H�[�}�b�g�G���[" & vbCrLf & _
                                      aReadFMT.Message
                            Return False
                        End If

                    Case "IJO"

                    Case "H"
                        EndFlag = False
                    Case "E"
                        EndFlag = True
                    Case "99"
                        EndFlag = True
                    Case "1A"
                        EndFlag = True
                    Case ""
                        Exit Do
                End Select

                '�w�b�_���R�[�h
                If aReadFMT.IsHeaderRecord() = True Then
                    '2016/02/08 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- START
                    If UketukeNoDisp = "" And UketukeNoDispFlg = True Then
                        UketukeNoDisp = nRecordNumber.ToString
                        UketukeNoDispFlg = False
                    Else
                        UketukeNoDisp = ""
                    End If
                    'nRecordNumber = 1
                    '2016/02/08 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- END

                    If CheckItakuCode(aReadFMT.InfoMeisaiMast.ITAKU_CODE, aReadFMT.InfoMeisaiMast.SYUBETU_CODE, mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T, TorisCode_Header, TorifCode_Header, Message, nRecordCount) = False Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�f�[�^���擾", "���s", "�t�@�C�����ϑ��҃R�[�h�`�F�b�N���s �ϑ��҃R�[�h:" & aReadFMT.InfoMeisaiMast.ITAKU_CODE & _
                                                                                                                                        " / ��\�ϑ��҃R�[�h:" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T)
                        Return False
                    End If

                    FuriDate = aReadFMT.InfoMeisaiMast.FURIKAE_DATE
                    '2016/02/08 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- START
                    SplitData = UketukeNoDisp & "/" & _
                                aReadFMT.InfoMeisaiMast.ITAKU_CODE & "/" & _
                                aReadFMT.InfoMeisaiMast.ITAKU_KNAME.Trim & "/" & _
                                aReadFMT.InfoMeisaiMast.FURIKAE_DATE & "/" & _
                                aReadFMT.InfoMeisaiMast.SYUBETU_CODE & "/"
                    'SplitData = nRecordNumber & "/" & _
                    '            aReadFMT.InfoMeisaiMast.ITAKU_CODE & "/" & _
                    '            aReadFMT.InfoMeisaiMast.ITAKU_KNAME.Trim & "/" & _
                    '            aReadFMT.InfoMeisaiMast.FURIKAE_DATE & "/" & _
                    '            aReadFMT.InfoMeisaiMast.SYUBETU_CODE & "/"
                    '2016/02/08 �^�X�N�j��� RSV2�Ή� UPD ---------------------------------------- END
                End If

                ' �g���[���[���R�[�h
                If aReadFMT.IsTrailerRecord Then
                    If aReadFMT.EOF = True AndAlso EndFlag = False Then
                        If aReadFMT.ToriData.INFOParameter.FMT_KBN <> "TO" Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�f�[�^���擾", "�t�H�[�}�b�g�G���[", (nRecordCount + 1).ToString & "�s�� �G���h���R�[�h������܂���")
                            Message = "�G���[���e�F�G���h���R�[�h�Ȃ�"
                            Return False
                        End If
                    End If

                    Dim Bikou As String = ""
                    Select Case rdbKigyo.Checked
                        Case True
                            If CheckSchmast(TorisCode_Header, TorifCode_Header, FuriDate) = False Then
                                Bikou = "���ޭ�قȂ�"
                            End If
                    End Select

                    SplitData &= Format(CInt(aReadFMT.InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO), "###,##0") & "/" & _
                                 Format(CLng(aReadFMT.InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO), "###,###,###,##0") & "/" & _
                                 TorisCode_Header & "-" & TorifCode_Header & "/" & _
                                 Bikou
                    ListViewArray.Add(SplitData)

                End If
            Loop

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�f�[�^���擾", "����", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�f�[�^���擾", "���s", ex.Message)
            Message = ""
            Return False
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�f�[�^���擾", "�I��", "")
        End Try

    End Function

    '================================
    ' �ϑ��҃R�[�h�`�F�b�N
    '================================
    Private Function CheckItakuCode(ByVal FileItakuCode As String, ByVal FileSyubetu As String, ByVal DispItakuKanriCode As String, ByRef TorisCode As String, ByRef TorifCode As String, ByRef RetMessage As String, ByVal RecCount As Integer) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��҃R�[�h�`�F�b�N", "�J�n", "�t�@�C�����ϑ��҃R�[�h:" & FileItakuCode & " / ��ʓ��͑�\�ϑ��҃R�[�h:" & DispItakuKanriCode)

            TorisCode = ""
            TorifCode = ""

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            If rdbKigyo.Checked = True Then
                SQL.Append("     TORIMAST")
                SQL.Append(" WHERE")
                SQL.Append("     ITAKU_CODE_T = '" & FileItakuCode & "'")
                SQL.Append(" AND SYUBETU_T    = '" & FileSyubetu & "'")
            Else
                SQL.Append("     S_TORIMAST")
                SQL.Append(" WHERE")
                SQL.Append("     ITAKU_CODE_T = '" & FileItakuCode & "'")
                SQL.Append(" AND SYUBETU_T    = '" & FileSyubetu & "'")
            End If

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then

                If GCom.NzStr(OraReader.Reader.Item("ITAKU_KANRI_CODE_T")) = DispItakuKanriCode Then
                    TorisCode = GCom.NzStr(OraReader.Reader.Item("TORIS_CODE_T"))
                    TorifCode = GCom.NzStr(OraReader.Reader.Item("TORIF_CODE_T"))
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��҃R�[�h�`�F�b�N", "����", "�����R�[�h:" & TorisCode & "-" & TorifCode)
                Else
                    RetMessage = "�G���[���e�F" & RecCount.ToString & "�s�ځ@�w�b�_���R�[�h�̈ϑ��҃R�[�h�������s(��\�ϑ��҃R�[�h�s��v)"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��҃R�[�h�`�F�b�N", "���s", "��\�ϑ��҃R�[�h�s��v")
                    Return False
                End If
            Else
                RetMessage = "�G���[���e�F" & RecCount.ToString & "�s�ځ@�w�b�_���R�[�h�̈ϑ��҃R�[�h�������s(�����Y���Ȃ�)"
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��҃R�[�h�`�F�b�N", "����", "�����}�X�^�Y���Ȃ�")
                Return False
            End If

            Return True

        Catch ex As Exception
            RetMessage = "�G���[���e�F" & RecCount.ToString & "�s�ځ@�w�b�_���R�[�h�̈ϑ��҃R�[�h�������s"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��҃R�[�h�`�F�b�N", "���s", ex.Message)
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

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��҃R�[�h�`�F�b�N", "�I��", "")
        End Try
    End Function

    Private Function CheckSchmast(ByVal TorisCode As String, ByVal TorifCode As String, ByVal FuriDate As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�X�P�W���[���}�X�^�`�F�b�N", "�J�n", "�����R�[�h:" & TorisCode & "-" & TorifCode & " / �U�֓�:" & FuriDate)

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("     FURI_DATE_S")
            SQL.Append(" FROM")
            SQL.Append("     SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_S = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_S = '" & TorifCode & "'")
            SQL.Append(" AND FURI_DATE_S  = '" & FuriDate & "'")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�X�P�W���[���}�X�^�`�F�b�N", "����", "�X�P�W���[���Ȃ�")
                Return False
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�X�P�W���[���}�X�^�`�F�b�N", "����", "�X�P�W���[������")
            Return True

        Catch ex As Exception
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

    '2016/02/05 �^�X�N�j�֓� RSV2�Ή� ADD ---------------------------------------- START
    '================================
    ' �R�[�h�ϊ����擾
    '================================
    Private Function GetPFileInfo(ByVal FmtKbn As String, _
                                  ByVal CodeKbn As String, _
                                  ByRef FtranP As String) As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�R�[�h�ϊ����擾", "�J�n", "")

            Select Case FmtKbn
                Case "00"
                    Select Case CodeKbn
                        Case "0" : FtranP = "120JIS��JIS.P"
                        Case "1" : FtranP = "120JIS��JIS��.P"
                        Case "2" : FtranP = "119JIS��JIS��.P"
                        Case "3" : FtranP = "118JIS��JIS��.P"
                        Case "4" : FtranP = "120.P"
                    End Select
                Case "01"
                    Select Case CodeKbn
                        Case "0" : FtranP = "120JIS��JIS.P"
                        Case "1" : FtranP = "120JIS��JIS��.P"
                        Case "4" : FtranP = "120.P"
                    End Select
                Case "02"
                    Select Case CodeKbn
                        Case "0" : FtranP = "390JIS��JIS.P"
                        Case "1" : FtranP = "390JIS��JIS��.P"
                        Case "4" : FtranP = "390.P"
                    End Select
                Case Else
                    If IsNumeric(FmtKbn) Then
                        Dim nFmtKbn As Integer = CInt(FmtKbn)
                        '�t�H�[�}�b�g�敪��50�`99�̏ꍇ
                        If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                            ' XML�t�H�[�}�b�g��root�I�u�W�F�N�g����
                            Dim xmlDoc As New ConfigXmlDocument
                            Dim mXmlRoot As XmlElement
                            Dim node As XmlNode
                            Dim attribute As XmlAttribute

                            'XML�p�X�쐬
                            Dim xmlFolderPath As String = CASTCommon.GetFSKJIni("COMMON", "XML_FORMAT_FLD")
                            If xmlFolderPath = "err" Or xmlFolderPath = "" Then
                                Throw New Exception("fskj.ini��XML_FORMAT_FLD����`����Ă��܂���B")
                            End If
                            If xmlFolderPath.EndsWith("\") = False Then
                                xmlFolderPath &= "\"
                            End If
                            Dim mXmlFile As String = "XML_FORMAT_" & FmtKbn & ".xml"

                            xmlDoc.Load(xmlFolderPath & mXmlFile)
                            mXmlRoot = xmlDoc.DocumentElement

                            ' �Ԋ҃t�@�C����`���t�@�C���ɃR�s�[����ۂ̃p�����[�^�t�@�C��
                            node = mXmlRoot.SelectSingleNode("�Ԋ�/�R�s�[�ݒ�ꗗ/�R�s�[�ݒ�[@�R�[�h�敪='" & CodeKbn & "']")
                            If node Is Nothing Then
                                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�R�s�[�ݒ�ꗗ/�R�s�[�ݒ�[@�R�[�h�敪='" & CodeKbn & "']�v�^�O����`����Ă��܂���B")
                            End If

                            attribute = node.Attributes.ItemOf("�p�����[�^�t�@�C��")
                            If attribute Is Nothing OrElse attribute.Value.Trim = "" Then
                                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�R�s�[�ݒ�ꗗ/�R�s�[�ݒ�v�^�O�́u�p�����[�^�t�@�C���v��������`����Ă��܂���B�i" & _
                                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                            End If
                            FtranP = attribute.Value.Trim

                        End If
                    End If
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�R�[�h�ϊ����擾", "����", _
                                 "FtranP" & FtranP)
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�R�[�h�ϊ����擾", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�R�[�h�ϊ����擾", "�I��", "")
        End Try
    End Function

    '================================
    ' FTRANP�R�[�h�ϊ�
    '================================
    Private Function ConvertFileFtranP(ByVal strGetOrPut As String, _
                                           ByVal strInFileName As String, _
                                           ByVal strOutFileName As String, _
                                           ByVal strPFileName As String) As Integer
        Try
            '�ϊ��R�}���h�g�ݗ���
            Dim Command As New StringBuilder
            With Command
                .Append(" /nwd/ cload ")
                .Append("""" & Me.IniInfo.COMMON_FTR & "FUSION" & """")
                .Append(" ; kanji 83_jis")
                .Append(" " & strGetOrPut & " ")
                .Append("""" & strInFileName & """" & " ")
                .Append("""" & strOutFileName & """" & " ")
                .Append(" ++" & """" & strPFileName & """")
            End With

            Dim Proc As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(Me.IniInfo.COMMON_FTRANP, "FP.EXE")
            ProcInfo.WorkingDirectory = Me.IniInfo.COMMON_FTRANP
            ProcInfo.Arguments = Command.ToString
            Proc = Process.Start(ProcInfo)
            Proc.WaitForExit()
            If Proc.ExitCode = 0 Then
                MainLOG.Write("(FTRANP�R�[�h�ϊ�)", "����", "�I���R�[�h�F" & Proc.ExitCode)
                Return 0
            Else
                MainLOG.Write("(FTRANP�R�[�h�ϊ�)", "���s", "�I���R�[�h�F" & Proc.ExitCode)
                Return 100
            End If
        Catch ex As Exception
            MainLOG.Write("(FTRANP�R�[�h�ϊ�)", "���s", ex.Message)
            Return 100
        End Try
    End Function
    '2016/02/05 �^�X�N�j�֓� RSV2�Ή� ADD ---------------------------------------- END

#End Region

#Region " �֐�(Sub) "

    '================================
    ' �e�L�X�g�{�b�N�X0����
    '================================
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtTorisCode.Validating, txtTorifCode.Validating

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
                lblBaitaiCode.Text = ""
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����R�[�h���͏���", "���s", ex.Message)
        Finally
            ' NOP
        End Try
    End Sub

    '================================
    ' ���X�g�̈�\�[�g
    '================================
    Private Sub SortListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick

        Try

            With CType(sender, ListView)

                '--------------------------------
                ' ����̏ꍇ�C�t��
                '--------------------------------
                If ClickedColumn = e.Column Then
                    SortOrderFlag = Not SortOrderFlag
                End If

                '--------------------------------
                ' ��ԍ��ݒ�
                '--------------------------------
                ClickedColumn = e.Column

                '--------------------------------
                ' �񐅕������z�u
                '--------------------------------
                Dim ColAlignment As HorizontalAlignment = .Columns(e.Column).TextAlign

                '--------------------------------
                ' �\�[�g
                '--------------------------------
                .ListViewItemSorter = New CASTCommon.ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)
                .Sort()

            End With

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���X�g�̈�\�[�g", "���s", ex.Message)
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
                ' �Ɩ�����(��Ǝ��U OR �����U��)
                '--------------------------------
                Dim Gyoumu As String = ""
                If rdbKigyo.Checked = True Then
                    Gyoumu = "1"
                Else
                    Gyoumu = "3"
                End If

                '--------------------------------
                ' �R���{�{�b�N�X�ݒ�
                '--------------------------------
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, Gyoumu, Jyoken) = -1 Then
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
                lblBaitaiCode.Text = ""
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
    Private Sub rdb_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbKigyo.CheckedChanged, rdbSofuri.CheckedChanged

        Try
            '--------------------------------
            ' �ǉ������ݒ�
            '--------------------------------
            Dim Jyoken As String = " AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')"   '�}�̃R�[�h���}�̂̂���

            '--------------------------------
            ' �Ɩ�����(��Ǝ��U OR �����U��)
            '--------------------------------
            Dim Gyoumu As String = ""
            If rdbKigyo.Checked = True Then
                Gyoumu = "1"
            Else
                Gyoumu = "3"
            End If

            '--------------------------------
            ' �R���{�{�b�N�X�ݒ�
            '--------------------------------
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, Gyoumu, Jyoken) = -1 Then
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
' 2015/12/28 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
