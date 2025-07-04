# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is the **Sistema Acervo Educacional Ferreira Costa** - a complete educational content management system with corporate identity, built using Clean Architecture principles. The system is 100% functional and production-ready.

## Architecture

The project follows Clean Architecture with clear separation of concerns:

### Backend (.NET 8)
- **Domain Layer**: Entities, enums, and business rules
- **Application Layer**: 7 complete services (Auth, User, Course, File, Report, Email, Security)
- **Infrastructure Layer**: Repositories, DbContext, external service integrations
- **WebApi Layer**: 5 controllers with 35+ REST endpoints

### Frontend (React 19)
- Modern React application with Vite build tool
- TailwindCSS with Ferreira Costa corporate identity
- Responsive design with corporate branding
- Complete user interface with dashboard, kanban, and management pages

## Development Commands

### Backend
```bash
# Navigate to backend
cd backend/AcervoEducacional.WebApi

# Restore dependencies
dotnet restore

# Run development server
dotnet run
# API available at: http://localhost:5000
# Swagger docs at: http://localhost:5000/swagger

# Run tests
cd ../AcervoEducacional.Application.Tests
dotnet test
```

### Frontend
```bash
# Navigate to frontend
cd frontend

# Install dependencies
npm install

# Run development server
npm run dev
# Frontend available at: http://localhost:5173

# Build for production
npm run build

# Run linter
npm run lint
```

### Database
- PostgreSQL is used as the primary database
- Entity Framework Core handles migrations automatically
- Connection string should be configured in appsettings.json

## Key Technologies

### Backend Stack
- .NET 8 with C#
- Entity Framework Core for ORM
- PostgreSQL for database
- JWT Bearer Authentication
- BCrypt for password hashing
- Swagger/OpenAPI for documentation
- Hangfire for background jobs
- AspNetCoreRateLimit for rate limiting

### Frontend Stack
- React 19.1.0
- Vite 6.3.5 for build tooling
- TailwindCSS 4.1.7 for styling
- Radix UI components for accessibility
- React Router DOM for navigation
- Recharts for data visualization
- Framer Motion for animations

## Code Architecture Patterns

### Services Pattern
All business logic is contained in service classes in the Application layer:
- `AuthService`: JWT authentication, login, token refresh
- `UsuarioService`: User management, CRUD operations
- `CursoService`: Course management with status tracking
- `ArquivoService`: File upload, sharing, AWS S3 integration
- `ReportService`: Statistics, exports, dashboard data
- `EmailService`: SMTP integration with corporate templates
- `SecurityService`: Security validations and audit logging

### Repository Pattern
Generic repository pattern with specific implementations:
- `BaseRepository<T>`: Generic CRUD operations
- Specific repositories for each entity with custom queries
- Unit of Work pattern for transaction management

### Controller Pattern
RESTful API controllers with consistent response format:
- Standard HTTP status codes
- Consistent error handling via middleware
- JWT authentication on protected endpoints
- Swagger documentation for all endpoints

## Security Implementation

### Authentication
- JWT tokens with 1-hour expiration
- Refresh tokens with 7-day expiration
- Rate limiting on authentication endpoints
- Password strength validation with BCrypt

### Authorization
- Role-based access control (Admin, Gestor, Usuario)
- Resource-based authorization for user data
- Middleware for automatic token validation

### Data Protection
- Input sanitization and validation
- SQL injection prevention via EF Core
- XSS protection with input filtering
- Audit logging for all sensitive operations

## Database Schema

Main entities:
- `Usuario`: User management with roles and status
- `Curso`: Course tracking with 10 status levels
- `Arquivo`: File management with metadata
- `LogAtividade`: Audit trail for all operations

## Corporate Identity

The system implements complete Ferreira Costa corporate branding:
- **Colors**: Red (#DC2626), Green (#16A34A), Orange (#F59E0B), Blue (#2563EB)
- **Typography**: Barlow font family (Google Fonts)
- **Logo**: Custom favicon and brand elements
- **Templates**: Corporate email templates

## Testing

### Backend Tests
- xUnit framework with FluentAssertions
- Moq for mocking dependencies
- Test coverage includes security validations
- Run with: `dotnet test`

### Test Structure
- Unit tests for services and validation logic
- Integration tests for API endpoints
- Security tests for authentication and authorization

## Common Development Tasks

### Adding New Features
1. Create entities in Domain layer
2. Add repositories in Infrastructure layer
3. Implement services in Application layer
4. Create controllers in WebApi layer
5. Add frontend components and pages
6. Update documentation and tests

### Database Changes
1. Modify entities in Domain layer
2. Add migration: `dotnet ef migrations add MigrationName`
3. Update database: `dotnet ef database update`

### API Documentation
- Swagger is automatically generated from controller attributes
- Access documentation at `/swagger` endpoint
- Update XML comments for better documentation

## Environment Configuration

### Required Environment Variables
- `DATABASE_URL`: PostgreSQL connection string
- `JWT_SECRET`: Secret key for JWT token signing
- `JWT_ISSUER`: JWT token issuer
- `JWT_AUDIENCE`: JWT token audience
- `EMAIL_SMTP_*`: SMTP configuration for email service

### Development vs Production
- Development uses local PostgreSQL instance
- Production requires proper secret management
- CORS configuration differs between environments

## Deployment

### Docker Support
- Backend has complete Dockerfile
- Frontend has Nginx-based Dockerfile
- docker-compose.yml for local development

### Production Considerations
- Enable HTTPS in production
- Configure proper CORS origins
- Set up proper logging and monitoring
- Configure rate limiting appropriately

## Important Notes

- The system is currently 100% functional and production-ready
- All 35+ API endpoints are implemented and documented
- Corporate identity is fully integrated throughout
- Security features are implemented but may need production hardening
- Test coverage exists but could be expanded for production use

## Documentation

Additional documentation available in the `docs/` folder:
- `PROJETO-FINALIZADO.md`: Project completion documentation
- `API-REST-Documentation.md`: Complete API documentation
- `GUIA-DESENVOLVIMENTO.md`: Detailed development guide
- `GUIA-IMPLEMENTACAO.md`: Implementation guide

## Support

For development questions or issues:
- Check existing documentation in `docs/` folder
- Review Swagger documentation at `/swagger`
- Examine test files for implementation examples
- Follow Clean Architecture principles for new features