Option Strict On
Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CAstSystem
Imports CASTCommon.ModPublic
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports CASTCommon
' �Z���^�������ό��ʍX�V����
Public Class ClsKessaiKekka

    ' �p�����[�^
    Private JobTuuban As Integer

    ' �W���u���b�Z�[�W
    Private JobMessage As String = ""


    ' �������ό��ʃt�@�C����
    Private mDataFileName As String

    ' �p�u���b�N�c�a
    Private MainDB As CASTCommon.MyOracle

    Private Const msgTitle As String = "�������ό��ʍX�V(KFK020)"

    Structure strcIni
        Dim RIENTA_PATH As String        '���G���^�t�@�C���쐬��
        Dim DAT_PATH As String           'DAT�̃p�X
        Dim RIENTA_FILENAME As String    '���G���^�t�@�C����
        '2018/01/23 saitou �L���M��(RSV2�W��) ADD �������σ��G���^���ʍX�V�Ή� -------------------- START
        Dim RSV2_EDITION As String       ' RSV2�@�\�ݒ�
        Dim COMMON_BAITAIREAD As String  ' �}�̓Ǎ��p�t�H���_
        '2018/01/23 saitou �L���M��(RSV2�W��) ADD ------------------------------------------------- END
    End Structure
    Private ini_info As strcIni

    ' New
    Public Sub New()
    End Sub

    ' �@�\�@ �F �������ό��ʍX�V���� ���C������
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
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "�������ό��ʍX�V����(�J�n)", "����")
            'MainLOG.Write("(��������)�J�n", "����", "")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

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
                    MainLOG.Write("���G���^�t�@�C�����擾", "���s", "���s")
                Else
                    MainLOG.Write("���G���^�t�@�C�����擾", "����")

                End If
            End If

            '�t���b�s�[�̃`�F�b�N
            If bRet = True Then
                '2018/01/23 saitou �L���M��(RSV2�W��) UPD �������σ��G���^���ʍX�V�Ή� -------------------- START
                Select Case ini_info.RSV2_EDITION
                    Case "2"
                        '��K�͔ł͉������Ȃ�

                    Case Else
                        Do
                            Try
                                Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                                Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()

                                Exit Do

                            Catch ex As Exception
                                If MessageBox.Show(String.Format(MSG0065I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                                    bRet = False
                                    JobMessage = "���[�U�[�L�����Z��"
                                    MainLOG.Write("�}�̗v��", "���s", "���[�U�[�L�����Z��")
                                    Exit Do
                                End If
                            End Try
                        Loop
                End Select
                'Do
                '    Try

                '        '20121225 maeda ���G���^�h���C�u�C��
                '        Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                '        'Dim DirInfo As New DirectoryInfo("A:\")
                '        '20121225 maeda ���G���^�h���C�u�C��
                '        Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()

                '        Exit Do

                '    Catch ex As Exception
                '        '20121225 maeda ���G���^�h���C�u�C��
                '        If MessageBox.Show(String.Format(MSG0065I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                '            'If MessageBox.Show(String.Format(MSG0065I, Path.GetPathRoot("A:\")), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                '            '20121225 maeda ���G���^�h���C�u�C��
                '            bRet = False
                '            JobMessage = "FD�}�����L�����Z������܂����B"
                '            MainLOG.Write("FD�v��", "���s", "FD�}�����L�����Z��")
                '            Exit Do
                '        End If
                '    End Try
                'Loop
                '2018/01/23 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------- END
            End If

            '�t�@�C���̑��݃`�F�b�N
            If bRet = True Then
                '2018/01/23 saitou �L���M��(RSV2�W��) UPD �������σ��G���^���ʍX�V�Ή� -------------------- START
                Select Case ini_info.RSV2_EDITION
                    Case "2"
                        FullRientaFileName = Path.Combine(ini_info.COMMON_BAITAIREAD, RientaFileName)
                    Case Else
                        FullRientaFileName = Path.Combine(ini_info.RIENTA_PATH, RientaFileName)
                End Select
                ''20121225 maeda ���G���^�h���C�u�C��
                'FullRientaFileName = Path.Combine(ini_info.RIENTA_PATH, RientaFileName)
                ''20121225 maeda ���G���^�h���C�u�C��
                '2018/01/23 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------- END

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
                    MainLOG.Write("���[�N�t�@�C���쐬", "����", "")
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
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "�������ό��ʍX�V����(�I��)", "����")
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

        ini_info.RIENTA_FILENAME = CASTCommon.GetFSKJIni("KESSAI", "RIENTANAME")
        If ini_info.RIENTA_FILENAME = "err" OrElse ini_info.RIENTA_FILENAME = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���� ����:KESSAI ����:RIENTANAME")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���� ����:KESSAI ����:RIENTANAME"
            Return False
        End If

        '2018/01/23 saitou �L���M��(RSV2�W��) ADD �������σ��G���^���ʍX�V�Ή� -------------------- START
        ini_info.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
        If ini_info.RSV2_EDITION = "err" OrElse ini_info.RSV2_EDITION = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:RSV2�@�\�ݒ� ����:RSV2_V1.0.0 ����:EDITION")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:RSV2�@�\�ݒ� ����:RSV2_V1.0.0 ����:EDITION"
            Return False
        End If

        ini_info.COMMON_BAITAIREAD = CASTCommon.GetFSKJIni("COMMON", "BAITAIREAD")
        If ini_info.COMMON_BAITAIREAD = "err" OrElse ini_info.COMMON_BAITAIREAD = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�}�̓Ǎ��p�t�H���_ ����:COMMON ����:BAITAIREAD")
            JobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�}�̓Ǎ��p�t�H���_ ����:COMMON ����:BAITAIREAD"
            Return False
        End If
        '2018/01/23 saitou �L���M��(RSV2�W��) ADD ------------------------------------------------- END

        Return True
    End Function


    ' �@�\�@ �F �������ό��ʍX�V����
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

        Try

            fr = New FileStream(TankingFileName, FileMode.Open, FileAccess.Read)     '�t���b�s�[�̃t�@�C��
            br = New BinaryReader(fr)

            Dim byteRecordData(RecordLen - 1) As Byte

            Dim Filelen As Long = fr.Length
            Dim Pos As Long = 0
            Dim KFKP003 As New KFKP003
            Dim DataList As New List(Of KFKP003.UpdateInfo)
            Dim Item As KFKP003.UpdateInfo = Nothing
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
                SQL.Append("UPDATE KESSAIMAST")
                SQL.Append(" SET")
                SQL.Append(" ERR_CODE_KR  =" & SQ(strKekka))   '�Ƃ肠����1�Ԗڂ����X�V
                SQL.Append(",ERR_MSG_KR =" & SQ(errMsg(0)))
                SQL.Append(" WHERE")
                SQL.Append("     TIME_STAMP_KR = " & SQ(TimeStamp))
                SQL.Append(" AND KAMOKU_CODE_KR = " & SQ(strOPE_CODE.Substring(0, 2)))
                SQL.Append(" AND OPE_CODE_KR = " & SQ(strOPE_CODE.Substring(2, 3)))
                SQL.Append(" AND RECORD_NO_KR        = " & lngTuuban)

                Dim nRet As Integer
                nRet = MainDB.ExecuteNonQuery(SQL)
                If nRet = 0 Then
                    MainLOG.Write("�������σ}�X�^�X�V", "���s", "�X�V���ׂȂ�")
                    JobMessage = "�X�V���ׂȂ�"
                    Return False
                Else
                    MainLOG.Write("�������σ}�X�^�X�V", "����", "")

                    JobMessage = ""
                End If

                '�������ʊm�F�\�̈���f�[�^��ݒ肷��
                Item.ErrCode = strKekka
                Item.ErrMsg = errMsg(0)
                Item.Kamoku = strOPE_CODE.Substring(0, 2)
                Item.OpeCode = strOPE_CODE.Substring(2, 3)
                Item.TimeStamp = TimeStamp
                Item.RecordNo = lngTuuban
                If KFKP003.SetData(Item, MainDB) = False Then
                    JobMessage = "�������ʊm�F�\(�������ό���)������쐬���s"
                    Return False
                Else
                    DataList.Add(Item)
                End If

                If fr.Position <> 0 Then Pos += RecordLen

            Loop Until Filelen <= Pos

            If KFKP003.MakeRecord(DataList) = False Then   '����ΏۂȂ�
                JobMessage = "�������ʊm�F�\(�������ό���)������s"
                Return False
            End If

            '����o�b�`�Ăяo��
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String = ""

            '�p�����[�^�ݒ�F���O�C�����A�b�r�u�t�@�C����
            param = MainLOG.UserID & "," & KFKP003.FileName

            Dim Ret As Integer = ExeRepo.ExecReport("KFKP003.EXE", param)
            If Ret <> 0 Then
                '������s�F�߂�l�ɑΉ������G���[���b�Z�[�W��\������
                Select Case Ret
                    Case -1
                        JobMessage = "�������ʊm�F�\(�������ό���)����ΏۂO���B"
                    Case Else
                        JobMessage = "�������ʊm�F�\(�������ό���)������s�B�G���[�R�[�h�F" & Ret
                End Select
                MainLOG.Write("�������ʊm�F�\(�������ό���)���", "���s", JobMessage)

                Return False
            Else
                MainLOG.Write("�������ʊm�F�\(�������ό���)���", "����")
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

            sql.Append("SELECT FILE_NAME_KR FROM KESSAIMAST WHERE TIME_STAMP_KR = " & TimeStamp)

            If orareader.DataReader(sql) = True Then
                FileName = orareader.GetString("FILE_NAME_KR")
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

                    '2014/05/01 saitou �W���ŏC�� DEL -------------------------------------------------->>>>
                    '�t�@�C�����̃t�@�C�����͊g���q�������Ȃ��Ă��邽�߁A�t�@�C�����̃`�F�b�N�͍s��Ȃ��B
                    'If headFileName.CompareTo(FileNameOnly) <> 0 Then
                    '    JobMessage = "�t�@�C�����s��v"
                    '    MainLOG.Write("���[�N�t�@�C���쐬", "���s", JobMessage)
                    '    Return -1
                    'End If
                    '2014/05/01 saitou �W���ŏC�� DEL --------------------------------------------------<<<<

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
