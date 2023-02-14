# Smart Waitlist Application
This application will provide the ability to track wait lists and estimated time before being served. 

## Description
This application is applicable to restaurants, as well as other types of businesses. The application provides rich interaction both for the customers who are waiting, as well as the business who is managing the wait list. It is "smart" in being able to track how fast the waitlist is flowing and provide estimates for users on how long it will take before they are served.

This will be delivered as a multi-tenanted service (Software-as-a-Service or SaaS). Data will be stored in an Azure SQL database, with ASP.NET Core Blazor for a middle tier and a browser-based client. Authentication will be done using Azure Active Directory (AAD). Customers will be able to sign up to create an environment in the multi-tenanted service. Both administrators and users will access the program through the web interface.

Many of the stories and details will talk in terms of restaurants, though this application is not scoped to only restaurants. It is applicable to any business that needs to track a waitlist.

## Stakeholders and their Interests 

Stakeholders are people who care about a given software program in any way. Often different stakeholders have different needs and desires related to the software. Explicitly listing all of the stakeholders, as well as what interests drive them, helps ensure that we properly capture requirements and design the software in such a way that all stakeholder needs are met. Often times it is too easy to forget about people like the IT staff at the customer who is using the software. Or the owners of the business who need to justify the cost of the software based on time to value. Explicitly enumerating all of the stakeholders avoids these mistakes.

### The Software Developer and Maintainer
 - Managable (visibility into telemetry, ways of diagnosing and tracking, etc)
 - Low operating costs
 - Easy to deploy and change
 - Support multiple POS systems (integrations to other)
 
### Restaurant Owners
 - Cheap (cost to purchase and run)
 - Easy to acquire and get set up
 - Fast time to value 
 - High return on investment (ROI)
 - Keep customers happy
 - Reduce operational costs and the restaurant run more smoothly
 - Able to integrate to other systems

### Restaurant Customers
 - Accurate
 - Easy to use, view

### Employees (Restaurant workers)
 - Accurate
 - Fast and easy to use with little to no training
 - Simple to fix things in edge cases like a restaurant customer not answering when called, but then later showing back up
 - Easy to understand the current waitlist at a glance while having a conversation with a customer

## Personas

Personas are used in user-centered design. They are not real people, but are examples of what a typical user may look like. They are used both when writing specifications to help tell the story of how software should work, as well as to make conversations about functionality easier. Instead of describing all the usage patterns, security rights, and other aspects of a situation, you can just say "this is applicable to the Nancy persona" and everyone on the team understands the type of user it applies to and what that user expects from the software. Each persona should be one of the stakeholders of the system, most commonly users of the system.

### Phil - Restaurant Owner
Phil is 43 years old and owns the local Red Robin restaurant as a franchisee. He hopes to be one of the top restaurants in the chain with the most satisfied customers as measured by customer surveys. Phil is comfortable using computers, and runs many reports from various systems including the Point of Sale (POS) system in the course of business. He is willing to invest in his business as long as those investments return benefits.

### Nancy - Restaurant Employee

Nancy is 58 and has been working at this restaurant for the last 3 years. The other employees love working with her, and the customers love her as well. She doesn't have a strong background with computers and normally doesn't use any beyond the Point of Sale system at the restaurant. She has gotten familiar with it and has no problem using it, but doesn't consider herself a technology person. She loves ensuring everything runs smoothly at the restaurant and that customers are happy.

### Joe - Person Eating at the Restaurant
Joe is a big fan of hamburgers and loves french fries. He is a frequent restaurant guest. While he doesn't mind a wait for a table, he is not a big fan of crowds. Sometimes he dines with his family, and sometimes by himself. While he is comfortable with computers, he generally lives on his phone and uses it for all his daily activities including email, web browsing, texting, and various applications.

## User Stories

User stories help you think through how a customer will ideally interact with the software. They start with a situation, a challenge to be met, how the software helps meet that challenge, and how the user experiences a good outcome. Writing user stories helps you think through what ideal real-world experiences will be with your software, prior to getting into detailed requirements and features. Usually the same persona will have multiple user stories since they will interact with the software in multiple ways.

