# Module Manifest Schema

Every module in the Pokok Building Blocks repository contains a `MODULE_MANIFEST.yaml` file that provides structural, semantic, and behavioral metadata. This makes each module **self-describing** — readable by both humans and AI systems.

## Why Manifests?

- **Discoverability**: Understand a module's purpose, API surface, and constraints without reading every source file.
- **Onboarding**: New contributors learn rules of engagement and failure modes before writing code.
- **AI-Friendly**: Structured YAML enables automated analysis, code generation, and intelligent tooling.
- **Contracts**: Behavioral contracts and invariants are documented alongside the code they govern.

## Schema Reference

```yaml
# ──────────────────────────────────────────────
# MODULE IDENTITY
# ──────────────────────────────────────────────
module:
  name: string            # NuGet package / project name
  namespace: string       # Root CLR namespace
  purpose: string         # One-sentence module description
  layer: enum             # domain | infrastructure | application | shared-kernel
  patterns:               # Architectural patterns implemented
    - string

# ──────────────────────────────────────────────
# DEPENDENCIES
# ──────────────────────────────────────────────
dependencies:
  projects:               # Sibling project references
    - name: string
      relationship: string  # Why this dependency exists
  packages:               # NuGet package dependencies
    - name: string
      purpose: string       # Why this package is needed

# ──────────────────────────────────────────────
# PUBLIC API SURFACE
# ──────────────────────────────────────────────
public_api:
  interfaces:
    - name: string
      purpose: string
      methods:
        - signature: string
          behavior: string
  classes:
    - name: string
      kind: enum           # abstract | sealed | static | concrete
      purpose: string
  exceptions:
    - name: string
      thrown_when: string
      recovery: string

# ──────────────────────────────────────────────
# BEHAVIORAL CONTRACTS
#   Guarantees the module makes to its consumers.
# ──────────────────────────────────────────────
behavioral_contracts:
  - name: string
    description: string
    enforced_by: string      # Code mechanism that enforces this

# ──────────────────────────────────────────────
# FAILURE MODES
#   Everything that can go wrong and what happens.
# ──────────────────────────────────────────────
failure_modes:
  - name: string
    trigger: string          # What causes this failure
    exception: string        # Exception type thrown (if any)
    impact: string           # What effect this has
    recovery: string         # How to recover or prevent

# ──────────────────────────────────────────────
# RULES OF ENGAGEMENT
#   What consumers MUST do to use this module
#   correctly.
# ──────────────────────────────────────────────
rules_of_engagement:
  - rule: string
    rationale: string

# ──────────────────────────────────────────────
# EXTENSION POINTS
#   How consumers can extend or customize behavior.
# ──────────────────────────────────────────────
extension_points:
  - name: string
    type: enum             # interface | abstract-class | virtual-method | event
    description: string

# ──────────────────────────────────────────────
# SERVICE REGISTRATION
#   How to wire this module into DI containers.
# ──────────────────────────────────────────────
service_registration:
  entry_point: string        # Extension method or class name
  example: string            # Code snippet
  registered_services:
    - interface: string
      implementation: string
      lifetime: enum         # singleton | scoped | transient
```

## Conventions

| Convention | Description |
|---|---|
| File location | `src/<ModuleName>/MODULE_MANIFEST.yaml` |
| Format | YAML 1.2, UTF-8 |
| Required sections | `module`, `dependencies`, `public_api`, `behavioral_contracts`, `failure_modes`, `rules_of_engagement` |
| Optional sections | `extension_points`, `service_registration` |
| Updates | Manifests must be updated when the public API or contracts change |
