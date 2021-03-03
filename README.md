# File conversion pipeline

Showcase project of file conversion pipeline

The main purpuse of the project was to build file conversion pipeline with ability to make conversion using several converters, then download combined result

# High level design

<p align="center">
  <img src="Images/Highlevel design.png">
  <br/>
</p>

The high level design is described by the picture above. 
After source file was uploaded to the pipeline converters are being notified through SNS/SQS mechanism.
The result of convertation is being published to the result SQS queue.

# Project structure:

## 01_DockerImageTest

Boilerplate example of creating Docker image used in AWS Lambda and instructions to deploy it to ECR and Lambda

## 02_Converters

Docker images of converters used in pipeline. Starting from AWS based image, to custom Docker image with added AWS Lambda runtime

## 03_TestConvertersApp

Main application, created using Serverless framework.
Contains .net and nodejs backend and frontend.
The original version of backend was created using .net core, but because of the duration of cold start which could take up to 8 seconds, it was rewriten to nodejs which made pipeline more responsible.

Frontend was created using ReactJs

## 04_LambdaConverterAppStack

Serverless stacks for AWS Lambda based converters. Each stack consists of SQS queue, subscribed to FileUploaded topic and Docker based AWS lambda
