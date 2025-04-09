// Simple script to start the Vite development server
import { spawn } from 'child_process';
import { fileURLToPath } from 'url';
import { dirname } from 'path';
import path from 'path';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

console.log('Starting Vite development server...');

// Use npx to run vite directly
const viteProcess = spawn('npx', ['vite'], {
  stdio: 'inherit',
  shell: true,
  cwd: __dirname
});

viteProcess.on('error', (error) => {
  console.error('Failed to start Vite server:', error);
});

viteProcess.on('close', (code) => {
  console.log(`Vite server process exited with code ${code}`);
});

console.log('Vite server should be running at http://localhost:5173');
console.log('Press Ctrl+C to stop the server');
