dotnet ef --startup-project .\TourOperator.Db --verbose migrations remove 
dotnet ef --startup-project .\TourOperator.Db --verbose migrations add base
dotnet ef --startup-project .\TourOperator.Db --verbose database update