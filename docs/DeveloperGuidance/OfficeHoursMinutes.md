# Arch Office hours minutes

Summary of the discussions happening during Architecture office hours.

[[_TOC_]]

# 4/13/2022 – 6 participants
Introduced David Madrian
David explained the DDD Structure. This was [recorded](https://<Client>.webex.com/recordingservice/sites/<Client>/recording/playback/db4ea5229d65103aaff7ea5ea5510137)
Password - 6qJ3Vx7S


# 4/12/2022 – 11 participants

* Talked about Shire 2.0 and suggested the teams focus on happy path only for now. Based on some of the questions, that'll need to be reenforced.
* Talked about who would own sequence diagrams. I suggested each team own their sequence diagrams
* Talked about CR process. Mona had some links Nagendra owns that process. I suggested that the Tech leaders on the team pair up with the Arch for the first CR’s and any new patterns. 
* Norman asked how do we see the value during this transaction. We suggested several possibilities like the rating saving the ID’s of the chars that are used for what ever it’s doing. Those domains still own those id’s and the data. 
* Pedro joined – Said we’d be using Terraform for deploying assets to Azure. There will be a repo with this that devs can add resources to. 
* We talked about having an API that can publish commands that execute our code. Pedro has concerns that this would go to production which increased the surface area of the application. I suggested that it would be secured in a similar manner to other application in the semi-trusted zone and the message handler could handle this message.  Pedro suggested a test only API that didn’t go to production.

## 4/11/2022 - 5 participants

This question started the conversation:
>Regarding the question we had on Thursday for design that we are going to follow for the current user stories. We have user stories to generate characteristics for RatedGender based on some mock rules (mock data hard coded in the solution). We need to discuss the design and make sure everyone is on same page. We have stories written to build API which provide the RatedGender in response. Since we dont have clear requirements on who is the consumer of this data, we dont have sequence diagram depicted this. in order to simplify this, I was assuming this as a service and we created user stories to build a service (API) when we created the user stories. This API can internally publish message like INeedOperatorRatedGender to build the response needed for API, and that way we are not doing typical API which is tightly coupled to the data layer. Also this way we are following our solution structure for event driven arch and also leveraging the current API layer that’s available as for testing the under lying messages or event. However there could be specific requirements for this API layer for the API's that we specifically want to expose, and we may dont want to open up public endpoints for all the events that we have underlying. So I think we need to clearly discuss the approach on this. Because the way we proceeded for the current sprint, Testers were looking for an interface for testing purpose too.
>Do we want to stop building the API layer for this sprint and figure out the testing strategy for handlers?

This was my answer to _How do we test this messaging based system?_

* We don’t have a UI and we really should per domain.
* Build an API to send the Command. This is used in testing and possible future integrations
* Build an API to retrieve the result after the Command and Events have executed.
* There will not be an API for sending events. You can only send command.
* There are lots of questions about  what is supposed to be saved and what containers should be created. I've addressed this below
* I’ve asked them to create a design doc for the data access. They are not sure if they need 1 container or 2
* I suggested to the OperatorDomain that they use a single container for Operator Characteristics.  The 2 teams in the operator domain are going to work together to create this persistence. They will create a short design doc to show the flow. 
* The team is not sure what to put on the Event
* Some teams are skipping saving the data for now.

I shared the 
[Operator Characteristics Tech Spec.docx](https://<Client>365.sharepoint.com/:w:/r/sites/IndyArchitectureProjectDelivery/Shared%20Documents/PACE/Frontend%20Integration/Operator%20Characteristics%20Tech%20Spec.docx?d=w02a251973cfe411f8b8dd0cbe73090bc&csf=1&web=1&e=EidAGE)

I Showed the [wiki](https://<Client>.visualstudio.com/PACE%20and%20PURE/_wiki/wikis/PaceWiki/6867/README)

I was asked again  about when do we retry and how. My answer was that we have to address this per message.  There are many different possible reason and results from messaging. It's nice to see this conversation at the beginning of the project instead of when we go to production and its' too late to fix

I did a Demo’s Service Pulse

There questions came in later
* Are each of the development teams (Discovery Columbia, sPACE Force) responsible for their own sequence diagrams for their associated domains or is that handled at a different level? (i.e. does PACE Discovery owns OperatorRisk diagram?)

* Is there a standard in place yet for architects to be included in the code review process for implementations ? or should it be just developers approving code?

## 4/8/2022 – 15 Participants

* Talked about using Managed Identity to connect to CosmosDB.  We’ll use a secrets file to connect locally and then Managed Identity eventually in Deployed environments.
* To start with we’ll use unencrypted connection string in DV1 to get started. This connection string will be replaced later with Managed Identity. The URL is considered a secret and needs to be stored in KeyVault which also needs MI to connect to.
* A final question I have is if we code this to send managed Identity how it will run locally.
* ASB will be deployed with Powershell. We want to use ARM, Terraform, or Bicep.
* Nagendra is going to do a code review process.

## 4/7/2022 – 11 Participants

* Reviewed Suing’s Backlog

* Pedro joined and provided good perspective from Systems team
Talked about code review process.  Suing added it to his backlog. PR’s will have 2 approvers. Code Reviews will be the responsibility of the Team and the architect/code reviewer will be embedded in the team. The Arch will confirm that all patterns are approved. 

* Mona – would like a check list for the code reviewers. Mona will create a checklist or update the one that the systems team created. 
https://<Client>365.sharepoint.com/:w:/r/sites/UWSystemsPACE/_layouts/15/Doc.aspx?sourcedoc=%7B1CD29342-AAC7-49A3-95D5-6646D09E762D%7D&file=Code_Review_Process.docx&action=default&mobileredirect=true

M* ona suggested 2 levels of reviews. A lead review and an Arch review. There needs to be a process to qualify Code reviewers. For example, a lead does 5 reviews and then can be promoted to a final reviewer.  Mona suggested the CR is for learning more than actually providing quality code.  Eventually all developers can do code reviews, then Arch can double check the review so that developers are learning.

* We though Pair programming could be used for onboarding people. Once people are comfortable it could time consumer.
* WE might make a code reviews group { SIG } that can address concerns.
* Mona Entezami shared this link - https://microsoft.github.io/code-with-engineering-playbook/code-reviews/recipes/csharp/
Goal is to go to trunk based development and have full CI with automation on unit tests Mona brought up this possibility. https://medium.com/@k8spin/ephemeral-kubernetes-environments-on-ci-cd-systems-3df936a67666   Goal is to be able to get code to DV1 {or some environment} and run integration tests which don’t break everyone. 
* Pedro is going to ask security about message level auth.

## 4/7/2022 – 6 Participants

Yash asked about returning exceptions from HTTP. We discussed a pattern for having base application exceptions and then either business or technical exceptions.  Then the controller could generically map to 400’s or 500’s.   He’s going to document this so that other groups can leverage this.

We talked about a vision to use Code as Documentation. One of the concepts I think we could try is auto generate our wiki from external sources such as the code files, Azure resources, Application Insights, Splunk, and the DevOps catalog. We’d be able to paint a decent picture of how things work or track overtime if we did this. This concept is gaining trending in the IT world and would be good to explore. 

I met with sPACE Force to talk about the definition of done. They are going to draft something we can publish for all teams.
