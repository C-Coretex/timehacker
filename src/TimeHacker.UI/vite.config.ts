import { defineConfig } from 'vite';
import path from 'path';
import tailwindcss from '@tailwindcss/vite';
import react from '@vitejs/plugin-react';
import type { Plugin } from 'vite';

function spaFallbackForApp(): Plugin {
  return {
    name: 'spa-fallback-for-app',
    apply: 'serve',
    configureServer(server) {
      server.middlewares.use((req, _res, next) => {
        const url = req.url ?? '';
        if ((url === '/app' || url.startsWith('/app/')) && !url.includes('.')) {
          req.url = '/app/index.html';
        }
        next();
      });
    },
  };
}

export default defineConfig({
  plugins: [react(), tailwindcss(), spaFallbackForApp()],
  server: {
    watch: {
      // Polling stays opt-in via the env var docker-compose sets, which keeps a native
      // `npm run dev` on real fs events.
      usePolling: !!process.env.CHOKIDAR_USEPOLLING,
      interval: 1000,
      binaryInterval: 2000,
      // Vite already ignores .git, node_modules, test-results and the cache dir — these are the ones
      // it does not cover.
      ignored: ['**/obj/**', '**/.react-router/**', '**/dist/**'],
    },
  },
  resolve: {
    alias: {
      api: path.resolve(import.meta.dirname, 'src/api'),
      components: path.resolve(import.meta.dirname, 'src/components'),
      config: path.resolve(import.meta.dirname, 'src/config'),
      contexts: path.resolve(import.meta.dirname, 'src/contexts'),
      hooks: path.resolve(import.meta.dirname, 'src/hooks'),
      pages: path.resolve(import.meta.dirname, 'src/pages'),
      types: path.resolve(import.meta.dirname, 'src/types'),
      utils: path.resolve(import.meta.dirname, 'src/utils'),
      i18n: path.resolve(import.meta.dirname, 'src/i18n'),
    },
  },
  build: {
    rollupOptions: {
      input: {
        main: path.resolve(import.meta.dirname, 'index.html'),
        app: path.resolve(import.meta.dirname, 'app/index.html'),
      },
    },
  },
});
