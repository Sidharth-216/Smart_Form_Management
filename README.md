# Smart Government Form Management & Printing Platform

A full-stack platform for digitizing, organizing, searching, previewing, downloading, and printing government forms, with AI-assisted OCR, semantic tagging, and moderation for Xerox and print shops.

## Architecture

- `frontend/` - React + Vite + Tailwind CSS + React Router + Axios
- `backend/` - .NET 8 modular monolith Web API
- `ai/` - FastAPI service for OCR, classification, and tag suggestions

## Backend Overview

The backend is split into API, Application, Domain, Infrastructure, and Shared projects. It uses MongoDB for persistence, Redis for caching and rate limiting when available, JWT for authentication, and stateless controllers to support horizontal scaling.

Key features:

- Structured browsing and search for country, state, department, and category
- Form details, creation, update, delete, and PDF upload
- Admin moderation workflow for pending uploads
- JWT authentication with role-based authorization
- Redis-backed response caching and fallback in-memory cache
- IP-based rate limiting with HTTP 429 responses
- Security headers and forwarded-header handling for proxy deployments

Important endpoints:

- `GET /api/forms`
- `GET /api/forms/facets`
- `GET /api/forms/{id}`
- `POST /api/forms`
- `PUT /api/forms/{id}`
- `DELETE /api/forms/{id}`
- `GET /api/forms/search`
- `POST /api/forms/upload`
- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/admin/uploads/pending`
- `POST /api/admin/uploads/{id}/approve`
- `POST /api/admin/uploads/{id}/reject`
- `POST /api/ai/extract-text`
- `POST /api/ai/classify`
- `POST /api/ai/suggest-tags`

## Frontend Overview

The frontend includes a searchable home page, a browsable listing page with server-driven filters and pagination, a print-ready form detail page with PDF preview, and an admin dashboard for uploads and moderation.

Implementation details:

- Lazy-loaded routes for code splitting
- Axios API layer with token injection
- Debounced search input
- Facet-driven filter sidebar
- Protected admin route handling
- PDF preview with `react-pdf`

## AI Service Overview

The AI module runs as an independent FastAPI app with OCR extraction, heuristic classification, semantic tag suggestions, and optional spaCy / sentence-transformers support. It is suitable for Docker deployment or Hugging Face Spaces.

Endpoints:

- `GET /health`
- `POST /extract-text`
- `POST /classify`
- `POST /suggest-tags`

## Environment Variables

Backend:

- `Database__ConnectionString`
- `Database__DatabaseName`
- `Redis__ConnectionString`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__SecretKey`
- `Jwt__ExpiryMinutes`
- `Storage__Provider`
- `Storage__PublicBaseUrl`
- `Storage__UploadEndpoint`
- `Storage__ApiKey`
- `Storage__ApiSecret`
- `Storage__Bucket`
- `Ai__BaseUrl`
- `Cors__AllowedOrigins`

AI:

- `AI_CORS_ORIGINS`

Frontend:

- `VITE_API_URL`

## Local Development

Backend:

1. Restore and run the solution from `backend/`.
2. Provide MongoDB and optional Redis connection strings through environment variables or appsettings overrides.
3. Open Swagger in development mode from the API project.

Frontend:

1. Install dependencies from `frontend/`.
2. Set `VITE_API_URL` to the backend API base URL.
3. Run the Vite dev server.

AI:

1. Install Python dependencies from `ai/requirements.txt`.
2. Run `uvicorn app:app --host 0.0.0.0 --port 8000` from the `ai/` directory.

## Deployment

- Backend: Docker image deployed to Render or a similar container platform
- Frontend: Vercel with environment-based API URL configuration
- AI: Docker image deployed to Hugging Face Spaces or a compatible container host

## Data Model

MongoDB collections:

- `forms` - form metadata, file URLs, keywords, languages, versioning, and latest flags
- `users` - authenticated user accounts and roles
- `uploads` - upload moderation records

## Notes

- PDF uploads are limited to 25 MB in the application layer.
- Redis is used when available; memory cache is the fallback.
- Admin registration is intentionally not exposed from the public auth endpoint.
