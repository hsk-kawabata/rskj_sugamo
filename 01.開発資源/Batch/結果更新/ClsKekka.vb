Option Strict On
Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CAstSystem
Imports CASTCommon.ModPublic
Imports Microsoft.VisualBasic
Imports clsFUSION.clsMain
Imports System.Windows.Forms
Imports CASTCommon.ModMessage
' �Z���^�s�\���ʍX�V����
Public Class ClsKekka

    ' �Z���^�s�\�N���p�����[�^ �\����
    Structure KOBETUPARAM
        Dim CP As CAstBatch.CommData.stPARAMETER
        Dim TKIN_CODE As String    '���s�p���Z�@�փR�[�h
        Dim MOTIKOMI_KBN As String '�������݋敪
        Dim KOUSIN_KBN As String   '�X�V�敪(0:�ʏ�X�V 1:�����X�V)
        '                                                                       
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public WriteOnly Property Data() As String
            Set(ByVal value As String)
                Dim para() As String = value.Split(","c)
                Try
                    Select Case para.Length
                        Case 4 '����/�g��������
                            CP.FURI_DATE = para(0)
                            MOTIKOMI_KBN = para(1)
                            If MOTIKOMI_KBN <> "0" Then
                                Throw New Exception("�����ُ�")
                            End If
                            CP.MODE1 = para(2)
                            CP.JOBTUUBAN = Integer.Parse(para(3))      ' �W���u�ʔ�
                            CP.RENKEI_FILENAME = "FUNOU.DAT"
                            ' �t�H�[�}�b�g�敪
                            CP.FMT_KBN = "MT"   '�s�\���ʍX�V
                        Case 5 '��Ǝ�����
                            CP.FURI_DATE = para(0)
                            MOTIKOMI_KBN = para(1)
                            '2017/01/18 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
                            Select Case MOTIKOMI_KBN
                                Case "1"
                                    CP.MODE1 = para(2)
                                    CP.TORI_CODE = para(3)
                                    CP.JOBTUUBAN = Integer.Parse(para(4))
                                    ' �t�H�[�}�b�g�敪
                                    CP.FMT_KBN = "00"   '�S��t�H�[�}�b�g

                                Case "4"
                                    CP.MODE1 = para(2)
                                    CP.FMT_KBN = para(3)
                                    CP.JOBTUUBAN = Integer.Parse(para(4))
                                    CP.RENKEI_FILENAME = "FUNOU_SYUKINDAIKOU.DAT"

                                Case Else
                                    Throw New Exception("�����ُ�")
                            End Select

                            'If MOTIKOMI_KBN <> "1" Then
                            '    Throw New Exception("�����ُ�")
                            'End If
                            'CP.MODE1 = para(2)
                            'CP.TORI_CODE = para(3)
                            'CP.JOBTUUBAN = Integer.Parse(para(4))
                            '' �t�H�[�}�b�g�敪
                            'CP.FMT_KBN = "00"   '�S��t�H�[�}�b�g
                            '2017/01/18 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
                        Case 7 '���s��
                            CP.FURI_DATE = para(0)
                            MOTIKOMI_KBN = para(1)
                            If MOTIKOMI_KBN <> "2" Then
                                Throw New Exception("�����ُ�")
                            End If
                            CP.MODE1 = para(2)
                            CP.TORI_CODE = para(3)
                            TKIN_CODE = para(4)
                            KOUSIN_KBN = para(5)
                            CP.JOBTUUBAN = Integer.Parse(para(6))
                            ' �t�H�[�}�b�g�敪
                            CP.FMT_KBN = "00"   '�S��t�H�[�}�b�g
                        Case Else '�����ُ�
                            Throw New Exception("�����ُ�")
                    End Select
                Catch ex As Exception
                    Throw New Exception(ex.Message)
                End Try
            End Set
        End Property
    End Structure
    Private mKoParam As KOBETUPARAM

    ' �W���u���b�Z�[�W
    Dim mJobMessage As String = ""

    ' �N���p�����[�^ ���ʏ��
    Private mArgumentData As CommData

    ' �˗��f�[�^�t�@�C����
    Private mDataFileName As String

    ' �p�u���b�N�c�a
    Private MainDB As CASTCommon.MyOracle

    '�r�r�r���s�X�P�X�V�s��Ή�
    Private Jikinko As String

    '2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
    Private vbDLL As New CMTCTRL.ClsFSKJ
    'Private vbDLL As New FSKJDLL.ClsFSKJ
    '2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END

    '��Ǝ����E���s�U���ݒ�p�ϐ�
    '�t�@�C����
    Private strIN_FILE_NAME As String
    Private strOUT_FILE_NAME As String
    '�i�[�t�H���_
    Private DAT_Folder As String
    Private DEN_Folder As String
    Private TAK_Folder As String
    Private DENBK_Folder As String
    Private DATBK_Folder As String
    'MT/CMT�Ǎ��p
    Private gstrMT As String
    Private strMT_FUNOU_FILE As String
    Private gstrCMT As String
    Private strCMT_FUNOU_FILE As String
    '���s�}�X�^�����p
    Private strTORIS_CODE As String
    Private strTORIF_CODE As String
    Private strTAKOU_KIN As String
    Private strTAKOU_ITAKU_CODE As String
    Private strTAKOU_ITAKU_KIN As String
    Private strTAKOU_ITAKU_SIT As String
    Private strTAKOU_ITAKU_KAMOKU As String
    Private strTAKOU_ITAKU_KOUZA As String
    Private strTAKO_BAITAI_CODE As String
    Private strTAKOU_S_FILE_NAME As String
    Private strR_FILE_NAME As String
    Private strCODE_KBN As String
    '2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
    Private strLABEL_CODE As Integer
    'Private strLABEL_CODE As Short
    '2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END
    Private TAKO_SEQ As String
    'FTRAN+��`�t�@�C�� 
    Public strP_FILE As String
    '�t�H�[�}�b�g
    '2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
    Private intREC_LENGTH As Integer
    Private intBLK_SIZE As Integer
    'Private intREC_LENGTH As Short
    'Private intBLK_SIZE As Short
    '2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END
    '�o�b�N�A�b�v�t�@�C����
    Private strBKUP_FILE As String
    '�Z���^�[�敪
    Private CENTER As String
    'ClsFusion
    Private clsFUSION As New clsFUSION.clsMain

    '�_���R�[�h
    Private MatomeNoukyo As String
    Private Noukyo_From As String
    Private Noukyo_To As String

    Private CountMinasi As Long = 0 '�݂Ȃ��X�V�����ǉ�
    '�U�֌��ʃR�[�h�ϊ��p�����N���X
    Protected Class FkekkaTbl
        Private FkekkaTblList As Hashtable      '�U�֌��ʃe�[�u�����X�g
        Private ToriCodeList As Hashtable       '�����R�[�h���X�g

        '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
        'Public Sub New()
        Public Sub New(ByVal MainDB As CASTCommon.MyOracle)
        '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            '�U�֌��ʃe�[�u�����X�g�쐬 ��������
            FkekkaTblList = New Hashtable(10)

            '�S�U�֌��ʃe�L�X�g�t�@�C���擾
            Dim KekkaTxt() As String = Directory.GetFiles(CASTCommon.GetFSKJIni("COMMON", "TXT"), "�U�֌��ʃR�[�h*.txt")

            '�e�U�֌��ʃe�L�X�g�t�@�C�����ɓ��e�擾
            For Each filename As String In KekkaTxt
                Dim sr As StreamReader = New StreamReader(filename)
                Dim ht As Hashtable = New Hashtable(30) '�U�֌��ʊi�[�p

                While sr.Peek > -1
                    Dim s() As String = sr.ReadLine().Split("="c)
                    '=�ŕ����ł���s�̂݊i�[
                    If s.Length = 2 Then
                        ht.Add(s(0), s(1))
                    End If
                End While
                sr.Close()

                '�f�t�H���g�t�@�C�����̏ꍇ�̓e�[�u��ID = 0
                If Path.GetFileName(filename).StartsWith("�U�֌��ʃR�[�h.") Then
                    FkekkaTblList.Add("0", ht)
                Else
                    '�t�@�C��������ID�擾
                    Dim id As String = Path.GetFileNameWithoutExtension(filename).Remove(0, 8)
                    FkekkaTblList.Add(id, ht)
                End If
            Next
            '�����܂�

            '�����R�[�h���X�g�쐬 ��������
            ToriCodeList = New Hashtable(3000)

            Dim SQL As StringBuilder = New StringBuilder(100)
            '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            'Dim oraReader As New CASTCommon.MyOracleReader
            Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
            '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_T || TORIF_CODE_T TORI_CODE")
            SQL.Append(",NVL(FKEKKA_TBL_T, '0') FKEKKA_TBL")
            SQL.Append(" FROM TORIMAST")

            With oraReader
                .DataReader(SQL)

                While .NextRead
                    ToriCodeList.Add(.GetString("TORI_CODE"), .GetString("FKEKKA_TBL"))
                End While

                .Close()
            End With
            '�����܂�
        End Sub

        '�U�֌��ʃe�[�u�����X�g�̃L�[���擾����
        Private Function GetFkekkaTblListKey(ByVal TORI_CODE As String) As String
            If ToriCodeList.ContainsKey(TORI_CODE) Then
                Return ToriCodeList.Item(TORI_CODE).ToString
            End If

            '�����R�[�h�����݂��Ȃ��ꍇ�͕W���̐U�֌��ʃe�[�u���̃L�[��Ԃ�
            Return "0"
        End Function

        '�U�֌��ʃe�[�u�����X�g����U�֌��ʃe�[�u�����擾����
        Private Function GetFkekkaTbl(ByVal id As String) As Hashtable
            If FkekkaTblList.ContainsKey(id) Then
                Return DirectCast(FkekkaTblList.Item(id), Hashtable)
            End If

            '�U�֌��ʃe�[�u�������݂��Ȃ��ꍇ�͕W���̐U�֌��ʃe�[�u����Ԃ�
            Return DirectCast(FkekkaTblList.Item("0"), Hashtable)
        End Function

        '�U�֌��ʃe�[�u�����X�g����U�֌��ʃR�[�h���擾����
        Public Function GetKekkaCode(ByVal TORI_CODE As String, ByVal FURIKETU_CODE As String) As Integer
            Dim id As String = GetFkekkaTblListKey(TORI_CODE)
            Dim FkekkaTbl As Hashtable = GetFkekkaTbl(id)
            Dim code As String = CAInt32(FURIKETU_CODE).ToString

            If FkekkaTbl.ContainsKey(code) Then
                Return Convert.ToInt32(FkekkaTbl.Item(code))
            End If

            '�U�֌��ʃR�[�h�����݂��Ȃ��ꍇ��ELSE�l��Ԃ�
            Return Convert.ToInt32(FkekkaTbl.Item("ELSE"))
        End Function
    End Class
    Private FKEKKA_TBL As FkekkaTbl

    ' 2016/03/09 �^�X�N�j���� ADD �yOT�zUI_B-14-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- START
    Private INI_RSV2_SYORIKEKKA_FUNOU As String
    ' 2016/03/09 �^�X�N�j���� ADD �yOT�zUI_B-14-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- END

    ' 2017/01/26 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- START
    Private INI_RSV2_EDITION As String
    Private UnmatchRecordNum As Integer = 0
    ' 2017/01/26 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- END
    
    '2017/03/13 �^�X�N�j���� ADD �W���ŏC���i���݃o�O�C���j------------------------------------ START
    '�a�������U�֕ύX�ʒm�����O���ŏo�͂����s����C��
    Private HenkoutuutiCount As Integer = 0
    '2017/03/13 �^�X�N�j���� ADD �W���ŏC���i���݃o�O�C���j------------------------------------ END

    ' New
    Public Sub New()
    End Sub

    ' �@�\�@ �F �Z���^�s�\���ʍX�V���� ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Function Main(ByVal command As String) As Integer
        ' �p�����[�^�`�F�b�N
        Try
            mKoParam.Data = command

            ' �W���u�ʔԐݒ�
            MainLOG.JobTuuban = mKoParam.CP.JOBTUUBAN
            MainLOG.ToriCode = mKoParam.CP.TORI_CODE
            MainLOG.FuriDate = mKoParam.CP.FURI_DATE
            If MainLOG.UserID.Trim = "" Then
                MainLOG.Write("���O�C�����擾", "���s", "")
                MainLOG.UpdateJOBMASTbyErr("���O�C�����擾���s")
                Return -200
            End If
            LW.UserID = MainLOG.UserID
            LW.FuriDate = mKoParam.CP.FURI_DATE
            If SetIniFIle() = False Then
                MainLOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���擾���s")
                Return -300
            End If
        Catch ex As Exception
            MainLOG.Write("�p�����^�ݒ�", "���s", "����[" & command & "] " & ex.Message)
            If mKoParam.CP.JOBTUUBAN <> 0 Then
                MainLOG.UpdateJOBMASTbyErr("�p�����[�^�ݒ莸�s")
            End If
            Return -100
        End Try

        Try
            ' �I���N��
            MainDB = New CASTCommon.MyOracle

            ' �N���p�����[�^���ʏ��
            mArgumentData = New CommData(MainDB)

            ' �p�����[�^����ݒ�
            Dim InfoParam As CommData.stPARAMETER = Nothing
            InfoParam.FMT_KBN = mKoParam.CP.FMT_KBN
            InfoParam.RENKEI_FILENAME = mKoParam.CP.RENKEI_FILENAME
            InfoParam.JOBTUUBAN = mKoParam.CP.JOBTUUBAN
            InfoParam.FURI_DATE = mKoParam.CP.FURI_DATE
            InfoParam.TORI_CODE = mKoParam.CP.TORI_CODE
            mArgumentData.INFOParameter = InfoParam
            If (mKoParam.CP.TORI_CODE).Trim <> "" Then
                LW.ToriCode = mKoParam.CP.TORI_CODE
                mArgumentData.GetTORIMAST(Mid(mKoParam.CP.TORI_CODE, 1, 10), Mid(mKoParam.CP.TORI_CODE, 11, 2))
                ' 2016/02/01 �^�X�N�j���� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�e�������R��)) -------------------- START
                ''2009/12/18 ���s�̌��ʍX�V�̏ꍇ�A�t�H�[�}�b�g�敪��ݒ�
                ''2011/01/04 �M�g�Ή� ��Ǝ����̏ꍇ�A�t�H�[�}�b�g�敪��ݒ�
                ''If mKoParam.MOTIKOMI_KBN = "2" Then
                'If mKoParam.MOTIKOMI_KBN = "2" OrElse mKoParam.MOTIKOMI_KBN = "1" Then
                '    mKoParam.CP.FMT_KBN = mArgumentData.INFOToriMast.FMT_KBN_T
                '    InfoParam.FMT_KBN = mKoParam.CP.FMT_KBN
                '    mArgumentData.INFOParameter = InfoParam
                'End If
                ''=======================================================
                '2017/05/26 �^�X�N�j���� CHG �W���ŏC���i���s���̌��ʍX�V�����C���j-------------------------- START
                '���s���̓t�H�[�}�b�g�敪��ύX���Ȃ�(NEW�����Ƃ��Ɂu00:�S��v���w�肵�Ă��� )
                If mKoParam.MOTIKOMI_KBN = "1" Then
                    'If mKoParam.MOTIKOMI_KBN = "2" OrElse mKoParam.MOTIKOMI_KBN = "1" Then
                    '2017/05/26 �^�X�N�j���� CHG �W���ŏC���i���s���̌��ʍX�V�����C���j-------------------------- END
                    If CInt(mArgumentData.INFOToriMast.FMT_KBN_T.Trim) < 50 Then
                        mKoParam.CP.FMT_KBN = mArgumentData.INFOToriMast.FMT_KBN_T
                        InfoParam.FMT_KBN = mKoParam.CP.FMT_KBN
                        mArgumentData.INFOParameter = InfoParam
                    End If
                End If
                ' 2016/02/01 �^�X�N�j���� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�e�������R��)) -------------------- END
            End If

            '2017/03/13 �^�X�N�j���� ADD �W���ŏC���i���݃o�O�C���j------------------------------------ START
            '�a�������U�֕ύX�ʒm�����O���ŏo�͂����s����C��
            HenkoutuutiCount = 0
            '2017/03/13 �^�X�N�j���� ADD �W���ŏC���i���݃o�O�C���j------------------------------------ END

            Dim bRet As Boolean
            ' ���ʍX�V����
            bRet = TourokuMain()
            If bRet = False Then
            Else
                Dim oRenkei As New ClsRenkei(mArgumentData, 1)
                Dim DestFile As String = ""
                Try
                    If mKoParam.MOTIKOMI_KBN = "2" Then
                        If strIN_FILE_NAME.Trim <> "" AndAlso strTAKO_BAITAI_CODE = "00" Then
                            oRenkei.InFileName = strIN_FILE_NAME
                        End If
                    End If
                    '2010/12/24 �M�g�Ή� ��Ǝ����̏ꍇ�̃t�@�C���ޔ�ǉ�
                    If mKoParam.MOTIKOMI_KBN = "1" Then
                        oRenkei.InFileName = strIN_FILE_NAME
                    End If
                    If oRenkei.InFileName <> "" Then
                        DestFile = oRenkei.InFileName
                        '������ێ����邽�߃��l�[��
                        Dim ToFile As String = Path.Combine(DENBK_Folder, Now.ToString("dd") & "_" & Path.GetFileName(DestFile))
                        If File.Exists(ToFile) Then File.Delete(ToFile)
                        File.Move(DestFile, ToFile)
                        DestFile = ToFile
                        MainLOG.Write("���̓t�@�C������t�H���_�ֈړ�", "����", oRenkei.InFileName & " -> " & DestFile)
                    End If
                Catch ex As Exception
                    MainLOG.Write("���̓t�@�C������t�H���_�ֈړ�", "���s", oRenkei.InFileName & " -> " & DestFile & " " & ex.Message)
                End Try
            End If

            MainDB.Close()

            MainLOG.Write("�o�^�I��", "����", bRet.ToString)


            If bRet = False Then
                Return 2
            End If

        Catch ex As Exception
            Return 3
        End Try
        Return 0
    End Function
#Region " �ݒ�t�@�C���擾"
    Private Function SetIniFIle() As Boolean
        Try
            '�����ɃR�[�h
            Jikinko = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If Jikinko.ToUpper = "ERR" OrElse Jikinko = Nothing Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�����ɃR�[�h ����:COMMON ����:KINKOCD")
                Return False
            End If
            '�Z���^�[�敪
            Center = CASTCommon.GetFSKJIni("COMMON", "CENTER")
            If CENTER.ToUpper = "ERR" OrElse CENTER = Nothing Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�Z���^�[�敪 ����:COMMON ����:CENTER")
                Return False
            End If
            'DAT�i�[�t�H���_
            DAT_Folder = CASTCommon.GetFSKJIni("COMMON", "DAT")
            If DAT_Folder.ToUpper = "ERR" OrElse DAT_Folder = Nothing Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:DAT�i�[�t�H���_ ����:COMMON ����:DAT")
                Return False
            End If
            '�`���i�[�t�H���_
            DEN_Folder = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If DEN_Folder.ToUpper = "ERR" OrElse DEN_Folder = Nothing Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�`���i�[�t�H���_ ����:COMMON ����:DEN")
                Return False
            End If
            '���s�i�[�f�B���N�g��
            TAK_Folder = CASTCommon.GetFSKJIni("COMMON", "TAK")
            If TAK_Folder.ToUpper = "ERR" OrElse TAK_Folder = Nothing Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:���s�i�[�f�B���N�g�� ����:COMMON ����:TAK")
                Return False
            End If
            '�`���o�b�N�A�b�v�i�[�t�H���_
            DENBK_Folder = CASTCommon.GetFSKJIni("COMMON", "DENBK")
            If DENBK_Folder.ToUpper = "ERR" OrElse DENBK_Folder = Nothing Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�`���o�b�N�A�b�v�i�[�t�H���_ ����:COMMON ����:DENBK")
                Return False
            End If
            '�`���o�b�N�A�b�v�i�[�t�H���_
            DATBK_Folder = CASTCommon.GetFSKJIni("COMMON", "DATBK")
            If DATBK_Folder.ToUpper = "ERR" OrElse DATBK_Folder = Nothing Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:DAT�o�b�N�A�b�v�i�[�t�H���_ ����:COMMON ����DATBK")
                Return False
            End If

            'MT�ڑ����
            gstrMT = CASTCommon.GetFSKJIni("COMMON", "MT")
            If gstrMT.ToUpper = "ERR" OrElse gstrMT = Nothing Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:MT�ڑ���� ����:COMMON ����:MT")
                Return False
            End If
            'MT�s�\�t�@�C����
            strMT_FUNOU_FILE = CASTCommon.GetFSKJIni("COMMON", "MTFUNOUFILE")
            If strMT_FUNOU_FILE.ToUpper = "ERR" OrElse strMT_FUNOU_FILE = Nothing Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:MT�s�\�t�@�C���� ����:COMMON ����:MTFUNOUFILE")
                Return False
            End If
            'CMT�ڑ����
            gstrCMT = CASTCommon.GetFSKJIni("COMMON", "CMT")
            If gstrCMT.ToUpper = "ERR" OrElse gstrCMT = Nothing Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:CMT�ڑ���� ����:COMMON ����:CMT")
                Return False
            End If
            'CMT�s�\�t�@�C����
            strCMT_FUNOU_FILE = CASTCommon.GetFSKJIni("COMMON", "CMTFUNOUFILE")
            If strCMT_FUNOU_FILE.ToUpper = "ERR" OrElse strCMT_FUNOU_FILE = Nothing Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:CMT�s�\�t�@�C���� ����:COMMON ����:CMTFUNOUFILE")
                Return False
            End If

            '���s��ƃV�[�P���X
            TAKO_SEQ = CASTCommon.GetFSKJIni("JIFURI", "TAKO_SEQ")
            If TAKO_SEQ.ToUpper = "ERR" OrElse TAKO_SEQ = Nothing Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:���s��ƃV�[�P���X ����:JIFURI ����:TAKO_SEQ")
                Return False
            End If

            '�_���֘A
            MatomeNoukyo = CASTCommon.GetFSKJIni("TAKO", "NOUKYOMATOME")
            If MatomeNoukyo.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:��\�_���R�[�h ����:TAKO ����:NOUKYOMATOME")
                Return False
            End If

            Noukyo_From = CASTCommon.GetFSKJIni("TAKO", "NOUKYOFROM")
            If Noukyo_From.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�_���R�[�h(FROM) ����:TAKO ����:NOUKYOFROM")
                Return False
            End If

            Noukyo_To = CASTCommon.GetFSKJIni("TAKO", "NOUKYOTO")
            If Noukyo_To.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�_���R�[�h(TO) ����:TAKO ����:NOUKYOTO")
                Return False
            End If

            ' 2016/03/09 �^�X�N�j���� ADD �yOT�zUI_B-14-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- START
            INI_RSV2_SYORIKEKKA_FUNOU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SYORIKEKKA_FUNOU")
            If INI_RSV2_SYORIKEKKA_FUNOU.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�������ʊm�F�\����v�� ����:RSV2_V1.0.0 ����:SYORIKEKKA_FUNOU")
                Return False
            End If
            ' 2016/03/09 �^�X�N�j���� ADD �yOT�zUI_B-14-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- END

            ' 2017/01/26 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- START
            INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            If INI_RSV2_EDITION.ToUpper = "ERR" Or INI_RSV2_EDITION = "" Then
                INI_RSV2_EDITION = "1"
            End If
            ' 2017/01/26 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- END

            Return True
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ݒ�t�@�C���擾)", "���s", ex.ToString)

        End Try
    End Function
#End Region
#Region " �s�\���ʍX�V���C������"
    ' �@�\�@ �F �s�\���ʍX�V�����i�Z���^�C���s�C��Ǝ����j
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function TourokuMain() As Boolean
        MainLOG.Write("�o�^�����J�n", "����", "")


        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 600
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 600
            End If
        End If

        Try
            ' �W���u���s���b�N
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                MainLOG.Write_Err("�o�^����", "���s", "�s�\���ʍX�V�����Ŏ��s�҂��^�C���A�E�g")
                MainLOG.UpdateJOBMASTbyErr("�s�\���ʍX�V�����Ŏ��s�҂��^�C���A�E�g")
                Return False
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            ' �p�����[�^����ݒ�
            Dim InfoParam As CommData.stPARAMETER = mArgumentData.INFOParameter
            InfoParam.FSYORI_KBN = mArgumentData.INFOToriMast.FSYORI_KBN_T
            mArgumentData.INFOParameter = InfoParam

            '�}�̓ǂݍ���
            '�t�H�[�}�b�g�@
            Dim oReadFMT As CAstFormat.CFormat
            Try
                ' �t�H�[�}�b�g�敪����C�t�H�[�}�b�g����肷��
                oReadFMT = CAstFormat.CFormat.GetFormat(mArgumentData.INFOParameter)
                If oReadFMT Is Nothing Then
                    MainLOG.Write("�t�H�[�}�b�g�擾�i�K��O�t�H�[�}�b�g�j", "���s")
                    MainLOG.UpdateJOBMASTbyErr("�t�H�[�}�b�g�擾�i�K��O�t�H�[�}�b�g�j")
                    Return False
                End If
                MainLOG.Write("�t�H�[�}�b�g�擾", "����")
            Catch ex As Exception
                MainLOG.Write("�t�H�[�}�b�g�擾", "���s", ex.Message)
                MainLOG.UpdateJOBMASTbyErr("�t�H�[�}�b�g�擾")
                Return False
            End Try

            Dim sReadFile As String = ""
            Try
                sReadFile = ReadMediaData(oReadFMT)
                ' 2016/06/14 �^�X�N�j���� ADD �yOT�zUI_B-14-99(RSV2�Ή�(�W���o�O�Ή�)) -------------------- START
                ' �\�[�X����͂��Â炢�וύX
                ''���s���X�V�敪�������X�V�̏ꍇ�A�t�@�C���Ȃ�
                ''���s���A�}�̂��˗����̏ꍇ�̓t�@�C���Ȃ�
                'If Not ((mKoParam.MOTIKOMI_KBN = "2" AndAlso mKoParam.KOUSIN_KBN = "1") OrElse _
                '        (mKoParam.MOTIKOMI_KBN = "2" AndAlso strTAKO_BAITAI_CODE = "04")) Then
                '    If sReadFile = "" Then  '�t�@�C���擾���s���̓G���[
                '        oReadFMT.Close()
                '        MainLOG.Write("�}�̓ǂݍ���", "���s", oReadFMT.Message)
                '        MainLOG.UpdateJOBMASTbyErr("�}�̓ǂݍ��ݎ��s:" & oReadFMT.Message)
                '        Return False
                '    End If
                '    Call oReadFMT.FirstRead(sReadFile)
                'End If
                'MainLOG.Write("�}�̓ǂݍ���", "����")
                Select Case mKoParam.MOTIKOMI_KBN
                    Case "2"
                        '--------------------------------------------------------------
                        ' �s�\���ʁi���s�j
                        '--------------------------------------------------------------
                        If mKoParam.KOUSIN_KBN = "1" Then
                            '----------------------------------------------------------
                            ' �����X�V�̓t�@�C������ [sReadFile]  = ""
                            '----------------------------------------------------------
                        ElseIf strTAKO_BAITAI_CODE = "04" Then
                            '----------------------------------------------------------
                            ' �˗����̓t�@�C������   [sReadFile]  = ""
                            '----------------------------------------------------------
                        Else
                            '----------------------------------------------------------
                            ' ���s�̋����X�V�E�˗����ȊO�́A�t�@�C�����擾���Ă��Ȃ�
                            ' �ꍇ�A�G���[�Ƃ���B
                            '----------------------------------------------------------
                            If sReadFile = "" Then  '�t�@�C���擾���s���̓G���[
                                oReadFMT.Close()
                                MainLOG.Write("�}�̓ǂݍ���", "���s", oReadFMT.Message)
                                MainLOG.UpdateJOBMASTbyErr("�}�̓ǂݍ��ݎ��s:" & oReadFMT.Message)
                                Return False
                            End If
                            Call oReadFMT.FirstRead(sReadFile)
                        End If
                    Case Else
                        '--------------------------------------------------------------
                        ' �s�\���ʁi���s�ȊO�j
                        ' �t�@�C�������݂��Ȃ��ꍇ�́A�G���[�Ƃ���B
                        ' �����Ɏ����Ńf�[�^���Ȃ��ꍇ�́A��ʂłO�o�C�g�t�@�C���쐬��
                        '--------------------------------------------------------------
                        If sReadFile = "" Then  '�t�@�C���擾���s���̓G���[
                            oReadFMT.Close()
                            MainLOG.Write("�}�̓ǂݍ���", "���s", oReadFMT.Message)
                            MainLOG.UpdateJOBMASTbyErr("�}�̓ǂݍ��ݎ��s:" & oReadFMT.Message)
                            Return False
                        End If
                        Call oReadFMT.FirstRead(sReadFile)
                End Select

                MainLOG.Write("�}�̓ǂݍ���", "����")

                ' 2016/06/14 �^�X�N�j���� ADD �yOT�zUI_B-14-99(RSV2�Ή�(�W���o�O�Ή�)) -------------------- END
            Catch ex As Exception
                MainLOG.Write("�}�̓ǂݍ���", "���s", ex.Message)
            End Try

            Select Case mKoParam.MOTIKOMI_KBN
                Case "0"    '����/�g������
                    MainLOG.Write("����/�g�������J�n", "����", "")

                    ' �Z���^�s�\����
                    ' ���׃}�X�^�o�^����
                    Select Case CENTER
                        Case "0" 'SKC
                            'SKC�s�\�t�H�[�}�b�g
                            If UpdateMeiMast(CType(oReadFMT, CAstFormat.CFormatSKCFunou)) = False Then
                                oReadFMT.Close()
                                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                                ' �W���u���s�A�����b�N
                                dblock.Job_UnLock(MainDB)
                                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

                                ' ���[���o�b�N
                                MainDB.Rollback()
                                Return False
                            End If
                        Case "1" '�k�C���Z���^�[
                        Case "2", "3", "5", "6"  '���k�Z���^�[,�����Z���^�[,���Z���^�[,�����Z���^�[
                            If UpdateMeiMast(CType(oReadFMT, CAstFormat.FUNOU_164_DATA)) = False Then
                                oReadFMT.Close()
                                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                                ' �W���u���s�A�����b�N
                                dblock.Job_UnLock(MainDB)
                                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

                                ' ���[���o�b�N
                                MainDB.Rollback()
                                Return False
                            End If
                        Case "4" '���C�Z���^�[
                            If UpdateMeiMast(CType(oReadFMT, CAstFormat.CFormatTokFunou)) = False Then
                                oReadFMT.Close()
                                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                                ' �W���u���s�A�����b�N
                                dblock.Job_UnLock(MainDB)
                                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

                                ' ���[���o�b�N
                                MainDB.Rollback()
                                Return False
                            End If
                        Case "7"  '��B�Z���^�[
                        Case Else

                    End Select
                Case "1" '��Ǝ����s�\����
                    MainLOG.Write("��Ǝ����o�^�J�n", "����", "")
                    '���׃}�X�^�o�^����
                    If UpdateMeiMastKigyo(CType(oReadFMT, CAstFormat.CFormatZengin)) = False Then
                        oReadFMT.Close()
                        If File.Exists(strOUT_FILE_NAME) = True Then
                            File.Delete(strOUT_FILE_NAME)
                        End If

                        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                        ' �W���u���s�A�����b�N
                        dblock.Job_UnLock(MainDB)
                        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

                        ' ���[���o�b�N
                        MainDB.Rollback()
                        Return False
                    End If
                Case "2"
                    ' ���s�s�\����
                    MainLOG.Write("���s�U���o�^�J�n", "����", "")
                    ' ���׃}�X�^�o�^����
                    If mKoParam.CP.FMT_KBN = "00" AndAlso _
                       UpdateMeiMastTako(CType(oReadFMT, CAstFormat.CFormatZengin)) = False Then    '�����ǉ�
                        oReadFMT.Close()
                        If File.Exists(strOUT_FILE_NAME) = True Then
                            File.Delete(strOUT_FILE_NAME)
                        End If

                        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                        ' �W���u���s�A�����b�N
                        dblock.Job_UnLock(MainDB)
                        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

                        ' ���[���o�b�N
                        MainDB.Rollback()
                        Return False
                    End If
                    '2017/01/18 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
                Case "4"
                    ' �X���[�G�X�s�\����
                    MainLOG.Write("�r�r�r�����o�^�J�n", "����", "")
                    ' ���׃}�X�^�o�^����
                    If UpdateMeiMastSSS(CType(oReadFMT, CAstFormat.CFormatZengin)) = False Then
                        oReadFMT.Close()
                        If File.Exists(strOUT_FILE_NAME) = True Then
                            File.Delete(strOUT_FILE_NAME)
                        End If

                        ' ���[���o�b�N
                        MainDB.Rollback()
                        Return False
                    End If
                    '2017/01/18 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END
            End Select

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή����g�������m�� ***
            ' �W���u���s�A�����b�N
            dblock.Job_UnLock(MainDB)

            ' �c�a�R�~�b�g�iUpdateMeiMast���\�b�h�ł͂Ȃ��ďo�����ŃR�~�b�g����悤�ɕύX�j
            MainDB.Commit()

            '2017/03/13 �^�X�N�j���� ADD �W���ŏC���i���݃o�O�C���j------------------------------------ START
            '�a�������U�֕ύX�ʒm�����O���ŏo�͂����s����C��
            If HenkoutuutiCount > 0 Then

                Dim HENKOU_PRINT_FLG As String = "0"
                HENKOU_PRINT_FLG = CASTCommon.GetFSKJIni("PRINT", "HENKOUTUCHI_PRINT")
                If HENKOU_PRINT_FLG = "err" OrElse HENKOU_PRINT_FLG = "" Then
                    MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�ύX�ʒm������t���O ����:PRINT ����:HENKOUTUCHI_PRINT")
                    HENKOU_PRINT_FLG = "0"          'ini�t�@�C���̎擾���s�����ꍇ�́A������Ȃ��B
                End If
                If HENKOU_PRINT_FLG = "1" Then
                    ''------------------------------------------
                    '' ���|�G�[�W�F���g���
                    ''------------------------------------------
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ' �a�������U�։��E�ύX�ʒm��
                    Dim Param As String = LW.UserID & "," & mKoParam.CP.FURI_DATE
                    MainLOG.Write("�a�������U�֕ύX�ʒm������o�b�`�ďo", "����", Param)
                    Dim nRet As Integer = ExeRepo.ExecReport("KFJP012.EXE", Param)
                    Select Case nRet
                        Case 0
                            MainLOG.Write("�a�������U�֕ύX�ʒm�����", "����", "")
                        Case -1
                            MainLOG.Write("�a�������U�֕ύX�ʒm�����", "���s", "����ΏۂȂ�")
                        Case Else
                            MainLOG.Write("�a�������U�֕ύX�ʒm�����", "���s", "")
                            MainLOG.UpdateJOBMASTbyErr("�a�������U�֕ύX�ʒm����� ���s")
                            Return False
                    End Select
                End If
            End If
            '2017/03/13 �^�X�N�j���� ADD �W���ŏC���i���݃o�O�C���j------------------------------------ END

            ' �g�����U�N�V�����J�n
            ' ����A�Öق̃g�����ł��邽�ߖ����I�R�~�b�g��NOP�ƂȂ�R�l�N�V�����N���[�X���ɃR�~�b�g�����̂�
            ' �����I�Ƀg�������J�n����
            MainDB.BeginTrans()
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή����g�������m�� ***

            '�萔���v�Z���W���u�o�^
            Dim jobid As String
            Dim para As String
            Try
                '�W���u�}�X�^�ɓo�^
                jobid = "J070"
                '�p�����[�^(�U�֓�)
                para = mKoParam.CP.FURI_DATE
                'job����
                Dim iRet As Integer
                iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                If iRet = 1 Then    '�W���u�o�^��
                    MainLOG.Write("�萔���v�Z�o�^", "���s", "�W���u�o�^�ς�")
                ElseIf iRet = -1 Then '�W���u�������s
                    MainLOG.Write("�萔���v�Z�o�^", "���s")
                Else
                    'job�o�^
                    If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then '�W���u�o�^���s
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�萔���v�Z�o�^", "���s", "�W���u�o�^���s �p�����[�^:" & para)
                    End If
                End If
            Catch ex As Exception
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�萔���v�Z�o�^", "���s", ex.ToString)
            End Try

            If sReadFile = oReadFMT.FileName Then
                oReadFMT.Close()
            Else
                oReadFMT.Close()
                File.Delete(sReadFile)
            End If

            ' JOB�}�X�^�X�V
            If MainLOG.UpdateJOBMASTbyOK(mJobMessage) = False Then
                ' ���[���o�b�N
                MainDB.Rollback()
                Return False
            End If
            MainLOG.Write("�W���u�}�X�^�X�V", "����")


            ' �c�a�R�~�b�g
            MainDB.Commit()

            Return True

        Finally
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ' �W���u���s�A�����b�N
            dblock.Job_UnLock(MainDB)

            ' ���[���o�b�N
            MainDB.Rollback()
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
        End Try
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

    End Function
    ' �@�\�@ �F �˗��f�[�^��ǂݍ���
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Public Function ReadMediaData(ByVal readfmt As CAstFormat.CFormat) As String
        Try
            ' �}�̓ǂݍ��݌���
            Dim nRetRead As Integer

            ' �A�g
            Dim oRenkei As ClsRenkei = New ClsRenkei(mArgumentData, 1)

            If mKoParam.CP.FMT_KBN = "MT" Then
                ' �Z���^�[�s�\���ʂ��O�o�C�g�̏ꍇ�̏���

                Dim InfileInfo As New FileInfo(oRenkei.InFileName)
                If InfileInfo.Exists() = True Then
                    If InfileInfo.Length = 0 Then
                        ' ���ۂO�o�C�g�ł���
                        oRenkei.OutFileName = oRenkei.InFileName

                        Return oRenkei.OutFileName
                    End If
                End If
            End If

            Select Case mKoParam.MOTIKOMI_KBN
                Case "0" '����/�M�g
                    nRetRead = oRenkei.CopyToDisk(readfmt)
                    'Return         :0=�����A50=�t�@�C���Ȃ��A100=�R�[�h�ϊ����s�A200=�R�[�h�敪�ُ�iJIS���s����j�A
                    '               :300=�R�[�h�敪�ُ�iJIS���s�Ȃ��j�A400=�o�̓t�@�C���쐬���s
                    Select Case nRetRead
                        Case 0
                            mDataFileName = oRenkei.OutFileName
                            MainLOG.Write("�t�@�C���捞", "����", "�t�@�C�����F" & oRenkei.InFileName)
                        Case 10
                            MainLOG.Write("�t�@�C���捞", "���s", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("�t�@�C���`���ُ�A�t�@�C�����F" & oRenkei.InFileName)
                            Return ""
                        Case 50
                            MainLOG.Write("�t�@�C������", "���s", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("�t�@�C���Ȃ��A�t�@�C�����F" & oRenkei.InFileName)
                            Return ""
                        Case 100
                            MainLOG.Write("�t�@�C���捞�i�R�[�h�ϊ��j", "���s", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("�t�@�C���捞�i�R�[�h�ϊ��j���s")
                            Return ""
                        Case 200
                            MainLOG.Write("�t�@�C���捞�i�R�[�h�敪�ُ�iJIS���s����j�j", "���s", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("�t�@�C���捞�i�R�[�h�敪�ُ�iJIS���s����j�j���s")
                            Return ""
                        Case 300
                            MainLOG.Write("�t�@�C���捞�i�R�[�h�敪�ُ�iJIS���s�Ȃ��j�j", "���s", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("�t�@�C���捞�i�R�[�h�敪�ُ�iJIS���s�Ȃ��j�j���s")
                            Return ""
                        Case 400
                            MainLOG.Write("�t�@�C���捞�i�o�̓t�@�C���쐬�j", "���s", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("�t�@�C���捞�i�o�̓t�@�C���쐬�j���s")
                            Return ""
                    End Select
                Case "1" '��Ǝ���
                    If fn_BAITAI_READ() Then
                        oRenkei.OutFileName = strOUT_FILE_NAME
                    Else
                        oRenkei.OutFileName = ""
                    End If
                Case "2" '���s�U��
                    ' 2016/06/14 �^�X�N�j���� ADD �yOT�zUI_B-14-99(RSV2�Ή�(�W���o�O�Ή�)) -------------------- START
                    ' �\�[�X����͂��Â炢���A�n�M���Ńo�O�����̈וύX
                    ''���s�����擾����
                    ''�}�̂���t�@�C������荞��(�˗����܂��͍X�V�敪��1:�����X�V�̏ꍇ�̓t�@�C���Ȃ�)
                    'If fn_TAKO_BAITAI_READ() OrElse (strTAKO_BAITAI_CODE <> "04" AndAlso mKoParam.KOUSIN_KBN <> "1") Then
                    '    oRenkei.OutFileName = strOUT_FILE_NAME
                    'Else
                    '    oRenkei.OutFileName = ""
                    'End If
                    If fn_TAKO_BAITAI_READ() = True Then
                        If mKoParam.KOUSIN_KBN = "1" Then
                            '----------------------------------------------
                            ' �����X�V     [sReadFile]�� ""(��) �Ƃ���
                            '----------------------------------------------
                            oRenkei.OutFileName = ""
                        ElseIf strTAKO_BAITAI_CODE = "04" Then
                            '----------------------------------------------
                            ' �˗���       [sReadFile]�� ""(��) �Ƃ���
                            '----------------------------------------------
                            oRenkei.OutFileName = ""
                        Else
                            '----------------------------------------------
                            ' ���펞       [sReadFile]�Ƀt�@�C������Ԃ�
                            '----------------------------------------------
                            oRenkei.OutFileName = strOUT_FILE_NAME
                        End If
                    Else
                        '--------------------------------------------------
                        ' �G���[��         [sReadFile]�� ""(��) �Ƃ���
                        '--------------------------------------------------
                        oRenkei.OutFileName = ""
                    End If
                    ' 2016/06/14 �^�X�N�j���� ADD �yOT�zUI_B-14-99(RSV2�Ή�(�W���o�O�Ή�)) -------------------- END
                    '2017/01/18 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
                Case "4"    '�r�r�r����
                    nRetRead = oRenkei.CopyToDisk(readfmt)
                    'Return         :0=�����A50=�t�@�C���Ȃ��A100=�R�[�h�ϊ����s�A200=�R�[�h�敪�ُ�iJIS���s����j�A
                    '               :300=�R�[�h�敪�ُ�iJIS���s�Ȃ��j�A400=�o�̓t�@�C���쐬���s
                    Select Case nRetRead
                        Case 0
                            mDataFileName = oRenkei.OutFileName
                            MainLOG.Write("�t�@�C���捞", "����", "�t�@�C�����F" & oRenkei.InFileName)
                        Case 10
                            MainLOG.Write("�t�@�C���捞", "���s", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("�t�@�C���`���ُ�A�t�@�C�����F" & oRenkei.InFileName)
                            Return ""
                        Case 50
                            MainLOG.Write("�t�@�C������", "���s", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("�t�@�C���Ȃ��A�t�@�C�����F" & oRenkei.InFileName)
                            Return ""
                        Case 100
                            MainLOG.Write("�t�@�C���捞�i�R�[�h�ϊ��j", "���s", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("�t�@�C���捞�i�R�[�h�ϊ��j���s")
                            Return ""
                        Case 200
                            MainLOG.Write("�t�@�C���捞�i�R�[�h�敪�ُ�iJIS���s����j�j", "���s", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("�t�@�C���捞�i�R�[�h�敪�ُ�iJIS���s����j�j���s")
                            Return ""
                        Case 300
                            MainLOG.Write("�t�@�C���捞�i�R�[�h�敪�ُ�iJIS���s�Ȃ��j�j", "���s", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("�t�@�C���捞�i�R�[�h�敪�ُ�iJIS���s�Ȃ��j�j���s")
                            Return ""
                        Case 400
                            MainLOG.Write("�t�@�C���捞�i�o�̓t�@�C���쐬�j", "���s", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("�t�@�C���捞�i�o�̓t�@�C���쐬�j���s")
                            Return ""
                    End Select
                    '2017/01/18 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END
            End Select

            Return oRenkei.OutFileName

        Catch ex As Exception
            MainLOG.Write("�˗��f�[�^�Ǎ�", "���s", ex.ToString)
            Return ""
        Finally

        End Try

    End Function

#End Region

#Region "���C�Z���^�[�p���׍X�V"
    ' �@�\�@ �F ���׃}�X�^�o�^����(���C�Z���^�[�p)
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateMeiMast(ByVal aReadFmt As CAstFormat.CFormatTokFunou) As Boolean
        Dim dblRECORD_COUNT As Integer = 0
        Dim dblALL_KEN As Decimal
        Dim dblALL_KINGAKU As Decimal
        Dim dblKOUSIN_KEN As Decimal


        Dim dblKIGYO_RECORD_COUNT As Integer = 0
        Dim intMINASI_COUNT As Integer = 0

        Dim sCheckRet As String                         ' �`�F�b�N��������

        Dim SQL As StringBuilder = New StringBuilder(512)

        Dim UpdateKeys As New ArrayList(1000)

        Dim strTODAY As String = DateTime.Today.ToString("yyyyMMdd")

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

        ' �S�f�[�^�`�F�b�N
        Try
            '�p�����[�^�̐U�֓���������΃V�X�e�����t�̂P�c�Ɠ��O��U�֓��Ƃ���
            If mKoParam.CP.FURI_DATE Is Nothing OrElse mKoParam.CP.FURI_DATE.Trim = "" Then
                Dim oFmt As New CAstFormat.CFormat
                oFmt.Oracle = MainDB
                Dim ZenDay As Date = CASTCommon.GetEigyobi(CASTCommon.Calendar.Now, -1, oFmt.HolidayList)
                oFmt = Nothing
                mKoParam.CP.FURI_DATE = ZenDay.ToString("yyyyMMdd")
            End If

            ' �N���p�����[�^���ʏ��
            aReadFmt.ToriData = mArgumentData

            Dim InfileInfo As New FileInfo(aReadFmt.FileName)
            If InfileInfo.Exists() = True AndAlso InfileInfo.Length = 0 Then
                ' ���ۂO�o�C�g�ł���
                MainLOG.Write("�s�\���ʌ����O������", "����")
                Dim Key(0) As String
                '���C�Z���^�[�����U�֓��Ή� ***
                Key(0) = mKoParam.CP.FURI_DATE
                '********************************************************
                UpdateKeys.Add(Key)
            Else
                ' �s�\���ʃt�@�C���ǂݍ��݊J�n
                If aReadFmt.FirstRead() = 0 Then
                    MainLOG.Write("�t�@�C���I�[�v��", "���s", aReadFmt.Message)
                    MainLOG.UpdateJOBMASTbyErr("�t�@�C���I�[�v�����s")
                    Return False
                End If
            End If

            Dim stMei As CAstFormat.CFormat.MEISAI      ' ���׃}�X�^���

            '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            'Dim FKEKKA_TBL As New FkekkaTbl
            Dim FKEKKA_TBL As New FkekkaTbl(MainDB)
            '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

            Do Until aReadFmt.EOF
                ' �f�[�^��ǂݍ���ŁC�t�H�[�}�b�g�`�F�b�N���s��
                sCheckRet = aReadFmt.CheckKekkaFormat()

                dblRECORD_COUNT += 1

                If aReadFmt.IsHeaderRecord = True Then
                    ' �w�b�_
                ElseIf aReadFmt.IsDataRecord = True Then
                    '�w�b�_�`�F�b�N�͂��Ȃ�
                    stMei = aReadFmt.InfoMeisaiMast
                    MainLOG.FuriDate = stMei.FURIKAE_DATE

                    '���C�Z���^�[�����U�֓��Ή� �U�֓��ȊO�̖��ׂ͖�������
                    If stMei.FURIKAE_DATE = mKoParam.CP.FURI_DATE Then
                        dblALL_KEN += 1
                        dblALL_KINGAKU += stMei.FURIKIN

                        Dim intCOUNT As Integer = 0
                        '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                        'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
                        OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                        '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                        SQL.Length = 0
                        SQL.Append("SELECT COUNT(*)")
                        SQL.Append(", MAX(TORIS_CODE_K || TORIF_CODE_K)")
                        SQL.Append(" FROM MEIMAST")
                        SQL.Append(" WHERE FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                        SQL.Append("   AND KIGYO_CODE_K = " & SQ(stMei.KIGYO_CODE))
                        If mKoParam.CP.MODE1 = "0" Then
                            ' �X�V�L�[����ƃV�[�P���X
                            SQL.Append("   AND KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
                        Else
                            ' �X�V�L�[���������
                            SQL.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                            SQL.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                            SQL.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                            SQL.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                            SQL.Append("   AND FURIKIN_K = " & stMei.FURIKIN)
                            '2010/01/19 �J�i�ϊ���̎��v�Ɣԍ���YOBI4_K�ɐݒ�
                            'SQL.Append("   AND JYUYOUKA_NO_K = " & SQ(stMei.JYUYOKA_NO))
                            ' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- START
                            'SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                            Select Case mKoParam.CP.MODE1
                                Case "9"
                                    ' MODE1="9"�́A���v�Ɣԍ�����������͂����X�V����
                                Case Else
                                    SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                            End Select
                            ' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- END
                            '================================================
                        End If
                        If OraMeiReader.DataReader(SQL) = True Then
                            intCOUNT = OraMeiReader.GetValueInt(0)
                        End If

                        Select Case stMei.KIGYO_SEQ.Chars(0)
                            Case "2"c, "5"c, "6"c
                                If intCOUNT = 0 Then
                                    '�������ʂ�0���̏ꍇ�A���b�Z�[�W��\��
                                    MainLOG.Write("���׌���", "���s", "�U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                                    '���ׂ�������Ȃ��Ă��ُ�I���Ƃ��Ȃ�
                                    'LOG.UpdateJOBMASTbyErr("���׌������s �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                                    'Return False
                                    GoTo NEXT_RECORD
                                    '********************************************************************
                                ElseIf intCOUNT > 1 Then
                                    MainLOG.Write("���׌���", "���s", "����������ו������݁@�U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                                    '�ُ�I���ɂ��Ȃ�
                                    'LOG.UpdateJOBMASTbyErr("���׌�������������ו������� �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                                    mJobMessage = "���׌�������������ו������� �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ
                                    MainLOG.UpdateJOBMAST(mJobMessage)
                                    '**********************************************
                                End If

                                '' �U�֌��ʃR�[�h�ϊ�
                                Dim ToriCode As String = OraMeiReader.GetValue(1).ToString
                                stMei.FURIKETU_CODE = FKEKKA_TBL.GetKekkaCode(ToriCode, aReadFmt.InfoMeisaiMast.FURIKETU_CENTERCODE)
                                '************************************************************

                                '�X�V
                                SQL = New StringBuilder(128)
                                SQL.Append("UPDATE MEIMAST")
                                SQL.Append(" SET")
                                SQL.Append("  FURIKETU_CODE_K = " & stMei.FURIKETU_CODE.ToString)
                                SQL.Append(", FURIKETU_CENTERCODE_K = " & stMei.FURIKETU_CENTERCODE.ToString)
                                SQL.Append(", MINASI_K = " & SQ(stMei.MINASI))
                                '���������i�[
                                SQL.Append(", TEISEI_SIT_K     = " & SQ(stMei.TEISEI_SIT))
                                SQL.Append(", TEISEI_KAMOKU_K  = " & SQ(stMei.TEISEI_KAMOKU))
                                SQL.Append(", TEISEI_KOUZA_K   = " & SQ(stMei.TEISEI_KOUZA))
                                SQL.Append(", TEISEI_AKAMOKU_K = " & SQ(stMei.TEISEI_AKAMOKU))
                                SQL.Append(", TEISEI_AKOUZA_K  = " & SQ(stMei.TEISEI_AKOUZA))
                                SQL.Append(", KOUSIN_DATE_K = " & SQ(strTODAY))

                                SQL.Append("WHERE FURI_DATE_K  = " & SQ(stMei.FURIKAE_DATE))
                                SQL.Append("  AND KIGYO_CODE_K = " & SQ(stMei.KIGYO_CODE))
                                If mKoParam.CP.MODE1 = "0" Then
                                    ' �X�V�L�[����ƃV�[�P���X
                                    SQL.Append("   AND KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
                                Else
                                    ' �X�V�L�[���������
                                    '���������������͌����������SEQ�̍ŏ��̂��̂̂ݍX�V����
                                    SQL.Append("   AND KIGYO_SEQ_K = ")
                                    SQL.Append("   (SELECT MIN(KIGYO_SEQ_K) FROM MEIMAST WHERE FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                                    SQL.Append("   AND KIGYO_CODE_K = " & SQ(stMei.KIGYO_CODE))
                                    SQL.Append("   AND FURIKETU_CODE_K = '0' ")
                                    SQL.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                                    SQL.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                                    SQL.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                                    SQL.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                                    SQL.Append("   AND FURIKIN_K = " & stMei.FURIKIN)
                                    ' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- START
                                    'SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                                    Select Case mKoParam.CP.MODE1
                                        Case "9"
                                            ' MODE1="9"�́A���v�Ɣԍ�����������͂����X�V����
                                        Case Else
                                            SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                                    End Select
                                    ' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- END
                                    SQL.Append("   )")
                                End If

                                Try
                                    MainDB.ExecuteNonQuery(SQL)
                                    dblKOUSIN_KEN += 1
                                    If aReadFmt.InfoMeisaiMast.MINASI = "1" Then
                                        CountMinasi += 1
                                    End If
                                Catch ex As Exception
                                    MainLOG.Write("���׍X�V", "���s", "�U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ & " " & ex.Message & ":" & ex.StackTrace)
                                    MainLOG.UpdateJOBMASTbyErr("���׍X�V �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                                    Return False
                                End Try

                                ' �X�P�W���[���L�[���ۑ�
                                Dim Key(0) As String
                                Key(0) = stMei.FURIKAE_DATE

                                Dim oSearch As New mySearchClass
                                If UpdateKeys.BinarySearch(Key, oSearch) < 0 Then
                                    '���C�Z���^�[�s�\�f�[�^�d�l �U�֓��͂Q���������Ă��邽�߈ُ�Ƃ��Ȃ�
                                    'If mKoParam.CP.RENKEI_KBN = "99" AndAlso mKoParam.CP.FURI_DATE <> stMei.FURIKAE_DATE Then
                                    '    LOG.Write("�U�֓��s��v", "���s", "���͐U�֓��F" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �f�[�^�U�֓��F" & ConvertDate(stMei.FURIKAE_DATE, "yyyy�NMM��dd��"))
                                    '    LOG.UpdateJOBMASTbyErr("�U�֓��s��v ���͐U�֓��F" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �f�[�^�U�֓��F" & ConvertDate(stMei.FURIKAE_DATE, "yyyy�NMM��dd��"))
                                    '    Return False
                                    'End If
                                    '*************************************************************************************************
                                    UpdateKeys.Add(Key)
                                End If

                                OraMeiReader.Close()

                            Case Else
                                '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                                OraMeiReader.Close()
                                '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                                '���Ɏ����ȊO
                                dblKIGYO_RECORD_COUNT += 1

                        End Select
                    End If
                End If
NEXT_RECORD:
            Loop

            '���C�Z���^�[�����U�֓��Ή�
            '�w��U�֓��̍X�V���ׂ��Ȃ��Ƃ��A�K���w��U�֓��̌��ʍX�V���s��
            Dim opSearch As New mySearchClass
            Dim ParamKey(0) As String
            ParamKey(0) = mKoParam.CP.FURI_DATE
            If UpdateKeys.BinarySearch(ParamKey, opSearch) < 0 Then
                UpdateKeys.Add(ParamKey)
            End If
            '********************************************************
            For i As Integer = 0 To UpdateKeys.Count - 1
                ' �X�P�W���[���X�V
                Dim Keys() As String = CType(UpdateKeys(i), String())
                If UpdateSCHMAST(Keys, True) = False Then
                    Return False
                End If
            Next i

            '*** Str Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            'MainDB.Commit()
            '*** End Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            '�G���[���b�Z�[�W���Ȃ��ꍇ
            If mJobMessage = "" Then
                '�����\�� ���R�[�h�����ǉ�
                '���R�[�h�����E�E�E�S���R�[�h����(�w�b�_������ꍇ�͊܂܂��)
                '�Ǎ������E�E�E�E�E�w��U�֓��݂̂̃f�[�^���R�[�h����
                '�X�V�����E�E�E�E�E���ۂɍX�V���ꂽ���R�[�h����
                '���Ǎ������E�X�V�������O���̏ꍇ�́A�U�֓����Ⴄ�\���L��
                mJobMessage = "���R�[�h�����F" & dblRECORD_COUNT & " �Ǎ������F" & dblALL_KEN & " �X�V�����F" & dblKOUSIN_KEN & " �݂Ȃ��X�V����:" & CountMinasi
                MainLOG.Write("���ʍX�V", "����", mJobMessage)
            End If

            '2017/03/13 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
            '�a�������U�֕ύX�ʒm�����O���ŏo�͂����s����C��
            'TourokuMain�̃R�~�b�g��Ɉ������
            HenkoutuutiCount = UpdateKeys.Count
            'If UpdateKeys.Count > 0 Then
            '    '2011/06/29 �W���ŏC�� �a�������U�֕ύX�ʒm������I�� ------------------START
            '    Dim HENKOU_PRINT_FLG As String = "0"
            '    HENKOU_PRINT_FLG = CASTCommon.GetFSKJIni("PRINT", "HENKOUTUCHI_PRINT")
            '    If HENKOU_PRINT_FLG = "err" OrElse HENKOU_PRINT_FLG = "" Then
            '        MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�ύX�ʒm������t���O ����:PRINT ����:HENKOUTUCHI_PRINT")
            '        HENKOU_PRINT_FLG = "0"          'ini�t�@�C���̎擾���s�����ꍇ�́A������Ȃ��B
            '    End If
            '    If HENKOU_PRINT_FLG = "1" Then
            '        '2011/06/29 �W���ŏC�� �a�������U�֕ύX�ʒm������I�� ------------------END

            '        ''------------------------------------------
            '        '' ���|�G�[�W�F���g���
            '        ''------------------------------------------
            '        Dim ExeRepo As New CAstReports.ClsExecute
            '        ' �a�������U�։��E�ύX�ʒm��
            '        Dim Keys() As String = CType(UpdateKeys(0), String())
            '        Dim Param As String = LW.UserID & "," & Keys(0).TrimEnd
            '        MainLOG.Write("�a�������U�֕ύX�ʒm������o�b�`�ďo", "����", Param)
            '        Dim nRet As Integer = ExeRepo.ExecReport("KFJP012.EXE", Param)
            '        Select Case nRet
            '            Case 0
            '                MainLOG.Write("�a�������U�֕ύX�ʒm�����", "����", "")
            '            Case -1
            '                MainLOG.Write("�a�������U�֕ύX�ʒm�����", "���s", "����ΏۂȂ�")
            '            Case Else
            '                MainLOG.Write("�a�������U�֕ύX�ʒm�����", "���s", "")
            '                MainLOG.UpdateJOBMASTbyErr("�a�������U�֕ύX�ʒm����� ���s")
            '                Return False
            '        End Select
            '        '2011/06/29 �W���ŏC�� �a�������U�֕ύX�ʒm������I�� ------------------START
            '    End If
            '    '2011/06/29 �W���ŏC�� �a�������U�֕ύX�ʒm������I�� ------------------END
            'End If
            '2017/03/13 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END

        Catch ex As Exception
            MainLOG.Write("���׃}�X�^�o�^����", "���s", ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("�V�X�e���G���[�i���O�Q�Ɓj")

            Return False
        Finally
            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

        End Try
        Return True
    End Function

#End Region

#Region "���k�E�����E���E�����Z���^�[�p���׍X�V"
    ' �@�\�@ �F ���׃}�X�^�o�^����(���k�E�����E���E�����Z���^�[)
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateMeiMast(ByVal aReadFmt As CAstFormat.FUNOU_164_DATA) As Boolean
        Dim dblRECORD_COUNT As Integer = 0
        Dim dblALL_KEN As Decimal
        Dim dblALL_KINGAKU As Decimal
        Dim dblKOUSIN_KEN As Decimal

        Dim dblKIGYO_RECORD_COUNT As Integer = 0
        Dim intMINASI_COUNT As Integer = 0

        Dim sCheckRet As String                         ' �`�F�b�N��������

        Dim SQL As StringBuilder = New StringBuilder(512)

        Dim UpdateKeys As New ArrayList(1000)

        Dim strTODAY As String = DateTime.Today.ToString("yyyyMMdd")

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***


        ' �S�f�[�^�`�F�b�N
        Try
            ' �N���p�����[�^���ʏ��
            aReadFmt.ToriData = mArgumentData
            Dim InfileInfo As New FileInfo(aReadFmt.FileName)
            If InfileInfo.Exists() = True AndAlso InfileInfo.Length = 0 Then
                ' ���ۂO�o�C�g�ł���
                MainLOG.Write("�s�\���ʌ����O������", "����")

                Dim Key(0) As String
                If mKoParam.CP.FURI_DATE Is Nothing OrElse mKoParam.CP.FURI_DATE.Trim = "" Then
                    ' �O�c�Ɠ�
                    Dim oFmt As New CAstFormat.CFormat
                    oFmt.Oracle = MainDB
                    Dim ZenDay As Date = CASTCommon.GetEigyobi(CASTCommon.Calendar.Now, -1, oFmt.HolidayList)
                    oFmt = Nothing
                    Key(0) = ZenDay.ToString("yyyyMMdd")
                Else
                    Key(0) = mKoParam.CP.FURI_DATE
                End If
                UpdateKeys.Add(Key)
            Else
                ' �s�\���ʃt�@�C���ǂݍ��݊J�n
                If aReadFmt.FirstRead() = 0 Then
                    MainLOG.Write("�t�@�C���I�[�v��", "���s", aReadFmt.Message)
                    MainLOG.UpdateJOBMASTbyErr("�t�@�C���I�[�v�����s")
                    Return False
                End If
            End If

            Dim stMei As CAstFormat.CFormat.MEISAI      ' ���׃}�X�^���
            '�U�֌��ʃe�[�u�����[�h 

            '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            'Dim FKEKKA_TBL As New FkekkaTbl
            Dim FKEKKA_TBL As New FkekkaTbl(MainDB)
            '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

            Do Until aReadFmt.EOF
                ' �f�[�^��ǂݍ���ŁC�t�H�[�}�b�g�`�F�b�N���s��
                sCheckRet = aReadFmt.CheckKekkaFormat()

                dblRECORD_COUNT += 1

                If aReadFmt.IsDataRecord = True Then

                    stMei = aReadFmt.InfoMeisaiMast

                    MainLOG.FuriDate = stMei.FURIKAE_DATE

                    dblALL_KEN += 1
                    dblALL_KINGAKU += stMei.FURIKIN

                    Dim intCOUNT As Integer = 0
                    '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                    'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
                    OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                    '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                    SQL.Length = 0
                    SQL.Append("SELECT COUNT(*)")
                    SQL.Append(", MAX(TORIS_CODE_K || TORIF_CODE_K)")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                    SQL.Append("   AND TRIM(KIGYO_CODE_K) = " & SQ(stMei.KIGYO_CODE))
                    If mKoParam.CP.MODE1 = "0" Then
                        ' �X�V�L�[����ƃV�[�P���X
                        SQL.Append("   AND KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
                    Else
                        ' �X�V�L�[���������
                        SQL.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                        SQL.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                        SQL.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                        SQL.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                        '�U�֋��z���l��
                        SQL.Append("   AND FURIKIN_K = " & CInt(stMei.FURIKIN))
                        '2010/01/19 �J�i�ϊ���̎��v�Ɣԍ���YOBI4_K�ɐݒ�
                        ' SQL.Append("   AND TRIM(JYUYOUKA_NO_K) = " & SQ(stMei.JYUYOKA_NO.Trim))
                        ' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- START
                        'SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                        Select Case mKoParam.CP.MODE1
                            Case "9"
                                ' MODE1="9"�́A���v�Ɣԍ�����������͂����X�V����
                            Case Else
                                SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                        End Select
                        ' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- END
                        '================================================
                    End If
                    If OraMeiReader.DataReader(SQL) = True Then
                        intCOUNT = OraMeiReader.GetValueInt(0)
                    End If

                    Select Case stMei.KIGYO_SEQ.Chars(0)
                        Case "2"c, "5"c, "6"c
                            If intCOUNT = 0 Then
                                '�������ʂ�0���̏ꍇ�A���b�Z�[�W��\��
                                ' 2017/01/26 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- START
                                'With stMei
                                '    If MessageBox.Show(String.Format(MSG0074I, .HENKANKBN, _
                                '                                               .FURIKAE_DATE, _
                                '                                               .KIGYO_CODE, _
                                '                                               .KEIYAKU_KIN, .KEIYAKU_SIT, _
                                '                                               .KEIYAKU_KAMOKU, .KEIYAKU_KOUZA, _
                                '                                               .KIGYO_SEQ), _
                                '                        msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.OK Then
                                '        GoTo NEXT_RECORD
                                '    Else
                                '        MainLOG.Write("���׌���", "���s", "�U�֓��F" & .FURIKAE_DATE & " ��ƃR�[�h�F" & .KIGYO_CODE & " ��ƃV�[�P���X�F" & .KIGYO_SEQ)
                                '        MainLOG.UpdateJOBMASTbyErr("���׌������s �U�֓��F" & .FURIKAE_DATE & " ��ƃR�[�h�F" & .KIGYO_CODE & " ��ƃV�[�P���X�F" & .KIGYO_SEQ)
                                '        Return False
                                '    End If
                                '    MainLOG.Write("���׌���", "���s", "�U�֓��F" & .FURIKAE_DATE & " ��ƃR�[�h�F" & .KIGYO_CODE & " ��ƃV�[�P���X�F" & .KIGYO_SEQ)

                                'End With
                                Select Case INI_RSV2_EDITION
                                    Case "2"
                                        With stMei
                                            MainLOG.Write("���׌���", "���s", "�U�֓��F" & .FURIKAE_DATE & " ��ƃR�[�h�F" & .KIGYO_CODE & " ��ƃV�[�P���X�F" & .KIGYO_SEQ)
                                            UnmatchRecordNum += 1
                                        End With
                                        GoTo NEXT_RECORD
                                    Case Else
                                        With stMei
                                            If MessageBox.Show(String.Format(MSG0074I, .HENKANKBN, _
                                                                                       .FURIKAE_DATE, _
                                                                                       .KIGYO_CODE, _
                                                                                       .KEIYAKU_KIN, .KEIYAKU_SIT, _
                                                                                       .KEIYAKU_KAMOKU, .KEIYAKU_KOUZA, _
                                                                                       .KIGYO_SEQ), _
                                                                msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.OK Then
                                                GoTo NEXT_RECORD
                                            Else
                                                MainLOG.Write("���׌���", "���s", "�U�֓��F" & .FURIKAE_DATE & " ��ƃR�[�h�F" & .KIGYO_CODE & " ��ƃV�[�P���X�F" & .KIGYO_SEQ)
                                                MainLOG.UpdateJOBMASTbyErr("���׌������s �U�֓��F" & .FURIKAE_DATE & " ��ƃR�[�h�F" & .KIGYO_CODE & " ��ƃV�[�P���X�F" & .KIGYO_SEQ)
                                                Return False
                                            End If
                                        End With
                                End Select
                                ' 2017/01/26 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- END
                                'MainLOG.Write("���׌���", "���s", "�U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                                'MainLOG.UpdateJOBMASTbyErr("���׌������s �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                                'Return False
                                '2011/06/16 �W���ŏC�� �}�X�^�ɑ��݂��Ȃ����ׂ��l�� ------------------END

                            ElseIf intCOUNT > 1 Then
                                MainLOG.Write("���׌���", "���s", "����������ו������݁@�U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                                ' �ُ�I���ɂ��Ȃ�
                                'LOG.UpdateJOBMASTbyErr("���׌�������������ו������� �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                                mJobMessage = "���׌�������������ו������� �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ
                                MainLOG.UpdateJOBMAST(mJobMessage)
                            End If

                            Dim ToriCode As String = OraMeiReader.GetValue(1).ToString

                            '�݂Ȃ������t���O��"1"�̏ꍇ�A�U�֌��ʃR�[�h0�ɍX�V����B���Z���^�[���ʃR�[�h�����̂܂�
                            Select Case aReadFmt.InfoMeisaiMast.MINASI
                                Case "1"
                                    stMei.FURIKETU_CODE = FKEKKA_TBL.GetKekkaCode(ToriCode, "00")
                                    CountMinasi += 1
                                Case Else
                                    stMei.FURIKETU_CODE = FKEKKA_TBL.GetKekkaCode(ToriCode, aReadFmt.InfoMeisaiMast.FURIKETU_CENTERCODE)
                            End Select

                            '�X�V
                            SQL = New StringBuilder(128)
                            SQL.Append("UPDATE MEIMAST")
                            SQL.Append(" SET")
                            SQL.Append("  FURIKETU_CODE_K = " & stMei.FURIKETU_CODE.ToString)
                            SQL.Append(", FURIKETU_CENTERCODE_K = " & stMei.FURIKETU_CENTERCODE.ToString)
                            SQL.Append(", MINASI_K = " & SQ(stMei.MINASI))
                            '���������i�[
                            SQL.Append(", TEISEI_SIT_K     = " & SQ(stMei.TEISEI_SIT))
                            SQL.Append(", TEISEI_KAMOKU_K  = " & SQ(stMei.TEISEI_KAMOKU))
                            SQL.Append(", TEISEI_KOUZA_K   = " & SQ(stMei.TEISEI_KOUZA))
                            SQL.Append(", TEISEI_AKAMOKU_K = " & SQ(stMei.TEISEI_AKAMOKU))
                            SQL.Append(", TEISEI_AKOUZA_K  = " & SQ(stMei.TEISEI_AKOUZA))
                            SQL.Append(", KOUSIN_DATE_K = " & SQ(strTODAY))
                            SQL.Append("WHERE FURI_DATE_K  = " & SQ(stMei.FURIKAE_DATE))
                            SQL.Append("  AND TRIM(KIGYO_CODE_K) = " & SQ(stMei.KIGYO_CODE))
                            If mKoParam.CP.MODE1 = "0" Then
                                ' �X�V�L�[����ƃV�[�P���X
                                SQL.Append("   AND KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
                            Else
                                ' �X�V�L�[���������
                                '���������������͌����������SEQ�̍ŏ��̂��̂̂ݍX�V����
                                SQL.Append("   AND KIGYO_SEQ_K = ")
                                SQL.Append("   (SELECT MIN(KIGYO_SEQ_K) FROM MEIMAST WHERE FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                                SQL.Append("   AND KIGYO_CODE_K = " & SQ(stMei.KIGYO_CODE))
                                SQL.Append("   AND FURIKETU_CODE_K = '0' ")
                                SQL.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                                SQL.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                                SQL.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                                SQL.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                                SQL.Append("   AND FURIKIN_K = " & stMei.FURIKIN)
                                '2010/01/19 �J�i�ϊ���̎��v�Ɣԍ���YOBI4_K�ɐݒ�
                                ' SQL.Append("   AND TRIM(JYUYOUKA_NO_K) = " & SQ(stMei.JYUYOKA_NO.Trim))
                                ' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- START
                                'SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                                Select Case mKoParam.CP.MODE1
                                    Case "9"
                                        ' MODE1="9"�́A���v�Ɣԍ�����������͂����X�V����
                                    Case Else
                                        SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                                End Select
                                ' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- END
                                '================================================
                                SQL.Append("   )")

                            End If

                            Try
                                MainDB.ExecuteNonQuery(SQL)
                                dblKOUSIN_KEN += 1
                            Catch ex As Exception
                                MainLOG.Write("���׍X�V", "���s", "�U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ & " " & ex.Message & ":" & ex.StackTrace)
                                MainLOG.UpdateJOBMASTbyErr("���׍X�V �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                                Return False
                            End Try

                            ' �X�P�W���[���L�[���ۑ�
                            Dim Key(0) As String
                            Key(0) = stMei.FURIKAE_DATE

                            Dim oSearch As New mySearchClass
                            If UpdateKeys.BinarySearch(Key, oSearch) < 0 Then
                                If mKoParam.CP.FURI_DATE <> stMei.FURIKAE_DATE Then
                                    MainLOG.Write("�U�֓��s��v", "���s", "���͐U�֓��F" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �f�[�^�U�֓��F" & ConvertDate(stMei.FURIKAE_DATE, "yyyy�NMM��dd��"))
                                    MainLOG.UpdateJOBMASTbyErr("�U�֓��s��v ���͐U�֓��F" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �f�[�^�U�֓��F" & ConvertDate(stMei.FURIKAE_DATE, "yyyy�NMM��dd��"))
                                    Return False
                                End If

                                UpdateKeys.Add(Key)
                            End If
                        Case Else
                            '���Ɏ����ȊO�̃f�[�^
                            dblKIGYO_RECORD_COUNT += 1
                    End Select

                    '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                    OraMeiReader.Close()
                    '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                End If
NEXT_RECORD:
            Loop

            For i As Integer = 0 To UpdateKeys.Count - 1

                Dim Keys() As String = CType(UpdateKeys(i), String())


                ' �X�P�W���[���X�V
                If UpdateSCHMAST(Keys, True) = False Then
                    Return False
                End If
            Next i

            '*** Str Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            'MainDB.Commit()
            '*** End Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            '�G���[���b�Z�[�W���Ȃ��ꍇ�����\��
            If mJobMessage = "" Then
                ' 2017/01/26 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- START
                'mJobMessage = "�Ǎ������F" & dblALL_KEN & " �X�V�����F" & dblKOUSIN_KEN & " �݂Ȃ��X�V����:" & CountMinasi
                If UnmatchRecordNum > 0 Then
                    mJobMessage = "�� ���X�V�����F" & UnmatchRecordNum & " �Ǎ������F" & dblALL_KEN & " �X�V�����F" & dblKOUSIN_KEN & " �݂Ȃ��X�V����:" & CountMinasi
                Else
                    mJobMessage = "�Ǎ������F" & dblALL_KEN & " �X�V�����F" & dblKOUSIN_KEN & " �݂Ȃ��X�V����:" & CountMinasi
                End If
                ' 2017/01/26 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- END
                MainLOG.Write("���ʍX�V", "����", mJobMessage)
            End If

            '2017/03/13 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
            '�a�������U�֕ύX�ʒm�����O���ŏo�͂����s����C��
            'TourokuMain�̃R�~�b�g��Ɉ������
            HenkoutuutiCount = UpdateKeys.Count
            'If UpdateKeys.Count > 0 Then
            '    '2011/06/29 �W���ŏC�� �a�������U�֕ύX�ʒm������I�� ------------------START
            '    Dim HENKOU_PRINT_FLG As String = "0"
            '    HENKOU_PRINT_FLG = CASTCommon.GetFSKJIni("PRINT", "HENKOUTUCHI_PRINT")
            '    If HENKOU_PRINT_FLG = "err" OrElse HENKOU_PRINT_FLG = "" Then
            '        MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�ύX�ʒm������t���O ����:PRINT ����:HENKOUTUCHI_PRINT")
            '        HENKOU_PRINT_FLG = "0"          'ini�t�@�C���̎擾���s�����ꍇ�́A������Ȃ��B
            '    End If
            '    If HENKOU_PRINT_FLG = "1" Then
            '        '2011/06/29 �W���ŏC�� �a�������U�֕ύX�ʒm������I�� ------------------END

            '        ''------------------------------------------
            '        '' ���|�G�[�W�F���g���
            '        ''------------------------------------------
            '        Dim ExeRepo As New CAstReports.ClsExecute
            '        ' �a�������U�։��E�ύX�ʒm��
            '        Dim Keys() As String = CType(UpdateKeys(0), String())
            '        Dim Param As String = LW.UserID & "," & Keys(0).TrimEnd
            '        MainLOG.Write("�a�������U�֕ύX�ʒm������o�b�`�ďo", "����", Param)
            '        Dim nRet As Integer = ExeRepo.ExecReport("KFJP012.EXE", Param)
            '        Select Case nRet
            '            Case 0
            '                MainLOG.Write("�a�������U�֕ύX�ʒm�����", "����", "")
            '            Case -1
            '                MainLOG.Write("�a�������U�֕ύX�ʒm�����", "���s", "����ΏۂȂ�")
            '            Case Else
            '                MainLOG.Write("�a�������U�֕ύX�ʒm�����", "���s", "")
            '                MainLOG.UpdateJOBMASTbyErr("�a�������U�֕ύX�ʒm����� ���s")
            '                Return False
            '        End Select
            '        '2011/06/29 �W���ŏC�� �a�������U�֕ύX�ʒm������I�� ------------------START
            '    End If
            '    '2011/06/29 �W���ŏC�� �a�������U�֕ύX�ʒm������I�� ------------------END
            'End If
            '2017/03/13 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END
        Catch ex As Exception
            MainLOG.Write("���׃}�X�^�o�^����", "���s", ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("�V�X�e���G���[�i���O�Q�Ɓj")

            Return False
        Finally
            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

            If Not aReadFmt Is Nothing Then
                aReadFmt.Close()
                aReadFmt.Dispose()
            End If
        End Try

        Return True
    End Function

#End Region

#Region "SKC�p���׍X�V"
    ' �@�\�@ �F ���׃}�X�^�o�^����(SKC)
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateMeiMast(ByVal aReadFmt As CAstFormat.CFormatSKCFunou) As Boolean
        Dim dblRECORD_COUNT As Integer = 0
        Dim dblALL_KEN As Decimal
        Dim dblALL_KINGAKU As Decimal
        Dim dblKOUSIN_KEN As Decimal

        Dim dblKIGYO_RECORD_COUNT As Integer = 0
        Dim intMINASI_COUNT As Integer = 0

        Dim sCheckRet As String                         ' �`�F�b�N��������

        Dim SQL As StringBuilder = New StringBuilder(512)

        Dim UpdateKeys As New ArrayList(1000)

        Dim strTODAY As String = DateTime.Today.ToString("yyyyMMdd")

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***


        ' �S�f�[�^�`�F�b�N
        Try
            ' �N���p�����[�^���ʏ��
            aReadFmt.ToriData = mArgumentData
            Dim InfileInfo As New FileInfo(aReadFmt.FileName)
            If InfileInfo.Exists() = True AndAlso InfileInfo.Length = 0 Then
                ' ���ۂO�o�C�g�ł���
                MainLOG.Write("�s�\���ʌ����O������", "����")

                Dim Key(0) As String
                If mKoParam.CP.FURI_DATE Is Nothing OrElse mKoParam.CP.FURI_DATE.Trim = "" Then
                    ' �O�c�Ɠ�
                    Dim oFmt As New CAstFormat.CFormat
                    oFmt.Oracle = MainDB
                    Dim ZenDay As Date = CASTCommon.GetEigyobi(CASTCommon.Calendar.Now, -1, oFmt.HolidayList)
                    oFmt = Nothing
                    Key(0) = ZenDay.ToString("yyyyMMdd")
                Else
                    Key(0) = mKoParam.CP.FURI_DATE
                End If
                UpdateKeys.Add(Key)
            Else
                ' �s�\���ʃt�@�C���ǂݍ��݊J�n
                If aReadFmt.FirstRead() = 0 Then
                    MainLOG.Write("�t�@�C���I�[�v��", "���s", aReadFmt.Message)
                    MainLOG.UpdateJOBMASTbyErr("�t�@�C���I�[�v�����s")
                    Return False
                End If
            End If

            Dim stMei As CAstFormat.CFormat.MEISAI      ' ���׃}�X�^���

            Do Until aReadFmt.EOF
                ' �f�[�^��ǂݍ���ŁC�t�H�[�}�b�g�`�F�b�N���s��
                sCheckRet = aReadFmt.CheckKekkaFormat()

                dblRECORD_COUNT += 1

                If aReadFmt.IsDataRecord = True Then

                    stMei = aReadFmt.InfoMeisaiMast

                    MainLOG.FuriDate = stMei.FURIKAE_DATE

                    dblALL_KEN += 1
                    dblALL_KINGAKU += stMei.FURIKIN

                    '�s�\�R�[�h0�ȊO���X�V����
                    If stMei.FURIKETU_CODE <> 0 Then

                        Dim intCOUNT As Integer = 0
                        '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                        'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
                        OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                        '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                        SQL.Length = 0
                        SQL.Append("SELECT COUNT(*)")
                        SQL.Append(", MAX(TORIS_CODE_K || TORIF_CODE_K)")
                        SQL.Append(" FROM MEIMAST")
                        SQL.Append(" WHERE FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                        SQL.Append("   AND TRIM(KIGYO_CODE_K) = " & SQ(stMei.KIGYO_CODE))
                        If mKoParam.CP.MODE1 = "0" Then
                            ' �X�V�L�[����ƃV�[�P���X
                            SQL.Append("   AND KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
                        Else
                            ' �X�V�L�[���������
                            SQL.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                            SQL.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                            SQL.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                            SQL.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                            '�U�֋��z���l��
                            SQL.Append("   AND FURIKIN_K = " & CInt(stMei.FURIKIN))
                            '2010/01/19 �J�i�ϊ���̎��v�Ɣԍ���YOBI4_K�ɐݒ�
                            ' SQL.Append("   AND TRIM(JYUYOUKA_NO_K) = " & SQ(stMei.JYUYOKA_NO.Trim))
                            ' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- START
                            'SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                            Select Case mKoParam.CP.MODE1
                                Case "9"
                                    ' MODE1="9"�́A���v�Ɣԍ�����������͂����X�V����
                                Case Else
                                    SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                            End Select
                            ' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- END
                            '================================================
                        End If
                        If OraMeiReader.DataReader(SQL) = True Then
                            intCOUNT = OraMeiReader.GetValueInt(0)
                        End If

                        If intCOUNT = 0 Then
                            '�������ʂ�0���̏ꍇ�A���b�Z�[�W��\��
                            MainLOG.Write("���׌���", "���s", "�U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                            MainLOG.UpdateJOBMASTbyErr("���׌������s �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                            Return False
                        ElseIf intCOUNT > 1 Then
                            MainLOG.Write("���׌���", "���s", "����������ו������݁@�U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                            ' �ُ�I���ɂ��Ȃ�
                            'LOG.UpdateJOBMASTbyErr("���׌�������������ו������� �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                            mJobMessage = "���׌�������������ו������� �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ
                            MainLOG.UpdateJOBMAST(mJobMessage)
                        End If

                        '�X�V
                        SQL = New StringBuilder(128)
                        SQL.Append("UPDATE MEIMAST")
                        SQL.Append(" SET")
                        SQL.Append("  FURIKETU_CODE_K = " & stMei.FURIKETU_CODE.ToString)
                        SQL.Append(", FURIKETU_CENTERCODE_K = " & stMei.FURIKETU_CENTERCODE.ToString)
                        SQL.Append(", KOUSIN_DATE_K = " & SQ(strTODAY))
                        SQL.Append("WHERE FURI_DATE_K  = " & SQ(stMei.FURIKAE_DATE))
                        SQL.Append("  AND TRIM(KIGYO_CODE_K) = " & SQ(stMei.KIGYO_CODE))
                        If mKoParam.CP.MODE1 = "0" Then
                            ' �X�V�L�[����ƃV�[�P���X
                            SQL.Append("   AND KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
                        Else
                            ' �X�V�L�[���������
                            '���������������͌����������SEQ�̍ŏ��̂��̂̂ݍX�V����
                            SQL.Append("   AND KIGYO_SEQ_K = ")
                            SQL.Append("   (SELECT MIN(KIGYO_SEQ_K) FROM MEIMAST WHERE FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                            SQL.Append("   AND KIGYO_CODE_K = " & SQ(stMei.KIGYO_CODE))
                            SQL.Append("   AND FURIKETU_CODE_K = '0' ")
                            SQL.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                            SQL.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                            SQL.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                            SQL.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                            SQL.Append("   AND FURIKIN_K = " & stMei.FURIKIN)
                            '2010/01/19 �J�i�ϊ���̎��v�Ɣԍ���YOBI4_K�ɐݒ�
                            ' SQL.Append("   AND TRIM(JYUYOUKA_NO_K) = " & SQ(stMei.JYUYOKA_NO.Trim))
                            ' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- START
                            'SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                            Select Case mKoParam.CP.MODE1
                                Case "9"
                                    ' MODE1="9"�́A���v�Ɣԍ�����������͂����X�V����
                                Case Else
                                    SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                            End Select
                            ' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- END
                            '================================================
                            SQL.Append("   )")

                        End If

                        Try
                            MainDB.ExecuteNonQuery(SQL)
                            dblKOUSIN_KEN += 1
                        Catch ex As Exception
                            MainLOG.Write("���׍X�V", "���s", "�U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ & " " & ex.Message & ":" & ex.StackTrace)
                            MainLOG.UpdateJOBMASTbyErr("���׍X�V �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃR�[�h�F" & stMei.KIGYO_CODE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                            Return False
                        End Try

                        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                        OraMeiReader.Close()
                        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                    End If

                    ' �X�P�W���[���L�[���ۑ�
                    Dim Key(0) As String
                    Key(0) = stMei.FURIKAE_DATE

                    Dim oSearch As New mySearchClass
                    If UpdateKeys.BinarySearch(Key, oSearch) < 0 Then
                        If mKoParam.CP.FURI_DATE <> stMei.FURIKAE_DATE Then
                            MainLOG.Write("�U�֓��s��v", "���s", "���͐U�֓��F" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �f�[�^�U�֓��F" & ConvertDate(stMei.FURIKAE_DATE, "yyyy�NMM��dd��"))
                            MainLOG.UpdateJOBMASTbyErr("�U�֓��s��v ���͐U�֓��F" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �f�[�^�U�֓��F" & ConvertDate(stMei.FURIKAE_DATE, "yyyy�NMM��dd��"))
                            Return False
                        End If

                        UpdateKeys.Add(Key)
                    End If
                End If
            Loop

            For i As Integer = 0 To UpdateKeys.Count - 1

                Dim Keys() As String = CType(UpdateKeys(i), String())

                ' �X�P�W���[���X�V
                If UpdateSCHMAST(Keys, True) = False Then
                    Return False
                End If

            Next i

            '*** Str Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            'MainDB.Commit()
            '*** End Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            '�G���[���b�Z�[�W���Ȃ��ꍇ�����\��
            If mJobMessage = "" Then
                mJobMessage = "�Ǎ������F" & dblALL_KEN & " �X�V�����F" & dblKOUSIN_KEN & " �݂Ȃ��X�V����:" & CountMinasi
                MainLOG.Write("���ʍX�V", "����", mJobMessage)
            End If

        Catch ex As Exception
            MainLOG.Write("���׃}�X�^�o�^����", "���s", ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("�V�X�e���G���[�i���O�Q�Ɓj")
            Return False
        Finally
            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

            If Not aReadFmt Is Nothing Then
                aReadFmt.Close()
                aReadFmt.Dispose()
            End If
        End Try

        Return True
    End Function

#End Region

#Region " ���׃}�X�^�o�^�����i�r�r�r�j"
    ' �@�\�@ �F ���׃}�X�^�o�^�����i�r�r�r�j
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateMeiMastSSS(ByVal aReadFmt As CAstFormat.CFormatZengin) As Boolean
        Dim dblRECORD_COUNT As Integer
        Dim dblALL_KEN As Decimal
        Dim dblALL_KINGAKU As Decimal
        Dim dblKOUSIN_KEN As Decimal

        Dim dblKIGYO_RECORD_COUNT As Double = 0
        Dim intMINASI_COUNT As Integer = 0

        Dim sCheckRet As String                         ' �`�F�b�N��������

        Dim SQL As StringBuilder

        Dim UpdateKeys As New ArrayList(1000)

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***


        ' �S�f�[�^�`�F�b�N
        Try
            ' �N���p�����[�^���ʏ��
            aReadFmt.ToriData = mArgumentData
            If aReadFmt.FirstRead() = 0 Then
                MainLOG.Write("�t�@�C���I�[�v��", "���s", aReadFmt.Message)
                MainLOG.UpdateJOBMASTbyErr("�t�@�C���I�[�v�����s")
                Return False
            End If

            Dim stTori As CAstBatch.CommData.stTORIMAST ' �������
            Dim stMei As CAstFormat.CFormat.MEISAI      ' ���׃}�X�^���
            Do Until aReadFmt.EOF
                ' �f�[�^��ǂݍ���ŁC�t�H�[�}�b�g�`�F�b�N���s��
                sCheckRet = aReadFmt.CheckKekkaFormatSSS()
                Select Case sCheckRet
                    Case "ERR"
                        ' �t�H�[�}�b�g�G���[
                        MainLOG.Write("�t�H�[�}�b�g�G���[", "���s", (dblRECORD_COUNT + 1).ToString & "�s�� " & aReadFmt.Message)
                        MainLOG.UpdateJOBMASTbyErr((dblRECORD_COUNT + 1).ToString & "�s�� " & aReadFmt.Message)
                        Return False
                        Exit Do
                    Case "IJO"
                        MainLOG.Write("�t�H�[�}�b�g�G���[", "���s", aReadFmt.Message)
                        MainLOG.UpdateJOBMASTbyErr(aReadFmt.Message)
                        Return False
                    Case "H"
                        MainLOG.Write("�t�@�C���w�b�_�f�[�^�Z�b�g", "����")
                    Case "D"
                    Case "T"
                        MainLOG.Write("�t�@�C���g���[���f�[�^�Z�b�g", "����")
                    Case ""
                        Exit Do
                End Select

                dblRECORD_COUNT += 1

                stMei = aReadFmt.InfoMeisaiMast
                If aReadFmt.IsHeaderRecord = True Then
                    ' �w�b�_

                    If mKoParam.CP.FURI_DATE <> stMei.FURIKAE_DATE Then
                        MainLOG.Write("�U�֓��s��v", "���s", "���͐U�֓��F" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �f�[�^�U�֓��F" & ConvertDate(stMei.FURIKAE_DATE, "yyyy�NMM��dd��"))
                        MainLOG.UpdateJOBMASTbyErr("�U�֓��s��v ���͐U�֓��F" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �f�[�^�U�֓��F" & ConvertDate(stMei.FURIKAE_DATE, "yyyy�NMM��dd��"))
                        Return False
                    End If

                    If aReadFmt.GetTorimastFromItakuCodeSSS(MainDB) = False Then
                        MainLOG.Write("�����}�X�^����", "���s", "�ϑ��҃R�[�h:" & stMei.ITAKU_CODE & " " & aReadFmt.Message)
                        MainLOG.UpdateJOBMASTbyErr("�����}�X�^�������s �ϑ��҃R�[�h:" & stMei.ITAKU_CODE)
                        Return False
                    End If
                ElseIf aReadFmt.IsDataRecord = True Then
                    stTori = aReadFmt.ToriData.INFOToriMast

                    dblALL_KEN += 1
                    dblALL_KINGAKU += stMei.FURIKIN

                    Dim intCOUNT As Integer = 0
                    '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                    'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
                    OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                    '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT COUNT(*)")
                    SQL.Append(", MAX(TORIS_CODE_K)")
                    SQL.Append(", MAX(TORIF_CODE_K)")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE TORIS_CODE_K =  " & SQ(stTori.TORIS_CODE_T))
                    SQL.Append("   AND TORIF_CODE_K =  " & SQ(stTori.TORIF_CODE_T))
                    SQL.Append("   AND FURI_DATE_K  =  " & SQ(stMei.FURIKAE_DATE))
                    SQL.Append("   AND DATA_KBN_K   =  '2'")
                    If mKoParam.CP.MODE1 = "0" Then
                        ' �X�V�L�[����ƃV�[�P���X
                        SQL.Append(" AND KIGYO_SEQ_K = " & SQ(CADec(aReadFmt.ZENGIN_REC2.ZG15).ToString))
                    Else
                        ' �X�V�L�[���������
                        SQL.Append(" AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                        SQL.Append(" AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                        ' 2017/11/10 �^�X�N�j���� CHG (�W���ŕs��Ή�(��168)) -------------------- START
                        SQL.Append(" AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
                        'SQL.Append(" AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                        ' 2017/11/10 �^�X�N�j���� CHG (�W���ŕs��Ή�(��168)) -------------------- END
                        SQL.Append(" AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                        SQL.Append(" AND FURIKIN_K = " & stMei.FURIKIN)
                        ' 2017/11/10 �^�X�N�j���� DEL (�W���ŕs��Ή�(��168)) -------------------- START
                        'SSS�ł��g�p���Ă��Ȃ��̂ŏ����ɓ���Ȃ�
                        '' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- START
                        ''SQL.Append(" AND SUBSTR(JYUYOUKA_NO_K,1,11) = " & SQ(stMei.JYUYOKA_NO.Substring(9, 11)))
                        'Select Case mKoParam.CP.MODE1
                        '    Case "9"
                        '        ' MODE1="9"�́A���v�Ɣԍ�����������͂����X�V����
                        '    Case Else
                        '        SQL.Append(" AND SUBSTR(JYUYOUKA_NO_K,1,11) = " & SQ(stMei.JYUYOKA_NO.Substring(9, 11)))
                        'End Select
                        '' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- END
                        ' 2017/11/10 �^�X�N�j���� DEL (�W���ŕs��Ή�(��168)) -------------------- END
                    End If

                    If OraMeiReader.DataReader(SQL) = True Then
                        intCOUNT = OraMeiReader.GetValueInt(0)
                    End If

                    If intCOUNT = 0 Then
                        '�������ʂ�0���̏ꍇ�A���b�Z�[�W��\��
                        ' 2017/11/10 �^�X�N�j���� CHG (�W���ŕs��Ή�(��168)) -------------------- START
                        '���O�o�͌�����
                        If mKoParam.CP.MODE1 = "0" Then

                            MainLOG.Write("���׌���(��ƃV�[�P���X)", "���s", "�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                          " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                            MainLOG.UpdateJOBMASTbyErr("���׌������s(��ƃV�[�P���X) �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                                       " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                        Else
                            MainLOG.Write("���׌���(�������)", "���s", "�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                          " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU) & _
                                          " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN)
                            MainLOG.UpdateJOBMASTbyErr("���׌���(�������) �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                          " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU) & _
                                          " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN)
                        End If
                        'MainLOG.Write("���׌���", "���s", "�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                        'MainLOG.UpdateJOBMASTbyErr("���׌������s �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                        ' 2017/11/10 �^�X�N�j���� CHG (�W���ŕs��Ή�(��168)) -------------------- END

                        Return False
                    ElseIf intCOUNT > 1 Then
                        ' 2017/11/10 �^�X�N�j���� CHG (�W���ŕs��Ή�(��168)) -------------------- START
                        '���O�o�͌�����
                        If mKoParam.CP.MODE1 = "0" Then
                            MainLOG.Write("���׌���(��ƃV�[�P���X)", "���s", "����������ו������݁@�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & _
                                          " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                            MainLOG.UpdateJOBMASTbyErr("���׌���(��ƃV�[�P���X) ����������ו������� �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & _
                                                       " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                        Else
                            MainLOG.Write("���׌���(�������)", "���s", "����������ו������� �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & _
                                          " �U�֓��F" & stMei.FURIKAE_DATE & " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & _
                                          " �ȖځF" & CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU) & " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN)
                            MainLOG.UpdateJOBMASTbyErr("���׌���(�������) ����������ו������� �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & _
                                                       " �U�֓��F" & stMei.FURIKAE_DATE & " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & _
                                                       " �ȖځF" & CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU) & " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN)
                        End If
                        'MainLOG.Write("���׌���", "���s", "����������ו������݁@�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                        'MainLOG.UpdateJOBMASTbyErr("���׌�������������ו������� �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                        ' 2017/11/10 �^�X�N�j���� CHG (�W���ŕs��Ή�(��168)) -------------------- END

                    End If

                    '�X�V
                    SQL = New StringBuilder(128)
                    SQL.Append("UPDATE MEIMAST ")
                    SQL.Append(" SET FURIKETU_CODE_K = " & stMei.FURIKETU_MOTO)
                    SQL.Append(",FURIKETU_CENTERCODE_K = " & stMei.FURIKETU_MOTO)
                    SQL.Append(",KOUSIN_DATE_K = " & SQ(DateTime.Today.ToString("yyyyMMdd")))
                    SQL.Append(" WHERE FURI_DATE_K  = " & SQ(stMei.FURIKAE_DATE))
                    SQL.Append("   AND TORIS_CODE_K = " & SQ(stTori.TORIS_CODE_T))
                    SQL.Append("   AND TORIF_CODE_K = " & SQ(stTori.TORIF_CODE_T))
                    If mKoParam.CP.MODE1 = "0" Then
                        ' �X�V�L�[����ƃV�[�P���X
                        SQL.Append(" AND KIGYO_SEQ_K = " & SQ(CADec(aReadFmt.ZENGIN_REC2.ZG15).ToString))
                    Else
                        ' �X�V�L�[���������
                        SQL.Append(" AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                        SQL.Append(" AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                        ' 2017/11/10 �^�X�N�j���� CHG (�W���ŕs��Ή�(��168)) -------------------- START
                        SQL.Append(" AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
                        'SQL.Append(" AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                        ' 2017/11/10 �^�X�N�j���� CHG (�W���ŕs��Ή�(��168)) -------------------- END
                        SQL.Append(" AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                        SQL.Append(" AND FURIKIN_K = " & stMei.FURIKIN)
                        ' 2017/11/10 �^�X�N�j���� DEL (�W���ŕs��Ή�(��168)) -------------------- START
                        'SSS�ł��g�p���Ă��Ȃ��̂ŏ����ɓ���Ȃ�
                        '' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- START
                        ''SQL.Append(" AND SUBSTR(JYUYOUKA_NO_K,1,11) = " & SQ(stMei.JYUYOKA_NO.Substring(9, 11)))
                        'Select Case mKoParam.CP.MODE1
                        '    Case "9"
                        '        ' MODE1="9"�́A���v�Ɣԍ�����������͂����X�V����
                        '    Case Else
                        '        SQL.Append(" AND SUBSTR(JYUYOUKA_NO_K,1,11) = " & SQ(stMei.JYUYOKA_NO.Substring(9, 11)))
                        'End Select
                        '' 2017/01/26 �^�X�N�j���� CHG �yOT�zUI_B-99(RSV2�Ή�(�W���ŋ@�\�ǉ�)) -------------------- END
                        ' 2017/11/10 �^�X�N�j���� DEL (�W���ŕs��Ή�(��168)) -------------------- END
                    End If

                    Try
                        MainDB.ExecuteNonQuery(SQL)
                        dblKOUSIN_KEN += 1
                    Catch ex As Exception
                        ' 2017/11/10 �^�X�N�j���� CHG (�W���ŕs��Ή�(��168)) -------------------- START
                        '���O�o�͌�����
                        If mKoParam.CP.MODE1 = "0" Then
                            MainLOG.Write("���׍X�V(��ƃV�[�P���X)", "���s", "�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                          " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ & " " & ex.Message & ":" & ex.StackTrace)
                            MainLOG.UpdateJOBMASTbyErr("���׍X�V(��ƃV�[�P���X) �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                                       " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                        Else
                            MainLOG.Write("���׍X�V(�������)", "���s", "�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                          " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU) & _
                                          " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN)
                            MainLOG.UpdateJOBMASTbyErr("���׍X�V(�������) �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                                       " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU) & _
                                                       " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN)
                        End If
                        'MainLOG.Write("���׍X�V", "���s", "�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ & " " & ex.Message & ":" & ex.StackTrace)
                        'MainLOG.UpdateJOBMASTbyErr("���׍X�V �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                        ' 2017/11/10 �^�X�N�j���� CHG (�W���ŕs��Ή�(��168)) -------------------- END
                        Return False
                    End Try

                    OraMeiReader.Close()

                ElseIf aReadFmt.IsTrailerRecord = True Then
                    ' �X�P�W���[���L�[���ۑ�
                    stTori = aReadFmt.ToriData.INFOToriMast
                    Dim Key(4) As String
                    Key(0) = stTori.TORIS_CODE_T
                    Key(1) = stTori.TORIF_CODE_T
                    Key(2) = stMei.FURIKAE_DATE
                    Key(3) = Jikinko
                    Key(4) = stMei.ITAKU_CODE.PadRight(10, " "c).Substring(9, 1)

                    Dim oSearch As New mySearchClass
                    If UpdateKeys.BinarySearch(Key, oSearch) < 0 Then
                        UpdateKeys.Add(Key)
                    End If

                End If
NEXT_RECORD:
            Loop

            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                ' ���s�X�P�W���[���X�V
                If UpdateTAKOSCHMAST(Keys) = False Then
                    Return False
                End If
            Next i

            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                ' �X�P�W���[���X�V
                If UpdateSCHMAST(Keys, False) = False Then
                    Return False
                End If
            Next i

            '2017/01/20 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
            '�������ʊm�F�\�̓X�P�W���[���X�V�ň�������A�ʂɈ������B
            '�i�s�\���ʍX�V�搔���������邽�߁j
            If Me.PrintSyoriKekkaKakuninhyoForSSS(UpdateKeys) = False Then
                Return False
            End If
            '2017/01/20 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END

            '*** Str Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            'MainDB.Commit()
            '*** End Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            ' �萔���v�Z
            '�����łȂ��U�֓��L�[�Ŏ萔���v�Z����
            '2017/01/18 saitou ���t�M��(RSV2�W��) DEL �X���[�G�X�Ή� ---------------------------------------- START
            '���̎萔���v�Z�͎g���Ȃ�
            'Dim ht As Hashtable = New Hashtable
            'For i As Integer = 0 To UpdateKeys.Count - 1
            '    Dim Keys() As String = CType(UpdateKeys(i), String())

            '    '�����łȂ��U�֓��L�[�Ŏ萔���v�Z����
            '    If ht.ContainsKey(Keys(2)) = False Then
            '        Call CalcTesuuExecute(Keys(2))
            '        ht.Add(Keys(2), Nothing)
            '    End If
            'Next i
            '2017/01/18 saitou ���t�M��(RSV2�W��) DEL ------------------------------------------------------- END

        Catch ex As Exception
            MainLOG.Write("���׃}�X�^�o�^����(SSS)", "���s", ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("�V�X�e���G���[�i���O�Q�Ɓj")

            Return False
        Finally
            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

        End Try

        Return True
    End Function
#End Region

#Region " ���׃}�X�^�o�^�����i��Ǝ����j"
    ' �@�\�@ �F ���׃}�X�^�o�^�����i��Ǝ����j
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateMeiMastKigyo(ByVal aReadFmt As CAstFormat.CFormatZengin) As Boolean
        Dim dblRECORD_COUNT As Integer
        Dim dblALL_KEN As Decimal
        Dim dblALL_KINGAKU As Decimal
        Dim dblKOUSIN_KEN As Decimal

        Dim dblKIGYO_RECORD_COUNT As Double = 0
        Dim intMINASI_COUNT As Integer = 0

        Dim sCheckRet As String                         ' �`�F�b�N��������

        Dim SQL As StringBuilder

        Dim UpdateKeys As New ArrayList(1000)
        Dim RecordNo As String = "0"
        Dim RecordHash As New Hashtable

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***


        ' �S�f�[�^�`�F�b�N
        Try

            ' �N���p�����[�^���ʏ��
            aReadFmt.ToriData = mArgumentData
            If aReadFmt.FirstRead() = 0 Then
                MainLOG.Write("�t�@�C���I�[�v��", "���s", aReadFmt.Message)
                MainLOG.UpdateJOBMASTbyErr("�t�@�C���I�[�v�����s")
                Return False
            End If
            Dim stTori As CAstBatch.CommData.stTORIMAST ' �������
            Dim stMei As CAstFormat.CFormat.MEISAI      ' ���׃}�X�^���
            Do Until aReadFmt.EOF
                ' �f�[�^��ǂݍ���ŁC�t�H�[�}�b�g�`�F�b�N���s��
                sCheckRet = aReadFmt.CheckKekkaFormat()
                Select Case sCheckRet
                    Case "ERR"
                        ' �t�H�[�}�b�g�G���[
                        MainLOG.Write("�t�H�[�}�b�g�G���[", "���s", (dblRECORD_COUNT + 1).ToString & "�s�� " & aReadFmt.Message)
                        MainLOG.UpdateJOBMASTbyErr((dblRECORD_COUNT + 1).ToString & "�s�� " & aReadFmt.Message)
                        Return False
                        Exit Do
                    Case "IJO"
                        MainLOG.Write("�t�H�[�}�b�g�G���[", "���s", aReadFmt.Message)
                        MainLOG.UpdateJOBMASTbyErr(aReadFmt.Message)
                        Return False
                    Case "H"
                        MainLOG.Write("�t�@�C���w�b�_�f�[�^�Z�b�g", "����")
                    Case "D"
                    Case "T"
                        MainLOG.Write("�t�@�C���g���[���f�[�^�Z�b�g", "����")
                    Case ""
                        Exit Do
                End Select

                dblRECORD_COUNT += 1

                Dim SQLWhere As New StringBuilder(128)

                stMei = aReadFmt.InfoMeisaiMast
                If aReadFmt.IsHeaderRecord = True Then
                    ' �w�b�_
                    If aReadFmt.GetTorimastFromItakuCodeKigyo(MainDB) = False Then
                        MainLOG.Write("�����}�X�^����", "���s", "�ϑ��҃R�[�h:" & stMei.ITAKU_CODE & " " & aReadFmt.Message)
                        MainLOG.UpdateJOBMASTbyErr("�����}�X�^�������s �ϑ��҃R�[�h:" & stMei.ITAKU_CODE)
                        Return False
                    End If

                    ' �X�P�W���[���L�[���ۑ�
                    stTori = aReadFmt.ToriData.INFOToriMast
                    Dim Key(2) As String
                    Key(0) = stTori.TORIS_CODE_T
                    Key(1) = stTori.TORIF_CODE_T
                    Key(2) = stMei.FURIKAE_DATE

                    Dim oSearch As New mySearchClass
                    If UpdateKeys.BinarySearch(Key, oSearch) < 0 Then
                        If mKoParam.CP.FURI_DATE <> stMei.FURIKAE_DATE Then
                            MainLOG.Write("�U�֓��s��v", "���s", "���͐U�֓��F" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �f�[�^�U�֓��F" & ConvertDate(stMei.FURIKAE_DATE, "yyyy�NMM��dd��"))
                            MainLOG.UpdateJOBMASTbyErr("�U�֓��s��v ���͐U�֓��F" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �f�[�^�U�֓��F" & ConvertDate(stMei.FURIKAE_DATE, "yyyy�NMM��dd��"))
                            Return False
                        End If

                        UpdateKeys.Add(Key)
                    End If

                ElseIf aReadFmt.IsDataRecord = True Then
                    stTori = aReadFmt.ToriData.INFOToriMast

                    dblALL_KEN += 1
                    dblALL_KINGAKU += stMei.FURIKIN

                    Dim intCOUNT As Integer = 0
                    '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                    'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
                    OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                    '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT COUNT(*)")
                    SQL.Append(", MAX(TORIS_CODE_K)")
                    SQL.Append(", MAX(TORIF_CODE_K)")
                    SQL.Append(", MIN(RECORD_NO_K) RECORD_NO")  '2009/12/18 �ǉ�
                    SQL.Append(" FROM MEIMAST")

                    SQLWhere = New StringBuilder(128)
                    SQLWhere.Append(" WHERE TORIS_CODE_K = " & SQ(stTori.TORIS_CODE_T))
                    If stTori.BAITAI_CODE_T = "07" Then
                        ' �w�Z���U�̏ꍇ�C���R�[�h���L�[�Ɋ܂܂Ȃ�
                    Else
                        SQLWhere.Append("   AND TORIF_CODE_K = " & SQ(stTori.TORIF_CODE_T))
                    End If
                    SQLWhere.Append("   AND FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                    SQLWhere.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                    SQLWhere.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                    SQLWhere.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
                    SQLWhere.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                    SQLWhere.Append("   AND FURIKIN_K = " & stMei.FURIKIN_MOTO)
                    '���v�Ɣԍ���FURI_DATA�Ɣ�r����
                    SQLWhere.Append("   AND SUBSTR(FURI_DATA_K,92,20) = " & SQ(stMei.JYUYOKA_NO))
                    SQLWhere.Append("   AND DATA_KBN_K   =  '2'")
                    Dim sSQL As String = SQL.ToString & SQLWhere.ToString
                    If OraMeiReader.DataReader(sSQL) = True Then
                        intCOUNT = CASTCommon.CAInt32(OraMeiReader.GetValue(0).ToString)
                    End If

                    If RecordHash.Contains(stTori.TORIS_CODE_T & stTori.TORIF_CODE_T) Then
                        RecordNo = RecordHash.Item(stTori.TORIS_CODE_T & stTori.TORIF_CODE_T).ToString
                    Else
                        RecordNo = "0"
                    End If

                    If intCOUNT > 1 Then
                        OraMeiReader.Close()
                        ' ���������L��C�ʂ̃L�[�ŃA�N�Z�X
                        SQLWhere = New StringBuilder(128)
                        SQLWhere.Append(" WHERE TORIS_CODE_K =  " & SQ(stTori.TORIS_CODE_T))
                        SQLWhere.Append("   AND TORIF_CODE_K =  " & SQ(stTori.TORIF_CODE_T))
                        SQLWhere.Append("   AND FURI_DATE_K  =  " & SQ(stMei.FURIKAE_DATE))
                        SQLWhere.Append("   AND DATA_KBN_K   =  '2'")
                        SQLWhere.Append("   AND RECORD_NO_K  =  (SELECT MIN(RECORD_NO_K) ")
                        SQLWhere.Append(" FROM MEIMAST")
                        SQLWhere.Append(" WHERE TORIS_CODE_K =  " & SQ(stTori.TORIS_CODE_T))
                        SQLWhere.Append("   AND TORIF_CODE_K =  " & SQ(stTori.TORIF_CODE_T))
                        SQLWhere.Append("   AND FURI_DATE_K  =  " & SQ(stMei.FURIKAE_DATE))
                        SQLWhere.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                        SQLWhere.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                        SQLWhere.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
                        SQLWhere.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                        SQLWhere.Append("   AND FURIKIN_K = " & stMei.FURIKIN_MOTO)
                        '���v�Ɣԍ���FURI_DATA�Ɣ�r����
                        SQLWhere.Append("   AND SUBSTR(FURI_DATA_K,92,20) = " & SQ(stMei.JYUYOKA_NO))
                        SQLWhere.Append("   AND DATA_KBN_K   =  '2'")
                        SQLWhere.Append("   AND RECORD_NO_K NOT IN(" & RecordNo & ")")
                        SQLWhere.Append("   )")
                        sSQL = SQL.ToString & SQLWhere.ToString
                        If OraMeiReader.DataReader(sSQL) = True Then
                            intCOUNT = OraMeiReader.GetValueInt(0)
                        End If
                    End If

                    If intCOUNT = 0 Then
                        '�������ʂ�0���̏ꍇ�A���b�Z�[�W��\��
                        MainLOG.Write("���׌���", "���s", "�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                      " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & stMei.KEIYAKU_KAMOKU & _
                                      " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN_MOTO)
                        MainLOG.UpdateJOBMASTbyErr("���׌��� �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                      " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & stMei.KEIYAKU_KAMOKU & _
                                      " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN_MOTO)
                        Return False
                    ElseIf intCOUNT > 1 Then
                        MainLOG.Write("���׌���", "���s", "����������ו������� �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                                                                        " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & stMei.KEIYAKU_KAMOKU & _
                                                                                        " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN_MOTO)
                        MainLOG.UpdateJOBMASTbyErr("���׌�������������ו������� �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                      " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & stMei.KEIYAKU_KAMOKU & _
                                      " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN_MOTO)
                        Return False
                    End If

                    '�X�V
                    SQL = New StringBuilder(128)
                    SQL.Append("UPDATE MEIMAST ")
                    SQL.Append(" SET FURIKETU_CODE_K = " & stMei.FURIKETU_MOTO)
                    SQL.Append(",FURIKETU_CENTERCODE_K = " & stMei.FURIKETU_MOTO)
                    SQL.Append(",KOUSIN_DATE_K = " & SQ(DateTime.Today.ToString("yyyyMMdd")))
                    RecordNo &= "," & OraMeiReader.GetInt("RECORD_NO")
                    If RecordHash.Contains(stTori.TORIS_CODE_T & stTori.TORIF_CODE_T) Then
                        RecordHash.Item(stTori.TORIS_CODE_T & stTori.TORIF_CODE_T) = RecordNo
                    Else
                        RecordHash.Add(stTori.TORIS_CODE_T & stTori.TORIF_CODE_T, RecordNo)
                    End If
                    Try
                        MainDB.ExecuteNonQuery(SQL.Append(SQLWhere))
                        dblKOUSIN_KEN += 1
                    Catch ex As Exception
                        MainLOG.Write("���׍X�V", "���s", "�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                                                  " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & stMei.KEIYAKU_KAMOKU & _
                                                                  " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN_MOTO & " " & ex.Message & ":" & ex.StackTrace)
                        MainLOG.UpdateJOBMASTbyErr("���׍X�V �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                      " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & stMei.KEIYAKU_KAMOKU & _
                                      " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN_MOTO)
                        Return False
                    End Try

                    OraMeiReader.Close()
                End If
NEXT_RECORD:
            Loop


            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                ' �X�P�W���[���X�V
                If UpdateSCHMAST(Keys, True) = False Then
                    Return False
                End If
            Next i

            '*** Str Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            'MainDB.Commit()
            '*** End Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***

        Catch ex As Exception
            MainLOG.Write("���׃}�X�^�o�^����", "���s", ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("�V�X�e���G���[�i���O�Q�Ɓj")

            Return False
        Finally
            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

        End Try

        Return True
    End Function

#End Region

#Region " ���׃}�X�^�o�^�����i���s�j"
    ' �@�\�@ �F ���׃}�X�^�o�^�����i���s�j
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateMeiMastTako(ByVal aReadFmt As CAstFormat.CFormatZengin) As Boolean
        Dim dblRECORD_COUNT As Integer
        Dim dblALL_KEN As Decimal
        Dim dblALL_KINGAKU As Decimal
        Dim dblKOUSIN_KEN As Decimal

        Dim dblKIGYO_RECORD_COUNT As Double = 0
        Dim intMINASI_COUNT As Integer = 0

        Dim sCheckRet As String                         ' �`�F�b�N��������

        Dim SQL As StringBuilder

        Dim UpdateKeys As New ArrayList(1000)
        Dim RecordNo As String = "0"

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***


        ' �S�f�[�^�`�F�b�N
        Try
            '�����X�V�̏ꍇ�܂��͔}�̂��˗����̏ꍇ�̓X�P�W���[���X�V�̂�
            If mKoParam.KOUSIN_KBN = "1" OrElse strTAKO_BAITAI_CODE = "04" Then
                Dim Key(3) As String
                '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                'Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
                OraReader = New CASTCommon.MyOracleReader(MainDB)
                '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                Key(0) = mKoParam.CP.TORIS_CODE
                Key(1) = mKoParam.CP.TORIF_CODE
                Key(2) = mKoParam.CP.FURI_DATE
                Key(3) = mKoParam.TKIN_CODE

                '�L�[���ڂ��}�X�^�o�^����Ă���ꍇ�A�}�X�^�X�V���s��
                SQL = New StringBuilder(128)
                SQL.Append(" SELECT COUNT(*) COUNTER")
                SQL.Append(" FROM   TAKOSCHMAST ")
                SQL.Append(" WHERE  TORIS_CODE_U = " & SQ(Key(0)))
                SQL.Append(" AND    TORIF_CODE_U = " & SQ(Key(1)))
                SQL.Append(" AND    FURI_DATE_U  = " & SQ(Key(2)))
                SQL.Append(" AND    TKIN_NO_U    = " & SQ(Key(3)))

                If OraReader.DataReader(SQL) = True Then
                    If OraReader.GetInt("COUNTER") = 0 Then
                        MainLOG.Write("���s�X�P�W���[���X�V", "���s", " ���s�X�P�W���[���}�X�^�����F ����� " & Key(0) & "-" & Key(1) & _
                                                                             " �U�֓� " & ConvertDate(Key(2), "yyyy�NMM��dd��") & " ���Z�@��" & Key(3))

                        MainLOG.UpdateJOBMASTbyErr("���s�X�P�W���[���X�V���s�F ����� " & Key(0) & "-" & Key(1) & _
                                                   " �U�֓� " & ConvertDate(Key(2), "yyyy�NMM��dd��") & " ���Z�@��" & Key(3))
                        Return False
                    End If

                Else
                    If mKoParam.KOUSIN_KBN = "1" Then
                        MainLOG.Write("���s�X�P�W���[���X�V�X�V", "���s", " ���s�X�P�W���[���}�X�^�����F ����� " & Key(0) & "-" & Key(1) & _
                                      " �U�֓� " & ConvertDate(Key(2), "yyyy�NMM��dd��") & " ���Z�@��" & Key(3))
                        MainLOG.UpdateJOBMASTbyErr("�����X�V���s�F ����� " & Key(0) & "-" & Key(1) & _
                                                   " �U�֓� " & ConvertDate(Key(2), "yyyy�NMM��dd��") & " ���Z�@��" & Key(3))
                    Else
                        MainLOG.Write("���s�X�P�W���[���X�V�X�V", "���s", " ���s�X�P�W���[���}�X�^�����F ����� " & Key(0) & "-" & Key(1) & _
                                                      " �U�֓� " & ConvertDate(Key(2), "yyyy�NMM��dd��") & " ���Z�@��" & Key(3))
                        MainLOG.UpdateJOBMASTbyErr("�˗����X�V���s�F ����� " & Key(0) & "-" & Key(1) & _
                                                   " �U�֓� " & ConvertDate(Key(2), "yyyy�NMM��dd��") & " ���Z�@��" & Key(3))
                    End If
                    Return False
                End If

                UpdateKeys.Add(Key)

                '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                OraReader.Close()
                '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            Else
                ' �N���p�����[�^���ʏ��
                aReadFmt.ToriData = mArgumentData
                If aReadFmt.FirstRead() = 0 Then
                    MainLOG.Write("�t�@�C���I�[�v��", "���s", aReadFmt.Message)
                    MainLOG.UpdateJOBMASTbyErr("�t�@�C���I�[�v�����s")
                    Return False
                End If
                Dim stTori As CAstBatch.CommData.stTORIMAST ' �������
                Dim stMei As CAstFormat.CFormat.MEISAI      ' ���׃}�X�^���
                Do Until aReadFmt.EOF
                    ' �f�[�^��ǂݍ���ŁC�t�H�[�}�b�g�`�F�b�N���s��
                    sCheckRet = aReadFmt.CheckKekkaFormat()
                    Select Case sCheckRet
                        Case "ERR"
                            ' �t�H�[�}�b�g�G���[
                            MainLOG.Write("�t�H�[�}�b�g�G���[", "���s", (dblRECORD_COUNT + 1).ToString & "�s�� " & aReadFmt.Message)
                            MainLOG.UpdateJOBMASTbyErr((dblRECORD_COUNT + 1).ToString & "�s�� " & aReadFmt.Message)
                            Return False
                            Exit Do
                        Case "IJO"
                            MainLOG.Write("�t�H�[�}�b�g�G���[", "���s", aReadFmt.Message)
                            MainLOG.UpdateJOBMASTbyErr(aReadFmt.Message)
                            Return False
                        Case "H"
                            MainLOG.Write("�t�@�C���w�b�_�f�[�^�Z�b�g", "����")
                        Case "D"
                        Case "T"
                            MainLOG.Write("�t�@�C���g���[���f�[�^�Z�b�g", "����")
                        Case ""
                            Exit Do
                    End Select

                    dblRECORD_COUNT += 1

                    Dim SQLWhere As New StringBuilder(128)

                    stMei = aReadFmt.InfoMeisaiMast
                    If aReadFmt.IsHeaderRecord = True Then
                        ' �w�b�_
                        If aReadFmt.GetTorimastFromItakuCodeTAKO(MainDB) = False Then
                            MainLOG.Write("���s�}�X�^����", "���s", "�ϑ��҃R�[�h:" & stMei.ITAKU_CODE & " " & aReadFmt.Message)
                            MainLOG.UpdateJOBMASTbyErr("���s�}�X�^�������s �ϑ��҃R�[�h:" & stMei.ITAKU_CODE)
                            Return False
                        End If

                        ' �X�P�W���[���L�[���ۑ�
                        stTori = aReadFmt.ToriData.INFOToriMast
                        If mKoParam.CP.FURI_DATE <> stMei.FURIKAE_DATE Then
                            MainLOG.Write("�U�֓��s��v", "���s", "���͐U�֓��F" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �f�[�^�U�֓��F" & ConvertDate(stMei.FURIKAE_DATE, "yyyy�NMM��dd��"))
                            MainLOG.UpdateJOBMASTbyErr("�U�֓��s��v ���͐U�֓��F" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �f�[�^�U�֓��F" & ConvertDate(stMei.FURIKAE_DATE, "yyyy�NMM��dd��"))
                            Return False
                        End If

                    ElseIf aReadFmt.IsDataRecord = True Then
                        stTori = aReadFmt.ToriData.INFOToriMast

                        dblALL_KEN += 1
                        dblALL_KINGAKU += stMei.FURIKIN

                        Dim intCOUNT As Integer = 0
                        '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                        'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
                        OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                        '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                        SQL = New StringBuilder(128)
                        SQL.Append("SELECT COUNT(*)")
                        SQL.Append(", MAX(TORIS_CODE_K)")
                        SQL.Append(", MAX(TORIF_CODE_K)")
                        SQL.Append(", MIN(RECORD_NO_K) RECORD_NO")  '2009/12/18 �ǉ�
                        SQL.Append(" FROM MEIMAST")

                        SQLWhere = New StringBuilder(128)
                        SQLWhere.Append(" WHERE TORIS_CODE_K = " & SQ(stTori.TORIS_CODE_T))
                        If stTori.BAITAI_CODE_T = "07" Then
                            ' �w�Z���U�̏ꍇ�C���R�[�h���L�[�Ɋ܂܂Ȃ�
                        Else
                            SQLWhere.Append("   AND TORIF_CODE_K = " & SQ(stTori.TORIF_CODE_T))
                        End If
                        SQLWhere.Append("   AND FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                        SQLWhere.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                        SQLWhere.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                        SQLWhere.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
                        SQLWhere.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                        SQLWhere.Append("   AND FURIKIN_K = " & stMei.FURIKIN_MOTO)
                        '���v�Ɣԍ���FURI_DATA�Ɣ�r����
                        SQLWhere.Append("   AND SUBSTR(FURI_DATA_K,92,20) = " & SQ(stMei.JYUYOKA_NO))
                        SQLWhere.Append("   AND DATA_KBN_K   =  '2'")
                        If TAKO_SEQ <> "0" Then
                            SQLWhere.Append("   AND KIGYO_SEQ_K  =  " & SQ(CADec(aReadFmt.ZENGIN_REC2.ZG15).ToString))
                        End If
                        Dim sSQL As String = SQL.ToString & SQLWhere.ToString
                        If OraMeiReader.DataReader(sSQL) = True Then
                            intCOUNT = CASTCommon.CAInt32(OraMeiReader.GetValue(0).ToString)
                        End If


                        If intCOUNT > 1 Then
                            OraMeiReader.Close()
                            ' ���������L��C�ʂ̃L�[�ŃA�N�Z�X
                            SQLWhere = New StringBuilder(128)
                            SQLWhere.Append(" WHERE TORIS_CODE_K =  " & SQ(stTori.TORIS_CODE_T))
                            SQLWhere.Append("   AND TORIF_CODE_K =  " & SQ(stTori.TORIF_CODE_T))
                            SQLWhere.Append("   AND FURI_DATE_K  =  " & SQ(stMei.FURIKAE_DATE))
                            SQLWhere.Append("   AND DATA_KBN_K   =  '2'")
                            If TAKO_SEQ <> "0" Then
                                SQLWhere.Append("   AND KIGYO_SEQ_K  =  " & SQ(CADec(aReadFmt.ZENGIN_REC2.ZG15).ToString))
                            Else
                                SQLWhere.Append("   AND RECORD_NO_K  =  (SELECT MIN(RECORD_NO_K) ")
                                SQLWhere.Append(" FROM MEIMAST")
                                SQLWhere.Append(" WHERE TORIS_CODE_K =  " & SQ(stTori.TORIS_CODE_T))
                                SQLWhere.Append("   AND TORIF_CODE_K =  " & SQ(stTori.TORIF_CODE_T))
                                SQLWhere.Append("   AND FURI_DATE_K  =  " & SQ(stMei.FURIKAE_DATE))
                                SQLWhere.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                                SQLWhere.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                                SQLWhere.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
                                SQLWhere.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                                SQLWhere.Append("   AND FURIKIN_K = " & stMei.FURIKIN_MOTO)
                                '���v�Ɣԍ���FURI_DATA�Ɣ�r����
                                SQLWhere.Append("   AND SUBSTR(FURI_DATA_K,92,20) = " & SQ(stMei.JYUYOKA_NO))
                                SQLWhere.Append("   AND DATA_KBN_K   =  '2'")
                                SQLWhere.Append("   AND RECORD_NO_K NOT IN(" & RecordNo & ")")
                                SQLWhere.Append("   )")
                            End If
                            sSQL = SQL.ToString & SQLWhere.ToString
                            If OraMeiReader.DataReader(sSQL) = True Then
                                intCOUNT = OraMeiReader.GetValueInt(0)
                            End If
                        End If

                        If intCOUNT = 0 Then
                            '�������ʂ�0���̏ꍇ�A���b�Z�[�W��\��
                            If TAKO_SEQ = "0" Then
                                MainLOG.Write("���׌���", "���s", "�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                                                          " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & stMei.KEIYAKU_KAMOKU & _
                                                                          " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN_MOTO)
                                MainLOG.UpdateJOBMASTbyErr("���׌��� �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                              " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & stMei.KEIYAKU_KAMOKU & _
                                              " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN_MOTO)
                            Else
                                MainLOG.Write("���׌���", "���s", "�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & aReadFmt.ZENGIN_REC2.ZG15)
                                MainLOG.UpdateJOBMASTbyErr("���׌������s �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & aReadFmt.ZENGIN_REC2.ZG15)
                            End If
                            Return False
                        ElseIf intCOUNT > 1 Then
                            If TAKO_SEQ = "0" Then
                                MainLOG.Write("���׌���", "���s", "����������ו������� �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                                                     " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & stMei.KEIYAKU_KAMOKU & _
                                                                     " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN_MOTO)
                                MainLOG.UpdateJOBMASTbyErr("���׌�������������ו������� �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                              " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & stMei.KEIYAKU_KAMOKU & _
                                              " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN_MOTO)
                                Return False
                            Else
                                MainLOG.Write("���׌���", "���s", "����������ו������݁@�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & aReadFmt.ZENGIN_REC2.ZG15)
                                MainLOG.UpdateJOBMASTbyErr("���׌�������������ו������� �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & aReadFmt.ZENGIN_REC2.ZG15)
                            End If
                        End If

                        '�X�V
                        SQL = New StringBuilder(128)
                        SQL.Append("UPDATE MEIMAST ")
                        SQL.Append(" SET FURIKETU_CODE_K = " & stMei.FURIKETU_MOTO)
                        SQL.Append(",FURIKETU_CENTERCODE_K = " & stMei.FURIKETU_MOTO)
                        SQL.Append(",KOUSIN_DATE_K = " & SQ(DateTime.Today.ToString("yyyyMMdd")))
                        If TAKO_SEQ = "0" Then
                            RecordNo &= "," & OraMeiReader.GetInt("RECORD_NO")
                        End If

                        Try
                            MainDB.ExecuteNonQuery(SQL.Append(SQLWhere))
                            dblKOUSIN_KEN += 1
                        Catch ex As Exception
                            If TAKO_SEQ = "0" Then
                                MainLOG.Write("���׍X�V", "���s", "�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                              " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & stMei.KEIYAKU_KAMOKU & _
                                              " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN_MOTO & " " & ex.Message & ":" & ex.StackTrace)
                                MainLOG.UpdateJOBMASTbyErr("���׍X�V �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & _
                                              " ���Z�@�ցF" & stMei.KEIYAKU_KIN & " �x�X�F" & stMei.KEIYAKU_SIT & " �ȖځF" & stMei.KEIYAKU_KAMOKU & _
                                              " �����ԍ��F" & stMei.KEIYAKU_KOUZA & " �U�֋��z�F" & stMei.FURIKIN_MOTO)
                            Else
                                MainLOG.Write("���׍X�V", "���s", "�����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ & " " & ex.Message & ":" & ex.StackTrace)
                                MainLOG.UpdateJOBMASTbyErr("���׍X�V �����R�[�h�F" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " �U�֓��F" & stMei.FURIKAE_DATE & " ��ƃV�[�P���X�F" & stMei.KIGYO_SEQ)
                            End If
                            Return False
                        End Try

                        ' �X�P�W���[���L�[���ۑ�
                        stTori = aReadFmt.ToriData.INFOToriMast
                        Dim Key(3) As String
                        Key(0) = stTori.TORIS_CODE_T
                        Key(1) = stTori.TORIF_CODE_T
                        Key(2) = stMei.FURIKAE_DATE
                        Key(3) = stMei.KEIYAKU_KIN

                        Dim oSearch As New mySearchClass
                        If UpdateKeys.BinarySearch(Key, oSearch) < 0 Then
                            UpdateKeys.Add(Key)
                        End If

                        OraMeiReader.Close()
                    End If
NEXT_RECORD:
                Loop
            End If


            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                ' ���s�X�P�W���[���X�V
                If UpdateTAKOSCHMAST(Keys) = False Then
                    Return False
                End If
            Next i

            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                '�����̔_���ł܂Ƃ߂��ꍇ�A���ׂ̋��Z�@�ւ̐������X�V���s������
                '2��ڈȍ~��s�v�Ƃ���
                If mKoParam.TKIN_CODE = MatomeNoukyo AndAlso i > 0 Then
                    Exit For
                End If
                ' �X�P�W���[���X�V
                If UpdateSCHMAST(Keys, False) = False Then
                    Return False
                End If
            Next i

            '*** Str Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            'MainDB.Commit()
            '*** End Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***

        Catch ex As Exception
            MainLOG.Write("���׃}�X�^�o�^����", "���s", ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("�V�X�e���G���[�i���O�Q�Ɓj")

            Return False
        Finally
            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

        End Try

        Return True
    End Function

#End Region

#Region " �X�P�W���[���}�X�^�X�V"
    ' �@�\�@ �F �X�P�W���[���}�X�^�X�V
    '
    ' ����   �F ARG1 - �X�V�L�[
    '           ARG2 - ���[�h TRUE-���s���ʍX�V�CFALSE-���s���ʍX�V
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateSCHMAST(ByVal Keys() As String, ByVal mode As Boolean) As Boolean
        '*** Str Del 2015/12/01 SO)�r�� for �s�vMyOracleReader�폜 ***
        'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
        '*** End Del 2015/12/01 SO)�r�� for �s�vMyOracleReader�폜 ***
        Dim SQL As New StringBuilder(128)
        Dim nUpdate As Integer

        Dim KeyToriSCode As String = ""
        Dim KeyToriFCode As String = ""
        Dim KeyFuriDate As String = ""

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Dim OraSchReader As CASTCommon.MyOracleReader = Nothing
        Dim OraCntReader As CASTCommon.MyOracleReader = Nothing

        Try
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

            If Keys.Length = 1 Then
                KeyFuriDate = Keys(0)
            Else
                KeyToriSCode = Keys(0)
                KeyToriFCode = Keys(1)
                KeyFuriDate = Keys(2)

                MainLOG.ToriCode = KeyToriSCode & KeyToriFCode
                MainLOG.FuriDate = KeyFuriDate
            End If

            If mode = False Then
                ' ���s���ʍX�V
                ' �X�P�W���[���}�X�^�X�V
                ' ���s�������Ȃ��i�`��)
                'Try
                '    SQL = New StringBuilder(128)
                '    SQL.Append("UPDATE SCHMAST")
                '    SQL.Append(" SET")
                '    'SQL.Append(" FUNOU_T1DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                '    SQL.Append(" FUNOU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                '    SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                '    'SQL.Append("   AND HAISIN_T1FLG_S= '1'")
                '    SQL.Append("   AND HAISIN_FLG_S= '1'")
                '    SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                '    SQL.Append("   AND FUNOU_FLG_S IN ('0', '2')")
                '    If Keys.Length > 1 Then
                '        SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                '        SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                '    End If
                '    SQL.Append("   AND NOT EXISTS")
                '    SQL.Append("(SELECT TORIS_CODE_U ")
                '    SQL.Append("  FROM TAKOSCHMAST")
                '    SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '    SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '    SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '    SQL.Append("   AND FUNOU_FLG_U  = '0'")             ' �s�\������������
                '    SQL.Append("   AND SYORI_KEN_U  <> 0")
                '    SQL.Append("   AND SYORI_KIN_U  <> 0")
                '    'SQL.Append("   AND TEIKEI_KBN_U = '0'")
                '    SQL.Append("   AND BAITAI_CODE_U = '00'")
                '    SQL.Append(")")
                '    SQL.Append("   AND EXISTS")
                '    SQL.Append("(SELECT TORIS_CODE_U ")
                '    SQL.Append("  FROM TAKOSCHMAST")
                '    SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '    SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '    SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '    SQL.Append("   AND FUNOU_FLG_U  = '1'")             ' �s�\����������
                '    SQL.Append("   AND SYORI_KEN_U  <> 0")
                '    SQL.Append("   AND SYORI_KIN_U  <> 0")
                '    'SQL.Append("   AND TEIKEI_KBN_U = '0'")
                '    SQL.Append("   AND BAITAI_CODE_U = '00'")
                '    SQL.Append(")")

                '    nUpdate = MainDB.ExecuteNonQuery(SQL)

                '    MainLOG.Write("�X�P�W���[���X�V�i���s����������i�`��)�j", "����", "�����F" & nUpdate.ToString)
                'Catch ex As Exception
                '    MainLOG.FuriDate = KeyFuriDate
                '    MainLOG.Write("�X�P�W���[���X�V�i���s����������i�`��)�j", "���s", "�U�֓��F" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                '    MainLOG.UpdateJOBMASTbyErr("�X�P�W���[���X�V�i���s����������i�`��)�j �U�֓��F" & KeyFuriDate)
                '    Return False
                'End Try

                '' ���s�������Ȃ��i�`���ȊO)
                'Try
                '    SQL = New StringBuilder(128)
                '    SQL.Append("UPDATE SCHMAST")
                '    SQL.Append(" SET")
                '    SQL.Append(" FUNOU_T2DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                '    SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                '    'SQL.Append("   AND HAISIN_T2FLG_S= '1'")
                '    SQL.Append("   AND HAISIN_FLG_S= '1'")
                '    SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                '    SQL.Append("   AND FUNOU_FLG_S IN ('0', '2')")
                '    If Keys.Length > 1 Then
                '        SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                '        SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                '    End If
                '    SQL.Append("   AND NOT EXISTS")
                '    SQL.Append("(SELECT TORIS_CODE_U ")
                '    SQL.Append("  FROM TAKOSCHMAST")
                '    SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '    SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '    SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '    SQL.Append("   AND FUNOU_FLG_U  = '0'")             ' �s�\������������
                '    SQL.Append("   AND SYORI_KEN_U  <> 0")
                '    SQL.Append("   AND SYORI_KIN_U  <> 0")
                '    'SQL.Append("   AND TEIKEI_KBN_U = '0'")
                '    SQL.Append("   AND BAITAI_CODE_U <> '00'")
                '    SQL.Append(")")
                '    SQL.Append("   AND EXISTS")
                '    SQL.Append("(SELECT TORIS_CODE_U ")
                '    SQL.Append("  FROM TAKOSCHMAST")
                '    SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '    SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '    SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '    SQL.Append("   AND FUNOU_FLG_U  = '1'")             ' �s�\����������
                '    SQL.Append("   AND SYORI_KEN_U  <> 0")
                '    SQL.Append("   AND SYORI_KIN_U  <> 0")
                '    'SQL.Append("   AND TEIKEI_KBN_U = '0'")
                '    SQL.Append("   AND BAITAI_CODE_U <> '00'")
                '    SQL.Append(")")

                '    nUpdate = MainDB.ExecuteNonQuery(SQL)

                '    MainLOG.Write("�X�P�W���[���X�V�i���s����������i�`���ȊO)�j", "����", "�����F" & nUpdate.ToString)
                'Catch ex As Exception
                '    MainLOG.FuriDate = KeyFuriDate
                '    MainLOG.Write("�X�P�W���[���X�V�i���s����������i�`���ȊO)�j", "���s", "�U�֓��F" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                '    MainLOG.UpdateJOBMASTbyErr("�X�P�W���[���X�V�i���s����������i�`���ȊO)�j �U�֓��F" & KeyFuriDate)
                '    Return False
                'End Try

                '================================
                'SSS�̂��߃R�����g��
                '================================
                '    ' �r�r�r�������Ȃ��i��g��)
                '    Try
                '        SQL = New StringBuilder(128)
                '        SQL.Append("UPDATE SCHMAST")
                '        SQL.Append(" SET")
                '        SQL.Append(" FUNOU_T1DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                '        SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                '        SQL.Append("   AND HAISIN_T1FLG_S= '1'")
                '        SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                '        SQL.Append("   AND FUNOU_FLG_S IN ('0', '2')")
                '        If Keys.Length > 1 Then
                '            SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                '            SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                '        End If
                '        SQL.Append("   AND NOT EXISTS")
                '        SQL.Append("(SELECT TORIS_CODE_U ")
                '        SQL.Append("  FROM TAKOSCHMAST")
                '        SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '        SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '        SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '        SQL.Append("   AND FUNOU_FLG_U  = '0'")             ' �s�\������������
                '        SQL.Append("   AND SYORI_KEN_U  <> 0")
                '        SQL.Append("   AND SYORI_KIN_U  <> 0")
                '        SQL.Append("   AND TEIKEI_KBN_U = '1'")
                '        SQL.Append(")")
                '        SQL.Append("   AND EXISTS")
                '        SQL.Append("(SELECT TORIS_CODE_U ")
                '        SQL.Append("  FROM TAKOSCHMAST")
                '        SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '        SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '        SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '        SQL.Append("   AND FUNOU_FLG_U  = '1'")             ' �s�\����������
                '        SQL.Append("   AND SYORI_KEN_U  <> 0")
                '        SQL.Append("   AND SYORI_KIN_U  <> 0")
                '        SQL.Append("   AND TEIKEI_KBN_U = '1'")
                '        SQL.Append(")")

                '        nUpdate = MainDB.ExecuteNonQuery(SQL)

                '        MainLOG.Write("�X�P�W���[���X�V�i�r�r�r�������Ȃ��i��g��)�j", "����", "�����F" & nUpdate.ToString)
                '    Catch ex As Exception
                '        MainLOG.FuriDate = KeyFuriDate
                '        MainLOG.Write("�X�P�W���[���X�V�i�r�r�r�������Ȃ��i��g��)�j", "���s", "�U�֓��F" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                '        MainLOG.UpdateJOBMASTbyErr("�X�P�W���[���X�V�i�r�r�r�������Ȃ��i��g��)�j �U�֓��F" & KeyFuriDate)
                '        Return False
                '    End Try

                '    ' �r�r�r�������Ȃ��i��g�O)
                '    Try
                '        SQL = New StringBuilder(128)
                '        SQL.Append("UPDATE SCHMAST")
                '        SQL.Append(" SET")
                '        SQL.Append(" FUNOU_T2DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                '        SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                '        SQL.Append("   AND HAISIN_T2FLG_S= '1'")
                '        SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                '        SQL.Append("   AND FUNOU_FLG_S IN ('0', '2')")
                '        If Keys.Length > 1 Then
                '            SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                '            SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                '        End If
                '        SQL.Append("   AND NOT EXISTS")
                '        SQL.Append("(SELECT TORIS_CODE_U ")
                '        SQL.Append("  FROM TAKOSCHMAST")
                '        SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '        SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '        SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '        SQL.Append("   AND FUNOU_FLG_U  = '0'")             ' �s�\������������
                '        SQL.Append("   AND SYORI_KEN_U  <> 0")
                '        SQL.Append("   AND SYORI_KIN_U  <> 0")
                '        SQL.Append("   AND TEIKEI_KBN_U = '2'")
                '        SQL.Append(")")
                '        SQL.Append("   AND EXISTS")
                '        SQL.Append("(SELECT TORIS_CODE_U ")
                '        SQL.Append("  FROM TAKOSCHMAST")
                '        SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '        SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '        SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '        SQL.Append("   AND FUNOU_FLG_U  = '1'")             ' �s�\����������
                '        SQL.Append("   AND SYORI_KEN_U  <> 0")
                '        SQL.Append("   AND SYORI_KIN_U  <> 0")
                '        SQL.Append("   AND TEIKEI_KBN_U = '2'")
                '        SQL.Append(")")

                '        nUpdate = MainDB.ExecuteNonQuery(SQL)

                '        MainLOG.Write("�X�P�W���[���X�V�i�r�r�r�������Ȃ��i��g�O)�j", "����", "�����F" & nUpdate.ToString)
                '    Catch ex As Exception
                '        MainLOG.FuriDate = KeyFuriDate
                '        MainLOG.Write("�X�P�W���[���X�V�i�r�r�r�������Ȃ��i��g�O)�j", "���s", "�U�֓��F" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                '        MainLOG.UpdateJOBMASTbyErr("�X�P�W���[���X�V�i�r�r�r�������Ȃ��i��g�O)�j �U�֓��F" & KeyFuriDate)
                '        Return False
                '    End Try
            End If

            '2017/01/20 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
            '�X���[�G�X�ň˗����S�Ď��s�̏ꍇ�A���s�f�[�^�쐬���s���Ă��z�M�f�[�^�Ɋ܂܂�Ȃ����߁A
            '���s�X�P�W���[���}�X�^�̏��������Ƌ��z�����đ��s�X�P�W���[���}�X�^�̕s�\�t���O���X�V����B
            Try
                SQL = New StringBuilder(128)
                SQL.Append("UPDATE TAKOSCHMAST")
                SQL.Append(" SET")
                SQL.Append(" FUNOU_FLG_U = '1'")
                SQL.Append(" WHERE FURI_DATE_U = " & SQ(KeyFuriDate))
                SQL.Append(" AND SYORI_KEN_U = 0")
                SQL.Append(" AND SYORI_KIN_U = 0")
                SQL.Append(" AND TKIN_NO_U = " & SQ(Jikinko))
                SQL.Append(" AND EXISTS(")
                SQL.Append(" SELECT * FROM SCHMAST")
                SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                SQL.Append(" AND TORIF_CODE_U = TORIF_CODE_S")
                SQL.Append(" AND FURI_DATE_U = FURI_DATE_S")
                SQL.Append(" AND TAKOU_FLG_S = '1'")
                SQL.Append(" )")

                nUpdate = MainDB.ExecuteNonQuery(SQL)
                MainLOG.Write("���s�X�P�W���[���X�V�i�r�r�r���s�˗��������ʍX�V�j", "����", "�����F" & nUpdate.ToString)

            Catch ex As Exception
                MainLOG.FuriDate = KeyFuriDate
                MainLOG.Write("���s�X�P�W���[���X�V�i�r�r�r���s�˗��������ʍX�V�j", "���s", "�U�֓��F" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                MainLOG.UpdateJOBMASTbyErr("���s�X�P�W���[���X�V�i�r�r�r���s�˗��������ʍX�V�j �U�֓��F" & KeyFuriDate)
                Return False
            End Try
            '2017/01/20 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END

            ' �S�Ċ����̏ꍇ
            Try
                SQL = New StringBuilder(128)
                SQL.Append("UPDATE SCHMAST")
                SQL.Append(" SET")
                SQL.Append(" FUNOU_FLG_S = '1'")
                SQL.Append(",FUNOU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                SQL.Append("   AND HAISIN_FLG_S= '1'")
                SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                '���C�ȊO�̓Z���^�[���ڎ����͍X�V���Ȃ�
                If CENTER <> "4" Then
                    SQL.Append("   AND MOTIKOMI_KBN_S = '0'")
                End If
                If mode = False Then
                    ' ���s���ʍX�V�̏ꍇ�C�s�\�t���O���Q�̎��̂� 
                    SQL.Append("   AND FUNOU_FLG_S = '2'")
                Else
                    SQL.Append("   AND FUNOU_FLG_S IN ('0', '2')")
                End If
                '�M�g�Ή� �g�������̏ꍇ�͑��M�敪�������ɓ����
                If CENTER = "0" AndAlso mKoParam.MOTIKOMI_KBN = "0" Then
                    SQL.Append("   AND SOUSIN_KBN_S = '0'")
                End If
                If Keys.Length > 1 Then
                    If mArgumentData.INFOToriMast.SOUSIN_KBN_T = "1" AndAlso mArgumentData.INFOToriMast.MULTI_KBN_T = "1" Then
                        '��Ǝ���(�}���`)
                        '2010/12/24 �N�G���C�� ��������
                        'SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        SQL.Append(" AND EXISTS(")
                        SQL.Append(" SELECT * FROM TORIMAST")
                        SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
                        SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                        SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        SQL.Append(")")
                        '2010/12/24 �N�G���C�� �����܂�
                        SQL.Append(" AND SOUSIN_KBN_S = '1'")
                    Else
                        SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                        SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                    End If
                End If
                SQL.Append("   AND ((NOT EXISTS")
                SQL.Append("(SELECT TORIS_CODE_U ")
                SQL.Append("  FROM TAKOSCHMAST")
                SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                SQL.Append("   AND SYORI_KEN_U  <> 0")
                SQL.Append("   AND SYORI_KIN_U  <> 0")
                SQL.Append(") AND TAKOU_FLG_S = '0')")
                SQL.Append(" OR ")
                SQL.Append("   (EXISTS")
                SQL.Append("(SELECT TORIS_CODE_U ")
                SQL.Append("  FROM TAKOSCHMAST")
                SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                SQL.Append("   AND SYORI_KEN_U  <> 0")
                SQL.Append("   AND SYORI_KIN_U  <> 0")
                '2017/01/20 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
                '�����Ƃ̋�ʂ�}�邽�߁A���Z�@�փR�[�h�����s�ȊO�̏�����ǉ�
                SQL.Append("   AND TKIN_NO_U <> " & SQ(Jikinko))
                '2017/01/20 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END
                '2017/01/20 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
                '�X���[�G�X�̏����ǉ�
                'SQL.Append(") AND TAKOU_FLG_S = '1'))")
                SQL.Append(") AND TAKOU_FLG_S = '1')")
                SQL.Append(" OR ")
                SQL.Append("   (EXISTS")
                SQL.Append("(SELECT TORIS_CODE_U ")
                SQL.Append("  FROM TAKOSCHMAST")
                SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                'SQL.Append("   AND SYORI_KEN_U  <> 0")
                'SQL.Append("   AND SYORI_KIN_U  <> 0")
                SQL.Append("   AND TKIN_NO_U = " & SQ(Jikinko))
                SQL.Append("   AND FUNOU_FLG_U = '1'")
                SQL.Append(") AND TAKOU_FLG_S = '1'))")
                '2017/01/20 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
                nUpdate = MainDB.ExecuteNonQuery(SQL)
                MainLOG.Write("�X�P�W���[���X�V�i���ʍX�V�j", "����", "�����F" & nUpdate.ToString)
            Catch ex As Exception
                MainLOG.FuriDate = KeyFuriDate
                MainLOG.Write("�X�P�W���[���X�V�i���ʍX�V�j", "���s", "�U�֓��F" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                MainLOG.UpdateJOBMASTbyErr("�X�P�W���[���X�V�i���s�̂݁j �U�֓��F" & KeyFuriDate)
                Return False
            End Try

            ' ���ʕԋp�s�v�̏ꍇ
            Try
                SQL = New StringBuilder(128)
                SQL.Append("UPDATE SCHMAST")
                SQL.Append(" SET")
                SQL.Append(" HENKAN_FLG_S = '1'")
                SQL.Append(",HENKAN_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                SQL.Append("   AND HAISIN_FLG_S= '1'")
                SQL.Append("   AND FUNOU_FLG_S = '1'")
                SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                SQL.Append("   AND HENKAN_FLG_S = '0'")
                '���C�ȊO�̓Z���^�[���ڎ����͍X�V���Ȃ�
                If CENTER <> "4" Then
                    SQL.Append("   AND MOTIKOMI_KBN_S = '0'")
                End If
                '�M�g�Ή� �g�������̏ꍇ�͑��M�敪�������ɓ����
                If CENTER = "0" AndAlso mKoParam.MOTIKOMI_KBN = "0" Then
                    SQL.Append("   AND SOUSIN_KBN_S = '0'")
                End If
                If Keys.Length > 1 Then
                    If mArgumentData.INFOToriMast.SOUSIN_KBN_T = "1" AndAlso mArgumentData.INFOToriMast.MULTI_KBN_T = "1" Then
                        '��Ǝ���(�}���`)
                        '2010/12/24 �N�G���C�� ��������
                        'SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        SQL.Append(" AND EXISTS(")
                        SQL.Append(" SELECT * FROM TORIMAST")
                        SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
                        SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                        SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        SQL.Append(")")
                        '2010/12/24 �N�G���C�� �����܂�
                        SQL.Append(" AND SOUSIN_KBN_S = '1'")
                    Else
                        SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                        SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                    End If
                End If
                SQL.Append("   AND EXISTS")
                SQL.Append("(SELECT TORIS_CODE_T ")
                SQL.Append("  FROM TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = TORIS_CODE_S")
                SQL.Append("   AND TORIF_CODE_T = TORIF_CODE_S")
                SQL.Append("   AND KEKKA_HENKYAKU_KBN_T = '0'")         ' ���ʕԋp�s�v
                SQL.Append(")")

                nUpdate = MainDB.ExecuteNonQuery(SQL)
                MainLOG.Write("�X�P�W���[���X�V�i���s�̂݁j", "����", "�����F" & nUpdate.ToString)

            Catch ex As Exception
                MainLOG.FuriDate = KeyFuriDate
                MainLOG.Write("�X�P�W���[���X�V�i���s�̂݁j", "���s", "�U�֓��F" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                MainLOG.UpdateJOBMASTbyErr("�X�P�W���[���X�V�i���s�̂݁j �U�֓��F" & KeyFuriDate)
                Return False
            End Try

            If Keys.Length = 1 Then
                ' ���s�C�r�r�r����������
                Try
                    SQL = New StringBuilder(128)
                    SQL.Append("UPDATE SCHMAST")
                    SQL.Append(" SET")
                    SQL.Append(" FUNOU_FLG_S = '2'")
                    SQL.Append(",FUNOU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                    SQL.Append("   AND HAISIN_FLG_S= '1'")
                    SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                    SQL.Append("   AND FUNOU_FLG_S   IN ( '0', '2' )")
                    SQL.Append("   AND EXISTS")
                    SQL.Append("(SELECT TORIS_CODE_U ")
                    SQL.Append("  FROM TAKOSCHMAST")
                    SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                    SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                    SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                    SQL.Append("   AND FUNOU_FLG_U  = '0'")
                    SQL.Append("   AND SYORI_KEN_U  <> 0")
                    SQL.Append("   AND SYORI_KIN_U  <> 0")
                    SQL.Append(")")

                    nUpdate = MainDB.ExecuteNonQuery(SQL)
                    MainLOG.Write("�X�P�W���[���X�V�i���s����������j", "����", "�����F" & nUpdate.ToString)
                Catch ex As Exception
                    MainLOG.FuriDate = KeyFuriDate
                    MainLOG.Write("�X�P�W���[���X�V�i���s����������j", "���s", "�U�֓��F" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                    MainLOG.UpdateJOBMASTbyErr("�X�P�W���[���X�V�i���s����������j �U�֓��F" & KeyFuriDate)
                    Return False
                End Try
            End If

            ' ���v
            '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            'Dim OraSchReader As New CASTCommon.MyOracleReader(MainDB)
            'Dim OraCntReader As New CASTCommon.MyOracleReader(MainDB)
            OraSchReader = New CASTCommon.MyOracleReader(MainDB)
            OraCntReader = New CASTCommon.MyOracleReader(MainDB)
            '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

            '�������ʊm�F�\(�s�\���ʍX�V)
            Dim CreateCSV As New KFJP013
            Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
            Dim NowTime As String = Format(DateTime.Now, "HHmmss")
            Dim strCSV_FILE_NAME As String = CreateCSV.CreateCsvFile()
            '------------------------------------
            '�X�V�Ώۂ̃X�P�W���[������
            '------------------------------------
            SQL = New StringBuilder(128)
            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_S")
            SQL.Append(",TORIF_CODE_S")
            SQL.Append(",FURI_DATE_S")
            SQL.Append(",KIGYO_CODE_S")
            SQL.Append(",FURI_CODE_S")
            SQL.Append(",FMT_KBN_T")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",ITAKU_CODE_T")
            SQL.Append(",SYORI_KEN_S")
            SQL.Append(",SYORI_KIN_S")
            SQL.Append(",FUNOU_FLG_S")
            SQL.Append(",FUNOU_KEN_S")
            SQL.Append(",FUNOU_KIN_S")
            SQL.Append(",MOTIKOMI_KBN_S")
            SQL.Append(" FROM SCHMAST ")
            SQL.Append("     ,TORIMAST ")
            SQL.Append(" WHERE FURI_DATE_S   = " & SQ(KeyFuriDate))
            SQL.Append("   AND FUNOU_FLG_S IN ('1','2')")
            SQL.Append("   AND TYUUDAN_FLG_S = '0'")
            SQL.Append("   AND TORIS_CODE_T = TORIS_CODE_S")
            SQL.Append("   AND TORIF_CODE_T = TORIF_CODE_S")
            '�M�g�Ή� �g�������̏ꍇ�͑��M�敪�������ɓ����
            If CENTER = "0" AndAlso mKoParam.MOTIKOMI_KBN = "0" Then
                SQL.Append("   AND SOUSIN_KBN_S = '0'")
            End If
            Select Case mKoParam.MOTIKOMI_KBN
                Case "0" '���Ɏ���
                    '���C�ȊO�̓Z���^�[���ڎ����͍X�V���Ȃ�
                    If CENTER <> "4" Then
                        SQL.Append("   AND MOTIKOMI_KBN_S = '0'")
                    End If
                Case "1" '��Ǝ���
                    If mArgumentData.INFOToriMast.MULTI_KBN_T = "1" Then
                        '��Ǝ���(�}���`)
                        '2010/12/24 �N�G���C�� ��������
                        'SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        SQL.Append(" AND EXISTS(")
                        SQL.Append(" SELECT * FROM TORIMAST")
                        SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
                        SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                        SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        SQL.Append(")")
                        '2010/12/24 �N�G���C�� �����܂�
                        SQL.Append(" AND SOUSIN_KBN_S = '1'")
                    Else
                        SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                        SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                    End If
                Case "2" '���s�U��
                    SQL.Append("   AND TORIS_CODE_T = " & SQ(KeyToriSCode))
                    SQL.Append("   AND TORIF_CODE_T = " & SQ(KeyToriFCode))
                    '2017/01/20 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
                Case "4"
                    SQL.Append("   AND TORIS_CODE_T = " & SQ(KeyToriSCode))
                    SQL.Append("   AND TORIF_CODE_T = " & SQ(KeyToriFCode))
                    '2017/01/20 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END
            End Select
            SQL.Append(" ORDER BY TORIS_CODE_S,TORIF_CODE_S")

            Dim intCOUNT As Integer
            If OraSchReader.DataReader(SQL) = True Then
                intCOUNT = 0
                While OraSchReader.EOF = False
                    intCOUNT += 1

                    '----------------------
                    '�s�\�����A���z�擾
                    '----------------------
                    Dim dblFUNOU_KEN As Decimal = 0
                    Dim dblFUNOU_KIN As Decimal = 0
                    Dim dblFURI_KEN As Decimal = 0
                    Dim dblFURI_KIN As Decimal = 0

                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT COUNT(FURIKIN_K) CNT1,SUM(FURIKIN_K) CNT2")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE FURI_DATE_K     = " & SQ(KeyFuriDate))
                    If OraSchReader.GetString("FMT_KBN_T") = "02" Then
                        ' ���ł̏ꍇ
                        SQL.Append("   AND DATA_KBN_K      = '3'")
                    Else
                        SQL.Append("   AND DATA_KBN_K      = '2'")
                    End If
                    SQL.Append("   AND FURIKETU_CODE_K <> 0")
                    '�U�֋��z���O�~�̂��̂͊܂܂Ȃ�
                    SQL.Append("  AND FURIKIN_K > 0")
                    SQL.Append("  AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    SQL.Append("  AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))

                    If OraCntReader.DataReader(SQL) = True Then
                        dblFUNOU_KEN = OraCntReader.GetInt64("CNT1")
                        dblFUNOU_KIN = OraCntReader.GetInt64("CNT2")
                    End If
                    OraCntReader.Close()

                    '----------------------
                    '�U�֍ό����A���z�擾
                    '----------------------
                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT COUNT(FURIKIN_K) CNT1,SUM(FURIKIN_K) CNT2")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE FURI_DATE_K     = " & SQ(KeyFuriDate))
                    If OraSchReader.GetString("FMT_KBN_T") = "02" Then
                        ' ���ł̏ꍇ
                        SQL.Append("   AND DATA_KBN_K      = '3'")
                    Else
                        SQL.Append("   AND DATA_KBN_K      = '2'")
                    End If
                    SQL.Append("   AND FURIKETU_CODE_K =  0")
                    '' 2008.03.14 �U�֋��z���O�~�̂��̂͊܂܂Ȃ�
                    SQL.Append("  AND FURIKIN_K > 0 ")
                    SQL.Append("  AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    SQL.Append("  AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                    If OraCntReader.DataReader(SQL) = True Then
                        dblFURI_KEN = OraCntReader.GetInt64("CNT1")
                        dblFURI_KIN = OraCntReader.GetInt64("CNT2")
                    End If
                    OraCntReader.Close()

                    '-------------------------------------------
                    '�X�P�W���[���}�X�^�̍X�V
                    '-------------------------------------------
                    SQL = New StringBuilder(128)
                    SQL.Append("UPDATE SCHMAST SET")
                    SQL.Append(" FURI_KEN_S = " & dblFURI_KEN.ToString)
                    SQL.Append(",FURI_KIN_S = " & dblFURI_KIN.ToString)
                    SQL.Append(",FUNOU_KEN_S = " & dblFUNOU_KEN.ToString)
                    SQL.Append(",FUNOU_KIN_S =" & dblFUNOU_KIN.ToString)
                    SQL.Append(" WHERE TORIS_CODE_S = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    SQL.Append("  AND TORIF_CODE_S = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                    SQL.Append("  AND FURI_DATE_S  = " & SQ(KeyFuriDate))

                    Try
                        MainDB.ExecuteNonQuery(SQL)
                    Catch ex As Exception
                        MainLOG.Write("�X�P�W���[���X�V�i�U�֍ς݁j", "���s", "�U�֓��F" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                        MainLOG.UpdateJOBMASTbyErr("�X�P�W���[���X�V�i�U�֍ς݁j �U�֓��F" & KeyFuriDate)
                        Return False
                    End Try

                    '���[����p
                    CreateCSV.OutputCsvData(Today, True)                                                '�V�X�e�����t
                    CreateCSV.OutputCsvData(NowTime, True)                                              '�^�C���X�^���v
                    CreateCSV.OutputCsvData(KeyFuriDate, True)                                          '�U�֓�
                    CreateCSV.OutputCsvData(OraSchReader.GetString("TORIS_CODE_S"), True)               '������R�[�h
                    CreateCSV.OutputCsvData(OraSchReader.GetString("TORIF_CODE_S"), True)               '����敛�R�[�h
                    CreateCSV.OutputCsvData(OraSchReader.GetString("ITAKU_NNAME_T"), True)              '����於
                    CreateCSV.OutputCsvData(OraSchReader.GetString("ITAKU_CODE_T"), True)               '�ϑ��҃R�[�h
                    CreateCSV.OutputCsvData(OraSchReader.GetString("SYORI_KEN_S"), True)                '�˗�����
                    CreateCSV.OutputCsvData(OraSchReader.GetString("SYORI_KIN_S"), True)                '�˗����z
                    CreateCSV.OutputCsvData(dblFUNOU_KEN.ToString, True)                                '�s�\����
                    CreateCSV.OutputCsvData(dblFUNOU_KIN.ToString, True)                                '�s�\���z
                    Select Case OraSchReader.GetString("FUNOU_FLG_S")                                   '���l
                        Case "1"
                            CreateCSV.OutputCsvData("�s�\���ʍX�V����", True)
                        Case "2"
                            CreateCSV.OutputCsvData("����������", True)
                    End Select
                    CreateCSV.OutputCsvData(OraSchReader.GetString("FURI_CODE_S"), True)                '�U�փR�[�h
                    CreateCSV.OutputCsvData(OraSchReader.GetString("KIGYO_CODE_S"), True, True)         '��ƃR�[�h

                    OraSchReader.NextRead()
                End While
            End If
            CreateCSV.CloseCsv()

            ' �w�Z�X�V
            If UpdateG_SCHMAST(Keys) = False Then
                Return False
            End If

            '����o�b�`�Ăяo��
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String = ""
            Dim errMessage As String
            Dim nret As Integer

            If intCOUNT = 0 Then
                errMessage = "�X�P�W���[���X�V������0���ł��B"
                MainLOG.JobMessage = "�X�P�W���[���X�V������0���̂��ߏ����𒆒f���܂��B"
                If mKoParam.MOTIKOMI_KBN = "2" Then
                    errMessage &= "���s���̕s�\���ʍX�V������"
                    MainLOG.JobMessage &= "���s���̍X�V���ɍs���Ă��������B"
                End If
                MainLOG.UpdateJOBMASTbyErr(MainLOG.JobMessage)
                MainLOG.Write("�X�P�W���[���X�V", "���s", errMessage)
                Return False
            End If

            '�M�g�Ή� ��Ǝ����̏ꍇ�͒��[������Ȃ�
            If CENTER = "0" AndAlso mKoParam.MOTIKOMI_KBN = "1" Then
                Return True
            End If

            ' 2016/03/09 �^�X�N�j���� ADD �yOT�zUI_B-14-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- START
            ' �������ʊm�F�\����v�ۂ� "0" �̏ꍇ�́A���[������Ȃ�
            If INI_RSV2_SYORIKEKKA_FUNOU = "0" Then
                Return True
            End If
            ' 2016/03/09 �^�X�N�j���� ADD �yOT�zUI_B-14-99(RSV2�Ή�(�ǉ��J�X�^�}�C�Y)) -------------------- END

            '2017/01/20 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
            '�X���[�G�X�̏ꍇ�͂����ň�����Ȃ�
            If mKoParam.MOTIKOMI_KBN = "4" Then
                Return True
            End If
            '2017/01/20 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END

            '�p�����[�^�ݒ�F���O�C�����A�b�r�u�t�@�C����
            param = MainLOG.UserID & "," & strCSV_FILE_NAME

            nret = ExeRepo.ExecReport("KFJP013.EXE", param)

            If nret <> 0 Then
                '������s�F�߂�l�ɑΉ������G���[���b�Z�[�W��\������
                Select Case nret
                    Case -1
                        errMessage = "�������ʊm�F�\(�s�\���ʍX�V)�̈���Ώۂ�0���ł��B"
                    Case Else
                        errMessage = "�������ʊm�F�\(�s�\���ʍX�V)�̈���Ɏ��s���܂����B"
                        MainLOG.JobMessage = "�������ʊm�F�\(�s�\���ʍX�V)������s"
                        MainLOG.UpdateJOBMASTbyErr(MainLOG.JobMessage)
                End Select
                MainLOG.Write("�������ʊm�F�\(�s�\���ʍX�V)���", "���s", errMessage)
                Return False
            Else
                MainLOG.Write("�������ʊm�F�\(�s�\���ʍX�V)���", "����")
                Return True
            End If

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Finally
            If Not OraSchReader Is Nothing Then
                OraSchReader.Close()
            End If

            If Not OraCntReader Is Nothing Then
                OraCntReader.Close()
            End If
        End Try
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

    End Function

    ''' <summary>
    ''' �X�P�W���[���}�X�^�X�V�i�r�r�r�j
    ''' </summary>
    ''' <param name="Keys">�X�V�L�[</param>
    ''' <param name="mode">���[�h(True:���s���ʍX�V False:���s���ʍX�V)</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function UpdateSCHMASTSSS(ByVal Keys() As String, _
                                      ByVal mode As Boolean) As Boolean
        Dim SQL As New StringBuilder
        Dim nUpdate As Integer

        Dim KeyToriSCode As String = ""
        Dim KeyToriFCode As String = ""
        Dim KeyFuriDate As String = ""

        If Keys.Length = 1 Then
            KeyFuriDate = Keys(0)
        Else
            KeyToriSCode = Keys(0)
            KeyToriFCode = Keys(1)
            KeyFuriDate = Keys(2)

            MainLOG.ToriCode = KeyToriSCode & KeyToriFCode
            MainLOG.FuriDate = KeyFuriDate
        End If

        '-------------------------------------------
        '�X�P�W���[���}�X�^�̍X�V
        '-------------------------------------------
        '�X�P�W���[���}�X�^�̌����Ƌ��z���X�V����Ȃ����߁A�����Ŗ��ׂ��W�v���āA�������l�ɍX�V����B
        '�Ȃ��A�X���[�G�X�̌��ʍX�V�ł͎萔���v�Z�͊��Ɏ��s���Ōv�Z�ς݂Ȃ̂ŁA���̎萔���v�Z�̏����Ōv�Z����Ȃ����Ƃ�Y��Ă͂Ȃ�Ȃ��B
        Dim oraSchReader As CASTCommon.MyOracleReader = Nothing
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing

        With SQL
            .Length = 0
            If Keys.Length = 1 Then
                .Append("select TORIS_CODE_S, TORIF_CODE_S ")
                .Append(" from SCHMAST, TORIMAST")
                .Append(" where FURI_DATE_S = " & SQ(KeyFuriDate))
                .Append(" and HAISIN_FLG_S = '1'")
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and FMT_KBN_T in ('20', '21')")
                .Append(" and TAKOU_FLG_S = '1'")
                .Append(" and TORIS_CODE_S = TORIS_CODE_T")
                .Append(" and TORIF_CODE_S = TORIF_CODE_T")
            Else
                .Append("select TORIS_CODE_S, TORIF_CODE_S ")
                .Append(" from SCHMAST, TORIMAST")
                .Append(" where FURI_DATE_S = " & SQ(KeyFuriDate))
                .Append(" and TORIS_CODE_S = " & SQ(KeyToriSCode))
                .Append(" and TORIF_CODE_S = " & SQ(KeyToriFCode))
                .Append(" and HAISIN_FLG_S = '1'")
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and FMT_KBN_T in ('20', '21')")
                .Append(" and TAKOU_FLG_S = '1'")
                .Append(" and TORIS_CODE_S = TORIS_CODE_T")
                .Append(" and TORIF_CODE_S = TORIF_CODE_T")
            End If
        End With

        Try
            oraSchReader = New CASTCommon.MyOracleReader(MainDB)
            If oraSchReader.DataReader(SQL) = True Then
                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                While oraSchReader.EOF = False
                    With SQL
                        .Length = 0
                        .Append("select ")
                        .Append(" count(FURIKIN_K) as SYORIKEN")
                        .Append(",sum(FURIKIN_K) as SYORIKIN")
                        .Append(",sum(decode(FURIKETU_CODE_K, 0, 1, 0)) as FURIKEN")
                        .Append(",sum(decode(FURIKETU_CODE_K, 0, FURIKIN_K, 0)) as FURIKIN")
                        .Append(",sum(decode(FURIKETU_CODE_K, 0, 0, 1)) as FUNOUKEN")
                        .Append(",sum(decode(FURIKETU_CODE_K, 0, 0, FURIKIN_K)) as FUNOUKIN")
                        .Append(" from MEIMAST")
                        .Append(" where TORIS_CODE_K = " & SQ(oraSchReader.GetString("TORIS_CODE_S")))
                        .Append(" and TORIF_CODE_K = " & SQ(oraSchReader.GetString("TORIF_CODE_S")))
                        .Append(" and FURI_DATE_K = " & SQ(KeyFuriDate))
                        .Append(" and DATA_KBN_K = '2'")
                    End With

                    If oraMeiReader.DataReader(SQL) = True Then
                        '�����Ƌ��z���擾�ł�����X�P�W���[���}�X�^�X�V
                        With SQL
                            .Length = 0
                            .Append("update SCHMAST set ")
                            .Append(" FURI_KEN_S = " & oraMeiReader.GetInt("FURIKEN"))
                            .Append(",FURI_KIN_S = " & oraMeiReader.GetInt64("FURIKIN"))
                            .Append(",FUNOU_KEN_S = " & oraMeiReader.GetInt("FUNOUKEN"))
                            .Append(",FUNOU_KIN_S = " & oraMeiReader.GetInt64("FUNOUKIN"))
                            .Append(",TAKOU_FLG_S = '2'")
                            .Append(" where TORIS_CODE_S = " & SQ(oraSchReader.GetString("TORIS_CODE_S")))
                            .Append(" and TORIF_CODE_S = " & SQ(oraSchReader.GetString("TORIF_CODE_S")))
                            .Append(" and FURI_DATE_S = " & SQ(KeyFuriDate))
                        End With

                        nUpdate = MainDB.ExecuteNonQuery(SQL)
                        If nUpdate < 0 Then
                            '�ُ�I��
                            MainLOG.Write("�X�P�W���[���X�V(SSS���ʍX�V)", "���s", MainDB.Message)
                            MainLOG.UpdateJOBMASTbyErr("�X�P�W���[���X�V(SSS���ʍX�V)���s �����F" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIF_CODE_S") & " �U�֓��F" & KeyFuriDate)
                            Return False
                        End If

                        MainLOG.Write("�X�P�W���[���X�V(SSS���ʍX�V)", "����", "�����F" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIF_CODE_S") & " �U�֓��F" & KeyFuriDate)
                    Else
                        '�ُ�I��
                        MainLOG.Write("�X�P�W���[���X�V(SSS���ʍX�V)", "���s", "���׃}�X�^���猏���Ƌ��z�̏W�v�Ɏ��s")
                        MainLOG.UpdateJOBMASTbyErr("�X�P�W���[���X�V(SSS���ʍX�V)���s �����F" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIF_CODE_S") & " �U�֓��F" & KeyFuriDate)
                        Return False
                    End If

                    oraMeiReader.Close()

                    oraSchReader.NextRead()
                End While
            Else
                '�X�P�W���[������
                MainLOG.Write("�X�P�W���[���X�V(SSS���ʍX�V)", "���s", "�ΏۃX�P�W���[������ �U�֓��F" & KeyFuriDate)
                MainLOG.UpdateJOBMASTbyErr("�X�P�W���[���X�V���s(SSS���ʍX�V) �ΏۃX�P�W���[������")
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("�X�P�W���[���X�V(SSS���ʍX�V)", "���s", "�U�֓��F" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("�X�P�W���[���X�V(SSS) ��O���� �U�֓��F" & KeyFuriDate)
            Return False
        Finally
            If Not oraSchReader Is Nothing Then
                oraSchReader.Close()
                oraSchReader = Nothing
            End If
            If Not oraMeiReader Is Nothing Then
                oraMeiReader.Close()
                oraMeiReader = Nothing
            End If
        End Try

        Return True

    End Function

#End Region

#Region " ���s�X�P�W���[���}�X�^�X�V"
    ' �@�\�@ �F ���s�X�P�W���[���}�X�^�X�V
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateTAKOSCHMAST(ByVal Keys() As String) As Boolean
        '*** Str Del 2015/12/01 SO)�r�� for �s�vMyOracleReader�폜 ***
        'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
        '*** End Del 2015/12/01 SO)�r�� for �s�vMyOracleReader�폜 ***
        Dim SQL As New StringBuilder(128)

        Dim KeyToriSCode As String = ""
        Dim KeyToriFCode As String = ""
        Dim KeyFuriDate As String = ""
        Dim KeyKeiyakuKin As String = ""
        Dim KeyTeikeiKubun As String = "0"

        KeyToriSCode = Keys(0)
        KeyToriFCode = Keys(1)
        KeyFuriDate = Keys(2)
        KeyKeiyakuKin = Keys(3)

        MainLOG.ToriCode = KeyToriSCode & KeyToriFCode
        MainLOG.FuriDate = KeyFuriDate

        If Keys.Length = 5 Then
            KeyTeikeiKubun = Keys(4)
        End If

        ' ���s�}�X�^���R�[�h���b�N
        Dim OraLockReader As New CASTCommon.MyOracleReader(MainDB)
        SQL = New StringBuilder(128)
        SQL.Append("SELECT TORIS_CODE_S, TORIF_CODE_S")
        SQL.Append(" FROM SCHMAST")
        SQL.Append(" WHERE TORIS_CODE_S = " & SQ(KeyToriSCode))
        SQL.Append("   AND TORIF_CODE_S = " & SQ(KeyToriFCode))
        SQL.Append("   AND FURI_DATE_S  = " & SQ(KeyFuriDate))
        SQL.Append(" FOR UPDATE")
        OraLockReader.DataReader(SQL)
        OraLockReader.Close()

        '----------------------
        '�s�\�����A���z�擾
        '----------------------
        Dim nFUNOU_KEN As Decimal = 0
        Dim nFUNOU_KIN As Decimal = 0
        Dim nFURI_KEN As Decimal = 0
        Dim nFURI_KIN As Decimal = 0

        Dim OraCntReader As New CASTCommon.MyOracleReader(MainDB)
        SQL = New StringBuilder(128)
        '2017/01/20 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
        '�R�����g�A�E�g����Ă��������𕜊�������B
        If KeyKeiyakuKin = Jikinko Then
            ' �r�r�r����
            SQL.Append("SELECT COUNT(FURIKIN_K) CNT1,SUM(FURIKIN_K) CNT2")
            SQL.Append(" FROM MEIMAST")
            SQL.Append(" WHERE FURI_DATE_K     = " & SQ(KeyFuriDate))
            SQL.Append("   AND TORIS_CODE_K    = " & SQ(KeyToriSCode))
            SQL.Append("   AND TORIF_CODE_K    = " & SQ(KeyToriFCode))
            SQL.Append("   AND DATA_KBN_K      = '2'")
            '���s���͑ΏۊO�Ƃ���
            SQL.Append("   AND KEIYAKU_KIN_K <> " & SQ(Jikinko))
            SQL.Append("   AND FURIKETU_CODE_K <> 0")
            '��g���A��g�O�̏����͕ύX����
            '�W����(�X���[�G�X���ϖ���)�͒�g���̂ݑΏ�
            SQL.Append("   AND EXISTS (")
            SQL.Append("   SELECT TEIKEI_KBN_N FROM TENMAST ")
            SQL.Append("    WHERE KIN_NO_N = KEIYAKU_KIN_K")
            SQL.Append("      AND SIT_NO_N = KEIYAKU_SIT_K")
            SQL.Append("      AND TEIKEI_KBN_N = '1'")
            SQL.Append("   )")
            'If KeyTeikeiKubun = "1" Then
            '    ' ��g��
            '    SQL.Append("   AND EXISTS (")
            'Else
            '    ' ��g�O
            '    SQL.Append("   AND NOT EXISTS (")
            'End If
            'SQL.Append("   SELECT TEIKEI_KBN_N FROM TENMAST ")
            'SQL.Append("    WHERE KIN_NO_N = KEIYAKU_KIN_K")
            'SQL.Append("      AND SIT_NO_N = KEIYAKU_SIT_K")
            'SQL.Append("      AND EDA_N = '01'")
            'SQL.Append("      AND TEIKEI_KBN_N = '1'")
            'SQL.Append("   )")
        Else
            ' ���s����
            SQL.Append("SELECT COUNT(FURIKIN_K) CNT1,SUM(FURIKIN_K) CNT2")
            SQL.Append(" FROM MEIMAST")
            SQL.Append(" WHERE FURI_DATE_K     = " & SQ(KeyFuriDate))
            SQL.Append("   AND TORIS_CODE_K    = " & SQ(KeyToriSCode))
            SQL.Append("   AND TORIF_CODE_K    = " & SQ(KeyToriFCode))
            SQL.Append("   AND DATA_KBN_K      = '2'")
            SQL.Append("   AND FURIKETU_CODE_K <> 0")
            SQL.Append("   AND KEIYAKU_KIN_K   = " & SQ(KeyKeiyakuKin))
        End If
        '2017/01/20 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
        If OraCntReader.DataReader(SQL) = True Then
            nFUNOU_KEN = OraCntReader.GetInt64("CNT1")
            nFUNOU_KIN = OraCntReader.GetInt64("CNT2")
        End If
        OraCntReader.Close()

        '----------------------
        '�U�֍ό����A���z�擾
        '----------------------
        SQL = New StringBuilder(128)
        '2017/01/20 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
        '�R�����g�A�E�g����Ă��������𕜊�������B
        If KeyKeiyakuKin = Jikinko Then
            ' �r�r�r����
            SQL.Append("SELECT COUNT(FURIKIN_K) CNT1,SUM(FURIKIN_K) CNT2")
            SQL.Append(" FROM MEIMAST")
            SQL.Append(" WHERE FURI_DATE_K     = " & SQ(KeyFuriDate))
            SQL.Append("   AND TORIS_CODE_K    = " & SQ(KeyToriSCode))
            SQL.Append("   AND TORIF_CODE_K    = " & SQ(KeyToriFCode))
            SQL.Append("   AND DATA_KBN_K      = '2'")
            '���s���͑ΏۊO�Ƃ���
            SQL.Append("   AND KEIYAKU_KIN_K <> " & SQ(Jikinko))
            SQL.Append("   AND FURIKETU_CODE_K = 0")
            '��g���A��g�O�̏����͕ύX����
            '�W����(�X���[�G�X���ϖ���)�͒�g���̂ݑΏ�
            SQL.Append("   AND EXISTS (")
            SQL.Append("   SELECT TEIKEI_KBN_N FROM TENMAST ")
            SQL.Append("    WHERE KIN_NO_N = KEIYAKU_KIN_K")
            SQL.Append("      AND SIT_NO_N = KEIYAKU_SIT_K")
            SQL.Append("      AND TEIKEI_KBN_N = '1'")
            SQL.Append("   )")
            'If KeyTeikeiKubun = "1" Then
            '    ' ��g��
            '    SQL.Append("   AND EXISTS (")
            'Else
            '    ' ��g�O
            '    SQL.Append("   AND NOT EXISTS (")
            'End If
            'SQL.Append("   SELECT TEIKEI_KBN_N FROM TENMAST ")
            'SQL.Append("    WHERE KIN_NO_N = KEIYAKU_KIN_K")
            'SQL.Append("      AND SIT_NO_N = KEIYAKU_SIT_K")
            'SQL.Append("      AND EDA_N = '01'")
            'SQL.Append("      AND TEIKEI_KBN_N = '1'")
            'SQL.Append("   )")
        Else
            ' ���s����
            SQL.Append("SELECT COUNT(FURIKIN_K) CNT1,SUM(FURIKIN_K) CNT2")
            SQL.Append(" FROM MEIMAST")
            SQL.Append(" WHERE FURI_DATE_K     = " & SQ(KeyFuriDate))
            SQL.Append("   AND TORIS_CODE_K = " & SQ(KeyToriSCode))
            SQL.Append("   AND TORIF_CODE_K = " & SQ(KeyToriFCode))
            SQL.Append("   AND DATA_KBN_K      = '2'")
            SQL.Append("   AND FURIKETU_CODE_K =  0")
            SQL.Append("   AND KEIYAKU_KIN_K   = " & SQ(KeyKeiyakuKin))
        End If
        '2017/01/20 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
        If OraCntReader.DataReader(SQL) = True Then
            nFURI_KEN = OraCntReader.GetInt64("CNT1")
            nFURI_KIN = OraCntReader.GetInt64("CNT2")
        End If
        OraCntReader.Close()

        SQL = New StringBuilder(128)
        SQL.Append("UPDATE TAKOSCHMAST")
        SQL.Append(" SET")
        SQL.Append(" FUNOU_FLG_U = '1'")
        SQL.Append(",FURI_KEN_U  = " & nFURI_KEN.ToString)
        SQL.Append(",FURI_KIN_U  = " & nFURI_KIN.ToString)
        SQL.Append(",FUNOU_KEN_U = " & nFUNOU_KEN.ToString)
        SQL.Append(",FUNOU_KIN_U = " & nFUNOU_KIN.ToString)
        SQL.Append(" WHERE TORIS_CODE_U = " & SQ(KeyToriSCode))
        SQL.Append("   AND TORIF_CODE_U = " & SQ(KeyToriFCode))
        SQL.Append("   AND FURI_DATE_U  = " & SQ(KeyFuriDate))
        SQL.Append("   AND TKIN_NO_U    = " & SQ(KeyKeiyakuKin))
        'SQL.Append("   AND TEIKEI_KBN_U = " & SQ(KeyTeikeiKubun))

        Try
            Dim nCount As Integer = MainDB.ExecuteNonQuery(SQL)
            MainLOG.Write("���s�X�P�W���[���X�V", "����", "���Z�@�փR�[�h�F" & KeyKeiyakuKin & " �X�V����:" & nCount.ToString)
        Catch ex As Exception
            MainLOG.Write("���s�X�P�W���[���X�V", "���s", "���Z�@�փR�[�h�F" & KeyKeiyakuKin)
            MainLOG.UpdateJOBMASTbyErr("���s�X�P�W���[���X�V���s ���Z�@�փR�[�h�F" & KeyKeiyakuKin)
            Return False
        End Try

        '2017/01/20 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
        '�X���[�G�X�̏ꍇ�A�W����(�X���[�G�X���ϖ���)�͑��s�t���O�̍X�V�s�v
        If KeyKeiyakuKin = Jikinko Then
            Return True
        End If
        '2017/01/20 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END

        ' ���s�t���O�X�V
        SQL = New StringBuilder(128)
        SQL.Append("UPDATE SCHMAST")
        SQL.Append(" SET")
        SQL.Append(" TAKOU_FLG_S = '1'")
        SQL.Append(" WHERE TORIS_CODE_S = " & SQ(KeyToriSCode))
        SQL.Append("   AND TORIF_CODE_S = " & SQ(KeyToriFCode))
        SQL.Append("   AND FURI_DATE_S  = " & SQ(KeyFuriDate))
        SQL.Append("   AND NOT EXISTS")
        SQL.Append("(SELECT TORIS_CODE_U ")
        SQL.Append("  FROM TAKOSCHMAST")
        SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
        SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
        SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
        SQL.Append("   AND FUNOU_FLG_U  = '0'")             ' �s�\������������
        SQL.Append("   AND SYORI_KEN_U  <> 0")
        SQL.Append("   AND SYORI_KIN_U  <> 0")
        SQL.Append(")")
        SQL.Append("   AND EXISTS")
        SQL.Append("(SELECT TORIS_CODE_U ")
        SQL.Append("  FROM TAKOSCHMAST")
        SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
        SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
        SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
        SQL.Append("   AND FUNOU_FLG_U  = '1'")             ' �s�\������������
        SQL.Append("   AND SYORI_KEN_U  <> 0")
        SQL.Append("   AND SYORI_KIN_U  <> 0")
        SQL.Append(")")

        Try
            Dim nCount As Integer = MainDB.ExecuteNonQuery(SQL)
            MainLOG.Write("�X�P�W���[�� ���s�t���O�X�V", "����", "�X�V����:" & nCount.ToString)
        Catch ex As Exception
            MainLOG.Write("�X�P�W���[�� ���s�t���O�X�V", "���s", ex.Message)
            MainLOG.UpdateJOBMASTbyErr("���s�X�P�W���[���X�V���s")
            Return False
        End Try

        Return True
    End Function

#End Region

#Region " �w�Z�X�P�W���[���}�X�^�X�V"
    ' �@�\�@ �F �w�Z�X�P�W���[���}�X�^�X�V
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateG_SCHMAST(ByVal keys() As String) As Boolean
        Dim SQL As New StringBuilder(128)

        '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
        'Dim OraSchReader As New CASTCommon.MyOracleReader(MainDB)
        'Dim OraCount As New CASTCommon.MyOracleReader(MainDB)
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        Dim OraSchReader As CASTCommon.MyOracleReader = Nothing
        Dim OraCount As CASTCommon.MyOracleReader = Nothing
        '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Try
            OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
            OraSchReader = New CASTCommon.MyOracleReader(MainDB)
            OraCount = New CASTCommon.MyOracleReader(MainDB)
            '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

            Dim KeyToriSCode As String = ""
            Dim KeyToriFCode As String = ""
            Dim KeyFuriDate As String = ""

            If keys.Length = 1 Then
                KeyFuriDate = keys(0)
            Else
                KeyToriSCode = keys(0)
                KeyToriFCode = keys(1)
                KeyFuriDate = keys(2)

                MainLOG.ToriCode = KeyToriSCode & KeyToriFCode
                MainLOG.FuriDate = KeyFuriDate
            End If

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_S")
            SQL.Append(",TORIF_CODE_S")
            SQL.Append(",FURI_DATE_S")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(" WHERE FURI_DATE_S  = " & SQ(KeyFuriDate))
            SQL.Append("   AND HAISIN_FLG_S= '1'")
            SQL.Append("   AND TYUUDAN_FLG_S= '0'")
            SQL.Append("   AND BAITAI_CODE_S= '07' ")
            If OraSchReader.DataReader(SQL) = True Then
                Do While OraSchReader.EOF = False

                    ' �w�Z���׃}�X�^�X�V
                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT ")
                    SQL.Append(" TORIS_CODE_K")
                    SQL.Append(",TORIF_CODE_K")
                    SQL.Append(",FURI_DATE_K")
                    SQL.Append(",JYUYOUKA_NO_K")
                    SQL.Append(",KEIYAKU_KAMOKU_K")
                    SQL.Append(",KEIYAKU_KIN_K")
                    SQL.Append(",KEIYAKU_SIT_K")
                    SQL.Append(",KEIYAKU_KOUZA_K")
                    SQL.Append(",FURIKETU_CODE_K")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE FURIKETU_CODE_K <> 0 ")
                    SQL.Append("   AND DATA_KBN_K   = '2'")
                    SQL.Append("   AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    SQL.Append("   AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                    SQL.Append("   AND FURI_DATE_K  = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                    If OraMeiReader.DataReader(SQL) = True Then

                        Do While OraMeiReader.EOF = False

                            '----------------------------
                            '�w�Z���U���׃}�X�^���猟��
                            '----------------------------
                            Dim strJYUYOUKA_NO As String, strKAMOKU As String

                            If OraMeiReader.GetString("JYUYOUKA_NO_K").TrimEnd.Length >= 20 Then
                                strJYUYOUKA_NO = OraMeiReader.GetString("JYUYOUKA_NO_K").Substring(0, 20)
                            Else
                                strJYUYOUKA_NO = OraMeiReader.GetString("JYUYOUKA_NO_K")
                            End If
                            strKAMOKU = OraMeiReader.GetString("KEIYAKU_KAMOKU_K")

                            SQL = New StringBuilder(128)
                            SQL.Append("UPDATE G_MEIMAST")
                            SQL.Append(" SET FURIKETU_CODE_M = " & OraMeiReader.GetInt("FURIKETU_CODE_K").ToString)
                            SQL.Append(" WHERE GAKKOU_CODE_M = " & SQ(OraMeiReader.GetString("TORIS_CODE_K")))
                            SQL.Append("   AND FURI_DATE_M   = " & SQ(OraMeiReader.GetString("FURI_DATE_K")))
                            SQL.Append("   AND TKIN_NO_M     = " & SQ(OraMeiReader.GetString("KEIYAKU_KIN_K")))
                            SQL.Append("   AND TSIT_NO_M     = " & SQ(OraMeiReader.GetString("KEIYAKU_SIT_K")))
                            SQL.Append("   AND TKAMOKU_M     = " & SQ(strKAMOKU))
                            SQL.Append("   AND TKOUZA_M      = " & SQ(OraMeiReader.GetString("KEIYAKU_KOUZA_K")))
                            SQL.Append("   AND JUYOUKA_NO_M  = " & SQ(strJYUYOUKA_NO))

                            Try
                                Dim nCount As Integer
                                nCount = MainDB.ExecuteNonQuery(SQL)
                                If nCount = 0 Then
                                    MainLOG.Write("�w�Z���׌���", "���s", "�w�Z�R�[�h�F" & OraMeiReader.GetString("TORIS_CODE_K") _
                                                & " �U�֓��F" & OraMeiReader.GetString("FURI_DATE_K") _
                                                & " ���v�Ɣԍ��F" & strJYUYOUKA_NO _
                                                & " ���Z�@�ցF" & OraMeiReader.GetString("KEIYAKU_KIN_K") _
                                                & " �x�X�F" & OraMeiReader.GetString("KEIYAKU_SIT_K") _
                                                & " �ȖځF" & strKAMOKU _
                                                & " �����ԍ��F" & OraMeiReader.GetString("KEIYAKU_KOUZA_K"))

                                    MainLOG.UpdateJOBMASTbyErr("�w�Z���׌��� �w�Z�R�[�h�F" & OraMeiReader.GetString("TORIS_CODE_K") _
                                        & " �U�֓��F" & OraMeiReader.GetString("FURI_DATE_K") _
                                        & " ���v�Ɣԍ��F" & strJYUYOUKA_NO)
                                    Return False
                                End If

                            Catch ex As Exception
                                MainLOG.Write("�w�Z���׍X�V", "���s", "�w�Z�R�[�h�F" & OraMeiReader.GetString("TORIS_CODE_K") _
                                            & " �U�֓��F" & OraMeiReader.GetString("FURI_DATE_K") _
                                            & " ���v�Ɣԍ��F" & strJYUYOUKA_NO _
                                            & " ���Z�@�ցF" & OraMeiReader.GetString("KEIYAKU_KIN_K") _
                                            & " �x�X�F" & OraMeiReader.GetString("KEIYAKU_SIT_K") _
                                            & " �ȖځF" & strKAMOKU _
                                            & " �����ԍ��F" & OraMeiReader.GetString("KEIYAKU_KOUZA_K") _
                                            & " " & ex.Message & ":" & ex.StackTrace)
                                MainLOG.UpdateJOBMASTbyErr("�w�Z���׍X�V �w�Z�R�[�h�F" & OraMeiReader.GetString("TORIS_CODE_K") _
                                        & " �U�֓��F" & OraMeiReader.GetString("FURI_DATE_K") _
                                        & " ���v�Ɣԍ��F" & strJYUYOUKA_NO)
                                Return False
                            End Try

                            OraMeiReader.NextRead()
                        Loop
                    Else
                        MainLOG.Write("�w�Z���׍X�V ���וs�\���ׂȂ�", "����", "�����R�[�h�F" _
                                    & OraSchReader.GetString("TORIS_CODE_S") _
                                    & "-" & OraSchReader.GetString("TORIF_CODE_S") _
                                    & " �U�֓��F" & OraSchReader.GetString("FURI_DATE_S"))

                    End If
                    OraMeiReader.Close()

                    ' �w�Z�X�P�W���[���X�V
                    '2017/03/14 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
                    '���ʐU�֓��Ή�
                    '���ʃX�P�W���[���͊w�N�ƍĐU����ς��邱�ƂŁA����U�֓��ɕ������R�[�h�쐬���邱�Ƃ��ł��邽�߁A
                    '�w�N�ɕR�t���w�Z�X�P�W���[���}�X�^���X�V����悤�ɏC������

                    '--------------------------------------------------
                    '�w�Z�X�P�W���[���}�X�^�̃��R�[�h�������o
                    '--------------------------------------------------
                    Dim MultiGScheduleFlg As Boolean = True

                    With SQL
                        .Length = 0
                        .Append("select count(*) as COUNTER from G_SCHMAST")
                        .Append(" where GAKKOU_CODE_S = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                        .Append(" and FURI_DATE_S = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                        .Append(" and FURI_KBN_S = " & SQ((CAInt32(OraSchReader.GetString("TORIF_CODE_S")) - 1).ToString))
                        .Append(" and TYUUDAN_FLG_S = '0'")
                    End With

                    If OraCount.DataReader(SQL) = True Then
                        '�X�P�W���[����1���R�[�h
                        If OraCount.GetInt("COUNTER") = 1 Then
                            MultiGScheduleFlg = False
                        End If
                    End If

                    OraCount.Close()

                    '�w�Z�X�P�W���[���}�X�^������U�֓��ɕ������R�[�h���݂��Ȃ��ꍇ�́A�����̏������s��
                    If MultiGScheduleFlg = False Then

                        '-----------------------------------------------
                        '�w�Z���U�U�֍ς݌����A���z�A�s�\�����A���z�̎擾
                        '-----------------------------------------------
                        Dim dblGFURI_KEN As Decimal = 0
                        Dim dblGFURI_KIN As Decimal = 0
                        Dim dblGFUNOU_KEN As Decimal = 0
                        Dim dblGFUNOU_KIN As Decimal = 0
                        dblGFURI_KEN = 0
                        dblGFURI_KIN = 0
                        dblGFUNOU_KEN = 0
                        dblGFUNOU_KIN = 0
                        SQL = New StringBuilder(128)
                        SQL.Append("SELECT COUNT(KEIYAKU_KNAME_K) CNT1,SUM(FURIKIN_K) CNT2 FROM MEIMAST")
                        SQL.Append(" WHERE FURI_DATE_K = " & SQ(KeyFuriDate))
                        SQL.Append("  AND FURIKETU_CODE_K = 0")
                        SQL.Append("  AND DATA_KBN_K = '2'")
                        SQL.Append("  AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                        SQL.Append("  AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                        If OraCount.DataReader(SQL) = True Then
                            dblGFURI_KEN = OraCount.GetInt64("CNT1")
                            dblGFURI_KIN = OraCount.GetInt64("CNT2")
                        End If
                        OraCount.Close()

                        SQL = New StringBuilder(128)
                        SQL.Append("SELECT COUNT(KEIYAKU_KNAME_K) CNT1,SUM(FURIKIN_K) CNT2 FROM MEIMAST")
                        SQL.Append(" WHERE FURI_DATE_K = " & SQ(KeyFuriDate))
                        SQL.Append("   AND FURIKETU_CODE_K <> 0")
                        SQL.Append("   AND DATA_KBN_K = '2'")
                        SQL.Append("   AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                        SQL.Append("   AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                        If OraCount.DataReader(SQL) = True Then
                            dblGFUNOU_KEN = OraCount.GetInt64("CNT1")
                            dblGFUNOU_KIN = OraCount.GetInt64("CNT2")
                        End If
                        OraCount.Close()

                        '-------------------------------------------
                        '�w�Z���U�X�P�W���[���}�X�^�̍X�V
                        '-------------------------------------------
                        SQL = New StringBuilder(128)
                        SQL.Append("UPDATE G_SCHMAST SET")
                        SQL.Append(" FUNOU_FLG_S = '1'")
                        SQL.Append(",FURI_KEN_S  = " & dblGFURI_KEN.ToString)
                        SQL.Append(",FURI_KIN_S = " & dblGFURI_KIN.ToString)
                        SQL.Append(",FUNOU_KEN_S = " & dblGFUNOU_KEN.ToString)
                        SQL.Append(",FUNOU_KIN_S = " & dblGFUNOU_KIN)
                        SQL.Append(",FUNOU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                        SQL.Append(" WHERE ")
                        SQL.Append("     FURI_DATE_S   = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                        SQL.Append(" AND GAKKOU_CODE_S = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                        SQL.Append(" AND TYUUDAN_FLG_S = '0'")
                        SQL.Append(" AND FURI_KBN_S    = " & SQ((CAInt32(OraSchReader.GetString("TORIF_CODE_S")) - 1).ToString))

                        Try
                            Dim nCount As Integer = MainDB.ExecuteNonQuery(SQL)
                        Catch ex As Exception
                            MainLOG.Write("�w�Z�X�P�W���[���X�V", "���s", SQL.ToString & " " & ex.Message & ":" & ex.StackTrace)
                            MainLOG.UpdateJOBMASTbyErr("�w�Z�X�P�W���[���X�V �w�Z�R�[�h�F" & OraSchReader.GetString("TORIS_CODE_S") _
                                    & " �U�֓��F" & OraSchReader.GetString("TORIS_CODE_S"))
                            Return False
                        End Try


                    Else
                        '============================================================
                        '����U�֓��ɕ������R�[�h���݂���ꍇ
                        '============================================================

                        '--------------------------------------------------
                        '�w�Z�X�P�W���[���}�X�^���o
                        '--------------------------------------------------
                        With SQL
                            .Length = 0
                            .Append("select ")
                            .Append(" GAKKOU_CODE_S")
                            .Append(",FURI_DATE_S")
                            .Append(",FURI_KBN_S")
                            .Append(",GAKUNEN1_FLG_S")
                            .Append(",GAKUNEN2_FLG_S")
                            .Append(",GAKUNEN3_FLG_S")
                            .Append(",GAKUNEN4_FLG_S")
                            .Append(",GAKUNEN5_FLG_S")
                            .Append(",GAKUNEN6_FLG_S")
                            .Append(",GAKUNEN7_FLG_S")
                            .Append(",GAKUNEN8_FLG_S")
                            .Append(",GAKUNEN9_FLG_S")
                            .Append(" from G_SCHMAST")
                            .Append(" where GAKKOU_CODE_S = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                            .Append(" and FURI_DATE_S = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                            .Append(" and FURI_KBN_S = " & SQ((CAInt32(OraSchReader.GetString("TORIF_CODE_S")) - 1).ToString))
                            .Append(" and TYUUDAN_FLG_S = '0'")
                        End With

                        If OraCount.DataReader(SQL) = True Then
                            While OraCount.EOF = False

                                Dim dblGFURI_KEN As Decimal = 0
                                Dim dblGFURI_KIN As Decimal = 0
                                Dim dblGFUNOU_KEN As Decimal = 0
                                Dim dblGFUNOU_KIN As Decimal = 0

                                '--------------------------------------------------
                                '�w�N�t���O�������Ă��閾�ׂɑ΂��ďW�v���s��
                                '--------------------------------------------------
                                For i As Integer = 1 To 9
                                    If OraCount.GetString("GAKUNEN" & i.ToString & "_FLG_S") = "1" Then
                                        With SQL
                                            .Length = 0
                                            .Append("select ")
                                            .Append(" sum(decode(FURIKETU_CODE_M, 0, 1, 0)) as FURI_KEN")
                                            .Append(",sum(decode(FURIKETU_CODE_M, 0, SEIKYU_KIN_M)) as FURI_KIN")
                                            .Append(",sum(decode(FURIKETU_CODE_M, 0, 0, 1)) as FUNO_KEN")
                                            .Append(",sum(decode(FURIKETU_CODE_M, 0, 0, SEIKYU_KIN_M)) as FUNO_KIN")
                                            .Append(" from G_MEIMAST")
                                            .Append(" where GAKKOU_CODE_M = " & SQ(OraCount.GetString("GAKKOU_CODE_S")))
                                            .Append(" and FURI_DATE_M = " & SQ(OraCount.GetString("FURI_DATE_S")))
                                            .Append(" and FURI_KBN_M = " & SQ(OraCount.GetString("FURI_KBN_S")))
                                            .Append(" and GAKUNEN_CODE_M = " & i.ToString)
                                        End With

                                        OraMeiReader.Close()
                                        If OraMeiReader.DataReader(SQL) = True Then
                                            dblGFURI_KEN += OraMeiReader.GetInt("FURI_KEN")
                                            dblGFURI_KIN += OraMeiReader.GetInt64("FURI_KIN")
                                            dblGFUNOU_KEN += OraMeiReader.GetInt("FUNO_KEN")
                                            dblGFUNOU_KIN += OraMeiReader.GetInt64("FUNO_KIN")
                                        End If
                                    End If
                                Next

                                '--------------------------------------------------
                                '�w�Z�X�P�W���[���}�X�^�X�V
                                '--------------------------------------------------
                                With SQL
                                    .Length = 0
                                    .Append("update G_SCHMAST set")
                                    .Append(" FUNOU_FLG_S = '1'")
                                    .Append(",FURI_KEN_S = " & dblGFURI_KEN.ToString)
                                    .Append(",FURI_KIN_S = " & dblGFURI_KIN.ToString)
                                    .Append(",FUNOU_KEN_S = " & dblGFUNOU_KEN.ToString)
                                    .Append(",FUNOU_KIN_S = " & dblGFUNOU_KIN.ToString)
                                    .Append(",FUNOU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                                    .Append(" where GAKKOU_CODE_S = " & SQ(OraCount.GetString("GAKKOU_CODE_S")))
                                    .Append(" and FURI_DATE_S = " & SQ(OraCount.GetString("FURI_DATE_S")))
                                    .Append(" and FURI_KBN_S = " & SQ(OraCount.GetString("FURI_KBN_S")))
                                    .Append(" and GAKUNEN1_FLG_S = " & SQ(OraCount.GetString("GAKUNEN1_FLG_S")))
                                    .Append(" and GAKUNEN2_FLG_S = " & SQ(OraCount.GetString("GAKUNEN2_FLG_S")))
                                    .Append(" and GAKUNEN3_FLG_S = " & SQ(OraCount.GetString("GAKUNEN3_FLG_S")))
                                    .Append(" and GAKUNEN4_FLG_S = " & SQ(OraCount.GetString("GAKUNEN4_FLG_S")))
                                    .Append(" and GAKUNEN5_FLG_S = " & SQ(OraCount.GetString("GAKUNEN5_FLG_S")))
                                    .Append(" and GAKUNEN6_FLG_S = " & SQ(OraCount.GetString("GAKUNEN6_FLG_S")))
                                    .Append(" and GAKUNEN7_FLG_S = " & SQ(OraCount.GetString("GAKUNEN7_FLG_S")))
                                    .Append(" and GAKUNEN8_FLG_S = " & SQ(OraCount.GetString("GAKUNEN8_FLG_S")))
                                    .Append(" and GAKUNEN9_FLG_S = " & SQ(OraCount.GetString("GAKUNEN9_FLG_S")))
                                End With

                                Try
                                    Dim nCount As Integer = MainDB.ExecuteNonQuery(SQL)
                                Catch ex As Exception
                                    MainLOG.Write("�w�Z�X�P�W���[���X�V", "���s", SQL.ToString & " " & ex.Message & ":" & ex.StackTrace)
                                    MainLOG.UpdateJOBMASTbyErr("�w�Z�X�P�W���[���X�V �w�Z�R�[�h�F" & OraSchReader.GetString("TORIS_CODE_S") _
                                            & " �U�֓��F" & OraSchReader.GetString("TORIS_CODE_S"))
                                    Return False
                                End Try

                                OraCount.NextRead()
                            End While
                        End If

                        OraCount.Close()

                    End If

                    ''-----------------------------------------------
                    ''�w�Z���U�U�֍ς݌����A���z�A�s�\�����A���z�̎擾
                    ''-----------------------------------------------
                    'Dim dblGFURI_KEN As Decimal = 0
                    'Dim dblGFURI_KIN As Decimal = 0
                    'Dim dblGFUNOU_KEN As Decimal = 0
                    'Dim dblGFUNOU_KIN As Decimal = 0
                    'dblGFURI_KEN = 0
                    'dblGFURI_KIN = 0
                    'dblGFUNOU_KEN = 0
                    'dblGFUNOU_KIN = 0
                    'SQL = New StringBuilder(128)
                    'SQL.Append("SELECT COUNT(KEIYAKU_KNAME_K) CNT1,SUM(FURIKIN_K) CNT2 FROM MEIMAST")
                    'SQL.Append(" WHERE FURI_DATE_K = " & SQ(KeyFuriDate))
                    'SQL.Append("  AND FURIKETU_CODE_K = 0")
                    'SQL.Append("  AND DATA_KBN_K = '2'")
                    'SQL.Append("  AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    'SQL.Append("  AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                    'If OraCount.DataReader(SQL) = True Then
                    '    dblGFURI_KEN = OraCount.GetInt64("CNT1")
                    '    dblGFURI_KIN = OraCount.GetInt64("CNT2")
                    'End If
                    'OraCount.Close()

                    'SQL = New StringBuilder(128)
                    'SQL.Append("SELECT COUNT(KEIYAKU_KNAME_K) CNT1,SUM(FURIKIN_K) CNT2 FROM MEIMAST")
                    'SQL.Append(" WHERE FURI_DATE_K = " & SQ(KeyFuriDate))
                    'SQL.Append("   AND FURIKETU_CODE_K <> 0")
                    'SQL.Append("   AND DATA_KBN_K = '2'")
                    'SQL.Append("   AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    'SQL.Append("   AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                    'If OraCount.DataReader(SQL) = True Then
                    '    dblGFUNOU_KEN = OraCount.GetInt64("CNT1")
                    '    dblGFUNOU_KIN = OraCount.GetInt64("CNT2")
                    'End If
                    'OraCount.Close()

                    ''-------------------------------------------
                    ''�w�Z���U�X�P�W���[���}�X�^�̍X�V
                    ''-------------------------------------------
                    'SQL = New StringBuilder(128)
                    'SQL.Append("UPDATE G_SCHMAST SET")
                    'SQL.Append(" FUNOU_FLG_S = '1'")
                    'SQL.Append(",FURI_KEN_S  = " & dblGFURI_KEN.ToString)
                    'SQL.Append(",FURI_KIN_S = " & dblGFURI_KIN.ToString)
                    'SQL.Append(",FUNOU_KEN_S = " & dblGFUNOU_KEN.ToString)
                    'SQL.Append(",FUNOU_KIN_S = " & dblGFUNOU_KIN)
                    'SQL.Append(",FUNOU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    'SQL.Append(" WHERE ")
                    'SQL.Append("     FURI_DATE_S   = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                    'SQL.Append(" AND GAKKOU_CODE_S = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    'SQL.Append(" AND TYUUDAN_FLG_S = '0'")
                    'SQL.Append(" AND FURI_KBN_S    = " & SQ((CAInt32(OraSchReader.GetString("TORIF_CODE_S")) - 1).ToString))

                    'Try
                    '    Dim nCount As Integer = MainDB.ExecuteNonQuery(SQL)
                    'Catch ex As Exception
                    '    MainLOG.Write("�w�Z�X�P�W���[���X�V", "���s", SQL.ToString & " " & ex.Message & ":" & ex.StackTrace)
                    '    MainLOG.UpdateJOBMASTbyErr("�w�Z�X�P�W���[���X�V �w�Z�R�[�h�F" & OraSchReader.GetString("TORIS_CODE_S") _
                    '            & " �U�֓��F" & OraSchReader.GetString("TORIS_CODE_S"))
                    '    Return False
                    'End Try
                    '2017/03/14 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END

                    OraSchReader.NextRead()
                Loop

                OraSchReader.Close()

            Else
                MainLOG.Write("�w�Z�X�P�W���[���Ώۃ��R�[�h�Ȃ�", "����", "�z�M�ςȂ��C�t�H�[�}�b�g�敪�F07�C�U�֓��F" & KeyFuriDate)
            End If

            Return True

            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Finally
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If

            If Not OraSchReader Is Nothing Then
                OraSchReader.Close()
            End If

            If Not OraCount Is Nothing Then
                OraCount.Close()
            End If
        End Try
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

    End Function

#End Region

#Region " �}�̓ǂݍ���"
    Public Function fn_BAITAI_READ() As Boolean
        '============================================================================
        'NAME           :fn_BAITAI_READ
        'Parameter      :
        'Description    :��Ǝ����}�̓ǂݍ��ݏ���
        'Return         :True=OK(����),False=NG�i���s�j
        'Create         :2009/09/12
        'Update         :
        '============================================================================
        fn_BAITAI_READ = False
        Try

            Dim Set_Code As String = ""
            If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                Set_Code = mArgumentData.INFOToriMast.TORIS_CODE_T + mArgumentData.INFOToriMast.TORIF_CODE_T
                strIN_FILE_NAME = DEN_Folder & "R" & Set_Code & ".DAT"
                strOUT_FILE_NAME = DAT_Folder & "R" & Set_Code & "_JIS.DAT"
            Else
                Set_Code = mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T
                strIN_FILE_NAME = DEN_Folder & "R" & Set_Code & ".DAT"
                strOUT_FILE_NAME = DAT_Folder & "R" & Set_Code & "_JIS.DAT"
            End If

            '�}�̓Ǎ�
            strP_FILE = "120.P"
            intREC_LENGTH = 120

            '�`���̂�
            Select Case clsFUSION.fn_DISK_CPYTO_DEN(MainLOG.ToriCode, strIN_FILE_NAME, strOUT_FILE_NAME, _
                                               intREC_LENGTH, "0", strP_FILE)
                'Return         :0=�����A100=�R�[�h�ϊ����s�A200=�R�[�h�敪�ُ�iJIS���s����j�A
                '               :300=�R�[�h�敪�ُ�iJIS���s�Ȃ��j�A400=�o�̓t�@�C���쐬���s
                Case 0
                    fn_BAITAI_READ = True
                    MainLOG.Write("(�`���t�@�C���捞)", "����", "�t�@�C����:" & strOUT_FILE_NAME)
                Case 100
                    fn_BAITAI_READ = False
                    MainLOG.Write("(�`���t�@�C���捞)", "���s", "�R�[�h�ϊ����s:" & Microsoft.VisualBasic.Err.Description)
                    Exit Function
                Case 200
                    fn_BAITAI_READ = False
                    MainLOG.Write("(�`���t�@�C���捞)", "���s", "�R�[�h�敪�ُ�:" & Microsoft.VisualBasic.Err.Description)
                    Exit Function
                Case 300
                    fn_BAITAI_READ = False
                    MainLOG.Write("(�`���t�@�C���捞)", "���s", "�R�[�h�敪�ُ�iJIS���s�Ȃ��j:" & Microsoft.VisualBasic.Err.Description)
                    Exit Function
                Case 400
                    fn_BAITAI_READ = False
                    MainLOG.Write("(�`���t�@�C���捞)", "���s", "�o�̓t�@�C���쐬:" & Microsoft.VisualBasic.Err.Description)
                    Exit Function
            End Select
            fn_BAITAI_READ = True
        Catch ex As Exception

        End Try
    End Function

    Public Function fn_TAKO_BAITAI_READ() As Boolean
        '============================================================================
        'NAME           :fn_BAITAI_READ
        'Parameter      :
        'Description    :�}�̓ǂݍ��ݏ���
        'Return         :True=OK(����),False=NG�i���s�j
        'Create         :2009/09/12
        'Update         :
        '============================================================================
        fn_TAKO_BAITAI_READ = False

        Try
            '�����p�p�����[�^�ݒ�
            strTORIS_CODE = Mid(mKoParam.CP.TORI_CODE, 1, 10)
            strTORIF_CODE = Mid(mKoParam.CP.TORI_CODE, 11, 2)
            strTAKOU_KIN = mKoParam.TKIN_CODE

            Dim intKEKKA As Integer
            Dim strKEKKA As String
            Dim SQL As New StringBuilder(128)
            '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            'Dim oraReader As New CASTCommon.MyOracleReader
            Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
            '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            Dim strFMT_KBN As String
            SQL.Append("SELECT * ")
            SQL.Append(" FROM TAKOSCHMAST,TAKOUMAST")
            SQL.Append(" WHERE TORIS_CODE_U = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_U = " & SQ(strTORIF_CODE))
            SQL.Append(" AND FURI_DATE_U = " & SQ(mKoParam.CP.FURI_DATE))
            If Trim(strTAKOU_KIN) = MatomeNoukyo Then
                SQL.Append(" AND TKIN_NO_U BETWEEN " & SQ(Noukyo_From) & " AND " & SQ(Noukyo_To))
            Else
                SQL.Append(" AND TKIN_NO_U = " & SQ(strTAKOU_KIN))
            End If
            SQL.Append(" AND TORIS_CODE_U = TORIS_CODE_V")
            SQL.Append(" AND TORIF_CODE_U = TORIF_CODE_V")
            SQL.Append(" AND TKIN_NO_U = TKIN_NO_V")
            If oraReader.DataReader(SQL) Then
                strLABEL_CODE = CType(oraReader.GetString("LABEL_CODE_U"), Short)
                strTAKO_BAITAI_CODE = oraReader.GetString("BAITAI_CODE_U")
                strFMT_KBN = oraReader.GetString("FMT_KBN_U")
                strCODE_KBN = oraReader.GetString("CODE_KBN_U")
                strR_FILE_NAME = oraReader.GetString("RFILE_NAME_V")
                oraReader.Close()
            Else
                '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                oraReader.Close()
                '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                '���s�}�X�^���o�^
                MainLOG.Write("(���s�X�P�W���[���}�X�^�擾)", "���s", "���s�X�P�W���[���}�X�^���o�^:" & strTAKOU_KIN)
            End If

            Select Case mArgumentData.INFOParameter.FMT_KBN
                Case "00" '�S��
                    intREC_LENGTH = 120
                    intBLK_SIZE = 1800
                    strP_FILE = "120.P"
                Case Else   '2009/12/18 �ǉ�
                    MainLOG.Write("�t�H�[�}�b�g�敪����", "���s", "�t�H�[�}�b�g�敪�ُ�:" & mArgumentData.INFOToriMast.FMT_KBN_T)
                    Exit Function
            End Select

            '2017/05/25 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j-------------------------- START
            If mKoParam.KOUSIN_KBN = "1" OrElse strTAKO_BAITAI_CODE = "04" Then
                '�t�@�C����������������
                strIN_FILE_NAME = ""
                Return True '�����X�V/�˗����͂��̂܂ܐ���I��
            End If
            'If mKoParam.KOUSIN_KBN = "1" OrElse strTAKO_BAITAI_CODE = "04" Then Return True '�����X�V/�˗����͂��̂܂ܐ���I��
            '2017/05/25 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j-------------------------- END

            strIN_FILE_NAME = DEN_Folder & strR_FILE_NAME.Trim     '���̓t�@�C��
            If Not Directory.Exists(TAK_Folder & strTAKOU_KIN) Then
                Directory.CreateDirectory(TAK_Folder & strTAKOU_KIN)
            End If
            strOUT_FILE_NAME = TAK_Folder & strTAKOU_KIN & "\F" & strTAKOU_KIN & strTORIS_CODE & strTORIF_CODE & System.DateTime.Today.ToString("yyyyMMddhhmmdd") & ".dat"   '�o�̓t�@�C��

            strBKUP_FILE = DENBK_Folder & strR_FILE_NAME.Trim.PadLeft(1, " "c)  '�o�b�N�A�b�v�t�@�C��

            Dim Baitai As String = ""
            If (strTAKO_BAITAI_CODE = "05" AndAlso gstrMT = "1") OrElse _
               (strTAKO_BAITAI_CODE = "06" AndAlso gstrCMT = "1") Then
                strIN_FILE_NAME = Path.Combine(DEN_Folder, "TAKOU\" & strTAKOU_KIN)
                Baitai = "00"
            Else
                Baitai = strTAKO_BAITAI_CODE
            End If
            Select Case Baitai
                Case "00"        '�`��
                    intKEKKA = clsFUSION.fn_DEN_CPYTO_DISK(LW.ToriCode, strIN_FILE_NAME, strOUT_FILE_NAME, _
                                                           intREC_LENGTH, strCODE_KBN, strP_FILE, msgTitle)
                    Select Case intKEKKA
                        'Return         :0=�����A100=�R�[�h�ϊ����s�A200=�R�[�h�敪�ُ�iJIS���s����j�A
                        '               :300=�R�[�h�敪�ُ�iJIS���s�Ȃ��j�A400=�o�̓t�@�C���쐬���s
                        Case 0
                            fn_TAKO_BAITAI_READ = True
                            MainLOG.Write("(�`���t�@�C���捞)", "����", "�t�@�C����:" & strOUT_FILE_NAME)
                        Case 50
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(�`���t�@�C���捞)", "���s", "�t�@�C���Ȃ�:" & Err.Description)
                            Exit Function
                        Case 100
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(�`���t�@�C���捞)", "���s", Err.Description)
                            Exit Function
                        Case 200
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(�`���t�@�C���捞)", "���s", "�R�[�h�ϊ����s:" & Err.Description)
                            Exit Function
                        Case 300
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(�`���t�@�C���捞)", "���s", "�R�[�h�敪�ُ�iJIS���s�Ȃ��j:" & Err.Description)
                            Exit Function
                        Case 400
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(�`���t�@�C���捞)", "���s", "�o�̓t�@�C���쐬:" & Err.Description)
                            Exit Function
                    End Select

                Case "01"        '�e�c�R�D�T
                    strIN_FILE_NAME = strR_FILE_NAME.Trim
                    intKEKKA = clsFUSION.fn_FD_CPYTO_DISK(LW.ToriCode, strIN_FILE_NAME, strOUT_FILE_NAME, _
                                                          intREC_LENGTH, strCODE_KBN, strP_FILE, msgTitle)
                    Select Case intKEKKA
                        Case 0
                            fn_TAKO_BAITAI_READ = True
                            MainLOG.Write("(�e�c�捞)", "����", "�t�@�C����:" & strOUT_FILE_NAME)
                        Case 100
                            fn_TAKO_BAITAI_READ = False
                            'Return         :0=�����A100=�t�@�C���ǂݍ��ݎ��s�A�A200=�R�[�h�敪�ُ�iJIS���s����j�A
                            '               :300=�R�[�h�敪�ُ�iJIS���s�Ȃ��j�A400=�o�̓t�@�C���쐬���s
                            MainLOG.Write("(�e�c�捞)", "���s", "�R�[�h�ϊ�:" & Err.Description)
                            Exit Function
                        Case 200
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(�e�c�捞)", "���s", "�R�[�h�敪�ُ�iJIS���s����j:" & Err.Description)
                            Exit Function
                        Case 300
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(�e�c�捞)", "���s", "�R�[�h�敪�ُ�iJIS���s�Ȃ��j:" & Err.Description)
                            Exit Function
                        Case 400
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(�e�c�捞)", "���s", "�o�̓t�@�C���쐬:" & Err.Description)
                            Exit Function
                    End Select
                Case "05"        '�l�s
                    Select Case gstrMT
                        Case "0"     '�l�s�����ڎ��U�T�[�o�ɐڑ����Ă���ꍇ
                            strIN_FILE_NAME = " "
                            '2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
                            Dim lngErrStatus As Long
                            strKEKKA = vbDLL.mtCPYtoDISK(intBLK_SIZE, intREC_LENGTH, strLABEL_CODE, "SMLT", 1, 4, strIN_FILE_NAME, strOUT_FILE_NAME, 0, lngErrStatus)
                            'Dim lngErrStatus As Integer
                            'strKEKKA = vbDLL.mtCPYtoDISK(intBLK_SIZE, intREC_LENGTH, strLABEL_CODE, "SMLT", 1, 4, strIN_FILE_NAME, strOUT_FILE_NAME, 0, lngErrStatus)
                            '2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END
                            If strKEKKA <> "" Then
                                MainLOG.Write("(MT�捞)", "���s", Err.Description)
                                Exit Function
                            Else
                                MainLOG.Write("(MT�捞)", "����", "�t�@�C����:" & strOUT_FILE_NAME)
                            End If
                        Case "1"      '�l�s�����U�T�[�o�ɐڑ����Ă��Ȃ��ꍇ
                            If Dir(strMT_FUNOU_FILE) = "" Then
                                MainLOG.Write("(MT�捞)", "���s", "�t�@�C���Ȃ�:" & strMT_FUNOU_FILE)
                                Exit Function
                            End If
                            File.Copy(strMT_FUNOU_FILE, strOUT_FILE_NAME)
                            If Err.Number <> 0 Then
                                MainLOG.Write("(MT�捞)", "���s", "�t�@�C���R�s�[:" & Err.Description)
                                Exit Function
                            End If
                    End Select

                Case "06"        '�b�l�s
                    Select Case gstrCMT
                        Case "0"    '�b�l�s�����ڎ��U�T�[�o�ɐڑ����Ă���ꍇ
                            strIN_FILE_NAME = " "
                            '2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
                            Dim lngErrStatus As Long
                            strKEKKA = vbDLL.cmtCPYtoDISK(intBLK_SIZE, intREC_LENGTH, strLABEL_CODE, "SMLT", 1, 4, strIN_FILE_NAME, strOUT_FILE_NAME, 0, lngErrStatus)
                            'Dim lngErrStatus As Integer
                            'strKEKKA = vbDLL.cmtCPYtoDISK(intBLK_SIZE, intREC_LENGTH, strLABEL_CODE, "SMLT", 1, 4, strIN_FILE_NAME, strOUT_FILE_NAME, 0, lngErrStatus)
                            '2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END
                            If strKEKKA <> "" Then
                                MainLOG.Write("(CMT�捞)", "���s", Err.Description)
                                Exit Function
                            Else
                                MainLOG.Write("(CMT�捞)", "����", "�t�@�C����:" & strOUT_FILE_NAME)
                            End If
                        Case "1"    '�b�l�s�����U�T�[�o�ɐڑ����Ă��Ȃ��ꍇ
                            If Dir(strCMT_FUNOU_FILE) = "" Then
                                MainLOG.Write("(CMT�捞)", "���s", "�t�@�C������:" & strCMT_FUNOU_FILE)
                                Exit Function
                            End If
                            File.Copy(strCMT_FUNOU_FILE, strOUT_FILE_NAME)
                            If Err.Number <> 0 Then
                                MainLOG.Write("(MT�捞)", "���s", "�t�@�C���R�s�[:" & Err.Description)
                                Exit Function
                            End If
                    End Select
                Case "07"        '�w�Z���U
                Case "09"        '�`�[
                    fn_TAKO_BAITAI_READ = True
                Case Else
            End Select
        Catch ex As Exception
            strOUT_FILE_NAME = ""
            MainLOG.Write("(���s�}�̎捞)", "���s", ex.ToString)

        End Try
        fn_TAKO_BAITAI_READ = True

    End Function
#End Region

    ''' <summary>
    ''' �������ʊm�F�\(�s�\���ʍX�V)�̈���������s���܂��B�i�X���[�G�X�s�\���ʍX�V�p�j
    ''' </summary>
    ''' <param name="UpdateKeys"></param>
    ''' <returns></returns>
    ''' <remarks>2017/01/20 saitou ���t�M��(RSV2�W��) added for �X���[�G�X�Ή�</remarks>
    Private Function PrintSyoriKekkaKakuninhyoForSSS(ByVal UpdateKeys As ArrayList) As Boolean
        Dim dbReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim CreateCSV As New KFJP013
        Dim strCSV_FILE_NAME As String = String.Empty
        Dim strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
        Dim strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

        Try
            Dim strTorisCode As String
            Dim strTorifCode As String
            Dim strFuriDate As String

            '----------------------------------------
            '�������
            '----------------------------------------
            If INI_RSV2_SYORIKEKKA_FUNOU = "0" Then
                Return True
            End If

            '----------------------------------------
            '��������O�Ɉ����̃`�F�b�N
            '----------------------------------------
            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                If Keys.Length = 5 Then
                    'OK
                Else
                    '�X���[�G�X�̏ꍇ�A�X�P�W���[���X�V�L�[�̌�5�Ȃ̂ŁA�ُ�Ƃ���
                    MainLOG.Write("�������ʊm�F�\(�s�\���ʍX�V)���", "���s", "����L�[�ُ�")
                    Return False
                End If
            Next

            '----------------------------------------
            '����pCSV�쐬
            '----------------------------------------
            strCSV_FILE_NAME = CreateCSV.CreateCsvFile()
            dbReader = New CASTCommon.MyOracleReader(MainDB)

            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                strTorisCode = Keys(0)
                strTorifCode = Keys(1)
                strFuriDate = Keys(2)

                With SQL
                    .Length = 0
                    .Append("select")
                    .Append("     TORIS_CODE_S")
                    .Append("    ,TORIF_CODE_S")
                    .Append("    ,FURI_DATE_S")
                    .Append("    ,KIGYO_CODE_S")
                    .Append("    ,FURI_CODE_S")
                    .Append("    ,ITAKU_NNAME_T")
                    .Append("    ,ITAKU_CODE_T")
                    .Append("    ,FUNOU_FLG_S")
                    .Append("    ,SYORI_KEN_S")
                    .Append("    ,SYORI_KIN_S")
                    .Append("    ,FUNOU_KEN_S")
                    .Append("    ,FUNOU_KIN_S")
                    .Append(" from")
                    .Append("     SCHMAST")
                    .Append(" inner join")
                    .Append("     TORIMAST")
                    .Append(" on  TORIS_CODE_S = TORIS_CODE_T")
                    .Append(" and TORIF_CODE_S = TORIF_CODE_T")
                    .Append(" where")
                    .Append("     FURI_DATE_S = " & SQ(strFuriDate))
                    .Append(" and FUNOU_FLG_S in ('1', '2')")
                    .Append(" and TYUUDAN_FLG_S = '0'")
                    .Append(" and TORIS_CODE_S = " & SQ(strTorisCode))
                    .Append(" and TORIF_CODE_S = " & SQ(strTorifCode))
                    .Append(" order by")
                    .Append("     TORIS_CODE_S")
                    .Append("    ,TORIF_CODE_S")
                End With

                If dbReader.DataReader(SQL) = True Then
                    While dbReader.EOF = False
                        With CreateCSV
                            .OutputCsvData(strDate, True)                                       '�V�X�e�����t
                            .OutputCsvData(strTime, True)                                       '�^�C���X�^���v
                            .OutputCsvData(dbReader.GetString("FURI_DATE_S"), True)             '�U�֓�
                            .OutputCsvData(dbReader.GetString("TORIS_CODE_S"), True)            '������R�[�h
                            .OutputCsvData(dbReader.GetString("TORIF_CODE_S"), True)            '����敛�R�[�h
                            .OutputCsvData(dbReader.GetString("ITAKU_NNAME_T"), True)           '�ϑ��Җ�
                            .OutputCsvData(dbReader.GetString("ITAKU_CODE_T"), True)            '�ϑ��҃R�[�h
                            .OutputCsvData(dbReader.GetInt("SYORI_KEN_S").ToString)             '�˗�����
                            .OutputCsvData(dbReader.GetInt64("SYORI_KIN_S").ToString)           '�˗����z
                            .OutputCsvData(dbReader.GetInt("FUNOU_KEN_S").ToString)             '�s�\����
                            .OutputCsvData(dbReader.GetInt64("FUNOU_KIN_S").ToString)           '�s�\���z
                            Select Case dbReader.GetString("FUNOU_FLG_S")                       '���l
                                Case "1"
                                    .OutputCsvData("�s�\���ʍX�V����", True)
                                Case "2"
                                    .OutputCsvData("����������", True)
                            End Select
                            .OutputCsvData(dbReader.GetString("FURI_CODE_S"), True)             '�U�փR�[�h
                            .OutputCsvData(dbReader.GetString("KIGYO_CODE_S"), True, True)      '��ƃR�[�h
                        End With

                        dbReader.NextRead()
                    End While
                End If

                dbReader.Close()
            Next

            CreateCSV.CloseCsv()

            '----------------------------------------
            '����o�b�`�Ăяo��
            '----------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim execName As String = "KFJP013.EXE"
            Dim CmdArg As String = MainLOG.UserID & "," & strCSV_FILE_NAME
            Dim Ret As Integer
            Ret = ExeRepo.ExecReport(execName, CmdArg)
            Select Case Ret
                Case 0
                    MainLOG.Write("�������ʊm�F�\(�s�\���ʍX�V)���", "����")
                    Return True
                Case -1
                    MainLOG.Write("�������ʊm�F�\(�s�\���ʍX�V)���", "���s", "����Ώۂ�0��")
                    Return False
                Case Else
                    MainLOG.Write("�������ʊm�F�\(�s�\���ʍX�V)���", "���s", "������s")
                    MainLOG.UpdateJOBMASTbyErr("�������ʊm�F�\(�s�\���ʍX�V)������s")
                    Return False
            End Select

        Catch ex As Exception
            MainLOG.Write("�������ʊm�F�\(�s�\���ʍX�V)���", "���s", ex.Message)
            MainLOG.UpdateJOBMASTbyErr("�������ʊm�F�\(�s�\���ʍX�V)������s")
            Return False

        Finally
            If Not dbReader Is Nothing Then
                dbReader.Close()
                dbReader = Nothing
            End If
        End Try
    End Function

    Private Function CompressArray(ByVal arr As ArrayList) As String()
        Dim WorkArray As ArrayList = New ArrayList(arr.Count)

        Dim OldKey As String = ""
        For i As Integer = 0 To arr.Count - 1
            If OldKey <> CType(arr(i), String) Then
                WorkArray.Add(arr(i))
            End If
        Next i

        Return CType(WorkArray.ToArray(GetType(String)), String())
    End Function

    Public Class mySearchClass
        Implements IComparer

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
            Implements IComparer.Compare

            Dim xA() As String = CType(x, String())
            Dim yA() As String = CType(y, String())

            For i As Integer = 0 To xA.Length - 1
                If xA(i) <> yA(i) Then
                    Return -1
                End If
            Next i

            Return 0
        End Function 'IComparer.Compare
    End Class 'myReverserClass

    Public Class mySearchClass2
        Implements IComparer

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
            Implements IComparer.Compare

            Dim xA As String = CType(x, String)
            Dim yA As String = CType(y, String)

            Return String.Compare(xA, yA)
        End Function 'IComparer.Compare
    End Class 'myReverserClass

    Public Shared Sub PrintIndexAndValues(ByVal myList As IEnumerable)
        Dim i As Integer = 0
        Dim myEnumerator As System.Collections.IEnumerator = myList.GetEnumerator()
        While myEnumerator.MoveNext()
            Console.WriteLine(Microsoft.VisualBasic.ControlChars.Tab + "[{0}]:" + Microsoft.VisualBasic.ControlChars.Tab + "{1}", i, myEnumerator.Current)
            i = i + 1
        End While
        Console.WriteLine()
    End Sub 'PrintIndexAndValues
End Class
