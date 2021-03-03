# LambdaConvertors

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