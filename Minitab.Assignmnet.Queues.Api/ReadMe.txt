This is my 2nd implementation. The original can be found at https://github.com/FrankM9521/Minitab-Assignment
The 1st implementation followed the requirement of returning after the address is validated. 

This implementation doesn't block. It uses an Azure Queue Storage function to accomplish this. I consider this "extra-credit", I was running out
of time, so it's not as polished and because I have 2 separate projects (Web API and Azure Functions using development storage) I have no end to end testing.
Swagger is set up and that is what I used for testing. There are 2 additional endpoints to help in this:

Get By Email - Used to validate a customer was added, returns customer and address if address was stored

Clear - Used to clear the CRM stubs underlying file system

Data is stored in a Customers.txt file in the User's Documents Folder in json format


The flow was changed to

            Input JSON
               Message
Client -----------------------> API                        Azure Queue                         Queue Trigger                     USPS Service            CRM
    |                                           |                                   |                                        Function                                    |                               |
    |                                           |  Queues Message      |                                             |                                            |                               |
    |                                           |----------------------->|                                             |                                            |                               |
    |   Return Ok                        |                                    |                                             |                                            |                               |
    |<----------------------------|                                    |            Alerts Trigger            |                                            |                               |
                                                                                     | ----------------------------->|    Validate address data     |                               |
                                                                                                                           | --------------------------------->|                                |
                                                                                                                           |   Validation Result                      |                                | 
                                                                                                                           | <-------------------------------- |                                 |
                                                                                                                           |     Upsert Customer                                                     |
                                                                                                                           | -------------------------------------------------------->|


=========================================================================================

Start Up Projects

1. Minitab.Assignment.Queues.Api - REST API
2. Minitab.Assignment.Queues.Functions - Azure Functions App with Queue Trigger Function

REST API Insert Customer 
API.CustomerController.Post --> Minitab.Assignment.Queues.QueueClient.EnqueueCustomer

Functions App
Minitab.Assignment.Queues.Functions.CustomerQueueTrigger --> Minitab.Assignment.Queues.Functions.Services.Customers

The data store for the stub CRM was changed to a file source from SQL Lite

