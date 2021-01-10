FROM public.ecr.aws/lambda/dotnet:5.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim as build
WORKDIR /src
COPY ["Converter.csproj", "Project/"]
RUN dotnet restore "Project/Converter.csproj"

WORKDIR "/src/Project"
COPY . .
RUN dotnet build "Converter.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "Converter.csproj" \
            --configuration Release \ 
            --runtime linux-x64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true

FROM base as final
WORKDIR /var/task
COPY --from=publish /app/publish .