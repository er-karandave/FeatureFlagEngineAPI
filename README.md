# FeatureFlagEngineAPI
API for Feature Flag Engine

# ✅ .NET SDK
dotnet SDK: 8.0.x

# ✅ SQL Server
SQL Server: 2019 or 2022
SQL Server Management Studio (SSMS) or Azure Data Studio

# ✅ Optional but Recommended
Visual Studio 2022 and VS Code

After importing script and seeding data from script folder in sql server set db name in connection string in appsetting.json file inside the .net project

"ConnectionStrings": {
    "DefaultConnection": "Server=**localhost**;Database=FeatureFlagEngineDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  },

  // Check environment.ts
export const environment = {
  apiUrl: 'https://localhost:7220/api',  // ✅ Must match backend URL
};

Once the set up is done straight away execute the project
