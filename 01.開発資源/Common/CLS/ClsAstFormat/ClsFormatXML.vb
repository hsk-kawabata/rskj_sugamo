'*** Str Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή��i�V�K�쐬�j ***
Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Imports CASTCommon.ModPublic
Imports Microsoft.VisualBasic


' �O�t����` �f�[�^�t�H�[�}�b�g�N���X
Public Class CFormatXML

    ' �f�[�^�t�H�[�}�b�g���N���X
    Inherits CFormat

    ' ���O���x��
    Private IS_LEVEL3 As Boolean
    Private IS_LEVEL4 As Boolean

    ' XML�t�@�C����
    Private mXmlFile As String
    ' XML�t�H�[�}�b�groot�I�u�W�F�N�g
    Private mXmlRootElement As XmlElement
    Private mXmlRoot As XmlNode

    ' ��DLL��
    Private mDllName As String = ""
    ' �ʃN���X��
    Private mClassName As String = ""
    ' ��DLL�A�Z���u��
    Private mDllAsm As Assembly
    ' �ʃN���X�C���X�^���X
    Private mClassInstance As Object

    ' ���׃f�[�^���
    Private MEISAI_KIND_1 As Integer = 1
    Private MEISAI_KIND_2 As Integer = 2

    ' �G���h���R�[�h�敪
    Private EndKubun As String = ""

    ' �t�H�[�}�b�g�f�[�^�敪���X�g
    Private mHeaderList As New List(Of XmlNode)
    Private mDataList As New List(Of XmlNode)
    Private mTrailerList As New List(Of XmlNode)
    Private mEndList As New List(Of XmlNode)

    ' �t�H�[�}�b�g�f�[�^��
    Private mHeaderCount As Integer
    Private mDataCount As Integer
    Private mTrailerCount As Integer
    Private mEndCount As Integer

    ' ���׃��X�g
    Private mMeisaiHeaderList As XmlNodeList
    Private mMeisaiDataList1 As XmlNodeList
    Private mMeisaiDataList2 As XmlNodeList
    Private mMeisaiTrailerList As XmlNodeList
    Private mMeisaiEndList As XmlNodeList

    ' �J�i�E�v�ݒ胊�X�g
    Private mKanaTekiList As XmlNodeList

    ' ���R�[�h�`�F�b�N�ʃ��\�b�h
    Private mChkHeaderRecMethod As String = ""
    Private mChkDataRecMethod As String = ""
    Private mChkTrailerRecMethod As String = ""
    Private mChkEndRecMethod As String = ""

    ' �K�蕶���`�F�b�N�ʃ��\�b�h
    Private mChkHeaderRegularStrMethod As String = ""
    Private mChkDataRegularStrMethod As String = ""
    Private mChkTrailerRegularStrMethod As String = ""
    Private mChkEndRegularStrMethod As String = ""

    ' ���R�[�h�l�␳�ʃ��\�b�h
    Private mChgHeaderRecMethod As String = ""
    Private mChgDataRecMethod As String = ""
    Private mChgTrailerRecMethod As String = ""
    Private mChgEndRecMethod As String = ""

    ' �Ԋ�/�Ԋ҃f�[�^�쐬@�f�[�^�敪
    Private mHenkanHeaderNode As XmlNode
    Private mHenkanDataNode As XmlNode
    Private mHenkanTrailerNode As XmlNode

    ' �Ԋ�/��0�~�f�[�^
    Private mInclude0Yen As Integer = 1  ' 1: 0�~�f�[�^���܂߂�  0: 0�~�f�[�^���܂߂Ȃ�

	' �ĐU/�ĐU�f�[�^�쐬@�f�[�^�敪
    Private mSaifuriHeaderNode As XmlNode
    Private mSaifuriDataNode As XmlNode
    Private mSaifuriTrailerNode As XmlNode



    ' �@�\   �F �R���X�g���N�^
    ' ����   �F �t�H�[�}�b�g�敪
    '
    Public Sub New(ByVal formatCode As String)

        MyBase.New()

        BLOG = New CASTCommon.BatchLOG("ClsFormatXML", "CFormatXML")
        IS_LEVEL3 = BLOG.IS_LEVEL3()
        IS_LEVEL4 = BLOG.IS_LEVEL4()

        Dim xmlDoc As New ConfigXmlDocument
        Dim node As XmlNode
        Dim nodeList As XmlNodeList
        Dim sWork As String

        'XML�p�X�쐬
        Dim xmlFolderPath As String = CASTCommon.GetFSKJIni("COMMON", "XML_FORMAT_FLD")
        If xmlFolderPath = "err" Or xmlFolderPath = "" then
            Throw New Exception("fskj.ini��XML_FORMAT_FLD����`����Ă��܂���B")
        End If
        If xmlFolderPath.EndsWith("\") = False Then
            xmlFolderPath &= "\"
        End If
        mXmlFile = "XML_FORMAT_" & formatCode & ".xml"

        ' XML�t�H�[�}�b�g��root�I�u�W�F�N�g����
        xmlDoc.Load(xmlFolderPath & mXmlFile)
        mXmlRootElement = xmlDoc.DocumentElement
        mXmlRoot = mXmlRootElement.SelectSingleNode("/�O�t����`")
        If mXmlRoot Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�O�t����`�v�^�O����`����Ă��܂���B")
        End If

        ' ��DLL���i�l�ȗ��j
        node = mXmlRoot.SelectSingleNode("����/��DLL��")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v�^�O����`����Ă��܂���B")
        End If
        mDllName = node.InnerText.Trim
        ' ��DLL�����[�h
        If mDllName <> "" Then
            Try
                mDllAsm = System.Reflection.Assembly.LoadFrom(mDllName & ".dll")
            Catch ex As Exception
                BLOG.Write_Err("CFormatXML.New", ex)

                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v�^�O�Ŏw�肳�ꂽ" & mDllName & ".dll" & "��������܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End Try
        End If

        ' �ʃN���X���i�l�ȗ��j
        node = mXmlRoot.SelectSingleNode("����/�ʃN���X��")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�ʃN���X���v�^�O����`����Ă��܂���B")
        End If
        mClassName = node.InnerText.Trim

        If mClassName <> "" Then
            If mDllName = "" Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�ʃN���X���v���w�肷��ꍇ�́u����/��DLL���v���w�肵�Ă��������B")
            End If

            ' �ʃN���X���C���X�^���X��
            Try
                mClassInstance = mDllAsm.CreateInstance(mDllName & "." & mClassName)
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�ʃN���X���v�^�O�Ŏw�肳�ꂽ�N���X��" & mDllName & ".dll" & "�ɂ���܂���B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If
            Catch ex As Exception
                BLOG.Write_Err("CFormatXML.New", ex)

                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�ʃN���X���v�^�O�Ŏw�肳�ꂽ�N���X��" & mDllName & ".dll" & "�ɂ���܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End Try
        End If

        ' ���R�[�h�f�[�^���i�K�{�j
        node = mXmlRoot.SelectSingleNode("����/���R�[�h�f�[�^��")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/���R�[�h�f�[�^���v�^�O����`����Ă��܂���B")
        End If
        sWork = node.InnerText.Trim
        If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/���R�[�h�f�[�^���v�^�O�̒l�i" & sWork & "�j���s���ł��B�i" & _
                                ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
        End If
        DataInfo.RecoedLen = CInt(sWork)

        ' Ftran�p�����[�^�t�@�C���i�l�ȗ��j
        node = mXmlRoot.SelectSingleNode("����/Ftran�p�����[�^�t�@�C��")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/Ftran�p�����[�^�t�@�C���v�^�O����`����Ă��܂���B")
        End If
        FtranPfile = node.InnerText.Trim

        ' FtranIBM�p�����[�^�t�@�C���i�l�ȗ��j
        node = mXmlRoot.SelectSingleNode("����/FtranIBM�p�����[�^�t�@�C��")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/FtranIBM�p�����[�^�t�@�C���v�^�O����`����Ă��܂���B")
        End If
        FtranIBMPfile = node.InnerText.Trim

        ' FtranIBM�o�C�i���p�����[�^�t�@�C���i�l�ȗ��j
        node = mXmlRoot.SelectSingleNode("����/FtranIBM�o�C�i���p�����[�^�t�@�C��")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/FtranIBM�o�C�i���p�����[�^�t�@�C���v�^�O����`����Ă��܂���B")
        End If
        FtranIBMBinaryPfile = node.InnerText.Trim

        ' CMT�u���b�N�T�C�Y�i�l�ȗ��j
        node = mXmlRoot.SelectSingleNode("����/CMT�u���b�N�T�C�Y")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/CMT�u���b�N�T�C�Y�v�^�O����`����Ă��܂���B")
        End If
        sWork = node.InnerText.Trim
        If sWork <> "" Then
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/CMT�u���b�N�T�C�Y�v�^�O�̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            CMTBlockSize = CInt(sWork)
        Else
            CMTBlockSize = 0
        End If

        ' ���R�[�h�敪�ꗗ[@�f�[�^�敪='�w�b�_']/���R�[�h�敪�i�K�{�j
        nodeList = mXmlRoot.SelectNodes("����/���R�[�h�敪�ꗗ[@�f�[�^�敪='�w�b�_']/���R�[�h�敪")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/���R�[�h�敪�ꗗ[@�f�[�^�敪='�w�b�_']/���R�[�h�敪�v�^�O����`����Ă��܂���B")
        End If
        Dim HeaderKubunList As New Generic.List(Of String)
        For Each element As XmlElement In nodeList
            sWork = element.InnerText.Trim
            If sWork = "" Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/���R�[�h�敪�ꗗ/���R�[�h�敪�v�^�O�̒l���w�肳��Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(element) & "�s�ځj")
            End If
            HeaderKubunList.Add(sWork)
        Next
        HeaderKubun = HeaderKubunList.ToArray

        ' ���R�[�h�敪�ꗗ[@�f�[�^�敪='�f�[�^']/���R�[�h�敪�i�K�{�j
        nodeList = mXmlRoot.SelectNodes("����/���R�[�h�敪�ꗗ[@�f�[�^�敪='�f�[�^']/���R�[�h�敪")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/���R�[�h�敪�ꗗ[@�f�[�^�敪='�f�[�^']/���R�[�h�敪�v�^�O����`����Ă��܂���B")
        End If
        Dim DataKubunList As New Generic.List(Of String)
        For Each element As XmlElement In nodeList
            sWork = element.InnerText.Trim
            If sWork = "" Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/���R�[�h�敪�ꗗ/���R�[�h�敪�v�^�O�̒l���w�肳��Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(element) & "�s�ځj")
            End If
            DataKubunList.Add(sWork)
        Next
        DataKubun = DataKubunList.ToArray

        ' ���R�[�h�敪�ꗗ[@�f�[�^�敪='�g���[��']/���R�[�h�敪�i�K�{�j
        nodeList = mXmlRoot.SelectNodes("����/���R�[�h�敪�ꗗ[@�f�[�^�敪='�g���[��']/���R�[�h�敪")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/���R�[�h�敪�ꗗ[@�f�[�^�敪='�g���[��']/���R�[�h�敪�v�^�O����`����Ă��܂���B")
        End If
        Dim TrailerKubunList As New Generic.List(Of String)
        For Each element As XmlElement In nodeList
            sWork = element.InnerText.Trim
            If sWork = "" Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/���R�[�h�敪�ꗗ/���R�[�h�敪�v�^�O�̒l���w�肳��Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(element) & "�s�ځj")
            End If
            TrailerKubunList.Add(sWork)
        Next
        TrailerKubun = TrailerKubunList.ToArray

        ' ���R�[�h�敪�ꗗ[@�f�[�^�敪='�G���h']/���R�[�h�敪�i�K�{�j
        node = mXmlRoot.SelectSingleNode("����/���R�[�h�敪�ꗗ[@�f�[�^�敪='�G���h']/���R�[�h�敪")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/���R�[�h�敪�ꗗ[@�f�[�^�敪='�G���h']/���R�[�h�敪�v�^�O����`����Ă��܂���B")
        End If
        sWork = node.InnerText.Trim
        If sWork = "" Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/���R�[�h�敪�ꗗ/���R�[�h�敪�v�^�O�̒l���w�肳��Ă��܂���B�i" & _
                                ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
        End If
        EndKubun = sWork

        ' �Œᕪ�̃��R�[�h�敪�i�K�{�j
        nodeList = mXmlRoot.SelectNodes("����/�Œᕪ�̃��R�[�h�敪�ꗗ/���R�[�h�敪")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�Œᕪ�̃��R�[�h�敪�ꗗ/���R�[�h�敪�v�^�O����`����Ă��܂���B")
        End If
        Dim MinRecordCodeList As New Generic.List(Of String)
        Dim count As Integer = 0
        For Each element As XmlElement In nodeList
            count += 1
            sWork = element.InnerText.Trim
            If sWork = "" Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�Œᕪ�̃��R�[�h�敪�ꗗ/���R�[�h�敪�v�^�O�̒l���w�肳��Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(element) & "�s�ځj")
            End If

            If count = 1 And sWork <> HeaderKubun(0) Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�Œᕪ�̃��R�[�h�敪�ꗗ/���R�[�h�敪�v�^�O�̒l���u����/���R�[�h�敪�ꗗ[@�f�[�^�敪='�w�b�_']/���R�[�h�敪�v�̒l�ƈقȂ�܂��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(element) & "�s�ځj")
            End If

            If count = nodeList.Count And sWork <> EndKubun Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�Œᕪ�̃��R�[�h�敪�ꗗ/���R�[�h�敪�v�^�O�̒l���u����/���R�[�h�敪�ꗗ[@�f�[�^�敪='�G���h']/���R�[�h�敪�v�̒l�ƈقȂ�܂��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(element) & "�s�ځj")
            End If

            MinRecordCodeList.Add(sWork)
        Next
        DataInfo.MinRecordCode = MinRecordCodeList.ToArray

        ' ����/�t�H�[�}�b�g[@�f�[�^�敪='']/�f�[�^�ꗗ/�f�[�^"�i�K�{�j
        nodeList = mXmlRoot.SelectNodes("����/�t�H�[�}�b�g[@�f�[�^�敪='�w�b�_']/�f�[�^�ꗗ/�f�[�^")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='�w�b�_']/�f�[�^�ꗗ/�f�[�^�v�^�O����`����Ă��܂���B")
        End If
        mHeaderCount = nodeList.Count

        nodeList = mXmlRoot.SelectNodes("����/�t�H�[�}�b�g[@�f�[�^�敪='�f�[�^']/�f�[�^�ꗗ/�f�[�^")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='�f�[�^']/�f�[�^�ꗗ/�f�[�^�v�^�O����`����Ă��܂���B")
        End If
        mDataCount = nodeList.Count

        nodeList = mXmlRoot.SelectNodes("����/�t�H�[�}�b�g[@�f�[�^�敪='�g���[��']/�f�[�^�ꗗ/�f�[�^")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='�g���[��']/�f�[�^�ꗗ/�f�[�^�v�^�O����`����Ă��܂���B")
        End If
        mTrailerCount = nodeList.Count

        nodeList = mXmlRoot.SelectNodes("����/�t�H�[�}�b�g[@�f�[�^�敪='�G���h']/�f�[�^�ꗗ/�f�[�^")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='�G���h']/�f�[�^�ꗗ/�f�[�^�v�^�O����`����Ă��܂���B")
        End If
        mEndCount = nodeList.Count

        ' �K�蕶���`�F�b�N�ʃ��\�b�h���̎擾�i�l�ȗ��j
        node = mXmlRoot.SelectSingleNode("����/�t�H�[�}�b�g[@�f�[�^�敪='�w�b�_']/�K�蕶���`�F�b�N���\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='�w�b�_']/�K�蕶���`�F�b�N���\�b�h�v�^�O����`����Ă��܂���B")
        End If
        mChkHeaderRegularStrMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("����/�t�H�[�}�b�g[@�f�[�^�敪='�f�[�^']/�K�蕶���`�F�b�N���\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='�f�[�^']/�K�蕶���`�F�b�N���\�b�h�v�^�O����`����Ă��܂���B")
        End If
        mChkDataRegularStrMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("����/�t�H�[�}�b�g[@�f�[�^�敪='�g���[��']/�K�蕶���`�F�b�N���\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='�g���[��']/�K�蕶���`�F�b�N���\�b�h�v�^�O����`����Ă��܂���B")
        End If
        mChkTrailerRegularStrMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("����/�t�H�[�}�b�g[@�f�[�^�敪='�G���h']/�K�蕶���`�F�b�N���\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='�G���h']/�K�蕶���`�F�b�N���\�b�h�v�^�O����`����Ă��܂���B")
        End If
        mChkEndRegularStrMethod = node.InnerText.Trim

        ' �l��␳����ʃ��\�b�h���̎擾�i�l�ȗ��j
        node = mXmlRoot.SelectSingleNode("����/�t�H�[�}�b�g[@�f�[�^�敪='�w�b�_']/�l�␳���\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='�w�b�_']/�l�␳���\�b�h�v�^�O����`����Ă��܂���B")
        End If
        mChgHeaderRecMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("����/�t�H�[�}�b�g[@�f�[�^�敪='�f�[�^']/�l�␳���\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='�f�[�^']/�l�␳���\�b�h�v�^�O����`����Ă��܂���B")
        End If
        mChgDataRecMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("����/�t�H�[�}�b�g[@�f�[�^�敪='�g���[��']/�l�␳���\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='�g���[��']/�l�␳���\�b�h�v�^�O����`����Ă��܂���B")
        End If
        mChgTrailerRecMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("����/�t�H�[�}�b�g[@�f�[�^�敪='�G���h']/�l�␳���\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='�G���h']/�l�␳���\�b�h�v�^�O����`����Ă��܂���B")
        End If
        mChgEndRecMethod = node.InnerText.Trim


        ' ���׃f�[�^���ځi�K�{�j
        mMeisaiHeaderList = mXmlRoot.SelectNodes("��������[@�f�[�^�敪='�w�b�_']/���׃f�[�^�ꗗ/���׃f�[�^")
        If mMeisaiHeaderList Is Nothing OrElse mMeisaiHeaderList.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������[@�f�[�^�敪='�w�b�_']/���׃f�[�^�ꗗ/���׃f�[�^�v�^�O����`����Ă��܂���B")
        End If

        mMeisaiDataList1 = mXmlRoot.SelectNodes("��������[@�f�[�^�敪='�f�[�^']/���׃f�[�^�ꗗ/���׃f�[�^")
        If mMeisaiDataList1 Is Nothing OrElse mMeisaiDataList1.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������[@�f�[�^�敪='�f�[�^']/���׃f�[�^�ꗗ/���׃f�[�^�v�^�O����`����Ă��܂���B")
        End If

        mMeisaiDataList2 = mXmlRoot.SelectNodes("��������[@�f�[�^�敪='�f�[�^']/���[���ڈꗗ/���׃f�[�^")
        If mMeisaiDataList2 Is Nothing OrElse mMeisaiDataList2.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������[@�f�[�^�敪='�f�[�^']/���[���ڈꗗ/���׃f�[�^�v�^�O����`����Ă��܂���B")
        End If

        mMeisaiTrailerList = mXmlRoot.SelectNodes("��������[@�f�[�^�敪='�g���[��']/���׃f�[�^�ꗗ/���׃f�[�^")
        If mMeisaiTrailerList Is Nothing OrElse mMeisaiTrailerList.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������[@�f�[�^�敪='�g���[��']/���׃f�[�^�ꗗ/���׃f�[�^�v�^�O����`����Ă��܂���B")
        End If

        mMeisaiEndList = mXmlRoot.SelectNodes("��������[@�f�[�^�敪='�G���h']/���׃f�[�^�ꗗ/���׃f�[�^")
        If mMeisaiEndList Is Nothing OrElse mMeisaiEndList.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������[@�f�[�^�敪='�G���h']/���׃f�[�^�ꗗ/���׃f�[�^�v�^�O����`����Ă��܂���B")
        End If

        ' ���R�[�h�`�F�b�N�ʃ��\�b�h���̎擾�i�l�ȗ��j
        Dim methodname As String = ""
        node = mXmlRoot.SelectSingleNode("��������[@�f�[�^�敪='�w�b�_']/���R�[�h�`�F�b�N���\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������[@�f�[�^�敪='�w�b�_']/���R�[�h�`�F�b�N���\�b�h�v�^�O����`����Ă��܂���B")
        End If
        mChkHeaderRecMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("��������[@�f�[�^�敪='�f�[�^']/���R�[�h�`�F�b�N���\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������[@�f�[�^�敪='�f�[�^']/���R�[�h�`�F�b�N���\�b�h�v�^�O����`����Ă��܂���B")
        End If
        mChkDataRecMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("��������[@�f�[�^�敪='�g���[��']/���R�[�h�`�F�b�N���\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������[@�f�[�^�敪='�g���[��']/���R�[�h�`�F�b�N���\�b�h�v�^�O����`����Ă��܂���B")
        End If
        mChkTrailerRecMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("��������[@�f�[�^�敪='�G���h']/���R�[�h�`�F�b�N���\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������[@�f�[�^�敪='�G���h']/���R�[�h�`�F�b�N���\�b�h�v�^�O����`����Ă��܂���B")
        End If
        mChkEndRecMethod = node.InnerText.Trim

        ' ��������/�J�i�E�v�ݒ�i�K�{�j
        mKanaTekiList = mXmlRoot.SelectNodes("��������/�J�i�E�v�ݒ�")
        If mKanaTekiList Is Nothing OrElse mKanaTekiList.Count = 0 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������/�J�i�E�v�ݒ�v�^�O����`����Ă��܂���B")
        End If

        ' �Ԋ�/�Ԋ҃f�[�^�쐬[@�f�[�^�敪='�w�b�_'�i�K�{�j
        node = mXmlRoot.SelectSingleNode("�Ԋ�/�Ԋ҃f�[�^�쐬[@�f�[�^�敪='�w�b�_']")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬[@�f�[�^�敪='�w�b�_']�v�^�O����`����Ă��܂���B")
        End If
        mHenkanHeaderNode = node

        ' �Ԋ�/�Ԋ҃f�[�^�쐬[@�f�[�^�敪='�f�[�^'�i�K�{�j
        node = mXmlRoot.SelectSingleNode("�Ԋ�/�Ԋ҃f�[�^�쐬[@�f�[�^�敪='�f�[�^']")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬[@�f�[�^�敪='�f�[�^']�v�^�O����`����Ă��܂���B")
        End If
        mHenkanDataNode = node

        ' �Ԋ�/�Ԋ҃f�[�^�쐬[@�f�[�^�敪='�g���[��'�i�K�{�j
        node = mXmlRoot.SelectSingleNode("�Ԋ�/�Ԋ҃f�[�^�쐬[@�f�[�^�敪='�g���[��']")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬[@�f�[�^�敪='�g���[��']�v�^�O����`����Ă��܂���B")
        End If
        mHenkanTrailerNode = node

        ' �Ԋ�/��0�~�f�[�^�i�K�{�j
        node = mXmlRoot.SelectSingleNode("�Ԋ�/��0�~�f�[�^")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/��0�~�f�[�^�v�^�O����`����Ă��܂���B")
        End If

        sWork = node.InnerText.Trim
        If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 1 Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/��0�~�f�[�^�v�^�O�̒l�i" & sWork & "�j���s���ł��B�i" & _
                                ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
        End If
        mInclude0Yen = CInt(sWork)


        ' �ĐU/�ĐU�f�[�^�쐬[@�f�[�^�敪='�w�b�_'�i�K�{�j
        node = mXmlRoot.SelectSingleNode("�ĐU/�ĐU�f�[�^�쐬[@�f�[�^�敪='�w�b�_']")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬[@�f�[�^�敪='�w�b�_'�v�^�O����`����Ă��܂���B")
        End If
        mSaifuriHeaderNode = node

        ' �ĐU/�ĐU�f�[�^�쐬[@�f�[�^�敪='�f�[�^'�i�K�{�j
        node = mXmlRoot.SelectSingleNode("�ĐU/�ĐU�f�[�^�쐬[@�f�[�^�敪='�f�[�^']")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬[@�f�[�^�敪='�f�[�^'�v�^�O����`����Ă��܂���B")
        End If
        mSaifuriDataNode = node

        ' �ĐU/�ĐU�f�[�^�쐬[@�f�[�^�敪='�g���[��'�i�K�{�j
        node = mXmlRoot.SelectSingleNode("�ĐU/�ĐU�f�[�^�쐬[@�f�[�^�敪='�g���[��']")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬[@�f�[�^�敪='�g���[��'�v�^�O����`����Ă��܂���B")
        End If
        mSaifuriTrailerNode = node


        ' �o�^�o�����\�b�h�^�O�`�F�b�N
        node = mXmlRoot.SelectSingleNode("�o�^�o��/�������ݗp�o�^�o�����\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�o�^�o��/�������ݗp�o�^�o�����\�b�h�v�^�O����`����Ă��܂���B")
        End If

        node = mXmlRoot.SelectSingleNode("�o�^�o��/�Ԋҗp�o�^�o�����\�b�h")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�o�^�o��/�Ԋҗp�o�^�o�����\�b�h�v�^�O����`����Ă��܂���B")
        End If

    End Sub


    ' �@�\�@ �F �K�蕶���`�F�b�N ���@�����u������
    '
    ' �߂�l �F ���펞�́A-1
    '           �ُ펞�́A�s�������̈ʒu
    ' ���l�@ �F RepaceString()�֐��ɂĕ����u�������{
    '           �u���Ώە����́C�s�������ɂ͂Ȃ�Ȃ�
    '
    Public Overrides Function CheckRegularString() As Long

        Dim nRet As Long = -1

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.CheckRegularString")
        End If

            If IsHeaderRecord() Then
                nRet = CheckRegularStringInternal("�w�b�_")
            ElseIf IsDataRecord() Then
                nRet = CheckRegularStringInternal("�f�[�^")
            ElseIf IsTrailerRecord() Then
                nRet = CheckRegularStringInternal("�g���[��")
            ElseIf IsEndRecord() Then
                nRet = CheckRegularStringInternal("�G���h")
            End If

            Return nRet

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.CheckRegularString", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                If nRet <> -1 Then
                    BLOG.Write_LEVEL3("CFormatXML.CheckRegularString", "�G���[���R�[�h", RecordData)
                End If
                BLOG.Write_Exit3(sw, "CFormatXML.CheckRegularString", "���A�l=" & nRet)
            End If

        End Try

    End Function


    '
    ' �@�\�@ �F ���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[
    ' ���l�@ �F
    '
    Public Overrides Function CheckDataFormat() As String

        Dim errPos As Integer = 0
        Dim sRet As String = ""
        Dim datakubun As String = ""    ' �f�[�^�敪

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.CheckDataFormat")
            End If

            ' ���N���X�̃��R�[�h�f�[�^�`�F�b�N
            sRet = MyBase.CheckDataFormat()
            If sRet = "ERR" Then
                ' �K��O��������
                errPos = 1
                Return sRet
            End If

            If RecordData.Length = 0 Then
                DataInfo.Message = "�t�@�C���ُ�"
                mnErrorNumber = 1
                errPos = 2
                sRet = "ERR"
                Return sRet
            End If

            ' �w�b�_���R�[�h�̏ꍇ
            If IsHeaderRecord() Then
                If BeforeRecKbn <> "" And IsBeforeTrailerRecord() = False And IsBeforeEndRecord() = False Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�w�b�_�敪�j�ُ�"
                    mnErrorNumber = 1
                    errPos = 3
                    sRet = "ERR"
                    Return sRet
                Else
                    datakubun = "�w�b�_"

                    ' �w�b�_���R�[�h�𖾍׃f�[�^�ɐݒ�
                    sRet = CheckRecord1()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord1()
                    End If
                End If

            ' �f�[�^���R�[�h�̏ꍇ
            ElseIf IsDataRecord() Then
                If IsBeforeHeaderRecord() = False And IsBeforeDataRecord() = False Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�f�[�^�敪�j�ُ�"
                    mnErrorNumber = 1
                    errPos = 4
                    sRet = "ERR"
                    Return sRet
                Else
                    datakubun = "�f�[�^"

                    ' �f�[�^���R�[�h�𖾍׃f�[�^�ɐݒ�
                    sRet = CheckRecord2()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord2()
                    End If
                End If

            ' �g���[�����R�[�h�̏ꍇ
            ElseIf IsTrailerRecord() Then
                If IsBeforeHeaderRecord() = False And IsBeforeDataRecord() = False Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�g���[���敪�j�ُ�"
                    mnErrorNumber = 1
                    errPos = 5
                    sRet = "ERR"
                    Return sRet
                Else
                    datakubun = "�g���[��"

                    ' �g���[�����R�[�h�𖾍׃f�[�^�ɐݒ�
                    sRet = CheckRecord8()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord8()
                    End If
                End If

            ' �G���h���R�[�h�̏ꍇ
            ElseIf IsEndRecord() Then
                '�G���h���R�[�h�����������Ă�OK
                If IsBeforeTrailerRecord() = False Then
                    If IsBeforeEndRecord() = False Then
                        DataInfo.Message = "�t�@�C�����R�[�h�i�G���h�敪�j�ُ�"
                        mnErrorNumber = 1
                        errPos = 6
                        sRet = "ERR"
                        Return sRet
                    Else
                        datakubun = "�G���h"

                        ' �G���h���R�[�h�𖾍׃f�[�^�ɐݒ�
                        sRet = CheckRecord9()
                        sRet = "99"
                    End If
                Else
                    datakubun = "�G���h"

                    ' �G���h���R�[�h�𖾍׃f�[�^�ɐݒ�
                    sRet = CheckRecord9()
                End If

                If sRet = "ERR" Then
                    errPos = 7
                    Return sRet
                End If

            ' ���̑��̃��R�[�h�̏ꍇ
            Else
                If RecordData.Substring(0, 1) = ChrW(&H1A) Then
                    If IsBeforeEndRecord() = False Then
                        DataInfo.Message = "���R�[�h�敪�ُ�i1A�j�ُ�"
                        mnErrorNumber = 1
                        errPos = 8
                        sRet = "ERR"
                        Return sRet
                    Else
                        sRet = "1A"
                    End If
                Else
                    DataInfo.Message = "���R�[�h�敪�ُ�i" & RecordData.Substring(0, 1) & "�j�ُ�"
                    mnErrorNumber = 1
                    errPos = 9
                    sRet = "ERR"
                    Return sRet
                End If
            End If


            If sRet <> "ERR" AndAlso datakubun <> "" Then
                ' ���R�[�h�`�F�b�N�ʃ��\�b�h��
                Dim methodname As String = ""
                If datakubun = "�w�b�_" Then
                    methodname = mChkHeaderRecMethod
                ElseIf datakubun = "�f�[�^" Then
                    methodname = mChkDataRecMethod
                ElseIf datakubun = "�g���[��" Then
                    methodname = mChkTrailerRecMethod
                Else
                    methodname = mChkEndRecMethod
                End If

                If methodname <> "" Then
                    If mClassInstance Is Nothing Then
                        sRet = "Exception"
                        Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v,�u����/�ʃN���X���v����`����Ă��܂���B")
                    End If

                    '----------------------------------------------------------------
                    ' ���R�[�h�`�F�b�N�ʃ��\�b�h�ďo��
                    '----------------------------------------------------------------
                    If IS_LEVEL3 = True Then
                        BLOG.Write_LEVEL3("CFormatXML.CheckDataFormat", "���R�[�h�`�F�b�N�ʃ��\�b�h�ďo��", methodname)
                    End If

                    Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                    If methodInfo Is Nothing Then
                        sRet = "Exception"
                        Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������/���R�[�h�`�F�b�N���\�b�h�v�^�O�Ŏw�肳�ꂽ" & methodname & "��������܂���B")
                    End If

                    Dim methodParams() As Object = {Me}
                    If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                        BLOG.Write_Err("CFormatXML.CheckDataFormat", "���R�[�h�`�F�b�N�ʃ��\�b�h�G���[", methodname)

                        errPos = 10
                        sRet = "ERR"
                        Return sRet
                    End If
                End If
            End If

            ' ���N���X�̌㏈��
            MyBase.CheckDataFormatAfter()

            Return sRet

        Catch ex As Exception
            ' �{���\�b�h�ł̗�O�̏ꍇ
            If sRet = "Exception" Then
                BLOG.Write_Err("CFormatXML.CheckDataFormat", ex)
            End If

            ' �t�H�[�}�b�g�ϊ��G���[���b�Z�[�W��ݒ�
            DataInfo.Message = ex.Message

            Throw

        Finally
            If IS_LEVEL3 Then
                If sRet = "ERR" Then
                    BLOG.Write_LEVEL3("CFormatXML.CheckDataFormat", "�G���[�ʒu", CStr(errPos))
                    BLOG.Write_LEVEL3("CFormatXML.CheckDataFormat", "�G���[���R�[�h", RecordData)
                End If
            End If

            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.CheckDataFormat", "���A�l=" & sRet)
            End If

        End Try

    End Function


    '
    ' �@�\�@ �F �w�b�_���R�[�h�`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_
    '
    Public Overrides Function CheckRecord1() As String

        Dim sRet As String = ""

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.CheckRecord1")
            End If

            ' ���R�[�h�f�[�^���f�[�^�z��ɕϊ�
            Dim SplitedRecordData() As String = SplitRecordData("�w�b�_")

            ' ���׃f�[�^��񏉊���
            Call InfoMeisaiMast.Init()

            ' ���׃f�[�^���ڐݒ�
            ' ��`�ɏ]�����׃f�[�^�P�ɒl��ݒ肷��
            SetMeisaiInfo(MEISAI_KIND_1, mMeisaiHeaderList, SplitedRecordData)

            sRet = "H"
            Return sRet

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.CheckRecord1", ex)

            ' �t�H�[�}�b�g�ϊ��G���[���b�Z�[�W��ݒ�
            DataInfo.Message = ex.Message

            Throw

        Finally
            If sRet <> "" Then
                If IS_LEVEL3 = True Then
                    BLOG.Write_Exit3(sw, "CFormatXML.CheckRecord1", "���A�l=" & sRet)
                End If
            End If

        End Try

    End Function


    '
    ' �@�\�@ �F �f�[�^���R�[�h�`�F�b�N
    '
    ' �߂�l �F "D" - �f�[�^
    '
    Protected Overridable Function CheckRecord2() As String

        Dim sRet As String = ""
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.CheckRecord2")
            End If

            ' ���R�[�h�f�[�^���f�[�^�z��ɕϊ�
            Dim SplitedRecordData() As String = SplitRecordData("�f�[�^")

            ' ���׃f�[�^���N���A
            Call InfoMeisaiMast.InitData()

            ' ���׃f�[�^���ڐݒ�
            ' ��`�ɏ]�����׃f�[�^�P�ɒl��ݒ肷��
            SetMeisaiInfo(MEISAI_KIND_1, mMeisaiDataList1, SplitedRecordData)

            ' ���[�o�͍��ڐݒ�
            ' ��`�ɏ]�����׃f�[�^�Q�ɒl��ݒ肷��
            SetMeisaiInfo(MEISAI_KIND_2, mMeisaiDataList2, SplitedRecordData)

            sRet = "D"
            Return sRet

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.CheckRecord2", ex)

            ' �t�H�[�}�b�g�ϊ��G���[���b�Z�[�W��ݒ�
            DataInfo.Message = ex.Message

            Throw

        Finally
            If sRet <> "" Then
                If IS_LEVEL3 = True Then
                    BLOG.Write_Exit3(sw, "CFormatXML.CheckRecord2", "���A�l=" & sRet)
                End If
            End If

        End Try

    End Function


    '
    ' �@�\�@ �F �g���[���[���R�[�h�`�F�b�N
    '
    ' �߂�l �F "T" - �g���[��
    '
    Protected Function CheckRecord8() As String

        Dim sRet As String = ""
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.CheckRecord8")
            End If

            ' ���R�[�h�f�[�^���f�[�^�z��ɕϊ�
            Dim SplitedRecordData() As String = SplitRecordData("�g���[��")

            ' ���׃f�[�^���N���A
            Call InfoMeisaiMast.InitData()

            ' ���׃f�[�^���ڐݒ�
            ' ��`�ɏ]�����׃f�[�^�P�ɒl��ݒ肷��
            SetMeisaiInfo(MEISAI_KIND_1, mMeisaiTrailerList, SplitedRecordData)

            sRet = "T"
            Return sRet

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.CheckRecord8", ex)

            ' �t�H�[�}�b�g�ϊ��G���[���b�Z�[�W��ݒ�
            DataInfo.Message = ex.Message

            Throw

        Finally
            If sRet <> "" Then
                If IS_LEVEL3 = True Then
                    BLOG.Write_Exit3(sw, "CFormatXML.CheckRecord8", "���A�l=" & sRet)
                End If
            End If

        End Try

    End Function


    '
    ' �@�\�@ �F �G���h���R�[�h�`�F�b�N
    '
    ' �߂�l �F "E" - �G���h
    '
    Protected Function CheckRecord9() As String

        Dim sRet As String = ""
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.CheckRecord9")
            End If

            ' ���R�[�h�f�[�^���f�[�^�z��ɕϊ�
            Dim SplitedRecordData() As String = SplitRecordData("�G���h")

            ' ���׃f�[�^���N���A
            Call InfoMeisaiMast.InitData()

            ' ���׃f�[�^���ڐݒ�
            ' ��`�ɏ]�����׃f�[�^�P�ɒl��ݒ肷��
            SetMeisaiInfo(MEISAI_KIND_1, mMeisaiEndList, SplitedRecordData)

            sRet = "E"
            Return sRet

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.CheckRecord9", ex)

            ' �t�H�[�}�b�g�ϊ��G���[���b�Z�[�W��ݒ�
            DataInfo.Message = ex.Message

            Throw

        Finally
            If sRet <> "" Then
                If IS_LEVEL3 = True Then
                    BLOG.Write_Exit3(sw, "CFormatXML.CheckRecord9", "���A�l=" & sRet)
                End If
            End If

        End Try

    End Function


    ' �@�\�@ �F ���׃f�[�^��Ԋ҃w�b�_���R�[�h�ɐݒ肷��
    '
    Public Overrides Sub GetHenkanHeaderRecord()

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.GetHenkanHeaderRecord")
            End If

            If IsHeaderRecord() = False Then
                Return
            End If

            ' �w�b�_���R�[�h�𖾍׃f�[�^�ɐݒ�
            If CheckRecord1() = "ERR" Then
                Throw New Exception(Message)
            End If


            ' ���R�[�h�f�[�^���f�[�^�z��ɕϊ�
            Dim SplitedRecordData() As String = SplitRecordData("�w�b�_")

            Dim node As XmlNode
            Dim methodname As String = ""

            ' �Ԋ�/�Ԋ҃f�[�^�쐬@�f�[�^�敪
            node = mHenkanHeaderNode

            node = node.SelectSingleNode("���R�[�h�f�[�^�p�ʃ��\�b�h")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬[@�f�[�^�敪='�w�b�_']/���R�[�h�f�[�^�p�ʃ��\�b�h�v�^�O����`����Ă��܂���B")
            End If
            methodname = node.InnerText.Trim

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v,�u����/�ʃN���X���v����`����Ă��܂���B")
                End If

                '--------------------------------------------------
                ' ���R�[�h�f�[�^�ɑ΂���ʃ��\�b�h�ďo��
                '--------------------------------------------------
                If IS_LEVEL3 = True Then
                    BLOG.Write_LEVEL3("CFormatXML.GetHenkanHeaderRecord", "���R�[�h�f�[�^�p�ʃ��\�b�h�ďo��", methodname)
                End If

                Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                If methodInfo Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬/���R�[�h�f�[�^�p�ʃ��\�b�h�v�^�O�Ŏw�肳�ꂽ" & methodname & "��������܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If

                Dim methodParams() As Object = {Me, SplitedRecordData}
                If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                    Throw New Exception("���R�[�h�f�[�^�p�ʃ��\�b�h�G���[�i���\�b�h���F" & methodname & "�j")
                End If
            End If


            ' �������R�[�h�f�[�^�z����P���R�[�h�ɂ��ăv���p�e�B�ɐݒ�
            RecordData = String.Join("", SplitedRecordData)

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.GetHenkanHeaderRecord", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.GetHenkanHeaderRecord")
            End If

        End Try

    End Sub


    ' �@�\�@ �F ���׃f�[�^��Ԋ҃f�[�^���R�[�h�ɐݒ肷��
    '
    Public Overrides Sub GetHenkanDataRecord()

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.GetHenkanDataRecord")
            End If

            If IsDataRecord() = False Then
                Return
            End If

            ' ���R�[�h�f�[�^���f�[�^�z��ɕϊ�
            Dim SplitedRecordData() As String = SplitRecordData("�f�[�^")

            Dim node As XmlNode
            Dim attribute As XmlAttribute
            Dim sWork As String
            Dim methodname As String = ""
            Dim IndexFuriketu As Integer = 0

            ' �Ԋ�/�Ԋ҃f�[�^�쐬@�f�[�^�敪
            node = mHenkanDataNode

            ' �U�֌��ʔԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("�U�֌��ʔԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬�v�^�O�́u�U�֌��ʔԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬�v�^�O�́u�U�֌��ʔԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexFuriketu = CInt(sWork) - 1

            ' �U�֌��ʂ��Z�b�g
            SplitedRecordData(IndexFuriketu) = InfoMeisaiMast.FURIKETU_KEKKA

            ' EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
            If Not ReadByteBin Is Nothing Then
                ' �o�C�i���f�[�^�̐U�֌��ʂ�����������
                Dim index As Integer = 0
                For i As Integer = 0 To IndexFuriketu - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetHenkanDataRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexFuriketu))
            End If

            node = node.SelectSingleNode("���R�[�h�f�[�^�p�ʃ��\�b�h")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬[@�f�[�^�敪='�f�[�^']/���R�[�h�f�[�^�p�ʃ��\�b�h�v�^�O����`����Ă��܂���B")
            End If
            methodname = node.InnerText.Trim

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v,�u����/�ʃN���X���v����`����Ă��܂���B")
                End If

                '--------------------------------------------------
                ' ���R�[�h�f�[�^�ɑ΂���ʃ��\�b�h�ďo��
                '--------------------------------------------------
                If IS_LEVEL3 = True Then
                    BLOG.Write_LEVEL3("CFormatXML.GetHenkanDataRecord", "���R�[�h�f�[�^�p�ʃ��\�b�h�ďo��", methodname)
                End If

                Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                If methodInfo Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬/���R�[�h�f�[�^�p�ʃ��\�b�h�v�^�O�Ŏw�肳�ꂽ" & methodname & "��������܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If

                Dim methodParams() As Object = {Me, SplitedRecordData}
                If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                    Throw New Exception("���R�[�h�f�[�^�p�ʃ��\�b�h�G���[�i���\�b�h���F" & methodname & "�j")
                End If
            End If


            ' �������R�[�h�f�[�^�z����P���R�[�h�ɂ��ăv���p�e�B�ɐݒ�
            RecordData = String.Join("", SplitedRecordData)

            ' �f�[�^���R�[�h�𖾍׃f�[�^�ɐݒ�
            If CheckRecord2() = "ERR" Then
                Throw New Exception(Message)
            End If

            ' ���N���X�̌㏈��
            Call MyBase.GetHenkanDataRecord()

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.GetHenkanDataRecord", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.GetHenkanDataRecord")
            End If

        End Try

    End Sub


    ' �@�\�@ �F ���׃f�[�^��Ԋ҃g���[�����R�[�h�ɐݒ肷��
    '
    Public Overrides Sub GetHenkanTrailerRecord()

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.GetHenkanTrailerRecord")
            End If

            If IsTrailerRecord() = False Then
                Return
            End If

            ' �g���[�����R�[�h�𖾍׃f�[�^�ɐݒ�
            If CheckRecord8() = "ERR" Then
                Throw New Exception(Message)
            End If

            ' ���R�[�h�f�[�^���f�[�^�z��ɕϊ�
            Dim SplitedRecordData() As String = SplitRecordData("�g���[��")

            Dim node As XmlNode
            Dim attribute As XmlAttribute
            Dim sWork As String
            Dim methodname As String = ""
            Dim IndexNormKen As Integer = 0
            Dim IndexNormKin As Integer = 0
            Dim IndexIjoKen As Integer = 0
            Dim IndexIjoKin As Integer = 0

            ' �Ԋ�/�Ԋ҃f�[�^�쐬@�f�[�^�敪
            node = mHenkanTrailerNode

            ' ���팏���ԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("���팏���ԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬�v�^�O�́u���팏���ԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬�v�^�O�́u���팏���ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexNormKen = CInt(sWork) - 1

            ' ������z�ԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("������z�ԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬�v�^�O�́u������z�ԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬�v�^�O�́u������z�ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexNormKin = CInt(sWork) - 1

            ' �s�\�����ԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("�s�\�����ԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬�v�^�O�́u�s�\�����ԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬�v�^�O�́u�s�\�����ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexIjoKen = CInt(sWork) - 1

            ' �s�\���z�ԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("�s�\���z�ԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬�v�^�O�́u�s�\���z�ԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬�v�^�O�́u�s�\���z�ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexIjoKin = CInt(sWork) - 1

            ' �U�֍ς݌������Z�b�g
            ' ���v���팏�� �v�Z�l�ɁA0�~�f�[�^���܂߂�ꍇ
            If mInclude0Yen = 1 Then
                SplitedRecordData(IndexNormKen) = InfoMeisaiMast.TOTAL_NORM_KEN.ToString.PadLeft(SplitedRecordData(IndexNormKen).Length, "0"c)
            ' ���v���팏�� �v�Z�l�ɁA0�~�f�[�^���܂߂Ȃ��ꍇ
            Else
                SplitedRecordData(IndexNormKen) = InfoMeisaiMast.TOTAL_NORM_KEN2.ToString.PadLeft(SplitedRecordData(IndexNormKen).Length, "0"c)
            End If

            ' �U�֍ς݋��z���Z�b�g
            SplitedRecordData(IndexNormKin) = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(SplitedRecordData(IndexNormKin).Length, "0"c)

            ' �U�֕s�\�������Z�b�g
            SplitedRecordData(IndexIjoKen) = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(SplitedRecordData(IndexIjoKen).Length, "0"c)

            ' �U�֕s�\���z���Z�b�g
            SplitedRecordData(IndexIjoKin) = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(SplitedRecordData(IndexIjoKin).Length, "0"c)

            'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
            If Not ReadByteBin Is Nothing Then
                ' �o�C�i���f�[�^�̐U�֍ς݌���������������
                Dim index As Integer = 0
                For i As Integer = 0 To IndexNormKen - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetHenkanTrailerRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexNormKen))

                ' �o�C�i���f�[�^�̐U�֍ς݋��z������������
                index = 0
                For i As Integer = 0 To IndexNormKin - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetHenkanTrailerRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexNormKin))

                ' �o�C�i���f�[�^�̐U�֕s�\����������������
                index = 0
                For i As Integer = 0 To IndexIjoKen - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetHenkanTrailerRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexIjoKen))

                ' �o�C�i���f�[�^�̐U�֕s�\���z������������
                index = 0
                For i As Integer = 0 To IndexIjoKin - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetHenkanTrailerRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexIjoKin))
            End If


            node = node.SelectSingleNode("���R�[�h�f�[�^�p�ʃ��\�b�h")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬[@�f�[�^�敪='�g���[��']/���R�[�h�f�[�^�p�ʃ��\�b�h�v�^�O����`����Ă��܂���B")
            End If
            methodname = node.InnerText.Trim

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v,�u����/�ʃN���X���v����`����Ă��܂���B")
                End If

                '--------------------------------------------------
                ' ���R�[�h�f�[�^�ɑ΂���ʃ��\�b�h�ďo��
                '--------------------------------------------------
                If IS_LEVEL3 = True Then
                    BLOG.Write_LEVEL3("CFormatXML.GetHenkanTrailerRecord", "���R�[�h�f�[�^�p�ʃ��\�b�h�ďo��", methodname)
                End If

                Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                If methodInfo Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�Ԋ�/�Ԋ҃f�[�^�쐬/���R�[�h�f�[�^�p�ʃ��\�b�h�v�^�O�Ŏw�肳�ꂽ" & methodname & "��������܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If

                Dim methodParams() As Object = {Me, SplitedRecordData}
                If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                    Throw New Exception("���R�[�h�f�[�^�p�ʃ��\�b�h�G���[�i���\�b�h���F" & methodname & "�j")
                End If
            End If

            ' �������R�[�h�f�[�^�z����P���R�[�h�ɂ��ăv���p�e�B�ɐݒ�
            RecordData = String.Join("", SplitedRecordData)

            ' ���N���X�̌㏈��
            Call MyBase.GetHenkanTrailerRecord()

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.GetHenkanTrailerRecord", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.GetHenkanTrailerRecord")
            End If

        End Try

    End Sub


    ' �@�\�@ �F ���׃f�[�^���ĐU�w�b�_���R�[�h�ɐݒ肷��
    ' ����   �F �U�֓�
    '
    Public Overrides Sub GetSaifuriHeaderRecord(ByVal SAIFURI_DATE As String)

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.GetSaifuriHeaderRecord")
            End If

            If IsHeaderRecord() = False Then
                Return
            End If

            ' �w�b�_���R�[�h�𖾍׃f�[�^�ɐݒ�
            If CheckRecord1() = "ERR" Then
                Throw New Exception(Message)
            End If

            ' ���R�[�h�f�[�^���f�[�^�z��ɕϊ�
            Dim SplitedRecordData() As String = SplitRecordData("�w�b�_")

            Dim node As XmlNode
            Dim attribute As XmlAttribute
            Dim sWork As String
            Dim methodname As String = ""
            Dim IndexFuridate As Integer = 0

            ' �ĐU/�ĐU�f�[�^�쐬@�f�[�^�敪
            node = mSaifuriHeaderNode

            ' �U�֓��ԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("�U�֓��ԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u�U�֓��ԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u�U�֓��ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexFuridate = CInt(sWork) - 1

            '�ĐU�����Z�b�g
            SplitedRecordData(IndexFuridate) = SAIFURI_DATE

            'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
            If Not ReadByteBin Is Nothing Then
                ' �o�C�i���f�[�^�̐U�֌��ʂ�����������
                Dim index As Integer = 0
                For i As Integer = 0 To IndexFuridate - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriHeaderRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexFuridate))
            End If

            node = node.SelectSingleNode("���R�[�h�f�[�^�p�ʃ��\�b�h")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬[@�f�[�^�敪='�w�b�_']/���R�[�h�f�[�^�p�ʃ��\�b�h�v�^�O����`����Ă��܂���B")
            End If
            methodname = node.InnerText.Trim

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v,�u����/�ʃN���X���v����`����Ă��܂���B")
                End If

                '--------------------------------------------------
                ' ���R�[�h�f�[�^�ɑ΂���ʃ��\�b�h�ďo��
                '--------------------------------------------------
                If IS_LEVEL3 = True Then
                    BLOG.Write_LEVEL3("CFormatXML.GetSaifuriHeaderRecord", "���R�[�h�f�[�^�p�ʃ��\�b�h�ďo��", methodname)
                End If

                Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                If methodInfo Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬/���R�[�h�f�[�^�p�ʃ��\�b�h�v�^�O�Ŏw�肳�ꂽ" & methodname & "��������܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If

                Dim methodParams() As Object = {Me, SplitedRecordData}
                If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                    Throw New Exception("���R�[�h�f�[�^�p�ʃ��\�b�h�G���[�i���\�b�h���F" & methodname & "�j")
                End If
            End If

            ' �������R�[�h�f�[�^�z����P���R�[�h�ɂ��ăv���p�e�B�ɐݒ�
            RecordData = String.Join("", SplitedRecordData)

            ' ���N���X�̌㏈��
            Call MyBase.GetSaifuriHeaderRecord(SAIFURI_DATE)

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.GetSaifuriHeaderRecord", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.GetSaifuriHeaderRecord")
            End If

        End Try

    End Sub


    ' �@�\�@ �F ���׃f�[�^���ĐU�f�[�^���R�[�h�ɐݒ肷��
    '
    Public Overrides Sub GetSaifuriDataRecord()

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.GetSaifuriDataRecord")
            End If

            If IsDataRecord() = False Then
                Return
            End If

            ' ���R�[�h�f�[�^���f�[�^�z��ɕϊ�
            Dim SplitedRecordData() As String = SplitRecordData("�f�[�^")

            Dim node As XmlNode
            Dim attribute As XmlAttribute
            Dim sWork As String
            Dim methodname As String = ""
            Dim IndexFuriketu As Integer = 0

            ' �ĐU/�ĐU�f�[�^�쐬@�f�[�^�敪
            node = mSaifuriDataNode

            ' �U�֌��ʔԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("�U�֌��ʔԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u�U�֌��ʔԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u�U�֌��ʔԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexFuriketu = CInt(attribute.Value.Trim) - 1


            ' �U�֌��ʂ��Z�b�g
            SplitedRecordData(IndexFuriketu) = InfoMeisaiMast.FURIKETU_KEKKA

            ' EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
            If Not ReadByteBin Is Nothing Then
                ' �o�C�i���f�[�^�̐U�֌��ʂ�����������
                Dim index As Integer = 0
                For i As Integer = 0 To IndexFuriketu - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriDataRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexFuriketu))
            End If

            node = node.SelectSingleNode("���R�[�h�f�[�^�p�ʃ��\�b�h")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬[@�f�[�^�敪='�f�[�^']/���R�[�h�f�[�^�p�ʃ��\�b�h�v�^�O����`����Ă��܂���B")
            End If
            methodname = node.InnerText.Trim

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v,�u����/�ʃN���X���v����`����Ă��܂���B")
                End If

                '--------------------------------------------------
                ' ���R�[�h�f�[�^�ɑ΂���ʃ��\�b�h�ďo��
                '--------------------------------------------------
                If IS_LEVEL3 = True Then
                    BLOG.Write_LEVEL3("CFormatXML.GetSaifuriDataRecord", "���R�[�h�f�[�^�p�ʃ��\�b�h�ďo��", methodname)
                End If

                Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                If methodInfo Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬/���R�[�h�f�[�^�p�ʃ��\�b�h�v�^�O�Ŏw�肳�ꂽ" & methodname & "��������܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If

                Dim methodParams() As Object = {Me, SplitedRecordData}
                If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                    Throw New Exception("���R�[�h�f�[�^�p�ʃ��\�b�h�G���[�i���\�b�h���F" & methodname & "�j")
                End If
            End If

            ' �������R�[�h�f�[�^�z����P���R�[�h�ɂ��ăv���p�e�B�ɐݒ�
            RecordData = String.Join("", SplitedRecordData)

            ' �f�[�^���R�[�h�𖾍׃f�[�^�ɐݒ�
            If CheckRecord2() = "ERR" Then
                Throw New Exception(Message)
            End If

            ' ���N���X�̌㏈��
            Call MyBase.GetSaifuriDataRecord()

            '�f�[�^���R�[�h�̐U�֌��ʂ�0�ɂ���
            '�U�֌��ʂ��Z�b�g
            SplitedRecordData(IndexFuriketu) = "0"

            'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
            If Not ReadByteBin Is Nothing Then
                ' �o�C�i���f�[�^�̐U�֌��ʂ�����������
                Dim index As Integer = 0
                For i As Integer = 0 To IndexFuriketu - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriDataRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexFuriketu))
            End If

            ' �������R�[�h�f�[�^�z����P���R�[�h�ɂ��ăv���p�e�B�ɐݒ�
            RecordData = String.Join("", SplitedRecordData)

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.GetSaifuriDataRecord", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.GetSaifuriDataRecord")
            End If

        End Try

    End Sub


    ' �@�\�@ �F ���׃f�[�^���ĐU�g���[�����R�[�h�ɐݒ肷��
    ' ����   �F �˗����v
    '           ���z���v
    '           �����̏����ݗL��
    '
    Public Overrides Sub GetSaifuriTrailerRecord(Optional ByVal SyoriKen As Long = 0, Optional ByVal SyoriKin As Long = 0, _
                                                       Optional ByVal Write As Boolean = False)

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.GetSaifuriTrailerRecord")
            End If

            If IsTrailerRecord() = False Then
                Return
            End If

            ' �g���[�����R�[�h�𖾍׃f�[�^�ɐݒ�
            If CheckRecord8() = "ERR" Then
                Throw New Exception(Message)
            End If

            ' ���R�[�h�f�[�^���f�[�^�z��ɕϊ�
            Dim SplitedRecordData() As String = SplitRecordData("�g���[��")

            Dim node As XmlNode
            Dim attribute As XmlAttribute
            Dim sWork As String
            Dim methodname As String = ""
            Dim IndexKen As Integer = 0
            Dim IndexKin As Integer = 0
            Dim IndexNormKen As Integer = 0
            Dim IndexNormKin As Integer = 0
            Dim IndexIjoKen As Integer = 0
            Dim IndexIjoKin As Integer = 0

            ' �ĐU/�ĐU�f�[�^�쐬@�f�[�^�敪
            node = mSaifuriTrailerNode

            ' �˗����v�ԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("�˗����v�ԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u�˗����v�ԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u�˗����v�ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexKen = CInt(attribute.Value.Trim) - 1

            ' ���z���v�ԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("���z���v�ԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u���z���v�ԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u���z���v�ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexKin = CInt(attribute.Value.Trim) - 1

            ' ���팏���ԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("���팏���ԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u���팏���ԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u���팏���ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexNormKen = CInt(attribute.Value.Trim) - 1

            ' ������z�ԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("������z�ԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u������z�ԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u������z�ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexNormKin = CInt(attribute.Value.Trim) - 1

            ' �s�\�����ԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("�s�\�����ԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u�s�\�����ԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u�s�\�����ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexIjoKen = CInt(attribute.Value.Trim) - 1

            ' �s�\���z�ԍ��i�K�{�j
            attribute = node.Attributes.ItemOf("�s�\���z�ԍ�")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u�s�\���z�ԍ��v��������`����Ă��܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬�v�^�O�́u�s�\���z�ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If
            IndexIjoKin = CInt(attribute.Value.Trim) - 1


           ' �U�֕s�\�������Z�b�g
            If Write = True Then
                SplitedRecordData(IndexKen) = SyoriKen.ToString.PadLeft(SplitedRecordData(IndexKen).Length, "0"c)
                SplitedRecordData(IndexKin) = SyoriKin.ToString.PadLeft(SplitedRecordData(IndexKin).Length, "0"c)
            Else
                SplitedRecordData(IndexKen) = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(SplitedRecordData(IndexKen).Length, "0"c)
                SplitedRecordData(IndexKin) = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(SplitedRecordData(IndexKin).Length, "0"c)
            End If

            ' 0���Z�b�g
            '���v���팏��
            SplitedRecordData(IndexNormKen) = "0".PadLeft(SplitedRecordData(IndexNormKen).Length, "0"c)
            ' �U�֍ς݋��z
            SplitedRecordData(IndexNormKin) = "0".PadLeft(SplitedRecordData(IndexNormKin).Length, "0"c)
            ' �U�֕s�\����
            SplitedRecordData(IndexIjoKen) = "0".PadLeft(SplitedRecordData(IndexIjoKen).Length, "0"c)
            ' �U�֕s�\���z
            SplitedRecordData(IndexIjoKin) = "0".PadLeft(SplitedRecordData(IndexIjoKin).Length, "0"c)

            'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
            If Not ReadByteBin Is Nothing Then
                ' �o�C�i���f�[�^�̈˗����v������������
                Dim index As Integer = 0
                For i As Integer = 0 To IndexKen - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriTrailerRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexKen))

                ' �o�C�i���f�[�^�̋��z���v������������
                index = 0
                For i As Integer = 0 To IndexKin - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriTrailerRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexKin))

                ' �o�C�i���f�[�^�̐U�֍ς݌���������������
                index = 0
                For i As Integer = 0 To IndexNormKen - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriTrailerRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexNormKen))

                ' �o�C�i���f�[�^�̐U�֍ς݋��z������������
                index = 0
                For i As Integer = 0 To IndexNormKin - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriTrailerRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexNormKin))

                ' �o�C�i���f�[�^�̐U�֕s�\����������������
                index = 0
                For i As Integer = 0 To IndexIjoKen - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriTrailerRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexIjoKen))

                ' �o�C�i���f�[�^�̐U�֕s�\���z������������
                index = 0
                For i As Integer = 0 To IndexIjoKin - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriTrailerRecord", "�o�C�i���f�[�^�U�֌��ʏ�����", "�������ʒu=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexIjoKin))
            End If

            node = node.SelectSingleNode("���R�[�h�f�[�^�p�ʃ��\�b�h")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬[@�f�[�^�敪='�g���[��']/���R�[�h�f�[�^�p�ʃ��\�b�h�v�^�O����`����Ă��܂���B")
            End If
            methodname = node.InnerText.Trim

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v,�u����/�ʃN���X���v����`����Ă��܂���B")
                End If

                '--------------------------------------------------
                ' ���R�[�h�f�[�^�ɑ΂���ʃ��\�b�h�ďo��
                '--------------------------------------------------
                If IS_LEVEL3 = True Then
                    BLOG.Write_LEVEL3("CFormatXML.GetSaifuriTrailerRecord", "���R�[�h�f�[�^�p�ʃ��\�b�h�ďo��", methodname)
                End If

                Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                If methodInfo Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�ĐU/�ĐU�f�[�^�쐬/���R�[�h�f�[�^�p�ʃ��\�b�h�v�^�O�Ŏw�肳�ꂽ" & methodname & "��������܂���B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If

                Dim methodParams() As Object = {Me, SplitedRecordData}
                If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                    Throw New Exception("���R�[�h�f�[�^�p�ʃ��\�b�h�G���[�i���\�b�h���F" & methodname & "�j")
                End If
            End If

            ' �������R�[�h�f�[�^�z����P���R�[�h�ɂ��ăv���p�e�B�ɐݒ�
            RecordData = String.Join("", SplitedRecordData)

            ' ���N���X�̌㏈��
            Call MyBase.GetSaifuriTrailerRecord(SyoriKin, SyoriKin, Write)

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.GetSaifuriTrailerRecord", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.GetSaifuriTrailerRecord")
            End If

        End Try

    End Sub


    ' �@�\�@ �F �K�蕶���`�F�b�N ���@�����u����������
    '
    ' �p�����[�^ �F �f�[�^�敪
    '
    ' �߂�l �F �s�������̈ʒu
    '
    ' ���l�@ �F RepaceString()�֐��ɂĕ����u�������{
    '           �u���Ώە����́C�s�������ɂ͂Ȃ�Ȃ�
    '
    Private Function CheckRegularStringInternal(ByVal DataKubun As String) As Long

        Dim nRet As Long = -1
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.CheckRegularStringInternal",  DataKubun)
            End If

            Dim buff As New StringBuilder(DataInfo.LenOfOneRec)
            Dim RD() As Byte = EncdJ.GetBytes(RecordData)
            Dim sWork As String

            Dim list As List(Of XmlNode) = Nothing
            Dim count As Integer
            If DataKubun = "�w�b�_" Then
                list = mHeaderList
                count = mHeaderCount
            ElseIf DataKubun = "�f�[�^" Then
                list = mDataList
                count = mDataCount
            ElseIf DataKubun = "�g���[��" Then
                list = mTrailerList
                count = mTrailerCount
            Else
                list = mEndList
                count = mEndCount
            End If

            Dim copyIndex As Integer = 0 ' �o�C�g�z�񂩂�̕��������񐶐����̃R�s�[�ʒu

            Dim node As XmlNode = Nothing

            For i As Integer = 1 To count
                If list.Count < count Then
                    ' ����/�t�H�[�}�b�g[@�f�[�^�敪='']/�f�[�^�ꗗ/�f�[�^[@�ԍ�='']"�i�K�{�j
                    node = mXmlRoot.SelectSingleNode("����/�t�H�[�}�b�g[@�f�[�^�敪='" & DataKubun & "']/�f�[�^�ꗗ/�f�[�^[@�ԍ�='" & i & "']")
                    If node Is Nothing Then
                        Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='" & DataKubun & "']/�f�[�^�ꗗ/�f�[�^[@�ԍ�='" & i & "']�v�^�O����`����Ă��܂���B")
                    End If

                    list.Add(node)
                Else
                    node = list.Item(i - 1)
                End IF

                ' �f�[�^���̐ݒ�l�`�F�b�N�i�K�{�j
                Dim dataLengthAttr As XmlAttribute = node.Attributes.ItemOf("�f�[�^��")
                If dataLengthAttr Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�f�[�^�ꗗ/�f�[�^�v�^�O�́u�f�[�^���v��������`����Ă��܂���B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If
                sWork = dataLengthAttr.Value.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�f�[�^�ꗗ/�f�[�^�v�^�O�́u�f�[�^���v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If
                ' �f�[�^���̎擾
                Dim dataLength As Integer = CInt(sWork)

                ' �f�[�^���̍��v�����R�[�h�f�[�^���𒴂���ꍇ
                If (copyIndex + dataLength) > DataInfo.RecoedLen Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�f�[�^�ꗗ/�f�[�^�v�^�O�́u�f�[�^���v�����̒l�i" & sWork & "�j�ł͍��v�����R�[�h�f�[�^���𒴂��܂��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If

                ' �f�[�^�^�̐ݒ�l�`�F�b�N�i�K�{�j
                Dim dataTypeAttr As XmlAttribute = node.Attributes.ItemOf("�f�[�^�^")
                If dataTypeAttr Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�f�[�^�ꗗ/�f�[�^�v�^�O�́u�f�[�^�^�v��������`����Ă��܂���B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If
                sWork = dataTypeAttr.Value.Trim
                If sWork <> "������" AndAlso sWork <> "���l" Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�f�[�^�ꗗ/�f�[�^�v�^�O�́u�f�[�^�^�v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If
                ' �f�[�^�^�̎擾
                Dim dataType As String = sWork

                ' �K��O��������̐ݒ�l�`�F�b�N�i�K�{�j
                Dim dataCheck As Integer = 0 ' �`�F�b�N���Ȃ�
                Dim dataCheckAttr As XmlAttribute = node.Attributes.ItemOf("�K��O��������")
                If dataCheckAttr Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�f�[�^�ꗗ/�f�[�^�v�^�O�́u�K��O��������v��������`����Ă��܂���B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If
                sWork = dataCheckAttr.Value.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 1 Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�f�[�^�ꗗ/�f�[�^�v�^�O�́u�K��O��������v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If
                dataCheck = CInt(sWork)

                ' �o�C�g�z�񂩂畔�������񐶐�
                Dim value As String = EncdJ.GetString(RD, copyIndex, dataLength)

                If dataType = "���l" Then
                    If value.Trim <> "" AndAlso IsNumeric(value.Trim) = False Then
                        Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='" & DataKubun & "']/�f�[�^�ꗗ/�f�[�^[@�ԍ�='" & i & "' �f�[�^�^='���l']�v�^�O�ɑΉ�����f�[�^�i" & value.Trim & "�j�����l�ł͂���܂���B�i" & _
                                            ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                    End If

                    buff.Append(value)
                Else
                    ' ������̏ꍇ�A�K��O������u���i�K��O�����ϊ��p�^�[��.txt�̒u���j�����{
                    buff.Append(MyBase.ReplaceString(value))
                End If

                ' �K��O��������̎��{�i���ɋK��O�������������ꍇ�̓`�F�b�N���Ȃ����ŏ��̕s�������̈ʒu��Ԃ��j
                If dataCheck = 1 AndAlso nRet = -1 Then
                    ' �K�蕶���`�F�b�N
                    Dim rc As Long = MyBase.CheckRegularStringVerA(value)
                    If rc = -1 Then
                        ' �K�蕶���`�F�b�N�ł�����͂������A�S�⃍�W�b�N�ł́A�K�蕶���`�F�b�N��ɑS�p�`�F�b�N������̂ō��킹�Ă���
                        rc = MyBase.GetZenkakuPos(value)
                    End If

                    If rc >= 0 Then
                        nRet = copyIndex + rc
                    End If

                End If

                copyIndex += dataLength
            Next

            ' �f�[�^���̍��v�����R�[�h�f�[�^���ɖ����Ȃ��ꍇ
            If copyIndex < DataInfo.RecoedLen Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�f�[�^�ꗗ/�f�[�^�v�^�O�́u�f�[�^���v�����̍��v�����R�[�h�f�[�^����菬�����ł��B�i" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            ' �Ǎ��񂾃v���p�e�B�̃��R�[�h���K��O�����ϊ���̃��R�[�h�ɒu������
            If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                mRecordData = buff.ToString(0, RecordLen)
            End If

            ' �K��O��������ŋK��O���������������ꍇ
            If nRet >= 0 Then
                ' �K��O�������������ꍇ�A�s���̉��s�R�[�h�Ȃ琳�툵���Ƃ���
                If nRet = DataInfo.RecoedLen - 2 Then
                    If mRecordData.Substring(DataInfo.RecoedLen - 2) = Environment.NewLine Then
                        nRet = -1
                    End If
                ElseIf nRet = DataInfo.RecoedLen - 1 Then
                    If mRecordData.Substring(DataInfo.RecoedLen - 1) = vbCr OrElse _
                       mRecordData.Substring(DataInfo.RecoedLen - 1) = vbLf Then
                        nRet = -1
                    End If
                End If

                If nRet >= 0 Then
                    Return nRet
                End If

            End If

            ' �K�蕶���`�F�b�N�ʃ��\�b�h��
            Dim methodname As String = ""
            If DataKubun = "�w�b�_" Then
                methodname = mChkHeaderRegularStrMethod
            ElseIf DataKubun = "�f�[�^" Then
                methodname = mChkDataRegularStrMethod
            ElseIf DataKubun = "�g���[��" Then
                methodname = mChkTrailerRegularStrMethod
            Else
                methodname = mChkEndRegularStrMethod
            End If

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v,�u����/�ʃN���X���v����`����Ă��܂���B")
                End If

                '----------------------------------------------------------------
                ' �K�蕶���`�F�b�N�ʃ��\�b�h�ďo��
                '----------------------------------------------------------------
                Try
                    If IS_LEVEL4 = True Then
                        BLOG.Write_LEVEL4("CFormatXML.CheckRegularStringInternal", "�K�蕶���ʃ��\�b�h�ďo��", methodname)
                    End If

                    Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                    If methodInfo Is Nothing Then
                        Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�K�蕶���`�F�b�N���\�b�h�v�^�O�Ŏw�肳�ꂽ" & methodname & "��������܂���B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                    End If

                    Dim methodParams() As Object = {mRecordData}
                    nRet = CLng(methodInfo.Invoke(mClassInstance, methodParams))
                    If nRet >= 0 Then
                        BLOG.Write_Err("CFormatXML.CheckRegularStringInternal", "�K�蕶���ʃ��\�b�h�G���[", methodname)
                        Return nRet
                    End If
                Catch ex As Exception
                    BLOG.Write_Err("CFormatXML.CheckRegularStringInternal", ex)
                    Throw
                End Try
            End If

            Return nRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.CheckRegularStringInternal", "���A�l=" & nRet)
            End If

        End Try

    End Function


    '
    ' �@�\�@ �F �w�b�_���R�[�h�`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_
    '           "ERR" - �G���[����
    ' ���l�@ �F
    '
    Private Function CheckDBRecord1() As String

        Dim sRet As String = ""
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.CheckDBRecord1")
            End If

            ' ���N���X�̃w�b�_���R�[�h�`�F�b�N
            If MyBase.CheckHeaderRecord() = False Then
                sRet = "ERR"
                Return sRet
            End If

            sRet = "H"
            Return sRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.CheckDBRecord1", "���A�l=" & sRet)
            End If

        End Try

    End Function


    '
    ' �@�\�@ �F �f�[�^���R�[�h�`�F�b�N
    '
    ' �߂�l �F "D" - �f�[�^
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[
    ' ���l�@ �F
    '
    Private Function CheckDBRecord2() As String

        Dim sRet As String = ""
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.CheckDBRecord2")
            End If

            Dim CheckRet As Boolean
            Dim sWork As String

            ' ���N���X�̃f�[�^���R�[�h�`�F�b�N
            CheckRet = MyBase.CheckDataRecord()

            ' �J�i�E�v�ݒ�
            InfoMeisaiMast.NTEKIYO = ""
            InfoMeisaiMast.KTEKIYO = ""

            If (Not mInfoComm Is Nothing) Then
                ' ���R�[�h�f�[�^���f�[�^�z��ɕϊ�
                Dim SplitedRecordData() As String = SplitRecordData("�f�[�^")

                Dim tekiyou_kbn As String = mInfoComm.INFOToriMast.TEKIYOU_KBN_T
                Select Case tekiyou_kbn
                    Case "0"
                        InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                        InfoMeisaiMast.NTEKIYO = ""
                    Case "1"
                        InfoMeisaiMast.KTEKIYO = ""
                        InfoMeisaiMast.NTEKIYO = mInfoComm.INFOToriMast.NTEKIYOU_T
                    Case Else
                        If IS_LEVEL4 = True Then
                            BLOG.Write_LEVEL4("CFormatXML.CheckDBRecord2", "�J�i�E�v�ݒ�", "�E�v�敪=" & tekiyou_kbn)
                        End If

                        Dim element As XmlElement = Nothing
                        For Each element In mKanaTekiList
                            sWork = Element.GetAttribute("�E�v�敪")
                            If sWork Is Nothing OrElse sWork.Trim = "" Then
                                Throw New Exception(mXmlFile & "��`�G���[�F�u��������/�J�i�E�v�ݒ�v�^�O�́u�E�v�敪�v��������`����Ă��܂���B�i" & _
                                                    ConfigurationErrorsException.GetLineNumber(element) & "�s�ځj")
                            End If
                            sWork = sWork.Trim
                            If IsNumeric(sWork) = False Then
                                Throw New Exception(mXmlFile & "��`�G���[�F�u��������/�J�i�E�v�ݒ�v�^�O�́u�E�v�敪�v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(element) & "�s�ځj")
                            End If

                            ' �Y��"�E�v�敪�̏ꍇ
                            IF sWork = tekiyou_kbn Then
                                Exit For
                            End If

                            element = Nothing
                        Next

                        If element Is Nothing Then
                            Throw New Exception(mXmlFile & "��`�G���[�F�u��������/�J�i�E�v�ݒ�v�^�O�́u�E�v�敪�v�����ɊY������l�i" & tekiyou_kbn & "�j����`����Ă��܂���B")
                        End If

                        Dim datano As Integer = -1
                        Dim datalen As Integer = 0

                        sWork = Element.GetAttribute("�f�[�^�ԍ�")
                        If Not sWork Is Nothing AndAlso sWork.Trim <> "" Then
                            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������/�J�i�E�v�ݒ�v�^�O�́u�f�[�^�ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                                    ConfigurationErrorsException.GetLineNumber(element) & "�s�ځj")
                            End If
                            datano = CInt(sWork) - 1
                        End If

                        sWork = Element.GetAttribute("�f�[�^��")
                        If Not sWork Is Nothing AndAlso sWork.Trim <> "" Then
                            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData(datano).Length Then
                                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u��������/�J�i�E�v�ݒ�v�^�O�́u�f�[�^���v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                                    ConfigurationErrorsException.GetLineNumber(element) & "�s�ځj")
                            End If
                            datalen = CInt(sWork)
                        End If

                        If datano <> -1 Then
                            If datalen > 0 Then
                                InfoMeisaiMast.KTEKIYO = SplitedRecordData(datano).PadRight(datalen, " "c).Substring(0, datalen).Trim
                            Else
                                InfoMeisaiMast.KTEKIYO = SplitedRecordData(datano)
                            End If
                        End If
                End Select
            End If

            If CheckRet = False Then
                sRet = "IJO"
                Return sRet
            End If

            sRet = "D"
            Return sRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.CheckDBRecord2", "���A�l=" & sRet)
            End If

        End Try

    End Function


    '
    ' �@�\�@ �F �g���[���[���R�[�h�`�F�b�N
    '
    ' �߂�l �F "T" - �g���[��
    '           "ERR" - �G���[����
    ' ���l�@ �F
    '
    Private Function CheckDBRecord8() As String

        Dim sRet As String = ""
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.CheckDBRecord8")
            End If

            ' ���N���X�̃g���[�����R�[�h�`�F�b�N
            If MyBase.CheckTrailerRecord() = False Then
                sRet = "ERR"
                Return sRet
            End If

            sRet = "T"
            Return sRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.CheckDBRecord8", "���A�l=" & sRet)
            End If

        End Try

    End Function


    ' �@�\�@ �F ��`�ɏ]�����R�[�h�f�[�^�𕪊����ĕ�����z����쐬
    '
    ' ����   �F DataKubun - �f�[�^�敪
    '
    ' �߂�l �F �������ꂽ���R�[�h�f�[�^
    '
    ' ���l�@ �F
    '
    Private Function SplitRecordData(ByVal DataKubun As String) As String()

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.SplitRecordData")
            End If

            Dim node As XmlNode
            Dim sWork As String
            Dim dataLength As Integer = 0

            Dim list As List(Of XmlNode) = Nothing
            Dim count As Integer
            If DataKubun = "�w�b�_" Then
                list = mHeaderList
                count = mHeaderCount
            ElseIf DataKubun = "�f�[�^" Then
                list = mDataList
                count = mDataCount
            ElseIf DataKubun = "�g���[��" Then
                list = mTrailerList
                count = mTrailerCount
            Else
                list = mEndList
                count = mEndCount
            End If

            Dim SplitedRecordData(count - 1) As String

            ' ���R�[�h�f�[�^���f�[�^�z��ɕϊ�
            Dim TmpData As String = String.Copy(RecordData)
            For i As Integer = 1 To count
                If list.Count < count Then
                    ' ����/�t�H�[�}�b�g[@�f�[�^�敪='']/�f�[�^�ꗗ/�f�[�^[@�ԍ�='']"�i�K�{�j
                    node = mXmlRoot.SelectSingleNode("����/�t�H�[�}�b�g[@�f�[�^�敪='" & DataKubun & "']/�f�[�^�ꗗ/�f�[�^[@�ԍ�='" & i & "']")
                    If node Is Nothing Then
                        Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g[@�f�[�^�敪='" & DataKubun & "']/�f�[�^�ꗗ/�f�[�^[@�ԍ�='" & i & "']�v�^�O����`����Ă��܂���B")
                    End If

                    list.Add(node)
                Else
                    node = list.Item(i - 1)
                End IF

                ' �f�[�^���̐ݒ�l�`�F�b�N�i�K�{�j
                Dim attribute As XmlAttribute = node.Attributes.ItemOf("�f�[�^��")
                If attribute Is Nothing 
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�f�[�^�ꗗ/�f�[�^�v�^�O�́u�f�[�^���v��������`����Ă��܂���B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If
                sWork = attribute.Value.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�f�[�^�ꗗ/�f�[�^�v�^�O�́u�f�[�^���v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If

                ' �f�[�^���̍��v�����R�[�h�f�[�^���𒴂���ꍇ
                dataLength += CInt(sWork)
                If dataLength > DataInfo.RecoedLen Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�f�[�^�ꗗ/�f�[�^�v�^�O�́u�f�[�^���v�����̒l�i" & sWork & "�j�ł͍��v�����R�[�h�f�[�^���𒴂��܂��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End If

                SplitedRecordData(i - 1) = CuttingData(TmpData, CInt(sWork))
            Next

            ' ���R�[�h�l�␳�ʃ��\�b�h
            Dim methodname As String = ""
            If DataKubun = "�w�b�_" Then
                methodname = mChgHeaderRecMethod
            ElseIf DataKubun = "�f�[�^" Then
                methodname = mChgDataRecMethod
            ElseIf DataKubun = "�g���[��" Then
                methodname = mChgTrailerRecMethod
            Else
                methodname = mChgEndRecMethod
            End If
            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v,�u����/�ʃN���X���v����`����Ă��܂���B")
                End If

                '-------------------------------------
                ' �l��␳����ʃ��\�b�h�ďo��
                '-------------------------------------
                Try
                    If IS_LEVEL4 = True Then
                        BLOG.Write_LEVEL4("CFormatXML.SplitRecordData", "�l��␳����ʃ��\�b�h�ďo��", methodname)
                    End If

                    Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                    If methodInfo Is Nothing Then
                        Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�t�H�[�}�b�g/�l�␳���\�b�h�v�^�O�Ŏw�肳�ꂽ" & methodname & "��������܂���B")
                    End If

                    Dim methodParams() As Object = {SplitedRecordData}
                    If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                        Throw New Exception("�l��␳����ʃ��\�b�h�G���[�i���\�b�h���F" & methodname & "�j")
                    End If
                Catch ex As Exception
                    BLOG.Write_Err("CFormatXML.SplitRecordData", ex)
                    Throw
                End Try
            End If

            Return SplitedRecordData

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.SplitRecordData")
            End If
        End Try

    End Function


    ' �@�\�@ �F ��`�ɏ]�����׃f�[�^�ɒl��ݒ肷��
    '
    ' ����   �F type ���׃f�[�^��ʁiMEISAI_KIND_1: ���׃f�[�^�ꗗ,  MEISAI_KIND_2: ���[���ڈꗗ�j
    '        �F nodeList ���׃f�[�^���`����XML�m�[�h���X�g
    '        �F SplitedRecordData �������ꂽ���R�[�h�f�[�^
    '
    Private Sub SetMeisaiInfo(ByVal type As Integer, ByRef nodeList As XmlNodeList, ByRef SplitedRecordData() As String)

        Dim Column As String = ""
        Dim sValue As String = ""
        Dim sValue_org As String = ""
        Dim element As XmlElement = Nothing
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.SetMeisaiInfo")
            End If

            Dim sTagName As String
            Dim sWork As String
            Dim iValue As Integer

            Dim InfoType As Type
            Dim Target As Object
            Dim valueType As ValueType
            Dim fieldInfo As FieldInfo


            If type = MEISAI_KIND_1 Then
                sTagName = "�u��������/���׃f�[�^�ꗗ/���׃f�[�^�v"
            Else
                sTagName = "�u��������/���[���ڈꗗ/���׃f�[�^�v"
            End If

            Dim no As Integer = 0
            For Each element In nodeList
                Column = ""
                sValue = ""
                sValue_org = ""

                ' ���׃f�[�^�̔ԍ��i�K�{�j
                no += 1
                sWork = Element.GetAttribute("�ԍ�")
                If sWork Is Nothing OrElse sWork.Trim <> CStr(no) Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u�ԍ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                End If

                ' ���׃f�[�^�̗񖼁i�K�{�j
                sWork = Element.GetAttribute("��")
                If sWork Is Nothing OrElse sWork.Trim = "" Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u�񖼁v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                End If
                Column = sWork.Trim

                ' �Œ�l�ݒ�i�K�{�j
                sWork = Element.GetAttribute("�Œ�l�ݒ�")
                If sWork Is Nothing OrElse sWork.Trim = "" Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u�Œ�l�ݒ�v��������`����Ă��܂���B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                End If
                sWork = sWork.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 1 Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u�Œ�l�ݒ�v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                End If
                iValue = CInt(sWork)

                ' �l�i�K�{�j
                sWork = Element.GetAttribute("�l")
                If sWork Is Nothing OrElse sWork.Trim = "" Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u�l�v��������`����Ă��܂���B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                End If
                sWork = sWork.Trim

                ' 1�s�f�[�^���̒l��ݒ肷��ꍇ
                If iValue = 0 Then
                    Dim Indexes() As String = sWork.Split(","c)
                    For Each index As String In Indexes
                        If Not IsNumeric(index) OrElse CInt(index) <= 0 OrElse CInt(index) > SplitedRecordData.Length Then
                            Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u�l�v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                                ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                        End If

                        sValue = sValue & SplitedRecordData(CInt(index) - 1)
                    Next
               ' �Œ�l�̏ꍇ
                Else
                    sValue = sWork
                End If

                sValue_org = sValue
                Dim ValueLength As Integer = sValue.Length

                ' �󔒕����̍폜�i�K�{�j
                sWork = Element.GetAttribute("�󔒕����폜")
                If sWork Is Nothing OrElse sWork.Trim = "" Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u�󔒕����폜�v��������`����Ă��܂���B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                End If
                sWork = sWork.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 3 Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u�󔒕����폜�v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                End If
                iValue = CInt(sWork)

                '�O��̋󔒕������폜����ꍇ
                If iValue = 1 Then
                    sValue = sValue.Trim
                '�擪�̋󔒕������폜����ꍇ
                ElseIf iValue = 2 Then
                    sValue = sValue.TrimStart
                '�����̋󔒕������폜����ꍇ
                ElseIf iValue = 3 Then
                    sValue = sValue.TrimEnd
                End If

                ' �g�p�����i�l�ȗ��j
                Dim paddingChar As Char
                sWork = Element.GetAttribute("�g�p����")
                If Not sWork Is Nothing AndAlso sWork <> "" Then
                    If System.Text.Encoding.GetEncoding(932).GetByteCount(sWork) <> 1 Then
                        Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u�g�p�����v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                            ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                    End If
                    paddingChar = CChar(sWork)
                Else
                    paddingChar = " "c  ' �f�t�H���g�F ���p��
                End If

                ' �������߁i�K�{�j
                sWork = Element.GetAttribute("��������")
                If sWork Is Nothing OrElse sWork.Trim = "" Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u�������߁v��������`����Ă��܂���B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                End If
                sWork = sWork.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 2 Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u�������߁v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                End If
                iValue = CInt(sWork)

                '�擪�𕶎����߂���ꍇ
                If iValue = 1 Then
                    sValue = sValue.PadLeft(ValueLength, paddingChar)
                '�����𕶎����߂���ꍇ
                ElseIf iValue = 2 Then
                    sValue = sValue.PadRight(ValueLength, paddingChar)
                End If

                ' ���t�����i�l�ȗ��j
                Dim fmt As String
                sWork = Element.GetAttribute("���t����")
                If Not sWork Is Nothing AndAlso sWork.Trim <> "" Then
                    fmt = sWork.Trim
                Else
                    fmt = "yyyyMMdd"  ' �f�t�H���g
                End If

                ' ���t�ϊ��i�K�{�j
                sWork = Element.GetAttribute("���t�ϊ�")
                If sWork Is Nothing OrElse sWork.Trim = "" Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u���t�ϊ��v��������`����Ă��܂���B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                End If
                sWork = sWork.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 1 Then
                    Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u���t�ϊ��v�����̒l�i" & sWork & "�j���s���ł��B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                End If
                iValue = CInt(sWork)

                ' ���t�ϊ�����ꍇ
                If iValue = 1 Then
                    sValue = CASTCommon.ConvertDate(sValue, fmt)
                End If

                Try
                    If type = MEISAI_KIND_1 Then
                        '���׃f�[�^�̐ݒ�Ώۂ̃����o�ϐ����擾
                        InfoType = GetType(MEISAI)
                        Target = InfoType.InvokeMember(Column, _
                            BindingFlags.Public Or BindingFlags.NonPublic Or _
                            BindingFlags.Instance Or BindingFlags.GetField, _
                            Nothing, _
                            InfoMeisaiMast, _
                            Nothing)

                        '�����o�ϐ��֒l��ݒ�
                        valueType = InfoMeisaiMast
                        fieldInfo = InfoMeisaiMast.GetType().GetField(Column)
                        If TypeOf Target Is Integer Then
                            '�ݒ�l��Integer�̏ꍇ
                            fieldInfo.SetValue(valueType, CInt(sValue))
                        ElseIf TypeOf Target Is Decimal Then
                            '�ݒ�l��Decimal�̏ꍇ
                            fieldInfo.SetValue(valueType, CDec(sValue))
                        Else
                            '�ݒ�l��String�̏ꍇ
                            fieldInfo.SetValue(valueType, sValue)
                        End If

                        InfoMeisaiMast = DirectCast(valueType, MEISAI)

                    Else
                        '���[�o�͍��ڂ̐ݒ�Ώۂ̃����o�ϐ����擾
                        InfoType = GetType(MEISAI2)
                        Target = InfoType.InvokeMember(Column, _
                            BindingFlags.Public Or BindingFlags.NonPublic Or _
                            BindingFlags.Instance Or BindingFlags.GetField, _
                            Nothing, _
                            InfoMeisaiMast2, _
                            Nothing)

                        '�����o�ϐ��֒l��ݒ�
                        valueType = InfoMeisaiMast2
                        fieldInfo = InfoMeisaiMast2.GetType().GetField(Column)
                        If TypeOf Target Is Integer Then
                            '�ݒ�l��Integer�̏ꍇ
                            fieldInfo.SetValue(valueType, CInt(sValue))
                        ElseIf TypeOf Target Is Decimal Then
                            '�ݒ�l��Decimal�̏ꍇ
                            fieldInfo.SetValue(valueType, CDec(sValue))
                        Else
                            '�ݒ�l��String�̏ꍇ
                            fieldInfo.SetValue(valueType, sValue)
                        End If

                        InfoMeisaiMast2 = DirectCast(valueType, MEISAI2)
                    End If
                Catch ex As Exception
                    BLOG.Write_Err("CFormatXML.SetMeisaiInfo", ex)

                    Throw New Exception(mXmlFile & "��`�G���[�F" & sTagName & "�^�O�́u�񖼁v�����̒l�i" & Column & "�j�Ŏw�肳�ꂽ���׃f�[�^�ւ̐ݒ�Ɏ��s���܂����B�i" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "�s�ځj")
                End Try
            Next

        Catch ex As Exception
            If Not Element Is Nothing AndAlso IS_LEVEL4 Then
                BLOG.Write_LEVEL4("CFormatXML.SetMeisaiInfo", "�G���[XML�m�[�h", ConfigurationErrorsException.GetLineNumber(Element) & "�s��")
                BLOG.Write_LEVEL4("CFormatXML.SetMeisaiInfo", "�G���[��", Column)
                BLOG.Write_LEVEL4("CFormatXML.SetMeisaiInfo", "�G���[�ݒ�l�i�␳�O�j", sValue_org)
                BLOG.Write_LEVEL4("CFormatXML.SetMeisaiInfo", "�G���[�ݒ�l", sValue)
            End If

            Throw

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.SetMeisaiInfo")
            End If

        End Try

    End Sub


    '
    ' �@�\�@ �F ���O�̃��R�[�h�敪���w�b�_���R�[�h������
    ' �߂�l �F True - �w�b�_���R�[�h
    '
    Private Function IsBeforeHeaderRecord() As Boolean

        Dim bRet As Boolean = False
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.IsBeforeHeaderRecord")
            End If

            For i As Integer = 0 To HeaderKubun.Length - 1
                If BeforeRecKbn = HeaderKubun(i) Then
                    bRet = True
                    Return bRet
                End If
            Next i

            Return bRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.IsBeforeHeaderRecord", "���A�l=" & bRet)
            End If

        End Try

    End Function

    '
    ' �@�\�@ �F ���O�̃��R�[�h�敪���f�[�^���R�[�h������
    ' �߂�l �F True - �f�[�^���R�[�h
    '
    Private Function IsBeforeDataRecord() As Boolean

        Dim bRet As Boolean = False
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.IsBeforeDataRecord")
            End If

            For i As Integer = 0 To DataKubun.Length - 1
                If BeforeRecKbn = DataKubun(i) Then
                    bRet = True
                    Return bRet
                End If
            Next i

            Return bRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.IsBeforeDataRecord", "���A�l=" & bRet)
            End If

        End Try

    End Function

    '
    ' �@�\�@ �F ���O�̃��R�[�h�敪���g���[�����R�[�h������
    ' �߂�l �F True - �g���[���[���R�[�h
    '
    Private Function IsBeforeTrailerRecord() As Boolean

        Dim bRet As Boolean = False
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.IsBeforeTrailerRecord")
            End If

            For i As Integer = 0 To TrailerKubun.Length - 1
                If BeforeRecKbn = TrailerKubun(i) Then
                    bRet = True
                    Return bRet
                End If
            Next i

            Return bRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.IsBeforeTrailerRecord", "���A�l=" & bRet)
            End If

        End Try

    End Function

    '
    ' �@�\�@ �F ���O�̃��R�[�h�敪���G���h���R�[�h������
    ' �߂�l �F True - �G���h���R�[�h
    '
    Private Function IsBeforeEndRecord() As Boolean

        Dim bRet As Boolean = False
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.IsBeforeEndRecord")
            End If

            If BeforeRecKbn = EndKubun Then
                bRet = True
                Return bRet
            End If

            Return bRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.IsBeforeEndRecord", "���A�l=" & bRet)
            End If

        End Try

    End Function

End Class
'*** End Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή��i�V�K�쐬�j ***
