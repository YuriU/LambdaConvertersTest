FROM public.ecr.aws/lambda/dotnet:5.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim as build
WORKDIR /src
COPY ["TestLambdaImage.csproj", "Project/"]
RUN dotnet restore "Project/TestLambdaImage.csproj"

WORKDIR "/src/Project"
COPY . .
RUN dotnet build "TestLambdaImage.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "TestLambdaImage.csproj" \
            --configuration Release \ 
            --runtime linux-x64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true

FROM base as final
WORKDIR /var/task
COPY --from=publish /app/publish .