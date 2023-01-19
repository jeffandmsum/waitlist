# Smart Waitlist Application
This application will provide the ability to track wait lists and estimated time before being served. 

## Description
This application is applicable to restaurants, as well as other types of businesses. The application provides rich interaction both for the customers who are waiting, as well as the business who is managing the wait list. It is "smart" in being able to track how fast the waitlist is flowing and provide estimates for users on how long it will take before they are served.

This will be delivered as a seperate web server instance for each customer who purchases the app, containing its own database, runtime layer, and UI layer.

## Stakeholders

Customers (Restaurant Onwner)
 - Cheap (cost to purchase and run)
 - Happy customers
 - Able to integrate to other systems

Me (The software developer)
 - Managable (visibility into telemetry, ways of diagnosing and tracking, etc)
 - Cheap to run and operation
 - Support multiple POS systems (integrartions to other)
 - Easy to deploy and change

Guests (Restaurant customers)
 - Accurate
 - Easy to use, view

Employees (Restaurant workers)
 - Accurate
 - Easy to use
 - Easy to maintain


## Personas

### Phil - Restaurant Owner
Phil is 43 years old and owns the local Red Robin restaurant as a franchisee. He hopes to be one of the top restaurants in the chain with the most satisfied customers as measured by customer surveys. Phil is comfortable using computers, and runs many reports from various systems including the Point of Sale (POS) system in the course of business. He is willing to invest in his business as long as those investments return benefits.

### Nancy - Restaurant Employee

Nancy is 58 and has been working at this restaurant for the last 3 years. The other employees love working with her, and the customers love her as well. She doesn't have a strong background with computers and normally doesn't use any beyond the Point of Sale system at the restaurant. She has gotten familiar with it and has no problem using it, but doesn't consider herself a technology person. She loves ensuring everything runs smoothly at the restaurant and that customers are happy.

### Jeff - Person Eating at the Restaurant
Jeff is a big fan of hamburgers and loves french fries. He is a frequent restaurant guest. While he doesn't mind a wait for a table, he is not a big fan of crowds. Sometimes he dines with his family, and sometimes by himself. While he is comfortable with computers, he generally lives on his phone and uses it for all his daily activities including email, web browsing, texting, and various applications.

## User Stories

User Story #1 (Jeff, customer)
Jeff wants to go eat supper at Red Robin. There are currently many people waiting for a table. He wants to stay in his truck until his table is ready, and doesn't even want to have to push through the crowd to get his name on the list. He goes to the Red Robin website and clicks on the "add me to waitlist button". He is redirected to the waitlist application and prompted for how many people are with him. His name then shows up on the list with and approximate wait time and his position in the list. It refreshes every few seconds until it notifies him it is his turn and he should come to the server stand.

User Story #2 (Nancy, restaurant employee)
Nancy is well versed with technology and works at Red Robin as the hostess. A customer walks in on a busy day and asks what the wait time is. Nancy looks at the point of sale system and sees the list of people currently waiting as well as a big notice at the top saying the wait time is 25 minutes for 4 people. The customer says "I have a group of 8 and we MUST sit together". She starts adding the customer to the waitlist by clicking "add" and putting in a party size of 8. The product shows them on the list with a wait time of 37 minutes. Nancy tells the customer it will be 37 minutes. The customer is very happy with the extremely precise answer and walks away to wait patiently.


## Use Case Diagrams (Lucid.app)
![Alt text](./DocImages/Waitlist%20Use%20Case%20Diagrams.jpeg "Use Case Diagrams")