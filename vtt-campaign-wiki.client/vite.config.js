import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import fs from 'fs';
import path from 'path';
import { env } from 'process';

const isProduction = process.env.NODE_ENV === 'production';

const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "vtt-campaign-wiki.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!isProduction && (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath))) {
    const result = require('child_process').spawnSync('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password'
    ], { stdio: 'inherit' });

    if (result.status !== 0) {
        throw new Error("Could not create certificate.");
    }
}

const target = 'https://localhost:7128';

export default defineConfig({
    plugins: [ vue() ],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url)),
            'vue': 'vue/dist/vue.esm-bundler.js'
        }
    },
    server: {
        proxy: {
            '^/api': {
                target,
                secure: false,
                changeOrigin: true,
                rewrite: (path) => path.replace(/^\/api/, '/api'),
                timeout: 60000,
                configure: (proxy, options) => {
                    proxy.on('proxyReq', (proxyReq, req, res) => {
                        console.log('Proxying request:', req.url);
                        proxyReq.setHeader('X-Proxy-Timeout', '60000');
                    });
                    proxy.on('error', (err, req, res) => {
                        console.error('Proxy error:', err);
                    });
                }
            }
        },
        port: 5173,
        https: !isProduction && {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
        },
        hmr: {
            timeout: 60000,
        }
    },
    build: {
        minify: 'esbuild',
        sourcemap: false,
        rollupOptions: {
            output: {
                manualChunks: {
                    vendor: [ 'vue' ]
                }
            }
        },
        commonjsOptions: {
            transformMixedEsModules: true
        }
    }
});
