#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Pipeline.CopyFileConverterWithRuntime/Pipeline.CopyFileConverterWithRuntime.csproj", "Pipeline.CopyFileConverterWithRuntime/"]
COPY ["Pipeline.BaseConverter/Pipeline.BaseConverter.csproj", "Pipeline.BaseConverter/"]
COPY ["Pipeline.Contracts/Pipeline.Contracts.csproj", "Pipeline.Contracts/"]
COPY ["Pipeline.BaseConverterLambda/Pipeline.BaseConverterLambda.csproj", "Pipeline.BaseConverterLambda/"]
RUN dotnet restore "Pipeline.CopyFileConverterWithRuntime/Pipeline.CopyFileConverterWithRuntime.csproj"
COPY . .
WORKDIR "/src/Pipeline.CopyFileConverterWithRuntime"
RUN dotnet build "Pipeline.CopyFileConverterWithRuntime.csproj"  -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pipeline.CopyFileConverterWithRuntime.csproj" \
		-c Release \
		--runtime linux-x64 \
		--self-contained false \ 
		-o /app/publish \
		-p:PublishReadyToRun=true
		

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
ENTRYPOINT [ "/var/task/bootstrap" ]