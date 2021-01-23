#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base

# Install aws-lambda-cpp build dependencies
RUN apt-get update && \
  apt-get install -y \
  g++ \
  make \
  cmake \
  unzip \
  libcurl4-openssl-dev

WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Pipeline.CopyFileConverter/Pipeline.CopyFileConverter.csproj", "Pipeline.CopyFileConverter/"]
COPY ["Pipeline.BaseConverter/Pipeline.BaseConverter.csproj", "Pipeline.BaseConverter/"]
COPY ["Pipeline.Contracts/Pipeline.Contracts.csproj", "Pipeline.Contracts/"]
COPY ["Pipeline.BaseConverterLambda/Pipeline.BaseConverterLambda.csproj", "Pipeline.BaseConverterLambda/"]
RUN dotnet restore "Pipeline.CopyFileConverter/Pipeline.CopyFileConverter.csproj"
COPY . .
WORKDIR "/src/Pipeline.CopyFileConverter"
RUN dotnet build "Pipeline.CopyFileConverter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pipeline.CopyFileConverter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pipeline.CopyFileConverter.dll"]