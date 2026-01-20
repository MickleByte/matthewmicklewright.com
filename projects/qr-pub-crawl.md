---
layout: layout.html
title: "QR Pub Crawl"
tools: "React, AWS, Python"
date: "2025-09-01"
images: ["pub-crawl-1.png"]
---
# The Project Overview

This is probably the largest project I've undertaken in terms of its longevity and number of features. To build it I used:

 - React
 - Python
 - AWS Serverless Infrastructure
 - Cloudformation
 - Github Actions for CI/CD
 - Temporary MR Testing Environments

The idea behind this project came, as many good ideas do, while on a pub crawl with some friends. I had the idea that it would be fun to be able to track your pub crawl as you go and better yet, to be able to have bragging rights over who can complete the crawl the fastest.

![Screenshot](/assets/pub-crawl-1.png)

As a result I started thinking of ways to non-invasively track people on their pub crawls. QR codes are an approachable way for people in a pub who likely only have a smart phone to interact. The idea was that each pub on the crawl has a QR code made up of a base URL plus a query parameter which contains a unique Id for the pub e.g. "pub-crawl.co.uk?pubId=" + "12345". 

When you scan the pub crawl on your phone the UI will be loaded and it can take that pubId from the URL to determine your location. By storing another generated Id in local storage (participantId) and sending both the pubId and participantId to the backend via a RESTful API we can get a response with the list of completed pubs, uncompleted pubs, etc.

Try scanning the below QR to see the site in action:

![Process Image 1](/assets/article-2-1.png)

### Design

![Process Image 1](/assets/pub-crawl-2.png)

The architecture is pretty straightforward and uses serverless AWS components. The API is exposed using API gateway and calls are handled by a Python lambda which can read and write to DynamoDB. On the UI the React app is available on an S3 bucket configured for static site hosting.

## Development

As this project has grown I've implemented a few processes to help me develop new features while ensuring existing features are not affected. I'm particularly happy with the temporary MR environments that can be created: A github action monitors MRs and any Merge Requests into Main cause a new Stack to be created on AWS and automatically deploy UI and API. This allows me to test the full stack automatically each time I want to make a change