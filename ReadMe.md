# re:Invent 2018 WIN401 Breakout Session
## Architecting ASP.NET Core Microservices Applications on AWS
### Demo code for sample purposes only.

This is the code we used to create the demo app for the re:Invent 2018 break-out session "WIN401 Architecting ASP.NET Core Microservices Applications on AWS". Note that for the demo, we had each project in its own Git repo, and the solution file in another repo, to demonstrate a pattern that allows different micro-services to be developed at different velocities, and be deployed independently. 

**For Github, however, we're just putting it all in one consolidated repo to make it easier to download and look at.**

We'll update this ReadMe more later, but for now, the Visual Studio solution file is in the Solution folder. If you're not using the full Visual Studio IDE (eg, you're using VS Code, or another editor), just open the root folder.

**Notes:**
+ The DynamoDB tables, Application Load Balancer (ALB), ECS tasks and services, and other infrastructure is not included here (yet). 
+ We plan to later add a CloudFormation template that will create that infrastructure later, time permitting.
+ The slides we used in our breakout session are in PDF format in the Solution folder.

