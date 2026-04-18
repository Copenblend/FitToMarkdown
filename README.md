# FTM — FIT to Markdown

A .NET command-line tool that converts Garmin `.fit` activity files into structured Markdown documents optimized for LLM consumption and human readability.

## Features

- **Convert** — Transform `.fit` files into detailed Markdown with YAML frontmatter, session tables, lap splits, record time-series, HR zones, HRV data, device info, events, and developer fields
- **Progression** — Build chronological sport progression documents that combine multiple activities, with multi-sport support
- **Inspect** — View FIT file metadata without generating Markdown
- **Interactive Mode** — Arrow-key menu-driven interface when run without arguments
- **Batch Processing** — Convert entire directories of `.fit` files at once

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

## Installation

### Install as a Global Tool

```shell
dotnet tool install -g FitToMarkdown.Cli --add-source ./nupkg
```

### Build from Source

```shell
git clone https://github.com/Copenblend/FitToMarkdown.git
cd FitToMarkdown
dotnet build
dotnet pack src/FitToMarkdown.Cli/FitToMarkdown.Cli.csproj -c Release -o ./nupkg
dotnet tool install -g FitToMarkdown.Cli --add-source ./nupkg --version 0.1.0-alpha.1
```

### Verify Installation

```shell
ftm version
```

### Uninstall

```shell
dotnet tool uninstall -g FitToMarkdown.Cli
```

### Update

Uninstall first, then reinstall with the new version:

```shell
dotnet tool uninstall -g FitToMarkdown.Cli
dotnet tool install -g FitToMarkdown.Cli --add-source ./nupkg --version <NEW_VERSION>
```

## Usage

### Interactive Mode

Run `ftm` with no arguments to launch the interactive menu:

```shell
ftm
```

```
  _____ _____ __  __
 |  ___|_   _|  \/  |
 | |_    | | | |\/| |
 |  _|   | | | |  | |
 |_|     |_| |_|  |_|
  FIT to Markdown converter

 What would you like to do?
 > Convert FIT files to Markdown
   Build Sport Progression
   Inspect FIT file metadata
   Show version
   Exit
```

Use arrow keys to navigate, Enter to select. All interactive prompts support this navigation style.

---

### `ftm convert` — Convert FIT Files

Convert a single `.fit` file or an entire directory to Markdown.

#### Single File

```shell
ftm convert activity.fit
```

Produces a Markdown file alongside the source, named with the format `YYYYMMDD_hhmmss_Sport.md` (e.g., `20260417_152737_Running.md`).

#### Directory (Batch)

```shell
ftm convert C:\Activities
```

Discovers all `.fit` files in the directory and converts each one. A file browser is shown if no path is provided.

#### Options

| Option | Description |
|--------|-------------|
| `[path]` | Path to a `.fit` file or directory. Prompted interactively if omitted. |
| `-o, --output <DIR>` | Output directory for generated Markdown files. Defaults to the source directory. |
| `--overwrite <MODE>` | How to handle existing output files: `skip`, `overwrite`, or `ask-each`. |
| `--no-interaction` | Disable all interactive prompts (for scripting/CI). |

#### Examples

```shell
# Convert all .fit files in a folder, output to a docs directory
ftm convert C:\Activities --output C:\Docs

# Batch convert without prompts, skip existing files
ftm convert C:\Activities --no-interaction --overwrite skip

# Batch convert, overwrite all existing
ftm convert C:\Activities --no-interaction --overwrite overwrite
```

#### Output Structure

Each converted Markdown file contains:

| Section | Description |
|---------|-------------|
| **YAML Frontmatter** | File type, sport, sub-sport, timestamps, device info, summary stats |
| **Heading & Timestamp** | H1 title (e.g., `# Running Activity`) with UTC start time |
| **Overview** | Bullet list of key metrics: distance, duration, calories, HR, speed, pace |
| **Session Details** | Per-session metric tables with sport, distance, elapsed/timer time, HR, cadence, power |
| **Laps** | Lap-by-lap table with distance, duration, speed, HR, and trigger type |
| **Record Statistics** | Min/avg/max/std-dev table for heart rate, speed, cadence, power, altitude, etc. |
| **Time Series Data** | Down-sampled CSV code block with timestamps, distance, speed, HR, cadence, power, GPS |
| **Heart Rate Zones** | Zone distribution table with time-in-zone and percentages |
| **HRV** | Heart rate variability summary (sample count, avg RR, RMSSD, SDNN, min/max RR) |
| **Devices** | Connected device table with serial, software version, battery status |
| **Events** | Timer start/stop, distance alerts, auto-lap triggers, gear changes |
| **Developer Fields** | Connect IQ developer data fields per session, lap, and record |

---

### `ftm progression` — Sport Progression Documents

Build or update a progression document that combines multiple activities for the same sport in chronological order. Useful for tracking training history over time.

#### Build from a Directory

```shell
ftm progression C:\Activities
```

