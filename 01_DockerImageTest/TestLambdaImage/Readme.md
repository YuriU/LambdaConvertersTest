# AWS Lambda Empty Docker Image Function Project

Contains simplest logic to be used in lambda

## Building docker image

```
cd ./src/TestLambdaImage
docker build -t awsimage .

```

## Running image

```
docker run --rm -p 9000:8080 awsimage:latest TestLambdaImage::TestLambdaImage.Function::FunctionHandler

```

