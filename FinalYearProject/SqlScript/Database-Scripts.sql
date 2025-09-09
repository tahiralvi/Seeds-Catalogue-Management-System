USE [master]
GO
/****** Object:  Database [SeedsDB]    Script Date: 08/09/2025 3:30:58 am ******/
CREATE DATABASE [SeedsDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SeedsDB', FILENAME = N'D:\Databases\MSSQL15.MSSQLSERVER\MSSQL\DATA\SeedsDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SeedsDB_log', FILENAME = N'D:\Databases\MSSQL15.MSSQLSERVER\MSSQL\DATA\SeedsDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [SeedsDB] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SeedsDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SeedsDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SeedsDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SeedsDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SeedsDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SeedsDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [SeedsDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SeedsDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SeedsDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SeedsDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SeedsDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SeedsDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SeedsDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SeedsDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SeedsDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SeedsDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SeedsDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SeedsDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SeedsDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SeedsDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SeedsDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SeedsDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SeedsDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SeedsDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SeedsDB] SET  MULTI_USER 
GO
ALTER DATABASE [SeedsDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SeedsDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SeedsDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SeedsDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [SeedsDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [SeedsDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'SeedsDB', N'ON'
GO
ALTER DATABASE [SeedsDB] SET QUERY_STORE = OFF
GO
USE [SeedsDB]
GO
/****** Object:  User [sa]    Script Date: 08/09/2025 3:30:59 am ******/
CREATE USER [sa] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [sa]
GO
/****** Object:  Table [dbo].[Agents]    Script Date: 08/09/2025 3:30:59 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Agents](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [nvarchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 08/09/2025 3:30:59 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[CreatedDate] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Seeds]    Script Date: 08/09/2025 3:30:59 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Seeds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[Approval] [bit] NOT NULL,
	[Stock] [int] NOT NULL,
	[Image] [nvarchar](500) NULL,
	[ExpiryDate] [datetime2](7) NOT NULL,
	[AgentID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedDate] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Agents] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Category] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Seeds] ADD  DEFAULT ((0)) FOR [Approval]
GO
ALTER TABLE [dbo].[Seeds] ADD  DEFAULT ((0)) FOR [Stock]
GO
ALTER TABLE [dbo].[Seeds] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Seeds] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[Seeds]  WITH CHECK ADD  CONSTRAINT [FK_Seeds_Agents] FOREIGN KEY([AgentID])
REFERENCES [dbo].[Agents] ([Id])
GO
ALTER TABLE [dbo].[Seeds] CHECK CONSTRAINT [FK_Seeds_Agents]
GO
ALTER TABLE [dbo].[Seeds]  WITH CHECK ADD  CONSTRAINT [FK_Seeds_Category] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Category] ([Id])
GO
ALTER TABLE [dbo].[Seeds] CHECK CONSTRAINT [FK_Seeds_Category]
GO
ALTER TABLE [dbo].[Seeds]  WITH CHECK ADD CHECK  (([ExpiryDate]>getdate()))
GO
ALTER TABLE [dbo].[Seeds]  WITH CHECK ADD CHECK  (([Price]>=(0)))
GO
ALTER TABLE [dbo].[Seeds]  WITH CHECK ADD CHECK  (([Stock]>=(0)))
GO
USE [master]
GO
ALTER DATABASE [SeedsDB] SET  READ_WRITE 
GO

-- Add Static Data into the Tables
INSERT INTO Category (Name, Description) VALUES
('Breeder Seed', 'The highest purity class of seed, produced and controlled by a plant breeder.'),
('Certified Seed', 'The progeny of foundation or registered seed, produced to maintain genetic identity and purity for commercial sale.'),
('Flower seeds', 'Seeds used for growing ornamental and flowering plants.'),
('Foundation Seed', 'The progeny of breeder seed, grown by a certified agency to produce certified seed.'),
('Fruit seeds', 'Seeds found in or used to grow various types of fruit.'),
('GMO', 'Genetically modified organism seeds, altered in a lab to introduce new or modified traits.'),
('Grain seeds', 'Seeds from cereal crops, including wheat, rice, and corn.'),
('Heirloom', 'A type of open-pollinated seed with a long history of being passed down through generations.'),
('Herb seeds', 'Seeds cultivated for growing herbs for culinary, medicinal, or aromatic purposes.'),
('Hybrid', 'Seeds created by cross-pollinating two different parent plant varieties.'),
('Legume seeds', 'Seeds from plants in the legume family, including beans, peas, and lentils.'),
('Open-pollinated', 'Seeds produced when pollination occurs naturally, resulting in offspring similar to the parent plants.'),
('Registered Seed', 'The progeny of foundation seed, produced to maintain genetic purity and identity.'),
('Vegetable seeds', 'Seeds used for growing a wide variety of vegetables.');
GO


INSERT INTO Agents (Name, Email) VALUES
('Green Acres Seed Co.', 'sales@greenacresseeds.com'),
('Harvest King Seed Supply', 'info@harvestking.net'),
('Global Seed Distributors', 'contact@globalseed.org'),
('Nature''s Best Seeds', 'support@naturesbestseeds.com'),
('Legacy Agronomics', 'info@legacyag.com');
GO
