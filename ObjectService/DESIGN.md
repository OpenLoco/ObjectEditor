---
version: alpha
name: OpenLoco Object Service
description: Web front-end for the OpenLoco Object Editor. A small fixed palette drives everything; light and dark are the only themes.
colors:
  # ── Palette — the eleven roles you edit (light theme; dark lives in base.css).
  # `primary` mirrors `accent`; everything below the divider derives via color-mix().
  bg: "#F4F5F7"
  surface: "#FFFFFF"
  surface-2: "#E3E6EA"
  border: "#D6DAE0"
  text: "#1F2430"
  muted: "#6B7280"
  primary: "#C07A1E"
  accent: "#C07A1E"
  accent-fg: "#1F2430"
  success: "#2E7D32"
  warning: "#B97A1F"
  danger: "#C0392B"
  # ── Derived from the palette with color-mix() — do not edit.
  panel: "rgba(227, 230, 234, 0.84)"
  panel-strong: "#E3E6EA"
  panel-glass: "rgba(227, 230, 234, 0.74)"
  surface-raised: "rgba(227, 230, 234, 0.68)"
  surface-subtle: "rgba(255, 255, 255, 0.58)"
  surface-table: "rgba(255, 255, 255, 0.72)"
  field-bg: "rgba(227, 230, 234, 0.9)"
  accent-strong: "#1F2430"
  accent-soft: "rgba(192, 122, 30, 0.18)"
  accent-soft-strong: "rgba(192, 122, 30, 0.12)"
  button-primary-text: "#1F2430"
  success-soft: "rgba(46, 125, 50, 0.14)"
  warning-soft: "rgba(185, 122, 31, 0.14)"
  danger-soft: "rgba(192, 57, 43, 0.12)"
  disabled-bg: "rgba(107, 114, 128, 0.18)"
typography:
  # Six sizes; fontFamily is always Aptos, except data (Cascadia Code).
  display:
    fontFamily: Aptos
    fontSize: 3.5rem
    fontWeight: 700
    lineHeight: 1.04
  xl:
    fontFamily: Aptos
    fontSize: 1.75rem
    fontWeight: 700
    lineHeight: 1.1
  lg:
    fontFamily: Aptos
    fontSize: 1.25rem
    fontWeight: 600
    lineHeight: 1.3
  base:
    fontFamily: Aptos
    fontSize: 1rem
    fontWeight: 400
    lineHeight: 1.5
  sm:
    fontFamily: Aptos
    fontSize: 0.875rem
    fontWeight: 400
    lineHeight: 1.5
  xs:
    fontFamily: Aptos
    fontSize: 0.75rem
    fontWeight: 700
    lineHeight: 1.2
    letterSpacing: 0.08em
  data:
    fontFamily: Cascadia Code
    fontSize: 0.875rem
    fontWeight: 400
    lineHeight: 1.45
rounded:
  sm: 8px
  md: 14px
  lg: 22px
  pill: 999px
spacing:
  xs: 4px
  sm: 8px
  md: 12px
  lg: 16px
  xl: 24px
  2xl: 32px
