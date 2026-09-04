---
title: "matthewmicklewright.com"
tools: "Astro, JavaScript, TailwindCSS"
date: "2025-01-04"
description: "Portfolio website built with Astro and TailwindCSS"
github: "https://github.com/MickleByte/matthewmicklewright.com"
demo: "https://matthewmicklewright.com/"
#cover: "./screenshot.jpg"
---
# Project Overview


A portfolio website built with Astro and TailwindCSS. The site is designed to showcase some of my projects.

The site is built using Astro, a modern static site generator that allows for the use of multiple frameworks and languages. TailwindCSS is used for styling, providing a utility-first approach to CSS.

The site is deployed to an S3 bucket configured for static site hosting behind a CloudFront distribution. I use GitHub Actions as a CI/CD pipeline to deploy the CloudFormation template (including the S3 and CF) and then build and transfer the site. This allows me to quickly deploy changes to the site whenever I make updates.
