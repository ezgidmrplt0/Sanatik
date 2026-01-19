
// 3D Starfield / Particle Background Animation
// Creates a sense of depth and 3D rotation based on mouse movement

const canvas = document.getElementById('star-canvas');
const ctx = canvas.getContext('2d');

let width, height;
let particles = [];
let mouseX = 0;
let mouseY = 0;
let targetRotationX = 0;
let targetRotationY = 0;
let currentRotationX = 0;
let currentRotationY = 0;

// Configuration
const PARTICLE_COUNT = 800;
const SPEED = 0.5;
const DEPTH = 1000;
const FOV = 600;

class Particle {
    constructor() {
        this.reset();
    }

    reset() {
        this.x = (Math.random() - 0.5) * 2000;
        this.y = (Math.random() - 0.5) * 2000;
        this.z = Math.random() * DEPTH;
        this.size = Math.random() * 2 + 0.5;
        this.color = this.getRandomColor();
    }

    getRandomColor() {
        const colors = [
            'rgba(255, 255, 255, ',
            'rgba(0, 243, 255, ', // Cyan (Accent)
            'rgba(255, 0, 85, '   // Pink (Accent)
        ];
        const base = colors[Math.floor(Math.random() * colors.length)];
        return { base: base, opacity: Math.random() * 0.5 + 0.2 };
    }

    update() {
        this.z -= SPEED; // Move particles towards viewer

        if (this.z <= 0) {
            this.reset();
            this.z = DEPTH;
        }
    }

    rotate(angleX, angleY) {
        // Rotate around Y axis
        const cosY = Math.cos(angleY);
        const sinY = Math.sin(angleY);
        let x1 = this.x * cosY - this.z * sinY;
        let z1 = this.z * cosY + this.x * sinY;

        // Rotate around X axis
        const cosX = Math.cos(angleX);
        const sinX = Math.sin(angleX);
        let y1 = this.y * cosX - z1 * sinX;
        let z2 = z1 * cosX + this.y * sinX;

        return { x: x1, y: y1, z: z2 };
    }

    draw() {
        // Apply rotation
        const p = this.rotate(currentRotationX, currentRotationY);

        // Perspective projection
        const scale = FOV / (FOV + p.z);
        const x2d = (p.x * scale) + width / 2;
        const y2d = (p.y * scale) + height / 2;

        if (scale > 0 && x2d > 0 && x2d < width && y2d > 0 && y2d < height) {
            ctx.fillStyle = this.color.base + (this.color.opacity * scale) + ')';
            ctx.beginPath();
            ctx.arc(x2d, y2d, this.size * scale, 0, Math.PI * 2);
            ctx.fill();
        }
    }
}

function init() {
    resize();
    for (let i = 0; i < PARTICLE_COUNT; i++) {
        particles.push(new Particle());
    }

    // Mouse interaction event
    document.addEventListener('mousemove', (e) => {
        // Normalize mouse position from -1 to 1
        mouseX = (e.clientX / width) * 2 - 1;
        mouseY = (e.clientY / height) * 2 - 1;

        // Set target rotation based on mouse position
        // Max rotation is roughly 15 degrees (0.26 rad)
        targetRotationY = mouseX * 0.5;
        targetRotationX = mouseY * 0.5;
    });

    window.addEventListener('resize', resize);
    animate();
}

function resize() {
    width = window.innerWidth;
    height = window.innerHeight;
    canvas.width = width;
    canvas.height = height;
}

function animate() {
    ctx.fillStyle = 'rgba(5, 5, 5, 0.3)'; // Trail effect (optional, or 'black' for clear)
    ctx.fillRect(0, 0, width, height);

    // Smooth rotation
    currentRotationX += (targetRotationX - currentRotationX) * 0.05;
    currentRotationY += (targetRotationY - currentRotationY) * 0.05;

    particles.forEach(p => {
        p.update();
        p.draw();
    });

    requestAnimationFrame(animate);
}

// Start only when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
} else {
    init();
}
