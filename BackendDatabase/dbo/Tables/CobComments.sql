/*
The database must have a MEMORY_OPTIMIZED_DATA filegroup
before the memory optimized object can be created.

The bucket count should be set to about two times the 
maximum expected number of distinct values in the 
index key, rounded up to the nearest power of two.
*/

CREATE TABLE [dbo].[CobComments]
(
	[Id] INT NOT NULL PRIMARY KEY NONCLUSTERED HASH WITH (BUCKET_COUNT = 131072),
	[CaseId] int not null,
	[CommentLine] nvarchar(max),
	[CommentLineNo] int not null,
	[DateTimeStored] datetime not null,
	[ContainsCob] bit not null default 0
) WITH (MEMORY_OPTIMIZED = ON)