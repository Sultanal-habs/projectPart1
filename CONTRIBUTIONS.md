# Team Contributions - Art Gallery Management System
## Project: projectPart1
**Team Members:** Sultan & Abdulla

---

## 📊 Overall Contribution Summary

| Team Member | Overall Contribution | Total Points |
|-------------|---------------------|--------------|
| **Sultan** | 50% | 10/20 |
| **Abdulla** | 50% | 10/20 |

---

## Part 1: In-Memory Implementation

### Task Distribution

| Task | Sultan | Abdulla | Description |
|------|--------|---------|-------------|
| **Project Setup & Configuration** | 50% | 50% | Initial Razor Pages setup, DI, CORS configuration in Program.cs |
| **Data Modeling** | 40% | 60% | Creating Models (Artist, Artwork, Exhibition) and Enums |
| **In-Memory DataStore** | 60% | 40% | DataStore class with seed data and initialization |
| **ArtworkService** | 70% | 30% | CRUD operations, search, filter, and sorting for artworks |
| **ArtistService** | 30% | 70% | Artist management, validation, and business logic |
| **ExhibitionService** | 60% | 40% | Exhibition operations and status management |
| **FileUploadService** | 40% | 60% | Image validation, upload, and deletion handling |
| **SortingHelper** | 60% | 40% | Generic date sorting implementation |
| **Razor Pages - Index** | 50% | 50% | Main gallery page with search and filter |
| **Razor Pages - Details** | 40% | 60% | Artwork details and like functionality |
| **Razor Pages - Add/Edit Forms** | 45% | 55% | Forms for creating and editing entities |
| **UI Styling & Layout** | 50% | 50% | CSS, Bootstrap integration, responsive design |

**Part 1 Subtotal:** Sultan 50% | Abdulla 50%

---

## Part 2: SQL Server Database Implementation

### Task Distribution

| Task | Sultan | Abdulla | Points | Description |
|------|--------|---------|--------|-------------|
| **Database Design & Planning** | 50% | 50% | 1 | ER diagram, table structure planning |
| **Create Database Script** | 50% | 50% | 0.5 | Database creation and configuration |
| **Users Table** | 40% | 60% | 0.5 | Login table with authentication fields |
| **Artists Table** | 60% | 40% | 2 | Artist profile table (from Artist.cs) |
| **Artworks Table** | 60% | 40% | 2 | Artwork catalog table (from Artwork.cs) |
| **Exhibitions Table** | 50% | 50% | 2 | Exhibition management (from Exhibition.cs) |
| **ExhibitionArtworks Table** | 50% | 50% | 0.5 | Junction table for M:M relationship |
| **ArtworkLikes Table** | 50% | 50% | 0.5 | Like tracking and engagement |
| **Primary Key Constraints** | 50% | 50% | 1 | All 6 tables with IDENTITY PKs |
| **Foreign Key Constraints** | 50% | 50% | 3 | 8 foreign keys with proper ON DELETE |
| **CHECK Constraints** | 55% | 45% | 1 | 21 check constraints for validation |
| **DEFAULT Constraints** | 45% | 55% | 1 | 14 default constraints |
| **UNIQUE Constraints** | 50% | 50% | 0.5 | 4 unique constraints |
| **NOT NULL Constraints** | 50% | 50% | 0.5 | Applied to critical fields |
| **Database Indexes** | 60% | 40% | 1 | 20+ indexes for performance |
| **vw_ArtistPortfolio** | 60% | 40% | 0.5 | Artist statistics view |
| **vw_ExhibitionDetails** | 50% | 50% | 0.5 | Exhibition metrics view |
| **vw_ArtworkGallery** | 55% | 45% | 1 | Main gallery view |
| **vw_ActiveExhibitions** | 45% | 55% | 0.5 | Available exhibitions view |
| **vw_PopularArtworks** | 50% | 50% | 0.5 | Trending artworks view |
| **Sample Data - Users** | 40% | 60% | 0.3 | 10 user records |
| **Sample Data - Artists** | 60% | 40% | 0.4 | 10 artist records |
| **Sample Data - Exhibitions** | 50% | 50% | 0.3 | 8 exhibition records |
| **Sample Data - Artworks** | 55% | 45% | 0.6 | 20 artwork records |
| **Sample Data - Relationships** | 50% | 50% | 0.4 | 45 relationship records |
| **Documentation - README** | 50% | 50% | 0.5 | Complete database documentation |
| **Documentation - ER Diagram** | 60% | 40% | 0.5 | Visual database schema |
| **Documentation - Quick Start** | 40% | 60% | 0.3 | Installation guide |
| **Testing & Verification** | 50% | 50% | 0.2 | SQL verification queries |

**Part 2 Subtotal:** Sultan 50.8% | Abdulla 49.2%

---

## Combined Project Totals (Part 1 + Part 2)

