---
version: alpha
name: OpenLoco Object Service
description: Web front-end for the OpenLoco Object Editor. One small, fixed vocabulary of fonts, sizes, radii and spacing keeps every page consistent.
colors:
  # Nine palette entries drive each theme; everything else is one of them
  # cast to an alpha at runtime. Light theme (default):
  background: "#F4F5F7"
  surface: "#FFFFFF"
  contrast: "#E3E6EA"
  panel: "rgba(227, 230, 234, 0.84)"
  panel-strong: "#E3E6EA"
  surface-raised: "rgba(227, 230, 234, 0.68)"
  surface-subtle: "rgba(255, 255, 255, 0.58)"
  surface-table: "rgba(255, 255, 255, 0.72)"
  text: "#1F2430"
  muted: "#6B7280"
  primary: "#C07A1E"
  accent: "#C07A1E"
  accent-strong: "#1F2430"
  accent-soft: "rgba(192, 122, 30, 0.18)"
  accent-soft-strong: "rgba(192, 122, 30, 0.12)"
  accent-border: "rgba(192, 122, 30, 0.34)"
  accent-border-strong: "rgba(192, 122, 30, 0.42)"
  border: "rgba(244, 245, 247, 0.18)"
  border-soft: "rgba(244, 245, 247, 0.08)"
  field-bg: "rgba(227, 230, 234, 0.9)"
  field-border: "rgba(244, 245, 247, 0.22)"
  button-primary-text: "#1F2430"
  success: "#2E7D32"
  success-soft: "rgba(46, 125, 50, 0.14)"
  success-border: "rgba(46, 125, 50, 0.26)"
  warning: "#B97A1F"
  warning-soft: "rgba(185, 122, 31, 0.14)"
  warning-border: "rgba(185, 122, 31, 0.26)"
  danger: "#C0392B"
  danger-soft: "rgba(192, 57, 43, 0.12)"
  danger-border: "rgba(192, 57, 43, 0.28)"
  disabled-bg: "rgba(107, 114, 128, 0.18)"
  disabled-border: "rgba(107, 114, 128, 0.3)"
  # Dark theme — an independent nine-colour palette (no shades derived).
  background-dark: "#2E2F33"
  surface-dark: "#3A3D44"
  contrast-dark: "#6E7280"
  text-dark: "#E8E8E8"
  muted-dark: "#8A8D8F"
  primary-dark: "#F6C945"
  accent-dark: "#F6C945"
  success-dark: "#46F65A"
  warning-dark: "#F6C945"
  danger-dark: "#F64646"
typography:
  # Six sizes. fontFamily is always --sans, except data/code which is --mono.
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
rounded:
  # Four radii. Anything circular uses 50%.
  sm: 8px
  md: 14px
  lg: 22px
  pill: 999px
spacing:
  # 4px grid — the only gaps/padding/margins allowed.
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
    backgroundColor: "{colors.accent-soft-strong}"
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
    textColor: "{colors.button-primary-text}"
    rounded: "{rounded.pill}"
    height: 40px
    padding: "0 {spacing.lg}"
  button-small:
    typography: "{typography.sm}"
    rounded: "{rounded.pill}"
    height: 32px
    padding: "0 {spacing.md}"
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
  stat-card:
    backgroundColor: "{colors.panel-strong}"
    textColor: "{colors.text}"
    rounded: "{rounded.lg}"
    padding: "{spacing.lg}"
  note-block:
    backgroundColor: "{colors.accent-soft-strong}"
    textColor: "{colors.accent-strong}"
    rounded: "{rounded.md}"
    padding: "{spacing.lg}"
  master-tab:
    backgroundColor: "{colors.surface-raised}"
    textColor: "{colors.text}"
    typography: "{typography.base}"
    rounded: "{rounded.pill}"
    height: 44px
    padding: "0 {spacing.xl}"
  master-tab-active:
    backgroundColor: "{colors.accent}"
    textColor: "{colors.button-primary-text}"
  master-tab-hover:
    backgroundColor: "{colors.accent-soft}"
    textColor: "{colors.accent-strong}"
  pagination-link:
    backgroundColor: "{colors.surface-raised}"
    textColor: "{colors.text}"
    rounded: "{rounded.pill}"
    height: 40px
    padding: "0 {spacing.lg}"
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
  hint:
    textColor: "{colors.muted}"
    typography: "{typography.sm}"
  theme-toggle:
    backgroundColor: "{colors.surface-raised}"
    rounded: "{rounded.pill}"
    width: 52px
    height: 28px
---

## Overview

The Object Service uses a deliberately **small, fixed vocabulary**. There are
two font families, six type sizes, four corner radii and six spacing steps —
nothing else. Every page composes from that set, so the UI stays consistent and
predictable no matter who builds the next screen.

Two rules make this work:

- **Everything derives from a nine-colour palette.** Each theme is nine named
  colours (`background`, `surface`, `contrast`, `text`, `muted`, `accent`,
  `success`, `warning`, `danger`); every other colour token is one of them cast
  to an alpha. Light and dark are independent palettes, applied at runtime by the
  inline script in `_Layout.cshtml` (with fallbacks in `base.css`).
- **Never invent a value.** If you need a size, radius or gap, pick the nearest
  token below — do not add a new `13px` or `0.92rem`. Consistency comes from the
  vocabulary being closed.

The styles ship as four layers: `base.css` (tokens + element defaults),
`layout.css` (shell/grids), `components.css` (atoms), `pages.css` (features).

