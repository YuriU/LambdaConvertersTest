#!/bin/bash

# -e  Exit immediately if a command exits with a non-zero status. 
# -u  Treat unset variables as an error when substituting.
set -eu

STAGE="${1:-dev}"
echo "Deploying static assets to ${STAGE}..."


BUCKET_NAME=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='WebSiteBucket'] | [0].OutputValue" \
    --output text)

WEBSITE_URL=$(aws \
    cloudformation describe-stacks \
    --stack-name "testconvertersapp-${STAGE}" \
    --query "Stacks[0].Outputs[?OutputKey=='WebSiteUrl'] | [0].OutputValue" \
    --output text)

(cd ./frontend; npm i; npm run build)

aws s3 sync --acl 'public-read' --delete ./frontend/dist "s3://${BUCKET_NAME}/"

echo "Bucket URL: ${WEBSITE_URL}"