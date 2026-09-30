---
name: Calm Kinetic
colors:
  surface: '#f6fbf5'
  surface-dim: '#d7dbd6'
  surface-bright: '#f6fbf5'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f0f5f0'
  surface-container: '#ebefea'
  surface-container-high: '#e5e9e4'
  surface-container-highest: '#dfe4df'
  on-surface: '#181d1a'
  on-surface-variant: '#424846'
  inverse-surface: '#2c322e'
  inverse-on-surface: '#edf2ed'
  outline: '#727876'
  outline-variant: '#c2c8c5'
  surface-tint: '#4c635e'
  primary: '#081f1b'
  on-primary: '#ffffff'
  primary-container: '#1e3430'
  on-primary-container: '#859d97'
  inverse-primary: '#b3ccc6'
  secondary: '#40655c'
  on-secondary: '#ffffff'
  secondary-container: '#bfe8dc'
  on-secondary-container: '#446a60'
  tertiary: '#2d1500'
  on-tertiary: '#ffffff'
  tertiary-container: '#4b2700'
  on-tertiary-container: '#c58c57'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#cfe8e2'
  primary-fixed-dim: '#b3ccc6'
  on-primary-fixed: '#081f1c'
  on-primary-fixed-variant: '#354b47'
  secondary-fixed: '#c2ebdf'
  secondary-fixed-dim: '#a6cfc3'
  on-secondary-fixed: '#00201a'
  on-secondary-fixed-variant: '#284d44'
  tertiary-fixed: '#ffdcc0'
  tertiary-fixed-dim: '#f9b980'
  on-tertiary-fixed: '#2d1600'
  on-tertiary-fixed-variant: '#683c0e'
  background: '#f6fbf5'
  on-background: '#181d1a'
  surface-variant: '#dfe4df'
typography:
  headline-xl:
    fontFamily: Newsreader
    fontSize: 3.5rem
    fontWeight: '400'
    lineHeight: 4rem
    letterSpacing: -0.025em
  headline-xl-mobile:
    fontFamily: Newsreader
    fontSize: 2.25rem
    fontWeight: '400'
    lineHeight: 2.75rem
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Newsreader
    fontSize: 2.5rem
    fontWeight: '400'
    lineHeight: 3rem
    letterSpacing: -0.02em
  headline-lg-mobile:
    fontFamily: Newsreader
    fontSize: 1.75rem
    fontWeight: '400'
    lineHeight: 2.25rem
    letterSpacing: -0.015em
  headline-md:
    fontFamily: Newsreader
    fontSize: 1.75rem
    fontWeight: '400'
    lineHeight: 2.25rem
    letterSpacing: -0.01em
  headline-sm:
    fontFamily: Newsreader
    fontSize: 1.25rem
    fontWeight: '500'
    lineHeight: 1.75rem
  body-lg:
    fontFamily: Work Sans
    fontSize: 1.125rem
    fontWeight: '400'
    lineHeight: 1.75rem
  body-md:
    fontFamily: Work Sans
    fontSize: 1rem
    fontWeight: '400'
    lineHeight: 1.5rem
  body-sm:
    fontFamily: Work Sans
    fontSize: 0.875rem
    fontWeight: '400'
    lineHeight: 1.375rem
  label-md:
    fontFamily: Work Sans
    fontSize: 0.8125rem
    fontWeight: '500'
    lineHeight: 1.125rem
    letterSpacing: 0.02em
  label-sm:
    fontFamily: Work Sans
    fontSize: 0.6875rem
    fontWeight: '600'
    lineHeight: 1rem
    letterSpacing: 0.06em
rounded:
  sm: 0.125rem
  DEFAULT: 0.25rem
  md: 0.375rem
  lg: 0.5rem
  xl: 0.75rem
  full: 9999px
spacing:
  gutter: 1.5rem
  gutter-mobile: 1rem
  margin: 3rem
  margin-mobile: 1.25rem
  space-xs: 0.25rem
  space-sm: 0.5rem
  space-md: 1rem
  space-lg: 1.5rem
  space-xl: 2.5rem
---

## Brand & Style

This design system embodies the "Calm Kinetic" design philosophy: an intentional, adult, and distraction-resistant workspace for execution. Rejecting gamified hyper-stimulation, synthetic neon gradients, and glossy veneers, the interface borrows from the permanence of architectural stone, archival publishing, and tactile paper. 

The aesthetic is anchored in an elevated literary rhythm. Newsreader brings reflective, authoritative prose pacing to headers, while Work Sans supplies rational, utilitarian clarity to interaction targets, tools, and systemic labels. The emotional environment is steady, quiet, and grounded—a tranquil studio that promotes deep concentration, decisive agency, and intellectual endurance.

## Colors

The palette derives from raw mineral pigment, warm limestone, and archival paper. Color is applied with restraint to prevent sensory saturation:

- **Primary (`#1E3430`)**: Deep mineral pine-graphite. The grounding weight of the design system, used for primary structural frames, key action buttons, and dominant states.
- **Secondary (`#4A7066`)**: Muted mineral sage. Used for secondary interactive states, toggles, calm highlights, and subtle progressive cues.
- **Tertiary (`#BA824E`)**: Warm terracotta/clay. Reserved strictly for contextual urgency, temporal reminders, and mindful markers.
- **Neutral (`#1E2320`)**: Deep graphite ink. Provides legible contrast on bone backgrounds without the synthetic harshness of pure black.

