# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Stack

- **Backend** `vtt-campaign-wiki.Server/` — ASP.NET Core 8 + [FastEndpoints](https://fast-endpoints.com/) + EF Core (SQLite) + ASP.NET Identity + JWT bearer + Mapster.
- **Frontend** `vtt-campaign-wiki.client/` — Vue 3 + Vite + Vuetify 3 + Pinia (persisted) + Vue Router + vue-quill (rich-text) + axios.
- Single VS solution `vtt-campaign-wiki.sln` ties them together via SpaProxy.

## Common commands

Run from the repo root unless noted.

**Backend** (cwd `vtt-campaign-wiki.Server/`):
```pwsh
dotnet run                                  # https://localhost:7128, swagger at /swagger; SpaProxy auto-launches `npm run dev` for the client
dotnet build
dotnet ef migrations add <Name>             # uses VttCampaignWikiDbContextFactory (reads appsettings.json)
dotnet ef database update
```

**Frontend** (cwd `vtt-campaign-wiki.client/`):
```pwsh
npm install
npm run dev      # vite on https://localhost:5173, proxies /api -> https://localhost:7128
npm run build
npm run lint     # eslint --fix on .vue/.js/.jsx/.cjs/.mjs
```

There is no test project in this repo.

**HTTPS dev certs**: `vite.config.js` shells out to `dotnet dev-certs https --export-path ...` on first run; if vite errors at startup with a cert problem, run `dotnet dev-certs https --trust` manually.

## Architecture

### Vertical slices

Server code is organized **by feature**, not by layer. Each `Features/<Name>/` contains its own entities, DTOs, endpoints, and services:

```
Features/
  Campaign/   Endpoints/<Action>/<Action>Endpoint.cs + <Action>Request.cs ; Services/Repository ; CampaignEntity.cs ; CampaignDto.cs ; Utilities/
  Player/     (PlayerEntity is the ASP.NET Identity user, Id : int)
  Session/    Sessions belong to a campaign; many-to-many with players via SessionPlayerEntity
  Image/      ItemImageEntity stores blob + content-type; ImageHelper extracts from IFormFile
  AI/         SuggestedItemsService — OpenAI chat-completions with json_schema; reads OpenAI:ApiKey from config
  Shared/     ItemBaseEntity (Title/Content/Position/Author/Image), IRepositoryBase<T>, RepositoryBase<T>, Pagination
```

When adding an endpoint: create `Features/<Feature>/Endpoints/<Action>/<Action>Endpoint.cs` deriving from `Endpoint<TReq, TResp>` (or `EndpointWithoutRequest<TResp>`), set the route in `Configure()`, and call `req.Adapt<Entity>()` / `entity.Adapt<Dto>()` for mapping (Mapster). FastEndpoints auto-discovers via `app.UseFastEndpoints()` — no manual registration.

### RepositoryBase<T> conventions (Shared/Services/RepositoryBase.cs)

Generic base implements pagination, sort, search, and shared business rules that subclasses inherit. Things to know before extending it:

- **Author + auth tracking is implicit.** On `AddAsync`, if `T : ItemBaseEntity`, the base sets `Author`/`AuthorId` from `PlayerProvider.GetCurrentPlayer()`. On `DeleteAsync`, the same path enforces "only the author can delete." Don't reimplement this in subclasses — call `base.AddAsync` / `base.DeleteAsync`.
- **`ApplySearch` is reflection-driven**: it `Contains`-filters on a `Title` string property if one exists.
- **Default sort** is by `Position` (decimal, stored as `decimal(18,5)`) if the property exists, otherwise unsorted.
- **`SanitizeEntity`** replaces any string property equal to `<p><br></p>` with empty string — that string is Quill's "empty editor" sentinel.

### Hierarchical campaign items + position math

`CampaignItemEntity` is self-referential (`ParentEntityId` → `CampaignItemEntity`) and ordered by a sparse decimal `Position`. See `Features/Shared/Constants/ItemBase.cs`:

- `POSITION_GAP = 5000` — new siblings are appended at `max(position) + GAP`.
- `POSITION_THRESHOLD = 10` — when re-parenting/reordering produces a position < threshold (positions colliding from repeated bisection), `CampaignItemRepository.RespreadPositionValuesAsync` re-spaces *all* siblings under that parent at multiples of `GAP`.

`GetByIdAsync`/`GetAllAsync(campaignId)` eagerly walk the children tree via `LoadChildrenRecursively` and sort each level by Position. The frontend uses `@he-tree/vue` / `vuedraggable` and posts new positions to `POST /api/campaigns/{id}/items/{itemId}/Position` with `priorPosition`/`nextPosition` of the drop neighbors.

### Current-player flow

`PlayerProvider` (registered as middleware via `app.UsePlayerProvider()` in Program.cs) reads the JWT, loads the `PlayerEntity` via `UserManager`, and stuffs it into an `AsyncLocal<PlayerEntity>`. Code anywhere downstream calls `PlayerProvider.GetCurrentPlayer()` — used by:
- `RepositoryBase` for author stamping + delete authorization
- `CampaignExtensions.IsDm()` and `.PlayerCampaigns()` for DM-only checks and listing only the caller's campaigns
- `CampaignItemRepository.AddAsync` rejects with `UnauthorizedAccessException` if the current player isn't the DM of the target campaign

If you add code that needs the caller, prefer this static accessor over taking `HttpContext` — most of the codebase is structured around it.

### Database

- SQLite file at `vtt-campaign-wiki.Server/Data/VttCampaignWikiV2.db` (connection string in `appsettings.json`).
- Migrations in `Data/Migrations/`; `Program.cs` runs `context.Database.Migrate()` on startup, then `DbInitializer.SeedUsersAsync` seeds the initial user(s).
- `VttCampaignWikiDbContext` extends `IdentityDbContext<PlayerEntity, IdentityRole<int>, int, ...>` — note int keys, not the default Guid/string.
- `VttCampaignWikiDbContextFactory` (nested in the DbContext file) is the design-time factory EF tools use for `dotnet ef`.

### Frontend specifics

- `vite.config.js` proxies `/api` → `https://localhost:7128` and rewrites with `secure: false`. The dev server itself runs HTTPS using the exported ASP.NET dev cert.
- `src/utils/axios.js` is the shared axios instance — interceptors pull the JWT from the Pinia `auth` store and on `401` clear auth + redirect to `/login`. Always import this, not bare `axios`.
- Pinia stores in `src/stores/` are persisted via `pinia-plugin-persistedstate`.
- Router guard in `src/router.js` auto-fetches the campaign whenever a route param `campaignId` is present and the store is stale; clears it when navigating away from a campaign route. Adding a new campaign-scoped route → put `campaignId` in the path and the store will load itself.
- Path alias `@` → `src/`. Note that `vue` is aliased to the runtime+compiler ESM bundle (`vue/dist/vue.esm-bundler.js`), so runtime template compilation works (Quill mention templates rely on this).

### AI integration

`SuggestedItemsService` POSTs to OpenAI `v1/chat/completions` with a strict json_schema response format to extract RPG entities (NPCs/locations/items) from `SessionDto` content. `GenerateImage` uses `v1/images/generations` (dall-e-2). Both pull `OpenAI:ApiKey` from `IConfiguration` — set it in `appsettings.json` or environment for AI endpoints to function.
