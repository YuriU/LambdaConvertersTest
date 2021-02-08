#!/bin/bash
set -eu

DESTINATION="${1:-connection_params.yml}"

STAGE="${2:-dev}"

TOPIC=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='FileUploadedTopic'] | [0].OutputValue" \
    --output text)

RESULT_NOTIFICATION_QUEUE_ENDPOINT=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='ResultNotificationQueueEndpoint'] | [0].OutputValue" \
    --output text)

RESULT_NOTIFICATION_QUEUE_ARN=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='ResultNotificationQueueArn'] | [0].OutputValue" \
    --output text)

RESULT_BUCKET_NAME=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='ResultBucket'] | [0].OutputValue" \
    --output text)


UPLOAD_BUCKET_NAME=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='UploadResultBucket'] | [0].OutputValue" \
    --output text)

echo "FileUploadedTopic: ${TOPIC}
ResultNotificationQueueEndpoint: ${RESULT_NOTIFICATION_QUEUE_ENDPOINT}
ResultNotificationQueueArn: ${RESULT_NOTIFICATION_QUEUE_ARN}
OriginalBucketName: ${RESULT_BUCKET_NAME}
ResultBucketName: ${UPLOAD_BUCKET_NAME}" > ${DESTINATION}