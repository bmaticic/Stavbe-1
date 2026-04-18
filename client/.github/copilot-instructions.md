# Project Guidelines

## Code Style

- TypeScript with strict mode, target ES2022
- Angular 21 standalone architecture
- Formatting follows .editorconfig and Angular CLI defaults
- Reference: [tsconfig.json](tsconfig.json), [.editorconfig](.editorconfig)

## Architecture

- Three main business domains: Stavbe (Buildings), Members, Moj-Elektro (Electricity)
- Services manage state with Angular signals, no centralized store
- Functional guards and interceptors for auth, error handling, and loading
- Component boundaries follow feature folders under src/app/

## Build and Test

- Install: `npm install`
- Dev server: `npm start` (serves at http://localhost:4200/)
- Build: `npm run build`
- Test: `npm test`
- See [README.md](README.md) for full CLI commands

## Conventions

- Role-based UI with `*appHasRole` directive extracting roles from JWT
- Pagination via custom HTTP response headers (not Link header)
- Mixed form patterns: reactive (FormBuilder) and template-driven (NgForm)
- Slovenian domain terms (e.g., "stavba" for building, "merilnoMesto" for metering point)
- Auth state restored from localStorage on app initialization
- Filters managed via dedicated Params classes (e.g., StavbaParams)

Potential pitfalls:
- JWT payload extraction assumes valid token format
- Form dirty checking differs between reactive and template-driven forms
- Chart data structures are untyped (any) in Moj-Elektro components