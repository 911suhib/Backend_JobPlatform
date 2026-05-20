# 1. بيئة البناء
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

# 2. انسخ كل المجلدات والمشاريع
COPY . .

# 3. توجيه الـ restore للمشروع الرئيسي مباشرة لتفادي مشكلة الـ .slnx
RUN dotnet restore "JobPlatform/JobPlatformBackend.API.csproj"

# 4. عمل الـ publish
RUN dotnet publish "JobPlatform/JobPlatformBackend.API.csproj" -c Release -o /app/out

# 5. مرحلة التشغيل النهائية الخفيفة
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "JobPlatformBackend.API.dll"]