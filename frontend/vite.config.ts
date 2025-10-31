import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";


export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
        '/api': {
            target: process.env.BACKEND_API_URL,
            changeOrigin: true,
        },
        '/hubs': {
            target: process.env.BACKEND_API_URL,
            ws: true,
            changeOrigin: true,
        }
    }
  }
});