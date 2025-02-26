IF OBJECT_ID('dbo.Station') IS NULL
CREATE TABLE dbo.Station
(
 StationId int NOT NULL,
 StationName nvarchar(255) NOT NULL
CONSTRAINT PK_Station PRIMARY KEY NONCLUSTERED (StationId),
CONSTRAINT CUX_Station_StationName UNIQUE CLUSTERED ([StationName] ASC)
);
GO

select * from dbo.Station

insert into dbo.Station
select 5100066,'Warszawa Wschodnia'



IF OBJECT_ID('dbo.AdvertClient') IS NULL
CREATE TABLE dbo.AdvertClient
(
 AdvertClientId int NOT NULL IDENTITY(1,1),
 ClientName nvarchar(255) NOT NULL,
 ClientRepresentativePerson nvarchar(125) NULL,
 ClientEmail nvarchar(50) NULL,
 ClientPhone nvarchar(10) NOT NULL,
 ClientAdress nvarchar(255) NULL,
CONSTRAINT PK_AdvertClient PRIMARY KEY NONCLUSTERED (AdvertClientId),
CONSTRAINT CUX_AdvertClient_ClientName_ClientPhone UNIQUE CLUSTERED ([ClientName] ASC, [ClientPhone] ASC)
);
GO

insert into dbo.AdvertClient(ClientName,ClientPhone)
select  'ZTM Warszawa','801044484' union
select 'PKP Polskie Linie Kolejowe S.A.','662114900' union
select 'Teatr Studio','505102974' union
select 'POLSKA OPERA KRÓLEWSKA','500309402' union
select 'Szybka Kolej Miejska w Warszawie','801044484'


IF OBJECT_ID('dbo.Advert') IS NULL
CREATE TABLE dbo.Advert
(
 AdvertId int NOT NULL IDENTITY(1,1),
 AdvertName nvarchar(120) NOT NULL,
 AdvertFile nvarchar(120) NOT NULL,
 AdvertClientId int not null,
CONSTRAINT PK_Advert PRIMARY KEY NONCLUSTERED (AdvertId),
CONSTRAINT FK_advert_Client FOREIGN KEY (AdvertClientId)REFERENCES dbo.AdvertClient(AdvertClientId),
CONSTRAINT CUX_Advert_AdvertName_AdverFile UNIQUE CLUSTERED ([AdvertName] ASC,[AdvertClientId] ASC)
);
GO



















insert into dbo.Advert
select 'Wy³amanie rogatki','PLK_wylamiane_rogatki_nowe-r20250123-7.webm',1 union
select 'Czarodziejski flet','POK_skm_CZARODZIEJSKI-FLET-DLA-DZIECI_03.2025-r20250205-9.webm',2 union
select 'Praca w SKM','Praca_SKM_elektryk_1920x810-r20250116-5.webm',3 union
select 'Teatr - Mahagoyny','TS_Mahagonny_1920x810-r20241204-3.webm',4 union
select 'Rogatek','4_UTK_animacja_BAGAZ_NEW-r20250113-5.webm',1 union
select 'Warszawa mruga','ZTM_Warszawa_mruga_9.02-r20250203-1.webm',5 union
select 'VENUS','POK_VENUS-AND-ADONIS_25.02-r20250115-3.webm',2 union
select 'TS_STARA','TS_STARA-1920x810-r20241017-19.webm',4 union
select 'ZTM HOLOGRAM','ZTM_PLAKAT_E_HOLOGRAM_DLA_HB_TABOR_SKM_2-r20250117-9.webm',5 union
select 'SKM 20 lat','SKM_20lecie_1920x810-r20240510-15.webm',3



select * from dbo.AdvertClient






IF OBJECT_ID('dbo.AnnoucementClient') IS NULL
CREATE TABLE dbo.AnnoucementClient
(
 AnnoucementClientId int NOT NULL IDENTITY(1,1),
 ClientName nvarchar(255) NOT NULL,
 ClientRepresentativePerson nvarchar(125) NULL,
 ClientEmail nvarchar(50) NULL,
 ClientPhone nvarchar(10) NOT NULL,
 ClientAdress nvarchar(255) NULL,
CONSTRAINT PK_AnnoucementClient PRIMARY KEY NONCLUSTERED (AnnoucementClientId),
CONSTRAINT CUX_AnnoucementClient_ClientName_ClientPhone UNIQUE CLUSTERED ([ClientName] ASC, [ClientPhone] ASC)
);
GO

IF OBJECT_ID('dbo.Annoucement') IS NULL
CREATE TABLE dbo.Annoucement
(
 AnnoucementId int NOT NULL IDENTITY(1,1),
 AnnoucementName varchar(255) NOT NULL,
 AnnoucementFile nvarchar(120) NOT NULL,
 AnnoucementContent varchar(max) not null,
 AnnoucementClientId int not null,
CONSTRAINT PK_Annoucement PRIMARY KEY NONCLUSTERED (AnnoucementId),
CONSTRAINT FK_Annoucement_Client FOREIGN KEY (AnnoucementClientId)REFERENCES dbo.AnnoucementClient(AnnoucementClientId),
CONSTRAINT CUX_Annoucement_AnnoucementName UNIQUE CLUSTERED ([AnnoucementName] ASC,[AnnoucementClientId] asc)
);
GO


drop TABLE dbo.Annoucement


go
CREATE OR ALTER PROCEDURE dbo.sp_api_getAdvertPlaylist4Station @StationId int
as
begin
	select isnull(JSON_QUERY('[' + STRING_AGG('"'+AdvertFile+'"',',') + ']','$'),'[]') as result from dbo.Advert

end
go
execute as login ='stationApi'
exec dbo.sp_api_getAdvertPlaylist4Station @StationId =5
revert;

grant execute on schema::dbo to stationApi


drop TABLE dbo.Advert