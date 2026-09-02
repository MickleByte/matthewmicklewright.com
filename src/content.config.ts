import { defineCollection, z } from "astro:content";
import { glob } from "astro/loaders";

const projects = defineCollection({
  // Each project is a folder of its own — `projects/<slug>/index.md` plus the
  // images that project uses. The folder name is the URL slug. Folders
  // starting with `_` (e.g. `_template/`) are skipped.
  loader: glob({
    base: "./src/content/projects",
    pattern: ["**/index.md", "!**/_*/**"],
  }),

  // `image()` resolves paths relative to the markdown file and hands them to
  // Astro's asset pipeline, so a typo in a filename fails the build instead of
  // shipping a broken image.
  schema: ({ image }) =>
    z
      .object({
        title: z.string(),
        description: z.string().optional(),
        // Authored as a comma separated string in the markdown frontmatter.
        tools: z.string().default(""),
        date: z.coerce.date().optional(),
        // Thumbnail for the project cards and the social preview image.
        cover: image().optional(),
        coverAlt: z.string().optional(),
        github: z.string().url().optional(),
      })
      .transform((data) => ({
        ...data,
        // Pages render these as individual pills.
        technologies: data.tools
          .split(",")
          .map((tool) => tool.trim())
          .filter(Boolean),
      })),
});

export const collections = { projects };
