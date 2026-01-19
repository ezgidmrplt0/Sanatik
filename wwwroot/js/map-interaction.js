document.addEventListener('DOMContentLoaded', () => {
    const viewport = document.getElementById('viewport');
    const world = document.getElementById('world');
    
    let isDragging = false;
    let startX, startY;
    let currentX = 0;
    let currentY = 0;
    let scale = 1;
    
    // Initial centering animation
    world.style.transform = `translate(0px, 0px) scale(0.8)`;
    setTimeout(() => {
        world.style.transition = 'transform 1s cubic-bezier(0.16, 1, 0.3, 1)';
        world.style.transform = `translate(0px, 0px) scale(1)`;
        // Remove transition after expand for smooth dragging
        setTimeout(() => world.style.transition = '', 1000);
    }, 100);

    // Mouse Down - Start Drag
    document.addEventListener('mousedown', (e) => {
        // Ignore if clicking a button or link
        if (e.target.closest('a') || e.target.closest('button')) return;
        
        isDragging = true;
        startX = e.clientX - currentX;
        startY = e.clientY - currentY;
        document.body.style.cursor = 'grabbing';
    });

    // Mouse Move - Dragging
    document.addEventListener('mousemove', (e) => {
        if (!isDragging) return;
        e.preventDefault();
        
        currentX = e.clientX - startX;
        currentY = e.clientY - startY;
        
        updateTransform();
    });

    // Mouse Up - Stop Drag
    document.addEventListener('mouseup', () => {
        isDragging = false;
        document.body.style.cursor = 'grab';
    });
    
    // Wheel - Zoom
    document.addEventListener('wheel', (e) => {
        e.preventDefault();
        const delta = e.deltaY > 0 ? -0.1 : 0.1;
        scale += delta;
        scale = Math.min(Math.max(0.5, scale), 3); // Clamp scale
        updateTransform();
    }, { passive: false });

    function updateTransform() {
        world.style.transform = `translate(${currentX}px, ${currentY}px) scale(${scale})`;
    }

    // Controls
    window.resetView = () => {
        currentX = 0;
        currentY = 0;
        scale = 1;
        world.style.transition = 'transform 0.5s ease';
        updateTransform();
        setTimeout(() => world.style.transition = '', 500);
    };
    
    window.zoomIn = () => {
        scale = Math.min(scale + 0.2, 3);
        updateTransform();
    };

    window.zoomOut = () => {
        scale = Math.max(scale - 0.2, 0.5);
        updateTransform();
    };
});
