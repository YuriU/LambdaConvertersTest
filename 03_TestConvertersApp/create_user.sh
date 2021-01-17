#!/bin/bash
set -eu

STAGE="${1:-dev}"
USER_NAME=$2

USER_POOL_ID=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='UserPoolId'] | [0].OutputValue" \
    --output text)

aws cognito-idp admin-create-user \
    --user-pool-id $USER_POOL_ID \
    --username $USER_NAME \
    --temporary-password MyTestPassword!2020 \
    --message-action SUPPRESS