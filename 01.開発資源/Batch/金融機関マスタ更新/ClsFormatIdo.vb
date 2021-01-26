Imports System
Imports System.Collections

' 金融機関支店異動通知データ フォーマット
Public Class ClsFormatIdo

    ' 固定長構造体用インターフェース
    Protected Interface IFormat
        ' データ
        Sub Init()
        WriteOnly Property Data() As Byte()
    End Interface

    ' SHIT-JISエンコーディング
    Protected Shared EncdSJIS As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")

#Region "金融機関支店異動通知データ 金融機関データ部"
    '　金融機関支店異動通知データ 金融機関データ部
    Public Structure KinkoFormat
        Implements ClsFormatIdo.IFormat

        Dim DataKubun As String         ' データ区分
        Dim DataSyubetu As String       ' データ種別
        Dim KinCode As String           ' 金融機関コード
        Dim KinFukaCode As String       ' 金融機関付加コード
        Dim DaiKinCode As String        ' 代表金融機関コード
        Dim SeiKinKana As String        ' 正読金融機関名（カナ）
        Dim SeiKinKanji As String       ' 正読金融機関名（漢字）
        Dim RyakuKinKana As String      ' 略称金融機関名（カナ）
        Dim RyakuKinKanji As String     ' 略称金融機関名（漢字）
        Dim JISIN() As String           ' 地震強化地域内金融機関表示
        Dim IdoDate As String           ' 異動年月日
        Dim IdoJiyuCode As String       ' 異動事由コード
        Dim NewKinCode As String        ' 新金融機関コード
        Dim NewKinFukaCode As String    ' 新金融機関付加コード
        Dim DeleteDate As String        ' 削除日
        Dim Dummy As String             ' ダミー

        Public Sub Init() Implements IFormat.Init
            DataKubun = ""
            DataSyubetu = ""
            KinCode = ""
            KinFukaCode = ""
            DaiKinCode = ""
            SeiKinKana = ""
            SeiKinKanji = ""
            RyakuKinKana = ""
            RyakuKinKanji = ""
            IdoDate = ""
            IdoJiyuCode = ""
            NewKinCode = ""
            NewKinFukaCode = ""
            DeleteDate = ""
            Dummy = ""

            JISIN = New String(9 - 1) {}

        End Sub

        '固定長データ処理用プロパティ
        Public WriteOnly Property Data() As Byte() Implements IFormat.Data
            Set(ByVal value() As Byte)
                DataKubun = EncdSJIS.GetString(value, 0, 1)
                DataSyubetu = EncdSJIS.GetString(value, 1, 1)
                KinCode = EncdSJIS.GetString(value, 2, 4)
                KinFukaCode = EncdSJIS.GetString(value, 6, 1)
                DaiKinCode = EncdSJIS.GetString(value, 7, 4)
                SeiKinKana = EncdSJIS.GetString(value, 11, 15)

                SeiKinKanji = EncdSJIS.GetString(value, 26, 30)

                RyakuKinKana = EncdSJIS.GetString(value, 56, 15)

                RyakuKinKanji = EncdSJIS.GetString(value, 71, 30)

                For i As Integer = 0 To JISIN.Length - 1
                    JISIN(i) = EncdSJIS.GetString(value, 101 + i, 1)
                Next i
                IdoDate = EncdSJIS.GetString(value, 110, 8)
                IdoJiyuCode = EncdSJIS.GetString(value, 118, 2)
                NewKinCode = EncdSJIS.GetString(value, 120, 4)
                NewKinFukaCode = EncdSJIS.GetString(value, 124, 1)
                DeleteDate = EncdSJIS.GetString(value, 125, 8)
                Dummy = EncdSJIS.GetString(value, 133, 247)
            End Set
        End Property
    End Structure
#End Region