components:
  button-primary:
    backgroundColor: "{colors.accent}"
    textColor: "{colors.button-primary-text}"
    typography: "{typography.base}"
    rounded: "{rounded.pill}"
    height: 40px
    padding: "0 {spacing.lg}"
  button-primary-hover:
    backgroundColor: "{colors.accent-soft}"
  button-secondary:
    backgroundColor: "{colors.surface-raised}"
    textColor: "{colors.text}"
    typography: "{typography.base}"
    rounded: "{rounded.pill}"
    height: 40px
    padding: "0 {spacing.lg}"
  button-danger:
    backgroundColor: "{colors.danger}"
    textColor: "#FFFFFF"
    rounded: "{rounded.pill}"
    height: 40px
    padding: "0 {spacing.lg}"
  button-warning:
    backgroundColor: "{colors.warning}"
    textColor: "{colors.accent-fg}"
    rounded: "{rounded.pill}"
    height: 40px
    padding: "0 {spacing.lg}"
  button-disabled:
    backgroundColor: "{colors.disabled-bg}"
    textColor: "{colors.text}"
    rounded: "{rounded.pill}"
  chip:
    backgroundColor: "{colors.accent-soft-strong}"
    textColor: "{colors.accent-strong}"
    rounded: "{rounded.pill}"
    height: 30px
    padding: "0 {spacing.md}"
  badge-success:
    backgroundColor: "{colors.success-soft}"
    textColor: "{colors.success}"
    rounded: "{rounded.sm}"
  badge-warning:
    backgroundColor: "{colors.warning-soft}"
    textColor: "{colors.warning}"
    rounded: "{rounded.sm}"
  badge-error:
    backgroundColor: "{colors.danger-soft}"
    textColor: "{colors.danger}"
    rounded: "{rounded.sm}"
  status-available:
    backgroundColor: "{colors.accent-soft}"
    textColor: "{colors.accent-strong}"
    rounded: "{rounded.pill}"
  status-missing:
    backgroundColor: "{colors.warning-soft}"
    textColor: "{colors.warning}"
    rounded: "{rounded.pill}"
  status-unavailable:
    backgroundColor: "{colors.danger-soft}"
    textColor: "{colors.danger}"
    rounded: "{rounded.pill}"
  panel:
    backgroundColor: "{colors.panel}"
    textColor: "{colors.text}"
    rounded: "{rounded.lg}"
    padding: "{spacing.xl}"
  object-card:
    backgroundColor: "{colors.panel}"
    textColor: "{colors.text}"
    rounded: "{rounded.lg}"
    padding: "{spacing.xl}"
  image-card:
    backgroundColor: "{colors.surface-subtle}"
    textColor: "{colors.text}"
    rounded: "{rounded.lg}"
    padding: "{spacing.lg}"
  stat-card:
    backgroundColor: "{colors.panel-strong}"
    textColor: "{colors.text}"
    rounded: "{rounded.lg}"
    padding: "{spacing.lg}"
  meta-cell:
    backgroundColor: "{colors.surface-subtle}"
    textColor: "{colors.text}"
    rounded: "{rounded.md}"
    padding: "{spacing.md}"
  data-table:
    backgroundColor: "{colors.surface-table}"
    textColor: "{colors.text}"
    typography: "{typography.base}"
  field-input:
    backgroundColor: "{colors.field-bg}"
    textColor: "{colors.text}"
    typography: "{typography.base}"
    rounded: "{rounded.md}"
    height: 48px
    padding: "0 {spacing.lg}"
  note-block:
    backgroundColor: "{colors.accent-soft-strong}"
    textColor: "{colors.accent-strong}"
    rounded: "{rounded.md}"
    padding: "{spacing.lg}"
  master-tab:
    backgroundColor: "{colors.surface-2}"
    textColor: "{colors.text}"
    typography: "{typography.base}"
    rounded: "{rounded.pill}"
    height: 44px
    padding: "0 {spacing.xl}"
  master-tab-active:
    backgroundColor: "{colors.accent}"
    textColor: "{colors.accent-fg}"
  site-header:
    backgroundColor: "{colors.panel-glass}"
    textColor: "{colors.text}"
    rounded: "{rounded.lg}"
    padding: "{spacing.lg}"
  hint:
    textColor: "{colors.muted}"
    typography: "{typography.sm}"
  health-indicator-healthy:
    backgroundColor: "{colors.success-soft}"
    textColor: "{colors.success}"
    rounded: "{rounded.pill}"
  health-indicator-degraded:
    backgroundColor: "{colors.warning-soft}"
    textColor: "{colors.warning}"
    rounded: "{rounded.pill}"
  health-indicator-unhealthy:
    backgroundColor: "{colors.danger-soft}"
    textColor: "{colors.danger}"
    rounded: "{rounded.pill}"
  alert-success:
    backgroundColor: "{colors.success-soft}"
    textColor: "{colors.success}"
    rounded: "{rounded.md}"
    padding: "{spacing.lg}"
  alert-error:
    backgroundColor: "{colors.danger-soft}"
    textColor: "{colors.danger}"
    rounded: "{rounded.md}"
    padding: "{spacing.lg}"
---

## Overview

A small, fixed vocabulary. Two fonts, six type sizes, four radii, six spacing
steps, and a palette of **eleven editable colours per theme**. Every page
composes from that set — there are no one-off values.

- **The palette is the single source of truth.** The eleven colours under
  `:root` (light) and `html[data-theme=dark]` (dark) live in `base.css`. Every
  other colour token is derived from them with `color-mix()`, so tints, borders
  and glows follow the palette automatically. There is **no runtime colour
  generation**.
- **Light and dark only.** Switching theme just flips `data-theme` on `<html>`;
  the CSS does the rest.
