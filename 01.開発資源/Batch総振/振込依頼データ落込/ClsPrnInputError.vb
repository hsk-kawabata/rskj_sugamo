Imports System
Imports System.IO

' �C���v�b�g�G���[
Class ClsPrnInputError
    Inherits CAstReports.ClsReportBase

    Private TextData As New CAstFormat.ClsText

    Private ToriSCode As String         ' ������R�[�h
    Private ToriFCode As String         ' ����敛�R�[�h
    Private FuriDate As String          ' �U����

    '2017/02/27 �^�X�N�j���� ADD �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- START
    Private STR_EDITION As String = ""
    '2017/02/27 �^�X�N�j���� ADD �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- END

    '2017/02/27 �^�X�N�j���� CHG �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- START
    Sub New(ByVal EDITION As String)

        STR_EDITION = EDITION

        If EDITION = "2" Then
            '��K�͔ł̒��[

            ' CSV�t�@�C���Z�b�g
            InfoReport.ReportName = "KFSP035"

            ' ��`�̖��Z�b�g
            ReportBaseName = "KFSP035_�C���v�b�g�G���[���X�g.rpd"
        Else
            ' CSV�t�@�C���Z�b�g
            InfoReport.ReportName = "KFSP003"

            ' ��`�̖��Z�b�g
            ReportBaseName = "KFSP003_�C���v�b�g�G���[���X�g.rpd"
        End If
    End Sub
    'Sub New()
    '    ' CSV�t�@�C���Z�b�g
    '    InfoReport.ReportName = "KFSP003"

    '    ' ��`�̖��Z�b�g
    '    ReportBaseName = "KFSP003_�C���v�b�g�G���[���X�g.rpd"
    'End Sub
    '2017/02/27 �^�X�N�j���� CHG �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- END

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()
        Return file
    End Function

    '2017/02/27 �^�X�N�j���� ADD �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- START
    Public Sub PutMem(ByVal value As String, Optional ByVal crlf As Boolean = False)
        Call MEMObject.Output(value, True, crlf)
    End Sub

    Public Sub PutCsv(ByVal value As String, Optional ByVal crlf As Boolean = False)
        Call CSVObject.Output(value, False, crlf)
    End Sub
    '2017/02/27 �^�X�N�j���� ADD �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- END

    '2017/02/27 �^�X�N�j���� CHG �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- START
    '�������X�g���[�������o���ɕύX�i��K�͔ł̏����ɍ��킹��j
    Public Sub OutputMemData(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
        Dim InfoPara As CAstBatch.CommData.stPARAMETER = aToriComm.INFOParameter    '�p�����[�^���
        Dim InfoTori As CAstBatch.CommData.stTORIMAST = aToriComm.INFOToriMast      '�������
        Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast          '�˗����׏��
        Dim InfoMei2 As CAstFormat.CFormat.MEISAI2 = aReadFmt.InfoMeisaiMast2       '�˗����׏��2(���Z�@�֖��A�X�ܖ�)

        Try
            For i As Integer = 0 To aReadFmt.InErrorArray.Count - 1
                Dim inf As CAstFormat.CFormat.INPUTERROR
                inf = CType(aReadFmt.InErrorArray(i), CAstFormat.CFormat.INPUTERROR)

                '---------------------------------------
                '�w�b�_���
                '---------------------------------------
                PutMem(InfoPara.FURI_DATE)                            '�U����
                PutMem(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '������
                PutMem(CASTCommon.Calendar.Now.ToString("HHmmss"))    '�^�C���X�^���v
                PutMem(InfoTori.TORIS_CODE_T)                         '������R�[�h 
                PutMem(InfoTori.TORIF_CODE_T)                         '����敛�R�[�h
                PutMem(InfoTori.ITAKU_NNAME_T)                  '����於
                PutMem(InfoTori.TKIN_NO_T)                            '������Z�@�փR�[�h
                PutMem(InfoTori.TKIN_NNAME_N)                   '������Z�@�֖�
                PutMem(InfoTori.TSIT_NO_T)                            '����x�X�R�[�h
                PutMem(InfoTori.TSIT_NNAME_N)                   '����x�X��

                Select Case InfoTori.SYUBETU_T                                  '���
                    Case "21"
                        PutMem("�����U��")
                    Case "11"
                        PutMem("���^�U��")
                    Case "12"
                        PutMem("�ܗ^�U��")
                    Case "21"
                        PutMem("�����U��")
                    Case Else
                        PutMem("�����U��")
                End Select

                Select Case InfoTori.CODE_KBN_T                                 '�R�[�h�敪
                    Case "0"
                        PutMem("JIS")
                    Case "1"
                        PutMem("JIS���L(120)")
                    Case "2"
                        PutMem("JIS���L(119)")
                    Case "3"
                        PutMem("JIS���L(118)")
                    Case "4"
                        PutMem("EBCDIC")
                End Select

                '2017/02/27 �^�X�N�j���� ADD �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- START
                If STR_EDITION = "2" Then
                    '---------------------------------------
                    '�˗��t�@�C���@�w�b�_�[���
                    '---------------------------------------
                    PutMem(TextData.GetBaitaiCode(InfoTori.BAITAI_CODE_T))  ' �}�̃R�[�h
                    PutMem(InfoMei.SYUBETU_CODE)                            ' ������ʃR�[�h
                    Select Case InfoMei.SYUBETU_CODE                        ' ������ʖ�
                        Case "11"
                            PutMem("���^�U��(����)")
                        Case "12"
                            PutMem("�ܗ^�U��(����)")
                        Case "71"
                            PutMem("���^�U��(������)")
                        Case "72"
                            PutMem("�ܗ^�U��(������)")
                        Case "21"
                            PutMem("�����U��")
                        Case "41"
                            PutMem("�����z�����U��")
                        Case "43"
                            PutMem("�N���M���_��ꎞ�����t�U��")
                        Case "44"
                            PutMem("���I�N���ꎞ�����t���U��")
                        Case "45"
                            PutMem("��Õی��̋��t�U��")
                        Case "91"
                            PutMem("�a�������U��")
                        Case Else
                            PutMem("")
                    End Select
                    PutMem(InfoMei.ITAKU_CODE)                              ' ��ЃR�[�h
                    PutMem(InfoMei.ITAKU_KNAME)                             ' �˗��l��
                    PutMem(InfoMei.ITAKU_SIT)                               ' ����X�܃R�[�h
                    PutMem(InfoMei.I_SIT_NNAME)                             ' ����X�ܖ�
                    PutMem(CASTCommon.ConvertKamoku2TO1_K(InfoMei.ITAKU_KAMOKU))    ' �˗��l�a�����
                    PutMem(InfoMei.ITAKU_KOUZA)                             ' �˗��l�����ԍ�
                End If
                '2017/02/27 �^�X�N�j���� ADD �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- END

                '---------------------------------------
                '�C���v�b�g�G���[���
                '---------------------------------------
                PutMem(CStr(i + 1))                                   '�ʔ�
                PutMem(inf.KIN)                                       '�U�����Z�@�փR�[�h
                '2018/10/07 saitou �L���M��(RSV2�W��) UPD �i�w�b�_�������`�F�b�N�Ή��j ------------------ START
                '�w�b�_�����o�͂���Ƃ��͐U�����Z�@�֖����Q�Ƃ��Ȃ��B
                If InfoMei2.KEIYAKU_KIN_KNAME Is Nothing Then
                    PutMem("")
                Else
                    PutMem(InfoMei2.KEIYAKU_KIN_KNAME.Trim)           '�U�����Z�@�֖�
                End If
                'PutMem(InfoMei2.KEIYAKU_KIN_KNAME.Trim)         '�U�����Z�@�֖�
                '2018/10/07 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------- END
                PutMem(inf.SIT)                                       '�U���x�X�R�[�h
                '2018/10/07 saitou �L���M��(RSV2�W��) UPD �i�w�b�_�������`�F�b�N�Ή��j ------------------ START
                '�w�b�_�����o�͂���Ƃ��͐U���x�X�����Q�Ƃ��Ȃ��B
                If InfoMei2.KEIYAKU_SIT_KNAME Is Nothing Then
                    PutMem("")
                Else
                    PutMem(InfoMei2.KEIYAKU_SIT_KNAME.Trim)           '�U���x�X��
                End If
                'PutMem(InfoMei2.KEIYAKU_SIT_KNAME.Trim)         '�U���x�X��
                '2018/10/07 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------- END
                PutMem(inf.KAMOKU)                                    '�Ȗ�
                PutMem(inf.KOUZA)                                     '�����ԍ�
                PutMem(inf.JYUYOKA_NO)                          '���v�Ɣԍ�
                PutMem(inf.KNAME)                               '�_��Җ�
                PutMem(inf.FURIKIN)                                   '�U�����z
                PutMem(inf.ERRINFO, True)                       '�G���[���b�Z�[�W
            Next i

        Catch ex As Exception
            BatchLog.Write("(�C���v�b�g�G���[���)", "���s", ex.Message)
        End Try
    End Sub
    ''==================================================================
    '' �@�\�@ �F �C���v�b�g�G���[���o��
    '' �����@ �F aReadFmt - �t�H�[�}�b�g�N���X / aToriComm - �o�b�`�����N���X
    '' ���l�@ �F 
    '' �쐬�� �F2009/09/16
    '' �X�V�� �F
    ''==================================================================
    'Public Sub OutputData(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
    '    Dim InfoPara As CAstBatch.CommData.stPARAMETER = aToriComm.INFOParameter    '�p�����[�^���
    '    Dim InfoTori As CAstBatch.CommData.stTORIMAST = aToriComm.INFOToriMast      '�������
    '    Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast          '�˗����׏��
    '    Dim InfoMei2 As CAstFormat.CFormat.MEISAI2 = aReadFmt.InfoMeisaiMast2       '�˗����׏��2(���Z�@�֖��A�X�ܖ�)

    '    Try
    '        For i As Integer = 0 To aReadFmt.InErrorArray.Count - 1
    '            Dim inf As CAstFormat.CFormat.INPUTERROR
    '            inf = CType(aReadFmt.InErrorArray(i), CAstFormat.CFormat.INPUTERROR)

    '            '---------------------------------------
    '            '�w�b�_���
    '            '---------------------------------------
    '            CSVObject.Output(InfoPara.FURI_DATE)                            '�U����
    '            CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '������
    '            CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    '�^�C���X�^���v
    '            CSVObject.Output(InfoTori.TORIS_CODE_T)                         '������R�[�h 
    '            CSVObject.Output(InfoTori.TORIF_CODE_T)                         '����敛�R�[�h
    '            CSVObject.Output(InfoTori.ITAKU_NNAME_T, True)                  '����於
    '            CSVObject.Output(InfoTori.TKIN_NO_T)                            '������Z�@�փR�[�h
    '            CSVObject.Output(InfoTori.TKIN_NNAME_N, True)                   '������Z�@�֖�
    '            CSVObject.Output(InfoTori.TSIT_NO_T)                            '����x�X�R�[�h
    '            CSVObject.Output(InfoTori.TSIT_NNAME_N, True)                   '����x�X��

    '            Select Case InfoTori.SYUBETU_T                                  '���
    '                Case "21"
    '                    CSVObject.Output("�����U��")
    '                Case "11"
    '                    CSVObject.Output("���^�U��")
    '                Case "12"
    '                    CSVObject.Output("�ܗ^�U��")
    '                Case "21"
    '                    CSVObject.Output("�����U��")
    '                Case Else
    '                    CSVObject.Output("�����U��")
    '            End Select

    '            Select Case InfoTori.CODE_KBN_T                                 '�R�[�h�敪
    '                Case "0"
    '                    CSVObject.Output("JIS")
    '                Case "1"
    '                    CSVObject.Output("JIS���L(120)")
    '                Case "2"
    '                    CSVObject.Output("JIS���L(119)")
    '                Case "3"
    '                    CSVObject.Output("JIS���L(118)")
    '                Case "4"
    '                    CSVObject.Output("EBCDIC")
    '            End Select

    '            '---------------------------------------
    '            '�C���v�b�g�G���[���
    '            '---------------------------------------
    '            CSVObject.Output(CStr(i + 1))                                   '�ʔ�
    '            CSVObject.Output(inf.KIN)                                       '�U�����Z�@�փR�[�h
    '            CSVObject.Output(InfoMei2.KEIYAKU_KIN_KNAME.Trim, True)         '�U�����Z�@�֖�
    '            CSVObject.Output(inf.SIT)                                       '�U���x�X�R�[�h
    '            CSVObject.Output(InfoMei2.KEIYAKU_SIT_KNAME.Trim, True)         '�U���x�X��
    '            CSVObject.Output(inf.KAMOKU)                                    '�Ȗ�
    '            CSVObject.Output(inf.KOUZA)                                     '�����ԍ�
    '            CSVObject.Output(inf.JYUYOKA_NO, True)                          '���v�Ɣԍ�
    '            CSVObject.Output(inf.KNAME, True)                               '�_��Җ�
    '            CSVObject.Output(inf.FURIKIN)                                   '�U�����z
    '            CSVObject.Output(inf.ERRINFO, True, True)                       '�G���[���b�Z�[�W
    '        Next i

    '    Catch ex As Exception
    '        BatchLog.Write("(�C���v�b�g�G���[���)", "���s", ex.Message)
    '    End Try
    'End Sub
    '2017/02/27 �^�X�N�j���� CHG �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- END

    '==================================================================
    ' �@�\�@ �F ������s
    ' ���l�@ �F 
    ' �쐬�� �F2009/09/16
    ' �X�V�� �F
    '==================================================================
    Public Overloads Overrides Function ReportExecute() As Boolean
        '���[�o�͏���
        Return MyBase.ReportExecute()
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        TextData = Nothing
    End Sub

    '2017/02/27 �^�X�N�j���� ADD �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- START
    ''
    ' �@�\�@ �F �C���v�b�g�G���[���Ƀg���[������t������
    '
    ' ���l�@ �F 
    '
    Public Sub OutputCsvData(ByVal aReadFmt As CAstFormat.CFormat)
        ' �˗����׏��
        Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast
        Dim Memory As CAstReports.MEM = MEMObject

        ' �擪�Ɉʒu�Â�
        Memory.Seek(0)
        Dim LineData As String = Memory.ReadLine

        If Not LineData Is Nothing Then
            Tenmei = aReadFmt.ToriData.INFOToriMast.TSIT_NNAME_N
            Itakusyamei = aReadFmt.ToriData.INFOToriMast.ITAKU_NNAME_T
            Hiduke = aReadFmt.ToriData.INFOParameter.FURI_DATE
            CreateCsvFile()
        End If

        Do Until LineData Is Nothing
            If STR_EDITION = "2" Then
                '��K�͔�
                PutCsv(LineData)
                PutCsv(InfoMei.TOTAL_IRAI_KEN.ToString)    '���v����
                PutCsv(InfoMei.TOTAL_IRAI_KIN.ToString)    '���v���z
                PutCsv(InfoMei.TOTAL_ZUMI_KEN.ToString)    '�U�֍ό���
                PutCsv(InfoMei.TOTAL_ZUMI_KIN.ToString)    '�U�֍ϋ��z
                PutCsv(InfoMei.TOTAL_FUNO_KEN.ToString)    '�U�֕s�\����
                PutCsv(InfoMei.TOTAL_FUNO_KIN.ToString)    '�U�֕s�\���z
                PutCsv("", True)
            Else
                '�W����
                PutCsv(LineData, True)

            End If

            LineData = Memory.ReadLine()
        Loop
    End Sub
    '2017/02/27 �^�X�N�j���� ADD �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- END

End Class
