#!/bin/bash
set -eu

STAGE="${1:-dev}"

echo "You are about to DELETE ALL STATIC ASSETS from ${STAGE}"
echo "If that's not what you want, press ctrl-C to kill this script"
echo "Or press enter to continue"

read

rmdir /tmp/empty
mkdir /tmp/empty
echo "Deleting original upload files from ${STAGE}..."


BUCKET_NAME=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='UploadOriginalBucket'] | [0].OutputValue" \
    --output text)


aws s3 sync --delete /tmp/empty/ "s3://${BUCKET_NAME}/"

echo "Deleting result upload files from ${STAGE}..."

BUCKET_NAME=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='UploadResultBucket'] | [0].OutputValue" \
    --output text)

aws s3 sync --delete /tmp/empty/ "s3://${BUCKET_NAME}/"

echo "Deleting result files from ${STAGE}..."

BUCKET_NAME=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='UploadResultBucket'] | [0].OutputValue" \
    --output text)

aws s3 sync --delete /tmp/empty/ "s3://${BUCKET_NAME}/"

rmdir /tmp/empty
echo "Bucket ${BUCKET_NAME} has been emptied"