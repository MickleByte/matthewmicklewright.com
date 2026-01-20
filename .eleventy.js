module.exports = function(eleventyConfig) {
  // Copy CSS and other static assets
  eleventyConfig.addPassthroughCopy("css");
  eleventyConfig.addPassthroughCopy("assets");

  // Add safe filter for Liquid templates
  eleventyConfig.addLiquidFilter("safe", function(value) {
    return value;
  });

  // Add filter to get first image from images array
  eleventyConfig.addLiquidFilter("firstImage", function(images) {
    if (!images || !Array.isArray(images) || images.length === 0) {
      return null;
    }
    // Return the first image with /assets/ prefix
    return "/assets/" + images[0];
  });

  // Create a collection for portfolio articles
  eleventyConfig.addCollection("portfolio", function(collectionApi) {
    return collectionApi.getFilteredByGlob("projects/*.md").sort((a, b) => {
      // Sort by date, newest first
      return (b.data.date || '') > (a.data.date || '') ? 1 : -1;
    });
  });

  return {
    dir: {
      input: ".",
      output: "build",
      includes: "_includes",
      layouts: "_includes"
    },
    templateFormats: ["html", "md", "liquid"],
    htmlTemplateEngine: "liquid",
    markdownTemplateEngine: "liquid"
  };
};

