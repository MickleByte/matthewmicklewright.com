# MatthewMicklewright.com

Personal portfolio site built with [Astro](https://astro.build/), deployed to AWS S3 + CloudFront via GitHub Actions.

## Tech Stack

- **Static site generator**: Astro 7 (content collections)
- **Styling**: Tailwind CSS v4 + `@tailwindcss/typography`
- **Hosting**: AWS S3 + CloudFront (CloudFormation in `infra/`)
- **CI/CD**: GitHub Actions (deploys on push to `master`)

## Getting Started

```bash
npm install
npm run dev      # dev server with hot reload
npm run build    # build to /build
npm run preview  # serve the built site
```

## Project Structure

```
├── src/
│   ├── content/projects/     # One folder per project: index.md + its images
│   ├── content.config.ts     # Projects collection schema
│   ├── layouts/              # Project page layout
│   ├── pages/                # Home, /projects, /projects/[slug]
│   └── styles/global.css     # Tailwind entrypoint
├── public/                   # Files copied verbatim (favicon, robots.txt…)
├── infra/                    # CloudFormation template for S3 + CloudFront
└── build/                    # Generated output (git-ignored)
```

## Adding a Project

Copy `src/content/projects/_template/` to `src/content/projects/<slug>/`. The
folder name becomes the URL (`/projects/<slug>`), and everything the project
needs lives inside it:

```
src/content/projects/qr-pub-crawl/
├── index.md
├── screenshot.png
├── architecture.png
└── qr-code.png
```

```markdown
---
title: "QR Pub Crawl"
description: "Optional one-liner shown on the project cards."
tools: "React, AWS, Python"
date: "2025-09-01"
cover: "./screenshot.png"
coverAlt: "The pub crawl app on a phone"
github: "https://github.com/MickleByte/example"   # optional
---

Body written in markdown — headings, lists, links and code blocks are styled by
the project layout.

![A description of the image](./screenshot.png)

![A description of the image](./screenshot.png "right")
```

Notes:

- **Images go next to `index.md`** and are referenced with relative paths
  (`./screenshot.png`), in both the body and `cover`. Astro then optimises and
  hashes them at build time, and a wrong path fails the build rather than
  shipping a broken image.
- **Text wraps around an image** when the path is followed by `"left"` or
  `"right"` — `![alt](./diagram.png "right")`. The marker is consumed rather
  than rendered as a tooltip, and the float is dropped on narrow screens. Any
  other title text renders as a normal image title.
- `cover` is used for the project card thumbnails and the social preview image;
  it is optional, as is everything except `title`.
- Folders starting with `_` are ignored, so `_template/` never gets published.
  The same trick works for drafts (`_wip-thing/`).
- Only put images in `public/` when they need a stable, unhashed URL.
- Projects sort newest-first by `date`; undated ones sort last.

## Deployment

Pushing to `master` triggers the GitHub Actions workflow, which builds the site and syncs the output to the S3 bucket (`matthewmicklewright.com`). CloudFront serves the content. Required secrets: `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`.
