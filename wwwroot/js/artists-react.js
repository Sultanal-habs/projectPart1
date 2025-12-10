const { useState, useEffect } = React;

function ArtistsDirectory({ initialArtists }) {
    const [artists, setArtists] = useState(initialArtists || []);
    const [filteredArtists, setFilteredArtists] = useState(initialArtists || []);
    const [searchTerm, setSearchTerm] = useState('');
    const [sortBy, setSortBy] = useState('newest');

    useEffect(() => {
        filterAndSortArtists();
    }, [searchTerm, sortBy, artists]);

    const filterAndSortArtists = () => {
        let filtered = [...artists];

        if (searchTerm) {
            filtered = filtered.filter(artist =>
                artist.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                artist.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
                artist.bio.toLowerCase().includes(searchTerm.toLowerCase())
            );
        }

        switch (sortBy) {
            case 'newest':
                filtered.sort((a, b) => new Date(b.joinedDate) - new Date(a.joinedDate));
                break;
            case 'oldest':
                filtered.sort((a, b) => new Date(a.joinedDate) - new Date(b.joinedDate));
                break;
            case 'name':
                filtered.sort((a, b) => a.name.localeCompare(b.name));
                break;
        }

        setFilteredArtists(filtered);
    };

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short' });
    };

    const getStatusColor = (status) => {
        return status === 0 ? 'success' : 'secondary';
    };

    const getStatusLabel = (status) => {
        const statuses = ['Active', 'Inactive', 'Suspended', 'Pending'];
        return statuses[status] || 'Unknown';
    };

    const handleImageError = (e) => {
        e.target.src = '/images/artists/default.svg';
        e.target.onerror = null; // Prevent infinite loop
    };

    return React.createElement('div', { className: 'artists-directory-container' },
        React.createElement('div', { className: 'row mb-4' },
            React.createElement('div', { className: 'col-md-6' },
                React.createElement('input', {
                    type: 'text',
                    className: 'form-control',
                    placeholder: 'Search artists...',
                    value: searchTerm,
                    onChange: (e) => setSearchTerm(e.target.value)
                })
            ),
            React.createElement('div', { className: 'col-md-4' },
                React.createElement('select', {
                    className: 'form-select',
                    value: sortBy,
                    onChange: (e) => setSortBy(e.target.value)
                },
                    React.createElement('option', { value: 'newest' }, 'Newest Members'),
                    React.createElement('option', { value: 'oldest' }, 'Oldest Members'),
                    React.createElement('option', { value: 'name' }, 'Name (A-Z)')
                )
            ),
            React.createElement('div', { className: 'col-md-2' },
                React.createElement('div', { className: 'badge bg-primary p-2 w-100' },
                    `${filteredArtists.length} Artists`
                )
            )
        ),
        filteredArtists.length > 0 ?
            React.createElement('div', { className: 'row g-4' },
                filteredArtists.map((artist) =>
                    React.createElement('div', { key: artist.id, className: 'col-md-4' },
                        React.createElement('div', { className: 'card h-100' },
                            React.createElement('img', {
                                src: artist.profileImageUrl || '/images/artists/default.svg',
                                className: 'card-img-top artist-profile-img',
                                alt: artist.name,
                                onError: handleImageError,
                                loading: 'lazy'
                            }),
                            React.createElement('div', { className: 'card-body' },
                                React.createElement('h5', { className: 'card-title' }, artist.name),
                                React.createElement('p', { className: 'text-muted' }, `Joined: ${formatDate(artist.joinedDate)}`),
                                React.createElement('p', { className: 'card-text' },
                                    artist.bio.length > 80 ?
                                        artist.bio.substring(0, 80) + '...' :
                                        artist.bio
                                ),
                                React.createElement('span', { className: `badge bg-${getStatusColor(artist.status)} mb-3` },
                                    getStatusLabel(artist.status)
                                ),
                                React.createElement('hr', null),
                                React.createElement('p', { className: 'mb-1' },
                                    `Email: ${artist.email}`
                                ),
                                artist.phone && React.createElement('p', null,
                                    `Phone: ${artist.phone}`
                                )
                            )
                        )
                    )
                )
            ) :
            React.createElement('div', { className: 'alert alert-info text-center' },
                React.createElement('h4', null, 'No artists found'),
                React.createElement('p', null, 'Try adjusting your search criteria.')
            )
    );
}

document.addEventListener('DOMContentLoaded', function () {
    const container = document.getElementById('artists-react-root');
    if (container) {
        const artistsData = JSON.parse(container.getAttribute('data-artists') || '[]');
        const root = ReactDOM.createRoot(container);
        root.render(React.createElement(ArtistsDirectory, { initialArtists: artistsData }));
    }
});
