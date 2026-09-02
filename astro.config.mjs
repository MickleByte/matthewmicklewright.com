// @ts-check
import { defineConfig } from "astro/config";
import tailwindcss from "@tailwindcss/vite";

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

  vite: {
    plugins: [tailwindcss()],
  },
});
