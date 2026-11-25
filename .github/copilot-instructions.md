# Blazor Web App Copilot Instructions

## General Blazor guidance

- Before editing, inspect the project's target framework, hosting and render-mode setup, routing, and styling conventions. Follow the existing architecture; do not assume every Blazor app uses .NET 11 or Interactive Server, or migrate hosting patterns or move render modes without a clear need.
- For visual changes, locate the project's globally loaded stylesheet and existing theme tokens. Reuse its semantic CSS variables; do not assume a particular stylesheet path, token name, or color palette exists across projects. Add shared tokens at the existing source of truth only when no existing role fits.
- Do not launch or open the application. Build-only validation is okay. End implementation summaries by asking me to test in my local device browser using Codespace port forwarding.

## DN11-specific conventions

Apply the conventions in this section only when working in MSFTJim/DN11.

- This project is a .NET 11 Blazor Web App using Interactive Server rendering. `App.razor` is the document root, `Routes.razor` sets `@rendermode InteractiveServer`, and `Program.cs` configures `AddRazorComponents().AddInteractiveServerComponents()` and `MapRazorComponents<App>().AddInteractiveServerRenderMode()`.
- For sidebar icons, use the raw SVG from the official Bootstrap Icons page. Add it as a `data:image/svg+xml` background in `NavMenu.razor.css`, following the existing `.bi-*-nav-menu` rules and reusing the shared `.bi` sizing styles. Use `fill='white'`, percent-encode the SVG tags, and add the matching `<span class="bi bi-*-nav-menu" aria-hidden="true">` in `NavMenu.razor`. Do not add an icon package or change other nav icons unless requested.
- `jfvQuickGrid/wwwroot/app.css` is the source of truth for this project's `:root` color tokens. Use these with `var(--app-...)` in global or scoped CSS instead of hard-coding theme colors or defining local copies:
  - `--app-background` for the page backdrop; `--app-surface` for cards, sidebars, and modal surfaces; `--app-surface-raised` for inset or raised areas such as headers, tiles, and inputs.
  - `--app-border` for surface borders; `--app-text` and `--app-text-muted` for primary and secondary text.
  - `--app-primary` and `--app-primary-hover` for primary actions; `--app-accent-light` and `--app-accent-hover` for accents; use `--app-nav-*` tokens for navigation-specific styling.
- When using Bootstrap components, set their relevant CSS custom properties in the component's styles if needed so they use the shared palette rather than Bootstrap's default colors. Add new semantic color tokens only in `:root` in `app.css` when necessary. Keep intentional status colors (such as validation errors) distinct from the theme palette.
