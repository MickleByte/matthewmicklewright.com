---
layout: layout.html
title: "The Erewash Rag"
tools: "React, AWS, LLMs"
images: ["article-1-1.webp", "article1-2.png"]
---
# Project Overview

I created [erewash-rag.co.uk](https://erewash-rag.co.uk/) primarily to annoy my friend who is the Mayor of Erewash (the Borough I live in). The original intention was to create a web scraper that collated some information on any press release/article the Borough Council produced and then pass these details to an LLM to generate a satirical news article.

As well as the direct benefit of annoying my friend, I thought it presented a good opportunity to play around with API integration with LLMs and more generally to build and deploy a solution in AWS with a web UI.

![Image 1](/assets/article-1-1.webp)

### Design

![Image 2](/assets/article-1-2.png)

The design centres around a CRUD REST API exposed with API Gateway. Calls to the endpoints are handled by a Python 'Articles' Lambda that can create/read/update/delete the Articles from DynamoDB. 

For the UI I used React to create a handful of reusable components to nicely display the article content, including the title, author, images etc. that are retrieved from the API.

Finally, population of the articles is done through a Python web scraping lambda. This used Beautiful Soup to parse the article and try and draw out keywords and information to pass into the LLM. Accuracy here is not particularly essential as the LLM tends to hallucinate and add nonsense details to the finished article regardless of the quality of the input however I found it made the articles funnier if it included names of real people/places from the original source.
The web scraper Lambda is currently just triggered manually and needs a source URL to be passed to it. An improvement would definetly be to have it run automatically, probably using some sort of daily/hourly schedule to scrape from some pre-defined locations.

### Tools & Technologies

I've built some similar CRUD APIs in the past, new additions in this project were:
 - Cloudfront for faster UI loading and https
 - A mulit-repo setup with seperate UI, API and Infra repos all deployed using Github actions for CI/CD
 - Terraform for the IaC