- **Never invent a value.** Pick the nearest token — do not add a new `13px`,
  `0.92rem` or hard-coded hex.

The styles ship as four layers: `base.css` (palette + tokens + defaults),
`layout.css` (shell/grids), `components.css` (atoms + utilities), `pages.css`
(features).

## Colors

Edit eleven values per theme — nothing else. The role of each:

| Token | Light | Dark | Role |
|---|---|---|---|
| `bg` | `#F4F5F7` | `#2E2F33` | page background |
| `surface` | `#FFFFFF` | `#3A3D44` | cards / brightest plane |
| `surface-2` | `#E3E6EA` | `#454851` | raised + inset surfaces |
| `border` | `#D6DAE0` | `#50545E` | hairlines |
| `text` | `#1F2430` | `#E8E8E8` | primary copy |
| `muted` | `#6B7280` | `#A2A6AB` | secondary copy |
| `accent` | `#C07A1E` | `#F6C945` | the only interaction colour |
| `accent-fg` | `#1F2430` | `#1F2430` | text drawn on the accent |
| `success` | `#2E7D32` | `#5ADB6B` | status |
| `warning` | `#B97A1F` | `#F6C945` | status |
| `danger` | `#C0392B` | `#F66868` | status + destructive actions |

Everything else is derived, e.g.:

```css
--panel:         color-mix(in srgb, var(--surface-2) 84%, transparent);
--accent-soft:   color-mix(in srgb, var(--accent) 18%, transparent);
--success-soft:  color-mix(in srgb, var(--success) 14%, transparent);
```

## Typography

Only two families: **`--sans`** (Aptos → Segoe UI) for all UI copy and
**`--mono`** (Cascadia Code) for machine values (`<dd>`, JSON, `code`). Weights
are limited to `400`, `600` and `700`.

| Token | Size | Use |
|---|---|---|
| `--text-display` | `clamp(2rem, 4vw, 3.5rem)` | hero `h1` only |
| `--text-xl` | `1.75rem` | page titles, `.brand` |
| `--text-lg` | `1.25rem` | section headings |
| `--text-base` | `1rem` | body |
| `--text-sm` | `0.875rem` | secondary / meta / table |
| `--text-xs` | `0.75rem` | badges, eyebrows, small caps |

## Layout & Spacing

A closed **4px grid** — `xs 4 · sm 8 · md 12 · lg 16 · xl 24 · 2xl 32` px — is
the only source of `gap`, `padding` and `margin`. `.site-shell` fills the
viewport with `--space-xl` padding and centres with `margin: 0 auto`.
Master–detail (a `240px` sticky sidebar + content) is the core pattern.
Breakpoints: `1100px`, `900px`, `720px`.

## Elevation & Depth

Depth is translucency and blur, not shadows: a radial-glow page background,
translucent `--panel` surfaces with `backdrop-filter: blur(10px)`, and the
slightly more opaque `--panel-glass` header (`blur(14px)`). Hovering a card or
button lifts it `translateY(-2px)`.

## Shapes

Soft and rounded only. `rounded.pill` for everything interactive/status;
`rounded.lg` (`22px`) for panels and cards; `rounded.md` (`14px`) for inputs and
nested cells; `rounded.sm` (`8px`) for the smallest cells. Circles use `50%`.

## Components

Every component is a combination of the tokens above (see the front matter for
exact values).

- **Buttons** — one shared pill base; `button-primary` is the only amber-filled
  action per screen, `button-secondary` the translucent default, `button-danger`
  / `button-warning` for destructive/cautionary flows.
- **Status** — pill chips (`status-*`), square badges (`badge-*`) and the header
  health indicator, each tinting a status colour over its `-soft` background.
- **Surfaces** — `panel` (workhorse), `object-card` (clickable), `stat-card`,
  `meta-cell`, `data-table`.
- **Inputs & navigation** — `field-input` with an accent focus ring; `master-tab`
  pills for primary navigation; the object-explorer sidebar for the rest.

## Do's and Don'ts

- **Do** change the look by editing the eleven palette values — nothing else.
- **Do** derive tints with `color-mix()` from a palette colour.
- **Do** use `--accent` for the single most important action per screen.
- **Don't** hard-code a hex, font, size, radius or spacing value in a component.
- **Don't** mix fonts, or mix pill and square corners on the same control type.
- **Don't** add runtime colour logic — the palette is pure CSS.

