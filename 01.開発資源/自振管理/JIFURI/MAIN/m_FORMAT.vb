Module m_FORMAT
    '------------------------------------------
    '全銀フォーマット
    '------------------------------------------
    'ヘッダレコード
    Structure gstrZG_Record1
        <VBFixedString(1)> Public ZG1 As String    'データ区分(=1)
        <VBFixedString(2)> Public ZG2 As String    '種別コード
        <VBFixedString(1)> Public ZG3 As String    'コード区分
        <VBFixedString(10)> Public ZG4 As String   '振込依頼人コード
        <VBFixedString(40)> Public ZG5 As String   '振込依頼人名
        <VBFixedString(4)> Public ZG6 As String    '取扱日
        <VBFixedString(4)> Public ZG7 As String    '仕向銀行ｺｰﾄﾞ
        <VBFixedString(15)> Public ZG8 As String   '仕向銀行名
        <VBFixedString(3)> Public ZG9 As String    '仕向支店ｺｰﾄﾞ
        <VBFixedString(15)> Public ZG10 As String  '仕向支店名
        <VBFixedString(1)> Public ZG11 As String   '預金種目
        <VBFixedString(7)> Public ZG12 As String   '口座番号
        <VBFixedString(17)> Public ZG13 As String  'ダミー
    End Structure
    Public gZENGIN_REC1 As gstrZG_Record1
    'データレコード
    Structure gstrZG_Record2
        <VBFixedString(1)> Public ZG1 As String    'データ区分(=2)
        <VBFixedString(4)> Public ZG2 As String    '被仕向銀行番号
        <VBFixedString(15)> Public ZG3 As String   '被仕向銀行名　
        <VBFixedString(3)> Public ZG4 As String    '被仕向支店番号
        <VBFixedString(15)> Public ZG5 As String   '被仕向支店名
        <VBFixedString(4)> Public ZG6 As String    '手形交換所番号
        <VBFixedString(1)> Public ZG7 As String    '預金種目
        <VBFixedString(7)> Public ZG8 As String    '口座番号
        <VBFixedString(30)> Public ZG9 As String   '受取人
        <VBFixedString(10)> Public ZG10 As String  '振込金額
        <VBFixedString(1)> Public ZG11 As String   '新規コード
        <VBFixedString(10)> Public ZG12 As String  '顧客コード１
        <VBFixedString(10)> Public ZG13 As String  '顧客コード２
        <VBFixedString(1)> Public ZG14 As String   '振込指定区分
        <VBFixedString(8)> Public ZG15 As String   'ダミー
    End Structure
    Public gZENGIN_REC2 As gstrZG_Record2
    'トレーラレコード
    Structure gstrZG_Record8
        <VBFixedString(1)> Public ZG1 As String    'データ区分(=8)
        <VBFixedString(6)> Public ZG2 As String    '合計件数
        <VBFixedString(12)> Public ZG3 As String   '合計金額
        <VBFixedString(101)> Public ZG4 As String  'ダミー
    End Structure
    Public gZENGIN_REC8 As gstrZG_Record8
    'エンドレコード
    Structure gstrZG_Record9
        <VBFixedString(1)> Public ZG1 As String    'データ区分(=9)
        <VBFixedString(119)> Public ZG2 As String  'ダミー数
    End Structure
    Public gZENGIN_REC9 As gstrZG_Record9
    Structure gstrFURI_DATA
        <VBFixedString(120)> Public strDATA As String
    End Structure
    '120バイト格納用
    Public gstrDATA As gstrFURI_DATA



    '------------------------------------------
    '地公体フォーマット
    '------------------------------------------
    'ヘッダーレコード
    Structure gstrZEIKIN_Record1
        <VBFixedString(1)> Public ZK1 As String    'データ区分(=1)
        <VBFixedString(2)> Public ZK2 As String    '種別コード
        <VBFixedString(1)> Public ZK3 As String    'コード区分
        <VBFixedString(10)> Public ZK4 As String   '委託者コード
        <VBFixedString(40)> Public ZK5 As String   '委託者名
        <VBFixedString(4)> Public ZK6 As String    '振替日
        <VBFixedString(4)> Public ZK7 As String    '取引銀行番号
        <VBFixedString(15)> Public ZK8 As String   '取引銀行名
        <VBFixedString(3)> Public ZK9 As String    '取引支店番号
        <VBFixedString(15)> Public ZK10 As String  '取引支店名
        <VBFixedString(1)> Public ZK11 As String   '預金種目
        <VBFixedString(7)> Public ZK12 As String   '口座番号
        <VBFixedString(17)> Public ZK13 As String  'ダミー
        <VBFixedString(20)> Public ZK14 As String   '収納種目
        <VBFixedString(80)> Public ZK15 As String  'ダミー
    End Structure
    Public gZEIKIN_REC1 As gstrZEIKIN_Record1

    'データレコード
    Structure gstrZEIKIN_Record2
        <VBFixedString(1)> Public ZK1 As String   'データ区分(=2)
        <VBFixedString(4)> Public ZK2 As String   '引落銀行番号
        <VBFixedString(15)> Public ZK3 As String  '引落銀行名
        <VBFixedString(3)> Public ZK4 As String   '引落支店番号
        <VBFixedString(15)> Public ZK5 As String  '引落支店名
        <VBFixedString(1)> Public ZK6 As String   '預金種目
        <VBFixedString(7)> Public ZK7 As String   '口座番号
        <VBFixedString(30)> Public ZK8 As String  '口座名義
        <VBFixedString(10)> Public ZK9 As String  '引落金額
        <VBFixedString(10)> Public ZK10 As String '保険料額(定)
        <VBFixedString(10)> Public ZK11 As String '保険料額(付)
        <VBFixedString(20)> Public ZK12 As String 'ダミー
        <VBFixedString(10)> Public ZK13 As String '前納報奨金
        <VBFixedString(1)> Public ZK14 As String  '新規コード
        <VBFixedString(20)> Public ZK15 As String '顧客番号
        <VBFixedString(30)> Public ZK16 As String '被保険者名
        <VBFixedString(1)> Public ZK17 As String  '振替結果コード
        <VBFixedString(10)> Public ZK18 As String '通帳略字
        <VBFixedString(2)> Public ZK19 As String  '年度
        <VBFixedString(20)> Public ZK20 As String 'ダミー
    End Structure
    Public gZEIKIN_REC2 As gstrZEIKIN_Record2

    'トレーラレコード
    Structure gstrZEIKIN_Record8
        <VBFixedString(1)> Public ZK1 As String   'データ区分(=8)
        <VBFixedString(6)> Public ZK2 As String   '合計件数
        <VBFixedString(12)> Public ZK3 As String  '合計金額
        <VBFixedString(6)> Public ZK4 As String   '振替済件数
        <VBFixedString(12)> Public ZK5 As String  '振替済金額
        <VBFixedString(6)> Public ZK6 As String   '振替不能件数
        <VBFixedString(12)> Public ZK7 As String  '振替不能金額
        <VBFixedString(165)> Public ZK8 As String 'ダミー
    End Structure
    Public gZEIKIN_REC8 As gstrZEIKIN_Record8

    Structure gstrZEIKIN_Record9
        <VBFixedString(1)> Public ZK1 As String   'データ区分(=9)
        <VBFixedString(219)> Public ZK2 As String   'ダミー
    End Structure
    Public gZEIKIN_REC9 As gstrZEIKIN_Record9


    Structure gstrFURI_DATA_220
        <VBFixedString(220)> Public strDATA As String
    End Structure
    '220バイト格納用
    Public gstrDATA_220 As gstrFURI_DATA_220



    '------------------------------------------
    '国税フォーマット
    '------------------------------------------
    'ファイル先頭レコード（Ａ）
    'Structure gstrKOKUZEI_Record1
    '    <VBFixedString(1)> Public KZ1 As String         'データ区分(=1)
    '    <VBFixedString(2)> Public KZ2 As String         'ファイル区分
    '    <VBFixedString(22)> Public KZ3 As String        'ダミー
    '    <VBFixedString(2)> Public KZ4 As String         '年度
    '    <VBFixedString(2)> Public KZ5 As String         '課税年分
    '    <VBFixedString(1)> Public KZ6 As String         '納期区分
    '    <VBFixedString(7)> Public KZ7 As String         '納期カナ文字
    '    <VBFixedString(2)> Public KZ8 As String         '徴定区分
    '    <VBFixedString(6)> Public KZ9 As String         '発送年月日
    '    <VBFixedString(6)> Public KZ10 As String        '振替日
    '    <VBFixedString(337)> Public KZ11 As String      'ダミー
    '    <VBFixedString(2)> Public KZ12 As String        '依頼ファイルＮＯ
    'End Structure
    Structure gstrKOKUZEI_Record1
        <VBFixedString(1)> Public KZ1 As String         'データ区分(=1)
        <VBFixedString(2)> Public KZ2 As String         'ファイル区分
        <VBFixedString(19)> Public KZ3 As String        'ダミー
        <VBFixedString(3)> Public KZ4 As String         '科目コード
        <VBFixedString(2)> Public KZ5 As String         '年度
        <VBFixedString(2)> Public KZ6 As String         '課税年分
        <VBFixedString(1)> Public KZ7 As String         '納期区分
        <VBFixedString(7)> Public KZ8 As String         '納期カナ文字
        <VBFixedString(2)> Public KZ9 As String         '徴定区分
        <VBFixedString(6)> Public KZ10 As String         '発送年月日
        <VBFixedString(6)> Public KZ11 As String        '振替日
        <VBFixedString(6)> Public KZ12 As String        '課税期間（自）
        <VBFixedString(6)> Public KZ13 As String        '課税期間（至）
        <VBFixedString(325)> Public KZ14 As String      'ダミー
        <VBFixedString(2)> Public KZ15 As String        '依頼ファイルＮＯ
    End Structure

    Public gKOKUZEI_REC1 As gstrKOKUZEI_Record1

    '署別金融機関店舗別名称レコード（Ｂ）
    '署別金融機関店舗別トータルレコード（Ｄ）
    '署別金融機関別トータルレコード（Ｅ）
    Structure gstrKOKUZEI_Record2
        <VBFixedString(1)> Public KZ1 As String         'データ区分(=2)
        <VBFixedString(2)> Public KZ2 As String         'ファイル区分
        <VBFixedString(5)> Public KZ3 As String         '局署番号
        <VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
        <VBFixedString(9)> Public KZ5 As String         'ダミー
        <VBFixedString(5)> Public KZ6 As String         '日銀コード
        <VBFixedString(10)> Public KZ7 As String        '税務署名
        <VBFixedString(7)> Public KZ8 As String         'ダミー
        <VBFixedString(5)> Public KZ9 As String         '税務署郵便番号
        <VBFixedString(7)> Public KZ10 As String        '取扱金融機関番号
        <VBFixedString(5)> Public KZ11 As String        '金融機関郵便番号
        <VBFixedString(7)> Public KZ12 As String        'ダミー
        <VBFixedString(6)> Public KZ13 As String        '送付分件数
        <VBFixedString(12)> Public KZ14 As String       '送付分合計金額
        <VBFixedString(6)> Public KZ15 As String        '振替納付不能件数
        <VBFixedString(12)> Public KZ16 As String       '振替納付不能合計
        <VBFixedString(6)> Public KZ17 As String        '振替納付件数
        <VBFixedString(12)> Public KZ18 As String       '振替納付合計金額
        <VBFixedString(5)> Public KZ19 As String        'ダミー
        <VBFixedString(8)> Public KZ20 As String        '税務署電話番号
        <VBFixedString(8)> Public KZ21 As String        '金融機関電話番
        <VBFixedString(27)> Public KZ22 As String       'ダミー
        <VBFixedString(30)> Public KZ23 As String       '都市区名
        <VBFixedString(30)> Public KZ24 As String       '所在地Ⅰ
        <VBFixedString(30)> Public KZ25 As String       '所在地Ⅱ
        <VBFixedString(30)> Public KZ26 As String       '肩書
        <VBFixedString(30)> Public KZ27 As String       '金融機関名称Ⅰ
        <VBFixedString(30)> Public KZ28 As String       '金融機関名称Ⅱ
        <VBFixedString(30)> Public KZ29 As String       '店舗名称
        <VBFixedString(1)> Public KZ30 As String        '補充記入
        <VBFixedString(5)> Public KZ31 As String        'ダミー
        <VBFixedString(2)> Public KZ32 As String        '依頼ファイルＮＯ
    End Structure
    Public gKOKUZEI_REC2 As gstrKOKUZEI_Record2
    Public gKOKUZEI_REC4 As gstrKOKUZEI_Record2
    Public gKOKUZEI_REC5 As gstrKOKUZEI_Record2


    '個別明細レコード（Ｃ）
    'Structure gstrKOKUZEI_Record3
    '    <VBFixedString(1)> Public KZ1 As String         'データ区分(=3)
    '    <VBFixedString(2)> Public KZ2 As String         'ファイル区分(=91)
    '    <VBFixedString(5)> Public KZ3 As String         '局署番号
    '    <VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
    '    <VBFixedString(7)> Public KZ5 As String         '納税者番号
    '    <VBFixedString(1)> Public KZ6 As String         '継承区分
    '    <VBFixedString(1)> Public KZ7 As String         '補完表示区分
    '    <VBFixedString(1)> Public KZ8 As String         '振替結果コード
    '    <VBFixedString(10)> Public KZ9 As String        '納付税額
    '    <VBFixedString(9)> Public KZ10 As String        '内利子税
    '    <VBFixedString(1)> Public KZ11 As String        '預金種別
    '    <VBFixedString(7)> Public KZ12 As String        '口座番号
    '    <VBFixedString(84)> Public KZ13 As String       'ダミー
    '    <VBFixedString(5)> Public KZ14 As String        '郵便番号
    '    <VBFixedString(1)> Public KZ15 As String        '補完表示
    '    <VBFixedString(7)> Public KZ16 As String        '取扱金融機関番号
    '    <VBFixedString(7)> Public KZ17 As String        'ダミー
    '    <VBFixedString(6)> Public KZ18 As String        '市外局番(納税者)
    '    <VBFixedString(8)> Public KZ19 As String        '電話番号(納税者)
    '    <VBFixedString(2)> Public KZ20 As String        'ダミー
    '    <VBFixedString(23)> Public KZ21 As String       '都市区分
    '    <VBFixedString(23)> Public KZ22 As String       '住所Ⅰ
    '    <VBFixedString(23)> Public KZ23 As String       '住所Ⅱ
    '    <VBFixedString(23)> Public KZ24 As String       '住所Ⅲ
    '    <VBFixedString(23)> Public KZ25 As String       '肩書Ⅰ
    '    <VBFixedString(23)> Public KZ26 As String       '肩書Ⅱ
    '    <VBFixedString(23)> Public KZ27 As String       '肩書Ⅲ
    '    <VBFixedString(23)> Public KZ28 As String       '納税者名Ⅰ
    '    <VBFixedString(23)> Public KZ29 As String       '納税者名Ⅱ
    '    <VBFixedString(5)> Public KZ30 As String        '納貯番号
    '    <VBFixedString(3)> Public KZ31 As String        '口座番号
    '    <VBFixedString(1)> Public KZ32 As String        '継続区分
    '    <VBFixedString(2)> Public KZ33 As String        '依頼ファイルＮＯ
    'End Structure
    Structure gstrKOKUZEI_Record3
        <VBFixedString(1)> Public KZ1 As String         'データ区分(=3)
        <VBFixedString(2)> Public KZ2 As String         'ファイル区分(=91)
        <VBFixedString(5)> Public KZ3 As String         '局署番号
        <VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
        <VBFixedString(7)> Public KZ5 As String         '納税者番号
        <VBFixedString(1)> Public KZ6 As String         '継承区分
        <VBFixedString(1)> Public KZ7 As String         '補完表示区分
        <VBFixedString(1)> Public KZ8 As String         '振替結果コード
        <VBFixedString(10)> Public KZ9 As String        '納付税額
        <VBFixedString(9)> Public KZ10 As String        '内利子税
        <VBFixedString(1)> Public KZ11 As String        '預金種別
        <VBFixedString(7)> Public KZ12 As String        '口座番号
        <VBFixedString(8)> Public KZ13 As String        '整理番号
        <VBFixedString(69)> Public KZ14 As String       'ダミー
        <VBFixedString(7)> Public KZ15 As String        '郵便番号（7桁）
        <VBFixedString(5)> Public KZ16 As String        '郵便番号（5桁）
        <VBFixedString(1)> Public KZ17 As String        '補完表示
        <VBFixedString(7)> Public KZ18 As String        '取扱金融機関番号
        <VBFixedString(7)> Public KZ19 As String        'ダミー
        <VBFixedString(6)> Public KZ20 As String        '市外局番(納税者)
        <VBFixedString(8)> Public KZ21 As String        '電話番号(納税者)
        <VBFixedString(2)> Public KZ22 As String        'ダミー
        <VBFixedString(23)> Public KZ23 As String       '都市区分
        <VBFixedString(23)> Public KZ24 As String       '住所Ⅰ
        <VBFixedString(23)> Public KZ25 As String       '住所Ⅱ
        <VBFixedString(23)> Public KZ26 As String       '住所Ⅲ
        <VBFixedString(23)> Public KZ27 As String       '肩書Ⅰ
        <VBFixedString(23)> Public KZ28 As String       '肩書Ⅱ
        <VBFixedString(23)> Public KZ29 As String       '肩書Ⅲ
        <VBFixedString(23)> Public KZ30 As String       '納税者名Ⅰ
        <VBFixedString(23)> Public KZ31 As String       '納税者名Ⅱ
        <VBFixedString(5)> Public KZ32 As String        '納貯番号
        <VBFixedString(3)> Public KZ33 As String        '口座番号
        <VBFixedString(1)> Public KZ34 As String        '継続区分
        <VBFixedString(2)> Public KZ35 As String        '依頼ファイルＮＯ
    End Structure
    Public gKOKUZEI_REC3 As gstrKOKUZEI_Record3

    'ファイル合計レコード（Ｆ）
    Structure gstrKOKUZEI_Record8
        <VBFixedString(1)> Public KZ1 As String         'データ区分(=8)
        <VBFixedString(2)> Public KZ2 As String         'ファイル区分(=91)
        <VBFixedString(5)> Public KZ3 As String         '局署番号
        <VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
        <VBFixedString(10)> Public KZ5 As String        'ダミー
        <VBFixedString(45)> Public KZ6 As String        'ダミー
        <VBFixedString(6)> Public KZ7 As String         '送付分件数
        <VBFixedString(12)> Public KZ8 As String        '送付分合計金額
        <VBFixedString(6)> Public KZ9 As String         '振替納付不能件数
        <VBFixedString(12)> Public KZ10 As String       '振替納付不能合計金額
        <VBFixedString(6)> Public KZ11 As String        '振替納付件数
        <VBFixedString(12)> Public KZ12 As String       '振替納付合計金額
        <VBFixedString(264)> Public KZ13 As String      'ダミー
        <VBFixedString(2)> Public KZ14 As String        '依頼ファイルＮＯ
    End Structure
    Public gKOKUZEI_REC8 As gstrKOKUZEI_Record8

    'ファイルエンドレコード（Ｇ）
    'Structure gstrKOKUZEI_Record9
    '    <VBFixedString(1)> Public KZ1 As String         'データ区分(=9)
    '    <VBFixedString(2)> Public KZ2 As String         'ファイル区分(=91)
    '    <VBFixedString(5)> Public KZ3 As String         '局署番号
    '    <VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
    '    <VBFixedString(10)> Public KZ5 As String        'ダミー
    '    <VBFixedString(363)> Public KZ6 As String       'ダミー
    '    <VBFixedString(2)> Public KZ7 As String         '依頼ファイルＮＯ
    'End Structure
    Structure gstrKOKUZEI_Record9
        <VBFixedString(1)> Public KZ1 As String         'データ区分(=9)
        <VBFixedString(2)> Public KZ2 As String         'ファイル区分(=91)
        <VBFixedString(5)> Public KZ3 As String         '局署番号
        <VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
        <VBFixedString(10)> Public KZ5 As String        'ダミー
        <VBFixedString(45)> Public KZ6 As String        'ダミー
        <VBFixedString(6)> Public KZ7 As String         '送付分件数
        <VBFixedString(12)> Public KZ8 As String        '送付分合計金額
        <VBFixedString(6)> Public KZ9 As String         '振替納付不能件数
        <VBFixedString(12)> Public KZ10 As String       '振替納付不能金額
        <VBFixedString(6)> Public KZ11 As String        '振替納付件数
        <VBFixedString(12)> Public KZ12 As String       '振替納付金額
        <VBFixedString(264)> Public KZ13 As String      'ダミー
        <VBFixedString(2)> Public KZ14 As String        '依頼ファイルＮＯ
    End Structure
    Public gKOKUZEI_REC9 As gstrKOKUZEI_Record9

    Structure gstrFURI_DATA_390
        <VBFixedString(390)> Public strDATA As String
    End Structure
    '390バイト格納用
    Public gstrDATA_390 As gstrFURI_DATA_390

    '--------------------
    '自振フォーマット定義
    '--------------------
    '----------------
    'ヘッダーレコード
    '----------------
    Structure JF_Record1
        <VBFixedString(9)> Public TC As String      '取引先コード
        <VBFixedString(30)> Public FKN As String    '振替企業名
        <VBFixedString(15)> Public KNM As String    '契約者氏名
        <VBFixedString(4)> Public JF1 As String     '金融機関コード
        <VBFixedString(3)> Public JF2 As String     '店舗コード
        <VBFixedString(1)> Public JF3 As String     'レコード種別
        <VBFixedString(4)> Public JF4 As String     '持込み金融機関コード
        <VBFixedString(1)> Public JF5 As String     '返還区分
        <VBFixedString(113)> Public JF6 As String   '予備
    End Structure
    Public JF_DATA1 As JF_Record1
    '--------------
    'データレコード
    '--------------
    Structure JF_Record2
        <VBFixedString(9)> Public TC As String      '取引先コード
        <VBFixedString(30)> Public FKN As String    '振替企業名
        <VBFixedString(15)> Public KNM As String    '契約者氏名
        <VBFixedString(4)> Public JF1 As String     '金融機関コード
        <VBFixedString(3)> Public JF2 As String     '店舗コード
        <VBFixedString(1)> Public JF3 As String     'レコード種別
        <VBFixedString(2)> Public JF4 As String     '科目コード
        <VBFixedString(7)> Public JF5 As String     '口座番号
        <VBFixedString(6)> Public JF6 As String     '自振指定日
        <VBFixedString(10)> Public JF7 As String    '金額
        <VBFixedString(1)> Public JF8 As String     '入出金区分
        <VBFixedString(4)> Public JF9 As String     '企業コード
        <VBFixedString(7)> Public JF10 As String    '企業シーケンス
        <VBFixedString(2)> Public JF11 As String    '口座内優先コード
        <VBFixedString(3)> Public JF12 As String    '振替コード
        <VBFixedString(2)> Public JF13 As String    '振替相手科目
        <VBFixedString(7)> Public JF14 As String    '振替相手口番
        <VBFixedString(4)> Public JF15 As String    '開始年月
        <VBFixedString(4)> Public JF16 As String    '終了年月
        <VBFixedString(1)> Public JF17 As String    '摘要設定区分
        <VBFixedString(13)> Public JF18 As String   'カナ摘要
        <VBFixedString(12)> Public JF19 As String   '漢字摘要
        <VBFixedString(24)> Public JF20 As String   '需要家番号
        <VBFixedString(7)> Public JF21 As String    '特定顧客番号
        <VBFixedString(1)> Public JF22 As String    '休日指定コード
        <VBFixedString(1)> Public JF23 As String    '予備
    End Structure
    Public JF_DATA2 As JF_Record2

    '--------------
    'エンドレコード
    '--------------
    Structure JF_Record3
        <VBFixedString(9)> Public TC As String   '取引先コード
        <VBFixedString(30)> Public FKN As String     '振替企業名
        <VBFixedString(15)> Public KNM As String    '契約者氏名
        <VBFixedString(4)> Public JF1 As String      '金融機関コード
        <VBFixedString(3)> Public JF2 As String      '店舗コード
        <VBFixedString(1)> Public JF3 As String      'レコード種別
        <VBFixedString(118)> Public JF4 As String    '予備
    End Structure
    Public JF_DATA3 As JF_Record3

    Structure JF_Record
        <VBFixedString(180)> Public JF1 As String
    End Structure
    Public JF_DATA As JF_Record

    '-------------------
    '日報データ取り込み用
    '-------------------
    Structure NIPPO_DATA_RECORD
        <VBFixedString(4)> Public NI1 As String      '金融機関コード
        <VBFixedString(3)> Public NI2 As String      '店舗コード
        <VBFixedString(2)> Public NI3 As String      'レコード種別
        <VBFixedString(3)> Public NI4 As String      '振替コード
        <VBFixedString(3)> Public NI5 As String      '企業コード
        <VBFixedString(3)> Public NI6 As String      '振替種別
        <VBFixedString(40)> Public NI7 As String     '企業名
        <VBFixedString(8)> Public NI8 As String      'データ作成日
        <VBFixedString(8)> Public NI9 As String      '振替指定日
        <VBFixedString(7)> Public NI10 As String     '普通件数
        <VBFixedString(13)> Public NI11 As String    '普通金額
        <VBFixedString(7)> Public NI12 As String     '当座件数
        <VBFixedString(13)> Public NI13 As String    '当座金額
        <VBFixedString(7)> Public NI14 As String     '企業完了件数
        <VBFixedString(13)> Public NI15 As String    '企業完了金額
        <VBFixedString(7)> Public NI16 As String     '金庫完了件数
        <VBFixedString(13)> Public NI17 As String    '金庫完了金額
        <VBFixedString(7)> Public NI18 As String     'ＷＭ完了件数
        <VBFixedString(13)> Public NI19 As String    'ＷＭ完了金額
        <VBFixedString(7)> Public NI20 As String     '企業不能件数
        <VBFixedString(13)> Public NI21 As String    '企業不能金額
        <VBFixedString(7)> Public NI22 As String     '金庫不能件数
        <VBFixedString(13)> Public NI23 As String    '金庫不能金額
        <VBFixedString(7)> Public NI24 As String     'ＷＭ不能件数
        <VBFixedString(13)> Public NI25 As String    'ＷＭ不能金額
        <VBFixedString(7)> Public NI26 As String     '依頼返却件数
        <VBFixedString(13)> Public NI27 As String    '依頼返却金額
        <VBFixedString(7)> Public NI28 As String     '事前照合正常件数
        <VBFixedString(7)> Public NI29 As String     '事前照合不能件数
        <VBFixedString(7)> Public NI30 As String     '企業持込件数
        <VBFixedString(13)> Public NI31 As String    '企業持込金額
        <VBFixedString(7)> Public NI32 As String     '金庫持込件数
        <VBFixedString(13)> Public NI33 As String    '金庫持込金額
        <VBFixedString(3)> Public NI34 As String     '予備
    End Structure
    Public NIPPO_DATA As NIPPO_DATA_RECORD

    '--------------------
    '年金フォーマット定義
    '--------------------
    '----------------
    'ヘッダーレコード
    '----------------
    Structure NK_Record1
        <VBFixedString(2)> Public NK1 As String     'データ区分
        <VBFixedString(2)> Public NK2 As String     '年金種別
        <VBFixedString(1)> Public NK3 As String     'コード区分
        <VBFixedString(10)> Public NK4 As String    '日本銀行コード
        <VBFixedString(20)> Public NK5 As String    '日本銀行名
        <VBFixedString(2)> Public NK6 As String     '日本銀行振込依頼店コード
        <VBFixedString(18)> Public NK7 As String    '日本銀行振込依頼店名
        <VBFixedString(6)> Public NK8 As String     '振込依頼日
        <VBFixedString(4)> Public NK9 As String     '依頼先金融機関コード
        <VBFixedString(14)> Public NK10 As String   '依頼先金融機関名
        <VBFixedString(14)> Public NK11 As String   'ダミー
        <VBFixedString(21)> Public NK12 As String   '依頼先金融機関店舗名
        <VBFixedString(10)> Public NK13 As String   '振込請求官庁名
        <VBFixedString(6)> Public NK14 As String    'ダミー
    End Structure
    Public NK_DATA1 As NK_Record1

    '--------------
    'データレコード
    '--------------
    Structure NK_Record2
        <VBFixedString(2)> Public NK1 As String     'データ区分
        <VBFixedString(7)> Public NK2 As String     '整理番号
        <VBFixedString(6)> Public NK3 As String     'ダミー
        <VBFixedString(4)> Public NK4 As String     '振込先金融機関コード
        <VBFixedString(14)> Public NK5 As String    '振込先金融機関名
        <VBFixedString(11)> Public NK6 As String    'ダミー
        <VBFixedString(3)> Public NK7 As String     '振込店番
        <VBFixedString(21)> Public NK8 As String    '振込先店舗名
        <VBFixedString(1)> Public NK9 As String     '振込科目
        <VBFixedString(10)> Public NK10 As String   '振込先口座番号
        <VBFixedString(25)> Public NK11 As String   '受取人氏名
        <VBFixedString(8)> Public NK12 As String    '金額
        <VBFixedString(15)> Public NK13 As String   '年金証書番号
        <VBFixedString(3)> Public NK14 As String    'ダミー
    End Structure
    Public NK_DATA2 As NK_Record2

    '--------------
    'トレーラレコード
    '--------------
    Structure NK_Record8
        <VBFixedString(2)> Public NK1 As String     'データ区分
        <VBFixedString(8)> Public NK2 As String     '合計件数
        <VBFixedString(13)> Public NK3 As String    '合計金額
        <VBFixedString(107)> Public NK4 As String   'ダミー
    End Structure
    Public NK_DATA8 As NK_Record8

    '--------------
    'エンドレコード
    '--------------
    Structure NK_Record9
        <VBFixedString(2)> Public NK1 As String     'データ区分
        <VBFixedString(8)> Public NK2 As String     '総合計件数
        <VBFixedString(13)> Public NK3 As String    '総合計金額
        <VBFixedString(107)> Public NK4 As String   'ダミー
    End Structure
    Public NK_DATA9 As NK_Record9

    Structure gstrNENKIN_DATA_130
        <VBFixedString(130)> Public strDATA As String
    End Structure
    '130バイト格納用
    Public gstrDATA_130 As gstrNENKIN_DATA_130


    '------------------------------------
    '自振契約設定ＣＳＶフォーマット定義
    '------------------------------------
    Structure JIFURI_SETTEI
        <VBFixedString(7)> Public CSV0 As String     '金庫一覧番号
        <VBFixedString(3)> Public CSV1 As String     '店舗番号
        <VBFixedString(1)> Public CSV2 As String     '役席コード
        <VBFixedString(2)> Public CSV3 As String     '科目コード
        <VBFixedString(3)> Public CSV4 As String     'オペコード
        <VBFixedString(9)> Public CSV5 As String     '科目＆口座番号
        <VBFixedString(2)> Public CSV6 As String     '行
        <VBFixedString(3)> Public CSV7 As String     '自振コード
        <VBFixedString(3)> Public CSV8 As String     '企業コード
        <VBFixedString(6)> Public CSV9 As String     '契約日
        '<VBFixedString(3)> Public CSV10 As String    '原点番号
    End Structure
    Public strJIFURI_SETTEI As JIFURI_SETTEI

    '------------------------------------
    '自振契約解除ＣＳＶフォーマット定義
    '------------------------------------
    Structure JIFURI_KAIJYO
        <VBFixedString(7)> Public CSV0 As String     '金庫一覧番号
        <VBFixedString(3)> Public CSV1 As String     '店舗番号
        <VBFixedString(1)> Public CSV2 As String     '役席コード
        <VBFixedString(2)> Public CSV3 As String     '科目コード
        <VBFixedString(3)> Public CSV4 As String     'オペコード
        <VBFixedString(9)> Public CSV5 As String     '科目＆口座番号
        <VBFixedString(2)> Public CSV6 As String     '行
        <VBFixedString(3)> Public CSV7 As String     '自振コード
        <VBFixedString(3)> Public CSV8 As String     '企業コード
        '<VBFixedString(3)> Public CSV9 As String    '原点番号
    End Structure
    Public strJIFURI_KAIJYO As JIFURI_KAIJYO


    '------------------------------------------
    '300バイトフォーマット(岡崎市税)
    '------------------------------------------
    'ヘッダーレコード
    Structure gstrSHIZEI_Record1
        <VBFixedString(1)> Public SZ1 As String    'データ区分(=1)
        <VBFixedString(2)> Public SZ2 As String    '種別コード
        <VBFixedString(1)> Public SZ3 As String    'コード区分
        <VBFixedString(10)> Public SZ4 As String   '委託者コード
        <VBFixedString(40)> Public SZ5 As String   '委託者名
        <VBFixedString(6)> Public SZ6 As String    '振替日(和暦)
        <VBFixedString(4)> Public SZ7 As String    '取引銀行番号
        <VBFixedString(15)> Public SZ8 As String   '取引銀行名
        <VBFixedString(3)> Public SZ9 As String    '取引支店番号
        <VBFixedString(15)> Public SZ10 As String  '取引支店名
        <VBFixedString(1)> Public SZ11 As String   '預金種目
        <VBFixedString(7)> Public SZ12 As String   '口座番号
        <VBFixedString(195)> Public SZ13 As String  'ダミー
    End Structure
    Public gSHIZEI_REC1 As gstrSHIZEI_Record1

    'データレコード
    Structure gstrSHIZEI_Record2
        <VBFixedString(1)> Public SZ1 As String   'データ区分(=2)
        <VBFixedString(4)> Public SZ2 As String   '引落銀行番号
        <VBFixedString(15)> Public SZ3 As String  '引落銀行名
        <VBFixedString(3)> Public SZ4 As String   '引落支店番号
        <VBFixedString(15)> Public SZ5 As String  '引落支店名
        <VBFixedString(4)> Public SZ6 As String   'ダミー
        <VBFixedString(1)> Public SZ7 As String   '預金種目
        <VBFixedString(7)> Public SZ8 As String  '預金口座番号
        <VBFixedString(30)> Public SZ9 As String  '預金者名
        <VBFixedString(10)> Public SZ10 As String '引落金額
        <VBFixedString(1)> Public SZ11 As String '新規コード
        <VBFixedString(1)> Public SZ12 As String '振替結果コード
        <VBFixedString(8)> Public SZ13 As String 'ダミー
        <VBFixedString(20)> Public SZ14 As String  '需要家番号(新)
        <VBFixedString(20)> Public SZ15 As String '需要家番号(旧)
        <VBFixedString(8)> Public SZ16 As String '調定計算年月
        <VBFixedString(6)> Public SZ17 As String  '使用水量(Ａ)
        <VBFixedString(6)> Public SZ18 As String '使用水量(Ｂ)
        <VBFixedString(6)> Public SZ19 As String  'その他
        <VBFixedString(8)> Public SZ20 As String  '量水器使用料金
        <VBFixedString(8)> Public SZ21 As String  '上水道料金
        <VBFixedString(8)> Public SZ22 As String  '下水道料金
        <VBFixedString(8)> Public SZ23 As String  'その他料金
        <VBFixedString(30)> Public SZ24 As String  '需要家名
        <VBFixedString(5)> Public SZ25 As String  '郵便番号
        <VBFixedString(53)> Public SZ26 As String  '住所(市町23桁＋字名22桁+番地8桁)
        <VBFixedString(14)> Public SZ27 As String  'ダミー
    End Structure
    Public gSHIZEI_REC2 As gstrSHIZEI_Record2

    'トレーラレコード
    Structure gstrSHIZEI_Record8
        <VBFixedString(1)> Public SZ1 As String   'データ区分(=8)
        <VBFixedString(6)> Public SZ2 As String   '合計件数
        <VBFixedString(12)> Public SZ3 As String  '合計金額
        <VBFixedString(6)> Public SZ4 As String   '振替済件数
        <VBFixedString(12)> Public SZ5 As String  '振替済金額
        <VBFixedString(6)> Public SZ6 As String   '振替不能件数
        <VBFixedString(12)> Public SZ7 As String  '振替不能金額
        <VBFixedString(245)> Public SZ8 As String 'ダミー
    End Structure
    Public gSHIZEI_REC8 As gstrSHIZEI_Record8

    Public gSHIZEI_REC9 As gstrSHIZEI_Record9
    Structure gstrSHIZEI_Record9
        <VBFixedString(1)> Public SZ1 As String   'データ区分(=9)
        <VBFixedString(299)> Public SZ2 As String   'ダミー
    End Structure

    Structure gstrFURI_DATA_300
        <VBFixedString(300)> Public strDATA As String
    End Structure
    '220バイト格納用
    Public gstrDATA_300 As gstrFURI_DATA_300

    '------------------------------------------
    '地公体フォーマット（３５０バイト）
    '------------------------------------------
    'ヘッダーレコード
    Structure gstrZEIKIN350_Record1
        <VBFixedString(1)> Public ZK1 As String    'データ区分(=1)
        <VBFixedString(2)> Public ZK2 As String    '種別コード
        <VBFixedString(1)> Public ZK3 As String    'コード区分
        <VBFixedString(8)> Public ZK4 As String    '委託者コード
        <VBFixedString(2)> Public ZK5 As String    '科目コード
        <VBFixedString(5)> Public ZK6 As String    '科目名
        <VBFixedString(35)> Public ZK7 As String   '委託者名
        <VBFixedString(2)> Public ZK8 As String    '振替年
        <VBFixedString(2)> Public ZK9 As String    '振替月
        <VBFixedString(2)> Public ZK10 As String   '振替日
        <VBFixedString(4)> Public ZK11 As String   '銀行コード
        <VBFixedString(15)> Public ZK12 As String  '銀行名
        <VBFixedString(3)> Public ZK13 As String   '支店コード
        <VBFixedString(15)> Public ZK14 As String  '支店名
        <VBFixedString(1)> Public ZK15 As String   '入金預金種目
        <VBFixedString(7)> Public ZK16 As String   '入金口座番号
        <VBFixedString(2)> Public ZK17 As String   '年度
        <VBFixedString(243)> Public ZK18 As String '予備
    End Structure
    Public gZEIKIN350_REC1 As gstrZEIKIN350_Record1

    'データレコード
    Structure gstrZEIKIN350_Record2
        <VBFixedString(1)> Public ZK1 As String   'データ区分(=2)
        <VBFixedString(4)> Public ZK2 As String   '引落銀行番号
        <VBFixedString(15)> Public ZK3 As String  '引落銀行名
        <VBFixedString(3)> Public ZK4 As String   '引落支店番号
        <VBFixedString(15)> Public ZK5 As String  '引落支店名
        <VBFixedString(4)> Public ZK6 As String   '予備
        <VBFixedString(1)> Public ZK7 As String   '預金種目
        <VBFixedString(7)> Public ZK8 As String   '口座番号
        <VBFixedString(30)> Public ZK9 As String  '口座名義人
        <VBFixedString(10)> Public ZK10 As String '引落金額
        <VBFixedString(1)> Public ZK11 As String  '新規コード
        <VBFixedString(1)> Public ZK12 As String  '振替結果コード
        <VBFixedString(10)> Public ZK13 As String '予備(スペース＋銀行使用欄)
        <VBFixedString(18)> Public ZK14 As String '（新）義務者番号
        <VBFixedString(2)> Public ZK15 As String  '予備
        <VBFixedString(18)> Public ZK16 As String '（旧）義務者番号
        <VBFixedString(2)> Public ZK17 As String  '科目コード
        <VBFixedString(5)> Public ZK18 As String  '科目名
        <VBFixedString(2)> Public ZK19 As String  '年度
        <VBFixedString(5)> Public ZK20 As String  '期別
        <VBFixedString(11)> Public ZK21 As String '整理番号
        <VBFixedString(26)> Public ZK22 As String '収納番号
        <VBFixedString(7)> Public ZK23 As String  '予備
        <VBFixedString(30)> Public ZK24 As String '納税（付）者名氏名
        <VBFixedString(5)> Public ZK25 As String  '郵便番号
        <VBFixedString(22)> Public ZK26 As String '住所（市町村）
        <VBFixedString(22)> Public ZK27 As String '住所（町名）
        <VBFixedString(22)> Public ZK28 As String '住所（番地）
        <VBFixedString(22)> Public ZK29 As String '住所（方書）
        <VBFixedString(15)> Public ZK30 As String '科目
        <VBFixedString(14)> Public ZK31 As String '口座番号
    End Structure
    Public gZEIKIN350_REC2 As gstrZEIKIN350_Record2

    'トレーラレコード
    Structure gstrZEIKIN350_Record8
        <VBFixedString(1)> Public ZK1 As String   'データ区分(=8)
        <VBFixedString(6)> Public ZK2 As String   '合計件数
        <VBFixedString(12)> Public ZK3 As String  '合計金額
        <VBFixedString(6)> Public ZK4 As String   '振替済件数
        <VBFixedString(12)> Public ZK5 As String  '振替済金額
        <VBFixedString(6)> Public ZK6 As String   '振替不能件数
        <VBFixedString(12)> Public ZK7 As String  '振替不能金額
        <VBFixedString(295)> Public ZK8 As String 'ダミー
    End Structure
    Public gZEIKIN350_REC8 As gstrZEIKIN350_Record8

    Structure gstrZEIKIN350_Record9
        <VBFixedString(1)> Public ZK1 As String   'データ区分(=9)
        <VBFixedString(349)> Public ZK2 As String   'ダミー
    End Structure
    Public gZEIKIN350_REC9 As gstrZEIKIN350_Record9


    Structure gstrFURI_DATA_350
        <VBFixedString(350)> Public strDATA As String
    End Structure
    '350バイト格納用
    Public gstrDATA_350 As gstrFURI_DATA_350




End Module
