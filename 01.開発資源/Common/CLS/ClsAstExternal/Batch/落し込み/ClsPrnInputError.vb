Imports System
Imports System.IO
Imports Microsoft.VisualBasic.Strings

' �C���v�b�g�G���[
Class ClsPrnInputError
    Inherits CAstReports.ClsReportBase

    Private TextData As New CAstFormat.ClsText

    Private ToriSCode As String         ' ������R�[�h
    Private ToriFCode As String         ' ����敛�R�[�h
    Private FuriDate As String          ' �U�֓�

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP004"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP004_�C���v�b�g�G���[���X�g.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()
        ''2008/11/07 T-Sakai ************************************
        ''FTP���M�ɕύX
        '' �^�C�g���s
        'CSVObject.Output("������")
        'CSVObject.Output("�^�C���X�^���v")
        'CSVObject.Output("������R�[�h")
        'CSVObject.Output("����敛�R�[�h")
        'CSVObject.Output("����於")
        'CSVObject.Output("�ϑ��҃R�[�h")
        'CSVObject.Output("�U�֓�")
        'CSVObject.Output("���܂ƂߓX")
        'CSVObject.Output("�}�̃R�[�h")
        'CSVObject.Output("������ʃR�[�h")
        'CSVObject.Output("��ʖ�")
        'CSVObject.Output("�R�[�h�敪")
        'CSVObject.Output("��ЃR�[�h")
        'CSVObject.Output("�˗��l��")
        'CSVObject.Output("������Z�@�փR�[�h")
        'CSVObject.Output("������Z�@�֖�")
        'CSVObject.Output("����X�܃R�[�h")
        'CSVObject.Output("����X�ܖ�")
        'CSVObject.Output("�˗��l�a�����")
        'CSVObject.Output("�˗��l�����ԍ�")
        'CSVObject.Output("���R�[�h�ԍ�")
        ''***�C�� maeda 2008/05/12*********************************************************
        ''�C���v�b�g�G���[�o�͍��ڂɋ��Z�@�֖��A�X�ܖ���ǉ�
        'CSVObject.Output("�������Z�@�փR�[�h")
        'CSVObject.Output("�������Z�@�֖�")
        'CSVObject.Output("�����X��")
        'CSVObject.Output("�����X�ܖ�")
        ''CSVObject.Output("�������Z�@�փR�[�h")
        ''CSVObject.Output("�����X��")
        ''*********************************************************************************
        'CSVObject.Output("�Ȗ�")
        'CSVObject.Output("���������ԍ�")
        'CSVObject.Output("�_��Ҕԍ�")
        'CSVObject.Output("�_��Җ�")
        'CSVObject.Output("�U�֋��z")
        'CSVObject.Output("�G���[���")
        'CSVObject.Output("���v����")
        'CSVObject.Output("���v���z")
        'CSVObject.Output("�U�֍ό���")
        'CSVObject.Output("�U�֍ϋ��z")
        'CSVObject.Output("�U�֕s�\����")
        'CSVObject.Output("�U�֕s�\���z", False, True)
        '**********************************************************************************
        Return file
    End Function

    'Public Sub PutMem(ByVal value As String, Optional ByVal crlf As Boolean = False)
    '    Call MEMObject.Output(value, True, crlf)
    'End Sub

    'Public Sub PutCsv(ByVal value As String, Optional ByVal crlf As Boolean = False)
    '    Call CSVObject.Output(value, False, crlf)
    'End Sub

    '==================================================================
    ' �@�\�@ �F �C���v�b�g�G���[���o��
    ' �����@ �F aReadFmt - �t�H�[�}�b�g�N���X / aToriComm - �o�b�`�����N���X
    ' ���l�@ �F 
    ' �쐬�� �F2009/09/16
    ' �X�V�� �F
    '==================================================================
    'Public Sub OutputMemData(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
    Public Sub OutputData(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
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
                CSVObject.Output(InfoPara.FURI_DATE)                            '�U�֓�
                CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '������
                CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    '�^�C���X�^���v
                CSVObject.Output(InfoTori.TORIS_CODE_T)                         '������R�[�h 
                CSVObject.Output(InfoTori.TORIF_CODE_T)                         '����敛�R�[�h
                CSVObject.Output(InfoTori.ITAKU_NNAME_T, True)                  '����於
                CSVObject.Output(InfoTori.TKIN_NO_T)                            '������Z�@�փR�[�h
                CSVObject.Output(InfoTori.TKIN_NNAME_N, True)                   '������Z�@�֖�
                CSVObject.Output(InfoTori.TSIT_NO_T)                            '����x�X�R�[�h
                CSVObject.Output(InfoTori.TSIT_NNAME_N, True)                   '����x�X��
                CSVObject.Output(InfoTori.FURI_CODE_T)                          '�U�փR�[�h
                CSVObject.Output(InfoTori.KIGYO_CODE_T)                         '��ƃR�[�h

                Select Case InfoTori.CODE_KBN_T                                 '�R�[�h�敪
                    Case "0"
                        CSVObject.Output("JIS")
                    Case "1"
                        CSVObject.Output("JIS���L(120)")
                    Case "2"
                        CSVObject.Output("JIS���L(119)")
                    Case "3"
                        CSVObject.Output("JIS���L(118)")
                    Case "4"
                        CSVObject.Output("EBCDIC")
                End Select

                '---------------------------------------
                '�C���v�b�g�G���[���
                '---------------------------------------
                CSVObject.Output(CStr(i + 1))                                   '�ʔ�
                CSVObject.Output(inf.KIN)                                       '�������Z�@�փR�[�h
                CSVObject.Output(InfoMei2.KEIYAKU_KIN_KNAME.Trim, True)         '�������Z�@�֖�
                CSVObject.Output(inf.SIT)                                       '�����x�X�R�[�h
                CSVObject.Output(InfoMei2.KEIYAKU_SIT_KNAME.Trim, True)         '�����x�X��
                CSVObject.Output(inf.KAMOKU)                                    '�Ȗ�
                CSVObject.Output(inf.KOUZA)                                     '�����ԍ�

                CSVObject.Output(inf.JYUYOKA_NO, True)                          '���v�Ɣԍ�
                'If inf.JYUYOKA_NO.Length > 10 Then                              '�_��Ҕԍ�
                '    CSVObject.Output(Left(inf.JYUYOKA_NO, 10))
                'Else
                '    CSVObject.Output(inf.JYUYOKA_NO)
                'End If

                CSVObject.Output(inf.KNAME, True)                               '�_��Җ�
                CSVObject.Output(inf.FURIKIN)                                   '�U�֋��z
                CSVObject.Output(inf.ERRINFO, True, True)                       '�G���[���b�Z�[�W
            Next i
            '2018/04/13 FJH) ���� �~�M���@FAX��{�萔���_���`�F�b�N�����ǉ� --------------------------------------------- START
            If aReadFmt.InfoMeisaiMast.FURIKETU_CODE = 6 Then

                '---------------------------------------
                '�w�b�_���
                '---------------------------------------
                CSVObject.Output(InfoPara.FURI_DATE)                            '�U�֓�
                CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '������
                CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    '�^�C���X�^���v
                CSVObject.Output(InfoTori.TORIS_CODE_T)                         '������R�[�h 
                CSVObject.Output(InfoTori.TORIF_CODE_T)                         '����敛�R�[�h
                CSVObject.Output(InfoTori.ITAKU_NNAME_T, True)                  '����於
                CSVObject.Output(InfoTori.TKIN_NO_T)                            '������Z�@�փR�[�h
                CSVObject.Output(InfoTori.TKIN_NNAME_N, True)                   '������Z�@�֖�
                CSVObject.Output(InfoTori.TSIT_NO_T)                            '����x�X�R�[�h
                CSVObject.Output(InfoTori.TSIT_NNAME_N, True)                   '����x�X��
                CSVObject.Output(InfoTori.FURI_CODE_T)                          '�U�փR�[�h
                CSVObject.Output(InfoTori.KIGYO_CODE_T)                         '��ƃR�[�h

                Select Case InfoTori.CODE_KBN_T                                 '�R�[�h�敪
                    Case "0"
                        CSVObject.Output("JIS")
                    Case "1"
                        CSVObject.Output("JIS���L(120)")
                    Case "2"
                        CSVObject.Output("JIS���L(119)")
                    Case "3"
                        CSVObject.Output("JIS���L(118)")
                    Case "4"
                        CSVObject.Output("EBCDIC")
                End Select

                '---------------------------------------
                '�C���v�b�g�G���[���
                '---------------------------------------
                CSVObject.Output(CStr(1))                                       '�ʔ�
                CSVObject.Output(InfoMei.KEIYAKU_KIN)                           '�������Z�@�փR�[�h
                CSVObject.Output(InfoMei2.KEIYAKU_KIN_KNAME.Trim, True)         '�������Z�@�֖�
                CSVObject.Output(InfoMei.KEIYAKU_SIT)                           '�����x�X�R�[�h
                CSVObject.Output(InfoMei2.KEIYAKU_SIT_KNAME.Trim, True)         '�����x�X��
                CSVObject.Output(InfoMei.KEIYAKU_KAMOKU)                        '�Ȗ�
                CSVObject.Output(InfoMei.KEIYAKU_KOUZA)                         '�����ԍ�

                CSVObject.Output(InfoMei.JYUYOKA_NO, True)                      '���v�Ɣԍ�
                'If inf.JYUYOKA_NO.Length > 10 Then                             '�_��Ҕԍ�
                '    CSVObject.Output(Left(inf.JYUYOKA_NO, 10))
                'Else
                '    CSVObject.Output(inf.JYUYOKA_NO)
                'End If

                CSVObject.Output(InfoMei.KEIYAKU_KNAME, True)                   '�_��Җ�
                CSVObject.Output(InfoMei.FURIKIN.ToString("0000000000"))        '�U�֋��z

                If aReadFmt.RecordDataNoChange.Substring(38, 1) = "1" Then
                    CSVObject.Output("�����ΏۊO �_����F" & aReadFmt.RecordDataNoChange.Substring(112, 8), True, True) '�G���[���b�Z�[�W
                Else
                    CSVObject.Output("�����ΏۊO �����F" & aReadFmt.RecordDataNoChange.Substring(112, 8), True, True) '�G���[���b�Z�[�W
                End If
            End If
            '2018/04/13 FJH) ���� �~�M���@FAX��{�萔���_���`�F�b�N�����ǉ� --------------------------------------------- END
        Catch ex As Exception
            BatchLog.Write("(�C���v�b�g�G���[���)", "���s", ex.Message)
        End Try
    End Sub

    '==================================================================
    ' �@�\�@ �F ������s
    ' ���l�@ �F 
    ' �쐬�� �F2009/09/16
    ' �X�V�� �F
    '==================================================================
    Public Overloads Overrides Function ReportExecute() As Boolean
        'Dim InErrFile As String

        'Try
        '    InErrFile = Path.Combine(Path.GetDirectoryName(CsvName), "INERR" & ToriSCode & ToriFCode & FuriDate & ".CSV")

        '    '�O��̏o�̓t�@�C���폜
        '    If File.Exists(InErrFile) = True Then
        '        File.Delete(InErrFile)
        '    End If

        '    File.Copy(CsvName, InErrFile)

        'Catch ex As System.Exception
        '    BatchLog.Write("(�C���v�b�g�G���[���)", "CSV�t�@�C���R�s�[���s�F" & ex.Message)
        'End Try

        '���[�o�͏���
        '2010/02/03 �ʏ�g�p����v�����^�ɂ��� ===
        'Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
        Return MyBase.ReportExecute()
        '=======================================================
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        TextData = Nothing
    End Sub






    ' ''
    '' �@�\�@ �F �C���v�b�g�G���[���Ƀg���[������t������
    ''
    '' ���l�@ �F 
    ''
    'Public Sub OutputCsvData(ByVal aReadFmt As CAstFormat.CFormat)
    '' �˗����׏��
    'Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast
    'Dim Memory As CAstReports.MEM = MEMObject

    '' �擪�Ɉʒu�Â�
    'Memory.Seek(0)
    'Dim LineData As String = Memory.ReadLine

    'If Not LineData Is Nothing Then
    '    Tenmei = aReadFmt.ToriData.INFOToriMast.TSIT_NNAME_N
    '    Itakusyamei = aReadFmt.ToriData.INFOToriMast.ITAKU_NNAME_T
    '    Hiduke = aReadFmt.ToriData.INFOParameter.FURI_DATE
    '    CreateCsvFile()
    'End If

    ''Do Until LineData Is Nothing
    ''    PutCsv(LineData)
    ''    PutCsv(InfoMei.TOTAL_IRAI_KEN.ToString)    '���v����
    ''    PutCsv(InfoMei.TOTAL_IRAI_KIN.ToString)    '���v���z
    ''    PutCsv(InfoMei.TOTAL_ZUMI_KEN.ToString)    '�U�֍ό���
    ''    PutCsv(InfoMei.TOTAL_ZUMI_KIN.ToString)    '�U�֍ϋ��z
    ''    PutCsv(InfoMei.TOTAL_FUNO_KEN.ToString)    '�U�֕s�\����
    ''    PutCsv(InfoMei.TOTAL_FUNO_KIN.ToString)    '�U�֕s�\���z
    ''    PutCsv("", True)

    ''Loop
    'End Sub

End Class
