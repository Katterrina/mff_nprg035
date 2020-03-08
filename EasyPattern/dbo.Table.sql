CREATE TABLE [dbo].[MeasuresSets]
(
	[name] VARCHAR(50) NOT NULL PRIMARY KEY, 
    [note] VARCHAR(MAX) NULL, 
    [height] SMALLINT NOT NULL, 
    [circ_bust] SMALLINT NOT NULL, 
    [circ_waist] SMALLINT NOT NULL, 
    [cirs_hips] SMALLINT NOT NULL, 
    [len_back] SMALLINT NOT NULL, 
    [len_dress] SMALLINT NOT NULL, 
    [wid_back] SMALLINT NOT NULL, 
    [len_sleeve] SMALLINT NOT NULL, 
    [circ_neck] SMALLINT NOT NULL, 
    [len_front] SMALLINT NULL, 
    [len_breast] SMALLINT NULL, 
    [circ_sleeve] SMALLINT NOT NULL DEFAULT 22
)
