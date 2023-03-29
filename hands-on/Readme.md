## Problem statement

Export Control is all about making sure you don't ship the wrong product to the wrong person and end up in prison. Various governments (aka "Jurisdictions") publish documentation on what you're allowed to do, and you're responsible for not violating those rules. One example is the US government Beureau of Industry and Security's [Export Administration Regulations](https://www.bis.doc.gov/index.php/regulations/export-administration-regulations-ear) (EAR).

For example, if you sell a product classified as [7A001](https://www.bis.doc.gov/index.php/documents/regulations-docs/2339-category-7-navigation-and-avionics-2/file) (Acclerometers as follows), it is controlled for National Security (NS), Missile Tech (MT), and Anti-Terrorism (AT). According to the [Commerce Country Chart](https://www.bis.doc.gov/index.php/documents/regulations-docs/federal-register-notices/federal-register-2014/1033-738-supp-1/file), that means you can't ship this product to Egypt without a special government license due to the NS and MT restrictions. 

So if you are [PCE Instruments](https://www.pce-instruments.com/us/) and you make an accelerometer data logger like [this one](https://www.pce-instruments.com/us/measuring-instruments/test-meters/data-logger-data-logging-instrument-pce-instruments-accelerometer-data-logger-pce-vdl-24i-3-axis-det_5857584.htm), you had better not ship one to Egypt or you'll end up on some very bad lists.

![Alt text](Accelerometer.png "Accelerometer")

### Challenge

One of the hardest things for companies to do is understand and keep up with this documentation. For instance, categories 0-9 alone are a [520-page PDF](https://www.bis.doc.gov/index.php/documents/regulations-docs/2330-ccl0-to-9-10-24-18/file) of legal jargon. So pretend you're PCE Industries and have just designed this product. How do you figure out how to classify it according to the US EAR? Once you've figured that out, then you can classify under ITAR, and under the Waasenaar arrangement, and under... well, you get the idea. A lot of governments want to vote on who can do what.

### Large Language Models

Previously there was no good way to search that government documentation, since any keyword you may use could miss a valid regulation and land you in prison. Instead, you would have humans (export control compliance officers) spend their time keeping track of the regulations and doing things manually.

Large Language Models provide a unique opportunity to drastically improve this experience. Since LLM's can understand the meaning of text, not just look for specific words, we can ask them to figure out which classifications for a given product seem to make the most sense. We still need a human making the final decision, since often a company will "interpret" the regulations to align with their businesses.

### Tools Available

1. https://chat.openai.com/chat
   (Note, this is sometimes overloaded, so not always available)
2. http://waitlistapplication.azurewebsites.net/GPT
   (Each call here costs me money, so be nice)
3. C# starter console app with key provided in class
4. Postman or tool of your choice with key provided in class

### Data Available

1. bis.json
   (JSON file containing 520 pages of PDF extracted to JSON)
2. bis.pdf
   (The original 520 page PDF from the US government)
3. ProductExamples.txt
   (Four real-world products that should be classified under US EAR, courtesy of ChatGPT)