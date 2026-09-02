# MatthewMicklewright.com

Personal portfolio site built with [Eleventy](https://www.11ty.dev/), deployed to AWS S3 + CloudFront via GitHub Actions.

## Tech Stack

- **Static site generator**: Eleventy (11ty) v3
- **Templating**: Liquid + HTML + Markdown
- **Hosting**: AWS S3 + CloudFront
- **CI/CD**: GitHub Actions (deploys on push to `master`)

## Getting Started

```bash
npm install
npm start        # dev server with hot reload
npm run build    # build to /build
```

## Project Structure

```
├── _includes/        # Layout templates (home + project pages)
├── projects/         # Portfolio entries as Markdown files
├── css/              # Stylesheet
├── assets/           # Images and static files
├── build/            # Generated output (git-ignored)
├── index.html        # Homepage
├── portfolio.html    # Portfolio grid
└── .eleventy.js      # Eleventy config (collections, filters, passthrough)
```

## Adding a Project

Create a new Markdown file in `projects/` with the following frontmatter:

```markdown
---
title: Project Name
date: 2025-01-01
role: Developer
tech: JavaScript, AWS
image: /assets/my-image.png
---

Project description here...
```

Projects are sorted newest-first in the portfolio collection.

## Deployment

Pushing to `master` triggers the GitHub Actions workflow, which builds the site and syncs the output to the S3 bucket (`matthewmicklewright.com`). CloudFront serves the content. Required secrets: `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`.
