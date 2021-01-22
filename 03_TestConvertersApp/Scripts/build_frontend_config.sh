#!/bin/bash
set -eu

DESTINATION="${1:-text.txt}"

STAGE="${2:-dev}"

REGION=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='Region'] | [0].OutputValue" \
    --output text)

HTTP_ENDPOINT=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='HttpEndpoint'] | [0].OutputValue" \
    --output text)

WS_ENDPOINT=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='WebSocketEndpoint'] | [0].OutputValue" \
    --output text)

USER_POOL_ID=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='UserPoolId'] | [0].OutputValue" \
    --output text)

USER_POOL_CLIENT_ID=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='UserPoolAppClientId'] | [0].OutputValue" \
    --output text)

echo ${HTTP_ENDPOINT}
echo ${WS_ENDPOINT}
echo ${REGION}
echo ${USER_POOL_ID}
echo ${USER_POOL_CLIENT_ID}

echo "const Config = {
    Region: \"${REGION}\",
    BackendHttpEndpoint: \"${HTTP_ENDPOINT}\",
    BackendWebSocketEndpoint: \"${WS_ENDPOINT}\",
    UserPoolId: \"${USER_POOL_ID}\",
    UserPoolClientId: \"${USER_POOL_CLIENT_ID}\"
}
export default Config;" > ${DESTINATION}