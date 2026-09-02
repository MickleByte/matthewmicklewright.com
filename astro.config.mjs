// @ts-check
import { defineConfig } from "astro/config";
import tailwindcss from "@tailwindcss/vite";
import { satteri } from "@astrojs/markdown-satteri";
import { imageAlign } from "./src/plugins/image-align.mjs";

// https://astro.build/config
export default defineConfig({
  site: "https://matthewmicklewright.com",

  // The GitHub Actions workflow syncs `build/` to S3, so override
  // Astro's default output directory (`dist/`) to match.
  outDir: "./build",

  // Emit `projects/erewash-rag/index.html` rather than
  // `projects/erewash-rag.html` so the S3 website endpoint can serve
  // clean URLs via its IndexDocument setting.
  build: {
    format: "directory",
  },

  markdown: {
    // `![alt](./img.png "left")` floats the image so the text wraps around it.
    processor: satteri({ mdastPlugins: [imageAlign] }),
  },

  vite: {
    plugins: [tailwindcss()],
  },
});
