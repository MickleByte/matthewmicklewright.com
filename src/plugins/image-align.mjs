/**
 * Lets a markdown image opt into having the body text wrap around it, by
 * putting `left` or `right` in the image's title slot:
 *
 *   ![A description of the image](./screenshot.png "right")
 *
 * The title is consumed rather than rendered, so it never shows up as a
 * tooltip; any other title is left alone. The paragraph holding the image gets
 * an `img-left` / `img-right` class, which `global.css` turns into a float.
 *
 * This is a Sätteri mdast plugin (Sätteri is Astro's default markdown
 * processor). It runs before the image is handed to Astro's asset pipeline, so
 * the marker never reaches the rendered `<img>`.
 */
const SIDES = new Set(["left", "right"]);

export const imageAlign = {
  name: "image-align",

  image(node, ctx) {
    const side = node.title;

    if (!SIDES.has(side)) return;

    ctx.setProperty(node, "title", null);

    const parent = ctx.parent(node);

    // Only a paragraph that exists to hold the image is worth floating.
    if (parent?.type !== "paragraph") return;

    ctx.setProperty(parent, "data", {
      ...parent.data,
      hProperties: { ...parent.data?.hProperties, class: `img-${side}` },
    });
  },
};
