FROM public.ecr.aws/lambda/dotnet:5.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TestLambdaImage.csproj", ""]
RUN dotnet restore "./TestLambdaImage.csproj"
COPY . .
RUN dotnet build "TestLambdaImage.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "TestLambdaImage.csproj" -c Release -o /app/publish

# This COPY command copies the .NET Lambda project's build artifacts from the host machine into the image. 
# The source of the COPY should match where the .NET Lambda project publishes its build artifacts. If the Lambda function is being built 
# with the AWS .NET Lambda Tooling, the `--docker-host-build-output-dir` switch controls where the .NET Lambda project
# will be built. The .NET Lambda project templates default to having `--docker-host-build-output-dir`
# set in the aws-lambda-tools-defaults.json file to "bin/Release/net5.0/linux-x64/publish".
#
# Alternatively Docker multi-stage build could be used to build the .NET Lambda project inside the image.
# For more information on this approach checkout the project's README.md file.

FROM base as final
WORKDIR /var/task
COPY --from=publish /app/publish .
#COPY "bin/Release/net5.0/linux-x64/publish"  .