## Colors

Nine entries per theme; everything else is derived. **`accent` is the only
interaction colour** — buttons, active tabs, focus rings, hover borders.

| Token | Light | Role |
|---|---|---|
| `background` | `#F4F5F7` | page base + gradient stops |
| `surface` | `#FFFFFF` | brightest plane (cards, frames) |
| `contrast` | `#E3E6EA` | solid raised panels |
| `text` / `muted` | `#1F2430` / `#6B7280` | primary copy / secondary + meta |
| `accent` | `#C07A1E` | the sole driver of interaction |
| `success` / `warning` / `danger` | `#2E7D32` / `#B97A1F` / `#C0392B` | status |

Derived tokens are just alphas, e.g. `accent-soft` = accent at 0.18,
`border` = background at 0.18. The dark theme swaps all nine for an independent
palette (`accent` becomes `#F6C945`), so contrast relationships hold in both.

## Typography

Only two families, both declared once as custom properties:

- **`--sans`** (Aptos → Segoe UI Variable Text → Segoe UI → sans-serif) — all UI
  and body copy.
- **`--mono`** (Cascadia Code → Consolas) — machine values: `<dd>`, JSON views,
  `code`.

Six sizes, and nothing between them:

| Token | Size | Use |
|---|---|---|
| `--text-display` | `3.5rem`* | hero `h1` only |
| `--text-xl` | `1.75rem` | page titles, `.brand` |
| `--text-lg` | `1.25rem` | section headings (`h2`) |
| `--text-base` | `1rem` | body |
| `--text-sm` | `0.875rem` | secondary / meta / table / data |
| `--text-xs` | `0.75rem` | badges, `.eyebrow`, small caps |

\* `--text-display` is authored as `clamp(2rem, 4vw, 3.5rem)` so it scales down
on narrow screens; the token records its maximum.

Weights are limited to `400`, `600` and `700`. Small-caps labels (`.eyebrow`,
`dt`, `th`) are uppercase, bold and letter-spaced.


## Layout & Spacing

Spacing is a closed **4px grid** — `xs 4 · sm 8 · md 12 · lg 16 · xl 24 ·
2xl 32` px. Every `gap`, `padding` and `margin` in the codebase is one of these
tokens; there are no one-off values.

- `.site-shell` spans the viewport with `--space-xl` padding; content is centred
  with `margin: 0 auto`.
- **Master–detail** is the core pattern: a `240px` sticky sidebar beside a
  flexible content column.
- Page-level distribution uses `--space-xl` (24px); gaps *inside* cards use
  `--space-lg` (16px).
- Responsive: at `1100px` grids go single-column, at `900px` master–detail
  stacks, at `720px` the header stacks and tables bleed to the edges.

## Elevation & Depth

Depth is translucency and blur, not shadows.

- **Page** — two soft radial glows over a vertical `background → surface`
  gradient.
- **Panels & cards** — translucent `--panel`, a hairline `--border`, `rounded.lg`
  and `backdrop-filter: blur(10px)`.
- **Header** — the most opaque surface (`--panel-glass`) with `blur(14px)`.
- Hovering a card or button lifts it `translateY(-2px)` and swaps the border to
  `--accent-border`, so interactivity reads as physical lift, not just colour.

## Shapes

Soft and rounded — no sharp corners anywhere.

- `rounded.pill` (`999px`) for everything interactive or status-like: buttons,
  chips, badges, tabs, the theme toggle, health pills.
- `rounded.lg` (`22px`) for primary surfaces: panels, object cards, the sidebar,
  the header.
- `rounded.md` (`14px`) for inputs, meta cells and nested cards; `rounded.sm`
  (`8px`) for the smallest cells.
- Circles (dots, the toggle thumb) use `50%`.

## Components

Every component is just a combination of the tokens above. The authoritative
values are in the front matter.

### Buttons & Actions

One shared base: pill shape, `40px` min height, `600` weight, a `140ms`
lift-on-hover. **`button-primary`** is the only amber-filled action per screen;
`button-secondary` is the translucent default; `button-danger`/`button-warning`
are for destructive/cautionary flows; `button-disabled` uses the muted
`disabled-*` pair.

### Status & Feedback

Pill **chips** (`status-available` / `-missing` / `-unavailable`) and square
**badges** (`badge-success` / `-warning` / `-error`) carry status inline. The
header **health indicator** tints itself success/warning/danger. Block feedback
uses `alert-success` / `alert-error`.

### Surfaces

`panel` is the workhorse container; `object-card` is the clickable variant that
lifts on hover; `stat-card` shows a big number; `meta-cell` and `data-table`
present key/value and tabular data.

### Inputs & Navigation

Fields use `rounded.md`, `--field-bg` and a 3px `accent-soft` focus ring. Primary
navigation uses **master tabs** (pill buttons that fill accent when active). The
object explorer uses a sticky sidebar of list links, with the active item marked
by `accent-soft` / `accent-strong`.

## Do's and Don'ts

- **Do** pick the nearest existing size/radius/space token — never add a new
  numeric value.
- **Do** derive every tinted surface from one of the nine palette colours by
  casting it to an alpha.
- **Do** use `--accent` for the single most important action per screen.
- **Don't** mix fonts — only `--sans` and `--mono` exist.
- **Don't** mix pill and square corners on the same control type.
- **Don't** hard-code hex values in component CSS — reference a token so theming
  and dark mode keep working.

