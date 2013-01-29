CREATE FULLTEXT CATALOG FTECatalog as DEFAULT


GO
CREATE UNIQUE INDEX ui_Employers on dbo.Employers(ID);


CREATE FULLTEXT INDEX ON dbo.Employers
  (CompanyName, Street, Place)
  KEY INDEX ui_Employers;

GO