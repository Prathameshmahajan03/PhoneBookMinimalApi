import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],

  server: {
    allowedHosts: ['visiting-morally-composure.ngrok-free.dev'],
    proxy: {
      '/api': {
        target: 'http://localhost:5127',
        changeOrigin: true,
        secure: false
      }
    }
  }
})