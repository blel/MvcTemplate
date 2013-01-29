create function udf_sessionSearch
 
      (@keywords nvarchar(4000))
 
returns table
 
as
 
  return (select [dbo.Employers.ID], [Rank]

            from containstable(Employers,(CompanyName, Street, ZipCode, Place),@keywords))

