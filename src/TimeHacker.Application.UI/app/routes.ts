import {
    type RouteConfig,
    route,
    index,
    layout,
} from '@react-router/dev/routes';

export default [
    layout('./layout.tsx', [
        index('routes/home.tsx'),
        route('*', 'routes/not-found.tsx')
    ])
] satisfies RouteConfig;
