#!/bin/bash
set -eu

STAGE="dev";

while getopts ":s:" opt; do
  case $opt in
    s) STAGE="$OPTARG";;
  esac
done

./scripts/delete_s3_files.sh $STAGE

sls remove $@