| Category | Sultan | Abdulla |
|----------|--------|---------|
| **Code Development** | 52% | 48% |
| **Database Design** | 51% | 49% |
| **Documentation** | 48% | 52% |
| **Testing & QA** | 50% | 50% |
| **Project Management** | 55% | 45% |
| **Overall** | **50%** | **50%** |

---

## 📈 Detailed Work Breakdown

### Sultan's Major Contributions

#### Part 1
- ✅ Initial project scaffolding and setup
- ✅ ArtworkService implementation (search, filter, CRUD)
- ✅ DataStore seed data creation
- ✅ SortingHelper utility class
- ✅ ExhibitionService core logic
- ✅ GitHub repository setup
- ✅ CORS configuration
- ✅ Logging infrastructure

#### Part 2
- ✅ Database schema design (Artists, Artworks tables)
- ✅ Foreign key relationships planning
- ✅ Primary key and index strategy
- ✅ CHECK constraints implementation
- ✅ vw_ArtistPortfolio view
- ✅ vw_ArtworkGallery view
- ✅ Sample artwork data creation
- ✅ ER diagram design
- ✅ GitHub commits and version control
- ✅ Performance optimization (indexes)

**Total Contribution:** 50%

---

### Abdulla's Major Contributions

#### Part 1
- ✅ Data model classes (Artist, Artwork, Exhibition)
- ✅ Validation attributes and business rules
- ✅ ArtistService implementation
- ✅ FileUploadService with validation
- ✅ Razor Pages for Add/Edit forms
- ✅ UI components and styling
- ✅ Artist profile pages
- ✅ Form validation logic

#### Part 2
- ✅ Users table design (authentication)
- ✅ Database view planning and logic
- ✅ DEFAULT constraints implementation
- ✅ Sample user data creation
- ✅ vw_ExhibitionDetails view
- ✅ vw_ActiveExhibitions view
- ✅ Documentation writing (README, QUICKSTART)
- ✅ Sample exhibition data
- ✅ Data validation rules
- ✅ Testing and verification queries

**Total Contribution:** 50%

---

## 🎯 Skills Demonstrated by Each Member

### Sultan
- Database architecture and design
- Performance optimization (indexing)
- Service layer implementation
- Business logic development
- Version control management
- System integration

### Abdulla
- Data modeling and validation
- User interface development
- Documentation and technical writing
- Authentication system design
- File handling and uploads
- Form design and validation

---

## 📅 Timeline

| Phase | Duration | Sultan Tasks | Abdulla Tasks |
|-------|----------|--------------|---------------|
| **Planning** | Week 1 | Project structure, Git setup | Requirements analysis, UI mockups |
| **Part 1 Development** | Weeks 2-3 | Services, DataStore | Models, Razor Pages |
| **Part 1 Testing** | Week 4 | Integration testing | UI testing, validation |
| **Part 2 Planning** | Week 5 | Database schema design | Table requirements, constraints |
| **Part 2 Implementation** | Week 6 | Tables, FKs, indexes | Views, sample data |
| **Part 2 Documentation** | Week 7 | ER diagrams, scripts | README, guides |

---

## 🏆 Key Achievements

### As a Team
- ✅ Fully functional art gallery application
- ✅ Complete SQL Server database with 93 records
- ✅ 6 optimized database views
- ✅ 47+ database constraints
- ✅ Comprehensive documentation (1000+ lines)
- ✅ Professional Git workflow
- ✅ Cultural authenticity (Omani context)

### Individual Excellence

**Sultan:**
- Complex service layer architecture
- Advanced SQL view design
- Performance optimization expertise

**Abdulla:**
- Comprehensive data validation
- User-friendly interface design
- Excellent documentation skills

---

## 📝 Contribution Proof

### Git Commit Distribution (Expected)

```
Sultan:   ~50% of commits
  - Database scripts
  - Service implementations
  - Core business logic
  - Performance optimization

Abdulla:  ~50% of commits
  - Model classes
  - Razor Pages
  - Documentation
  - Sample data
```

### Code Review
- Both members reviewed each other's code
- Pair programming on complex features
- Collaborative problem-solving

---

## 📧 Individual Contact

| Member | Role | Email | Strengths |
|--------|------|-------|-----------|
| **Sultan** | Developer/DB Admin | sultan@example.com | Backend, Database, Architecture |
| **Abdulla** | Developer/UI | abdulla@example.com | Frontend, Documentation, UX |

---

## 🎓 Academic Integrity Statement

We, Sultan and Abdulla, declare that:
- All work was completed by our team
- Contributions were distributed evenly (50/50)
- We collaborated on design decisions
- Individual tasks were completed independently
- All code is original or properly attributed
- No plagiarism or academic misconduct occurred

**Signatures:**
- Sultan Al-Habsi: _____________
- Abdulla Al-Saidi: _____________

**Date:** January 2024

---

**End of Contribution Report**
