FROM node:24-alpine AS development-dependencies-env
COPY . /app
WORKDIR /app
RUN npm ci

FROM node:24-alpine AS build-env
COPY . /app/
COPY --from=development-dependencies-env /app/node_modules /app/node_modules
WORKDIR /app
RUN npm run build

FROM node:24-alpine
WORKDIR /app
# `vite preview` serves the built SPA and needs vite (a devDependency) + the config,
# so we reuse the full (dev) node_modules rather than the production-only set.
COPY ./package.json vite.config.ts /app/
COPY --from=development-dependencies-env /app/node_modules /app/node_modules
COPY --from=build-env /app/dist /app/dist
EXPOSE 5173
CMD ["npm", "run", "start"]