USE [Shiran]
GO
/****** Object:  Table [dbo].[Banks]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Banks](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BanksBrunchs]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BanksBrunchs](
	[KeyId] [int] IDENTITY(1,1) NOT NULL,
	[Id] [int] NOT NULL,
	[BankId] [int] NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.BanksBrunchs] PRIMARY KEY CLUSTERED 
(
	[KeyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cities]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cities](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EsekConfiguraions]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EsekConfiguraions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EsekName] [nvarchar](max) NULL,
	[EsekAdress] [nvarchar](max) NULL,
	[EsekPhone] [nvarchar](max) NULL,
	[EsekNikuim] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.EsekConfiguraions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FarmInstructors]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FarmInstructors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ClalitNumber] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.FarmInstructors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FarmManagers]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FarmManagers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FarmId] [int] NOT NULL,
	[UserName] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[SupplierID] [int] NOT NULL,
	[SectionCode] [nvarchar](max) NULL,
	[CareCode] [nvarchar](max) NULL,
	[MefarzelUser] [nvarchar](max) NULL,
	[VetrinarUser] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.FarmManagers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FarmPDFFiles]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FarmPDFFiles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FarmId] [int] NULL,
	[FileName] [nvarchar](500) NULL,
	[Seq] [int] NULL,
	[StatusId] [int] NULL,
	[Is101] [bit] NULL,
 CONSTRAINT [PK_FarmPDFFiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Farms]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Farms](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Deleted] [bit] NOT NULL,
	[Style] [int] NULL,
	[Address] [nvarchar](500) NULL,
	[TikNikuim] [nvarchar](50) NULL,
	[IdNumber] [nvarchar](50) NULL,
	[OfficeNumber] [nvarchar](50) NULL,
	[OfficeMail] [nvarchar](50) NULL,
	[OfficeIsMail] [bit] NULL,
	[ContactName] [nvarchar](50) NULL,
	[ContactNumber] [nvarchar](50) NULL,
	[ContactMail] [nvarchar](50) NULL,
	[ContactIsMail] [bit] NULL,
 CONSTRAINT [PK_dbo.Farms] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Fields]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fields](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FarmId] [int] NULL,
	[Name] [nvarchar](100) NULL,
	[WorkerTableField] [nvarchar](100) NULL,
	[StatusId] [int] NULL,
 CONSTRAINT [PK_FieldsGens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Fields2Groups]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fields2Groups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FarmId] [int] NULL,
	[FieldsId] [int] NULL,
	[FieldsGroupsId] [int] NULL,
	[Seq] [int] NULL,
	[DefaultValue] [nvarchar](100) NULL,
	[IsWorkerShow] [bit] NULL,
	[Title] [nvarchar](100) NULL,
	[FieldsDataTypesId] [int] NULL,
 CONSTRAINT [PK_Fields2Groups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Fields2PDF]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fields2PDF](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Fields2GroupsId] [int] NULL,
	[FarmPDFFilesId] [int] NULL,
	[PageNumber] [int] NULL,
	[PdfX] [float] NULL,
	[PdfY] [float] NULL,
	[PdfWidthX] [float] NULL,
	[PdfHeightY] [float] NULL,
	[FieldsId] [int] NULL,
	[StatusId] [int] NULL,
 CONSTRAINT [PK_Fields2PDF] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FieldsDataTypes]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FieldsDataTypes](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[NameEn] [nvarchar](50) NULL,
 CONSTRAINT [PK_FieldsDataTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FieldsDDL]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FieldsDDL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FieldsGensId] [int] NULL,
	[Name] [nvarchar](100) NULL,
 CONSTRAINT [PK_FieldsDDL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FieldsGroups]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FieldsGroups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FarmId] [int] NULL,
	[Name] [nvarchar](100) NULL,
	[Seq] [int] NULL,
	[CountFieldsInRow] [int] NULL,
	[StatusId] [int] NULL,
 CONSTRAINT [PK_FieldsGroups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Files]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Files](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WorkerId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[FileName] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Files] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Logs]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Logs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Action] [nvarchar](max) NULL,
	[DateTime] [datetime] NOT NULL,
	[Device] [nvarchar](max) NULL,
	[UserAgent] [nvarchar](max) NULL,
	[Expetion] [nvarchar](max) NULL,
	[UserId] [int] NULL,
	[UserName] [nvarchar](max) NULL,
	[WorkerName] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Logs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Testpdfs]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Testpdfs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[llx] [int] NULL,
	[lly] [int] NULL,
	[urx] [int] NULL,
	[ury] [int] NULL,
	[Word] [nvarchar](50) NULL,
	[Comment] [nvarchar](50) NULL,
	[Space] [int] NULL,
	[Font] [int] NULL,
	[PageNumber] [int] NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_TestPdf] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Role] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[Password] [nvarchar](50) NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[Deleted] [bit] NOT NULL,
	[Farm_Id] [int] NOT NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[Active] [nvarchar](20) NULL,
	[AreaId] [nvarchar](max) NULL,
	[AreaId2] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkerChilds]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkerChilds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WorkerId] [int] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Taz] [nvarchar](max) NULL,
	[BirthDate] [datetime] NULL,
	[IsInHouse] [bit] NOT NULL,
	[IsBituaLeumi] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.WorkerChilds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Workers]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Workers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FarmId] [int] NULL,
	[UserId] [int] NOT NULL,
	[ShnatMas] [nvarchar](max) NULL,
	[FirstName] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[Taz] [nvarchar](max) NULL,
	[BirthDate] [date] NULL,
	[AliaDate] [date] NULL,
	[PhoneSelular] [nvarchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[Street] [nvarchar](max) NULL,
	[HouseNumber] [nvarchar](max) NULL,
	[Mikud] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Gender] [nvarchar](max) NULL,
	[ToshavIsrael] [nvarchar](max) NULL,
	[Kibutz] [nvarchar](max) NULL,
	[KupatHolim] [nvarchar](max) NULL,
	[Kupa] [nvarchar](max) NULL,
	[StatusFamely] [nvarchar](max) NULL,
	[SugMaskuret] [nvarchar](max) NULL,
	[StartWorkDate] [date] NULL,
	[AnotherMaskuret] [nvarchar](max) NULL,
	[AnotherSugMaskuret] [nvarchar](max) NULL,
	[Nkudutzikuy] [nvarchar](max) NULL,
	[AnotherMaskuretHafrashot1] [nvarchar](max) NULL,
	[AnotherMaskuretHafrashot2] [nvarchar](max) NULL,
	[ZoogFirstName] [nvarchar](max) NULL,
	[ZoogLastName] [nvarchar](max) NULL,
	[ZoogTaz] [nvarchar](max) NULL,
	[ZoogDarkon] [nvarchar](max) NULL,
	[ZoogBirthDate] [date] NULL,
	[ZoogAliaDate] [date] NULL,
	[ZoogMaskuret] [nvarchar](max) NULL,
	[ZoogMaskuret1] [nvarchar](max) NULL,
	[ZoogMaskuret2] [nvarchar](max) NULL,
	[ZikuyToshavIsrael] [nvarchar](max) NULL,
	[ZikuyNeke] [bit] NOT NULL,
	[ZikuyToshavMas] [bit] NOT NULL,
	[ZikuyYeshuvStartDate] [date] NULL,
	[ZikuyOleHadash] [bit] NOT NULL,
	[ZikuyOleHadashDate] [date] NULL,
	[ZikuyOleHadashFromStartYearDate] [date] NULL,
	[ZikuyZoog] [bit] NOT NULL,
	[ZikuyHoreHayNefrad] [bit] NOT NULL,
	[ZikuyLuladBehzka] [bit] NOT NULL,
	[ZikuyLuladNuldoShnatMas] [int] NULL,
	[ZikuyLulad_1_5] [int] NULL,
	[ZikuyLulad_6_17] [int] NULL,
	[ZikuyLulad_18] [int] NULL,
	[ZikuyLuladPeutim] [bit] NOT NULL,
	[ZikuyLuladNuldoShnatMas2] [int] NULL,
	[ZikuyLulad2_1_5] [int] NULL,
	[ZikuyHoreYahid] [bit] NOT NULL,
	[ZikuyPsakDinMezonot] [bit] NOT NULL,
	[ZikuyLuladMugbalut] [bit] NOT NULL,
	[ZikuyLuladMugbalutNumber] [int] NULL,
	[ZikuyTashlumMezonot] [bit] NOT NULL,
	[ZikuyBetween16_18] [bit] NOT NULL,
	[ZikuyHayalEnd] [bit] NOT NULL,
	[ZikuyHayalEndStartDate] [date] NULL,
	[ZikuyHayalEndEndDate] [date] NULL,
	[ZikuyToarAkdemi] [bit] NOT NULL,
	[TiumMas] [nvarchar](max) NULL,
	[TiumMasBakasha] [nvarchar](max) NULL,
	[TiumMasAnotherMaskuretName] [nvarchar](max) NULL,
	[TiumMasAnotherMaskuretKtuvet] [nvarchar](max) NULL,
	[TiumMasAnotherMaskuretTikNikuim] [nvarchar](max) NULL,
	[TiumMasAnotherMaskuretSug] [nvarchar](max) NULL,
	[TiumMasAnotherMaskuretSum] [int] NULL,
	[TiumMasAnotherMaskuretMas] [int] NULL,
	[HeskemYom] [nvarchar](max) NULL,
	[HeskemHodesh] [nvarchar](max) NULL,
	[HeskemShana] [nvarchar](max) NULL,
	[HeskemWorkerName] [nvarchar](max) NULL,
	[HeskemWorkerTaz] [nvarchar](max) NULL,
	[HeskemWorkerStreet] [nvarchar](max) NULL,
	[HeskemWorkerPhone] [nvarchar](max) NULL,
	[HeskemTafkid] [nvarchar](max) NULL,
	[HeskemSamkut] [nvarchar](max) NULL,
	[HeskemStartDate] [date] NULL,
	[HeskemStartHour] [nvarchar](max) NULL,
	[HeskemEndHour] [nvarchar](max) NULL,
	[HeskemBriutEnKlumCheckbox] [bit] NOT NULL,
	[HeskemWorkerMigbalotCheckbox] [bit] NOT NULL,
	[HeskemWorkerMigbalot] [nvarchar](max) NULL,
	[HeskemWorkerTrufotCheckbox] [bit] NOT NULL,
	[HeskemWorkerTrufot] [nvarchar](max) NULL,
	[HeskemWorkerRgishutCheckbox] [bit] NOT NULL,
	[HeskemWorkerRgishut] [nvarchar](max) NULL,
	[HeskemWorkerCronitCheckbox] [bit] NOT NULL,
	[HeskemWorkerCronit] [nvarchar](max) NULL,
	[HeskemWorkerMaskuert3] [int] NULL,
	[HeskemWorkerMaskuert4] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[Status] [nvarchar](max) NULL,
	[DateRigster] [datetime] NULL,
	[ImgData] [nvarchar](max) NULL,
	[Darkon] [nvarchar](max) NULL,
	[HeskemWorkerMaskuert5] [int] NULL,
	[BankNumName] [nvarchar](max) NULL,
	[BrunchNumName] [nvarchar](max) NULL,
	[BankAccountNumber] [nvarchar](max) NULL,
	[Comments] [nvarchar](max) NULL,
	[UniqNumber] [nvarchar](max) NULL,
	[IsNew] [bit] NOT NULL,
	[IsSendSMS] [bit] NULL,
	[IsValid] [bit] NULL,
 CONSTRAINT [PK_dbo.Workers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[FarmPDFFiles] ADD  CONSTRAINT [DF_FarmPDFFiles_StatusId]  DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [dbo].[FarmPDFFiles] ADD  CONSTRAINT [DF_FarmPDFFiles_Is101]  DEFAULT ((0)) FOR [Is101]
GO
ALTER TABLE [dbo].[Farms] ADD  CONSTRAINT [DF_Farms_OfficeIsMail]  DEFAULT ((0)) FOR [OfficeIsMail]
GO
ALTER TABLE [dbo].[Farms] ADD  CONSTRAINT [DF_Farms_ContactIsMail]  DEFAULT ((0)) FOR [ContactIsMail]
GO
ALTER TABLE [dbo].[Fields] ADD  CONSTRAINT [DF_FieldsGens_StatusId]  DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [dbo].[Fields2Groups] ADD  CONSTRAINT [DF_Fields2Groups_IsWorkerShow]  DEFAULT ((1)) FOR [IsWorkerShow]
GO
ALTER TABLE [dbo].[FieldsGroups] ADD  CONSTRAINT [DF_FieldsGroups_StatusId]  DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF__Users__Deleted__37C5420D]  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF__Users__Farm_Id__4BCC3ABA]  DEFAULT ((0)) FOR [Farm_Id]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_Deleted]  DEFAULT (N'((active))') FOR [Active]
GO
ALTER TABLE [dbo].[Workers] ADD  CONSTRAINT [DF__Workers__IsNew__2FCF1A8A]  DEFAULT ((0)) FOR [IsNew]
GO
ALTER TABLE [dbo].[Workers] ADD  CONSTRAINT [DF_Workers_IsSendSMS]  DEFAULT ((0)) FOR [IsSendSMS]
GO
ALTER TABLE [dbo].[Workers] ADD  CONSTRAINT [DF_Workers_IsValid]  DEFAULT ((0)) FOR [IsValid]
GO
ALTER TABLE [dbo].[Workers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Workers_dbo.Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Workers] CHECK CONSTRAINT [FK_dbo.Workers_dbo.Users_UserId]
GO
/****** Object:  StoredProcedure [dbo].[CreateEntityFromDatabase]    Script Date: 18/05/2025 22:52:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CreateEntityFromDatabase]
(
  @TableName nvarchar(500) = 'Fields2PDF'
 
 
)
AS

 
declare @Result varchar(max) = '[Table("'+ @TableName +'")]' + char(10) + 'public class ' + @TableName + '
{'

select @Result = @Result + '
    public ' + ColumnType + NullableSign + ' ' + ColumnName + ' { get; set; }
'
from
(
    select 
        replace(col.name, ' ', '_') ColumnName,
        column_id ColumnId,
        case typ.name 
            when 'bigint' then 'long'
            when 'binary' then 'byte[]'
            when 'bit' then 'bool'
            when 'char' then 'string?'
            when 'date' then 'DateTime'
            when 'datetime' then 'DateTime'
            when 'datetime2' then 'DateTime'
            when 'datetimeoffset' then 'DateTimeOffset'
            when 'decimal' then 'decimal'
            when 'float' then 'double'
            when 'image' then 'byte[]'
            when 'int' then 'int'
            when 'money' then 'decimal'
            when 'nchar' then 'string?'
            when 'ntext' then 'string?'
            when 'numeric' then 'decimal'
            when 'nvarchar' then 'string?'
            when 'real' then 'float'
            when 'smalldatetime' then 'DateTime'
            when 'smallint' then 'short'
            when 'smallmoney' then 'decimal'
            when 'text' then 'string'
            when 'time' then 'TimeSpan'
            when 'timestamp' then 'long'
            when 'tinyint' then 'byte'
            when 'uniqueidentifier' then 'Guid'
            when 'varbinary' then 'byte[]'
            when 'varchar' then 'string?'
            else 'UNKNOWN_' + typ.name
        end ColumnType,
        case 
            when col.is_nullable = 1 and typ.name in ('bigint', 'bit', 'date', 'datetime', 'datetime2', 'datetimeoffset', 'decimal', 'float', 'int', 'money', 'numeric', 'real', 'smalldatetime', 'smallint', 'smallmoney', 'time', 'tinyint', 'uniqueidentifier') 
            then '?' 
            else '' 
        end NullableSign
    from sys.columns col
        join sys.types typ on
            col.system_type_id = typ.system_type_id AND col.user_type_id = typ.user_type_id
    where object_id = object_id(@TableName)
) t
order by ColumnId

set @Result = @Result  + '
}'

print @Result


GO
