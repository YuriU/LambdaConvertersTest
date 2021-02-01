#!/bin/bash
set -eu

(cd ./Backend_dotnet/Pipeline.FileUploaded && ./build.sh)
(cd ./Backend_dotnet/Pipeline.CopyOriginalFile && ./build.sh)
(cd ./Backend_dotnet/Pipeline.ProcessResult && ./build.sh)
(cd ./Backend_dotnet/Pipeline.GetJobsList && ./build.sh)
(cd ./Backend_dotnet/Pipeline.GetUploadUrl && ./build.sh)
(cd ./Backend_dotnet/Pipeline.WebSockets && ./build.sh)
(cd ./Backend_dotnet/Pipeline.ProcessJobTableStream && ./build.sh)
(cd ./Backend_dotnet/Pipeline.GetDownloadUrl && ./build.sh)
(cd ./Backend_dotnet/Pipeline.DeleteJob && ./build.sh)