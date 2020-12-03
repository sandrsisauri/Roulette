using FluentMigrator;
using Microsoft.Extensions.Configuration;
using Roulette.Helper;

namespace Roulette.Data.Migrations
{
    [Migration(2, "general table creating queries")]
    public class Migration_General : Migration
    {
        public Migration_General(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        private readonly IConfiguration configuration;
        public override void Up()
        {
            var connectionString = new System.Data.SqlClient.SqlConnectionStringBuilder(configuration.GetConnectionString(Const.RouletteConnectionString));
            var db = connectionString.InitialCatalog;
            #region General Queries
            Execute.Sql(@$"
                        USE [{db}];
                        GO
                            SET ANSI_NULLS ON
                            GO

                            SET QUOTED_IDENTIFIER ON
                            GO

                            CREATE TABLE [dbo].[Bets](
	                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                            [BetString] [nvarchar](250) NULL,
	                            [BetAmount] [bigint] NULL,
	                            [UserId] [nvarchar](450) NULL,
	                            [ModifiedAt] [datetime] NULL,
	                            [CreatedAt] [datetime] NULL,
                             CONSTRAINT [PK_Bets] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY]
                            GO

                            ALTER TABLE [dbo].[Bets]  WITH CHECK ADD  CONSTRAINT [FK_Bets_RouletteUsers] FOREIGN KEY([UserId])
                            REFERENCES [dbo].[RouletteUsers] ([Id])
                            GO

                            ALTER TABLE [dbo].[Bets] CHECK CONSTRAINT [FK_Bets_RouletteUsers]
                            GO
                     ");
            Execute.Sql(@$"
                        USE [{db}];
                        GO

                            SET ANSI_NULLS ON
                            GO

                            SET QUOTED_IDENTIFIER ON
                            GO

                            CREATE TABLE [dbo].[Winnings](
	                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                            [WinningNumber] [int] NULL,
	                            [WonAmount] [int] NULL,
	                            [UserId] [nvarchar](450) NULL,
	                            [CreatedAt] [datetime] NULL,
	                            [ModifiedAt] [datetime] NULL,
	                            [BetId] [int] NULL,
                             CONSTRAINT [PK_Winnings] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY]
                            GO

                            ALTER TABLE [dbo].[Winnings]  WITH CHECK ADD FOREIGN KEY([BetId])
                            REFERENCES [dbo].[Bets] ([Id])
                            GO

                            ALTER TABLE [dbo].[Winnings]  WITH CHECK ADD  CONSTRAINT [FK_Winnings_RouletteUsers] FOREIGN KEY([UserId])
                            REFERENCES [dbo].[RouletteUsers] ([Id])
                            GO

                            ALTER TABLE [dbo].[Winnings] CHECK CONSTRAINT [FK_Winnings_RouletteUsers]
                            GO

                     ");
            Execute.Sql(@$"
                        USE [{db}];
                        GO
                            SET ANSI_NULLS ON
                            GO

                            SET QUOTED_IDENTIFIER ON
                            GO

                            CREATE TABLE [dbo].[Jackpots](
	                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                            [Amount] [decimal](5, 3) NULL,
	                            [CreatedAt] [datetime] NULL,
	                            [ModifiedAt] [datetime] NULL,
	                            [BetId] [int] NULL,
                             CONSTRAINT [PK_Jackpots] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY]
                            GO

                            ALTER TABLE [dbo].[Jackpots]  WITH CHECK ADD FOREIGN KEY([BetId])
                            REFERENCES [dbo].[Bets] ([Id])
                            GO

                            ALTER TABLE [dbo].[Jackpots]  WITH CHECK ADD FOREIGN KEY([BetId])
                            REFERENCES [dbo].[Bets] ([Id])
                            GO
                     ");
            Execute.Sql(@$"
                        USE [{db}]; ALTER TABLE RouletteUsers ADD Balance DECIMAL(9, 3);");
            #endregion

        }
        public override void Down()
        {
        }
    }
}