#Region "金融機関支店異動通知データ 店舗データ部"
    '　金融機関支店異動通知データ 店舗データ部
    Public Structure TenpoFormat
        Implements ClsFormatIdo.IFormat

        Dim DataKubun As String         ' データ区分
        Dim DataSyubetu As String       ' データ種別
        Dim KinCode As String           ' 金融機関コード
        Dim KinFukaCode As String       ' 金融機関付加コード
        Dim TenCode As String           ' 店舗コード
        Dim TenFukaCode As String       ' 店舗付加コード
        Dim TenKana As String           ' 店舗名（カナ）
        Dim TenKanji As String          ' 店舗名（漢字）
        Dim SeiHyouji As String         ' 正読店名表示
        Dim Yubin As String             ' 郵便番号
        Dim TenSyozaiKana As String     ' 店舗所在地（カナ）
        Dim TenSyozaiKanji As String    ' 店舗所在地（漢字）
        Dim TegataKoukan As String      ' 手形交換所番号
        Dim TelNo As String             ' 電話番号
        Dim TenZokusei As String        ' 店舗属性表示
        Dim JikoCenter As String        ' 自行センタ表示
        Dim FuriCenter As String        ' 振込センタ表示
        Dim SyuCenter As String         ' 集手センタ表示
        Dim KawaseCenter As String      ' 為替センタ表示
        Dim Daitegai As String          ' 代手対象外表示
        Dim JisinHyoji As String        ' 地震強化地域内店舗表示
        Dim JCBango As String           ' ＪＣ番号
        Dim IdoDate As String           ' 異動年月日
        Dim IdoJiyuCode As String       ' 異動事由コード
        Dim NewKinCode As String        ' 新金融機関コード
        Dim NewKinFukaCode As String    ' 新金融機関付加コード
        Dim NewTenCode As String        ' 新店舗コード
        Dim NewTenFukaCode As String    ' 新店舗付加コード
        Dim DeleteDate As String        ' 削除日
        Dim TegataKoukanKanji As String ' 所属手形交換所名（漢字）
        Dim Dummy As String             ' ダミー

        Public Sub Init() Implements IFormat.Init
            DataKubun = ""
            DataSyubetu = ""
            KinCode = ""
            KinFukaCode = ""
            TenCode = ""
            TenFukaCode = ""
            TenKana = ""
            TenKanji = ""
            SeiHyouji = ""
            Yubin = ""
            TenSyozaiKana = ""
            TenSyozaiKanji = ""
            TegataKoukan = ""
            TelNo = ""
            TenZokusei = ""
            JikoCenter = ""
            FuriCenter = ""
            SyuCenter = ""
            KawaseCenter = ""
            Daitegai = ""
            JisinHyoji = ""
            JCBango = ""
            IdoDate = ""
            IdoJiyuCode = ""
            NewKinCode = ""
            NewKinFukaCode = ""
            NewTenCode = ""
            NewTenFukaCode = ""
            DeleteDate = ""
            TegataKoukanKanji = ""
            Dummy = ""
        End Sub

        '固定長データ処理用プロパティ
        Public WriteOnly Property Data() As Byte() Implements IFormat.Data
            Set(ByVal value() As Byte)
                DataKubun = EncdSJIS.GetString(value, 0, 1)
                DataSyubetu = EncdSJIS.GetString(value, 1, 1)
                KinCode = EncdSJIS.GetString(value, 2, 4)
                KinFukaCode = EncdSJIS.GetString(value, 6, 1)
                TenCode = EncdSJIS.GetString(value, 7, 3)
                TenFukaCode = EncdSJIS.GetString(value, 10, 2)
                TenKana = EncdSJIS.GetString(value, 12, 15)

                TenKanji = EncdSJIS.GetString(value, 27, 30)

                SeiHyouji = EncdSJIS.GetString(value, 57, 1)
                Yubin = EncdSJIS.GetString(value, 58, 10)
                TenSyozaiKana = EncdSJIS.GetString(value, 68, 80)

                TenSyozaiKanji = EncdSJIS.GetString(value, 148, 110)

                TegataKoukan = EncdSJIS.GetString(value, 258, 4)
                TelNo = EncdSJIS.GetString(value, 262, 17)
                TenZokusei = EncdSJIS.GetString(value, 279, 1)
                JikoCenter = EncdSJIS.GetString(value, 280, 1)
                FuriCenter = EncdSJIS.GetString(value, 281, 1)
                SyuCenter = EncdSJIS.GetString(value, 282, 1)
                KawaseCenter = EncdSJIS.GetString(value, 283, 1)
                Daitegai = EncdSJIS.GetString(value, 284, 1)
                JisinHyoji = EncdSJIS.GetString(value, 285, 1)
                JCBango = EncdSJIS.GetString(value, 286, 1)
                IdoDate = EncdSJIS.GetString(value, 287, 8)
                IdoJiyuCode = EncdSJIS.GetString(value, 295, 2)
                NewKinCode = EncdSJIS.GetString(value, 297, 4)
                NewKinFukaCode = EncdSJIS.GetString(value, 301, 1)
                NewTenCode = EncdSJIS.GetString(value, 302, 3)
                NewTenFukaCode = EncdSJIS.GetString(value, 305, 2)
                DeleteDate = EncdSJIS.GetString(value, 307, 8)

                TegataKoukanKanji = EncdSJIS.GetString(value, 315, 20)

                Dummy = EncdSJIS.GetString(value, 335, 45)
            End Set
        End Property
    End Structure
#End Region
End Class
