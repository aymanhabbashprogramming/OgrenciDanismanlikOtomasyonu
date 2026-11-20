-- Countries.sql
-- Creates the Countries lookup table, seeds all ISO 3166-1 countries with Turkish names,
-- adds NationalityCountryId to Students, and backfills domestic students with Türkiye.
-- Idempotent: safe to run multiple times on both fresh and existing databases.
-- Run with: sqlcmd -S EYMEN -d OgrenciDanismanlikDB -i DatabaseScripts/Countries.sql -f 65001

SET NOCOUNT ON;

-- ── 1. Create Countries table if it does not exist ────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Countries')
BEGIN
    CREATE TABLE Countries
    (
        CountryId   INT IDENTITY(1,1) NOT NULL,
        CountryName NVARCHAR(100)     NOT NULL,
        IsoCode     CHAR(2)           NOT NULL,
        CONSTRAINT PK_Countries        PRIMARY KEY (CountryId),
        CONSTRAINT UQ_Countries_IsoCode UNIQUE     (IsoCode)
    );
    PRINT N'Countries tablosu oluşturuldu.';
END
ELSE
BEGIN
    PRINT N'Countries tablosu zaten mevcut.';
END

-- ── 2. Seed all countries (idempotent via MERGE on IsoCode) ───────────────────
MERGE INTO Countries AS Target
USING (VALUES
    ('AF', N'Afganistan'),
    ('AX', N'Åland Adaları'),
    ('AL', N'Arnavutluk'),
    ('DZ', N'Cezayir'),
    ('AS', N'Amerikan Samoası'),
    ('AD', N'Andorra'),
    ('AO', N'Angola'),
    ('AI', N'Anguilla'),
    ('AQ', N'Antarktika'),
    ('AG', N'Antigua ve Barbuda'),
    ('AR', N'Arjantin'),
    ('AM', N'Ermenistan'),
    ('AW', N'Aruba'),
    ('AU', N'Avustralya'),
    ('AT', N'Avusturya'),
    ('AZ', N'Azerbaycan'),
    ('BS', N'Bahamalar'),
    ('BH', N'Bahreyn'),
    ('BD', N'Bangladeş'),
    ('BB', N'Barbados'),
    ('BY', N'Beyaz Rusya'),
    ('BE', N'Belçika'),
    ('BZ', N'Belize'),
    ('BJ', N'Benin'),
    ('BM', N'Bermuda'),
    ('BT', N'Bhutan'),
    ('BO', N'Bolivya'),
    ('BQ', N'Karayip Hollanda'),
    ('BA', N'Bosna Hersek'),
    ('BW', N'Botsvana'),
    ('BV', N'Bouvet Adası'),
    ('BR', N'Brezilya'),
    ('IO', N'Britanya Hint Okyanusu Toprakları'),
    ('BN', N'Brunei'),
    ('BG', N'Bulgaristan'),
    ('BF', N'Burkina Faso'),
    ('BI', N'Burundi'),
    ('CV', N'Yeşil Burun Adaları'),
    ('KH', N'Kamboçya'),
    ('CM', N'Kamerun'),
    ('CA', N'Kanada'),
    ('KY', N'Cayman Adaları'),
    ('CF', N'Orta Afrika Cumhuriyeti'),
    ('TD', N'Çad'),
    ('CL', N'Şili'),
    ('CN', N'Çin'),
    ('CX', N'Christmas Adası'),
    ('CC', N'Cocos Adaları'),
    ('CO', N'Kolombiya'),
    ('KM', N'Komorlar'),
    ('CG', N'Kongo'),
    ('CD', N'Kongo Demokratik Cumhuriyeti'),
    ('CK', N'Cook Adaları'),
    ('CR', N'Kosta Rika'),
    ('CI', N'Fildişi Sahili'),
    ('HR', N'Hırvatistan'),
    ('CU', N'Küba'),
    ('CW', N'Curaçao'),
    ('CY', N'Kıbrıs'),
    ('CZ', N'Çekya'),
    ('DK', N'Danimarka'),
    ('DJ', N'Cibuti'),
    ('DM', N'Dominika'),
    ('DO', N'Dominik Cumhuriyeti'),
    ('EC', N'Ekvador'),
    ('EG', N'Mısır'),
    ('SV', N'El Salvador'),
    ('GQ', N'Ekvator Ginesi'),
    ('ER', N'Eritre'),
    ('EE', N'Estonya'),
    ('SZ', N'Esvatini'),
    ('ET', N'Etiyopya'),
    ('FK', N'Falkland Adaları'),
    ('FO', N'Faroe Adaları'),
    ('FJ', N'Fiji'),
    ('FI', N'Finlandiya'),
    ('FR', N'Fransa'),
    ('GF', N'Fransız Guyanası'),
    ('PF', N'Fransız Polinezyası'),
    ('TF', N'Fransız Güney Toprakları'),
    ('GA', N'Gabon'),
    ('GM', N'Gambiya'),
    ('GE', N'Gürcistan'),
    ('DE', N'Almanya'),
    ('GH', N'Gana'),
    ('GI', N'Cebelitarık'),
    ('GR', N'Yunanistan'),
    ('GL', N'Grönland'),
    ('GD', N'Grenada'),
    ('GP', N'Guadeloupe'),
    ('GU', N'Guam'),
    ('GT', N'Guatemala'),
    ('GG', N'Guernsey'),
    ('GN', N'Gine'),
    ('GW', N'Gine-Bissau'),
    ('GY', N'Guyana'),
    ('HT', N'Haiti'),
    ('HM', N'Heard ve McDonald Adaları'),
    ('VA', N'Vatikan'),
    ('HN', N'Honduras'),
    ('HK', N'Hong Kong'),
    ('HU', N'Macaristan'),
    ('IS', N'İzlanda'),
    ('IN', N'Hindistan'),
    ('ID', N'Endonezya'),
    ('IR', N'İran'),
    ('IQ', N'Irak'),
    ('IE', N'İrlanda'),
    ('IM', N'Man Adası'),
    ('IL', N'İsrail'),
    ('IT', N'İtalya'),
    ('JM', N'Jamaika'),
    ('JP', N'Japonya'),
    ('JE', N'Jersey'),
    ('JO', N'Ürdün'),
    ('KZ', N'Kazakistan'),
    ('KE', N'Kenya'),
    ('KI', N'Kiribati'),
    ('KP', N'Kuzey Kore'),
    ('KR', N'Güney Kore'),
    ('KW', N'Kuveyt'),
    ('KG', N'Kırgızistan'),
    ('LA', N'Laos'),
    ('LV', N'Letonya'),
    ('LB', N'Lübnan'),
    ('LS', N'Lesotho'),
    ('LR', N'Liberya'),
    ('LY', N'Libya'),
    ('LI', N'Liechtenstein'),
    ('LT', N'Litvanya'),
    ('LU', N'Lüksemburg'),
    ('MO', N'Makao'),
    ('MG', N'Madagaskar'),
    ('MW', N'Malavi'),
    ('MY', N'Malezya'),
    ('MV', N'Maldivler'),
    ('ML', N'Mali'),
    ('MT', N'Malta'),
    ('MH', N'Marshall Adaları'),
    ('MQ', N'Martinik'),
    ('MR', N'Moritanya'),
    ('MU', N'Mauritius'),
    ('YT', N'Mayotte'),
    ('MX', N'Meksika'),
    ('FM', N'Mikronezya'),
    ('MD', N'Moldova'),
    ('MC', N'Monako'),
    ('MN', N'Moğolistan'),
    ('ME', N'Karadağ'),
    ('MS', N'Montserrat'),
    ('MA', N'Fas'),
    ('MZ', N'Mozambik'),
    ('MM', N'Myanmar'),
    ('NA', N'Namibya'),
    ('NR', N'Nauru'),
    ('NP', N'Nepal'),
    ('NL', N'Hollanda'),
    ('NC', N'Yeni Kaledonya'),
    ('NZ', N'Yeni Zelanda'),
    ('NI', N'Nikaragua'),
    ('NE', N'Nijer'),
    ('NG', N'Nijerya'),
    ('NU', N'Niue'),
    ('NF', N'Norfolk Adası'),
    ('MK', N'Kuzey Makedonya'),
    ('MP', N'Kuzey Mariana Adaları'),
    ('NO', N'Norveç'),
    ('OM', N'Umman'),
    ('PK', N'Pakistan'),
    ('PW', N'Palau'),
    ('PS', N'Filistin'),
    ('PA', N'Panama'),
    ('PG', N'Papua Yeni Gine'),
    ('PY', N'Paraguay'),
    ('PE', N'Peru'),
    ('PH', N'Filipinler'),
    ('PN', N'Pitcairn Adaları'),
    ('PL', N'Polonya'),
    ('PT', N'Portekiz'),
    ('PR', N'Porto Riko'),
    ('QA', N'Katar'),
    ('RE', N'Réunion'),
    ('RO', N'Romanya'),
    ('RU', N'Rusya'),
    ('RW', N'Ruanda'),
    ('BL', N'Saint Barthélemy'),
    ('SH', N'Saint Helena'),
    ('KN', N'Saint Kitts ve Nevis'),
    ('LC', N'Saint Lucia'),
    ('MF', N'Saint Martin'),
    ('PM', N'Saint Pierre ve Miquelon'),
    ('VC', N'Saint Vincent ve Grenadinler'),
    ('WS', N'Samoa'),
    ('SM', N'San Marino'),
    ('ST', N'São Tomé ve Príncipe'),
    ('SA', N'Suudi Arabistan'),
    ('SN', N'Senegal'),
    ('RS', N'Sırbistan'),
    ('SC', N'Seyşeller'),
    ('SL', N'Sierra Leone'),
    ('SG', N'Singapur'),
    ('SX', N'Sint Maarten'),
    ('SK', N'Slovakya'),
    ('SI', N'Slovenya'),
    ('SB', N'Solomon Adaları'),
    ('SO', N'Somali'),
    ('ZA', N'Güney Afrika'),
    ('GS', N'Güney Georgia ve Güney Sandwich Adaları'),
    ('SS', N'Güney Sudan'),
    ('ES', N'İspanya'),
    ('LK', N'Sri Lanka'),
    ('SD', N'Sudan'),
    ('SR', N'Surinam'),
    ('SJ', N'Svalbard ve Jan Mayen'),
    ('SE', N'İsveç'),
    ('CH', N'İsviçre'),
    ('SY', N'Suriye'),
    ('TW', N'Tayvan'),
    ('TJ', N'Tacikistan'),
    ('TZ', N'Tanzanya'),
    ('TH', N'Tayland'),
    ('TL', N'Doğu Timor'),
    ('TG', N'Togo'),
    ('TK', N'Tokelau'),
    ('TO', N'Tonga'),
    ('TT', N'Trinidad ve Tobago'),
    ('TN', N'Tunus'),
    ('TR', N'Türkiye'),
    ('TM', N'Türkmenistan'),
    ('TC', N'Turks ve Caicos Adaları'),
    ('TV', N'Tuvalu'),
    ('UG', N'Uganda'),
    ('UA', N'Ukrayna'),
    ('AE', N'Birleşik Arap Emirlikleri'),
    ('GB', N'Birleşik Krallık'),
    ('US', N'Amerika Birleşik Devletleri'),
    ('UM', N'Amerika Birleşik Devletleri Uzak Adaları'),
    ('UY', N'Uruguay'),
    ('UZ', N'Özbekistan'),
    ('VU', N'Vanuatu'),
    ('VE', N'Venezuela'),
    ('VN', N'Vietnam'),
    ('VG', N'Britanya Virjin Adaları'),
    ('VI', N'ABD Virjin Adaları'),
    ('WF', N'Wallis ve Futuna'),
    ('EH', N'Batı Sahara'),
    ('YE', N'Yemen'),
    ('ZM', N'Zambiya'),
    ('ZW', N'Zimbabve'),
    ('XK', N'Kosova')
) AS Source (IsoCode, CountryName)
ON Target.IsoCode = Source.IsoCode
WHEN NOT MATCHED THEN
    INSERT (CountryName, IsoCode) VALUES (Source.CountryName, Source.IsoCode);

