/*

	This script will create the necessary schema to support the Transaction Management feature of ABus
	using SQL Server.

*/

CREATE TABLE [dbo].[ABus.TransactionManagement](
	[InboundMessageId] [varchar](50) NOT NULL,
	[OutboundMessageId] [varchar](50) NOT NULL,
	[RawMessage] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ABus.TransactionManagement] PRIMARY KEY CLUSTERED 
(
	[InboundMessageId] ASC,
	[OutboundMessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
