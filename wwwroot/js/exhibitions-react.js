const { useState, useEffect } = React;

function ExhibitionsGallery({ initialExhibitions }) {
    const [exhibitions, setExhibitions] = useState(initialExhibitions || []);
    const [filteredExhibitions, setFilteredExhibitions] = useState(initialExhibitions || []);
    const [searchTerm, setSearchTerm] = useState('');
    const [filterStatus, setFilterStatus] = useState('');
    const [sortBy, setSortBy] = useState('startDate');

    useEffect(() => {
        filterAndSortExhibitions();
    }, [searchTerm, filterStatus, sortBy, exhibitions]);

    const filterAndSortExhibitions = () => {
        let filtered = [];
        for(let i=0;i<exhibitions.length;i++){
            filtered.push(exhibitions[i]);
        }

        if (searchTerm) {
            let result=[];
            for(let i=0;i<filtered.length;i++){
                let exhibition=filtered[i];
                let nameLower=exhibition.name.toLowerCase();
                let descLower=exhibition.description.toLowerCase();
                let locLower=exhibition.location.toLowerCase();
                let searchLower=searchTerm.toLowerCase();
                
                if(nameLower.includes(searchLower)||descLower.includes(searchLower)||locLower.includes(searchLower)){
                    result.push(exhibition);
                }
            }
            filtered=result;
        }

        if (filterStatus) {
            let result=[];
            for(let i=0;i<filtered.length;i++){
                let statusStr=filtered[i].status.toString();
                if(statusStr===filterStatus){
                    result.push(filtered[i]);
                }
            }
            filtered=result;
        }

        if(sortBy==='startDate'){
            for(let i=0;i<filtered.length-1;i++){
                for(let j=0;j<filtered.length-i-1;j++){
                    let date1=new Date(filtered[j].startDate);
                    let date2=new Date(filtered[j+1].startDate);
                    if(date1<date2){
                        let temp=filtered[j];
                        filtered[j]=filtered[j+1];
                        filtered[j+1]=temp;
                    }
                }
            }
        }else if(sortBy==='endDate'){
            for(let i=0;i<filtered.length-1;i++){
                for(let j=0;j<filtered.length-i-1;j++){
                    let date1=new Date(filtered[j].endDate);
                    let date2=new Date(filtered[j+1].endDate);
                    if(date1>date2){
                        let temp=filtered[j];
                        filtered[j]=filtered[j+1];
                        filtered[j+1]=temp;
                    }
                }
            }
        }else if(sortBy==='name'){
            for(let i=0;i<filtered.length-1;i++){
                for(let j=0;j<filtered.length-i-1;j++){
                    if(filtered[j].name>filtered[j+1].name){
                        let temp=filtered[j];
                        filtered[j]=filtered[j+1];
                        filtered[j+1]=temp;
                    }
                }
            }
        }

        setFilteredExhibitions(filtered);
    };

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
    };

    const getStatusColor = (status) => {
        const colors = ['primary', 'success', 'secondary', 'danger'];
        if(status>=0&&status<colors.length){
            return colors[status];
        }
        return 'secondary';
    };

    const getStatusLabel = (status) => {
        const statuses = ['Upcoming', 'Active', 'Ended', 'Cancelled'];
        if(status>=0&&status<statuses.length){
            return statuses[status];
        }
        return 'Unknown';
    };

    const getDaysRemaining = (startDate, endDate, status) => {
        const now = new Date();
        const start = new Date(startDate);
        const end = new Date(endDate);

        if (status === 1) {
            let timeDiff=end-now;
            let daysDiff=Math.ceil(timeDiff/(1000*60*60*24));
            return `${daysDiff} days remaining`;
        } else if (status === 0) {
            let timeDiff=start-now;
            let daysDiff=Math.ceil(timeDiff/(1000*60*60*24));
            return `Starts in ${daysDiff} days`;
        }
        return '';
    };

    return React.createElement('div', { className: 'exhibitions-gallery-container' },
        React.createElement('div', { className: 'row mb-4' },
            React.createElement('div', { className: 'col-md-4' },
                React.createElement('input', {
                    type: 'text',
                    className: 'form-control',
                    placeholder: 'Search exhibitions...',
                    value: searchTerm,
                    onChange: (e) => setSearchTerm(e.target.value)
                })
            ),
            React.createElement('div', { className: 'col-md-3' },
                React.createElement('select', {
                    className: 'form-select',
                    value: filterStatus,
                    onChange: (e) => setFilterStatus(e.target.value)
                },
                    React.createElement('option', { value: '' }, 'All Exhibitions'),
                    React.createElement('option', { value: '0' }, 'Upcoming'),
                    React.createElement('option', { value: '1' }, 'Active'),
                    React.createElement('option', { value: '2' }, 'Ended')
                )
            ),
            React.createElement('div', { className: 'col-md-3' },
                React.createElement('select', {
                    className: 'form-select',
                    value: sortBy,
                    onChange: (e) => setSortBy(e.target.value)
                },
                    React.createElement('option', { value: 'startDate' }, 'Start Date'),
                    React.createElement('option', { value: 'endDate' }, 'End Date'),
                    React.createElement('option', { value: 'name' }, 'Name (A-Z)')
                )
            ),
            React.createElement('div', { className: 'col-md-2' },
                React.createElement('div', { className: 'badge bg-primary p-2 w-100' },
                    `${filteredExhibitions.length} Exhibitions`
                )
            )
        ),
        filteredExhibitions.length > 0 ?
            React.createElement('div', null,
                filteredExhibitions.map((exhibition) =>
                    React.createElement('div', { key: exhibition.id, className: 'card mb-4' },
                        React.createElement('div', { className: 'row g-0' },
                            React.createElement('div', { className: 'col-md-4' },
                                React.createElement('img', {
                                    src: exhibition.bannerImageUrl,
                                    className: 'exhibition-banner',
                                    alt: exhibition.name
                                })
                            ),
                            React.createElement('div', { className: 'col-md-8' },
                                React.createElement('div', { className: 'card-body' },
                                    React.createElement('div', { className: 'd-flex justify-content-between' },
                                        React.createElement('h4', null, exhibition.name),
                                        React.createElement('span', { className: `badge bg-${getStatusColor(exhibition.status)}` },
                                            getStatusLabel(exhibition.status)
                                        )
                                    ),
                                    React.createElement('p', null, exhibition.description),
                                    React.createElement('p', null,
                                        React.createElement('strong', null, 'From: '),
                                        formatDate(exhibition.startDate),
                                        React.createElement('strong', null, ' To: '),
                                        formatDate(exhibition.endDate)
                                    ),
                                    React.createElement('p', null,
                                        React.createElement('strong', null, 'Location: '),
                                        exhibition.location
                                    ),
                                    React.createElement('p', null,
                                        React.createElement('strong', null, 'Max Artworks: '),
                                        exhibition.maxArtworks
                                    ),
                                    exhibition.status <= 1 &&
                                    React.createElement('div', { className: `alert alert-${exhibition.status === 1 ? 'success' : 'info'} mt-2` },
                                        getDaysRemaining(exhibition.startDate, exhibition.endDate, exhibition.status)
                                    )
                                )
                            )
                        )
                    )
                )
            ) :
            React.createElement('div', { className: 'alert alert-info text-center' },
                React.createElement('h4', null, 'No exhibitions found'),
                React.createElement('p', null, 'Try adjusting your search or filter criteria.')
            )
    );
}

document.addEventListener('DOMContentLoaded', function () {
    const container = document.getElementById('exhibitions-react-root');
    if (container) {
        let exhibitionsData=[];
        const dataAttr=container.getAttribute('data-exhibitions');
        if(dataAttr){
            exhibitionsData=JSON.parse(dataAttr);
        }
        const root = ReactDOM.createRoot(container);
        root.render(React.createElement(ExhibitionsGallery, { initialExhibitions: exhibitionsData }));
    }
});