### Canvas & Surface Hierarchy
- **Canvas Base**: `#F6F4EE` (warm limestone bone)
- **Canvas Sunk / Recessed**: `#EFECE3` (milled linen)
- **Canvas Elevated**: `#FFFFFF` (clean bleached chalk, used sparingly)
- **Text Secondary**: `#69706C` (muted stone)
- **Borders & Dividers**: `#E3DFD4` (soft lime wash) and `#D6D1C4` (etched stone line)

## Typography

The typographical tension between **Newsreader** and **Work Sans** creates an intentional editorial balance:

1. **Newsreader**: Conveys pause, critical perspective, and narrative presence. Applied in optical sizing to titles, reflective headers, execution prompts, and macro time segments. Italics are reserved for meditative quotes, empty states, and contextual notes.
2. **Work Sans**: Delivers structural clarity. Set with open counters and balanced geometry, it manages task items, metadata chips, control toggles, and data readouts.

Maintain deliberate vertical proportion: large headlines must be cushioned by ample vertical clearance, refusing the crammed density typical of analytics dashboards.

## Layout & Spacing

The layout is built around deliberate whitespace rather than enclosed containers. Content breathes on an asymmetric, editorial 12-column grid on desktop screens, consolidating to a focused 4-column flow on mobile.

- **Desktop (min 1024px)**: Center-weighted execution canvas capped at 1200px max width. Outer margin defaults to `3rem` (`space-xl` + `space-md`), preserving focus through lateral borders. Gutters measure `1.5rem`.
- **Tablet (768px – 1023px)**: 8-column layout with `2rem` margins and `1.25rem` gutters.
- **Mobile (<768px)**: 4-column single-stream canvas with `1.25rem` margins and `1rem` gutters.

Spacing enforces rhythm: sections are divided by generous `space-xl` gaps rather than hard full-bleed colored slabs, fostering a spacious reading experience.

## Elevation & Depth

This design system avoids multi-tier drop shadows, synthetic blurs, and skeuomorphic bevels. Visual hierarchy is established via **tonal depth and tactile separation**:

- **Cardless Canvases**: Information groups live directly on the `#F6F4EE` canvas. Distinction is achieved through hairline borders (`#E3DFD4`) and generous whitespace rather than elevated, floating boxes.
- **Surface Inset**: Depressed zones (active focus blocks, scratchpads, timeline troughs) use `#EFECE3` with a crisp `1px` inner boundary of `#D6D1C4`.
- **Ambient Floor (Popovers / Overlays)**: Menus and modals use an ultra-diffused shadow tinted with deep mineral pigments:
  - `0 8px 24px -4px rgba(30, 35, 32, 0.06), 0 2px 6px -1px rgba(30, 35, 32, 0.03)`
- **Subtle Surface Texture**: High-density elements carry a fine mineral paper grain (`1.5%` monochromatic noise) embedded directly within SVG fills to eliminate digital flatness.

## Shapes

Shapes feature restrained, architectural tailoring (`roundedness: 1`). Corner radii are crisp and disciplined:

- **Base Radius (`0.25rem`)**: Applied to interactive inputs, standard buttons, task checkboxes, and data tags.
- **Radius-LG (`0.5rem`)**: Applied to larger context sheets, dialog frames, and utility overlays.
- **Radius-XL (`0.75rem`)**: Reserved for expansive canvas shells.

Pill-shaped radii and circular action bubbles are explicitly prohibited to prevent childish or gamified visual cues.

## Components

### Buttons
- **Primary**: Background `#1E3430`, typography `Work Sans` 500 in `#F6F4EE`. Border `1px solid transparent`. Hover transitions to `#2C4641`. Active state presses flat with no scale-down gimmicks.
- **Secondary**: Background transparent, typography `#1E2320`. Border `1px solid #D6D1C4`. Hover transitions to background `#EFECE3`.
- **Tertiary / Context**: Background transparent, text `#BA824E`. Hover adds hairline underline with `2px` offset.

### Chips & Meta Indicators
- Structural, rectilinear tags with `0.25rem` radius.
- Background `#EFECE3`, border `1px solid #E3DFD4`, text `#69706C` set in `label-sm` uppercase.
- Status indicators replace bright dots with subtle stone-toned glyphs or understated mineral green fills (`#4A7066`).

### Lists & Execution Tracks
- Borderless list items separated by `1px` divider lines (`#E3DFD4`).
- Generous internal padding (`space-md` vertical).
- Leading identifiers use `Newsreader` italic numerals (e.g., *01*, *02*) set in `#69706C`.

### Form Controls (Checkboxes & Radios)
- Square geometry with `0.25rem` corner radius.
- Inactive: Background `#F6F4EE`, border `1.5px solid #D6D1C4`.
- Active/Checked: Fill `#1E3430` with `#F6F4EE` checkmark icon. No spring animations or celebratory confetti bursts.

### Text Inputs
- Minimal field frames: Background `#FFFFFF`, border `1px solid #D6D1C4`, text `#1E2320`.
- Focus state: Border shifts quietly to `#4A7066` without high-contrast glow rings.

### Focus Frame (Cardless Container)
- Replaces traditional elevated cards. Composed of an unadorned `#F6F4EE` plane framed by an etched boundary (`1px solid #E3DFD4`), using `space-lg` padding to structure thoughts without visual clutter.