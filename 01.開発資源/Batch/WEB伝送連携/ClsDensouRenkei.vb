Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon
Imports Microsoft.VisualBasic
Imports System.Diagnostics

' WEB�`���A�g����
Public Class ClsDensouRenkei

    ' �p�u���b�N�c�a
    Private MainDB As CASTCommon.MyOracle

    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***
    ' �T�u�c�a
    Private SubDB As CASTCommon.MyOracle
    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***

    ' ���b�Z�[�W
    Public Message As String = ""

    Public JobTuuban As Integer = 0

    Private FilePath As String = ""
    Private FileName As String = ""

    Private clsFUSION As New clsFUSION.clsMain
    Public S_DATFileArray As New System.Collections.Specialized.StringCollection

    Private MOJICode As Encoding

    '�t�@�C����
    Private RENKEI_FILENAME As String = ""

    '�ꎞ�t�@�C��
    Private TmpFileName As String = ""

    Private DelFlg As Integer = 0

    Private Koufuri_kbn As Integer = 1      '1�F���U�@3�F���U

    Private File_Byte As Integer = 120       '120�o�C�g

    '�}���`�敪
    Private Multi_Kbn As String = "0"

    '2012/12/05 saitou WEB�`�� UPD -------------------------------------------------->>>>
    Protected Friend END_KEN As String = String.Empty       'END�t�@�C�����̌���(�p�����[�^���)
    Protected Friend END_KIN As String = String.Empty       'END�t�@�C�����̋��z(�p�����[�^���)
    '2012/12/05 saitou WEB�`�� UPD --------------------------------------------------<<<<

    ' 2015/12/28 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
    Friend Structure INI_PARAM
        Dim RSV2_EDITION As String                          ' RSV2�@�\�ݒ�
    End Structure
    Private Ini_Info As INI_PARAM

    Private TimeStamp As String
    ' 2015/12/28 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

    ' �@�\�@ �F ���C��
    '
    ' �߂�l �F 0 - ���� �C 0�ȊO - �ُ�
    '
    ' ���l�@ �F 
    Public Function Main(ByVal command As String) As Integer

        MainLOG.ToriCode = "0000000000-00"
        MainLOG.FuriDate = "00000000"
        MainLOG.UserID = "Densou"

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 30
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME4")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 30
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***


        Try
            FilePath = Path.GetDirectoryName(command)
            FileName = Path.GetFileName(command)

            ' �I���N��
            MainDB = New CASTCommon.MyOracle
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***
            SubDB = New CASTCommon.MyOracle
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***

            '�t�H���_�����������WEB�`���t�H���_�Ƃ���
            If FilePath = "" Then
                RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), FileName)
            Else
                RENKEI_FILENAME = Path.Combine(FilePath, FileName)
            End If

            '�p�����[�^�ۑ��p
            Dim bRet As Boolean

            ' 2015/12/28 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
            If GetIniInfo() = False Then
                Message = "�ݒ�t�@�C���擾���s ���O�Q��"
                bRet = False
            End If

            TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff")
            ' 2015/12/28 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

            If File.Exists(RENKEI_FILENAME) = False Then
                MainLOG.Write("WEB�`���t�@�C���擾", "���s", "WEB�`���f�[�^�t�@�C���Ȃ��F" & RENKEI_FILENAME)
                Message = "WEB�`���f�[�^�t�@�C���Ȃ��F" & RENKEI_FILENAME

                bRet = False
            Else
                '�t�@�C������

                If fn_FileSplit(RENKEI_FILENAME) = False Then
                    bRet = False
                Else

                    For i As Integer = 0 To S_DATFileArray.Count - 1
                        Dim JobParam As String = ""

                        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***
                        ' �g�����J�n
                        SubDB.BeginTrans()

                        ' WEB_RIREKIMAST�o�^���b�N
                        dblock = New CASTCommon.CDBLock
                        If dblock.InsertWEB_RIREKIMAST_Lock(MainDB, LockWaitTime) = False Then
                            ' ���[���o�b�N
                            SubDB.Rollback()
                            MainLOG.Write_Err("WEB_RIREKIMAST�o�^", "���s", "InsertWEB_RIREKIMAST�����Ń^�C���A�E�g")
                            Message = "InsertWEB_RIREKIMAST�����Ń^�C���A�E�g"
                            bRet = False
                            Exit For
                        End If
                        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***

                        bRet = TourokuFile(S_DATFileArray.Item(i), JobParam)

                        If bRet = True Then

                            bRet = InsertJobMast(JobParam)

                        End If

                        If bRet = False Then
                            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***
                            ' WEB_RIREKIMAST�o�^�A�����b�N
                            dblock.InsertWEB_RIREKIMAST_UnLock(MainDB)

                            ' ���[���o�b�N
                            SubDB.Rollback()
                            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***

                            For j As Integer = 0 To S_DATFileArray.Count - 1
                                File.Delete(S_DATFileArray.Item(j))
                            Next

                            Exit For
                        End If

                        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***
                        ' WEB_RIREKIMAST�o�^�A�����b�N
                        dblock.InsertWEB_RIREKIMAST_UnLock(MainDB)

                        ' �R�~�b�g
                        SubDB.Commit()
                        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***
                    Next
                End If


                '�ꎞ�t�@�C�����폜
                Try
                    If File.Exists(TmpFileName) = True Then
                        File.Delete(TmpFileName)
                    End If

                Catch ex As Exception
                    MainLOG.Write("�ꎞ�t�@�C���폜", "���s", TmpFileName & "�@" & ex.Message)
                End Try

            End If

            '�G���[������ꍇ
            If bRet = False Then
                '�W���u�Ď��ɓo�^����
                If JobTuuban = 0 Then
                    InsertJOBMASTbyError(Message)
                Else
                    MainLOG.UpdateJOBMASTbyErr(Message)
                End If
                '���[���o�b�N
                MainDB.Rollback()


                Return 2
            End If

            '����I�����R�~�b�g
            If JobTuuban > 0 Then
                MainLOG.UpdateJOBMASTbyOK(Message)
            End If
            MainDB.Commit()

            '�t�@�C����WEB_REV_BK�ֈړ�
            If S_DATFileArray.Count <> 1 Then

                Dim DestFile As String = ""
                Try

                    DestFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV_BK"), FileName)

                    '�O��t�@�C�����폜
                    If File.Exists(DestFile) = True Then
                        File.Delete(DestFile)
                    End If

                    File.Move(RENKEI_FILENAME, DestFile)

                Catch ex As Exception
                    MainLOG.Write("����t�H���_�ړ�", "���s", RENKEI_FILENAME & " -> " & DestFile)
                    
                End Try

                Return 0
                '�t���O��1�̏ꍇ�A�V���O���t�@�C�������l�[�������Ƃ݂Ȃ�
            ElseIf S_DATFileArray.Count = 1 And DelFlg = 1 Then
                Dim DestFile As String = ""
                Try

                    DestFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV_BK"), FileName)

                    '�O��t�@�C�����폜
                    If File.Exists(DestFile) = True Then
                        File.Delete(DestFile)
                    End If

                    File.Move(RENKEI_FILENAME, DestFile)

                Catch ex As Exception
                    MainLOG.Write("����t�H���_�ړ�", "���s", RENKEI_FILENAME & " -> " & DestFile)
                    
                End Try

                Return 0
            Else
                Return 0
            End If

        Catch
            Throw
        Finally
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***
            If Not SubDB Is Nothing Then
                ' WEB_RIREKIMAST�o�^�A�����b�N
                dblock.InsertWEB_RIREKIMAST_UnLock(MainDB)

                SubDB.Rollback()
                SubDB.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iWEB_RIREKIMAST�o�^���̈�Ӑ��ۏ؁j ***

            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

        Return 0

    End Function

    Private Function TourokuFile(ByVal filename As String, ByRef jobparam As String) As Boolean
        Dim ParamFile As String = ""
        '�G���h���R�[�h�ǉ�
        If fn_AddEndRecord(filename) = False Then
            Return False
        End If

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Dim OraReader As MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

        '�ϑ��҃R�[�h��̓��C������
        Try
            Using fs As FileStream = New FileStream(filename, FileMode.Open, FileAccess.Read)
                Dim Rec(0) As Byte              '���R�[�h�敪�i�[�p
                Dim Hed As Byte                 '�w�b�_���R�[�h�̃f�[�^�敪
                Dim Enc As System.Text.Encoding '�G���R�[�f�B���O

                '�t�@�C���擪1�o�C�g��ǂݎ��G���R�[�h����
                fs.Read(Rec, 0, 1)

                Select Case Rec(0)
                    Case 49 'SJIS.GetBytes("1"c)(0)�ɊY��
                        Hed = 49
                        Enc = Encoding.GetEncoding("SHIFT-JIS")

                    Case 241 'EncdE.GetBytes("1"c)(0)�ɊY��
                        Hed = 241
                        Enc = Encoding.GetEncoding("IBM290")

                    Case Else
                        MainLOG.Write("�G���R�[�h����", "���s", "�t�@�C�����F" & filename)
                        Message = "�G���R�[�h���莸�s �t�@�C�����F" & filename
                        Return False
                End Select

                '�t�@�C���擪�܂ŃV�[�N
                fs.Seek(0, SeekOrigin.Begin)

                '�w�b�_���R�[�h�Ǎ�
                'Dim Header(119) As Byte
                Dim Header(121) As Byte
                'fs.Read(Header, 0, 120)
                fs.Read(Header, 0, File_Byte)

                Dim Syubetu As String = Enc.GetString(Header, 1, 2)     '���
                Dim CodeKbn As String = Enc.GetString(Header, 3, 1)     '�R�[�h�敪
                Dim ItakuCode As String = Enc.GetString(Header, 4, 10)  '�ϑ��҃R�[�h
                Dim FuriDate As String = Enc.GetString(Header, 54, 4)   '�U����
                Dim KinCode As String = Enc.GetString(Header, 58, 4)      '���Z�@�փR�[�h
                Dim SitCode As String = Enc.GetString(Header, 77, 3)      '�x�X�R�[�h
                Dim Kamoku As String = Enc.GetString(Header, 95, 1)       '�Ȗ�
                Dim Kouza As String = Enc.GetString(Header, 96, 7)        '�����ԍ�

                Dim CheckFlg As String = GetFSKJIni("WEB_DEN", "HED_CHECK")     '�w�b�_�̓X/�Ȗ�/������0:�`�F�b�N����,1:�`�F�b�N���Ȃ�

                If Syubetu = "91" Then
                    Koufuri_kbn = 1
                Else
                    Koufuri_kbn = 3
                End If

                '�w�b�_���`�F�b�N
                '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                'Dim OraReader As MyOracleReader = New MyOracleReader(MainDB)
                OraReader = New MyOracleReader(MainDB)
                '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                Dim SQL As StringBuilder = New StringBuilder

                '*** Str Del 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                'OraReader = New CASTCommon.MyOracleReader
                '*** End Del 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                SQL = New StringBuilder(128)
                SQL.AppendLine("SELECT")
                SQL.AppendLine(" TORIS_CODE_T")
                SQL.AppendLine(",TORIF_CODE_T")
                SQL.AppendLine(",BAITAI_CODE_T")
                SQL.AppendLine(",FMT_KBN_T")
                SQL.AppendLine(",CODE_KBN_T")
                SQL.AppendLine(",LABEL_KBN_T")
                SQL.AppendLine(",FILE_NAME_T")
                SQL.AppendLine(",MULTI_KBN_T")
                SQL.AppendLine(",ITAKU_KANRI_CODE_T")
                SQL.AppendLine(",SFURI_FLG_T")
                SQL.AppendLine(" FROM TORIMAST , SCHMAST ")
                '�����ǉ� start
                SQL.AppendLine(" WHERE TORIS_CODE_T = TORIS_CODE_S")
                SQL.AppendLine(" AND TORIF_CODE_T = TORIF_CODE_S")
                SQL.AppendLine(" AND BAITAI_CODE_T = '10'  ")
                SQL.AppendLine(" AND FURI_DATE_S = '" & ConvertDate(FuriDate, "yyyyMMdd") & "'")
                'end
                SQL.AppendLine(" AND ITAKU_CODE_T = '" & ItakuCode & "'")
                SQL.AppendLine(" AND SYUBETU_T = '" & Syubetu & "'")
                SQL.AppendLine(" ORDER BY SFURI_FLG_T DESC")

                If OraReader.DataReader(SQL) Then

                    If OraReader.GetString("SFURI_FLG_T") <> "1" Then

                        '�p�����[�^�g�ݗ���
                        Dim param As StringBuilder = New StringBuilder
                        param.Append(OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T"))
                        param.Append("," & ConvertDate(FuriDate, "yyyyMMdd"))
                        param.Append("," & OraReader.GetString("CODE_KBN_T"))
                        param.Append("," & OraReader.GetString("FMT_KBN_T"))
                        param.Append("," & OraReader.GetString("BAITAI_CODE_T"))
                        param.Append("," & OraReader.GetString("LABEL_KBN_T"))
                        '���Ƃ����݃t�@�C�������
                        ' 2015/12/28 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                        'If OraReader.GetString("FILE_NAME_T") = "" Then
                        '    If OraReader.GetString("MULTI_KBN_T") = "0" Then
                        '        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                        '                                 "D" & OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & ".DAT")
                        '    Else
                        '        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                        '                                 "D" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT")
                        '    End If
                        'Else
                        '    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), OraReader.GetString("FILE_NAME_T"))
                        'End If
                        '=========================================================
                        ' �t�@�C�����ݒ�i�ĐU�Ȃ��j
                        '=========================================================
                        Select Case Ini_Info.RSV2_EDITION
                            Case "2"
                                '-------------------------------------------------
                                ' ��K�͐ݒ�\�z
                                '-------------------------------------------------
                                If OraReader.GetString("MULTI_KBN_T") = "0" Then
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "J_WEB_S_" & _
                                                             OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & "_" & _
                                                             OraReader.GetString("FMT_KBN_T") & "_" & _
                                                             ConvertDate(FuriDate, "yyyyMMdd") & "_" & _
                                                             TimeStamp & "_" & _
                                                             Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                                             "000" & _
                                                             ".DAT")
                                Else
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "J_WEB_M_" & _
                                                             OraReader.GetString("ITAKU_KANRI_CODE_T") & "00" & "_" & _
                                                             OraReader.GetString("FMT_KBN_T") & "_" & _
                                                             ConvertDate(FuriDate, "yyyyMMdd") & "_" & _
                                                             TimeStamp & "_" & _
                                                             Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                                             "000" & _
                                                             ".DAT")
                                End If
                            Case Else
                                '-------------------------------------------------
                                ' �W���ݒ�\�z
                                '-------------------------------------------------
                                If OraReader.GetString("MULTI_KBN_T") = "0" Then
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "D" & OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & ".DAT")
                                Else
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "D" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT")
                                End If
                        End Select
                        ' 2015/12/28 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

                        '�t���O��1�̏ꍇ�A�V���O���t�@�C�������l�[�������Ƃ݂Ȃ�
                        If RENKEI_FILENAME.ToUpper <> ParamFile.ToUpper Then
                            DelFlg = 1
                        End If

                        'WEB�`�������}�X�^�ǉ�
                        InsertWEB_RIREKIMAST(OraReader.GetString("TORIS_CODE_T"), OraReader.GetString("TORIF_CODE_T"), ConvertDate(FuriDate, "yyyyMMdd"), OraReader.GetString("ITAKU_KANRI_CODE_T"))

                        jobparam = param.ToString
                        OraReader.Close()
                    Else
                        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                        OraReader.Close()
                        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                        '�ĐU�L�̏ꍇ�́A�����ϑ��҃R�[�h���������݂��邽�߁A�X�P�W���[������
                        '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                        'OraReader = New CASTCommon.MyOracleReader
                        OraReader = New CASTCommon.MyOracleReader(MainDB)
                        '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                        SQL = New StringBuilder(128)
                        SQL.AppendLine("SELECT")
                        SQL.AppendLine(" TORIS_CODE_T")
                        SQL.AppendLine(",TORIF_CODE_T")
                        SQL.AppendLine(",BAITAI_CODE_T")
                        SQL.AppendLine(",FMT_KBN_T")
                        SQL.AppendLine(",CODE_KBN_T")
                        SQL.AppendLine(",LABEL_KBN_T")
                        SQL.AppendLine(",FILE_NAME_T")
                        SQL.AppendLine(",MULTI_KBN_T")
                        SQL.AppendLine(",ITAKU_KANRI_CODE_T")
                        SQL.AppendLine(" FROM TORIMAST,SCHMAST")
                        SQL.AppendLine(" WHERE ITAKU_CODE_T = '" & ItakuCode & "'")
                        SQL.AppendLine(" AND SYUBETU_T = '" & Syubetu & "'")
                        SQL.AppendLine(" AND FURI_DATE_S = '" & ConvertDate(FuriDate, "yyyyMMdd") & "'")
                        SQL.AppendLine(" AND TORIS_CODE_T = TORIS_CODE_S")
                        SQL.AppendLine(" AND TORIF_CODE_T = TORIF_CODE_S")
                        '2014/05/01 saitou �W���C�� ADD -------------------------------------------------->>>>
                        '�����ǉ�
                        SQL.AppendLine(" AND BAITAI_CODE_T = '10'")
                        '2014/05/01 saitou �W���C�� ADD --------------------------------------------------<<<<

                        If OraReader.DataReader(SQL) Then
                            '�p�����[�^�g�ݗ���
                            Dim param As StringBuilder = New StringBuilder
                            param.Append(OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T"))
                            param.Append("," & ConvertDate(FuriDate, "yyyyMMdd"))
                            param.Append("," & OraReader.GetString("CODE_KBN_T"))
                            param.Append("," & OraReader.GetString("FMT_KBN_T"))
                            param.Append("," & OraReader.GetString("BAITAI_CODE_T"))
                            param.Append("," & OraReader.GetString("LABEL_KBN_T"))
                            '���Ƃ����݃t�@�C�������
                            ' 2015/12/28 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                            'If OraReader.GetString("FILE_NAME_T") = "" Then
                            '    If OraReader.GetString("MULTI_KBN_T") = "0" Then
                            '        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                            '                                 "D" & OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & ".DAT")
                            '    Else
                            '        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                            '                                 "D" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT")
                            '    End If
                            'Else
                            '    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), OraReader.GetString("FILE_NAME_T"))
                            'End If
                            '=========================================================
                            ' �t�@�C�����ݒ�i�ĐU����j
                            '=========================================================
                            Select Case Ini_Info.RSV2_EDITION
                                Case "2"
                                    '-------------------------------------------------
                                    ' ��K�͐ݒ�\�z
                                    '-------------------------------------------------
                                    If OraReader.GetString("MULTI_KBN_T") = "0" Then
                                        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                                 "J_WEB_S_" & _
                                                                 OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & "_" & _
                                                                 OraReader.GetString("FMT_KBN_T") & "_" & _
                                                                 ConvertDate(FuriDate, "yyyyMMdd") & "_" & _
                                                                 TimeStamp & "_" & _
                                                                 Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                                                 "000" & _
                                                                 ".DAT")
                                    Else
                                        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                                 "J_WEB_M_" & _
                                                                 OraReader.GetString("ITAKU_KANRI_CODE_T") & "00" & "_" & _
                                                                 OraReader.GetString("FMT_KBN_T") & "_" & _
                                                                 ConvertDate(FuriDate, "yyyyMMdd") & "_" & _
                                                                 TimeStamp & "_" & _
                                                                 Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                                                 "000" & _
                                                                 ".DAT")
                                    End If
                                Case Else
                                    '-------------------------------------------------
                                    ' �W���ݒ�\�z
                                    '-------------------------------------------------
                                    If OraReader.GetString("MULTI_KBN_T") = "0" Then
                                        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                                 "D" & OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & ".DAT")
                                    Else
                                        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                                 "D" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT")
                                    End If
                            End Select
                            ' 2015/12/28 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

                            '�t���O��1�̏ꍇ�A�V���O���t�@�C�������l�[�������Ƃ݂Ȃ�
                            If RENKEI_FILENAME.ToUpper <> ParamFile.ToUpper Then
                                DelFlg = 1
                            End If

                            'WEB�`�������}�X�^�ǉ�
                            InsertWEB_RIREKIMAST(OraReader.GetString("TORIS_CODE_T"), OraReader.GetString("TORIF_CODE_T"), ConvertDate(FuriDate, "yyyyMMdd"), OraReader.GetString("ITAKU_KANRI_CODE_T"))

                            jobparam = param.ToString
                            OraReader.Close()
                        Else
                            Message = "�X�P�W���[�����擾���s �ϑ��҃R�[�h�F" & ItakuCode & " ��ʃR�[�h�F" & Syubetu & " �t�@�C�����F" & RENKEI_FILENAME & " �U�֓�:" & FuriDate
                            MainLOG.Write("�X�P�W���[�����擾", "���s", "�X�P�W���[�����擾���s �ϑ��҃R�[�h�F" & ItakuCode & " ��ʃR�[�h�F" & Syubetu & " �t�@�C�����F" & RENKEI_FILENAME & " �U�֓�:" & FuriDate)
                            
                            OraReader.Close()

                            Return False
                        End If
                    End If

                ElseIf GetFSKJIni("OPTION", "SOUFURI") = "1" Then   '���U�I�v�V�������g�p���Ă��邩�H
                    '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                    OraReader.Close()
                    '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                    '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                    'OraReader = New CASTCommon.MyOracleReader
                    OraReader = New CASTCommon.MyOracleReader(MainDB)
                    '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                    SQL = New StringBuilder(128)
                    '���U�łȂ��ꍇ�A���U���Ŋm�F
                    SQL.AppendLine("SELECT")
                    SQL.AppendLine(" TORIS_CODE_T")
                    SQL.AppendLine(",TORIF_CODE_T")
                    SQL.AppendLine(",BAITAI_CODE_T")
                    SQL.AppendLine(",FMT_KBN_T")
                    SQL.AppendLine(",CODE_KBN_T")
                    SQL.AppendLine(",LABEL_KBN_T")
                    SQL.AppendLine(",FILE_NAME_T")
                    SQL.AppendLine(",MULTI_KBN_T")
                    SQL.AppendLine(",ITAKU_KANRI_CODE_T")
                    SQL.AppendLine(" FROM S_TORIMAST")
                    SQL.AppendLine(" WHERE ITAKU_CODE_T = '" & ItakuCode & "'")
                    SQL.AppendLine(" AND SYUBETU_T = '" & Syubetu & "'")
                    '2014/05/01 saitou �W���C�� ADD -------------------------------------------------->>>>
                    '�����ǉ�
                    SQL.AppendLine(" AND BAITAI_CODE_T = '10'")
                    '2014/05/01 saitou �W���C�� ADD --------------------------------------------------<<<<
                    If CheckFlg = "1" Then
                        SQL.AppendLine(" AND TKIN_NO_T = '" & KinCode & "'")
                        SQL.AppendLine(" AND TSIT_NO_T = '" & SitCode & "'")
                        SQL.AppendLine(" AND KAMOKU_T = '" & CASTCommon.ConvertKamoku1TO2(Kamoku) & "'")
                        SQL.AppendLine(" AND KOUZA_T = '" & Kouza & "'")
                    End If
                    SQL.AppendLine(" AND ROWNUM = 1")

                    If OraReader.DataReader(SQL) Then
                        '�p�����[�^�g�ݗ���
                        Dim param As StringBuilder = New StringBuilder
                        param.Append(OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T"))
                        param.Append("," & ConvertDate(FuriDate, "yyyyMMdd"))
                        param.Append("," & OraReader.GetString("CODE_KBN_T"))
                        param.Append("," & OraReader.GetString("FMT_KBN_T"))
                        param.Append("," & OraReader.GetString("BAITAI_CODE_T"))
                        param.Append("," & OraReader.GetString("LABEL_KBN_T"))
                        '���Ƃ����݃t�@�C�������
                        ' 2015/12/28 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                        'If OraReader.GetString("FILE_NAME_T") = "" Then
                        '    If OraReader.GetString("MULTI_KBN_T") = "0" Then
                        '        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                        '                                 "D" & OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & ".DAT")
                        '    Else
                        '        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                        '                                 "D" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT")
                        '    End If
                        'Else
                        '    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), OraReader.GetString("FILE_NAME_T"))
                        'End If
                        '=========================================================
                        ' �t�@�C�����ݒ�i�����U���j
                        '=========================================================
                        Select Case Ini_Info.RSV2_EDITION
                            Case "2"
                                '-------------------------------------------------
                                ' ��K�͐ݒ�\�z
                                '-------------------------------------------------
                                If OraReader.GetString("MULTI_KBN_T") = "0" Then
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "S_WEB_S_" & _
                                                             OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & "_" & _
                                                             OraReader.GetString("FMT_KBN_T") & "_" & _
                                                             ConvertDate(FuriDate, "yyyyMMdd") & "_" & _
                                                             TimeStamp & "_" & _
                                                             Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                                             "000" & _
                                                             ".DAT")
                                Else
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "S_WEB_M_" & _
                                                             OraReader.GetString("ITAKU_KANRI_CODE_T") & "00" & "_" & _
                                                             OraReader.GetString("FMT_KBN_T") & "_" & _
                                                             ConvertDate(FuriDate, "yyyyMMdd") & "_" & _
                                                             TimeStamp & "_" & _
                                                             Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                                             "000" & _
                                                             ".DAT")
                                End If
                            Case Else
                                '-------------------------------------------------
                                ' �W���ݒ�\�z
                                '-------------------------------------------------
                                If OraReader.GetString("MULTI_KBN_T") = "0" Then
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "D" & OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & ".DAT")
                                Else
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "D" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT")
                                End If
                        End Select
                        ' 2015/12/28 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

                        '�t���O��1�̏ꍇ�A�V���O���t�@�C�������l�[�������Ƃ݂Ȃ�
                        If RENKEI_FILENAME.ToUpper <> ParamFile.ToUpper Then
                            DelFlg = 1
                        End If

                        'WEB�`�������}�X�^�ǉ�
                        InsertWEB_RIREKIMAST(OraReader.GetString("TORIS_CODE_T"), OraReader.GetString("TORIF_CODE_T"), ConvertDate(FuriDate, "yyyyMMdd"), OraReader.GetString("ITAKU_KANRI_CODE_T"))

                        jobparam = param.ToString
                        OraReader.Close()

                    Else
                        ' �������擾���s

                        Message = "�������擾���s �ϑ��҃R�[�h�F" & ItakuCode & " ��ʃR�[�h�F" & Syubetu & " �t�@�C�����F" & RENKEI_FILENAME
                        MainLOG.Write("�������擾", "���s", "�������擾���s �ϑ��҃R�[�h�F" & ItakuCode & " ��ʃR�[�h�F" & Syubetu & " �t�@�C�����F" & RENKEI_FILENAME)
                        OraReader.Close()

                        Return False
                    End If
                Else
                    ' �������擾���s

                    Message = "�������擾���s �ϑ��҃R�[�h�F" & ItakuCode & " ��ʃR�[�h�F" & Syubetu & " �t�@�C�����F" & RENKEI_FILENAME
                    MainLOG.Write("�������擾", "���s", "�������擾���s �ϑ��҃R�[�h�F" & ItakuCode & " ��ʃR�[�h�F" & Syubetu & " �t�@�C�����F" & RENKEI_FILENAME)
                    
                    OraReader.Close()

                    Return False
                End If
            End Using
            '����ϑ��҂̗�����������܂őҋ@����
            If DelFlg <> 0 Then
                For cnt As Integer = 1 To 300 Step 1
                    Try
                        If File.Exists(ParamFile) = True Then
                            If cnt = 300 Then
                                Message = "��ḑ�ّ��M�װ:" & ParamFile
                                MainLOG.Write("�t�@�C���R�s�[", "���s", "�t�@�C�����F" & ParamFile)
                                Return False
                            End If
                        Else
                            File.Copy(filename, ParamFile, True)
                            '�ꎞ�t�@�C������
                            File.Delete(filename)
                            Exit For
                        End If
                    Catch
                    End Try
                    System.Threading.Thread.Sleep(100)
                Next
            Else
                File.Copy(filename, ParamFile, True)
                '�ꎞ�t�@�C������
                File.Delete(filename)
            End If

        Catch ex As Exception
            MainLOG.Write("�������擾", "���s", "�t�@�C�����F" & RENKEI_FILENAME & " " & ex.Message)
            
            Message = "�������擾���s �t�@�C�����F" & RENKEI_FILENAME
            Return False

            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

        End Try

        Return True
    End Function

    Private Function InsertJobMast(ByVal param As String) As Boolean
        '------------------------------------------------
        '�W���u�}�X�^�ɓo�^
        '------------------------------------------------
        Try

            System.Threading.Thread.Sleep(500)

            If Koufuri_kbn <> 1 Then
                '�ʗ���(���U)
                '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                'If MainLOG.SearchJOBMAST("S010", param, MainDB) = -1 Then
                If MainLOG.SearchJOBMAST("S010", param, SubDB) = -1 Then
                '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                    Message = "�ΏۂƂȂ�W���u���o�^�ς݂ł�"
                    Return False
                End If

                System.Threading.Thread.Sleep(500)

                '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                'If MainLOG.InsertJOBMAST("S010", MainLOG.UserID, param, MainDB) = False Then
                If MainLOG.InsertJOBMAST("S010", MainLOG.UserID, param, SubDB) = False Then
                '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                    Message = "�W���u�}�X�^�̓o�^�Ɏ��s���܂���"
                    Return False
                End If
            Else
                '�ʗ���(���U)

                '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                'If MainLOG.SearchJOBMAST("J010", param, MainDB) = -1 Then
                If MainLOG.SearchJOBMAST("J010", param, SubDB) = -1 Then
                '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                    Message = "�ΏۂƂȂ�W���u���o�^�ς݂ł�"
                    Return False
                End If

                System.Threading.Thread.Sleep(500)

                '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                'If MainLOG.InsertJOBMAST("J010", MainLOG.UserID, param, MainDB) = False Then
                If MainLOG.InsertJOBMAST("J010", MainLOG.UserID, param, SubDB) = False Then
                '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                    Message = "�W���u�}�X�^�̓o�^�Ɏ��s���܂���"
                    Return False
                End If
                MainLOG.Write("f", "")
            End If

        Catch
            Throw
        End Try
        MainLOG.Write("�W���u�o�^", "����", "�p�����[�^�F" & param)
        Return True
    End Function

    Public Function InsertJOBMASTbyError(ByVal Message As String) As Boolean
        Dim SQL As New StringBuilder(128)

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
        Dim DB As CASTCommon.MyOracle = Nothing
        Dim dblock As CASTCommon.CDBLock = New CASTCommon.CDBLock

        Dim LockWaitTime As Integer = 30
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME4")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 30
            End If
        End If

        Try
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

            SQL.Append("INSERT INTO JOBMAST(")
            SQL.Append(" TOUROKU_DATE_J")
            SQL.Append(",TOUROKU_TIME_J")
            SQL.Append(",STA_DATE_J")
            SQL.Append(",STA_TIME_J")
            SQL.Append(",END_DATE_J")
            SQL.Append(",END_TIME_J")
            SQL.Append(",JOBID_J")
            SQL.Append(",STATUS_J")
            SQL.Append(",USERID_J")
            SQL.Append(",PARAMETA_J")
            SQL.Append(",ERRMSG_J")
            SQL.Append(") VALUES (")
            SQL.Append(" TO_CHAR(SYSDATE,'YYYYMMDD')")                          ' TOUROKU_DATE_J
            SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' TOUROKU_TIME_J
            SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' STA_DATE_J
            SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' STA_TIME_J
            SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' END_DATE_J
            SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' END_TIME_J
            SQL.Append(",'W010'")                                               ' JOBID_J
            SQL.Append(",'7'")          ' �ُ�I��                              ' STATUS_J
            SQL.Append("," & SQ(MainLOG.UserID))                                    ' USERID_J
            SQL.Append("," & SQ(Path.GetFileName(RENKEI_FILENAME)))              ' PARAMETA_J
            SQL.Append("," & SQ(Message))                                       ' ERRMSG_J
            SQL.Append(")")

            '*** Str Chg 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
            'Dim DB As New CASTCommon.MyOracle
            DB = New CASTCommon.MyOracle

            ' �W���u�o�^���b�N
            If dblock.InsertJOBMAST_Lock(DB, LockWaitTime) = False Then
                MainLog.Write_Err("�W���u�}�X�^�o�^", "���s", "�^�C���A�E�g")

                Dim ELog As New CASTCommon.ClsEventLOG
                ELog.Write("WEB�`���A�g�@���s�@" & Message, Diagnostics.EventLogEntryType.Error)
                Return False
            End If
            '*** End Chg 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

            If DB.ExecuteNonQuery(SQL) <= 0 Then
                Dim ELog As New CASTCommon.ClsEventLOG
                ELog.Write("WEB�`���A�g�@���s�@" & Message, Diagnostics.EventLogEntryType.Error)
                Return False
            End If

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
            ' �W���u�o�^�A�����b�N
            dblock.InsertJOBMAST_UnLock(DB)
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

            DB.Commit()
            DB.Close()

            Return True

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
        Finally
            If Not DB Is Nothing Then
                ' �W���u�o�^�A�����b�N
                dblock.InsertJOBMAST_UnLock(DB)

                DB.Rollback()
                DB.Close()
            End If
        End Try
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

    End Function

    Public Function InsertWEB_RIREKIMAST(ByVal TORIS_CODE As String, ByVal TORIF_CODE As String, ByVal FURIDATE As String, ByVal ITAKU_KANRI_CODE As String) As Boolean
        Dim SQL As New StringBuilder(128)
        '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
        'Dim OraReader As MyOracleReader = New MyOracleReader(MainDB)
        Dim OraReader As MyOracleReader = New MyOracleReader(SubDB)
        '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
        Dim SEQ As Integer

        If Strings.Right(FileName, 4).ToUpper = ".DAT" Then
            '�t�@�C������"*.DAT*"�̏ꍇ�A��x�A���l�[���������I������Ƃ݂Ȃ��āA�C���T�[�g�����͂��Ȃ�
            Return False
        End If

        'FileArray(0) = ���[�U��
        'FileArray(1) = ���t�@�C����
        'FileArray(2) = �z�M��
        'FileArray(3) = �z�M����
        Dim FileArray() As String = FileName.Split("_"c)

        SQL.Append(" SELECT SEQ_NO_W ")
        SQL.Append(" FROM WEB_RIREKIMAST ")
        SQL.Append(" WHERE TORIS_CODE_W = '" & TORIS_CODE & "'")
        SQL.Append(" AND TORIF_CODE_W = '" & TORIF_CODE & "'")
        SQL.Append(" AND FURI_DATE_W = '" & FURIDATE & "'")
        SQL.Append(" ORDER BY SEQ_NO_W DESC")

        '�����񎝂����݊m�F
        If OraReader.DataReader(SQL) Then
            SEQ = OraReader.GetInt("SEQ_NO_W") + 1
        Else
            SEQ = 1
        End If

        OraReader.Close()

        SQL = New StringBuilder(128)
        SQL.Append("INSERT INTO WEB_RIREKIMAST(")
        SQL.Append(" FSYORI_KBN_W")
        SQL.Append(",TORIS_CODE_W")
        SQL.Append(",TORIF_CODE_W")
        SQL.Append(",FURI_DATE_W")
        SQL.Append(",SEQ_NO_W")
        SQL.Append(",ITAKU_KANRI_CODE_W")
        SQL.Append(",USER_ID_W")
        SQL.Append(",FILE_NAME_W")
        SQL.Append(",SAKUSEI_DATE_W")
        SQL.Append(",SAKUSEI_TIME_W")
        SQL.Append(",STATUS_KBN_W")
        '2012/12/05 saitou WEB�`�� UPD -------------------------------------------------->>>>
        '�����A���z�A�\�����ڒǉ�
        SQL.Append(",END_KEN_W")
        SQL.Append(",END_KIN_W")
        SQL.Append(",YOBI1_W")
        SQL.Append(",YOBI2_W")
        SQL.Append(",YOBI3_W")
        SQL.Append(",YOBI4_W")
        SQL.Append(",YOBI5_W")
        SQL.Append(",YOBI6_W")
        SQL.Append(",YOBI7_W")
        SQL.Append(",YOBI8_W")
        SQL.Append(",YOBI9_W")
        SQL.Append(",YOBI10_W")
        '2012/12/05 saitou WEB�`�� UPD --------------------------------------------------<<<<
        SQL.Append(") VALUES (")
        SQL.Append("'" & Koufuri_kbn & "'")                                 ' FSYORI_KBN_W
        SQL.Append("," & SQ(TORIS_CODE))                                    ' TORIS_CODE_W
        SQL.Append("," & SQ(TORIF_CODE))                                    ' TORIF_CODE_W
        SQL.Append("," & SQ(FURIDATE))                                      ' FURI_DATE_W
        SQL.Append("," & SQ(SEQ))                                           ' SEQ_NO_W
        SQL.Append("," & SQ(ITAKU_KANRI_CODE))                              ' ITAKU_KANRI_CODE_W
        SQL.Append("," & SQ(FileArray(0)))                                  ' USER_ID_W
        SQL.Append("," & SQ(FileArray(1)))                                  ' FILE_NAME_W
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' SAKUSEI_DATE_W
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' SAKUSEI_TIME_W
        SQL.Append("," & SQ("0"))                                           ' STATUS_KBN_W (0:��t��)
        '2012/12/05 saitou WEB�`�� UPD -------------------------------------------------->>>>
        '�����A���z�A�\�����ڒǉ�
        SQL.Append("," & CInt(END_KEN))                                     ' END_KEN_W
        SQL.Append("," & CInt(END_KIN))                                     ' END_KIN_W
        SQL.Append("," & SQ(""))                                            ' YOBI1_W
        SQL.Append("," & SQ(""))                                            ' YOBI2_W
        SQL.Append("," & SQ(""))                                            ' YOBI3_W
        SQL.Append("," & SQ(""))                                            ' YOBI4_W
        SQL.Append("," & SQ(""))                                            ' YOBI5_W
        SQL.Append("," & SQ(""))                                            ' YOBI6_W
        SQL.Append("," & SQ(""))                                            ' YOBI7_W
        SQL.Append("," & SQ(""))                                            ' YOBI8_W
        SQL.Append("," & SQ(""))                                            ' YOBI9_W
        SQL.Append("," & SQ(""))                                            ' YOBI10_W
        '2012/12/05 saitou WEB�`�� UPD --------------------------------------------------<<<<
        SQL.Append(")")
        '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
        'Dim DB As New CASTCommon.MyOracle
        'If DB.ExecuteNonQuery(SQL) <= 0 Then
        '    Dim ELog As New CASTCommon.ClsEventLOG
        '    ELog.Write("WEB�`���A�g�@���s�@" & Message, Diagnostics.EventLogEntryType.Error)
        '    Return False
        'End If
        'DB.Commit()
        'DB.Close()
        If SubDB.ExecuteNonQuery(SQL) <= 0 Then
            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write("WEB�`���A�g�@���s�@" & Message, Diagnostics.EventLogEntryType.Error)
            Return False
        End If
        '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

        Return True
    End Function

    Private Function fn_FileSplit(ByVal FileName As String) As Boolean

        Dim SFileInfo As New clsSplitFileInfo.SplitFilePara

        Dim fs As FileStream = Nothing
        Dim br As BinaryReader = Nothing

        '�ǂݍ��񂾃t�@�C�����R�[�h
        Dim key1 As String = ""

        '�U����
        Dim FuriDate As String = ""
        Dim FuriDate_bk As String = ""

        '�ϑ��҃R�[�h
        Dim ItakuCode As String = ""
        Dim ItakuCode_bk As String = ""

        '�Ǎ��t�@�C���̈ʒu���
        Dim _cnt As Long = 0
        Dim _cntbk As Long = 0

        '���R�[�h�敪(�G���h)
        Dim E_RecKbn As String = ""

        '�����t���O
        Dim flgSplit As Boolean = False

        Dim OraReader As MyOracleReader = New MyOracleReader(MainDB)
        Dim SQL As StringBuilder = New StringBuilder

        Try
            '���ԃt�@�C������ݒ�
            TmpFileName = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV_BK"), DateTime.Now.ToString("yyyyMMddHHmmssfff") & "_" & Path.GetFileName(FileName))
            File.Copy(FileName, TmpFileName, True)

            '*********************************************************************
            '�����R�[�h����
            '*********************************************************************
            Try
                fs = New FileStream(TmpFileName, FileMode.Open, FileAccess.Read)
                br = New BinaryReader(fs)

                '��t�@�C���̏ꍇ
                If fs.Length = 0 Then
                    MainLOG.Write("�t�@�C���f�[�^�Ǎ�", "���s", "��t�@�C���̂��ߏ����o���܂���B�F" & FileName)
                    Message = "��t�@�C���̂��ߏ����o���܂���F" & FileName
                    Return False
                End If

                br.BaseStream.Seek(0, SeekOrigin.Begin)
                br.BaseStream.Position = 0

                '�擪�P�o�C�g��ǂݍ��ݕ����R�[�h�𔻒肷��
                Select Case br.ReadByte()
                    Case 49
                        '�����R�[�h��JIS�ɐݒ�
                        MOJICode = Encoding.GetEncoding("SHIFT-JIS")

                    Case 241
                        '�����R�[�h��EBCDIC�ɐݒ�
                        MOJICode = Encoding.GetEncoding("IBM290")
                    Case Else
                        MainLOG.Write("�`���t�@�C���R�[�h����", "���s", "�擪�����R�[�h�ُ�F" & FileName)
                        
                        Message = "�擪�����R�[�h�ُ�F" & FileName
                        Return False
                End Select

            Catch ex As Exception
                MainLOG.Write("�`���t�@�C���R�[�h����", "���s", "�t�@�C�����F" & FileName & "�@" & ex.Message)
                
                Message = "�t�@�C�����F" & FileName & "�@" & ex.Message
                Return False
            End Try

            '*********************************************************************
            '�t�@�C������
            '*********************************************************************
            Dim num As Integer = 1
            Dim _FileLen As Long = fs.Length
            Dim TwoByte(1) As Byte

            _cnt = 0

            br.BaseStream.Seek(0, SeekOrigin.Begin)
            br.BaseStream.Position = 0

            If _FileLen Mod 120 = 0 Then
                br.BaseStream.Seek(120, SeekOrigin.Begin)
                TwoByte = br.ReadBytes(2)
                Select Case TwoByte(0)
                    Case &HD    '0D
                        Select Case TwoByte(1)
                            Case &HA, &H25  '0A,25
                                File_Byte = 122
                            Case Else
                                File_Byte = 121
                        End Select
                    Case &HA    '0A
                        File_Byte = 121
                    Case &HF1   'F1
                        File_Byte = 121
                    Case Else
                        File_Byte = 120
                End Select
            Else
                br.BaseStream.Seek(120, SeekOrigin.Begin)
                TwoByte = br.ReadBytes(2)
                Select Case TwoByte(0)
                    Case &HA    '0A
                        File_Byte = 121
                    Case Else
                        File_Byte = 122
                End Select
            End If

            br.BaseStream.Seek(0, SeekOrigin.Begin)
            br.BaseStream.Position = 0

            Do
                '=================================================================================
                '�t�@�C�������������菈��
                '=================================================================================

                'Dim bt_RecordData(119) As Byte
                Dim bt_RecordData(121) As Byte

                '�t�@�C�������łȂ���Ε�������������s��
                If _cnt = _FileLen Then
                    flgSplit = True
                    '_cnt += 120
                    _cnt += File_Byte
                Else
                    '�U�������擾
                    If _cnt = 0 Then
                        '1�s�ڃw�b�_�[���擾
                        br.BaseStream.Seek(0, SeekOrigin.Begin)
                        'bt_RecordData = br.ReadBytes(120)
                        bt_RecordData = br.ReadBytes(File_Byte)

                        If SFileInfo.fn_GetFirstRecord(bt_RecordData, MOJICode) = True Then
                            '�ϑ��҃R�[�h�擾
                            ItakuCode_bk = SFileInfo.ZG04
                            '�U�����擾
                            FuriDate_bk = SFileInfo.ZG06
                        End If
                    End If

                    '�t�@�C����120�o�C�g�Âǂݍ���
                    br.BaseStream.Seek(_cnt, SeekOrigin.Begin)
                    'bt_RecordData = br.ReadBytes(120)
                    bt_RecordData = br.ReadBytes(File_Byte)

                    '�w�b�_�[���R�[�h���ǂ�������
                    If SFileInfo.fn_GetFirstRecord(bt_RecordData, MOJICode) = True Then

                        '�ϑ��҃R�[�h�擾
                        ItakuCode = SFileInfo.ZG04
                        '�U�������擾
                        FuriDate = SFileInfo.ZG06

                        '�w�b�_���`�F�b�N

                        '*** Str Del 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                        'OraReader = New CASTCommon.MyOracleReader
                        '*** End Del 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                        SQL = New StringBuilder(128)

                        SQL.Append(" SELECT MULTI_KBN_T ")
                        SQL.Append(" FROM TORIMAST ")
                        SQL.Append(" WHERE ITAKU_CODE_T = '" & ItakuCode & "'")
                        SQL.Append(" AND   SYUBETU_T = '" & SFileInfo.ZG02 & "'")

                        If OraReader.DataReader(SQL) Then
                            Multi_Kbn = OraReader.GetString("MULTI_KBN_T")
                        Else
                            '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j�� ���d���s�Ή��iDB�R�l�N�V�����̈�{���j***
                            'OraReader = New CASTCommon.MyOracleReader
                            OraReader.Close()
                            OraReader = New CASTCommon.MyOracleReader(MainDB)
                            '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j�� ���d���s�Ή��iDB�R�l�N�V�����̈�{���j***
                            SQL = New StringBuilder(128)

                            SQL.Append(" SELECT MULTI_KBN_T")
                            SQL.Append(" FROM S_TORIMAST")
                            SQL.Append(" WHERE ITAKU_CODE_T = '" & ItakuCode & "'")
                            SQL.Append(" AND SYUBETU_T = '" & SFileInfo.ZG02 & "'")

                            If OraReader.DataReader(SQL) Then
                                Multi_Kbn = OraReader.GetString("MULTI_KBN_T")
                            End If
                        End If

                        OraReader.Close()

                        '�O���R�[�h��ǂݍ���
                        'If _cnt >= 120 Then
                        If _cnt >= File_Byte Then
                            'br.BaseStream.Seek(_cnt - 120, SeekOrigin.Begin)
                            br.BaseStream.Seek(_cnt - File_Byte, SeekOrigin.Begin)
                            E_RecKbn = MOJICode.GetString(br.ReadBytes(1))
                        End If

                        '�t�@�C�����������ݒ�
                        '�ȉ���3�����ɓ��Ă͂܂�ꍇ�͕����ΏۂƂ���
                        '=================================================================
                        '1�O���R�[�h���G���h���R�[�h�܂��̓g���[�����R�[�h
                        '2�U�����s��v
                        '3�ϑ��҃R�[�h��v���A�U������v
                        '=================================================================

                        If E_RecKbn <> Nothing And E_RecKbn <> "" Then
                            '����1:�O���R�[�h���G���h���R�[�h
                            If E_RecKbn.Substring(0, 1) = "9" Or E_RecKbn.Substring(0, 1) = "8" Then
                                'If E_RecKbn.Substring(0, 1) = "9" Then
                                flgSplit = True
                            End If
                        End If

                        If FuriDate <> FuriDate_bk Then
                            '����2:�U�����s��v
                            flgSplit = True
                        End If

                        If ItakuCode = ItakuCode_bk AndAlso FuriDate = FuriDate_bk Then
                            '����3:�ϑ��҃R�[�h��v���A�U������v
                            If _cnt <> 0 Then
                                flgSplit = True
                            End If
                        End If

                        '�ϑ��҃R�[�h��ޔ�
                        ItakuCode_bk = ItakuCode
                        '�U��������ޔ�
                        FuriDate_bk = FuriDate

                        '������񂪃}���`�t�@�C���̏ꍇ�A�������Ȃ�
                        If Multi_Kbn <> "0" Then
                            flgSplit = False
                        End If

                    End If
                End If

                '=================================================================================
                '�t�@�C����������
                '=================================================================================
                '�����t���O��true�Ȃ番������
                If flgSplit = True Then

                    '�����t�@�C�����t�H���_�ɏ�����
                    Dim _FileName As String = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), Path.GetFileName(FileName) & "." & num.ToString("000"))
                    For i As Integer = num To 999
                        If File.Exists(_FileName) = False Then
                            Exit For
                        End If
                        '�t�@�C�������݂���ꍇ�̓J�E���g�A�b�v����
                        num += 1
                        _FileName = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), Path.GetFileName(FileName) & "." & num.ToString("000"))
                    Next
                   
                    num += 1

                    If _cnt <> 0 Then
                        Try
                            '�����t�@�C��������
                            br.BaseStream.Seek(_cntbk, SeekOrigin.Begin)

                            Dim sw As New StreamWriter(_FileName, True)
                            Dim bw As New BinaryWriter(sw.BaseStream)
                            bw.Write(br.ReadBytes(CInt(_cnt - _cntbk)))
                            sw.Close()
                            bw.Close()

                            '�����t���O��false�ɖ߂�
                            flgSplit = False

                        Catch ex As Exception
                            'MainLOG.Write("�����t�@�C������", "���s", "�t�@�C�����F" & _FileName & "�@" & ex.Message)
                            Message = "�t�@�C�����F" & _FileName & "�@" & ex.Message
                            Return False
                        End Try

                        '�����t�@�C������z��ɒǉ�
                        S_DATFileArray.Add(_FileName)

                        _cntbk = _cnt
                    End If
                End If

                '_cnt += 120
                _cnt += File_Byte

            Loop Until _cnt > _FileLen

            Return True

        Catch ex As Exception
            MainLOG.Write("�����t�@�C������", "���s", "�t�@�C�����F" & FileName & "�@" & ex.Message)
            
            Message = "�t�@�C�����F" & FileName & "�@" & ex.Message
            Return False
        Finally
            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

            If Not fs Is Nothing Then fs.Close()
            If Not br Is Nothing Then br.Close()
        End Try

    End Function

    ' 2015/12/28 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
    Private Function GetIniInfo() As Boolean

        Try
            MainLOG.Write(MainLOG.UserID, MainLOG.ToriCode, MainLOG.FuriDate, "�ݒ�t�@�C���擾", "")

            '==================================================================
            '�@RSV2�@�\�ݒ�
            '==================================================================
            Ini_Info.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            If Ini_Info.RSV2_EDITION.ToUpper = "ERR" OrElse Ini_Info.RSV2_EDITION = Nothing Then
                MainLOG.Write(MainLOG.UserID, MainLOG.ToriCode, MainLOG.FuriDate, "�ݒ�t�@�C���擾", "���s", "���ږ�:RSV2�@�\�ݒ� ����:RSV2_V1.0.0 ����:EDITION")
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(MainLOG.ToriCode, MainLOG.FuriDate, "�ݒ�t�@�C���擾","���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(MainLOG.UserID, MainLOG.ToriCode, MainLOG.FuriDate, "�ݒ�t�@�C���擾", "")
        End Try

    End Function
    ' 2015/12/28 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

    Public Class clsSplitFileInfo
        '�����t�@�C�����i�[�p�\����
        Structure SplitFilePara

            '***�w�b�_���R�[�h���***
            <VBFixedString(1)> Public ZG01 As String    '�f�[�^�敪(=1)
            <VBFixedString(2)> Public ZG02 As String    '��ʃR�[�h
            <VBFixedString(1)> Public ZG03 As String    '�R�[�h�敪
            <VBFixedString(10)> Public ZG04 As String   '�U���˗��l�R�[�h
            <VBFixedString(40)> Public ZG05 As String   '�U���˗��l��
            <VBFixedString(4)> Public ZG06 As String    '�戵��
            <VBFixedString(4)> Public ZG07 As String    '�d����s����
            <VBFixedString(15)> Public ZG08 As String   '�d����s��
            <VBFixedString(3)> Public ZG09 As String    '�d���x�X����
            <VBFixedString(15)> Public ZG10 As String   '�d���x�X��
            <VBFixedString(1)> Public ZG11 As String    '�a�����
            <VBFixedString(7)> Public ZG12 As String    '�����ԍ�
            <VBFixedString(17)> Public ZG13 As String   '�_�~�[

            Public Sub init()
                ZG01 = ""
                ZG02 = ""
                ZG03 = ""
                ZG04 = ""
                ZG05 = ""
                ZG06 = ""
                ZG07 = ""
                ZG08 = ""
                ZG09 = ""
                ZG10 = ""
                ZG11 = ""
                ZG12 = ""
                ZG13 = ""
            End Sub

            Public Function fn_GetFirstRecord(ByVal recordbyte() As Byte, ByVal code As Encoding) As Boolean

                Try
                    '�ǂݍ��񂾕�����120�o�C�g�ȉ�
                    If recordbyte.Length < 120 Then
                        Return False
                    End If

                    '�f�[�^�Z�b�g
                    ZG01 = code.GetString(recordbyte, 0, 1) '�f�[�^�敪(=1)
                    ZG02 = code.GetString(recordbyte, 1, 2) '��ʃR�[�h
                    ZG03 = code.GetString(recordbyte, 3, 1)    '�R�[�h�敪
                    ZG04 = code.GetString(recordbyte, 4, 10)   '�U���˗��l�R�[�h
                    ZG05 = code.GetString(recordbyte, 14, 40)  '�U���˗��l��
                    ZG06 = code.GetString(recordbyte, 54, 4)   '�戵��
                    ZG07 = code.GetString(recordbyte, 58, 4)   '�d����s����
                    ZG08 = code.GetString(recordbyte, 62, 15) '�d����s��
                    ZG09 = code.GetString(recordbyte, 77, 3)  '�d���x�X����
                    ZG10 = code.GetString(recordbyte, 80, 15) '�d���x�X��
                    ZG11 = code.GetString(recordbyte, 95, 1)  '�a�����
                    ZG12 = code.GetString(recordbyte, 96, 7)  '�����ԍ�
                    ZG13 = code.GetString(recordbyte, 103, 17) '�_�~�[

                    '�f�[�^�敪(=1)
                    If ZG01 <> "1" Then
                        Return False
                    End If

                    Return True

                Catch
                    Throw
                End Try

            End Function

        End Structure
        Public SplitFile_Para As SplitFilePara = Nothing

    End Class

    Private Function fn_AddEndRecord(ByVal DatFilePath As String) As Boolean
        Try
            Dim fs As FileStream = Nothing
            Dim br As BinaryReader = Nothing
            'Dim bt(119) As Byte
            Dim bt(121) As Byte
            Dim EndRecord As String

            '�����t�@�C�����݃`�F�b�N
            If Not File.Exists(DatFilePath) Then
                Return False
            End If

            Try
                '�J�n���R�[�h�ƍŏI���R�[�h�Ǎ�
                fs = New FileStream(DatFilePath, FileMode.Open, FileAccess.ReadWrite)
                br = New BinaryReader(fs, MOJICode)
                '�J�n���R�[�h�Ǎ�
                'bt = br.ReadBytes(120)
                bt = br.ReadBytes(File_Byte)
                '�ŏI���R�[�h�Ǎ�
                'fs.Seek(-120, SeekOrigin.End)
                fs.Seek(-File_Byte, SeekOrigin.End)
                EndRecord = MOJICode.GetString(br.ReadBytes(1))

                '�ŏI���R�[�h���g���[���̂Ƃ��G���h���R�[�h�ǉ�
                If EndRecord = "8" Then
                    'Dim b() As Byte = MOJICode.GetBytes("9".PadRight(120))
                    Dim b() As Byte = MOJICode.GetBytes("9".PadRight(File_Byte))
                    fs.Seek(0, SeekOrigin.End)
                    fs.Write(b, 0, 120)
                    'fs.Write(b, 0, File_Byte)
                End If

            Catch
                Throw
            Finally
                If Not fs Is Nothing Then fs.Close()
                If Not br Is Nothing Then br.Close()
            End Try

        Catch ex As Exception
            MainLOG.Write("�G���h���R�[�h�ǉ�", "���s", "�t�@�C�����F" & FileName & "�@" & ex.Message)
            Message = "�G���h���R�[�h�ǉ����s�F" & FileName & "�@" & ex.Message
            Return False
        End Try

        Return True
    End Function

End Class
