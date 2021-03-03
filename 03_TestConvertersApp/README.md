# File conversion pipeline orchestrator

The main application stack

Consists of orchestration lambdas which react to FileUploaded and ConversionProcessed events.

The orchestrator has 2 versions of backend 
1. .Net
2. NodeJs
It was originaly created with .net vesion, but because of long cold starts. NodeJs version was used as main
Frontend was created using React

To deploy run 
./deploy.sh [-s Your stage]

To create new cognito user, after deployment run
./create_user.sh -s your_stage user_name
The password during creation will be MyTestPassword!2020
Then change it on first login
