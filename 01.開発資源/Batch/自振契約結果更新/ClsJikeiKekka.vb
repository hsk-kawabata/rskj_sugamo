Option Strict On
Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
'Imports CAstSystem
Imports CASTCommon.ModPublic
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports CASTCommon
' �Z���^���U�_�񌋉ʍX�V����
Public Class ClsJikeiKekka

    Private JobTuuban As Integer

    Public MainLOG As New CASTCommon.BatchLOG("KFJ100", "���U�_�񌋉ʍX�V")

    ' �W���u���b�Z�[�W
    Private JobMessage As String = ""

    ' �N���p�����[�^ ���ʏ��
    'Private mArgumentData As CommData

    ' ���U�_�񌋉ʃt�@�C����
    Private mDataFileName As String

    ' �p�u���b�N�c�a
    Private MainDB As CASTCommon.MyOracle

    Private Const msgTitle As String = "���U�_�񌋉ʍX�V(KFJ100)"

    Structure strcIni
        Dim RIENTA_PATH As String        '���G���^�t�@�C���쐬��
        Dim DAT_PATH As String           ' DAT�̃p�X
        Dim JIKEI_RIENTAFILENAME As String    '���G���^�t�@�C�� 
        Dim JIKONKO_CODE As String    '���G���^�t�@�C�� 
    End Structure
    Private ini_info As strcIni

    ' New
    Public Sub New()
    End Sub

    ' �@�\�@ �F ���U�_�񌋉ʍX�V���� ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Function Main(ByVal CmdArg As String) As Integer

        Dim TimeStamp As String
        Dim RientaFileName As String = ""
        Dim FullRientaFileName As String = ""
        Dim TankingFileName As String = ""
        Dim bRet As Boolean = True
        Dim Param() As String = CmdArg.Split(","c)

        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        Try
            '*********************************
            ' ��������
            '*********************************
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "���C������(�J�n)", "����")
            'BatchLog.Write("0000000000-00", "00000000", "���O�C��(�J�n)", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

            MainLOG.Write("(��������)�J�n", "����", "")

            If Param.Length = 2 Then
                TimeStamp = Param(0)
                JobTuuban = CASTCommon.CAInt32(Param(1))
            Else
                TimeStamp = Param(0)
                JobTuuban = 0
            End If

            MainLOG.JobTuuban = JobTuuban
            MainLOG.FuriDate = TimeStamp.Substring(0, 8)

            MainLOG.Write("�p�����[�^�擾", "����", "�^�C���X�^���v" & TimeStamp)

            ' �I���N��
            MainDB = New CASTCommon.MyOracle

            If ini_read() = False Then
                bRet = False
            End If

            MainLOG.Write("(��������)�I��", "����", "")


            '*********************************
            ' �又��
            '*********************************
            MainLOG.Write("(�又��)�J�n", "����", "")


            '���ʃ��G���^�t�@�C���̖��O�擾
            If bRet = True Then
                If GetRientaFileName(TimeStamp, RientaFileName) = False Then
                    bRet = False
                    MainLOG.Write("�t�@�C���`�F�b�N", "���s", "���G���^�t�@�C�����擾���s")
                Else
                    MainLOG.Write("�t�@�C���`�F�b�N", "����", "���G���^�t�@�C�����擾")

                End If
            End If

            '�t���b�s�[�̃`�F�b�N
            If bRet = True Then
                Do
                    Try

                        '2012/01/13 saitou �W���C�� MODIFY ------------------------------------->>>>
                        '���G���^�쐬���INI�t�@�C���ŊǗ�����
                        Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                        'Dim DirInfo As New DirectoryInfo("A:\")
                        '2012/01/13 saitou �W���C�� MODIFY -------------------------------------<<<<
                        Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()

                        Exit Do

                    Catch ex As Exception
                        '2012/01/13 saitou �W���C�� MODIFY ------------------------------------->>>>
                        '���G���^�쐬���INI�t�@�C���ŊǗ�����
                        If MessageBox.Show(String.Format(MSG0065I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                            bRet = False
                            JobMessage = "FD�}�����L�����Z������܂����B"
                            MainLOG.Write("FD�v��", "���s", "FD�}�����L�����Z��")
                            Exit Do
                        End If
                        'If MessageBox.Show(String.Format(MSG0065I, Path.GetPathRoot("A:\")), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                        '    bRet = False
                        '    JobMessage = "FD�}�����L�����Z������܂����B"
                        '    MainLOG.Write("FD�v��", "���s", "FD�}�����L�����Z��")
                        '    Exit Do
                        'End If
                        '2012/01/13 saitou �W���C�� MODIFY -------------------------------------<<<<
                    End Try
                Loop
            End If

            '�t�@�C���̑��݃`�F�b�N
            If bRet = True Then

                '2012/01/13 saitou �W���C�� MODIFY ------------------------------------->>>>
                '���G���^�쐬���INI�t�@�C���ŊǗ�����
                FullRientaFileName = Path.Combine(ini_info.RIENTA_PATH, RientaFileName)
                'FullRientaFileName = Path.Combine("A:\", RientaFileName)
                '2012/01/13 saitou �W���C�� MODIFY -------------------------------------<<<<

                If File.Exists(FullRientaFileName) = False Then
                    JobMessage = "���̓t�@�C����������܂���"
                    MainLOG.Write("�t�@�C���`�F�b�N", "���s", "���̓t�@�C����������܂���")
                    bRet = False
                End If

            End If

            '�^���L���O�f�[�^�݂̂̃t�@�C���쐬
            If bRet = True Then
                If CreateTankingData(TimeStamp, FullRientaFileName, TankingFileName) <> 0 Then
                    MainLOG.Write("���[�N�t�@�C���쐬", "���s", "")
                    bRet = False
                Else
                    MainLOG.Write("���[�N�t�@�C���쐬", "����")

                End If
            End If

            ' ���ʍX�V����
            If bRet = True Then
                bRet = KekkaMain(TankingFileName, TimeStamp)
            End If

            If bRet = False Then
                MainDB.Rollback()
                If JobMessage = "" Then
                    MainLOG.UpdateJOBMASTbyErr("���O�Q��")
                Else
                    MainLOG.UpdateJOBMASTbyErr(JobMessage)
                End If
            Else
                MainDB.Commit()
                MainLOG.UpdateJOBMASTbyOK(JobMessage)
            End If

            If bRet = False Then
                Return 2
            End If

        Catch ex As Exception
            If Not MainDB Is Nothing Then MainDB.Rollback()
            MainLOG.Write("(�又��)", "���s", ex.Message)
            Return -1
        Finally

            If Not MainDB Is Nothing Then MainDB.Close()
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "���C������(�I��)", "����")
            'MainLOG.Write("(�又��)�I��", "����", "")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        End Try

        Return 0
    End Function

    Private Function ini_read() As Boolean

        ini_info.RIENTA_PATH = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")        '���G���^�t�@�C���쐬��
        If ini_info.RIENTA_PATH = "err" OrElse ini_info.RIENTA_PATH = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���쐬�t�H���_ ����:COMMON ����:RIENTADR")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���쐬�t�H���_ ����:COMMON ����:RIENTADR"
            Return False
        End If

        ini_info.DAT_PATH = CASTCommon.GetFSKJIni("COMMON", "DAT")           'DAT�̃p�X
        If ini_info.DAT_PATH = "err" OrElse ini_info.DAT_PATH = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:DAT�t�H���_ ����:COMMON ����:DAT")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:DAT�t�H���_ ����:COMMON ����:DAT"
            Return False
        End If

        ini_info.JIKEI_RIENTAFILENAME = CASTCommon.GetFSKJIni("COMMON", "JIKEI_RIENTAFILENAME")
        If ini_info.JIKEI_RIENTAFILENAME = "err" OrElse ini_info.JIKEI_RIENTAFILENAME = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���� ����:COMMON ����:JIKEI_RIENTAFILENAME")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���� ����:COMMON ����:JIKEI_RIENTAFILENAME"
            Return False
        End If

        ini_info.JIKONKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
        If ini_info.JIKONKO_CODE = "err" OrElse ini_info.JIKONKO_CODE = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�����ɃR�[�h ����:COMMON ����:JIKONKO_CODE")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�����ɃR�[�h ����:COMMON ����:JIKONKO_CODE"
            Return False
        End If

        Return True
    End Function


    ' �@�\�@ �F ���U�_�񌋉ʍX�V����
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function KekkaMain(ByVal TankingFileName As String, ByVal TimeStamp As String) As Boolean

        Dim fr As FileStream = Nothing
        Dim br As BinaryReader = Nothing

        Dim Enco As Encoding = Encoding.GetEncoding(50220)  'JIS

        Dim RecordLen As Integer = 262

        Dim lngTuuban As Long
        Dim strOPE_CODE As String
        Dim strKOKYAKU_NO As String
        Dim strKOUZA_NO As String
        Dim strKekka As String

        Dim errMsg(4) As String '���ۂɂ͂S�����E�E�E

        Dim KFJP044 As New clsKFJP044

        Try

            fr = New FileStream(TankingFileName, FileMode.Open, FileAccess.Read)     '�t���b�s�[�̃t�@�C��
            br = New BinaryReader(fr)

            Dim byteRecordData(RecordLen - 1) As Byte

            Dim Filelen As Long = fr.Length
            Dim Pos As Long = 0
            Dim clsKFJP044 As New clsKFJP044
            Dim DataList As New List(Of clsKFJP044.UpdateInfo)
            Dim Item As clsKFJP044.UpdateInfo = Nothing
            Do
                lngTuuban = 0
                strOPE_CODE = ""
                strKOKYAKU_NO = ""
                strKOUZA_NO = ""
                strKekka = ""
                errMsg = New String() {"", "", "", "", ""}

                byteRecordData = Nothing

                br.BaseStream.Seek(Pos, SeekOrigin.Begin)

                '1���R�[�h�Ǎ�
                byteRecordData = br.ReadBytes(RecordLen)

                '�t�@�C���̍Ō�(�ŏI�f�[�^�̎�)�́uFF FF�v
                If byteRecordData(0).ToString("X").PadLeft(2, "0"c) = "FF" And byteRecordData(1).ToString("X").PadLeft(2, "0"c) = "FF" Then
                    Exit Do
                End If

                lngTuuban = byteRecordData(16) * 256 + byteRecordData(17)

                '�I�y�R�[�h�擾
                strOPE_CODE = Enco.GetString(byteRecordData, 18, 5)

                '�������ʎ擾
                strKekka = byteRecordData(28).ToString("X").PadLeft(2, "0"c)


                '�ڋq�ԍ��擾
                strKOKYAKU_NO = Enco.GetString(byteRecordData, 32, 7)

                '�����ԍ��擾

                strKOUZA_NO = Enco.GetString(byteRecordData, 39, 7)


                '���ʂ�"23"�̏ꍇ�̂݃G���[���b�Z�[�W��ǂݎ�� 2007/08/16
                If strKekka = "23" AndAlso byteRecordData(46).ToString("X").PadLeft(2, "0"c) = "0F" Then '0x0f�������ԍ��̌�ɗ����ꍇ�G���[MSG���ݒ肳��Ă���
                    '�G���[MSG�擾

                    Dim errSet As String = ""
                    Dim cnt As Integer = 0

                    errSet = Enco.GetString(byteRecordData, 49, 213)
                    For Each TextLine As String In errSet.Split(CType(Microsoft.VisualBasic.vbCrLf, Char))
                        If TextLine.Trim.Length <> 0 Then
                            errMsg(cnt) = TextLine.Trim
                            cnt += 1
                        End If
                    Next

                End If

                Dim SQL As New StringBuilder(128)
                SQL.Append("UPDATE JIKEIMAST")
                SQL.Append(" SET")
                SQL.Append(" ERR_CODE_JR  =" & SQ(strKekka))   '�Ƃ肠����1�Ԗڂ����X�V
                SQL.Append(",ERR_MSG_JR =" & SQ(errMsg(0)))
                SQL.Append(" WHERE")
                SQL.Append("     TIME_STAMP_JR = " & SQ(TimeStamp))
                SQL.Append(" AND KAMOKU_CODE_JR = " & SQ(strOPE_CODE.Substring(0, 2)))
                SQL.Append(" AND OPE_CODE_JR = " & SQ(strOPE_CODE.Substring(2, 3)))
                SQL.Append(" AND RECORD_NO_JR        = " & lngTuuban)

                Dim nRet As Integer
                nRet = MainDB.ExecuteNonQuery(SQL)
                If nRet = 0 Then
                    MainLOG.Write("���U�_��}�X�^�X�V", "���s", "�X�V���ׂȂ�")

                    JobMessage = "�X�V���ׂȂ�"
                    Return False
                Else
                    MainLOG.Write("���U�_��}�X�^�X�V", "����", "")
                    JobMessage = ""
                End If

                '�������ʊm�F�\�̈���f�[�^��ݒ肷��
                Item.ErrCode = strKekka
                Item.ErrMsg = errMsg(0)
                Item.Kamoku = strOPE_CODE.Substring(0, 2)
                Item.OpeCode = strOPE_CODE.Substring(2, 3)
                Item.TimeStamp = TimeStamp
                Item.RecordNo = lngTuuban
                Item.Jikinko = ini_info.JIKONKO_CODE
                If KFJP044.SetData(Item, MainDB) = False Then
                    JobMessage = "�������ʊm�F�\(���U�_�񌋉�)������쐬���s"
                    Return False
                Else
                    DataList.Add(Item)
                End If

                If fr.Position <> 0 Then Pos += RecordLen

            Loop Until Filelen <= Pos

            KFJP044.OraDB = MainDB

            If KFJP044.MakeRecord(DataList) = False Then   '����ΏۂȂ�
                JobMessage = "�������ʊm�F�\(���U�_�񌋉�)������s"
                Return False
            End If

            '����o�b�`�Ăяo��
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String = ""

            '�p�����[�^�ݒ�F���O�C�����A�b�r�u�t�@�C����
            param = MainLOG.UserID & "," & KFJP044.FileName

            Dim Ret As Integer = ExeRepo.ExecReport("KFJP044.EXE", param)
            If Ret <> 0 Then
                '������s�F�߂�l�ɑΉ������G���[���b�Z�[�W��\������
                Select Case Ret
                    Case -1
                        JobMessage = "�������ʊm�F�\(���U�_�񌋉�)����ΏۂO���B"
                    Case Else
                        JobMessage = "�������ʊm�F�\(���U�_�񌋉�)������s�B�G���[�R�[�h�F" & Ret
                End Select
                MainLOG.Write("�������ʊm�F�\(���U�_�񌋉�)���", "���s", JobMessage)

                Return False
            Else
                MainLOG.Write("�������ʊm�F�\(���U�_�񌋉�)���", "����")
            End If
        Catch
            Throw
        Finally
            If Not fr Is Nothing Then fr.Close()
            If Not br Is Nothing Then br.Close()
            fr = Nothing
            br = Nothing

            If File.Exists(TankingFileName) Then File.Delete(TankingFileName)

        End Try

        Return True

    End Function

    Private Function GetRientaFileName(ByVal TimeStamp As String, ByRef FileName As String) As Boolean

        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(MainDB)

        Try

            sql.Append("SELECT FILE_NAME_JR FROM JIKEIMAST WHERE TIME_STAMP_JR = '" & TimeStamp & "'")

            If orareader.DataReader(sql) = True Then
                FileName = orareader.GetString("FILE_NAME_JR")
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function

    Private Function CreateTankingData(ByVal TimeStamp As String, ByVal RientaFileName As String, ByRef CreateFileName As String) As Integer

        '���G���^���ʃt�@�C������f�[�^���̎��o��

        '���f�[�^
        Dim fr As FileStream = Nothing
        Dim br As BinaryReader = Nothing
        '�����f�[�^(�������s���t�@�C��)
        Dim fw As FileStream = Nothing
        Dim bw As BinaryWriter = Nothing

        Dim Enco As Encoding = Encoding.GetEncoding(50220)  'JIS
        Dim headFileName As String      '���G���^�ɏ�������ł���t�@�C����
        Dim headKbn As String           '�w�b�_�敪
        Dim headHiduke As String        '�^���L���O���t

        Dim FileNameOnly As String = Path.GetFileName(RientaFileName) '�����̃t���p�X���G���^�t�@�C�������烊�G���^�t�@�C�����𔲂��o���ăZ�b�g
        'Dim FileNameOnly As String = Path.GetFileNameWithoutExtension(RientaFileName) '���ʃt�@�C���͊g���q�������邩���H

        Dim RecordLen As Integer = 256
        Dim StartPos As Integer = 1024

        Dim WorkFile As String = Path.Combine(ini_info.DAT_PATH, "RIENTER_WORK")

        Try


            fr = New FileStream(RientaFileName, FileMode.Open, FileAccess.Read)     '�t���b�s�[�̃t�@�C��
            br = New BinaryReader(fr)
            fw = New FileStream(WorkFile, FileMode.Create, FileAccess.Write)        '�������ݗp�̃t�@�C��
            bw = New BinaryWriter(fw)

            Dim byteRecordData(RecordLen - 1) As Byte

            Dim Filelen As Long = fr.Length
            Dim Pos As Long = 0

            Do
                byteRecordData = Nothing

                br.BaseStream.Seek(Pos, SeekOrigin.Begin)

                '1���R�[�h�Ǎ�
                byteRecordData = br.ReadBytes(RecordLen)

                '�ŏ��̃��R�[�h��������`�F�b�N
                If Pos = 0 Then

                    headFileName = Enco.GetString(byteRecordData, 0, 12).Trim
                    headKbn = Enco.GetString(byteRecordData, 19, 1).Trim
                    headHiduke = Enco.GetString(byteRecordData, 24, 8).Trim

                    If headFileName.CompareTo(FileNameOnly) <> 0 Then
                        JobMessage = "�t�@�C�����s��v"
                        MainLOG.Write("���[�N�t�@�C���쐬", "���s", JobMessage)
                        Return -1
                    End If

                    If headKbn.CompareTo("E") <> 0 Then
                        JobMessage = "���G���^�σt�@�C���ł͂���܂���"
                        MainLOG.Write("���[�N�t�@�C���쐬", "���s", JobMessage)
                        Return -1
                    End If

                    If headHiduke.CompareTo(TimeStamp.Substring(0, 8)) <> 0 Then
                        JobMessage = "�^���L���O���t�s��v"
                        MainLOG.Write("���[�N�t�@�C���쐬", "���s", JobMessage)
                        Return -1
                    End If

                End If

                If Pos >= StartPos Then

                    bw.Write(byteRecordData)

                End If
                If fr.Position <> 0 Then
                    Pos += RecordLen
                End If

            Loop Until Filelen <= Pos

            If Not fr Is Nothing Then fr.Close()
            If Not br Is Nothing Then br.Close()
            If Not fw Is Nothing Then fw.Close()
            If Not bw Is Nothing Then bw.Close()
            fr = Nothing
            br = Nothing
            fw = Nothing
            bw = Nothing

            CreateFileName = Path.Combine(ini_info.DAT_PATH, "TANKING_WORK")

            'FTRAN+�Ōڋq�ԍ��E�����ԍ��̃p�b�N�ϊ�
            Dim strCMD As String
            Dim strDIR As String
            Dim strFTRDIR As String = CASTCommon.GetFSKJIni("COMMON", "FTR")
            Dim strFTRANPDIR As String = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
            Dim lngEXITCODE As Long

            Dim P_File As String = "RIENTA.p"
            strDIR = Microsoft.VisualBasic.CurDir()

            Microsoft.VisualBasic.CurDir(CType(strFTRANPDIR, Char))

            strCMD = "FP /nwd/ cload " & strFTRDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & WorkFile & " " & CreateFileName & " ++" & strFTRDIR & P_File

            Dim ProcFT As New Process
            Dim ProcInfo As New ProcessStartInfo(strFTRANPDIR & "FP", strCMD.Substring(3))
            ProcInfo.CreateNoWindow = True
            ProcInfo.WorkingDirectory = strFTRANPDIR
            ProcFT = Process.Start(ProcInfo)
            ProcFT.WaitForExit()
            lngEXITCODE = ProcFT.ExitCode

            If lngEXITCODE = 0 Then
                Return 0
            Else
                JobMessage = "�R�[�h�ϊ�"
                MainLOG.Write("���[�N�t�@�C���쐬", "���s", JobMessage)
                Return 100         '�R�[�h�ϊ����s
            End If
        Catch ex As Exception
            Throw
        Finally
            If Not fr Is Nothing Then fr.Close()
            If Not br Is Nothing Then br.Close()
            If Not fw Is Nothing Then fw.Close()
            If Not bw Is Nothing Then bw.Close()
            fr = Nothing
            br = Nothing
            fw = Nothing
            bw = Nothing

            If File.Exists(WorkFile) Then File.Delete(WorkFile)

        End Try

    End Function

End Class
