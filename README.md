# Mark2

This repository contains the `Mark2` Unity project files.

Contents:
- Unity project source under `Assets/` and project settings
- Notebook and other files (if any)

Quick start — prepare and push to GitHub (terminal)

1. Change to the project folder (adjust path if different):

```zsh
cd "/Users/thilak/PythonFiles/Sem 7/Unity Projects/Mark2"
```

2. Initialize git, add files, and make the initial commit:

```zsh
git init
git add .
git commit -m "Initial commit — Add Mark2 project"
```

3. Create the GitHub repo and push (recommended: use GitHub CLI `gh` if installed):

Using `gh` (easiest):
```zsh
gh repo create thilak0105/Mark2 --public --source=. --remote=origin --push
```
Use `--private` instead of `--public` for a private repo.

Manual (create repo on github.com and then):
```zsh
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/Mark2.git
git push -u origin main
```

4. If you have files larger than 100 MB, install Git LFS and track them before pushing:

```zsh
brew install git-lfs
git lfs install
# Example: track PSD files
git lfs track "*.psd"
git add .gitattributes
git add <large-files>
git commit -m "Track large files with Git LFS"
git push
```

Using GitHub Desktop (GUI):
- File → Add local repository → Select this folder → Commit changes → Publish repository → set visibility → Publish.

If this path is incorrect, please tell me the correct absolute path and I will update the files accordingly.

---
If you'd like, I can also prepare a small `.gitattributes` for LFS and run the git commands locally (I can only create files here; you'll need to run push commands on your Mac or give me confirmation to provide copy-paste commands). 