PRINT N'Ülke verileri güncellendi. Toplam: ' + CAST(@@ROWCOUNT AS NVARCHAR) + N' yeni kayıt eklendi.';

-- ── 3. Add NationalityCountryId column to Students if it does not exist ───────
IF NOT EXISTS (
    SELECT 1
    FROM   sys.columns
    WHERE  object_id = OBJECT_ID(N'Students')
      AND  name      = N'NationalityCountryId'
)
BEGIN
    ALTER TABLE Students
        ADD NationalityCountryId INT NULL
            REFERENCES Countries (CountryId);
    PRINT N'Students.NationalityCountryId kolonu eklendi.';
END
ELSE
BEGIN
    PRINT N'Students.NationalityCountryId kolonu zaten mevcut.';
END

GO

-- ── 4. Backfill: domestic students without a nationality get Türkiye ──────────
DECLARE @TurkiyeId INT = (SELECT CountryId FROM Countries WHERE IsoCode = 'TR');

UPDATE Students
SET    NationalityCountryId = @TurkiyeId
WHERE  IsInternational        = 0
  AND  NationalityCountryId   IS NULL;

PRINT N'Yurt içi öğrenciler Türkiye ile güncellendi.';

-- ── 5. Verification ────────────────────────────────────────────────────────────
PRINT N'';
PRINT N'Ülke tablosu özeti:';
SELECT COUNT(*) AS ToplamUlkeSayisi FROM Countries;

PRINT N'Türkçe karakter doğrulama (ü=252, İ=304 beklenir):';
SELECT
    CountryName,
    UNICODE(SUBSTRING(CountryName, CHARINDEX(N'ü', LOWER(CountryName)), 1)) AS UnicodeU
FROM Countries
WHERE IsoCode IN ('TR', 'DE', 'FR');

PRINT N'Bitti.';
