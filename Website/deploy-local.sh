#!/usr/bin/env bash
PROJECT_NAME="awstechsummit.net"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"
FRONTEND_BUILD_PATH="$DIR/dist"
AWS_REGION="$(aws configure get region)"
AWS_ACCOUNT="kirk"
if [ -z "$AWS_REGION" ]; then
    AWS_REGION="us-west-2";
fi
S3_BUCKET_NAME="www.awstechsummit.net"

cd ${DIR} && \
npm run build -- --prod
aws s3 mb s3://$S3_BUCKET_NAME --region $AWS_REGION --profile $AWS_ACCOUNT || true
aws s3 website s3://$S3_BUCKET_NAME --index index.html --error index.html --profile $AWS_ACCOUNT
aws s3 rm s3://$S3_BUCKET_NAME --recursive --profile $AWS_ACCOUNT
aws s3 cp $FRONTEND_BUILD_PATH s3://$S3_BUCKET_NAME --acl public-read --recursive --profile $AWS_ACCOUNT
echo "View your project here: http://$S3_BUCKET_NAME.s3-website.$AWS_REGION.amazonaws.com"
