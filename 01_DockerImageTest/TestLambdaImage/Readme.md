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

where arg is <assembly-name>::<full-type-name>::<function-name>

```

## Testing image

```
curl -XPOST "http://localhost:9000/2015-03-31/functions/function/invocations" -d "\"hello world\""

```

## For moreinformation see

https://aws.amazon.com/ru/blogs/developer/net-5-aws-lambda-support-with-container-images/