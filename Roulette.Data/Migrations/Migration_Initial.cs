using FluentMigrator;
using Microsoft.Extensions.Configuration;
using Roulette.Helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Roulette.Data.Migrations
{
    [Migration(1, "Create database and tables")]
    public class Migration_Initial : Migration
    {
        public Migration_Initial(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        private readonly IConfiguration configuration;

        public override void Up()
        {
            var connectionString = new System.Data.SqlClient.SqlConnectionStringBuilder(configuration.GetConnectionString(Const.RouletteConnectionString));
            var db = connectionString.InitialCatalog;

            Execute.Sql(@$"
                        USE [{db}];
                        IF OBJECT_ID('VersionInfo') IS NULL 
                        BEGIN
                            CREATE TABLE [dbo].[VersionInfo](
	                            [Version] [int] NOT NULL,
	                            [AppliedOn] [datetime2](7) NOT NULL,
	                            [Description] [nvarchar](255) NULL
                               ) 
                        END
                     ");

            #region Authorization Queries
            #region Are you sure?
            Execute.Sql($@" USE [{db}];
IF OBJECT_ID('RouletteUsers') IS NULL
begin
USE [{db}]
 

/****** Object:  Table [dbo].[RouletteUsers]    Script Date: 01/05/2020 2:55:50 PM ******/
SET ANSI_NULLS ON
 

SET QUOTED_IDENTIFIER ON
 

CREATE TABLE [dbo].[RouletteUsers](
	[Id] [nvarchar](450) NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[ClientId] [int] NULL,
 CONSTRAINT [PK_RouletteUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 
end
");

            Execute.Sql($@" USE [{db}];
IF OBJECT_ID('RouletteRoles') IS NULL
begin
                        USE [{db}]
 

/****** Object:  Table [dbo].[RouletteRoles]    Script Date: 01/05/2020 3:07:34 PM ******/
SET ANSI_NULLS ON
 

SET QUOTED_IDENTIFIER ON
 

CREATE TABLE [dbo].[RouletteRoles](
	[Id] [nvarchar](450) NOT NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[Discriminator] [nvarchar](max) NULL,
 CONSTRAINT [PK_RouletteRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 end
                     ");

            Execute.Sql($@" USE [{db}];
IF OBJECT_ID('RouletteUserRoles') IS NULL
begin
                        USE [{db}]
 

/****** Object:  Table [dbo].[RouletteUserRoles]    Script Date: 01/05/2020 2:57:12 PM ******/
SET ANSI_NULLS ON
 

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[RouletteUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_RouletteUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
 

ALTER TABLE [dbo].[RouletteUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_RouletteUserRoles_RouletteRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[RouletteRoles] ([Id])
ON DELETE CASCADE
 

ALTER TABLE [dbo].[RouletteUserRoles] CHECK CONSTRAINT [FK_RouletteUserRoles_RouletteRoles_RoleId]
 

ALTER TABLE [dbo].[RouletteUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_RouletteUserRoles_RouletteUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[RouletteUsers] ([Id])
ON DELETE CASCADE
 

ALTER TABLE [dbo].[RouletteUserRoles] CHECK CONSTRAINT [FK_RouletteUserRoles_RouletteUsers_UserId]
 end
                     ");

            Execute.Sql($@" USE [{db}];
IF OBJECT_ID('RouletteUserTokens') IS NULL
begin
                        USE [{db}]
 

/****** Object:  Table [dbo].[RouletteUserTokens]    Script Date: 01/05/2020 2:57:46 PM ******/
SET ANSI_NULLS ON
 

SET QUOTED_IDENTIFIER ON
 

CREATE TABLE [dbo].[RouletteUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_RouletteUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 end
                     ");

            Execute.Sql($@" USE [{db}];
IF OBJECT_ID('RouletteUserLogins') IS NULL
begin
                        USE [{db}]
 

/****** Object:  Table [dbo].[RouletteUserLogins]    Script Date: 01/05/2020 2:58:12 PM ******/
SET ANSI_NULLS ON
 

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[RouletteUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_RouletteUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 

ALTER TABLE [dbo].[RouletteUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_RouletteUserLogins_RouletteUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[RouletteUsers] ([Id])
ON DELETE CASCADE
 

ALTER TABLE [dbo].[RouletteUserLogins] CHECK CONSTRAINT [FK_RouletteUserLogins_RouletteUsers_UserId]
 end
                     ");

            Execute.Sql($@" USE [{db}];
IF OBJECT_ID('RouletteRoleClaims') IS NULL
begin
                       USE [{db}]
 

/****** Object:  Table [dbo].[RouletteRoleClaims]    Script Date: 01/05/2020 2:59:40 PM ******/
SET ANSI_NULLS ON
 

SET QUOTED_IDENTIFIER ON
 

CREATE TABLE [dbo].[RouletteRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_RouletteRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 

ALTER TABLE [dbo].[RouletteRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_RouletteRoleClaims_RouletteRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[RouletteRoles] ([Id])
ON DELETE CASCADE
 

ALTER TABLE [dbo].[RouletteRoleClaims] CHECK CONSTRAINT [FK_RouletteRoleClaims_RouletteRoles_RoleId]
 
end");

            Execute.Sql($@" USE [{db}];
IF OBJECT_ID('RouletteUserClaims') IS NULL
begin
                        USE [{db}]
 

/****** Object:  Table [dbo].[RouletteUserClaims]    Script Date: 01/05/2020 3:01:52 PM ******/
SET ANSI_NULLS ON
 

SET QUOTED_IDENTIFIER ON
 
CREATE TABLE [dbo].[RouletteUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_RouletteUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 

ALTER TABLE [dbo].[RouletteUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_RouletteUserClaims_RouletteUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[RouletteUsers] ([Id])
ON DELETE CASCADE
 

ALTER TABLE [dbo].[RouletteUserClaims] CHECK CONSTRAINT [FK_RouletteUserClaims_RouletteUsers_UserId]
 end
 ");
            #endregion
            #endregion
        }
        public override void Down()
        {
        }
    }
    public class DatabaseHelper
    {
        public string ConnectionString { get; protected init; }

        public DatabaseHelper(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public static void CreateIfNotExists(string connectionString)
        {
            new DatabaseHelper(connectionString).CreateIfNotExists();
        }

        public void CreateIfNotExists()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
            var databaseName = connectionStringBuilder.InitialCatalog;

            connectionStringBuilder.InitialCatalog = "master";

            using var connection = new SqlConnection(connectionStringBuilder.ToString());
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT db_id(N'{0}')", databaseName);
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                if (reader[0] != DBNull.Value) // exists
                    return;
            }

            command.CommandText = string.Format("CREATE DATABASE {0}", databaseName);
            command.ExecuteNonQuery();
        }
    }
}
