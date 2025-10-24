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

## How to run (short)

1. Unity editor
	- Recommended Unity version: see `ProjectSettings/ProjectVersion.txt` (this project: 6000.1.14f1). Open the project with that version (or a compatible newer editor). Unity will regenerate `Library/` on first open.

2. Model for inference
	- The ONNX model used by the agent is stored at `Assets/TrainedModels/WarehouseAgent.onnx` in this repo. Unity with Barracuda (or ML-Agents inference) will load this file at runtime.

3. Python / ML-Agents (training)
	- A `requirements.txt` was added containing the Python packages used for training. To recreate the training environment:

```zsh
# create a venv (example)
python3 -m venv mlagents-env-py
source mlagents-env-py/bin/activate
pip install --upgrade pip
pip install -r requirements.txt
```

	- Note: the original `mlagents-env` in this workspace is Windows-style; the `requirements.txt` was generated from its installed packages. Adjust Python versions if necessary.

4. If you need large checkpoints or many model files
	- Keep only the final ONNX(s) required for inference inside `Assets/` to keep the repo small. For large checkpoints (`*.pt`, very large `.onnx`) use Git LFS:

```zsh
brew install git-lfs
git lfs install
git lfs track "*.pt" "*.onnx"
git add .gitattributes
```

5. Quick checklist to open & run locally
	- Clone repo, open in Unity (matching version), confirm `Packages/manifest.json` installs dependencies, and run the scene `Assets/Scenes/SampleScene.unity` or the project scene specified in `Assets`.

If you'd like, I can add a short developer-runner script or expand the README with step-by-step instructions for training or inference. Tell me what you prefer.