# AWS Lambda Empty Docker Image Function Project

Contains simplest logic to be used in lambda

## Building docker image

```
cd ./src/TestLambdaImage
docker build -t test_lambda_image .

```

## Running image

```
docker run --rm -p 9000:8080 test_lambda_image:latest TestLambdaImage::TestLambdaImage.Function::FunctionHandler

where arg is <assembly-name>::<full-type-name>::<function-name>

```

## Testing image locally

```
curl -XPOST "http://localhost:9000/2015-03-31/functions/function/invocations" -d "\"hello world\""

```

## Publishing image to ECR

aws ecr get-login-password --region <you_region> | docker login --username AWS --password-stdin <your_account_id>.dkr.ecr.<you_region>.amazonaws.com
docker build -t test_lambda_image .
docker tag test_lambda_image:latest <your_account_id>.dkr.ecr.<you_region>.amazonaws.com/test_lambda_image:latest
docker push <your_account_id>.dkr.ecr.<you_region>.amazonaws.com/test_lambda_image:latest


## Configure lambda

Create lambda with image <your_account_id>.dkr.ecr.<you_region>.amazonaws.com/test_lambda_image:latest
and CMD = TestLambdaImage::TestLambdaImage.Function::FunctionHandler
The rest of params leave blank
Create test event with "Hello world" string
Call lambda
Enjoy

## For moreinformation see

https://aws.amazon.com/ru/blogs/developer/net-5-aws-lambda-support-with-container-images/