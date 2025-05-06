import { createServer } from 'http';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';
import { readFile } from 'fs/promises';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const MIME_TYPES = {
    '.html': 'text/html; charset=utf-8',
    '.js': 'application/javascript; charset=utf-8',
    '.mjs': 'application/javascript; charset=utf-8',
    '.css': 'text/css; charset=utf-8',
    '.json': 'application/json; charset=utf-8',
    '.png': 'image/png',
    '.jpg': 'image/jpeg',
    '.gif': 'image/gif',
    '.svg': 'image/svg+xml',
    '.ico': 'image/x-icon'
};

const server = createServer(async (req, res) => {
    try {
        let filePath = join(__dirname, req.url === '/' ? 'index.html' : req.url);
        const content = await readFile(filePath, 'utf8');
        const ext = filePath.split('.').pop();
        const contentType = MIME_TYPES[`.${ext}`] || 'text/plain; charset=utf-8';
        
        res.writeHead(200, {
            'Content-Type': contentType,
            'Cache-Control': 'no-cache'
        });
        res.end(content);
    } catch (error) {
        console.error('Server error:', error);
        if (error.code === 'ENOENT') {
            res.writeHead(404);
            res.end('Not Found');
        } else {
            res.writeHead(500);
            res.end('Internal Server Error');
        }
    }
});

const PORT = 3000;
server.listen(PORT, () => {
    console.log(`Server running at http://localhost:${PORT}/`);
}); 