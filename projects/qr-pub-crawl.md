---
layout: layout.html
title: "QR Pub Crawl"
tools: "React, AWS, Python"
images: ["article-2-1.png"]
---
# The Project Overview
The idea behind this project came, as many good ideas do, while on a pub crawl with some friends. I had the idea that it would be fun to be able to track your pub crawl as you go and better yet, to be able to have bragging rights over who can complete the crawl the fastest.

As a result I started thinking of ways to non-invasively track people on their pub crawls. QR codes are an approachable way for people in a pub who likely only have a smart phone to interact. The idea was that each pub on the crawl has a QR code made up of a base URL plus a query parameter which contains a unique Id for the pub e.g. "pub-crawl.co.uk?pubId=" + "12345". 

When you scan the pub crawl on your phone the UI will be loaded and it can take that pubId from the URL to determine your location. By storing another generated Id in local storage (participantId) and sending both the pubId and participantId to the backend via a RESTful API we can get a response with the list of completed pubs, uncompleted pubs, etc.

![Process Image 1](/assets/article-2-1.png)

### Design


