import { defineCollection, z } from "astro:content";
import { glob } from "astro/loaders";

const projects = defineCollection({
  // Astro 5+ replaced `type: "content"` with explicit loaders.
  loader: glob({ base: "./src/content/projects", pattern: "**/*.md" }),

  schema: z
    .object({
      title: z.string(),
      description: z.string().optional(),
      // Authored as a comma separated string in the markdown frontmatter.
      tools: z.string().default(""),
      date: z.coerce.date().optional(),
      images: z.array(z.string()).default([]),
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