This will:

1. Scan all `.fit` files in the directory
2. Group files by sport and sub-sport (e.g., `Running`, `FitnessEquipment (IndoorRowing)`)
3. Display a sport selection prompt
4. Ask if you want to include additional sports/sub-sports (multi-select with spacebar)
5. Build a chronological progression document with all selected activities

#### Multi-Sport Selection

When building a progression, after selecting a primary sport you can combine related activities:

```
Select a sport to build progression for:
> Running (12 files)
  FitnessEquipment (IndoorRowing) (3 files)
  Cycling (IndoorCycling) (5 files)

Include other sports/sub-sports in this progression? [y/n] (n): y

Toggle sports to include:
  (Press <space> to toggle, <enter> to confirm)
> [ ] FitnessEquipment (IndoorRowing) (3 files)
  [ ] Cycling (IndoorCycling) (5 files)
```

- **Single sport selected** → File: `Running_Progression_20260417.md`, H1: `# Running Progression`
- **Multiple sports selected** → File: `Activity_Progression_20260417.md`, H1: `# Activity Progression`

#### Add a Single File to an Existing Progression

**Interactive (recommended):** Select a single `.fit` file from the progression menu — the tool will find matching `*_Progression_*.md` files in the same directory and let you pick one.

**CLI flags:**

```shell
ftm progression --add new-run.fit --progression-file Running_Progression_20260417.md
```

The activity is inserted at the correct chronological position within the existing progression document.

#### Options

| Option | Description |
|--------|-------------|
| `[path]` | Path to a directory of `.fit` files. If a single `.fit` file is given interactively, switches to add-to-progression mode. |
| `-o, --output <DIR>` | Output directory for the progression file. Defaults to the input directory. |
| `-s, --sport <SPORT>` | Sport to build for (e.g., `Running`, `Cycling`). Prompted if omitted. |
| `--add <FILE>` | Path to a single `.fit` file to add to an existing progression. Requires `--progression-file`. |
| `--progression-file <FILE>` | Path to an existing progression `.md` file. Requires `--add`. |
| `--no-interaction` | Disable interactive prompts. Requires `--sport`. |

#### Examples

```shell
# Build a Running progression non-interactively
ftm progression C:\Activities --sport Running --no-interaction

# Build and output to a specific directory
ftm progression C:\Activities --sport Cycling --output C:\Docs

# Add a new activity to an existing progression
ftm progression --add C:\Activities\new-run.fit --progression-file C:\Docs\Running_Progression_20260417.md
```

#### Progression Document Structure

Each activity within the progression is rendered under an H2 heading with its UTC timestamp:

```markdown
# Running Progression

---

## 2026-04-15 15:20:00 UTC

### Overview
- **Sport:** Running
- **Distance:** 5.01 km / 3.11 mi
- ...

### Session 1: Running
| Metric | Value |
| --- | --- |
| ...

#### Laps
| Lap | Distance | Duration | ... |
| --- | --- | --- | ... |

---

## 2026-04-10 08:30:00 UTC

### Overview
...
```

Activities are separated by `---` horizontal rules and ordered chronologically.

---

### `ftm info` — Inspect FIT Metadata

Display FIT file metadata in a formatted table without generating a full Markdown document.

```shell
ftm info activity.fit
```

Shows: file type, sport, sub-sport, manufacturer, product, serial number, start time, elapsed time, timer time, distance, lap count, and record count.

---

### `ftm version` — Show Version

```shell
ftm version
```

---

## Non-Interactive / Scripting

All commands support `--no-interaction` for use in scripts and CI pipelines. When this flag is present, no interactive prompts are shown — all required parameters must be provided via command-line options.

```shell
# Convert all .fit files, skip existing, no prompts
ftm convert C:\Activities --output C:\Output --overwrite skip --no-interaction

# Build a progression non-interactively
ftm progression C:\Activities --sport Running --output C:\Output --no-interaction
```

## Project Structure

```
FitToMarkdown/
├── src/
│   ├── FitToMarkdown.Core/       # Shared abstractions, models, enums
│   ├── FitToMarkdown.Fit/        # FIT SDK parsing and decoding
│   ├── FitToMarkdown.Markdown/   # Markdown document generation
│   └── FitToMarkdown.Cli/        # CLI tool, commands, interactive UI
├── tests/
│   ├── FitToMarkdown.Fit.Tests/
│   ├── FitToMarkdown.Markdown.Tests/
│   └── FitToMarkdown.Cli.Tests/
├── Directory.Build.props          # Shared build properties
├── Directory.Packages.props       # Central package management
└── FitToMarkdown.Version.props    # Version metadata
```

## Building & Testing

```shell
# Build
dotnet build

# Run all tests
dotnet test

# Run tests for a specific project
dotnet test tests/FitToMarkdown.Cli.Tests
```

## License

[MIT](LICENSE) — Copyright (c) 2026 Copenblend
