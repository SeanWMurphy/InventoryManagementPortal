/*
The database must have a MEMORY_OPTIMIZED_DATA filegroup
before the memory optimized object can be created.

The bucket count should be set to about two times the 
maximum expected number of distinct values in the 
index key, rounded up to the nearest power of two.
*/

CREATE TABLE [dbo].[CobData]
(
	[Id] INT NOT NULL PRIMARY KEY NONCLUSTERED HASH WITH (BUCKET_COUNT = 131072),
	[ClaimNumber] varchar(12) not null,
	[ClaimType] varchar(2) not null,
	[PreviousExaminer] varchar not null,
	[BilledAmount] money,
	[QueueEntryDate] datetime not null,
	[CommentsUpdated] bit not null default 0,
	[CaseRouted] bit not null default 0	
) WITH (MEMORY_OPTIMIZED = ON)
