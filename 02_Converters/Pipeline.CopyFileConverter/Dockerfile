#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM public.ecr.aws/lambda/dotnet:core3.1 AS base

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Pipeline.CopyFileConverter/Pipeline.CopyFileConverter.csproj", "Pipeline.CopyFileConverter/"]
COPY ["Pipeline.BaseConverter/Pipeline.BaseConverter.csproj", "Pipeline.BaseConverter/"]
COPY ["Pipeline.Contracts/Pipeline.Contracts.csproj", "Pipeline.Contracts/"]
COPY ["Pipeline.BaseConverterLambda/Pipeline.BaseConverterLambda.csproj", "Pipeline.BaseConverterLambda/"]
RUN dotnet restore "Pipeline.CopyFileConverter/Pipeline.CopyFileConverter.csproj"
COPY . .
WORKDIR "/src/Pipeline.CopyFileConverter"
RUN dotnet build "Pipeline.CopyFileConverter.csproj"  -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pipeline.CopyFileConverter.csproj" \
		-c Release \
		--runtime linux-x64 \
		--self-contained false \ 
		-o /app/publish \
		-p:PublishReadyToRun=true
		

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
CMD [ "Pipeline.CopyFileConverter::Pipeline.CopyFileConverter.CopyFileConverter::Process" ]