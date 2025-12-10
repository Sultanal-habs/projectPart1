const { useState, useEffect } = React;

function ArtworksGallery({ initialArtworks }) {
    console.log('ArtworksGallery initialized with:', initialArtworks);
    
    const [artworks,setArtworks] =useState(initialArtworks||[]);
    const [filteredArtworks,setFilteredArtworks]=useState(initialArtworks||[]);
    const [searchTerm,setSearchTerm]=useState('');
    const [filterType,setFilterType] =useState('');
    const [sortBy,setSortBy]=useState('newest');

    useEffect(()=>{
        console.log('Filtering artworks...');
        filterAndSortArtworks();
    },[searchTerm,filterType,sortBy,artworks]);

    const filterAndSortArtworks=()=>{
        let filtered=[];
        for(let i=0;i<artworks.length;i++){
            filtered.push(artworks[i]);
        }

        if(searchTerm){
            let result=[];
            for(let i=0;i<filtered.length;i++){
                let artwork=filtered[i];
                let titleLower=artwork.title.toLowerCase();
                let artistLower=artwork.artistName.toLowerCase();
                let descLower=artwork.description.toLowerCase();
                let searchLower=searchTerm.toLowerCase();
                
                if(titleLower.includes(searchLower)||artistLower.includes(searchLower)||descLower.includes(searchLower)){
                    result.push(artwork);
                }
            }
            filtered=result;
        }

        if(filterType){
            let result=[];
            for(let i=0;i<filtered.length;i++){
                let typeStr=filtered[i].type.toString();
                if(typeStr===filterType){
                    result.push(filtered[i]);
                }
            }
            filtered=result;
        }

        if(sortBy==='newest'){
            for(let i=0;i<filtered.length-1;i++){
                for(let j=0;j<filtered.length-i-1;j++){
                    let date1=new Date(filtered[j].createdDate);
                    let date2=new Date(filtered[j+1].createdDate);
                    if(date1<date2){
                        let temp=filtered[j];
                        filtered[j]=filtered[j+1];
                        filtered[j+1]=temp;
                    }
                }
            }
        }else if(sortBy==='oldest'){
            for(let i=0;i<filtered.length-1;i++){
                for(let j=0;j<filtered.length-i-1;j++){
                    let date1=new Date(filtered[j].createdDate);
                    let date2=new Date(filtered[j+1].createdDate);
                    if(date1>date2){
                        let temp=filtered[j];
                        filtered[j]=filtered[j+1];
                        filtered[j+1]=temp;
                    }
                }
            }
        }else if(sortBy==='mostLiked'){
            for(let i=0;i<filtered.length-1;i++){
                for(let j=0;j<filtered.length-i-1;j++){
                    if(filtered[j].likes<filtered[j+1].likes){
                        let temp=filtered[j];
                        filtered[j]=filtered[j+1];
                        filtered[j+1]=temp;
                    }
                }
            }
        }else if(sortBy==='title'){
            for(let i=0;i<filtered.length-1;i++){
                for(let j=0;j<filtered.length-i-1;j++){
                    if(filtered[j].title>filtered[j+1].title){
                        let temp=filtered[j];
                        filtered[j]=filtered[j+1];
                        filtered[j+1]=temp;
                    }
                }
            }
        }

        console.log('Filtered artworks:',filtered.length);
        setFilteredArtworks(filtered);
    };

    const getTypeLabel=(type)=>{
        let types=['Painting','Photography','Handmade Craft'];
        if(type>=0&&type<types.length){
            return types[type];
        }
        return 'Unknown';
    };

    const formatDate=(dateString)=>{
        let date=new Date(dateString);
        let options={year:'numeric',month:'short',day:'numeric'};
        return date.toLocaleDateString('en-US',options);
    };

    const handleImageError=(e)=>{
        e.target.src='/images/placeholder-artwork.svg';
        e.target.onerror=null;
    };

    console.log('Rendering with',filteredArtworks.length,'artworks');

    return React.createElement('div',{className:'artworks-gallery-container'},
        React.createElement('div',{className:'row mb-4'},
            React.createElement('div',{className:'col-md-4'},
                React.createElement('input',{
                    type:'text',
                    className:'form-control',
                    placeholder:'Search artworks...',
                    value:searchTerm,
                    onChange:(e)=>setSearchTerm(e.target.value)
                })
            ),
            React.createElement('div',{className:'col-md-3'},
                React.createElement('select',{
                    className:'form-select',
                    value:filterType,
                    onChange:(e)=>setFilterType(e.target.value)
                },
                    React.createElement('option',{value:''},'All Types'),
                    React.createElement('option',{value:'0'},'Painting'),
                    React.createElement('option',{value:'1'},'Photography'),
                    React.createElement('option',{value:'2'},'Handmade Craft')
                )
            ),
            React.createElement('div',{className:'col-md-3'},
                React.createElement('select',{
                    className:'form-select',
                    value:sortBy,
                    onChange:(e)=>setSortBy(e.target.value)
                },
                    React.createElement('option',{value:'newest'},'Newest First'),
                    React.createElement('option',{value:'oldest'},'Oldest First'),
                    React.createElement('option',{value:'mostLiked'},'Most Liked'),
                    React.createElement('option',{value:'title'},'Title (A-Z)')
                )
            ),
            React.createElement('div',{className:'col-md-2'},
                React.createElement('div',{className:'badge bg-primary p-2 w-100'},
                    `${filteredArtworks.length} Artworks`
                )
            )
        ),
        filteredArtworks.length>0?
            React.createElement('div',{className:'row g-4'},
                filteredArtworks.map((artwork)=>
                    React.createElement('div',{key:artwork.id,className:'col-md-4'},
                        React.createElement('div',{className:'card h-100'},
                            React.createElement('img',{
                                src:artwork.imageUrl,
                                className:'card-img-top',
                                alt:artwork.title,
                                style:{height:'250px',objectFit:'cover'},
                                onError:handleImageError,
                                loading:'lazy'
                            }),
                            React.createElement('div',{className:'card-body'},
                                React.createElement('h5',{className:'card-title'},artwork.title),
                                React.createElement('p',{className:'text-muted mb-2'},
                                    React.createElement('strong',null,'Artist: '),
                                    artwork.artistName
                                ),
                                React.createElement('p',{className:'card-text'},
                                    artwork.description.length>100?
                                        artwork.description.substring(0,100)+'...':
                                        artwork.description
                                ),
                                React.createElement('div',{className:'d-flex justify-content-between align-items-center mb-3'},
                                    React.createElement('span',{className:'badge bg-info'},getTypeLabel(artwork.type)),
                                    React.createElement('span',{className:'text-danger'},`${artwork.likes} likes`)
                                ),
                                React.createElement('small',{className:'text-muted d-block mb-2'},
                                    formatDate(artwork.createdDate)
                                ),
                                React.createElement('a',{
                                    href:`/ArtworkDetails/${artwork.id}`,
                                    className:'btn btn-outline-primary btn-sm w-100'
                                },'View Details')
                            )
                        )
                    )
                )
            ):
            React.createElement('div',{className:'alert alert-info text-center'},
                React.createElement('h4',null,'No artworks found'),
                React.createElement('p',null,'Try adjusting your search or filter criteria.')
            )
    );
}

document.addEventListener('DOMContentLoaded',function(){
    console.log('DOM Content Loaded');
    const container=document.getElementById('artworks-react-root');
    console.log('Container found:',container);
    
    if(container){
        const dataAttr=container.getAttribute('data-artworks');
        console.log('Data attribute:',dataAttr);
        
        try{
            let artworksData=[];
            if(dataAttr){
                artworksData=JSON.parse(dataAttr);
            }
            console.log('Parsed artworks:',artworksData);
            
            if(typeof ReactDOM!=='undefined'&&ReactDOM.createRoot){
                const root=ReactDOM.createRoot(container);
                root.render(React.createElement(ArtworksGallery,{initialArtworks:artworksData}));
                console.log('React component rendered');
            }else{
                console.error('ReactDOM not found or createRoot not available');
            }
        }catch(e){
            console.error('Error parsing or rendering:',e);
        }
    }else{
        console.error('Container artworks-react-root not found');
    }
});
