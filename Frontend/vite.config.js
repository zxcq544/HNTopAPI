import { defineConfig } from "vite";
import { svelte } from "@sveltejs/vite-plugin-svelte";
import { viteStaticCopy } from "vite-plugin-static-copy";

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        svelte(),
        viteStaticCopy({
            targets: [
                {
                    src: "./dist/*",
                    dest: "../../wwwroot",
                },
            ],
        }),
    ],
    server: {
        proxy: {
            "/api": {
                target: "http://localhost:8085/api",
                changeOrigin: true,
                secure: false,
                rewrite: (path) => path.replace(/^\/api/, ""),
            },
        },
    },
});