### User Story #1 (Joe, customer)
Joe wants to go eat supper at Red Robin. There are currently many people waiting for a table. He wants to stay in his truck until his table is ready, and doesn't even want to have to push through the crowd to get his name on the list. He goes to the Red Robin website and clicks on the "add me to waitlist button". He is redirected to the waitlist application and prompted for how many people are with him. His name then shows up on the list with and approximate wait time and his position in the list. It refreshes every few seconds until it notifies him it is his turn and he should come to the server stand.

### User Story #2 (Nancy, restaurant employee)
 A customer walks in on a busy day while Nancy is working and asks what the wait time is. Nancy looks at the point of sale system and sees the list of people currently waiting as well as a big notice at the top saying the wait time is 25 minutes for 4 people. The customer says "I have a group of 8 and we MUST sit together". She starts adding the customer to the waitlist by clicking "add" and putting in a party size of 8. The product shows them on the list with a wait time of 37 minutes. This is highlighted as being longer than other tables because of the large group size. Nancy tells the customer it will be 37 minutes based on their group size. The customer is very happy with the extremely precise answer and walks away to wait patiently.

### User Story #3 (Nancy, restaurant employee)
A free table has just opened up at the restaurant while Nancy is working. He looks at the waitlist app and calls the name of the next person on the list. There is no answer. Nancy calls again. Still no answer. So she marks "no response" on that party and moves to the next on the list. The next time there is an open table she tries again, still no response. At this point she knows that party is not around, and knows she has previously tried them based on being marked as no response. She now selects "no-show" on that party. The person in that party which had left without telling Nancy sees a notification pop up in their application that their waitlist space has been cancelled due to no response. They were already at another restaurant, so are fine with being cancelled. Nancy is happy it was easy to track the no-show as well as to notify them in a way that would help them understand what had happened.

## Use Case Diagrams

Use case diagrams are tools used to understand which users perform which activities in a given application. We have used the user stories to help elicit what many of these activities are. The activities in a use case diagram are *not* the same as user stories. A single user story about adding a party to a waitlist and eventually removing them may involve multiple activities (adding a party, removing a party, etc), as well as multiple actors (the restaurant staff, the customer, etc). To draw a use case diagram, all you need is access to stick figures, ovals, and lines. In class I demo'd using Lucid.app, a free service. You can just as easily use PowerPoint or Visio or even just a paper and pencil and then take a picture of it when done. Of course, it's best if everyone on the team can read the handwriting in the diagram. That is why I prefer the online tool.

In the diagram below we see that some activities like checking the current estimated wait time are things done by multiple users, while other actions changing the branding of the application are only applicable to one user.

![Alt text](./DocImages/Waitlist%20Use%20Case%20Diagrams.png "Use Case Diagrams")

## Requirements

### Functional

1. Worker persona must be able to specify the number of people in a party. When this is specified, the estimated wait time should reflect the new estimate immediately.
2. Worker must be able to manually add a party who previously wasn't on the list.
3. Worker must be able to manually remove a party, regardless of position on the list.
4. Worker must be able to take the "next" party, which should always be the one currently waiting.
5. Software must allow the owner persona to edit the theme. They should be able to pick colors, and logo.
6. When deleting a guest from the list, it should show in a recycle bin for at least 10 minutes before disappering.

### Non-Functional

1. Every interaction should be less than 200ms.
2. Guest personas must be able to interact unauthenticated, while worker personas require a login to be able to do admin-level functions.
3. All interactions require a simple undo.
4. Must have telemetry for monitoring and troubleshooting by the helpdesk.

### Non-Requirements
1. Requires an active internet connection at all times, not required to support offline mode.

### Public Auth Signin Process

The waitlist application will let users sign in to the waitlist without any form of signup at all in order to ensure friction-free signups. But at the same time we need to ensure other people cannot mess with other people's wait requests. To accomplish this, we will have the browser generate a secret value, and pass that to the website as its only identifier. As long as the browser remembers the secret, it can be used to both edit the request, as well as check the request status. Other people can see the request, but can not edit it without having the secret.

The process diagram shows the overall decisions made in adding a user to the waitlist.

![Alt text](./DocImages/Waitlist%20Public%20Auth%20Flowchart.png) "Public Auth Process")

The sequence diagram shows the subsystems and services involved, and the lifespan of their requests.

![Alt text](./DocImages/Waitlist%20Public%20Auth%20Sequence%20Diagram.png) "Public Auth Sequence Diagram")
