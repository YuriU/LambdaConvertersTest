#!/bin/bash
set -eu

STAGE="dev";

while getopts ":s:" opt; do
  case $opt in
    s) STAGE="$OPTARG";;
  esac
done

sls deploy $@
./scripts/build_frontend_config.sh ./frontend/src/config.js $STAGE