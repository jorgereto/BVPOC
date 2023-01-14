CREATE TABLE [dbo].[LogInstrument] (
    [LogInstrumentId] INT            IDENTITY (1, 1) NOT NULL,
    [Username]        NVARCHAR (255) NULL,
    [Date]            DATETIME2 (7)  NOT NULL,
    [Resource]        NVARCHAR (255) NOT NULL,
    [CallType]        NVARCHAR (255) NOT NULL,
    [Request]         NVARCHAR (MAX) NOT NULL,
    [Response]        NVARCHAR (MAX) NULL,
    [EllapsedMs]      INT            NOT NULL,
    CONSTRAINT [PK_LogInstrument] PRIMARY KEY CLUSTERED ([LogInstrumentId] ASC)
);


go
