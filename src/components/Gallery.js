import React from 'react';
import '../styles/Gallery.css';

function Gallery() {
    const images = [
        'https://picsum.photos/id/227/100/200',
        'https://picsum.photos/id/217/100/200',
        'https://picsum.photos/id/247/100/200',
        'https://picsum.photos/id/257/100/200',
        'https://picsum.photos/id/267/100/200',
        'https://picsum.photos/id/277/100/200',
        'https://picsum.photos/id/237/200/200',
        'https://picsum.photos/id/237/300/400',
        'https://picsum.photos/id/169/150/200',
        'https://picsum.photos/id/269/250/300',
    ];

    return (
        <section className="gallery">
            <h2>Galéria</h2>
            <div className="gallery-images">
                {images.map((image, index) => (
                    <div className="image-item" key={index}>
                        <img src={image} alt={`Galéria kép ${index + 1}`} />
                    </div>
                ))}
            </div>
        </section>
    );
}

export default Gallery;
