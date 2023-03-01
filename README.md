# FX Exchange Service

## TIPs to start the project
##### 1] Create Database For Logs
##### 3] Configure FX tranasction & Logs DB connections in "appsettings.Development"
##### 4] Install "dotnet-ef" => dotnet tool install --global dotnet-ef
##### 5] Run Database update using Package Manager Console  => dotnet ef database update --startup-project APIs --project DataAccessLayer
##### 5] Configure Redis IP, User & Password in "appsettings.Development"

## Task Coverage
##### 1] When an exchange rate is used it should never be older than 30 minutes 
###### =>Covered through applying fx rate caching for 1 minute and return JWT contains the rate offer with a validaty for 29 minutes. 
##### 2] Limiting each client to 10 currency exchange trades per hour (Bonus question)
###### =>Covered through using Redis SetInc.
##### 3] Caching
###### =>Covered through using Redis
##### 4] Logging
###### =>Covered through using Serilog

