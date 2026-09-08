

/** @type { import('@storybook/react-vite').StorybookConfig } */
const config = {
  "stories": [
    "../src/**/*.mdx",
    "../src/**/*.stories.@(js|jsx|mjs|ts|tsx)"
  ],
  "addons": [
    "@chromatic-com/storybook",
    "@storybook/addon-vitest",
    "@storybook/addon-a11y",
    "@storybook/addon-docs",
    "@storybook/addon-mcp"
  ],
  "framework": "@storybook/react-vite",
  async viteFinal(config) {
    // vite-plugin-pwa's generateSW step fails on Storybook's own build output
    // (unrelated service worker precache), so it's excluded here.
    config.plugins = config.plugins?.filter(
      (plugin) => !(Array.isArray(plugin) ? plugin : [plugin]).some((p) => p?.name?.startsWith('vite-plugin-pwa'))
    );
    return config;
  },
};
export default config;