Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CASTCommon
Imports CASTCommon.ModPublic

' ���Z�@�փ}�X�^�X�V����
Public Class ClsKousin

    ' ���O�����N���X
    Private MainLOG As New CASTCommon.BatchLOG("KFU010", "���Z�@�փ}�X�^�X�V")

    ' ���Z�@�փ}�X�^�X�V �\����
    Structure KOBETUPARAM
        Dim SyoriDate As String
        Dim SyoriKbn As String
        Dim tuuban As Integer

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public WriteOnly Property Data() As String
            Set(ByVal value As String)
                Dim para() As String = value.Split(","c)

                If para.Length = 3 Then
                    SyoriDate = para(0)                  ' �������t
                    SyoriKbn = para(1)              '�����敪
                    tuuban = Integer.Parse(para(2))   ' �W���u�ʔ�
                End If
            End Set
        End Property
    End Structure
    Private mParam As KOBETUPARAM

    ' ���Z�@�փ��[�N�}�X�^�\����
    Public Structure strTENVIEW
        Dim KIN_NO As String        '���Z�@�փR�[�h
        Dim KIN_FUKA As String      '���Z�@�֕t���R�[�h
        Dim KIN_KNAME As String     '���Z�@�փJ�i��
        Dim KIN_NNAME As String     '���Z�@�֊�����
        Dim SIT_NO As String        '�X�܃R�[�h
        Dim SIT_FUKA As String      '�X�ܕt���R�[�h
        Dim SIT_KNAME As String     '�X�܃J�i��
        Dim SIT_NNAME As String     '�X�܊�����
        Dim DENWA As String         '�d�b�ԍ�
        Dim YUUBIN As String        '�X�֔ԍ�
        Dim KJYUSYO As String       '�Z���J�i
        Dim NJYUSYO As String       '�Z������
        Dim KIN_IDO_CODE As String  '���Z�@�ֈٓ����R�R�[�h 
        Dim KIN_IDO_DATE As String  '���Z�@�ֈٓ���
        Dim SIT_IDO_CODE As String  '�X�܈ٓ����R�R�[�h
        Dim SIT_IDO_DATE As String  '�X�܈ٓ���
        Dim DEL_DATE As String      '�X�܍폜��
        Dim DEL_KIN_DATE As String  '���Z�@�֍폜��
        Dim TEN_ZOKUSEI As String   '�X�ܑ����\��
        Dim SEIDOKU As String       '���Ǖ\��

        Public WriteOnly Property SetData() As MyOracleReader
            Set(ByVal OraReader As MyOracleReader)
                With OraReader
                    KIN_NO = .GetString("KIN_NO")
                    KIN_FUKA = .GetString("KIN_FUKA")
                    KIN_KNAME = .GetString("KIN_KNAME")
                    KIN_NNAME = .GetString("KIN_NNAME")
                    SIT_NO = .GetString("SIT_NO")
                    SIT_FUKA = .GetString("SIT_FUKA")
                    SIT_KNAME = .GetString("SIT_KNAME")
                    SIT_NNAME = .GetString("SIT_NNAME")
                    DENWA = .GetString("DENWA")
                    YUUBIN = .GetString("YUUBIN")
                    KJYUSYO = .GetString("KJYUSYO")
                    NJYUSYO = .GetString("NJYUSYO")
                    KIN_IDO_CODE = .GetString("KIN_IDO_CODE")
                    KIN_IDO_DATE = .GetString("KIN_IDO_DATE")
                    SIT_IDO_CODE = .GetString("SIT_IDO_CODE")
                    SIT_IDO_DATE = .GetString("SIT_IDO_DATE")
                    DEL_DATE = .GetString("DEL_DATE")
                    DEL_KIN_DATE = .GetString("DEL_KIN_DATE")
                    TEN_ZOKUSEI = .GetString("TEN_ZOKUSEI")
                    SEIDOKU = .GetString("SEIDOKU")
                End With
            End Set
        End Property
    End Structure
    Private TENVIEW As strTENVIEW

    Structure strcIni

        Dim DAT As String           'DAT�̃p�X
        Dim DEN As String           'DEN�̃p�X
        Dim KIN_KOUSIN As String    '�X�V�敪 0:�ꊇ 1:����
        Dim KIN_FILENAME As String  '���G���^�t�@�C����
        Dim FTRANP As String        '�e�s�q�`�m�o�t�H���_
        Dim FTR As String           '�e�s�q�`�m�t�H���_
        Dim DATBK As String         'DATBK�̃p�X

    End Structure
    Private ini_info As strcIni

    ' �W���u���b�Z�[�W
    Private JobMessage As String = ""

    ' �˗��f�[�^�t�@�C����
    Private mDataFileName As String

    ' �p�u���b�N�c�a
    Private MainDB As CASTCommon.MyOracle

    ' New
    Public Sub New()
    End Sub

    ' �@�\�@ �F ���Z�@�փ}�X�^�X�V���� ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Function Main(ByVal command As String) As Integer
        ' �p�����[�^�`�F�b�N

        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        Try
            ' ���C�������ݒ�
            mParam.Data = command

            ' �W���u�ʔԐݒ�
            MainLOG.JobTuuban = mParam.tuuban

            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "(���C������)�J�n", "����")
            'MainLOG.Write("�����J�n", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***


            ' �I���N��
            MainDB = New MyOracle

            ' ���Z�@�փ}�X�^�X�V����
            Dim bRet As Boolean = KousinMain()

            If bRet = False Then
                MainDB.Rollback()
                MainDB.Close()

                MainLOG.UpdateJOBMASTbyErr(JobMessage)

                Return 2
            Else
                MainDB.Commit()
                MainDB.Close()

                MainLOG.UpdateJOBMASTbyOK(JobMessage)

                Return 0
            End If
        Catch ex As Exception
            MainLOG.Write("(���C������)", "���s", ex.Message)

            Return -1
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "(���C������)�I��", "����")
            'MainLOG.Write("�����I��", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        End Try

    End Function

    ' ���Z�@�փ}�X�^�X�V
    Private Function KousinMain() As Boolean
        ' ���̓t�@�C����
        Dim InFileName As String
        ' ��ƃt�@�C��
        Dim WorkFileName As String

        Dim oraKinReader As New CASTCommon.MyOracleReader(MainDB)
        Dim oraSitReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            'ini�t�@�C���擾
            If Read_ini() = False Then
                Return False
            End If

            '�O�̂���ini�t�@�C���̏����敪�����������`�F�b�N
            If mParam.SyoriKbn.Equals(ini_info.KIN_KOUSIN) = False Then
                JobMessage = "ini�t�@�C���@�����敪�s��v"
                MainLOG.Write("���Z�@�փ}�X�^�X�V", "���s", JobMessage)
                Return False
            End If

            InFileName = Path.Combine(ini_info.DAT, ini_info.KIN_FILENAME)
            WorkFileName = Path.Combine(ini_info.DATBK, Path.GetFileNameWithoutExtension(ini_info.KIN_FILENAME) & "_work")

            '���̓t�@�C���`�F�b�N
            If File.Exists(InFileName) = False Then
                JobMessage = "�t�@�C���Ȃ��F" & InFileName
                MainLOG.Write("���Z�@�փ}�X�^�X�V", "���s", JobMessage)
                Return False
            End If

            'EBCDIC��JIS�R�[�h�ϊ����A���[�N�t�H���_�փR�s�[
            If ConvertData(InFileName, WorkFileName) = False Then
                MainLOG.Write("�R�[�h�ϊ�", "���s", JobMessage)

                Return False
            End If

            '�ꊇ�̏ꍇ�́H�����e�[�u���폜
            'If mParam.SyoriKbn = "0" Then
            If MainDB.ExecuteNonQuery("DELETE FROM WRK_KININFO") < 0 Then Return False
            If MainDB.ExecuteNonQuery("DELETE FROM WRK_SITINFO") < 0 Then Return False
            'End If

            '���[�N�e�[�u���ɃC���T�[�g
            If CreateWork(WorkFileName) = False Then
                Return False
            End If

            '�ꊇ�ƍ����ŏ�������
            Select Case mParam.SyoriKbn

                Case "0" '�ꊇ

                    '���Z�@�փ}�X�^�N���A
                    'If MainDB.ExecuteNonQuery("DELETE FROM TENMAST") < 0 Then Return False
                    If MainDB.ExecuteNonQuery("DELETE FROM KIN_INFOMAST") < 0 Then Return False
                    If MainDB.ExecuteNonQuery("DELETE FROM SITEN_INFOMAST") < 0 Then Return False

                    If GetKinReader(oraKinReader) = False Then
                        Return False
                    End If

                    '���Z�@�֏��̍X�V
                    If RenewKinInfo(oraKinReader) = False Then
                        Return False
                    End If

                    oraKinReader.Close()

                    '���[�N�x�X���̎擾
                    If GetSItReader(oraSitReader) = False Then
                        Return False
                    End If

                    '�x�X���̍X�V
                    If RenewSitInfo(oraSitReader) = False Then
                        Return False
                    End If

                    oraSitReader.Close()

                Case "1" '����

                    '�폜���������R�[�h�폜
                    If DeleteMast() = False Then
                        Return False
                    End If

                    '���[�N���Z�@�֏��̎擾
                    If GetKinReader(oraKinReader) = False Then
                        Return False
                    End If

                    '���Z�@�֏��̍X�V
                    If RenewKinInfo(oraKinReader) = False Then
                        Return False
                    End If

                    oraKinReader.Close()

                    '���[�N�x�X���̎擾
                    If GetSitReader(oraSitReader) = False Then
                        Return False
                    End If

                    '�x�X���̍X�V
                    If RenewSitInfo(oraSitReader) = False Then
                        Return False
                    End If

                    oraSitReader.Close()

            End Select

            ''�x�X���[�N�}�X�^���x�[�X�ɋ��Z�@�փ��[�N�}�X�^�̋��Z�@�֏�����������()
            ''���Z�@�փ��[�N�}�X�^�ɑ��݂��Ȃ���΋��Z�@�փ}�X�^�ƌ�������()
            'sql.Length = 0
            'sql.Append("SELECT * FROM WRK_TENMAST_VIEW")
            ''�폜�����������ȍ~�̂���()
            'sql.Append(" WHERE (DEL_DATE IS NULL")
            'sql.Append("     OR DEL_DATE = ' '")
            'sql.Append("     OR DEL_DATE >= '" & SYORIDATE & "')")
            ''�P�H�M���p �䂤�����s�͓X�ܑ��� = 2(�o����)�̂ݑΏ�
            'sql.Append("   AND (KIN_NO   <> '9900' OR TEN_ZOKUSEI <> '2')")

            '�t�@�C���ړ�
            Dim bkFileName As String
            bkFileName = Path.Combine(ini_info.DATBK, Path.GetFileNameWithoutExtension(InFileName) & "_" & _
                                      mParam.SyoriDate & Path.GetExtension(InFileName))
            File.Copy(InFileName, bkFileName, True)                 '�t�@�C���㏑���R�s�[
            If File.Exists(InFileName) Then File.Delete(InFileName) '���t�@�C���폜

            Return True

        Catch ex As Exception
            JobMessage = "���O�Q��"
            MainLOG.Write("���Z�@�փ}�X�^�X�V", "���s", ex.Message)

            Return False
            If Not oraKinReader Is Nothing Then oraKinReader.Close()
            If Not oraSitReader Is Nothing Then oraSitReader.Close()
        End Try

        Return True
    End Function

    Private Function Read_ini() As Boolean

        ini_info.DAT = CASTCommon.GetFSKJIni("COMMON", "DAT")
        If ini_info.DAT = "err" OrElse ini_info.DAT = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:DAT�t�H���_ ����:COMMON ����:DAT")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:DAT�t�H���_ ����:COMMON ����:DAT"
            Return False
        End If

        ini_info.DEN = CASTCommon.GetFSKJIni("COMMON", "DEN")
        If ini_info.DEN = "err" OrElse ini_info.DEN = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:DEN�t�H���_ ����:COMMON ����:DEN")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:DEN�t�H���_ ����:COMMON ����:DEN"
            Return False
        End If

        ini_info.KIN_KOUSIN = CASTCommon.GetFSKJIni("COMMON", "KIN_KOUSIN")
        If ini_info.KIN_KOUSIN = "err" OrElse ini_info.KIN_KOUSIN = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���Z�@�֍X�V�敪 ����:COMMON ����:KIN_KOUSIN")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:���Z�@�֍X�V�敪 ����:COMMON ����:KIN_KOUSIN"
            Return False
        End If

        ini_info.KIN_FILENAME = CASTCommon.GetFSKJIni("COMMON", "KIN_FILENAME")
        If ini_info.KIN_FILENAME = "err" OrElse ini_info.KIN_FILENAME = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���Z�@�փt�@�C���� ����:COMMON ����:KIN_FILENAME")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:���Z�@�փt�@�C���� ����:COMMON ����:KIN_FILENAME"
            Return False
        End If

        ini_info.FTRANP = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
        If ini_info.FTRANP = "err" OrElse ini_info.FTRANP = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:FTRANP�t�H���_ ����:COMMON ����:FTRANP")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:FTRANP�t�H���_ ����:COMMON ����:FTRANP"
            Return False
        End If

        ini_info.FTR = CASTCommon.GetFSKJIni("COMMON", "FTR")
        If ini_info.FTR = "err" OrElse ini_info.FTR = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:FTR�t�H���_ ����:COMMON ����:FTR")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:FTR�t�H���_ ����:COMMON ����:FTR"
            Return False
        End If

        ini_info.DATBK = CASTCommon.GetFSKJIni("COMMON", "DATBK")
        If ini_info.DATBK = "err" OrElse ini_info.DATBK = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:DATBK�t�H���_ ����:COMMON ����:DATBK")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:DATBK�t�H���_ ����:COMMON ����:DATBK"
            Return False
        End If

        Return True
    End Function

    Private Function CreateWork(ByVal SyoriFile As String) As Boolean

        Dim StrmReader As FileStream = Nothing
        Dim OraReader As New MyOracleReader
        Dim SQL As StringBuilder = New StringBuilder(1024)

        '�ٓ��ʒm�f�[�^�����[�N�}�X�^�ɏ�������
        Dim RecCnt As Integer = 0       '���R�[�h����
        Dim KinCnt As Integer = 0       '���Z�@�փ��R�[�h����
        Dim SitCnt As Integer = 0       '�X�܃��R�[�h����

        Try
            '���[�N�t�@�C�����g�p���ď�������
            StrmReader = New FileStream(SyoriFile, FileMode.Open, FileAccess.Read)

            Dim LineData(380 - 1) As Byte
            Dim Ch380(380 - 1) As Char
            Dim ReadLen As Integer
            Dim Encd As Encoding = Encoding.GetEncoding("SHIFT-JIS")

            ReadLen = StrmReader.Read(LineData, 0, LineData.Length)

            If ReadLen <> LineData.Length Then
                JobMessage = "���R�[�h���s��v"
                MainLOG.Write("���Z�@�փ}�X�^�X�V", "���s", "���R�[�h���s��v")

                Return False
            End If

            Console.WriteLine("�����e�[�u���쐬��")

            Do While ReadLen = LineData.Length
                RecCnt += 1

                Select Case Encd.GetString(LineData, 0, 2)
                    Case "21"
                        ' ���Z�@�փf�[�^�t�H�[�}�b�g
                        Dim KinkoData As New ClsFormatIdo.KinkoFormat
                        ' ���Z�@�փf�[�^
                        Call KinkoData.Init()
                        KinkoData.Data = LineData

                        'If KinkoData.KinCode = KinkoData.DaiKinCode Then
                        SQL.Length = 0
                        SQL.Append("INSERT INTO WRK_KININFO(")
                        SQL.Append(" DATA_KBN_N")
                        SQL.Append(",DATA_SYUBETU_N")
                        SQL.Append(",KIN_NO_N")
                        SQL.Append(",KIN_FUKA_N")
                        SQL.Append(",DAIHYO_NO_N")
                        SQL.Append(",KIN_KNAME_N")
                        SQL.Append(",KIN_NNAME_N")
                        SQL.Append(",RYAKU_KIN_KNAME_N")
                        SQL.Append(",RYAKU_KIN_NNAME_N")
                        SQL.Append(",JC1_N")
                        SQL.Append(",JC2_N")
                        SQL.Append(",JC3_N")
                        SQL.Append(",JC4_N")
                        SQL.Append(",JC5_N")
                        SQL.Append(",JC6_N")
                        SQL.Append(",JC7_N")
                        SQL.Append(",JC8_N")
                        SQL.Append(",JC9_N")
                        SQL.Append(",KIN_IDO_DATE_N")
                        SQL.Append(",KIN_IDO_CODE_N")
                        SQL.Append(",NEW_KIN_NO_N")
                        SQL.Append(",NEW_KIN_FUKA_N")
                        SQL.Append(",KIN_DEL_DATE_N")
                        SQL.Append(",SYORI_DATE_N")
                        SQL.Append(") VALUES (")
                        SQL.Append(" " & SQ(KinkoData.DataKubun))
                        SQL.Append("," & SQ(KinkoData.DataSyubetu))
                        SQL.Append("," & SQ(KinkoData.KinCode))
                        SQL.Append("," & SQ(KinkoData.KinFukaCode))
                        SQL.Append("," & SQ(KinkoData.DaiKinCode))
                        SQL.Append("," & SQ(KinkoData.SeiKinKana))
                        SQL.Append("," & SQ(KinkoData.SeiKinKanji))
                        SQL.Append("," & SQ(KinkoData.RyakuKinKana))
                        SQL.Append("," & SQ(KinkoData.RyakuKinKanji))
                        SQL.Append("," & SQ(KinkoData.JISIN(0)))
                        SQL.Append("," & SQ(KinkoData.JISIN(1)))
                        SQL.Append("," & SQ(KinkoData.JISIN(2)))
                        SQL.Append("," & SQ(KinkoData.JISIN(3)))
                        SQL.Append("," & SQ(KinkoData.JISIN(4)))
                        SQL.Append("," & SQ(KinkoData.JISIN(5)))
                        SQL.Append("," & SQ(KinkoData.JISIN(6)))
                        SQL.Append("," & SQ(KinkoData.JISIN(7)))
                        SQL.Append("," & SQ(KinkoData.JISIN(8)))
                        SQL.Append("," & SQ(KinkoData.IdoDate))
                        SQL.Append("," & SQ(KinkoData.IdoJiyuCode))
                        SQL.Append("," & SQ(KinkoData.NewKinCode))
                        SQL.Append("," & SQ(KinkoData.NewKinFukaCode))
                        SQL.Append("," & SQ(KinkoData.DeleteDate.Trim.PadLeft(8, "0"c)))
                        SQL.Append("," & SQ(System.DateTime.Now.ToString("yyyyMMddHHmmss")))
                        SQL.Append(")")

                        Dim ret As Integer = MainDB.ExecuteNonQuery(SQL)

                        If ret = 0 Then
                            JobMessage = "���Z�@�փf�[�^�o�^���s"
                            MainLOG.Write("���Z�@�փf�[�^�o�^", "���s", MainDB.Message)
                            Return False
                        End If

                        KinCnt += ret

                    Case "22"
                        ' �X�܃f�[�^�t�H�[�}�b�g
                        Dim TenpoData As New ClsFormatIdo.TenpoFormat

                        ' �X�܃f�[�^
                        Call TenpoData.Init()
                        TenpoData.Data = LineData

                        SQL.Length = 0
                        SQL.Append("INSERT INTO WRK_SITINFO(")
                        SQL.Append(" DATA_KBN_N")
                        SQL.Append(",DATA_SYBETU_N")
                        SQL.Append(",KIN_NO_N")
                        SQL.Append(",KIN_FUKA_N")
                        SQL.Append(",SIT_NO_N")
                        SQL.Append(",SIT_FUKA_N")
                        SQL.Append(",SIT_KNAME_N")
                        SQL.Append(",SIT_NNAME_N")
                        SQL.Append(",SEIDOKU_HYOUJI_N")
                        SQL.Append(",YUUBIN_N")
                        SQL.Append(",KJYU_N")
                        SQL.Append(",NJYU_N")
                        SQL.Append(",TKOUKAN_NO_N")
                        SQL.Append(",DENWA_N")
                        SQL.Append(",TENPO_ZOKUSEI_N")
                        SQL.Append(",JIKOU_HYOUJI_N")
                        SQL.Append(",FURI_HYOUJI_N")
                        SQL.Append(",SYUUTE_HYOUJI_N")
                        SQL.Append(",KAWASE_HYOUJI_N")
                        SQL.Append(",DAITE_HYOUJI_N")
                        SQL.Append(",JISIN_HYOUJI_N")
                        SQL.Append(",JC_CODE_N")
                        SQL.Append(",SIT_IDO_DATE_N")
                        SQL.Append(",SIT_IDO_CODE_N")
                        SQL.Append(",NEW_KIN_NO_N")
                        SQL.Append(",NEW_KIN_FUKA_N")
                        SQL.Append(",NEW_SIT_NO_N")
                        SQL.Append(",NEW_SIT_FUKA_N")
                        SQL.Append(",SIT_DEL_DATE_N")
                        SQL.Append(",TKOUKAN_NNAME_N")
                        SQL.Append(",SYORI_DATE_N")
                        SQL.Append(") VALUES (")
                        SQL.Append(" " & SQ(TenpoData.DataKubun))
                        SQL.Append("," & SQ(TenpoData.DataSyubetu))
                        SQL.Append("," & SQ(TenpoData.KinCode))
                        SQL.Append("," & SQ(TenpoData.KinFukaCode))
                        SQL.Append("," & SQ(TenpoData.TenCode))
                        SQL.Append("," & SQ(TenpoData.TenFukaCode))
                        SQL.Append("," & SQ(TenpoData.TenKana))
                        SQL.Append("," & SQ(TenpoData.TenKanji))
                        SQL.Append("," & SQ(TenpoData.SeiHyouji))
                        SQL.Append("," & SQ(TenpoData.Yubin))
                        SQL.Append("," & SQ(TenpoData.TenSyozaiKana))
                        SQL.Append("," & SQ(TenpoData.TenSyozaiKanji))
                        SQL.Append("," & SQ(TenpoData.TegataKoukan))
                        SQL.Append("," & SQ(TenpoData.TelNo))
                        SQL.Append("," & SQ(TenpoData.TenZokusei))
                        SQL.Append("," & SQ(TenpoData.JikoCenter))
                        SQL.Append("," & SQ(TenpoData.FuriCenter))
                        SQL.Append("," & SQ(TenpoData.SyuCenter))
                        SQL.Append("," & SQ(TenpoData.KawaseCenter))
                        SQL.Append("," & SQ(TenpoData.Daitegai))
                        SQL.Append("," & SQ(TenpoData.JisinHyoji))
                        SQL.Append("," & SQ(TenpoData.JCBango))
                        SQL.Append("," & SQ(TenpoData.IdoDate))
                        SQL.Append("," & SQ(TenpoData.IdoJiyuCode))
                        SQL.Append("," & SQ(TenpoData.NewKinCode))
                        SQL.Append("," & SQ(TenpoData.NewKinFukaCode))
                        SQL.Append("," & SQ(TenpoData.NewTenCode))
                        SQL.Append("," & SQ(TenpoData.NewTenFukaCode))
                        SQL.Append("," & SQ(TenpoData.DeleteDate.Trim.PadLeft(8, "0"c)))
                        SQL.Append("," & SQ(TenpoData.TegataKoukanKanji))
                        SQL.Append("," & SQ(System.DateTime.Now.ToString("yyyyMMddHHmmss")))
                        SQL.Append(")")

                        Dim ret As Integer = MainDB.ExecuteNonQuery(SQL)

                        If ret = 0 Then
                            JobMessage = "�X�܃f�[�^�o�^���s"
                            MainLOG.Write("�X�܃f�[�^�o�^", "���s", MainDB.Message)
                            Return False
                        End If

                        SitCnt += ret
                End Select

                If RecCnt Mod 1000 = 0 Then Console.Write("#")

                ReadLen = StrmReader.Read(LineData, 0, LineData.Length)
            Loop

            Console.WriteLine(" ")   '�b��
            MainLOG.Write("���[�N�}�X�^�o�^", "����", "���R�[�h�����F" & RecCnt & "�@���Z�@�փ��R�[�h�����F" & KinCnt & "�@�X�܃��R�[�h�����F" & SitCnt)


            Return True

        Catch ex As Exception
            JobMessage = "���[�N�}�X�^�o�^���s"
            MainLOG.Write("���[�N�}�X�^�o�^", "���s", RecCnt & "���R�[�h�ځ@" & ex.Message)

            Return False

        Finally
            If Not StrmReader Is Nothing Then
                StrmReader.Close()
            End If
        End Try

    End Function

    'Private Function GetReader(ByRef oraReader As MyOracleReader) As Boolean

    '    Dim sql As New StringBuilder(256)

    '    Try
    '        sql.Append(" SELECT ")
    '        sql.Append(" WRK_KININFO.KIN_NO_N")
    '        sql.Append(",WRK_KININFO.KIN_FUKA_N")
    '        sql.Append(",WRK_KININFO.DAIHYO_NO_N")
    '        sql.Append(",WRK_KININFO.KIN_KNAME_N")
    '        sql.Append(",WRK_KININFO.KIN_NNAME_N")
    '        sql.Append(",WRK_KININFO.RYAKU_KIN_KNAME_N")
    '        sql.Append(",WRK_KININFO.RYAKU_KIN_NNAME_N")
    '        sql.Append(",WRK_KININFO.JC1_N")
    '        sql.Append(",WRK_KININFO.JC2_N")
    '        sql.Append(",WRK_KININFO.JC3_N")
    '        sql.Append(",WRK_KININFO.JC4_N")
    '        sql.Append(",WRK_KININFO.JC5_N")
    '        sql.Append(",WRK_KININFO.JC6_N")
    '        sql.Append(",WRK_KININFO.JC7_N")
    '        sql.Append(",WRK_KININFO.JC8_N")
    '        sql.Append(",WRK_KININFO.JC9_N")
    '        sql.Append(",WRK_KININFO.KIN_IDO_DATE_N")
    '        sql.Append(",WRK_KININFO.KIN_IDO_CODE_N")
    '        sql.Append(",WRK_KININFO.NEW_KIN_NO_N")
    '        sql.Append(",WRK_KININFO.NEW_KIN_FUKA_N")
    '        sql.Append(",WRK_KININFO.KIN_DEL_DATE_N")

    '        sql.Append(",WRK_SITINFO.SIT_NO_N")
    '        sql.Append(",WRK_SITINFO.SIT_FUKA_N")
    '        sql.Append(",WRK_SITINFO.SIT_KNAME_N")
    '        sql.Append(",WRK_SITINFO.SIT_NNAME_N")
    '        sql.Append(",WRK_SITINFO.SEIDOKU_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.YUUBIN_N")
    '        sql.Append(",WRK_SITINFO.KJYU_N")
    '        sql.Append(",WRK_SITINFO.NJYU_N")
    '        sql.Append(",WRK_SITINFO.TKOUKAN_NO_N")
    '        sql.Append(",WRK_SITINFO.DENWA_N")
    '        sql.Append(",WRK_SITINFO.TENPO_ZOKUSEI_N")
    '        sql.Append(",WRK_SITINFO.JIKOU_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.FURI_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.SYUUTE_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.KAWASE_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.DAITE_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.JISIN_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.JC_CODE_N")
    '        sql.Append(",WRK_SITINFO.SIT_IDO_DATE_N")
    '        sql.Append(",WRK_SITINFO.SIT_IDO_CODE_N")
    '        sql.Append(",WRK_SITINFO.NEW_KIN_NO_N AS SITEN_NEW_KIN_NO_N")
    '        sql.Append(",WRK_SITINFO.NEW_KIN_FUKA_N AS SITEN_NEW_KIN_FUKA_N")
    '        sql.Append(",WRK_SITINFO.NEW_SIT_NO_N")
    '        sql.Append(",WRK_SITINFO.NEW_SIT_FUKA_N")
    '        sql.Append(",WRK_SITINFO.SIT_DEL_DATE_N")
    '        sql.Append(",WRK_SITINFO.TKOUKAN_NNAME_N")
    '        sql.Append(" FROM ")
    '        sql.Append(" WRK_KININFO ")
    '        sql.Append(" ,WRK_SITINFO ")
    '        sql.Append(" WHERE ")
    '        sql.Append(" WRK_KININFO.KIN_NO_N = WRK_SITINFO.KIN_NO_N ")
    '        sql.Append(" AND WRK_KININFO.KIN_FUKA_N = WRK_SITINFO.KIN_FUKA_N ")
    '        sql.Append(" AND (WRK_KININFO.KIN_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR WRK_KININFO.KIN_DEL_DATE_N = '00000000')")
    '        sql.Append(" AND (WRK_SITINFO.SIT_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR WRK_SITINFO.SIT_DEL_DATE_N = '00000000')")
    '        sql.Append(" ORDER BY WRK_KININFO.KIN_NO_N, WRK_KININFO.KIN_FUKA_N, WRK_SITINFO.SIT_NO_N, WRK_SITINFO.SIT_FUKA_N, WRK_KININFO.KIN_DEL_DATE_N, WRK_SITINFO.SIT_DEL_DATE_N")

    '        If oraReader.DataReader(sql) = False Then
    '            JobMessage = "���Z�@�֗����}�X�^�������s"
    '            MainLOG.Write("���Z�@�֗����}�X�^����", "���s", "")
    '            Return False
    '        End If

    '        Return True

    '    Catch ex As Exception
    '        Throw
    '    Finally

    '    End Try

    'End Function

    Private Function GetSItReader(ByRef oraReader As MyOracleReader) As Boolean

        Dim sql As New StringBuilder(2048)

        Try
            sql.Append(" SELECT ")
            sql.Append(" WRK_SITINFO.KIN_NO_N")
            sql.Append(",WRK_SITINFO.KIN_FUKA_N")
            sql.Append(",WRK_SITINFO.SIT_NO_N")
            sql.Append(",WRK_SITINFO.SIT_FUKA_N")
            sql.Append(",WRK_SITINFO.SIT_KNAME_N")
            sql.Append(",WRK_SITINFO.SIT_NNAME_N")
            sql.Append(",WRK_SITINFO.SEIDOKU_HYOUJI_N")
            sql.Append(",WRK_SITINFO.YUUBIN_N")
            sql.Append(",WRK_SITINFO.KJYU_N")
            sql.Append(",WRK_SITINFO.NJYU_N")
            sql.Append(",WRK_SITINFO.TKOUKAN_NO_N")
            sql.Append(",WRK_SITINFO.DENWA_N")
            sql.Append(",WRK_SITINFO.TENPO_ZOKUSEI_N")
            sql.Append(",WRK_SITINFO.JIKOU_HYOUJI_N")
            sql.Append(",WRK_SITINFO.FURI_HYOUJI_N")
            sql.Append(",WRK_SITINFO.SYUUTE_HYOUJI_N")
            sql.Append(",WRK_SITINFO.KAWASE_HYOUJI_N")
            sql.Append(",WRK_SITINFO.DAITE_HYOUJI_N")
            sql.Append(",WRK_SITINFO.JISIN_HYOUJI_N")
            sql.Append(",WRK_SITINFO.JC_CODE_N")
            sql.Append(",WRK_SITINFO.SIT_IDO_DATE_N")
            sql.Append(",WRK_SITINFO.SIT_IDO_CODE_N")
            sql.Append(",WRK_SITINFO.NEW_KIN_NO_N")
            sql.Append(",WRK_SITINFO.NEW_KIN_FUKA_N")
            sql.Append(",WRK_SITINFO.NEW_SIT_NO_N")
            sql.Append(",WRK_SITINFO.NEW_SIT_FUKA_N")
            sql.Append(",WRK_SITINFO.SIT_DEL_DATE_N")
            sql.Append(",WRK_SITINFO.TKOUKAN_NNAME_N")
            sql.Append(",WRK_CNT,INF_CNT") '2011/03/30 ���R�[�h�����ǉ�
            sql.Append(" FROM ")
            sql.Append(" WRK_SITINFO ")
            '2011/03/30 ���R�[�h�����W�v�N�G�� ��������
            'WRK_SITINFO�̃��R�[�h�������W�v����
            sql.Append(",(SELECT")
            sql.Append(" COUNT(*) WRK_CNT")
            sql.Append(",KIN_NO_N WRK_KIN_NO")
            sql.Append(",KIN_FUKA_N WRK_KIN_FUKA")
            sql.Append(",SIT_NO_N WRK_SIT_NO")
            sql.Append(",SIT_FUKA_N WRK_SIT_FUKA")
            sql.Append(" FROM ")
            sql.Append(" WRK_SITINFO ")
            sql.Append(" WHERE ")
            sql.Append("  (WRK_SITINFO.SIT_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR WRK_SITINFO.SIT_DEL_DATE_N = '00000000')")
            sql.Append(" GROUP BY ")
            sql.Append(" KIN_NO_N")
            sql.Append(",KIN_FUKA_N")
            sql.Append(",SIT_NO_N")
            sql.Append(",SIT_FUKA_N)")
            'SITEN_INFOMAST�̃��R�[�h�������W�v����
            sql.Append(",(SELECT")
            sql.Append(" COUNT(*) INF_CNT")
            sql.Append(",KIN_NO_N INF_KIN_NO")
            sql.Append(",KIN_FUKA_N INF_KIN_FUKA")
            sql.Append(",SIT_NO_N INF_SIT_NO")
            sql.Append(",SIT_FUKA_N INF_SIT_FUKA")
            sql.Append(" FROM ")
            sql.Append(" SITEN_INFOMAST ")
            sql.Append(" WHERE ")
            sql.Append("  (SITEN_INFOMAST.SIT_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR SITEN_INFOMAST.SIT_DEL_DATE_N = '00000000')")
            sql.Append(" GROUP BY ")
            sql.Append(" KIN_NO_N")
            sql.Append(",KIN_FUKA_N")
            sql.Append(",SIT_NO_N")
            sql.Append(",SIT_FUKA_N)")
            '2011/03/30 ���R�[�h�����W�v�N�G�� �����܂�
            sql.Append(" WHERE ")
            sql.Append(" (WRK_SITINFO.SIT_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR WRK_SITINFO.SIT_DEL_DATE_N = '00000000')")
            '2011/03/30 ���������ǉ� ��������
            sql.Append(" AND KIN_NO_N = WRK_KIN_NO")
            sql.Append(" AND KIN_FUKA_N = WRK_KIN_FUKA")
            sql.Append(" AND SIT_NO_N = WRK_SIT_NO")
            sql.Append(" AND SIT_FUKA_N = WRK_SIT_FUKA")
            sql.Append(" AND KIN_NO_N = INF_KIN_NO(+)")
            sql.Append(" AND KIN_FUKA_N = INF_KIN_FUKA(+)")
            sql.Append(" AND SIT_NO_N = INF_SIT_NO(+)")
            sql.Append(" AND SIT_FUKA_N = INF_SIT_FUKA(+)")
            '2011/03/30 ���������ǉ� �����܂�
            sql.Append(" ORDER BY WRK_SITINFO.KIN_NO_N, WRK_SITINFO.KIN_FUKA_N, WRK_SITINFO.SIT_NO_N, WRK_SITINFO.SIT_FUKA_N, WRK_SITINFO.SIT_DEL_DATE_N")

            If oraReader.DataReader(sql) = False Then
                'JobMessage = "�x�X��񃏁[�N�}�X�^�������s"
                MainLOG.Write("�x�X��񃏁[�N�}�X�^����", "����", "�ΏۂȂ�")


                'Return False
            End If

            Return True

        Catch ex As Exception
            Throw
        Finally

        End Try

    End Function

    Private Function GetKinReader(ByRef oraReader As MyOracleReader) As Boolean

        Dim sql As New StringBuilder(2048)

        Try
            sql.Append(" SELECT ")
            sql.Append(" WRK_KININFO.KIN_NO_N")
            sql.Append(",WRK_KININFO.KIN_FUKA_N")
            sql.Append(",WRK_KININFO.DAIHYO_NO_N")
            sql.Append(",WRK_KININFO.KIN_KNAME_N")
            sql.Append(",WRK_KININFO.KIN_NNAME_N")
            sql.Append(",WRK_KININFO.RYAKU_KIN_KNAME_N")
            sql.Append(",WRK_KININFO.RYAKU_KIN_NNAME_N")
            sql.Append(",WRK_KININFO.JC1_N")
            sql.Append(",WRK_KININFO.JC2_N")
            sql.Append(",WRK_KININFO.JC3_N")
            sql.Append(",WRK_KININFO.JC4_N")
            sql.Append(",WRK_KININFO.JC5_N")
            sql.Append(",WRK_KININFO.JC6_N")
            sql.Append(",WRK_KININFO.JC7_N")
            sql.Append(",WRK_KININFO.JC8_N")
            sql.Append(",WRK_KININFO.JC9_N")
            sql.Append(",WRK_KININFO.KIN_IDO_DATE_N")
            sql.Append(",WRK_KININFO.KIN_IDO_CODE_N")
            sql.Append(",WRK_KININFO.NEW_KIN_NO_N")
            sql.Append(",WRK_KININFO.NEW_KIN_FUKA_N")
            sql.Append(",WRK_KININFO.KIN_DEL_DATE_N")
            sql.Append(",WRK_CNT,INF_CNT") '2011/03/30 ���R�[�h�����ǉ�

            sql.Append(" FROM ")
            sql.Append(" WRK_KININFO ")
            '2011/03/30 ���R�[�h�����W�v�N�G�� ��������
            'WRK_KININFO�̃��R�[�h�������W�v����
            sql.Append(",(SELECT")
            sql.Append(" COUNT(*) WRK_CNT")
            sql.Append(",KIN_NO_N WRK_KIN_NO")
            sql.Append(",KIN_FUKA_N WRK_KIN_FUKA")
            sql.Append(" FROM ")
            sql.Append(" WRK_KININFO ")
            sql.Append(" WHERE ")
            sql.Append("  (WRK_KININFO.KIN_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR WRK_KININFO.KIN_DEL_DATE_N = '00000000')")
            sql.Append(" GROUP BY ")
            sql.Append(" KIN_NO_N")
            sql.Append(",KIN_FUKA_N)")
            'KIN_INFOMAST�̃��R�[�h�������W�v����
            sql.Append(",(SELECT")
            sql.Append(" COUNT(*) INF_CNT")
            sql.Append(",KIN_NO_N INF_KIN_NO")
            sql.Append(",KIN_FUKA_N INF_KIN_FUKA")
            sql.Append(" FROM ")
            sql.Append(" KIN_INFOMAST ")
            sql.Append(" WHERE ")
            sql.Append("  (KIN_INFOMAST.KIN_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR KIN_INFOMAST.KIN_DEL_DATE_N = '00000000')")
            sql.Append(" GROUP BY ")
            sql.Append(" KIN_NO_N")
            sql.Append(",KIN_FUKA_N)")
            '2011/03/30 ���R�[�h�����W�v�N�G�� �����܂�

            sql.Append(" WHERE ")
            sql.Append("  (WRK_KININFO.KIN_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR WRK_KININFO.KIN_DEL_DATE_N = '00000000')")
            '2011/03/30 ���������ǉ� ��������
            sql.Append(" AND KIN_NO_N = WRK_KIN_NO")
            sql.Append(" AND KIN_FUKA_N = WRK_KIN_FUKA")
            sql.Append(" AND KIN_NO_N = INF_KIN_NO(+)")
            sql.Append(" AND KIN_FUKA_N = INF_KIN_FUKA(+)")
            '2011/03/30 ���������ǉ� �����܂�
            sql.Append(" ORDER BY WRK_KININFO.KIN_NO_N, WRK_KININFO.KIN_FUKA_N,  WRK_KININFO.KIN_DEL_DATE_N")

            If oraReader.DataReader(sql) = False Then
                'JobMessage = "���Z�@�֏�񃏁[�N�}�X�^�������s"
                MainLOG.Write("���Z�@�֏�񃏁[�N�}�X�^����", "����", "�ΏۂȂ�")
                'Return False
            End If

            Return True

        Catch ex As Exception
            Throw
        Finally

        End Try

    End Function

    Private Function RenewKinInfo(ByVal oraReader As CASTCommon.MyOracleReader) As Boolean

        Dim sql As StringBuilder
        Dim iRet As Integer
        Dim RecCnt As Integer = 0

        Try

            Console.WriteLine("���Z�@�֏��}�X�^�쐬��")

            Do Until oraReader.EOF

                '2017/01/18 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
                '���Z�@�֏��}�X�^�A�b�v�f�[�gSQL�쐬���Ɏ擾���Ă�����g�敪���O�o��
                '���炩���ߎ擾���Ă����Đ��\�򉻂�h��
                Dim strTeikeiKbn As String = Me.GetTeikeiKbn(oraReader.GetString("KIN_NO_N"))
                '2017/01/18 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END

                '�f���ɃA�b�v�f�[�g�C���T�[�g���Ă݂�

                sql = New StringBuilder(2048)

                '2011/03/30 ���R�[�h�W�v���ʂɂ���ăN�G���𕪂��� ��������
                If oraReader.GetInt("INF_CNT") >= 1 AndAlso oraReader.GetInt("WRK_CNT") = 1 Then
                    '���Z�@�֏��}�X�^��1���R�[�h�ȏ�A���[�N�}�X�^��1���R�[�h�̏ꍇ
                    '�폜���Ȃ��̃��R�[�h���X�V����
                    sql.Append("UPDATE KIN_INFOMAST SET")
                    sql.Append(" DAIHYO_NO_N = " & SQ(oraReader.GetString("DAIHYO_NO_N")))
                    sql.Append(",KIN_KNAME_N = " & SQ(oraReader.GetString("KIN_KNAME_N")))
                    sql.Append(",KIN_NNAME_N = " & SQ(oraReader.GetString("KIN_NNAME_N")))
                    sql.Append(",RYAKU_KIN_KNAME_N = " & SQ(oraReader.GetString("RYAKU_KIN_KNAME_N")))
                    sql.Append(",RYAKU_KIN_NNAME_N = " & SQ(oraReader.GetString("RYAKU_KIN_NNAME_N")))
                    sql.Append(",JC1_N = " & SQ(oraReader.GetString("JC1_N")))
                    sql.Append(",JC2_N = " & SQ(oraReader.GetString("JC2_N")))
                    sql.Append(",JC3_N = " & SQ(oraReader.GetString("JC3_N")))
                    sql.Append(",JC4_N = " & SQ(oraReader.GetString("JC4_N")))
                    sql.Append(",JC5_N = " & SQ(oraReader.GetString("JC5_N")))
                    sql.Append(",JC6_N = " & SQ(oraReader.GetString("JC6_N")))
                    sql.Append(",JC7_N = " & SQ(oraReader.GetString("JC7_N")))
                    sql.Append(",JC8_N = " & SQ(oraReader.GetString("JC8_N")))
                    sql.Append(",JC9_N = " & SQ(oraReader.GetString("JC9_N")))
                    sql.Append(",KIN_IDO_DATE_N = " & SQ(oraReader.GetString("KIN_IDO_DATE_N")))
                    sql.Append(",KIN_IDO_CODE_N = " & SQ(oraReader.GetString("KIN_IDO_CODE_N")))
                    sql.Append(",NEW_KIN_NO_N = " & SQ(oraReader.GetString("NEW_KIN_NO_N")))
                    sql.Append(",NEW_KIN_FUKA_N = " & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
                    sql.Append(",KIN_DEL_DATE_N = " & SQ(oraReader.GetString("KIN_DEL_DATE_N")))
                    '2017/01/18 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
                    '��g�敪�A���Z�@�֎�ށA�n��R�[�h�A�x���萔���̐ݒ�
                    sql.Append(",TEIKEI_KBN_N = " & SQ(strTeikeiKbn))
                    sql.Append(",SYUBETU_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(0)))
                    sql.Append(",TIKU_CODE_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(1)))
                    sql.Append(",TESUU_TANKA_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(2)))
                    'sql.Append(",TEIKEI_KBN_N = " & SQ(GetTeikeiKbn(oraReader.GetString("KIN_NO_N"))))
                    'sql.Append(",SYUBETU_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(0)))
                    'sql.Append(",TIKU_CODE_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(1)))
                    'sql.Append(",TESUU_TANKA_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(2)))
                    '2017/01/18 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
                    sql.Append(",KOUSIN_DATE_N = " & SQ(mParam.SyoriDate))

                    sql.Append(" WHERE KIN_NO_N = " & SQ(oraReader.GetString("KIN_NO_N")))
                    sql.Append(" AND KIN_FUKA_N = " & SQ(oraReader.GetString("KIN_FUKA_N")))
                    sql.Append(" AND KIN_DEL_DATE_N = '00000000'")
                Else
                    '2011/03/30 ���R�[�h�W�v���ʂɂ���ăN�G���𕪂��� �����܂�
                    sql.Append("MERGE INTO KIN_INFOMAST")
                    sql.Append(" USING (SELECT")
                    sql.Append("  " & SQ(oraReader.GetString("KIN_NO_N")) & " KIN_NO")
                    sql.Append(", " & SQ(oraReader.GetString("KIN_FUKA_N")) & " KIN_FUKA")
                    sql.Append(", " & SQ(oraReader.GetString("KIN_DEL_DATE_N")) & " KIN_DEL_DATE")
                    sql.Append(" FROM DUAL)")
                    sql.Append(" ON (KIN_NO_N = KIN_NO")
                    sql.Append(" AND KIN_FUKA_N = KIN_FUKA")
                    sql.Append(" AND KIN_DEL_DATE_N = KIN_DEL_DATE)")
                    'UPDATE��
                    sql.Append(" WHEN MATCHED THEN")
                    sql.Append(" UPDATE SET")
                    'sql.Append(" KIN_NO_N = " & SQ(oraReader.GetString("KIN_NO_N")))
                    'sql.Append(",KIN_FUKA_N = " & SQ(oraReader.GetString("KIN_FUKA_N")))
                    sql.Append(" DAIHYO_NO_N = " & SQ(oraReader.GetString("DAIHYO_NO_N")))
                    sql.Append(",KIN_KNAME_N = " & SQ(oraReader.GetString("KIN_KNAME_N")))
                    sql.Append(",KIN_NNAME_N = " & SQ(oraReader.GetString("KIN_NNAME_N")))
                    sql.Append(",RYAKU_KIN_KNAME_N = " & SQ(oraReader.GetString("RYAKU_KIN_KNAME_N")))
                    sql.Append(",RYAKU_KIN_NNAME_N = " & SQ(oraReader.GetString("RYAKU_KIN_NNAME_N")))
                    sql.Append(",JC1_N = " & SQ(oraReader.GetString("JC1_N")))
                    sql.Append(",JC2_N = " & SQ(oraReader.GetString("JC2_N")))
                    sql.Append(",JC3_N = " & SQ(oraReader.GetString("JC3_N")))
                    sql.Append(",JC4_N = " & SQ(oraReader.GetString("JC4_N")))
                    sql.Append(",JC5_N = " & SQ(oraReader.GetString("JC5_N")))
                    sql.Append(",JC6_N = " & SQ(oraReader.GetString("JC6_N")))
                    sql.Append(",JC7_N = " & SQ(oraReader.GetString("JC7_N")))
                    sql.Append(",JC8_N = " & SQ(oraReader.GetString("JC8_N")))
                    sql.Append(",JC9_N = " & SQ(oraReader.GetString("JC9_N")))
                    sql.Append(",KIN_IDO_DATE_N = " & SQ(oraReader.GetString("KIN_IDO_DATE_N")))
                    sql.Append(",KIN_IDO_CODE_N = " & SQ(oraReader.GetString("KIN_IDO_CODE_N")))
                    sql.Append(",NEW_KIN_NO_N = " & SQ(oraReader.GetString("NEW_KIN_NO_N")))
                    sql.Append(",NEW_KIN_FUKA_N = " & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
                    'sql.Append(",KIN_DEL_DATE_N = " & SQ(oraReader.GetString("KIN_DEL_DATE_N")))
                    '2017/01/18 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
                    '��g�敪�A���Z�@�֎�ށA�n��R�[�h�A�x���萔���̐ݒ�
                    sql.Append(",TEIKEI_KBN_N = " & SQ(strTeikeiKbn))
                    sql.Append(",SYUBETU_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(0)))
                    sql.Append(",TIKU_CODE_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(1)))
                    sql.Append(",TESUU_TANKA_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(2)))
                    'sql.Append(",TEIKEI_KBN_N = " & SQ(GetTeikeiKbn(oraReader.GetString("KIN_NO_N"))))
                    'sql.Append(",SYUBETU_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(0)))
                    'sql.Append(",TIKU_CODE_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(1)))
                    'sql.Append(",TESUU_TANKA_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(2)))
                    '2017/01/18 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
                    'sql.Append(",SAKUSEI_DATE_N = " & SQ(mParam.SyoriDate))
                    sql.Append(",KOUSIN_DATE_N = " & SQ(mParam.SyoriDate))
                    'INSERT��
                    sql.Append(" WHEN NOT MATCHED THEN")
                    sql.Append(" INSERT VALUES(")
                    sql.Append(" " & SQ(oraReader.GetString("KIN_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_FUKA_N")))
                    sql.Append("," & SQ(oraReader.GetString("DAIHYO_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_KNAME_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_NNAME_N")))
                    sql.Append("," & SQ(oraReader.GetString("RYAKU_KIN_KNAME_N")))
                    sql.Append("," & SQ(oraReader.GetString("RYAKU_KIN_NNAME_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC1_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC2_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC3_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC4_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC5_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC6_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC7_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC8_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC9_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_IDO_DATE_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_IDO_CODE_N")))
                    sql.Append("," & SQ(oraReader.GetString("NEW_KIN_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_DEL_DATE_N")))
                    '2017/01/18 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
                    '��g�敪�A���Z�@�֎�ށA�n��R�[�h�A�x���萔���̐ݒ�
                    sql.Append("," & SQ(strTeikeiKbn))
                    sql.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(0)))
                    sql.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(1)))
                    sql.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(2)))
                    'sql.Append("," & SQ(GetTeikeiKbn(oraReader.GetString("KIN_NO_N"))))
                    'sql.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(0)))
                    'sql.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(1)))
                    'sql.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(2)))
                    '2017/01/18 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
                    sql.Append("," & SQ(mParam.SyoriDate))
                    sql.Append("," & SQ(mParam.SyoriDate))
                    sql.Append(")")
                End If '2011/03/30 If����

                iRet = MainDB.ExecuteNonQuery(sql)

                If iRet <> 1 Then
                    JobMessage = "���Z�@�֏��}�X�^�X�V���s"
                    MainLOG.Write("���Z�@�֏��}�X�^�X�V", "���s", "�X�V�����s��:" & iRet)
                    Return False
                End If

                RecCnt += 1

                oraReader.NextRead()

                If RecCnt Mod 100 = 0 Then Console.Write("#")

            Loop

            Console.WriteLine(" ")

            JobMessage = "���������F" & RecCnt
            MainLOG.Write("���Z�@�֏��}�X�^�X�V", "����", JobMessage)


            Return True

        Catch ex As Exception
            Throw
        End Try

    End Function


    Private Function RenewSitInfo(ByVal oraReader As CASTCommon.MyOracleReader) As Boolean

        Dim sql As StringBuilder
        Dim iRet As Integer
        Dim RecCnt As Integer = 0

        Try

            Console.WriteLine("�x�X���}�X�^�쐬��")

            Do Until oraReader.EOF

                '�f���ɃA�b�v�f�[�g�C���T�[�g���Ă݂�

                sql = New StringBuilder(2048)

                '2011/03/30 ���R�[�h�W�v���ʂɂ���ăN�G���𕪂��� ��������
                If oraReader.GetInt("INF_CNT") >= 1 AndAlso oraReader.GetInt("WRK_CNT") = 1 Then
                    '�x�X���}�X�^��1���R�[�h�ȏ�A���[�N�}�X�^��1���R�[�h�̏ꍇ
                    '�폜���Ȃ��̃��R�[�h���X�V����
                    sql.Append("UPDATE SITEN_INFOMAST SET")
                    sql.Append(" SIT_KNAME_N = " & SQ(oraReader.GetString("SIT_KNAME_N")))
                    sql.Append(",SIT_NNAME_N = " & SQ(oraReader.GetString("SIT_NNAME_N")))
                    sql.Append(",SEIDOKU_HYOUJI_N = " & SQ(oraReader.GetString("SEIDOKU_HYOUJI_N")))
                    sql.Append(",YUUBIN_N = " & SQ(oraReader.GetString("YUUBIN_N")))
                    sql.Append(",KJYU_N = " & SQ(oraReader.GetString("KJYU_N")))
                    sql.Append(",NJYU_N = " & SQ(oraReader.GetString("NJYU_N")))
                    sql.Append(",TKOUKAN_NO_N = " & SQ(oraReader.GetString("TKOUKAN_NO_N")))
                    sql.Append(",DENWA_N = " & SQ(oraReader.GetString("DENWA_N")))
                    sql.Append(",TENPO_ZOKUSEI_N = " & SQ(oraReader.GetString("TENPO_ZOKUSEI_N")))
                    sql.Append(",JIKOU_HYOUJI_N = " & SQ(oraReader.GetString("JIKOU_HYOUJI_N")))
                    sql.Append(",FURI_HYOUJI_N = " & SQ(oraReader.GetString("FURI_HYOUJI_N")))
                    sql.Append(",SYUUTE_HYOUJI_N = " & SQ(oraReader.GetString("SYUUTE_HYOUJI_N")))
                    sql.Append(",KAWASE_HYOUJI_N = " & SQ(oraReader.GetString("KAWASE_HYOUJI_N")))
                    sql.Append(",DAITE_HYOUJI_N = " & SQ(oraReader.GetString("DAITE_HYOUJI_N")))
                    sql.Append(",JISIN_HYOUJI_N = " & SQ(oraReader.GetString("JISIN_HYOUJI_N")))
                    sql.Append(",JC_CODE_N = " & SQ(oraReader.GetString("JC_CODE_N")))
                    sql.Append(",SIT_IDO_DATE_N = " & SQ(oraReader.GetString("SIT_IDO_DATE_N")))
                    sql.Append(",SIT_IDO_CODE_N = " & SQ(oraReader.GetString("SIT_IDO_CODE_N")))
                    sql.Append(",NEW_KIN_NO_N = " & SQ(oraReader.GetString("NEW_KIN_NO_N")))
                    sql.Append(",NEW_KIN_FUKA_N = " & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
                    sql.Append(",NEW_SIT_NO_N = " & SQ(oraReader.GetString("NEW_SIT_NO_N")))
                    sql.Append(",NEW_SIT_FUKA_N = " & SQ(oraReader.GetString("NEW_SIT_FUKA_N")))
                    sql.Append(",SIT_DEL_DATE_N = " & SQ(oraReader.GetString("SIT_DEL_DATE_N")))
                    sql.Append(",TKOUKAN_NNAME_N = " & SQ(oraReader.GetString("TKOUKAN_NNAME_N")))
                    sql.Append(",KOUSIN_DATE_N = " & SQ(mParam.SyoriDate))

                    sql.Append(" WHERE KIN_NO_N = " & SQ(oraReader.GetString("KIN_NO_N")))
                    sql.Append(" AND KIN_FUKA_N = " & SQ(oraReader.GetString("KIN_FUKA_N")))
                    sql.Append(" AND SIT_NO_N = " & SQ(oraReader.GetString("SIT_NO_N")))
                    sql.Append(" AND SIT_FUKA_N = " & SQ(oraReader.GetString("SIT_FUKA_N")))
                    sql.Append(" AND SIT_DEL_DATE_N = '00000000'")
                Else
                    '2011/03/30 ���R�[�h�W�v���ʂɂ���ăN�G���𕪂��� �����܂�
                    sql.Append("MERGE INTO SITEN_INFOMAST")
                    sql.Append(" USING (SELECT")
                    sql.Append("  " & SQ(oraReader.GetString("KIN_NO_N")) & " KIN_NO")
                    sql.Append(", " & SQ(oraReader.GetString("KIN_FUKA_N")) & " KIN_FUKA")
                    sql.Append(", " & SQ(oraReader.GetString("SIT_NO_N")) & " SIT_NO")
                    sql.Append(", " & SQ(oraReader.GetString("SIT_FUKA_N")) & " SIT_FUKA")
                    sql.Append(", " & SQ(oraReader.GetString("SIT_DEL_DATE_N")) & " SIT_DEL_DATE")
                    sql.Append(" FROM DUAL)")
                    sql.Append(" ON (KIN_NO_N = KIN_NO")
                    sql.Append(" AND KIN_FUKA_N = KIN_FUKA")
                    sql.Append(" AND SIT_NO_N = SIT_NO")
                    sql.Append(" AND SIT_FUKA_N = SIT_FUKA")
                    sql.Append(" AND SIT_DEL_DATE_N = SIT_DEL_DATE)")
                    'UPDATE��
                    sql.Append(" WHEN MATCHED THEN")
                    sql.Append(" UPDATE SET")
                    'sql.Append(" KIN_NO_N = " & SQ(oraReader.GetString("KIN_NO_N")))
                    'sql.Append(",KIN_FUKA_N = " & SQ(oraReader.GetString("KIN_FUKA_N")))
                    'sql.Append(",SIT_NO_N = " & SQ(oraReader.GetString("SIT_NO_N")))
                    'sql.Append(",SIT_FUKA_N = " & SQ(oraReader.GetString("SIT_FUKA_N")))
                    sql.Append(" SIT_KNAME_N = " & SQ(oraReader.GetString("SIT_KNAME_N")))
                    sql.Append(",SIT_NNAME_N = " & SQ(oraReader.GetString("SIT_NNAME_N")))
                    sql.Append(",SEIDOKU_HYOUJI_N = " & SQ(oraReader.GetString("SEIDOKU_HYOUJI_N")))
                    sql.Append(",YUUBIN_N = " & SQ(oraReader.GetString("YUUBIN_N")))
                    sql.Append(",KJYU_N = " & SQ(oraReader.GetString("KJYU_N")))
                    sql.Append(",NJYU_N = " & SQ(oraReader.GetString("NJYU_N")))
                    sql.Append(",TKOUKAN_NO_N = " & SQ(oraReader.GetString("TKOUKAN_NO_N")))
                    sql.Append(",DENWA_N = " & SQ(oraReader.GetString("DENWA_N")))
                    sql.Append(",TENPO_ZOKUSEI_N = " & SQ(oraReader.GetString("TENPO_ZOKUSEI_N")))
                    sql.Append(",JIKOU_HYOUJI_N = " & SQ(oraReader.GetString("JIKOU_HYOUJI_N")))
                    sql.Append(",FURI_HYOUJI_N = " & SQ(oraReader.GetString("FURI_HYOUJI_N")))
                    sql.Append(",SYUUTE_HYOUJI_N = " & SQ(oraReader.GetString("SYUUTE_HYOUJI_N")))
                    sql.Append(",KAWASE_HYOUJI_N = " & SQ(oraReader.GetString("KAWASE_HYOUJI_N")))
                    sql.Append(",DAITE_HYOUJI_N = " & SQ(oraReader.GetString("DAITE_HYOUJI_N")))
                    sql.Append(",JISIN_HYOUJI_N = " & SQ(oraReader.GetString("JISIN_HYOUJI_N")))
                    sql.Append(",JC_CODE_N = " & SQ(oraReader.GetString("JC_CODE_N")))
                    sql.Append(",SIT_IDO_DATE_N = " & SQ(oraReader.GetString("SIT_IDO_DATE_N")))
                    sql.Append(",SIT_IDO_CODE_N = " & SQ(oraReader.GetString("SIT_IDO_CODE_N")))
                    sql.Append(",NEW_KIN_NO_N = " & SQ(oraReader.GetString("NEW_KIN_NO_N")))
                    sql.Append(",NEW_KIN_FUKA_N = " & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
                    sql.Append(",NEW_SIT_NO_N = " & SQ(oraReader.GetString("NEW_SIT_NO_N")))
                    sql.Append(",NEW_SIT_FUKA_N = " & SQ(oraReader.GetString("NEW_SIT_FUKA_N")))
                    'sql.Append(",SIT_DEL_DATE_N = " & SQ(oraReader.GetString("SIT_DEL_DATE_N")))
                    sql.Append(",TKOUKAN_NNAME_N = " & SQ(oraReader.GetString("TKOUKAN_NNAME_N")))
                    sql.Append(",KOUSIN_DATE_N = " & SQ(mParam.SyoriDate))
                    'INSERT��
                    sql.Append(" WHEN NOT MATCHED THEN")
                    sql.Append(" INSERT VALUES(")
                    sql.Append(" " & SQ(oraReader.GetString("KIN_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_FUKA_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_FUKA_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_KNAME_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_NNAME_N")))
                    sql.Append("," & SQ(oraReader.GetString("SEIDOKU_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("YUUBIN_N")))
                    sql.Append("," & SQ(oraReader.GetString("KJYU_N")))
                    sql.Append("," & SQ(oraReader.GetString("NJYU_N")))
                    sql.Append("," & SQ(oraReader.GetString("TKOUKAN_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("DENWA_N")))
                    sql.Append("," & SQ(" "))
                    sql.Append("," & SQ(oraReader.GetString("TENPO_ZOKUSEI_N")))
                    sql.Append("," & SQ(oraReader.GetString("JIKOU_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("FURI_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("SYUUTE_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("KAWASE_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("DAITE_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("JISIN_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC_CODE_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_IDO_DATE_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_IDO_CODE_N")))
                    sql.Append("," & SQ(oraReader.GetString("NEW_KIN_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
                    sql.Append("," & SQ(oraReader.GetString("NEW_SIT_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("NEW_SIT_FUKA_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_DEL_DATE_N")))
                    sql.Append("," & SQ(oraReader.GetString("TKOUKAN_NNAME_N")))
                    sql.Append("," & SQ(mParam.SyoriDate))
                    sql.Append("," & SQ(mParam.SyoriDate))
                    sql.Append(")")
                End If '2011/03/30 If����

                iRet = MainDB.ExecuteNonQuery(sql)

                If iRet <> 1 Then
                    JobMessage = "�x�X���}�X�^�X�V���s"
                    MainLOG.Write("�x�X���}�X�^�X�V", "���s", "�X�V�����s��:" & iRet)

                    Return False
                End If

                oraReader.NextRead()

                RecCnt += 1

                If RecCnt Mod 1000 = 0 Then Console.Write("#")

            Loop


            Console.WriteLine(" ")

            JobMessage = "���������F" & RecCnt
            MainLOG.Write("�x�X���}�X�^�X�V", "����", JobMessage)


            Return True

        Catch ex As Exception

            Throw

        End Try


        'Dim sql As StringBuilder

        'Try
        '    Dim ret As Integer
        '    Dim InsertCnt As Integer = 0        'INSERT����
        '    Dim UpdateCnt As Integer = 0        'UPDATE����
        '    RecCnt = 0

        '    oraReader = New MyOracleReader(MainDB)


        '    Do Until oraReader.EOF
        '        RecCnt += 1

        '        '�ٓ����R�R�[�h�ŕ���
        '        Select Case oraReader.GetString("SIT_IDO_CODE")
        '            Case "00", "01"
        '                ' �����o�^�C�V��
        '                ret = 0

        '            Case "02", "03", "05", "09"
        '                ' �p�~�C�p�~�p���C�ύX�O�C�c�Ə��n�i�ύX�O�j

        '                '�폜���A���Z�@�֍폜���A�ٓ����A�X�V����UPDATE
        '                sql.Length = 0
        '                sql.Append("UPDATE TENMAST SET ")
        '                sql.Append(" DEL_DATE_N     = '" & TENVIEW.DEL_DATE & "'")
        '                sql.Append(",DEL_KIN_DATE_N = '" & TENVIEW.DEL_KIN_DATE & "'")
        '                If TENVIEW.SIT_IDO_CODE = "02" Then
        '                    sql.Append(",YOBI5_N        = '" & TENVIEW.SIT_IDO_DATE & "'")
        '                End If
        '                sql.Append(",KOUSIN_DATE_N  = '" & SYORIDATE & "'")
        '                sql.Append(" WHERE KIN_NO_N = '" & TENVIEW.KIN_NO & "'")
        '                sql.Append("   AND SIT_NO_N = '" & TENVIEW.SIT_NO & "'")
        '                '���X�V�̃��R�[�h�̍ŏ��̎}�Ԃ��X�V����
        '                sql.Append("   AND EDA_N    = (")
        '                sql.Append("SELECT MIN(EDA_N) FROM TENMAST")
        '                sql.Append(" WHERE KIN_NO_N = '" & TENVIEW.KIN_NO & "'")
        '                sql.Append("   AND SIT_NO_N = '" & TENVIEW.SIT_NO & "'")
        '                sql.Append("   AND YOBI1_N  = '" & TENVIEW.KIN_FUKA & "'")
        '                sql.Append("   AND YOBI2_N  = '" & TENVIEW.SIT_FUKA & "'")
        '                '���X�V�̃��R�[�h�̂ݑΏ�
        '                sql.Append("   AND KOUSIN_DATE_N <> '" & SYORIDATE & "'")
        '                sql.Append(")")

        '                ret = MainDB.ExecuteNonQuery(sql)
        '                UpdateCnt += ret

        '                '0�`1���������肦�Ȃ��͂��E�E�E
        '                If ret > 1 Then
        '                    JobMessage = "���Z�@�փ}�X�^UPDATE�����ُ�"
        '                    LOG.Write("���Z�@�փ}�X�^UPDATE", "���s", "�����ُ�F" & sql.ToString)
        '                    Return False
        '                End If

        '            Case "04", "06", "10"
        '                ' �ύX��C�c�Ə��n�i�ύX��j�C���̑��ύX

        '                '���R�[�h�̑S����UPDATE
        '                sql.Length = 0
        '                sql.Append("UPDATE TENMAST SET ")
        '                sql.Append(" KIN_KNAME_N    = '" & TENVIEW.KIN_KNAME & "'")
        '                sql.Append(",KIN_NNAME_N    = '" & TENVIEW.KIN_NNAME & "'")
        '                sql.Append(",SIT_KNAME_N    = '" & TENVIEW.SIT_KNAME & "'")
        '                sql.Append(",SIT_NNAME_N    = '" & TENVIEW.SIT_NNAME & "'")
        '                sql.Append(",DENWA_N        = '" & TENVIEW.DENWA & "'")
        '                sql.Append(",YUUBIN_N       = '" & TENVIEW.YUUBIN & "'")
        '                sql.Append(",KJYU_N         = '" & TENVIEW.KJYUSYO & "'")
        '                sql.Append(",NJYU_N         = '" & TENVIEW.NJYUSYO & "'")
        '                sql.Append(",DEL_DATE_N     = '" & TENVIEW.DEL_DATE & "'")
        '                sql.Append(",DEL_KIN_DATE_N = '" & TENVIEW.DEL_KIN_DATE & "'")
        '                sql.Append(",KOUSIN_DATE_N  = '" & SYORIDATE & "'")
        '                sql.Append(",YOBI3_N        = '" & TENVIEW.TEN_ZOKUSEI & "'")
        '                sql.Append(",YOBI4_N        = '" & TENVIEW.SEIDOKU & "'")
        '                'SQL.Append(",YOBI5_N        = '" & TENVIEW.SIT_IDO_DATE & "'")
        '                sql.Append(" WHERE KIN_NO_N = '" & TENVIEW.KIN_NO & "'")
        '                sql.Append("   AND SIT_NO_N = '" & TENVIEW.SIT_NO & "'")
        '                '���X�V�̃��R�[�h�̍ŏ��̎}�Ԃ��X�V����
        '                sql.Append("   AND EDA_N    = (")
        '                sql.Append("SELECT MIN(EDA_N) FROM TENMAST")
        '                sql.Append(" WHERE KIN_NO_N = '" & TENVIEW.KIN_NO & "'")
        '                sql.Append("   AND SIT_NO_N = '" & TENVIEW.SIT_NO & "'")
        '                sql.Append("   AND YOBI1_N  = '" & TENVIEW.KIN_FUKA & "'")
        '                sql.Append("   AND YOBI2_N  = '" & TENVIEW.SIT_FUKA & "'")
        '                '���X�V�̃��R�[�h�̂ݑΏ�
        '                sql.Append("   AND KOUSIN_DATE_N <> '" & SYORIDATE & "'")
        '                sql.Append(")")

        '                ret = MainDB.ExecuteNonQuery(sql)
        '                UpdateCnt += ret

        '                '0�`1���������肦�Ȃ��͂��E�E�E
        '                If ret > 1 Then
        '                    JobMessage = "���Z�@�փ}�X�^UPDATE�����ُ�"
        '                    LOG.Write("���Z�@�փ}�X�^UPDATE", "���s", "�����ُ�F" & sql.ToString)
        '                    Return False
        '                End If

        '            Case Else
        '                JobMessage = "�X�܈ٓ����R�R�[�h�ُ�"
        '                LOG.Write("���Z�@�փ}�X�^�X�V", "���s", "�X�܈ٓ����R�R�[�h�ُ�F" & TENVIEW.SIT_IDO_CODE)
        '                Return False
        '        End Select

        '        'UPDATE������0�̏ꍇ��INSERT����
        '        If ret = 0 Then
        '            ret = InsertTENMAST()
        '            InsertCnt += ret
        '        End If

        '        '������-1�̏ꍇ�̓G���[
        '        If ret = -1 Then
        '            Return False
        '        End If

        '    Loop While oraReader.NextRead = True

        '    '���������E�E�E���[�N�}�X�^�̃��R�[�h����
        '    '�o�^�����E�E�E�E�EINSERT����
        '    '�X�V�����E�E�E�E�EUPDATE����
        '    '�폜�����E�E�E�E�EDELETE����
        '    '���������� = (�o�^���� + �X�V����)�̐�������Ȃ��ꍇ�́A���炩�̌����œ�d�o�^�`�F�b�N�Ɉ����������Ă���
        '    JobMessage = "���������F" & RecCnt & "�@�o�^�����F" & InsertCnt & "�@�X�V�����F" & UpdateCnt & "�@�폜�����F" & DelCnt
        '    LOG.Write("���Z�@�փ}�X�^�X�V", "����", JobMessage)

        'Catch ex As Exception
        '    JobMessage = "���Z�@�փ}�X�^�X�V���s"
        '    LOG.Write("���Z�@�փ}�X�^�X�V", "���s", ex.Message)
        '    Return False

        'Finally
        '    oraReader.Close()
        'End Try

    End Function


    '' ���Z�@�փ}�X�^�Ƀf�[�^��o�^����
    'Private Function InsertTENMAST(ByVal oraReader As CASTCommon.MyOracleReader) As Boolean

    '    Dim iRet As Integer
    '    Dim SQL As StringBuilder
    '    Dim RecCnt As Integer = 0
    '    Dim KinOldKey As String = ""
    '    Dim KinNewKey As String = ""
    '    Try

    '        Console.WriteLine("���Z�@�փ}�X�^�C���T�[�g��")

    '        Do Until oraReader.EOF

    '            ''�������Z�@�֏�񂪑��݂���ꍇ�͓o�^���Ȃ�(��d�o�^�h�~)
    '            'If CheckSameData(oraReader.GetString("KIN_NO_N"), oraReader.GetString("KIN_FUKA_N"), _
    '            '                 oraReader.GetString("SIT_NO_N"), oraReader.GetString("SIT_FUKA_N")) = True Then

    '            SQL = New StringBuilder(128)

    '            SQL.Append("INSERT INTO TENMAST(")
    '            SQL.Append(" KIN_NO_N")
    '            SQL.Append(",KIN_FUKA_N")
    '            SQL.Append(",SIT_NO_N")
    '            SQL.Append(",SIT_FUKA_N")
    '            SQL.Append(",KIN_KNAME_N")
    '            SQL.Append(",KIN_NNAME_N")
    '            SQL.Append(",SIT_KNAME_N")
    '            SQL.Append(",SIT_NNAME_N")
    '            SQL.Append(",NEW_KIN_NO_N")
    '            SQL.Append(",NEW_KIN_FUKA_N")
    '            SQL.Append(",NEW_SIT_NO_N")
    '            SQL.Append(",NEW_SIT_FUKA_N")
    '            SQL.Append(",KIN_DEL_DATE_N")
    '            SQL.Append(",SIT_DEL_DATE_N")
    '            SQL.Append(",SAKUSEI_DATE_N")
    '            SQL.Append(",KOUSIN_DATE_N")
    '            SQL.Append(")")
    '            SQL.Append(" VALUES(")
    '            SQL.Append(" " & SQ(oraReader.GetString("KIN_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("KIN_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_FUKA_N")))
    '            '�}�Ԃ��J�E���g�A�b�v����
    '            'SQL.Append(",(SELECT")
    '            'SQL.Append(" TRIM(TO_CHAR(NVL(MAX(EDA_N), 0) + 1, '00'))")      ' EDA_N
    '            'SQL.Append(" FROM TENMAST")
    '            'SQL.Append(" WHERE KIN_NO_N = '" & TENVIEW.KIN_NO & "'")
    '            'SQL.Append("   AND SIT_NO_N = '" & TENVIEW.SIT_NO & "')")

    '            SQL.Append("," & SQ(oraReader.GetString("KIN_KNAME_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("KIN_NNAME_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_KNAME_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_NNAME_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NEW_KIN_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NEW_SIT_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NEW_SIT_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("KIN_DEL_DATE_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_DEL_DATE_N")))
    '            SQL.Append("," & SQ(mParam.SyoriDate))
    '            SQL.Append("," & SQ(mParam.SyoriDate))
    '            SQL.Append(")")

    '            '�܂Ƃ܂�܂ŃR�����g�@TENMAST�@�͖��g�p
    '            'iRet = MainDB.ExecuteNonQuery(SQL)

    '            'If iRet <= 0 Then
    '            '    JobMessage = "���Z�@�փ}�X�^INSERT���s"
    '            '    MainLOG.Write("���Z�@�փ}�X�^INSERT", "���s", "")
    '            '    Return False
    '            'End If

    '            '***************************************
    '            '���Z�@�֏��}�X�^
    '            '***************************************

    '            ''�������Z�@�֏�񂪑��݂���ꍇ�͓o�^���Ȃ�(��d�o�^�h�~)
    '            If CheckSameData(oraReader.GetString("KIN_NO_N"), oraReader.GetString("KIN_FUKA_N"), _
    '                             oraReader.GetString("SIT_NO_N"), oraReader.GetString("SIT_FUKA_N")) = True Then

    '            Else
    '                '�b��ǂꂭ�炢������񂩐����Ă݂�
    '                MainLOG.Write("���Z�@�փ}�X�^�o�^", "���s", "�d�� ����" _
    '                              & oraReader.GetString("KIN_NO_N") & "-" & oraReader.GetString("KIN_FUKA_N") _
    '                             & "�x�X" & oraReader.GetString("SIT_NO_N") & "-" & oraReader.GetString("SIT_FUKA_N") & " ���ɍ폜��:" & oraReader.GetString("KIN_DEL_DATE_N") & " �x�X�폜��:" & oraReader.GetString("SIT_DEL_DATE_N"))

    '            End If

    '            KinOldKey = String.Concat(oraReader.GetString("KIN_NO_N"), oraReader.GetString("KIN_FUKA_N"))

    '            If KinOldKey <> KinNewKey Then

    '                SQL = New StringBuilder(128)

    '                SQL.Append("INSERT INTO KIN_INFOMAST(")
    '                SQL.Append(" KIN_NO_N")
    '                SQL.Append(",KIN_FUKA_N")
    '                SQL.Append(",DAIHYO_NO_N")
    '                SQL.Append(",KIN_KNAME_N")
    '                SQL.Append(",KIN_NNAME_N")
    '                SQL.Append(",RYAKU_KIN_KNAME_N")
    '                SQL.Append(",RYAKU_KIN_NNAME_N")
    '                SQL.Append(",JC1_N")
    '                SQL.Append(",JC2_N")
    '                SQL.Append(",JC3_N")
    '                SQL.Append(",JC4_N")
    '                SQL.Append(",JC5_N")
    '                SQL.Append(",JC6_N")
    '                SQL.Append(",JC7_N")
    '                SQL.Append(",JC8_N")
    '                SQL.Append(",JC9_N")
    '                SQL.Append(",KIN_IDO_DATE_N")
    '                SQL.Append(",KIN_IDO_CODE_N")
    '                SQL.Append(",NEW_KIN_NO_N")
    '                SQL.Append(",NEW_KIN_FUKA_N")
    '                SQL.Append(",KIN_DEL_DATE_N")
    '                SQL.Append(",TEIKEI_KBN_N")
    '                SQL.Append(",SYUBETU_N")
    '                SQL.Append(",TIKU_CODE_N")
    '                SQL.Append(",TESUU_TANKA_N")
    '                SQL.Append(",SAKUSEI_DATE_N")
    '                SQL.Append(",KOUSIN_DATE_N")
    '                SQL.Append(")")
    '                SQL.Append(" VALUES(")
    '                SQL.Append(" " & SQ(oraReader.GetString("KIN_NO_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("KIN_FUKA_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("DAIHYO_NO_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("KIN_KNAME_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("KIN_NNAME_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("RYAKU_KIN_KNAME_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("RYAKU_KIN_NNAME_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC1_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC2_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC3_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC4_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC5_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC6_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC7_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC8_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC9_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("KIN_IDO_DATE_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("KIN_IDO_CODE_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("NEW_KIN_NO_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("KIN_DEL_DATE_N")))
    '                SQL.Append("," & SQ(GetTeikeiKbn(oraReader.GetString("KIN_NO_N"))))
    '                SQL.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(0)))
    '                SQL.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(1)))
    '                SQL.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(2)))
    '                SQL.Append("," & SQ(mParam.SyoriDate))
    '                SQL.Append("," & SQ(mParam.SyoriDate))
    '                SQL.Append(")")

    '                iRet = MainDB.ExecuteNonQuery(SQL)

    '                If iRet <= 0 Then
    '                    JobMessage = "���Z�@�֏��}�X�^INSERT���s"
    '                    MainLOG.Write("���Z�@�֏��}�X�^INSERT", "���s", "")
    '                    Return False
    '                End If

    '            End If

    '            KinNewKey = String.Concat(oraReader.GetString("KIN_NO_N"), oraReader.GetString("KIN_FUKA_N"))

    '            '***************************************
    '            '�x�X���}�X�^
    '            '***************************************
    '            SQL = New StringBuilder(128)
    '            SQL.Append("INSERT INTO SITEN_INFOMAST(")
    '            SQL.Append(" KIN_NO_N")
    '            SQL.Append(",KIN_FUKA_N")
    '            SQL.Append(",SIT_NO_N")
    '            SQL.Append(",SIT_FUKA_N")
    '            SQL.Append(",SIT_KNAME_N")
    '            SQL.Append(",SIT_NNAME_N")
    '            SQL.Append(",SEIDOKU_HYOUJI_N")
    '            SQL.Append(",YUUBIN_N")
    '            SQL.Append(",KJYU_N")
    '            SQL.Append(",NJYU_N")
    '            SQL.Append(",TKOUKAN_NO_N")
    '            SQL.Append(",DENWA_N")
    '            SQL.Append(",TENPO_ZOKUSEI_N")
    '            SQL.Append(",JIKOU_HYOUJI_N")
    '            SQL.Append(",FURI_HYOUJI_N")
    '            SQL.Append(",SYUUTE_HYOUJI_N")
    '            SQL.Append(",KAWASE_HYOUJI_N")
    '            SQL.Append(",DAITE_HYOUJI_N")
    '            SQL.Append(",JISIN_HYOUJI_N")
    '            SQL.Append(",JC_CODE_N")
    '            SQL.Append(",SIT_IDOU_DATE_N")
    '            SQL.Append(",SIT_IDO_CODE_N")
    '            SQL.Append(",NEW_KIN_NO_N")
    '            SQL.Append(",NEW_KIN_FUKA_N")
    '            SQL.Append(",NEW_SIT_NO_N")
    '            SQL.Append(",NEW_SIT_FUKA_N")
    '            SQL.Append(",SIT_DEL_DATE_N")
    '            SQL.Append(",TKOUKAN_NNAME_N")
    '            SQL.Append(",SAKUSEI_DATE_N")
    '            SQL.Append(",KOUSIN_DATE_N")
    '            SQL.Append(")")
    '            SQL.Append(" VALUES(")
    '            SQL.Append(" " & SQ(oraReader.GetString("KIN_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("KIN_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_KNAME_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_NNAME_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SEIDOKU_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("YUUBIN_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("KJYU_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NJYU_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("TKOUKAN_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("DENWA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("TENPO_ZOKUSEI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("JIKOU_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("FURI_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SYUUTE_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("KAWASE_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("DAITE_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("JISIN_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("JC_CODE_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_IDOU_DATE_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_IDO_CODE_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SITEN_NEW_KIN_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SITEN_NEW_KIN_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NEW_SIT_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NEW_SIT_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_DEL_DATE_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("TKOUKAN_NNAME_N")))
    '            SQL.Append("," & SQ(mParam.SyoriDate))
    '            SQL.Append("," & SQ(mParam.SyoriDate))
    '            SQL.Append(")")

    '            iRet = MainDB.ExecuteNonQuery(SQL)

    '            If iRet <= 0 Then
    '                JobMessage = "�x�X���}�X�^INSERT���s"
    '                MainLOG.Write("�x�X���}�X�^INSERT", "���s", "")
    '                Return False
    '            End If

    '            oraReader.NextRead()

    '            RecCnt += 1

    '            If RecCnt Mod 1000 = 0 Then Console.Write("#")

    '        Loop
    '        Return True

    '    Catch ex As Exception
    '        Throw
    '    Finally

    '    End Try
    'End Function

    ' �����f�[�^���Ȃ������`�F�b�N����
    Private Function CheckSameData(ByVal Kin As String, ByVal kin_fuka As String, ByVal sit As String, ByVal sit_fuka As String) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As New MyOracleReader(MainDB)

        Try

            SQL.Append("SELECT COUNT(*) AS COUNTER FROM KIN_INFOMAST,SITEN_INFOMAST")
            SQL.Append(" WHERE ")
            SQL.Append("  KIN_INFOMAST.KIN_NO_N = SITEN_INFOMAST.KIN_NO_N")
            SQL.Append(" AND KIN_INFOMAST.KIN_FUKA_N  = SITEN_INFOMAST.KIN_FUKA_N")
            SQL.Append(" AND KIN_INFOMAST.KIN_NO_N = '" & Kin & "'")
            SQL.Append(" AND KIN_INFOMAST.KIN_FUKA_N  = '" & kin_fuka & "'")
            SQL.Append(" AND SITEN_INFOMAST.SIT_NO_N = '" & sit & "'")
            SQL.Append(" AND SITEN_INFOMAST.SIT_FUKA_N  = '" & sit_fuka & "'")

            If OraReader.DataReader(SQL) = True Then
                If OraReader.GetInt("COUNTER") = 0 Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

    ' ���V�X�e���p�R�[�h�ϊ�
    Private Function ConvertData(ByVal inFile As String, ByVal outFile As String) As Boolean
        '�ϊ��p��`�t�@�C��
        Dim pFile As String = ""
        Dim CmdLine As String = ""

        Try

            Select Case mParam.SyoriKbn
                Case "0" '�ꊇ
                    pFile = Path.Combine(ini_info.FTR, "SSC���Z�@�փ}�X�^JIS.p")

                    CmdLine = "/nwd/ cload "
                    CmdLine &= ini_info.FTR & "FUSION ; ank ebcdic ; kanji JIS getrand "
                    ' 2016/04/22 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
                    'CmdLine &= inFile & " " & outFile & " ++" & pFile
                    CmdLine &= """" & inFile & """" & " " & """" & outFile & """" & " ++" & """" & pFile & """"
                    ' 2016/04/22 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

                Case "1" '����

                    pFile = Path.Combine(ini_info.FTR, "SSC���Z�@�փ}�X�^JIS(�����X�V).p") '2010/05/19

                    CmdLine = "/nwd/ cload "
                    CmdLine &= ini_info.FTR & "FUSION ; getrand "
                    ' 2016/04/22 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
                    'CmdLine &= inFile & " " & outFile & " ++" & pFile
                    CmdLine &= """" & inFile & """" & " " & """" & outFile & """" & " ++" & """" & pFile & """"
                    ' 2016/04/22 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

            End Select

            '------------------------------------------------------------------
            '��s�t�@�C���E�R�[�h�ϊ�
            '------------------------------------------------------------------

            Dim ExecProc As Process = Process.Start(Path.Combine(ini_info.FTRANP, "FP.exe"), CmdLine)
            If ExecProc.Id <> 0 Then
                '�I���ҋ@
                ExecProc.WaitForExit()
            Else
                JobMessage = "FTRAN�A�v���P�[�V�����̋N���Ɏ��s���܂����B"
                Return False
            End If

            If ExecProc.ExitCode <> 0 Then
                JobMessage = "�R�[�h�ϊ��Ɏ��s���܂����B"
                Return False
            End If

        Catch ex As Exception
            JobMessage = ex.Message
            Return False
        End Try

        Return True
    End Function

    Private Function GetTeikeiKbn(ByVal kin As String) As String

        Dim sql As New StringBuilder(64)
        Dim oraReader As New MyOracleReader(MainDB)


        Try

            sql.Append("SELECT TEIKEI_KBN_N FROM SSS_TKBNMAST")
            sql.Append(" WHERE ")
            sql.Append(" KIN_NO_N = '" & kin & "'")

            If oraReader.DataReader(sql) = True Then

                If oraReader.GetString("TEIKEI_KBN_N") = "" Then
                    Return "0"  '�b��
                End If

                Return oraReader.GetString("TEIKEI_KBN_N")

            Else
                Return "0"  '�b��
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

        '    '------------------------------------------------------------------
        '    '��g�敪�X�V(2008.03.13 �b��R�[�f�B���O By Astar)
        '    '------------------------------------------------------------------
        '    RecCnt = 0
        '    For i As Integer = 0 To 5 Step 1
        '        SQL.Length = 0
        '        SQL.Append("UPDATE TENMAST T1")
        '        SQL.Append(" SET TEIKEI_KBN_N = '1'")

        '        Select Case i
        '            Case 0
        '                'MSG = "��s�֌W"
        '                SQL.Append(" WHERE KIN_NO_N IN")
        '                SQL.Append(" ('0001', '0005', '0009', '0010', '0016', '0017', '0149'")
        '                SQL.Append(", '0150', '0151', '0152', '0153', '0154', '0155', '0538'")
        '                SQL.Append(", '0541', '0542', '0543', '0544', '0546', '9900')") '�䂤�����s���ǉ�
        '            Case 1
        '                'MSG = "�M�p����(���sCB,���mCB,�ȊO)"
        '                SQL.Append(" WHERE KIN_NO_N BETWEEN '1000' AND '1999'")
        '                SQL.Append(" AND NOT KIN_NO_N IN ('1610', '1881')")
        '            Case 2
        '                'MSG = "�_��(��)"
        '                SQL.Append(" WHERE KIN_NO_N = '3020'")
        '                SQL.Append(" OR KIN_NO_N BETWEEN '6129' AND '6313'")
        '            Case 3
        '                'MSG = "�_��(�É�)"
        '                SQL.Append(" WHERE KIN_NO_N = '3021'")
        '                SQL.Append(" OR KIN_NO_N BETWEEN '6328' AND '6426'")
        '            Case 4
        '                'MSG = "�_��(���m)"
        '                SQL.Append(" WHERE KIN_NO_N = '3022'")
        '                SQL.Append(" OR KIN_NO_N BETWEEN '6430' AND '6618'")
        '            Case 5
        '                'MSG = "�_��(�O�d)"
        '                SQL.Append(" WHERE KIN_NO_N = '3023'")
        '                SQL.Append(" OR KIN_NO_N BETWEEN '6631' AND '6770'")
        '        End Select

        '        '�X�V�̂��������Z�@�ւ̂ݑΏ�
        '        SQL.Append(" AND EXISTS (SELECT KIN_NO_N FROM TENMAST T2")
        '        SQL.Append(" WHERE T1.KIN_NO_N = T2.KIN_NO_N AND T2.KOUSIN_DATE_N = '" & SYORIDATE & "')")

        '        RecCnt += MainDB.ExecuteNonQuery(SQL)
        '    Next i

        '    LOG.Write("���Z�@�փ}�X�^�X�V ��g�敪�X�V", "����", "�X�V�����F" & RecCnt)
    End Function




    Private Function CreateSitenPlus(ByVal kin As String, ByVal teikeikbn As String) As String()

        Dim intSYUBETU_CODE As Integer
        Dim intTESUURYOU As Integer
        Dim intTIKU_CODE As Integer
        Dim Str() As String = New String() {"", "", ""}
        'Dim YOBI1 As Integer '�\���P�@�ŋ敪(0:�O�ŁA1:����)

        '��g����s==================
        '0001:�݂��ً�s
        '0005:�O�H�����t�e�i��s
        '0009:�O��Z�F��s
        '0010:�肻�ȋ�s
        '0016:�݂��كR�[�|���[�g��s                  
        '0017:��ʂ肻�ȋ�s
        '0149:�É���s
        '0150:�X���K��s
        '0151:������s
        '0152:��_������s
        '0153:�\�Z��s
        '0154:�O�d��s
        '0155:�S�܋�s
        '0538:�É�������s
        '0541:�򕌋�s
        '0542:���m��s
        '0543:���É���s
        '0544:������s
        '0546:��O��s
        '============================
        '��g����s��                                       ��ʃR�[�h0                 �萔����50
        '��g���M����                                       ��ʃR�[�h1                 �萔����30
        '���C�n��_����                                     ��ʃR�[�h2                 �萔����50
        '�䂤�����s��                                     ��ʃR�[�h3                 �萔����25(����)
        '���̑���g�O���Z�@�ւ�(���s�M���A���m�M���܂�)     ��ʃR�[�h4                 �萔����85

        Dim intKinCode As Integer = Integer.Parse(kin)

        Select Case intKinCode
            Case Is < 1000 '��s
                Select Case teikeikbn
                    Case "0" : intSYUBETU_CODE = 4
                    Case "1" : intSYUBETU_CODE = 0
                    Case Else : intSYUBETU_CODE = 4
                End Select

            Case Is < 2000 '�M��
                Select Case teikeikbn
                    Case "0" : intSYUBETU_CODE = 4
                    Case "1" : intSYUBETU_CODE = 1
                    Case Else : intSYUBETU_CODE = 4
                End Select

            Case 9900 '�䂤����
                intSYUBETU_CODE = 3

            Case Else
                '�_��
                If (intKinCode >= 3000 AndAlso intKinCode < 4000) OrElse _
                   (intKinCode >= 6000 AndAlso intKinCode < 9450) Then
                    Select Case teikeikbn
                        Case "0" : intSYUBETU_CODE = 4
                        Case "1" : intSYUBETU_CODE = 2
                        Case Else : intSYUBETU_CODE = 4
                    End Select

                Else '����ȊO
                    intSYUBETU_CODE = 4
                End If
        End Select

        '�n��R�[�h�A�萔���A�ŋ敪�Z�o
        Select Case intSYUBETU_CODE
            Case 0 '��g����s
                intTIKU_CODE = 0
                intTESUURYOU = 50
                'YOBI1 = 0

            Case 1 '��g���M��
                intTIKU_CODE = GetTikuCode(intKinCode)
                intTESUURYOU = 30
                'YOBI1 = 0

            Case 2 '���C�n��_��
                intTIKU_CODE = GetTikuCode(intKinCode)
                intTESUURYOU = 50
                'YOBI1 = 0

            Case 3 '�䂤����
                intTIKU_CODE = 0
                intTESUURYOU = 25
                'YOBI1 = 1

            Case 4 '��g�O���Z�@��
                intTIKU_CODE = 0
                intTESUURYOU = 85
                'YOBI1 = 0
        End Select

        'Dim sb As StringBuilder = New StringBuilder
        Str(0) = intSYUBETU_CODE.ToString
        Str(1) = intTIKU_CODE.ToString
        Str(2) = intTESUURYOU.ToString

        Return Str
    End Function

    Private Function GetTikuCode(ByVal intKinCode As Integer) As Integer
        '���m
        If intKinCode > 1549 And intKinCode < 1567 Or _
           intKinCode = 3022 Or _
           intKinCode > 6429 And intKinCode < 6619 Then
            Return 1
        End If

        '��
        If intKinCode > 1529 And intKinCode < 1541 Or _
           intKinCode = 3020 Or _
           intKinCode > 6128 And intKinCode < 6314 Then
            Return 2
        End If

        '�O�d
        If intKinCode > 1579 And intKinCode < 1586 Or _
           intKinCode = 3023 Or _
           intKinCode > 6630 And intKinCode < 6771 Then
            Return 3
        End If

        '�É�
        If intKinCode > 1500 And intKinCode < 1518 Or _
           intKinCode = 3021 Or _
           intKinCode > 6327 And intKinCode < 6427 Then
            Return 4
        End If

        '���̑�
        Return 0
    End Function

    Private Function DeleteMast() As Boolean

        Dim sql As StringBuilder
        Dim iRet As Integer

        Try

            sql = New StringBuilder(128)
            sql.Append("DELETE FROM KIN_INFOMAST")
            'NULL�A�󔒁A0�ȊO �ŏ��������ߋ��̍폜���t��ΏۂƂ���
            sql.Append(" WHERE KIN_DEL_DATE_N IS NOT NULL")
            sql.Append(" AND KIN_DEL_DATE_N NOT IN('        ', '00000000')")
            sql.Append(" AND KIN_DEL_DATE_N < '" & mParam.SyoriDate & "'")

            iRet = MainDB.ExecuteNonQuery(sql)

            If iRet < 0 Then
                JobMessage = "���Z�@�֏��}�X�^�폜���������R�[�h�폜���s"
                MainLOG.Write("���Z�@�֏��}�X�^�폜", "���s", JobMessage)

                Return False
            End If

            sql = New StringBuilder(128)
            sql.Append("DELETE FROM SITEN_INFOMAST")
            'NULL�A�󔒁A0�ȊO �ŏ��������ߋ��̍폜���t��ΏۂƂ���
            sql.Append(" WHERE SIT_DEL_DATE_N IS NOT NULL")
            sql.Append(" AND SIT_DEL_DATE_N NOT IN('        ', '00000000')")
            sql.Append(" AND SIT_DEL_DATE_N < '" & mParam.SyoriDate & "'")

            iRet = MainDB.ExecuteNonQuery(sql)

            If iRet < 0 Then
                JobMessage = "�x�X���}�X�^�폜���������R�[�h�폜���s"
                MainLOG.Write("�x�X���}�X�^�폜", "���s", JobMessage)
                Return False
            End If

            Return True

        Catch ex As Exception
            Throw
        End Try

    End Function



    'Private Class CreateTENMAST_S
    '    '----------------------------------------------------------
    '    '�r�r�r�p���Z�@�փ}�X�^�̂b�r�u���쐬����N���X
    '    '2008/06/13 mitsu �F�X�C��
    '    '2008/06/24 mitsu ���Z�@�փ}�X�^�ꊇ�X�V���ɑg�ݍ���
    '    '----------------------------------------------------------
    '    Private CurDir As String
    '    Private CsvFile As String
    '    Private SysDate As String = DateTime.Today.ToString("yyyyMMdd")
    '    Private ENCD As Encoding = Encoding.GetEncoding(932)

    '    Sub New(ByVal LoadDataFolder As String, ByRef db As MyOracle)
    '        CurDir = LoadDataFolder
    '        CsvFile = CurDir & "SSS.CSV"

    '        Try
    '            '�b�r�u�t�@�C����Shift JIS�ŏ�������
    '            Dim sw As StreamWriter = New StreamWriter(CsvFile, False, ENCD)

    '            Dim strSQL As String = "SELECT DISTINCT KIN_NO_N, KIN_KNAME_N, KIN_NNAME_N,TEIKEI_KBN_N"
    '            strSQL &= " FROM TENMAST ORDER BY KIN_NO_N"

    '            Dim OraReader As MyOracleReader = New MyOracleReader(db)

    '            If OraReader.DataReader(strSQL) Then
    '                Do
    '                    sw.WriteLine(CreateCsvLine( _
    '                        OraReader.GetString("KIN_NO_N"), _
    '                        OraReader.GetString("KIN_KNAME_N"), _
    '                        OraReader.GetString("KIN_NNAME_N"), _
    '                        OraReader.GetString("TEIKEI_KBN_N") _
    '                    ))
    '                Loop While OraReader.NextRead = True
    '            End If

    '            OraReader.Close()
    '            sw.Close()

    '            'CSV�쐬��������TENMAST_S�̒��g���N���A
    '            strSQL = "DELETE FROM TENMAST_S"
    '            Dim workDB As MyOracle = New MyOracle
    '            workDB.ExecuteNonQuery(strSQL)
    '            workDB.Commit()
    '            workDB.Close()

    '            'CTL�t�@�C���쐬
    '            CreateCtlFile()

    '            '���[�_�[����t�@�C���̎��s�R�}���h��g�ݗ��Ă�
    '            Dim CmdLine As String = "KZAMAST/KZAMAST@FSKJ_LSNR"
    '            CmdLine &= " CONTROL = '" & CurDir & "SSS.CTL'"
    '            CmdLine &= " LOG = '" & CurDir & "SSS.LOG'"

    '            Dim PSI As New ProcessStartInfo
    '            With PSI
    '                .FileName = "SQLLDR"
    '                .CreateNoWindow = True
    '                .Arguments = CmdLine
    '            End With

    '            Dim Pro As Process = Process.Start(PSI)
    '            Pro.WaitForExit()

    '        Catch ex As Exception
    '            Try
    '                Dim sw As StreamWriter = New StreamWriter(CurDir & "err.log", False, Encoding.GetEncoding(932))
    '                sw.WriteLine(ex.Message)
    '                sw.Close()

    '            Catch ex2 As Exception
    '            End Try
    '        End Try
    '    End Sub




    '    Private Function CreateCtlFile() As Boolean
    '        Dim sw As StreamWriter = New StreamWriter(CurDir & "SSS.CTL", False, ENCD)
    '        sw.AutoFlush = False

    '        sw.WriteLine("LOAD DATA")
    '        sw.WriteLine("INFILE '" & CsvFile & "' ")
    '        sw.WriteLine("PRESERVE BLANKS")
    '        sw.WriteLine("INTO TABLE KZAMAST.TENMAST_S")
    '        sw.WriteLine("FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '""'")
    '        sw.WriteLine("(KIN_NO_N,")
    '        sw.WriteLine("KIN_KNAME_N,")
    '        sw.WriteLine("KIN_NNAME_N,")
    '        sw.WriteLine("SYUBETU_N,")
    '        sw.WriteLine("TIKU_CODE_N,")
    '        sw.WriteLine("TESUU_TANKA_N,")
    '        sw.WriteLine("SAKUSEI_DATE_N,")
    '        sw.WriteLine("KOUSIN_DATE_N,")
    '        sw.WriteLine("YOBI1_N,")
    '        sw.WriteLine("YOBI2_N,")
    '        sw.WriteLine("YOBI3_N,")
    '        sw.WriteLine("YOBI4_N,")
    '        sw.WriteLine("YOBI5_N")
    '        sw.WriteLine(")")
    '        sw.Close()

    '        Return True
    '    End Function
    'End Class

End Class

