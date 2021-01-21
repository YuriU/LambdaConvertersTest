#!/bin/bash
set -eu

(cd ./Backend/Pipeline.FileUploaded && ./build.sh)
(cd ./Backend/Pipeline.CopyOriginalFile && ./build.sh)
(cd ./Backend/Pipeline.ProcessResult && ./build.sh)
(cd ./Backend/Pipeline.GetJobsList && ./build.sh)
(cd ./Backend/Pipeline.GetUploadUrl && ./build.sh)