# GitHub Pages Setup

This directory contains the documentation that will be served via GitHub Pages.

## Setup Instructions

To enable GitHub Pages for this repository:

1. **Go to Repository Settings**
   - Navigate to your repository on GitHub
   - Click on "Settings" tab
   - Scroll down to "Pages" section in the left sidebar

2. **Configure GitHub Pages**
   - Under "Build and deployment":
     - Source: Select "GitHub Actions"
   - The workflow in `.github/workflows/github-pages.yml` will automatically deploy the documentation

3. **Access Your Documentation**
   - After the workflow runs successfully, your documentation will be available at:
   - `https://jurkflash.github.io/building-blocks/`

## Structure

- `index.md` - Main landing page
- `USAGE_GUIDE.md` - Comprehensive usage guide with examples
- `CONTRIBUTING.md` - Contributing guidelines
- `_config.yml` - Jekyll configuration
- Other documentation files

## Testing Locally

To test the GitHub Pages site locally:

```bash
# Install Jekyll and dependencies
gem install bundler jekyll

# Navigate to docs directory
cd docs

# Serve the site locally
bundle exec jekyll serve

# Visit http://localhost:4000/building-blocks/
```

## Theme

This site uses the Cayman theme, which provides:
- Clean, professional appearance
- Mobile-responsive design
- Syntax highlighting for code blocks
- Easy navigation

## Workflow

The GitHub Actions workflow (`.github/workflows/github-pages.yml`) will:
1. Trigger on pushes to main branch that modify docs
2. Build the Jekyll site from the `docs` folder
3. Deploy to GitHub Pages automatically

No manual deployment is needed after the initial setup!
